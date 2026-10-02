using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public sealed class ProductionDialogueView : MonoBehaviour
    {
        public const int CurrentRevision = 2;
        public int Revision = CurrentRevision;
        public RectTransform Content, Body;
        public Image Environment, Ivan, Dobrynya, Firebird, Panel;
        public Sprite Hall, Forest, BlankPanel, DobrynyaPanelRu, ContinueRu, ContinueLocalized;
        public Font BodyFont, TitleFont;
        public Text Speaker, Description, ContinueLabel;
        public Button Continue;
        public bool ActionsBound { get; private set; }
        // Explicit campaign data does not read or mutate the live StoryService singleton.
        public Func<string, LocalizedStoryLine> LineProvider;
        public Func<string> SceneIdProvider, SpeakerKeyProvider;
        UnityAction listener;
        bool leaving;
        string language;

        public bool HasRequiredBindings => Revision == CurrentRevision && Content && Body &&
            Environment && Ivan && Dobrynya && Firebird && Hall && Forest && BodyFont &&
            TitleFont && Panel && BlankPanel && DobrynyaPanelRu && ContinueRu && ContinueLocalized && Speaker && Description && ContinueLabel && Continue;

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
                if (LineProvider == null && ScreenRouter.Instance != null && (ScreenRouter.Instance.IsScreenTransitioning || ScreenRouter.Instance.HasActiveModalBlocker)) return;
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
            var line = LineProvider != null ? LineProvider(language) : StoryService.CurrentLineForLanguage(language);
            Speaker.text = line != null ? line.Speaker : (en ? "STORY" : "СКАЗАНИЕ");
            Description.text = line != null ? line.Text : (en ? "There is no active story scene." : "Сейчас нет активной истории.");
            ContinueLabel.text = en ? "CONTINUE" : "ПРОДОЛЖИТЬ";
            Continue.interactable = line != null;
            string sceneId = SceneIdProvider != null ? SceneIdProvider() : StoryService.ActiveSceneId;
            bool intro = sceneId == "chapter01.intro" || string.IsNullOrEmpty(sceneId);
            string key = SpeakerKeyProvider != null ? SpeakerKeyProvider() : StoryService.CurrentSpeakerKey;
            bool originalName = !en && line != null && key == "speaker.dobrynya";
            Panel.sprite = originalName ? DobrynyaPanelRu : BlankPanel;
            Speaker.enabled = !originalName;
            ((Image)Continue.targetGraphic).sprite = en ? ContinueLocalized : ContinueRu;
            ContinueLabel.enabled = en;
            Continue.targetGraphic.color = line != null ? Color.white : new Color(.62f,.62f,.62f,1);
            ContinueLabel.color = line != null ? new Color32(241,204,128,255) : new Color32(149,126,79,255);
            Environment.sprite = intro ? Hall : Forest;
            Ivan.gameObject.SetActive(intro || key == "speaker.ivan");
            Dobrynya.gameObject.SetActive(intro || key == "speaker.dobrynya");
            Firebird.gameObject.SetActive(!intro && key == "speaker.firebird");
            // Preserve original character colors. Other speakers use an unobstructed scene,
            // rather than misidentifying a reused portrait as the elder or raven.
        }
    }
}
