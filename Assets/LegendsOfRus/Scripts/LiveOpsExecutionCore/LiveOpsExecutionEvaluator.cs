using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsExecution
{
    public static class LiveOpsExecutionEvaluator
    {
        public static LiveOpsExecutionReviewResult Evaluate(LiveOpsExecutionEvidence execution,ContentRotationEvidence rotation,ExperimentObservationEvidence observation,bool productionAnalyticsProviderEnabled,bool productionRemoteConfigEnabled,bool liveOpsExecutionAuthorized,bool contentRotationAuthorized,bool experimentObservationAuthorized,bool experimentTrafficChangeAuthorized,bool manualExecutionReviewAuthorized)
        {
            var review=new List<string>();Add(review,LiveOpsExecutionValidation.ValidateExecution(execution));
            RotationReviewResult rr=ContentRotationEvaluator.Evaluate(rotation);Add(review,rr.Blockers);
            ExperimentObservationResult or=ExperimentObservationEvaluator.Evaluate(observation);Add(review,or.Blockers);
            bool ready=LiveOpsExecutionValidation.ValidateExecution(execution).Length==0&&rr.StructurallyValid&&or.StructurallyValid&&or.SampleSufficient;
            bool healthy=observation!=null&&or.SampleRatioHealthy&&or.DataQualityHealthy&&or.GuardrailsHealthy&&!observation.StoppingRuleTriggered;
            var action=new List<string>();
            if(!productionAnalyticsProviderEnabled)AddOne(action,LiveOpsExecutionBlockerCodes.ProductionAnalyticsDisabled);
            if(!productionRemoteConfigEnabled)AddOne(action,LiveOpsExecutionBlockerCodes.ProductionRemoteConfigDisabled);
            if(!liveOpsExecutionAuthorized)AddOne(action,LiveOpsExecutionBlockerCodes.ExecutionNotAuthorized);
            if(!contentRotationAuthorized)AddOne(action,LiveOpsExecutionBlockerCodes.RotationNotAuthorized);
            if(!experimentObservationAuthorized)AddOne(action,LiveOpsExecutionBlockerCodes.ObservationNotAuthorized);
            if(!experimentTrafficChangeAuthorized)AddOne(action,LiveOpsExecutionBlockerCodes.TrafficChangeNotAuthorized);
            if(!manualExecutionReviewAuthorized)AddOne(action,LiveOpsExecutionBlockerCodes.ManualExecutionReviewNotAuthorized);
            LiveOpsExecutionRecommendation rec;
            if(!ready)rec=LiveOpsExecutionRecommendation.InsufficientEvidence;
            else if(!rr.RotationSafe)rec=LiveOpsExecutionRecommendation.ReviseRotation;
            else if(or.Recommendation==ExperimentObservationRecommendation.StopForGuardrailReview||or.Recommendation==ExperimentObservationRecommendation.StopForStoppingRuleReview)rec=LiveOpsExecutionRecommendation.StopExperimentReview;
            else if(or.Recommendation==ExperimentObservationRecommendation.PauseForDataQualityReview||or.Recommendation==ExperimentObservationRecommendation.PauseForSampleRatioReview)rec=LiveOpsExecutionRecommendation.PauseExperimentReview;
            else if(healthy&&manualExecutionReviewAuthorized)rec=LiveOpsExecutionRecommendation.ReadyForManualExecutionReview;
            else rec=LiveOpsExecutionRecommendation.ContinueObservation;
            return new LiveOpsExecutionReviewResult{ReadyForHumanReview=ready,RotationSafe=rr.RotationSafe,ExperimentObservationHealthy=healthy,MayProceedWithManualExecutionReview=ready&&rr.RotationSafe&&healthy&&productionAnalyticsProviderEnabled&&productionRemoteConfigEnabled&&liveOpsExecutionAuthorized&&contentRotationAuthorized&&experimentObservationAuthorized&&experimentTrafficChangeAuthorized&&manualExecutionReviewAuthorized,Recommendation=rec,ReviewBlockers=review.ToArray(),ActionBlockers=action.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
