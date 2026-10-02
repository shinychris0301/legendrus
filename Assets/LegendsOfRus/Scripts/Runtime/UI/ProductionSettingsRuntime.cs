using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionSettingsRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/SettingsAligned/"+key);
        public static Sprite Shared(string key)=>Resources.Load<Sprite>("ProductionUI/IndividualRankingAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionSettingsView>();if(existing!=null){if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Settings outside Play Mode.");ApplyReadability(existing);existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionSettingsView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Settings needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"SETTINGS_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Resources.Load<Sprite>("ProductionUI/HomeReferenceAligned/clean_environment"),0,0,941,1672);v.Environment.color=new Color(.68f,.68f,.72f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            if(!shop.Header||!shop.Header.HasBindings)throw new InvalidOperationException("Settings requires the shared player header.");
            v.Header=UnityEngine.Object.Instantiate(shop.Header,v.Body,false);v.Header.name="NativeHud";v.Hud=(RectTransform)v.Header.transform;
            foreach(var button in v.Header.GetComponentsInChildren<Button>(true))AddPressFeedback(button);
            v.ProfileButton=v.Header.Profile;v.SettingsButton=v.Header.Settings;v.HeroName=v.Header.PlayerName;v.LevelText=v.Header.Level;v.PlusButtons=v.Header.Plus;
            v.GoldText=v.Header.Balances[0];v.GemsText=v.Header.Balances[1];v.EnergyText=v.Header.Balances[2];

            var frame=Art(v.Body,"CarvedFrame",Load("frame"),57,284,822,1363);frame.type=Image.Type.Sliced;frame.preserveAspect=false;frame.pixelsPerUnitMultiplier=1;
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),115,115,709,232);
            v.Heading=Label(v,v.Body,"Heading",235,203,472,87,65);Fit(v.Heading,40,65);
            Art(v.Body,"PreferencesPaper",Load("paper"),102,346,733,443);
            v.SwitchButtons=new Button[4];v.SwitchLabels=new Text[4];
            string[] icons={"music","sfx","vibration","notifications","language","cloud","account","support","privacy","logout"};
            for(int i=0;i<4;i++){
                Art(v.Body,"Icon_"+icons[i],Load("icon_"+icons[i]),133,374+i*98,103,105);
                v.SwitchLabels[i]=Label(v,v.Body,"PreferenceLabel_"+i,249,392+i*98,398,60,36);v.SwitchLabels[i].alignment=TextAnchor.MiddleLeft;v.SwitchLabels[i].color=new Color32(44,28,12,255);Fit(v.SwitchLabels[i],26,36);
                v.SwitchButtons[i]=Button(Art(v.Body,"Preference_"+i,Load("switch_on"),658,389+i*98,147,69));
                // Keep source switch geometry while extending the touch area on narrow phones.
                v.SwitchButtons[i].image.raycastPadding=new Vector4(0,-4,0,-4);
                if(i<3)Art(v.Body,"Divider_"+i,Resources.Load<Sprite>("ProductionUI/MailAligned/letter_divider_r3"),246,465+i*98,548,14).preserveAspect=false;
            }
            v.Rows=new Button[6];v.RowLabels=new Text[6];
            for(int i=0;i<6;i++){
                float y=789+i*111;v.Rows[i]=Button(Art(v.Body,"SettingsRow_"+i,Load(i==5?"logout":"row"),104,y,732,i==5?124:109));
                Art(v.Rows[i].transform,"Icon",Load("icon_"+icons[i+4]),29,5,103,105);
                Art(v.Rows[i].transform,"Arrow",Load("arrow"),638,23,61,63);
                v.RowLabels[i]=Label(v,v.Rows[i].transform,"Label",145,22,i<2?255:482,i==5?52:68,36);v.RowLabels[i].alignment=TextAnchor.MiddleLeft;Fit(v.RowLabels[i],24,36);
            }
            v.LanguageValue=Label(v,v.Rows[0].transform,"LanguageValue",440,22,190,68,33);Fit(v.LanguageValue,24,33);
            v.CloudValue=Label(v,v.Rows[1].transform,"CloudValue",418,22,212,68,27);Fit(v.CloudValue,20,27);
            v.SignOutNote=Label(v,v.Rows[5].transform,"Availability",145,76,482,29,24);Fit(v.SignOutNote,18,24);
            v.BackButton=Button(Art(v.Body,"Back",Load("back"),380,1477,180,168));
            v.VersionText=Label(v,v.Body,"Version",636,1573,266,45,22);Fit(v.VersionText,16,22);
            BuildInfo(v);UpgradeLanguageModal(v);ApplyReadability(v);var layout=root.gameObject.AddComponent<ProductionSettingsLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Settings bindings incomplete.");
        }
        static void BuildInfo(ProductionSettingsView v)
        {
            v.InfoPanel=Rect(v.Body,"InfoOverlay",0,0,941,1672).gameObject;
            var shield=Rect(v.InfoPanel.transform,"InputShield",0,0,941,1672).gameObject.AddComponent<ProductionHomePanel>();shield.Frame=false;shield.Radius=0;shield.LeftColor=shield.RightColor=new Color(0,0,0,.92f);shield.raycastTarget=true;
            Panel(v.InfoPanel.transform,"Frame",Shared("reward_panel"),77,469,787,709);
            v.InfoHeading=Label(v,v.InfoPanel.transform,"Heading",120,505,701,99,37);Fit(v.InfoHeading,25,37);
            v.InfoDescription=Label(v,v.InfoPanel.transform,"Description",132,638,677,246,32);Fit(v.InfoDescription,23,32);
            v.LanguageChoices=Rect(v.InfoPanel.transform,"LanguageChoices",132,906,677,91).gameObject;v.LanguageButtons=new Button[2];v.LanguageLabels=new Text[2];
            for(int i=0;i<2;i++){v.LanguageButtons[i]=Button(Panel(v.LanguageChoices.transform,"Language_"+i,Shared("tab"),i*347,0,330,90));v.LanguageLabels[i]=Label(v,v.LanguageButtons[i].transform,"Label",15,15,300,60,32);}
            v.CloseInfoButton=Button(Panel(v.InfoPanel.transform,"Close",Shared("rules"),272,1040,397,87));v.CloseInfoLabel=Label(v,v.CloseInfoButton.transform,"Label",24,16,349,56,33);v.InfoPanel.SetActive(false);
        }
        public static void UpgradeLanguageModal(ProductionSettingsView v)
        {
            if(v.LanguagePanel!=null)return;
            // Upgrade only the explanation subtree; keep all R1 serialized controls and object IDs.
            v.GeneralInfoContent=Rect(v.InfoPanel.transform,"GeneralInfoContent",0,0,941,1672).gameObject;
            foreach(Transform child in new[]{v.InfoPanel.transform.Find("Frame"),v.InfoHeading.transform,v.InfoDescription.transform,v.CloseInfoButton.transform})child.SetParent(v.GeneralInfoContent.transform,false);
            var choices=(RectTransform)v.LanguageChoices.transform;choices.SetAsLastSibling();choices.anchoredPosition=Vector2.zero;choices.sizeDelta=new Vector2(941,1672);
            v.LanguagePanel=Art(choices,"LanguagePanel",Load("language_panel_ru"),179,573,581,455);v.LanguagePanel.transform.SetAsFirstSibling();
            v.LanguageHeading=Label(v,choices,"LanguageHeading",266,659,427,65,55);Fit(v.LanguageHeading,35,55);
            for(int i=0;i<2;i++){
                var rect=(RectTransform)v.LanguageButtons[i].transform;rect.anchoredPosition=new Vector2(240,-(i==0?765:888));rect.sizeDelta=new Vector2(455,103);
                v.LanguageButtons[i].image.type=Image.Type.Simple;v.LanguageButtons[i].image.preserveAspect=true;
                var text=v.LanguageLabels[i];text.rectTransform.anchoredPosition=new Vector2(58,-12);text.rectTransform.sizeDelta=new Vector2(339,79);text.fontSize=53;Fit(text,38,53);text.color=new Color32(244,202,119,255);
                var shadow=text.gameObject.GetComponent<Shadow>()??text.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(.16f,.075f,.008f,1);shadow.effectDistance=new Vector2(1.1f,-1.5f);
            }
            v.CloseLanguageButton=Button(Art(choices,"CloseLanguage",Load("language_close"),693,605,71,76));
            v.CloseLanguageButton.image.raycastPadding=new Vector4(-3,0,-3,0);
            v.Revision=ProductionSettingsView.CurrentRevision;
        }
        // Shared with the owning builder so serialized and runtime presentations stay in agreement.
        public static void ApplyReadability(ProductionSettingsView v)
        {
            Place(v.Title.rectTransform,160,110,620,203);
            Place(v.Heading.rectTransform,265,185,412,76);
            Place((RectTransform)v.Body.Find("PreferencesPaper"),102,310,733,503);
            var paper=v.Body.Find("PreferencesPaper").GetComponent<Image>();paper.type=Image.Type.Sliced;paper.preserveAspect=false;paper.pixelsPerUnitMultiplier=2;
            string[] icons={"music","sfx","vibration","notifications"};
            for(int i=0;i<4;i++)
            {
                Place((RectTransform)v.Body.Find("Icon_"+icons[i]),133,327+i*124,103,105);
                Place(v.SwitchLabels[i].rectTransform,249,345+i*124,398,60);Fit(v.SwitchLabels[i],32,40);
                Place((RectTransform)v.SwitchButtons[i].transform,658,342+i*124,147,69);
                v.SwitchButtons[i].image.raycastPadding=new Vector4(0,-27,0,-27);
                if(i<3)Place((RectTransform)v.Body.Find("Divider_"+i),246,432+i*124,548,14);
            }
            for(int i=0;i<6;i++)((RectTransform)v.Rows[i].transform).anchoredPosition=new Vector2(104,-(818+i*111));
            Place(v.RowLabels[1].rectTransform,145,22,220,68);
            Place(v.CloudValue.rectTransform,375,34,255,44);Fit(v.CloudValue,28,33);Fit(v.LanguageValue,28,36);
            Place(v.SignOutNote.rectTransform,145,76,482,34);Fit(v.SignOutNote,26,28);
            Place((RectTransform)v.BackButton.transform,395,1504,151,141);
            Fit(v.VersionText,24,30);
            Place(v.InfoDescription.rectTransform,132,628,677,290);Fit(v.InfoDescription,30,36);
            Place((RectTransform)v.CloseInfoButton.transform,210,1020,520,124);
            Place(v.CloseInfoLabel.rectTransform,40,29,440,66);v.CloseInfoLabel.fontSize=36;
            var choices=(RectTransform)v.LanguageChoices.transform;
            choices.localScale=Vector3.one*1.35f;
            choices.anchoredPosition=new Vector2((941-941*1.35f)*.5f,(1672*1.35f-1672)*.5f);
            v.CloseLanguageButton.image.raycastPadding=new Vector4(-12,-10,-12,-10);
        }
        static void Place(RectTransform r,float x,float y,float w,float h){r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="paper")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="paper")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};AddPressFeedback(b);return b;}
        static void AddPressFeedback(Button b){b.transition=Selectable.Transition.None;if(b.targetGraphic)b.targetGraphic.CrossFadeColor(Color.white,0,true,true);if(!b.GetComponent<ProductionMapButtonFeedback>())b.gameObject.AddComponent<ProductionMapButtonFeedback>();}
        static Text Label(ProductionSettingsView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
