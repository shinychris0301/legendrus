using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Native Loading contrast: the authored fade followed by constant shade through display insets.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionLoadingShade : MaskableGraphic
    {
        public const float FadeSpan = 562f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            float fadeEnd = Mathf.Max(r.yMin, r.yMax - FadeSpan);
            Color clear = color; clear.a = 0f;
            AddBand(vh, r.xMin, r.xMax, fadeEnd, r.yMax, color, clear);
            if (fadeEnd > r.yMin)
                AddBand(vh, r.xMin, r.xMax, r.yMin, fadeEnd, color, color);
        }

        static void AddBand(VertexHelper vh, float left, float right, float bottom, float top,
            Color lower, Color upper)
        {
            int first = vh.currentVertCount;
            vh.AddVert(new Vector3(left, bottom), lower, Vector2.zero);
            vh.AddVert(new Vector3(left, top), upper, Vector2.zero);
            vh.AddVert(new Vector3(right, top), upper, Vector2.zero);
            vh.AddVert(new Vector3(right, bottom), lower, Vector2.zero);
            vh.AddTriangle(first, first + 1, first + 2);
            vh.AddTriangle(first, first + 2, first + 3);
        }
    }
}
