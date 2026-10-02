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
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Friends List needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"FRIENDS_LIST_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),248,115,451,209);v.Heading=Label(v,v.Body,"Heading",315,192,321,78,54);Fit(v.Heading,36,54);
            v.PreviewLabel=Label(v,v.Body,"Preview",163,312,659,27,18);Fit(v.PreviewLabel,14,18);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),42,250,85,81));
            v.Tabs=new Button[3];v.TabLabels=new Text[3];for(int i=0;i<3;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Shared("tab"),83+i*260,345,254,79));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",18,14,218,51,32);Fit(v.TabLabels[i],22,32);}
            var search=Panel(v.Body,"Search",Shared("row"),88,435,749,51);search.raycastTarget=true;v.Search=search.gameObject.AddComponent<InputField>();v.Search.targetGraphic=search;v.Search.characterLimit=40;v.Search.lineType=InputField.LineType.SingleLine;
            var text=Label(v,search.transform,"InputText",19,6,708,39,25);text.alignment=TextAnchor.MiddleLeft;text.supportRichText=false;v.Search.textComponent=text;v.SearchPlaceholder=Label(v,search.transform,"Placeholder",19,6,708,39,23);v.SearchPlaceholder.alignment=TextAnchor.MiddleLeft;v.SearchPlaceholder.color=new Color32(184,170,140,255);v.Search.placeholder=v.SearchPlaceholder;
            Panel(v.Body,"TableFrame",Shared("table_frame"),57,490,817,889);v.RowsRoot=Rect(v.Body,"Rows",72,495,795,870);v.EmptyLabel=Label(v,v.Body,"Empty",123,773,695,92,34);Fit(v.EmptyLabel,25,34);
            v.SortButton=Button(Panel(v.Body,"Sort",Shared("rules"),82,1390,245,68));v.SortLabel=Label(v,v.SortButton.transform,"Label",15,12,215,42,25);Fit(v.SortLabel,17,25);
            v.Counter=Label(v,v.Body,"Count",328,1389,251,68,24);Fit(v.Counter,17,24);
            v.RemoveButton=Button(Panel(v.Body,"RemoveInactive",Shared("tab"),592,1390,267,68));v.RemoveLabel=Label(v,v.RemoveButton.transform,"Label",15,12,237,42,22);Fit(v.RemoveLabel,15,22);
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];for(int i=0;i<5;i++){v.NavButtons[i]=Button(Art(v.Body,"Navigation_"+i,Load("nav_"+i),12+i*185,1499,180,167));v.NavLabels[i]=Label(v,v.NavButtons[i].transform,"Label",31,120,118,29,20);Fit(v.NavLabels[i],12,20);}
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionFriendsListLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Friends List bindings incomplete.");
        }
        static void BuildInfo(ProductionFriendsListView v){
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),62,402,817,838);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",108,431,725,102,40);Fit(v.InfoHeading,29,40);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",114,556,713,492,29);Fit(v.InfoDescription,23,29);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1118,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        public static void RebuildRows(ProductionFriendsListView v){
            for(int i=v.RowsRoot.childCount-1;i>=0;i--){var go=v.RowsRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool en=DemoFeatureState.LanguageCode=="en",requests=v.SelectedTab==FriendsListTab.Requests;
            string[] ru=requests?new[]{"Маша","Добрыня","Василиса","Мерлин","Любава"}:new[]{"Алёна Прекрасная","Добрыня Никитич","Мудрый Волхв","Василиса Премудрая","Илья Муромец","Снегурочка"};
            string[] eng=requests?new[]{"Masha","Dobrynya","Vasilisa","Merlin","Lyubava"}:new[]{"Alena the Beautiful","Dobrynya Nikitich","Wise Volkhv","Vasilisa the Wise","Ilya Muromets","Snow Maiden"};
            string[] guildRu={"Светлояр","Богатыри Руси","Хранители Леса","Лукоморье","Богатыри Руси","Северное Сияние"},guildEn={"Svetloyar","Heroes of Rus","Forest Guardians","Lukomorye","Heroes of Rus","Northern Lights"};
            var indices=new System.Collections.Generic.List<int>();string q=(v.Search.text??"").Trim();for(int i=0;i<ru.Length;i++){string match=ru[i]+" "+eng[i]+(requests?"":" "+guildRu[i]+" "+guildEn[i]);if(q.Length==0||match.IndexOf(q,StringComparison.OrdinalIgnoreCase)>=0)indices.Add(i);}if(v.SortByName)indices.Sort((a,b)=>string.Compare(en?eng[a]:ru[a],en?eng[b]:ru[b],StringComparison.Ordinal));
            v.VisibleCount=indices.Count;v.EmptyLabel.gameObject.SetActive(indices.Count==0);v.Counter.text=en?"EXAMPLES "+indices.Count+" / "+ru.Length:"ПРИМЕРЫ "+indices.Count+" / "+ru.Length;
            for(int j=0;j<indices.Count;j++){
                int i=indices[j];string name=en?eng[i]:ru[i];float h=requests?172:144;var row=Rect(v.RowsRoot,"Example_"+i,0,j*h,795,h-4);Panel(row,"Frame",Shared("row"),0,0,784,h-5);Art(row,"Portrait",Load((requests?"request_":"portrait_")+i),9,-2,145,h-4);
                var title=Label(v,row,"Name",165,8,302,47,29);title.alignment=TextAnchor.MiddleLeft;title.text=name;Fit(title,20,29);
                if(requests){var desc=Label(v,row,"Request",167,53,285,98,25);desc.alignment=TextAnchor.MiddleLeft;desc.text=en?"Sample request to add you as a friend":"Пример запроса на добавление в друзья";Fit(desc,20,25);Art(row,"Gift",Load(new[]{"gift_red","gift_blue","gift_green"}[i%3]),482,51,72,81);
                    var accept=Button(Art(row,"Accept",Load("accept"),574,41,83,87));accept.onClick.AddListener(()=>v.ShowInfo(2,name));var decline=Button(Art(row,"Decline",Load("decline"),677,41,83,87));decline.onClick.AddListener(()=>v.ShowInfo(3,name));
                }else{
                    Art(row,"GuildEmblem",Resources.Load<Sprite>("ProductionUI/TeamRankingAligned/crest_"+new[]{"eagle","flame","stag","knot","flame","eagle"}[i]),165,62,42,52);var guild=Label(v,row,"Guild",216,56,257,42,24);guild.alignment=TextAnchor.MiddleLeft;guild.text=en?guildEn[i]:guildRu[i];Fit(guild,18,24);
                    var status=Label(v,row,"Status",166,106,297,29,18);status.alignment=TextAnchor.MiddleLeft;status.color=i<3?new Color32(171,211,93,255):new Color32(194,188,171,255);status.text=en?(i<3?"Online · example":"Offline · example"):(i<3?"Онлайн · пример":"Офлайн · пример");
                    bool find=v.SelectedTab==FriendsListTab.Find;var energy=Button(Art(row,"Energy",Load(find?"visit_frame":"energy_frame"),485,14,237,66));var energyText=Label(v,energy.transform,"Label",find?20:64,10,find?197:155,44,22);energyText.font=v.TitleFont;energyText.text=find?(en?"ADD FRIEND":"ДОБАВИТЬ"):(en?"SEND ENERGY":"ОТПРАВИТЬ ЭНЕРГИЮ");Fit(energyText,16,22);energy.onClick.AddListener(()=>v.ShowInfo(find?5:0,name));
                    var visit=Button(Art(row,"Visit",Load("visit_frame"),485,80,237,57));var visitText=Label(v,visit.transform,"Label",20,10,197,37,24);visitText.font=v.TitleFont;visitText.text=en?"VISIT":"ПОСЕТИТЬ";Fit(visitText,18,24);visit.onClick.AddListener(()=>v.ShowInfo(1,name));
                }
            }
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionFriendsListView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
