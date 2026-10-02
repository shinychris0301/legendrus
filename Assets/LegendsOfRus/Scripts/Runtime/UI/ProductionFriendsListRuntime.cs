using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionFriendsListRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/FriendsListAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionFriendsListView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Friends List outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionFriendsListView>();
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"FRIENDS_LIST_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);v.Environment.color=new Color(.68f,.68f,.72f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            v.ProfileButton=Button(Art(v.Hud,"Profile",Shared("rules"),0,0,174,235));
            v.SettingsButton=Button(Art(v.Hud,"Settings",Shared("rules"),853,23,88,95));
            v.HeroName=Label(v,v.Hud,"HeroName",180,30,150,44,34);v.LevelText=Label(v,v.Hud,"Level",180,75,150,29,24);
            v.PlusButtons=new Button[3];Text[] values=new Text[3];
            for(int i=0;i<3;i++){values[i]=Label(v,v.Hud,"Balance_"+i,0,0,80,35,26);v.PlusButtons[i]=Button(Art(v.Hud,"Plus_"+i,Shared("rules"),0,0,44,47));}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];ProductionPlayerHeader.Attach(v);
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),248,115,451,209);v.Heading=Label(v,v.Body,"Heading",315,192,321,78,54);Fit(v.Heading,36,54);
            v.PreviewLabel=Label(v,v.Body,"Preview",163,316,659,20,16);Fit(v.PreviewLabel,12,16);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),42,250,85,81));
            float[] tabX={69,336,602},tabW={267,266,252};v.Tabs=new Button[3];v.TabLabels=new Text[3];for(int i=0;i<3;i++){v.Tabs[i]=Button(Art(v.Body,"Tab_"+i,Load("tab_"+i+"_r2"),tabX[i],337,tabW[i],103));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",25,23,tabW[i]-50,53,31);Fit(v.TabLabels[i],24,31);}
            var search=Panel(v.Body,"Search",Shared("row"),88,444,749,37);search.raycastTarget=true;v.Search=search.gameObject.AddComponent<InputField>();v.Search.targetGraphic=search;v.Search.characterLimit=40;v.Search.lineType=InputField.LineType.SingleLine;
            var text=Label(v,search.transform,"InputText",19,3,708,31,22);text.alignment=TextAnchor.MiddleLeft;text.supportRichText=false;v.Search.textComponent=text;v.SearchPlaceholder=Label(v,search.transform,"Placeholder",19,3,708,31,21);v.SearchPlaceholder.alignment=TextAnchor.MiddleLeft;v.SearchPlaceholder.color=new Color32(184,170,140,255);v.Search.placeholder=v.SearchPlaceholder;
            Panel(v.Body,"TableFrame",Load("outer_frame_r2"),43,479,860,1014);v.RequestsFrame=Panel(v.Body,"RequestsFrame",Load("requests_surround_r4"),7,478,927,946);v.RowsRoot=Rect(v.Body,"Rows",88,490,747,924);v.EmptyLabel=Label(v,v.Body,"Empty",123,773,695,92,34);Fit(v.EmptyLabel,25,34);
            v.SortButton=Button(Art(v.Body,"Sort",Load("sort_ru_r3"),86,1424,223,58));v.SortLabel=Label(v,v.SortButton.transform,"Label",51,10,154,38,23);Fit(v.SortLabel,16,23);
            v.Counter=Label(v,v.Body,"Count",328,1424,251,58,26);Fit(v.Counter,19,26);
            v.RemoveButton=Button(Art(v.Body,"RemoveInactive",Load("remove_ru_r3"),598,1430,256,55));v.RemoveLabel=Label(v,v.RemoveButton.transform,"Label",24,8,208,36,21);Fit(v.RemoveLabel,15,21);
            var footer=Art(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1431,941,241);footer.preserveAspect=false;footer.color=new Color(.65f,.65f,.65f,1);
            // Reuse World Map's complete metalwork. Only the center emblem and live caption change.
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];
            for(int i=0;i<5;i++){
                var face=WorldMapFooter(i);float h=236,w=h*face.rect.width/face.rect.height;
                var button=Button(Art(v.Body,"Navigation_"+i,face,(i+.5f)*188.2f-w*.5f,1430,w,h));v.NavButtons[i]=button;
                if(i!=1){
                    var surface=Rect(button.transform,"MedallionSurface",21,26,144,130).gameObject.AddComponent<ProductionFooterSymbolGraphic>();
                    surface.Source=Resources.Load<Sprite>("ProductionUI/WorldMapReferenceAligned/card_surface");
                    surface.SourceTopRect=new Rect(280,35,144,130);surface.raycastTarget=false;
                    var key=new[]{"map","shop","events","friends","characters"}[i];
                    var sprite=Load("symbol_"+key+"_r7");var bounds=FooterSymbolBounds(i);
                    // Fit the visible isolated silhouette uniformly; retain the entire PNG and its alpha.
                    float visibleHeight=i==4?116:101;float visibleTop=i==4?31:46;
                    float fit=Mathf.Min(102/bounds.width,visibleHeight/bounds.height);
                    float x=42+(102-bounds.width*fit)*.5f-bounds.x*fit;
                    float y=visibleTop+(visibleHeight-bounds.height*fit)*.5f-bounds.y*fit;
                    var symbol=Art(button.transform,"PageSymbol",sprite,x,y,sprite.rect.width*fit,sprite.rect.height*fit);
                    symbol.color=new Color(.76f,.76f,.76f,1);
                }
                v.NavLabels[i]=Label(v,button.transform,"Label",22*h/236f,165*h/236f,w-44*h/236f,35*h/236f,22);Fit(v.NavLabels[i],12,22);
                v.NavLabels[i].horizontalOverflow=HorizontalWrapMode.Overflow;
            }
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionFriendsListLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Friends List bindings incomplete.");
        }
        public static void FitFooterCaption(Text label){
            // One crisp line in both languages, with room for dynamic-font pixel rounding on phones.
            label.resizeTextForBestFit=false;label.fontSize=22;
            while(label.fontSize>12&&(label.preferredWidth>label.rectTransform.rect.width-14||label.preferredHeight>label.rectTransform.rect.height))label.fontSize--;
        }
        public static Rect FooterSymbolBounds(int i){
            // Visible alpha bounds (>8/255), measured from the preserved imagegen outputs.
            if(i==0)return new Rect(163,188,886,876);
            if(i==2)return new Rect(180,124,903,996);
            if(i==3)return new Rect(170,306,915,689);
            return new Rect(182,50,896,1154);
        }
        public static Sprite WorldMapFooter(int index)=>Resources.Load<Sprite>("ProductionUI/WorldMapReferenceAligned/"+(index==1?"nav_shop_blank":index==3?"nav_map_blank":"nav_guild_blank"));
        static void BuildInfo(ProductionFriendsListView v){
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,402,817,838);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",108,431,725,102,40);Fit(v.InfoHeading,29,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",114,556,713,492,29);Fit(v.InfoDescription,23,29);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1118,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        public static void RebuildRows(ProductionFriendsListView v){
            for(int i=v.RowsRoot.childCount-1;i>=0;i--){var go=v.RowsRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool en=DemoFeatureState.LanguageCode=="en",requests=v.SelectedTab==FriendsListTab.Requests;
            v.Body.Find("TableFrame").gameObject.SetActive(!requests);v.RequestsFrame.gameObject.SetActive(requests);
            v.RowsRoot.localScale=Vector3.one*.94f;v.RowsRoot.anchoredPosition=new Vector2((941-(requests?886:747)*.94f)*.5f,-490);v.RowsRoot.sizeDelta=new Vector2(requests?886:747,924);
            string[] ru=requests?new[]{"Маша","Добрыня","Василиса","Мерлин","Любава"}:new[]{"Алёна Прекрасная","Добрыня Никитич","Мудрый Волхв","Василиса Премудрая","Илья Муромец","Снегурочка"};
            string[] eng=requests?new[]{"Masha","Dobrynya","Vasilisa","Merlin","Lyubava"}:new[]{"Alena the Beautiful","Dobrynya Nikitich","Wise Volkhv","Vasilisa the Wise","Ilya Muromets","Snow Maiden"};
            string[] guildRu={"Светлояр","Богатыри Руси","Хранители Леса","Лукоморье","Богатыри Руси","Северное Сияние"},guildEn={"Svetloyar","Heroes of Rus","Forest Guardians","Lukomorye","Heroes of Rus","Northern Lights"};
            var indices=new System.Collections.Generic.List<int>();string q=(v.Search.text??"").Trim();for(int i=0;i<ru.Length;i++){string match=ru[i]+" "+eng[i]+(requests?"":" "+guildRu[i]+" "+guildEn[i]);if(q.Length==0||match.IndexOf(q,StringComparison.OrdinalIgnoreCase)>=0)indices.Add(i);}if(v.SortByName)indices.Sort((a,b)=>string.Compare(en?eng[a]:ru[a],en?eng[b]:ru[b],StringComparison.Ordinal));
            v.VisibleCount=indices.Count;v.EmptyLabel.gameObject.SetActive(indices.Count==0);v.Counter.text=en?"EXAMPLES "+indices.Count+" / "+ru.Length:"ПРИМЕРЫ "+indices.Count+" / "+ru.Length;
            for(int j=0;j<indices.Count;j++){
                int i=indices[j];string name=en?eng[i]:ru[i];float h=requests?178:154,w=requests?886:747;var row=Rect(v.RowsRoot,"Example_"+i,0,j*h,w,h-4);var frame=Art(row,"Frame",Load(requests?"request_card_r4":"row_rebuild"),0,0,w,h-4);frame.preserveAspect=false;Art(row,"Portrait",Load((requests?"request_":"portrait_")+i),requests?39:5,requests?-5:0,requests?186:157,requests?186:h-4);
                var title=Label(v,row,"Name",requests?242:170,requests?28:7,283,requests?53:48,requests?36:29);title.font=v.NamesFont;title.alignment=TextAnchor.MiddleLeft;title.text=name;Fit(title,22,requests?36:29);
                if(requests){var desc=Label(v,row,"Request",242,83,285,81,28);desc.alignment=TextAnchor.MiddleLeft;desc.text=en?"Sample request to add you as a friend":"Пример запроса на добавление в друзья";Fit(desc,22,28);Art(row,"Gift",Load(new[]{"gift_red","gift_blue","gift_green"}[i%3]),538,46,86,88);
                    var accept=Button(Art(row,"Accept",Load("accept"),640,44,89,92));accept.onClick.AddListener(()=>v.ShowInfo(2,name));var decline=Button(Art(row,"Decline",Load("decline"),750,44,88,91));decline.onClick.AddListener(()=>v.ShowInfo(3,name));
                }else{
                    Art(row,"GuildEmblem",Load("flag_"+new[]{"blue","red","green","purple","red","winter"}[i]+"_r2"),171,59,53,64);var guild=Label(v,row,"Guild",233,61,221,42,26);guild.alignment=TextAnchor.MiddleLeft;guild.text=en?guildEn[i]:guildRu[i];Fit(guild,20,26);
                    var status=Label(v,row,"Status",173,115,278,29,21);status.alignment=TextAnchor.MiddleLeft;status.color=i<3?new Color32(171,211,93,255):new Color32(194,188,171,255);status.text=en?(i<3?"● Online · example":"● Offline · example"):(i<3?"● Онлайн · пример":"● Офлайн · пример");
                    bool find=v.SelectedTab==FriendsListTab.Find;var energy=Button(Art(row,"Energy",Load(find?"visit_frame":en?"energy_frame":"energy_ru_rebuild"),464,12,249,70));var energyText=Label(v,energy.transform,"Label",find?20:64,10,find?209:163,44,22);energyText.font=v.TitleFont;energyText.text=find?(en?"ADD FRIEND":"ДОБАВИТЬ"):(en?"SEND ENERGY":"ОТПРАВИТЬ ЭНЕРГИЮ");Fit(energyText,15,22);energyText.enabled=en||find;energy.onClick.AddListener(()=>v.ShowInfo(find?5:0,name));
                    var visit=Button(Art(row,"Visit",Load(en?"visit_frame":"visit_ru_rebuild"),464,84,249,61));var visitText=Label(v,visit.transform,"Label",20,10,209,41,24);visitText.font=v.TitleFont;visitText.text=en?"VISIT":"ПОСЕТИТЬ";Fit(visitText,18,24);visitText.enabled=en;visit.onClick.AddListener(()=>v.ShowInfo(1,name));
                }
            }
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name=="requests_surround_r4"?1:s&&s.name=="chat_surround_r2"?3:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionFriendsListView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
