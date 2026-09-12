using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class LiveOpsReadinessValidation
    {
        public static bool IsStableId(string value, string prefix)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(prefix) || !value.StartsWith(prefix, StringComparison.Ordinal)) return false;
            if (value.Length <= prefix.Length) return false;
            for (int i = prefix.Length; i < value.Length; i++)
            {
                char c = value[i];
                if (!(c >= 'A' && c <= 'Z') && !(c >= '0' && c <= '9') && c != '-') return false;
            }
            return true;
        }

        public static int RateBasisPoints(long numerator, long denominator)
        {
            if (denominator <= 0 || numerator <= 0) return 0;
            long value = numerator * LiveOpsReadinessRequirements.WeightBasisPointsTotal / denominator;
            if (value < 0) return 0;
            if (value > int.MaxValue) return int.MaxValue;
            return (int)value;
        }

        public static string[] ValidateProfile(ReviewedLiveOpsGuardrailProfile p)
        {
            var b = new List<string>();
            if (p == null) { b.Add(LiveOpsReadinessBlockerCodes.ProfileMissing); return b.ToArray(); }
            if (!p.EvidenceRetained) b.Add(LiveOpsReadinessBlockerCodes.ProfileMissing);
            if (!p.EvidenceReviewed) b.Add(LiveOpsReadinessBlockerCodes.ProfileNotReviewed);
            if (!IsStableId(p.ProfileId, LiveOpsReadinessRequirements.GuardrailProfilePrefix)) b.Add(LiveOpsReadinessBlockerCodes.ProfileInvalid);
            if (p.MinimumLeadMinutes < 0 || p.MaximumConcurrentEvents < 1 || p.MinimumGapMinutes < 0 || p.MaximumEventDurationMinutes < 1 || p.MinimumContentParticipants < 1 || p.MinimumContentCompletionRateBasisPoints < 0 || p.MinimumContentCompletionRateBasisPoints > 10000 || p.MaximumContentEntryFailureRateBasisPoints < 0 || p.MaximumContentEntryFailureRateBasisPoints > 10000 || p.MaximumContentCrashRateBasisPoints < 0 || p.MaximumContentCrashRateBasisPoints > 10000 || p.MinimumRewardClaimRateBasisPoints < 0 || p.MinimumRewardClaimRateBasisPoints > 10000 || p.MaximumRewardClaimRateBasisPoints < p.MinimumRewardClaimRateBasisPoints || p.MaximumRewardClaimRateBasisPoints > 10000 || p.MinimumExperimentSamplePerVariant < 1 || p.MinimumExperimentDurationHours < 1 || p.MaximumExperimentDurationHours < p.MinimumExperimentDurationHours) b.Add(LiveOpsReadinessBlockerCodes.ProfileInvalid);
            return b.ToArray();
        }

        public static string[] ValidateCadence(LiveOpsCadencePlanEvidence p)
        {
            var b = new List<string>();
            if (p == null) { b.Add(LiveOpsReadinessBlockerCodes.CadenceMissing); return b.ToArray(); }
            if (!p.EvidenceRetained) b.Add(LiveOpsReadinessBlockerCodes.CadenceMissing);
            if (!p.EvidenceReviewed) b.Add(LiveOpsReadinessBlockerCodes.CadenceNotReviewed);
            if (!IsStableId(p.PlanId, LiveOpsReadinessRequirements.CadencePlanPrefix) || p.MinutesUntilWindowStart < 0 || p.Events == null) b.Add(LiveOpsReadinessBlockerCodes.CadenceInvalid);
            if (p.Events != null)
            {
                var ids = new HashSet<string>(StringComparer.Ordinal);
                for (int i = 0; i < p.Events.Length; i++)
                {
                    LiveOpsEventSlotEvidence e = p.Events[i];
                    if (e == null || !IsStableId(e.EventId, LiveOpsReadinessRequirements.EventPrefix) || !ids.Add(e.EventId)) b.Add(LiveOpsReadinessBlockerCodes.EventIdInvalid);
                    else if (e.EndMinute <= e.StartMinute || e.StartMinute < 0) b.Add(LiveOpsReadinessBlockerCodes.EventTimeInvalid);
                }
            }
            return Unique(b);
        }

        public static string[] ValidateContentHealth(ContentHealthEvidence e)
        {
            var b = new List<string>();
            if (e == null) { b.Add(LiveOpsReadinessBlockerCodes.ContentHealthMissing); return b.ToArray(); }
            if (!e.EvidenceRetained) b.Add(LiveOpsReadinessBlockerCodes.ContentHealthMissing);
            if (!e.EvidenceReviewed) b.Add(LiveOpsReadinessBlockerCodes.ContentHealthNotReviewed);
            if (!IsStableId(e.HealthId, LiveOpsReadinessRequirements.ContentHealthPrefix) || string.IsNullOrEmpty(e.ContentVersion) || e.Participants < 0 || e.Entries < 0 || e.Completions < 0 || e.EntryFailures < 0 || e.ContentCrashes < 0 || e.RewardClaims < 0 || e.ConfigValidationFailures < 0 || e.Completions > e.Entries || e.EntryFailures > e.Entries || e.RewardClaims > e.Completions) b.Add(LiveOpsReadinessBlockerCodes.ContentHealthInvalid);
            return Unique(b);
        }

        public static string[] ValidateExperiment(ExperimentProposalEvidence e)
        {
            var b = new List<string>();
            if (e == null) { b.Add(LiveOpsReadinessBlockerCodes.ExperimentMissing); return b.ToArray(); }
            if (!e.EvidenceRetained) b.Add(LiveOpsReadinessBlockerCodes.ExperimentMissing);
            if (!e.EvidenceReviewed) b.Add(LiveOpsReadinessBlockerCodes.ExperimentNotReviewed);
            if (!IsStableId(e.ExperimentId, LiveOpsReadinessRequirements.ExperimentPrefix) || e.ExpectedEligibleUsers < 0 || e.DurationHours < 0 || e.Variants == null || e.Variants.Length < 2 || e.Variants.Length > LiveOpsReadinessRequirements.MaxExperimentVariants) b.Add(LiveOpsReadinessBlockerCodes.ExperimentInvalid);
            if (e.Variants != null)
            {
                var ids = new HashSet<string>(StringComparer.Ordinal); int total = 0;
                for (int i = 0; i < e.Variants.Length; i++)
                {
                    ExperimentVariantEvidence v = e.Variants[i];
                    if (v == null || string.IsNullOrEmpty(v.VariantId) || !ids.Add(v.VariantId) || v.WeightBasisPoints <= 0) b.Add(LiveOpsReadinessBlockerCodes.ExperimentVariantInvalid);
                    else total += v.WeightBasisPoints;
                }
                if (total != LiveOpsReadinessRequirements.WeightBasisPointsTotal) b.Add(LiveOpsReadinessBlockerCodes.ExperimentWeightInvalid);
            }
            return Unique(b);
        }

        static string[] Unique(List<string> values)
        {
            var outList = new List<string>();
            for (int i = 0; i < values.Count; i++) if (!outList.Contains(values[i])) outList.Add(values[i]);
            return outList.ToArray();
        }
    }
}
