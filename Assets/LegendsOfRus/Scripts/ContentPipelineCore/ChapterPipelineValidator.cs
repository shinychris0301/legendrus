using System;
using System.Collections.Generic;

namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterPipelineValidator
    {
        public static ContentPipelineValidationResult ValidateManifest(ChapterPipelineManifest manifest)
        {
            var blockers = new List<string>();
            if (manifest == null)
            {
                blockers.Add(ChapterPipelineBlockerCodes.ManifestMissing);
                return new ContentPipelineValidationResult(blockers.ToArray());
            }

            if (manifest.SchemaVersion != ChapterPipelinePolicy.ManifestSchemaVersion)
                Add(blockers, ChapterPipelineBlockerCodes.ManifestSchemaInvalid);
            if (string.IsNullOrWhiteSpace(manifest.ManifestId))
                Add(blockers, ChapterPipelineBlockerCodes.ManifestIdMissing);
            if (manifest.ExpectedChapterCount != ChapterPipelinePolicy.ExpectedChapterCount ||
                manifest.Chapters == null || manifest.Chapters.Length != ChapterPipelinePolicy.ExpectedChapterCount)
            {
                Add(blockers, ChapterPipelineBlockerCodes.ChapterCountInvalid);
            }

            if (manifest.Chapters == null)
                return new ContentPipelineValidationResult(blockers.ToArray());

            var ids = new HashSet<int>();
            var stableIds = new HashSet<string>(StringComparer.Ordinal);
            var addresses = new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < manifest.Chapters.Length; i++)
            {
                ChapterPipelineEntry entry = manifest.Chapters[i];
                if (entry == null)
                {
                    Add(blockers, ChapterPipelineBlockerCodes.ChapterEntryMissing);
                    continue;
                }

                int expectedId = i + 1;
                if (entry.ChapterId < ChapterPipelinePolicy.MinimumChapterId ||
                    entry.ChapterId > ChapterPipelinePolicy.MaximumChapterId)
                    Add(blockers, ChapterPipelineBlockerCodes.ChapterIdInvalid);
                if (!ids.Add(entry.ChapterId))
                    Add(blockers, ChapterPipelineBlockerCodes.ChapterIdDuplicate);
                if (entry.ChapterId != expectedId)
                    Add(blockers, ChapterPipelineBlockerCodes.ChapterOrderInvalid);

                string expectedStableId = ChapterPipelinePolicy.ChapterStableId(entry.ChapterId);
                if (!string.Equals(entry.StableId, expectedStableId, StringComparison.Ordinal))
                    Add(blockers, ChapterPipelineBlockerCodes.StableIdInvalid);
                if (string.IsNullOrWhiteSpace(entry.StableId) || !stableIds.Add(entry.StableId))
                    Add(blockers, ChapterPipelineBlockerCodes.StableIdDuplicate);
                if (string.IsNullOrWhiteSpace(entry.DisplayNameKey))
                    Add(blockers, ChapterPipelineBlockerCodes.DisplayNameKeyMissing);

                ValidateAddress(entry.LevelsAddress, entry.ChapterId, addresses, blockers);
                ValidateAddress(entry.StoryAddress, entry.ChapterId, addresses, blockers);
                ValidateAddress(entry.LocalizationAddress, entry.ChapterId, addresses, blockers);
                ValidateAddress(entry.EnvironmentAddress, entry.ChapterId, addresses, blockers);
                ValidateAddress(entry.MapArtAddress, entry.ChapterId, addresses, blockers);

                if (!string.Equals(entry.CanonicalFolder, ChapterPipelinePolicy.ChapterFolder(entry.ChapterId), StringComparison.Ordinal))
                    Add(blockers, ChapterPipelineBlockerCodes.CanonicalFolderInvalid);

                if (entry.UsesLegacyChapter01Source != (entry.ChapterId == 1))
                    Add(blockers, ChapterPipelineBlockerCodes.LegacySourceBoundaryInvalid);

                if (entry.ProductionReady &&
                    (!entry.DataOwnershipReviewed || !entry.ArtOwnershipReviewed ||
                     !entry.LocalizationOwnershipReviewed || !entry.QaOwnershipReviewed))
                {
                    Add(blockers, ChapterPipelineBlockerCodes.OwnershipIncomplete);
                }
            }

            return new ContentPipelineValidationResult(blockers.ToArray());
        }

        public static ContentPipelineValidationResult ValidateTemplate(ChapterTemplateDefinition template)
        {
            var blockers = new List<string>();
            if (template == null)
            {
                blockers.Add(ChapterPipelineBlockerCodes.TemplateMissing);
                return new ContentPipelineValidationResult(blockers.ToArray());
            }
            if (template.SchemaVersion != ChapterPipelinePolicy.TemplateSchemaVersion)
                Add(blockers, ChapterPipelineBlockerCodes.TemplateSchemaInvalid);
            if (template.ChapterId < ChapterPipelinePolicy.MinimumChapterId ||
                template.ChapterId > ChapterPipelinePolicy.MaximumChapterId ||
                !string.Equals(template.StableId, ChapterPipelinePolicy.ChapterStableId(template.ChapterId), StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(template.DisplayNameKey))
                Add(blockers, ChapterPipelineBlockerCodes.TemplateIdentityInvalid);
            if (template.LevelCount < ChapterPipelinePolicy.MinimumLevelCount ||
                template.LevelCount > ChapterPipelinePolicy.MaximumLevelCount)
                Add(blockers, ChapterPipelineBlockerCodes.TemplateLevelCountInvalid);
            if (template.FirstPlayableLevel < 1 || template.FirstPlayableLevel > template.LevelCount ||
                template.FinaleLevelId < template.FirstPlayableLevel || template.FinaleLevelId > template.LevelCount)
                Add(blockers, ChapterPipelineBlockerCodes.TemplateFinaleInvalid);
            if (!ValidateMilestones(template.StoryMilestoneLevels, template.LevelCount))
                Add(blockers, ChapterPipelineBlockerCodes.TemplateMilestoneInvalid);
            return new ContentPipelineValidationResult(blockers.ToArray());
        }

        static bool ValidateMilestones(int[] values, int levelCount)
        {
            if (values == null) return false;
            int previous = 0;
            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                if (value < 1 || value > levelCount || value <= previous)
                    return false;
                previous = value;
            }
            return true;
        }

        static void ValidateAddress(string address, int chapterId, HashSet<string> addresses, List<string> blockers)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                Add(blockers, ChapterPipelineBlockerCodes.AddressMissing);
                return;
            }
            if (!address.StartsWith(ChapterPipelinePolicy.ChapterAddressPrefix(chapterId), StringComparison.Ordinal))
                Add(blockers, ChapterPipelineBlockerCodes.AddressPrefixInvalid);
            if (!addresses.Add(address))
                Add(blockers, ChapterPipelineBlockerCodes.AddressDuplicate);
        }

        static void Add(List<string> blockers, string value)
        {
            if (!blockers.Contains(value)) blockers.Add(value);
        }
    }
}
