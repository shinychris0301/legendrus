using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus
{
    [Serializable] public sealed class GameplayGoalView
    {
        public RectTransform Root;public Image Icon;public Text Description,Count;public ProductionGameplayGoalSymbol Symbol;
    }
    [DisallowMultipleComponent]
    public sealed class ProductionGameplayView : MonoBehaviour
    {
        public const int CurrentRevision=22;
        public int Revision=CurrentRevision;
        public bool SourceControls;
        public SplashTitleTypography Typography;
        public RectTransform Content,Hud,Footer,BoardArea,BoardMount;
        public Image Environment,GoalProgress,StarProgress;
        public Text HeroName,LevelText,MovesHeading,MovesValue,GoalsHeading,ScoreHeading,ScoreValue;
        public Button[] BoosterButtons;public Text[] BoosterCounts;
        public ProductionWorldMapGraphic[] Stars,ArmedMarkers;
        public GameplayGoalView[] Goals;
        public Button PauseButton;
        public ProductionGameplayStatus Status;
        public ProductionGameplayScoreFrame ScoreFrame;
        public ProductionGameplayScoreMeter ScoreMeter;
        public Match3Board Board{get;private set;}
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&SourceControls&&HasCoreBindings&&Array.TrueForAll(Stars,s=>s.StarEmptyArt!=null&&s.StarFilledArt!=null)&&HasFramePolishBindings&&Status!=null&&Status.HasRequiredBindings&&ScoreFrame!=null&&ScoreMeter!=null&&Array.TrueForAll(Goals,g=>g.Symbol!=null);
        internal bool HasCoreBindings=>Typography!=null&&Content!=null&&Hud!=null&&Footer!=null&&BoardArea!=null&&BoardMount!=null&&
            Environment!=null&&Environment.sprite!=null&&GoalProgress!=null&&StarProgress!=null&&HeroName!=null&&LevelText!=null&&MovesHeading!=null&&MovesValue!=null&&GoalsHeading!=null&&ScoreHeading!=null&&ScoreValue!=null&&
            Valid(BoosterButtons,4)&&Valid(BoosterCounts,4)&&Valid(Stars,3)&&Valid(ArmedMarkers,4)&&PauseButton!=null&&Goals!=null&&Goals.Length==3&&Array.TrueForAll(Goals,g=>g!=null&&g.Root!=null&&g.Icon!=null&&g.Count!=null&&g.Description!=null);
        public bool HasFramePolishBindings=>BoardArea!=null&&BoardArea.Find("BoardFrame/CarvedFrame")?.GetComponent<Image>()?.sprite!=null&&BoardArea.Find("BoardLacquer")!=null&&Hud!=null&&Hud.Find("GoalLacquer")!=null&&
            Footer!=null&&Footer.Find("ScoreInset")!=null&&Valid(Stars,3)&&Array.TrueForAll(Stars,s=>s.GetComponent<ProductionGameplayStarPolish>()!=null);
        static bool Valid<T>(T[] a,int length)where T:UnityEngine.Object=>a!=null&&a.Length==length&&Array.TrueForAll(a,x=>x!=null);
        string language;GameState state;Action pause;Func<BoosterType,int> campaignCount;
        readonly UnityAction[] boosterActions=new UnityAction[4];UnityAction pauseAction;
        void OnEnable(){RefreshContent();}
        void Update()
        {
            if(language!=DemoFeatureState.LanguageCode)RefreshContent();
            if(Board!=null)RefreshAvailability();
        }
        void OnDisable(){Unbind();}
        public void Bind(Match3Board board,Action showPause)
        {
            BindCore(board,showPause,null);
        }
        public void BindCampaign(Match3Board board,Action showPause,Func<BoosterType,int> count)
        { BindCore(board,showPause,count); }
        void BindCore(Match3Board board,Action showPause,Func<BoosterType,int> count)
        {
            Unbind();Board=board;pause=showPause;campaignCount=count;state=count==null?GameState.Instance:null;
            Board.MovesChanged+=OnValue;Board.ScoreChanged+=OnValue;Board.GoalSlotsChanged+=OnGoals;
            if(state!=null)state.Changed+=RefreshState;
            for(int i=0;i<4;i++)
            {
                int index=i;boosterActions[i]=()=>{if(Blocked()||!BoosterButtons[index].IsInteractable())return;BoosterButtons[index].GetComponent<ProductionHomeButtonFeedback>()?.NotifyActivated();Board.UseBooster(ProductionPreLevelRuntime.Boosters[index]);RefreshState();};
                BoosterButtons[i].onClick.AddListener(boosterActions[i]);
            }
            pauseAction=()=>{if(!Blocked()&&PauseButton.IsInteractable())pause?.Invoke();};PauseButton.onClick.AddListener(pauseAction);ActionsBound=true;RefreshContent();
        }
        bool Blocked()=>!isActiveAndEnabled||Board==null||ScreenRouter.Instance!=null&&ScreenRouter.Instance.HasActiveModalBlocker;
        void Unbind()
        {
            if(Board!=null){Board.MovesChanged-=OnValue;Board.ScoreChanged-=OnValue;Board.GoalSlotsChanged-=OnGoals;}
            if(state!=null)state.Changed-=RefreshState;
            if(BoosterButtons!=null)for(int i=0;i<4;i++)if(boosterActions[i]!=null&&BoosterButtons[i]!=null)BoosterButtons[i].onClick.RemoveListener(boosterActions[i]);
            if(PauseButton!=null&&pauseAction!=null)PauseButton.onClick.RemoveListener(pauseAction);
            Board=null;state=null;pause=null;campaignCount=null;ActionsBound=false;
        }
        void OnValue(int value)=>RefreshState();void OnGoals(int a,int b)=>RefreshState();
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            HeroName.text=en?"Ivan":"Иван";MovesHeading.text=en?"MOVES":"ХОДЫ";GoalsHeading.text=en?"GOALS":"ЦЕЛИ";ScoreHeading.text=en?"SCORE":"СЧЁТ";
            string source;Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true)){t.font=font;t.raycastTarget=false;}
            string[] ru={"БОГАТЫРСКАЯ\nБУЛАВА","ПЕРО\nЖАР-ПТИЦЫ","ЖИВОЙ СВЕТ","ЖЕМЧУЖИНА\nМОРСКОГО ЦАРЯ"};
            string[] english={"BOGATYR\nMACE","FIREBIRD\nFEATHER","LIVING LIGHT","SEA KING'S\nPEARL"};
            for(int i=0;i<4;i++)Footer.Find("BoosterCard_"+i+"/BoosterLabel").GetComponent<Text>().text=en?english[i]:ru[i];
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;bool en=DemoFeatureState.LanguageCode=="en";
            LevelData level=Board!=null?Board.CurrentLevel:ChapterLevelCatalog.ResolveSelected(null);
            LevelText.text=(en?"LEVEL ":"УРОВЕНЬ ")+level.levelId;
            MovesValue.text=Board!=null?Board.Moves.ToString():"—";ScoreValue.text=Board!=null?Balance(Board.Score):"—";
            ProductionGameplayHudStyle.LayoutGoals(this,level.goals.Length);
            float progress=0;int slots=0;
            for(int i=0;i<Goals.Length;i++)
            {
                var slot=Goals[i];var goal=Board!=null?Board.GetGoalSlot(i):null;slot.Root.gameObject.SetActive(goal!=null);if(goal==null)continue;
                bool tile=goal.Type==LevelGoalTypes.CollectTile;Sprite icon=tile?ProductionGameplayArt.Gem(goal.TileType):goal.Type==LevelGoalTypes.ClearObstacle?ProductionGameplayArt.Obstacle(goal.ObstacleKind):goal.Type==LevelGoalTypes.BringDown?ProductionGameplayArt.Load("feather"):ProductionGameplayArt.Load(goal.Type==LevelGoalTypes.Score?"goal_score_r5":"goal_survival_r5");
                slot.Icon.sprite=icon;
                if(Board!=null&&Board.RuleVersion!=GameplayRuleVersion.Legacy&&goal.Type==LevelGoalTypes.ClearObstacle)
                    PrototypeObstacleArt.Apply(slot.Icon,goal.ObstacleKind,2,false);
                else
                {
                    var damage=slot.Icon.transform.Find("DamageFissures");if(damage)damage.gameObject.SetActive(false);
                    RunicSealGraphic.Refresh(slot.Icon,goal.Type==LevelGoalTypes.ClearObstacle?goal.ObstacleKind:BoardObstacleKind.None);
                }
                slot.Count.text=goal.IsComplete?"✓":Balance(goal.Remaining);
                slot.Description.text=tile||goal.Type==LevelGoalTypes.ClearObstacle||goal.Type==LevelGoalTypes.BringDown?"":goal.Type==LevelGoalTypes.Score?(en?"SCORE":"СЧЁТ"):(en?"MOVES":"ХОДОВ");
                slot.Description.enabled=false;
                // Retain the old serialized graphic binding; the complete painted icon replaces its geometry.
                slot.Symbol.enabled=false;
                bool survival=goal.Type==LevelGoalTypes.Survival;
                if(slot.Symbol.Survival!=survival){slot.Symbol.Survival=survival;slot.Symbol.SetVerticesDirty();}
                slot.Count.rectTransform.anchoredPosition=new Vector2(0,-65);
                slot.Count.rectTransform.sizeDelta=new Vector2(196,66);
                progress+=Mathf.Clamp01(goal.Current/(float)goal.Target);slots++;
            }
            GoalProgress.fillAmount=slots>0?progress/slots:0;
            int[] thresholds=level.starScoreThresholds;int score=Board!=null?Board.Score:0;
            StarProgress.fillAmount=thresholds!=null&&thresholds.Length==3?Mathf.Clamp01(score/(float)Mathf.Max(1,thresholds[2])):0;
            ScoreMeter.SetFraction(StarProgress.fillAmount);
            // Live progress shows earned thresholds. The guaranteed completion reward
            // remains in EvaluateStars and is awarded only when the level is completed.
            int stars=0;
            if(Board!=null&&thresholds!=null)foreach(int threshold in thresholds)if(score>=threshold)stars++;
            for(int i=0;i<3;i++){bool dim=i>=stars;if(Stars[i].Dimmed!=dim){Stars[i].Dimmed=dim;Stars[i].SetAllDirty();}}
            for(int i=0;i<4;i++)BoosterCounts[i].text=Balance(Count(ProductionPreLevelRuntime.Boosters[i]));
            RefreshAvailability();
        }
        void RefreshAvailability()
        {
            bool can=Board!=null&&Board.CanAcceptPointerInput&&!Blocked();
            for(int i=0;i<4;i++)
            {
                var booster=ProductionPreLevelRuntime.Boosters[i];BoosterButtons[i].interactable=can&&Count(booster)>0;
                bool armed=Board!=null&&Board.IsBoosterArmed&&Board.ArmedBooster==booster;
                if(ArmedMarkers[i].gameObject.activeSelf!=armed)ArmedMarkers[i].gameObject.SetActive(armed);
            }
            PauseButton.interactable=Board!=null&&!Blocked();
        }
        int Count(BoosterType type)=>campaignCount!=null?campaignCount(type):state!=null?state.BoosterCount(type):0;
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
    }
}
