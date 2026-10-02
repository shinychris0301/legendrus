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
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Team Chat needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"TEAM_CHAT_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),147,124,646,167);v.Heading=Label(v,v.Body,"Heading",218,181,506,69,49);v.Heading.font=v.TitleFont;Fit(v.Heading,36,49);
            v.PreviewLabel=Label(v,v.Body,"Preview",133,272,690,28,18);Fit(v.PreviewLabel,14,18);
            Panel(v.Body,"ChestPanel",Shared("table_frame"),53,298,829,119);
            Art(v.Body,"GuildCrest",Resources.Load<Sprite>("ProductionUI/TeamRankingAligned/crest_eagle"),81,297,97,119);
            v.ChestTitle=Label(v,v.Body,"ChestTitle",192,307,457,43,31);Fit(v.ChestTitle,23,31);
            var track=Panel(v.Body,"ChestProgressExample",Shared("rules"),230,360,385,35);v.ChestProgress=Label(v,v.Body,"ChestProgress",198,355,441,47,25);Fit(v.ChestProgress,20,25);
            v.ChestButton=Button(Art(v.Body,"GuildChest",Load("chest"),660,291,176,135));
            Panel(v.Body,"ChatFrame",Shared("table_frame"),27,415,887,981);
            v.ChatViewport=Rect(v.Body,"ChatViewport",43,421,855,964);var viewportImage=v.ChatViewport.gameObject.AddComponent<Image>();viewportImage.color=new Color(0,0,0,.001f);viewportImage.raycastTarget=true;v.ChatViewport.gameObject.AddComponent<RectMask2D>();
            v.Messages=Rect(v.ChatViewport,"Messages",0,0,855,964);v.ChatScroll=v.ChatViewport.gameObject.AddComponent<ScrollRect>();v.ChatScroll.viewport=v.ChatViewport;v.ChatScroll.content=v.Messages;v.ChatScroll.horizontal=false;v.ChatScroll.vertical=true;v.ChatScroll.movementType=ScrollRect.MovementType.Clamped;v.ChatScroll.scrollSensitivity=55;
            var field=Panel(v.Body,"Composer",Load("composer_frame"),123,1404,552,83);v.Composer=field.gameObject.AddComponent<InputField>();field.raycastTarget=true;v.Composer.targetGraphic=field;v.Composer.characterLimit=ProductionTeamChatView.MessageLimit;v.Composer.lineType=InputField.LineType.MultiLineNewline;
            var inputText=Label(v,field.transform,"InputText",22,15,439,53,25);inputText.alignment=TextAnchor.MiddleLeft;inputText.supportRichText=false;v.Composer.textComponent=inputText;
            v.InputPlaceholder=Label(v,field.transform,"Placeholder",22,15,439,53,23);v.InputPlaceholder.alignment=TextAnchor.MiddleLeft;v.InputPlaceholder.color=new Color32(174,155,119,255);v.Composer.placeholder=v.InputPlaceholder;
            v.EmojiButton=Button(Art(field.transform,"Stickers",Load("emoji"),483,12,50,56));
            v.SendButton=Button(Art(v.Body,"Send",Load("send_frame"),683,1410,210,75));v.SendLabel=Label(v,v.SendButton.transform,"Label",17,15,176,46,29);Fit(v.SendLabel,21,29);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),15,1398,102,98));
            v.LocalStatus=Label(v,v.Body,"LocalStatus",89,1487,763,29,17);Fit(v.LocalStatus,14,17);
            v.FooterButtons=new Button[3];v.FooterLabels=new Text[3];string[] icons={"footer_gift","footer_help","footer_sticker"};
            for(int i=0;i<3;i++){float x=63+i*287;v.FooterButtons[i]=Button(Art(v.Body,"Footer_"+i,Load(icons[i]),x,1516,252,151));v.FooterLabels[i]=Label(v,v.FooterButtons[i].transform,"Label",new[]{42,40,36}[i],100,new[]{175,165,186}[i],34,23);Fit(v.FooterLabels[i],i==1?12:16,23);}
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
            string[] text=en?new[]{"Good morning, warriors! Today is important. We need teamwork.","Ready to help with guild tasks!","Requests help: defeat the Forest Giant (level 3).","Sent an energy gift! +5 · example","Viktor raised the guild chest to level 6!","Great work, team! Together we are strong!","Requests help: collect 10 ancient artifacts.","Remember the boss rewards! Guild raid tonight at 20:00."}:new[]{"Доброе утро, воины! Сегодня важный день. Нам нужна слаженная работа.","Готова помочь с заданиями гильдии!","Запрашивает помощь: победить Лесного великана (3 ур.).","Отправила энергию в подарок! +5 · пример","Игрок Виктор повысил сундук гильдии до 6 уровня!","Отличная работа, команда! Вместе мы сила!","Запрашивает помощь: собрать 10 древних артефактов.","Не забывайте собирать награды с боссов! Вечером рейд в 20:00."};
            string[] times={"09:12","09:15","09:16","09:17","09:20","09:21","09:22","09:24"};
            for(int i=0;i<8;i++){
                float h=i==4?84:i==0||i==2||i==6||i==7?132:112;var row=Rect(v.Messages,"SampleMessage_"+i,0,y,855,h);bool help=i==2||i==6;
                if(portraits[i]!=null)Art(row,"Portrait",Load("portrait_"+portraits[i]),6,0,128,h-6);else Art(row,"SystemCrest",Resources.Load<Sprite>("ProductionUI/TeamRankingAligned/crest_eagle"),25,0,83,88);
                var name=Label(v,row,"Name",146,0,210,36,28);name.alignment=TextAnchor.MiddleLeft;
                name.text=names[i];var role=Label(v,row,"Role",365,6,232,28,19);role.alignment=TextAnchor.MiddleLeft;role.text=i==4?"":en?(i==0||i==7?"Leader · sample":i==1?"Officer · sample":"Member · sample"):(i==0||i==7?"Глава · пример":i==1?"Офицер · пример":"Участник · пример");
                Label(v,row,"Time",744,4,84,30,20).text=times[i];Panel(row,"Bubble",i==4?Shared("row"):Load("message_frame"),137,34,687,h-42);
                var message=Label(v,row,"Message",159,43,help?456:636,h-59,i==4?23:27);message.alignment=TextAnchor.MiddleLeft;message.color=i==4?new Color32(243,206,137,255):new Color32(40,28,15,255);message.text=text[i];message.supportRichText=false;Fit(message,21,i==4?23:27);
                if(help){var b=Button(Art(row,"Help",Load("send_frame"),633,61,174,56));Label(v,b.transform,"Label",12,6,150,43,23).text=en?"HELP":"ПОМОЧЬ";b.onClick.AddListener(()=>v.ShowInfo(4));}
                y+=h;
            }
            foreach(var item in ProductionTeamChatView.LocalMessages){
                var row=Rect(v.Messages,"LocalMessage",0,y,855,120);var msg=Label(v,row,"Message",170,43,626,300,27);msg.text=item.Body;msg.supportRichText=false;msg.alignment=TextAnchor.UpperLeft;float h=Mathf.Max(125,Mathf.Ceil(msg.preferredHeight*1.25f+16)+60);row.sizeDelta=new Vector2(855,h);msg.rectTransform.sizeDelta=new Vector2(626,h-56);
                var bubble=Panel(row,"Bubble",Load("message_frame"),137,34,687,h-42);bubble.transform.SetAsFirstSibling();msg.color=new Color32(40,28,15,255);Art(row,"Portrait",Resources.Load<Sprite>("ProductionUI/TeamHubAligned/portrait_ivan"),6,0,128,120);
                var name=Label(v,row,"Name",150,0,500,36,26);name.alignment=TextAnchor.MiddleLeft;name.text=en?"Ivan · local only":"Иван · только локально";Label(v,row,"Time",740,3,88,30,20).text=item.Time;y+=h;
            }
            v.Messages.sizeDelta=new Vector2(855,Mathf.Max(v.ChatViewport.rect.height,y));
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name=="reward_frame"?6:s&&(s.name=="message_frame"||s.name=="composer_frame")?3:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionTeamChatView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
