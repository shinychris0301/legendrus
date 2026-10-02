using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionMailRuntime
    {
        public static Sprite Load(string key)=>key=="gem"?ProductionGameplayArt.Gem(1):ProductionBoosterArt.Replacement("MailAligned/"+key)??Resources.Load<Sprite>("ProductionUI/MailAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionMailView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Mail outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionMailView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Mail needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"MAIL_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);
            v.Environment.color=new Color(.68f,.68f,.72f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            var letterBacking=Art(v.Body,"LetterTitleBacking",Load("letter_title_backing_r5"),0,143,941,153);letterBacking.preserveAspect=false;
            var shared=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);shared.name="NativeHud";
            v.Header=shared;v.Hud=(RectTransform)shared.transform;
            v.ProfileButton=shared.Profile;v.SettingsButton=shared.Settings;v.HeroName=shared.PlayerName;v.LevelText=shared.Level;
            v.PlusButtons=shared.Plus;v.GoldText=shared.Balances[0];v.GemsText=shared.Balances[1];v.EnergyText=shared.Balances[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),198,105,542,161);v.Heading=Label(v,v.Body,"Heading",299,168,344,81,57);v.Heading.font=v.TitleFont;
            v.PreviewLabel=Label(v,v.Body,"Preview",82,341,777,28,19);Fit(v.PreviewLabel,14,19);
            v.TabStrip=Rect(v.Body,"JoinedTabs",30,262,881,89);
            Art(v.TabStrip,"OriginalTrim",Load("tabs_joined_r2"),0,0,881,89);
            v.Tabs=new Button[3];v.TabLabels=new Text[3];
            for(int i=0;i<3;i++){
                v.Tabs[i]=Button(Art(v.TabStrip,"Tab_"+i,Load("tab_face_r2"),59+i*263,22,244,52));
                v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",8,1,228,49,29);v.TabLabels[i].font=v.TitleFont;Fit(v.TabLabels[i],21,29);
            }
            v.DynamicRoot=Rect(v.Body,"Letters",0,0,941,1672);
            var footer=Art(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1486,941,186);footer.preserveAspect=false;footer.color=new Color(.65f,.65f,.65f,1);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),20,1492,132,131));
            v.ComposeButton=Button(Art(v.Body,"Compose",Load("compose"),163,1502,112,111));v.EnvelopeButton=Button(Art(v.Body,"Inbox",Load("envelope"),675,1502,112,114));v.DeleteButton=Button(Art(v.Body,"Delete",Load("trash"),800,1492,126,131));
            v.ClaimAllButton=Button(Art(v.Body,"ClaimAll",Load("claim_all_ru_rebuild"),277,1480,393,138));v.ClaimAllLabel=Label(v,v.ClaimAllButton.transform,"Label",51,32,292,58,37);Fit(v.ClaimAllLabel,27,37);
            v.Counter=Label(v,v.Body,"Count",170,1623,600,35,19);Fit(v.Counter,15,19);
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionMailLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Mail bindings incomplete.");
        }
        static void BuildInfo(ProductionMailView v){
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,402,817,838);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",108,431,725,102,40);Fit(v.InfoHeading,29,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",114,556,713,492,29);Fit(v.InfoDescription,23,29);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1118,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        static readonly string[] SubjectsRu={"Щедрый дар от Добрыни","Благословение от Василисы","Гильдия богатырей","Системное сообщение","Тайный совет"};
        static readonly string[] SubjectsEn={"A generous gift from Dobrynya","Vasilisa's blessing","Guild of heroes","System message","Secret advice"};
        static readonly string[] NamesRu={"Добрыня Никитич","Василиса","Совет Гильдии","Система","Таинственная гостья"};
        static readonly string[] NamesEn={"Dobrynya Nikitich","Vasilisa","Guild Council","System","Mysterious visitor"};
        static readonly string[] SummaryRu={"Держи, Иван! Это тебе пригодится в твоих подвигах!","Пусть эта сила оберегает тебя на пути к славе!","Пример еженедельного письма гильдии.","Пример сообщения об обновлении игры.","Тени шепчут... Проверь, что скрыто в ночной тишине."};
        static readonly string[] SummaryEn={"Here, Ivan! This will help you on your adventures!","May this strength protect you on the path to glory!","An example of a weekly guild letter.","An example of a game update notice.","The shadows whisper... Discover what the night conceals."};
        static readonly string[][] Rewards={new[]{"gem"},new[]{"potion"},new[]{"coin","gem","hammer"},new string[0],new[]{"book"}};
        static readonly string[][] Amounts={new[]{"200"},new[]{"5"},new[]{"5 000","100","×1"},new string[0],new[]{"3"}};
        public static void RebuildContent(ProductionMailView v){
            for(int i=v.DynamicRoot.childCount-1;i>=0;i--){var go=v.DynamicRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool en=DemoFeatureState.LanguageCode=="en";int[] indices=v.SelectedTab==MailTab.Sent?new int[0]:v.SelectedTab==MailTab.System?new[]{3}:new[]{0,1,2,3,4};v.VisibleCount=indices.Length;v.Counter.text=en?"EXAMPLES: "+indices.Length+" · CLAIMS UNAVAILABLE":"ПРИМЕРЫ: "+indices.Length+" · НАГРАДЫ НЕДОСТУПНЫ";
            ConfigurePresentation(v,en);
            if(v.DetailOpen){BuildLetter(v,v.SelectedMessage,true);v.PreviewLabel.transform.SetAsLastSibling();v.Title.transform.SetAsLastSibling();v.Heading.transform.SetAsLastSibling();v.InfoPanel.transform.SetAsLastSibling();return;}
            if(v.UnreadCount>0){Badge(v.DynamicRoot,"InboxUnread",308,263,36);Label(v,v.DynamicRoot,"InboxUnreadCount",308,263,36,36,22).text=v.UnreadCount.ToString();}
            if(v.IsUnread(3)){Badge(v.DynamicRoot,"SystemUnread",838,263,36);Label(v,v.DynamicRoot,"SystemUnreadCount",838,263,36,36,22).text="1";}
            Panel(v.DynamicRoot,"ListFrame",Shared("table_frame"),31,346,879,686);
            if(indices.Length==0){var empty=Label(v,v.DynamicRoot,"Empty",137,560,667,210,34);empty.text=en?"No sent sample messages.\nOnline mail is not connected.":"Нет отправленных примеров писем.\nОнлайн-почта не подключена.";Fit(empty,25,34);return;}
            for(int j=0;j<indices.Length;j++){
                int i=indices[j];var row=Rect(v.DynamicRoot,"Message_"+i,48,354+j*135,845,130);var button=Button(Art(row,"Open",Load("row_rebuild"),0,0,845,130));button.onClick.AddListener(()=>v.OpenMessage(i));if(v.IsUnread(i))Badge(row,"Unread",10,11,18);Art(row,"Sender",Load("sender_"+i),35,3,124,124);
                var title=Label(v,row,"Subject",177,8,442,42,27);title.alignment=TextAnchor.MiddleLeft;title.text=en?SubjectsEn[i]:SubjectsRu[i];Fit(title,20,27);
                var summary=Label(v,row,"Summary",177,52,402,66,23);summary.alignment=TextAnchor.MiddleLeft;summary.text=en?SummaryEn[i]:SummaryRu[i];Fit(summary,18,23);
                Art(row,"Arrow",Load("arrow"),787,42,39,45);var status=Label(v,row,"Sample",626,5,157,27,17);status.text=en?"EXAMPLE":"ПРИМЕР";
                for(int k=0;k<Math.Min(2,Rewards[i].Length);k++)Reward(v,row,Rewards[i][k],Amounts[i][k],Rewards[i].Length>1?588+k*88:630,39,80,80,true);
            }
            BuildLetter(v,v.SelectedMessage,false);
        }
        static void BuildLetter(ProductionMailView v,int index,bool full){
            if(full){BuildOpenedLetter(v,index);return;}
            bool en=DemoFeatureState.LanguageCode=="en";var p=v.DynamicRoot;float top=full?316:1030,height=full?1164:447;
            Panel(p,"LetterParchment",Load(full?"letter_surface_r4":"preview_surface_r4"),46,top,850,height);
            Art(p,"LetterSender",Load("sender_"+index),full?102:79,top+(full?45:22),full?156:196,full?156:184);
            if(!full){
                var original=p.Find("LetterSender").gameObject;original.SetActive(index!=0);
                Art(p,index==0?"PreviewPortraitAndPlaque":"NamePlaque",Load(index==0?"preview_portrait_r2":"preview_nameplate_r2"),74,top+(index==0?19:180),202,index==0?239:78);
                Art(p,"SubjectDivider",Load("preview_rule_r2"),292,top+67,549,9);
                if(Rewards[index].Length>0)Reward(v,p,Rewards[index][0],Amounts[index][0],704,top+110,140,145,true);
            }
            var subject=Label(v,p,"LetterSubject",full?280:292,top+(full?50:22),full?560:551,full?87:46,full?36:29);subject.alignment=TextAnchor.MiddleLeft;if(!full)subject.font=v.TitleFont;subject.color=Ink;subject.text=en?SubjectsEn[index]:SubjectsRu[index];Fit(subject,full?26:20,full?36:28);
            var sender=Label(v,p,"SenderName",full?280:89,top+(full?142:191),full?560:172,full?48:55,full?27:24);sender.color=full?Ink:new Color32(243,206,137,255);sender.text=en?NamesEn[index]:NamesRu[index];if(!full){sender.font=v.TitleFont;sender.text=sender.text.ToUpperInvariant();if(index==0)sender.text=en?"DOBRYNYA\nNIKITICH":"ДОБРЫНЯ\nНИКИТИЧ";}Fit(sender,18,full?27:24);
            string body=index==2?(en?"Good day, Ivan!\n\nThank you for helping our guild grow. Your courage and loyalty make us stronger every day.\nKeep up the good work! Together we are invincible.\n\nWith respect,\nThe Guild Council":"Добрый день, Иван!\n\nБлагодарим тебя за вклад в развитие нашей Гильдии! Твоя активность и преданность помогают нам становиться сильнее каждый день.\n\nПродолжай в том же духе! Вместе мы непобедимы!\n\nС уважением,\nСовет Гильдии"):(en?SummaryEn[index]+"\n\nThis letter and its attachments are design examples. No rewards can be claimed yet.":SummaryRu[index]+"\n\nЭто пример письма и вложений для дизайна. Получение наград пока недоступно.");
            if(!full)body=index==0?(en?"Greetings, Ivan!\nFor your courage and loyalty to Rus, accept this humble gift. May it help you through the trials and glorious deeds ahead!\n\nFor the glory of Rus!":"Приветствую, Иван!\nЗа твою доблесть и верность Руси прими этот скромный дар. Пусть он поможет тебе в грядущих испытаниях и славных подвигах!\n\nВо славу Руси!"):(en?SummaryEn[index]:SummaryRu[index]);
            var text=Label(v,p,"LetterBody",full?107:292,top+(full?217:82),full?727:393,full?508:185,full?29:25);text.alignment=TextAnchor.UpperLeft;if(!full)text.font=v.TitleFont;text.color=Ink;text.text=body;Fit(text,full?23:21,full?29:25);
            var attachments=Rect(p,"AttachmentGroup",0,0,941,1672);
            if(!full){
                Art(attachments,"RewardDividerLeft",Load("preview_divider_r2"),279,top+283,129,18);
                var right=Art(attachments,"RewardDividerRight",Load("preview_divider_r2"),643,top+283,129,18);right.rectTransform.localScale=new Vector3(-1,1,1);right.rectTransform.anchoredPosition=new Vector2(772,-top-283);
            }
            var rewards=Label(v,attachments,"AttachmentsLabel",full?200:414,top+(full?740:274),full?542:220,42,full?29:24);if(!full)rewards.font=v.TitleFont;rewards.color=Ink;rewards.text=en?(Rewards[index].Length>0?"ATTACHMENTS · EXAMPLES":"NO ATTACHMENTS"):(Rewards[index].Length>0?"ВЛОЖЕНИЯ · ПРИМЕРЫ":"БЕЗ ВЛОЖЕНИЙ");if(!full)rewards.text=en?(Rewards[index].Length>0?"REWARD EXAMPLES":"NO ATTACHMENTS"):(Rewards[index].Length>0?"НАГРАДЫ · ПРИМЕР":"БЕЗ ВЛОЖЕНИЙ");Fit(rewards,18,full?29:24);
            for(int k=0;k<Rewards[index].Length;k++)Reward(v,attachments,Rewards[index][k],Amounts[index][k],full?470-Rewards[index].Length*91+k*182:297+k*80,top+(full?800:317),full?160:Rewards[index].Length==1?102:72,full?169:106,true);
            if(Rewards[index].Length>0){var claim=Button(Art(attachments,"Claim",Load(en?"claim_frame":"claim_ru_rebuild"),full?209:532,top+(full?990:321),full?526:315,full?147:94));claim.onClick.AddListener(v.ShowInfo);var l=Label(v,claim.transform,"Label",full?73:42,full?40:24,full?380:231,full?64:44,full?42:29);l.font=v.TitleFont;l.text=en?"CLAIM":"ЗАБРАТЬ";l.enabled=en;Fit(l,22,full?42:27);}
        }
        // The opened letter has its own source hierarchy; the compact Inbox preview stays independent.
        static void BuildOpenedLetter(ProductionMailView v,int index)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            var p=v.DynamicRoot;
            Panel(p,"LetterParchment",Load("letter_surface_r4"),10,296,921,1236);
            Art(p,"LetterSender",Load(index==2?"guild_emblem_r3":"sender_"+index),92,344,146,148);
            var sender=LetterText(v,p,"SenderName",250,370,587,57,43);
            sender.text=index==2?(en?"GUILD":"ГИЛЬДИЯ"):(en?NamesEn[index]:NamesRu[index]);
            var role=LetterText(v,p,"SenderRole",250,431,587,41,29);
            role.text=index==2?(en?"Guild leaders":"Лидеры Гильдии"):(en?"Sample message":"Пример письма");
            Art(p,"SenderDivider",Load("letter_rule_r3"),105,492,731,12);
            var subject=LetterText(v,p,"LetterSubject",105,506,731,48,31);
            subject.text=(en?"Subject: ":"Тема: ")+(index==2?(en?"Reward for your contribution!":"Награда за активность!"):(en?SubjectsEn[index]:SubjectsRu[index]));
            Art(p,"SubjectDivider",Load("letter_divider_r3"),103,559,735,32);
            var greeting=LetterText(v,p,"LetterGreeting",112,602,722,49,34);
            greeting.text=en?"Good day, Ivan!":"Добрый день, Иван!";
            var body=LetterText(v,p,"LetterBody",112,664,722,300,31);
            body.alignment=TextAnchor.UpperLeft;body.lineSpacing=1.2f;
            body.text=index==2?(en?"Thank you for helping our guild grow! Your courage and loyalty make us stronger every day. As a token of our appreciation for your efforts, we have prepared a reward for you.\nKeep up the good work!\nTogether we are invincible!":"Благодарим тебя за вклад в развитие нашей Гильдии! Твоя активность и преданность помогают нам становиться сильнее каждый день. В знак признательности за твою работу мы подготовили для тебя награду.\nПродолжай в том же духе!\nВместе мы непобедимы!"):(en?SummaryEn[index]+"\n\nThis letter and its attachments are design examples. No rewards can be claimed yet.":SummaryRu[index]+"\n\nЭто пример письма и вложений для дизайна. Получение наград пока недоступно.");
            var closing=LetterText(v,p,"LetterClosing",112,977,722,82,31);
            closing.alignment=TextAnchor.UpperLeft;
            closing.text=(en?"With respect,\n":"С уважением,\n")+(en?NamesEn[index]:NamesRu[index]);
            var attachments=Rect(p,"AttachmentGroup",0,0,941,1672);
            Art(attachments,"BodyDivider",Load("letter_rule_r3"),105,1068,731,12);
            Art(attachments,"RewardDividerLeft",Load("preview_divider_r2"),199,1097,137,18);
            var right=Art(attachments,"RewardDividerRight",Load("preview_divider_r2"),609,1097,137,18);
            right.rectTransform.localScale=new Vector3(-1,1,1);right.rectTransform.anchoredPosition=new Vector2(746,-1097);
            var title=LetterText(v,attachments,"AttachmentsLabel",342,1084,261,42,27);
            title.alignment=TextAnchor.MiddleCenter;
            title.text=Rewards[index].Length>0?(en?"ATTACHMENTS":"ПРИЛОЖЕНИЯ"):(en?"NO ATTACHMENTS":"БЕЗ ВЛОЖЕНИЙ");
            for(int k=0;k<Rewards[index].Length;k++)Reward(v,attachments,Rewards[index][k],Amounts[index][k],470-Rewards[index].Length*95+12+k*190,1124,166,169,true);
            if(Rewards[index].Length>0){
                var claim=Button(Art(attachments,"Claim",Load(en?"claim_frame":"claim_ru_rebuild"),169,1336,603,153));
                claim.onClick.AddListener(v.ShowInfo);
                var caption=Label(v,claim.transform,"Label",85,40,433,68,42);caption.font=v.TitleFont;
                caption.text=en?"CLAIM":"ЗАБРАТЬ";caption.enabled=en;Fit(caption,28,43);
            }
        }
        static void ConfigurePresentation(ProductionMailView v,bool en)
        {
            bool detail=v.DetailOpen;
            v.Body.Find("LetterTitleBacking").gameObject.SetActive(detail);
            v.Title.sprite=Load(detail?(en?"letter_title_blank_r5":"letter_title_ru_r5"):(en?"title_blank":"title_ru"));
            Place(v.Title.rectTransform,detail?187:198,detail?109:105,detail?570:542,detail?200:161);
            Place(v.Heading.rectTransform,detail?317:299,detail?181:168,detail?307:344,detail?79:81);
            v.Heading.fontSize=detail?64:57;
            v.ComposeButton.gameObject.SetActive(!detail);v.DeleteButton.gameObject.SetActive(!detail);v.ClaimAllButton.gameObject.SetActive(!detail);
            v.BackButton.image.sprite=detail?Load("letter_back_r5"):Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2");
            v.EnvelopeButton.image.sprite=Load(detail?"letter_inbox_r5":"envelope");
            ((RectTransform)v.BackButton.transform).sizeDelta=detail?new Vector2(167,168):new Vector2(132,131);
            ((RectTransform)v.EnvelopeButton.transform).sizeDelta=detail?new Vector2(171,168):new Vector2(112,114);
            v.Counter.rectTransform.sizeDelta=detail?new Vector2(530,66):new Vector2(600,35);
            if(detail)v.Counter.text=en?"SAMPLE LETTER\nREWARDS UNAVAILABLE":"ПРИМЕР ПИСЬМА\nНАГРАДЫ НЕДОСТУПНЫ";
            v.PreviewLabel.gameObject.SetActive(detail);
            Place(v.PreviewLabel.rectTransform,detail?248:82,detail?340:341,detail?600:777,28);
            v.PreviewLabel.color=detail?Ink:new Color32(243,206,137,255);
            if(detail)v.PreviewLabel.text=en?"SAMPLE MAIL · CLAIMS UNAVAILABLE":"ПРИМЕР ПИСЬМА · НАГРАДЫ НЕДОСТУПНЫ";
            // The title's lower sapphire sits in front of the top paper rail.
            v.Title.transform.SetAsLastSibling();v.Heading.transform.SetAsLastSibling();v.InfoPanel.transform.SetAsLastSibling();
        }
        static void Place(RectTransform rect,float x,float y,float w,float h)
        {rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);}
        static Text LetterText(ProductionMailView v,Transform parent,string name,float x,float y,float w,float h,int size)
        {
            var text=Label(v,parent,name,x,y,w,h,size);text.font=v.TitleFont;text.color=Ink;
            text.alignment=TextAnchor.MiddleLeft;Fit(text,Mathf.Max(22,size-5),size);return text;
        }
        static void Badge(Transform p,string name,float x,float y,float size){var a=Rect(p,name,x,y,size,size).gameObject.AddComponent<ProductionHomePanel>();a.Frame=true;a.RaisedBadge=true;a.Radius=size*.5f;a.BorderColor=new Color32(184,132,53,255);a.LeftColor=a.RightColor=new Color32(130,22,8,255);a.raycastTarget=false;}
        static Color Ink=>new Color32(44,29,15,255);
        static void Reward(ProductionMailView v,Transform p,string icon,string amount,float x,float y,float w,float h,bool frame){if(frame)Panel(p,"AttachmentFrame",Shared("reward_frame"),x,y,w,h);Art(p,"Attachment_"+icon,Load(icon),x+8,y+5,w-16,h*.68f);var t=Label(v,p,"Amount",x+3,y+h*.7f,w-6,h*.29f,frame&&w>100?28:19);t.text=amount;Fit(t,14,frame&&w>100?28:19);}
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="letter_frame"||s.name=="preview_frame"||s.name=="preview_paper_r2"||s.name=="letter_surface_r4"||s.name=="preview_surface_r4")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="letter_frame"||s.name=="preview_frame"||s.name=="preview_paper_r2"||s.name=="letter_surface_r4"||s.name=="preview_surface_r4")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionMailView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
