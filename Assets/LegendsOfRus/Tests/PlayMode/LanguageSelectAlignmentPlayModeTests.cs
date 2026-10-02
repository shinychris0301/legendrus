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
    public sealed class LanguageSelectAlignmentPlayModeTests
    {
        ScreenRouter router; string priorLanguage, priorPreference; bool hadPreference;
        [UnitySetUp] public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated)) Assert.Ignore("Requires isolated storage.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar), Is.True);
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            router=ScreenRouter.Instance; priorLanguage=DemoFeatureState.LanguageCode;
            hadPreference=PlayerPrefs.HasKey(LanguagePreference.Key); priorPreference=PlayerPrefs.GetString(LanguagePreference.Key);
            DemoFeatureState.SetLanguage("ru"); router.Show(ScreenId.SplashTitle); Settle();
            router.Show(ScreenId.LanguageSelect); Settle(); yield return null;
        }
        [UnityTearDown] public IEnumerator TearDown()
        {
            if (router) { router.Show(ScreenId.SplashTitle); Settle(); }
            PlayerPrefs.DeleteKey(LanguagePreference.Key); if(hadPreference)PlayerPrefs.SetString(LanguagePreference.Key,priorPreference);PlayerPrefs.Save();
            DemoFeatureState.SetLanguage(priorLanguage); yield return null;
        }
        ProductionLanguageSelectView View()=>router.Viewport.GetComponentInChildren<ProductionLanguageSelectView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var c=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));c.lastEnergyUtcTicks=0;return JsonUtility.ToJson(c);}
        [UnityTest] public IEnumerator LocalizedPortraitsAndPointerSelection()
        {
            var v=View(); Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);
            string save=StableSave(); int generation=router.RouteGeneration;
            yield return WaitForNormalButtonColors();
            Capture("LanguageSelect_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("LanguageSelect_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("LanguageSelect_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            ExecuteEvents.Execute(v.English.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);
            Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("en")); Assert.That(LanguagePreference.Load(),Is.EqualTo("en"));
            Assert.That(v.English.image.sprite.name,Is.EqualTo("language_selected"));
            Assert.That(v.Russian.image.sprite.name,Is.EqualTo("language_unselected"));
            yield return null;yield return WaitForNormalButtonColors();
            Capture("LanguageSelect_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("LanguageSelect_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(),Is.EqualTo(save));Assert.That(router.RouteGeneration,Is.EqualTo(generation));
        }
        [UnityTest] public IEnumerator CloseBackAndRepeatedBindingReturnToOrigin()
        {
            var v=View();int closed=0;v.Bind(()=>closed++);v.Bind(()=>closed++);
            v.Close.onClick.Invoke();v.Close.onClick.Invoke();Assert.That(closed,Is.EqualTo(1));
            v.Bind(router.NavigateBack);v.Close.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));
            router.Show(ScreenId.Settings);Settle();router.Show(ScreenId.LanguageSelect);Settle();
            router.Show(ScreenId.LanguageSelect);Settle();router.NavigateBack();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Settings));
            router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SplashTitle));
            router.Show(ScreenId.Home);Settle();router.Show(ScreenId.LanguageSelect);Settle();router.NavigateBack();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
        }
        [UnityTest] public IEnumerator PreferenceReloadAndSettingsShareSelectionWithoutChangingProgress()
        {
            string save=StableSave();PlayerPrefs.DeleteKey(LanguagePreference.Key);DemoFeatureState.ResetSession();
            Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));
            PlayerPrefs.SetString(LanguagePreference.Key,"unsupported");DemoFeatureState.ResetSession();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));
            View().English.onClick.Invoke();DemoFeatureState.ResetSession();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("en"));
            GameState.Instance.Persist();Assert.That(LanguagePreference.Load(),Is.EqualTo("en"));
            router.Show(ScreenId.Settings);Settle();yield return null;
            var settings=router.Viewport.GetComponentInChildren<ProductionSettingsView>();Assert.That(settings.LanguageValue.text,Is.EqualTo("English"));
            settings.Rows[0].onClick.Invoke();settings.LanguageButtons[0].onClick.Invoke();
            DemoFeatureState.ResetSession();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));Assert.That(LanguagePreference.Load(),Is.EqualTo("ru"));
            Assert.That(StableSave(),Is.EqualTo(save));
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

            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_LANGUAGE_SELECT_CAPTURE_DIR");
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
            var cameraGo = new GameObject("SettingsCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionLanguageSelectView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionLanguageSelectView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect shield=Bounds(v.Shield,camera);Assert.That(shield.xMin,Is.LessThanOrEqualTo(1));Assert.That(shield.yMin,Is.LessThanOrEqualTo(1));Assert.That(shield.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(shield.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var b in new[]{v.Russian,v.English,v.Close})
            {
                Rect r=Bounds((RectTransform)b.transform,camera);Assert.That(safeArea.Contains(r.min)&&safeArea.Contains(r.max),Is.True,b.name);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=r.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(b));
            }
            var blocked=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=new Vector2(size.x*.5f,size.y*.85f)},blocked);
            Assert.That(blocked[0].gameObject,Is.EqualTo(v.Shield.gameObject));
            foreach(var b in v.Backdrop.GetComponentsInChildren<Button>())Assert.That(b.IsInteractable(),Is.False);
            foreach(var label in v.Body.GetComponentsInChildren<Text>())if(label.enabled){Assert.That(label.cachedTextGenerator.vertexCount,Is.GreaterThan(0));}
        }
    }
}
