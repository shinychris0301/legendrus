using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Runtime loader for explicitly gated meta/progression production pages.
    /// The visual prefab stays presentation-only; ScreenRouter retains all
    /// authoritative navigation, purchase, ad, reward, and economy callbacks.
    /// </summary>
    public static class ProductionMetaPageRuntime
    {
        public static bool TryBuild(ScreenId id, RectTransform parent)
        {
            if (parent == null || !ProductionMetaPageCutoverRegistry.IsCutover(id))
                return false;

            // Splash/Title loads the canonical serialized prefab with direct generated sprites,
            // live labels and real Buttons. ScreenRouter binds its existing destination routes.
            if (id == ScreenId.SplashTitle)
                return ProductionSplashTitleRuntime.TryBuild(parent);

            string key = ProductionMetaPageCutoverRegistry.ResourceKey(id);
            GameObject prefab = string.IsNullOrEmpty(key) ? null : Resources.Load<GameObject>(key);
            if (prefab == null)
            {
                Debug.LogError("Active meta-page cutover is missing its production prefab for " + id + ": " + key + ". Rendering a screenshot-free emergency surface.");
                BuildEmergencySurface(id, parent);
                return true;
            }

            GameObject instance = Object.Instantiate(prefab, parent, false);
            instance.name = "ProductionMetaDesign_" + id;
            RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect != null)
                Stretch(rect);

            ProductionCoreLoopPageMarker marker = instance.GetComponent<ProductionCoreLoopPageMarker>();
            if (marker == null || marker.ScreenId != id || !marker.ScreenshotFree)
                Debug.LogError("Production meta-page marker mismatch for " + id + ".");
            return true;
        }

        static void BuildEmergencySurface(ScreenId id, RectTransform parent)
        {
            GameObject root = new GameObject("ProductionMetaEmergency_" + id, typeof(RectTransform), typeof(Image));
            root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>();
            Stretch(rect);
            Image image = root.GetComponent<Image>();
            image.color = new Color32(4, 14, 26, 255);
            image.raycastTarget = false;

            GameObject labelGo = new GameObject("Status", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(root.transform, false);
            RectTransform labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.10f, 0.40f);
            labelRect.anchorMax = new Vector2(0.90f, 0.60f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            Text label = labelGo.GetComponent<Text>();
            label.text = id + "\nPRODUCTION META PREFAB MISSING";
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
