using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestExecution
{
    public static class FeedbackTriageEvaluator
    {
        public static FeedbackTriageSummary Evaluate(ClosedTestFeedbackItem[] items)
        {
            items = items ?? new ClosedTestFeedbackItem[0];
            var blockers = new List<string>();
            var seen = new HashSet<string>();
            var result = new FeedbackTriageSummary { TotalItems = items.Length };

            if (items.Length < ClosedTestExecutionRequirements.MinimumFeedbackItemsForReview)
                blockers.Add(ClosedTestExecutionBlockerCodes.FeedbackEvidenceMissing);
            if (items.Length > ClosedTestExecutionRequirements.MaximumFeedbackItems)
                blockers.Add("CTE_FEEDBACK_ITEM_LIMIT_EXCEEDED");

            for (int i = 0; i < items.Length; i++)
            {
                ClosedTestFeedbackItem item = items[i];
                if (item == null) { blockers.Add("CTE_NULL_FEEDBACK_ITEM"); continue; }
                if (!IsIssueId(item.IssueId)) blockers.Add(ClosedTestExecutionBlockerCodes.InvalidIssueId);
                else if (!seen.Add(item.IssueId)) blockers.Add(ClosedTestExecutionBlockerCodes.DuplicateIssueId);
                if (item.SchemaVersion != ClosedTestExecutionRequirements.SchemaVersion) blockers.Add("CTE_FEEDBACK_SCHEMA_UNSUPPORTED");
                if (item.Category == FeedbackCategory.Unknown) blockers.Add("CTE_FEEDBACK_CATEGORY_UNKNOWN");
                if (item.Reviewed) result.ReviewedItems++;
                if (item.Disposition == FeedbackDisposition.Fixed) result.FixedItems++;
                if (item.Disposition == FeedbackDisposition.Deferred) result.DeferredItems++;
                if (item.Disposition == FeedbackDisposition.Open)
                {
                    result.OpenItems++;
                    if (item.Severity == FeedbackSeverity.Critical) result.CriticalOpenItems++;
                    if (item.Severity == FeedbackSeverity.High) result.HighOpenItems++;
                    if (item.BlocksProduction) result.BlockingOpenItems++;
                }
                if (item.Disposition == FeedbackDisposition.Fixed && item.RetestRequired && !item.RetestPassed)
                    result.FixedItemsMissingRetest++;
            }

            result.AllReviewed = result.TotalItems > 0 && result.ReviewedItems == result.TotalItems;
            if (!result.AllReviewed) blockers.Add(ClosedTestExecutionBlockerCodes.FeedbackNotFullyTriaged);
            if (result.BlockingOpenItems > 0) blockers.Add(ClosedTestExecutionBlockerCodes.BlockingFeedbackOpen);
            if (result.CriticalOpenItems > 0) blockers.Add(ClosedTestExecutionBlockerCodes.CriticalFeedbackOpen);
            if (result.HighOpenItems > 0) blockers.Add(ClosedTestExecutionBlockerCodes.HighFeedbackOpen);
            if (result.FixedItemsMissingRetest > 0) blockers.Add(ClosedTestExecutionBlockerCodes.FixedItemNotRetested);

            result.Blockers = blockers.ToArray();
            result.ReadyForProductionAccessReview = blockers.Count == 0;
            return result;
        }

        public static bool IsIssueId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(ClosedTestExecutionRequirements.IssuePrefix)) return false;
            string suffix = value.Substring(ClosedTestExecutionRequirements.IssuePrefix.Length);
            if (suffix.Length != 4) return false;
            for (int i = 0; i < suffix.Length; i++) if (!char.IsDigit(suffix[i])) return false;
            return suffix != "0000";
        }
    }
}
