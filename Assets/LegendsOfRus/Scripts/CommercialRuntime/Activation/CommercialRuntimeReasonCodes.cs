namespace LegendsOfRus.CommercialRuntime
{
    public static class CommercialRuntimeReasonCodes
    {
        public const string None = "";
        public const string NotInitialized = "COMMERCIAL_RUNTIME_NOT_INITIALIZED";
        public const string SafeDisabled = "COMMERCIAL_RUNTIME_SAFE_DISABLED";
        public const string UnexpectedFailure = "COMMERCIAL_RUNTIME_UNEXPECTED_FAILURE";

        public const string ConfigResourceMissing = "COMMERCIAL_CONFIG_RESOURCE_MISSING";
        public const string ConfigSourceInvalid = "COMMERCIAL_CONFIG_SOURCE_INVALID";
        public const string ConfigSourceHashMismatch = "COMMERCIAL_CONFIG_SOURCE_HASH_MISMATCH";
        public const string ConfigJsonInvalid = "COMMERCIAL_CONFIG_JSON_INVALID";
        public const string ConfigIdentityMismatch = "COMMERCIAL_CONFIG_IDENTITY_MISMATCH";
        public const string ConfigEnablementForbidden = "COMMERCIAL_CONFIG_ENABLEMENT_FORBIDDEN";

        public const string RemoteConfigProviderMissing = "COMMERCIAL_RC_PROVIDER_MISSING";
        public const string RemoteConfigProviderFailed = "COMMERCIAL_RC_PROVIDER_FAILED";
        public const string RemoteConfigRejected = "COMMERCIAL_RC_REJECTED";
        public const string RemoteConfigBindingMismatch = "COMMERCIAL_RC_BINDING_MISMATCH";
        public const string RemoteConfigLoaded = "COMMERCIAL_RC_LKG_LOADED";
        public const string RemoteConfigFallback = "COMMERCIAL_RC_CONSERVATIVE_FALLBACK";

        public const string ManifestResourceMissing = "COMMERCIAL_MANIFEST_RESOURCE_MISSING";
        public const string ManifestSourceInvalid = "COMMERCIAL_MANIFEST_SOURCE_INVALID";
        public const string ManifestSourceHashMismatch = "COMMERCIAL_MANIFEST_SOURCE_HASH_MISMATCH";
        public const string ManifestJsonInvalid = "COMMERCIAL_MANIFEST_JSON_INVALID";
        public const string ManifestIdentityMismatch = "COMMERCIAL_MANIFEST_IDENTITY_MISMATCH";
        public const string UnityVersionMismatch = "COMMERCIAL_UNITY_VERSION_MISMATCH";
        public const string EconomyBindingMismatch = "COMMERCIAL_ECONOMY_BINDING_MISMATCH";
        public const string RuntimeConfigBindingMismatch = "COMMERCIAL_RUNTIME_CONFIG_BINDING_MISMATCH";
        public const string StepOneVerificationIncomplete = "COMMERCIAL_STEP1_VERIFICATION_INCOMPLETE";
        public const string V115EconomyInactive = "COMMERCIAL_V115_ECONOMY_INACTIVE";
        public const string ReviewApprovalsIncomplete = "COMMERCIAL_REVIEW_APPROVALS_INCOMPLETE";
        public const string DurableFulfillmentUnavailable = "COMMERCIAL_DURABLE_FULFILLMENT_UNAVAILABLE";
        public const string CapabilityApprovalMissing = "COMMERCIAL_CAPABILITY_APPROVAL_MISSING";
        public const string FinalApprovalMissing = "COMMERCIAL_FINAL_APPROVAL_MISSING";
        public const string BuildActivationDisabled = "COMMERCIAL_BUILD_ACTIVATION_DISABLED";
        public const string ProviderBindingUnavailable = "COMMERCIAL_PROVIDER_BINDING_UNAVAILABLE";
        public const string Activated = "COMMERCIAL_RUNTIME_ACTIVATED";
    }
}
