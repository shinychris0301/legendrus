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
    public sealed class InventoryAlignmentPlayModeTests
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
            router.Show(ScreenId.Inventory);Settle();yield return null;yield return null;
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

        ProductionInventoryView View()=>router.Viewport.GetComponentInChildren<ProductionInventoryView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
 var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(v.GoldText.text,Is.EqualTo(ProductionInventoryView.Number(before+17)));string save=StableSave();yield return WaitForNormalButtonColors();Capture("Inventory_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Inventory_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("Inventory_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Inventory_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Capture("Inventory_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Inventory_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest]public IEnumerator RoutesCommitOnceAndShopCategoriesArePreserved(){
 var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.Home,ScreenId.SpecialOffer,ScreenId.Shop,ScreenId.Shop};string save=StableSave();for(int i=0;i<targets.Length;i++){router.Show(ScreenId.Inventory);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton,v.NavButtons[0],v.NavButtons[1],v.NavButtons[2]};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));if(targets[i]==ScreenId.Shop)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==2?"coins":i==8?"bundles":"gems"));}Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest]public IEnumerator EveryReferenceItemAndCategoryIsAnHonestPreview(){
 var v=View();Assert.That(v.HasCollectibleInventory,Is.False);string save=StableSave();for(int i=0;i<20;i++){v.DynamicRoot.Find("Item_"+i).GetComponent<Button>().onClick.Invoke();Assert.That(v.SelectedItem,Is.EqualTo(i));Assert.That(v.ItemName.text,Is.EqualTo(ProductionInventoryView.ItemTitle(i).ToUpperInvariant()));Assert.That(v.QuantityText.text,Is.EqualTo("—"));Assert.That(v.UseButton.interactable||v.CanUseSelectedItem,Is.False);v.UseButton.onClick.Invoke();}yield return WaitForNormalButtonColors();Capture("Inventory_Map_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Tabs[2].onClick.Invoke();Assert.That(v.SelectedItem,Is.EqualTo(3));Assert.That(v.DynamicRoot.Find("Item_6"),Is.Not.Null);Assert.That(v.DynamicRoot.Find("Item_0"),Is.Null);yield return WaitForNormalButtonColors();Capture("Inventory_Materials_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Tabs[3].onClick.Invoke();Assert.That(v.SelectedItem,Is.EqualTo(2));Assert.That(v.DynamicRoot.Find("Item_18"),Is.Not.Null);yield return WaitForNormalButtonColors();Capture("Inventory_Quest_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectItem(23));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectTab((InventoryTab)8));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest]public IEnumerator RealBoosterQuantitiesRefreshWithoutConsumption(){
 var v=View();GameState.Instance.Save.boosterMace=7;GameState.Instance.Save.boosterFirebird=0;GameState.Instance.Save.boosterLivingLight=-5;GameState.Instance.Save.boosterSeaPearl=13;string save=StableSave();v.NavButtons[3].onClick.Invoke();Assert.That(v.SelectedTab,Is.EqualTo(InventoryTab.Boosters));Assert.That(v.QuantityText.text,Is.EqualTo("7"));Assert.That(v.DynamicRoot.Find("Item_21/Count").GetComponent<Text>().text,Is.EqualTo("0"));Assert.That(v.DynamicRoot.Find("Item_22/Count").GetComponent<Text>().text,Is.EqualTo("0"));v.UseButton.onClick.Invoke();yield return WaitForNormalButtonColors();Capture("Inventory_Boosters_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.That(StableSave(),Is.EqualTo(save));GameState.Instance.AddBooster(BoosterType.BogatyrMace,2);yield return null;Assert.That(v.QuantityText.text,Is.EqualTo("9"));v.SelectItem(23);Assert.That(v.QuantityText.text,Is.EqualTo("13"));yield return WaitForNormalButtonColors();Capture("Inventory_Pearl_safe_RU_R4",new Vector2Int(576,1280),new Rect(0,48,576,1172));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Inventory_Boosters_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
 }
 [UnityTest]public IEnumerator DetailsAndVipBlockUnderlyingInputAndBackDismisses(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;v.DetailsButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.ProfileButton.onClick.Invoke();v.Tabs[1].onClick.Invoke();v.SelectTab(InventoryTab.Boosters);v.SelectItem(3);Assert.That(v.SelectedItem,Is.Zero);Assert.That(v.SelectedTab,Is.EqualTo(InventoryTab.Artifacts));Assert.That(router.RouteGeneration,Is.EqualTo(generation));yield return WaitForNormalButtonColors();Capture("Inventory_Details_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Inventory_Details_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Inventory));v.NavButtons[4].onClick.Invoke();Assert.That(v.InfoHeading.text,Is.EqualTo("VIP"));yield return WaitForNormalButtonColors();Capture("Inventory_VIP_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.CloseInfoButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest]public IEnumerator MaximumBalancesAndRepeatedBind(){
 var v=View();var s=GameState.Instance.Save;s.gold=int.MaxValue;s.gems=int.MaxValue;s.boosterMace=int.MaxValue;v.SelectTab(InventoryTab.Boosters);v.RefreshState();Assert.That(v.QuantityText.text,Is.EqualTo("2 147 483 647"));yield return WaitForNormalButtonColors();Capture("Inventory_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));int calls=0;v.Bind(id=>calls++,()=>calls++,category=>calls++);v.Bind(id=>calls++,()=>calls++,category=>calls++);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));
 }
        [UnityTest]public IEnumerator SelectionAndSharedArtworkRemainIsolated(){
            var v=View();string save=StableSave();
            Assert.That(v.GetComponentsInChildren<ProductionPlayerHeader>().Length,Is.EqualTo(1));
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(v.Header.Backing.sprite,Is.SameAs(home.Header.Backing.sprite));
            for(int i=0;i<20;i++){
                v.SelectItem(i);var highlight=v.DynamicRoot.GetComponentsInChildren<WorldMapPanelBorderHighlight>();
                Assert.That(highlight.Length,Is.EqualTo(1));Assert.That(highlight[0].transform.parent.name,Is.EqualTo("Item_"+i));Assert.That(highlight[0].raycastTarget,Is.False);
                Assert.That(v.DynamicRoot.Find("Item_"+i).GetComponent<Image>().sprite,Is.SameAs(ProductionInventoryRuntime.ItemArt(i)));
            }
            v.SelectTab(InventoryTab.Boosters);yield return WaitForNormalButtonColors();
            for(int i=0;i<4;i++)Assert.That(v.DynamicRoot.Find("Item_"+(20+i)+"/Artwork").GetComponent<Image>().sprite,Is.SameAs(ProductionBoosterArt.Icon(i,true)));
            var item=v.DynamicRoot.Find("Item_20").GetComponent<Button>();var feedback=item.GetComponent<ProductionMapButtonFeedback>();
            var pointer=new PointerEventData(EventSystem.current){pointerId=42,button=PointerEventData.InputButton.Left};
            var bounds=((RectTransform)item.transform).anchoredPosition;ExecuteEvents.Execute(item.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            Assert.That(feedback.IsPressed,Is.True);Assert.That(((RectTransform)item.transform).anchoredPosition,Is.EqualTo(bounds));
            Capture("Inventory_PressedBooster_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            ExecuteEvents.Execute(item.gameObject,pointer,ExecuteEvents.pointerUpHandler);Assert.That(feedback.IsPressed,Is.False);Assert.That(StableSave(),Is.EqualTo(save));
            yield return null;
        }
        [UnityTest]public IEnumerator OriginalLetteringAndLocalizedTabStates(){
            var v=View();string save=StableSave();
            foreach(string lang in new[]{"ru","en"}){
                DemoFeatureState.SetLanguage(lang);v.RefreshContent();bool en=lang=="en";
                for(int selected=0;selected<4;selected++){
                    v.Tabs[selected].onClick.Invoke();Assert.That(v.SelectedTab,Is.EqualTo((InventoryTab)selected));
                    for(int i=0;i<4;i++){
                        Assert.That(v.TabLabels[i].enabled,Is.EqualTo(en));
                        Assert.That(v.Tabs[i].image.sprite,Is.SameAs(ProductionInventoryRuntime.TabArt(i,i==selected,en)));
                    }
                    Assert.That(v.DetailsLabel.enabled,Is.EqualTo(en));Assert.That(v.DetailsButton.image.sprite,Is.SameAs(ProductionInventoryRuntime.Load(en?"details_blank_r3":"details_ru_r3")));
                    Assert.That(v.UseButton.interactable,Is.False);
                    yield return WaitForNormalButtonColors();
                    if(selected==2)Capture("Inventory_Materials_"+lang.ToUpperInvariant()+"_R3",new Vector2Int(576,1280),new Rect(0,48,576,1172));
                }
            }
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator NativeTabsFooterAndTallDetailsPreserveStockAndRoutes()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.Inventory);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration;
                for(int i=0;i<4;i++)
                {
                    int tab=i;
                    Capture("Inventory_TabTarget_"+i+"_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                    {
                        var button=View().Tabs[tab];Rect art=Bounds(button.image.rectTransform,camera);
                        Click(button,camera,new Vector2(art.center.x,art.yMin-1));
                    });
                    Assert.That(View().SelectedTab,Is.EqualTo((InventoryTab)i));yield return WaitForNormalButtonColors();
                }
                Capture("Inventory_FooterBooster_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().NavButtons[3],camera));
                Assert.That(View().SelectedTab,Is.EqualTo(InventoryTab.Boosters));yield return WaitForNormalButtonColors();
                Capture("Inventory_TallBoosters_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().DynamicRoot.Find("Item_23").GetComponent<Button>(),camera));
                Assert.That(View().SelectedItem,Is.EqualTo(23));yield return WaitForNormalButtonColors();
                Capture("Inventory_TallPearl_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>
                {
                    Click(View().UseButton,camera);Assert.That(View().InfoPanel.activeSelf,Is.False);Assert.That(StableSave(),Is.EqualTo(save));
                    Click(View().DetailsButton,camera);
                });
                Assert.That(View().InfoPanel.activeSelf,Is.True);yield return WaitForNormalButtonColors();
                Capture("Inventory_TallDetails_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("Inventory_DetailsRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().CloseInfoButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.False);Assert.That(View().SelectedItem,Is.EqualTo(23));yield return WaitForNormalButtonColors();
                Capture("Inventory_VipTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().NavButtons[4],camera));
                Assert.That(View().InfoPanel.activeSelf,Is.True);Assert.That(View().InfoHeading.text,Is.EqualTo("VIP"));yield return WaitForNormalButtonColors();
                Capture("Inventory_TallVip_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().CloseInfoButton,camera));
                yield return WaitForNormalButtonColors();
                Capture("Inventory_GemsRoute_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().NavButtons[1],camera));
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Shop));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
                Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo("gems"));
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
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_INVENTORY_CAPTURE_DIR");
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
            var cameraGo = new GameObject("InventoryCaptureCamera",typeof(Camera));
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
        static void VerifyLayoutAndInput(ProductionInventoryView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(v.Header.HasBindings,Is.True);
            var header=Bounds(v.Header.Backing.rectTransform,camera);
            Assert.That(header.yMax,Is.GreaterThan(safeArea.yMax-20*safeArea.width/941f),"Header must follow the safe top edge.");
            var footer=Bounds(v.Body.Find("CarvedFooter").GetComponent<RectTransform>(),camera);
            Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin).Within(1),"Footer must follow the safe bottom edge.");
            float unit=Bounds(v.Body,camera).width/941f;
            for(int i=0;i<5;i++)
            {
                var button=v.NavButtons[i];Rect touch=Bounds((RectTransform)button.transform,camera);
                Assert.That(touch.yMin,Is.EqualTo(safeArea.yMin+16*unit).Within(.5f));
                Assert.That(Mathf.Min(touch.width,touch.height),Is.GreaterThanOrEqualTo(44));
                foreach(string name in new[]{"OriginalMedallion","LabelPlaque"})
                {
                    var art=button.transform.Find(name).GetComponent<Image>();Rect artwork=Bounds(art.rectTransform,camera);
                    Assert.That(art.type,Is.EqualTo(Image.Type.Simple));
                    Assert.That(art.sprite.rect.size,Is.EqualTo(new Vector2(art.sprite.texture.width,art.sprite.texture.height)),"Complete footer artwork");
                    Assert.That(artwork.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f));Assert.That(artwork.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f));Assert.That(artwork.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f));
                    if(i<4)Assert.That(artwork.xMax,Is.LessThan(Bounds((RectTransform)v.NavButtons[i+1].transform.Find(name),camera).xMin));
                }
            }
            foreach(var button in v.Tabs)
            {
                Rect visual=Bounds(button.image.rectTransform,camera);Vector4 padding=button.image.raycastPadding;
                Rect target=Rect.MinMaxRect(visual.xMin+padding.x*unit,visual.yMin+padding.y*unit,visual.xMax-padding.z*unit,visual.yMax-padding.w*unit);
                Assert.That(Mathf.Min(target.width,target.height),Is.GreaterThanOrEqualTo(44),"Category target at tested phone size");
                Assert.That(target.yMin,Is.GreaterThan(Bounds(v.PreviewLabel.rectTransform,camera).yMax),"Expanded category target clears grid notice");
                Assert.That(padding.w,Is.Zero,"No upward expansion towards Back");
                if(!v.InfoPanel.activeSelf)
                {
                    var pointer=new PointerEventData(EventSystem.current){position=new Vector2(target.center.x,target.yMin+1)};
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                }
            }
            foreach(var button in new[]{v.BackButton,v.UseButton,v.DetailsButton})
            {
                Rect target=Bounds((RectTransform)button.transform,camera);
                Assert.That(Mathf.Min(target.width,target.height),Is.GreaterThanOrEqualTo(44));
                if(button!=v.BackButton)Assert.That(target.yMin,Is.GreaterThan(Bounds((RectTransform)v.NavButtons[2].transform,camera).yMax),"Detail controls clear footer");
            }
            if(v.InfoPanel.activeSelf)
            {
                var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                var points=new List<Vector2>{Bounds(v.BackButton.image.rectTransform,camera).center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)};
                foreach(var button in v.NavButtons)points.Add(Bounds((RectTransform)button.transform,camera).center);
                foreach(var point in points)
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Modal blocks full safe viewport and navigation");
                }
            }
            foreach(var value in v.DynamicRoot.GetComponentsInChildren<Text>())if(value.name=="Count")Assert.That(value.font,Is.SameAs(v.NumberFont));
            var enclosure=v.Body.Find("GridFrame").GetComponent<Image>();Assert.That(enclosure.sprite,Is.SameAs(ProductionInventoryRuntime.Load("grid_frame_r2")));
            var frame=Bounds(enclosure.rectTransform,camera);var note=Bounds(v.PreviewLabel.rectTransform,camera);
            foreach(var item in v.DynamicRoot.GetComponentsInChildren<Button>()){
                var card=Bounds((RectTransform)item.transform,camera);
                Assert.That(card.yMax,Is.LessThan(note.yMin),"Item overlaps the preview label.");
                Assert.That(card.xMin,Is.GreaterThan(frame.xMin+20*safeArea.width/941f));Assert.That(card.xMax,Is.LessThan(frame.xMax-20*safeArea.width/941f));
                Assert.That(card.yMin,Is.GreaterThan(frame.yMin+20*safeArea.width/941f),"Item covers lower carving.");
            }
            var detailTitle=Bounds(v.ItemName.rectTransform,camera);var description=Bounds(v.Description.rectTransform,camera);
            var divider=Bounds(v.DetailDivider.rectTransform,camera);var usage=Bounds(v.UsageNote.rectTransform,camera);
            Assert.That(description.yMax,Is.LessThan(detailTitle.yMin),"Description overlaps item title.");
            Assert.That(description.yMin,Is.GreaterThan(divider.yMax),"Description obscures divider.");
            Assert.That(usage.yMax,Is.LessThan(divider.yMin),"Usage note obscures divider.");
            Assert.That(usage.yMin,Is.GreaterThan(Bounds((RectTransform)v.UseButton.transform,camera).yMax),"Usage note covers buttons.");
            Assert.That(v.DetailDivider.raycastTarget,Is.False);
            var quantityFrame=Bounds(v.Body.Find("QuantityPanel").GetComponent<RectTransform>(),camera);
            var quantityTitle=Bounds(v.QuantityLabel.rectTransform,camera);var quantityValue=Bounds(v.QuantityText.rectTransform,camera);
            Assert.That(quantityTitle.yMin,Is.GreaterThanOrEqualTo(quantityValue.yMax-1));
            Assert.That(quantityValue.yMin,Is.GreaterThan(quantityFrame.yMin));Assert.That(quantityValue.xMin,Is.GreaterThan(quantityFrame.xMin));Assert.That(quantityValue.xMax,Is.LessThan(quantityFrame.xMax));
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,button.name+" outside safe area");
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
