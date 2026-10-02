using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class LiveOpsReadinessValidationEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void StableProfileIdAccepted(){Assert.That(LiveOpsReadinessValidation.IsStableId("LOR-LO-GR-0001",LiveOpsReadinessRequirements.GuardrailProfilePrefix),Is.True);}
        [Test] public void LowercaseStableIdRejected(){Assert.That(LiveOpsReadinessValidation.IsStableId("LOR-LO-GR-bad",LiveOpsReadinessRequirements.GuardrailProfilePrefix),Is.False);}
        [Test] public void RateBasisPointsWorks(){Assert.That(LiveOpsReadinessValidation.RateBasisPoints(1,4),Is.EqualTo(2500));}
        [Test] public void MissingProfileBlocked(){Assert.That(Has(LiveOpsReadinessValidation.ValidateProfile(null),LiveOpsReadinessBlockerCodes.ProfileMissing),Is.True);}
        [Test] public void ReviewedProfileValid(){Assert.That(LiveOpsReadinessValidation.ValidateProfile(P()),Is.Empty);}
        [Test] public void MissingCadenceBlocked(){Assert.That(Has(LiveOpsReadinessValidation.ValidateCadence(null),LiveOpsReadinessBlockerCodes.CadenceMissing),Is.True);}
        [Test] public void HealthyCadenceValid(){Assert.That(LiveOpsReadinessValidation.ValidateCadence(C()),Is.Empty);}
        [Test] public void DuplicateEventIdBlocked(){var c=C();c.Events[1].EventId=c.Events[0].EventId;Assert.That(Has(LiveOpsReadinessValidation.ValidateCadence(c),LiveOpsReadinessBlockerCodes.EventIdInvalid),Is.True);}
        [Test] public void InvalidEventTimeBlocked(){var c=C();c.Events[0].EndMinute=0;Assert.That(Has(LiveOpsReadinessValidation.ValidateCadence(c),LiveOpsReadinessBlockerCodes.EventTimeInvalid),Is.True);}
        [Test] public void HealthyContentValid(){Assert.That(LiveOpsReadinessValidation.ValidateContentHealth(H()),Is.Empty);}
        [Test] public void CompletionAboveEntriesInvalid(){var h=H();h.Completions=1001;Assert.That(Has(LiveOpsReadinessValidation.ValidateContentHealth(h),LiveOpsReadinessBlockerCodes.ContentHealthInvalid),Is.True);}
        [Test] public void HealthyExperimentValid(){Assert.That(LiveOpsReadinessValidation.ValidateExperiment(E()),Is.Empty);}
        [Test] public void BadWeightsBlocked(){var e=E();e.Variants[0].WeightBasisPoints=4000;Assert.That(Has(LiveOpsReadinessValidation.ValidateExperiment(e),LiveOpsReadinessBlockerCodes.ExperimentWeightInvalid),Is.True);}
        [Test] public void DuplicateVariantBlocked(){var e=E();e.Variants[1].VariantId="CONTROL";Assert.That(Has(LiveOpsReadinessValidation.ValidateExperiment(e),LiveOpsReadinessBlockerCodes.ExperimentVariantInvalid),Is.True);}

    }
}
