using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionSettingsView : MonoBehaviour
    {
        public const int CurrentRevision=4;
        public int Revision=CurrentRevision;
        public RectTransform Content, Body, Hud;
        public Image Environment, Title;
        public Font BodyFont, TitleFont;
        public ProductionPlayerHeader Header;
        public Text SignOutNote;
        public Text Heading, HeroName, LevelText, GoldText, GemsText, EnergyText, VersionText;
        public Text LanguageValue, CloudValue, InfoHeading, InfoDescription, CloseInfoLabel;
        public Button ProfileButton, SettingsButton, BackButton, CloseInfoButton;
        public Button[] PlusButtons, SwitchButtons, Rows, LanguageButtons;
        public Text[] SwitchLabels, RowLabels, LanguageLabels;
        public GameObject InfoPanel, LanguageChoices, GeneralInfoContent;
        public Image LanguagePanel;
        public Text LanguageHeading;
        public Button CloseLanguageButton;
        public bool ActionsBound { get; private set; }
        public bool HasRequiredBindings => Revision==CurrentRevision && Header && Header.HasBindings && SignOutNote && Content && Body && Hud && Environment && Environment.sprite && Title && Title.sprite && BodyFont && TitleFont && Heading && HeroName && LevelText && GoldText && GemsText && EnergyText && VersionText && LanguageValue && CloudValue && InfoHeading && InfoDescription && CloseInfoLabel && ProfileButton && SettingsButton && BackButton && CloseInfoButton && InfoPanel && LanguageChoices && GeneralInfoContent && LanguagePanel && LanguageHeading && CloseLanguageButton && Valid(PlusButtons,3) && Valid(SwitchButtons,4) && Valid(SwitchLabels,4) && Valid(Rows,6) && Valid(RowLabels,6) && Valid(LanguageButtons,2) && Valid(LanguageLabels,2);
        static bool Valid<T>(T[] values,int count) where T:UnityEngine.Object => values!=null && values.Length==count && Array.TrueForAll(values,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;
        string language;
        bool committed;
        int infoKind;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        bool InputReady(){if(!ActionsBound||committed||!isActiveAndEnabled)return false;var router=ScreenRouter.Instance;if(router==null)return true;if(router.ActiveModal==ScreenId.Settings)return !router.IsModalTransitioning;return !router.HasActiveModalBlocker&&!router.IsScreenTransitioning;}
        public bool CanAct()=>InputReady()&&!InfoPanel.activeSelf;
        void Listen(Button button,Action action,bool overlay=false){UnityAction call=()=>{if(!InputReady()||!button.IsInteractable()||(!overlay&&InfoPanel.activeSelf))return;action();};button.onClick.AddListener(call);buttons.Add(button);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back,Action<string> shop)
        {
            Unbind();committed=false;
            Listen(BackButton,()=>{committed=true;back();});
            Listen(ProfileButton,()=>Route(()=>show(ScreenId.Profile)));
            Listen(SettingsButton,()=>{});
            for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>Route(()=>{if(j==2)show(ScreenId.OutOfEnergy);else shop(j==0?"coins":"gems");}));}
            for(int i=0;i<4;i++){int j=i;Listen(SwitchButtons[i],()=>TogglePreference(j));}
            for(int i=0;i<6;i++){int j=i;Listen(Rows[i],()=>{if(j==2&&!(ScreenRouter.Instance!=null&&ScreenRouter.Instance.ActiveModal==ScreenId.Settings))Route(()=>show(ScreenId.Login));else ShowInfo(j);});}
            Listen(CloseInfoButton,()=>TryDismiss(),true);
            Listen(CloseLanguageButton,()=>{if(LanguageChoices.activeInHierarchy)TryDismiss();},true);
            for(int i=0;i<2;i++){int j=i;Listen(LanguageButtons[i],()=>{if(!InfoPanel.activeSelf||infoKind!=0)return;DemoFeatureState.SelectLanguage(j==0?"ru":"en");RefreshContent();},true);}
            ActionsBound=true;RefreshContent();
        }
        void Route(Action action){if(ScreenRouter.Instance!=null&&ScreenRouter.Instance.ActiveModal==ScreenId.Settings){ShowInfo(6);return;}committed=true;action();}
        public void TogglePreference(int index)
        {
            if(index<0||index>3)throw new ArgumentOutOfRangeException(nameof(index));
            if(!CanAct()||GameState.Instance==null)return;
            switch(index){case 0:GameState.Instance.ToggleMusic();break;case 1:GameState.Instance.ToggleSfx();break;case 2:GameState.Instance.ToggleVibration();break;case 3:GameState.Instance.ToggleNotifications();break;}
            RefreshState();
        }
        public bool Preference(int index){var s=GameState.Instance!=null?GameState.Instance.Save:null;if(index<0||index>3)throw new ArgumentOutOfRangeException(nameof(index));return s!=null&&new[]{s.music,s.sfx,s.vibration,s.notifications}[index];}
        public bool TryDismiss(){if(!InfoPanel||!InfoPanel.activeSelf)return false;InfoPanel.SetActive(false);return true;}
        public void ShowInfo(int kind){if(!CanAct())return;if(kind<0||kind>6)throw new ArgumentOutOfRangeException(nameof(kind));infoKind=kind;RefreshInfo();InfoPanel.SetActive(true);}
        static readonly string[] RuRows={"Язык","Облако","Аккаунт","Поддержка","Конфиденциальность","ВЫЙТИ ИЗ АККАУНТА"};
        static readonly string[] EnRows={"Language","Cloud","Account","Support","Privacy","SIGN OUT"};
        void RefreshInfo()
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            InfoHeading.text=infoKind==6?(en?"PAUSED GAME":"ИГРА НА ПАУЗЕ"):(en?EnRows:RuRows)[infoKind];
            string[] ru={"Выберите язык интерфейса для текущего сеанса.","Прогресс хранится на этом устройстве. Облачная синхронизация пока недоступна.","Сейчас используется локальный профиль. Привязка аккаунта и восстановление пока недоступны.","Обращение в службу поддержки пока недоступно. Версия игры: "+Application.version,"Раздел документов и управления согласием пока недоступен. Ваши настройки согласия не изменены.","Выход и смена аккаунта пока недоступны. Локальный прогресс сохранён.","Закройте настройки и вернитесь к игре, прежде чем открывать другие страницы."};
            string[] eng={"Choose the interface language for this session.","Progress is stored on this device. Cloud synchronization is not available yet.","You are using a local profile. Account linking and recovery are not available yet.","Contacting support is not available yet. Game version: "+Application.version,"Documents and consent controls are not available yet. Your consent settings have not changed.","Sign out and account switching are not available yet. Local progress is preserved.","Close Settings and return to the game before opening another page."};
            InfoDescription.text=(en?eng:ru)[infoKind];CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";
            LanguageChoices.SetActive(infoKind==0);GeneralInfoContent.SetActive(infoKind!=0);
            var shield=InfoPanel.transform.Find("InputShield").GetComponent<ProductionHomePanel>();
            shield.LeftColor=shield.RightColor=new Color(0,0,0,infoKind==0?.60f:.92f);shield.SetVerticesDirty();
            LanguagePanel.sprite=ProductionSettingsRuntime.Load(en?"language_panel_blank":"language_panel_ru");
            LanguageHeading.enabled=en;LanguageHeading.font=TitleFont;LanguageHeading.text=en?"SELECT LANGUAGE":"ВЫБОР ЯЗЫКА";
            LanguageLabels[0].text="РУССКИЙ";LanguageLabels[1].text="ENGLISH";
            for(int i=0;i<2;i++){LanguageLabels[i].font=TitleFont;LanguageButtons[i].image.sprite=ProductionSettingsRuntime.Load((en?1:0)==i?"language_selected":"language_unselected");}
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            foreach(var text in GetComponentsInChildren<Text>(true)){if(text.transform.IsChildOf(Hud))continue;text.font=BodyFont;text.fontStyle=FontStyle.Normal;}
            Heading.font=TitleFont;Title.sprite=ProductionSettingsRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"SETTINGS":"НАСТРОЙКИ";
            SignOutNote.text=en?"Not available yet":"Пока недоступно";
            string[] labels=en?new[]{"Music","Sound","Vibration","Notifications"}:new[]{"Музыка","Звуки","Вибрация","Уведомления"};
            for(int i=0;i<4;i++)SwitchLabels[i].text=labels[i];
            for(int i=0;i<6;i++)RowLabels[i].text=(en?EnRows:RuRows)[i];
            LanguageValue.text=en?"English":"Русский";CloudValue.text=en?"Local save":"На устройстве";
            VersionText.text=(en?"Version ":"Версия ")+Application.version;
            RefreshInfo();RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";
            Header.RefreshValues();
            for(int i=0;i<4;i++){SwitchButtons[i].image.sprite=ProductionSettingsRuntime.Load(Preference(i)?"switch_on":"switch_off");SwitchButtons[i].interactable=s!=null;}
        }
    }
}
