using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Native lacquer panel and antique-gold frame; also supplies circular stencil masks.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionHomePanel : MaskableGraphic
    {
        public float Radius = 13f;
        public bool Frame = true;
        public bool CornerJewels;
        public bool RaisedBadge;
        public Color LeftColor = new Color32(15, 29, 42, 255);
        public Color RightColor = new Color32(9, 19, 32, 255);
        public Color BorderColor = new Color32(176, 140, 80, 255);

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            if (r.width <= 0 || r.height <= 0) return;
            if (RaisedBadge) { DrawRaisedBadge(vh, r); return; }
            if (Frame)
            {
                Rounded(vh, r, Radius, BorderColor, BorderColor);
                Rounded(vh, Inset(r, 2), Mathf.Max(0, Radius - 2), new Color32(7, 15, 25, 255), new Color32(7, 15, 25, 255));
                Rounded(vh, Inset(r, 5), Mathf.Max(0, Radius - 5), new Color32(75, 58, 36, 255), new Color32(75, 58, 36, 255));
                Rounded(vh, Inset(r, 6), Mathf.Max(0, Radius - 6), LeftColor, RightColor);
            }
            else Rounded(vh, r, Radius, LeftColor, RightColor);
            if (CornerJewels)
            {
                Diamond(vh, new Vector2(r.xMin + 17, r.yMax - 17), 10, BorderColor);
                Diamond(vh, new Vector2(r.xMin + 17, r.yMax - 17), 9, new Color32(22, 43, 66, 255));
                Diamond(vh, new Vector2(r.xMax - 17, r.yMax - 17), 10, BorderColor);
                Diamond(vh, new Vector2(r.xMax - 17, r.yMax - 17), 9, new Color32(22, 43, 66, 255));
            }
        }

        // Badge-only native geometry. Lighting is authored in UI space, so the small
        // ruby dome and rolled metal rim remain legible at either phone scale.
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
                new Color32(85, 14, 6, 255), new Color32(51, 6, 4, 255),
                RubyColor(Vector2.up), RubyColor(Vector2.down));

            const int rings = 14;
            int origin = vh.currentVertCount;
            vh.AddVert(center, RubyColor(Vector2.zero) * color, Vector2.zero);
            for (int ring = 1; ring <= rings; ring++)
            {
                int start = vh.currentVertCount;
                for (int segment = 0; segment < BadgeSegments; segment++)
                {
                    float angle = segment * 2 * Mathf.PI / BadgeSegments;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (ring / (float)rings);
                    vh.AddVert(center + point * (.78f * radius), RubyColor(point) * color, Vector2.zero);
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

        static Color RubyColor(Vector2 point)
        {
            Vector2 slope = point * .82f;
            Vector3 normal = new Vector3(slope.x, slope.y, Mathf.Sqrt(Mathf.Max(0, 1 - slope.sqrMagnitude)));
            float diffuse = Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-.4f, .55f, .7331439f)));
            Color ruby = Color.Lerp(new Color32(46, 2, 6, 255), new Color32(204, 33, 21, 255), diffuse * diffuse);
            float highlight = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-.25f, .37f, .8947625f))), 70);
            return Color.Lerp(ruby, new Color32(255, 203, 159, 255), highlight * .73f);
        }

        static Rect Inset(Rect r, float d) => new Rect(r.x + d, r.y + d, Mathf.Max(0, r.width - d * 2), Mathf.Max(0, r.height - d * 2));
        void Rounded(VertexHelper vh, Rect r, float radius, Color left, Color right)
        {
            radius = Mathf.Clamp(radius, 0, Mathf.Min(r.width, r.height) * .5f);
            int start = vh.currentVertCount;
            vh.AddVert(r.center, Color.Lerp(left, right, .5f) * color, Vector2.zero);
            const int steps = 10;
            for (int corner = 0; corner < 4; corner++)
            {
                Vector2 center = new Vector2(corner == 0 || corner == 3 ? r.xMax - radius : r.xMin + radius,
                    corner < 2 ? r.yMax - radius : r.yMin + radius);
                for (int step = 0; step <= steps; step++)
                {
                    float angle = (corner * 90f + step * 90f / steps) * Mathf.Deg2Rad;
                    Vector2 p = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    vh.AddVert(p, Color.Lerp(left, right, Mathf.InverseLerp(r.xMin, r.xMax, p.x)) * color, Vector2.zero);
                }
            }
            int count = 4 * (steps + 1);
            for (int i = 0; i < count; i++) vh.AddTriangle(start, start + 1 + i, start + 1 + (i + 1) % count);
        }

        void Diamond(VertexHelper vh, Vector2 center, float size, Color tint)
        {
            int n = vh.currentVertCount;
            vh.AddVert(center + Vector2.up * size, tint * color, Vector2.zero);
            vh.AddVert(center + Vector2.left * size, tint * color, Vector2.zero);
            vh.AddVert(center + Vector2.down * size, tint * color, Vector2.zero);
            vh.AddVert(center + Vector2.right * size, tint * color, Vector2.zero);
            vh.AddTriangle(n, n + 1, n + 2); vh.AddTriangle(n, n + 2, n + 3);
        }
    }
}
