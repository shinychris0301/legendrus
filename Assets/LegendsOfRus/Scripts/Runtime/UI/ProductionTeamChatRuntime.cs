using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionTeamChatRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/TeamChatAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionTeamChatView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Team Chat outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionTeamChatView>();
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"TEAM_CHAT_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.22f,.24f,.28f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            v.ProfileButton=Button(Art(v.Hud,"Profile",Shared("rules"),0,0,174,235));
            v.SettingsButton=Button(Art(v.Hud,"Settings",Shared("rules"),853,23,88,95));
            v.HeroName=Label(v,v.Hud,"HeroName",180,30,150,44,34);v.LevelText=Label(v,v.Hud,"Level",180,75,150,29,24);
            v.PlusButtons=new Button[3];Text[] values=new Text[3];
            for(int i=0;i<3;i++){values[i]=Label(v,v.Hud,"Balance_"+i,0,0,80,35,26);v.PlusButtons[i]=Button(Art(v.Hud,"Plus_"+i,Shared("rules"),0,0,44,47));}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];ProductionPlayerHeader.Attach(v);
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),147,124,646,167);v.Heading=Label(v,v.Body,"Heading",218,181,506,69,49);v.Heading.font=v.TitleFont;Fit(v.Heading,36,49);
            v.PreviewLabel=Label(v,v.Body,"Preview",238,388,380,20,15);Fit(v.PreviewLabel,12,15);
            var enclosure=Art(v.Body,"ChestPanel",Load("chest_panel_r2"),58,278,823,133);enclosure.preserveAspect=false;enclosure.transform.SetAsFirstSibling();
            Art(v.Body,"GuildCrest",Load("guild_crest_r2"),88,250,136,158);
            v.ChestTitle=Label(v,v.Body,"ChestTitle",238,300,397,47,31);Fit(v.ChestTitle,23,31);
            v.ChestLevel=Label(v,v.Body,"ChestLevel",238,346,69,40,25);
            var track=Rect(v.Body,"ChestProgressExample",307,347,307,41);
            var empty=Rect(track,"EmptyChannel",22,8,263,24).gameObject.AddComponent<ProductionHomePanel>();empty.Frame=false;empty.Radius=4;empty.LeftColor=empty.RightColor=new Color32(3,15,27,255);empty.raycastTarget=false;
            v.ChestFill=Art(track,"SapphireFill",Resources.Load<Sprite>("ProductionUI/LoadingReferenceAligned/source_progress_fill"),22,8,263,24);v.ChestFill.preserveAspect=false;v.ChestFill.type=Image.Type.Filled;v.ChestFill.fillMethod=Image.FillMethod.Horizontal;v.ChestFill.fillOrigin=0;v.ChestFill.fillAmount=7450f/15000f;
            var rail=Art(track,"SourceRail",Resources.Load<Sprite>("ProductionUI/TeamHubAligned/progress_rail_r2"),0,0,307,41);rail.preserveAspect=false;
            v.ChestProgress=Label(v,track,"ChestProgress",23,0,261,41,24);Fit(v.ChestProgress,20,24);
            v.ChestButton=Button(Art(v.Body,"GuildChest",Load("chest"),638,269,191,147));
            v.ChestInfoButton=Button(Art(v.Body,"ChestInfo",Load("chest_info_r2"),823,278,52,49));
            var surround=Panel(v.Body,"ChatFrame",Load("chat_surround_r2"),24,276,888,1128);surround.transform.SetAsFirstSibling();
            v.ChatViewport=Rect(v.Body,"ChatViewport",43,410,855,985);var viewportImage=v.ChatViewport.gameObject.AddComponent<Image>();viewportImage.color=new Color(0,0,0,.001f);viewportImage.raycastTarget=true;v.ChatViewport.gameObject.AddComponent<RectMask2D>();
            v.Messages=Rect(v.ChatViewport,"Messages",0,0,855,985);v.ChatScroll=v.ChatViewport.gameObject.AddComponent<ScrollRect>();v.ChatScroll.viewport=v.ChatViewport;v.ChatScroll.content=v.Messages;v.ChatScroll.horizontal=false;v.ChatScroll.vertical=true;v.ChatScroll.movementType=ScrollRect.MovementType.Clamped;v.ChatScroll.scrollSensitivity=55;
            var field=Panel(v.Body,"Composer",Load("composer_frame"),123,1404,552,83);v.Composer=field.gameObject.AddComponent<InputField>();field.raycastTarget=true;v.Composer.targetGraphic=field;v.Composer.characterLimit=ProductionTeamChatView.MessageLimit;v.Composer.lineType=InputField.LineType.MultiLineNewline;
            var inputText=Label(v,field.transform,"InputText",22,15,439,53,25);inputText.alignment=TextAnchor.MiddleLeft;inputText.supportRichText=false;v.Composer.textComponent=inputText;
            v.InputPlaceholder=Label(v,field.transform,"Placeholder",22,15,439,53,23);v.InputPlaceholder.alignment=TextAnchor.MiddleLeft;v.InputPlaceholder.color=new Color32(174,155,119,255);v.Composer.placeholder=v.InputPlaceholder;
            v.EmojiButton=Button(Art(field.transform,"Stickers",Load("emoji"),483,12,50,56));
            // Preserve the source icon while making its target usable on narrow phones.
            v.EmojiButton.image.raycastPadding=new Vector4(-13,-10,-13,-10);
            v.SendButton=Button(Art(v.Body,"Send",Load("send_ru_rebuild"),679,1406,219,80));v.SendLabel=Label(v,v.SendButton.transform,"Label",23,15,173,47,27);Fit(v.SendLabel,21,29);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),15,1398,102,98));
            v.LocalStatus=Label(v,v.Body,"LocalStatus",20,1645,275,25,13);Fit(v.LocalStatus,10,13);
            v.FooterButtons=new Button[3];v.FooterLabels=new Text[3];string[] icons={"footer_gift","footer_help","footer_sticker"};
            var footer=Art(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1493,941,179);footer.preserveAspect=false;footer.color=new Color(.55f,.55f,.55f,1);
            float[] fx={85,361,638},fy={1492,1484,1493},fw={220,241,260},fh={152,161,152};
            for(int i=0;i<3;i++){v.FooterButtons[i]=Button(Art(v.Body,"Footer_"+i,Load(icons[i]+"_ru_rebuild"),fx[i],fy[i],fw[i],fh[i]));v.FooterLabels[i]=Label(v,v.FooterButtons[i].transform,"Label",new[]{26,19,36}[i],new[]{101,109,100}[i],new[]{175,200,193}[i],35,23);Fit(v.FooterLabels[i],i==1?14:16,23);}
            Art(v.Body,"FooterSapphire",Load("footer_sapphire_r4"),300,1615,350,57);
            v.LocalStatus.transform.SetAsLastSibling();
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionTeamChatLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Team Chat native bindings incomplete.");
        }
        static void BuildInfo(ProductionTeamChatView v)
        {
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,402,817,838);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",108,431,725,102,40);Fit(v.InfoHeading,29,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",114,556,713,492,29);Fit(v.InfoDescription,23,29);
            v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1118,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        public static void RebuildMessages(ProductionTeamChatView v)
        {
            // Destroyed Play Mode rows leave the layout immediately before their deferred destruction.
            for(int i=v.Messages.childCount-1;i>=0;i--){var go=v.Messages.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool en=DemoFeatureState.LanguageCode=="en";float y=0;
            string[] names=en?new[]{"Yaroslav","Alena","Mikhail","Svetlana","SYSTEM","Viktor","Nikolai","Yaroslav"}:new[]{"Ярослав","Алёна","Михаил","Светлана","СИСТЕМА","Виктор","Николай","Ярослав"};
            string[] portraits={"yaroslav","alena","mikhail","svetlana",null,"viktor","nikolai","yaroslav"};
            string[] text=en?new[]{"Good morning, warriors! Today is important. We need teamwork.","Ready to help with guild tasks!","Requests help: defeat the Forest Giant (level 3).","Sent an energy gift!","Viktor raised the guild chest to level 6!","Great work, team! Together we are strong!","Requests help: collect 10 ancient artifacts.","Remember the boss rewards! Guild raid tonight at 20:00."}:new[]{"Доброе утро, воины! Сегодня важный день. Нам нужна слаженная работа.","Готова помочь с заданиями гильдии!","Запрашивает помощь: победить Лесного великана (3 ур.).","Отправила энергию в подарок!","Игрок Виктор повысил сундук гильдии до 6 уровня!","Отличная работа, команда! Вместе мы сила!","Запрашивает помощь: собрать 10 древних артефактов.","Не забывайте собирать награды с боссов! Вечером рейд в 20:00."};
            string[] times={"09:12","09:15","09:16","09:17","09:20","09:21","09:22","09:24"};
            for(int i=0;i<8;i++){
                float[] heights={141,132,125,120,90,120,125,132};
                float h=heights[i];var row=Rect(v.Messages,"SampleMessage_"+i,0,y,855,h);bool help=i==2||i==6;
                if(i==4){
                    var panel=Art(row,"Bubble",Load("system_panel_r2"),139,7,696,80);panel.preserveAspect=false;
                    Art(row,"SystemCrest",Load("system_crest_r2"),14,-5,123,102);
                    var heading=Label(v,row,"Name",164,12,300,35,25);heading.text=names[i];heading.alignment=TextAnchor.MiddleLeft;
                    Label(v,row,"Time",758,8,74,27,18).text=times[i];
                    var content=Label(v,row,"Message",164,45,655,36,24);content.text=text[i];content.alignment=TextAnchor.MiddleLeft;content.supportRichText=false;Fit(content,18,24);
                }else{
                    var portrait=Load("portrait_"+portraits[i]);Art(row,"Portrait",portrait,13,0,128,h-2);
                    var name=Label(v,row,"Name",161,0,210,36,26);name.alignment=TextAnchor.MiddleLeft;name.text=names[i];
                    var role=Label(v,row,"Role",365,6,232,28,19);role.alignment=TextAnchor.MiddleLeft;role.text=en?(i==0||i==7?"Leader · sample":i==1?"Officer · sample":"Member · sample"):(i==0||i==7?"Глава · пример":i==1?"Офицер · пример":"Участник · пример");
                    Label(v,row,"Time",744,4,84,30,20).text=times[i];
                    float bubbleY=i==1?37:i==3?32:i==5?42:34;
                    float bubbleHeight=i==1?67:i==3?72:i==5?62:h-bubbleY-5;
                    Panel(row,"Bubble",Load("message_frame_r3"),141,bubbleY,680,bubbleHeight);
                    var message=Label(v,row,"Message",174,bubbleY+8,help?443:622,bubbleHeight-16,25);message.alignment=TextAnchor.MiddleLeft;message.color=new Color32(40,28,15,255);message.text=text[i];message.supportRichText=false;Fit(message,20,25);
                    if(i==1||i==3||i==5){
                        float available=i==1?408:i==3?345:463;
                        message.rectTransform.sizeDelta=new Vector2(available,bubbleHeight-16);
                        FitInline(message,available);
                        if(i==1)Art(row,"MessageSwords",Load("message_swords_r3"),591,51,45,39);
                        if(i==5)Art(row,"MessageStrength",Load("message_strength_r3"),651,48,44,44);
                        if(i==3){
                            Art(row,"GiftLightning",Resources.Load<Sprite>("ProductionUI/NavigationAligned/energy"),532,38,32,50);
                            var amount=Label(v,row,"GiftAmount",570,43,58,44,27);amount.text="+5";amount.color=new Color32(40,28,15,255);
                            Art(row,"EnergyMedallion",Load("energy_medallion_r3"),652,-4,108,113);
                            var stamp=row.Find("Time").GetComponent<Text>();stamp.rectTransform.anchoredPosition=new Vector2(766,-4);stamp.rectTransform.sizeDelta=new Vector2(64,30);stamp.fontSize=18;
                        }
                    }
                    if(help){var button=Button(Art(row,"Help",Load(en?"help_blank_r4":"help_ru_r4"),635,i==2?38:46,165,61));var label=Label(v,button.transform,"Label",20,10,125,39,25);label.text=en?"HELP":"ПОМОЧЬ";label.enabled=en;button.onClick.AddListener(()=>v.ShowInfo(4));}
                }
                y+=h;
            }
            foreach(var item in ProductionTeamChatView.LocalMessages){
                var row=Rect(v.Messages,"LocalMessage",0,y,855,120);var msg=Label(v,row,"Message",170,43,626,300,27);msg.text=item.Body;msg.supportRichText=false;msg.alignment=TextAnchor.UpperLeft;float h=Mathf.Max(125,Mathf.Ceil(msg.preferredHeight*1.25f+16)+60);row.sizeDelta=new Vector2(855,h);msg.rectTransform.sizeDelta=new Vector2(626,h-56);
                var bubble=Panel(row,"Bubble",Load("message_frame_r3"),137,34,687,h-42);bubble.transform.SetAsFirstSibling();msg.color=new Color32(40,28,15,255);Art(row,"Portrait",Resources.Load<Sprite>("ProductionUI/TeamHubAligned/portrait_ivan"),6,0,128,120);
                var name=Label(v,row,"Name",150,0,500,36,26);name.alignment=TextAnchor.MiddleLeft;name.text=en?"Ivan · local only":"Иван · только локально";Label(v,row,"Time",740,3,88,30,20).text=item.Time;y+=h;
            }
            v.Messages.sizeDelta=new Vector2(855,Mathf.Max(v.ChatViewport.rect.height,y));
        }
        public static void RefitInlineMessages(ProductionTeamChatView v)
        {
            foreach(int index in new[]{1,3,5}){
                var node=v.Messages.Find("SampleMessage_"+index+"/Message");
                if(node!=null){var text=node.GetComponent<Text>();FitInline(text,text.rectTransform.rect.width);}
            }
        }
        static void FitInline(Text t,float width)
        {
            t.resizeTextForBestFit=false;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.fontSize=25;
            while(t.fontSize>14&&t.preferredWidth>width-4)t.fontSize--;
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
                static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name=="reward_frame"?6:s&&(s.name.StartsWith("message_frame")||s.name=="composer_frame")?3:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionTeamChatView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
