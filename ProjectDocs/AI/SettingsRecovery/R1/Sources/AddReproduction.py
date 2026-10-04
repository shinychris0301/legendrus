from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
def insert(name,text):
 p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/name;s=p.read_text();i=s.rfind('    }');assert i>0;p.write_text(s[:i]+text+'\n'+s[i:])
insert('LevelSystemRuntimePlayModeTests.cs',r'''
        [TestCase(0,false)] [TestCase(1,false)] [TestCase(2,false)] [TestCase(3,false)]
        [TestCase(0,true)] [TestCase(1,true)] [TestCase(2,true)] [TestCase(3,true)]
        public void SettingsRejectedLegacyToggleKeepsLiveJournalAndFeedback(int index,bool locked)
        {
            var state=GameState.Instance;
            var present=new bool[PersistedSaveKeys.Length];var journal=new string[present.Length];
            for(int i=0;i<present.Length;i++){present[i]=PlayerPrefs.HasKey(PersistedSaveKeys[i]);journal[i]=PlayerPrefs.GetString(PersistedSaveKeys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;
            foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            try
            {
                if(locked)Assert.That(state.SpendGold(1),Is.False);var original=state.Save;string before=JsonUtility.ToJson(original);
                SettingsLegacyToggle(state,index);
                Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(events,Is.Zero);Assert.That(state.CanPersist,Is.False);
                var feedback=CoreLoopFeedbackService.Instance;if(feedback!=null){Assert.That(feedback.MusicEnabled,Is.True);Assert.That(feedback.SfxEnabled,Is.True);Assert.That(feedback.VibrationEnabled,Is.True);}
                var write=state.LastWriteResult;SettingsLegacyToggle(state,index);Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(events,Is.Zero);
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));
            }
            finally
            {
                state.Changed-=changed;for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();LogAssert.ignoreFailingMessages=ignored;
            }
        }
        static void SettingsLegacyToggle(GameState state,int index)
        {
            switch(index){case 0:state.ToggleMusic();break;case 1:state.ToggleSfx();break;case 2:state.ToggleVibration();break;case 3:state.ToggleNotifications();break;default:throw new System.ArgumentOutOfRangeException(nameof(index));}
        }
''')
insert('SettingsAlignmentPlayModeTests.cs',r'''
        [UnityTest] public IEnumerator SettingsRejectedNativeSwitchKeepsPageAndExplainsFailureInBothLanguages()
        {yield return VerifySettingsRejectedNativeSwitch(false);}
        [UnityTest] public IEnumerator SettingsRejectedPausedSwitchKeepsBoardAndModalInBothLanguages()
        {yield return VerifySettingsRejectedNativeSwitch(true);}
        IEnumerator VerifySettingsRejectedNativeSwitch(bool paused)
        {
            foreach(string language in new[]{"ru","en"})
            {
                var state=GameState.Instance;state.ResetProgress();DemoFeatureState.SetLanguage(language);Match3Board board=null;
                if(paused)
                {
                    state.Save.highestUnlockedLevel=31;Assert.That(state.SelectLevel(1,31),Is.True);
                    foreach(var step in ChapterLevelCatalog.Find(1,31).tutorialSteps)state.CompleteTutorial(step.id);
                    router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;yield return null;
                    var game=router.Viewport.GetComponentInChildren<ProductionGameplayView>();board=game.Board;
                    game.PauseButton.onClick.Invoke();Settle();yield return null;
                    router.Viewport.GetComponentInChildren<ProductionPauseView>().Actions[2].onClick.Invoke();Settle();yield return null;
                }
                else{router.Show(ScreenId.Settings);Settle();yield return null;}
                var view=View();var root=Root();string before=JsonUtility.ToJson(state.Save);string hash=board!=null?board.BoardStateHash:null;int moves=board!=null?board.Moves:0;
                var present=new bool[3];var journal=new string[3];for(int i=0;i<3;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
                var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;foreach(string key in keys)if(key!=LanguagePreference.Key)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
                bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
                try
                {
                    for(int index=0;index<4;index++)
                    {
                        view.SwitchButtons[index].onClick.Invoke();Settle();yield return null;
                        Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(Root(),Is.SameAs(root));Assert.That(view.InfoPanel.activeSelf,Is.True);
                        Assert.That(view.InfoHeading.text,Is.EqualTo(language=="ru"?"ПРОИЗОШЛА ОШИБКА":"AN ERROR OCCURRED"));
                        Assert.That(view.SwitchButtons[index].image.sprite.name,Is.EqualTo("switch_on"));Assert.That(view.LanguageChoices.activeSelf,Is.False);
                        Assert.That(router.ActiveModal,paused?Is.EqualTo(ScreenId.Settings):Is.Null);var write=state.LastWriteResult;
                        view.SwitchButtons[index].onClick.Invoke();Assert.That(state.LastWriteResult,Is.SameAs(write));
                        if(index%2==0)view.CloseInfoButton.onClick.Invoke();else router.NavigateBack();Settle();yield return null;
                        Assert.That(view.InfoPanel.activeSelf,Is.False);Assert.That(Root(),Is.SameAs(root));
                        if(board!=null){Assert.That(board.Phase,Is.EqualTo(GameplayCore.GameplayPhase.Paused));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.CanAcceptPointerInput,Is.False);}
                    }
                    Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
                    if(paused){view.BackButton.onClick.Invoke();Settle();yield return null;Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(router.Viewport.GetComponentInChildren<ProductionGameplayView>().Board,Is.SameAs(board));}
                }
                finally
                {
                    for(int i=0;i<3;i++){PlayerPrefs.DeleteKey(keys[i]);if(present[i])PlayerPrefs.SetString(keys[i],journal[i]);}PlayerPrefs.Save();LogAssert.ignoreFailingMessages=ignored;
                }
            }
        }
        [UnityTest] public IEnumerator SettingsReentrantChangedSwitchCommitsOnlyOneToggle()
        {
            var state=GameState.Instance;var view=View();int events=0;Action changed=()=>{if(++events==1)view.SwitchButtons[0].onClick.Invoke();};state.Changed+=changed;
            try{view.SwitchButtons[0].onClick.Invoke();}
            finally{state.Changed-=changed;}
            Assert.That(state.Save.music,Is.False);Assert.That(events,Is.EqualTo(1));Assert.That(SaveService.Load().music,Is.False);yield return null;
        }
        [UnityTest] public IEnumerator SettingsReboundSwitchRejectsRetiredListener()
        {
            var view=View();var buttons=(List<Button>)typeof(ProductionSettingsView).GetField("buttons",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(view);
            var listeners=(List<UnityEngine.Events.UnityAction>)typeof(ProductionSettingsView).GetField("listeners",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(view);
            var retired=listeners[buttons.IndexOf(view.SwitchButtons[0])];view.Bind(id=>{},()=>{},category=>{});string before=JsonUtility.ToJson(GameState.Instance.Save);var write=GameState.Instance.LastWriteResult;
            retired.Invoke();Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(before));Assert.That(GameState.Instance.LastWriteResult,Is.SameAs(write));yield return null;
        }
''')
