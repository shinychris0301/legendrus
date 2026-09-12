using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>A native UI gradient for footer contrast, not an image containing background or text.</summary>
    public sealed class SplashTitleFooterShade : MaskableGraphic
    {
        [SerializeField, Min(1f)] float fadeSpan = 366f;
        public float FadeSpan
        {
            get => fadeSpan;
            set
            {
                float span = Mathf.Max(1f, value);
                if (Mathf.Approximately(fadeSpan, span)) return;
                fadeSpan = span;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            float fadeEnd = Mathf.Max(r.yMin, r.yMax - fadeSpan);
            Color lower = color, upper = color; upper.a = 0;
            AddBand(vh, r.xMin, r.xMax, fadeEnd, r.yMax, lower, upper);
            // Preserve the original fade, then continue its final opacity below the reference viewport.
            if (fadeEnd > r.yMin) AddBand(vh, r.xMin, r.xMax, r.yMin, fadeEnd, lower, lower);
        }

        static void AddBand(VertexHelper vh, float left, float right, float bottom, float top,
            Color lower, Color upper)
        {
            int first = vh.currentVertCount;
            vh.AddVert(new Vector3(left, bottom), lower, Vector2.zero);
            vh.AddVert(new Vector3(left, top), upper, Vector2.zero);
            vh.AddVert(new Vector3(right, top), upper, Vector2.zero);
            vh.AddVert(new Vector3(right, bottom), lower, Vector2.zero);
            vh.AddTriangle(first, first + 1, first + 2); vh.AddTriangle(first, first + 2, first + 3);
        }
    }
}
