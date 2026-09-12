using System;
using System.Collections.Generic;

namespace LegendsOfRus.StoreRelease
{
    public static class StoreListingRequirements
    {
        // Google Play requirements snapshot reviewed 2026-09-09.
        public const int AppNameMaxCharacters = 30;
        public const int ShortDescriptionMaxCharacters = 80;
        public const int FullDescriptionMaxCharacters = 4000;

        public const int IconWidth = 512;
        public const int IconHeight = 512;
        public const long IconMaxFileSizeBytes = 1024L * 1024L;

        public const int FeatureGraphicWidth = 1024;
        public const int FeatureGraphicHeight = 500;

        public const int MinimumScreenshotCount = 2;
        public const int MaximumScreenshotsPerDeviceType = 8;
        public const int ScreenshotMinimumDimension = 320;
        public const int ScreenshotMaximumDimension = 3840;
        public const int ScreenshotMaximumLongToShortRatio = 2;
        public const int ScreenshotAltTextMaxCharacters = 140;

        // Project release policy adopts Google Play's game recommendation target.
        public const int RecommendedGameScreenshotCount = 3;
        public const int RecommendedPortraitScreenshotWidth = 1080;
        public const int RecommendedPortraitScreenshotHeight = 1920;

        public static StoreListingValidationResult Validate(
            StoreListingTextEvidence text,
            StoreAssetEvidence? icon,
            StoreAssetEvidence? featureGraphic,
            StoreAssetEvidence[] screenshots)
        {
            var blockers = new List<StoreListingIssueCode>();
            var recommendations = new List<StoreListingIssueCode>();

            ValidateText(text, blockers);
            ValidateIcon(icon, blockers);
            ValidateFeatureGraphic(featureGraphic, blockers);
            ValidateScreenshots(screenshots ?? Array.Empty<StoreAssetEvidence>(), blockers, recommendations);

            return new StoreListingValidationResult
            {
                PublicationMinimumMet = blockers.Count == 0,
                GameRecommendationTargetMet = blockers.Count == 0 && recommendations.Count == 0,
                BlockingIssues = blockers.ToArray(),
                RecommendationIssues = recommendations.ToArray()
            };
        }

        static void ValidateText(StoreListingTextEvidence text, List<StoreListingIssueCode> blockers)
        {
            ValidateTextField(text.AppName, AppNameMaxCharacters,
                StoreListingIssueCode.AppNameMissing, StoreListingIssueCode.AppNameTooLong, blockers);
            ValidateTextField(text.ShortDescription, ShortDescriptionMaxCharacters,
                StoreListingIssueCode.ShortDescriptionMissing, StoreListingIssueCode.ShortDescriptionTooLong, blockers);
            ValidateTextField(text.FullDescription, FullDescriptionMaxCharacters,
                StoreListingIssueCode.FullDescriptionMissing, StoreListingIssueCode.FullDescriptionTooLong, blockers);
        }

        static void ValidateTextField(
            string value,
            int maxCharacters,
            StoreListingIssueCode missing,
            StoreListingIssueCode tooLong,
            List<StoreListingIssueCode> blockers)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                blockers.Add(missing);
                return;
            }
            if (value.Length > maxCharacters)
                blockers.Add(tooLong);
        }

        static void ValidateIcon(StoreAssetEvidence? value, List<StoreListingIssueCode> blockers)
        {
            if (!value.HasValue)
            {
                blockers.Add(StoreListingIssueCode.IconMissing);
                return;
            }
            StoreAssetEvidence icon = value.Value;
            if (icon.Kind != StoreAssetKind.AppIcon || icon.Format != StoreAssetFormat.Png)
                blockers.Add(StoreListingIssueCode.IconWrongFormat);
            if (icon.Width != IconWidth || icon.Height != IconHeight)
                blockers.Add(StoreListingIssueCode.IconWrongDimensions);
            if (icon.FileSizeBytes <= 0 || icon.FileSizeBytes > IconMaxFileSizeBytes)
                blockers.Add(StoreListingIssueCode.IconTooLarge);
            if (!icon.HasAlphaChannel)
                blockers.Add(StoreListingIssueCode.IconAlphaChannelMissing);
        }

        static void ValidateFeatureGraphic(StoreAssetEvidence? value, List<StoreListingIssueCode> blockers)
        {
            if (!value.HasValue)
            {
                blockers.Add(StoreListingIssueCode.FeatureGraphicMissing);
                return;
            }
            StoreAssetEvidence feature = value.Value;
            if (feature.Kind != StoreAssetKind.FeatureGraphic ||
                (feature.Format != StoreAssetFormat.Png && feature.Format != StoreAssetFormat.Jpeg))
                blockers.Add(StoreListingIssueCode.FeatureGraphicWrongFormat);
            if (feature.Width != FeatureGraphicWidth || feature.Height != FeatureGraphicHeight)
                blockers.Add(StoreListingIssueCode.FeatureGraphicWrongDimensions);
            if (feature.HasAlphaChannel)
                blockers.Add(StoreListingIssueCode.FeatureGraphicHasAlpha);
        }

        static void ValidateScreenshots(
            StoreAssetEvidence[] screenshots,
            List<StoreListingIssueCode> blockers,
            List<StoreListingIssueCode> recommendations)
        {
            if (screenshots.Length < MinimumScreenshotCount)
                blockers.Add(StoreListingIssueCode.ScreenshotCountTooLow);
            if (screenshots.Length > MaximumScreenshotsPerDeviceType)
                blockers.Add(StoreListingIssueCode.ScreenshotCountTooHigh);

            bool recommendedResolutionSatisfied = screenshots.Length >= RecommendedGameScreenshotCount;
            for (int i = 0; i < screenshots.Length; i++)
            {
                StoreAssetEvidence shot = screenshots[i];
                if (shot.Kind != StoreAssetKind.PhoneScreenshot ||
                    (shot.Format != StoreAssetFormat.Png && shot.Format != StoreAssetFormat.Jpeg))
                    blockers.Add(StoreListingIssueCode.ScreenshotWrongFormat);

                int min = Math.Min(shot.Width, shot.Height);
                int max = Math.Max(shot.Width, shot.Height);
                if (min < ScreenshotMinimumDimension || max > ScreenshotMaximumDimension)
                    blockers.Add(StoreListingIssueCode.ScreenshotDimensionOutOfRange);
                if (min <= 0 || max > min * ScreenshotMaximumLongToShortRatio)
                    blockers.Add(StoreListingIssueCode.ScreenshotAspectOutOfRange);
                if (shot.HasAlphaChannel)
                    blockers.Add(StoreListingIssueCode.ScreenshotHasAlpha);
                if (!shot.RepresentsActualRuntime)
                    blockers.Add(StoreListingIssueCode.ScreenshotNotRuntimeCapture);
                if (string.IsNullOrWhiteSpace(shot.AltText))
                    blockers.Add(StoreListingIssueCode.ScreenshotAltTextMissing);
                else if (shot.AltText.Length > ScreenshotAltTextMaxCharacters)
                    blockers.Add(StoreListingIssueCode.ScreenshotAltTextTooLong);

                bool portrait1080 = shot.Width >= RecommendedPortraitScreenshotWidth &&
                    shot.Height >= RecommendedPortraitScreenshotHeight;
                if (!portrait1080)
                    recommendedResolutionSatisfied = false;
            }

            if (screenshots.Length < RecommendedGameScreenshotCount)
                recommendations.Add(StoreListingIssueCode.GameRecommendationScreenshotCountLow);
            if (!recommendedResolutionSatisfied)
                recommendations.Add(StoreListingIssueCode.GameRecommendationResolutionLow);
        }
    }
}
