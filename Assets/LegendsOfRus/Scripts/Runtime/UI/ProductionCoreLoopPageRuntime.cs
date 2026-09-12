using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Loads reconstructed core-loop prefabs. A missing prefab never falls back
    /// to a DesignScreens screenshot; a small screenshot-free emergency surface
    /// is created instead so the cutover boundary remains truthful.
    /// </summary>
    public static class ProductionCoreLoopPageRuntime
    {
        public static bool TryBuild(ScreenId id, RectTransform parent)
        {
            if (!ProductionCoreLoopPageRegistry.IsCutover(id))
                return false;

            string key = ProductionCoreLoopPageRegistry.ResourceKey(id);
            GameObject prefab = HomeLoadingPreparation.LoadPreparedPrefab(key) ??
                Resources.Load<GameObject>(key);
            if (prefab == null)
            {
                Debug.LogError("Missing serialized production UI prefab for " + id + ": " + key + ". Building FIX01 asset-based runtime fallback.");
                if (ProductionUiFix01Runtime.TryBuild(id, parent))
                    return true;
                BuildEmergencySurface(id, parent);
                return true;
            }

            GameObject instance = Object.Instantiate(prefab, parent, false);
            instance.name = "ProductionDesign_" + id;
            RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect != null)
                Stretch(rect);

            ProductionUiFix01PageBootstrap fix01 = instance.GetComponent<ProductionUiFix01PageBootstrap>();
            if (fix01 != null)
            {
                if (fix01.ScreenId != id)
                    Debug.LogError("FIX01 production page bootstrap ScreenId mismatch for " + id + ".");
                return true;
            }

            ProductionCoreLoopPageMarker marker = instance.GetComponent<ProductionCoreLoopPageMarker>();
            if (marker == null || !marker.ScreenshotFree || marker.ScreenId != id)
                Debug.LogError("Production page marker mismatch for " + id + ".");
            return true;
        }

        static void BuildEmergencySurface(ScreenId id, RectTransform parent)
        {
            GameObject root = new GameObject("ProductionEmergency_" + id, typeof(RectTransform), typeof(Image));
            root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>();
            Stretch(rect);
            Image image = root.GetComponent<Image>();
            image.color = new Color32(4, 14, 26, 255);
            image.raycastTarget = false;

            GameObject labelGo = new GameObject("Status", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(root.transform, false);
            RectTransform labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.12f, 0.40f);
            labelRect.anchorMax = new Vector2(0.88f, 0.60f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            Text label = labelGo.GetComponent<Text>();
            label.text = id + "\nPRODUCTION UI PREFAB MISSING";
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (label.font == null)
                label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 30;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color32(244, 211, 137, 255);
            label.raycastTarget = false;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
