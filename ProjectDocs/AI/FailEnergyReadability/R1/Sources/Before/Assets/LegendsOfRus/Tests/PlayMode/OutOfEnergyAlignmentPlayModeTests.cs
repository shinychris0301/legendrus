using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class OutOfEnergyAlignmentPlayModeTests
    {
        ScreenRouter router;
        string previousSave, previousLanguage;
        readonly string[] keys = { "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging" };
        string[] values;
        bool[] existed;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated)) Assert.Ignore("Requires isolated test storage.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated) + Path.DirectorySeparatorChar), Is.True);
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(ScreenRouter.Instance, Is.SameAs(router));
            previousSave = JsonUtility.ToJson(GameState.Instance.Save);
            previousLanguage = DemoFeatureState.LanguageCode;
            values = new string[keys.Length]; existed = new bool[keys.Length];
            for (int i = 0; i < keys.Length; i++) { existed[i] = PlayerPrefs.HasKey(keys[i]); values[i] = PlayerPrefs.GetString(keys[i]); }
            GameState.Instance.ResetProgress(); GameState.Instance.Save.highestUnlockedLevel=10; StoryService.ResetRuntime(); DemoFeatureState.SetLanguage("ru");
            GameState.Instance.Save.energy=0;GameState.Instance.Save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;GameState.Instance.Persist();
            router.Show(ScreenId.PreLevel);Settle();yield return null;yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (router != null) { router.Show(ScreenId.SplashTitle); Settle(); }
            StoryService.ResetRuntime();
            if (previousSave != null) GameState.Instance.Save = JsonUtility.FromJson<PlayerSaveData>(previousSave);
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (values != null)
            {
                for (int i = 0; i < keys.Length; i++) { PlayerPrefs.DeleteKey(keys[i]); if (existed[i]) PlayerPrefs.SetString(keys[i], values[i]); }
                PlayerPrefs.Save();
            }
            yield return null;
        }

        ProductionPreLevelView Preparation()=>router.Viewport.GetComponentInChildren<ProductionPreLevelView>();
        ProductionOutOfEnergyView View()=>router.Viewport.GetComponentInChildren<ProductionOutOfEnergyView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        void Open(){Preparation().PlayButton.onClick.Invoke();Settle();}
        static string StableSave()
        {
            var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);
        }
        [UnityTest]public IEnumerator EnergyGateUsesNativeModalAndCloseDoesNotSpendOrLeavePreparation()
        {
            int generation=router.RouteGeneration;Open();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
            Assert.That(View().ActionsBound&&View().HasRequiredBindings,Is.True);Assert.That(View().Environment.gameObject.activeSelf,Is.False);
            Assert.That(View().Count.text,Is.EqualTo("0"));Assert.That(GameState.Instance.Save.energy,Is.Zero);
            Preparation().PlayButton.onClick.Invoke();Preparation().BackButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));
            var close=View().Close;close.onClick.Invoke();close.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(GameState.Instance.Save.energy,Is.Zero);
            Open();yield return null;Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));
            Assert.That(router.Viewport.GetComponentsInChildren<ProductionOutOfEnergyView>().Length,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator ActualClockRussianEnglishAndPortraitSafeAreas()
        {
            var state=GameState.Instance;state.Save.lastEnergyUtcTicks=DateTime.UtcNow.AddMinutes(-GameState.EnergyRefillMinutes).AddSeconds(337).Ticks;state.Persist();
            Open();yield return null;yield return WaitForNormalButtonColors();View().RefreshState();
            Assert.That(View().Countdown.text,Does.StartWith("05:"));Assert.That(View().Count.text,Is.EqualTo("0"));string before=StableSave();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.True);Assert.That(View().Title.enabled,Is.False);
            Assert.That(View().Actions[1].image.sprite.name,Is.EqualTo("ask_ru_r2"));Assert.That(View().ActionLabels[1].enabled,Is.False);
            Capture("OutOfEnergy_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("OutOfEnergy_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("OutOfEnergy_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.False);Assert.That(View().Title.enabled,Is.True);
            Assert.That(View().ActionLabels[1].enabled,Is.True);Assert.That(View().Actions[1].image.sprite.name,Is.EqualTo("ask_blank_r2"));
            Capture("OutOfEnergy_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("OutOfEnergy_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(),Is.EqualTo(before));
        }
        [UnityTest]public IEnumerator RegeneratedEnergyAndFullCapacityUpdateWithoutAutomaticLevelStart()
        {
            Open();yield return null;var state=GameState.Instance;
            state.Save.lastEnergyUtcTicks=DateTime.UtcNow.AddMinutes(-GameState.EnergyRefillMinutes).AddSeconds(-2).Ticks;View().RefreshState();
            Assert.That(state.Save.energy,Is.EqualTo(1));Assert.That(SaveService.Load().energy,Is.EqualTo(1));Assert.That(View().Count.text,Is.EqualTo("1"));Assert.That(View().Title.text,Is.EqualTo("ЭНЕРГИЯ ЕСТЬ"));
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.False);Assert.That(View().Title.enabled,Is.True);
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));
            yield return WaitForNormalButtonColors();Capture("OutOfEnergy_Regenerated_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            state.Save.energy=state.Save.energyMax;state.Persist();View().RefreshState();Assert.That(View().Countdown.text,Is.EqualTo("—"));
            Capture("OutOfEnergy_Full_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            View().Close.onClick.Invoke();Settle();yield return null;Assert.That(state.Save.energy,Is.EqualTo(state.Save.energyMax));Assert.That(Preparation().PlayButton.IsInteractable(),Is.True);
        }
        [UnityTest]public IEnumerator UnavailableOffersAndRepeatedBindingCannotGrantOrCallServices()
        {
            Open();yield return null;int calls=0,closes=0;
            View().Bind(true,()=>closes++,()=>calls++,()=>calls++,()=>calls++);
            View().Bind(true,()=>closes++,()=>calls++,()=>calls++,()=>calls++);string before=StableSave();
            for(int i=0;i<3;i++){Assert.That(View().CanUseOffer(i),Is.False);Assert.That(View().Actions[i].IsInteractable(),Is.False);View().Actions[i].onClick.Invoke();View().Actions[i].onClick.Invoke();Assert.That(View().ActionLabels[i].text,Is.EqualTo("НЕДОСТУПНО"));}
            Assert.That(calls,Is.Zero);Assert.That(StableSave(),Is.EqualTo(before));
            View().Close.onClick.Invoke();View().Close.onClick.Invoke();Assert.That(closes,Is.EqualTo(1));yield return null;
        }
        [UnityTest]public IEnumerator ResourceHeaderKeepsStandaloneRouteAndCloseReturnsSafely()
        {
            router.Show(ScreenId.Shop);Settle();yield return null;var shop=router.Viewport.GetComponentInChildren<ProductionShopView>();
            int generation=router.RouteGeneration;shop.PlusButtons[2].onClick.Invoke();shop.PlusButtons[2].onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.OutOfEnergy));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));Assert.That(router.ActiveModal,Is.Null);Assert.That(View().Environment.gameObject.activeSelf,Is.True);
            yield return WaitForNormalButtonColors();Capture("OutOfEnergy_Standalone_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            var close=View().Close;close.onClick.Invoke();close.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+2));Assert.That(GameState.Instance.Save.energy,Is.Zero);
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
            string directory = Environment.GetEnvironmentVariable("LOR_OUT_OF_ENERGY_CAPTURE_DIR");
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
            var cameraGo = new GameObject("OutOfEnergyCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionOutOfEnergyView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionOutOfEnergyView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            if(v.Environment.gameObject.activeSelf)
            {
                var bg=v.Environment.rectTransform;Rect bounds=Bounds(bg,camera);
                Assert.That(bounds.xMin,Is.LessThanOrEqualTo(1));Assert.That(bounds.yMin,Is.LessThanOrEqualTo(1));
                Assert.That(bounds.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bounds.yMax,Is.GreaterThanOrEqualTo(size.y-1));
                Assert.That(bg.rect.width/bg.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001));
            }
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" text height");
                }
            var outsideHits=new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=new Vector2(size.x*.5f,size.y-10)},outsideHits);
            Assert.That(outsideHits.Count,Is.GreaterThan(0));
            Assert.That(outsideHits[0].gameObject.GetComponentInParent<ProductionOutOfEnergyView>()!=null || outsideHits[0].gameObject.name=="Modal_OutOfEnergy",Is.True,"The modal blocks the live page even outside the card.");
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
