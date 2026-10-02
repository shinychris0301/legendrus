using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionInventoryRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/InventoryAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionInventoryView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Inventory outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionInventoryView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Inventory needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"INVENTORY_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),159,124,628,165);v.Heading=Label(v,v.Body,"Heading",235,181,472,81,56);Fit(v.Heading,34,56);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),28,214,74,74));
            v.Tabs=new Button[4];v.TabLabels=new Text[4];for(int i=0;i<4;i++){v.Tabs[i]=Button(Panel(v.Body,"Tab_"+i,Shared("tab"),22+i*225,291,220,65));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",14,9,192,46,29);Fit(v.TabLabels[i],19,29);}
            Panel(v.Body,"GridFrame",Shared("table_frame"),9,360,922,746);v.PreviewLabel=Label(v,v.Body,"Availability",76,360,790,27,20);Fit(v.PreviewLabel,15,20);v.DynamicRoot=Rect(v.Body,"InventoryItems",0,0,941,1672);
            v.DetailArt=Art(v.Body,"SelectedItem",Load("feather_detail"),12,1093,291,326);Panel(v.Body,"DetailPaper",Load("paper"),299,1102,629,402);
            Panel(v.Body,"QuantityPanel",Shared("row"),22,1416,275,82);v.QuantityLabel=Label(v,v.Body,"QuantityLabel",37,1422,245,32,25);Fit(v.QuantityLabel,18,25);v.QuantityText=Label(v,v.Body,"Quantity",42,1451,235,43,35);Fit(v.QuantityText,22,35);
            v.ItemName=Label(v,v.Body,"ItemName",335,1125,563,53,37);Fit(v.ItemName,23,37);v.Description=Label(v,v.Body,"Description",344,1190,543,190,29);v.Description.alignment=TextAnchor.UpperLeft;Fit(v.Description,22,29);v.ItemName.color=v.Description.color=new Color32(46,29,12,255);
            v.UseButton=Button(Art(v.Body,"Use",Load("use"),321,1402,283,82));v.UseLabel=Label(v,v.UseButton.transform,"Label",33,22,217,40,25);Fit(v.UseLabel,18,25);v.UseButton.interactable=false;
            v.DetailsButton=Button(Art(v.Body,"Details",Load("details"),611,1402,290,82));v.DetailsLabel=Label(v,v.DetailsButton.transform,"Label",32,22,226,40,27);Fit(v.DetailsLabel,20,27);
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];string[] nav={"offers","gems","bundles","boosters","vip"};for(int i=0;i<5;i++){v.NavButtons[i]=Button(Art(v.Body,"Navigation_"+i,Resources.Load<Sprite>("ProductionUI/ShopAligned/nav_"+nav[i]),14+i*185,1504,173,161));Panel(v.NavButtons[i].transform,"Plaque",Shared("tab"),0,117,173,43);v.NavLabels[i]=Label(v,v.NavButtons[i].transform,"Label",9,123,155,30,21);Fit(v.NavLabels[i],14,21);}
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionInventoryLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Inventory bindings incomplete.");
        }
        static void BuildInfo(ProductionInventoryView v){v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.9f);shield.raycastTarget=true;Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),77,469,787,709);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",120,505,701,99,37);Fit(v.InfoHeading,25,37);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",132,638,677,340,30);Fit(v.InfoDescription,23,30);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1040,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);}
        public static Sprite ItemArt(int id)=>id>=20?ProductionGameplayArt.Booster(id-20):Load("item_"+id);
        public static void RebuildContent(ProductionInventoryView v){
            for(int i=v.DynamicRoot.childCount-1;i>=0;i--){var go=v.DynamicRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool boosters=v.SelectedTab==InventoryTab.Boosters;var ids=ProductionInventoryView.VisibleIds(v.SelectedTab);
            for(int i=0;i<ids.Length;i++){int id=ids[i];float x=boosters?91+i%2*392:39+i%5*174,y=boosters?405+i/2*323:390+i/5*175;float w=boosters?368:173,h=boosters?303:174;
                if(id==v.SelectedItem){var highlight=Panel(v.DynamicRoot,"SelectedHighlight",Shared("tab_active"),x-4,y-4,w+8,h+8);highlight.color=new Color32(255,220,145,255);}
                var b=Button(boosters?Panel(v.DynamicRoot,"Item_"+id,Shared("reward_panel"),x,y,w,h):Art(v.DynamicRoot,"Item_"+id,ItemArt(id),x,y,w,h));b.onClick.AddListener(()=>v.SelectItem(id));
                if(boosters){Art(b.transform,"Artwork",ItemArt(id),97,14,174,184);var title=Label(v,b.transform,"Name",23,198,322,58,26);title.text=ProductionInventoryView.ItemTitle(id);Fit(title,20,26);}
                var count=Label(v,b.transform,"Count",boosters?60:95,boosters?257:130,boosters?248:62,boosters?36:32,boosters?28:24);count.text=boosters?ProductionInventoryView.Number(v.BoosterQuantity(id-20)):"—";Fit(count,14,boosters?28:24);
            }
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="paper")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="paper")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionInventoryView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
