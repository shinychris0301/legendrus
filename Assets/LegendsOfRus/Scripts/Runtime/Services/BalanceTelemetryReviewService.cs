using LegendsOfRus.BalanceReview;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Closed-test-only, local balance/tutorial observation service. It stores
    /// no free-form strings, device/account identifiers, or durable data.
    /// Production analytics remains a separate consent-gated path.
    /// </summary>
    public sealed class BalanceTelemetryReviewService : MonoBehaviour
    {
        public static BalanceTelemetryReviewService Instance { get; private set; }

        BalanceTelemetryReviewActivationSnapshot activation;
        BalanceObservationBuffer observations;

        public BalanceTelemetryReviewActivationSnapshot Activation => activation;
        public int ObservationCount => observations != null ? observations.Count : 0;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            activation = BalanceTelemetryReviewActivation.Load();
            observations = activation.LocalObservationBufferApproved
                ? new BalanceObservationBuffer()
                : null;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool TryRecordLevelStart(LevelData level, int attemptNumber)
        {
            return level != null && observations != null && observations.TryRecord(
                BalanceObservationCode.LevelStarted,
                level.levelId,
                attemptNumber,
                0,
                level.moveLimit,
                0,
                0);
        }

        public bool TryRecordLevelWin(
            LevelData level,
            int attemptNumber,
            int movesRemaining,
            int score)
        {
            return level != null && observations != null && observations.TryRecord(
                BalanceObservationCode.LevelWon,
                level.levelId,
                attemptNumber,
                Mathf.Max(0, movesRemaining),
                Mathf.Max(0, level.moveLimit),
                Mathf.Max(0, score),
                0);
        }

        public bool TryRecordLevelFail(
            LevelData level,
            int attemptNumber,
            int score)
        {
            return level != null && observations != null && observations.TryRecord(
                BalanceObservationCode.LevelFailed,
                level.levelId,
                attemptNumber,
                0,
                Mathf.Max(0, level.moveLimit),
                Mathf.Max(0, score),
                0);
        }

        public bool TryRecordTutorialShown(LevelData level, string tutorialId)
        {
            int tutorialKey = StableReviewKey.FromAuthoredId(tutorialId);
            return level != null && observations != null && observations.TryRecord(
                BalanceObservationCode.TutorialShown,
                level.levelId,
                0,
                0,
                0,
                0,
                tutorialKey);
        }

        public bool TryRecordTutorialCompleted(LevelData level, string tutorialId)
        {
            int tutorialKey = StableReviewKey.FromAuthoredId(tutorialId);
            return level != null && observations != null && observations.TryRecord(
                BalanceObservationCode.TutorialCompleted,
                level.levelId,
                0,
                0,
                0,
                0,
                tutorialKey);
        }

        public BalanceObservation[] CopyObservations()
        {
            return observations != null
                ? observations.Snapshot()
                : new BalanceObservation[0];
        }

        public BalanceReviewReport BuildLocalReview()
        {
            if (!activation.LocalReviewReportApproved || observations == null)
                return new BalanceReviewReport();
            return BalanceReviewAggregator.Build(observations.Snapshot());
        }
    }
}
