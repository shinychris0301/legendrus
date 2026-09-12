using System;

namespace LegendsOfRus.IncidentResponse
{
    public static class IncidentResponseBlockerCodes
    {
        public const string RegisterMissing = "LOR_IR_REGISTER_MISSING";
        public const string RegisterNotReviewed = "LOR_IR_REGISTER_NOT_REVIEWED";
        public const string IncidentIdInvalid = "LOR_IR_INCIDENT_ID_INVALID";
        public const string IncidentIdDuplicate = "LOR_IR_INCIDENT_ID_DUPLICATE";
        public const string IncidentCountExceeded = "LOR_IR_INCIDENT_COUNT_EXCEEDED";
        public const string CriticalIncidentOpen = "LOR_IR_CRITICAL_INCIDENT_OPEN";
        public const string HighIncidentOpen = "LOR_IR_HIGH_INCIDENT_OPEN";
        public const string DataLossRiskOpen = "LOR_IR_DATA_LOSS_RISK_OPEN";
        public const string PurchaseIntegrityRiskOpen = "LOR_IR_PURCHASE_INTEGRITY_RISK_OPEN";
        public const string RetestPending = "LOR_IR_RETEST_PENDING";
        public const string IncidentStateInvalid = "LOR_IR_INCIDENT_STATE_INVALID";
        public const string HotfixEvidenceMissing = "LOR_IR_HOTFIX_EVIDENCE_MISSING";
        public const string HotfixIdInvalid = "LOR_IR_HOTFIX_ID_INVALID";
        public const string HotfixCandidateIdInvalid = "LOR_IR_HOTFIX_CANDIDATE_ID_INVALID";
        public const string BaseArtifactHashInvalid = "LOR_IR_BASE_ARTIFACT_HASH_INVALID";
        public const string HotfixArtifactHashInvalid = "LOR_IR_HOTFIX_ARTIFACT_HASH_INVALID";
        public const string HotfixArtifactUnchanged = "LOR_IR_HOTFIX_ARTIFACT_UNCHANGED";
        public const string LinkedIncidentMissing = "LOR_IR_LINKED_INCIDENT_MISSING";
        public const string LinkedIncidentInvalid = "LOR_IR_LINKED_INCIDENT_INVALID";
        public const string RootCauseNotReviewed = "LOR_IR_ROOT_CAUSE_NOT_REVIEWED";
        public const string ScopeNotReviewed = "LOR_IR_SCOPE_NOT_REVIEWED";
        public const string ScopeTooLarge = "LOR_IR_SCOPE_TOO_LARGE";
        public const string SourceChangeNotReviewed = "LOR_IR_SOURCE_CHANGE_NOT_REVIEWED";
        public const string CompileNotVerified = "LOR_IR_COMPILE_NOT_VERIFIED";
        public const string EditModeNotVerified = "LOR_IR_EDITMODE_NOT_VERIFIED";
        public const string PlayModeNotVerified = "LOR_IR_PLAYMODE_NOT_VERIFIED";
        public const string DeviceSmokeNotVerified = "LOR_IR_DEVICE_SMOKE_NOT_VERIFIED";
        public const string SaveRiskNotReviewed = "LOR_IR_SAVE_RISK_NOT_REVIEWED";
        public const string EconomyRiskNotReviewed = "LOR_IR_ECONOMY_RISK_NOT_REVIEWED";
        public const string PrivacyRiskNotReviewed = "LOR_IR_PRIVACY_RISK_NOT_REVIEWED";
        public const string StoreImpactNotReviewed = "LOR_IR_STORE_IMPACT_NOT_REVIEWED";
        public const string VersionCodeNotReviewed = "LOR_IR_VERSION_CODE_NOT_REVIEWED";
        public const string AabEvidenceMissing = "LOR_IR_AAB_EVIDENCE_MISSING";
        public const string SigningEvidenceNotReviewed = "LOR_IR_SIGNING_EVIDENCE_NOT_REVIEWED";
        public const string HotfixReleaseNotAuthorized = "LOR_IR_HOTFIX_RELEASE_NOT_AUTHORIZED";
        public const string RolloutEvidenceNotReviewed = "LOR_IR_ROLLOUT_EVIDENCE_NOT_REVIEWED";
        public const string RolloutPercentInvalid = "LOR_IR_ROLLOUT_PERCENT_INVALID";
        public const string RequestedPercentInvalid = "LOR_IR_REQUESTED_PERCENT_INVALID";
        public const string RolloutActionInvalidForState = "LOR_IR_ACTION_INVALID_FOR_STATE";
        public const string FirstReleaseCannotHaltFull = "LOR_IR_FIRST_RELEASE_CANNOT_HALT_FULL";
        public const string FirstReleaseCannotBeStaged = "LOR_IR_FIRST_RELEASE_CANNOT_BE_STAGED";
        public const string PreviousReleaseUnavailable = "LOR_IR_PREVIOUS_RELEASE_UNAVAILABLE";
        public const string BlockingIssueNotCleared = "LOR_IR_BLOCKING_ISSUE_NOT_CLEARED";
        public const string HotfixNotQualified = "LOR_IR_HOTFIX_NOT_QUALIFIED";
        public const string RolloutActionNotAuthorized = "LOR_IR_ROLLOUT_ACTION_NOT_AUTHORIZED";
        public const string AndroidVitalsNotReviewed = "LOR_IR_ANDROID_VITALS_NOT_REVIEWED";
    }

    public enum IncidentCategory
    {
        Unknown = 0,
        Crash = 1,
        Anr = 2,
        DataLoss = 3,
        PurchaseIntegrity = 4,
        Functional = 5,
        Performance = 6,
        UiVisual = 7,
        LocalizationAccessibility = 8,
        PolicyCompliance = 9,
        BackendService = 10,
        Other = 11
    }

    public enum IncidentSeverity
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum IncidentLifecycleState
    {
        Detected = 0,
        Investigating = 1,
        Mitigated = 2,
        FixedPendingRetest = 3,
        Closed = 4
    }

    public enum IncidentResponseRecommendation
    {
        Observe = 0,
        ContinueMonitoring = 1,
        HoldRolloutReview = 2,
        HotfixQualificationReview = 3,
        EscalateRolloutControlReview = 4
    }

    public enum RolloutTrackState
    {
        NotStarted = 0,
        StagedActive = 1,
        StagedHalted = 2,
        FullyRolledOut = 3,
        FullReleaseHalted = 4
    }

    public enum RolloutControlAction
    {
        None = 0,
        Hold = 1,
        HaltCurrentRelease = 2,
        ResumeCurrentRelease = 3,
        IncreaseRollout = 4,
        ReplaceWithQualifiedHotfix = 5,
        EscalateRollbackReview = 6
    }

    [Serializable]
    public sealed class IncidentRecord
    {
        public string IncidentId = string.Empty;
        public IncidentCategory Category;
        public IncidentSeverity Severity = IncidentSeverity.Low;
        public IncidentLifecycleState State;
        public bool ProductionBlocking;
        public bool UserDataLossRisk;
        public bool PurchaseIntegrityRisk;
        public bool CrashOrAnrCluster;
        public bool RootCauseReviewed;
        public bool MitigationReviewed;
        public bool RetestRequired;
        public bool RetestPassed;
    }

    [Serializable]
    public sealed class IncidentRegisterEvidence
    {
        public int SchemaVersion = IncidentResponseRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public IncidentRecord[] Incidents = Array.Empty<IncidentRecord>();
    }

    [Serializable]
    public sealed class IncidentTriageResult
    {
        public bool RegisterStructurallyValid;
        public bool ProductionHealthClear;
        public int OpenCritical;
        public int OpenHigh;
        public int OpenMedium;
        public int OpenLow;
        public int PendingRetest;
        public IncidentResponseRecommendation Recommendation;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class HotfixCandidateEvidence
    {
        public int SchemaVersion = IncidentResponseRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public string HotfixId = string.Empty;
        public string BaseCandidateId = string.Empty;
        public string BaseArtifactSha256 = string.Empty;
        public string HotfixCandidateId = string.Empty;
        public string HotfixArtifactSha256 = string.Empty;
        public string[] LinkedIncidentIds = Array.Empty<string>();
        public bool RootCauseReviewed;
        public bool ScopeReviewed;
        public int ChangedScopeUnits;
        public bool SourceChangeReviewed;
        public bool UnityCompilePassed;
        public bool EditModePassed;
        public bool PlayModePassed;
        public bool DeviceSmokePassed;
        public bool SaveMigrationRiskReviewed;
        public bool EconomyRiskReviewed;
        public bool PrivacyRiskReviewed;
        public bool StoreListingImpactReviewed;
        public bool VersionCodeReviewed;
        public bool AabEvidenceRetained;
        public bool SigningEvidenceReviewed;
        public bool ManualHotfixReleaseAuthorized;
    }

    [Serializable]
    public sealed class HotfixQualificationResult
    {
        public bool ReadyForHumanHotfixReview;
        public bool QualifiedForManualReleasePreparation;
        public bool MayProceedWithAuthorizedManualHotfixRelease;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] ReleaseBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class RolloutControlReview
    {
        public int SchemaVersion = IncidentResponseRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public bool FirstReleaseOnTrack;
        public RolloutTrackState CurrentState;
        public int CurrentRolloutPercent;
        public bool PreviousEligibleReleaseAvailable;
        public bool CurrentArtifactHasBlockingIssue;
        public bool IncidentTriageReviewed;
        public bool IncidentHealthClear;
        public bool AndroidVitalsReviewed;
        public bool QualifiedHotfixAvailable;
        public RolloutControlAction RequestedAction;
        public int RequestedRolloutPercent;
        public bool ManualRolloutActionAuthorized;
    }

    [Serializable]
    public sealed class RolloutControlResult
    {
        public bool ReadyForHumanRolloutControlReview;
        public bool RequestedActionValid;
        public bool MayApplyRequestedActionManually;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] ActionBlockers = Array.Empty<string>();
    }
}
