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
    public sealed class PauseAlignmentPlayModeTests
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
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay); Settle();
            yield return null; yield return null;
            yield return null;
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

        ProductionGameplayView Gameplay()=>router.Viewport.GetComponentInChildren<ProductionGameplayView>();
        ProductionPauseView View()=>Root().GetComponentInChildren<ProductionPauseView>();
        GameObject Root()
        {
            foreach(Transform t in router.Viewport)
                if(t.name=="Modal_Pause" && t.gameObject.activeInHierarchy)return t.gameObject;
            Assert.Fail("No active Pause modal.");return null;
        }
        void OpenPause()
        {
            Gameplay().PauseButton.onClick.Invoke();Settle();
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));
            Assert.That(View().Board,Is.SameAs(Gameplay().Board));
        }
        [UnityTest] public IEnumerator ReferenceAndTallLayoutsKeepLiveRoundAndLocalizedControls()
        {
            var board=Gameplay().Board;string hash=board.BoardStateHash;int moves=board.Moves,score=board.Score;
            OpenPause();yield return WaitForNormalButtonColors();
            Assert.That(board.CanAcceptPointerInput,Is.False);Assert.That(View().ActionsBound,Is.True);
            for(int i=0;i<board.GoalCount;i++)Assert.That(View().Goals[i].Count.text,Is.EqualTo(board.GetGoalSlot(i).Remaining.ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            for(int i=0;i<4;i++)Assert.That(View().BoosterCounts[i].text,Is.EqualTo(GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));
            Assert.That(View().Title.enabled,Is.False);foreach(var text in View().ActionLabels)Assert.That(text.enabled,Is.False);
            VerifyCaptionLayers(View(),false);
            Capture("Pause_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Pause_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Pause_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().Title.enabled,Is.True);foreach(var text in View().ActionLabels)Assert.That(text.enabled,Is.True);
            VerifyCaptionLayers(View(),true);
            Capture("Pause_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");yield return null;Assert.That(View().Title.enabled,Is.False);
            Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.Score,Is.EqualTo(score));
            View().Actions[0].onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(Gameplay().Board,Is.SameAs(board));Assert.That(board.CanAcceptPointerInput,Is.True);
            Assert.That(board.BoardStateHash,Is.EqualTo(hash));
        }
        [UnityTest] public IEnumerator ThreeGoalsAndSettingsReturnPreserveThePausedRound()
        {
            GameState.Instance.Save.selectedLevel=10;router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
            var board=Gameplay().Board;string hash=board.BoardStateHash;OpenPause();yield return WaitForNormalButtonColors();
            Assert.That(board.GoalCount,Is.EqualTo(3));
            Capture("Pause_ThreeGoals_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");yield return null;
            Capture("Pause_ThreeGoals_576_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            View().Actions[2].onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Settings));Assert.That(board.CanAcceptPointerInput,Is.False);
            var settings=router.Viewport.GetComponentInChildren<ProductionSettingsView>();
            Assert.That(settings,Is.Not.Null);settings.BackButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(View().Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));
            View().Actions[0].onClick.Invoke();Settle();yield return null;Assert.That(board.CanAcceptPointerInput,Is.True);
        }
        [UnityTest] public IEnumerator RestartAndExitUseExistingRoutesAndPreventDoubleActions()
        {
            var board=Gameplay().Board;OpenPause();yield return WaitForNormalButtonColors();
            var v=View();v.Actions[1].onClick.Invoke();v.Actions[3].onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(router.ActiveModal,Is.Null);
            Assert.That(Gameplay().Board,Is.Not.SameAs(board));Assert.That(Gameplay().Board.Score,Is.Zero);
            OpenPause();yield return WaitForNormalButtonColors();View().Actions[3].onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));Assert.That(router.ActiveModal,Is.Null);
        }
        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            Canvas.ForceUpdateCanvases();
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
            string directory = Environment.GetEnvironmentVariable("LOR_PAUSE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("PauseCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionPauseView>(),camera,size,safeArea);
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
        static void VerifyCaptionLayers(ProductionPauseView v,bool english)
        {
            Assert.That(v.GoalsHeading.enabled,Is.EqualTo(english));
            Assert.That(v.BoostersHeading.enabled,Is.EqualTo(english));
            Assert.That(v.Instructions.enabled,Is.EqualTo(english));
            Assert.That(v.Body.Find("InstructionsArtwork").gameObject.activeSelf,Is.EqualTo(!english));
            Assert.That(v.Body.Find("GoalsCaption").GetComponent<Image>().sprite.name,Is.EqualTo(english?"goals_flourishes_r2":"goals_heading_ru_r2"));
            Assert.That(v.Body.Find("BoostersCaption").GetComponent<Image>().sprite.name,Is.EqualTo(english?"boosters_flourishes_r2":"boosters_heading_ru_r2"));
            foreach(var label in v.BoosterCounts)Assert.That(label.enabled,Is.True,"Stock stays live in every locale.");
        }
        static void VerifyLayoutAndInput(ProductionPauseView v,Camera camera,Vector2Int size,Rect safeArea)
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
            for(int i=0;i<3;i++)Assert.That(Bounds((RectTransform)v.Actions[i].transform,camera).Overlaps(Bounds((RectTransform)v.Actions[i+1].transform,camera)),Is.False);
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.name+" text height");
                }
            foreach(var goal in v.Goals)if(goal.Root.gameObject.activeSelf)
                Assert.That(Bounds(goal.Icon.rectTransform,camera).Overlaps(Bounds(goal.Count.rectTransform,camera)),Is.False);
            var panel=Bounds(v.Body.Find("MainPanel") as RectTransform,camera);
            foreach(var label in new[]{v.GoalsHeading,v.BoostersHeading,v.Instructions})
                if(label.enabled)
                {
                    var rect=Bounds(label.rectTransform,camera);
                    Assert.That(panel.Contains(rect.min)&&panel.Contains(rect.max),Is.True,label.name+" must stay inside the panel at its actual transform scale.");
                    foreach(var goal in v.Goals)if(goal.Root.gameObject.activeSelf)
                        Assert.That(rect.Overlaps(Bounds(goal.Icon.rectTransform,camera))||rect.Overlaps(Bounds(goal.Count.rectTransform,camera)),Is.False,label.name+" must clear the live goals.");
                    foreach(var booster in v.BoosterArt)Assert.That(rect.Overlaps(Bounds(booster.rectTransform,camera)),Is.False,label.name+" must clear booster cards.");
                }

            var outsideHits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=new Vector2(size.x*.05f,size.y*.5f)},outsideHits);
            Assert.That(outsideHits.Count,Is.GreaterThan(0));Assert.That(outsideHits[0].gameObject.GetComponentInParent<Match3Board>(),Is.Null,"Modal must block gameplay input");
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
