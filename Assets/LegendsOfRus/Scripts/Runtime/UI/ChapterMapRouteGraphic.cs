using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ChapterMapRouteGraphic : MaskableGraphic
    {
        public ProductionChapterMapScroller Scroller;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (!Scroller || !Scroller.View) return;
            var nodes = Scroller.View.Nodes;
            for (int i = 1; i < nodes.Length; i++)
            {
                if (nodes[i].LevelId > ProductionChapterMapScroller.PlannedLevels) continue;
                Vector2 a = nodes[i - 1].Root.anchoredPosition + new Vector2(63, -50);
                Vector2 b = nodes[i].Root.anchoredPosition + new Vector2(63, -50);
                Color tint = new Color(0.85f, 0.66f, 0.31f, nodes[i].Lock.gameObject.activeSelf ? .34f : .70f);
                int count = Mathf.CeilToInt(Vector2.Distance(a, b) / 12);
                for (int j = 1; j < count; j++)
                {
                    float t = j / (float)count;
                    Vector2 point = new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, Mathf.SmoothStep(0, 1, t)));
                    if (Vector2.Distance(point, a) < 65 || Vector2.Distance(point, b) < 65) continue;
                    int first = vh.currentVertCount;
                    const float r = 2;
                    vh.AddVert(point + Vector2.up * r, tint, Vector2.zero);
                    vh.AddVert(point + Vector2.right * r, tint, Vector2.zero);
                    vh.AddVert(point + Vector2.down * r, tint, Vector2.zero);
                    vh.AddVert(point + Vector2.left * r, tint, Vector2.zero);
                    vh.AddTriangle(first, first + 1, first + 2);
                    vh.AddTriangle(first, first + 2, first + 3);
                }
            }
        }
    }
}
