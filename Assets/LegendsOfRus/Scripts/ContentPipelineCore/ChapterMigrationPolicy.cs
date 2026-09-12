using System;

namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterMigrationPolicy
    {
        public const int DryRunSchemaVersion = 1;
        public const int PackageManifestSchemaVersion = 1;
        public const int ExpectedChapterId = 1;
        public const int ExpectedLevelCount = 10;
        public const int ExpectedStoryMilestoneCount = 7;
        public const int MinimumLocalizationEntryCount = 1;
        public const int ExpectedCopiedPayloadCount = 3;
        public const string DryRunId = "LOR-V120-STEP02-CH01-DRY-RUN";
        public const string PackageId = "LOR-CH01-CANONICAL-DRY-RUN";
        public const string StagingRoot = "ProjectDocs/ContentPipeline/Staging/CH01";
        public const string StagedDefinition = StagingRoot + "/chapter_definition.json";
        public const string StagedLevels = StagingRoot + "/chapter01_levels.json";
        public const string StagedStory = StagingRoot + "/chapter01_story.json";
        public const string StagedLocalization = StagingRoot + "/story_ru.json";
        public const string StagedPackageManifest = StagingRoot + "/chapter_package_manifest.json";
        public const string StagedDryRunReport = StagingRoot + "/migration_dry_run_report.json";

        public static bool IsSha256(string value)
        {
            return ChapterPipelinePolicy.IsSha256(value);
        }
    }
}
