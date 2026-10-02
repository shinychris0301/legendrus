using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class GameplayCompletenessAuditPlayModeTests
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
            GameState.Instance.ResetProgress(); StoryService.ResetRuntime(); DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.Gameplay); Settle();
            yield return null; yield return null;
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

        // Audit-only: exercise the unchanged routed page and record omitted visual states.
        [UnityTest]
        public IEnumerator CaptureUnreviewedGameplayStates()
        {
            var tutorial=Root().GetComponentInChildren<LevelTutorialPresenter>();
            Assert.That(tutorial.IsPresenting,Is.True);
            CaptureAudit("01_FirstEntryTutorial");
            while(tutorial.IsPresenting){tutorial.Advance();yield return null;}
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
            yield return WaitForNormalButtonColors();
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
            CaptureAudit("02_Idle");
            Assert.That(board.TapCell(0,0),Is.True);
            Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Selected));
            CaptureAudit("03_SelectedTopLeftGem");
            Assert.That(board.TapCell(0,0),Is.True);
            view.PauseButton.onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));
            Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Paused));
            CaptureAudit("04_Pause");
            AnimatedHotspot resume=null;
            foreach(var hotspot in router.GameCanvas.GetComponentsInChildren<AnimatedHotspot>())
                if(hotspot.name=="Hotspot_Resume"&&hotspot.isActiveAndEnabled)resume=hotspot;
            Assert.That(resume,Is.Not.Null);
            var pointer=new PointerEventData(EventSystem.current){pointerId=807};
            resume.OnPointerDown(pointer);resume.OnPointerUp(pointer);resume.OnPointerClick(pointer);
            Settle();yield return null;yield return null;
            Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.LoadQAPreset("booster.firebird"),Is.True);
            BoardQAPresetFactory.TryGet("booster.firebird",out BoardQAPreset preset);
            view.BoosterButtons[1].onClick.Invoke();
            Assert.That(board.ArmedBooster,Is.EqualTo(BoosterType.Firebird));
            CaptureAudit("05_FirebirdTargeting_QAPreset");
            Assert.That(board.TapCell(preset.Primary.X,preset.Primary.Y),Is.True);
            Assert.That(board.HasActiveCommand,Is.True);
            Assert.That(board.VfxPresenter.HasActiveCue(CoreLoopVfxCue.BoosterFirebird),Is.True);
            board.VfxPresenter.TickForTesting(.06f);
            CaptureAudit("06_FirebirdActivation_QAPreset");
            float deadline=Time.realtimeSinceStartup+12f;
            while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(board.HasActiveCommand,Is.False);
        }

        [UnityTest]
        public IEnumerator CaptureAllBoosterModesAndCompletion()
        {
            yield return PrepareIdle();
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
            string[] presets={"booster.mace-row","booster.mace-column","booster.firebird","booster.living-light","booster.firebird"};
            string[] names={"MaceRow","MaceColumn","Firebird","LivingLight","SeaPearl"};
            BoosterType[] kinds={BoosterType.BogatyrMace,BoosterType.BogatyrMace,BoosterType.Firebird,BoosterType.LivingLight,BoosterType.SeaPearl};
            int[] buttons={0,0,1,2,3};
            CoreLoopVfxCue[] cues={CoreLoopVfxCue.BoosterMaceRow,CoreLoopVfxCue.BoosterMaceColumn,CoreLoopVfxCue.BoosterFirebird,CoreLoopVfxCue.BoosterLivingLight,CoreLoopVfxCue.BoosterSeaPearl};
            for(int i=0;i<names.Length;i++)
            {
                Assert.That(board.LoadQAPreset(presets[i]),Is.True);
                Assert.That(BoardQAPresetFactory.TryGet(presets[i],out BoardQAPreset preset),Is.True);
                GameState.Instance.AddBooster(kinds[i],2);
                yield return WaitForNormalButtonColors();
                int stock=GameState.Instance.BoosterCount(kinds[i]);int moves=board.Moves;
                view.BoosterButtons[buttons[i]].onClick.Invoke();
                if(i<4)
                {
                    Assert.That(board.ArmedBooster,Is.EqualTo(kinds[i]));
                    CaptureAudit("10_"+names[i]+"_Armed_QAPreset");
                    if(i==1)Assert.That(board.SwipeCell(preset.Primary.X,preset.Primary.Y,Vector2Int.up),Is.True);
                    else Assert.That(board.TapCell(preset.Primary.X,preset.Primary.Y),Is.True);
                }
                Assert.That(board.HasActiveCommand,Is.True);
                Assert.That(board.VfxPresenter.HasActiveCue(cues[i]),Is.True);
                board.VfxPresenter.TickForTesting(.06f);
                CaptureAudit("11_"+names[i]+"_Activation_QAPreset");
                float deadline=Time.realtimeSinceStartup+12f;
                while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(board.HasActiveCommand,Is.False,names[i]);
                Assert.That(GameState.Instance.BoosterCount(kinds[i]),Is.EqualTo(stock-1),names[i]);
                if(i==4)Assert.That(board.Moves,Is.EqualTo(moves+5));
            }
        }

        [UnityTest]
        public IEnumerator CaptureAuthoredObstaclesAndSpecialFamilies()
        {
            yield return PrepareIdle();
            GameState.Instance.Save.highestUnlockedLevel=10;
            foreach(int id in new[]{4,6,8,10})
            {
                GameState.Instance.SelectLevel(1,id);router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
                yield return WaitForNormalButtonColors();
                var view=Root().GetComponentInChildren<ProductionGameplayView>();
                Assert.That(view.Board.CurrentLevel.levelId,Is.EqualTo(id));
                CaptureAudit("20_Level"+id.ToString("00")+"_AuthoredObstacles");
            }
            var board=Root().GetComponentInChildren<ProductionGameplayView>().Board;
            foreach(string preset in new[]{"special.row-column","special.burst-burst","special.color-normal"})
            {
                Assert.That(board.LoadQAPreset(preset),Is.True);
                yield return WaitForNormalButtonColors();
                CaptureAudit("21_"+preset+"_QAPreset");
            }
        }

        IEnumerator PrepareIdle()
        {
            var tutorial=Root().GetComponentInChildren<LevelTutorialPresenter>();
            while(tutorial.IsPresenting){tutorial.Advance();yield return null;}
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
            yield return WaitForNormalButtonColors();
        }

        void CaptureAudit(string name)
        {
            Capture(name,new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }

        void CloseVisibleModal()
        {
            AnimatedHotspot close=null;
            foreach(var hotspot in router.GameCanvas.GetComponentsInChildren<AnimatedHotspot>())
                if(hotspot.name=="Hotspot_Close"&&hotspot.isActiveAndEnabled)close=hotspot;
            Assert.That(close,Is.Not.Null);
            var pointer=new PointerEventData(EventSystem.current){pointerId=704,position=new Vector2(100,100)};
            close.OnPointerDown(pointer);close.OnPointerUp(pointer);close.OnPointerClick(pointer);
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_Gameplay" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active Gameplay."); return null;
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
                    Color expected=button.colors.normalColor*button.colors.colorMultiplier;
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
            public string unityVersion, graphicsDevice, language, phase, vfxCue;
            public string stateSource;
            public int activeEffects;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_GAMEPLAY_CAPTURE_DIR");
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
            var cameraGo = new GameObject("GameplayCaptureCamera",typeof(Camera));
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
                foreach (Text text in canvas.GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Rebuild glyph snapping after the capture camera's responsive layout has settled.
                foreach (Text text in canvas.GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();

                // Do not apply the idle-only raycast assertions to tutorial/modal/action captures.
                inspect?.Invoke(camera);
                camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0); texture.Apply();
                string output = Path.Combine(directory,name);
                Assert.That(File.Exists(output + ".png"), Is.False, "Keep earlier evidence.");
                File.WriteAllBytes(output + ".png",texture.EncodeToPNG());
                File.WriteAllText(output + ".json",JsonUtility.ToJson(new CaptureInfo
                {
                    stateSource = name.Contains("QAPreset") ? "QA preset for state inspection; 50 moves and 999 goals are test data, not authored level values" : "Existing routed page with authored level and isolated save",
                    unityVersion = Application.unityVersion, graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                    language = DemoFeatureState.LanguageCode, width = size.x, height = size.y, safeArea = safeArea,
                    phase = Root().GetComponentInChildren<ProductionGameplayView>().Board.Phase.ToString(),
                    vfxCue = Root().GetComponentInChildren<ProductionGameplayView>().Board.VfxPresenter.LastCue.ToString(),
                    activeEffects = Root().GetComponentInChildren<ProductionGameplayView>().Board.VfxPresenter.ActiveEffectCount,
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

    }
}
