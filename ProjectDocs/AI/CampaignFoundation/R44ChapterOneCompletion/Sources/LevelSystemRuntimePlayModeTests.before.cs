using System.Collections;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class LevelSystemRuntimePlayModeTests
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
                stateObject = new GameObject("LevelSystemTestState");
                stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }
            else
            {
                previousState = GameState.Instance;
                previousRuntimeSaveJson = JsonUtility.ToJson(previousState.Save);
            }

            GameState.Instance.ResetProgress();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (GameState.Instance != null)
                GameState.Instance.ResetProgress();
            if (ownsStateObject && stateObject != null)
                Object.Destroy(stateObject);
            yield return null;
            RestoreSaveJournal();
            RestoreRuntimeState();
        }

        [Test]
        public void Chapter1Catalog_LoadsTwoHundredValidatedRecords()
        {
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            LevelValidationReport report = LevelDataValidator.ValidateCampaignCatalog(catalog);

            Assert.That(report.Errors, Is.Empty);
            Assert.That(catalog.levels, Has.Length.EqualTo(200));
            Assert.That(catalog.levels[0].id, Is.EqualTo("chapter01.level01"));
            Assert.That(catalog.levels[9].HasStoryTrigger("chapter.complete"), Is.False);
            Assert.That(catalog.schemaVersion, Is.EqualTo(3));
            for (int i = 0; i < catalog.levels.Length; i++)
            {
                Assert.That(catalog.levels[i].obstacles, Is.Not.Null);
                Assert.That(catalog.levels[i].boardItems, Is.Not.Null);
                Assert.That(catalog.levels[i].tutorialSteps, Is.Not.Null);
            }
        }

        [Test]
        public void V112Prototype_ValidatesAndMapsMechanicState()
        {
            ChapterLevelCatalogData source = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            ChapterLevelCatalogData catalog = JsonUtility.FromJson<ChapterLevelCatalogData>(
                JsonUtility.ToJson(source)
            );
            LevelData level = catalog.levels[0];
            level.fixedCells = System.Array.Empty<LevelFixedCellData>();
            level.goals = new[]
            {
                new LevelGoalData
                {
                    type = LevelGoalTypes.ClearObstacle,
                    obstacleType = LevelObstacleTypes.Ice,
                    target = 1
                },
                new LevelGoalData
                {
                    type = LevelGoalTypes.BringDown,
                    itemType = LevelBoardItemTypes.FirebirdFeather,
                    target = 1
                }
            };
            level.obstacles = new[]
            {
                new LevelObstacleData
                {
                    x = 0,
                    y = 0,
                    type = LevelObstacleTypes.Ice,
                    layers = 2
                }
            };
            level.boardItems = new[]
            {
                new LevelBoardItemData
                {
                    x = 2,
                    y = 2,
                    type = LevelBoardItemTypes.FirebirdFeather
                }
            };
            catalog.levels[0] = level;

            LevelValidationReport report = LevelDataValidator.ValidateCampaignCatalog(catalog);
            Assert.That(report.Errors, Is.Empty);

            BoardModel board = LevelBoardFactory.Create(level, level.boardSeed);
            Assert.That(
                board.GetObstacle(new BoardPosition(0, 0)),
                Is.EqualTo(new BoardObstacleData(BoardObstacleKind.Ice, 2))
            );
            Assert.That(
                board.GetObjectiveItem(new BoardPosition(2, 2)),
                Is.EqualTo(BoardObjectiveItemKind.FirebirdFeather)
            );
            Assert.That(board.HasAnyMatches(), Is.False);
            Assert.That(board.HasLegalMove(), Is.True);
        }

        [Test]
        public void V112ScoreAndSurvivalGoalContracts_Validate()
        {
            ChapterLevelCatalogData source = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            ChapterLevelCatalogData catalog = JsonUtility.FromJson<ChapterLevelCatalogData>(
                JsonUtility.ToJson(source)
            );
            LevelData level = catalog.levels[0];
            level.goals = new[]
            {
                new LevelGoalData
                {
                    type = LevelGoalTypes.Score,
                    target = level.startingScore + 500
                },
                new LevelGoalData
                {
                    type = LevelGoalTypes.Survival,
                    target = level.moveLimit
                }
            };

            LevelValidationReport report = LevelDataValidator.ValidateCampaignCatalog(catalog);
            Assert.That(report.Errors, Is.Empty);
        }

        [Test]
        public void V112Validator_RejectsIllegalIceDurability()
        {
            ChapterLevelCatalogData source = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            ChapterLevelCatalogData catalog = JsonUtility.FromJson<ChapterLevelCatalogData>(
                JsonUtility.ToJson(source)
            );
            LevelData level = catalog.levels[0];
            level.obstacles = new[]
            {
                new LevelObstacleData
                {
                    x = 0,
                    y = 0,
                    type = LevelObstacleTypes.Ice,
                    layers = 3
                }
            };

            LevelValidationReport report = LevelDataValidator.ValidateCampaignCatalog(catalog);
            Assert.That(report.IsValid, Is.False);
            Assert.That(
                string.Join("\n", report.Errors),
                Does.Contain("layers must be in [1, 2] for 'ice'")
            );
        }

        [Test]
        public void LevelBoardFactory_HonorsDimensionsAndAuthoredCells()
        {
            LevelData level = ChapterLevelCatalog.Find(1, 5);
            var board = LevelBoardFactory.Create(level, level.boardSeed);

            Assert.That(board.Width, Is.EqualTo(level.columns));
            Assert.That(board.Height, Is.EqualTo(level.rows));
            Assert.That(board.HasAnyMatches(), Is.False);
            Assert.That(board.HasLegalMove(), Is.True);

            LevelFixedCellData authored = level.fixedCells[0];
            Assert.That(
                board.GetTile(new BoardPosition(authored.x, authored.y)),
                Is.EqualTo(authored.tileType)
            );
            Assert.That(
                (int)board.GetSpecial(new BoardPosition(authored.x, authored.y)),
                Is.EqualTo(authored.special)
            );
        }

        [UnityTest]
        public IEnumerator Match3Board_UsesSelectedLevelRecord()
        {
            LevelData level = ChapterLevelCatalog.Find(1, 2);
            GameState.Instance.Save.highestUnlockedLevel = 2;
            GameState.Instance.Persist();
            Assert.That(GameState.Instance.SelectLevel(1, 2), Is.True);
            var canvasObject = new GameObject("LevelCanvas", typeof(RectTransform), typeof(Canvas));
            var boardObject = new GameObject("LevelBoard", typeof(RectTransform), typeof(Match3Board));
            boardObject.transform.SetParent(canvasObject.transform, false);
            Match3Board board = boardObject.GetComponent<Match3Board>();
            board.Initialize(level, null, null);
            yield return null;

            Assert.That(board.CurrentLevel.id, Is.EqualTo(level.id));
            Assert.That(board.ColumnCount, Is.EqualTo(level.columns));
            Assert.That(board.RowCount, Is.EqualTo(level.rows));
            Assert.That(board.Moves, Is.EqualTo(level.moveLimit));
            // Score goals include the authored starting score; collect/obstacle goals start at zero.
            int firstRemaining=level.goals[0].type==LevelGoalTypes.Score
                ? Mathf.Max(0,level.goals[0].target-level.startingScore) : level.goals[0].target;
            int secondRemaining=level.goals[1].type==LevelGoalTypes.Score
                ? Mathf.Max(0,level.goals[1].target-level.startingScore) : level.goals[1].target;
            Assert.That(board.RedRemaining, Is.EqualTo(firstRemaining));
            Assert.That(board.BlueRemaining, Is.EqualTo(secondRemaining));
            Assert.That(board.GoalSlot1.Type, Is.EqualTo(level.goals[0].type));
            Assert.That(board.GoalSlot2.Type, Is.EqualTo(level.goals[1].type));
            Assert.That(board.GoalSlot1Remaining, Is.EqualTo(board.RedRemaining));
            Assert.That(board.GoalSlot2Remaining, Is.EqualTo(board.BlueRemaining));

            Object.Destroy(boardObject);
            Object.Destroy(canvasObject);
            yield return null;
        }

        [Test]
        public void Progression_PersistsSelectionStarsUnlocksRetriesAndRewards()
        {
            GameState state = GameState.Instance;
            LevelData level = ChapterLevelCatalog.Find(1, 1);
            int goldBefore = state.Save.gold;

            state.RegisterLevelAttempt(level);
            state.RecordLevelFailure(level, level.startingScore + 100);
            state.RegisterLevelAttempt(level);
            LevelCompletionResult result = state.CompleteLevel(
                level,
                level.starScoreThresholds[2]
            );

            LevelProgressData progress = state.GetLevelProgress(1, 1);
            Assert.That(progress.attempts, Is.EqualTo(2));
            Assert.That(progress.retries, Is.EqualTo(1));
            Assert.That(progress.failures, Is.EqualTo(1));
            Assert.That(progress.bestStars, Is.EqualTo(3));
            Assert.That(progress.rewardClaimed, Is.True);
            Assert.That(result.FirstClearRewardGranted, Is.True);
            Assert.That(state.Save.highestUnlockedLevel, Is.EqualTo(2));
            Assert.That(state.Save.selectedLevel, Is.EqualTo(2));
            Assert.That(state.Save.stars, Is.EqualTo(3));
            Assert.That(state.Save.gold, Is.EqualTo(goldBefore + level.rewards.gold));

            Assert.That(state.SelectLevel(1, 1), Is.True);
            Assert.That(state.SelectLevel(1, 3), Is.False);

            PlayerSaveData reloaded = SaveService.Load();
            Assert.That(reloaded.selectedLevel, Is.EqualTo(1));
            Assert.That(reloaded.highestUnlockedLevel, Is.EqualTo(2));
            Assert.That(reloaded.levelProgress, Has.Count.EqualTo(1));
            Assert.That(reloaded.levelProgress[0].retries, Is.EqualTo(1));
        }

        [Test]
        public void FirstClearReward_AndDoubleReward_CannotBeClaimedTwice()
        {
            GameState state = GameState.Instance;
            LevelData level = ChapterLevelCatalog.Find(1, 1);
            int goldBefore = state.Save.gold;

            LevelCompletionResult first = state.CompleteLevel(
                level,
                level.starScoreThresholds[0]
            );
            Assert.That(first.GoldReward, Is.EqualTo(level.rewards.gold));
            Assert.That(state.TryDoubleLastLevelGoldReward(), Is.True);
            Assert.That(state.TryDoubleLastLevelGoldReward(), Is.False);

            LevelCompletionResult replay = state.CompleteLevel(
                level,
                level.starScoreThresholds[2]
            );
            Assert.That(replay.FirstClearRewardGranted, Is.False);
            Assert.That(replay.GoldReward, Is.Zero);
            Assert.That(
                state.Save.gold,
                Is.EqualTo(goldBefore + level.rewards.gold * 2)
            );
        }

        [Test]
        public void TutorialProgress_PersistsAndCannotBeCompletedTwice()
        {
            GameState state = GameState.Instance;
            const string tutorialId = "tutorial.match.start";

            Assert.That(state.HasCompletedTutorial(tutorialId), Is.False);
            Assert.That(state.CompleteTutorial(tutorialId), Is.True);
            Assert.That(state.CompleteTutorial(tutorialId), Is.False);
            Assert.That(state.HasCompletedTutorial(tutorialId), Is.True);

            PlayerSaveData reloaded = SaveService.Load();
            Assert.That(reloaded.saveVersion, Is.EqualTo(6));
            Assert.That(reloaded.completedTutorialSteps, Does.Contain(tutorialId));
            Assert.That(reloaded.completedStoryScenes, Is.Not.Null);
            Assert.That(reloaded.acknowledgedChapterCompletions, Is.Not.Null);
        }

        [Test]
        public void StoryRouting_UsesCompletedLevelTriggers()
        {
            GameState state = GameState.Instance;
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);

            Assert.That(StoryService.ShouldShowDialogueAfterVictory(), Is.True);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);

            state.ResetProgress();
            StoryService.ResetRuntime();

            LevelData finale = ChapterLevelCatalog.Find(1, 10);
            state.Save.highestUnlockedLevel = 10;
            state.Save.selectedLevel = 10;
            state.Persist();
            state.CompleteLevel(finale, finale.starScoreThresholds[0]);

            Assert.That(StoryService.ShouldShowDialogueAfterVictory(), Is.True);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("campaign.chapter01.hearth_secured"));

            StoryAdvanceResult result;
            do
            {
                result = StoryService.AdvanceDialogue();
            }
            while (result == StoryAdvanceResult.MoreLines);

            Assert.That(result, Is.EqualTo(StoryAdvanceResult.ChapterMap));
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(state.Save.highestUnlockedLevel, Is.EqualTo(11));
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
