using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>Anchors the Splash header to the safe top and continues its footer shade to the display edge.</summary>
    [ExecuteAlways, DisallowMultipleComponent]
    public sealed class SplashTitleForegroundLayout : MonoBehaviour
    {
        public RectTransform Logo;
        public RectTransform Version;
        public RectTransform Language;
        public SplashTitleFooterShade FooterShade;
        [SerializeField] Vector2 logoPosition;
        [SerializeField] Vector2 versionPosition;
        [SerializeField] Vector2 languagePosition;
        [SerializeField] float footerTop;
        [SerializeField] bool authoredLayoutCaptured;
        readonly Vector3[] corners = new Vector3[4];

        public bool HasRequiredBindings => authoredLayoutCaptured && Logo != null && Version != null &&
            Language != null && FooterShade != null && Logo.parent == transform &&
            Version.parent == transform && Language.parent == transform && FooterShade.transform.parent == transform;

        // Called only by the prefab owner, before placing the authored prefab inside a Canvas.
        public void CaptureAuthoredLayout()
        {
            if (Logo == null || Version == null || Language == null || FooterShade == null) return;
            logoPosition = Logo.anchoredPosition;
            versionPosition = Version.anchoredPosition;
            languagePosition = Language.anchoredPosition;
            footerTop = -FooterShade.rectTransform.anchoredPosition.y;
            FooterShade.FadeSpan = FooterShade.rectTransform.sizeDelta.y;
            authoredLayoutCaptured = true;
        }

        // Run before the Canvas graphic rebuild so the new shade bounds render in the same frame.
        void OnEnable() { Canvas.preWillRenderCanvases += ApplyNow; ApplyNow(); }
        void OnDisable() { Canvas.preWillRenderCanvases -= ApplyNow; }
        void OnRectTransformDimensionsChange() { ApplyNow(); }

        public void ApplyNow()
        {
            if (!HasRequiredBindings) return;
            RectTransform root = transform as RectTransform;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (root == null || canvas == null) return;
            RectTransform canvasRect = canvas.rootCanvas.transform as RectTransform;
            if (canvasRect == null) return;
            SafeAreaFitter safe = GetComponentInParent<SafeAreaFitter>();
            RectTransform safeRect = safe != null ? safe.transform as RectTransform : canvasRect;
            if (safeRect == null) return;
            safeRect.GetWorldCorners(corners);
            float gap = Mathf.Max(0f, root.InverseTransformPoint(corners[2]).y - root.rect.yMax);
            Vector2 shift = Vector2.up * gap;
            SetPosition(Logo, logoPosition + shift);
            SetPosition(Version, versionPosition + shift);
            SetPosition(Language, languagePosition + shift);

            canvasRect.GetWorldCorners(corners);
            Vector3 lo = root.InverseTransformPoint(corners[0]);
            Vector3 hi = root.InverseTransformPoint(corners[2]);
            float top = root.rect.yMax - footerTop;
            Vector2 size = new Vector2(hi.x - lo.x, top - lo.y);
            if (size.x < 1f || size.y < 1f) return;
            RectTransform shade = FooterShade.rectTransform;
            shade.anchorMin = shade.anchorMax = shade.pivot = new Vector2(0, 1);
            SetPosition(shade, new Vector2(lo.x - root.rect.xMin, -footerTop));
            if ((shade.sizeDelta - size).sqrMagnitude > .0001f) shade.sizeDelta = size;
        }

        static void SetPosition(RectTransform rect, Vector2 position)
        {
            if ((rect.anchoredPosition - position).sqrMagnitude > .0001f) rect.anchoredPosition = position;
        }
    }
}
