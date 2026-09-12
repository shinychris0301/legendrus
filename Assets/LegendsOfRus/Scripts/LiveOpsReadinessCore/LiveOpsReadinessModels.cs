using System;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class LiveOpsReadinessBlockerCodes
    {
        public const string ProfileMissing = "LOR_LO_PROFILE_MISSING";
        public const string ProfileNotReviewed = "LOR_LO_PROFILE_NOT_REVIEWED";
        public const string ProfileInvalid = "LOR_LO_PROFILE_INVALID";
        public const string CadenceMissing = "LOR_LO_CADENCE_MISSING";
        public const string CadenceNotReviewed = "LOR_LO_CADENCE_NOT_REVIEWED";
        public const string CadenceInvalid = "LOR_LO_CADENCE_INVALID";
        public const string EventIdInvalid = "LOR_LO_EVENT_ID_INVALID";
        public const string EventTimeInvalid = "LOR_LO_EVENT_TIME_INVALID";
        public const string EventDurationExceeded = "LOR_LO_EVENT_DURATION_EXCEEDED";
        public const string EventLeadTimeInsufficient = "LOR_LO_EVENT_LEAD_TIME_INSUFFICIENT";
        public const string ConcurrentEventsExceeded = "LOR_LO_CONCURRENT_EVENTS_EXCEEDED";
        public const string EventGapInsufficient = "LOR_LO_EVENT_GAP_INSUFFICIENT";
        public const string ContentHealthMissing = "LOR_LO_CONTENT_HEALTH_MISSING";
        public const string ContentHealthNotReviewed = "LOR_LO_CONTENT_HEALTH_NOT_REVIEWED";
        public const string ContentHealthInvalid = "LOR_LO_CONTENT_HEALTH_INVALID";
        public const string ContentSampleInsufficient = "LOR_LO_CONTENT_SAMPLE_INSUFFICIENT";
        public const string ContentCompletionLow = "LOR_LO_CONTENT_COMPLETION_LOW";
        public const string ContentEntryFailureHigh = "LOR_LO_CONTENT_ENTRY_FAILURE_HIGH";
        public const string ContentCrashHigh = "LOR_LO_CONTENT_CRASH_HIGH";
        public const string RewardClaimAnomaly = "LOR_LO_REWARD_CLAIM_ANOMALY";
        public const string ExperimentMissing = "LOR_EXP_MISSING";
        public const string ExperimentNotReviewed = "LOR_EXP_NOT_REVIEWED";
        public const string ExperimentInvalid = "LOR_EXP_INVALID";
        public const string ExperimentHypothesisMissing = "LOR_EXP_HYPOTHESIS_MISSING";
        public const string ExperimentMetricPlanMissing = "LOR_EXP_METRIC_PLAN_MISSING";
        public const string ExperimentVariantInvalid = "LOR_EXP_VARIANT_INVALID";
        public const string ExperimentWeightInvalid = "LOR_EXP_WEIGHT_INVALID";
        public const string ExperimentSampleInsufficient = "LOR_EXP_SAMPLE_INSUFFICIENT";
        public const string ExperimentDurationInvalid = "LOR_EXP_DURATION_INVALID";
        public const string ExperimentMutualExclusionMissing = "LOR_EXP_MUTUAL_EXCLUSION_MISSING";
        public const string ExperimentStoppingRuleMissing = "LOR_EXP_STOPPING_RULE_MISSING";
        public const string ExperimentPrivacyReviewMissing = "LOR_EXP_PRIVACY_REVIEW_MISSING";
        public const string ExperimentEconomyReviewMissing = "LOR_EXP_ECONOMY_REVIEW_MISSING";
        public const string ExperimentRewardReviewMissing = "LOR_EXP_REWARD_REVIEW_MISSING";
        public const string ExperimentDifficultyReviewMissing = "LOR_EXP_DIFFICULTY_REVIEW_MISSING";
        public const string ProductionAnalyticsDisabled = "LOR_EXP_PROD_ANALYTICS_DISABLED";
        public const string ProductionRemoteConfigDisabled = "LOR_EXP_PROD_REMOTE_CONFIG_DISABLED";
        public const string ExperimentLaunchNotAuthorized = "LOR_EXP_LAUNCH_NOT_AUTHORIZED";
        public const string LiveOpsPublishNotAuthorized = "LOR_LO_PUBLISH_NOT_AUTHORIZED";
        public const string ManualLiveOpsReviewNotAuthorized = "LOR_LO_MANUAL_REVIEW_NOT_AUTHORIZED";
        public const string ContentUnhealthy = "LOR_LO_CONTENT_UNHEALTHY";
        public const string CadenceUnsafe = "LOR_LO_CADENCE_UNSAFE";
    }

    public enum LiveOpsReadinessRecommendation
    {
        InsufficientEvidence = 0,
        ContinueObservation = 1,
        ReviseCadence = 2,
        InvestigateContentHealth = 3,
        ReviseExperiment = 4,
        HoldExperimentLaunch = 5,
        ReadyForManualLiveOpsReview = 6
    }

    [Serializable]
    public sealed class ReviewedLiveOpsGuardrailProfile
    {
        public int SchemaVersion = LiveOpsReadinessRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ProfileId = string.Empty;
        public int MinimumLeadMinutes = 1440;
        public int MaximumConcurrentEvents = 2;
        public int MinimumGapMinutes = 60;
        public int MaximumEventDurationMinutes = 10080;
        public int MinimumContentParticipants = 100;
        public int MinimumContentCompletionRateBasisPoints = 2000;
        public int MaximumContentEntryFailureRateBasisPoints = 500;
        public int MaximumContentCrashRateBasisPoints = 200;
        public int MinimumRewardClaimRateBasisPoints = 1000;
        public int MaximumRewardClaimRateBasisPoints = 9900;
        public int MinimumExperimentSamplePerVariant = 100;
        public int MinimumExperimentDurationHours = 24;
        public int MaximumExperimentDurationHours = 336;
    }

    [Serializable]
    public sealed class LiveOpsEventSlotEvidence
    {
        public string EventId = string.Empty;
        public int StartMinute;
        public int EndMinute;
        public bool IsExperiment;
        public string ContentVersion = string.Empty;
    }

    [Serializable]
    public sealed class LiveOpsCadencePlanEvidence
    {
        public int SchemaVersion = LiveOpsReadinessRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string PlanId = string.Empty;
        public int MinutesUntilWindowStart;
        public LiveOpsEventSlotEvidence[] Events = Array.Empty<LiveOpsEventSlotEvidence>();
    }

    [Serializable]
    public sealed class ContentHealthEvidence
    {
        public int SchemaVersion = LiveOpsReadinessRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string HealthId = string.Empty;
        public string ContentVersion = string.Empty;
        public int Participants;
        public int Entries;
        public int Completions;
        public int EntryFailures;
        public int ContentCrashes;
        public int RewardClaims;
        public int ConfigValidationFailures;
    }

    [Serializable]
    public sealed class ExperimentVariantEvidence
    {
        public string VariantId = string.Empty;
        public int WeightBasisPoints;
    }

    [Serializable]
    public sealed class ExperimentProposalEvidence
    {
        public int SchemaVersion = LiveOpsReadinessRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ExperimentId = string.Empty;
        public bool HypothesisReviewed;
        public bool PrimaryMetricReviewed;
        public int GuardrailMetricCount;
        public int ExpectedEligibleUsers;
        public int DurationHours;
        public ExperimentVariantEvidence[] Variants = Array.Empty<ExperimentVariantEvidence>();
        public bool MutuallyExclusiveExposure;
        public bool StoppingRuleReviewed;
        public bool PrivacyReviewed;
        public bool ImpactsEconomy;
        public bool EconomyImpactReviewed;
        public bool ImpactsRewards;
        public bool RewardImpactReviewed;
        public bool ImpactsDifficulty;
        public bool DifficultyImpactReviewed;
    }

    [Serializable]
    public sealed class CadenceReviewResult
    {
        public bool StructurallyValid;
        public bool CadenceSafe;
        public int MaximumObservedConcurrency;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ContentHealthReviewResult
    {
        public bool StructurallyValid;
        public bool SampleSufficient;
        public bool ContentHealthy;
        public int CompletionRateBasisPoints;
        public int EntryFailureRateBasisPoints;
        public int ContentCrashRateBasisPoints;
        public int RewardClaimRateBasisPoints;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ExperimentReadinessResult
    {
        public bool StructurallyValid;
        public bool DesignReady;
        public bool MeasurementReady;
        public bool LaunchReady;
        public int ExpectedSamplePerVariant;
        public string[] DesignBlockers = Array.Empty<string>();
        public string[] LaunchBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsReadinessResult
    {
        public bool ReadyForHumanReview;
        public bool CadenceSafe;
        public bool ContentHealthy;
        public bool ExperimentDesignReady;
        public bool ExperimentLaunchReady;
        public bool MayProceedWithManualLiveOpsPublishReview;
        public LiveOpsReadinessRecommendation Recommendation;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] PublishBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsAggregateObservation
    {
        public int WindowIndex;
        public int Participants;
        public int Entries;
        public int Completions;
        public int EntryFailures;
        public int ContentCrashes;
        public int RewardClaims;
    }
}
