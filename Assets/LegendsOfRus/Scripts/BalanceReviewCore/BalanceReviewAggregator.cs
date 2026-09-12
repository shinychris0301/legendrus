using System;
using System.Collections.Generic;

namespace LegendsOfRus.BalanceReview
{
    public static class BalanceReviewAggregator
    {
        const int DefaultMinimumLevelSamples = 5;
        const int DefaultMinimumTutorialShows = 3;

        public static BalanceReviewReport Build(
            BalanceObservation[] observations,
            int minimumLevelSamples = DefaultMinimumLevelSamples,
            int minimumTutorialShows = DefaultMinimumTutorialShows)
        {
            observations = observations ?? Array.Empty<BalanceObservation>();
            if (minimumLevelSamples < 1)
                minimumLevelSamples = 1;
            if (minimumTutorialShows < 1)
                minimumTutorialShows = 1;

            var levelMap = new SortedDictionary<int, LevelBalanceSummary>();
            var tutorialMap = new SortedDictionary<long, TutorialFunnelSummary>();

            for (int i = 0; i < observations.Length; i++)
            {
                BalanceObservation o = observations[i];
                if (o.LevelNumber < 1)
                    continue;

                if (o.Code == BalanceObservationCode.LevelStarted ||
                    o.Code == BalanceObservationCode.LevelWon ||
                    o.Code == BalanceObservationCode.LevelFailed)
                {
                    LevelBalanceSummary level;
                    levelMap.TryGetValue(o.LevelNumber, out level);
                    level.LevelNumber = o.LevelNumber;
                    if (o.Code == BalanceObservationCode.LevelStarted)
                    {
                        level.Attempts++;
                        if (o.AttemptNumber > 1)
                            level.Retries++;
                    }
                    else if (o.Code == BalanceObservationCode.LevelWon)
                    {
                        level.Wins++;
                        level.TotalWinningMovesRemaining += o.MovesRemaining;
                        level.TotalWinningMoveLimit += o.MoveLimit;
                        level.TotalWinningScore += o.Score;
                    }
                    else
                    {
                        level.Failures++;
                    }
                    levelMap[o.LevelNumber] = level;
                }

                if (o.Code == BalanceObservationCode.TutorialShown ||
                    o.Code == BalanceObservationCode.TutorialCompleted)
                {
                    long key = ((long)o.LevelNumber << 32) | (uint)o.TutorialKey;
                    TutorialFunnelSummary tutorial;
                    tutorialMap.TryGetValue(key, out tutorial);
                    tutorial.LevelNumber = o.LevelNumber;
                    tutorial.TutorialKey = o.TutorialKey;
                    if (o.Code == BalanceObservationCode.TutorialShown)
                        tutorial.Shown++;
                    else
                        tutorial.Completed++;
                    tutorialMap[key] = tutorial;
                }
            }

            var levels = new List<LevelBalanceSummary>(levelMap.Values);
            var tutorials = new List<TutorialFunnelSummary>(tutorialMap.Values);
            var issues = new List<BalanceReviewIssue>();

            for (int i = 0; i < levels.Count; i++)
            {
                LevelBalanceSummary level = levels[i];
                if (level.Attempts < minimumLevelSamples)
                {
                    issues.Add(new BalanceReviewIssue(
                        BalanceReviewIssueCode.InsufficientSamples,
                        level.LevelNumber, 0, level.Attempts, level.WinRate));
                    continue;
                }

                if (level.FailureRate >= 0.65f)
                {
                    issues.Add(new BalanceReviewIssue(
                        BalanceReviewIssueCode.HighFailureRate,
                        level.LevelNumber, 0, level.Attempts, level.FailureRate));
                }
                else if (level.WinRate >= 0.95f)
                {
                    issues.Add(new BalanceReviewIssue(
                        BalanceReviewIssueCode.NearPerfectWinRate,
                        level.LevelNumber, 0, level.Attempts, level.WinRate));
                }

                if (level.Wins >= 3 && level.AverageWinningMoveMargin <= 0.08f)
                {
                    issues.Add(new BalanceReviewIssue(
                        BalanceReviewIssueCode.LowWinningMoveMargin,
                        level.LevelNumber, 0, level.Wins, level.AverageWinningMoveMargin));
                }
            }

            for (int i = 0; i < tutorials.Count; i++)
            {
                TutorialFunnelSummary tutorial = tutorials[i];
                if (tutorial.Shown >= minimumTutorialShows &&
                    tutorial.CompletionRate < 0.80f)
                {
                    issues.Add(new BalanceReviewIssue(
                        BalanceReviewIssueCode.TutorialCompletionDrop,
                        tutorial.LevelNumber,
                        tutorial.TutorialKey,
                        tutorial.Shown,
                        tutorial.CompletionRate));
                }
            }

            return new BalanceReviewReport
            {
                Levels = levels.ToArray(),
                Tutorials = tutorials.ToArray(),
                Issues = issues.ToArray(),
                ObservationCount = observations.Length
            };
        }
    }
}
