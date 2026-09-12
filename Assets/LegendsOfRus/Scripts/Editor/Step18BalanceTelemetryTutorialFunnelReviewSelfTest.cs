#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.BalanceReview;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18BalanceTelemetryTutorialFunnelReviewSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("Unity version is pinned to 6000.5.9f1", () =>
                string.Equals(Application.unityVersion, "6000.5.9f1", StringComparison.Ordinal),
                ref passed, ref total, failures);
            Check("Addressables 2.8.1 compatibility hotfix is retained", () =>
                Read("Packages/manifest.json").Contains("\"com.unity.addressables\": \"2.8.1\""),
                ref passed, ref total, failures);
            Check("Step 5 PlayMode asmdef compatibility hotfix is retained", () =>
                Read("Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")
                    .Contains("LegendsOfRus.ContentDeliveryCore"),
                ref passed, ref total, failures);
            Check("Step 4 NUnit Type compatibility hotfix is retained", () =>
                Read("Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")
                    .Contains("Array.IndexOf(parameterTypes, typeof(string))"),
                ref passed, ref total, failures);
            Check("balance review core is engine-free", BalanceCoreIsEngineFree,
                ref passed, ref total, failures);
            Check("runtime directly references balance review core", () =>
                Read("Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef")
                    .Contains("LegendsOfRus.BalanceReviewCore"),
                ref passed, ref total, failures);
            Check("balance review service is bootstrap-wired", () =>
                Read("Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs")
                    .Contains("FindOrAdd<BalanceTelemetryReviewService>(root);"),
                ref passed, ref total, failures);
            Check("local balance observation is approved", () =>
                BalanceTelemetryReviewActivation.Load().LocalObservationBufferApproved,
                ref passed, ref total, failures);
            Check("local balance review report is approved", () =>
                BalanceTelemetryReviewActivation.Load().LocalReviewReportApproved,
                ref passed, ref total, failures);
            Check("remote telemetry and production analytics bridge remain blocked", RemoteTelemetryBlocked,
                ref passed, ref total, failures);
            Check("automatic balance and tutorial mutation remain blocked", AutomaticMutationBlocked,
                ref passed, ref total, failures);
            Check("balance review source contains no network or persistent identifier APIs", BalanceReviewSensitiveApisAbsent,
                ref passed, ref total, failures);
            Check("balance observation buffer remains bounded", ObservationBufferBounded,
                ref passed, ref total, failures);
            Check("authored tutorial keys are deterministic and nonzero", StableTutorialKeyWorks,
                ref passed, ref total, failures);
            Check("level aggregation computes win and retry rates", LevelAggregationWorks,
                ref passed, ref total, failures);
            Check("level aggregation computes winning move margin", MoveMarginWorks,
                ref passed, ref total, failures);
            Check("high failure review is detected", HighFailureDetected,
                ref passed, ref total, failures);
            Check("tutorial completion drop-off is detected", TutorialDropDetected,
                ref passed, ref total, failures);
            Check("gameplay level start is locally review-wired", () =>
                Read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs")
                    .Contains("TryRecordLevelStart"),
                ref passed, ref total, failures);
            Check("gameplay win and fail are locally review-wired", () =>
            {
                string source = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
                return source.Contains("TryRecordLevelWin") && source.Contains("TryRecordLevelFail");
            }, ref passed, ref total, failures);
            Check("tutorial shown and completed are locally review-wired", () =>
            {
                string source = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs");
                return source.Contains("TryRecordTutorialShown") &&
                    source.Contains("TryRecordTutorialCompleted");
            }, ref passed, ref total, failures);
            Check("Step 1 QA remote/signing/publishing gates remain blocked", Step1ReleaseGatesBlocked,
                ref passed, ref total, failures);
            Check("commercial and economy production activation remain blocked", CommercialBlocked,
                ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", ApprovedPngInventoryUnchanged,
                ref passed, ref total, failures);
            Check("save payload version remains 6", SaveSchemaUnchanged,
                ref passed, ref total, failures);
            Check("remote content delivery remains blocked", RemoteContentBlocked,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.18 Step 2 self-tests passed: " + passed + "/" + total +
                    ". Source/editor guardrails only: this does not claim Test Runner, " +
                    "representative-device, balance quality, tutorial conversion, signed build, " +
                    "production analytics transport, or Play closed-test publishing evidence.");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.18 Step 2 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary);
            throw new InvalidOperationException("v1.18 Step 2 self-tests failed.\n" + summary);
        }

        static bool BalanceCoreIsEngineFree()
        {
            string source = Read(
                "Assets/LegendsOfRus/Scripts/BalanceReviewCore/LegendsOfRus.BalanceReviewCore.asmdef");
            if (!source.Contains("\"noEngineReferences\": true"))
                return false;
            string root = Full("Assets/LegendsOfRus/Scripts/BalanceReviewCore");
            foreach (string path in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string code = File.ReadAllText(path);
                if (code.Contains("using UnityEngine") || code.Contains("UnityEditor"))
                    return false;
            }
            return true;
        }

        static bool RemoteTelemetryBlocked()
        {
            BalanceTelemetryReviewActivationSnapshot value = BalanceTelemetryReviewActivation.Load();
            return !value.RemoteTelemetryUploadApproved &&
                !value.ProductionAnalyticsBridgeApproved &&
                !BalanceTelemetryReviewActivation.RemoteTelemetryTransportCompiled &&
                !BalanceTelemetryReviewActivation.ProductionAnalyticsBridgeCompiled;
        }

        static bool AutomaticMutationBlocked()
        {
            BalanceTelemetryReviewActivationSnapshot value = BalanceTelemetryReviewActivation.Load();
            return !value.AutomaticBalanceMutationApproved &&
                !value.TutorialExperimentMutationApproved &&
                !BalanceTelemetryReviewActivation.AutomaticBalanceMutationCompiled &&
                !BalanceTelemetryReviewActivation.TutorialExperimentMutationCompiled;
        }

        static bool BalanceReviewSensitiveApisAbsent()
        {
            string[] roots =
            {
                Full("Assets/LegendsOfRus/Scripts/BalanceReviewCore"),
                Full("Assets/LegendsOfRus/Scripts/Runtime/Services")
            };
            string[] forbidden =
            {
                "deviceUniqueIdentifier",
                "advertisingIdentifier",
                "SystemInfo.deviceModel",
                "SystemInfo.deviceName",
                "UnityWebRequest",
                "HttpClient",
                "PlayerPrefs.SetString(\"Balance",
                "File.WriteAllText"
            };
            for (int r = 0; r < roots.Length; r++)
            {
                foreach (string path in Directory.GetFiles(roots[r], "*.cs", SearchOption.AllDirectories))
                {
                    if (!path.Contains("BalanceReviewCore") &&
                        !path.EndsWith("BalanceTelemetryReviewService.cs", StringComparison.Ordinal) &&
                        !path.EndsWith("BalanceTelemetryReviewActivation.cs", StringComparison.Ordinal))
                        continue;
                    string source = File.ReadAllText(path);
                    for (int i = 0; i < forbidden.Length; i++)
                        if (source.Contains(forbidden[i]))
                            return false;
                }
            }
            return true;
        }

        static bool ObservationBufferBounded()
        {
            var buffer = new BalanceObservationBuffer(4);
            for (int i = 1; i <= 12; i++)
                buffer.TryRecord(BalanceObservationCode.LevelStarted, i, 1);
            BalanceObservation[] snapshot = buffer.Snapshot();
            return buffer.Count == 4 && snapshot.Length == 4 &&
                snapshot[0].LevelNumber == 9 && snapshot[3].LevelNumber == 12;
        }

        static bool StableTutorialKeyWorks()
        {
            int first = StableReviewKey.FromAuthoredId("tutorial.match.start");
            int second = StableReviewKey.FromAuthoredId("tutorial.match.start");
            return first != 0 && first == second;
        }

        static bool LevelAggregationWorks()
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, 1, attempt);
                buffer.TryRecord(
                    attempt <= 3 ? BalanceObservationCode.LevelWon : BalanceObservationCode.LevelFailed,
                    1, attempt, attempt <= 3 ? 4 : 0, 20, 1000);
            }
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            return report.Levels.Length == 1 && report.Levels[0].Attempts == 5 &&
                report.Levels[0].Wins == 3 && report.Levels[0].Retries == 4 &&
                Math.Abs(report.Levels[0].WinRate - 0.6f) < 0.001f;
        }

        static bool MoveMarginWorks()
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, 2, attempt);
                buffer.TryRecord(BalanceObservationCode.LevelWon, 2, attempt, 5, 20, 1000);
            }
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot(), 3);
            return report.Levels.Length == 1 &&
                Math.Abs(report.Levels[0].AverageWinningMoveMargin - 0.25f) < 0.001f;
        }

        static bool HighFailureDetected()
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= 10; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, 3, attempt);
                buffer.TryRecord(
                    attempt <= 2 ? BalanceObservationCode.LevelWon : BalanceObservationCode.LevelFailed,
                    3, attempt, 3, 25, 1000);
            }
            return HasIssue(BalanceReviewAggregator.Build(buffer.Snapshot()),
                BalanceReviewIssueCode.HighFailureRate);
        }

        static bool TutorialDropDetected()
        {
            var buffer = new BalanceObservationBuffer();
            int key = StableReviewKey.FromAuthoredId("tutorial.ice.first");
            for (int i = 0; i < 5; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialShown, 4, tutorialKey: key);
            for (int i = 0; i < 2; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialCompleted, 4, tutorialKey: key);
            return HasIssue(BalanceReviewAggregator.Build(buffer.Snapshot()),
                BalanceReviewIssueCode.TutorialCompletionDrop);
        }

        static bool HasIssue(BalanceReviewReport report, BalanceReviewIssueCode code)
        {
            for (int i = 0; i < report.Issues.Length; i++)
                if (report.Issues[i].Code == code)
                    return true;
            return false;
        }

        static bool Step1ReleaseGatesBlocked()
        {
            ClosedTestQaActivationSnapshot value = ClosedTestQaActivation.Load();
            return !value.RemoteQaUploadApproved &&
                !value.ProductionSigningApproved &&
                !value.ControlledTestBuildAuthorized &&
                !value.StorePublishingAuthorized;
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

        static bool ApprovedPngInventoryUnchanged()
        {
            return Directory.GetFiles(Full("Assets/LegendsOfRus"), "*.png",
                SearchOption.AllDirectories).Length == 65;
        }

        static bool SaveSchemaUnchanged()
        {
            string source = Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs");
            return source.Contains("CurrentSaveVersion = 6") ||
                source.Contains("SaveVersion = 6") ||
                source.Contains("currentSaveVersion = 6") ||
                source.Contains("saveVersion = 6");
        }

        static bool RemoteContentBlocked()
        {
            ContentDeliveryActivationSnapshot value = ContentDeliveryActivation.Load();
            return !value.AnyRemoteFeatureApproved &&
                !ContentDeliveryActivation.RemoteDeliveryCompiled &&
                !ContentDeliveryActivation.CatalogUpdatesCompiled;
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
