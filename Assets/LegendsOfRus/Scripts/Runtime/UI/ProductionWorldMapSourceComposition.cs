using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [Serializable] public sealed class WorldMapChapterCard
    {
        public int Chapter;
        public string RussianName, EnglishName;
        public RectTransform Root;
        public Button Button;
        public Image Terrain, Surface, Frame, Lock, ProgressPlate, ContinueArt;
        public Graphic BorderGlow;
        public Text Heading, Name, Status, Progress, ContinueLabel;
        public bool Available, Completed, Current;
    }
    [Serializable] sealed class WorldMapChapterNames { public WorldMapChapterName[] chapters; }
    [Serializable] sealed class WorldMapChapterName { public int id; public string ru, en; }

    /// <summary>Approved original UI sprites, native scrolling and real chapter progress.</summary>
    public static class ProductionWorldMapSourceComposition
    {
        public const string ArtRoot="ProductionUI/WorldMapReferenceAligned/";
        static readonly Color Gold=new Color32(239,207,143,255);
        static Font Body=>Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
        static Font Numbers=>Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
        public static Sprite Load(string name)=>Resources.Load<Sprite>(ArtRoot+name) ?? throw new InvalidOperationException("Missing World Map source art: "+name);
        static Sprite Home(string name)=>Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+name);
        public static void Apply(ProductionWorldMapView v)
        {
            if(v.SourceComposition)
            {
                ProductionPlayerHeader.Attach(v);
                foreach(var card in v.Cards)
                {
                    if(card.BorderGlow is WorldMapPanelBorderHighlight)
                    {
                        if(card.BorderGlow.GetComponent<CanvasRenderer>()==null)card.BorderGlow.gameObject.AddComponent<CanvasRenderer>();
                        continue;
                    }
                    // Preserve the old serialized artwork record, but retire its alpha-derived glow.
                    card.BorderGlow.gameObject.SetActive(false);
                    card.BorderGlow=PanelBorder(card.Root);
                    card.BorderGlow.transform.SetSiblingIndex(card.Frame.transform.GetSiblingIndex()+1);
                }
                v.Notice=ProductionMapNotice.Install(v.Content);
                ProductionMapButtonFeedback.Install(v.transform);
                v.Revision=ProductionWorldMapView.CurrentRevision;
                return;
            }
            // Preserve existing serialized records and task dialog; retire only the obsolete presentation.
            foreach(Transform child in v.Content) if(child.gameObject!=v.TaskPanel)child.gameObject.SetActive(false);
            var stage=Rect(v.Content,"SourceComposition",0,0,941,1672);
            v.Environment=Art(stage,"Environment",Load("reference_environment"),0,0,941,1672);
            v.HudBacking=Rect(stage,"HeaderBacking",0,0,941,140);
            v.Hud=Rect(stage,"SharedHomeHeader",0,0,941,140);
            ProductionPlayerHeader.Attach(v);
            v.TitleArea=Rect(stage,"ChapterTitle",154,102,644,205);
            v.TitlePlaque=Art(v.TitleArea,"OriginalLettering",Load("chapter_title"),0,0,644,205);
            v.TitleText=Label(v.TitleArea,"LocalizedTitle",68,88,508,89,49);
            v.MapArea=Rect(stage,"ChapterViewport",92,313,758,1006);
            var viewportHit=v.MapArea.gameObject.AddComponent<Image>();viewportHit.color=Color.clear;
            v.MapArea.gameObject.AddComponent<RectMask2D>();
            v.ChapterScroll=v.MapArea.gameObject.AddComponent<ScrollRect>();v.ChapterScroll.viewport=v.MapArea;
            v.ChapterScroll.horizontal=false;v.ChapterScroll.vertical=true;v.ChapterScroll.movementType=ScrollRect.MovementType.Clamped;
            v.ChapterScroll.scrollSensitivity=50;v.ChapterScroll.decelerationRate=.10f;
            var content=Rect(v.MapArea,"TwentyChapterCards",0,0,758,4820);v.ChapterScroll.content=content;
            var names=JsonUtility.FromJson<WorldMapChapterNames>(Resources.Load<TextAsset>(ArtRoot+"chapters").text);
            if(names.chapters==null||names.chapters.Length!=20)throw new InvalidOperationException("Expected the confirmed twenty chapters.");
            v.Cards=new WorldMapChapterCard[20];
            for(int id=20;id>=1;id--)
            {
                var n=names.chapters[id-1];var c=new WorldMapChapterCard{Chapter=id,RussianName=n.ru,EnglishName=n.en};
                c.Button=Hit(content,"Chapter_"+id,0,(20-id)*239,758,230);c.Root=(RectTransform)c.Button.transform;
                c.Surface=Art(c.Root,"OpaqueNavySurface",Load("card_surface"),12,10,734,208);
                c.Terrain=Art(c.Root,"AcceptedChapterPainting",Load("region_"+id.ToString("00")),18,15,445,199);
                c.Frame=Art(c.Root,"OriginalDecorativeBorder",Load("card_frame"),0,0,758,230);
                c.BorderGlow=PanelBorder(c.Root);
                c.Heading=Label(c.Root,"ChapterNumber",391,20,339,32,25);
                c.Name=Label(c.Root,"ChapterName",389,52,341,67,30);
                c.Name.resizeTextForBestFit=true;c.Name.resizeTextMinSize=42;c.Name.resizeTextMaxSize=60;
                c.Status=Label(c.Root,"Availability",391,119,339,28,19);
                c.ProgressPlate=Art(c.Root,"StarProgressPlaque",Load("progress_plate"),469,151,183,57);
                c.Progress=Label(c.ProgressPlate.transform,"SavedStars",18,7,105,43,30);c.Progress.font=Numbers;
                c.Lock=Art(c.Root,"LockedChapter",Load("lock_source"),325,111,109,119);
                c.ContinueArt=Art(c.Root,"Continue",Load("continue_source"),431,193,259,72);
                c.ContinueLabel=Label(c.ContinueArt.transform,"LocalizedContinue",38,18,185,38,24);
                v.Cards[id-1]=c;
            }
            v.Footer=Rect(stage,"ReferenceNavigation",0,1436,941,236);
            v.FooterOrnament=Art(v.Footer,"CarvedBacking",Load("footer_backing"),0,0,941,236);
            Art(v.Footer,"HomeLowerCarving",Home("source_footer_carving"),0,18,941,218);
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];
            string[] nav={"map","tasks","guild","home","shop"};float[] navX={192,377,565,0,751},navW={185,188,186,192,190};
            for(int i=0;i<5;i++)
            {
                v.NavButtons[i]=Control(v.Footer,nav[i],Load("nav_"+nav[i]),navX[i],0,navW[i],236);
                v.NavLabels[i]=Label(v.NavButtons[i].transform,"LocalizedCaption",26,170,navW[i]-48,37,23);
            }
            v.BackButton=Control(stage,"BottomBack",Load("back_source"),0,1334,208,91);
            v.BackLabel=Label(v.BackButton.transform,"LocalizedBack",41,26,135,39,25);
            v.ChapterAction=Control(stage,"OpenCurrentChapter",Load("map_button_source"),220,1318,495,128);
            v.ChapterActionLabel=Label(v.ChapterAction.transform,"LocalizedOpen",100,31,295,68,43);
            v.TaskPanel.transform.SetAsLastSibling();
            v.Notice=ProductionMapNotice.Install(v.Content);
                ProductionMapButtonFeedback.Install(v.transform);
                v.Revision=ProductionWorldMapView.CurrentRevision;
            RefreshLanguage(v);RefreshState(v);
            v.ChapterScroll.verticalNormalizedPosition=0;
        }
        public static void RefreshLanguage(ProductionWorldMapView v)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            v.TitleText.rectTransform.anchoredPosition=new Vector2(68,-82);
            v.BackLabel.rectTransform.anchoredPosition=new Vector2(41,-32);
            v.ChapterActionLabel.rectTransform.anchoredPosition=new Vector2(100,-36);
            foreach(var label in v.NavLabels)label.rectTransform.anchoredPosition=new Vector2(26,-162);
            v.Header.RefreshValues();
            Localized(v.TitlePlaque,v.TitleText,"chapter_title",en,"CHAPTER SELECT");
            Localized((Image)v.BackButton.targetGraphic,v.BackLabel,"back_source",en,"BACK");
            Localized((Image)v.ChapterAction.targetGraphic,v.ChapterActionLabel,"map_button_source",en,"TO MAP");
            string[] keys={"map","tasks","guild","home","shop"},labels={"MAP","TASKS","GUILD","HOME","SHOP"};
            for(int i=0;i<5;i++)Localized((Image)v.NavButtons[i].targetGraphic,v.NavLabels[i],"nav_"+keys[i],en,labels[i]);
            foreach(var c in v.Cards)
            {
                c.Heading.text=(en?"CHAPTER ":"ГЛАВА ")+c.Chapter;
                c.Name.text=(en?c.EnglishName:c.RussianName).ToUpperInvariant();
                Localized(c.ContinueArt,c.ContinueLabel,"continue_source",en,"CONTINUE");
            }
            v.TaskHeading.text=en?"CHAPTER TASKS":"ЗАДАНИЯ ГЛАВЫ";v.TaskCloseText.text=en?"CLOSE":"ЗАКРЫТЬ";
            foreach(var t in v.TaskPanel.GetComponentsInChildren<Text>(true))t.font=Body;
        }
        public static void RefreshState(ProductionWorldMapView v)
        {
            bool en=DemoFeatureState.LanguageCode=="en";float y=0;
            bool saved=GameState.Instance!=null&&GameState.Instance.Save!=null;
            int current=0;
            foreach(var c in v.Cards)if(saved&&v.ChapterAvailable(c.Chapter)){current=c.Chapter;break;}
            for(int i=19;i>=0;i--)
            {
                var c=v.Cards[i];var summary=saved?GameState.Instance.GetChapterCompletionSummary(c.Chapter):null;
                c.Available=saved&&v.ChapterAvailable(c.Chapter);
                c.Completed=c.Available&&summary!=null&&summary.TotalLevels>0&&summary.CompletedLevels>=summary.TotalLevels;
                c.Current=c.Chapter==current;float height=c.Current?270:230;
                Place(c.Root,0,y,758,height);y+=height+9;
                Place(c.Surface.rectTransform,12,10,734,height-22);
                Place(c.Terrain.rectTransform,18,15,445,height-31);
                Place(c.Frame.rectTransform,0,0,758,height);Place(c.BorderGlow.rectTransform,0,0,758,height);
                Place(c.Lock.rectTransform,325,height-119,109,119);
                // A geometric panel perimeter highlights selection; extracted ornament alpha is not used.
                c.Surface.color=c.Frame.color=Color.white;
                c.Terrain.sprite=Load("region_"+c.Chapter.ToString("00")+(c.Current?"_current":""));
                c.Terrain.color=c.Available?Color.white:new Color(.48f,.48f,.48f,1);
                c.BorderGlow.gameObject.SetActive(c.Current);c.Lock.gameObject.SetActive(!c.Available);
                c.ContinueArt.gameObject.SetActive(c.Current);
                c.Status.gameObject.SetActive(!c.Current);
                c.Status.text=c.Completed?(en?"Completed":"Пройдено"):c.Available?(en?"Available":"Доступно"):(en?"Not yet available":"Пока недоступно");
                c.Progress.text=c.Available&&summary!=null?summary.EarnedStars+"/"+summary.MaximumStars:"—";
                c.ProgressPlate.color=c.Available?Color.white:new Color(.55f,.55f,.55f,1);
                c.Progress.color=c.Available?Gold:new Color32(150,145,122,255);
                // Current card uses the description slot for the star count and an independent action face below.
                Place(c.ProgressPlate.rectTransform,469,c.Current?130:151,183,57);
            }
            v.ChapterScroll.content.sizeDelta=new Vector2(758,y+10);
            v.ChapterAction.interactable=current!=0;
        }
        static WorldMapPanelBorderHighlight PanelBorder(Transform parent)
        {
            var border=Rect(parent,"CurrentPanelBorder",0,0,758,230).gameObject.AddComponent<WorldMapPanelBorderHighlight>();
            border.raycastTarget=false;
            return border;
        }
        static void Localized(Image image,Text label,string key,bool en,string text)
        {image.sprite=Load(key+(en?"_blank":""));label.text=text;label.enabled=en;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)=>ProductionWorldMapRuntime.Rect(p,n,x,y,w,h);
        static void Place(RectTransform r,float x,float y,float w,float h){r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        static Image Art(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {var im=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();im.sprite=sprite;im.raycastTarget=false;return im;}
        static Button Hit(Transform p,string n,float x,float y,float w,float h)
        {var im=Art(p,n,null,x,y,w,h);im.color=Color.clear;im.raycastTarget=true;var button=im.gameObject.AddComponent<Button>();button.targetGraphic=im;button.transition=Selectable.Transition.None;button.navigation=new Navigation{mode=Navigation.Mode.None};return button;}
        static Button Control(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {var b=Hit(p,n,x,y,w,h);var im=(Image)b.targetGraphic;im.sprite=sprite;im.color=Color.white;return b;}
        static Text Label(Transform p,string n,float x,float y,float w,float h,int size,TextAnchor align=TextAnchor.MiddleCenter)
        {var t=Rect(p,n,x,y,w*2,h*2).gameObject.AddComponent<Text>();t.rectTransform.localScale=Vector3.one*.5f;t.font=Body;t.fontSize=size*2;t.color=Gold;t.alignment=align;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;}
    }
}
