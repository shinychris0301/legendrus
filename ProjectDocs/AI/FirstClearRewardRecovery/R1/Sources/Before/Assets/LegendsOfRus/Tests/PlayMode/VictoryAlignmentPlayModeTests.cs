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
    public sealed class VictoryAlignmentPlayModeTests
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
            router.Show(ScreenId.Victory); Settle();
            yield return null; yield return null;
            yield return WaitForNormalButtonColors();
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

        [UnityTest] public IEnumerator SmallPortraitContinuePaddingUsesNativeStoryRouteWithoutRewardDuplication()
        {
            Win(1,int.MaxValue);yield return new WaitForSecondsRealtime(1);var v=View();int gold=GameState.Instance.Save.gold,gems=GameState.Instance.Save.gems;GameObject tap=null;
            foreach(var lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);v.RefreshContent();yield return null;yield return null;
                Assert.That(v.Score.text,Is.EqualTo("2 147 483 647"));
                Capture("Victory_Touch_360_"+lang,new Vector2Int(360,640),new Rect(0,20,360,600),camera=>tap=CoreOutcomeTouchAssertions.Check(v.ContinueButton,camera,new Rect(0,20,360,600),"Victory","360"));
                Capture("Victory_Touch_1080_"+lang,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>CoreOutcomeTouchAssertions.Check(v.ContinueButton,camera,new Rect(45,100,990,2160),"Victory","1080"));
            }
            CoreOutcomeTouchAssertions.Click(tap);Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Dialogue).Or.EqualTo(ScreenId.ChapterMap));var destination=router.CurrentScreen;v.ContinueButton.onClick.Invoke();Assert.That(router.CurrentScreen,Is.EqualTo(destination));Assert.That(GameState.Instance.Save.gold,Is.EqualTo(gold));Assert.That(GameState.Instance.Save.gems,Is.EqualTo(gems));
        }

        [UnityTest] public IEnumerator OutcomeReactionSwitchesLanguageWithoutNewRewards()
        {
            var state=GameState.Instance;
            state.Save.highestUnlockedLevel=51;
            for(int n=1;n<51;n++) state.Save.levelProgress.Add(new LevelProgressData { chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=1 });
            Win(51,3500); Win(51,3500); // Replay exposes the existing caption instead of the first-clear ad explanation.
            yield return new WaitForSecondsRealtime(1);
            var v=View(); string before=OutcomeStateWithoutClock(); int objects=Root().GetComponentsInChildren<Transform>(true).Length;
            foreach(string lang in new[]{"en","ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);yield return null;yield return null;
                var expected=StoryCatalog.LocalizeReaction(StoryCatalog.FindReaction(51,"victory"),StoryCatalog.LoadDialogueLanguage(lang));
                Assert.That(v.Reaction.text,Is.EqualTo(expected.Speaker+": "+expected.Text));
                Assert.That(v.Reaction.gameObject.activeInHierarchy,Is.True);
                Assert.That(v.Reaction.preferredHeight,Is.LessThanOrEqualTo(v.Reaction.rectTransform.rect.height+1));
                Assert.That(Root().GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(objects));
                Assert.That(OutcomeStateWithoutClock(),Is.EqualTo(before));
                Assert.That(Root().transform.Find("OutcomeReaction"),Is.Null);
            }
            Capture("Outcome51_EN_941",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Outcome51_EN_576",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            DemoFeatureState.SetLanguage("ru");yield return null;yield return null;
            Capture("Outcome51_RU_941",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Assert.That(OutcomeStateWithoutClock(),Is.EqualTo(before));
        }
        [UnityTest] public IEnumerator CaptionsRemainReadableAcrossLanguagesAndPhoneSizes()
        {
            var state=GameState.Instance;
            state.Save.highestUnlockedLevel=51;
            for(int n=1;n<51;n++) state.Save.levelProgress.Add(new LevelProgressData { chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=1 });
            Win(51,3500);Win(51,3500);
            yield return new WaitForSecondsRealtime(1);
            var v=View();string before=OutcomeStateWithoutClock();
            foreach(string lang in new[]{"en","ru"})
            {
                DemoFeatureState.SetLanguage(lang);yield return null;yield return null;
                foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                {
                    var reaction=StoryService.GetLocalizedReaction(level,StoryReactionOutcomes.Victory,lang);
                    if(reaction==null)continue;
                    v.Reaction.text=reaction.Speaker+": "+reaction.Text;
                    Canvas.ForceUpdateCanvases();
                    Assert.That(v.Reaction.preferredHeight,Is.LessThanOrEqualTo(v.Reaction.rectTransform.rect.height),lang+" round "+level.levelId);
                }
                v.RefreshState();
                Capture("Caption_"+lang+"_360",new Vector2Int(360,640),new Rect(0,20,360,600),camera=>
                {
                    float pixels=Bounds(v.Reaction.rectTransform,camera).height/v.Reaction.rectTransform.rect.height*v.Reaction.fontSize;
                    Assert.That(pixels,Is.GreaterThanOrEqualTo(11),"Caption must retain readable type at the smallest verified viewport.");
                });
                Capture("Caption_"+lang+"_576",new Vector2Int(576,1280),new Rect(0,48,576,1172));
                Capture("Caption_"+lang+"_941",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                v.SetStatus("Видео с наградой недоступно");
                Capture("Caption_Status_"+lang+"_360",new Vector2Int(360,640),new Rect(0,20,360,600));
                Assert.That(v.Reaction.gameObject.activeSelf,Is.False);
            }
            Assert.That(OutcomeStateWithoutClock(),Is.EqualTo(before));
        }
        static string OutcomeStateWithoutClock()
        {
            var save=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            save.lastEnergyUtcTicks=0;return JsonUtility.ToJson(save);
        }
        [UnityTest] public IEnumerator LegacyOutcomeOverlayUpdatesExistingTextOnLanguageChange()
        {
            // Explicit fallback composition fixture; does not install an overlay on the native page.
            var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var field=typeof(ScreenRouter).GetField("activeOutcomeReaction",flags);var prior=field.GetValue(router);
            var parent=new GameObject("LegacyOutcomeFixture",typeof(RectTransform));parent.transform.SetParent(router.SafeArea,false);
            try
            {
                field.SetValue(router,StoryService.GetLocalizedReaction(ChapterLevelCatalog.Find(1,51),"victory"));
                typeof(ScreenRouter).GetMethod("BuildOutcomeReactionOverlay",flags).Invoke(router,new object[]{(RectTransform)parent.transform,ScreenId.Victory});
                int count=parent.GetComponentsInChildren<Transform>(true).Length;
                string before=OutcomeStateWithoutClock();
                foreach(string lang in new[]{"en","ru"})
                {
                    DemoFeatureState.SetLanguage(lang);yield return null;yield return null;
                    var expected=StoryCatalog.LocalizeReaction(StoryCatalog.FindReaction(51,"victory"),StoryCatalog.LoadDialogueLanguage(lang));
                    Assert.That(parent.transform.Find("OutcomeReaction/ReactionBody").GetComponent<Text>().text,Is.EqualTo(expected.Text));
                    Assert.That(parent.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                    Assert.That(OutcomeStateWithoutClock(),Is.EqualTo(before));
                }
            }
            finally { field.SetValue(router,prior);UnityEngine.Object.Destroy(parent); }
            yield return null;
        }

        ProductionVictoryView View()=>Root().GetComponentInChildren<ProductionVictoryView>();
        void Win(int level,int score){GameState.Instance.CompleteLevel(ChapterLevelCatalog.Find(1,level),score);router.Show(ScreenId.Victory);Settle();}
        [UnityTest]
        public IEnumerator AllAuthoredRewardsAndCurrentRoundScoreUseTheCompletionResult()
        {
            var state=GameState.Instance;
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
            {
                int score=level.starScoreThresholds[2]+17;
                Win(level.levelId,score);yield return null;
                var view=View();string save=JsonUtility.ToJson(state.Save);view.RefreshContent();
                Assert.That(view.HasRequiredBindings&&view.ActionsBound,Is.True);
                Assert.That(view.Score.text.Replace(" ",""),Does.EndWith(score.ToString()));
                Assert.That(view.RewardValues[0].text,Is.EqualTo(level.rewards.gold.ToString()));
                Assert.That(view.RewardValues[1].text,Is.EqualTo(level.rewards.gems.ToString()));
                int cards=2;
                for(int i=0;i<4;i++)
                {
                    int count=ProductionPreLevelView.BoosterReward(level.rewards,i);if(count==0)continue;
                    Assert.That(view.RewardIcons[cards].sprite,Is.SameAs(ProductionGameplayArt.Booster(i)));
                    Assert.That(view.RewardValues[cards++].text,Is.EqualTo(count.ToString()));
                }
                Assert.That(Array.FindAll(view.RewardCards,r=>r.gameObject.activeSelf).Length,Is.EqualTo(cards));
                foreach(var star in view.Stars)Assert.That(star.Dimmed,Is.False);
                Assert.That(view.DoubleButton.interactable,Is.True);
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(save),"Rendering cannot grant rewards.");
                if(level.levelId==2||level.levelId==10)
                {yield return new WaitForSecondsRealtime(1);Capture("Victory_Level"+level.levelId+"_Rewards_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
            }
            DemoFeatureState.SetLanguage("en");View().RefreshContent();
            // The installed chapter now ends at 200, not 10. Wait for that page's reveal before capture.
            float deadline=Time.realtimeSinceStartup+3f;
            while(Array.Exists(View().StarGlows,g=>g.color.a<.63f)&&Time.realtimeSinceStartup<deadline)yield return null;
            Capture("Victory_LastAuthoredRound_Rewards_576_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
        }
        [UnityTest]
        public IEnumerator ReplayShowsLowerCurrentScoreAndNoDuplicateFirstClearRewards()
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,2);
            Win(2,level.starScoreThresholds[2]+100);int gold=state.Save.gold,boosters=state.Save.boosterMace;
            Win(2,level.starScoreThresholds[0]);yield return new WaitForSecondsRealtime(1);
            Assert.That(state.LastCompletionResult.Score,Is.EqualTo(level.starScoreThresholds[0]));
            Assert.That(View().Score.text.Replace(" ",""),Does.EndWith(level.starScoreThresholds[0].ToString()));
            Assert.That(View().Stars[0].Dimmed,Is.False);Assert.That(View().Stars[1].Dimmed,Is.True);Assert.That(View().Stars[2].Dimmed,Is.True);
            Assert.That(View().RewardValues[0].text,Is.EqualTo("0"));Assert.That(View().RewardValues[1].text,Is.EqualTo("0"));
            Assert.That(View().RewardCards[2].gameObject.activeSelf,Is.False);Assert.That(View().DoubleButton.interactable,Is.False);
            View().DoubleButton.onClick.Invoke();Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.boosterMace,Is.EqualTo(boosters));
            Capture("Victory_Replay_OneStar_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            state.ResetProgress();View().RefreshContent();Assert.That(state.LastCompletionResult,Is.Null);Assert.That(View().DoubleButton.interactable,Is.False);
            Capture("Victory_NoResult_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [UnityTest]
        public IEnumerator VerifiedRewardDomainOperationUpdatesGoldOnlyAndOnlyOnce()
        {
            var state=GameState.Instance;Win(3,4000);yield return null;
            int gold=state.Save.gold,gems=state.Save.gems;
            // Domain contract only: the UI remains routed through the unchanged verified provider gate.
            Assert.That(state.TryDoubleLastLevelGoldReward(),Is.True);
            Assert.That(state.Save.gold,Is.EqualTo(gold+140));Assert.That(state.Save.gems,Is.EqualTo(gems));
            Assert.That(View().RewardValues[0].text,Is.EqualTo("280"));Assert.That(View().RewardValues[1].text,Is.EqualTo("5"));
            Assert.That(View().DoubleButton.interactable,Is.False);Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);
            yield return new WaitForSecondsRealtime(1);
            Capture("Victory_GoldDoubled_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [TestCase(int.MaxValue,1)]
        [TestCase(int.MaxValue-139,140)]
        [TestCase(1,int.MaxValue)]
        public void DoubleGoldOverflowPreservesEligibilityBalancesAndJournal(int gold,int reward)
        {
            var state=GameState.Instance;Win(3,4000);
            state.Save.gold=gold;state.Save.lastRewardGold=reward;state.Persist();
            string before=JsonUtility.ToJson(state.Save);var originalSave=state.Save;
            var priorWrite=state.LastWriteResult;var outcome=state.LastCompletionResult;
            var present=new bool[keys.Length];var journal=new string[keys.Length];
            for(int i=0;i<keys.Length;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
            int events=0;Action changed=()=>events++;state.Changed+=changed;
            try
            {
                Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);
                Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);
                Assert.That(state.Save,Is.SameAs(originalSave));
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(state.LastCompletionResult,Is.SameAs(outcome));
                Assert.That(state.LastWriteResult,Is.EqualTo(priorWrite));
                Assert.That(state.CanPersist,Is.True);Assert.That(events,Is.Zero);
                for(int i=0;i<keys.Length;i++)
                {
                    Assert.That(PlayerPrefs.HasKey(keys[i]),Is.EqualTo(present[i]));
                    Assert.That(PlayerPrefs.GetString(keys[i]),Is.EqualTo(journal[i]));
                }
            }
            finally{state.Changed-=changed;}
        }
        [UnityTest]
        public IEnumerator FailedDoubleGoldSaveDoesNotPublishOrConsumeReward()
        {
            var state=GameState.Instance;Win(3,4000);yield return null;
            string before=JsonUtility.ToJson(state.Save);var originalSave=state.Save;
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;
            foreach(string key in keys)PlayerPrefs.DeleteKey(key);
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
            Assert.That(SaveService.LoadWithResult().Status,Is.EqualTo(PlayerSaveLoadStatus.UnsupportedFutureVersion));
            var outcome=state.LastCompletionResult;int events=0;Action changed=()=>events++;state.Changed+=changed;
            try
            {
                // Preserve the behavioral assertion when reproducing the old Persist error log.
                bool ignoredLogs=LogAssert.ignoreFailingMessages;bool doubled;
                try{LogAssert.ignoreFailingMessages=true;doubled=state.TryDoubleLastLevelGoldReward();}
                finally{LogAssert.ignoreFailingMessages=ignoredLogs;}
                Assert.That(doubled,Is.False);
                Assert.That(state.Save,Is.SameAs(originalSave));
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(state.LastCompletionResult,Is.SameAs(outcome));
                Assert.That(state.LastWriteResult.Status,Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                Assert.That(state.CanPersist,Is.False);var rejectedWrite=state.LastWriteResult;
                Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);
                Assert.That(state.LastWriteResult,Is.EqualTo(rejectedWrite));
                Assert.That(events,Is.Zero);Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(keys[1]),Is.False);Assert.That(PlayerPrefs.HasKey(keys[2]),Is.False);
                Assert.That(View().RewardValues[0].text,Is.EqualTo("140"));
            }
            finally{state.Changed-=changed;}
        }
        [UnityTest]
        public IEnumerator OverflowDoubleGoldCanRetryAtExactLimitAndPersistsOnce()
        {
            foreach(string language in new[]{"ru","en"})
            {
                GameState.Instance.ResetProgress();GameState.Instance.Save.highestUnlockedLevel=3;
                DemoFeatureState.SetLanguage(language);
                Win(3,4000);yield return null;
                var state=GameState.Instance;int reward=state.Save.lastRewardGold;
                Assert.That(reward,Is.EqualTo(140));state.Save.gold=int.MaxValue-reward+1;state.Persist();
                string before=JsonUtility.ToJson(state.Save);
                Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(View().DoubleButton.interactable,Is.True);
                Assert.That(View().RewardValues[0].text,Is.EqualTo("140"));
                Assert.That(state.SpendGold(1),Is.True);
                var expected=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
                expected.gold=int.MaxValue;expected.lastRewardDoubled=true;
                var outcome=state.LastCompletionResult;int events=0;Action changed=()=>events++;state.Changed+=changed;
                try
                {
                    // Service call only: this does not bypass or certify the video-provider gate.
                    Assert.That(state.TryDoubleLastLevelGoldReward(),Is.True);
                    Assert.That(events,Is.EqualTo(1));Assert.That(state.CanPersist,Is.True);
                    Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(JsonUtility.ToJson(expected)));
                    Assert.That(state.LastCompletionResult,Is.SameAs(outcome));
                    Assert.That(View().RewardValues[0].text,Is.EqualTo("280"));
                    Assert.That(View().DoubleButton.interactable,Is.False);
                    Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False);Assert.That(events,Is.EqualTo(1));
                }
                finally{state.Changed-=changed;}
                var loaded=SaveService.Load();Assert.That(loaded.gold,Is.EqualTo(int.MaxValue));
                Assert.That(loaded.lastRewardDoubled,Is.True);
                router.Show(ScreenId.Home);Settle();router.Show(ScreenId.Victory);Settle();yield return null;
                Assert.That(View().RewardValues[0].text,Is.EqualTo("280"));Assert.That(View().DoubleButton.interactable,Is.False);
                yield return new WaitForSecondsRealtime(1);
                yield return WaitForNormalButtonColors();
                Capture("Victory_OverflowDoubleRecovered_"+language,new Vector2Int(941,1672),new Rect(0,0,941,1672));
                state.Save=loaded;
                Assert.That(state.TryDoubleLastLevelGoldReward(),Is.False,"Reloaded flags must reject duplicate credit.");
                Assert.That(state.Save.gold,Is.EqualTo(int.MaxValue));
            }
        }
        [UnityTest]
        public IEnumerator UnavailableVideoPreservesRewardsAndContinueRoutesThroughExistingStoryFlow()
        {
            var state=GameState.Instance;Win(1,3000);yield return null;
            int gold=state.Save.gold,gems=state.Save.gems;
            View().DoubleButton.onClick.Invoke();yield return null;
            Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.gems,Is.EqualTo(gems));
            Assert.That(state.Save.lastRewardDoubled,Is.False);Assert.That(View().Status.text,Does.Contain("Видео недоступно"));Assert.That(View().Reaction.gameObject.activeSelf,Is.False);
            yield return new WaitForSecondsRealtime(1);
            Capture("Victory_VideoUnavailable_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            var button=View().ContinueButton;button.onClick.Invoke();Settle();ScreenId destination=router.CurrentScreen;
            Assert.That(destination,Is.EqualTo(ScreenId.Dialogue).Or.EqualTo(ScreenId.ChapterMap));
            button.onClick.Invoke();Assert.That(router.CurrentScreen,Is.EqualTo(destination));
            Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.energy,Is.EqualTo(state.Save.energyMax));
            router.Show(ScreenId.Victory);Settle();yield return null;
            Assert.That(View().ActionsBound,Is.True);
            Assert.That(View().Hud.gameObject.activeInHierarchy,Is.False,"Selected original omits the wallet header.");
        }
        [UnityTest]
        public IEnumerator RealBoardWinCallbackOpensNativeVictoryWithTheFinalScore()
        {
            var state=GameState.Instance;state.SelectLevel(1,1);router.Show(ScreenId.Gameplay);Settle();yield return null;
            var board=router.Viewport.GetComponentInChildren<Match3Board>();Assert.That(board,Is.Not.Null);
            var field=typeof(Match3Board).GetField("onWin",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            var callback=(Action<int>)field.GetValue(board);Assert.That(callback,Is.Not.Null);
            // Exercise the router's real callback; this is not a simulated gem-swap win.
            callback(2468);Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Victory));Assert.That(state.LastCompletionResult.Score,Is.EqualTo(2468));
            Assert.That(View().Score.text,Does.Contain("2 468"));Assert.That(View().ActionsBound,Is.True);
            Assert.That(View().Reaction.text,Is.Not.Empty);
        }

        [UnityTest]
        public IEnumerator ReferenceAndPhonePortraitsRetainNativeInputLiveTextAndSafeAreas()
        {
            var level=ChapterLevelCatalog.Find(1,1);Win(1,level.starScoreThresholds[1]);yield return new WaitForSecondsRealtime(1);
            Assert.That(View().Stars[1].Dimmed,Is.False);Assert.That(View().Stars[2].Dimmed,Is.True);
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            Capture("Victory_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Victory_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Victory_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;
            Capture("Victory_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }

        [UnityTest]
        public IEnumerator LocalizedTitleKeepsRenderedGlyphsStableAcrossRefreshes()
        {
            Win(1,3000);yield return null;
            var view=View();var lettering=view.Title.GetComponent<ProductionVictoryTitleLettering>();
            Assert.That(lettering,Is.Not.Null);Assert.That(lettering.enabled,Is.True);
            int objects=Root().GetComponentsInChildren<Transform>(true).Length;
            Vector3[] firstRussian=null;
            foreach(string language in new[]{"ru","en","ru"})
            {
                DemoFeatureState.SetLanguage(language);view.RefreshContent();yield return null;
                Canvas.ForceUpdateCanvases();
                Assert.That(view.Title.text,Is.EqualTo(language=="en"?"VICTORY!":"ПОБЕДА!"));
                VerifyCurvedTitleGeometry(view);
                Vector3[] before=(view.Title.enabled?view.Title.canvasRenderer.GetMesh().vertices:Array.Empty<Vector3>());
                for(int refresh=0;refresh<3;refresh++)
                {
                    view.RefreshContent();lettering.Refresh();Canvas.ForceUpdateCanvases();
                    AssertSameVertices(before,(view.Title.enabled?view.Title.canvasRenderer.GetMesh().vertices:Array.Empty<Vector3>()),"Refreshing "+language+" must not accumulate a curve or depth layers.");
                    Assert.That(Root().GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(objects));
                    Assert.That(view.Title.GetComponents<ProductionVictoryTitleLettering>().Length,Is.EqualTo(1));
                }
                if(language=="ru")
                {
                    if(firstRussian==null)firstRussian=before;
                    else AssertSameVertices(firstRussian,before,"Switching back to Russian must restore the same glyph geometry.");
                }
            }
        }

        [UnityTest]
        public IEnumerator HeaderRefillTimeReadsExistingEnergyClockWithoutChangingTheSave()
        {
            var state=GameState.Instance;var view=View();
            state.Save.energy=state.Save.energyMax-1;
            state.Save.lastEnergyUtcTicks=DateTime.UtcNow.AddMinutes(-10).Ticks;
            long ticks=state.Save.lastEnergyUtcTicks;int energy=state.Save.energy,gold=state.Save.gold;
            view.RefreshState();
            Assert.That(view.ResourceValues[2].text,Is.EqualTo(energy+"/"+state.Save.energyMax));
            int first=TimerSeconds(view.EnergyTimer.text);
            Assert.That(first,Is.EqualTo(1200).Within(1));
            // UTC may cross a second after the frame timestamp used to schedule refresh.
            // Observe the next visible tick with a bounded wait instead of racing that phase.
            float deadline=Time.realtimeSinceStartup+2.5f;
            while(TimerSeconds(view.EnergyTimer.text)==first&&Time.realtimeSinceStartup<deadline)yield return null;
            int updated=TimerSeconds(view.EnergyTimer.text);
            Assert.That(updated,Is.LessThan(first),"Visible countdown must refresh without a wallet event.");
            Assert.That(updated,Is.EqualTo(Math.Ceiling(state.EnergyRemaining().TotalSeconds)).Within(1));
            Assert.That(state.Save.lastEnergyUtcTicks,Is.EqualTo(ticks));
            Assert.That(state.Save.energy,Is.EqualTo(energy));Assert.That(state.Save.gold,Is.EqualTo(gold));
            Capture("Victory_EnergyCountdown_576_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            state.Save.energy=state.Save.energyMax;view.RefreshState();Assert.That(view.EnergyTimer.text,Is.EqualTo("ПОЛНАЯ"));
            DemoFeatureState.SetLanguage("en");view.RefreshContent();Assert.That(view.EnergyTimer.text,Is.EqualTo("FULL"));
        }
        [UnityTest] public IEnumerator LiveBilingualRewardCaptionsRetainAmountsAndUnavailableSemantics()
        {
            GameState.Instance.Save.highestUnlockedLevel=500;Win(500,int.MaxValue);yield return new WaitForSecondsRealtime(1);var view=View();
            foreach(var state in new[]{"FirstClear","NoBoosters","Replay","Unavailable","NoResult"})
            {
                if(state=="NoBoosters"){Win(1,3500);view=View();}
                if(state=="Replay"){Win(500,3500);view=View();}
                if(state=="Unavailable")view.SetStatus("Видео с наградой недоступно");
                if(state=="NoResult"){GameState.Instance.ResetProgress();view.RefreshContent();}
                yield return null;string before=OutcomeStateWithoutClock();
                foreach(var language in new[]{"ru","en"})
                {
                    DemoFeatureState.SetLanguage(language);view.RefreshContent();yield return null;yield return null;
                    foreach(var size in new[]{new Vector2Int(360,640),new Vector2Int(576,1280),new Vector2Int(941,1672),new Vector2Int(1080,2400)})
                    {
                        Rect safe=size.x==360?new Rect(0,20,360,600):size.x==576?new Rect(0,48,576,1172):size.x==941?new Rect(0,0,941,1672):new Rect(45,100,990,2160);
                        Capture("LiveCaptions_"+state+"_"+language+"_"+size.x,size,safe,camera=>
                        {
                            var rows=new List<CoreCaptionAssertions.Row>();
                            foreach(var text in new[]{view.RewardsHeading,view.RewardsArea.Find("ScoreHeading").GetComponent<Text>(),view.RewardsArea.Find("BoosterHeading").GetComponent<Text>()})rows.Add(CoreCaptionAssertions.Check(text,camera,11.9f));
                            Assert.That(view.RewardsPanel.sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("panel")));
                            Assert.That(view.RewardsHeading.text,Is.EqualTo(language=="ru"?"НАГРАДЫ":"REWARDS"));
                            for(int i=0;i<view.RewardValues.Length;i++)if(view.RewardCards[i].gameObject.activeSelf)rows.Add(CoreCaptionAssertions.Check(view.RewardValues[i],camera,i<2?13.9f:12.5f));
                            var empty=view.RewardsArea.Find("BoosterEmpty").GetComponent<Text>();if(empty.gameObject.activeInHierarchy)rows.Add(CoreCaptionAssertions.Check(empty,camera,11.2f));
                            if(state=="FirstClear")Assert.That(view.Score.text,Is.EqualTo("2 147 483 647"));
                            if(state=="FirstClear")Assert.That(view.RewardCards[2].gameObject.activeSelf,Is.True,"Current round500 booster quantity must be covered.");
                            if(state=="NoBoosters")Assert.That(empty.gameObject.activeInHierarchy,Is.True);
                            if(state=="Replay"||state=="Unavailable"||state=="NoResult")Assert.That(view.DoubleButton.interactable,Is.False);
                            CoreCaptionAssertions.Save("Victory_"+state+"_"+language+"_"+size.x,rows);
                        });
                    }
                    Assert.That(OutcomeStateWithoutClock(),Is.EqualTo(before));
                }
            }
        }
        static int TimerSeconds(string text)
        {
            string[] parts=text.Split(':');Assert.That(parts.Length,Is.EqualTo(2));
            return int.Parse(parts[0])*60+int.Parse(parts[1]);
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_Victory" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active Victory."); return null;
        }

        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            Canvas.ForceUpdateCanvases();
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
            string directory = Environment.GetEnvironmentVariable("LOR_VICTORY_CAPTURE_DIR");
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
            var cameraGo = new GameObject("VictoryCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionVictoryView>(),camera,size,safeArea);
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

        static void VerifyProtectedFrame(Image image,bool card)
        {
            Mesh mesh=image.canvasRenderer.GetMesh();
            Assert.That(mesh,Is.Not.Null);
            Assert.That(mesh.vertexCount,Is.EqualTo(card?24:36),"Frame needs independent wood spans and protected ornaments.");
            var p=mesh.vertices;
            float scaleX=(p[1].x-p[0].x)/(card?240:290);
            float scaleY=(p[6].y-p[0].y)/(card?350:225);
            Assert.That(scaleX,Is.EqualTo(scaleY).Within(.0001f),"Corner carving must scale uniformly.");
            float crestScale=(p[3].x-p[2].x)/(card?260:600);
            Assert.That(crestScale,Is.EqualTo(scaleY).Within(.0001f),"Center crest must not expand horizontally.");
        }

        static Rect Bounds(RectTransform rect,Camera camera)
        {
            var corners=new Vector3[4];rect.GetWorldCorners(corners);
            Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        void VerifyLayoutAndInput(ProductionVictoryView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);
            Rect environment=Bounds(view.Environment.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(Button button in view.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds(button.transform as RectTransform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                var results=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},results);
                Assert.That(results.Count,Is.GreaterThan(0),button.name);
                Assert.That(results[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" obscured");
            }
            foreach(Text text in view.GetComponentsInChildren<Text>())
            {
                if(!text.enabled||string.IsNullOrEmpty(text.text))continue;
                Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThan(0),text.name);
                if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)
                    Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.name+" horizontal overflow");
                Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.name+" vertical overflow");
            }
            Assert.That(view.Hud.gameObject.activeInHierarchy,Is.False);
            bool sourceAd=DemoFeatureState.LanguageCode!="en"&&view.DoubleButton.interactable;
            Assert.That(view.DoubleText.enabled,Is.EqualTo(!sourceAd));
            Assert.That(view.DoubleButton.GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load(sourceAd?"video_ru":"video")));
            if(sourceAd&&string.IsNullOrEmpty(view.Status.text))Assert.That(view.RewardStatus.text,Does.Contain("только монеты"));
            var birdGlow=view.Body.Find("FirebirdGlow").GetComponent<Image>();
            Assert.That(birdGlow.raycastTarget,Is.False);
            Assert.That(birdGlow.transform.GetSiblingIndex(),Is.LessThan(view.Firebird.transform.GetSiblingIndex()));
            Rect panel=Bounds(view.RewardsPanel.rectTransform,camera);
            for(int i=0;i<view.RewardIcons.Length;i++)
            {
                if(!view.RewardCards[i].gameObject.activeSelf)continue;
                var image=view.RewardIcons[i];Rect icon=Bounds(image.rectTransform,camera),card=Bounds(view.RewardCards[i],camera);
                Assert.That(card.xMin,Is.GreaterThan(panel.xMin));Assert.That(card.xMax,Is.LessThan(panel.xMax));
                Assert.That(card.yMin,Is.GreaterThan(panel.yMin));Assert.That(card.yMax,Is.LessThan(panel.yMax));
                Assert.That(card.Contains(icon.min)&&card.Contains(icon.max),Is.True,"Full reward icon must fit its own card.");
                Assert.That(icon.width/icon.height,Is.EqualTo(image.sprite.rect.width/image.sprite.rect.height).Within(.001f));
                var value=Bounds(view.RewardValues[i].rectTransform,camera);
                Assert.That(card.Contains(value.min)&&card.Contains(value.max),Is.True,"Reward amount must clear frame edges.");
                Assert.That(icon.Overlaps(value),Is.False,"Reward art must not obscure the actual amount.");
                if(i>=2)
                {
                    Assert.That(icon.center.x,Is.EqualTo(card.center.x).Within(.1f));
                    Assert.That(value.yMin-card.yMin,Is.GreaterThan(card.height*.075f),"Booster count must clear the lower carving.");
                }
            }
            foreach(Text text in new[]{view.Score,view.RewardsHeading})
            {
                Rect bounds=Bounds(text.rectTransform,camera);
                Assert.That(panel.Contains(bounds.min)&&panel.Contains(bounds.max),Is.True,text.name+" must stay inside the reward board.");
            }
            Rect ornament=Bounds(view.FooterOrnament.rectTransform,camera);
            Assert.That(ornament.yMin,Is.EqualTo(safe.yMin).Within(2),"Footer stays at safe-area bottom.");
            VerifyCurvedTitleGeometry(view);VerifyStarVisibility(view);
            foreach(var star in view.Stars)
            {
                Assert.That(star.ActiveStarArt,Is.SameAs(ProductionVictoryReferenceStyle.Load(star.Dimmed?"star_empty":"star")));
                Assert.That(star.mainTexture,Is.SameAs(star.ActiveStarArt.texture),"Replay and earned-star state must refresh the rendered texture.");
                Assert.That(star.canvasRenderer.GetMesh().vertexCount,Is.EqualTo(4));
            }
            Rect primary=Bounds(view.ContinueButton.transform as RectTransform,camera);
            var primaryImage=view.ContinueButton.GetComponent<Image>();
            Assert.That(primaryImage.preserveAspect,Is.True);
            Assert.That(primary.width/primary.height,Is.EqualTo(primaryImage.sprite.rect.width/primaryImage.sprite.rect.height).Within(.01f));
            var label=Bounds(view.ContinueText.rectTransform,camera);
            Assert.That(primary.Contains(label.min)&&primary.Contains(label.max),Is.True,"Continue lettering must fit the source button.");
            Assert.That(primary.Overlaps(panel),Is.False,"Actions must clear the reward frame.");
            Assert.That(primary.Overlaps(Bounds(view.DoubleButton.transform as RectTransform,camera)),Is.False);
            foreach(Text message in new[]{view.Reaction,view.Status,view.RewardStatus})
            {
                if(!message.gameObject.activeSelf||string.IsNullOrEmpty(message.text))continue;
                Rect bounds=Bounds(message.rectTransform,camera),inset=Bounds(view.MessageInset.rectTransform,camera);
                Assert.That(view.MessageInset.enabled,Is.True);
                Assert.That(inset.Contains(bounds.min)&&inset.Contains(bounds.max),Is.True,"Message must remain within its writing area.");
                foreach(var card in view.RewardCards)
                    if(card.gameObject.activeSelf)Assert.That(bounds.Overlaps(Bounds(card,camera)),Is.False,"Status must not cover granted rewards.");
            }
            Vector2 old=view.Body.anchoredPosition;view.GetComponent<ProductionVictoryLayout>().ApplyNow();Canvas.ForceUpdateCanvases();
            Assert.That(Vector2.Distance(old,view.Body.anchoredPosition),Is.LessThan(.01f));
        }

        static void AssertSameVertices(Vector3[] expected,Vector3[] actual,string message)
        {
            Assert.That(actual.Length,Is.EqualTo(expected.Length),message);
            for(int i=0;i<expected.Length;i++)
                Assert.That(Vector3.Distance(expected[i],actual[i]),Is.LessThan(.01f),message+" Vertex "+i);
        }

        static void VerifyStarVisibility(ProductionVictoryView view)
        {
            for(int i=0;i<view.Stars.Length;i++)
            {
                var star=view.Stars[i];var halo=view.StarGlows[i];
                Assert.That(star.gameObject.activeInHierarchy,Is.True,"Both earned and empty star slots stay visible.");
                Assert.That(Array.Exists(star.canvasRenderer.GetMesh().colors32,c=>c.a>0),Is.True);
                Assert.That(halo.GetComponent<ProductionVictoryStarHalo>().enabled,Is.True);
                Assert.That(halo.gameObject.activeSelf,Is.EqualTo(!star.Dimmed),"Only earned stars may show a halo.");
                if(star.Dimmed)continue;
                Assert.That(halo.color.a,Is.GreaterThan(0),"An earned star retains a resting glow after its reveal.");
                Mesh mesh=halo.canvasRenderer.GetMesh();
                Assert.That(mesh,Is.Not.Null);Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                Color32[] colors=mesh.colors32;
                Assert.That(Array.Exists(colors,c=>c.a>0),Is.True,"Earned halo needs visible mesh alpha.");
                Assert.That(Array.Exists(colors,c=>c.a==0),Is.True,"Halo edges must fade to transparent.");
                foreach(Color32 color in colors)
                    Assert.That(color.a/255f,Is.LessThanOrEqualTo(halo.color.a+1f/255f),"Halo mesh must honor the view's reveal opacity.");
                Rect rect=halo.GetPixelAdjustedRect();
                foreach(Vector3 vertex in mesh.vertices)
                {
                    Assert.That(vertex.x,Is.InRange(rect.xMin-.01f,rect.xMax+.01f),"Halo rays stay within their allocated rect.");
                    Assert.That(vertex.y,Is.InRange(rect.yMin-.01f,rect.yMax+.01f));
                }
            }
        }

        static void VerifyCurvedTitleGeometry(ProductionVictoryView view)
        {
            if(DemoFeatureState.LanguageCode!="en")
            {
                Assert.That(view.Title.enabled,Is.False,"Source Russian title must not have duplicate live lettering.");
                Assert.That(view.ContinueText.enabled,Is.False);
                Assert.That(view.Body.Find("VictoryBanner").GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("title_ru")));
                Assert.That(view.ContinueButton.GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("continue_ru")));
                return;
            }
            Assert.That(view.Title.enabled,Is.True,"English must use the live localized title.");

            Text title=view.Title;
            Assert.That(title.GetComponent<ProductionVictoryTitleLettering>().isActiveAndEnabled,Is.True);
            Assert.That(title.GetComponent<ProductionHomeGoldText>().enabled,Is.False);
            Assert.That(title.GetComponent<Shadow>().enabled,Is.False);
            Mesh mesh=title.canvasRenderer.GetMesh();
            Assert.That(mesh,Is.Not.Null);
            Vector3[] positions=mesh.vertices;
            // Measured from the independently generated banner_r4 blank face:
            // normalized x .17-.83, top-down y .16-.50. Check rendered glyph
            // geometry including depth, because the label RectTransform alone
            // does not describe rotated lettering or its raised middle.
            var banner=view.Body.Find("VictoryBanner") as RectTransform;
            Rect bannerRect=banner.rect;
            Rect titleSafe=Rect.MinMaxRect(bannerRect.xMin+bannerRect.width*.12f,
                bannerRect.yMax-bannerRect.height*.86f,bannerRect.xMin+bannerRect.width*.88f,
                bannerRect.yMax-bannerRect.height*.36f);
            foreach(Vector3 vertex in positions)
            {
                Vector3 local=banner.InverseTransformPoint(title.rectTransform.TransformPoint(vertex));
                Assert.That(local.x,Is.InRange(titleSafe.xMin-.75f,titleSafe.xMax+.75f),title.text+" rendered glyph outside ribbon face horizontally.");
                Assert.That(local.y,Is.InRange(titleSafe.yMin-.75f,titleSafe.yMax+.75f),title.text+" rendered glyph outside ribbon face vertically.");
            }
            var uv=new List<Vector4>();var atlas=new List<Vector4>();mesh.GetUVs(0,uv);mesh.GetUVs(1,atlas);
            Assert.That(uv.Count,Is.EqualTo(positions.Length));Assert.That(atlas.Count,Is.EqualTo(positions.Length));
            // Read the live beveled faces, identified by their nonzero shader step.
            // Each character has its own atlas rectangle in these two title strings.
            var glyphs=new List<List<int>>();var rectangles=new List<Vector4>();
            for(int i=0;i<positions.Length;i++)
            {
                if(Mathf.Abs(uv[i].z)+Mathf.Abs(uv[i].w)<.000000001f)continue;
                int glyph=rectangles.IndexOf(atlas[i]);
                if(glyph<0){glyph=rectangles.Count;rectangles.Add(atlas[i]);glyphs.Add(new List<int>());}
                glyphs[glyph].Add(i);
            }
            Assert.That(glyphs.Count,Is.EqualTo(title.text.Length),"Every localized character must have its own visible curved face.");
            Assert.That(positions.Length,Is.GreaterThan(title.text.Length*4),"Title depth must be present in the rendered mesh.");
            Assert.That(positions.Length,Is.LessThan(title.text.Length*128),"A title refresh must not repeatedly expand its effect mesh.");
            var first=glyphs[0];var last=glyphs[glyphs.Count-1];
            Vector3 left=positions[first[1]]-positions[first[0]],right=positions[last[1]]-positions[last[0]];
            Assert.That(left.x,Is.GreaterThan(0));Assert.That(right.x,Is.GreaterThan(0));
            Assert.That(Mathf.Abs(left.y/left.x),Is.LessThan(.01f),"The new title uses a straight baseline.");
            Assert.That(Mathf.Abs(right.y/right.x),Is.LessThan(.01f));
        }

        static float GlyphBaseline(Vector3[] positions,List<int> glyph)
        {
            // Face bands retain the live glyph's lower-left/lower-right edge last.
            return (positions[glyph[glyph.Count-1]].y+positions[glyph[glyph.Count-2]].y)*.5f;
        }
    }
}
