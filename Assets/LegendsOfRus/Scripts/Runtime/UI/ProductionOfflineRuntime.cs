using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionOfflineRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/OfflineAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionOfflineView>();if(old&&old.HasRequiredBindings){old.Refresh();return;}
            if(old){if(Application.isPlaying)throw new InvalidOperationException("Rebuild Offline outside Play Mode.");if(old.Content)UnityEngine.Object.DestroyImmediate(old.Content.gameObject);var stale=root.GetComponent<ProductionOfflineLayout>();if(stale)UnityEngine.Object.DestroyImmediate(stale);}
            var v=old?old:root.gameObject.AddComponent<ProductionOfflineView>();v.Revision=ProductionOfflineView.CurrentRevision;
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"OFFLINE_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);v.Environment.color=new Color(.72f,.72f,.76f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            BuildHeader(v);
            v.Panel=Art(v.Body,"SourcePanel",Load("panel"),120,185,700,1350);
            v.Heading=Label(v,v.Body,"Heading",192,352,560,81,56,true);
            var shade=v.Heading.gameObject.AddComponent<Shadow>();shade.effectColor=new Color(0,0,0,.8f);shade.effectDistance=new Vector2(2,-3);
            v.Emblem=Art(v.Body,"CrossedPlug",Load("emblem"),233,475,477,425);
            v.Description=Label(v,v.Body,"NetworkDescription",218,914,505,100,30);
            v.LocalDescription=Label(v,v.Body,"LocalPlayDescription",207,1017,527,26,20);
            v.Retry=Button(Art(v.Body,"Retry",Load("retry"),239,1054,467,142));v.RetryLabel=Label(v,v.Retry.transform,"Label",68,43,331,65,40,true);
            // Separate adjacent hit rectangles without changing the source button artwork.
            v.Retry.image.raycastPadding=new Vector4(0,4,0,0);
            v.Back=Button(Art(v.Body,"Back",Load("home"),267,1193,408,107));v.BackLabel=Label(v,v.Back.transform,"Label",36,26,336,63,33,true);
            v.CheckNote=Label(v,v.Body,"CheckNote",207,1300,527,27,20);
            Art(v.Body,"SupportIcon",Resources.Load<Sprite>("ProductionUI/SettingsAligned/icon_support"),244,1327,36,36);
            v.SupportLabel=Label(v,v.Body,"SupportAvailability",289,1330,404,39,23);
            v.OnlineDescription=Label(v,v.Body,"ServerStatus",249,1378,448,37,22);
            var layout=root.gameObject.AddComponent<ProductionOfflineLayout>();layout.View=v;v.Refresh();layout.ApplyNow();
        }
        static void BuildHeader(ProductionOfflineView v)
        {
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Offline requires approved navigation header.");
            if(!shop.Header||!shop.Header.HasBindings)throw new InvalidOperationException("Offline requires the shared player header.");
            v.Header=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);v.Header.name="NativeHud";v.Hud=(RectTransform)v.Header.transform;
            foreach(var button in v.Header.GetComponentsInChildren<Button>(true))AddPressFeedback(button);
            foreach(var button in v.Header.Plus)button.image.raycastPadding=new Vector4(-16,-14.5f,-16,-14.5f);
            // Expand energy leftward to keep clear of the adjacent Settings action.
            v.Header.Plus[2].image.raycastPadding=new Vector4(-32,-14.5f,0,-14.5f);
            v.ProfileButton=v.Header.Profile;v.SettingsButton=v.Header.Settings;v.HeroName=v.Header.PlayerName;v.LevelText=v.Header.Level;v.PlusButtons=v.Header.Plus;
            v.GoldText=v.Header.Balances[0];v.GemsText=v.Header.Balances[1];v.EnergyText=v.Header.Balances[2];
        }
        static void AddPressFeedback(Button b){b.transition=Selectable.Transition.None;if(!b.GetComponent<ProductionMapButtonFeedback>())b.gameObject.AddComponent<ProductionMapButtonFeedback>();}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Offline art "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};AddPressFeedback(b);return b;}
        static Text Label(ProductionOfflineView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.resizeTextForBestFit=true;t.resizeTextMinSize=size-5;t.resizeTextMaxSize=size;return t;}
    }
}
