using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Reuses native controls and approved artwork; sample standings remain read-only.
    public static class ProductionTeamRankingReferenceLayout
    {
        public static void Apply(ProductionTeamRankingView v)
        {
            v.Revision=ProductionTeamRankingView.CurrentRevision;
            ProductionPlayerHeader.Attach(v);
            // Original joined groups leave a clear gap for the central rank medallion.
            float[] tabX={13,208,529,728},tabW={195,206,199,188};
            for(int i=0;i<4;i++)
            {
                Place((RectTransform)v.Tabs[i].transform,tabX[i],284,tabW[i],64);
                v.Tabs[i].image.type=Image.Type.Simple;v.Tabs[i].image.preserveAspect=false;
                Place(v.TabLabels[i].rectTransform,i%2==0?31:16,12,i%2==0?tabW[i]-48:tabW[i]-44,40);
                v.TabLabels[i].fontSize=22;v.TabLabels[i].resizeTextForBestFit=true;
                v.TabLabels[i].resizeTextMinSize=18;v.TabLabels[i].resizeTextMaxSize=22;
            }
            Place(v.SeasonNote.rectTransform,150,245,641,24);v.SeasonNote.fontSize=20;
            Place(v.PreviewLabel.rectTransform,155,270,631,17);v.PreviewLabel.fontSize=13;
            for(int i=0;i<3;i++)
            {
                var gem=v.Podiums[i].Find("RewardIcon_1").GetComponent<Image>();
                gem.sprite=ProductionGameplayArt.Gem(1);gem.preserveAspect=true;
                float top=-v.Podiums[i].anchoredPosition.y,width=v.Podiums[i].rect.width;
                float nameInset=i==0?39:32;
                Place(v.PodiumNames[i].rectTransform,nameInset,572-top,width-2*nameInset,32);
                v.PodiumNames[i].fontSize=i==0?26:23;v.PodiumNames[i].resizeTextMinSize=17;
                var source=v.Podiums[i].Find("SourceFrameAndEmblem").GetComponent<Image>();
                source.sprite=ProductionTeamRankingRuntime.Load("podium_"+new[]{"gold","silver","bronze"}[i]+"_r5");
                // Original pictograms/divider are part of the restored opaque stat face.
                var memberIcon=v.Podiums[i].Find("MembersIcon");if(memberIcon!=null)memberIcon.gameObject.SetActive(false);
                float left=v.Podiums[i].anchoredPosition.x;
                Place(v.PodiumMembers[i].rectTransform,new[]{451f,139f,732f}[i]-left,607-top,new[]{98f,100f,100f}[i],32);
                Place(v.PodiumScores[i].rectTransform,new[]{436f,130f,727f}[i]-left,644-top,new[]{134f,113f,114f}[i],35);
                foreach(var stat in new[]{v.PodiumMembers[i],v.PodiumScores[i]})
                {
                    stat.alignment=TextAnchor.MiddleLeft;stat.fontSize=24;
                    stat.resizeTextForBestFit=true;stat.resizeTextMinSize=20;stat.resizeTextMaxSize=24;
                    stat.horizontalOverflow=HorizontalWrapMode.Overflow;stat.verticalOverflow=VerticalWrapMode.Truncate;
                }
                float groupX=new[]{29f,15f,-2f}[i],groupWidth=new[]{279f,242f,240f}[i];
                var enclosure=Enclosure(v.Podiums[i],groupX,684-top,groupWidth,99);
                enclosure.transform.SetSiblingIndex(source.transform.GetSiblingIndex()+1);
                float factor=groupWidth/242f;
                for(int j=0;j<3;j++)
                {
                    float cx=groupX+new[]{42f,115f,193f}[j]*factor,cellWidth=66*factor;
                    var slot=v.Podiums[i].Find("RewardSlot_"+j).GetComponent<Image>();slot.enabled=false;
                    Place(slot.rectTransform,cx-cellWidth*.5f,692-top,cellWidth,87);
                    Place((RectTransform)v.Podiums[i].Find("RewardIcon_"+j),cx-27,698-top,54,51);
                    var count=v.PodiumRewards[i*3+j];Place(count.rectTransform,cx-29,752-top,58,24);
                    count.fontSize=22;count.resizeTextForBestFit=true;count.resizeTextMinSize=18;count.resizeTextMaxSize=22;
                }
                v.PodiumNames[i].resizeTextMaxSize=v.PodiumNames[i].fontSize;
            }
            var table=v.Body.Find("TableFrame").GetComponent<Image>();
            table.sprite=ProductionTeamRankingRuntime.Load("table_panel_r4");table.type=Image.Type.Simple;table.preserveAspect=false;
            Place(table.rectTransform,25,796,891,574);
            for(int i=0;i<7;i++)
            {
                Place(v.Rows[i],30,844+i*74,880,73);
                var rowFrame=v.Rows[i].Find("Frame").GetComponent<Image>();
                rowFrame.sprite=ProductionTeamRankingRuntime.Load(i==3?"row_highlight_r4":"row_divider_r4");rowFrame.type=Image.Type.Simple;rowFrame.preserveAspect=false;rowFrame.color=Color.white;
                Place(rowFrame.rectTransform,0,i==3?0:70,880,i==3?73:3);
                Place(v.Ranks[i].rectTransform,12,15,67,48);
                Place(v.Crests[i].rectTransform,84,-2,82,76);
                Place((RectTransform)v.Rows[i].Find("MembersIcon"),357,22,27,30);
                Place((RectTransform)v.Rows[i].Find("ContributionIcon"),478,15,35,40);
                Place(v.Members[i].rectTransform,387,15,89,44);
                Place(v.Scores[i].rectTransform,515,15,113,44);

                var gem=v.Rows[i].Find("RewardIcon_1").GetComponent<Image>();gem.sprite=ProductionGameplayArt.Gem(1);gem.preserveAspect=true;
                // One continuous source enclosure replaces stacked generic slot frames.
                var enclosure=Enclosure(v.Rows[i],639,6,220,61);enclosure.transform.SetSiblingIndex(v.Rows[i].Find("Frame").GetSiblingIndex()+1);
                for(int j=0;j<3;j++)
                {
                    float cx=639+new[]{42f,115f,193f}[j]*(220f/242f);
                    var slot=v.Rows[i].Find("RewardSlot_"+j).GetComponent<Image>();slot.enabled=false;
                    Place(slot.rectTransform,cx-32,7,64,59);
                    Place((RectTransform)v.Rows[i].Find("RewardIcon_"+j),cx-26,10,52,30);
                    var count=v.Rewards[i*3+j];Place(count.rectTransform,cx-28,44,56,19);
                    count.fontSize=17;count.resizeTextForBestFit=true;count.resizeTextMinSize=15;count.resizeTextMaxSize=17;
                }
                Place(v.Names[i].rectTransform,175,18,176,36);
                v.Names[i].fontSize=22;v.Names[i].resizeTextForBestFit=false;
                v.Names[i].horizontalOverflow=HorizontalWrapMode.Overflow;
                v.Names[i].verticalOverflow=VerticalWrapMode.Truncate;
                v.Members[i].fontSize=25;v.Members[i].resizeTextMaxSize=25;
                v.Scores[i].fontSize=25;v.Scores[i].resizeTextMaxSize=25;
            }
            for(int i=0;i<5;i++)v.ColumnLabels[i].fontSize=20;
            Place((RectTransform)v.Body.Find("PlayerGuildPlaque"),328,1059,367,31);
            Place(v.PlayerGuildLabel.rectTransform,337,1057,349,35);
            Place((RectTransform)v.BackButton.transform,23,1372,72,69);
            // Keep the source arrow unchanged while making narrow-phone edge taps reliable.
            v.BackButton.image.raycastPadding=new Vector4(-4,-6,-4,-6);
            Place((RectTransform)v.DetailsButton.transform,180,1458,593,150);
            v.DetailsButton.image.type=Image.Type.Simple;v.DetailsButton.image.preserveAspect=false;
            Place(v.DetailsLabel.rectTransform,55,47,470,65);v.DetailsLabel.fontSize=34;
            v.DetailsLabel.resizeTextMinSize=26;v.DetailsLabel.resizeTextMaxSize=34;
            var old=v.Body.Find("FooterOrnament");if(old!=null)old.gameObject.SetActive(false);
            var r=v.Body.Find("CarvedFooter") as RectTransform;
            if(r==null){var go=new GameObject("CarvedFooter",typeof(RectTransform),typeof(Image));go.layer=5;go.transform.SetParent(v.Body,false);r=(RectTransform)go.transform;}
            Place(r,0,1537,941,135);if(r.GetComponent<Image>().sprite==null)r.GetComponent<Image>().sprite=Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4");r.gameObject.SetActive(false);
            FooterArt(v,"FooterBannerLeft","banner_left_r8",0,1444,218,228);
            FooterArt(v,"FooterBannerRight","banner_right_r8",728,1444,213,228);
            FooterArt(v,"FooterSourceCarving","footer_carving_r8",198,1597,536,75);
            v.DetailsButton.transform.SetAsLastSibling();v.BackButton.transform.SetAsLastSibling();v.InfoPanel.transform.SetAsLastSibling();
            ProductionMapButtonFeedback.Install(v.transform);
        }
        static void FooterArt(ProductionTeamRankingView v,string name,string sprite,float x,float y,float w,float h)
        {
            var r=v.Body.Find(name) as RectTransform;
            if(r==null){var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.layer=5;go.transform.SetParent(v.Body,false);r=(RectTransform)go.transform;}
            Place(r,x,y,w,h);var image=r.GetComponent<Image>();image.sprite=ProductionTeamRankingRuntime.Load(sprite);image.preserveAspect=true;image.raycastTarget=false;image.color=Color.white;r.SetAsLastSibling();
        }
        static Image Enclosure(Transform parent,float x,float y,float w,float h)
        {
            var t=parent.Find("SourceRewardEnclosure") as RectTransform;
            if(t==null){var go=new GameObject("SourceRewardEnclosure",typeof(RectTransform),typeof(Image));go.layer=5;go.transform.SetParent(parent,false);t=(RectTransform)go.transform;}
            Place(t,x,y,w,h);var image=t.GetComponent<Image>();image.sprite=ProductionTeamRankingRuntime.Load("reward_enclosure_r3");image.type=Image.Type.Simple;image.preserveAspect=false;image.raycastTarget=false;return image;
        }
        static void Place(RectTransform r,float x,float y,float w,float h){r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
    }
}
