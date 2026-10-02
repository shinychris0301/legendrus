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
    public sealed class TeamHubAlignmentPlayModeTests
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
            router.Show(ScreenId.TeamHub);Settle();yield return null;yield return null;
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

        ProductionTeamHubView View()=>router.Viewport.GetComponentInChildren<ProductionTeamHubView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty,"Native Team Hub must not receive a duplicate demo HUD");int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("TeamHub_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamHub_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("TeamHub_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;Assert.That(View().PreviewLabel.text,Does.Contain("SAMPLE"));Capture("TeamHub_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Capture("TeamHub_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamHub_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.TeamChat,ScreenId.TeamRanking,ScreenId.FriendsList,ScreenId.Home};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.TeamHub);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.NavButtons[0],v.NavButtons[1],v.FriendsButton,v.BackButton};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator UnavailableActionsExplainPreviewAndDoNotSpend(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;Assert.That(v.HasLiveTeam,Is.False);
            foreach(var b in new[]{v.NavButtons[2],v.NavButtons[3],v.ChestButton,v.GuildInfoButton}){b.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.ProfileButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);}
            v.NavButtons[2].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("TeamHub_Info_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();Capture("TeamHub_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.CloseInfoButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(StableSave(),Is.EqualTo(save));int routes=0;v.Bind(id=>routes++,()=>routes++);v.Bind(id=>routes++,()=>routes++);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(routes,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator WorldMapEntryAndMaximumBalances(){router.Show(ScreenId.WorldMap);Settle();yield return null;var map=router.Viewport.GetComponentInChildren<ProductionWorldMapView>();Assert.That(map,Is.Not.Null);map.NavButtons[2].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;View().RefreshState();Assert.That(View().Power.text,Is.EqualTo("125 840"));yield return WaitForNormalButtonColors();Capture("TeamHub_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
        [UnityTest]public IEnumerator ExpandedInfoTargetAndTallModalKeepFriendsRouteAndSave()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.TeamHub);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration;
                Capture("TeamHub_InfoTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var button=View().GuildInfoButton;Rect visual=Bounds(button.image.rectTransform,camera);
                    var pointer=new PointerEventData(EventSystem.current){position=new Vector2(visual.center.x,visual.yMax+4)};
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                    ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
                });
                Assert.That(View().InfoPanel.activeSelf,Is.True);Assert.That(router.RouteGeneration,Is.EqualTo(generation));
                yield return WaitForNormalButtonColors();
                Capture("TeamHub_TallInfo_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("TeamHub_InfoRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var close=View().CloseInfoButton;var pointer=new PointerEventData(EventSystem.current){position=Bounds((RectTransform)close.transform,camera).center};
                    ExecuteEvents.Execute(close.gameObject,pointer,ExecuteEvents.pointerClickHandler);
                });
                Assert.That(View().InfoPanel.activeSelf,Is.False);yield return WaitForNormalButtonColors();
                Capture("TeamHub_FriendsTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var button=View().FriendsButton;var pointer=new PointerEventData(EventSystem.current){position=Bounds((RectTransform)button.transform,camera).center};
                    ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
                });
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.FriendsList));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
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
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_TEAM_HUB_CAPTURE_DIR");
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
            var cameraGo = new GameObject("TeamHubCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionTeamHubView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionTeamHubView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect header=Bounds(v.Hud,camera);float unit=header.width/941f;
            Assert.That(header.yMax,Is.EqualTo(safeArea.yMax).Within(1));
            Assert.That(Bounds((RectTransform)v.BackButton.transform,camera).yMin,Is.EqualTo(safeArea.yMin+17*unit).Within(1));
            Assert.That(v.SeasonFill.fillAmount,Is.EqualTo((float)ProductionTeamHubView.SampleSeasonContribution/ProductionTeamHubView.SampleSeasonTarget).Within(.0001f));
            Assert.That(v.Progress.text,Is.EqualTo("7 560 / 10 000"));Assert.That(v.HasLiveTeam,Is.False);
            bool english=DemoFeatureState.LanguageCode=="en";
            Assert.That(v.Motto.enabled,Is.EqualTo(english));
            Assert.That(v.Body.Find("MottoFrame").GetComponent<Image>().sprite.name,Is.EqualTo(english?"motto_blank_r4":"motto_ru_r4"));
            Rect plaque=Bounds((RectTransform)v.Body.Find("MottoFrame"),camera), friends=Bounds((RectTransform)v.FriendsButton.transform,camera);
            Assert.That(plaque.xMax,Is.LessThan(friends.xMin));
            var carving=v.Body.Find("CarvedFooter").GetComponent<Image>();
            Assert.That(Bounds(carving.rectTransform,camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            Assert.That(friends.yMin,Is.EqualTo(safeArea.yMin+20*unit).Within(.5f));
            var info=v.GuildInfoButton.image;Rect infoVisual=Bounds(info.rectTransform,camera);Vector4 padding=info.raycastPadding;
            Rect infoTarget=Rect.MinMaxRect(infoVisual.xMin+padding.x*unit,infoVisual.yMin+padding.y*unit,infoVisual.xMax-padding.z*unit,infoVisual.yMax-padding.w*unit);
            Assert.That(infoTarget.xMax,Is.LessThan(friends.xMin),"Info target clears Friends");
            Assert.That(safeArea.Contains(infoTarget.min)&&safeArea.Contains(infoTarget.max),Is.True);
            foreach(var rect in new[]{infoTarget,friends,Bounds((RectTransform)v.BackButton.transform,camera)})Assert.That(Mathf.Min(rect.width,rect.height),Is.GreaterThanOrEqualTo(44),"Footer touch target at tested phone size");
            foreach(var image in new[]{info,v.BackButton.image,v.Body.Find("FooterCrest").GetComponent<Image>(),v.Body.Find("MottoFrame").GetComponent<Image>()})
            {
                Assert.That(image.type,Is.EqualTo(Image.Type.Simple));Assert.That(image.preserveAspect,Is.True);
                Assert.That(image.sprite.rect.size,Is.EqualTo(new Vector2(image.sprite.texture.width,image.sprite.texture.height)),"Whole footer artwork retained");
            }
            if(v.InfoPanel.activeSelf)
            {
                var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                foreach(var point in new[]{infoVisual.center,friends.center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Modal blocks the entire safe height");
                }
            }
            else foreach(var point in new[]{new Vector2(infoTarget.xMin+1,infoTarget.center.y),new Vector2(infoTarget.xMax-1,infoTarget.center.y),new Vector2(infoTarget.center.x,infoTarget.yMax-1),new Vector2(infoTarget.center.x,infoTarget.yMin+1)})
            {
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(v.GuildInfoButton));
            }

            Assert.That(Bounds((RectTransform)v.Body.Find("FooterCrest"),camera).xMin,Is.GreaterThan(Bounds((RectTransform)v.BackButton.transform,camera).xMax));
            Assert.That(v.TasksHeading.enabled,Is.EqualTo(english));Assert.That(v.ChestLabel.enabled,Is.EqualTo(english));
            Assert.That(v.Body.Find("GuildChestEnclosure").GetComponent<Image>().sprite.name,Is.EqualTo(english?"chest_enclosure_blank_r2":"chest_enclosure_ru_r2"));
            Assert.That(v.Body.Find("TasksEnclosure").GetComponent<Image>().sprite.name,Is.EqualTo(english?"tasks_enclosure_blank_r2":"tasks_enclosure_ru_r2"));
            for(int i=0;i<4;i++){bool en=DemoFeatureState.LanguageCode=="en";Assert.That(v.NavLabels[i].enabled,Is.EqualTo(en));Assert.That(v.NavButtons[i].image.sprite.name,Is.EqualTo("nav_"+i+(en?"_blank":"_ru")));}
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            for(int i=0;i<5;i++){Rect row=Bounds(v.MemberRows[i],camera);Assert.That(safeArea.Contains(row.min)&&safeArea.Contains(row.max),Is.True);if(i<4)Assert.That(row.yMin,Is.GreaterThanOrEqualTo(Bounds(v.MemberRows[i+1],camera).yMax-.1f));}
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&button!=v.CloseInfoButton)continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.1f),button.name+" left");Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.1f),button.name+" bottom");
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.1f),button.name+" right");Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.1f),button.name+" top");
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
