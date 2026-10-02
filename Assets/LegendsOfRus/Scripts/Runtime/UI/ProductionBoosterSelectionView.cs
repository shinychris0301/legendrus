using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionBoosterSelectionView:MonoBehaviour
    {
        public const int CurrentRevision=2;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body;
        public Image Environment,Shade,Panel,HeadingLettering;
        public Image[] PreviewIcons;
        public Text[] PreviewNames,PreviewLabels;
        public Text Title,Subtitle,Hint,PlayText;
        public Text[] Names,Effects,Counts,Availability;
        public Button Close,Play;
        public Button[] Tools;
        public bool ActionsBound{get;private set;}
        public int InspectedIndex{get;private set;}=-1;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Typography&&Content&&Body&&Environment&&Environment.sprite&&Shade&&Panel&&Panel.sprite&&HeadingLettering&&HeadingLettering.sprite&&Valid(PreviewIcons)&&Valid(PreviewNames)&&Valid(PreviewLabels)&&Title&&Subtitle&&Hint&&PlayText&&Close&&Close.image.sprite&&Play&&Play.image.sprite&&Valid(Tools)&&Valid(Names)&&Valid(Effects)&&Valid(Counts)&&Valid(Availability);
        static bool Valid<T>(T[] items)where T:UnityEngine.Object=>items!=null&&items.Length==4&&Array.TrueForAll(items,item=>item!=null);
        Action close;Func<bool> play;bool committed;string language;float nextRefresh;
        readonly UnityEngine.Events.UnityAction[] inspectors=new UnityEngine.Events.UnityAction[4];
        void OnEnable(){RefreshContent();}
        void OnDisable(){Unbind();}
        void Update(){if(language!=DemoFeatureState.LanguageCode)RefreshContent();if(Time.unscaledTime>=nextRefresh){nextRefresh=Time.unscaledTime+.25f;RefreshState();}}
        public void Bind(bool modal,Action onClose,Func<bool> onPlay)
        {
            Unbind();close=onClose;play=onPlay;committed=false;InspectedIndex=-1;
            Environment.gameObject.SetActive(!modal);Close.onClick.AddListener(CloseOnce);Play.onClick.AddListener(PlayOnce);
            for(int i=0;i<4;i++){int index=i;inspectors[i]=()=>Inspect(index);Tools[i].onClick.AddListener(inspectors[i]);}
            ActionsBound=true;RefreshContent();
        }
        void Unbind()
        {
            if(Close)Close.onClick.RemoveListener(CloseOnce);if(Play)Play.onClick.RemoveListener(PlayOnce);
            if(Valid(Tools))for(int i=0;i<4;i++)if(inspectors[i]!=null)Tools[i].onClick.RemoveListener(inspectors[i]);
            close=null;play=null;ActionsBound=false;
        }
        bool Allowed(Button b)=>ActionsBound&&!committed&&isActiveAndEnabled&&b.IsInteractable()&&
            !(Environment.gameObject.activeSelf&&ScreenRouter.Instance!=null&&ScreenRouter.Instance.HasActiveModalBlocker);
        void CloseOnce(){if(!Allowed(Close))return;committed=true;RefreshState();close?.Invoke();}
        void PlayOnce(){if(!Allowed(Play))return;committed=true;bool accepted=play!=null&&play();if(!accepted)committed=false;RefreshState();}
        public void Inspect(int index)
        {
            if(index<0||index>=4||!Allowed(Tools[index]))return;
            // Inspection is not equipping or consuming a tool, including empty inventory.
            InspectedIndex=index;RefreshState();
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var t in GetComponentsInChildren<Text>(true))t.font=font;
            Title.text=en?"BOOSTER SELECTION":"ВЫБОР БУСТЕРОВ";
            Subtitle.text=en?"Explore your tools before the level":"Ознакомьтесь с бустерами перед уровнем";
            PlayText.text=en?"PLAY":"ИГРАТЬ";
            ProductionBoosterSelectionRuntime.ApplySourceState(this,en);
            string[] names=en?new[]{"BOGATYR MACE","FIREBIRD FEATHER","LIVING LIGHT","SEA KING'S PEARL"}:new[]{"БОГАТЫРСКАЯ БУЛАВА","ПЕРО ЖАР-ПТИЦЫ","ЖИВОЙ СВЕТ","ЖЕМЧУЖИНА МОРСКОГО ЦАРЯ"};
            string[] effects=en?new[]{"Tap: clear a row.\nSwipe up/down:\nclear a column.","Clear a 3×3 area\naround a chosen cell.","Clear all gems\nof a chosen color.","Adds 5 moves\nto the current round."}:new[]{"Ряд — касание клетки.\nСтолбец — свайп\nвверх или вниз.","Очистить область\n3×3 вокруг\nвыбранной клетки.","Убрать самоцветы\nвыбранного цвета.","Добавляет 5 ходов\nв текущем раунде."};
            string[] previews=en?new[]{"LUCK SCROLL","LIGHTNING ORB","VELES BOMB","SWAP"}:new[]{"СВИТОК УДАЧИ","ШАР МОЛНИИ","БОМБА ВЕЛЕСА","ПЕРЕСТАНОВКА"};
            for(int i=0;i<4;i++)
            {
                Names[i].text=names[i];Effects[i].text=effects[i];
                PreviewNames[i].text=previews[i];PreviewLabels[i].text=en?"PREVIEW\nNot available yet":"ПРЕДПРОСМОТР\nПока недоступно";
            }
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;bool en=DemoFeatureState.LanguageCode=="en";
            Close.interactable=Play.interactable=ActionsBound&&!committed;
            for(int i=0;i<4;i++)
            {
                int count=GameState.Instance!=null?Mathf.Max(0,GameState.Instance.BoosterCount(ProductionPreLevelRuntime.Boosters[i])):0;
                Counts[i].text=count.ToString();Availability[i].text=count==0?(en?"NO STOCK":"НЕТ В ЗАПАСЕ"):(en?"IN STOCK":"В ЗАПАСЕ");
                Tools[i].interactable=ActionsBound&&!committed;Tools[i].image.color=count==0?new Color(.72f,.72f,.72f,1):Color.white;
                Availability[i].color=count==0?new Color32(190,169,141,255):new Color32(229,206,150,255);
                Names[i].color=i==InspectedIndex?new Color32(255,239,189,255):new Color32(245,211,147,255);
            }
            Hint.text=en?"Use boosters during the round.\nInspecting them does not spend your stock.":"Бустеры применяются во время игры.\nПросмотр не расходует запас.";
        }
    }
}
