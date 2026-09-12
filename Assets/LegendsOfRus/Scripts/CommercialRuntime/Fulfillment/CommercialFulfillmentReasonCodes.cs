namespace LegendsOfRus.CommercialRuntime.Fulfillment
{
    /// <summary>
    /// Stable, non-sensitive outcomes for the detached commercial fulfillment
    /// boundary. Provider receipts, proof payloads, transaction identifiers, and
    /// reward tokens must never be appended to these values.
    /// </summary>
    public static class CommercialFulfillmentReasonCodes
    {
        public const string None = "";
        public const string RuntimeInactive =
            "COMMERCIAL_FULFILLMENT_RUNTIME_INACTIVE";
        public const string EconomyInactive =
            "COMMERCIAL_FULFILLMENT_ECONOMY_INACTIVE";
        public const string CatalogBindingMismatch =
            "COMMERCIAL_FULFILLMENT_CATALOG_BINDING_MISMATCH";
        public const string EntitlementGateUnavailable =
            "COMMERCIAL_FULFILLMENT_GATE_UNAVAILABLE";
        public const string SourceInvalid =
            "COMMERCIAL_FULFILLMENT_SOURCE_INVALID";
        public const string UnknownIdentity =
            "COMMERCIAL_FULFILLMENT_UNKNOWN_IDENTITY";
        public const string RecipeUnavailable =
            "COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE";
        public const string CandidateRejected =
            "COMMERCIAL_FULFILLMENT_CANDIDATE_REJECTED";
        public const string ReservationReleaseFailed =
            "COMMERCIAL_FULFILLMENT_RESERVATION_RELEASE_FAILED";
        public const string AtomicStoreUnavailable =
            "COMMERCIAL_FULFILLMENT_ATOMIC_STORE_UNAVAILABLE";
        public const string AtomicCommitNotApproved =
            "COMMERCIAL_FULFILLMENT_ATOMIC_COMMIT_NOT_APPROVED";
        public const string SourceChanged =
            "COMMERCIAL_FULFILLMENT_SOURCE_CHANGED";
        public const string DuplicateFulfillment =
            "COMMERCIAL_FULFILLMENT_DUPLICATE_FULFILLMENT";
        public const string InternalError =
            "COMMERCIAL_FULFILLMENT_INTERNAL_ERROR";
    }
}
