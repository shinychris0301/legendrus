using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LegendsOfRus.DiagnosticsCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Diagnostics
{
    [TestFixture]
    public sealed class ErrorReportFoundationEditModeTests
    {
        const long StartTicks = 638923680000000000L;
        const string ExpectedFingerprint =
            "40a6e5321af214d439966d57d8037278c2abe7921066e4b27e596b257f52afd7";

        [Test]
        public void Policy_DefaultsAndRetryBackoffArePinned()
        {
            ErrorReportPolicy policy = ErrorReportPolicy.Default;

            Assert.That(policy.IsValid, Is.True);
            Assert.That(policy.Capacity, Is.EqualTo(64));
            Assert.That(ErrorReportPolicy.MaximumCapacity, Is.EqualTo(256));
            Assert.That(
                policy.DeduplicationWindowTicks,
                Is.EqualTo(10L * TimeSpan.TicksPerMinute)
            );
            Assert.That(
                policy.RetentionTicks,
                Is.EqualTo(7L * TimeSpan.TicksPerDay)
            );
            Assert.That(
                ErrorReportPolicy.MaximumRetentionTicks,
                Is.EqualTo(30L * TimeSpan.TicksPerDay)
            );
            Assert.That(policy.MaximumDeliveryAttempts, Is.EqualTo(5));
            Assert.That(
                policy.RetryDelayTicks(1),
                Is.EqualTo(30L * TimeSpan.TicksPerSecond)
            );
            Assert.That(
                policy.RetryDelayTicks(2),
                Is.EqualTo(60L * TimeSpan.TicksPerSecond)
            );
            Assert.That(
                policy.RetryDelayTicks(100),
                Is.EqualTo(TimeSpan.TicksPerHour)
            );
        }

        [Test]
        public void Policy_InvalidBoundariesFailClosed()
        {
            var invalid = new[]
            {
                Policy(0, 10L, 100L, 3, 10L, 40L),
                Policy(257, 10L, 100L, 3, 10L, 40L),
                Policy(2, 0L, 100L, 3, 10L, 40L),
                Policy(2, 10L, 9L, 3, 10L, 40L),
                Policy(
                    2,
                    10L,
                    ErrorReportPolicy.MaximumRetentionTicks + 1L,
                    3,
                    10L,
                    40L
                ),
                Policy(2, 10L, 100L, 0, 10L, 40L),
                Policy(2, 10L, 100L, 11, 10L, 40L),
                Policy(2, 10L, 100L, 3, 0L, 40L),
                Policy(2, 10L, 100L, 3, 20L, 19L),
                Policy(
                    2,
                    10L,
                    ErrorReportPolicy.MaximumRetentionTicks,
                    3,
                    10L,
                    ErrorReportPolicy.MaximumRetryDelayTicksLimit + 1L
                )
            };

            Assert.That(invalid.All(value => !value.IsValid), Is.True);
            Assert.That(invalid.All(value => value.RetryDelayTicks(1) == 0L),
                Is.True);
            Assert.That(ErrorReportPolicy.Default.RetryDelayTicks(0), Is.Zero);
        }

        [Test]
        public void Validator_AcceptsCanonicalDraftAndRejectsUnknownEnums()
        {
            ErrorReportOperationResult valid =
                ErrorReportValidator.ValidateDraft(DraftA());
            ErrorReportOperationResult badKind =
                ErrorReportValidator.ValidateDraft(
                    new ErrorReportDraft(
                        (ErrorReportKind)int.MaxValue,
                        ErrorReportSubsystem.Gameplay,
                        ErrorReportCode.Match3PlanFailure,
                        ErrorReportSiteCode.Match3ResolvePlan
                    )
                );
            ErrorReportOperationResult badSubsystem =
                ErrorReportValidator.ValidateDraft(
                    new ErrorReportDraft(
                        ErrorReportKind.NonFatal,
                        (ErrorReportSubsystem)int.MaxValue,
                        ErrorReportCode.Match3PlanFailure,
                        ErrorReportSiteCode.Match3ResolvePlan
                    )
                );

            Assert.That(valid.Accepted, Is.True, valid.ReasonCode);
            Assert.That(badKind.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.KindInvalid));
            Assert.That(badSubsystem.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.SubsystemInvalid));
            Assert.That(
                ErrorReportValidator.ValidateDraft(null).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.DraftNull)
            );
        }

        [Test]
        public void Validator_RejectsUnknownCatalogValuesAndMismatchedTuple()
        {
            var unknownError = new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Gameplay,
                (ErrorReportCode)int.MaxValue,
                ErrorReportSiteCode.Match3ResolvePlan
            );
            var unknownSite = new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Gameplay,
                ErrorReportCode.Match3PlanFailure,
                (ErrorReportSiteCode)int.MaxValue
            );
            var mismatch = new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Save,
                ErrorReportCode.Match3PlanFailure,
                ErrorReportSiteCode.Match3ResolvePlan
            );

            Assert.That(
                ErrorReportValidator.ValidateDraft(unknownError).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.ErrorCodeInvalid));
            Assert.That(
                ErrorReportValidator.ValidateDraft(unknownSite).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.SiteCodeInvalid));
            Assert.That(
                ErrorReportValidator.ValidateDraft(mismatch).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.CodeTupleInvalid));
        }

        [Test]
        public void DraftConstructor_HasNoStringOrArbitraryMetadataParameter()
        {
            ConstructorInfo constructor = typeof(ErrorReportDraft)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Single();
            Type[] parameterTypes = constructor.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    typeof(ErrorReportKind),
                    typeof(ErrorReportSubsystem),
                    typeof(ErrorReportCode),
                    typeof(ErrorReportSiteCode)
                },
                parameterTypes
            );
            Assert.That(Array.IndexOf(parameterTypes, typeof(string)), Is.EqualTo(-1));
        }

        [Test]
        public void ReviewedCatalog_IsExactAndEveryTupleHasPinnedWireCodes()
        {
            var codes = new[]
            {
                ErrorReportCode.BootstrapFailure,
                ErrorReportCode.RuntimeInvariantFailure,
                ErrorReportCode.ManagedCrashUnhandled,
                ErrorReportCode.Match3PlanFailure,
                ErrorReportCode.SaveReadFailure,
                ErrorReportCode.SaveWriteFailure,
                ErrorReportCode.SaveMigrationFailure,
                ErrorReportCode.EconomyTransactionFailure,
                ErrorReportCode.CommercialAdapterFailure,
                ErrorReportCode.UiPresentationFailure,
                ErrorReportCode.AudioPlaybackFailure,
                ErrorReportCode.ContentLoadFailure
            };
            var sites = new[]
            {
                ErrorReportSiteCode.BootstrapInitialize,
                ErrorReportSiteCode.RuntimeInvariantCheck,
                ErrorReportSiteCode.RuntimeUnhandled,
                ErrorReportSiteCode.Match3ResolvePlan,
                ErrorReportSiteCode.SaveReadDecode,
                ErrorReportSiteCode.SaveWriteCommit,
                ErrorReportSiteCode.SaveMigrationApply,
                ErrorReportSiteCode.EconomyTransactionCommit,
                ErrorReportSiteCode.CommercialAdapterInvoke,
                ErrorReportSiteCode.UiPresentationUpdate,
                ErrorReportSiteCode.AudioPlaybackInvoke,
                ErrorReportSiteCode.ContentResourceLoad
            };
            var subsystems = new[]
            {
                ErrorReportSubsystem.Bootstrap,
                ErrorReportSubsystem.Runtime,
                ErrorReportSubsystem.Runtime,
                ErrorReportSubsystem.Gameplay,
                ErrorReportSubsystem.Save,
                ErrorReportSubsystem.Save,
                ErrorReportSubsystem.Save,
                ErrorReportSubsystem.Economy,
                ErrorReportSubsystem.Commercial,
                ErrorReportSubsystem.Ui,
                ErrorReportSubsystem.Audio,
                ErrorReportSubsystem.Content
            };
            var errorWireCodes = new[]
            {
                "BOOTSTRAP_FAILURE", "RUNTIME_INVARIANT_FAILURE",
                "MANAGED_CRASH_UNHANDLED", "MATCH3_PLAN_FAILURE",
                "SAVE_READ_FAILURE", "SAVE_WRITE_FAILURE",
                "SAVE_MIGRATION_FAILURE", "ECONOMY_TRANSACTION_FAILURE",
                "COMMERCIAL_ADAPTER_FAILURE", "UI_PRESENTATION_FAILURE",
                "AUDIO_PLAYBACK_FAILURE", "CONTENT_LOAD_FAILURE"
            };
            var siteWireCodes = new[]
            {
                "bootstrap.initialize", "runtime.invariant.check",
                "runtime.unhandled", "match3.resolve.plan",
                "save.read.decode", "save.write.commit",
                "save.migration.apply", "economy.transaction.commit",
                "commercial.adapter.invoke", "ui.presentation.update",
                "audio.playback.invoke", "content.resource.load"
            };

            CollectionAssert.AreEqual(
                codes,
                Enum.GetValues(typeof(ErrorReportCode))
                    .Cast<ErrorReportCode>().ToArray()
            );
            CollectionAssert.AreEqual(
                sites,
                Enum.GetValues(typeof(ErrorReportSiteCode))
                    .Cast<ErrorReportSiteCode>().ToArray()
            );
            for (int i = 0; i < codes.Length; i++)
            {
                var draft = new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    subsystems[i],
                    codes[i],
                    sites[i]
                );
                Assert.That(
                    ErrorReportValidator.ValidateDraft(draft).Accepted,
                    Is.True,
                    codes[i].ToString()
                );
                Assert.That(ErrorReportDeterminism.ErrorCode(codes[i]),
                    Is.EqualTo(errorWireCodes[i]));
                Assert.That(ErrorReportDeterminism.SiteCode(sites[i]),
                    Is.EqualTo(siteWireCodes[i]));
            }
        }

        [Test]
        public void PublicModelsExposeOnlyReviewedImmutableFields()
        {
            string[] draftProperties = PublicPropertyNames(
                typeof(ErrorReportDraft)
            );
            string[] recordProperties = PublicPropertyNames(
                typeof(ErrorReportRecord)
            );
            string[] pipelineProperties = PublicPropertyNames(
                typeof(ErrorReportingPipeline)
            );
            string[] pipelineMethods = typeof(ErrorReportingPipeline)
                .GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly
                )
                .Where(method => !method.IsSpecialName)
                .Select(method => method.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            CollectionAssert.AreEqual(
                new[] { "ErrorCode", "Kind", "SiteCode", "Subsystem" },
                draftProperties
            );
            CollectionAssert.AreEqual(
                new[]
                {
                    "DeliveryAttempts",
                    "DeliveryState",
                    "ErrorCode",
                    "Fingerprint",
                    "FirstUtcTicks",
                    "Kind",
                    "LastUtcTicks",
                    "NextAttemptUtcTicks",
                    "OccurrenceCount",
                    "SchemaVersion",
                    "Sequence",
                    "SiteCode",
                    "Subsystem"
                },
                recordProperties
            );
            CollectionAssert.AreEqual(
                new[]
                {
                    "BufferedCount", "CaptureAllowed",
                    "NetworkTransportAllowed", "Policy"
                },
                pipelineProperties
            );
            CollectionAssert.AreEqual(
                new[]
                {
                    "CanonicalSnapshot", "PruneExpired", "Snapshot",
                    "TryCapture", "TryFlushOne"
                },
                pipelineMethods
            );
            AssertNoPublicSetter(typeof(ErrorReportDraft));
            AssertNoPublicSetter(typeof(ErrorReportRecord));
            Assert.That(
                typeof(ErrorReportDraft).GetFields(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly
                ),
                Is.Empty
            );
            Assert.That(
                typeof(ErrorReportRecord).GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public
                ),
                Is.Empty
            );
        }

        [Test]
        public void Fingerprint_IsPinnedLowerHexAndUsesOnlyCanonicalTuple()
        {
            string first;
            string second;
            string changed;

            Assert.That(
                ErrorReportDeterminism.TryComputeFingerprint(
                    DraftA(),
                    out first
                ),
                Is.True
            );
            Assert.That(
                ErrorReportDeterminism.TryComputeFingerprint(
                    DraftA(),
                    out second
                ),
                Is.True
            );
            Assert.That(
                ErrorReportDeterminism.TryComputeFingerprint(
                    DraftB(),
                    out changed
                ),
                Is.True
            );

            Assert.That(first, Is.EqualTo(ExpectedFingerprint));
            Assert.That(second, Is.EqualTo(first));
            Assert.That(changed, Is.Not.EqualTo(first));
            Assert.That(ErrorReportValidator.IsLowerHexSha256(first), Is.True);
            Assert.That(first, Does.Not.Contain("MATCH3_PLAN_FAILURE"));
            Assert.That(first, Does.Not.Contain("match3.resolve.plan"));
        }

        [Test]
        public void Fingerprint_InvalidDraftReturnsEmptyOutValue()
        {
            string fingerprint = "sentinel";
            bool computed = ErrorReportDeterminism.TryComputeFingerprint(
                new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    ErrorReportSubsystem.Gameplay,
                    (ErrorReportCode)int.MaxValue,
                    ErrorReportSiteCode.Match3ResolvePlan
                ),
                out fingerprint
            );

            Assert.That(computed, Is.False);
            Assert.That(fingerprint, Is.Empty);
        }

        [Test]
        public void RecordValidation_RejectsInvalidStateCombinations()
        {
            ErrorReportRecord valid = Record();
            ErrorReportRecord badSchema = Record(schemaVersion: 2);
            ErrorReportRecord badFingerprint = Record(
                fingerprint: ExpectedFingerprint.ToUpperInvariant()
            );
            ErrorReportRecord badPending = Record(deliveryAttempts: 1);
            ErrorReportRecord badTerminal = Record(
                deliveryState: ErrorReportDeliveryState.RetryExhausted,
                deliveryAttempts: 5,
                nextAttemptUtcTicks: StartTicks
            );
            ErrorReportRecord badRetryTimestamp = Record(
                deliveryState: ErrorReportDeliveryState.RetryScheduled,
                deliveryAttempts: 1,
                nextAttemptUtcTicks: StartTicks
            );
            ErrorReportRecord excessiveAttempts = Record(
                deliveryState: ErrorReportDeliveryState.PermanentlyRejected,
                deliveryAttempts:
                    ErrorReportPolicy.MaximumDeliveryAttemptsLimit + 1,
                nextAttemptUtcTicks: long.MaxValue
            );

            Assert.That(
                ErrorReportValidator.ValidateRecord(valid).Accepted,
                Is.True
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(badSchema).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.SchemaVersionUnsupported)
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(badFingerprint).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.FingerprintInvalid)
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(badPending).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RecordInvalid)
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(badTerminal).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RecordInvalid)
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(badRetryTimestamp).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RecordInvalid)
            );
            Assert.That(
                ErrorReportValidator.ValidateRecord(excessiveAttempts).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RecordInvalid)
            );
        }

        [Test]
        public void Capture_AssignsMonotonicSequenceAndStableOrder()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 100L, 3, 10L, 40L)
            );

            ErrorReportOperationResult first = buffer.TryCapture(
                DraftA(),
                StartTicks
            );
            ErrorReportOperationResult second = buffer.TryCapture(
                DraftB(),
                StartTicks + 1L
            );
            IReadOnlyList<ErrorReportRecord> snapshot = buffer.Snapshot();

            Assert.That(first.Accepted, Is.True, first.ReasonCode);
            Assert.That(second.Accepted, Is.True, second.ReasonCode);
            Assert.That(snapshot.Count, Is.EqualTo(2));
            Assert.That(snapshot[0].Sequence, Is.EqualTo(1L));
            Assert.That(snapshot[1].Sequence, Is.EqualTo(2L));
            Assert.That(snapshot[0].ErrorCode,
                Is.EqualTo(ErrorReportCode.Match3PlanFailure));
            Assert.That(snapshot[1].ErrorCode,
                Is.EqualTo(ErrorReportCode.SaveWriteFailure));
        }

        [Test]
        public void Capacity_RejectsNewRecordAndPreservesExistingBytes()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(1, 10L, 100L, 3, 10L, 40L)
            );
            Assert.That(buffer.TryCapture(DraftA(), StartTicks).Accepted,
                Is.True);
            string before = buffer.CanonicalSnapshot();

            ErrorReportOperationResult rejected = buffer.TryCapture(
                DraftB(),
                StartTicks + 1L
            );

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(rejected.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.CapacityReached));
            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(buffer.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void DuplicateAtCapacity_CoalescesAtInclusiveWindowBoundary()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(1, 10L, 100L, 3, 10L, 40L)
            );
            buffer.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult duplicate = buffer.TryCapture(
                DraftA(),
                StartTicks + 10L
            );
            ErrorReportRecord record = buffer.Snapshot()[0];

            Assert.That(duplicate.Accepted, Is.True, duplicate.ReasonCode);
            Assert.That(duplicate.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Deduplicated));
            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(record.Sequence, Is.EqualTo(1L));
            Assert.That(record.FirstUtcTicks, Is.EqualTo(StartTicks));
            Assert.That(record.LastUtcTicks, Is.EqualTo(StartTicks + 10L));
            Assert.That(record.OccurrenceCount, Is.EqualTo(2L));
        }

        [Test]
        public void OneTickOutsideDedupWindow_CreatesANewSequence()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 100L, 3, 10L, 40L)
            );
            buffer.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult result = buffer.TryCapture(
                DraftA(),
                StartTicks + 11L
            );

            Assert.That(result.Accepted, Is.True, result.ReasonCode);
            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Captured));
            Assert.That(result.Sequence, Is.EqualTo(2L));
            Assert.That(buffer.Count, Is.EqualTo(2));
        }

        [Test]
        public void DifferentCanonicalTuple_DoesNotDeduplicate()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 100L, 3, 10L, 40L)
            );

            buffer.TryCapture(DraftA(), StartTicks);
            ErrorReportOperationResult result = buffer.TryCapture(
                DraftB(),
                StartTicks + 1L
            );

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Captured));
            Assert.That(buffer.Count, Is.EqualTo(2));
            Assert.That(buffer.Snapshot()[0].Fingerprint,
                Is.Not.EqualTo(buffer.Snapshot()[1].Fingerprint));
        }

        [Test]
        public void TimestampRollback_IsRejectedWithoutMutation()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks + 10L);
            string before = buffer.CanonicalSnapshot();

            ErrorReportOperationResult result = buffer.TryCapture(
                DraftB(),
                StartTicks
            );

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TimestampRollback));
            Assert.That(buffer.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void HardRetention_IsFirstSeenBasedAndBoundaryInclusive()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 20L, 3, 2L, 8L)
            );
            buffer.TryCapture(DraftA(), StartTicks);
            buffer.TryCapture(DraftA(), StartTicks + 9L);

            Assert.That(buffer.PruneExpired(StartTicks + 19L), Is.Zero);
            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(buffer.PruneExpired(StartTicks + 20L), Is.EqualTo(1));
            Assert.That(buffer.Count, Is.Zero);
        }

        [Test]
        public void Snapshot_IsDetachedReadOnlyAndByteStable()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks);

            IReadOnlyList<ErrorReportRecord> first = buffer.Snapshot();
            IReadOnlyList<ErrorReportRecord> second = buffer.Snapshot();
            string before = ErrorReportDeterminism.SerializeTrace(first);

            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(first[0], Is.Not.SameAs(second[0]));
            Assert.That(first, Is.InstanceOf<System.Collections.ObjectModel.ReadOnlyCollection<ErrorReportRecord>>());
            Assert.That(ErrorReportDeterminism.SerializeTrace(second),
                Is.EqualTo(before));
            Assert.That(buffer.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void CanonicalJson_HasExactPinnedKeyOrderAndCompactShape()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks);

            string expected =
                "[{\"schemaVersion\":1,\"sequence\":1," +
                "\"fingerprint\":\"" + ExpectedFingerprint + "\"," +
                "\"kind\":\"nonfatal\",\"subsystem\":\"gameplay\"," +
                "\"errorCode\":\"MATCH3_PLAN_FAILURE\"," +
                "\"siteCode\":\"match3.resolve.plan\"," +
                "\"firstUtcTicks\":" + StartTicks + "," +
                "\"lastUtcTicks\":" + StartTicks + "," +
                "\"occurrenceCount\":1,\"deliveryState\":\"pending\"," +
                "\"deliveryAttempts\":0,\"nextAttemptUtcTicks\":" +
                StartTicks + "}]";
            string actual = buffer.CanonicalSnapshot();

            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actual, Does.Not.Contain("\n"));
            Assert.That(actual, Does.Not.Contain("\r"));
            Assert.That(actual, Does.Not.Contain("message"));
            Assert.That(actual, Does.Not.Contain("stack"));
            Assert.That(actual, Does.Not.Contain("payload"));
        }

        [Test]
        public void CanonicalTrace_OrdersRecordsBySequence()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks);
            buffer.TryCapture(DraftB(), StartTicks + 1L);

            IReadOnlyList<ErrorReportRecord> snapshot = buffer.Snapshot();
            string json = ErrorReportDeterminism.SerializeTrace(
                new[] { snapshot[1], snapshot[0] }
            );

            Assert.That(
                json.IndexOf("\"sequence\":1", StringComparison.Ordinal),
                Is.LessThan(
                    json.IndexOf("\"sequence\":2", StringComparison.Ordinal)
                )
            );
        }

        [Test]
        public void CaptureDisabledAndInvalidPolicy_FailWithoutMutation()
        {
            var disabled = new ErrorReportingPipeline(false);
            var invalid = new ErrorReportingPipeline(
                true,
                Policy(0, 10L, 100L, 3, 10L, 40L)
            );

            ErrorReportOperationResult denied = disabled.TryCapture(
                DraftA(),
                StartTicks
            );
            ErrorReportOperationResult policyFailure = invalid.TryCapture(
                DraftA(),
                StartTicks
            );

            Assert.That(denied.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.CaptureNotAllowed));
            Assert.That(policyFailure.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.PolicyInvalid));
            Assert.That(disabled.BufferedCount, Is.Zero);
            Assert.That(invalid.BufferedCount, Is.Zero);
        }

        [Test]
        public void OfflineFlush_DoesNotCallTransportOrMutateRetryState()
        {
            var transport = new ProbeTransport();
            var pipeline = new ErrorReportingPipeline(
                true,
                null,
                transport
            );
            pipeline.TryCapture(DraftA(), StartTicks);
            string before = pipeline.CanonicalSnapshot();

            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks,
                false
            );

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Offline));
            Assert.That(transport.SendCalls, Is.Zero);
            Assert.That(transport.CapabilityReads, Is.Zero);
            Assert.That(pipeline.CanonicalSnapshot(), Is.EqualTo(before));
        }

        [Test]
        public void DisabledTransport_IsStableAndPreservesRecord()
        {
            var pipeline = new ErrorReportingPipeline(true);
            pipeline.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks,
                true
            );

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportDisabled));
            Assert.That(pipeline.BufferedCount, Is.EqualTo(1));
            Assert.That(pipeline.Snapshot()[0].DeliveryAttempts, Is.Zero);
            Assert.That(pipeline.NetworkTransportAllowed, Is.False);
        }

        [Test]
        public void NetworkCapableTransport_IsBlockedBeforeSend()
        {
            var transport = new ProbeTransport { Network = true };
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks,
                true
            );

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.NetworkTransportBlocked));
            Assert.That(transport.SendCalls, Is.Zero);
            Assert.That(pipeline.Snapshot()[0].DeliveryAttempts, Is.Zero);
        }

        [Test]
        public void ThrowingCapabilityReads_AreContainedWithoutMutation()
        {
            var captureThrow = new ProbeTransport
            {
                ThrowOnDeliveryEnabled = true
            };
            var networkThrow = new ProbeTransport
            {
                ThrowOnSupportsNetwork = true
            };
            var first = PipelineWithOne(captureThrow);
            var second = PipelineWithOne(networkThrow);
            string firstBefore = first.CanonicalSnapshot();
            string secondBefore = second.CanonicalSnapshot();

            Assert.That(first.TryFlushOne(StartTicks, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(second.TryFlushOne(StartTicks, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(first.CanonicalSnapshot(), Is.EqualTo(firstBefore));
            Assert.That(second.CanonicalSnapshot(), Is.EqualTo(secondBefore));
            Assert.That(captureThrow.SendCalls, Is.Zero);
            Assert.That(networkThrow.SendCalls, Is.Zero);
        }

        [Test]
        public void RetryableFailure_SchedulesBackoffAndEventuallyDelivers()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult failed = pipeline.TryFlushOne(
                StartTicks,
                true
            );
            ErrorReportRecord scheduled = pipeline.Snapshot()[0];
            ErrorReportOperationResult early = pipeline.TryFlushOne(
                scheduled.NextAttemptUtcTicks - 1L,
                true
            );
            ErrorReportOperationResult delivered = pipeline.TryFlushOne(
                scheduled.NextAttemptUtcTicks,
                true
            );

            Assert.That(failed.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RetryScheduled));
            Assert.That(scheduled.DeliveryAttempts, Is.EqualTo(1));
            Assert.That(
                scheduled.NextAttemptUtcTicks,
                Is.EqualTo(
                    StartTicks +
                    ErrorReportPolicy.DefaultInitialRetryDelayTicks
                )
            );
            Assert.That(early.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.NothingDue));
            Assert.That(transport.SendCalls, Is.EqualTo(2));
            Assert.That(delivered.Accepted, Is.True, delivered.ReasonCode);
            Assert.That(delivered.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Delivered));
            Assert.That(pipeline.BufferedCount, Is.Zero);
        }

        [Test]
        public void RetryBackoffCapsAndExhaustionStopsFurtherSends()
        {
            var policy = Policy(2, 10L, 1000L, 3, 10L, 20L);
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            var pipeline = new ErrorReportingPipeline(true, policy, transport);
            pipeline.TryCapture(DraftA(), 100L);

            Assert.That(pipeline.TryFlushOne(100L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RetryScheduled));
            Assert.That(pipeline.Snapshot()[0].NextAttemptUtcTicks,
                Is.EqualTo(110L));
            Assert.That(pipeline.TryFlushOne(110L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RetryScheduled));
            Assert.That(pipeline.Snapshot()[0].NextAttemptUtcTicks,
                Is.EqualTo(130L));
            Assert.That(pipeline.TryFlushOne(130L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.RetryExhausted));

            ErrorReportRecord exhausted = pipeline.Snapshot()[0];
            Assert.That(exhausted.DeliveryAttempts, Is.EqualTo(3));
            Assert.That(exhausted.DeliveryState,
                Is.EqualTo(ErrorReportDeliveryState.RetryExhausted));
            Assert.That(exhausted.NextAttemptUtcTicks, Is.EqualTo(long.MaxValue));
            Assert.That(pipeline.TryFlushOne(200L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.NothingDue));
            Assert.That(transport.SendCalls, Is.EqualTo(3));
        }

        [Test]
        public void PermanentFailure_IsTerminalAndDoesNotLoop()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.PermanentFailure());
            var pipeline = PipelineWithOne(transport);

            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks,
                true
            );
            ErrorReportRecord terminal = pipeline.Snapshot()[0];

            Assert.That(result.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.PermanentlyRejected));
            Assert.That(terminal.DeliveryState,
                Is.EqualTo(ErrorReportDeliveryState.PermanentlyRejected));
            Assert.That(terminal.DeliveryAttempts, Is.EqualTo(1));
            Assert.That(terminal.NextAttemptUtcTicks, Is.EqualTo(long.MaxValue));
            Assert.That(
                pipeline.TryFlushOne(StartTicks + 1L, true).ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.NothingDue)
            );
            Assert.That(transport.SendCalls, Is.EqualTo(1));
        }

        [Test]
        public void ThrowingAndNullTransportResults_AreContainedAndRetryable()
        {
            var throwingTransport = new ProbeTransport { ThrowOnSend = true };
            var nullTransport = new ProbeTransport { ReturnNull = true };
            var throwingPipeline = PipelineWithOne(throwingTransport);
            var nullPipeline = PipelineWithOne(nullTransport);

            ErrorReportOperationResult thrown = throwingPipeline.TryFlushOne(
                StartTicks,
                true
            );
            ErrorReportOperationResult returnedNull = nullPipeline.TryFlushOne(
                StartTicks,
                true
            );

            Assert.That(thrown.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(returnedNull.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.TransportFailure));
            Assert.That(throwingPipeline.Snapshot()[0].DeliveryState,
                Is.EqualTo(ErrorReportDeliveryState.RetryScheduled));
            Assert.That(nullPipeline.Snapshot()[0].DeliveryState,
                Is.EqualTo(ErrorReportDeliveryState.RetryScheduled));
            Assert.That(throwingPipeline.Snapshot()[0].DeliveryAttempts,
                Is.EqualTo(1));
            Assert.That(nullPipeline.Snapshot()[0].DeliveryAttempts,
                Is.EqualTo(1));
        }

        [Test]
        public void Flush_DeliversAtMostOneOldestDueRecordPerCall()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);
            pipeline.TryCapture(DraftB(), StartTicks + 1L);

            ErrorReportOperationResult first = pipeline.TryFlushOne(
                StartTicks + 1L,
                true
            );

            Assert.That(first.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Delivered));
            Assert.That(transport.SendCalls, Is.EqualTo(1));
            Assert.That(transport.SentSequences, Is.EqualTo(new[] { 1L }));
            Assert.That(pipeline.BufferedCount, Is.EqualTo(1));
            Assert.That(pipeline.Snapshot()[0].Sequence, Is.EqualTo(2L));

            pipeline.TryFlushOne(StartTicks + 1L, true);
            Assert.That(transport.SentSequences, Is.EqualTo(new[] { 1L, 2L }));
            Assert.That(pipeline.BufferedCount, Is.Zero);
        }

        [Test]
        public void ReentrantCaptureFromTransport_IsBlockedAndOuterSendCompletes()
        {
            var transport = new ProbeTransport();
            ErrorReportingPipeline pipeline = null;
            ErrorReportOperationResult nested = null;
            transport.OnSend = _ =>
            {
                nested = pipeline.TryCapture(DraftB(), StartTicks + 1L);
            };
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);

            ErrorReportOperationResult outer = pipeline.TryFlushOne(
                StartTicks,
                true
            );

            Assert.That(nested, Is.Not.Null);
            Assert.That(nested.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.ReentrantOperationBlocked));
            Assert.That(outer.ReasonCode,
                Is.EqualTo(ErrorReportReasonCodes.Delivered));
            Assert.That(pipeline.BufferedCount, Is.Zero);
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

        static ErrorReportPolicy Policy(
            int capacity,
            long deduplicationWindowTicks,
            long retentionTicks,
            int maximumDeliveryAttempts,
            long initialRetryDelayTicks,
            long maximumRetryDelayTicks
        )
        {
            return new ErrorReportPolicy(
                capacity,
                deduplicationWindowTicks,
                retentionTicks,
                maximumDeliveryAttempts,
                initialRetryDelayTicks,
                maximumRetryDelayTicks
            );
        }

        static ErrorReportRecord Record(
            int schemaVersion = ErrorReportSchema.CurrentSchemaVersion,
            string fingerprint = ExpectedFingerprint,
            ErrorReportDeliveryState deliveryState =
                ErrorReportDeliveryState.Pending,
            int deliveryAttempts = 0,
            long nextAttemptUtcTicks = StartTicks
        )
        {
            return new ErrorReportRecord(
                schemaVersion,
                1L,
                fingerprint,
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Gameplay,
                ErrorReportCode.Match3PlanFailure,
                ErrorReportSiteCode.Match3ResolvePlan,
                StartTicks,
                StartTicks,
                1L,
                deliveryState,
                deliveryAttempts,
                nextAttemptUtcTicks
            );
        }

        static ErrorReportingPipeline PipelineWithOne(
            IErrorReportTransport transport
        )
        {
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            ErrorReportOperationResult capture = pipeline.TryCapture(
                DraftA(),
                StartTicks
            );
            Assert.That(capture.Accepted, Is.True, capture.ReasonCode);
            return pipeline;
        }

        static string[] PublicPropertyNames(Type type)
        {
            return type.GetProperties(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly
                )
                .Select(property => property.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }

        static void AssertNoPublicSetter(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly
            );
            for (int i = 0; i < properties.Length; i++)
            {
                Assert.That(
                    properties[i].GetSetMethod(false),
                    Is.Null,
                    properties[i].Name
                );
            }
        }

        sealed class ProbeTransport : IErrorReportTransport
        {
            readonly Queue<ErrorReportTransportResult> results =
                new Queue<ErrorReportTransportResult>();
            readonly List<long> sentSequences = new List<long>();

            public bool Enabled = true;
            public bool Network;
            public bool ThrowOnDeliveryEnabled;
            public bool ThrowOnSupportsNetwork;
            public bool ThrowOnSend;
            public bool ReturnNull;
            public Action<ErrorReportRecord> OnSend;
            public int CapabilityReads;
            public int SendCalls;

            public string TransportId { get { return "test_probe"; } }

            public bool DeliveryEnabled
            {
                get
                {
                    CapabilityReads++;
                    if (ThrowOnDeliveryEnabled)
                        throw new InvalidOperationException("capture");
                    return Enabled;
                }
            }

            public bool SupportsNetwork
            {
                get
                {
                    CapabilityReads++;
                    if (ThrowOnSupportsNetwork)
                        throw new InvalidOperationException("network");
                    return Network;
                }
            }

            public IReadOnlyList<long> SentSequences
            {
                get { return sentSequences.AsReadOnly(); }
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
                sentSequences.Add(record.Sequence);
                if (OnSend != null)
                    OnSend(record.DetachedCopy());
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
