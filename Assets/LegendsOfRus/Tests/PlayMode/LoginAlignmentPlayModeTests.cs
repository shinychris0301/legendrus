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
    public sealed class LoginAlignmentPlayModeTests
    {
        ScreenRouter router;string previousLanguage;bool guestBefore;
        [UnitySetUp] public IEnumerator SetUp()
        {
            string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");if(string.IsNullOrEmpty(isolated))Assert.Ignore("Requires isolated storage.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);
            yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);router=ScreenRouter.Instance;previousLanguage=DemoFeatureState.LanguageCode;guestBefore=DemoFeatureState.GuestSignedIn;
            DemoFeatureState.ResetSession();DemoFeatureState.SetLanguage("ru");router.Show(ScreenId.SplashTitle);Settle();router.Show(ScreenId.Login);Settle();yield return null;
        }
        [UnityTearDown] public IEnumerator TearDown(){if(router){router.Show(ScreenId.SplashTitle);Settle();}DemoFeatureState.ResetSession();if(guestBefore)DemoFeatureState.SignInGuest();DemoFeatureState.SetLanguage(previousLanguage);yield return null;}
        ProductionLoginView View()=>router.Viewport.GetComponentInChildren<ProductionLoginView>();GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
        [UnityTest]public IEnumerator LocalizedLayoutsPreserveProgress()
        {
            var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(v.OnlineAuthenticationAvailable,Is.False);string save=StableSave();
            yield return WaitForNormalButtonColors();Capture("Login_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Login_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("Login_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Login_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Login_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.GuestSignedIn,Is.False);
        }
        [UnityTest]public IEnumerator FormVisibilityUnavailableActionsAndLegalState()
        {
            var v=View();string save=StableSave();var privacy=PrivacySupportService.Instance;object consent=privacy!=null?typeof(PrivacySupportService).GetProperty("State").GetValue(privacy):null;
            v.Identity.text="demo@example.invalid";v.Password.text="SyntheticTest42";Assert.That(v.Password.contentType,Is.EqualTo(InputField.ContentType.Password));v.Eye.onClick.Invoke();Assert.That(v.PasswordVisible,Is.True);Assert.That(v.Password.contentType,Is.EqualTo(InputField.ContentType.Standard));v.Eye.onClick.Invoke();Assert.That(v.Password.contentType,Is.EqualTo(InputField.ContentType.Password));
            int generation=router.RouteGeneration;var actions=new List<Button>{v.Login,v.Link,v.Terms,v.Privacy};actions.AddRange(v.Providers);
            foreach(var button in actions){button.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);Assert.That(v.InfoDescription.text,Does.Not.Contain("SyntheticTest42"));v.Guest.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(DemoFeatureState.GuestSignedIn,Is.False);router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);}
            v.Login.onClick.Invoke();yield return WaitForNormalButtonColors();Capture("Login_Unavailable_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();Capture("Login_Unavailable_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.InfoClose.onClick.Invoke();
            if(privacy!=null)Assert.That(typeof(PrivacySupportService).GetProperty("State").GetValue(privacy),Is.SameAs(consent));Assert.That(StableSave(),Is.EqualTo(save));
            v.gameObject.SetActive(false);Assert.That(v.Identity.text,Is.Empty);Assert.That(v.Password.text,Is.Empty);Assert.That(v.PasswordVisible,Is.False);yield return null;
        }
        [UnityTest]public IEnumerator GuestCommitsOnceAndDoesNotResetProgress()
        {
            var v=View();string save=StableSave();v.Identity.text="demo@example.invalid";v.Password.text="SyntheticTest42";int n=0;
            v.Bind(()=>n++,()=>{});v.Bind(()=>n++,()=>{});v.Guest.onClick.Invoke();v.Guest.onClick.Invoke();Assert.That(n,Is.EqualTo(1));Assert.That(v.Identity.text,Is.Empty);Assert.That(v.Password.text,Is.Empty);
            router.Show(ScreenId.Login);Settle();yield return null;int generation=router.RouteGeneration;var guest=View().Guest;ExecuteEvents.Execute(guest.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);guest.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));Assert.That(DemoFeatureState.GuestSignedIn,Is.True);Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator SplashAccountAndSettingsReturnToTheirOrigins()
        {
            router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));
            router.Viewport.GetComponentInChildren<SplashTitleView>().AccountButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Login));View().Back.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));
            router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Settings);Settle();yield return null;router.Viewport.GetComponentInChildren<ProductionSettingsView>().Rows[2].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Login));router.Show(ScreenId.Login);Settle();router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Settings));router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
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
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            Assert.That(View().transform.parent.GetComponentsInChildren<Button>().Length,Is.EqualTo(View().GetComponentsInChildren<Button>().Length),"No legacy shortcut controls outside the native page");
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_LOGIN_CAPTURE_DIR");
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
            var cameraGo = new GameObject("LoginCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionLoginView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionLoginView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>()){
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" input target");
            }
            if(!v.InfoPanel.activeSelf)foreach(var input in new[]{v.Identity,v.Password}){
                Rect rect=Bounds((RectTransform)input.transform,camera);var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);Assert.That(hits[0].gameObject.GetComponentInParent<InputField>(),Is.SameAs(input));
            }
            foreach(var text in v.GetComponentsInChildren<Text>())if(text.enabled&&text.gameObject.activeInHierarchy&&!string.IsNullOrEmpty(text.text))Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name);
        }
    }
}
