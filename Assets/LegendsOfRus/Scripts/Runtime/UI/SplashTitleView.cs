using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Serialized art and live controls. ScreenRouter remains the navigation authority.</summary>
    [DisallowMultipleComponent]
    public sealed class SplashTitleView : MonoBehaviour
    {
        public const int CurrentRevision = 5;
        public int Revision = CurrentRevision;
        public SplashTitleTypography Typography;
        public Button StartButton;
        public Button LanguageButton;
        public Button SettingsButton;
        public Button AccountButton;
        public Button HelpButton;
        public Text VersionText;
        public Text LanguageText;
        public Text StartText;
        public Text SettingsText;
        public Text AccountText;
        public Text HelpText;
        public Text TaglineText;
        public Text LegalText;
        public string ActiveFontSource { get; private set; }
        public bool ActionsBound { get; private set; }
        public bool TypographyVerified => Typography != null && Typography.HasVerifiedBindings;
        public bool HasCurrentRevision => Revision == CurrentRevision;
        public bool HasCompleteBindings => Typography != null &&
            StartButton != null && LanguageButton != null && SettingsButton != null &&
            AccountButton != null && HelpButton != null && VersionText != null &&
            LanguageText != null && StartText != null && SettingsText != null &&
            AccountText != null && HelpText != null && TaglineText != null && LegalText != null;
        public bool HasRequiredBindings => HasCurrentRevision && HasCompleteBindings;

        /// <summary>Diagnostic only: do not conflate an old revision with missing references.</summary>
        public string GetValidationFailure()
        {
            var issues = new List<string>();
            if (!HasCurrentRevision)
                issues.Add("Serialized revision " + Revision + "; required revision " + CurrentRevision);
            UnityEngine.Object[] references = {
                Typography, StartButton, LanguageButton, SettingsButton, AccountButton, HelpButton,
                VersionText, LanguageText, StartText, SettingsText, AccountText, HelpText, TaglineText, LegalText
            };
            string[] names = {
                "Typography", "StartButton", "LanguageButton", "SettingsButton", "AccountButton", "HelpButton",
                "VersionText", "LanguageText", "StartText", "SettingsText", "AccountText", "HelpText", "TaglineText", "LegalText"
            };
            for (int i = 0; i < references.Length; i++)
                if (references[i] == null) issues.Add("Missing binding: " + names[i]);
            return string.Join("; ", issues);
        }

        readonly UnityAction[] ownedListeners = new UnityAction[5];
        string lastLanguage;
        string lastVersion;
        bool navigationCommitted;

        void OnEnable() { navigationCommitted = false; RefreshContent(); }
        void Update()
        {
            if (lastLanguage != DemoFeatureState.LanguageCode || lastVersion != Application.version)
                RefreshContent();
        }
        void OnDisable() { UnbindActions(); }

        public void RefreshContent()
        {
            if (!HasRequiredBindings) return;
            lastLanguage = DemoFeatureState.LanguageCode;
            lastVersion = Application.version;
            bool en = lastLanguage == "en";
            VersionText.text = "v" + Application.version;
            LanguageText.text = en ? "English" : "Русский";
            StartText.text = en ? "Start" : "Начать";
            SettingsText.text = en ? "Settings" : "Настройки";
            AccountText.text = en ? "Account" : "Аккаунт";
            HelpText.text = en ? "Help" : "Помощь";
            TaglineText.text = en ? "Inspired by legends. Made for you." : "Вдохновлено легендами. Собрано для тебя.";
            // Preserve the approved copy/year; do not invent a new copyright date.
            LegalText.text = en ? "© 2024–2025 Legends of Rus. All rights reserved." : "© 2024–2025 Легенды Руси. Все права защищены.";
            string source;
            Font regular = Typography.Resolve(false, out source);
            ActiveFontSource = source;
            Text[] labels = { VersionText, LanguageText, SettingsText, AccountText, HelpText, TaglineText, LegalText };
            foreach (Text label in labels) { label.font = regular; label.raycastTarget = false; }
            StartText.font = Typography.Resolve(true, out source);
            StartText.raycastTarget = false;
        }

        public void BindActions(Action start, Action language, Action settings, Action account, Action help)
        {
            UnbindActions();
            Action[] actions = { start, language, settings, account, help };
            Button[] buttons = Buttons();
            navigationCommitted = false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == null) continue;
                Action captured = actions[i];
                ownedListeners[i] = () =>
                {
                    if (!isActiveAndEnabled || navigationCommitted || captured == null) return;
                    navigationCommitted = true;
                    captured();
                };
                buttons[i].onClick.AddListener(ownedListeners[i]);
            }
            ActionsBound = true;
        }

        public void UnbindActions()
        {
            Button[] buttons = Buttons();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null && ownedListeners[i] != null)
                    buttons[i].onClick.RemoveListener(ownedListeners[i]);
                ownedListeners[i] = null;
            }
            ActionsBound = false;
        }
        public Button[] Buttons() => new[] { StartButton, LanguageButton, SettingsButton, AccountButton, HelpButton };
    }
}
