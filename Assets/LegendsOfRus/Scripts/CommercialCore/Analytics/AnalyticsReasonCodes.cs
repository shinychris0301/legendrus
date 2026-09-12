namespace LegendsOfRus.CommercialCore.Analytics
{
    /// <summary>
    /// Stable, non-localized analytics result codes. Callers may branch on a
    /// code, but must not attach exception text or other free-form payloads.
    /// </summary>
    public static class AnalyticsReasonCodes
    {
        public const string Accepted = "ANALYTICS_ACCEPTED";
        public const string Disabled = "ANALYTICS_DISABLED";
        public const string CaptureNotAllowed = "ANALYTICS_CAPTURE_NOT_ALLOWED";
        public const string SessionNotStarted = "ANALYTICS_SESSION_NOT_STARTED";
        public const string SessionAlreadyStarted = "ANALYTICS_SESSION_ALREADY_STARTED";
        public const string SessionIdInvalid = "ANALYTICS_SESSION_ID_INVALID";
        public const string TimestampInvalid = "ANALYTICS_TIMESTAMP_INVALID";
        public const string TimestampRollback = "ANALYTICS_TIMESTAMP_ROLLBACK";
        public const string DraftNull = "ANALYTICS_DRAFT_NULL";
        public const string EnvelopeNull = "ANALYTICS_ENVELOPE_NULL";
        public const string SchemaVersionUnsupported =
            "ANALYTICS_SCHEMA_VERSION_UNSUPPORTED";
        public const string EventIdInvalid = "ANALYTICS_EVENT_ID_INVALID";
        public const string EventUnknown = "ANALYTICS_EVENT_UNKNOWN";
        public const string EventReserved = "ANALYTICS_EVENT_RESERVED";
        public const string AttributeCountInvalid = "ANALYTICS_ATTRIBUTE_COUNT_INVALID";
        public const string AttributeNull = "ANALYTICS_ATTRIBUTE_NULL";
        public const string AttributeKeyInvalid = "ANALYTICS_ATTRIBUTE_KEY_INVALID";
        public const string AttributeDuplicate = "ANALYTICS_ATTRIBUTE_DUPLICATE";
        public const string AttributeUnexpected = "ANALYTICS_ATTRIBUTE_UNEXPECTED";
        public const string AttributeMissing = "ANALYTICS_ATTRIBUTE_MISSING";
        public const string AttributeTypeMismatch =
            "ANALYTICS_ATTRIBUTE_TYPE_MISMATCH";
        public const string StringValueInvalid = "ANALYTICS_STRING_VALUE_INVALID";
        public const string StringValueNotAllowed =
            "ANALYTICS_STRING_VALUE_NOT_ALLOWED";
        public const string IntegerValueOutOfRange =
            "ANALYTICS_INTEGER_VALUE_OUT_OF_RANGE";
        public const string SequenceInvalid = "ANALYTICS_SEQUENCE_INVALID";
        public const string SessionMismatch = "ANALYTICS_SESSION_MISMATCH";
        public const string AdapterConfigurationInvalid =
            "ANALYTICS_ADAPTER_CONFIGURATION_INVALID";
        public const string CapacityReached = "ANALYTICS_CAPACITY_REACHED";
        public const string AdapterFailure = "ANALYTICS_ADAPTER_FAILURE";
        public const string UploadUnavailable = "ANALYTICS_UPLOAD_UNAVAILABLE";
        public const string FunnelAttemptKeyInvalid =
            "ANALYTICS_FUNNEL_ATTEMPT_KEY_INVALID";
        public const string FunnelAttemptAlreadyActive =
            "ANALYTICS_FUNNEL_ATTEMPT_ALREADY_ACTIVE";
        public const string FunnelAttemptDuplicate =
            "ANALYTICS_FUNNEL_ATTEMPT_DUPLICATE";
        public const string FunnelTerminalOutOfOrder =
            "ANALYTICS_FUNNEL_TERMINAL_OUT_OF_ORDER";
        public const string FunnelAttemptMismatch =
            "ANALYTICS_FUNNEL_ATTEMPT_MISMATCH";
        public const string FunnelTerminalDuplicate =
            "ANALYTICS_FUNNEL_TERMINAL_DUPLICATE";
        public const string FunnelOutcomeInvalid =
            "ANALYTICS_FUNNEL_OUTCOME_INVALID";
        public const string FunnelConfigurationInvalid =
            "ANALYTICS_FUNNEL_CONFIGURATION_INVALID";
        public const string FunnelCapacityReached =
            "ANALYTICS_FUNNEL_CAPACITY_REACHED";
    }
}
