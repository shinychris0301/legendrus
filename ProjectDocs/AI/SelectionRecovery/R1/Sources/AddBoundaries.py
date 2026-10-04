from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();i=s.rfind('    }');s=s[:i]+r'''
        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void SelectionInvalidTargetPreservesSaveAndWriteResult(int boundary)
        {
            var state=GameState.Instance;var original=state.Save;string before=JsonUtility.ToJson(original);var write=state.LastWriteResult;
            int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                int chapter=boundary==2?2:1;int level=boundary==0?0:boundary==1?-1:boundary==2?1:2;
                Assert.That(state.SelectLevel(chapter,level),Is.False);Assert.That(state.Save,Is.SameAs(original));
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(events,Is.Zero);
            }
            finally{state.Changed-=changed;}
        }

        [Test]
        public void SelectionValidTargetCommitsOnceAndReloadsWithoutAttemptOrDebit()
        {
            var state=GameState.Instance;state.Save.highestUnlockedLevel=2;state.Persist();
            var candidate=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));candidate.selectedLevel=2;
            int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                Assert.That(state.SelectLevel(1,2),Is.True);Assert.That(events,Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(JsonUtility.ToJson(candidate)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(candidate)));
                Assert.That(state.GetLevelProgress(1,2),Is.Null);
            }
            finally{state.Changed-=changed;}
        }

        [Test]
        public void SelectionNormalizationCommitsExistingRepairRulesOnCandidate()
        {
            var state=GameState.Instance;state.Save.selectedChapter=9;state.Save.selectedLevel=999;state.Save.highestUnlockedLevel=999;
            state.Save.levelProgress=new System.Collections.Generic.List<LevelProgressData>{null,new LevelProgressData{chapterId=1,levelId=501},
                new LevelProgressData{chapterId=1,levelId=1,bestStars=1},new LevelProgressData{chapterId=1,levelId=1,bestStars=7,bestScore=-3,attempts=-1,retries=4,failures=4,completions=1}};
            var original=state.Save;string before=JsonUtility.ToJson(original);int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                Assert.That(state.TryEnsureSelectedLevelIsUnlocked(),Is.True);Assert.That(state.Save,Is.Not.SameAs(original));
                Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(events,Is.EqualTo(1));
                Assert.That(state.Save.selectedChapter,Is.EqualTo(1));Assert.That(state.Save.selectedLevel,Is.EqualTo(500));Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(500));
                Assert.That(state.Save.levelProgress,Has.Count.EqualTo(1));var progress=state.Save.levelProgress[0];
                Assert.That(progress.bestStars,Is.EqualTo(3));Assert.That(progress.bestScore,Is.Zero);Assert.That(progress.attempts,Is.Zero);Assert.That(progress.retries,Is.Zero);Assert.That(progress.failures,Is.Zero);Assert.That(state.Save.stars,Is.EqualTo(3));
                Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(state.Save)));
            }
            finally{state.Changed-=changed;}
        }

        [TestCase(false)] [TestCase(true)]
        public void SelectionRecoveryRequiresFreshStateAndCommitsAfterJournalRepair(bool normalization)
        {
            var state=GameState.Instance;GameObject freshObject=null;state.Save.highestUnlockedLevel=2;state.Persist();
            var present=new bool[PersistedSaveKeys.Length];var journal=new string[present.Length];
            for(int i=0;i<present.Length;i++){present[i]=PlayerPrefs.HasKey(PersistedSaveKeys[i]);journal[i]=PlayerPrefs.GetString(PersistedSaveKeys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);PlayerPrefs.SetString(PersistedSaveKeys[0],JsonUtility.ToJson(future));PlayerPrefs.Save();
            try
            {
                Assert.That(normalization?state.TryEnsureSelectedLevelIsUnlocked():state.SelectLevel(1,2),Is.False);var write=state.LastWriteResult;
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
                Assert.That(state.TryEnsureSelectedLevelIsUnlocked(),Is.False);Assert.That(state.SelectLevel(1,2),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));
                freshObject=new GameObject("FreshSelectionRecoveryState");var fresh=freshObject.AddComponent<GameState>();
                Assert.That(fresh.CanPersist,Is.True);Assert.That(fresh.Save.selectedLevel,Is.EqualTo(1));
                int events=0;System.Action changed=()=>events++;fresh.Changed+=changed;
                try{Assert.That(fresh.SelectLevel(1,2),Is.True);Assert.That(events,Is.EqualTo(1));Assert.That(SaveService.Load().selectedLevel,Is.EqualTo(2));}
                finally{fresh.Changed-=changed;}
            }
            finally
            {
                typeof(GameState).GetProperty("Instance").SetValue(null,state);if(freshObject!=null)Object.DestroyImmediate(freshObject);
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
            }
        }
'''+s[i:];p.write_text(s)
