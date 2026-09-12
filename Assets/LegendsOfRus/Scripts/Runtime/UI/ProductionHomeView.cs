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
        public const int CurrentRevision = 1;
        public int Revision = CurrentRevision;
        public SplashTitleTypography Typography;
        public Image Environment;
        public Image Logo;
        public Image Ivan;
        public Image Firebird;
        public ProductionHomeShade LowerShade;

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
            Ivan != null && Ivan.sprite != null && Firebird != null && Firebird.sprite != null &&
            LowerShade != null && LowerShade.GetComponent<CanvasRenderer>() != null &&
            HeroName != null && LevelText != null && GoldText != null && GemsText != null &&
            EnergyText != null && PlayText != null && WorldMapText != null && ShopText != null &&
            EventsText != null && CharactersText != null && QuestText != null &&
            ProfileButton != null && SettingsButton != null && PlayButton != null &&
            WorldMapButton != null && ShopButton != null && EventsButton != null &&
            CharactersButton != null && MailButton != null && DailyRewardButton != null;

        readonly Button[] boundButtons = new Button[9];
        readonly UnityAction[] ownedListeners = new UnityAction[9];
        GameState subscribedState;
        string lastLanguage;
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
            if (EnsureStateBinding()) RefreshState();
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
            lastLanguage = DemoFeatureState.LanguageCode;
            bool en = lastLanguage == "en";
            SetText(HeroName, en ? "Ivan" : "Иван");
            SetText(PlayText, en ? "PLAY" : "ИГРАТЬ");
            SetText(WorldMapText, en ? "WORLD MAP" : "КАРТА МИРА");
            SetText(ShopText, en ? "SHOP" : "МАГАЗИН");
            SetText(EventsText, en ? "EVENTS" : "СОБЫТИЯ");
            SetText(CharactersText, en ? "CHARACTERS" : "ПЕРСОНАЖИ");
            // Existing copy only: no quest service currently supplies a goal, measurement or reward.
            SetText(QuestText, en ? "Collect blue gems" : "Соберите синие самоцветы");

            if (Typography != null)
            {
                string source;
                Font regular = Typography.Resolve(false, out source);
                ActiveFontSource = source;
                Text[] labels = { HeroName, LevelText, GoldText, GemsText, EnergyText,
                    WorldMapText, ShopText, EventsText, CharactersText, QuestText };
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
                ScreenId.Mail, ScreenId.DailyReward };
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
            ShopButton, EventsButton, CharactersButton, MailButton, DailyRewardButton };
    }
}
