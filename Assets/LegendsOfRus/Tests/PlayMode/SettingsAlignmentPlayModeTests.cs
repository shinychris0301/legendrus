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
            Capture("Settings_360_RU",new Vector2Int(360,640),new Rect(0,20,360,600));
            Capture("Settings_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Settings_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();
            Capture("Settings_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            Capture("Settings_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_360_EN",new Vector2Int(360,640),new Rect(0,20,360,600));
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
            Capture("Settings_Language_360_RU",new Vector2Int(360,640),new Rect(0,20,360,600));
            Capture("Settings_Language_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            v.LanguageButtons[1].onClick.Invoke();Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("en"));Assert.That(v.LanguageValue.text,Is.EqualTo("English"));
            yield return WaitForNormalButtonColors();Capture("Settings_Language_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Settings_Language_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_Language_360_EN",new Vector2Int(360,640),new Rect(0,20,360,600));
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
            Capture("Settings_Language_FromPause_360_EN",new Vector2Int(360,640),new Rect(0,20,360,600));
            v.CloseLanguageButton.onClick.Invoke();Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Settings));Assert.That(v.InfoPanel.activeSelf,Is.False);
            v.ProfileButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);router.NavigateBack();
            yield return WaitForNormalButtonColors();Capture("Settings_FromPause_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Settings_FromPause_360_EN",new Vector2Int(360,640),new Rect(0,20,360,600));
            Assert.That(board.CanAcceptPointerInput,Is.False);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(game.Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.Score,Is.EqualTo(score));
            router.Viewport.GetComponentInChildren<ProductionPauseView>().Actions[0].onClick.Invoke();Settle();yield return null;Assert.That(board.CanAcceptPointerInput,Is.True);
        }
        [UnityTest] public IEnumerator SharedHeaderAndPressFeedbackPreservePreferences()
        {
            var v=View();string save=StableSave();
            Assert.That(v.GetComponentsInChildren<ProductionPlayerHeader>().Length,Is.EqualTo(1));
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(v.Header.Backing.sprite,Is.SameAs(home.Header.Backing.sprite));Assert.That(v.Header.Avatar.sprite,Is.SameAs(home.Header.Avatar.sprite));
            foreach(var button in v.GetComponentsInChildren<Button>(true))Assert.That(button.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null,button.name);
            yield return WaitForNormalButtonColors();
            var target=v.Rows[0];var initial=((RectTransform)target.transform).anchoredPosition;
            var pointer=new PointerEventData(EventSystem.current){pointerId=42,button=PointerEventData.InputButton.Left};
            ExecuteEvents.Execute(target.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            Assert.That(target.GetComponent<ProductionMapButtonFeedback>().IsPressed,Is.True);
            Capture("Settings_PressedLanguage_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            ExecuteEvents.Execute(target.gameObject,pointer,ExecuteEvents.pointerUpHandler);
            Assert.That(target.GetComponent<ProductionMapButtonFeedback>().IsPressed,Is.False);
            Assert.That(((RectTransform)target.transform).anchoredPosition,Is.EqualTo(initial));Assert.That(StableSave(),Is.EqualTo(save));
        }

        [UnityTest] public IEnumerator NativeExpandedControlsTallModalsAndOriginReturn()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.Inventory);Settle();yield return null;
                router.Show(ScreenId.Settings);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration;
                Capture("Settings_SwitchTargets_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    for(int i=0;i<4;i++)
                    {
                        var button=View().SwitchButtons[i];bool before=View().Preference(i);Rect art=Bounds(button.image.rectTransform,camera);
                        var point=new Vector2(art.center.x,art.yMax+1);
                        Click(button,camera,point);Assert.That(View().Preference(i),Is.EqualTo(!before));
                        var persisted=SaveService.Load();bool[] actual={persisted.music,persisted.sfx,persisted.vibration,persisted.notifications};Assert.That(actual[i],Is.EqualTo(!before));
                        Click(button,camera,point);Assert.That(View().Preference(i),Is.EqualTo(before));
                    }
                    Click(View().Rows[0],camera);
                });
                Assert.That(View().InfoPanel.activeSelf&&View().LanguageChoices.activeSelf,Is.True);yield return WaitForNormalButtonColors();
                Capture("Settings_TallLanguage_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().LanguageButtons[language=="ru"?1:0],camera));
                Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo(language=="ru"?"en":"ru"));yield return WaitForNormalButtonColors();
                Capture("Settings_LanguageCloseTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var button=View().CloseLanguageButton;Rect art=Bounds(button.image.rectTransform,camera);Click(button,camera,new Vector2(art.xMax+1,art.center.y));
                });
                Assert.That(View().InfoPanel.activeSelf,Is.False);yield return WaitForNormalButtonColors();
                Capture("Settings_SupportTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().Rows[3],camera));
                Assert.That(View().InfoPanel.activeSelf,Is.True);yield return WaitForNormalButtonColors();
                Capture("Settings_TallSupport_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("Settings_SupportRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().CloseInfoButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.False);yield return WaitForNormalButtonColors();
                Capture("Settings_ReturnToInventory_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().BackButton,camera));
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Inventory));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest] public IEnumerator SmallPortraitExplanationsKeepTextInsideTheirPanel()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);View().RefreshContent();
                foreach(int kind in new[]{1,2,3,4,5,6})
                {
                    View().ShowInfo(kind);yield return WaitForNormalButtonColors();
                    Capture("Settings_Info_"+kind+"_360_"+language,new Vector2Int(360,640),new Rect(0,20,360,600),camera=>
                    {
                        var v=View();Rect panel=Bounds((RectTransform)v.GeneralInfoContent.transform.Find("Frame"),camera);
                        foreach(var text in new[]{v.InfoHeading,v.InfoDescription,v.CloseInfoLabel})
                        {
                            foreach(Vector3 vertex in text.canvasRenderer.GetMesh().vertices)
                                Assert.That(panel.Contains(RectTransformUtility.WorldToScreenPoint(camera,text.transform.TransformPoint(vertex))),Is.True,text.name+" outside explanation panel");
                        }
                        Assert.That(Bounds(v.InfoDescription.rectTransform,camera).Overlaps(Bounds(v.CloseInfoButton.image.rectTransform,camera)),Is.False);
                        Click(v.CloseInfoButton,camera);
                    });
                    Assert.That(View().InfoPanel.activeSelf,Is.False);
                }
            }
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        static void Click(Button button,Camera camera,Vector2? point=null)
        {
            var pointer=new PointerEventData(EventSystem.current){position=point??Bounds((RectTransform)button.transform,camera).center};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
            ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
        }
        static Rect TouchBounds(Button button,Camera camera,float scale)
        {
            Rect art=Bounds(button.image.rectTransform,camera);scale=art.width/button.image.rectTransform.rect.width;Vector4 p=button.image.raycastPadding;
            return Rect.MinMaxRect(art.xMin+p.x*scale,art.yMin+p.y*scale,art.xMax-p.z*scale,art.yMax-p.w*scale);
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
                    Color expected=button.transition==Selectable.Transition.None?Color.white:(button.IsInteractable()?button.colors.normalColor:button.colors.disabledColor)*button.colors.colorMultiplier;
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
                VerifyLayoutAndInput(View(),camera,size,safeArea);
                inspect?.Invoke(camera);
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
            var hud=Bounds(v.Hud,camera);Assert.That(hud.yMax,Is.EqualTo(safeArea.yMax).Within(1),"Shared header follows safe top edge.");
            float bodyScale=Bounds((RectTransform)v.BackButton.transform,camera).width/151f;
            Assert.That(Bounds((RectTransform)v.BackButton.transform,camera).yMin,Is.EqualTo(safeArea.yMin+27*bodyScale).Within(1),"Back follows safe bottom edge.");
            Assert.That(Bounds(v.VersionText.rectTransform,camera).yMin,Is.GreaterThanOrEqualTo(safeArea.yMin));
            var paperImage=v.Body.Find("PreferencesPaper").GetComponent<Image>();
            Assert.That(paperImage.type,Is.EqualTo(Image.Type.Sliced));Assert.That(paperImage.preserveAspect,Is.False);
            Rect paper=Bounds(paperImage.rectTransform,camera);
            for(int i=0;i<4;i++)
            {
                foreach(var rect in new[]{Bounds(v.SwitchLabels[i].rectTransform,camera),Bounds(v.SwitchButtons[i].image.rectTransform,camera)})
                    Assert.That(paper.Contains(rect.min)&&paper.Contains(rect.max),Is.True,"Preference rows must stay on the parchment surface");
            }
            var targets=new List<Button>();targets.AddRange(v.SwitchButtons);
            if(v.LanguageChoices.activeInHierarchy){targets.Add(v.CloseLanguageButton);targets.AddRange(v.LanguageButtons);}
            foreach(var button in targets)
            {
                Rect target=TouchBounds(button,camera,bodyScale);
                Assert.That(Mathf.Min(target.width,target.height),Is.GreaterThanOrEqualTo(44),button.name+" touch target at tested phone size");
                Assert.That(safeArea.Contains(target.min)&&safeArea.Contains(target.max),Is.True);
                if(!v.InfoPanel.activeSelf||button.transform.IsChildOf(v.LanguageChoices.transform))
                {
                    var points=button==v.CloseLanguageButton?new[]{new Vector2(target.xMin+1,target.center.y),new Vector2(target.xMax-1,target.center.y)}:new[]{new Vector2(target.center.x,target.yMin+1),new Vector2(target.center.x,target.yMax-1)};
                    foreach(var point in points)
                    {
                        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                        Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                    }
                }
            }
            for(int i=0;i<3;i++)Assert.That(TouchBounds(v.SwitchButtons[i],camera,bodyScale).yMin,Is.GreaterThan(TouchBounds(v.SwitchButtons[i+1],camera,bodyScale).yMax),"Preference touch targets remain separate");
            foreach(var image in new[]{v.BackButton.image,v.CloseLanguageButton.image,v.LanguagePanel})
            {
                Assert.That(image.type,Is.EqualTo(Image.Type.Simple));Assert.That(image.preserveAspect,Is.True);
                Assert.That(image.sprite.rect.size,Is.EqualTo(new Vector2(image.sprite.texture.width,image.sprite.texture.height)),"Whole approved ornament retained");
            }
            if(v.InfoPanel.activeSelf)
            {
                var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                foreach(var point in new[]{Bounds(v.BackButton.image.rectTransform,camera).center,Bounds(v.SwitchButtons[0].image.rectTransform,camera).center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Modal blocks full safe height and underlying preferences");
                }
            }
            var carved=Bounds((RectTransform)v.Body.Find("CarvedFrame"),camera);
            Assert.That(carved.yMin,Is.EqualTo(safeArea.yMin+25*bodyScale).Within(1),"Frame arch follows Back at the bottom.");
            for(int i=0;i<4;i++)Assert.That(Bounds(v.SwitchLabels[i].rectTransform,camera).xMax,Is.LessThan(Bounds((RectTransform)v.SwitchButtons[i].transform,camera).xMin),"Preference label covers toggle.");
            for(int i=0;i<2;i++){
                var value=i==0?v.LanguageValue:v.CloudValue;
                Assert.That(Bounds(v.RowLabels[i].rectTransform,camera).xMax,Is.LessThan(Bounds(value.rectTransform,camera).xMin),"Row label covers value.");
                Assert.That(Bounds(value.rectTransform,camera).xMax,Is.LessThan(Bounds((RectTransform)v.Rows[i].transform.Find("Arrow"),camera).xMin),"Value covers arrow.");
            }
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,button.name+" outside safe area: "+rect+" safe: "+safeArea);
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
            foreach(var label in new[]{v.CloudValue,v.SignOutNote,v.VersionText,v.InfoDescription})
                if(label.gameObject.activeInHierarchy)
                {
                    float used=label.resizeTextForBestFit?label.cachedTextGenerator.fontSizeUsedForBestFit/label.pixelsPerUnit:label.fontSize;
                    Assert.That(used*Bounds(label.rectTransform,camera).height/label.rectTransform.rect.height,Is.GreaterThanOrEqualTo(9.2f),label.name+" small-portrait caption font");
                }
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
