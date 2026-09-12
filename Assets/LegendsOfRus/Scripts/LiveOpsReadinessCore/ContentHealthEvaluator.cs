using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class ContentHealthEvaluator
    {
        public static ContentHealthReviewResult Evaluate(ReviewedLiveOpsGuardrailProfile profile, ContentHealthEvidence evidence)
        {
            var blockers = new List<string>();
            Add(blockers, LiveOpsReadinessValidation.ValidateProfile(profile));
            Add(blockers, LiveOpsReadinessValidation.ValidateContentHealth(evidence));
            if (profile == null || evidence == null || blockers.Count > 0) return Result(false, false, 0, 0, 0, 0, blockers);
            bool sample = evidence.Participants >= profile.MinimumContentParticipants;
            if (!sample) AddOne(blockers, LiveOpsReadinessBlockerCodes.ContentSampleInsufficient);
            int completion = LiveOpsReadinessValidation.RateBasisPoints(evidence.Completions, evidence.Entries);
            int entryFailure = LiveOpsReadinessValidation.RateBasisPoints(evidence.EntryFailures, evidence.Entries);
            int crash = LiveOpsReadinessValidation.RateBasisPoints(evidence.ContentCrashes, evidence.Participants);
            int claim = LiveOpsReadinessValidation.RateBasisPoints(evidence.RewardClaims, evidence.Completions);
            if (sample && completion < profile.MinimumContentCompletionRateBasisPoints) AddOne(blockers, LiveOpsReadinessBlockerCodes.ContentCompletionLow);
            if (sample && entryFailure > profile.MaximumContentEntryFailureRateBasisPoints) AddOne(blockers, LiveOpsReadinessBlockerCodes.ContentEntryFailureHigh);
            if (sample && crash > profile.MaximumContentCrashRateBasisPoints) AddOne(blockers, LiveOpsReadinessBlockerCodes.ContentCrashHigh);
            if (sample && (claim < profile.MinimumRewardClaimRateBasisPoints || claim > profile.MaximumRewardClaimRateBasisPoints || evidence.ConfigValidationFailures > 0)) AddOne(blockers, LiveOpsReadinessBlockerCodes.RewardClaimAnomaly);
            bool healthy = sample && blockers.Count == 0;
            return Result(true, sample, completion, entryFailure, crash, claim, blockers, healthy);
        }
        static ContentHealthReviewResult Result(bool structural, bool sample, int completion, int entryFailure, int crash, int claim, List<string> blockers, bool healthy=false) { return new ContentHealthReviewResult { StructurallyValid=structural, SampleSufficient=sample, ContentHealthy=healthy, CompletionRateBasisPoints=completion, EntryFailureRateBasisPoints=entryFailure, ContentCrashRateBasisPoints=crash, RewardClaimRateBasisPoints=claim, Blockers=blockers.ToArray() }; }
        static void Add(List<string> d,string[] v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
