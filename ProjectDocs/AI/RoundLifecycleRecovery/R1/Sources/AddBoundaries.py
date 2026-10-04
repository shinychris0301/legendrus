from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
def insert(name,text):
 p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/name;s=p.read_text();i=s.rfind('    }');assert i>0;p.write_text(s[:i]+text+'\n'+s[i:])
insert('LevelSystemRuntimePlayModeTests.cs',r'''
        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void RoundStartEnergyClockAndRetryCommitTogether(int scenario)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);
            state.Save.energy=scenario==0?0:scenario==1?0:state.Save.energyMax;
            long clock=System.DateTime.UtcNow.AddMinutes(scenario==1?-31:0).Ticks;
            state.Save.lastEnergyUtcTicks=clock;
            if(scenario==2)Assert.That(state.RegisterLevelAttempt(level),Is.EqualTo(1));
            state.Persist();var original=state.Save;string before=JsonUtility.ToJson(original);
            var write=state.LastWriteResult;int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                bool started=state.TryStartLevel(level,out int attempt,out bool insufficient);
                if(scenario==0)
                {
                    Assert.That(started,Is.False);Assert.That(insufficient,Is.True);Assert.That(attempt,Is.Zero);
                    Assert.That(state.Save,Is.SameAs(original));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(events,Is.Zero);
                }
                else
                {
                    Assert.That(started,Is.True);Assert.That(insufficient,Is.False);Assert.That(attempt,Is.EqualTo(scenario==1?1:2));
                    Assert.That(events,Is.EqualTo(1));Assert.That(state.Save,Is.Not.SameAs(original));
                    Assert.That(state.Save.energy,Is.EqualTo(scenario==1?0:state.Save.energyMax-1));
                    Assert.That(state.GetLevelProgress(1,1).retries,Is.EqualTo(scenario==1?0:1));
                    Assert.That(state.Save.lastEnergyUtcTicks,Is.GreaterThan(clock));
                }
                Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));
                Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(state.Save)));
            }
            finally{state.Changed-=changed;}
        }

        [Test]
        public void RoundStartRejectedSaveKeepsDebitClockAndAttemptUntilFreshRecovery()
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);state.Save.energy=0;
            state.Save.lastEnergyUtcTicks=System.DateTime.UtcNow.AddMinutes(-31).Ticks;state.Persist();
            var original=state.Save;string before=JsonUtility.ToJson(original);GameObject freshObject=null;
            var present=new bool[PersistedSaveKeys.Length];var journal=new string[present.Length];
            for(int i=0;i<present.Length;i++){present[i]=PlayerPrefs.HasKey(PersistedSaveKeys[i]);journal[i]=PlayerPrefs.GetString(PersistedSaveKeys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                Assert.That(state.TryStartLevel(level,out int attempt,out bool insufficient),Is.False);Assert.That(attempt,Is.Zero);Assert.That(insufficient,Is.False);
                Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(events,Is.Zero);
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));var write=state.LastWriteResult;
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
                Assert.That(state.TryStartLevel(level,out _,out _),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));
                freshObject=new GameObject("FreshRoundStartRecoveryState");var fresh=freshObject.AddComponent<GameState>();
                Assert.That(fresh.CanPersist,Is.True);Assert.That(fresh.Save.energy,Is.EqualTo(1),"Existing Awake clock catch-up is committed before start.");Assert.That(fresh.GetLevelProgress(1,1),Is.Null);
                Assert.That(fresh.TryStartLevel(level,out attempt,out insufficient),Is.True);Assert.That(attempt,Is.EqualTo(1));Assert.That(insufficient,Is.False);
                Assert.That(fresh.Save.energy,Is.Zero);Assert.That(fresh.GetLevelProgress(1,1).retries,Is.Zero);
                Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(fresh.Save)));
            }
            finally
            {
                state.Changed-=changed;typeof(GameState).GetProperty("Instance").SetValue(null,state);if(freshObject!=null)Object.DestroyImmediate(freshObject);
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
            }
        }

        [Test]
        public void RoundStartRejectsNullLockedLevelAndPendingBoosterWithoutWriting()
        {
            var state=GameState.Instance;var original=state.Save;string before=JsonUtility.ToJson(original);var write=state.LastWriteResult;
            Assert.That(state.TryStartLevel(null,out _,out _),Is.False);
            Assert.That(state.TryStartLevel(ChapterLevelCatalog.Find(1,2),out _,out _),Is.False);
            Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));
            Assert.That(state.TryBeginBoosterCommand(BoosterType.Firebird,out _),Is.True);
            original=state.Save;before=JsonUtility.ToJson(original);write=state.LastWriteResult;
            Assert.That(state.TryStartLevel(ChapterLevelCatalog.Find(1,1),out int attempt,out bool insufficient),Is.False);
            Assert.That(attempt,Is.Zero);Assert.That(insufficient,Is.False);Assert.That(state.Save,Is.SameAs(original));
            Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));
        }

        [Test]
        public void RoundFailureOverflowKeepsPreviousResultAndFailurePublishesBeforeChanged()
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);int[] goals=new int[level.goals.Length];
            for(int i=0;i<goals.Length;i++)goals[i]=int.MaxValue;
            bool observed=false;System.Action changed=()=>{observed=true;Assert.That(state.LastFailureResult.Score,Is.EqualTo(123));Assert.That(state.GetLevelProgress(1,1).failures,Is.EqualTo(1));};
            state.Changed+=changed;
            try{Assert.That(state.TryRecordLevelFailure(level,123,goals),Is.True);}
            finally{state.Changed-=changed;}
            Assert.That(observed,Is.True);var outcome=state.LastFailureResult;
            for(int i=0;i<goals.Length;i++){goals[i]=0;Assert.That(outcome.CurrentAt(i),Is.EqualTo(level.goals[i].target));}
            state.GetLevelProgress(1,1).failures=int.MaxValue;state.Persist();var original=state.Save;string before=JsonUtility.ToJson(original);var write=state.LastWriteResult;
            Assert.That(state.TryRecordLevelFailure(level,400),Is.False);Assert.That(state.Save,Is.SameAs(original));Assert.That(state.LastFailureResult,Is.SameAs(outcome));
            Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(state.CanPersist,Is.True);
            Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(before));
        }
''')
insert('PreLevelAlignmentPlayModeTests.cs',r'''
        [UnityTest]
        public IEnumerator RoundStartNativeReentrantCallbackCommitsOnceInBothLanguages()
        {
            foreach(string language in new[]{"ru","en"})
            {
                var state=GameState.Instance;state.ResetProgress();DemoFeatureState.SetLanguage(language);
                state.Save.highestUnlockedLevel=31;state.SelectLevel(1,31);state.Save.energy=4;
                state.Save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;state.Persist();router.Show(ScreenId.PreLevel);Settle();yield return null;
                var view=Root().GetComponentInChildren<ProductionPreLevelView>();var play=view.PlayButton;
                int calls=0;System.Action changed=()=>{calls++;typeof(ScreenRouter).GetMethod("StartLevel",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(router,null);};
                state.Changed+=changed;
                try{play.onClick.Invoke();}
                finally{state.Changed-=changed;}
                Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(router.ActiveModal,Is.Null);
                Assert.That(calls,Is.EqualTo(2),"Selection normalization and one combined debit/attempt; board initialization must not register again.");
                Assert.That(state.Save.energy,Is.EqualTo(3));Assert.That(state.GetLevelProgress(1,31).attempts,Is.EqualTo(1));Assert.That(state.GetLevelProgress(1,31).retries,Is.Zero);
                var active=router.Viewport.GetComponentInChildren<Match3Board>();Assert.That(active,Is.Not.Null);Assert.That(active.CurrentLevel.levelId,Is.EqualTo(31));
                Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(state.Save)));
                router.Show(ScreenId.PreLevel);Settle();yield return null;
            }
        }

        [UnityTest]
        public IEnumerator RoundStartNativeWriteRejectedAfterNormalizationKeepsPreLevelInBothLanguages()
        {
            foreach(string language in new[]{"ru","en"})
            {
                var state=GameState.Instance;state.ResetProgress();DemoFeatureState.SetLanguage(language);state.Save.energy=4;state.Save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;state.Persist();
                router.Show(ScreenId.PreLevel);Settle();yield return null;string before=JsonUtility.ToJson(state.Save);
                var present=new bool[keys.Length];var journal=new string[keys.Length];for(int i=0;i<keys.Length;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
                string raw=null;int calls=0;System.Action changed=()=>
                {
                    if(++calls!=1)return;var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
                    foreach(string key in keys)PlayerPrefs.DeleteKey(key);raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
                };
                state.Changed+=changed;
                try
                {
                    var view=Root().GetComponentInChildren<ProductionPreLevelView>();view.PlayButton.onClick.Invoke();Settle();yield return null;
                    Assert.That(calls,Is.EqualTo(1));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
                    Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.CanPersist,Is.False);
                    Assert.That(router.Viewport.GetComponentInChildren<Match3Board>(),Is.Null);Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
                    router.NavigateBack();Settle();yield return null;var write=state.LastWriteResult;
                    view.PlayButton.onClick.Invoke();Settle();yield return null;
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
                    Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                }
                finally
                {
                    state.Changed-=changed;for(int i=0;i<keys.Length;i++){PlayerPrefs.DeleteKey(keys[i]);if(present[i])PlayerPrefs.SetString(keys[i],journal[i]);}PlayerPrefs.Save();
                }
            }
        }

        [UnityTest]
        public IEnumerator RoundStartNativeNoEnergyOpensExistingGateWithoutAnAttempt()
        {
            var state=GameState.Instance;state.Save.energy=0;state.Save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;state.Persist();
            string before=JsonUtility.ToJson(state.Save);var view=Root().GetComponentInChildren<ProductionPreLevelView>();view.PlayButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));
            Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(router.Viewport.GetComponentInChildren<Match3Board>(),Is.Null);
            router.Viewport.GetComponentInChildren<ProductionOutOfEnergyView>().Close.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
        }
''')
insert('Match3RuntimePlayModeTests.cs',r'''
        [UnityTest]
        public IEnumerator RoundStartCommittedOrdinaryBoardKeepsRuntimeBoosterOwnershipAndSeed()
        {
            var state=GameState.Instance;var level=board.CurrentLevel;
            Assert.That(state.TryStartLevel(level,out int attempt,out _),Is.True);
            int energy=state.Save.energy;var write=state.LastWriteResult;
            board.Initialize(level,null,null,attempt);
            Assert.That(state.GetLevelProgress(level.chapterId,level.levelId).attempts,Is.EqualTo(attempt));Assert.That(state.Save.energy,Is.EqualTo(energy));
            Assert.That(state.LastWriteResult,Is.SameAs(write));
            Assert.That(board.BoardSeed,Is.EqualTo(BoardSeedUtility.CreateRuntimeSeed(level.chapterId,level.levelId,level.boardSeed+attempt)));
            int moves=board.Moves;int boosters=state.BoosterCount(BoosterType.SeaPearl);board.UseBooster(BoosterType.SeaPearl);yield return WaitUntilInteractive();
            Assert.That(board.Moves,Is.EqualTo(moves+5));Assert.That(state.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(boosters-1));
            Assert.That(state.Save.boosterCommand.state,Is.EqualTo(CampaignBoosterState.Applied));
            Assert.That(state.GetLevelProgress(level.chapterId,level.levelId).attempts,Is.EqualTo(attempt));Assert.That(state.Save.energy,Is.EqualTo(energy));
            Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(state.Save)));
        }

        [Test]
        public void RoundStartOrdinaryInitializerRejectsUncommittedAttempt()
        {
            var level=board.CurrentLevel;var state=GameState.Instance;var write=state.LastWriteResult;string hash=board.BoardStateHash;
            Assert.Throws<System.ArgumentException>(()=>board.Initialize(level,null,null,0));
            Assert.Throws<System.ArgumentException>(()=>board.Initialize(level,null,null,state.GetLevelProgress(level.chapterId,level.levelId).attempts+1));
            Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(state.LastWriteResult,Is.SameAs(write));
        }
''')
