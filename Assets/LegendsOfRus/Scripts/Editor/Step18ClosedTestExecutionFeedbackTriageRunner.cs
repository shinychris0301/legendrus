#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.ClosedTestExecution;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ClosedTestExecutionFeedbackTriageRunner
    {
        const string EvidenceDir = "ProjectDocs/ClosedTestEvidence";
        const string ActivationFile = "Assets/LegendsOfRus/Resources/Config/closed_test_execution_activation.json";
        const string ExecutionEvidenceFile = EvidenceDir + "/closed_test_execution_summary.json";
        const string FeedbackEvidenceFile = EvidenceDir + "/feedback_triage.json";
        const string ProductionEvidenceFile = EvidenceDir + "/production_access_application_evidence.json";
        const string SnapshotFile = EvidenceDir + "/STEP07_PRODUCTION_ACCESS_READINESS_SNAPSHOT.json";

        [Serializable]
        sealed class Activation
        {
            public int schemaVersion;
            public string candidateId = string.Empty;
            public bool localClosedTestExecutionReviewApproved;
            public bool localFeedbackTriageToolingApproved;
            public bool localProductionAccessEvidenceToolingApproved;
            public bool closedTestExecutionEvidenceRetained;
            public bool closedTestExecutionEvidenceVerified;
            public bool feedbackTriageEvidenceRetained;
            public bool feedbackTriageReviewed;
            public bool feedbackBlockersCleared;
            public bool developerAccountRuleReviewed;
            public bool personalAccountCreatedAfter2023November13;
            public int continuousOptedInTesterCount;
            public int continuousTestDays;
            public bool testingProcessSummaryReviewed;
            public bool testerFeedbackSummaryReviewed;
            public bool changesMadeSummaryReviewed;
            public bool productionReadinessSummaryReviewed;
            public bool productionAccessQuestionnaireEvidenceRetained;
            public bool productionAccessApplicationAuthorized;
            public bool productionAccessDecisionRetained;
            public bool productionAccessGranted;
            public bool productionReleaseReadinessAuthorized;
            public bool storePublishingAuthorized;
        }

        [Serializable]
        sealed class SanitizedSnapshot
        {
            public int schemaVersion = 1;
            public string candidateId = string.Empty;
            public bool closedTestExecutionEvidenceRetained;
            public bool feedbackTriageEvidenceRetained;
            public bool feedbackTriageReviewed;
            public bool feedbackBlockersCleared;
            public bool developerAccountRuleReviewed;
            public bool personalAccountRuleApplies;
            public int continuousOptedInTesterCount;
            public int continuousTestDays;
            public bool productionAccessQuestionnaireEvidenceRetained;
            public bool productionAccessApplicationAuthorized;
            public bool productionAccessDecisionRetained;
            public bool productionAccessGranted;
            public bool productionReleaseReadinessAuthorized;
            public bool storePublishingAuthorized;
        }

        [MenuItem("Legends of Rus/QA/Print v1.18 Step 7 Closed Test Execution & Production Access Preflight")]
        public static void PrintPreflight()
        {
            Activation a = LoadActivation();
            var production = ToProductionEvidence(a);
            ProductionAccessEvidenceResult result = ProductionAccessEvidenceEvaluator.Evaluate(production);
            Debug.Log(
                "v1.18 Step 7 production-access preflight\n" +
                "Execution evidence file: " + File.Exists(ExecutionEvidenceFile) + "\n" +
                "Feedback triage file: " + File.Exists(FeedbackEvidenceFile) + "\n" +
                "Production access evidence file: " + File.Exists(ProductionEvidenceFile) + "\n" +
                "May prepare application: " + result.MayPrepareApplication + "\n" +
                "May submit application: " + result.MaySubmitApplication + "\n" +
                "May advance to production release readiness: " + result.MayAdvanceToProductionReleaseReadiness + "\n" +
                "Prepare blockers: " + string.Join(", ", result.PrepareBlockers) + "\n" +
                "Submit blockers: " + string.Join(", ", result.SubmitBlockers) + "\n" +
                "Advance blockers: " + string.Join(", ", result.AdvanceBlockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.18 Step 7 Sanitized Production Access Snapshot")]
        public static void WriteSnapshot()
        {
            Activation a = LoadActivation();
            Directory.CreateDirectory(EvidenceDir);
            var snapshot = new SanitizedSnapshot
            {
                candidateId = a.candidateId,
                closedTestExecutionEvidenceRetained = a.closedTestExecutionEvidenceRetained,
                feedbackTriageEvidenceRetained = a.feedbackTriageEvidenceRetained,
                feedbackTriageReviewed = a.feedbackTriageReviewed,
                feedbackBlockersCleared = a.feedbackBlockersCleared,
                developerAccountRuleReviewed = a.developerAccountRuleReviewed,
                personalAccountRuleApplies = a.personalAccountCreatedAfter2023November13,
                continuousOptedInTesterCount = a.continuousOptedInTesterCount,
                continuousTestDays = a.continuousTestDays,
                productionAccessQuestionnaireEvidenceRetained = a.productionAccessQuestionnaireEvidenceRetained,
                productionAccessApplicationAuthorized = a.productionAccessApplicationAuthorized,
                productionAccessDecisionRetained = a.productionAccessDecisionRetained,
                productionAccessGranted = a.productionAccessGranted,
                productionReleaseReadinessAuthorized = a.productionReleaseReadinessAuthorized,
                storePublishingAuthorized = a.storePublishingAuthorized
            };
            File.WriteAllText(SnapshotFile, JsonUtility.ToJson(snapshot, true) + "\n");
            AssetDatabase.Refresh();
            Debug.Log("Wrote privacy-minimal Step 7 snapshot: " + SnapshotFile);
        }

        static ProductionAccessEvidence ToProductionEvidence(Activation a)
        {
            return new ProductionAccessEvidence
            {
                CandidateId = a.candidateId,
                ArtifactSha256 = string.Empty,
                DeveloperAccountRuleReviewed = a.developerAccountRuleReviewed,
                PersonalAccountCreatedAfter2023November13 = a.personalAccountCreatedAfter2023November13,
                ContinuousOptedInTesterCount = a.continuousOptedInTesterCount,
                ContinuousTestDays = a.continuousTestDays,
                ClosedTestExecutionEvidenceValid = a.closedTestExecutionEvidenceVerified,
                FeedbackTriageReady = a.feedbackTriageReviewed && a.feedbackBlockersCleared,
                TestingProcessSummaryReviewed = a.testingProcessSummaryReviewed,
                TesterFeedbackSummaryReviewed = a.testerFeedbackSummaryReviewed,
                ChangesMadeSummaryReviewed = a.changesMadeSummaryReviewed,
                ProductionReadinessSummaryReviewed = a.productionReadinessSummaryReviewed,
                ProductionAccessApplicationAuthorized = a.productionAccessApplicationAuthorized,
                ProductionAccessDecisionRetained = a.productionAccessDecisionRetained,
                ProductionAccessGranted = a.productionAccessGranted
            };
        }

        static Activation LoadActivation()
        {
            if (!File.Exists(ActivationFile)) return new Activation();
            return JsonUtility.FromJson<Activation>(File.ReadAllText(ActivationFile)) ?? new Activation();
        }
    }
}
#endif
