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
    public sealed class FriendsListAlignmentPlayModeTests
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
            router.Show(ScreenId.FriendsList);Settle();yield return null;yield return null;
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

        ProductionFriendsListView View()=>router.Viewport.GetComponentInChildren<ProductionFriendsListView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("FriendsList_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("FriendsList_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("FriendsList_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;Assert.That(View().PreviewLabel.text,Does.Contain("SAMPLE"));Capture("FriendsList_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("FriendsList_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("FriendsList_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.TeamHub};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.FriendsList);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator SearchSortAndTabStatesRemainReadOnly(){
            var v=View();string save=StableSave();Assert.That(v.HasLiveFriends,Is.False);Assert.That(v.VisibleCount,Is.EqualTo(6));v.Search.text="богатыри";Assert.That(v.VisibleCount,Is.EqualTo(2));v.Search.text="snow";Assert.That(v.VisibleCount,Is.EqualTo(1));Assert.That(v.RowsRoot.GetChild(0).Find("Name").GetComponent<Text>().text,Is.EqualTo("Снегурочка"));
            v.Search.text="NO_SUCH_FRIEND";Assert.That(v.VisibleCount,Is.Zero);Assert.That(v.EmptyLabel.gameObject.activeSelf,Is.True);yield return WaitForNormalButtonColors();Capture("FriendsList_Empty_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Search.text="";v.Tabs[1].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("FriendsList_Requests_Default_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Tabs[0].onClick.Invoke();v.SortButton.onClick.Invoke();Assert.That(v.SortByName,Is.True);Assert.That(v.VisibleCount,Is.EqualTo(6));
            v.Tabs[1].onClick.Invoke();Assert.That(v.SelectedTab,Is.EqualTo(FriendsListTab.Requests));Assert.That(v.VisibleCount,Is.EqualTo(5));yield return WaitForNormalButtonColors();Capture("FriendsList_Requests_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("FriendsList_Requests_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            v.Tabs[2].onClick.Invoke();Assert.That(v.VisibleCount,Is.EqualTo(6));v.Search.text="Alena";Assert.That(v.VisibleCount,Is.EqualTo(1));yield return WaitForNormalButtonColors();Capture("FriendsList_Find_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.That(StableSave(),Is.EqualTo(save));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectTab((FriendsListTab)8));
        }
        [UnityTest]public IEnumerator SocialActionsAreHonestAndOverlayBlocksRoutes(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;
            foreach(string action in new[]{"Energy","Visit"}){v.RowsRoot.GetChild(0).Find(action).GetComponent<Button>().onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.ProfileButton.onClick.Invoke();v.Tabs[1].onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(v.SelectedTab,Is.EqualTo(FriendsListTab.Friends));v.CloseInfoButton.onClick.Invoke();}
            v.Tabs[1].onClick.Invoke();foreach(string action in new[]{"Accept","Decline"}){v.RowsRoot.GetChild(0).Find(action).GetComponent<Button>().onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.CloseInfoButton.onClick.Invoke();Assert.That(v.VisibleCount,Is.EqualTo(5));}
            v.RemoveButton.onClick.Invoke();yield return WaitForNormalButtonColors();Capture("FriendsList_Info_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();Capture("FriendsList_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.FriendsList));Assert.That(StableSave(),Is.EqualTo(save));router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));
        }
        [UnityTest]public IEnumerator TeamHubEntryAndMaximumBalances(){router.Show(ScreenId.TeamHub);Settle();yield return null;router.Viewport.GetComponentInChildren<ProductionTeamHubView>().FriendsButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.FriendsList));var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;View().RefreshState();yield return WaitForNormalButtonColors();Capture("FriendsList_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
        [UnityTest]public IEnumerator FooterRoutesAndRepeatedBindCommitOnce(){
            ScreenId[] targets={ScreenId.WorldMap,ScreenId.Shop,ScreenId.Events,ScreenId.FriendsList,ScreenId.Characters};string save=StableSave();
            for(int i=0;i<5;i++){router.Show(ScreenId.FriendsList);Settle();yield return null;int g=router.RouteGeneration;var b=View().NavButtons[i];b.onClick.Invoke();b.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(g+(i==3?0:1)));}
            router.Show(ScreenId.FriendsList);Settle();yield return null;int calls=0;View().Bind(id=>calls++,()=>calls++);View().Bind(id=>calls++,()=>calls++);View().BackButton.onClick.Invoke();View().BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));Assert.That(StableSave(),Is.EqualTo(save));
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
            Assert.That(View().Search.textComponent.supportRichText,Is.False);
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_FRIENDS_LIST_CAPTURE_DIR");
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
            var cameraGo = new GameObject("FriendsListCaptureCamera",typeof(Camera));
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
                inspect?.Invoke(camera);
                camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0); texture.Apply();
                string output = Path.Combine(directory,name);
                Assert.That(File.Exists(output + ".png"), Is.False, "Keep earlier evidence.");
                File.WriteAllBytes(output + ".png",texture.EncodeToPNG());
                // Retain visual evidence even if a layout assertion fails.
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionFriendsListView>(),camera,size,safeArea);
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
        static void VerifyLayoutAndInput(ProductionFriendsListView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(v.Header.HasBindings,Is.True);
            for(int i=0;i<2;i++)Assert.That(Bounds((RectTransform)v.Tabs[i].transform,camera).xMax,Is.EqualTo(Bounds((RectTransform)v.Tabs[i+1].transform,camera).xMin).Within(.5f),"Joined tabs have no gap");
            Assert.That(Bounds((RectTransform)v.Tabs[0].transform,camera).yMin,Is.GreaterThanOrEqualTo(Bounds(v.Search.GetComponent<RectTransform>(),camera).yMax),"Search clears tab ornament");
            foreach(Transform row in v.RowsRoot){var flag=row.Find("GuildEmblem");if(flag){var icon=flag.GetComponent<Image>();Assert.That(icon.raycastTarget,Is.False);Assert.That(Bounds(icon.rectTransform,camera).xMax,Is.LessThan(Bounds(row.Find("Guild").GetComponent<RectTransform>(),camera).xMin),"Guild flag clears label");}}
            bool requests=v.SelectedTab==FriendsListTab.Requests;
            Assert.That(v.RequestsFrame.gameObject.activeSelf,Is.EqualTo(requests));
            Assert.That(v.Body.Find("TableFrame").gameObject.activeSelf,Is.EqualTo(!requests));
            Assert.That(v.RowsRoot.rect.width,Is.EqualTo(requests?886:747),"Switching tabs restores the correct row width");
            if(requests)foreach(Transform row in v.RowsRoot){
                Rect portrait=Bounds((RectTransform)row.Find("Portrait"),camera),name=Bounds((RectTransform)row.Find("Name"),camera),gift=Bounds((RectTransform)row.Find("Gift"),camera),accept=Bounds((RectTransform)row.Find("Accept"),camera),decline=Bounds((RectTransform)row.Find("Decline"),camera);
                Assert.That(portrait.xMax,Is.LessThan(name.xMin),"Portrait ornament clears live name");
                Assert.That(name.xMax,Is.LessThan(gift.xMin),"Request text clears gift");
                Assert.That(gift.xMax,Is.LessThan(accept.xMin),"Gift clears Accept");
                Assert.That(accept.xMax,Is.LessThan(decline.xMin),"Accept clears Decline");
                Assert.That(portrait.yMin,Is.GreaterThan(Bounds((RectTransform)v.SortButton.transform,camera).yMax),"Last portrait clears lower controls");
            }
            bool english=DemoFeatureState.LanguageCode=="en";
            Assert.That(v.SortLabel.enabled,Is.EqualTo(english||v.SortByName),"Sort uses one source or live caption");
            Assert.That(v.RemoveLabel.enabled,Is.EqualTo(english),"Remove uses one localized caption");
            Assert.That(v.SortLabel.rectTransform.anchoredPosition.x,Is.GreaterThanOrEqualTo(50),"Live sort caption clears original people icon");
            float unit=Bounds(v.Body,camera).width/941f;
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(1));
            // Footer artwork uses full, uncropped source sprites at native aspect.
            float previousRight = float.NegativeInfinity;
            foreach (var button in v.NavButtons)
            {
                var icon = button.image;
                Rect bounds = Bounds(icon.rectTransform, camera);
                Assert.That(icon.rectTransform.rect.height,Is.EqualTo(236),"World Map footer scale");
                Assert.That(icon.type, Is.EqualTo(Image.Type.Simple));
                Assert.That(icon.preserveAspect, Is.True);
                Assert.That(icon.sprite.rect.size, Is.EqualTo(new Vector2(icon.sprite.texture.width, icon.sprite.texture.height)));
                Assert.That(bounds.xMin, Is.GreaterThanOrEqualTo(previousRight - 1), "Footer icons must not overlap");
                Assert.That(bounds.width / bounds.height,
                    Is.EqualTo(icon.sprite.rect.width / icon.sprite.rect.height).Within(.015f), "Original icon aspect");
                previousRight = bounds.xMax;
            }
            Assert.That(Bounds((RectTransform)v.NavButtons[0].transform,camera).yMin,Is.EqualTo(safeArea.yMin+6*unit).Within(1));
            foreach(var label in v.NavLabels)Assert.That(label.enabled,Is.True,"Localized live footer captions");
            for(int i=0;i<5;i++){
                Assert.That(v.NavButtons[i].image.sprite,Is.SameAs(ProductionFriendsListRuntime.WorldMapFooter(i)),"World Map metalwork is reused directly");
                var symbol=v.NavButtons[i].GetComponentInChildren<ProductionFooterSymbolGraphic>();
                Assert.That(symbol!=null,Is.EqualTo(i!=1));
                if(symbol){
                    Assert.That(symbol.Source,Is.SameAs(Resources.Load<Sprite>("ProductionUI/WorldMapReferenceAligned/card_surface")),"Neutral inset contains no copied selection ring");Assert.That(symbol.raycastTarget,Is.False);
                    var emblem=v.NavButtons[i].transform.Find("PageSymbol").GetComponent<Image>();
                    Assert.That(emblem.sprite.name,Does.StartWith("symbol_").And.EndWith("_r7"));Assert.That(emblem.preserveAspect,Is.True);Assert.That(emblem.raycastTarget,Is.False);
                    Assert.That(emblem.rectTransform.rect.width/emblem.rectTransform.rect.height,Is.EqualTo(emblem.sprite.rect.width/emblem.sprite.rect.height).Within(.001f),"Whole emblem keeps its aspect");
                    Rect full=Bounds(emblem.rectTransform,camera),pixels=ProductionFriendsListRuntime.FooterSymbolBounds(i),frame=Bounds((RectTransform)v.NavButtons[i].transform,camera);
                    float sx=full.width/emblem.sprite.rect.width,sy=full.height/emblem.sprite.rect.height;
                    var visible=Rect.MinMaxRect(full.xMin+pixels.xMin*sx,full.yMax-pixels.yMax*sy,full.xMin+pixels.xMax*sx,full.yMax-pixels.yMin*sy);
                    Assert.That(visible.xMin,Is.GreaterThan(frame.xMin+28*unit));Assert.That(visible.xMax,Is.LessThan(frame.xMax-28*unit));
                    Assert.That(visible.yMax,Is.LessThan(frame.yMax-28*unit),"Complete top clears crest");Assert.That(visible.yMin,Is.GreaterThan(Bounds(v.NavLabels[i].rectTransform,camera).yMax+10*unit),"Complete bottom clears caption rail");
                    Assert.That(symbol.GetComponent<CanvasRenderer>(),Is.Not.Null,"Adapted emblem needs its native renderer");
                    var mesh=symbol.canvasRenderer.GetMesh();Assert.That(mesh,Is.Not.Null);Assert.That(mesh.vertexCount,Is.GreaterThan(90),"Emblem mesh must actually render");
                }
                Assert.That(Bounds(v.NavLabels[i].rectTransform,camera).yMin,Is.GreaterThan(Bounds((RectTransform)v.NavButtons[i].transform,camera).yMin),"Caption stays inside medallion");
            }
            foreach(Transform row in v.RowsRoot){
                var visit=row.Find("Visit");if(visit)Assert.That(visit.Find("Label").GetComponent<Text>().enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
                Assert.That(Bounds((RectTransform)row,camera).yMin,Is.GreaterThanOrEqualTo(Bounds((RectTransform)v.SortButton.transform,camera).yMax));
            }
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            for(int i=0;i<v.RowsRoot.childCount;i++){Rect row=Bounds((RectTransform)v.RowsRoot.GetChild(i),camera);Assert.That(safeArea.Contains(row.min)&&safeArea.Contains(row.max),Is.True);if(i+1<v.RowsRoot.childCount)Assert.That(row.yMin,Is.GreaterThanOrEqualTo(Bounds((RectTransform)v.RowsRoot.GetChild(i+1),camera).yMax-.1f));}
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&button!=v.CloseInfoButton)continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f),button.name+" left safe edge");Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f),button.name+" right safe edge");Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f),button.name+" bottom safe edge");Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.5f),button.name+" top safe edge");
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
