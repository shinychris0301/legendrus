using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Whole isolated art, live labels and native controls for the selected level.</summary>
    public static class ProductionPreLevelRuntime
    {
        public const string ContentName = "PRE_LEVEL_ALIGNED_CONTENT";
        public const string ArtRoot = "ProductionUI/PreLevelAligned/";
        public static readonly string[] ArtNames = { "panel_goals", "panel_wide", "ruby", "mace", "firebird", "shield", "potion", "chest" };
        public static readonly BoosterType[] Boosters = { BoosterType.BogatyrMace, BoosterType.Firebird, BoosterType.LivingLight, BoosterType.SeaPearl };

        public static void BuildInto(RectTransform root)
        {
            var existing = root.GetComponent<ProductionPreLevelView>();
            if (existing != null && existing.HasRequiredBindings) { AlignPresentation(existing); return; }
            if (root.Find(ContentName) != null) throw new InvalidOperationException("Pre-Level bindings need repair outside Play Mode.");
            var source = Resources.Load<GameObject>("ProductionUI/Pages/ChapterMap").GetComponent<ProductionChapterMapView>();
            if (source == null || !source.HasRequiredBindings) throw new InvalidOperationException("Pre-Level requires the accepted Chapter Map.");
            var view = existing != null ? existing : root.gameObject.AddComponent<ProductionPreLevelView>();
            view.Typography = source.Typography;
            view.Content = Rect(root,ContentName,0,0,941,1672);
            view.Environment = Clone(source.Environment.rectTransform,view.Content,"Environment").GetComponent<Image>();
            view.Body = Rect(view.Content,"Preparation",0,400,941,1040);
            view.GoalsArea = Rect(view.Body,"Goals",185,0,571,360);
            Art(view.GoalsArea,"GoalsPanel",Load("panel_goals"),0,0,571,360);
            view.GoalsHeading = Label(view,view.GoalsArea,"GoalsHeading","ЦЕЛИ",100,43,371,48,34);
            view.SelectedLevel = Label(view,view.GoalsArea,"SelectedLevel","УРОВЕНЬ 1",100,88,371,30,21);
            view.Goals = new PreLevelGoalView[2];
            view.Ruby = Load("ruby"); view.Sapphire = Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+"gems_icon");
            view.GoalMedallion = Resources.Load<Sprite>(ProductionChapterMapRuntime.ArtRoot+"node");
            for (int i=0;i<2;i++)
            {
                var slot = Rect(view.GoalsArea,"Goal_"+i,110+i*195,118,156,200);
                view.Goals[i] = new PreLevelGoalView {
                    Root=slot, Icon=Art(slot,"GoalIcon",i==0?view.Ruby:view.Sapphire,8,0,140,140),
                    Description=Label(view,slot,"GoalDescription","",0,33,156,72,24),
                    Count=Label(view,slot,"GoalCount","12",0,142,156,49,42)
                };
                view.Goals[i].Icon.preserveAspect=true;
            }
            view.MovesArea=Rect(view.Body,"Moves",370,300,200,200);
            Art(view.MovesArea,"MovesMedallion",view.GoalMedallion,0,0,200,200);
            view.MovesHeading=Label(view,view.MovesArea,"MovesHeading","ХОДЫ",20,40,160,42,26);
            view.MovesValue=Label(view,view.MovesArea,"MovesValue","28",25,78,150,78,62);
            view.BoostersArea=Rect(view.Body,"Boosters",105,455,731,295);
            Art(view.BoostersArea,"BoostersPanel",Load("panel_wide"),0,0,731,295);
            view.BoostersHeading=Label(view,view.BoostersArea,"BoostersHeading","ВЫБЕРИТЕ БУСТЕРЫ",50,29,631,47,32);
            view.BoosterButtons=new Button[4];view.BoosterCounts=new Text[4];view.BonusRoots=new RectTransform[4];view.BonusCounts=new Text[4];
            string[] art = {"mace","firebird","shield","potion"};
            for(int i=0;i<4;i++)
            {
                var face=Art(view.BoostersArea,"Booster_"+Boosters[i],Load(art[i]),46+i*164,93,150,150);
                view.BoosterButtons[i]=Button(face);
                var badge=Rect(face.transform,"StockBadge",107,112,46,46).gameObject.AddComponent<ProductionPreLevelBadge>();badge.raycastTarget=false;
                view.BoosterCounts[i]=Label(view,badge.transform,"Count","0",0,0,46,46,24);
                view.BoosterCounts[i].resizeTextForBestFit=true;view.BoosterCounts[i].resizeTextMinSize=20;view.BoosterCounts[i].resizeTextMaxSize=48;
            }
            view.RewardsArea=Rect(view.Body,"Rewards",105,755,731,280);
            Art(view.RewardsArea,"RewardsPanel",Load("panel_wide"),0,0,731,280);
            view.RewardsHeading=Label(view,view.RewardsArea,"RewardsHeading","НАГРАДА ЗА УРОВЕНЬ",50,23,631,45,31);
            view.RewardStatus=Label(view,view.RewardsArea,"RewardStatus","",260,67,400,29,20);
            Art(view.RewardsArea,"Chest",Load("chest"),65,76,205,190).preserveAspect=true;
            Art(view.RewardsArea,"Coin",Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+"coin_icon"),350,93,58,58).preserveAspect=true;
            Art(view.RewardsArea,"Gem",view.Sapphire,350,154,58,58).preserveAspect=true;
            view.RewardGold=Label(view,view.RewardsArea,"RewardGold","120",420,96,175,54,38);view.RewardGold.alignment=TextAnchor.MiddleLeft;
            view.RewardGems=Label(view,view.RewardsArea,"RewardGems","0",420,157,175,54,38);view.RewardGems.alignment=TextAnchor.MiddleLeft;
            for(int i=0;i<4;i++)
            {
                view.BonusRoots[i]=Rect(view.RewardsArea,"Bonus_"+Boosters[i],310+i*83,218,83,40);
                Art(view.BonusRoots[i],"Icon",Load(art[i]),0,0,38,38);
                view.BonusCounts[i]=Label(view,view.BonusRoots[i],"Count","1",38,0,43,38,23);
            }
            view.HudBacking=Clone(source.HudBacking,view.Content,"HudOpaqueNavyBacking");
            view.Hud=Clone(source.Hud,view.Content,"PreLevelHud");
            view.ProfileButton=view.Hud.Find("ProfileButton").GetComponent<Button>();view.SettingsButton=view.Hud.Find("SettingsButton").GetComponent<Button>();
            view.HeroName=view.ProfileButton.transform.Find("HeroName").GetComponent<Text>();view.LevelText=view.ProfileButton.transform.Find("SavedLevel").GetComponent<Text>();
            view.GoldText=view.Hud.Find("GoldResource/GoldValue").GetComponent<Text>();view.GemsText=view.Hud.Find("GemsResource/GemsValue").GetComponent<Text>();view.EnergyText=view.Hud.Find("EnergyResource/EnergyValue").GetComponent<Text>();
            view.PlusButtons=new[]{view.Hud.Find("GoldResource/ResourcePlus").GetComponent<Button>(),view.Hud.Find("GemsResource/ResourcePlus").GetComponent<Button>(),view.Hud.Find("EnergyResource/ResourcePlus").GetComponent<Button>()};
            view.TitleArea=Clone(source.TitleArea,view.Content,"TitleArea");view.ChapterText=view.TitleArea.Find("Chapter").GetComponent<Text>();view.TitleText=view.TitleArea.Find("Title").GetComponent<Text>();
            view.FooterOrnament=Clone(source.FooterOrnament.rectTransform,view.Content,"FooterOrnament").GetComponent<Image>();
            view.Footer=Clone(source.Footer,view.Content,"Navigation");view.BackButton=view.Footer.Find("Back").GetComponent<Button>();view.PlayButton=view.Footer.Find("PlayButton").GetComponent<Button>();view.PlayText=view.PlayButton.transform.Find("PlayLabel").GetComponent<Text>();
            var layout=root.GetComponent<ProductionPreLevelLayout>()??root.gameObject.AddComponent<ProductionPreLevelLayout>();layout.View=view;
            view.Revision=ProductionPreLevelView.CurrentRevision;AlignPresentation(view);view.RefreshContent();layout.ApplyNow();
            if(!view.HasRequiredBindings)throw new InvalidOperationException("Pre-Level composition is incomplete.");
        }
        static void AlignPresentation(ProductionPreLevelView view)
        {
            // Position live lettering inside the lacquer surface, clear of the raised trim.
            view.GoalsArea.sizeDelta=new Vector2(571,380);
            ((RectTransform)view.GoalsArea.Find("GoalsPanel")).sizeDelta=new Vector2(571,380);
            Place(view.GoalsHeading,100,77,371,42,32);Place(view.SelectedLevel,100,117,371,30,20);
            for(int i=0;i<2;i++)
            {
                var goal=view.Goals[i];goal.Root.anchoredPosition=new Vector2(110+i*195,-145);
                Place(goal.Description,18,19,120,70,17);Place(goal.Count,0,119,156,44,36);
            }
            Place(view.BoostersHeading,50,49,631,43,30);
            Place(view.RewardsHeading,50,44,631,43,29);Place(view.RewardStatus,265,81,395,24,18);
            Place(view.RewardGold,420,100,175,42,32);Place(view.RewardGems,420,149,175,42,32);
            var coin=(RectTransform)view.RewardsArea.Find("Coin");coin.anchoredPosition=new Vector2(350,-100);coin.sizeDelta=new Vector2(42,42);
            var gem=(RectTransform)view.RewardsArea.Find("Gem");gem.anchoredPosition=new Vector2(350,-149);gem.sizeDelta=new Vector2(42,42);
            for(int i=0;i<4;i++)
            {
                view.BonusRoots[i].sizeDelta=new Vector2(83,32);
                ((RectTransform)view.BonusRoots[i].Find("Icon")).sizeDelta=new Vector2(32,32);
                Place(view.BonusCounts[i],32,0,45,32,20);
            }
        }
        static void Place(Text text,float x,float y,float w,float h,int size)
        {
            text.rectTransform.anchoredPosition=new Vector2(x,-y);text.rectTransform.sizeDelta=new Vector2(w*2,h*2);text.fontSize=size*2;
        }
        static Sprite Load(string name)=>Resources.Load<Sprite>(ArtRoot+name)??throw new InvalidOperationException("Missing Pre-Level art: "+name);
        static RectTransform Clone(RectTransform source,Transform parent,string name){var go=UnityEngine.Object.Instantiate(source.gameObject,parent,false);go.name=name;return (RectTransform)go.transform;}
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h)=>ProductionChapterMapRuntime.Rect(p,n,x,y,w,h);
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){var image=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=s;image.raycastTarget=false;return image;}
        static Button Button(Image image)
        {
            image.raycastTarget=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;button.navigation=new Navigation{mode=Navigation.Mode.None};
            var colors=button.colors;colors.pressedColor=new Color(.8f,.8f,.8f,1);colors.selectedColor=Color.white;colors.fadeDuration=.06f;button.colors=colors;
            button.gameObject.AddComponent<ProductionHomeButtonFeedback>();return button;
        }
        static Text Label(ProductionPreLevelView v,Transform p,string n,string value,float x,float y,float w,float h,int size)
        {
            var text=Rect(p,n,x,y,w*2,h*2).gameObject.AddComponent<Text>();text.rectTransform.localScale=Vector3.one*.5f;
            text.text=value;text.font=v.Typography.RegularFont;text.fontSize=size*2;text.fontStyle=FontStyle.Bold;text.color=new Color32(243,213,156,255);
            text.alignment=TextAnchor.MiddleCenter;text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;text.raycastTarget=false;
            text.gameObject.AddComponent<SplashTitleTextGeometry>().SnapToScreenPixels=false;
            text.gameObject.AddComponent<ProductionHomeGoldText>().BeveledFace=true;text.material=Resources.Load<Material>(ProductionHomeRuntime.ArtRoot+"home_play_gold");return text;
        }
    }
}
