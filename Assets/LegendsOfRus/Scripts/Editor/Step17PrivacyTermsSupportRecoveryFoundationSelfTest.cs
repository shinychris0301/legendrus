#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.Compliance;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17PrivacyTermsSupportRecoveryFoundationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 6 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("Unity version is pinned to 6000.5.9f1", () =>
                string.Equals(Application.unityVersion, "6000.5.9f1", StringComparison.Ordinal),
                ref passed, ref total, failures);
            Check("Addressables 2.8.1 compatibility hotfix is retained", AddressablesHotfixRetained,
                ref passed, ref total, failures);
            Check("Step 5 PlayMode asmdef dependency hotfix is retained", Step5AsmdefHotfixRetained,
                ref passed, ref total, failures);
            Check("compliance core remains engine-free", ComplianceCoreIsEngineFree,
                ref passed, ref total, failures);
            Check("runtime references compliance core", RuntimeReferencesComplianceCore,
                ref passed, ref total, failures);
            Check("privacy/support service is bootstrap-wired", RuntimeServiceIsWired,
                ref passed, ref total, failures);
            Check("local compliance persistence is approved", LocalPersistenceApproved,
                ref passed, ref total, failures);
            Check("final legal documents and UI remain unapproved", LegalFlowFailsClosed,
                ref passed, ref total, failures);
            Check("account recovery and deletion backends remain blocked", AccountBackendsBlocked,
                ref passed, ref total, failures);
            Check("recovery candidate persistence is forbidden", () =>
                !AccountRecoveryPolicy.CandidatePersistenceAllowed &&
                !AccountRecoveryPolicy.RecoveryBackendCompiled &&
                !AccountRecoveryPolicy.AccountDeletionBackendCompiled,
                ref passed, ref total, failures);
            Check("support export is a fixed local allowlist", SupportExportIsAllowlisted,
                ref passed, ref total, failures);
            Check("device unique identifier is absent from Step 6 runtime", DeviceUniqueIdentifierAbsent,
                ref passed, ref total, failures);
            Check("recovery input is never logged or persisted", RecoveryInputIsEphemeral,
                ref passed, ref total, failures);
            Check("runtime analytics callsites are consent-gated", RuntimeAnalyticsIsConsentGated,
                ref passed, ref total, failures);
            Check("rewarded ads are consent-gated", RewardedAdsAreConsentGated,
                ref passed, ref total, failures);
            Check("commercial and economy activation remain false", CommercialGatesRemainDisabled,
                ref passed, ref total, failures);
            Check("remote content delivery remains false", RemoteContentRemainsDisabled,
                ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", ApprovedPngInventoryUnchanged,
                ref passed, ref total, failures);
            Check("save payload version remains 6", SaveSchemaUnchanged,
                ref passed, ref total, failures);
            Check("Step 4 NUnit compatibility hotfix remains retained", Step4NUnitHotfixRetained,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.17 Step 6 self-tests passed: " + passed + "/" + total +
                    ". This validates local source/editor boundaries only; it does not " +
                    "claim final legal text, production account backend, store review, " +
                    "Unity Test Runner, Android device, or publishing evidence.");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.17 Step 6 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary);
            throw new InvalidOperationException(
                "v1.17 Step 6 self-tests failed.\n" + summary);
        }

        static bool AddressablesHotfixRetained()
        {
            return Read("Packages/manifest.json").Contains(
                "\"com.unity.addressables\": \"2.8.1\"");
        }

        static bool Step5AsmdefHotfixRetained()
        {
            return Read(
                "Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")
                .Contains("LegendsOfRus.ContentDeliveryCore");
        }

        static bool ComplianceCoreIsEngineFree()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/ComplianceCore/LegendsOfRus.ComplianceCore.asmdef");
            return source.Contains("\"noEngineReferences\": true") &&
                !source.Contains("UnityEngine") &&
                !source.Contains("Unity.Addressables");
        }

        static bool RuntimeReferencesComplianceCore()
        {
            return Read(
                "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef")
                .Contains("LegendsOfRus.ComplianceCore");
        }

        static bool RuntimeServiceIsWired()
        {
            return Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs")
                .Contains("FindOrAdd<PrivacySupportService>(root);");
        }

        static bool LocalPersistenceApproved()
        {
            PrivacySupportActivationSnapshot value = PrivacySupportActivation.Load();
            return value.MayPersistLocalState &&
                value.MayExportLocalSupportSnapshot;
        }

        static bool LegalFlowFailsClosed()
        {
            PrivacySupportActivationSnapshot value = PrivacySupportActivation.Load();
            return !value.FinalLegalDocumentsApproved &&
                !value.LegalAcceptanceUiApproved &&
                !value.SupportUiApproved &&
                !value.AccountRecoveryUiApproved;
        }

        static bool AccountBackendsBlocked()
        {
            PrivacySupportActivationSnapshot value = PrivacySupportActivation.Load();
            return !value.AnyProductionAccountFeatureApproved &&
                !PrivacySupportActivation.ProductionAccountTransportCompiled &&
                !PrivacySupportActivation.RecoveryBackendCompiled &&
                !PrivacySupportActivation.DataDeletionBackendCompiled;
        }

        static bool SupportExportIsAllowlisted()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportService.cs");
            return source.Contains("sealed class SupportExportDto") &&
                source.Contains("supportReference") &&
                source.Contains("applicationVersion") &&
                source.Contains("deviceModel") &&
                !source.Contains("deviceUniqueIdentifier") &&
                !source.Contains("advertisingIdentifier") &&
                !source.Contains("SystemInfo.deviceUniqueIdentifier");
        }

        static bool DeviceUniqueIdentifierAbsent()
        {
            string runtimeDir = Full("Assets/LegendsOfRus/Scripts/Runtime");
            foreach (string path in Directory.GetFiles(runtimeDir, "*.cs", SearchOption.AllDirectories))
            {
                if (File.ReadAllText(path).Contains("deviceUniqueIdentifier"))
                    return false;
            }
            return true;
        }

        static bool RecoveryInputIsEphemeral()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportService.cs");
            string store = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerPrefsComplianceStateStore.cs");
            return source.Contains("TryBeginAccountRecovery") &&
                !store.Contains("candidateCode") &&
                !store.Contains("recoveryCode") &&
                !store.Contains("password") &&
                !store.Contains("email");
        }

        static bool RuntimeAnalyticsIsConsentGated()
        {
            string wrapper = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacyAwareAnalytics.cs");
            if (!wrapper.Contains("PrivacySupportService.Instance.MayUseAnalytics") ||
                !wrapper.Contains("AnalyticsReasonCodes.CaptureNotAllowed"))
                return false;

            string[] productionFiles =
            {
                "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs"
            };
            for (int i = 0; i < productionFiles.Length; i++)
            {
                string source = Read(productionFiles[i]);
                if (source.Contains("AnalyticsService.Try"))
                    return false;
            }
            return true;
        }

        static bool RewardedAdsAreConsentGated()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs");
            return source.Contains("OptionalAdsAllowed()") &&
                source.Contains("PrivacySupportService.Instance.MayUseAds") &&
                source.Contains("MONETIZATION_AD_CONSENT_REQUIRED");
        }

        static bool CommercialGatesRemainDisabled()
        {
            string economy = Read(
                "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json");
            string commercial = Read(
                "Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json");
            return economy.Contains("\"runtimeActivationApproved\": false") &&
                commercial.Contains("\"analyticsActivationApproved\": false") &&
                commercial.Contains("\"iapActivationApproved\": false") &&
                commercial.Contains("\"rewardedAdsActivationApproved\": false") &&
                commercial.Contains("\"finalRuntimeActivationApproved\": false");
        }

        static bool RemoteContentRemainsDisabled()
        {
            string source = Read(
                "Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json");
            return source.Contains("\"remoteCatalogActivationApproved\": false") &&
                source.Contains("\"remoteContentActivationApproved\": false") &&
                source.Contains("\"catalogUpdatesActivationApproved\": false") &&
                source.Contains("\"finalRuntimeActivationApproved\": false");
        }

        static bool ApprovedPngInventoryUnchanged()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D", new[] { "Assets/LegendsOfRus" });
            int count = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    count++;
            }
            return count == 65;
        }

        static bool SaveSchemaUnchanged()
        {
            return new PlayerSaveData().saveVersion == 6 &&
                Read("Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs")
                    .Contains("CurrentPayloadVersion = 6");
        }

        static bool Step4NUnitHotfixRetained()
        {
            string source = Read(
                "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs");
            return source.Contains("Array.IndexOf(parameterTypes, typeof(string))") &&
                !source.Contains("Does.Not.Contain(typeof(string))");
        }

        static string Read(string relativePath)
        {
            return File.ReadAllText(Full(relativePath));
        }

        static string Full(string relativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }

        static void Check(
            string name,
            Func<bool> check,
            ref int passed,
            ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (check())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + name);
            }
            catch (Exception ex)
            {
                failures.Add("FAIL: " + name + " — " + ex.GetType().Name + ": " + ex.Message);
            }
        }
    }
}
#endif
