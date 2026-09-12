using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Presentation-only fade at the cropped bottom of the supplied portraits.
    /// PNG bytes and silhouette edges stay unchanged. This is not alpha repair or a rig.
    /// </summary>
    [RequireComponent(typeof(Image)), DisallowMultipleComponent]
    public sealed class SplashTitleCharacterFade : BaseMeshEffect
    {
        [Range(0f, .3f)] public float BottomFraction = .13f;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount != 4 || BottomFraction <= 0f) return;
            UIVertex bl = default(UIVertex), tl = default(UIVertex), tr = default(UIVertex), br = default(UIVertex);
            vh.PopulateUIVertex(ref bl, 0); vh.PopulateUIVertex(ref tl, 1);
            vh.PopulateUIVertex(ref tr, 2); vh.PopulateUIVertex(ref br, 3);
            vh.Clear();
            const int strips = 40;
            for (int i = 0; i < strips; i++)
            {
                float t0 = (float)i / strips, t1 = (float)(i + 1) / strips;
                int v = vh.currentVertCount;
                vh.AddVert(Interpolate(bl, tl, t0)); vh.AddVert(Interpolate(bl, tl, t1));
                vh.AddVert(Interpolate(br, tr, t1)); vh.AddVert(Interpolate(br, tr, t0));
                vh.AddTriangle(v, v + 1, v + 2); vh.AddTriangle(v, v + 2, v + 3);
            }
        }
        UIVertex Interpolate(UIVertex a, UIVertex b, float t)
        {
            UIVertex v = a;
            v.position = Vector3.Lerp(a.position, b.position, t);
            v.uv0 = Vector4.Lerp(a.uv0, b.uv0, t);
            Color c = Color.Lerp(a.color, b.color, t);
            c.a *= Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / BottomFraction));
            v.color = c;
            return v;
        }
    }
}
