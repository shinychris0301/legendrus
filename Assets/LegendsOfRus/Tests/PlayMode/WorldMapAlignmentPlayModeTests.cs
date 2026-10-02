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
    public sealed class WorldMapAlignmentPlayModeTests
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
            router.Show(ScreenId.WorldMap); Settle();
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
        public IEnumerator SharedHeaderValuesAndFittingAgreeAcrossPages()
        {
            var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.energy=3;save.level=28;
            foreach(string language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);
                router.Show(ScreenId.WorldMap);Settle();yield return null;
                var map=Root().GetComponentInChildren<ProductionWorldMapView>();map.RefreshContent();
                string[] values=Array.ConvertAll(map.Header.Balances,t=>t.text);
                int[] sizes=Array.ConvertAll(map.Header.Balances,t=>t.fontSize);
                string name=map.Header.PlayerName.text,level=map.Header.Level.text;
                foreach(var label in map.Header.Balances)Assert.That(label.preferredWidth,Is.LessThanOrEqualTo(label.rectTransform.rect.width-20));
                Capture("WorldMap_MaxBalances_"+language,new Vector2Int(941,1672),new Rect(0,0,941,1672));
                router.Show(ScreenId.Home);Settle();yield return null;
                var home=router.Viewport.GetComponentInChildren<ProductionHomeView>();home.RefreshContent();
                CollectionAssert.AreEqual(values,Array.ConvertAll(home.Header.Balances,t=>t.text));
                CollectionAssert.AreEqual(sizes,Array.ConvertAll(home.Header.Balances,t=>t.fontSize));
                Assert.That(home.Header.PlayerName.text,Is.EqualTo(name));Assert.That(home.Header.Level.text,Is.EqualTo(level));
                Assert.That(home.Header.Backing.sprite,Is.SameAs(Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+"source_header")));
            }
        }

        [UnityTest]
        public IEnumerator ChapterCardsShowPressFeedbackAndStyledNotice()
        {
            var view=Root().GetComponentInChildren<ProductionWorldMapView>();
            foreach(var b in view.GetComponentsInChildren<Button>(true))
                Assert.That(b.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null,b.name);
            var card=view.Cards[0];var feedback=card.Button.GetComponent<ProductionMapButtonFeedback>();
            Color normal=card.Terrain.canvasRenderer.GetColor();
            var e=new PointerEventData(EventSystem.current){pointerId=17,button=PointerEventData.InputButton.Left};
            ExecuteEvents.Execute(card.Button.gameObject,e,ExecuteEvents.pointerDownHandler);
            Assert.That(feedback.IsPressed,Is.True);
            Assert.That(card.Terrain.canvasRenderer.GetColor().r,Is.LessThan(normal.r*.8f));
            Capture("WorldMap_Pressed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            ExecuteEvents.Execute(card.Button.gameObject,e,ExecuteEvents.pointerUpHandler);
            yield return new WaitForSecondsRealtime(.2f);
            Assert.That(card.Terrain.canvasRenderer.GetColor(),Is.EqualTo(normal));
            view.Cards[1].Button.onClick.Invoke();
            Assert.That(view.Notice.Group.alpha,Is.EqualTo(1));
            Assert.That(view.Notice.Message.text,Does.Contain("2"));
            Assert.That(view.Notice.Group.blocksRaycasts,Is.False);
            Capture("WorldMap_LockedNotice_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            router.Show(ScreenId.Home);Settle();
            Assert.That(view.Notice.Group.alpha,Is.Zero);
        }

        [UnityTest]
        public IEnumerator AuthoredChapterAndLockedNodesUseRealStateAndRoutes()
        {
            var view=Root().GetComponentInChildren<ProductionWorldMapView>();
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.ActionsBound,Is.True);
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            string save=JsonUtility.ToJson(GameState.Instance.Save);
            for(int i=1;i<20;i++)
            {
                Assert.That(view.ChapterAvailable(i+1),Is.False);
                Assert.That(view.Cards[i].Lock.gameObject.activeSelf,Is.True);
                view.Cards[i].Button.onClick.Invoke();
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
            }
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
            Assert.That(view.Cards[0].Lock.gameObject.activeSelf,Is.False);
            view.Cards[0].Button.onClick.Invoke();Settle();
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
            router.NavigateBack();Settle();
            view=Root().GetComponentInChildren<ProductionWorldMapView>();view.BackButton.onClick.Invoke();Settle();
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
            yield return null;
        }

        [UnityTest]
        public IEnumerator HudTasksAndGuildRespondWithoutInventingProgress()
        {
            var view=Root().GetComponentInChildren<ProductionWorldMapView>();
            Assert.That(view.LevelText.text,Is.EqualTo("Уровень 1"));
            GameState.Instance.AddGold(7);GameState.Instance.AddGems(3);GameState.Instance.SpendEnergy(1);
            Assert.That(view.GoldText.text,Is.EqualTo("12 547"));Assert.That(view.GemsText.text,Is.EqualTo("353"));Assert.That(view.EnergyText.text,Is.EqualTo("4/5"));
            view.NavButtons[1].onClick.Invoke();Assert.That(view.TaskPanel.activeSelf,Is.True);
            ChapterLevelCatalogData catalog=ChapterLevelCatalog.LoadChapter1();
            Assert.That(view.TaskCopy.text,Does.Contain("0 / "+catalog.levels.Length));
            for(int i=0;i<catalog.levels.Length;i++)
            {
                LevelData level=catalog.levels[i];GameState.Instance.CompleteLevel(level,level.starScoreThresholds[2]);
                if(i<catalog.levels.Length-1)Assert.That(view.Cards[0].Completed,Is.False,"Completion needs every authored level.");
            }
            Assert.That(view.TaskCopy.text,Does.Contain(catalog.levels.Length+" / "+catalog.levels.Length));
            Assert.That(view.Cards[0].Completed,Is.True);Assert.That(view.Cards[0].Progress.text,Is.EqualTo((catalog.levels.Length*3)+"/"+(catalog.levels.Length*3)));
            Assert.That(view.ChapterAvailable(2),Is.False,"A completed chapter must not unlock missing authored data.");
            router.NavigateBack();Assert.That(view.TaskPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
            view.NavButtons[2].onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));
            ScreenId[] targets={ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++)
            {
                router.Show(ScreenId.WorldMap);Settle();view=Root().GetComponentInChildren<ProductionWorldMapView>();
                Button button=i==0?view.ProfileButton:i==1?view.SettingsButton:view.PlusButtons[i-2];
                button.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator ScrollHighlightAndLocalizationPreserveRealProgress()
        {
            var view=Root().GetComponentInChildren<ProductionWorldMapView>();
            Assert.That(view.Cards.Length,Is.EqualTo(20));
            var first=view.Cards[0];Assert.That(first.Current,Is.True);
            Assert.That(first.Progress.text,Is.EqualTo("0/30"));
            Assert.That(first.BorderGlow.gameObject.activeSelf,Is.True);
            Assert.That(first.Surface.color,Is.EqualTo(Color.white));
            Assert.That(first.Terrain.color,Is.EqualTo(Color.white));
            Assert.That(first.Frame.color,Is.EqualTo(Color.white));
            Assert.That(first.Button.transition,Is.EqualTo(Selectable.Transition.None));
            for(int i=1;i<20;i++)
            {
                Assert.That(view.Cards[i].BorderGlow.gameObject.activeSelf,Is.False);
                Assert.That(view.Cards[i].Progress.text,Is.EqualTo("—"));
                Assert.That(view.Cards[i].Terrain.color.r,Is.LessThan(1));
            }
            string save=JsonUtility.ToJson(GameState.Instance.Save);
            var scroll=view.ChapterScroll;Canvas.ForceUpdateCanvases();
            float before=scroll.content.anchoredPosition.y;
            Vector2 center=RectTransformUtility.WorldToScreenPoint(null,scroll.viewport.TransformPoint(scroll.viewport.rect.center));
            var pointer=new PointerEventData(EventSystem.current){position=center,pressPosition=center,button=PointerEventData.InputButton.Left};
            scroll.OnInitializePotentialDrag(pointer);scroll.OnBeginDrag(pointer);
            pointer.position=center+Vector2.down*160;scroll.OnDrag(pointer);scroll.OnEndDrag(pointer);scroll.StopMovement();
            Assert.That(scroll.content.anchoredPosition.y,Is.LessThan(before),"Dragging down brings later chapters from above.");
            Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(save));
            scroll.verticalNormalizedPosition=1;yield return null;
            Capture("WorldMap_LastChapters_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Assert.That(view.TitleText.enabled,Is.True);Assert.That(view.TitlePlaque.sprite.name,Does.EndWith("_blank"));
            Assert.That(view.Cards[0].Name.text,Is.EqualTo(view.Cards[0].EnglishName.ToUpperInvariant()));
            DemoFeatureState.SetLanguage("ru");yield return null;yield return null;
            Assert.That(view.TitleText.enabled,Is.False);Assert.That(view.TitlePlaque.sprite.name,Is.EqualTo("chapter_title"));
            scroll.verticalNormalizedPosition=0;Canvas.ForceUpdateCanvases();
            first.BorderGlow.gameObject.SetActive(false);
            Capture("WorldMap_BorderGlow_Off",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            first.BorderGlow.gameObject.SetActive(true);
            Capture("WorldMap_BorderGlow_On",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            // Additional reference navigation slots keep their actual routes.
            view.NavButtons[4].onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Shop));
            router.Show(ScreenId.WorldMap);Settle();view=Root().GetComponentInChildren<ProductionWorldMapView>();
            view.NavButtons[3].onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
        }

        [UnityTest]
        public IEnumerator CaptureReferenceAndPhonePortraits()
        {
            Capture("WorldMap_941x1672_RU", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            Capture("WorldMap_576x1280_safearea_RU", new Vector2Int(576,1280), new Rect(0,48,576,1172));
            Capture("WorldMap_1080x2400_safearea_RU", new Vector2Int(1080,2400), new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en"); yield return null; yield return null;
            Capture("WorldMap_941x1672_EN", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru"); yield return null; yield return null;
            Capture("WorldMap_941x1672_RU_roundtrip", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            var view=Root().GetComponentInChildren<ProductionWorldMapView>();
            view.NavButtons[1].onClick.Invoke();
            yield return null; yield return null;
            Capture("WorldMap_Tasks_941x1672_RU", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            Capture("WorldMap_Tasks_576x1280_safearea_RU", new Vector2Int(576,1280), new Rect(0,48,576,1172));
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Capture("WorldMap_Tasks_941x1672_EN", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            view.NavButtons[0].onClick.Invoke();Assert.That(view.TaskPanel.activeSelf,Is.False);
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_WorldMap" && t.gameObject.activeInHierarchy) return t.gameObject;
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
            string directory = Environment.GetEnvironmentVariable("LOR_WORLDMAP_CAPTURE_DIR");
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
            var cameraGo = new GameObject("WorldMapCaptureCamera",typeof(Camera));
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
                var view=Root().GetComponentInChildren<ProductionWorldMapView>();
                if(view!=null) VerifyLayoutAndInput(view,camera,size,safeArea);
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
        void VerifyLayoutAndInput(ProductionWorldMapView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);
            foreach(var graphic in view.GetComponentsInChildren<ProductionWorldMapGraphic>())
            {
                Assert.That(graphic.GetComponent<CanvasRenderer>(),Is.Not.Null,graphic.name);
                var mesh = graphic.canvasRenderer.GetMesh();
                Assert.That(mesh,Is.Not.Null,graphic.name);
                Assert.That(mesh.vertexCount,Is.GreaterThan(0),graphic.name+" must have rendered geometry");
            }
            Rect environment=Bounds(view.Environment.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(Button button in view.TaskPanel.activeSelf ? view.TaskPanel.GetComponentsInChildren<Button>() : view.GetComponentsInChildren<Button>())
            {
                if(!button.gameObject.activeInHierarchy)continue;
                Rect rect=Bounds(button.transform as RectTransform,camera);
                if(button.transform.IsChildOf(view.ChapterScroll.content))
                {
                    Rect viewport=Bounds(view.ChapterScroll.viewport,camera);
                    if(!viewport.Contains(rect.center))continue;
                }
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                var results=new List<RaycastResult>();
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},results);
                Assert.That(results.Count,Is.GreaterThan(0),button.name);
                Assert.That(results[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" is obscured");
            }
            var layout=view.GetComponent<ProductionWorldMapLayout>();
            Vector2 old=view.Cards[0].Root.anchoredPosition;
            layout.ApplyNow();Canvas.ForceUpdateCanvases();
            Assert.That(Vector2.Distance(old,view.Cards[0].Root.anchoredPosition),Is.LessThan(.01f));
        }
    }
}
