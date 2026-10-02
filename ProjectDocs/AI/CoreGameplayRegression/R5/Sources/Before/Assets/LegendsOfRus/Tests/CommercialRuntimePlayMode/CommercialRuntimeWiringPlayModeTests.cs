using System;
using System.Linq;
using System.Reflection;
using LegendsOfRus.CommercialCore.Analytics;
using LegendsOfRus.CommercialCore.Monetization;
using LegendsOfRus.CommercialRuntime;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.CommercialRuntime.PlayMode
{
    public sealed class CommercialRuntimeWiringPlayModeTests
    {
        const string SaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] SaveKeys =
        {
            SaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject host;
        MonetizationService previousMonetization;
        bool[] hadSaves;
        string[] previousSaves;

        [SetUp]
        public void SetUp()
        {
            hadSaves = new bool[SaveKeys.Length];
            previousSaves = new string[SaveKeys.Length];
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                string key = SaveKeys[i];
                hadSaves[i] = PlayerPrefs.HasKey(key);
                previousSaves[i] = hadSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();

            previousMonetization = MonetizationService.Instance;
            AnalyticsService.ResetTrusted();
        }

        [TearDown]
        public void TearDown()
        {
            AnalyticsService.ResetTrusted();
            if (host != null)
                UnityEngine.Object.DestroyImmediate(host);
            RestorePreviousMonetizationInstance();
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                string key = SaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadSaves[i])
                    PlayerPrefs.SetString(key, previousSaves[i]);
            }
            PlayerPrefs.Save();
        }

        [Test]
        public void CommercialBootstrap_ComposesPreactivationDisabledRuntime()
        {
            InvokeCommercialBootstrap("ResetForDomainReloadDisabled");
            InvokeCommercialBootstrap("Boot");

            CommercialRuntimeComposition composition =
                CommercialRuntimeBootstrap.Current;
            Assert.That(composition, Is.Not.Null);
            Assert.That(composition.Activation.Phase,
                Is.EqualTo(CommercialRuntimePhase.VerificationRequired));
            Assert.That(composition.Activation.IsRuntimeActivated, Is.False);
            Assert.That(composition.Capabilities.AnalyticsCaptureEnabled, Is.False);
            Assert.That(composition.Capabilities.AnalyticsUploadEnabled, Is.False);
            Assert.That(composition.Capabilities.IapEnabled, Is.False);
            Assert.That(composition.Capabilities.RewardedAdsEnabled, Is.False);
            Assert.That(composition.Capabilities.DurableFulfillmentEnabled, Is.False);
            Assert.That(composition.PerformsExternalCommunication, Is.False);
            Assert.That(composition.PerformsSaveWrites, Is.False);
            Assert.That(composition.PreservesOfflineCoreLoop, Is.True);
        }

        [Test]
        public void CommercialBootstrap_CreatesNoGameObjectAndWritesNoSave()
        {
            string sentinel = "{\"sentinel\":\"commercial_boot\"}";
            PlayerPrefs.SetString(SaveKey, sentinel);
            PlayerPrefs.Save();
            int beforeObjects = UnityEngine.Object
                .FindObjectsByType<Transform>(FindObjectsInactive.Include).Length;

            InvokeCommercialBootstrap("ResetForDomainReloadDisabled");
            InvokeCommercialBootstrap("Boot");

            int afterObjects = UnityEngine.Object
                .FindObjectsByType<Transform>(FindObjectsInactive.Include).Length;
            Assert.That(afterObjects, Is.EqualTo(beforeObjects));
            Assert.That(PlayerPrefs.GetString(SaveKey), Is.EqualTo(sentinel));
        }

        [Test]
        public void DefaultMonetizationActions_AreImmediateUnavailableAndSaveNeutral()
        {
            MonetizationService service = CreateMonetizationService();
            string sentinel = "{\"sentinel\":\"commercial_actions\"}";
            PlayerPrefs.SetString(SaveKey, sentinel);
            PlayerPrefs.Save();

            RuntimeMonetizationActionResult[] results =
            {
                service.TryPurchase("starter"),
                service.TryPurchase("hero"),
                service.TryPurchase("firebird"),
                service.TryPurchase("energy"),
                service.TryRewardedEnergy(),
                service.TryRewardedDouble(),
                service.TryRewardedContinue()
            };

            Assert.That(results.All(result => result != null), Is.True);
            Assert.That(results.All(result =>
                result.State == RuntimeMonetizationActionState.Unavailable), Is.True);
            Assert.That(results.All(result =>
                !result.IsVerifiedAndDurablyCommitted), Is.True);
            Assert.That(PlayerPrefs.GetString(SaveKey), Is.EqualTo(sentinel));
        }

        [Test]
        public void UnknownPurchase_NoLongerDefaultsToFiftyGems()
        {
            MonetizationService service = CreateMonetizationService();
            var save = new PlayerSaveData { gems = 7 };
            string before = JsonUtility.ToJson(save);

            RuntimeMonetizationActionResult result =
                service.TryPurchase("unknown_pack");

            Assert.That(result.State,
                Is.EqualTo(RuntimeMonetizationActionState.Unavailable));
            Assert.That(result.ReasonCode, Is.EqualTo("MONETIZATION_UNKNOWN_OFFER"));
            Assert.That(save.gems, Is.EqualTo(7));
            Assert.That(JsonUtility.ToJson(save), Is.EqualTo(before));
        }

        [Test]
        public void LegacyMockWrappers_CannotGrantOrReportSuccess()
        {
            MonetizationService service = CreateMonetizationService();
            var save = new PlayerSaveData
            {
                gold = 100,
                gems = 20,
                energy = 2,
                lastRewardGold = 50,
                lastRewardEligibleForDouble = true,
                lastRewardDoubled = false
            };
            string before = JsonUtility.ToJson(save);

            Assert.That(service.MockPurchase("starter"), Is.False);
            Assert.That(service.MockPurchase("unknown_pack"), Is.False);
            Assert.That(service.MockRewardedEnergy(), Is.False);
            Assert.That(service.MockRewardedContinue(), Is.False);

            Assert.That(JsonUtility.ToJson(save), Is.EqualTo(before));
        }

        [Test]
        public void ThrowingOrNullTrustedPort_IsContainedAsUnavailable()
        {
            MonetizationService service = CreateMonetizationService();
            Assert.That(service.TryBindRuntimePort(new ThrowingPort()), Is.True);

            Assert.That(service.TryPurchase("starter").State,
                Is.EqualTo(RuntimeMonetizationActionState.Unavailable));
            Assert.That(service.TryRewardedEnergy().State,
                Is.EqualTo(RuntimeMonetizationActionState.Unavailable));
            Assert.That(service.TryRewardedDouble().State,
                Is.EqualTo(RuntimeMonetizationActionState.Unavailable));
            Assert.That(service.TryRewardedContinue().State,
                Is.EqualTo(RuntimeMonetizationActionState.Unavailable));
        }

        [Test]
        public void MonetizationTrustedBindingAndCommittedFactory_AreNotPublic()
        {
            Assert.That(typeof(MonetizationService).GetMethod(
                "TryBindRuntimePort",
                BindingFlags.Instance | BindingFlags.Public
            ), Is.Null);
            Assert.That(typeof(RuntimeMonetizationActionResult).GetMethod(
                "Committed",
                BindingFlags.Static | BindingFlags.Public
            ), Is.Null);
            Assert.That(typeof(RuntimeMonetizationActionResult).GetConstructors(
                BindingFlags.Instance | BindingFlags.Public
            ), Is.Empty);
        }

        [Test]
        public void AnalyticsFacade_DefaultCompositionIsDisabledAndNonThrowing()
        {
            AnalyticsService.ResetTrusted();

            Assert.That(AnalyticsService.CaptureAllowed, Is.False);
            Assert.DoesNotThrow(() => AnalyticsService.TryBeginSession());
            Assert.DoesNotThrow(() => AnalyticsService.TryScreenView(ScreenId.Home));
            Assert.DoesNotThrow(() =>
                AnalyticsService.TryRewardedAdResult(
                    MonetizationAllowlists.RewardedDoublePlacementId,
                    "reward_double",
                    "unavailable",
                    false
                ));
            Assert.That(AnalyticsService.HasStartedSession, Is.False);
            Assert.That(AnalyticsService.AcceptedCount, Is.Zero);
        }

        [Test]
        public void AnalyticsFacade_MapsEveryScreenIdExactlyToAllowlistedSchema()
        {
            long ticks = 10L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "screen_session",
                () => ++ticks
            );
            Assert.That(AnalyticsService.TryInstallTrusted(bridge), Is.True);
            Assert.That(AnalyticsService.TryBeginSession().Accepted, Is.True);

            ScreenId[] values = (ScreenId[])Enum.GetValues(typeof(ScreenId));
            foreach (ScreenId value in values)
            {
                Assert.That(
                    AnalyticsService.TryScreenView(value).Accepted,
                    Is.True,
                    "Screen mapping rejected: " + value
                );
            }
            Assert.That(adapter.Count, Is.EqualTo(values.Length + 1));
            Assert.That(adapter.Snapshot().Skip(1).All(record =>
                record.EventId == AnalyticsEventIds.ScreenView), Is.True);

            AnalyticsRecordResult invalid =
                AnalyticsService.TryScreenView((ScreenId)int.MaxValue);
            Assert.That(invalid.Accepted, Is.False);
            Assert.That(invalid.ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.StringValueNotAllowed));
            Assert.That(adapter.Count, Is.EqualTo(values.Length + 1));
        }

        [Test]
        public void ModalScreens_PauseOutOfEnergyAndBoosterSelectAreMapped()
        {
            long ticks = 20L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "modal_session",
                () => ++ticks
            );
            Assert.That(AnalyticsService.TryInstallTrusted(bridge), Is.True);
            Assert.That(AnalyticsService.TryBeginSession().Accepted, Is.True);

            Assert.That(AnalyticsService.TryScreenView(ScreenId.Pause).Accepted, Is.True);
            Assert.That(AnalyticsService.TryScreenView(ScreenId.OutOfEnergy).Accepted,
                Is.True);
            Assert.That(AnalyticsService.TryScreenView(ScreenId.BoosterSelect).Accepted,
                Is.True);

            string[] mapped = adapter.Snapshot()
                .Skip(1)
                .Select(record => record.Attributes.Single().StringValue)
                .ToArray();
            Assert.That(mapped, Is.EqualTo(new[]
            {
                "pause",
                "out_of_energy",
                "booster_select"
            }));
        }

        [Test]
        public void GameplayViewCanPrecedeLevelStartAndCancelDoesNotEmitTerminal()
        {
            long ticks = 30L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "level_session",
                () => ++ticks
            );
            Assert.That(AnalyticsService.TryInstallTrusted(bridge), Is.True);
            Assert.That(AnalyticsService.TryBeginSession().Accepted, Is.True);
            Assert.That(AnalyticsService.TryScreenView(ScreenId.Gameplay).Accepted,
                Is.True);
            Assert.That(AnalyticsService.TryLevelStart("level_1", 1, false).Accepted,
                Is.True);
            Assert.That(AnalyticsService.TryCancelLevelAttempt("level_1", 1).Accepted,
                Is.True);

            Assert.That(adapter.Snapshot().Select(record => record.EventId),
                Is.EqualTo(new[]
                {
                    AnalyticsEventIds.SessionStart,
                    AnalyticsEventIds.ScreenView,
                    AnalyticsEventIds.LevelStart
                }));
            Assert.That(bridge.HasActiveLevelAttempt, Is.False);
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.Zero);
        }

        MonetizationService CreateMonetizationService()
        {
            host = new GameObject("CommercialRuntimeWiringPlayModeTests");
            return host.AddComponent<MonetizationService>();
        }

        void RestorePreviousMonetizationInstance()
        {
            FieldInfo field = typeof(MonetizationService).GetField(
                "<Instance>k__BackingField",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            Assert.That(field, Is.Not.Null);
            field.SetValue(null, previousMonetization);
        }

        static void InvokeCommercialBootstrap(string methodName)
        {
            MethodInfo method = typeof(CommercialRuntimeBootstrap).GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic
            );
            Assert.That(method, Is.Not.Null, "Missing bootstrap method: " + methodName);
            method.Invoke(null, null);
        }

        sealed class ThrowingPort : IRuntimeMonetizationPort
        {
            public RuntimeMonetizationActionResult TryPurchase(string offerId)
            {
                throw new InvalidOperationException("test-only purchase failure");
            }

            public RuntimeMonetizationActionResult TryRewardedEnergy()
            {
                return null;
            }

            public RuntimeMonetizationActionResult TryRewardedDouble()
            {
                throw new InvalidOperationException("test-only ad failure");
            }

            public RuntimeMonetizationActionResult TryRewardedContinue()
            {
                return null;
            }
        }
    }
}
