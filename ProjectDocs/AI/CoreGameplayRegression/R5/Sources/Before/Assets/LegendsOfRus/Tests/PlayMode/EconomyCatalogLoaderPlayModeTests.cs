using System;
using LegendsOfRus.EconomyCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class EconomyCatalogLoaderPlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        [Test]
        public void Loader_LoadsValidatedProposalWithoutRuntimeAuthority()
        {
            EconomyCatalogLoadResult result = EconomyCatalogLoader.LoadProposal();

            Assert.That(result.Succeeded, Is.True, result.Error);
            Assert.That(result.ErrorCode, Is.Empty);
            Assert.That(result.SourceJson, Is.Not.Empty);
            Assert.That(result.Catalog, Is.Not.Null);
            Assert.That(result.Catalog.schemaVersion, Is.EqualTo(EconomyCatalogValidator.CurrentSchemaVersion));
            Assert.That(result.Catalog.catalogId, Is.EqualTo("v1.15-step01-proposal"));
            Assert.That(result.Catalog.proposalOnly, Is.True);
            Assert.That(result.Catalog.authoritative, Is.False);
            Assert.That(result.Validation.verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(result.Validation.violations, Is.Empty);
        }

        [Test]
        public void LoaderSuccess_RevalidatesMutableCatalogAndFailsClosedAfterMutation()
        {
            EconomyCatalogLoadResult result = EconomyCatalogLoader.LoadProposal();
            Assert.That(result.Succeeded, Is.True, result.Error);

            result.Catalog.authoritative = true;

            Assert.That(result.Succeeded, Is.False);
            EconomyValidationReport validation = EconomyCatalogValidator.Validate(result.Catalog);
            Assert.That(validation.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(
                Array.Exists(
                    validation.violations,
                    item => item.code == EconomyViolationCodes.AuthoritativeForbidden
                ),
                Is.True
            );
        }

        [Test]
        public void Coordinator_ProducesDeterministicNonAuthoritativeIncompleteComparison()
        {
            EconomySimulationCoordinatorResult first =
                EconomySimulationCoordinator.LoadAndRunProposal();
            EconomySimulationCoordinatorResult second =
                EconomySimulationCoordinator.LoadAndRunProposal();

            Assert.That(first.LoadSucceeded, Is.True, first.Error);
            Assert.That(first.Simulation.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
            Assert.That(first.PrototypeComparison.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
            Assert.That(
                Array.Exists(
                    first.PrototypeComparison.entries,
                    item => item.path == "continue.price" &&
                        item.status == EconomyComparisonStatus.Dormant
                ),
                Is.True
            );
            string firstJson = EconomySimulationCoordinator.ToDeterministicJson(first);
            string secondJson = EconomySimulationCoordinator.ToDeterministicJson(second);
            Assert.That(firstJson, Is.EqualTo(secondJson));
            Assert.That(firstJson, Does.Contain("\"nonAuthoritative\":true"));
            Assert.That(firstJson, Does.Contain("\"authority\":\"proposal-comparison-only\""));
        }

        [Test]
        public void LoaderValidationAndSimulation_DoNotMutatePlayerSaveOrPlayerPrefs()
        {
            var hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            var originalPersistedSaves = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                originalPersistedSaves[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();
            GameState liveState = GameState.Instance;
            string liveSaveBefore = liveState == null
                ? string.Empty
                : JsonUtility.ToJson(liveState.Save);

            var detachedSave = new PlayerSaveData
            {
                saveVersion = 6,
                gold = 4321,
                gems = 87,
                energy = 2,
                energyMax = 5,
                lastEnergyUtcTicks = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc).Ticks,
                chapter = 1,
                level = 4,
                selectedChapter = 1,
                selectedLevel = 4,
                highestUnlockedLevel = 4,
                boosterMace = 3,
                boosterFirebird = 2,
                boosterLivingLight = 1,
                boosterSeaPearl = 0,
                lastDailyRewardUtc = "20260831",
                music = false,
                sfx = true,
                vibration = false,
                notifications = true,
            };
            string detachedBefore = JsonUtility.ToJson(detachedSave);

            try
            {
                PlayerPrefs.SetString(PersistedSaveKey, detachedBefore);
                PlayerPrefs.Save();
                string rawBefore = PlayerPrefs.GetString(PersistedSaveKey);

                EconomyCatalogLoadResult load = EconomyCatalogLoader.LoadProposal();
                Assert.That(load.Succeeded, Is.True, load.Error);
                EconomyValidationReport validation = EconomyCatalogValidator.Validate(load.Catalog);
                EconomySimulationCoordinatorResult coordinator =
                    EconomySimulationCoordinator.LoadAndRunProposal();

                Assert.That(validation.verdict, Is.EqualTo(EconomyVerdict.Pass));
                Assert.That(coordinator.Simulation.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
                Assert.That(coordinator.PrototypeComparison.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
                Assert.That(JsonUtility.ToJson(detachedSave), Is.EqualTo(detachedBefore));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKey), Is.EqualTo(rawBefore));

                PlayerSaveData reloaded = SaveService.Load();
                Assert.That(JsonUtility.ToJson(reloaded), Is.EqualTo(detachedBefore));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKey), Is.EqualTo(rawBefore));
                if (liveState != null)
                    Assert.That(JsonUtility.ToJson(liveState.Save), Is.EqualTo(liveSaveBefore));
            }
            finally
            {
                for (int i = 0; i < PersistedSaveKeys.Length; i++)
                {
                    string key = PersistedSaveKeys[i];
                    PlayerPrefs.DeleteKey(key);
                    if (hadPersistedSaves[i])
                        PlayerPrefs.SetString(key, originalPersistedSaves[i]);
                }
                PlayerPrefs.Save();
            }
        }
    }
}
