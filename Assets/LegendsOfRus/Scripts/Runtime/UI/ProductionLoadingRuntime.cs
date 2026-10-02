using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Shared owner of the serialized Loading composition and its asset-only runtime repair.</summary>
    public static class ProductionLoadingRuntime
    {
        public const string ArtRoot = "ProductionUI/LoadingReferenceAligned/";
        public const string LogoResource = ArtRoot + "source_logo";
        public const string EnvironmentResource = ArtRoot + "clean_environment";
        public const string IvanResource = ArtRoot + "source_ivan";
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
            Sprite environment = RequireSprite(loadSprite, EnvironmentResource);
            Sprite logo = RequireSprite(loadSprite, LogoResource);
            Sprite ivan = RequireSprite(loadSprite, IvanResource);
            Sprite firebird = RequireSprite(loadSprite, ArtRoot + "source_firebird");
            Sprite panel = RequireSprite(loadSprite, ArtRoot + "source_tip_ru");
            if (typography == null) throw new InvalidOperationException("Missing shared SplashTitleTypography resource.");

            ProductionLoadingView view = existing != null ? existing : root.gameObject.AddComponent<ProductionLoadingView>();
            view.Revision = ProductionLoadingView.CurrentRevision;
            view.Typography = typography;
            RectTransform content = MakeRect(root, ContentName, 0, 0, 941, 1672);
            content.anchorMin = Vector2.zero; content.anchorMax = Vector2.one;
            content.offsetMin = content.offsetMax = Vector2.zero;
            view.Environment = MakeImage(content, "JourneyEnvironment", environment, 0, 0, 941, 1672);
            view.Logo = MakeImage(content, "ApprovedSplashLogo", logo, 195.5f, 35, 550, 550f * 1041 / 1511);
            view.Ivan = MakeImage(content, "IvanJourney", ivan, 87, 623, 398, 597);
            view.Firebird = MakeImage(content, "FirebirdJourney", firebird, 425, 715, 315, 345);
            view.Environment.color = ProductionHomeRuntime.EnvironmentTone;
            view.Ivan.color = ProductionHomeRuntime.HeroTone;
            view.Firebird.color = ProductionHomeRuntime.HeroTone;
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
            ApplyReferenceComposition(view, loadSprite);
            if (Application.isPlaying) { view.RefreshContent(); responsive.ApplyNow(); shadeLayout.ApplyNow(); }
        }

        /// <summary>Change only authorized artwork/tone on the existing serialized page.</summary>
        public static void UpgradeHomeStyle(ProductionLoadingView view, Func<string, Sprite> loadSprite)
        {
            if (Application.isPlaying) throw new InvalidOperationException("Loading art migration must run outside Play Mode.");
            if (view == null || view.Revision < 3 || view.Revision > ProductionLoadingView.CurrentRevision)
                throw new InvalidOperationException("Loading Home-style migration requires the installed aligned page.");
            ApplyReferenceComposition(view, loadSprite);
            if (!view.HasRequiredBindings) throw new InvalidOperationException("Loading reference migration produced incomplete bindings.");
        }

        static void ApplyReferenceComposition(ProductionLoadingView view, Func<string, Sprite> loadSprite)
        {
            RectTransform content=(RectTransform)view.transform.Find(ContentName);
            view.Environment.sprite=RequireSprite(loadSprite,EnvironmentResource);
            view.Logo.sprite=RequireSprite(loadSprite,LogoResource);
            view.Ivan.sprite=RequireSprite(loadSprite,IvanResource);
            view.Firebird.sprite=RequireSprite(loadSprite,ArtRoot+"source_firebird");
            view.Environment.color=view.Logo.color=view.Ivan.color=view.Firebird.color=Color.white;
            Place(view.Logo.rectTransform,185,15,565,398);
            Place(view.Ivan.rectTransform,23,383,409,831);
            Place(view.Firebird.rectTransform,481,619,437,832);
            view.SourceTipRussian=RequireSprite(loadSprite,ArtRoot+"source_tip_ru");
            view.SourceTipBlank=RequireSprite(loadSprite,ArtRoot+"source_tip_blank");
            view.TipPanel.sprite=view.SourceTipRussian;view.TipPanel.preserveAspect=true;
            Place(view.TipPanel.rectTransform,276,1068,394,361);
            Transform backing=content.Find("OpaquePaperBacking");if(backing)backing.gameObject.SetActive(false);
            view.LowerShade.color=Color.clear;
            view.ReferenceBodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            view.ReferenceTitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            PlaceText(view.TipTitle,326,1152,294,58,42,view.ReferenceTitleFont);
            PlaceText(view.TipBody,312,1238,326,113,22,view.ReferenceBodyFont);
            view.TipTitle.enabled=view.TipBody.enabled=false;
            PlaceText(view.StatusText,290,1432,360,51,34,view.ReferenceTitleFont);
            view.StatusText.enabled=false;
            view.SourceStatus=ReferenceImage(content,"OriginalLoadingLettering",RequireSprite(loadSprite,ArtRoot+"source_loading_ru"),229,1433,488,51);
            view.SourceProgressTrack=ReferenceImage(content,"OriginalProgressTrack",RequireSprite(loadSprite,ArtRoot+"source_progress_track"),105,1480,730,92);
            Place(view.ProgressBar.rectTransform,179,1507,585,41);
            view.ProgressBar.SourceFill=RequireSprite(loadSprite,ArtRoot+"source_progress_fill");
            view.ProgressBar.SetMaterialDirty();view.ProgressBar.SetVerticesDirty();
            view.SourceOuterFrame=ReferenceImage(content,"OriginalOuterFrame",RequireSprite(loadSprite,ArtRoot+"source_outer_frame"),0,0,941,1672);
            PlaceText(view.PercentText,406,1511,130,36,30,view.ReferenceBodyFont);
            view.PercentText.color=new Color32(255,234,171,255);
            var percentShadow=view.PercentText.GetComponent<Outline>()??view.PercentText.gameObject.AddComponent<Outline>();
            percentShadow.effectColor=new Color32(3,8,12,255);percentShadow.effectDistance=new Vector2(2,-2);
            Place(view.RetryButton.GetComponent<RectTransform>(),320.5f,1480,300,94);
            var retryImage=(Image)view.RetryButton.targetGraphic;
            retryImage.sprite=RequireSprite(loadSprite,"ProductionUI/SplashReferenceAligned/source_start_blank");
            retryImage.color=Color.white;retryImage.preserveAspect=true;
            var retryOutline=retryImage.GetComponent<Outline>();if(retryOutline)retryOutline.enabled=false;
            PlaceText(view.RetryText,25,25,250,44,24,view.ReferenceBodyFont);
            // Preserve source ornament depth; the measured fill/number are separate native layers.
            view.SourceProgressTrack.transform.SetAsLastSibling();view.ProgressBar.transform.SetAsLastSibling();
            view.SourceOuterFrame.transform.SetAsLastSibling();view.SourceStatus.transform.SetAsLastSibling();
            view.StatusText.transform.SetAsLastSibling();view.PercentText.transform.SetAsLastSibling();view.RetryButton.transform.SetAsLastSibling();
            view.Revision=ProductionLoadingView.CurrentRevision;
        }

        static Image ReferenceImage(RectTransform content,string name,Sprite sprite,float x,float y,float w,float h)
        {
            Transform old=content.Find(name);Image image=old?old.GetComponent<Image>():MakeImage(content,name,sprite,x,y,w,h);
            image.sprite=sprite;image.color=Color.white;image.raycastTarget=false;image.preserveAspect=true;
            Place(image.rectTransform,x,y,w,h);return image;
        }
        static void Place(RectTransform rect,float x,float y,float w,float h,int density=1)
        {
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);
            rect.sizeDelta=new Vector2(w,h)*density;rect.localScale=new Vector3(1f/density,1f/density,1);
        }
        static void PlaceText(Text text,float x,float y,float w,float h,int size,Font font)
        {
            Place(text.rectTransform,x,y,w,h,2);text.font=font;text.fontSize=size*2;text.alignment=TextAnchor.MiddleCenter;
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
