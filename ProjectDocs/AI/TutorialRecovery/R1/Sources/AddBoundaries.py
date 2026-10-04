from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs';s=p.read_text().replace('if (CurrentStep == null || board == null || !IsPresenting) return;', 'if (!isActiveAndEnabled || CurrentStep == null || board == null || !IsPresenting) return;',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();marker='        void SnapshotAndClearSaveJournal()'
block='''        [TestCase(false)] [TestCase(true)]
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

'''
s=s.replace(marker,block+marker,1);p.write_text(s)
