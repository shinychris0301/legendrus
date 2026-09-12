using System;
using LegendsOfRus.EconomyCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class RuntimeEconomyActivationGatePlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        [Test]
        public void ShippedManifest_IsInactiveWithVerificationIncompleteAndLegacyFallback()
        {
            TextAsset manifestAsset = Resources.Load<TextAsset>(
                RuntimeEconomyActivationGate.ManifestResourcePath
            );
            Assert.That(manifestAsset, Is.Not.Null, "The shipped activation manifest is missing.");

            RuntimeEconomyActivationManifestData shippedManifest =
                JsonUtility.FromJson<RuntimeEconomyActivationManifestData>(manifestAsset.text);
            Assert.That(shippedManifest, Is.Not.Null);
            Assert.That(shippedManifest.productApproval, Is.True);
            Assert.That(shippedManifest.compilationVerified, Is.False);
            Assert.That(shippedManifest.editorSelfTestsPassed, Is.False);
            Assert.That(shippedManifest.editModeTestsPassed, Is.False);
            Assert.That(shippedManifest.playModeTestsPassed, Is.False);
            Assert.That(shippedManifest.runtimeActivationApproved, Is.False);

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Load();

            Assert.That(
                result.Status,
                Is.EqualTo(RuntimeEconomyActivationStatus.VerificationIncomplete),
                result.Reason
            );
            Assert.That(result.ReasonCode, Is.EqualTo("RUNTIME_ECON_COMPILATION_NOT_VERIFIED"));
            Assert.That(result.IsRuntimeActivated, Is.False);
            Assert.That(result.UsesLegacyFallback, Is.True);
            Assert.That(result.Policy, Is.SameAs(RuntimeEconomyPolicy.LegacyFallback));
            Assert.That(result.Policy.PolicyId, Is.EqualTo(RuntimeEconomyPolicy.LegacyPolicyId));
            Assert.That(result.Policy.IsCatalogConfigured, Is.False);
            Assert.That(
                result.ComputedSourceSha256,
                Is.EqualTo(RuntimeEconomyActivationGate.CatalogSourceSha256)
            );
        }

        [Test]
        public void AllTrueExactManifest_ActivatesConfiguredImmutablePolicy()
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );

            Assert.That(result.Status, Is.EqualTo(RuntimeEconomyActivationStatus.Activated), result.Reason);
            Assert.That(result.ReasonCode, Is.EqualTo("RUNTIME_ECON_ACTIVATED"));
            Assert.That(result.IsRuntimeActivated, Is.True);
            Assert.That(result.UsesLegacyFallback, Is.False);
            Assert.That(result.Policy, Is.Not.SameAs(RuntimeEconomyPolicy.LegacyFallback));
            Assert.That(result.Policy.PolicyId, Is.EqualTo(RuntimeEconomyActivationGate.CatalogId));
            Assert.That(result.Policy.CatalogSourceSha256, Is.EqualTo(RuntimeEconomyActivationGate.CatalogSourceSha256));
            Assert.That(result.Policy.IsCatalogConfigured, Is.True);
            Assert.That(result.Policy.CanRebaseStartingBalances, Is.False);
            Assert.That(result.Policy.EnergyCapacity, Is.EqualTo(5L));
            Assert.That(result.Policy.LevelEntryCost, Is.EqualTo(1L));
            Assert.That(result.Policy.RetryCost, Is.EqualTo(0L));
            Assert.That(result.Policy.RefillIntervalMinutes, Is.EqualTo(30L));
            Assert.That(result.Policy.DailyRewardEnabled, Is.True);
            Assert.That(result.Policy.DailyCadenceUtcDays, Is.EqualTo(1));
            Assert.That(result.Policy.DailyGold, Is.EqualTo(500L));
            Assert.That(result.Policy.DailyGems, Is.EqualTo(20L));
            Assert.That(result.Policy.DailyEnergy, Is.EqualTo(1L));
            Assert.That(result.Policy.RewardDoublingEnabled, Is.True);
            Assert.That(result.Policy.RewardDoublingCurrencyId, Is.EqualTo("gold"));
            Assert.That(result.Policy.RewardDoublingMultiplier, Is.EqualTo(2));
            Assert.That(result.Policy.RewardDoublingMaxClaims, Is.EqualTo(1));
            Assert.That(result.Policy.RewardDoublingOptional, Is.True);
            Assert.That(result.Policy.FirstClearRewardCount, Is.EqualTo(10));
            RuntimeEconomyFirstClearReward levelTwoReward;
            Assert.That(
                result.Policy.TryGetFirstClearReward(
                    1,
                    2,
                    "chapter01.level02",
                    out levelTwoReward
                ),
                Is.True
            );
            Assert.That(levelTwoReward, Is.Not.Null);
            Assert.That(levelTwoReward.Gold, Is.EqualTo(130L));
            Assert.That(levelTwoReward.Gems, Is.Zero);
            Assert.That(levelTwoReward.BogatyrMace, Is.EqualTo(1L));
            foreach (var property in typeof(RuntimeEconomyPolicy).GetProperties())
            {
                Assert.That(
                    property.CanWrite,
                    Is.False,
                    property.Name + " must remain read-only."
                );
            }
            foreach (var property in typeof(RuntimeEconomyFirstClearReward).GetProperties())
            {
                Assert.That(
                    property.CanWrite,
                    Is.False,
                    property.Name + " must remain read-only."
                );
            }

            RuntimeEconomyPolicy activatedPolicy = result.Policy;
            proposal.Catalog.energy.capacity = 99L;
            proposal.Catalog.progression.levels[1].firstClearGrants[0].amount = 999999L;
            manifest.catalogId = "mutated-after-evaluation";
            manifest.runtimeActivationApproved = false;

            Assert.That(activatedPolicy.PolicyId, Is.EqualTo(RuntimeEconomyActivationGate.CatalogId));
            Assert.That(activatedPolicy.EnergyCapacity, Is.EqualTo(5L));
            Assert.That(activatedPolicy.IsCatalogConfigured, Is.True);
            RuntimeEconomyFirstClearReward stillFrozen;
            Assert.That(
                activatedPolicy.TryGetFirstClearReward(
                    1,
                    2,
                    "chapter01.level02",
                    out stillFrozen
                ),
                Is.True
            );
            Assert.That(stillFrozen.Gold, Is.EqualTo(130L));
        }

        [TestCase(
            "productApproval",
            RuntimeEconomyActivationStatus.ProductApprovalMissing,
            "RUNTIME_ECON_PRODUCT_APPROVAL_MISSING"
        )]
        [TestCase(
            "compilationVerified",
            RuntimeEconomyActivationStatus.VerificationIncomplete,
            "RUNTIME_ECON_COMPILATION_NOT_VERIFIED"
        )]
        [TestCase(
            "editorSelfTestsPassed",
            RuntimeEconomyActivationStatus.VerificationIncomplete,
            "RUNTIME_ECON_EDITOR_SELF_TESTS_NOT_PASSED"
        )]
        [TestCase(
            "editModeTestsPassed",
            RuntimeEconomyActivationStatus.VerificationIncomplete,
            "RUNTIME_ECON_EDIT_MODE_TESTS_NOT_PASSED"
        )]
        [TestCase(
            "playModeTestsPassed",
            RuntimeEconomyActivationStatus.VerificationIncomplete,
            "RUNTIME_ECON_PLAY_MODE_TESTS_NOT_PASSED"
        )]
        [TestCase(
            "runtimeActivationApproved",
            RuntimeEconomyActivationStatus.RuntimeApprovalMissing,
            "RUNTIME_ECON_RUNTIME_APPROVAL_MISSING"
        )]
        public void EachFalseVerificationOrApprovalFlag_FailsClosed(
            string flag,
            RuntimeEconomyActivationStatus expectedStatus,
            string expectedReasonCode
        )
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);
            SetFlagFalse(manifest, flag);

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );

            AssertFallback(result, expectedStatus, expectedReasonCode);
        }

        [Test]
        public void WrongUnityVersion_FailsClosed()
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion + "-wrong"
            );

            AssertFallback(
                result,
                RuntimeEconomyActivationStatus.UnityVersionMismatch,
                "RUNTIME_ECON_UNITY_VERSION_MISMATCH"
            );
        }

        [Test]
        public void WrongManifestSourceHash_FailsClosed()
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);
            manifest.catalogSourceSha256 = new string('0', 64);

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );

            AssertFallback(
                result,
                RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                "RUNTIME_ECON_SOURCE_HASH_MISMATCH"
            );
        }

        [Test]
        public void ManifestCatalogMismatch_FailsClosed()
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);
            manifest.catalogId = RuntimeEconomyActivationGate.CatalogId + "-wrong";

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );

            AssertFallback(
                result,
                RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                "RUNTIME_ECON_CATALOG_IDENTITY_MISMATCH"
            );
        }

        [Test]
        public void MutatedDetachedCatalogDto_FailsSourceBindingClosed()
        {
            EconomyCatalogLoadResult proposal = LoadProposal();
            RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);
            proposal.Catalog.energy.capacity = 6L;

            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );

            AssertFallback(
                result,
                RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                "RUNTIME_ECON_DETACHED_SOURCE_MISMATCH"
            );
        }

        [Test]
        public void Evaluation_NeverMutatesRepresentativePlayerSaveOrRawPlayerPrefsSave()
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
            var detachedSave = new PlayerSaveData
            {
                saveVersion = 6,
                gold = 4321,
                gems = 87,
                energy = 2,
                energyMax = 5,
                lastEnergyUtcTicks = new DateTime(2026, 8, 31, 12, 30, 0, DateTimeKind.Utc).Ticks,
                chapter = 1,
                level = 7,
                stars = 14,
                storyStage = 3,
                selectedChapter = 1,
                selectedLevel = 7,
                highestUnlockedLevel = 8,
                lastCompletedChapter = 1,
                lastCompletedLevel = 6,
                lastRewardGold = 190,
                lastRewardGems = 5,
                lastRewardEligibleForDouble = true,
                lastRewardDoubled = false,
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
            detachedSave.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 6,
                    bestStars = 3,
                    bestScore = 24680,
                    attempts = 2,
                    completions = 1,
                    rewardClaimed = true,
                }
            );
            detachedSave.completedTutorialSteps.Add("swap");
            detachedSave.completedStoryScenes.Add("chapter01.intro");
            detachedSave.acknowledgedChapterCompletions.Add(1);
            string detachedBefore = JsonUtility.ToJson(detachedSave);
            const string rawSaveBefore =
                "{\"saveVersion\":6,\"gold\":777,\"sentinel\":\"activation-gate-raw-save\"}";

            try
            {
                PlayerPrefs.SetString(PersistedSaveKey, rawSaveBefore);
                PlayerPrefs.Save();

                EconomyCatalogLoadResult proposal = LoadProposal();
                RuntimeEconomyActivationManifestData manifest = CreateAllTrueManifest(proposal);
                RuntimeEconomyActivationResult activated = RuntimeEconomyActivationGate.Evaluate(
                    proposal.Catalog,
                    proposal.SourceJson,
                    manifest,
                    RuntimeEconomyActivationGate.RequiredUnityVersion
                );
                Assert.That(activated.IsRuntimeActivated, Is.True, activated.Reason);

                manifest.playModeTestsPassed = false;
                RuntimeEconomyActivationResult fallback = RuntimeEconomyActivationGate.Evaluate(
                    proposal.Catalog,
                    proposal.SourceJson,
                    manifest,
                    RuntimeEconomyActivationGate.RequiredUnityVersion
                );
                Assert.That(fallback.UsesLegacyFallback, Is.True, fallback.Reason);

                Assert.That(JsonUtility.ToJson(detachedSave), Is.EqualTo(detachedBefore));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKey), Is.EqualTo(rawSaveBefore));
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

        static EconomyCatalogLoadResult LoadProposal()
        {
            EconomyCatalogLoadResult proposal = EconomyCatalogLoader.LoadProposal();
            Assert.That(proposal.Succeeded, Is.True, proposal.Error);
            Assert.That(proposal.SourceJson, Is.Not.Empty);
            Assert.That(
                RuntimeEconomyActivationGate.ComputeNormalizedSourceSha256(proposal.SourceJson),
                Is.EqualTo(RuntimeEconomyActivationGate.CatalogSourceSha256)
            );
            return proposal;
        }

        static RuntimeEconomyActivationManifestData CreateAllTrueManifest(
            EconomyCatalogLoadResult proposal
        )
        {
            return new RuntimeEconomyActivationManifestData
            {
                schemaVersion = RuntimeEconomyActivationGate.ManifestSchemaVersion,
                manifestId = RuntimeEconomyActivationGate.ManifestId,
                catalogSchemaVersion = proposal.Catalog.schemaVersion,
                catalogId = RuntimeEconomyActivationGate.CatalogId,
                catalogSourceSha256 = RuntimeEconomyActivationGate.CatalogSourceSha256,
                unityVersion = RuntimeEconomyActivationGate.RequiredUnityVersion,
                productApproval = true,
                compilationVerified = true,
                editorSelfTestsPassed = true,
                editModeTestsPassed = true,
                playModeTestsPassed = true,
                runtimeActivationApproved = true,
            };
        }

        static void SetFlagFalse(RuntimeEconomyActivationManifestData manifest, string flag)
        {
            switch (flag)
            {
                case "productApproval":
                    manifest.productApproval = false;
                    break;
                case "compilationVerified":
                    manifest.compilationVerified = false;
                    break;
                case "editorSelfTestsPassed":
                    manifest.editorSelfTestsPassed = false;
                    break;
                case "editModeTestsPassed":
                    manifest.editModeTestsPassed = false;
                    break;
                case "playModeTestsPassed":
                    manifest.playModeTestsPassed = false;
                    break;
                case "runtimeActivationApproved":
                    manifest.runtimeActivationApproved = false;
                    break;
                default:
                    Assert.Fail("Unknown activation flag: " + flag);
                    break;
            }
        }

        static void AssertFallback(
            RuntimeEconomyActivationResult result,
            RuntimeEconomyActivationStatus expectedStatus,
            string expectedReasonCode
        )
        {
            Assert.That(result.Status, Is.EqualTo(expectedStatus), result.Reason);
            Assert.That(result.ReasonCode, Is.EqualTo(expectedReasonCode));
            Assert.That(result.IsRuntimeActivated, Is.False);
            Assert.That(result.UsesLegacyFallback, Is.True);
            Assert.That(result.Policy, Is.SameAs(RuntimeEconomyPolicy.LegacyFallback));
            Assert.That(result.Policy.PolicyId, Is.EqualTo(RuntimeEconomyPolicy.LegacyPolicyId));
            Assert.That(result.Policy.IsCatalogConfigured, Is.False);
        }
    }
}
