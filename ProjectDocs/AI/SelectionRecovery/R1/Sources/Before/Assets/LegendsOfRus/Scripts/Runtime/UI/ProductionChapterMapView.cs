using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [Serializable] public sealed class ChapterMapNode
    {
        public int LevelId;
        public RectTransform Root;
        public Button Button;
        public Image Face;
        public ProductionWorldMapGraphic Halo, Lock;
        public Text Number;
        public ProductionWorldMapGraphic[] Stars;
        public Image[] SourceStars;
    }

    [DisallowMultipleComponent]
    public sealed class ProductionChapterMapView : MonoBehaviour
    {
        public const int CurrentRevision = 5;
        public int Revision = CurrentRevision;
        public ProductionMapNotice Notice;
        public ProductionPlayerHeader Header;
        public ProductionChapterMapScroller Scroller;
        public bool SourceControls;
        public Image SourcePlay, SourceBack, EnvironmentMargins;
        public SplashTitleTypography Typography;
        public RectTransform Content, Hud, HudBacking, MapArea, TitleArea, Footer;
        public Image Environment, TitlePlaque, FooterOrnament;
        public Text HeroName, LevelText, GoldText, GemsText, EnergyText, TitleText, ChapterText, PlayText;
        public Button ProfileButton, SettingsButton, BackButton, PlayButton;
        public Button[] PlusButtons;
        public ChapterMapNode[] Nodes;
        public bool ActionsBound { get; private set; }
        public bool HasRequiredBindings => Revision==CurrentRevision && Scroller!=null && Scroller.HasBindings && SourceControls && Header!=null && Header.HasBindings && SourcePlay!=null && SourceBack!=null && EnvironmentMargins!=null && Typography!=null && Content!=null &&
            Hud!=null && HudBacking!=null && MapArea!=null && TitleArea!=null && Footer!=null &&
            Environment!=null && Environment.sprite!=null && TitlePlaque!=null && TitlePlaque.sprite!=null &&
            FooterOrnament!=null && FooterOrnament.sprite!=null && TitleText!=null && ChapterText!=null && PlayText!=null &&
            HeroName!=null && LevelText!=null && GoldText!=null && GemsText!=null && EnergyText!=null &&
            ProfileButton!=null && SettingsButton!=null && BackButton!=null && PlayButton!=null &&
            PlusButtons!=null && PlusButtons.Length==3 && Array.TrueForAll(PlusButtons,x=>x!=null) &&
            Nodes!=null && Nodes.Length==ProductionChapterMapRuntime.NodePoolSize && Array.TrueForAll(Nodes,x=>x!=null &&
                x.Root!=null && x.Button!=null && x.Face!=null && x.Face.sprite!=null && x.Halo!=null && x.Lock!=null &&
                x.SourceStars!=null && x.SourceStars.Length==3 && Array.TrueForAll(x.SourceStars,t=>t!=null) && x.Number!=null && x.Stars!=null && x.Stars.Length==3 && Array.TrueForAll(x.Stars,t=>t!=null));

        GameState subscribed;
        string language;
        bool committed;
        readonly List<Button> bound = new List<Button>();
        readonly List<UnityAction> listeners = new List<UnityAction>();

        void OnEnable() { committed=false; BindState(); RefreshContent(); }
        void Update()
        {
            if (subscribed != GameState.Instance) { BindState(); RefreshState(); }
            if (language != DemoFeatureState.LanguageCode) RefreshContent();
        }
        void OnDisable()
        {
            Unbind(); if (subscribed != null) subscribed.Changed -= RefreshState; subscribed=null;
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
            if(SourceControls){ProductionChapterMapSourceControls.RefreshLanguage(this);RefreshState();return;}
            HeroName.text=en?"Ivan":"Иван";
            ChapterText.text=en?"CHAPTER 1":"ГЛАВА 1";
            TitleText.text=en?"NATIVE VILLAGE":"РОДНАЯ ДЕРЕВНЯ";
            PlayText.text=en?"PLAY":"ИГРАТЬ";
            string source; Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(Text text in GetComponentsInChildren<Text>(true)) { text.font=font; text.raycastTarget=false; }
            PlayText.font=Application.isPlaying?Typography.Resolve(true,out source):(Typography.StartFont!=null?Typography.StartFont:font);
            RefreshState();
        }
        public bool LevelAvailable(int level)
        {
            return GameState.Instance!=null && ChapterLevelCatalog.Find(1,level)!=null && GameState.Instance.IsLevelUnlocked(1,level);
        }
        public void RefreshState()
        {
            if (!HasRequiredBindings) return;
            PlayerSaveData save=GameState.Instance!=null?GameState.Instance.Save:null;
            bool en=DemoFeatureState.LanguageCode=="en";
            GoldText.text=save!=null?Balance(save.gold):"—";GemsText.text=save!=null?Balance(save.gems):"—";
            EnergyText.text=save!=null?save.energy+"/"+save.energyMax:"— / —";
            LevelText.text=(en?"Level ":"Уровень ")+(save!=null?save.level.ToString():"—");
            for(int i=0;i<Nodes.Length;i++)
            {
                ChapterMapNode node=Nodes[i];int level=node.LevelId;bool available=save!=null&&LevelAvailable(level);
                var progress=GameState.Instance!=null?GameState.Instance.GetLevelProgress(1,level):null;
                int stars=progress!=null?Mathf.Clamp(progress.bestStars,0,3):0;
                node.Face.color=available?Color.white:new Color(.76f,.77f,.79f,1);
                node.Number.text=level.ToString();node.Number.fontSize=level>=100?62:86;node.Number.color=available?new Color32(245,215,155,255):new Color32(174,165,144,255);
                node.Halo.gameObject.SetActive(available&&save.selectedLevel==level);
                node.Lock.gameObject.SetActive(!available);
                for(int j=0;j<3;j++){node.Stars[j].Dimmed=j>=stars;node.Stars[j].SetVerticesDirty();}
            }
            PlayButton.interactable=save!=null&&LevelAvailable(save.selectedLevel);
            if(SourceControls){Header.RefreshValues();ProductionChapterMapSourceControls.RefreshStars(this);Scroller.RefreshLabels();}
        }
        static string Balance(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");

        public void BindNavigation(Action<ScreenId> show,Action openSelectedLevel,Action back,Action<string> notify)
        {
            Unbind();committed=false;
            Bind(ProfileButton,()=>show(ScreenId.Profile),true);Bind(SettingsButton,()=>show(ScreenId.Settings),true);
            Bind(BackButton,back,true);
            Bind(PlayButton,openSelectedLevel,true);
            Bind(Scroller.CurrentButton,Scroller.JumpToCurrent,false);
            for(int i=0;i<3;i++){int resource=i;Bind(PlusButtons[i],()=>show(resource==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            for(int i=0;i<Nodes.Length;i++)
            {
                var node=Nodes[i];
                Bind(node.Button,()=>
                {
                    if(Scroller.IsDragging)return;
                    int level=node.LevelId;
                    if(!LevelAvailable(level))
                    {
                        bool published=ChapterLevelCatalog.Find(1,level)!=null;
                        notify(published?(DemoFeatureState.LanguageCode=="en"?"Complete the previous level to unlock this one.":"Пройдите предыдущий уровень, чтобы открыть этот."):(DemoFeatureState.LanguageCode=="en"?"This level is coming soon.":"Этот уровень скоро появится."));return;
                    }
                    GameState.Instance.SelectLevel(1,level);
                    committed=true;openSelectedLevel();
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
