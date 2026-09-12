using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class ChapterFinaleCompletionPlayModeTests
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
                stateObject = new GameObject("ChapterFinaleCompletionTestState");
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
            ChapterLevelCatalog.LoadChapter1(forceReload: true);
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
        public void CatalogV3_DefinesUniqueFinaleCompletionContract()
        {
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1();
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1();
            StoryValidationReport report = StoryCatalog.Validate(
                story,
                StoryCatalog.LoadRussian(),
                levels
            );

            Assert.That(report.Errors, Is.Empty);
            Assert.That(story.schemaVersion, Is.EqualTo(3));
            Assert.That(story.completion.finalLevelId, Is.EqualTo(10));
            Assert.That(story.completion.finaleSceneId, Is.EqualTo("chapter01.finale"));
            Assert.That(story.completion.completionTrigger, Is.EqualTo("chapter.complete"));

            StoryMilestoneData milestone = StoryCatalog.FindMilestoneAfterLevel(10);
            Assert.That(milestone.major, Is.True);
            Assert.That(milestone.sceneId, Is.EqualTo(story.completion.finaleSceneId));
            Assert.That(ChapterLevelCatalog.Find(1, 10).HasStoryTrigger("chapter.complete"), Is.True);
        }

        [Test]
        public void FinalePresentation_LocalizesFiveLinesInStableOrder()
        {
            StorySceneData finale = StoryCatalog.FindScene("chapter01.finale");
            var sequence = new StorySequenceRuntime(finale, StoryCatalog.LoadRussian());
            var speakers = new List<string>();

            while (!sequence.IsComplete)
            {
                speakers.Add(sequence.CurrentLine.Speaker);
                sequence.Advance();
            }

            Assert.That(finale.lines, Has.Length.EqualTo(5));
            Assert.That(
                speakers,
                Is.EqualTo(new[] { "СКАЗИТЕЛЬ", "ЧЁРНЫЙ ВОРОН", "ЖАР-ПТИЦА", "ДОБРЫНЯ", "ИВАН" })
            );
            Assert.That(finale.returnRoute, Is.EqualTo(StoryReturnRoutes.ChapterComplete));
        }

        [Test]
        public void CompletionState_CannotBeForgedFromLastCompletedLevel()
        {
            GameState state = GameState.Instance;
            state.Save.lastCompletedChapter = 1;
            state.Save.lastCompletedLevel = 10;
            state.Persist();

            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.InProgress)
            );
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
        }

        [Test]
        public void Level10Victory_RequiresFinaleBeforeChapterComplete()
        {
            GameState state = GameState.Instance;
            CompleteFinalLevel(state);

            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.FinalePending)
            );
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.finale"));

            Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);

            StoryAdvanceResult result = DrainActiveDialogue();
            Assert.That(result, Is.EqualTo(StoryAdvanceResult.ChapterComplete));
            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.PresentationPending)
            );
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.True);
        }

        [Test]
        public void PendingChapterComplete_SurvivesRuntimeReset()
        {
            GameState state = GameState.Instance;
            CompleteFinale(state);

            StoryService.ResetRuntime();
            state.Save = SaveService.Load();

            Assert.That(StoryService.ShouldShowChapterComplete(), Is.True);
            Assert.That(state.HasPendingChapterCompletion(1), Is.True);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
        }

        [Test]
        public void ChapterCompleteAcknowledgement_IsPersistentAndIdempotent()
        {
            GameState state = GameState.Instance;
            CompleteFinale(state);

            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.False);
            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.Acknowledged)
            );
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);

            PlayerSaveData reloaded = SaveService.Load();
            Assert.That(reloaded.saveVersion, Is.EqualTo(6));
            Assert.That(reloaded.acknowledgedChapterCompletions, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void LegacyV5Save_MigratesAcknowledgementStateToVersion6()
        {
            GameState state = GameState.Instance;
            state.Save = new PlayerSaveData
            {
                saveVersion = 5,
                acknowledgedChapterCompletions = null
            };
            MethodInfo migrate = typeof(GameState).GetMethod(
                "MigrateSave",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            Assert.That(migrate, Is.Not.Null);
            Assert.DoesNotThrow(() => migrate.Invoke(state, null));
            Assert.That(state.Save.saveVersion, Is.EqualTo(6));
            Assert.That(state.Save.acknowledgedChapterCompletions, Is.Not.Null);
            Assert.That(state.Save.acknowledgedChapterCompletions, Is.Empty);
        }

        [Test]
        public void ReplayedLevel10_DoesNotRepeatFinaleCompletionOrRewards()
        {
            GameState state = GameState.Instance;
            LevelCompletionResult first = CompleteFinale(state);
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
            int goldAfterFirstClear = state.Save.gold;

            LevelData finaleLevel = ChapterLevelCatalog.Find(1, 10);
            LevelCompletionResult replay = state.CompleteLevel(
                finaleLevel,
                finaleLevel.starScoreThresholds[2]
            );

            Assert.That(first.FirstClearRewardGranted, Is.True);
            Assert.That(replay.FirstClearRewardGranted, Is.False);
            Assert.That(replay.GoldReward, Is.Zero);
            Assert.That(state.Save.gold, Is.EqualTo(goldAfterFirstClear));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.Acknowledged)
            );
        }

        [Test]
        public void CompletionSummary_UsesSavedProgressValues()
        {
            GameState state = GameState.Instance;
            for (int levelId = 1; levelId <= 10; levelId++)
            {
                LevelData level = ChapterLevelCatalog.Find(1, levelId);
                state.CompleteLevel(level, level.starScoreThresholds[0]);
            }

            ChapterCompletionSummary summary = state.GetChapterCompletionSummary(1);
            Assert.That(summary.CompletedLevels, Is.EqualTo(10));
            Assert.That(summary.TotalLevels, Is.EqualTo(10));
            Assert.That(summary.EarnedStars, Is.EqualTo(10));
            Assert.That(summary.MaximumStars, Is.EqualTo(30));
            Assert.That(summary.Phase, Is.EqualTo(ChapterCompletionPhase.FinalePending));

            LocalizedChapterCompletion localized = StoryService.GetLocalizedChapterCompletion();
            Assert.That(localized.Title, Is.EqualTo("ИТОГ ГЛАВЫ"));
            Assert.That(localized.SummaryFormat, Does.Contain("{3}"));
        }

        [Test]
        public void EarlierPendingMilestone_IsResolvedBeforeFinale()
        {
            GameState state = GameState.Instance;
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);
            CompleteFinalLevel(state);

            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.firebird_found"));
            Assert.That(DrainActiveDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));

            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.finale"));
        }

        [Test]
        public void Validator_RejectsMalformedCompletionContractWithoutThrowing()
        {
            ChapterStoryCatalogData source = StoryCatalog.LoadChapter1();
            ChapterStoryCatalogData malformed = JsonUtility.FromJson<ChapterStoryCatalogData>(
                JsonUtility.ToJson(source)
            );
            malformed.completion.finalLevelId = 9;
            malformed.completion.finaleSceneId = "chapter01.missing";

            StoryValidationReport report = null;
            Assert.DoesNotThrow(() =>
                report = StoryCatalog.Validate(
                    malformed,
                    StoryCatalog.LoadRussian(),
                    ChapterLevelCatalog.LoadChapter1()
                )
            );
            Assert.That(report.IsValid, Is.False);
            Assert.That(string.Join("\n", report.Errors), Does.Contain("terminal released level"));
            Assert.That(string.Join("\n", report.Errors), Does.Contain("missing finale scene"));
        }

        [Test]
        public void FullFirstLaunchRoute_EndsAtAcknowledgedChapterComplete()
        {
            GameState state = GameState.Instance;
            Assert.That(StoryService.TryBeginChapterIntro(), Is.True);
            Assert.That(DrainActiveDialogue(), Is.EqualTo(StoryAdvanceResult.PreLevel));

            var mappedScenes = new Dictionary<int, string>
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
                LevelData level = ChapterLevelCatalog.Find(1, levelId);
                state.CompleteLevel(level, level.starScoreThresholds[0]);

                if (!mappedScenes.TryGetValue(levelId, out string sceneId))
                {
                    Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False, $"Level {levelId}");
                    continue;
                }

                Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True, $"Level {levelId}");
                Assert.That(StoryService.ActiveSceneId, Is.EqualTo(sceneId));
                StoryAdvanceResult route = DrainActiveDialogue();
                Assert.That(
                    route,
                    Is.EqualTo(levelId == 10
                        ? StoryAdvanceResult.ChapterComplete
                        : StoryAdvanceResult.ChapterMap)
                );
            }

            Assert.That(state.Save.completedStoryScenes, Has.Count.EqualTo(8));
            Assert.That(state.HasCompletedChapter(1), Is.True);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.True);
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(
                state.GetChapterCompletionPhase(1),
                Is.EqualTo(ChapterCompletionPhase.Acknowledged)
            );
        }

        static LevelCompletionResult CompleteFinalLevel(GameState state)
        {
            LevelData level = ChapterLevelCatalog.Find(1, 10);
            state.Save.highestUnlockedLevel = 10;
            state.Save.selectedChapter = 1;
            state.Save.selectedLevel = 10;
            state.Persist();
            return state.CompleteLevel(level, level.starScoreThresholds[0]);
        }

        static LevelCompletionResult CompleteFinale(GameState state)
        {
            LevelCompletionResult completion = CompleteFinalLevel(state);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.finale"));
            Assert.That(DrainActiveDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterComplete));
            return completion;
        }

        static StoryAdvanceResult DrainActiveDialogue()
        {
            StoryAdvanceResult result;
            do
            {
                result = StoryService.AdvanceDialogue();
            }
            while (result == StoryAdvanceResult.MoreLines);
            return result;
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
