using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionShopView:MonoBehaviour
    {
        public const int CurrentRevision=2;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body,Hud;
        public ProductionPlayerHeader Header;
        public Image Environment,Title;
        public Text TitleText,HeroName,LevelText,GoldText,GemsText,EnergyText,SupportText,EmptyText,BackText,SpecialText;
        public Button ProfileButton,SettingsButton,BackButton,SpecialButton;
        public Button[] Tabs,NavButtons,PurchaseButtons,PlusButtons;
        public Text[] TabLabels,NavLabels,ProductNames,ProductDetails,PurchaseLabels;
        public RectTransform[] Cards;
        public GameObject EmptyPanel;
        public bool ActionsBound{get;private set;}
        // No verified display catalog or IAP entitlement recipes exist yet. Binding a port alone is insufficient.
        public bool CanPurchase(int index)=>false;
        public string SelectedCategory{get;private set;}="offers";
        public static readonly string[] OfferIds={"starter","hero","firebird",null};
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Header!=null&&Header.HasBindings&&Typography!=null&&Content!=null&&Body!=null&&Environment!=null&&Title!=null&&TitleText!=null&&HeroName!=null&&LevelText!=null&&GoldText!=null&&GemsText!=null&&EnergyText!=null&&SupportText!=null&&EmptyText!=null&&BackText!=null&&SpecialText!=null&&ProfileButton!=null&&SettingsButton!=null&&BackButton!=null&&SpecialButton!=null&&EmptyPanel!=null&&Valid(Tabs,4)&&Valid(NavButtons,5)&&Valid(PurchaseButtons,4)&&Valid(PlusButtons,3)&&Valid(TabLabels,4)&&Valid(NavLabels,5)&&Valid(ProductNames,4)&&Valid(ProductDetails,4)&&Valid(PurchaseLabels,4)&&Valid(Cards,4);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> bound=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Unbind(){for(int i=0;i<bound.Count;i++)if(bound[i]!=null)bound[i].onClick.RemoveListener(listeners[i]);bound.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button b,Action action,bool navigate=false)
        {
            UnityAction l=()=>{var router=ScreenRouter.Instance;if(committed||!isActiveAndEnabled||!b.IsInteractable()||router!=null&&(router.HasActiveModalBlocker||router.IsScreenTransitioning))return;if(navigate)committed=true;action?.Invoke();};
            b.onClick.AddListener(l);bound.Add(b);listeners.Add(l);
        }
        public void Bind(Action<ScreenId> show,Action back,Action<string> purchase)
        {
            Unbind();committed=false;
            Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);Listen(SpecialButton,()=>show(ScreenId.SpecialOffer),true);
            string[] tabs={"coins","gems","bundles","boosters"};string[] nav={"offers","gems","bundles","boosters","vip"};
            for(int i=0;i<4;i++){int index=i;Listen(Tabs[i],()=>SelectCategory(tabs[index]));Listen(PurchaseButtons[i],()=>{if(CanPurchase(index)&&OfferIds[index]!=null)purchase(OfferIds[index]);});}
            for(int i=0;i<5;i++){int index=i;Listen(NavButtons[i],()=>SelectCategory(nav[index]));}
            Listen(PlusButtons[0],()=>SelectCategory("coins"));Listen(PlusButtons[1],()=>SelectCategory("gems"));Listen(PlusButtons[2],()=>show(ScreenId.OutOfEnergy),true);
            ActionsBound=true;RefreshContent();
        }
        public void SelectCategory(string category)
        {
            if(Array.IndexOf(new[]{"offers","coins","gems","bundles","boosters","vip"},category)<0)throw new ArgumentOutOfRangeException(nameof(category));
            SelectedCategory=category;RefreshContent();
        }
        public void RefreshContent()
        {

            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true))t.font=font;
            Title.sprite=ProductionShopRuntime.Load(en?"title_blank_r2":"title_ru");TitleText.enabled=en;TitleText.text="SHOP";
            HeroName.text=en?"Ivan":"Иван";BackText.text=en?"BACK":"НАЗАД";SpecialText.text=en?"SPECIAL\nOFFER":"СПЕЦ.\nПРЕДЛОЖЕНИЕ";SpecialText.fontSize=en?20:17;
            string[] tabs=en?new[]{"COINS","GEMS","BUNDLES","BOOSTERS"}:new[]{"МОНЕТЫ","САМОЦВЕТЫ","НАБОРЫ","БУСТЕРЫ"};
            string[] nav=en?new[]{"OFFERS","GEMS","BUNDLES","BOOSTERS","VIP"}:new[]{"ПРЕДЛОЖЕНИЯ","САМОЦВЕТЫ","НАБОРЫ","БУСТЕРЫ","VIP"};
            string[] categories={"coins","gems","bundles","boosters"},navCategories={"offers","gems","bundles","boosters","vip"};
            for(int i=0;i<4;i++){TabLabels[i].text=tabs[i];Tabs[i].GetComponent<Image>().color=SelectedCategory==categories[i]?new Color32(150,205,255,255):Color.white;}
            for(int i=0;i<5;i++){NavLabels[i].text=nav[i];NavLabels[i].fontSize=en?20:i==0?18:i==1?20:22;NavLabels[i].horizontalOverflow=HorizontalWrapMode.Overflow;NavLabels[i].color=SelectedCategory==navCategories[i]?Color.white:new Color32(235,194,116,255);}
            string[] names=en?new[]{"Bag of Coins","Hero's Chest","Golden Cart","Mage's Bundle"}:new[]{"Мешок монет","Сундук богатыря","Золотая телега","Набор мага"};
            int visible=0;
            for(int i=0;i<4;i++)
            {
                bool show=SelectedCategory=="offers"||SelectedCategory=="coins"&&(i==0||i==2)||SelectedCategory=="bundles"&&(i==1||i==3);
                Cards[i].gameObject.SetActive(show);if(show){Cards[i].anchoredPosition=new Vector2(39+(visible%2)*441,-393-(visible/2)*502);visible++;}
                ProductNames[i].text=names[i];ProductDetails[i].text=en?"Coming soon":"Скоро в продаже";PurchaseLabels[i].text=en?"UNAVAILABLE":"НЕДОСТУПНО";PurchaseButtons[i].interactable=ActionsBound&&CanPurchase(i);
            }
            EmptyPanel.SetActive(visible==0);EmptyText.text=en?"This section is coming soon.\nNo products are available yet.":"Раздел скоро откроется.\nТовары пока недоступны.";
            SupportText.text=en?"Purchases are not available yet.":"Покупки пока недоступны.";
            ProductionShopRuntime.ApplyLocalizedArtwork(this,en);
            Header.ApplyPresentation();
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;
            GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;
            if(Header!=null)Header.RefreshValues();
            LevelText.text=(DemoFeatureState.LanguageCode=="en"?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());
        }
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
