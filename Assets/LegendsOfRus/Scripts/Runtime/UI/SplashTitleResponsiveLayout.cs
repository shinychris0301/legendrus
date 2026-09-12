using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Existing AdaptiveCanvasScaler/SafeAreaFitter keep the control stage at 941x1672.
    /// Only the environment is extended to cover the full Canvas, including display insets.
    /// </summary>
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class SplashTitleResponsiveLayout : MonoBehaviour
    {
        public Image Background;
        readonly Vector3[] corners = new Vector3[4];
        void OnEnable() { Canvas.willRenderCanvases += ApplyNow; ApplyNow(); }
        void OnDisable() { Canvas.willRenderCanvases -= ApplyNow; }
        public void ApplyNow()
        {
            if (Background == null || Background.sprite == null) return;
            RectTransform root = transform as RectTransform;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (root == null || canvas == null) return;
            RectTransform canvasRect = canvas.rootCanvas.transform as RectTransform;
            if (canvasRect == null) return;
            canvasRect.GetWorldCorners(corners);
            Vector3 lo = root.InverseTransformPoint(corners[0]);
            Vector3 hi = root.InverseTransformPoint(corners[2]);
            float width = Mathf.Abs(hi.x - lo.x), height = Mathf.Abs(hi.y - lo.y);
            if (width < 1f || height < 1f) return;
            Vector2 native = Background.sprite.rect.size;
            float scale = Mathf.Max(width / native.x, height / native.y);
            Vector2 size = native * scale;
            Vector2 center = new Vector2((lo.x + hi.x) * .5f, (lo.y + hi.y) * .5f) - root.rect.center;
            RectTransform rt = Background.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
            if ((rt.anchoredPosition - center).sqrMagnitude > .0001f) rt.anchoredPosition = center;
            if ((rt.sizeDelta - size).sqrMagnitude > .0001f) rt.sizeDelta = size;
        }
    }
}
