using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Retain existing bindings while migrating the accepted Team Hub prefab.
    public static class ProductionTeamHubReferenceLayout
    {
        public static void Apply(ProductionTeamHubView v)
        {
            v.Revision=ProductionTeamHubView.CurrentRevision;
            v.CaptionFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            ProductionPlayerHeader.Attach(v);
            Place(v.PreviewLabel.rectTransform,300,270,591,30);v.PreviewLabel.fontSize=18;
            var summary=NeedArt(v.Body,"GuildSummaryBacking",ProductionTeamHubRuntime.Load("nav_frame"),403,311,496,110);
            summary.type=Image.Type.Sliced;summary.pixelsPerUnitMultiplier=3;summary.color=new Color(.58f,.58f,.58f,1);
            summary.transform.SetAsFirstSibling();
            Place(v.GuildName.rectTransform,424,314,459,64);v.GuildName.fontSize=53;v.GuildName.resizeTextMinSize=30;v.GuildName.resizeTextMaxSize=53;
            v.MemberCount.fontSize=27;v.Membership.fontSize=22;
            NeedArt(v.Body,"MemberGroupIcon",ProductionTeamHubRuntime.Load("members_icon_r2"),442,378,42,36);
            Place(v.MemberCount.rectTransform,482,377,130,42);Place(v.Membership.rectTransform,625,377,255,42);
            var power=v.Body.Find("PowerFrame").GetComponent<Image>();power.sprite=ProductionTeamHubRuntime.Load("power_frame_r2");
            power.type=Image.Type.Simple;power.preserveAspect=false;Place(power.rectTransform,412,425,494,114);
            Place(v.PowerHeading.rectTransform,527,433,342,41);v.PowerHeading.fontSize=25;v.PowerHeading.alignment=TextAnchor.MiddleLeft;
            Place(v.Power.rectTransform,527,468,342,65);v.Power.alignment=TextAnchor.MiddleLeft;
            Place(v.ProgressHeading.rectTransform,375,551,266,38);v.ProgressHeading.fontSize=16;
            var track=(RectTransform)v.ProgressFill.transform.parent;Place(track,369,589,271,36);
            var face=track.GetComponent<ProductionHomePanel>();face.enabled=false;
            var empty=track.Find("EmptyChannel") as RectTransform;
            if(empty==null){var go=new GameObject("EmptyChannel",typeof(RectTransform));go.layer=5;go.transform.SetParent(track,false);empty=(RectTransform)go.transform;}
            Place(empty,19,7,233,22);empty.SetAsFirstSibling();
            var surface=empty.GetComponent<ProductionHomePanel>()??empty.gameObject.AddComponent<ProductionHomePanel>();
            surface.Frame=false;surface.Radius=5;surface.LeftColor=surface.RightColor=new Color32(3,15,27,255);surface.raycastTarget=false;
            Place((RectTransform)v.ProgressFill.transform,19,8,233*.756f,20);
            v.ProgressFill.Radius=5;v.ProgressFill.enabled=false;
            v.SeasonFill=NeedArt(track,"SapphireFill",Resources.Load<Sprite>("ProductionUI/LoadingReferenceAligned/source_progress_fill"),19,7,233,22);
            v.SeasonFill.preserveAspect=false;v.SeasonFill.type=Image.Type.Filled;v.SeasonFill.fillMethod=Image.FillMethod.Horizontal;v.SeasonFill.fillOrigin=0;
            v.SeasonFill.fillAmount=ProductionTeamHubView.SampleSeasonFraction;
            var rail=NeedArt(track,"SourceRail",ProductionTeamHubRuntime.Load("progress_rail_r2"),0,0,271,36);
            Before(v.SeasonFill.transform,rail.transform);rail.transform.SetAsLastSibling();
            Place(v.Progress.rectTransform,20,-1,231,38);v.Progress.fontSize=23;v.Progress.transform.SetAsLastSibling();
            NeedArt(v.Body,"SeasonChest",ProductionTeamHubRuntime.Load("chest"),641,549,92,76);
            var enclosure=NeedArt(v.Body,"GuildChestEnclosure",ProductionTeamHubRuntime.Load("chest_enclosure_ru_r2"),738,480,145,159);
            Before(enclosure.transform,v.ChestButton.transform);
            // Count/plaque sprites carry only ornament or blank faces; values remain live.
            Place(v.ChestLabel.rectTransform,747,590,128,42);v.ChestLabel.fontSize=17;
            var taskShell=NeedArt(v.Body,"TasksEnclosure",ProductionTeamHubRuntime.Load("tasks_enclosure_ru_r2"),24,652,894,239);
            Before(taskShell.transform,v.TasksHeading.transform);
            Place(v.TasksHeading.rectTransform,276,658,390,38);v.TasksHeading.fontSize=25;
            for(int i=0;i<3;i++)
            {
                var card=v.TaskLabels[i].transform.parent;
                float[] taskX={54,338,608};float[] taskWidth={276,259,270};
                Place((RectTransform)card,taskX[i],702,taskWidth[i],179);
                var frame=card.Find("Frame").GetComponent<Image>();frame.preserveAspect=false;
                Place(frame.rectTransform,0,0,taskWidth[i],179);
                var count=NeedArt(card,"CountPlaque",ProductionTeamHubRuntime.Load("task_count_plaque_r2"),105,78,taskWidth[i]-119,36);
                count.preserveAspect=false;Before(count.transform,v.TaskCounts[i].transform);
                Place(v.TaskLabels[i].rectTransform,103,16,taskWidth[i]-113,61);v.TaskLabels[i].resizeTextMinSize=18;v.TaskLabels[i].resizeTextMaxSize=22;v.TaskLabels[i].fontSize=22;
                Place(v.TaskCounts[i].rectTransform,108,76,taskWidth[i]-125,38);v.TaskCounts[i].fontSize=25;
                var reward=card.Find("RewardPlaque").GetComponent<Image>();reward.sprite=ProductionTeamHubRuntime.Load("task_reward_plaque_r2");
                reward.type=Image.Type.Simple;reward.preserveAspect=false;Place(reward.rectTransform,19,111,taskWidth[i]-38,49);
                var gem=card.Find("RewardGem").GetComponent<Image>();gem.sprite=ProductionGameplayArt.Gem(1);
                Place(gem.rectTransform,taskWidth[i]/2-45,118,31,35);Place(v.TaskRewards[i].rectTransform,taskWidth[i]/2-9,115,74,41);v.TaskRewards[i].fontSize=28;
                if(i==2){var potion=card.Find("Icon").GetComponent<Image>();potion.sprite=ProductionBoosterArt.Icon(3);Place(potion.rectTransform,16,8,78,100);}
            }
            float[] xs={33,252,465,667},ws={215,206,194,241};
            for(int i=0;i<4;i++)
            {
                Place((RectTransform)v.NavButtons[i].transform,xs[i],894,ws[i],134);
                v.NavButtons[i].image.type=Image.Type.Simple;v.NavButtons[i].image.preserveAspect=true;
                v.NavButtons[i].transform.Find("Icon").gameObject.SetActive(false);
                Place(v.NavLabels[i].rectTransform,16,84,ws[i]-32,41);v.NavLabels[i].fontSize=27;
            }
            // Source guild medallions and carved rows; keep member text live.
            for(int i=0;i<5;i++)
            {
                Place(v.MemberRows[i],39,1073+i*91,851,85);
                Place(v.Portraits[i].rectTransform,96,-18,117,109);
                Place(v.MemberNames[i].rectTransform,222,4,353,43);v.MemberNames[i].resizeTextMaxSize=29;v.MemberNames[i].fontSize=29;
                Place(v.MemberRoles[i].rectTransform,222,43,353,32);
                Place(v.MemberScores[i].rectTransform,721,14,113,49);v.MemberScores[i].fontSize=29;
                v.MemberScores[i].alignment=TextAnchor.MiddleLeft;
                var frame=v.MemberRows[i].Find("Frame").GetComponent<Image>();
                frame.sprite=ProductionTeamHubRuntime.Load(i==0?"roster_leader_r3":"roster_member_r3");
                frame.type=Image.Type.Simple;frame.preserveAspect=false;frame.color=Color.white;
                Place(frame.rectTransform,0,0,851,85);
                var badge=v.MemberRows[i].Find("RankIcon").GetComponent<Image>();
                badge.gameObject.SetActive(i<3);
                if(i<3){badge.sprite=ProductionTeamHubRuntime.Load("guild_medal_"+i+"_r3");badge.type=Image.Type.Simple;badge.preserveAspect=true;Place(badge.rectTransform,12,4,81,81);}
                v.MemberRanks[i].enabled=i>=3;
                Place(v.MemberRanks[i].rectTransform,24,8,52,62);v.MemberRanks[i].fontSize=39;
                var contribution=v.MemberRows[i].Find("PowerIcon").GetComponent<Image>();
                contribution.sprite=ProductionTeamHubRuntime.Load("power");
                Place(contribution.rectTransform,658,13,57,59);
            }
            Place(v.ContributionHeading.rectTransform,684,1059,215,23);v.ContributionHeading.fontSize=16;
            v.Body.Find("FooterOrnament").gameObject.SetActive(false);
            var footer=NeedArt(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1537,941,135);
            footer.preserveAspect=false;footer.color=new Color(.5f,.5f,.5f,1);footer.transform.SetAsLastSibling();
            var motto=v.Body.Find("MottoFrame").GetComponent<Image>();
            motto.sprite=ProductionTeamHubRuntime.Load("motto_ru_r4");motto.type=Image.Type.Simple;motto.preserveAspect=true;motto.color=Color.white;
            Place(motto.rectTransform,200,1545,539.2f,79.2f);
            var crest=NeedArt(v.Body,"FooterCrest",ProductionTeamHubRuntime.Load("footer_crest_r4"),129.6f,1537.8f,97.6f,102.4f);
            Place(v.Motto.rectTransform,244,1564,420,46);v.Motto.fontSize=24;v.Motto.resizeTextMinSize=20;v.Motto.resizeTextMaxSize=24;
            Place((RectTransform)v.FriendsButton.transform,755,1576,160,76);
            Place(v.FriendsLabel.rectTransform,10,15,140,46);v.FriendsLabel.resizeTextMinSize=19;v.FriendsLabel.resizeTextMaxSize=24;
            var info=NeedArt(v.Body,"GuildInfo",ProductionTeamHubRuntime.Load("guild_info_r4"),686,1564,48,48);
            // Preserve the 48px source icon; extend input into the clear noninteractive margin.
            info.raycastPadding=new Vector4(-14,-14,-14,-14);
            v.GuildInfoButton=info.GetComponent<Button>()??info.gameObject.AddComponent<Button>();v.GuildInfoButton.targetGraphic=info;info.raycastTarget=true;
            foreach(var t in new[]{v.BackButton.transform,motto.transform,v.Motto.transform,crest.transform,v.GuildInfoButton.transform,v.FriendsButton.transform})t.SetAsLastSibling();
            v.InfoPanel.transform.SetAsLastSibling();
            // Restrained local lettering shadow; shared header/source lettering stay untouched.
            foreach(var text in new[]{v.GuildName,v.MemberCount,v.Membership,v.PowerHeading,v.Power,v.Progress,v.MembersHeading})Lettering(text);
            foreach(var text in v.TaskLabels)Lettering(text);
            foreach(var text in v.TaskCounts)Lettering(text);
            foreach(var text in v.TaskRewards)Lettering(text);
            foreach(var text in v.MemberNames)Lettering(text);
            foreach(var text in v.MemberScores)Lettering(text);
            ProductionMapButtonFeedback.Install(v.transform);
        }
        static void Lettering(Text text)
        {
            var shadow=text.GetComponent<Shadow>()??text.gameObject.AddComponent<Shadow>();
            shadow.effectColor=new Color(0,0,0,.8f);shadow.effectDistance=new Vector2(1,-1);shadow.useGraphicAlpha=true;
        }
        static void Before(Transform decoration,Transform content)
        {
            int target=content.GetSiblingIndex();
            if(decoration.GetSiblingIndex()<target)target--;
            decoration.SetSiblingIndex(target);
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        static Image NeedArt(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {
            var child=p.Find(n);if(child==null){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);child=go.transform;}
            var a=child.GetComponent<Image>()??child.gameObject.AddComponent<Image>();Place(a.rectTransform,x,y,w,h);
            a.sprite=sprite;a.preserveAspect=true;a.raycastTarget=false;a.color=Color.white;return a;
        }
    }
}
