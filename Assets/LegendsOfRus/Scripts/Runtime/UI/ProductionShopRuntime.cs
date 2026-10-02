using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionShopRuntime
    {
        public const string ArtRoot="ProductionUI/ShopAligned/";
        public static Sprite Load(string name)=>Resources.Load<Sprite>(ArtRoot+name);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionShopView>();if(existing!=null){if(Application.isPlaying && !existing.HasRequiredBindings)throw new InvalidOperationException("Repair Shop outside Play Mode.");Decorate(existing);Refine(existing);existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionShopView>();v.Typography=Resources.Load<SplashTitleTypography>("ProductionUI/SplashTitleConfig/SplashTitleTypography");
            v.Content=Rect(root,"SHOP_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanWoodMaterial","environment",0,0,1672,1672);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"NativeHudBacking","hud",0,0,941,120);
            v.ProfileButton=Button(v.Body,"Profile",null,6,9,146,190);v.ProfileButton.GetComponent<Image>().sprite=Home("avatar_portrait");
            var ring=Art(v.ProfileButton.transform,"PortraitRing",null,0,0,146,190);ring.sprite=Home("avatar_ring");
            v.HeroName=Text(v,v.Body,"HeroName",163,24,160,38,32);v.HeroName.alignment=TextAnchor.MiddleLeft;
            v.LevelText=Text(v,v.Body,"Level",163,63,166,32,25);v.LevelText.alignment=TextAnchor.MiddleLeft;
            v.PlusButtons=new Button[3];Text[] values=new Text[3];string[] icons={"coin_icon","gems_icon","energy_icon"};
            for(int i=0;i<3;i++)
            {
                float x=337+i*176;var cap=Art(v.Body,"ResourceCapsule_"+i,null,x,38,172,56);cap.sprite=Home("resource_capsule");
                var icon=Art(v.Body,"ResourceIcon_"+i,null,x-5,32,53,65);icon.sprite=Home(icons[i]);
                values[i]=Text(v,v.Body,"ResourceValue_"+i,x+44,43,81,38,25);
                v.PlusButtons[i]=Button(v.Body,"ResourcePlus_"+i,null,x+126,42,42,42);v.PlusButtons[i].GetComponent<Image>().sprite=Home("resource_plus");
            }
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];
            v.SettingsButton=Button(v.Body,"Settings",null,868,29,68,73);v.SettingsButton.GetComponent<Image>().sprite=Home("settings_medallion");
            v.Title=Art(v.Body,"ShopPlaque","title_ru",106,94,733,189);v.TitleText=Text(v,v.Body,"ShopHeading",245,185,453,73,57);
            v.Tabs=new Button[4];v.TabLabels=new Text[4];
            for(int i=0;i<4;i++){v.Tabs[i]=Button(v.Body,"Category_"+i,"tab",14+i*231,284,225,89);v.TabLabels[i]=Text(v,v.Tabs[i].transform,"Label",13,17,199,57,28);}
            v.Cards=new RectTransform[4];v.ProductNames=new Text[4];v.ProductDetails=new Text[4];v.PurchaseButtons=new Button[4];v.PurchaseLabels=new Text[4];
            string[] art={"bag","chest","cart","magic"};
            for(int i=0;i<4;i++)
            {
                var card=Art(v.Body,"Product_"+i,"card",39+(i%2)*441,393+(i/2)*502,431,491);v.Cards[i]=card.rectTransform;
                v.ProductNames[i]=Text(v,card.transform,"ProductName",28,18,375,51,33);
                Art(card.transform,"ProductArt",art[i],29,70,374,258);
                var detail=Art(card.transform,"ProductDetailPlaque","quantity",96,326,239,67);v.ProductDetails[i]=Text(v,detail.transform,"Availability",13,7,213,51,24);
                v.PurchaseButtons[i]=Button(card.transform,"Purchase","buy",45,397,334,77);
                var colors=v.PurchaseButtons[i].colors;colors.disabledColor=new Color(.83f,.83f,.83f,1);v.PurchaseButtons[i].colors=colors;
                v.PurchaseLabels[i]=Text(v,v.PurchaseButtons[i].transform,"Label",35,15,264,48,29);
            }
            v.EmptyPanel=Rect(v.Body,"EmptyCategory",115,500,711,400).gameObject;
            var blank=Art(v.EmptyPanel.transform,"Panel","empty_panel",0,0,711,400);blank.preserveAspect=true;
            v.EmptyText=Text(v,v.EmptyPanel.transform,"Message",48,112,615,176,32);
            v.SupportText=Text(v,v.Body,"StoreAvailability",220,1400,501,47,26);
            v.BackButton=Button(v.Body,"Back","tab",15,1394,188,66);v.BackText=Text(v,v.BackButton.transform,"Label",12,12,164,42,25);
            v.SpecialButton=Button(v.Body,"SpecialOffer","tab",724,1394,203,66);v.SpecialText=Text(v,v.SpecialButton.transform,"Label",15,9,173,48,20);
            v.NavButtons=new Button[5];v.NavLabels=new Text[5];string[] nav={"offers","gems","bundles","boosters","vip"};
            for(int i=0;i<5;i++)
            {
                var b=Button(v.Body,"Navigation_"+i,"nav_"+nav[i],14+i*185,1465,173,161);v.NavButtons[i]=b;
                Art(b.transform,"LabelPlaque","quantity",0,125,173,48);v.NavLabels[i]=Text(v,b.transform,"Label",5,132,163,32,22);
            }
            Decorate(v);Refine(v);
            var layout=root.gameObject.AddComponent<ProductionShopLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Shop bindings incomplete.");
        }
        static void Decorate(ProductionShopView v)
        {
            var profile=v.ProfileButton;profile.GetComponent<Image>().sprite=Home("avatar_ring");
            if(profile.transform.Find("PortraitClip")==null)
            {
                var clip=Rect(profile.transform,"PortraitClip",22,19,103,103);var face=clip.gameObject.AddComponent<ProductionHomePanel>();face.Frame=false;face.Radius=51.5f;face.LeftColor=face.RightColor=new Color32(7,24,42,255);face.raycastTarget=false;
                var mask=clip.gameObject.AddComponent<Mask>();mask.showMaskGraphic=true;
                var portrait=Art(clip,"Ivan",null,0,0,103,103);portrait.sprite=Home("avatar_portrait");clip.SetAsFirstSibling();
            }
            var empty=v.EmptyPanel.transform.Find("Panel").GetComponent<Image>();empty.sprite=Load("empty_panel");empty.preserveAspect=true;
            if(v.Body.Find("LeftRail")!=null)return;
            Art(v.Body,"LeftRail","rail_left",5,388,21,1012);
            Art(v.Body,"RightRail","rail_right",917,388,21,1012);
            Art(v.Body,"TopRail","rail_top",7,373,927,20);
            Art(v.Body,"LowerRail","rail_bottom",4,1380,932,17);
            Art(v.Body,"FooterOrnament","footer_ornament",3,1627,935,44);
        }
        static void Refine(ProductionShopView v)
        {
            ProductionPlayerHeader.Attach(v);
            v.Revision=ProductionShopView.CurrentRevision;
            if(v.Body.Find("OriginalWoodArch")==null)
            {
                var arch=Art(v.Body,"OriginalWoodArch","arch_r2",0,120,941,164);
                arch.transform.SetAsFirstSibling();
            }
            for(int i=0;i<4;i++)
            {
                v.Cards[i].GetComponent<Image>().sprite=Load("card_r2");
                if(v.Cards[i].Find("OriginalProductName")==null)
                    Art(v.Cards[i],"OriginalProductName","name_"+i+"_ru_r2",28,20,375,42);
                NativeLettering(v.Cards[i].Find("OriginalProductName").GetComponent<Image>(),431,23);
                var product=v.Cards[i].Find("ProductArt").GetComponent<Image>();
                // Keep original source silhouette proportions and align the subjects on a common lower edge.
                product.rectTransform.anchoredPosition=new Vector2(29,-70);product.rectTransform.sizeDelta=new Vector2(374,258);
                if(v.Tabs[i].transform.Find("OriginalCaption")==null)
                    Art(v.Tabs[i].transform,"OriginalCaption","tab_"+i+"_ru_r2",13,17,199,57);
                NativeLettering(v.Tabs[i].transform.Find("OriginalCaption").GetComponent<Image>(),225,29);
            }
            string[] keys={"offers","gems","bundles","boosters","vip"};
            for(int i=0;i<5;i++)
            {
                var button=v.NavButtons[i];button.GetComponent<Image>().color=Color.clear;
                var orb=button.transform.Find("OriginalMedallion")?.GetComponent<Image>();
                if(orb==null)orb=Art(button.transform,"OriginalMedallion","nav_"+keys[i]+"_r2",0,0,173,130);
                NativeLettering(orb,173,0);orb.transform.SetAsFirstSibling();orb.raycastTarget=true;button.targetGraphic=orb;
                var plaque=button.transform.Find("LabelPlaque").GetComponent<Image>();
                plaque.sprite=Load("nav_label_"+i+"_ru_r2");
                plaque.rectTransform.anchoredPosition=new Vector2(-2,-125);plaque.rectTransform.sizeDelta=new Vector2(177,48);
            }
            var back=v.BackButton.GetComponent<Image>();back.sprite=Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2");
            back.rectTransform.anchoredPosition=new Vector2(22,-1388);back.rectTransform.sizeDelta=new Vector2(74,74);
            v.BackText.gameObject.SetActive(false);
        }
        static void NativeLettering(Image image,float width,float y)
        {
            image.rectTransform.sizeDelta=image.sprite.rect.size;
            image.rectTransform.anchoredPosition=new Vector2((width-image.sprite.rect.width)*.5f,-y);
        }
        public static void ApplyLocalizedArtwork(ProductionShopView v,bool english)
        {
            for(int i=0;i<4;i++)
            {
                v.ProductNames[i].enabled=english;v.TabLabels[i].enabled=english;
                v.Cards[i].Find("OriginalProductName").gameObject.SetActive(!english);
                v.Tabs[i].transform.Find("OriginalCaption").gameObject.SetActive(!english);
            }
            for(int i=0;i<5;i++)
            {
                v.NavLabels[i].enabled=english;
                var plaque=v.NavButtons[i].transform.Find("LabelPlaque").GetComponent<Image>();
                plaque.sprite=Load(english?"nav_label_blank_r2":"nav_label_"+i+"_ru_r2");
                string[] keys={"offers","gems","bundles","boosters","vip"};
                plaque.color=v.SelectedCategory==keys[i]?Color.white:new Color(.82f,.82f,.82f,1);
            }
        }
        static Sprite Home(string n)=>Resources.Load<Sprite>("ProductionUI/HomeAligned/"+n);
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,string sprite,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=sprite==null?null:Load(sprite);a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Transform p,string n,string sprite,float x,float y,float w,float h){var a=Art(p,n,sprite,x,y,w,h);a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Text(ProductionShopView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,214,155,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
