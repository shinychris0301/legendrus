using System;
using LegendsOfRus.CommercialCore.Monetization;

namespace LegendsOfRus.CommercialRuntime.Fulfillment
{
    /// <summary>
    /// Prepares verified, catalog-bound, copy-on-write fulfillment candidates.
    /// It never assigns live state, calls a persistence service, completes an
    /// entitlement, or persists a durable marker.
    /// </summary>
    public sealed class CommercialFulfillmentCoordinator
    {
        readonly CommercialRuntimeActivationDecision commercialActivation;
        readonly RuntimeEconomyActivationResult economyActivation;
        readonly VerifiedEntitlementGate entitlementGate;

        public bool IsReady
        {
            get { return string.IsNullOrEmpty(ReadinessFailure()); }
        }

        public CommercialFulfillmentCoordinator(
            CommercialRuntimeActivationDecision commercialActivation,
            RuntimeEconomyActivationResult economyActivation,
            VerifiedEntitlementGate entitlementGate
        )
        {
            this.commercialActivation = commercialActivation;
            this.economyActivation = economyActivation;
            this.entitlementGate = entitlementGate;
        }

        /// <summary>
        /// All current IAP identities deliberately stop at recipe resolution.
        /// Their legacy development grants are not present in the reviewed
        /// economy catalog and therefore cannot produce a candidate.
        /// </summary>
        public CommercialFulfillmentPreparationResult TryPreparePurchase(
            PlayerSaveData source,
            PurchaseRequest request,
            PurchaseResult purchase
        )
        {
            string readiness = ReadinessFailure(requireIap: true);
            if (!string.IsNullOrEmpty(readiness))
                return Rejected(readiness);

            if (!CommercialFulfillmentIdentity.TryFingerprint(source, out _))
                return Rejected(CommercialFulfillmentReasonCodes.SourceInvalid);

            string detailCode;
            if (!CommercialEntitlementRecipeCatalog.IsKnownPurchaseIdentity(
                request,
                out detailCode
            ))
            {
                return Rejected(
                    CommercialFulfillmentReasonCodes.UnknownIdentity,
                    detailCode
                );
            }

            // Do not invoke a receipt verifier when no approved economy recipe
            // exists. A verified purchase result alone is never an entitlement.
            return Rejected(
                CommercialFulfillmentReasonCodes.RecipeUnavailable
            );
        }

        public CommercialFulfillmentPreparationResult TryPrepareRewardedAd(
            PlayerSaveData source,
            RewardedAdRequest request,
            RewardedAdResult result
        )
        {
            string readiness = ReadinessFailure(requireRewardedAds: true);
            if (!string.IsNullOrEmpty(readiness))
                return Rejected(readiness);

            string sourceFingerprint;
            if (!CommercialFulfillmentIdentity.TryFingerprint(
                source,
                out sourceFingerprint
            ))
            {
                return Rejected(CommercialFulfillmentReasonCodes.SourceInvalid);
            }

            string actionId;
            string mappingDetail;
            if (!CommercialEntitlementRecipeCatalog.TryResolveRewardedAction(
                request,
                out actionId,
                out mappingDetail
            ))
            {
                return string.Equals(
                    mappingDetail,
                    CommercialFulfillmentReasonCodes.RecipeUnavailable,
                    StringComparison.Ordinal
                )
                    ? Rejected(CommercialFulfillmentReasonCodes.RecipeUnavailable)
                    : Rejected(
                        CommercialFulfillmentReasonCodes.UnknownIdentity,
                        mappingDetail
                    );
            }

            EntitlementEligibilityResult eligibility;
            try
            {
                eligibility = entitlementGate.TryReserveRewardedAd(
                    request,
                    result
                );
            }
            catch
            {
                return Rejected(CommercialFulfillmentReasonCodes.InternalError);
            }

            if (
                eligibility == null ||
                !eligibility.IsEligible ||
                eligibility.Reservation == null
            )
            {
                return Rejected(
                    CommercialFulfillmentReasonCodes.CandidateRejected,
                    eligibility != null
                        ? eligibility.ReasonCode
                        : MonetizationReasonCodes.ReservationInvalid
                );
            }

            EntitlementReservation reservation = eligibility.Reservation;
            CommercialFulfillmentPreparationResult prepared;
            try
            {
                prepared = PrepareRewardDouble(
                    source,
                    sourceFingerprint,
                    actionId,
                    reservation
                );
            }
            catch
            {
                prepared = Rejected(
                    CommercialFulfillmentReasonCodes.InternalError
                );
            }

            // Candidate-only Step 2 never holds or completes a reservation. A
            // future atomic store must introduce its own reviewed lifecycle.
            bool released;
            try
            {
                released = entitlementGate.Release(reservation);
            }
            catch
            {
                released = false;
            }

            return released
                ? prepared
                : Rejected(
                    CommercialFulfillmentReasonCodes.ReservationReleaseFailed
                );
        }

        CommercialFulfillmentPreparationResult PrepareRewardDouble(
            PlayerSaveData source,
            string sourceFingerprint,
            string actionId,
            EntitlementReservation reservation
        )
        {
            string marker;
            if (!CommercialFulfillmentIdentity.TryBuildMarker(
                reservation,
                out marker
            ))
            {
                return Rejected(
                    CommercialFulfillmentReasonCodes.CandidateRejected,
                    MonetizationReasonCodes.ReservationInvalid
                );
            }

            string transactionKey =
                CommercialFulfillmentIdentity.TransactionKey(marker);

            // A fresh detached coordinator prevents an abandoned candidate from
            // retaining a process-local staged key and blocking a safe retry.
            var economy = new DetachedRuntimeEconomyCoordinator(
                economyActivation
            );
            DetachedRuntimeEconomyResult staged = economy.TryDoubleLastReward(
                source,
                transactionKey
            );
            if (
                staged == null ||
                !staged.Applied ||
                !staged.Changed ||
                staged.Candidate == null
            )
            {
                return Rejected(
                    CommercialFulfillmentReasonCodes.CandidateRejected,
                    staged != null ? staged.Code : string.Empty
                );
            }

            PlayerSaveData detached =
                CommercialFulfillmentIdentity.CloneSave(staged.Candidate);
            if (detached == null)
                return Rejected(CommercialFulfillmentReasonCodes.SourceInvalid);

            return CommercialFulfillmentPreparationResult.Prepared(
                new CommercialFulfillmentCandidate(
                    actionId,
                    marker,
                    transactionKey,
                    sourceFingerprint,
                    staged.Changed,
                    detached,
                    staged.Snapshot
                )
            );
        }

        string ReadinessFailure(
            bool requireIap = false,
            bool requireRewardedAds = false
        )
        {
            if (
                commercialActivation == null ||
                !commercialActivation.IsRuntimeActivated ||
                commercialActivation.Phase != CommercialRuntimePhase.Active ||
                (requireIap && !commercialActivation.IapApproved) ||
                (
                    requireRewardedAds &&
                    !commercialActivation.RewardedAdsApproved
                )
            )
            {
                return CommercialFulfillmentReasonCodes.RuntimeInactive;
            }
            if (
                economyActivation == null ||
                !economyActivation.IsRuntimeActivated
            )
            {
                return CommercialFulfillmentReasonCodes.EconomyInactive;
            }
            if (!CommercialEntitlementRecipeCatalog.IsBoundTo(economyActivation))
            {
                return CommercialFulfillmentReasonCodes.CatalogBindingMismatch;
            }
            if (entitlementGate == null || !entitlementGate.IsOperational)
            {
                return CommercialFulfillmentReasonCodes.EntitlementGateUnavailable;
            }
            if (!EnvironmentMatches(commercialActivation, entitlementGate))
            {
                return CommercialFulfillmentReasonCodes.EntitlementGateUnavailable;
            }
            return CommercialFulfillmentReasonCodes.None;
        }

        static bool EnvironmentMatches(
            CommercialRuntimeActivationDecision activation,
            VerifiedEntitlementGate gate
        )
        {
            if (activation == null || gate == null)
                return false;

            if (string.Equals(
                activation.Environment,
                "production",
                StringComparison.Ordinal
            ))
            {
                return gate.ExpectedEnvironment == MonetizationEnvironment.Production;
            }

            if (
                string.Equals(
                    activation.Environment,
                    "development",
                    StringComparison.Ordinal
                ) ||
                string.Equals(
                    activation.Environment,
                    "development_sandbox",
                    StringComparison.Ordinal
                )
            )
            {
                return gate.ExpectedEnvironment ==
                    MonetizationEnvironment.DevelopmentSandbox;
            }

            return false;
        }

        static CommercialFulfillmentPreparationResult Rejected(
            string reasonCode,
            string detailCode = null
        )
        {
            return CommercialFulfillmentPreparationResult.Rejected(
                reasonCode,
                detailCode
            );
        }
    }
}
