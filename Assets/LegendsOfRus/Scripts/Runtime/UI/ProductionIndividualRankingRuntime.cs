using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionIndividualRankingRuntime
    {
        public static Sprite Load(string name)=>ProductionEventsRuntime.Load(name);
        static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionIndividualRankingView>();
            if(existing!=null){ProductionIndividualRankingReferenceLayout.Apply(existing);if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Individual Ranking outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionIndividualRankingView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();
            if(shop==null||!shop.HasRequiredBindings)throw new InvalidOperationException("Individual Ranking requires the accepted shared header.");
            v.Typography=shop.Typography;
            v.Content=Rect(root,"INDIVIDUAL_RANKING_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("FrontEndHomeAligned/loading_environment_dusk"),0,0,941,1672);v.Environment.color=new Color(.46f,.46f,.52f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);
            v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();
            v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];


            v.Title=Art(v.Body,"TitlePlaque",Load("title_r3"),211,117,535,184);
            v.Heading=Label(v,v.Body,"Heading",260,169,435,105,64);Gold(v.Heading,true);
            v.PreviewLabel=Label(v,v.Body,"PreviewLabel",110,302,721,40,27);
            Sliced(v.Body,"SeasonFrame",Load("frame_r3"),45,355,851,145);
            v.SeasonHeading=Label(v,v.Body,"SeasonHeading",100,367,741,53,40);Gold(v.SeasonHeading,false);
            v.SeasonNote=Label(v,v.Body,"SeasonNote",100,431,741,54,26);
            v.Tabs=new Button[2];v.TabLabels=new Text[2];
            for(int i=0;i<2;i++){
                v.Tabs[i]=Button(Sliced(v.Body,"Tab_"+i,Load("tab_r3"),65+i*416,520,395,70));
                var face=Rect(v.Tabs[i].transform,"SelectedFace",23,17,349,36).gameObject.AddComponent<ProductionHomePanel>();
                face.Frame=false;face.Radius=8;face.LeftColor=new Color32(10,69,112,255);face.RightColor=new Color32(5,31,57,255);face.raycastTarget=false;face.transform.SetAsFirstSibling();
                v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",26,15,343,43,29);
            }
            v.ColumnLabels=new Text[3];
            v.ColumnLabels[0]=Label(v,v.Body,"RankColumn",60,602,125,31,22);
            v.ColumnLabels[1]=Label(v,v.Body,"PlayerColumn",288,602,329,31,22);v.ColumnLabels[1].alignment=TextAnchor.MiddleLeft;
            v.ColumnLabels[2]=Label(v,v.Body,"ScoreColumn",658,602,211,31,22);
            v.Rows=new RectTransform[7];v.Ranks=new Text[7];v.Names=new Text[7];v.Scores=new Text[7];
            for(int i=0;i<7;i++){
                var row=Rect(v.Body,"PreviewRow_"+i,45,643+i*104,851,94);v.Rows[i]=row;
                Sliced(row,"Frame",Load("frame_r3"),0,0,851,94);
                if(i<3){var badge=Rect(row,"RankBadge",37,16,62,62).gameObject.AddComponent<ProductionHomePanel>();badge.RaisedBadge=true;badge.raycastTarget=false;}
                v.Ranks[i]=Label(v,row,"Rank",37,20,62,54,34);Gold(v.Ranks[i],false);
                Portrait(row,130,12,i%2==1);
                v.Names[i]=Label(v,row,"PlayerName",243,19,329,54,32);v.Names[i].alignment=TextAnchor.MiddleLeft;
                v.Scores[i]=Label(v,row,"Score",613,19,211,54,34);Gold(v.Scores[i],false);
            }
            v.EmptyPanel=Rect(v.Body,"FriendsUnavailable",75,746,791,400).gameObject;
            Sliced(v.EmptyPanel.transform,"Frame",Load("frame_r3"),0,0,791,400);
            v.EmptyText=Label(v,v.EmptyPanel.transform,"Message",65,82,661,236,34);
            v.PlayerRow=Rect(v.Body,"YourStanding",45,1390,851,139);
            var highlight=Sliced(v.PlayerRow,"Frame",Load("frame_r3"),0,0,851,139);highlight.color=new Color32(158,222,247,255);
            v.PlayerRank=Label(v,v.PlayerRow,"ActualRank",32,42,72,54,34);
            Portrait(v.PlayerRow,130,34,false);
            v.PlayerName=Label(v,v.PlayerRow,"PlayerName",243,16,329,50,32);v.PlayerName.alignment=TextAnchor.MiddleLeft;
            v.PlayerStatus=Label(v,v.PlayerRow,"PlayerStatus",243, 70,329,48,25);v.PlayerStatus.alignment=TextAnchor.MiddleLeft;
            v.PlayerScore=Label(v,v.PlayerRow,"ActualScore",613,42,211,54,34);
            v.BackButton=Button(Art(v.Body,"Back",Shared("DailyRewardAligned/back_r2"),26,1550,90,86));
            Sliced(v.Body,"FooterBacking",Load("status_r3"),130,1542,770,88);
            v.Footer=Label(v,v.Body,"Footer",152,1547,728,77,24);
            Art(v.Body,"FooterOrnament",Shared("ShopAligned/footer_ornament"),124,1637,780,28);
            var layout=root.gameObject.AddComponent<ProductionIndividualRankingLayout>();layout.View=v;ProductionIndividualRankingReferenceLayout.Apply(v);v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Individual Ranking bindings incomplete.");
        }
        static Image Sliced(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {var a=Art(p,n,sprite,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;return a;}
        static void Portrait(Transform parent,float x,float y,bool female)
        {
            var rim=Rect(parent,"PortraitRim",x-4,y-4,78,78).gameObject.AddComponent<ProductionHomePanel>();rim.Radius=39;rim.raycastTarget=false;
            var window=Rect(parent,"PortraitWindow",x,y,70,70);var shape=window.gameObject.AddComponent<ProductionHomePanel>();shape.Frame=false;shape.Radius=35;shape.raycastTarget=false;
            window.gameObject.AddComponent<Mask>().showMaskGraphic=true;
            var sprite=Shared(female?"FrontEndHomeAligned/masha_dusk":"HomeAligned/avatar_portrait");
            float h=female?307.2f:70,w=h*sprite.rect.width/sprite.rect.height;
            Art(window,"Portrait",sprite,female?-72:0,female?-28:0,w,h);
        }
        static void Gold(Text text,bool bevel)
        {
            var shadow=text.GetComponent<Shadow>()??text.gameObject.AddComponent<Shadow>();
            shadow.effectColor=new Color32(36,16,3,235);shadow.effectDistance=new Vector2(1.5f,-2);
            var gold=text.GetComponent<ProductionHomeGoldText>()??text.gameObject.AddComponent<ProductionHomeGoldText>();
            gold.BeveledFace=bevel;gold.TopColor=new Color32(255,235,163,255);gold.BottomColor=new Color32(189,115,25,255);
            if(bevel)text.material=Resources.Load<Material>(ProductionHomeRuntime.PlayGoldMaterialResource);
        }
        static Transform Clone(Transform source,Transform parent){var copy=UnityEngine.Object.Instantiate(source,parent,false);copy.name=source.name;return copy;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=s!=null;a.raycastTarget=false;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionIndividualRankingView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,214,155,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
