using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Victory-only warm glow, tapered rays and fixed, sparse sparkle detail.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionVictoryStarHalo : BaseMeshEffect
    {
        public int Seed;
        static readonly float[] Rings = { 0, .33f, .65f, 1 };
        static readonly float[] Opacity = { .84f, .59f, .18f, 0 };

        public override void ModifyMesh(VertexHelper vh)
        {
            var halo = graphic as ProductionWorldMapGraphic;
            if (!IsActive() || halo == null || halo.Kind != WorldMapGraphicKind.Halo) return;
            vh.Clear();
            // The existing Victory view owns earned visibility and the one-shot
            // reveal alpha. Unlike the shared map halo, this mesh honors that alpha.
            float alpha = Mathf.Clamp01(halo.color.a);
            if (halo.Dimmed || alpha <= 0) return;
            Rect rect = halo.GetPixelAdjustedRect();
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * .48f;
            if (radius <= 0) return;

            Glow(vh, center, radius, alpha);
            for (int i = 0; i < 48; i++)
            {
                float angle = i * Mathf.PI * 2 / 48 + Seed * .47f + .07f * Mathf.Sin(i * 2.4f);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 normal = new Vector2(-direction.y, direction.x);
                float length = radius * (.71f + .26f * Fraction(i * .618f + Seed * .137f));
                Vector2 start = center + direction * radius * .18f;
                Vector2 peak = center + direction * radius * .44f;
                Vector2 tip = center + direction * length;
                float width = radius * (i % 3 == 0 ? .017f : .008f);
                Color clear = new Color(1, .60f, .12f, 0);
                Color warm = new Color(1, .70f, .20f, alpha * (i % 3 == 0 ? .85f : .55f));
                Tri(vh, start, peak + normal * width, peak, clear, clear, warm);
                Tri(vh, start, peak, peak - normal * width, clear, warm, clear);
                Tri(vh, peak + normal * width, tip, peak, clear, clear, warm);
                Tri(vh, peak, tip, peak - normal * width, warm, clear, clear);
            }
            for (int i = 0; i < 28; i++)
            {
                float angle = i * 2.39996f + Seed * .79f;
                float distance = radius * (.62f + .29f * Fraction(i * .3819f + Seed * .17f));
                Vector2 p = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                Spark(vh, p, radius * (i % 3 == 0 ? .030f : .018f), alpha);
            }
        }

        static void Glow(VertexHelper vh, Vector2 center, float radius, float alpha)
        {
            const int segments = 48;
            // A bright inner cloud settles into an almost invisible outer edge.
            for (int ring = 1; ring < Rings.Length; ring++)
            {
                Color inner = new Color(1, .72f + .08f * Rings[ring - 1],
                    .16f + .09f * Rings[ring - 1], Opacity[ring - 1] * alpha);
                Color outer = new Color(1, .72f + .08f * Rings[ring],
                    .16f + .09f * Rings[ring], Opacity[ring] * alpha);
                for (int i = 0; i < segments; i++)
                {
                    float aa = i * Mathf.PI * 2 / segments, ab = (i + 1) * Mathf.PI * 2 / segments;
                    Vector2 a = new Vector2(Mathf.Cos(aa), Mathf.Sin(aa)) * radius;
                    Vector2 b = new Vector2(Mathf.Cos(ab), Mathf.Sin(ab)) * radius;
                    Tri(vh, center + a * Rings[ring - 1], center + a * Rings[ring],
                        center + b * Rings[ring], inner, outer, outer);
                    if (ring > 1)
                        Tri(vh, center + b * Rings[ring], center + b * Rings[ring - 1],
                            center + a * Rings[ring - 1], outer, inner, inner);
                }
            }
        }

        static void Spark(VertexHelper vh, Vector2 p, float size, float alpha)
        {
            Color light = new Color(1, .93f, .62f, alpha * .95f);
            Color clear = new Color(1, .69f, .20f, 0);
            Vector2 up = Vector2.up * size * 1.9f, right = Vector2.right * size * 1.25f;
            Vector2 neck = new Vector2(size * .18f, size * .18f);
            Tri(vh, p + up, p + neck, p, clear, clear, light);
            Tri(vh, p, p + neck, p + right, light, clear, clear);
            Tri(vh, p + right, p + new Vector2(neck.x, -neck.y), p, clear, clear, light);
            Tri(vh, p, p + new Vector2(neck.x, -neck.y), p - up, light, clear, clear);
            Tri(vh, p - up, p - neck, p, clear, clear, light);
            Tri(vh, p, p - neck, p - right, light, clear, clear);
            Tri(vh, p - right, p + new Vector2(-neck.x, neck.y), p, clear, clear, light);
            Tri(vh, p, p + new Vector2(-neck.x, neck.y), p + up, light, clear, clear);
        }

        static float Fraction(float value) => value - Mathf.Floor(value);

        static void Tri(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Color ca, Color cb, Color cc)
        {
            int n = vh.currentVertCount;
            vh.AddVert(a, ca, Vector2.zero);
            vh.AddVert(b, cb, Vector2.zero);
            vh.AddVert(c, cc, Vector2.zero);
            vh.AddTriangle(n, n + 1, n + 2);
        }
    }
}
