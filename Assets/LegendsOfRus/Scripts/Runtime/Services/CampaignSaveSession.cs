using System;
using System.Linq;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus
{
    public sealed class CampaignSaveOperation
    {
        public bool Committed, NoOp;
        public string Diagnostic;
        public SaveJournalWriteResult JournalWrite;
    }

    // Explicit development composition. No default PlayerPrefs store, bootstrap or live keys.
    public sealed partial class CampaignSaveSession
    {
        bool storageUncertain;
        readonly SaveJournalRepository journal;
        readonly CampaignDefinition definition;
        readonly ValidatedCampaignCatalog catalog;
        public static SaveJournalKeys DevelopmentKeys() => new SaveJournalKeys(
            "lor.campaign.dev.primary", "lor.campaign.dev.backup", "lor.campaign.dev.staging");
        public CampaignSaveSession(ISaveJournalStore store, CampaignDefinition definition, ValidatedCampaignCatalog catalog)
        {
            if (CampaignContract.Validate(definition).Count != 0) throw new ArgumentException("Invalid campaign definition.");
            this.definition = JsonUtility.FromJson<CampaignDefinition>(JsonUtility.ToJson(definition));
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (catalog.ContentRevision != definition.ContentRevision) throw new ArgumentException("Catalog/save revision mismatch.");
            journal = new SaveJournalRepository(store, new SaveEnvelopeCodec(), new PlayerSavePayloadCodec(), DevelopmentKeys(), CampaignPlayerSaveCodec.PayloadVersion);
        }
        static CampaignSaveOperation Blocked(string reason) => new CampaignSaveOperation { Diagnostic = reason };

        public CampaignSaveOperation ImportLegacyCopy(string suppliedCopy)
        {
            if (storageUncertain) return Blocked("campaign_storage_restart_required");
            CampaignPlayerSave target;
            try { target = CampaignPlayerSaveCodec.FromLegacy(suppliedCopy, definition); }
            catch (ArgumentException e) { return Blocked(e.Message); }
            var current = journal.Load();
            if (current.Status == SaveJournalLoadStatus.NoSave)
            {
                var seed = journal.TrySave(suppliedCopy, 6);
                if (!seed.Success && !seed.PrimaryDurablyCommitted)
                {
                    storageUncertain = true;
                    return new CampaignSaveOperation { Diagnostic = seed.DiagnosticCode, JournalWrite = seed };
                }
                current = journal.Load();
            }
            if (!current.Success) return Blocked(current.DiagnosticCode);
            if (current.PayloadVersion == CampaignPlayerSaveCodec.PayloadVersion)
            {
                if (!CampaignPlayerSaveCodec.TryDecode(current.Payload, definition, out var existing, out var error)) return Blocked(error);
                return existing.legacySourceJson == suppliedCopy
                    ? new CampaignSaveOperation { Committed = true, NoOp = true, Diagnostic = "campaign_already_migrated" }
                    : Blocked("campaign_migration_source_conflict");
            }
            if (current.PayloadVersion != 6 || current.Payload != suppliedCopy) return Blocked("campaign_migration_source_conflict");
            return Commit(target);
        }
        public CampaignSaveOperation MigrateLegacy()
        {
            if (storageUncertain) return Blocked("campaign_storage_restart_required");
            var current = journal.Load();
            if (!current.Success) return Blocked(current.DiagnosticCode);
            if (current.PayloadVersion == CampaignPlayerSaveCodec.PayloadVersion)
                return CampaignPlayerSaveCodec.TryDecode(current.Payload, definition, out _, out var error)
                    ? new CampaignSaveOperation { Committed = true, NoOp = true, Diagnostic = "campaign_already_migrated" }
                    : Blocked(error);
            if (current.PayloadVersion == 7 || current.PayloadVersion == 8)
            {
                try { return Commit(current.PayloadVersion == 7 ? CampaignPlayerSaveCodec.UpgradeV7(current.Payload, definition) : CampaignPlayerSaveCodec.UpgradeV8(current.Payload, definition)); }
                catch (Exception e) when (e is ArgumentException || e is System.IO.IOException || e is FormatException)
                { return Blocked("campaign_v7_invalid"); }
            }
            if (current.PayloadVersion != 6) return Blocked("campaign_source_version_unsupported");
            return ImportLegacyCopy(current.Payload);
        }
        public bool TryLoad(out CampaignPlayerSave save, out string diagnostic)
        {
            save = null;
            if (storageUncertain) { diagnostic = "campaign_storage_restart_required"; return false; }
            var loaded = journal.Load();
            diagnostic = loaded.DiagnosticCode;
            if (!loaded.Success) return false;
            if (loaded.PayloadVersion != CampaignPlayerSaveCodec.PayloadVersion) { diagnostic = "campaign_migration_required"; return false; }
            return CampaignPlayerSaveCodec.TryDecode(loaded.Payload, definition, out save, out diagnostic);
        }
        public CampaignRoundAvailability Availability(int chapter, int round)
        {
            if (!TryLoad(out var save, out _)) return CampaignRoundAvailability.ContentUnavailable;
            return catalog.Progression(CampaignPlayerSaveCodec.Records(save.player), save.player.acknowledgedChapterCompletions).Availability(chapter, round);
        }
        public CampaignSaveOperation SelectRound(int chapter, int round)
        {
            if (!TryLoad(out var save, out var reason)) return Blocked(reason);
            if (save.run.state == CampaignRunState.Running) return Blocked("campaign_run_in_progress");
            var state = catalog.Progression(CampaignPlayerSaveCodec.Records(save.player), save.player.acknowledgedChapterCompletions);
            if (state.Availability(chapter, round) != CampaignRoundAvailability.Playable) return Blocked("campaign_round_not_playable");
            if (save.player.selectedChapter == chapter && save.player.selectedLevel == round)
                return new CampaignSaveOperation { Committed = true, NoOp = true, Diagnostic = "campaign_selection_unchanged" };
            save.player.selectedChapter = chapter; save.player.selectedLevel = round;
            CampaignPlayerSaveCodec.RefreshDerivedProgress(save.player);
            return Commit(save);
        }
        public LevelData ResolveSelectedRound()
        {
            if (!TryLoad(out var save, out _)) return null;
            if (catalog.Progression(CampaignPlayerSaveCodec.Records(save.player), save.player.acknowledgedChapterCompletions)
                .Availability(save.player.selectedChapter, save.player.selectedLevel) != CampaignRoundAvailability.Playable) return null;
            return ChapterLevelCatalog.Find(catalog, save.player.selectedChapter, save.player.selectedLevel);
        }
        CampaignSaveOperation Commit(CampaignPlayerSave data)
        {
            string payload;
            try { payload = CampaignPlayerSaveCodec.Encode(data, definition); }
            catch (ArgumentException e) { return Blocked(e.Message); }
            var result = journal.TrySave(payload, CampaignPlayerSaveCodec.PayloadVersion);
            var operation = new CampaignSaveOperation { JournalWrite = result, Diagnostic = result.DiagnosticCode };
            // Never turn a successful volatile read after failed Flush into a committed result.
            if (!result.PrimaryDurablyCommitted) { storageUncertain = true; return operation; }
            var readback = journal.Load();
            operation.Committed = readback.Success && readback.PayloadVersion == CampaignPlayerSaveCodec.PayloadVersion && readback.Payload == payload &&
                CampaignPlayerSaveCodec.TryDecode(readback.Payload, definition, out _, out _);
            if (!operation.Committed) { storageUncertain = true; operation.Diagnostic = "campaign_readback_failed"; }
            return operation;
        }
    }
}
