using System;
using System.Collections.Generic;

namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterMigrationDryRunValidator
    {
        public static ChapterMigrationDryRunResult ValidatePackage(CanonicalChapterPackageManifest package)
        {
            var blockers = new List<string>();
            if (package == null)
            {
                blockers.Add(ChapterMigrationBlockerCodes.PackageMissing);
                return new ChapterMigrationDryRunResult(blockers.ToArray());
            }

            if (package.SchemaVersion != ChapterMigrationPolicy.PackageManifestSchemaVersion)
                Add(blockers, ChapterMigrationBlockerCodes.PackageSchemaInvalid);
            if (!string.Equals(package.PackageId, ChapterMigrationPolicy.PackageId, StringComparison.Ordinal) ||
                package.ChapterId != 1 || !string.Equals(package.StableId, "chapter01", StringComparison.Ordinal) ||
                !string.Equals(package.SourceMode, "legacy-byte-copy-dry-run", StringComparison.Ordinal))
                Add(blockers, ChapterMigrationBlockerCodes.PackageIdentityInvalid);
            if (!ChapterMigrationPolicy.IsSha256(package.CanonicalDefinitionSha256))
                Add(blockers, ChapterMigrationBlockerCodes.DefinitionHashInvalid);
            if (package.CopiedPayloads == null || package.CopiedPayloads.Length != ChapterMigrationPolicy.ExpectedCopiedPayloadCount)
                Add(blockers, ChapterMigrationBlockerCodes.PayloadCountInvalid);
            else
                for (int i = 0; i < package.CopiedPayloads.Length; i++) ValidateFile(package.CopiedPayloads[i], blockers);
            if (package.LevelCount != ChapterMigrationPolicy.ExpectedLevelCount)
                Add(blockers, ChapterMigrationBlockerCodes.LevelCountInvalid);
            if (package.StoryMilestoneCount != ChapterMigrationPolicy.ExpectedStoryMilestoneCount)
                Add(blockers, ChapterMigrationBlockerCodes.StoryMilestoneCountInvalid);
            if (package.LocalizationEntryCount < ChapterMigrationPolicy.MinimumLocalizationEntryCount)
                Add(blockers, ChapterMigrationBlockerCodes.LocalizationEntryCountInvalid);
            if (!package.RuntimeStillUsesLegacyResources)
                Add(blockers, ChapterMigrationBlockerCodes.LegacyAuthorityLost);
            if (package.RuntimeCutoverPerformed)
                Add(blockers, ChapterMigrationBlockerCodes.RuntimeCutoverOccurred);
            if (package.AddressablesMutated)
                Add(blockers, ChapterMigrationBlockerCodes.AddressablesMutated);
            if (package.LockedArtTouched)
                Add(blockers, ChapterMigrationBlockerCodes.LockedArtTouched);
            if (package.LevelBalanceChanged)
                Add(blockers, ChapterMigrationBlockerCodes.LevelBalanceChanged);
            if (package.RewardsChanged)
                Add(blockers, ChapterMigrationBlockerCodes.RewardsChanged);
            return new ChapterMigrationDryRunResult(blockers.ToArray());
        }

        public static ChapterMigrationDryRunResult ValidateEvidence(ChapterMigrationDryRunEvidence evidence)
        {
            var blockers = new List<string>();
            if (evidence == null)
            {
                blockers.Add(ChapterMigrationBlockerCodes.EvidenceMissing);
                return new ChapterMigrationDryRunResult(blockers.ToArray());
            }
            if (evidence.SchemaVersion != ChapterMigrationPolicy.DryRunSchemaVersion)
                Add(blockers, ChapterMigrationBlockerCodes.SchemaInvalid);
            if (!string.Equals(evidence.DryRunId, ChapterMigrationPolicy.DryRunId, StringComparison.Ordinal))
                Add(blockers, ChapterMigrationBlockerCodes.DryRunIdInvalid);
            if (!evidence.EvidenceRetained)
                Add(blockers, ChapterMigrationBlockerCodes.EvidenceMissing);
            if (!ChapterMigrationPolicy.IsSha256(evidence.PackageManifestSha256))
                Add(blockers, ChapterMigrationBlockerCodes.PackageManifestHashInvalid);
            Append(blockers, ValidatePackage(evidence.Package).Blockers);
            if (!evidence.LegacyRuntimeStillAuthoritative)
                Add(blockers, ChapterMigrationBlockerCodes.LegacyAuthorityLost);
            if (evidence.RuntimeCutoverPerformed)
                Add(blockers, ChapterMigrationBlockerCodes.RuntimeCutoverOccurred);
            if (evidence.AddressablesMutated)
                Add(blockers, ChapterMigrationBlockerCodes.AddressablesMutated);
            if (evidence.LockedArtTouched)
                Add(blockers, ChapterMigrationBlockerCodes.LockedArtTouched);
            if (evidence.AutomaticArtGenerationUsed)
                Add(blockers, ChapterMigrationBlockerCodes.AutomaticArtGenerationUsed);
            if (evidence.LocalizationRewritten)
                Add(blockers, ChapterMigrationBlockerCodes.LocalizationRewritten);
            if (evidence.LevelBalanceChanged)
                Add(blockers, ChapterMigrationBlockerCodes.LevelBalanceChanged);
            if (evidence.RewardsChanged)
                Add(blockers, ChapterMigrationBlockerCodes.RewardsChanged);
            if (evidence.ProgressionChanged)
                Add(blockers, ChapterMigrationBlockerCodes.ProgressionChanged);
            if (evidence.UnresolvedCriticalBlockers > 0)
                Add(blockers, ChapterMigrationBlockerCodes.CriticalBlockersOpen);
            return new ChapterMigrationDryRunResult(blockers.ToArray());
        }

        static void ValidateFile(ChapterMigrationFileEvidence file, List<string> blockers)
        {
            if (file == null)
            {
                Add(blockers, ChapterMigrationBlockerCodes.PayloadMissing);
                return;
            }
            if (string.IsNullOrWhiteSpace(file.LogicalName) || string.IsNullOrWhiteSpace(file.SourcePath) || string.IsNullOrWhiteSpace(file.StagedPath))
                Add(blockers, ChapterMigrationBlockerCodes.PayloadPathInvalid);
            if (!ChapterMigrationPolicy.IsSha256(file.SourceSha256) || !ChapterMigrationPolicy.IsSha256(file.StagedSha256))
                Add(blockers, ChapterMigrationBlockerCodes.PayloadHashInvalid);
            if (!file.ExactByteMatch || !string.Equals(file.SourceSha256, file.StagedSha256, StringComparison.OrdinalIgnoreCase))
                Add(blockers, ChapterMigrationBlockerCodes.PayloadHashMismatch);
            if (file.ByteCount <= 0)
                Add(blockers, ChapterMigrationBlockerCodes.PayloadByteCountInvalid);
        }

        static void Append(List<string> destination, string[] source)
        {
            if (source == null) return;
            for (int i = 0; i < source.Length; i++) Add(destination, source[i]);
        }

        static void Add(List<string> blockers, string value)
        {
            if (!blockers.Contains(value)) blockers.Add(value);
        }
    }
}
