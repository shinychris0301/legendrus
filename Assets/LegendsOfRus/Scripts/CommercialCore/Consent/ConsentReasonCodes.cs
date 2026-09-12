namespace LegendsOfRus.CommercialCore.Consent
{
    /// <summary>
    /// Stable machine-readable reasons. UI and provider-specific text must be
    /// mapped outside the commercial core rather than added to these values.
    /// </summary>
    public static class ConsentReasonCodes
    {
        public const string Valid = "CONSENT_VALID";
        public const string Eligible = "CONSENT_ELIGIBLE";

        public const string SnapshotMissing = "CONSENT_SNAPSHOT_MISSING";
        public const string SchemaUnsupported = "CONSENT_SCHEMA_UNSUPPORTED";
        public const string PolicyUnsupported = "CONSENT_POLICY_UNSUPPORTED";
        public const string AudienceInvalid = "CONSENT_AUDIENCE_INVALID";
        public const string AnalyticsDecisionInvalid =
            "CONSENT_ANALYTICS_DECISION_INVALID";
        public const string RewardedAdvertisingDecisionInvalid =
            "CONSENT_REWARDED_AD_DECISION_INVALID";
        public const string RestrictedAudienceGrant =
            "CONSENT_RESTRICTED_AUDIENCE_GRANT";

        public const string ProviderMissing = "CONSENT_PROVIDER_MISSING";
        public const string ProviderDisabled = "CONSENT_PROVIDER_DISABLED";
        public const string ProviderFailed = "CONSENT_PROVIDER_FAILED";
        public const string ProviderSnapshotInvalid =
            "CONSENT_PROVIDER_SNAPSHOT_INVALID";
        public const string DevelopmentProviderReady =
            "CONSENT_DEVELOPMENT_PROVIDER_READY";
        public const string DevelopmentProviderReset =
            "CONSENT_DEVELOPMENT_PROVIDER_RESET";

        public const string ExternalActivationDisabled =
            "CONSENT_EXTERNAL_ACTIVATION_DISABLED";
        public const string AudienceRestricted = "CONSENT_AUDIENCE_RESTRICTED";
        public const string AnalyticsDecisionRequired =
            "CONSENT_ANALYTICS_DECISION_REQUIRED";
        public const string AnalyticsDenied = "CONSENT_ANALYTICS_DENIED";
        public const string RewardedAdvertisingDecisionRequired =
            "CONSENT_REWARDED_AD_DECISION_REQUIRED";
        public const string RewardedAdvertisingDenied =
            "CONSENT_REWARDED_AD_DENIED";
    }
}
