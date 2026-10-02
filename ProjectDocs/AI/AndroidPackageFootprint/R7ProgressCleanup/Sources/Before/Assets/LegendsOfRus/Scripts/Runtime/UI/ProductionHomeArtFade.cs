using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Presentation-only lower fade of the approved full-rect Ivan sprite; source pixels stay intact.</summary>
    public sealed class ProductionHomeArtFade : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount != 4) return;
            UIVertex a = default, b = default, c = default, d = default;
            vh.PopulateUIVertex(ref a, 0); vh.PopulateUIVertex(ref b, 1);
            vh.PopulateUIVertex(ref c, 2); vh.PopulateUIVertex(ref d, 3);
            // A simple, unsliced Unity Image is bottom-left, top-left, top-right, bottom-right.
            UIVertex left = a, right = d;
            left.position = Vector3.Lerp(a.position, b.position, .2f);
            left.uv0 = Vector4.Lerp(a.uv0, b.uv0, .2f);
            right.position = Vector3.Lerp(d.position, c.position, .2f);
            right.uv0 = Vector4.Lerp(d.uv0, c.uv0, .2f);
            a.color.a = d.color.a = 0;
            vh.Clear();
            vh.AddVert(a); vh.AddVert(left); vh.AddVert(right); vh.AddVert(d);
            vh.AddVert(left); vh.AddVert(b); vh.AddVert(c); vh.AddVert(right);
            vh.AddTriangle(0, 1, 2); vh.AddTriangle(0, 2, 3);
            vh.AddTriangle(4, 5, 6); vh.AddTriangle(4, 6, 7);
        }
    }
}
