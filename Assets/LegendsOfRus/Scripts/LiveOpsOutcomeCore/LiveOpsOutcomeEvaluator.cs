using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class LiveOpsOutcomeEvaluator
    {
        public static OutcomeReviewResult Evaluate(LiveOpsOutcomeEvidence e)
        {
            var b=new List<string>();Add(b,LiveOpsOutcomeValidation.ValidateOutcome(e));
            if(e==null)return new OutcomeReviewResult{Blockers=b.ToArray()};
            bool structural=!ContainsAny(b,LiveOpsOutcomeBlockerCodes.OutcomeMissing,LiveOpsOutcomeBlockerCodes.OutcomeNotReviewed,LiveOpsOutcomeBlockerCodes.OutcomeInvalid,LiveOpsOutcomeBlockerCodes.OutcomeContentHealthReviewMissing,LiveOpsOutcomeBlockerCodes.OutcomeEconomyImpactReviewMissing,LiveOpsOutcomeBlockerCodes.OutcomePrivacyImpactReviewMissing);
            bool sample=e.ActiveParticipants>=LiveOpsOutcomeRequirements.MinimumOutcomeParticipants;if(!sample)AddOne(b,LiveOpsOutcomeBlockerCodes.OutcomeSampleInsufficient);
            if(e.ContentCrashes>0)AddOne(b,LiveOpsOutcomeBlockerCodes.OutcomeCrashObserved);
            if(e.OpenBlockingIncidents>0)AddOne(b,LiveOpsOutcomeBlockerCodes.OutcomeBlockingIncidentOpen);
            if(e.RewardClaimsExpected!=e.RewardClaimsGranted)AddOne(b,LiveOpsOutcomeBlockerCodes.OutcomeRewardReconciliationMismatch);
            if(e.OpenSupportEscalations>0)AddOne(b,LiveOpsOutcomeBlockerCodes.OutcomeSupportEscalationOpen);
            bool healthy=structural&&sample&&e.ContentCrashes==0&&e.OpenBlockingIncidents==0&&e.RewardClaimsExpected==e.RewardClaimsGranted&&e.OpenSupportEscalations==0;
            return new OutcomeReviewResult{StructurallyValid=structural,SampleSufficient=sample,OutcomeHealthy=healthy,CompletionRateBasisPoints=LiveOpsOutcomeValidation.RateBasisPoints(e.ContentCompletions,e.ContentEntries),Blockers=b.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
        static bool ContainsAny(List<string>b,params string[]v){for(int i=0;i<v.Length;i++)if(b.Contains(v[i]))return true;return false;}
    }
}
