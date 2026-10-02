using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionVictoryRuntime
    {
        public const string ContentName="VICTORY_ALIGNED_CONTENT",ArtRoot="ProductionUI/VictoryAligned/";
        public static readonly string[] ArtNames={"banner_r4","continue_r7","video_r5","footer_r5","rewards_panel_r2","environment_r3","firebird_r7","resource_header_r3"};
        public static Sprite Home(string name)=>Load(ProductionHomeRuntime.ArtRoot+name);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionVictoryView>();
            if(existing!=null&&existing.HasBaseBindings){ApplyCurrentStyle(existing);existing.RefreshContent();return;}
            if(root.Find(ContentName)!=null)throw new InvalidOperationException("Repair Victory bindings outside Play Mode.");
            var legacy=root.Find("FIX01_CONTENT");if(legacy!=null)throw new InvalidOperationException("Victory legacy content must be removed by its editor builder.");
            var source=Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>();
            var view=root.gameObject.AddComponent<ProductionVictoryView>();view.Typography=source.Typography;
            view.Content=Rect(root,ContentName,0,0,941,1672);
            view.Environment=Art(view.Content,"Environment",Load(ArtRoot+"environment_r3"),0,0,941,1672);
            view.Hud=Rect(view.Content,"Resources",0,0,941,135);
            Art(view.Hud,"HudFrame",Load(ArtRoot+"resource_header_r3"),0,0,941,130);
            view.ResourceButtons=new Button[3];view.ResourceValues=new Text[3];
            string[] icons={"coin_icon","gems_icon","energy_icon"};
            for(int i=0;i<3;i++)
            {
                var capsule=Rect(view.Hud,"Resource_"+i,64+i*278,33,255,70);
                Art(capsule,"Capsule",Home("resource_capsule"),0,0,255,70);
                Art(capsule,"Icon",Home(icons[i]),-9,-4,75,75).preserveAspect=true;
                view.ResourceValues[i]=Text(view,capsule,"Value","—",57,12,141,45,28);
                view.ResourceButtons[i]=Button(Art(capsule,"Plus",Home("resource_plus"),195,5,58,58));
            }
            view.Body=Rect(view.Content,"VictoryBody",0,140,941,1170);
            view.Firebird=Art(view.Body,"Firebird",Load(ArtRoot+"firebird_r7"),198,-5,545,525);view.Firebird.preserveAspect=true;
            Art(view.Body,"VictoryBanner",Load(ArtRoot+"banner_r4"),20,375,901,425);
            view.Title=Text(view,view.Body,"VictoryTitle","ПОБЕДА!",170,473,601,104,78);
            view.Stars=new ProductionWorldMapGraphic[3];view.StarGlows=new ProductionWorldMapGraphic[3];
            for(int i=0;i<3;i++)
            {
                float y=i==1?674:694;
                view.StarGlows[i]=Graphic(view.Body,"StarGlow_"+i,WorldMapGraphicKind.Halo,228+i*158,y-24,170,170);view.StarGlows[i].color=new Color(1,.62f,.12f,.20f);
                view.Stars[i]=Graphic(view.Body,"Star_"+i,WorldMapGraphicKind.Star,250+i*158,y,126,126);
                view.Stars[i].gameObject.AddComponent<ProductionGameplayStarPolish>();
            }
            view.Score=Text(view,view.Body,"Score","",150,822,641,40,28);
            view.Reaction=Text(view,view.Body,"Reaction","",110,866,721,52,20);view.Reaction.horizontalOverflow=HorizontalWrapMode.Wrap;
            view.RewardsArea=Rect(view.Body,"Rewards",0,919,941,286);
            view.RewardsHeading=Text(view,view.RewardsArea,"Heading","ВАШИ НАГРАДЫ",130,0,681,38,30);
            view.RewardStatus=Text(view,view.RewardsArea,"RewardStatus","",125,34,691,25,19);
            view.RewardCards=new RectTransform[6];view.RewardIcons=new Image[6];view.RewardValues=new Text[6];view.RewardLabels=new Text[6];
            for(int i=0;i<6;i++)
            {
                var card=Art(view.RewardsArea,"Reward_"+i,ProductionGameplayArt.Load("card_wood_r9"),0,62,220,220);card.type=Image.Type.Sliced;
                view.RewardCards[i]=card.rectTransform;
                view.RewardIcons[i]=Art(card.transform,"Icon",Home("coin_icon"),48,37,116,116);view.RewardIcons[i].preserveAspect=true;
                view.RewardValues[i]=Text(view,card.transform,"Amount","0",10,156,200,38,32);
                view.RewardLabels[i]=Text(view,card.transform,"Label","",10,16,200,24,19);
            }
            view.Footer=Rect(view.Content,"Actions",0,1340,941,332);
            Art(view.Footer,"Ornament",source.FooterOrnament.sprite,-5,203,951,320);
            view.ContinueButton=Button(Art(view.Footer,"Continue",Load(ArtRoot+"continue_r7"),171,0,600,155));
            view.ContinueText=Text(view,view.ContinueButton.transform,"Label","ПРОДОЛЖИТЬ",74,52,452,55,42);
            view.DoubleButton=Button(Art(view.Footer,"DoubleGold",Load(ArtRoot+"video_r5"),170,163,601,112));
            view.DoubleText=Text(view,view.DoubleButton.transform,"Label","",45,30,490,43,25);
            view.Status=Text(view,view.Footer,"Status","",80,278,781,42,20);view.Status.horizontalOverflow=HorizontalWrapMode.Wrap;
            var layout=root.gameObject.AddComponent<ProductionVictoryLayout>();layout.View=view;
            ApplyCurrentStyle(view);view.RefreshContent();layout.ApplyNow();
            if(!view.HasRequiredBindings)throw new InvalidOperationException("Victory bindings incomplete.");
        }
        // The prefab owner and runtime repair path apply the same page-only style.
        public static void ApplyCurrentStyle(ProductionVictoryView view)
        {
            // Upgrade in place: keep all prior object identities and serialized actions.
            view.Environment.sprite=Load(ArtRoot+"environment_r3");
            view.Environment.color=Color.white;
            view.Firebird.sprite=Load(ArtRoot+"firebird_r7");
            view.Firebird.color=Color.white;view.Firebird.preserveAspect=true;
            ProductionVictoryView.Place(view.Firebird.rectTransform,210,-12,570,510);
            ApplyResourceHeader(view);
            if(view.RewardsPanel==null)
            {
                var panel=view.RewardsArea.Find("RewardsPanel");
                view.RewardsPanel=panel!=null?panel.GetComponent<Image>():Art(view.RewardsArea,"RewardsPanel",Load(ArtRoot+"rewards_panel_r2"),50,0,841,420);
            }
            view.RewardsPanel.transform.SetAsFirstSibling();
            view.RewardsPanel.sprite=Load(ArtRoot+"rewards_panel_r2");
            FitPanel(view.RewardsPanel,false);
            ProductionVictoryView.Place(view.RewardsPanel.rectTransform,50,0,841,520);
            ProductionVictoryView.Place(view.RewardsArea,0,760,941,472);
            ProductionVictoryView.PlaceText(view.RewardsHeading,155,68,631,32,32);
            ApplyGoldLettering(view.RewardsHeading,true);
            view.Score.transform.SetParent(view.RewardsArea,false);
            ProductionVictoryView.PlaceText(view.Score,155,104,631,25,21);
            ProductionVictoryView.PlaceText(view.RewardStatus,120,135,701,24,19);
            view.Reaction.transform.SetParent(view.RewardsArea,false);
            view.Status.transform.SetParent(view.RewardsArea,false);
            var inset=view.MessageInset!=null?view.MessageInset.transform:view.RewardsArea.Find("MessageInset");
            view.MessageInset=inset!=null?inset.GetComponent<Image>():Rect(view.RewardsArea,"MessageInset",102,389,737,52).gameObject.AddComponent<Image>();
            ProductionVictoryView.Place(view.MessageInset.rectTransform,102,389,737,52);
            view.MessageInset.color=new Color32(20,13,8,245);view.MessageInset.raycastTarget=false;
            view.MessageInset.transform.SetSiblingIndex(1);
            ProductionVictoryView.PlaceText(view.Reaction,112,392,717,46,22);
            view.Reaction.horizontalOverflow=HorizontalWrapMode.Wrap;
            foreach(RectTransform card in view.RewardCards)FitPanel(card.GetComponent<Image>(),true);

            var banner=view.Body.Find("VictoryBanner") as RectTransform;
            banner.GetComponent<Image>().sprite=Load(ArtRoot+"banner_r4");
            ProductionVictoryView.Place(banner,20,375,901,425);
            ProductionVictoryView.PlaceText(view.Title,110,463,721,150,112);
            ApplyGoldLettering(view.Title,true);
            // Preserve existing components/IDs; the page-only effect owns the curved
            // face, bevel and extrusion on the same live/localizable Text mesh.
            view.Title.GetComponent<ProductionHomeGoldText>().enabled=false;
            view.Title.GetComponent<Shadow>().enabled=false;
            var lettering=view.Title.GetComponent<ProductionVictoryTitleLettering>();
            if(lettering==null)lettering=view.Title.gameObject.AddComponent<ProductionVictoryTitleLettering>();
            lettering.enabled=true;lettering.ArcHeight=32;lettering.Refresh();
            for(int i=0;i<3;i++)
            {
                float size=i==1?170:150;
                float centerX=313+i*158;
                float y=i==1?618:634;
                ProductionVictoryView.Place(view.Stars[i].rectTransform,centerX-size*.5f,y,size,size);
                ProductionVictoryView.Place(view.StarGlows[i].rectTransform,centerX-132,y+size*.5f-132,264,264);
                var halo=view.StarGlows[i].GetComponent<ProductionVictoryStarHalo>();
                if(halo==null)halo=view.StarGlows[i].gameObject.AddComponent<ProductionVictoryStarHalo>();
                halo.Seed=i;view.StarGlows[i].SetVerticesDirty();
                var polish=view.Stars[i].GetComponent<ProductionVictoryStarDepth>();
                if(polish==null)polish=view.Stars[i].gameObject.AddComponent<ProductionVictoryStarDepth>();
            }
            // The wider halos overlap: draw every halo before every opaque star,
            // so a neighbour's glow cannot wash out an already rendered gold face.
            int glowIndex=view.Title.transform.GetSiblingIndex()+1;
            for(int i=0;i<3;i++)view.StarGlows[i].transform.SetSiblingIndex(glowIndex+i);
            view.Body.sizeDelta=new Vector2(941,1260);
            view.Footer.sizeDelta=new Vector2(941,388);
            var continueArt=view.ContinueButton.GetComponent<Image>();
            continueArt.sprite=Load(ArtRoot+"continue_r7");continueArt.type=Image.Type.Simple;continueArt.preserveAspect=true;
            // Fit the complete ornament uniformly; never widen its gems/scrollwork.
            float buttonWidth=Mathf.Min(701,164*continueArt.sprite.rect.width/continueArt.sprite.rect.height);
            ProductionVictoryView.Place(view.ContinueButton.transform as RectTransform,(941-buttonWidth)*.5f,0,buttonWidth,164);
            ProductionVictoryView.PlaceText(view.ContinueText,buttonWidth*.09f,46,buttonWidth*.82f,72,48);
            ApplyGoldLettering(view.ContinueText,true);
            var video=view.DoubleButton.GetComponent<Image>();video.sprite=Load(ArtRoot+"video_r5");video.type=Image.Type.Simple;
            ProductionVictoryView.Place(video.rectTransform,135,170,671,112);
            ProductionVictoryView.PlaceText(view.DoubleText,150,20,380,72,30);
            ApplyGoldLettering(view.DoubleText,true);
            view.FooterOrnament=view.Footer.Find("Ornament").GetComponent<Image>();
            view.FooterOrnament.sprite=Load(ArtRoot+"footer_r5");
            ProductionVictoryView.Place(view.FooterOrnament.rectTransform,0,255,941,105);
            view.FooterOrnament.transform.SetAsFirstSibling();
            ProductionVictoryView.PlaceText(view.Status,112,392,717,46,22);
            view.Status.horizontalOverflow=HorizontalWrapMode.Wrap;
            foreach(var text in view.GetComponentsInChildren<Text>(true))
                if(text!=view.Title&&text!=view.ContinueText)ApplyClearLettering(text);
            ProductionVictoryReferenceStyle.Apply(view);
            view.Revision=ProductionVictoryView.CurrentRevision;
        }
        static void ApplyResourceHeader(ProductionVictoryView view)
        {
            // The independently painted bar contains three blank resource compartments.
            // Keep the existing live values and native plus buttons over their own cells.
            view.ResourceHeader=view.Hud.Find("HudFrame").GetComponent<Image>();
            view.ResourceHeader.sprite=Load(ArtRoot+"resource_header_r3");
            view.ResourceHeader.color=Color.white;view.ResourceHeader.type=Image.Type.Simple;
            ProductionVictoryView.Place(view.ResourceHeader.rectTransform,60,12,821,117);
            for(int i=0;i<3;i++)
            {
                var group=view.Hud.Find("Resource_"+i) as RectTransform;
                ProductionVictoryView.Place(group,60+i*(821f/3),12,821f/3,117);
                group.Find("Capsule").gameObject.SetActive(false);
                group.Find("Icon").gameObject.SetActive(false);
                ProductionVictoryView.PlaceText(view.ResourceValues[i],i==2?80:103,i==2?30:33,112,i==2?43:50,29);
                view.ResourceValues[i].color=new Color32(255,230,178,255);
                ProductionVictoryView.Place(view.ResourceButtons[i].transform as RectTransform,i==2?194:217,36,50,50);
            }
            if(view.EnergyTimer==null)
            {
                var timer=view.ResourceValues[2].transform.parent.Find("EnergyTimer");
                view.EnergyTimer=timer!=null?timer.GetComponent<Text>():Text(view,view.ResourceValues[2].transform.parent,"EnergyTimer","—",79,60,113,27,19);
            }
            ProductionVictoryView.PlaceText(view.EnergyTimer,88,83,119,20,16);
            view.EnergyTimer.color=new Color32(83,222,255,255);
        }
        static void FitPanel(Image image,bool card)
        {
            image.type=Image.Type.Simple;
            var fit=image.GetComponent<ProductionVictoryPanelFit>();
            if(fit==null)fit=image.gameObject.AddComponent<ProductionVictoryPanelFit>();
            fit.RewardCard=card;fit.enabled=true;image.SetVerticesDirty();
        }
        static void ApplyClearLettering(Text text)
        {
            // Keep small live glyphs crisp: no multi-layer bevel or long offset shadow.
            var gold=text.GetComponent<ProductionHomeGoldText>();if(gold!=null)gold.enabled=false;
            text.material=null;
            text.fontStyle=FontStyle.Normal;
            var geometry=text.GetComponent<SplashTitleTextGeometry>();
            if(geometry!=null)geometry.SnapToScreenPixels=true;
            var shadow=text.GetComponent<Shadow>();
            if(shadow!=null){shadow.effectColor=new Color32(10,6,2,220);shadow.effectDistance=new Vector2(1,-1);}
        }
        static void ApplyGoldLettering(Text text,bool bold)
        {
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            var effect=text.GetComponent<ProductionHomeGoldText>();
            if(effect==null)effect=text.gameObject.AddComponent<ProductionHomeGoldText>();
            effect.BeveledFace=true;effect.TopColor=new Color32(255,236,157,255);effect.BottomColor=new Color32(191,113,17,255);
            text.material=home.PlayText.material;text.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;
            var shadow=text.GetComponent<Shadow>();shadow.effectColor=new Color32(45,19,3,245);shadow.effectDistance=new Vector2(4,-5);
        }

        static Sprite Load(string key)=>Resources.Load<Sprite>(key)??throw new InvalidOperationException("Missing Victory resource: "+key);
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)=>ProductionChapterMapRuntime.Rect(p,n,x,y,w,h);
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.raycastTarget=false;return a;}
        static ProductionWorldMapGraphic Graphic(Transform p,string n,WorldMapGraphicKind kind,float x,float y,float w,float h){var g=Rect(p,n,x,y,w,h).gameObject.AddComponent<ProductionWorldMapGraphic>();g.Kind=kind;g.raycastTarget=false;return g;}
        static Button Button(Image image){image.raycastTarget=true;var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Text(ProductionVictoryView v,Transform p,string n,string value,float x,float y,float w,float h,int size)
        {
            var t=Rect(p,n,x,y,w*2,h*2).gameObject.AddComponent<Text>();t.rectTransform.localScale=Vector3.one*.5f;
            t.text=value;t.font=v.Typography.RegularFont;t.fontSize=size*2;t.color=new Color32(250,225,171,255);t.alignment=TextAnchor.MiddleCenter;
            t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;t.raycastTarget=false;
            t.gameObject.AddComponent<SplashTitleTextGeometry>().SnapToScreenPixels=false;
            var shadow=t.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(15,8,3,220);shadow.effectDistance=new Vector2(2,-2);
            return t;
        }
    }
}
