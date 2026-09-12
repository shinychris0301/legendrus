using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class StoryMilestoneReactionPlayModeTests
    {
        static readonly string[] PersistedSaveKeys =
        {
            "LegendsOfRus_Save_v1",
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject stateObject;
        bool ownsStateObject;
        bool[] hadPersistedSaves;
        string[] persistedSaveValues;
        GameState previousState;
        string previousRuntimeSaveJson;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SnapshotAndClearSaveJournal();
            yield return null;
            if (GameState.Instance == null)
            {
                stateObject = new GameObject("StoryMilestoneReactionTestState");
                stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }
            else
            {
                previousState = GameState.Instance;
                previousRuntimeSaveJson = JsonUtility.ToJson(previousState.Save);
            }

            GameState.Instance.ResetProgress();
            StoryService.ResetRuntime();
            StoryCatalog.LoadChapter1(forceReload: true);
            StoryCatalog.LoadRussian(forceReload: true);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            StoryService.ResetRuntime();
            if (GameState.Instance != null)
                GameState.Instance.ResetProgress();
            if (ownsStateObject && stateObject != null)
                Object.Destroy(stateObject);
            yield return null;
            RestoreSaveJournal();
            RestoreRuntimeState();
        }

        [Test]
        public void CatalogV3_ValidatesMilestonesAndReactionCoverage()
        {
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1();
            StoryValidationReport report = StoryCatalog.Validate(
                story,
                StoryCatalog.LoadRussian(),
                ChapterLevelCatalog.LoadChapter1(forceReload: true)
            );

            Assert.That(report.Errors, Is.Empty);
            Assert.That(story.schemaVersion, Is.EqualTo(3));
            Assert.That(story.milestones, Has.Length.EqualTo(7));
            Assert.That(story.reactions, Has.Length.EqualTo(20));
        }

        [Test]
        public void PostVictoryLookup_MapsExactReleasedLevels()
        {
            var expected = new Dictionary<int, string>
            {
                { 1, "chapter01.firebird_found" },
                { 3, "chapter01.raven_watch" },
                { 4, "chapter01.elder_request" },
                { 6, "chapter01.embers_fade" },
                { 8, "chapter01.river_shadow" },
                { 9, "chapter01.living_flame" },
                { 10, "chapter01.finale" }
            };

            for (int levelId = 1; levelId <= 10; levelId++)
            {
                StoryMilestoneData milestone = StoryCatalog.FindMilestoneAfterLevel(levelId);
                if (expected.TryGetValue(levelId, out string sceneId))
                {
                    Assert.That(milestone, Is.Not.Null, $"Level {levelId}");
                    Assert.That(milestone.sceneId, Is.EqualTo(sceneId));
                }
                else
                {
                    Assert.That(milestone, Is.Null, $"Level {levelId}");
                }
            }
        }

        [Test]
        public void MajorMilestones_AreExactlyLevelsOneThreeSixAndTen()
        {
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1();
            var major = new List<int>();
            for (int i = 0; i < story.milestones.Length; i++)
                if (story.milestones[i].major)
                    major.Add(story.milestones[i].afterLevelId);

            Assert.That(major, Is.EqualTo(new[] { 1, 3, 6, 10 }));
        }

        [Test]
        public void OutcomeReactions_CoverAndLocalizeBothOutcomesForAllLevels()
        {
            string[] outcomes = {
                StoryReactionOutcomes.Victory,
                StoryReactionOutcomes.Failure
            };
            for (int levelId = 1; levelId <= 10; levelId++)
            {
                for (int outcomeIndex = 0; outcomeIndex < outcomes.Length; outcomeIndex++)
                {
                    StoryReactionData data = StoryCatalog.FindReaction(
                        levelId,
                        outcomes[outcomeIndex]
                    );
                    LocalizedStoryReaction localized = StoryCatalog.LocalizeReaction(data);
                    Assert.That(localized, Is.Not.Null, $"{levelId}:{outcomes[outcomeIndex]}");
                    Assert.That(localized.Speaker, Is.Not.Empty);
                    Assert.That(localized.Text, Is.Not.Empty);
                }
            }
        }

        [Test]
        public void ReactionLookup_IsRepeatableAndDoesNotMutateStoryProgress()
        {
            GameState state = GameState.Instance;
            int completedBefore = state.Save.completedStoryScenes.Count;
            LevelData level = ChapterLevelCatalog.Find(1, 6);

            LocalizedStoryReaction first = StoryService.GetLocalizedReaction(
                level,
                StoryReactionOutcomes.Failure
            );
            LocalizedStoryReaction second = StoryService.GetLocalizedReaction(
                level,
                StoryReactionOutcomes.Failure
            );

            Assert.That(first.Text, Is.EqualTo(second.Text));
            Assert.That(first.LevelId, Is.EqualTo(6));
            Assert.That(state.Save.completedStoryScenes.Count, Is.EqualTo(completedBefore));
        }

        [Test]
        public void OutcomeLookup_UsesCapturedLevelInsteadOfAdvancedOrStaleSaveState()
        {
            GameState state = GameState.Instance;
            LevelData levelOne = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(levelOne, levelOne.starScoreThresholds[0]);

            Assert.That(state.Save.selectedLevel, Is.EqualTo(2));
            LocalizedStoryReaction victory = StoryService.GetLocalizedReaction(
                levelOne,
                StoryReactionOutcomes.Victory
            );
            Assert.That(victory.LevelId, Is.EqualTo(1));

            LevelData levelTwo = ChapterLevelCatalog.Find(1, 2);
            state.RecordLevelFailure(levelTwo, levelTwo.startingScore);
            LocalizedStoryReaction failure = StoryService.GetLocalizedReaction(
                levelTwo,
                StoryReactionOutcomes.Failure
            );
            Assert.That(state.Save.lastCompletedLevel, Is.EqualTo(1));
            Assert.That(failure.LevelId, Is.EqualTo(2));
        }

        [Test]
        public void CompletedMilestone_IsSuppressedWhileCompatibilityPredicateStaysPure()
        {
            GameState state = GameState.Instance;
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);

            Assert.That(StoryService.ShouldShowDialogueAfterVictory(), Is.True);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            StoryAdvanceResult result;
            do
            {
                result = StoryService.AdvanceDialogue();
            }
            while (result == StoryAdvanceResult.MoreLines);

            Assert.That(result, Is.EqualTo(StoryAdvanceResult.ChapterMap));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(StoryService.ShouldShowDialogueAfterVictory(), Is.True);
        }

        [Test]
        public void PendingMilestone_RecoversAfterRuntimeResetBeforeNextLevel()
        {
            GameState state = GameState.Instance;
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);

            StoryService.ResetRuntime();

            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.firebird_found"));
        }

        [Test]
        public void ActiveScene_CannotBeOverwrittenByDifferentPendingMilestone()
        {
            GameState state = GameState.Instance;
            Assert.That(StoryService.TryBeginChapterIntro(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.intro"));

            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);

            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.intro"));
        }

        [Test]
        public void Validator_RejectsDuplicateReactionAndMilestoneTriggerMismatch()
        {
            ChapterStoryCatalogData source = StoryCatalog.LoadChapter1();
            StoryLocalizationData russian = StoryCatalog.LoadRussian();
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1();

            ChapterStoryCatalogData duplicate = JsonUtility.FromJson<ChapterStoryCatalogData>(
                JsonUtility.ToJson(source)
            );
            duplicate.reactions[1].outcome = StoryReactionOutcomes.Victory;
            StoryValidationReport duplicateReport = StoryCatalog.Validate(
                duplicate,
                russian,
                levels
            );
            Assert.That(duplicateReport.IsValid, Is.False);
            Assert.That(string.Join("\n", duplicateReport.Errors), Does.Contain("duplicated"));

            ChapterStoryCatalogData mismatch = JsonUtility.FromJson<ChapterStoryCatalogData>(
                JsonUtility.ToJson(source)
            );
            mismatch.milestones[0].sceneId = "chapter01.raven_watch";
            StoryValidationReport mismatchReport = StoryCatalog.Validate(
                mismatch,
                russian,
                levels
            );
            Assert.That(mismatchReport.IsValid, Is.False);
            Assert.That(
                string.Join("\n", mismatchReport.Errors),
                Does.Contain("not authored on level 1")
            );

            ChapterStoryCatalogData missingSceneId = JsonUtility.FromJson<ChapterStoryCatalogData>(
                JsonUtility.ToJson(source)
            );
            missingSceneId.milestones[0].sceneId = null;
            StoryValidationReport missingSceneReport = null;
            Assert.DoesNotThrow(() =>
                missingSceneReport = StoryCatalog.Validate(
                    missingSceneId,
                    russian,
                    levels
                )
            );
            Assert.That(missingSceneReport.IsValid, Is.False);
            Assert.That(
                string.Join("\n", missingSceneReport.Errors),
                Does.Contain("has no sceneId")
            );
        }

        void SnapshotAndClearSaveJournal()
        {
            hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            persistedSaveValues = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                persistedSaveValues[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();
        }

        void RestoreRuntimeState()
        {
            if (
                previousState == null ||
                GameState.Instance != previousState ||
                string.IsNullOrEmpty(previousRuntimeSaveJson)
            )
            {
                return;
            }

            PlayerSaveData restored = JsonUtility.FromJson<PlayerSaveData>(
                previousRuntimeSaveJson
            );
            if (restored != null)
                previousState.Save = restored;
        }

        void RestoreSaveJournal()
        {
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadPersistedSaves[i])
                    PlayerPrefs.SetString(key, persistedSaveValues[i]);
            }
            PlayerPrefs.Save();
        }
    }
}
