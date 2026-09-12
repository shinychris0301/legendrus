using LegendsOfRus.CommercialCore.Analytics;

namespace LegendsOfRus
{
    /// <summary>
    /// Production-facing analytics gate. The underlying typed analytics facade
    /// remains directly testable, but runtime call sites use this wrapper so no
    /// optional analytics capture is attempted without current legal acceptance
    /// and an explicit analytics grant.
    /// </summary>
    public static class PrivacyAwareAnalytics
    {
        static bool Allowed =>
            PrivacySupportService.Instance != null &&
            PrivacySupportService.Instance.MayUseAnalytics;

        static AnalyticsRecordResult Blocked()
        {
            return AnalyticsRecordResult.Reject(
                AnalyticsReasonCodes.CaptureNotAllowed);
        }

        public static AnalyticsRecordResult TryBeginSession()
        {
            return Allowed ? AnalyticsService.TryBeginSession() : Blocked();
        }

        public static AnalyticsRecordResult TryScreenView(ScreenId screenId)
        {
            return Allowed
                ? AnalyticsService.TryScreenView(screenId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryLevelStart(
            string levelId,
            int attemptNumber,
            bool isRetry)
        {
            return Allowed
                ? AnalyticsService.TryLevelStart(
                    levelId,
                    attemptNumber,
                    isRetry)
                : Blocked();
        }

        public static AnalyticsRecordResult TryLevelComplete(
            string levelId,
            int attemptNumber,
            int score,
            int stars,
            bool firstClear)
        {
            return Allowed
                ? AnalyticsService.TryLevelComplete(
                    levelId,
                    attemptNumber,
                    score,
                    stars,
                    firstClear)
                : Blocked();
        }

        public static AnalyticsRecordResult TryLevelFail(
            string levelId,
            int attemptNumber,
            int score)
        {
            return Allowed
                ? AnalyticsService.TryLevelFail(
                    levelId,
                    attemptNumber,
                    score)
                : Blocked();
        }

        public static AnalyticsRecordResult TryCancelLevelAttempt(
            string levelId,
            int attemptNumber)
        {
            return Allowed
                ? AnalyticsService.TryCancelLevelAttempt(
                    levelId,
                    attemptNumber)
                : Blocked();
        }

        public static AnalyticsRecordResult TryEnergyGateBlocked(
            string levelId,
            int requiredEnergy,
            int availableEnergy)
        {
            return Allowed
                ? AnalyticsService.TryEnergyGateBlocked(
                    levelId,
                    requiredEnergy,
                    availableEnergy)
                : Blocked();
        }

        public static AnalyticsRecordResult TryTutorialStepView(
            string levelId,
            string tutorialId,
            string triggerId)
        {
            return Allowed
                ? AnalyticsService.TryTutorialStepView(
                    levelId,
                    tutorialId,
                    triggerId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryTutorialStepComplete(
            string levelId,
            string tutorialId)
        {
            return Allowed
                ? AnalyticsService.TryTutorialStepComplete(
                    levelId,
                    tutorialId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryStorySceneStart(string sceneId)
        {
            return Allowed
                ? AnalyticsService.TryStorySceneStart(sceneId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryStorySceneComplete(string sceneId)
        {
            return Allowed
                ? AnalyticsService.TryStorySceneComplete(sceneId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryChapterComplete(int chapterId)
        {
            return Allowed
                ? AnalyticsService.TryChapterComplete(chapterId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryChapterCompleteAcknowledged(
            int chapterId)
        {
            return Allowed
                ? AnalyticsService.TryChapterCompleteAcknowledged(chapterId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryPurchaseOfferView(
            string productId,
            string placementId)
        {
            return Allowed
                ? AnalyticsService.TryPurchaseOfferView(
                    productId,
                    placementId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryPurchaseStart(
            string productId,
            string placementId)
        {
            return Allowed
                ? AnalyticsService.TryPurchaseStart(productId, placementId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryPurchaseResult(
            string productId,
            string placementId,
            string outcomeId)
        {
            return Allowed
                ? AnalyticsService.TryPurchaseResult(
                    productId,
                    placementId,
                    outcomeId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryRewardedAdOfferView(
            string placementId,
            string rewardId)
        {
            return Allowed
                ? AnalyticsService.TryRewardedAdOfferView(
                    placementId,
                    rewardId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryRewardedAdStart(
            string placementId,
            string rewardId)
        {
            return Allowed
                ? AnalyticsService.TryRewardedAdStart(
                    placementId,
                    rewardId)
                : Blocked();
        }

        public static AnalyticsRecordResult TryRewardedAdResult(
            string placementId,
            string rewardId,
            string outcomeId,
            bool rewardGranted)
        {
            return Allowed
                ? AnalyticsService.TryRewardedAdResult(
                    placementId,
                    rewardId,
                    outcomeId,
                    rewardGranted)
                : Blocked();
        }
    }
}
