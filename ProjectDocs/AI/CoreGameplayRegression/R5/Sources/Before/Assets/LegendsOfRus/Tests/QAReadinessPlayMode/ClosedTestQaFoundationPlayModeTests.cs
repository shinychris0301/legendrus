using System.Collections;
using LegendsOfRus.QA;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.QAReadinessPlayMode
{
    public sealed class ClosedTestQaFoundationPlayModeTests
    {
        [SetUp]
        public void SetUp()
        {
            DestroyTestService();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyTestService();
        }

        [UnityTest]
        public IEnumerator Activation_IsLocalOnlyAndProductionBlocked()
        {
            ClosedTestQaService service = CreateService();
            yield return null;

            Assert.That(service.Activation.LocalDeviceProfileCaptureApproved, Is.True);
            Assert.That(service.Activation.LocalObservationBufferApproved, Is.True);
            Assert.That(service.Activation.RemoteQaUploadApproved, Is.False);
            Assert.That(service.Activation.ProductionQaProviderApproved, Is.False);
            Assert.That(service.Activation.ProductionSigningApproved, Is.False);
            Assert.That(service.Activation.StorePublishingAuthorized, Is.False);
            Assert.That(ClosedTestQaActivation.RemoteQaTransportCompiled, Is.False);
            Assert.That(ClosedTestQaActivation.StorePublishingCompiled, Is.False);
        }

        [UnityTest]
        public IEnumerator Service_CapturesOnlyCoarseDeviceProfile()
        {
            ClosedTestQaService service = CreateService();
            yield return null;

            Assert.That(service.TryCaptureCurrentDevice(out DeviceProfileSnapshot snapshot), Is.True);
            Assert.That(snapshot.ScreenWidth, Is.GreaterThan(0));
            Assert.That(snapshot.ScreenHeight, Is.GreaterThan(0));
            Assert.That(snapshot.AspectPermille, Is.GreaterThan(0));
        }

        [UnityTest]
        public IEnumerator Service_ObservationBufferStartsWithBootAndRemainsBounded()
        {
            ClosedTestQaService service = CreateService();
            yield return null;

            Assert.That(service.ObservationCount, Is.EqualTo(1));
            for (int i = 0; i < 100; i++)
                service.TryRecord(ClosedTestObservationCode.LevelStarted, i + 1);
            ClosedTestObservation[] snapshot = service.CopyObservations();
            Assert.That(snapshot.Length, Is.EqualTo(ClosedTestObservationBuffer.DefaultCapacity));
            Assert.That(snapshot[snapshot.Length - 1].LevelNumber, Is.EqualTo(100));
        }

        [UnityTest]
        public IEnumerator Service_DoesNotPersistObservationBufferAcrossRestart()
        {
            ClosedTestQaService first = CreateService();
            yield return null;
            first.TryRecord(ClosedTestObservationCode.LevelWon, 1);
            Assert.That(first.ObservationCount, Is.EqualTo(2));

            Object.Destroy(first.gameObject);
            yield return null;

            ClosedTestQaService second = CreateService();
            yield return null;
            Assert.That(second.ObservationCount, Is.EqualTo(1));
        }

        static ClosedTestQaService CreateService()
        {
            var host = new GameObject("ClosedTestQaService_Test");
            return host.AddComponent<ClosedTestQaService>();
        }

        static void DestroyTestService()
        {
            for (int i = 0; i < 8; i++)
            {
                ClosedTestQaService existing =
                    Object.FindAnyObjectByType<ClosedTestQaService>();
                if (existing == null)
                    return;
                if (existing.gameObject.name == "ClosedTestQaService_Test")
                    Object.DestroyImmediate(existing.gameObject);
                else
                    Object.DestroyImmediate(existing);
            }
        }
    }
}
