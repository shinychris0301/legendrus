#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step13StoryMilestoneReactionSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.13 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1(forceReload: true);
            StoryLocalizationData russian = StoryCatalog.LoadRussian(forceReload: true);
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1(forceReload: true);
            StoryValidationReport validation = StoryCatalog.Validate(story, russian, levels);

            Check("schema-v3 story validates", () => validation.IsValid, ref passed, ref total, failures);
            Check("eight persistent scenes remain", () => story.scenes.Length == 8, ref passed, ref total, failures);
            Check("seven post-victory mappings exist", () => story.milestones.Length == 7, ref passed, ref total, failures);
            Check("twenty outcome reactions exist", () => story.reactions.Length == 20, ref passed, ref total, failures);
            Check("post-victory levels are deterministic", () =>
                MilestoneLevels(story, majorOnly: false) == "1,3,4,6,8,9,10",
                ref passed, ref total, failures);
            Check("major milestones are levels 1 3 6 10", () =>
                MilestoneLevels(story, majorOnly: true) == "1,3,6,10",
                ref passed, ref total, failures);
            Check("levels 2 5 7 have no story interruption", () =>
                StoryCatalog.FindMilestoneAfterLevel(2) == null &&
                StoryCatalog.FindMilestoneAfterLevel(5) == null &&
                StoryCatalog.FindMilestoneAfterLevel(7) == null,
                ref passed, ref total, failures);
            Check("all released levels have both reactions", () =>
                CountResolvedReactions(levels) == 20,
                ref passed, ref total, failures);
            Check("all reactions localize", () =>
                CountLocalizedReactions(levels, russian) == 20,
                ref passed, ref total, failures);
            Check("reaction lookup is deterministic", () =>
            {
                LocalizedStoryReaction first = StoryCatalog.LocalizeReaction(
                    StoryCatalog.FindReaction(6, StoryReactionOutcomes.Failure),
                    russian
                );
                LocalizedStoryReaction second = StoryCatalog.LocalizeReaction(
                    StoryCatalog.FindReaction(6, StoryReactionOutcomes.Failure),
                    russian
                );
                return first != null && second != null && first.Text == second.Text;
            }, ref passed, ref total, failures);
            Check("intro route remains pre-level", () =>
                StoryCatalog.FindScene(story.introSceneId).returnRoute == StoryReturnRoutes.PreLevel,
                ref passed, ref total, failures);
            Check("finale route remains chapter complete", () =>
                StoryCatalog.FindScene("chapter01.finale").returnRoute == StoryReturnRoutes.ChapterComplete,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log($"v1.13 Step 2 milestone/reaction self-tests passed: {passed}/{total}.");
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Tests",
                    $"v1.13 Step 2 self-tests passed: {passed}/{total}",
                    "OK"
                );
                return;
            }

            Debug.LogError(
                $"v1.13 Step 2 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static string MilestoneLevels(ChapterStoryCatalogData story, bool majorOnly)
        {
            var ids = new List<int>();
            for (int i = 0; i < story.milestones.Length; i++)
            {
                StoryMilestoneData milestone = story.milestones[i];
                if (!majorOnly || milestone.major)
                    ids.Add(milestone.afterLevelId);
            }
            return string.Join(",", ids);
        }

        static int CountResolvedReactions(ChapterLevelCatalogData levels)
        {
            int count = 0;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                int levelId = levels.levels[i].levelId;
                if (StoryCatalog.FindReaction(levelId, StoryReactionOutcomes.Victory) != null)
                    count++;
                if (StoryCatalog.FindReaction(levelId, StoryReactionOutcomes.Failure) != null)
                    count++;
            }
            return count;
        }

        static int CountLocalizedReactions(
            ChapterLevelCatalogData levels,
            StoryLocalizationData russian
        )
        {
            int count = 0;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                int levelId = levels.levels[i].levelId;
                string[] outcomes = {
                    StoryReactionOutcomes.Victory,
                    StoryReactionOutcomes.Failure
                };
                for (int outcomeIndex = 0; outcomeIndex < outcomes.Length; outcomeIndex++)
                {
                    LocalizedStoryReaction localized = StoryCatalog.LocalizeReaction(
                        StoryCatalog.FindReaction(levelId, outcomes[outcomeIndex]),
                        russian
                    );
                    if (
                        localized != null &&
                        !string.IsNullOrWhiteSpace(localized.Speaker) &&
                        !string.IsNullOrWhiteSpace(localized.Text)
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
