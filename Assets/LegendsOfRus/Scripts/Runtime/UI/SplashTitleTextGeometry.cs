using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Single-line label alignment based on actual generated glyph quads, not the
    /// font's ascent/descent box. Runs before Shadow. Does not change letters,
    /// alpha, font identity, navigation, or the parent Canvas scaling policy.
    /// </summary>
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public sealed class SplashTitleTextGeometry : BaseMeshEffect
    {
        public bool CenterHorizontally = true;
        public bool CenterVertically = true;
        public bool SnapToScreenPixels = true;
        [Tooltip("Local text units, including the intentional raster-density scale.")]
        public Vector2 OpticalOffset;

        public override void ModifyMesh(VertexHelper vh)
        {
            Text label = graphic as Text;
            if (!IsActive() || label == null || vh.currentVertCount < 4 ||
                string.IsNullOrEmpty(label.text) || label.text.IndexOf('\n') >= 0)
                return;

            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            UIVertex vertex = default(UIVertex);
            bool found = false;
            int count = vh.currentVertCount - vh.currentVertCount % 4;
            // Text's unmodified mesh is a sequence of four-vertex glyph quads.
            // Ignore degenerate whitespace / trailing quads so they cannot move
            // the optical center. This component must precede Shadow/Outline.
            for (int start = 0; start < count; start += 4)
            {
                Vector2 qMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                Vector2 qMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
                for (int k = 0; k < 4; k++)
                {
                    vh.PopulateUIVertex(ref vertex, start + k);
                    Vector2 p = vertex.position;
                    qMin = Vector2.Min(qMin, p);
                    qMax = Vector2.Max(qMax, p);
                }
                if (qMax.x - qMin.x < .001f || qMax.y - qMin.y < .001f) continue;
                min = Vector2.Min(min, qMin);
                max = Vector2.Max(max, qMax);
                found = true;
            }
            if (!found) return;

            RectTransform rt = label.rectTransform;
            Vector2 shift = OpticalOffset;
            if (CenterHorizontally) shift.x += rt.rect.center.x - (min.x + max.x) * .5f;
            if (CenterVertically) shift.y += rt.rect.center.y - (min.y + max.y) * .5f;

            Canvas canvas = label.canvas;
            if (SnapToScreenPixels && canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
                Vector3 world = rt.TransformPoint(new Vector3(min.x + shift.x, min.y + shift.y, 0f));
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, world);
                Vector2 snapped = new Vector2(Mathf.Round(screen.x), Mathf.Round(screen.y));
                Vector2 from, to;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screen, cam, out from) &&
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, snapped, cam, out to))
                    shift += to - from;
            }

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.position += new Vector3(shift.x, shift.y, 0f);
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}
