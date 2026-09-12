using System;

namespace LegendsOfRus.LaunchCandidate
{
    public static class LaunchCandidateBlockerCodes
    {
        public const string AccountRuleNotReviewed = "LCR_ACCOUNT_RULE_NOT_REVIEWED";
        public const string DecisionRequirementUnresolved = "LCR_PRODUCTION_ACCESS_REQUIREMENT_UNRESOLVED";
        public const string DecisionEvidenceMissing = "LCR_PRODUCTION_ACCESS_DECISION_EVIDENCE_MISSING";
        public const string DecisionNotReviewed = "LCR_PRODUCTION_ACCESS_DECISION_NOT_REVIEWED";
        public const string DecisionNotGranted = "LCR_PRODUCTION_ACCESS_NOT_GRANTED";
        public const string CandidateIdInvalid = "LCR_CANDIDATE_ID_INVALID";
        public const string ArtifactHashInvalid = "LCR_ARTIFACT_HASH_INVALID";
        public const string FreezeHashInvalid = "LCR_FREEZE_HASH_INVALID";
        public const string CandidateFreezeMissing = "LCR_CANDIDATE_FREEZE_MISSING";
        public const string CandidateFreezeInvalid = "LCR_CANDIDATE_FREEZE_INVALID";
        public const string ArtifactFreezeMismatch = "LCR_ARTIFACT_FREEZE_MISMATCH";
        public const string ClosedTestEvidenceInvalid = "LCR_CLOSED_TEST_EVIDENCE_INVALID";
        public const string FeedbackTriageNotReady = "LCR_FEEDBACK_TRIAGE_NOT_READY";
        public const string StoreChecklistIncomplete = "LCR_STORE_CHECKLIST_INCOMPLETE";
        public const string PrivacyDataSafetyIncomplete = "LCR_PRIVACY_DATA_SAFETY_INCOMPLETE";
        public const string LegalDocumentsIncomplete = "LCR_LEGAL_DOCUMENTS_INCOMPLETE";
        public const string AppContentDeclarationsIncomplete = "LCR_APP_CONTENT_DECLARATIONS_INCOMPLETE";
        public const string ApplicationIdUnapproved = "LCR_APPLICATION_ID_UNAPPROVED";
        public const string VersioningUnapproved = "LCR_VERSIONING_UNAPPROVED";
        public const string PlayAppSigningIncomplete = "LCR_PLAY_APP_SIGNING_INCOMPLETE";
        public const string UploadKeyWorkflowIncomplete = "LCR_UPLOAD_KEY_WORKFLOW_INCOMPLETE";
        public const string TargetApiInsufficient = "LCR_TARGET_API_INSUFFICIENT";
        public const string AppBundleUnverified = "LCR_AAB_UNVERIFIED";
        public const string Arm64Unverified = "LCR_ARM64_UNVERIFIED";
        public const string PageSize16KbUnverified = "LCR_16KB_PAGE_SIZE_UNVERIFIED";
        public const string ReleaseNotesUnapproved = "LCR_RELEASE_NOTES_UNAPPROVED";
        public const string CountriesRegionsUnapproved = "LCR_COUNTRIES_REGIONS_UNAPPROVED";
        public const string SupportNotReady = "LCR_SUPPORT_NOT_READY";
        public const string MonitoringNotReady = "LCR_MONITORING_NOT_READY";
        public const string RollbackNotReady = "LCR_ROLLBACK_NOT_READY";
        public const string ReleaseHistoryNotReviewed = "LCR_RELEASE_HISTORY_NOT_REVIEWED";
        public const string OutstandingReleaseExists = "LCR_OUTSTANDING_RELEASE_EXISTS";
        public const string FirstReleaseRequiresFullRollout = "LCR_FIRST_RELEASE_REQUIRES_100_PERCENT";
        public const string UpdateRolloutInvalid = "LCR_UPDATE_ROLLOUT_INVALID";
        public const string ProductionReleaseNotAuthorized = "LCR_PRODUCTION_RELEASE_NOT_AUTHORIZED";
        public const string StorePublishingNotAuthorized = "LCR_STORE_PUBLISHING_NOT_AUTHORIZED";
    }

    public enum ProductionAccessDecisionStatus
    {
        Unknown = 0,
        NotRequired = 1,
        Pending = 2,
        Granted = 3,
        Rejected = 4,
        AdditionalTestingRequired = 5
    }

    [Serializable]
    public sealed class ProductionAccessDecisionEvidence
    {
        public int SchemaVersion = LaunchCandidateRequirements.SchemaVersion;
        public bool DeveloperAccountRuleReviewed;
        public bool ProductionAccessRequirementResolved;
        public bool ProductionAccessDecisionRequired;
        public bool DecisionEvidenceRetained;
        public bool DecisionReviewed;
        public ProductionAccessDecisionStatus DecisionStatus;
    }

    [Serializable]
    public sealed class ProductionAccessDecisionResult
    {
        public bool Satisfied;
        public bool ProductionAccessGrantedOrNotRequired;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class LaunchCandidateEvidence
    {
        public int SchemaVersion = LaunchCandidateRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public string CandidateFreezeManifestSha256 = string.Empty;
        public bool CandidateFreezeManifestRetained;
        public bool CandidateFreezeVerified;
        public bool ArtifactMatchesFreeze;
        public bool ClosedTestExecutionEvidenceValid;
        public bool FeedbackTriageReady;
        public bool ProductionAccessDecisionSatisfied;
        public bool StoreReleaseChecklistComplete;
        public bool PrivacyAndDataSafetyApproved;
        public bool LegalDocumentsApproved;
        public bool AppContentDeclarationsApproved;
        public bool ProductionApplicationIdApproved;
        public bool VersioningApproved;
        public bool PlayAppSigningConfigured;
        public bool UploadKeyWorkflowReviewed;
        public int TargetApiLevel;
        public bool AppBundleVerified;
        public bool Arm64Verified;
        public bool PageSize16KbVerified;
        public bool ReleaseNotesApproved;
        public bool ProductionCountriesRegionsApproved;
        public bool SupportReadinessApproved;
        public bool LaunchMonitoringApproved;
        public bool RollbackPlanApproved;
        public bool ReleaseHistoryReviewed;
        public bool OutstandingReleasesCleared;
        public bool FirstProductionRelease;
        public int RequestedRolloutPercent;
        public bool ProductionReleaseAuthorized;
        public bool StorePublishingAuthorized;
    }

    [Serializable]
    public sealed class LaunchCandidateReadinessResult
    {
        public bool ReadyForHumanLaunchReview;
        public bool MayCreateProductionReleaseDraft;
        public bool MayStartManualProductionRollout;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] DraftBlockers = Array.Empty<string>();
        public string[] RolloutBlockers = Array.Empty<string>();
    }
}
