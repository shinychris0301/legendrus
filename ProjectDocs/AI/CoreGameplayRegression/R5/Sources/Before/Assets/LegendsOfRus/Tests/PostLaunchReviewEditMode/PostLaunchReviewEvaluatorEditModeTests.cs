using NUnit.Framework;
using LegendsOfRus.PostLaunchReview;

namespace LegendsOfRus.Tests.PostLaunchReview
{
    public sealed class PostLaunchReviewEvaluatorEditModeTests
    {

        ReviewedGuardrailProfile P(){return new ReviewedGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-GR-0001",MinimumActiveUsers=100,MinimumCohortSize=100};}
        TrendWindowEvidence T(){return new TrendWindowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-TW-0001",WindowDays=7,BaselineActiveUsers=1000,CurrentActiveUsers=1000,BaselineSessions=2000,CurrentSessions=2000,BaselineLevelStarts=1000,BaselineLevelCompletes=800,CurrentLevelStarts=1000,CurrentLevelCompletes=800};}
        RetentionCohortEvidence R(){return new RetentionCohortEvidence{EvidenceRetained=true,EvidenceReviewed=true,CohortId="LOR-COHORT-0001",CohortSize=1000,D1Returned=400,D7Returned=200,D30Returned=100,BaselineD1BasisPoints=4000,BaselineD7BasisPoints=2000,BaselineD30BasisPoints=1000};}
        EconomyFlowEvidence E(){return new EconomyFlowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-ECO-0001",ActiveUsers=1000,SoftCurrencyEarned=100000,SoftCurrencySpent=90000,HardCurrencyGranted=1000,HardCurrencySpent=1000,BoostersGranted=1000,BoostersConsumed=1000,BaselineRewardUnitsGranted=100000,CurrentRewardUnitsGranted=100000};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        [Test] public void HealthyReviewContinuesObservation(){var x=PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false);Assert.That(x.Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.ContinueObservation));}
        [Test] public void HealthyReviewReady(){Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false).ReadyForHumanReview,Is.True);}
        [Test] public void ManualEconomyReviewNeedsAuthorization(){Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false).MayProceedWithManualEconomyChangeReview,Is.False);}
        [Test] public void ManualEconomyReviewCanProceedWhenHealthyAndAuthorized(){Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),true).MayProceedWithManualEconomyChangeReview,Is.True);}
        [Test] public void TrendProblemRecommendsTrendInvestigation(){var t=T();t.CurrentActiveUsers=500;Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),t,R(),E(),false).Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.InvestigateTrendOrRetention));}
        [Test] public void EconomyProblemRecommendsEconomyInvestigation(){var e=E();e.SoftCurrencySpent=1000;Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),e,false).Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.InvestigateEconomy));}
        [Test] public void MultipleProblemsRecommendMultiple(){var t=T();t.CurrentActiveUsers=500;var e=E();e.SoftCurrencySpent=1000;Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),t,R(),e,false).Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.InvestigateMultiple));}
        [Test] public void AuthorizedButUnhealthyHoldsEconomyReview(){var e=E();e.SoftCurrencySpent=1000;Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),e,true).Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.HoldEconomyChangeReview));}
        [Test] public void MissingProfileIsInsufficient(){var x=PostLaunchReviewEvaluator.Evaluate(null,T(),R(),E(),false);Assert.That(x.Recommendation,Is.EqualTo(PostLaunchReviewRecommendation.InsufficientEvidence));}
        [Test] public void ManualBlockerPresentWhenUnauthorized(){var x=PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false);Assert.That(Has(x.EconomyChangeBlockers,PostLaunchReviewBlockerCodes.ManualEconomyReviewNotAuthorized),Is.True);}
        [Test] public void ReviewBlockersDeterministic(){var a=PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false);var b=PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false);Assert.That(a.ReviewBlockers,Is.EqualTo(b.ReviewBlockers));}
        [Test] public void ReviewDoesNotNeedManualAuthorizationToBeReady(){Assert.That(PostLaunchReviewEvaluator.Evaluate(P(),T(),R(),E(),false).ReadyForHumanReview,Is.True);}
    }
}
