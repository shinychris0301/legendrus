using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>A native UI gradient for footer contrast, not an image containing background or text.</summary>
    public sealed class SplashTitleFooterShade : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            Color lower = color, upper = color; upper.a = 0;
            vh.AddVert(new Vector3(r.xMin, r.yMin), lower, Vector2.zero);
            vh.AddVert(new Vector3(r.xMin, r.yMax), upper, Vector2.zero);
            vh.AddVert(new Vector3(r.xMax, r.yMax), upper, Vector2.zero);
            vh.AddVert(new Vector3(r.xMax, r.yMin), lower, Vector2.zero);
            vh.AddTriangle(0,1,2); vh.AddTriangle(0,2,3);
        }
    }
}
