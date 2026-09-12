using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class ContentHealthEvaluatorEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void HealthyContentPasses(){Assert.That(ContentHealthEvaluator.Evaluate(P(),H()).ContentHealthy,Is.True);}
        [Test] public void CompletionRateCalculated(){Assert.That(ContentHealthEvaluator.Evaluate(P(),H()).CompletionRateBasisPoints,Is.EqualTo(6000));}
        [Test] public void EntryFailureRateCalculated(){Assert.That(ContentHealthEvaluator.Evaluate(P(),H()).EntryFailureRateBasisPoints,Is.EqualTo(100));}
        [Test] public void CrashRateCalculated(){Assert.That(ContentHealthEvaluator.Evaluate(P(),H()).ContentCrashRateBasisPoints,Is.EqualTo(10));}
        [Test] public void RewardClaimRateCalculated(){Assert.That(ContentHealthEvaluator.Evaluate(P(),H()).RewardClaimRateBasisPoints,Is.EqualTo(8333));}
        [Test] public void SmallSampleBlocked(){var h=H();h.Participants=10;var x=ContentHealthEvaluator.Evaluate(P(),h);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.ContentSampleInsufficient),Is.True);}
        [Test] public void LowCompletionBlocked(){var h=H();h.Completions=100;h.RewardClaims=50;var x=ContentHealthEvaluator.Evaluate(P(),h);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.ContentCompletionLow),Is.True);}
        [Test] public void HighEntryFailureBlocked(){var h=H();h.EntryFailures=100;var x=ContentHealthEvaluator.Evaluate(P(),h);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.ContentEntryFailureHigh),Is.True);}
        [Test] public void HighCrashBlocked(){var h=H();h.ContentCrashes=30;var x=ContentHealthEvaluator.Evaluate(P(),h);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.ContentCrashHigh),Is.True);}
        [Test] public void ConfigFailureTriggersRewardAnomaly(){var h=H();h.ConfigValidationFailures=1;var x=ContentHealthEvaluator.Evaluate(P(),h);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.RewardClaimAnomaly),Is.True);}
        [Test] public void MissingHealthNotStructural(){Assert.That(ContentHealthEvaluator.Evaluate(P(),null).StructurallyValid,Is.False);}

    }
}
