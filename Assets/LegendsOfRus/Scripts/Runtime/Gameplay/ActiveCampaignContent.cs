using System;
using UnityEngine;
using LegendsOfRus.ContentPipeline;

namespace LegendsOfRus
{
    // One validated package supplies normal gameplay, story and localization.
    // The legacy Resources remain intact for migration fixtures and historical checks.
    public static class ActiveCampaignContent
    {
        public const string ResourceRoot = "CampaignActive/";
        static ValidatedCampaignCatalog cached;

        public static ValidatedCampaignCatalog Load(bool forceReload = false)
        {
            if (!forceReload && cached != null) return cached;
            var definition = JsonUtility.FromJson<CampaignDefinition>(Read("definition"));
            var catalog = new ValidatedCampaignCatalog(definition);
            if (!catalog.TryAdd(new CampaignPackageInput
            {
                ChapterId = 1, ContentRevision = definition.ContentRevision, FinaleReady = true,
                LevelsJson = Read("levels"), StoryJson = Read("story"),
                RussianJson = Read("story_ru"), EnglishJson = Read("story_en")
            }, out var errors))
                throw new InvalidOperationException("Active campaign is invalid: " + string.Join("; ", errors));
            cached = catalog;
            return cached;
        }

        public static void InvalidateCache() { cached = null; }

        static string Read(string name)
        {
            var asset = Resources.Load<TextAsset>(ResourceRoot + name);
            if (!asset) throw new InvalidOperationException("Missing active campaign resource: " + name);
            return asset.text;
        }
    }
}
