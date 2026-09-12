#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step13ChapterFinaleCompletionSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.13 Step 3 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1(forceReload: true);
            StoryLocalizationData russian = StoryCatalog.LoadRussian(forceReload: true);
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1(forceReload: true);
            StoryValidationReport validation = StoryCatalog.Validate(story, russian, levels);
            StoryChapterCompletionData completion = story?.completion;
            LevelData finalLevel = completion != null
                ? ChapterLevelCatalog.Find(story.chapterId, completion.finalLevelId)
                : null;
            StorySceneData finale = completion != null
                ? StoryCatalog.FindScene(completion.finaleSceneId)
                : null;

            Check("schema-v3 story validates", () => validation.IsValid, ref passed, ref total, failures);
            Check("eight story scenes remain", () => story.scenes.Length == 8, ref passed, ref total, failures);
            Check("seven milestones remain", () => story.milestones.Length == 7, ref passed, ref total, failures);
            Check("twenty reactions remain", () => story.reactions.Length == 20, ref passed, ref total, failures);
            Check("completion contract targets level 10", () => completion.finalLevelId == 10, ref passed, ref total, failures);
            Check("completion trigger is unique", () => CountCompletionTriggers(levels, completion.completionTrigger) == 1, ref passed, ref total, failures);
            Check("level 10 is terminal", () => finalLevel != null && ChapterLevelCatalog.Next(finalLevel) == null, ref passed, ref total, failures);
            Check("level 10 owns completion trigger", () => finalLevel.HasStoryTrigger(completion.completionTrigger), ref passed, ref total, failures);
            Check("final milestone is major", () =>
            {
                StoryMilestoneData milestone = StoryCatalog.FindMilestoneAfterLevel(completion.finalLevelId);
                return milestone != null && milestone.major && milestone.sceneId == completion.finaleSceneId;
            }, ref passed, ref total, failures);
            Check("finale returns Chapter Complete", () => finale.returnRoute == StoryReturnRoutes.ChapterComplete, ref passed, ref total, failures);
            Check("finale has five lines", () => finale.lines.Length == 5, ref passed, ref total, failures);
            Check("all finale lines localize", () => CountLocalizedLines(finale, russian) == 5, ref passed, ref total, failures);
            Check("completion summary localizes", () => StoryCatalog.LocalizeCompletion(completion, russian) != null, ref passed, ref total, failures);
            Check("save schema is version 6", () => new PlayerSaveData().saveVersion == 6, ref passed, ref total, failures);
            Check("acknowledgement list initializes", () => new PlayerSaveData().acknowledgedChapterCompletions != null, ref passed, ref total, failures);
            Check("approved Chapter Complete art is 941x1672", () =>
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/LegendsOfRus/Resources/DesignScreens/ChapterComplete.png"
                );
                return texture != null && texture.width == 941 && texture.height == 1672;
            }, ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log($"v1.13 Step 3 finale/completion self-tests passed: {passed}/{total}.");
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Tests",
                    $"v1.13 Step 3 self-tests passed: {passed}/{total}",
                    "OK"
                );
                return;
            }

            Debug.LogError(
                $"v1.13 Step 3 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static int CountCompletionTriggers(ChapterLevelCatalogData levels, string trigger)
        {
            int count = 0;
            for (int i = 0; i < levels.levels.Length; i++)
                if (levels.levels[i] != null && levels.levels[i].HasStoryTrigger(trigger))
                    count++;
            return count;
        }

        static int CountLocalizedLines(
            StorySceneData scene,
            StoryLocalizationData localization
        )
        {
            int count = 0;
            for (int i = 0; i < scene.lines.Length; i++)
            {
                StoryLineData line = scene.lines[i];
                if (
                    !string.IsNullOrWhiteSpace(StoryCatalog.Localize(line.speakerKey, localization)) &&
                    !string.IsNullOrWhiteSpace(StoryCatalog.Localize(line.textKey, localization))
                )
                {
                    count++;
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
