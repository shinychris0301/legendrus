using System;
using System.Collections.Generic;

namespace LegendsOfRus.CommercialCore.Monetization
{
    public enum EntitlementSource
    {
        InAppPurchase = 0,
        RewardedAd = 1
    }

    /// <summary>
    /// A detached verified-evidence reservation. It carries identity evidence
    /// only and never applies an economy mutation.
    /// </summary>
    public sealed class EntitlementReservation
    {
        internal long Sequence { get; private set; }

        public string FulfillmentKey { get; private set; }
        public string RequestId { get; private set; }
        public EntitlementSource Source { get; private set; }
        public MonetizationEnvironment Environment { get; private set; }
        public string ProviderId { get; private set; }
        public string ProductId { get; private set; }
        public string TransactionId { get; private set; }
        public string PlacementId { get; private set; }
        public string OpportunityId { get; private set; }
        public string RewardToken { get; private set; }

        internal EntitlementReservation(
            long sequence,
            string fulfillmentKey,
            string requestId,
            EntitlementSource source,
            MonetizationEnvironment environment,
            string providerId,
            string productId,
            string transactionId,
            string placementId,
            string opportunityId,
            string rewardToken
        )
        {
            Sequence = sequence;
            FulfillmentKey = fulfillmentKey ?? string.Empty;
            RequestId = requestId ?? string.Empty;
            Source = source;
            Environment = environment;
            ProviderId = providerId ?? string.Empty;
            ProductId = productId ?? string.Empty;
            TransactionId = transactionId ?? string.Empty;
            PlacementId = placementId ?? string.Empty;
            OpportunityId = opportunityId ?? string.Empty;
            RewardToken = rewardToken ?? string.Empty;
        }
    }

    public sealed class EntitlementEligibilityResult
    {
        public bool IsEligible { get; private set; }
        public string ReasonCode { get; private set; }
        public EntitlementReservation Reservation { get; private set; }

        internal EntitlementEligibilityResult(
            bool isEligible,
            string reasonCode,
            EntitlementReservation reservation
        )
        {
            IsEligible = isEligible;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.ReservationInvalid
            );
            Reservation = reservation;
        }
    }

    /// <summary>
    /// Uses immutable constructor-bound verifiers to validate evidence against
    /// exact offer/placement identity allowlists and reserve a one-time
    /// fulfillment key. Completion records are process-local; a future approved
    /// integration must persist exported keys atomically with its separately
    /// approved fulfillment and seed them on construction.
    /// </summary>
    public sealed class VerifiedEntitlementGate
    {
        const int MaxReceiptPayloadLength = 65536;
        const int MaxAdProofPayloadLength = 8192;

        readonly object syncRoot = new object();
        readonly MonetizationEnvironment expectedEnvironment;
        readonly IPurchaseVerifier purchaseVerifier;
        readonly IRewardedAdVerifier rewardedAdVerifier;
        readonly bool environmentValid;
        readonly bool verifierBindingsValid;
        readonly bool durableHistoryValid;
        readonly Dictionary<string, EntitlementReservation> reservations =
            new Dictionary<string, EntitlementReservation>(StringComparer.Ordinal);
        readonly Dictionary<string, EntitlementReservation> requestReservations =
            new Dictionary<string, EntitlementReservation>(StringComparer.Ordinal);
        readonly HashSet<string> completedFulfillmentKeys =
            new HashSet<string>(StringComparer.Ordinal);
        readonly HashSet<string> completedRequestKeys =
            new HashSet<string>(StringComparer.Ordinal);
        long nextSequence;

        public MonetizationEnvironment ExpectedEnvironment
        {
            get { return expectedEnvironment; }
        }

        public int ReservedCount
        {
            get
            {
                lock (syncRoot)
                    return reservations.Count;
            }
        }

        public int CompletedCount
        {
            get
            {
                lock (syncRoot)
                    return completedFulfillmentKeys.Count;
            }
        }

        public bool IsOperational
        {
            get
            {
                return
                    environmentValid &&
                    verifierBindingsValid &&
                    durableHistoryValid;
            }
        }

        public VerifiedEntitlementGate(
            MonetizationEnvironment environment,
            IPurchaseVerifier purchaseVerifier,
            IRewardedAdVerifier rewardedAdVerifier,
            IEnumerable<string> durableCompletedFulfillmentKeys = null
        )
        {
            expectedEnvironment = environment;
            this.purchaseVerifier = purchaseVerifier;
            this.rewardedAdVerifier = rewardedAdVerifier;
            environmentValid = IsSupportedEnvironment(environment);
            verifierBindingsValid =
                purchaseVerifier != null && rewardedAdVerifier != null;
            bool validHistory = true;
            if (durableCompletedFulfillmentKeys == null)
            {
                durableHistoryValid = true;
                return;
            }

            foreach (string key in durableCompletedFulfillmentKeys)
            {
                if (
                    !IsStableFulfillmentKey(key) ||
                    !completedFulfillmentKeys.Add(key)
                )
                {
                    validHistory = false;
                }
            }
            durableHistoryValid = validHistory;
        }

        public EntitlementEligibilityResult TryReservePurchase(
            PurchaseRequest request,
            PurchaseResult purchase
        )
        {
            string readiness = InitializationFailureReason();
            if (!string.IsNullOrEmpty(readiness))
                return Rejected(readiness);

            string validation = ValidatePurchaseBeforeVerification(
                request,
                purchase,
                out _
            );
            if (!string.IsNullOrEmpty(validation))
                return Rejected(validation);
            PurchaseVerificationResult verification;
            try
            {
                verification = purchaseVerifier.Verify(purchase.Evidence);
            }
            catch
            {
                return Rejected(MonetizationReasonCodes.VerifierUnavailable);
            }
            return TryReservePurchaseVerified(
                request,
                purchase,
                verification
            );
        }

        public EntitlementEligibilityResult TryReserveRewardedAd(
            RewardedAdRequest request,
            RewardedAdResult result
        )
        {
            string readiness = InitializationFailureReason();
            if (!string.IsNullOrEmpty(readiness))
                return Rejected(readiness);

            string validation = ValidateRewardedAdBeforeVerification(
                request,
                result,
                out _
            );
            if (!string.IsNullOrEmpty(validation))
                return Rejected(validation);
            RewardedAdVerificationResult verification;
            try
            {
                verification = rewardedAdVerifier.Verify(result.Evidence);
            }
            catch
            {
                return Rejected(MonetizationReasonCodes.VerifierUnavailable);
            }

            return TryReserveRewardedAdVerified(
                request,
                result,
                verification
            );
        }

        EntitlementEligibilityResult TryReservePurchaseVerified(
            PurchaseRequest request,
            PurchaseResult purchase,
            PurchaseVerificationResult verification
        )
        {
            string validation = ValidatePurchase(
                request,
                purchase,
                verification,
                out PurchaseOfferDefinition offer,
                out string fulfillmentKey
            );
            if (!string.IsNullOrEmpty(validation))
                return Rejected(validation);

            return Reserve(
                fulfillmentKey,
                RequestKey(EntitlementSource.InAppPurchase, request.RequestId),
                request.RequestId,
                EntitlementSource.InAppPurchase,
                purchase.Evidence.Environment,
                purchase.Evidence.ProviderId,
                offer.ProductId,
                purchase.Evidence.TransactionId,
                string.Empty,
                string.Empty,
                string.Empty
            );
        }

        EntitlementEligibilityResult TryReserveRewardedAdVerified(
            RewardedAdRequest request,
            RewardedAdResult result,
            RewardedAdVerificationResult verification
        )
        {
            string validation = ValidateRewardedAd(
                request,
                result,
                verification,
                out RewardedPlacementDefinition placement,
                out string fulfillmentKey
            );
            if (!string.IsNullOrEmpty(validation))
                return Rejected(validation);

            return Reserve(
                fulfillmentKey,
                RequestKey(EntitlementSource.RewardedAd, request.RequestId),
                request.RequestId,
                EntitlementSource.RewardedAd,
                result.Evidence.Environment,
                result.Evidence.ProviderId,
                string.Empty,
                string.Empty,
                placement.PlacementId,
                result.Evidence.OpportunityId,
                result.Evidence.RewardToken
            );
        }

        public bool Complete(EntitlementReservation reservation)
        {
            if (reservation == null)
                return false;

            lock (syncRoot)
            {
                if (
                    !reservations.TryGetValue(
                        reservation.FulfillmentKey,
                        out EntitlementReservation current
                    ) ||
                    !ReferenceEquals(current, reservation) ||
                    current.Sequence != reservation.Sequence
                )
                {
                    return false;
                }

                string requestKey = RequestKey(reservation.Source, reservation.RequestId);
                reservations.Remove(reservation.FulfillmentKey);
                requestReservations.Remove(requestKey);
                completedFulfillmentKeys.Add(reservation.FulfillmentKey);
                completedRequestKeys.Add(requestKey);
                return true;
            }
        }

        public bool Release(EntitlementReservation reservation)
        {
            if (reservation == null)
                return false;

            lock (syncRoot)
            {
                if (
                    !reservations.TryGetValue(
                        reservation.FulfillmentKey,
                        out EntitlementReservation current
                    ) ||
                    !ReferenceEquals(current, reservation) ||
                    current.Sequence != reservation.Sequence
                )
                {
                    return false;
                }

                reservations.Remove(reservation.FulfillmentKey);
                requestReservations.Remove(
                    RequestKey(reservation.Source, reservation.RequestId)
                );
                return true;
            }
        }

        public bool HasCompleted(string fulfillmentKey)
        {
            if (!IsStableFulfillmentKey(fulfillmentKey))
                return false;

            lock (syncRoot)
                return completedFulfillmentKeys.Contains(fulfillmentKey);
        }

        public string[] ExportCompletedFulfillmentKeys()
        {
            lock (syncRoot)
            {
                var keys = new string[completedFulfillmentKeys.Count];
                completedFulfillmentKeys.CopyTo(keys);
                Array.Sort(keys, StringComparer.Ordinal);
                return keys;
            }
        }

        EntitlementEligibilityResult Reserve(
            string fulfillmentKey,
            string requestKey,
            string requestId,
            EntitlementSource source,
            MonetizationEnvironment environment,
            string providerId,
            string productId,
            string transactionId,
            string placementId,
            string opportunityId,
            string rewardToken
        )
        {
            lock (syncRoot)
            {
                if (
                    reservations.ContainsKey(fulfillmentKey) ||
                    completedFulfillmentKeys.Contains(fulfillmentKey)
                )
                {
                    return Rejected(MonetizationReasonCodes.DuplicateEntitlement);
                }

                if (
                    requestReservations.ContainsKey(requestKey) ||
                    completedRequestKeys.Contains(requestKey)
                )
                {
                    return Rejected(MonetizationReasonCodes.DuplicateRequest);
                }

                if (nextSequence == long.MaxValue)
                    nextSequence = 0;
                nextSequence++;
                var reservation = new EntitlementReservation(
                    nextSequence,
                    fulfillmentKey,
                    requestId,
                    source,
                    environment,
                    providerId,
                    productId,
                    transactionId,
                    placementId,
                    opportunityId,
                    rewardToken
                );
                reservations.Add(fulfillmentKey, reservation);
                requestReservations.Add(requestKey, reservation);
                return new EntitlementEligibilityResult(
                    true,
                    MonetizationReasonCodes.None,
                    reservation
                );
            }
        }

        string ValidatePurchaseBeforeVerification(
            PurchaseRequest request,
            PurchaseResult purchase,
            out PurchaseOfferDefinition offer
        )
        {
            offer = null;
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.OfferId) ||
                !MonetizationIdentifiers.IsStable(request.ProductId)
            )
            {
                return MonetizationReasonCodes.InvalidRequest;
            }

            if (!MonetizationAllowlists.TryGetPurchaseOffer(request.OfferId, out offer))
                return MonetizationReasonCodes.UnknownOffer;
            if (!string.Equals(offer.ProductId, request.ProductId, StringComparison.Ordinal))
                return MonetizationReasonCodes.ProductMismatch;
            if (purchase == null)
                return MonetizationReasonCodes.PurchaseNotCompleted;
            if (!string.Equals(purchase.RequestId, request.RequestId, StringComparison.Ordinal))
                return MonetizationReasonCodes.InvalidRequest;
            if (purchase.State != PurchaseState.PurchasedUnverified)
                return PurchaseFailureReason(purchase);
            if (!string.IsNullOrEmpty(purchase.ReasonCode))
                return MonetizationReasonCodes.InconsistentResult;
            if (purchase.Evidence == null)
                return MonetizationReasonCodes.EvidenceMissing;

            PurchaseEvidence evidence = purchase.Evidence;
            if (!ValidPurchaseEvidence(evidence))
                return MonetizationReasonCodes.EvidenceMalformed;
            if (evidence.Environment != expectedEnvironment)
                return MonetizationReasonCodes.EnvironmentMismatch;
            if (!string.Equals(evidence.ProductId, offer.ProductId, StringComparison.Ordinal))
                return MonetizationReasonCodes.ProductMismatch;
            return MonetizationReasonCodes.None;
        }

        string ValidatePurchase(
            PurchaseRequest request,
            PurchaseResult purchase,
            PurchaseVerificationResult verification,
            out PurchaseOfferDefinition offer,
            out string fulfillmentKey
        )
        {
            fulfillmentKey = string.Empty;
            string prevalidation = ValidatePurchaseBeforeVerification(
                request,
                purchase,
                out offer
            );
            if (!string.IsNullOrEmpty(prevalidation))
                return prevalidation;
            if (verification == null)
                return MonetizationReasonCodes.EvidenceUnverified;
            if (!verification.IsVerified)
                return VerificationFailureReason(verification.State, verification.ReasonCode);
            if (!string.IsNullOrEmpty(verification.ReasonCode))
                return MonetizationReasonCodes.InconsistentResult;

            PurchaseEvidence evidence = purchase.Evidence;
            if (
                !IsSupportedEnvironment(expectedEnvironment) ||
                verification.Environment != expectedEnvironment
            )
            {
                return MonetizationReasonCodes.EnvironmentMismatch;
            }
            if (!string.Equals(verification.ProductId, evidence.ProductId, StringComparison.Ordinal))
                return MonetizationReasonCodes.ProductMismatch;
            if (!string.Equals(verification.ProviderId, evidence.ProviderId, StringComparison.Ordinal))
                return MonetizationReasonCodes.ProviderMismatch;
            if (!string.Equals(verification.TransactionId, evidence.TransactionId, StringComparison.Ordinal))
                return MonetizationReasonCodes.EvidenceMalformed;

            fulfillmentKey = "iap|" + evidence.ProviderId + "|" + evidence.TransactionId;
            return MonetizationReasonCodes.None;
        }

        string ValidateRewardedAdBeforeVerification(
            RewardedAdRequest request,
            RewardedAdResult result,
            out RewardedPlacementDefinition placement
        )
        {
            placement = null;
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.PlacementId) ||
                !MonetizationIdentifiers.IsStable(request.OpportunityId)
            )
            {
                return MonetizationReasonCodes.InvalidRequest;
            }

            if (!MonetizationAllowlists.TryGetRewardedPlacement(
                request.PlacementId,
                out placement
            ))
            {
                return MonetizationReasonCodes.UnknownPlacement;
            }
            if (result == null)
                return MonetizationReasonCodes.AdNotCompleted;
            if (!string.Equals(result.RequestId, request.RequestId, StringComparison.Ordinal))
                return MonetizationReasonCodes.InvalidRequest;
            if (result.State != RewardedAdState.CompletedUnverified)
                return RewardedFailureReason(result);
            if (!string.IsNullOrEmpty(result.ReasonCode))
                return MonetizationReasonCodes.InconsistentResult;
            if (result.Evidence == null)
                return MonetizationReasonCodes.EvidenceMissing;

            RewardedAdEvidence evidence = result.Evidence;
            if (!ValidRewardedEvidence(evidence))
                return MonetizationReasonCodes.EvidenceMalformed;
            if (evidence.Environment != expectedEnvironment)
                return MonetizationReasonCodes.EnvironmentMismatch;
            if (!string.Equals(evidence.PlacementId, request.PlacementId, StringComparison.Ordinal))
                return MonetizationReasonCodes.PlacementMismatch;
            if (!string.Equals(evidence.OpportunityId, request.OpportunityId, StringComparison.Ordinal))
                return MonetizationReasonCodes.OpportunityMismatch;
            return MonetizationReasonCodes.None;
        }

        string ValidateRewardedAd(
            RewardedAdRequest request,
            RewardedAdResult result,
            RewardedAdVerificationResult verification,
            out RewardedPlacementDefinition placement,
            out string fulfillmentKey
        )
        {
            fulfillmentKey = string.Empty;
            string prevalidation = ValidateRewardedAdBeforeVerification(
                request,
                result,
                out placement
            );
            if (!string.IsNullOrEmpty(prevalidation))
                return prevalidation;
            if (verification == null)
                return MonetizationReasonCodes.EvidenceUnverified;
            if (!verification.IsVerified)
                return VerificationFailureReason(verification.State, verification.ReasonCode);
            if (!string.IsNullOrEmpty(verification.ReasonCode))
                return MonetizationReasonCodes.InconsistentResult;

            RewardedAdEvidence evidence = result.Evidence;
            if (
                !IsSupportedEnvironment(expectedEnvironment) ||
                verification.Environment != expectedEnvironment
            )
            {
                return MonetizationReasonCodes.EnvironmentMismatch;
            }
            if (!string.Equals(verification.PlacementId, evidence.PlacementId, StringComparison.Ordinal))
            {
                return MonetizationReasonCodes.PlacementMismatch;
            }
            if (!string.Equals(verification.OpportunityId, evidence.OpportunityId, StringComparison.Ordinal))
            {
                return MonetizationReasonCodes.OpportunityMismatch;
            }
            if (!string.Equals(verification.ProviderId, evidence.ProviderId, StringComparison.Ordinal))
                return MonetizationReasonCodes.ProviderMismatch;
            if (!string.Equals(verification.RewardToken, evidence.RewardToken, StringComparison.Ordinal))
                return MonetizationReasonCodes.EvidenceMalformed;

            fulfillmentKey = "ad|" + evidence.ProviderId + "|" + evidence.RewardToken;
            return MonetizationReasonCodes.None;
        }

        static bool ValidPurchaseEvidence(PurchaseEvidence evidence)
        {
            return
                evidence != null &&
                IsSupportedEnvironment(evidence.Environment) &&
                MonetizationIdentifiers.IsStable(evidence.ProviderId) &&
                MonetizationIdentifiers.IsStable(evidence.ProductId) &&
                MonetizationIdentifiers.IsStable(evidence.TransactionId) &&
                !string.IsNullOrWhiteSpace(evidence.ReceiptPayload) &&
                evidence.ReceiptPayload.Length <= MaxReceiptPayloadLength;
        }

        static bool ValidRewardedEvidence(RewardedAdEvidence evidence)
        {
            return
                evidence != null &&
                IsSupportedEnvironment(evidence.Environment) &&
                MonetizationIdentifiers.IsStable(evidence.ProviderId) &&
                MonetizationIdentifiers.IsStable(evidence.PlacementId) &&
                MonetizationIdentifiers.IsStable(evidence.OpportunityId) &&
                MonetizationIdentifiers.IsStable(evidence.RewardToken) &&
                !string.IsNullOrWhiteSpace(evidence.ProofPayload) &&
                evidence.ProofPayload.Length <= MaxAdProofPayloadLength;
        }

        static string PurchaseFailureReason(PurchaseResult result)
        {
            if (!string.IsNullOrEmpty(result.ReasonCode))
                return result.ReasonCode;
            switch (result.State)
            {
                case PurchaseState.Pending:
                    return MonetizationReasonCodes.PurchasePending;
                case PurchaseState.Cancelled:
                    return MonetizationReasonCodes.PurchaseCancelled;
                case PurchaseState.Declined:
                    return MonetizationReasonCodes.PurchaseDeclined;
                case PurchaseState.Unavailable:
                    return MonetizationReasonCodes.AdapterUnavailable;
                default:
                    return MonetizationReasonCodes.PurchaseFailed;
            }
        }

        static string RewardedFailureReason(RewardedAdResult result)
        {
            if (!string.IsNullOrEmpty(result.ReasonCode))
                return result.ReasonCode;
            switch (result.State)
            {
                case RewardedAdState.Pending:
                    return MonetizationReasonCodes.AdPending;
                case RewardedAdState.ClosedEarly:
                    return MonetizationReasonCodes.AdClosedEarly;
                case RewardedAdState.Skipped:
                    return MonetizationReasonCodes.AdSkipped;
                case RewardedAdState.Cancelled:
                    return MonetizationReasonCodes.AdCancelled;
                case RewardedAdState.Unavailable:
                    return MonetizationReasonCodes.AdapterUnavailable;
                default:
                    return MonetizationReasonCodes.AdFailed;
            }
        }

        static string VerificationFailureReason(
            VerificationState state,
            string explicitReason
        )
        {
            if (!string.IsNullOrEmpty(explicitReason))
                return explicitReason;
            switch (state)
            {
                case VerificationState.Rejected:
                    return MonetizationReasonCodes.VerificationRejected;
                case VerificationState.Retryable:
                    return MonetizationReasonCodes.VerificationRetryable;
                case VerificationState.Unavailable:
                    return MonetizationReasonCodes.VerifierUnavailable;
                default:
                    return MonetizationReasonCodes.EvidenceUnverified;
            }
        }

        static EntitlementEligibilityResult Rejected(string reasonCode)
        {
            return new EntitlementEligibilityResult(false, reasonCode, null);
        }

        string InitializationFailureReason()
        {
            if (!environmentValid)
                return MonetizationReasonCodes.EnvironmentMismatch;
            if (!verifierBindingsValid)
                return MonetizationReasonCodes.VerifierUnavailable;
            if (!durableHistoryValid)
                return MonetizationReasonCodes.DurableHistoryInvalid;
            return MonetizationReasonCodes.None;
        }

        static bool IsSupportedEnvironment(MonetizationEnvironment environment)
        {
            return
                environment == MonetizationEnvironment.DevelopmentSandbox ||
                environment == MonetizationEnvironment.Production;
        }

        static string RequestKey(EntitlementSource source, string requestId)
        {
            return (source == EntitlementSource.InAppPurchase ? "iap|" : "ad|") +
                "request|" + requestId;
        }

        static bool IsStableFulfillmentKey(string key)
        {
            if (
                string.IsNullOrWhiteSpace(key) ||
                key.Length > 512 ||
                !string.Equals(key, key.Trim(), StringComparison.Ordinal)
            )
            {
                return false;
            }

            string[] parts = key.Split('|');
            return
                parts.Length == 3 &&
                (
                    string.Equals(parts[0], "iap", StringComparison.Ordinal) ||
                    string.Equals(parts[0], "ad", StringComparison.Ordinal)
                ) &&
                MonetizationIdentifiers.IsStable(parts[1]) &&
                MonetizationIdentifiers.IsStable(parts[2]);
        }
    }
}
