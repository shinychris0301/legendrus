namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Stable, non-localized operation codes. Provider text and exception
    /// messages are never copied into these values.
    /// </summary>
    public static class ErrorReportReasonCodes
    {
        public const string Captured = "ERROR_REPORT_CAPTURED";
        public const string Deduplicated = "ERROR_REPORT_DEDUPLICATED";
        public const string Delivered = "ERROR_REPORT_DELIVERED";
        public const string CaptureNotAllowed = "ERROR_REPORT_CAPTURE_NOT_ALLOWED";
        public const string DraftNull = "ERROR_REPORT_DRAFT_NULL";
        public const string KindInvalid = "ERROR_REPORT_KIND_INVALID";
        public const string SubsystemInvalid = "ERROR_REPORT_SUBSYSTEM_INVALID";
        public const string ErrorCodeInvalid = "ERROR_REPORT_ERROR_CODE_INVALID";
        public const string SiteCodeInvalid = "ERROR_REPORT_SITE_CODE_INVALID";
        public const string CodeTupleInvalid = "ERROR_REPORT_CODE_TUPLE_INVALID";
        public const string RecordNull = "ERROR_REPORT_RECORD_NULL";
        public const string RecordInvalid = "ERROR_REPORT_RECORD_INVALID";
        public const string SchemaVersionUnsupported =
            "ERROR_REPORT_SCHEMA_VERSION_UNSUPPORTED";
        public const string FingerprintInvalid = "ERROR_REPORT_FINGERPRINT_INVALID";
        public const string FingerprintFailure = "ERROR_REPORT_FINGERPRINT_FAILURE";
        public const string TimestampInvalid = "ERROR_REPORT_TIMESTAMP_INVALID";
        public const string TimestampRollback = "ERROR_REPORT_TIMESTAMP_ROLLBACK";
        public const string SequenceExhausted = "ERROR_REPORT_SEQUENCE_EXHAUSTED";
        public const string PolicyInvalid = "ERROR_REPORT_POLICY_INVALID";
        public const string CapacityReached = "ERROR_REPORT_CAPACITY_REACHED";
        public const string Offline = "ERROR_REPORT_OFFLINE";
        public const string TransportDisabled = "ERROR_REPORT_TRANSPORT_DISABLED";
        public const string NetworkTransportBlocked =
            "ERROR_REPORT_NETWORK_TRANSPORT_BLOCKED";
        public const string NothingDue = "ERROR_REPORT_NOTHING_DUE";
        public const string RetryScheduled = "ERROR_REPORT_RETRY_SCHEDULED";
        public const string RetryExhausted = "ERROR_REPORT_RETRY_EXHAUSTED";
        public const string PermanentlyRejected =
            "ERROR_REPORT_PERMANENTLY_REJECTED";
        public const string TransportFailure = "ERROR_REPORT_TRANSPORT_FAILURE";
        public const string ReentrantOperationBlocked =
            "ERROR_REPORT_REENTRANT_OPERATION_BLOCKED";
        public const string BufferFailure = "ERROR_REPORT_BUFFER_FAILURE";
        public const string UnexpectedFailure = "ERROR_REPORT_UNEXPECTED_FAILURE";
    }
}
