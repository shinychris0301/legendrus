#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using LegendsOfRus.LaunchCandidate;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ProductionAccessDecisionLaunchCandidateSelfTest
    {
        static int total;
        static int passed;
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 8 Self-Tests")]
        public static void Run()
        {
            total = 0; passed = 0;
            string root = Directory.GetParent(Application.dataPath).FullName;
            string core = Path.Combine(root, "Assets/LegendsOfRus/Scripts/LaunchCandidateCore");
            string evid = Path.Combine(root, "ProjectDocs/ClosedTestEvidence");
            string activationPath = Path.Combine(root, "Assets/LegendsOfRus/Resources/Config/launch_candidate_activation.json");
            string activation = File.ReadAllText(activationPath);

            Check("Step8 core asmdef exists", File.Exists(Path.Combine(core, "LegendsOfRus.LaunchCandidateCore.asmdef")));
            Check("Step8 models exist", File.Exists(Path.Combine(core, "LaunchCandidateModels.cs")));
            Check("Step8 decision evaluator exists", File.Exists(Path.Combine(core, "ProductionAccessDecisionEvaluator.cs")));
            Check("Step8 launch evaluator exists", File.Exists(Path.Combine(core, "LaunchCandidateReadinessEvaluator.cs")));
            Check("Step8 activation exists", File.Exists(activationPath));
            Check("local production access decision tooling approved", activation.Contains("\"localProductionAccessDecisionToolingApproved\": true"));
            Check("local launch candidate tooling approved", activation.Contains("\"localLaunchCandidateReadinessToolingApproved\": true"));
            Check("account rule remains unreviewed", activation.Contains("\"developerAccountRuleReviewed\": false"));
            Check("production access requirement remains unresolved", activation.Contains("\"productionAccessRequirementResolved\": false"));
            Check("production access evidence remains unretained", activation.Contains("\"productionAccessDecisionEvidenceRetained\": false"));
            Check("production access review remains false", activation.Contains("\"productionAccessDecisionReviewed\": false"));
            Check("production access satisfaction remains false", activation.Contains("\"productionAccessDecisionSatisfied\": false"));
            Check("candidate freeze remains unretained", activation.Contains("\"candidateFreezeManifestRetained\": false"));
            Check("candidate freeze remains unverified", activation.Contains("\"candidateFreezeVerified\": false"));
            Check("artifact freeze match remains false", activation.Contains("\"artifactMatchesFreeze\": false"));
            Check("closed test evidence remains unverified", activation.Contains("\"closedTestExecutionEvidenceVerified\": false"));
            Check("feedback review remains false", activation.Contains("\"feedbackTriageReviewed\": false"));
            Check("store checklist remains unapproved", activation.Contains("\"storeReleaseChecklistApproved\": false"));
            Check("privacy and data safety remains unapproved", activation.Contains("\"privacyAndDataSafetyApproved\": false"));
            Check("legal documents remain unapproved", activation.Contains("\"legalDocumentsApproved\": false"));
            Check("app content declarations remain unapproved", activation.Contains("\"appContentDeclarationsApproved\": false"));
            Check("production application id remains unapproved", activation.Contains("\"productionApplicationIdApproved\": false"));
            Check("versioning remains unapproved", activation.Contains("\"versioningApproved\": false"));
            Check("Play App Signing remains unconfigured", activation.Contains("\"playAppSigningConfigured\": false"));
            Check("upload key workflow remains unreviewed", activation.Contains("\"uploadKeyWorkflowReviewed\": false"));
            Check("Android technical requirements remain unverified", activation.Contains("\"androidTechnicalRequirementsVerified\": false"));
            Check("release notes remain unapproved", activation.Contains("\"releaseNotesApproved\": false"));
            Check("countries regions remain unapproved", activation.Contains("\"productionCountriesRegionsApproved\": false"));
            Check("support readiness remains unapproved", activation.Contains("\"supportReadinessApproved\": false"));
            Check("launch monitoring remains unapproved", activation.Contains("\"launchMonitoringApproved\": false"));
            Check("rollback plan remains unapproved", activation.Contains("\"rollbackPlanApproved\": false"));
            Check("release history remains unreviewed", activation.Contains("\"releaseHistoryReviewed\": false"));
            Check("outstanding releases remain uncleared", activation.Contains("\"outstandingReleasesCleared\": false"));
            Check("first release safe default is 100 percent", activation.Contains("\"requestedRolloutPercent\": 100"));
            Check("production release remains unauthorized", activation.Contains("\"productionReleaseAuthorized\": false"));
            Check("store publishing remains unauthorized", activation.Contains("\"storePublishingAuthorized\": false"));

            Check("production access decision template exists", File.Exists(Path.Combine(evid, "production_access_decision.template.json")));
            Check("launch candidate template exists", File.Exists(Path.Combine(evid, "launch_candidate_readiness.template.json")));
            Check("launch candidate staging readme exists", File.Exists(Path.Combine(evid, "LaunchCandidate/README.md")));
            Check("actual production access decision absent", !File.Exists(Path.Combine(evid, "production_access_decision.json")));
            Check("actual launch candidate evidence absent", !File.Exists(Path.Combine(evid, "launch_candidate_readiness.json")));
            Check("Step8 sanitized snapshot absent in source", !File.Exists(Path.Combine(evid, "STEP08_LAUNCH_CANDIDATE_READINESS_SNAPSHOT.json")));

            Check("SHA-256 accepted", LaunchCandidateValidation.IsSha256(HashA));
            Check("short SHA-256 rejected", !LaunchCandidateValidation.IsSha256("abc"));
            Check("candidate rc01 accepted", LaunchCandidateValidation.IsCandidateId("v1.18-step05-rc01"));
            Check("candidate rc00 rejected", !LaunchCandidateValidation.IsCandidateId("v1.18-step05-rc00"));

            var decision = ReadyDecision(); decision.DeveloperAccountRuleReviewed = false;
            Check("decision requires account rule review", !ProductionAccessDecisionEvaluator.Evaluate(decision).Satisfied);
            decision = ReadyDecision(); decision.ProductionAccessRequirementResolved = false;
            Check("decision requires requirement resolution", !ProductionAccessDecisionEvaluator.Evaluate(decision).Satisfied);
            decision = ReadyDecision(); decision.DecisionStatus = ProductionAccessDecisionStatus.AdditionalTestingRequired;
            Check("additional testing decision blocks launch", !ProductionAccessDecisionEvaluator.Evaluate(decision).Satisfied);
            Check("granted production decision accepted", ProductionAccessDecisionEvaluator.Evaluate(ReadyDecision()).Satisfied);
            decision = ReadyDecision(); decision.ProductionAccessDecisionRequired = false; decision.DecisionEvidenceRetained = false; decision.DecisionReviewed = false; decision.DecisionStatus = ProductionAccessDecisionStatus.NotRequired;
            Check("explicit not-required decision accepted", ProductionAccessDecisionEvaluator.Evaluate(decision).Satisfied);

            var launch = ReadyLaunch(); launch.CandidateFreezeVerified = false;
            Check("launch requires verified freeze", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.ClosedTestExecutionEvidenceValid = false;
            Check("launch requires closed test evidence", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.FeedbackTriageReady = false;
            Check("launch requires feedback triage", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.ProductionAccessDecisionSatisfied = false;
            Check("launch requires production access decision", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.StoreReleaseChecklistComplete = false;
            Check("launch requires store checklist", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.TargetApiLevel = 35;
            Check("launch requires target API 36", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.FirstProductionRelease = true; launch.RequestedRolloutPercent = 20;
            Check("first production release rejects staged rollout", !LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.FirstProductionRelease = false; launch.RequestedRolloutPercent = 10;
            Check("update may use valid staged rollout", LaunchCandidateReadinessEvaluator.Evaluate(launch).ReadyForHumanLaunchReview);
            launch = ReadyLaunch(); launch.ProductionReleaseAuthorized = false;
            Check("manual rollout requires production authorization", !LaunchCandidateReadinessEvaluator.Evaluate(launch).MayStartManualProductionRollout);
            launch = ReadyLaunch(); launch.StorePublishingAuthorized = false;
            Check("manual rollout requires publishing authorization", !LaunchCandidateReadinessEvaluator.Evaluate(launch).MayStartManualProductionRollout);
            Check("fully satisfied launch evidence may start manual rollout", LaunchCandidateReadinessEvaluator.Evaluate(ReadyLaunch()).MayStartManualProductionRollout);

            string source = string.Join("\n", Directory.GetFiles(core, "*.cs").Select(File.ReadAllText)) + "\n" + File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Editor/Step18ProductionAccessDecisionLaunchCandidateRunner.cs"));
            Check("Step8 contains no network or build invocation", !ContainsAny(source, new[] { "BuildPipeline.BuildPlayer", "UnityWebRequest", "HttpClient", "Google.Apis", "PlayDeveloperReporting" }));
            Check("Step8 contains no signing credential fields", !ContainsAny(source, new[] { "keystorePass", "keyaliasPass", "keyStorePass", "keyAliasPass" }));
            Check("Step8 contains no persistent device identifiers", !ContainsAny(source, new[] { "deviceUniqueIdentifier", "advertisingIdentifier", "SystemInfo.deviceModel", "AndroidId", "IMEI" }));
            Check("Step8 contains no tester identity fields", !ContainsAny(source, new[] { "TesterEmail", "testerEmail", "TesterName", "testerName", "GoogleGroup", "googleGroup" }));

            string balanceAsm = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef"));
            Check("Hotfix04 TestAssemblies reference retained", balanceAsm.Contains("TestAssemblies"));
            string manifest = File.ReadAllText(Path.Combine(root, "Packages/manifest.json"));
            Check("Addressables 2.8.1 retained", manifest.Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("approved PNG count remains 65", Directory.GetFiles(Path.Combine(root, "Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65);
            string gameState = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs"));
            Check("save v6 boundary retained", gameState.Contains("public int saveVersion = 6;"));

            if (passed != total) throw new Exception("v1.18 Step 8 self-tests failed: " + passed + "/" + total);
            Debug.Log("v1.18 Step 8 self-tests passed: " + passed + "/" + total);
        }

        static ProductionAccessDecisionEvidence ReadyDecision()
        {
            return new ProductionAccessDecisionEvidence
            {
                DeveloperAccountRuleReviewed = true,
                ProductionAccessRequirementResolved = true,
                ProductionAccessDecisionRequired = true,
                DecisionEvidenceRetained = true,
                DecisionReviewed = true,
                DecisionStatus = ProductionAccessDecisionStatus.Granted
            };
        }

        static LaunchCandidateEvidence ReadyLaunch()
        {
            return new LaunchCandidateEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, CandidateFreezeManifestSha256 = HashB,
                CandidateFreezeManifestRetained = true, CandidateFreezeVerified = true, ArtifactMatchesFreeze = true,
                ClosedTestExecutionEvidenceValid = true, FeedbackTriageReady = true, ProductionAccessDecisionSatisfied = true,
                StoreReleaseChecklistComplete = true, PrivacyAndDataSafetyApproved = true, LegalDocumentsApproved = true,
                AppContentDeclarationsApproved = true, ProductionApplicationIdApproved = true, VersioningApproved = true,
                PlayAppSigningConfigured = true, UploadKeyWorkflowReviewed = true, TargetApiLevel = 36,
                AppBundleVerified = true, Arm64Verified = true, PageSize16KbVerified = true,
                ReleaseNotesApproved = true, ProductionCountriesRegionsApproved = true, SupportReadinessApproved = true,
                LaunchMonitoringApproved = true, RollbackPlanApproved = true, ReleaseHistoryReviewed = true,
                OutstandingReleasesCleared = true, FirstProductionRelease = true, RequestedRolloutPercent = 100,
                ProductionReleaseAuthorized = true, StorePublishingAuthorized = true
            };
        }

        static bool ContainsAny(string text, string[] values)
        {
            for (int i = 0; i < values.Length; i++) if (text.Contains(values[i])) return true;
            return false;
        }

        static void Check(string name, bool condition)
        {
            total++;
            if (!condition) throw new Exception("Step 8 self-test failed: " + name);
            passed++;
        }
    }
}
#endif
