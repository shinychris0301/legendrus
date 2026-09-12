using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsOutcome
{
    public static class LiveOpsOutcomeReviewEvaluator
    {
        public static LiveOpsOutcomeReviewResult Evaluate(LiveOpsOutcomeEvidence outcome,ExperimentDecisionEvidence decision,RotationCloseoutEvidence closeout,bool productionAnalyticsProviderEnabled,bool productionRemoteConfigEnabled,bool outcomeReviewAuthorized,bool experimentDecisionAuthorized,bool rotationCloseoutAuthorized,bool manualCloseoutReviewAuthorized)
        {
            OutcomeReviewResult o=LiveOpsOutcomeEvaluator.Evaluate(outcome);
            ExperimentDecisionReviewResult d=ExperimentDecisionEvaluator.Evaluate(decision);
            RotationCloseoutReviewResult c=RotationCloseoutEvaluator.Evaluate(closeout);
            var review=new List<string>();Add(review,o.Blockers);Add(review,d.Blockers);Add(review,c.Blockers);
            var action=new List<string>();
            if(!productionAnalyticsProviderEnabled)AddOne(action,LiveOpsOutcomeBlockerCodes.ProductionAnalyticsDisabled);
            if(!productionRemoteConfigEnabled)AddOne(action,LiveOpsOutcomeBlockerCodes.ProductionRemoteConfigDisabled);
            if(!outcomeReviewAuthorized)AddOne(action,LiveOpsOutcomeBlockerCodes.OutcomeReviewNotAuthorized);
            if(!experimentDecisionAuthorized)AddOne(action,LiveOpsOutcomeBlockerCodes.ExperimentDecisionNotAuthorized);
            if(!rotationCloseoutAuthorized)AddOne(action,LiveOpsOutcomeBlockerCodes.RotationCloseoutNotAuthorized);
            if(!manualCloseoutReviewAuthorized)AddOne(action,LiveOpsOutcomeBlockerCodes.ManualCloseoutReviewNotAuthorized);
            bool ready=o.StructurallyValid&&d.StructurallyValid&&c.StructurallyValid;
            bool may=ready&&o.OutcomeHealthy&&d.ReadyForArchive&&c.CloseoutComplete&&productionAnalyticsProviderEnabled&&productionRemoteConfigEnabled&&outcomeReviewAuthorized&&experimentDecisionAuthorized&&rotationCloseoutAuthorized&&manualCloseoutReviewAuthorized;
            LiveOpsOutcomeRecommendation r;
            if(!ready)r=LiveOpsOutcomeRecommendation.InsufficientEvidence;
            else if(!o.OutcomeHealthy)r=LiveOpsOutcomeRecommendation.ResolveOutcomeBlockers;
            else if(!d.ReadyForArchive)r=d.Recommendation==ExperimentDecisionRecommendation.ContinueObservation?LiveOpsOutcomeRecommendation.ContinueObservation:LiveOpsOutcomeRecommendation.RevisitExperimentDecision;
            else if(!c.CloseoutComplete)r=LiveOpsOutcomeRecommendation.CompleteRotationCloseout;
            else if(may)r=LiveOpsOutcomeRecommendation.ReadyForManualCloseoutReview;
            else r=LiveOpsOutcomeRecommendation.ContinueObservation;
            return new LiveOpsOutcomeReviewResult{ReadyForHumanReview=ready,OutcomeHealthy=o.OutcomeHealthy,ExperimentDecisionReady=d.ReadyForArchive,RotationCloseoutComplete=c.CloseoutComplete,MayProceedWithManualCloseoutReview=may,Recommendation=r,ReviewBlockers=review.ToArray(),ActionBlockers=action.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
