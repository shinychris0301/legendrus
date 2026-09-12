using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsBaseline
{
    public static class V119EvidenceChainEvaluator
    {
        public static EvidenceChainReviewResult Evaluate(V119EvidenceChainEvidence e)
        {
            var b=LiveOpsBaselineValidation.ValidateChain(e);
            return new EvidenceChainReviewResult{StructurallyValid=b.Length==0,ChainReady=b.Length==0,Blockers=b};
        }
    }

    public static class OperatingBaselineEvaluator
    {
        public static OperatingBaselineReviewResult Evaluate(LiveOpsOperatingBaselineEvidence e,string chainId)
        {
            var b=LiveOpsBaselineValidation.ValidateBaseline(e,chainId);
            return new OperatingBaselineReviewResult{StructurallyValid=b.Length==0,BaselineReady=b.Length==0,Blockers=b};
        }
    }

    public static class V120TransitionEvaluator
    {
        public static TransitionReviewResult Evaluate(V120TransitionPlanEvidence e,string baselineId)
        {
            var b=LiveOpsBaselineValidation.ValidateTransition(e,baselineId);
            return new TransitionReviewResult{StructurallyValid=b.Length==0,TransitionReady=b.Length==0,Blockers=b};
        }
    }

    public static class LiveOpsBaselineReviewEvaluator
    {
        public static LiveOpsBaselineReviewResult Evaluate(
            V119EvidenceChainEvidence chain,
            LiveOpsOperatingBaselineEvidence baseline,
            V120TransitionPlanEvidence transition,
            bool productionAnalyticsEnabled,
            bool productionRemoteConfigEnabled,
            bool chainReviewAuthorized,
            bool baselineReviewAuthorized,
            bool transitionReviewAuthorized,
            bool manualV119ClosureAuthorized,
            bool v120PreparationAuthorized,
            bool liveOpsPublishingAuthorized)
        {
            var review=new List<string>();
            var action=new List<string>();
            var c=V119EvidenceChainEvaluator.Evaluate(chain);
            var b=OperatingBaselineEvaluator.Evaluate(baseline,chain==null?string.Empty:chain.ChainId);
            var t=V120TransitionEvaluator.Evaluate(transition,baseline==null?string.Empty:baseline.BaselineId);
            Add(review,c.Blockers); Add(review,b.Blockers); Add(review,t.Blockers);
            if(!productionAnalyticsEnabled)review.Add(LiveOpsBaselineBlockerCodes.ProductionAnalyticsDisabled);
            if(!productionRemoteConfigEnabled)review.Add(LiveOpsBaselineBlockerCodes.ProductionRemoteConfigDisabled);
            if(!chainReviewAuthorized)action.Add(LiveOpsBaselineBlockerCodes.ChainReviewNotAuthorized);
            if(!baselineReviewAuthorized)action.Add(LiveOpsBaselineBlockerCodes.BaselineReviewNotAuthorized);
            if(!transitionReviewAuthorized)action.Add(LiveOpsBaselineBlockerCodes.TransitionReviewNotAuthorized);
            if(!manualV119ClosureAuthorized)action.Add(LiveOpsBaselineBlockerCodes.V119ClosureNotAuthorized);
            if(!v120PreparationAuthorized)action.Add(LiveOpsBaselineBlockerCodes.V120PreparationNotAuthorized);
            if(!liveOpsPublishingAuthorized)action.Add(LiveOpsBaselineBlockerCodes.LiveOpsPublishingNotAuthorized);
            bool structural=c.ChainReady&&b.BaselineReady&&t.TransitionReady;
            var recommendation=!c.ChainReady?LiveOpsBaselineRecommendation.CompleteV119Chain:!b.BaselineReady?LiveOpsBaselineRecommendation.CompleteOperatingBaseline:!t.TransitionReady?LiveOpsBaselineRecommendation.ReviseV120Transition:LiveOpsBaselineRecommendation.ReadyForManualV119Closure;
            bool reviewReady=structural;
            bool mayClose=structural&&chainReviewAuthorized&&baselineReviewAuthorized&&transitionReviewAuthorized&&manualV119ClosureAuthorized;
            bool mayBeginV120=mayClose&&v120PreparationAuthorized;
            return new LiveOpsBaselineReviewResult{ReadyForHumanReview=reviewReady,MayCloseV119Manually=mayClose,MayBeginV120Preparation=mayBeginV120,Recommendation=recommendation,ReviewBlockers=review.ToArray(),ActionBlockers=action.ToArray()};
        }
        static void Add(List<string> dst,string[] src){if(src==null)return;for(int i=0;i<src.Length;i++)dst.Add(src[i]);}
    }
}
