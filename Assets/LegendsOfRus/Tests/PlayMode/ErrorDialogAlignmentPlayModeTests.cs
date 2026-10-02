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
 public sealed class ErrorDialogAlignmentPlayModeTests
 {
 ScreenRouter router;string previousLanguage;
 [UnitySetUp] public IEnumerator SetUp(){string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");router.Show(ScreenId.Home);Settle();yield return null;Assert.That(router.ShowErrorDialog(),Is.True);Settle();yield return null;}
 [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.Home);Settle();}DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
 ProductionErrorDialogView View()=>router.Viewport.GetComponentInChildren<ProductionErrorDialogView>();GameObject Root()=>View().gameObject;
 void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
 static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
 [UnityTest] public IEnumerator GenericDialogIsHonestLocalizedAndFitsPortraitDisplays(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;Assert.That(v.CanRetry,Is.False);Assert.That(v.Retry.interactable,Is.False);Assert.That(v.Support.interactable,Is.False);Assert.That(v.SupportLabel.text,Does.Contain("НЕДОСТУПНА"));
 yield return WaitForNormalButtonColors();Capture("ErrorDialog_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 DemoFeatureState.SetLanguage("en");yield return null;Assert.That(v.CloseLabel.text,Is.EqualTo("CLOSE"));yield return WaitForNormalButtonColors();Capture("ErrorDialog_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("ErrorDialog_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 DemoFeatureState.SetLanguage("ru");yield return null;yield return WaitForNormalButtonColors();Capture("ErrorDialog_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 Assert.That(StableSave(),Is.EqualTo(save));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
 }
 [UnityTest] public IEnumerator CloseAndSystemBackPreserveUnderlyingHomeInstance(){
 var home=router.Viewport.GetComponentInChildren<ProductionHomeView>();string save=StableSave();int generation=router.RouteGeneration;
 ExecuteEvents.Execute(View().Close.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);Assert.That(router.HasActiveModalBlocker,Is.True);Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);Assert.That(router.Viewport.GetComponentInChildren<ProductionHomeView>(),Is.SameAs(home));
 router.Show(ScreenId.EmptyErrorState);Settle();yield return null;Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.EmptyErrorState));router.NavigateBack();Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator RetryRunsOnceAfterCloseAndCanNavigate(){
 router.NavigateBack();Settle();yield return null;int attempts=0;bool closed=false;Assert.That(router.ShowErrorDialog(()=>{attempts++;closed=!router.ActiveModal.HasValue;router.Show(ScreenId.Shop);}),Is.True);Settle();yield return null;
 Assert.That(View().CanRetry,Is.True);yield return WaitForNormalButtonColors();Capture("ErrorDialog_RetryReady_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));var v=View();v.Retry.onClick.Invoke();v.Retry.onClick.Invoke();Assert.That(attempts,Is.Zero);Settle();yield return null;Assert.That(attempts,Is.EqualTo(1));Assert.That(closed,Is.True);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Shop));
 }
 [UnityTest] public IEnumerator RouteChangeCancelsPendingRetry(){
 router.NavigateBack();Settle();yield return null;int attempts=0;Assert.That(router.ShowErrorDialog(()=>attempts++),Is.True);Settle();yield return null;View().Retry.onClick.Invoke();router.Show(ScreenId.Inventory);Settle();yield return null;Assert.That(attempts,Is.Zero);Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Inventory));
 }
 [UnityTest] public IEnumerator ModalBlocksUnderlyingInputAndRejectsDuplicateRequests(){
 int attempts=0;Assert.That(router.ShowErrorDialog(()=>attempts++),Is.False);var home=router.Viewport.GetComponentInChildren<ProductionHomeView>();var button=home.GetComponentInChildren<Button>();var hits=new List<RaycastResult>();var rect=(RectTransform)button.transform;var point=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.transform.IsChildOf(home.transform),Is.False);
 View().Retry.onClick.Invoke();View().Support.onClick.Invoke();Assert.That(attempts,Is.Zero);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));View().Close.onClick.Invoke();Assert.That(router.HasActiveModalBlocker,Is.True);Assert.That(router.ShowErrorDialog(()=>attempts++),Is.False);Settle();yield return null;Assert.That(attempts,Is.Zero);
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
                    Color expected=(button.IsInteractable()?button.colors.normalColor:button.colors.disabledColor)*button.colors.colorMultiplier;
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
            string directory = Environment.GetEnvironmentVariable("LOR_ERROR_DIALOG_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ErrorDialogCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionErrorDialogView>(),camera,size,safeArea);
                inspect?.Invoke(camera);
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
        static void VerifyLayoutAndInput(ProductionErrorDialogView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Shade.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>()){
                Rect rect=Bounds((RectTransform)button.transform,camera);Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text)){
                Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name);
                Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(text.text.Length),text.name+" complete text must be rendered");
            }
        }
    }
}
