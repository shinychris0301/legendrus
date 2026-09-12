using System;

namespace LegendsOfRus.CommercialCore.Monetization
{
    public enum MonetizationEnvironment
    {
        Disabled = 0,
        DevelopmentSandbox = 1,
        Production = 2
    }

    public enum CommercialAvailabilityState
    {
        Disabled = 0,
        Ready = 1,
        Unavailable = 2,
        Offline = 3,
        ConsentRequired = 4,
        NotInitialized = 5,
        Busy = 6,
        Unsupported = 7
    }

    public enum PurchaseState
    {
        Pending = 0,
        PurchasedUnverified = 1,
        Cancelled = 2,
        Declined = 3,
        Unavailable = 4,
        Failed = 5
    }

    public enum VerificationState
    {
        Unavailable = 0,
        Verified = 1,
        Rejected = 2,
        Retryable = 3
    }

    public enum RewardedAdState
    {
        Pending = 0,
        CompletedUnverified = 1,
        ClosedEarly = 2,
        Skipped = 3,
        Cancelled = 4,
        Unavailable = 5,
        Failed = 6
    }

    public sealed class CommercialAvailability
    {
        public CommercialAvailabilityState State { get; private set; }
        public string ReasonCode { get; private set; }
        public bool IsReady { get { return State == CommercialAvailabilityState.Ready; } }

        public CommercialAvailability(
            CommercialAvailabilityState state,
            string reasonCode
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.AdapterUnavailable
            );
        }

        public static CommercialAvailability Ready()
        {
            return new CommercialAvailability(
                CommercialAvailabilityState.Ready,
                MonetizationReasonCodes.None
            );
        }

        public static CommercialAvailability Unavailable(string reasonCode)
        {
            return new CommercialAvailability(
                CommercialAvailabilityState.Unavailable,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.AdapterUnavailable
                    : reasonCode
            );
        }
    }

    public sealed class PurchaseRequest
    {
        public string RequestId { get; private set; }
        public string OfferId { get; private set; }
        public string ProductId { get; private set; }

        public PurchaseRequest(string requestId, string offerId, string productId)
        {
            RequestId = requestId ?? string.Empty;
            OfferId = offerId ?? string.Empty;
            ProductId = productId ?? string.Empty;
        }
    }

    public sealed class PurchaseEvidence
    {
        public MonetizationEnvironment Environment { get; private set; }
        public string ProviderId { get; private set; }
        public string ProductId { get; private set; }
        public string TransactionId { get; private set; }
        public string ReceiptPayload { get; private set; }

        public PurchaseEvidence(
            MonetizationEnvironment environment,
            string providerId,
            string productId,
            string transactionId,
            string receiptPayload
        )
        {
            Environment = environment;
            ProviderId = providerId ?? string.Empty;
            ProductId = productId ?? string.Empty;
            TransactionId = transactionId ?? string.Empty;
            ReceiptPayload = receiptPayload ?? string.Empty;
        }
    }

    public sealed class PurchaseResult
    {
        public string RequestId { get; private set; }
        public PurchaseState State { get; private set; }
        public string ReasonCode { get; private set; }
        public PurchaseEvidence Evidence { get; private set; }

        public PurchaseResult(
            string requestId,
            PurchaseState state,
            string reasonCode,
            PurchaseEvidence evidence
        )
        {
            RequestId = requestId ?? string.Empty;
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.PurchaseFailed
            );
            Evidence = evidence;
        }
    }

    public sealed class PurchaseVerificationResult
    {
        public VerificationState State { get; private set; }
        public string ReasonCode { get; private set; }
        public MonetizationEnvironment Environment { get; private set; }
        public string ProviderId { get; private set; }
        public string ProductId { get; private set; }
        public string TransactionId { get; private set; }

        public bool IsVerified { get { return State == VerificationState.Verified; } }

        internal PurchaseVerificationResult(
            VerificationState state,
            string reasonCode,
            MonetizationEnvironment environment,
            string providerId,
            string productId,
            string transactionId
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.VerificationRejected
            );
            Environment = environment;
            ProviderId = providerId ?? string.Empty;
            ProductId = productId ?? string.Empty;
            TransactionId = transactionId ?? string.Empty;
        }

        public static PurchaseVerificationResult Verified(PurchaseEvidence evidence)
        {
            return FromEvidence(
                VerificationState.Verified,
                MonetizationReasonCodes.None,
                evidence
            );
        }

        public static PurchaseVerificationResult Rejected(
            PurchaseEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Rejected,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerificationRejected
                    : reasonCode,
                evidence
            );
        }

        public static PurchaseVerificationResult Retryable(
            PurchaseEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Retryable,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerificationRetryable
                    : reasonCode,
                evidence
            );
        }

        public static PurchaseVerificationResult Unavailable(
            PurchaseEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Unavailable,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerifierUnavailable
                    : reasonCode,
                evidence
            );
        }

        static PurchaseVerificationResult FromEvidence(
            VerificationState state,
            string reasonCode,
            PurchaseEvidence evidence
        )
        {
            return new PurchaseVerificationResult(
                state,
                reasonCode,
                evidence != null
                    ? evidence.Environment
                    : MonetizationEnvironment.Disabled,
                evidence != null ? evidence.ProviderId : string.Empty,
                evidence != null ? evidence.ProductId : string.Empty,
                evidence != null ? evidence.TransactionId : string.Empty
            );
        }
    }

    public sealed class RewardedAdRequest
    {
        public string RequestId { get; private set; }
        public string PlacementId { get; private set; }
        public string OpportunityId { get; private set; }

        public RewardedAdRequest(
            string requestId,
            string placementId,
            string opportunityId
        )
        {
            RequestId = requestId ?? string.Empty;
            PlacementId = placementId ?? string.Empty;
            OpportunityId = opportunityId ?? string.Empty;
        }
    }

    public sealed class RewardedAdEvidence
    {
        public MonetizationEnvironment Environment { get; private set; }
        public string ProviderId { get; private set; }
        public string PlacementId { get; private set; }
        public string OpportunityId { get; private set; }
        public string RewardToken { get; private set; }
        public string ProofPayload { get; private set; }

        public RewardedAdEvidence(
            MonetizationEnvironment environment,
            string providerId,
            string placementId,
            string opportunityId,
            string rewardToken,
            string proofPayload
        )
        {
            Environment = environment;
            ProviderId = providerId ?? string.Empty;
            PlacementId = placementId ?? string.Empty;
            OpportunityId = opportunityId ?? string.Empty;
            RewardToken = rewardToken ?? string.Empty;
            ProofPayload = proofPayload ?? string.Empty;
        }
    }

    public sealed class RewardedAdResult
    {
        public string RequestId { get; private set; }
        public RewardedAdState State { get; private set; }
        public string ReasonCode { get; private set; }
        public RewardedAdEvidence Evidence { get; private set; }

        public RewardedAdResult(
            string requestId,
            RewardedAdState state,
            string reasonCode,
            RewardedAdEvidence evidence
        )
        {
            RequestId = requestId ?? string.Empty;
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.AdFailed
            );
            Evidence = evidence;
        }
    }

    public sealed class RewardedAdVerificationResult
    {
        public VerificationState State { get; private set; }
        public string ReasonCode { get; private set; }
        public MonetizationEnvironment Environment { get; private set; }
        public string ProviderId { get; private set; }
        public string PlacementId { get; private set; }
        public string OpportunityId { get; private set; }
        public string RewardToken { get; private set; }

        public bool IsVerified { get { return State == VerificationState.Verified; } }

        internal RewardedAdVerificationResult(
            VerificationState state,
            string reasonCode,
            MonetizationEnvironment environment,
            string providerId,
            string placementId,
            string opportunityId,
            string rewardToken
        )
        {
            State = state;
            ReasonCode = MonetizationReasonCodes.NormalizeOr(
                reasonCode,
                MonetizationReasonCodes.VerificationRejected
            );
            Environment = environment;
            ProviderId = providerId ?? string.Empty;
            PlacementId = placementId ?? string.Empty;
            OpportunityId = opportunityId ?? string.Empty;
            RewardToken = rewardToken ?? string.Empty;
        }

        public static RewardedAdVerificationResult Verified(RewardedAdEvidence evidence)
        {
            return FromEvidence(
                VerificationState.Verified,
                MonetizationReasonCodes.None,
                evidence
            );
        }

        public static RewardedAdVerificationResult Rejected(
            RewardedAdEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Rejected,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerificationRejected
                    : reasonCode,
                evidence
            );
        }

        public static RewardedAdVerificationResult Retryable(
            RewardedAdEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Retryable,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerificationRetryable
                    : reasonCode,
                evidence
            );
        }

        public static RewardedAdVerificationResult Unavailable(
            RewardedAdEvidence evidence,
            string reasonCode = null
        )
        {
            return FromEvidence(
                VerificationState.Unavailable,
                string.IsNullOrEmpty(reasonCode)
                    ? MonetizationReasonCodes.VerifierUnavailable
                    : reasonCode,
                evidence
            );
        }

        static RewardedAdVerificationResult FromEvidence(
            VerificationState state,
            string reasonCode,
            RewardedAdEvidence evidence
        )
        {
            return new RewardedAdVerificationResult(
                state,
                reasonCode,
                evidence != null
                    ? evidence.Environment
                    : MonetizationEnvironment.Disabled,
                evidence != null ? evidence.ProviderId : string.Empty,
                evidence != null ? evidence.PlacementId : string.Empty,
                evidence != null ? evidence.OpportunityId : string.Empty,
                evidence != null ? evidence.RewardToken : string.Empty
            );
        }
    }

    public interface IIapAdapter
    {
        MonetizationEnvironment Environment { get; }
        CommercialAvailability GetAvailability(string offerId);
        PurchaseResult BeginPurchase(PurchaseRequest request);
        PurchaseResult ResolvePending(string requestId);
        bool ReleasePending(string requestId);
    }

    public interface IPurchaseVerifier
    {
        PurchaseVerificationResult Verify(PurchaseEvidence evidence);
    }

    public interface IRewardedAdAdapter
    {
        MonetizationEnvironment Environment { get; }
        CommercialAvailability GetAvailability(string placementId);
        RewardedAdResult Show(RewardedAdRequest request);
        RewardedAdResult ResolvePending(string requestId);
        bool ReleasePending(string requestId);
    }

    public interface IRewardedAdVerifier
    {
        RewardedAdVerificationResult Verify(RewardedAdEvidence evidence);
    }
}
