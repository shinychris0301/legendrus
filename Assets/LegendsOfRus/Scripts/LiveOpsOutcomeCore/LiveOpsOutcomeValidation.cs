using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class LiveOpsOutcomeValidation
    {
        public static bool HasPrefix(string value,string prefix){return !string.IsNullOrEmpty(value)&&value.StartsWith(prefix,StringComparison.Ordinal);}
        public static int RateBasisPoints(long part,long total){if(total<=0||part<=0)return 0;if(part>=total)return LiveOpsOutcomeRequirements.BasisPointsTotal;return (int)((part*LiveOpsOutcomeRequirements.BasisPointsTotal)/total);}
        public static string[] ValidateOutcome(LiveOpsOutcomeEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsOutcomeBlockerCodes.OutcomeMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsOutcomeBlockerCodes.OutcomeMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsOutcomeBlockerCodes.OutcomeNotReviewed);
            if(e.SchemaVersion!=LiveOpsOutcomeRequirements.SchemaVersion||!HasPrefix(e.OutcomeId,LiveOpsOutcomeRequirements.OutcomePrefix)||string.IsNullOrEmpty(e.ExecutionId)||e.WindowEndMinute<=e.WindowStartMinute||e.ActiveParticipants<0||e.ContentEntries<0||e.ContentCompletions<0||e.ContentCompletions>e.ContentEntries||e.ContentCrashes<0||e.OpenBlockingIncidents<0||e.RewardClaimsExpected<0||e.RewardClaimsGranted<0||e.OpenSupportEscalations<0)b.Add(LiveOpsOutcomeBlockerCodes.OutcomeInvalid);
            if(!e.ContentHealthReviewed)b.Add(LiveOpsOutcomeBlockerCodes.OutcomeContentHealthReviewMissing);
            if(!e.EconomyImpactReviewed)b.Add(LiveOpsOutcomeBlockerCodes.OutcomeEconomyImpactReviewMissing);
            if(!e.PrivacyImpactReviewed)b.Add(LiveOpsOutcomeBlockerCodes.OutcomePrivacyImpactReviewMissing);
            return Unique(b);
        }
        public static string[] ValidateDecision(ExperimentDecisionEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsOutcomeBlockerCodes.DecisionMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsOutcomeBlockerCodes.DecisionMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsOutcomeBlockerCodes.DecisionNotReviewed);
            if(e.SchemaVersion!=LiveOpsOutcomeRequirements.SchemaVersion||!HasPrefix(e.DecisionId,LiveOpsOutcomeRequirements.DecisionPrefix)||string.IsNullOrEmpty(e.ObservationId)||string.IsNullOrEmpty(e.ExperimentId)||e.TotalSamples<0||e.GuardrailBreaches<0||e.Decision==ExperimentDecisionKind.None)b.Add(LiveOpsOutcomeBlockerCodes.DecisionInvalid);
            if(!e.StoppingRuleReviewed)b.Add(LiveOpsOutcomeBlockerCodes.DecisionStoppingRuleReviewMissing);
            if(!e.DecisionBasisReviewed)b.Add(LiveOpsOutcomeBlockerCodes.DecisionBasisReviewMissing);
            return Unique(b);
        }
        public static string[] ValidateCloseout(RotationCloseoutEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsOutcomeBlockerCodes.CloseoutMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsOutcomeBlockerCodes.CloseoutMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsOutcomeBlockerCodes.CloseoutNotReviewed);
            if(e.SchemaVersion!=LiveOpsOutcomeRequirements.SchemaVersion||!HasPrefix(e.CloseoutId,LiveOpsOutcomeRequirements.CloseoutPrefix)||string.IsNullOrEmpty(e.RotationPlanId)||e.PlannedEntries<=0||e.ClosedEntries<0||e.ClosedEntries>e.PlannedEntries||e.ActiveEntriesRemaining<0||e.PendingRewardClaims<0||e.OrphanedContentReferences<0)b.Add(LiveOpsOutcomeBlockerCodes.CloseoutInvalid);
            return Unique(b);
        }
        static string[] Unique(List<string>b){var r=new List<string>();for(int i=0;i<b.Count;i++)if(!r.Contains(b[i]))r.Add(b[i]);return r.ToArray();}
    }
}
