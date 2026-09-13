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
    public sealed class GameplayAlignmentPlayModeTests
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
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay); Settle();
            yield return null; yield return null;
            yield return WaitForNormalButtonColors();
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

        [UnityTest]
        public IEnumerator CaptureReferenceAndPhonePortraits()
        {
            Capture("Gameplay_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Gameplay_576x1280_safearea_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Gameplay_1080x2400_safearea_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Capture("Gameplay_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");yield return null;yield return null;
            Capture("Gameplay_941x1672_RU_roundtrip",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }

        [UnityTest]
        public IEnumerator LiveInventoryAndMovesUseTheExistingBoosterTransaction()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;var state=GameState.Instance;
            Assert.That(view.MovesValue.text,Is.EqualTo(board.Moves.ToString()));Assert.That(view.Goals[0].Count.text,Is.EqualTo(board.GoalSlot1Remaining.ToString()));
            for(int i=0;i<4;i++){state.AddBooster(ProductionPreLevelRuntime.Boosters[i],2);Assert.That(view.BoosterCounts[i].text,Is.EqualTo(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));}
            int inventory=state.BoosterCount(BoosterType.Firebird);string before=board.BoardStateHash;
            view.BoosterButtons[1].onClick.Invoke();yield return null;
            Assert.That(board.ArmedBooster,Is.EqualTo(BoosterType.Firebird));Assert.That(view.ArmedMarkers[1].gameObject.activeSelf,Is.True);
            view.BoosterButtons[1].onClick.Invoke();yield return null;Assert.That(board.IsBoosterArmed,Is.False);
            Assert.That(state.BoosterCount(BoosterType.Firebird),Is.EqualTo(inventory));Assert.That(board.BoardStateHash,Is.EqualTo(before));
            int moves=board.Moves;inventory=state.BoosterCount(BoosterType.SeaPearl);
            view.BoosterButtons[3].onClick.Invoke();view.BoosterButtons[3].onClick.Invoke();
            float deadline=Time.realtimeSinceStartup+8;
            while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(board.HasActiveCommand,Is.False);Assert.That(board.Moves,Is.EqualTo(moves+5));Assert.That(view.MovesValue.text,Is.EqualTo(board.Moves.ToString()));
            Assert.That(state.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(inventory-1));Assert.That(view.BoosterCounts[3].text,Is.EqualTo((inventory-1).ToString()));
        }

        [UnityTest]
        public IEnumerator NativePauseKeepsTheSameBoardAndBlocksBoosterInput()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;string hash=board.BoardStateHash;
            view.PauseButton.onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Paused));
            int stock=GameState.Instance.BoosterCount(BoosterType.SeaPearl);view.BoosterButtons[3].onClick.Invoke();Assert.That(GameState.Instance.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(stock));
            AnimatedHotspot resume=null;foreach(var hotspot in router.GameCanvas.GetComponentsInChildren<AnimatedHotspot>())if(hotspot.name=="Hotspot_Resume"&&hotspot.isActiveAndEnabled)resume=hotspot;
            Assert.That(resume,Is.Not.Null);var pointer=new PointerEventData(EventSystem.current){pointerId=709};resume.OnPointerDown(pointer);resume.OnPointerUp(pointer);resume.OnPointerClick(pointer);
            Settle();yield return null;yield return null;
            Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Idle));Assert.That(view.Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(view.ActionsBound,Is.True);
        }

        [UnityTest]
        public IEnumerator AuthoredGoalsObstaclesAndFeathersUseTheirActualState()
        {
            GameState.Instance.Save.highestUnlockedLevel=10;
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
            {
                GameState.Instance.SelectLevel(1,level.levelId);router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;yield return WaitForNormalButtonColors();
                var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
                Assert.That(board.CurrentLevel.levelId,Is.EqualTo(level.levelId));Assert.That(view.LevelText.text,Is.EqualTo("Уровень "+level.levelId));
                var model=(BoardModel)typeof(Match3Board).GetField("model",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(board);
                for(int y=0;y<board.RowCount;y++)for(int x=0;x<board.ColumnCount;x++)
                {
                    var pos=new BoardPosition(x,y);var cell=board.GetCellInputForTesting(x,y);var obstacle=model.GetObstacle(pos);
                    var graphic=cell.transform.Find("CellContent/Obstacle").GetComponent<Image>();
                    Assert.That(graphic.enabled,Is.EqualTo(!obstacle.IsEmpty));if(!obstacle.IsEmpty)Assert.That(graphic.sprite,Is.SameAs(ProductionGameplayArt.Obstacle(obstacle.Kind,obstacle.Layers)));
                    var count=cell.transform.Find("CellContent/ObstacleLayers").GetComponent<Text>();Assert.That(count.enabled,Is.EqualTo(!obstacle.IsEmpty&&obstacle.Layers>1));
                    if(count.enabled)Assert.That(count.text,Is.EqualTo(obstacle.Layers.ToString()));
                    var gem=cell.transform.Find("CellContent/GemVisual/Gem").GetComponent<Image>();
                    var key=cell.transform.Find("CellContent/GemVisual/SpecialMatchColor").GetComponent<Image>();
                    bool special=model.GetSpecial(pos)!=SpecialGemKind.None&&model.GetTile(pos)>=0&&model.GetObjectiveItem(pos)==BoardObjectiveItemKind.None;
                    Assert.That(key.enabled,Is.EqualTo(special));if(special)Assert.That(key.sprite,Is.SameAs(ProductionGameplayArt.Gem(model.GetTile(pos))));
                    if(model.GetObjectiveItem(pos)!=BoardObjectiveItemKind.None){Assert.That(gem.enabled,Is.True);Assert.That(gem.sprite,Is.SameAs(ProductionGameplayArt.Load("feather")));}
                    else if(model.GetSpecial(pos)==SpecialGemKind.None&&model.GetTile(pos)>=0)Assert.That(gem.sprite,Is.SameAs(ProductionGameplayArt.Gem(model.GetTile(pos))));
                }
                if(level.levelId==4||level.levelId==6||level.levelId==8||level.levelId==10)Capture("Gameplay_Level"+level.levelId.ToString("00")+"_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            }
            var finalView=Root().GetComponentInChildren<ProductionGameplayView>();DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Capture("Gameplay_Level10_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
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
            Assert.Fail("No active Pre-Level."); return null;
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
            public string unityVersion, graphicsDevice, language;
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
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Rebuild glyph snapping after the capture camera's responsive layout has settled.
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();

                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionGameplayView>(),camera,size,safeArea);
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
            var corners=new Vector3[4];rect.GetWorldCorners(corners);
            Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        void VerifyLayoutAndInput(ProductionGameplayView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.ActionsBound,Is.True);
            foreach(var star in view.Stars)
            {
                Assert.That(star.GetComponent<ProductionGameplayStarPolish>(),Is.Not.Null);
                var mesh=star.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                if(star.Dimmed)
                {
                    var center=star.rectTransform.rect.center;var vertices=mesh.vertices;var colors=mesh.colors32;int darkCenters=0;
                    for(int i=0;i<vertices.Length;i++)
                    {
                        Assert.That(float.IsNaN(vertices[i].x)||float.IsNaN(vertices[i].y),Is.False);
                        if(Vector2.Distance(vertices[i],center)<.1f&&colors[i].r<12&&colors[i].g<20&&colors[i].b<25)darkCenters++;
                    }
                    Assert.That(darkCenters,Is.GreaterThanOrEqualTo(10),"Unearned Gameplay stars need dark centres inside the gold outlines.");
                }
            }
            Assert.That(view.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(5));
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            Rect environment=Bounds(view.Environment.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Rect board=Bounds(view.BoardMount,camera);
            Assert.That(board.width/board.height,Is.EqualTo(786f/942).Within(.001f));
            foreach(Button button in view.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                Assert.That(rect.Overlaps(board),Is.False,button.name+" overlaps playable board");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits,Is.Not.Empty,button.name);Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name);
            }
            for(int y=0;y<view.Board.RowCount;y++)for(int x=0;x<view.Board.ColumnCount;x++)
            {
                var cell=view.Board.GetCellInputForTesting(x,y);Rect rect=Bounds((RectTransform)cell.transform,camera);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits,Is.Not.Empty);Assert.That(hits[0].gameObject.GetComponent<Match3CellInput>(),Is.SameAs(cell));
                var graphic=cell.GetComponentInChildren<ProductionGameplayCell>();Assert.That(graphic.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(0));
            }
            foreach(var star in view.Stars)Assert.That(star.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(0));
            foreach(var badge in view.GetComponentsInChildren<ProductionPreLevelBadge>())Assert.That(badge.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(100));
            var layout=view.GetComponent<ProductionGameplayLayout>();Vector2 old=view.BoardArea.anchoredPosition;layout.ApplyNow();Canvas.ForceUpdateCanvases();Assert.That(Vector2.Distance(old,view.BoardArea.anchoredPosition),Is.LessThan(.01f));
        }
    }
}
