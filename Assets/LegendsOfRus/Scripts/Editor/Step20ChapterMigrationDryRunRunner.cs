#if UNITY_EDITOR
using System;
using System.IO;
using System.Security.Cryptography;
using LegendsOfRus.ContentPipeline;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ChapterMigrationDryRunRunner
    {
        [Serializable] sealed class LegacyLevelRef { public int levelId; }
        [Serializable] sealed class LegacyLevelsHeader { public int schemaVersion; public int chapterId; public string chapterName = string.Empty; public LegacyLevelRef[] levels = Array.Empty<LegacyLevelRef>(); }
        [Serializable] sealed class LegacyMilestoneRef { public int afterLevelId; public string sceneId = string.Empty; }
        [Serializable] sealed class LegacyStoryHeader { public int schemaVersion; public int chapterId; public LegacyMilestoneRef[] milestones = Array.Empty<LegacyMilestoneRef>(); }
        [Serializable] sealed class LegacyLocalizationEntry { public string key = string.Empty; public string value = string.Empty; }
        [Serializable] sealed class LegacyLocalizationHeader { public int schemaVersion; public string locale = string.Empty; public LegacyLocalizationEntry[] entries = Array.Empty<LegacyLocalizationEntry>(); }

        [MenuItem("Legends of Rus/Content Pipeline/Generate v1.20 Step 2 Chapter 1 Dry Run")]
        public static void GenerateDryRun()
        {
            string root = ProjectRoot();
            string stage = Path.Combine(root, ChapterMigrationPolicy.StagingRoot);
            Directory.CreateDirectory(stage);

            string levelsSource = Path.Combine(root, ChapterPipelinePolicy.LegacyChapter01Levels);
            string storySource = Path.Combine(root, ChapterPipelinePolicy.LegacyChapter01Story);
            string locSource = Path.Combine(root, ChapterPipelinePolicy.LegacyRussianLocalization);

            byte[] levelsBytes = File.ReadAllBytes(levelsSource);
            byte[] storyBytes = File.ReadAllBytes(storySource);
            byte[] locBytes = File.ReadAllBytes(locSource);

            string levelsStage = Path.Combine(root, ChapterMigrationPolicy.StagedLevels);
            string storyStage = Path.Combine(root, ChapterMigrationPolicy.StagedStory);
            string locStage = Path.Combine(root, ChapterMigrationPolicy.StagedLocalization);
            File.WriteAllBytes(levelsStage, levelsBytes);
            File.WriteAllBytes(storyStage, storyBytes);
            File.WriteAllBytes(locStage, locBytes);

            LegacyLevelsHeader levels = JsonUtility.FromJson<LegacyLevelsHeader>(System.Text.Encoding.UTF8.GetString(levelsBytes));
            LegacyStoryHeader story = JsonUtility.FromJson<LegacyStoryHeader>(System.Text.Encoding.UTF8.GetString(storyBytes));
            LegacyLocalizationHeader loc = JsonUtility.FromJson<LegacyLocalizationHeader>(System.Text.Encoding.UTF8.GetString(locBytes));

            int[] milestones = new int[story.milestones == null ? 0 : story.milestones.Length];
            for (int i = 0; i < milestones.Length; i++) milestones[i] = story.milestones[i].afterLevelId;
            var definition = new ChapterTemplateDefinition
            {
                SchemaVersion = ChapterPipelinePolicy.TemplateSchemaVersion,
                ChapterId = 1,
                StableId = "chapter01",
                DisplayNameKey = "chapter.01.title",
                LevelCount = levels.levels == null ? 0 : levels.levels.Length,
                FirstPlayableLevel = 1,
                FinaleLevelId = levels.levels == null ? 0 : levels.levels.Length,
                StoryMilestoneLevels = milestones,
                LevelsAddress = "lor.chapter.01.levels",
                StoryAddress = "lor.chapter.01.story",
                LocalizationAddress = "lor.chapter.01.localization.ru",
                EnvironmentAddress = "lor.chapter.01.environment",
                MapArtAddress = "lor.chapter.01.map_art",
                ChapterArtLocked = true,
                LevelDataReviewed = false,
                StoryDataReviewed = false,
                LocalizationReviewed = false,
                QaReviewed = false
            };
            string definitionPath = Path.Combine(root, ChapterMigrationPolicy.StagedDefinition);
            File.WriteAllText(definitionPath, JsonUtility.ToJson(definition, true) + "\n");

            var package = new CanonicalChapterPackageManifest
            {
                SchemaVersion = ChapterMigrationPolicy.PackageManifestSchemaVersion,
                PackageId = ChapterMigrationPolicy.PackageId,
                ChapterId = 1,
                StableId = "chapter01",
                SourceMode = "legacy-byte-copy-dry-run",
                CanonicalDefinitionPath = ChapterMigrationPolicy.StagedDefinition,
                CanonicalDefinitionSha256 = Sha256(definitionPath),
                CopiedPayloads = new[]
                {
                    Evidence("levels", ChapterPipelinePolicy.LegacyChapter01Levels, ChapterMigrationPolicy.StagedLevels, levelsSource, levelsStage),
                    Evidence("story", ChapterPipelinePolicy.LegacyChapter01Story, ChapterMigrationPolicy.StagedStory, storySource, storyStage),
                    Evidence("localization_ru", ChapterPipelinePolicy.LegacyRussianLocalization, ChapterMigrationPolicy.StagedLocalization, locSource, locStage)
                },
                LevelCount = levels.levels == null ? 0 : levels.levels.Length,
                StoryMilestoneCount = story.milestones == null ? 0 : story.milestones.Length,
                LocalizationEntryCount = loc.entries == null ? 0 : loc.entries.Length,
                RuntimeStillUsesLegacyResources = true,
                RuntimeCutoverPerformed = false,
                AddressablesMutated = false,
                LockedArtTouched = false,
                LevelBalanceChanged = false,
                RewardsChanged = false
            };
            string manifestPath = Path.Combine(root, ChapterMigrationPolicy.StagedPackageManifest);
            File.WriteAllText(manifestPath, JsonUtility.ToJson(package, true) + "\n");

            var dryRun = new ChapterMigrationDryRunEvidence
            {
                SchemaVersion = ChapterMigrationPolicy.DryRunSchemaVersion,
                DryRunId = ChapterMigrationPolicy.DryRunId,
                EvidenceRetained = true,
                EvidenceReviewed = false,
                PackageManifestSha256 = Sha256(manifestPath),
                Package = package,
                LegacyRuntimeStillAuthoritative = true,
                RuntimeCutoverPerformed = false,
                AddressablesMutated = false,
                LockedArtTouched = false,
                AutomaticArtGenerationUsed = false,
                LocalizationRewritten = false,
                LevelBalanceChanged = false,
                RewardsChanged = false,
                ProgressionChanged = false,
                UnresolvedCriticalBlockers = 0
            };
            string reportPath = Path.Combine(root, ChapterMigrationPolicy.StagedDryRunReport);
            File.WriteAllText(reportPath, JsonUtility.ToJson(dryRun, true) + "\n");

            ChapterMigrationDryRunResult result = ChapterMigrationDryRunValidator.ValidateEvidence(dryRun);
            if (!result.IsStructurallyValid)
                throw new InvalidOperationException("v1.20 Step 2 dry run generated invalid evidence: " + string.Join(", ", result.Blockers));

            Debug.Log("v1.20 Step 2 Chapter 1 dry run generated. Legacy Resources remain authoritative. Staging: " + ChapterMigrationPolicy.StagingRoot);
        }

        [MenuItem("Legends of Rus/Content Pipeline/Validate v1.20 Step 2 Chapter 1 Dry Run")]
        public static void ValidateDryRun()
        {
            string root = ProjectRoot();
            string reportPath = Path.Combine(root, ChapterMigrationPolicy.StagedDryRunReport);
            if (!File.Exists(reportPath)) throw new FileNotFoundException("Generate the Step 2 dry run first.", reportPath);
            var evidence = JsonUtility.FromJson<ChapterMigrationDryRunEvidence>(File.ReadAllText(reportPath));
            ChapterMigrationDryRunResult result = ChapterMigrationDryRunValidator.ValidateEvidence(evidence);
            if (!result.IsStructurallyValid) throw new InvalidOperationException("v1.20 Step 2 dry run validation failed: " + string.Join(", ", result.Blockers));
            VerifyFileHash(root, evidence.Package.CopiedPayloads[0]);
            VerifyFileHash(root, evidence.Package.CopiedPayloads[1]);
            VerifyFileHash(root, evidence.Package.CopiedPayloads[2]);
            Debug.Log("v1.20 Step 2 dry run validation passed. All three legacy payloads match staging byte-for-byte.");
        }

        [MenuItem("Legends of Rus/Content Pipeline/Print v1.20 Step 2 Migration Dry Run Review")]
        public static void PrintReview()
        {
            ValidateDryRun();
            Debug.Log("v1.20 Step 2: Chapter 1 canonical staging is structurally valid; runtime cutover remains blocked. Next revised milestone: v1.20 Step 3 Production UI Asset Reconstruction and Screenshot Removal.");
        }

        static ChapterMigrationFileEvidence Evidence(string logical, string sourceRel, string stagedRel, string sourceFull, string stagedFull)
        {
            string a = Sha256(sourceFull); string b = Sha256(stagedFull);
            return new ChapterMigrationFileEvidence
            {
                LogicalName = logical,
                SourcePath = sourceRel,
                StagedPath = stagedRel,
                SourceSha256 = a,
                StagedSha256 = b,
                ByteCount = new FileInfo(sourceFull).Length,
                ExactByteMatch = string.Equals(a, b, StringComparison.OrdinalIgnoreCase)
            };
        }

        static void VerifyFileHash(string root, ChapterMigrationFileEvidence e)
        {
            string source = Sha256(Path.Combine(root, e.SourcePath));
            string staged = Sha256(Path.Combine(root, e.StagedPath));
            if (!string.Equals(source, e.SourceSha256, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(staged, e.StagedSha256, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(source, staged, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Dry-run payload hash changed: " + e.LogicalName);
        }

        static string Sha256(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        static string ProjectRoot() { return Path.GetFullPath(Path.Combine(Application.dataPath, "..")); }
    }
}
#endif
