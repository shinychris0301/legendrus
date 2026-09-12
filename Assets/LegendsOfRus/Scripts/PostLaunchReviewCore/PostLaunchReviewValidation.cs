using System;
using System.Collections.Generic;

namespace LegendsOfRus.PostLaunchReview
{
    public static class PostLaunchReviewValidation
    {
        public static string[] ValidateProfile(ReviewedGuardrailProfile value)
        {
            var b = new List<string>();
            if (value == null || !value.EvidenceRetained) { b.Add(PostLaunchReviewBlockerCodes.ProfileMissing); return b.ToArray(); }
            if (!value.EvidenceReviewed) b.Add(PostLaunchReviewBlockerCodes.ProfileNotReviewed);
            if (value.SchemaVersion != PostLaunchReviewRequirements.SchemaVersion || !IsStableId(value.ProfileId, PostLaunchReviewRequirements.GuardrailProfilePrefix)) b.Add(PostLaunchReviewBlockerCodes.ProfileIdInvalid);
            if (value.MinimumActiveUsers <= 0 || value.MinimumActiveUsers > PostLaunchReviewRequirements.MaxAggregateCount ||
                value.MinimumCohortSize <= 0 || value.MinimumCohortSize > PostLaunchReviewRequirements.MaxAggregateCount ||
                !InBps(value.MaxActiveUserRelativeDeclineBasisPoints) || !InBps(value.MaxSessionsPerUserRelativeDeclineBasisPoints) ||
                !InBps(value.MaxCompletionRateDropBasisPoints) || !InBps(value.MaxD1RetentionDropBasisPoints) ||
                !InBps(value.MaxD7RetentionDropBasisPoints) || !InBps(value.MaxD30RetentionDropBasisPoints) ||
                value.MinSoftCurrencySinkSourceRatioBasisPoints < 0 || value.MaxSoftCurrencySinkSourceRatioBasisPoints < value.MinSoftCurrencySinkSourceRatioBasisPoints ||
                value.MaxSoftCurrencySinkSourceRatioBasisPoints > PostLaunchReviewRequirements.BasisPointsScale * 10 ||
                value.MaxHardCurrencyNetGrantPerActiveUser < 0 || value.MaxHardCurrencyNetGrantPerActiveUser > PostLaunchReviewRequirements.MaxPerActiveUserGuardrail ||
                value.MaxBoosterNetGrantPerActiveUser < 0 || value.MaxBoosterNetGrantPerActiveUser > PostLaunchReviewRequirements.MaxPerActiveUserGuardrail ||
                !InBps(value.MaxRewardGrantRelativeIncreaseBasisPoints)) b.Add(PostLaunchReviewBlockerCodes.GuardrailInvalid);
            return b.ToArray();
        }

        public static string[] ValidateTrend(TrendWindowEvidence value)
        {
            var b = new List<string>();
            if (value == null || !value.EvidenceRetained) { b.Add(PostLaunchReviewBlockerCodes.TrendMissing); return b.ToArray(); }
            if (!value.EvidenceReviewed) b.Add(PostLaunchReviewBlockerCodes.TrendNotReviewed);
            if (value.SchemaVersion != PostLaunchReviewRequirements.SchemaVersion || !IsStableId(value.WindowId, PostLaunchReviewRequirements.TrendWindowPrefix) || value.WindowDays <= 0 || value.WindowDays > PostLaunchReviewRequirements.MaxWindowDays) b.Add(PostLaunchReviewBlockerCodes.TrendIdInvalid);
            int[] counts = { value.BaselineActiveUsers, value.CurrentActiveUsers, value.BaselineSessions, value.CurrentSessions, value.BaselineLevelStarts, value.BaselineLevelCompletes, value.CurrentLevelStarts, value.CurrentLevelCompletes };
            for (int i=0;i<counts.Length;i++) if (counts[i] < 0 || counts[i] > PostLaunchReviewRequirements.MaxAggregateCount) { b.Add(PostLaunchReviewBlockerCodes.TrendCountInvalid); break; }
            if (value.BaselineLevelCompletes > value.BaselineLevelStarts || value.CurrentLevelCompletes > value.CurrentLevelStarts) AddUnique(b, PostLaunchReviewBlockerCodes.TrendCountInvalid);
            return b.ToArray();
        }

        public static string[] ValidateRetention(RetentionCohortEvidence value)
        {
            var b = new List<string>();
            if (value == null || !value.EvidenceRetained) { b.Add(PostLaunchReviewBlockerCodes.RetentionMissing); return b.ToArray(); }
            if (!value.EvidenceReviewed) b.Add(PostLaunchReviewBlockerCodes.RetentionNotReviewed);
            if (value.SchemaVersion != PostLaunchReviewRequirements.SchemaVersion || !IsStableId(value.CohortId, PostLaunchReviewRequirements.RetentionCohortPrefix)) b.Add(PostLaunchReviewBlockerCodes.RetentionIdInvalid);
            if (value.CohortSize < 0 || value.CohortSize > PostLaunchReviewRequirements.MaxAggregateCount || value.D1Returned < 0 || value.D7Returned < 0 || value.D30Returned < 0 || value.D1Returned > value.CohortSize || value.D7Returned > value.CohortSize || value.D30Returned > value.CohortSize || !InBps(value.BaselineD1BasisPoints) || !InBps(value.BaselineD7BasisPoints) || !InBps(value.BaselineD30BasisPoints)) b.Add(PostLaunchReviewBlockerCodes.RetentionCountInvalid);
            if (value.D7Returned > value.D1Returned || value.D30Returned > value.D7Returned || value.BaselineD7BasisPoints > value.BaselineD1BasisPoints || value.BaselineD30BasisPoints > value.BaselineD7BasisPoints) b.Add(PostLaunchReviewBlockerCodes.RetentionOrderInvalid);
            return b.ToArray();
        }

        public static string[] ValidateEconomy(EconomyFlowEvidence value)
        {
            var b = new List<string>();
            if (value == null || !value.EvidenceRetained) { b.Add(PostLaunchReviewBlockerCodes.EconomyMissing); return b.ToArray(); }
            if (!value.EvidenceReviewed) b.Add(PostLaunchReviewBlockerCodes.EconomyNotReviewed);
            if (value.SchemaVersion != PostLaunchReviewRequirements.SchemaVersion || !IsStableId(value.WindowId, PostLaunchReviewRequirements.EconomyFlowPrefix)) b.Add(PostLaunchReviewBlockerCodes.EconomyIdInvalid);
            if (value.ActiveUsers < 0 || value.ActiveUsers > PostLaunchReviewRequirements.MaxAggregateCount || !ValidAggregate(value.SoftCurrencyEarned) || !ValidAggregate(value.SoftCurrencySpent) || !ValidAggregate(value.HardCurrencyGranted) || !ValidAggregate(value.HardCurrencySpent) || !ValidAggregate(value.BoostersGranted) || !ValidAggregate(value.BoostersConsumed) || !ValidAggregate(value.BaselineRewardUnitsGranted) || !ValidAggregate(value.CurrentRewardUnitsGranted)) b.Add(PostLaunchReviewBlockerCodes.EconomyCountInvalid);
            return b.ToArray();
        }

        public static bool IsStableId(string value, string prefix)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(prefix) || value.Length > PostLaunchReviewRequirements.StableIdMaxLength || !value.StartsWith(prefix, StringComparison.Ordinal)) return false;
            for (int i=0;i<value.Length;i++)
            {
                char c=value[i];
                if (!((c>='A'&&c<='Z')||(c>='0'&&c<='9')||c=='-'||c=='_'||c=='.')) return false;
            }
            return true;
        }

        public static int RateBasisPoints(long numerator, long denominator)
        {
            if (numerator <= 0 || denominator <= 0) return 0;
            long scaled = numerator * PostLaunchReviewRequirements.BasisPointsScale / denominator;
            if (scaled > int.MaxValue) return int.MaxValue;
            return (int)scaled;
        }

        public static int PositiveRelativeDeclineBasisPoints(long baseline, long current)
        {
            if (baseline <= 0 || current >= baseline) return 0;
            return RateBasisPoints(baseline-current, baseline);
        }

        public static int PositiveDrop(int baselineBasisPoints, int currentBasisPoints)
        {
            return currentBasisPoints >= baselineBasisPoints ? 0 : baselineBasisPoints-currentBasisPoints;
        }

        static bool InBps(int v) { return v >= 0 && v <= PostLaunchReviewRequirements.BasisPointsScale; }
        static bool ValidAggregate(long v) { return v >= 0 && v <= PostLaunchReviewRequirements.MaxCurrencyAggregate; }
        static void AddUnique(List<string> b, string code) { if (!b.Contains(code)) b.Add(code); }
    }
}
