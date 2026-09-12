#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LegendsOfRus.AndroidBuild;
using LegendsOfRus.CommercialRuntime;
using LegendsOfRus.DiagnosticsCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17CrashErrorReportingFoundationSelfTest
    {
        const long StartTicks = 638923680000000000L;
        const string ExpectedFingerprint =
            "40a6e5321af214d439966d57d8037278c2abe7921066e4b27e596b257f52afd7";

        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 3 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("schema and policy constants are pinned",
                SchemaAndPolicyConstantsArePinned,
                ref passed, ref total, failures);
            Check("canonical minimized draft validates",
                CanonicalDraftValidates,
                ref passed, ref total, failures);
            Check("public models expose only reviewed immutable fields",
                PublicModelsAreMinimized,
                ref passed, ref total, failures);
            Check("drafts accept only the reviewed typed code catalog",
                DraftCatalogIsClosed,
                ref passed, ref total, failures);
            Check("fingerprint is deterministic and pinned",
                FingerprintIsDeterministic,
                ref passed, ref total, failures);
            Check("invalid fingerprint input returns an empty out value",
                InvalidFingerprintReturnsEmpty,
                ref passed, ref total, failures);
            Check("buffer is bounded and preserves accepted bytes",
                BufferIsBounded,
                ref passed, ref total, failures);
            Check("duplicate coalesces at the inclusive window boundary",
                DuplicateBoundaryIsInclusive,
                ref passed, ref total, failures);
            Check("one tick outside dedup creates a new sequence",
                OutsideDedupCreatesNewSequence,
                ref passed, ref total, failures);
            Check("hard retention remains based on first occurrence",
                HardRetentionUsesFirstOccurrence,
                ref passed, ref total, failures);
            Check("clock rollback leaves the buffer unchanged",
                ClockRollbackIsNonMutating,
                ref passed, ref total, failures);
            Check("canonical JSON has exact minimized ordering",
                CanonicalJsonIsMinimizedAndOrdered,
                ref passed, ref total, failures);
            Check("capture-disabled pipeline fails without state",
                CaptureDisabledFailsClosed,
                ref passed, ref total, failures);
            Check("offline flush calls no transport and changes no retry",
                OfflineFlushIsNonMutating,
                ref passed, ref total, failures);
            Check("shipped disabled transport preserves local evidence",
                DisabledTransportPreservesEvidence,
                ref passed, ref total, failures);
            Check("network-capable transport is blocked before send",
                NetworkTransportIsBlocked,
                ref passed, ref total, failures);
            Check("retry scheduling uses exact injected-time backoff",
                RetrySchedulingIsExact,
                ref passed, ref total, failures);
            Check("retry exhaustion becomes terminal",
                RetryExhaustionIsTerminal,
                ref passed, ref total, failures);
            Check("permanent failure does not retry-loop",
                PermanentFailureIsTerminal,
                ref passed, ref total, failures);
            Check("throwing capability access is contained",
                ThrowingCapabilityIsContained,
                ref passed, ref total, failures);
            Check("throwing send is contained as retryable state",
                ThrowingSendIsContained,
                ref passed, ref total, failures);
            Check("null transport result is contained as retryable state",
                NullTransportResultIsContained,
                ref passed, ref total, failures);
            Check("reentrant capture is blocked without losing outer send",
                ReentrantCaptureIsBlocked,
                ref passed, ref total, failures);
            Check("diagnostics core has no Unity persistence or network API",
                DiagnosticsCoreIsDetached,
                ref passed, ref total, failures);
            Check("no Unity log subscription exists in production scripts",
                ProductionScriptsHaveNoLogSubscription,
                ref passed, ref total, failures);
            Check("compiled network transport gate remains false",
                NetworkTransportCompiledRemainsFalse,
                ref passed, ref total, failures);
            Check("v1.17 Step 2 entry evidence remains unretained",
                Step2EntryEvidenceRemainsUnretained,
                ref passed, ref total, failures);
            Check("runtime economy activation remains disabled",
                EconomyActivationRemainsDisabled,
                ref passed, ref total, failures);
            Check("commercial provider activation remains disabled",
                CommercialActivationRemainsDisabled,
                ref passed, ref total, failures);
            Check("production Android activation remains disabled",
                AndroidProductionRemainsDisabled,
                ref passed, ref total, failures);
            Check("resolved package-lock approval remains false",
                ResolvedPackageLockApprovalRemainsFalse,
                ref passed, ref total, failures);
            Check("Unity version boundary remains exactly 6000.5.9f1",
                UnityVersionBoundaryIsExact,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.17 Step 3 crash/error reporting foundation " +
                    "self-tests passed: " + passed + "/" + total +
                    ". Process-memory diagnostics remain detached; network, " +
                    "providers, persistence, package-lock approval, economy, " +
                    "commercial, and production Android activation are blocked."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        "v1.17 Step 3 self-tests passed: " + passed + "/" +
                        total + ".\n\nThis Editor self-test does not claim " +
                        "Test Runner, device, retained Step 2 entry, provider, " +
                        "network, persistence, or activation evidence.",
                        "OK"
                    );
                }
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.17 Step 3 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary
            );
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Test failures", summary, "OK");
            throw new InvalidOperationException(
                "v1.17 Step 3 self-tests failed."
            );
        }

        static bool SchemaAndPolicyConstantsArePinned()
        {
            ErrorReportPolicy value = ErrorReportPolicy.Default;
            return
                ErrorReportSchema.CurrentSchemaVersion == 1 &&
                ErrorReportSchema.MaximumErrorCodeLength == 64 &&
                ErrorReportSchema.MaximumSiteCodeLength == 64 &&
                value.IsValid &&
                value.Capacity == ErrorReportPolicy.DefaultCapacity &&
                ErrorReportPolicy.MaximumCapacity == 256 &&
                value.DeduplicationWindowTicks ==
                    10L * TimeSpan.TicksPerMinute &&
                value.RetentionTicks == 7L * TimeSpan.TicksPerDay &&
                ErrorReportPolicy.MaximumRetentionTicks ==
                    30L * TimeSpan.TicksPerDay &&
                value.MaximumDeliveryAttempts == 5 &&
                value.RetryDelayTicks(1) ==
                    30L * TimeSpan.TicksPerSecond &&
                value.RetryDelayTicks(100) == TimeSpan.TicksPerHour;
        }

        static bool CanonicalDraftValidates()
        {
            ErrorReportOperationResult value =
                ErrorReportValidator.ValidateDraft(DraftA());
            return value != null && value.Accepted && value.Succeeded;
        }

        static bool PublicModelsAreMinimized()
        {
            string[] draft = PublicPropertyNames(typeof(ErrorReportDraft));
            string[] record = PublicPropertyNames(typeof(ErrorReportRecord));
            string[] pipelineProperties = PublicPropertyNames(
                typeof(ErrorReportingPipeline));
            string[] pipelineMethods = typeof(ErrorReportingPipeline)
                .GetMethods(
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .Select(method => method.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            return
                draft.SequenceEqual(new[]
                    { "ErrorCode", "Kind", "SiteCode", "Subsystem" }) &&
                record.SequenceEqual(new[]
                {
                    "DeliveryAttempts", "DeliveryState", "ErrorCode",
                    "Fingerprint", "FirstUtcTicks", "Kind", "LastUtcTicks",
                    "NextAttemptUtcTicks", "OccurrenceCount", "SchemaVersion",
                    "Sequence", "SiteCode", "Subsystem"
                }) &&
                HasNoPublicSetter(typeof(ErrorReportDraft)) &&
                HasNoPublicSetter(typeof(ErrorReportRecord)) &&
                pipelineProperties.SequenceEqual(new[]
                {
                    "BufferedCount", "CaptureAllowed",
                    "NetworkTransportAllowed", "Policy"
                }) &&
                pipelineMethods.SequenceEqual(new[]
                {
                    "CanonicalSnapshot", "PruneExpired", "Snapshot",
                    "TryCapture", "TryFlushOne"
                }) &&
                typeof(ErrorReportRecord).GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public).Length == 0 &&
                typeof(ErrorReportingPipeline).GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public)
                    .All(constructor => constructor.GetParameters()
                        .All(parameter => parameter.ParameterType !=
                            typeof(IErrorReportTransport)));
        }

        static bool DraftCatalogIsClosed()
        {
            ConstructorInfo constructor = typeof(ErrorReportDraft)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Single();
            Type[] parameters = constructor.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();
            ErrorReportOperationResult unknownError =
                ErrorReportValidator.ValidateDraft(new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    ErrorReportSubsystem.Gameplay,
                    (ErrorReportCode)int.MaxValue,
                    ErrorReportSiteCode.Match3ResolvePlan));
            ErrorReportOperationResult unknownSite =
                ErrorReportValidator.ValidateDraft(new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    ErrorReportSubsystem.Gameplay,
                    ErrorReportCode.Match3PlanFailure,
                    (ErrorReportSiteCode)int.MaxValue));
            ErrorReportOperationResult mismatch =
                ErrorReportValidator.ValidateDraft(new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    ErrorReportSubsystem.Save,
                    ErrorReportCode.Match3PlanFailure,
                    ErrorReportSiteCode.Match3ResolvePlan));
            var codes = (ErrorReportCode[])Enum.GetValues(
                typeof(ErrorReportCode));
            var sites = (ErrorReportSiteCode[])Enum.GetValues(
                typeof(ErrorReportSiteCode));
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
            var expectedErrorWire = new[]
            {
                "BOOTSTRAP_FAILURE", "RUNTIME_INVARIANT_FAILURE",
                "MANAGED_CRASH_UNHANDLED", "MATCH3_PLAN_FAILURE",
                "SAVE_READ_FAILURE", "SAVE_WRITE_FAILURE",
                "SAVE_MIGRATION_FAILURE", "ECONOMY_TRANSACTION_FAILURE",
                "COMMERCIAL_ADAPTER_FAILURE", "UI_PRESENTATION_FAILURE",
                "AUDIO_PLAYBACK_FAILURE", "CONTENT_LOAD_FAILURE"
            };
            var expectedSiteWire = new[]
            {
                "bootstrap.initialize", "runtime.invariant.check",
                "runtime.unhandled", "match3.resolve.plan",
                "save.read.decode", "save.write.commit",
                "save.migration.apply", "economy.transaction.commit",
                "commercial.adapter.invoke", "ui.presentation.update",
                "audio.playback.invoke", "content.resource.load"
            };
            if (codes.Length != 12 || sites.Length != 12)
                return false;
            for (int i = 0; i < codes.Length; i++)
            {
                if ((int)codes[i] != i || (int)sites[i] != i ||
                    ErrorReportDeterminism.ErrorCode(codes[i]) !=
                        expectedErrorWire[i] ||
                    ErrorReportDeterminism.SiteCode(sites[i]) !=
                        expectedSiteWire[i] ||
                    !ErrorReportValidator.ValidateDraft(new ErrorReportDraft(
                        ErrorReportKind.NonFatal,
                        subsystems[i],
                        codes[i],
                        sites[i])).Accepted)
                    return false;
            }
            return
                parameters.SequenceEqual(new[]
                {
                    typeof(ErrorReportKind), typeof(ErrorReportSubsystem),
                    typeof(ErrorReportCode), typeof(ErrorReportSiteCode)
                }) &&
                !parameters.Contains(typeof(string)) &&
                unknownError.ReasonCode ==
                    ErrorReportReasonCodes.ErrorCodeInvalid &&
                unknownSite.ReasonCode ==
                    ErrorReportReasonCodes.SiteCodeInvalid &&
                mismatch.ReasonCode ==
                    ErrorReportReasonCodes.CodeTupleInvalid;
        }

        static bool FingerprintIsDeterministic()
        {
            string first;
            string second;
            return
                ErrorReportDeterminism.TryComputeFingerprint(
                    DraftA(), out first) &&
                ErrorReportDeterminism.TryComputeFingerprint(
                    DraftA(), out second) &&
                first == ExpectedFingerprint &&
                second == first &&
                ErrorReportValidator.IsLowerHexSha256(first) &&
                !first.Contains("MATCH3_PLAN_FAILURE") &&
                !first.Contains("match3.resolve.plan");
        }

        static bool InvalidFingerprintReturnsEmpty()
        {
            string value = "sentinel";
            bool computed = ErrorReportDeterminism.TryComputeFingerprint(
                new ErrorReportDraft(
                    ErrorReportKind.NonFatal,
                    ErrorReportSubsystem.Gameplay,
                    (ErrorReportCode)int.MaxValue,
                    ErrorReportSiteCode.Match3ResolvePlan
                ),
                out value
            );
            return !computed && value == string.Empty;
        }

        static bool BufferIsBounded()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(1, 10L, 100L, 3, 10L, 40L));
            ErrorReportOperationResult first = buffer.TryCapture(
                DraftA(), StartTicks);
            string before = buffer.CanonicalSnapshot();
            ErrorReportOperationResult second = buffer.TryCapture(
                DraftB(), StartTicks + 1L);
            return
                first.Accepted &&
                second.ReasonCode == ErrorReportReasonCodes.CapacityReached &&
                buffer.Count == 1 &&
                buffer.CanonicalSnapshot() == before;
        }

        static bool DuplicateBoundaryIsInclusive()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(1, 10L, 100L, 3, 10L, 40L));
            buffer.TryCapture(DraftA(), StartTicks);
            ErrorReportOperationResult result = buffer.TryCapture(
                DraftA(), StartTicks + 10L);
            ErrorReportRecord record = buffer.Snapshot()[0];
            return
                result.ReasonCode == ErrorReportReasonCodes.Deduplicated &&
                buffer.Count == 1 &&
                record.Sequence == 1L &&
                record.FirstUtcTicks == StartTicks &&
                record.LastUtcTicks == StartTicks + 10L &&
                record.OccurrenceCount == 2L;
        }

        static bool OutsideDedupCreatesNewSequence()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 100L, 3, 10L, 40L));
            buffer.TryCapture(DraftA(), StartTicks);
            ErrorReportOperationResult result = buffer.TryCapture(
                DraftA(), StartTicks + 11L);
            return result.Accepted && result.Sequence == 2L && buffer.Count == 2;
        }

        static bool HardRetentionUsesFirstOccurrence()
        {
            var buffer = new BoundedErrorReportBuffer(
                Policy(2, 10L, 20L, 3, 2L, 8L));
            buffer.TryCapture(DraftA(), StartTicks);
            buffer.TryCapture(DraftA(), StartTicks + 9L);
            return
                buffer.PruneExpired(StartTicks + 19L) == 0 &&
                buffer.Count == 1 &&
                buffer.PruneExpired(StartTicks + 20L) == 1 &&
                buffer.Count == 0;
        }

        static bool ClockRollbackIsNonMutating()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks + 10L);
            string before = buffer.CanonicalSnapshot();
            ErrorReportOperationResult result = buffer.TryCapture(
                DraftB(), StartTicks);
            return
                result.ReasonCode == ErrorReportReasonCodes.TimestampRollback &&
                buffer.CanonicalSnapshot() == before;
        }

        static bool CanonicalJsonIsMinimizedAndOrdered()
        {
            var buffer = new BoundedErrorReportBuffer();
            buffer.TryCapture(DraftA(), StartTicks);
            string value = buffer.CanonicalSnapshot();
            string[] keys =
            {
                "schemaVersion", "sequence", "fingerprint", "kind",
                "subsystem", "errorCode", "siteCode", "firstUtcTicks",
                "lastUtcTicks", "occurrenceCount", "deliveryState",
                "deliveryAttempts", "nextAttemptUtcTicks"
            };
            int previous = -1;
            for (int i = 0; i < keys.Length; i++)
            {
                int index = value.IndexOf(
                    "\"" + keys[i] + "\"",
                    StringComparison.Ordinal
                );
                if (index <= previous)
                    return false;
                previous = index;
            }
            string lower = value.ToLowerInvariant();
            return
                !value.Contains("\n") && !value.Contains("\r") &&
                !lower.Contains("message") && !lower.Contains("stack") &&
                !lower.Contains("payload") && !lower.Contains("player") &&
                !lower.Contains("device") && !lower.Contains("receipt") &&
                !lower.Contains("token");
        }

        static bool CaptureDisabledFailsClosed()
        {
            var pipeline = new ErrorReportingPipeline(false);
            ErrorReportOperationResult result = pipeline.TryCapture(
                DraftA(), StartTicks);
            return
                result.ReasonCode ==
                    ErrorReportReasonCodes.CaptureNotAllowed &&
                pipeline.BufferedCount == 0;
        }

        static bool OfflineFlushIsNonMutating()
        {
            var transport = new ProbeTransport();
            var pipeline = PipelineWithOne(transport);
            string before = pipeline.CanonicalSnapshot();
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, false);
            return
                result.ReasonCode == ErrorReportReasonCodes.Offline &&
                transport.CapabilityReads == 0 &&
                transport.SendCalls == 0 &&
                pipeline.CanonicalSnapshot() == before;
        }

        static bool DisabledTransportPreservesEvidence()
        {
            var pipeline = new ErrorReportingPipeline(true);
            pipeline.TryCapture(DraftA(), StartTicks);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            return
                result.ReasonCode ==
                    ErrorReportReasonCodes.TransportDisabled &&
                DisabledErrorReportTransport.Instance.TransportId ==
                    "disabled" &&
                !DisabledErrorReportTransport.Instance.DeliveryEnabled &&
                pipeline.BufferedCount == 1 &&
                pipeline.Snapshot()[0].DeliveryAttempts == 0 &&
                !pipeline.NetworkTransportAllowed;
        }

        static bool NetworkTransportIsBlocked()
        {
            var transport = new ProbeTransport { Network = true };
            var pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            return
                result.ReasonCode ==
                    ErrorReportReasonCodes.NetworkTransportBlocked &&
                transport.SendCalls == 0 &&
                pipeline.Snapshot()[0].DeliveryAttempts == 0;
        }

        static bool RetrySchedulingIsExact()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            var pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult failed = pipeline.TryFlushOne(
                StartTicks, true);
            ErrorReportRecord scheduled = pipeline.Snapshot()[0];
            ErrorReportOperationResult early = pipeline.TryFlushOne(
                scheduled.NextAttemptUtcTicks - 1L, true);
            ErrorReportOperationResult delivered = pipeline.TryFlushOne(
                scheduled.NextAttemptUtcTicks, true);
            return
                failed.ReasonCode ==
                    ErrorReportReasonCodes.RetryScheduled &&
                scheduled.DeliveryAttempts == 1 &&
                scheduled.NextAttemptUtcTicks == StartTicks +
                    ErrorReportPolicy.DefaultInitialRetryDelayTicks &&
                early.ReasonCode == ErrorReportReasonCodes.NothingDue &&
                delivered.ReasonCode == ErrorReportReasonCodes.Delivered &&
                transport.SendCalls == 2 &&
                pipeline.BufferedCount == 0;
        }

        static bool RetryExhaustionIsTerminal()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            transport.Enqueue(ErrorReportTransportResult.RetryableFailure());
            var pipeline = new ErrorReportingPipeline(
                true,
                Policy(2, 10L, 1000L, 3, 10L, 20L),
                transport
            );
            pipeline.TryCapture(DraftA(), 100L);
            pipeline.TryFlushOne(100L, true);
            pipeline.TryFlushOne(110L, true);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                130L, true);
            ErrorReportRecord record = pipeline.Snapshot()[0];
            ErrorReportOperationResult later = pipeline.TryFlushOne(
                200L, true);
            return
                result.ReasonCode ==
                    ErrorReportReasonCodes.RetryExhausted &&
                record.DeliveryState ==
                    ErrorReportDeliveryState.RetryExhausted &&
                record.DeliveryAttempts == 3 &&
                record.NextAttemptUtcTicks == long.MaxValue &&
                later.ReasonCode == ErrorReportReasonCodes.NothingDue &&
                transport.SendCalls == 3;
        }

        static bool PermanentFailureIsTerminal()
        {
            var transport = new ProbeTransport();
            transport.Enqueue(ErrorReportTransportResult.PermanentFailure());
            var pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            ErrorReportRecord record = pipeline.Snapshot()[0];
            ErrorReportOperationResult later = pipeline.TryFlushOne(
                StartTicks + 1L, true);
            return
                result.ReasonCode ==
                    ErrorReportReasonCodes.PermanentlyRejected &&
                record.DeliveryState ==
                    ErrorReportDeliveryState.PermanentlyRejected &&
                record.DeliveryAttempts == 1 &&
                record.NextAttemptUtcTicks == long.MaxValue &&
                later.ReasonCode == ErrorReportReasonCodes.NothingDue &&
                transport.SendCalls == 1;
        }

        static bool ThrowingCapabilityIsContained()
        {
            var transport = new ProbeTransport
                { ThrowOnDeliveryEnabled = true };
            var pipeline = PipelineWithOne(transport);
            string before = pipeline.CanonicalSnapshot();
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            return
                result.ReasonCode == ErrorReportReasonCodes.TransportFailure &&
                transport.SendCalls == 0 &&
                pipeline.CanonicalSnapshot() == before;
        }

        static bool ThrowingSendIsContained()
        {
            var transport = new ProbeTransport { ThrowOnSend = true };
            var pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            ErrorReportRecord record = pipeline.Snapshot()[0];
            return
                result.ReasonCode == ErrorReportReasonCodes.TransportFailure &&
                record.DeliveryState ==
                    ErrorReportDeliveryState.RetryScheduled &&
                record.DeliveryAttempts == 1;
        }

        static bool NullTransportResultIsContained()
        {
            var transport = new ProbeTransport { ReturnNull = true };
            var pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult result = pipeline.TryFlushOne(
                StartTicks, true);
            ErrorReportRecord record = pipeline.Snapshot()[0];
            return
                result.ReasonCode == ErrorReportReasonCodes.TransportFailure &&
                record.DeliveryState ==
                    ErrorReportDeliveryState.RetryScheduled &&
                record.DeliveryAttempts == 1;
        }

        static bool ReentrantCaptureIsBlocked()
        {
            var transport = new ProbeTransport();
            ErrorReportingPipeline pipeline = null;
            ErrorReportOperationResult nested = null;
            transport.OnSend = _ =>
                nested = pipeline.TryCapture(DraftB(), StartTicks + 1L);
            transport.Enqueue(ErrorReportTransportResult.Delivered());
            pipeline = PipelineWithOne(transport);
            ErrorReportOperationResult outer = pipeline.TryFlushOne(
                StartTicks, true);
            return
                nested != null &&
                nested.ReasonCode ==
                    ErrorReportReasonCodes.ReentrantOperationBlocked &&
                outer.ReasonCode == ErrorReportReasonCodes.Delivered &&
                pipeline.BufferedCount == 0;
        }

        static bool DiagnosticsCoreIsDetached()
        {
            string root = Path.Combine(
                Application.dataPath, "LegendsOfRus", "Scripts",
                "DiagnosticsCore");
            if (!Directory.Exists(root))
                return false;
            string[] files = Directory.GetFiles(
                root, "*.cs", SearchOption.TopDirectoryOnly);
            if (files.Length != 9)
                return false;
            string[] forbidden =
            {
                @"\busing\s+UnityEngine\b",
                @"\busing\s+System\.Net(?:\.|\s*;)",
                @"\bUnityWebRequest\b", @"\bHttpClient\b",
                @"\bWebClient\b", @"\bPlayerPrefs\s*\.",
                @"\bDebug\s*\.", @"\bApplication\s*\.",
                @"\bFile\s*\.", @"\bDirectory\s*\.",
                @"\bDateTime\s*\.\s*(?:Now|UtcNow)\b",
                @"\bFirebase\b", @"\bCrashlytics\b", @"\bSentry\b",
                @"\bBugsnag\b", @"\bBacktrace\b", @"\bAppCenter\b",
                @"\bPlayFab\b"
            };
            for (int i = 0; i < files.Length; i++)
            {
                string source = File.ReadAllText(files[i]);
                for (int j = 0; j < forbidden.Length; j++)
                {
                    if (Regex.IsMatch(source, forbidden[j],
                        RegexOptions.CultureInvariant))
                        return false;
                }
            }
            return true;
        }

        static bool ProductionScriptsHaveNoLogSubscription()
        {
            string root = Path.Combine(
                Application.dataPath, "LegendsOfRus", "Scripts");
            if (!Directory.Exists(root))
                return false;
            var forbidden = new Regex(
                @"Application\s*\.\s*logMessageReceived(?:Threaded)?\b",
                RegexOptions.CultureInvariant);
            string[] files = Directory.GetFiles(
                root, "*.cs", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (forbidden.IsMatch(File.ReadAllText(files[i])))
                    return false;
            }
            return true;
        }

        static bool NetworkTransportCompiledRemainsFalse()
        {
            return IsPrivateFalseConstant(
                typeof(ErrorReportingPipeline).GetField(
                    "NetworkTransportCompiled",
                    BindingFlags.Static | BindingFlags.NonPublic));
        }

        static bool Step2EntryEvidenceRemainsUnretained()
        {
            string path = Path.Combine(
                Application.dataPath, "LegendsOfRus", "Documentation",
                "IMPLEMENTATION_STATUS_V1_17_STEP03.json");
            return File.Exists(path) && Regex.IsMatch(
                File.ReadAllText(path),
                "\"step2_entry_evidence_retained\"\\s*:\\s*false",
                RegexOptions.CultureInvariant);
        }

        static bool EconomyActivationRemainsDisabled()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                RuntimeEconomyActivationGate.ManifestResourcePath);
            if (asset == null)
                return false;
            RuntimeEconomyActivationManifestData manifest =
                JsonUtility.FromJson<RuntimeEconomyActivationManifestData>(
                    asset.text);
            RuntimeEconomyActivationResult decision =
                RuntimeEconomyActivationGate.Load();
            return
                manifest != null && !manifest.compilationVerified &&
                !manifest.editorSelfTestsPassed &&
                !manifest.editModeTestsPassed &&
                !manifest.playModeTestsPassed &&
                !manifest.runtimeActivationApproved &&
                decision != null && !decision.IsRuntimeActivated &&
                decision.UsesLegacyFallback;
        }

        static bool CommercialActivationRemainsDisabled()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeActivationGate.ResourcePath);
            if (asset == null)
                return false;
            CommercialActivationMirror value =
                JsonUtility.FromJson<CommercialActivationMirror>(asset.text);
            FieldInfo compiled = typeof(CommercialRuntimeActivationGate)
                .GetField("ExternalProviderActivationCompiled",
                    BindingFlags.Static | BindingFlags.NonPublic);
            return
                value != null && value.environment == "disabled" &&
                !value.v115RuntimeEconomyApproved &&
                !value.v116Step1CompilationVerified &&
                !value.v116Step1ProjectValidatorPassed &&
                !value.v116Step1EditorSelfTestsPassed &&
                !value.v116Step1EditModeTestsPassed &&
                !value.v116Step1PlayModeTestsPassed &&
                !value.providerSelectionApproved &&
                !value.legalPrivacyApproved &&
                !value.consentFlowApproved &&
                !value.productIdentifiersApproved &&
                !value.rewardedPlacementsApproved &&
                !value.regionalBehaviorApproved &&
                !value.securityReviewPassed &&
                !value.representativeDeviceFailurePathsPassed &&
                !value.saveMigrationApproved &&
                !value.durableFulfillmentApproved &&
                !value.analyticsActivationApproved &&
                !value.iapActivationApproved &&
                !value.rewardedAdsActivationApproved &&
                !value.finalRuntimeActivationApproved &&
                IsPrivateFalseConstant(compiled);
        }

        static bool AndroidProductionRemainsDisabled()
        {
            AndroidBuildPolicyLoadResult loaded =
                AndroidBuildPolicyLoader.Load();
            AndroidBuildFoundationPolicy value =
                loaded != null ? loaded.Policy : null;
            FieldInfo compiled = typeof(AndroidBuildGuard).GetField(
                "ProductionAndroidBuildCompiled",
                BindingFlags.Static | BindingFlags.NonPublic);
            return
                loaded != null && loaded.IsValid && value != null &&
                !value.allowReleaseApk && !value.allowAppBundle &&
                !value.allowGradleExport && !value.allowCustomSigning &&
                !value.productionApplicationIdentifierApproved &&
                !value.versioningApproved && !value.sdkPolicyApproved &&
                !value.signingIdentityApproved &&
                !value.playAppSigningApproved &&
                !value.nativePluginInventoryApproved &&
                !value.sixteenKbPageCompatibilityVerified &&
                !value.dataSafetyApproved &&
                !value.productionBuildApproved &&
                IsPrivateFalseConstant(compiled);
        }

        static bool ResolvedPackageLockApprovalRemainsFalse()
        {
            return IsPrivateFalseConstant(
                typeof(AndroidBuildStateReader).GetField(
                    "ResolvedPackageLockApproved",
                    BindingFlags.Static | BindingFlags.NonPublic));
        }

        static bool UnityVersionBoundaryIsExact()
        {
            AndroidBuildPolicyLoadResult loaded =
                AndroidBuildPolicyLoader.Load();
            return
                Application.unityVersion == "6000.5.9f1" &&
                RuntimeEconomyActivationGate.RequiredUnityVersion ==
                    "6000.5.9f1" &&
                CommercialRuntimeActivationGate.RequiredUnityVersion ==
                    "6000.5.9f1" &&
                loaded != null && loaded.Policy != null &&
                loaded.Policy.unityVersion == "6000.5.9f1";
        }

        static ErrorReportDraft DraftA()
        {
            return new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Gameplay,
                ErrorReportCode.Match3PlanFailure,
                ErrorReportSiteCode.Match3ResolvePlan);
        }

        static ErrorReportDraft DraftB()
        {
            return new ErrorReportDraft(
                ErrorReportKind.NonFatal,
                ErrorReportSubsystem.Save,
                ErrorReportCode.SaveWriteFailure,
                ErrorReportSiteCode.SaveWriteCommit);
        }

        static ErrorReportPolicy Policy(
            int capacity,
            long deduplicationWindowTicks,
            long retentionTicks,
            int maximumDeliveryAttempts,
            long initialRetryDelayTicks,
            long maximumRetryDelayTicks)
        {
            return new ErrorReportPolicy(
                capacity, deduplicationWindowTicks, retentionTicks,
                maximumDeliveryAttempts, initialRetryDelayTicks,
                maximumRetryDelayTicks);
        }

        static ErrorReportingPipeline PipelineWithOne(
            IErrorReportTransport transport)
        {
            var pipeline = new ErrorReportingPipeline(true, null, transport);
            pipeline.TryCapture(DraftA(), StartTicks);
            return pipeline;
        }

        static string[] PublicPropertyNames(Type type)
        {
            return type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.DeclaredOnly)
                .Select(property => property.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }

        static bool HasNoPublicSetter(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.DeclaredOnly);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetSetMethod(false) != null)
                    return false;
            }
            return true;
        }

        static bool IsPrivateFalseConstant(FieldInfo field)
        {
            return field != null && field.IsPrivate && field.IsLiteral &&
                Equals(field.GetRawConstantValue(), false);
        }

        static void Check(
            string name,
            Func<bool> assertion,
            ref int passed,
            ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (assertion())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + name);
            }
            catch (Exception exception)
            {
                failures.Add(
                    "ERROR: " + name + " (" +
                    exception.GetType().Name + ": " + exception.Message + ")");
            }
        }

        sealed class ProbeTransport : IErrorReportTransport
        {
            readonly Queue<ErrorReportTransportResult> results =
                new Queue<ErrorReportTransportResult>();
            public bool Network;
            public bool ThrowOnDeliveryEnabled;
            public bool ThrowOnSend;
            public bool ReturnNull;
            public Action<ErrorReportRecord> OnSend;
            public int CapabilityReads;
            public int SendCalls;

            public string TransportId { get { return "selftest_probe"; } }
            public bool DeliveryEnabled
            {
                get
                {
                    CapabilityReads++;
                    if (ThrowOnDeliveryEnabled)
                        throw new InvalidOperationException("capture");
                    return true;
                }
            }
            public bool SupportsNetwork
            {
                get
                {
                    CapabilityReads++;
                    return Network;
                }
            }

            public void Enqueue(ErrorReportTransportResult value)
            {
                results.Enqueue(value);
            }

            public ErrorReportTransportResult TrySend(ErrorReportRecord record)
            {
                SendCalls++;
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

        [Serializable]
        sealed class CommercialActivationMirror
        {
            public string environment = string.Empty;
            public bool v115RuntimeEconomyApproved;
            public bool v116Step1CompilationVerified;
            public bool v116Step1ProjectValidatorPassed;
            public bool v116Step1EditorSelfTestsPassed;
            public bool v116Step1EditModeTestsPassed;
            public bool v116Step1PlayModeTestsPassed;
            public bool providerSelectionApproved;
            public bool legalPrivacyApproved;
            public bool consentFlowApproved;
            public bool productIdentifiersApproved;
            public bool rewardedPlacementsApproved;
            public bool regionalBehaviorApproved;
            public bool securityReviewPassed;
            public bool representativeDeviceFailurePathsPassed;
            public bool saveMigrationApproved;
            public bool durableFulfillmentApproved;
            public bool analyticsActivationApproved;
            public bool iapActivationApproved;
            public bool rewardedAdsActivationApproved;
            public bool finalRuntimeActivationApproved;
        }
    }
}
#endif
