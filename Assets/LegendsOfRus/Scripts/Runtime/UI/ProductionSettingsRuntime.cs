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
            var existing=root.GetComponent<ProductionSettingsView>();if(existing!=null){UpgradeLanguageModal(existing);if(!existing.HasRequiredBindings)throw new InvalidOperationException("Repair Settings outside Play Mode.");existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionSettingsView>();
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();if(!shop||!shop.HasRequiredBindings)throw new InvalidOperationException("Settings needs the approved navigation header.");
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"SETTINGS_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Shared("background"),0,0,941,1672);v.Environment.color=new Color(.3f,.3f,.34f,1);v.Body=Rect(v.Content,"Composition",0,0,941,1672);v.Hud=Rect(v.Body,"NativeHud",0,0,941,235);
            Clone(shop.Body.Find("NativeHudBacking"),v.Hud);v.ProfileButton=Clone(shop.ProfileButton.transform,v.Hud).GetComponent<Button>();v.SettingsButton=Clone(shop.SettingsButton.transform,v.Hud).GetComponent<Button>();v.HeroName=Clone(shop.HeroName.transform,v.Hud).GetComponent<Text>();v.LevelText=Clone(shop.LevelText.transform,v.Hud).GetComponent<Text>();
            v.PlusButtons=new Button[3];Text[] sourceValues={shop.GoldText,shop.GemsText,shop.EnergyText},values=new Text[3];
            for(int i=0;i<3;i++){Clone(shop.Body.Find("ResourceCapsule_"+i),v.Hud);Clone(shop.Body.Find("ResourceIcon_"+i),v.Hud);values[i]=Clone(sourceValues[i].transform,v.Hud).GetComponent<Text>();v.PlusButtons[i]=Clone(shop.PlusButtons[i].transform,v.Hud).GetComponent<Button>();}
            v.GoldText=values[0];v.GemsText=values[1];v.EnergyText=values[2];

            Art(v.Body,"CarvedFrame",Load("frame"),57,284,822,1363);
            v.Title=Art(v.Body,"TitlePlaque",Load("title_ru"),115,115,709,232);
            v.Heading=Label(v,v.Body,"Heading",235,203,472,87,65);Fit(v.Heading,40,65);
            Art(v.Body,"PreferencesPaper",Load("paper"),102,346,733,443);
            v.SwitchButtons=new Button[4];v.SwitchLabels=new Text[4];
            string[] icons={"music","sfx","vibration","notifications","language","cloud","account","support","privacy","logout"};
            for(int i=0;i<4;i++){
                Art(v.Body,"Icon_"+icons[i],Load("icon_"+icons[i]),133,374+i*98,103,105);
                v.SwitchLabels[i]=Label(v,v.Body,"PreferenceLabel_"+i,249,392+i*98,398,70,40);v.SwitchLabels[i].alignment=TextAnchor.MiddleLeft;v.SwitchLabels[i].color=new Color32(44,28,12,255);Fit(v.SwitchLabels[i],28,40);
                v.SwitchButtons[i]=Button(Art(v.Body,"Preference_"+i,Load("switch_on"),658,389+i*98,147,69));
                if(i<3){var line=Rect(v.Body,"Divider_"+i,246,470+i*98,548,2).gameObject.AddComponent<ProductionHomePanel>();line.Frame=false;line.Radius=0;line.LeftColor=line.RightColor=new Color(.38f,.23f,.07f,.42f);line.raycastTarget=false;}
            }
            v.Rows=new Button[6];v.RowLabels=new Text[6];
            for(int i=0;i<6;i++){
                float y=789+i*111;v.Rows[i]=Button(Art(v.Body,"SettingsRow_"+i,Load(i==5?"logout":"row"),104,y,732,i==5?124:109));
                Art(v.Rows[i].transform,"Icon",Load("icon_"+icons[i+4]),29,5,103,105);
                Art(v.Rows[i].transform,"Arrow",Load("arrow"),638,23,61,63);
                v.RowLabels[i]=Label(v,v.Rows[i].transform,"Label",145,22,i<2?280:482,68,i==5?36:40);v.RowLabels[i].alignment=TextAnchor.MiddleLeft;Fit(v.RowLabels[i],27,i==5?36:40);
            }
            v.LanguageValue=Label(v,v.Rows[0].transform,"LanguageValue",430,22,207,68,36);Fit(v.LanguageValue,27,36);
            v.CloudValue=Label(v,v.Rows[1].transform,"CloudValue",408,22,229,68,29);Fit(v.CloudValue,23,29);
            v.BackButton=Button(Art(v.Body,"Back",Load("back"),380,1477,180,168));
            v.VersionText=Label(v,v.Body,"Version",636,1573,266,45,22);Fit(v.VersionText,16,22);
            BuildInfo(v);UpgradeLanguageModal(v);var layout=root.gameObject.AddComponent<ProductionSettingsLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("Settings bindings incomplete.");
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
            if(v.LanguagePanel!=null){v.Revision=ProductionSettingsView.CurrentRevision;return;}
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
            v.Revision=ProductionSettingsView.CurrentRevision;
        }
        static void Fit(Text t,int min,int max){t.resizeTextForBestFit=true;t.resizeTextMinSize=min;t.resizeTextMaxSize=max;}
        static Transform Clone(Transform t,Transform p){var c=UnityEngine.Object.Instantiate(t,p,false);c.name=t.name;return c;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=s&&(s.name=="paper")?Image.Type.Simple:Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=s&&(s.name=="paper")?1:s&&s.name=="reward_frame"?6:2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionSettingsView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.BodyFont;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(243,206,137,255);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
