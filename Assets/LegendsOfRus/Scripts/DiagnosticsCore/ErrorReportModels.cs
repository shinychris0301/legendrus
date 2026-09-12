namespace LegendsOfRus.DiagnosticsCore
{
    public enum ErrorReportKind
    {
        NonFatal = 0,
        ManagedCrash = 1
    }

    public enum ErrorReportSubsystem
    {
        Bootstrap = 0,
        Runtime = 1,
        Gameplay = 2,
        Save = 3,
        Economy = 4,
        Commercial = 5,
        Ui = 6,
        Audio = 7,
        Content = 8
    }

    /// <summary>
    /// Reviewed incident catalog. Callers cannot supply arbitrary strings,
    /// identifiers, provider text, or user-entered values as an error code.
    /// </summary>
    public enum ErrorReportCode
    {
        BootstrapFailure = 0,
        RuntimeInvariantFailure = 1,
        ManagedCrashUnhandled = 2,
        Match3PlanFailure = 3,
        SaveReadFailure = 4,
        SaveWriteFailure = 5,
        SaveMigrationFailure = 6,
        EconomyTransactionFailure = 7,
        CommercialAdapterFailure = 8,
        UiPresentationFailure = 9,
        AudioPlaybackFailure = 10,
        ContentLoadFailure = 11
    }

    /// <summary>
    /// Reviewed capture-site catalog paired with <see cref="ErrorReportCode"/>.
    /// Stable wire strings are produced internally by the canonical serializer.
    /// </summary>
    public enum ErrorReportSiteCode
    {
        BootstrapInitialize = 0,
        RuntimeInvariantCheck = 1,
        RuntimeUnhandled = 2,
        Match3ResolvePlan = 3,
        SaveReadDecode = 4,
        SaveWriteCommit = 5,
        SaveMigrationApply = 6,
        EconomyTransactionCommit = 7,
        CommercialAdapterInvoke = 8,
        UiPresentationUpdate = 9,
        AudioPlaybackInvoke = 10,
        ContentResourceLoad = 11
    }

    public enum ErrorReportDeliveryState
    {
        Pending = 0,
        RetryScheduled = 1,
        RetryExhausted = 2,
        PermanentlyRejected = 3
    }

    public static class ErrorReportSchema
    {
        public const int CurrentSchemaVersion = 1;
        public const int MaximumErrorCodeLength = 64;
        public const int MaximumSiteCodeLength = 64;
    }

    /// <summary>
    /// Already-minimized caller intent. Free-form messages, stack traces,
    /// identifiers, save data, purchases, credentials, and arbitrary context
    /// deliberately have no representation in this contract.
    /// </summary>
    public sealed class ErrorReportDraft
    {
        public ErrorReportKind Kind { get; private set; }
        public ErrorReportSubsystem Subsystem { get; private set; }
        public ErrorReportCode ErrorCode { get; private set; }
        public ErrorReportSiteCode SiteCode { get; private set; }

        public ErrorReportDraft(
            ErrorReportKind kind,
            ErrorReportSubsystem subsystem,
            ErrorReportCode errorCode,
            ErrorReportSiteCode siteCode
        )
        {
            Kind = kind;
            Subsystem = subsystem;
            ErrorCode = errorCode;
            SiteCode = siteCode;
        }

        internal ErrorReportDraft DetachedCopy()
        {
            return new ErrorReportDraft(Kind, Subsystem, ErrorCode, SiteCode);
        }
    }

    /// <summary>
    /// Immutable process-local record. Transport state is explicit so retry
    /// decisions and diagnostic snapshots remain deterministic.
    /// </summary>
    public sealed class ErrorReportRecord
    {
        public int SchemaVersion { get; private set; }
        public long Sequence { get; private set; }
        public string Fingerprint { get; private set; }
        public ErrorReportKind Kind { get; private set; }
        public ErrorReportSubsystem Subsystem { get; private set; }
        public ErrorReportCode ErrorCode { get; private set; }
        public ErrorReportSiteCode SiteCode { get; private set; }
        public long FirstUtcTicks { get; private set; }
        public long LastUtcTicks { get; private set; }
        public long OccurrenceCount { get; private set; }
        public ErrorReportDeliveryState DeliveryState { get; private set; }
        public int DeliveryAttempts { get; private set; }
        public long NextAttemptUtcTicks { get; private set; }

        internal ErrorReportRecord(
            int schemaVersion,
            long sequence,
            string fingerprint,
            ErrorReportKind kind,
            ErrorReportSubsystem subsystem,
            ErrorReportCode errorCode,
            ErrorReportSiteCode siteCode,
            long firstUtcTicks,
            long lastUtcTicks,
            long occurrenceCount,
            ErrorReportDeliveryState deliveryState,
            int deliveryAttempts,
            long nextAttemptUtcTicks
        )
        {
            SchemaVersion = schemaVersion;
            Sequence = sequence;
            Fingerprint = fingerprint;
            Kind = kind;
            Subsystem = subsystem;
            ErrorCode = errorCode;
            SiteCode = siteCode;
            FirstUtcTicks = firstUtcTicks;
            LastUtcTicks = lastUtcTicks;
            OccurrenceCount = occurrenceCount;
            DeliveryState = deliveryState;
            DeliveryAttempts = deliveryAttempts;
            NextAttemptUtcTicks = nextAttemptUtcTicks;
        }

        public ErrorReportRecord DetachedCopy()
        {
            return new ErrorReportRecord(
                SchemaVersion,
                Sequence,
                Fingerprint,
                Kind,
                Subsystem,
                ErrorCode,
                SiteCode,
                FirstUtcTicks,
                LastUtcTicks,
                OccurrenceCount,
                DeliveryState,
                DeliveryAttempts,
                NextAttemptUtcTicks
            );
        }

        internal ErrorReportDraft ToDraft()
        {
            return new ErrorReportDraft(Kind, Subsystem, ErrorCode, SiteCode);
        }
    }

    public sealed class ErrorReportOperationResult
    {
        public bool Accepted { get; private set; }
        public bool Succeeded { get { return Accepted; } }
        public string ReasonCode { get; private set; }
        public long Sequence { get; private set; }
        public string Fingerprint { get; private set; }

        ErrorReportOperationResult(
            bool accepted,
            string reasonCode,
            long sequence,
            string fingerprint
        )
        {
            Accepted = accepted;
            ReasonCode = reasonCode ?? ErrorReportReasonCodes.UnexpectedFailure;
            Sequence = sequence;
            Fingerprint = fingerprint ?? string.Empty;
        }

        public static ErrorReportOperationResult Success(
            string reasonCode,
            long sequence = 0L,
            string fingerprint = null
        )
        {
            return new ErrorReportOperationResult(
                true,
                reasonCode,
                sequence,
                fingerprint
            );
        }

        public static ErrorReportOperationResult Reject(
            string reasonCode,
            long sequence = 0L,
            string fingerprint = null
        )
        {
            return new ErrorReportOperationResult(
                false,
                reasonCode,
                sequence,
                fingerprint
            );
        }
    }
}
