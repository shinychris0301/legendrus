using System;

namespace LegendsOfRus.StoreRelease
{
    public enum StoreAssetKind
    {
        None = 0,
        AppIcon = 1,
        FeatureGraphic = 2,
        PhoneScreenshot = 3
    }

    public enum StoreAssetFormat
    {
        Unknown = 0,
        Png = 1,
        Jpeg = 2
    }

    public enum StoreListingIssueCode
    {
        None = 0,
        AppNameMissing = 1,
        AppNameTooLong = 2,
        ShortDescriptionMissing = 3,
        ShortDescriptionTooLong = 4,
        FullDescriptionMissing = 5,
        FullDescriptionTooLong = 6,
        IconMissing = 7,
        IconWrongFormat = 8,
        IconWrongDimensions = 9,
        IconTooLarge = 10,
        IconAlphaChannelMissing = 11,
        FeatureGraphicMissing = 12,
        FeatureGraphicWrongFormat = 13,
        FeatureGraphicWrongDimensions = 14,
        FeatureGraphicHasAlpha = 15,
        ScreenshotCountTooLow = 16,
        ScreenshotCountTooHigh = 17,
        ScreenshotWrongFormat = 18,
        ScreenshotDimensionOutOfRange = 19,
        ScreenshotAspectOutOfRange = 20,
        ScreenshotHasAlpha = 21,
        ScreenshotNotRuntimeCapture = 22,
        ScreenshotAltTextMissing = 23,
        ScreenshotAltTextTooLong = 24,
        GameRecommendationScreenshotCountLow = 25,
        GameRecommendationResolutionLow = 26
    }

    [Serializable]
    public struct StoreListingTextEvidence
    {
        public string AppName;
        public string ShortDescription;
        public string FullDescription;
    }

    [Serializable]
    public struct StoreAssetEvidence
    {
        public StoreAssetKind Kind;
        public StoreAssetFormat Format;
        public int Width;
        public int Height;
        public long FileSizeBytes;
        public bool HasAlphaChannel;
        public bool RepresentsActualRuntime;
        public string AltText;
    }

    [Serializable]
    public sealed class StoreListingValidationResult
    {
        public bool PublicationMinimumMet;
        public bool GameRecommendationTargetMet;
        public StoreListingIssueCode[] BlockingIssues = Array.Empty<StoreListingIssueCode>();
        public StoreListingIssueCode[] RecommendationIssues = Array.Empty<StoreListingIssueCode>();
    }
}
