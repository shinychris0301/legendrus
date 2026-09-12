using System;
using System.Collections.Generic;

namespace LegendsOfRus.CommercialCore.Monetization
{
    public static class DevelopmentMonetizationProofs
    {
        public const string ProviderId = "development_sandbox";

        public static string PurchaseReceipt(string productId, string transactionId)
        {
            return "DEV-PURCHASE|" + (productId ?? string.Empty) + "|" +
                (transactionId ?? string.Empty);
        }

        public static string RewardedProof(
            string placementId,
            string opportunityId,
            string rewardToken
        )
        {
            return "DEV-AD|" + (placementId ?? string.Empty) + "|" +
                (opportunityId ?? string.Empty) + "|" +
                (rewardToken ?? string.Empty);
        }
    }

    public sealed class DevelopmentPurchaseScript
    {
        public PurchaseState State { get; private set; }
        public string ReasonCode { get; private set; }
        public string TransactionId { get; private set; }
        public bool HoldInFlight { get; private set; }

        public DevelopmentPurchaseScript(
            PurchaseState state,
            string reasonCode,
            string transactionId,
            bool holdInFlight
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.PurchaseFailed
            );
            TransactionId = transactionId ?? string.Empty;
            // A pending result must remain resolvable. Coerce legacy false values
            // to the safe behavior so a caller can never receive an orphaned
            // pending operation.
            HoldInFlight = state == PurchaseState.Pending;
        }

        public static DevelopmentPurchaseScript Purchased(string transactionId)
        {
            return new DevelopmentPurchaseScript(
                PurchaseState.PurchasedUnverified,
                MonetizationReasonCodes.None,
                transactionId,
                false
            );
        }

        public static DevelopmentPurchaseScript Pending(bool holdInFlight = true)
        {
            return new DevelopmentPurchaseScript(
                PurchaseState.Pending,
                MonetizationReasonCodes.PurchasePending,
                string.Empty,
                holdInFlight
            );
        }

        public static DevelopmentPurchaseScript Cancelled()
        {
            return new DevelopmentPurchaseScript(
                PurchaseState.Cancelled,
                MonetizationReasonCodes.PurchaseCancelled,
                string.Empty,
                false
            );
        }

        public static DevelopmentPurchaseScript Declined()
        {
            return new DevelopmentPurchaseScript(
                PurchaseState.Declined,
                MonetizationReasonCodes.PurchaseDeclined,
                string.Empty,
                false
            );
        }

        public static DevelopmentPurchaseScript Failed(string reasonCode = null)
        {
            return new DevelopmentPurchaseScript(
                PurchaseState.Failed,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.PurchaseFailed
                    : reasonCode,
                string.Empty,
                false
            );
        }
    }

    public sealed class DevelopmentRewardedAdScript
    {
        public RewardedAdState State { get; private set; }
        public string ReasonCode { get; private set; }
        public string RewardToken { get; private set; }
        public bool HoldInFlight { get; private set; }

        public DevelopmentRewardedAdScript(
            RewardedAdState state,
            string reasonCode,
            string rewardToken,
            bool holdInFlight
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.AdFailed
            );
            RewardToken = rewardToken ?? string.Empty;
            // A pending result must remain resolvable. Coerce legacy false values
            // to the safe behavior so a caller can never receive an orphaned
            // pending operation.
            HoldInFlight = state == RewardedAdState.Pending;
        }

        public static DevelopmentRewardedAdScript Completed(string rewardToken)
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.CompletedUnverified,
                MonetizationReasonCodes.None,
                rewardToken,
                false
            );
        }

        public static DevelopmentRewardedAdScript Pending(bool holdInFlight = true)
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.Pending,
                MonetizationReasonCodes.AdPending,
                string.Empty,
                holdInFlight
            );
        }

        public static DevelopmentRewardedAdScript ClosedEarly()
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.ClosedEarly,
                MonetizationReasonCodes.AdClosedEarly,
                string.Empty,
                false
            );
        }

        public static DevelopmentRewardedAdScript Skipped()
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.Skipped,
                MonetizationReasonCodes.AdSkipped,
                string.Empty,
                false
            );
        }

        public static DevelopmentRewardedAdScript Cancelled()
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.Cancelled,
                MonetizationReasonCodes.AdCancelled,
                string.Empty,
                false
            );
        }

        public static DevelopmentRewardedAdScript Failed(string reasonCode = null)
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.Failed,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.AdFailed
                    : reasonCode,
                string.Empty,
                false
            );
        }

        public static DevelopmentRewardedAdScript Unavailable()
        {
            return new DevelopmentRewardedAdScript(
                RewardedAdState.Unavailable,
                MonetizationReasonCodes.AdapterUnavailable,
                string.Empty,
                false
            );
        }
    }

    public sealed class DevelopmentVerificationScript
    {
        public VerificationState State { get; private set; }
        public string ReasonCode { get; private set; }

        public DevelopmentVerificationScript(
            VerificationState state,
            string reasonCode
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.VerificationRejected
            );
        }

        public static DevelopmentVerificationScript Verified()
        {
            return new DevelopmentVerificationScript(
                VerificationState.Verified,
                MonetizationReasonCodes.None
            );
        }

        public static DevelopmentVerificationScript Rejected()
        {
            return new DevelopmentVerificationScript(
                VerificationState.Rejected,
                MonetizationReasonCodes.VerificationRejected
            );
        }

        public static DevelopmentVerificationScript Retryable()
        {
            return new DevelopmentVerificationScript(
                VerificationState.Retryable,
                MonetizationReasonCodes.VerificationRetryable
            );
        }

        public static DevelopmentVerificationScript Unavailable()
        {
            return new DevelopmentVerificationScript(
                VerificationState.Unavailable,
                MonetizationReasonCodes.VerifierUnavailable
            );
        }
    }

    /// <summary>
    /// Explicitly scripted store sandbox. It defaults unavailable and creates
    /// evidence only; it has no economy, save, UI, analytics, or SDK dependency.
    /// </summary>
    public sealed class ScriptedDevelopmentIapAdapter : IIapAdapter
    {
        readonly object syncRoot = new object();
        readonly Queue<DevelopmentPurchaseScript> scripts =
            new Queue<DevelopmentPurchaseScript>();
        readonly HashSet<string> completedRequestIds =
            new HashSet<string>(StringComparer.Ordinal);
        bool available;
        string inFlightRequestId = string.Empty;
        PurchaseRequest inFlightRequest;

        public MonetizationEnvironment Environment
        {
            get { return MonetizationEnvironment.DevelopmentSandbox; }
        }

        public bool IsBusy
        {
            get
            {
                lock (syncRoot)
                    return !string.IsNullOrEmpty(inFlightRequestId);
            }
        }

        public int QueuedScriptCount
        {
            get
            {
                lock (syncRoot)
                    return scripts.Count;
            }
        }

        public void SetAvailable(bool value)
        {
            lock (syncRoot)
                available = value;
        }

        public void Enqueue(DevelopmentPurchaseScript script)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            lock (syncRoot)
                scripts.Enqueue(script);
        }

        public CommercialAvailability GetAvailability(string offerId)
        {
            if (!MonetizationAllowlists.TryGetPurchaseOffer(offerId, out _))
                return CommercialAvailability.Unavailable(
                    MonetizationReasonCodes.UnknownOffer
                );

            lock (syncRoot)
            {
                if (!available)
                    return CommercialAvailability.Unavailable(
                        MonetizationReasonCodes.AdapterUnavailable
                    );
                if (!string.IsNullOrEmpty(inFlightRequestId))
                {
                    return new CommercialAvailability(
                        CommercialAvailabilityState.Busy,
                        MonetizationReasonCodes.Busy
                    );
                }
                return CommercialAvailability.Ready();
            }
        }

        public PurchaseResult BeginPurchase(PurchaseRequest request)
        {
            string validation = ValidatePurchaseRequest(request);
            if (!string.IsNullOrEmpty(validation))
            {
                return new PurchaseResult(
                    request != null ? request.RequestId : string.Empty,
                    PurchaseState.Failed,
                    validation,
                    null
                );
            }

            lock (syncRoot)
            {
                if (completedRequestIds.Contains(request.RequestId))
                    return PurchaseFailure(request.RequestId, MonetizationReasonCodes.DuplicateRequest);
                if (!string.IsNullOrEmpty(inFlightRequestId))
                {
                    return PurchaseFailure(
                        request.RequestId,
                        string.Equals(
                            inFlightRequestId,
                            request.RequestId,
                            StringComparison.Ordinal
                        )
                            ? MonetizationReasonCodes.DuplicateRequest
                            : MonetizationReasonCodes.Busy
                    );
                }
                if (!available)
                {
                    completedRequestIds.Add(request.RequestId);
                    return new PurchaseResult(
                        request.RequestId,
                        PurchaseState.Unavailable,
                        MonetizationReasonCodes.AdapterUnavailable,
                        null
                    );
                }
                if (scripts.Count == 0)
                {
                    completedRequestIds.Add(request.RequestId);
                    return new PurchaseResult(
                        request.RequestId,
                        PurchaseState.Unavailable,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        null
                    );
                }

                inFlightRequestId = request.RequestId;
                inFlightRequest = request;
                DevelopmentPurchaseScript script = scripts.Dequeue();
                PurchaseResult result = BuildPurchaseResult(request, script);
                if (!script.HoldInFlight)
                {
                    inFlightRequestId = string.Empty;
                    inFlightRequest = null;
                    completedRequestIds.Add(request.RequestId);
                }
                return result;
            }
        }

        public PurchaseResult ResolvePending(string requestId)
        {
            if (!MonetizationIdentifiers.IsStable(requestId))
                return PurchaseFailure(requestId, MonetizationReasonCodes.InvalidRequest);

            lock (syncRoot)
            {
                if (
                    inFlightRequest == null ||
                    !string.Equals(
                        inFlightRequestId,
                        requestId,
                        StringComparison.Ordinal
                    )
                )
                {
                    return PurchaseFailure(
                        requestId,
                        MonetizationReasonCodes.NoPendingOperation
                    );
                }

                if (scripts.Count == 0)
                {
                    return new PurchaseResult(
                        requestId,
                        PurchaseState.Pending,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        null
                    );
                }

                DevelopmentPurchaseScript script = scripts.Dequeue();
                PurchaseResult result = BuildPurchaseResult(inFlightRequest, script);
                if (!script.HoldInFlight)
                {
                    inFlightRequestId = string.Empty;
                    inFlightRequest = null;
                    completedRequestIds.Add(requestId);
                }
                return result;
            }
        }

        public bool ReleasePending(string requestId)
        {
            if (!MonetizationIdentifiers.IsStable(requestId))
                return false;

            lock (syncRoot)
            {
                if (
                    inFlightRequest == null ||
                    !string.Equals(inFlightRequestId, requestId, StringComparison.Ordinal)
                )
                {
                    return false;
                }
                inFlightRequestId = string.Empty;
                inFlightRequest = null;
                completedRequestIds.Add(requestId);
                return true;
            }
        }

        static string ValidatePurchaseRequest(PurchaseRequest request)
        {
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.OfferId) ||
                !MonetizationIdentifiers.IsStable(request.ProductId)
            )
            {
                return MonetizationReasonCodes.InvalidRequest;
            }
            if (!MonetizationAllowlists.TryGetPurchaseOffer(
                request.OfferId,
                out PurchaseOfferDefinition offer
            ))
            {
                return MonetizationReasonCodes.UnknownOffer;
            }
            return string.Equals(
                offer.ProductId,
                request.ProductId,
                StringComparison.Ordinal
            )
                ? MonetizationReasonCodes.None
                : MonetizationReasonCodes.ProductMismatch;
        }

        static PurchaseResult BuildPurchaseResult(
            PurchaseRequest request,
            DevelopmentPurchaseScript script
        )
        {
            if (script.State != PurchaseState.PurchasedUnverified)
            {
                return new PurchaseResult(
                    request.RequestId,
                    script.State,
                    script.ReasonCode,
                    null
                );
            }

            if (!MonetizationIdentifiers.IsStable(script.TransactionId))
            {
                return PurchaseFailure(
                    request.RequestId,
                    MonetizationReasonCodes.EvidenceMalformed
                );
            }

            var evidence = new PurchaseEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                request.ProductId,
                script.TransactionId,
                DevelopmentMonetizationProofs.PurchaseReceipt(
                    request.ProductId,
                    script.TransactionId
                )
            );
            return new PurchaseResult(
                request.RequestId,
                PurchaseState.PurchasedUnverified,
                MonetizationReasonCodes.None,
                evidence
            );
        }

        static PurchaseResult PurchaseFailure(string requestId, string reasonCode)
        {
            return new PurchaseResult(
                requestId,
                PurchaseState.Failed,
                reasonCode,
                null
            );
        }
    }

    public sealed class ScriptedDevelopmentPurchaseVerifier : IPurchaseVerifier
    {
        readonly object syncRoot = new object();
        readonly Queue<DevelopmentVerificationScript> scripts =
            new Queue<DevelopmentVerificationScript>();

        public void Enqueue(DevelopmentVerificationScript script)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            lock (syncRoot)
                scripts.Enqueue(script);
        }

        public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
        {
            DevelopmentVerificationScript script;
            lock (syncRoot)
            {
                if (scripts.Count == 0)
                {
                    return PurchaseVerification(
                        VerificationState.Unavailable,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        evidence
                    );
                }
                script = scripts.Dequeue();
            }

            if (script.State == VerificationState.Verified &&
                !ValidDevelopmentPurchaseEvidence(evidence))
            {
                return PurchaseVerification(
                    VerificationState.Rejected,
                    MonetizationReasonCodes.EvidenceMalformed,
                    evidence
                );
            }

            return PurchaseVerification(script.State, script.ReasonCode, evidence);
        }

        static bool ValidDevelopmentPurchaseEvidence(PurchaseEvidence evidence)
        {
            return
                evidence != null &&
                evidence.Environment == MonetizationEnvironment.DevelopmentSandbox &&
                string.Equals(
                    evidence.ProviderId,
                    DevelopmentMonetizationProofs.ProviderId,
                    StringComparison.Ordinal
                ) &&
                MonetizationIdentifiers.IsStable(evidence.ProductId) &&
                MonetizationIdentifiers.IsStable(evidence.TransactionId) &&
                string.Equals(
                    evidence.ReceiptPayload,
                    DevelopmentMonetizationProofs.PurchaseReceipt(
                        evidence.ProductId,
                        evidence.TransactionId
                    ),
                    StringComparison.Ordinal
                );
        }

        static PurchaseVerificationResult PurchaseVerification(
            VerificationState state,
            string reasonCode,
            PurchaseEvidence evidence
        )
        {
            switch (state)
            {
                case VerificationState.Verified:
                    return PurchaseVerificationResult.Verified(evidence);
                case VerificationState.Retryable:
                    return PurchaseVerificationResult.Retryable(evidence, reasonCode);
                case VerificationState.Unavailable:
                    return PurchaseVerificationResult.Unavailable(evidence, reasonCode);
                default:
                    return PurchaseVerificationResult.Rejected(evidence, reasonCode);
            }
        }
    }

    /// <summary>
    /// Explicitly scripted rewarded-ad sandbox. It defaults unavailable and
    /// returns proof evidence only after an explicitly queued completion.
    /// </summary>
    public sealed class ScriptedDevelopmentRewardedAdAdapter : IRewardedAdAdapter
    {
        readonly object syncRoot = new object();
        readonly Queue<DevelopmentRewardedAdScript> scripts =
            new Queue<DevelopmentRewardedAdScript>();
        readonly HashSet<string> completedRequestIds =
            new HashSet<string>(StringComparer.Ordinal);
        bool available;
        string inFlightRequestId = string.Empty;
        RewardedAdRequest inFlightRequest;

        public MonetizationEnvironment Environment
        {
            get { return MonetizationEnvironment.DevelopmentSandbox; }
        }

        public bool IsBusy
        {
            get
            {
                lock (syncRoot)
                    return !string.IsNullOrEmpty(inFlightRequestId);
            }
        }

        public void SetAvailable(bool value)
        {
            lock (syncRoot)
                available = value;
        }

        public void Enqueue(DevelopmentRewardedAdScript script)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            lock (syncRoot)
                scripts.Enqueue(script);
        }

        public CommercialAvailability GetAvailability(string placementId)
        {
            if (!MonetizationAllowlists.TryGetRewardedPlacement(placementId, out _))
                return CommercialAvailability.Unavailable(
                    MonetizationReasonCodes.UnknownPlacement
                );

            lock (syncRoot)
            {
                if (!available)
                    return CommercialAvailability.Unavailable(
                        MonetizationReasonCodes.AdapterUnavailable
                    );
                if (!string.IsNullOrEmpty(inFlightRequestId))
                {
                    return new CommercialAvailability(
                        CommercialAvailabilityState.Busy,
                        MonetizationReasonCodes.Busy
                    );
                }
                return CommercialAvailability.Ready();
            }
        }

        public RewardedAdResult Show(RewardedAdRequest request)
        {
            string validation = ValidateRewardedRequest(request);
            if (!string.IsNullOrEmpty(validation))
            {
                return new RewardedAdResult(
                    request != null ? request.RequestId : string.Empty,
                    RewardedAdState.Failed,
                    validation,
                    null
                );
            }

            lock (syncRoot)
            {
                if (completedRequestIds.Contains(request.RequestId))
                    return AdFailure(request.RequestId, MonetizationReasonCodes.DuplicateRequest);
                if (!string.IsNullOrEmpty(inFlightRequestId))
                {
                    return AdFailure(
                        request.RequestId,
                        string.Equals(
                            inFlightRequestId,
                            request.RequestId,
                            StringComparison.Ordinal
                        )
                            ? MonetizationReasonCodes.DuplicateRequest
                            : MonetizationReasonCodes.Busy
                    );
                }
                if (!available)
                {
                    completedRequestIds.Add(request.RequestId);
                    return new RewardedAdResult(
                        request.RequestId,
                        RewardedAdState.Unavailable,
                        MonetizationReasonCodes.AdapterUnavailable,
                        null
                    );
                }
                if (scripts.Count == 0)
                {
                    completedRequestIds.Add(request.RequestId);
                    return new RewardedAdResult(
                        request.RequestId,
                        RewardedAdState.Unavailable,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        null
                    );
                }

                inFlightRequestId = request.RequestId;
                inFlightRequest = request;
                DevelopmentRewardedAdScript script = scripts.Dequeue();
                RewardedAdResult result = BuildRewardedResult(request, script);
                if (!script.HoldInFlight)
                {
                    inFlightRequestId = string.Empty;
                    inFlightRequest = null;
                    completedRequestIds.Add(request.RequestId);
                }
                return result;
            }
        }

        public RewardedAdResult ResolvePending(string requestId)
        {
            if (!MonetizationIdentifiers.IsStable(requestId))
                return AdFailure(requestId, MonetizationReasonCodes.InvalidRequest);

            lock (syncRoot)
            {
                if (
                    inFlightRequest == null ||
                    !string.Equals(
                        inFlightRequestId,
                        requestId,
                        StringComparison.Ordinal
                    )
                )
                {
                    return AdFailure(
                        requestId,
                        MonetizationReasonCodes.NoPendingOperation
                    );
                }

                if (scripts.Count == 0)
                {
                    return new RewardedAdResult(
                        requestId,
                        RewardedAdState.Pending,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        null
                    );
                }

                DevelopmentRewardedAdScript script = scripts.Dequeue();
                RewardedAdResult result = BuildRewardedResult(inFlightRequest, script);
                if (!script.HoldInFlight)
                {
                    inFlightRequestId = string.Empty;
                    inFlightRequest = null;
                    completedRequestIds.Add(requestId);
                }
                return result;
            }
        }

        public bool ReleasePending(string requestId)
        {
            if (!MonetizationIdentifiers.IsStable(requestId))
                return false;

            lock (syncRoot)
            {
                if (
                    inFlightRequest == null ||
                    !string.Equals(inFlightRequestId, requestId, StringComparison.Ordinal)
                )
                {
                    return false;
                }
                inFlightRequestId = string.Empty;
                inFlightRequest = null;
                completedRequestIds.Add(requestId);
                return true;
            }
        }

        static string ValidateRewardedRequest(RewardedAdRequest request)
        {
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.PlacementId) ||
                !MonetizationIdentifiers.IsStable(request.OpportunityId)
            )
            {
                return MonetizationReasonCodes.InvalidRequest;
            }
            return MonetizationAllowlists.TryGetRewardedPlacement(
                request.PlacementId,
                out _
            )
                ? MonetizationReasonCodes.None
                : MonetizationReasonCodes.UnknownPlacement;
        }

        static RewardedAdResult BuildRewardedResult(
            RewardedAdRequest request,
            DevelopmentRewardedAdScript script
        )
        {
            if (script.State != RewardedAdState.CompletedUnverified)
            {
                return new RewardedAdResult(
                    request.RequestId,
                    script.State,
                    script.ReasonCode,
                    null
                );
            }

            if (!MonetizationIdentifiers.IsStable(script.RewardToken))
            {
                return AdFailure(
                    request.RequestId,
                    MonetizationReasonCodes.EvidenceMalformed
                );
            }

            var evidence = new RewardedAdEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                request.PlacementId,
                request.OpportunityId,
                script.RewardToken,
                DevelopmentMonetizationProofs.RewardedProof(
                    request.PlacementId,
                    request.OpportunityId,
                    script.RewardToken
                )
            );
            return new RewardedAdResult(
                request.RequestId,
                RewardedAdState.CompletedUnverified,
                MonetizationReasonCodes.None,
                evidence
            );
        }

        static RewardedAdResult AdFailure(string requestId, string reasonCode)
        {
            return new RewardedAdResult(
                requestId,
                RewardedAdState.Failed,
                reasonCode,
                null
            );
        }
    }

    public sealed class ScriptedDevelopmentRewardedAdVerifier : IRewardedAdVerifier
    {
        readonly object syncRoot = new object();
        readonly Queue<DevelopmentVerificationScript> scripts =
            new Queue<DevelopmentVerificationScript>();

        public void Enqueue(DevelopmentVerificationScript script)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            lock (syncRoot)
                scripts.Enqueue(script);
        }

        public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
        {
            DevelopmentVerificationScript script;
            lock (syncRoot)
            {
                if (scripts.Count == 0)
                {
                    return RewardedVerification(
                        VerificationState.Unavailable,
                        MonetizationReasonCodes.DevelopmentScriptMissing,
                        evidence
                    );
                }
                script = scripts.Dequeue();
            }

            if (script.State == VerificationState.Verified &&
                !ValidDevelopmentRewardedEvidence(evidence))
            {
                return RewardedVerification(
                    VerificationState.Rejected,
                    MonetizationReasonCodes.EvidenceMalformed,
                    evidence
                );
            }

            return RewardedVerification(script.State, script.ReasonCode, evidence);
        }

        static bool ValidDevelopmentRewardedEvidence(RewardedAdEvidence evidence)
        {
            return
                evidence != null &&
                evidence.Environment == MonetizationEnvironment.DevelopmentSandbox &&
                string.Equals(
                    evidence.ProviderId,
                    DevelopmentMonetizationProofs.ProviderId,
                    StringComparison.Ordinal
                ) &&
                MonetizationIdentifiers.IsStable(evidence.PlacementId) &&
                MonetizationIdentifiers.IsStable(evidence.OpportunityId) &&
                MonetizationIdentifiers.IsStable(evidence.RewardToken) &&
                string.Equals(
                    evidence.ProofPayload,
                    DevelopmentMonetizationProofs.RewardedProof(
                        evidence.PlacementId,
                        evidence.OpportunityId,
                        evidence.RewardToken
                    ),
                    StringComparison.Ordinal
                );
        }

        static RewardedAdVerificationResult RewardedVerification(
            VerificationState state,
            string reasonCode,
            RewardedAdEvidence evidence
        )
        {
            switch (state)
            {
                case VerificationState.Verified:
                    return RewardedAdVerificationResult.Verified(evidence);
                case VerificationState.Retryable:
                    return RewardedAdVerificationResult.Retryable(evidence, reasonCode);
                case VerificationState.Unavailable:
                    return RewardedAdVerificationResult.Unavailable(evidence, reasonCode);
                default:
                    return RewardedAdVerificationResult.Rejected(evidence, reasonCode);
            }
        }
    }
}
