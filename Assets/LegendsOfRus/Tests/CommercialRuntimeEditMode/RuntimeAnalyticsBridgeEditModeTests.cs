using System;
using System.Collections.Generic;
using System.Linq;
using LegendsOfRus.CommercialCore.Analytics;
using NUnit.Framework;

namespace LegendsOfRus.Tests.CommercialRuntime
{
    public sealed class RuntimeAnalyticsBridgeEditModeTests
    {
        [Test]
        public void DisabledBridge_DoesNotReadClockOrCapture()
        {
            int clockReads = 0;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                false,
                adapter,
                "session_disabled",
                () =>
                {
                    clockReads++;
                    throw new InvalidOperationException("must not be read");
                }
            );

            AnalyticsRecordResult begin = bridge.TryBeginSession();
            AnalyticsRecordResult screen = bridge.TryScreenView("home");

            Assert.That(begin.Accepted, Is.False);
            Assert.That(begin.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.CaptureNotAllowed));
            Assert.That(screen.Accepted, Is.False);
            Assert.That(clockReads, Is.Zero);
            Assert.That(adapter.Count, Is.Zero);
            Assert.That(bridge.AcceptedCount, Is.Zero);
        }

        [Test]
        public void Session_StartsExactlyOnceWithMonotonicEventOrder()
        {
            long ticks = 100L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "session_one",
                () => ++ticks
            );

            Assert.That(bridge.TryBeginSession().Accepted, Is.True);
            Assert.That(bridge.TryBeginSession().ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.SessionAlreadyStarted));
            Assert.That(bridge.TryScreenView("gameplay").Accepted, Is.True);
            Assert.That(bridge.TryStorySceneStart("scene_intro").Accepted, Is.True);

            var records = adapter.Snapshot();
            Assert.That(records.Select(value => value.EventId), Is.EqualTo(new[]
            {
                AnalyticsEventIds.SessionStart,
                AnalyticsEventIds.ScreenView,
                AnalyticsEventIds.StorySceneStart
            }));
            Assert.That(records.Select(value => value.Sequence),
                Is.EqualTo(new long[] { 1L, 2L, 3L }));
            Assert.That(records[0].UtcTicks, Is.LessThan(records[1].UtcTicks));
            Assert.That(records[1].UtcTicks, Is.LessThan(records[2].UtcTicks));
        }

        [Test]
        public void LevelFunnel_AllowsOneStartAndOneComplete()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);

            Assert.That(bridge.TryLevelStart("level_1", 1, false).Accepted, Is.True);
            Assert.That(bridge.TryLevelComplete("level_1", 1, 500, 3, true).Accepted,
                Is.True);
            AnalyticsRecordResult duplicate = bridge.TryLevelFail("level_1", 1, 0);

            Assert.That(duplicate.Accepted, Is.False);
            Assert.That(duplicate.ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.FunnelTerminalDuplicate));
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.EqualTo(1));
            Assert.That(adapter.Snapshot().Select(value => value.EventId),
                Is.EqualTo(new[]
                {
                    AnalyticsEventIds.SessionStart,
                    AnalyticsEventIds.LevelStart,
                    AnalyticsEventIds.LevelComplete
                }));
        }

        [Test]
        public void LevelTerminalBeforeStart_IsRejectedWithoutPipelineMutation()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);

            AnalyticsRecordResult result = bridge.TryLevelFail("level_1", 1, 20);

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.FunnelTerminalOutOfOrder));
            Assert.That(adapter.Count, Is.EqualTo(1));
            Assert.That(bridge.TrackedLevelAttemptCount, Is.Zero);
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.Zero);
        }

        [Test]
        public void RejectedBeginRecord_LeavesFunnelReplayStateUntouched()
        {
            var adapter = new ControllableAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);
            adapter.RejectNext = true;

            AnalyticsRecordResult rejected = bridge.TryLevelStart("level_2", 1, false);

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(bridge.HasActiveLevelAttempt, Is.False);
            Assert.That(bridge.TrackedLevelAttemptCount, Is.Zero);
            Assert.That(bridge.TryLevelStart("level_2", 1, false).Accepted, Is.True);
            Assert.That(bridge.HasActiveLevelAttempt, Is.True);
            Assert.That(bridge.TrackedLevelAttemptCount, Is.EqualTo(1));
        }

        [Test]
        public void ThrowingBeginRecord_LeavesFunnelReplayStateUntouched()
        {
            var adapter = new ControllableAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);
            adapter.ThrowNext = true;

            AnalyticsRecordResult rejected = bridge.TryLevelStart("level_3", 1, false);

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(rejected.ReasonCode, Is.EqualTo(AnalyticsReasonCodes.AdapterFailure));
            Assert.That(bridge.HasActiveLevelAttempt, Is.False);
            Assert.That(bridge.TrackedLevelAttemptCount, Is.Zero);
            Assert.That(bridge.TryLevelStart("level_3", 1, false).Accepted, Is.True);
        }

        [Test]
        public void RejectedTerminalRecord_LeavesActiveAttemptRetryable()
        {
            var adapter = new ControllableAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);
            Assert.That(bridge.TryLevelStart("level_4", 1, false).Accepted, Is.True);
            adapter.RejectNext = true;

            AnalyticsRecordResult rejected =
                bridge.TryLevelComplete("level_4", 1, 100, 2, false);

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(bridge.HasActiveLevelAttempt, Is.True);
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.Zero);
            Assert.That(bridge.TryLevelComplete("level_4", 1, 100, 2, false).Accepted,
                Is.True);
            Assert.That(bridge.HasActiveLevelAttempt, Is.False);
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.EqualTo(1));
        }

        [Test]
        public void Cancel_EmitsNothingRetainsReplayKeyAndAllowsNewAttempt()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);
            Assert.That(bridge.TryLevelStart("level_5", 1, false).Accepted, Is.True);
            int beforeCancel = adapter.Count;

            Assert.That(bridge.TryCancelLevelAttempt("level_5", 1).Accepted, Is.True);
            Assert.That(adapter.Count, Is.EqualTo(beforeCancel));
            Assert.That(bridge.HasActiveLevelAttempt, Is.False);
            Assert.That(bridge.AcceptedLevelTerminalCount, Is.Zero);
            Assert.That(bridge.TryLevelStart("level_5", 1, true).ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.FunnelAttemptDuplicate));
            Assert.That(bridge.TryLevelStart("level_5", 2, true).Accepted, Is.True);
        }

        [Test]
        public void InvalidIdentifiersAndRangesRejectWithoutThrowing()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);

            Assert.DoesNotThrow(() => bridge.TryScreenView("not_a_screen"));
            Assert.That(bridge.TryScreenView("not_a_screen").ReasonCode,
                Is.EqualTo(AnalyticsReasonCodes.StringValueNotAllowed));
            Assert.That(bridge.TryLevelStart("bad level", 0, false).Accepted, Is.False);
            Assert.That(bridge.TryLevelComplete("level_6", 1, -1, 9, false).Accepted,
                Is.False);
            Assert.That(bridge.TryChapterComplete(0).Accepted, Is.False);
        }

        [Test]
        public void TypedCommercialEvents_AcceptOnlySchemaAllowlistedValues()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = ActiveBridge(adapter);
            Assert.That(bridge.TryBeginSession().Accepted, Is.True);

            Assert.That(bridge.TryPurchaseOfferView(
                "legends_of_rus.starter_pack",
                "shop_starter"
            ).Accepted, Is.True);
            Assert.That(bridge.TryPurchaseResult(
                "legends_of_rus.starter_pack",
                "shop_starter",
                "verified"
            ).Accepted, Is.True);
            Assert.That(bridge.TryRewardedAdResult(
                "victory_double_reward",
                "reward_double",
                "completed",
                false
            ).Accepted, Is.True);
            Assert.That(bridge.TryPurchaseResult(
                "legends_of_rus.starter_pack",
                "shop_starter",
                "receipt_text"
            ).ReasonCode, Is.EqualTo(AnalyticsReasonCodes.StringValueNotAllowed));
        }

        [Test]
        public void RuntimeBridge_ExposesNoFlushOrUploadMethod()
        {
            string[] publicMethods = typeof(RuntimeAnalyticsBridge)
                .GetMethods(System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public)
                .Select(method => method.Name)
                .ToArray();

            Assert.That(publicMethods.Any(name =>
                name.IndexOf("Flush", StringComparison.OrdinalIgnoreCase) >= 0),
                Is.False);
            Assert.That(publicMethods.Any(name =>
                name.IndexOf("Upload", StringComparison.OrdinalIgnoreCase) >= 0),
                Is.False);
        }

        static RuntimeAnalyticsBridge ActiveBridge(IAnalyticsAdapter adapter)
        {
            long ticks = 1000L;
            return new RuntimeAnalyticsBridge(
                true,
                adapter,
                "test_session",
                () => ++ticks
            );
        }

        sealed class ControllableAdapter : IAnalyticsAdapter
        {
            readonly List<AnalyticsEventEnvelope> records =
                new List<AnalyticsEventEnvelope>();

            public string AdapterId => "test_offline";
            public bool CaptureEnabled => true;
            public bool SupportsUpload => false;
            public bool RejectNext { get; set; }
            public bool ThrowNext { get; set; }

            public AnalyticsRecordResult TryRecord(AnalyticsEventEnvelope envelope)
            {
                if (ThrowNext)
                {
                    ThrowNext = false;
                    throw new InvalidOperationException("test-only adapter failure");
                }
                if (RejectNext)
                {
                    RejectNext = false;
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.CapacityReached
                    );
                }
                records.Add(envelope);
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
