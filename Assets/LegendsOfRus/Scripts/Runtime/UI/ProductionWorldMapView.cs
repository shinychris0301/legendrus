using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [Serializable] public sealed class WorldMapNode
    {
        public RectTransform Root;
        public Button Button;
        public ProductionWorldMapGraphic Face, Halo, Lock;
        public Text Number;
        public ProductionWorldMapGraphic[] Stars;
    }

    [DisallowMultipleComponent]
    public sealed class ProductionWorldMapView : MonoBehaviour
    {
        public const int CurrentRevision = 6;
        public int Revision = CurrentRevision;
        public ProductionMapNotice Notice;
        public ProductionPlayerHeader Header;
        public SplashTitleTypography Typography;
        public RectTransform Content, Hud, HudBacking, MapArea, TitleArea, Footer;
        public Image Environment, TitlePlaque, FooterOrnament;
        public ProductionWorldMapGraphic Path;
        public Text HeroName, LevelText, GoldText, GemsText, EnergyText, TitleText, TaskHeading, TaskCopy, TaskCloseText;
        public Button ProfileButton, SettingsButton, BackButton, TaskClose;
        public Button[] PlusButtons, NavButtons;
        public Text[] NavLabels;
        public WorldMapNode[] Nodes;
        public GameObject TaskPanel;
        public WorldMapChapterCard[] Cards;
        public ScrollRect ChapterScroll;
        public Button ChapterAction;
        public Text BackLabel, ChapterActionLabel;
        public bool SourceComposition => Cards != null && Cards.Length == 20 && ChapterScroll != null && ChapterAction != null;
        public bool ActionsBound { get; private set; }
        public bool HasRequiredBindings => Revision == CurrentRevision && Notice && Notice.HasBindings && Header!=null && Header.HasBindings && SourceComposition && Array.TrueForAll(Cards,c=>c!=null&&c.Root!=null&&c.Button!=null&&c.Frame!=null&&c.BorderGlow is WorldMapPanelBorderHighlight&&c.BorderGlow.GetComponent<CanvasRenderer>()!=null&&c.Terrain!=null&&c.Surface!=null&&c.Name!=null&&c.Progress!=null&&c.Lock!=null&&c.ContinueArt!=null) && Typography != null && Content != null &&
            Hud != null && HudBacking != null && MapArea != null && TitleArea != null && Footer != null &&
            Environment != null && Environment.sprite != null && TitlePlaque != null && TitlePlaque.sprite != null &&
            FooterOrnament != null && FooterOrnament.sprite != null && Path != null && TitleText != null &&
            HeroName != null && LevelText != null && GoldText != null && GemsText != null && EnergyText != null &&
            ProfileButton != null && SettingsButton != null && BackButton != null && TaskClose != null &&
            TaskHeading != null && TaskCopy != null && TaskCloseText != null && TaskPanel != null &&
            PlusButtons != null && PlusButtons.Length == 3 && Array.TrueForAll(PlusButtons,x=>x!=null) &&
            NavButtons != null && NavButtons.Length == 5 && Array.TrueForAll(NavButtons,x=>x!=null) &&
            NavLabels != null && NavLabels.Length == 5 && Array.TrueForAll(NavLabels,x=>x!=null) &&
            Nodes != null && Nodes.Length == 15 && Array.TrueForAll(Nodes,x=>x!=null && x.Root!=null &&
                x.Button!=null && x.Face!=null && x.Halo!=null && x.Lock!=null && x.Number!=null &&
                x.Stars!=null && x.Stars.Length==3 && Array.TrueForAll(x.Stars,s=>s!=null));

        GameState subscribed;
        string language;
        bool committed;
        readonly List<Button> bound = new List<Button>();
        readonly List<UnityAction> listeners = new List<UnityAction>();

        void OnEnable() { committed=false; BindState(); RefreshContent(); if(SourceComposition)ChapterScroll.verticalNormalizedPosition=0; }
        void Update()
        {
            if (subscribed != GameState.Instance) { BindState(); RefreshState(); }
            if (language != DemoFeatureState.LanguageCode) RefreshContent();
        }
        void OnDisable()
        {
            Unbind(); if (subscribed != null) subscribed.Changed -= RefreshState; subscribed=null;
            if (TaskPanel != null) TaskPanel.SetActive(false);
        }
        void BindState()
        {
            if (subscribed != null) subscribed.Changed -= RefreshState;
            subscribed=GameState.Instance;
            if (isActiveAndEnabled && subscribed!=null) subscribed.Changed+=RefreshState;
        }
        public void RefreshContent()
        {
            ProductionNavigationStyle.Apply(this);
            if (!HasRequiredBindings) return;
            language=DemoFeatureState.LanguageCode; bool en=language=="en";
            if(SourceComposition){ProductionWorldMapSourceComposition.RefreshLanguage(this);RefreshState();return;}
            HeroName.text=en?"Ivan":"Иван"; TitleText.text=en?"WORLD MAP":"КАРТА МИРА";
            string[] labels=en?new[]{"MAP","TASKS","GUILD"}:new[]{"КАРТА","ЗАДАНИЯ","ГИЛЬДИЯ"};
            for(int i=0;i<3;i++) NavLabels[i].text=labels[i];
            TaskHeading.text=en?"CHAPTER TASKS":"ЗАДАНИЯ ГЛАВЫ";TaskCloseText.text=en?"CLOSE":"ЗАКРЫТЬ";
            string source; Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(Text text in GetComponentsInChildren<Text>(true)) { text.font=font; text.raycastTarget=false; }
            RefreshState();
        }
        public bool ChapterAvailable(int chapter)
        {
            // Only Chapter 1 has authored data. Completion must not unlock missing content.
            if (chapter != 1) return false;
            ChapterLevelCatalogData catalog=ChapterLevelCatalog.LoadChapter1();
            return chapter==1 && catalog!=null && catalog.chapterId==chapter && catalog.levels!=null && catalog.levels.Length>0;
        }
        public void RefreshState()
        {
            if (!HasRequiredBindings) return;
            PlayerSaveData save=GameState.Instance!=null?GameState.Instance.Save:null;
            bool en=DemoFeatureState.LanguageCode=="en";
            Header.RefreshValues();
            ChapterCompletionSummary summary=GameState.Instance!=null?GameState.Instance.GetChapterCompletionSummary(1):null;
            int stars=summary!=null&&summary.MaximumStars>0?Mathf.Clamp(Mathf.FloorToInt(3f*summary.EarnedStars/summary.MaximumStars),0,3):0;
            for(int i=0;i<Nodes.Length;i++)
            {
                WorldMapNode node=Nodes[i];bool available=save!=null&&ChapterAvailable(i+1);
                node.Face.Dimmed=!available;node.Face.SetVerticesDirty();
                node.Number.text=(i+1).ToString();node.Number.color=available?new Color32(245,215,155,255):new Color32(165,158,137,255);
                node.Halo.gameObject.SetActive(available);node.Lock.gameObject.SetActive(!available);
                for(int j=0;j<3;j++) { node.Stars[j].Dimmed=!available||j>=stars;node.Stars[j].SetVerticesDirty(); }
            }
            if(summary!=null)
                TaskCopy.text=en?"Chapter 1 · Native Village\nLevels completed: "+summary.CompletedLevels+" / "+summary.TotalLevels+
                    "\nStars earned: "+summary.EarnedStars+" / "+summary.MaximumStars:
                    "Глава 1 · Родная деревня\nПройдено уровней: "+summary.CompletedLevels+" / "+summary.TotalLevels+
                    "\nСобрано звёзд: "+summary.EarnedStars+" / "+summary.MaximumStars;
            else TaskCopy.text=en?"Progress unavailable":"Прогресс недоступен";
            if(SourceComposition)ProductionWorldMapSourceComposition.RefreshState(this);
        }

        public void BindNavigation(Action<ScreenId> show,Action openChapter,Action back,Action<string> notify)
        {
            Unbind();committed=false;
            Bind(ProfileButton,()=>show(ScreenId.Profile),true);Bind(SettingsButton,()=>show(ScreenId.Settings),true);
            Bind(BackButton,()=> { if(TaskPanel.activeSelf)TaskPanel.SetActive(false);else back(); },false);
            for(int i=0;i<3;i++) { int resource=i;Bind(PlusButtons[i],()=>show(resource==2?ScreenId.OutOfEnergy:ScreenId.Shop),true); }
            Bind(NavButtons[0],()=> { TaskPanel.SetActive(false);RefreshState(); },false);
            Bind(NavButtons[1],()=> { RefreshState();TaskPanel.SetActive(true);TaskPanel.transform.SetAsLastSibling(); },false);
            Bind(NavButtons[2],()=>show(ScreenId.TeamHub),true);
            Bind(TaskClose,()=>TaskPanel.SetActive(false),false);
            if(SourceComposition)
            {
                Bind(NavButtons[3],()=>show(ScreenId.Home),true);
                Bind(NavButtons[4],()=>show(ScreenId.Shop),true);
                Bind(ChapterAction,()=>{if(ChapterAvailable(1)){committed=true;openChapter();}},false);
                foreach(var card in Cards)
                {
                    int id=card.Chapter;
                    Bind(card.Button,()=>{
                        if(!ChapterAvailable(id)){notify(DemoFeatureState.LanguageCode=="en"?"Chapter "+id+" is not yet available.":"Глава "+id+" пока недоступна.");return;}
                        committed=true;openChapter();
                    },false);
                }
                ActionsBound=true;return;
            }
            for(int i=0;i<Nodes.Length;i++)
            {
                int chapter=i+1;
                Bind(Nodes[i].Button,()=>
                {
                    if(!ChapterAvailable(chapter)) { notify(DemoFeatureState.LanguageCode=="en"?"Chapter "+chapter+" is locked.":"Глава "+chapter+" пока закрыта.");return; }
                    committed=true;openChapter();
                },false);
            }
            ActionsBound=true;
        }
        void Bind(Button button,Action action,bool route)
        {
            UnityAction listener=()=>
            {
                if(committed||!isActiveAndEnabled||!button.IsInteractable())return;
                if(route)committed=true;
                button.GetComponent<ProductionHomeButtonFeedback>()?.NotifyActivated();action();
            };
            bound.Add(button);listeners.Add(listener);button.onClick.AddListener(listener);
        }
        void Unbind()
        {
            for(int i=0;i<bound.Count;i++)if(bound[i]!=null)bound[i].onClick.RemoveListener(listeners[i]);
            bound.Clear();listeners.Clear();ActionsBound=false;
        }
    }
}
