using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionPauseView : MonoBehaviour
    {
        public Func<BoosterType,int> CampaignCountProvider;
        public const int CurrentRevision = 2;
        public int Revision = CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content, Body;
        public Image Shade, TitleArt;
        public Text Title, GoalsHeading, Instructions, BoostersHeading;
        public PreLevelGoalView[] Goals;
        public Image[] BoosterArt;
        public Text[] BoosterCounts, ActionLabels;
        public Button[] Actions;
        public Match3Board Board { get; private set; }
        public bool ActionsBound { get; private set; }
        public bool HasRequiredBindings => Revision == CurrentRevision && Typography != null && Content != null && Body != null &&
            Shade != null && TitleArt != null && Title != null && GoalsHeading != null && Instructions != null && BoostersHeading != null &&
            Goals != null && Goals.Length == 3 && Array.TrueForAll(Goals, g => g != null && g.Root != null && g.Icon != null && g.Count != null) &&
            Valid(BoosterArt,4) && Valid(BoosterCounts,4) && Valid(Actions,4) && Valid(ActionLabels,4);
        static bool Valid<T>(T[] array,int count) where T:UnityEngine.Object => array != null && array.Length == count && Array.TrueForAll(array,x=>x!=null);
        readonly UnityAction[] listeners = new UnityAction[4];
        string language;
        bool committed;
        void OnEnable() { RefreshContent(); }
        void Update() { if (language != DemoFeatureState.LanguageCode) RefreshContent(); }
        void OnDisable() { Unbind(); }
        public void Bind(Match3Board board, Action resume, Action restart, Action settings, Action exit)
        {
            Unbind(); Board = board; committed = false;
            Action[] callbacks = {resume,restart,settings,exit};
            for (int i=0;i<4;i++)
            {
                int index=i;
                listeners[i]=()=>
                {
                    if(committed || !isActiveAndEnabled || !Actions[index].IsInteractable() ||
                        ScreenRouter.Instance != null && ScreenRouter.Instance.IsModalTransitioning) return;
                    committed=true;callbacks[index]?.Invoke();
                };
                Actions[i].onClick.AddListener(listeners[i]);
            }
            ActionsBound=true;RefreshContent();
        }
        void Unbind()
        {
            if(Actions!=null)for(int i=0;i<Math.Min(4,Actions.Length);i++)
                if(Actions[i]!=null && listeners[i]!=null){Actions[i].onClick.RemoveListener(listeners[i]);listeners[i]=null;}
            ActionsBound=false;Board=null;
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";
            string fontSource;Font font=Application.isPlaying?Typography.Resolve(false,out fontSource):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true)){t.font=font;t.raycastTarget=false;}
            Title.text=en?"PAUSED":"ПАУЗА";Title.enabled=en;
            TitleArt.sprite=ProductionPauseRuntime.Load("title_"+(en?"blank":"ru"));
            GoalsHeading.text=en?"LEVEL GOALS":"ЦЕЛИ УРОВНЯ";
            Instructions.text=en?"Complete every goal within\nthe available moves!":"Собери все цели за ограниченное\nколичество ходов!";
            BoostersHeading.text=en?"BOOSTERS":"БУСТЕРЫ";
            string[] labels=en?new[]{"CONTINUE","RESTART","SETTINGS","EXIT"}:new[]{"ПРОДОЛЖИТЬ","ПЕРЕЗАПУСК","НАСТРОЙКИ","ВЫЙТИ"};
            for(int i=0;i<4;i++)
            {
                ActionLabels[i].text=labels[i];ActionLabels[i].enabled=en;
                Actions[i].GetComponent<Image>().sprite=ProductionPauseRuntime.Load(ProductionPauseRuntime.ActionNames[i]+(en?"_blank":"_ru"));
                BoosterCounts[i].text=CampaignCountProvider!=null?CampaignCountProvider(ProductionPreLevelRuntime.Boosters[i]).ToString():GameState.Instance!=null?GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString():"—";
                Actions[i].interactable=Board!=null;
            }
            int count=Board!=null?Mathf.Min(3,Board.GoalCount):0;
            for(int i=0;i<3;i++)
            {
                var slot=Goals[i];slot.Root.gameObject.SetActive(i<count);if(i>=count)continue;
                var goal=Board.GetGoalSlot(i);
                slot.Root.anchoredPosition=new Vector2(310.5f-count*140*.5f+i*140,-267);
                slot.Icon.sprite=goal.Type==LevelGoalTypes.CollectTile?ProductionGameplayArt.Gem(goal.TileType):
                    goal.Type==LevelGoalTypes.ClearObstacle?ProductionGameplayArt.Obstacle(goal.ObstacleKind):
                    ProductionGameplayArt.Load(goal.Type==LevelGoalTypes.BringDown?"feather":goal.Type==LevelGoalTypes.Score?"goal_score_r5":"goal_survival_r5");
                slot.Count.text=goal.Remaining.ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ");
            }
            if(count==0)Instructions.text=en?"No active round":"Нет активного раунда";
            ProductionPauseRuntime.ApplyLocalizedArtwork(this,en,count>0);
        }
    }
}
