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
        public Func<CampaignPlayerSave> CampaignSaveProvider;
        public Func<LevelData> CampaignLevelProvider;
        public const int CurrentRevision=5;
        public int Revision=CurrentRevision;
        public bool SourceControls;
        public ProductionPlayerHeader Header;
        public Image TitlePlaque,SourcePlay;
        public SplashTitleTypography Typography;
        public RectTransform Content,Hud,HudBacking,TitleArea,Footer,Body,GoalsArea,MovesArea,BoostersArea,RewardsArea;
        public Image Environment,FooterOrnament;
        public Sprite Ruby,Sapphire,GoalMedallion;
        public Text HeroName,LevelText,GoldText,GemsText,EnergyText,TitleText,ChapterText,PlayText;
        public Text GoalsHeading,SelectedLevel,MovesHeading,MovesValue,BoostersHeading,RewardsHeading,RewardStatus,RewardGold,RewardGems;
        public Button ProfileButton,SettingsButton,BackButton,PlayButton;
        public Button[] PlusButtons,BoosterButtons;
        public RectTransform BoosterDetailsRoot;
        public Image BoosterDetailIcon;
        public Text BoosterDetailText,BoosterDetailsBackText;
        public Button BoosterDetailsBack,BoosterCatalog;
        public Text BoosterCatalogText;
        public int InspectedBoosterIndex{get;private set;}=-1;
        public Text[] BoosterCounts,BonusCounts;
        public RectTransform[] BonusRoots;
        public PreLevelGoalView[] Goals;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&SourceControls&&Header&&Header.HasBindings&&TitlePlaque&&SourcePlay&&Typography!=null&&Content!=null&&Hud!=null&&HudBacking!=null&&TitleArea!=null&&Footer!=null&&Body!=null&&
            GoalsArea!=null&&MovesArea!=null&&BoostersArea!=null&&RewardsArea!=null&&Environment!=null&&Environment.sprite!=null&&FooterOrnament!=null&&FooterOrnament.sprite!=null&&
            Ruby!=null&&Sapphire!=null&&GoalMedallion!=null&&HeroName!=null&&LevelText!=null&&GoldText!=null&&GemsText!=null&&EnergyText!=null&&TitleText!=null&&ChapterText!=null&&PlayText!=null&&
            GoalsHeading!=null&&SelectedLevel!=null&&MovesHeading!=null&&MovesValue!=null&&BoostersHeading!=null&&RewardsHeading!=null&&RewardStatus!=null&&RewardGold!=null&&RewardGems!=null&&
            ProfileButton!=null&&SettingsButton!=null&&BackButton!=null&&PlayButton!=null&&Valid(PlusButtons,3)&&Valid(BoosterButtons,4)&&Valid(BoosterCounts,4)&&Valid(BonusCounts,4)&&Valid(BonusRoots,4)&&
            BoosterCatalog!=null&&BoosterCatalogText!=null&&BoosterDetailsRoot!=null&&BoosterDetailIcon!=null&&BoosterDetailText!=null&&BoosterDetailsBackText!=null&&BoosterDetailsBack!=null&&
            Goals!=null&&Goals.Length==3&&Array.TrueForAll(Goals,g=>g!=null&&g.Root!=null&&g.Icon!=null&&g.Description!=null&&g.Count!=null);
        static bool Valid<T>(T[] values,int length) where T:UnityEngine.Object=>values!=null&&values.Length==length&&Array.TrueForAll(values,x=>x!=null);
        GameState subscribed;
        string language;
        bool committed;
        readonly List<Button> bound=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        void OnEnable(){InspectedBoosterIndex=-1;committed=false;BindState();RefreshContent();}
        void Update()
        {
            if(CampaignSaveProvider==null&&subscribed!=GameState.Instance){BindState();RefreshState();}
            if(language!=DemoFeatureState.LanguageCode)RefreshContent();
        }
        void OnDisable(){Unbind();if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=null;}
        void BindState()
        {
            if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=CampaignSaveProvider==null?GameState.Instance:null;
            if(isActiveAndEnabled&&subscribed!=null)subscribed.Changed+=RefreshState;
        }
        public void RefreshContent()
        {
            ProductionNavigationStyle.Apply(this);
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";
            HeroName.text=en?"Ivan":"Иван";ChapterText.text=en?"CHAPTER 1":"ГЛАВА 1";TitleText.text=en?"NATIVE VILLAGE":"РОДНАЯ ДЕРЕВНЯ";PlayText.text=en?"PLAY":"ИГРАТЬ";
            GoalsHeading.text=en?"GOALS":"ЦЕЛИ";MovesHeading.text=en?"MOVES":"ХОДЫ";BoostersHeading.text=en?"IN-GAME BOOSTERS":"БУСТЕРЫ В ИГРЕ";RewardsHeading.text=en?"LEVEL REWARDS":"НАГРАДА ЗА УРОВЕНЬ";
            string source;Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            if(!SourceControls)foreach(Text text in GetComponentsInChildren<Text>(true)){text.font=font;text.raycastTarget=false;}
            if(!SourceControls)PlayText.font=Application.isPlaying?Typography.Resolve(true,out source):(Typography.StartFont!=null?Typography.StartFont:font);
            if(SourceControls)ProductionPreLevelSourceControls.RefreshLanguage(this);
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;
            var campaign=CampaignSaveProvider?.Invoke();GameState state=CampaignSaveProvider==null?GameState.Instance:null;PlayerSaveData save=campaign!=null?campaign.player:state!=null?state.Save:null;bool en=DemoFeatureState.LanguageCode=="en";
            GoldText.text=save!=null?Balance(save.gold):"—";GemsText.text=save!=null?Balance(save.gems):"—";EnergyText.text=save!=null?save.energy+"/"+save.energyMax:"— / —";
            LevelText.text=(en?"Level ":"Уровень ")+(save!=null?save.level.ToString():"—");
            LevelData level=CampaignLevelProvider!=null?CampaignLevelProvider():ChapterLevelCatalog.ResolveSelected(save);if(level==null){PlayButton.interactable=false;return;}
            if(campaign!=null&&Header!=null)Header.RefreshValues(save);
            SelectedLevel.text=(en?"LEVEL ":"УРОВЕНЬ ")+level.levelId;MovesValue.text=level.moveLimit.ToString();
            for(int i=0;i<Goals.Length;i++)
            {
                var slot=Goals[i];bool present=level.goals!=null&&i<level.goals.Length;slot.Root.gameObject.SetActive(present);if(!present)continue;
                var goal=level.goals[i];
                int count=level.goals.Length;float stride=count==3?152:195;
                slot.Root.anchoredPosition=new Vector2(285.5f-78-(count-1)*stride*.5f+i*stride,-145);
                bool tile=goal.type==LevelGoalTypes.CollectTile;
                slot.Icon.rectTransform.anchoredPosition=new Vector2(33,0);slot.Icon.rectTransform.sizeDelta=new Vector2(90,106);
                slot.Icon.sprite=tile?ProductionGameplayArt.Gem(goal.tileType):goal.type==LevelGoalTypes.Score?ProductionGameplayArt.Load("goal_score_r5"):goal.type==LevelGoalTypes.Survival?ProductionGameplayArt.Load("goal_survival_r5"):goal.type==LevelGoalTypes.BringDown?ProductionGameplayArt.Load("feather"):ProductionGameplayArt.Obstacle(goal.obstacleType==LevelObstacleTypes.Ice?LegendsOfRus.GameplayCore.BoardObstacleKind.Ice:goal.obstacleType==LevelObstacleTypes.Chain?LegendsOfRus.GameplayCore.BoardObstacleKind.Chain:LegendsOfRus.GameplayCore.BoardObstacleKind.Crate);
                slot.Description.text="";slot.Count.text=Balance(goal.target);
            }
            bool claimed=campaign!=null?campaign.player.levelProgress.Exists(p=>p.chapterId==level.chapterId&&p.levelId==level.levelId&&p.rewardClaimed):state!=null&&state.GetLevelProgress(level.chapterId,level.levelId)?.rewardClaimed==true;
            var reward=level.rewards;
            RewardStatus.text=claimed?(en?"REWARD RECEIVED":"НАГРАДА ПОЛУЧЕНА"):"";
            RewardGold.text=Balance(!claimed&&reward!=null?reward.gold:0);RewardGems.text=Balance(!claimed&&reward!=null?reward.gems:0);
            int slotIndex=0;
            for(int i=0;i<4;i++)
            {
                BoosterCounts[i].text=campaign!=null?Balance(CampaignBoosterTransactions.Count(save,ProductionPreLevelRuntime.Boosters[i])):state!=null?Balance(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i])):"—";
                int bonus=claimed?0:BoosterReward(reward,i);BonusCounts[i].text="+"+bonus;BonusRoots[i].gameObject.SetActive(bonus>0);
                if(bonus>0)BonusRoots[i].anchoredPosition=new Vector2(310+slotIndex++*83,-200);
            }
            if(SourceControls)ProductionPreLevelSourceControls.RefreshState(this,level.goals!=null?level.goals.Length:0);
            RefreshBoosterDetails(en);
            PlayButton.interactable=save!=null&&(campaign!=null||state!=null&&state.IsLevelUnlocked(level.chapterId,level.levelId));
        }
        public void InspectBooster(int index)
        {
            if(index < -1 || index >= BoosterButtons.Length)return;
            InspectedBoosterIndex=index;
            RefreshState();
        }
        void RefreshBoosterDetails(bool en)
        {
            bool inspecting=InspectedBoosterIndex>=0;
            BoosterDetailsRoot.gameObject.SetActive(inspecting);
            foreach(Button button in BoosterButtons)button.gameObject.SetActive(!inspecting);
            BoostersHeading.text=en?"IN-GAME BOOSTERS":"БУСТЕРЫ В ИГРЕ";
            if(!inspecting)return;
            int i=InspectedBoosterIndex;
            string[] names=en?new[]{"BOGATYR MACE","FIREBIRD FEATHER","LIVING LIGHT","SEA KING'S PEARL"}:
                new[]{"БОГАТЫРСКАЯ БУЛАВА","ПЕРО ЖАР-ПТИЦЫ","ЖИВОЙ СВЕТ","ЖЕМЧУЖИНА МОРСКОГО ЦАРЯ"};
            string[] effects=en?new[]{
                "Tap a cell to clear its row.\nSwipe up/down to clear its column.",
                "Choose a cell to clear\na 3×3 area around it.",
                "Choose a gem to clear\nall gems of that color.",
                "Adds 5 moves\nto the current round."}:
                new[]{"Коснитесь клетки: очистить ряд.\nПроведите вверх/вниз: столбец.",
                "Выберите клетку: очистить\nобласть 3×3 вокруг неё.",
                "Выберите самоцвет: убрать\nвсе самоцветы этого цвета.",
                "Добавляет 5 ходов\nв текущем раунде."};
            BoostersHeading.text=names[i];
            BoosterDetailIcon.sprite=BoosterButtons[i].GetComponent<Image>().sprite;
            var campaign=CampaignSaveProvider?.Invoke();int stock=campaign!=null?CampaignBoosterTransactions.Count(campaign.player,ProductionPreLevelRuntime.Boosters[i]):GameState.Instance!=null?GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]):0;
            BoosterDetailText.text=(en?"In-game • In stock: ":"В игре • В запасе: ")+Balance(stock)+"\n"+effects[i];
            BoosterDetailsBackText.text=en?"BACK":"НАЗАД";
            BoosterCatalogText.text=en?"ALL BOOSTERS":"ВСЕ БУСТЕРЫ";
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
        public void BindNavigation(Action<ScreenId> show,Func<bool> start,Action back,Action openBoosters=null)
        {
            Unbind();committed=false;
            Bind(ProfileButton,()=>show(ScreenId.Profile),true);Bind(SettingsButton,()=>show(ScreenId.Settings),true);Bind(BackButton,back,true);
            // An energy gate leaves this page active. Only commit after a successful route.
            Bind(PlayButton,()=>committed=start(),false);
            for(int i=0;i<3;i++){int resource=i;Bind(PlusButtons[i],()=>show(resource==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            // These inventory tools activate on the board; preparation only explains them.
            for(int i=0;i<BoosterButtons.Length;i++){int index=i;Bind(BoosterButtons[i],()=>InspectBooster(index),false);}
            Bind(BoosterDetailsBack,()=>InspectBooster(-1),false);
            if(openBoosters!=null)Bind(BoosterCatalog,openBoosters,false);
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
