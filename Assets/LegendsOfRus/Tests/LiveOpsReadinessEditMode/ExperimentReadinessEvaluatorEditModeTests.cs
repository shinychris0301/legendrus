using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class ExperimentReadinessEvaluatorEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void HealthyDesignReady(){Assert.That(ExperimentReadinessEvaluator.Evaluate(P(),E(),false,false,false).DesignReady,Is.True);}
        [Test] public void ProductionMeasurementDisabledByDefault(){Assert.That(ExperimentReadinessEvaluator.Evaluate(P(),E(),false,false,false).MeasurementReady,Is.False);}
        [Test] public void LaunchDisabledWithoutAuthorization(){var x=ExperimentReadinessEvaluator.Evaluate(P(),E(),true,true,false);Assert.That(x.LaunchReady,Is.False);}
        [Test] public void LaunchReadyWithAllApprovals(){Assert.That(ExperimentReadinessEvaluator.Evaluate(P(),E(),true,true,true).LaunchReady,Is.True);}
        [Test] public void HypothesisRequired(){var e=E();e.HypothesisReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentHypothesisMissing),Is.True);}
        [Test] public void PrimaryMetricRequired(){var e=E();e.PrimaryMetricReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentMetricPlanMissing),Is.True);}
        [Test] public void GuardrailMetricRequired(){var e=E();e.GuardrailMetricCount=0;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentMetricPlanMissing),Is.True);}
        [Test] public void MutualExclusionRequired(){var e=E();e.MutuallyExclusiveExposure=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentMutualExclusionMissing),Is.True);}
        [Test] public void StoppingRuleRequired(){var e=E();e.StoppingRuleReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentStoppingRuleMissing),Is.True);}
        [Test] public void PrivacyReviewRequired(){var e=E();e.PrivacyReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentPrivacyReviewMissing),Is.True);}
        [Test] public void EconomyImpactReviewRequired(){var e=E();e.ImpactsEconomy=true;e.EconomyImpactReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentEconomyReviewMissing),Is.True);}
        [Test] public void RewardImpactReviewRequired(){var e=E();e.ImpactsRewards=true;e.RewardImpactReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentRewardReviewMissing),Is.True);}
        [Test] public void DifficultyImpactReviewRequired(){var e=E();e.ImpactsDifficulty=true;e.DifficultyImpactReviewed=false;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentDifficultyReviewMissing),Is.True);}
        [Test] public void SampleFloorEnforced(){var e=E();e.ExpectedEligibleUsers=100;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentSampleInsufficient),Is.True);}
        [Test] public void DurationFloorEnforced(){var e=E();e.DurationHours=1;var x=ExperimentReadinessEvaluator.Evaluate(P(),e,true,true,true);Assert.That(Has(x.DesignBlockers,LiveOpsReadinessBlockerCodes.ExperimentDurationInvalid),Is.True);}
        [Test] public void AnalyticsGateReported(){var x=ExperimentReadinessEvaluator.Evaluate(P(),E(),false,true,true);Assert.That(Has(x.LaunchBlockers,LiveOpsReadinessBlockerCodes.ProductionAnalyticsDisabled),Is.True);}
        [Test] public void RemoteConfigGateReported(){var x=ExperimentReadinessEvaluator.Evaluate(P(),E(),true,false,true);Assert.That(Has(x.LaunchBlockers,LiveOpsReadinessBlockerCodes.ProductionRemoteConfigDisabled),Is.True);}
        [Test] public void ExpectedSamplePerVariantCalculated(){Assert.That(ExperimentReadinessEvaluator.Evaluate(P(),E(),false,false,false).ExpectedSamplePerVariant,Is.EqualTo(500));}

    }
}
