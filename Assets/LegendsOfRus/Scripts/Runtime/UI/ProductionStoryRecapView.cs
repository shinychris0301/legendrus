using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Local read-only sequence over the real Dialogue prefab in the existing modal host.</summary>
    public sealed class ProductionStoryRecapView : MonoBehaviour
    {
        public ProductionDialogueView Dialogue { get; private set; }
        public Button Back { get; private set; }
        public Button Earlier { get; private set; }
        public Button Later { get; private set; }
        public int SceneIndex { get; private set; }
        public int LineIndex => sequence?.LineIndex ?? 0;
        IReadOnlyList<StoryRecapEntry> entries;
        StorySequenceRuntime sequence;
        Func<bool> allowed;
        Action close;
        Text heading, backLabel, earlierLabel, laterLabel;
        string language;
        int actionFrame = -1;

        public void Bind(ProductionDialogueView dialogue, IReadOnlyList<StoryRecapEntry> scenes, Func<bool> canAct, Action onClose)
        {
            if (scenes == null || scenes.Count == 0) throw new ArgumentException("Recap requires an acknowledged scene.");
            Dialogue = dialogue; entries = scenes; allowed = canAct; close = onClose;
            heading = Label(dialogue.Body, "RecapHeading", 95, 30, 751, 58, 30);
            Back = Control("RecapBack", 45, out backLabel);
            Earlier = Control("RecapEarlier", 345, out earlierLabel);
            Later = Control("RecapLater", 645, out laterLabel);
            Back.onClick.AddListener(Close);
            Earlier.onClick.AddListener(() => Select(SceneIndex - 1));
            Later.onClick.AddListener(() => Select(SceneIndex + 1));
            Dialogue.LineProvider = locale => sequence.LocalizeCurrentLine(StoryCatalog.LoadDialogueLanguage(locale));
            Dialogue.SceneIdProvider = () => entries[SceneIndex].Scene.id;
            Dialogue.SpeakerKeyProvider = () => sequence.CurrentSpeakerKey;
            SetScene(entries.Count - 1);
        }

        bool CanAct => isActiveAndEnabled && allowed != null && allowed() && Time.frameCount != actionFrame;
        void Update() { if (Dialogue != null && language != DemoFeatureState.LanguageCode) Refresh(); }
        void OnDisable() { allowed = null; close = null; }
        void Close() { if (!CanAct) return; actionFrame = Time.frameCount; close?.Invoke(); }
        void Select(int index)
        {
            if (!CanAct || index < 0 || index >= entries.Count) return;
            actionFrame = Time.frameCount; SetScene(index);
        }
        void SetScene(int index)
        {
            SceneIndex = index;
            sequence = new StorySequenceRuntime(entries[index].Scene, StoryCatalog.LoadRussian());
            Refresh();
            Dialogue.Bind(Advance);
        }
        void Advance()
        {
            // Bind resets the native one-shot latch; the frame latch also rejects a repeated event.
            if (!CanAct) { Dialogue.Bind(Advance); return; }
            actionFrame = Time.frameCount;
            if (!sequence.Advance()) { close?.Invoke(); return; }
            Refresh(); Dialogue.Bind(Advance);
        }
        void Refresh()
        {
            language = DemoFeatureState.LanguageCode;
            bool en = language == "en";
            int level = entries[SceneIndex].AfterLevel;
            heading.text = (level == 0 ? (en ? "INTRODUCTION" : "НАЧАЛО СКАЗАНИЯ") :
                (en ? "AFTER LEVEL " : "ПОСЛЕ УРОВНЯ ") + level) + "  ·  " + (SceneIndex + 1) + " / " + entries.Count;
            backLabel.text = en ? "BACK" : "НАЗАД";
            earlierLabel.text = en ? "EARLIER" : "РАНЬШЕ";
            laterLabel.text = en ? "LATER" : "ПОЗЖЕ";
            Earlier.interactable = SceneIndex > 0; Later.interactable = SceneIndex + 1 < entries.Count;
            Earlier.image.color = Earlier.interactable ? Color.white : new Color(.52f, .52f, .52f, 1);
            Later.image.color = Later.interactable ? Color.white : new Color(.52f, .52f, .52f, 1);
            Dialogue.Refresh();
        }
        Button Control(string name, float x, out Text text)
        {
            var rect = Rect(Dialogue.Body, name, x, 105, 251, 110);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = ProductionProfileRuntime.Shared("tab"); image.type = Image.Type.Sliced;
            image.preserveAspect = false; image.pixelsPerUnitMultiplier = 2; image.raycastPadding = new Vector4(0, -10, 0, -10);
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None }; button.transition = Selectable.Transition.None;
            rect.gameObject.AddComponent<ProductionMapButtonFeedback>();
            text = Label(rect, "Label", 12, 20, 227, 70, 32);
            return button;
        }
        Text Label(Transform parent, string name, float x, float y, float w, float h, int size)
        {
            var label = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = Dialogue.TitleFont; label.fontSize = size; label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color32(243, 206, 137, 255); label.raycastTarget = false;
            label.resizeTextForBestFit = true; label.resizeTextMinSize = size - 4; label.resizeTextMaxSize = size;
            label.horizontalOverflow = HorizontalWrapMode.Wrap; label.verticalOverflow = VerticalWrapMode.Truncate;
            return label;
        }
        static RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var go = new GameObject(name, typeof(RectTransform)); go.layer = 5; go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform; rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(w, h); return rect;
        }
    }
}
