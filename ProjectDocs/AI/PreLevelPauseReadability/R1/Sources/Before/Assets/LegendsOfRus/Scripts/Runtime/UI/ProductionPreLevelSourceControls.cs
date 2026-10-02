using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Original isolated Pre-Level panels with native values and existing preparation actions.</summary>
    public static class ProductionPreLevelSourceControls
    {
        public const string ArtRoot="ProductionUI/PreLevelReferenceAligned/";
        public static readonly string[] ArtNames={"goals","moves","boosters","rewards","mace","firebird","shield","potion","chest","environment"};
        public static Sprite Load(string name)
        {
            var shared=ProductionBoosterArt.Replacement(ArtRoot+name);if(shared!=null)return shared;
#if UNITY_EDITOR
            if(!Application.isPlaying)return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/LegendsOfRus/Resources/"+ArtRoot+name+".png");
#endif
            return Resources.Load<Sprite>(ArtRoot+name);
        }
        public static void Apply(ProductionPreLevelView v)
        {
            foreach(var g in v.HudBacking.GetComponentsInChildren<Graphic>(true))g.enabled=false;
            ProductionPlayerHeader.Attach(v);
            v.Environment.sprite=Load("environment");v.Environment.color=Color.white;
            foreach(var s in v.Environment.GetComponentsInChildren<ProductionHomeBackdropShade>(true))s.enabled=false;
            Place(v.TitleArea,0,116,941,273);
            foreach(var g in v.TitleArea.GetComponentsInChildren<Graphic>(true))g.enabled=false;
            v.TitlePlaque=NeedImage(v.TitleArea,"OriginalBanner");Art(v.TitlePlaque,Chapter("banner"),77,0,798,273);
            Label(v.ChapterText,250,75,455,38,28,"Title");Label(v.TitleText,194,112,557,65,42,"Title");
            Place(v.Body,0,382,941,1078);
            Place(v.GoalsArea,208,0,529,353);Art(v.GoalsArea.Find("GoalsPanel").GetComponent<Image>(),Load("goals"),0,0,529,353);
            Label(v.GoalsHeading,180,68,170,45,31);Label(v.SelectedLevel,155,108,220,25,18);
            foreach(var goal in v.Goals){Label(goal.Count,0,112,136,49,39,"RankNumbers");goal.Description.enabled=false;}
            Place(v.MovesArea,365,293,214,206);Art(v.MovesArea.Find("MovesMedallion").GetComponent<Image>(),Load("moves"),0,0,214,206);
            Label(v.MovesHeading,40,39,134,39,27);Label(v.MovesValue,36,73,142,92,72,"RankNumbers");
            Place(v.BoostersArea,120,437,703,320);Art(v.BoostersArea.Find("BoostersPanel").GetComponent<Image>(),Load("boosters"),0,0,703,320);
            Label(v.BoostersHeading,42,66,619,43,29);
            v.BoostersHeading.resizeTextForBestFit=true;v.BoostersHeading.resizeTextMinSize=30;v.BoostersHeading.resizeTextMaxSize=58;
            string[] names={"mace","firebird","shield","potion"};float[] xs={27,187,346,503};float[] badgeXs={103,103,94,93};
            for(int i=0;i<4;i++)
            {
                var button=v.BoosterButtons[i];var face=button.GetComponent<Image>();Art(face,Load(names[i]),xs[i],106,180,180);face.raycastTarget=true;button.targetGraphic=face;
                var badge=v.BoosterCounts[i].transform.parent as RectTransform;Place(badge,badgeXs[i],111,60,60);
                Label(v.BoosterCounts[i],0,0,60,60,26,"RankNumbers");v.BoosterCounts[i].resizeTextForBestFit=true;v.BoosterCounts[i].resizeTextMinSize=24;v.BoosterCounts[i].resizeTextMaxSize=52;
                ProductionPreLevelBadge.AlignCount(v.BoosterCounts[i],badge);
            }
            Place(v.BoosterDetailsRoot,30,114,643,193);
            Art(v.BoosterDetailIcon,Load("mace"),0,0,130,130);
            Label(v.BoosterDetailText,155,0,460,90,20);v.BoosterDetailText.alignment=TextAnchor.MiddleLeft;
            Control(v.BoosterDetailsBack,Chapter("play_blank"),160,106,210,78);
            Control(v.BoosterCatalog,Chapter("play_blank"),405,106,210,78);
            Label(v.BoosterDetailsBackText,25,20,160,34,21);Label(v.BoosterCatalogText,20,20,170,34,19);
            Place(v.RewardsArea,122,758,698,320);Art(v.RewardsArea.Find("RewardsPanel").GetComponent<Image>(),Load("rewards"),0,0,698,320);
            Label(v.RewardsHeading,144,23,410,42,29);Label(v.RewardStatus,335,67,327,23,16);
            Art(v.RewardsArea.Find("Chest").GetComponent<Image>(),Load("chest"),110,78,218,208);
            var coin=v.RewardsArea.Find("Coin").GetComponent<Image>();Art(coin,coin.sprite,382,94,70,70);
            var gem=v.RewardsArea.Find("Gem").GetComponent<Image>();Art(gem,gem.sprite,382,164,70,70);
            Label(v.RewardGold,480,93,164,54,39,"RankNumbers");v.RewardGold.alignment=TextAnchor.MiddleLeft;
            Label(v.RewardGems,480,167,164,52,39,"RankNumbers");v.RewardGems.alignment=TextAnchor.MiddleLeft;
            foreach(var count in v.BonusCounts)Label(count,38,0,43,38,23,"RankNumbers");
            Place(v.Footer,0,1440,941,232);
            Control(v.BackButton,Chapter("back"),8,32,174,156);
            v.SourcePlay=Control(v.PlayButton,Chapter("play"),232,20,481,203);
            Label(v.PlayText,85,43,310,85,48,"Title");
            Art(v.FooterOrnament,Chapter("footer"),0,1590,941,82);
            ProductionMapButtonFeedback.Install(v.transform);
            v.SourceControls=true;v.Revision=ProductionPreLevelView.CurrentRevision;v.RefreshContent();v.GetComponent<ProductionPreLevelLayout>().ApplyNow();
        }
        public static void RefreshLanguage(ProductionPreLevelView v)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            v.TitlePlaque.sprite=Chapter(en?"banner_blank":"banner");v.SourcePlay.sprite=Chapter(en?"play_blank":"play");
            v.ChapterText.enabled=v.TitleText.enabled=v.PlayText.enabled=en;
        }
        public static void RefreshState(ProductionPreLevelView v,int count)
        {
            if(v.CampaignSaveProvider!=null)v.Header.RefreshValues(v.CampaignSaveProvider()?.player);else v.Header.RefreshValues();
            float stride=count==3?146:188;
            for(int i=0;i<count&&i<v.Goals.Length;i++)
            {
                var g=v.Goals[i];Place(g.Root,264.5f-68-(count-1)*stride*.5f+i*stride,136,136,170);
                Place(g.Icon.rectTransform,18,0,100,112);
            }
            int slot=0;
            foreach(var bonus in v.BonusRoots)if(bonus.gameObject.activeSelf)bonus.anchoredPosition=new Vector2(335+slot++*83,-229);
        }
        static Sprite Chapter(string n)=>Resources.Load<Sprite>(ProductionChapterMapSourceControls.ArtRoot+n);
        static Image NeedImage(Transform parent,string name)
        {
            var rt=parent.Find(name) as RectTransform;if(!rt)rt=ProductionChapterMapRuntime.Rect(parent,name,0,0,1,1);
            var g=rt.GetComponent<Image>();return g?g:rt.gameObject.AddComponent<Image>();
        }
        static Image Control(Button b,Sprite sprite,float x,float y,float w,float h)
        {
            foreach(var g in b.GetComponentsInChildren<Graphic>(true))g.enabled=false;
            foreach(var e in b.GetComponentsInChildren<BaseMeshEffect>(true))e.enabled=false;
            Place((RectTransform)b.transform,x,y,w,h);
            var image=b.GetComponent<Image>();if(!image)image=NeedImage(b.transform,"OriginalSourceArtwork");
            if(image.transform==b.transform){image.sprite=sprite;image.enabled=true;image.color=Color.white;image.preserveAspect=true;}
            else Art(image,sprite,0,0,w,h);
            image.raycastTarget=true;b.targetGraphic=image;b.transition=Selectable.Transition.None;return image;
        }
        static void Art(Image image,Sprite sprite,float x,float y,float w,float h)
        {
            Place(image.rectTransform,x,y,w,h);image.sprite=sprite;image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=true;image.raycastTarget=false;image.enabled=true;
        }
        public static void Place(RectTransform r,float x,float y,float w,float h)
        {
            r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);
        }
        static void Label(Text t,float x,float y,float w,float h,int size,string font="PlayerNames")
        {
            foreach(var effect in t.GetComponents<BaseMeshEffect>())effect.enabled=false;
            Place(t.rectTransform,x,y,w*2,h*2);t.rectTransform.localScale=new Vector3(.5f,.5f,1);
            t.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/"+font);t.fontSize=size*2;t.fontStyle=FontStyle.Normal;t.material=null;t.color=new Color32(245,214,153,255);t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;t.enabled=true;t.transform.SetAsLastSibling();
        }
    }
}
