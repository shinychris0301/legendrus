using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public sealed class ProductionLanguageSelectView : MonoBehaviour
    {
        public const int CurrentRevision = 4;
        public int Revision = CurrentRevision;
        public RectTransform Body, Shield;
        public Image Panel;
        public Text Heading;
        public Button Russian, English, Close;
        public CanvasGroup Backdrop;
        public SplashTitleView BackdropView;
        public Font TitleFont;
        public bool ActionsBound { get; private set; }
        string language;
        Action back;
        bool closed;
        public bool HasRequiredBindings => Revision == CurrentRevision && Body && Shield && Panel &&
            Heading && Russian && English && Close && Backdrop && BackdropView && BackdropView.HasRequiredBindings && TitleFont;
        void OnEnable() { Refresh(); }
        void OnDisable() { Unbind(); }
        void Update() { if (language != DemoFeatureState.LanguageCode) Refresh(); }
        public void Bind(Action onClose)
        {
            Unbind(); back = onClose; closed = false;
            Russian.onClick.AddListener(SelectRussian);
            English.onClick.AddListener(SelectEnglish);
            Close.onClick.AddListener(ClosePage);
            ActionsBound = true; Refresh();
        }
        void Unbind()
        {
            if (Russian) Russian.onClick.RemoveListener(SelectRussian);
            if (English) English.onClick.RemoveListener(SelectEnglish);
            if (Close) Close.onClick.RemoveListener(ClosePage);
            ActionsBound = false; back = null;
        }
        bool CanAct() => ActionsBound && !closed && isActiveAndEnabled && Close.IsInteractable();
        void SelectRussian() { Select("ru"); }
        void SelectEnglish() { Select("en"); }
        void Select(string code)
        {
            if (!CanAct()) return;
            DemoFeatureState.SelectLanguage(code); Refresh();
        }
        void ClosePage()
        {
            if (!CanAct()) return;
            closed = true; back?.Invoke();
        }
        public void Refresh()
        {
            if (!HasRequiredBindings) return;
            language = DemoFeatureState.LanguageCode;
            bool en = language == "en";
            Panel.sprite = ProductionSettingsRuntime.Load(en ? "language_panel_blank" : "language_panel_ru");
            Heading.enabled = en; Heading.text = "SELECT LANGUAGE"; Heading.font = TitleFont;
            Button[] buttons = { Russian, English };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].image.sprite = ProductionSettingsRuntime.Load((i == 1) == en ? "language_selected" : "language_unselected");
                Text label = buttons[i].GetComponentInChildren<Text>(true);
                label.text = i == 0 ? "РУССКИЙ" : "ENGLISH"; label.font = TitleFont;
            }
        }
    }
}
