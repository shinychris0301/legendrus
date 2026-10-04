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
        public void Chapter1Catalog_LoadsFiveHundredValidatedRecords()
        {
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            LevelValidationReport report = LevelDataValidator.ValidateCampaignCatalog(catalog);

            Assert.That(report.Errors, Is.Empty);
            Assert.That(catalog.levels, Has.Length.EqualTo(500));
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

        static LevelData FirstClearBoundaryFixture()
        {
            var level = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(ChapterLevelCatalog.Find(1, 1)));
            level.rewards = new LevelRewardData { gold = 1, gems = 1, boosterMace = 1,
                boosterFirebird = 1, boosterLivingLight = 1, boosterSeaPearl = 1 };
            return level;
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void FirstClearArithmeticRejectionPreservesRewardsProgressAndJournal(int boundary)
        {
            var state = GameState.Instance;
            var level = FirstClearBoundaryFixture();
            state.RegisterLevelAttempt(level);
            switch (boundary)
            {
                case 0: state.Save.gold = int.MaxValue; break;
                case 1: state.Save.gems = int.MaxValue; break;
                case 2: state.Save.boosterMace = int.MaxValue; break;
                case 3: state.Save.boosterFirebird = int.MaxValue; break;
                case 4: state.Save.boosterLivingLight = int.MaxValue; break;
                case 5: state.Save.boosterSeaPearl = int.MaxValue; break;
                case 6:
                    var progress = state.GetLevelProgress(1, 1);
                    progress.completions = int.MaxValue; progress.rewardClaimed = true;
                    break;
            }
            state.Persist();
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var priorWrite = state.LastWriteResult; var outcome = state.LastCompletionResult;
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < present.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    System.Exception rejection = null;
                    bool ignoredLogs = LogAssert.ignoreFailingMessages;
                    try
                    {
                        LogAssert.ignoreFailingMessages = true;
                        try { state.CompleteLevel(level, level.starScoreThresholds[2]); }
                        catch (System.InvalidOperationException ex) { rejection = ex; }
                    }
                    finally { LogAssert.ignoreFailingMessages = ignoredLogs; }
                    Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "No partial credit or progress, boundary " + boundary);
                    Assert.That(rejection, Is.Not.Null, "Compatibility API must not report a committed completion.");
                }
                Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastCompletionResult, Is.SameAs(outcome));
                Assert.That(state.LastWriteResult, Is.SameAs(priorWrite)); Assert.That(state.CanPersist, Is.True);
                Assert.That(events, Is.Zero);
                for (int i = 0; i < present.Length; i++)
                {
                    Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[i]), Is.EqualTo(present[i]));
                    Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[i]), Is.EqualTo(journal[i]));
                }
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void FirstClearBlockedSaveCannotPublishRewardsOrProgress()
        {
            var state = GameState.Instance; var level = FirstClearBoundaryFixture();
            state.RegisterLevelAttempt(level);
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var savedJournal = new string[PersistedSaveKeys.Length]; var savedPresent = new bool[savedJournal.Length];
            for (int i = 0; i < savedJournal.Length; i++)
            { savedPresent[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); savedJournal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            Assert.That(SaveService.LoadWithResult().Status, Is.EqualTo(PlayerSaveLoadStatus.UnsupportedFutureVersion));
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                System.Exception rejection = null; bool ignoredLogs = LogAssert.ignoreFailingMessages;
                try
                {
                    LogAssert.ignoreFailingMessages = true;
                    try { state.CompleteLevel(level, level.starScoreThresholds[2]); }
                    catch (System.InvalidOperationException ex) { rejection = ex; }
                }
                finally { LogAssert.ignoreFailingMessages = ignoredLogs; }
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(rejection, Is.Not.Null); Assert.That(state.LastCompletionResult, Is.Null); Assert.That(events, Is.Zero);
                Assert.That(state.LastWriteResult.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                Assert.That(state.CanPersist, Is.False);
                var blockedWrite = state.LastWriteResult;
                Assert.That(state.TryCompleteLevel(level, level.starScoreThresholds[2], out var leaked), Is.False);
                Assert.That(leaked, Is.Null); Assert.That(state.LastWriteResult, Is.SameAs(blockedWrite));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]), Is.False); Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                state.Changed -= changed;
                for (int i = 0; i < savedJournal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (savedPresent[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], savedJournal[i]); }
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void FirstClearFullGrantFitsExactLimitsAndReplayCannotCreditAgain()
        {
            var state = GameState.Instance; var level = FirstClearBoundaryFixture();
            Assert.That(state.SpendEnergy(1), Is.True);
            state.Save.gold = state.Save.gems = state.Save.boosterMace = state.Save.boosterFirebird =
                state.Save.boosterLivingLight = state.Save.boosterSeaPearl = int.MaxValue - 1;
            state.RegisterLevelAttempt(level);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryCompleteLevel(level, level.starScoreThresholds[2], out var first), Is.True);
                Assert.That(first, Is.SameAs(state.LastCompletionResult)); Assert.That(first.FirstClearRewardGranted, Is.True);
                Assert.That(first.GoldReward, Is.EqualTo(1)); Assert.That(first.GemReward, Is.EqualTo(1));
                Assert.That(events, Is.EqualTo(1)); Assert.That(state.GetLevelProgress(1, 1).completions, Is.EqualTo(1));
                Assert.That(state.GetLevelProgress(1, 1).rewardClaimed, Is.True);
                Assert.That(state.Save.gold, Is.EqualTo(int.MaxValue)); Assert.That(state.Save.gems, Is.EqualTo(int.MaxValue));
                foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))Assert.That(state.BoosterCount(type), Is.EqualTo(int.MaxValue));
                Assert.That(state.Save.highestUnlockedLevel, Is.EqualTo(2)); Assert.That(state.Save.selectedLevel, Is.EqualTo(2));
                Assert.That(state.Save.stars, Is.EqualTo(3)); Assert.That(state.Save.storyStage, Is.EqualTo(1));
                var loaded = SaveService.Load(); Assert.That(JsonUtility.ToJson(loaded), Is.EqualTo(JsonUtility.ToJson(state.Save)));
                state.Save = loaded;
                Assert.That(state.TryCompleteLevel(level, level.starScoreThresholds[0], out var replay), Is.True);
                Assert.That(replay.FirstClearRewardGranted, Is.False); Assert.That(replay.GoldReward, Is.Zero); Assert.That(replay.GemReward, Is.Zero);
                Assert.That(events, Is.EqualTo(2)); Assert.That(state.GetLevelProgress(1, 1).completions, Is.EqualTo(2));
                Assert.That(state.Save.stars, Is.EqualTo(3)); Assert.That(state.Save.gold, Is.EqualTo(int.MaxValue)); Assert.That(state.Save.gems, Is.EqualTo(int.MaxValue));
                foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))Assert.That(state.BoosterCount(type), Is.EqualTo(int.MaxValue));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void FirstClearExistingInvalidArgumentsAndNonpositiveRewardsRemainGuarded()
        {
            var state = GameState.Instance;
            Assert.Throws<System.ArgumentNullException>(() => state.TryCompleteLevel(null, 100, out _));
            Assert.Throws<System.InvalidOperationException>(() => state.TryCompleteLevel(ChapterLevelCatalog.Find(1, 2), 100, out _));
            var level = FirstClearBoundaryFixture();
            level.rewards = new LevelRewardData { gold = -1, gems = -1, boosterMace = -1,
                boosterFirebird = -1, boosterLivingLight = -1, boosterSeaPearl = -1 };
            int gold = state.Save.gold, gems = state.Save.gems;
            var boosters = new int[4]; for (int i = 0; i < 4; i++)boosters[i] = state.BoosterCount((BoosterType)i);
            Assert.That(state.TryCompleteLevel(level, level.starScoreThresholds[0], out var result), Is.True);
            Assert.That(result.FirstClearRewardGranted, Is.True); Assert.That(result.GoldReward, Is.Zero); Assert.That(result.GemReward, Is.Zero);
            Assert.That(state.Save.lastRewardEligibleForDouble, Is.False);
            Assert.That(state.Save.gold, Is.EqualTo(gold)); Assert.That(state.Save.gems, Is.EqualTo(gems));
            for (int i = 0; i < 4; i++)Assert.That(state.BoosterCount((BoosterType)i), Is.EqualTo(boosters[i]));
        }

        static void SetOrdinaryCreditBalance(PlayerSaveData save, int resource, int value)
        {
            switch (resource)
            {
                case 0: save.gold = value; break;
                case 1: save.gems = value; break;
                case 2: save.boosterMace = value; break;
                case 3: save.boosterFirebird = value; break;
                case 4: save.boosterLivingLight = value; break;
                case 5: save.boosterSeaPearl = value; break;
                default: Assert.Fail("Unknown credit resource."); break;
            }
        }

        static void AddOrdinaryCredit(GameState state, int resource, int amount)
        {
            if (resource == 0) state.AddGold(amount);
            else if (resource == 1) state.AddGems(amount);
            else state.AddBooster((BoosterType)(resource - 2), amount);
        }

        static bool TryOrdinaryCredit(GameState state, int resource, int amount)
        {
            if (resource == 0) return state.TryAddGold(amount);
            if (resource == 1) return state.TryAddGems(amount);
            return state.TryAddBooster((BoosterType)(resource - 2), amount);
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        [TestCase(3)] [TestCase(4)] [TestCase(5)]
        public void OrdinaryCreditOverflowPreservesLiveSaveAndJournal(int resource)
        {
            var state = GameState.Instance;
            SetOrdinaryCreditBalance(state.Save, resource, int.MaxValue - 1);
            state.Persist();
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var priorWrite = state.LastWriteResult;
            var journal = new string[PersistedSaveKeys.Length]; var present = new bool[journal.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignoredLogs = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    AddOrdinaryCredit(state, resource, 2);
                    Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "Full credit must be rejected without wrapping or partial mutation.");
                    Assert.That(TryOrdinaryCredit(state, resource, 2), Is.False);
                }
                Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite));
                Assert.That(state.CanPersist, Is.True); Assert.That(events, Is.Zero);
                for (int i = 0; i < journal.Length; i++)
                { Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[i]), Is.EqualTo(present[i])); Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[i]), Is.EqualTo(journal[i])); }
            }
            finally { LogAssert.ignoreFailingMessages = ignoredLogs; state.Changed -= changed; }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        [TestCase(3)] [TestCase(4)] [TestCase(5)]
        public void OrdinaryCreditBlockedWritePreservesLiveSaveAndFutureJournal(int resource)
        {
            var state = GameState.Instance; var original = state.Save; string before = JsonUtility.ToJson(original);
            var journal = new string[PersistedSaveKeys.Length]; var present = new bool[journal.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignoredLogs = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                AddOrdinaryCredit(state, resource, 1);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(events, Is.Zero); Assert.That(state.CanPersist, Is.False);
                Assert.That(state.LastWriteResult.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                var blockedWrite = state.LastWriteResult;
                AddOrdinaryCredit(state, resource, 1);
                for (int other = 0; other < 6; other++) Assert.That(TryOrdinaryCredit(state, other, 1), Is.False);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(blockedWrite));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                LogAssert.ignoreFailingMessages = ignoredLogs; state.Changed -= changed;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        [TestCase(3)] [TestCase(4)] [TestCase(5)]
        public void OrdinaryCreditFullLimitReloadAndOverflowRecovery(int resource)
        {
            var state = GameState.Instance;
            Assert.That(state.SpendEnergy(1), Is.True);
            SetOrdinaryCreditBalance(state.Save, resource, int.MaxValue - 2); state.Persist();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(TryOrdinaryCredit(state, resource, 2), Is.True); Assert.That(events, Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
                var atLimit = state.Save; var priorWrite = state.LastWriteResult;
                Assert.That(TryOrdinaryCredit(state, resource, 1), Is.False);
                Assert.That(state.Save, Is.SameAs(atLimit)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite));
                Assert.That(events, Is.EqualTo(1)); Assert.That(state.CanPersist, Is.True);
                bool spent = resource == 0 ? state.SpendGold(1) : resource == 1 ? state.SpendGems(1) : state.TryUseBooster((BoosterType)(resource - 2));
                Assert.That(spent, Is.True); Assert.That(TryOrdinaryCredit(state, resource, 1), Is.True);
                Assert.That(events, Is.EqualTo(3));
                var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
                SetOrdinaryCreditBalance(expected, resource, int.MaxValue);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        [TestCase(3)] [TestCase(4)] [TestCase(5)]
        public void OrdinaryCreditLargestGrantPreservesOtherResources(int resource)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(1), Is.True);
            SetOrdinaryCreditBalance(state.Save, resource, 0); state.Persist();
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            SetOrdinaryCreditBalance(expected, resource, int.MaxValue);
            AddOrdinaryCredit(state, resource, int.MaxValue);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
        }

        [Test]
        public void OrdinaryCreditNonpositiveAndUnknownBoosterKeepLegacyBehavior()
        {
            var state = GameState.Instance; state.Persist();
            string before = JsonUtility.ToJson(state.Save);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                foreach (int amount in new[] { 0, -1, int.MinValue })
                {
                    Assert.That(state.TryAddGold(amount), Is.True); Assert.That(state.TryAddGems(amount), Is.True);
                    foreach (BoosterType type in System.Enum.GetValues(typeof(BoosterType)))
                        Assert.That(state.TryAddBooster(type, amount), Is.True);
                }
                Assert.That(events, Is.EqualTo(6), "Gold/gem no-ops still persist; booster no-ops do not.");
                var original = state.Save; var priorWrite = state.LastWriteResult;
                foreach (int amount in new[] { 0, 1, int.MaxValue })
                {
                    state.AddBooster((BoosterType)99, amount);
                    Assert.That(state.TryAddBooster((BoosterType)99, amount), Is.False);
                }
                Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(events, Is.EqualTo(6));
            }
            finally { state.Changed -= changed; }
        }

        static bool SpendResource(GameState state, int resource, int amount = 1)
        {
            if (resource == 0) return state.SpendGold(amount);
            if (resource == 1) return state.SpendGems(amount);
            if (resource == 2) return state.SpendEnergy(amount);
            if (resource == 7) return state.TryBeginBoosterCommand(BoosterType.Firebird, out _);
            return state.TryUseBooster((BoosterType)(resource - 3));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)]
        public void SpendingBlockedWriteCannotDebitOrReportSuccess(int resource)
        {
            var state = GameState.Instance;
            Assert.That(state.SpendEnergy(1), Is.True);
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                bool spent = SpendResource(state, resource);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "No debit before durable commitment.");
                Assert.That(spent, Is.False); Assert.That(state.Save, Is.SameAs(original)); Assert.That(events, Is.Zero);
                Assert.That(state.CanPersist, Is.False); var failedWrite = state.LastWriteResult;
                Assert.That(failedWrite.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                for (int other = 0; other < 8; other++) Assert.That(SpendResource(state, other), Is.False);
                Assert.That(state.LastWriteResult, Is.SameAs(failedWrite)); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                LogAssert.ignoreFailingMessages = ignored; state.Changed -= changed;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void SpendingNegativeEnergyIsRejectedWithoutMintingOrClockMutation()
        {
            var state = GameState.Instance; string before = JsonUtility.ToJson(state.Save); var priorWrite = state.LastWriteResult;
            Assert.That(state.SpendEnergy(-1), Is.False);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        [TestCase(4)] [TestCase(5)] [TestCase(6)]
        public void SpendingSuccessCommitsOnlyItsBalanceAndReloads(int resource)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(1), Is.True);
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            if (resource == 0) expected.gold--;
            else if (resource == 1) expected.gems--;
            else if (resource == 2) expected.energy--;
            else SetOrdinaryCreditBalance(expected, resource - 1, state.BoosterCount((BoosterType)(resource - 3)) - 1);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(SpendResource(state, resource), Is.True); Assert.That(events, Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void SpendingInvalidInsufficientAndEnergyClockBoundaries()
        {
            var state = GameState.Instance; var original = state.Save; var write = state.LastWriteResult;
            string before = JsonUtility.ToJson(original);
            Assert.That(state.SpendGold(-1) || state.SpendGems(-1) || state.SpendEnergy(int.MinValue), Is.False);
            Assert.That(state.SpendGold(int.MaxValue) || state.SpendGems(int.MaxValue) || state.SpendEnergy(int.MaxValue), Is.False);
            Assert.That(state.TryUseBooster((BoosterType)99), Is.False);
            Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastWriteResult, Is.SameAs(write));
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
            state.Save.energy = 0; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-31).Ticks; state.Persist();
            Assert.That(state.SpendEnergy(), Is.True); Assert.That(state.Save.energy, Is.Zero);
            Assert.That(state.SpendEnergy(), Is.False); Assert.That(state.Save.energy, Is.Zero);
            Assert.That(state.SpendGold(0) && state.SpendGems(0) && state.SpendEnergy(0), Is.True);
        }

        [TestCase(BoosterType.BogatyrMace, false)] [TestCase(BoosterType.Firebird, false)]
        [TestCase(BoosterType.LivingLight, false)] [TestCase(BoosterType.SeaPearl, false)]
        [TestCase(BoosterType.BogatyrMace, true)] [TestCase(BoosterType.Firebird, true)]
        [TestCase(BoosterType.LivingLight, true)] [TestCase(BoosterType.SeaPearl, true)]
        public void SpendingBoosterReceiptSettlesOnceAndRejectsConflictingSettlement(BoosterType type, bool applied)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(1), Is.True); int before = state.BoosterCount(type);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryBeginBoosterCommand(type, out string id), Is.True);
                Assert.That(System.Guid.TryParseExact(id, "N", out _), Is.True);
                Assert.That(state.BoosterCount(type), Is.EqualTo(before - 1)); Assert.That(state.HasPendingBoosterCommand, Is.True);
                Assert.That(SaveService.Load().boosterCommand.commandId, Is.EqualTo(id));
                Assert.That(state.TryBeginBoosterCommand(type, out _), Is.False);
                Assert.That(state.TryCompleteLevel(ChapterLevelCatalog.Find(1,1),15000,out var premature),Is.False);
                Assert.That(premature,Is.Null);
                Assert.That(state.TrySettleBoosterCommand(id, applied), Is.True);
                Assert.That(state.BoosterCount(type), Is.EqualTo(applied ? before - 1 : before));
                Assert.That(state.HasPendingBoosterCommand, Is.False); Assert.That(events, Is.EqualTo(2));
                var written = state.LastWriteResult; var committed = state.Save;
                Assert.That(state.TrySettleBoosterCommand(id, applied), Is.True);
                Assert.That(state.TrySettleBoosterCommand(id, !applied), Is.False);
                Assert.That(state.Save, Is.SameAs(committed)); Assert.That(state.LastWriteResult, Is.SameAs(written)); Assert.That(events, Is.EqualTo(2));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(BoosterType.BogatyrMace)] [TestCase(BoosterType.Firebird)]
        [TestCase(BoosterType.LivingLight)] [TestCase(BoosterType.SeaPearl)]
        public void SpendingFreshGameStateRecoversAbandonedReceiptExactlyOnce(BoosterType type)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(1), Is.True); int before = state.BoosterCount(type);
            Assert.That(state.TryBeginBoosterCommand(type, out string id), Is.True);
            Assert.That(SaveService.Load().boosterCommand.state, Is.EqualTo(CampaignBoosterState.Reserved));
            var freshObject = new GameObject("FreshSpendingRecoveryState");
            try
            {
                var fresh = freshObject.AddComponent<GameState>();
                Assert.That(GameState.Instance, Is.SameAs(fresh)); Assert.That(fresh.HasPendingBoosterCommand, Is.False);
                Assert.That(fresh.Save.boosterCommand.commandId, Is.EqualTo(id));
                Assert.That(fresh.Save.boosterCommand.state, Is.EqualTo(CampaignBoosterState.Refunded));
                Assert.That(fresh.BoosterCount(type), Is.EqualTo(before));
                var write = fresh.LastWriteResult;
                Assert.That(fresh.TryRecoverPendingBoosterCommand(), Is.True);
                Assert.That(fresh.TrySettleBoosterCommand(id, false), Is.True); Assert.That(fresh.LastWriteResult, Is.SameAs(write));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(fresh.Save)));
            }
            finally
            {
                typeof(GameState).GetProperty("Instance").SetValue(null, state);
                Object.Destroy(freshObject);
            }
        }

        [Test]
        public void SpendingStaleReceiptCannotCreditNewCommandOrResetHistory()
        {
            var state = GameState.Instance;
            Assert.That(state.TryBeginBoosterCommand(BoosterType.Firebird, out string old), Is.True);
            Assert.That(state.TrySettleBoosterCommand(old, false), Is.True);
            Assert.That(state.TryBeginBoosterCommand(BoosterType.Firebird, out string current), Is.True);
            Assert.That(current, Is.Not.EqualTo(old)); string before = JsonUtility.ToJson(state.Save);
            Assert.That(state.TrySettleBoosterCommand(old, false), Is.False); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
            state.ResetProgress(); before = JsonUtility.ToJson(state.Save);
            Assert.That(state.TrySettleBoosterCommand(current, false), Is.False); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
        public void SpendingMalformedReceiptCannotBeWritten(int invalid)
        {
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            candidate.boosterCommand = new BoosterCommandReceipt { commandId = System.Guid.NewGuid().ToString("N"), state = CampaignBoosterState.Reserved };
            if (invalid == 0) candidate.boosterCommand.schemaVersion = 2;
            if (invalid == 1) candidate.boosterCommand.commandId = "invalid";
            if (invalid == 2) candidate.boosterCommand.type = (BoosterType)99;
            if (invalid == 3) candidate.boosterCommand.state = (CampaignBoosterState)99;
            if (invalid == 4) candidate.boosterCommand.state = CampaignBoosterState.None;
            string primary = PlayerPrefs.GetString(PersistedSaveKeys[0]);
            Assert.That(SaveService.TrySave(candidate, out var result), Is.False);
            Assert.That(result.Status, Is.EqualTo(PlayerSaveWriteStatus.InvalidInput));
            Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(primary));
        }

        [Test]
        public void SpendingLegacyV6WithoutReceiptStillLoads()
        {
            var save = GameState.Instance.Save;
            string raw = JsonUtility.ToJson(save).Replace(",\"boosterCommand\":" + JsonUtility.ToJson(save.boosterCommand), "");
            Assert.That(raw.Contains("boosterCommand"), Is.False);
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            var loaded = SaveService.LoadWithResult(); Assert.That(loaded.CanPersist, Is.True);
            Assert.That(loaded.Data.boosterCommand.state, Is.EqualTo(CampaignBoosterState.None));
            Assert.That(loaded.Data.gold, Is.EqualTo(save.gold)); Assert.That(loaded.Data.boosterFirebird, Is.EqualTo(save.boosterFirebird));
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

        [TestCase(1)] [TestCase(3)] [TestCase(5)]
        public void EnergyGrantLargestAmountRespectsExistingCap(int energy)
        {
            var state = GameState.Instance;
            state.Save.energy = energy; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            state.AddEnergy(int.MaxValue);
            Assert.That(state.Save.energy, Is.EqualTo(state.Save.energyMax));
            Assert.That(SaveService.Load().energy, Is.EqualTo(state.Save.energyMax));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void EnergyMutationBlockedSavePreservesBalanceClockAndEvents(int operation)
        {
            var state = GameState.Instance;
            state.Save.energy = operation == 3 ? state.Save.energyMax : 1;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                if (operation == 3) Assert.That(state.SpendGold(1), Is.False);
                if (operation == 0) state.AddEnergy(2);
                else if (operation == 1) state.RefillEnergy();
                else state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "Energy and its clock cannot publish before saving.");
                Assert.That(state.Save, Is.SameAs(original)); Assert.That(events, Is.Zero);
                Assert.That(state.CanPersist, Is.False); var failedWrite = state.LastWriteResult;
                Assert.That(failedWrite.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                state.AddEnergy(1); state.RefillEnergy(); state.RefreshEnergyFromClock();
                Assert.That(state.TryAddEnergy(1) || state.TryRefillEnergy() || state.TryRefreshEnergyFromClock(), Is.False);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(events, Is.Zero);
                Assert.That(state.LastWriteResult, Is.SameAs(failedWrite));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                LogAssert.ignoreFailingMessages = ignored; state.Changed -= changed;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
        }

        [TestCase(false)] [TestCase(true)]
        public void EnergyClockLargestCapCannotWrapDuringRefreshOrSpending(bool spend)
        {
            var state = GameState.Instance;
            state.Save.energyMax = int.MaxValue; state.Save.energy = int.MaxValue - 1;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            if (spend)
            {
                Assert.That(state.SpendEnergy(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(int.MaxValue - 1));
            }
            else
            {
                state.RefreshEnergyFromClock();
                Assert.That(state.Save.energy, Is.EqualTo(int.MaxValue));
            }
            Assert.That(SaveService.Load().energy, Is.EqualTo(state.Save.energy));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void EnergyTryApisCommitOnceAndPreserveUnrelatedSave(int operation)
        {
            var state = GameState.Instance;
            state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(operation == 2 ? -61 : 0).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            long before = System.DateTime.UtcNow.Ticks;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                bool committed = operation == 0 ? state.TryAddEnergy(2) : operation == 1 ? state.TryRefillEnergy() : state.TryRefreshEnergyFromClock();
                Assert.That(committed, Is.True); Assert.That(events, Is.EqualTo(1));
                expected.energy = operation == 1 ? expected.energyMax : 3;
                if (operation == 1)
                {
                    Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                    expected.lastEnergyUtcTicks = state.Save.lastEnergyUtcTicks;
                }
                else if (operation == 2) expected.lastEnergyUtcTicks += System.TimeSpan.FromMinutes(60).Ticks;
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(-10, 1)] [TestCase(29, 1)] [TestCase(30, 2)] [TestCase(61, 3)] [TestCase(1000, 5)]
        public void EnergyClockBoundariesPreserveRemainderAndReload(int minutes, int expectedEnergy)
        {
            var state = GameState.Instance;
            state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-minutes).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            var original = state.Save; long oldTicks = original.lastEnergyUtcTicks;
            var priorWrite = state.LastWriteResult; long before = System.DateTime.UtcNow.Ticks;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(expectedEnergy));
                Assert.That(events, Is.EqualTo(expectedEnergy > 1 ? 1 : 0));
                if (expectedEnergy == 1)
                { Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite)); }
                if (expectedEnergy == state.Save.energyMax)
                    Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                else Assert.That(state.Save.lastEnergyUtcTicks, Is.EqualTo(oldTicks + (expectedEnergy - 1) * System.TimeSpan.FromMinutes(GameState.EnergyRefillMinutes).Ticks));
                var committed = state.LastWriteResult; long settledTicks = state.Save.lastEnergyUtcTicks;
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True);
                Assert.That(state.Save.lastEnergyUtcTicks, Is.EqualTo(settledTicks));
                Assert.That(state.LastWriteResult, Is.SameAs(committed));
                Assert.That(events, Is.EqualTo(expectedEnergy > 1 ? 1 : 0));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void EnergyAtFullDoesNotRewriteClockAndSpendingStartsRealCountdown()
        {
            var state = GameState.Instance;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddHours(-2).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
            var original = state.Save; string before = JsonUtility.ToJson(original); var write = state.LastWriteResult;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True); state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(state.LastWriteResult, Is.SameAs(write)); Assert.That(events, Is.Zero);
                Assert.That(state.EnergyRemaining(), Is.EqualTo(System.TimeSpan.Zero));
                long start = System.DateTime.UtcNow.Ticks; Assert.That(state.SpendEnergy(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(4)); Assert.That(events, Is.EqualTo(1));
                Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(start, System.DateTime.UtcNow.Ticks));
                Assert.That(state.EnergyRemaining().TotalMinutes, Is.InRange(29.9, 30.0));
                Assert.That(SaveService.Load().lastEnergyUtcTicks, Is.EqualTo(state.Save.lastEnergyUtcTicks));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(int.MinValue)] [TestCase(-1)] [TestCase(0)] [TestCase(int.MaxValue)]
        public void EnergyGrantNonpositiveAndLargestAmountsRetainCapBehavior(int amount)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(2), Is.True);
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            long before = System.DateTime.UtcNow.Ticks;
            Assert.That(state.TryAddEnergy(amount), Is.True);
            if (amount > 0)
            {
                expected.energy = expected.energyMax;
                Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                expected.lastEnergyUtcTicks = state.Save.lastEnergyUtcTicks;
            }
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
        public void NarrativeBlockedWriteKeepsAcknowledgementPendingAndDialogueVisible(int operation)
        {
            var state = GameState.Instance; var story = StoryCatalog.LoadChapter1();
            StoryService.ResetRuntime();
            if (operation == 1 || operation == 3)
            {
                state.Save.highestUnlockedLevel = 500;
                state.Save.levelProgress.Add(new LevelProgressData { chapterId = 1, levelId = 500, completions = 1, bestStars = 3, rewardClaimed = true });
                state.Save.lastCompletedChapter = 1; state.Save.lastCompletedLevel = 500;
                Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
                if (operation == 1) Assert.That(state.CompleteStoryScene(story.completion.finaleSceneId), Is.True);
            }
            if (operation == 2)
            {
                var level = ChapterLevelCatalog.Find(1, 1); state.CompleteLevel(level, level.starScoreThresholds[0]);
            }
            bool service = operation >= 2;
            if (service)
            {
                Assert.That(operation == 4 ? StoryService.TryBeginChapterIntro() : StoryService.TryBeginPostVictoryDialogue(), Is.True);
                var scene = StoryCatalog.FindScene(StoryService.ActiveSceneId);
                for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            }
            string sceneId = StoryService.ActiveSceneId;
            var finalLine = StoryService.CurrentLine;
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                bool acknowledged = service ? StoryService.AdvanceDialogue().ToString() != "SaveRejected" :
                    operation == 0 ? state.CompleteStoryScene(story.introSceneId) : state.AcknowledgeChapterCompletion(1);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "Rejected story/chapter acknowledgement must remain unread/pending.");
                Assert.That(acknowledged, Is.False); Assert.That(events, Is.Zero); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(state.CanPersist, Is.False); var failedWrite = state.LastWriteResult;
                if (service)
                {
                    Assert.That(StoryService.ActiveSceneId, Is.EqualTo(sceneId));
                    Assert.That(StoryService.CurrentLine.Text, Is.EqualTo(finalLine.Text));
                    Assert.That(StoryService.AdvanceDialogue().ToString(), Is.EqualTo("SaveRejected"));
                }
                else Assert.That(operation == 0 ? state.CompleteStoryScene(story.introSceneId) : state.AcknowledgeChapterCompletion(1), Is.False);
                Assert.That(state.LastWriteResult, Is.SameAs(failedWrite)); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                StoryService.ResetRuntime(); state.Changed -= changed; LogAssert.ignoreFailingMessages = ignored;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
            var freshObject = new GameObject("FreshNarrativeRecoveryState");
            try
            {
                var fresh = freshObject.AddComponent<GameState>(); Assert.That(fresh.CanPersist, Is.True);
                int gold = fresh.Save.gold, gems = fresh.Save.gems, energy = fresh.Save.energy;
                if (operation == 1)
                {
                    Assert.That(fresh.HasPendingChapterCompletion(1), Is.True);
                    Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
                    Assert.That(StoryService.AcknowledgeChapterComplete(), Is.False);
                    Assert.That(fresh.GetChapterCompletionPhase(1), Is.EqualTo(ChapterCompletionPhase.Acknowledged));
                }
                else if (service)
                {
                    Assert.That(operation == 4 ? StoryService.TryBeginChapterIntro() : StoryService.TryBeginPostVictoryDialogue(), Is.True);
                    Assert.That(StoryService.ActiveSceneId, Is.EqualTo(sceneId));
                    var scene = StoryCatalog.FindScene(sceneId);
                    for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
                    Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(operation == 4 ? StoryAdvanceResult.PreLevel : operation == 3 ? StoryAdvanceResult.ChapterComplete : StoryAdvanceResult.ChapterMap));
                    Assert.That(fresh.HasCompletedStoryScene(sceneId), Is.True);
                    Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
                }
                else
                {
                    Assert.That(fresh.HasCompletedStoryScene(story.introSceneId), Is.False);
                    Assert.That(fresh.CompleteStoryScene(story.introSceneId), Is.True);
                    Assert.That(fresh.CompleteStoryScene(story.introSceneId), Is.False);
                }
                Assert.That(fresh.Save.gold, Is.EqualTo(gold)); Assert.That(fresh.Save.gems, Is.EqualTo(gems)); Assert.That(fresh.Save.energy, Is.EqualTo(energy));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(fresh.Save)));
            }
            finally
            {
                StoryService.ResetRuntime(); typeof(GameState).GetProperty("Instance").SetValue(null, state); Object.Destroy(freshObject);
            }
        }

        [TestCase(false)] [TestCase(true)]
        public void NarrativeAcknowledgementCommitsOnceWithoutOtherSaveMutation(bool chapter)
        {
            var state = GameState.Instance; var story = StoryCatalog.LoadChapter1();
            if (chapter)
            {
                state.Save.highestUnlockedLevel = 500;
                state.Save.levelProgress.Add(new LevelProgressData { chapterId = 1, levelId = 500, completions = 1, bestStars = 3, rewardClaimed = true });
                Assert.That(state.CompleteStoryScene(story.completion.finaleSceneId), Is.True);
            }
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            if (chapter) expected.acknowledgedChapterCompletions.Add(1);
            else expected.completedStoryScenes.Add(story.introSceneId);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(chapter ? state.AcknowledgeChapterCompletion(1) : state.CompleteStoryScene(story.introSceneId), Is.True);
                Assert.That(events, Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
                var committed = state.LastWriteResult;
                Assert.That(chapter ? state.AcknowledgeChapterCompletion(1) : state.CompleteStoryScene(story.introSceneId), Is.False);
                Assert.That(events, Is.EqualTo(1)); Assert.That(state.LastWriteResult, Is.SameAs(committed));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void NarrativeInvalidAndIneligibleAcknowledgementsDoNotInitializeOrWrite()
        {
            var state = GameState.Instance; state.Save.completedStoryScenes = null; state.Save.acknowledgedChapterCompletions = null;
            string before = JsonUtility.ToJson(state.Save); var write = state.LastWriteResult;
            Assert.That(state.CompleteStoryScene(null) || state.CompleteStoryScene("") || state.CompleteStoryScene(" "), Is.False);
            Assert.That(state.AcknowledgeChapterCompletion(-1) || state.AcknowledgeChapterCompletion(0) || state.AcknowledgeChapterCompletion(1) || state.AcknowledgeChapterCompletion(2), Is.False);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(write));
            Assert.That(state.CompleteStoryScene(StoryCatalog.LoadChapter1().introSceneId), Is.True);
            Assert.That(SaveService.Load().completedStoryScenes, Does.Contain(StoryCatalog.LoadChapter1().introSceneId));
        }

        [Test]
        public void NarrativeExternallyCommittedActiveSceneReturnsWithoutDuplicateGrantOrEvent()
        {
            var state = GameState.Instance; StoryService.ResetRuntime(); Assert.That(StoryService.TryBeginChapterIntro(), Is.True);
            var scene = StoryCatalog.FindScene(StoryService.ActiveSceneId);
            for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            Assert.That(state.CompleteStoryScene(scene.id), Is.True); string before = JsonUtility.ToJson(state.Save); var write = state.LastWriteResult;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.PreLevel));
                Assert.That(StoryService.ActiveSceneId, Is.Empty); Assert.That(StoryService.CurrentLine, Is.Null);
                Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(write)); Assert.That(events, Is.Zero);
            }
            finally { state.Changed -= changed; StoryService.ResetRuntime(); }
        }

        [TestCase(false)] [TestCase(true)]
        public void TutorialRejectedWritePreservesUnreadLedgerAndEvents(bool nullLedger)
        {
            var state = GameState.Instance;
            if (nullLedger) state.Save.completedTutorialSteps = null;
            string before = JsonUtility.ToJson(state.Save); var original = state.Save;
            var journal = new string[PersistedSaveKeys.Length]; var present = new bool[journal.Length];
            for (int i = 0; i < journal.Length; i++) { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages; LogAssert.ignoreFailingMessages = true;
            try
            {
                Assert.That(state.CompleteTutorial("test.tutorial.rejected"), Is.False);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(events, Is.Zero); Assert.That(state.CanPersist, Is.False);
                var write = state.LastWriteResult;
                Assert.That(state.CompleteTutorial("test.tutorial.rejected"), Is.False);
                Assert.That(state.CompleteTutorial("test.tutorial.other"), Is.False);
                Assert.That(state.LastWriteResult, Is.SameAs(write)); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                state.Changed -= changed; LogAssert.ignoreFailingMessages = ignored;
                for (int i = 0; i < journal.Length; i++) { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); } PlayerPrefs.Save();
            }
        }

        [TestCase(false)] [TestCase(true)]
        public void TutorialRejectedContinueKeepsExactStepQueueOverlayAndPause(bool queued)
        {
            var state = GameState.Instance; GameObject host = null;
            var journal = new string[PersistedSaveKeys.Length]; var present = new bool[journal.Length];
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                host = CreateTutorialRecoveryHost(queued, out var board, out var presenter);
                var step = presenter.CurrentStep; var overlay = presenter.transform.Find("TutorialOverlay_" + step.id).gameObject;
                int pending = presenter.PendingStepCount; string before = JsonUtility.ToJson(state.Save);
                for (int i = 0; i < journal.Length; i++) { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
                var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
                foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
                string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
                LogAssert.ignoreFailingMessages = true;
                var button = presenter.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.Invoke(); button.onClick.Invoke();
                Assert.That(presenter.IsPresenting, Is.True); Assert.That(presenter.CurrentStep, Is.SameAs(step));
                Assert.That(overlay.activeInHierarchy, Is.True); Assert.That(presenter.PendingStepCount, Is.EqualTo(pending));
                Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Paused)); Assert.That(board.CanAcceptPointerInput, Is.False);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.HasCompletedTutorial(step.id), Is.False);
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
            }
            finally
            {
                LogAssert.ignoreFailingMessages = ignored;
                for (int i = 0; i < journal.Length; i++) { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); } PlayerPrefs.Save();
                if (host != null) Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void TutorialStaleContinueCannotCompleteTheNextQueuedStep()
        {
            var host = CreateTutorialRecoveryHost(true, out var board, out var presenter);
            try
            {
                var first = presenter.CurrentStep;
                var button = presenter.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.Invoke(); button.onClick.Invoke();
                Assert.That(presenter.IsPresenting, Is.True); Assert.That(presenter.CurrentStep.id, Is.EqualTo("test.tutorial.recovery.second"));
                Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Paused));
                Assert.That(GameState.Instance.HasCompletedTutorial(first.id), Is.True);
                Assert.That(GameState.Instance.HasCompletedTutorial("test.tutorial.recovery.second"), Is.False);
            }
            finally { Object.DestroyImmediate(host); }
        }

        static GameObject CreateTutorialRecoveryHost(bool queued, out Match3Board board, out LevelTutorialPresenter presenter)
        {
            var level = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(ChapterLevelCatalog.Find(1, 1)));
            var first = new LevelTutorialStepData { id = "test.tutorial.recovery.first", mechanic = LevelMechanicKeys.Match, trigger = LevelTutorialTriggerTypes.LevelStart, title = "Test", body = "Test tutorial body." };
            var second = new LevelTutorialStepData { id = "test.tutorial.recovery.second", mechanic = LevelMechanicKeys.Match, trigger = LevelTutorialTriggerTypes.LevelStart, title = "Test second", body = "Second tutorial body." };
            level.tutorialSteps = queued ? new[] { first, second } : new[] { first };
            var host = new GameObject("TutorialRecoveryTestCanvas", typeof(RectTransform), typeof(Canvas));
            var boardObject = new GameObject("TutorialRecoveryBoard", typeof(RectTransform), typeof(Match3Board)); boardObject.transform.SetParent(host.transform, false);
            board = boardObject.GetComponent<Match3Board>(); board.Initialize(level, null, null);
            var presenterObject = new GameObject("TutorialRecoveryPresenter", typeof(RectTransform), typeof(LevelTutorialPresenter)); presenterObject.transform.SetParent(host.transform, false);
            presenter = presenterObject.GetComponent<LevelTutorialPresenter>(); presenter.Initialize(board, level);
            Assert.That(presenter.IsPresenting, Is.True); Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Paused));
            return host;
        }

        [TestCase(false)] [TestCase(true)]
        public void TutorialAcknowledgementCommitsOnlyItsLedgerAndPublishesOnce(bool nullLedger)
        {
            var state = GameState.Instance; if (nullLedger) state.Save.completedTutorialSteps = null;
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            if (expected.completedTutorialSteps == null) expected.completedTutorialSteps = new System.Collections.Generic.List<string>();
            expected.completedTutorialSteps.Add("test.tutorial.atomic");
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.CompleteTutorial("test.tutorial.atomic"), Is.True);
                Assert.That(events, Is.EqualTo(1)); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
                var committed = state.LastWriteResult;
                Assert.That(state.CompleteTutorial("test.tutorial.atomic"), Is.False);
                Assert.That(state.LastWriteResult, Is.SameAs(committed)); Assert.That(events, Is.EqualTo(1));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void TutorialInvalidIdsDoNotInitializeTheLedgerOrWrite()
        {
            var state = GameState.Instance; state.Save.completedTutorialSteps = null;
            string before = JsonUtility.ToJson(state.Save); var write = state.LastWriteResult;
            Assert.That(state.CompleteTutorial(null) || state.CompleteTutorial("") || state.CompleteTutorial(" "), Is.False);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(write));
        }

        [Test]
        public void TutorialExternallyAcknowledgedStepClosesWithoutAnotherWriteOrCompletionObservation()
        {
            var host = CreateTutorialRecoveryHost(false, out var board, out var presenter);
            try
            {
                Assert.That(GameState.Instance.CompleteTutorial(presenter.CurrentStep.id), Is.True);
                string before = JsonUtility.ToJson(GameState.Instance.Save); var write = GameState.Instance.LastWriteResult;
                int observations = BalanceTelemetryReviewService.Instance.ObservationCount;
                presenter.Advance(); presenter.Advance();
                Assert.That(presenter.IsPresenting, Is.False); Assert.That(board.CanAcceptPointerInput, Is.True);
                Assert.That(JsonUtility.ToJson(GameState.Instance.Save), Is.EqualTo(before)); Assert.That(GameState.Instance.LastWriteResult, Is.SameAs(write));
                Assert.That(BalanceTelemetryReviewService.Instance.ObservationCount, Is.EqualTo(observations));
            }
            finally { Object.DestroyImmediate(host); }
        }

        [TestCase(false)] [TestCase(true)]
        public void TutorialFreshStateReoffersRejectedStepThenCommitsAndResumesOnce(bool queued)
        {
            var state = GameState.Instance; GameObject host = null, freshObject = null;
            var journal = new string[PersistedSaveKeys.Length]; var present = new bool[journal.Length];
            for (int i = 0; i < journal.Length; i++) { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            try
            {
                host = CreateTutorialRecoveryHost(queued, out _, out var rejected);
                var future = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save)); future.saveVersion = 999;
                foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key); PlayerPrefs.SetString(PersistedSaveKeys[0], JsonUtility.ToJson(future)); PlayerPrefs.Save();
                rejected.Advance(); Assert.That(rejected.IsPresenting, Is.True); Assert.That(state.CanPersist, Is.False);
                Object.DestroyImmediate(host); host = null;
                // Remove only the injected future journal, retaining the real prior isolated durable save.
                for (int i = 0; i < journal.Length; i++) { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); } PlayerPrefs.Save();
                freshObject = new GameObject("FreshTutorialRecoveryState"); var fresh = freshObject.AddComponent<GameState>();
                Assert.That(fresh.CanPersist, Is.True); Assert.That(fresh.HasCompletedTutorial("test.tutorial.recovery.first"), Is.False);
                int gold = fresh.Save.gold, gems = fresh.Save.gems, energy = fresh.Save.energy;
                host = CreateTutorialRecoveryHost(queued, out var board, out var presenter);
                Assert.That(presenter.CurrentStep.id, Is.EqualTo("test.tutorial.recovery.first"));
                int observations = BalanceTelemetryReviewService.Instance.ObservationCount;
                int expectedSteps = queued ? 2 : 1;
                for (int i = 0; i < expectedSteps; i++) presenter.GetComponentInChildren<UnityEngine.UI.Button>().onClick.Invoke();
                Assert.That(presenter.IsPresenting, Is.False); Assert.That(board.CanAcceptPointerInput, Is.True);
                Assert.That(BalanceTelemetryReviewService.Instance.ObservationCount, Is.EqualTo(observations + expectedSteps + (queued ? 1 : 0)));
                Assert.That(fresh.Save.gold, Is.EqualTo(gold)); Assert.That(fresh.Save.gems, Is.EqualTo(gems)); Assert.That(fresh.Save.energy, Is.EqualTo(energy));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(fresh.Save)));
                var write = fresh.LastWriteResult; presenter.Advance(); Assert.That(fresh.LastWriteResult, Is.SameAs(write));
            }
            finally
            {
                if (host != null) Object.DestroyImmediate(host);
                typeof(GameState).GetProperty("Instance").SetValue(null, state); if (freshObject != null) Object.DestroyImmediate(freshObject);
                for (int i = 0; i < journal.Length; i++) { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); } PlayerPrefs.Save();
            }
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
