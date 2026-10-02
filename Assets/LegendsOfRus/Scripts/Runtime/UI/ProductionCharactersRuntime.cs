using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionCharactersRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/CharactersAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionCharactersView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Characters outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionCharactersView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Characters needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"CHARACTERS_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            BuildOuterFrame(v);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            // These adjustments affect only the Characters clone of the shared header.
            HeaderText(v.HeroName,178,26,148,43,30);HeaderText(v.LevelText,178,67,148,32,22);
            foreach(var value in values){value.fontSize=44;value.resizeTextMaxSize=44;value.color=new Color32(245,222,175,255);}
            v.Title=Art(v.Body,"TitlePlaque",Load("title_r4"),170,102,585,145);
            v.Heading=Label(v,v.Body,"Heading",244,143,454,78,60);v.Heading.font=v.TitleFont;Fit(v.Heading,48,60);DenseText(v.Heading);GoldLettering(v.Heading,true);
            v.Tabs=new Button[3];v.TabLabels=new Text[3];for(int i=0;i<3;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Shared("tab"),82+i*260,246,254,58));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",14,8,226,42,25);Fit(v.TabLabels[i],19,25);DenseText(v.TabLabels[i]);GoldLettering(v.TabLabels[i],false);}
            var portrait=Rect(v.Body,"PortraitWindow",22,320,411,512);portrait.gameObject.AddComponent<RectMask2D>();
            var backdrop=Art(portrait,"PortraitBackdrop",Load("sunset_r7"),-6,-8,423,528);
            v.DetailArt=Art(portrait,"SelectedHero",Load("hero_firebird_r7"),-5.6f,-7,422.2f,527);
            // Draw the isolated gold perimeter last so the source-cropped subject ends beneath the frame.
            Panel(v.Body,"PortraitFrame",Load("portrait_frame_r7"),10,309,435,534);
            Panel(v.Body,"DetailFrame",Load("detail_frame_r4"),443,309,486,534);
            v.CharacterName=Label(v,v.Body,"CharacterName",467,321,438,47,35);Fit(v.CharacterName,25,35);DenseText(v.CharacterName);
            v.Availability=Label(v,v.Body,"RarityPreview",469,378,99,28,16);Fit(v.Availability,12,16);
            v.DetailStars=new Image[5];for(int i=0;i<5;i++)v.DetailStars[i]=Art(v.Body,"RarityStar_"+i,Load("star_r2"),574+i*39,375,32,31);
            Art(v.Body,"RarityPlaque",Load("rarity_r2"),506,405,323,43);
            v.Rarity=Label(v,v.Body,"Rarity",550,411,238,30,24);Fit(v.Rarity,18,24);
            v.Description=Label(v,v.Body,"HeroLevelUnavailable",471,451,142,29,23);v.Description.alignment=TextAnchor.MiddleLeft;Fit(v.Description,17,23);
            Art(v.Body,"UnavailableProgressTrack",Load("progress_track_r2"),613,453,268,26);
            v.StatLabels=new Text[4];v.StatValues=new Text[4];string[] statArt={"ProfileAligned/sword","ProfileAligned/shield","ChapterCompleteAligned/gem","ProfileAligned/heart"};
            for(int i=0;i<4;i++){
                Art(v.Body,"StatIcon_"+i,Resources.Load<Sprite>("ProductionUI/"+statArt[i]),474,485+i*27,26,25);
                v.StatLabels[i]=Label(v,v.Body,"Stat_"+i,506,483+i*27,270,28,23);v.StatLabels[i].alignment=TextAnchor.MiddleLeft;
                v.StatValues[i]=Label(v,v.Body,"Value_"+i,806,483+i*27,75,28,24);
                var line=Rect(v.Body,"StatDivider_"+i,506,510+i*27,375,1).gameObject.AddComponent<ProductionHomePanel>();line.Frame=false;line.Radius=0;line.LeftColor=line.RightColor=new Color(.57f,.39f,.15f,.52f);line.raycastTarget=false;
            }
            v.SkillHeading=Label(v,v.Body,"SkillHeading",476,590,422,29,23);Fit(v.SkillHeading,18,23);
            v.AbilityLevels=new Text[4];for(int i=0;i<4;i++){
                Art(v.Body,"AbilityConcept_"+i,Load("ability_"+i+"_r2"),475+i*103,613,92,90);
                v.AbilityLevels[i]=Label(v,v.Body,"UnavailableAbilityLevel_"+i,475+i*103+new[]{65,64,59,57}[i],672,20,24,18);
            }
            v.BonusHeading=Label(v,v.Body,"BonusHeading",519,703,372,28,23);Fit(v.BonusHeading,18,23);
            Art(v.Body,"BonusConcept",Load("badge_shield_r2"),473,710,34,38);
            v.BonusNote=Label(v,v.Body,"BonusAvailability",520,730,366,27,21);Fit(v.BonusNote,17,21);
            v.UpgradeButton=Button(Art(v.Body,"Upgrade",Load("upgrade_r4"),452,758,231,67));v.UpgradeLabel=Label(v,v.UpgradeButton.transform,"Label",20,14,177,38,23);Fit(v.UpgradeLabel,17,23);
            v.SelectButton=Button(Art(v.Body,"Select",Load("select_r4"),683,758,226,67));v.SelectLabel=Label(v,v.SelectButton.transform,"Label",20,14,172,38,22);Fit(v.SelectLabel,16,22);
            // Keep the detail hierarchy crisp at phone scale without changing selected font families.
            foreach(var text in v.StatLabels)DenseText(text);
            foreach(var text in v.StatValues)DenseText(text);
            foreach(var text in new[]{v.Rarity,v.Description,v.SkillHeading,v.BonusHeading,v.BonusNote,v.UpgradeLabel,v.SelectLabel})DenseText(text);
            v.CollectionViewport=Rect(v.Body,"CollectionViewport",18,845,905,624);
            var scrollHit=v.CollectionViewport.gameObject.AddComponent<ProductionHomePanel>();scrollHit.Frame=false;scrollHit.Radius=0;scrollHit.LeftColor=scrollHit.RightColor=Color.clear;scrollHit.raycastTarget=true;
            v.CollectionViewport.gameObject.AddComponent<RectMask2D>();
            v.CollectionContent=Rect(v.CollectionViewport,"Cards",0,0,905,742);
            v.CollectionScroll=v.CollectionViewport.gameObject.AddComponent<ScrollRect>();
            v.CollectionScroll.viewport=v.CollectionViewport;v.CollectionScroll.content=v.CollectionContent;
            v.CollectionScroll.horizontal=false;v.CollectionScroll.vertical=true;v.CollectionScroll.movementType=ScrollRect.MovementType.Clamped;v.CollectionScroll.scrollSensitivity=42;
            v.Cards=new Button[8];v.CardFrames=new Image[8];v.CardNames=new Text[8];v.CardStatuses=new Text[8];v.CardSelections=new Image[8];v.CardStars=new Image[40];
            for(int i=0;i<8;i++){
                var frame=Panel(v.CollectionContent,"HeroCard_"+i,Load(i==v.CurrentHero?"card_border_r2":"card_normal_r6"),10+i%4*220,5+i/4*244,218,240);v.Cards[i]=Button(frame);
                v.CardSelections[i]=Art(frame.transform,"SelectedBorder",Load("card_selection_r2"),-5,-5,228,250);
                var window=Rect(frame.transform,"PortraitWindow",13,13,192,202);window.gameObject.AddComponent<RectMask2D>();
                BuildCollectionPortrait(window,i);
                Art(frame.transform,"ClassEmblem",Load(i==7?"badge_skull_r2":i==0?"star_r2":"badge_sword_r2"),14,12,37,39);
                Art(frame.transform,"AffinityEmblem",Load(i<3?"badge_gem_r2":"badge_shield_r2"),176,12,29,36);
                v.CardNames[i]=Label(v,frame.transform,"Name",13,173,192,31,24);Fit(v.CardNames[i],17,24);
                for(int j=0;j<5;j++)v.CardStars[i*5+j]=Art(frame.transform,"ConceptStar_"+j,Load("star_r2"),48+j*25,203,23,22);
                v.CardStatuses[i]=Label(v,frame.transform,"Status",17,220,184,15,12);Fit(v.CardStatuses[i],10,12);
                v.CardFrames[i]=Panel(frame.transform,"FrameEdge",Load(i==v.CurrentHero?"card_border_r2":"card_normal_r6"),0,0,218,240);
                v.CardStatuses[i].transform.SetAsLastSibling();
            }
            v.FutureCards=new GameObject[4];v.FutureLabels=new Text[4];for(int i=0;i<4;i++){
                var frame=Panel(v.CollectionContent,"FutureHero_"+i,Load("card_normal_r6"),10+i*220,493,218,240);v.FutureCards[i]=frame.gameObject;
                var window=Rect(frame.transform,"PreviewWindow",13,12,192,202);window.gameObject.AddComponent<RectMask2D>();
                var scene=Art(window,"CleanScenery",Load("card_forest_r12"),0,-1,192,204);scene.color=new Color(.65f,.65f,.68f,1);
                var silhouette=Art(window,"FutureSilhouette",Load("card_portrait_"+(4+i)+"_r12"),-3,1,198,186);silhouette.color=new Color(.035f,.045f,.05f,1);
                Art(frame.transform,"Lock",Resources.Load<Sprite>("ProductionUI/LoginAligned/lock"),88,120,42,44);
                v.FutureLabels[i]=Label(v,frame.transform,"Status",17,198,184,28,21);Fit(v.FutureLabels[i],14,21);
                Panel(frame.transform,"FrameEdge",Load("card_normal_r6"),0,0,218,240);v.FutureLabels[i].transform.SetAsLastSibling();
            }
            Art(v.Body,"CarvedFooter",Load("footer_backing_r8"),0,1468,941,204);
            v.InfoButton=Button(Panel(v.Body,"CollectionInfo",Shared("rules"),259,1470,461,35));v.CollectionLabel=Label(v,v.InfoButton.transform,"CollectionPreview",22,2,417,30,19);Fit(v.CollectionLabel,15,19);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),16,1521,134,134));
            v.NavButtons=new Button[4];v.NavLabels=new Text[4];string[] nav={"map","shop","events","team"};
            for(int i=0;i<4;i++){
                v.NavButtons[i]=Button(Art(v.Body,"Navigation_"+i,Load("nav_"+nav[i]+"_r8"),174+i*187,1502,171,157));
                Art(v.NavButtons[i].transform,"Plaque",Load("nav_caption_r8"),0,122,171,46);
                v.NavLabels[i]=Label(v,v.NavButtons[i].transform,"Label",14,131,143,26,20);Fit(v.NavLabels[i],16,20);
            }
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionCharactersLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Characters bindings incomplete.");
        }
        static void BuildInfo(ProductionCharactersView v){v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.9f);shield.raycastTarget=true;Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),77,390,787,889);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",120,437,701,79,37);Fit(v.InfoHeading,25,37);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",132,553,677,521,30);Fit(v.InfoDescription,23,30);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1136,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);}
        static void BuildCollectionPortrait(RectTransform window,int hero)
        {
            // Reuse the source character at source pixel proportions rather than enlarging every bust.
            // Independent reconstructed plates supply only the scenery hidden behind the source figure.
            bool woodland=hero==2||hero==5||hero==7;
            var scene=Art(window,"CleanScenery",Load(woodland?"card_forest_r12":"card_town_r12"),-3-hero%3*2,-4,200,213);
            scene.color=woodland?new Color(.9f,.9f,.9f,1):new Color(.82f,.82f,.85f,1);
            float[] x={24,9,1,6,7,5,3,8};
            var sprite=Load("card_portrait_"+hero+"_r12");
            float top=hero==0?-2:hero<4?-1:0;
            Art(window,"Portrait",sprite,x[hero],top,sprite.rect.width,sprite.rect.height);
            // The original crop stops at baked source lettering. Complete the caption shadow
            // before that edge so a blue scenery stripe never exposes a floating bust.
            float baseY=top+sprite.rect.height-2;
            var fade=Art(window,"PortraitFade",Load("portrait_fade_r2"),0,baseY-30,192,30);fade.preserveAspect=false;
            var shade=Rect(window,"CaptionShadow",0,baseY,192,202-baseY).gameObject.AddComponent<ProductionHomePanel>();
            shade.Frame=false;shade.Radius=0;shade.LeftColor=shade.RightColor=new Color32(9,15,18,255);shade.raycastTarget=false;
        }
        static void BuildOuterFrame(ProductionCharactersView v)
        {
            v.OuterFrame=Rect(v.Body,"OuterCarvedFrame",0,0,941,1672);
            v.OuterFrame.SetAsFirstSibling();
            // Isolated reference ornament only. All page controls and scrolling content draw above it.
            Art(v.OuterFrame,"UpperLeft",Load("outer_upper_left_r11"),0,118,245,192);
            Art(v.OuterFrame,"UpperRight",Load("outer_upper_right_r11"),734,118,207,192);
            Art(v.OuterFrame,"LowerLeft",Load("outer_lower_left_r11"),0,1408,58,97);
            Art(v.OuterFrame,"LowerRight",Load("outer_lower_right_r11"),891,1408,50,97);
            Art(v.OuterFrame,"LeftTrim",Load("outer_left_trim_r11"),0,310,8,1098);
            Art(v.OuterFrame,"RightTrim",Load("outer_right_trim_r11"),933,310,8,1098);
        }
        static void DenseText(Text text)
        {
            // Rasterize live glyphs at twice the design density without scaling their visible bounds.
            text.rectTransform.sizeDelta*=2;text.rectTransform.localScale=Vector3.one*.5f;
            text.fontSize*=2;text.resizeTextMinSize*=2;text.resizeTextMaxSize*=2;
        }
        static void HeaderText(Text text,float x,float y,float w,float h,int size)
        {
            text.rectTransform.anchoredPosition=new Vector2(x,-y);text.rectTransform.sizeDelta=new Vector2(w*2,h*2);
            text.rectTransform.localScale=Vector3.one*.5f;text.fontSize=size*2;
            Fit(text,size*2-8,size*2);text.color=new Color32(245,222,175,255);
        }
        static void GoldLettering(Text text,bool beveled)
        {
            var shadow=text.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(32,17,5,230);shadow.effectDistance=new Vector2(2,-2);
            var gold=text.gameObject.AddComponent<ProductionHomeGoldText>();gold.BeveledFace=beveled;
            gold.TopColor=new Color32(255,231,164,255);gold.BottomColor=new Color32(169,106,35,255);
            if(beveled)text.material=Resources.Load<Material>(ProductionHomeRuntime.PlayGoldMaterialResource);
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=1;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionCharactersView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
