using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionChapterCompleteView:MonoBehaviour
    {
        public const int CurrentRevision=8;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body;
        public Image Environment,Header,Banner,Foreground,Teaser,RewardsHeaderArt,RewardsDivider,SceneRailLeft,SceneRailRight;
        public Text HeaderText,BannerText,VictoryText,Summary,RewardsHeading,RewardNote,NextHeading,TeaserText,TeaserTitle,TeaserDescription;
        public Image[] RewardIcons;
        public Text[] RewardCounts,ActionLabels;
        public Button[] Actions;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography!=null&&Content!=null&&Body!=null&&Environment!=null&&Header!=null&&Banner!=null&&Foreground!=null&&Teaser!=null&&HeaderText!=null&&BannerText!=null&&VictoryText!=null&&Summary!=null&&RewardsHeading!=null&&RewardNote!=null&&NextHeading!=null&&TeaserText!=null&&TeaserTitle!=null&&TeaserDescription!=null&&RewardsHeaderArt!=null&&RewardsDivider!=null&&SceneRailLeft!=null&&SceneRailRight!=null&&Valid(RewardIcons,6)&&Valid(RewardCounts,6)&&Valid(Actions,2)&&Valid(ActionLabels,2);
        static bool Valid<T>(T[] a,int count)where T:UnityEngine.Object=>a!=null&&a.Length==count&&Array.TrueForAll(a,x=>x!=null);
        readonly UnityAction[] listeners=new UnityAction[2];
        GameState observed;string language;bool committed;
        void OnEnable(){observed=GameState.Instance;if(observed!=null)observed.Changed+=RefreshContent;RefreshContent();}
        void Update(){if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshContent;observed=null;Unbind();}
        void Unbind(){if(Actions!=null)for(int i=0;i<Math.Min(2,Actions.Length);i++)if(Actions[i]!=null&&listeners[i]!=null){Actions[i].onClick.RemoveListener(listeners[i]);listeners[i]=null;}ActionsBound=false;}
        public void Bind(Func<bool> navigate)
        {
            Unbind();committed=false;
            for(int i=0;i<2;i++){int index=i;listeners[i]=()=>{
                var router=ScreenRouter.Instance;
                if(committed||!isActiveAndEnabled||!Actions[index].IsInteractable()||router!=null&&(router.HasActiveModalBlocker||router.IsScreenTransitioning))return;
                committed=true;if(navigate==null||!navigate())committed=false;
            };Actions[i].onClick.AddListener(listeners[i]);}
            ActionsBound=true;RefreshContent();
        }
        // These are base first-clear rewards already granted by CompleteLevel, never a new chapter grant.
        public static int[] ReceivedRewards(GameState state)
        {
            var values=new int[6];if(state==null)return values;
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
            {
                var progress=state.GetLevelProgress(1,level.levelId);if(progress==null||!progress.rewardClaimed)continue;
                var r=level.rewards;if(r==null)continue;
                values[0]+=r.gold;values[1]+=r.gems;values[2]+=r.boosterMace;values[3]+=r.boosterFirebird;values[4]+=r.boosterLivingLight;values[5]+=r.boosterSeaPearl;
            }
            return values;
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            string source;var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true))t.font=font;
            var s=GameState.Instance!=null?GameState.Instance.GetChapterCompletionSummary(1):null;
            bool completed=s!=null&&(s.Phase==ChapterCompletionPhase.PresentationPending||s.Phase==ChapterCompletionPhase.Acknowledged);
            string variant=en?"blank":"ru";
            Header.sprite=ProductionChapterCompleteRuntime.Load("header_"+(en||!completed?"blank":"ru"));
            Banner.sprite=ProductionChapterCompleteRuntime.Load("banner_"+(en?"blank_r8":"ru"));Foreground.sprite=ProductionChapterCompleteRuntime.Load("foreground_"+(en?"blank_r8":"ru"));Teaser.sprite=ProductionChapterCompleteRuntime.Load("teaser_"+(en?"blank_r8":"ru"));
            Foreground.gameObject.SetActive(completed);VictoryText.enabled=en&&completed;HeaderText.enabled=en||!completed;BannerText.enabled=en;TeaserText.enabled=en;TeaserTitle.enabled=en;TeaserDescription.enabled=en;
            HeaderText.text=completed?(en?"CHAPTER COMPLETE":"ГЛАВА ЗАВЕРШЕНА"):(en?"CHAPTER IN PROGRESS":"ГЛАВА НЕ ЗАВЕРШЕНА");
            BannerText.text="CHAPTER 1\nHOME VILLAGE";VictoryText.text="VICTORY!";
            Summary.text=s==null?(en?"No chapter progress":"Нет прогресса главы"):$"{s.CompletedLevels}/{s.TotalLevels} "+(en?"LEVELS":"УРОВНЕЙ")+$"  •  {s.EarnedStars}/{s.MaximumStars} "+(en?"STARS":"ЗВЁЗД");
            RewardsHeaderArt.sprite=ProductionChapterCompleteRuntime.Load("rewards_heading_"+variant);
            RewardsHeading.enabled=en;RewardsHeading.text=en?"REWARDS":"НАГРАДЫ";
            Summary.text=(en?"Rewards received · ":"Награды получены · ")+Summary.text;
            RewardNote.enabled=false;
            RewardNote.text=en?"Already received · no additional chapter bonus":"Уже получены · дополнительного бонуса нет";
            NextHeading.text=en?"NEXT CHAPTER · COMING SOON":"СКОРО";
            var originalNext=Body.Find("NextHeadingOriginal");if(originalNext!=null)originalNext.gameObject.SetActive(!en);
            NextHeading.rectTransform.anchoredPosition=new Vector2(en?100:704,en?-1397:-1398);
            NextHeading.rectTransform.sizeDelta=new Vector2(en?741:135,29);NextHeading.fontSize=en?26:20;
            TeaserText.text="CHAPTER 2";TeaserTitle.text="MYSTERY OF\nTHE FOREST";
            TeaserDescription.text="Dark forces awaken in the forest.\nExplore its depths to uncover the truth.";
            var values=ReceivedRewards(GameState.Instance);for(int i=0;i<6;i++)RewardCounts[i].text=values[i].ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ");
            for(int i=0;i<2;i++){Actions[i].GetComponent<Image>().sprite=ProductionChapterCompleteRuntime.Load((i==0?"next_":"map_")+(en?"blank_r8":"ru"));ActionLabels[i].enabled=en;ActionLabels[i].text=i==0?"CONTINUE":"WORLD MAP";Actions[i].interactable=ActionsBound&&(i==1||completed);}
        }
    }
}
