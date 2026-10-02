using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public static class ProductionOutOfEnergyRuntime
    {
        public const string ArtRoot="ProductionUI/OutOfEnergyAligned/";
        public static Sprite Load(string key)=>Resources.Load<Sprite>(ArtRoot+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionOutOfEnergyView>();
            if(existing!=null)
            {
                if(Application.isPlaying&&!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Out of Energy bindings outside Play Mode.");
                RefreshArt(existing);existing.RefreshContent();return;
            }
            var v=root.gameObject.AddComponent<ProductionOutOfEnergyView>();
            v.Typography=Resources.Load<SplashTitleTypography>(ProductionHomeRuntime.TypographyResource);
            v.Content=Rect(root,"OUT_OF_ENERGY_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"StandaloneEnvironment",Resources.Load<Sprite>("ProductionUI/ChapterMapAligned/environment"),0,0,941,1672);
            v.Environment.color=ProductionHomeRuntime.EnvironmentTone;
            v.Shade=Art(v.Content,"DimLivePage",null,0,0,941,1672);v.Shade.color=new Color(0,0,0,.6f);v.Shade.raycastTarget=true;
            v.Body=Rect(v.Content,"EnergyPanel",72,274,796,1111);
            v.Panel=Art(v.Body,"NavyGoldFrame",Load("panel"),0,0,796,1111);
            v.Title=Label(v,v.Body,"Heading",105,119,590,76,50);
            v.Emblem=Art(v.Body,"EnergyMedallion",Load("medallion"),223,194,354,244);
            var badge=Rect(v.Body,"EnergyCountBadge",444,353,74,74).gameObject.AddComponent<ProductionPreLevelBadge>();badge.raycastTarget=false;
            v.Count=Label(v,badge.transform,"Count",1,0,72,73,43);
            v.Message=Label(v,v.Body,"Message",65,437,666,75,30);
            v.TimerHeading=Label(v,v.Body,"TimerHeading",190,530,430,46,31);
            Art(v.Body,"Hourglass",Load("hourglass"),295,576,55,61);
            v.Countdown=Label(v,v.Body,"Countdown",354,570,223,75,53);
            v.ActionTitles=new Text[3];v.ActionDetails=new Text[3];v.ActionLabels=new Text[3];v.Actions=new Button[3];
            string[] iconKeys={"energy","friends","ad"};
            for(int i=0;i<3;i++)
            {
                float y=660+i*137;
                var icon=i==0?Resources.Load<Sprite>("ProductionUI/NavigationAligned/energy"):Load(iconKeys[i]);
                Art(v.Body,"ActionIcon_"+i,icon,72,y+15,95,95);
                v.ActionTitles[i]=Label(v,v.Body,"ActionTitle_"+i,184,y+17,310,48,27);
                v.ActionDetails[i]=Label(v,v.Body,"ActionDetails_"+i,184,y+56,310,44,21);
                var face=Art(v.Body,"Action_"+i,Load(i==0?"green":"blue"),504,y+21,225,91);
                v.Actions[i]=Button(face);v.ActionLabels[i]=Label(v,face.transform,"Label",15,17,195,57,24);
            }
            v.Close=Button(Art(v.Body,"Close",Load("close"),709,57,79,84));
            var layout=root.gameObject.AddComponent<ProductionOutOfEnergyLayout>();layout.View=v;
            RefreshArt(v);v.RefreshContent();layout.ApplyNow();
            if(!v.HasRequiredBindings)throw new InvalidOperationException("Out of Energy native bindings incomplete.");
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;}
        static void RefreshArt(ProductionOutOfEnergyView v)
        {
            v.Revision=ProductionOutOfEnergyView.CurrentRevision;
            v.Panel.sprite=Load("panel_r2");
            v.Emblem.sprite=Load("medallion_r2");Place(v.Emblem.rectTransform,238,193,320,223);
            if(v.HeadingLettering==null)v.HeadingLettering=Art(v.Body,"OriginalHeading",Load("title_ru_r2"),231,131,338,54);
            Place(v.Title.rectTransform,85,127,626,63);v.Title.fontSize=43;
            var badge=(RectTransform)v.Count.transform.parent;
            badge.GetComponent<ProductionPreLevelBadge>().enabled=false;
            Place(badge,432,325,78,79);
            if(v.CountBacking==null)v.CountBacking=Art(badge,"OriginalNavyFace",Load("badge_r2"),0,0,78,79);
            v.CountBacking.transform.SetAsFirstSibling();Place(v.Count.rectTransform,7,8,64,62);v.Count.fontSize=43;
            Place(v.Message.rectTransform,50,420,696,110);v.Message.fontSize=34;
            Place(v.TimerHeading.rectTransform,177,531,430,43);v.TimerHeading.fontSize=34;
            Place((RectTransform)v.Body.Find("Hourglass"),292,572,55,61);
            Place(v.Countdown.rectTransform,359,573,202,65);v.Countdown.fontSize=53;
            if(v.ActionLettering==null||v.ActionLettering.Length!=3)v.ActionLettering=new Image[3];
            float[] titleY={660,793,926};float[] titleW={312,306,302};
            for(int i=0;i<3;i++)
            {
                if(v.ActionLettering[i]==null)v.ActionLettering[i]=Art(v.Body,"OriginalActionHeading_"+i,Load("action_title_"+i+"_ru_r2"),193,titleY[i],titleW[i],37);
                Place(v.ActionTitles[i].rectTransform,193,titleY[i],306,72);v.ActionTitles[i].fontSize=32;v.ActionTitles[i].alignment=TextAnchor.MiddleLeft;
                Place(v.ActionDetails[i].rectTransform,72,titleY[i]+74,658,36);v.ActionDetails[i].fontSize=30;v.ActionDetails[i].alignment=TextAnchor.MiddleCenter;
                Place((RectTransform)v.Actions[i].transform,502,titleY[i],228,72);
                Place(v.ActionLabels[i].rectTransform,32,11,164,50);v.ActionLabels[i].fontSize=22;v.ActionLabels[i].horizontalOverflow=HorizontalWrapMode.Overflow;
            }
            Place((RectTransform)v.Close.transform,686,38,112,119);
            var energy=v.Body.Find("ActionIcon_0").GetComponent<Image>();energy.sprite=Load("energy_r2");Place(energy.rectTransform,99,662,55,68);
            Place((RectTransform)v.Body.Find("ActionIcon_1"),89,801,64,65);
            Place((RectTransform)v.Body.Find("ActionIcon_2"),87,934,68,64);
        }
        public static void ApplySourceState(ProductionOutOfEnergyView v,bool ready,bool en)
        {
            v.HeadingLettering.gameObject.SetActive(!ready&&!en);v.Title.enabled=ready||en;
            for(int i=0;i<3;i++)
            {
                v.ActionLettering[i].gameObject.SetActive(false);v.ActionTitles[i].enabled=true;
                string key=i==0?"buy_blank_r2":i==1?(en?"ask_blank_r2":"ask_ru_r2"):(en?"watch_blank_r2":"watch_ru_r2");
                v.Actions[i].image.sprite=Load(key);v.ActionLabels[i].enabled=i==0||en;
                // UI Image keeps this top-left pivot when preserving the source aspect.
                // Center the label in the painted face, not the wider Button rectangle.
                var face=v.Actions[i].image;var rect=face.rectTransform.rect;
                float drawnWidth=Mathf.Min(rect.width,rect.height*face.sprite.rect.width/face.sprite.rect.height);
                v.ActionLabels[i].rectTransform.anchoredPosition=new Vector2((drawnWidth-v.ActionLabels[i].rectTransform.rect.width)*.5f,-11);
            }
        }
        static Button Button(Image image)
        {
            image.raycastTarget=true;var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.navigation=new Navigation{mode=Navigation.Mode.None};
            var c=b.colors;c.disabledColor=new Color(.72f,.72f,.72f,1);b.colors=c;return b;
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        static Image Art(Transform p,string n,Sprite sprite,float x,float y,float w,float h)
        {var image=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=sprite!=null;image.raycastTarget=false;return image;}
        static Text Label(ProductionOutOfEnergyView v,Transform p,string n,float x,float y,float w,float h,int size)
        {
            var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,211,147,255);
            t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;
        }
    }
}
