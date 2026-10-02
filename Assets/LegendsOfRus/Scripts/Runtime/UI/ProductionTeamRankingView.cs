using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public enum TeamRankingScope { Local, Global }
    public enum TeamRankingPeriod { Week, Season }
    [DisallowMultipleComponent]
    public sealed class ProductionTeamRankingView:MonoBehaviour
    {
        public const int CurrentRevision=1;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud;
        public Image Environment,Title;
        public Font BodyFont,NamesFont,TitleFont,RankFont;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel,SeasonNote,PlayerGuildLabel;
        public Text DetailsLabel,InfoHeading,InfoDescription,CloseInfoLabel;
        public Text[] TabLabels,PodiumNames,PodiumMembers,PodiumScores,PodiumRewards,ColumnLabels,Ranks,Names,Members,Scores,Rewards;
        public RectTransform[] Rows,Podiums;
        public Image[] Crests;
        public Button ProfileButton,SettingsButton,BackButton,DetailsButton,CloseInfoButton;
        public Button[] PlusButtons,Tabs;
        public GameObject InfoPanel;
        public TeamRankingScope Scope{get;private set;}=TeamRankingScope.Global;
        public TeamRankingPeriod Period{get;private set;}=TeamRankingPeriod.Season;
        public bool HasLiveLeaderboard=>false;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Hud&&Environment&&Environment.sprite&&Title&&Title.sprite&&BodyFont&&NamesFont&&TitleFont&&RankFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&SeasonNote&&PlayerGuildLabel&&DetailsLabel&&InfoHeading&&InfoDescription&&CloseInfoLabel&&InfoPanel&&ProfileButton&&SettingsButton&&BackButton&&DetailsButton&&CloseInfoButton&&Valid(PlusButtons,3)&&Valid(Tabs,4)&&Valid(TabLabels,4)&&Valid(Podiums,3)&&Valid(PodiumNames,3)&&Valid(PodiumMembers,3)&&Valid(PodiumScores,3)&&Valid(PodiumRewards,9)&&Valid(ColumnLabels,5)&&Valid(Rows,7)&&Valid(Ranks,7)&&Valid(Names,7)&&Valid(Members,7)&&Valid(Scores,7)&&Valid(Rewards,21)&&Valid(Crests,7);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{var r=ScreenRouter.Instance;if(!ActionsBound||committed||!isActiveAndEnabled||!b.IsInteractable()||r!=null&&(r.HasActiveModalBlocker||r.IsScreenTransitioning)||InfoPanel.activeSelf&&b!=CloseInfoButton)return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);
            for(int i=0;i<3;i++){int index=i;Listen(PlusButtons[i],()=>show(index==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            Listen(Tabs[0],()=>SelectScope(TeamRankingScope.Local));Listen(Tabs[1],()=>SelectScope(TeamRankingScope.Global));Listen(Tabs[2],()=>SelectPeriod(TeamRankingPeriod.Week));Listen(Tabs[3],()=>SelectPeriod(TeamRankingPeriod.Season));
            Listen(DetailsButton,()=>InfoPanel.SetActive(true));Listen(CloseInfoButton,CloseInfo);ActionsBound=true;RefreshContent();
        }
        public void SelectScope(TeamRankingScope scope){if(!Enum.IsDefined(typeof(TeamRankingScope),scope))throw new ArgumentOutOfRangeException(nameof(scope));Scope=scope;RefreshContent();}
        public void SelectPeriod(TeamRankingPeriod period){if(!Enum.IsDefined(typeof(TeamRankingPeriod),period))throw new ArgumentOutOfRangeException(nameof(period));Period=period;RefreshContent();}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            foreach(var text in GetComponentsInChildren<Text>(true)){text.font=BodyFont;text.fontStyle=FontStyle.Normal;}
            Heading.font=TitleFont;HeroName.font=NamesFont;foreach(var t in Names)t.font=NamesFont;foreach(var t in PodiumNames)t.font=NamesFont;foreach(var t in Ranks)t.font=RankFont;
            Title.sprite=ProductionTeamRankingRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"GUILD RANKING":"РЕЙТИНГ ГИЛЬДИЙ";
            HeroName.text=en?"Ivan":"Иван";PreviewLabel.text=en?"PREVIEW · SAMPLE STANDINGS AND REWARDS":"ПРЕДПРОСМОТР · ПРИМЕР РЕЙТИНГА И НАГРАД";SeasonNote.text=en?"Season dates to be announced":"Даты сезона будут объявлены позже";
            string[] tabs=en?new[]{"LOCAL","GLOBAL","WEEK","SEASON"}:new[]{"ЛОКАЛЬНЫЙ","ГЛОБАЛЬНЫЙ","НЕДЕЛЯ","СЕЗОН"};
            for(int i=0;i<4;i++){TabLabels[i].text=tabs[i];bool active=i<2?i==(int)Scope:i-2==(int)Period;Tabs[i].image.sprite=ProductionTeamRankingRuntime.Shared(active?"tab_active":"tab");}
            string[] podiumNames=en?new[]{"Slavs","Northern Shield","Ancient Kin"}:new[]{"Славяне","Северный Щит","Род Древних"};string[] podiumMembers={"30/30","29/30","28/30"},podiumScores={"125 680","98 450","76 230"};string[] podiumRewards={"600","800","1","600","600","1","300","300","1"};
            for(int i=0;i<3;i++){PodiumNames[i].text=podiumNames[i];PodiumMembers[i].text=podiumMembers[i];PodiumScores[i].text=podiumScores[i];}for(int i=0;i<9;i++)PodiumRewards[i].text=podiumRewards[i];
            string[] columns=en?new[]{"RANK","GUILD","MEMBERS","CONTRIBUTION","REWARDS"}:new[]{"МЕСТО","ГИЛЬДИЯ","УЧАСТНИКИ","ВКЛАД","НАГРАДЫ"};for(int i=0;i<5;i++)ColumnLabels[i].text=columns[i];
            string[] names=en?new[]{"Wolf Guard","Stone Circle","Forest Shadows","Brotherhood of Rus","Iron Horn","Dawn Guardians","Fiery Hearts"}:new[]{"Волчья Стража","Каменный Круг","Лесные Тени","Братство Руси","Железный Рог","Стражи Рассвета","Огненные Сердца"};string[] members={"28/30","27/30","27/30","30/30","26/30","25/30","24/30"};int[] scores={58760,46120,38940,34560,29310,22870,18450};int[] rainbow={250,200,180,160,140,120,100},gems={300,250,200,180,150,130,110};
            for(int i=0;i<7;i++){Ranks[i].text=(i+4).ToString();Names[i].text=names[i];Members[i].text=members[i];Scores[i].text=Balance(scores[i]);Rewards[i*3].text=rainbow[i].ToString();Rewards[i*3+1].text=gems[i].ToString();Rewards[i*3+2].text="1";}
            PlayerGuildLabel.text=en?"PLAYER GUILD · EXAMPLE":"ГИЛЬДИЯ ИГРОКА · ПРИМЕР";DetailsLabel.text=en?"GUILD DETAILS":"ПОДРОБНОСТИ ГИЛЬДИИ";InfoHeading.text=en?"BROTHERHOOD OF RUS":"БРАТСТВО РУСИ";
            InfoDescription.text=en?"Sample guild · Rank 7\n30/30 members · Contribution 34 560\n\nLocal/Global and Week/Season show the same example standings. Guild membership, scores, season dates and rewards are not live.\n\nOnline ranking and guild details will be available in a later feature phase.":"Пример гильдии · Место 7\n30/30 участников · Вклад 34 560\n\nЛокальный/глобальный рейтинг и неделя/сезон показывают одну таблицу-пример. Состав гильдии, очки, даты сезона и награды пока не являются реальными данными.\n\nОнлайн-рейтинг и подробности гильдий появятся на этапе разработки функций.";
            CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";RefreshState();
        }
        public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Balance(s.gold);GemsText.text=s==null?"—":Balance(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;}
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
