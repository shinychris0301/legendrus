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
    public sealed class ShopAlignmentPlayModeTests
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
            router.Show(ScreenId.Shop);Settle();yield return null;yield return null;
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

        ProductionShopView View()=>router.Viewport.GetComponentInChildren<ProductionShopView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveHudLocalizationAndResponsiveCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(View().Header.HasBindings,Is.True);
            Assert.That(View().Cards[0].Find("OriginalProductName").gameObject.activeSelf,Is.True);
            Assert.That(View().ProductNames[0].enabled,Is.False);
            Assert.That(router.Viewport.GetComponentInChildren<AnimatedHotspot>(),Is.Null);
            int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=JsonUtility.ToJson(GameState.Instance.Save);yield return WaitForNormalButtonColors();
            Capture("Shop_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Shop_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Shop_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().ProductNames[0].enabled,Is.True);
            foreach(var nav in View().NavButtons)
            {
                var plaque=nav.transform.Find("LabelPlaque").GetComponent<Image>();
                Assert.That(plaque.sprite.rect.width/plaque.sprite.rect.height,Is.GreaterThan(3.5f),"English caption must use a wide plaque, not the category tab.");
            }
            Assert.That(View().Cards[0].Find("OriginalProductName").gameObject.activeSelf,Is.False);
            Capture("Shop_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Shop_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            AssertUnchangedSave(save);
        }
        [UnityTest]public IEnumerator CategoriesAndUnverifiedPurchaseControlsAreTruthful()
        {
            string save=JsonUtility.ToJson(GameState.Instance.Save);var v=View();int purchaseRequests=0;
            v.Bind(id=>{},()=>{},id=>purchaseRequests++);v.Bind(id=>{},()=>{},id=>purchaseRequests++);
            for(int i=0;i<4;i++){Assert.That(v.CanPurchase(i),Is.False);Assert.That(v.PurchaseButtons[i].interactable,Is.False);v.PurchaseButtons[i].onClick.Invoke();}
            Assert.That(purchaseRequests,Is.Zero);
            v.Tabs[0].onClick.Invoke();Assert.That(v.SelectedCategory,Is.EqualTo("coins"));Assert.That(v.Cards[0].gameObject.activeSelf&&v.Cards[2].gameObject.activeSelf,Is.True);Assert.That(v.Cards[1].gameObject.activeSelf,Is.False);
            yield return WaitForNormalButtonColors();Capture("Shop_Coins_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.NavButtons[1].onClick.Invoke();Assert.That(v.EmptyPanel.activeSelf,Is.True);Assert.That(v.SelectedCategory,Is.EqualTo("gems"));
            yield return WaitForNormalButtonColors();Capture("Shop_GemsUnavailable_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.NavButtons[0].onClick.Invoke();Assert.That(v.EmptyPanel.activeSelf,Is.False);foreach(var card in v.Cards)Assert.That(card.gameObject.activeSelf,Is.True);
            AssertUnchangedSave(save);
        }
        [UnityTest]public IEnumerator NativeNavigationRoutesOnceAndKeepsHomeReturn()
        {
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.SpecialOffer,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++)
            {
                router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Shop);Settle();yield return null;
                var v=View();var b=i==0?v.ProfileButton:i==1?v.SettingsButton:i==2?v.SpecialButton:v.PlusButtons[2];int generation=router.RouteGeneration;
                b.onClick.Invoke();b.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
            router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Shop);Settle();yield return null;
            View().BackButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
        }
        static void AssertUnchangedSave(string snapshot)
        {
            var before=JsonUtility.FromJson<PlayerSaveData>(snapshot);
            var after=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            // GameState refreshes this clock while energy is full. Every other saved field must match.
            if(before.energy>=before.energyMax && after.energy==before.energy)
            {
                Assert.That(after.lastEnergyUtcTicks,Is.GreaterThanOrEqualTo(before.lastEnergyUtcTicks));
                after.lastEnergyUtcTicks=before.lastEnergyUtcTicks;
            }
            Assert.That(JsonUtility.ToJson(after),Is.EqualTo(snapshot));
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
            string directory = Environment.GetEnvironmentVariable("LOR_SHOP_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ChapterCompleteCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionShopView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionShopView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f),button.name+" left safe edge");
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f),button.name+" bottom safe edge");
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f),button.name+" right safe edge");
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.5f),button.name+" top safe edge");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" text height");
                }
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
