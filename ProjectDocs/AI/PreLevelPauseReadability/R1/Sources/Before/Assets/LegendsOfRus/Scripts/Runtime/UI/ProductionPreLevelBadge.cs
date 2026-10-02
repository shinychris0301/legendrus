using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Source-matched forest-green stock badge with antique-gold edging.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionPreLevelBadge : MaskableGraphic
    {
        public static void AlignCount(Text label, RectTransform badge)
        {
            if(label==null||badge==null)return;
            var rect=label.rectTransform;
            if(rect.parent!=badge)rect.SetParent(badge,false);
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
            // Keep wide inventory values inside the circular face, not merely the square bounds.
            rect.anchoredPosition=new Vector2(badge.sizeDelta.x*.14f,0);
            rect.sizeDelta=new Vector2(badge.sizeDelta.x*1.44f,badge.sizeDelta.y*2);
            rect.localScale=new Vector3(.5f,.5f,1);rect.localRotation=Quaternion.identity;
            foreach(var effect in label.GetComponents<BaseMeshEffect>())effect.enabled=false;
            var geometry=label.GetComponent<SplashTitleTextGeometry>()??label.gameObject.AddComponent<SplashTitleTextGeometry>();
            geometry.enabled=true;geometry.CenterHorizontally=geometry.CenterVertically=true;
            geometry.SnapToScreenPixels=false;geometry.OpticalOffset=Vector2.zero;
            var font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            if(font!=null)label.font=font;
            label.fontStyle=FontStyle.Normal;label.material=null;label.color=new Color32(247,235,187,255);
            label.alignment=TextAnchor.MiddleCenter;label.horizontalOverflow=HorizontalWrapMode.Wrap;
            label.verticalOverflow=VerticalWrapMode.Truncate;label.raycastTarget=false;
            label.resizeTextForBestFit=true;label.resizeTextMinSize=Mathf.RoundToInt(badge.sizeDelta.y*.64f);
            label.resizeTextMaxSize=label.fontSize=Mathf.RoundToInt(badge.sizeDelta.y*1.4f);
            label.SetVerticesDirty();
        }
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
                new Color32(240, 231, 173, 255), new Color32(127, 84, 26, 255));
            BadgeBand(vh, center, radius, .967f, .895f,
                new Color32(240, 231, 173, 255), new Color32(127, 84, 26, 255),
                new Color32(172, 134, 59, 255), new Color32(74, 44, 12, 255));
            BadgeBand(vh, center, radius, .895f, .85f,
                new Color32(172, 134, 59, 255), new Color32(74, 44, 12, 255),
                new Color32(69, 26, 6, 255), new Color32(38, 11, 3, 255));
            BadgeBand(vh, center, radius, .85f, .807f,
                new Color32(49, 39, 13, 255), new Color32(106, 78, 26, 255),
                new Color32(80, 63, 21, 255), new Color32(145, 116, 51, 255));
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
            Color face = Color.Lerp(new Color32(3, 20, 2, 255), new Color32(22, 57, 7, 255), diffuse * diffuse);
            float highlight = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-.25f, .37f, .8947625f))), 70);
            return Color.Lerp(face, new Color32(99, 124, 45, 255), highlight * .22f);
        }

    }
}
