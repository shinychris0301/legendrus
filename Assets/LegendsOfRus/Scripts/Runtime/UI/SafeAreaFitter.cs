using UnityEngine;

namespace LegendsOfRus
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        RectTransform rect;
        Rect lastSafe;
        Vector2Int lastSize;

        public Rect LastAppliedSafeArea => lastSafe;
        public Vector2Int LastAppliedScreenSize => lastSize;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            ApplyNow();
        }

        void Update()
        {
            var size = new Vector2Int(Screen.width, Screen.height);
            Rect safe = FidelityVerification.NormalizeSafeArea(
                Screen.safeArea,
                size
            );
            if (safe != lastSafe || size != lastSize)
                ApplyNow();
        }

        public void ApplyNow()
        {
            Apply(Screen.safeArea, new Vector2Int(Screen.width, Screen.height));
        }

        public void Apply(Rect safeArea, Vector2Int screenSize)
        {
            if (rect == null) rect = GetComponent<RectTransform>();
            Vector2Int size = new Vector2Int(
                Mathf.Max(1, screenSize.x),
                Mathf.Max(1, screenSize.y)
            );
            Rect safe = FidelityVerification.NormalizeSafeArea(safeArea, size);
            Vector2 min = safe.position;
            Vector2 max = safe.position + safe.size;
            min.x /= size.x;
            min.y /= size.y;
            max.x /= size.x;
            max.y /= size.y;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            lastSafe = safe;
            lastSize = size;
        }
    }
}
