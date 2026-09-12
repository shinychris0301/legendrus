using System;

namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterPipelinePolicy
    {
        public const int ManifestSchemaVersion = 1;
        public const int TemplateSchemaVersion = 1;
        public const int ExpectedChapterCount = 20;
        public const int MinimumChapterId = 1;
        public const int MaximumChapterId = 20;
        public const int MinimumLevelCount = 1;
        public const int MaximumLevelCount = 100;
        public const int ExistingLevelCatalogSchemaVersion = 3;
        public const int ExistingStoryCatalogSchemaVersion = 3;
        public const string ChapterStableIdPrefix = "chapter";
        public const string AddressPrefix = "lor.chapter.";
        public const string CanonicalContentRoot = "Assets/LegendsOfRus/Content/Chapters";
        public const string LegacyChapter01Levels = "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json";
        public const string LegacyChapter01Story = "Assets/LegendsOfRus/Resources/Config/chapter01_story.json";
        public const string LegacyRussianLocalization = "Assets/LegendsOfRus/Resources/Localization/story_ru.json";

        public static string ChapterStableId(int chapterId)
        {
            return ChapterStableIdPrefix + chapterId.ToString("00");
        }

        public static string ChapterAddressPrefix(int chapterId)
        {
            return AddressPrefix + chapterId.ToString("00") + ".";
        }

        public static string ChapterFolder(int chapterId)
        {
            return CanonicalContentRoot + "/CH" + chapterId.ToString("00");
        }

        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool hex = (c >= '0' && c <= '9') ||
                           (c >= 'a' && c <= 'f') ||
                           (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }
    }
}
