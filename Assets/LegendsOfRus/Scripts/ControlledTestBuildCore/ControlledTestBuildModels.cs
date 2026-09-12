using System;

namespace LegendsOfRus.ControlledTestBuild
{
    public static class ControlledTestBuildBlockerCodes
    {
        public const string StaticVerificationMissing = "CTB_STATIC_VERIFICATION_MISSING";
        public const string UnityCompileMissing = "CTB_UNITY_COMPILE_EVIDENCE_MISSING";
        public const string SelfTestMissing = "CTB_EDITOR_SELF_TEST_EVIDENCE_MISSING";
        public const string EditModeMissing = "CTB_EDITMODE_EVIDENCE_MISSING";
        public const string PlayModeMissing = "CTB_PLAYMODE_EVIDENCE_MISSING";
        public const string DeviceMatrixIncomplete = "CTB_DEVICE_MATRIX_INCOMPLETE";
        public const string PackageLockUnreviewed = "CTB_PACKAGE_LOCK_UNREVIEWED";
        public const string ProjectSettingsUnreviewed = "CTB_PROJECT_SETTINGS_UNREVIEWED";
        public const string TargetApiTooLow = "CTB_TARGET_API_BELOW_36";
        public const string AppBundleNotSelected = "CTB_APP_BUNDLE_NOT_SELECTED";
        public const string Il2CppMissing = "CTB_IL2CPP_NOT_ENABLED";
        public const string Arm64Missing = "CTB_ARM64_NOT_ENABLED";
        public const string SixteenKbEvidenceMissing = "CTB_16KB_PAGE_COMPATIBILITY_NOT_VERIFIED";
        public const string ProductionIdentityMissing = "CTB_PRODUCTION_APPLICATION_IDENTIFIER_NOT_APPROVED";
        public const string VersioningMissing = "CTB_VERSIONING_NOT_APPROVED";
        public const string PlayAppSigningMissing = "CTB_PLAY_APP_SIGNING_NOT_APPROVED";
        public const string UploadKeyWorkflowMissing = "CTB_UPLOAD_KEY_WORKFLOW_NOT_APPROVED";
        public const string ProductionSigningMissing = "CTB_PRODUCTION_SIGNING_NOT_APPROVED";
        public const string ControlledBuildNotAuthorized = "CTB_CONTROLLED_TEST_BUILD_NOT_AUTHORIZED";
        public const string ArtifactEvidenceMissing = "CTB_SIGNED_AAB_EVIDENCE_MISSING";
        public const string StoreChecklistIncomplete = "CTB_STORE_RELEASE_CHECKLIST_INCOMPLETE";
        public const string PrivacyDataSafetyMissing = "CTB_PRIVACY_DATA_SAFETY_NOT_APPROVED";
        public const string ClosedTestUploadNotAuthorized = "CTB_CLOSED_TEST_UPLOAD_NOT_AUTHORIZED";
        public const string PublishingNotAuthorized = "CTB_STORE_PUBLISHING_NOT_AUTHORIZED";
        public const string PackageRegistrationReviewPending = "CTB_PACKAGE_REGISTRATION_REVIEW_DUE_2026_09_30";
        public const string RemoteBuildDisabled = "CTB_REMOTE_BUILD_SERVICE_DISABLED";
    }

    [Serializable]
    public struct ControlledTestBuildEvidence
    {
        public bool SourceStaticVerificationPassed;
        public bool UnityCompilePassed;
        public bool EditorSelfTestPassed;
        public bool EditModePassed;
        public bool PlayModePassed;
        public int RequiredDeviceProfiles;
        public int PassedDeviceProfiles;
        public bool PackageLockReviewed;
        public bool ProjectSettingsReviewed;
        public int TargetApiLevel;
        public bool AppBundleSelected;
        public bool Il2CppEnabled;
        public bool Arm64Enabled;
        public bool SixteenKbPageCompatibilityVerified;
        public bool ProductionApplicationIdentifierApproved;
        public bool VersioningApproved;
        public bool PlayAppSigningApproved;
        public bool UploadKeyWorkflowApproved;
        public bool ProductionSigningApproved;
        public bool ControlledTestBuildAuthorized;
        public bool SignedAppBundleEvidenceRetained;
        public bool StoreReleaseChecklistApproved;
        public bool PrivacyDataSafetyApproved;
        public bool ClosedTestUploadAuthorized;
        public bool StorePublishingAuthorized;
        public bool PackageNameRegistrationReviewed;
        public bool RemoteBuildServiceApproved;
    }

    [Serializable]
    public sealed class ControlledTestBuildResult
    {
        public bool MayPrepareReleaseCandidatePlan;
        public bool MayCreateSignedAppBundle;
        public bool MayUploadClosedTest;
        public string[] PlanBlockers = Array.Empty<string>();
        public string[] BuildBlockers = Array.Empty<string>();
        public string[] UploadBlockers = Array.Empty<string>();
        public string[] Warnings = Array.Empty<string>();
    }

    [Serializable]
    public struct ControlledTestArtifactEvidence
    {
        public string FileName;
        public string Sha256;
        public long SizeBytes;
        public string VersionName;
        public int VersionCode;
        public int TargetApiLevel;
        public bool Signed;
        public bool AppBundle;
        public bool Arm64Included;
        public bool SixteenKbPageCompatibilityVerified;
    }

    [Serializable]
    public sealed class ControlledTestArtifactValidationResult
    {
        public bool Valid;
        public string[] Issues = Array.Empty<string>();
    }
}
