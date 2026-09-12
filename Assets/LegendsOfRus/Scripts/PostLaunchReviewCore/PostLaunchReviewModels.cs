using System;

namespace LegendsOfRus.PostLaunchReview
{
    public static class PostLaunchReviewBlockerCodes
    {
        public const string ProfileMissing = "LOR_PL_PROFILE_MISSING";
        public const string ProfileNotReviewed = "LOR_PL_PROFILE_NOT_REVIEWED";
        public const string ProfileIdInvalid = "LOR_PL_PROFILE_ID_INVALID";
        public const string GuardrailInvalid = "LOR_PL_GUARDRAIL_INVALID";
        public const string TrendMissing = "LOR_PL_TREND_MISSING";
        public const string TrendNotReviewed = "LOR_PL_TREND_NOT_REVIEWED";
        public const string TrendIdInvalid = "LOR_PL_TREND_ID_INVALID";
        public const string TrendCountInvalid = "LOR_PL_TREND_COUNT_INVALID";
        public const string TrendSampleInsufficient = "LOR_PL_TREND_SAMPLE_INSUFFICIENT";
        public const string ActiveUsersDeclined = "LOR_PL_ACTIVE_USERS_DECLINED";
        public const string SessionsPerUserDeclined = "LOR_PL_SESSIONS_PER_USER_DECLINED";
        public const string CompletionRateDeclined = "LOR_PL_COMPLETION_RATE_DECLINED";
        public const string RetentionMissing = "LOR_PL_RETENTION_MISSING";
        public const string RetentionNotReviewed = "LOR_PL_RETENTION_NOT_REVIEWED";
        public const string RetentionIdInvalid = "LOR_PL_RETENTION_ID_INVALID";
        public const string RetentionCountInvalid = "LOR_PL_RETENTION_COUNT_INVALID";
        public const string RetentionOrderInvalid = "LOR_PL_RETENTION_ORDER_INVALID";
        public const string RetentionSampleInsufficient = "LOR_PL_RETENTION_SAMPLE_INSUFFICIENT";
        public const string D1RetentionDeclined = "LOR_PL_D1_RETENTION_DECLINED";
        public const string D7RetentionDeclined = "LOR_PL_D7_RETENTION_DECLINED";
        public const string D30RetentionDeclined = "LOR_PL_D30_RETENTION_DECLINED";
        public const string EconomyMissing = "LOR_PL_ECONOMY_MISSING";
        public const string EconomyNotReviewed = "LOR_PL_ECONOMY_NOT_REVIEWED";
        public const string EconomyIdInvalid = "LOR_PL_ECONOMY_ID_INVALID";
        public const string EconomyCountInvalid = "LOR_PL_ECONOMY_COUNT_INVALID";
        public const string EconomySampleInsufficient = "LOR_PL_ECONOMY_SAMPLE_INSUFFICIENT";
        public const string SoftCurrencyFlowMissing = "LOR_PL_SOFT_FLOW_MISSING";
        public const string SoftCurrencySinkSourceLow = "LOR_PL_SOFT_SINK_SOURCE_LOW";
        public const string SoftCurrencySinkSourceHigh = "LOR_PL_SOFT_SINK_SOURCE_HIGH";
        public const string HardCurrencyNetGrantHigh = "LOR_PL_HARD_NET_GRANT_HIGH";
        public const string BoosterNetGrantHigh = "LOR_PL_BOOSTER_NET_GRANT_HIGH";
        public const string RewardGrantDeltaHigh = "LOR_PL_REWARD_GRANT_DELTA_HIGH";
        public const string EconomyGuardrailBreached = "LOR_PL_ECONOMY_GUARDRAIL_BREACHED";
        public const string ManualEconomyReviewNotAuthorized = "LOR_PL_MANUAL_ECONOMY_REVIEW_NOT_AUTHORIZED";
    }

    public enum PostLaunchReviewRecommendation
    {
        InsufficientEvidence = 0,
        ContinueObservation = 1,
        InvestigateTrendOrRetention = 2,
        InvestigateEconomy = 3,
        InvestigateMultiple = 4,
        HoldEconomyChangeReview = 5
    }

    [Serializable]
    public sealed class ReviewedGuardrailProfile
    {
        public int SchemaVersion = PostLaunchReviewRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ProfileId = string.Empty;
        public int MinimumActiveUsers = 100;
        public int MinimumCohortSize = 100;
        public int MaxActiveUserRelativeDeclineBasisPoints = 2500;
        public int MaxSessionsPerUserRelativeDeclineBasisPoints = 2000;
        public int MaxCompletionRateDropBasisPoints = 1500;
        public int MaxD1RetentionDropBasisPoints = 1000;
        public int MaxD7RetentionDropBasisPoints = 750;
        public int MaxD30RetentionDropBasisPoints = 500;
        public int MinSoftCurrencySinkSourceRatioBasisPoints = 7000;
        public int MaxSoftCurrencySinkSourceRatioBasisPoints = 13000;
        public int MaxHardCurrencyNetGrantPerActiveUser = 20;
        public int MaxBoosterNetGrantPerActiveUser = 3;
        public int MaxRewardGrantRelativeIncreaseBasisPoints = 2500;
    }

    [Serializable]
    public sealed class TrendWindowEvidence
    {
        public int SchemaVersion = PostLaunchReviewRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string WindowId = string.Empty;
        public int WindowDays;
        public int BaselineActiveUsers;
        public int CurrentActiveUsers;
        public int BaselineSessions;
        public int CurrentSessions;
        public int BaselineLevelStarts;
        public int BaselineLevelCompletes;
        public int CurrentLevelStarts;
        public int CurrentLevelCompletes;
    }

    [Serializable]
    public sealed class RetentionCohortEvidence
    {
        public int SchemaVersion = PostLaunchReviewRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string CohortId = string.Empty;
        public int CohortSize;
        public int D1Returned;
        public int D7Returned;
        public int D30Returned;
        public int BaselineD1BasisPoints;
        public int BaselineD7BasisPoints;
        public int BaselineD30BasisPoints;
    }

    [Serializable]
    public sealed class EconomyFlowEvidence
    {
        public int SchemaVersion = PostLaunchReviewRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string WindowId = string.Empty;
        public int ActiveUsers;
        public long SoftCurrencyEarned;
        public long SoftCurrencySpent;
        public long HardCurrencyGranted;
        public long HardCurrencySpent;
        public long BoostersGranted;
        public long BoostersConsumed;
        public long BaselineRewardUnitsGranted;
        public long CurrentRewardUnitsGranted;
    }

    [Serializable]
    public sealed class TrendRetentionReviewResult
    {
        public bool StructurallyValid;
        public bool SampleSufficient;
        public bool TrendHealthy;
        public bool RetentionHealthy;
        public int ActiveUserRelativeDeclineBasisPoints;
        public int SessionsPerUserRelativeDeclineBasisPoints;
        public int CompletionRateDropBasisPoints;
        public int D1BasisPoints;
        public int D7BasisPoints;
        public int D30BasisPoints;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class EconomyGuardrailResult
    {
        public bool StructurallyValid;
        public bool SampleSufficient;
        public bool EconomyHealthy;
        public int SoftCurrencySinkSourceRatioBasisPoints;
        public int HardCurrencyNetGrantPerActiveUser;
        public int BoosterNetGrantPerActiveUser;
        public int RewardGrantRelativeIncreaseBasisPoints;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class PostLaunchReviewResult
    {
        public bool ReadyForHumanReview;
        public bool TrendRetentionHealthy;
        public bool EconomyHealthy;
        public bool MayProceedWithManualEconomyChangeReview;
        public PostLaunchReviewRecommendation Recommendation;
        public string[] ReviewBlockers = Array.Empty<string>();
        public string[] EconomyChangeBlockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class AggregateObservation
    {
        public int DayIndex;
        public int ActiveUsers;
        public int Sessions;
        public int LevelStarts;
        public int LevelCompletes;
        public long SoftCurrencyEarned;
        public long SoftCurrencySpent;
    }
}
