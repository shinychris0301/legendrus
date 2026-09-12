using LegendsOfRus.CommercialCore.Analytics;

namespace LegendsOfRus
{
    /// <summary>
    /// Typed runtime analytics facade. It ships bound to the disabled bridge
    /// and exposes no free-form forwarding or logging API. Only the reviewed
    /// friend composition assembly may replace the process-local bridge.
    /// </summary>
    public static class AnalyticsService
    {
        static readonly object SyncRoot = new object();
        static RuntimeAnalyticsBridge current = RuntimeAnalyticsBridge.Disabled;

        public static bool CaptureAllowed => Current.CaptureAllowed;
        public static bool HasStartedSession => Current.HasStartedSession;
        public static long AcceptedCount => Current.AcceptedCount;

        internal static bool TryInstallTrusted(RuntimeAnalyticsBridge candidate)
        {
            if (candidate == null)
                return false;

            lock (SyncRoot)
            {
                if (object.ReferenceEquals(current, candidate))
                    return true;

                if (current.HasStartedSession)
                    return false;

                current = candidate;
                return true;
            }
        }

        /// <summary>
        /// Friend-only domain lifecycle reset. The reviewed composition invokes
        /// this from SubsystemRegistration before installing any candidate, so
        /// disabled defaults survive Editor domain-reload configurations.
        /// </summary>
        internal static void ResetTrusted()
        {
            lock (SyncRoot)
                current = RuntimeAnalyticsBridge.Disabled;
        }

        public static AnalyticsRecordResult TryBeginSession()
        {
            return Current.TryBeginSession();
        }

        public static AnalyticsRecordResult TryScreenView(ScreenId screenId)
        {
            string canonicalId;
            if (!TryMapScreenId(screenId, out canonicalId))
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.StringValueNotAllowed
                );
            }

            return Current.TryScreenView(canonicalId);
        }

        public static AnalyticsRecordResult TryLevelStart(
            string levelId,
            int attemptNumber,
            bool isRetry
        )
        {
            return Current.TryLevelStart(levelId, attemptNumber, isRetry);
        }

        public static AnalyticsRecordResult TryLevelComplete(
            string levelId,
            int attemptNumber,
            int score,
            int stars,
            bool firstClear
        )
        {
            return Current.TryLevelComplete(
                levelId,
                attemptNumber,
                score,
                stars,
                firstClear
            );
        }

        public static AnalyticsRecordResult TryLevelFail(
            string levelId,
            int attemptNumber,
            int score
        )
        {
            return Current.TryLevelFail(levelId, attemptNumber, score);
        }

        public static AnalyticsRecordResult TryCancelLevelAttempt(
            string levelId,
            int attemptNumber
        )
        {
            return Current.TryCancelLevelAttempt(levelId, attemptNumber);
        }

        public static AnalyticsRecordResult TryEnergyGateBlocked(
            string levelId,
            int requiredEnergy,
            int availableEnergy
        )
        {
            return Current.TryEnergyGateBlocked(
                levelId,
                requiredEnergy,
                availableEnergy
            );
        }

        public static AnalyticsRecordResult TryTutorialStepView(
            string levelId,
            string tutorialId,
            string triggerId
        )
        {
            return Current.TryTutorialStepView(
                levelId,
                tutorialId,
                triggerId
            );
        }

        public static AnalyticsRecordResult TryTutorialStepComplete(
            string levelId,
            string tutorialId
        )
        {
            return Current.TryTutorialStepComplete(levelId, tutorialId);
        }

        public static AnalyticsRecordResult TryStorySceneStart(string sceneId)
        {
            return Current.TryStorySceneStart(sceneId);
        }

        public static AnalyticsRecordResult TryStorySceneComplete(string sceneId)
        {
            return Current.TryStorySceneComplete(sceneId);
        }

        public static AnalyticsRecordResult TryChapterComplete(int chapterId)
        {
            return Current.TryChapterComplete(chapterId);
        }

        public static AnalyticsRecordResult TryChapterCompleteAcknowledged(
            int chapterId
        )
        {
            return Current.TryChapterCompleteAcknowledged(chapterId);
        }

        public static AnalyticsRecordResult TrySandboxEntitlementGrant(
            string sourceId,
            string entitlementId
        )
        {
            return Current.TrySandboxEntitlementGrant(
                sourceId,
                entitlementId
            );
        }

        public static AnalyticsRecordResult TryPurchaseOfferView(
            string productId,
            string placementId
        )
        {
            return Current.TryPurchaseOfferView(productId, placementId);
        }

        public static AnalyticsRecordResult TryPurchaseStart(
            string productId,
            string placementId
        )
        {
            return Current.TryPurchaseStart(productId, placementId);
        }

        public static AnalyticsRecordResult TryPurchaseResult(
            string productId,
            string placementId,
            string outcomeId
        )
        {
            return Current.TryPurchaseResult(
                productId,
                placementId,
                outcomeId
            );
        }

        public static AnalyticsRecordResult TryRewardedAdOfferView(
            string placementId,
            string rewardId
        )
        {
            return Current.TryRewardedAdOfferView(placementId, rewardId);
        }

        public static AnalyticsRecordResult TryRewardedAdStart(
            string placementId,
            string rewardId
        )
        {
            return Current.TryRewardedAdStart(placementId, rewardId);
        }

        public static AnalyticsRecordResult TryRewardedAdResult(
            string placementId,
            string rewardId,
            string outcomeId,
            bool rewardGranted
        )
        {
            return Current.TryRewardedAdResult(
                placementId,
                rewardId,
                outcomeId,
                rewardGranted
            );
        }

        static RuntimeAnalyticsBridge Current
        {
            get
            {
                lock (SyncRoot)
                    return current;
            }
        }

        static bool TryMapScreenId(ScreenId screenId, out string canonicalId)
        {
            switch (screenId)
            {
                case ScreenId.Loading:
                    canonicalId = "loading";
                    return true;
                case ScreenId.Home:
                    canonicalId = "home";
                    return true;
                case ScreenId.WorldMap:
                    canonicalId = "world_map";
                    return true;
                case ScreenId.ChapterMap:
                    canonicalId = "chapter_map";
                    return true;
                case ScreenId.PreLevel:
                    canonicalId = "pre_level";
                    return true;
                case ScreenId.Gameplay:
                    canonicalId = "gameplay";
                    return true;
                case ScreenId.Dialogue:
                    canonicalId = "dialogue";
                    return true;
                case ScreenId.Victory:
                    canonicalId = "victory";
                    return true;
                case ScreenId.Fail:
                    canonicalId = "fail";
                    return true;
                case ScreenId.Shop:
                    canonicalId = "shop";
                    return true;
                case ScreenId.Chapter15:
                    canonicalId = "chapter_15";
                    return true;
                case ScreenId.DailyReward:
                    canonicalId = "daily_reward";
                    return true;
                case ScreenId.RewardClaim:
                    canonicalId = "reward_claim";
                    return true;
                case ScreenId.BoosterSelect:
                    canonicalId = "booster_select";
                    return true;
                case ScreenId.Events:
                    canonicalId = "events";
                    return true;
                case ScreenId.Profile:
                    canonicalId = "profile";
                    return true;
                case ScreenId.Settings:
                    canonicalId = "settings";
                    return true;
                case ScreenId.Pause:
                    canonicalId = "pause";
                    return true;
                case ScreenId.Characters:
                    canonicalId = "characters";
                    return true;
                case ScreenId.Mail:
                    canonicalId = "mail";
                    return true;
                case ScreenId.OutOfEnergy:
                    canonicalId = "out_of_energy";
                    return true;
                case ScreenId.ChapterComplete:
                    canonicalId = "chapter_complete";
                    return true;
                case ScreenId.SplashTitle:
                    canonicalId = "splash_title";
                    return true;
                case ScreenId.EventDetail:
                    canonicalId = "event_detail";
                    return true;
                case ScreenId.IndividualRanking:
                    canonicalId = "individual_ranking";
                    return true;
                case ScreenId.TeamHub:
                    canonicalId = "team_hub";
                    return true;
                case ScreenId.TeamRanking:
                    canonicalId = "team_ranking";
                    return true;
                case ScreenId.TeamChat:
                    canonicalId = "team_chat";
                    return true;
                case ScreenId.FriendsList:
                    canonicalId = "friends_list";
                    return true;
                case ScreenId.Inventory:
                    canonicalId = "inventory";
                    return true;
                case ScreenId.SpecialOffer:
                    canonicalId = "special_offer";
                    return true;
                case ScreenId.LanguageSelect:
                    canonicalId = "language_select";
                    return true;
                case ScreenId.Login:
                    canonicalId = "login";
                    return true;
                case ScreenId.TutorialOverlay:
                    canonicalId = "tutorial_overlay";
                    return true;
                case ScreenId.OfflineState:
                    canonicalId = "offline_state";
                    return true;
                case ScreenId.MaintenanceState:
                    canonicalId = "maintenance_state";
                    return true;
                case ScreenId.EmptyErrorState:
                    canonicalId = "empty_error_state";
                    return true;
                default:
                    canonicalId = string.Empty;
                    return false;
            }
        }
    }
}
