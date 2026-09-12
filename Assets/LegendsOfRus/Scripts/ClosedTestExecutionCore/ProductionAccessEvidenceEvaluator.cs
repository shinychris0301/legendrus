using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestExecution
{
    public static class ProductionAccessEvidenceEvaluator
    {
        public static ProductionAccessEvidenceResult Evaluate(ProductionAccessEvidence evidence)
        {
            var prepare = new List<string>();
            Require(evidence.ClosedTestExecutionEvidenceValid, ClosedTestExecutionBlockerCodes.ExecutionEvidenceMissing, prepare);
            Require(evidence.FeedbackTriageReady, ClosedTestExecutionBlockerCodes.FeedbackNotFullyTriaged, prepare);
            Require(evidence.DeveloperAccountRuleReviewed, ClosedTestExecutionBlockerCodes.AccountRuleNotReviewed, prepare);
            if (evidence.DeveloperAccountRuleReviewed && evidence.PersonalAccountCreatedAfter2023November13)
            {
                Require(evidence.ContinuousOptedInTesterCount >= ClosedTestExecutionRequirements.PersonalAccountProductionAccessTesterCount, ClosedTestExecutionBlockerCodes.TesterContinuityInsufficient, prepare);
                Require(evidence.ContinuousTestDays >= ClosedTestExecutionRequirements.PersonalAccountProductionAccessContinuousDays, ClosedTestExecutionBlockerCodes.TesterContinuityInsufficient, prepare);
            }
            Require(evidence.TestingProcessSummaryReviewed, ClosedTestExecutionBlockerCodes.TestingSummaryMissing, prepare);
            Require(evidence.TesterFeedbackSummaryReviewed, ClosedTestExecutionBlockerCodes.FeedbackSummaryMissing, prepare);
            Require(evidence.ChangesMadeSummaryReviewed, ClosedTestExecutionBlockerCodes.ChangesSummaryMissing, prepare);
            Require(evidence.ProductionReadinessSummaryReviewed, ClosedTestExecutionBlockerCodes.ReadinessSummaryMissing, prepare);

            var submit = new List<string>(prepare);
            Require(evidence.ProductionAccessApplicationAuthorized, ClosedTestExecutionBlockerCodes.ProductionApplicationNotAuthorized, submit);

            var advance = new List<string>(submit);
            Require(evidence.ProductionAccessDecisionRetained, ClosedTestExecutionBlockerCodes.ProductionAccessDecisionMissing, advance);
            Require(evidence.ProductionAccessGranted, ClosedTestExecutionBlockerCodes.ProductionAccessNotGranted, advance);

            return new ProductionAccessEvidenceResult
            {
                MayPrepareApplication = prepare.Count == 0,
                MaySubmitApplication = submit.Count == 0,
                MayAdvanceToProductionReleaseReadiness = advance.Count == 0,
                PrepareBlockers = prepare.ToArray(),
                SubmitBlockers = submit.ToArray(),
                AdvanceBlockers = advance.ToArray()
            };
        }

        static void Require(bool value, string blocker, List<string> blockers)
        {
            if (!value) blockers.Add(blocker);
        }
    }
}
