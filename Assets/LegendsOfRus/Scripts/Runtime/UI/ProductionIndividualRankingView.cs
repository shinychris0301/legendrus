using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public enum IndividualRankingPeriod{Day,Week,Season}
    [DisallowMultipleComponent]
    public sealed class ProductionIndividualRankingView:MonoBehaviour
    {
        public const int CurrentRevision=6;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public Font PlayerNamesFont,HeadingFont,RankNumbersFont;
        public RectTransform Content,Body,Hud,PlayerRow;
        public Image Environment,Title;
        public Sprite OriginalRussianTitle;
        public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,Footer,PreviewLabel,SeasonHeading,SeasonNote,EmptyText;
        public Text PlayerRank,PlayerName,PlayerStatus,PlayerScore;
        public Text[] TabLabels,ColumnLabels,Ranks,Names,Scores;
        public RectTransform[] Rows;
        public Button ProfileButton,SettingsButton,BackButton;
        public Button[] PlusButtons,Tabs;
        public GameObject EmptyPanel;
        public bool ActionsBound{get;private set;}
        public IndividualRankingPeriod Period{get;private set;}=IndividualRankingPeriod.Season;
        public Text RewardsHeading,RulesButtonText,RulesHeading,RulesDescription,CloseRulesText;
        public Button RulesButton,CloseRulesButton;
        public GameObject RulesPanel;
        public RectTransform[] PrizeFrames;
        public Text[] PrizeTitles,PrizeGems,PrizeCards;
        public Image[] RowPortraits,RowRewards;
        public bool HasLiveLeaderboard=>false;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography&&PlayerNamesFont&&HeadingFont&&RankNumbersFont&&Content&&Body&&Hud&&PlayerRow&&Environment&&Environment.sprite&&Title&&Title.sprite&&OriginalRussianTitle&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&Footer&&PreviewLabel&&SeasonHeading&&SeasonNote&&EmptyText&&PlayerRank&&PlayerName&&PlayerStatus&&PlayerScore&&ProfileButton&&SettingsButton&&BackButton&&EmptyPanel&&Valid(PlusButtons,3)&&Valid(Tabs,3)&&Valid(TabLabels,3)&&Valid(ColumnLabels,4)&&Valid(Rows,14)&&Valid(Ranks,14)&&Valid(Names,14)&&Valid(Scores,14)&&RewardsHeading&&RulesButton&&RulesButtonText&&RulesHeading&&RulesDescription&&CloseRulesText&&CloseRulesButton&&RulesPanel&&Valid(PrizeFrames,3)&&Valid(PrizeTitles,3)&&Valid(PrizeGems,3)&&Valid(PrizeCards,3)&&Valid(RowPortraits,14)&&Valid(RowRewards,14);
        static bool Valid<T>(T[] items,int length)where T:UnityEngine.Object=>items!=null&&items.Length==length&&Array.TrueForAll(items,x=>x!=null);
        readonly List<Button> bound=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        GameState observed;string language;bool committed;
        void OnEnable(){Observe();RefreshContent();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
        void Unbind(){for(int i=0;i<bound.Count;i++)if(bound[i])bound[i].onClick.RemoveListener(listeners[i]);bound.Clear();listeners.Clear();ActionsBound=false;}
        void Listen(Button button,Action action,bool route=false)
        {
            UnityAction listener=()=>{var router=ScreenRouter.Instance;if(!ActionsBound||committed||!isActiveAndEnabled||!button.IsInteractable()||router!=null&&(router.HasActiveModalBlocker||router.IsScreenTransitioning))return;if(RulesPanel.activeSelf&&button!=CloseRulesButton)return;if(route)committed=true;action?.Invoke();};
            button.onClick.AddListener(listener);bound.Add(button);listeners.Add(listener);
        }

        public void Bind(Action<ScreenId> show,Action back)
        {
            Unbind();committed=false;
            Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);
            for(int i=0;i<3;i++){int index=i;Listen(PlusButtons[i],()=>show(index==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            for(int i=0;i<3;i++){int index=i;Listen(Tabs[i],()=>SelectPeriod((IndividualRankingPeriod)index));}
            Listen(RulesButton,()=>RulesPanel.SetActive(true));Listen(CloseRulesButton,CloseRules);
            ActionsBound=true;RefreshContent();
        }

        public void SelectPeriod(IndividualRankingPeriod period)
        {if(!Enum.IsDefined(typeof(IndividualRankingPeriod),period))throw new ArgumentOutOfRangeException(nameof(period));Period=period;RefreshContent();}
        public void CloseRules(){if(RulesPanel!=null)RulesPanel.SetActive(false);}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var text in GetComponentsInChildren<Text>(true))text.font=font;
            // User-selected page roles must survive locale, tab and data refreshes.
            Heading.font=HeadingFont;Heading.fontStyle=FontStyle.Normal;
            HeroName.font=PlayerNamesFont;HeroName.fontStyle=FontStyle.Normal;
            foreach(var text in Names){text.font=PlayerNamesFont;text.fontStyle=FontStyle.Normal;}
            foreach(var text in Ranks){text.font=RankNumbersFont;text.fontStyle=FontStyle.Normal;}
            Title.sprite=en?ProductionIndividualRankingReferenceLayout.Load("title"):OriginalRussianTitle;
            Title.rectTransform.anchoredPosition=new Vector2(113,en?-107:-103);
            Title.rectTransform.sizeDelta=new Vector2(743,en?169:173);
            Heading.enabled=en; // Russian title preserves the source lettering; English stays live.
            Heading.text=en?"INDIVIDUAL RANKING":"ЛИЧНЫЙ РЕЙТИНГ";HeroName.text=en?"Ivan":"Иван";
            PreviewLabel.text=en?"PREVIEW · SAMPLE STANDINGS AND REWARDS":"ПРЕДПРОСМОТР · ПРИМЕР РЕЙТИНГА И НАГРАД";
            SeasonNote.text=en?"Season dates to be announced":"Даты сезона будут объявлены позже";
            string[] periods=en?new[]{"DAY","WEEK","SEASON"}:new[]{"ДЕНЬ","НЕДЕЛЯ","СЕЗОН"};
            for(int i=0;i<3;i++){TabLabels[i].text=periods[i];Tabs[i].image.sprite=ProductionIndividualRankingReferenceLayout.Load((int)Period==i?"tab_active":"tab");}
            RewardsHeading.text=en?"REWARDS BY PLACE":"НАГРАДЫ ЗА МЕСТО";
            for(int i=0;i<3;i++){PrizeTitles[i].text=en?new[]{"1ST PLACE","2ND PLACE","3RD PLACE"}[i]:(i+1)+" МЕСТО";PrizeGems[i].text=new[]{"10 000","7 000","5 000"}[i];PrizeCards[i].text=(5-i).ToString();}
            string[] columns=en?new[]{"RANK","PLAYER","POWER","REWARD"}:new[]{"МЕСТО","ИГРОК","МОЩЬ","НАГРАДА"};
            for(int i=0;i<4;i++){ColumnLabels[i].text=columns[i];ColumnLabels[i].gameObject.SetActive(true);}
            // Reference examples only. All periods preview the same illustration until live data exists.
            string[] names=en?new[]{"Masha","Ilya Muromets","Firebird","Vasilisa","Bayun the Cat","Koschei","Dobrynya","Baba Yaga","Alyosha Popovich","Zmey Gorynych","Ivan","Nightingale the Robber","Vodyanoy","Leshy"}:new[]{"Маша","Илья Муромец","Жар-Птица","Василиса","Кот Баюн","Кощей","Добрыня","Баба-Яга","Алёша Попович","Змей Горыныч","Иван","Соловей-Разбойник","Водяной","Леший"};
            int[] ranks={1,2,3,4,5,6,7,8,9,10,12,13,14,20};int[] power={1985300,1742850,1512640,1268400,1135780,1023560,987450,856270,742180,658920,512430,498760,472310,352140};
            for(int i=0;i<14;i++){Rows[i].gameObject.SetActive(true);Ranks[i].text=ranks[i].ToString();Names[i].text=names[i];Scores[i].text=Balance(power[i]);}
            RulesButtonText.text=en?"RANKING RULES":"ПРАВИЛА РЕЙТИНГА";RulesHeading.text=RulesButtonText.text;
            RulesDescription.text=en?"This is a preview of the ranking screen.\n\nDay, Week and Season currently show the same sample standings. Positions, power and prizes are examples.\n\nScoring rules, season dates and actual rewards will be announced before launch.":"Это предварительный просмотр рейтинга.\n\nДень, неделя и сезон пока показывают один пример таблицы. Места, мощь и призы приведены для образца.\n\nПравила подсчёта, даты сезона и настоящие награды будут объявлены перед стартом.";
            CloseRulesText.text=en?"CLOSE":"ЗАКРЫТЬ";EmptyPanel.SetActive(false);RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;var save=GameState.Instance!=null?GameState.Instance.Save:null;
            GoldText.text=save==null?"—":Balance(save.gold);GemsText.text=save==null?"—":Balance(save.gems);EnergyText.text=save==null?"—":save.energy+"/"+save.energyMax;
            LevelText.text=(DemoFeatureState.LanguageCode=="en"?"Level ":"Уровень ")+(save==null?"—":save.level.ToString());
        }
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
