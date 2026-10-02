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
    public sealed class CharactersAlignmentPlayModeTests
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
            router.Show(ScreenId.Characters);Settle();yield return null;yield return null;
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

        ProductionCharactersView View()=>router.Viewport.GetComponentInChildren<ProductionCharactersView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest] public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
 var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(v.GoldText.text,Is.EqualTo(ProductionProfileView.Number(before+17)));string save=StableSave();yield return WaitForNormalButtonColors();Capture("Characters_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Characters_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Characters_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Characters_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator PreviewFiltersAndActionsNeverGrantOwnership(){
 var v=View();string save=StableSave();Assert.That(v.HasHeroOwnershipService||v.CanUpgrade,Is.False);for(int i=0;i<8;i++){v.Cards[i].onClick.Invoke();Assert.That(v.SelectedHero,Is.EqualTo(i));Assert.That(v.CharacterName.text,Is.EqualTo(ProductionCharactersView.NamesRu[i]));Assert.That(v.CurrentHero,Is.Zero);Assert.That(v.DetailArt.rectTransform.anchoredPosition.y-v.DetailArt.rectTransform.rect.height,Is.LessThan(-((RectTransform)v.DetailArt.transform.parent).rect.height),"Every preview cutoff lies below the portrait viewport");Assert.That(v.UpgradeButton.interactable||v.SelectButton.interactable,Is.False);v.UpgradeButton.onClick.Invoke();v.SelectButton.onClick.Invoke();foreach(var value in v.StatValues)Assert.That(value.text,Is.EqualTo("—"));foreach(var value in v.AbilityLevels)Assert.That(value.text,Is.EqualTo("—"));for(int j=0;j<8;j++){
 Assert.That(v.CardSelections[j].gameObject.activeSelf,Is.EqualTo(j==v.CurrentHero));
 string expectedFrame=j==v.CurrentHero?"card_border_r2":"card_normal_r6";
 Assert.That(v.CardFrames[j].sprite.name,Is.EqualTo(expectedFrame));Assert.That(v.Cards[j].image.sprite.name,Is.EqualTo(expectedFrame));
 }Assert.That(v.Availability.text,Is.EqualTo("ПРИМЕР"));}
 v.Tabs[1].onClick.Invoke();Assert.That(v.Filter,Is.EqualTo(HeroPreviewFilter.Rare));Assert.That(v.SelectedHero,Is.EqualTo(1));for(int i=0;i<8;i++)Assert.That(v.Cards[i].gameObject.activeSelf,Is.EqualTo(!ProductionCharactersView.IsLegendary(i)));v.PreviewHero(0);Assert.That(v.SelectedHero,Is.EqualTo(1));yield return WaitForNormalButtonColors();Capture("Characters_Rare_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
 v.Tabs[2].onClick.Invoke();Assert.That(v.SelectedHero,Is.Zero);for(int i=0;i<8;i++)Assert.That(v.Cards[i].gameObject.activeSelf,Is.EqualTo(ProductionCharactersView.IsLegendary(i)));v.Tabs[0].onClick.Invoke();v.PreviewHero(2);yield return WaitForNormalButtonColors();Capture("Characters_Firebird_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.PreviewHero(-1);v.SetFilter((HeroPreviewFilter)99);Assert.That(v.SelectedHero,Is.EqualTo(2));Assert.That(v.Filter,Is.EqualTo(HeroPreviewFilter.All));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator CurrentHeroOutlineAndLockedCardsScrollIndependently(){
 var v=View();string save=StableSave();v.PreviewHero(1);
 Assert.That(v.SelectedHero,Is.EqualTo(1));Assert.That(v.CardSelections[0].gameObject.activeSelf,Is.True);Assert.That(v.CardSelections[1].gameObject.activeSelf,Is.False);
 var locked=(RectTransform)v.FutureCards[0].transform;
 Assert.That(locked.rect.height,Is.EqualTo(((RectTransform)v.Cards[0].transform).rect.height));
 Assert.That(-locked.anchoredPosition.y,Is.LessThan(v.CollectionViewport.rect.height));
 // Whether a locked card is initially clipped depends on available phone height.
 v.PreviewHero(0);yield return WaitForNormalButtonColors();
 Capture("Characters_LockedScrolled_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672),camera=>{
     Assert.That(-locked.anchoredPosition.y+locked.rect.height,Is.GreaterThan(v.CollectionViewport.rect.height),"Reference-size viewport initially clips locked cards");
     Rect viewport=Bounds(v.CollectionViewport,camera);Vector2 start=viewport.center;
     var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=start,pointerPressRaycast=new RaycastResult{module=router.GameCanvas.GetComponent<GraphicRaycaster>()}};
     v.CollectionScroll.OnInitializePotentialDrag(pointer);v.CollectionScroll.OnBeginDrag(pointer);
     pointer.position=start+Vector2.up*400;v.CollectionScroll.OnDrag(pointer);v.CollectionScroll.OnEndDrag(pointer);v.CollectionScroll.StopMovement();Canvas.ForceUpdateCanvases();
     Assert.That(v.CollectionScroll.verticalNormalizedPosition,Is.EqualTo(0).Within(.01f));
     foreach(var future in v.FutureCards){Rect card=Bounds((RectTransform)future.transform,camera);Assert.That(viewport.Contains(card.min)&&viewport.Contains(card.max),Is.True,"Locked card fully revealed after upward drag");}
     VerifyLayoutAndInput(v,camera,new Vector2Int(941,1672),new Rect(0,0,941,1672));
 });
 DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();
 Capture("Characters_LockedScrolled_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{Rect viewport=Bounds(v.CollectionViewport,camera);foreach(var future in v.FutureCards){Rect card=Bounds((RectTransform)future.transform,camera);Assert.That(viewport.Contains(card.min)&&viewport.Contains(card.max),Is.True);}});
 v.SetFilter(HeroPreviewFilter.Rare);Canvas.ForceUpdateCanvases();Assert.That(v.CollectionContent.anchoredPosition.y,Is.EqualTo(0).Within(.01f));Assert.That(v.CollectionContent.rect.height,Is.EqualTo(v.CollectionViewport.rect.height));
 v.SetFilter(HeroPreviewFilter.All);Canvas.ForceUpdateCanvases();Assert.That(v.CollectionScroll.verticalNormalizedPosition,Is.EqualTo(1).Within(.01f));Assert.That(v.CardSelections[0].gameObject.activeSelf,Is.True);Assert.That(v.SelectedHero,Is.EqualTo(1));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator InfoBlocksUnderlyingInputAndBackDismisses(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;v.InfoButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.ProfileButton.onClick.Invoke();v.Tabs[1].onClick.Invoke();v.Cards[4].onClick.Invoke();Assert.That(v.SelectedHero,Is.Zero);Assert.That(v.Filter,Is.EqualTo(HeroPreviewFilter.All));Assert.That(router.RouteGeneration,Is.EqualTo(generation));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Characters_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Characters));v.InfoButton.onClick.Invoke();v.CloseInfoButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator NavigationUsesRealRoutesAndCommitsOnce(){
 var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.WorldMap,ScreenId.Shop,ScreenId.Events,ScreenId.TeamHub};string save=StableSave();for(int i=0;i<targets.Length;i++){router.Show(ScreenId.Characters);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.NavButtons[0],v.NavButtons[1],v.NavButtons[2],v.NavButtons[3]};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));if(i==2||i==3)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==2?"coins":"gems"));}
 router.Show(ScreenId.Home);Settle();yield return null;router.Show(ScreenId.Characters);Settle();yield return null;View().BackButton.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator RebindingAndDisableRemoveOwnedCallbacks(){
 var v=View();int calls=0;v.Bind(id=>calls++,()=>calls++);v.Bind(id=>calls++,()=>calls++);v.BackButton.interactable=false;v.BackButton.onClick.Invoke();Assert.That(calls,Is.Zero);v.BackButton.interactable=true;v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));v.gameObject.SetActive(false);v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));v.gameObject.SetActive(true);v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));v.Bind(id=>calls++,()=>calls++);v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(2));yield return null;
 }
 [UnityTest] public IEnumerator PressCancellationPreservesHeroAndDisabledActions(){
 var v=View();string save=StableSave();var b=v.Cards[1];var feedback=b.GetComponent<ProductionMapButtonFeedback>();
 var e=new PointerEventData(EventSystem.current){pointerId=19,button=PointerEventData.InputButton.Left};
 ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerDownHandler);yield return null;
 Assert.That(feedback.IsPressed,Is.True);Assert.That(b.targetGraphic.canvasRenderer.GetColor().r,Is.LessThan(.8f));
 Assert.That(v.SelectedHero,Is.Zero);Assert.That(v.CardSelections[0].gameObject.activeSelf,Is.True);
 ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerExitHandler);yield return new WaitForSecondsRealtime(.2f);
 Assert.That(feedback.IsPressed,Is.False);Assert.That(b.targetGraphic.canvasRenderer.GetColor().r,Is.EqualTo(1).Within(.001f));Assert.That(v.SelectedHero,Is.Zero);
 ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerClickHandler);Assert.That(v.SelectedHero,Is.EqualTo(1));Assert.That(v.CurrentHero,Is.Zero);
 ExecuteEvents.Execute(v.UpgradeButton.gameObject,e,ExecuteEvents.pointerDownHandler);Assert.That(v.UpgradeButton.GetComponent<ProductionMapButtonFeedback>().IsPressed,Is.False);
 Assert.That(v.UpgradeButton.targetGraphic.color.r,Is.EqualTo(.62f).Within(.001f));Assert.That(StableSave(),Is.EqualTo(save));
 }
 [UnityTest] public IEnumerator OriginalLetteringSwitchesWithLocaleAndSelectedTab(){
 var v=View();string save=StableSave();
 Assert.That(v.Title.sprite,Is.SameAs(v.TitleRu));Assert.That(v.Heading.enabled||v.UpgradeLabel.enabled||v.SelectLabel.enabled,Is.False);
 Assert.That(v.UpgradeButton.image.sprite,Is.SameAs(v.UpgradeRu));Assert.That(v.SelectButton.image.sprite,Is.SameAs(v.SelectRu));
 for(int filter=0;filter<3;filter++){v.SetFilter((HeroPreviewFilter)filter);for(int i=0;i<3;i++){Assert.That(v.TabLabels[i].enabled,Is.False);Assert.That(v.Tabs[i].image.sprite,Is.SameAs(((i==0)==(i==filter))?v.TabSourceRu[i]:v.TabAlternateRu[i]));}}
 for(int i=0;i<4;i++){Assert.That(v.NavLabels[i].enabled,Is.False);Assert.That(v.NavCaptions[i].sprite,Is.SameAs(v.NavRu[i]));}
 DemoFeatureState.SetLanguage("en");v.RefreshContent();Assert.That(v.Heading.enabled&&v.SelectLabel.enabled&&v.UpgradeLabel.enabled,Is.True);
 foreach(var t in v.TabLabels)Assert.That(t.enabled,Is.True);foreach(var t in v.NavLabels)Assert.That(t.enabled,Is.True);
 Assert.That(v.UpgradeButton.interactable||v.SelectButton.interactable,Is.False);Assert.That(StableSave(),Is.EqualTo(save));yield return null;
 }
 [UnityTest] public IEnumerator SafeEdgesResizeCollectionAndModalWithoutChangingPreview(){
 var v=View();v.PreviewHero(1);string save=StableSave();
 Capture("Characters_941x1412_short_RU",new Vector2Int(941,1412),new Rect(0,24,941,1358));
 Capture("Characters_576x1280_Expanded_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{
 Assert.That(v.CollectionViewport.rect.height,Is.GreaterThan(742));Assert.That(v.CollectionContent.anchoredPosition.y,Is.Zero.Within(.01f));
 Rect area=Bounds(v.CollectionViewport,camera);foreach(var future in v.FutureCards){Rect card=Bounds((RectTransform)future.transform,camera);Assert.That(area.Contains(card.min)&&area.Contains(card.max),Is.True,"Tall display reveals full future cards without scrolling");}
 });
 Assert.That(v.SelectedHero,Is.EqualTo(1));Assert.That(v.CurrentHero,Is.Zero);
 v.ShowInfo();yield return WaitForNormalButtonColors();Capture("Characters_Info_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>{
 var safe=new Rect(45,100,990,2160);var shade=Bounds((RectTransform)v.InfoPanel.transform.Find("InputShield"),camera);
 Assert.That(shade.yMin,Is.EqualTo(safe.yMin).Within(1));Assert.That(shade.yMax,Is.EqualTo(safe.yMax).Within(1));
 foreach(var button in new[]{v.ProfileButton,v.BackButton}){var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=Bounds((RectTransform)button.transform,camera).center},hits);Assert.That(hits[0].gameObject.transform.IsChildOf(v.InfoPanel.transform),Is.True,"Info blocks header and footer at safe edges");}
 });v.CloseInfo();Assert.That(StableSave(),Is.EqualTo(save));yield return null;
 }
 [UnityTest] public IEnumerator NativeHeaderFooterAndBackCommitOnce(){
 string save=StableSave();var targets=new[]{ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.Profile,ScreenId.Settings,ScreenId.WorldMap,ScreenId.Shop,ScreenId.Events,ScreenId.TeamHub,ScreenId.Home};
 for(int i=0;i<targets.Length;i++){
 router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Characters);Settle();yield return null;yield return WaitForNormalButtonColors();var v=View();int generation=router.RouteGeneration;
 Capture("Characters_NativeRoute_"+i,new Vector2Int(576,1280),new Rect(0,48,576,1172),afterCapture:camera=>{
 var buttons=new[]{v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.ProfileButton,v.SettingsButton,v.NavButtons[0],v.NavButtons[1],v.NavButtons[2],v.NavButtons[3],v.BackButton};var button=buttons[i];Rect target=Target(button,camera);Click(button,camera,i<3?new Vector2(target.xMin+2,target.center.y):target.center);button.onClick.Invoke();
 });Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));if(i<2)Assert.That(router.Viewport.GetComponentInChildren<ProductionShopView>().SelectedCategory,Is.EqualTo(i==0?"coins":"gems"));Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 [UnityTest] public IEnumerator NativeFiltersPreviewInfoAndRelocalizationPreserveCurrentHero(){
 var v=View();string save=StableSave();int generation=router.RouteGeneration;
 foreach(string language in new[]{"ru","en"}){
 DemoFeatureState.SetLanguage(language);v.RefreshContent();v.SetFilter(HeroPreviewFilter.All);yield return null;yield return WaitForNormalButtonColors();
 Capture("Characters_NativeRare_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),afterCapture:camera=>{Rect r=Target(v.Tabs[1],camera);Click(v.Tabs[1],camera,new Vector2(r.center.x,r.yMin+2));});
 Assert.That(v.Filter,Is.EqualTo(HeroPreviewFilter.Rare));yield return null;
 Capture("Characters_NativePreview_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),afterCapture:camera=>Click(v.Cards[2],camera,Target(v.Cards[2],camera).center));
 Assert.That(v.SelectedHero,Is.EqualTo(2));Assert.That(v.CurrentHero,Is.Zero);Assert.That(v.CardSelections[0].gameObject.activeSelf,Is.True);Assert.That(v.CardSelections[2].gameObject.activeSelf,Is.False);
 Capture("Characters_NativeInfoTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),afterCapture:camera=>{Rect r=Target(v.InfoButton,camera);Click(v.InfoButton,camera,new Vector2(r.center.x,r.yMax-2));});
 Assert.That(v.InfoPanel.activeSelf,Is.True);DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");yield return null;yield return WaitForNormalButtonColors();
 Capture("Characters_RelocalizedInfo_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),afterCapture:camera=>{Rect r=Target(v.CloseInfoButton,camera);Click(v.CloseInfoButton,camera,new Vector2(r.xMin+2,r.center.y));});
 Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(v.SelectedHero,Is.EqualTo(2));Assert.That(v.Filter,Is.EqualTo(HeroPreviewFilter.Rare));Assert.That(v.CharacterName.text,Is.EqualTo(language=="ru"?"Firebird":"Жар-Птица"));Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));
 }
 }
 static Rect Target(Button button,Camera camera){Rect art=Bounds(button.image.rectTransform,camera);float scale=art.width/button.image.rectTransform.rect.width;Vector4 p=button.image.raycastPadding;return Rect.MinMaxRect(art.xMin+p.x*scale,art.yMin+p.y*scale,art.xMax-p.z*scale,art.yMax-p.w*scale);}
 static void Click(Button button,Camera camera,Vector2 point){var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" native target");ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);}
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

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null, Action<Camera> afterCapture = null)
        {
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_CHARACTERS_CAPTURE_DIR");
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
            var cameraGo = new GameObject("CharactersCaptureCamera",typeof(Camera));
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
                camera.Render();
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionCharactersView>(),camera,size,safeArea);
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
                afterCapture?.Invoke(camera);
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
        static void VerifyLayoutAndInput(ProductionCharactersView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect hud=Bounds(v.Hud,camera),footer=Bounds((RectTransform)v.Body.Find("CarvedFooter"),camera);
            Assert.That(hud.yMax,Is.EqualTo(safeArea.yMax).Within(1),"Header at safe top");
            Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin).Within(1),"Footer at safe bottom");
            Assert.That(Bounds(v.CollectionViewport,camera).yMin,Is.GreaterThanOrEqualTo(footer.yMax-2),"Collection cannot cover footer");
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Target(button,camera);
                bool clipped=button.transform.IsChildOf(v.CollectionContent)&&!Bounds(v.CollectionViewport,camera).Contains(rect.center);
                if(clipped){var clippedHits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},clippedHits);Assert.That(clippedHits.Exists(hit=>hit.gameObject.GetComponentInParent<Button>()==button),Is.False,"Masked card cannot intercept header/footer");continue;}
                // Shared header hit areas intentionally meet the safe-area edge. Rect.Contains excludes its maximum.
                Assert.That(rect.xMin>=safeArea.xMin-1 && rect.yMin>=safeArea.yMin-1 && rect.xMax<=safeArea.xMax+1 && rect.yMax<=safeArea.yMax+1,Is.True,button.name+" outside safe area");
                if(button.IsInteractable()){Assert.That(rect.width,Is.GreaterThanOrEqualTo(44),button.name+" touch width");Assert.That(rect.height,Is.GreaterThanOrEqualTo(44),button.name+" touch height");}
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" hit "+hits[0].gameObject.name);

            }
            Assert.That(Bounds(v.CollectionViewport,camera).yMin,Is.GreaterThan(Target(v.InfoButton,camera).yMax),"Info target must not steal collection drags");
            var fixedButtons=new List<Button>{v.ProfileButton,v.SettingsButton,v.BackButton,v.InfoButton};fixedButtons.AddRange(v.PlusButtons);fixedButtons.AddRange(v.Tabs);fixedButtons.AddRange(v.NavButtons);
            for(int i=0;i<fixedButtons.Count;i++)for(int j=i+1;j<fixedButtons.Count;j++)Assert.That(Target(fixedButtons[i],camera).Overlaps(Target(fixedButtons[j],camera)),Is.False,fixedButtons[i].name+" overlaps "+fixedButtons[j].name);
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.transform.IsChildOf(v.CollectionContent)&&!Bounds(v.CollectionViewport,camera).Overlaps(Bounds(text.rectTransform,camera)))continue;
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name+" visible glyph mesh");
                    Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(text.text.Length),text.name+" complete text");
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
