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
    public sealed class ProfileAlignmentPlayModeTests
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
            router.Show(ScreenId.Profile);Settle();yield return null;yield return null;
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

        ProductionProfileView View()=>router.Viewport.GetComponentInChildren<ProductionProfileView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            var v=View();Assert.That(v.HasRequiredBindings&&v.ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(v.GoldText.text,Is.EqualTo(ProductionProfileView.Number(before+17)));string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("Profile_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Profile_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("Profile_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Capture("Profile_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("Profile_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.Home,ScreenId.IndividualRanking,ScreenId.TeamHub,ScreenId.Inventory,ScreenId.Login};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.Profile);Settle();yield return null;var v=View();Button b;if(i<5)b=new[]{v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton}[i];else{v.ShowInfo();b=v.RouteButtons[i-5];}int generation=router.RouteGeneration;b.onClick.Invoke();b.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator CollectionsAndOwnedItemsNeverGrantPreviewRewards(){
            var v=View();Assert.That(v.HasOnlineProfile,Is.False);GameState.Instance.Save.boosterMace=7;GameState.Instance.Save.boosterSeaPearl=13;string save=StableSave();v.Tabs[1].onClick.Invoke();Assert.That(v.SelectedTab,Is.EqualTo(ProfileTab.Relics));Assert.That(v.DynamicRoot.Find("Hero_0"),Is.Null);v.DynamicRoot.Find("Relic_5").GetComponent<Button>().onClick.Invoke();Assert.That(v.SelectedRelic,Is.EqualTo(5));yield return WaitForNormalButtonColors();Capture("Profile_Relics_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.Tabs[2].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Quantity_0").GetComponent<Text>().text,Is.EqualTo("7"));Assert.That(v.DynamicRoot.Find("Quantity_3").GetComponent<Text>().text,Is.EqualTo("13"));yield return WaitForNormalButtonColors();Capture("Profile_Items_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_Items_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.Tabs[3].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Hero_4"),Is.Not.Null);Assert.That(StableSave(),Is.EqualTo(save));Assert.Throws<ArgumentOutOfRangeException>(()=>v.SelectTab((ProfileTab)9));
        }
        [UnityTest]public IEnumerator RelicSelectionRemainsDistinctAcrossLanguagesAndLayouts(){
            var v=View();string save=StableSave();
            foreach(string language in new[]{"ru","en"}){
                DemoFeatureState.SetLanguage(language);v.RefreshContent();
                for(int i=0;i<6;i++){
                    v.SelectRelic(i);yield return WaitForNormalButtonColors();
                    for(int j=0;j<6;j++){
                        var name=v.DynamicRoot.Find("Relic_"+j+"/Name").GetComponent<Text>();
                        Assert.That(name.color.b>name.color.r,Is.EqualTo(i==j),"Only the inspected relic has the sapphire caption.");
                    }
                    var detail=v.DynamicRoot.Find("SelectedRelic");var caption=detail.Find("DetailCaption");
                    Assert.That(caption!=null,Is.EqualTo(i!=0));
                    if(caption)Assert.That(caption.GetComponent<Text>().text,Is.EqualTo(v.DynamicRoot.Find("Relic_"+i+"/Name").GetComponent<Text>().text));
                    Assert.That(v.DynamicRoot.Find("RelicTitle").GetComponent<Text>().font,Is.SameAs(v.TitleFont));
                    Capture("Profile_Selected_"+i+"_"+language.ToUpperInvariant(),language=="en"?new Vector2Int(576,1280):new Vector2Int(941,1672),language=="en"?new Rect(0,48,576,1172):new Rect(0,0,941,1672));
                }
                v.SelectTab(ProfileTab.Relics);yield return WaitForNormalButtonColors();
                Assert.That(v.SelectedRelic,Is.EqualTo(5));
                Capture("Profile_SelectionGrid_"+language.ToUpperInvariant(),language=="en"?new Vector2Int(576,1280):new Vector2Int(941,1672),language=="en"?new Rect(0,48,576,1172):new Rect(0,0,941,1672));
                v.SelectTab(ProfileTab.Heroes);
            }
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator ChapterProgressDisplaysEmptyPartialAndCompleteWithoutMutation(){
            var v=View();var catalog=ChapterLevelCatalog.LoadChapter1();
            foreach(string language in new[]{"ru","en"}){
                DemoFeatureState.SetLanguage(language);v.RefreshContent();
                foreach(int completed in new[]{0,catalog.levels.Length/2,catalog.levels.Length}){
                    GameState.Instance.Save.levelProgress.Clear();
                    for(int i=0;i<completed;i++)GameState.Instance.Save.levelProgress.Add(new LevelProgressData{chapterId=catalog.chapterId,levelId=catalog.levels[i].levelId,completions=1,bestStars=1});
                    string save=StableSave();v.RefreshState();yield return WaitForNormalButtonColors();
                    Assert.That(v.ProgressChapterLabel.text,Is.EqualTo((language=="en"?"Chapter ":"Глава ")+catalog.chapterId));
                    Assert.That(v.ProgressLabel.text,Is.EqualTo(completed+" / "+catalog.levels.Length));
                    Assert.That(v.ProgressFill.fillAmount,Is.EqualTo((float)completed/catalog.levels.Length).Within(.0001));
                    Assert.That(v.ProgressLabel.font,Is.SameAs(v.NumberFont));
                    Capture("Profile_Progress_"+completed+"_"+language.ToUpperInvariant(),language=="en"?new Vector2Int(576,1280):new Vector2Int(941,1672),language=="en"?new Rect(0,48,576,1172):new Rect(0,0,941,1672));
                    Assert.That(StableSave(),Is.EqualTo(save));
                }
            }
        }
        [UnityTest]public IEnumerator PressFeedbackRestoresWithoutMovingOrActivatingControls(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;yield return WaitForNormalButtonColors();
            var pointer=new PointerEventData(EventSystem.current){pointerId=41,button=PointerEventData.InputButton.Left};
            Button[] controls={v.Tabs[1],v.DynamicRoot.Find("Relic_2").GetComponent<Button>(),v.FooterButtons[1]};
            string[] names={"Tab","Relic","Footer"};
            for(int i=0;i<controls.Length;i++){
                var button=controls[i];var feedback=button.GetComponent<ProductionMapButtonFeedback>();
                var rect=(RectTransform)button.transform;Vector2 position=rect.anchoredPosition,size=rect.sizeDelta;
                var graphics=button.GetComponentsInChildren<Graphic>(true);var normal=new Color[graphics.Length];
                for(int j=0;j<graphics.Length;j++)normal[j]=graphics[j].canvasRenderer.GetColor();
                ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerDownHandler);
                Assert.That(feedback.IsPressed,Is.True);
                for(int j=0;j<graphics.Length;j++)Assert.That(graphics[j].canvasRenderer.GetColor().r,Is.LessThan(normal[j].r*.8f),"Art and caption should both respond.");
                Assert.That(rect.anchoredPosition,Is.EqualTo(position));Assert.That(rect.sizeDelta,Is.EqualTo(size));
                Capture("Profile_Pressed_"+names[i]+"_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                ExecuteEvents.Execute(button.gameObject,new PointerEventData(EventSystem.current){pointerId=42},ExecuteEvents.pointerUpHandler);
                Assert.That(feedback.IsPressed,Is.True,"Unrelated pointer cannot release the press.");
                if(i==1)ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerExitHandler);else ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerUpHandler);
                yield return new WaitForSecondsRealtime(.2f);
                Assert.That(feedback.IsPressed,Is.False);
                for(int j=0;j<graphics.Length;j++)Assert.That(graphics[j].canvasRenderer.GetColor(),Is.EqualTo(normal[j]));
            }
            var tab=v.Tabs[1];var tabFeedback=tab.GetComponent<ProductionMapButtonFeedback>();
            tab.interactable=false;ExecuteEvents.Execute(tab.gameObject,pointer,ExecuteEvents.pointerDownHandler);Assert.That(tabFeedback.IsPressed,Is.False);tab.interactable=true;
            ExecuteEvents.Execute(tab.gameObject,pointer,ExecuteEvents.pointerDownHandler);tabFeedback.enabled=false;
            Assert.That(tabFeedback.IsPressed,Is.False);Assert.That(tab.image.canvasRenderer.GetColor(),Is.EqualTo(Color.white));tabFeedback.enabled=true;
            Assert.That(v.SelectedTab,Is.EqualTo(ProfileTab.Heroes));Assert.That(v.SelectedRelic,Is.Zero);Assert.That(v.Section,Is.EqualTo(ProfileSection.Profile));
            Assert.That(router.RouteGeneration,Is.EqualTo(generation));Assert.That(StableSave(),Is.EqualTo(save));
            DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();
            ExecuteEvents.Execute(v.BackButton.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            Capture("Profile_Pressed_Back_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            ExecuteEvents.Execute(v.BackButton.gameObject,pointer,ExecuteEvents.pointerUpHandler);yield return new WaitForSecondsRealtime(.2f);
        }
        [UnityTest]public IEnumerator HistoryBoundaryControlsAreDisabledAndDoNotRebuild(){
            var v=View();var pointer=new PointerEventData(EventSystem.current){pointerId=51,button=PointerEventData.InputButton.Left};
            foreach(int count in new[]{0,1,6,7,12}){
                DemoFeatureState.SetLanguage("ru");GameState.Instance.Save.levelProgress.Clear();
                for(int i=0;i<count;i++)GameState.Instance.Save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=i+1,attempts=1,bestScore=100*(i+1)});
                v.SelectSection(ProfileSection.History);string save=StableSave();yield return WaitForNormalButtonColors();
                if(count==0){Assert.That(v.DynamicRoot.Find("Previous"),Is.Null);Assert.That(v.DynamicRoot.Find("Next"),Is.Null);continue;}
                var previous=v.DynamicRoot.Find("Previous").GetComponent<Button>();var next=v.DynamicRoot.Find("Next").GetComponent<Button>();
                Assert.That(previous.interactable,Is.False);Assert.That(next.interactable,Is.EqualTo(count>6));
                Assert.That(previous.image.color.r,Is.LessThan(.6f));
                var firstRecord=v.DynamicRoot.Find("Record_0");previous.onClick.Invoke();v.ChangeHistoryPage(int.MinValue);
                Assert.That(v.DynamicRoot.Find("Record_0"),Is.SameAs(firstRecord),"Unavailable navigation must preserve existing content.");
                ExecuteEvents.Execute(previous.gameObject,pointer,ExecuteEvents.pointerDownHandler);Assert.That(previous.GetComponent<ProductionMapButtonFeedback>().IsPressed,Is.False);
                if(count==6)Capture("Profile_HistorySinglePage_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                if(count==7)Capture("Profile_HistoryFirstPage_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                if(count>6){
                    next.onClick.Invoke();Assert.That(v.HistoryPage,Is.EqualTo(1));
                    var lastRecord=v.DynamicRoot.Find("Record_6");next=v.DynamicRoot.Find("Next").GetComponent<Button>();previous=v.DynamicRoot.Find("Previous").GetComponent<Button>();
                    Assert.That(next.interactable,Is.False);Assert.That(previous.interactable,Is.True);
                    next.onClick.Invoke();v.ChangeHistoryPage(int.MaxValue);Assert.That(v.DynamicRoot.Find("Record_6"),Is.SameAs(lastRecord));
                    if(count==12){DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_HistoryLastPage_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));}
                    v.DynamicRoot.Find("Previous").GetComponent<Button>().onClick.Invoke();Assert.That(v.HistoryPage,Is.Zero);
                }
                Assert.That(StableSave(),Is.EqualTo(save));
            }
        }
        [UnityTest]public IEnumerator HistoryStatisticsStayReadableWithMaximumValues(){
            var v=View();GameState.Instance.Save.levelProgress.Clear();
            for(int i=0;i<6;i++)GameState.Instance.Save.levelProgress.Add(new LevelProgressData{chapterId=20,levelId=500-i,bestStars=i%4,bestScore=i==0?int.MaxValue:0,attempts=i==0?int.MaxValue:0});
            string save=StableSave();
            foreach(string language in new[]{"ru","en"}){
                DemoFeatureState.SetLanguage(language);v.SelectSection(ProfileSection.History);v.RefreshContent();yield return WaitForNormalButtonColors();
                var row=v.DynamicRoot.Find("Record_0");
                Assert.That(row.Find("Heading").GetComponent<Text>().text,Is.EqualTo(language=="en"?"Chapter 20 · Level 500":"Глава 20 · Уровень 500"));
                Assert.That(row.Find("StatValue_0").GetComponent<Text>().text,Is.EqualTo("0 / 3"));
                Assert.That(row.Find("StatValue_1").GetComponent<Text>().text,Is.EqualTo("2 147 483 647"));
                Assert.That(row.Find("StatValue_2").GetComponent<Text>().text,Is.EqualTo("2 147 483 647"));
                Capture("Profile_HistoryMax_safe_"+language.ToUpperInvariant(),new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>{
                    for(int i=0;i<6;i++){
                        var record=v.DynamicRoot.Find("Record_"+i);var frame=Bounds((RectTransform)record,camera);
                        var heading=Bounds(record.Find("Heading").GetComponent<RectTransform>(),camera);
                        float previousRight=frame.xMin;
                        for(int column=0;column<3;column++){
                            var label=record.Find("StatLabel_"+column).GetComponent<Text>();var value=record.Find("StatValue_"+column).GetComponent<Text>();
                            var labelBounds=Bounds(label.rectTransform,camera);var valueBounds=Bounds(value.rectTransform,camera);
                            Assert.That(value.font,Is.SameAs(v.NumberFont));
                            Assert.That(labelBounds.yMax,Is.LessThan(heading.yMin),"Statistics must stay below the level heading.");
                            Assert.That(valueBounds.yMax,Is.LessThan(labelBounds.yMin),"Values must not cover labels.");
                            Assert.That(valueBounds.yMin,Is.GreaterThan(frame.yMin),"Values must clear the lower frame.");
                            Assert.That(valueBounds.xMin,Is.GreaterThan(previousRight),"Statistic columns must not overlap.");
                            Assert.That(valueBounds.xMax,Is.LessThan(frame.xMax));previousRight=valueBounds.xMax;
                        }
                    }
                });
                Assert.That(StableSave(),Is.EqualTo(save));
            }
        }
        [UnityTest]public IEnumerator RealProgressMilestonesAndPagedHistory(){
            var v=View();GameState.Instance.Save.levelProgress.Clear();v.RefreshState();v.FooterButtons[2].onClick.Invoke();Assert.That(v.DynamicRoot.Find("Empty"),Is.Not.Null);yield return WaitForNormalButtonColors();Capture("Profile_EmptyHistory_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            for(int i=1;i<=8;i++)GameState.Instance.Save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=i,bestStars=i%3+1,bestScore=i*1200,attempts=i,completions=1});
            string save=StableSave();v.RefreshState();Assert.That(v.CompletedCount,Is.EqualTo(8));Assert.That(v.BestScore,Is.EqualTo(9600));Assert.That(v.AttemptCount,Is.EqualTo(36));Assert.That(v.ChapterCompleted,Is.EqualTo(8));Assert.That(v.ProgressFill.preserveAspect,Is.False,"Progress fill must span the full track rather than letterbox the source tab sprite.");Assert.That(v.ProgressFill.fillAmount,Is.EqualTo(8f/v.ChapterTotal).Within(.0001));yield return WaitForNormalButtonColors();Capture("Profile_History_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));v.DynamicRoot.Find("Next").GetComponent<Button>().onClick.Invoke();Assert.That(v.HistoryPage,Is.EqualTo(1));v.ChangeHistoryPage(100);Assert.That(v.HistoryPage,Is.EqualTo(1));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_HistoryPage2_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));v.FooterButtons[1].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("Profile_Achievements_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Assert.That(v.DynamicRoot.Find("MilestoneStatus_0").GetComponent<Text>().text,Is.EqualTo("REACHED"));Assert.That(v.DynamicRoot.Find("MilestoneStatus_2").GetComponent<Text>().text,Is.EqualTo("NOT YET REACHED"));v.ProfileButton.onClick.Invoke();Assert.That(v.Section,Is.EqualTo(ProfileSection.Profile));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator IdentityExplanationBlocksUnderlyingInputAndBackDismisses(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;v.EditButton.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);v.SettingsButton.onClick.Invoke();v.Tabs[2].onClick.Invoke();v.SelectTab(ProfileTab.Items);v.SelectSection(ProfileSection.History);v.SelectRelic(3);Assert.That(v.Section,Is.EqualTo(ProfileSection.Profile));Assert.That(v.SelectedTab,Is.EqualTo(ProfileTab.Heroes));Assert.That(v.SelectedRelic,Is.Zero);Assert.That(router.RouteGeneration,Is.EqualTo(generation));yield return WaitForNormalButtonColors();Capture("Profile_Info_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();yield return WaitForNormalButtonColors();Capture("Profile_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Profile));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator MaximumBalancesAndRepeatedBind(){
            var v=View();var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;save.levelProgress=new List<LevelProgressData>{new LevelProgressData{chapterId=1,levelId=1,attempts=int.MaxValue,bestScore=int.MaxValue},new LevelProgressData{chapterId=1,levelId=2,attempts=int.MaxValue}};v.RefreshState();Assert.That(v.AttemptCount,Is.EqualTo(4294967294L));yield return WaitForNormalButtonColors();Capture("Profile_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));int calls=0;v.Bind(id=>calls++,()=>calls++);v.Bind(id=>calls++,()=>calls++);v.BackButton.onClick.Invoke();v.BackButton.onClick.Invoke();Assert.That(calls,Is.EqualTo(1));
        }
        [UnityTest]public IEnumerator NativeSectionsExpandedActionsAndTallModalPreserveSave()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.Profile);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration;
                Capture("Profile_FooterTargets_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().FooterButtons[1],camera));
                Assert.That(View().Section,Is.EqualTo(ProfileSection.Achievements));yield return WaitForNormalButtonColors();
                Capture("Profile_TallAchievements_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().FooterButtons[2],camera));
                Assert.That(View().Section,Is.EqualTo(ProfileSection.History));yield return WaitForNormalButtonColors();
                Capture("Profile_TallHistory_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>Click(View().FooterButtons[0],camera));
                Assert.That(View().Section,Is.EqualTo(ProfileSection.Profile));yield return WaitForNormalButtonColors();
                Capture("Profile_EditTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    Rect visual=Bounds(View().EditButton.image.rectTransform,camera);
                    Click(View().EditButton,camera,new Vector2(visual.xMin-4,visual.center.y));
                });
                Assert.That(View().InfoPanel.activeSelf,Is.True);yield return WaitForNormalButtonColors();
                Capture("Profile_TallActions_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("Profile_ActionsRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().CloseInfoButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.False);yield return WaitForNormalButtonColors();
                Capture("Profile_CollectionTarget_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    var action=View().DynamicRoot.Find("CollectionInfo").GetComponent<Button>();Rect visual=Bounds(action.image.rectTransform,camera);
                    Click(action,camera,new Vector2(visual.center.x,visual.yMax+4));
                });
                Assert.That(View().InfoPanel.activeSelf,Is.True);yield return WaitForNormalButtonColors();
                Capture("Profile_InventoryRoute_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().RouteButtons[2],camera));
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Inventory));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
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
        static Rect TouchBounds(Button button,Camera camera,float unit)
        {
            Rect visual=Bounds(button.image.rectTransform,camera);Vector4 p=button.image.raycastPadding;
            return Rect.MinMaxRect(visual.xMin+p.x*unit,visual.yMin+p.y*unit,visual.xMax-p.z*unit,visual.yMax-p.w*unit);
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
                    // Custom feedback uses CanvasRenderer tint; modal CanvasGroups block input without a ColorTint transition.
                    Color expected=button.transition==Selectable.Transition.None?Color.white:(button.IsInteractable()?button.colors.normalColor:button.colors.disabledColor)*button.colors.colorMultiplier;
                    var feedback=button.GetComponent<ProductionMapButtonFeedback>();if(feedback&&feedback.IsPressed)ready=false;
                    Color actual=button.targetGraphic.canvasRenderer.GetColor();
                    if(Mathf.Max(Mathf.Abs(actual.r-expected.r),Mathf.Abs(actual.g-expected.g),
                        Mathf.Abs(actual.b-expected.b),Mathf.Abs(actual.a-expected.a))>.0001f)ready=false;
                }
                stable=ready?stable+1:0;
            } while(stable<2&&Time.realtimeSinceStartup<deadline);
            Assert.That(stable,Is.EqualTo(2),"Capture only after the active button feedback has returned to its resting tint.");
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
            string directory = Environment.GetEnvironmentVariable("LOR_PROFILE_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ProfileCaptureCamera",typeof(Camera));
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
        static void VerifyLayoutAndInput(ProductionProfileView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            float unit=Bounds(v.Body,camera).width/941f;
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
            Assert.That(Bounds(v.BackButton.image.rectTransform,camera).yMin,Is.EqualTo(safeArea.yMin+17*unit).Within(.5f));
            Assert.That(Bounds((RectTransform)v.Body.Find("CarvedFooter"),camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            for(int i=0;i<3;i++)
            {
                Rect footer=Bounds(v.FooterButtons[i].image.rectTransform,camera);
                Assert.That(footer.yMin,Is.EqualTo(safeArea.yMin+37*unit).Within(.5f));
                Assert.That(Mathf.Min(footer.width,footer.height),Is.GreaterThanOrEqualTo(44));
                if(i<2)Assert.That(footer.xMax,Is.LessThan(Bounds(v.FooterButtons[i+1].image.rectTransform,camera).xMin));
                var art=v.FooterButtons[i].transform.Find("Icon").GetComponent<Image>();
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple));Assert.That(art.preserveAspect,Is.True);
                Assert.That(art.sprite.rect.size,Is.EqualTo(new Vector2(art.sprite.texture.width,art.sprite.texture.height)));
                Rect iconBounds=Bounds(art.rectTransform,camera);Assert.That(footer.Contains(iconBounds.min)&&footer.Contains(iconBounds.max),Is.True);
            }
            var targets=new List<Button>{v.EditButton};targets.AddRange(v.Tabs);
            foreach(string name in new[]{"CollectionInfo","Previous","Next"}){var t=v.DynamicRoot.Find(name);if(t)targets.Add(t.GetComponent<Button>());}
            foreach(var button in targets)
            {
                Rect touch=TouchBounds(button,camera,unit);
                Assert.That(Mathf.Min(touch.width,touch.height),Is.GreaterThanOrEqualTo(44),button.name+" touch area on tested phone size");
                Assert.That(safeArea.Contains(touch.min)&&safeArea.Contains(touch.max),Is.True);
                if(button.name=="CollectionInfo")Assert.That(touch.yMin,Is.GreaterThan(Bounds(v.FooterButtons[0].image.rectTransform,camera).yMax),"Collection action clears footer");
                if(!v.InfoPanel.activeSelf)
                foreach(var point in new[]{new Vector2(touch.center.x,touch.yMin+1),new Vector2(touch.center.x,touch.yMax-1)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                }
            }
            Assert.That(TouchBounds(v.EditButton,camera,unit).yMin,Is.GreaterThan(TouchBounds(v.Tabs[1],camera,unit).yMax),"Expanded Edit target clears tabs");
            if(v.InfoPanel.activeSelf)
            {
                var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
                Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
                foreach(var point in new[]{Bounds(v.BackButton.image.rectTransform,camera).center,Bounds(v.FooterButtons[1].image.rectTransform,camera).center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Profile modal blocks full safe height");
                }
            }
            foreach(var value in v.StatValues)Assert.That(value.font,Is.SameAs(v.NumberFont),"Stat numeral font must survive locale and section changes.");
            for(int i=0;i<6;i++){
                Rect label=Bounds(v.StatLabels[i].rectTransform,camera),value=Bounds(v.StatValues[i].rectTransform,camera);
                Assert.That(label.yMin,Is.GreaterThanOrEqualTo(value.yMax-1),"Stat label overlaps value: "+i);
            }
            for(int i=0;i<6;i++){
                var card=v.DynamicRoot.Find("Relic_"+i);if(!card)continue;
                var caption=card.Find("Name").GetComponent<Text>();
                Assert.That(caption.fontSize,Is.GreaterThanOrEqualTo(15),"Relic name must remain readable.");
                Rect nameRect=Bounds(caption.rectTransform,camera),cardRect=Bounds((RectTransform)card,camera);
                Assert.That(nameRect.yMin,Is.GreaterThan(cardRect.yMin+2),"Relic name overlaps the lower frame.");
            }
            var selectedImage=v.DynamicRoot.Find("SelectedRelic");
            if(selectedImage&&selectedImage.Find("DetailCaption")){
                var art=selectedImage.GetComponent<Image>();Rect container=Bounds(art.rectTransform,camera);
                Vector2 source=art.sprite.rect.size;float factor=Mathf.Min(container.width/source.x,container.height/source.y);
                float renderedWidth=source.x*factor;
                Rect caption=Bounds((RectTransform)selectedImage.Find("DetailCaption"),camera);
                Assert.That(caption.center.x,Is.EqualTo(container.xMin+renderedWidth*.5f).Within(1),"Detail caption must follow the visible top-left-aligned sprite, not its wider container.");
                Assert.That(caption.xMax,Is.LessThan(container.xMin+renderedWidth),"Detail caption crosses the visible frame.");
            }
            Rect chapterPlaque=Bounds((RectTransform)v.Body.Find("ChapterPlaque"),camera),progressTrack=Bounds((RectTransform)v.Body.Find("ProgressTrack"),camera),chapterText=Bounds(v.ProgressChapterLabel.rectTransform,camera),countText=Bounds(v.ProgressLabel.rectTransform,camera),fillRect=Bounds(v.ProgressFill.rectTransform,camera);
            Assert.That(chapterText.xMax,Is.LessThan(countText.xMin),"Chapter and completion labels must remain distinct.");
            Assert.That(fillRect.xMin,Is.GreaterThan(progressTrack.xMin));Assert.That(fillRect.xMax,Is.LessThan(progressTrack.xMax));
            Assert.That(fillRect.yMin,Is.GreaterThan(progressTrack.yMin));Assert.That(fillRect.yMax,Is.LessThan(progressTrack.yMax));
            Assert.That(chapterText.xMin,Is.GreaterThan(chapterPlaque.xMin));Assert.That(chapterText.xMax,Is.LessThan(chapterPlaque.xMax));
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Assert.That(button.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null,button.name+" must retain press feedback after content rebuilds.");
                if(v.InfoPanel.activeSelf&&!button.transform.IsChildOf(v.InfoPanel.transform))continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                // Allow one rendered pixel for camera-space rounding at a shared header edge.
                Assert.That(rect.xMin>=safeArea.xMin-1&&rect.yMin>=safeArea.yMin-1&&rect.xMax<=safeArea.xMax+1&&rect.yMax<=safeArea.yMax+1,Is.True,button.name+" outside safe area: "+rect+" safe="+safeArea);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" hit "+hits[0].gameObject.name);

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width at font="+text.fontSize+", ppu="+text.pixelsPerUnit);
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
