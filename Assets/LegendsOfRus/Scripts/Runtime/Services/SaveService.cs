using LegendsOfRus.SaveCore;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Compatibility facade over a versioned PlayerPrefs journal. Load is
    /// deliberately read-only; migration and recovery rewrites are explicit.
    /// </summary>
    public static class SaveService
    {
        const string Key = "LegendsOfRus_Save_v1";
        internal const string BackupKey = "LegendsOfRus_Save_v1_Backup";
        internal const string StagingKey = "LegendsOfRus_Save_v1_Staging";

        public static PlayerSaveData Load()
        {
            return LoadWithResult().Data;
        }

        public static PlayerSaveLoadResult LoadWithResult()
        {
            SaveJournalLoadResult journal = CreateRepository().Load();
            switch (journal.Status)
            {
                case SaveJournalLoadStatus.LoadedPrimary:
                case SaveJournalLoadStatus.RecoveredBackup:
                    return Decode(journal);

                case SaveJournalLoadStatus.NoSave:
                    return new PlayerSaveLoadResult(
                        PlayerSaveLoadStatus.FirstRun,
                        new PlayerSaveData(),
                        PlayerSaveMigrationRegistry.CurrentPayloadVersion,
                        0,
                        true,
                        journal.DiagnosticCode
                    );

                case SaveJournalLoadStatus.UnsupportedFutureVersion:
                    return Failure(
                        PlayerSaveLoadStatus.UnsupportedFutureVersion,
                        journal.DiagnosticCode
                    );

                case SaveJournalLoadStatus.StorageFailure:
                    return Failure(
                        PlayerSaveLoadStatus.StorageFailure,
                        journal.DiagnosticCode
                    );

                default:
                    return Failure(
                        PlayerSaveLoadStatus.InvalidPayload,
                        journal.DiagnosticCode
                    );
            }
        }

        public static void Save(PlayerSaveData data)
        {
            PlayerSaveWriteResult result;
            if (!TrySave(data, out result))
                Debug.LogError("Player save write blocked: " + result);
        }

        public static bool TrySave(
            PlayerSaveData data,
            out PlayerSaveWriteResult result)
        {
            return TryWrite(data, false, out result);
        }

        public static void SaveReset(PlayerSaveData data)
        {
            PlayerSaveWriteResult result;
            if (!TryReplaceHistory(data, out result)
                && !result.PrimaryDurablyCommitted)
                Debug.LogError("Player save reset write blocked: " + result);
            else if (!result.Success)
                Debug.LogWarning(
                    "Player save reset primary committed; journal cleanup incomplete: " +
                    result
                );
        }

        public static bool TryReplaceHistory(
            PlayerSaveData data,
            out PlayerSaveWriteResult result)
        {
            return TryWrite(data, true, out result);
        }

        static bool TryWrite(
            PlayerSaveData data,
            bool replaceHistory,
            out PlayerSaveWriteResult result)
        {
            if (data == null
                || data.saveVersion != PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                result = new PlayerSaveWriteResult(
                    PlayerSaveWriteStatus.InvalidInput,
                    "None",
                    0,
                    "SAVE_CURRENT_PAYLOAD_REQUIRED"
                );
                return false;
            }

            string payload;
            try
            {
                payload = JsonUtility.ToJson(data);
            }
            catch
            {
                result = new PlayerSaveWriteResult(
                    PlayerSaveWriteStatus.EncodingFailure,
                    "None",
                    0,
                    "SAVE_PAYLOAD_SERIALIZATION_FAILED"
                );
                return false;
            }

            PlayerSaveData roundTrip;
            string payloadCode;
            if (!PlayerSavePayloadCodec.TryDeserialize(
                    payload,
                    PlayerSaveMigrationRegistry.CurrentPayloadVersion,
                    false,
                    out roundTrip,
                    out payloadCode))
            {
                result = new PlayerSaveWriteResult(
                    PlayerSaveWriteStatus.InvalidInput,
                    "None",
                    0,
                    payloadCode
                );
                return false;
            }
            if (roundTrip == null
                || roundTrip.saveVersion
                    != PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                result = new PlayerSaveWriteResult(
                    PlayerSaveWriteStatus.InvalidInput,
                    "None",
                    0,
                    "SAVE_PAYLOAD_ROUNDTRIP_INVALID"
                );
                return false;
            }

            SaveJournalRepository repository = CreateRepository();
            SaveJournalLoadResult current = repository.Load();
            if (current.Status == SaveJournalLoadStatus.UnsupportedFutureVersion)
            {
                result = new PlayerSaveWriteResult(
                    PlayerSaveWriteStatus.WriteBlocked,
                    "Scan",
                    0,
                    current.DiagnosticCode
                );
                return false;
            }
            if (current.Status == SaveJournalLoadStatus.Unrecoverable
                || current.Status == SaveJournalLoadStatus.StorageFailure)
            {
                result = new PlayerSaveWriteResult(
                    current.Status == SaveJournalLoadStatus.StorageFailure
                        ? PlayerSaveWriteStatus.StorageFailure
                        : PlayerSaveWriteStatus.WriteBlocked,
                    "Scan",
                    0,
                    current.DiagnosticCode
                );
                return false;
            }

            if (current.Success)
            {
                PlayerSaveData currentData;
                string currentCode;
                bool allowMissingVersion =
                    current.Format == SavePayloadFormat.Legacy;
                int declaredVersion;
                if (PlayerSavePayloadCodec.TryInspectVersion(
                        current.Payload,
                        allowMissingVersion,
                        out declaredVersion,
                        out currentCode)
                    && declaredVersion
                        > PlayerSaveMigrationRegistry.CurrentPayloadVersion)
                {
                    result = new PlayerSaveWriteResult(
                        PlayerSaveWriteStatus.WriteBlocked,
                        "Scan",
                        0,
                        PlayerSaveMigrationRegistry.FutureVersionBlocked
                    );
                    return false;
                }
                if (!PlayerSavePayloadCodec.TryDeserialize(
                        current.Payload,
                        current.PayloadVersion,
                        allowMissingVersion,
                        out currentData,
                        out currentCode))
                {
                    result = new PlayerSaveWriteResult(
                        PlayerSaveWriteStatus.WriteBlocked,
                        "Scan",
                        0,
                        currentCode
                    );
                    return false;
                }
                if (currentData.saveVersion
                    > PlayerSaveMigrationRegistry.CurrentPayloadVersion)
                {
                    result = new PlayerSaveWriteResult(
                        PlayerSaveWriteStatus.WriteBlocked,
                        "Scan",
                        0,
                        PlayerSaveMigrationRegistry.FutureVersionBlocked
                    );
                    return false;
                }
            }

            // A primary-missing namespace is an explicit new history. Replacing
            // both committed slots prevents a stale backup from becoming the
            // recovery source if this new primary is later damaged.
            bool replaceCommittedHistory = replaceHistory
                || current.Status == SaveJournalLoadStatus.NoSave;
            SaveJournalWriteResult journal = replaceCommittedHistory
                ? repository.TryReplaceHistory(
                    payload,
                    PlayerSaveMigrationRegistry.CurrentPayloadVersion)
                : repository.TrySave(
                    payload,
                    PlayerSaveMigrationRegistry.CurrentPayloadVersion);

            result = Map(journal);
            return result.Success;
        }

        static PlayerSaveLoadResult Decode(SaveJournalLoadResult journal)
        {
            PlayerSaveData data;
            string payloadCode;
            bool legacy = journal.Format == SavePayloadFormat.Legacy;
            int declaredVersion;
            if (PlayerSavePayloadCodec.TryInspectVersion(
                    journal.Payload,
                    legacy,
                    out declaredVersion,
                    out payloadCode)
                && declaredVersion
                    > PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                return Failure(
                    PlayerSaveLoadStatus.UnsupportedFutureVersion,
                    PlayerSaveMigrationRegistry.FutureVersionBlocked
                );
            }
            if (!PlayerSavePayloadCodec.TryDeserialize(
                    journal.Payload,
                    journal.PayloadVersion,
                    legacy,
                    out data,
                    out payloadCode))
            {
                return Failure(PlayerSaveLoadStatus.InvalidPayload, payloadCode);
            }

            if (data.saveVersion > PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                return Failure(
                    PlayerSaveLoadStatus.UnsupportedFutureVersion,
                    PlayerSaveMigrationRegistry.FutureVersionBlocked
                );
            }

            // Unity inline serialization can materialize a null optional class as an
            // empty object. An absent identity is the explicit cleared-checkpoint sentinel.
            if (data.retainedRound != null && string.IsNullOrEmpty(data.retainedRound.attemptId))
                data.retainedRound = null;

            bool recovered = journal.Status == SaveJournalLoadStatus.RecoveredBackup;
            return new PlayerSaveLoadResult(
                recovered
                    ? PlayerSaveLoadStatus.RecoveredBackup
                    : PlayerSaveLoadStatus.LoadedPrimary,
                data,
                data.saveVersion,
                journal.Revision,
                legacy
                    || recovered
                    || data.saveVersion
                        < PlayerSaveMigrationRegistry.CurrentPayloadVersion,
                journal.DiagnosticCode
            );
        }

        static PlayerSaveLoadResult Failure(
            PlayerSaveLoadStatus status,
            string diagnosticCode)
        {
            return new PlayerSaveLoadResult(
                status,
                new PlayerSaveData(),
                0,
                0,
                false,
                diagnosticCode
            );
        }

        static PlayerSaveWriteResult Map(SaveJournalWriteResult journal)
        {
            PlayerSaveWriteStatus status;
            switch (journal.Status)
            {
                case SaveJournalWriteStatus.Success:
                    status = PlayerSaveWriteStatus.Success;
                    break;
                case SaveJournalWriteStatus.InvalidInput:
                    status = PlayerSaveWriteStatus.InvalidInput;
                    break;
                case SaveJournalWriteStatus.UnsupportedFutureVersion:
                    status = PlayerSaveWriteStatus.WriteBlocked;
                    break;
                case SaveJournalWriteStatus.RevisionExhausted:
                    status = PlayerSaveWriteStatus.RevisionExhausted;
                    break;
                case SaveJournalWriteStatus.EncodingFailure:
                    status = PlayerSaveWriteStatus.EncodingFailure;
                    break;
                default:
                    status = PlayerSaveWriteStatus.StorageFailure;
                    break;
            }

            return new PlayerSaveWriteResult(
                status,
                journal.Phase.ToString(),
                journal.Revision,
                journal.DiagnosticCode,
                journal.PrimaryDurablyCommitted
            );
        }

        static SaveJournalRepository CreateRepository()
        {
            return new SaveJournalRepository(
                new PlayerPrefsSaveJournalStore(),
                new SaveEnvelopeCodec(),
                new PlayerSavePayloadCodec(),
                new SaveJournalKeys(Key, BackupKey, StagingKey),
                PlayerSaveMigrationRegistry.CurrentPayloadVersion
            );
        }
    }
}
