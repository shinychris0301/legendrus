using System;
using System.Linq;
using LegendsOfRus.StoreRelease;
using NUnit.Framework;

namespace LegendsOfRus.Tests.StoreRelease
{
    public sealed class StoreListingReleaseChecklistEditModeTests
    {
        [Test]
        public void Listing_DefaultEvidence_FailsClosed()
        {
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                default, null, null, Array.Empty<StoreAssetEvidence>());
            Assert.That(result.PublicationMinimumMet, Is.False);
            Assert.That(HasIssue(result, StoreListingIssueCode.AppNameMissing), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.IconMissing), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.FeatureGraphicMissing), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.ScreenshotCountTooLow), Is.True);
        }

        [Test]
        public void Listing_ValidMinimumPassesPublicationMinimum()
        {
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), ValidFeature(), new[] {ValidScreenshot(), ValidScreenshot()});
            Assert.That(result.PublicationMinimumMet, Is.True);
            Assert.That(result.BlockingIssues, Is.Empty);
            Assert.That(result.GameRecommendationTargetMet, Is.False);
        }

        [Test]
        public void Listing_ThreeRecommendedPortraitScreensPassRecommendationTarget()
        {
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), ValidFeature(),
                new[] {ValidScreenshot(), ValidScreenshot(), ValidScreenshot()});
            Assert.That(result.PublicationMinimumMet, Is.True);
            Assert.That(result.GameRecommendationTargetMet, Is.True);
            Assert.That(result.RecommendationIssues, Is.Empty);
        }

        [TestCase(31, StoreListingIssueCode.AppNameTooLong)]
        [TestCase(81, StoreListingIssueCode.ShortDescriptionTooLong)]
        public void Listing_TextLimitsAreEnforced(int length, StoreListingIssueCode expected)
        {
            StoreListingTextEvidence text = ValidText();
            if (expected == StoreListingIssueCode.AppNameTooLong) text.AppName = new string('a', length);
            else text.ShortDescription = new string('b', length);
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                text, ValidIcon(), ValidFeature(), new[] {ValidScreenshot(), ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, expected), Is.True);
        }

        [Test]
        public void Listing_FullDescriptionLimitIsEnforced()
        {
            StoreListingTextEvidence text = ValidText();
            text.FullDescription = new string('c', StoreListingRequirements.FullDescriptionMaxCharacters + 1);
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                text, ValidIcon(), ValidFeature(), new[] {ValidScreenshot(), ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.FullDescriptionTooLong), Is.True);
        }

        [Test]
        public void Listing_IconRequirementsAreEnforced()
        {
            StoreAssetEvidence icon = ValidIcon();
            icon.Width = 511;
            icon.HasAlphaChannel = false;
            icon.FileSizeBytes = StoreListingRequirements.IconMaxFileSizeBytes + 1;
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), icon, ValidFeature(), new[] {ValidScreenshot(), ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.IconWrongDimensions), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.IconTooLarge), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.IconAlphaChannelMissing), Is.True);
        }

        [Test]
        public void Listing_FeatureGraphicRejectsAlphaAndWrongDimensions()
        {
            StoreAssetEvidence feature = ValidFeature();
            feature.Width = 1000;
            feature.HasAlphaChannel = true;
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), feature, new[] {ValidScreenshot(), ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.FeatureGraphicWrongDimensions), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.FeatureGraphicHasAlpha), Is.True);
        }

        [Test]
        public void Listing_ScreenshotRejectsAlphaAndNonRuntimeReferenceArt()
        {
            StoreAssetEvidence shot = ValidScreenshot();
            shot.HasAlphaChannel = true;
            shot.RepresentsActualRuntime = false;
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), ValidFeature(), new[] {shot, ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.ScreenshotHasAlpha), Is.True);
            Assert.That(HasIssue(result, StoreListingIssueCode.ScreenshotNotRuntimeCapture), Is.True);
        }

        [Test]
        public void Listing_ScreenshotDimensionRatioIsBounded()
        {
            StoreAssetEvidence shot = ValidScreenshot();
            shot.Width = 320;
            shot.Height = 1000;
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), ValidFeature(), new[] {shot, ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.ScreenshotAspectOutOfRange), Is.True);
        }

        [Test]
        public void Listing_AltTextLimitIsEnforced()
        {
            StoreAssetEvidence shot = ValidScreenshot();
            shot.AltText = new string('x', StoreListingRequirements.ScreenshotAltTextMaxCharacters + 1);
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                ValidText(), ValidIcon(), ValidFeature(), new[] {shot, ValidScreenshot(), ValidScreenshot()});
            Assert.That(HasIssue(result, StoreListingIssueCode.ScreenshotAltTextTooLong), Is.True);
        }

        [Test]
        public void ReleaseChecklist_DefaultEvidence_FailsClosed()
        {
            StoreReleaseChecklistResult result = StoreReleaseChecklistEvaluator.Evaluate(default);
            Assert.That(result.MayCreateControlledTestArtifact, Is.False);
            Assert.That(result.MayUploadClosedTest, Is.False);
            Assert.That(result.ArtifactBlockers, Does.Contain(StoreReleaseBlockerCodes.UnityCompileMissing));
            Assert.That(result.UploadBlockers, Does.Contain(StoreReleaseBlockerCodes.StoreListingAssetsMissing));
            Assert.That(result.UploadBlockers, Does.Contain(StoreReleaseBlockerCodes.PublishingNotAuthorized));
        }

        [Test]
        public void ReleaseChecklist_CompleteArtifactEvidence_AllowsArtifactButNotUpload()
        {
            StoreReleaseEvidence evidence = CompleteArtifactEvidence();
            StoreReleaseChecklistResult result = StoreReleaseChecklistEvaluator.Evaluate(evidence);
            Assert.That(result.MayCreateControlledTestArtifact, Is.True);
            Assert.That(result.MayUploadClosedTest, Is.False);
            Assert.That(result.ArtifactBlockers, Is.Empty);
            Assert.That(result.UploadBlockers, Does.Contain(StoreReleaseBlockerCodes.StoreListingTextMissing));
        }

        [Test]
        public void ReleaseChecklist_FullyApprovedEvidence_AllowsUpload()
        {
            StoreReleaseEvidence evidence = CompleteArtifactEvidence();
            evidence.StoreListingTextApproved = true;
            evidence.StoreListingAssetsApproved = true;
            evidence.PrivacyDataSafetyApproved = true;
            evidence.ContentRatingCompleted = true;
            evidence.TargetAudienceCompleted = true;
            evidence.AdsDeclarationCompleted = true;
            evidence.AppAccessDeclarationCompleted = true;
            evidence.AppBundleEvidenceRetained = true;
            evidence.ReleaseNotesApproved = true;
            evidence.PublishingAuthorized = true;
            StoreReleaseChecklistResult result = StoreReleaseChecklistEvaluator.Evaluate(evidence);
            Assert.That(result.MayUploadClosedTest, Is.True);
            Assert.That(result.UploadBlockers, Is.Empty);
            Assert.That(result.Warnings, Does.Contain(StoreReleaseBlockerCodes.ProductionCommercialDisabled));
        }

        static StoreListingTextEvidence ValidText()
        {
            return new StoreListingTextEvidence
            {
                AppName = "Legends of Rus",
                ShortDescription = "A fairy-tale match-3 adventure.",
                FullDescription = "Journey through a Russian fairy-tale world in a story-driven match-3 adventure."
            };
        }

        static StoreAssetEvidence ValidIcon()
        {
            return new StoreAssetEvidence
            {
                Kind = StoreAssetKind.AppIcon,
                Format = StoreAssetFormat.Png,
                Width = 512,
                Height = 512,
                FileSizeBytes = 512000,
                HasAlphaChannel = true
            };
        }

        static StoreAssetEvidence ValidFeature()
        {
            return new StoreAssetEvidence
            {
                Kind = StoreAssetKind.FeatureGraphic,
                Format = StoreAssetFormat.Png,
                Width = 1024,
                Height = 500,
                FileSizeBytes = 900000,
                HasAlphaChannel = false
            };
        }

        static StoreAssetEvidence ValidScreenshot()
        {
            return new StoreAssetEvidence
            {
                Kind = StoreAssetKind.PhoneScreenshot,
                Format = StoreAssetFormat.Png,
                Width = 1080,
                Height = 1920,
                FileSizeBytes = 1500000,
                HasAlphaChannel = false,
                RepresentsActualRuntime = true,
                AltText = "Match-3 gameplay with story characters and objectives."
            };
        }

        static bool HasIssue(StoreListingValidationResult result, StoreListingIssueCode code)
        {
            return Array.IndexOf(result.BlockingIssues, code) >= 0;
        }

        static StoreReleaseEvidence CompleteArtifactEvidence()
        {
            return new StoreReleaseEvidence
            {
                SourceStaticVerificationPassed = true,
                UnityCompilePassed = true,
                EditorSelfTestPassed = true,
                EditModePassed = true,
                PlayModePassed = true,
                RequiredDeviceProfiles = 5,
                PassedDeviceProfiles = 5,
                AndroidDevelopmentBuildEvidenceRetained = true,
                ProductionSigningApproved = true,
                ControlledTestBuildAuthorized = true,
                ProductionCommercialActivationEnabled = false,
                RemoteQaUploadEnabled = false
            };
        }
    }
}
