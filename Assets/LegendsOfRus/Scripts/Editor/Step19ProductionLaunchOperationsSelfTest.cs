#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using LegendsOfRus.LaunchOperations;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19ProductionLaunchOperationsSelfTest
    {
        static int total;
        static int passed;
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 1 Self-Tests")]
        public static void Run()
        {
            total = 0; passed = 0;
            string root = Directory.GetParent(Application.dataPath).FullName;
            string core = Path.Combine(root, "Assets/LegendsOfRus/Scripts/LaunchOperationsCore");
            string evidence = Path.Combine(root, "ProjectDocs/LaunchOperations");
            string activationPath = Path.Combine(root, "Assets/LegendsOfRus/Resources/Config/launch_operations_activation.json");
            string activation = File.ReadAllText(activationPath);

            Check("Step19 core asmdef exists", File.Exists(Path.Combine(core, "LegendsOfRus.LaunchOperationsCore.asmdef")));
            Check("Step19 requirements exist", File.Exists(Path.Combine(core, "LaunchOperationsRequirements.cs")));
            Check("Step19 models exist", File.Exists(Path.Combine(core, "LaunchOperationsModels.cs")));
            Check("Step19 validation exists", File.Exists(Path.Combine(core, "LaunchOperationsValidation.cs")));
            Check("Step19 preflight evaluator exists", File.Exists(Path.Combine(core, "LaunchOperationsPreflightEvaluator.cs")));
            Check("Step19 vitals evaluator exists", File.Exists(Path.Combine(core, "AndroidVitalsEvaluator.cs")));
            Check("Step19 observation buffer exists", File.Exists(Path.Combine(core, "LaunchObservationBuffer.cs")));
            Check("Step19 activation exists", File.Exists(activationPath));
            Check("local launch operations tooling approved", activation.Contains("\"localLaunchOperationsToolingApproved\": true"));
            Check("local observability tooling approved", activation.Contains("\"localPostLaunchObservabilityToolingApproved\": true"));
            Check("launch candidate remains unverified", activation.Contains("\"launchCandidateReadinessVerified\": false"));
            Check("production release remains unauthorized", activation.Contains("\"productionReleaseAuthorized\": false"));
            Check("store publishing remains unauthorized", activation.Contains("\"storePublishingAuthorized\": false"));
            Check("monitoring plan remains unapproved", activation.Contains("\"monitoringPlanApproved\": false"));
            Check("incident response plan remains unapproved", activation.Contains("\"incidentResponsePlanApproved\": false"));
            Check("rollback plan remains unapproved", activation.Contains("\"rollbackPlanApproved\": false"));
            Check("support coverage remains unapproved", activation.Contains("\"supportCoverageApproved\": false"));
            Check("release history remains unreviewed", activation.Contains("\"releaseHistoryReviewed\": false"));
            Check("outstanding releases remain uncleared", activation.Contains("\"outstandingReleasesCleared\": false"));
            Check("first release default remains true", activation.Contains("\"firstProductionRelease\": true"));
            Check("first release rollout remains 100", activation.Contains("\"requestedRolloutPercent\": 100"));
            Check("manual launch remains unauthorized", activation.Contains("\"manualLaunchStartAuthorized\": false"));
            Check("launch remains unstarted", activation.Contains("\"launchStarted\": false"));
            Check("release remains unavailable", activation.Contains("\"releaseAvailableToUsers\": false"));
            Check("vitals evidence remains unretained", activation.Contains("\"androidVitalsEvidenceRetained\": false"));
            Check("vitals evidence remains unreviewed", activation.Contains("\"androidVitalsEvidenceReviewed\": false"));
            Check("incident register remains unretained", activation.Contains("\"incidentRegisterRetained\": false"));
            Check("incident register remains unreviewed", activation.Contains("\"incidentRegisterReviewed\": false"));
            Check("launch review remains unretained", activation.Contains("\"launchWindowReviewRetained\": false"));
            Check("manual rollout action remains unauthorized", activation.Contains("\"manualRolloutActionAuthorized\": false"));
            Check("automatic rollout remains disabled", activation.Contains("\"automaticRolloutActionEnabled\": false"));
            Check("production telemetry provider remains disabled", activation.Contains("\"productionTelemetryProviderEnabled\": false"));
            Check("Play reporting API remains disabled", activation.Contains("\"playDeveloperReportingApiEnabled\": false"));
            Check("store publishing action remains disabled", activation.Contains("\"storePublishingActionEnabled\": false"));

            Check("Android vitals template exists", File.Exists(Path.Combine(evidence, "android_vitals_snapshot.template.json")));
            Check("incident template exists", File.Exists(Path.Combine(evidence, "launch_incident_summary.template.json")));
            Check("launch review template exists", File.Exists(Path.Combine(evidence, "launch_window_review.template.json")));
            Check("launch operations README exists", File.Exists(Path.Combine(evidence, "README.md")));
            Check("actual Android vitals evidence absent", !File.Exists(Path.Combine(evidence, "android_vitals_snapshot.json")));
            Check("actual incident evidence absent", !File.Exists(Path.Combine(evidence, "launch_incident_summary.json")));
            Check("actual launch review absent", !File.Exists(Path.Combine(evidence, "launch_window_review.json")));
            Check("sanitized snapshot absent in source", !File.Exists(Path.Combine(evidence, "STEP01_SANITIZED_LAUNCH_OPERATIONS_SNAPSHOT.json")));

            Check("crash threshold pinned 1.09", Math.Abs(LaunchOperationsRequirements.UserPerceivedCrashOverallBadBehaviorPercent - 1.09) < 0.0001);
            Check("ANR threshold pinned 0.47", Math.Abs(LaunchOperationsRequirements.UserPerceivedAnrOverallBadBehaviorPercent - 0.47) < 0.0001);
            Check("phone crash threshold pinned 8", Math.Abs(LaunchOperationsRequirements.UserPerceivedCrashPhoneModelBadBehaviorPercent - 8.0) < 0.0001);
            Check("phone ANR threshold pinned 8", Math.Abs(LaunchOperationsRequirements.UserPerceivedAnrPhoneModelBadBehaviorPercent - 8.0) < 0.0001);
            Check("wake lock threshold pinned 5", Math.Abs(LaunchOperationsRequirements.ExcessivePartialWakeLockOverallBadBehaviorPercent - 5.0) < 0.0001);
            Check("battery threshold pinned 1", Math.Abs(LaunchOperationsRequirements.ExcessiveBatteryUsageOverallBadBehaviorPercent - 1.0) < 0.0001);
            Check("vitals window bounded 28 days", LaunchOperationsRequirements.MaximumVitalsWindowDays == 28);
            Check("observation buffer bounded 64", LaunchOperationsRequirements.ObservationBufferCapacity == 64);
            Check("watch band is local 75 percent", Math.Abs(LaunchOperationsRequirements.WatchBandRatio - 0.75) < 0.0001);

            Check("SHA-256 accepted", LaunchOperationsValidation.IsSha256(HashA));
            Check("bad SHA rejected", !LaunchOperationsValidation.IsSha256("abc"));
            Check("candidate rc01 accepted", LaunchOperationsValidation.IsCandidateId("v1.18-step05-rc01"));
            Check("candidate rc00 rejected", !LaunchOperationsValidation.IsCandidateId("v1.18-step05-rc00"));

            var plan = ReadyPlan();
            Check("ready plan may launch", LaunchOperationsPreflightEvaluator.Evaluate(plan).MayStartManualProductionLaunch);
            plan = ReadyPlan(); plan.MonitoringPlanApproved = false;
            Check("monitoring plan gates launch", !LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview);
            plan = ReadyPlan(); plan.IncidentResponsePlanApproved = false;
            Check("incident plan gates launch", !LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview);
            plan = ReadyPlan(); plan.RollbackPlanApproved = false;
            Check("rollback plan gates launch", !LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview);
            plan = ReadyPlan(); plan.RequestedRolloutPercent = 20;
            Check("first launch rejects staged rollout", !LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview);
            plan = ReadyPlan(); plan.ReleaseKind = ReleaseKind.ProductionUpdate; plan.RequestedRolloutPercent = 10;
            Check("production update allows staged rollout", LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview);
            plan = ReadyPlan(); plan.ManualLaunchStartAuthorized = false;
            Check("manual launch authorization remains separate", LaunchOperationsPreflightEvaluator.Evaluate(plan).ReadyForHumanLaunchOperationsReview && !LaunchOperationsPreflightEvaluator.Evaluate(plan).MayStartManualProductionLaunch);

            var vitals = HealthyVitals(); var incidents = CleanIncidents();
            Check("healthy vitals classify healthy", AndroidVitalsEvaluator.Evaluate(vitals, incidents, ReleaseKind.FirstProductionRelease, 100, true).Classification == LaunchHealthClassification.Healthy);
            Check("healthy vitals recommend continue", AndroidVitalsEvaluator.Evaluate(vitals, incidents, ReleaseKind.FirstProductionRelease, 100, true).Recommendation == LaunchRolloutRecommendation.Continue);
            Check("healthy rollout needs manual action authorization", !AndroidVitalsEvaluator.Evaluate(vitals, incidents, ReleaseKind.FirstProductionRelease, 100, false).MayContinueOrIncreaseRollout);
            vitals = HealthyVitals(); vitals.UserPerceivedCrashRatePercent = 1.09;
            Check("crash threshold classifies critical", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Critical);
            vitals = HealthyVitals(); vitals.UserPerceivedAnrRatePercent = 0.47;
            Check("ANR threshold classifies critical", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Critical);
            vitals = HealthyVitals(); vitals.MaxPhoneModelCrashRatePercent = 8;
            Check("phone crash threshold classifies critical", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Critical);
            vitals = HealthyVitals(); vitals.MaxPhoneModelAnrRatePercent = 8;
            Check("phone ANR threshold classifies critical", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Critical);
            vitals = HealthyVitals(); vitals.ExcessivePartialWakeLockRatePercent = 5;
            Check("wake lock threshold classifies unhealthy", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Unhealthy);
            incidents = CleanIncidents(); incidents.OpenCritical = 1;
            Check("critical incident classifies critical", AndroidVitalsEvaluator.Evaluate(HealthyVitals(), incidents, ReleaseKind.FirstProductionRelease, 100, false).Classification == LaunchHealthClassification.Critical);
            vitals = HealthyVitals(); vitals.UserPerceivedCrashRatePercent = 2;
            Check("critical staged update recommends halt", AndroidVitalsEvaluator.Evaluate(vitals, CleanIncidents(), ReleaseKind.ProductionUpdate, 25, false).Recommendation == LaunchRolloutRecommendation.HaltStagedRollout);

            var buffer = new LaunchObservationBuffer(2);
            buffer.Add(new LaunchHealthEvaluation { Classification = LaunchHealthClassification.Healthy, Recommendation = LaunchRolloutRecommendation.Continue }, 1, 0, 0);
            buffer.Add(new LaunchHealthEvaluation { Classification = LaunchHealthClassification.Watch, Recommendation = LaunchRolloutRecommendation.Hold }, 2, 0, 0);
            buffer.Add(new LaunchHealthEvaluation { Classification = LaunchHealthClassification.Critical, Recommendation = LaunchRolloutRecommendation.EscalateRollbackReview }, 3, 1, 1);
            Check("observation buffer remains bounded", buffer.Count == 2);
            Check("observation buffer retains newest", buffer.Snapshot()[1].Classification == LaunchHealthClassification.Critical);

            string source = string.Join("\n", Directory.GetFiles(core, "*.cs").Select(File.ReadAllText)) + "\n" + File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Editor/Step19ProductionLaunchOperationsRunner.cs"));
            Check("Step19 contains no automatic build", !source.Contains("BuildPipeline.BuildPlayer"));
            Check("Step19 contains no UnityWebRequest", !source.Contains("UnityWebRequest"));
            Check("Step19 contains no HttpClient", !source.Contains("HttpClient"));
            Check("Step19 contains no Google APIs", !source.Contains("Google.Apis"));
            Check("Step19 contains no Play reporting SDK", !source.Contains("PlayDeveloperReporting"));
            Check("Step19 contains no signing credential fields", !ContainsAny(source, new[] { "keystorePass", "keyaliasPass", "keyStorePass", "keyAliasPass" }));
            Check("Step19 contains no device identifiers", !ContainsAny(source, new[] { "deviceUniqueIdentifier", "advertisingIdentifier", "SystemInfo.deviceModel", "AndroidId", "IMEI" }));
            Check("Step19 contains no tester identity fields", !ContainsAny(source, new[] { "TesterEmail", "testerEmail", "TesterName", "testerName", "GoogleGroup", "googleGroup" }));

            string manifest = File.ReadAllText(Path.Combine(root, "Packages/manifest.json"));
            Check("Addressables 2.8.1 retained", manifest.Contains("\"com.unity.addressables\": \"2.8.1\""));
            string balanceAsm = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef"));
            Check("Hotfix04 TestAssemblies retained", balanceAsm.Contains("TestAssemblies"));
            string diagnosticsTest = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs"));
            Check("NUnit type containment hotfix retained", diagnosticsTest.Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            string runtimeTests = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef"));
            Check("ContentDeliveryCore asmdef hotfix retained", runtimeTests.Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("approved PNG count remains 65", Directory.GetFiles(Path.Combine(root, "Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65);
            string gameState = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs"));
            Check("save v6 boundary retained", gameState.Contains("public int saveVersion = 6;"));

            if (passed != total) throw new Exception("v1.19 Step 1 self-tests failed: " + passed + "/" + total);
            Debug.Log("v1.19 Step 1 self-tests passed: " + passed + "/" + total);
        }

        static LaunchOperationsPlan ReadyPlan()
        {
            return new LaunchOperationsPlan
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, LaunchCandidateReadinessVerified = true,
                ProductionReleaseAuthorized = true, StorePublishingAuthorized = true, MonitoringPlanApproved = true,
                IncidentResponsePlanApproved = true, RollbackPlanApproved = true, SupportCoverageApproved = true,
                ReleaseHistoryReviewed = true, OutstandingReleasesCleared = true, ReleaseKind = ReleaseKind.FirstProductionRelease,
                RequestedRolloutPercent = 100, ManualLaunchStartAuthorized = true
            };
        }

        static AndroidVitalsSnapshot HealthyVitals()
        {
            return new AndroidVitalsSnapshot
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, EvidenceRetained = true, EvidenceReviewed = true,
                WindowDays = 1, UserPerceivedCrashRatePercent = 0.1, UserPerceivedAnrRatePercent = 0.05,
                MaxPhoneModelCrashRatePercent = 1, MaxPhoneModelAnrRatePercent = 1,
                ExcessivePartialWakeLockRatePercent = 0.2, ExcessiveBatteryUsageRatePercent = 0.1
            };
        }

        static LaunchIncidentSummary CleanIncidents() => new LaunchIncidentSummary { RegisterRetained = true, RegisterReviewed = true };

        static bool ContainsAny(string text, string[] values)
        {
            for (int i = 0; i < values.Length; i++) if (text.Contains(values[i])) return true;
            return false;
        }

        static void Check(string name, bool condition)
        {
            total++;
            if (!condition) throw new Exception("Step 19 self-test failed: " + name);
            passed++;
        }
    }
}
#endif
