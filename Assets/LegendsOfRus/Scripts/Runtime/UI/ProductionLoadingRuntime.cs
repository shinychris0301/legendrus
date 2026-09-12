using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Shared owner of the serialized Loading composition and its asset-only runtime repair.</summary>
    public static class ProductionLoadingRuntime
    {
        public const string ArtRoot = "ProductionUI/LoadingAligned/";
        public const string LogoResource = "ProductionUI/SplashTitleReconstructed/legends_of_rus_ornate_crest";
        public const string TypographyResource = "ProductionUI/SplashTitleConfig/SplashTitleTypography";
        public const string ContentName = "LOADING_ALIGNED_CONTENT";

        public static void BuildInto(RectTransform root, Func<string, Sprite> loadSprite = null,
            SplashTitleTypography typography = null)
        {
            ProductionLoadingView existing = root.GetComponent<ProductionLoadingView>();
            if (existing != null && existing.HasRequiredBindings) return;
            if (root.Find(ContentName) != null)
            {
                Debug.LogError("Loading alignment has incomplete serialized bindings; repair the Loading prefab outside Play Mode.");
                return;
            }
            if (loadSprite == null) loadSprite = Resources.Load<Sprite>;
            if (typography == null) typography = Resources.Load<SplashTitleTypography>(TypographyResource);
            Sprite environment = RequireSprite(loadSprite, ArtRoot + "environment_journey");
            Sprite logo = RequireSprite(loadSprite, LogoResource);
            Sprite ivan = RequireSprite(loadSprite, ArtRoot + "ivan_journey");
            Sprite firebird = RequireSprite(loadSprite, ArtRoot + "firebird_journey");
            Sprite panel = RequireSprite(loadSprite, ArtRoot + "tip_panel");
            if (typography == null) throw new InvalidOperationException("Missing shared SplashTitleTypography resource.");

            ProductionLoadingView view = existing != null ? existing : root.gameObject.AddComponent<ProductionLoadingView>();
            view.Revision = ProductionLoadingView.CurrentRevision;
            view.Typography = typography;
            RectTransform content = MakeRect(root, ContentName, 0, 0, 941, 1672);
            content.anchorMin = Vector2.zero; content.anchorMax = Vector2.one;
            content.offsetMin = content.offsetMax = Vector2.zero;
            view.Environment = MakeImage(content, "JourneyEnvironment", environment, 0, 0, 941, 1672);
            view.Logo = MakeImage(content, "ApprovedSplashLogo", logo, 195.5f, 35, 550, 440.16f);
            view.Ivan = MakeImage(content, "IvanJourney", ivan, 87, 623, 398, 597);
            view.Firebird = MakeImage(content, "FirebirdJourney", firebird, 425, 715, 315, 345);
            RectTransform shadeRect = MakeRect(content, "LowerContrast", 0, 1110, 941, 562);
            shadeRect.gameObject.AddComponent<CanvasRenderer>();
            ProductionLoadingShade shade = shadeRect.gameObject.AddComponent<ProductionLoadingShade>();
            shade.color = new Color32(3, 9, 17, 222); shade.raycastTarget = false;
            view.LowerShade = shade;
            Image backing = MakeImage(content, "OpaquePaperBacking", null, 156, 1260, 629, 166);
            backing.color = new Color32(229, 202, 148, 255);
            view.TipPanel = MakeImage(content, "TipPanel", panel, 125, 1210, 691, 260);
            view.TipPanel.preserveAspect = false;
            view.TipTitle = MakeText(content, "TipTitle", "СОВЕТ", 185, 1264, 571, 38, 30, new Color32(88, 64, 29, 255), typography);
            view.TipBody = MakeText(content, "TipBody", "Совет: объединяйте особые самоцветы,\nчтобы очищать тьму быстрее.", 187, 1315, 567, 82, 30, new Color32(52, 40, 23, 255), typography);
            view.StatusText = MakeText(content, "LoadingStatus", "Загрузка...", 170, 1490, 601, 46, 34, new Color32(227, 196, 132, 255), typography);
            Shadow shadow = view.StatusText.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color32(5, 10, 16, 255); shadow.effectDistance = new Vector2(0, -2);
            RectTransform barRect = MakeRect(content, "MeasuredProgressBar", 170, 1554, 601, 29);
            barRect.gameObject.AddComponent<CanvasRenderer>();
            view.ProgressBar = barRect.gameObject.AddComponent<ProductionLoadingProgressBar>();
            view.ProgressBar.raycastTarget = false;
            view.PercentText = MakeText(content, "MeasuredPercent", "0%", 320, 1590, 301, 34, 24, new Color32(227, 196, 132, 255), typography);
            Image retryImage = MakeImage(content, "RetryButton", null, 320.5f, 1595, 300, 62);
            retryImage.color = new Color32(19, 58, 53, 255); retryImage.raycastTarget = true;
            Outline retryBorder = retryImage.gameObject.AddComponent<Outline>();
            retryBorder.effectColor = new Color32(155, 115, 58, 255);
            retryBorder.effectDistance = new Vector2(2, -2);
            view.RetryButton = retryImage.gameObject.AddComponent<Button>();
            view.RetryButton.targetGraphic = retryImage;
            Navigation navigation = view.RetryButton.navigation; navigation.mode = Navigation.Mode.None;
            view.RetryButton.navigation = navigation;
            view.RetryText = MakeText(retryImage.rectTransform, "RetryLabel", "Повторить", 0, 0, 300, 62, 28, new Color32(227, 196, 132, 255), typography);
            view.RetryButton.gameObject.SetActive(false);
            view.PercentText.gameObject.SetActive(false);
            SplashTitleResponsiveLayout responsive = root.GetComponent<SplashTitleResponsiveLayout>();
            if (responsive == null) responsive = root.gameObject.AddComponent<SplashTitleResponsiveLayout>();
            responsive.Background = view.Environment;
            ProductionLoadingShadeLayout shadeLayout = root.GetComponent<ProductionLoadingShadeLayout>();
            if (shadeLayout == null) shadeLayout = root.gameObject.AddComponent<ProductionLoadingShadeLayout>();
            shadeLayout.Shade = shade;
            if (Application.isPlaying) { view.RefreshContent(); responsive.ApplyNow(); shadeLayout.ApplyNow(); }
        }

        static Sprite RequireSprite(Func<string, Sprite> loadSprite, string key)
        {
            Sprite sprite = loadSprite(key);
            if (sprite == null) throw new InvalidOperationException("Loading alignment requires an imported standalone Sprite: " + key);
            return sprite;
        }

        static RectTransform MakeRect(RectTransform parent, string name, float x, float y, float width, float height)
        {
            GameObject go = new GameObject(name, typeof(RectTransform)); go.layer = 5;
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        static Image MakeImage(RectTransform parent, string name, Sprite sprite, float x, float y, float width, float height)
        {
            Image image = MakeRect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
            image.sprite = sprite; image.color = Color.white; image.raycastTarget = false;
            image.preserveAspect = sprite != null;
            return image;
        }

        static Text MakeText(RectTransform parent, string name, string value, float x, float y,
            float width, float height, int size, Color color, SplashTitleTypography typography)
        {
            RectTransform rect = MakeRect(parent, name, x, y, width, height);
            // Share Splash's 2x glyph raster density while preserving authored layout dimensions.
            rect.sizeDelta *= 2f; rect.localScale = new Vector3(.5f, .5f, 1f);
            Text text = rect.gameObject.AddComponent<Text>();
            text.text = value; text.font = typography.RegularFont; text.fontSize = size * 2;
            text.fontStyle = FontStyle.Normal; text.alignment = TextAnchor.MiddleCenter;
            text.alignByGeometry = true; text.color = color; text.raycastTarget = false;
            text.supportRichText = false; text.resizeTextForBestFit = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }
    }
}
