using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Expands native button input within its safe area without changing painted artwork.</summary>
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class ProductionControlTouchTarget : MaskableGraphic
    {
        public const float MinimumPixels = 44;
        public Button Owner;
        bool applying;
        readonly Vector3[] corners = new Vector3[4];

        public static ProductionControlTouchTarget Ensure(Button button)
        {
            var found = button.transform.Find("TouchTarget");
            var target = found != null ? found.GetComponent<ProductionControlTouchTarget>() :
                new GameObject("TouchTarget", typeof(RectTransform), typeof(CanvasRenderer), typeof(ProductionControlTouchTarget))
                    .GetComponent<ProductionControlTouchTarget>();
            target.transform.SetParent(button.transform, false);
            target.gameObject.layer = button.gameObject.layer;
            target.Owner = button;
            target.color = Color.clear;
            target.raycastTarget = true;
            target.rectTransform.localScale = Vector3.one;
            target.rectTransform.localRotation = Quaternion.identity;
            target.rectTransform.anchorMin = Vector2.zero;
            target.rectTransform.anchorMax = Vector2.one;
            target.rectTransform.offsetMin = target.rectTransform.offsetMax = Vector2.zero;
            target.ApplyNow();
            return target;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            canvasRenderer.cullTransparentMesh = false;
            Canvas.preWillRenderCanvases += ApplyNow;
            ApplyNow();
        }
        protected override void OnDisable()
        {
            Canvas.preWillRenderCanvases -= ApplyNow;
            base.OnDisable();
        }
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            ApplyNow();
        }
        public void ApplyNow()
        {
            if (applying || Owner == null || canvas == null) return;
            var root = canvas.rootCanvas;
            var camera = root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
            var ownerRect = (RectTransform)Owner.transform;
            Rect bounds = ScreenBounds(ownerRect, camera);
            if (bounds.width <= 0 || bounds.height <= 0 || ownerRect.rect.width <= 0 || ownerRect.rect.height <= 0) return;
            var safe = Owner.GetComponentInParent<SafeAreaFitter>();
            Rect limit = ScreenBounds(safe != null ? (RectTransform)safe.transform : (RectTransform)root.transform, camera);
            float left, right, bottom, top;
            Padding(bounds.xMin, bounds.xMax, limit.xMin, limit.xMax, out left, out right);
            Padding(bounds.yMin, bounds.yMax, limit.yMin, limit.yMax, out bottom, out top);
            float xPixels = bounds.width / ownerRect.rect.width, yPixels = bounds.height / ownerRect.rect.height;
            applying = true;
            try
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = Vector2.one * .5f;
                Vector2 low = new Vector2(-left / xPixels, -bottom / yPixels);
                Vector2 high = new Vector2(right / xPixels, top / yPixels);
                if ((rectTransform.offsetMin - low).sqrMagnitude > .0001f) rectTransform.offsetMin = low;
                if ((rectTransform.offsetMax - high).sqrMagnitude > .0001f) rectTransform.offsetMax = high;
            }
            finally { applying = false; }
        }
        static void Padding(float low, float high, float safeLow, float safeHigh, out float before, out float after)
        {
            float need = Mathf.Max(0, MinimumPixels - (high - low));
            float roomBefore = Mathf.Max(0, low - safeLow), roomAfter = Mathf.Max(0, safeHigh - high);
            before = Mathf.Min(need * .5f, roomBefore);
            after = Mathf.Min(need - before, roomAfter);
            before = Mathf.Min(need - after, roomBefore);
        }
        Rect ScreenBounds(RectTransform rect, Camera camera)
        {
            rect.GetWorldCorners(corners);
            Vector2 low = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            Vector2 high = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
            return Rect.MinMaxRect(low.x, low.y, high.x, high.y);
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // A transparent quad participates in native GraphicRaycaster ordering.
            // Pointer events bubble to Owner's existing Button and its guarded action.
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            vh.AddVert(new Vector3(r.xMin, r.yMin), Color.clear, Vector2.zero);
            vh.AddVert(new Vector3(r.xMin, r.yMax), Color.clear, Vector2.zero);
            vh.AddVert(new Vector3(r.xMax, r.yMax), Color.clear, Vector2.zero);
            vh.AddVert(new Vector3(r.xMax, r.yMin), Color.clear, Vector2.zero);
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}
