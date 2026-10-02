using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionDailyRewardRuntime
    {
        public static Sprite Load(string n)=>Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/"+n);
        static Sprite Shared(string path)=>ProductionBoosterArt.Replacement(path)??Resources.Load<Sprite>("ProductionUI/"+path);
        public static void BuildInto(RectTransform root)
        {
            var existing=root.GetComponent<ProductionDailyRewardView>();if(existing!=null){if(Application.isPlaying && !existing.HasRequiredBindings)throw new InvalidOperationException("DailyReward bindings need repair outside Play Mode.");RefreshArt(existing);existing.RefreshContent();return;}
            var v=root.gameObject.AddComponent<ProductionDailyRewardView>();v.Typography=Resources.Load<SplashTitleTypography>("ProductionUI/SplashTitleConfig/SplashTitleTypography");
            v.Content=Rect(root,"DAILY_REWARD_ALIGNED_CONTENT",0,0,941,1672);v.Environment=Art(v.Content,"CleanEnvironment",Load("environment"),0,0,941,1672);v.Environment.preserveAspect=false;
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"Hud",Shared("ShopAligned/hud"),0,0,941,130);
            v.ProfileButton=Button(v.Body,"Profile",Shared("HomeAligned/avatar_ring"),4,6,156,193);
            var clip=Rect(v.ProfileButton.transform,"PortraitClip",24,21,108,108);var face=clip.gameObject.AddComponent<ProductionHomePanel>();face.Frame=false;face.Radius=54;face.LeftColor=face.RightColor=new Color32(7,24,42,255);face.raycastTarget=false;clip.gameObject.AddComponent<Mask>().showMaskGraphic=true;
            Art(clip,"Ivan",Shared("HomeAligned/avatar_portrait"),0,0,108,108);clip.SetAsFirstSibling();
            v.HeroName=Label(v,v.Body,"HeroName",175,28,150,38,33);v.LevelText=Label(v,v.Body,"Level",175,68,150,32,25);
            v.PlusButtons=new Button[3];Text[] balances=new Text[3];string[] resource={"coin_icon","gems_icon","energy_icon"};
            for(int i=0;i<3;i++)
            {
                float x=342+i*171;Art(v.Body,"ResourceCapsule_"+i,Shared("HomeAligned/resource_capsule"),x,44,164,52);
                Art(v.Body,"ResourceIcon_"+i,Shared("HomeAligned/"+resource[i]),x-7,33,53,66);
                balances[i]=Label(v,v.Body,"ResourceValue_"+i,x+42,49,80,38,25);balances[i].resizeTextForBestFit=true;balances[i].resizeTextMinSize=15;balances[i].resizeTextMaxSize=25;
                v.PlusButtons[i]=Button(v.Body,"Plus_"+i,Shared("HomeAligned/resource_plus"),x+121,48,42,42);
            }
            v.GoldText=balances[0];v.GemsText=balances[1];v.EnergyText=balances[2];v.SettingsButton=Button(v.Body,"Settings",Shared("HomeAligned/settings_medallion"),866,30,68,74);
            v.Header=Art(v.Body,"HeadingPlaque",Load("header"),86,110,773,257.67f);
            v.Title=Label(v,v.Body,"Heading",147,215,649,80,40);v.Intro=Label(v,v.Body,"Intro",50,334,841,49,30);
            v.DayLabels=new Text[7];v.Amounts=new Text[7];v.States=new Text[7];v.Cards=new Image[7];v.RewardIcons=new Image[7];
            string[] iconKeys={"HomeAligned/coin_icon","HomeAligned/gems_icon","HomeAligned/energy_icon","GameplayAligned/booster_livinglight_r9","GameplayAligned/booster_seapearl_r9","GameplayAligned/booster_mace_r9"};
            for(int i=0;i<7;i++)
            {
                float x=i<4?35+i*221:i<6?35+(i-4)*221:477;float y=i<4?390:757;float w=i==6?429:208;
                var card=Art(v.Body,"DayCard_"+(i+1),Load("card"),x,y,w,347);v.Cards[i]=card;
                if(i==6){card.type=Image.Type.Sliced;card.preserveAspect=false;card.pixelsPerUnitMultiplier=4;}
                v.DayLabels[i]=Label(v,card.transform,"Day",12,12,w-24,50,i==6?38:31);
                v.RewardIcons[i]=Art(card.transform,"Reward",i==6?Load("chest"):Shared(iconKeys[i]),i==6?51:29,80,i==6?327:150,i==6?187:157);
                v.Amounts[i]=Label(v,card.transform,"Amount",10,i==6?253:241,w-20,i==6?54:43,i==6?23:33);
                v.States[i]=Label(v,card.transform,"State",8,311,w-16,27,20);
            }
            var month=Art(v.Body,"MonthPanel",Shared("ShopAligned/empty_panel"),24,1124,893,205);month.preserveAspect=false; // Plain horizontal panel material, no circular ornament to distort.
            v.MonthTitle=Label(v,month.transform,"Heading",150,12,593,43,32);v.MonthCount=Label(v,month.transform,"Count",21,71,210,66,25);
            var rail=Art(month.transform,"Rail",Shared("HomeAligned/resource_capsule"),239,106,600,20);rail.preserveAspect=false;
            v.MonthFill=Art(month.transform,"Progress",Shared("HomeAligned/resource_capsule"),239,106,600,20);v.MonthFill.preserveAspect=false;v.MonthFill.type=Image.Type.Filled;v.MonthFill.fillMethod=Image.FillMethod.Horizontal;v.MonthFill.color=new Color32(110,177,129,255);
            for(int i=0;i<4;i++){Art(month.transform,"Milestone_"+i,Load("chest"),275+i*157,53,69,47);Label(v,month.transform,"Day_"+i,268+i*157,128,80,29,25).text=((i+1)*7).ToString();}
            v.MonthNote=Label(v,month.transform,"BonusAvailability",225,167,635,26,22);
            v.Tomorrow=Label(v,v.Body,"Tomorrow",90,1340,761,74,29);
            v.Status=Label(v,v.Body,"Rules",52,1426,837,61,23);
            v.ClaimButton=Button(v.Body,"Claim",Shared("ShopAligned/buy"),241,1520,501,115.5f);v.ClaimText=Label(v,v.ClaimButton.transform,"Label",40,23,421,66,40);
            v.BackButton=Button(v.Body,"Back",Shared("ShopAligned/tab"),19,1541,182,72);v.BackText=Label(v,v.BackButton.transform,"Label",12,12,158,46,27);
            RefreshArt(v);var layout=root.gameObject.AddComponent<ProductionDailyRewardLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();if(!v.HasRequiredBindings)throw new InvalidOperationException("DailyReward incomplete.");
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;}
        static void RefreshArt(ProductionDailyRewardView v)
        {
            v.Revision=ProductionDailyRewardView.CurrentRevision;
            ProductionPlayerHeader.Attach(v);
            var back=v.BackButton.GetComponent<Image>();back.sprite=Load("back_r2");back.preserveAspect=true;
            Place(back.rectTransform,10,1504,140,133);v.BackText.gameObject.SetActive(false);
            Place(v.Header.rectTransform,88,108,765,219);v.Header.sprite=Load("header_ru_r3");
            Place(v.Title.rectTransform,145,211,653,80);v.Title.fontSize=39;
            Place(v.Intro.rectTransform,90,328,761,47);v.Intro.fontSize=27;
            for(int i=0;i<7;i++)
            {
                var card=v.Cards[i];card.type=Image.Type.Simple;card.preserveAspect=true;
                float x=i<4?39+i*221:i<6?39+(i-4)*221:476,y=i<4?388:750,w=i==6?430:204,h=i==6?355:349;
                Place(card.rectTransform,x,y,w,h);
                Place(v.DayLabels[i].rectTransform,12,i==6?17:14,w-24,46);v.DayLabels[i].fontSize=i==6?38:31;
                Place(v.RewardIcons[i].rectTransform,i==6?58:27,i==6?79:83,i==6?314:150,i==6?179:157);
                Place(v.Amounts[i].rectTransform,10,i==6?255:231,w-20,i==6?52:43);v.Amounts[i].fontSize=i==6?23:33;
                Place(v.States[i].rectTransform,8,i==6?309:298,w-16,24);v.States[i].fontSize=19;
            }
            var dayPlaque=v.Cards[6].transform.Find("DaySevenPlaque")?.GetComponent<Image>();
            if(dayPlaque==null)dayPlaque=Art(v.Cards[6].transform,"DaySevenPlaque",Load("day7_title_ru_r5"),64,7,302,87.4f);
            if(dayPlaque.transform.GetSiblingIndex()>v.DayLabels[6].transform.GetSiblingIndex())dayPlaque.transform.SetSiblingIndex(v.DayLabels[6].transform.GetSiblingIndex());
            v.RewardIcons[6].sprite=Load("day7_chest_r5");
            Place(v.DayLabels[6].rectTransform,90,21,250,49);v.DayLabels[6].fontSize=36;
            var month=v.MonthTitle.transform.parent.GetComponent<Image>();month.sprite=Load("month_r3");month.preserveAspect=true;Place(month.rectTransform,23,1100,897,240);
            Place(v.MonthTitle.rectTransform,170,47,557,39);v.MonthTitle.fontSize=30;
            var badge=month.transform.Find("CountBadge")?.GetComponent<Image>();
            if(badge==null)badge=Art(month.transform,"CountBadge",Load("month_badge_r5"),37,83,101,122);
            if(badge.transform.GetSiblingIndex()>v.MonthCount.transform.GetSiblingIndex())badge.transform.SetSiblingIndex(v.MonthCount.transform.GetSiblingIndex());
            Place(v.MonthCount.rectTransform,48,101,78,88);v.MonthCount.fontSize=30;
            Place(v.MonthNote.rectTransform,210,185,653,24);v.MonthNote.fontSize=20;
            var rail=month.transform.Find("Rail").GetComponent<Image>();rail.sprite=Load("month_rail_r6");rail.color=Color.white;rail.preserveAspect=false;Place(rail.rectTransform,139,135,715,37);
            v.MonthFill.sprite=Load("month_fill_r6");v.MonthFill.color=Color.white;Place(v.MonthFill.rectTransform,139,135,715,37);
            for(int i=0;i<4;i++)
            {
                var icon=month.transform.Find("Milestone_"+i).GetComponent<Image>();icon.sprite=Load("milestone"+(i+1)+"_r5");
                Place(icon.rectTransform,i==3?713:202+i*179,i==3?64:76,i==3?146:100,i==3?98:84);icon.transform.SetAsLastSibling();
                var label=month.transform.Find("Day_"+i).GetComponent<Text>();Place(label.rectTransform,208+i*179,157,86,26);label.fontSize=26;
            }
            var next=v.Body.Find("TomorrowSourcePanel")?.GetComponent<Image>();
            if(next==null)next=Art(v.Body,"TomorrowSourcePanel",Load("tomorrow_r3"),45,1338,849,159);
            next.sprite=Load("tomorrow_r3");if(next.transform.GetSiblingIndex()>v.Tomorrow.transform.GetSiblingIndex())next.transform.SetSiblingIndex(v.Tomorrow.transform.GetSiblingIndex());
            Place(v.Tomorrow.rectTransform,95,1350,751,75);v.Tomorrow.fontSize=25;
            Place(v.Status.rectTransform,92,1427,757,46);v.Status.fontSize=20;
            if(v.ConfirmButton==null)v.ConfirmButton=Button(v.Body,"Confirm",Load("confirm_ru_r4"),225,1502,490,129);
            if(v.ConfirmText==null)v.ConfirmText=Label(v,v.ConfirmButton.transform,"Label",42,26,406,70,38);
            Place((RectTransform)v.ConfirmButton.transform,225,1502,490,129);
            Place(v.ClaimText.rectTransform,19,9,153,38);v.ClaimText.fontSize=24;
        }
        public static void ApplySourceState(ProductionDailyRewardView v,bool en,int day,bool available)
        {
            v.Header.sprite=Load(en?"header_blank_r3":"header_ru_r3");v.Title.enabled=en;
            for(int i=0;i<7;i++)
            {
                bool current=i+1==day&&available;
                v.Cards[i].sprite=Load(i==6?"day7_surface_r5":current?"card_active_r3":"card_r3");
                v.Cards[i].color=Color.white;
                v.States[i].gameObject.SetActive(!current);
            }
            // Original in-card claim and bottom confirmation have distinct actions.
            int index=Mathf.Clamp(day-1,0,6);
            v.ClaimButton.transform.SetParent(v.Cards[index].transform,false);
            Place((RectTransform)v.ClaimButton.transform,(v.Cards[index].rectTransform.rect.width-191)/2,277,191,59);
            v.ClaimButton.gameObject.SetActive(available);
            v.ClaimButton.GetComponent<Image>().sprite=Load(en?"claim_blank_r4":"claim_ru_r4");
            v.ClaimText.enabled=en;
            v.ConfirmButton.GetComponent<Image>().sprite=Load(en?"claim_blank_r3":"confirm_ru_r4");
            v.ConfirmText.enabled=en;
            var plaque=v.Cards[6].transform.Find("DaySevenPlaque").GetComponent<Image>();
            plaque.sprite=Load(en?"day7_title_blank_r5":"day7_title_ru_r5");v.DayLabels[6].enabled=en;
            float chestHeight=index==6&&available?143:187,chestWidth=chestHeight*314f/250f;
            Place(v.RewardIcons[6].rectTransform,(430-chestWidth)/2,81,chestWidth,chestHeight);
            Place(v.States[6].rectTransform,8,317,414,24);
            Place(v.Amounts[6].rectTransform,10,index==6&&available?224:268,410,index==6&&available?52:43);
            v.Amounts[6].fontSize=index==6&&available?23:21;
        }
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite sprite,float x,float y,float w,float h){var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=sprite;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Button Button(Transform p,string n,Sprite sprite,float x,float y,float w,float h){var a=Art(p,n,sprite,x,y,w,h);a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionDailyRewardView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=v.Typography.RegularFont;t.fontSize=size;t.color=new Color32(245,214,155,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
    }
}
