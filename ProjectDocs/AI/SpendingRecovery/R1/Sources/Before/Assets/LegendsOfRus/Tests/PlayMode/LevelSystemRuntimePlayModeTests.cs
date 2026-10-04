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
