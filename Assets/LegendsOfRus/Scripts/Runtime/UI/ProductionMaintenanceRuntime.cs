using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionMaintenanceRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/MaintenanceAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionMaintenanceView>();if(old&&old.HasRequiredBindings){old.Refresh();return;}
            if(old){if(Application.isPlaying)throw new InvalidOperationException("Rebuild Maintenance outside Play Mode.");if(old.Content)UnityEngine.Object.DestroyImmediate(old.Content.gameObject);var stale=root.GetComponent<ProductionMaintenanceLayout>();if(stale)UnityEngine.Object.DestroyImmediate(stale);}
            var v=old?old:root.gameObject.AddComponent<ProductionMaintenanceView>();v.Revision=ProductionMaintenanceView.CurrentRevision;
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"MAINTENANCE_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>(ProductionLoadingRuntime.EnvironmentResource),0,0,941,1672);v.Environment.color=new Color(.45f,.45f,.53f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            v.Logo=Art(v.Body,"ApprovedLogo",Resources.Load<Sprite>(ProductionLoadingRuntime.LogoResource),300,42,341,251);
            Art(v.Body,"SourcePanel",Load("panel"),60,311,820,1212);
            v.Heading=Label(v,v.Body,"Heading",178,408,586,163,59,true);var shadow=v.Heading.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(0,0,0,.9f);shadow.effectDistance=new Vector2(2,-3);
            v.Description=Paper(v,"Description",155,662,631,134,31);
            Art(v.Body,"CompletionPlaque",Load("timer"),166,854,608,192);
            v.EstimateHeading=Label(v,v.Body,"EstimateHeading",224,877,493,40,27);
            v.EstimateValue=Label(v,v.Body,"EstimateValue",226,938,488,76,40);
            v.CompensationHeading=Paper(v,"CompensationHeading",167,1073,324,45,29);
            v.CompensationDescription=Paper(v,"CompensationDescription",157,1126,280,126,27);
            v.Chest=Art(v.Body,"GiftPreview",Load("chest"),449,1046,333,238);
            v.PreviewLabel=Paper(v,"PreviewLabel",448,1280,336,22,18);
            var face=Art(v.Body,"Continue",Load("button"),206,1302,532,134);face.raycastTarget=true;v.Continue=face.gameObject.AddComponent<Button>();v.Continue.targetGraphic=face;v.Continue.navigation=new Navigation{mode=Navigation.Mode.None};v.ContinueLabel=Label(v,v.Continue.transform,"Label",76,31,380,73,46,true);
            var layout=root.gameObject.AddComponent<ProductionMaintenanceLayout>();layout.View=v;v.Refresh();layout.ApplyNow();
        }
        static Text Paper(ProductionMaintenanceView v,string n,float x,float y,float w,float h,int size){var t=Label(v,v.Body,n,x,y,w,h,size);t.color=new Color32(48,30,12,255);return t;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Maintenance art "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Text Label(ProductionMaintenanceView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.resizeTextForBestFit=true;t.resizeTextMinSize=size-5;t.resizeTextMaxSize=size;return t;}
    }
}
