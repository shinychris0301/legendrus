using System.Collections;
using System.Reflection;
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
        BalanceObservationBuffer previousObservations;
        ScreenRouter previousRouter;
        GameState previousState;
        static readonly FieldInfo Observations = typeof(BalanceTelemetryReviewService)
            .GetField("observations", BindingFlags.Instance | BindingFlags.NonPublic);

        [SetUp]
        public void SetUp()
        {
            previousRouter = ScreenRouter.Instance;
            previousState = GameState.Instance;
            service = BalanceTelemetryReviewService.Instance;
            if (service == null)
            {
                host = new GameObject("BalanceReviewTestHost");
                service = host.AddComponent<BalanceTelemetryReviewService>();
            }
            else
            {
                // Keep the shared root and every other service alive. Swap only this
                // fixture's transient buffer and restore the exact original object.
                Assert.That(Observations, Is.Not.Null);
                previousObservations = (BalanceObservationBuffer)Observations.GetValue(service);
                Observations.SetValue(service, new BalanceObservationBuffer());
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null)
            {
                Object.DestroyImmediate(host);
                host = null;
            }
            else if (service != null)
                Observations.SetValue(service, previousObservations);
            Assert.That(ScreenRouter.Instance, Is.SameAs(previousRouter));
            Assert.That(GameState.Instance, Is.SameAs(previousState));
            service = null;
            previousObservations = null;
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
