using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionProfileRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/ProfileAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionProfileView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Profile outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionProfileView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Profile needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");v.NumberFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            v.Content=Rect(root,"PROFILE_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);v.Environment.color=new Color(.68f,.68f,.72f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            var wood=Art(v.Body,"TitleWood",Load("wood_band_r1"),0,122,941,101);wood.preserveAspect=false;
            var shared=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);shared.name="NativeHud";
            foreach(var button in shared.GetComponentsInChildren<Button>(true))AddPressFeedback(button);
            v.Header=shared;v.Hud=(RectTransform)shared.transform;v.ProfileButton=shared.Profile;v.SettingsButton=shared.Settings;v.HeroName=shared.PlayerName;v.LevelText=shared.Level;
            v.PlusButtons=shared.Plus;v.GoldText=shared.Balances[0];v.GemsText=shared.Balances[1];v.EnergyText=shared.Balances[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),260,101,425,130);v.Heading=Label(v,v.Body,"Heading",339,144,261,68,43);Fit(v.Heading,30,43);
            var backing=Rect(v.Body,"ContentBacking",0,217,941,1301).gameObject.AddComponent<ProductionHomePanel>();backing.Frame=false;backing.Radius=0;backing.LeftColor=backing.RightColor=new Color32(6,14,21,235);backing.raycastTarget=false;
            Art(v.Body,"HeroPortrait",Load("portrait_main"),27,217,382,380);v.EditButton=Button(Art(v.Body,"EditProfile",Load("edit"),334,521,52,59));
            // Increase touch areas independently of the approved artwork.
            v.EditButton.image.raycastPadding=new Vector4(-12,-9,-12,-9);
            Panel(v.Body,"IdentityPanel",Load("identity_surface_r1"),410,223,502,160);
            v.CharacterName=Label(v,v.Body,"CharacterName",543,239,344,48,34);v.CharacterName.alignment=TextAnchor.MiddleLeft;v.IdentityLabel=Label(v,v.Body,"IdentityState",543,287,344,30,21);v.IdentityLabel.alignment=TextAnchor.MiddleLeft;Fit(v.IdentityLabel,17,21);v.IdentityLabel.color=new Color32(215,186,129,255);
            Panel(v.Body,"ChapterPlaque",Shared("tab"),532,321,136,40);
            v.ProgressChapterLabel=Label(v,v.Body,"ProgressChapterLabel",544,326,112,29,17);Fit(v.ProgressChapterLabel,14,17);
            Panel(v.Body,"ProgressTrack",Shared("tab"),666,321,216,40);
            v.ProgressFill=Art(v.Body,"ChapterCompletionFill",Resources.Load<Sprite>("ProductionUI/LoadingReferenceAligned/source_progress_fill"),678,331,192,19);v.ProgressFill.preserveAspect=false;v.ProgressFill.type=Image.Type.Filled;v.ProgressFill.fillMethod=Image.FillMethod.Horizontal;v.ProgressFill.fillAmount=0;
            v.ProgressLabel=Label(v,v.Body,"ProgressLabel",682,326,184,29,19);v.ProgressLabel.font=v.NumberFont;Fit(v.ProgressLabel,16,19);
            for(int c=0;c<2;c++)Panel(v.Body,"StatColumn_"+c,Load("stats_surface_r1"),413+c*247,390,245,202);
            v.StatLabels=new Text[6];v.StatValues=new Text[6];string[] icons={"sword","shield","heart","star","feather","boots"};for(int i=0;i<6;i++){int c=i/3,r=i%3;float x=413+c*247,y=392+r*65;Art(v.Body,"StatIcon_"+i,Load(icons[i]),x+9,y+8,45,45);v.StatLabels[i]=Label(v,v.Body,"StatName_"+i,x+70,y+9,162,24,17);v.StatLabels[i].alignment=TextAnchor.MiddleLeft;v.StatLabels[i].color=new Color32(204,167,104,255);Fit(v.StatLabels[i],13,17);v.StatValues[i]=Label(v,v.Body,"StatValue_"+i,x+70,y+33,162,28,25);v.StatValues[i].alignment=TextAnchor.MiddleLeft;v.StatValues[i].font=v.NumberFont;v.StatValues[i].color=new Color32(241,207,146,255);Fit(v.StatValues[i],15,25);}
            v.Tabs=new Button[4];v.TabLabels=new Text[4];for(int i=0;i<4;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Load("tab_surface_r1"),21+i*225,600,226,70));v.Tabs[i].image.raycastPadding=new Vector4(0,-3,0,-3);v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",17,13,192,44,25);Fit(v.TabLabels[i],18,25);}
            v.SectionLabel=Label(v,v.Body,"SectionTitle",73,683,795,43,26);Fit(v.SectionLabel,18,26);v.DynamicRoot=Rect(v.Body,"ProfileContents",0,0,941,1672);
            var footerArt=Art(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1518,941,154);footerArt.preserveAspect=false;footerArt.color=new Color(.7f,.7f,.7f,1);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),22,1527,127,128));
            var strip=Art(v.Body,"FooterStrip",Load("footer_strip_r3"),158,1528,747,120);strip.preserveAspect=false;
            v.FooterButtons=new Button[3];v.FooterLabels=new Text[3];string[] footer={"footer_profile","footer_trophy","footer_history"};float[] footerX={178,480,654},footerW={292,162,196};
            for(int i=0;i<3;i++){float w=footerW[i];v.FooterButtons[i]=Button(Art(v.Body,"Footer_"+i,Load("footer_cell_"+i+"_r3"),footerX[i],1543,w,92));v.FooterButtons[i].image.preserveAspect=false;Art(v.FooterButtons[i].transform,"Icon",Load(footer[i]),(w-60)*.5f,1,60,59);v.FooterLabels[i]=Label(v,v.FooterButtons[i].transform,"Label",6,61,w-12,28,20);Fit(v.FooterLabels[i],12,20);}
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionProfileLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Profile bindings incomplete.");
        }
        static void BuildInfo(ProductionProfileView v){
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.9f);shield.raycastTarget=true;Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,345,817,1070);
            v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",103,385,735,83,38);Fit(v.InfoHeading,28,38);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",109,484,723,510,29);Fit(v.InfoDescription,22,29);
            v.RouteButtons=new Button[4];v.RouteLabels=new Text[4];for(int i=0;i<4;i++){v.RouteButtons[i]=Button(Panel(v.InfoPanel.transform,"Route_"+i,Shared("tab"),115+i%2*365,1025+i/2*101,345,83));v.RouteLabels[i]=Label(v,v.RouteButtons[i].transform,"Label",13,14,319,55,30);Fit(v.RouteLabels[i],22,30);}
            v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1260,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        static readonly string[] HeroRu={"Иван","Маша","Добрыня","Василиса","Кот Баюн"},HeroEn={"Ivan","Masha","Dobrynya","Vasilisa","Bayun Cat"};
        static readonly string[] RelicRu={"Меч-Кладенец","Жар-Перо","Яйцо Жар-Птицы","Ярило-Солнце","Рог Изобилия","Слово Мудрости"},RelicEn={"Magic Sword","Fire Feather","Firebird Egg","Yarilo Sun","Horn of Plenty","Word of Wisdom"};
        // Explicit line breaks preserve the approved names without shrinking long words to miniature text.
        static readonly string[] RelicCaptionRu={"Меч-\nКладенец","Жар-Перо","Яйцо\nЖар-Птицы","Ярило-\nСолнце","Рог\nИзобилия","Слово\nМудрости"},RelicCaptionEn={"Magic\nSword","Fire\nFeather","Firebird\nEgg","Yarilo\nSun","Horn of\nPlenty","Word of\nWisdom"};
        public static void RebuildContent(ProductionProfileView v){
            for(int i=v.DynamicRoot.childCount-1;i>=0;i--){var go=v.DynamicRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool en=DemoFeatureState.LanguageCode=="en";var p=v.DynamicRoot;
            v.SectionLabel.rectTransform.anchoredPosition=new Vector2(73,-683);v.SectionLabel.rectTransform.sizeDelta=new Vector2(795,43);Fit(v.SectionLabel,18,26);
            if(v.Section==ProfileSection.History){BuildHistory(v,en);return;}if(v.Section==ProfileSection.Achievements){BuildAchievements(v,en);return;}
            if(v.SelectedTab==ProfileTab.Items){BuildItems(v,en);return;}
            bool relics=v.SelectedTab==ProfileTab.Relics;
            var collectionFrame=Panel(p,"RelicCollectionFrame",Load("relic_collection_r2_frame"),8,relics?677:993,927,relics?845:529);collectionFrame.pixelsPerUnitMultiplier=1;
            if(!relics){
            Panel(p,"CollectionFrame",Load("collection_surface_r1"),8,677,926,322);
            CollectionHeading(v,v.SectionLabel,"HeroHeading",683,en?"HERO COLLECTION":"КОЛЛЕКЦИЯ ГЕРОЕВ",en);v.SectionLabel.transform.SetAsLastSibling();v.InfoPanel.transform.SetAsLastSibling();
            for(int i=0;i<5;i++){int j=i;var b=Button(Art(p,"Hero_"+i,Load("hero_"+i),39+i*174,731,168,250));b.onClick.AddListener(v.ShowInfo);var name=Label(v,b.transform,"Name",13,193,142,36,21);name.text=en?HeroEn[i]:HeroRu[i];Fit(name,15,21);var level=Label(v,b.transform,"PreviewMark",4,160,38,34,21);level.text="—";}
            }
            if(relics)CollectionHeading(v,v.SectionLabel,"RelicHeading",683,en?"RELIC COLLECTION":"КОЛЛЕКЦИЯ РЕЛИКВИЙ",en);
            else CollectionHeading(v,Label(v,p,"RelicsHeading",325,1005,291,33,27),"RelicHeading",1005,en?"RELICS":"РЕЛИКВИИ",en);
            v.SectionLabel.transform.SetAsLastSibling();v.InfoPanel.transform.SetAsLastSibling();
            for(int i=0;i<6;i++){int j=i;var b=Button(Art(p,"Relic_"+i,Load("relic_"+i),relics?120+i%3*255:30+i*148,relics?748+i/3*257:1048,relics?166:143,relics?246:213));b.onClick.AddListener(()=>v.SelectRelic(j));var name=Label(v,b.transform,"Name",10,relics?181:157,relics?146:123,relics?54:45,18);name.text=en?RelicCaptionEn[i]:RelicCaptionRu[i];name.horizontalOverflow=HorizontalWrapMode.Overflow;if(i==v.SelectedRelic)name.color=new Color32(116,195,239,255);}
            var detailFrame=Panel(p,"RelicDetailFrame",Load("detail_r3_frame"),53,1266,839,253);detailFrame.pixelsPerUnitMultiplier=1;
            var selected=Art(p,"SelectedRelic",v.SelectedRelic==0?Load("detail_sword_r3"):Load("relic_"+v.SelectedRelic),56,1270,204,239);
            if(v.SelectedRelic!=0){
                // Keep the caption within the rendered sprite, rather than the wider aspect-fit container.
                Vector2 source=selected.sprite.rect.size;float scale=Mathf.Min(204/source.x,239/source.y);
                // Image.preserveAspect aligns its drawing rectangle to this top-left pivot.
                Vector2 rendered=source*scale;float left=(204-rendered.x)*selected.rectTransform.pivot.x,top=(239-rendered.y)*(1-selected.rectTransform.pivot.y);
                var caption=Label(v,selected.transform,"DetailCaption",left+rendered.x*.07f,top+rendered.y*.737f,rendered.x*.86f,rendered.y*.211f,17);
                caption.text=en?RelicCaptionEn[v.SelectedRelic]:RelicCaptionRu[v.SelectedRelic];Fit(caption,15,17);
            }
            var title=Label(v,p,"RelicTitle",276,1282,323,42,26);title.alignment=TextAnchor.MiddleLeft;title.font=v.TitleFont;title.text=(en?RelicEn[v.SelectedRelic]:RelicRu[v.SelectedRelic]).ToUpperInvariant();Fit(title,17,26);
            var badge=Art(p,"RelicPreviewBadge",Load("preview_tag_r3"),620,1283,222,40);badge.preserveAspect=false;
            var badgeText=Label(v,p,"RelicPreviewLabel",639,1290,184,26,17);badgeText.text=en?"PREVIEW":"ПРИМЕР";Fit(badgeText,14,17);badgeText.color=new Color32(215,186,129,255);
            var description=Label(v,p,"RelicDescription",276,1331,287,150,20);description.alignment=TextAnchor.UpperLeft;description.text=en?"Collection preview. Ownership, levels and upgrades are not connected.":"Пример коллекции. Владение, уровни и улучшения пока не подключены.";Fit(description,17,20);description.color=new Color32(218,190,140,255);
            var hint=Label(v,p,"RelicSelectionHint",592,1348,262,84,18);hint.text=en?"Select a relic from the collection.":"Выберите реликвию в коллекции.";Fit(hint,16,18);hint.color=new Color32(218,190,140,255);
            var detail=Button(Panel(p,"CollectionInfo",Shared("rules"),585,1452,273,52));detail.image.raycastPadding=new Vector4(0,-12,0,-12);detail.onClick.AddListener(v.ShowInfo);var text=Label(v,detail.transform,"Label",14,8,245,35,18);text.text=en?"PROFILE ACTIONS":"ДЕЙСТВИЯ ПРОФИЛЯ";Fit(text,13,18);
        }
        static void CollectionHeading(ProductionProfileView v,Text title,string prefix,float y,string wording,bool en){
            title.rectTransform.anchoredPosition=new Vector2(323,-y);title.rectTransform.sizeDelta=new Vector2(295,33);title.text=wording;title.font=v.TitleFont;Fit(title,18,25);
            Art(v.DynamicRoot,prefix+"Left",Load("heading_left_r2"),216,y+5,99,28);
            Art(v.DynamicRoot,prefix+"Right",Load("heading_right_r2"),626,y+5,99,28);
            var note=Label(v,v.DynamicRoot,prefix+"Preview",322,y+30,297,20,13);note.color=new Color32(192,171,128,255);note.text=en?"DESIGN PREVIEW":"ПРИМЕР ДИЗАЙНА";
        }
        static void BuildItems(ProductionProfileView v,bool en){var p=v.DynamicRoot;v.SectionLabel.text=en?"OWNED BOOSTERS · LOCAL SAVE":"УСИЛИТЕЛИ · ЛОКАЛЬНОЕ СОХРАНЕНИЕ";var s=GameState.Instance!=null?GameState.Instance.Save:null;int[] counts=s==null?new int[4]:new[]{s.boosterMace,s.boosterFirebird,s.boosterLivingLight,s.boosterSeaPearl};string[] ru={"Богатырская булава","Перо Жар-Птицы","Живой свет","Жемчужина Морского царя"},eng={"Bogatyr Mace","Firebird Feather","Living Light","Sea King's Pearl"};for(int i=0;i<4;i++){float x=80+i%2*403,y=753+i/2*296;Panel(p,"ItemFrame_"+i,Shared("reward_panel"),x,y,378,279);Art(p,"Item_"+i,ProductionGameplayArt.Booster(i),x+107,y+21,164,157);var label=Label(v,p,"ItemName_"+i,x+22,y+174,334,59,28);label.text=en?eng[i]:ru[i];Fit(label,21,28);Label(v,p,"Quantity_"+i,x+50,y+234,278,35,27).text=ProductionProfileView.Number(counts[i]);}var b=Button(Panel(p,"InventoryInfo",Shared("rules"),236,1399,469,83));b.onClick.AddListener(v.ShowInfo);Label(v,b.transform,"Label",18,15,433,53,29).text=en?"INVENTORY / ACCOUNT":"ИНВЕНТАРЬ / АККАУНТ";}
        static void BuildAchievements(ProductionProfileView v,bool en){v.SectionLabel.text=en?"PROGRESS MILESTONES":"ОТМЕТКИ ПРОГРЕССА";bool[] reached={v.CompletedCount>0,v.Records.Exists(x=>x.bestStars==3),v.ChapterTotal>0&&v.ChapterCompleted==v.ChapterTotal};string[] ru={"Первый пройденный уровень","Три звезды за уровень","Пройдена первая глава"},eng={"First level cleared","Three stars on one level","First chapter completed"};for(int i=0;i<3;i++){float y=773+i*200;Panel(v.DynamicRoot,"Milestone_"+i,Shared("reward_panel"),80,y,780,178);Art(v.DynamicRoot,"MilestoneIcon_"+i,Load("footer_trophy"),110,y+29,107,109);var title=Label(v,v.DynamicRoot,"MilestoneName_"+i,245,y+25,579,66,32);title.text=en?eng[i]:ru[i];Fit(title,23,32);var status=Label(v,v.DynamicRoot,"MilestoneStatus_"+i,245,y+102,579,44,27);status.text=en?(reached[i]?"REACHED":"NOT YET REACHED"):(reached[i]?"ДОСТИГНУТО":"ПОКА НЕ ДОСТИГНУТО");status.color=reached[i]?new Color32(170,220,109,255):new Color32(205,195,169,255);}var l=Label(v,v.DynamicRoot,"MilestoneNote",113,1390,715,88,25);l.text=en?"Derived from saved progress. No extra rewards are granted.":"По данным сохранения. Дополнительные награды не выдаются.";Fit(l,20,25);}
        static void BuildHistory(ProductionProfileView v,bool en){
            v.SectionLabel.rectTransform.sizeDelta=new Vector2(530,43);
            bool read=StoryRecap.Read(GameState.Instance!=null?GameState.Instance.Save:null).Count>0;
            var story=Button(Panel(v.DynamicRoot,"StoryRecap",Shared("tab"),625,679,253,61));
            story.interactable=read;story.onClick.AddListener(v.OpenStoryRecap);
            var storyLabel=Label(v,story.transform,"Label",12,7,229,47,27);
            storyLabel.text=en?(read?"STORY":"NO READ SCENES"):(read?"СЮЖЕТ":"НЕТ ПРОЧИТАННЫХ СЦЕН");Fit(storyLabel,19,27);
            if(!read){story.image.color=new Color(.52f,.52f,.52f,1);storyLabel.color=new Color32(145,137,121,255);}
            v.SectionLabel.text=en?"SAVED LEVEL PROGRESS":"СОХРАНЁННЫЙ ПРОГРЕСС УРОВНЕЙ";
            if(v.Records.Count==0){var e=Label(v,v.DynamicRoot,"Empty",122,901,697,222,32);e.text=en?"No recorded attempts yet.\nPlay a level to begin your history.":"Попыток пока нет.\nСыграйте уровень, чтобы начать историю.";Fit(e,24,32);return;}
            for(int i=v.HistoryPage*6;i<Math.Min(v.Records.Count,v.HistoryPage*6+6);i++){
                var r=v.Records[i];float y=752+(i%6)*110;
                var row=Panel(v.DynamicRoot,"Record_"+i,Shared("row"),63,y,815,102).transform;
                var heading=Label(v,row,"Heading",26,8,763,30,25);heading.alignment=TextAnchor.MiddleLeft;
                heading.text=(en?"Chapter ":"Глава ")+r.chapterId+(en?" · Level ":" · Уровень ")+r.levelId;Fit(heading,18,25);
                string[] labels={en?"Stars":"Звёзды",en?"Best":"Рекорд",en?"Attempts":"Попытки"};
                string[] values={r.bestStars+" / 3",ProductionProfileView.Number(r.bestScore),ProductionProfileView.Number(r.attempts)};
                for(int column=0;column<3;column++){
                    float x=26+column*261;
                    var label=Label(v,row,"StatLabel_"+column,x,42,239,19,15);
                    label.alignment=TextAnchor.MiddleLeft;label.color=new Color32(192,166,121,255);label.text=labels[column];
                    var value=Label(v,row,"StatValue_"+column,x,63,239,28,23);
                    value.alignment=TextAnchor.MiddleLeft;value.font=v.NumberFont;value.text=values[column];Fit(value,16,23);
                }
            }
            int pageCount=(v.Records.Count-1)/6+1;
            for(int i=0;i<2;i++){
                int delta=i==0?-1:1;bool available=i==0?v.HistoryPage>0:v.HistoryPage+1<pageCount;
                var b=Button(Panel(v.DynamicRoot,i==0?"Previous":"Next",Shared("tab"),91+i*507,1427,243,68));b.image.raycastPadding=new Vector4(0,-4,0,-4);
                b.interactable=available;
                b.onClick.AddListener(()=>{if(b.IsInteractable())v.ChangeHistoryPage(delta);});
                var label=Label(v,b.transform,"Label",12,10,219,48,27);label.text=en?(i==0?"PREVIOUS":"NEXT"):(i==0?"НАЗАД":"ДАЛЕЕ");
                // Authored color, independent of transient press tint; retain the opaque source face.
                if(!available){b.image.color=new Color(.52f,.52f,.52f,1);label.color=new Color32(145,137,121,255);}
            }
            var page=Label(v,v.DynamicRoot,"PageNumber",342,1432,254,55,26);page.font=v.NumberFont;Fit(page,18,26);page.text=(v.HistoryPage+1)+" / "+pageCount;
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name.EndsWith("_r1"))?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name.EndsWith("_r1"))?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};AddPressFeedback(b);return b;}
        static void AddPressFeedback(Button button){
            // Reuse the map's visual response without installing new audio/haptic callbacks.
            button.transition=Selectable.Transition.None;
            // Stop the default tween that Button.OnEnable may have started while its parent was blocked.
            if(button.targetGraphic)button.targetGraphic.CrossFadeColor(Color.white,0,true,true);
            if(!button.GetComponent<ProductionMapButtonFeedback>())button.gameObject.AddComponent<ProductionMapButtonFeedback>();
        }
        static Text Label(ProductionProfileView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
