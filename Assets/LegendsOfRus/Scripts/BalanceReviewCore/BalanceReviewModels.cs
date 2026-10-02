using System;

namespace LegendsOfRus.BalanceReview
{
    public enum BalanceReviewIssueCode
    {
        None = 0,
        InsufficientSamples = 1,
        HighFailureRate = 2,
        NearPerfectWinRate = 3,
        LowWinningMoveMargin = 4,
        TutorialCompletionDrop = 5
    }

    [Serializable]
    public struct LevelBalanceSummary
    {
        public int LevelNumber;
        public int Attempts;
        public int Wins;
        public int Failures;
        public int Retries;
        public int TotalWinningMovesRemaining;
        public int TotalWinningMoveLimit;
        public long TotalWinningScore;

        public float WinRate => Attempts > 0 ? (float)Wins / Attempts : 0f;
        public float FailureRate => Attempts > 0 ? (float)Failures / Attempts : 0f;
        public float RetryRate => Attempts > 0 ? (float)Retries / Attempts : 0f;
        public float AverageWinningMovesRemaining => Wins > 0
            ? (float)TotalWinningMovesRemaining / Wins
            : 0f;
        public float AverageWinningMoveMargin => Wins > 0 && TotalWinningMoveLimit > 0
            ? (float)TotalWinningMovesRemaining / TotalWinningMoveLimit
            : 0f;
        public float AverageWinningScore => Wins > 0
            ? (float)TotalWinningScore / Wins
            : 0f;
    }

    [Serializable]
    public struct TutorialFunnelSummary
    {
        public int LevelNumber;
        public int TutorialKey;
        public int Shown;
        public int Completed;
        public float CompletionRate => Shown > 0 ? (float)Completed / Shown : 0f;
    }

    [Serializable]
    public struct BalanceReviewIssue
    {
        public BalanceReviewIssueCode Code;
        public int LevelNumber;
        public int TutorialKey;
        public int SampleCount;
        public float ObservedRate;

        public BalanceReviewIssue(
            BalanceReviewIssueCode code,
            int levelNumber,
            int tutorialKey,
            int sampleCount,
            float observedRate)
        {
            Code = code;
            LevelNumber = levelNumber;
            TutorialKey = tutorialKey;
            SampleCount = sampleCount;
            ObservedRate = observedRate;
        }
    }

    [Serializable]
    public sealed class BalanceReviewReport
    {
        public LevelBalanceSummary[] Levels = Array.Empty<LevelBalanceSummary>();
        public TutorialFunnelSummary[] Tutorials = Array.Empty<TutorialFunnelSummary>();
        public BalanceReviewIssue[] Issues = Array.Empty<BalanceReviewIssue>();
        public int ObservationCount;
    }
}
