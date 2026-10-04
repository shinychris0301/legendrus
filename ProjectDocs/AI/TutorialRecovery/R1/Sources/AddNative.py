from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/TutorialAlignmentPlayModeTests.cs';s=p.read_text();marker=' [UnityTest] public IEnumerator SectionsAndLocalizedCapturesDoNotMutateProgress()'
block=''' [UnityTest] public IEnumerator TutorialRejectedNativeContinuePreservesPausedGameplayInBothLanguages(){
 var keys=new[]{"LegendsOfRus_Save_v1","LegendsOfRus_Save_v1_Backup","LegendsOfRus_Save_v1_Staging"};
 foreach(string language in new[]{"ru","en"}){
 var state=GameState.Instance;state.ResetProgress();StoryService.ResetRuntime();DemoFeatureState.SetLanguage(language);
 router.Show(ScreenId.Gameplay);Settle();yield return null;
 var game=router.Viewport.GetComponentInChildren<ProductionGameplayView>();var board=game.Board;
 var presenter=router.Viewport.GetComponentInChildren<LevelTutorialPresenter>();Assert.That(presenter.IsPresenting,Is.True);
 var step=presenter.CurrentStep;var overlay=presenter.transform.Find("TutorialOverlay_"+step.id).gameObject;
 var button=presenter.GetComponentInChildren<Button>();int pending=presenter.PendingStepCount,generation=router.RouteGeneration,moves=board.Moves,score=board.Score;
 string before=JsonUtility.ToJson(state.Save),hash=board.BoardStateHash;Assert.That(router.ShowErrorDialog(),Is.False,"Generic Gameplay error guard remains closed.");
 var journal=new string[keys.Length];var existed=new bool[keys.Length];for(int i=0;i<keys.Length;i++){existed[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
 var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;foreach(string key in keys)PlayerPrefs.DeleteKey(key);string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
 try{
 button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;
 Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));var error=router.Viewport.GetComponentInChildren<ProductionErrorDialogView>();Assert.That(error.Retry.interactable,Is.False);Assert.That(error.CloseLabel.text,Is.EqualTo(language=="en"?"CLOSE":"ЗАКРЫТЬ"));
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(game.Board,Is.SameAs(board));
 Assert.That(presenter.CurrentStep,Is.SameAs(step));Assert.That(presenter.PendingStepCount,Is.EqualTo(pending));Assert.That(overlay.activeInHierarchy,Is.True);Assert.That(board.CanAcceptPointerInput,Is.False);Assert.That(board.Phase,Is.EqualTo(LegendsOfRus.GameplayCore.GameplayPhase.Paused));
 Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.CanPersist,Is.False);Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
 error.Retry.onClick.Invoke();Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
 if(language=="ru")error.Close.onClick.Invoke();else router.NavigateBack();Settle();yield return null;
 Assert.That(router.ActiveModal,Is.Null);Assert.That(board.Phase,Is.EqualTo(LegendsOfRus.GameplayCore.GameplayPhase.Paused));Assert.That(presenter.CurrentStep,Is.SameAs(step));
 button.onClick.Invoke();Settle();yield return null;Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));
 Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.Score,Is.EqualTo(score));Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
 router.NavigateBack();Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);
 }
 finally{for(int i=0;i<keys.Length;i++){PlayerPrefs.DeleteKey(keys[i]);if(existed[i])PlayerPrefs.SetString(keys[i],journal[i]);}PlayerPrefs.Save();router.Show(ScreenId.Home);Settle();state.ResetProgress();StoryService.ResetRuntime();}
 yield return null;
 }
 }
 [UnityTest] public IEnumerator TutorialRetiredPageAndStaleErrorOwnerCannotCommitOrOpenModal(){
 var state=GameState.Instance;state.ResetProgress();StoryService.ResetRuntime();router.Show(ScreenId.Gameplay);Settle();yield return null;
 var game=router.Viewport.GetComponentInChildren<ProductionGameplayView>();var board=game.Board;var presenter=router.Viewport.GetComponentInChildren<LevelTutorialPresenter>();Assert.That(presenter.IsPresenting,Is.True);
 var step=presenter.CurrentStep;var oldButton=presenter.GetComponentInChildren<Button>();string before=JsonUtility.ToJson(state.Save);var write=state.LastWriteResult;
 router.Show(ScreenId.Home);oldButton.onClick.Invoke();presenter.Advance();
 typeof(ScreenRouter).GetMethod("ShowGameplayTutorialError",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(router,new object[]{board,presenter});
 Assert.That(state.CanPersist,Is.True);Assert.That(state.HasCompletedTutorial(step.id),Is.False);Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(router.ActiveModal,Is.Null);
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.ActiveModal,Is.Null);
 }
'''
assert marker in s;s=s.replace(marker,block+marker,1);p.write_text(s)
