namespace LegendsOfRus.CommercialCore.Monetization
{
    /// <summary>
    /// Safe release fallback when no approved store integration is installed.
    /// </summary>
    public sealed class DefaultUnavailableIapAdapter : IIapAdapter
    {
        public MonetizationEnvironment Environment
        {
            get { return MonetizationEnvironment.Disabled; }
        }

        public CommercialAvailability GetAvailability(string offerId)
        {
            return MonetizationAllowlists.TryGetPurchaseOffer(offerId, out _)
                ? new CommercialAvailability(
                    CommercialAvailabilityState.Disabled,
                    MonetizationReasonCodes.AdapterDisabled
                )
                : CommercialAvailability.Unavailable(
                    MonetizationReasonCodes.UnknownOffer
                );
        }

        public PurchaseResult BeginPurchase(PurchaseRequest request)
        {
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.OfferId) ||
                !MonetizationIdentifiers.IsStable(request.ProductId)
            )
            {
                return PurchaseFailure(
                    request != null ? request.RequestId : string.Empty,
                    MonetizationReasonCodes.InvalidRequest
                );
            }
            if (!MonetizationAllowlists.TryGetPurchaseOffer(
                request.OfferId,
                out PurchaseOfferDefinition offer
            ))
            {
                return PurchaseFailure(
                    request.RequestId,
                    MonetizationReasonCodes.UnknownOffer
                );
            }
            if (!string.Equals(
                offer.ProductId,
                request.ProductId,
                System.StringComparison.Ordinal
            ))
            {
                return PurchaseFailure(
                    request.RequestId,
                    MonetizationReasonCodes.ProductMismatch
                );
            }

            return new PurchaseResult(
                request.RequestId,
                PurchaseState.Unavailable,
                MonetizationReasonCodes.AdapterDisabled,
                null
            );
        }

        public PurchaseResult ResolvePending(string requestId)
        {
            return new PurchaseResult(
                requestId,
                PurchaseState.Unavailable,
                MonetizationReasonCodes.AdapterDisabled,
                null
            );
        }

        public bool ReleasePending(string requestId)
        {
            return false;
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

    public sealed class DefaultUnavailablePurchaseVerifier : IPurchaseVerifier
    {
        public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
        {
            return PurchaseVerificationResult.Unavailable(
                evidence,
                MonetizationReasonCodes.VerifierUnavailable
            );
        }
    }

    /// <summary>
    /// Safe release fallback. A caller can keep all non-ad navigation available
    /// while presenting the returned reason as a non-blocking status.
    /// </summary>
    public sealed class DefaultUnavailableRewardedAdAdapter : IRewardedAdAdapter
    {
        public MonetizationEnvironment Environment
        {
            get { return MonetizationEnvironment.Disabled; }
        }

        public CommercialAvailability GetAvailability(string placementId)
        {
            return MonetizationAllowlists.TryGetRewardedPlacement(placementId, out _)
                ? new CommercialAvailability(
                    CommercialAvailabilityState.Disabled,
                    MonetizationReasonCodes.AdapterDisabled
                )
                : CommercialAvailability.Unavailable(
                    MonetizationReasonCodes.UnknownPlacement
                );
        }

        public RewardedAdResult Show(RewardedAdRequest request)
        {
            if (
                request == null ||
                !MonetizationIdentifiers.IsStable(request.RequestId) ||
                !MonetizationIdentifiers.IsStable(request.PlacementId) ||
                !MonetizationIdentifiers.IsStable(request.OpportunityId)
            )
            {
                return AdFailure(
                    request != null ? request.RequestId : string.Empty,
                    MonetizationReasonCodes.InvalidRequest
                );
            }
            if (!MonetizationAllowlists.TryGetRewardedPlacement(
                request.PlacementId,
                out _
            ))
            {
                return AdFailure(
                    request.RequestId,
                    MonetizationReasonCodes.UnknownPlacement
                );
            }

            return new RewardedAdResult(
                request.RequestId,
                RewardedAdState.Unavailable,
                MonetizationReasonCodes.AdapterDisabled,
                null
            );
        }

        public RewardedAdResult ResolvePending(string requestId)
        {
            return new RewardedAdResult(
                requestId,
                RewardedAdState.Unavailable,
                MonetizationReasonCodes.AdapterDisabled,
                null
            );
        }

        public bool ReleasePending(string requestId)
        {
            return false;
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

    public sealed class DefaultUnavailableRewardedAdVerifier : IRewardedAdVerifier
    {
        public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
        {
            return RewardedAdVerificationResult.Unavailable(
                evidence,
                MonetizationReasonCodes.VerifierUnavailable
            );
        }
    }
}
