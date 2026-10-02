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
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.NamesFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");v.NumberFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            v.Content=Rect(root,"INVENTORY_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);v.Environment.color=new Color(.68f,.68f,.72f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            var wood=Art(v.Body,"TitleWood",Resources.Load<Sprite>("ProductionUI/ProfileAligned/wood_band_r1"),0,122,941,239);wood.preserveAspect=false;
            if(!shop.Header||!shop.Header.HasBindings)throw new InvalidOperationException("Inventory requires the shared player header.");
            v.Header=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);v.Header.name="NativeHud";v.Hud=(RectTransform)v.Header.transform;
            foreach(var button in v.Header.GetComponentsInChildren<Button>(true))AddPressFeedback(button);
            v.ProfileButton=v.Header.Profile;v.SettingsButton=v.Header.Settings;v.HeroName=v.Header.PlayerName;v.LevelText=v.Header.Level;v.PlusButtons=v.Header.Plus;
            v.GoldText=v.Header.Balances[0];v.GemsText=v.Header.Balances[1];v.EnergyText=v.Header.Balances[2];
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),159,124,628,165);v.Heading=Label(v,v.Body,"Heading",235,181,472,81,56);Fit(v.Heading,34,56);
            v.BackButton=Button(Art(v.Body,"Back",Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2"),28,214,74,74));
            v.Tabs=new Button[4];v.TabLabels=new Text[4];float[] tabX={17,256,479,698},tabY={286,290,291,290},tabW={230,215,210,223},tabH={72,67,66,67};
            for(int i=0;i<4;i++){
                v.Tabs[i]=Button(Art(v.Body,"Tab_"+i,TabArt(i,i==0,false),tabX[i],tabY[i],tabW[i],tabH[i]));
                // Extend downward only, preserving separation from Back above the first tab.
                v.Tabs[i].image.raycastPadding=new Vector4(0,-(76-tabH[i]),0,0);
                v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",15,i==0?17:11,tabW[i]-30,44,26);Fit(v.TabLabels[i],18,26);
            }
            Panel(v.Body,"GridFrame",Load("grid_frame_r2"),4,356,934,755);v.PreviewLabel=Label(v,v.Body,"Availability",76,380,790,20,16);Fit(v.PreviewLabel,13,16);v.DynamicRoot=Rect(v.Body,"InventoryItems",0,0,941,1672);
            v.DetailArt=Art(v.Body,"SelectedItem",Load("feather_detail"),12,1093,291,326);Panel(v.Body,"DetailPaper",Load("paper"),299,1102,629,402);
            Panel(v.Body,"QuantityPanel",Load("quantity_frame_r2"),35,1412,248,74);v.QuantityLabel=Label(v,v.Body,"QuantityLabel",57,1419,204,26,21);Fit(v.QuantityLabel,16,21);v.QuantityText=Label(v,v.Body,"Quantity",54,1445,210,32,29);Fit(v.QuantityText,16,29);
            v.ItemName=Label(v,v.Body,"ItemName",335,1125,563,53,33);Fit(v.ItemName,22,33);
            v.Description=Label(v,v.Body,"Description",349,1183,533,106,29);v.Description.alignment=TextAnchor.UpperLeft;Fit(v.Description,23,29);
            v.DetailDivider=Art(v.Body,"DetailDivider",Resources.Load<Sprite>("ProductionUI/MailAligned/letter_divider_r3"),349,1295,483,21);
            v.UsageNote=Label(v,v.Body,"UsageNote",349,1320,533,69,27);v.UsageNote.alignment=TextAnchor.UpperLeft;Fit(v.UsageNote,20,27);
            v.ItemName.color=v.Description.color=v.UsageNote.color=new Color32(46,29,12,255);
            v.UseButton=Button(Art(v.Body,"Use",Load("use"),321,1402,283,82));v.UseLabel=Label(v,v.UseButton.transform,"Label",33,22,217,40,23);Fit(v.UseLabel,17,23);v.UseButton.interactable=false;
            v.DetailsButton=Button(Art(v.Body,"Details",Load("details"),611,1402,290,82));v.DetailsLabel=Label(v,v.DetailsButton.transform,"Label",32,22,226,40,24);Fit(v.DetailsLabel,18,24);
            var footer=Art(v.Body,"CarvedFooter",Resources.Load<Sprite>("ProductionUI/EventDetailAligned/footer_r4"),0,1503,941,169);footer.preserveAspect=false;
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];
            for(int i=0;i<5;i++){
                var nav=Clone(shop.NavButtons[i].transform,v.Body);nav.name="Navigation_"+i;((RectTransform)nav).anchoredPosition=new Vector2(14+i*185,-1495);
                v.NavButtons[i]=nav.GetComponent<Button>();v.NavButtons[i].onClick=new Button.ButtonClickedEvent();AddPressFeedback(v.NavButtons[i]);
                v.NavLabels[i]=nav.Find("Label").GetComponent<Text>();
            }
            BuildInfo(v);var layout=root.gameObject.AddComponent<ProductionInventoryLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Inventory bindings incomplete.");
        }
        static void BuildInfo(ProductionInventoryView v){v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.9f);shield.raycastTarget=true;Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),77,469,787,709);v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",120,505,701,99,37);Fit(v.InfoHeading,25,37);v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",132,638,677,340,30);Fit(v.InfoDescription,23,30);v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1040,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);}
        public static Sprite TabArt(int i,bool active,bool en)=>Load("tab_"+i+"_"+(active?"active":"normal")+"_"+(en?"en":"ru")+"_r3");
        public static Sprite ItemArt(int id)=>id>=20?ProductionBoosterArt.Icon(id-20,true):Load("item_"+id);
        public static void RebuildContent(ProductionInventoryView v){
            for(int i=v.DynamicRoot.childCount-1;i>=0;i--){var go=v.DynamicRoot.GetChild(i).gameObject;go.SetActive(false);go.transform.SetParent(null);if(Application.isPlaying)UnityEngine.Object.Destroy(go);else UnityEngine.Object.DestroyImmediate(go);}
            bool boosters=v.SelectedTab==InventoryTab.Boosters;var ids=ProductionInventoryView.VisibleIds(v.SelectedTab);
            for(int i=0;i<ids.Length;i++){int id=ids[i];float x=boosters?91+i%2*392:39+i%5*174,y=boosters?405+i/2*323:405+i/5*170;float w=boosters?368:173,h=boosters?303:167;
                var b=Button(boosters?Panel(v.DynamicRoot,"Item_"+id,Shared("reward_panel"),x,y,w,h):Art(v.DynamicRoot,"Item_"+id,ItemArt(id),x,y,w,h));b.onClick.AddListener(()=>v.SelectItem(id));
                if(boosters){Art(b.transform,"Artwork",ItemArt(id),97,14,174,184);var title=Label(v,b.transform,"Name",23,198,322,58,26);title.text=ProductionInventoryView.ItemTitle(id);Fit(title,20,26);}
                if(boosters)Panel(b.transform,"StockBacking",Shared("tab"),70,251,228,42);
                var count=Label(v,b.transform,"Count",boosters?80:95,boosters?256:125,boosters?208:62,boosters?32:32,boosters?24:24);count.font=v.NumberFont;count.text=boosters?ProductionInventoryView.Number(v.BoosterQuantity(id-20)):"—";Fit(count,14,24);
                if(id==v.SelectedItem){
                    var perimeter=Rect(b.transform,"SelectedHighlight",0,0,w*2,h*2);perimeter.localScale=new Vector3(.5f,.5f,1);
                    perimeter.gameObject.AddComponent<WorldMapPanelBorderHighlight>().raycastTarget=false;
                }
            }
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="paper"||s.name=="grid_frame_r2"||s.name=="quantity_frame_r2")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="paper"||s.name=="grid_frame_r2"||s.name=="quantity_frame_r2")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};AddPressFeedback(b);return b;}
        static void AddPressFeedback(Button b){b.transition=Selectable.Transition.None;if(b.targetGraphic)b.targetGraphic.CrossFadeColor(Color.white,0,true,true);if(!b.GetComponent<ProductionMapButtonFeedback>())b.gameObject.AddComponent<ProductionMapButtonFeedback>();}
        static Text Label(ProductionInventoryView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
