using System;
using System.Collections.Generic;
using System.Linq;

namespace LegendsOfRus.ContentPipeline
{
    // Version 2 is a new campaign contract. It does not relax the legacy v1 dry-run policy.
    [Serializable] public sealed class CampaignDefinition
    {
        public int SchemaVersion = 2;
        public string CampaignId = "legends-of-rus-main";
        public int ContentRevision = 1;
        public int RoundsPerChapter = 500;
        public int SectorSize = 50;
        public CampaignChapterDefinition[] Chapters = Array.Empty<CampaignChapterDefinition>();
    }

    [Serializable] public sealed class CampaignChapterDefinition
    {
        public int ChapterId;
        public string StableId;
        public string RussianName;
        public string EnglishName;
        public int PrerequisiteChapterId;
        public int FinaleRound = 500;
    }

    // Adapter input: rounds whose underlying level data has passed gameplay validation.
    // File existence or world-map artwork must never be used to populate this list.
    public sealed class CampaignChapterPackage
    {
        public int ChapterId;
        public int ContentRevision;
        public int[] ValidatedRoundIds = Array.Empty<int>();
        public bool FinaleReady;
    }

    public static class CampaignContract
    {
        public const int ChapterCount = 20;
        public const int RoundsPerChapter = 500;
        public const int SectorSize = 50;
        public const int TotalRounds = ChapterCount * RoundsPerChapter;

        public static List<string> Validate(CampaignDefinition definition)
        {
            var errors = new List<string>();
            if (definition == null) { errors.Add("campaign_missing"); return errors; }
            if (definition.SchemaVersion != 2) errors.Add("campaign_schema_unsupported");
            if (definition.CampaignId != "legends-of-rus-main") errors.Add("campaign_id_invalid");
            if (definition.ContentRevision < 1) errors.Add("campaign_revision_invalid");
            if (definition.RoundsPerChapter != RoundsPerChapter || definition.SectorSize != SectorSize)
                errors.Add("campaign_round_structure_invalid");
            if (definition.Chapters == null || definition.Chapters.Length != ChapterCount)
            { errors.Add("campaign_chapter_count_invalid"); return errors; }
            for (int i = 0; i < ChapterCount; i++)
            {
                var chapter = definition.Chapters[i];
                if (chapter == null || chapter.ChapterId != i + 1 ||
                    chapter.StableId != "chapter" + (i + 1).ToString("00") ||
                    chapter.PrerequisiteChapterId != i || chapter.FinaleRound != RoundsPerChapter ||
                    string.IsNullOrWhiteSpace(chapter.RussianName) || string.IsNullOrWhiteSpace(chapter.EnglishName))
                    errors.Add("campaign_chapter_invalid:" + (i + 1));
            }
            return errors;
        }

        public static bool IsRound(int chapter, int round) => chapter >= 1 && chapter <= ChapterCount && round >= 1 && round <= RoundsPerChapter;
        public static string RoundId(int chapter, int round)
        {
            if (!IsRound(chapter, round)) throw new ArgumentOutOfRangeException(nameof(round));
            return "chapter" + chapter.ToString("00") + ".round" + round.ToString("000");
        }
        public static int Sector(int round)
        {
            if (round < 1 || round > RoundsPerChapter) throw new ArgumentOutOfRangeException(nameof(round));
            return (round - 1) / SectorSize + 1;
        }
    }

    public enum CampaignRoundAvailability { Invalid, ContentUnavailable, Locked, Playable }

    public sealed class CampaignProgressRecord
    {
        public int ChapterId, RoundId, BestStars, BestScore, Attempts, Retries, Failures, Completions;
        public bool RewardClaimed;
        public CampaignProgressRecord Copy() => (CampaignProgressRecord)MemberwiseClone();
    }

    // Read-only progression snapshot; never connected to PlayerPrefs or GameState in R1.
    public sealed class CampaignProgression
    {
        readonly Dictionary<int, int> released = new Dictionary<int, int>();
        readonly HashSet<int> finalesReady = new HashSet<int>();
        readonly HashSet<int> finalesAcknowledged;
        readonly Dictionary<int, HashSet<int>> cleared = new Dictionary<int, HashSet<int>>();

        public CampaignProgression(CampaignDefinition definition, IEnumerable<CampaignChapterPackage> packages,
            IEnumerable<CampaignProgressRecord> progress, IEnumerable<int> acknowledgedFinales)
        {
            if (CampaignContract.Validate(definition).Count != 0) throw new ArgumentException("Invalid campaign contract.");
            if (packages == null || progress == null || acknowledgedFinales == null) throw new ArgumentNullException();
            foreach (var package in packages)
            {
                if (package == null || package.ChapterId < 1 || package.ChapterId > CampaignContract.ChapterCount ||
                    package.ContentRevision != definition.ContentRevision || released.ContainsKey(package.ChapterId) ||
                    package.ValidatedRoundIds == null || package.ValidatedRoundIds.Length < 1 || package.ValidatedRoundIds.Length > 500)
                    throw new ArgumentException("Invalid or incompatible chapter package.");
                for (int i = 0; i < package.ValidatedRoundIds.Length; i++)
                    if (package.ValidatedRoundIds[i] != i + 1) throw new ArgumentException("Chapter package rounds must be contiguous from one.");
                if (package.FinaleReady && package.ValidatedRoundIds.Length != 500)
                    throw new ArgumentException("A partial chapter cannot publish its campaign finale.");
                released.Add(package.ChapterId, package.ValidatedRoundIds.Length);
                if (package.FinaleReady) finalesReady.Add(package.ChapterId);
            }
            var seen = new HashSet<string>();
            foreach (var record in progress)
            {
                if (!ValidRecord(record) || !seen.Add(CampaignContract.RoundId(record.ChapterId, record.RoundId)))
                    throw new ArgumentException("Invalid or duplicate progress record.");
                if (record.Completions <= 0 && record.BestStars <= 0) continue;
                if (!cleared.TryGetValue(record.ChapterId, out var rounds))
                    cleared.Add(record.ChapterId, rounds = new HashSet<int>());
                rounds.Add(record.RoundId);
            }
            finalesAcknowledged = new HashSet<int>(acknowledgedFinales);
            if (finalesAcknowledged.Any(id => id < 1 || id > 20)) throw new ArgumentException("Invalid finale chapter.");
        }

        internal static bool ValidRecord(CampaignProgressRecord r) => r != null && CampaignContract.IsRound(r.ChapterId, r.RoundId) &&
            r.BestStars >= 0 && r.BestStars <= 3 && r.BestScore >= 0 && r.Attempts >= 0 && r.Retries >= 0 &&
            r.Retries <= r.Attempts && r.Failures >= 0 && r.Failures <= r.Attempts && r.Completions >= 0;

        public int CompletedPrefix(int chapter)
        {
            if (!cleared.TryGetValue(chapter, out var rounds)) return 0;
            int prefix = 0;
            while (prefix < 500 && rounds.Contains(prefix + 1)) prefix++;
            return prefix;
        }
        public bool ChapterCompleted(int chapter) => chapter >= 1 && chapter <= 20 &&
            finalesReady.Contains(chapter) && finalesAcknowledged.Contains(chapter) && CompletedPrefix(chapter) == 500 &&
            (chapter == 1 || ChapterCompleted(chapter - 1));

        public CampaignRoundAvailability Availability(int chapter, int round)
        {
            if (!CampaignContract.IsRound(chapter, round)) return CampaignRoundAvailability.Invalid;
            if (!released.TryGetValue(chapter, out var count) || round > count) return CampaignRoundAvailability.ContentUnavailable;
            if (chapter > 1 && !ChapterCompleted(chapter - 1)) return CampaignRoundAvailability.Locked;
            return round <= CompletedPrefix(chapter) + 1 ? CampaignRoundAvailability.Playable : CampaignRoundAvailability.Locked;
        }
    }

    public sealed class LegacyCampaignProgress
    {
        public int SaveVersion = 6;
        public int SelectedChapter = 1, SelectedRound = 1;
        public CampaignProgressRecord[] Records = Array.Empty<CampaignProgressRecord>();
        public string[] CompletedStoryScenes = Array.Empty<string>();
        public int[] AcknowledgedChapterCompletions = Array.Empty<int>();
    }

    public sealed class CampaignMigrationPlan
    {
        public readonly List<string> Blockers = new List<string>();
        public bool CanReview => Blockers.Count == 0;
        public CampaignProgressRecord[] Records = Array.Empty<CampaignProgressRecord>();
        public string[] LegacyStoryScenes = Array.Empty<string>();
        public int[] LegacyChapterAcknowledgements = Array.Empty<int>();
        public int SelectedChapter, SelectedRound;
        // Legacy level-ten finale acknowledgement is never promoted to a 500-round completion.
        public int[] CampaignFinales = Array.Empty<int>();
    }

    public static class CampaignSaveMigration
    {
        public static CampaignMigrationPlan Plan(LegacyCampaignProgress source)
        {
            var plan = new CampaignMigrationPlan();
            if (source == null || source.SaveVersion != 6) { plan.Blockers.Add("source_save_version_unsupported"); return plan; }
            if (source.SelectedChapter != 1 || source.SelectedRound < 1 || source.SelectedRound > 10)
                plan.Blockers.Add("legacy_selection_unrecognized");
            if (source.Records == null || source.CompletedStoryScenes == null || source.AcknowledgedChapterCompletions == null)
            { plan.Blockers.Add("legacy_collections_missing"); return plan; }
            var seen = new HashSet<int>();
            foreach (var record in source.Records)
                if (!CampaignProgression.ValidRecord(record) || record.ChapterId != 1 || record.RoundId > 10 || !seen.Add(record.RoundId))
                    plan.Blockers.Add("legacy_record_unrecognized");
            if (source.CompletedStoryScenes.Any(string.IsNullOrWhiteSpace) || source.AcknowledgedChapterCompletions.Any(id => id != 1))
                plan.Blockers.Add("legacy_story_state_unrecognized");
            if (!plan.CanReview) return plan; // Never silently drop or normalize unknown progress.
            plan.Records = source.Records.Select(r => r.Copy()).ToArray();
            plan.LegacyStoryScenes = (string[])source.CompletedStoryScenes.Clone();
            plan.LegacyChapterAcknowledgements = (int[])source.AcknowledgedChapterCompletions.Clone();
            plan.SelectedChapter = source.SelectedChapter;
            plan.SelectedRound = source.SelectedRound;
            return plan;
        }
    }
}
