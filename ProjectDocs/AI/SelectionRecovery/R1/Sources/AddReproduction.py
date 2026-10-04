from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
def insert(name,text):
 p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/name;s=p.read_text();i=s.rfind('    }');assert i>0;p.write_text(s[:i]+text+'\n'+s[i:])
insert('LevelSystemRuntimePlayModeTests.cs',r'''
        [TestCase(false)] [TestCase(true)]
        public void SelectionRejectedWritePreservesSelectionAndLockedState(bool alreadyLocked)
        {
            var state=GameState.Instance;state.Save.highestUnlockedLevel=2;state.Persist();
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            int events=0;System.Action changed=()=>events++;
            try
            {
                if(alreadyLocked)Assert.That(state.SpendGold(1),Is.False);
                var original=state.Save;string before=JsonUtility.ToJson(original);state.Changed+=changed;
                Assert.That(state.SelectLevel(1,2),Is.False);
                Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(events,Is.Zero);Assert.That(state.CanPersist,Is.False);var write=state.LastWriteResult;
                Assert.That(state.SelectLevel(1,1),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1])||PlayerPrefs.HasKey(PersistedSaveKeys[2]),Is.False);
            }
            finally{state.Changed-=changed;LogAssert.ignoreFailingMessages=ignored;}
        }

        [TestCase(false)] [TestCase(true)]
        public void SelectionNormalizationRejectedWritePreservesExactSave(bool alreadyLocked)
        {
            var state=GameState.Instance;
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            int events=0;System.Action changed=()=>events++;
            try
            {
                if(alreadyLocked)Assert.That(state.SpendGold(1),Is.False);
                state.Save.selectedChapter=9;state.Save.selectedLevel=999;state.Save.highestUnlockedLevel=999;state.Save.levelProgress=null;
                var original=state.Save;string before=JsonUtility.ToJson(original);state.Changed+=changed;
                state.EnsureSelectedLevelIsUnlocked();
                Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(events,Is.Zero);Assert.That(state.CanPersist,Is.False);var write=state.LastWriteResult;
                state.EnsureSelectedLevelIsUnlocked();Assert.That(state.LastWriteResult,Is.SameAs(write));
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));
            }
            finally{state.Changed-=changed;LogAssert.ignoreFailingMessages=ignored;}
        }
''')
insert('ChapterMapAlignmentPlayModeTests.cs',r'''
        [UnityTest]
        public IEnumerator SelectionRejectedNativeNodeStaysOnMapWithError()
        {
            yield return VerifyRejectedSelection(false);
        }
        [UnityTest]
        public IEnumerator SelectionRejectedNativePlayStaysOnMapWithError()
        {
            yield return VerifyRejectedSelection(true);
        }
        IEnumerator VerifyRejectedSelection(bool play)
        {
            var state=GameState.Instance;state.Save.highestUnlockedLevel=2;state.EnsureSelectedLevelIsUnlocked();
            state.CompleteStoryScene(StoryCatalog.LoadChapter1().introSceneId);StoryService.ResetRuntime();
            var original=state.Save;string before=JsonUtility.ToJson(original);
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;
            foreach(string key in keys)PlayerPrefs.DeleteKey(key);
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            try
            {
                var root=Root();var view=root.GetComponentInChildren<ProductionChapterMapView>();
                (play?view.PlayButton:view.Nodes[1].Button).onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));Assert.That(Root(),Is.SameAs(root));
                Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
                Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(state.CanPersist,Is.False);var write=state.LastWriteResult;
                var error=router.GameCanvas.GetComponentInChildren<ProductionErrorDialogView>();Assert.That(error.Retry.interactable,Is.False);
                router.NavigateBack();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));Assert.That(router.ActiveModal,Is.Null);
                (play?view.PlayButton:view.Nodes[1].Button).onClick.Invoke();Settle();yield return null;
                Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));Assert.That(state.LastWriteResult,Is.SameAs(write));
                Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                router.NavigateBack();Settle();yield return null;
                view.BackButton.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
            }
            finally{LogAssert.ignoreFailingMessages=ignored;}
        }
''')
