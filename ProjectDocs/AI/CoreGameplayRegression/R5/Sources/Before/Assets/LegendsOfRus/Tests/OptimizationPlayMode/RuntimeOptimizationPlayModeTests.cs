using System.Collections;
using LegendsOfRus.OptimizationCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.Optimization.PlayMode
{
    [TestFixture]
    public sealed class RuntimeOptimizationPlayModeTests
    {
        static readonly string[] SaveKeys =
        {
            "LegendsOfRus_Save_v1",
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        bool[] previousPresence;
        string[] previousValues;
        string[] sentinels;
        GameObject ownedServiceHost;

        [SetUp]
        public void SetUp()
        {
            previousPresence = new bool[SaveKeys.Length];
            previousValues = new string[SaveKeys.Length];
            sentinels = new string[SaveKeys.Length];
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                previousPresence[i] = PlayerPrefs.HasKey(SaveKeys[i]);
                previousValues[i] = previousPresence[i]
                    ? PlayerPrefs.GetString(SaveKeys[i])
                    : string.Empty;
                sentinels[i] = "optimization-playmode-sentinel-" + i;
                PlayerPrefs.SetString(SaveKeys[i], sentinels[i]);
            }
            PlayerPrefs.Save();
        }

        [TearDown]
        public void TearDown()
        {
            if (ownedServiceHost != null)
            {
                Object.DestroyImmediate(ownedServiceHost);
                ownedServiceHost = null;
            }

            for (int i = 0; i < SaveKeys.Length; i++)
            {
                PlayerPrefs.DeleteKey(SaveKeys[i]);
                if (previousPresence[i])
                    PlayerPrefs.SetString(SaveKeys[i], previousValues[i]);
            }
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator RuntimeBootstrap_ProvidesSingleOptimizationService()
        {
            yield return null;
            RuntimeOptimizationService service = RuntimeOptimizationService.Instance;
            if (service == null)
            {
                ownedServiceHost = new GameObject("OptimizationService_PlayModeTest");
                service = ownedServiceHost.AddComponent<RuntimeOptimizationService>();
                yield return null;
            }

            Assert.That(service, Is.Not.Null);

            var duplicateHost = new GameObject("OptimizationDuplicate_PlayModeTest");
            duplicateHost.AddComponent<RuntimeOptimizationService>();
            yield return null;
            Assert.That(RuntimeOptimizationService.Instance, Is.SameAs(service));
            Object.Destroy(duplicateHost);
            AssertSaveSentinels();
        }

        [Test]
        public void CoreEvidence_StaysBoundedDuringManyRouteSamples()
        {
            var buffer = new BoundedOptimizationEvidenceBuffer();
            for (int i = 0; i < 500; i++)
                buffer.AddLoad(OptimizationLoadKind.ScreenRoute, i % 300);

            Assert.That(buffer.Count,
                Is.EqualTo(OptimizationPolicy.DefaultEvidenceCapacity));
            Assert.That(buffer.Snapshot()[0].Sequence,
                Is.EqualTo(500 - OptimizationPolicy.DefaultEvidenceCapacity + 1));
            AssertSaveSentinels();
        }

        [UnityTest]
        public IEnumerator RuntimeService_LoadingStateCompletesWithoutPersistence()
        {
            yield return null;
            RuntimeOptimizationService service = RuntimeOptimizationService.Instance;
            if (service == null)
            {
                ownedServiceHost = new GameObject("OptimizationLoading_PlayModeTest");
                service = ownedServiceHost.AddComponent<RuntimeOptimizationService>();
                yield return null;
            }

            if (service.LoadingPhase == RuntimeLoadingPhase.NotStarted)
            {
                service.BeginBootstrap();
                service.MarkLocalStateReady();
                service.MarkUiConstructionReady();
                service.CompleteBootstrap();
            }

            Assert.That(service.LoadingPhase,
                Is.EqualTo(RuntimeLoadingPhase.Ready));
            Assert.That(service.LoadingProgress01, Is.EqualTo(1.0));
            AssertSaveSentinels();
        }

        [Test]
        public void OptimizationCore_ContainsNoPersistentOrNetworkState()
        {
            var buffer = new BoundedOptimizationEvidenceBuffer(4);
            buffer.AddMemory(100, 200, 300);
            buffer.AddLowMemory(100, 200, 300);
            string json = buffer.ToDeterministicJson();

            Assert.That(json, Does.Not.Contain("player"));
            Assert.That(json, Does.Not.Contain("device"));
            Assert.That(json, Does.Not.Contain("url"));
            Assert.That(json, Does.Not.Contain("token"));
            Assert.That(json, Does.Not.Contain("save"));
            AssertSaveSentinels();
        }

        void AssertSaveSentinels()
        {
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                Assert.That(PlayerPrefs.HasKey(SaveKeys[i]), Is.True);
                Assert.That(PlayerPrefs.GetString(SaveKeys[i]),
                    Is.EqualTo(sentinels[i]));
            }
        }
    }
}
