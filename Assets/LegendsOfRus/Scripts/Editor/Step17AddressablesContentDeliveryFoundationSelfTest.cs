#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LegendsOfRus.ContentDelivery;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17AddressablesContentDeliveryFoundationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 5 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("Unity version is pinned to 6000.5.9f1", UnityVersionIsPinned,
                ref passed, ref total, failures);
            Check("Addressables package is pinned to released 2.8.1", AddressablesPackageIsPinned,
                ref passed, ref total, failures);
            Check("content-delivery core remains engine-free", ContentCoreIsEngineFree,
                ref passed, ref total, failures);
            Check("stable group and label contract is pinned", GroupAndLabelContractIsPinned,
                ref passed, ref total, failures);
            Check("migration manifest validates deterministically", MigrationManifestValidates,
                ref passed, ref total, failures);
            Check("only local validation probe is addressable-ready", OnlyProbeIsAddressableReady,
                ref passed, ref total, failures);
            Check("current production Resources assets retain fallback keys", ProductionEntriesHaveFallbacks,
                ref passed, ref total, failures);
            Check("remote delivery activation remains false", RemoteActivationIsBlocked,
                ref passed, ref total, failures);
            Check("runtime content service is bootstrap-wired", RuntimeServiceIsWired,
                ref passed, ref total, failures);
            Check("runtime loader retains Resources fallback", RuntimeLoaderHasFallback,
                ref passed, ref total, failures);
            Check("runtime loader has bounded release path", RuntimeLoaderHasReleasePath,
                ref passed, ref total, failures);
            Check("editor sync creates groups without migrating Resources", EditorSyncBoundaryIsSafe,
                ref passed, ref total, failures);
            Check("local validation probe exists outside Resources", ValidationProbeIsSafe,
                ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", ApprovedPngInventoryIsUnchanged,
                ref passed, ref total, failures);
            Check("commercial and economy activation remains false", CommercialGatesRemainDisabled,
                ref passed, ref total, failures);
            Check("Step 4 NUnit compile hotfix remains retained", Step4CompileHotfixIsRetained,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log("v1.17 Step 5 self-tests passed: " + passed + "/" + total +
                    ". This validates source/editor boundaries only; it does not claim " +
                    "Addressables build, Unity Test Runner, Android device, CDN, or production evidence.");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.17 Step 5 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary);
            throw new InvalidOperationException("v1.17 Step 5 self-tests failed.\n" + summary);
        }

        static bool UnityVersionIsPinned()
        {
            return string.Equals(Application.unityVersion, "6000.5.9f1", StringComparison.Ordinal);
        }

        static bool AddressablesPackageIsPinned()
        {
            string manifest = Read("Packages/manifest.json");
            return manifest.Contains("\"com.unity.addressables\": \"2.8.1\"");
        }

        static bool ContentCoreIsEngineFree()
        {
            string asmdef = Read(
                "Assets/LegendsOfRus/Scripts/ContentDeliveryCore/LegendsOfRus.ContentDeliveryCore.asmdef");
            return asmdef.Contains("\"noEngineReferences\": true") &&
                !asmdef.Contains("Unity.Addressables") &&
                !asmdef.Contains("UnityEngine");
        }

        static bool GroupAndLabelContractIsPinned()
        {
            return ContentDeliveryPolicy.BootLocalGroup == "LOR_BootLocal" &&
                ContentDeliveryPolicy.SharedDeferredGroup == "LOR_SharedDeferred" &&
                ContentDeliveryPolicy.Chapter01DeferredGroup == "LOR_Chapter01Deferred" &&
                ContentDeliveryPolicy.ValidationLocalGroup == "LOR_ValidationLocal" &&
                ContentDeliveryPolicy.LocalLabel == "lor.local" &&
                ContentDeliveryPolicy.ValidationProbeAddress == "lor.validation.local-probe";
        }

        static bool MigrationManifestValidates()
        {
            return RuntimeContentCatalog.TryLoad(out RuntimeContentCatalog catalog, out _) &&
                catalog != null && catalog.Count == 31;
        }

        static bool OnlyProbeIsAddressableReady()
        {
            string source = Read("Assets/LegendsOfRus/Resources/Config/content_delivery_manifest.json");
            int readyTrue = CountOccurrences(source, "\"addressableReady\": true");
            int readyFalse = CountOccurrences(source, "\"addressableReady\": false");
            return readyTrue == 1 && readyFalse == 30 &&
                source.Contains(ContentDeliveryPolicy.ValidationProbeAddress);
        }

        static bool ProductionEntriesHaveFallbacks()
        {
            return RuntimeContentCatalog.TryLoad(out RuntimeContentCatalog catalog, out _) &&
                catalog.TryGet("lor.screen.home", out ContentCatalogEntry home) &&
                home.legacyResourceKey == "DesignScreens/Home" &&
                !home.addressableReady &&
                catalog.TryGet("lor.chapter.01.data-levels", out ContentCatalogEntry levels) &&
                levels.legacyResourceKey == "Config/chapter01_levels" &&
                !levels.addressableReady;
        }

        static bool RemoteActivationIsBlocked()
        {
            ContentDeliveryActivationSnapshot value = ContentDeliveryActivation.Load();
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/ContentDeliveryActivation.cs");
            return !value.LocalAddressablesApproved &&
                !value.AnyRemoteFeatureApproved &&
                !value.FinalRuntimeApproved &&
                !ContentDeliveryActivation.RemoteDeliveryCompiled &&
                !ContentDeliveryActivation.CatalogUpdatesCompiled &&
                !source.Contains("UnityWebRequest") &&
                !source.Contains("HttpClient");
        }

        static bool RuntimeServiceIsWired()
        {
            string bootstrap = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs");
            string asmdef = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef");
            return bootstrap.Contains("FindOrAdd<ContentDeliveryService>(root);") &&
                asmdef.Contains("LegendsOfRus.ContentDeliveryCore") &&
                asmdef.Contains("Unity.Addressables") &&
                asmdef.Contains("Unity.ResourceManager");
        }

        static bool RuntimeLoaderHasFallback()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/ContentDeliveryService.cs");
            return source.Contains("Addressables.LoadAssetAsync<Sprite>(address)") &&
                source.Contains("Resources.Load<Sprite>(entry.legacyResourceKey)") &&
                source.Contains("LegacyResourcesFallback");
        }

        static bool RuntimeLoaderHasReleasePath()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/ContentDeliveryService.cs");
            return source.Contains("public bool ReleaseSprite(string address)") &&
                source.Contains("Addressables.Release(lease.Handle)") &&
                source.Contains("activeSprites.Remove(address)") &&
                source.Contains("void OnDestroy()");
        }

        static bool EditorSyncBoundaryIsSafe()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/Editor/Step17AddressablesContentDeliveryConfigurator.cs");
            return source.Contains("AddressableAssetSettingsDefaultObject.GetSettings(true)") &&
                source.Contains("settings.CreateOrMoveEntry(probeGuid, validationGroup)") &&
                source.Contains("Step 5 intentionally does not move or mark any current Resources asset") &&
                !source.Contains("DesignScreens/Home.png") &&
                !source.Contains("Resources/DesignScreens");
        }

        static bool ValidationProbeIsSafe()
        {
            const string path = "Assets/LegendsOfRus/ContentDelivery/Validation/local_content_probe.txt";
            return File.Exists(Path.Combine(Directory.GetCurrentDirectory(), path)) &&
                path.IndexOf("/Resources/", StringComparison.Ordinal) < 0;
        }

        static bool ApprovedPngInventoryIsUnchanged()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D", new[] { "Assets/LegendsOfRus" });
            return guids.Select(AssetDatabase.GUIDToAssetPath)
                .Count(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) == 65;
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

        static bool Step4CompileHotfixIsRetained()
        {
            string source = Read(
                "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs");
            return source.Contains("Array.IndexOf(parameterTypes, typeof(string))") &&
                !source.Contains("Does.Not.Contain(typeof(string))");
        }

        static string Read(string relativePath)
        {
            return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), relativePath));
        }

        static int CountOccurrences(string source, string value)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += value.Length;
            }
            return count;
        }

        static void Check(
            string label,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (test())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + label);
            }
            catch (Exception exception)
            {
                failures.Add("FAIL: " + label + " (" + exception.GetType().Name +
                    ": " + exception.Message + ")");
            }
        }
    }
}
#endif
