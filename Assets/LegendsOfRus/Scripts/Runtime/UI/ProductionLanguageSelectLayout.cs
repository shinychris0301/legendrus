using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class ProductionLanguageSelectLayout : MonoBehaviour
    {
        public ProductionLanguageSelectView View;
        readonly Vector3[] corners = new Vector3[4];
        bool applying;
        void OnEnable() { Canvas.preWillRenderCanvases += ApplyNow; ApplyNow(); }
        void OnDisable() { Canvas.preWillRenderCanvases -= ApplyNow; }
        void OnRectTransformDimensionsChange() { ApplyNow(); }
        public void ApplyNow()
        {
            if (applying || !View || !View.HasRequiredBindings) return;
            var canvas = GetComponentInParent<Canvas>(); if (!canvas) return;
            var root = (RectTransform)transform;
            var whole = (RectTransform)canvas.rootCanvas.transform;
            var fitter = GetComponentInParent<SafeAreaFitter>();
            var safe = fitter ? (RectTransform)fitter.transform : whole;
            applying = true;
            try
            {
                safe.GetWorldCorners(corners);
                var low = root.InverseTransformPoint(corners[0]); var high = root.InverseTransformPoint(corners[2]);
                float scale = Mathf.Max(.1f, Mathf.Min(1.35f, (high.x-low.x-32)/621, (high.y-low.y-48)/500));
                View.Body.localScale = Vector3.one * scale;
                View.Body.anchoredPosition = new Vector2((low.x+high.x-941*scale)*.5f-root.rect.xMin, (low.y+high.y+1672*scale)*.5f-root.rect.yMax);
                whole.GetWorldCorners(corners);
                low = root.InverseTransformPoint(corners[0]); high = root.InverseTransformPoint(corners[2]);
                View.Shield.sizeDelta = new Vector2(high.x-low.x, high.y-low.y);
                View.Shield.anchoredPosition = new Vector2(low.x-root.rect.xMin, high.y-root.rect.yMax);
            }
            finally { applying = false; }
        }
    }
}
