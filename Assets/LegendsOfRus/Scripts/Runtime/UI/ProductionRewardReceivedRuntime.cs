using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public static class ProductionRewardReceivedRuntime
    {
        public const string ArtRoot = "ProductionUI/RewardReceivedAligned/";
        public static Sprite Load(string name) => Resources.Load<Sprite>(ArtRoot + name);
        public static void BuildInto(RectTransform root)
        {
            var existing = root.GetComponent<ProductionRewardReceivedView>();
            if (existing != null)
            {
                if (Application.isPlaying && !existing.HasRequiredBindings) throw new InvalidOperationException("Repair Reward Received bindings outside Play Mode.");
                RefreshArt(existing);existing.RefreshContent();return;
            }
            var v = root.gameObject.AddComponent<ProductionRewardReceivedView>();
            v.Typography = Resources.Load<SplashTitleTypography>(ProductionHomeRuntime.TypographyResource);
            v.Content = Rect(root,"REWARD_RECEIVED_CONTENT",0,0,941,1672);
            v.Shade = Art(v.Content,"DimLivePage",null,0,0,941,1672);v.Shade.color = new Color(0,0,0,.66f);v.Shade.raycastTarget = true;
            v.Body = Rect(v.Content,"RewardPanel",72,420,794,980);
            v.Panel = Art(v.Body,"GoldLacquerFrame",Load("panel"),0,0,794,980);
            v.Title = Label(v,v.Body,"Heading",85,104,624,74,48);
            v.Chest = Art(v.Body,"GlowingChest",Load("chest"),76,165,623,313);
            v.Message = Label(v,v.Body,"Message",80,478,634,74,30);
            v.Cards = new Image[7];v.Icons = new Image[7];v.Amounts = new Text[7];v.Names = new Text[7];
            for (int i=0;i<7;i++)
            {
                v.Cards[i] = Art(v.Body,"Reward_"+i,Resources.Load<Sprite>("ProductionUI/ChapterCompleteAligned/reward_card"),75+i%4*160,563+i/4*155,144,146);
                v.Icons[i] = Art(v.Cards[i].transform,"Icon",null,32,10,80,80);
                v.Amounts[i] = Label(v,v.Cards[i].transform,"Amount",10,88,124,31,29);
                v.Names[i] = Label(v,v.Cards[i].transform,"Name",5,116,134,24,16);
            }
            var buttonImage = Art(v.Body,"Confirm",Resources.Load<Sprite>("ProductionUI/ShopAligned/buy"),171,850,452,104.2f);
            buttonImage.raycastTarget = true;v.Confirm = buttonImage.gameObject.AddComponent<Button>();v.Confirm.targetGraphic = buttonImage;
            v.Confirm.navigation = new Navigation { mode=Navigation.Mode.None };
            v.ConfirmText = Label(v,v.Confirm.transform,"Label",52,21,348,61,43);
            var layout = root.gameObject.AddComponent<ProductionRewardReceivedLayout>();layout.View=v;
            RefreshArt(v);v.RefreshContent();layout.ApplyNow();
            if (!v.HasRequiredBindings) throw new InvalidOperationException("Reward Received bindings incomplete.");
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;}
        static void RefreshArt(ProductionRewardReceivedView v)
        {
            v.Revision=ProductionRewardReceivedView.CurrentRevision;
            v.Body.sizeDelta=new Vector2(795,982);
            v.Panel.sprite=Load("panel_r2");Place(v.Panel.rectTransform,0,0,795,982);
            if(v.HeadingLettering==null)v.HeadingLettering=Art(v.Body,"OriginalHeading",Load("title_ru_r2"),139,122,529,56);
            Place(v.Title.rectTransform,77,120,641,65);v.Title.fontSize=45;
            if(v.ChestGlow==null)v.ChestGlow=Art(v.Body,"SourceGlow",Load("chest_glow_r2"),53,172,694,320);
            if(v.ChestGlow.transform.GetSiblingIndex()>v.Chest.transform.GetSiblingIndex())v.ChestGlow.transform.SetSiblingIndex(v.Chest.transform.GetSiblingIndex());
            v.Chest.sprite=Load("chest_r2");Place(v.Chest.rectTransform,95,176,601,314);
            Place(v.Message.rectTransform,80,485,635,70);v.Message.fontSize=28;
            for(int i=0;i<7;i++)
            {
                v.Cards[i].sprite=Load("card_r2");v.Cards[i].rectTransform.sizeDelta=new Vector2(122,130);
                Place(v.Icons[i].rectTransform,23,8,76,66);
                Place(v.Amounts[i].rectTransform,6,75,110,27);v.Amounts[i].fontSize=27;
                Place(v.Names[i].rectTransform,3,103,116,18);v.Names[i].fontSize=14;
            }
            v.Confirm.GetComponent<Image>().sprite=Load("button_ru_r2");Place((RectTransform)v.Confirm.transform,175,845,451,138);
            Place(v.ConfirmText.rectTransform,54,29,343,65);v.ConfirmText.fontSize=41;
        }
        public static void ApplySourceState(ProductionRewardReceivedView v,bool en)
        {
            v.HeadingLettering.gameObject.SetActive(v.HasReceipt&&!en);v.Title.enabled=!v.HasReceipt||en;
            v.ChestGlow.gameObject.SetActive(v.HasReceipt);
            v.Confirm.GetComponent<Image>().sprite=Load(v.HasReceipt&&!en?"button_ru_r2":"button_blank_r2");
            v.ConfirmText.enabled=!v.HasReceipt||en;
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        static Image Art(Transform parent,string name,Sprite sprite,float x,float y,float w,float h)
        {var image=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return image;}
        static Text Label(ProductionRewardReceivedView v,Transform parent,string name,float x,float y,float w,float h,int size)
        {
            var text=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Text>();text.font=v.Typography.RegularFont;text.fontSize=size;
            text.color=new Color32(246,217,161,255);text.alignment=TextAnchor.MiddleCenter;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;text.raycastTarget=false;return text;
        }
    }
}
