using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class LiveOpsCadenceEvaluatorEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void HealthyCadenceSafe(){Assert.That(LiveOpsCadenceEvaluator.Evaluate(P(),C()).CadenceSafe,Is.True);}
        [Test] public void LeadTimeEnforced(){var c=C();c.MinutesUntilWindowStart=1;var x=LiveOpsCadenceEvaluator.Evaluate(P(),c);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.EventLeadTimeInsufficient),Is.True);}
        [Test] public void MaxDurationEnforced(){var c=C();c.Events[0].EndMinute=2000;var x=LiveOpsCadenceEvaluator.Evaluate(P(),c);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.EventDurationExceeded),Is.True);}
        [Test] public void ConcurrencyEnforced(){var p=P();p.MaximumConcurrentEvents=1;var c=C();c.Events[1].StartMinute=60;c.Events[1].EndMinute=200;var x=LiveOpsCadenceEvaluator.Evaluate(p,c);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.ConcurrentEventsExceeded),Is.True);}
        [Test] public void GapEnforced(){var c=C();c.Events[1].StartMinute=130;var x=LiveOpsCadenceEvaluator.Evaluate(P(),c);Assert.That(Has(x.Blockers,LiveOpsReadinessBlockerCodes.EventGapInsufficient),Is.True);}
        [Test] public void ExactGapAccepted(){var c=C();c.Events[1].StartMinute=150;Assert.That(LiveOpsCadenceEvaluator.Evaluate(P(),c).CadenceSafe,Is.True);}
        [Test] public void ObservedConcurrencyOneForSeparatedEvents(){Assert.That(LiveOpsCadenceEvaluator.Evaluate(P(),C()).MaximumObservedConcurrency,Is.EqualTo(1));}
        [Test] public void ObservedConcurrencyTwoForOverlap(){var c=C();c.Events[1].StartMinute=60;Assert.That(LiveOpsCadenceEvaluator.Evaluate(P(),c).MaximumObservedConcurrency,Is.EqualTo(2));}
        [Test] public void MissingProfileNotStructural(){Assert.That(LiveOpsCadenceEvaluator.Evaluate(null,C()).StructurallyValid,Is.False);}
        [Test] public void MissingPlanNotStructural(){Assert.That(LiveOpsCadenceEvaluator.Evaluate(P(),null).StructurallyValid,Is.False);}

    }
}
