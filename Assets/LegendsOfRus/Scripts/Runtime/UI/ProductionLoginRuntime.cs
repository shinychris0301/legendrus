using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionLoginRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/LoginAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionLoginView>();if(old){if(!old.HasRequiredBindings)throw new InvalidOperationException("Repair Login outside Play Mode.");old.LinkLabel.rectTransform.anchoredPosition=new Vector2(64,-12);old.LinkLabel.rectTransform.sizeDelta=new Vector2(213,27);old.LinkLabel.fontSize=16;Fit(old.LinkLabel,13,16);EnsureInfoPresentation(old);old.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionLoginView>();v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"LOGIN_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/FrontEndHomeAligned/loading_environment_dusk"),0,0,941,1672);
            v.Environment.color=new Color(.84f,.84f,.88f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"IvanAndFirebird",Load("subject"),0,393,941,877);
            Art(v.Body,"UpperLeftOrnament",Load("corner_top"),0,0,310,293);
            var right=Art(v.Body,"UpperRightOrnament",Load("corner_top"),631,0,310,293);right.rectTransform.pivot=new Vector2(.5f,.5f);right.rectTransform.anchoredPosition=new Vector2(786,-146.5f);right.rectTransform.localScale=new Vector3(-1,1,1);
            Art(v.Body,"LowerLeftOrnament",Load("corner_left"),0,1270,145,402);
            Art(v.Body,"LowerRightOrnament",Load("corner_right"),811,1198,130,474);
            Art(v.Body,"ApprovedLogo",Resources.Load<Sprite>("ProductionUI/HomeAligned/home_logo"),213,46,503,352);
            v.Panel=Art(v.Body,"AccountPanel",Load("panel"),133,794,680,878);
            v.Heading=Label(v,v.Body,"Heading",250,873,445,50,31,true);Fit(v.Heading,22,31);
            v.Identity=Field(v,"Identity",940,false);Art(v.Body,"IdentityIcon",Load("user"),220,946,37,43);
            v.Password=Field(v,"Password",1020,true);Art(v.Body,"PasswordIcon",Load("lock"),222,1030,31,44);
            v.Eye=Button(Art(v.Body,"PasswordVisibility",Load("eye"),665,1026,65,52));
            v.Login=Button(Art(v.Body,"SignIn",Load("login"),210,1098,521,102));v.LoginLabel=Label(v,v.Login.transform,"Label",66,17,388,68,49,true);Fit(v.LoginLabel,34,49);
            v.Availability=Label(v,v.Body,"Availability",224,1194,493,28,22);v.Availability.color=new Color32(71,39,15,255);Fit(v.Availability,18,22);
            v.Divider=Label(v,v.Body,"ProviderDivider",282,1220,381,30,23);v.Divider.color=new Color32(59,34,12,255);Fit(v.Divider,18,23);
            v.Providers=new Button[4];string[] providers={"vk","ok","mail","google"};int[] xs={265,372,484,596};
            for(int i=0;i<4;i++)v.Providers[i]=Button(Art(v.Body,"Provider_"+providers[i],Load(providers[i]),xs[i],1248,80,75));
            v.Guest=Button(Art(v.Body,"Guest",Load("guest"),208,1335,525,94));v.GuestLabel=Label(v,v.Guest.transform,"Label",57,15,411,62,32,true);Fit(v.GuestLabel,24,32);
            v.Link=Button(Art(v.Body,"LinkAccount",Load("link"),327,1431,289,51));v.LinkLabel=Label(v,v.Link.transform,"Label",64,12,213,27,16);Fit(v.LinkLabel,13,16);v.LinkLabel.color=new Color32(111,189,223,255);
            Art(v.Body,"ConsentNotRecorded",Load("checkbox"),253,1490,51,46);
            v.Terms=TextButton(v,"Terms",313,1490,383,33,out v.TermsLabel);v.Privacy=TextButton(v,"Privacy",313,1526,383,33,out v.PrivacyLabel);
            v.Back=Button(Art(v.Body,"Back",ProductionSettingsRuntime.Load("back"),15,157,91,85));
            BuildInfo(v);EnsureInfoPresentation(v);var layout=root.gameObject.AddComponent<ProductionLoginLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();
        }
        static void EnsureInfoPresentation(ProductionLoginView v)
        {
            var panel=v.InfoPanel.transform.Find("Panel").GetComponent<Image>();panel.type=Image.Type.Sliced;panel.preserveAspect=false;panel.pixelsPerUnitMultiplier=2;
            var rootImage=v.InfoPanel.GetComponent<Image>();rootImage.color=Color.clear;rootImage.raycastTarget=false;
            if(!v.InfoPanel.transform.Find("FullscreenShade")){
                var rect=Rect(v.InfoPanel.transform,"FullscreenShade",0,0,941,1672);rect.SetAsFirstSibling();var shade=rect.gameObject.AddComponent<Image>();shade.color=new Color(0,0,0,.88f);shade.raycastTarget=true;
            }
        }
        static InputField Field(ProductionLoginView v,string name,float y,bool password)
        {
            var art=Art(v.Body,name,Load("field"),199,y,539,65);art.raycastTarget=true;art.preserveAspect=false;
            var input=art.gameObject.AddComponent<InputField>();input.targetGraphic=art;input.characterLimit=password?128:254;input.contentType=password?InputField.ContentType.Password:InputField.ContentType.Standard;input.keyboardType=password?TouchScreenKeyboardType.Default:TouchScreenKeyboardType.EmailAddress;
            var text=Label(v,art.transform,"Value",79,6,password?380:444,53,28);text.alignment=TextAnchor.MiddleLeft;text.supportRichText=false;text.horizontalOverflow=HorizontalWrapMode.Overflow;
            var placeholder=Label(v,art.transform,"Placeholder",79,6,password?380:444,53,28);placeholder.alignment=TextAnchor.MiddleLeft;placeholder.color=new Color32(189,164,120,255);Fit(placeholder,21,28);
            input.textComponent=text;input.placeholder=placeholder;input.text="";input.navigation=new Navigation{mode=Navigation.Mode.None};return input;
        }
        static Button TextButton(ProductionLoginView v,string name,float x,float y,float w,float h,out Text label)
        {
            var a=Rect(v.Body,name,x,y,w,h).gameObject.AddComponent<Image>();a.color=new Color(0,0,0,.001f);var button=Button(a);label=Label(v,a.transform,"Label",0,0,w,h,23);Fit(label,19,23);label.alignment=TextAnchor.MiddleLeft;label.color=new Color32(31,80,100,255);return button;
        }
        static void BuildInfo(ProductionLoginView v)
        {
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;var shield=v.InfoPanel.AddComponent<Image>();shield.color=new Color(0,0,0,.88f);shield.raycastTarget=true;
            Art(v.InfoPanel.transform,"Panel",ProductionSettingsRuntime.Shared("reward_panel"),100,506,741,580);
            v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",145,535,651,86,36,true);Fit(v.InfoHeading,24,36);
            v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",162,646,617,236,29);v.InfoDescription.horizontalOverflow=HorizontalWrapMode.Wrap;Fit(v.InfoDescription,23,29);
            v.InfoClose=Button(Art(v.InfoPanel.transform,"Close",ProductionSettingsRuntime.Shared("rules"),272,944,397,87));v.InfoCloseLabel=Label(v,v.InfoClose.transform,"Label",24,16,349,56,32,true);Fit(v.InfoCloseLabel,23,32);v.InfoPanel.SetActive(false);
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Login art "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionLoginView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
