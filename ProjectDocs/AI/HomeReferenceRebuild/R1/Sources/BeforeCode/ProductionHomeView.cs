using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Home's live labels and visible controls; existing state and router own game behavior.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionHomeView : MonoBehaviour
    {
        public const int CurrentRevision = 15;
        public int Revision = CurrentRevision;
        public SplashTitleTypography Typography;
        public Image Environment;
        public ProductionHomeBackdropShade BackgroundShade;
        public Image MapFoliage;
        public Image Logo;
        public Image Ivan;
        public Image Firebird;
        public ProductionHomeShade LowerShade;
        public Image HudFrame;
        public Image AvatarFrame;
        public Image GoldCapsule;
        public Image GemsCapsule;
        public Image EnergyCapsule;
        public Image GoldIcon;
        public Image GemsIcon;
        public Image EnergyIcon;
        public Image QuestFrame;
        public Image FooterBase;
        public Image ProfileProgressFrame;
        public Text ProfileProgressText;
        public Image QuestProgressFrame;
        public Image QuestProgressFill;
        public Image QuestRewardFrame;
        public Image QuestRewardIcon;
        public ProductionHomePanel DailyRewardBadge;
        public ProductionHomePanel EventsBadge;
        public Text EventsBadgeText;
        public Text QuestProgressText;
        public Text QuestRewardText;
        public Text DailyRewardBadgeText;
        public Button GoldPlusButton;
        public Button GemsPlusButton;
        public Button EnergyPlusButton;
        // The project has no persisted blue-gem quest or XP service. Empty meters and
        // unavailable values are honest until a real service supplies those measurements.
        public bool HasQuestMeasurement => false;

        public Text HeroName;
        public Text LevelText;
        public Text GoldText;
        public Text GemsText;
        public Text EnergyText;
        public Text PlayText;
        public Text WorldMapText;
        public Text ShopText;
        public Text EventsText;
        public Text CharactersText;
        public Text QuestText;

        public Button ProfileButton;
        public Button SettingsButton;
        public Button PlayButton;
        public Button WorldMapButton;
        public Button ShopButton;
        public Button EventsButton;
        public Button CharactersButton;
        public Button MailButton;
        public Button DailyRewardButton;

        public bool ActionsBound { get; private set; }
        public string ActiveFontSource { get; private set; }
        public bool HasRequiredBindings => Revision == CurrentRevision && Typography != null &&
            Environment != null && Environment.sprite != null && Logo != null && Logo.sprite != null &&
            Ivan != null && Ivan.sprite != null && Firebird != null && !Firebird.gameObject.activeSelf && Firebird.sprite == null &&
            Environment.color == ProductionHomeRuntime.EnvironmentTone && Ivan.color == ProductionHomeRuntime.HeroTone &&
            LowerShade != null && LowerShade.GetComponent<CanvasRenderer>() != null &&
            HeroName != null && LevelText != null && GoldText != null && GemsText != null &&
            EnergyText != null && PlayText != null && WorldMapText != null && ShopText != null &&
            EventsText != null && CharactersText != null && QuestText != null &&
            ProfileButton != null && SettingsButton != null && PlayButton != null &&
            WorldMapButton != null && ShopButton != null && EventsButton != null &&
            CharactersButton != null && MailButton != null && DailyRewardButton != null &&
            HasGeneratedControlBindings && HasReferenceForegroundBindings && HasReferenceBackdropBindings && HasSceneLightingBindings;

        public bool HasSceneLightingBindings => HasSceneLighting(Ivan, "home_hero_lighting", 0) &&
            HasSceneLighting(MapFoliage, "home_foliage_lighting", 1);

        static bool HasSceneLighting(Image image, string materialName, int profile)
        {
            if (image == null || image.material == null || image.material.name != materialName ||
                image.material.shader.name != ProductionHomeRuntime.SceneLightingShader ||
                image.material.GetFloat("_Profile") != profile) return false;
            ProductionHomeSceneLighting lighting = image.GetComponent<ProductionHomeSceneLighting>();
            return lighting != null && lighting.enabled && lighting.Strength == 1f && lighting.Padding == .03f;
        }

        public bool HasReferenceBackdropBindings =>
            Environment != null && Ivan != null && Environment.transform.parent == Ivan.transform.parent &&
            Environment.transform.GetSiblingIndex() < Ivan.transform.GetSiblingIndex() &&
            BackgroundShade != null && BackgroundShade.enabled && BackgroundShade.gameObject.activeSelf &&
            BackgroundShade.GetComponent<CanvasRenderer>() != null && !BackgroundShade.raycastTarget &&
            BackgroundShade.transform.parent == Environment.transform &&
            BackgroundShade.rectTransform.anchorMin == Vector2.zero && BackgroundShade.rectTransform.anchorMax == Vector2.one &&
            BackgroundShade.rectTransform.offsetMin == Vector2.zero && BackgroundShade.rectTransform.offsetMax == Vector2.zero &&
            HasArtwork(MapFoliage, "home_map_foliage") && MapFoliage.enabled && MapFoliage.gameObject.activeSelf && !MapFoliage.raycastTarget &&
            WorldMapButton != null && MapFoliage.transform.parent == WorldMapButton.transform.parent &&
            MapFoliage.transform.parent != null && MapFoliage.transform.parent.parent == Ivan.transform.parent &&
            MapFoliage.transform.parent.GetSiblingIndex() > Ivan.transform.GetSiblingIndex() &&
            MapFoliage.transform.GetSiblingIndex() == 0;

        public bool HasGeneratedControlBindings =>
            HasArtwork(HudFrame, "header") && HasArtwork(AvatarFrame, "avatar_ring") &&
            ProfileButton != null && ProfileButton.targetGraphic == AvatarFrame &&
            HasArtwork(GoldCapsule, "capsule") && HasArtwork(GemsCapsule, "capsule") &&
            HasArtwork(EnergyCapsule, "capsule") && HasArtwork(GoldIcon, "coin") &&
            HasArtwork(GemsIcon, "gem") && HasArtwork(EnergyIcon, "energy") &&
            HasArtwork(QuestFrame, "quest_panel") && HasArtwork(FooterBase, "footer_base_generated") &&
            !FooterBase.raycastTarget && HasButtonArtwork(SettingsButton, "settings") &&
            HasButtonArtwork(WorldMapButton, "nav_map_generated") && HasButtonArtwork(ShopButton, "nav_shop_generated") &&
            HasButtonArtwork(EventsButton, "nav_events_generated") && HasButtonArtwork(CharactersButton, "nav_characters_generated") &&
            HasButtonArtwork(MailButton, "mail_medallion_generated") && HasButtonArtwork(DailyRewardButton, "reward_medallion_generated");

        public bool HasReferenceForegroundBindings =>
            HasArtwork(Logo, "home_logo") && HasArtwork(Ivan, "home_hero_firebird") &&
            HasButtonArtwork(PlayButton, "home_play_button") &&
            HasReferencePortrait && Ivan.GetComponent<SplashTitleCharacterFade>() != null &&
            Ivan.GetComponent<SplashTitleCharacterFade>().enabled &&
            PlayText != null && PlayText.GetComponent<SplashTitleTextGeometry>() != null &&
            PlayText.GetComponent<ProductionHomeGoldText>() != null && PlayText.GetComponent<ProductionHomeGoldText>().enabled &&
            PlayText.GetComponent<ProductionHomeGoldText>().BeveledFace &&
            PlayText.material != null && PlayText.material.shader.name == "LegendsOfRus/UI/HomePlayGold" &&
            HasArtwork(ProfileProgressFrame, "progress_frame") && ProfileProgressText != null &&
            HasArtwork(QuestProgressFrame, "progress_frame") && HasArtwork(QuestProgressFill, "progress_fill") &&
            HasArtwork(QuestRewardFrame, "quest_reward_frame") && HasArtwork(QuestRewardIcon, "quest_reward_gem") &&
            HasButtonArtwork(GoldPlusButton, "plus") && HasButtonArtwork(GemsPlusButton, "plus") &&
            HasButtonArtwork(EnergyPlusButton, "plus") && QuestProgressText != null &&
            QuestRewardText != null && DailyRewardBadge != null && DailyRewardBadgeText != null &&
            EventsBadge != null && EventsBadgeText != null && !EventsBadge.raycastTarget &&
            EventsBadge.RaisedBadge && DailyRewardBadge.RaisedBadge && !DailyRewardBadge.raycastTarget;

        bool HasReferencePortrait => ProfileButton != null &&
            ProfileButton.transform.Find("PortraitClip/ApprovedIvanPortrait") != null &&
            HasArtwork(ProfileButton.transform.Find("PortraitClip/ApprovedIvanPortrait").GetComponent<Image>(), "avatar_portrait");

        static bool HasArtwork(Image image, string expectedName) =>
            image != null && image.sprite != null && image.sprite.name == expectedName;

        static bool HasButtonArtwork(Button button, string expectedName) =>
            button != null && HasArtwork(button.targetGraphic as Image, expectedName);

        readonly Button[] boundButtons = new Button[12];
        readonly UnityAction[] ownedListeners = new UnityAction[12];
        GameState subscribedState;
        string lastLanguage;
        string lastRewardDay;
        bool refreshingState;
        bool navigationCommitted;

        void OnEnable()
        {
            navigationCommitted = false;
            EnsureStateBinding();
            RefreshContent();
        }

        void Update()
        {
            // The established language setting has no event. Match Splash/Loading's change check.
            if (lastLanguage != DemoFeatureState.LanguageCode) RefreshContent();
            if (EnsureStateBinding() || lastRewardDay != DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture)) RefreshState();
        }

        void OnDisable()
        {
            UnbindNavigation();
            UnbindState();
        }

        bool EnsureStateBinding()
        {
            GameState state = GameState.Instance;
            if (!isActiveAndEnabled || subscribedState == state) return false;
            UnbindState();
            subscribedState = state;
            if (subscribedState != null) subscribedState.Changed += RefreshState;
            return true;
        }

        void UnbindState()
        {
            if (subscribedState != null) subscribedState.Changed -= RefreshState;
            subscribedState = null;
        }

        public void RefreshContent()
        {
            ProductionNavigationStyle.Apply(this);
            lastLanguage = DemoFeatureState.LanguageCode;
            bool en = lastLanguage == "en";
            SetText(HeroName, en ? "Ivan" : "Иван");
            SetText(PlayText, en ? "PLAY" : "ИГРАТЬ");
            SetText(WorldMapText, en ? "WORLD MAP" : "КАРТА МИРА");
            SetText(ShopText, en ? "SHOP" : "МАГАЗИН");
            SetText(EventsText, en ? "EVENTS" : "СОБЫТИЯ");
            SetText(EventsBadgeText, "!"); // Fixed reference navigation decoration, not an event-state notification.
            SetText(CharactersText, en ? "CHARACTERS" : "ПЕРСОНАЖИ");
            // Existing copy only: no quest service currently supplies a goal, measurement or reward.
            SetText(QuestText, en ? "Collect blue gems" : "Соберите синие самоцветы");

            if (Typography != null)
            {
                string source;
                Font regular = Typography.Resolve(false, out source);
                ActiveFontSource = source;
                Text[] labels = { HeroName, LevelText, GoldText, GemsText, EnergyText,
                    WorldMapText, ShopText, EventsText, CharactersText, QuestText, QuestProgressText, QuestRewardText, DailyRewardBadgeText, ProfileProgressText, EventsBadgeText };
                foreach (Text label in labels)
                {
                    if (label == null) continue;
                    label.font = regular;
                    label.raycastTarget = false;
                }
                if (PlayText != null)
                {
                    PlayText.font = Typography.Resolve(true, out source);
                    PlayText.raycastTarget = false;
                }
            }
            RefreshState();
        }

        public void RefreshState()
        {
            // RefreshEnergyFromClock may persist and raise Changed synchronously.
            if (refreshingState) return;
            refreshingState = true;
            try
            {
                EnsureStateBinding();
                GameState state = GameState.Instance;
                if (state != null && state.Save != null && Application.isPlaying)
                    state.RefreshEnergyFromClock();
                PlayerSaveData save = state != null ? state.Save : null;
                SetText(GoldText, save != null ? FormatAmount(save.gold) : "—");
                SetText(GemsText, save != null ? FormatAmount(save.gems) : "—");
                SetText(EnergyText, save != null
                    ? save.energy.ToString(CultureInfo.InvariantCulture) + "/" +
                        save.energyMax.ToString(CultureInfo.InvariantCulture)
                    : "—");
                string level = save != null ? save.level.ToString(CultureInfo.InvariantCulture) : "—";
                SetText(LevelText, (DemoFeatureState.LanguageCode == "en" ? "Level " : "Уровень ") + level);
                SetText(ProfileProgressText, "—");
                SetText(QuestProgressText, "— / —");
                SetText(QuestRewardText, "—");
                if (QuestProgressFill != null)
                {
                    QuestProgressFill.fillAmount = 0;
                    QuestProgressFill.gameObject.SetActive(false);
                }
                // This mirrors GameState.ClaimDailyReward's actual persisted UTC rule.
                lastRewardDay = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                bool dailyAvailable = DailyRewardSchedule.CanClaim(save, DateTime.UtcNow);
                if (DailyRewardBadge != null) DailyRewardBadge.gameObject.SetActive(dailyAvailable);
                SetText(DailyRewardBadgeText, dailyAvailable ? "1" : string.Empty);
            }
            finally { refreshingState = false; }
        }

        static string FormatAmount(int value)
        {
            // Stable grouping on both Russian and English devices, matching the existing HUD.
            return value.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
        }

        static void SetText(Text label, string value)
        {
            if (label == null) return;
            label.text = value;
            label.raycastTarget = false;
        }

        public void BindNavigation(Action<ScreenId> show, Action openChapterMap)
        {
            UnbindNavigation();
            Button[] buttons = Buttons();
            ScreenId[] destinations = { ScreenId.Profile, ScreenId.Settings, ScreenId.ChapterMap,
                ScreenId.WorldMap, ScreenId.Shop, ScreenId.Events, ScreenId.Characters,
                ScreenId.Mail, ScreenId.DailyReward, ScreenId.Shop, ScreenId.Shop, ScreenId.Shop };
            navigationCommitted = false;
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];
                if (button == null) continue;
                ScreenId destination = destinations[i];
                // Play delegates to the existing story/chapter gate; it never calls Show directly.
                Action action = i == 2 ? openChapterMap :
                    (show != null ? (Action)(() => show(destination)) : null);
                boundButtons[i] = button;
                ownedListeners[i] = () =>
                {
                    if (!isActiveAndEnabled || navigationCommitted || action == null ||
                        button == null || !button.isActiveAndEnabled || !button.IsInteractable()) return;
                    navigationCommitted = true;
                    button.GetComponent<ProductionHomeButtonFeedback>()?.NotifyActivated();
                    action();
                };
                button.onClick.AddListener(ownedListeners[i]);
            }
            ActionsBound = show != null && openChapterMap != null;
        }

        public void UnbindNavigation()
        {
            for (int i = 0; i < boundButtons.Length; i++)
            {
                if (boundButtons[i] != null && ownedListeners[i] != null)
                    boundButtons[i].onClick.RemoveListener(ownedListeners[i]);
                boundButtons[i] = null;
                ownedListeners[i] = null;
            }
            ActionsBound = false;
        }

        public Button[] Buttons() => new[] { ProfileButton, SettingsButton, PlayButton, WorldMapButton,
            ShopButton, EventsButton, CharactersButton, MailButton, DailyRewardButton, GoldPlusButton, GemsPlusButton, EnergyPlusButton };
    }
}
