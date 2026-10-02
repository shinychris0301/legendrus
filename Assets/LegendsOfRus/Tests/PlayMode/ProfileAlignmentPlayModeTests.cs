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
    public sealed class ProfileAlignmentPlayModeTests
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
            router.Show(ScreenId.Profile);Settle();yield return null;yield return null;
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

        ProductionProfileView View()=>router.Viewport.GetComponentInChildren<ProductionProfileView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(v.GoldText.text,Is.EqualTo(ProductionProfileView.Number(before+17)));string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("Profile_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Profile_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("Profile_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Profile_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.Home,ScreenId.IndividualRanking,ScreenId.TeamHub,ScreenId.Inventory,ScreenId.Login};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.Profile);Settle();yield return null;var v=View();Button b;if(i<5)b=new[]{v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton}[i];else{v.ShowInfo();b=v.RouteButtons[i-5];}int generation=router.RouteGeneration;b.onClick.Invoke();b.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator CollectionsAndOwnedItemsNeverGrantPreviewRewards(){
            var v=View();Assert.That(v.HasOnlineProfile,Is.False);GameState.Instance.Save.boosterMace=7;GameState.Instance.Save.boosterSeaPearl=13;string save=StableSave();v.Tabs[1].onClick.Invoke();Assert.That(v.SelectedTab,Is.EqualTo(ProfileTab.Relics));Assert.That(v.DynamicRoot.Find("Hero_0"),Is.Null);v.DynamicRoot.Find("Relic_5").GetComponent<Button>().onClick.Invoke();Assert.That(v.SelectedRelic,Is.EqualTo(5));yield return WaitForNormalButtonColors();Capture("Profile_Relics_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Tabs[2].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Quantity_0").GetComponent<Text>().text,Is.EqualTo("7"));Assert.That(v.DynamicRoot.Find("Quantity_3").GetComponent<Text>().text,Is.EqualTo("13"));yield return WaitForNormalButtonColors();Capture("Profile_Items_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_Items_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.Tabs[3].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Hero_4"),Is.Not.Null);Assert.That(StableSave(),Is.EqualTo(save));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectTab((ProfileTab)9));
        }
        [UnityTest]public IEnumerator RealProgressMilestonesAndPagedHistory(){
            var v=View();GameState.Instance.Save.levelProgress.Clear();v.RefreshState();v.FooterButtons[2].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Empty"),Is.Not.Null);yield return WaitForNormalButtonColors();Capture("Profile_EmptyHistory_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            for(int i=1;i<=8;i++)GameState.Instance.Save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=i,bestStars=i%3+1,bestScore=i*1200,attempts=i,completions=1});
            string save=StableSave();v.RefreshState();Assert.That(v.CompletedCount,Is.EqualTo(8));Assert.That(v.BestScore,Is.EqualTo(9600));Assert.That(v.AttemptCount,Is.EqualTo(36));Assert.That(v.ChapterCompleted,Is.EqualTo(8));Assert.That(v.ProgressFill.preserveAspect,Is.False,"Progress fill must span the full track rather than letterbox the source tab sprite.");Assert.That(v.ProgressFill.fillAmount,Is.EqualTo(8f/v.ChapterTotal).Within(.0001));yield return WaitForNormalButtonColors();Capture("Profile_History_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.DynamicRoot.Find("Next").GetComponent<Button>().onClick.Invoke();Assert.That(v.HistoryPage,Is.EqualTo(1));v.ChangeHistoryPage(100);Assert.That(v.HistoryPage,Is.EqualTo(1));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_HistoryPage2_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.FooterButtons[1].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("Profile_Achievements_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.That(v.DynamicRoot.Find("MilestoneStatus_0").GetComponent<Text>().text,Is.EqualTo("REACHED"));Assert.That(v.DynamicRoot.Find("MilestoneStatus_2").GetComponent<Text>().text,Is.EqualTo("NOT YET REACHED"));v.ProfileButton.onClick.Invoke();Assert.That(v.Section,Is.EqualTo(ProfileSection.Profile));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator IdentityExplanationBlocksUnderlyingInputAndBackDismisses(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;v.EditButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.SettingsButton.onClick.Invoke();v.Tabs[2].onClick.Invoke();v.SelectTab(ProfileTab.Items);v.SelectSection(ProfileSection.History);v.SelectRelic(3);Assert.That(v.Section,Is.EqualTo(ProfileSection.Profile));Assert.That(v.SelectedTab,Is.EqualTo(ProfileTab.Heroes));Assert.That(v.SelectedRelic,Is.Zero);Assert.That(router.RouteGeneration,Is.EqualTo(generation));yield return WaitForNormalButtonColors();Capture("Profile_Info_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Profile));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator MaximumBalancesAndRepeatedBind(){
            var v=View();var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.levelProgress=new List<LevelProgressData>{new LevelProgressData{chapterId=1,levelId=1,attempts=int.MaxValue,bestScore=int.MaxValue},new LevelProgressData{chapterId=1,levelId=2,attempts=int.MaxValue}};v.RefreshState();Assert.That(v.AttemptCount,Is.EqualTo(4294967294L));yield return WaitForNormalButtonColors();Capture("Profile_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));int calls=0;v.Bind(id=>calls++,()=>calls++);v.Bind(id=>calls++,()=>calls++);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));
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
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_PROFILE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ProfileCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionProfileView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionProfileView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(safeArea.Contains(rect.min)&&safeArea.Contains(rect.max),Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" hit "+hits[0].gameObject.name);

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name+" visible glyph mesh");
                    float height=text.preferredHeight;
                    if(text.resizeTextForBestFit)
                    {
                        float used=text.cachedTextGenerator.fontSizeUsedForBestFit/text.pixelsPerUnit;
                        Assert.That(used,Is.InRange(text.resizeTextMinSize-1f,text.resizeTextMaxSize+1f),text.name+" fitted font size");
                        var settings=text.GetGenerationSettings(text.rectTransform.rect.size);settings.resizeTextForBestFit=false;settings.fontSize=Mathf.RoundToInt(used);
                        height=new TextGenerator().GetPreferredHeight(text.text,settings)/text.pixelsPerUnit;
                    }
                    Assert.That(height,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" actual fitted text height");
                }
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
