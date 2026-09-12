using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>Loads the canonical serialized Splash, never the legacy screenshot crops.</summary>
    public static class ProductionSplashTitleRuntime
    {
        public const string PrefabResource = "ProductionUI/Pages/SplashTitle";

        public static bool TryBuild(RectTransform parent)
        {
            if (parent == null) return false;
            GameObject prefab = Resources.Load<GameObject>(PrefabResource);
            SplashTitleView source = prefab != null ? prefab.GetComponent<SplashTitleView>() : null;
            if (prefab == null || source == null || !source.HasRequiredBindings ||
                prefab.GetComponent<RectTransform>() == null)
            {
                string reason = prefab == null
                    ? "Resource not found: " + PrefabResource
                    : source == null ? "Prefab loaded, but its root SplashTitleView is missing. Check script compilation and .meta GUIDs."
                    : !source.HasRequiredBindings ? source.GetValidationFailure()
                    : "Prefab root RectTransform is missing.";
                Debug.LogError("SplashTitle is not ready: " + reason + ". " +
                    "Stop Play Mode, then run Legends of Rus > Production UI > Splash R3 > Build or Repair. " +
                    "After repair run Validate PrefabFix07 before pressing Play again. " +
                    "Legacy screenshot layers are intentionally not used as a fallback.");
                return false;
            }
            GameObject instance = Object.Instantiate(prefab, parent, false);
            instance.name = "ProductionSplashTitle";
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            SplashTitleResponsiveLayout layout = instance.GetComponent<SplashTitleResponsiveLayout>();
            if (layout != null) layout.ApplyNow();
            return true;
        }
    }
}
