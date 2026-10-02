using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public sealed class ProductionDialogueView : MonoBehaviour
    {
        public const int CurrentRevision = 1;
        public int Revision = CurrentRevision;
        public RectTransform Content, Body;
        public Image Environment, Ivan, Dobrynya, Firebird;
        public Sprite Hall, Forest;
        public Font BodyFont, TitleFont;
        public Text Speaker, Description, ContinueLabel;
        public Button Continue;
        public bool ActionsBound { get; private set; }
        UnityAction listener;
        bool leaving;
        string language;

        public bool HasRequiredBindings => Revision == CurrentRevision && Content && Body &&
            Environment && Ivan && Dobrynya && Firebird && Hall && Forest && BodyFont &&
            TitleFont && Speaker && Description && ContinueLabel && Continue;

        void OnEnable() { Refresh(); }
        void OnDisable() { Unbind(); }
        void Update() { if (language != DemoFeatureState.LanguageCode) Refresh(); }

        public void Bind(Action onContinue)
        {
            Unbind();
            leaving = false;
            listener = () =>
            {
                if (!ActionsBound || leaving || !isActiveAndEnabled || !Continue.IsInteractable() || onContinue == null) return;
                if (ScreenRouter.Instance != null && (ScreenRouter.Instance.IsScreenTransitioning || ScreenRouter.Instance.HasActiveModalBlocker)) return;
                leaving = true;
                onContinue();
            };
            Continue.onClick.AddListener(listener);
            ActionsBound = true;
            Refresh();
        }

        void Unbind()
        {
            if (Continue && listener != null) Continue.onClick.RemoveListener(listener);
            listener = null;
            ActionsBound = false;
        }

        public void Refresh()
        {
            if (!HasRequiredBindings) return;
            language = DemoFeatureState.LanguageCode;
            bool en = language == "en";
            var line = StoryService.CurrentLineForLanguage(language);
            Speaker.text = line != null ? line.Speaker : (en ? "STORY" : "СКАЗАНИЕ");
            Description.text = line != null ? line.Text : (en ? "There is no active story scene." : "Сейчас нет активной истории.");
            ContinueLabel.text = en ? "CONTINUE" : "ПРОДОЛЖИТЬ";
            Continue.interactable = line != null;
            bool intro = StoryService.ActiveSceneId == "chapter01.intro" || string.IsNullOrEmpty(StoryService.ActiveSceneId);
            string key = StoryService.CurrentSpeakerKey;
            Environment.sprite = intro ? Hall : Forest;
            Ivan.gameObject.SetActive(intro || key == "speaker.ivan");
            Dobrynya.gameObject.SetActive(intro || key == "speaker.dobrynya");
            Firebird.gameObject.SetActive(!intro && key == "speaker.firebird");
            // Preserve original character colors. Other speakers use an unobstructed scene,
            // rather than misidentifying a reused portrait as the elder or raven.
        }
    }
}
