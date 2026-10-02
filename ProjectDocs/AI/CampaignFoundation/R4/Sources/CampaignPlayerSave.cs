using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus
{
    [Serializable] public sealed class CampaignPlayerSave
    {
        public int schemaVersion = 7;
        public string format = "lor.campaign-player";
        public string campaignId = "legends-of-rus-main";
        public int contentRevision;
        // Immutable migration provenance. Its chapter-ten finale is never a campaign finale.
        public string legacySourceJson;
        public string legacySourceSha256;
        // Reuse all existing economy/preferences/record fields without running legacy normalization.
        // The nested DTO's saveVersion remains 6; the owning envelope/codec is version 7.
        public PlayerSaveData player;
    }

    public static class CampaignPlayerSaveCodec
    {
        public const int PayloadVersion = 7;
        const int MaximumExpandedBytes = 8 * 1024 * 1024;
        [Serializable] sealed class CompressedPayload { public string compressedCampaign; }
        static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        public static CampaignPlayerSave FromLegacy(string source, CampaignDefinition definition)
        {
            if (CampaignContract.Validate(definition).Count != 0) throw new ArgumentException("Invalid campaign definition.");
            // Uses the strict v6 decoder and rejects unknown progress through the R1 planner.
            CampaignMigrationRehearsal.Prepare(source);
            if (!PlayerSavePayloadCodec.TryDeserialize(source, 6, false, out var player, out var error))
                throw new ArgumentException(error);
            var result = new CampaignPlayerSave { contentRevision = definition.ContentRevision,
                legacySourceJson = source, legacySourceSha256 = Hash(source), player = player };
            // Archive contains all original scenes/acknowledgements. Campaign narrative uses new state.
            player.completedStoryScenes = new List<string>();
            player.acknowledgedChapterCompletions = new List<int>();
            RefreshDerivedProgress(player);
            Encode(result, definition); // Do not return a malformed candidate.
            return result;
        }
        public static IEnumerable<CampaignProgressRecord> Records(PlayerSaveData player) => player.levelProgress.Select(r =>
            r == null ? null : new CampaignProgressRecord { ChapterId = r.chapterId, RoundId = r.levelId, BestStars = r.bestStars,
                BestScore = r.bestScore, Attempts = r.attempts, Retries = r.retries, Failures = r.failures,
                Completions = r.completions, RewardClaimed = r.rewardClaimed });

        public static string Encode(CampaignPlayerSave data, CampaignDefinition definition)
        {
            string error = Validate(data, definition);
            if (error != null) throw new ArgumentException(error);
            string json = JsonUtility.ToJson(data);
            if (StrictUtf8.GetByteCount(json) > MaximumExpandedBytes) throw new ArgumentException("campaign_save_too_large");
            if (json.Length <= SaveEnvelopeCodec.MaximumPayloadCharacters) return json;
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
                { var bytes = StrictUtf8.GetBytes(json); gzip.Write(bytes, 0, bytes.Length); }
                string packed = JsonUtility.ToJson(new CompressedPayload { compressedCampaign = Convert.ToBase64String(output.ToArray()) });
                if (packed.Length > SaveEnvelopeCodec.MaximumPayloadCharacters) throw new ArgumentException("campaign_save_too_large");
                return packed;
            }
        }
        public static bool TryDecode(string json, CampaignDefinition definition, out CampaignPlayerSave data, out string error)
        {
            data = null; error = "campaign_payload_invalid";
            try
            {
                if (string.IsNullOrWhiteSpace(json) || json.Length > SaveEnvelopeCodec.MaximumPayloadCharacters) return false;
                if (json.StartsWith("{\"compressedCampaign\":", StringComparison.Ordinal))
                {
                    var wrapper = JsonUtility.FromJson<CompressedPayload>(json);
                    if (JsonUtility.ToJson(wrapper) != json || wrapper.compressedCampaign == null) return false;
                    using (var input = new MemoryStream(Convert.FromBase64String(wrapper.compressedCampaign)))
                    using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                    using (var expanded = new MemoryStream())
                    {
                        var buffer = new byte[8192]; int count;
                        while ((count = gzip.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (expanded.Length + count > MaximumExpandedBytes) return false;
                            expanded.Write(buffer, 0, count);
                        }
                        json = StrictUtf8.GetString(expanded.ToArray());
                    }
                }
                var parsed = JsonUtility.FromJson<CampaignPlayerSave>(json);
                error = Validate(parsed, definition);
                if (error != null) return false;
                // Only canonical output from this codec is admitted; rejects omitted/unknown/duplicate fields.
                if (JsonUtility.ToJson(parsed) != json) { error = "campaign_payload_noncanonical"; return false; }
                data = parsed; error = null; return true;
            }
            catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is OverflowException || e is FormatException || e is IOException)
            { error = "campaign_payload_invalid"; return false; }
        }
        static string Validate(CampaignPlayerSave data, CampaignDefinition definition)
        {
            if (CampaignContract.Validate(definition).Count != 0) return "campaign_definition_invalid";
            if (data == null || data.schemaVersion != PayloadVersion || data.format != "lor.campaign-player" ||
                data.campaignId != definition.CampaignId || data.contentRevision != definition.ContentRevision)
                return "campaign_payload_identity_invalid";
            if (data.legacySourceJson == null || data.legacySourceJson.Length > SaveEnvelopeCodec.MaximumPayloadCharacters ||
                data.legacySourceSha256 != Hash(data.legacySourceJson)) return "campaign_migration_source_invalid";
            try { CampaignMigrationRehearsal.Prepare(data.legacySourceJson); }
            catch (ArgumentException) { return "campaign_migration_source_invalid"; }
            var p = data.player;
            if (p == null || p.saveVersion != 6 || p.levelProgress == null || p.completedTutorialSteps == null ||
                p.completedStoryScenes == null || p.acknowledgedChapterCompletions == null || p.lastDailyRewardUtc == null ||
                !CampaignContract.IsRound(p.selectedChapter, p.selectedLevel)) return "campaign_player_fields_invalid";
            // The v7 boundary checks canonical field presence/types and campaign semantics.
            // Do not send the expanded 10,000-round ledger through the bounded v6 importer.
            if (p.gold < 0 || p.gems < 0 || p.energy < 0 || p.energyMax < 1 || p.energy > p.energyMax || p.lastEnergyUtcTicks < 0 ||
                p.boosterMace < 0 || p.boosterFirebird < 0 || p.boosterLivingLight < 0 || p.boosterSeaPearl < 0)
                return "campaign_economy_invalid";
            if (p.completedTutorialSteps.Any(string.IsNullOrWhiteSpace) || p.completedStoryScenes.Any(string.IsNullOrWhiteSpace) ||
                p.acknowledgedChapterCompletions.Distinct().Count() != p.acknowledgedChapterCompletions.Count)
                return "campaign_story_state_invalid";
            CampaignProgression progress;
            try { progress = new CampaignProgression(definition, Array.Empty<CampaignChapterPackage>(), Records(p), p.acknowledgedChapterCompletions); }
            catch (ArgumentException) { return "campaign_records_invalid"; }
            PlayerSavePayloadCodec.TryDeserialize(data.legacySourceJson, 6, false, out var legacy, out _);
            foreach (var previous in legacy.levelProgress)
            {
                var current = p.levelProgress.FirstOrDefault(r => r.chapterId == previous.chapterId && r.levelId == previous.levelId);
                if (current == null || (previous.rewardClaimed && !current.rewardClaimed) || current.bestStars < previous.bestStars ||
                    current.bestScore < previous.bestScore || current.attempts < previous.attempts || current.retries < previous.retries ||
                    current.failures < previous.failures || current.completions < previous.completions)
                    return "campaign_legacy_progress_regressed";
            }
            foreach (int chapter in p.acknowledgedChapterCompletions)
                for (int predecessor = 1; predecessor <= chapter; predecessor++)
                    if (progress.CompletedPrefix(predecessor) != 500 || !p.acknowledgedChapterCompletions.Contains(predecessor))
                        return "campaign_finale_without_progress";
            if (p.chapter != p.selectedChapter || p.level != p.highestUnlockedLevel ||
                p.highestUnlockedLevel != Math.Min(500, progress.CompletedPrefix(p.selectedChapter) + 1) ||
                p.stars != p.levelProgress.Sum(r => r.bestStars)) return "campaign_derived_progress_invalid";
            return null;
        }
        public static void RefreshDerivedProgress(PlayerSaveData player)
        {
            if (player == null || player.levelProgress == null) throw new ArgumentException("Missing player progress.");
            var wins = new HashSet<int>(player.levelProgress.Where(r => r != null && r.chapterId == player.selectedChapter &&
                (r.completions > 0 || r.bestStars > 0)).Select(r => r.levelId));
            int prefix = 0; while (prefix < 500 && wins.Contains(prefix + 1)) prefix++;
            player.chapter = player.selectedChapter;
            player.highestUnlockedLevel = Math.Min(500, prefix + 1);
            player.level = player.highestUnlockedLevel;
            player.stars = player.levelProgress.Where(r => r != null).Sum(r => r.bestStars);
        }
        static string Hash(string value)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLowerInvariant();
        }
    }
}
