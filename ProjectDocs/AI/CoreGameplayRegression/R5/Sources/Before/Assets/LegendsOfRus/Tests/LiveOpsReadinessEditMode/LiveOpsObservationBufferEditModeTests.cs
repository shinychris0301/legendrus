using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class LiveOpsObservationBufferEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void DefaultCapacityPinned(){Assert.That(new LiveOpsObservationBuffer().Capacity,Is.EqualTo(30));}
        [Test] public void MaxCapacityAccepted(){Assert.That(new LiveOpsObservationBuffer(90).Capacity,Is.EqualTo(90));}
        [Test] public void ZeroCapacityRejected(){Assert.Throws<System.ArgumentOutOfRangeException>(()=>new LiveOpsObservationBuffer(0));}
        [Test] public void OverMaxCapacityRejected(){Assert.Throws<System.ArgumentOutOfRangeException>(()=>new LiveOpsObservationBuffer(91));}
        [Test] public void AddIncreasesCount(){var b=new LiveOpsObservationBuffer(2);b.Add(new LiveOpsAggregateObservation{WindowIndex=1});Assert.That(b.Count,Is.EqualTo(1));}
        [Test] public void BufferIsBounded(){var b=new LiveOpsObservationBuffer(2);b.Add(new LiveOpsAggregateObservation{WindowIndex=1});b.Add(new LiveOpsAggregateObservation{WindowIndex=2});b.Add(new LiveOpsAggregateObservation{WindowIndex=3});Assert.That(b.Count,Is.EqualTo(2));}
        [Test] public void SnapshotOldestFirst(){var b=new LiveOpsObservationBuffer(2);b.Add(new LiveOpsAggregateObservation{WindowIndex=1});b.Add(new LiveOpsAggregateObservation{WindowIndex=2});b.Add(new LiveOpsAggregateObservation{WindowIndex=3});Assert.That(b.SnapshotOldestFirst()[0].WindowIndex,Is.EqualTo(2));}
        [Test] public void SnapshotNewestLast(){var b=new LiveOpsObservationBuffer(2);b.Add(new LiveOpsAggregateObservation{WindowIndex=1});b.Add(new LiveOpsAggregateObservation{WindowIndex=2});b.Add(new LiveOpsAggregateObservation{WindowIndex=3});Assert.That(b.SnapshotOldestFirst()[1].WindowIndex,Is.EqualTo(3));}
        [Test] public void ClearResetsCount(){var b=new LiveOpsObservationBuffer(2);b.Add(new LiveOpsAggregateObservation());b.Clear();Assert.That(b.Count,Is.EqualTo(0));}
        [Test] public void NullObservationRejected(){var b=new LiveOpsObservationBuffer(2);Assert.Throws<System.ArgumentNullException>(()=>b.Add(null));}

    }
}
