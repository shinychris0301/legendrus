using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionChapterCompleteRuntime
    {
        public const string ContentName="CHAPTER_COMPLETE_ALIGNED_CONTENT", ArtRoot="ProductionUI/ChapterCompleteAligned/";
        public static readonly string[] ArtNames={"environment","header_ru","header_blank","banner_ru","banner_blank","foreground_ru","foreground_blank","footer","teaser_ru","teaser_blank","reward_card","coin","gem","next_ru","next_blank","map_ru","map_blank","rewards_heading_ru","rewards_heading_blank","reward_divider","scene_rail_left","scene_rail_right","environment_source_r8","next_heading_ru_r8","banner_blank_r8","foreground_blank_r8","next_blank_r8","map_blank_r8","teaser_blank_r8"};
        public static Sprite Load(string name)=>Resources.Load<Sprite>(ArtRoot+name);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionChapterCompleteView>();
            if(old!=null){if(!old.HasRequiredBindings)throw new InvalidOperationException("Repair Chapter Complete bindings outside Play Mode.");old.RefreshContent();return;}
            if(root.Find(ContentName)!=null||root.Find("FIX01_CONTENT")!=null)throw new InvalidOperationException("Repair legacy Chapter Complete outside Play Mode.");
            var v=root.gameObject.AddComponent<ProductionChapterCompleteView>();
            v.Typography=Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>().Typography;
            v.Content=Rect(root,ContentName,0,0,941,1672);
            v.Environment=Art(v.Content,"CleanVillagePlate","environment",0,0,1672,1672);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"CarvedWoodFooter","footer",5,1037,931,635);
            v.Header=Art(v.Body,"Header","header_ru",0,0,941,193);
            v.Banner=Art(v.Body,"ChapterRibbon","banner_ru",156,194,631,221);
            v.Foreground=Art(v.Body,"IvanFirebirdCeremonialCrest","foreground_ru",29,392,789,695);
            v.HeaderText=Label(v,v.Body,"HeaderText",150,65,640,75,48);
            v.BannerText=Label(v,v.Body,"ChapterText",235,243,480,104,35);
            v.VictoryText=Label(v,v.Body,"VictoryText",357,1030,225,55,36);
            v.Summary=Label(v,v.Body,"ActualCompletion",140,1080,661,29,24);
            v.RewardsHeading=Label(v,v.Body,"RewardsHeading",100,1110,741,31,25);
            v.RewardIcons=new Image[6];v.RewardCounts=new Text[6];
            for(int i=0;i<6;i++)
            {
                var card=Art(v.Body,"EarnedReward_"+i,"reward_card",80+i*131,1145,124,119);
                // The source tile is shown at its native aspect ratio, with live contents.
                v.RewardIcons[i]=Art(card.transform,"Icon",i==0?"coin":i==1?"gem":null,28,11,68,68);
                if(i>=2)v.RewardIcons[i].sprite=ProductionGameplayArt.Booster(i-2);
                v.RewardCounts[i]=Label(v,card.transform,"Received",9,80,106,32,27);
            }
            v.Actions=new Button[2];v.ActionLabels=new Text[2];
            for(int i=0;i<2;i++)
            {
                var img=Art(v.Body,i==0?"Continue":"WorldMap",i==0?"next_ru":"map_ru",i==0?79:475,1269,i==0?390:382,94);img.raycastTarget=true;
                var b=img.gameObject.AddComponent<Button>();b.targetGraphic=img;b.navigation=new Navigation{mode=Navigation.Mode.None};v.Actions[i]=b;
                v.ActionLabels[i]=Label(v,img.transform,"Label",48,17,290,62,36);
            }
            v.RewardNote=Label(v,v.Body,"RewardExplanation",90,1365,761,25,21);
            v.NextHeading=Label(v,v.Body,"NextChapterHeading",100,1397,741,29,26);
            v.Teaser=Art(v.Body,"ChapterTwoTeaser","teaser_ru",72,1426,790,201);
            v.TeaserText=Label(v,v.Teaser.transform,"TeaserText",72,20,425,151,24);v.TeaserText.color=new Color32(48,29,12,255);
            ApplyRefinement(v);
            EnsureScene(v);
            var layout=root.gameObject.AddComponent<ProductionChapterCompleteLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Chapter Complete bindings incomplete.");
        }
#if UNITY_EDITOR
        // Preserve all existing serialized objects while the owning editor builder advances the visual revision.
        public static void UpgradeSerializedForEditor(RectTransform root)
        {
            if(Application.isPlaying)throw new InvalidOperationException("Chapter Complete upgrade requires an idle Editor.");
            var view=root.GetComponent<ProductionChapterCompleteView>();if(view!=null)ApplyRefinement(view);
        }
#endif
        static void ApplyRefinement(ProductionChapterCompleteView v)
        {
            v.Environment.sprite=Load("environment_source_r8");
            var scene=v.Body.Find("VillageScene");if(scene!=null)scene.GetComponent<Image>().sprite=v.Environment.sprite;
            if(v.Body.Find("NextHeadingOriginal")==null)Art(v.Body,"NextHeadingOriginal","next_heading_ru_r8",302,1393,339,35);
            if(v.SceneRailLeft==null)v.SceneRailLeft=Art(v.Body,"SceneRailLeft","scene_rail_left",0,200,75,840);
            if(v.SceneRailRight==null)v.SceneRailRight=Art(v.Body,"SceneRailRight","scene_rail_right",866,200,75,840);
            // Source-coordinate rails sit behind the existing header, figure and footer.
            var footer=v.Body.Find("CarvedWoodFooter");
            v.SceneRailLeft.transform.SetSiblingIndex(footer.GetSiblingIndex());
            v.SceneRailRight.transform.SetSiblingIndex(footer.GetSiblingIndex());
            v.Header.rectTransform.sizeDelta=new Vector2(941,390);
            v.Foreground.rectTransform.sizeDelta=new Vector2(883,695);
            if(v.RewardsHeaderArt==null)v.RewardsHeaderArt=Art(v.Body,"RewardsHeaderArt","rewards_heading_ru",150,1082,642,44);
            if(v.RewardsDivider==null)v.RewardsDivider=Art(v.Body,"RewardsDivider","reward_divider",71,1390,802,14);
            SetRect(v.RewardsHeading.rectTransform,370,1084,201,41);v.RewardsHeading.fontSize=36;
            SetRect(v.Summary.rectTransform,75,1364,791,31);v.Summary.fontSize=25;
            // One live received-reward/progress line replaces the two competing explanatory lines.
            v.RewardNote.enabled=false;
            for(int i=0;i<6;i++)
            {
                SetRect((RectTransform)v.RewardIcons[i].transform.parent,48.5f+i*142,1126,134,139);
                SetRect(v.RewardIcons[i].rectTransform,27,10,80,80);
                SetRect(v.RewardCounts[i].rectTransform,9,95,116,37);v.RewardCounts[i].fontSize=30;
            }
            // Divider sits above the next-chapter caption without crossing its live text.
            v.RewardsDivider.rectTransform.anchoredPosition=new Vector2(71,-1390);
            v.RewardsDivider.transform.SetSiblingIndex(v.Teaser.transform.GetSiblingIndex());
            v.RewardsHeaderArt.transform.SetSiblingIndex(v.RewardsHeading.transform.GetSiblingIndex());
            // Separate live localized roles reproduce the reference teaser hierarchy.
            SetRect(v.TeaserText.rectTransform,87,20,420,28);v.TeaserText.fontSize=22;
            if(v.TeaserTitle==null)v.TeaserTitle=Label(v,v.Teaser.transform,"TeaserTitle",87,51,420,77,33);
            if(v.TeaserDescription==null)v.TeaserDescription=Label(v,v.Teaser.transform,"TeaserDescription",87,132,420,57,22);
            SetRect(v.TeaserTitle.rectTransform,87,51,420,77);v.TeaserTitle.fontSize=33;
            SetRect(v.TeaserDescription.rectTransform,87,132,420,57);v.TeaserDescription.fontSize=22;
            foreach(var text in new[]{v.TeaserText,v.TeaserTitle,v.TeaserDescription})
            {
                text.alignment=TextAnchor.UpperLeft;text.color=new Color32(48,29,12,255);
            }
            // Larger live captions share one bilingual layout; the original frame's
            // corners stay at native size through nine-slicing.
            var frame=footer.GetComponent<Image>();frame.type=Image.Type.Sliced;frame.preserveAspect=false;
            SetRect(frame.rectTransform,5,807,931,865);
            SetRect(v.Foreground.rectTransform,196.5f,392,548,431.33f);
            SetRect(v.VictoryText.rectTransform,388,782,164,40);v.VictoryText.fontSize=26;
            SetRect(v.RewardsHeaderArt.rectTransform,150,832,642,44);
            SetRect(v.RewardsHeading.rectTransform,200,828,541,52);v.RewardsHeading.fontSize=40;
            for(int i=0;i<6;i++)
            {
                SetRect((RectTransform)v.RewardIcons[i].transform.parent,48.5f+i*142,881,134,139);
                SetRect(v.RewardIcons[i].rectTransform,33,8,68,68);
                SetRect(v.RewardCounts[i].rectTransform,5,80,124,48);v.RewardCounts[i].fontSize=40;
            }
            for(int i=0;i<2;i++)v.Actions[i].GetComponent<RectTransform>().anchoredPosition=new Vector2(i==0?79:475,-1028);
            SetRect(v.Summary.rectTransform,60,1128,821,96);v.Summary.fontSize=40;
            SetRect(v.RewardsDivider.rectTransform,71,1225,802,14);
            SetRect(v.NextHeading.rectTransform,60,1240,821,47);v.NextHeading.fontSize=38;
            SetRect(v.Teaser.rectTransform,72,1290,790,201);
            SetRect(v.TeaserText.rectTransform,65,20,425,43);v.TeaserText.fontSize=34;
            SetRect(v.TeaserTitle.rectTransform,65,65,425,111);v.TeaserTitle.fontSize=42;
            v.TeaserDescription.transform.SetParent(v.Body,false);
            SetRect(v.TeaserDescription.rectTransform,60,1495,821,96);v.TeaserDescription.fontSize=38;
            v.TeaserDescription.alignment=TextAnchor.MiddleCenter;v.TeaserDescription.color=new Color32(245,210,144,255);
            v.Revision=ProductionChapterCompleteView.CurrentRevision;
        }
        static void SetRect(RectTransform r,float x,float y,float w,float h)
        {
            r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);
        }
        static void EnsureScene(ProductionChapterCompleteView v)
        {
            if(v.Body.Find("VillageScene")==null){var scene=Art(v.Body,"VillageScene","environment_source_r8",0,150,941,941);scene.transform.SetAsFirstSibling();}
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;
            r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        static Image Art(Transform parent,string name,string sprite,float x,float y,float w,float h)
        {
            var i=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();i.sprite=sprite==null?null:Load(sprite);i.preserveAspect=true;i.raycastTarget=false;return i;
        }
        static Text Label(ProductionChapterCompleteView v,Transform parent,string name,float x,float y,float w,float h,int size)
        {
            var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,210,144,255);t.alignment=TextAnchor.MiddleCenter;
            t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;
        }
    }
}
