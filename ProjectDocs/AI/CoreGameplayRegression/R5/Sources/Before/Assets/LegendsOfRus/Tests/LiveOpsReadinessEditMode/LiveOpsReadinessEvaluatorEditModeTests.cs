using NUnit.Framework;
using LegendsOfRus.LiveOpsReadiness;

namespace LegendsOfRus.Tests.LiveOpsReadiness
{
    public sealed class LiveOpsReadinessEvaluatorEditModeTests
    {

        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500,ConfigValidationFailures=0};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void HealthyEvidenceReadyForHumanReview(){Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),false,false,false,false,false).ReadyForHumanReview,Is.True);}
        [Test] public void DefaultRecommendationHoldsExperiment(){Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),false,false,false,false,false).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.HoldExperimentLaunch));}
        [Test] public void UnsafeCadenceRecommendsRevision(){var c=C();c.MinutesUntilWindowStart=1;Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),c,H(),E(),true,true,true,true,true).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.ReviseCadence));}
        [Test] public void UnhealthyContentRecommendsInvestigation(){var h=H();h.Completions=100;h.RewardClaims=50;Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),h,E(),true,true,true,true,true).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.InvestigateContentHealth));}
        [Test] public void BadExperimentRecommendsRevision(){var e=E();e.HypothesisReviewed=false;Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),e,true,true,true,true,true).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.ReviseExperiment));}
        [Test] public void FullApprovalsReadyForManualReview(){Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),true,true,true,true,true).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.ReadyForManualLiveOpsReview));}
        [Test] public void ManualPublishCanProceedOnlyWithAllApprovals(){Assert.That(LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),true,true,true,true,true).MayProceedWithManualLiveOpsPublishReview,Is.True);}
        [Test] public void ManualReviewAuthorizationReported(){var x=LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),true,true,true,true,false);Assert.That(Has(x.PublishBlockers,LiveOpsReadinessBlockerCodes.ManualLiveOpsReviewNotAuthorized),Is.True);}
        [Test] public void LiveOpsPublishAuthorizationRequired(){var x=LiveOpsReadinessEvaluator.Evaluate(P(),C(),H(),E(),true,true,true,false,true);Assert.That(Has(x.PublishBlockers,LiveOpsReadinessBlockerCodes.LiveOpsPublishNotAuthorized),Is.True);}
        [Test] public void ContentUnhealthyBlocksPublish(){var h=H();h.ContentCrashes=50;var x=LiveOpsReadinessEvaluator.Evaluate(P(),C(),h,E(),true,true,true,true,true);Assert.That(Has(x.PublishBlockers,LiveOpsReadinessBlockerCodes.ContentUnhealthy),Is.True);}
        [Test] public void CadenceUnsafeBlocksPublish(){var c=C();c.MinutesUntilWindowStart=0;var x=LiveOpsReadinessEvaluator.Evaluate(P(),c,H(),E(),true,true,true,true,true);Assert.That(Has(x.PublishBlockers,LiveOpsReadinessBlockerCodes.CadenceUnsafe),Is.True);}
        [Test] public void MissingProfileInsufficient(){Assert.That(LiveOpsReadinessEvaluator.Evaluate(null,C(),H(),E(),false,false,false,false,false).Recommendation,Is.EqualTo(LiveOpsReadinessRecommendation.InsufficientEvidence));}

    }
}
