using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Applies the safe-area fit scale directly, keeping the fitted portrait
    /// viewport at exactly 941x1672 canvas units. This prevents fixed-size
    /// runtime UI (board cells, type, and effects) from changing scale when
    /// device insets alter the limiting dimension.
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class AdaptiveCanvasScaler : MonoBehaviour
    {
        public float DesignWidth = 941f;
        public float DesignHeight = 1672f;

        CanvasScaler scaler;
        int lastWidth = -1;
        int lastHeight = -1;
        Rect lastSafeArea;

        public Vector2Int LastAppliedScreenSize => new Vector2Int(lastWidth, lastHeight);
        public Rect LastAppliedSafeArea => lastSafeArea;
        public float LastAppliedMatch => scaler != null ? scaler.matchWidthOrHeight : 0f;
        public float LastAppliedScale => scaler != null ? scaler.scaleFactor : 0f;

        void Awake()
        {
            scaler = GetComponent<CanvasScaler>();
            ApplyNow();
        }

        void Update()
        {
            var size = new Vector2Int(Screen.width, Screen.height);
            Rect safe = FidelityVerification.NormalizeSafeArea(
                Screen.safeArea,
                size
            );
            if (
                size.x == lastWidth &&
                size.y == lastHeight &&
                safe == lastSafeArea
            )
                return;

            ApplyNow();
        }

        public void ApplyNow()
        {
            Apply(
                Screen.safeArea,
                new Vector2Int(Screen.width, Screen.height)
            );
        }

        public void Apply(Vector2Int screenSize)
        {
            Apply(
                new Rect(0f, 0f, screenSize.x, screenSize.y),
                screenSize
            );
        }

        public void Apply(Rect safeArea, Vector2Int screenSize)
        {
            if (scaler == null) scaler = GetComponent<CanvasScaler>();
            lastWidth = Mathf.Max(1, screenSize.x);
            lastHeight = Mathf.Max(1, screenSize.y);
            lastSafeArea = FidelityVerification.NormalizeSafeArea(
                safeArea,
                new Vector2Int(lastWidth, lastHeight)
            );

            var availableSize = new Vector2Int(
                Mathf.Max(1, Mathf.RoundToInt(lastSafeArea.width)),
                Mathf.Max(1, Mathf.RoundToInt(lastSafeArea.height))
            );
            // ConstantPixelSize with the safe-area fit scale makes the fitted
            // viewport exactly 941x1672 canvas units even when device insets
            // change the limiting dimension. MatchWidthOrHeight alone uses the
            // full screen and cannot represent that inset-adjusted scale.
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = ResolveScale(
                availableSize,
                DesignWidth,
                DesignHeight
            );
            scaler.matchWidthOrHeight = ResolveMatch(
                availableSize,
                DesignWidth,
                DesignHeight
            );
        }

        public static float ResolveScale(
            Vector2Int availableSize,
            float designWidth,
            float designHeight
        )
        {
            float width = Mathf.Max(1, availableSize.x);
            float height = Mathf.Max(1, availableSize.y);
            float safeDesignWidth = Mathf.Max(1f, designWidth);
            float safeDesignHeight = Mathf.Max(1f, designHeight);
            return Mathf.Min(
                width / safeDesignWidth,
                height / safeDesignHeight
            );
        }

        public static float ResolveMatch(
            Vector2Int screenSize,
            float designWidth,
            float designHeight
        )
        {
            float width = Mathf.Max(1, screenSize.x);
            float height = Mathf.Max(1, screenSize.y);
            float safeDesignWidth = Mathf.Max(1f, designWidth);
            float safeDesignHeight = Mathf.Max(1f, designHeight);
            return width / height > safeDesignWidth / safeDesignHeight ? 1f : 0f;
        }
    }
}
