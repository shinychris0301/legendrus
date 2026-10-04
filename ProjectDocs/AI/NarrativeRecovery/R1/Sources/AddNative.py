from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/DialogueAlignmentPlayModeTests.cs';s=p.read_text();marker=' [UnityTest] public IEnumerator RebindingAndDisabledStateDoNotAdvanceOrDuplicateActions()'
new=''' [UnityTest] public IEnumerator RejectedNarrativeWritesKeepExactNativePageInBothLanguages(){
 var keys=new[]{"LegendsOfRus_Save_v1","LegendsOfRus_Save_v1_Backup","LegendsOfRus_Save_v1_Staging"};
 foreach(var language in new[]{"ru","en"})foreach(int kind in new[]{0,1,2,3}){
 var state=GameState.Instance;state.ResetProgress();StoryService.ResetRuntime();DemoFeatureState.SetLanguage(language);
 var story=StoryCatalog.LoadChapter1();
 if(kind>=2){state.Save.highestUnlockedLevel=500;state.Save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=500,completions=1,bestStars=3,rewardClaimed=true});state.Save.lastCompletedChapter=1;state.Save.lastCompletedLevel=500;Assert.That(SaveService.TrySave(state.Save,out _),Is.True);}
 if(kind==1){var level=ChapterLevelCatalog.Find(1,1);state.CompleteLevel(level,level.starScoreThresholds[0]);}
 if(kind==3){Assert.That(state.CompleteStoryScene(story.completion.finaleSceneId),Is.True);router.Show(ScreenId.ChapterComplete);Settle();yield return null;}
 else{
 Assert.That(kind==0?StoryService.TryBeginChapterIntro():StoryService.TryBeginPostVictoryDialogue(),Is.True);router.Show(ScreenId.Dialogue);Settle();yield return null;
 var scene=StoryCatalog.FindScene(StoryService.ActiveSceneId);for(int i=1;i<scene.lines.Length;i++){ClickContinue();yield return null;}
 }
 var page=kind==3?router.Viewport.GetComponentInChildren<ProductionChapterCompleteView>().gameObject:View().gameObject;
 int generation=router.RouteGeneration;string before=JsonUtility.ToJson(state.Save),sceneId=StoryService.ActiveSceneId;
 string line=kind==3?null:View().Description.text;
 var present=new bool[keys.Length];var journal=new string[keys.Length];for(int i=0;i<keys.Length;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
 var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;foreach(string key in keys)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
 try{
 if(kind==3){var chapter=page.GetComponent<ProductionChapterCompleteView>();chapter.Actions[1].onClick.Invoke();chapter.Actions[1].onClick.Invoke();}
 else{View().Continue.onClick.Invoke();View().Continue.onClick.Invoke();}
 Settle();yield return null;
 Assert.That(router.CurrentScreen,Is.EqualTo(kind==3?ScreenId.ChapterComplete:ScreenId.Dialogue));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(page.activeInHierarchy,Is.True);
 Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));var error=router.Viewport.GetComponentInChildren<ProductionErrorDialogView>();Assert.That(error,Is.Not.Null);Assert.That(error.Retry.interactable,Is.False);
 Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.CanPersist,Is.False);Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
 if(kind==3)Assert.That(state.HasPendingChapterCompletion(1),Is.True);else{Assert.That(View().gameObject,Is.SameAs(page));Assert.That(View().Description.text,Is.EqualTo(line));Assert.That(StoryService.ActiveSceneId,Is.EqualTo(sceneId));Assert.That(state.HasCompletedStoryScene(sceneId),Is.False);}
 error.Retry.onClick.Invoke();Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
 if(language=="ru")error.Close.onClick.Invoke();else router.NavigateBack();Settle();yield return null;
 Assert.That(router.ActiveModal,Is.Null);Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
 if(kind<3){yield return WaitForNormalButtonColors();Capture("Dialogue_SavePending_"+kind+"_"+language,new Vector2Int(941,1672),new Rect(0,0,941,1672));View().Continue.onClick.Invoke();}
 else page.GetComponent<ProductionChapterCompleteView>().Actions[0].onClick.Invoke();
 Settle();yield return null;Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
 router.NavigateBack();Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);
 }
 finally{for(int i=0;i<keys.Length;i++){PlayerPrefs.DeleteKey(keys[i]);if(present[i])PlayerPrefs.SetString(keys[i],journal[i]);}PlayerPrefs.Save();state.ResetProgress();StoryService.ResetRuntime();router.Show(ScreenId.Home);Settle();}
 yield return null;
 }
 }
'''
assert marker in s and 'RejectedNarrativeWritesKeepExactNativePage' not in s;p.write_text(s.replace(marker,new+marker))
