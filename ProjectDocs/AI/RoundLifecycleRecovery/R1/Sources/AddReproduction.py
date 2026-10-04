from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
def insert(name,text):
 p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/name;s=p.read_text();i=s.rfind('    }');assert i>0;p.write_text(s[:i]+text+'\n'+s[i:])
insert('LevelSystemRuntimePlayModeTests.cs',r'''
        [TestCase(false)] [TestCase(true)]
        public void RoundRegistrationRejectedWriteKeepsCountersAndSelection(bool locked)
        {
            VerifyRejectedRoundWrite(locked,false);
        }
        [TestCase(false)] [TestCase(true)]
        public void RoundFailureRejectedWriteKeepsProgressAndPresentation(bool locked)
        {
            VerifyRejectedRoundWrite(locked,true);
        }
        void VerifyRejectedRoundWrite(bool locked,bool failure)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);
            if(failure){state.RegisterLevelAttempt(level);state.RecordLevelFailure(level,10);}
            var present=new bool[PersistedSaveKeys.Length];var journal=new string[present.Length];
            for(int i=0;i<present.Length;i++){present[i]=PlayerPrefs.HasKey(PersistedSaveKeys[i]);journal[i]=PlayerPrefs.GetString(PersistedSaveKeys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                if(locked)Assert.That(state.SpendGold(1),Is.False);
                var original=state.Save;var outcome=state.LastFailureResult;string before=JsonUtility.ToJson(original);
                if(failure)state.RecordLevelFailure(level,200);else Assert.That(state.RegisterLevelAttempt(level),Is.Zero);
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.Save,Is.SameAs(original));Assert.That(state.LastFailureResult,Is.SameAs(outcome));Assert.That(events,Is.Zero);
                Assert.That(state.CanPersist,Is.False);var write=state.LastWriteResult;
                if(failure)state.RecordLevelFailure(level,400);else Assert.That(state.RegisterLevelAttempt(level),Is.Zero);
                Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));
            }
            finally
            {
                state.Changed-=changed;
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
                LogAssert.ignoreFailingMessages=ignored;
            }
        }
        [TestCase(false)] [TestCase(true)]
        public void RoundRegistrationCounterOverflowDoesNotPublish(bool retries)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);var progress=state.GetLevelProgress(1,1,true);
            progress.attempts=retries?1:int.MaxValue;progress.retries=retries?int.MaxValue:0;
            string before=JsonUtility.ToJson(state.Save);var original=state.Save;var write=state.LastWriteResult;
            Assert.That(state.RegisterLevelAttempt(level),Is.Zero);Assert.That(state.Save,Is.SameAs(original));
            Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(state.CanPersist,Is.True);
        }
''')
insert('PreLevelAlignmentPlayModeTests.cs',r'''
        [UnityTest]
        public IEnumerator RoundStartOverflowStaysOnPreLevelWithoutDebitingEnergy()
        {
            var state=GameState.Instance;state.Save.energy=4;state.Save.lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks;
            state.GetLevelProgress(1,1,true).attempts=int.MaxValue;state.Persist();
            string before=JsonUtility.ToJson(state.Save);var view=Root().GetComponentInChildren<ProductionPreLevelView>();
            view.PlayButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
            Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(router.Viewport.GetComponentInChildren<Match3Board>(),Is.Null);
            router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
        }
''')
insert('RetainedRoundPlayModeTests.cs',r'''
        [Test]
        public void RoundFailureFinalizeOverflowRetainsCheckpointAndJournal()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);
            State.GetLevelProgress(1,data.level).failures=int.MaxValue;State.Persist();
            string before=JsonUtility.ToJson(State.Save);var original=State.Save;var write=State.LastWriteResult;
            Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.False);
            Assert.That(State.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(State.Save),Is.EqualTo(before));
            Assert.That(State.LastWriteResult,Is.SameAs(write));Assert.That(State.TryGetRetainedRound(out var after),Is.True);
            Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(data)));Assert.That(State.CanPersist,Is.True);
        }
''')
