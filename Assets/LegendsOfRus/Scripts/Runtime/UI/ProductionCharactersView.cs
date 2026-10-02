using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public enum HeroPreviewFilter { All, Rare, Legendary }

    public sealed class ProductionCharactersView : MonoBehaviour
    {
        public const int CurrentRevision = 16;
        public int Revision = CurrentRevision;
        public RectTransform Content, Body, Hud, OuterFrame;
        public RectTransform CollectionViewport, CollectionContent;
        public ScrollRect CollectionScroll;
        public Image Environment, Title, DetailArt;
        public Font BodyFont, TitleFont;
        public ProductionPlayerHeader Header;
        public Sprite TitleRu, UpgradeRu, SelectRu;
        public Sprite[] TabSourceRu, TabAlternateRu, NavRu;
        public Image[] NavCaptions;
        public Text Heading, HeroName, LevelText, GoldText, GemsText, EnergyText;
        public Text CharacterName, Availability, Rarity, Description, SkillHeading, BonusNote;
        public Text UpgradeLabel, SelectLabel, CollectionLabel, InfoHeading, InfoDescription, CloseInfoLabel;
        public Text[] StatLabels, StatValues, TabLabels, CardNames, CardStatuses, NavLabels, FutureLabels;
        public Button ProfileButton, SettingsButton, BackButton, UpgradeButton, SelectButton, InfoButton, CloseInfoButton;
        public Button[] PlusButtons, Tabs, Cards, NavButtons;
        public Image[] CardFrames, CardSelections, DetailStars, CardStars;
        public Text BonusHeading;
        public Text[] AbilityLevels;
        public GameObject[] FutureCards;
        public GameObject InfoPanel;
        public HeroPreviewFilter Filter { get; private set; }
        public int SelectedHero { get; private set; }
        public bool ActionsBound { get; private set; }
        public bool HasHeroOwnershipService => false;
        public bool CanUpgrade => false;
        public int CurrentHero => 0;

        public static readonly string[] NamesRu = { "Иван", "Маша", "Жар-Птица", "Василиса", "Добрыня", "Баба-Яга", "Илья Муромец", "Кощей" };
        public static readonly string[] NamesEn = { "Ivan", "Masha", "Firebird", "Vasilisa", "Dobrynya", "Baba Yaga", "Ilya Muromets", "Koschei" };
        // Visual rarity groups from the source concept, not ownership or upgrade levels.
        public static bool IsLegendary(int id) => id == 0 || id >= 4;
        public bool HasRequiredBindings => Revision == CurrentRevision && TitleRu && UpgradeRu && SelectRu && Valid(TabSourceRu,3) && Valid(TabAlternateRu,3) && Valid(NavRu,4) && Valid(NavCaptions,4) && Header && Header.HasBindings && Content && Body && Hud && OuterFrame && CollectionViewport && CollectionContent && CollectionScroll &&
            Environment && Environment.sprite && Title && DetailArt && BodyFont && TitleFont && Heading &&
            HeroName && LevelText && GoldText && GemsText && EnergyText && CharacterName && Availability &&
            Rarity && Description && SkillHeading && BonusNote && UpgradeLabel && SelectLabel && CollectionLabel &&
            InfoHeading && InfoDescription && CloseInfoLabel && ProfileButton && SettingsButton && BackButton &&
            UpgradeButton && SelectButton && InfoButton && CloseInfoButton && InfoPanel &&
            Valid(PlusButtons,3) && Valid(Tabs,3) && Valid(TabLabels,3) && Valid(Cards,8) && Valid(CardFrames,8) &&
            Valid(CardNames,8) && Valid(CardStatuses,8) && Valid(NavButtons,4) && Valid(NavLabels,4) &&
            BonusHeading && Valid(CardSelections,8) && Valid(DetailStars,5) && Valid(CardStars,40) && Valid(AbilityLevels,4) && Valid(FutureCards,4) && Valid(FutureLabels,4) && Valid(StatLabels,4) && Valid(StatValues,4);
        static bool Valid<T>(T[] array,int length) where T : UnityEngine.Object => array != null && array.Length == length && Array.TrueForAll(array,x=>x != null);
        readonly List<Button> bound = new List<Button>();
        readonly List<UnityAction> listeners = new List<UnityAction>();
        GameState observed;
        string language;
        bool committed;

        void OnEnable() { Observe(); RefreshContent(); }
        void OnDisable() { if(observed != null) observed.Changed -= RefreshState; observed=null; Unbind(); }
        void Update() { if(observed != GameState.Instance) { Observe(); RefreshState(); } if(language != DemoFeatureState.LanguageCode) RefreshContent(); }
        void Observe() { if(observed != null) observed.Changed -= RefreshState; observed=GameState.Instance; if(isActiveAndEnabled && observed != null) observed.Changed += RefreshState; }
        void Unbind() { for(int i=0;i<bound.Count;i++) if(bound[i]) bound[i].onClick.RemoveListener(listeners[i]); bound.Clear(); listeners.Clear(); ActionsBound=false; }
        public bool CanAct => ActionsBound && !committed && isActiveAndEnabled && !InfoPanel.activeSelf &&
            (ScreenRouter.Instance == null || !ScreenRouter.Instance.IsScreenTransitioning && !ScreenRouter.Instance.HasActiveModalBlocker);
        void Listen(Button button,Action action,bool route=false,bool inInfo=false)
        {
            UnityAction listener=()=> {
                bool allowed=inInfo ? ActionsBound && !committed && isActiveAndEnabled && InfoPanel.activeSelf &&
                    (ScreenRouter.Instance==null || !ScreenRouter.Instance.IsScreenTransitioning && !ScreenRouter.Instance.HasActiveModalBlocker) : CanAct;
                if(!allowed || !button.IsInteractable()) return;
                if(route) committed=true;
                action?.Invoke();
            };
            button.onClick.AddListener(listener);bound.Add(button);listeners.Add(listener);
        }
        public void Bind(Action<ScreenId> show,Action back,Action<string> openShop=null)
        {
            Unbind(); committed=false;
            Listen(ProfileButton,()=>show(ScreenId.Profile),true); Listen(SettingsButton,()=>show(ScreenId.Settings),true); Listen(BackButton,back,true);
            for(int i=0;i<3;i++) { int index=i; Listen(PlusButtons[i],()=>{if(index==2)show(ScreenId.OutOfEnergy);else if(openShop!=null)openShop(index==0?"coins":"gems");else show(ScreenId.Shop);},true); Listen(Tabs[i],()=>SetFilter((HeroPreviewFilter)index)); }
            for(int i=0;i<8;i++) { int index=i; Listen(Cards[i],()=>PreviewHero(index)); }
            ScreenId[] targets={ScreenId.WorldMap,ScreenId.Shop,ScreenId.Events,ScreenId.TeamHub};
            for(int i=0;i<4;i++) { int index=i; Listen(NavButtons[i],()=>show(targets[index]),true); }
            Listen(InfoButton,ShowInfo); Listen(CloseInfoButton,CloseInfo,false,true);
            ActionsBound=true; RefreshContent();
        }
        public bool Includes(int id) => Filter==HeroPreviewFilter.All || (Filter==HeroPreviewFilter.Legendary)==IsLegendary(id);
        public void SetFilter(HeroPreviewFilter filter)
        {
            if(!CanAct || !Enum.IsDefined(typeof(HeroPreviewFilter),filter)) return;
            CollectionScroll.StopMovement();
            Filter=filter;
            if(!Includes(SelectedHero)) for(int i=0;i<8;i++) if(Includes(i)) { SelectedHero=i; break; }
            RefreshContent();
            CollectionScroll.verticalNormalizedPosition=1;
        }
        public void PreviewHero(int id) { if(!CanAct || id<0 || id>=8 || !Includes(id)) return; SelectedHero=id; RefreshContent(); }
        public void ShowInfo() { if(!CanAct) return; RefreshInfo(); InfoPanel.SetActive(true); }
        public void CloseInfo() { if(InfoPanel) InfoPanel.SetActive(false); }
        public bool TryDismiss() { if(!InfoPanel.activeSelf) return false; CloseInfo(); return true; }
        void RefreshInfo()
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            InfoHeading.text=en?"HERO COLLECTION":"КОЛЛЕКЦИЯ ГЕРОЕВ";
            InfoDescription.text=en?
                "Ivan is the current hero. Cards and rarity filters let you preview the characters.\n\nOwnership, combat stats, hero upgrades and team bonuses are not available yet. Browsing a card does not change your hero or spend resources.\n\nThe header shows your saved balances. Stars, emblems and ability illustrations are reference concepts, not earned ranks or unlocked skills.":
                "Иван — текущий герой. Карточки и фильтры редкости позволяют рассмотреть персонажей.\n\nВладение героями, боевые характеристики, улучшения и командные бонусы пока недоступны. Просмотр не меняет героя и не тратит ресурсы.\n\nВверху показаны ваши сохранённые балансы. Звёзды, эмблемы и изображения умений — примеры оформления, а не полученные ранги и способности.";
            CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings) return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";
            foreach(var text in GetComponentsInChildren<Text>(true)){if(text.transform.IsChildOf(Hud))continue;text.font=BodyFont;text.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;
            Title.sprite=en?ProductionCharactersRuntime.Load("title_r4"):TitleRu;Heading.enabled=en;
            UpgradeButton.image.sprite=en?ProductionCharactersRuntime.Load("upgrade_r4"):UpgradeRu;
            SelectButton.image.sprite=en?ProductionCharactersRuntime.Load("select_r4"):SelectRu;
            UpgradeLabel.enabled=SelectLabel.enabled=en;
            Heading.text=en?"CHARACTERS":"ПЕРСОНАЖИ";HeroName.text=en?"Ivan":"Иван";
            CharacterName.text=en?NamesEn[SelectedHero]:NamesRu[SelectedHero];
            Availability.text=en?"PREVIEW":"ПРИМЕР";
            Rarity.text=en?(IsLegendary(SelectedHero)?"LEGENDARY":"RARE"):(IsLegendary(SelectedHero)?"ЛЕГЕНДАРНЫЙ":"РЕДКИЙ");
            Description.text=en?"Level: —":"Уровень: —";
            for(int i=0;i<5;i++) DetailStars[i].gameObject.SetActive(i<(IsLegendary(SelectedHero)?5:4));
            for(int i=0;i<4;i++) AbilityLevels[i].text="—";
            string[] stats=en?new[]{"Strength","Defense","Magic","Health"}:new[]{"Сила","Защита","Магия","Здоровье"};
            for(int i=0;i<4;i++) { StatLabels[i].text=stats[i];StatValues[i].text="—"; }
            SkillHeading.text=en?"ABILITIES · PREVIEW":"УМЕНИЯ · ПРИМЕР";BonusHeading.text=en?"TEAM BONUS":"БОНУС КОМАНДЫ";BonusNote.text=en?"Not available yet":"Пока недоступно";
            UpgradeLabel.text=en?"UPGRADE":"УЛУЧШИТЬ";SelectLabel.text=SelectedHero==0?(en?"CURRENT":"ТЕКУЩИЙ"):(en?"UNAVAILABLE":"НЕДОСТУПНО");UpgradeButton.interactable=SelectButton.interactable=false;
            UpgradeButton.targetGraphic.color=SelectButton.targetGraphic.color=new Color(.62f,.62f,.62f,1);
            UpgradeLabel.color=SelectLabel.color=new Color32(151,128,85,255);
            CollectionLabel.text=en?"COLLECTION PREVIEW · INFO":"ПРИМЕРЫ ГЕРОЕВ · ПОДРОБНЕЕ";
            string[] tabs=en?new[]{"ALL","RARE","LEGENDARY"}:new[]{"ВСЕ","РЕДКИЕ","ЛЕГЕНДАРНЫЕ"};
            for(int i=0;i<3;i++) { TabLabels[i].text=tabs[i];bool sourceState=(i==0)==(i==(int)Filter);TabLabels[i].enabled=en;
                Tabs[i].image.type=en?Image.Type.Sliced:Image.Type.Simple;
                Tabs[i].image.sprite=en?ProductionCharactersRuntime.Shared(i==(int)Filter?"tab_active":"tab"):(sourceState?TabSourceRu[i]:TabAlternateRu[i]); }
            int slot=0;
            for(int i=0;i<8;i++) {
                bool visible=Includes(i);Cards[i].gameObject.SetActive(visible);if(!visible)continue;
                var rect=(RectTransform)Cards[i].transform;rect.anchoredPosition=new Vector2(10+slot%4*220,-(5+slot/4*244));slot++;
                CardNames[i].text=en?NamesEn[i]:NamesRu[i];CardStatuses[i].text=i==0?(en?"CURRENT HERO":"ТЕКУЩИЙ ГЕРОЙ"):(en?"PREVIEW":"ПРЕДПРОСМОТР");
                var cardFrame=ProductionCharactersRuntime.Load(i==CurrentHero?"card_border_r2":"card_normal_r6");
                Cards[i].image.sprite=CardFrames[i].sprite=cardFrame;
                CardFrames[i].color=Color.white;
                CardSelections[i].gameObject.SetActive(i==CurrentHero);
                for(int j=0;j<5;j++)CardStars[i*5+j].gameObject.SetActive(j<(IsLegendary(i)?5:4));
            }
            for(int i=0;i<4;i++) { FutureCards[i].SetActive(Filter==HeroPreviewFilter.All);FutureLabels[i].text=en?"COMING LATER":"СКОРО"; }
            UpdateCollectionExtent();
            DetailArt.sprite=SelectedHero==0?ProductionCharactersRuntime.Load("hero_firebird_r7"):ProductionCharactersRuntime.Load("portrait_"+SelectedHero);
            float viewportHeight=((RectTransform)DetailArt.transform.parent).rect.height;
            if(SelectedHero==0) {
                float height=viewportHeight+15;float width=height*DetailArt.sprite.rect.width/DetailArt.sprite.rect.height;
                DetailArt.rectTransform.sizeDelta=new Vector2(width,height);
                DetailArt.rectTransform.anchoredPosition=new Vector2((411-width)*.5f,7);
            }
            else {
                // Account for preserveAspect centering and the irregular cropped base of the isolated bust.
                // Let the frame cover that base instead of exposing a straight source-image edge.
                DetailArt.rectTransform.sizeDelta=new Vector2(299,290);
                Vector2 spriteSize=DetailArt.sprite.rect.size;
                float drawnHeight=spriteSize.y*Mathf.Min(299/spriteSize.x,290/spriteSize.y);
                DetailArt.rectTransform.anchoredPosition=new Vector2(56,-(viewportHeight+22-(290+drawnHeight)*.5f));
            }
            string[] nav=en?new[]{"WORLD MAP","SHOP","EVENTS","TEAM"}:new[]{"КАРТА МИРА","МАГАЗИН","СОБЫТИЯ","ГИЛЬДИЯ"};for(int i=0;i<4;i++){NavLabels[i].text=nav[i];NavLabels[i].enabled=en;NavCaptions[i].sprite=en?ProductionCharactersRuntime.Load("nav_caption_r8"):NavRu[i];}
            RefreshInfo();RefreshState();
        }
        public void UpdateCollectionExtent()
        {
            if(!CollectionViewport||!CollectionContent||Cards==null)return;
            int visible=0;foreach(var card in Cards)if(card&&card.gameObject.activeSelf)visible++;
            int rows=Mathf.CeilToInt(visible/4f)+(Filter==HeroPreviewFilter.All?1:0);
            float height=Mathf.Max(CollectionViewport.rect.height,10+rows*244);
            CollectionContent.sizeDelta=new Vector2(905,height);
            var position=CollectionContent.anchoredPosition;
            position.y=Mathf.Clamp(position.y,0,height-CollectionViewport.rect.height);
            CollectionContent.anchoredPosition=position;
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;Header.RefreshValues();
        }
    }
}
