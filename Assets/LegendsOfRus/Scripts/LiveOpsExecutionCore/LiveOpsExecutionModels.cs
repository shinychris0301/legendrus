using System;

namespace LegendsOfRus.LiveOpsExecution
{
    public static class LiveOpsExecutionBlockerCodes
    {
        public const string ExecutionMissing="LOR_EXEC_MISSING";
        public const string ExecutionNotReviewed="LOR_EXEC_NOT_REVIEWED";
        public const string ExecutionInvalid="LOR_EXEC_INVALID";
        public const string RotationMissing="LOR_ROT_MISSING";
        public const string RotationNotReviewed="LOR_ROT_NOT_REVIEWED";
        public const string RotationInvalid="LOR_ROT_INVALID";
        public const string RotationOverlap="LOR_ROT_OVERLAP";
        public const string RotationCooldownInsufficient="LOR_ROT_COOLDOWN_INSUFFICIENT";
        public const string RotationContentVersionMissing="LOR_ROT_CONTENT_VERSION_MISSING";
        public const string ObservationMissing="LOR_OBS_MISSING";
        public const string ObservationNotReviewed="LOR_OBS_NOT_REVIEWED";
        public const string ObservationInvalid="LOR_OBS_INVALID";
        public const string ObservationSampleInsufficient="LOR_OBS_SAMPLE_INSUFFICIENT";
        public const string ObservationSampleRatioMismatch="LOR_OBS_SAMPLE_RATIO_MISMATCH";
        public const string ObservationDataQualityFailure="LOR_OBS_DATA_QUALITY_FAILURE";
        public const string ObservationGuardrailBreach="LOR_OBS_GUARDRAIL_BREACH";
        public const string ObservationStoppingRuleTriggered="LOR_OBS_STOPPING_RULE_TRIGGERED";
        public const string ObservationPrivacyReviewMissing="LOR_OBS_PRIVACY_REVIEW_MISSING";
        public const string ExecutionNotAuthorized="LOR_EXEC_NOT_AUTHORIZED";
        public const string RotationNotAuthorized="LOR_ROT_NOT_AUTHORIZED";
        public const string ObservationNotAuthorized="LOR_OBS_NOT_AUTHORIZED";
        public const string TrafficChangeNotAuthorized="LOR_TRAFFIC_CHANGE_NOT_AUTHORIZED";
        public const string ProductionAnalyticsDisabled="LOR_EXEC_PROD_ANALYTICS_DISABLED";
        public const string ProductionRemoteConfigDisabled="LOR_EXEC_PROD_REMOTE_CONFIG_DISABLED";
        public const string ManualExecutionReviewNotAuthorized="LOR_EXEC_MANUAL_REVIEW_NOT_AUTHORIZED";
    }

    public enum ExperimentObservationRecommendation
    {
        InsufficientEvidence=0,
        ContinueObservation=1,
        PauseForDataQualityReview=2,
        PauseForSampleRatioReview=3,
        StopForGuardrailReview=4,
        StopForStoppingRuleReview=5,
        ConcludeForHumanReview=6
    }

    public enum LiveOpsExecutionRecommendation
    {
        InsufficientEvidence=0,
        ReviseRotation=1,
        ContinueObservation=2,
        PauseExperimentReview=3,
        StopExperimentReview=4,
        ReadyForManualExecutionReview=5
    }

    [Serializable]
    public sealed class LiveOpsExecutionEvidence
    {
        public int SchemaVersion=LiveOpsExecutionRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ExecutionId=string.Empty;
        public string ApprovedReadinessId=string.Empty;
        public int WindowStartMinute;
        public int WindowEndMinute;
        public bool PreflightPassed;
        public bool SupportCoverageConfirmed;
        public bool RollbackPlanConfirmed;
    }

    [Serializable]
    public sealed class ContentRotationEntryEvidence
    {
        public string RotationId=string.Empty;
        public string ContentId=string.Empty;
        public string ContentVersion=string.Empty;
        public int StartMinute;
        public int EndMinute;
        public int RequiredCooldownMinutes;
    }

    [Serializable]
    public sealed class ContentRotationEvidence
    {
        public int SchemaVersion=LiveOpsExecutionRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string RotationPlanId=string.Empty;
        public ContentRotationEntryEvidence[] Entries=Array.Empty<ContentRotationEntryEvidence>();
    }

    [Serializable]
    public sealed class ExperimentObservationWindow
    {
        public int WindowIndex;
        public int ControlSamples;
        public int TreatmentSamples;
        public int PrimaryMetricDeltaBasisPoints;
        public int GuardrailBreaches;
        public bool DataQualityPassed;
    }

    [Serializable]
    public sealed class ExperimentObservationEvidence
    {
        public int SchemaVersion=LiveOpsExecutionRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ObservationId=string.Empty;
        public string ExperimentId=string.Empty;
        public bool PrivacyReviewed;
        public bool StoppingRuleTriggered;
        public bool ExperimentWindowComplete;
        public ExperimentObservationWindow[] Windows=Array.Empty<ExperimentObservationWindow>();
    }

    [Serializable]
    public sealed class RotationReviewResult
    {
        public bool StructurallyValid;
        public bool RotationSafe;
        public int EntryCount;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class ExperimentObservationResult
    {
        public bool StructurallyValid;
        public bool SampleSufficient;
        public bool SampleRatioHealthy;
        public bool DataQualityHealthy;
        public bool GuardrailsHealthy;
        public int TotalControlSamples;
        public int TotalTreatmentSamples;
        public int SampleRatioMismatchBasisPoints;
        public int TotalGuardrailBreaches;
        public ExperimentObservationRecommendation Recommendation;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsExecutionReviewResult
    {
        public bool ReadyForHumanReview;
        public bool RotationSafe;
        public bool ExperimentObservationHealthy;
        public bool MayProceedWithManualExecutionReview;
        public LiveOpsExecutionRecommendation Recommendation;
        public string[] ReviewBlockers=Array.Empty<string>();
        public string[] ActionBlockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsExecutionAggregateObservation
    {
        public int WindowIndex;
        public int ActiveParticipants;
        public int ContentEntries;
        public int ContentCompletions;
        public int ExperimentControlSamples;
        public int ExperimentTreatmentSamples;
        public int GuardrailBreaches;
    }
}
