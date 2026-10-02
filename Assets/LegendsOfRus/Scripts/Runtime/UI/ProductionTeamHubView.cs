using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionTeamHubView:MonoBehaviour
    {
        public const int CurrentRevision=7;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud;
        public Image Environment,Title,SeasonFill;
        public const int SampleSeasonContribution=7560,SampleSeasonTarget=10000;
        public const float SampleSeasonFraction=(float)SampleSeasonContribution/SampleSeasonTarget;
        public ProductionHomePanel ProgressFill;
        public Font BodyFont,TitleFont,CaptionFont;
        public ProductionPlayerHeader Header;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel;
        public Text GuildName,MemberCount,Membership,PowerHeading,Power,ProgressHeading,Progress,ChestLabel,TasksHeading,MembersHeading,ContributionHeading,Motto;
        public Text[] TaskLabels,TaskCounts,TaskRewards,NavLabels,MemberRanks,MemberNames,MemberRoles,MemberScores;
        public RectTransform[] MemberRows;
        public Image[] Portraits;
        public Button ProfileButton,SettingsButton,BackButton,FriendsButton,ChestButton,GuildInfoButton;
        public Button[] PlusButtons,NavButtons;
        public Text FriendsLabel,InfoHeading,InfoDescription,CloseInfoText;
        public Button CloseInfoButton;
        public GameObject InfoPanel;
        public bool HasLiveTeam=>false;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Header&&Header.HasBindings&&CaptionFont&&Content&&Body&&Hud&&Environment&&Environment.sprite&&Title&&Title.sprite&&ProgressFill&&SeasonFill&&SeasonFill.sprite&&BodyFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&GuildName&&MemberCount&&Membership&&PowerHeading&&Power&&ProgressHeading&&Progress&&ChestLabel&&TasksHeading&&MembersHeading&&ContributionHeading&&Motto&&ProfileButton&&SettingsButton&&BackButton&&FriendsButton&&ChestButton&&GuildInfoButton&&FriendsLabel&&InfoHeading&&InfoDescription&&CloseInfoText&&CloseInfoButton&&InfoPanel&&Valid(PlusButtons,3)&&Valid(NavButtons,4)&&Valid(NavLabels,4)&&Valid(TaskLabels,3)&&Valid(TaskCounts,3)&&Valid(TaskRewards,3)&&Valid(MemberRows,5)&&Valid(Portraits,5)&&Valid(MemberRanks,5)&&Valid(MemberNames,5)&&Valid(MemberRoles,5)&&Valid(MemberScores,5);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;int infoKind;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{var r=ScreenRouter.Instance;if(!ActionsBound||committed||!isActiveAndEnabled||!b.IsInteractable()||r!=null&&(r.HasActiveModalBlocker||r.IsScreenTransitioning)||InfoPanel.activeSelf&&b!=CloseInfoButton)return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;
            Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);Listen(FriendsButton,()=>show(ScreenId.FriendsList),true);
            for(int i=0;i<3;i++){int index=i;Listen(PlusButtons[i],()=>show(index==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            Listen(NavButtons[0],()=>show(ScreenId.TeamChat),true);Listen(NavButtons[1],()=>show(ScreenId.TeamRanking),true);
            Listen(NavButtons[2],()=>OpenInfo(0));Listen(NavButtons[3],()=>OpenInfo(1));Listen(ChestButton,()=>OpenInfo(2));Listen(GuildInfoButton,()=>OpenInfo(3));Listen(CloseInfoButton,CloseInfo);
            ActionsBound=true;RefreshContent();
        }
        void OpenInfo(int kind){infoKind=kind;RefreshInfo();InfoPanel.SetActive(true);}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            foreach(var text in GetComponentsInChildren<Text>(true)){text.font=CaptionFont;text.fontStyle=FontStyle.Normal;}
            foreach(var name in MemberNames)name.font=BodyFont;
            Heading.font=TitleFont;Heading.text=en?"GUILD":"ГИЛЬДИЯ";Heading.enabled=en;Title.sprite=ProductionTeamHubRuntime.Load(en?"title_blank":"title_ru");
            HeroName.text=en?"Ivan":"Иван";PreviewLabel.text=en?"PREVIEW · SAMPLE GUILD AND REWARDS":"ПРЕДПРОСМОТР · ПРИМЕР ГИЛЬДИИ И НАГРАД";
            GuildName.font=TitleFont;GuildName.text=en?"FALCONS OF RUS":"СОКОЛЫ РУСИ";MemberCount.text="32/40";Membership.text=en?"Open · example":"Открытая · пример";
            PowerHeading.text=en?"GUILD POWER":"МОЩЬ ГИЛЬДИИ";Power.text="125 840";ProgressHeading.text=en?"SEASON CONTRIBUTION · EXAMPLE":"ВКЛАД В СЕЗОН · ПРИМЕР";Progress.text=Balance(SampleSeasonContribution)+" / "+Balance(SampleSeasonTarget);SeasonFill.fillAmount=SampleSeasonFraction;((RectTransform)ProgressFill.transform).sizeDelta=new Vector2(233*SampleSeasonFraction,20);ChestLabel.text=en?"GUILD CHEST":"ГИЛЬДЕЙСКИЙ\nСУНДУК";
            TasksHeading.text=en?"GUILD TASK EXAMPLES":"ЕЖЕДНЕВНЫЕ ЗАДАНИЯ ГИЛЬДИИ";
            TasksHeading.enabled=en;ChestLabel.enabled=en;
            Body.Find("TasksEnclosure").GetComponent<Image>().sprite=ProductionTeamHubRuntime.Load(en?"tasks_enclosure_blank_r2":"tasks_enclosure_ru_r2");
            Body.Find("GuildChestEnclosure").GetComponent<Image>().sprite=ProductionTeamHubRuntime.Load(en?"chest_enclosure_blank_r2":"chest_enclosure_ru_r2");
            string[] tasks=en?new[]{"Donate\ncoins","Win\nbattles","Use\ngems"}:new[]{"Пожертвовать\nмонеты","Победить в\nсражениях","Использовать\nсамоцветы"};
            string[] counts={"0 / 5 000","0 / 10","0 / 50"};for(int i=0;i<3;i++){TaskLabels[i].text=tasks[i];TaskCounts[i].text=counts[i];TaskRewards[i].text=i==1?"150":"100";}
            string[] nav=en?new[]{"CHAT","RANKING","TASKS","DONATE"}:new[]{"ЧАТ","РЕЙТИНГ","ЗАДАНИЯ","ПОЖЕРТВОВАТЬ"};for(int i=0;i<4;i++){NavLabels[i].text=nav[i];NavLabels[i].enabled=en;
                NavButtons[i].image.sprite=ProductionTeamHubRuntime.Load("nav_"+i+(en?"_blank":"_ru"));}
            MembersHeading.text=en?"GUILD MEMBERS · EXAMPLE":"УЧАСТНИКИ ГИЛЬДИИ · ПРИМЕР";ContributionHeading.text=en?"Season contribution":"Вклад в сезон";
            string[] names=en?new[]{"Ivan","Olga","Svyatoslav","Velimir","Lada"}:new[]{"Иван","Ольга","Святослав","Велимир","Лада"};string[] values={"2 450","1 890","1 620","980","620"};
            for(int i=0;i<5;i++){MemberRanks[i].text=(i+1).ToString();MemberNames[i].text=names[i];MemberRoles[i].text=i==0?(en?"Leader":"Лидер"):i<3?(en?"Officer":"Офицер"):"";MemberScores[i].text=values[i];}
            Motto.enabled=en;Body.Find("MottoFrame").GetComponent<Image>().sprite=ProductionTeamHubRuntime.Load(en?"motto_blank_r4":"motto_ru_r4");
            Motto.text=en?"Together we are strong! Glory to Rus!":"Вместе мы — сила! Слава Руси!";FriendsLabel.text=en?"FRIENDS":"ДРУЗЬЯ";RefreshInfo();Header.ApplyPresentation();RefreshState();
        }
        void RefreshInfo(){if(!HasRequiredBindings)return;bool en=DemoFeatureState.LanguageCode=="en";InfoHeading.text=en?new[]{"GUILD TASKS","DONATIONS","GUILD CHEST","GUILD"}[infoKind]:new[]{"ЗАДАНИЯ ГИЛЬДИИ","ПОЖЕРТВОВАНИЯ","СУНДУК ГИЛЬДИИ","ГИЛЬДИЯ"}[infoKind];InfoDescription.text=en?"Coming later\n\nOnline guild services are not available yet. The roster, progress, tasks and rewards shown here are examples.\n\nNo currency is spent and no reward is claimed.":"Скоро\n\nСервисы гильдии пока недоступны. Состав, прогресс, задания и награды показаны для примера.\n\nВалюта не списывается, награда не выдаётся.";CloseInfoText.text=en?"CLOSE":"ЗАКРЫТЬ";}
        public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;Header.RefreshValues();}
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
