using System;

namespace LegendsOfRus.LaunchOperations
{
    public static class LaunchOperationsBlockerCodes
    {
        public const string CandidateIdInvalid = "LOR_OPS_CANDIDATE_ID_INVALID";
        public const string ArtifactHashInvalid = "LOR_OPS_ARTIFACT_HASH_INVALID";
        public const string LaunchCandidateNotReady = "LOR_OPS_LAUNCH_CANDIDATE_NOT_READY";
        public const string ProductionReleaseNotAuthorized = "LOR_OPS_PRODUCTION_RELEASE_NOT_AUTHORIZED";
        public const string StorePublishingNotAuthorized = "LOR_OPS_STORE_PUBLISHING_NOT_AUTHORIZED";
        public const string MonitoringPlanNotApproved = "LOR_OPS_MONITORING_PLAN_NOT_APPROVED";
        public const string IncidentResponsePlanNotApproved = "LOR_OPS_INCIDENT_RESPONSE_PLAN_NOT_APPROVED";
        public const string RollbackPlanNotApproved = "LOR_OPS_ROLLBACK_PLAN_NOT_APPROVED";
        public const string SupportCoverageNotApproved = "LOR_OPS_SUPPORT_COVERAGE_NOT_APPROVED";
        public const string ReleaseHistoryNotReviewed = "LOR_OPS_RELEASE_HISTORY_NOT_REVIEWED";
        public const string OutstandingReleaseExists = "LOR_OPS_OUTSTANDING_RELEASE_EXISTS";
        public const string FirstReleaseRequiresFullRollout = "LOR_OPS_FIRST_RELEASE_REQUIRES_100_PERCENT";
        public const string UpdateRolloutInvalid = "LOR_OPS_UPDATE_ROLLOUT_INVALID";
        public const string LaunchStartNotAuthorized = "LOR_OPS_LAUNCH_START_NOT_AUTHORIZED";
        public const string VitalsEvidenceMissing = "LOR_OPS_VITALS_EVIDENCE_MISSING";
        public const string VitalsEvidenceNotReviewed = "LOR_OPS_VITALS_EVIDENCE_NOT_REVIEWED";
        public const string VitalsWindowInvalid = "LOR_OPS_VITALS_WINDOW_INVALID";
        public const string CrashThresholdExceeded = "LOR_OPS_CRASH_THRESHOLD_EXCEEDED";
        public const string AnrThresholdExceeded = "LOR_OPS_ANR_THRESHOLD_EXCEEDED";
        public const string DeviceCrashThresholdExceeded = "LOR_OPS_DEVICE_CRASH_THRESHOLD_EXCEEDED";
        public const string DeviceAnrThresholdExceeded = "LOR_OPS_DEVICE_ANR_THRESHOLD_EXCEEDED";
        public const string WakeLockThresholdExceeded = "LOR_OPS_WAKELOCK_THRESHOLD_EXCEEDED";
        public const string BatteryThresholdExceeded = "LOR_OPS_BATTERY_THRESHOLD_EXCEEDED";
        public const string BlockingIncidentOpen = "LOR_OPS_BLOCKING_INCIDENT_OPEN";
        public const string HighSeverityIncidentOpen = "LOR_OPS_HIGH_SEVERITY_INCIDENT_OPEN";
        public const string CriticalIncidentOpen = "LOR_OPS_CRITICAL_INCIDENT_OPEN";
        public const string IncidentRegisterNotReviewed = "LOR_OPS_INCIDENT_REGISTER_NOT_REVIEWED";
        public const string RolloutActionNotAuthorized = "LOR_OPS_ROLLOUT_ACTION_NOT_AUTHORIZED";
    }

    public enum ReleaseKind
    {
        FirstProductionRelease = 0,
        ProductionUpdate = 1
    }

    public enum LaunchHealthClassification
    {
        Unknown = 0,
        Healthy = 1,
        Watch = 2,
        Unhealthy = 3,
        Critical = 4
    }

    public enum LaunchRolloutRecommendation
    {
        NoLaunch = 0,
        Observe = 1,
        Continue = 2,
        Hold = 3,
        HaltStagedRollout = 4,
        EscalateRollbackReview = 5
    }

    public enum LaunchIncidentSeverity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    [Serializable]
    public sealed class LaunchOperationsPlan
    {
        public int SchemaVersion = LaunchOperationsRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public bool LaunchCandidateReadinessVerified;
        public bool ProductionReleaseAuthorized;
        public bool StorePublishingAuthorized;
        public bool MonitoringPlanApproved;
        public bool IncidentResponsePlanApproved;
        public bool RollbackPlanApproved;
        public bool SupportCoverageApproved;
        public bool ReleaseHistoryReviewed;
        public bool OutstandingReleasesCleared;
        public ReleaseKind ReleaseKind;
        public int RequestedRolloutPercent;
        public bool ManualLaunchStartAuthorized;
    }

    [Serializable]
    public sealed class LaunchOperationsPreflightResult
    {
        public bool ReadyForHumanLaunchOperationsReview;
        public bool MayStartManualProductionLaunch;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] LaunchBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class AndroidVitalsSnapshot
    {
        public int SchemaVersion = LaunchOperationsRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public int WindowDays;
        public double UserPerceivedCrashRatePercent;
        public double UserPerceivedAnrRatePercent;
        public double MaxPhoneModelCrashRatePercent;
        public double MaxPhoneModelAnrRatePercent;
        public double ExcessivePartialWakeLockRatePercent;
        public double ExcessiveBatteryUsageRatePercent;
    }

    [Serializable]
    public sealed class LaunchIncidentSummary
    {
        public int SchemaVersion = LaunchOperationsRequirements.SchemaVersion;
        public bool RegisterRetained;
        public bool RegisterReviewed;
        public int OpenLow;
        public int OpenMedium;
        public int OpenHigh;
        public int OpenCritical;
        public bool BlockingRegressionOpen;
        public bool CrashOrAnrClusterUnderInvestigation;
    }

    [Serializable]
    public sealed class LaunchHealthEvaluation
    {
        public LaunchHealthClassification Classification;
        public LaunchRolloutRecommendation Recommendation;
        public bool MayContinueOrIncreaseRollout;
        public bool HumanIncidentReviewRequired;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class LaunchObservationRecord
    {
        public int Sequence;
        public LaunchHealthClassification Classification;
        public LaunchRolloutRecommendation Recommendation;
        public int VitalsWindowDays;
        public int OpenHighIncidents;
        public int OpenCriticalIncidents;
    }
}
