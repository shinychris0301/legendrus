using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class RotationCloseoutEvaluator
    {
        public static RotationCloseoutReviewResult Evaluate(RotationCloseoutEvidence e)
        {
            var b=new List<string>();Add(b,LiveOpsOutcomeValidation.ValidateCloseout(e));
            if(e==null)return new RotationCloseoutReviewResult{Blockers=b.ToArray()};
            bool structural=!ContainsAny(b,LiveOpsOutcomeBlockerCodes.CloseoutMissing,LiveOpsOutcomeBlockerCodes.CloseoutNotReviewed,LiveOpsOutcomeBlockerCodes.CloseoutInvalid);
            if(e.ClosedEntries!=e.PlannedEntries||e.ActiveEntriesRemaining>0)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutEntriesRemaining);
            if(e.PendingRewardClaims>0)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutRewardClaimsPending);
            if(e.OrphanedContentReferences>0)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutOrphanedContent);
            if(!e.CooldownsSatisfied)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutCooldownIncomplete);
            if(!e.ContentVersionsArchived)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutVersionsNotArchived);
            if(!e.SupportHandoffComplete)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutSupportHandoffMissing);
            if(!e.CloseoutWindowComplete)AddOne(b,LiveOpsOutcomeBlockerCodes.CloseoutWindowIncomplete);
            bool complete=structural&&b.Count==0;
            return new RotationCloseoutReviewResult{StructurallyValid=structural,CloseoutComplete=complete,Blockers=b.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
        static bool ContainsAny(List<string>b,params string[]v){for(int i=0;i<v.Length;i++)if(b.Contains(v[i]))return true;return false;}
    }
}
