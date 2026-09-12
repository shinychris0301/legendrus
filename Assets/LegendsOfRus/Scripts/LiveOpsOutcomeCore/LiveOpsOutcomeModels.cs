using System;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class LiveOpsOutcomeBlockerCodes
    {
        public const string OutcomeMissing="LOR_OUTCOME_MISSING";
        public const string OutcomeNotReviewed="LOR_OUTCOME_NOT_REVIEWED";
        public const string OutcomeInvalid="LOR_OUTCOME_INVALID";
        public const string OutcomeSampleInsufficient="LOR_OUTCOME_SAMPLE_INSUFFICIENT";
        public const string OutcomeCrashObserved="LOR_OUTCOME_CRASH_OBSERVED";
        public const string OutcomeBlockingIncidentOpen="LOR_OUTCOME_BLOCKING_INCIDENT_OPEN";
        public const string OutcomeRewardReconciliationMismatch="LOR_OUTCOME_REWARD_RECONCILIATION_MISMATCH";
        public const string OutcomeSupportEscalationOpen="LOR_OUTCOME_SUPPORT_ESCALATION_OPEN";
        public const string OutcomeContentHealthReviewMissing="LOR_OUTCOME_CONTENT_HEALTH_REVIEW_MISSING";
        public const string OutcomeEconomyImpactReviewMissing="LOR_OUTCOME_ECONOMY_IMPACT_REVIEW_MISSING";
        public const string OutcomePrivacyImpactReviewMissing="LOR_OUTCOME_PRIVACY_IMPACT_REVIEW_MISSING";
        public const string DecisionMissing="LOR_DECISION_MISSING";
        public const string DecisionNotReviewed="LOR_DECISION_NOT_REVIEWED";
        public const string DecisionInvalid="LOR_DECISION_INVALID";
        public const string DecisionSampleInsufficient="LOR_DECISION_SAMPLE_INSUFFICIENT";
        public const string DecisionDataQualityFailure="LOR_DECISION_DATA_QUALITY_FAILURE";
        public const string DecisionSampleRatioUnhealthy="LOR_DECISION_SAMPLE_RATIO_UNHEALTHY";
        public const string DecisionGuardrailConflict="LOR_DECISION_GUARDRAIL_CONFLICT";
        public const string DecisionWindowIncomplete="LOR_DECISION_WINDOW_INCOMPLETE";
        public const string DecisionStoppingRuleReviewMissing="LOR_DECISION_STOPPING_RULE_REVIEW_MISSING";
        public const string DecisionBasisReviewMissing="LOR_DECISION_BASIS_REVIEW_MISSING";
        public const string CloseoutMissing="LOR_CLOSEOUT_MISSING";
        public const string CloseoutNotReviewed="LOR_CLOSEOUT_NOT_REVIEWED";
        public const string CloseoutInvalid="LOR_CLOSEOUT_INVALID";
        public const string CloseoutEntriesRemaining="LOR_CLOSEOUT_ENTRIES_REMAINING";
        public const string CloseoutRewardClaimsPending="LOR_CLOSEOUT_REWARD_CLAIMS_PENDING";
        public const string CloseoutOrphanedContent="LOR_CLOSEOUT_ORPHANED_CONTENT";
        public const string CloseoutCooldownIncomplete="LOR_CLOSEOUT_COOLDOWN_INCOMPLETE";
        public const string CloseoutVersionsNotArchived="LOR_CLOSEOUT_VERSIONS_NOT_ARCHIVED";
        public const string CloseoutSupportHandoffMissing="LOR_CLOSEOUT_SUPPORT_HANDOFF_MISSING";
        public const string CloseoutWindowIncomplete="LOR_CLOSEOUT_WINDOW_INCOMPLETE";
        public const string ProductionAnalyticsDisabled="LOR_OUTCOME_PROD_ANALYTICS_DISABLED";
        public const string ProductionRemoteConfigDisabled="LOR_OUTCOME_PROD_REMOTE_CONFIG_DISABLED";
        public const string OutcomeReviewNotAuthorized="LOR_OUTCOME_REVIEW_NOT_AUTHORIZED";
        public const string ExperimentDecisionNotAuthorized="LOR_EXPERIMENT_DECISION_NOT_AUTHORIZED";
        public const string RotationCloseoutNotAuthorized="LOR_ROTATION_CLOSEOUT_NOT_AUTHORIZED";
        public const string ManualCloseoutReviewNotAuthorized="LOR_MANUAL_CLOSEOUT_REVIEW_NOT_AUTHORIZED";
    }

    public enum ExperimentDecisionKind
    {
        None=0,
        KeepControl=1,
        AdoptTreatment=2,
        StopForGuardrail=3,
        Inconclusive=4
    }

    public enum ExperimentDecisionRecommendation
    {
        InsufficientEvidence=0,
        ContinueObservation=1,
        RevisitDecision=2,
        ArchiveControl=3,
        ArchiveTreatment=4,
        ArchiveStoppedForGuardrail=5,
        ArchiveInconclusive=6
    }

    public enum LiveOpsOutcomeRecommendation
    {
        InsufficientEvidence=0,
        ResolveOutcomeBlockers=1,
        RevisitExperimentDecision=2,
        CompleteRotationCloseout=3,
        ContinueObservation=4,
        ReadyForManualCloseoutReview=5
    }

    [Serializable]
    public sealed class LiveOpsOutcomeEvidence
    {
        public int SchemaVersion=LiveOpsOutcomeRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string OutcomeId=string.Empty;
        public string ExecutionId=string.Empty;
        public int WindowStartMinute;
        public int WindowEndMinute;
        public int ActiveParticipants;
        public int ContentEntries;
        public int ContentCompletions;
        public int ContentCrashes;
        public int OpenBlockingIncidents;
        public int RewardClaimsExpected;
        public int RewardClaimsGranted;
        public int OpenSupportEscalations;
        public bool ContentHealthReviewed;
        public bool EconomyImpactReviewed;
        public bool PrivacyImpactReviewed;
    }

    [Serializable]
    public sealed class ExperimentDecisionEvidence
    {
        public int SchemaVersion=LiveOpsOutcomeRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string DecisionId=string.Empty;
        public string ObservationId=string.Empty;
        public string ExperimentId=string.Empty;
        public ExperimentDecisionKind Decision;
        public int TotalSamples;
        public int PrimaryMetricDeltaBasisPoints;
        public int GuardrailBreaches;
        public bool DataQualityPassed;
        public bool SampleRatioHealthy;
        public bool ExperimentWindowComplete;
        public bool StoppingRuleReviewed;
        public bool DecisionBasisReviewed;
    }

    [Serializable]
    public sealed class RotationCloseoutEvidence
    {
        public int SchemaVersion=LiveOpsOutcomeRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string CloseoutId=string.Empty;
        public string RotationPlanId=string.Empty;
        public int PlannedEntries;
        public int ClosedEntries;
        public int ActiveEntriesRemaining;
        public int PendingRewardClaims;
        public int OrphanedContentReferences;
        public bool CooldownsSatisfied;
        public bool ContentVersionsArchived;
        public bool SupportHandoffComplete;
        public bool CloseoutWindowComplete;
    }

    [Serializable]
    public sealed class OutcomeReviewResult
    {
        public bool StructurallyValid;
        public bool SampleSufficient;
        public bool OutcomeHealthy;
        public int CompletionRateBasisPoints;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class ExperimentDecisionReviewResult
    {
        public bool StructurallyValid;
        public bool DecisionFinalized;
        public bool ReadyForArchive;
        public ExperimentDecisionRecommendation Recommendation;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class RotationCloseoutReviewResult
    {
        public bool StructurallyValid;
        public bool CloseoutComplete;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsOutcomeReviewResult
    {
        public bool ReadyForHumanReview;
        public bool OutcomeHealthy;
        public bool ExperimentDecisionReady;
        public bool RotationCloseoutComplete;
        public bool MayProceedWithManualCloseoutReview;
        public LiveOpsOutcomeRecommendation Recommendation;
        public string[] ReviewBlockers=Array.Empty<string>();
        public string[] ActionBlockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsOutcomeAggregateObservation
    {
        public int WindowIndex;
        public int ActiveParticipants;
        public int ContentEntries;
        public int ContentCompletions;
        public int RewardClaimsGranted;
        public int SupportEscalations;
    }
}
