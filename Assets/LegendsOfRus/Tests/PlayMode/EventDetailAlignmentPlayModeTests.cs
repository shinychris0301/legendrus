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
    public sealed class EventDetailAlignmentPlayModeTests
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
            router.Show(ScreenId.EventDetail);Settle();yield return null;yield return null;
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

        ProductionEventDetailView View()=>router.Viewport.GetComponentInChildren<ProductionEventDetailView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveHudLocalizationAndResponsiveCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(router.Viewport.GetComponentsInChildren<AnimatedHotspot>(),Is.Empty);
            int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("EventDetail_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("EventDetail_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("EventDetail_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().EventName.text,Is.EqualTo("FIREBIRD FESTIVAL"));
            Capture("EventDetail_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("EventDetail_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator PreviewDoesNotClaimRewardsAndRepeatedBindingRoutesOnce()
        {
            string save=StableSave();bool claimed=DemoFeatureState.EventRewardClaimed;var v=View();int routes=0;
            v.Bind(id=>routes++,()=>routes++);v.Bind(id=>routes++,()=>routes++);
            Assert.That(v.CanParticipate,Is.False);Assert.That(v.ActionButton.IsInteractable(),Is.False);
            v.ActionButton.onClick.Invoke();v.ActionButton.onClick.Invoke();v.Body.Find("PremiumPanel/UnavailablePass").GetComponent<Button>().onClick.Invoke();Assert.That(routes,Is.Zero);
            Assert.That(v.RewardHeading.text,Is.EqualTo("Пример наград"));Assert.That(v.Availability.text,Is.EqualTo("Дата будет объявлена"));
            Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.EventRewardClaimed,Is.EqualTo(claimed));
            v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(routes,Is.EqualTo(1));yield return null;
        }
        [UnityTest]public IEnumerator NativeHeaderNavigationRoutesOnce()
        {
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++){
                router.Show(ScreenId.EventDetail);Settle();yield return null;var v=View();var button=i==0?v.ProfileButton:i==1?v.SettingsButton:v.PlusButtons[i-2];int generation=router.RouteGeneration;
                button.onClick.Invoke();button.onClick.Invoke();Settle();yield return null;
                Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
        }
        [UnityTest]public IEnumerator EventsPreviewAndNativeBackPreserveState()
        {
            router.Show(ScreenId.Events);Settle();yield return null;
            string save=StableSave();bool claimed=DemoFeatureState.EventRewardClaimed;int generation=router.RouteGeneration;
            var events=router.Viewport.GetComponentInChildren<ProductionEventsView>();var preview=events.DetailsButtons[0];
            preview.onClick.Invoke();preview.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.EventDetail));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            Assert.That(View().ActionsBound,Is.True);var back=View().BackButton;back.onClick.Invoke();back.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Events));Assert.That(router.RouteGeneration,Is.EqualTo(generation+2));
            Assert.That(router.Viewport.GetComponentInChildren<ProductionEventsView>().ActionsBound,Is.True);
            Assert.That(StableSave(),Is.EqualTo(save));Assert.That(DemoFeatureState.EventRewardClaimed,Is.EqualTo(claimed));
        }
        [UnityTest]public IEnumerator MaximumBalancesRemainReadable()
        {
            var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.energy=save.energyMax;View().RefreshState();
            Assert.That(View().GoldText.text,Is.EqualTo("2 147 483 647"));Assert.That(View().GemsText.text,Is.EqualTo("2 147 483 647"));
            yield return WaitForNormalButtonColors();Capture("EventDetail_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
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
            string directory = Environment.GetEnvironmentVariable("LOR_EVENT_DETAIL_CAPTURE_DIR");
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
            var cameraGo = new GameObject("EventDetailCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionEventDetailView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionEventDetailView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(.5f),"Header must reach safe top");
            Assert.That(Bounds((RectTransform)v.ActionButton.transform,camera).yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f));
            var window=v.Body.Find("FestivalScene/SceneWindow");
            Assert.That(window.Find("CleanCity").gameObject.activeSelf,Is.False,"Free source Firebird must not carry a rectangular scenery crop");
            var subject=window.Find("SourceFirebird").GetComponent<Image>();
            Assert.That(subject.sprite,Is.SameAs(ProductionEventDetailRuntime.DetailArt("firebird_r3")));
            foreach(string layer in new[]{"FirebirdHalo","FirebirdSparks"})
            {
                var effect=window.Find(layer).GetComponent<Image>();
                Assert.That(effect.raycastTarget,Is.False);
                Assert.That(effect.rectTransform.sizeDelta,Is.EqualTo(subject.rectTransform.sizeDelta));
                Assert.That(effect.rectTransform.anchoredPosition,Is.EqualTo(subject.rectTransform.anchoredPosition));
            }
            Assert.That(window.Find("FirebirdHalo").GetSiblingIndex(),Is.LessThan(subject.transform.GetSiblingIndex()));
            Assert.That(window.Find("FirebirdSparks").GetSiblingIndex(),Is.GreaterThan(subject.transform.GetSiblingIndex()));
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            Rect objectives=Bounds((RectTransform)v.Body.Find("ObjectivesFrame"),camera);
            Rect progress=Bounds((RectTransform)v.Body.Find("RewardsFrame"),camera);
            Assert.That(objectives.yMin,Is.GreaterThan(progress.yMax-8),"Objective/progress surfaces must not substantially overlap");
            Assert.That(Bounds(v.Description.rectTransform,camera).yMin,Is.GreaterThan(Bounds(v.RewardHeading.rectTransform,camera).yMax));
            for(int i=0;i<5;i++)
            {
                var milestones=v.Body.Find("Milestones");
                var chest=Bounds((RectTransform)milestones.Find("Chest_"+i),camera);
                var reward=Bounds((RectTransform)milestones.Find("RewardCard_"+i),camera);
                Assert.That(chest.yMin,Is.GreaterThan(reward.yMax));
                Assert.That(reward.xMin,Is.GreaterThan(progress.xMin));Assert.That(reward.xMax,Is.LessThan(progress.xMax));
                Assert.That(reward.yMin,Is.GreaterThan(progress.yMin));
                Assert.That(milestones.Find("Threshold_"+i).GetComponent<Text>().text,Is.EqualTo("—"),"Unconfirmed milestones must not advertise source values");
                Assert.That(milestones.Find("RewardCard_"+i+"/Quantity").GetComponent<Text>().text,Is.EqualTo("—"));
            }
            Assert.That(v.Body.Find("PremiumPanel/UnavailablePass").GetComponent<Button>().interactable,Is.False);
            // Restored ornaments must contain their live contents at every supported safe-area size.
            Rect balance=Bounds((RectTransform)v.Body.Find("FeatherBalanceFrame"),camera);
            foreach(string name in new[]{"FeatherBalanceIcon","FeatherBalance"})
            {
                Rect content=Bounds((RectTransform)v.Body.Find(name),camera);
                Assert.That(content.xMin,Is.GreaterThan(balance.xMin),name+" clears left gold rail");
                Assert.That(content.xMax,Is.LessThan(balance.xMax),name+" clears right gold rail");
                Assert.That(content.yMin,Is.GreaterThan(balance.yMin),name+" clears lower gold rail");
                Assert.That(content.yMax,Is.LessThan(balance.yMax),name+" clears upper gold rail");
            }
            var premium=v.Body.Find("PremiumPanel");
            var inset=premium.Find("RewardInset").GetComponent<Image>();
            Rect enclosure=Bounds(inset.rectTransform,camera);
            Assert.That(inset.raycastTarget,Is.False,"Decoration cannot intercept navigation");
            for(int i=0;i<4;i++)
            {
                var icon=premium.Find("Reward_"+i);
                Rect content=Bounds((RectTransform)icon,camera);
                Assert.That(content.xMin,Is.GreaterThan(enclosure.xMin));
                Assert.That(content.xMax,Is.LessThan(enclosure.xMax));
                Assert.That(content.yMin,Is.GreaterThan(enclosure.yMin));
                Assert.That(content.yMax,Is.LessThan(enclosure.yMax));
                Assert.That(icon.GetSiblingIndex(),Is.GreaterThan(inset.transform.GetSiblingIndex()),"Frame must draw behind reward art");
            }
            Assert.That(v.ActionButton.interactable,Is.False);
            var carving=v.Body.Find("CarvedFooter").GetComponent<Image>();
            Rect footer=Bounds(carving.rectTransform,camera);
            Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin).Within(.5f),"Carved footer follows safe bottom");
            Assert.That(carving.raycastTarget,Is.False);
            Assert.That(carving.transform.GetSiblingIndex(),Is.LessThan(v.BackButton.transform.GetSiblingIndex()));
            Assert.That(carving.transform.GetSiblingIndex(),Is.LessThan(v.ActionButton.transform.GetSiblingIndex()));
            var gift=v.Body.Find("RewardPreview").GetComponent<Image>();
            Assert.That(gift.sprite,Is.SameAs(Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+"source_gift")),"Reuse actual Home gift resource");
            Assert.That(Bounds(gift.rectTransform,camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            Assert.That(v.Body.Find("RewardRim").gameObject.activeSelf,Is.False,"Home gift already includes its own rim");
            var leaders=v.Body.Find("LeadersPanel");
            Rect section=Bounds((RectTransform)leaders,camera);
            float precedingBottom=section.yMax;
            for(int i=0;i<4;i++)
            {
                var row=leaders.Find("Row_"+i);Rect bounds=Bounds((RectTransform)row,camera);
                Assert.That(bounds.yMax,Is.LessThanOrEqualTo(precedingBottom+.5f),"Leaderboard rows cannot overlap");
                Assert.That(bounds.yMin,Is.GreaterThan(section.yMin));precedingBottom=bounds.yMin;
                Assert.That(row.Find("Rank").GetComponent<Text>().text,Is.EqualTo("—"));
                Assert.That(row.Find("Score").GetComponent<Text>().text,Is.EqualTo("—"));
            }
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                // Rect.Contains excludes its maximum edge; shared chrome intentionally reaches it.
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
