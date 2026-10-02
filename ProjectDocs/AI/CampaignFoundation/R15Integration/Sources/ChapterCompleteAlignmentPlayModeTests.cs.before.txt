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
    public sealed class ChapterCompleteAlignmentPlayModeTests
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
            router.Show(ScreenId.ChapterComplete);Settle();yield return null;yield return null;
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

        ProductionChapterCompleteView View()=>router.Viewport.GetComponentInChildren<ProductionChapterCompleteView>();
        GameObject Root()=>View().gameObject;
        void CompleteChapter()
        {
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)GameState.Instance.CompleteLevel(level,level.starScoreThresholds[2]);
            foreach(var m in StoryCatalog.LoadChapter1().milestones)GameState.Instance.CompleteStoryScene(m.sceneId);
            StoryService.ResetRuntime();router.Show(ScreenId.ChapterComplete);Settle();
        }
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveCompletionRewardsLocalizationAndResponsiveCaptures()
        {
            CompleteChapter();yield return null;yield return WaitForNormalButtonColors();
            string save=JsonUtility.ToJson(GameState.Instance.Save);
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(View().Summary.text,Does.Contain("10/10").And.Contain("30/30"));
            CollectionAssert.AreEqual(new[]{1850,40,2,2,2,2},ProductionChapterCompleteView.ReceivedRewards(GameState.Instance));
            Assert.That(View().RewardCounts[0].text,Is.EqualTo("1 850"));
            Assert.That(View().Body.Find("NextHeadingOriginal").gameObject.activeSelf,Is.True);
            Assert.That(View().NextHeading.text,Is.EqualTo("СКОРО"));
            Capture("ChapterComplete_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("ChapterComplete_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("ChapterComplete_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().HeaderText.enabled,Is.True);Assert.That(View().NextHeading.text,Does.Contain("COMING SOON"));
            Assert.That(View().Body.Find("NextHeadingOriginal").gameObject.activeSelf,Is.False);
            Assert.That(View().Banner.sprite.name,Is.EqualTo("banner_blank_r8"));
            Assert.That(View().Teaser.sprite.name,Is.EqualTo("teaser_blank_r8"));
            Capture("ChapterComplete_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("ChapterComplete_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save),"Showing/localizing the summary cannot award rewards or acknowledge completion.");
            router.Show(ScreenId.ChapterComplete);Settle();yield return null;
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator NativeActionsAcknowledgeExactlyOnceWithoutRewardDuplication()
        {
            for(int i=0;i<2;i++){
                GameState.Instance.ResetProgress();CompleteChapter();yield return null;yield return WaitForNormalButtonColors();
                var state=GameState.Instance;int gold=state.Save.gold,gems=state.Save.gems;
                var button=View().Actions[i];int generation=router.RouteGeneration;
                button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
                Assert.That(state.GetChapterCompletionPhase(1),Is.EqualTo(ChapterCompletionPhase.Acknowledged));
                Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.gems,Is.EqualTo(gems));
            }
        }
        [UnityTest]public IEnumerator IncompleteChapterNeverShowsSampleRewardsOrFalseVictory()
        {
            Assert.That(View().Foreground.gameObject.activeSelf,Is.False);Assert.That(View().Actions[0].interactable,Is.False);
            Assert.That(View().Summary.text,Does.Contain("0/10").And.Contain("0/30"));
            foreach(var t in View().RewardCounts)Assert.That(t.text,Is.EqualTo("0"));
            yield return WaitForNormalButtonColors();Capture("ChapterComplete_InProgress_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            var level=ChapterLevelCatalog.LoadChapter1().levels[0];GameState.Instance.CompleteLevel(level,level.starScoreThresholds[0]);yield return null;
            Assert.That(View().Summary.text,Does.Contain("1/10").And.Contain("1/30"));Assert.That(View().RewardCounts[0].text,Is.EqualTo("120"));
            View().Actions[1].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
            Assert.That(GameState.Instance.GetChapterCompletionPhase(1),Is.EqualTo(ChapterCompletionPhase.InProgress));
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
            string directory = Environment.GetEnvironmentVariable("LOR_CHAPTER_COMPLETE_CAPTURE_DIR");
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionChapterCompleteView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionChapterCompleteView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var button in v.Actions)
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                var sprite=button.GetComponent<Image>().sprite;Assert.That(rect.width/rect.height,Is.EqualTo(sprite.rect.width/sprite.rect.height).Within(.002f));
            }
            for(int i=0;i<1;i++)Assert.That(Bounds((RectTransform)v.Actions[i].transform,camera).Overlaps(Bounds((RectTransform)v.Actions[i+1].transform,camera)),Is.False);
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.name+" text height");
                }
            foreach(var icon in v.RewardIcons)
            {
                var card=Bounds((RectTransform)icon.transform.parent,camera);
                Assert.That(card.Overlaps(Bounds((RectTransform)v.Actions[0].transform,camera)),Is.False,"Reward card overlaps Continue");
                Assert.That(card.Overlaps(Bounds((RectTransform)v.Actions[1].transform,camera)),Is.False,"Reward card overlaps World Map");
                Assert.That(Bounds(icon.rectTransform,camera).Overlaps(Bounds(icon.transform.parent.Find("Received").GetComponent<RectTransform>(),camera)),Is.False,"Reward amount overlaps icon");
            }
            Assert.That(Bounds(v.Summary.rectTransform,camera).Overlaps(Bounds((RectTransform)v.Actions[0].transform,camera)),Is.False,"Summary overlaps actions");
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
