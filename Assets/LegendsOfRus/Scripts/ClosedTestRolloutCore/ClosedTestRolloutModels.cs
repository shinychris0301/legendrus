using System;

namespace LegendsOfRus.ClosedTestRollout
{
    public static class ClosedTestRolloutBlockerCodes
    {
        public const string FreezeManifestMissing = "CTR_FREEZE_MANIFEST_NOT_VERIFIED";
        public const string ArtifactHashMismatch = "CTR_ARTIFACT_HASH_DOES_NOT_MATCH_FREEZE";
        public const string AppSetupIncomplete = "CTR_PLAY_APP_SETUP_INCOMPLETE";
        public const string TestingPermissionMissing = "CTR_TESTING_RELEASE_PERMISSION_MISSING";
        public const string ClosedTrackMissing = "CTR_CLOSED_TEST_TRACK_NOT_CONFIGURED";
        public const string TesterAccessMissing = "CTR_TESTER_ACCESS_NOT_CONFIGURED";
        public const string CountryReviewMissing = "CTR_COUNTRY_AVAILABILITY_NOT_REVIEWED";
        public const string ReleaseNotesMissing = "CTR_RELEASE_NOTES_NOT_APPROVED";
        public const string OutstandingReleasePresent = "CTR_OUTSTANDING_RELEASE_PRESENT";
        public const string UploadNotAuthorized = "CTR_CLOSED_TEST_UPLOAD_NOT_AUTHORIZED";
        public const string ClosedTestPublishNotAuthorized = "CTR_CLOSED_TEST_PUBLISH_NOT_AUTHORIZED";
        public const string UploadEvidenceInvalid = "CTR_CLOSED_TEST_UPLOAD_EVIDENCE_INVALID";
        public const string ReleaseNotAvailableToTesters = "CTR_RELEASE_NOT_AVAILABLE_TO_TESTERS";
        public const string HealthEvidenceInvalid = "CTR_CLOSED_TEST_HEALTH_EVIDENCE_INVALID";
        public const string BlockingCrash = "CTR_BLOCKING_CRASH_OBSERVED";
        public const string BlockingAnr = "CTR_BLOCKING_ANR_OBSERVED";
        public const string CriticalBug = "CTR_CRITICAL_BUG_OPEN";
        public const string BlockingFeedback = "CTR_BLOCKING_TESTER_FEEDBACK_OPEN";
        public const string FeedbackNotReviewed = "CTR_TESTER_FEEDBACK_NOT_REVIEWED";
        public const string GoNoGoNotAuthorized = "CTR_GO_NO_GO_NOT_AUTHORIZED";
        public const string AccountTypeNotReviewed = "CTR_DEVELOPER_ACCOUNT_TYPE_NOT_REVIEWED";
        public const string PersonalTesterCountInsufficient = "CTR_PERSONAL_ACCOUNT_TESTER_COUNT_BELOW_12";
        public const string PersonalTestDurationInsufficient = "CTR_PERSONAL_ACCOUNT_CONTINUOUS_TEST_BELOW_14_DAYS";
        public const string ProductionAccessMissing = "CTR_PRODUCTION_ACCESS_NOT_GRANTED";
        public const string StoreChecklistIncomplete = "CTR_STORE_RELEASE_CHECKLIST_NOT_APPROVED";
        public const string PrivacyDataSafetyIncomplete = "CTR_PRIVACY_DATA_SAFETY_NOT_APPROVED";
        public const string PlayDeclarationsIncomplete = "CTR_PLAY_DECLARATIONS_NOT_APPROVED";
        public const string RollbackPlanMissing = "CTR_ROLLBACK_PLAN_NOT_REVIEWED";
        public const string MonitoringPlanMissing = "CTR_MONITORING_PLAN_NOT_REVIEWED";
        public const string ManagedPublishingNotReviewed = "CTR_MANAGED_PUBLISHING_NOT_REVIEWED";
        public const string FirstReleaseStagedRolloutUnsupported = "CTR_FIRST_PRODUCTION_RELEASE_MUST_USE_100_PERCENT";
        public const string RolloutPercentInvalid = "CTR_ROLLOUT_PERCENT_INVALID";
        public const string ProductionRolloutNotAuthorized = "CTR_PRODUCTION_ROLLOUT_NOT_AUTHORIZED";
        public const string StorePublishingNotAuthorized = "CTR_STORE_PUBLISHING_NOT_AUTHORIZED";
    }

    public enum ClosedTestReleaseStatus
    {
        None = 0,
        Draft = 1,
        InReview = 2,
        AvailableToTesters = 3,
        Rejected = 4,
        Halted = 5
    }

    [Serializable]
    public struct ClosedTestUploadReadinessEvidence
    {
        public bool CandidateFreezeManifestVerified;
        public bool ArtifactHashMatchesFreeze;
        public bool PlayAppSetupComplete;
        public bool TestingTrackReleasePermissionApproved;
        public bool ClosedTestTrackConfigured;
        public bool TesterAccessConfigured;
        public bool CountryAvailabilityReviewed;
        public bool ReleaseNotesApproved;
        public bool NoOutstandingRelease;
        public bool ClosedTestUploadAuthorized;
        public bool ClosedTestPublishAuthorized;
    }

    [Serializable]
    public sealed class ClosedTestUploadReadinessResult
    {
        public bool MayPrepareClosedTestRelease;
        public bool MayRolloutClosedTestRelease;
        public string[] PrepareBlockers = Array.Empty<string>();
        public string[] RolloutBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ClosedTestUploadEvidence
    {
        public int SchemaVersion = ClosedTestRolloutRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string FreezeManifestSha256 = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public string VersionName = string.Empty;
        public int VersionCode;
        public ClosedTestReleaseStatus ReleaseStatus;
        public bool ArtifactAcceptedByPlay;
        public bool ClosedTestTrackConfigured;
        public bool TesterAccessConfigured;
        public bool CountryAvailabilityReviewed;
        public bool ReleaseNotesRetained;
    }

    [Serializable]
    public sealed class ClosedTestHealthEvidence
    {
        public int SchemaVersion = ClosedTestRolloutRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public int ObservationDays;
        public int OptedInTesterCount;
        public int ContinuousOptedInTesterCount;
        public int ContinuousTestDays;
        public int BlockingCrashCount;
        public int BlockingAnrCount;
        public int OpenCriticalBugCount;
        public int OpenBlockingFeedbackCount;
        public bool FeedbackReviewed;
        public bool CoreLoopFeedbackReviewed;
    }

    [Serializable]
    public sealed class ClosedTestEvidenceValidationResult
    {
        public bool Valid;
        public string[] Issues = Array.Empty<string>();
    }

    [Serializable]
    public struct ProductionRolloutReadinessEvidence
    {
        public bool ClosedTestUploadEvidenceValid;
        public bool ClosedTestHealthEvidenceValid;
        public bool ClosedTestReleaseAvailableToTesters;
        public bool ClosedTestGoNoGoAuthorized;
        public bool DeveloperAccountTypeReviewed;
        public bool PersonalAccountCreatedAfter2023November13;
        public int ContinuousOptedInTesterCount;
        public int ContinuousTestDays;
        public bool ProductionAccessGranted;
        public bool StoreReleaseChecklistApproved;
        public bool PrivacyDataSafetyApproved;
        public bool ContentRatingApproved;
        public bool TargetAudienceApproved;
        public bool AdsDeclarationApproved;
        public bool AppAccessDeclarationApproved;
        public bool RollbackPlanReviewed;
        public bool MonitoringPlanReviewed;
        public bool ManagedPublishingReviewed;
        public bool NoOutstandingRelease;
        public bool IsFirstProductionRelease;
        public int RequestedRolloutPercent;
        public bool ProductionRolloutAuthorized;
        public bool StorePublishingAuthorized;
    }

    [Serializable]
    public sealed class ProductionRolloutReadinessResult
    {
        public bool MayApplyForProductionAccess;
        public bool MayRolloutProduction;
        public bool UsesStagedRollout;
        public string[] ProductionAccessBlockers = Array.Empty<string>();
        public string[] ProductionRolloutBlockers = Array.Empty<string>();
    }
}
