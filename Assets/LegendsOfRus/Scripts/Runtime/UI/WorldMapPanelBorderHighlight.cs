using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>A continuous panel perimeter, independent of the extracted ornament's alpha.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class WorldMapPanelBorderHighlight : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect bounds = rectTransform.rect;
            // Concentric chamfered contours: soft halo, narrow gold edge, empty center.
            float[] insets = { 1f, 5f, 7f, 9f, 11f, 15f };
            Color32[] colors = {
                new Color32(255, 166, 26, 0), new Color32(255, 177, 32, 100),
                new Color32(255, 219, 112, 255), new Color32(255, 239, 174, 255),
                new Color32(255, 177, 32, 100), new Color32(255, 166, 26, 0)
            };
            for (int ring = 0; ring < insets.Length; ring++)
            {
                float inset = insets[ring];
                float left = bounds.xMin + inset, right = bounds.xMax - inset;
                float bottom = bounds.yMin + inset, top = bounds.yMax - inset;
                float corner = Mathf.Max(1f, 23f - inset);
                Vector2[] points = {
                    new Vector2(left + corner, top), new Vector2(right - corner, top),
                    new Vector2(right, top - corner), new Vector2(right, bottom + corner),
                    new Vector2(right - corner, bottom), new Vector2(left + corner, bottom),
                    new Vector2(left, bottom + corner), new Vector2(left, top - corner)
                };
                foreach (Vector2 point in points) mesh.AddVert(point, colors[ring], Vector2.zero);
                if (ring == 0) continue;
                for (int edge = 0; edge < 8; edge++)
                {
                    int a = (ring - 1) * 8 + edge, b = (ring - 1) * 8 + (edge + 1) % 8;
                    int c = ring * 8 + edge, d = ring * 8 + (edge + 1) % 8;
                    mesh.AddTriangle(a, b, c);
                    mesh.AddTriangle(b, d, c);
                }
            }
        }
    }
}
