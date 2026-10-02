using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed class StoryRecapEntry
    {
        public int AfterLevel { get; }
        public StorySceneData Scene { get; }
        internal StoryRecapEntry(StorySceneData scene, int afterLevel)
        {
            AfterLevel = afterLevel;
            // A local sequence must never modify the cached progression catalogue.
            Scene = JsonUtility.FromJson<StorySceneData>(JsonUtility.ToJson(scene));
        }
    }

    public static class StoryRecap
    {
        public static IReadOnlyList<StoryRecapEntry> Read(PlayerSaveData save) => Read(save, StoryCatalog.LoadChapter1());

        public static IReadOnlyList<StoryRecapEntry> Read(PlayerSaveData save, ChapterStoryCatalogData catalog)
        {
            var result = new List<StoryRecapEntry>();
            if (save == null || catalog == null) return result.AsReadOnly();
            var read = new HashSet<string>(save.completedStoryScenes ?? new List<string>(), StringComparer.Ordinal);
            var added = new HashSet<string>(StringComparer.Ordinal);
            Action<string, int> append = (id, level) =>
            {
                if (string.IsNullOrEmpty(id) || !read.Contains(id) || !added.Add(id)) return;
                var scene = (catalog.scenes ?? Array.Empty<StorySceneData>()).FirstOrDefault(s => s != null && s.id == id);
                if (scene?.lines == null || scene.lines.Length == 0) return;
                result.Add(new StoryRecapEntry(scene, level));
            };
            append(catalog.introSceneId, 0);
            foreach (var milestone in (catalog.milestones ?? Array.Empty<StoryMilestoneData>())
                .Where(m => m != null && m.afterLevelId > 0).OrderBy(m => m.afterLevelId))
            {
                if (save.levelProgress == null || !save.levelProgress.Any(p => p != null &&
                    p.chapterId == catalog.chapterId && p.levelId == milestone.afterLevelId && p.completions > 0)) continue;
                append(milestone.sceneId, milestone.afterLevelId);
            }
            return result.AsReadOnly();
        }
    }
}
