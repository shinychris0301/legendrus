using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Page-specific source-layout migration. Existing prefab objects and bindings are retained.
    public static class ProductionIndividualRankingReferenceLayout
    {
        public const string ArtRoot="ProductionUI/IndividualRankingAligned/";
        public static Sprite Load(string key)=>Resources.Load<Sprite>(ArtRoot+key);
        static readonly float[] Tops={616,689,762,827,888,949,1010,1070,1130,1189,1267,1337,1398,1478};
        static readonly float[] Heights={72,73,65,61,61,61,60,60,59,55,70,61,60,59};
        static readonly string[] Portraits={"masha","ilya","firebird","vasilisa","cat","koschey","dobrynya","yaga","alyosha","zmei","ivan","solovey","vodyanoi","leshy"};
        public static void Apply(ProductionIndividualRankingView v)
        {
            v.Revision=ProductionIndividualRankingView.CurrentRevision;
            v.PlayerNamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            v.HeadingFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.RankNumbersFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            v.Environment.sprite=Load("background");v.Environment.color=Color.white;
            v.OriginalRussianTitle=Load("title_original_ru");v.Title.sprite=Load("title");Place(v.Title.rectTransform,113,107,743,169);
            Place(v.Heading.rectTransform,144,168,661,75);v.Heading.fontSize=64;v.Heading.resizeTextForBestFit=true;v.Heading.resizeTextMinSize=44;v.Heading.resizeTextMaxSize=64;
            Hide(v.Body,"SeasonFrame");v.SeasonHeading.gameObject.SetActive(false);
            Place(v.SeasonNote.rectTransform,228,272,481,35);v.SeasonNote.fontSize=24;
            Art(v.Body,"SeasonClock",Load("clock"),522,268,34,36).gameObject.SetActive(false); // No invented deadline.
            Place(v.PreviewLabel.rectTransform,227,252,481,24);v.PreviewLabel.fontSize=19;
            Array.Resize(ref v.Tabs,3);Array.Resize(ref v.TabLabels,3);
            for(int i=0;i<3;i++)
            {
                if(v.Tabs[i]==null)v.Tabs[i]=Button(Art(v.Body,"Tab_"+i,Load("tab"),0,0,1,1));
                Place((RectTransform)v.Tabs[i].transform,120+i*244,310,223,69);
                v.Tabs[i].image.type=Image.Type.Simple;v.Tabs[i].image.preserveAspect=true;
                Hide(v.Tabs[i].transform,"SelectedFace");
                if(v.TabLabels[i]==null)v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",0,0,1,1,30);
                Place(v.TabLabels[i].rectTransform,18,15,187,40);v.TabLabels[i].fontSize=30;
            }
            var rewardPanel=Art(v.Body,"SourceRewardPanel",Load("reward_panel"),16,379,909,199,true);rewardPanel.type=Image.Type.Simple;rewardPanel.preserveAspect=false;rewardPanel.transform.SetSiblingIndex(v.Title.transform.GetSiblingIndex());
            v.RewardsHeading=Label(v,v.Body,"PlaceRewardsHeading",120,389,701,34,28);
            v.PrizeFrames=new RectTransform[3];v.PrizeTitles=new Text[3];v.PrizeGems=new Text[3];v.PrizeCards=new Text[3];
            for(int i=0;i<3;i++)
            {
                float width=new[]{280f,264f,249f}[i],gemX=new[]{164f,147f,150f}[i];
                var card=Rect(v.Body,"PlaceReward_"+i,new[]{65f,351f,622f}[i],424,width,127);v.PrizeFrames[i]=card;
                Art(card,"Frame",Load("reward_frame"),0,0,width,127,true);
                Art(card,"Chest",Load("chest_"+new[]{"gold","silver","bronze"}[i]),new[]{7f,14f,6f}[i],new[]{5f,17f,22f}[i],new[]{145f,132f,134f}[i],new[]{119f,107f,95f}[i]);
                v.PrizeTitles[i]=Label(v,card,"Place",new[]{132f,112f,63f}[i],0,new[]{143f,143f,170f}[i],38,27);v.PrizeTitles[i].resizeTextForBestFit=true;v.PrizeTitles[i].resizeTextMinSize=20;v.PrizeTitles[i].resizeTextMaxSize=27;
                Art(card,"Gem",Load("gem"),gemX,53,27,31);v.PrizeGems[i]=Label(v,card,"GemCount",gemX+27,51,width-gemX-31,34,23);
                Art(card,"Card",Load("card"),gemX,85,33,34);v.PrizeCards[i]=Label(v,card,"CardCount",gemX+40,85,40,33,23);
            }
            var table=Art(v.Body,"SourceTableFrame",Load("table_frame"),17,575,907,971,true);table.type=Image.Type.Simple;table.preserveAspect=false;table.transform.SetSiblingIndex(rewardPanel.transform.GetSiblingIndex()+1);
            Array.Resize(ref v.ColumnLabels,4);
            float[] cx={38,223,554,745},cw={111,255,157,143};
            for(int i=0;i<4;i++)
            {
                if(v.ColumnLabels[i]==null)v.ColumnLabels[i]=Label(v,v.Body,"RewardColumn",0,0,1,1,24);
                Place(v.ColumnLabels[i].rectTransform,cx[i],584,cw[i],31);v.ColumnLabels[i].fontSize=24;v.ColumnLabels[i].alignment=TextAnchor.MiddleCenter;
            }
            Array.Resize(ref v.Rows,14);Array.Resize(ref v.Ranks,14);Array.Resize(ref v.Names,14);Array.Resize(ref v.Scores,14);
            v.RowPortraits=new Image[14];v.RowRewards=new Image[14];
            var artwork=Rect(v.Body,"RowArtwork",0,0,941,1672);
            for(int i=0;i<14;i++)
            {
                if(i==10){v.Rows[i]=v.PlayerRow;v.Ranks[i]=v.PlayerRank;v.Names[i]=v.PlayerName;v.Scores[i]=v.PlayerScore;v.PlayerStatus.gameObject.SetActive(false);}
                if(v.Rows[i]==null)v.Rows[i]=Rect(v.Body,"ReferenceRow_"+i,0,0,1,1);
                var row=v.Rows[i];Place(row,30,Tops[i],880,Heights[i]);
                foreach(string old in new[]{"PortraitWindow","PortraitRim","RankBadge","SourcePortrait","SourceReward"})Hide(row,old);
                var frame=Art(row,"Frame",Load(i==10?"row_player":"row"),0,0,880,Heights[i],true);frame.color=Color.white;frame.transform.SetAsFirstSibling();
                bool medal=i<3;string rank=medal?new[]{"medal_gold","medal_silver","medal_bronze"}[i]:i==10?"rank_player":"rank_plain";
                Art(row,"SourceRank",Load(rank),medal?10:24,medal?new[]{615f,686f,758f}[i]-Tops[i]:(Heights[i]-57)*.5f,medal?94:59,medal?(i==2?74:77):57);
                if(v.Ranks[i]==null)v.Ranks[i]=Label(v,row,"Rank",0,0,1,1,32);
                Place(v.Ranks[i].rectTransform,medal?32:28.5f,(Heights[i]-46)*.5f,50,46);v.Ranks[i].fontSize=medal?35:31;v.Ranks[i].enabled=!medal; // Original fixed podium numerals belong to the extracted medals.
                v.Ranks[i].transform.SetAsLastSibling();
                var portrait=Load("portrait_"+Portraits[i]);Vector2 portraitSize=portrait.rect.size/3f;
                v.RowPortraits[i]=Art(artwork,"Portrait_"+i,portrait,152,Tops[i]+(Heights[i]-portraitSize.y)*.5f,portraitSize.x,portraitSize.y);
                if(v.Names[i]==null)v.Names[i]=Label(v,row,"PlayerName",0,0,1,1,30);
                Place(v.Names[i].rectTransform,228,(Heights[i]-42)*.5f,263,42);v.Names[i].alignment=TextAnchor.MiddleLeft;v.Names[i].fontSize=30;v.Names[i].resizeTextForBestFit=true;v.Names[i].resizeTextMinSize=18;v.Names[i].resizeTextMaxSize=30;
                v.Names[i].color=i==10?new Color32(106,207,255,255):new Color32(240,203,135,255);
                Art(row,"PowerIcon",Load("power"),495,(Heights[i]-38)*.5f,36,38);
                if(v.Scores[i]==null)v.Scores[i]=Label(v,row,"Power",0,0,1,1,30);
                Place(v.Scores[i].rectTransform,533,(Heights[i]-42)*.5f,128,42);v.Scores[i].fontSize=30;v.Scores[i].alignment=TextAnchor.MiddleRight;v.Scores[i].resizeTextForBestFit=true;v.Scores[i].resizeTextMinSize=25;v.Scores[i].resizeTextMaxSize=30;
                v.Scores[i].color=i==10?new Color32(106,207,255,255):new Color32(240,203,135,255);
                var reward=Load(i<3?"row_chest_"+new[]{"gold","silver","bronze"}[i]:"chest_purple");Vector2 rewardSize=reward.rect.size/3f;
                v.RowRewards[i]=Art(artwork,"Reward_"+i,reward,744,Tops[i]+(Heights[i]-rewardSize.y)*.5f,rewardSize.x,rewardSize.y);
            }
            Label(v,v.Body,"MoreAbovePlayer",440,1231,60,32,24).text=". . .";
            Label(v,v.Body,"MoreBelowPlayer",440,1438,60,28,24).text=". . .";
            v.EmptyPanel.SetActive(false);Hide(v.Body,"FooterBacking");v.Footer.gameObject.SetActive(false);
            v.BackButton.image.sprite=Load("back");Place((RectTransform)v.BackButton.transform,20,1533,110,119);
            Place((RectTransform)v.Body.Find("FooterOrnament"),130,1627,774,32);
            v.RulesButton=Button(Art(v.Body,"RulesButton",Load("rules"),247,1548,450,81));
            v.RulesButtonText=Label(v,v.RulesButton.transform,"Label",45,21,360,39,29);
            BuildRules(v);
        }
        static void BuildRules(ProductionIndividualRankingView v)
        {
            v.RulesPanel=Rect(v.Body,"RulesOverlay",0,0,941,1672).gameObject;
            var shade=Rect(v.RulesPanel.transform,"InputShield",0,0,941,1672).GetComponent<ProductionHomePanel>()??v.RulesPanel.transform.Find("InputShield").gameObject.AddComponent<ProductionHomePanel>();
            shade.Frame=false;shade.Radius=0;shade.LeftColor=shade.RightColor=new Color(0,0,0,.85f);shade.raycastTarget=true;
            var rulesFrame=Art(v.RulesPanel.transform,"Frame",Load("reward_panel"),90,450,761,685,true);rulesFrame.type=Image.Type.Tiled;
            v.RulesHeading=Label(v,v.RulesPanel.transform,"Heading",130,476,681,80,42);
            v.RulesDescription=Label(v,v.RulesPanel.transform,"Description",150,579,641,365,31);
            v.CloseRulesButton=Button(Art(v.RulesPanel.transform,"CloseRules",Load("rules"),270,995,401,81));
            v.CloseRulesText=Label(v,v.CloseRulesButton.transform,"Label",35,20,331,42,31);
            v.RulesPanel.transform.SetAsLastSibling();v.RulesPanel.SetActive(false);
        }
        static void Hide(Transform p,string name){var t=p.Find(name);if(t!=null)t.gameObject.SetActive(false);}
        static void Place(RectTransform r,float x,float y,float w,float h){r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)
        {var r=p.Find(n) as RectTransform;if(r==null){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);r=(RectTransform)go.transform;}Place(r,x,y,w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h,bool sliced=false)
        {var r=Rect(p,n,x,y,w,h);var a=r.GetComponent<Image>()??r.gameObject.AddComponent<Image>();a.sprite=s;a.type=sliced?Image.Type.Sliced:Image.Type.Simple;a.preserveAspect=!sliced;a.pixelsPerUnitMultiplier=sliced?(s.name=="reward_frame"?3:2):1;a.raycastTarget=false;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.GetComponent<Button>()??a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionIndividualRankingView v,Transform p,string n,float x,float y,float w,float h,int size)
        {var r=Rect(p,n,x,y,w,h);var t=r.GetComponent<Text>()??r.gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(239,201,130,255);t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;}
    }
}
