from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
def insert(name,text):
 p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/name;s=p.read_text();i=s.rfind('    }');assert i>0;p.write_text(s[:i]+text+'\n'+s[i:])
insert('LevelSystemRuntimePlayModeTests.cs',r'''
        [TestCase(0,false)] [TestCase(1,false)] [TestCase(2,false)] [TestCase(3,false)]
        [TestCase(0,true)] [TestCase(1,true)] [TestCase(2,true)] [TestCase(3,true)]
        public void SettingsHealthyToggleCommitsOnlyItsPreferenceAndPreservesPendingReceipt(int index,bool enabled)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);Assert.That(state.RegisterLevelAttempt(level),Is.EqualTo(1));state.RecordLevelFailure(level,123);
            state.Save.music=state.Save.sfx=state.Save.vibration=state.Save.notifications=enabled;
            Assert.That(state.TryBeginBoosterCommand(BoosterType.Firebird,out string command),Is.True);
            var original=state.Save;var failure=state.LastFailureResult;string before=JsonUtility.ToJson(original);var expected=JsonUtility.FromJson<PlayerSaveData>(before);
            switch(index){case 0:expected.music=!expected.music;break;case 1:expected.sfx=!expected.sfx;break;case 2:expected.vibration=!expected.vibration;break;case 3:expected.notifications=!expected.notifications;break;}
            int events=0;System.Action changed=()=>{events++;Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(JsonUtility.ToJson(expected)));Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(JsonUtility.ToJson(expected)));};state.Changed+=changed;
            try{Assert.That(state.TryTogglePreference(index),Is.True);}
            finally{state.Changed-=changed;}
            Assert.That(events,Is.EqualTo(1));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.Save,Is.Not.SameAs(original));
            Assert.That(state.LastFailureResult,Is.SameAs(failure));Assert.That(state.HasPendingBoosterCommand,Is.True);Assert.That(state.Save.boosterCommand.commandId,Is.EqualTo(command));
        }
        [TestCase(-1)] [TestCase(4)]
        public void SettingsInvalidPreferenceDoesNotWriteOrPublish(int index)
        {
            var state=GameState.Instance;var original=state.Save;string before=JsonUtility.ToJson(original);var write=state.LastWriteResult;
            Assert.Throws<System.ArgumentOutOfRangeException>(()=>state.TryTogglePreference(index));Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));
        }
        [Test]
        public void SettingsRecoveryKeepsOldSessionLockedAndFreshStateCanCommitAfterRepair()
        {
            var state=GameState.Instance;state.Save.energy=4;state.Save.lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks;state.Persist();string before=JsonUtility.ToJson(state.Save);GameObject freshObject=null;
            var present=new bool[PersistedSaveKeys.Length];var journal=new string[present.Length];for(int i=0;i<present.Length;i++){present[i]=PlayerPrefs.HasKey(PersistedSaveKeys[i]);journal[i]=PlayerPrefs.GetString(PersistedSaveKeys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
            try
            {
                Assert.That(state.TryTogglePreference(3),Is.False);Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]),Is.EqualTo(raw));var write=state.LastWriteResult;
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
                Assert.That(state.TryTogglePreference(3),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));
                freshObject=new GameObject("FreshSettingsRecoveryState");var fresh=freshObject.AddComponent<GameState>();Assert.That(fresh.CanPersist,Is.True);Assert.That(fresh.Save.notifications,Is.True);
                int events=0;System.Action changed=()=>events++;fresh.Changed+=changed;
                try{Assert.That(fresh.TryTogglePreference(3),Is.True);Assert.That(events,Is.EqualTo(1));Assert.That(fresh.Save.notifications,Is.False);Assert.That(SaveService.Load().notifications,Is.False);}
                finally{fresh.Changed-=changed;}
                Assert.That(fresh.Save.energy,Is.EqualTo(4));Assert.That(fresh.Save.music&&fresh.Save.sfx&&fresh.Save.vibration,Is.True);
            }
            finally
            {
                typeof(GameState).GetProperty("Instance").SetValue(null,state);if(freshObject!=null)Object.DestroyImmediate(freshObject);
                for(int i=0;i<present.Length;i++){PlayerPrefs.DeleteKey(PersistedSaveKeys[i]);if(present[i])PlayerPrefs.SetString(PersistedSaveKeys[i],journal[i]);}PlayerPrefs.Save();
            }
        }
''')
insert('SettingsAlignmentPlayModeTests.cs',r'''
        [UnityTest] public IEnumerator SettingsRejectedInfoBlocksOtherControlsAndPreservesLanguageOnRefresh()
        {
            var state=GameState.Instance;state.ResetProgress();var view=View();var root=Root();
            var present=new bool[3];var journal=new string[3];for(int i=0;i<3;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
            var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));future.saveVersion=999;for(int i=0;i<3;i++)PlayerPrefs.DeleteKey(keys[i]);PlayerPrefs.SetString(keys[0],JsonUtility.ToJson(future));PlayerPrefs.Save();
            try
            {
                view.SwitchButtons[1].onClick.Invoke();Settle();yield return null;Assert.That(view.InfoPanel.activeSelf,Is.True);string before=JsonUtility.ToJson(state.Save);int generation=router.RouteGeneration;
                view.ProfileButton.onClick.Invoke();view.PlusButtons[0].onClick.Invoke();view.Rows[0].onClick.Invoke();view.SwitchButtons[2].onClick.Invoke();view.LanguageButtons[1].onClick.Invoke();
                Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                DemoFeatureState.SetLanguage("en");view.RefreshContent();Assert.That(view.InfoPanel.activeSelf,Is.True);Assert.That(view.InfoHeading.text,Is.EqualTo("AN ERROR OCCURRED"));
                Assert.That(view.InfoDescription.text,Is.EqualTo("Action could not be completed.\nClose this message and try again."));Assert.That(view.CloseInfoLabel.text,Is.EqualTo("CLOSE"));
                yield return WaitForNormalButtonColors();Capture("Settings_Rejected_360_EN",new Vector2Int(360,640),new Rect(0,20,360,600));
                DemoFeatureState.SetLanguage("ru");view.RefreshContent();yield return WaitForNormalButtonColors();Capture("Settings_Rejected_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                view.CloseInfoButton.onClick.Invoke();Assert.That(view.InfoPanel.activeSelf,Is.False);Assert.That(Root(),Is.SameAs(root));
                view.BackButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));
            }
            finally{for(int i=0;i<3;i++){PlayerPrefs.DeleteKey(keys[i]);if(present[i])PlayerPrefs.SetString(keys[i],journal[i]);}PlayerPrefs.Save();}
        }
''')
