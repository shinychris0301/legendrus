using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Home's safe-top header, safe-bottom controls and full-display scenery, independent of the shared fixed stage.</summary>
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class ProductionHomeLayout : MonoBehaviour
    {
        public RectTransform Content, Hud, HudBacking, Logo, Ivan, Firebird, Play, Navigation, Footer, FooterBase, FooterBacking;
        public Image Environment;
        public ProductionHomeShade Shade;
        readonly Vector3[] corners = new Vector3[4];
        bool applying;
        public bool HasRequiredBindings => HasBaseBindings && HudBacking != null && FooterBase != null && FooterBacking != null;
        // Older serialized revisions predate the backing and footer bindings. The Editor migration
        // uses this base contract so upgrading never destroys an otherwise complete page.
        public bool HasBaseBindings => Content != null && Hud != null && Logo != null && Ivan != null &&
            Firebird != null && Play != null && Navigation != null && Footer != null && Environment != null &&
            Environment.sprite != null && Shade != null;

        void OnEnable() { Canvas.preWillRenderCanvases += ApplyNow; ApplyNow(); }
        void OnDisable() { Canvas.preWillRenderCanvases -= ApplyNow; }
        void OnRectTransformDimensionsChange() { ApplyNow(); }
        public void ApplyNow()
        {
            if (applying || !HasRequiredBindings) return;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
            RectTransform canvasRect = canvas.rootCanvas.transform as RectTransform;
            SafeAreaFitter safe = GetComponentInParent<SafeAreaFitter>();
            RectTransform safeRect = safe != null ? safe.transform as RectTransform : canvasRect;
            if (canvasRect == null || safeRect == null) return;
            applying = true;
            try
            {
                safeRect.GetWorldCorners(corners);
                Vector3 safeLo = Content.InverseTransformPoint(corners[0]), safeHi = Content.InverseTransformPoint(corners[2]);
                float top = safeHi.y - Content.rect.yMax, bottom = safeLo.y - Content.rect.yMin;
                float extra = Mathf.Max(0, safeHi.y - safeLo.y - 1672f);
                Position(Hud, 0, top);
                Position(HudBacking, 5, top - 5);
                Position(Logo, 205.5f, top - 110);
                // Keep the reference character and perched bird close to the crest;
                // their complete shared sprite preserves the pose on tall phones while
                // lower interactive controls remain anchored to the safe bottom.
                float heroOffset = extra * .10f;
                Position(Ivan, -20, top - 474 - heroOffset);
                Position(Play, 185.5f, bottom - (1230 - 570f * 803 / 1959)); Position(Navigation, 51, bottom - 1230);
                Position(Footer, 0, bottom - 1450);
                canvasRect.GetWorldCorners(corners);
                Vector3 lo = Content.InverseTransformPoint(corners[0]), hi = Content.InverseTransformPoint(corners[2]);
                // The connecting ornament reaches the physical edges beneath system
                // insets; its interactive children stay inside the safe rectangle.
                Vector3 footerLo = Footer.InverseTransformPoint(corners[0]);
                Vector3 footerHi = Footer.InverseTransformPoint(corners[2]);
                float footerWidth = footerHi.x - footerLo.x;
                float carvedBackingHeight = 362f * footerWidth / 2136;
                Position(FooterBacking, footerLo.x - Footer.rect.xMin,
                    footerLo.y - Footer.rect.yMax + carvedBackingHeight);
                Size(FooterBacking, new Vector2(footerWidth, carvedBackingHeight));
                // The low carved strip retains source aspect, with its visible lower
                // edge at the physical display bottom and transparent margins intact.
                float footerScale = footerWidth / 2136;
                Position(FooterBase, footerLo.x - Footer.rect.xMin - 4 * footerScale,
                    footerLo.y - Footer.rect.yMax + 708 * footerScale);
                Size(FooterBase, new Vector2(2143 * footerScale, 734 * footerScale));
                Vector2 native = Environment.sprite.rect.size;
                float scale = Mathf.Max((hi.x - lo.x) / native.x, (hi.y - lo.y) / native.y);
                Vector2 size = native * scale;
                Position(Environment.rectTransform, (lo.x + hi.x - size.x) * .5f - Content.rect.xMin,
                    (lo.y + hi.y + size.y) * .5f - Content.rect.yMax);
                Size(Environment.rectTransform, size);
                float shadeHeight = 690 + Mathf.Max(0, safeLo.y - lo.y);
                Position(Shade.rectTransform, lo.x - Content.rect.xMin, lo.y + shadeHeight - Content.rect.yMax);
                Size(Shade.rectTransform, new Vector2(hi.x - lo.x, shadeHeight));
            }
            finally { applying = false; }
        }
        static void Position(RectTransform rect, float x, float y)
        {
            Vector2 p = new Vector2(x, y);
            if ((rect.anchoredPosition - p).sqrMagnitude > .0001f) rect.anchoredPosition = p;
        }
        static void Size(RectTransform rect, Vector2 size)
        {
            if ((rect.sizeDelta - size).sqrMagnitude > .0001f) rect.sizeDelta = size;
        }
    }
}
