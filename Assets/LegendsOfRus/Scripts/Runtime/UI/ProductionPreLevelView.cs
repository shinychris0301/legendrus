using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [Serializable] public sealed class PreLevelGoalView
    {
        public RectTransform Root;
        public Image Icon;
        public Text Description,Count;
    }

    [DisallowMultipleComponent]
    public sealed class ProductionPreLevelView : MonoBehaviour
    {
        public const int CurrentRevision=1;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Hud,HudBacking,TitleArea,Footer,Body,GoalsArea,MovesArea,BoostersArea,RewardsArea;
        public Image Environment,FooterOrnament;
        public Sprite Ruby,Sapphire,GoalMedallion;
        public Text HeroName,LevelText,GoldText,GemsText,EnergyText,TitleText,ChapterText,PlayText;
        public Text GoalsHeading,SelectedLevel,MovesHeading,MovesValue,BoostersHeading,RewardsHeading,RewardStatus,RewardGold,RewardGems;
        public Button ProfileButton,SettingsButton,BackButton,PlayButton;
        public Button[] PlusButtons,BoosterButtons;
        public Text[] BoosterCounts,BonusCounts;
        public RectTransform[] BonusRoots;
        public PreLevelGoalView[] Goals;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography!=null&&Content!=null&&Hud!=null&&HudBacking!=null&&TitleArea!=null&&Footer!=null&&Body!=null&&
            GoalsArea!=null&&MovesArea!=null&&BoostersArea!=null&&RewardsArea!=null&&Environment!=null&&Environment.sprite!=null&&FooterOrnament!=null&&FooterOrnament.sprite!=null&&
            Ruby!=null&&Sapphire!=null&&GoalMedallion!=null&&HeroName!=null&&LevelText!=null&&GoldText!=null&&GemsText!=null&&EnergyText!=null&&TitleText!=null&&ChapterText!=null&&PlayText!=null&&
            GoalsHeading!=null&&SelectedLevel!=null&&MovesHeading!=null&&MovesValue!=null&&BoostersHeading!=null&&RewardsHeading!=null&&RewardStatus!=null&&RewardGold!=null&&RewardGems!=null&&
            ProfileButton!=null&&SettingsButton!=null&&BackButton!=null&&PlayButton!=null&&Valid(PlusButtons,3)&&Valid(BoosterButtons,4)&&Valid(BoosterCounts,4)&&Valid(BonusCounts,4)&&Valid(BonusRoots,4)&&
            Goals!=null&&Goals.Length==2&&Array.TrueForAll(Goals,g=>g!=null&&g.Root!=null&&g.Icon!=null&&g.Description!=null&&g.Count!=null);
        static bool Valid<T>(T[] values,int length) where T:UnityEngine.Object=>values!=null&&values.Length==length&&Array.TrueForAll(values,x=>x!=null);
        GameState subscribed;
        string language;
        bool committed;
        readonly List<Button> bound=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        void OnEnable(){committed=false;BindState();RefreshContent();}
        void Update()
        {
            if(subscribed!=GameState.Instance){BindState();RefreshState();}
            if(language!=DemoFeatureState.LanguageCode)RefreshContent();
        }
        void OnDisable(){Unbind();if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=null;}
        void BindState()
        {
            if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=GameState.Instance;
            if(isActiveAndEnabled&&subscribed!=null)subscribed.Changed+=RefreshState;
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";
            HeroName.text=en?"Ivan":"Иван";ChapterText.text=en?"CHAPTER 1":"ГЛАВА 1";TitleText.text=en?"NATIVE VILLAGE":"РОДНАЯ ДЕРЕВНЯ";PlayText.text=en?"PLAY":"ИГРАТЬ";
            GoalsHeading.text=en?"GOALS":"ЦЕЛИ";MovesHeading.text=en?"MOVES":"ХОДЫ";BoostersHeading.text=en?"CHOOSE BOOSTERS":"ВЫБЕРИТЕ БУСТЕРЫ";RewardsHeading.text=en?"LEVEL REWARDS":"НАГРАДА ЗА УРОВЕНЬ";
            string source;Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(Text text in GetComponentsInChildren<Text>(true)){text.font=font;text.raycastTarget=false;}
            PlayText.font=Application.isPlaying?Typography.Resolve(true,out source):(Typography.StartFont!=null?Typography.StartFont:font);
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;
            GameState state=GameState.Instance;PlayerSaveData save=state!=null?state.Save:null;bool en=DemoFeatureState.LanguageCode=="en";
            GoldText.text=save!=null?Balance(save.gold):"—";GemsText.text=save!=null?Balance(save.gems):"—";EnergyText.text=save!=null?save.energy+"/"+save.energyMax:"— / —";
            LevelText.text=(en?"Level ":"Уровень ")+(save!=null?save.level.ToString():"—");
            LevelData level=ChapterLevelCatalog.ResolveSelected(save);
            SelectedLevel.text=(en?"LEVEL ":"УРОВЕНЬ ")+level.levelId;MovesValue.text=level.moveLimit.ToString();
            for(int i=0;i<Goals.Length;i++)
            {
                var slot=Goals[i];bool present=level.goals!=null&&i<level.goals.Length;slot.Root.gameObject.SetActive(present);if(!present)continue;
                var goal=level.goals[i];bool ruby=goal.type==LevelGoalTypes.CollectTile&&goal.tileType==0;bool sapphire=goal.type==LevelGoalTypes.CollectTile&&goal.tileType==1;
                slot.Icon.rectTransform.anchoredPosition=ruby||sapphire?new Vector2(23,0):new Vector2(8,14);
                slot.Icon.rectTransform.sizeDelta=Vector2.one*(ruby||sapphire?110:140);
                slot.Icon.sprite=ruby?Ruby:sapphire?Sapphire:GoalMedallion;slot.Description.text=ruby||sapphire?"":GoalDescription(goal,en);slot.Count.text=Balance(goal.target);
            }
            bool claimed=state!=null&&state.GetLevelProgress(level.chapterId,level.levelId)?.rewardClaimed==true;
            var reward=level.rewards;
            RewardStatus.text=claimed?(en?"REWARD RECEIVED":"НАГРАДА ПОЛУЧЕНА"):"";
            RewardGold.text=Balance(!claimed&&reward!=null?reward.gold:0);RewardGems.text=Balance(!claimed&&reward!=null?reward.gems:0);
            int slotIndex=0;
            for(int i=0;i<4;i++)
            {
                BoosterCounts[i].text=state!=null?Balance(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i])):"—";
                int bonus=claimed?0:BoosterReward(reward,i);BonusCounts[i].text="+"+bonus;BonusRoots[i].gameObject.SetActive(bonus>0);
                if(bonus>0)BonusRoots[i].anchoredPosition=new Vector2(310+slotIndex++*83,-200);
            }
            PlayButton.interactable=save!=null&&state.IsLevelUnlocked(level.chapterId,level.levelId);
        }
        public static int BoosterReward(LevelRewardData reward,int index)
        {
            if(reward==null)return 0;
            switch(index){case 0:return reward.boosterMace;case 1:return reward.boosterFirebird;case 2:return reward.boosterLivingLight;case 3:return reward.boosterSeaPearl;default:return 0;}
        }
        public static string GoalDescription(LevelGoalData goal,bool en)
        {
            switch(goal.type)
            {
                case LevelGoalTypes.Score:return en?"SCORE":"СЧЁТ";
                case LevelGoalTypes.Survival:return en?"SURVIVE\nMOVES":"ВЫЖИТЬ\nХОДОВ";
                case LevelGoalTypes.BringDown:return en?"LOWER\nFEATHERS":"ОПУСТИТЬ\nПЕРЬЯ";
                case LevelGoalTypes.ClearObstacle:
                    switch(goal.obstacleType){case LevelObstacleTypes.Ice:return en?"CLEAR\nICE":"УБРАТЬ\nЛЁД";case LevelObstacleTypes.Chain:return en?"BREAK\nCHAINS":"СНЯТЬ\nЦЕПИ";case LevelObstacleTypes.Crate:return en?"BREAK\nCRATES":"РАЗБИТЬ\nЯЩИКИ";}
                    return en?"CLEAR":"УБРАТЬ";
                default:return en?"COLLECT":"СОБРАТЬ";
            }
        }
        static string Balance(int value)=>Mathf.Max(0,value).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
        public void BindNavigation(Action<ScreenId> show,Func<bool> start,Action back,Action openBoosters)
        {
            Unbind();committed=false;
            Bind(ProfileButton,()=>show(ScreenId.Profile),true);Bind(SettingsButton,()=>show(ScreenId.Settings),true);Bind(BackButton,back,true);
            // An energy gate leaves this page active. Only commit after a successful route.
            Bind(PlayButton,()=>committed=start(),false);
            for(int i=0;i<3;i++){int resource=i;Bind(PlusButtons[i],()=>show(resource==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            // Preserve the existing booster modal. Opening it neither selects nor consumes inventory.
            foreach(Button button in BoosterButtons)Bind(button,openBoosters,false);
            ActionsBound=true;
        }
        void Bind(Button button,Action action,bool route)
        {
            UnityAction listener=()=>
            {
                if(committed||!isActiveAndEnabled||!button.IsInteractable()||ScreenRouter.Instance!=null&&ScreenRouter.Instance.HasActiveModalBlocker)return;
                if(route)committed=true;button.GetComponent<ProductionHomeButtonFeedback>()?.NotifyActivated();action();
            };
            bound.Add(button);listeners.Add(listener);button.onClick.AddListener(listener);
        }
        void Unbind(){for(int i=0;i<bound.Count;i++)if(bound[i]!=null)bound[i].onClick.RemoveListener(listeners[i]);bound.Clear();listeners.Clear();ActionsBound=false;}
    }
}
