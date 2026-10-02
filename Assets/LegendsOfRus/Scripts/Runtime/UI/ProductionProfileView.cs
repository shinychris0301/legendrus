using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public enum ProfileTab { Heroes, Relics, Items, Collection }
    public enum ProfileSection { Profile, Achievements, History }
    [DisallowMultipleComponent]
    public sealed class ProductionProfileView : MonoBehaviour
    {
        public const int CurrentRevision=1;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud,DynamicRoot;
        public Image Environment,Title,ProgressFill;
        public Font BodyFont,NamesFont,TitleFont;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,CharacterName,IdentityLabel,ProgressLabel,SectionLabel,InfoHeading,InfoDescription,CloseInfoLabel;
        public Text[] StatLabels,StatValues,TabLabels,FooterLabels,RouteLabels;
        public Button ProfileButton,SettingsButton,BackButton,EditButton,CloseInfoButton;
        public Button[] PlusButtons,Tabs,FooterButtons,RouteButtons;
        public GameObject InfoPanel;
        public ProfileTab SelectedTab {get;private set;}
        public ProfileSection Section {get;private set;}
        public int SelectedRelic {get;private set;}
        public int HistoryPage {get;private set;}
        public bool ActionsBound {get;private set;}
        public bool HasOnlineProfile=>false;
        public long CompletedCount {get;private set;}
        public long StarCount {get;private set;}
        public long AttemptCount {get;private set;}
        public int BestScore {get;private set;}
        public int ChapterCompleted {get;private set;}
        public int ChapterTotal {get;private set;}
        public List<LevelProgressData> Records {get;private set;}=new List<LevelProgressData>();
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Hud&&DynamicRoot&&Environment&&Environment.sprite&&Title&&Title.sprite&&ProgressFill&&BodyFont&&NamesFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&CharacterName&&IdentityLabel&&ProgressLabel&&SectionLabel&&InfoHeading&&InfoDescription&&CloseInfoLabel&&ProfileButton&&SettingsButton&&BackButton&&EditButton&&CloseInfoButton&&InfoPanel&&Valid(PlusButtons,3)&&Valid(Tabs,4)&&Valid(TabLabels,4)&&Valid(StatLabels,6)&&Valid(StatValues,6)&&Valid(FooterButtons,3)&&Valid(FooterLabels,3)&&Valid(RouteButtons,4)&&Valid(RouteLabels,4);
        static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        public bool CanAct()=>ActionsBound&&!committed&&isActiveAndEnabled&&!InfoPanel.activeSelf&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning);
        void Listen(Button b,Action action,bool route=false,bool inInfo=false){UnityAction call=()=>{bool allowed=inInfo?ActionsBound&&!committed&&isActiveAndEnabled&&InfoPanel.activeSelf&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning):CanAct();if(!allowed||!b.IsInteractable())return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action<ScreenId> show,Action back){
            Unbind();committed=false;Listen(ProfileButton,()=>SelectSection(ProfileSection.Profile));Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);Listen(EditButton,ShowInfo);
            for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>show(j==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);Listen(FooterButtons[i],()=>SelectSection((ProfileSection)j));}
            for(int i=0;i<4;i++){int j=i;Listen(Tabs[i],()=>SelectTab((ProfileTab)j));}
            ScreenId[] targets={ScreenId.IndividualRanking,ScreenId.TeamHub,ScreenId.Inventory,ScreenId.Login};for(int i=0;i<4;i++){int j=i;Listen(RouteButtons[i],()=>show(targets[j]),true,true);}Listen(CloseInfoButton,CloseInfo,false,true);ActionsBound=true;RefreshContent();
        }
        public void SelectTab(ProfileTab tab){if(!Enum.IsDefined(typeof(ProfileTab),tab))throw new ArgumentOutOfRangeException(nameof(tab));if(!CanAct())return;SelectedTab=tab;Section=ProfileSection.Profile;RefreshContent();}
        public void SelectSection(ProfileSection section){if(!Enum.IsDefined(typeof(ProfileSection),section))throw new ArgumentOutOfRangeException(nameof(section));if(!CanAct())return;Section=section;HistoryPage=0;RefreshContent();}
        public void SelectRelic(int index){if(!CanAct())return;if(index<0||index>=6)throw new ArgumentOutOfRangeException(nameof(index));SelectedRelic=index;ProductionProfileRuntime.RebuildContent(this);}
        public void ChangeHistoryPage(int delta){if(!CanAct())return;HistoryPage=Mathf.Clamp(HistoryPage+delta,0,Mathf.Max(0,(Records.Count-1)/6));ProductionProfileRuntime.RebuildContent(this);}
        public bool TryDismiss(){if(InfoPanel.activeSelf){CloseInfo();return true;}return false;}
        public void ShowInfo(){if(!CanAct())return;RefreshInfo();InfoPanel.SetActive(true);}
        public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
        void RefreshInfo(){bool en=DemoFeatureState.LanguageCode=="en";InfoHeading.text=en?"PROFILE AND ACCOUNT":"ПРОФИЛЬ И АККАУНТ";InfoDescription.text=en?"Ivan is the current hero. Statistics and booster quantities come from the local save.\n\nHero and relic collections are design previews. Their stars do not indicate ownership. Name editing, character upgrades, relic upgrades and online identity are not connected.\n\nProgress milestones are informational and grant no extra rewards.":"Иван — текущий герой. Статистика и количество усилителей взяты из локального сохранения.\n\nГерои и реликвии — примеры дизайна. Их звёзды не означают владение. Изменение имени, развитие героев и реликвий, онлайн-профиль пока не подключены.\n\nОтметки прогресса не выдают дополнительных наград.";CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";string[] labels=en?new[]{"RANKING","TEAM","INVENTORY","ACCOUNT"}:new[]{"РЕЙТИНГ","КОМАНДА","ИНВЕНТАРЬ","АККАУНТ"};for(int i=0;i<4;i++)RouteLabels[i].text=labels[i];}
        public void RefreshContent(){if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";foreach(var t in GetComponentsInChildren<Text>(true)){t.font=BodyFont;t.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;HeroName.font=NamesFont;foreach(var t in FooterLabels)t.font=TitleFont;
            Title.sprite=ProductionProfileRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"PROFILE":"ПРОФИЛЬ";HeroName.text=CharacterName.text=en?"Ivan":"Иван";IdentityLabel.text=en?"LOCAL PROGRESS":"ЛОКАЛЬНЫЙ ПРОГРЕСС";
            string[] tabs=en?new[]{"HEROES","RELICS","ITEMS","COLLECTION"}:new[]{"ГЕРОИ","РЕЛИКВИИ","ПРЕДМЕТЫ","КОЛЛЕКЦИЯ"};for(int i=0;i<4;i++){TabLabels[i].text=tabs[i];Tabs[i].image.sprite=ProductionProfileRuntime.Shared(Section==ProfileSection.Profile&&i==(int)SelectedTab?"tab_active":"tab");}
            string[] foot=en?new[]{"PROFILE","ACHIEVEMENTS","HISTORY"}:new[]{"ПРОФИЛЬ","ДОСТИЖЕНИЯ","ИСТОРИЯ"};for(int i=0;i<3;i++){FooterLabels[i].text=foot[i];FooterButtons[i].image.sprite=ProductionProfileRuntime.Shared(i==(int)Section?"tab_active":"tab");}
            string[] stats=en?new[]{"BEST SCORE","LEVELS CLEARED","ATTEMPTS","STARS","CHAPTER","LEVEL UNLOCKED"}:new[]{"ЛУЧШИЙ СЧЁТ","ПРОЙДЕНО","ПОПЫТКИ","ЗВЁЗДЫ","ГЛАВА","ОТКРЫТ УРОВЕНЬ"};for(int i=0;i<6;i++)StatLabels[i].text=stats[i];RefreshInfo();RefreshState();}
        public static List<LevelProgressData> ReadRecords(PlayerSaveData save){
            if(save==null||save.levelProgress==null)return new List<LevelProgressData>();
            return save.levelProgress.Where(p=>p!=null&&p.chapterId>0&&p.levelId>0).GroupBy(p=>new{p.chapterId,p.levelId}).Select(g=>new LevelProgressData{chapterId=g.Key.chapterId,levelId=g.Key.levelId,bestStars=g.Max(p=>Mathf.Clamp(p.bestStars,0,3)),bestScore=g.Max(p=>Mathf.Max(0,p.bestScore)),attempts=g.Max(p=>Mathf.Max(0,p.attempts)),completions=g.Max(p=>Mathf.Max(0,p.completions))}).OrderByDescending(p=>p.chapterId).ThenByDescending(p=>p.levelId).ToList();
        }
        public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Number(s.gold);GemsText.text=s==null?"—":Number(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;
            Records=ReadRecords(s);CompletedCount=Records.LongCount(p=>p.completions>0||p.bestStars>0);StarCount=Records.Sum(p=>(long)p.bestStars);AttemptCount=Records.Sum(p=>(long)p.attempts);BestScore=Records.Count==0?0:Records.Max(p=>p.bestScore);
            var catalog=ChapterLevelCatalog.LoadChapter1();ChapterTotal=catalog.levels.Length;ChapterCompleted=Records.Count(p=>p.chapterId==catalog.chapterId&&(p.completions>0||p.bestStars>0)&&Array.Exists(catalog.levels,l=>l!=null&&l.levelId==p.levelId));ProgressFill.fillAmount=ChapterTotal>0?(float)ChapterCompleted/ChapterTotal:0;ProgressLabel.text=(en?"Chapter 1: ":"Глава 1: ")+ChapterCompleted+" / "+ChapterTotal;
            string[] values={Number(BestScore),Number(CompletedCount),Number(AttemptCount),Number(StarCount),s==null?"—":Number(s.chapter),s==null?"—":Number(s.highestUnlockedLevel)};for(int i=0;i<6;i++)StatValues[i].text=values[i];HistoryPage=Mathf.Clamp(HistoryPage,0,Mathf.Max(0,(Records.Count-1)/6));ProductionProfileRuntime.RebuildContent(this);
        }
        public static string Number(long n)=>Math.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
