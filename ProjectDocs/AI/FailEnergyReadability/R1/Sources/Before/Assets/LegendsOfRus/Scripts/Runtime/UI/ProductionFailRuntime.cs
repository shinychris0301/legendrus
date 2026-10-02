using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionFailRuntime
    {
        public const string ContentName="FAIL_ALIGNED_CONTENT",ArtRoot="ProductionUI/FailAligned/";
        public static readonly string[] ArtNames={"environment_r2","ivan_r2","panel_r2","title_r2","goals_r2","retry_r2","nameplate_r2","medallion_r2","coin_r3","title_r4","panel_r4","paid_r4","ad_r4","paid_r9","ad_r9","environment_r10","ivan_r10","panel_r10","title_r10","goals_r10","nameplate_r10","title_r11","retry_r11","paid_r11","ad_r11","nameplate_r11","goals_r11","heading_r11","mace_r11","firebird_r11","shield_r11","potion_r11","title_ru_r12","retry_ru_r12","name_mace_ru_r12","name_firebird_ru_r12","name_shield_ru_r12","name_potion_ru_r12","environment_source_r13","ivan_source_r13","panel_source_r13","heading_source_ru_r13"};
        public static readonly string[] ReferenceBoosters={"mace_r11","firebird_r11","shield_r11","potion_r11"};
        public static readonly string[] ReferenceNameplatesRu={"name_mace_ru_r12","name_firebird_ru_r12","name_shield_ru_r12","name_potion_ru_r12"};
        public const float SceneTop=-35;
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionFailView>();
            if(old!=null)
            {
                if(!Application.isPlaying){old.Revision=ProductionFailView.CurrentRevision;ApplyOffers(old);}
                if(!old.HasRequiredBindings)throw new InvalidOperationException("Repair Failure bindings outside Play Mode.");
                ApplyPresentation(old);old.RefreshContent();return;
            }
            if(root.Find(ContentName)!=null||root.Find("FIX01_CONTENT")!=null)throw new InvalidOperationException("Repair Failure bindings outside Play Mode.");
            var source=Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>();
            var v=root.gameObject.AddComponent<ProductionFailView>();v.Typography=source.Typography;
            v.Content=Rect(root,ContentName,0,0,941,1672);
            v.Environment=Art(v.Content,"Environment",Load(ArtRoot+"environment_r1"),0,0,941,1672);
            v.Body=Rect(v.Content,"FailureBody",0,150,941,1515);
            v.Hero=Art(v.Body,"Ivan",Load(ArtRoot+"ivan_r1"),190,0,561,660);v.Hero.preserveAspect=true;
            Panel(v.Body,"MainPanel",30,445,881,1060);
            Panel(v.Body,"FailureTitlePanel",80,440,781,185);
            v.Title=Label(v,v.Body,"Title","",115,474,711,75,62);
            v.Subtitle=Label(v,v.Body,"Subtitle","",135,554,671,42,31);
            Panel(v.Body,"GoalsPanel",120,638,701,250);
            v.GoalsHeading=Label(v,v.Body,"GoalsHeading","",160,660,621,38,29);
            v.Score=Label(v,v.Body,"RoundScore","",165,702,611,28,21);
            v.Goals=new PreLevelGoalView[3];
            for(int i=0;i<3;i++)
            {
                var slot=Rect(v.Body,"Goal_"+i,185+i*190,738,190,140);
                v.Goals[i]=new PreLevelGoalView{Root=slot,Icon=Art(slot,"Icon",ProductionGameplayArt.Gem(i),48,0,94,94),Count=Label(v,slot,"Progress","",0,100,190,36,29),Description=Label(v,slot,"Description","",0,0,190,20,18)};
                v.Goals[i].Description.gameObject.SetActive(false);v.Goals[i].Icon.preserveAspect=true;
            }
            var retrySprite=Load("ProductionUI/VictoryAligned/continue_r7");
            float retryW=621,retryH=retryW*retrySprite.rect.height/retrySprite.rect.width;
            v.Retry=Button(Art(v.Body,"Retry",retrySprite,(941-retryW)*.5f,886,retryW,retryH));v.Retry.GetComponent<Image>().preserveAspect=true;
            v.RetryText=Label(v,v.Retry.transform,"Label","",55,55,retryW-110,68,35);
            v.Exit=Button(Panel(v.Body,"Exit",220,1074,501,76));
            v.ExitText=Label(v,v.Exit.transform,"Label","",45,18,411,39,28);
            v.Reaction=Label(v,v.Body,"Reaction","",115,1153,711,55,21);v.Reaction.horizontalOverflow=HorizontalWrapMode.Wrap;
            v.BoostersHeading=Label(v,v.Body,"BoostersHeading","",110,1214,721,38,28);
            v.BoosterCounts=new Text[4];v.BoosterLabels=new Text[4];v.BoosterButtons=new Button[4];
            for(int i=0;i<4;i++)
            {
                var group=Rect(v.Body,"Booster_"+i,102+i*185,1260,182,204);
                var art=Art(group,"Art",ProductionGameplayArt.Booster(i),12,0,140,140);Fit(art,82,70,140,140);v.BoosterButtons[i]=Button(art);
                var badge=Rect(group,"StockBadge",124,107,50,50).gameObject.AddComponent<ProductionPreLevelBadge>();badge.raycastTarget=false;
                v.BoosterCounts[i]=Label(v,badge.transform,"Count","",0,0,50,50,27);
                v.BoosterLabels[i]=Label(v,group,"Name","",0,161,182,44,18);v.BoosterLabels[i].horizontalOverflow=HorizontalWrapMode.Wrap;
            }
            v.HudBacking=Clone(source.HudBacking,v.Content,"HudBacking");v.Hud=Clone(source.Hud,v.Content,"FailureHud");
            v.Profile=v.Hud.Find("ProfileButton").GetComponent<Button>();v.Settings=v.Hud.Find("SettingsButton").GetComponent<Button>();
            v.HeroName=v.Profile.transform.Find("HeroName").GetComponent<Text>();v.Level=v.Profile.transform.Find("SavedLevel").GetComponent<Text>();
            v.Resources=new[]{v.Hud.Find("GoldResource/GoldValue").GetComponent<Text>(),v.Hud.Find("GemsResource/GemsValue").GetComponent<Text>(),v.Hud.Find("EnergyResource/EnergyValue").GetComponent<Text>()};
            v.Plus=new[]{v.Hud.Find("GoldResource/ResourcePlus").GetComponent<Button>(),v.Hud.Find("GemsResource/ResourcePlus").GetComponent<Button>(),v.Hud.Find("EnergyResource/ResourcePlus").GetComponent<Button>()};
            foreach(var text in v.Hud.GetComponentsInChildren<Text>(true))text.raycastTarget=false;
            var layout=root.gameObject.AddComponent<ProductionFailLayout>();layout.View=v;ApplyPresentation(v);v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Failure native bindings incomplete.");
        }
        static void ApplyPresentation(ProductionFailView v)
        {
            ApplyHeader(v);ApplyOffers(v);
            v.Environment.sprite=Load(ArtRoot+"environment_source_r13");
            v.Hero.sprite=Load(ArtRoot+"ivan_source_r13");
            v.Hero.color=Color.white;
            ProductionVictoryView.Place(v.Hero.rectTransform,185,-30,584,489);
            v.Hero.preserveAspect=true;
            // The source-aligned clean plate already places the visible scenery correctly.
            // Retain the legacy object for serialized identity without drawing a second crop.
            var scenery=v.Body.Find("SceneWindow");
            if(scenery!=null)scenery.gameObject.SetActive(false);
            SetPanel(v,"MainPanel","panel_source_r13",15,488,911,1022,false);
            var main=v.Body.Find("MainPanel").GetComponent<Image>();main.color=Color.white;main.preserveAspect=true;
            SetPanel(v,"FailureTitlePanel","title_r11",123,373,695,245,false);
            var titleArt=v.Body.Find("FailureTitlePanel").GetComponent<Image>();titleArt.preserveAspect=true;titleArt.color=Color.white;
            ProductionVictoryView.PlaceText(v.Title,153,451,635,92,76);
            ProductionVictoryView.PlaceText(v.Subtitle,149,542,643,45,34);
            v.Title.fontStyle=FontStyle.Bold;
            Gold(v.Title,1.1f);Gold(v.Subtitle,.65f);
            // Enclose the heading and inset separately, behind all live goal content.
            SetPanel(v,"GoalsOuterPanel","goals_r11",149,648,643,221,true);
            v.Body.Find("GoalsOuterPanel").SetSiblingIndex(v.Body.Find("MainPanel").GetSiblingIndex()+1);
            var outer=v.Body.Find("GoalsOuterPanel").GetComponent<Image>();
            outer.color=Color.white;
            outer.GetComponent<ProductionFailPanelFit>().ProtectedDetailScale=.5f;
            SetPanel(v,"GoalsPanel","goals_r11",169,680,603,179,false);
            var inset=v.Body.Find("GoalsPanel").GetComponent<Image>();inset.preserveAspect=true;inset.color=Color.white;
            SetPanel(v,"GoalsHeadingPlaque","heading_r11",227,627,491,49,false);
            var plaque=v.Body.Find("GoalsHeadingPlaque");
            plaque.SetAsLastSibling();
            plaque.SetSiblingIndex(v.GoalsHeading.transform.GetSiblingIndex());
            plaque.GetComponent<Image>().preserveAspect=true;
            ProductionVictoryView.PlaceText(v.GoalsHeading,236,633,473,37,26);
            ClearLabel(v.GoalsHeading,true);
            // Retain the live round score as metadata, but the reference's hierarchy gives
            // this space to the failed-goal icons. It is not a decorative sample score.
            v.Score.gameObject.SetActive(false);
            foreach(var goal in v.Goals)
                ProductionVictoryView.PlaceText(goal.Count,0,100,190,40,31);
            var retrySprite=Load(ArtRoot+"retry_r11");v.Retry.GetComponent<Image>().sprite=retrySprite;
            float retryWidth=625,retryHeight=retryWidth*retrySprite.rect.height/retrySprite.rect.width;
            ProductionVictoryView.Place((RectTransform)v.Retry.transform,(941-retryWidth)*.5f,877,retryWidth,retryHeight);
            ProductionVictoryView.PlaceText(v.RetryText,35,(retryHeight-60)*.5f,retryWidth-70,60,36);Gold(v.RetryText,.8f);
            var exit=v.Exit.GetComponent<Image>();exit.sprite=Load(ArtRoot+"goals_r10");
            ProductionVictoryView.Place((RectTransform)v.Exit.transform,642,1158,200,56);
            ProductionVictoryView.PlaceText(v.ExitText,10,5,180,46,19);
            v.ExitText.horizontalOverflow=HorizontalWrapMode.Wrap;
            ProductionVictoryView.PlaceText(v.Reaction,104,1158,518,49,18);
            v.Reaction.horizontalOverflow=HorizontalWrapMode.Wrap;
            v.Reaction.gameObject.SetActive(false);
            SetPanel(v,"BoostersPanel","goals_r11",72,1145,797,300,true);
            ProductionVictoryView.PlaceText(v.BoostersHeading,104,1160,518,40,26);
            for(int i=0;i<4;i++)
            {
                var group=v.Body.Find("Booster_"+i) as RectTransform;
                ProductionVictoryView.Place(group,83+i*194,1210,194,234);
                var frame=EnsureArt(group,"Medallion",Load(ArtRoot+"medallion_r2"),9,0,176,176);frame.transform.SetAsFirstSibling();
                frame.preserveAspect=true;frame.gameObject.SetActive(false);
                frame.color=new Color32(205,199,184,255);
                // Keep a generous interactive rectangle while preserving the whole booster illustration.
                var art=v.BoosterButtons[i].GetComponent<Image>();
                art.sprite=ProductionBoosterArt.Icon(i,true);
                float scale=184/art.sprite.rect.width;
                ProductionVictoryView.Place(art.rectTransform,5,6,184,art.sprite.rect.height*scale);art.preserveAspect=true;art.color=Color.white;
                // Source stock/plus circles are removed; use a live native stock badge.
                var badge=group.Find("StockBadge") as RectTransform;
                badge.GetComponent<ProductionPreLevelBadge>().enabled=true;
                float[] badgeCenterX={150,150,150,150};
                ProductionVictoryView.Place(badge,5+(badgeCenterX[i]-22)*scale,6+131*scale,44*scale,44*scale);
                ProductionVictoryView.PlaceText(v.BoosterCounts[i],0,0,44*scale,44*scale,28);
                ProductionPreLevelBadge.AlignCount(v.BoosterCounts[i],badge);
                var plate=EnsureArt(group,"Nameplate",Load(ArtRoot+"nameplate_r11"),0,178,194,56);
                var fit=plate.GetComponent<ProductionFailPanelFit>();if(fit!=null)fit.enabled=false;
                plate.preserveAspect=true;plate.color=Color.white;
                plate.transform.SetSiblingIndex(1);
                ProductionVictoryView.PlaceText(v.BoosterLabels[i],10,182,174,48,20);
                ClearLabel(v.BoosterLabels[i],false);
                v.BoosterLabels[i].transform.SetAsLastSibling();
            }
        }
        internal static void ApplyLocalizedArtwork(ProductionFailView v,bool russian,bool hasFailure)
        {
            // R12 retains original fixed Russian lettering with the artwork. Keep
            // Text bindings/content for localization, but render exactly one label layer.
            bool sourceTitle=russian&&hasFailure;
            v.Body.Find("FailureTitlePanel").GetComponent<Image>().sprite=Load(ArtRoot+(sourceTitle?"title_ru_r12":"title_r11"));
            v.Title.enabled=v.Subtitle.enabled=!sourceTitle;
            v.Retry.GetComponent<Image>().sprite=Load(ArtRoot+(russian?"retry_ru_r12":"retry_r11"));
            v.RetryText.enabled=!russian;
            bool sourceHeading=russian&&hasFailure&&v.GoalsHeading.text=="НЕ ВЫПОЛНЕНЫ ЦЕЛИ";
            v.Body.Find("GoalsHeadingPlaque").GetComponent<Image>().sprite=Load(ArtRoot+(sourceHeading?"heading_source_ru_r13":"heading_r11"));
            v.GoalsHeading.enabled=!sourceHeading;
            for(int i=0;i<4;i++)
            {
                v.Body.Find("Booster_"+i+"/Nameplate").GetComponent<Image>().sprite=Load(ArtRoot+(russian?ReferenceNameplatesRu[i]:"nameplate_r11"));
                v.BoosterLabels[i].enabled=!russian;
            }
        }
        internal static void ApplyOffers(ProductionFailView v)
        {
            if(v.Body==null)return;
            // Source-extracted whole cards retain native proportions; baked labels were
            // repaired locally, original gem/lightning/play artwork stays in each sprite.
            var paidSprite=Load(ArtRoot+"paid_r11");var adSprite=Load(ArtRoot+"ad_r11");
            var paid=EnsureArt(v.Body,"PaidMovesOffer",paidSprite,105,992,324,324f*149/328);
            var ad=EnsureArt(v.Body,"RewardedContinueOffer",adSprite,512,992,324,324f*149/329);
            foreach(var card in new[]{paid,ad})
            {
                var fit=card.GetComponent<ProductionFailPanelFit>()??card.gameObject.AddComponent<ProductionFailPanelFit>();
                fit.enabled=false;card.color=Color.white;
            }
            v.PaidOffer=paid.GetComponent<Button>()??Button(paid);
            v.AdOffer=ad.GetComponent<Button>()??Button(ad);
            foreach(var b in new[]{v.PaidOffer,v.AdOffer})
            {
                b.interactable=false;b.navigation=new Navigation{mode=Navigation.Mode.None};
                var colors=b.colors;colors.disabledColor=new Color(.82f,.82f,.82f,1);b.colors=colors;
                b.GetComponent<Image>().preserveAspect=true;b.GetComponent<Image>().raycastTarget=true;
            }
            v.PaidOfferTitle=EnsureLabel(v,paid.transform,"Title",42,16,215,33,28);
            v.AdOfferTitle=EnsureLabel(v,ad.transform,"Title",35,12,254,43,21);
            v.AdOfferTitle.horizontalOverflow=HorizontalWrapMode.Wrap;
            v.PaidOfferStatus=EnsureLabel(v,paid.transform,"Availability",42,53,240,22,18);
            var gem=EnsureArt(paid.transform,"Currency",ProductionGameplayArt.Gem(2),110,90,43,43);gem.preserveAspect=true;gem.enabled=false;
            EnsureLabel(v,paid.transform,"Price",161,92,48,36,28).text="—";
            var lightning=EnsureArt(paid.transform,"MovesSymbol",Load(ProductionHomeRuntime.ArtRoot+"energy_icon"),256,12,40,40);lightning.preserveAspect=true;lightning.enabled=false;
            v.AdOfferStatus=EnsureLabel(v,ad.transform,"Availability",42,56,240,22,18);
            ClearLabel(v.PaidOfferStatus,true);ClearLabel(v.AdOfferStatus,true);
            var play=EnsureLabel(v,ad.transform,"PlaySymbol",138,91,43,36,32);play.text="";play.enabled=false;
            v.OfferSeparator=EnsureLabel(v,v.Body,"OffersSeparator",435,1044,71,40,24);
        }
        static Text EnsureLabel(ProductionFailView v,Transform parent,string name,float x,float y,float w,float h,int size)
        {
            var found=parent.Find(name);var text=found!=null?found.GetComponent<Text>():Label(v,parent,name,"",x,y,w,h,size);
            ProductionVictoryView.PlaceText(text,x,y,w,h,size);return text;
        }
        static void ApplyHeader(ProductionFailView v)
        {
            ProductionPlayerHeader.Attach(v);
        }
        static Image EnsureArt(Transform parent,string name,Sprite sprite,float x,float y,float w,float h)
        {
            var found=parent.Find(name);var image=found!=null?found.GetComponent<Image>():Art(parent,name,sprite,x,y,w,h);
            image.sprite=sprite;image.raycastTarget=false;ProductionVictoryView.Place(image.rectTransform,x,y,w,h);return image;
        }
        static void SetPanel(ProductionFailView v,string name,string asset,float x,float y,float w,float h,bool fitted)
        {
            var image=EnsureArt(v.Body,name,Load(ArtRoot+asset),x,y,w,h);
            if(name=="MainPanel")image.color=new Color32(200,200,200,255);
            else if(name=="FailureTitlePanel")image.color=new Color32(214,214,214,255);
            else if(name=="GoalsPanel"||name=="BoostersPanel")image.color=new Color32(220,220,220,255);
            var fit=image.GetComponent<ProductionFailPanelFit>();if(fit==null&&fitted)fit=image.gameObject.AddComponent<ProductionFailPanelFit>();
            if(fit!=null)fit.enabled=fitted;
            if(name=="BoostersPanel")image.transform.SetSiblingIndex(v.Body.Find("MainPanel").GetSiblingIndex()+1);
        }
        static void ClearLabel(Text text,bool bold)
        {
            text.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;
            text.color=new Color32(255,236,192,255);
            // A compact dark edge supports small live glyphs without a broad glow.
            var shadow=text.GetComponent<Shadow>();
            if(shadow!=null){shadow.enabled=true;shadow.effectColor=new Color32(5,9,14,255);shadow.effectDistance=new Vector2(.6f,-.6f);}
        }
        static void Gold(Text text,float bevel)
        {
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            text.material=home.PlayText.material;
            var shadow=text.GetComponent<Shadow>();if(shadow!=null)shadow.enabled=false;
            var gold=text.GetComponent<ProductionVictoryTitleLettering>();if(gold==null)gold=text.gameObject.AddComponent<ProductionVictoryTitleLettering>();
            gold.ArcHeight=0;gold.BevelWidth=bevel;gold.ShadowDistance=new Vector2(1.6f,-2.4f);gold.ExtrusionDistance=new Vector2(.7f,-1);
            gold.BottomColor=new Color32(152,91,21,255);gold.BodyColor=new Color32(217,167,67,255);
            gold.HighlightColor=new Color32(255,235,174,255);gold.TopColor=new Color32(235,198,111,255);gold.Refresh();
        }
        public static void Fit(Image image,float centerX,float centerY,float width,float height)
        {
            Vector2 n=image.sprite.rect.size;float scale=Mathf.Min(width/n.x,height/n.y);
            image.rectTransform.anchoredPosition=new Vector2(centerX-n.x*scale*.5f,-centerY+n.y*scale*.5f);image.rectTransform.sizeDelta=n*scale;
        }
        static Sprite Load(string path)=>Resources.Load<Sprite>(path)??throw new InvalidOperationException("Missing Failure sprite: "+path);
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)=>ProductionChapterMapRuntime.Rect(p,n,x,y,w,h);
        static RectTransform Clone(RectTransform r,Transform p,string name){var clone=UnityEngine.Object.Instantiate(r,p,false);clone.name=name;return clone;}
        static Image Art(Transform p,string n,Sprite sprite,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=sprite;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,float x,float y,float w,float h){var a=Art(p,n,Load(ProductionPreLevelRuntime.ArtRoot+"panel_wide"),x,y,w,h);a.gameObject.AddComponent<ProductionFailPanelFit>();return a;}
        static Button Button(Image image){image.raycastTarget=true;var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionFailView v,Transform p,string n,string value,float x,float y,float w,float h,int size)
        {
            var t=Rect(p,n,x,y,w*2,h*2).gameObject.AddComponent<Text>();t.rectTransform.localScale=Vector3.one*.5f;t.font=v.Typography.RegularFont;t.fontSize=size*2;t.text=value;
            t.color=new Color32(255,226,162,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;
            t.gameObject.AddComponent<SplashTitleTextGeometry>().SnapToScreenPixels=true;var shadow=t.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(10,6,2,220);shadow.effectDistance=new Vector2(1,-1);return t;
        }
    }
}
