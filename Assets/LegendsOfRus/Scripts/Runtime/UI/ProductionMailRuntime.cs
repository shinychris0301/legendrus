using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionMailRuntime
    {
        public static Sprite Load(string key)=>ProductionBoosterArt.Replacement("MailAligned/"+key)??Resources.Load<Sprite>("ProductionUI/MailAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionMailView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Mail outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionMailView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Mail needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"MAIL_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),198,105,542,161);v.Heading=Label(v,v.Body,"Heading",299,168,344,81,57);v.Heading.font=v.TitleFont;
            v.PreviewLabel=Label(v,v.Body,"Preview",82,341,777,28,19);Fit(v.PreviewLabel,14,19);
            v.Tabs=new Button[3];v.TabLabels=new Text[3];for(int i=0;i<3;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Shared("tab"),76+i*265,274,264,67));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",14,9,236,47,29);Fit(v.TabLabels[i],20,29);}
            v.DynamicRoot=Rect(v.Body,"Letters",0,0,941,1672);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),20,1492,132,131));
            v.ComposeButton=Button(Art(v.Body,"Compose",Load("compose"),163,1502,112,111));v.EnvelopeButton=Button(Art(v.Body,"Inbox",Load("envelope"),675,1502,112,114));v.DeleteButton=Button(Art(v.Body,"Delete",Load("trash"),800,1492,126,131));
            v.ClaimAllButton=Button(Art(v.Body,"ClaimAll",Load("claim_frame"),276,1493,394,123));v.ClaimAllLabel=Label(v,v.ClaimAllButton.transform,"Label",51,32,292,58,37);Fit(v.ClaimAllLabel,27,37);
            v.Counter=Label(v,v.Body,"Count",270,1620,401,39,22);Fit(v.Counter,17,22);
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
            bool en=DemoFeatureState.LanguageCode=="en";int[] indices=v.SelectedTab==MailTab.Sent?new int[0]:v.SelectedTab==MailTab.System?new[]{3}:new[]{0,1,2,3,4};v.VisibleCount=indices.Length;v.Counter.text=en?"EXAMPLE LETTERS: "+indices.Length:"ПРИМЕРОВ ПИСЕМ: "+indices.Length;
            v.PreviewLabel.rectTransform.anchoredPosition=new Vector2(82,v.DetailOpen?-280:-341);
            if(v.DetailOpen){BuildLetter(v,v.SelectedMessage,true);return;}
            if(v.UnreadCount>0){Badge(v.DynamicRoot,"InboxUnread",308,263,36);Label(v,v.DynamicRoot,"InboxUnreadCount",308,263,36,36,22).text=v.UnreadCount.ToString();}
            if(v.IsUnread(3)){Badge(v.DynamicRoot,"SystemUnread",838,263,36);Label(v,v.DynamicRoot,"SystemUnreadCount",838,263,36,36,22).text="1";}
            Panel(v.DynamicRoot,"ListFrame",Shared("table_frame"),31,367,879,649);
            if(indices.Length==0){var empty=Label(v,v.DynamicRoot,"Empty",137,560,667,210,34);empty.text=en?"No sent sample messages.\nOnline mail is not connected.":"Нет отправленных примеров писем.\nОнлайн-почта не подключена.";Fit(empty,25,34);return;}
            for(int j=0;j<indices.Length;j++){
                int i=indices[j];var row=Rect(v.DynamicRoot,"Message_"+i,48,376+j*126,845,123);var button=Button(Panel(row,"Open",Shared("row"),0,0,845,123));button.onClick.AddListener(()=>v.OpenMessage(i));if(v.IsUnread(i))Badge(row,"Unread",10,11,18);Art(row,"Sender",Load("sender_"+i),33,3,122,119);
                var title=Label(v,row,"Subject",173,6,454,42,28);title.alignment=TextAnchor.MiddleLeft;title.text=en?SubjectsEn[i]:SubjectsRu[i];Fit(title,20,28);
                var summary=Label(v,row,"Summary",174,49,445,67,23);summary.alignment=TextAnchor.MiddleLeft;summary.text=en?SummaryEn[i]:SummaryRu[i];Fit(summary,18,23);
                Art(row,"Arrow",Load("arrow"),787,42,39,45);var status=Label(v,row,"Sample",626,5,157,27,17);status.text=en?"EXAMPLE":"ПРИМЕР";
                for(int k=0;k<Math.Min(2,Rewards[i].Length);k++)Reward(v,row,Rewards[i][k],Amounts[i][k],633+k*67,37,64,79,false);
            }
            BuildLetter(v,v.SelectedMessage,false);
        }
        static void BuildLetter(ProductionMailView v,int index,bool full){
            bool en=DemoFeatureState.LanguageCode=="en";var p=v.DynamicRoot;float top=full?316:1030,height=full?1164:447;
            Panel(p,"LetterParchment",Load(full?"letter_frame":"preview_frame"),46,top,850,height);
            Art(p,"LetterSender",Load("sender_"+index),full?102:76,top+(full?45:25),full?156:172,full?156:172);
            var subject=Label(v,p,"LetterSubject",full?280:270,top+(full?50:24),full?560:568,full?87:53,full?36:28);subject.alignment=TextAnchor.MiddleLeft;subject.color=Ink;subject.text=en?SubjectsEn[index]:SubjectsRu[index];Fit(subject,full?26:20,full?36:28);
            var sender=Label(v,p,"SenderName",full?280:72,top+(full?142:198),full?560:194,full?48:65,full?27:22);sender.color=Ink;sender.text=en?NamesEn[index]:NamesRu[index];Fit(sender,18,full?27:22);
            string body=index==2?(en?"Good day, Ivan!\n\nThank you for helping our guild grow. Your courage and loyalty make us stronger every day.\nKeep up the good work! Together we are invincible.\n\nWith respect,\nThe Guild Council":"Добрый день, Иван!\n\nБлагодарим тебя за вклад в развитие нашей Гильдии! Твоя активность и преданность помогают нам становиться сильнее каждый день.\n\nПродолжай в том же духе! Вместе мы непобедимы!\n\nС уважением,\nСовет Гильдии"):(en?SummaryEn[index]+"\n\nThis letter and its attachments are design examples. No rewards can be claimed yet.":SummaryRu[index]+"\n\nЭто пример письма и вложений для дизайна. Получение наград пока недоступно.");
            if(!full)body=(en?SummaryEn[index]+"\n\nDesign example — rewards unavailable.":SummaryRu[index]+"\n\nПример — награды недоступны.");
            var text=Label(v,p,"LetterBody",full?107:283,top+(full?217:92),full?727:551,full?508:198,full?29:24);text.alignment=TextAnchor.UpperLeft;text.color=Ink;text.text=body;Fit(text,full?23:19,full?29:24);
            var rewards=Label(v,p,"AttachmentsLabel",full?200:277,top+(full?740:297),full?542:560,42,full?29:24);rewards.color=Ink;rewards.text=en?(Rewards[index].Length>0?"ATTACHMENTS · EXAMPLES":"NO ATTACHMENTS"):(Rewards[index].Length>0?"ВЛОЖЕНИЯ · ПРИМЕРЫ":"БЕЗ ВЛОЖЕНИЙ");Fit(rewards,18,full?29:24);
            for(int k=0;k<Rewards[index].Length;k++)Reward(v,p,Rewards[index][k],Amounts[index][k],full?470-Rewards[index].Length*91+k*182:286+k*78,top+(full?800:339),full?160:70,full?169:80,true);
            if(Rewards[index].Length>0){var claim=Button(Art(p,"Claim",Load("claim_frame"),full?209:555,top+(full?990:339),full?526:282,full?147:79));claim.onClick.AddListener(v.ShowInfo);var l=Label(v,claim.transform,"Label",full?73:34,full?40:21,full?380:214,full?64:39,full?42:27);l.font=v.TitleFont;l.text=en?"CLAIM":"ЗАБРАТЬ";Fit(l,22,full?42:27);}
        }
        static void Badge(Transform p,string name,float x,float y,float size){var a=Rect(p,name,x,y,size,size).gameObject.AddComponent<ProductionHomePanel>();a.Frame=true;a.RaisedBadge=true;a.Radius=size*.5f;a.BorderColor=new Color32(184,132,53,255);a.LeftColor=a.RightColor=new Color32(130,22,8,255);a.raycastTarget=false;}
        static Color Ink=>new Color32(44,29,15,255);
        static void Reward(ProductionMailView v,Transform p,string icon,string amount,float x,float y,float w,float h,bool frame){if(frame)Panel(p,"AttachmentFrame",Shared("row"),x,y,w,h);Art(p,"Attachment_"+icon,Load(icon),x+8,y+5,w-16,h*.68f);var t=Label(v,p,"Amount",x+3,y+h*.7f,w-6,h*.29f,frame&&w>100?28:19);t.text=amount;Fit(t,14,frame&&w>100?28:19);}
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="letter_frame"||s.name=="preview_frame")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="letter_frame"||s.name=="preview_frame")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionMailView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
