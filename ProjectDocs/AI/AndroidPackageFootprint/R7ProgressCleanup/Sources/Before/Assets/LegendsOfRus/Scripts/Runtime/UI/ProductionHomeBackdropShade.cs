using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Restrained lower and edge tone on Home's environment plate only.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionHomeBackdropShade : MaskableGraphic
    {
        [Range(0, 1)] public float Strength = 1;

        static readonly float[] Columns = { 0f, .05f, .12f, .22f, .35f, .5f, .65f, .78f, .88f, .95f, 1f };
        static readonly float[] Rows = { 0f, .30f, .46f, .50f, .56f, .62f, .68f, .74f, .80f, .86f, .92f, 1f };

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            for (int y = 0; y < Rows.Length; y++)
            {
                float lower = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.50f, 1, Rows[y]));
                float edgeFalloff = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.46f, .86f, Rows[y]));
                for (int x = 0; x < Columns.Length; x++)
                {
                    float edge = Mathf.Pow(Mathf.Abs(Columns[x] * 2 - 1), 2.5f);
                    Color tint = color;
                    // At full strength: zero above46%, at most16% at the lower
                    // center and23% at its edges. No global UI exposure changes.
                    tint.a *= (.16f * lower + .07f * edgeFalloff * edge) * Mathf.Clamp01(Strength);
                    vh.AddVert(new Vector3(r.xMin + r.width * Columns[x], r.yMax - r.height * Rows[y]), tint, Vector2.zero);
                    if (x == 0 || y == 0) continue;
                    int n = y * Columns.Length + x;
                    vh.AddTriangle(n - Columns.Length - 1, n - Columns.Length, n);
                    vh.AddTriangle(n - Columns.Length - 1, n, n - 1);
                }
            }
        }
    }
}
