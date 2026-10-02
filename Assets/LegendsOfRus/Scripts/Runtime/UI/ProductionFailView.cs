using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionFailView:MonoBehaviour
    {
        public Func<CampaignPlayerSave> CampaignSaveProvider;
        public const int CurrentRevision=14;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public ProductionPlayerHeader Header;
        public RectTransform Content,Body,Hud,HudBacking;
        public Image Environment,Hero;
        public Text Title,Subtitle,GoalsHeading,Score,RetryText,ExitText,Reaction,BoostersHeading,HeroName,Level;
        public Text[] Resources,BoosterCounts,BoosterLabels;
        public Button Retry,Exit,Profile,Settings;
        public Button PaidOffer,AdOffer;
        public Text PaidOfferTitle,AdOfferTitle,PaidOfferStatus,AdOfferStatus,OfferSeparator;
        public Button[] Plus,BoosterButtons;
        public PreLevelGoalView[] Goals;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>PaidOffer!=null&&AdOffer!=null&&PaidOfferTitle!=null&&AdOfferTitle!=null&&PaidOfferStatus!=null&&AdOfferStatus!=null&&OfferSeparator!=null&&Revision==CurrentRevision&&Typography!=null&&Content!=null&&Body!=null&&Hud!=null&&HudBacking!=null&&Environment!=null&&Environment.sprite!=null&&Hero!=null&&Hero.sprite!=null&&Title!=null&&Subtitle!=null&&GoalsHeading!=null&&Score!=null&&RetryText!=null&&ExitText!=null&&Reaction!=null&&BoostersHeading!=null&&HeroName!=null&&Level!=null&&Retry!=null&&Exit!=null&&Profile!=null&&Settings!=null&&Valid(Resources,3)&&Valid(Plus,3)&&Valid(BoosterCounts,4)&&Valid(BoosterLabels,4)&&Valid(BoosterButtons,4)&&Goals!=null&&Goals.Length==3&&Array.TrueForAll(Goals,g=>g!=null&&g.Root!=null&&g.Icon!=null&&g.Count!=null);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        GameState subscribed;
        string language;
        bool committed;
        int inspected=-1;
        readonly List<Button> buttons=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        void OnEnable(){committed=false;inspected=-1;BindState();RefreshContent();}
        void OnDisable(){Unbind();if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=null;}
        void Update(){if(CampaignSaveProvider==null&&subscribed!=GameState.Instance){BindState();RefreshContent();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void BindState(){if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=CampaignSaveProvider==null?GameState.Instance:null;if(subscribed!=null)subscribed.Changed+=RefreshState;}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(Text t in GetComponentsInChildren<Text>(true)){t.font=font;t.raycastTarget=false;}
            HeroName.text=en?"Ivan":"Иван";Title.text=en?"DEFEAT":"ПОРАЖЕНИЕ";
            RetryText.text=en?"TRY AGAIN":"ПОПРОБОВАТЬ СНОВА";ExitText.text=en?"CHAPTER MAP":"К КАРТЕ ГЛАВЫ";
            PaidOfferTitle.text=en?"+5 MOVES":"+5 ХОДОВ";
            AdOfferTitle.text=en?"CONTINUE\nWATCH AN AD":"ПРОДОЛЖИТЬ\nЧЕРЕЗ РЕКЛАМУ";
            PaidOfferStatus.text=AdOfferStatus.text=en?"UNAVAILABLE":"НЕДОСТУПНО";
            OfferSeparator.text=en?"OR":"ИЛИ";
            BoostersHeading.text=en?"RECOMMENDED BOOSTERS":"РЕКОМЕНДУЕМЫЕ БУСТЕРЫ";
            string[] names=en?new[]{"THUNDER\nHAMMER","PHOENIX","DRUZHINA\nSHIELD","POWER POTION"}:new[]{"МОЛОТ ГРОМА","ФЕНИКС","ЩИТ ДРУЖИНЫ","ЗЕЛЬЕ СИЛЫ"};
            for(int i=0;i<4;i++)BoosterLabels[i].text=names[i];
            if(Header!=null)Header.ApplyPresentation();
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;
            var campaign=CampaignSaveProvider?.Invoke();var state=CampaignSaveProvider==null?GameState.Instance:null;var save=campaign!=null?campaign.player:state!=null?state.Save:null;bool en=DemoFeatureState.LanguageCode=="en";
            Resources[0].text=save!=null?Number(save.gold):"—";Resources[1].text=save!=null?Number(save.gems):"—";Resources[2].text=save!=null?save.energy+"/"+save.energyMax:"—";
            if(Header!=null){if(campaign!=null)Header.RefreshValues(save);else Header.RefreshValues();}
            Level.text=(en?"Level ":"Уровень ")+(save!=null?save.level.ToString():"—");
            var result=campaign!=null&&campaign.run.state==CampaignRunState.Failed?new LevelFailureResult(JsonUtility.FromJson<LevelData>(campaign.run.levelJson),campaign.run.score,campaign.run.goalProgress):state!=null?state.LastFailureResult:null;
            Subtitle.text=result!=null?(en?"OUT OF MOVES":"ХОДЫ ЗАКОНЧИЛИСЬ"):(en?"NO FAILED ROUND":"НЕТ ЗАВЕРШЁННОЙ ПОПЫТКИ");
            Score.text=result!=null?(en?"LEVEL ":"УРОВЕНЬ ")+result.Level.levelId+"  •  "+(en?"SCORE ":"СЧЁТ ")+Number(result.Score):"";
            GoalsHeading.text=result!=null&&result.HasGoalProgress?(en?"UNFINISHED GOALS":"НЕ ВЫПОЛНЕНЫ ЦЕЛИ"):(en?"GOAL PROGRESS UNAVAILABLE":"ПРОГРЕСС ЦЕЛЕЙ НЕДОСТУПЕН");
            var pending=new List<int>();
            if(result!=null&&result.HasGoalProgress)
                for(int i=0;i<result.Level.goals.Length;i++)if(result.CurrentAt(i)<result.Level.goals[i].target)pending.Add(i);
            if(result!=null&&result.HasGoalProgress&&pending.Count==0)GoalsHeading.text=en?"GOALS COMPLETE":"ЦЕЛИ ВЫПОЛНЕНЫ";
            for(int i=0;i<3;i++)
            {
                Goals[i].Root.gameObject.SetActive(i<pending.Count);if(i>=pending.Count)continue;
                int index=pending[i];var data=result.Level.goals[index];
                var slot=new LevelGoalRuntime(new[]{data},0).GetSlot(0);
                Sprite icon=slot.Type==LevelGoalTypes.CollectTile?ProductionGameplayArt.Gem(slot.TileType):slot.Type==LevelGoalTypes.ClearObstacle?ProductionGameplayArt.Obstacle(slot.ObstacleKind):slot.Type==LevelGoalTypes.BringDown?ProductionGameplayArt.Load("feather"):ProductionGameplayArt.Load(slot.Type==LevelGoalTypes.Score?"goal_score_r5":"goal_survival_r5");
                Goals[i].Root.anchoredPosition=new Vector2(470.5f-pending.Count*202*.5f+i*202+6,-700);
                Goals[i].Icon.sprite=icon;ProductionFailRuntime.Fit(Goals[i].Icon,95,50,88,88);
                Goals[i].Count.text=Number(result.CurrentAt(index))+"/"+Number(data.target);
            }
            ProductionFailRuntime.ApplyLocalizedArtwork(this,DemoFeatureState.LanguageCode=="ru",result!=null);
            Retry.interactable=result!=null;
            for(int i=0;i<4;i++)BoosterCounts[i].text=campaign!=null?Number(CampaignBoosterTransactions.Count(save,ProductionPreLevelRuntime.Boosters[i])):state!=null?Number(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i])):"—";
            var story=result!=null&&campaign==null?StoryService.GetLocalizedReaction(result.Level,StoryReactionOutcomes.Failure,DemoFeatureState.LanguageCode):null;
            Reaction.text=story!=null?story.Speaker+": "+story.Text:"";
            Reaction.gameObject.SetActive(inspected>=0);
            BoostersHeading.gameObject.SetActive(inspected<0);
            if(inspected>=0)
            {
                string[] info=en?new[]{"Thunder hammer: clears a row; swipe vertically for a column.","Phoenix: clears a 3×3 area.","Druzhina shield: clears all gems of the selected color.","Power potion: adds 5 moves during gameplay."}:new[]{"Молот грома: очистить ряд; вертикальный жест — столбец.","Феникс: очистить область 3×3.","Щит дружины: убрать все самоцветы выбранного цвета.","Зелье силы: добавить 5 ходов во время игры."};
                Reaction.text=info[inspected];
            }
        }
        public void BindNavigation(Action retry,Action exit,Action<ScreenId> show)
        {
            Unbind();committed=false;Bind(Retry,retry,true);Bind(Exit,exit,true);Bind(Profile,()=>show(ScreenId.Profile),true);Bind(Settings,()=>show(ScreenId.Settings),true);
            for(int i=0;i<3;i++){int n=i;Bind(Plus[i],()=>show(n==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            for(int i=0;i<4;i++){int n=i;Bind(BoosterButtons[i],()=>{inspected=inspected==n?-1:n;RefreshState();},false);}
            ActionsBound=true;
        }
        void Bind(Button b,Action action,bool route)
        {
            UnityAction listener=()=>{if(committed||!isActiveAndEnabled||!b.IsInteractable()||ScreenRouter.Instance!=null&&ScreenRouter.Instance.HasActiveModalBlocker)return;if(route)committed=true;action();};
            buttons.Add(b);listeners.Add(listener);b.onClick.AddListener(listener);
        }
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i]!=null)buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        static string Number(int value)=>Mathf.Max(0,value).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ");
    }
}
