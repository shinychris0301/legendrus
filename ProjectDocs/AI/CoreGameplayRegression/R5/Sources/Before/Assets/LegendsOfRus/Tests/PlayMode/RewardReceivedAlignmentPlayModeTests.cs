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
    public sealed class RewardReceivedAlignmentPlayModeTests
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
            router.Show(ScreenId.DailyReward);Settle();yield return null;yield return null;
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

        static string StableSave(PlayerSaveData save)
        {
            var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(save));
            copy.lastEnergyUtcTicks=0; // Normal energy-clock maintenance is unrelated to reward grants.
            return JsonUtility.ToJson(copy);
        }
        ProductionDailyRewardView Daily()=>router.Viewport.GetComponentInChildren<ProductionDailyRewardView>();
        ProductionRewardReceivedView View()=>Root().GetComponentInChildren<ProductionRewardReceivedView>();
        GameObject Root()
        {
            foreach(Transform t in router.Viewport)if(t.name=="Modal_RewardClaim"&&t.gameObject.activeInHierarchy)return t.gameObject;
            Assert.Fail("No active Reward Received modal");return null;
        }
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        void OpenEmpty()
        {
            var method=typeof(ScreenRouter).GetMethod("ShowModal",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            Assert.That((bool)method.Invoke(router,new object[]{ScreenId.RewardClaim}),Is.True);Settle();
        }
        [UnityTest]public IEnumerator DailyClaimOpensReceiptAndConfirmationNeverRegrants()
        {
            var state=GameState.Instance;int before=state.Save.gold;
            Daily().ClaimButton.onClick.Invoke();Daily().ClaimButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.RewardClaim));Assert.That(View().HasReceipt,Is.True);
            Assert.That(View().VisibleRewardCount,Is.EqualTo(1));Assert.That(View().Amounts[0].text,Is.EqualTo("500"));
            Assert.That(state.Save.gold,Is.EqualTo(before+500));Assert.That(SaveService.Load().gold,Is.EqualTo(state.Save.gold));
            string snapshot=StableSave(state.Save);
            int generation=router.RouteGeneration;Daily().BackButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation),"Underlying navigation stays blocked while the receipt is open.");
            yield return WaitForNormalButtonColors();
            Capture("RewardReceived_Day1_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            var confirm=View().Confirm;confirm.onClick.Invoke();confirm.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.DailyReward));Assert.That(StableSave(state.Save),Is.EqualTo(snapshot));
            OpenEmpty();yield return null;Assert.That(View().HasReceipt,Is.False);Assert.That(View().VisibleRewardCount,Is.Zero);
            Assert.That(View().Chest.gameObject.activeSelf||View().ChestGlow.gameObject.activeSelf||View().HeadingLettering.gameObject.activeSelf,Is.False);yield return WaitForNormalButtonColors();
            Capture("RewardReceived_NoReceipt_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            View().Confirm.onClick.Invoke();Settle();yield return null;Daily().ClaimButton.onClick.Invoke();
            Assert.That(router.ActiveModal,Is.Null);Assert.That(StableSave(state.Save),Is.EqualTo(snapshot));
        }
        [UnityTest]public IEnumerator DaySevenLiveReceiptFitsRussianEnglishAndPhoneSafeAreas()
        {
            var state=GameState.Instance;state.Save.lastDailyRewardUtc=DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");state.Save.dailyRewardCycleDay=6;state.Persist();Daily().RefreshContent();
            Daily().ClaimButton.onClick.Invoke();Settle();yield return null;yield return WaitForNormalButtonColors();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.True);Assert.That(View().Title.enabled,Is.False);Assert.That(View().ConfirmText.enabled,Is.False);
            Assert.That(View().ChestGlow.gameObject.activeSelf,Is.True);Assert.That(View().ChestGlow.transform.GetSiblingIndex(),Is.LessThan(View().Chest.transform.GetSiblingIndex()));
            Assert.That(View().VisibleRewardCount,Is.EqualTo(3));CollectionAssert.AreEqual(new[]{"1 000","40","1"},new[]{View().Amounts[0].text,View().Amounts[1].text,View().Amounts[2].text});
            string snapshot=StableSave(state.Save);
            Capture("RewardReceived_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("RewardReceived_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("RewardReceived_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().HeadingLettering.gameObject.activeSelf,Is.False);Assert.That(View().Title.enabled&&View().ConfirmText.enabled,Is.True);
            Capture("RewardReceived_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("RewardReceived_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(state.Save),Is.EqualTo(snapshot));
            // A route change retires this receipt; it cannot be resurrected over a later page.
            router.Show(ScreenId.Home);Settle();OpenEmpty();yield return null;Assert.That(View().HasReceipt,Is.False);
        }
        [UnityTest]public IEnumerator EnergyReceiptUsesActualCappedDelta()
        {
            var state=GameState.Instance;state.Save.lastDailyRewardUtc=DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");state.Save.dailyRewardCycleDay=2;state.Save.energy=state.Save.energyMax;state.Persist();Daily().RefreshContent();
            Daily().ClaimButton.onClick.Invoke();Settle();yield return null;
            Assert.That(View().VisibleRewardCount,Is.EqualTo(1));Assert.That(View().Amounts[0].text,Is.EqualTo("0"));Assert.That(View().Message.text,Does.Contain("полон"));
            Assert.That(state.Save.energy,Is.EqualTo(state.Save.energyMax));string snapshot=StableSave(state.Save);
            yield return WaitForNormalButtonColors();Capture("RewardReceived_EnergyCapped_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            View().Confirm.onClick.Invoke();Settle();yield return null;Assert.That(StableSave(state.Save),Is.EqualTo(snapshot));
            Assert.That(state.ClaimDailyReward(out var rejected),Is.False);Assert.That(rejected.Energy,Is.Zero);
        }
        [UnityTest]public IEnumerator FailedPersistenceDoesNotOpenOrInventAReceipt()
        {
            var state=GameState.Instance;string before=StableSave(state.Save);
            foreach(string key in keys)PlayerPrefs.DeleteKey(key);
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;PlayerPrefs.SetString(keys[0],JsonUtility.ToJson(future));PlayerPrefs.Save();
            Assert.That(SaveService.LoadWithResult().Status,Is.EqualTo(PlayerSaveLoadStatus.UnsupportedFutureVersion));
            Daily().ClaimButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(StableSave(state.Save),Is.EqualTo(before));
            Assert.That(state.ClaimDailyReward(out var rejected),Is.False);Assert.That(rejected.Gold,Is.Zero);
        }
        [UnityTest]public IEnumerator SevenTypePresentationAndRepeatedBindingDoNotAwardAnything()
        {
            OpenEmpty();yield return null;string before=StableSave(GameState.Instance.Save);int closed=0;
            var fixture=new DailyRewardGrant(5000,250,30,3,5,1,2);
            View().Bind(fixture,0,()=>closed++);View().Bind(fixture,0,()=>closed++);
            Assert.That(View().VisibleRewardCount,Is.EqualTo(7));yield return WaitForNormalButtonColors();
            Capture("Diagnostic_RewardReceived_SevenTypes_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            View().Confirm.onClick.Invoke();View().Confirm.onClick.Invoke();Assert.That(closed,Is.EqualTo(1));Assert.That(StableSave(GameState.Instance.Save),Is.EqualTo(before));
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
            string directory = Environment.GetEnvironmentVariable("LOR_REWARD_RECEIVED_CAPTURE_DIR");
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
            var cameraGo = new GameObject("RewardReceivedCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionRewardReceivedView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionRewardReceivedView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
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
            for(int i=0;i<v.VisibleRewardCount;i++)
            {
                var icon=v.Icons[i].rectTransform;var label=v.Names[i].rectTransform;
                Assert.That(icon.anchoredPosition.x+icon.rect.width/2,Is.EqualTo(61).Within(.01f),"Reward icon centered inside card");
                Assert.That(-label.anchoredPosition.y+label.rect.height,Is.LessThanOrEqualTo(121),"Name clears lower gold rail");
            }
            var outsideHits=new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=new Vector2(size.x*.5f,size.y-10)},outsideHits);
            Assert.That(outsideHits.Count,Is.GreaterThan(0));
            Assert.That(outsideHits[0].gameObject.GetComponentInParent<ProductionRewardReceivedView>()!=null || outsideHits[0].gameObject.name=="Modal_RewardClaim",Is.True,"The modal blocks the live page even outside the card.");
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
