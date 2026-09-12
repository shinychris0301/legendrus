using System;

namespace LegendsOfRus.ContentPipeline
{
    [Serializable]
    public sealed class ChapterMigrationFileEvidence
    {
        public string LogicalName = string.Empty;
        public string SourcePath = string.Empty;
        public string StagedPath = string.Empty;
        public string SourceSha256 = string.Empty;
        public string StagedSha256 = string.Empty;
        public long ByteCount;
        public bool ExactByteMatch;
    }

    [Serializable]
    public sealed class CanonicalChapterPackageManifest
    {
        public int SchemaVersion = ChapterMigrationPolicy.PackageManifestSchemaVersion;
        public string PackageId = string.Empty;
        public int ChapterId;
        public string StableId = string.Empty;
        public string SourceMode = string.Empty;
        public string CanonicalDefinitionPath = string.Empty;
        public string CanonicalDefinitionSha256 = string.Empty;
        public ChapterMigrationFileEvidence[] CopiedPayloads = Array.Empty<ChapterMigrationFileEvidence>();
        public int LevelCount;
        public int StoryMilestoneCount;
        public int LocalizationEntryCount;
        public bool RuntimeStillUsesLegacyResources;
        public bool RuntimeCutoverPerformed;
        public bool AddressablesMutated;
        public bool LockedArtTouched;
        public bool LevelBalanceChanged;
        public bool RewardsChanged;
    }

    [Serializable]
    public sealed class ChapterMigrationDryRunEvidence
    {
        public int SchemaVersion = ChapterMigrationPolicy.DryRunSchemaVersion;
        public string DryRunId = string.Empty;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string PackageManifestSha256 = string.Empty;
        public CanonicalChapterPackageManifest Package = null;
        public bool LegacyRuntimeStillAuthoritative;
        public bool RuntimeCutoverPerformed;
        public bool AddressablesMutated;
        public bool LockedArtTouched;
        public bool AutomaticArtGenerationUsed;
        public bool LocalizationRewritten;
        public bool LevelBalanceChanged;
        public bool RewardsChanged;
        public bool ProgressionChanged;
        public int UnresolvedCriticalBlockers;
    }

    public sealed class ChapterMigrationDryRunResult
    {
        public string[] Blockers { get; private set; }
        public bool IsStructurallyValid { get { return Blockers.Length == 0; } }

        public ChapterMigrationDryRunResult(string[] blockers)
        {
            Blockers = blockers ?? Array.Empty<string>();
        }
    }
}
