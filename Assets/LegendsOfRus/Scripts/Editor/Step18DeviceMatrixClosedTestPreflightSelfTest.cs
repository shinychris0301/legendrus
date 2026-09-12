#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.QA;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18DeviceMatrixClosedTestPreflightSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 1 Self-Tests")]
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
            Check("QA readiness core remains engine-free", QaCoreIsEngineFree,
                ref passed, ref total, failures);
            Check("runtime directly references QA readiness core", () =>
                Read("Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef")
                    .Contains("LegendsOfRus.QAReadinessCore"),
                ref passed, ref total, failures);
            Check("closed-test QA service is bootstrap-wired", () =>
                Read("Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs")
                    .Contains("FindOrAdd<ClosedTestQaService>(root);"),
                ref passed, ref total, failures);
            Check("local coarse device capture is approved", () =>
                ClosedTestQaActivation.Load().LocalDeviceProfileCaptureApproved,
                ref passed, ref total, failures);
            Check("local observation buffer is approved", () =>
                ClosedTestQaActivation.Load().LocalObservationBufferApproved,
                ref passed, ref total, failures);
            Check("remote QA transport remains blocked", RemoteQaBlocked,
                ref passed, ref total, failures);
            Check("production signing and publishing remain blocked", SigningPublishingBlocked,
                ref passed, ref total, failures);
            Check("QA source contains no persistent device identifier API", QaIdentifiersAbsent,
                ref passed, ref total, failures);
            Check("device matrix validates deterministic buckets", () =>
            {
                string failure;
                return DeviceMatrixCatalog.Validate(out failure);
            }, ref passed, ref total, failures);
            Check("device matrix covers at least five profiles", () =>
                DeviceMatrixCatalog.RequiredProfileCount >= 5,
                ref passed, ref total, failures);
            Check("observation buffer remains bounded", ObservationBufferBounded,
                ref passed, ref total, failures);
            Check("preflight fails closed without evidence", PreflightFailsClosed,
                ref passed, ref total, failures);
            Check("remote content delivery remains blocked", RemoteContentBlocked,
                ref passed, ref total, failures);
            Check("commercial and economy production activation remain blocked", CommercialBlocked,
                ref passed, ref total, failures);
            Check("final legal and account production paths remain blocked", LegalAccountBlocked,
                ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", ApprovedPngInventoryUnchanged,
                ref passed, ref total, failures);
            Check("save payload version remains 6", SaveSchemaUnchanged,
                ref passed, ref total, failures);
            Check("controlled-test build authorization remains false", () =>
                !ClosedTestQaActivation.Load().ControlledTestBuildAuthorized,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.18 Step 1 self-tests passed: " + passed + "/" + total +
                    ". This validates source/editor guardrails only; it does not claim " +
                    "EditMode/PlayMode completion, representative-device execution, signed " +
                    "AAB creation, Play closed-test upload, or publishing approval.");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.18 Step 1 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary);
            throw new InvalidOperationException("v1.18 Step 1 self-tests failed.\n" + summary);
        }

        static bool QaCoreIsEngineFree()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/QAReadinessCore/LegendsOfRus.QAReadinessCore.asmdef");
            return source.Contains("\"noEngineReferences\": true") &&
                !source.Contains("UnityEngine") && !source.Contains("Unity.Addressables");
        }

        static bool RemoteQaBlocked()
        {
            ClosedTestQaActivationSnapshot value = ClosedTestQaActivation.Load();
            return !value.RemoteQaUploadApproved &&
                !value.ProductionQaProviderApproved &&
                !ClosedTestQaActivation.RemoteQaTransportCompiled &&
                !ClosedTestQaActivation.ProductionQaProviderCompiled;
        }

        static bool SigningPublishingBlocked()
        {
            ClosedTestQaActivationSnapshot value = ClosedTestQaActivation.Load();
            return !value.ProductionSigningApproved &&
                !value.StorePublishingAuthorized &&
                !ClosedTestQaActivation.ProductionSigningCompiled &&
                !ClosedTestQaActivation.StorePublishingCompiled;
        }

        static bool QaIdentifiersAbsent()
        {
            string[] roots =
            {
                Full("Assets/LegendsOfRus/Scripts/QAReadinessCore"),
                Full("Assets/LegendsOfRus/Scripts/Runtime/Services")
            };
            string[] forbidden =
            {
                "deviceUniqueIdentifier",
                "advertisingIdentifier",
                "SystemInfo.deviceModel",
                "SystemInfo.deviceName"
            };
            for (int r = 0; r < roots.Length; r++)
            {
                foreach (string path in Directory.GetFiles(roots[r], "*.cs", SearchOption.AllDirectories))
                {
                    string source = File.ReadAllText(path);
                    if (!path.EndsWith("ClosedTestQaService.cs", StringComparison.Ordinal) &&
                        !path.Contains("QAReadinessCore"))
                        continue;
                    for (int i = 0; i < forbidden.Length; i++)
                        if (source.Contains(forbidden[i]))
                            return false;
                }
            }
            return true;
        }

        static bool ObservationBufferBounded()
        {
            var buffer = new ClosedTestObservationBuffer(4);
            for (int i = 0; i < 12; i++)
                buffer.TryRecord(ClosedTestObservationCode.LevelStarted, i + 1);
            ClosedTestObservation[] snapshot = buffer.Snapshot();
            return buffer.Count == 4 && snapshot.Length == 4 &&
                snapshot[0].LevelNumber == 9 && snapshot[3].LevelNumber == 12;
        }

        static bool PreflightFailsClosed()
        {
            ClosedTestPreflightResult result =
                ClosedTestPreflightEvaluator.Evaluate(default);
            return !result.MayBuildControlledTest &&
                !result.MayPublishClosedTest && result.Blockers.Length >= 10;
        }

        static bool RemoteContentBlocked()
        {
            ContentDeliveryActivationSnapshot value = ContentDeliveryActivation.Load();
            return !value.AnyRemoteFeatureApproved &&
                !ContentDeliveryActivation.RemoteDeliveryCompiled &&
                !ContentDeliveryActivation.CatalogUpdatesCompiled;
        }

        static bool CommercialBlocked()
        {
            string economy = Read("Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json");
            string commercial = Read("Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json");
            return economy.Contains("\"runtimeActivationApproved\": false") &&
                commercial.Contains("\"analyticsActivationApproved\": false") &&
                commercial.Contains("\"iapActivationApproved\": false") &&
                commercial.Contains("\"rewardedAdsActivationApproved\": false") &&
                commercial.Contains("\"finalRuntimeActivationApproved\": false");
        }

        static bool LegalAccountBlocked()
        {
            PrivacySupportActivationSnapshot value = PrivacySupportActivation.Load();
            return !value.FinalLegalDocumentsApproved &&
                !value.AnyProductionAccountFeatureApproved &&
                !PrivacySupportActivation.ProductionAccountTransportCompiled;
        }

        static bool ApprovedPngInventoryUnchanged()
        {
            string root = Full("Assets/LegendsOfRus");
            return Directory.GetFiles(root, "*.png", SearchOption.AllDirectories).Length == 65;
        }

        static bool SaveSchemaUnchanged()
        {
            return Read("Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs")
                .Contains("CurrentPayloadVersion = 6") &&
                Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")
                .Contains("public int saveVersion = 6;");
        }

        static void Check(
            string name, Func<bool> test, ref int passed, ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (test()) { passed++; return; }
            }
            catch (Exception ex)
            {
                failures.Add(name + ": " + ex.GetType().Name);
                return;
            }
            failures.Add(name);
        }

        static string Read(string relative)
        {
            return File.ReadAllText(Full(relative));
        }

        static string Full(string relative)
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
