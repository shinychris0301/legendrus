using System;
using System.Collections.Generic;
using LegendsOfRus.CommercialCore.Analytics;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Commercial
{
    public sealed class CommercialAnalyticsEditModeTests
    {
        const long StartTicks = 1000L;

        [Test]
        public void Registry_IsValidAndContainsExactSchemaOneEventSet()
        {
            AnalyticsRecordResult validation =
                AnalyticsEventValidator.ValidateRegistry();

            Assert.That(validation.Accepted, Is.True, validation.ReasonCode);
            Assert.That(AnalyticsSchemaRegistry.CurrentSchemaVersion, Is.EqualTo(1));
            Assert.That(AnalyticsSchemaRegistry.All.Count, Is.EqualTo(19));

            string[] expected =
            {
                AnalyticsEventIds.SessionStart,
                AnalyticsEventIds.ScreenView,
                AnalyticsEventIds.LevelStart,
                AnalyticsEventIds.LevelComplete,
                AnalyticsEventIds.LevelFail,
                AnalyticsEventIds.EnergyGateBlocked,
                AnalyticsEventIds.TutorialStepView,
                AnalyticsEventIds.TutorialStepComplete,
                AnalyticsEventIds.StorySceneStart,
                AnalyticsEventIds.StorySceneComplete,
                AnalyticsEventIds.ChapterComplete,
                AnalyticsEventIds.ChapterCompleteAcknowledged,
                AnalyticsEventIds.SandboxEntitlementGrant,
                AnalyticsEventIds.PurchaseOfferView,
                AnalyticsEventIds.PurchaseStart,
                AnalyticsEventIds.PurchaseResult,
                AnalyticsEventIds.RewardedAdOfferView,
                AnalyticsEventIds.RewardedAdStart,
                AnalyticsEventIds.RewardedAdResult,
            };

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(
                    AnalyticsSchemaRegistry.TryGet(expected[i], out _),
                    Is.True,
                    expected[i]
                );
            }
        }

        [Test]
        public void Draft_DeepCopiesCallerArrayAndValidatesTypedAttributes()
        {
            var attributes = new[]
            {
                AnalyticsAttribute.String(AnalyticsAttributeKeys.LevelId, "chapter01.level01"),
                AnalyticsAttribute.Integer(AnalyticsAttributeKeys.AttemptNumber, 1L),
                AnalyticsAttribute.Boolean(AnalyticsAttributeKeys.IsRetry, false),
            };
            var draft = new AnalyticsEventDraft(
                AnalyticsEventIds.LevelStart,
                attributes
            );

            attributes[0] = AnalyticsAttribute.String(
                AnalyticsAttributeKeys.LevelId,
                "mutated"
            );

            AnalyticsRecordResult result = AnalyticsEventValidator.ValidateDraft(draft);

            Assert.That(result.Accepted, Is.True, result.ReasonCode);
            Assert.That(draft.Attributes[0].StringValue, Is.EqualTo("chapter01.level01"));
        }

        [Test]
        public void NullDraft_IsRejectedWithoutThrowing()
        {
            AnalyticsRecordResult result = AnalyticsEventValidator.ValidateDraft(null);

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.DraftNull));
        }

        [TestCase("Unknown_Event", AnalyticsReasonCodes.EventIdInvalid)]
        [TestCase("unknown_event", AnalyticsReasonCodes.EventUnknown)]
        [TestCase(" screen_view", AnalyticsReasonCodes.EventIdInvalid)]
        public void InvalidEventId_IsRejectedWithStableReason(
            string eventId,
            string expectedCode
        )
        {
            AnalyticsRecordResult result = AnalyticsEventValidator.ValidateDraft(
                new AnalyticsEventDraft(eventId)
            );

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(expectedCode));
        }

        [Test]
        public void MissingUnexpectedDuplicateAndWrongTypeAttributes_FailClosed()
        {
            AssertRejected(
                new AnalyticsEventDraft(AnalyticsEventIds.ScreenView),
                AnalyticsReasonCodes.AttributeMissing
            );
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.ScreenId, "home"),
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.SourceId, "menu")
                ),
                AnalyticsReasonCodes.AttributeUnexpected
            );
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.ScreenId, "home"),
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.ScreenId, "shop")
                ),
                AnalyticsReasonCodes.AttributeDuplicate
            );
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.Integer(AnalyticsAttributeKeys.ScreenId, 1L)
                ),
                AnalyticsReasonCodes.AttributeTypeMismatch
            );
        }

        [Test]
        public void FreeFormDisallowedAndOutOfRangeValues_FailClosed()
        {
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.ScreenId,
                        "player email@example.com"
                    )
                ),
                AnalyticsReasonCodes.StringValueInvalid
            );
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.ScreenId, "home_screen")
                ),
                AnalyticsReasonCodes.StringValueNotAllowed
            );
            AssertRejected(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.LevelComplete,
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.LevelId, "chapter01.level01"),
                    AnalyticsAttribute.Integer(AnalyticsAttributeKeys.AttemptNumber, 1L),
                    AnalyticsAttribute.Integer(AnalyticsAttributeKeys.Score, 100L),
                    AnalyticsAttribute.Integer(AnalyticsAttributeKeys.Stars, 4L),
                    AnalyticsAttribute.Boolean(AnalyticsAttributeKeys.FirstClear, true)
                ),
                AnalyticsReasonCodes.IntegerValueOutOfRange
            );
        }

        [Test]
        public void CaptureDeniedAndDisabledDefaults_NeverStartASession()
        {
            var denied = new AnalyticsPipeline(
                false,
                new DeterministicOfflineAnalyticsAdapter()
            );
            var disabled = new AnalyticsPipeline(true);

            AnalyticsRecordResult deniedResult = denied.TryBeginSession(
                "session_1",
                StartTicks
            );
            AnalyticsRecordResult disabledResult = disabled.TryBeginSession(
                "session_1",
                StartTicks
            );

            Assert.That(deniedResult.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.CaptureNotAllowed));
            Assert.That(disabledResult.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.Disabled));
            Assert.That(denied.HasStartedSession, Is.False);
            Assert.That(disabled.HasStartedSession, Is.False);
            Assert.That(denied.AcceptedCount, Is.Zero);
            Assert.That(disabled.AcceptedCount, Is.Zero);
        }

        [Test]
        public void Pipeline_RequiresSessionAndRejectsDuplicateSession()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var pipeline = new AnalyticsPipeline(true, adapter);

            AnalyticsRecordResult beforeSession = pipeline.TryRecord(
                ScreenView("home"),
                StartTicks
            );
            AnalyticsRecordResult started = pipeline.TryBeginSession(
                "session_1",
                StartTicks
            );
            AnalyticsRecordResult duplicate = pipeline.TryBeginSession(
                "session_2",
                StartTicks + 1L
            );

            Assert.That(beforeSession.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.SessionNotStarted));
            Assert.That(started.Accepted, Is.True, started.ReasonCode);
            Assert.That(duplicate.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.SessionAlreadyStarted));
            Assert.That(adapter.Count, Is.EqualTo(1));
            Assert.That(pipeline.SessionId, Is.EqualTo("session_1"));
        }

        [TestCase("")]
        [TestCase(" session")]
        [TestCase("session with spaces")]
        public void InvalidSessionIdentity_IsRejectedWithoutAdapterMutation(string sessionId)
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var pipeline = new AnalyticsPipeline(true, adapter);

            AnalyticsRecordResult result = pipeline.TryBeginSession(
                sessionId,
                StartTicks
            );

            Assert.That(result.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.SessionIdInvalid));
            Assert.That(adapter.Count, Is.Zero);
            Assert.That(pipeline.HasStartedSession, Is.False);
        }

        [Test]
        public void AcceptedEnvelope_HasMonotonicSequenceAndSortedAttributes()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var pipeline = new AnalyticsPipeline(true, adapter);
            Assert.That(
                pipeline.TryBeginSession("session_1", StartTicks).Accepted,
                Is.True
            );

            AnalyticsRecordResult result = pipeline.TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.LevelStart,
                    AnalyticsAttribute.String(AnalyticsAttributeKeys.LevelId, "chapter01.level01"),
                    AnalyticsAttribute.Boolean(AnalyticsAttributeKeys.IsRetry, false),
                    AnalyticsAttribute.Integer(AnalyticsAttributeKeys.AttemptNumber, 1L)
                ),
                StartTicks + 1L
            );

            Assert.That(result.Accepted, Is.True, result.ReasonCode);
            IReadOnlyList<AnalyticsEventEnvelope> snapshot = adapter.Snapshot();
            Assert.That(snapshot.Count, Is.EqualTo(2));
            Assert.That(snapshot[0].Sequence, Is.EqualTo(1L));
            Assert.That(snapshot[1].Sequence, Is.EqualTo(2L));
            Assert.That(snapshot[1].Attributes[0].Key, Is.EqualTo(AnalyticsAttributeKeys.AttemptNumber));
            Assert.That(snapshot[1].Attributes[1].Key, Is.EqualTo(AnalyticsAttributeKeys.IsRetry));
            Assert.That(snapshot[1].Attributes[2].Key, Is.EqualTo(AnalyticsAttributeKeys.LevelId));
        }

        [Test]
        public void TimestampRollbackAndCapacityFailure_DoNotAdvancePipelineState()
        {
            var rollbackAdapter = new DeterministicOfflineAnalyticsAdapter();
            var rollbackPipeline = new AnalyticsPipeline(true, rollbackAdapter);
            rollbackPipeline.TryBeginSession("session_1", StartTicks);

            AnalyticsRecordResult rollback = rollbackPipeline.TryRecord(
                ScreenView("home"),
                StartTicks - 1L
            );

            var fullAdapter = new DeterministicOfflineAnalyticsAdapter(1);
            var fullPipeline = new AnalyticsPipeline(true, fullAdapter);
            fullPipeline.TryBeginSession("session_2", StartTicks);
            string before = fullAdapter.CanonicalSnapshot();
            AnalyticsRecordResult full = fullPipeline.TryRecord(
                ScreenView("home"),
                StartTicks + 1L
            );

            Assert.That(rollback.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.TimestampRollback));
            Assert.That(rollbackPipeline.AcceptedCount, Is.EqualTo(1L));
            Assert.That(rollbackAdapter.Count, Is.EqualTo(1));
            Assert.That(full.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.CapacityReached));
            Assert.That(fullPipeline.AcceptedCount, Is.EqualTo(1L));
            Assert.That(fullAdapter.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void UploadCapableAndThrowingAdapters_FailWithoutLeakingExceptions()
        {
            var uploading = new ProbeAdapter { Upload = true };
            var throwing = new ProbeAdapter { ThrowOnCapabilityRead = true };
            var throwingRecord = new ProbeAdapter { ThrowOnRecord = true };

            AnalyticsRecordResult uploadResult = new AnalyticsPipeline(
                true,
                uploading
            ).TryBeginSession("session_1", StartTicks);
            AnalyticsRecordResult capabilityResult = new AnalyticsPipeline(
                true,
                throwing
            ).TryBeginSession("session_1", StartTicks);
            AnalyticsRecordResult recordResult = new AnalyticsPipeline(
                true,
                throwingRecord
            ).TryBeginSession("session_1", StartTicks);

            Assert.That(uploadResult.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.UploadUnavailable));
            Assert.That(uploading.RecordCalls, Is.Zero);
            Assert.That(capabilityResult.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.AdapterFailure));
            Assert.That(recordResult.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.AdapterFailure));
            Assert.That(throwingRecord.RecordCalls, Is.EqualTo(1));
        }

        [Test]
        public void CanonicalSnapshot_IsByteStableAndFlushAlwaysUnavailable()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var pipeline = new AnalyticsPipeline(true, adapter);
            pipeline.TryBeginSession("session_1", StartTicks);
            pipeline.TryRecord(ScreenView("world_map"), StartTicks + 1L);

            string first = adapter.CanonicalSnapshot();
            string second = adapter.CanonicalSnapshot();

            Assert.That(second, Is.EqualTo(first));
            Assert.That(first, Does.Not.Contain("\n"));
            Assert.That(first, Does.Contain("\"sequence\":2"));
            Assert.That(pipeline.TryFlush().ReasonCode, Is.EqualTo(AnalyticsReasonCodes.UploadUnavailable));
            Assert.That(adapter.TryFlush().ReasonCode, Is.EqualTo(AnalyticsReasonCodes.UploadUnavailable));
        }

        [Test]
        public void Funnel_LegalTerminalAllowsNextUniqueAttemptAndResetClearsState()
        {
            Assert.That(
                AnalyticsAttemptKey.TryCreate("chapter01.level01", 1L, out string firstKey),
                Is.True
            );
            Assert.That(
                AnalyticsAttemptKey.TryCreate("chapter01.level01", 2L, out string secondKey),
                Is.True
            );
            var tracker = new AnalyticsFunnelTracker(2);

            Assert.That(tracker.TryBeginLevelAttempt(firstKey).Accepted, Is.True);
            Assert.That(tracker.TryCompleteLevelAttempt(firstKey).Accepted, Is.True);
            Assert.That(tracker.TryBeginLevelAttempt(secondKey).Accepted, Is.True);
            Assert.That(tracker.TryFailLevelAttempt(secondKey).Accepted, Is.True);
            Assert.That(tracker.AcceptedTerminalCount, Is.EqualTo(2));
            Assert.That(tracker.TrackedAttemptCount, Is.EqualTo(2));

            tracker.Reset();

            Assert.That(tracker.TrackedAttemptCount, Is.Zero);
            Assert.That(tracker.AcceptedTerminalCount, Is.Zero);
            Assert.That(tracker.HasActiveAttempt, Is.False);
            Assert.That(tracker.TerminalOutcome, Is.EqualTo(AnalyticsLevelTerminalOutcome.None));
        }

        [Test]
        public void Funnel_RejectionsLeaveActiveAttemptSnapshotUnchanged()
        {
            var tracker = new AnalyticsFunnelTracker();
            AnalyticsAttemptKey.TryCreate("chapter01.level01", 1L, out string firstKey);
            AnalyticsAttemptKey.TryCreate("chapter01.level02", 1L, out string otherKey);
            tracker.TryBeginLevelAttempt(firstKey);

            AnalyticsRecordResult activeConflict = tracker.TryBeginLevelAttempt(otherKey);
            AnalyticsRecordResult mismatch = tracker.TryCompleteLevelAttempt(otherKey);
            AnalyticsRecordResult invalidOutcome = tracker.TryAcceptTerminal(
                firstKey,
                AnalyticsLevelTerminalOutcome.None
            );

            Assert.That(activeConflict.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.FunnelAttemptAlreadyActive));
            Assert.That(mismatch.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.FunnelAttemptMismatch));
            Assert.That(invalidOutcome.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.FunnelOutcomeInvalid));
            Assert.That(tracker.ActiveAttemptKey, Is.EqualTo(firstKey));
            Assert.That(tracker.HasTerminalOutcome, Is.False);
            Assert.That(tracker.AcceptedTerminalCount, Is.Zero);
        }

        [Test]
        public void Funnel_DuplicateBeginAndTerminalAreRejectedExactlyOnce()
        {
            var tracker = new AnalyticsFunnelTracker();
            AnalyticsAttemptKey.TryCreate("chapter01.level01", 1L, out string key);

            Assert.That(tracker.TryBeginLevelAttempt(key).Accepted, Is.True);
            Assert.That(tracker.TryFailLevelAttempt(key).Accepted, Is.True);
            AnalyticsRecordResult duplicateTerminal = tracker.TryCompleteLevelAttempt(key);
            AnalyticsRecordResult duplicateBegin = tracker.TryBeginLevelAttempt(key);

            Assert.That(duplicateTerminal.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.FunnelTerminalDuplicate));
            Assert.That(duplicateBegin.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.FunnelAttemptDuplicate));
            Assert.That(tracker.AcceptedTerminalCount, Is.EqualTo(1));
        }

        [TestCase("chapter01.level01", 0L)]
        [TestCase("chapter01.level01", -1L)]
        [TestCase("chapter 01", 1L)]
        public void AttemptKey_InvalidInputsFailWithoutReturningAPartialKey(
            string levelId,
            long attempt
        )
        {
            bool created = AnalyticsAttemptKey.TryCreate(levelId, attempt, out string key);

            Assert.That(created, Is.False);
            Assert.That(key, Is.Empty);
        }

        static AnalyticsEventDraft ScreenView(string screenId)
        {
            return new AnalyticsEventDraft(
                AnalyticsEventIds.ScreenView,
                AnalyticsAttribute.String(AnalyticsAttributeKeys.ScreenId, screenId)
            );
        }

        static void AssertRejected(
            AnalyticsEventDraft draft,
            string expectedReason
        )
        {
            AnalyticsRecordResult result = AnalyticsEventValidator.ValidateDraft(draft);
            Assert.That(result.Accepted, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(expectedReason));
        }

        sealed class ProbeAdapter : IAnalyticsAdapter
        {
            public bool Upload;
            public bool ThrowOnCapabilityRead;
            public bool ThrowOnRecord;
            public int RecordCalls;

            public string AdapterId => "probe";
            public bool CaptureEnabled
            {
                get
                {
                    if (ThrowOnCapabilityRead)
                        throw new InvalidOperationException("capability");
                    return true;
                }
            }
            public bool SupportsUpload => Upload;

            public AnalyticsRecordResult TryRecord(AnalyticsEventEnvelope envelope)
            {
                RecordCalls++;
                if (ThrowOnRecord)
                    throw new InvalidOperationException("record");
                return AnalyticsRecordResult.Success();
            }

            public AnalyticsRecordResult TryFlush()
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.UploadUnavailable
                );
            }
        }
    }
}
