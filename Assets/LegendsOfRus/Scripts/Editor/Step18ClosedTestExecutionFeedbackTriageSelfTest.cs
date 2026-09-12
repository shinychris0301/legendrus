#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using LegendsOfRus.ClosedTestExecution;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ClosedTestExecutionFeedbackTriageSelfTest
    {
        static int total;
        static int passed;
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 7 Self-Tests")]
        public static void Run()
        {
            total = 0; passed = 0;
            string root = Directory.GetCurrentDirectory();
            string core = Path.Combine(root, "Assets/LegendsOfRus/Scripts/ClosedTestExecutionCore");
            string cfg = Path.Combine(root, "Assets/LegendsOfRus/Resources/Config/closed_test_execution_activation.json");
            string evid = Path.Combine(root, "ProjectDocs/ClosedTestEvidence");

            Check("core directory exists", Directory.Exists(core));
            Check("core assembly exists", File.Exists(Path.Combine(core, "LegendsOfRus.ClosedTestExecutionCore.asmdef")));
            Check("core assembly is engine-free", File.ReadAllText(Path.Combine(core, "LegendsOfRus.ClosedTestExecutionCore.asmdef")).Contains("\"noEngineReferences\": true"));
            Check("activation config exists", File.Exists(cfg));
            string activation = File.Exists(cfg) ? File.ReadAllText(cfg) : string.Empty;
            Check("local execution review tooling approved", activation.Contains("\"localClosedTestExecutionReviewApproved\": true"));
            Check("local feedback triage tooling approved", activation.Contains("\"localFeedbackTriageToolingApproved\": true"));
            Check("local production access tooling approved", activation.Contains("\"localProductionAccessEvidenceToolingApproved\": true"));
            Check("execution evidence remains unretained", activation.Contains("\"closedTestExecutionEvidenceRetained\": false"));
            Check("feedback evidence remains unretained", activation.Contains("\"feedbackTriageEvidenceRetained\": false"));
            Check("feedback review remains false", activation.Contains("\"feedbackTriageReviewed\": false"));
            Check("account rule remains unreviewed", activation.Contains("\"developerAccountRuleReviewed\": false"));
            Check("production application remains unauthorized", activation.Contains("\"productionAccessApplicationAuthorized\": false"));
            Check("production access decision remains unretained", activation.Contains("\"productionAccessDecisionRetained\": false"));
            Check("production access remains ungranted", activation.Contains("\"productionAccessGranted\": false"));
            Check("production release readiness remains unauthorized", activation.Contains("\"productionReleaseReadinessAuthorized\": false"));
            Check("store publishing remains unauthorized", activation.Contains("\"storePublishingAuthorized\": false"));
            Check("execution summary template exists", File.Exists(Path.Combine(evid, "closed_test_execution_summary.template.json")));
            Check("feedback triage template exists", File.Exists(Path.Combine(evid, "feedback_triage.template.json")));
            Check("production access template exists", File.Exists(Path.Combine(evid, "production_access_application_evidence.template.json")));
            Check("actual execution evidence absent", !File.Exists(Path.Combine(evid, "closed_test_execution_summary.json")));
            Check("actual feedback evidence absent", !File.Exists(Path.Combine(evid, "feedback_triage.json")));
            Check("actual production access evidence absent", !File.Exists(Path.Combine(evid, "production_access_application_evidence.json")));
            Check("Step7 sanitized snapshot absent in source", !File.Exists(Path.Combine(evid, "STEP07_PRODUCTION_ACCESS_READINESS_SNAPSHOT.json")));

            Check("SHA-256 format accepted", ClosedTestExecutionValidation.IsSha256(HashA));
            Check("invalid SHA-256 rejected", !ClosedTestExecutionValidation.IsSha256("abc"));
            Check("candidate rc01 accepted", ClosedTestExecutionValidation.IsCandidateId("v1.18-step05-rc01"));
            Check("candidate rc00 rejected", !ClosedTestExecutionValidation.IsCandidateId("v1.18-step05-rc00"));
            Check("issue id accepted", FeedbackTriageEvaluator.IsIssueId("LOR-CT-0001"));
            Check("zero issue id rejected", !FeedbackTriageEvaluator.IsIssueId("LOR-CT-0000"));

            Check("valid execution evidence accepted", ClosedTestExecutionValidation.Validate(ReadyExecution(), HashA).Valid);
            var execution = ReadyExecution(); execution.ArtifactSha256 = HashB;
            Check("execution artifact mismatch rejected", !ClosedTestExecutionValidation.Validate(execution, HashA).Valid);
            execution = ReadyExecution(); execution.ClosedTestReleaseAvailableToTesters = false;
            Check("execution requires tester availability", !ClosedTestExecutionValidation.Validate(execution, HashA).Valid);
            execution = ReadyExecution(); execution.BlockingCrashOrAnrOpen = true;
            Check("execution rejects blocking stability issue", !ClosedTestExecutionValidation.Validate(execution, HashA).Valid);

            Check("fixed reviewed retested feedback accepted", FeedbackTriageEvaluator.Evaluate(new[] { FixedItem("LOR-CT-0001") }).ReadyForProductionAccessReview);
            Check("empty feedback rejected", !FeedbackTriageEvaluator.Evaluate(new ClosedTestFeedbackItem[0]).ReadyForProductionAccessReview);
            var item = FixedItem("LOR-CT-0001"); item.Disposition = FeedbackDisposition.Open; item.Severity = FeedbackSeverity.Critical;
            Check("open critical feedback rejected", !FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview);
            item = FixedItem("LOR-CT-0001"); item.RetestPassed = false;
            Check("fixed unretested feedback rejected", !FeedbackTriageEvaluator.Evaluate(new[] { item }).ReadyForProductionAccessReview);

            var production = ReadyProduction(); production.DeveloperAccountRuleReviewed = false;
            Check("production preparation requires account rule review", !ProductionAccessEvidenceEvaluator.Evaluate(production).MayPrepareApplication);
            production = ReadyProduction(); production.ContinuousOptedInTesterCount = 11;
            Check("applicable personal account requires 12 testers", !ProductionAccessEvidenceEvaluator.Evaluate(production).MayPrepareApplication);
            production = ReadyProduction(); production.ContinuousTestDays = 13;
            Check("applicable personal account requires 14 days", !ProductionAccessEvidenceEvaluator.Evaluate(production).MayPrepareApplication);
            production = ReadyProduction(); production.TesterFeedbackSummaryReviewed = false;
            Check("production preparation requires feedback summary", !ProductionAccessEvidenceEvaluator.Evaluate(production).MayPrepareApplication);
            production = ReadyProduction(); production.ProductionAccessApplicationAuthorized = false;
            Check("submission requires explicit authorization", !ProductionAccessEvidenceEvaluator.Evaluate(production).MaySubmitApplication);
            production = ReadyProduction(); production.ProductionAccessDecisionRetained = false;
            Check("advance requires retained Play decision", !ProductionAccessEvidenceEvaluator.Evaluate(production).MayAdvanceToProductionReleaseReadiness);
            Check("fully satisfied production evidence can advance", ProductionAccessEvidenceEvaluator.Evaluate(ReadyProduction()).MayAdvanceToProductionReleaseReadiness);

            string source = string.Join("\n", Directory.GetFiles(core, "*.cs").Select(File.ReadAllText)) + "\n" + File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Editor/Step18ClosedTestExecutionFeedbackTriageRunner.cs"));
            Check("Step7 contains no network or build invocation", !ContainsAny(source, new[] { "BuildPipeline.BuildPlayer", "UnityWebRequest", "HttpClient", "Google.Apis" }));
            Check("Step7 contains no signing credential fields", !ContainsAny(source, new[] { "keystorePass", "keyaliasPass", "keyStorePass", "keyAliasPass" }));
            Check("Step7 contains no persistent device identifiers", !ContainsAny(source, new[] { "deviceUniqueIdentifier", "advertisingIdentifier", "SystemInfo.deviceModel", "AndroidId", "IMEI" }));
            Check("Step7 schema contains no tester identity fields", !ContainsAny(source, new[] { "TesterEmail", "testerEmail", "TesterName", "testerName", "GoogleGroup", "googleGroup" }));

            string balanceAsm = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef"));
            Check("Hotfix04 TestAssemblies reference retained", balanceAsm.Contains("TestAssemblies"));
            string manifest = File.ReadAllText(Path.Combine(root, "Packages/manifest.json"));
            Check("Addressables 2.8.1 retained", manifest.Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("approved PNG count remains 65", Directory.GetFiles(Path.Combine(root, "Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65);
            string gameState = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs"));
            Check("save v6 boundary retained", gameState.Contains("public int saveVersion = 6;"));

            if (passed != total) throw new Exception("v1.18 Step 7 self-tests failed: " + passed + "/" + total);
            Debug.Log("v1.18 Step 7 self-tests passed: " + passed + "/" + total);
        }

        static ClosedTestExecutionEvidence ReadyExecution()
        {
            return new ClosedTestExecutionEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, ClosedTestReleaseAvailableToTesters = true,
                ObservationDays = 14, OptedInTesterCount = 12, ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14,
                HealthEvidenceValid = true, DeviceEvidenceValid = true, FeedbackEvidenceRetained = true, FeedbackTriageComplete = true
            };
        }

        static ClosedTestFeedbackItem FixedItem(string id)
        {
            return new ClosedTestFeedbackItem
            {
                IssueId = id, Category = FeedbackCategory.Gameplay, Severity = FeedbackSeverity.Medium,
                Disposition = FeedbackDisposition.Fixed, Reviewed = true, Reproduced = true,
                RetestRequired = true, RetestPassed = true
            };
        }

        static ProductionAccessEvidence ReadyProduction()
        {
            return new ProductionAccessEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA,
                DeveloperAccountRuleReviewed = true, PersonalAccountCreatedAfter2023November13 = true,
                ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14, ClosedTestExecutionEvidenceValid = true,
                FeedbackTriageReady = true, TestingProcessSummaryReviewed = true, TesterFeedbackSummaryReviewed = true,
                ChangesMadeSummaryReviewed = true, ProductionReadinessSummaryReviewed = true,
                ProductionAccessApplicationAuthorized = true, ProductionAccessDecisionRetained = true, ProductionAccessGranted = true
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
            if (!condition) throw new Exception("Step 7 self-test failed: " + name);
            passed++;
        }
    }
}
#endif
