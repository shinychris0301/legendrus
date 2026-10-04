using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace LegendsOfRus.Tests.PlayMode
{
 public sealed class TutorialAlignmentPlayModeTests
 {
 ScreenRouter router;string previousLanguage;
 [UnitySetUp] public IEnumerator SetUp(){string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");router.Show(ScreenId.SplashTitle);Settle();router.Show(ScreenId.TutorialOverlay);Settle();yield return null;}
 [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.SplashTitle);Settle();}DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
 ProductionTutorialView View()=>router.Viewport.GetComponentInChildren<ProductionTutorialView>();GameObject Root()=>View().gameObject;
 void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
 static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
 [UnityTest] public IEnumerator TutorialRejectedNativeContinuePreservesPausedGameplayInBothLanguages(){
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
 oldButton.onClick.Invoke();presenter.Advance();Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(router.ActiveModal,Is.Null);
 }
 [UnityTest] public IEnumerator SectionsAndLocalizedCapturesDoNotMutateProgress(){string save=StableSave();var v=View();v.ShowDetails();Assert.That(v.ActionsBound,Is.True);Assert.That(v.Previous.interactable,Is.False);v.Previous.onClick.Invoke();Assert.That(v.Section,Is.Zero);
 for(int i=0;i<4;i++){v.Tabs[i].onClick.Invoke();Assert.That(v.Section,Is.EqualTo(i));Assert.That(v.MatchDiagram.activeSelf,Is.EqualTo(i==0));Assert.That(v.Previous.gameObject.activeSelf,Is.EqualTo(i>0));yield return WaitForNormalButtonColors();Capture("Tutorial_941x1672_RU_"+i,new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Tutorial_576x1280_safe_EN_"+i,new Vector2Int(576,1280),new Rect(0,48,576,1172));DemoFeatureState.SetLanguage("ru");v.RefreshContent();}
 v.SelectSection(2);yield return WaitForNormalButtonColors();Capture("Tutorial_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Assert.That(StableSave(),Is.EqualTo(save));}
 [UnityTest] public IEnumerator NextPreviousAndDoneReturnToOrigin(){router.Show(ScreenId.Home);Settle();router.Show(ScreenId.TutorialOverlay);Settle();yield return null;var v=View();v.ShowDetails();v.Next.onClick.Invoke();Assert.That(v.Section,Is.EqualTo(1));v.Previous.onClick.Invoke();Assert.That(v.Section,Is.Zero);for(int i=0;i<3;i++)v.Next.onClick.Invoke();Assert.That(v.Section,Is.EqualTo(3));int generation=router.RouteGeneration;v.Next.onClick.Invoke();Assert.That(v.DetailsVisible,Is.False);v.Overview.Done.onClick.Invoke();v.Overview.Done.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}
 [UnityTest] public IEnumerator SplashHelpCloseAndSystemBack(){router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));router.Viewport.GetComponentInChildren<SplashTitleView>().HelpButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TutorialOverlay));var close=View().Overview.Back;ExecuteEvents.Execute(close.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));router.Show(ScreenId.TutorialOverlay);Settle();router.Show(ScreenId.TutorialOverlay);Settle();router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));}
 [UnityTest] public IEnumerator RebindingDoesNotDuplicateActions(){var v=View();int n=0;v.Bind(()=>n++);v.Bind(()=>n++);v.Overview.Done.onClick.Invoke();v.Overview.Done.onClick.Invoke();Assert.That(n,Is.EqualTo(1));yield return null;}
 [UnityTest] public IEnumerator SharedArtworkAndPressFeedbackPreserveReadOnlyGuide()
 {
     yield return WaitForNormalButtonColors();var v=View();v.ShowDetails();string save=StableSave();int generation=router.RouteGeneration;
     foreach(var name in new[]{"BeforeGrid","AfterGrid"}){
         var grid=v.MatchDiagram.transform.Find(name);var rails=grid.Find("SourceGrid").GetComponent<ProductionGameplayCell>();
         Assert.That(rails.GridOnly,Is.True);Assert.That(rails.GridJointCount,Is.EqualTo(16));Assert.That(rails.sprite,Is.SameAs(ProductionGameplaySourceControls.Load("grid")));Assert.That(rails.raycastTarget,Is.False);
         Assert.That(grid.GetComponentsInChildren<Button>(true),Is.Empty);
     }
     v.SelectSection(2);for(int i=0;i<4;i++)Assert.That(v.RowIcons[i].sprite,Is.SameAs(ProductionBoosterArt.Icon(i,true)));
     v.SelectSection(0);
     foreach(var button in new[]{v.Tabs[2],v.Next,v.Close}){
         var feedback=button.GetComponent<ProductionMapButtonFeedback>();Assert.That(feedback,Is.Not.Null);
         var data=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,pointerId=23};
         Color original=button.targetGraphic.canvasRenderer.GetColor();ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);
         Assert.That(feedback.IsPressed,Is.True);Assert.That(button.targetGraphic.canvasRenderer.GetColor().r,Is.LessThan(original.r));
         Assert.That(v.Section,Is.Zero);Assert.That(router.RouteGeneration,Is.EqualTo(generation));
         if(button==v.Next)Capture("Tutorial_NextPressed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
         ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerExitHandler);yield return WaitForNormalButtonColors();Assert.That(feedback.IsPressed,Is.False);
     }
     Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator SharedHeaderRoutesToMatchingResourceCategory(){
 for(int i=0;i<3;i++){
 router.Show(ScreenId.TutorialOverlay);Settle();yield return null;View().Overview.Header.Plus[i].onClick.Invoke();Settle();yield return null;
 Assert.That(router.CurrentScreen,Is.EqualTo(i==2?ScreenId.OutOfEnergy:ScreenId.Shop));
 if(i<2)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==0?"coins":"gems"));
 }
 }
 [UnityTest] public IEnumerator HelpReturnsToSamePausedRound(){
 foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
 router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;yield return null;
 var game=router.Viewport.GetComponentInChildren<ProductionGameplayView>();var board=game.Board;string hash=board.BoardStateHash;int moves=board.Moves,score=board.Score;
 game.PauseButton.onClick.Invoke();Settle();yield return null;router.Show(ScreenId.TutorialOverlay);Settle();yield return null;
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.TutorialOverlay));var v=View();Assert.That(v.ReturnToRound,Is.True);Assert.That(v.Overview.Header.Profile.interactable,Is.False);
 v.Overview.First.onClick.Invoke();Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);Assert.That(game.Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.Score,Is.EqualTo(score));Assert.That(board.CanAcceptPointerInput,Is.True);
 }
 [UnityTest] public IEnumerator OriginalOverviewUsesGameplayGemsAndFirstLevelRoute(){
 var v=View();string save=StableSave();Assert.That(v.DetailsVisible,Is.False);
 int[] ids={0,2,3,4,1,0,0,1,1,1,3,4,2,2,0,0,4,1,3,4,2,1,0,0};
 for(int i=0;i<24;i++)Assert.That(v.Overview.DiagramGems[i].sprite,Is.SameAs(ProductionGameplayArt.Gem(ids[i])));
 for(int i=0;i<2;i++)Assert.That(v.Overview.GoalGems[i].sprite,Is.SameAs(ProductionGameplayArt.Gem(i)));
 yield return WaitForNormalButtonColors();Capture("Tutorial_Overview_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 Capture("Tutorial_Overview_Short_RU",new Vector2Int(941,1450),new Rect(0,0,941,1450));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Tutorial_Overview_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Tutorial_Overview_Tall_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 v.Overview.Lessons[1].onClick.Invoke();Assert.That(v.DetailsVisible,Is.True);Assert.That(v.Section,Is.EqualTo(1));router.NavigateBack();Assert.That(v.DetailsVisible,Is.False);Assert.That(StableSave(),Is.EqualTo(save));
 int energy=GameState.Instance.Save.energy;v.Overview.First.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(GameState.Instance.Save.energy,Is.EqualTo(energy));
 }
 [UnityTest] public IEnumerator NativeLessonsScrollAndFooterReturn(){
 string save=StableSave();
 foreach(string language in new[]{"ru","en"}){
 router.Show(ScreenId.Home);Settle();DemoFeatureState.SetLanguage(language);router.Show(ScreenId.TutorialOverlay);Settle();yield return null;
 var v=View();yield return WaitForNormalButtonColors();
 Capture("Tutorial_ScrolledRewards_"+language,new Vector2Int(941,1450),new Rect(0,0,941,1450),null,camera=>{
     var point=Bounds(v.Overview.Viewport,camera).center;
     var pointer=new PointerEventData(EventSystem.current){position=point,scrollDelta=new Vector2(0,-30)};
     var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);Assert.That(hits.Count,Is.GreaterThan(0));
     ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,pointer,ExecuteEvents.scrollHandler);Canvas.ForceUpdateCanvases();
     Assert.That(v.Overview.Scroller.verticalNormalizedPosition,Is.LessThan(.01f));
     Rect panel=Bounds(v.Overview.Panel.rectTransform,camera),viewport=Bounds(v.Overview.Viewport,camera);
     Assert.That(panel.yMin,Is.GreaterThanOrEqualTo(viewport.yMin-1),"Complete lower panel can be reached by scrolling");
 });
 v.Overview.Scroller.verticalNormalizedPosition=1;yield return null;
 Capture("Tutorial_LessonTap_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Overview.Lessons[1],camera));
 Assert.That(v.DetailsVisible,Is.True);Assert.That(v.Section,Is.EqualTo(1));yield return null;
 Capture("Tutorial_DetailNext_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(v.Next,camera));
 Assert.That(v.Section,Is.EqualTo(2));yield return null;
 Capture("Tutorial_DetailPrevious_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Previous,camera));
 Assert.That(v.Section,Is.EqualTo(1));yield return null;
 Capture("Tutorial_DetailClose_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Close,camera));
 Assert.That(v.DetailsVisible,Is.False);yield return null;int generation=router.RouteGeneration;
 Capture("Tutorial_OverviewReturn_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(v.Overview.Done,camera));
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 [UnityTest] public IEnumerator NativeExpandedHeaderTargetsRouteWithoutChangingProgress(){
 string save=StableSave();
 for(int i=0;i<3;i++){
 router.Show(ScreenId.TutorialOverlay);Settle();yield return null;yield return WaitForNormalButtonColors();var v=View();
 Capture("Tutorial_HeaderTarget_"+i,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{
 var button=v.Overview.Header.Plus[i];Rect art=Bounds(button.image.rectTransform,camera);Click(button,camera,new Vector2(art.xMin-2,art.center.y));});
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(i==2?ScreenId.OutOfEnergy:ScreenId.Shop));
 if(i<2)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==0?"coins":"gems"));Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 static void Click(Button button,Camera camera){Click(button,camera,Bounds(button.image.rectTransform,camera).center);}
 static void Click(Button button,Camera camera,Vector2 point){
 var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);
 }
 static Rect Target(Button button,Camera camera){Rect r=Bounds(button.image.rectTransform,camera);float scale=r.width/button.image.rectTransform.rect.width;Vector4 p=button.image.raycastPadding;return Rect.MinMaxRect(r.xMin+p.x*scale,r.yMin+p.y*scale,r.xMax-p.z*scale,r.yMax-p.w*scale);}
        IEnumerator WaitForNormalButtonColors()
        {
            float deadline=Time.realtimeSinceStartup+2f;
            int stable=0;
            do
            {
                yield return null;
                bool ready=true;
                foreach(Button button in Root().GetComponentsInChildren<Button>())
                {
                    Color expected=button.transition==Selectable.Transition.None?Color.white:(button.IsInteractable()?button.colors.normalColor:button.colors.disabledColor)*button.colors.colorMultiplier;
                    Color actual=button.targetGraphic.canvasRenderer.GetColor();
                    if(Mathf.Max(Mathf.Abs(actual.r-expected.r),Mathf.Abs(actual.g-expected.g),
                        Mathf.Abs(actual.b-expected.b),Mathf.Abs(actual.a-expected.a))>.0001f)ready=false;
                }
                stable=ready?stable+1:0;
            } while(stable<2&&Time.realtimeSinceStartup<deadline);
            Assert.That(stable,Is.EqualTo(2),"Capture only after native disabled-to-normal button fades finish.");
        }

        [Serializable] sealed class CaptureInfo
        {
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null, Action<Camera> prepare = null)
        {
            Assert.That(View().transform.parent.GetComponentsInChildren<Button>().Length,Is.EqualTo(View().GetComponentsInChildren<Button>().Length),"No legacy shortcut controls outside the native page");
            string directory = Environment.GetEnvironmentVariable("LOR_TUTORIAL_CAPTURE_DIR");
            Assert.That(directory, Is.Not.Null.And.Not.Empty);
            Directory.CreateDirectory(directory);
            Canvas canvas = router.GameCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            AdaptiveCanvasScaler adaptive = canvas.GetComponent<AdaptiveCanvasScaler>();
            SafeAreaFitter safe = router.SafeArea.GetComponent<SafeAreaFitter>();
            RenderMode mode = canvas.renderMode; Camera oldCamera = canvas.worldCamera;
            float distance = canvas.planeDistance, scale = canvas.scaleFactor;
            bool se = scaler.enabled, ae = adaptive.enabled, fe = safe.enabled;
            RenderTexture active = RenderTexture.active;
            var target = new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);
            var cameraGo = new GameObject("TutorialCaptureCamera",typeof(Camera));
            var texture = new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try
            {
                target.Create(); scaler.enabled = false; adaptive.enabled = false; safe.enabled = false;
                Camera camera = cameraGo.GetComponent<Camera>(); camera.enabled = false;
                camera.transform.position = new Vector3(0,0,-100); camera.orthographic = true;
                camera.nearClipPlane = .1f; camera.farClipPlane = 1000;
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color32(4,13,24,255);
                camera.targetTexture = target; canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera; canvas.planeDistance = 100;
                adaptive.Apply(safeArea,size);
                canvas.scaleFactor = AdaptiveCanvasScaler.ResolveScale(new Vector2Int(Mathf.RoundToInt(safeArea.width),Mathf.RoundToInt(safeArea.height)),941,1672);
                safe.Apply(safeArea,size);
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Rebuild glyph snapping after the capture camera's responsive layout has settled.
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                prepare?.Invoke(camera);
                Canvas.ForceUpdateCanvases();
                camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0); texture.Apply();
                string output = Path.Combine(directory,name);
                Assert.That(File.Exists(output + ".png"), Is.False, "Keep earlier evidence.");
                File.WriteAllBytes(output + ".png",texture.EncodeToPNG());
                File.WriteAllText(output + ".json",JsonUtility.ToJson(new CaptureInfo
                {
                    unityVersion = Application.unityVersion, graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                    language = DemoFeatureState.LanguageCode, width = size.x, height = size.y, safeArea = safeArea,
                    buttonCount = Root().GetComponentsInChildren<Button>(true).Length
                },true) + "\n");
                VerifyLayoutAndInput(View(),camera,size,safeArea);
                inspect?.Invoke(camera);
            }
            finally
            {
                RenderTexture.active = active; canvas.renderMode = mode; canvas.worldCamera = oldCamera;
                canvas.planeDistance = distance; canvas.scaleFactor = scale;
                scaler.enabled = se; adaptive.enabled = ae; safe.enabled = fe;
                adaptive.ApplyNow(); safe.ApplyNow();
                UnityEngine.Object.Destroy(texture); UnityEngine.Object.Destroy(cameraGo);
                target.Release(); UnityEngine.Object.Destroy(target);
            }
        }

        static Rect Bounds(RectTransform rect,Camera camera)
        {
            var corners=new Vector3[4];rect.GetWorldCorners(corners);Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        static void VerifyLayoutAndInput(ProductionTutorialView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>()){
                Rect rect=Bounds((RectTransform)button.transform,camera);
                if(!v.DetailsVisible&&button.transform.IsChildOf(v.Overview.ScrollContent)){
                    Rect clip=Bounds(v.Overview.Viewport,camera);if(!clip.Contains(rect.min)||!clip.Contains(rect.max))continue;
                }
                Rect target=Target(button,camera);Assert.That(target.width,Is.GreaterThanOrEqualTo(44),button.name+" target width");Assert.That(target.height,Is.GreaterThanOrEqualTo(44),button.name+" target height");
                Assert.That(target.xMin>=safeArea.xMin-1&&target.yMin>=safeArea.yMin-1&&target.xMax<=safeArea.xMax+1&&target.yMax<=safeArea.yMax+1,Is.True,button.name+" safe target");
                Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,button.name+" "+rect+" safe "+safeArea);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            if(!v.DetailsVisible){
                var header=new List<Button>(v.Overview.Header.Plus){v.Overview.Header.Profile,v.Overview.Header.Settings};
                for(int i=0;i<header.Count;i++)for(int j=i+1;j<header.Count;j++)Assert.That(Target(header[i],camera).Overlaps(Target(header[j],camera)),Is.False,"Separate header actions");
                foreach(var art in new[]{v.Overview.Panel,v.Overview.DoneArt,v.Overview.FirstArt,v.Overview.Back.image}){Assert.That(art.type,Is.EqualTo(Image.Type.Simple));Assert.That(art.preserveAspect,Is.True);}
                Rect footer=Bounds(v.Overview.Footer,camera);Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin).Within(1),"Footer follows safe bottom");
                Rect headerBounds=Bounds((RectTransform)v.Overview.Header.transform,camera);Assert.That(headerBounds.yMax,Is.EqualTo(safeArea.yMax).Within(1),"Header follows safe top");
            }
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text))Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name);
        }
    }
}
