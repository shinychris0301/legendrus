using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionSpecialOfferView:MonoBehaviour
    {
        public const int CurrentRevision=4;
        public const string OfferId="starter";
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body,Hud;
        public ProductionPlayerHeader PlayerHeader;
        public Image Environment,Header,OfferCard,Hero;
        public Text Title,BundleName,ContentsNote,Price,Status,GoldText,GemsText,EnergyText,BackText,ShopText,PurchaseText;
        public Text[] RewardLabels,RewardAmounts;
        public Image[] RewardIcons;
        public Button BackButton,ShopButton,PurchaseButton;
        public bool ActionsBound{get;private set;}
        // Preserve the commercial identity without inventing a price, recipe, discount or expiry.
        // Provider binding alone does not establish a verified display catalog or entitlement recipe.
        public bool CanPurchase=>false;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&PlayerHeader!=null&&PlayerHeader.HasBindings&&Typography!=null&&Content!=null&&Body!=null&&Environment!=null&&Header!=null&&OfferCard!=null&&Hero!=null&&Title!=null&&BundleName!=null&&ContentsNote!=null&&Price!=null&&Status!=null&&GoldText!=null&&GemsText!=null&&EnergyText!=null&&BackText!=null&&ShopText!=null&&PurchaseText!=null&&BackButton!=null&&ShopButton!=null&&PurchaseButton!=null&&Valid(RewardLabels)&&Valid(RewardAmounts)&&Valid(RewardIcons);
        static bool Valid<T>(T[] a)where T:UnityEngine.Object=>a!=null&&a.Length==3&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool navigating;
        void OnEnable(){Observe();RefreshContent();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i]!=null)buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button b,Action action,bool navigation)
        {
            UnityAction l=()=>{var router=ScreenRouter.Instance;if(navigating||!isActiveAndEnabled||!b.IsInteractable()||router!=null&&(router.HasActiveModalBlocker||router.IsScreenTransitioning))return;if(navigation)navigating=true;action?.Invoke();};
            b.onClick.AddListener(l);buttons.Add(b);listeners.Add(l);
        }
        public void Bind(Action back,Action shop,Action<string> purchase)
        {
            Unbind();navigating=false;Listen(BackButton,back,true);Listen(ShopButton,shop,true);
            Listen(PurchaseButton,()=>{if(CanPurchase)purchase?.Invoke(OfferId);},false);ActionsBound=true;RefreshContent();
        }
        public void RefreshContent()
        {

            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var text in GetComponentsInChildren<Text>(true))text.font=font;
            Title.text=en?"SPECIAL\nOFFER":"ОСОБОЕ ПРЕДЛОЖЕНИЕ";
            BundleName.text=en?"HERO'S BUNDLE":"НАБОР БОГАТЫРЯ";
            string[] labels=en?new[]{"Coins","Gems","Bonuses"}:new[]{"Монеты","Самоцветы","Бонусы"};
            for(int i=0;i<3;i++){RewardLabels[i].text=labels[i];RewardAmounts[i].text="—";}
            ContentsNote.text=en?"Bundle contents have not been announced.":"Состав набора пока не объявлен.";
            Price.text=en?"Price unavailable":"Цена пока недоступна";
            Status.text=en?"This offer is not available yet.\nCheck back later.":"Предложение пока недоступно.\nЗагляните сюда позже.";
            PurchaseText.text=en?"BUY NOW":"КУПИТЬ СЕЙЧАС";BackText.text=en?"BACK":"НАЗАД";ShopText.text=en?"SHOP":"МАГАЗИН";
            PurchaseButton.interactable=ActionsBound&&CanPurchase;
            ProductionSpecialOfferRuntime.ApplyLocalizedArt(this,en);
            PlayerHeader.ApplyPresentation();
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;
            GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;
            if(PlayerHeader!=null)PlayerHeader.RefreshValues();
        }
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
