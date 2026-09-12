using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>Extends Loading's lower contrast wash through letterboxing and display insets.</summary>
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class ProductionLoadingShadeLayout : MonoBehaviour
    {
        public ProductionLoadingShade Shade;
        readonly Vector3[] corners = new Vector3[4];

        void OnEnable() { Canvas.willRenderCanvases += ApplyNow; ApplyNow(); }
        void OnDisable() { Canvas.willRenderCanvases -= ApplyNow; }

        public void ApplyNow()
        {
            if (Shade == null) return;
            RectTransform parent = Shade.rectTransform.parent as RectTransform;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (parent == null || canvas == null) return;
            RectTransform canvasRect = canvas.rootCanvas.transform as RectTransform;
            if (canvasRect == null) return;
            canvasRect.GetWorldCorners(corners);
            Vector3 lo = parent.InverseTransformPoint(corners[0]);
            Vector3 hi = parent.InverseTransformPoint(corners[2]);
            // Preserve the authored fade start; only extend the bottom and side edges.
            float top = parent.rect.yMax - 1110f;
            Vector2 size = new Vector2(hi.x - lo.x, top - lo.y);
            if (size.x < 1f || size.y < 1f) return;
            RectTransform rect = Shade.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            Vector2 position = new Vector2(lo.x - parent.rect.xMin, -1110f);
            if ((rect.anchoredPosition - position).sqrMagnitude > .0001f) rect.anchoredPosition = position;
            if ((rect.sizeDelta - size).sqrMagnitude > .0001f) rect.sizeDelta = size;
        }
    }
}
