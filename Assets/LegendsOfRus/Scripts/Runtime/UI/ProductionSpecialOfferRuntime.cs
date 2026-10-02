using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionSpecialOfferRuntime
    {
        public static Sprite Load(string name)=>Resources.Load<Sprite>("ProductionUI/ShopAligned/"+name);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionSpecialOfferView>();
            if(existing!=null){if(Application.isPlaying && !existing.HasRequiredBindings)throw new InvalidOperationException("Repair Special Offer outside Play Mode.");RefreshArt(existing);existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionSpecialOfferView>();v.Typography=Resources.Load<SplashTitleTypography>("ProductionUI/SplashTitleConfig/SplashTitleTypography");
            v.Content=Rect(root,"SPECIAL_OFFER_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanWoodMaterial","environment",0,0,1672,1672);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"NativeHudBacking","hud",0,25,941,120);
            v.BackButton=Button(v.Body,"Back","tab",26,49,158,62.12f);v.BackText=Label(v,v.BackButton.transform,"Label",16,12,126,39,26);
            Text[] balances=new Text[3];string[] icons={"coin_icon","gems_icon","energy_icon"};
            for(int i=0;i<3;i++)
            {
                float x=258+i*224;var cap=Art(v.Body,"ResourceCapsule_"+i,null,x,54,203,66);cap.sprite=Home("resource_capsule");
                var icon=Art(v.Body,"ResourceIcon_"+i,null,x-5,49,65,77);icon.sprite=Home(icons[i]);
                balances[i]=Label(v,v.Body,"ResourceValue_"+i,x+58,66,134,41,28);
                balances[i].resizeTextForBestFit=true;balances[i].resizeTextMinSize=17;balances[i].resizeTextMaxSize=28;
            }
            v.GoldText=balances[0];v.GemsText=balances[1];v.EnergyText=balances[2];
            v.Header=Art(v.Body,"OfferHeadingPlaque","title_blank",106,170,733,189);
            v.Title=Label(v,v.Body,"OfferHeading",228,261,493,73,34);
            v.OfferCard=Art(v.Body,"OfferCard","card",115,373,711,810);
            v.BundleName=Label(v,v.Body,"BundleName",165,405,611,65,39);
            v.Hero=Art(v.Body,"BogatyrChest","chest",194,483,553,392.63f);
            v.RewardIcons=new Image[3];v.RewardLabels=new Text[3];v.RewardAmounts=new Text[3];
            for(int i=0;i<3;i++)
            {
                var tile=Art(v.Body,"BundlePreviewSlot_"+i,null,215+i*179,891,153,159);tile.sprite=Resources.Load<Sprite>("ProductionUI/ChapterCompleteAligned/reward_card");
                v.RewardIcons[i]=Art(tile.transform,"PreviewIcon",i==2?"nav_offers":null,36,10,81,81);
                if(i<2)v.RewardIcons[i].sprite=Home(icons[i]);
                v.RewardLabels[i]=Label(v,tile.transform,"Category",5,91,143,34,23);
                v.RewardAmounts[i]=Label(v,tile.transform,"UnspecifiedAmount",5,122,143,28,24);
            }
            v.ContentsNote=Label(v,v.Body,"ContentsAvailability",174,1070,593,72,27);
            v.Price=Label(v,v.Body,"PriceAvailability",180,1191,581,42,29);
            v.PurchaseButton=Button(v.Body,"Purchase","buy",220,1250,501,115.5f);
            var colors=v.PurchaseButton.colors;colors.disabledColor=new Color(.83f,.83f,.83f,1);v.PurchaseButton.colors=colors;
            v.PurchaseText=Label(v,v.PurchaseButton.transform,"Label",45,24,411,63,38);
            v.Status=Label(v,v.Body,"Availability",160,1380,621,82,28);
            v.ShopButton=Button(v.Body,"Shop","tab",307,1491,327,128.56f);v.ShopText=Label(v,v.ShopButton.transform,"Label",30,31,267,65,34);
            Art(v.Body,"FooterOrnament","footer_ornament",3,1627,935,44);
            RefreshArt(v);
            var layout=root.gameObject.AddComponent<ProductionSpecialOfferLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Special Offer bindings incomplete.");
        }
        static Sprite Source(string key)=>Resources.Load<Sprite>("ProductionUI/SpecialOfferAligned/"+key);
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;}
        static void RefreshArt(ProductionSpecialOfferView v)
        {
            v.Revision=ProductionSpecialOfferView.CurrentRevision;
            v.Environment.sprite=ProductionSettingsRuntime.Shared("background");v.Environment.color=new Color(.32f,.32f,.36f,1);
            v.OfferCard.sprite=Source("panel_body_r3");Place(v.OfferCard.rectTransform,32,104,885,1345);
            v.OfferCard.transform.SetAsFirstSibling();
            v.Header.gameObject.SetActive(true);v.Header.sprite=Source("header_ru_r3");Place(v.Header.rectTransform,32,104,885,278);
            v.Header.transform.SetSiblingIndex(v.OfferCard.transform.GetSiblingIndex()+1);
            ProductionPlayerHeader.Attach(v);
            v.Hero.sprite=Source("hero_source_r2");Place(v.Hero.rectTransform,71,485,779,446);
            var glow=v.Body.Find("SourceTreasureGlow")?.GetComponent<Image>();
            if(glow==null)glow=Art(v.Body,"SourceTreasureGlow",null,71,485,779,446);
            glow.sprite=Source("hero_glow_r2");
            if(glow.transform.GetSiblingIndex()>v.Hero.transform.GetSiblingIndex())glow.transform.SetSiblingIndex(v.Hero.transform.GetSiblingIndex());
            Place(v.Title.rectTransform,148,195,660,158);v.Title.fontSize=62;
            Place(v.BundleName.rectTransform,255,393,425,65);v.BundleName.fontSize=34;
            for(int i=0;i<3;i++)
            {
                var slot=(RectTransform)v.RewardIcons[i].transform.parent;Place(slot,240+i*168,1005,126,187);
                slot.GetComponent<Image>().sprite=Source("reward_frame_r2");
                Place(v.RewardIcons[i].rectTransform,13,12,99,99);
                Place(v.RewardLabels[i].rectTransform,4,147,118,32);v.RewardLabels[i].fontSize=20;
                Place(v.RewardAmounts[i].rectTransform,4,113,118,31);
            }
            v.RewardIcons[2].sprite=v.Hero.sprite;
            Place(v.ContentsNote.rectTransform,172,936,597,52);v.ContentsNote.fontSize=25;
            Place(v.Price.rectTransform,180,1206,581,44);
            var buy=v.PurchaseButton.GetComponent<Image>();buy.sprite=Resources.Load<Sprite>("ProductionUI/TutorialAligned/next");Place(buy.rectTransform,173,1263,597,125);
            Place(v.PurchaseText.rectTransform,60,26,477,71);v.PurchaseText.fontSize=40;
            Place(v.Status.rectTransform,180,1390,581,39);v.Status.fontSize=23;
            var close=v.BackButton.GetComponent<Image>();close.sprite=Resources.Load<Sprite>("ProductionUI/TutorialAligned/close");Place(close.rectTransform,799,160,119,123);v.BackText.gameObject.SetActive(false);v.BackButton.transform.SetAsLastSibling();
            var shop=v.ShopButton.GetComponent<Image>();shop.sprite=Resources.Load<Sprite>("ProductionUI/ShopAligned/nav_label_blank_r2");Place(shop.rectTransform,307,1500,327,80);
            Place(v.ShopText.rectTransform,20,18,287,44);v.ShopText.fontSize=29;
        }
        public static void ApplyLocalizedArt(ProductionSpecialOfferView v,bool english)
        {
            v.OfferCard.sprite=Source("panel_body_r3");
            v.Header.gameObject.SetActive(true);v.Header.sprite=Source(english?"header_blank_r3":"header_ru_r3");
            v.Title.gameObject.SetActive(true);v.Title.enabled=english;
            v.PurchaseButton.GetComponent<Image>().sprite=english?Resources.Load<Sprite>("ProductionUI/TutorialAligned/next"):Source("buy_lettered_r3");
            v.PurchaseText.enabled=english;
            v.ShopButton.gameObject.SetActive(false);
            v.Status.text=english?"This offer is not available yet.":"Предложение пока недоступно.";
        }
        static Sprite Home(string n)=>Resources.Load<Sprite>("ProductionUI/HomeAligned/"+n);
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,string sprite,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=sprite==null?null:Load(sprite);a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Transform p,string n,string sprite,float x,float y,float w,float h){var a=Art(p,n,sprite,x,y,w,h);a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionSpecialOfferView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,214,155,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
