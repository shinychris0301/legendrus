using NUnit.Framework;
using LegendsOfRus.PostLaunchReview;

namespace LegendsOfRus.Tests.PostLaunchReview
{
    public sealed class PostLaunchReviewValidationEditModeTests
    {

        ReviewedGuardrailProfile P(){return new ReviewedGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-GR-0001"};}
        TrendWindowEvidence T(){return new TrendWindowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-TW-0001",WindowDays=7,BaselineActiveUsers=100,CurrentActiveUsers=100,BaselineSessions=200,CurrentSessions=200,BaselineLevelStarts=100,BaselineLevelCompletes=80,CurrentLevelStarts=100,CurrentLevelCompletes=80};}
        RetentionCohortEvidence R(){return new RetentionCohortEvidence{EvidenceRetained=true,EvidenceReviewed=true,CohortId="LOR-COHORT-0001",CohortSize=100,D1Returned=40,D7Returned=20,D30Returned=10,BaselineD1BasisPoints=4000,BaselineD7BasisPoints=2000,BaselineD30BasisPoints=1000};}
        EconomyFlowEvidence E(){return new EconomyFlowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-ECO-0001",ActiveUsers=100,SoftCurrencyEarned=1000,SoftCurrencySpent=900};}
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        [Test] public void ValidProfilePasses(){Assert.That(PostLaunchReviewValidation.ValidateProfile(P()),Is.Empty);}
        [Test] public void MissingProfileBlocks(){Assert.That(Has(PostLaunchReviewValidation.ValidateProfile(null),PostLaunchReviewBlockerCodes.ProfileMissing),Is.True);}
        [Test] public void UnreviewedProfileBlocks(){var x=P();x.EvidenceReviewed=false;Assert.That(Has(PostLaunchReviewValidation.ValidateProfile(x),PostLaunchReviewBlockerCodes.ProfileNotReviewed),Is.True);}
        [Test] public void BadProfileIdBlocks(){var x=P();x.ProfileId="bad id";Assert.That(Has(PostLaunchReviewValidation.ValidateProfile(x),PostLaunchReviewBlockerCodes.ProfileIdInvalid),Is.True);}
        [Test] public void BadGuardrailBlocks(){var x=P();x.MinimumActiveUsers=0;Assert.That(Has(PostLaunchReviewValidation.ValidateProfile(x),PostLaunchReviewBlockerCodes.GuardrailInvalid),Is.True);}
        [Test] public void ValidTrendPasses(){Assert.That(PostLaunchReviewValidation.ValidateTrend(T()),Is.Empty);}
        [Test] public void MissingTrendBlocks(){Assert.That(Has(PostLaunchReviewValidation.ValidateTrend(null),PostLaunchReviewBlockerCodes.TrendMissing),Is.True);}
        [Test] public void TrendCompleteCannotExceedStarts(){var x=T();x.CurrentLevelCompletes=101;Assert.That(Has(PostLaunchReviewValidation.ValidateTrend(x),PostLaunchReviewBlockerCodes.TrendCountInvalid),Is.True);}
        [Test] public void TrendWindowDaysBounded(){var x=T();x.WindowDays=91;Assert.That(Has(PostLaunchReviewValidation.ValidateTrend(x),PostLaunchReviewBlockerCodes.TrendIdInvalid),Is.True);}
        [Test] public void ValidRetentionPasses(){Assert.That(PostLaunchReviewValidation.ValidateRetention(R()),Is.Empty);}
        [Test] public void MissingRetentionBlocks(){Assert.That(Has(PostLaunchReviewValidation.ValidateRetention(null),PostLaunchReviewBlockerCodes.RetentionMissing),Is.True);}
        [Test] public void ReturnedCannotExceedCohort(){var x=R();x.D1Returned=101;Assert.That(Has(PostLaunchReviewValidation.ValidateRetention(x),PostLaunchReviewBlockerCodes.RetentionCountInvalid),Is.True);}
        [Test] public void RetentionOrderMustBeMonotonic(){var x=R();x.D7Returned=50;Assert.That(Has(PostLaunchReviewValidation.ValidateRetention(x),PostLaunchReviewBlockerCodes.RetentionOrderInvalid),Is.True);}
        [Test] public void ValidEconomyPasses(){Assert.That(PostLaunchReviewValidation.ValidateEconomy(E()),Is.Empty);}
        [Test] public void MissingEconomyBlocks(){Assert.That(Has(PostLaunchReviewValidation.ValidateEconomy(null),PostLaunchReviewBlockerCodes.EconomyMissing),Is.True);}
        [Test] public void NegativeEconomyCountBlocks(){var x=E();x.SoftCurrencyEarned=-1;Assert.That(Has(PostLaunchReviewValidation.ValidateEconomy(x),PostLaunchReviewBlockerCodes.EconomyCountInvalid),Is.True);}
        [Test] public void StableIdAcceptsUppercaseDigits(){Assert.That(PostLaunchReviewValidation.IsStableId("LOR-TW-WEEK-0001",PostLaunchReviewRequirements.TrendWindowPrefix),Is.True);}
        [Test] public void StableIdRejectsSpaces(){Assert.That(PostLaunchReviewValidation.IsStableId("LOR-TW-BAD ID",PostLaunchReviewRequirements.TrendWindowPrefix),Is.False);}
        [Test] public void StableIdRejectsWrongPrefix(){Assert.That(PostLaunchReviewValidation.IsStableId("LOR-ECO-0001",PostLaunchReviewRequirements.TrendWindowPrefix),Is.False);}
        [Test] public void RateBasisPointsComputes(){Assert.That(PostLaunchReviewValidation.RateBasisPoints(1,4),Is.EqualTo(2500));}
        [Test] public void RelativeDeclineFloorsAtZero(){Assert.That(PostLaunchReviewValidation.PositiveRelativeDeclineBasisPoints(100,110),Is.Zero);}
        [Test] public void PositiveDropFloorsAtZero(){Assert.That(PostLaunchReviewValidation.PositiveDrop(4000,4500),Is.Zero);}
    }
}
