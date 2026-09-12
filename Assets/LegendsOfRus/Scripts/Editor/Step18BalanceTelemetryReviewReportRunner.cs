#if UNITY_EDITOR
using System.Text;
using LegendsOfRus.BalanceReview;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18BalanceTelemetryReviewReportRunner
    {
        [MenuItem("Legends of Rus/QA/Print v1.18 Step 2 Local Balance Review")]
        public static void Print()
        {
            BalanceTelemetryReviewService service = BalanceTelemetryReviewService.Instance;
            if (!Application.isPlaying || service == null)
            {
                Debug.LogWarning(
                    "v1.18 Step 2 local balance review is process-memory only. " +
                    "Enter Play Mode and exercise levels/tutorials before printing it.");
                return;
            }

            BalanceReviewReport report = service.BuildLocalReview();
            var text = new StringBuilder();
            text.AppendLine("v1.18 Step 2 local balance/tutorial review");
            text.AppendLine("observations=" + report.ObservationCount);
            for (int i = 0; i < report.Levels.Length; i++)
            {
                LevelBalanceSummary level = report.Levels[i];
                text.Append("level=").Append(level.LevelNumber)
                    .Append(" attempts=").Append(level.Attempts)
                    .Append(" wins=").Append(level.Wins)
                    .Append(" failures=").Append(level.Failures)
                    .Append(" retries=").Append(level.Retries)
                    .Append(" winRate=").Append(level.WinRate.ToString("0.000"))
                    .Append(" avgWinMoves=").Append(level.AverageWinningMovesRemaining.ToString("0.00"))
                    .Append(" moveMargin=").Append(level.AverageWinningMoveMargin.ToString("0.000"))
                    .AppendLine();
            }
            for (int i = 0; i < report.Tutorials.Length; i++)
            {
                TutorialFunnelSummary tutorial = report.Tutorials[i];
                text.Append("tutorial level=").Append(tutorial.LevelNumber)
                    .Append(" key=").Append(tutorial.TutorialKey)
                    .Append(" shown=").Append(tutorial.Shown)
                    .Append(" completed=").Append(tutorial.Completed)
                    .Append(" completionRate=").Append(tutorial.CompletionRate.ToString("0.000"))
                    .AppendLine();
            }
            for (int i = 0; i < report.Issues.Length; i++)
            {
                BalanceReviewIssue issue = report.Issues[i];
                text.Append("reviewIssue=").Append(issue.Code)
                    .Append(" level=").Append(issue.LevelNumber)
                    .Append(" tutorialKey=").Append(issue.TutorialKey)
                    .Append(" samples=").Append(issue.SampleCount)
                    .Append(" observed=").Append(issue.ObservedRate.ToString("0.000"))
                    .AppendLine();
            }
            Debug.Log(text.ToString());
        }
    }
}
#endif
