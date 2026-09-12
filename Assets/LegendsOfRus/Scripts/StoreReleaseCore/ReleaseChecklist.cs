using System;
using System.Collections.Generic;

namespace LegendsOfRus.StoreRelease
{
    public static class StoreReleaseBlockerCodes
    {
        public const string StaticVerificationMissing = "RELEASE_STATIC_VERIFICATION_MISSING";
        public const string UnityCompileMissing = "RELEASE_UNITY_COMPILE_EVIDENCE_MISSING";
        public const string SelfTestMissing = "RELEASE_EDITOR_SELF_TEST_EVIDENCE_MISSING";
        public const string EditModeMissing = "RELEASE_EDITMODE_EVIDENCE_MISSING";
        public const string PlayModeMissing = "RELEASE_PLAYMODE_EVIDENCE_MISSING";
        public const string DeviceMatrixIncomplete = "RELEASE_DEVICE_MATRIX_INCOMPLETE";
        public const string AndroidBuildEvidenceMissing = "RELEASE_ANDROID_BUILD_EVIDENCE_MISSING";
        public const string StoreListingTextMissing = "RELEASE_STORE_LISTING_TEXT_NOT_APPROVED";
        public const string StoreListingAssetsMissing = "RELEASE_STORE_LISTING_ASSETS_NOT_APPROVED";
        public const string PrivacyDataSafetyMissing = "RELEASE_PRIVACY_DATA_SAFETY_NOT_APPROVED";
        public const string ContentRatingMissing = "RELEASE_CONTENT_RATING_INCOMPLETE";
        public const string TargetAudienceMissing = "RELEASE_TARGET_AUDIENCE_INCOMPLETE";
        public const string AdsDeclarationMissing = "RELEASE_ADS_DECLARATION_INCOMPLETE";
        public const string AppAccessMissing = "RELEASE_APP_ACCESS_DECLARATION_INCOMPLETE";
        public const string ProductionSigningMissing = "RELEASE_PRODUCTION_SIGNING_NOT_APPROVED";
        public const string AppBundleEvidenceMissing = "RELEASE_AAB_EVIDENCE_MISSING";
        public const string ReleaseNotesMissing = "RELEASE_NOTES_NOT_APPROVED";
        public const string ControlledBuildNotAuthorized = "RELEASE_CONTROLLED_TEST_BUILD_NOT_AUTHORIZED";
        public const string PublishingNotAuthorized = "RELEASE_STORE_PUBLISHING_NOT_AUTHORIZED";
        public const string ProductionCommercialDisabled = "RELEASE_PRODUCTION_COMMERCIAL_DISABLED";
        public const string RemoteQaUploadDisabled = "RELEASE_REMOTE_QA_UPLOAD_DISABLED";
    }

    [Serializable]
    public struct StoreReleaseEvidence
    {
        public bool SourceStaticVerificationPassed;
        public bool UnityCompilePassed;
        public bool EditorSelfTestPassed;
        public bool EditModePassed;
        public bool PlayModePassed;
        public int RequiredDeviceProfiles;
        public int PassedDeviceProfiles;
        public bool AndroidDevelopmentBuildEvidenceRetained;
        public bool StoreListingTextApproved;
        public bool StoreListingAssetsApproved;
        public bool PrivacyDataSafetyApproved;
        public bool ContentRatingCompleted;
        public bool TargetAudienceCompleted;
        public bool AdsDeclarationCompleted;
        public bool AppAccessDeclarationCompleted;
        public bool ProductionSigningApproved;
        public bool AppBundleEvidenceRetained;
        public bool ReleaseNotesApproved;
        public bool ControlledTestBuildAuthorized;
        public bool PublishingAuthorized;
        public bool ProductionCommercialActivationEnabled;
        public bool RemoteQaUploadEnabled;
    }

    [Serializable]
    public sealed class StoreReleaseChecklistResult
    {
        public bool MayCreateControlledTestArtifact;
        public bool MayUploadClosedTest;
        public string[] ArtifactBlockers = Array.Empty<string>();
        public string[] UploadBlockers = Array.Empty<string>();
        public string[] Warnings = Array.Empty<string>();
    }

    public static class StoreReleaseChecklistEvaluator
    {
        public static StoreReleaseChecklistResult Evaluate(StoreReleaseEvidence evidence)
        {
            var artifact = new List<string>();
            var upload = new List<string>();
            var warnings = new List<string>();

            Require(evidence.SourceStaticVerificationPassed, StoreReleaseBlockerCodes.StaticVerificationMissing, artifact);
            Require(evidence.UnityCompilePassed, StoreReleaseBlockerCodes.UnityCompileMissing, artifact);
            Require(evidence.EditorSelfTestPassed, StoreReleaseBlockerCodes.SelfTestMissing, artifact);
            Require(evidence.EditModePassed, StoreReleaseBlockerCodes.EditModeMissing, artifact);
            Require(evidence.PlayModePassed, StoreReleaseBlockerCodes.PlayModeMissing, artifact);
            Require(evidence.RequiredDeviceProfiles > 0 && evidence.PassedDeviceProfiles >= evidence.RequiredDeviceProfiles,
                StoreReleaseBlockerCodes.DeviceMatrixIncomplete, artifact);
            Require(evidence.AndroidDevelopmentBuildEvidenceRetained,
                StoreReleaseBlockerCodes.AndroidBuildEvidenceMissing, artifact);
            Require(evidence.ProductionSigningApproved,
                StoreReleaseBlockerCodes.ProductionSigningMissing, artifact);
            Require(evidence.ControlledTestBuildAuthorized,
                StoreReleaseBlockerCodes.ControlledBuildNotAuthorized, artifact);

            upload.AddRange(artifact);
            Require(evidence.StoreListingTextApproved, StoreReleaseBlockerCodes.StoreListingTextMissing, upload);
            Require(evidence.StoreListingAssetsApproved, StoreReleaseBlockerCodes.StoreListingAssetsMissing, upload);
            Require(evidence.PrivacyDataSafetyApproved, StoreReleaseBlockerCodes.PrivacyDataSafetyMissing, upload);
            Require(evidence.ContentRatingCompleted, StoreReleaseBlockerCodes.ContentRatingMissing, upload);
            Require(evidence.TargetAudienceCompleted, StoreReleaseBlockerCodes.TargetAudienceMissing, upload);
            Require(evidence.AdsDeclarationCompleted, StoreReleaseBlockerCodes.AdsDeclarationMissing, upload);
            Require(evidence.AppAccessDeclarationCompleted, StoreReleaseBlockerCodes.AppAccessMissing, upload);
            Require(evidence.AppBundleEvidenceRetained, StoreReleaseBlockerCodes.AppBundleEvidenceMissing, upload);
            Require(evidence.ReleaseNotesApproved, StoreReleaseBlockerCodes.ReleaseNotesMissing, upload);
            Require(evidence.PublishingAuthorized, StoreReleaseBlockerCodes.PublishingNotAuthorized, upload);

            if (!evidence.ProductionCommercialActivationEnabled)
                warnings.Add(StoreReleaseBlockerCodes.ProductionCommercialDisabled);
            if (!evidence.RemoteQaUploadEnabled)
                warnings.Add(StoreReleaseBlockerCodes.RemoteQaUploadDisabled);

            return new StoreReleaseChecklistResult
            {
                MayCreateControlledTestArtifact = artifact.Count == 0,
                MayUploadClosedTest = upload.Count == 0,
                ArtifactBlockers = artifact.ToArray(),
                UploadBlockers = upload.ToArray(),
                Warnings = warnings.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition)
                blockers.Add(code);
        }
    }
}
