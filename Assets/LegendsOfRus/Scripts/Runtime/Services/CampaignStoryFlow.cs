using System;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed class CampaignStoryScene
    {
        public int Chapter;
        public string Key;
        public StorySceneData Scene;
        public StoryLocalizationData Russian, English;
    }

    public sealed partial class CampaignSaveSession
    {
        // Durable eligibility is derived from progress and scoped acknowledgements. Closing a
        // partly read scene reoffers it from its first line; it never starts another paid round.
        public bool TryGetPendingStory(out CampaignStoryScene pending, out string diagnostic)
        {
            pending = null;
            if (!TryLoad(out var save, out diagnostic)) return false;
            pending = PendingStory(save); return true;
        }
        CampaignStoryScene PendingStory(CampaignPlayerSave save)
        {
            if (save.run.state == CampaignRunState.Running ||
                save.run.state != CampaignRunState.None && !save.run.acknowledged) return null;
            int chapter = save.player.selectedChapter;
            var story = catalog.LoadStory(chapter);
            if (story == null) return null;
            var scenes = story.scenes ?? Array.Empty<StorySceneData>();
            var intro = scenes.FirstOrDefault(s => s.id == story.introSceneId);
            if (intro != null && !StoryRead(save, chapter, intro.id)) return StoryEntry(chapter, intro);
            foreach (var milestone in (story.milestones ?? Array.Empty<StoryMilestoneData>()).OrderBy(m => m.afterLevelId))
            {
                if (!save.player.levelProgress.Any(p => p.chapterId == chapter && p.levelId == milestone.afterLevelId && p.completions > 0)) continue;
                // Already-read milestones cannot become pending. Avoid reparsing the entire
                // 500-round package for each one while preparing a later playtest block.
                if (StoryRead(save, chapter, milestone.sceneId)) continue;
                var scene = scenes.FirstOrDefault(s => s.id == milestone.sceneId);
                var level = ChapterLevelCatalog.Find(catalog, chapter, milestone.afterLevelId);
                if (scene != null && level != null && level.HasStoryTrigger(scene.trigger))
                    return StoryEntry(chapter, scene);
            }
            return null;
        }
        CampaignStoryScene StoryEntry(int chapter, StorySceneData scene) => new CampaignStoryScene {
            Chapter = chapter, Key = catalog.StoryLedgerKey(chapter, scene.id), Scene = scene,
            Russian = catalog.LoadLocalization(chapter, "ru"), English = catalog.LoadLocalization(chapter, "en")
        };
        bool StoryRead(CampaignPlayerSave save, int chapter, string scene)
        {
            if (save.player.completedStoryScenes.Contains(catalog.StoryLedgerKey(chapter, scene))) return true;
            // Only the unchanged legacy bridge receives credit from the immutable source archive.
            // Its round-ten finale cannot acknowledge the expanded campaign's finale.
            if (!catalog.IsLegacyStoryBridge(chapter)) return false;
            var legacy = JsonUtility.FromJson<PlayerSaveData>(save.legacySourceJson);
            return legacy?.completedStoryScenes != null && legacy.completedStoryScenes.Contains(scene);
        }
        public CampaignSaveOperation AcknowledgeStory(string key)
        {
            if (!TryLoad(out var save, out var reason)) return Blocked(reason);
            if (string.IsNullOrWhiteSpace(key)) return Blocked("campaign_story_invalid");
            if (save.player.completedStoryScenes.Contains(key)) return NoChange("campaign_story_already_acknowledged");
            var pending = PendingStory(save);
            if (pending == null || !string.Equals(pending.Key, key, StringComparison.Ordinal)) return Blocked("campaign_story_not_pending");
            save.player.completedStoryScenes.Add(key);
            return Commit(save); // No grants, attempts, energy, selection or finale unlock mutation.
        }
    }
}
