using LegendsOfRus.ClosedTestRollout;
using NUnit.Framework;

namespace LegendsOfRus.Tests.ClosedTestRollout
{
    public sealed class ClosedTestRolloutCoreEditModeTests
    {
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [Test] public void Requirements_PersonalAccountTesterCount_IsTwelve() => Assert.That(ClosedTestRolloutRequirements.PersonalAccountProductionAccessTesterCount, Is.EqualTo(12));
        [Test] public void Requirements_PersonalAccountContinuousDays_IsFourteen() => Assert.That(ClosedTestRolloutRequirements.PersonalAccountProductionAccessContinuousDays, Is.EqualTo(14));
        [Test] public void Requirements_FirstProductionReleasePercent_IsOneHundred() => Assert.That(ClosedTestRolloutRequirements.FirstProductionReleasePercent, Is.EqualTo(100));
        [Test] public void Requirements_HashAlgorithm_IsSha256() => Assert.That(ClosedTestRolloutRequirements.HashAlgorithm, Is.EqualTo("SHA-256"));

        [Test]
        public void ClosedTestPrepare_BlockedWithoutFreeze()
        {
            var e = ReadyUpload(); e.CandidateFreezeManifestVerified = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).PrepareBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.FreezeManifestMissing));
        }

        [Test]
        public void ClosedTestPrepare_BlockedOnArtifactMismatch()
        {
            var e = ReadyUpload(); e.ArtifactHashMatchesFreeze = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).PrepareBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.ArtifactHashMismatch));
        }

        [Test]
        public void ClosedTestPrepare_BlockedWithoutTestingPermission()
        {
            var e = ReadyUpload(); e.TestingTrackReleasePermissionApproved = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).MayPrepareClosedTestRelease, Is.False);
        }

        [Test]
        public void ClosedTestPrepare_BlockedWithoutTesterAccess()
        {
            var e = ReadyUpload(); e.TesterAccessConfigured = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).PrepareBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.TesterAccessMissing));
        }

        [Test]
        public void ClosedTestPrepare_BlockedByOutstandingRelease()
        {
            var e = ReadyUpload(); e.NoOutstandingRelease = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).PrepareBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.OutstandingReleasePresent));
        }

        [Test]
        public void ClosedTestPrepare_PassesWithAllPreparationEvidence()
        {
            var e = ReadyUpload(); e.ClosedTestUploadAuthorized = false; e.ClosedTestPublishAuthorized = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).MayPrepareClosedTestRelease, Is.True);
        }

        [Test]
        public void ClosedTestRollout_RequiresSeparateUploadAuthorization()
        {
            var e = ReadyUpload(); e.ClosedTestUploadAuthorized = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).MayRolloutClosedTestRelease, Is.False);
        }

        [Test]
        public void ClosedTestRollout_RequiresSeparatePublishAuthorization()
        {
            var e = ReadyUpload(); e.ClosedTestPublishAuthorized = false;
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(e).RolloutBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.ClosedTestPublishNotAuthorized));
        }

        [Test]
        public void ClosedTestRollout_PassesOnlyWhenEveryGatePasses()
        {
            Assert.That(ClosedTestUploadReadinessEvaluator.Evaluate(ReadyUpload()).MayRolloutClosedTestRelease, Is.True);
        }

        [Test]
        public void UploadEvidence_RejectsArtifactHashMismatch()
        {
            var e = ValidUploadEvidence(); e.ArtifactSha256 = HashB;
            Assert.That(ClosedTestEvidenceValidator.ValidateUpload(e, HashA).Issues, Does.Contain(ClosedTestRolloutBlockerCodes.ArtifactHashMismatch));
        }

        [Test]
        public void UploadEvidence_RejectsRejectedRelease()
        {
            var e = ValidUploadEvidence(); e.ReleaseStatus = ClosedTestReleaseStatus.Rejected;
            Assert.That(ClosedTestEvidenceValidator.ValidateUpload(e, HashA).Valid, Is.False);
        }

        [Test]
        public void UploadEvidence_AcceptsAvailableRelease()
        {
            Assert.That(ClosedTestEvidenceValidator.ValidateUpload(ValidUploadEvidence(), HashA).Valid, Is.True);
        }

        [Test]
        public void HealthEvidence_RejectsArtifactHashMismatch()
        {
            var e = ValidHealth(); e.ArtifactSha256 = HashB;
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(e, HashA).Valid, Is.False);
        }

        [Test]
        public void HealthEvidence_RejectsBlockingCrash()
        {
            var e = ValidHealth(); e.BlockingCrashCount = 1;
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(e, HashA).Issues, Does.Contain(ClosedTestRolloutBlockerCodes.BlockingCrash));
        }

        [Test]
        public void HealthEvidence_RejectsBlockingAnr()
        {
            var e = ValidHealth(); e.BlockingAnrCount = 1;
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(e, HashA).Issues, Does.Contain(ClosedTestRolloutBlockerCodes.BlockingAnr));
        }

        [Test]
        public void HealthEvidence_RejectsOpenCriticalBug()
        {
            var e = ValidHealth(); e.OpenCriticalBugCount = 1;
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(e, HashA).Valid, Is.False);
        }

        [Test]
        public void HealthEvidence_RequiresReviewedFeedback()
        {
            var e = ValidHealth(); e.FeedbackReviewed = false;
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(e, HashA).Issues, Does.Contain(ClosedTestRolloutBlockerCodes.FeedbackNotReviewed));
        }

        [Test]
        public void HealthEvidence_AcceptsHealthyClosedTest()
        {
            Assert.That(ClosedTestEvidenceValidator.ValidateHealth(ValidHealth(), HashA).Valid, Is.True);
        }

        [Test]
        public void ProductionAccess_BlockedUntilAccountTypeReviewed()
        {
            var e = ReadyProduction(); e.DeveloperAccountTypeReviewed = false;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionAccessBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.AccountTypeNotReviewed));
        }

        [Test]
        public void NewPersonalAccount_RequiresTwelveContinuousTesters()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = true; e.ContinuousOptedInTesterCount = 11;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionAccessBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.PersonalTesterCountInsufficient));
        }

        [Test]
        public void NewPersonalAccount_RequiresFourteenContinuousDays()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = true; e.ContinuousTestDays = 13;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionAccessBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.PersonalTestDurationInsufficient));
        }

        [Test]
        public void NewPersonalAccount_MayApplyAfterTwelveByFourteen()
        {
            var e = ReadyProduction(); e.PersonalAccountCreatedAfter2023November13 = true; e.ProductionAccessGranted = false; e.ProductionRolloutAuthorized = false; e.StorePublishingAuthorized = false;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).MayApplyForProductionAccess, Is.True);
        }

        [Test]
        public void FirstProductionRelease_RejectsStagedRollout()
        {
            var e = ReadyProduction(); e.IsFirstProductionRelease = true; e.RequestedRolloutPercent = 10;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionRolloutBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.FirstReleaseStagedRolloutUnsupported));
        }

        [Test]
        public void FirstProductionRelease_AllowsOneHundredPercent()
        {
            var e = ReadyProduction(); e.IsFirstProductionRelease = true; e.RequestedRolloutPercent = 100;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).MayRolloutProduction, Is.True);
        }

        [Test]
        public void UpdateRelease_AllowsStagedTenPercent()
        {
            var e = ReadyProduction(); e.IsFirstProductionRelease = false; e.RequestedRolloutPercent = 10;
            var r = ProductionRolloutReadinessEvaluator.Evaluate(e);
            Assert.That(r.MayRolloutProduction && r.UsesStagedRollout, Is.True);
        }

        [Test]
        public void ProductionRollout_RequiresRollbackPlan()
        {
            var e = ReadyProduction(); e.RollbackPlanReviewed = false;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionRolloutBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.RollbackPlanMissing));
        }

        [Test]
        public void ProductionRollout_RequiresExplicitAuthorization()
        {
            var e = ReadyProduction(); e.ProductionRolloutAuthorized = false;
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(e).ProductionRolloutBlockers, Does.Contain(ClosedTestRolloutBlockerCodes.ProductionRolloutNotAuthorized));
        }

        [Test]
        public void ProductionRollout_PassesWhenAllRequiredEvidenceAndApprovalsPass()
        {
            Assert.That(ProductionRolloutReadinessEvaluator.Evaluate(ReadyProduction()).MayRolloutProduction, Is.True);
        }

        static ClosedTestUploadReadinessEvidence ReadyUpload()
        {
            return new ClosedTestUploadReadinessEvidence
            {
                CandidateFreezeManifestVerified = true,
                ArtifactHashMatchesFreeze = true,
                PlayAppSetupComplete = true,
                TestingTrackReleasePermissionApproved = true,
                ClosedTestTrackConfigured = true,
                TesterAccessConfigured = true,
                CountryAvailabilityReviewed = true,
                ReleaseNotesApproved = true,
                NoOutstandingRelease = true,
                ClosedTestUploadAuthorized = true,
                ClosedTestPublishAuthorized = true
            };
        }

        static ClosedTestUploadEvidence ValidUploadEvidence()
        {
            return new ClosedTestUploadEvidence
            {
                CandidateId = "v1.18-step05-rc01",
                FreezeManifestSha256 = HashB,
                ArtifactSha256 = HashA,
                VersionName = "1.18.0",
                VersionCode = 11806,
                ReleaseStatus = ClosedTestReleaseStatus.AvailableToTesters,
                ArtifactAcceptedByPlay = true,
                ClosedTestTrackConfigured = true,
                TesterAccessConfigured = true,
                CountryAvailabilityReviewed = true,
                ReleaseNotesRetained = true
            };
        }

        static ClosedTestHealthEvidence ValidHealth()
        {
            return new ClosedTestHealthEvidence
            {
                CandidateId = "v1.18-step05-rc01",
                ArtifactSha256 = HashA,
                ObservationDays = 14,
                OptedInTesterCount = 12,
                ContinuousOptedInTesterCount = 12,
                ContinuousTestDays = 14,
                BlockingCrashCount = 0,
                BlockingAnrCount = 0,
                OpenCriticalBugCount = 0,
                OpenBlockingFeedbackCount = 0,
                FeedbackReviewed = true,
                CoreLoopFeedbackReviewed = true
            };
        }

        static ProductionRolloutReadinessEvidence ReadyProduction()
        {
            return new ProductionRolloutReadinessEvidence
            {
                ClosedTestUploadEvidenceValid = true,
                ClosedTestHealthEvidenceValid = true,
                ClosedTestReleaseAvailableToTesters = true,
                ClosedTestGoNoGoAuthorized = true,
                DeveloperAccountTypeReviewed = true,
                PersonalAccountCreatedAfter2023November13 = false,
                ContinuousOptedInTesterCount = 12,
                ContinuousTestDays = 14,
                ProductionAccessGranted = true,
                StoreReleaseChecklistApproved = true,
                PrivacyDataSafetyApproved = true,
                ContentRatingApproved = true,
                TargetAudienceApproved = true,
                AdsDeclarationApproved = true,
                AppAccessDeclarationApproved = true,
                RollbackPlanReviewed = true,
                MonitoringPlanReviewed = true,
                ManagedPublishingReviewed = true,
                NoOutstandingRelease = true,
                IsFirstProductionRelease = true,
                RequestedRolloutPercent = 100,
                ProductionRolloutAuthorized = true,
                StorePublishingAuthorized = true
            };
        }
    }
}
