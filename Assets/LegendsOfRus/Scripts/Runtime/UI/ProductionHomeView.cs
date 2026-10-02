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
        public const int CurrentRevision = 22;
        public int Revision = CurrentRevision;
        public ProductionPlayerHeader Header;
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
        public Button RankingButton, TeamButton, FriendsButton;
        public Text RankingText, TeamText, FriendsText;

        public Image SourcePlay;
        public Image[] SourceNavigation;
        public Sprite SourceRussianPlay, SourceBlankPlay, SourceRussianQuest, SourceBlankQuest;
        public Sprite[] SourceRussianCards, SourceBlankCards;
        public Font ReferenceBodyFont, ReferenceTitleFont;

        public bool ActionsBound { get; private set; }
        public string ActiveFontSource { get; private set; }
        public bool HasRequiredBindings => Revision == CurrentRevision && Header!=null && Header.HasBindings && Typography != null &&
            Environment != null && Environment.sprite != null && Logo != null && Logo.sprite != null &&
            Ivan != null && Ivan.sprite != null && Firebird != null && !Firebird.gameObject.activeSelf && Firebird.sprite == null &&
            Environment.color == Color.white && Ivan.color == Color.white &&
            LowerShade != null && LowerShade.GetComponent<CanvasRenderer>() != null &&
            HeroName != null && LevelText != null && GoldText != null && GemsText != null &&
            EnergyText != null && PlayText != null && WorldMapText != null && ShopText != null &&
            EventsText != null && CharactersText != null && QuestText != null &&
            ProfileButton != null && SettingsButton != null && PlayButton != null &&
            WorldMapButton != null && ShopButton != null && EventsButton != null &&
            RankingButton != null && TeamButton != null && FriendsButton != null &&
            RankingText != null && TeamText != null && FriendsText != null &&
            CharactersButton != null && MailButton != null && DailyRewardButton != null &&
            ProductionHomeSourceComposition.Valid(this);

        // Current source art does not use the historical lighting effects or foliage overlay.
        public bool HasSceneLightingBindings => HasRequiredBindings &&
            HasDisabledLighting(Ivan) && HasDisabledLighting(MapFoliage);

        static bool HasDisabledLighting(Image image)
        {
            var lighting = image != null ? image.GetComponent<ProductionHomeSceneLighting>() : null;
            return lighting != null && !lighting.enabled;
        }

        public bool HasReferenceBackdropBindings => HasRequiredBindings &&
            BackgroundShade != null && !BackgroundShade.enabled &&
            MapFoliage != null && !MapFoliage.enabled && MapFoliage.sprite == null;

        // The historical control diagnostic now validates the installed source composition.
        public bool HasGeneratedControlBindings => HasRequiredBindings;

        // Keep the historical diagnostic aligned with the current source composition.
        public bool HasReferenceForegroundBindings => HasRequiredBindings;

        readonly Button[] boundButtons = new Button[15];
        readonly UnityAction[] ownedListeners = new UnityAction[15];
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
            // This page now owns the extracted original header; shared navigation remains for other pages.
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
            ProductionHomeSourceComposition.Refresh(this);
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
                ProductionHomeSourceComposition.FitBalances(this);
            }
            finally { refreshingState = false; }
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
                ScreenId.Mail, ScreenId.DailyReward, ScreenId.Shop, ScreenId.Shop, ScreenId.Shop,
                ScreenId.IndividualRanking, ScreenId.TeamHub, ScreenId.FriendsList };
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
            ShopButton, EventsButton, CharactersButton, MailButton, DailyRewardButton, GoldPlusButton, GemsPlusButton, EnergyPlusButton, RankingButton, TeamButton, FriendsButton };
    }
}
