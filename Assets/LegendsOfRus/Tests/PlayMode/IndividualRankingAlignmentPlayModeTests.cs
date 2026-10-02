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
    public sealed class IndividualRankingAlignmentPlayModeTests
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
            router.Show(ScreenId.IndividualRanking);Settle();yield return null;yield return null;
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

        ProductionIndividualRankingView View()=>router.Viewport.GetComponentInChildren<ProductionIndividualRankingView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveHeaderLocalizationAndPortraitCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(router.Viewport.GetComponentsInChildren<AnimatedHotspot>(),Is.Empty);
            int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("IndividualRanking_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("IndividualRanking_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("IndividualRanking_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().PreviewLabel.text,Does.Contain("SAMPLE STANDINGS"));Assert.That(View().PlayerStatus.gameObject.activeSelf,Is.False);
            Capture("IndividualRanking_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            Capture("IndividualRanking_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("IndividualRanking_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            DemoFeatureState.SetLanguage("ru");yield return null;View().RefreshContent();
            Assert.That(View().Heading.enabled,Is.False);Assert.That(View().Title.sprite,Is.SameAs(View().OriginalRussianTitle));
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator TabsAndRepeatedBindingsKeepPreviewReadOnly()
        {
            string save=StableSave();bool claimed=DemoFeatureState.EventRewardClaimed;var v=View();int routes=0;
            v.Bind(id=>routes++,()=>routes++);v.Bind(id=>routes++,()=>routes++);
            Assert.That(v.HasLiveLeaderboard,Is.False);Assert.That(v.PreviewLabel.text,Does.Contain("ПРИМЕР"));
            Assert.That(v.Scores[0].text,Is.EqualTo("1 985 300"));Assert.That(v.Scores[10].text,Is.EqualTo("512 430"));
            for(int i=0;i<3;i++){
                v.Tabs[i].onClick.Invoke();Assert.That(v.Period,Is.EqualTo((IndividualRankingPeriod)i));
                foreach(var row in v.Rows)Assert.That(row.gameObject.activeSelf,Is.True);
                Assert.That(v.Tabs[i].image.sprite.name,Is.EqualTo("tab_active"));
            }
            v.Tabs[0].onClick.Invoke();yield return WaitForNormalButtonColors();
            Capture("IndividualRanking_Day_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();v.Tabs[1].onClick.Invoke();
            Capture("IndividualRanking_Week_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(routes,Is.Zero);Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.EventRewardClaimed,Is.EqualTo(claimed));
            v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(routes,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator NativeHeaderRoutesOnce()
        {
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++){
                router.Show(ScreenId.IndividualRanking);Settle();yield return null;var v=View();var button=i==0?v.ProfileButton:i==1?v.SettingsButton:v.PlusButtons[i-2];int generation=router.RouteGeneration;
                button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
        }
        [UnityTest]public IEnumerator ProfileShortcutAndNativeBackKeepExistingDestinations()
        {
            router.Show(ScreenId.Profile);Settle();yield return null;
            string save=StableSave();int generation=router.RouteGeneration;
            var profile=router.Viewport.GetComponentInChildren<ProductionProfileView>();
            Assert.That(profile,Is.Not.Null);Assert.That(profile.HasRequiredBindings,Is.True);
            profile.ShowInfo();Assert.That(profile.InfoPanel.activeSelf,Is.True);
            var shortcut=profile.RouteButtons[0];shortcut.onClick.Invoke();shortcut.onClick.Invoke();
            Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.IndividualRanking));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            var back=View().BackButton;back.onClick.Invoke();back.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+2));
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator MaximumBalancesStayVisibleWithoutBecomingRankScores()
        {
            var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.energy=save.energyMax;View().RefreshState();
            Assert.That(View().GoldText.text,Is.EqualTo("2 147 483 647"));Assert.That(View().GemsText.text,Is.EqualTo("2 147 483 647"));
            Assert.That(View().PlayerScore.text,Is.EqualTo("512 430"));Assert.That(View().Scores[0].text,Is.EqualTo("1 985 300"));
            yield return WaitForNormalButtonColors();Capture("IndividualRanking_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [UnityTest]public IEnumerator RulesOverlayBlocksUnderlyingControlsAndBackClosesItFirst()
        {
            var v=View();int generation=router.RouteGeneration;string save=StableSave();var period=v.Period;
            v.RulesButton.onClick.Invoke();Assert.That(v.RulesPanel.activeSelf,Is.True);
            v.ProfileButton.onClick.Invoke();v.Tabs[0].onClick.Invoke();v.BackButton.onClick.Invoke();
            Assert.That(v.Period,Is.EqualTo(period));Assert.That(router.RouteGeneration,Is.EqualTo(generation));
            yield return WaitForNormalButtonColors();Capture("IndividualRanking_Rules_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();
            Capture("IndividualRanking_Rules_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            v.CloseRulesButton.onClick.Invoke();Assert.That(v.RulesPanel.activeSelf,Is.False);
            v.RulesButton.onClick.Invoke();router.NavigateBack();Assert.That(v.RulesPanel.activeSelf,Is.False);
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.IndividualRanking));Assert.That(StableSave(),Is.EqualTo(save));
            router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
        }
        [UnityTest]public IEnumerator TallPeriodChangesAndRulesNativeClosePreservePreview()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);View().RefreshContent();
                foreach(int tab in new[]{0,1,2})
                {
                    View().Tabs[tab].onClick.Invoke();yield return WaitForNormalButtonColors();
                    Capture("IndividualRanking_TallPeriod_"+tab+"_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172));
                }
                View().RulesButton.onClick.Invoke();yield return WaitForNormalButtonColors();
                Capture("IndividualRanking_TallRules_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>
                {
                    var close=View().CloseRulesButton;
                    var pointer=new PointerEventData(EventSystem.current){position=Bounds((RectTransform)close.transform,camera).center};
                    ExecuteEvents.Execute(close.gameObject,pointer,ExecuteEvents.pointerClickHandler);
                });
                Assert.That(View().RulesPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.IndividualRanking));
                yield return WaitForNormalButtonColors();
                Capture("IndividualRanking_TallRulesClosed_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172));
            }
            Assert.That(StableSave(),Is.EqualTo(save));
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
            var fontsView=View();
            Assert.That(fontsView.Heading.font,Is.SameAs(fontsView.HeadingFont));
            bool english=DemoFeatureState.LanguageCode=="en";
            Assert.That(fontsView.Heading.enabled,Is.EqualTo(english),"No live Russian text over the source lettering");
            Assert.That(fontsView.Title.sprite,Is.SameAs(english?ProductionIndividualRankingReferenceLayout.Load("title"):fontsView.OriginalRussianTitle));
            Assert.That(fontsView.HeroName.font,Is.SameAs(fontsView.PlayerNamesFont));
            Assert.That(fontsView.Header.HasBindings,Is.True);
            Assert.That(fontsView.RulesButtonText.enabled,Is.EqualTo(english));
            Assert.That(fontsView.RulesButton.image.sprite.name,Is.EqualTo(english?"rules":"rules_original_ru"));
            foreach(var label in fontsView.Names)Assert.That(label.font,Is.SameAs(fontsView.PlayerNamesFont));
            foreach(var label in fontsView.Ranks)Assert.That(label.font,Is.SameAs(fontsView.RankNumbersFont));

            string directory = Environment.GetEnvironmentVariable("LOR_RANKING_CAPTURE_DIR");
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
            var cameraGo = new GameObject("IndividualRankingCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionIndividualRankingView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionIndividualRankingView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect hud=Bounds(v.Hud,camera);float unit=hud.width/941f;
            Assert.That(hud.yMax,Is.EqualTo(safeArea.yMax).Within(1),"Shared header pins to safe top");
            Rect footer=Bounds((RectTransform)v.BackButton.transform,camera);
            Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin+20*unit).Within(1),"Back control pins to safe bottom");
            Assert.That(Bounds(v.Header.Avatar.rectTransform,camera).yMin,Is.GreaterThan(Bounds(v.Tabs[0].image.rectTransform,camera).yMax),"Hanging portrait crest remains above tabs");
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            for(int i=0;i<14;i++)if(v.Rows[i].gameObject.activeSelf){
                Rect row=Bounds(v.Rows[i],camera);Assert.That(safeArea.Contains(row.min)&&safeArea.Contains(row.max),Is.True);
                if(i<13)Assert.That(row.yMin,Is.GreaterThanOrEqualTo(Bounds(v.Rows[i+1],camera).yMax-.1f));
                Assert.That(Bounds(v.Names[i].rectTransform,camera).xMax,Is.LessThan(Bounds(v.Scores[i].rectTransform,camera).xMin));
                var reward=v.RowRewards[i];Rect bounds=Bounds(reward.rectTransform,camera);
                Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(row.yMin+3.9f*unit),"Reward clears lower row rail");
                Assert.That(bounds.yMax,Is.LessThanOrEqualTo(row.yMax-3.9f*unit),"Reward clears upper row rail");
                Assert.That(bounds.xMin,Is.GreaterThan(Bounds(v.Scores[i].rectTransform,camera).xMax));
                Assert.That(bounds.xMax,Is.LessThan(row.xMax));
                Assert.That(reward.preserveAspect,Is.True);Assert.That(reward.type,Is.EqualTo(Image.Type.Simple));
                Assert.That(reward.rectTransform.rect.width/reward.rectTransform.rect.height,Is.EqualTo(reward.sprite.rect.width/reward.sprite.rect.height).Within(.0001f));
                Assert.That(reward.sprite.rect.size,Is.EqualTo(new Vector2(reward.sprite.texture.width,reward.sprite.texture.height)),"Whole reward sprite retained");

            }
            Assert.That(Bounds(v.Rows[9],camera).yMin,Is.GreaterThan(Bounds(v.PlayerRow,camera).yMax));
            Assert.That(Bounds(v.Rows[13],camera).yMin,Is.GreaterThan(Bounds((RectTransform)v.RulesButton.transform,camera).yMax));
            var carving=v.Body.Find("CarvedFooter").GetComponent<Image>();
            Assert.That(Bounds(carving.rectTransform,camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            Rect rules=Bounds((RectTransform)v.RulesButton.transform,camera);
            Assert.That(rules.yMin,Is.EqualTo(safeArea.yMin+43*unit).Within(.5f));
            Assert.That(footer.xMax,Is.LessThan(rules.xMin),"Footer controls do not overlap");
            foreach(var control in new[]{v.BackButton,v.RulesButton})
            {
                Rect target=Bounds((RectTransform)control.transform,camera);
                Assert.That(Mathf.Min(target.width,target.height),Is.GreaterThanOrEqualTo(44),"Footer target at tested phone size");
                Assert.That(control.image.type,Is.EqualTo(Image.Type.Simple));Assert.That(control.image.preserveAspect,Is.True);
                Assert.That(control.image.sprite.rect.size,Is.EqualTo(new Vector2(control.image.sprite.texture.width,control.image.sprite.texture.height)));
            }
            if(v.RulesPanel.activeSelf)
            {
                var shield=(RectTransform)v.RulesPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                foreach(var point in new[]{new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2),footer.center,rules.center})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Tall modal blocks underlying navigation");
                }
            }
            Assert.That(v.PlayerRank.text,Is.EqualTo("12"));Assert.That(v.PlayerScore.text,Is.EqualTo("512 430"));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.RulesPanel.activeSelf&&button!=v.CloseRulesButton)continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                // Rect.Contains excludes the maximum edge; an edge-anchored header legitimately touches it.
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.1f),button.name+" left");
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.1f),button.name+" bottom");
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.1f),button.name+" right");
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.1f),button.name+" top");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
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
