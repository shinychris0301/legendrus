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
    public sealed class BoosterSelectionAlignmentPlayModeTests
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
            GameState.Instance.Save.energy=5;GameState.Instance.Save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;GameState.Instance.Persist();
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
        ProductionBoosterSelectionView View()=>router.Viewport.GetComponentInChildren<ProductionBoosterSelectionView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        void Open(){Preparation().BoosterButtons[0].onClick.Invoke();Preparation().BoosterCatalog.onClick.Invoke();Settle();}
        static string StableSave()
        {
            var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);
        }
        [UnityTest]public IEnumerator GuideEntryInspectionCloseAndReopenPreserveInventory()
        {
            string before=StableSave();int generation=router.RouteGeneration;Open();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.BoosterSelect));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
            Assert.That(View().ActionsBound&&View().HasRequiredBindings,Is.True);Assert.That(View().Environment.gameObject.activeSelf,Is.False);
            for(int i=0;i<4;i++){View().Tools[i].onClick.Invoke();Assert.That(View().InspectedIndex,Is.EqualTo(i));Assert.That(View().Counts[i].text,Is.EqualTo(GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));}
            Preparation().PlayButton.onClick.Invoke();Preparation().BackButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));
            Assert.That(StableSave(),Is.EqualTo(before));var close=View().Close;close.onClick.Invoke();close.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(Preparation().BoosterDetailsRoot.gameObject.activeSelf,Is.True);
            Preparation().BoosterCatalog.onClick.Invoke();Settle();yield return null;
            Assert.That(router.Viewport.GetComponentsInChildren<ProductionBoosterSelectionView>().Length,Is.EqualTo(1));Assert.That(View().InspectedIndex,Is.EqualTo(-1));Assert.That(StableSave(),Is.EqualTo(before));
        }
        [UnityTest]public IEnumerator LiveInventoryRussianEnglishAndPortraitSafeAreas()
        {
            Open();yield return null;var state=GameState.Instance;
            for(int i=0;i<4;i++)Assert.That(View().Counts[i].text,Is.EqualTo(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));
            state.AddBooster(BoosterType.BogatyrMace,5);View().RefreshState();Assert.That(View().Counts[0].text,Is.EqualTo(state.BoosterCount(BoosterType.BogatyrMace).ToString()));
            yield return WaitForNormalButtonColors();string before=StableSave();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.True);Assert.That(View().Title.enabled,Is.False);
            Assert.That(View().Play.image.sprite.name,Is.EqualTo("play_ru_r2"));Assert.That(View().PlayText.enabled,Is.False);
            foreach(var label in View().PreviewLabels)Assert.That(label.text,Does.Contain("ПРЕДПРОСМОТР"));
            Capture("BoosterSelection_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("BoosterSelection_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("BoosterSelection_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.False);Assert.That(View().Title.enabled,Is.True);
            Assert.That(View().Play.image.sprite.name,Is.EqualTo("play_blank_r2"));Assert.That(View().PlayText.enabled,Is.True);
            foreach(var label in View().PreviewLabels)Assert.That(label.text,Does.Contain("PREVIEW"));
            Capture("BoosterSelection_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("BoosterSelection_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(),Is.EqualTo(before));Assert.That(View().Effects[3].text,Does.Contain("5 moves"));
        }
        [UnityTest]public IEnumerator PlaySpendsOneEnergyAndNoBoosters()
        {
            Open();yield return null;int energy=GameState.Instance.Save.energy;int[] stock=new int[4];for(int i=0;i<4;i++)stock[i]=GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]);
            var play=View().Play;play.onClick.Invoke();play.onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(GameState.Instance.Save.energy,Is.EqualTo(energy-1));
            for(int i=0;i<4;i++)Assert.That(GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]),Is.EqualTo(stock[i]));
        }
        [UnityTest]public IEnumerator NoEnergyOpensExistingGateAndAllowsReturnToGuide()
        {
            GameState.Instance.Save.energy=0;GameState.Instance.Persist();Open();yield return null;
            View().Play.onClick.Invoke();Settle();yield return null;Settle();
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(View(),Is.Null);
            router.Viewport.GetComponentInChildren<ProductionOutOfEnergyView>().Close.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Preparation().BoosterCatalog.onClick.Invoke();Settle();yield return null;Assert.That(View().ActionsBound,Is.True);
        }
        [UnityTest]public IEnumerator EmptyStockCanBeInspectedButNeverEquippedOrConsumed()
        {
            var state=GameState.Instance;for(int i=0;i<4;i++){var type=ProductionPreLevelRuntime.Boosters[i];while(state.BoosterCount(type)>0)state.TryUseBooster(type);}
            Open();yield return null;string before=StableSave();
            for(int i=0;i<4;i++){Assert.That(View().Counts[i].text,Is.EqualTo("0"));Assert.That(View().Availability[i].text,Is.EqualTo("НЕТ В ЗАПАСЕ"));View().Tools[i].onClick.Invoke();Assert.That(View().InspectedIndex,Is.EqualTo(i));}
            Assert.That(StableSave(),Is.EqualTo(before));yield return WaitForNormalButtonColors();Capture("BoosterSelection_Empty_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [UnityTest]public IEnumerator RepeatedBindingInvokesPlayOnlyOnce()
        {
            Open();yield return null;int plays=0;View().Bind(true,()=>{},()=>{plays++;return true;});View().Bind(true,()=>{},()=>{plays++;return true;});
            View().Play.onClick.Invoke();View().Play.onClick.Invoke();Assert.That(plays,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator StandaloneRouteHasNativeBackgroundAndBack()
        {
            router.Show(ScreenId.BoosterSelect);Settle();yield return null;Assert.That(View().Environment.gameObject.activeSelf,Is.True);
            yield return WaitForNormalButtonColors();Capture("BoosterSelection_Standalone_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            View().Close.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
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
            string directory = Environment.GetEnvironmentVariable("LOR_BOOSTER_SELECTION_CAPTURE_DIR");
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
            var cameraGo = new GameObject("BoosterSelectionCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionBoosterSelectionView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionBoosterSelectionView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            if(v.Environment.gameObject.activeSelf)
            {
                var bg=v.Environment.rectTransform;Rect bounds=Bounds(bg,camera);
                Assert.That(bounds.xMin,Is.LessThanOrEqualTo(1));Assert.That(bounds.yMin,Is.LessThanOrEqualTo(1));
                Assert.That(bounds.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bounds.yMax,Is.GreaterThanOrEqualTo(size.y-1));
                Assert.That(bg.rect.width/bg.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001));
            }
            for(int i=0;i<4;i++)
            {
                Rect explanation=Bounds(v.Effects[i].rectTransform,camera),nextIcon=Bounds(v.PreviewIcons[i].rectTransform,camera);
                Assert.That(explanation.yMin,Is.GreaterThan(nextIcon.yMax),"Rows must not overlap.");
                Assert.That(v.PreviewIcons[i].raycastTarget,Is.False);
                Assert.That(v.PreviewIcons[i].GetComponent<Button>(),Is.Null,"Previews must not pretend to equip or purchase.");
                Assert.That(v.PreviewIcons[i].GetComponentsInChildren<Text>(),Is.Empty,"Previews do not advertise inventory counts.");
            }
            Assert.That(v.Play.image.rectTransform.anchoredPosition.y-v.Play.image.rectTransform.rect.height,Is.GreaterThan(-v.Body.rect.height+50),"Play must leave the lower ornament visible.");
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
            Assert.That(outsideHits[0].gameObject.GetComponentInParent<ProductionBoosterSelectionView>()!=null || outsideHits[0].gameObject.name=="Modal_BoosterSelect",Is.True,"The modal blocks the live page even outside the card.");
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
