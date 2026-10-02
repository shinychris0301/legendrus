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
    public sealed class TeamRankingAlignmentPlayModeTests
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
            router.Show(ScreenId.TeamRanking);Settle();yield return null;yield return null;
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

        ProductionTeamRankingView View()=>router.Viewport.GetComponentInChildren<ProductionTeamRankingView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("TeamRanking_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamRanking_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("TeamRanking_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;Assert.That(View().PreviewLabel.text,Does.Contain("SAMPLE"));Capture("TeamRanking_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Capture("TeamRanking_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamRanking_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.TeamHub};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.TeamRanking);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator FiltersKeepExamplesReadOnlyAndBindingsDoNotDuplicate(){
            var v=View();string save=StableSave();int routes=0;v.Bind(id=>routes++,()=>routes++);v.Bind(id=>routes++,()=>routes++);
            Assert.That(v.HasLiveLeaderboard,Is.False);v.Tabs[0].onClick.Invoke();v.Tabs[2].onClick.Invoke();Assert.That(v.Scope,Is.EqualTo(TeamRankingScope.Local));Assert.That(v.Period,Is.EqualTo(TeamRankingPeriod.Week));Assert.That(v.Tabs[0].image.sprite.name,Is.EqualTo("tab_0_active_r6"));Assert.That(v.Tabs[2].image.sprite.name,Is.EqualTo("tab_2_active_r6"));
            Assert.That(v.PodiumScores[0].text,Is.EqualTo("125 680"));Assert.That(v.Scores[3].text,Is.EqualTo("34 560"));yield return WaitForNormalButtonColors();Capture("TeamRanking_LocalWeek_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.Tabs[1].onClick.Invoke();v.Tabs[3].onClick.Invoke();Assert.That(v.Scope,Is.EqualTo(TeamRankingScope.Global));Assert.That(v.Period,Is.EqualTo(TeamRankingPeriod.Season));Assert.That(v.PodiumScores[0].text,Is.EqualTo("125 680"));Assert.That(StableSave(),Is.EqualTo(save));Assert.That(routes,Is.Zero);Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectPeriod((TeamRankingPeriod)4));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectScope((TeamRankingScope)4));v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(routes,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator DetailsPreviewBlocksBackgroundAndBackClosesFirst(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;
            v.DetailsButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.ProfileButton.onClick.Invoke();v.BackButton.onClick.Invoke();v.Tabs[0].onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(v.Scope,Is.EqualTo(TeamRankingScope.Global));
            yield return WaitForNormalButtonColors();Capture("TeamRanking_Details_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();Capture("TeamRanking_Details_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.CloseInfoButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.False);v.DetailsButton.onClick.Invoke();router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamRanking));Assert.That(StableSave(),Is.EqualTo(save));router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));
        }
        [UnityTest]public IEnumerator TeamHubEntryAndMaximumBalances(){router.Show(ScreenId.TeamHub);Settle();yield return null;var hub=router.Viewport.GetComponentInChildren<ProductionTeamHubView>();hub.NavButtons[1].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamRanking));var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;View().RefreshState();Assert.That(View().PodiumScores[0].text,Is.EqualTo("125 680"));yield return WaitForNormalButtonColors();Capture("TeamRanking_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
        [UnityTest]public IEnumerator TallFiltersDetailsAndExpandedBackPreserveSave()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.TeamRanking);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration;
                Capture("TeamRanking_TallFilters_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>
                {
                    Click(View().Tabs[0],camera);Click(View().Tabs[2],camera);
                    Assert.That(View().Scope,Is.EqualTo(TeamRankingScope.Local));Assert.That(View().Period,Is.EqualTo(TeamRankingPeriod.Week));
                });
                yield return WaitForNormalButtonColors();
                Capture("TeamRanking_LocalWeekTall_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().DetailsButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.True);Assert.That(router.RouteGeneration,Is.EqualTo(generation));
                yield return WaitForNormalButtonColors();
                Capture("TeamRanking_TallDetails_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("TeamRanking_DetailsRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().CloseInfoButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.False);yield return WaitForNormalButtonColors();
                Capture("TeamRanking_BackTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var button=View().BackButton;Rect visual=Bounds(button.image.rectTransform,camera);
                    Click(button,camera,new Vector2(visual.center.x,visual.yMax+2));
                });
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
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
            foreach(var t in View().Names)Assert.That(t.font,Is.SameAs(View().NamesFont));foreach(var t in View().Ranks)Assert.That(t.font,Is.SameAs(View().RankFont));
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_TEAM_RANKING_CAPTURE_DIR");
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
            var cameraGo = new GameObject("TeamRankingCaptureCamera",typeof(Camera));
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
        static void VerifyRewardClearance(RectTransform parent,Text count,int index,Camera camera,float unit)
        {
            Rect slot=Bounds((RectTransform)parent.Find("RewardSlot_"+index),camera);
            Rect icon=Bounds((RectTransform)parent.Find("RewardIcon_"+index),camera),label=Bounds(count.rectTransform,camera);
            foreach(var rect in new[]{icon,label})
            {
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(slot.xMin+2*unit));Assert.That(rect.xMax,Is.LessThanOrEqualTo(slot.xMax-2*unit));
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(slot.yMin+2*unit));Assert.That(rect.yMax,Is.LessThanOrEqualTo(slot.yMax-2*unit));
            }
            Assert.That(icon.yMin,Is.GreaterThan(label.yMax));
        }
        static Rect PodiumStatGlyphBounds(Text text,RectTransform body)
        {
            var mesh=text.canvasRenderer.GetMesh();
            Assert.That(mesh,Is.Not.Null,text.name+" rendered mesh");
            var vertices=mesh.vertices;
            Vector2 minimum=new Vector2(float.PositiveInfinity,float.PositiveInfinity);
            Vector2 maximum=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
            int visibleQuads=0;
            for(int i=0;i+3<vertices.Length;i+=4)
            {
                Vector3 quadMinimum=vertices[i],quadMaximum=vertices[i];
                for(int j=1;j<4;j++){quadMinimum=Vector3.Min(quadMinimum,vertices[i+j]);quadMaximum=Vector3.Max(quadMaximum,vertices[i+j]);}
                // Spaces and a generator's terminal quad have no visible glyph area.
                if(quadMaximum.x-quadMinimum.x<.0001f||quadMaximum.y-quadMinimum.y<.0001f)continue;
                visibleQuads++;
                for(int j=0;j<4;j++)
                {
                    Vector3 local=body.InverseTransformPoint(text.rectTransform.TransformPoint(vertices[i+j]));
                    Vector2 point=new Vector2(local.x-body.rect.xMin,body.rect.yMax-local.y);
                    minimum=Vector2.Min(minimum,point);maximum=Vector2.Max(maximum,point);
                }
            }
            Assert.That(visibleQuads,Is.GreaterThan(0),text.text+" visible glyph quads");
            return Rect.MinMaxRect(minimum.x,minimum.y,maximum.x,maximum.y);
        }
        static Rect VerifyPodiumStat(Text text,RectTransform body,Rect sourceLane,float unit)
        {
            string label=text.transform.parent.name+"/"+text.name+" "+text.text;
            Assert.That(text.cachedTextGenerator.lineCount,Is.EqualTo(1),label+" single line");
            Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(text.text.Length),label+" complete text");
            Rect glyphs=PodiumStatGlyphBounds(text,body);
            // Inspect the actual rendered mesh, including Canvas pixel snapping, in reference coordinates.
            float tolerance=1.5f/unit;
            Assert.That(glyphs.xMin,Is.GreaterThanOrEqualTo(sourceLane.xMin-tolerance),label+" clears original source icon");
            Assert.That(glyphs.xMax,Is.LessThanOrEqualTo(sourceLane.xMax+tolerance),label+" stays inside source panel");
            Assert.That(glyphs.yMin,Is.GreaterThanOrEqualTo(sourceLane.yMin-tolerance),label+" stays below ribbon or divider");
            Assert.That(glyphs.yMax,Is.LessThanOrEqualTo(sourceLane.yMax+tolerance),label+" stays above divider or rewards");
            return glyphs;
        }
        static void VerifyLayoutAndInput(ProductionTeamRankingView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            float unit=Bounds(v.Body,camera).width/941f;
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(1));
            Assert.That(Bounds((RectTransform)v.BackButton.transform,camera).yMin,Is.EqualTo(safeArea.yMin+231*unit).Within(1));
            Assert.That(Bounds((RectTransform)v.DetailsButton.transform,camera).yMin,Is.EqualTo(safeArea.yMin+64*unit).Within(1));
            Rect back=Bounds((RectTransform)v.BackButton.transform,camera),lastRow=Bounds(v.Rows[6],camera);
            Assert.That(back.yMax,Is.LessThan(lastRow.yMin),"Back has its own space below the standings");
            Rect leftBanner=Bounds((RectTransform)v.Body.Find("FooterBannerLeft"),camera);
            Assert.That(back.yMin,Is.GreaterThan(leftBanner.yMax),"Back clears the banner and its upper candle");
            foreach(string name in new[]{"FooterBannerLeft","FooterBannerRight","FooterSourceCarving"})
            {
                var art=v.Body.Find(name).GetComponent<Image>();Assert.That(art.raycastTarget,Is.False);
                Rect bounds=Bounds(art.rectTransform,camera);
                Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.1f));
                Assert.That(art.transform.GetSiblingIndex(),Is.LessThan(v.DetailsButton.transform.GetSiblingIndex()));
            }
            var backImage=v.BackButton.image;Vector4 padding=backImage.raycastPadding;
            Rect backTarget=Rect.MinMaxRect(back.xMin+padding.x*unit,back.yMin+padding.y*unit,back.xMax-padding.z*unit,back.yMax-padding.w*unit);
            Assert.That(safeArea.Contains(backTarget.min)&&safeArea.Contains(backTarget.max),Is.True);
            Assert.That(backTarget.yMax,Is.LessThan(lastRow.yMin),"Back touch area clears the table");
            Rect details=Bounds((RectTransform)v.DetailsButton.transform,camera);
            Assert.That(backTarget.Overlaps(details),Is.False);
            foreach(var target in new[]{backTarget,details})Assert.That(Mathf.Min(target.width,target.height),Is.GreaterThanOrEqualTo(44),"Footer touch target at tested phone size");
            foreach(string name in new[]{"FooterBannerLeft","FooterBannerRight","FooterSourceCarving"})
            {
                var art=v.Body.Find(name).GetComponent<Image>();
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple));Assert.That(art.preserveAspect,Is.True);
                Assert.That(art.sprite.rect.size,Is.EqualTo(new Vector2(art.sprite.texture.width,art.sprite.texture.height)),"Whole original ornament");
                Rect bounds=Bounds(art.rectTransform,camera);
                Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f));
                Assert.That(bounds.yMin,Is.EqualTo(safeArea.yMin).Within(.5f),"Footer ornament anchored at safe bottom");
            }
            if(v.InfoPanel.activeSelf)
            {
                var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                foreach(var point in new[]{back.center,details.center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Details blocks the full safe viewport");
                }
            }
            else foreach(var point in new[]{new Vector2(backTarget.xMin+1,backTarget.center.y),new Vector2(backTarget.xMax-1,backTarget.center.y),new Vector2(backTarget.center.x,backTarget.yMax-1),new Vector2(backTarget.center.x,backTarget.yMin+1)})
            {
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(v.BackButton));
            }
            Assert.That(v.Body.Find("CarvedFooter").gameObject.activeSelf,Is.False);
            bool english=DemoFeatureState.LanguageCode=="en";
            Assert.That(v.DetailsLabel.enabled,Is.EqualTo(english));
            Assert.That(v.DetailsButton.image.sprite.name,Is.EqualTo(english?"details":"details_ru_r7"));
            var detailsRect=(RectTransform)v.DetailsButton.transform;
            var detailsSprite=v.DetailsButton.image.sprite.rect;
            Assert.That(detailsRect.rect.width/detailsRect.rect.height,Is.EqualTo(detailsSprite.width/detailsSprite.height).Within(.0001f),"Complete footer art must not be stretched or cropped between locales");
            for(int i=0;i<4;i++)
            {
                var label=v.TabLabels[i];var tab=(RectTransform)v.Tabs[i].transform;
                bool selected=i<2?i==(int)v.Scope:i-2==(int)v.Period;
                Assert.That(v.Tabs[i].image.sprite.name,Is.EqualTo("tab_"+i+(selected?"_active_r6":"_normal_r6")));
                Assert.That(label.cachedTextGenerator.lineCount,Is.EqualTo(1));
                Assert.That(label.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(label.text.Length));
                Rect glyphs=PodiumStatGlyphBounds(label,v.Body);
                Assert.That(glyphs.xMin,Is.GreaterThan(tab.anchoredPosition.x+12));
                Assert.That(glyphs.xMax,Is.LessThan(tab.anchoredPosition.x+tab.rect.width-12));
                Assert.That(glyphs.yMin,Is.GreaterThan(291));Assert.That(glyphs.yMax,Is.LessThan(340));
            }
            Assert.That(Bounds(v.SeasonNote.rectTransform,camera).yMin,Is.GreaterThan(Bounds(v.PreviewLabel.rectTransform,camera).yMax));
            Assert.That(v.SeasonNote.text,Does.Not.Contain("2д."),"No invented source countdown");
            for(int i=0;i<3;i++)
            {
                Rect name=Bounds(v.PodiumNames[i].rectTransform,camera), podium=Bounds(v.Podiums[i],camera);
                Assert.That(name.xMin,Is.GreaterThan(podium.xMin+25*unit));Assert.That(name.xMax,Is.LessThan(podium.xMax-25*unit));
                Assert.That(name.height,Is.LessThanOrEqualTo(33*unit));
                // Source lanes begin to the right of the baked member/contribution icons.
                Rect members=VerifyPodiumStat(v.PodiumMembers[i],v.Body,new Rect(new[]{451f,139f,732f}[i],607,new[]{98f,100f,100f}[i],32),unit);
                Rect score=VerifyPodiumStat(v.PodiumScores[i],v.Body,new Rect(new[]{436f,130f,727f}[i],644,new[]{134f,113f,114f}[i],35),unit);
                Assert.That(members.yMax,Is.LessThan(642),"Podium "+(i+1)+" members clear the original divider");
                Assert.That(members.yMax,Is.LessThan(score.yMin),"Podium "+(i+1)+" stat lines remain separate");
                Assert.That(score.yMax,Is.LessThan(682),"Podium "+(i+1)+" score clears reward enclosure top at 684");
                for(int j=0;j<3;j++)VerifyRewardClearance(v.Podiums[i],v.PodiumRewards[i*3+j],j,camera,unit);
            }
            for(int i=0;i<7;i++)for(int j=0;j<3;j++)
            {
                VerifyRewardClearance(v.Rows[i],v.Rewards[i*3+j],j,camera,unit);
                Rect slot=Bounds((RectTransform)v.Rows[i].Find("RewardSlot_"+j),camera),row=Bounds(v.Rows[i],camera);
                Assert.That(slot.yMax,Is.LessThanOrEqualTo(row.yMax-6*unit));Assert.That(slot.yMin,Is.GreaterThanOrEqualTo(row.yMin+6*unit));
            }
            foreach(var name in v.Names)
            {
                Assert.That(name.cachedTextGenerator.lineCount,Is.EqualTo(1),name.text+" single line");
                Assert.That(name.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(name.text.Length),name.text+" complete text");
                Assert.That(name.fontSize,Is.InRange(17,22));Assert.That(name.resizeTextForBestFit,Is.False);
                Assert.That(name.preferredWidth,Is.LessThanOrEqualTo(name.rectTransform.rect.width-2),name.text+" measured width");
                Rect textBounds=Bounds(name.rectTransform,camera);
                var parent=name.transform.parent;
                Assert.That(textBounds.xMin,Is.GreaterThan(Bounds((RectTransform)parent.Find("GuildCrest"),camera).xMax));
                Assert.That(textBounds.xMax,Is.LessThan(Bounds((RectTransform)parent.Find("MembersIcon"),camera).xMin));
            }
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            for(int i=0;i<7;i++){Rect row=Bounds(v.Rows[i],camera);Assert.That(safeArea.Contains(row.min)&&safeArea.Contains(row.max),Is.True);if(i<6)Assert.That(row.yMin,Is.GreaterThanOrEqualTo(Bounds(v.Rows[i+1],camera).yMax-.1f));}
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&button!=v.CloseInfoButton)continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin>=safeArea.xMin-.1f&&rect.yMin>=safeArea.yMin-.1f&&rect.xMax<=safeArea.xMax+.1f&&rect.yMax<=safeArea.yMax+.1f,Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

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
