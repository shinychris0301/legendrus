using LegendsOfRus.ClosedTestExecution;
using NUnit.Framework;

namespace LegendsOfRus.Tests.ClosedTestExecution
{
    public sealed class ClosedTestExecutionFeedbackProductionAccessEditModeTests
    {
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [Test] public void Sha256ValidatorAcceptsHex() => Assert.That(ClosedTestExecutionValidation.IsSha256(HashA), Is.True);
        [Test] public void Sha256ValidatorRejectsShortValue() => Assert.That(ClosedTestExecutionValidation.IsSha256("abc"), Is.False);
        [Test] public void CandidateIdValidatorAcceptsRc01() => Assert.That(ClosedTestExecutionValidation.IsCandidateId("v1.18-step05-rc01"), Is.True);
        [Test] public void CandidateIdValidatorRejectsRc00() => Assert.That(ClosedTestExecutionValidation.IsCandidateId("v1.18-step05-rc00"), Is.False);
        [Test] public void IssueIdValidatorAcceptsFourDigits() => Assert.That(FeedbackTriageEvaluator.IsIssueId("LOR-CT-0001"), Is.True);
        [Test] public void IssueIdValidatorRejectsZero() => Assert.That(FeedbackTriageEvaluator.IsIssueId("LOR-CT-0000"), Is.False);

        [Test]
        public void ValidExecutionEvidencePasses()
        {
            Assert.That(ClosedTestExecutionValidation.Validate(ReadyExecution(), HashA).Valid, Is.True);
        }

        [Test]
        public void ExecutionEvidenceRejectsArtifactMismatch()
        {
            Assert.That(ClosedTestExecutionValidation.Validate(ReadyExecution(), HashB).Valid, Is.False);
        }

        [Test]
        public void ExecutionEvidenceRequiresTesterAvailability()
        {
            var e = ReadyExecution(); e.ClosedTestReleaseAvailableToTesters = false;
            Assert.That(ClosedTestExecutionValidation.Validate(e, HashA).Valid, Is.False);
        }

        [Test]
        public void ExecutionEvidenceRequiresFeedbackTriage()
        {
            var e = ReadyExecution(); e.FeedbackTriageComplete = false;
            Assert.That(ClosedTestExecutionValidation.Validate(e, HashA).Valid, Is.False);
        }

        [Test]
        public void ExecutionEvidenceRejectsBlockingCrashOrAnr()
        {
            var e = ReadyExecution(); e.BlockingCrashOrAnrOpen = true;
            Assert.That(ClosedTestExecutionValidation.Validate(e, HashA).Valid, Is.False);
        }

        [Test]
        public void FeedbackTriageAcceptsReviewedFixedRetestedItems()
        {
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { FixedItem("LOR-CT-0001") }).ReadyForProductionAccessReview, Is.True);
        }

        [Test]
        public void FeedbackTriageRequiresAtLeastOneItem()
        {
            Assert.That(FeedbackTriageEvaluator.Evaluate(new ClosedTestFeedbackItem[0]).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRejectsDuplicateIssueIds()
        {
            var a = FixedItem("LOR-CT-0001"); var b = FixedItem("LOR-CT-0001");
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { a, b }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRejectsUnreviewedItem()
        {
            var item = FixedItem("LOR-CT-0001"); item.Reviewed = false;
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRejectsOpenCriticalItem()
        {
            var item = FixedItem("LOR-CT-0001"); item.Disposition = FeedbackDisposition.Open; item.Severity = FeedbackSeverity.Critical;
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRejectsOpenHighItem()
        {
            var item = FixedItem("LOR-CT-0001"); item.Disposition = FeedbackDisposition.Open; item.Severity = FeedbackSeverity.High;
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRejectsBlockingOpenItem()
        {
            var item = FixedItem("LOR-CT-0001"); item.Disposition = FeedbackDisposition.Open; item.BlocksProduction = true;
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackTriageRequiresRetestForFixedItemWhenRequested()
        {
            var item = FixedItem("LOR-CT-0001"); item.RetestPassed = false;
            Assert.That(FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview, Is.False);
        }

        [Test]
        public void FeedbackSummaryCountsDisposition()
        {
            var fixedItem = FixedItem("LOR-CT-0001");
            var deferred = FixedItem("LOR-CT-0002"); deferred.Disposition = FeedbackDisposition.Deferred; deferred.RetestRequired = false; deferred.RetestPassed = false;
            var r = FeedbackTriageEvaluator.Evaluate(new[] { fixedItem, deferred });
            Assert.That(r.FixedItems, Is.EqualTo(1)); Assert.That(r.DeferredItems, Is.EqualTo(1));
        }

        [Test]
        public void ProductionAccessPreparationPassesForOrganizationStyleAccountRule()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.True);
        }

        [Test]
        public void ProductionAccessPreparationRequiresAccountRuleReview()
        {
            var e = ReadyProduction(); e.DeveloperAccountRuleReviewed = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void NewPersonalAccountRequiresTwelveContinuousTesters()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = true; e.ContinuousOptedInTesterCount = 11;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void NewPersonalAccountRequiresFourteenContinuousDays()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = true; e.ContinuousTestDays = 13;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void ProductionAccessPreparationRequiresTestingSummary()
        {
            var e = ReadyProduction(); e.TestingProcessSummaryReviewed = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void ProductionAccessPreparationRequiresFeedbackSummary()
        {
            var e = ReadyProduction(); e.TesterFeedbackSummaryReviewed = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void ProductionAccessPreparationRequiresChangesSummary()
        {
            var e = ReadyProduction(); e.ChangesMadeSummaryReviewed = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void ProductionAccessPreparationRequiresReadinessSummary()
        {
            var e = ReadyProduction(); e.ProductionReadinessSummaryReviewed = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayPrepareApplication, Is.False);
        }

        [Test]
        public void ProductionAccessSubmissionRequiresExplicitAuthorization()
        {
            var e = ReadyProduction(); e.ProductionAccessApplicationAuthorized = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MaySubmitApplication, Is.False);
        }

        [Test]
        public void ProductionAdvanceRequiresRetainedDecision()
        {
            var e = ReadyProduction(); e.ProductionAccessDecisionRetained = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayAdvanceToProductionReleaseReadiness, Is.False);
        }

        [Test]
        public void ProductionAdvanceRequiresGrantedDecision()
        {
            var e = ReadyProduction(); e.ProductionAccessGranted = false;
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(e).MayAdvanceToProductionReleaseReadiness, Is.False);
        }

        [Test]
        public void FullySatisfiedProductionEvidenceCanAdvance()
        {
            Assert.That(ProductionAccessEvidenceEvaluator.Evaluate(ReadyProduction()).MayAdvanceToProductionReleaseReadiness, Is.True);
        }

        static ClosedTestExecutionEvidence ReadyExecution()
        {
            return new ClosedTestExecutionEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, ClosedTestReleaseAvailableToTesters = true,
                ObservationDays = 14, OptedInTesterCount = 12, ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14,
                HealthEvidenceValid = true, DeviceEvidenceValid = true, FeedbackEvidenceRetained = true, FeedbackTriageComplete = true,
                BlockingCrashOrAnrOpen = false, BlockingVisualOrCoreLoopIssueOpen = false
            };
        }

        static ClosedTestFeedbackItem FixedItem(string id)
        {
            return new ClosedTestFeedbackItem
            {
                IssueId = id, Category = FeedbackCategory.Gameplay, Severity = FeedbackSeverity.Medium,
                Disposition = FeedbackDisposition.Fixed, Reviewed = true, Reproduced = true, BlocksProduction = false,
                RetestRequired = true, RetestPassed = true
            };
        }

        static ProductionAccessEvidence ReadyProduction()
        {
            return new ProductionAccessEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA,
                DeveloperAccountRuleReviewed = true, PersonalAccountCreatedAfter2023November13 = true,
                ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14, ClosedTestExecutionEvidenceValid = true,
                FeedbackTriageReady = true, TestingProcessSummaryReviewed = true, TesterFeedbackSummaryReviewed = true,
                ChangesMadeSummaryReviewed = true, ProductionReadinessSummaryReviewed = true,
                ProductionAccessApplicationAuthorized = true, ProductionAccessDecisionRetained = true, ProductionAccessGranted = true
            };
        }
    }
}
