namespace LegendsOfRus.CommercialCore.Monetization
{
    /// <summary>
    /// Stable machine-readable outcomes. Presentation and analytics may map these
    /// values, but receipts, proof payloads, and provider tokens must never be
    /// placed in user-facing text or telemetry.
    /// </summary>
    public static class MonetizationReasonCodes
    {
        public const string None = "";
        public const string InvalidRequest = "MONETIZATION_INVALID_REQUEST";
        public const string UnknownOffer = "MONETIZATION_UNKNOWN_OFFER";
        public const string UnknownProduct = "MONETIZATION_UNKNOWN_PRODUCT";
        public const string UnknownPlacement = "MONETIZATION_UNKNOWN_PLACEMENT";
        public const string ProductMismatch = "MONETIZATION_PRODUCT_MISMATCH";
        public const string PlacementMismatch = "MONETIZATION_PLACEMENT_MISMATCH";
        public const string OpportunityMismatch = "MONETIZATION_OPPORTUNITY_MISMATCH";
        public const string ProviderMismatch = "MONETIZATION_PROVIDER_MISMATCH";
        public const string EnvironmentMismatch = "MONETIZATION_ENVIRONMENT_MISMATCH";
        public const string AdapterDisabled = "MONETIZATION_ADAPTER_DISABLED";
        public const string AdapterUnavailable = "MONETIZATION_ADAPTER_UNAVAILABLE";
        public const string Offline = "MONETIZATION_OFFLINE";
        public const string ConsentRequired = "MONETIZATION_CONSENT_REQUIRED";
        public const string NotInitialized = "MONETIZATION_NOT_INITIALIZED";
        public const string Unsupported = "MONETIZATION_UNSUPPORTED";
        public const string VerifierUnavailable = "MONETIZATION_VERIFIER_UNAVAILABLE";
        public const string DevelopmentScriptMissing = "MONETIZATION_DEVELOPMENT_SCRIPT_MISSING";
        public const string Busy = "MONETIZATION_BUSY";
        public const string DuplicateRequest = "MONETIZATION_DUPLICATE_REQUEST";
        public const string DuplicateEntitlement = "MONETIZATION_DUPLICATE_ENTITLEMENT";
        public const string NoPendingOperation = "MONETIZATION_NO_PENDING_OPERATION";
        public const string PurchasePending = "MONETIZATION_PURCHASE_PENDING";
        public const string PurchaseCancelled = "MONETIZATION_PURCHASE_CANCELLED";
        public const string PurchaseDeclined = "MONETIZATION_PURCHASE_DECLINED";
        public const string PurchaseFailed = "MONETIZATION_PURCHASE_FAILED";
        public const string PurchaseNotCompleted = "MONETIZATION_PURCHASE_NOT_COMPLETED";
        public const string AdPending = "MONETIZATION_AD_PENDING";
        public const string AdClosedEarly = "MONETIZATION_AD_CLOSED_EARLY";
        public const string AdSkipped = "MONETIZATION_AD_SKIPPED";
        public const string AdCancelled = "MONETIZATION_AD_CANCELLED";
        public const string AdFailed = "MONETIZATION_AD_FAILED";
        public const string AdNotCompleted = "MONETIZATION_AD_NOT_COMPLETED";
        public const string EvidenceMissing = "MONETIZATION_EVIDENCE_MISSING";
        public const string EvidenceMalformed = "MONETIZATION_EVIDENCE_MALFORMED";
        public const string EvidenceUnverified = "MONETIZATION_EVIDENCE_UNVERIFIED";
        public const string InconsistentResult = "MONETIZATION_INCONSISTENT_RESULT";
        public const string VerificationRejected = "MONETIZATION_VERIFICATION_REJECTED";
        public const string VerificationRetryable = "MONETIZATION_VERIFICATION_RETRYABLE";
        public const string ReservationInvalid = "MONETIZATION_RESERVATION_INVALID";
        public const string DurableHistoryInvalid = "MONETIZATION_DURABLE_HISTORY_INVALID";

        public static bool IsKnown(string reasonCode)
        {
            switch (reasonCode)
            {
                case None:
                case InvalidRequest:
                case UnknownOffer:
                case UnknownProduct:
                case UnknownPlacement:
                case ProductMismatch:
                case PlacementMismatch:
                case OpportunityMismatch:
                case ProviderMismatch:
                case EnvironmentMismatch:
                case AdapterDisabled:
                case AdapterUnavailable:
                case Offline:
                case ConsentRequired:
                case NotInitialized:
                case Unsupported:
                case VerifierUnavailable:
                case DevelopmentScriptMissing:
                case Busy:
                case DuplicateRequest:
                case DuplicateEntitlement:
                case NoPendingOperation:
                case PurchasePending:
                case PurchaseCancelled:
                case PurchaseDeclined:
                case PurchaseFailed:
                case PurchaseNotCompleted:
                case AdPending:
                case AdClosedEarly:
                case AdSkipped:
                case AdCancelled:
                case AdFailed:
                case AdNotCompleted:
                case EvidenceMissing:
                case EvidenceMalformed:
                case EvidenceUnverified:
                case InconsistentResult:
                case VerificationRejected:
                case VerificationRetryable:
                case ReservationInvalid:
                case DurableHistoryInvalid:
                    return true;
                default:
                    return false;
            }
        }

        internal static string NormalizeOr(string reasonCode, string fallback)
        {
            return IsKnown(reasonCode)
                ? reasonCode
                : IsKnown(fallback)
                    ? fallback
                    : InvalidRequest;
        }
    }

    internal static class MonetizationIdentifiers
    {
        const int MaxIdentifierLength = 160;

        public static bool IsStable(string value)
        {
            if (
                string.IsNullOrWhiteSpace(value) ||
                value.Length > MaxIdentifierLength ||
                !string.Equals(value, value.Trim(), System.StringComparison.Ordinal)
            )
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                bool valid =
                    character >= 'a' && character <= 'z' ||
                    character >= 'A' && character <= 'Z' ||
                    character >= '0' && character <= '9' ||
                    character == '.' ||
                    character == '_' ||
                    character == '-';
                if (!valid)
                    return false;
            }

            return true;
        }
    }
}
