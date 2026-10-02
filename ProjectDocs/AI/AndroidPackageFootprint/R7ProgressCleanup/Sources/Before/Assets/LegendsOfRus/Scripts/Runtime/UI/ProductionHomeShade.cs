using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Full-display lower wash matching the reviewed Home's four gradient stops.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionHomeShade : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            float[] stops = { 0f, .26f, .63f, 1f };
            Color[] colors = { new Color32(4, 12, 22, 0), new Color32(4, 12, 22, 107),
                new Color32(4, 12, 22, 224), new Color32(6, 14, 25, 255) };
            for (int i = 0; i < stops.Length - 1; i++)
            {
                int n = vh.currentVertCount;
                float top = r.yMax - r.height * stops[i], bottom = r.yMax - r.height * stops[i + 1];
                vh.AddVert(new Vector3(r.xMin, bottom), colors[i + 1] * color, Vector2.zero);
                vh.AddVert(new Vector3(r.xMin, top), colors[i] * color, Vector2.zero);
                vh.AddVert(new Vector3(r.xMax, top), colors[i] * color, Vector2.zero);
                vh.AddVert(new Vector3(r.xMax, bottom), colors[i + 1] * color, Vector2.zero);
                vh.AddTriangle(n, n + 1, n + 2); vh.AddTriangle(n, n + 2, n + 3);
            }
        }
    }
}
