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
    public sealed class SettingsAlignmentPlayModeTests
    {
        ScreenRouter router;
        string previousSave, previousLanguage;
        readonly string[] keys = { "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging", LanguagePreference.Key };
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
            router.Show(ScreenId.Settings);Settle();yield return null;yield return null;
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

        ProductionSettingsView View()=>router.Viewport.GetComponentInChildren<ProductionSettingsView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}

        [UnityTest] public IEnumerator LiveHeaderAndLocalizedPhoneLayouts()
        {
            var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);
            Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);
            GameState.Instance.AddGold(17);yield return null;Assert.That(v.GoldText.text,Is.EqualTo(ProductionInventoryView.Number(GameState.Instance.Save.gold)));
            Assert.That(v.VersionText.text,Does.Contain(Application.version));string save=StableSave();
            yield return WaitForNormalButtonColors();Capture("Settings_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Settings_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();
            Capture("Settings_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest] public IEnumerator PreferencesPersistIndependentlyAndRefreshOnReturn()
        {
            var v=View();int coins=GameState.Instance.Save.gold,gems=GameState.Instance.Save.gems;var service=CoreLoopFeedbackService.Instance;
            for(int i=0;i<4;i++){
                v.SwitchButtons[i].onClick.Invoke();var saved=SaveService.Load();bool[] actual={saved.music,saved.sfx,saved.vibration,saved.notifications};
                for(int j=0;j<4;j++)Assert.That(actual[j],Is.EqualTo(j>i));Assert.That(v.Preference(i),Is.False);Assert.That(v.SwitchButtons[i].image.sprite.name,Is.EqualTo("switch_off"));
            }
            if(service!=null){Assert.That(service.MusicEnabled||service.SfxEnabled||service.VibrationEnabled,Is.False);}
            yield return WaitForNormalButtonColors();Capture("Settings_AllOff_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Settings);Settle();yield return null;v=View();for(int i=0;i<4;i++)Assert.That(v.Preference(i),Is.False);
            GameState.Instance.ToggleMusic();yield return null;Assert.That(v.SwitchButtons[0].image.sprite.name,Is.EqualTo("switch_on"));
            v.SwitchButtons[2].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("Settings_Mixed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Assert.That(GameState.Instance.Save.gold,Is.EqualTo(coins));Assert.That(GameState.Instance.Save.gems,Is.EqualTo(gems));
            bool before=v.Preference(1);v.Bind(id=>{},()=>{},c=>{});v.Bind(id=>{},()=>{},c=>{});v.SwitchButtons[1].onClick.Invoke();Assert.That(v.Preference(1),Is.EqualTo(!before));
        }
        [UnityTest] public IEnumerator LanguageAndUnavailableServicesPreserveProgress()
        {
            var v=View();string save=StableSave();var consent=typeof(PrivacySupportService).GetProperty("State").GetValue(PrivacySupportService.Instance);int generation=router.RouteGeneration;
            v.Rows[0].onClick.Invoke();Assert.That(v.InfoPanel.activeSelf&&v.LanguageChoices.activeSelf,Is.True);v.SwitchButtons[0].onClick.Invoke();Assert.That(v.Preference(0),Is.True);
            yield return WaitForNormalButtonColors();Capture("Settings_Language_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_Language_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            v.LanguageButtons[1].onClick.Invoke();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("en"));Assert.That(v.LanguageValue.text,Is.EqualTo("English"));
            yield return WaitForNormalButtonColors();Capture("Settings_Language_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Settings_Language_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Assert.That(v.LanguageButtons[1].image.sprite.name,Is.EqualTo("language_selected"));Assert.That(v.LanguageButtons[0].image.sprite.name,Is.EqualTo("language_unselected"));
            v.CloseLanguageButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Settings));
            v.Rows[0].onClick.Invoke();v.LanguageButtons[0].onClick.Invoke();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));
            Assert.That(v.LanguageButtons[0].image.sprite.name,Is.EqualTo("language_selected"));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);
            for(int i=1;i<6;i++){if(i==2)continue;v.Rows[i].onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);Assert.That(v.LanguageChoices.activeSelf,Is.False);v.BackButton.onClick.Invoke();v.ProfileButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));if(i==1){yield return WaitForNormalButtonColors();Capture("Settings_Cloud_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}v.CloseInfoButton.onClick.Invoke();}
            Assert.That(typeof(PrivacySupportService).GetProperty("State").GetValue(PrivacySupportService.Instance),Is.SameAs(consent));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest] public IEnumerator BackReturnsToOriginAndHeaderRoutesCommitOnce()
        {
            foreach(var origin in new[]{ScreenId.Home,ScreenId.Profile,ScreenId.Inventory,ScreenId.SplashTitle}){
                router.Show(origin);Settle();yield return null;router.Show(ScreenId.Settings);Settle();yield return null;
                var v=View();int generation=router.RouteGeneration;v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(origin));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
            var targets=new[]{ScreenId.Profile,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<4;i++){router.Show(ScreenId.Settings);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2]};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));if(i==1||i==2)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==1?"coins":"gems"));}
        }
        [UnityTest] public IEnumerator PausedBoardSurvivesSettingsLanguageAndBack()
        {
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;yield return null;
            var game=router.Viewport.GetComponentInChildren<ProductionGameplayView>();var board=game.Board;string hash=board.BoardStateHash;int moves=board.Moves,score=board.Score;
            game.PauseButton.onClick.Invoke();Settle();yield return null;
            router.Viewport.GetComponentInChildren<ProductionPauseView>().Actions[2].onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Settings));var v=View();Assert.That(v.ActionsBound,Is.True);
            v.SwitchButtons[0].onClick.Invoke();Assert.That(v.Preference(0),Is.False);
            v.Rows[0].onClick.Invoke();v.LanguageButtons[1].onClick.Invoke();
            yield return WaitForNormalButtonColors();Capture("Settings_Language_FromPause_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.CloseLanguageButton.onClick.Invoke();Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Settings));Assert.That(v.InfoPanel.activeSelf,Is.False);
            v.ProfileButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);router.NavigateBack();
            yield return WaitForNormalButtonColors();Capture("Settings_FromPause_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Assert.That(board.CanAcceptPointerInput,Is.False);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(game.Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.Score,Is.EqualTo(score));
            router.Viewport.GetComponentInChildren<ProductionPauseView>().Actions[0].onClick.Invoke();Settle();yield return null;Assert.That(board.CanAcceptPointerInput,Is.True);
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
            Assert.That(View().transform.parent.GetComponentsInChildren<Button>().Length,Is.EqualTo(View().GetComponentsInChildren<Button>().Length),"No legacy shortcut controls outside the native page");
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_SETTINGS_CAPTURE_DIR");
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
            var cameraGo = new GameObject("SettingsCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionSettingsView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionSettingsView v,Camera camera,Vector2Int size,Rect safeArea)
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
