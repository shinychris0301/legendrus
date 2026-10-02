using System;
using System.Collections;
using System.Linq;
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
    public sealed class ChapterMapAlignmentPlayModeTests
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
            router.Show(ScreenId.ChapterMap); Settle();
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
        public IEnumerator LevelSelectionStarsAndLockedTapsUseActualSave()
        {
            var view=Root().GetComponentInChildren<ProductionChapterMapView>();
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.ActionsBound,Is.True);
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            string save=JsonUtility.ToJson(GameState.Instance.Save);
            for(int i=1;i<10;i++)
            {
                Assert.That(view.LevelAvailable(i+1),Is.False);
                Assert.That(view.Nodes[i].Lock.gameObject.activeSelf,Is.True);
                view.Nodes[i].Button.onClick.Invoke();
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
            }
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
            foreach(var node in view.Nodes)foreach(var star in node.Stars)Assert.That(star.Dimmed,Is.True);
            var level=ChapterLevelCatalog.Find(1,1);
            GameState.Instance.CompleteLevel(level,level.starScoreThresholds[2]);
            foreach(var star in view.Nodes[0].Stars)Assert.That(star.Dimmed,Is.False);
            foreach(var star in view.Nodes[0].SourceStars)Assert.That(star.sprite.name,Is.EqualTo("star_full"));
            Assert.That(view.LevelAvailable(2),Is.True);Assert.That(view.LevelAvailable(3),Is.False);
            GameState.Instance.CompleteStoryScene(StoryCatalog.LoadChapter1().introSceneId);
            GameState.Instance.CompleteStoryScene(StoryCatalog.FindMilestoneAfterLevel(1).sceneId);StoryService.ResetRuntime();
            view.Nodes[1].Button.onClick.Invoke();Settle();
            Assert.That(GameState.Instance.Save.selectedLevel,Is.EqualTo(2));
            Assert.That(view.Nodes[1].Halo.gameObject.activeSelf,Is.True);
            Assert.That(view.Nodes[0].Halo.gameObject.activeSelf,Is.False);
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel),"Tapping an available round opens its preparation page.");
            var preparation=router.Viewport.GetComponentInChildren<ProductionPreLevelView>();
            Assert.That(preparation.SelectedLevel.text,Does.Contain("2"));
            int energy=GameState.Instance.Save.energy;
            preparation.PlayButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));
            Assert.That(router.Viewport.GetComponentInChildren<Match3Board>().CurrentLevel.levelId,Is.EqualTo(2));
            Assert.That(GameState.Instance.Save.energy,Is.EqualTo(energy-1));
            string selectedSave=JsonUtility.ToJson(GameState.Instance.Save);
            router.Show(ScreenId.WorldMap);Settle();router.Show(ScreenId.ChapterMap);Settle();
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(selectedSave));
            yield return null;yield return null;yield return WaitForNormalButtonColors();
            view=Root().GetComponentInChildren<ProductionChapterMapView>();
            Assert.That(view.Nodes[1].Halo.gameObject.activeSelf,Is.True);
            Capture("ChapterMap_Progress_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            view.BackButton.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
        }

        [UnityTest]
        public IEnumerator PlayPreservesIntroAndPreLevelRoutingAndHudActions()
        {
            var view=Root().GetComponentInChildren<ProductionChapterMapView>();
            view.Nodes[0].Button.onClick.Invoke();Settle();
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Dialogue));
            GameState.Instance.CompleteStoryScene(StoryCatalog.LoadChapter1().introSceneId);StoryService.ResetRuntime();
            router.Show(ScreenId.ChapterMap);Settle();
            view=Root().GetComponentInChildren<ProductionChapterMapView>();
            view.PlayButton.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
            router.Show(ScreenId.ChapterMap);Settle();view=Root().GetComponentInChildren<ProductionChapterMapView>();
            GameState.Instance.AddGold(7);GameState.Instance.AddGems(3);GameState.Instance.SpendEnergy(1);
            Assert.That(view.GoldText.text,Is.EqualTo("12 547"));Assert.That(view.GemsText.text,Is.EqualTo("353"));Assert.That(view.EnergyText.text,Is.EqualTo("4/5"));
            ScreenId[] targets={ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++)
            {
                router.Show(ScreenId.ChapterMap);Settle();view=Root().GetComponentInChildren<ProductionChapterMapView>();
                Button button=i==0?view.ProfileButton:i==1?view.SettingsButton:view.PlusButtons[i-2];
                button.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator CaptureReferenceAndPhonePortraits()
        {
            Capture("ChapterMap_941x1672_RU", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            Capture("ChapterMap_576x1280_safearea_RU", new Vector2Int(576,1280), new Rect(0,48,576,1172));
            Capture("ChapterMap_1080x2400_safearea_RU", new Vector2Int(1080,2400), new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en"); yield return null; yield return null;
            Capture("ChapterMap_941x1672_EN", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            var english=Root().GetComponentInChildren<ProductionChapterMapView>();
            Assert.That(english.PlayText.enabled,Is.True);Assert.That(english.PlayText.text,Is.EqualTo("PLAY"));
            Assert.That(english.PlayText.transform.GetSiblingIndex(),Is.GreaterThan(english.SourcePlay.transform.GetSiblingIndex()));
            DemoFeatureState.SetLanguage("ru"); yield return null; yield return null;
            Capture("ChapterMap_941x1672_RU_roundtrip", new Vector2Int(941,1672), new Rect(0,0,941,1672));

        }

        [UnityTest]
        public IEnumerator FiveHundredSlotsRecycleWithoutUnlockingOrStaleCallbacks()
        {
            var view=Root().GetComponentInChildren<ProductionChapterMapView>();var sc=view.Scroller;
            var buttons=view.Nodes.Select(n=>n.Button).ToArray();
            string saved=JsonUtility.ToJson(GameState.Instance.Save);
            sc.JumpToLevel(500);yield return null;
            Assert.That(view.Nodes.Any(n=>n.LevelId==500&&n.Root.gameObject.activeSelf),Is.True);
            var last=view.Nodes.First(n=>n.LevelId==500);last.Button.onClick.Invoke();
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(saved));
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
            Capture("ChapterMap_End500_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            sc.JumpToLevel(51);yield return null;
            Assert.That(view.Nodes.Select(n=>n.Button).ToArray(),Is.EqualTo(buttons));
            foreach(var n in view.Nodes)if(n.Root.gameObject.activeSelf)n.Button.onClick.Invoke();
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(saved));
            Capture("ChapterMap_SectionBoundary_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            sc.CurrentButton.onClick.Invoke();yield return null;
            Assert.That(sc.Offset,Is.Zero);Assert.That(view.Nodes[0].LevelId,Is.EqualTo(1));
            
            sc.SetOffset(-100);Assert.That(sc.Offset,Is.Zero);
            sc.SetOffset(float.MaxValue);Assert.That(sc.Offset,Is.EqualTo(sc.MaximumScrollOffset));
            sc.JumpToCurrent();
            var pointer=new PointerEventData(EventSystem.current){position=new Vector2(400,700),pointerId=8,button=PointerEventData.InputButton.Left,eligibleForClick=true};
            sc.OnBeginDrag(pointer);pointer.position+=Vector2.down*250;sc.OnDrag(pointer);
            Assert.That(sc.Offset,Is.GreaterThan(0));Assert.That(pointer.eligibleForClick,Is.False);
            float dragged=sc.Offset;
            var other=new PointerEventData(EventSystem.current){position=Vector2.zero,pointerId=9};
            sc.OnDrag(other);sc.OnEndDrag(other);Assert.That(sc.Offset,Is.EqualTo(dragged));Assert.That(sc.IsDragging,Is.True);
            sc.OnEndDrag(pointer);Assert.That(sc.IsDragging,Is.False);
            sc.JumpToCurrent();
        }


        [UnityTest]
        public IEnumerator PressFeedbackNoticeAndDragCancellation()
        {
            var view=Root().GetComponentInChildren<ProductionChapterMapView>();
            foreach(var button in view.GetComponentsInChildren<Button>(true))
                Assert.That(button.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null,button.name);
            var node=view.Nodes[0];var feedback=node.Button.GetComponent<ProductionMapButtonFeedback>();
            var pointer=new PointerEventData(EventSystem.current){pointerId=41,button=PointerEventData.InputButton.Left};
            Color normal=node.Face.canvasRenderer.GetColor();
            Vector3 position=node.Root.position;
            ExecuteEvents.Execute(node.Button.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            Assert.That(feedback.IsPressed,Is.True);
            Assert.That(node.Face.canvasRenderer.GetColor().r,Is.LessThan(normal.r*.8f));
            Assert.That(node.Root.position,Is.EqualTo(position),"Press must not move the hit area.");
            Capture("ChapterMap_Pressed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            ExecuteEvents.Execute(node.Button.gameObject,new PointerEventData(EventSystem.current){pointerId=42},ExecuteEvents.pointerUpHandler);
            Assert.That(feedback.IsPressed,Is.True,"Another pointer cannot release this press.");
            ExecuteEvents.Execute(node.Button.gameObject,pointer,ExecuteEvents.pointerExitHandler);
            yield return new WaitForSecondsRealtime(.2f);
            Assert.That(feedback.IsPressed,Is.False);
            Assert.That(node.Face.canvasRenderer.GetColor(),Is.EqualTo(normal));
            pointer.position=new Vector2(400,700);
            view.Scroller.OnBeginDrag(pointer);
            node.Button.onClick.Invoke();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
            view.Scroller.OnEndDrag(pointer);
            view.Nodes[1].Button.onClick.Invoke();
            Assert.That(view.Notice.Group.alpha,Is.EqualTo(1));
            Assert.That(view.Notice.Group.blocksRaycasts,Is.False);
            Assert.That(view.Notice.Message.text,Does.Contain("предыдущий"));
            Capture("ChapterMap_LockedNotice_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            view.Notice.Hide();
            view.Scroller.JumpToLevel(500);
            view.Nodes.First(n=>n.LevelId==500).Button.onClick.Invoke();
            Assert.That(view.Notice.Message.text,Does.Contain("скоро"));
            Assert.That(view.GetComponentsInChildren<ProductionMapNotice>(true).Length,Is.EqualTo(1));
            yield return new WaitForSecondsRealtime(3.8f);
            Assert.That(view.Notice.Group.alpha,Is.Zero);
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_ChapterMap" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active World Map."); return null;
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
            string directory = Environment.GetEnvironmentVariable("LOR_CHAPTERMAP_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ChapterMapCaptureCamera",typeof(Camera));
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
                var view=Root().GetComponentInChildren<ProductionChapterMapView>();
                VerifyLayoutAndInput(view,camera,size,safeArea);
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
        void VerifyLayoutAndInput(ProductionChapterMapView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);
            foreach(var graphic in view.GetComponentsInChildren<ProductionWorldMapGraphic>())
            {
                if(!graphic.enabled||graphic.canvasRenderer.cull)continue;
                Assert.That(graphic.GetComponent<CanvasRenderer>(),Is.Not.Null,graphic.name);
                Rect gb=Bounds(graphic.rectTransform,camera),window=Bounds(view.MapArea,camera);
                if(graphic.transform.IsChildOf(view.MapArea)&&!gb.Overlaps(window))continue;
                var mesh = graphic.canvasRenderer.GetMesh();
                Assert.That(mesh,Is.Not.Null,graphic.name);
                Assert.That(mesh.vertexCount,Is.GreaterThan(0),graphic.name+" must have rendered geometry");
            }
            Assert.That(Bounds(view.Hud,camera).yMax,Is.EqualTo(safe.yMax).Within(1f),"Header must meet the top safe-area edge.");
            Assert.That(Bounds(view.Footer,camera).yMin,Is.EqualTo(safe.yMin).Within(1f),"Footer must meet the bottom safe-area edge.");
            Assert.That(Bounds(view.MapArea,camera).yMin,Is.EqualTo(Bounds(view.Footer,camera).yMax).Within(1f),"Map must fill the area above the footer.");
            Rect environment=Bounds(view.EnvironmentMargins.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(Button button in view.GetComponentsInChildren<Button>())
            {
                if(!button.gameObject.activeInHierarchy)continue;
                var pooled=System.Array.Find(view.Nodes,n=>n.Button==button);
                if(pooled!=null){Rect visible=Bounds(view.MapArea,camera),nr=Bounds(pooled.Root,camera);if(!visible.Contains(nr.min)||!visible.Contains(nr.max))continue;}
                Rect rect=Bounds(button.transform as RectTransform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                var results=new List<RaycastResult>();
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},results);
                Assert.That(results.Count,Is.GreaterThan(0),button.name);
                Assert.That(results[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" is obscured");
            }
            Rect mapWindow=Bounds(view.MapArea,camera);
            int fullyVisible=view.Nodes.Count(n=>n.Root.gameObject.activeInHierarchy && mapWindow.Contains(Bounds(n.Root,camera).min) && mapWindow.Contains(Bounds(n.Root,camera).max));
            Assert.That(fullyVisible,Is.GreaterThanOrEqualTo(8),"Taller vertical spacing must retain eight complete round controls while scrolling (nine at rest).");
            foreach(var node in view.Nodes)
                foreach(var other in view.Nodes)
                {
                    if(node==other||!node.Root.gameObject.activeInHierarchy||!other.Root.gameObject.activeInHierarchy)continue;
                    Rect face=Bounds(other.Face.rectTransform,camera);
                    foreach(var star in node.SourceStars)
                        Assert.That(Bounds(star.rectTransform,camera).Overlaps(face),Is.False,node.Root.name+" stars overlap "+other.Root.name);
                }
            var layout=view.GetComponent<ProductionChapterMapLayout>();
            Vector2 old=view.Nodes[0].Root.anchoredPosition;
            layout.ApplyNow();Canvas.ForceUpdateCanvases();
            Assert.That(Vector2.Distance(old,view.Nodes[0].Root.anchoredPosition),Is.LessThan(.01f));
        }
    }
}
