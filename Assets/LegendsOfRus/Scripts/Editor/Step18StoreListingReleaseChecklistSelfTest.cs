#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.StoreRelease;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18StoreListingReleaseChecklistSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 3 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("Unity version is pinned to 6000.5.9f1", () =>
                string.Equals(Application.unityVersion, "6000.5.9f1", StringComparison.Ordinal),
                ref passed, ref total, failures);
            Check("Addressables 2.8.1 hotfix is retained", () =>
                Read("Packages/manifest.json").Contains("\"com.unity.addressables\": \"2.8.1\""),
                ref passed, ref total, failures);
            Check("Step 5 PlayMode asmdef dependency hotfix is retained", () =>
                Read("Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")
                    .Contains("LegendsOfRus.ContentDeliveryCore"),
                ref passed, ref total, failures);
            Check("Step 4 NUnit Type compatibility hotfix is retained", () =>
                Read("Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")
                    .Contains("Array.IndexOf(parameterTypes, typeof(string))"),
                ref passed, ref total, failures);
            Check("Store release core remains engine-free", StoreReleaseCoreIsEngineFree,
                ref passed, ref total, failures);
            Check("store listing snapshot date is pinned", () =>
                Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json")
                    .Contains("\"requirementsSnapshotDate\": \"2026-09-09\""),
                ref passed, ref total, failures);
            Check("app name limit is 30", () => StoreListingRequirements.AppNameMaxCharacters == 30,
                ref passed, ref total, failures);
            Check("short description limit is 80", () => StoreListingRequirements.ShortDescriptionMaxCharacters == 80,
                ref passed, ref total, failures);
            Check("full description limit is 4000", () => StoreListingRequirements.FullDescriptionMaxCharacters == 4000,
                ref passed, ref total, failures);
            Check("app icon requirement is 512 square", () =>
                StoreListingRequirements.IconWidth == 512 && StoreListingRequirements.IconHeight == 512,
                ref passed, ref total, failures);
            Check("feature graphic requirement is 1024 by 500", () =>
                StoreListingRequirements.FeatureGraphicWidth == 1024 && StoreListingRequirements.FeatureGraphicHeight == 500,
                ref passed, ref total, failures);
            Check("screenshot publication minimum is two", () =>
                StoreListingRequirements.MinimumScreenshotCount == 2,
                ref passed, ref total, failures);
            Check("game screenshot recommendation target is three", () =>
                StoreListingRequirements.RecommendedGameScreenshotCount == 3,
                ref passed, ref total, failures);
            Check("recommended portrait screenshot is 1080 by 1920", () =>
                StoreListingRequirements.RecommendedPortraitScreenshotWidth == 1080 &&
                StoreListingRequirements.RecommendedPortraitScreenshotHeight == 1920,
                ref passed, ref total, failures);
            Check("screenshot alt text limit is 140", () =>
                StoreListingRequirements.ScreenshotAltTextMaxCharacters == 140,
                ref passed, ref total, failures);
            Check("default store listing validation fails closed", StoreListingFailsClosed,
                ref passed, ref total, failures);
            Check("default release checklist fails closed", ReleaseChecklistFailsClosed,
                ref passed, ref total, failures);
            Check("local store listing validation is approved", () =>
                Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json")
                    .Contains("\"localStoreListingValidationApproved\": true"),
                ref passed, ref total, failures);
            Check("store listing text remains unapproved", () =>
                Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json")
                    .Contains("\"storeListingTextApproved\": false"),
                ref passed, ref total, failures);
            Check("store listing assets remain unapproved", () =>
                Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json")
                    .Contains("\"storeListingAssetsApproved\": false"),
                ref passed, ref total, failures);
            Check("privacy Data safety remains unapproved", () =>
                Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json")
                    .Contains("\"privacyDataSafetyApproved\": false"),
                ref passed, ref total, failures);
            Check("production signing remains blocked", ProductionSigningBlocked,
                ref passed, ref total, failures);
            Check("controlled-test build remains blocked", ControlledBuildBlocked,
                ref passed, ref total, failures);
            Check("store publishing remains blocked", StorePublishingBlocked,
                ref passed, ref total, failures);
            Check("production commercial activation remains blocked", CommercialBlocked,
                ref passed, ref total, failures);
            Check("remote QA upload remains blocked", RemoteQaBlocked,
                ref passed, ref total, failures);
            Check("final store asset staging has no publishable graphics", FinalStoreGraphicsAbsent,
                ref passed, ref total, failures);
            Check("reference design screens are explicitly excluded", () =>
                Read("Assets/LegendsOfRus/Documentation/V1_18_STEP03_STORE_LISTING_ASSET_INVENTORY.json")
                    .Contains("REFERENCE_ONLY_NOT_STORE_ASSETS"),
                ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", ApprovedPngInventoryUnchanged,
                ref passed, ref total, failures);
            Check("save payload version remains 6", SaveSchemaUnchanged,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log("v1.18 Step 3 self-tests passed: " + passed + "/" + total +
                    ". This validates the source release-preflight foundation only; it does not approve store assets, production signing, AAB upload, or publishing.");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.18 Step 3 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary);
            throw new InvalidOperationException("v1.18 Step 3 self-tests failed.\n" + summary);
        }

        static bool StoreReleaseCoreIsEngineFree()
        {
            string asm = Read("Assets/LegendsOfRus/Scripts/StoreReleaseCore/LegendsOfRus.StoreReleaseCore.asmdef");
            if (!asm.Contains("\"noEngineReferences\": true")) return false;
            string root = Full("Assets/LegendsOfRus/Scripts/StoreReleaseCore");
            foreach (string path in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(path);
                if (source.Contains("using UnityEngine") || source.Contains("UnityEditor")) return false;
            }
            return true;
        }

        static bool StoreListingFailsClosed()
        {
            StoreListingValidationResult result = StoreListingRequirements.Validate(
                default, null, null, Array.Empty<StoreAssetEvidence>());
            return !result.PublicationMinimumMet && result.BlockingIssues.Length >= 4;
        }

        static bool ReleaseChecklistFailsClosed()
        {
            StoreReleaseChecklistResult result = StoreReleaseChecklistEvaluator.Evaluate(default);
            return !result.MayCreateControlledTestArtifact && !result.MayUploadClosedTest &&
                Array.IndexOf(result.UploadBlockers, StoreReleaseBlockerCodes.PublishingNotAuthorized) >= 0;
        }

        static bool ProductionSigningBlocked()
        {
            string store = Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json");
            string qa = Read("Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json");
            return store.Contains("\"productionSigningApproved\": false") &&
                qa.Contains("\"productionSigningApproved\": false");
        }

        static bool ControlledBuildBlocked()
        {
            string store = Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json");
            string qa = Read("Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json");
            return store.Contains("\"controlledTestBuildAuthorized\": false") &&
                qa.Contains("\"controlledTestBuildAuthorized\": false");
        }

        static bool StorePublishingBlocked()
        {
            string store = Read("Assets/LegendsOfRus/Resources/Config/store_release_activation.json");
            string qa = Read("Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json");
            return store.Contains("\"storePublishingAuthorized\": false") &&
                qa.Contains("\"storePublishingAuthorized\": false");
        }

        static bool CommercialBlocked()
        {
            string commercial = Read("Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json");
            return commercial.Contains("\"analyticsActivationApproved\": false") &&
                commercial.Contains("\"iapActivationApproved\": false") &&
                commercial.Contains("\"rewardedAdsActivationApproved\": false") &&
                commercial.Contains("\"finalRuntimeActivationApproved\": false");
        }

        static bool RemoteQaBlocked()
        {
            return Read("Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json")
                .Contains("\"remoteQaUploadApproved\": false");
        }

        static bool FinalStoreGraphicsAbsent()
        {
            string dir = Full("ProjectDocs/StoreListing/FinalAssets");
            if (!Directory.Exists(dir)) return false;
            return Directory.GetFiles(dir, "*.png", SearchOption.TopDirectoryOnly).Length == 0 &&
                Directory.GetFiles(dir, "*.jpg", SearchOption.TopDirectoryOnly).Length == 0 &&
                Directory.GetFiles(dir, "*.jpeg", SearchOption.TopDirectoryOnly).Length == 0;
        }

        static bool ApprovedPngInventoryUnchanged()
        {
            return Directory.GetFiles(Full("Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65;
        }

        static bool SaveSchemaUnchanged()
        {
            string source = Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs");
            return source.Contains("CurrentSaveVersion = 6") || source.Contains("SaveVersion = 6") ||
                source.Contains("currentSaveVersion = 6") || source.Contains("saveVersion = 6");
        }

        static void Check(
            string name,
            Func<bool> condition,
            ref int passed,
            ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (condition())
                {
                    passed++;
                    return;
                }
            }
            catch (Exception ex)
            {
                failures.Add("FAIL: " + name + " (" + ex.GetType().Name + ": " + ex.Message + ")");
                return;
            }
            failures.Add("FAIL: " + name);
        }

        static string Full(string relative)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative));
        }

        static string Read(string relative)
        {
            return File.ReadAllText(Full(relative));
        }
    }
}
#endif
