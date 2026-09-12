using System;

namespace LegendsOfRus.ClosedTestExecution
{
    public static class ClosedTestExecutionBlockerCodes
    {
        public const string ExecutionEvidenceMissing = "CTE_EXECUTION_EVIDENCE_MISSING";
        public const string ArtifactHashMismatch = "CTE_ARTIFACT_HASH_MISMATCH";
        public const string ReleaseNotAvailable = "CTE_RELEASE_NOT_AVAILABLE_TO_TESTERS";
        public const string ObservationTooShort = "CTE_OBSERVATION_TOO_SHORT";
        public const string TesterContinuityInsufficient = "CTE_TESTER_CONTINUITY_INSUFFICIENT";
        public const string FeedbackEvidenceMissing = "CTE_FEEDBACK_EVIDENCE_MISSING";
        public const string FeedbackNotFullyTriaged = "CTE_FEEDBACK_NOT_FULLY_TRIAGED";
        public const string BlockingFeedbackOpen = "CTE_BLOCKING_FEEDBACK_OPEN";
        public const string CriticalFeedbackOpen = "CTE_CRITICAL_FEEDBACK_OPEN";
        public const string HighFeedbackOpen = "CTE_HIGH_FEEDBACK_OPEN";
        public const string FixedItemNotRetested = "CTE_FIXED_ITEM_NOT_RETESTED";
        public const string DuplicateIssueId = "CTE_DUPLICATE_ISSUE_ID";
        public const string InvalidIssueId = "CTE_INVALID_ISSUE_ID";
        public const string ProductionQuestionnaireIncomplete = "CTE_PRODUCTION_QUESTIONNAIRE_INCOMPLETE";
        public const string TestingSummaryMissing = "CTE_TESTING_SUMMARY_MISSING";
        public const string FeedbackSummaryMissing = "CTE_FEEDBACK_SUMMARY_MISSING";
        public const string ChangesSummaryMissing = "CTE_CHANGES_SUMMARY_MISSING";
        public const string ReadinessSummaryMissing = "CTE_READINESS_SUMMARY_MISSING";
        public const string ProductionApplicationNotAuthorized = "CTE_PRODUCTION_APPLICATION_NOT_AUTHORIZED";
        public const string AccountRuleNotReviewed = "CTE_ACCOUNT_RULE_NOT_REVIEWED";
        public const string ProductionAccessDecisionMissing = "CTE_PRODUCTION_ACCESS_DECISION_MISSING";
        public const string ProductionAccessNotGranted = "CTE_PRODUCTION_ACCESS_NOT_GRANTED";
    }

    public enum FeedbackCategory
    {
        Unknown = 0,
        Stability = 1,
        Performance = 2,
        Gameplay = 3,
        TutorialOnboarding = 4,
        UserInterface = 5,
        BalanceDifficulty = 6,
        EconomyReward = 7,
        AccessibilityLocalization = 8,
        StorePolicyCompliance = 9,
        Other = 10
    }

    public enum FeedbackSeverity
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum FeedbackDisposition
    {
        Open = 0,
        Accepted = 1,
        Fixed = 2,
        Duplicate = 3,
        Deferred = 4,
        NotActionable = 5
    }

    [Serializable]
    public sealed class ClosedTestFeedbackItem
    {
        public int SchemaVersion = ClosedTestExecutionRequirements.SchemaVersion;
        public string IssueId = string.Empty;
        public FeedbackCategory Category;
        public FeedbackSeverity Severity;
        public FeedbackDisposition Disposition;
        public bool Reviewed;
        public bool Reproduced;
        public bool BlocksProduction;
        public bool RetestRequired;
        public bool RetestPassed;
    }

    [Serializable]
    public sealed class FeedbackTriageSummary
    {
        public int TotalItems;
        public int ReviewedItems;
        public int FixedItems;
        public int DeferredItems;
        public int OpenItems;
        public int CriticalOpenItems;
        public int HighOpenItems;
        public int BlockingOpenItems;
        public int FixedItemsMissingRetest;
        public bool AllReviewed;
        public bool ReadyForProductionAccessReview;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ClosedTestExecutionEvidence
    {
        public int SchemaVersion = ClosedTestExecutionRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public bool ClosedTestReleaseAvailableToTesters;
        public int ObservationDays;
        public int OptedInTesterCount;
        public int ContinuousOptedInTesterCount;
        public int ContinuousTestDays;
        public bool HealthEvidenceValid;
        public bool DeviceEvidenceValid;
        public bool FeedbackEvidenceRetained;
        public bool FeedbackTriageComplete;
        public bool BlockingCrashOrAnrOpen;
        public bool BlockingVisualOrCoreLoopIssueOpen;
    }

    [Serializable]
    public sealed class ClosedTestExecutionValidationResult
    {
        public bool Valid;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ProductionAccessEvidence
    {
        public int SchemaVersion = ClosedTestExecutionRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public bool DeveloperAccountRuleReviewed;
        public bool PersonalAccountCreatedAfter2023November13;
        public int ContinuousOptedInTesterCount;
        public int ContinuousTestDays;
        public bool ClosedTestExecutionEvidenceValid;
        public bool FeedbackTriageReady;
        public bool TestingProcessSummaryReviewed;
        public bool TesterFeedbackSummaryReviewed;
        public bool ChangesMadeSummaryReviewed;
        public bool ProductionReadinessSummaryReviewed;
        public bool ProductionAccessApplicationAuthorized;
        public bool ProductionAccessDecisionRetained;
        public bool ProductionAccessGranted;
    }

    [Serializable]
    public sealed class ProductionAccessEvidenceResult
    {
        public bool MayPrepareApplication;
        public bool MaySubmitApplication;
        public bool MayAdvanceToProductionReleaseReadiness;
        public string[] PrepareBlockers = Array.Empty<string>();
        public string[] SubmitBlockers = Array.Empty<string>();
        public string[] AdvanceBlockers = Array.Empty<string>();
    }
}
