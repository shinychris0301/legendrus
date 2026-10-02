using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionTeamRankingRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/TeamRankingAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionTeamRankingView>();if(existing!=null){ProductionTeamRankingReferenceLayout.Apply(existing);if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Team Ranking outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionTeamRankingView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Team Ranking needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");v.RankFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            v.Content=Rect(root,"TEAM_RANKING_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);var shared=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);shared.name="NativeHud";
            v.Hud=(RectTransform)shared.transform;v.Header=shared;
            v.ProfileButton=shared.Profile;v.SettingsButton=shared.Settings;v.HeroName=shared.PlayerName;v.LevelText=shared.Level;
            v.PlusButtons=shared.Plus;v.GoldText=shared.Balances[0];v.GemsText=shared.Balances[1];v.EnergyText=shared.Balances[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),121,101,708,153);v.Heading=Label(v,v.Body,"Heading",185,160,572,82,54);Fit(v.Heading,38,54);
            v.PreviewLabel=Label(v,v.Body,"PreviewLabel",190,248,630,28,18);v.SeasonNote=Label(v,v.Body,"SeasonNote",210,271,550,28,22);
            v.Tabs=new Button[4];v.TabLabels=new Text[4];float[] tx={25,219,537,728};
            for(int i=0;i<4;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Shared("tab"),tx[i],296,188,57));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",12,8,164,42,26);Fit(v.TabLabels[i],19,26);}
            v.Podiums=new RectTransform[3];v.PodiumNames=new Text[3];v.PodiumMembers=new Text[3];v.PodiumScores=new Text[3];v.PodiumRewards=new Text[9];
            float[] px={309,23,638},py={305,352,357},pw={324,267,246},ph={490,438,433};
            // Source podium order is second, first, third on screen; array order follows actual rank.
            for(int i=0;i<3;i++){
                var card=Rect(v.Body,"Podium_"+(i+1),px[i],py[i],pw[i],ph[i]);v.Podiums[i]=card;Art(card,"SourceFrameAndEmblem",Load(new[]{"podium_gold","podium_silver","podium_bronze"}[i]),0,0,pw[i],ph[i]);
                float nx=i==0?23:18,nw=pw[i]-nx*2;
                v.PodiumNames[i]=Label(v,card,"GuildName",nx,568-py[i],nw,48,i==0?32:28);Fit(v.PodiumNames[i],20,i==0?32:28);
                v.PodiumMembers[i]=Label(v,card,"Members",i==0?145:111,608-py[i],i==0?112:107,36,29);
                if(i==0)Art(card,"MembersIcon",Load("members"),112,613-py[i],27,31);Fit(v.PodiumMembers[i],23,29);
                v.PodiumScores[i]=Label(v,card,"Contribution",i==0?125:108,644-py[i],i==0?160:120,44,32);Fit(v.PodiumScores[i],23,32);
                float left=i==0?35:21,cell=(pw[i]-left*2)/3;
                for(int j=0;j<3;j++){float x=left+j*cell;Panel(card,"RewardSlot_"+j,Shared("reward_frame"),x,691-py[i],cell,89);Art(card,"RewardIcon_"+j,Load(new[]{"reward_rainbow","reward_gem","reward_chest"}[j]),x+3,697-py[i],cell-6,55);v.PodiumRewards[i*3+j]=Label(v,card,"RewardCount_"+j,x,748-py[i],cell,36,27);}
            }
            Panel(v.Body,"TableFrame",Shared("table_frame"),21,797,899,662);v.ColumnLabels=new Text[5];float[] cx={39,198,382,522,681},cw={83,193,129,139,202};
            for(int i=0;i<5;i++){v.ColumnLabels[i]=Label(v,v.Body,"Column_"+i,cx[i],804,cw[i],33,22);Fit(v.ColumnLabels[i],14,22);}
            v.Rows=new RectTransform[7];v.Crests=new Image[7];v.Ranks=new Text[7];v.Names=new Text[7];v.Members=new Text[7];v.Scores=new Text[7];v.Rewards=new Text[21];
            for(int i=0;i<7;i++){
                var row=Rect(v.Body,"GuildExample_"+(i+4),30,841+i*86,880,85);v.Rows[i]=row;Panel(row,"Frame",Shared(i==3?"row_player":"row"),0,0,880,85);
                v.Ranks[i]=Label(v,row,"Rank",12,22,67,48,34);v.Crests[i]=Art(row,"GuildCrest",Load("crest_"+new[]{"wolf","knot","stag","eagle","iron","sun","flame"}[i]),84,-3,82,90);
                v.Names[i]=Label(v,row,"GuildName",175,22,170,50,26);v.Names[i].alignment=TextAnchor.MiddleLeft;Fit(v.Names[i],18,26);
                Art(row,"MembersIcon",Load("members"),357,29,27,34);v.Members[i]=Label(v,row,"Members",387,22,89,48,27);Fit(v.Members[i],22,27);
                Art(row,"ContributionIcon",Load("contribution"),478,20,35,46);v.Scores[i]=Label(v,row,"Contribution",515,22,113,48,27);Fit(v.Scores[i],22,27);
                for(int j=0;j<3;j++){float x=643+j*72;Panel(row,"RewardSlot_"+j,Shared("reward_frame"),x,4,70,78);Art(row,"RewardIcon_"+j,Load(new[]{"reward_rainbow","reward_gem","reward_chest"}[j]),x+5,8,61,49);v.Rewards[i*3+j]=Label(v,row,"RewardCount_"+j,x,53,72,29,23);}
            }
            Panel(v.Body,"PlayerGuildPlaque",Shared("rules"),328,1086,367,31);v.PlayerGuildLabel=Label(v,v.Body,"PlayerGuildLabel",337,1084,349,35,20);Fit(v.PlayerGuildLabel,16,20);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),23,1539,106,101));
            v.DetailsButton=Button(Art(v.Body,"GuildDetails",Load("details"),181,1478,605,153));v.DetailsLabel=Label(v,v.DetailsButton.transform,"Label",45,45,515,66,39);Fit(v.DetailsLabel,27,39);
            Art(v.Body,"FooterOrnament",Resources.Load<Sprite>("ProductionUI/ShopAligned/footer_ornament"),139,1631,759,36);
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionTeamRankingLayout>();layout.View=v;ProductionTeamRankingReferenceLayout.Apply(v);v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings){
                foreach(var field in typeof(ProductionTeamRankingView).GetFields()){
                    var value=field.GetValue(v);if(typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)&&!(value as UnityEngine.Object))Debug.LogError("Team Ranking missing binding: "+field.Name);
                    if(value is Array array)for(int i=0;i<array.Length;i++)if(!(array.GetValue(i) as UnityEngine.Object))Debug.LogError("Team Ranking missing array binding: "+field.Name+"["+i+"]");
                }
                throw new InvalidOperationException("Team Ranking native bindings incomplete.");
            }
        }
        static void BuildInfo(ProductionTeamRankingView v)
        {
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,402,817,838);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",108,431,725,102,40);Fit(v.InfoHeading,29,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",114,556,713,492,31);Fit(v.InfoDescription,25,31);
            v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1118,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionTeamRankingView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
