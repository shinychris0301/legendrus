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
    public sealed class FailAlignmentPlayModeTests
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
            router.Show(ScreenId.Fail); Settle();
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

        [UnityTest] public IEnumerator LocalizedReactionDoesNotReplaceBoosterHelp()
        {
            GameState.Instance.Save.highestUnlockedLevel=55;
            Fail(55,120,new[]{1,1,0});yield return null;
            var v=View();var snapshot=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));snapshot.lastEnergyUtcTicks=0;
            string before=JsonUtility.ToJson(snapshot);
            foreach(string lang in new[]{"en","ru"})
            {
                DemoFeatureState.SetLanguage(lang);yield return null;yield return null;
                var expected=StoryCatalog.LocalizeReaction(StoryCatalog.FindReaction(55,"failure"),StoryCatalog.LoadDialogueLanguage(lang));
                Assert.That(v.Reaction.text,Is.EqualTo(expected.Speaker+": "+expected.Text));
                Assert.That(v.Reaction.gameObject.activeSelf,Is.False,"Keep accepted Fail caption visibility");
            }
            v.BoosterButtons[0].onClick.Invoke();Assert.That(v.Reaction.gameObject.activeSelf,Is.True);
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Assert.That(v.Reaction.text,Does.Contain("Thunder hammer"));
            Capture("Outcome55_FailHelp_EN_576",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            v.BoosterButtons[0].onClick.Invoke();Assert.That(v.Reaction.gameObject.activeSelf,Is.False);
            var en=StoryService.GetLocalizedReaction(ChapterLevelCatalog.Find(1,55),"failure","en");
            Assert.That(v.Reaction.text,Is.EqualTo(en.Speaker+": "+en.Text));
            snapshot=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));snapshot.lastEnergyUtcTicks=0;
            Assert.That(JsonUtility.ToJson(snapshot),Is.EqualTo(before));
            Assert.That(Root().transform.Find("OutcomeReaction"),Is.Null);
        }

        ProductionFailView View()=>Root().GetComponentInChildren<ProductionFailView>();
        void Fail(int level,int score,int[] progress)
        {
            GameState.Instance.Save.selectedLevel=level;GameState.Instance.RecordLevelFailure(ChapterLevelCatalog.Find(1,level),score,progress);router.Show(ScreenId.Fail);Settle();
        }
        static void VerifyLocalizedArtwork(ProductionFailView v,bool russian,bool hasFailure)
        {
            bool sourceTitle=russian&&hasFailure;
            Assert.That(v.Body.Find("FailureTitlePanel").GetComponent<Image>().sprite.name,Is.EqualTo(sourceTitle?"title_ru_r12":"title_r11"));
            Assert.That(v.Title.enabled||v.Subtitle.enabled,Is.EqualTo(!sourceTitle),"Never double-render Russian title or show out-of-moves without a failed round.");
            Assert.That(v.Title.enabled,Is.EqualTo(v.Subtitle.enabled));
            Assert.That(v.RetryText.enabled,Is.EqualTo(!russian));
            Assert.That(v.Retry.GetComponent<Image>().sprite.name,Is.EqualTo(russian?"retry_ru_r12":"retry_r11"));
            for(int i=0;i<4;i++)
            {
                Assert.That(v.Body.Find("Booster_"+i+"/Nameplate").GetComponent<Image>().sprite.name,Is.EqualTo(russian?ProductionFailRuntime.ReferenceNameplatesRu[i]:"nameplate_r11"));
                Assert.That(v.BoosterLabels[i].enabled,Is.EqualTo(!russian));
                Assert.That(v.BoosterCounts[i].enabled,Is.True,"Quantities stay live in every locale.");
            }
            bool sourceHeading=russian&&hasFailure&&v.GoalsHeading.text=="НЕ ВЫПОЛНЕНЫ ЦЕЛИ";
            Assert.That(v.GoalsHeading.enabled,Is.EqualTo(!sourceHeading));
            Assert.That(v.Body.Find("GoalsHeadingPlaque").GetComponent<Image>().sprite.name,Is.EqualTo(sourceHeading?"heading_source_ru_r13":"heading_r11"));
            Assert.That(v.PaidOfferTitle.enabled&&v.AdOfferTitle.enabled&&v.PaidOfferStatus.enabled&&v.AdOfferStatus.enabled,Is.True);
        }
        [UnityTest]public IEnumerator ActualProgressAndStockRenderInBothLanguagesAndSafeAreas()
        {
            var level=ChapterLevelCatalog.Find(1,2);var counts=new int[level.goals.Length];counts[0]=level.goals[0].target-2;
            Fail(2,321,counts);yield return null;yield return WaitForNormalButtonColors();
            var state=GameState.Instance;string save=JsonUtility.ToJson(state.Save);var v=View();
            Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(v.Score.text,Does.Contain("321"));
            Assert.That(v.Goals[0].Count.text,Is.EqualTo(counts[0]+"/"+level.goals[0].target));
            for(int i=0;i<4;i++)Assert.That(v.BoosterCounts[i].text,Is.EqualTo(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));
            v.RefreshContent();Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(save));
            VerifyLocalizedArtwork(v,true,true);
            Capture("Fail_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return null;
            VerifyLocalizedArtwork(v,false,true);
            Capture("Fail_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");v.RefreshContent();yield return null;
            VerifyLocalizedArtwork(v,true,true);
            Capture("Fail_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Fail_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(0,96,1080,2184));
        }
        [UnityTest]public IEnumerator ThreeGoalsCompletedGoalFilteringAndMissingResultAreTruthful()
        {
            var level=ChapterLevelCatalog.Find(1,10);Assert.That(level.goals.Length,Is.EqualTo(3));
            int[] progress={1,2,3};Fail(10,890,progress);yield return null;yield return WaitForNormalButtonColors();
            Capture("Fail_Level10_ThreeGoals_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;
            Capture("Fail_Level10_ThreeGoals_576_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            progress[0]=level.goals[0].target;Fail(10,900,progress);yield return null;
            Assert.That(Array.FindAll(View().Goals,g=>g.Root.gameObject.activeSelf).Length,Is.EqualTo(2));
            Assert.That(View().Goals[0].Count.text,Is.EqualTo(progress[1]+"/"+level.goals[1].target));
            Capture("Fail_CompletedGoalFiltered_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Fail(10,910,null);yield return null;Assert.That(View().Goals[0].Root.gameObject.activeSelf,Is.False);Assert.That(View().GoalsHeading.text,Does.Contain("UNAVAILABLE"));
            GameState.Instance.ResetProgress();View().RefreshContent();yield return WaitForNormalButtonColors();
            Assert.That(GameState.Instance.LastFailureResult,Is.Null);Assert.That(View().Retry.interactable,Is.False);
            VerifyLocalizedArtwork(View(),false,false);
            Capture("Fail_NoResult_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");View().RefreshContent();yield return null;
            VerifyLocalizedArtwork(View(),true,false);
            Assert.That(View().Subtitle.text,Does.Contain("НЕТ ЗАВЕРШЁННОЙ"));
            Capture("Fail_NoResult_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [UnityTest]public IEnumerator RetryExitAndBoosterGuidePreserveExistingTransactions()
        {
            Fail(2,130,new[]{1,2});yield return null;var v=View();var state=GameState.Instance;
            string before=JsonUtility.ToJson(state.Save);v.PaidOffer.onClick.Invoke();v.AdOffer.onClick.Invoke();Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
            string[] captions={"МОЛОТ ГРОМА","ФЕНИКС","ЩИТ ДРУЖИНЫ","ЗЕЛЬЕ СИЛЫ"};
            string[] effects={"столбец","3×3","выбранного цвета","5 ходов"};
            for(int i=0;i<4;i++)
            {
                Assert.That(v.BoosterLabels[i].text,Is.EqualTo(captions[i]));
                v.BoosterButtons[i].onClick.Invoke();
                Assert.That(v.Reaction.text,Does.Contain(effects[i]),"Reference alias must still explain the existing function.");
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before),"Inspecting an alias must not spend stock or resources.");
            }
            Capture("Fail_BoosterGuide_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            int energy=state.Save.energy,gold=state.Save.gold;
            v.Retry.onClick.Invoke();Settle();yield return null;
            var board=router.Viewport.GetComponentInChildren<Match3Board>();Assert.That(board,Is.Not.Null);Assert.That(board.CurrentLevel.levelId,Is.EqualTo(2));Assert.That(board.Score,Is.Zero);
            v.Retry.onClick.Invoke();Assert.That(router.Viewport.GetComponentInChildren<Match3Board>(),Is.SameAs(board));
            Assert.That(state.Save.energy,Is.EqualTo(energy),"Preserve existing retry policy.");Assert.That(state.Save.gold,Is.EqualTo(gold));
            router.Show(ScreenId.Fail);Settle();yield return null;View().Exit.onClick.Invoke();Settle();yield return null;
            Assert.That(router.Viewport.GetComponentInChildren<ProductionChapterMapView>(),Is.Not.Null);
            router.Show(ScreenId.Fail);Settle();yield return null;View().Settings.onClick.Invoke();Settle();yield return null;
            Assert.That(router.Viewport.Find("Screen_Settings"),Is.Not.Null);
        }
        [UnityTest]public IEnumerator SharedHeaderKeepsLiveBalancesAndAllExistingRoutes()
        {
            Fail(2,130,new[]{1,2});yield return null;
            var v=View();var save=GameState.Instance.Save;
            save.gold=1234567;save.gems=7654;save.energy=2;v.RefreshState();
            Assert.That(v.Header.HasBindings,Is.True);
            Assert.That(v.Header.Balances[0].text,Is.EqualTo("1 234 567"));
            Assert.That(v.Header.Balances[1].text,Is.EqualTo("7 654"));
            Assert.That(v.Header.Balances[2].text,Is.EqualTo("2/"+save.energyMax));
            Assert.That(v.Header.Profile,Is.SameAs(v.Profile));Assert.That(v.Header.Settings,Is.SameAs(v.Settings));
            var destinations=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<destinations.Length;i++)
            {
                router.Show(ScreenId.Fail);Settle();yield return null;v=View();
                var button=i==0?v.Header.Profile:i==1?v.Header.Settings:v.Header.Plus[i-2];
                button.onClick.Invoke();Settle();yield return null;
                Assert.That(router.Viewport.Find("Screen_"+destinations[i]),Is.Not.Null,"Shared header route "+i);
                Assert.That(GameState.Instance.LastFailureResult.Score,Is.EqualTo(130),"Header navigation must retain the failed-round snapshot.");
            }
            Assert.That(save.gold,Is.EqualTo(1234567));Assert.That(save.gems,Is.EqualTo(7654));
        }
        [UnityTest]public IEnumerator RegisteredGameplayFailureCallbackCopiesGoalsBeforeBoardRetirement()
        {
            GameState.Instance.Save.selectedLevel=2;router.Show(ScreenId.Gameplay);Settle();yield return null;
            var board=router.Viewport.GetComponentInChildren<Match3Board>();Assert.That(board,Is.Not.Null);
            var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var tracker=(LevelGoalRuntime)typeof(Match3Board).GetField("goalRuntime",flags).GetValue(board);
            tracker.RestoreSnapshot(new LevelGoalRuntimeSnapshot(new[]{3,4}));
            var callback=(Action<int>)typeof(Match3Board).GetField("onFail",flags).GetValue(board);callback(567);Settle();yield return null;
            var result=GameState.Instance.LastFailureResult;Assert.That(result.Score,Is.EqualTo(567));Assert.That(result.CurrentAt(0),Is.EqualTo(3));Assert.That(result.CurrentAt(1),Is.EqualTo(4));
            Assert.That(View().Goals[0].Count.text,Does.StartWith("3/"));Assert.That(View().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("OutcomeReaction"),Is.Null,"No legacy story overlay over Ivan.");
        }
        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_Fail" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active Fail."); return null;
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
            string directory = Environment.GetEnvironmentVariable("LOR_FAIL_CAPTURE_DIR");
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
            var cameraGo = new GameObject("FailCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionFailView>(),camera,size,safeArea);
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
        void VerifyLayoutAndInput(ProductionFailView v,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect env=Bounds(v.Environment.rectTransform,camera);Assert.That(env.xMin,Is.LessThanOrEqualTo(1));Assert.That(env.yMin,Is.LessThanOrEqualTo(1));Assert.That(env.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(env.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect r=Bounds((RectTransform)button.transform,camera);Assert.That(r.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);Assert.That(r.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);Assert.That(r.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);Assert.That(r.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=r.center},hits);Assert.That(hits.Count,Is.GreaterThan(0),button.name);Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" covered");
            }
            foreach(var t in v.GetComponentsInChildren<Text>())
            {
                if(string.IsNullOrEmpty(t.text))continue;
                if(t.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(t.preferredWidth,Is.LessThanOrEqualTo(t.rectTransform.rect.width+2),t.name+" width");
                Assert.That(t.preferredHeight,Is.LessThanOrEqualTo(t.rectTransform.rect.height+2),t.name+" height");
            }
            Rect retry=Bounds((RectTransform)v.Retry.transform,camera),exit=Bounds((RectTransform)v.Exit.transform,camera);
            Assert.That(retry.Overlaps(exit),Is.False,"Retry and Exit cannot overlap.");
            var art=v.Retry.GetComponent<Image>();Assert.That(retry.width/retry.height,Is.EqualTo(art.sprite.rect.width/art.sprite.rect.height).Within(.001f));
            foreach(var slot in v.Goals)
                if(slot.Root.gameObject.activeSelf){Rect icon=Bounds(slot.Icon.rectTransform,camera),row=Bounds(slot.Root,camera);Assert.That(icon.center.x,Is.EqualTo(row.center.x).Within(.1f));Assert.That(icon.Overlaps(Bounds(slot.Count.rectTransform,camera)),Is.False);}
            Rect goalPanel=Bounds(v.Body.Find("GoalsPanel") as RectTransform,camera);
            foreach(var goal in v.Goals)
                if(goal.Root.gameObject.activeSelf)Assert.That(Bounds(goal.Count.rectTransform,camera).yMin,Is.GreaterThan(goalPanel.yMin+goalPanel.height*.10f),"Goal count needs clearance above bottom carving.");
            foreach(var goal in v.Goals)
                if(goal.Root.gameObject.activeSelf)
                {
                    Rect icon=Bounds(goal.Icon.rectTransform,camera);
                    Assert.That(icon.yMax,Is.LessThan(goalPanel.yMax-goalPanel.height*.10f),"Goal icon must sit below inner frame top.");
                }
            Rect boosters=Bounds(v.Body.Find("BoostersPanel") as RectTransform,camera);
            Rect heading=Bounds(v.BoostersHeading.rectTransform,camera);
            Assert.That(heading.yMax,Is.LessThan(boosters.yMax-boosters.height*.025f),"Booster heading must sit inside the frame.");
            Assert.That(v.PaidOffer.IsInteractable()||v.AdOffer.IsInteractable(),Is.False,"Offer designs must not imply implemented transactions.");
            Rect paid=Bounds((RectTransform)v.PaidOffer.transform,camera),ad=Bounds((RectTransform)v.AdOffer.transform,camera);
            Assert.That(paid.Overlaps(ad)||paid.Overlaps(retry)||ad.Overlaps(retry)||paid.Overlaps(exit)||ad.Overlaps(exit),Is.False);
            Rect mainPanel=Bounds(v.Body.Find("MainPanel") as RectTransform,camera);
            // The return action must clear the lower ornamental band and booster hit areas.
            Assert.That(exit.yMin,Is.GreaterThan(mainPanel.yMin+mainPanel.height*.09f),"Chapter-map action covers footer carving.");
            Assert.That(exit.Overlaps(heading),Is.False,"Return action overlaps booster heading.");
            foreach(var button in v.BoosterButtons)Assert.That(exit.Overlaps(Bounds((RectTransform)button.transform,camera)),Is.False);
            // Native source pixels: green faces share their original y=87..139 band.
            float paidScale=paid.width/328,adScale=ad.width/329;
            float paidFace=paid.yMax-6*paidScale,adFace=ad.yMax-6*adScale;
            float paidGreen=paid.yMax-112*paidScale,adGreen=ad.yMax-112*adScale;
            float tolerance=paid.width/324*3;
            Assert.That(Mathf.Abs(paidFace-adFace),Is.LessThan(tolerance),"Offer upper painted faces misaligned.");
            Assert.That(Mathf.Abs(paidGreen-adGreen),Is.LessThan(tolerance),"Offer green faces misaligned.");
            var price=(RectTransform)v.PaidOffer.transform.Find("Price");
            Rect paidInner=new Rect(paid.xMin+79*paidScale,paid.yMax-139*paidScale,179*paidScale,52*paidScale);
            Rect priceBounds=Bounds(price,camera);
            Assert.That(paidInner.Contains(priceBounds.min)&&paidInner.Contains(priceBounds.max),Is.True,"Live price must fit within original green face.");
            Assert.That(v.PaidOffer.transform.Find("Currency").GetComponent<Image>().enabled,Is.False,"No duplicate of original embedded gem.");
            Assert.That(v.PaidOffer.transform.Find("MovesSymbol").GetComponent<Image>().enabled,Is.False,"No duplicate lightning.");
            Assert.That(v.AdOffer.transform.Find("PlaySymbol").GetComponent<Text>().enabled,Is.False,"No duplicate play triangle.");
            foreach(var offer in new[]{v.PaidOffer,v.AdOffer})
            {
                var image=offer.GetComponent<Image>();var bounds=Bounds((RectTransform)offer.transform,camera);
                Assert.That(offer.GetComponent<ProductionFailPanelFit>().enabled,Is.False);
                Assert.That(image.preserveAspect,Is.True);
                Assert.That(bounds.width/bounds.height,Is.EqualTo(image.sprite.rect.width/image.sprite.rect.height).Within(.001f),"Source card cannot stretch.");
            }
            Assert.That(Bounds(v.AdOfferTitle.rectTransform,camera).Overlaps(Bounds(v.AdOfferStatus.rectTransform,camera)),Is.False,"Ad title/status need separate lines.");
            Rect plaque=Bounds(v.Body.Find("GoalsHeadingPlaque") as RectTransform,camera);
            Rect goalHeading=Bounds(v.GoalsHeading.rectTransform,camera);
            Assert.That(plaque.Contains(goalHeading.min)&&plaque.Contains(goalHeading.max),Is.True,"Goal heading must fit its plaque.");
            Assert.That(plaque.Overlaps(goalPanel),Is.False,"Heading plaque must not cover the goal inset.");
            Rect outerGoals=Bounds(v.Body.Find("GoalsOuterPanel") as RectTransform,camera);
            Assert.That(outerGoals.Contains(goalPanel.min)&&outerGoals.Contains(goalPanel.max),Is.True,"Goal inset must stay inside heading enclosure.");
            foreach(var label in v.BoosterLabels)Assert.That(Bounds(label.rectTransform,camera).yMin,Is.GreaterThan(mainPanel.yMin+mainPanel.height*.055f),"Booster label must clear bottom carving.");
            Assert.That(Root().GetComponentsInChildren<Button>(true).Length,Is.EqualTo(13));
        }
    }
}
