using System.Collections.Generic;

namespace LegendsOfRus.PostLaunchReview
{
    public static class PostLaunchReviewEvaluator
    {
        public static PostLaunchReviewResult Evaluate(ReviewedGuardrailProfile profile, TrendWindowEvidence trend, RetentionCohortEvidence retention, EconomyFlowEvidence economy, bool manualEconomyChangeReviewAuthorized)
        {
            TrendRetentionReviewResult tr = TrendRetentionReviewEvaluator.Evaluate(profile, trend, retention);
            EconomyGuardrailResult eg = EconomyGuardrailEvaluator.Evaluate(profile, economy);
            var review = new List<string>();
            Add(review, tr.Blockers); Add(review, eg.Blockers);
            var change = new List<string>();
            if (!eg.EconomyHealthy) change.Add(PostLaunchReviewBlockerCodes.EconomyGuardrailBreached);
            if (!manualEconomyChangeReviewAuthorized) change.Add(PostLaunchReviewBlockerCodes.ManualEconomyReviewNotAuthorized);

            bool ready = tr.StructurallyValid && eg.StructurallyValid && tr.SampleSufficient && eg.SampleSufficient;
            bool trendHealthy = tr.TrendHealthy && tr.RetentionHealthy;
            bool economyHealthy = eg.EconomyHealthy;
            PostLaunchReviewRecommendation rec;
            if (!ready) rec = PostLaunchReviewRecommendation.InsufficientEvidence;
            else if (trendHealthy && economyHealthy) rec = PostLaunchReviewRecommendation.ContinueObservation;
            else if (!trendHealthy && economyHealthy) rec = PostLaunchReviewRecommendation.InvestigateTrendOrRetention;
            else if (trendHealthy && !economyHealthy) rec = PostLaunchReviewRecommendation.InvestigateEconomy;
            else rec = PostLaunchReviewRecommendation.InvestigateMultiple;
            if (!economyHealthy && manualEconomyChangeReviewAuthorized) rec = PostLaunchReviewRecommendation.HoldEconomyChangeReview;

            return new PostLaunchReviewResult
            {
                ReadyForHumanReview = ready,
                TrendRetentionHealthy = trendHealthy,
                EconomyHealthy = economyHealthy,
                MayProceedWithManualEconomyChangeReview = ready && trendHealthy && economyHealthy && manualEconomyChangeReviewAuthorized,
                Recommendation = rec,
                ReviewBlockers = review.ToArray(),
                EconomyChangeBlockers = change.ToArray()
            };
        }
        static void Add(List<string> dest, string[] values) { for(int i=0;i<values.Length;i++) if(!dest.Contains(values[i])) dest.Add(values[i]); }
    }
}
