using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionEventDetailRuntime
    {
        public static Sprite Load(string name)=>ProductionEventsRuntime.Load(name);
        static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionEventDetailView>();
            if(existing!=null){if(existing.Revision<ProductionEventDetailView.CurrentRevision)existing.Revision=ProductionEventDetailView.CurrentRevision;if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Event Detail outside Play Mode.");AlignTextBackings(existing);existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionEventDetailView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();
            if(shop==null||!shop.HasRequiredBindings)throw new InvalidOperationException("Event Detail requires the accepted shared header.");
            v.Typography=shop.Typography;
            v.Content=Rect(root,"EVENT_DETAIL_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("FrontEndHomeAligned/loading_environment_dusk"),0,0,941,1672);v.Environment.color=new Color(.65f,.65f,.70f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            var shared=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);shared.name="NativeHud";
            v.Hud=(RectTransform)shared.transform;v.Header=shared;
            v.ProfileButton=shared.Profile;v.SettingsButton=shared.Settings;
            v.HeroName=shared.PlayerName;v.LevelText=shared.Level;v.PlusButtons=shared.Plus;
            v.GoldText=shared.Balances[0];v.GemsText=shared.Balances[1];v.EnergyText=shared.Balances[2];

            v.Title=Art(v.Body,"TitlePlaque",Load("title_r3"),211,117,535,184);
            v.Heading=Label(v,v.Body,"Heading",262,169,431,105,66);Gold(v.Heading,true);
            var hero=Rect(v.Body,"FestivalScene",35,328,871,420);
            Frame(hero,"Backing",0,0,871,420);
            var window=Rect(hero,"SceneWindow",20,20,831,380);window.gameObject.AddComponent<RectMask2D>();
            var scene=Art(window,"CleanCity",Load("scene_firebird_r3"),0,-121,831,623);scene.preserveAspect=true;
            Art(window,"SourceFirebird",Load("firebird_r3"),171,0,489,380);
            var edge=Art(hero,"ForegroundOrnaments",Load("frame_ornaments_r3"),0,0,871,420);edge.type=Image.Type.Sliced;edge.preserveAspect=false;
            v.EventName=Label(v,v.Body,"EventName",77,767,787,68,43);Gold(v.EventName,false);
            var status=Art(v.Body,"AvailabilityFrame",Load("status_r3"),255,842,431,48);status.type=Image.Type.Sliced;status.preserveAspect=false;
            Art(v.Body,"Clock",Load("clock_r2"),265,846,38,38);
            v.Availability=Label(v,v.Body,"Availability",315,845,350,40,27);
            Frame(v.Body,"ObjectivesFrame",65,918,811,178);
            v.ObjectiveHeading=Label(v,v.Body,"ObjectiveHeading",100,933,741,45,33);Gold(v.ObjectiveHeading,false);
            v.Description=Label(v,v.Body,"Description",110,987,721,88,29);
            Frame(v.Body,"RewardsFrame",65,1120,811,244);
            v.RewardHeading=Label(v,v.Body,"RewardHeading",100,1135,741,44,33);Gold(v.RewardHeading,false);
            v.RewardExamples=Art(v.Body,"RewardExamples",270,1184,400,119);
            v.RewardNote=Label(v,v.Body,"RewardNote",100,1313,741,36,24);
            v.ActionButton=Button(Art(v.Body,"UnavailableParticipation",Load("action_r3"),290,1390,360,128));
            v.ActionLabel=Label(v,v.ActionButton.transform,"Label",32,25,296,70,32);Gold(v.ActionLabel,false);
            var colors=v.ActionButton.colors;colors.disabledColor=new Color(.76f,.76f,.76f,1);v.ActionButton.colors=colors;
            v.BackButton=Button(Art(v.Body,"Back",Shared("DailyRewardAligned/back_r2"),26,1550,90,86));
            v.Footer=Label(v,v.Body,"AvailabilityNote",138,1542,754,87,25);
            Art(v.Body,"FooterOrnament",Shared("ShopAligned/footer_ornament"),124,1637,780,28);
            var layout=root.gameObject.AddComponent<ProductionEventDetailLayout>();layout.View=v;AlignTextBackings(v);v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Event Detail native bindings incomplete.");
        }
        public const string DetailArtRoot="ProductionUI/EventDetailAligned/";
        public static Sprite DetailArt(string key)=>Resources.Load<Sprite>(DetailArtRoot+key);
        static void AlignTextBackings(ProductionEventDetailView v)
        {
            v.Revision=ProductionEventDetailView.CurrentRevision;ProductionPlayerHeader.Attach(v);
            v.Environment.sprite=Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+"clean_environment");v.Environment.color=Color.white;
            v.Title.sprite=DetailArt("title");Place(v.Title.rectTransform,154,90,637,353);
            Caption(v.Heading,226,249,496,154,64);v.Heading.enabled=false;
            NeedLabel(v,v.Body,"EnglishLogo",332,141,290,87,28);
            // Original Firebird is a free silhouette, not a framed screenshot or landscape crop.
            var hero=(RectTransform)v.Body.Find("FestivalScene");Place(hero,0,402,941,343);
            var window=(RectTransform)hero.Find("SceneWindow");Place(window,0,0,941,343);
            if(window.GetComponent<RectMask2D>())window.GetComponent<RectMask2D>().enabled=false;
            if(window.GetComponent<Mask>())window.GetComponent<Mask>().enabled=false;
            if(window.GetComponent<ProductionHomePanel>())window.GetComponent<ProductionHomePanel>().enabled=false;
            hero.Find("Backing").gameObject.SetActive(false);hero.Find("ForegroundOrnaments").gameObject.SetActive(false);
            window.Find("CleanCity").gameObject.SetActive(false);
            var subject=window.Find("SourceFirebird").GetComponent<Image>();subject.sprite=DetailArt("firebird_r3");Place(subject.rectTransform,108,0,710,343);
            foreach(string layer in new[]{"Halo","Sparks"})
            {
                var effect=NeedArt(window,"Firebird"+layer,DetailArt(layer=="Halo"?"firebird_halo_r3":"firebird_sparks_r3"),108,0,710,343);
                effect.raycastTarget=false;
            }
            window.Find("FirebirdHalo").SetAsFirstSibling();subject.transform.SetAsLastSibling();window.Find("FirebirdSparks").SetAsLastSibling();
            foreach(string old in new[]{"EventNameBacking","FooterTextBacking","FooterOrnament"})if(v.Body.Find(old)!=null)v.Body.Find(old).gameObject.SetActive(false);
            v.EventName.gameObject.SetActive(false);v.ObjectiveHeading.gameObject.SetActive(false);v.RewardExamples.gameObject.SetActive(false);v.RewardNote.gameObject.SetActive(false);
            var status=v.Body.Find("AvailabilityFrame").GetComponent<Image>();status.sprite=DetailArt("countdown_r3");status.type=Image.Type.Simple;status.preserveAspect=true;Place(status.rectTransform,218,681,490,63);
            var clock=v.Body.Find("Clock").GetComponent<Image>();clock.sprite=DetailArt("hourglass");Place(clock.rectTransform,262,690,33,45);
            Caption(v.Availability,309,697,355,36,26);
            var objective=v.Body.Find("ObjectivesFrame").GetComponent<Image>();objective.sprite=DetailArt("objectives");objective.type=Image.Type.Simple;objective.preserveAspect=true;Place(objective.rectTransform,17,739,908,102);
            if(objective.transform.Find("SourceOrnaments"))objective.transform.Find("SourceOrnaments").gameObject.SetActive(false);
            Caption(v.Description,78,757,551,64,24);v.Description.alignment=TextAnchor.MiddleLeft;
            var balance=NeedArt(v.Body,"FeatherBalanceFrame",DetailArt("balance_r3"),649,748,244,84);
            balance.transform.SetSiblingIndex(objective.transform.GetSiblingIndex()+1);
            NeedArt(v.Body,"FeatherBalanceIcon",DetailArt("feather"),674,751,60,77);
            NeedLabel(v,v.Body,"FeatherBalance",741,767,142,52,44).text="—";
            var panel=v.Body.Find("RewardsFrame").GetComponent<Image>();panel.sprite=DetailArt("progress");panel.type=Image.Type.Simple;panel.preserveAspect=true;Place(panel.rectTransform,17,834,908,350);
            if(panel.transform.Find("SourceOrnaments"))panel.transform.Find("SourceOrnaments").gameObject.SetActive(false);
            Caption(v.RewardHeading,221,842,499,38,26);
            var milestones=v.Body.Find("Milestones") as RectTransform??Rect(v.Body,"Milestones",0,0,941,1672);
            string[] rewards={"gem_blue","gem_red","feather","potion","medal"};
            var track=NeedArt(milestones,"EmptyTrack",Load("status_r3"),47,1001,847,20);track.color=new Color32(86,96,108,255);
            NeedArt(milestones,"OriginalRail",DetailArt("progress_rail"),35,995,871,32);
            for(int i=0;i<5;i++)
            {
                float center=130+i*169;
                float[] markerX={130,295,462,631,805};
                NeedArt(milestones,"Diamond_"+i,DetailArt("diamond"+i),markerX[i]-21,988,42,44);
                var chest=NeedArt(milestones,"Chest_"+i,DetailArt("chest"+i),center-75,872,150,119);
                chest.preserveAspect=true;
                NeedLabel(v,milestones,"Threshold_"+i,center-70,1027,140,33,27).text="—";
                var card=NeedArt(milestones,"RewardCard_"+i,DetailArt("reward_card"),center-53,1060,105,107);card.preserveAspect=true;
                NeedArt(card.transform,"Reward",DetailArt(rewards[i]),12,5,81,83);
                NeedLabel(v,card.transform,"Quantity",58,77,36,25,23).text="—";
            }
            var leaders=NeedArt(v.Body,"LeadersPanel",DetailArt("leaders"),17,1180,430,318);
            NeedLabel(v,leaders.transform,"Heading",9,4,412,34,25);
            NeedLabel(v,leaders.transform,"PreviewNote",15,38,400,24,18).gameObject.SetActive(false);
            for(int i=0;i<4;i++)
            {
                var row=NeedArt(leaders.transform,"Row_"+i,DetailArt(i==3?"row_player_r4":"row_r4"),13,38+i*66,400,i==3?70:65);
                row.type=Image.Type.Simple;row.preserveAspect=true;
                if(i<3){var medal=NeedArt(row.transform,"RankMedal",DetailArt("rank"+i+"_r4"),3,0,53,63);medal.transform.SetAsFirstSibling();}
                NeedLabel(v,row.transform,"Rank",8,i<3?6:13,43,37,27).text="—";
                NeedArt(row.transform,"Portrait",i<3?DetailArt("portrait"+i+"_r4"):Shared("IndividualRankingAligned/portrait_ivan"),58,0,68,66);
                NeedLabel(v,row.transform,"Player",150,7,141,49,24).alignment=TextAnchor.MiddleLeft;
                NeedArt(row.transform,"Feather",DetailArt("feather"),296,10,31,42);
                NeedLabel(v,row.transform,"Score",338,9,52,46,26).text="—";
            }
            var premium=NeedArt(v.Body,"PremiumPanel",DetailArt("premium"),447,1180,478,318);
            NeedLabel(v,premium.transform,"Heading",9,4,460,37,26);
            NeedLabel(v,premium.transform,"PreviewNote",26,43,427,58,22);
            var inset=NeedArt(premium.transform,"RewardInset",DetailArt("premium_inset_r3"),8,104,455,131);
            inset.transform.SetAsFirstSibling();
            string[] premiumIcons={"premium_chest","gem_blue","feather","pass"};
            float[] px={13,168,257,346},pw={146,81,81,91};
            for(int i=0;i<4;i++)NeedArt(premium.transform,"Reward_"+i,DetailArt(premiumIcons[i]),px[i],116,pw[i],105);
            var purchase=NeedArt(premium.transform,"UnavailablePass",DetailArt("activate"),29,230,420,74);
            var purchaseButton=purchase.GetComponent<Button>()??Button(purchase);purchaseButton.interactable=false;purchase.raycastTarget=true;
            NeedLabel(v,purchase.transform,"EnglishLabel",38,9,323,53,31);
            // Original lettering remains on Russian controls; English has repaired blank faces.
            v.ActionButton.image.sprite=DetailArt("play");Place((RectTransform)v.ActionButton.transform,211,1495,522,173);
            Caption(v.ActionLabel,71,57,379,74,56);v.ActionLabel.enabled=false;
            var colors=v.ActionButton.colors;colors.disabledColor=new Color(.8f,.8f,.8f,1);v.ActionButton.colors=colors;
            Caption(v.Footer,292,1530,359,26,18);v.Footer.transform.SetAsLastSibling();
            var carving=NeedArt(v.Body,"CarvedFooter",DetailArt("footer_r4"),0,1498,941,174);
            carving.transform.SetAsFirstSibling();
            v.BackButton.image.sprite=DetailArt("back_rim_r4");v.BackButton.image.type=Image.Type.Simple;v.BackButton.image.preserveAspect=true;
            Place((RectTransform)v.BackButton.transform,40,1503,183,164);
            NeedArt(v.BackButton.transform,"BackArrow",DetailArt("back_arrow_r4"),43,51,68,56);
            v.Body.Find("RewardRim").gameObject.SetActive(false);
            NeedArt(v.Body,"RewardPreview",Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+"source_gift"),758,1495,183,177);
            NeedLabel(v,v.Body,"RewardPreviewLabel",795,1517,96,19,14);
        }
        static Image NeedArt(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {
            var image=p.Find(n)?.GetComponent<Image>()??Art(p,n,sprite,x,y,w,h);
            image.sprite=sprite;image.enabled=true;image.raycastTarget=false;Place(image.rectTransform,x,y,w,h);return image;
        }
        static Text NeedLabel(ProductionEventDetailView v,Transform p,string n,float x,float y,float w,float h,int size)
        {
            var t=p.Find(n)?.GetComponent<Text>()??Label(v,p,n,x,y,w,h,size);Caption(t,x,y,w,h,size);return t;
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;}
        static void Caption(Text t,float x,float y,float w,float h,int size)
        {Place(t.rectTransform,x,y,w*2,h*2);t.rectTransform.localScale=new Vector3(.5f,.5f,1);t.fontSize=size*2;}
        static void TextBacking(ProductionEventDetailView v,Text label,string name,float x,float y,float w,float h)
        {
            var backing=v.Body.Find(name)?.GetComponent<Image>()??Art(v.Body,name,Load("status_r3"),x,y,w,h);
            Place(backing.rectTransform,x,y,w,h);
            backing.type=Image.Type.Sliced;backing.preserveAspect=false;
            backing.transform.SetSiblingIndex(label.transform.GetSiblingIndex());
            // Move the live label above its source-derived dark face on repeated builds too.
            label.transform.SetSiblingIndex(backing.transform.GetSiblingIndex()+1);
        }
        static Image Art(Transform p,string n,float x,float y,float w,float h)=>Art(p,n,Load("rewards_firebird_r3"),x,y,w,h);
        static void Frame(Transform p,string n,float x,float y,float w,float h)
        {var image=Art(p,n,Load("frame_r3"),x,y,w,h);image.type=Image.Type.Sliced;image.preserveAspect=false;}
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
        static Text Label(ProductionEventDetailView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,214,155,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
