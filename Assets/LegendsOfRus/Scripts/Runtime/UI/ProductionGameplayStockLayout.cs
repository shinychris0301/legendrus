using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Uses the existing lower card well for exact inventory values that outgrow its circular badge.</summary>
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public sealed class ProductionGameplayStockLayout : MonoBehaviour
    {
        public ProductionPreLevelBadge Badge;
        public ProductionHomePanel WideBacking;
        Text label;
        string observed;

        void OnEnable() { ApplyNow(); }
        void LateUpdate()
        {
            if (label == null) label = GetComponent<Text>();
            if (label.text != observed) ApplyNow();
        }

        public void ApplyNow()
        {
            if (Badge == null || WideBacking == null) return;
            if (label == null) label = GetComponent<Text>();
            observed = label.text;
            int digits = 0;
            foreach (char c in observed) if (c >= '0' && c <= '9') digits++;
            bool wide = digits > 3;
            Badge.enabled = !wide;
            WideBacking.enabled = wide;
            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.localScale = new Vector3(.5f, .5f, 1);
            rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = wide ? new Vector2(-108, -58) : new Vector2(7.28f, 0);
            rect.sizeDelta = wide ? new Vector2(324, 104) : new Vector2(74.88f, 104);
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 12;
            label.resizeTextMaxSize = label.fontSize = 73;
            label.raycastTarget = false;
            label.SetVerticesDirty();
        }
    }
}
