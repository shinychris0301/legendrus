using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionDialogueRuntime
    {
        public static Sprite Load(string key) => Resources.Load<Sprite>("ProductionUI/DialogueAligned/" + key);
        public static void BuildInto(RectTransform root)
        {
            var previous = root.GetComponent<ProductionDialogueView>();
            if (previous && previous.HasRequiredBindings) { previous.Refresh(); return; }
            if (previous)
            {
                if (Application.isPlaying) throw new InvalidOperationException("Rebuild Dialogue outside Play Mode.");
                if (previous.Content) UnityEngine.Object.DestroyImmediate(previous.Content.gameObject);
                var stale = root.GetComponent<ProductionDialogueLayout>();
                if (stale) UnityEngine.Object.DestroyImmediate(stale);
            }
            var v = previous ? previous : root.gameObject.AddComponent<ProductionDialogueView>();
            v.Revision = ProductionDialogueView.CurrentRevision;
            v.BodyFont = Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            v.TitleFont = Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Hall = Load("hall");
            v.Forest = Resources.Load<Sprite>(ProductionLoadingRuntime.EnvironmentResource);
            v.Content = Rect(root,"DIALOGUE_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment = Art(v.Content,"CleanEnvironment",v.Hall,0,0,941,1672);
            v.Body = Rect(v.Content,"Composition",0,0,941,1672);
            v.Ivan = Art(v.Body,"Ivan",Load("ivan"),0,254,490,870);
            v.Dobrynya = Art(v.Body,"Dobrynya",Load("dobrynya"),474,261,467,863);
            v.Firebird = Art(v.Body,"Firebird",Resources.Load<Sprite>("ProductionUI/LoadingAligned/firebird_journey"),211,300,650,760);
            Art(v.Body,"SourceDialoguePanel",Load("panel"),45,1022,849,618);
            v.Speaker = Label(v,v.Body,"Speaker",289,1086,365,71,49,true);
            var shadow=v.Speaker.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(0,0,0,.9f);shadow.effectDistance=new Vector2(1,-2);
            v.Description = Label(v,v.Body,"Dialogue",124,1198,692,208,38);
            v.Description.color = new Color32(48,28,13,255);
            v.Description.alignment = TextAnchor.UpperLeft;
            v.Description.lineSpacing = 1.08f;
            // A native divider keeps the paper face clean and avoids copying reference words.
            var divider = Rect(v.Body,"Divider",213,1423,515,2).gameObject.AddComponent<Image>();
            divider.color = new Color32(123,74,25,255);divider.raycastTarget=false;
            var diamond=Rect(v.Body,"DividerDiamond",463,1416,15,15).gameObject.AddComponent<Image>();
            diamond.rectTransform.localEulerAngles=new Vector3(0,0,45);diamond.color=divider.color;diamond.raycastTarget=false;
            var face=Art(v.Body,"Continue",Load("continue"),195,1454,552,138);
            face.raycastTarget=true;v.Continue=face.gameObject.AddComponent<Button>();v.Continue.targetGraphic=face;
            v.Continue.navigation=new Navigation{mode=Navigation.Mode.None};
            v.ContinueLabel=Label(v,face.transform,"Label",65,34,424,73,44,true);
            var layout=root.gameObject.AddComponent<ProductionDialogueLayout>();layout.View=v;v.Refresh();layout.ApplyNow();
        }
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)
        {
            var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);
            var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);
            r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h)
        {
            if(!s)throw new InvalidOperationException("Missing Dialogue art "+n);
            var image=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=s;image.preserveAspect=true;image.raycastTarget=false;return image;
        }
        static Text Label(ProductionDialogueView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false)
        {
            var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;
            t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;
            t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            t.resizeTextForBestFit=true;t.resizeTextMinSize=size-8;t.resizeTextMaxSize=size;return t;
        }
    }
}
