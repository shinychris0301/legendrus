using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class ExperimentDecisionEvaluator
    {
        public static ExperimentDecisionReviewResult Evaluate(ExperimentDecisionEvidence e)
        {
            var b=new List<string>();Add(b,LiveOpsOutcomeValidation.ValidateDecision(e));
            if(e==null)return new ExperimentDecisionReviewResult{Recommendation=ExperimentDecisionRecommendation.InsufficientEvidence,Blockers=b.ToArray()};
            bool structural=!ContainsAny(b,LiveOpsOutcomeBlockerCodes.DecisionMissing,LiveOpsOutcomeBlockerCodes.DecisionNotReviewed,LiveOpsOutcomeBlockerCodes.DecisionInvalid,LiveOpsOutcomeBlockerCodes.DecisionStoppingRuleReviewMissing,LiveOpsOutcomeBlockerCodes.DecisionBasisReviewMissing);
            bool sample=e.TotalSamples>=LiveOpsOutcomeRequirements.MinimumExperimentSamplesTotal;if(!sample)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionSampleInsufficient);
            if(!e.DataQualityPassed)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionDataQualityFailure);
            if(!e.SampleRatioHealthy)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionSampleRatioUnhealthy);
            if(!e.ExperimentWindowComplete)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionWindowIncomplete);
            bool guardrailDecision=e.Decision==ExperimentDecisionKind.StopForGuardrail;
            if(e.GuardrailBreaches>0&&!guardrailDecision)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict);
            if(e.GuardrailBreaches==0&&guardrailDecision)AddOne(b,LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict);
            bool finalized=structural&&sample&&e.DataQualityPassed&&e.SampleRatioHealthy&&e.ExperimentWindowComplete&&!b.Contains(LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict);
            bool archive=finalized;
            ExperimentDecisionRecommendation r;
            if(!structural||!sample)r=ExperimentDecisionRecommendation.InsufficientEvidence;
            else if(!e.DataQualityPassed||!e.SampleRatioHealthy||b.Contains(LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict))r=ExperimentDecisionRecommendation.RevisitDecision;
            else if(!e.ExperimentWindowComplete)r=ExperimentDecisionRecommendation.ContinueObservation;
            else if(e.Decision==ExperimentDecisionKind.KeepControl)r=ExperimentDecisionRecommendation.ArchiveControl;
            else if(e.Decision==ExperimentDecisionKind.AdoptTreatment)r=ExperimentDecisionRecommendation.ArchiveTreatment;
            else if(e.Decision==ExperimentDecisionKind.StopForGuardrail)r=ExperimentDecisionRecommendation.ArchiveStoppedForGuardrail;
            else r=ExperimentDecisionRecommendation.ArchiveInconclusive;
            return new ExperimentDecisionReviewResult{StructurallyValid=structural,DecisionFinalized=finalized,ReadyForArchive=archive,Recommendation=r,Blockers=b.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
        static bool ContainsAny(List<string>b,params string[]v){for(int i=0;i<v.Length;i++)if(b.Contains(v[i]))return true;return false;}
    }
}
