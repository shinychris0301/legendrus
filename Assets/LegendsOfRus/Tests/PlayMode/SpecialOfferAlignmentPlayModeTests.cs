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
    public sealed class SpecialOfferAlignmentPlayModeTests
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
            router.Show(ScreenId.Shop);Settle();router.Show(ScreenId.SpecialOffer);Settle();yield return null;yield return null;
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

        ProductionSpecialOfferView View()=>router.Viewport.GetComponentInChildren<ProductionSpecialOfferView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveBalancesLocalizationAndSafeAreaCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(View().PlayerHeader.HasBindings,Is.True);
            Assert.That(View().Title.enabled,Is.False);
            Assert.That(View().Header.isActiveAndEnabled,Is.True);
            Assert.That(View().Header.sprite.name,Is.EqualTo("header_ru_r3"));
            Assert.That(View().PurchaseButton.GetComponent<Image>().sprite.name,Is.EqualTo("buy_lettered_r3"));
            Assert.That(View().PurchaseText.enabled,Is.False);
            Assert.That(View().ShopButton.gameObject.activeSelf,Is.False);
            Assert.That(View().PlayerHeader.Capsules[2].gameObject.activeInHierarchy,Is.True);
            GameState.Instance.Save.energy=7;GameState.Instance.Save.energyMax=30;View().RefreshState();
            Assert.That(View().EnergyText.text,Is.EqualTo("7/30"));
            Assert.That(View().Hud.GetComponent<CanvasGroup>().blocksRaycasts,Is.False);
            Assert.That(router.Viewport.GetComponentInChildren<AnimatedHotspot>(),Is.Null);
            Assert.That(router.Viewport.GetComponentsInChildren<ProductionSpecialOfferView>().Length,Is.EqualTo(1));
            int gold=GameState.Instance.Save.gold;GameState.Instance.AddGold(23);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((gold+23).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=JsonUtility.ToJson(GameState.Instance.Save);yield return WaitForNormalButtonColors();
            Capture("SpecialOffer_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("SpecialOffer_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("SpecialOffer_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().Title.enabled,Is.True);
            Assert.That(View().Header.isActiveAndEnabled,Is.True);
            Assert.That(View().Header.sprite.name,Is.EqualTo("header_blank_r3"));
            Assert.That(View().PurchaseText.enabled,Is.True);
            Assert.That(View().ShopButton.gameObject.activeSelf,Is.False);
            Assert.That(View().PlayerHeader.Capsules[2].gameObject.activeInHierarchy,Is.True);
            Assert.That(View().EnergyText.text,Is.EqualTo("7/30"));
            Assert.That(View().PurchaseText.text,Is.EqualTo("BUY NOW"));
            Assert.That(View().Title.text.Replace("\n"," "),Is.EqualTo("SPECIAL OFFER"));
            Capture("SpecialOffer_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("SpecialOffer_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator UnavailableOfferCannotPurchaseOrGrantThroughRepeatedBindings()
        {
            var v=View();int calls=0;string save=JsonUtility.ToJson(GameState.Instance.Save);
            v.Bind(()=>{},()=>{},id=>calls++);v.Bind(()=>{},()=>{},id=>calls++);
            Assert.That(v.CanPurchase,Is.False);Assert.That(v.PurchaseButton.interactable,Is.False);
            v.PurchaseButton.onClick.Invoke();v.PurchaseButton.onClick.Invoke();yield return null;
            Assert.That(calls,Is.Zero);foreach(var amount in v.RewardAmounts)Assert.That(amount.text,Is.EqualTo("—"));
            foreach(var t in v.GetComponentsInChildren<Text>()){Assert.That(t.text,Does.Not.Contain("₽"));Assert.That(t.text,Does.Not.Contain("%"));Assert.That(t.text,Does.Not.Contain("02:48"));}
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator ShopEntryAndCloseNavigateExactlyOnce()
        {
            for(int i=0;i<2;i++)
            {
                router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Shop);Settle();yield return null;
                var shop=router.Viewport.GetComponentInChildren<ProductionShopView>();shop.SpecialButton.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.SpecialOffer));Assert.That(View().ActionsBound,Is.True);
                var b=View().BackButton;int generation=router.RouteGeneration;
                b.onClick.Invoke();b.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Shop));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
                Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().HasRequiredBindings,Is.True);
            }
        }
        IEnumerator WaitForNormalButtonColors()
        {
            float deadline=Time.realtimeSinceStartup+2f;
            int stable=0;
            do
            {
                yield return null;
                bool ready=true;
                foreach(Button button in new[]{View().BackButton,View().PurchaseButton})
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
            string directory = Environment.GetEnvironmentVariable("LOR_SPECIAL_OFFER_CAPTURE_DIR");
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionSpecialOfferView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionSpecialOfferView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var button in new[]{v.BackButton,v.PurchaseButton})
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            Assert.That(v.PlayerHeader.Capsules[2].gameObject.activeSelf,Is.True);
            var blocked=new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=Bounds(v.Hud,camera).center},blocked);
            Assert.That(blocked.Exists(hit=>hit.gameObject.GetComponentInParent<ProductionPlayerHeader>()!=null),Is.False,"Dimmed background header must not receive input.");
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" text height");
                }
            foreach(var img in new[]{v.Hero,v.OfferCard,v.PurchaseButton.GetComponent<Image>()})
            {
                var rect=Bounds(img.rectTransform,camera);Assert.That(rect.width/rect.height,Is.EqualTo(img.sprite.rect.width/img.sprite.rect.height).Within(.002f),img.name+" aspect");
            }
            foreach(var icon in v.RewardIcons)Assert.That(Bounds(icon.rectTransform,camera).Overlaps(Bounds(v.PurchaseButton.GetComponent<RectTransform>(),camera)),Is.False);
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
