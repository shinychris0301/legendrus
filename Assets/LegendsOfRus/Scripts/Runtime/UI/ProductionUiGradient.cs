using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Lightweight two-color UI backdrop used by reconstructed production pages.
    /// It avoids a flattened full-screen screenshot while keeping the backdrop
    /// resolution independent and safe for the project's aspect-fitted viewport.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionUiGradient : MaskableGraphic
    {
        [SerializeField] Color topColor = new Color32(8, 30, 53, 255);
        [SerializeField] Color bottomColor = new Color32(3, 10, 18, 255);

        public Color TopColor { get { return topColor; } }
        public Color BottomColor { get { return bottomColor; } }

        public void Configure(Color top, Color bottom)
        {
            topColor = top;
            bottomColor = bottom;
            raycastTarget = false;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            UIVertex v = UIVertex.simpleVert;
            v.uv0 = Vector2.zero;

            v.position = new Vector3(r.xMin, r.yMin); v.color = bottomColor; vh.AddVert(v);
            v.position = new Vector3(r.xMin, r.yMax); v.color = topColor; vh.AddVert(v);
            v.position = new Vector3(r.xMax, r.yMax); v.color = topColor; vh.AddVert(v);
            v.position = new Vector3(r.xMax, r.yMin); v.color = bottomColor; vh.AddVert(v);
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}
