from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();marker='        void SnapshotAndClearSaveJournal()';assert marker in s
block='''        [TestCase(false)] [TestCase(true)]
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

'''
s=s.replace(marker,block+marker,1);p.write_text(s)
