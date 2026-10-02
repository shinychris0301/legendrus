using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionDailyRewardView:MonoBehaviour
    {
        public const int CurrentRevision=5;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body,Hud;
        public ProductionPlayerHeader PlayerHeader;
        public Image Environment,Header,MonthFill;
        public Text Title,Intro,HeroName,LevelText,GoldText,GemsText,EnergyText,MonthTitle,MonthCount,MonthNote,Tomorrow,Status,ClaimText,BackText,ConfirmText;
        public Text[] DayLabels,Amounts,States;
        public Image[] Cards,RewardIcons;
        public Button ClaimButton,ConfirmButton,BackButton,ProfileButton,SettingsButton;
        public Button[] PlusButtons;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&PlayerHeader!=null&&PlayerHeader.HasBindings&&Typography&&Content&&Body&&Environment&&Header&&MonthFill&&Title&&Intro&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&MonthTitle&&MonthCount&&MonthNote&&Tomorrow&&Status&&ClaimText&&BackText&&ConfirmText&&ConfirmButton&&ClaimButton&&BackButton&&ProfileButton&&SettingsButton&&Valid(DayLabels,7)&&Valid(Amounts,7)&&Valid(States,7)&&Valid(Cards,7)&&Valid(RewardIcons,7)&&Valid(PlusButtons,3);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        Func<bool> claim;Action back;Action<ScreenId> navigate;GameState observed;string language,date;bool navigating,claiming;string feedback;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshContent;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshContent();}if(language!=DemoFeatureState.LanguageCode||date!=DateTime.UtcNow.ToString("yyyyMMdd",CultureInfo.InvariantCulture))RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshContent;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshContent;}
        public void Bind(Func<bool> claimAction,Action backAction,Action<ScreenId> navigation)
        {
            Unbind();claim=claimAction;back=backAction;navigate=navigation;navigating=false;ActionsBound=true;
            ClaimButton.onClick.AddListener(Claim);BackButton.onClick.AddListener(Back);ConfirmButton.onClick.AddListener(Back);ProfileButton.onClick.AddListener(Profile);SettingsButton.onClick.AddListener(Settings);
            PlusButtons[0].onClick.AddListener(Shop);PlusButtons[1].onClick.AddListener(Shop);PlusButtons[2].onClick.AddListener(Energy);RefreshContent();
        }
        void Unbind(){if(!HasRequiredBindings)return;ClaimButton.onClick.RemoveListener(Claim);BackButton.onClick.RemoveListener(Back);ConfirmButton.onClick.RemoveListener(Back);ProfileButton.onClick.RemoveListener(Profile);SettingsButton.onClick.RemoveListener(Settings);PlusButtons[0].onClick.RemoveListener(Shop);PlusButtons[1].onClick.RemoveListener(Shop);PlusButtons[2].onClick.RemoveListener(Energy);ActionsBound=false;claim=null;back=null;navigate=null;}
        bool InputAllowed()=>ActionsBound&&isActiveAndEnabled&&!navigating&&(ScreenRouter.Instance==null||(!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning));
        void Go(ScreenId target){if(!InputAllowed())return;navigating=true;navigate?.Invoke(target);}
        void Back(){if(!InputAllowed())return;navigating=true;back?.Invoke();}
        void Profile()=>Go(ScreenId.Profile);void Settings()=>Go(ScreenId.Settings);void Shop()=>Go(ScreenId.Shop);void Energy()=>Go(ScreenId.OutOfEnergy);
        void Claim()
        {
            var state=GameState.Instance;
            if(!InputAllowed()||claiming||state==null||!ClaimButton.IsInteractable()||!DailyRewardSchedule.CanClaim(state.Save,DateTime.UtcNow))return;
            claiming=true;
            try{feedback=claim!=null&&claim()?"claimed":"failed";}finally{claiming=false;RefreshContent();}
        }
        public static string RewardText(int day,bool en)
        {
            switch(day){case 1:return "500";case 2:return "20";case 3:return en?"Up to +1":"До +1";case 4:case 5:return "1";case 6:return "2";default:return en?"1,000 coins · 40 gems\nFirebird ×1":"1 000 монет · 40 самоцветов\nЖар-птица ×1";}
        }
        public static string RewardSummary(int day,bool en)
        {
            switch(day)
            {
                case 1:return en?"500 coins":"500 монет";
                case 2:return en?"20 gems":"20 самоцветов";
                case 3:return en?"Up to +1 energy":"До +1 энергии";
                case 4:return en?"Living Light ×1":"Живой свет ×1";
                case 5:return en?"Sea Pearl ×1":"Жемчужина морского царя ×1";
                case 6:return en?"Bogatyr Mace ×2":"Богатырская булава ×2";
                default:return en?"1,000 coins · 40 gems · Firebird ×1":"1 000 монет · 40 самоцветов · Жар-птица ×1";
            }
        }
        public void RefreshContent()
        {
            ProductionNavigationStyle.Apply(this);
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";DateTime now=DateTime.UtcNow;date=now.ToString("yyyyMMdd",CultureInfo.InvariantCulture);
            string source;var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true))t.font=font;
            var state=GameState.Instance;var s=state!=null?state.Save:null;int day=DailyRewardSchedule.Day(s,now);bool available=DailyRewardSchedule.CanClaim(s,now);
            Title.text=en?"DAILY REWARD":"ЕЖЕДНЕВНАЯ НАГРАДА";Intro.text=en?"Return each day and collect your rewards!":"Заходите каждый день и получайте награды!";
            HeroName.text=en?"Ivan":"Иван";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());
            GoldText.text=s==null?"—":s.gold.ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");GemsText.text=s==null?"—":s.gems.ToString();EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;
            for(int i=0;i<7;i++)
            {
                bool done=i+1<day||i+1==day&&!available;bool current=i+1==day;
                DayLabels[i].text=(en?"DAY ":"ДЕНЬ ")+(i+1);Amounts[i].text=RewardText(i+1,en);
                States[i].text=done?(en?"CLAIMED":"ПОЛУЧЕНО"):current?(en?"AVAILABLE":"ДОСТУПНО"):(en?"UPCOMING":"ВПЕРЕДИ");
                States[i].color=done?new Color32(149,192,127,255):current?new Color32(255,225,121,255):new Color32(189,180,154,255);
                Cards[i].color=current?Color.white:new Color(.83f,.83f,.83f,1);
            }
            int count=DailyRewardSchedule.MonthClaims(s,now);MonthTitle.text=en?"MONTHLY PROGRESS":"ПРОГРЕСС МЕСЯЦА";MonthCount.text=count+"\n"+(en?"Days":"Дней");
            MonthFill.fillAmount=Mathf.Clamp01(count/28f);MonthNote.text=en?"Milestone bonuses are not available yet":"Бонусы за этапы пока недоступны";
            int next=day%7+1;Tomorrow.text=available?(en?"Collect today's reward to advance the cycle.":"Получите сегодняшнюю награду, чтобы продолжить цикл."):(en?"NEXT: DAY ":"ДАЛЕЕ: ДЕНЬ ")+next+"\n"+RewardSummary(next,en);
            Status.text=feedback=="failed"?(en?"Reward not saved. Please try again after resolving storage.":"Не удалось сохранить награду. Проверьте сохранение."):(en?"Resets at 00:00 UTC. Missing a day restarts the cycle.\nEnergy is limited by your current capacity.":"Новый день — в 00:00 UTC. Пропуск возвращает к дню 1.\nЭнергия начисляется в пределах текущего лимита.");
            ClaimText.text=available?(en?"CLAIM":"ПОЛУЧИТЬ"):(en?"CLAIMED":"ПОЛУЧЕНО");BackText.text=en?"BACK":"НАЗАД";ConfirmText.text=en?"GREAT":"ОТЛИЧНО";
            ProductionDailyRewardRuntime.ApplySourceState(this,en,day,available);PlayerHeader.ApplyPresentation();
            ClaimButton.interactable=ActionsBound&&state!=null&&state.CanPersist&&available&&!claiming;
        }
    }
}
