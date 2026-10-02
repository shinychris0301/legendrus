using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Shared Home/World Map header presentation. Page views retain route and state subscriptions.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionPlayerHeader : MonoBehaviour
    {
        public Image Backing, Avatar;
        public Button Profile, Settings;
        public Text PlayerName, Level, Experience;
        public Image[] Capsules;
        public Text[] Balances;
        public Button[] Plus;
        public bool HasBindings => Backing && Avatar && Profile && Settings && PlayerName && Level && Experience &&
            Capsules!=null && Capsules.Length==3 && Array.TrueForAll(Capsules,x=>x!=null) &&
            Balances!=null && Balances.Length==3 && Array.TrueForAll(Balances,x=>x!=null) &&
            Plus!=null && Plus.Length==3 && Array.TrueForAll(Plus,x=>x!=null);

        public static void Attach(ProductionHomeView view)
        {
            var hud=view.GetComponent<ProductionHomeLayout>().Hud;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=view.HudFrame; h.Avatar=view.AvatarFrame;
            h.Profile=view.ProfileButton; h.Settings=view.SettingsButton;
            h.PlayerName=view.HeroName; h.Level=view.LevelText; h.Experience=view.ProfileProgressText;
            h.Capsules=new[]{view.GoldCapsule,view.GemsCapsule,view.EnergyCapsule};
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};
            h.Plus=new[]{view.GoldPlusButton,view.GemsPlusButton,view.EnergyPlusButton};
            view.Header=h; h.ApplyPresentation();
        }
        public static void Attach(ProductionWorldMapView view)
        {
            var hud=view.Hud;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");
            h.Profile=view.ProfileButton ? view.ProfileButton : Need<Button>(hud,"Profile");
            h.Avatar=Need<Image>(h.Profile.transform,"Avatar");
            h.PlayerName=view.HeroName ? view.HeroName : Need<Text>(h.Profile.transform,"HeroName");
            h.Level=view.LevelText ? view.LevelText : Need<Text>(h.Profile.transform,"Level");
            h.Experience=Need<Text>(h.Profile.transform,"UnavailableXP");
            h.Settings=view.SettingsButton ? view.SettingsButton : Need<Button>(hud,"Settings");
            h.Capsules=new Image[3];h.Balances=new Text[3];h.Plus=new Button[3];
            string[] keys={"gold","gems","energy"};
            for(int i=0;i<3;i++)
            {
                h.Capsules[i]=Need<Image>(hud,keys[i]);
                h.Balances[i]=Need<Text>(h.Capsules[i].transform,"Value");
                h.Plus[i]=Need<Button>(h.Capsules[i].transform,"Plus");
            }
            view.Header=h;view.ProfileButton=h.Profile;view.SettingsButton=h.Settings;
            view.HeroName=h.PlayerName;view.LevelText=h.Level;
            view.GoldText=h.Balances[0];view.GemsText=h.Balances[1];view.EnergyText=h.Balances[2];view.PlusButtons=h.Plus;
            h.ApplyPresentation();
        }
        public static void Attach(ProductionChapterMapView view)
        {
            var hud=view.Hud;
            foreach(var graphic in hud.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in hud.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");
            h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=view.ProfileButton.targetGraphic as Image;
            if(h.Avatar==null || h.Avatar.transform==h.Profile.transform)h.Avatar=Need<Image>(h.Profile.transform,"Avatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;
            h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            h.Capsules=new Image[3];
            for(int i=0;i<3;i++)h.Capsules[i]=h.Balances[i].transform.parent.GetComponent<Image>() ?? Need<Image>(h.Balances[i].transform.parent,"SharedResourceSurface");
            view.Header=h;h.ApplyPresentation();
        }
        public static void Attach(ProductionPreLevelView view)
        {
            var hud=view.Hud;
            foreach(var graphic in hud.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in hud.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");
            h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=view.ProfileButton.targetGraphic as Image;
            if(h.Avatar==null || h.Avatar.transform==h.Profile.transform)h.Avatar=Need<Image>(h.Profile.transform,"Avatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;
            h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            h.Capsules=new Image[3];
            for(int i=0;i<3;i++)h.Capsules[i]=h.Balances[i].transform.parent.GetComponent<Image>() ?? Need<Image>(h.Balances[i].transform.parent,"SharedResourceSurface");
            view.Header=h;h.ApplyPresentation();
        }
        public static void Attach(ProductionFailView view)
        {
            var hud=view.Hud;
            foreach(var graphic in hud.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in hud.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            view.HudBacking.gameObject.SetActive(false);
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");h.Profile=view.Profile;h.Settings=view.Settings;
            h.Avatar=view.Profile.targetGraphic as Image;
            if(h.Avatar==null || h.Avatar.transform==h.Profile.transform)h.Avatar=Need<Image>(h.Profile.transform,"SharedAvatar");
            h.PlayerName=view.HeroName;h.Level=view.Level;
            h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=view.Resources;h.Plus=view.Plus;h.Capsules=new Image[3];
            for(int i=0;i<3;i++)h.Capsules[i]=h.Balances[i].transform.parent.GetComponent<Image>() ?? Need<Image>(h.Balances[i].transform.parent,"SharedResourceSurface");
            view.Header=h;h.ApplyPresentation();
        }
        public static void Attach(ProductionShopView view)
        {
            if(view.Hud==null)view.Hud=Need<RectTransform>(view.Body,"SharedPlayerHeader");
            var hud=view.Hud;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            var old=view.Body.Find("NativeHudBacking");if(old!=null)old.gameObject.SetActive(false);
            foreach(Transform child in view.ProfileButton.transform)child.gameObject.SetActive(false);
            h.Backing=Need<Image>(hud,"ReferenceArtwork");h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=Need<Image>(h.Profile.transform,"SharedAvatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            var capsules=h.Capsules;h.Capsules=new Image[3];
            for(int i=0;i<3;i++)
            {
                h.Capsules[i]=capsules!=null && capsules.Length==3 && capsules[i]!=null ? capsules[i] : view.Body.Find("ResourceCapsule_"+i)?.GetComponent<Image>() ?? Need<Image>(hud,"SharedResource_"+i);
                var icon=view.Body.Find("ResourceIcon_"+i);if(icon!=null)icon.gameObject.SetActive(false);
            }
            view.Header=h;h.ApplyPresentation();
            if(hud.GetSiblingIndex()>view.Title.transform.GetSiblingIndex())hud.SetSiblingIndex(view.Title.transform.GetSiblingIndex());
        }
        public static void Attach(ProductionDailyRewardView view)
        {
            if(view.Hud==null)view.Hud=Need<RectTransform>(view.Body,"SharedPlayerHeader");
            var hud=view.Hud;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            var old=view.Body.Find("Hud");if(old!=null)old.gameObject.SetActive(false);
            foreach(Transform child in view.ProfileButton.transform)child.gameObject.SetActive(false);
            h.Backing=Need<Image>(hud,"ReferenceArtwork");h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=Need<Image>(h.Profile.transform,"SharedAvatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            var capsules=h.Capsules;h.Capsules=new Image[3];
            for(int i=0;i<3;i++)
            {
                h.Capsules[i]=capsules!=null && capsules.Length==3 && capsules[i]!=null ? capsules[i] : view.Body.Find("ResourceCapsule_"+i)?.GetComponent<Image>() ?? Need<Image>(hud,"SharedResource_"+i);
                var icon=view.Body.Find("ResourceIcon_"+i);if(icon!=null)icon.gameObject.SetActive(false);
            }
            view.PlayerHeader=h;h.ApplyPresentation();
            if(hud.GetSiblingIndex()>view.Title.transform.GetSiblingIndex())hud.SetSiblingIndex(view.Title.transform.GetSiblingIndex());
        }
        public static void Attach(ProductionEventsView view)
        {
            var hud=view.Hud;
            foreach(var graphic in hud.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in hud.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=Need<Image>(h.Profile.transform,"SharedAvatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            var capsules=h.Capsules;h.Capsules=new Image[3];
            for(int i=0;i<3;i++)
                h.Capsules[i]=capsules!=null && capsules.Length==3 && capsules[i]!=null ? capsules[i] : hud.Find("ResourceCapsule_"+i)?.GetComponent<Image>() ?? Need<Image>(hud,"SharedResource_"+i);
            view.Header=h;h.ApplyPresentation();
        }
        public static void Attach(ProductionEventDetailView view)
        {
            var hud=view.Hud;
            foreach(var graphic in hud.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in hud.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            h.Backing=Need<Image>(hud,"ReferenceArtwork");h.Profile=view.ProfileButton;h.Settings=view.SettingsButton;
            h.Avatar=Need<Image>(h.Profile.transform,"SharedAvatar");
            h.PlayerName=view.HeroName;h.Level=view.LevelText;h.Experience=Need<Text>(h.Profile.transform,"SharedExperience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};h.Plus=view.PlusButtons;
            var capsules=h.Capsules;h.Capsules=new Image[3];
            for(int i=0;i<3;i++)
                h.Capsules[i]=capsules!=null && capsules.Length==3 && capsules[i]!=null ? capsules[i] : hud.Find("ResourceCapsule_"+i)?.GetComponent<Image>() ?? Need<Image>(hud,"SharedResource_"+i);
            view.Header=h;h.ApplyPresentation();
        }
        public static void Attach(ProductionSpecialOfferView view)
        {
            if(view.Hud==null)view.Hud=Need<RectTransform>(view.Body,"SharedPlayerHeader");
            var hud=view.Hud;
            var h=hud.GetComponent<ProductionPlayerHeader>() ?? hud.gameObject.AddComponent<ProductionPlayerHeader>();
            var old=view.Body.Find("NativeHudBacking");if(old!=null)old.gameObject.SetActive(false);
            h.Backing=Need<Image>(hud,"ReferenceArtwork");
            h.Profile=Need<Button>(hud,"BackgroundProfile");h.Settings=Need<Button>(hud,"BackgroundSettings");
            h.Avatar=Need<Image>(h.Profile.transform,"Avatar");
            h.PlayerName=Need<Text>(h.Profile.transform,"PlayerName");h.Level=Need<Text>(h.Profile.transform,"Level");h.Experience=Need<Text>(h.Profile.transform,"Experience");
            h.Balances=new[]{view.GoldText,view.GemsText,view.EnergyText};
            var capsules=h.Capsules;h.Capsules=new Image[3];h.Plus=new Button[3];
            for(int i=0;i<3;i++)
            {
                h.Capsules[i]=capsules!=null && capsules.Length==3 && capsules[i]!=null ? capsules[i] : view.Body.Find("ResourceCapsule_"+i)?.GetComponent<Image>() ?? Need<Image>(hud,"Resource_"+i);
                h.Plus[i]=Need<Button>(h.Capsules[i].transform,"Plus");
                var icon=view.Body.Find("ResourceIcon_"+i);if(icon!=null)icon.gameObject.SetActive(false);
            }
            view.PlayerHeader=h;h.ApplyPresentation();
            h.Profile.GetComponent<Image>().sprite=h.Avatar.sprite;
            var group=hud.GetComponent<CanvasGroup>();if(group==null)group=hud.gameObject.AddComponent<CanvasGroup>();
            group.alpha=.4f;group.interactable=false;group.blocksRaycasts=false;
            // The full shared resource header stays visible behind this noninteractive backdrop.
            if(hud.GetSiblingIndex()>view.OfferCard.transform.GetSiblingIndex())hud.SetSiblingIndex(view.OfferCard.transform.GetSiblingIndex());
        }
        public void ApplyPresentation()
        {
            Place((RectTransform)transform,transform.parent,0,0,941,140);
            Art(Backing,transform,"source_header",0,0,941,140);Backing.transform.SetAsFirstSibling();
            Place((RectTransform)Profile.transform,transform,0,0,337,140);
            // A transparent native hit area includes the name and the portrait's hanging crest.
            Graphic hit=Profile.GetComponent<Graphic>() ?? Profile.gameObject.AddComponent<Image>();
            hit.enabled=true;hit.color=Color.clear;hit.raycastTarget=true;
            Art(Avatar,Profile.transform,"source_avatar",0,0,174,235);Avatar.raycastTarget=true;
            Profile.targetGraphic=Avatar;Profile.transition=Selectable.Transition.None;
            Label(PlayerName,Profile.transform,180,30,150,44,34,TextAnchor.MiddleLeft);
            Label(Level,Profile.transform,180,75,150,29,24,TextAnchor.MiddleLeft);
            Label(Experience,Profile.transform,194,107,126,20,17);
            string[] keys={"gold","gems","energy"};float[] xs={344,529,702},widths={180,165,149},tx={60,58,46},tw={72,62,59};
            for(int i=0;i<3;i++)
            {
                Art(Capsules[i],transform,"source_"+keys[i],xs[i],39,widths[i],61);
                Label(Balances[i],Capsules[i].transform,tx[i],13,tw[i],35,26);
                Balances[i].horizontalOverflow=HorizontalWrapMode.Overflow;
                Control(Plus[i],Capsules[i].transform,"source_plus",widths[i]-47,8,44,47);
            }
            Control(Settings,transform,"source_settings",853,23,88,95);
            RefreshValues();
        }
        public void RefreshValues()
        {
            if(!HasBindings)return;
            bool en=DemoFeatureState.LanguageCode=="en";
            var save=GameState.Instance!=null?GameState.Instance.Save:null;
            PlayerName.text=en?"Ivan":"Иван";
            Level.text=(en?"Level ":"Уровень ")+(save!=null?save.level.ToString(CultureInfo.InvariantCulture):"—");
            Experience.text="—";
            Balances[0].text=save!=null?Amount(save.gold):"—";
            Balances[1].text=save!=null?Amount(save.gems):"—";
            Balances[2].text=save!=null?save.energy.ToString(CultureInfo.InvariantCulture)+"/"+save.energyMax.ToString(CultureInfo.InvariantCulture):"—";
            foreach(var label in Balances)
            {
                label.resizeTextForBestFit=false;label.fontSize=52;
                float width=label.preferredWidth,available=Mathf.Max(1,label.rectTransform.rect.width-28);
                if(width>available)label.fontSize=Mathf.Clamp(Mathf.FloorToInt(52*available/width),10,52);
            }
        }
        static string Amount(int value)=>value.ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
        static T Need<T>(Transform parent,string name) where T:Component
        {
            var child=parent.Find(name);
            var go=child!=null?child.gameObject:new GameObject(name,typeof(RectTransform));
            if(child==null)go.transform.SetParent(parent,false);
            return go.GetComponent<T>() ?? go.AddComponent<T>();
        }
        static void Place(RectTransform rect,Transform parent,float x,float y,float width,float height)
        {
            rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
            rect.localRotation=Quaternion.identity;rect.localScale=Vector3.one;
            rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(width,height);rect.gameObject.SetActive(true);
        }
        static void Art(Image image,Transform parent,string key,float x,float y,float w,float h)
        {
            Place(image.rectTransform,parent,x,y,w,h);
            image.sprite=Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+key) ?? throw new InvalidOperationException("Missing shared header sprite: "+key);
            image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=true;image.material=null;image.enabled=true;image.raycastTarget=false;
        }
        static void Control(Button button,Transform parent,string key,float x,float y,float w,float h)
        {
            Place((RectTransform)button.transform,parent,x,y,w,h);
            var image=button.targetGraphic as Image;
            if(image==null)image=Need<Image>(button.transform,"OriginalSourceArtwork");
            if(image.transform==button.transform)Art(image,parent,key,x,y,w,h);
            else Art(image,button.transform,key,0,0,w,h);
            image.raycastTarget=true;button.targetGraphic=image;button.transition=Selectable.Transition.None;
            button.navigation=new Navigation{mode=Navigation.Mode.None};
        }
        static void Label(Text text,Transform parent,float x,float y,float w,float h,int size,TextAnchor align=TextAnchor.MiddleCenter)
        {
            Place(text.rectTransform,parent,x,y,w*2,h*2);text.rectTransform.localScale=new Vector3(.5f,.5f,1);
            text.transform.SetAsLastSibling();text.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            text.fontStyle=FontStyle.Normal;text.fontSize=size*2;text.color=new Color32(240,221,169,255);text.alignment=align;
            text.material=null;text.resizeTextForBestFit=false;text.horizontalOverflow=HorizontalWrapMode.Wrap;
            text.verticalOverflow=VerticalWrapMode.Truncate;text.enabled=true;text.raycastTarget=false;
        }
    }
}
