using System;

namespace LegendsOfRus.ContentPipeline
{
    [Serializable]
    public sealed class ChapterPipelineEntry
    {
        public int ChapterId;
        public string StableId = string.Empty;
        public string DisplayNameKey = string.Empty;
        public string LevelsAddress = string.Empty;
        public string StoryAddress = string.Empty;
        public string LocalizationAddress = string.Empty;
        public string EnvironmentAddress = string.Empty;
        public string MapArtAddress = string.Empty;
        public string CanonicalFolder = string.Empty;
        public bool UsesLegacyChapter01Source;
        public bool DataOwnershipReviewed;
        public bool ArtOwnershipReviewed;
        public bool LocalizationOwnershipReviewed;
        public bool QaOwnershipReviewed;
        public bool ProductionReady;
    }

    [Serializable]
    public sealed class ChapterPipelineManifest
    {
        public int SchemaVersion = ChapterPipelinePolicy.ManifestSchemaVersion;
        public string ManifestId = string.Empty;
        public int ExpectedChapterCount = ChapterPipelinePolicy.ExpectedChapterCount;
        public ChapterPipelineEntry[] Chapters = Array.Empty<ChapterPipelineEntry>();
    }

    [Serializable]
    public sealed class ChapterTemplateDefinition
    {
        public int SchemaVersion = ChapterPipelinePolicy.TemplateSchemaVersion;
        public int ChapterId;
        public string StableId = string.Empty;
        public string DisplayNameKey = string.Empty;
        public int LevelCount;
        public int FirstPlayableLevel = 1;
        public int FinaleLevelId;
        public int[] StoryMilestoneLevels = Array.Empty<int>();
        public string LevelsAddress = string.Empty;
        public string StoryAddress = string.Empty;
        public string LocalizationAddress = string.Empty;
        public string EnvironmentAddress = string.Empty;
        public string MapArtAddress = string.Empty;
        public bool ChapterArtLocked;
        public bool LevelDataReviewed;
        public bool StoryDataReviewed;
        public bool LocalizationReviewed;
        public bool QaReviewed;
    }

    [Serializable]
    public sealed class ContentPipelineTransitionEvidence
    {
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string TransitionId = string.Empty;
        public string SourceTreeSha256 = string.Empty;
        public bool LegacyChapter01LevelsValidated;
        public bool LegacyChapter01StoryValidated;
        public bool LegacyRussianLocalizationValidated;
        public bool ManifestContractReviewed;
        public bool TemplateContractReviewed;
        public bool StableIdConventionReviewed;
        public bool AddressablesNamespaceReviewed;
        public bool ContentOwnershipReviewed;
        public bool LocalizationKeyConventionReviewed;
        public bool QaAutomationReviewed;
        public bool MigrationPlanReviewed;
        public bool LockedArtNonDestructiveBoundaryReviewed;
        public int UnresolvedCriticalBlockers;
    }

    public sealed class ContentPipelineValidationResult
    {
        public string[] Blockers { get; private set; }
        public bool IsValid { get { return Blockers.Length == 0; } }

        public ContentPipelineValidationResult(string[] blockers)
        {
            Blockers = blockers ?? Array.Empty<string>();
        }
    }

    public sealed class ContentPipelineReviewResult
    {
        public string[] ReviewBlockers { get; private set; }
        public string[] ActionBlockers { get; private set; }
        public bool ReadyForHumanReview { get { return ReviewBlockers.Length == 0; } }
        public bool MayBeginChapterMigration { get { return ReadyForHumanReview && ActionBlockers.Length == 0; } }

        public ContentPipelineReviewResult(string[] reviewBlockers, string[] actionBlockers)
        {
            ReviewBlockers = reviewBlockers ?? Array.Empty<string>();
            ActionBlockers = actionBlockers ?? Array.Empty<string>();
        }
    }
}
