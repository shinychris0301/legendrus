using System;
using System.Collections.Generic;
using LegendsOfRus.DiagnosticsCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.Diagnostics.PlayMode
{
    [TestFixture]
    public sealed class ErrorReportFoundationPlayModeTests
    {
        const long StartTicks = 638923680000000000L;
        static readonly string[] SaveKeys =
        {
            "LegendsOfRus_Save_v1",
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        bool[] previousPresence;
        string[] previousValues;
        string[] sentinels;

        [SetUp]
        public void SetUp()
        {
            previousPresence = new bool[SaveKeys.Length];
            previousValues = new string[SaveKeys.Length];
            sentinels = new string[SaveKeys.Length];
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                previousPresence[i] = PlayerPrefs.HasKey(SaveKeys[i]);
                previousValues[i] = previousPresence[i]
                    ? PlayerPrefs.GetString(SaveKeys[i])
                    : string.Empty;
                sentinels[i] = "diagnostics-playmode-sentinel-" + i;
                PlayerPrefs.SetString(SaveKeys[i], sentinels[i]);
            }
            PlayerPrefs.Save();
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                PlayerPrefs.DeleteKey(SaveKeys[i]);
                if (previousPresence[i])
                    PlayerPrefs.SetString(SaveKeys[i], previousValues[i]);
            }
            PlayerPrefs.Save();
        }

        [Test]
        public void DefaultDisabledTransport_IsNonThrowingAndLocalOnly()
        {
            var pipeline = new ErrorReportingPipeline(true);

            ErrorReportOperationResult capture = pipeline.TryCapture(
                DraftA(),
                StartTicks
            );
            ErrorReportOperationResult flush = pipeline.TryFlushOne(
                StartTicks,
                true
            );

            Assert.That(capture.Accepted, Is.True, capture.ReasonCode);
            Assert.That(flush.Accepted, Is.False);
            Assert.That(flush.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportDisabled));
            Assert.That(pipeline.NetworkTransportAllowed, Is.False);
            Assert.That(pipeline.BufferedCount, Is.EqualTo(1));
        }

        [Test]
        public void OfflineCapture_PreservesSnapshotAndMakesNoTransportCall()
        {
            var transport = new ProbeTransport();
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);
            string before = pipeline.CanonicalSnapshot();

            ErrorReportOperationResult flush = pipeline.TryFlushOne(
                StartTicks,
                false
            );

            Assert.That(flush.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Offline));
            Assert.That(transport.SendCalls, Is.Zero);
            Assert.That(transport.CapabilityReads, Is.Zero);
            Assert.That(pipeline.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void CapacityAndDeduplication_MatchEngineFreeContract()
        {
            var policy = new ErrorReportPolicy(
                1,
                10L,
                100L,
                3,
                10L,
                40L
            );
            var pipeline = new ErrorReportingPipeline(true, policy);

            ErrorReportOperationResult first = pipeline.TryCapture(
                DraftA(),
                StartTicks
            );
            ErrorReportOperationResult duplicate = pipeline.TryCapture(
                DraftA(),
                StartTicks + 10L
            );
            string beforeRejected = pipeline.CanonicalSnapshot();
            ErrorReportOperationResult full = pipeline.TryCapture(
                DraftB(),
                StartTicks + 11L
            );

            Assert.That(first.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Captured));
            Assert.That(duplicate.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Deduplicated));
            Assert.That(full.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.CapacityReached));
            Assert.That(pipeline.BufferedCount, Is.EqualTo(1));
            Assert.That(pipeline.Snapshot()[0].OccurrenceCount, Is.EqualTo(2L));
            Assert.That(pipeline.CanonicalSnapshot(), Is.EqualTo(beforeRejected));
        }

        [Test]
        public void RetryDueThenSuccess_UsesInjectedTicksAndRemovesRecord()
        {
            var policy = new ErrorReportPolicy(
                2,
                10L,
                1000L,
                3,
                10L,
                40L
            );
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            var pipeline = new ErrorReportingPipeline(true, policy, transport);
            pipeline.TryCapture(DraftA(), 100L);

            Assert.That(pipeline.TryFlushOne(100L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RetryScheduled));
            Assert.That(pipeline.Snapshot()[0].NextAttemptUtcTicks,
                Is.EqualTo(110L));
            Assert.That(pipeline.TryFlushOne(109L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.NothingDue));
            Assert.That(pipeline.TryFlushOne(110L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Delivered));
            Assert.That(transport.SendCalls, Is.EqualTo(2));
            Assert.That(pipeline.BufferedCount, Is.Zero);
        }

        [Test]
        public void ThrowingAndNullTransportFailures_AreContained()
        {
            var throwing = new ProbeTransport { ThrowOnSend = true };
            var returningNull = new ProbeTransport { ReturnNull = true };
            var first = new ErrorReportingPipeline(true, null, throwing);
            var second = new ErrorReportingPipeline(true, null, returningNull);
            first.TryCapture(DraftA(), StartTicks);
            second.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult thrown = null;
            ErrorReportOperationResult nullResult = null;
            Assert.DoesNotThrow(() =>
                thrown = first.TryFlushOne(StartTicks, true));
            Assert.DoesNotThrow(() =>
                nullResult = second.TryFlushOne(StartTicks, true));

            Assert.That(thrown.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(nullResult.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(first.Snapshot()[0].DeliveryAttempts, Is.EqualTo(1));
            Assert.That(second.Snapshot()[0].DeliveryAttempts, Is.EqualTo(1));
        }

        [Test]
        public void CanonicalSnapshot_ContainsOnlyMinimizedReviewedFields()
        {
            var pipeline = new ErrorReportingPipeline(true);
            pipeline.TryCapture(DraftA(), StartTicks);

            string json = pipeline.CanonicalSnapshot();

            Assert.That(json, Does.StartWith("[{\"schemaVersion\":1,"));
            Assert.That(json, Does.Contain("\"fingerprint\":"));
            Assert.That(json, Does.Contain("\"errorCode\":"));
            Assert.That(json, Does.Contain("\"siteCode\":"));
            Assert.That(json, Does.Not.Contain("message"));
            Assert.That(json, Does.Not.Contain("stack"));
            Assert.That(json, Does.Not.Contain("payload"));
            Assert.That(json, Does.Not.Contain("player"));
            Assert.That(json, Does.Not.Contain("device"));
            Assert.That(json, Does.Not.Contain("receipt"));
            Assert.That(json, Does.Not.Contain("token"));
        }

        [Test]
        public void Pipeline_CreatesNoGameObjectAndPreservesThreeSaveKeys()
        {
            int beforeObjects = UnityEngine.Object
                .FindObjectsByType<Transform>(FindObjectsInactive.Include)
                .Length;

            var pipeline = new ErrorReportingPipeline(true);
            pipeline.TryCapture(DraftA(), StartTicks);
            pipeline.TryFlushOne(StartTicks, false);
            pipeline.PruneExpired(StartTicks + 1L);

            int afterObjects = UnityEngine.Object
                .FindObjectsByType<Transform>(FindObjectsInactive.Include)
                .Length;
            Assert.That(afterObjects, Is.EqualTo(beforeObjects));
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                Assert.That(PlayerPrefs.HasKey(SaveKeys[i]), Is.True);
                Assert.That(
                    PlayerPrefs.GetString(SaveKeys[i]),
                    Is.EqualTo(sentinels[i]),
                    SaveKeys[i]
                );
            }
        }

        static ErrorReportDraft DraftA()
        {
            return new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Gameplay,
                ErrorReportCode.Match3PlanFailure,
                ErrorReportSiteCode.Match3ResolvePlan
            );
        }

        static ErrorReportDraft DraftB()
        {
            return new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Save,
                ErrorReportCode.SaveWriteFailure,
                ErrorReportSiteCode.SaveWriteCommit
            );
        }

        sealed class ProbeTransport : IErrorReportTransport
        {
            readonly Queue<ErrorReportTransportResult> results =
                new Queue<ErrorReportTransportResult>();

            public bool ThrowOnSend;
            public bool ReturnNull;
            public int CapabilityReads;
            public int SendCalls;

            public string TransportId { get { return "playmode_probe"; } }
            public bool DeliveryEnabled
            {
                get
                {
                    CapabilityReads++;
                    return true;
                }
            }
            public bool SupportsNetwork
            {
                get
                {
                    CapabilityReads++;
                    return false;
                }
            }

            public void Enqueue(ErrorReportTransportResult result)
            {
                results.Enqueue(result);
            }

            public ErrorReportTransportResult TrySend(
                ErrorReportRecord record
            )
            {
                SendCalls++;
                if (ThrowOnSend)
                    throw new InvalidOperationException("send");
                if (ReturnNull)
                    return null;
                return results.Count > 0
                    ? results.Dequeue()
                    : ErrorReportTransportResult.Delivered();
            }
        }
    }
}
