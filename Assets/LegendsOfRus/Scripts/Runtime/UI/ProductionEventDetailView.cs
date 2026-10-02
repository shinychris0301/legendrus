using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionEventDetailView:MonoBehaviour
    {
        public const int CurrentRevision=5;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public ProductionPlayerHeader Header;
        public RectTransform Content,Body,Hud;
        public Image Environment,Title,RewardExamples;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,Footer;
        public Text EventName,Availability,ObjectiveHeading,Description,RewardHeading,RewardNote,ActionLabel;
        public Button ProfileButton,SettingsButton,BackButton,ActionButton;
        public Button[] PlusButtons;
        public bool ActionsBound{get;private set;}
        // A preview has no authoritative schedule, progress or claimable recipe.
        public bool CanParticipate=>false;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography&&Content&&Body&&Hud&&Environment&&Environment.sprite&&Title&&Title.sprite&&RewardExamples&&RewardExamples.sprite&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&Footer&&EventName&&Availability&&ObjectiveHeading&&Description&&RewardHeading&&RewardNote&&ActionLabel&&ProfileButton&&SettingsButton&&BackButton&&ActionButton&&PlusButtons!=null&&PlusButtons.Length==3&&Array.TrueForAll(PlusButtons,x=>x!=null);
        readonly List<Button> bound=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<bound.Count;i++)if(bound[i])bound[i].onClick.RemoveListener(listeners[i]);bound.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button button,Action action,bool route=false)
        {
            UnityAction listener=()=>{var router=ScreenRouter.Instance;if(!ActionsBound||committed||!isActiveAndEnabled||!button.IsInteractable()||router!=null&&(router.HasActiveModalBlocker||router.IsScreenTransitioning))return;if(route)committed=true;action?.Invoke();};
            button.onClick.AddListener(listener);bound.Add(button);listeners.Add(listener);
        }

        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;
            Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);
            for(int i=0;i<3;i++){int index=i;Listen(PlusButtons[i],()=>show(index==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            // No listener is attached to participation until a real event service exists.
            ActionsBound=true;RefreshContent();
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var text in GetComponentsInChildren<Text>(true))text.font=font;
            Heading.text="FIREBIRD\nFESTIVAL";Heading.enabled=en;HeroName.text=en?"Ivan":"Иван";
            Title.sprite=ProductionEventDetailRuntime.DetailArt(en?"blank_title":"title");
            var logo=Body.Find("EnglishLogo").GetComponent<Text>();logo.enabled=en;logo.text="LEGENDS OF RUS\nMATCH 3";
            EventName.text=en?"FIREBIRD FESTIVAL":"ФЕСТИВАЛЬ ЖАР-ПТИЦЫ";
            Availability.text=en?"Date to be announced":"Дата будет объявлена";
            Description.text=en?"Collect Firebird feathers.\nObjectives will be announced at launch.":"Собирайте перья Жар-Птицы.\nЗадания появятся перед стартом.";
            RewardHeading.text=en?"Reward examples":"Пример наград";
            ActionLabel.text="PLAY";ActionLabel.enabled=en;ActionButton.interactable=false;
            ActionButton.image.sprite=ProductionEventDetailRuntime.DetailArt(en?"blank_play":"play");
            Footer.text=en?"COMING SOON • PREVIEW":"СКОРО • ПРЕДПРОСМОТР";
            var leaders=Body.Find("LeadersPanel");
            leaders.Find("Heading").GetComponent<Text>().text=en?"LEADERS • PREVIEW":"ЛИДЕРЫ • ПРИМЕР";
            leaders.Find("PreviewNote").GetComponent<Text>().text=en?"Preview • ranking not open":"Пример • рейтинг ещё не открыт";
            string[] names=en?new[]{"Vasilisa","Dobrynya","Lyudmila","Ivan"}:new[]{"Василиса","Добрыня","Людмила","Иван"};
            for(int i=0;i<4;i++)leaders.Find("Row_"+i+"/Player").GetComponent<Text>().text=names[i];
            var premium=Body.Find("PremiumPanel");
            premium.Find("Heading").GetComponent<Text>().text=en?"PREMIUM PASS":"ПРЕМИУМ-ПРОПУСК";
            premium.Find("PreviewNote").GetComponent<Text>().text=en?"Pass preview.\nPurchase is not available yet.":"Предпросмотр пропуска.\nПокупка пока недоступна.";
            var pass=premium.Find("UnavailablePass").GetComponent<Button>();pass.interactable=false;
            pass.image.sprite=ProductionEventDetailRuntime.DetailArt(en?"blank_activate":"activate");
            var passLabel=pass.transform.Find("EnglishLabel").GetComponent<Text>();passLabel.enabled=en;passLabel.text="ACTIVATE";
            Body.Find("RewardPreviewLabel").GetComponent<Text>().text=en?"EXAMPLE":"ПРИМЕР";
            if(Header!=null)Header.ApplyPresentation();
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var save=GameState.Instance!=null?GameState.Instance.Save:null;
            GoldText.text=save==null?"—":Balance(save.gold);GemsText.text=save==null?"—":Balance(save.gems);EnergyText.text=save==null?"—":save.energy+"/"+save.energyMax;
            LevelText.text=(DemoFeatureState.LanguageCode=="en"?"Level ":"Уровень ")+(save==null?"—":save.level.ToString());
            if(Header!=null)Header.RefreshValues();
        }
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
