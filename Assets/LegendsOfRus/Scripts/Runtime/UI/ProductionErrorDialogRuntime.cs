using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionErrorDialogRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/ErrorDialogAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionErrorDialogView>();if(old&&old.HasRequiredBindings){old.Refresh();return;}
            if(old){if(Application.isPlaying)throw new InvalidOperationException("Rebuild Error Dialog outside Play Mode.");if(old.Content)UnityEngine.Object.DestroyImmediate(old.Content.gameObject);var stale=root.GetComponent<ProductionErrorDialogLayout>();if(stale)UnityEngine.Object.DestroyImmediate(stale);}
            var v=old?old:root.gameObject.AddComponent<ProductionErrorDialogView>();v.Revision=ProductionErrorDialogView.CurrentRevision;
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"ERROR_DIALOG_CONTENT",0,0,941,1672);
            v.Shade=Rect(v.Content,"ModalShade",0,0,941,1672).gameObject.AddComponent<Image>();v.Shade.color=new Color(0,0,0,.68f);v.Shade.raycastTarget=true;
            v.Body=Rect(v.Content,"Dialog",124,407,696,859);
            v.Panel=Art(v.Body,"OriginalPanel",Load("panel"),0,0,696,859);
            v.Heading=Label(v,v.Body,"Heading",75,87,547,63,43,true);var sh=v.Heading.gameObject.AddComponent<Shadow>();sh.effectColor=new Color(0,0,0,.8f);sh.effectDistance=new Vector2(2,-3);
            v.Emblem=Art(v.Body,"WarningEmblem",Load("emblem"),218,183,260,233);
            v.Description=Label(v,v.Body,"Description",94,416,508,78,29);v.Description.color=new Color32(45,29,13,255);
            v.Retry=Button(Art(v.Body,"Retry",Load("retry"),101,515,493,106));v.RetryLabel=Label(v,v.Retry.transform,"Label",83,21,327,64,40,true);
            v.Close=Button(Art(v.Body,"Close",Load("close"),135,635,423,91));v.CloseLabel=Label(v,v.Close.transform,"Label",61,16,301,60,35,true);
            v.Support=Button(Art(v.Body,"Support",Load("support"),152,735,395,70));v.SupportLabel=Label(v,v.Support.transform,"Label",27,14,341,43,23);
            var layout=root.gameObject.AddComponent<ProductionErrorDialogLayout>();layout.View=v;v.Refresh();layout.ApplyNow();
        }
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Error Dialog art "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};b.transition=Selectable.Transition.None;b.gameObject.AddComponent<ProductionMapButtonFeedback>();return b;}
        static Text Label(ProductionErrorDialogView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.resizeTextForBestFit=true;t.resizeTextMinSize=size-6;t.resizeTextMaxSize=size;return t;}
    }
}
