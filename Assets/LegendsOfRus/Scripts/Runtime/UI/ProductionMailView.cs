using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public enum MailTab { Inbox, Sent, System }
    [DisallowMultipleComponent]
    public sealed class ProductionMailView : MonoBehaviour
    {
        public const int CurrentRevision=6;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud,DynamicRoot,TabStrip;
        public Image Environment,Title;
        public ProductionPlayerHeader Header;
        public Font BodyFont,NamesFont,TitleFont;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel,Counter,ClaimAllLabel,InfoHeading,InfoDescription,CloseInfoLabel;
        public Button ProfileButton,SettingsButton,BackButton,ClaimAllButton,ComposeButton,EnvelopeButton,DeleteButton,CloseInfoButton;
        public Button[] PlusButtons,Tabs;
        public Text[] TabLabels;
        public GameObject InfoPanel;
        public MailTab SelectedTab {get;private set;}
        public int SelectedMessage {get;private set;}
        public bool DetailOpen {get;private set;}
        public bool ActionsBound {get;private set;}
        public bool HasLiveMail=>false;
        readonly bool[] readExamples=new bool[5];
        public bool IsUnread(int index)=>index>=0&&index<5&&(index==0||index==1||index==3)&&!readExamples[index];
        public int UnreadCount {get {int n=0;for(int i=0;i<5;i++)if(IsUnread(i))n++;return n;}}
        public int VisibleCount {get;internal set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Header&&Header.HasBindings&&Content&&Body&&Hud&&TabStrip&&DynamicRoot&&Environment&&Environment.sprite&&Title&&Title.sprite&&BodyFont&&NamesFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&Counter&&ClaimAllLabel&&InfoHeading&&InfoDescription&&CloseInfoLabel&&ProfileButton&&SettingsButton&&BackButton&&ClaimAllButton&&ComposeButton&&EnvelopeButton&&DeleteButton&&CloseInfoButton&&InfoPanel&&Body.Find("LetterTitleBacking")&&Valid(PlusButtons,3)&&Valid(Tabs,3)&&Valid(TabLabels,3);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        public bool CanAct()=>ActionsBound&&!committed&&isActiveAndEnabled&&!InfoPanel.activeSelf&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning);
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{if(!CanAct()||!b.IsInteractable())return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,()=>{if(!TryDismiss()){committed=true;back();}});
            for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>show(j==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);Listen(Tabs[i],()=>SelectTab((MailTab)j));}
            Listen(ClaimAllButton,ShowInfo);Listen(ComposeButton,ShowInfo);Listen(EnvelopeButton,()=>SelectTab(MailTab.Inbox));Listen(DeleteButton,ShowInfo);
            UnityAction close=CloseInfo;CloseInfoButton.onClick.AddListener(close);buttons.Add(CloseInfoButton);listeners.Add(close);ActionsBound=true;RefreshContent();
        }
        public void SelectTab(MailTab tab){if(!Enum.IsDefined(typeof(MailTab),tab))throw new ArgumentOutOfRangeException(nameof(tab));SelectedTab=tab;SelectedMessage=tab==MailTab.System?3:0;DetailOpen=false;RefreshContent();}
        public void OpenMessage(int index){if(!CanAct())return;if(index<0||index>4)throw new ArgumentOutOfRangeException(nameof(index));SelectedMessage=index;readExamples[index]=true;DetailOpen=true;RefreshContent();}
        public bool TryDismiss(){if(InfoPanel.activeSelf){CloseInfo();return true;}if(DetailOpen){DetailOpen=false;RefreshContent();return true;}return false;}
        public void ShowInfo(){if(!CanAct())return;RefreshInfo();InfoPanel.SetActive(true);}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        void RefreshInfo(){bool en=DemoFeatureState.LanguageCode=="en";InfoHeading.text=en?"MAIL IS NOT CONNECTED":"ПОЧТА НЕ ПОДКЛЮЧЕНА";InfoDescription.text=en?"These messages, senders and attachments are examples for the design.\n\nSending and deleting mail, claiming attachments and Claim All are unavailable until the mail service is connected. No rewards or currency are granted.\n\nYou can browse sample letters and tabs.":"Письма, отправители и вложения показаны как примеры дизайна.\n\nОтправка и удаление писем, получение вложений и сбор всех наград будут доступны после подключения почты. Награды и валюта не начисляются.\n\nМожно просматривать примеры писем и вкладки.";CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";}
        public void RefreshContent(){
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";foreach(var t in GetComponentsInChildren<Text>(true)){t.font=BodyFont;t.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;HeroName.font=NamesFont;ClaimAllLabel.font=TitleFont;
            Title.sprite=ProductionMailRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"MAIL":"ПОЧТА";HeroName.text=en?"Ivan":"Иван";PreviewLabel.text=en?"PREVIEW · SAMPLE MAIL · CLAIMS UNAVAILABLE":"ПРИМЕРЫ ПИСЕМ · ПОЛУЧЕНИЕ НАГРАД НЕДОСТУПНО";
            TabStrip.gameObject.SetActive(!DetailOpen);string[] tabs=en?new[]{"INBOX","SENT","SYSTEM"}:new[]{"ВХОДЯЩИЕ","ОТПРАВЛЕННЫЕ","СИСТЕМА"};for(int i=0;i<3;i++){Tabs[i].gameObject.SetActive(!DetailOpen);TabLabels[i].text=tabs[i];TabLabels[i].font=TitleFont;Tabs[i].image.sprite=ProductionMailRuntime.Load(i==(int)SelectedTab?"tab_face_active_r2":"tab_face_r2");}
            ClaimAllButton.image.sprite=ProductionMailRuntime.Load(en?"claim_all_blank_rebuild":"claim_all_ru_rebuild");ClaimAllLabel.enabled=en;ClaimAllLabel.text=en?"CLAIM ALL":"ЗАБРАТЬ ВСЁ";RefreshInfo();RefreshState();ProductionMailRuntime.RebuildContent(this);
        }
        public void RefreshState(){if(!HasRequiredBindings)return;Header.RefreshValues();var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;}
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
