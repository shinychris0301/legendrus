using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionTeamHubRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/TeamHubAligned/"+key);
        static Sprite Ranking(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionTeamHubView>();if(existing!=null){ProductionTeamHubReferenceLayout.Apply(existing);if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Team Hub outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionTeamHubView>();
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Team Hub needs the accepted navigation header.");
            v.Content=Rect(root,"TEAM_HUB_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Load("background"),0,0,941,1672);v.Environment.color=new Color(.68f,.68f,.68f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            var shared=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);shared.name="NativeHud";
            v.Hud=(RectTransform)shared.transform;v.Header=shared;
            v.ProfileButton=shared.Profile;v.SettingsButton=shared.Settings;v.HeroName=shared.PlayerName;v.LevelText=shared.Level;
            v.PlusButtons=shared.Plus;v.GoldText=shared.Balances[0];v.GemsText=shared.Balances[1];v.EnergyText=shared.Balances[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),178,96,585,171);v.Heading=Label(v,v.Body,"Heading",290,159,365,78,58);v.PreviewLabel=Label(v,v.Body,"PreviewLabel",240,270,661,29,20);
            Art(v.Body,"GuildBanner",Load("banner"),15,250,382,411);
            v.GuildName=Label(v,v.Body,"GuildName",414,314,481,64,53);Fit(v.GuildName,30,53);
            v.MemberCount=Label(v,v.Body,"MemberCount",438,380,166,38,29);v.Membership=Label(v,v.Body,"Membership",614,380,281,38,24);
            Panel(v.Body,"PowerFrame",Ranking("reward_panel"),413,427,487,110);Art(v.Body,"PowerIcon",Load("power"),431,442,79,82);
            v.PowerHeading=Label(v,v.Body,"PowerHeading",521,438,361,36,26);v.Power=Label(v,v.Body,"Power",521,469,361,66,44);Fit(v.Power,35,44);
            v.ProgressHeading=Label(v,v.Body,"ProgressHeading",384,555,351,33,19);
            var track=Rect(v.Body,"ProgressTrack",387,591,346,35);var trackSurface=track.gameObject.AddComponent<ProductionHomePanel>();trackSurface.Radius=12;trackSurface.Frame=true;trackSurface.LeftColor=trackSurface.RightColor=new Color32(3,15,27,255);trackSurface.raycastTarget=false;
            v.ProgressFill=Rect(track,"ProgressFill",7,7,332*.756f,21).gameObject.AddComponent<ProductionHomePanel>();
            v.ProgressFill.Frame=false;v.ProgressFill.Radius=5;v.ProgressFill.LeftColor=new Color32(15,93,148,255);v.ProgressFill.RightColor=new Color32(22,132,189,255);v.ProgressFill.raycastTarget=false;
            v.Progress=Label(v,track,"ProgressText",8,0,330,35,23);
            v.ChestButton=Button(Art(v.Body,"GuildChest",Load("chest"),763,497,110,91));v.ChestLabel=Label(v,v.Body,"ChestLabel",731,587,179,65,19);
            v.TasksHeading=Label(v,v.Body,"TasksHeading",89,662,763,38,26);v.TaskLabels=new Text[3];v.TaskCounts=new Text[3];v.TaskRewards=new Text[3];
            for(int i=0;i<3;i++){
                float x=54+i*283;var card=Rect(v.Body,"TaskExample_"+i,x,704,276,179);Art(card,"Frame",Load("task_frame"),0,0,276,179);Art(card,"Icon",Load(new[]{"task_banner","task_swords","task_potion"}[i]),14,15,79,88);
                v.TaskLabels[i]=Label(v,card,"Task",98,12,161,63,24);Fit(v.TaskLabels[i],19,24);v.TaskCounts[i]=Label(v,card,"Progress",95,80,164,39,25);Panel(card,"RewardPlaque",Ranking("rules"),20,125,239,43);Art(card,"RewardGem",Ranking("gem"),100,127,31,37);v.TaskRewards[i]=Label(v,card,"RewardCount",132,124,112,41,30);
            }
            v.NavButtons=new Button[4];v.NavLabels=new Text[4];string[] icons={"chat","ranking","tasks","donate"};
            for(int i=0;i<4;i++){
                float x=35+i*219;var card=Panel(v.Body,"Navigation_"+i,Load(i==3?"donate_frame":"nav_frame"),x,899,214,128);v.NavButtons[i]=Button(card);Art(card.transform,"Icon",Load(icons[i]),43,10,128,71);v.NavLabels[i]=Label(v,card.transform,"Label",12,81,190,42,27);Fit(v.NavLabels[i],19,27);
            }
            v.MembersHeading=Label(v,v.Body,"MembersHeading",180,1034,666,35,26);v.ContributionHeading=Label(v,v.Body,"ContributionHeading",684,1065,215,27,17);
            v.MemberRows=new RectTransform[5];v.Portraits=new Image[5];v.MemberRanks=new Text[5];v.MemberNames=new Text[5];v.MemberRoles=new Text[5];v.MemberScores=new Text[5];
            for(int i=0;i<5;i++){
                float y=1092+i*88;var row=Rect(v.Body,"MemberExample_"+i,40,y,858,82);v.MemberRows[i]=row;Panel(row,"Frame",Ranking(i==0?"row_player":"row"),0,0,858,82);
                bool medal=i<3;Art(row,"RankIcon",Ranking(medal?new[]{"medal_gold","medal_silver","medal_bronze"}[i]:"rank_plain"),10,9,medal?82:62,64);
                v.MemberRanks[i]=Label(v,row,"Rank",11,15,60,53,32);v.MemberRanks[i].enabled=!medal;
                v.Portraits[i]=Art(row,"Portrait",Load("portrait_"+new[]{"ivan","olga","svyatoslav","velimir","lada"}[i]),97,-6,101,94);
                v.MemberNames[i]=Label(v,row,"Name",215,7,355,40,31);v.MemberNames[i].alignment=TextAnchor.MiddleLeft;Fit(v.MemberNames[i],23,31);
                v.MemberRoles[i]=Label(v,row,"Role",215,47,355,28,20);v.MemberRoles[i].alignment=TextAnchor.MiddleLeft;v.MemberRoles[i].color=i==0?new Color32(145,204,97,255):new Color32(117,185,227,255);
                Art(row,"PowerIcon",Load("power"),648,14,47,54);v.MemberScores[i]=Label(v,row,"Contribution",700,17,142,46,31);
            }
            v.BackButton=Button(Art(v.Body,"Back",Ranking("back"),22,1548,100,107));
            Panel(v.Body,"MottoFrame",Ranking("reward_panel"),132,1547,563,94);v.Motto=Label(v,v.Body,"Motto",151,1562,521,60,27);Fit(v.Motto,22,27);
            v.FriendsButton=Button(Panel(v.Body,"Friends",Ranking("rules"),715,1547,196,94));v.FriendsLabel=Label(v,v.FriendsButton.transform,"Label",14,25,168,42,26);Fit(v.FriendsLabel,21,26);
            Art(v.Body,"FooterOrnament",Resources.Load<Sprite>("ProductionUI/ShopAligned/footer_ornament"),132,1636,764,29);
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionTeamHubLayout>();layout.View=v;ProductionTeamHubReferenceLayout.Apply(v);v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Team Hub native bindings incomplete.");
        }
        static void BuildInfo(ProductionTeamHubView v)
        {
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Ranking("reward_panel"),70,450,801,720);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",112,480,717,100,40);Fit(v.InfoHeading,31,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",123,614,695,357,29);Fit(v.InfoDescription,25,29);
            v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"CloseInfo",Ranking("rules"),277,1043,387,85));v.CloseInfoText=Label(v,v.CloseInfoButton.transform,"Label",24,19,339,49,30);v.InfoPanel.SetActive(false);
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name.EndsWith("_frame")?3:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionTeamHubView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
