using NUnit.Framework;
using LegendsOfRus.PostLaunchReview;

namespace LegendsOfRus.Tests.PostLaunchReview
{
    public sealed class TrendRetentionReviewEvaluatorEditModeTests
    {

        ReviewedGuardrailProfile P(){return new ReviewedGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-GR-0001",MinimumActiveUsers=100,MinimumCohortSize=100};}
        TrendWindowEvidence T(){return new TrendWindowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-TW-0001",WindowDays=7,BaselineActiveUsers=1000,CurrentActiveUsers=950,BaselineSessions=2000,CurrentSessions=1950,BaselineLevelStarts=5000,BaselineLevelCompletes=4000,CurrentLevelStarts=4800,CurrentLevelCompletes=3800};}
        RetentionCohortEvidence R(){return new RetentionCohortEvidence{EvidenceRetained=true,EvidenceReviewed=true,CohortId="LOR-COHORT-0001",CohortSize=1000,D1Returned=400,D7Returned=200,D30Returned=100,BaselineD1BasisPoints=4200,BaselineD7BasisPoints=2100,BaselineD30BasisPoints=1050};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        [Test] public void HealthyEvidencePasses(){var x=TrendRetentionReviewEvaluator.Evaluate(P(),T(),R());Assert.That(x.TrendHealthy&&x.RetentionHealthy,Is.True);}
        [Test] public void SampleFloorBlocksTrend(){var p=P();p.MinimumActiveUsers=2000;var x=TrendRetentionReviewEvaluator.Evaluate(p,T(),R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.TrendSampleInsufficient),Is.True);}
        [Test] public void SampleFloorBlocksRetention(){var p=P();p.MinimumCohortSize=2000;var x=TrendRetentionReviewEvaluator.Evaluate(p,T(),R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.RetentionSampleInsufficient),Is.True);}
        [Test] public void ActiveUserDeclineFlags(){var t=T();t.CurrentActiveUsers=500;var x=TrendRetentionReviewEvaluator.Evaluate(P(),t,R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.ActiveUsersDeclined),Is.True);}
        [Test] public void SessionPerUserDeclineFlags(){var t=T();t.CurrentSessions=900;var x=TrendRetentionReviewEvaluator.Evaluate(P(),t,R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.SessionsPerUserDeclined),Is.True);}
        [Test] public void CompletionDropFlags(){var t=T();t.CurrentLevelCompletes=2000;var x=TrendRetentionReviewEvaluator.Evaluate(P(),t,R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.CompletionRateDeclined),Is.True);}
        [Test] public void D1DropFlags(){var r=R();r.D1Returned=200;var x=TrendRetentionReviewEvaluator.Evaluate(P(),T(),r);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.D1RetentionDeclined),Is.True);}
        [Test] public void D7DropFlags(){var r=R();r.D7Returned=100;var x=TrendRetentionReviewEvaluator.Evaluate(P(),T(),r);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.D7RetentionDeclined),Is.True);}
        [Test] public void D30DropFlags(){var r=R();r.D30Returned=40;var x=TrendRetentionReviewEvaluator.Evaluate(P(),T(),r);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.D30RetentionDeclined),Is.True);}
        [Test] public void D1RateComputed(){Assert.That(TrendRetentionReviewEvaluator.Evaluate(P(),T(),R()).D1BasisPoints,Is.EqualTo(4000));}
        [Test] public void D7RateComputed(){Assert.That(TrendRetentionReviewEvaluator.Evaluate(P(),T(),R()).D7BasisPoints,Is.EqualTo(2000));}
        [Test] public void D30RateComputed(){Assert.That(TrendRetentionReviewEvaluator.Evaluate(P(),T(),R()).D30BasisPoints,Is.EqualTo(1000));}
        [Test] public void StructuralFailureStopsEvaluation(){var p=P();p.EvidenceRetained=false;var x=TrendRetentionReviewEvaluator.Evaluate(p,T(),R());Assert.That(x.StructurallyValid,Is.False);}
        [Test] public void ImprovementDoesNotFlag(){var t=T();t.CurrentActiveUsers=1200;t.CurrentSessions=2600;var x=TrendRetentionReviewEvaluator.Evaluate(P(),t,R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.ActiveUsersDeclined),Is.False);}
        [Test] public void ExactActiveThresholdPasses(){var p=P();p.MaxActiveUserRelativeDeclineBasisPoints=500;var x=TrendRetentionReviewEvaluator.Evaluate(p,T(),R());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.ActiveUsersDeclined),Is.False);}
        [Test] public void RetentionHealthyRequiresSample(){var p=P();p.MinimumCohortSize=2000;Assert.That(TrendRetentionReviewEvaluator.Evaluate(p,T(),R()).RetentionHealthy,Is.False);}
        [Test] public void TrendHealthyRequiresSample(){var p=P();p.MinimumActiveUsers=2000;Assert.That(TrendRetentionReviewEvaluator.Evaluate(p,T(),R()).TrendHealthy,Is.False);}
        [Test] public void BlockersAreDeterministic(){var a=TrendRetentionReviewEvaluator.Evaluate(P(),T(),R());var b=TrendRetentionReviewEvaluator.Evaluate(P(),T(),R());Assert.That(a.Blockers,Is.EqualTo(b.Blockers));}
    }
}
