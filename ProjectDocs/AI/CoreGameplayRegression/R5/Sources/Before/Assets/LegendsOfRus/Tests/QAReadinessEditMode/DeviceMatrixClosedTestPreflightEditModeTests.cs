using System;
using System.Linq;
using LegendsOfRus.QA;
using NUnit.Framework;

namespace LegendsOfRus.Tests.QAReadiness
{
    public sealed class DeviceMatrixClosedTestPreflightEditModeTests
    {
        [Test]
        public void Classifier_RejectsBelowMinimumMemory()
        {
            var input = new DeviceProfileInput(1536, 512, 4, 4096, 720, 1280);
            Assert.That(DeviceProfileClassifier.ClassifyCapability(input),
                Is.EqualTo(DeviceCapabilityBand.BelowMinimum));
        }

        [Test]
        public void Classifier_AssignsLowMidAndHighBands()
        {
            Assert.That(DeviceProfileClassifier.ClassifyCapability(
                new DeviceProfileInput(3072, 512, 4, 4096, 720, 1280)),
                Is.EqualTo(DeviceCapabilityBand.Low));
            Assert.That(DeviceProfileClassifier.ClassifyCapability(
                new DeviceProfileInput(4096, 768, 6, 4096, 1080, 2160)),
                Is.EqualTo(DeviceCapabilityBand.Mid));
            Assert.That(DeviceProfileClassifier.ClassifyCapability(
                new DeviceProfileInput(6144, 1024, 8, 8192, 1080, 2400)),
                Is.EqualTo(DeviceCapabilityBand.High));
        }

        [TestCase(720, 1280, DeviceAspectBucket.Standard16By9)]
        [TestCase(1080, 2160, DeviceAspectBucket.Tall18By9)]
        [TestCase(1080, 2340, DeviceAspectBucket.Tall19_5By9)]
        [TestCase(1080, 2400, DeviceAspectBucket.Tall20By9)]
        public void Classifier_AssignsRequiredAspectBuckets(
            int width, int height, DeviceAspectBucket expected)
        {
            int permille = DeviceProfileClassifier.AspectPermille(width, height);
            Assert.That(DeviceProfileClassifier.ClassifyAspect(permille), Is.EqualTo(expected));
        }

        [Test]
        public void Matrix_ValidatesAllRequiredProfiles()
        {
            Assert.That(DeviceMatrixCatalog.Validate(out string failure), Is.True, failure);
            Assert.That(DeviceMatrixCatalog.RequiredProfileCount, Is.GreaterThanOrEqualTo(5));
        }

        [Test]
        public void Matrix_CopyCannotMutateCatalog()
        {
            DeviceMatrixProfile[] first = DeviceMatrixCatalog.CopyRequiredProfiles();
            first[0].Id = "changed";
            DeviceMatrixProfile[] second = DeviceMatrixCatalog.CopyRequiredProfiles();
            Assert.That(second[0].Id, Is.Not.EqualTo("changed"));
        }

        [Test]
        public void ObservationBuffer_IsBoundedAndOrdered()
        {
            var buffer = new ClosedTestObservationBuffer(3);
            for (int i = 1; i <= 5; i++)
                Assert.That(buffer.TryRecord(ClosedTestObservationCode.LevelStarted, i), Is.True);
            ClosedTestObservation[] entries = buffer.Snapshot();
            Assert.That(entries.Length, Is.EqualTo(3));
            CollectionAssert.AreEqual(new[] {3,4,5}, entries.Select(x => x.LevelNumber).ToArray());
            Assert.That(entries[0].Sequence, Is.LessThan(entries[2].Sequence));
        }

        [Test]
        public void ObservationBuffer_RejectsInvalidCode()
        {
            var buffer = new ClosedTestObservationBuffer();
            Assert.That(buffer.TryRecord(ClosedTestObservationCode.None), Is.False);
            Assert.That(buffer.Count, Is.EqualTo(0));
        }

        [Test]
        public void ObservationBuffer_RejectsInvalidCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClosedTestObservationBuffer(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClosedTestObservationBuffer(257));
        }

        [Test]
        public void Preflight_DefaultEvidence_FailsClosed()
        {
            ClosedTestPreflightResult result = ClosedTestPreflightEvaluator.Evaluate(default);
            Assert.That(result.MayBuildControlledTest, Is.False);
            Assert.That(result.MayPublishClosedTest, Is.False);
            Assert.That(result.Blockers, Does.Contain(ClosedTestPreflightCodes.UnityCompileMissing));
            Assert.That(result.Blockers, Does.Contain(ClosedTestPreflightCodes.DeviceMatrixIncomplete));
            Assert.That(result.Blockers, Does.Contain(ClosedTestPreflightCodes.PublishingNotAuthorized));
        }

        [Test]
        public void Preflight_CompleteBuildEvidence_AllowsBuildButNotPublish()
        {
            var evidence = FullyReadyEvidence();
            evidence.PublishingAuthorized = false;
            ClosedTestPreflightResult result = ClosedTestPreflightEvaluator.Evaluate(evidence);
            Assert.That(result.MayBuildControlledTest, Is.True);
            Assert.That(result.MayPublishClosedTest, Is.False);
            CollectionAssert.AreEqual(
                new[] {ClosedTestPreflightCodes.PublishingNotAuthorized},
                result.Blockers);
        }

        [Test]
        public void Preflight_FullyAuthorizedEvidence_AllowsClosedTestPublish()
        {
            var evidence = FullyReadyEvidence();
            ClosedTestPreflightResult result = ClosedTestPreflightEvaluator.Evaluate(evidence);
            Assert.That(result.MayBuildControlledTest, Is.True);
            Assert.That(result.MayPublishClosedTest, Is.True);
            Assert.That(result.Blockers, Is.Empty);
            Assert.That(result.Warnings, Does.Contain(
                ClosedTestPreflightCodes.RemoteQaUploadDisabled));
        }

        static ClosedTestPreflightEvidence FullyReadyEvidence()
        {
            return new ClosedTestPreflightEvidence
            {
                SourceStaticVerificationPassed = true,
                UnityCompilePassed = true,
                EditorSelfTestPassed = true,
                EditModePassed = true,
                PlayModePassed = true,
                RequiredDeviceProfiles = DeviceMatrixCatalog.RequiredProfileCount,
                PassedDeviceProfiles = DeviceMatrixCatalog.RequiredProfileCount,
                AndroidDevelopmentBuildEvidenceRetained = true,
                PrivacyDataSafetyApproved = true,
                StoreListingAssetsApproved = true,
                ProductionSigningApproved = true,
                ControlledTestBuildAuthorized = true,
                PublishingAuthorized = true,
                RemoteQaUploadEnabled = false,
                ProductionCommercialActivationEnabled = false
            };
        }
    }
}
