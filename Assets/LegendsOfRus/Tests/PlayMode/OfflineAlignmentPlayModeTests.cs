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
 public sealed class OfflineAlignmentPlayModeTests
 {
 ScreenRouter router;string previousLanguage;
 [UnitySetUp] public IEnumerator SetUp(){string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");router.Show(ScreenId.OfflineState);Settle();yield return null;}
 [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.Home);Settle();}DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
 ProductionOfflineView View()=>router.Viewport.GetComponentInChildren<ProductionOfflineView>();GameObject Root()=>View().gameObject;
 void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
 static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
 [UnityTest] public IEnumerator RetryChecksNetworkWithoutClaimingServiceSuccessOrChangingSave(){
 var v=View();NetworkReachability reach=NetworkReachability.NotReachable;int probes=0;v.Bind(router.NavigateBack,()=>{probes++;return reach;});string save=StableSave();int generation=router.RouteGeneration;
 Assert.That(v.NetworkAvailable,Is.False);Assert.That(v.Emblem.gameObject.activeSelf,Is.True);Assert.That(probes,Is.EqualTo(1));
 yield return WaitForNormalButtonColors();Capture("Offline_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 v.Retry.onClick.Invoke();Assert.That(probes,Is.EqualTo(2));Assert.That(v.CheckCount,Is.EqualTo(1));Assert.That(v.NetworkAvailable,Is.False);
 yield return WaitForNormalButtonColors();Capture("Offline_Retry_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 reach=NetworkReachability.ReachableViaLocalAreaNetwork;v.Retry.onClick.Invoke();Assert.That(v.NetworkAvailable,Is.True);Assert.That(v.Emblem.gameObject.activeSelf,Is.True);Assert.That(v.Description.text,Does.Contain("не подтверждён"));Assert.That(router.RouteGeneration,Is.EqualTo(generation));
 yield return WaitForNormalButtonColors();Capture("NetworkDetected_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 DemoFeatureState.SetLanguage("en");v.Refresh();Assert.That(v.Description.text,Does.Contain("not confirmed"));
 yield return WaitForNormalButtonColors();Capture("NetworkDetected_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 reach=NetworkReachability.NotReachable;v.Retry.onClick.Invoke();Assert.That(v.NetworkAvailable,Is.False);
 yield return WaitForNormalButtonColors();Capture("Offline_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 DemoFeatureState.SetLanguage("ru");v.Refresh();yield return WaitForNormalButtonColors();Capture("Offline_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
 Assert.That(StableSave(),Is.EqualTo(save));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.OfflineState));
 }
 [UnityTest] public IEnumerator BackAndSystemBackReturnHome(){
 View().Back.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
 router.Show(ScreenId.OfflineState);Settle();yield return null;var close=View().Back;ExecuteEvents.Execute(close.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
 router.Show(ScreenId.OfflineState);Settle();router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
 }
 [UnityTest] public IEnumerator RebindingAndRepeatedClicksCannotDuplicateExit(){
 var v=View();int old=0,current=0,probes=0;v.Bind(()=>old++,()=>NetworkReachability.NotReachable);v.Bind(()=>current++,()=>{probes++;return NetworkReachability.ReachableViaCarrierDataNetwork;});v.Retry.onClick.Invoke();Assert.That(probes,Is.EqualTo(2));Assert.That(v.NetworkAvailable,Is.True);v.Back.onClick.Invoke();v.Back.onClick.Invoke();v.Retry.onClick.Invoke();Assert.That(current,Is.EqualTo(1));Assert.That(old,Is.Zero);Assert.That(probes,Is.EqualTo(2));yield return null;
 }
 [UnityTest] public IEnumerator FocusRefreshAndLanguageChangesKeepAvailabilityTruthful(){
 var v=View();var reach=NetworkReachability.NotReachable;v.Bind(router.NavigateBack,()=>reach);reach=NetworkReachability.ReachableViaCarrierDataNetwork;v.SendMessage("OnApplicationFocus",true);Assert.That(v.NetworkAvailable,Is.True);Assert.That(v.CheckCount,Is.Zero);DemoFeatureState.SetLanguage("en");yield return null;Assert.That(v.Heading.text,Is.EqualTo("CHECK CONNECTION"));reach=NetworkReachability.NotReachable;v.SendMessage("OnApplicationFocus",true);Assert.That(v.NetworkAvailable,Is.False);Assert.That(v.Heading.text,Is.EqualTo("NO CONNECTION"));
 }
 [UnityTest] public IEnumerator HeaderShowsSavedBalancesAndRoutesToExistingPages(){
 var v=View();var save=GameState.Instance.Save;Assert.That(v.GoldText.text,Is.EqualTo(ProductionInventoryView.Number(save.gold)));Assert.That(v.GemsText.text,Is.EqualTo(ProductionInventoryView.Number(save.gems)));Assert.That(v.EnergyText.text,Is.EqualTo(save.energy+"/"+save.energyMax));
 v.ProfileButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Profile));
 router.Show(ScreenId.OfflineState);Settle();yield return null;View().SettingsButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Settings));
 for(int i=0;i<3;i++){router.Show(ScreenId.OfflineState);Settle();yield return null;View().PlusButtons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(i==2?ScreenId.OutOfEnergy:ScreenId.Shop));}
 }
 [UnityTest] public IEnumerator SharedHeaderLetteringAndPressFeedbackFollowState(){
 var v=View();v.Bind(router.NavigateBack,()=>NetworkReachability.NotReachable);yield return WaitForNormalButtonColors();
 var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
 Assert.That(v.Header.Backing.sprite,Is.SameAs(home.Header.Backing.sprite));Assert.That(v.Header.Avatar.sprite,Is.SameAs(home.Header.Avatar.sprite));
 Assert.That(v.Panel.sprite,Is.SameAs(ProductionOfflineRuntime.Load("panel_ru_r3")));Assert.That(v.Heading.enabled,Is.False);Assert.That(v.RetryLabel.enabled,Is.False);
 foreach(var button in new[]{v.Retry,v.Back}){
 var feedback=button.GetComponent<ProductionMapButtonFeedback>();Assert.That(feedback,Is.Not.Null);
 var data=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,pointerId=23};
 Color original=button.targetGraphic.canvasRenderer.GetColor();ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerDownHandler);
 Assert.That(feedback.IsPressed,Is.True);Assert.That(button.targetGraphic.canvasRenderer.GetColor().r,Is.LessThan(original.r));Assert.That(v.CheckCount,Is.Zero);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.OfflineState));
 ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerExitHandler);yield return WaitForNormalButtonColors();Assert.That(feedback.IsPressed,Is.False);
 }
 DemoFeatureState.SetLanguage("en");v.Refresh();Assert.That(v.Heading.enabled,Is.True);Assert.That(v.RetryLabel.enabled,Is.True);Assert.That(((Image)v.Retry.targetGraphic).sprite,Is.SameAs(ProductionOfflineRuntime.Load("retry")));
 DemoFeatureState.SetLanguage("ru");v.Bind(router.NavigateBack,()=>NetworkReachability.ReachableViaLocalAreaNetwork);Assert.That(v.Heading.enabled,Is.True);Assert.That(v.Panel.sprite,Is.SameAs(ProductionOfflineRuntime.Load("panel")));Assert.That(v.Heading.text,Is.EqualTo("ПРОВЕРЬТЕ СЕТЬ"));
 }
 [UnityTest] public IEnumerator NativeRetryRelocalizationAndHomePreserveSave(){
 string save=StableSave();
 foreach(string language in new[]{"ru","en"}){
 router.Show(ScreenId.Home);Settle();DemoFeatureState.SetLanguage(language);router.Show(ScreenId.OfflineState);Settle();yield return null;
 var v=View();var reach=NetworkReachability.NotReachable;int probes=0;
 v.Bind(router.NavigateBack,()=>{probes++;return reach;});int generation=router.RouteGeneration;
 yield return WaitForNormalButtonColors();
 Capture("Offline_NativeRetry_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Retry,camera));
 Assert.That(v.CheckCount,Is.EqualTo(1));Assert.That(probes,Is.EqualTo(2));Assert.That(v.NetworkAvailable,Is.False);
 reach=NetworkReachability.ReachableViaLocalAreaNetwork;yield return null;
 Capture("Offline_TallDisconnected_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(v.Retry,camera));
 Assert.That(v.NetworkAvailable,Is.True);Assert.That(v.CheckCount,Is.EqualTo(2));Assert.That(probes,Is.EqualTo(3));
 Assert.That(v.Description.text,Does.Contain(language=="ru"?"не подтверждён":"not confirmed"));
 Assert.That(router.RouteGeneration,Is.EqualTo(generation));reach=NetworkReachability.NotReachable;yield return null;
 Capture("Offline_NativeConnected_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(v.Retry,camera));
 Assert.That(v.NetworkAvailable,Is.False);Assert.That(v.CheckCount,Is.EqualTo(3));Assert.That(probes,Is.EqualTo(4));
 DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");v.Refresh();yield return null;
 Capture("Offline_RelocalizedReturn_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>{Click(v.Back,camera);v.Back.onClick.Invoke();});
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 [UnityTest] public IEnumerator NativeHeaderTargetsRouteToCorrectDestinations(){
 string save=StableSave();
 for(int i=0;i<5;i++){
 router.Show(ScreenId.OfflineState);Settle();yield return null;yield return WaitForNormalButtonColors();var v=View();
 Capture("Offline_HeaderTarget_"+i,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{
 var b=i<3?v.PlusButtons[i]:(i==3?v.ProfileButton:v.SettingsButton);Rect art=Bounds(b.image.rectTransform,camera);Click(b,camera,i<3?new Vector2(art.xMin-2,art.center.y):art.center);});
 Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(i<2?ScreenId.Shop:i==2?ScreenId.OutOfEnergy:i==3?ScreenId.Profile:ScreenId.Settings));
 if(i<2)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==0?"coins":"gems"));
 Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 static void Click(Button button,Camera camera){Click(button,camera,Bounds(button.image.rectTransform,camera).center);}
 static void Click(Button button,Camera camera,Vector2 point){var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);}
 static Rect Target(Button button,Camera camera){Rect art=Bounds(button.image.rectTransform,camera);float scale=art.width/button.image.rectTransform.rect.width;Vector4 p=button.image.raycastPadding;return Rect.MinMaxRect(art.xMin+p.x*scale,art.yMin+p.y*scale,art.xMax-p.z*scale,art.yMax-p.w*scale);}
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
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area and network availability; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            Assert.That(View().transform.parent.GetComponentsInChildren<Button>().Length,Is.EqualTo(View().GetComponentsInChildren<Button>().Length),"No legacy shortcut controls outside the native page");
            string directory = Environment.GetEnvironmentVariable("LOR_OFFLINE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("OfflineCaptureCamera",typeof(Camera));
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
        static void VerifyLayoutAndInput(ProductionOfflineView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(1),"Header follows safe top edge");
            foreach(var button in v.GetComponentsInChildren<Button>()){
                Rect rect=Bounds((RectTransform)button.transform,camera);Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,button.name);
                Rect target=Target(button,camera);Assert.That(target.width,Is.GreaterThanOrEqualTo(44),button.name+" target width");Assert.That(target.height,Is.GreaterThanOrEqualTo(44),button.name+" target height");
                Assert.That(target.xMin>=safeArea.xMin-1&&target.yMin>=safeArea.yMin-1&&target.xMax<=safeArea.xMax+1&&target.yMax<=safeArea.yMax+1,Is.True,button.name+" safe target");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            var buttons=v.GetComponentsInChildren<Button>();
            for(int i=0;i<buttons.Length;i++)for(int j=i+1;j<buttons.Length;j++)Assert.That(Target(buttons[i],camera).Overlaps(Target(buttons[j],camera)),Is.False,buttons[i].name+" overlaps "+buttons[j].name);
            foreach(var art in new[]{v.Panel,v.Emblem,v.Retry.image,v.Back.image}){
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple));Assert.That(art.preserveAspect,Is.True);
                Rect bounds=Bounds(art.rectTransform,camera);Assert.That(bounds.xMin>=safeArea.xMin-1&&bounds.yMin>=safeArea.yMin-1&&bounds.xMax<=safeArea.xMax+1&&bounds.yMax<=safeArea.yMax+1,Is.True,art.name+" complete artwork inside safe area");
            }
            Assert.That(v.Description.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(v.Description.text.Replace("\n","").Length),"Full connectivity message must remain visible");
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text))Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name);
        }
    }
}
