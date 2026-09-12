#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step13StoryDataSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.13 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1(forceReload: true);
            StoryLocalizationData russian = StoryCatalog.LoadRussian(forceReload: true);
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1(forceReload: true);
            StoryValidationReport validation = StoryCatalog.Validate(story, russian, levels);

            Check("story and localization validate", () => validation.IsValid, ref passed, ref total, failures);
            Check("intro is first-launch scene", () => story.introSceneId == "chapter01.intro", ref passed, ref total, failures);
            Check("intro returns to pre-level", () =>
                StoryCatalog.FindScene(story.introSceneId).returnRoute == StoryReturnRoutes.PreLevel,
                ref passed, ref total, failures);
            Check("all seven dialogue triggers resolve", () => CountDialogueTriggers(levels) == 7, ref passed, ref total, failures);
            Check("finale returns to chapter complete", () =>
                StoryCatalog.FindByTrigger("dialogue.chapter_finale").returnRoute == StoryReturnRoutes.ChapterComplete,
                ref passed, ref total, failures);
            Check("Russian speaker resolves", () =>
                StoryCatalog.Localize("speaker.dobrynya", russian) == "ДОБРЫНЯ",
                ref passed, ref total, failures);
            Check("intro sequence has three localized lines", () =>
            {
                var sequence = new StorySequenceRuntime(StoryCatalog.FindScene(story.introSceneId), russian);
                int count = 0;
                while (!sequence.IsComplete)
                {
                    if (sequence.CurrentLine == null || string.IsNullOrWhiteSpace(sequence.CurrentLine.Text))
                        return false;
                    count++;
                    sequence.Advance();
                }
                return count == 3;
            }, ref passed, ref total, failures);
            Check("story completion is idempotent", () =>
            {
                var completed = new List<string>();
                return
                    StoryProgress.Complete(completed, story.introSceneId) &&
                    !StoryProgress.Complete(completed, story.introSceneId) &&
                    completed.Count == 1;
            }, ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log($"v1.13 Step 1 story-data self-tests passed: {passed}/{total}.");
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Tests",
                    $"v1.13 Step 1 self-tests passed: {passed}/{total}",
                    "OK"
                );
                return;
            }

            Debug.LogError(
                $"v1.13 Step 1 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static int CountDialogueTriggers(ChapterLevelCatalogData levels)
        {
            int count = 0;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                LevelData level = levels.levels[i];
                if (level.storyTriggers == null) continue;
                for (int j = 0; j < level.storyTriggers.Length; j++)
                {
                    string trigger = level.storyTriggers[j];
                    if (
                        trigger.StartsWith("dialogue.", StringComparison.Ordinal) &&
                        StoryCatalog.FindByTrigger(trigger) != null
                    )
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        static void Check(
            string name,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test()) passed++;
                else failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
