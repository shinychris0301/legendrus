using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Original Home foreground, extracted as individual source sprites. No reference screenshot at runtime.</summary>
    public static class ProductionHomeSourceComposition
    {
        public const string ArtRoot = "ProductionUI/HomeReferenceAligned/";
        public static readonly string[] Names = {
            "clean_environment", "source_hero", "source_logo", "source_avatar", "source_header",
            "source_play_ru", "source_play_blank", "source_map_ru", "source_map_blank",
            "source_shop_ru", "source_shop_blank", "source_events_ru", "source_events_blank",
            "source_characters_ru", "source_characters_blank", "source_gold", "source_gems", "source_energy",
            "source_settings", "source_plus", "source_mail", "source_quest_ru", "source_quest_blank",
            "source_gift", "source_footer_carving", "source_daily_badge", "source_events_badge"
        };
        public static readonly string[] ShortcutResources = {
            ArtRoot + "shortcut_ranking_r6", ArtRoot + "shortcut_team_r6", ArtRoot + "shortcut_friends_r6"
        };
        public static void Apply(ProductionHomeView v)
        {
            ProductionHomeRuntime.ClearDormantProgress(v);
            var layout = v.GetComponent<ProductionHomeLayout>();
            var content = layout.Content;
            // Retain serialized objects and native action ownership while retiring the old approximation.
            foreach (Graphic g in content.GetComponentsInChildren<Graphic>(true)) g.enabled = false;
            foreach (BaseMeshEffect effect in content.GetComponentsInChildren<BaseMeshEffect>(true)) effect.enabled = false;
            foreach (var g in content.GetComponentsInChildren<ProductionHomeSceneLighting>(true)) g.enabled = false;
            foreach (var g in content.GetComponentsInChildren<SplashTitleCharacterFade>(true)) g.enabled = false;
            Put(v.Environment, content, "clean_environment", 0, 0, 941, 1672);
            Put(v.Ivan, content, "source_hero", 0, 466, 941, 990);
            ProductionPlayerHeader.Attach(v);
            Put(v.Logo, content, "source_logo", 203, 107, 530, 379);
            // Main scene and native UI keep exactly the same source-coordinate registration.
            v.Environment.transform.SetAsFirstSibling();v.Ivan.transform.SetSiblingIndex(1);
            layout.Hud.SetAsLastSibling();v.Logo.transform.SetAsLastSibling();
            // Russian button lettering belongs to the source face; English uses the repaired blank variant.
            Rect(v.PlayButton.transform as RectTransform, content, 178, 998, 584, 216);
            v.SourcePlay = ControlArt(v.PlayButton, "source_play_ru", 0, 0, 584, 242);
            ProductionHomeRuntime.ClearDormantPlayArtwork(v.PlayButton);
            Text(v.PlayText, v.PlayButton.transform, 100, 84, 384, 98, 66);
            Rect(layout.Navigation, content, 0, 1214, 941, 236);
            Button[] buttons={v.WorldMapButton,v.ShopButton,v.EventsButton,v.CharactersButton};
            Text[] labels={v.WorldMapText,v.ShopText,v.EventsText,v.CharactersText};
            string[] names={"map","shop","events","characters"};float[] x={52,263,472,677};float[] widths={208,205,201,207};
            v.SourceNavigation = new Image[4];v.SourceRussianCards = new Sprite[4];v.SourceBlankCards = new Sprite[4];
            for(int i=0;i<4;i++)
            {
                Rect(buttons[i].transform as RectTransform,layout.Navigation,x[i],0,widths[i],236);
                v.SourceNavigation[i]=ControlArt(buttons[i],"source_"+names[i]+"_ru",0,0,widths[i],236);
                Text(labels[i],buttons[i].transform,13,176,widths[i]-26,35,23);
                v.SourceRussianCards[i]=Load("source_"+names[i]+"_ru");v.SourceBlankCards[i]=Load("source_"+names[i]+"_blank");
            }
            // Keep the source notification separate so hiding it reveals a complete card.
            Rect(v.EventsBadge.rectTransform,v.EventsButton.transform,148,8,53,53);
            Art(v.EventsBadge.transform,"SourceBadge","source_events_badge",0,0,53,53);
            v.EventsBadge.transform.SetAsLastSibling();
            v.EventsBadge.enabled=false;v.EventsBadgeText.enabled=false;
            Rect(layout.Footer, content, 0, 1454, 941, 218);
            Put(v.FooterBase,layout.Footer,"source_footer_carving",0,0,941,218);v.FooterBase.transform.SetAsFirstSibling();
            Control(v.MailButton,layout.Footer,"source_mail",0,5,184,177);
            Control(v.DailyRewardButton,layout.Footer,"source_gift",758,5,183,177);
            Put(v.QuestFrame,layout.Footer,"source_quest_ru",184,0,574,190);
            Text(v.QuestText,v.QuestFrame.transform,52,36,376,52,30);
            Text(v.QuestProgressText,v.QuestFrame.transform,58,107,347,38,28);
            Text(v.QuestRewardText,v.QuestFrame.transform,444,121,92,40,32);
            Rect(v.DailyRewardBadge.rectTransform,v.DailyRewardButton.transform,117,-4,53,55);
            Art(v.DailyRewardBadge.transform,"SourceBadge","source_daily_badge",0,0,53,55);
            v.DailyRewardBadge.transform.SetAsLastSibling();
            v.DailyRewardBadge.enabled=false;v.DailyRewardBadgeText.enabled=false;
            v.SourceRussianPlay=Load("source_play_ru");v.SourceBlankPlay=Load("source_play_blank");
            v.SourceRussianQuest=Load("source_quest_ru");v.SourceBlankQuest=Load("source_quest_blank");
            v.ReferenceBodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            v.ReferenceTitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.RankingButton=Shortcut(content,"RankingShortcut",ShortcutResources[0],490,out v.RankingText);
            v.TeamButton=Shortcut(content,"TeamShortcut",ShortcutResources[1],660,out v.TeamText);
            v.FriendsButton=Shortcut(content,"FriendsShortcut",ShortcutResources[2],830,out v.FriendsText);
            layout.SocialShortcuts = new[] { (RectTransform)v.RankingButton.transform,
                (RectTransform)v.TeamButton.transform, (RectTransform)v.FriendsButton.transform };
            ProductionHomeRuntime.ClearDormantArtwork(v);
            layout.SourceComposition=true;
            v.Revision=ProductionHomeView.CurrentRevision;
            Refresh(v);
        }
        public static bool Valid(ProductionHomeView v) => v.SourcePlay != null && v.SourcePlay.sprite != null &&
            v.SourceNavigation != null && v.SourceNavigation.Length == 4 && v.SourceNavigation.All(i=>i!=null && i.sprite!=null) &&
            v.SourceRussianCards != null && v.SourceRussianCards.Length == 4 && v.SourceRussianCards.All(i=>i!=null) &&
            v.SourceBlankCards != null && v.SourceBlankCards.Length == 4 && v.SourceBlankCards.All(i=>i!=null) &&
            v.SourceRussianPlay != null && v.SourceBlankPlay != null && v.SourceRussianQuest != null && v.SourceBlankQuest != null &&
            v.ReferenceBodyFont != null && v.ReferenceTitleFont != null && v.Environment.sprite.name=="clean_environment" &&
            v.Ivan.sprite.name=="source_hero" && v.Logo.sprite.name=="source_logo" && v.HudFrame.sprite.name=="source_header" &&
            v.GoldPlusButton != null && v.GemsPlusButton != null && v.EnergyPlusButton != null;
        public static void Refresh(ProductionHomeView v)
        {
            if(v.SourcePlay==null)return;
            bool en=DemoFeatureState.LanguageCode=="en";
            v.SourcePlay.sprite=en?v.SourceBlankPlay:v.SourceRussianPlay;
            v.PlayText.enabled=en;v.PlayText.font=v.ReferenceTitleFont;
            if(v.RankingText!=null&&v.TeamText!=null&&v.FriendsText!=null){v.RankingText.text=en?"RANKING":"РЕЙТИНГ";v.TeamText.text=en?"TEAM":"КОМАНДА";v.FriendsText.text=en?"FRIENDS":"ДРУЗЬЯ";
            foreach(var caption in new[]{v.RankingText,v.TeamText,v.FriendsText})caption.font=v.ReferenceBodyFont;}
            Text[] captions={v.WorldMapText,v.ShopText,v.EventsText,v.CharactersText};
            for(int i=0;i<4;i++){v.SourceNavigation[i].sprite=en?v.SourceBlankCards[i]:v.SourceRussianCards[i];captions[i].enabled=en;captions[i].font=v.ReferenceBodyFont;}
            v.QuestFrame.sprite=en?v.SourceBlankQuest:v.SourceRussianQuest;v.QuestText.enabled=en;
            foreach(var text in new[]{v.HeroName,v.LevelText,v.GoldText,v.GemsText,v.EnergyText,v.QuestText,v.QuestProgressText,v.QuestRewardText,v.ProfileProgressText})text.font=v.ReferenceBodyFont;
            v.EventsBadgeText.enabled=false;v.DailyRewardBadgeText.enabled=false;
        }
        public static void FitBalances(ProductionHomeView v)
        {
            if(v.SourcePlay==null)return;
            if(v.Header!=null)v.Header.RefreshValues();
        }
        static Button Shortcut(Transform parent, string name, string artKey, float y, out Text caption)
        {
            var found = parent.Find(name);
            var image = found != null ? found.GetComponent<Image>() :
                new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            Rect(image.rectTransform, parent, 34, y, 138, 165);
            image.gameObject.layer = 5;
            image.sprite = Resources.Load<Sprite>(artKey);
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            // Match the more restrained bronze/gold of Home Map and Shop without changing source pixels.
            image.color = new Color32(198, 190, 180, 255);
            image.enabled = image.raycastTarget = true;
            var button = image.GetComponent<Button>() ?? image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            if (!image.GetComponent<ProductionHomeButtonFeedback>())
                image.gameObject.AddComponent<ProductionHomeButtonFeedback>();
            // R5 used a separate pictogram over a shared frame. The reviewed R6 art owns
            // both surfaces, so retire that layer while preserving the Button identity.
            var oldIcon = image.transform.Find("Icon");
            if (oldIcon != null)
            {
                oldIcon.GetComponent<Image>().sprite = null;
                oldIcon.gameObject.SetActive(false);
            }
            var label = image.transform.Find("Label");
            caption = label != null ? label.GetComponent<Text>() :
                new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
            Text(caption, image.transform, 17, 123, 104, 24, 18);
            // Small captions should rasterize at their final size, not be halved after
            // rasterization. Use the same approved body face as the Home navigation.
            caption.rectTransform.sizeDelta = new Vector2(104, 24);
            caption.rectTransform.localScale = Vector3.one;
            caption.fontSize = 18;
            caption.horizontalOverflow = HorizontalWrapMode.Overflow;
            caption.gameObject.layer = 5;
            return button;
        }
        static Sprite Load(string name) => Resources.Load<Sprite>(ArtRoot+name) ?? throw new InvalidOperationException("Missing Home original source sprite: "+name);
        static void Control(Button button,Transform parent,string name,float x,float y,float w,float h)
        {Rect(button.transform as RectTransform,parent,x,y,w,h);ControlArt(button,name,0,0,w,h);}
        static Image ControlArt(Button button,string name,float x,float y,float w,float h)
        {var image=Art(button.transform,"OriginalSourceArtwork",name,x,y,w,h);image.raycastTarget=true;button.targetGraphic=image;return image;}
        static Image Art(Transform parent,string objectName,string name,float x,float y,float w,float h)
        {var found=parent.Find(objectName);var image=found!=null?found.GetComponent<Image>():new GameObject(objectName,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<Image>();Put(image,parent,name,x,y,w,h);return image;}
        static void Put(Image image,Transform parent,string name,float x,float y,float w,float h)
        {Rect(image.rectTransform,parent,x,y,w,h);image.sprite=Load(name);image.type=Image.Type.Simple;image.preserveAspect=true;image.color=Color.white;image.material=null;image.enabled=true;image.raycastTarget=false;image.gameObject.SetActive(true);}
        static void Text(Text text,Transform parent,float x,float y,float w,float h,int size,TextAnchor align=TextAnchor.MiddleCenter)
        {Rect(text.rectTransform,parent,x,y,w*2,h*2);text.rectTransform.localScale=new Vector3(.5f,.5f,1);text.transform.SetAsLastSibling();text.fontStyle=FontStyle.Normal;text.fontSize=size*2;text.alignment=align;text.color=new Color32(240,221,169,255);text.material=null;text.resizeTextForBestFit=false;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;text.enabled=true;text.gameObject.SetActive(true);text.raycastTarget=false;}
        static void Rect(RectTransform rect,Transform parent,float x,float y,float w,float h)
        {rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);rect.gameObject.SetActive(true);}
    }
}
