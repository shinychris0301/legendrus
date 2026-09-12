using LegendsOfRus.BalanceReview;
using NUnit.Framework;

namespace LegendsOfRus.BalanceReview.Tests
{
    public sealed class BalanceTelemetryTutorialFunnelEditModeTests
    {
        [Test]
        public void BufferRejectsInvalidLevel()
        {
            var buffer = new BalanceObservationBuffer();
            Assert.That(buffer.TryRecord(BalanceObservationCode.LevelStarted, 0, 1), Is.False);
        }

        [Test]
        public void BufferRejectsLevelOutcomeWithoutAttempt()
        {
            var buffer = new BalanceObservationBuffer();
            Assert.That(buffer.TryRecord(BalanceObservationCode.LevelWon, 1), Is.False);
        }

        [Test]
        public void BufferRejectsTutorialWithoutKey()
        {
            var buffer = new BalanceObservationBuffer();
            Assert.That(buffer.TryRecord(BalanceObservationCode.TutorialShown, 1), Is.False);
        }

        [Test]
        public void BufferIsBoundedAndOrdered()
        {
            var buffer = new BalanceObservationBuffer(3);
            for (int i = 1; i <= 6; i++)
                Assert.That(buffer.TryRecord(BalanceObservationCode.LevelStarted, i, 1), Is.True);
            BalanceObservation[] snapshot = buffer.Snapshot();
            Assert.That(snapshot.Length, Is.EqualTo(3));
            Assert.That(snapshot[0].LevelNumber, Is.EqualTo(4));
            Assert.That(snapshot[2].LevelNumber, Is.EqualTo(6));
        }

        [Test]
        public void StableReviewKeyIsDeterministic()
        {
            int a = StableReviewKey.FromAuthoredId("tutorial.match.start");
            int b = StableReviewKey.FromAuthoredId("tutorial.match.start");
            Assert.That(a, Is.Not.EqualTo(0));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void AggregateComputesWinAndRetryRate()
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, 2, attempt);
                buffer.TryRecord(
                    attempt <= 3 ? BalanceObservationCode.LevelWon : BalanceObservationCode.LevelFailed,
                    2, attempt, attempt <= 3 ? 5 : 0, 25, 1000 + attempt);
            }
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(report.Levels.Length, Is.EqualTo(1));
            Assert.That(report.Levels[0].Attempts, Is.EqualTo(5));
            Assert.That(report.Levels[0].Wins, Is.EqualTo(3));
            Assert.That(report.Levels[0].Retries, Is.EqualTo(4));
            Assert.That(report.Levels[0].WinRate, Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void AggregateComputesWinningMoveMargin()
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, 3, attempt);
                buffer.TryRecord(BalanceObservationCode.LevelWon, 3, attempt, 5, 20, 1000);
            }
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot(), 3);
            Assert.That(report.Levels[0].AverageWinningMovesRemaining, Is.EqualTo(5f));
            Assert.That(report.Levels[0].AverageWinningMoveMargin, Is.EqualTo(0.25f).Within(0.001f));
        }

        [Test]
        public void ReviewFlagsHighFailureRate()
        {
            var buffer = BuildLevel(4, 10, 2, 2, 25);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(HasIssue(report, BalanceReviewIssueCode.HighFailureRate), Is.True);
        }

        [Test]
        public void ReviewFlagsNearPerfectWinRate()
        {
            var buffer = BuildLevel(5, 20, 20, 6, 25);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(HasIssue(report, BalanceReviewIssueCode.NearPerfectWinRate), Is.True);
        }

        [Test]
        public void ReviewFlagsLowWinningMoveMargin()
        {
            var buffer = BuildLevel(6, 6, 6, 1, 25);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(HasIssue(report, BalanceReviewIssueCode.LowWinningMoveMargin), Is.True);
        }

        [Test]
        public void TutorialFunnelComputesCompletionRate()
        {
            var buffer = new BalanceObservationBuffer();
            const int key = 1234;
            for (int i = 0; i < 5; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialShown, 1, tutorialKey: key);
            for (int i = 0; i < 4; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialCompleted, 1, tutorialKey: key);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(report.Tutorials.Length, Is.EqualTo(1));
            Assert.That(report.Tutorials[0].CompletionRate, Is.EqualTo(0.8f).Within(0.001f));
        }

        [Test]
        public void TutorialFunnelFlagsDropOffBelowThreshold()
        {
            var buffer = new BalanceObservationBuffer();
            const int key = 5678;
            for (int i = 0; i < 5; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialShown, 1, tutorialKey: key);
            for (int i = 0; i < 2; i++)
                buffer.TryRecord(BalanceObservationCode.TutorialCompleted, 1, tutorialKey: key);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(HasIssue(report, BalanceReviewIssueCode.TutorialCompletionDrop), Is.True);
        }

        [Test]
        public void InsufficientSamplesAreFlaggedWithoutDifficultyJudgment()
        {
            var buffer = BuildLevel(7, 2, 0, 0, 25);
            BalanceReviewReport report = BalanceReviewAggregator.Build(buffer.Snapshot());
            Assert.That(HasIssue(report, BalanceReviewIssueCode.InsufficientSamples), Is.True);
            Assert.That(HasIssue(report, BalanceReviewIssueCode.HighFailureRate), Is.False);
        }

        static BalanceObservationBuffer BuildLevel(
            int levelNumber,
            int attempts,
            int wins,
            int winningMovesRemaining,
            int moveLimit)
        {
            var buffer = new BalanceObservationBuffer();
            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                buffer.TryRecord(BalanceObservationCode.LevelStarted, levelNumber, attempt);
                if (attempt <= wins)
                    buffer.TryRecord(BalanceObservationCode.LevelWon, levelNumber, attempt,
                        winningMovesRemaining, moveLimit, 1000);
                else
                    buffer.TryRecord(BalanceObservationCode.LevelFailed, levelNumber, attempt,
                        0, moveLimit, 500);
            }
            return buffer;
        }

        static bool HasIssue(BalanceReviewReport report, BalanceReviewIssueCode code)
        {
            for (int i = 0; i < report.Issues.Length; i++)
                if (report.Issues[i].Code == code)
                    return true;
            return false;
        }
    }
}
