using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsCycle
{
    public static class RetrospectiveEvaluator
    {
        public static RetrospectiveReviewResult Evaluate(CycleRetrospectiveEvidence evidence)
        {
            var b=LiveOpsCycleValidation.ValidateRetrospective(evidence);
            return new RetrospectiveReviewResult{StructurallyValid=b.Length==0,ReadyForArchive=b.Length==0,Blockers=b};
        }
    }

    public static class CycleArchiveEvaluator
    {
        public static ArchiveReviewResult Evaluate(CycleArchiveEvidence evidence)
        {
            var b=LiveOpsCycleValidation.ValidateArchive(evidence);
            return new ArchiveReviewResult{StructurallyValid=b.Length==0,ArchiveReady=b.Length==0,Blockers=b};
        }
    }

    public static class NextCadenceEvaluator
    {
        public static NextCadenceReviewResult Evaluate(NextCadencePlanEvidence evidence)
        {
            var b=LiveOpsCycleValidation.ValidateCadence(evidence);
            return new NextCadenceReviewResult{StructurallyValid=b.Length==0,CadenceReady=b.Length==0,Blockers=b};
        }
    }

    public static class LiveOpsCycleReviewEvaluator
    {
        public static LiveOpsCycleReviewResult Evaluate(CycleRetrospectiveEvidence retrospective,CycleArchiveEvidence archive,NextCadencePlanEvidence cadence,bool productionAnalyticsEnabled,bool productionRemoteConfigEnabled,bool retrospectiveReviewAuthorized,bool archiveReviewAuthorized,bool nextCadenceReviewAuthorized,bool manualCycleClosureAuthorized,bool liveOpsPublishingAuthorized)
        {
            var rr=RetrospectiveEvaluator.Evaluate(retrospective);var ar=CycleArchiveEvaluator.Evaluate(archive);var cr=NextCadenceEvaluator.Evaluate(cadence);
            var review=new List<string>();Add(review,rr.Blockers);Add(review,ar.Blockers);Add(review,cr.Blockers);
            var action=new List<string>();
            if(!productionAnalyticsEnabled)action.Add(LiveOpsCycleBlockerCodes.ProductionAnalyticsDisabled);
            if(!productionRemoteConfigEnabled)action.Add(LiveOpsCycleBlockerCodes.ProductionRemoteConfigDisabled);
            if(!retrospectiveReviewAuthorized)action.Add(LiveOpsCycleBlockerCodes.RetrospectiveReviewNotAuthorized);
            if(!archiveReviewAuthorized)action.Add(LiveOpsCycleBlockerCodes.ArchiveReviewNotAuthorized);
            if(!nextCadenceReviewAuthorized)action.Add(LiveOpsCycleBlockerCodes.NextCadenceReviewNotAuthorized);
            if(!manualCycleClosureAuthorized)action.Add(LiveOpsCycleBlockerCodes.ManualCycleClosureNotAuthorized);
            if(!liveOpsPublishingAuthorized)action.Add(LiveOpsCycleBlockerCodes.LiveOpsPublishingNotAuthorized);
            var rec=LiveOpsCycleRecommendation.InsufficientEvidence;
            if(rr.Blockers.Length>0)rec=LiveOpsCycleRecommendation.ResolveRetrospective;
            else if(ar.Blockers.Length>0)rec=LiveOpsCycleRecommendation.CompleteArchive;
            else if(cr.Blockers.Length>0)rec=LiveOpsCycleRecommendation.ReviseNextCadence;
            else rec=LiveOpsCycleRecommendation.ReadyForManualCycleClosure;
            bool ready=review.Count==0;
            bool close=ready&&productionAnalyticsEnabled&&retrospectiveReviewAuthorized&&archiveReviewAuthorized&&manualCycleClosureAuthorized;
            bool publish=close&&productionRemoteConfigEnabled&&nextCadenceReviewAuthorized&&liveOpsPublishingAuthorized;
            return new LiveOpsCycleReviewResult{ReadyForHumanReview=ready,MayCloseCycleManually=close,MayPublishNextCadence=publish,Recommendation=rec,ReviewBlockers=review.ToArray(),ActionBlockers=Unique(action)};
        }
        static void Add(List<string>d,string[]s){for(int i=0;i<s.Length;i++)if(!d.Contains(s[i]))d.Add(s[i]);}
        static string[] Unique(List<string>v){var r=new List<string>();for(int i=0;i<v.Count;i++)if(!r.Contains(v[i]))r.Add(v[i]);return r.ToArray();}
    }
}
