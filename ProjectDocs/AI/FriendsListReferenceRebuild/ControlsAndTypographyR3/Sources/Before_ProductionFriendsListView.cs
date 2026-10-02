using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public enum FriendsListTab { Friends, Requests, Find }
    [DisallowMultipleComponent]
    public sealed class ProductionFriendsListView:MonoBehaviour
    {
        public const int CurrentRevision=3;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud,RowsRoot;
        public Image Environment,Title;
        public ProductionPlayerHeader Header;
        public Font BodyFont,NamesFont,TitleFont;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel,SearchPlaceholder,Counter,SortLabel,RemoveLabel,EmptyLabel,InfoHeading,InfoDescription,CloseInfoLabel;
        public Text[] TabLabels,NavLabels;
        public Button ProfileButton,SettingsButton,BackButton,SortButton,RemoveButton,CloseInfoButton;
        public Button[] PlusButtons,Tabs,NavButtons;
        public InputField Search;
        public GameObject InfoPanel;
        public FriendsListTab SelectedTab{get;private set;}
        public bool SortByName{get;private set;}
        public bool ActionsBound{get;private set;}
        public bool HasLiveFriends=>false;
        public int VisibleCount{get;internal set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Header&&Header.HasBindings&&Content&&Body&&Hud&&RowsRoot&&Environment&&Environment.sprite&&Title&&Title.sprite&&BodyFont&&NamesFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&SearchPlaceholder&&Counter&&SortLabel&&RemoveLabel&&EmptyLabel&&InfoHeading&&InfoDescription&&CloseInfoLabel&&ProfileButton&&SettingsButton&&BackButton&&SortButton&&RemoveButton&&CloseInfoButton&&Search&&Search.textComponent&&InfoPanel&&Valid(PlusButtons,3)&&Valid(Tabs,3)&&Valid(TabLabels,3)&&Valid(NavButtons,5)&&Valid(NavLabels,5);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;int infoKind;string infoName="";
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();if(Search)Search.onValueChanged.RemoveListener(SearchChanged);ActionsBound=false;}
        bool CanAct()=>ActionsBound&&!committed&&isActiveAndEnabled&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning);
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{if(!CanAct()||!b.IsInteractable()||InfoPanel.activeSelf&&b!=CloseInfoButton)return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);
            for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>show(j==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);Listen(Tabs[i],()=>SelectTab((FriendsListTab)j));}
            ScreenId[] targets={ScreenId.WorldMap,ScreenId.Shop,ScreenId.Events,ScreenId.FriendsList,ScreenId.Characters};
            for(int i=0;i<5;i++){int j=i;if(j==3)Listen(NavButtons[i],()=>SelectTab(FriendsListTab.Friends));else Listen(NavButtons[i],()=>show(targets[j]),true);}
            Listen(SortButton,()=>{SortByName=!SortByName;RefreshContent();});Listen(RemoveButton,()=>ShowInfo(4,""));Listen(CloseInfoButton,CloseInfo);Search.onValueChanged.AddListener(SearchChanged);ActionsBound=true;RefreshContent();
        }
        public void SelectTab(FriendsListTab tab){if(!Enum.IsDefined(typeof(FriendsListTab),tab))throw new ArgumentOutOfRangeException(nameof(tab));SelectedTab=tab;Search.SetTextWithoutNotify("");RefreshContent();}
        void SearchChanged(string query){if(InfoPanel.activeSelf)return;ProductionFriendsListRuntime.RebuildRows(this);}
        public void ShowInfo(int kind,string name){if(!CanAct()||InfoPanel.activeSelf)return;infoKind=Mathf.Clamp(kind,0,5);infoName=name??"";Search.DeactivateInputField();RefreshInfo();InfoPanel.SetActive(true);}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        void RefreshInfo(){bool en=DemoFeatureState.LanguageCode=="en";string[] titles=en?new[]{"SEND ENERGY","VISIT PLAYER","FRIEND REQUEST","DECLINE REQUEST","REMOVE INACTIVE","ADD FRIEND"}:new[]{"ОТПРАВИТЬ ЭНЕРГИЮ","ПОСЕТИТЬ ИГРОКА","ЗАПРОС В ДРУЗЬЯ","ОТКЛОНИТЬ ЗАПРОС","УДАЛИТЬ НЕАКТИВНЫХ","ДОБАВИТЬ ДРУГА"};InfoHeading.text=titles[infoKind];InfoDescription.text=(string.IsNullOrEmpty(infoName)?"":infoName+"\n\n")+(en?"This is a sample Friends page. Names, levels, guilds, status and requests are examples.\n\nOnline friends, invitations, profile visits and energy gifts are not connected yet. This action does not change your friends, balance or rewards.\n\nSearch and sorting work locally on the examples.":"Это пример страницы друзей. Имена, уровни, гильдии, статусы и запросы показаны для дизайна.\n\nОнлайн-друзья, приглашения, посещения и подарки энергии пока не подключены. Действие не меняет список друзей, баланс и награды.\n\nПоиск и сортировка работают локально по примерам.");CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";}
        public void RefreshContent(){
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";foreach(var t in GetComponentsInChildren<Text>(true)){t.font=BodyFont;t.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;HeroName.font=NamesFont;foreach(var t in NavLabels)t.font=TitleFont;
            Title.sprite=ProductionFriendsListRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"FRIENDS":"ДРУЗЬЯ";HeroName.text=en?"Ivan":"Иван";PreviewLabel.text=en?"PREVIEW · SAMPLE FRIENDS AND REQUESTS":"ПРЕДПРОСМОТР · ПРИМЕР ДРУЗЕЙ И ЗАПРОСОВ";
            string[] tabs=en?new[]{"FRIENDS","REQUESTS","FIND"}:new[]{"ДРУЗЬЯ","ЗАПРОСЫ","НАЙТИ"};for(int i=0;i<3;i++){TabLabels[i].text=tabs[i];Tabs[i].image.sprite=ProductionFriendsListRuntime.Load("tab_"+i+(i==(int)SelectedTab?"_active":"")+"_r2");}
            string[] nav=en?new[]{"WORLD MAP","SHOP","EVENTS","FRIENDS","CHARACTERS"}:new[]{"КАРТА МИРА","МАГАЗИН","СОБЫТИЯ","ДРУЗЬЯ","ПЕРСОНАЖИ"};for(int i=0;i<5;i++){NavLabels[i].text=nav[i];NavLabels[i].enabled=en;NavButtons[i].image.sprite=ProductionFriendsListRuntime.Load("nav_"+i+(en?"":"_ru_rebuild"));}
            SearchPlaceholder.text=en?"Search sample names or guilds…":"Поиск по именам или гильдиям…";SortLabel.text=en?(SortByName?"SORT: NAME":"SORT: DEFAULT"):(SortByName?"ПО ИМЕНИ":"ПО ПОРЯДКУ");RemoveLabel.text=en?"REMOVE INACTIVE":"УДАЛИТЬ НЕАКТИВНЫХ";EmptyLabel.text=en?"No matching examples":"Подходящих примеров нет";Header.ApplyPresentation();RefreshInfo();RefreshState();ProductionFriendsListRuntime.RebuildRows(this);
        }
        public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;Header.RefreshValues();}
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
