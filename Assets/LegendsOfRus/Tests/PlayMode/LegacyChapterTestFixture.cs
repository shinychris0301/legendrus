using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace LegendsOfRus.Tests.PlayMode
{
    // Historical ten-round finale coverage only. Active 40-round content is covered
    // independently by ActiveCampaignIntegrationTests and normal navigation tests.
    internal sealed class LegacyChapterTestFixture : IDisposable
    {
        readonly Dictionary<FieldInfo, object> previous = new Dictionary<FieldInfo, object>();
        public static ChapterLevelCatalogData Levels() => Read<ChapterLevelCatalogData>("Config/chapter01_levels");
        public static ChapterStoryCatalogData Story() => Read<ChapterStoryCatalogData>("Config/chapter01_story");
        public static StoryLocalizationData Russian() => Read<StoryLocalizationData>("Localization/story_ru");
        static T Read<T>(string path) => JsonUtility.FromJson<T>(Resources.Load<TextAsset>(path).text);
        public LegacyChapterTestFixture()
        {
            Set(typeof(ChapterLevelCatalog), "cached", Levels());
            Set(typeof(StoryCatalog), "cachedCatalog", Story());
            Set(typeof(StoryCatalog), "cachedRussian", Russian());
            Set(typeof(StoryCatalog), "cachedEnglish", Read<StoryLocalizationData>("Localization/story_en"));
        }
        void Set(Type type, string name, object value)
        {
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
            previous.Add(field, field.GetValue(null));field.SetValue(null, value);
        }
        public void Dispose()
        {
            StoryService.ResetRuntime();
            foreach (var pair in previous) pair.Key.SetValue(null, pair.Value);
        }
    }
}
