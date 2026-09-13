using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Native gameplay shell; ScreenRouter supplies the existing live board and outcome callbacks.</summary>
    public static class ProductionGameplayRuntime
    {
        public const string ContentName="GAMEPLAY_ALIGNED_CONTENT";
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionGameplayView>();
            if(existing!=null&&existing.HasCoreBindings){AlignPresentation(existing);ApplyFramePolish(existing);existing.RefreshContent();return;}
            if(root.Find(ContentName)!=null)throw new InvalidOperationException("Gameplay bindings need repair outside Play Mode.");
            var v=existing??root.gameObject.AddComponent<ProductionGameplayView>();
            v.Typography=Resources.Load<SplashTitleTypography>(ProductionHomeRuntime.TypographyResource);
            v.Content=Rect(root,ContentName,0,0,941,1672);
            v.Environment=Art(v.Content,"Environment",ProductionGameplayArt.Load("environment"),0,0,941,1672);
            v.BoardArea=Rect(v.Content,"BoardArea",79,357,786,942);
            var frame=Rect(v.BoardArea,"BoardFrame",-6,-6,798,954).gameObject.AddComponent<ProductionGameplayCell>();frame.FrameOnly=true;frame.raycastTarget=false;
            v.BoardMount=Rect(v.BoardArea,"BoardMount",0,0,786,942);
            v.Hud=Rect(v.Content,"GameplayHud",0,0,941,282);
            Art(v.Hud,"HudPanel",ProductionGameplayArt.Load("panel"),-6,-55,953,325);
            Art(v.Hud,"IvanPortrait",Home("avatar_portrait"),26,15,174,174).preserveAspect=true;
            Art(v.Hud,"IvanFrame",Home("avatar_ring"),0,-9,224,224).preserveAspect=true;
            Art(v.Hud,"IvanCrest",ProductionGameplayArt.Load("shield"),82,173,62,82).preserveAspect=true;
            v.HeroName=Label(v,v.Hud,"HeroName","Иван",228,22,155,45,33);v.HeroName.alignment=TextAnchor.MiddleLeft;
            v.LevelText=Label(v,v.Hud,"SelectedLevel","Уровень 1",228,70,165,38,23);v.LevelText.alignment=TextAnchor.MiddleLeft;
            Art(v.Hud,"GoalProgressFrame",Home("progress_frame"),228,111,175,22);
            v.GoalProgress=Art(v.Hud,"GoalProgress",Home("progress_fill"),235,117,161,10);Fill(v.GoalProgress);
            Art(v.Hud,"StarProgressFrame",Home("progress_frame"),405,47,179,24);
            v.StarProgress=Art(v.Hud,"StarProgress",Home("progress_fill"),412,53,165,12);Fill(v.StarProgress);
            v.Stars=new ProductionWorldMapGraphic[3];
            for(int i=0;i<3;i++){v.Stars[i]=Rect(v.Hud,"Star_"+i,395+i*76,26,47,47).gameObject.AddComponent<ProductionWorldMapGraphic>();v.Stars[i].Kind=WorldMapGraphicKind.Star;v.Stars[i].raycastTarget=false;}
            Art(v.Hud,"MovesMedallion",Resources.Load<Sprite>(ProductionChapterMapRuntime.ArtRoot+"node"),371,72,240,220);
            v.MovesHeading=Label(v,v.Hud,"MovesHeading","ХОДЫ",401,109,180,39,25);
            v.MovesValue=Label(v,v.Hud,"MovesDynamic","28",400,145,182,97,77);
            v.GoalsHeading=Label(v,v.Hud,"GoalsHeading","ЦЕЛИ",634,23,249,43,32);
            var goalFrame=Rect(v.Hud,"GoalPanel",627,72,262,118).gameObject.AddComponent<ProductionGameplayCell>();goalFrame.raycastTarget=false;
            v.Goals=new GameplayGoalView[2];
            for(int i=0;i<2;i++)
            {
                var g=Rect(v.Hud,"Goal_"+i,637+i*128,75,116,113);
                v.Goals[i]=new GameplayGoalView{Root=g,Icon=Art(g,"Icon",ProductionGameplayArt.Gem(i),14,0,77,83),
                    Count=Label(v,g,"Count","0",52,72,60,35,29),Description=Label(v,g,"Description","",0,80,64,30,13)};
                v.Goals[i].Icon.preserveAspect=true;
            }
            v.Footer=Rect(v.Content,"GameplayFooter",0,1308,941,364);
            v.BoosterButtons=new Button[4];v.BoosterCounts=new Text[4];v.ArmedMarkers=new ProductionWorldMapGraphic[4];
            string[] names={"mace","firebird","livinglight","seapearl"};
            for(int i=0;i<4;i++)
            {
                var marker=Rect(v.Footer,"Armed_"+i,39+i*197,-4,196,196).gameObject.AddComponent<ProductionWorldMapGraphic>();marker.Kind=WorldMapGraphicKind.Halo;marker.raycastTarget=false;marker.gameObject.SetActive(false);v.ArmedMarkers[i]=marker;
                var face=Art(v.Footer,"Booster_"+ProductionPreLevelRuntime.Boosters[i],ProductionGameplayArt.Load(names[i]),43+i*197,2,188,188);face.preserveAspect=true;
                v.BoosterButtons[i]=Button(face);
                var badge=Rect(face.transform,"StockBadge",130,138,54,54).gameObject.AddComponent<ProductionPreLevelBadge>();badge.raycastTarget=false;
                v.BoosterCounts[i]=Label(v,badge.transform,"Count","0",0,0,54,54,30);v.BoosterCounts[i].resizeTextForBestFit=true;v.BoosterCounts[i].resizeTextMinSize=30;v.BoosterCounts[i].resizeTextMaxSize=60;
            }
            var pause=Art(v.Footer,"Pause",Resources.Load<Sprite>("ProductionUI/SplashTitleReconstructed/ornate_gold_gear_settings_emblem"),838,65,94,103);pause.preserveAspect=true;v.PauseButton=Button(pause);
            Art(v.Footer,"ScorePanel",ProductionGameplayArt.Load("panel"),-3,159,947,246);
            v.ScoreHeading=Label(v,v.Footer,"ScoreHeading","СЧЁТ",305,220,331,38,29);
            v.ScoreValue=Label(v,v.Footer,"ScoreDynamic","0",260,257,421,62,52);
            var layout=root.GetComponent<ProductionGameplayLayout>()??root.gameObject.AddComponent<ProductionGameplayLayout>();layout.View=v;
            AlignPresentation(v);ApplyFramePolish(v);v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Incomplete Gameplay composition.");
        }
        static void ApplyFramePolish(ProductionGameplayView v)
        {
            // Upgrade the existing serialized hierarchy in place, retaining its object IDs and bindings.
            var lacquer=EnsureArt(v.BoardArea,"BoardLacquer","cell_lacquer",0,0,786,942);
            lacquer.transform.SetAsFirstSibling();
            v.BoardArea.Find("BoardFrame").SetAsLastSibling();
            var moves=v.Hud.Find("MovesMedallion").GetComponent<Image>();
            moves.sprite=ProductionGameplayArt.Load("moves_frame");moves.preserveAspect=true;
            moves.rectTransform.anchoredPosition=new Vector2(365,-70);moves.rectTransform.sizeDelta=new Vector2(252,216);
            v.MovesHeading.rectTransform.anchoredPosition=new Vector2(401,-108);
            v.MovesValue.rectTransform.anchoredPosition=new Vector2(400,-137);
            var goal=v.Hud.Find("GoalPanel").GetComponent<ProductionGameplayCell>();
            goal.InsetPanel=true;goal.SetVerticesDirty();
            var goalLacquer=EnsureArt(v.Hud,"GoalLacquer","cell_lacquer",627,72,262,118);
            PutBefore(goalLacquer.transform,goal.transform);
            var found=v.Footer.Find("ScoreInset");
            var inset=found!=null?found.GetComponent<ProductionGameplayCell>():Rect(v.Footer,"ScoreInset",184,247,573,65).gameObject.AddComponent<ProductionGameplayCell>();
            inset.InsetPanel=true;inset.BronzeSurface=true;inset.raycastTarget=false;inset.SetVerticesDirty();
            PutBefore(inset.transform,v.ScoreHeading.transform);
            v.ScoreHeading.rectTransform.anchoredPosition=new Vector2(305,-214);
            v.ScoreValue.rectTransform.anchoredPosition=new Vector2(260,-248);
            foreach(var star in v.Stars)
            {
                if(star.GetComponent<ProductionGameplayStarPolish>()==null)star.gameObject.AddComponent<ProductionGameplayStarPolish>();
                star.SetVerticesDirty();
            }
            v.Revision=ProductionGameplayView.CurrentRevision;
        }
        static Image EnsureArt(Transform parent,string name,string resource,float x,float y,float w,float h)
        {
            var found=parent.Find(name);
            return found!=null?found.GetComponent<Image>():Art(parent,name,ProductionGameplayArt.Load(resource),x,y,w,h);
        }
        static void PutBefore(Transform item,Transform target)
        {
            int index=target.GetSiblingIndex();
            if(item.GetSiblingIndex()<index)index--;
            item.SetSiblingIndex(index);
        }
        static void AlignPresentation(ProductionGameplayView v)
        {
            v.Environment.color=new Color32(174,182,208,255);
            string[] boosterArt={"mace","firebird","livinglight","seapearl"};
            for(int i=0;i<4;i++)v.BoosterButtons[i].GetComponent<Image>().sprite=ProductionGameplayArt.Load(boosterArt[i]);
            var frame=(RectTransform)v.Hud.Find("IvanFrame");frame.anchoredPosition=new Vector2(-8,8);frame.sizeDelta=new Vector2(231,231f*1425/1104);
            frame.GetComponent<Image>().preserveAspect=false;
            // Reuse Home's whole portrait and the same circular stencil construction.
            var found=v.Hud.Find("PortraitClip");
            var clip=found!=null?found.GetComponent<ProductionHomePanel>():Rect(v.Hud,"PortraitClip",28,23,160,160).gameObject.AddComponent<ProductionHomePanel>();
            clip.Frame=false;clip.Radius=80;clip.LeftColor=clip.RightColor=new Color32(7,24,42,255);clip.raycastTarget=false;
            var mask=clip.GetComponent<Mask>()??clip.gameObject.AddComponent<Mask>();mask.showMaskGraphic=true;
            var portrait=v.Hud.Find("IvanPortrait")??clip.transform.Find("IvanPortrait");portrait.SetParent(clip.transform,false);
            var r=(RectTransform)portrait;r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(160,160);portrait.GetComponent<Image>().preserveAspect=false;
            clip.transform.SetSiblingIndex(frame.GetSiblingIndex());frame.SetSiblingIndex(clip.transform.GetSiblingIndex()+1);
            v.Hud.Find("IvanCrest").gameObject.SetActive(false); // The approved ring already includes its heraldic pendant.
            v.MovesHeading.rectTransform.anchoredPosition=new Vector2(401,-117);
        }
        static void Fill(Image image){image.type=Image.Type.Filled;image.fillMethod=Image.FillMethod.Horizontal;image.fillAmount=0;}
        static Sprite Home(string n)=>Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+n);
        public static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)=>ProductionChapterMapRuntime.Rect(p,n,x,y,w,h);
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var i=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();i.sprite=s;i.raycastTarget=false;return i;}
        static Button Button(Image image)
        {
            image.raycastTarget=true;var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.navigation=new Navigation{mode=Navigation.Mode.None};
            var c=b.colors;c.pressedColor=new Color(.8f,.8f,.8f,1);c.selectedColor=Color.white;c.disabledColor=new Color(.4f,.4f,.4f,.8f);c.fadeDuration=.06f;b.colors=c;
            b.gameObject.AddComponent<ProductionHomeButtonFeedback>();return b;
        }
        static Text Label(ProductionGameplayView v,Transform p,string n,string value,float x,float y,float w,float h,int size)
        {
            var t=Rect(p,n,x,y,w*2,h*2).gameObject.AddComponent<Text>();t.rectTransform.localScale=Vector3.one*.5f;t.text=value;t.font=v.Typography.RegularFont;t.fontSize=size*2;t.fontStyle=FontStyle.Bold;
            t.color=new Color32(243,213,156,255);t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;t.raycastTarget=false;
            t.gameObject.AddComponent<SplashTitleTextGeometry>().SnapToScreenPixels=false;t.gameObject.AddComponent<ProductionHomeGoldText>().BeveledFace=true;t.material=Resources.Load<Material>(ProductionHomeRuntime.ArtRoot+"home_play_gold");return t;
        }
    }
}
