using NUnit.Framework;
using LegendsOfRus.PostLaunchReview;

namespace LegendsOfRus.Tests.PostLaunchReview
{
    public sealed class EconomyGuardrailEvaluatorEditModeTests
    {

        ReviewedGuardrailProfile P(){return new ReviewedGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-GR-0001",MinimumActiveUsers=100};}
        EconomyFlowEvidence E(){return new EconomyFlowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-ECO-0001",ActiveUsers=1000,SoftCurrencyEarned=100000,SoftCurrencySpent=90000,HardCurrencyGranted=10000,HardCurrencySpent=9000,BoostersGranted=3000,BoostersConsumed=2500,BaselineRewardUnitsGranted=100000,CurrentRewardUnitsGranted=105000};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        [Test] public void HealthyEconomyPasses(){Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),E()).EconomyHealthy,Is.True);}
        [Test] public void SinkSourceComputed(){Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),E()).SoftCurrencySinkSourceRatioBasisPoints,Is.EqualTo(9000));}
        [Test] public void LowSinkRatioFlags(){var e=E();e.SoftCurrencySpent=10000;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceLow),Is.True);}
        [Test] public void HighSinkRatioFlags(){var e=E();e.SoftCurrencySpent=200000;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceHigh),Is.True);}
        [Test] public void MissingSoftFlowFlags(){var e=E();e.SoftCurrencyEarned=0;e.SoftCurrencySpent=0;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.SoftCurrencyFlowMissing),Is.True);}
        [Test] public void HardNetGrantComputed(){Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),E()).HardCurrencyNetGrantPerActiveUser,Is.EqualTo(1));}
        [Test] public void HardNetGrantFlags(){var e=E();e.HardCurrencyGranted=50000;e.HardCurrencySpent=0;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.HardCurrencyNetGrantHigh),Is.True);}
        [Test] public void BoosterNetGrantComputed(){Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),E()).BoosterNetGrantPerActiveUser,Is.Zero);}
        [Test] public void BoosterNetGrantFlags(){var e=E();e.BoostersGranted=10000;e.BoostersConsumed=0;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.BoosterNetGrantHigh),Is.True);}
        [Test] public void RewardIncreaseComputed(){Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),E()).RewardGrantRelativeIncreaseBasisPoints,Is.EqualTo(500));}
        [Test] public void RewardIncreaseFlags(){var e=E();e.CurrentRewardUnitsGranted=150000;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.RewardGrantDeltaHigh),Is.True);}
        [Test] public void LowerRewardDoesNotFlag(){var e=E();e.CurrentRewardUnitsGranted=90000;var x=EconomyGuardrailEvaluator.Evaluate(P(),e);Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.RewardGrantDeltaHigh),Is.False);}
        [Test] public void SampleFloorBlocks(){var p=P();p.MinimumActiveUsers=2000;var x=EconomyGuardrailEvaluator.Evaluate(p,E());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.EconomySampleInsufficient),Is.True);}
        [Test] public void StructuralFailureStops(){var e=E();e.EvidenceReviewed=false;Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),e).StructurallyValid,Is.False);}
        [Test] public void EqualGrantSpendHasZeroNet(){var e=E();e.HardCurrencySpent=e.HardCurrencyGranted;Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),e).HardCurrencyNetGrantPerActiveUser,Is.Zero);}
        [Test] public void ConsumptionAboveGrantHasZeroNet(){var e=E();e.BoostersConsumed=4000;Assert.That(EconomyGuardrailEvaluator.Evaluate(P(),e).BoosterNetGrantPerActiveUser,Is.Zero);}
        [Test] public void ExactSoftMinPasses(){var p=P();p.MinSoftCurrencySinkSourceRatioBasisPoints=9000;var x=EconomyGuardrailEvaluator.Evaluate(p,E());Assert.That(Has(x.Blockers,PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceLow),Is.False);}
        [Test] public void DeterministicBlockers(){var a=EconomyGuardrailEvaluator.Evaluate(P(),E());var b=EconomyGuardrailEvaluator.Evaluate(P(),E());Assert.That(a.Blockers,Is.EqualTo(b.Blockers));}
    }
}
