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
    public sealed class EventsAlignmentPlayModeTests
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
            router.Show(ScreenId.Events);Settle();yield return null;yield return null;
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

        ProductionEventsView View()=>router.Viewport.GetComponentInChildren<ProductionEventsView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveHudLocalizationAndResponsiveCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(router.Viewport.GetComponentInChildren<AnimatedHotspot>(),Is.Null);
            int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("Events_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Events_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Events_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Capture("Events_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Events_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator CategoriesAndUnavailableControlsNeverGrantOrRequestParticipation()
        {
            string save=StableSave();bool claimed=DemoFeatureState.EventRewardClaimed;var v=View();int requests=0;
            v.Bind(id=>requests++,()=>{});v.Bind(id=>requests++,()=>{});
            Assert.That(v.Category,Is.EqualTo(EventsCategory.Upcoming));
            for(int i=0;i<4;i++){Assert.That(v.CanParticipate(i),Is.False);Assert.That(v.Statuses[i].text,Is.EqualTo("Дата будет объявлена"));if(i>0){Assert.That(v.DetailsButtons[i].IsInteractable(),Is.False);v.DetailsButtons[i].onClick.Invoke();}}
            Assert.That(requests,Is.Zero);
            v.Tabs[0].onClick.Invoke();Assert.That(v.Category,Is.EqualTo(EventsCategory.Active));Assert.That(v.EmptyPanel.activeSelf,Is.True);foreach(var card in v.Cards)Assert.That(card.gameObject.activeSelf,Is.False);
            yield return WaitForNormalButtonColors();Capture("Events_ActiveEmpty_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.Tabs[2].onClick.Invoke();Assert.That(v.Category,Is.EqualTo(EventsCategory.Completed));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Events_CompletedEmpty_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            v.Tabs[1].onClick.Invoke();Assert.That(v.EmptyPanel.activeSelf,Is.False);foreach(var card in v.Cards)Assert.That(card.gameObject.activeSelf,Is.True);
            var preview=v.DetailsButtons[0];preview.onClick.Invoke();preview.onClick.Invoke();Assert.That(requests,Is.EqualTo(1));
            Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.EventRewardClaimed,Is.EqualTo(claimed));
        }
        [UnityTest]public IEnumerator NativeHeaderNavigationRoutesOnceAndKeepsHomeReturn()
        {
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++)
            {
                router.Show(ScreenId.Events);Settle();yield return null;var v=View();var button=i==0?v.ProfileButton:i==1?v.SettingsButton:v.PlusButtons[i-2];int generation=router.RouteGeneration;
                button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
            router.Show(ScreenId.Events);Settle();yield return null;View().BackButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));
        }
        [UnityTest]public IEnumerator HomeEntryAndDemoDetailPreviewPreserveBalancesAndClaimState()
        {
            router.Show(ScreenId.Home);Settle();yield return null;var home=router.Viewport.GetComponentInChildren<ProductionHomeView>();
            string save=StableSave();bool claimed=DemoFeatureState.EventRewardClaimed;int generation=router.RouteGeneration;
            home.EventsButton.onClick.Invoke();home.EventsButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Events));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            var button=View().DetailsButtons[0];button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.EventDetail));
            router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Events));Assert.That(View().ActionsBound,Is.True);
            Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.EventRewardClaimed,Is.EqualTo(claimed));
        }
        [UnityTest]public IEnumerator MaximumBalancesRemainVisibleAndDoNotChangePageAvailability()
        {
            var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.energy=save.energyMax;View().RefreshState();
            Assert.That(View().GoldText.text,Is.EqualTo("2 147 483 647"));Assert.That(View().GemsText.text,Is.EqualTo("2 147 483 647"));
            yield return WaitForNormalButtonColors();Capture("Events_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Assert.That(View().Category,Is.EqualTo(EventsCategory.Upcoming));
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
            string directory = Environment.GetEnvironmentVariable("LOR_EVENTS_CAPTURE_DIR");
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
            var cameraGo = new GameObject("EventsCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionEventsView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionEventsView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(v.Header.HasBindings,Is.True);
            // A single textured selection follows category changes in both languages.
            for(int tab=0;tab<3;tab++)
            {
                Assert.That(v.Tabs[tab].image.sprite.name,Is.EqualTo((int)v.Category==tab?"tab_selected_r7":"tab_r3"));
                Assert.That(v.Tabs[tab].image.color,Is.EqualTo(Color.white),"Do not recolor antique gold together with the blue face");
                var flat=v.Tabs[tab].transform.Find("ActiveFace");
                if(flat!=null)Assert.That(flat.gameObject.activeSelf,Is.False);
            }
            var footer=v.Body.Find("FooterOrnament").GetComponent<Image>();
            Rect footerBounds=Bounds(footer.rectTransform,camera),note=Bounds(v.Footer.rectTransform,camera);
            Assert.That(footerBounds.xMin,Is.GreaterThan(Bounds((RectTransform)v.BackButton.transform,camera).xMax),"Back must not cover source curls");
            Assert.That(footerBounds.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin),"Footer jewel below safe edge");
            Assert.That(footerBounds.Contains(note.min)&&footerBounds.Contains(note.max),Is.True,"Live footer note must fit its source face");
            Assert.That(footer.raycastTarget,Is.False);
            Assert.That(footer.transform.GetSiblingIndex(),Is.LessThan(v.Footer.transform.GetSiblingIndex()));
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(.5f),"Header must reach safe top");
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            for(int i=0;i<4;i++)if(v.Cards[i].gameObject.activeSelf)
            {
                Rect card=Bounds(v.Cards[i],camera);Assert.That(safeArea.Contains(card.min)&&safeArea.Contains(card.max),Is.True);
                if(i<3)Assert.That(card.yMin,Is.GreaterThan(Bounds(v.Cards[i+1],camera).yMax),"Event cards overlap");
                Rect status=Bounds(v.Statuses[i].rectTransform,camera),clock=Bounds((RectTransform)v.Cards[i].Find("Clock"),camera);
                Assert.That(clock.xMax,Is.LessThanOrEqualTo(status.xMin),"Clock overlaps date text");
                Rect description=Bounds(v.Descriptions[i].rectTransform,camera),reward=Bounds((RectTransform)v.Cards[i].Find("RewardHeading"),camera);
                Assert.That(description.yMin,Is.GreaterThanOrEqualTo(reward.yMax),"Description overlaps reward heading");
                Assert.That(Bounds(v.Names[i].rectTransform,camera).yMin,Is.GreaterThan(status.yMax),"Name overlaps status");
                Rect pending=Bounds((RectTransform)v.Cards[i].Find("RewardExamples"),camera),action=Bounds((RectTransform)v.DetailsButtons[i].transform,camera);
                Assert.That(pending.xMax,Is.LessThan(action.xMin),"Reward examples overlap action");
                var window=(RectTransform)v.Cards[i].Find("IllustrationWindow");var subject=window.Find("Illustration").GetComponent<Image>();var painting=window.Find("CleanScene").GetComponent<Image>();
                Assert.That(subject.sprite.name,Does.EndWith("_r3"));
                Assert.That(painting.sprite.name,Does.EndWith("_r6"));
                Assert.That(subject.rectTransform.anchoredPosition,Is.EqualTo(painting.rectTransform.anchoredPosition));
                Assert.That(subject.rectTransform.sizeDelta,Is.EqualTo(painting.rectTransform.sizeDelta));
                if(i==0)
                {
                    var halo=window.Find("FirebirdHalo").GetComponent<Image>();var sparks=window.Find("FirebirdSparks").GetComponent<Image>();
                    Assert.That(halo.raycastTarget||sparks.raycastTarget,Is.False);
                    Assert.That(halo.transform.GetSiblingIndex(),Is.LessThan(subject.transform.GetSiblingIndex()));
                    Assert.That(sparks.transform.GetSiblingIndex(),Is.GreaterThan(subject.transform.GetSiblingIndex()));
                    Assert.That(sparks.rectTransform.sizeDelta,Is.EqualTo(subject.rectTransform.sizeDelta));
                    Assert.That(sparks.rectTransform.anchoredPosition,Is.EqualTo(subject.rectTransform.anchoredPosition));
                }
                Assert.That(subject.rectTransform.rect.width/subject.rectTransform.rect.height,Is.EqualTo(subject.sprite.rect.width/subject.sprite.rect.height).Within(.0001f));
                Assert.That(v.Cards[i].Find("RewardHeading").GetComponent<Text>().text,Is.EqualTo(DemoFeatureState.LanguageCode=="en"?"Reward examples:":"Пример наград:"));
                Assert.That(v.Rewards[i].gameObject.activeSelf,Is.False);
                Rect windowBounds=Bounds(window,camera),paintingBounds=Bounds(painting.rectTransform,camera);
                Assert.That(window.GetComponent<RectMask2D>(),Is.Not.Null);
                Assert.That(window.GetComponent<Mask>(),Is.Not.Null,"Rounded stencil keeps scene out of frame corner cutouts");
                var rewardRoot=(RectTransform)v.Cards[i].Find("RewardExamples");
                for(int j=0;j<3;j++)
                {
                    var icon=rewardRoot.Find("SourceReward_"+j).GetComponent<Image>();
                    Assert.That(icon.preserveAspect,Is.True);
                    Rect iconBounds=Bounds(icon.rectTransform,camera);
                    Assert.That(iconBounds.xMax,Is.LessThan(action.xMin),"Individual reward crosses action button");
                    Assert.That(iconBounds.yMin,Is.GreaterThan(card.yMin+8*card.height/v.Cards[i].rect.height),"Reward covers lower decorative rail");
                    Assert.That(iconBounds.yMax,Is.LessThanOrEqualTo(reward.yMin),"Reward covers reward caption");
                }
                Assert.That(v.Cards[i].Find("FrameOrnaments").GetSiblingIndex(),Is.GreaterThan(window.GetSiblingIndex()),"Source curls must draw above scenery");
                Assert.That(paintingBounds.xMin,Is.LessThanOrEqualTo(windowBounds.xMin+1));Assert.That(paintingBounds.xMax,Is.GreaterThanOrEqualTo(windowBounds.xMax-1));
                Assert.That(paintingBounds.yMin,Is.LessThanOrEqualTo(windowBounds.yMin+1));Assert.That(paintingBounds.yMax,Is.GreaterThanOrEqualTo(windowBounds.yMax-1));
                Assert.That(painting.rectTransform.rect.width/painting.rectTransform.rect.height,Is.EqualTo(painting.sprite.rect.width/painting.sprite.rect.height).Within(.0001f));
                Assert.That(Bounds((RectTransform)v.BackButton.transform,camera).yMax,Is.LessThan(Bounds(v.Cards[3],camera).yMin),"Back overlaps final card");
            }
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f),button.name+" outside safe left");
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f),button.name+" outside safe right");
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f),button.name+" outside safe bottom");
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.5f),button.name+" outside safe top");
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
