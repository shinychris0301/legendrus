using LegendsOfRus.LaunchCandidate;
using NUnit.Framework;

namespace LegendsOfRus.Tests.LaunchCandidate
{
    public sealed class ProductionAccessDecisionLaunchCandidateEditModeTests
    {
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [Test] public void Sha256AcceptsHex() => Assert.That(LaunchCandidateValidation.IsSha256(HashA), Is.True);
        [Test] public void Sha256RejectsShort() => Assert.That(LaunchCandidateValidation.IsSha256("abc"), Is.False);
        [Test] public void CandidateIdAcceptsRc01() => Assert.That(LaunchCandidateValidation.IsCandidateId("v1.18-step05-rc01"), Is.True);
        [Test] public void CandidateIdRejectsRc00() => Assert.That(LaunchCandidateValidation.IsCandidateId("v1.18-step05-rc00"), Is.False);

        [Test]
        public void ProductionDecisionRequiresAccountRuleReview()
        {
            var e = ReadyDecision(); e.DeveloperAccountRuleReviewed = false;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void ProductionDecisionRequiresRequirementResolution()
        {
            var e = ReadyDecision(); e.ProductionAccessRequirementResolved = false;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void RequiredDecisionNeedsRetainedEvidence()
        {
            var e = ReadyDecision(); e.DecisionEvidenceRetained = false;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void RequiredDecisionNeedsReview()
        {
            var e = ReadyDecision(); e.DecisionReviewed = false;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void RequiredDecisionNeedsGrantedStatus()
        {
            var e = ReadyDecision(); e.DecisionStatus = ProductionAccessDecisionStatus.AdditionalTestingRequired;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void RequiredGrantedDecisionPasses()
        {
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(ReadyDecision()).Satisfied, Is.True);
        }

        [Test]
        public void NotRequiredDecisionPassesOnlyWithExplicitNotRequiredStatus()
        {
            var e = ReadyDecision(); e.ProductionAccessDecisionRequired = false; e.DecisionEvidenceRetained = false; e.DecisionReviewed = false; e.DecisionStatus = ProductionAccessDecisionStatus.NotRequired;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.True);
        }

        [Test]
        public void NotRequiredDecisionRejectsUnknownStatus()
        {
            var e = ReadyDecision(); e.ProductionAccessDecisionRequired = false; e.DecisionEvidenceRetained = false; e.DecisionReviewed = false; e.DecisionStatus = ProductionAccessDecisionStatus.Unknown;
            Assert.That(ProductionAccessDecisionEvaluator.Evaluate(e).Satisfied, Is.False);
        }

        [Test]
        public void FullyReadyLaunchCandidatePassesHumanReview()
        {
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(ReadyLaunch()).ReadyForHumanLaunchReview, Is.True);
        }

        [Test]
        public void LaunchCandidateRequiresValidCandidateId()
        {
            var e = ReadyLaunch(); e.CandidateId = "bad";
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresArtifactHash()
        {
            var e = ReadyLaunch(); e.ArtifactSha256 = "bad";
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresFreezeHash()
        {
            var e = ReadyLaunch(); e.CandidateFreezeManifestSha256 = "bad";
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresVerifiedFreeze()
        {
            var e = ReadyLaunch(); e.CandidateFreezeVerified = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresArtifactFreezeMatch()
        {
            var e = ReadyLaunch(); e.ArtifactMatchesFreeze = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresClosedTestEvidence()
        {
            var e = ReadyLaunch(); e.ClosedTestExecutionEvidenceValid = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresFeedbackTriage()
        {
            var e = ReadyLaunch(); e.FeedbackTriageReady = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresProductionAccessDecision()
        {
            var e = ReadyLaunch(); e.ProductionAccessDecisionSatisfied = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresStoreChecklist()
        {
            var e = ReadyLaunch(); e.StoreReleaseChecklistComplete = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresPrivacyAndDataSafety()
        {
            var e = ReadyLaunch(); e.PrivacyAndDataSafetyApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresLegalDocuments()
        {
            var e = ReadyLaunch(); e.LegalDocumentsApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresAppContentDeclarations()
        {
            var e = ReadyLaunch(); e.AppContentDeclarationsApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresProductionIdentityAndVersioning()
        {
            var e = ReadyLaunch(); e.ProductionApplicationIdApproved = false; e.VersioningApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresSigningWorkflow()
        {
            var e = ReadyLaunch(); e.PlayAppSigningConfigured = false; e.UploadKeyWorkflowReviewed = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresApi36()
        {
            var e = ReadyLaunch(); e.TargetApiLevel = 35;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresAabArm64And16KbEvidence()
        {
            var e = ReadyLaunch(); e.AppBundleVerified = false; e.Arm64Verified = false; e.PageSize16KbVerified = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresReleaseNotesAndCountries()
        {
            var e = ReadyLaunch(); e.ReleaseNotesApproved = false; e.ProductionCountriesRegionsApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresSupportMonitoringAndRollback()
        {
            var e = ReadyLaunch(); e.SupportReadinessApproved = false; e.LaunchMonitoringApproved = false; e.RollbackPlanApproved = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRequiresReleaseHistoryReview()
        {
            var e = ReadyLaunch(); e.ReleaseHistoryReviewed = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void LaunchCandidateRejectsOutstandingRelease()
        {
            var e = ReadyLaunch(); e.OutstandingReleasesCleared = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void FirstProductionReleaseRequiresOneHundredPercent()
        {
            var e = ReadyLaunch(); e.FirstProductionRelease = true; e.RequestedRolloutPercent = 20;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void UpdateAllowsValidStagedRolloutPercentage()
        {
            var e = ReadyLaunch(); e.FirstProductionRelease = false; e.RequestedRolloutPercent = 10;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.True);
        }

        [Test]
        public void UpdateRejectsZeroRolloutPercentage()
        {
            var e = ReadyLaunch(); e.FirstProductionRelease = false; e.RequestedRolloutPercent = 0;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).ReadyForHumanLaunchReview, Is.False);
        }

        [Test]
        public void ManualRolloutRequiresExplicitProductionAuthorization()
        {
            var e = ReadyLaunch(); e.ProductionReleaseAuthorized = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).MayStartManualProductionRollout, Is.False);
        }

        [Test]
        public void ManualRolloutRequiresExplicitPublishingAuthorization()
        {
            var e = ReadyLaunch(); e.StorePublishingAuthorized = false;
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(e).MayStartManualProductionRollout, Is.False);
        }

        [Test]
        public void FullyAuthorizedEvidenceMayStartManualRollout()
        {
            Assert.That(LaunchCandidateReadinessEvaluator.Evaluate(ReadyLaunch()).MayStartManualProductionRollout, Is.True);
        }

        static ProductionAccessDecisionEvidence ReadyDecision()
        {
            return new ProductionAccessDecisionEvidence
            {
                DeveloperAccountRuleReviewed = true,
                ProductionAccessRequirementResolved = true,
                ProductionAccessDecisionRequired = true,
                DecisionEvidenceRetained = true,
                DecisionReviewed = true,
                DecisionStatus = ProductionAccessDecisionStatus.Granted
            };
        }

        static LaunchCandidateEvidence ReadyLaunch()
        {
            return new LaunchCandidateEvidence
            {
                CandidateId = "v1.18-step05-rc01",
                ArtifactSha256 = HashA,
                CandidateFreezeManifestSha256 = HashB,
                CandidateFreezeManifestRetained = true,
                CandidateFreezeVerified = true,
                ArtifactMatchesFreeze = true,
                ClosedTestExecutionEvidenceValid = true,
                FeedbackTriageReady = true,
                ProductionAccessDecisionSatisfied = true,
                StoreReleaseChecklistComplete = true,
                PrivacyAndDataSafetyApproved = true,
                LegalDocumentsApproved = true,
                AppContentDeclarationsApproved = true,
                ProductionApplicationIdApproved = true,
                VersioningApproved = true,
                PlayAppSigningConfigured = true,
                UploadKeyWorkflowReviewed = true,
                TargetApiLevel = 36,
                AppBundleVerified = true,
                Arm64Verified = true,
                PageSize16KbVerified = true,
                ReleaseNotesApproved = true,
                ProductionCountriesRegionsApproved = true,
                SupportReadinessApproved = true,
                LaunchMonitoringApproved = true,
                RollbackPlanApproved = true,
                ReleaseHistoryReviewed = true,
                OutstandingReleasesCleared = true,
                FirstProductionRelease = true,
                RequestedRolloutPercent = 100,
                ProductionReleaseAuthorized = true,
                StorePublishingAuthorized = true
            };
        }
    }
}
