using System;

namespace LegendsOfRus.SaveCore
{
    public sealed class SaveJournalRepository
    {
        readonly ISaveJournalStore store;
        readonly SaveEnvelopeCodec codec;
        readonly ILegacySavePayloadInspector legacyInspector;
        readonly SaveJournalKeys keys;
        readonly int maximumSupportedPayloadVersion;

        public SaveJournalRepository(
            ISaveJournalStore store,
            SaveEnvelopeCodec codec,
            ILegacySavePayloadInspector legacyInspector,
            SaveJournalKeys keys,
            int maximumSupportedPayloadVersion)
        {
            this.store = store ?? throw new ArgumentNullException("store");
            this.codec = codec ?? throw new ArgumentNullException("codec");
            this.legacyInspector = legacyInspector ?? throw new ArgumentNullException("legacyInspector");
            this.keys = keys ?? throw new ArgumentNullException("keys");
            if (maximumSupportedPayloadVersion <= 0)
                throw new ArgumentOutOfRangeException("maximumSupportedPayloadVersion");
            this.maximumSupportedPayloadVersion = maximumSupportedPayloadVersion;
        }

        public SaveJournalLoadResult Load()
        {
            JournalScan scan = Scan();
            return DecideLoad(scan);
        }

        public SaveJournalWriteResult TrySave(string payload, int payloadVersion)
        {
            return TryWrite(payload, payloadVersion, SaveJournalWriteMode.Ordinary);
        }

        public SaveJournalWriteResult TryReplaceHistory(string payload, int payloadVersion)
        {
            return TryWrite(payload, payloadVersion, SaveJournalWriteMode.ReplaceHistory);
        }

        SaveJournalWriteResult TryWrite(
            string payload,
            int payloadVersion,
            SaveJournalWriteMode mode)
        {
            if (payload == null || payloadVersion <= 0 || payloadVersion > maximumSupportedPayloadVersion)
            {
                return WriteFailure(
                    SaveJournalWriteStatus.InvalidInput,
                    SaveJournalWritePhase.None,
                    mode,
                    0,
                    "SAVE_WRITE_INPUT_INVALID");
            }

            JournalScan scan = Scan();
            if (scan.HasStorageFailure)
            {
                return WriteFailure(
                    SaveJournalWriteStatus.StorageFailure,
                    SaveJournalWritePhase.Scan,
                    mode,
                    0,
                    "SAVE_WRITE_SCAN_FAILED");
            }
            if (scan.HasUnsupportedFuture)
            {
                return WriteFailure(
                    SaveJournalWriteStatus.UnsupportedFutureVersion,
                    SaveJournalWritePhase.Scan,
                    mode,
                    0,
                    "SAVE_WRITE_BLOCKED_BY_FUTURE_VERSION");
            }

            // A missing authoritative primary plus any companion evidence denotes a
            // new/reset history boundary. Even a direct ordinary core call must replace
            // that companion history so later primary corruption cannot resurrect it.
            if (mode == SaveJournalWriteMode.Ordinary
                && scan.Primary.State == SaveCandidateState.Missing
                && (scan.Backup.State != SaveCandidateState.Missing
                    || scan.Staging.State != SaveCandidateState.Missing))
            {
                mode = SaveJournalWriteMode.ReplaceHistory;
            }

            long highestRevision = Math.Max(
                scan.Primary.Valid ? scan.Primary.Revision : 0,
                Math.Max(
                    scan.Backup.Valid ? scan.Backup.Revision : 0,
                    scan.Staging.Valid ? scan.Staging.Revision : 0));
            if (highestRevision == long.MaxValue)
            {
                return WriteFailure(
                    SaveJournalWriteStatus.RevisionExhausted,
                    SaveJournalWritePhase.None,
                    mode,
                    0,
                    "SAVE_REVISION_EXHAUSTED");
            }

            long nextRevision = highestRevision + 1;
            SaveEnvelopeEncodeResult encoded = codec.TryEncode(
                payload,
                payloadVersion,
                nextRevision,
                mode == SaveJournalWriteMode.ReplaceHistory
                    ? SaveEnvelopeIntent.ReplaceHistory
                    : SaveEnvelopeIntent.Ordinary);
            if (!encoded.Success)
            {
                return WriteFailure(
                    encoded.Status == SaveEnvelopeEncodeStatus.InvalidInput
                        ? SaveJournalWriteStatus.InvalidInput
                        : SaveJournalWriteStatus.EncodingFailure,
                    SaveJournalWritePhase.None,
                    mode,
                    0,
                    "SAVE_ENVELOPE_ENCODING_FAILED");
            }

            SaveJournalWriteResult failure;
            if (!TryStoreWrite(keys.StagingKey, encoded.Envelope, mode, nextRevision,
                    SaveJournalWritePhase.StagingWrite, "SAVE_STAGING_WRITE_FAILED", out failure))
                return failure;
            if (!TryStoreFlush(mode, nextRevision, SaveJournalWritePhase.StagingFlush,
                    "SAVE_STAGING_FLUSH_FAILED", out failure))
                return failure;

            if (mode == SaveJournalWriteMode.Ordinary && scan.Primary.Valid)
            {
                if (!TryStoreWrite(keys.BackupKey, scan.Primary.Raw, mode, nextRevision,
                        SaveJournalWritePhase.BackupWrite, "SAVE_BACKUP_WRITE_FAILED", out failure))
                    return failure;
                if (!TryStoreFlush(mode, nextRevision, SaveJournalWritePhase.BackupFlush,
                        "SAVE_BACKUP_FLUSH_FAILED", out failure))
                    return failure;
            }

            if (!TryStoreWrite(keys.PrimaryKey, encoded.Envelope, mode, nextRevision,
                    SaveJournalWritePhase.PrimaryWrite, "SAVE_PRIMARY_WRITE_FAILED", out failure))
                return failure;
            if (!TryStoreFlush(mode, nextRevision, SaveJournalWritePhase.PrimaryFlush,
                    "SAVE_PRIMARY_FLUSH_FAILED", out failure))
                return failure;

            if (mode == SaveJournalWriteMode.ReplaceHistory)
            {
                if (!TryStoreWrite(keys.BackupKey, encoded.Envelope, mode, nextRevision,
                        SaveJournalWritePhase.BackupWrite, "SAVE_BACKUP_WRITE_FAILED", out failure))
                    return MarkPrimaryDurablyCommitted(failure);
                if (!TryStoreFlush(mode, nextRevision, SaveJournalWritePhase.BackupFlush,
                        "SAVE_BACKUP_FLUSH_FAILED", out failure))
                    return MarkPrimaryDurablyCommitted(failure);
            }

            if (!TryStoreDelete(keys.StagingKey, mode, nextRevision,
                    SaveJournalWritePhase.StagingDelete, "SAVE_STAGING_DELETE_FAILED", out failure))
                return MarkPrimaryDurablyCommitted(failure);
            if (!TryStoreFlush(mode, nextRevision, SaveJournalWritePhase.StagingDeleteFlush,
                    "SAVE_STAGING_DELETE_FLUSH_FAILED", out failure))
                return MarkPrimaryDurablyCommitted(failure);

            return new SaveJournalWriteResult(
                SaveJournalWriteStatus.Success,
                SaveJournalWritePhase.None,
                mode,
                nextRevision,
                mode == SaveJournalWriteMode.ReplaceHistory
                    ? "SAVE_HISTORY_REPLACED"
                    : "SAVE_COMMITTED",
                true);
        }

        JournalScan Scan()
        {
            Candidate primary = ReadCandidate(keys.PrimaryKey, SaveJournalSlot.Primary);
            Candidate backup = ReadCandidate(keys.BackupKey, SaveJournalSlot.Backup);
            Candidate staging = ReadCandidate(keys.StagingKey, SaveJournalSlot.Staging);
            return new JournalScan(primary, backup, staging);
        }

        Candidate ReadCandidate(string key, SaveJournalSlot slot)
        {
            string raw;
            try
            {
                if (!store.TryRead(key, out raw))
                    return Candidate.Missing(slot);
            }
            catch (Exception)
            {
                return Candidate.Failure(slot);
            }

            if (raw == null)
                return Candidate.Invalid(slot);

            SaveEnvelopeDecodeResult decoded = codec.Decode(raw, maximumSupportedPayloadVersion);
            if (decoded.Success)
            {
                return Candidate.ValidEnvelope(
                    slot,
                    raw,
                    decoded.Payload,
                    decoded.PayloadVersion,
                    decoded.Revision,
                    decoded.Intent);
            }
            if (decoded.Status == SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion
                || decoded.Status == SaveEnvelopeDecodeStatus.UnsupportedPayloadVersion)
            {
                return Candidate.Unsupported(slot);
            }
            if (decoded.Status != SaveEnvelopeDecodeStatus.NotEnvelope)
                return Candidate.Invalid(slot);

            LegacyPayloadInspection inspected;
            try
            {
                inspected = legacyInspector.Inspect(raw);
            }
            catch (Exception)
            {
                return Candidate.Invalid(slot);
            }

            if (inspected == null
                || inspected.Status != LegacyPayloadInspectionStatus.Valid
                || inspected.PayloadVersion <= 0)
            {
                return Candidate.Invalid(slot);
            }
            if (inspected.PayloadVersion > maximumSupportedPayloadVersion)
                return Candidate.Unsupported(slot);
            return Candidate.ValidLegacy(slot, raw, inspected.PayloadVersion);
        }

        static SaveJournalLoadResult DecideLoad(JournalScan scan)
        {
            if (scan.HasStorageFailure)
                return LoadFailure(scan, SaveJournalLoadStatus.StorageFailure, "SAVE_STORAGE_READ_FAILED");
            if (scan.HasUnsupportedFuture)
            {
                return LoadFailure(
                    scan,
                    SaveJournalLoadStatus.UnsupportedFutureVersion,
                    "SAVE_FUTURE_VERSION_BLOCKED");
            }

            // Staging is only evidence of an interrupted write. It is never promoted by Load.
            // A supported, valid primary is the sole authoritative committed candidate.
            if (scan.Primary.Valid)
                return LoadSuccess(scan, scan.Primary, SaveJournalLoadStatus.LoadedPrimary, "SAVE_PRIMARY_LOADED");

            // Missing primary is an explicit reset/no-save state. Companion keys never
            // resurrect history when the authoritative key has been deliberately removed.
            if (scan.Primary.State == SaveCandidateState.Missing)
            {
                return LoadFailure(
                    scan,
                    SaveJournalLoadStatus.NoSave,
                    "SAVE_PRIMARY_MISSING_NO_COMMITTED_SAVE");
            }

            // Ordinary interrupted writes retain last-known-good backup recovery. Only an
            // interrupted history replacement blocks an older backup from resurrecting the
            // deliberately replaced state after primary corruption.
            if (scan.Staging.Valid
                && scan.Staging.Intent == SaveEnvelopeIntent.ReplaceHistory
                && (!scan.Backup.Valid
                    || scan.Staging.Revision > scan.Backup.Revision
                    || (scan.Staging.Revision == scan.Backup.Revision
                        && !string.Equals(
                            scan.Staging.Raw,
                            scan.Backup.Raw,
                            StringComparison.Ordinal))))
            {
                return LoadFailure(
                    scan,
                    SaveJournalLoadStatus.Unrecoverable,
                    "SAVE_HISTORY_REPLACEMENT_INTERRUPTED");
            }

            if (scan.Backup.Valid)
            {
                return LoadSuccess(
                    scan,
                    scan.Backup,
                    SaveJournalLoadStatus.RecoveredBackup,
                    "SAVE_BACKUP_RECOVERED_PRIMARY_INVALID");
            }

            return LoadFailure(scan, SaveJournalLoadStatus.Unrecoverable, "SAVE_NO_VALID_COMMITTED_CANDIDATE");
        }

        static SaveJournalLoadResult LoadSuccess(
            JournalScan scan,
            Candidate candidate,
            SaveJournalLoadStatus status,
            string diagnosticCode)
        {
            return new SaveJournalLoadResult(
                status,
                candidate.Slot,
                candidate.Format,
                candidate.Payload,
                candidate.PayloadVersion,
                candidate.Revision,
                scan.Primary.State,
                scan.Backup.State,
                scan.Staging.State,
                diagnosticCode);
        }

        static SaveJournalLoadResult LoadFailure(
            JournalScan scan,
            SaveJournalLoadStatus status,
            string diagnosticCode)
        {
            return new SaveJournalLoadResult(
                status,
                SaveJournalSlot.None,
                SavePayloadFormat.None,
                null,
                0,
                0,
                scan.Primary.State,
                scan.Backup.State,
                scan.Staging.State,
                diagnosticCode);
        }

        bool TryStoreWrite(
            string key,
            string value,
            SaveJournalWriteMode mode,
            long revision,
            SaveJournalWritePhase phase,
            string diagnosticCode,
            out SaveJournalWriteResult failure)
        {
            try
            {
                store.Write(key, value);
                failure = null;
                return true;
            }
            catch (Exception)
            {
                failure = WriteFailure(
                    SaveJournalWriteStatus.StorageFailure,
                    phase,
                    mode,
                    revision,
                    diagnosticCode);
                return false;
            }
        }

        bool TryStoreDelete(
            string key,
            SaveJournalWriteMode mode,
            long revision,
            SaveJournalWritePhase phase,
            string diagnosticCode,
            out SaveJournalWriteResult failure)
        {
            try
            {
                store.Delete(key);
                failure = null;
                return true;
            }
            catch (Exception)
            {
                failure = WriteFailure(
                    SaveJournalWriteStatus.StorageFailure,
                    phase,
                    mode,
                    revision,
                    diagnosticCode);
                return false;
            }
        }

        bool TryStoreFlush(
            SaveJournalWriteMode mode,
            long revision,
            SaveJournalWritePhase phase,
            string diagnosticCode,
            out SaveJournalWriteResult failure)
        {
            try
            {
                store.Flush();
                failure = null;
                return true;
            }
            catch (Exception)
            {
                failure = WriteFailure(
                    SaveJournalWriteStatus.StorageFailure,
                    phase,
                    mode,
                    revision,
                    diagnosticCode);
                return false;
            }
        }

        static SaveJournalWriteResult WriteFailure(
            SaveJournalWriteStatus status,
            SaveJournalWritePhase phase,
            SaveJournalWriteMode mode,
            long revision,
            string diagnosticCode)
        {
            return new SaveJournalWriteResult(status, phase, mode, revision, diagnosticCode);
        }

        static SaveJournalWriteResult MarkPrimaryDurablyCommitted(
            SaveJournalWriteResult failure)
        {
            return new SaveJournalWriteResult(
                failure.Status,
                failure.Phase,
                failure.Mode,
                failure.Revision,
                failure.DiagnosticCode,
                true);
        }

        sealed class JournalScan
        {
            public JournalScan(Candidate primary, Candidate backup, Candidate staging)
            {
                Primary = primary;
                Backup = backup;
                Staging = staging;
            }

            public Candidate Primary { get; private set; }
            public Candidate Backup { get; private set; }
            public Candidate Staging { get; private set; }
            public bool HasUnsupportedFuture
            {
                get
                {
                    return Primary.State == SaveCandidateState.UnsupportedFuture
                        || Backup.State == SaveCandidateState.UnsupportedFuture
                        || Staging.State == SaveCandidateState.UnsupportedFuture;
                }
            }
            public bool HasStorageFailure
            {
                get
                {
                    return Primary.State == SaveCandidateState.StorageFailure
                        || Backup.State == SaveCandidateState.StorageFailure
                        || Staging.State == SaveCandidateState.StorageFailure;
                }
            }
        }

        sealed class Candidate
        {
            Candidate(
                SaveJournalSlot slot,
                SaveCandidateState state,
                SavePayloadFormat format,
                string raw,
                string payload,
                int payloadVersion,
                long revision,
                SaveEnvelopeIntent intent)
            {
                Slot = slot;
                State = state;
                Format = format;
                Raw = raw;
                Payload = payload;
                PayloadVersion = payloadVersion;
                Revision = revision;
                Intent = intent;
            }

            public SaveJournalSlot Slot { get; private set; }
            public SaveCandidateState State { get; private set; }
            public SavePayloadFormat Format { get; private set; }
            public string Raw { get; private set; }
            public string Payload { get; private set; }
            public int PayloadVersion { get; private set; }
            public long Revision { get; private set; }
            public SaveEnvelopeIntent Intent { get; private set; }
            public bool Valid { get { return State == SaveCandidateState.Valid; } }

            public static Candidate Missing(SaveJournalSlot slot)
            {
                return new Candidate(slot, SaveCandidateState.Missing, SavePayloadFormat.None, null, null, 0, 0, SaveEnvelopeIntent.Ordinary);
            }

            public static Candidate Invalid(SaveJournalSlot slot)
            {
                return new Candidate(slot, SaveCandidateState.Invalid, SavePayloadFormat.None, null, null, 0, 0, SaveEnvelopeIntent.Ordinary);
            }

            public static Candidate Unsupported(SaveJournalSlot slot)
            {
                return new Candidate(
                    slot,
                    SaveCandidateState.UnsupportedFuture,
                    SavePayloadFormat.None,
                    null,
                    null,
                    0,
                    0,
                    SaveEnvelopeIntent.Ordinary);
            }

            public static Candidate Failure(SaveJournalSlot slot)
            {
                return new Candidate(
                    slot,
                    SaveCandidateState.StorageFailure,
                    SavePayloadFormat.None,
                    null,
                    null,
                    0,
                    0,
                    SaveEnvelopeIntent.Ordinary);
            }

            public static Candidate ValidEnvelope(
                SaveJournalSlot slot,
                string raw,
                string payload,
                int payloadVersion,
                long revision,
                SaveEnvelopeIntent intent)
            {
                return new Candidate(
                    slot,
                    SaveCandidateState.Valid,
                    SavePayloadFormat.EnvelopeV1,
                    raw,
                    payload,
                    payloadVersion,
                    revision,
                    intent);
            }

            public static Candidate ValidLegacy(SaveJournalSlot slot, string raw, int payloadVersion)
            {
                return new Candidate(
                    slot,
                    SaveCandidateState.Valid,
                    SavePayloadFormat.Legacy,
                    raw,
                    raw,
                    payloadVersion,
                    0,
                    SaveEnvelopeIntent.Ordinary);
            }
        }
    }
}
