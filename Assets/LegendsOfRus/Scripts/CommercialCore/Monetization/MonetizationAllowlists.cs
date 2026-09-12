using System;

namespace LegendsOfRus.CommercialCore.Monetization
{
    public sealed class PurchaseOfferDefinition
    {
        public string OfferId { get; private set; }
        public string ProductId { get; private set; }

        public PurchaseOfferDefinition(string offerId, string productId)
        {
            OfferId = offerId ?? string.Empty;
            ProductId = productId ?? string.Empty;
        }

        internal PurchaseOfferDefinition DetachedCopy()
        {
            return new PurchaseOfferDefinition(OfferId, ProductId);
        }
    }

    public sealed class RewardedPlacementDefinition
    {
        public string PlacementId { get; private set; }

        public RewardedPlacementDefinition(string placementId)
        {
            PlacementId = placementId ?? string.Empty;
        }

        internal RewardedPlacementDefinition DetachedCopy()
        {
            return new RewardedPlacementDefinition(PlacementId);
        }
    }

    /// <summary>
    /// Closed identity allowlists for the detached v1.16 adapter foundation.
    /// They deliberately contain no price or economy mutation payload. Those
    /// remain behind the v1.15 activation boundary and cannot be supplied by an
    /// adapter or remote-config payload.
    /// </summary>
    public static class MonetizationAllowlists
    {
        public const string StarterOfferId = "starter";
        public const string HeroOfferId = "hero";
        public const string FirebirdOfferId = "firebird";
        public const string EnergyOfferId = "energy";

        public const string StarterProductId = "legends_of_rus.starter_pack";
        public const string HeroProductId = "legends_of_rus.hero_pack";
        public const string FirebirdProductId = "legends_of_rus.firebird_pack";
        public const string EnergyProductId = "legends_of_rus.energy_refill";

        public const string RewardedEnergyPlacementId = "out_of_energy_energy_1";
        public const string RewardedDoublePlacementId = "victory_double_reward";

        static readonly PurchaseOfferDefinition[] PurchaseOffers =
        {
            new PurchaseOfferDefinition(StarterOfferId, StarterProductId),
            new PurchaseOfferDefinition(HeroOfferId, HeroProductId),
            new PurchaseOfferDefinition(FirebirdOfferId, FirebirdProductId),
            new PurchaseOfferDefinition(EnergyOfferId, EnergyProductId)
        };

        static readonly RewardedPlacementDefinition[] RewardedPlacements =
        {
            new RewardedPlacementDefinition(RewardedEnergyPlacementId),
            new RewardedPlacementDefinition(RewardedDoublePlacementId)
        };

        public static bool TryGetPurchaseOffer(
            string offerId,
            out PurchaseOfferDefinition definition
        )
        {
            for (int i = 0; i < PurchaseOffers.Length; i++)
            {
                if (string.Equals(
                    PurchaseOffers[i].OfferId,
                    offerId,
                    StringComparison.Ordinal
                ))
                {
                    definition = PurchaseOffers[i].DetachedCopy();
                    return true;
                }
            }

            definition = null;
            return false;
        }

        public static bool TryGetPurchaseOfferByProduct(
            string productId,
            out PurchaseOfferDefinition definition
        )
        {
            for (int i = 0; i < PurchaseOffers.Length; i++)
            {
                if (string.Equals(
                    PurchaseOffers[i].ProductId,
                    productId,
                    StringComparison.Ordinal
                ))
                {
                    definition = PurchaseOffers[i].DetachedCopy();
                    return true;
                }
            }

            definition = null;
            return false;
        }

        public static bool TryGetRewardedPlacement(
            string placementId,
            out RewardedPlacementDefinition definition
        )
        {
            for (int i = 0; i < RewardedPlacements.Length; i++)
            {
                if (string.Equals(
                    RewardedPlacements[i].PlacementId,
                    placementId,
                    StringComparison.Ordinal
                ))
                {
                    definition = RewardedPlacements[i].DetachedCopy();
                    return true;
                }
            }

            definition = null;
            return false;
        }
    }
}
