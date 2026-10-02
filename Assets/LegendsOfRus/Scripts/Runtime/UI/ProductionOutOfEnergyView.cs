using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionOutOfEnergyView:MonoBehaviour
    {
        public const int CurrentRevision=3;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body;
        public Image Environment,Shade,Panel,Emblem;
        public Image HeadingLettering,CountBacking;
        public Image[] ActionLettering;
        public Text Title,Message,Count,TimerHeading,Countdown;
        public Text[] ActionTitles,ActionDetails,ActionLabels;
        public Button Close;
        public Button[] Actions;
        public bool ActionsBound {get;private set;}
        // The existing runtime port cannot commit commercial fulfillment and has no
        // approved price catalog. Friend requests are local demo records, not delivery.
        // Binding a port is not sufficient evidence to enable any of these offers.
        public bool CanUseOffer(int index)=>false;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography&&Content&&Body&&Environment&&Environment.sprite&&Shade&&Panel&&Panel.sprite&&Emblem&&Emblem.sprite&&HeadingLettering&&HeadingLettering.sprite&&CountBacking&&CountBacking.sprite&&Valid(ActionLettering)&&Title&&Message&&Count&&TimerHeading&&Countdown&&Close&&Close.image.sprite&&Valid(Actions)&&Valid(ActionTitles)&&Valid(ActionDetails)&&Valid(ActionLabels);
        static bool Valid<T>(T[] items)where T:UnityEngine.Object=>items!=null&&items.Length==3&&Array.TrueForAll(items,item=>item!=null);
        Action close,purchase,friends,ad;bool closing;string language;float nextRefresh;
        void OnEnable(){RefreshContent();}
        void OnDisable(){Unbind();}
        void Update()
        {
            if(language!=DemoFeatureState.LanguageCode)RefreshContent();
            if(Time.unscaledTime<nextRefresh)return;nextRefresh=Time.unscaledTime+.25f;RefreshState();
        }
        public void Bind(bool modal,Action onClose,Action onPurchase,Action onFriends,Action onAd)
        {
            Unbind();close=onClose;purchase=onPurchase;friends=onFriends;ad=onAd;closing=false;
            Environment.gameObject.SetActive(!modal);Close.onClick.AddListener(CloseOnce);
            Actions[0].onClick.AddListener(Purchase);Actions[1].onClick.AddListener(Friends);Actions[2].onClick.AddListener(Ad);
            ActionsBound=true;RefreshContent();
        }
        void Unbind()
        {
            if(Close!=null)Close.onClick.RemoveListener(CloseOnce);
            if(Valid(Actions)){Actions[0].onClick.RemoveListener(Purchase);Actions[1].onClick.RemoveListener(Friends);Actions[2].onClick.RemoveListener(Ad);}
            close=purchase=friends=ad=null;ActionsBound=false;
        }
        bool InputAllowed(Button button)=>ActionsBound&&!closing&&isActiveAndEnabled&&button.IsInteractable();
        void CloseOnce(){if(!InputAllowed(Close))return;closing=true;Close.interactable=false;close?.Invoke();}
        void Purchase(){if(InputAllowed(Actions[0])&&CanUseOffer(0))purchase?.Invoke();}
        void Friends(){if(InputAllowed(Actions[1])&&CanUseOffer(1))friends?.Invoke();}
        void Ad(){if(InputAllowed(Actions[2])&&CanUseOffer(2))ad?.Invoke();}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true))t.font=font;
            string[] titles=en?new[]{"BUY ENERGY","ASK FRIENDS","WATCH AN AD"}:new[]{"КУПИТЬ ЭНЕРГИЮ","ПОПРОСИТЬ У ДРУЗЕЙ","СМОТРЕТЬ РЕКЛАМУ"};
            string[] details=en?new[]{"Purchases are not available yet","Friend requests are coming soon","Rewarded video is not available yet"}:new[]{"Покупки пока недоступны","Запросы друзьям скоро появятся","Видео с наградой пока недоступно"};
            for(int i=0;i<3;i++){ActionTitles[i].text=titles[i];ActionDetails[i].text=details[i];ActionLabels[i].text=en?"UNAVAILABLE":"НЕДОСТУПНО";Actions[i].interactable=ActionsBound&&CanUseOffer(i);}
            Close.interactable=ActionsBound&&!closing;RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var state=GameState.Instance;bool en=DemoFeatureState.LanguageCode=="en";
            if(Application.isPlaying&&state!=null)state.RefreshEnergyFromClock();
            var save=state!=null?state.Save:null;bool ready=save!=null&&save.energy>0;
            Title.text=ready?(en?"ENERGY AVAILABLE":"ЭНЕРГИЯ ЕСТЬ"):(en?"OUT OF ENERGY":"НЕТ ЭНЕРГИИ");
            Count.text=save==null?"—":Mathf.Max(0,save.energy).ToString();
            Message.text=ready?(en?"Energy is available.\nClose this window to continue.":"Энергии достаточно.\nЗакройте окно, чтобы продолжить."):(en?"You are out of energy.\nWait for it to recover to continue.":"Ваша энергия на исходе.\nПодождите восстановления, чтобы продолжить.");
            bool full=save!=null&&save.energy>=save.energyMax;
            TimerHeading.text=full?(en?"Energy is full":"Запас энергии полон"):(en?"Next energy in:":"До восстановления:");
            Countdown.text=save==null?"—":full?"—":FormatRemaining(state.EnergyRemaining());
            ProductionOutOfEnergyRuntime.ApplySourceState(this,ready,en);
        }
        public static string FormatRemaining(TimeSpan remaining)
        {long seconds=Math.Max(0,(long)Math.Ceiling(remaining.TotalSeconds));return (seconds/60).ToString("00")+":"+(seconds%60).ToString("00");}
    }
}
