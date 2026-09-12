using System;
using LegendsOfRus.CommercialCore.Monetization;

namespace LegendsOfRus.CommercialRuntime.Fulfillment
{
    /// <summary>
    /// Closed bridge from verified commercial identity to an already-reviewed
    /// economy operation. The v1.15 catalog defines reward doubling, but it does
    /// not define any IAP bundle or rewarded-energy recipe. Missing recipes must
    /// remain unavailable; legacy mock amounts are not commercial authority.
    /// </summary>
    public static class CommercialEntitlementRecipeCatalog
    {
        public const string RewardDoubleActionId =
            "economy.reward_double.v1";

        public static bool IsBoundTo(
            RuntimeEconomyActivationResult economyActivation
        )
        {
            return
                economyActivation != null &&
                economyActivation.IsRuntimeActivated &&
                economyActivation.Policy != null &&
                economyActivation.Policy.IsCatalogConfigured &&
                string.Equals(
                    economyActivation.Policy.PolicyId,
                    RuntimeEconomyActivationGate.CatalogId,
                    StringComparison.Ordinal
                ) &&
                string.Equals(
                    economyActivation.Policy.CatalogSourceSha256,
                    RuntimeEconomyActivationGate.CatalogSourceSha256,
                    StringComparison.Ordinal
                ) &&
                string.Equals(
                    economyActivation.ComputedSourceSha256,
                    RuntimeEconomyActivationGate.CatalogSourceSha256,
                    StringComparison.Ordinal
                );
        }

        public static bool IsKnownPurchaseIdentity(
            PurchaseRequest request,
            out string detailCode
        )
        {
            detailCode = MonetizationReasonCodes.InvalidRequest;
            if (
                request == null ||
                string.IsNullOrWhiteSpace(request.RequestId) ||
                string.IsNullOrWhiteSpace(request.OfferId) ||
                string.IsNullOrWhiteSpace(request.ProductId)
            )
            {
                return false;
            }

            PurchaseOfferDefinition offer;
            if (!MonetizationAllowlists.TryGetPurchaseOffer(
                request.OfferId,
                out offer
            ))
            {
                detailCode = MonetizationReasonCodes.UnknownOffer;
                return false;
            }

            if (!string.Equals(
                request.ProductId,
                offer.ProductId,
                StringComparison.Ordinal
            ))
            {
                detailCode = MonetizationReasonCodes.ProductMismatch;
                return false;
            }

            detailCode = MonetizationReasonCodes.None;
            return true;
        }

        public static bool TryResolveRewardedAction(
            RewardedAdRequest request,
            out string actionId,
            out string detailCode
        )
        {
            actionId = string.Empty;
            detailCode = MonetizationReasonCodes.InvalidRequest;
            if (
                request == null ||
                string.IsNullOrWhiteSpace(request.RequestId) ||
                string.IsNullOrWhiteSpace(request.PlacementId) ||
                string.IsNullOrWhiteSpace(request.OpportunityId)
            )
            {
                return false;
            }

            RewardedPlacementDefinition placement;
            if (!MonetizationAllowlists.TryGetRewardedPlacement(
                request.PlacementId,
                out placement
            ))
            {
                detailCode = MonetizationReasonCodes.UnknownPlacement;
                return false;
            }

            if (string.Equals(
                placement.PlacementId,
                MonetizationAllowlists.RewardedDoublePlacementId,
                StringComparison.Ordinal
            ))
            {
                actionId = RewardDoubleActionId;
                detailCode = MonetizationReasonCodes.None;
                return true;
            }

            // out_of_energy_energy_1 is a valid placement identity, but the
            // economy catalog contains no approved ad-energy recipe.
            detailCode = CommercialFulfillmentReasonCodes.RecipeUnavailable;
            return false;
        }
    }
}
