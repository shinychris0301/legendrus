using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Restrained 2.5D life for the approved Splash/Title composition.
    /// The base hero/firebird silhouettes remain essentially locked to the reference;
    /// only small overlays, glows and sub-pixel motion are animated so fidelity is preserved.
    /// No Animator/package dependency and no gameplay time-scale dependency.
    /// </summary>
    public sealed class SplashTitleLivingMotion : MonoBehaviour
    {
        RectTransform environment, heroesBase, ivanLife, mashaLife, rim, firebird, birdAura, birdShimmer, logo, start;
        CanvasGroup ivanGroup, mashaGroup, rimGroup, birdAuraGroup, birdShimmerGroup, startGlowGroup, taglineGroup;
        Vector2 env0, ivan0, masha0, logo0, start0;
        Vector3 ivanScale0, mashaScale0, logoScale0, startScale0;

        public void Configure(
            RectTransform environmentLayer,
            RectTransform heroesBaseLayer,
            RectTransform ivanLifeLayer, CanvasGroup ivanLifeGroup,
            RectTransform mashaLifeLayer, CanvasGroup mashaLifeGroup,
            RectTransform rimLayer, CanvasGroup rimGlowGroup,
            RectTransform firebirdBaseLayer,
            RectTransform birdAuraLayer, CanvasGroup birdAuraCanvasGroup,
            RectTransform birdShimmerLayer, CanvasGroup birdShimmerCanvasGroup,
            RectTransform logoLayer,
            RectTransform startLayer, CanvasGroup startGlowCanvasGroup,
            CanvasGroup taglineCanvasGroup)
        {
            environment = environmentLayer;
            heroesBase = heroesBaseLayer;
            ivanLife = ivanLifeLayer; ivanGroup = ivanLifeGroup;
            mashaLife = mashaLifeLayer; mashaGroup = mashaLifeGroup;
            rim = rimLayer; rimGroup = rimGlowGroup;
            firebird = firebirdBaseLayer;
            birdAura = birdAuraLayer; birdAuraGroup = birdAuraCanvasGroup;
            birdShimmer = birdShimmerLayer; birdShimmerGroup = birdShimmerCanvasGroup;
            logo = logoLayer;
            start = startLayer; startGlowGroup = startGlowCanvasGroup;
            taglineGroup = taglineCanvasGroup;

            if (environment != null) env0 = environment.anchoredPosition;
            if (ivanLife != null) { ivan0 = ivanLife.anchoredPosition; ivanScale0 = ivanLife.localScale; }
            if (mashaLife != null) { masha0 = mashaLife.anchoredPosition; mashaScale0 = mashaLife.localScale; }
            if (logo != null) { logo0 = logo.anchoredPosition; logoScale0 = logo.localScale; }
            if (start != null) { start0 = start.anchoredPosition; startScale0 = start.localScale; }
        }

        void Update()
        {
            float t = Time.unscaledTime;

            // Background remains visually locked; the sub-pixel drift only prevents a dead still frame.
            if (environment != null)
                environment.anchoredPosition = env0 + new Vector2(Mathf.Sin(t * 0.09f) * 0.18f, Mathf.Sin(t * 0.07f) * 0.12f);

            // Upper-body overlays provide breathing/weight shift without moving the base silhouettes.
            if (ivanLife != null)
            {
                float b = Mathf.Sin(t * 0.68f);
                ivanLife.anchoredPosition = ivan0 + new Vector2(Mathf.Sin(t * 0.23f) * 0.22f, b * 0.52f);
                ivanLife.localScale = ivanScale0 * (1f + b * 0.0012f);
                ivanLife.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 0.19f) * 0.035f);
                if (ivanGroup != null) ivanGroup.alpha = 0.075f + (b + 1f) * 0.018f;
            }

            if (mashaLife != null)
            {
                float b = Mathf.Sin(t * 0.76f + 0.7f);
                mashaLife.anchoredPosition = masha0 + new Vector2(Mathf.Sin(t * 0.21f + 0.8f) * 0.18f, b * 0.45f);
                mashaLife.localScale = mashaScale0 * (1f + b * 0.0010f);
                mashaLife.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 0.18f + 0.45f) * 0.028f);
                if (mashaGroup != null) mashaGroup.alpha = 0.068f + (b + 1f) * 0.016f;
            }

            if (rimGroup != null)
                rimGroup.alpha = 0.055f + (Mathf.Sin(t * 0.42f) + 1f) * 0.018f;

            // The Firebird base remains fixed; aura/shimmer carry the magical movement.
            if (birdAuraGroup != null)
                birdAuraGroup.alpha = 0.25f + (Mathf.Sin(t * 0.82f) + 1f) * 0.08f;
            if (birdShimmerGroup != null)
                birdShimmerGroup.alpha = 0.13f + (Mathf.Sin(t * 1.48f + 0.4f) + 1f) * 0.07f;
            if (birdShimmer != null)
                birdShimmer.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 0.31f) * 0.06f);

            if (logo != null)
            {
                float p = Mathf.Sin(t * 0.43f);
                logo.anchoredPosition = logo0 + new Vector2(0f, p * 0.30f);
                logo.localScale = logoScale0 * (1f + p * 0.00045f);
            }

            if (start != null)
            {
                float p = (Mathf.Sin(t * 1.16f) + 1f) * 0.5f;
                start.anchoredPosition = start0 + new Vector2(0f, Mathf.Sin(t * 0.55f) * 0.22f);
                start.localScale = startScale0 * Mathf.Lerp(0.9995f, 1.0015f, p);
                if (startGlowGroup != null) startGlowGroup.alpha = Mathf.Lerp(0.12f, 0.30f, p);
            }

            if (taglineGroup != null)
                taglineGroup.alpha = 0.97f + Mathf.Sin(t * 0.55f) * 0.025f;
        }

        void OnDisable() => Restore();
        void OnDestroy() => Restore();

        void Restore()
        {
            if (environment != null) environment.anchoredPosition = env0;
            if (ivanLife != null) { ivanLife.anchoredPosition = ivan0; ivanLife.localScale = ivanScale0; ivanLife.localRotation = Quaternion.identity; }
            if (mashaLife != null) { mashaLife.anchoredPosition = masha0; mashaLife.localScale = mashaScale0; mashaLife.localRotation = Quaternion.identity; }
            if (logo != null) { logo.anchoredPosition = logo0; logo.localScale = logoScale0; }
            if (start != null) { start.anchoredPosition = start0; start.localScale = startScale0; }
        }
    }
}
