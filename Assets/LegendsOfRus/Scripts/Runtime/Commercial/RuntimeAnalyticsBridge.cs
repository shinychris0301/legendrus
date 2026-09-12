using System;
using LegendsOfRus.CommercialCore.Analytics;

namespace LegendsOfRus
{
    /// <summary>
    /// Typed runtime seam over the engine-free analytics pipeline. The bridge
    /// owns one process-local funnel and contains every adapter or clock
    /// failure as a reason-coded result. It has no upload, persistence, player
    /// identity, or free-form event forwarding path.
    /// </summary>
    public sealed class RuntimeAnalyticsBridge
    {
        static readonly Func<long> DisabledClock = () => 1L;
        static readonly RuntimeAnalyticsBridge disabled =
            new RuntimeAnalyticsBridge(
                false,
                DisabledAnalyticsAdapter.Instance,
                "runtime_session",
                DisabledClock
            );

        readonly object syncRoot = new object();
        readonly bool captureAllowed;
        readonly string sessionId;
        readonly Func<long> utcNowTicks;
        readonly AnalyticsPipeline pipeline;
        readonly AnalyticsFunnelTracker funnel;

        public static RuntimeAnalyticsBridge Disabled => disabled;
        public bool CaptureAllowed => captureAllowed;
        public bool HasStartedSession => pipeline.HasStartedSession;
        public long AcceptedCount => pipeline.AcceptedCount;
        public bool HasActiveLevelAttempt => funnel.HasActiveAttempt;
        public string ActiveLevelAttemptKey => funnel.ActiveAttemptKey;
        public int TrackedLevelAttemptCount => funnel.TrackedAttemptCount;
        public int AcceptedLevelTerminalCount => funnel.AcceptedTerminalCount;

        public RuntimeAnalyticsBridge(
            bool captureAllowed,
            IAnalyticsAdapter adapter,
            string sessionId,
            Func<long> utcNowTicks
        )
        {
            this.captureAllowed = captureAllowed;
            this.sessionId = sessionId ?? string.Empty;
            this.utcNowTicks = utcNowTicks;
            pipeline = new AnalyticsPipeline(captureAllowed, adapter);
            funnel = new AnalyticsFunnelTracker();
        }

        public AnalyticsRecordResult TryBeginSession()
        {
            lock (syncRoot)
            {
                if (!captureAllowed)
                {
                    return pipeline.TryBeginSession(sessionId, 1L);
                }

                AnalyticsRecordResult clockFailure;
                long ticks;
                if (!TryReadClock(out ticks, out clockFailure))
                    return clockFailure;

                return pipeline.TryBeginSession(sessionId, ticks);
            }
        }

        public AnalyticsRecordResult TryScreenView(string screenId)
        {
            return TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.ScreenId,
                        screenId
                    )
                )
            );
        }

        public AnalyticsRecordResult TryLevelStart(
            string levelId,
            int attemptNumber,
            bool isRetry
        )
        {
            var draft = new AnalyticsEventDraft(
                AnalyticsEventIds.LevelStart,
                AnalyticsAttribute.String(
                    AnalyticsAttributeKeys.LevelId,
                    levelId
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.AttemptNumber,
                    attemptNumber
                ),
                AnalyticsAttribute.Boolean(
                    AnalyticsAttributeKeys.IsRetry,
                    isRetry
                )
            );
            return TryBeginLevelAttempt(draft, levelId, attemptNumber);
        }

        public AnalyticsRecordResult TryLevelComplete(
            string levelId,
            int attemptNumber,
            int score,
            int stars,
            bool firstClear
        )
        {
            var draft = new AnalyticsEventDraft(
                AnalyticsEventIds.LevelComplete,
                AnalyticsAttribute.String(
                    AnalyticsAttributeKeys.LevelId,
                    levelId
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.AttemptNumber,
                    attemptNumber
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.Score,
                    score
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.Stars,
                    stars
                ),
                AnalyticsAttribute.Boolean(
                    AnalyticsAttributeKeys.FirstClear,
                    firstClear
                )
            );
            return TryFinishLevelAttempt(
                draft,
                levelId,
                attemptNumber,
                true
            );
        }

        public AnalyticsRecordResult TryLevelFail(
            string levelId,
            int attemptNumber,
            int score
        )
        {
            var draft = new AnalyticsEventDraft(
                AnalyticsEventIds.LevelFail,
                AnalyticsAttribute.String(
                    AnalyticsAttributeKeys.LevelId,
                    levelId
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.AttemptNumber,
                    attemptNumber
                ),
                AnalyticsAttribute.Integer(
                    AnalyticsAttributeKeys.Score,
                    score
                )
            );
            return TryFinishLevelAttempt(
                draft,
                levelId,
                attemptNumber,
                false
            );
        }

        public AnalyticsRecordResult TryCancelLevelAttempt(
            string levelId,
            int attemptNumber
        )
        {
            lock (syncRoot)
            {
                string attemptKey;
                if (!AnalyticsAttemptKey.TryCreate(
                    levelId,
                    attemptNumber,
                    out attemptKey
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                return funnel.TryCancelActiveLevelAttempt(attemptKey);
            }
        }

        public AnalyticsRecordResult TryEnergyGateBlocked(
            string levelId,
            int requiredEnergy,
            int availableEnergy
        )
        {
            return TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.EnergyGateBlocked,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.LevelId,
                        levelId
                    ),
                    AnalyticsAttribute.Integer(
                        AnalyticsAttributeKeys.RequiredEnergy,
                        requiredEnergy
                    ),
                    AnalyticsAttribute.Integer(
                        AnalyticsAttributeKeys.AvailableEnergy,
                        availableEnergy
                    )
                )
            );
        }

        public AnalyticsRecordResult TryTutorialStepView(
            string levelId,
            string tutorialId,
            string triggerId
        )
        {
            return TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.TutorialStepView,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.LevelId,
                        levelId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.TutorialId,
                        tutorialId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.TriggerId,
                        triggerId
                    )
                )
            );
        }

        public AnalyticsRecordResult TryTutorialStepComplete(
            string levelId,
            string tutorialId
        )
        {
            return TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.TutorialStepComplete,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.LevelId,
                        levelId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.TutorialId,
                        tutorialId
                    )
                )
            );
        }

        public AnalyticsRecordResult TryStorySceneStart(string sceneId)
        {
            return TryRecord(
                SingleStringDraft(
                    AnalyticsEventIds.StorySceneStart,
                    AnalyticsAttributeKeys.SceneId,
                    sceneId
                )
            );
        }

        public AnalyticsRecordResult TryStorySceneComplete(string sceneId)
        {
            return TryRecord(
                SingleStringDraft(
                    AnalyticsEventIds.StorySceneComplete,
                    AnalyticsAttributeKeys.SceneId,
                    sceneId
                )
            );
        }

        public AnalyticsRecordResult TryChapterComplete(int chapterId)
        {
            return TryRecord(
                SingleIntegerDraft(
                    AnalyticsEventIds.ChapterComplete,
                    AnalyticsAttributeKeys.ChapterId,
                    chapterId
                )
            );
        }

        public AnalyticsRecordResult TryChapterCompleteAcknowledged(
            int chapterId
        )
        {
            return TryRecord(
                SingleIntegerDraft(
                    AnalyticsEventIds.ChapterCompleteAcknowledged,
                    AnalyticsAttributeKeys.ChapterId,
                    chapterId
                )
            );
        }

        public AnalyticsRecordResult TrySandboxEntitlementGrant(
            string sourceId,
            string entitlementId
        )
        {
            return TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.SandboxEntitlementGrant,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.SourceId,
                        sourceId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.EntitlementId,
                        entitlementId
                    )
                )
            );
        }

        public AnalyticsRecordResult TryPurchaseOfferView(
            string productId,
            string placementId
        )
        {
            return TryPurchaseEvent(
                AnalyticsEventIds.PurchaseOfferView,
                productId,
                placementId,
                string.Empty
            );
        }

        public AnalyticsRecordResult TryPurchaseStart(
            string productId,
            string placementId
        )
        {
            return TryPurchaseEvent(
                AnalyticsEventIds.PurchaseStart,
                productId,
                placementId,
                string.Empty
            );
        }

        public AnalyticsRecordResult TryPurchaseResult(
            string productId,
            string placementId,
            string outcomeId
        )
        {
            return TryPurchaseEvent(
                AnalyticsEventIds.PurchaseResult,
                productId,
                placementId,
                outcomeId
            );
        }

        public AnalyticsRecordResult TryRewardedAdOfferView(
            string placementId,
            string rewardId
        )
        {
            return TryRewardedAdEvent(
                AnalyticsEventIds.RewardedAdOfferView,
                placementId,
                rewardId,
                string.Empty,
                false
            );
        }

        public AnalyticsRecordResult TryRewardedAdStart(
            string placementId,
            string rewardId
        )
        {
            return TryRewardedAdEvent(
                AnalyticsEventIds.RewardedAdStart,
                placementId,
                rewardId,
                string.Empty,
                false
            );
        }

        public AnalyticsRecordResult TryRewardedAdResult(
            string placementId,
            string rewardId,
            string outcomeId,
            bool rewardGranted
        )
        {
            return TryRewardedAdEvent(
                AnalyticsEventIds.RewardedAdResult,
                placementId,
                rewardId,
                outcomeId,
                rewardGranted
            );
        }

        AnalyticsRecordResult TryBeginLevelAttempt(
            AnalyticsEventDraft draft,
            string levelId,
            int attemptNumber
        )
        {
            AnalyticsRecordResult validation =
                AnalyticsEventValidator.ValidateDraft(draft);
            if (!validation.Accepted)
                return validation;

            lock (syncRoot)
            {
                AnalyticsRecordResult clockFailure;
                long ticks;
                if (!TryReadClockOrDisabled(out ticks, out clockFailure))
                    return clockFailure;

                if (!pipeline.HasStartedSession)
                    return pipeline.TryRecord(draft, ticks);

                string attemptKey;
                if (!AnalyticsAttemptKey.TryCreate(
                    levelId,
                    attemptNumber,
                    out attemptKey
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                return funnel.TryBeginLevelAttempt(
                    attemptKey,
                    () => pipeline.TryRecord(draft, ticks)
                );
            }
        }

        AnalyticsRecordResult TryFinishLevelAttempt(
            AnalyticsEventDraft draft,
            string levelId,
            int attemptNumber,
            bool complete
        )
        {
            AnalyticsRecordResult validation =
                AnalyticsEventValidator.ValidateDraft(draft);
            if (!validation.Accepted)
                return validation;

            lock (syncRoot)
            {
                AnalyticsRecordResult clockFailure;
                long ticks;
                if (!TryReadClockOrDisabled(out ticks, out clockFailure))
                    return clockFailure;

                if (!pipeline.HasStartedSession)
                    return pipeline.TryRecord(draft, ticks);

                string attemptKey;
                if (!AnalyticsAttemptKey.TryCreate(
                    levelId,
                    attemptNumber,
                    out attemptKey
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                return funnel.TryAcceptTerminal(
                    attemptKey,
                    complete
                        ? AnalyticsLevelTerminalOutcome.Complete
                        : AnalyticsLevelTerminalOutcome.Fail,
                    () => pipeline.TryRecord(draft, ticks)
                );
            }
        }

        AnalyticsRecordResult TryRecord(AnalyticsEventDraft draft)
        {
            lock (syncRoot)
            {
                AnalyticsRecordResult clockFailure;
                long ticks;
                if (!TryReadClockOrDisabled(out ticks, out clockFailure))
                    return clockFailure;
                return pipeline.TryRecord(draft, ticks);
            }
        }

        AnalyticsRecordResult TryPurchaseEvent(
            string eventId,
            string productId,
            string placementId,
            string outcomeId
        )
        {
            if (string.Equals(
                eventId,
                AnalyticsEventIds.PurchaseResult,
                StringComparison.Ordinal
            ))
            {
                return TryRecord(
                    new AnalyticsEventDraft(
                        eventId,
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.ProductId,
                            productId
                        ),
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.PlacementId,
                            placementId
                        ),
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.OutcomeId,
                            outcomeId
                        )
                    )
                );
            }

            return TryRecord(
                new AnalyticsEventDraft(
                    eventId,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.ProductId,
                        productId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.PlacementId,
                        placementId
                    )
                )
            );
        }

        AnalyticsRecordResult TryRewardedAdEvent(
            string eventId,
            string placementId,
            string rewardId,
            string outcomeId,
            bool rewardGranted
        )
        {
            if (string.Equals(
                eventId,
                AnalyticsEventIds.RewardedAdResult,
                StringComparison.Ordinal
            ))
            {
                return TryRecord(
                    new AnalyticsEventDraft(
                        eventId,
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.PlacementId,
                            placementId
                        ),
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.RewardId,
                            rewardId
                        ),
                        AnalyticsAttribute.String(
                            AnalyticsAttributeKeys.OutcomeId,
                            outcomeId
                        ),
                        AnalyticsAttribute.Boolean(
                            AnalyticsAttributeKeys.RewardGranted,
                            rewardGranted
                        )
                    )
                );
            }

            return TryRecord(
                new AnalyticsEventDraft(
                    eventId,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.PlacementId,
                        placementId
                    ),
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.RewardId,
                        rewardId
                    )
                )
            );
        }

        bool TryReadClockOrDisabled(
            out long ticks,
            out AnalyticsRecordResult failure
        )
        {
            if (!captureAllowed)
            {
                ticks = 1L;
                failure = null;
                return true;
            }

            return TryReadClock(out ticks, out failure);
        }

        bool TryReadClock(
            out long ticks,
            out AnalyticsRecordResult failure
        )
        {
            ticks = 0L;
            failure = null;
            if (utcNowTicks == null)
            {
                failure = AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.TimestampInvalid
                );
                return false;
            }

            try
            {
                ticks = utcNowTicks();
            }
            catch (Exception)
            {
                failure = AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterFailure
                );
                return false;
            }

            if (ticks < 1L)
            {
                failure = AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.TimestampInvalid
                );
                return false;
            }

            return true;
        }

        static AnalyticsEventDraft SingleStringDraft(
            string eventId,
            string key,
            string value
        )
        {
            return new AnalyticsEventDraft(
                eventId,
                AnalyticsAttribute.String(key, value)
            );
        }

        static AnalyticsEventDraft SingleIntegerDraft(
            string eventId,
            string key,
            long value
        )
        {
            return new AnalyticsEventDraft(
                eventId,
                AnalyticsAttribute.Integer(key, value)
            );
        }
    }
}
