using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Victory-only rolled gold rim and faceted, raised star face.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionVictoryStarDepth : BaseMeshEffect
    {
        const int Points = 10;
        const float FaceScale = .72f;

        public override void ModifyMesh(VertexHelper vh)
        {
            var star = graphic as ProductionWorldMapGraphic;
            if (!IsActive() || star == null || star.Kind != WorldMapGraphicKind.Star) return;
            vh.Clear();
            Rect rect = star.GetPixelAdjustedRect();
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * .48f;
            if (radius <= 0) return;

            // Finish each whole layer before the next: an offset shadow must never
            // overpaint the already finished face of a neighbouring star point.
            if (!star.Dimmed)
            {
                Color clear = new Color(1, .59f, .08f, 0);
                Color glow = new Color(1, .79f, .25f, .65f);
                Band(vh, star, center, radius, 1.10f, 1.015f, clear, clear, glow, glow);
            }
            Solid(vh, star, center + new Vector2(radius * .016f, -radius * .037f), radius,
                new Color32(99, 52, 10, 235));
            Band(vh, star, center, radius, 1, .971f,
                new Color32(164, 87, 13, 255), new Color32(255, 223, 107, 255),
                new Color32(250, 189, 48, 255), new Color32(255, 247, 164, 255));
            Band(vh, star, center, radius, .971f, .916f,
                new Color32(255, 213, 69, 255), new Color32(255, 251, 189, 255),
                new Color32(247, 173, 27, 255), new Color32(255, 231, 104, 255));
            // A narrow warm groove separates the luminous lip from the broad bevel.
            Band(vh, star, center, radius, .916f, .879f,
                new Color32(129, 66, 9, 255), new Color32(209, 143, 31, 255),
                new Color32(149, 74, 8, 255), new Color32(233, 166, 39, 255));
            Band(vh, star, center, radius, .879f, FaceScale,
                new Color32(213, 122, 14, 255), new Color32(255, 232, 119, 255),
                new Color32(239, 162, 24, 255), new Color32(255, 217, 83, 255));
            if (star.Dimmed)
            {
                // Empty stars keep their inset lacquer face and subdued metal rim.
                Solid(vh, star, center, radius * FaceScale, new Color32(20, 27, 33, 255), false);
            }
            else Face(vh, star, center, radius * FaceScale);
        }

        static void Face(VertexHelper vh, ProductionWorldMapGraphic star, Vector2 center, float radius)
        {
            // Ten distinct gold planes meet at a raised apex. Deliberate hard
            // facet boundaries remain readable at phone size without black seams.
            for (int point = 0; point < Points; point++)
            {
                bool lit = point % 2 == 0;
                Color tip = lit ? new Color32(255, 224, 92, 255) : new Color32(211, 120, 12, 255);
                Color valley = lit ? new Color32(247, 181, 34, 255) : new Color32(164, 83, 8, 255);
                Color apex = lit ? new Color32(255, 249, 163, 255) : new Color32(249, 190, 49, 255);
                Tri(vh, center, center + Point(point, radius), center + Point(point + 1, radius),
                    Tone(star, apex), Tone(star, lit ? tip : valley), Tone(star, lit ? valley : tip));
            }
        }

        static void Band(VertexHelper vh, ProductionWorldMapGraphic star, Vector2 center, float radius,
            float outer, float inner, Color outerShade, Color outerLight, Color innerShade, Color innerLight)
        {
            for (int i = 0; i < Points; i++)
            {
                Vector2 a = Point(i, radius), b = Point(i + 1, radius);
                Color ao = Tone(star, Direction(outerShade, outerLight, a));
                Color bo = Tone(star, Direction(outerShade, outerLight, b));
                Color ai = Tone(star, Direction(innerShade, innerLight, a));
                Color bi = Tone(star, Direction(innerShade, innerLight, b));
                Tri(vh, center + a * outer, center + b * outer, center + b * inner, ao, bo, bi);
                Tri(vh, center + b * inner, center + a * inner, center + a * outer, bi, ai, ao);
            }
        }

        static Color Direction(Color shade, Color light, Vector2 p)
        {
            Vector2 direction = p.normalized;
            return Color.Lerp(shade, light, Mathf.Clamp01(.5f + (-direction.x + direction.y) * .34f));
        }

        static Color Tone(ProductionWorldMapGraphic star, Color input, bool dimMetal = true)
        {
            if (star.Dimmed && dimMetal)
            {
                float gray = input.grayscale;
                input = new Color(gray * .48f, gray * .42f, gray * .31f, input.a);
            }
            input.a *= star.color.a;
            return input;
        }

        static void Solid(VertexHelper vh, ProductionWorldMapGraphic star, Vector2 center, float radius,
            Color color, bool dimMetal = true)
        {
            color = Tone(star, color, dimMetal);
            for (int i = 0; i < Points; i++)
                Tri(vh, center, center + Point(i, radius), center + Point(i + 1, radius), color, color, color);
        }

        static Vector2 Point(int i, float radius)
        {
            // Wrap explicitly so both sides of the closing seam use identical data.
            i %= Points;
            float angle = Mathf.PI * .5f + i * Mathf.PI / 5;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * (i % 2 == 0 ? 1 : .44f);
        }

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
