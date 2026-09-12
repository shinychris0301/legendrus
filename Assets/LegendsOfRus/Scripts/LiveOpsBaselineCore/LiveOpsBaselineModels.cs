using System;

namespace LegendsOfRus.LiveOpsBaseline
{
    public static class LiveOpsBaselineBlockerCodes
    {
        public const string ChainMissing="LOR_BASE_CHAIN_MISSING";
        public const string ChainNotReviewed="LOR_BASE_CHAIN_NOT_REVIEWED";
        public const string ChainInvalid="LOR_BASE_CHAIN_INVALID";
        public const string ChainStepMissing="LOR_BASE_CHAIN_STEP_MISSING";
        public const string ChainSourceHashInvalid="LOR_BASE_CHAIN_SOURCE_HASH_INVALID";
        public const string ChainArchiveHashInvalid="LOR_BASE_CHAIN_ARCHIVE_HASH_INVALID";
        public const string ChainCriticalBlockersOpen="LOR_BASE_CHAIN_CRITICAL_BLOCKERS_OPEN";
        public const string ChainUnityEvidenceMissing="LOR_BASE_CHAIN_UNITY_EVIDENCE_MISSING";
        public const string ChainPrivacyBoundaryNotReviewed="LOR_BASE_CHAIN_PRIVACY_NOT_REVIEWED";
        public const string ChainMutationBoundaryNotReviewed="LOR_BASE_CHAIN_MUTATION_NOT_REVIEWED";
        public const string BaselineMissing="LOR_OPS_BASELINE_MISSING";
        public const string BaselineNotReviewed="LOR_OPS_BASELINE_NOT_REVIEWED";
        public const string BaselineInvalid="LOR_OPS_BASELINE_INVALID";
        public const string BaselineChainMismatch="LOR_OPS_BASELINE_CHAIN_MISMATCH";
        public const string BaselineRunbooksIncomplete="LOR_OPS_BASELINE_RUNBOOKS_INCOMPLETE";
        public const string BaselineObservabilityMissing="LOR_OPS_BASELINE_OBSERVABILITY_MISSING";
        public const string BaselineIncidentMissing="LOR_OPS_BASELINE_INCIDENT_MISSING";
        public const string BaselineHotfixMissing="LOR_OPS_BASELINE_HOTFIX_MISSING";
        public const string BaselineLiveOpsMissing="LOR_OPS_BASELINE_LIVEOPS_MISSING";
        public const string BaselinePrivacySupportMissing="LOR_OPS_BASELINE_PRIVACY_SUPPORT_MISSING";
        public const string BaselineRollbackMissing="LOR_OPS_BASELINE_ROLLBACK_MISSING";
        public const string BaselineArchiveNotPinned="LOR_OPS_BASELINE_ARCHIVE_NOT_PINNED";
        public const string BaselineCriticalActionsOpen="LOR_OPS_BASELINE_CRITICAL_ACTIONS_OPEN";
        public const string TransitionMissing="LOR_V120_TRANSITION_MISSING";
        public const string TransitionNotReviewed="LOR_V120_TRANSITION_NOT_REVIEWED";
        public const string TransitionInvalid="LOR_V120_TRANSITION_INVALID";
        public const string TransitionBaselineMismatch="LOR_V120_TRANSITION_BASELINE_MISMATCH";
        public const string TransitionLeadTimeInsufficient="LOR_V120_TRANSITION_LEAD_TIME_INSUFFICIENT";
        public const string TransitionScopeMissing="LOR_V120_TRANSITION_SCOPE_MISSING";
        public const string TransitionChapterPipelineMissing="LOR_V120_TRANSITION_CHAPTER_PIPELINE_MISSING";
        public const string TransitionContentOwnershipMissing="LOR_V120_TRANSITION_CONTENT_OWNERSHIP_MISSING";
        public const string TransitionAddressablesReviewMissing="LOR_V120_TRANSITION_ADDRESSABLES_REVIEW_MISSING";
        public const string TransitionLocalizationMissing="LOR_V120_TRANSITION_LOCALIZATION_MISSING";
        public const string TransitionQaMissing="LOR_V120_TRANSITION_QA_MISSING";
        public const string TransitionAutomationMissing="LOR_V120_TRANSITION_AUTOMATION_MISSING";
        public const string TransitionCapacityMissing="LOR_V120_TRANSITION_CAPACITY_MISSING";
        public const string TransitionCarryoverBlockers="LOR_V120_TRANSITION_CARRYOVER_BLOCKERS";
        public const string ChainReviewNotAuthorized="LOR_BASE_CHAIN_REVIEW_NOT_AUTHORIZED";
        public const string BaselineReviewNotAuthorized="LOR_OPS_BASELINE_REVIEW_NOT_AUTHORIZED";
        public const string TransitionReviewNotAuthorized="LOR_V120_TRANSITION_REVIEW_NOT_AUTHORIZED";
        public const string V119ClosureNotAuthorized="LOR_V119_CLOSURE_NOT_AUTHORIZED";
        public const string V120PreparationNotAuthorized="LOR_V120_PREPARATION_NOT_AUTHORIZED";
        public const string LiveOpsPublishingNotAuthorized="LOR_BASE_LIVEOPS_PUBLISH_NOT_AUTHORIZED";
        public const string ProductionAnalyticsDisabled="LOR_BASE_PROD_ANALYTICS_DISABLED";
        public const string ProductionRemoteConfigDisabled="LOR_BASE_PROD_REMOTE_CONFIG_DISABLED";
    }

    public enum LiveOpsBaselineRecommendation
    {
        InsufficientEvidence=0,
        CompleteV119Chain=1,
        CompleteOperatingBaseline=2,
        ReviseV120Transition=3,
        ReadyForManualV119Closure=4
    }

    [Serializable]
    public sealed class V119EvidenceChainEvidence
    {
        public int SchemaVersion=LiveOpsBaselineRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ChainId=string.Empty;
        public string SourceTreeSha256=string.Empty;
        public string ArchiveSha256=string.Empty;
        public bool Step01Reviewed;
        public bool Step02Reviewed;
        public bool Step03Reviewed;
        public bool Step04Reviewed;
        public bool Step05Reviewed;
        public bool Step06Reviewed;
        public bool Step07Reviewed;
        public bool UnityCompileEvidenceReviewed;
        public bool TestRunnerEvidenceReviewed;
        public bool PrivacyBoundaryReviewed;
        public bool ProductionMutationBoundaryReviewed;
        public int UnresolvedCriticalBlockers;
    }

    [Serializable]
    public sealed class LiveOpsOperatingBaselineEvidence
    {
        public int SchemaVersion=LiveOpsBaselineRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string BaselineId=string.Empty;
        public string ChainId=string.Empty;
        public string BaselineSha256=string.Empty;
        public int RunbookSectionCount;
        public bool ObservabilityRunbookReviewed;
        public bool IncidentRunbookReviewed;
        public bool HotfixRunbookReviewed;
        public bool LiveOpsRunbookReviewed;
        public bool PrivacySupportRunbookReviewed;
        public bool RollbackBoundaryReviewed;
        public bool ArchivePinned;
        public int UnresolvedCriticalActions;
    }

    [Serializable]
    public sealed class V120TransitionPlanEvidence
    {
        public int SchemaVersion=LiveOpsBaselineRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string TransitionId=string.Empty;
        public string BaselineId=string.Empty;
        public string TargetMilestone=LiveOpsBaselineRequirements.V120Milestone;
        public int LeadTimeHours;
        public bool TransitionScopeReviewed;
        public bool ChapterPipelineEntryReviewed;
        public bool ContentOwnershipReviewed;
        public bool AddressablesContentModelReviewed;
        public bool LocalizationPipelineReviewed;
        public bool QaStrategyReviewed;
        public bool AutomationStrategyReviewed;
        public bool TeamCapacityReviewed;
        public int CarryoverBlockerCount;
    }

    [Serializable]
    public sealed class EvidenceChainReviewResult
    {
        public bool StructurallyValid;
        public bool ChainReady;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class OperatingBaselineReviewResult
    {
        public bool StructurallyValid;
        public bool BaselineReady;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class TransitionReviewResult
    {
        public bool StructurallyValid;
        public bool TransitionReady;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsBaselineReviewResult
    {
        public bool ReadyForHumanReview;
        public bool MayCloseV119Manually;
        public bool MayBeginV120Preparation;
        public LiveOpsBaselineRecommendation Recommendation;
        public string[] ReviewBlockers=Array.Empty<string>();
        public string[] ActionBlockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsBaselineAggregateObservation
    {
        public int WindowIndex;
        public int OpenCriticalIncidents;
        public int OpenLiveOpsBlockers;
        public int OutstandingArchiveActions;
        public int TransitionCarryoverBlockers;
    }
}
