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
 public sealed class MaintenanceAlignmentPlayModeTests
 {
 ScreenRouter router;string previousLanguage;
 [UnitySetUp] public IEnumerator SetUp(){string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");router.Show(ScreenId.MaintenanceState);Settle();yield return null;}
 [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.Home);Settle();}DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
 ProductionMaintenanceView View()=>router.Viewport.GetComponentInChildren<ProductionMaintenanceView>();GameObject Root()=>View().gameObject;
 void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
 static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
 [UnityTest] public IEnumerator ReferencePageLocalizesWithoutCountdownOrRewardGrant(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;Assert.That(v.EstimateValue.text,Is.EqualTo("Время не объявлено"));Assert.That(v.CompensationDescription.text,Does.Contain("не подтверждена"));Assert.That(v.PreviewLabel.text,Is.EqualTo("ПРЕДПРОСМОТР ПОДАРКА"));
 yield return WaitForNormalButtonColors();Capture("Maintenance_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 DemoFeatureState.SetLanguage("en");yield return null;Assert.That(v.EstimateValue.text,Is.EqualTo("Not announced"));Assert.That(v.CompensationDescription.text,Does.Contain("not"));yield return WaitForNormalButtonColors();Capture("Maintenance_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Maintenance_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 DemoFeatureState.SetLanguage("ru");yield return null;yield return WaitForNormalButtonColors();Capture("Maintenance_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 Assert.That(v.EstimateValue.text,Is.EqualTo("Время не объявлено"));Assert.That(StableSave(),Is.EqualTo(save));Assert.That(router.RouteGeneration,Is.EqualTo(generation));
 }
 [UnityTest] public IEnumerator UnderstoodAndSystemBackReturnHomeWithoutChangingSave(){
 string save=StableSave();var button=View().Continue;ExecuteEvents.Execute(button.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(StableSave(),Is.EqualTo(save));
 router.Show(ScreenId.MaintenanceState);Settle();yield return null;router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator RebindingRepeatedClicksAndDisabledStateProtectExit(){
 var v=View();int old=0,current=0;v.Bind(()=>old++);v.Bind(()=>current++);v.Continue.interactable=false;v.Continue.onClick.Invoke();Assert.That(current,Is.Zero);v.Continue.interactable=true;v.Continue.onClick.Invoke();v.Continue.onClick.Invoke();Assert.That(current,Is.EqualTo(1));Assert.That(old,Is.Zero);yield return null;
 }
 [UnityTest] public IEnumerator SourceLetteringAndPressFeedbackPreserveStatusAndSave(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;yield return WaitForNormalButtonColors();
 Assert.That(v.Heading.enabled,Is.False);Assert.That(v.EstimateHeading.enabled,Is.False);Assert.That(v.ContinueLabel.enabled,Is.False);
 Assert.That(v.Panel.sprite,Is.SameAs(ProductionMaintenanceRuntime.Load("panel_ru_surface_r3")));Assert.That(v.EstimateValue.enabled,Is.True);
 var feedback=v.Continue.GetComponent<ProductionMapButtonFeedback>();Assert.That(feedback,Is.Not.Null);
 var data=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,pointerId=23};Color original=v.Continue.targetGraphic.canvasRenderer.GetColor();
 ExecuteEvents.Execute(v.Continue.gameObject,data,ExecuteEvents.pointerDownHandler);Assert.That(feedback.IsPressed,Is.True);Assert.That(v.Continue.targetGraphic.canvasRenderer.GetColor().r,Is.LessThan(original.r));
 Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));Capture("Maintenance_Pressed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 ExecuteEvents.Execute(v.Continue.gameObject,data,ExecuteEvents.pointerExitHandler);yield return WaitForNormalButtonColors();Assert.That(feedback.IsPressed,Is.False);
 DemoFeatureState.SetLanguage("en");v.Refresh();Assert.That(v.Heading.enabled&&v.EstimateHeading.enabled&&v.ContinueLabel.enabled,Is.True);Assert.That(v.Panel.sprite,Is.SameAs(ProductionMaintenanceRuntime.Load("panel_en_surface_r3")));Assert.That(((Image)v.Continue.targetGraphic).sprite,Is.SameAs(ProductionMaintenanceRuntime.Load("button")));
 DemoFeatureState.SetLanguage("ru");v.Refresh();Assert.That(v.Heading.enabled,Is.False);Assert.That(v.EstimateValue.text,Is.EqualTo("Время не объявлено"));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator NativeDisabledRelocalizedAndEdgeReturnPreserveProgress(){
 string save=StableSave();
 foreach(string language in new[]{"ru","en"}){
 router.Show(ScreenId.Home);Settle();DemoFeatureState.SetLanguage(language);router.Show(ScreenId.MaintenanceState);Settle();yield return null;
 var v=View();int generation=router.RouteGeneration;v.Continue.interactable=false;yield return WaitForNormalButtonColors();
 Capture("Maintenance_Disabled_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Continue,camera));
 Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.MaintenanceState));Assert.That(router.RouteGeneration,Is.EqualTo(generation));
 v.Continue.interactable=true;DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");yield return null;yield return WaitForNormalButtonColors();
 Capture("Maintenance_RelocalizedTall_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 Assert.That(v.EstimateValue.text,Is.EqualTo(language=="ru"?"Not announced":"Время не объявлено"));
 DemoFeatureState.SetLanguage(language);yield return null;yield return WaitForNormalButtonColors();
 Capture("Maintenance_Short_"+language,new Vector2Int(941,1450),new Rect(0,0,941,1450));
 Capture("Maintenance_NativeReturn_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{
 Rect rect=Bounds(v.Continue.image.rectTransform,camera);Click(v.Continue,camera,new Vector2(rect.xMin+2,rect.center.y));v.Continue.onClick.Invoke();});
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));Assert.That(StableSave(),Is.EqualTo(save));
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
            string directory = Environment.GetEnvironmentVariable("LOR_MAINTENANCE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("MaintenanceCaptureCamera",typeof(Camera));
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
        static void VerifyLayoutAndInput(ProductionMaintenanceView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>()){
                Rect rect=Bounds((RectTransform)button.transform,camera);Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name);
                Assert.That(rect.width,Is.GreaterThanOrEqualTo(44),button.name+" target width");Assert.That(rect.height,Is.GreaterThanOrEqualTo(44),button.name+" target height");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            foreach(var art in new[]{v.Panel,v.Logo,v.Chest,v.EstimatePanel,v.Continue.image}){
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple),art.name);Assert.That(art.preserveAspect,Is.True,art.name);
                Rect bounds=Bounds(art.rectTransform,camera);Assert.That(bounds.xMin>=safeArea.xMin-1&&bounds.yMin>=safeArea.yMin-1&&bounds.xMax<=safeArea.xMax+1&&bounds.yMax<=safeArea.yMax+1,Is.True,art.name+" complete source within safe area");
            }
            Rect chest=Bounds(v.Chest.rectTransform,camera),buttonBounds=Bounds(v.Continue.image.rectTransform,camera);
            Assert.That(chest.Overlaps(buttonBounds),Is.False,"Gift illustration clears Continue");
            Assert.That(v.Chest.GetComponentsInChildren<Button>(),Is.Empty,"Preview is not a reward claim");
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text)){Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name);Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(text.text.Replace("\n","").Length),text.name+" complete text");}
        }
    }
}
