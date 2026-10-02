using System.Collections;
using LegendsOfRus.BalanceReview;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests
{
    public sealed class BalanceTelemetryTutorialFunnelPlayModeTests
    {
        GameObject host;
        BalanceTelemetryReviewService service;

        [SetUp]
        public void SetUp()
        {
            if (BalanceTelemetryReviewService.Instance != null)
                Object.DestroyImmediate(BalanceTelemetryReviewService.Instance.gameObject);
            host = new GameObject("BalanceReviewTestHost");
            service = host.AddComponent<BalanceTelemetryReviewService>();
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null)
                Object.DestroyImmediate(host);
        }

        [Test]
        public void LocalObservationAndReviewAreApproved()
        {
            Assert.That(service.Activation.LocalObservationBufferApproved, Is.True);
            Assert.That(service.Activation.LocalReviewReportApproved, Is.True);
        }

        [Test]
        public void RemoteAndMutationPathsRemainBlocked()
        {
            BalanceTelemetryReviewActivationSnapshot value = service.Activation;
            Assert.That(value.RemoteTelemetryUploadApproved, Is.False);
            Assert.That(value.ProductionAnalyticsBridgeApproved, Is.False);
            Assert.That(value.AutomaticBalanceMutationApproved, Is.False);
            Assert.That(value.TutorialExperimentMutationApproved, Is.False);
        }

        [Test]
        public void ServiceRecordsLevelLifecycle()
        {
            LevelData level = NewLevel(1);
            Assert.That(service.TryRecordLevelStart(level, 1), Is.True);
            Assert.That(service.TryRecordLevelWin(level, 1, 4, 1500), Is.True);
            Assert.That(service.ObservationCount, Is.EqualTo(2));
            Assert.That(service.BuildLocalReview().Levels.Length, Is.EqualTo(1));
        }

        [Test]
        public void ServiceRecordsTutorialLifecycleWithoutRawIdInCoreRecord()
        {
            LevelData level = NewLevel(2);
            Assert.That(service.TryRecordTutorialShown(level, "tutorial.ice.first"), Is.True);
            Assert.That(service.TryRecordTutorialCompleted(level, "tutorial.ice.first"), Is.True);
            BalanceObservation[] observations = service.CopyObservations();
            Assert.That(observations.Length, Is.EqualTo(2));
            Assert.That(observations[0].TutorialKey, Is.Not.EqualTo(0));
            Assert.That(observations[0].TutorialKey, Is.EqualTo(observations[1].TutorialKey));
        }

        [UnityTest]
        public IEnumerator DuplicateServiceDestroysItself()
        {
            var duplicate = new GameObject("BalanceReviewDuplicate");
            duplicate.AddComponent<BalanceTelemetryReviewService>();
            yield return null;
            Assert.That(BalanceTelemetryReviewService.Instance, Is.SameAs(service));
            Assert.That(duplicate == null, Is.True);
        }

        [UnityTest]
        public IEnumerator BufferRemainsProcessLocalAcrossFrame()
        {
            LevelData level = NewLevel(3);
            service.TryRecordLevelStart(level, 1);
            yield return null;
            Assert.That(service.ObservationCount, Is.EqualTo(1));
        }

        static LevelData NewLevel(int levelNumber)
        {
            return new LevelData
            {
                id = "chapter01.level" + levelNumber,
                chapterId = 1,
                levelId = levelNumber,
                moveLimit = 25
            };
        }
    }
}
