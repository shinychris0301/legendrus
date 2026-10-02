using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Emerald stock dome using the accepted Home badge lighting geometry.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionPreLevelBadge : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect rect=GetPixelAdjustedRect();
            if(rect.width>0&&rect.height>0)DrawRaisedBadge(vh,rect);
        }
        const int BadgeSegments = 72;
        void DrawRaisedBadge(VertexHelper vh, Rect rect)
        {
            float radius = Mathf.Min(rect.width, rect.height) * .5f;
            Vector2 center = rect.center;
            Vector2 shadowCenter = center + new Vector2(.045f, -.095f) * radius;
            for (int i = 0; i < 5; i++)
            {
                float outer = 1.19f - i * .05f, inner = outer - .05f;
                Color a = new Color(0.035f, .012f, .008f, i * i * .025f);
                Color b = new Color(a.r, a.g, a.b, (i + 1) * (i + 1) * .025f);
                BadgeBand(vh, shadowCenter, radius, outer, inner, a, a, b, b);
            }
            Color contact = new Color(.035f, .012f, .008f, .625f);
            BadgeBand(vh, shadowCenter, radius, .94f, 0, contact, contact, contact, contact);
            // Alternating lit shoulders and shaded grooves model a rolled gold rim.
            Color32 dark = new Color32(49, 20, 6, 255);
            BadgeBand(vh, center, radius, 1.015f, 1f,
                new Color32(129, 76, 21, 0), new Color32(35, 12, 3, 0),
                new Color32(194, 129, 43, 255), dark);
            BadgeBand(vh, center, radius, 1f, .967f,
                new Color32(194, 129, 43, 255), dark,
                new Color32(255, 242, 190, 255), new Color32(167, 95, 24, 255));
            BadgeBand(vh, center, radius, .967f, .895f,
                new Color32(255, 242, 190, 255), new Color32(167, 95, 24, 255),
                new Color32(205, 145, 49, 255), new Color32(96, 43, 9, 255));
            BadgeBand(vh, center, radius, .895f, .85f,
                new Color32(205, 145, 49, 255), new Color32(96, 43, 9, 255),
                new Color32(69, 26, 6, 255), new Color32(38, 11, 3, 255));
            BadgeBand(vh, center, radius, .85f, .807f,
                new Color32(100, 45, 11, 255), new Color32(221, 146, 47, 255),
                new Color32(176, 101, 22, 255), new Color32(255, 214, 121, 255));
            BadgeBand(vh, center, radius, .807f, .78f,
                new Color32(4, 32, 14, 255), new Color32(2, 16, 8, 255),
                EmeraldColor(Vector2.up), EmeraldColor(Vector2.down));

            const int rings = 14;
            int origin = vh.currentVertCount;
            vh.AddVert(center, EmeraldColor(Vector2.zero) * color, Vector2.zero);
            for (int ring = 1; ring <= rings; ring++)
            {
                int start = vh.currentVertCount;
                for (int segment = 0; segment < BadgeSegments; segment++)
                {
                    float angle = segment * 2 * Mathf.PI / BadgeSegments;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (ring / (float)rings);
                    vh.AddVert(center + point * (.78f * radius), EmeraldColor(point) * color, Vector2.zero);
                }
                for (int segment = 0; segment < BadgeSegments; segment++)
                {
                    int next = (segment + 1) % BadgeSegments;
                    if (ring == 1) vh.AddTriangle(origin, start + segment, start + next);
                    else
                    {
                        int previous = start - BadgeSegments;
                        vh.AddTriangle(previous + segment, start + segment, start + next);
                        vh.AddTriangle(previous + segment, start + next, previous + next);
                    }
                }
            }
        }

        void BadgeBand(VertexHelper vh, Vector2 center, float radius, float outer, float inner,
            Color upperOuter, Color lowerOuter, Color upperInner, Color lowerInner)
        {
            int start = vh.currentVertCount;
            for (int i = 0; i <= BadgeSegments; i++)
            {
                float angle = i * 2 * Mathf.PI / BadgeSegments;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float light = Mathf.Clamp01(.5f + .5f * Vector2.Dot(direction, new Vector2(-.6f, .8f)));
                vh.AddVert(center + direction * (radius * outer), Color.Lerp(lowerOuter, upperOuter, light) * color, Vector2.zero);
                vh.AddVert(center + direction * (radius * inner), Color.Lerp(lowerInner, upperInner, light) * color, Vector2.zero);
                if (i == 0) continue;
                int n = start + i * 2;
                vh.AddTriangle(n - 2, n, n + 1);
                vh.AddTriangle(n - 2, n + 1, n - 1);
            }
        }

        static Color EmeraldColor(Vector2 point)
        {
            Vector2 slope = point * .82f;
            Vector3 normal = new Vector3(slope.x, slope.y, Mathf.Sqrt(Mathf.Max(0, 1 - slope.sqrMagnitude)));
            float diffuse = Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-.4f, .55f, .7331439f)));
            Color ruby = Color.Lerp(new Color32(3, 30, 14, 255), new Color32(28, 131, 55, 255), diffuse * diffuse);
            float highlight = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-.25f, .37f, .8947625f))), 70);
            return Color.Lerp(ruby, new Color32(205, 240, 169, 255), highlight * .73f);
        }

    }
}
