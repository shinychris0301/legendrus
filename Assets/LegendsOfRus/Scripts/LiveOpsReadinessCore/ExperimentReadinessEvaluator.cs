using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class ExperimentReadinessEvaluator
    {
        public static ExperimentReadinessResult Evaluate(ReviewedLiveOpsGuardrailProfile profile, ExperimentProposalEvidence experiment, bool productionAnalyticsProviderEnabled, bool productionRemoteConfigEnabled, bool experimentLaunchAuthorized)
        {
            var design = new List<string>();
            Add(design, LiveOpsReadinessValidation.ValidateProfile(profile));
            Add(design, LiveOpsReadinessValidation.ValidateExperiment(experiment));
            int samplePerVariant = 0;
            if (profile != null && experiment != null && design.Count == 0)
            {
                if (!experiment.HypothesisReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentHypothesisMissing);
                if (!experiment.PrimaryMetricReviewed || experiment.GuardrailMetricCount < 1) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentMetricPlanMissing);
                if (!experiment.MutuallyExclusiveExposure) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentMutualExclusionMissing);
                if (!experiment.StoppingRuleReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentStoppingRuleMissing);
                if (!experiment.PrivacyReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentPrivacyReviewMissing);
                if (experiment.ImpactsEconomy && !experiment.EconomyImpactReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentEconomyReviewMissing);
                if (experiment.ImpactsRewards && !experiment.RewardImpactReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentRewardReviewMissing);
                if (experiment.ImpactsDifficulty && !experiment.DifficultyImpactReviewed) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentDifficultyReviewMissing);
                if (experiment.DurationHours < profile.MinimumExperimentDurationHours || experiment.DurationHours > profile.MaximumExperimentDurationHours) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentDurationInvalid);
                int variants = experiment.Variants == null ? 0 : experiment.Variants.Length;
                if (variants > 0) samplePerVariant = experiment.ExpectedEligibleUsers / variants;
                if (samplePerVariant < profile.MinimumExperimentSamplePerVariant) AddOne(design, LiveOpsReadinessBlockerCodes.ExperimentSampleInsufficient);
            }
            bool designReady = design.Count == 0;
            var launch = new List<string>(); Add(launch, design.ToArray());
            if (!productionAnalyticsProviderEnabled) AddOne(launch, LiveOpsReadinessBlockerCodes.ProductionAnalyticsDisabled);
            if (!productionRemoteConfigEnabled) AddOne(launch, LiveOpsReadinessBlockerCodes.ProductionRemoteConfigDisabled);
            if (!experimentLaunchAuthorized) AddOne(launch, LiveOpsReadinessBlockerCodes.ExperimentLaunchNotAuthorized);
            bool measurementReady = productionAnalyticsProviderEnabled && productionRemoteConfigEnabled;
            return new ExperimentReadinessResult { StructurallyValid = experiment != null && LiveOpsReadinessValidation.ValidateExperiment(experiment).Length == 0 && profile != null && LiveOpsReadinessValidation.ValidateProfile(profile).Length == 0, DesignReady = designReady, MeasurementReady = measurementReady, LaunchReady = designReady && measurementReady && experimentLaunchAuthorized, ExpectedSamplePerVariant = samplePerVariant, DesignBlockers = design.ToArray(), LaunchBlockers = launch.ToArray() };
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
