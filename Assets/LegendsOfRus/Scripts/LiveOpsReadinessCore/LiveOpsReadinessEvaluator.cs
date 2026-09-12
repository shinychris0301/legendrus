using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class LiveOpsReadinessEvaluator
    {
        public static LiveOpsReadinessResult Evaluate(ReviewedLiveOpsGuardrailProfile profile, LiveOpsCadencePlanEvidence cadence, ContentHealthEvidence contentHealth, ExperimentProposalEvidence experiment, bool productionAnalyticsProviderEnabled, bool productionRemoteConfigEnabled, bool experimentLaunchAuthorized, bool liveOpsPublishAuthorized, bool manualLiveOpsReviewAuthorized)
        {
            CadenceReviewResult c = LiveOpsCadenceEvaluator.Evaluate(profile, cadence);
            ContentHealthReviewResult h = ContentHealthEvaluator.Evaluate(profile, contentHealth);
            ExperimentReadinessResult e = ExperimentReadinessEvaluator.Evaluate(profile, experiment, productionAnalyticsProviderEnabled, productionRemoteConfigEnabled, experimentLaunchAuthorized);
            var review = new List<string>(); Add(review,c.Blockers); Add(review,h.Blockers); Add(review,e.DesignBlockers);
            var publish = new List<string>();
            if (!c.CadenceSafe) AddOne(publish,LiveOpsReadinessBlockerCodes.CadenceUnsafe);
            if (!h.ContentHealthy) AddOne(publish,LiveOpsReadinessBlockerCodes.ContentUnhealthy);
            Add(publish,e.LaunchBlockers);
            if (!liveOpsPublishAuthorized) AddOne(publish,LiveOpsReadinessBlockerCodes.LiveOpsPublishNotAuthorized);
            if (!manualLiveOpsReviewAuthorized) AddOne(publish,LiveOpsReadinessBlockerCodes.ManualLiveOpsReviewNotAuthorized);
            bool ready = c.StructurallyValid && h.StructurallyValid && e.StructurallyValid && h.SampleSufficient;
            LiveOpsReadinessRecommendation r;
            if (!ready) r=LiveOpsReadinessRecommendation.InsufficientEvidence;
            else if (!c.CadenceSafe) r=LiveOpsReadinessRecommendation.ReviseCadence;
            else if (!h.ContentHealthy) r=LiveOpsReadinessRecommendation.InvestigateContentHealth;
            else if (!e.DesignReady) r=LiveOpsReadinessRecommendation.ReviseExperiment;
            else if (!e.LaunchReady) r=LiveOpsReadinessRecommendation.HoldExperimentLaunch;
            else if (manualLiveOpsReviewAuthorized) r=LiveOpsReadinessRecommendation.ReadyForManualLiveOpsReview;
            else r=LiveOpsReadinessRecommendation.ContinueObservation;
            return new LiveOpsReadinessResult { ReadyForHumanReview=ready, CadenceSafe=c.CadenceSafe, ContentHealthy=h.ContentHealthy, ExperimentDesignReady=e.DesignReady, ExperimentLaunchReady=e.LaunchReady, MayProceedWithManualLiveOpsPublishReview=ready && c.CadenceSafe && h.ContentHealthy && e.LaunchReady && liveOpsPublishAuthorized && manualLiveOpsReviewAuthorized, Recommendation=r, ReviewBlockers=review.ToArray(), PublishBlockers=publish.ToArray() };
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
