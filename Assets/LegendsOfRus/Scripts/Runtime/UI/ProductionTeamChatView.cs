using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionTeamChatView:MonoBehaviour
    {
        public const int CurrentRevision=6,MessageLimit=240,RetainedMessageLimit=20;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud,Messages,ChatViewport;
        public Image Environment,Title,ChestFill;
        public ProductionPlayerHeader Header;
        public Text ChestLevel;
        public Font BodyFont,NamesFont,TitleFont;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel,ChestTitle,ChestProgress,SendLabel,InputPlaceholder,LocalStatus,InfoHeading,InfoDescription,CloseInfoLabel;
        public Text[] FooterLabels;
        public Button ProfileButton,SettingsButton,BackButton,SendButton,ChestButton,ChestInfoButton,EmojiButton,CloseInfoButton;
        public Button[] PlusButtons,FooterButtons;
        public InputField Composer;
        public ScrollRect ChatScroll;
        public GameObject InfoPanel;
        public bool HasLiveChat=>false;
        public bool ActionsBound{get;private set;}
        public int LocalMessageCount=>local.Count;
        internal sealed class LocalMessage { public string Body,Time; }
        static readonly List<LocalMessage> local=new List<LocalMessage>();
        internal static IReadOnlyList<LocalMessage> LocalMessages=>local;
        static int lastDemoCount;
        public static void ResetLocalSession(){local.Clear();lastDemoCount=DemoFeatureState.TeamMessagesSent;}
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetRuntime(){local.Clear();lastDemoCount=0;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Header&&Header.HasBindings&&ChestFill&&ChestLevel&&Content&&Body&&Hud&&Messages&&ChatViewport&&Environment&&Environment.sprite&&Title&&Title.sprite&&BodyFont&&NamesFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&ChestTitle&&ChestProgress&&SendLabel&&InputPlaceholder&&LocalStatus&&InfoHeading&&InfoDescription&&CloseInfoLabel&&ProfileButton&&SettingsButton&&BackButton&&SendButton&&ChestButton&&ChestInfoButton&&EmojiButton&&CloseInfoButton&&Composer&&Composer.textComponent&&ChatScroll&&InfoPanel&&Valid(PlusButtons,3)&&Valid(FooterButtons,3)&&Valid(FooterLabels,3);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;int infoKind;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();if(Composer)Composer.onValueChanged.RemoveListener(InputChanged);ActionsBound=false;}
        bool CanAct()=>ActionsBound&&!committed&&isActiveAndEnabled&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning);
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{if(!CanAct()||!b.IsInteractable()||InfoPanel.activeSelf&&b!=CloseInfoButton)return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);
            for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>show(j==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);Listen(FooterButtons[i],()=>ShowInfo(j+1));}
            Listen(ChestButton,()=>ShowInfo(0));Listen(ChestInfoButton,()=>ShowInfo(0));Listen(EmojiButton,()=>ShowInfo(3));Listen(SendButton,()=>TrySendLocal());Listen(CloseInfoButton,CloseInfo);
            Composer.onValueChanged.AddListener(InputChanged);ActionsBound=true;RefreshContent();InputChanged(Composer.text);
        }
        void InputChanged(string value){if(SendButton)SendButton.interactable=!string.IsNullOrWhiteSpace(value);}
        public bool TrySendLocal()
        {
            if(!CanAct()||InfoPanel.activeSelf)return false;
            var clean=new StringBuilder();foreach(char c in Composer.text??string.Empty)if(!char.IsControl(c)||c=='\n')clean.Append(c);
            string message=clean.ToString().Trim();if(message.Length>MessageLimit){message=message.Substring(0,MessageLimit);if(char.IsHighSurrogate(message[message.Length-1]))message=message.Substring(0,message.Length-1);}if(string.IsNullOrWhiteSpace(message))return false;
            if(DemoFeatureState.TeamMessagesSent<lastDemoCount)local.Clear();
            local.Add(new LocalMessage{Body=message,Time=DateTime.Now.ToString("HH:mm",CultureInfo.InvariantCulture)});if(local.Count>RetainedMessageLimit)local.RemoveAt(0);lastDemoCount=DemoFeatureState.RecordTeamMessage();
            Composer.text=string.Empty;Composer.DeactivateInputField();ProductionTeamChatRuntime.RebuildMessages(this);Canvas.ForceUpdateCanvases();ChatScroll.verticalNormalizedPosition=0;RefreshLocalStatus();return true;
        }
        public void ShowInfo(int kind){if(!CanAct()||InfoPanel.activeSelf)return;infoKind=Mathf.Clamp(kind,0,4);Composer.DeactivateInputField();RefreshInfo();InfoPanel.SetActive(true);}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        void RefreshInfo(){bool en=DemoFeatureState.LanguageCode=="en";string[] headings=en?new[]{"GUILD CHEST","GIFTS","HELP REQUESTS","STICKERS","HELP REQUEST"}:new[]{"СУНДУК ГИЛЬДИИ","ПОДАРКИ","ЗАПРОСЫ ПОМОЩИ","СТИКЕРЫ","ЗАПРОС ПОМОЩИ"};InfoHeading.text=headings[infoKind];InfoDescription.text=en?"This is a design preview. Guild progress, gifts and help requests shown in chat are examples.\n\nOnline chat, verified membership, moderation, chest rewards, energy gifts and stickers are not connected yet.\n\nThe composer adds a message only to this local session. Nothing is sent to other players or added to your balance.":"Это предпросмотр дизайна. Прогресс гильдии, подарки и запросы помощи в чате показаны как примеры.\n\nОнлайн-чат, проверка участия, модерация, награды сундука, подарки энергии и стикеры пока не подключены.\n\nПоле ввода добавляет сообщение только в локальную сессию. Другие игроки его не получают, баланс не меняется.";CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            if(DemoFeatureState.TeamMessagesSent<lastDemoCount){local.Clear();lastDemoCount=DemoFeatureState.TeamMessagesSent;}
            foreach(var t in GetComponentsInChildren<Text>(true)){t.font=BodyFont;t.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;HeroName.font=NamesFont;foreach(var t in FooterLabels)t.font=TitleFont;
            Title.sprite=ProductionTeamChatRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"GUILD CHAT":"ЧАТ ГИЛЬДИИ";
            HeroName.text=en?"Ivan":"Иван";PreviewLabel.text=en?"SAMPLE CHAT":"ПРИМЕР ЧАТА";
            ChestTitle.text=en?"Guild chest · example":"Сундук гильдии · пример";ChestProgress.text="7 450 / 15 000";ChestLevel.text=en?"Lv. 6":"Ур. 6";ChestFill.fillAmount=7450f/15000f;
            SendButton.image.sprite=ProductionTeamChatRuntime.Load(en?"send_frame":"send_ru_rebuild");SendLabel.enabled=en;SendLabel.text=en?"SEND":"ОТПРАВИТЬ";InputPlaceholder.text=en?"Write a local message…":"Напишите локальное сообщение…";
            string[] labels=en?new[]{"GIFTS","HELP REQUESTS","STICKERS"}:new[]{"ПОДАРКИ","ЗАПРОСЫ ПОМОЩИ","СТИКЕРЫ"};for(int i=0;i<3;i++){FooterLabels[i].text=labels[i];FooterLabels[i].enabled=en;FooterButtons[i].image.sprite=ProductionTeamChatRuntime.Load(new[]{"footer_gift","footer_help","footer_sticker"}[i]+(en?"":"_ru_rebuild"));}
            Header.ApplyPresentation();RefreshState();RefreshInfo();RefreshLocalStatus();ProductionTeamChatRuntime.RebuildMessages(this);
        }
        void RefreshLocalStatus(){bool en=DemoFeatureState.LanguageCode=="en";LocalStatus.text=en?"LOCAL ONLY · "+local.Count+" MESSAGES":"ЛОКАЛЬНО · СООБЩЕНИЙ: "+local.Count;}
        public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;Header.RefreshValues();}
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
