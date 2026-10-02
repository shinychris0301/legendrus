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
 public sealed class DialogueAlignmentPlayModeTests
 {
 ScreenRouter router;string previousLanguage;
 [UnitySetUp] public IEnumerator SetUp(){string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");GameState.Instance.ResetProgress();StoryService.ResetRuntime();Assert.That(StoryService.TryBeginChapterIntro(),Is.True);router.Show(ScreenId.Dialogue);Settle();yield return null;}
 [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.Home);Settle();}DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
 ProductionDialogueView View()=>router.Viewport.GetComponentInChildren<ProductionDialogueView>();GameObject Root()=>View().gameObject;
 void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
 static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
 [UnityTest] public IEnumerator PortraitsAndLanguageSwitchPreserveStoryPosition(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;
 Assert.That(v.Panel.sprite,Is.SameAs(v.DobrynyaPanelRu));Assert.That(v.Speaker.enabled,Is.False);Assert.That(v.ContinueLabel.enabled,Is.False);Assert.That(v.Speaker.text,Is.EqualTo("ДОБРЫНЯ"));Assert.That(v.Description.text,Is.EqualTo(StoryService.CurrentLine.Text));Assert.That(v.Ivan.gameObject.activeSelf&&v.Dobrynya.gameObject.activeSelf,Is.True);
 yield return WaitForNormalButtonColors();Capture("Dialogue_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 DemoFeatureState.SetLanguage("en");yield return null;Assert.That(v.Panel.sprite,Is.SameAs(v.BlankPanel));Assert.That(v.Speaker.enabled&&v.ContinueLabel.enabled,Is.True);Assert.That(v.Speaker.text,Is.EqualTo("DOBRYNYA"));Assert.That(v.Description.text,Does.StartWith("Darkness gathers"));
 yield return WaitForNormalButtonColors();Capture("Dialogue_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Dialogue_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 DemoFeatureState.SetLanguage("ru");yield return null;Capture("Dialogue_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 router.NavigateBack();Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo("speaker.dobrynya"));
 }
 void ClickContinue(){var button=View().Continue;ExecuteEvents.Execute(button.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);Settle();}
 [UnityTest] public IEnumerator NativeContinueAdvancesOnceAndCompletesIntroBeforePreLevel(){
 int energy=GameState.Instance.Save.energy;var old=View().Continue;old.onClick.Invoke();old.onClick.Invoke();Settle();yield return null;
 Assert.That(View().Panel.sprite,Is.SameAs(View().BlankPanel));Assert.That(View().Speaker.enabled,Is.True);Capture("Dialogue_Ivan_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo("speaker.ivan"));Assert.That(GameState.Instance.HasCompletedStoryScene("chapter01.intro"),Is.False);
 ClickContinue();yield return null;Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo("speaker.dobrynya"));ClickContinue();yield return null;
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(GameState.Instance.HasCompletedStoryScene("chapter01.intro"),Is.True);Assert.That(GameState.Instance.Save.energy,Is.EqualTo(energy));Assert.That(StoryService.TryBeginChapterIntro(),Is.False);
 }
 [UnityTest] public IEnumerator EveryMilestoneRendersAndReturnsToItsEstablishedRoute(){
 while(router.CurrentScreen==ScreenId.Dialogue){ClickContinue();yield return null;}
 int completedLevel=0;
 foreach(var milestone in StoryCatalog.LoadChapter1().milestones){
 while(completedLevel<milestone.afterLevelId){var level=ChapterLevelCatalog.Find(1,++completedLevel);GameState.Instance.CompleteLevel(level,level.starScoreThresholds[0]);}Assert.That(StoryService.TryBeginPostVictoryDialogue(),Is.True);Assert.That(StoryService.ActiveSceneId,Is.EqualTo(milestone.sceneId));router.Show(ScreenId.Dialogue);Settle();yield return null;
 int line=0;
 while(router.CurrentScreen==ScreenId.Dialogue){
 foreach(var language in new[]{"ru","en"}){DemoFeatureState.SetLanguage(language);View().Refresh();Canvas.ForceUpdateCanvases();foreach(var t in View().GetComponentsInChildren<Text>())if(t.enabled)Assert.That(t.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(t.text.Length),milestone.sceneId+" "+t.name+" "+language);}
 if(milestone.afterLevelId==1&&line==1){Assert.That(View().Firebird.gameObject.activeSelf,Is.True);Assert.That(View().Dobrynya.gameObject.activeSelf,Is.False);yield return WaitForNormalButtonColors();Capture("Dialogue_Firebird_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
 if(milestone.afterLevelId==10&&line==0){Assert.That(View().Ivan.gameObject.activeSelf||View().Dobrynya.gameObject.activeSelf||View().Firebird.gameObject.activeSelf,Is.False);yield return WaitForNormalButtonColors();Capture("Dialogue_Finale_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));}
 if(milestone.afterLevelId==1&&line==1){DemoFeatureState.SetLanguage("ru");View().Refresh();Capture("Dialogue_Firebird_Short_RU",new Vector2Int(941,1450),new Rect(0,0,941,1450));}
 if(milestone.afterLevelId==10&&line==4){DemoFeatureState.SetLanguage("en");View().Refresh();Capture("Dialogue_FinaleLast_Tall_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));}
 ClickContinue();line++;yield return null;Assert.That(line,Is.LessThanOrEqualTo(5));
 }
 Assert.That(router.CurrentScreen,Is.EqualTo(milestone.afterLevelId==10?ScreenId.ChapterComplete:ScreenId.ChapterMap));Assert.That(GameState.Instance.HasCompletedStoryScene(milestone.sceneId),Is.True);
 }
 }
 [UnityTest] public IEnumerator RebindingAndDisabledStateDoNotAdvanceOrDuplicateActions(){
 var v=View();int old=0,current=0;v.Bind(()=>old++);v.Bind(()=>current++);v.Continue.interactable=false;v.Continue.onClick.Invoke();Assert.That(current,Is.Zero);v.Continue.interactable=true;v.Continue.onClick.Invoke();v.Continue.onClick.Invoke();Assert.That(current,Is.EqualTo(1));Assert.That(old,Is.Zero);
 StoryService.ResetRuntime();v.Refresh();Assert.That(v.Continue.interactable,Is.False);Assert.That(v.Description.text,Does.Contain("нет активной"));yield return null;
 }
 [UnityTest] public IEnumerator PressAndCancelDoesNotAdvanceAndInactiveStoryClearsSourceName(){
 var v=View();var feedback=v.Continue.GetComponent<ProductionMapButtonFeedback>();Assert.That(feedback,Is.Not.Null);
 var e=new PointerEventData(EventSystem.current){pointerId=17,button=PointerEventData.InputButton.Left};
 ExecuteEvents.Execute(v.Continue.gameObject,e,ExecuteEvents.pointerDownHandler);yield return null;
 Assert.That(feedback.IsPressed,Is.True);Assert.That(v.Continue.targetGraphic.canvasRenderer.GetColor().r,Is.LessThan(.8f));
 Capture("Dialogue_Pressed_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 ExecuteEvents.Execute(v.Continue.gameObject,e,ExecuteEvents.pointerExitHandler);yield return new WaitForSecondsRealtime(.2f);
 Assert.That(feedback.IsPressed,Is.False);Assert.That(v.Continue.targetGraphic.canvasRenderer.GetColor().r,Is.EqualTo(1).Within(.001f));
 Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo("speaker.dobrynya"));
 StoryService.ResetRuntime();v.Refresh();Assert.That(v.Panel.sprite,Is.SameAs(v.BlankPanel));Assert.That(v.Speaker.enabled,Is.True);Assert.That(v.Continue.interactable,Is.False);
 ExecuteEvents.Execute(v.Continue.gameObject,e,ExecuteEvents.pointerDownHandler);Assert.That(feedback.IsPressed,Is.False);
 }
 [UnityTest] public IEnumerator NativeIntroEdgesAndRelocalizationPreservePositionUntilContinue(){
 foreach(string language in new[]{"ru","en"}){
 router.Show(ScreenId.Home);Settle();GameState.Instance.ResetProgress();StoryService.ResetRuntime();Assert.That(StoryService.TryBeginChapterIntro(),Is.True);DemoFeatureState.SetLanguage(language);router.Show(ScreenId.Dialogue);Settle();yield return null;
 string save=StableSave(),lineText=StoryService.CurrentLine.Text;int energy=GameState.Instance.Save.energy;
 DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");yield return null;yield return WaitForNormalButtonColors();
 Capture("Dialogue_RelocalizedTall_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 Assert.That(View().Description.text,Is.EqualTo(StoryService.CurrentLineForLanguage(DemoFeatureState.LanguageCode).Text));Assert.That(StoryService.CurrentLine.Text,Is.EqualTo(lineText));Assert.That(StableSave(),Is.EqualTo(save));
 DemoFeatureState.SetLanguage(language);yield return null;
 var scene=StoryCatalog.FindScene("chapter01.intro");
 for(int i=0;i<scene.lines.Length;i++){
 yield return WaitForNormalButtonColors();Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo(scene.lines[i].speakerKey));int generation=router.RouteGeneration;
 router.NavigateBack();Assert.That(router.RouteGeneration,Is.EqualTo(generation),"System Back must not skip a story line");
 var v=View();bool shortScreen=i==1;
 Capture("Dialogue_NativeIntro_"+language+"_"+i,shortScreen?new Vector2Int(941,1450):new Vector2Int(576,1280),shortScreen?new Rect(0,0,941,1450):new Rect(0,48,576,1172),camera=>{
 Rect rect=Bounds(v.Continue.image.rectTransform,camera);Click(v.Continue,camera,new Vector2(i%2==0?rect.xMin+2:rect.xMax-2,rect.center.y));v.Continue.onClick.Invoke();
 });
 Settle();yield return null;Assert.That(router.RouteGeneration,Is.EqualTo(generation+1),"One route transition per tap even if repeated");
 if(i<scene.lines.Length-1){Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Dialogue));Assert.That(StoryService.CurrentSpeakerKey,Is.EqualTo(scene.lines[i+1].speakerKey));Assert.That(StableSave(),Is.EqualTo(save));}
 }
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(GameState.Instance.HasCompletedStoryScene(scene.id),Is.True);Assert.That(GameState.Instance.Save.energy,Is.EqualTo(energy));Assert.That(StoryService.TryBeginChapterIntro(),Is.False);
 }
 }
 [UnityTest] public IEnumerator NativeInactiveStoryCannotAdvanceInEitherLanguage(){
 StoryService.ResetRuntime();string save=StableSave();int generation=router.RouteGeneration;var v=View();
 foreach(string language in new[]{"ru","en"}){
 DemoFeatureState.SetLanguage(language);v.Refresh();yield return null;yield return WaitForNormalButtonColors();
 Assert.That(v.Continue.interactable,Is.False);Assert.That(v.Speaker.enabled,Is.True);Assert.That(v.Panel.sprite,Is.SameAs(v.BlankPanel));
 Capture("Dialogue_Inactive_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Continue,camera));
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Dialogue));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));Assert.That(StoryService.CurrentLine,Is.Null);
 }
 }
 static void Click(Button button,Camera camera){Click(button,camera,Bounds(button.image.rectTransform,camera).center);}
 static void Click(Button button,Camera camera,Vector2 point){
 var pointer=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
 }
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
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area ; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            Assert.That(View().transform.parent.GetComponentsInChildren<Button>().Length,Is.EqualTo(View().GetComponentsInChildren<Button>().Length),"No legacy shortcut controls outside the native page");
            string directory = Environment.GetEnvironmentVariable("LOR_DIALOGUE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("DialogueCaptureCamera",typeof(Camera));
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
        static void VerifyLayoutAndInput(ProductionDialogueView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>()){
                Rect rect=Bounds((RectTransform)button.transform,camera);Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name);
                Assert.That(rect.width,Is.GreaterThanOrEqualTo(44),button.name+" touch width");Assert.That(rect.height,Is.GreaterThanOrEqualTo(44),button.name+" touch height");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            foreach(var art in new[]{v.Panel,v.Ivan,v.Dobrynya,v.Firebird,v.Continue.image})if(art.gameObject.activeInHierarchy){
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple),art.name);Assert.That(art.preserveAspect,Is.True,art.name);
                Rect rect=Bounds(art.rectTransform,camera);Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,art.name+" full sprite within safe area");
            }
            Rect panel=Bounds(v.Panel.rectTransform,camera),description=Bounds(v.Description.rectTransform,camera),buttonBounds=Bounds(v.Continue.image.rectTransform,camera);
            Assert.That(panel.Contains(description.min)&&panel.Contains(description.max),Is.True,"Story text remains inside parchment");Assert.That(description.Overlaps(buttonBounds),Is.False,"Story text must not cover Continue");
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text))Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(text.text.Length),text.name+" complete text");
        }
    }
}
