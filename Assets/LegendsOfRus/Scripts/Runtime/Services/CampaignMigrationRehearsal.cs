using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus
{
    [Serializable] public sealed class CampaignMigrationCandidate
    {
        public int rehearsalSchema = 1;
        public string sourceSha256;
        public string sourcePayload;
        public int selectedChapter, selectedRound;
        public string[] roundIds;
        public bool[] rewardClaims;
        public int[] campaignFinales;
    }

    public sealed class CampaignRehearsalResult
    {
        public SaveJournalWriteResult InterruptedWrite;
        public int RecoveredPayloadVersion;
        public bool RecoveredPayloadValid, RepeatedMigrationWasNoOp;
        public string FinalCandidate;
    }

    // Only an in-memory store is constructed here. No live journal keys or PlayerPrefs calls.
    // Payload version 7 is a rehearsal envelope marker, NOT the installed player save schema.
    public static class CampaignMigrationRehearsal
    {
        public static string Prepare(string sourcePayload)
        {
            if (!PlayerSavePayloadCodec.TryDeserialize(sourcePayload, 6, false, out var save, out var reason))
                throw new ArgumentException("Legacy payload rejected: " + reason);
            var source = new LegacyCampaignProgress { SaveVersion = save.saveVersion,
                SelectedChapter = save.selectedChapter, SelectedRound = save.selectedLevel,
                Records = save.levelProgress?.Select(r => r == null ? null : new CampaignProgressRecord {
                    ChapterId = r.chapterId, RoundId = r.levelId, BestStars = r.bestStars, BestScore = r.bestScore,
                    Attempts = r.attempts, Retries = r.retries, Failures = r.failures, Completions = r.completions,
                    RewardClaimed = r.rewardClaimed }).ToArray(),
                CompletedStoryScenes = save.completedStoryScenes?.ToArray(),
                AcknowledgedChapterCompletions = save.acknowledgedChapterCompletions?.ToArray() };
            var plan = CampaignSaveMigration.Plan(source);
            if (!plan.CanReview) throw new ArgumentException("Legacy progress rejected: " + string.Join(",", plan.Blockers));
            return JsonUtility.ToJson(new CampaignMigrationCandidate { sourceSha256 = Hash(sourcePayload), sourcePayload = sourcePayload,
                selectedChapter = plan.SelectedChapter, selectedRound = plan.SelectedRound,
                roundIds = plan.Records.Select(r => CampaignContract.RoundId(r.ChapterId, r.RoundId)).ToArray(),
                rewardClaims = plan.Records.Select(r => r.RewardClaimed).ToArray(), campaignFinales = plan.CampaignFinales });
        }
        public static bool Validate(string candidate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(candidate) || candidate.Length > SaveEnvelopeCodec.MaximumPayloadCharacters) return false;
                var data = JsonUtility.FromJson<CampaignMigrationCandidate>(candidate);
                if (data == null || data.rehearsalSchema != 1 || data.sourceSha256 != Hash(data.sourcePayload)) return false;
                // Canonical output comparison also rejects tampered projections and unrecognized fields.
                return candidate == Prepare(data.sourcePayload);
            }
            catch (Exception e) when (e is ArgumentException || e is NullReferenceException) { return false; }
        }
        static string Hash(string value)
        {
            if (value == null) throw new ArgumentException("Missing source payload.");
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLowerInvariant();
        }
        public static CampaignRehearsalResult Run(string legacyPayload, int failAtMutation = 0)
        {
            if (failAtMutation < 0 || failAtMutation > 8) throw new ArgumentOutOfRangeException(nameof(failAtMutation));
            string candidate = Prepare(legacyPayload);
            var memory = new MemoryJournal();
            var keys = new SaveJournalKeys("rehearsal.primary", "rehearsal.backup", "rehearsal.staging");
            var codec = new SaveEnvelopeCodec();
            var repository = new SaveJournalRepository(memory, codec, new PlayerSavePayloadCodec(), keys, 7);
            if (!repository.TrySave(legacyPayload, 6).Success) throw new InvalidOperationException("Cannot seed isolated journal.");
            memory.Mutations = 0; memory.FailAt = failAtMutation;
            var result = new CampaignRehearsalResult { InterruptedWrite = MigrateIfNeeded(repository, legacyPayload) };
            memory.Crash(); // Discard unflushed changes, just as an interrupted process would.
            repository = new SaveJournalRepository(memory, codec, new PlayerSavePayloadCodec(), keys, 7);
            var recovered = repository.Load();
            result.RecoveredPayloadVersion = recovered.PayloadVersion;
            result.RecoveredPayloadValid = recovered.Success && (recovered.PayloadVersion == 6
                ? recovered.Payload == legacyPayload : recovered.PayloadVersion == 7 && Validate(recovered.Payload));
            if (!result.RecoveredPayloadValid) throw new InvalidOperationException("Interrupted migration did not recover a complete payload.");
            if (recovered.PayloadVersion == 6 && !MigrateIfNeeded(repository, legacyPayload).Success)
                throw new InvalidOperationException("Migration retry failed.");
            var final = repository.Load();
            result.FinalCandidate = final.Payload;
            if (!Validate(final.Payload)) throw new InvalidOperationException("Candidate readback failed.");
            int mutations = memory.Mutations;
            // The migration identity is the source hash, and matching valid target is already migrated.
            if (final.Payload != candidate) throw new InvalidOperationException("Unexpected migration identity.");
            result.RepeatedMigrationWasNoOp = MigrateIfNeeded(repository, legacyPayload) == null && repository.Load().Payload == candidate && memory.Mutations == mutations;
            return result;
        }

        static SaveJournalWriteResult MigrateIfNeeded(SaveJournalRepository repository, string source)
        {
            string candidate = Prepare(source);
            var current = repository.Load();
            if (current.Success && current.PayloadVersion == 7 && Validate(current.Payload) && current.Payload == candidate)
                return null;
            if (!current.Success || current.PayloadVersion != 6 || current.Payload != source)
                throw new InvalidOperationException("Migration source changed or target incompatible.");
            return repository.TrySave(candidate, 7);
        }

        sealed class MemoryJournal : ISaveJournalStore
        {
            Dictionary<string, string> volatileData = new Dictionary<string, string>();
            Dictionary<string, string> durable = new Dictionary<string, string>();
            public int Mutations, FailAt;
            void BeforeMutation() { if (++Mutations == FailAt) throw new InvalidOperationException("Injected process interruption."); }
            public bool TryRead(string key, out string value) => volatileData.TryGetValue(key, out value);
            public void Write(string key, string value) { BeforeMutation(); volatileData[key] = value; }
            public void Delete(string key) { BeforeMutation(); volatileData.Remove(key); }
            public void Flush() { BeforeMutation(); durable = new Dictionary<string, string>(volatileData); }
            public void Crash() { volatileData = new Dictionary<string, string>(durable); FailAt = 0; Mutations = 0; }
        }
    }
}
