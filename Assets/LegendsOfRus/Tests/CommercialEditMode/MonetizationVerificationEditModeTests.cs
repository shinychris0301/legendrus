using System;
using LegendsOfRus.CommercialCore.Monetization;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Commercial
{
    public sealed class MonetizationVerificationEditModeTests
    {
        [Test]
        public void ClosedAllowlists_ResolveOnlyReviewedOffersAndPlacements()
        {
            Assert.That(
                MonetizationAllowlists.TryGetPurchaseOffer(
                    MonetizationAllowlists.StarterOfferId,
                    out PurchaseOfferDefinition starter
                ),
                Is.True
            );
            Assert.That(starter.ProductId, Is.EqualTo(
                MonetizationAllowlists.StarterProductId
            ));
            Assert.That(
                MonetizationAllowlists.TryGetPurchaseOffer("unknown", out _),
                Is.False
            );
            Assert.That(
                MonetizationAllowlists.TryGetPurchaseOfferByProduct(
                    MonetizationAllowlists.EnergyProductId,
                    out PurchaseOfferDefinition energy
                ),
                Is.True
            );
            Assert.That(energy.OfferId, Is.EqualTo(MonetizationAllowlists.EnergyOfferId));
            Assert.That(
                MonetizationAllowlists.TryGetRewardedPlacement(
                    MonetizationAllowlists.RewardedEnergyPlacementId,
                    out _
                ),
                Is.True
            );
            Assert.That(
                MonetizationAllowlists.TryGetRewardedPlacement("unknown", out _),
                Is.False
            );
        }

        [Test]
        public void DefaultFallbacksAndUnboundGate_FailClosedWithoutReservations()
        {
            var iap = new DefaultUnavailableIapAdapter();
            var purchaseVerifier = new DefaultUnavailablePurchaseVerifier();
            var ad = new DefaultUnavailableRewardedAdAdapter();
            var adVerifier = new DefaultUnavailableRewardedAdVerifier();
            PurchaseRequest purchaseRequest = PurchaseRequestOne();
            RewardedAdRequest adRequest = AdRequestOne();

            PurchaseResult purchase = iap.BeginPurchase(purchaseRequest);
            RewardedAdResult rewarded = ad.Show(adRequest);

            Assert.That(iap.Environment, Is.EqualTo(MonetizationEnvironment.Disabled));
            Assert.That(iap.GetAvailability(purchaseRequest.OfferId).IsReady, Is.False);
            Assert.That(purchase.State, Is.EqualTo(PurchaseState.Unavailable));
            Assert.That(purchase.Evidence, Is.Null);
            Assert.That(
                purchaseVerifier.Verify(null).State,
                Is.EqualTo(VerificationState.Unavailable)
            );
            Assert.That(ad.Environment, Is.EqualTo(MonetizationEnvironment.Disabled));
            Assert.That(ad.GetAvailability(adRequest.PlacementId).IsReady, Is.False);
            Assert.That(rewarded.State, Is.EqualTo(RewardedAdState.Unavailable));
            Assert.That(rewarded.Evidence, Is.Null);
            Assert.That(
                adVerifier.Verify(null).State,
                Is.EqualTo(VerificationState.Unavailable)
            );

            PurchaseFlow purchaseFlow = PurchasedEvidence(
                "unbound_purchase",
                "unbound_transaction"
            );
            RewardedFlow rewardedFlow = VerifiedRewardedFlow(
                "unbound_ad",
                "unbound_reward_token"
            );
            var unboundGate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                null,
                null
            );
            EntitlementEligibilityResult unboundPurchase =
                unboundGate.TryReservePurchase(
                    purchaseFlow.Request,
                    purchaseFlow.Purchase
                );
            EntitlementEligibilityResult unboundRewarded =
                unboundGate.TryReserveRewardedAd(
                    rewardedFlow.Request,
                    rewardedFlow.Result
                );

            Assert.That(unboundGate.IsOperational, Is.False);
            Assert.That(unboundPurchase.IsEligible, Is.False);
            Assert.That(unboundRewarded.IsEligible, Is.False);
            Assert.That(
                unboundPurchase.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.VerifierUnavailable)
            );
            Assert.That(
                unboundRewarded.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.VerifierUnavailable)
            );
            Assert.That(unboundGate.ReservedCount, Is.Zero);
            Assert.That(unboundGate.CompletedCount, Is.Zero);
        }

        [Test]
        public void DevelopmentAdapters_DefaultUnavailableAndRequireExplicitScripts()
        {
            var iap = new ScriptedDevelopmentIapAdapter();
            var ad = new ScriptedDevelopmentRewardedAdAdapter();

            PurchaseResult purchase = iap.BeginPurchase(PurchaseRequestOne());
            RewardedAdResult rewarded = ad.Show(AdRequestOne());

            Assert.That(iap.GetAvailability(MonetizationAllowlists.HeroOfferId).IsReady, Is.False);
            Assert.That(purchase.State, Is.EqualTo(PurchaseState.Unavailable));
            Assert.That(purchase.ReasonCode, Is.EqualTo(MonetizationReasonCodes.AdapterUnavailable));
            Assert.That(purchase.Evidence, Is.Null);
            Assert.That(ad.GetAvailability(MonetizationAllowlists.RewardedDoublePlacementId).IsReady, Is.False);
            Assert.That(rewarded.State, Is.EqualTo(RewardedAdState.Unavailable));
            Assert.That(rewarded.ReasonCode, Is.EqualTo(MonetizationReasonCodes.AdapterUnavailable));
            Assert.That(rewarded.Evidence, Is.Null);
        }

        [Test]
        public void ScriptedPurchase_CreatesUnverifiedEvidenceBeforeGateEligibility()
        {
            PurchaseFlow flow = VerifiedPurchaseFlow("request_1", "transaction_1");
            var verifier = new ScriptedDevelopmentPurchaseVerifier();
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            PurchaseVerificationResult verification = verifier.Verify(
                flow.Purchase.Evidence
            );

            Assert.That(flow.Purchase.State, Is.EqualTo(PurchaseState.PurchasedUnverified));
            Assert.That(flow.Purchase.Evidence, Is.Not.Null);
            Assert.That(flow.Purchase.Evidence.ReceiptPayload, Is.EqualTo(
                DevelopmentMonetizationProofs.PurchaseReceipt(
                    flow.Request.ProductId,
                    "transaction_1"
                )
            ));
            Assert.That(verification.IsVerified, Is.True);
        }

        [Test]
        public void PurchaseVerifier_RejectsMalformedDevelopmentReceipt()
        {
            var verifier = new ScriptedDevelopmentPurchaseVerifier();
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            var evidence = new PurchaseEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                MonetizationAllowlists.StarterProductId,
                "transaction_1",
                "forged-receipt"
            );

            PurchaseVerificationResult result = verifier.Verify(evidence);

            Assert.That(result.IsVerified, Is.False);
            Assert.That(result.State, Is.EqualTo(VerificationState.Rejected));
            Assert.That(result.ReasonCode, Is.EqualTo(MonetizationReasonCodes.EvidenceMalformed));
        }

        [TestCase(PurchaseState.Pending)]
        [TestCase(PurchaseState.Cancelled)]
        [TestCase(PurchaseState.Declined)]
        [TestCase(PurchaseState.Unavailable)]
        [TestCase(PurchaseState.Failed)]
        public void PurchaseNonTerminalOrFailureStates_NeverReserveEntitlement(
            PurchaseState state
        )
        {
            PurchaseRequest request = PurchaseRequestOne();
            var purchase = new PurchaseResult(
                request.RequestId,
                state,
                string.Empty,
                null
            );
            var verifier = new ThrowingPurchaseVerifier();
            var gate = DevelopmentGate(purchaseVerifier: verifier);

            EntitlementEligibilityResult result = gate.TryReservePurchase(
                request,
                purchase
            );

            Assert.That(result.IsEligible, Is.False);
            Assert.That(result.Reservation, Is.Null);
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(gate.CompletedCount, Is.Zero);
            Assert.That(verifier.Calls, Is.Zero);
        }

        [TestCase(VerificationState.Rejected)]
        [TestCase(VerificationState.Retryable)]
        [TestCase(VerificationState.Unavailable)]
        public void UnverifiedPurchaseEvidence_NeverReservesEntitlement(
            VerificationState state
        )
        {
            PurchaseFlow flow = PurchasedEvidence("request_1", "transaction_1");
            var verifier = new FixedPurchaseVerifier(state);
            var gate = DevelopmentGate(purchaseVerifier: verifier);

            EntitlementEligibilityResult result = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );

            Assert.That(result.IsEligible, Is.False);
            Assert.That(result.Reservation, Is.Null);
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(verifier.Calls, Is.EqualTo(1));
        }

        [Test]
        public void VerifiedPurchase_ReservesStableIdentityWithoutRewardRecipe()
        {
            PurchaseFlow flow = VerifiedPurchaseFlow("request_1", "transaction_1");
            var verifier = new FixedPurchaseVerifier(VerificationState.Verified);
            var gate = DevelopmentGate(purchaseVerifier: verifier);

            EntitlementEligibilityResult result = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );

            Assert.That(result.IsEligible, Is.True, result.ReasonCode);
            Assert.That(result.Reservation, Is.Not.Null);
            Assert.That(result.Reservation.FulfillmentKey, Is.EqualTo(
                "iap|development_sandbox|transaction_1"
            ));
            Assert.That(result.Reservation.Source, Is.EqualTo(EntitlementSource.InAppPurchase));
            Assert.That(result.Reservation.ProductId, Is.EqualTo(flow.Request.ProductId));
            Assert.That(result.Reservation.TransactionId, Is.EqualTo("transaction_1"));
            Assert.That(result.Reservation.PlacementId, Is.Empty);
            Assert.That(result.Reservation.RewardToken, Is.Empty);
            Assert.That(gate.ReservedCount, Is.EqualTo(1));
            Assert.That(verifier.Calls, Is.EqualTo(1));

            var unbound = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                null,
                new FixedRewardedVerifier(VerificationState.Verified)
            );
            EntitlementEligibilityResult withoutVerifier = unbound
                .TryReservePurchase(flow.Request, flow.Purchase);
            Assert.That(unbound.IsOperational, Is.False);
            Assert.That(withoutVerifier.IsEligible, Is.False);
            Assert.That(
                withoutVerifier.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.VerifierUnavailable)
            );
        }

        [Test]
        public void PurchaseIdentityMismatches_FailBeforeReservation()
        {
            PurchaseFlow flow = VerifiedPurchaseFlow("request_1", "transaction_1");

            var wrongEnvironment = new AlteredPurchaseVerifier(
                environment: MonetizationEnvironment.Production
            );
            var wrongProvider = new AlteredPurchaseVerifier(
                providerId: "other_provider"
            );
            var wrongTransaction = new AlteredPurchaseVerifier(
                transactionId: "transaction_2"
            );

            AssertPurchaseRejected(flow, wrongEnvironment, MonetizationReasonCodes.EnvironmentMismatch);
            AssertPurchaseRejected(flow, wrongProvider, MonetizationReasonCodes.ProviderMismatch);
            AssertPurchaseRejected(flow, wrongTransaction, MonetizationReasonCodes.EvidenceMalformed);
            AssertPurchaseRejected(
                flow,
                new ThrowingPurchaseVerifier(),
                MonetizationReasonCodes.VerifierUnavailable
            );

            var malformedEvidence = new PurchaseEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                flow.Request.ProductId,
                "transaction_1",
                string.Empty
            );
            var malformedResult = new PurchaseResult(
                flow.Request.RequestId,
                PurchaseState.PurchasedUnverified,
                MonetizationReasonCodes.None,
                malformedEvidence
            );
            var untouchedVerifier = new ThrowingPurchaseVerifier();
            EntitlementEligibilityResult malformed = DevelopmentGate(
                purchaseVerifier: untouchedVerifier
            )
                .TryReservePurchase(
                    flow.Request,
                    malformedResult
                );
            Assert.That(
                malformed.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.EvidenceMalformed)
            );
            Assert.That(untouchedVerifier.Calls, Is.Zero);
        }

        [Test]
        public void CompletionAndDurableSeed_BlockPurchaseReplay()
        {
            PurchaseFlow flow = VerifiedPurchaseFlow("request_1", "transaction_1");
            var verifier = new FixedPurchaseVerifier(VerificationState.Verified);
            var gate = DevelopmentGate(purchaseVerifier: verifier);
            EntitlementEligibilityResult first = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );

            Assert.That(gate.Complete(first.Reservation), Is.True);
            Assert.That(gate.Complete(first.Reservation), Is.False);
            EntitlementEligibilityResult replay = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );

            Assert.That(replay.IsEligible, Is.False);
            Assert.That(replay.ReasonCode, Is.EqualTo(MonetizationReasonCodes.DuplicateEntitlement));
            Assert.That(gate.CompletedCount, Is.EqualTo(1));
            CollectionAssert.AreEqual(
                new[] { "iap|development_sandbox|transaction_1" },
                gate.ExportCompletedFulfillmentKeys()
            );

            var restored = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                verifier,
                new FixedRewardedVerifier(VerificationState.Verified),
                gate.ExportCompletedFulfillmentKeys()
            );
            Assert.That(
                restored.TryReservePurchase(
                    flow.Request,
                    flow.Purchase
                ).ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.DuplicateEntitlement)
            );

            var throwingVerifier = new ThrowingPurchaseVerifier();
            var invalidHistory = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                throwingVerifier,
                new FixedRewardedVerifier(VerificationState.Verified),
                new[] { "not-a-fulfillment-key" }
            );
            EntitlementEligibilityResult invalidHistoryResult =
                invalidHistory.TryReservePurchase(
                    flow.Request,
                    flow.Purchase
                );
            Assert.That(invalidHistory.IsOperational, Is.False);
            Assert.That(
                invalidHistoryResult.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.DurableHistoryInvalid)
            );
            Assert.That(throwingVerifier.Calls, Is.Zero);

            var invalidEnvironment = new VerifiedEntitlementGate(
                MonetizationEnvironment.Disabled,
                throwingVerifier,
                new FixedRewardedVerifier(VerificationState.Verified)
            );
            Assert.That(invalidEnvironment.IsOperational, Is.False);
            Assert.That(
                invalidEnvironment.TryReservePurchase(
                    flow.Request,
                    flow.Purchase
                ).ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.EnvironmentMismatch)
            );
            Assert.That(throwingVerifier.Calls, Is.Zero);
        }

        [Test]
        public void Release_RemovesReservationAndAllowsSafeRetry()
        {
            PurchaseFlow flow = VerifiedPurchaseFlow("request_1", "transaction_1");
            var verifier = new FixedPurchaseVerifier(VerificationState.Verified);
            var gate = DevelopmentGate(purchaseVerifier: verifier);
            EntitlementEligibilityResult first = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );

            Assert.That(gate.Release(first.Reservation), Is.True);
            Assert.That(gate.Release(first.Reservation), Is.False);
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(gate.CompletedCount, Is.Zero);
            Assert.That(
                gate.TryReservePurchase(
                    flow.Request,
                    flow.Purchase
                ).IsEligible,
                Is.True
            );
        }

        [Test]
        public void PendingPurchase_HoldsOneChannelAndReleaseRestoresAvailability()
        {
            var adapter = new ScriptedDevelopmentIapAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentPurchaseScript.Pending());
            PurchaseResult pending = adapter.BeginPurchase(PurchaseRequestOne());

            PurchaseResult duplicate = adapter.BeginPurchase(PurchaseRequestOne());
            var other = new PurchaseRequest(
                "request_2",
                MonetizationAllowlists.HeroOfferId,
                MonetizationAllowlists.HeroProductId
            );
            PurchaseResult busy = adapter.BeginPurchase(other);

            Assert.That(pending.State, Is.EqualTo(PurchaseState.Pending));
            Assert.That(adapter.IsBusy, Is.True);
            Assert.That(duplicate.ReasonCode, Is.EqualTo(MonetizationReasonCodes.DuplicateRequest));
            Assert.That(busy.ReasonCode, Is.EqualTo(MonetizationReasonCodes.Busy));
            Assert.That(adapter.ReleasePending("request_1"), Is.True);
            Assert.That(adapter.ReleasePending("request_1"), Is.False);
            Assert.That(adapter.ReleasePending(string.Empty), Is.False);
            Assert.That(adapter.IsBusy, Is.False);
            Assert.That(adapter.GetAvailability(MonetizationAllowlists.HeroOfferId).IsReady, Is.True);

            var resolvable = new ScriptedDevelopmentIapAdapter();
            resolvable.SetAvailable(true);
            resolvable.Enqueue(DevelopmentPurchaseScript.Pending());
            Assert.That(
                resolvable.BeginPurchase(PurchaseRequestOne()).State,
                Is.EqualTo(PurchaseState.Pending)
            );
            PurchaseResult stillPending = resolvable.ResolvePending("request_1");
            Assert.That(stillPending.State, Is.EqualTo(PurchaseState.Pending));
            Assert.That(
                stillPending.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.DevelopmentScriptMissing)
            );
            Assert.That(resolvable.IsBusy, Is.True);
            resolvable.Enqueue(DevelopmentPurchaseScript.Purchased("transaction_1"));
            PurchaseResult resolved = resolvable.ResolvePending("request_1");
            Assert.That(resolved.State, Is.EqualTo(PurchaseState.PurchasedUnverified));
            Assert.That(resolved.Evidence, Is.Not.Null);
            Assert.That(resolvable.IsBusy, Is.False);
        }

        [Test]
        public void ScriptedRewardedCompletion_CreatesUnverifiedProofThenVerifies()
        {
            RewardedFlow flow = VerifiedRewardedFlow("ad_request_1", "reward_token_1");
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            RewardedAdVerificationResult verification = verifier.Verify(
                flow.Result.Evidence
            );

            Assert.That(flow.Result.State, Is.EqualTo(RewardedAdState.CompletedUnverified));
            Assert.That(flow.Result.Evidence, Is.Not.Null);
            Assert.That(flow.Result.Evidence.ProofPayload, Is.EqualTo(
                DevelopmentMonetizationProofs.RewardedProof(
                    flow.Request.PlacementId,
                    flow.Request.OpportunityId,
                    "reward_token_1"
                )
            ));
            Assert.That(verification.IsVerified, Is.True);
        }

        [TestCase(RewardedAdState.Pending)]
        [TestCase(RewardedAdState.ClosedEarly)]
        [TestCase(RewardedAdState.Skipped)]
        [TestCase(RewardedAdState.Cancelled)]
        [TestCase(RewardedAdState.Unavailable)]
        [TestCase(RewardedAdState.Failed)]
        public void RewardedAdNonCompletionStates_NeverReserveEntitlement(
            RewardedAdState state
        )
        {
            RewardedAdRequest request = AdRequestOne();
            var result = new RewardedAdResult(
                request.RequestId,
                state,
                string.Empty,
                null
            );
            var verifier = new ThrowingRewardedVerifier();
            var gate = DevelopmentGate(rewardedVerifier: verifier);

            EntitlementEligibilityResult eligibility = gate.TryReserveRewardedAd(
                request,
                result
            );

            Assert.That(eligibility.IsEligible, Is.False);
            Assert.That(eligibility.Reservation, Is.Null);
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(verifier.Calls, Is.Zero);
        }

        [Test]
        public void RewardedVerifier_RejectsForgedProof()
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            RewardedAdRequest request = AdRequestOne();
            var evidence = new RewardedAdEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                request.PlacementId,
                request.OpportunityId,
                "reward_token_1",
                "forged-proof"
            );

            RewardedAdVerificationResult result = verifier.Verify(evidence);

            Assert.That(result.IsVerified, Is.False);
            Assert.That(result.State, Is.EqualTo(VerificationState.Rejected));
            Assert.That(result.ReasonCode, Is.EqualTo(MonetizationReasonCodes.EvidenceMalformed));
        }

        [Test]
        public void VerifiedRewardedAd_ReservesOnceAndRejectsReplay()
        {
            RewardedFlow flow = VerifiedRewardedFlow("ad_request_1", "reward_token_1");
            var verifier = new FixedRewardedVerifier(VerificationState.Verified);
            var gate = DevelopmentGate(rewardedVerifier: verifier);

            EntitlementEligibilityResult first = gate.TryReserveRewardedAd(
                flow.Request,
                flow.Result
            );

            Assert.That(first.IsEligible, Is.True, first.ReasonCode);
            Assert.That(first.Reservation.FulfillmentKey, Is.EqualTo(
                "ad|development_sandbox|reward_token_1"
            ));
            Assert.That(first.Reservation.Source, Is.EqualTo(EntitlementSource.RewardedAd));
            Assert.That(first.Reservation.PlacementId, Is.EqualTo(flow.Request.PlacementId));
            Assert.That(first.Reservation.OpportunityId, Is.EqualTo(flow.Request.OpportunityId));
            Assert.That(first.Reservation.RewardToken, Is.EqualTo("reward_token_1"));
            Assert.That(gate.Complete(first.Reservation), Is.True);

            EntitlementEligibilityResult replay = gate.TryReserveRewardedAd(
                flow.Request,
                flow.Result
            );
            Assert.That(replay.IsEligible, Is.False);
            Assert.That(replay.ReasonCode, Is.EqualTo(MonetizationReasonCodes.DuplicateEntitlement));

            var unbound = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new FixedPurchaseVerifier(VerificationState.Verified),
                null
            );
            EntitlementEligibilityResult withoutVerifier = unbound
                .TryReserveRewardedAd(flow.Request, flow.Result);
            Assert.That(unbound.IsOperational, Is.False);
            Assert.That(withoutVerifier.IsEligible, Is.False);
            Assert.That(
                withoutVerifier.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.VerifierUnavailable)
            );
        }

        [Test]
        public void RewardedIdentityMismatches_FailBeforeReservation()
        {
            RewardedFlow flow = VerifiedRewardedFlow("ad_request_1", "reward_token_1");

            var wrongPlacement = new AlteredRewardedVerifier(
                placementId: MonetizationAllowlists.RewardedDoublePlacementId
            );
            var wrongOpportunity = new AlteredRewardedVerifier(
                opportunityId: "other_opportunity"
            );
            var wrongToken = new AlteredRewardedVerifier(
                rewardToken: "other_token"
            );

            AssertRewardedRejected(flow, wrongPlacement, MonetizationReasonCodes.PlacementMismatch);
            AssertRewardedRejected(flow, wrongOpportunity, MonetizationReasonCodes.OpportunityMismatch);
            AssertRewardedRejected(flow, wrongToken, MonetizationReasonCodes.EvidenceMalformed);
            AssertRewardedRejected(
                flow,
                new ThrowingRewardedVerifier(),
                MonetizationReasonCodes.VerifierUnavailable
            );

            var malformedEvidence = new RewardedAdEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                flow.Request.PlacementId,
                flow.Request.OpportunityId,
                "reward_token_1",
                string.Empty
            );
            var malformedResult = new RewardedAdResult(
                flow.Request.RequestId,
                RewardedAdState.CompletedUnverified,
                MonetizationReasonCodes.None,
                malformedEvidence
            );
            var untouchedVerifier = new ThrowingRewardedVerifier();
            EntitlementEligibilityResult malformed = DevelopmentGate(
                rewardedVerifier: untouchedVerifier
            )
                .TryReserveRewardedAd(
                    flow.Request,
                    malformedResult
                );
            Assert.That(
                malformed.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.EvidenceMalformed)
            );
            Assert.That(untouchedVerifier.Calls, Is.Zero);
        }

        [Test]
        public void PendingRewardedAd_HoldsOneChannelAndCanBeReleased()
        {
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentRewardedAdScript.Pending());
            RewardedAdResult pending = adapter.Show(AdRequestOne());

            var other = new RewardedAdRequest(
                "ad_request_2",
                MonetizationAllowlists.RewardedDoublePlacementId,
                "opportunity_2"
            );
            RewardedAdResult busy = adapter.Show(other);

            Assert.That(pending.State, Is.EqualTo(RewardedAdState.Pending));
            Assert.That(adapter.IsBusy, Is.True);
            Assert.That(busy.ReasonCode, Is.EqualTo(MonetizationReasonCodes.Busy));
            Assert.That(adapter.ReleasePending("ad_request_1"), Is.True);
            Assert.That(adapter.ReleasePending(string.Empty), Is.False);
            Assert.That(adapter.IsBusy, Is.False);
            Assert.That(
                adapter.GetAvailability(MonetizationAllowlists.RewardedDoublePlacementId).IsReady,
                Is.True
            );

            var resolvable = new ScriptedDevelopmentRewardedAdAdapter();
            resolvable.SetAvailable(true);
            resolvable.Enqueue(DevelopmentRewardedAdScript.Pending());
            Assert.That(
                resolvable.Show(AdRequestOne()).State,
                Is.EqualTo(RewardedAdState.Pending)
            );
            RewardedAdResult stillPending = resolvable.ResolvePending("ad_request_1");
            Assert.That(stillPending.State, Is.EqualTo(RewardedAdState.Pending));
            Assert.That(
                stillPending.ReasonCode,
                Is.EqualTo(MonetizationReasonCodes.DevelopmentScriptMissing)
            );
            Assert.That(resolvable.IsBusy, Is.True);
            resolvable.Enqueue(
                DevelopmentRewardedAdScript.Completed("reward_token_1")
            );
            RewardedAdResult resolved = resolvable.ResolvePending("ad_request_1");
            Assert.That(resolved.State, Is.EqualTo(RewardedAdState.CompletedUnverified));
            Assert.That(resolved.Evidence, Is.Not.Null);
            Assert.That(resolvable.IsBusy, Is.False);
        }

        [Test]
        public void AdUnavailableReturnsImmediatelyWithoutReservationOrEvidence()
        {
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentRewardedAdScript.Unavailable());
            RewardedAdRequest request = AdRequestOne();

            RewardedAdResult result = adapter.Show(request);
            EntitlementEligibilityResult eligibility = DevelopmentGate().TryReserveRewardedAd(
                request,
                result
            );

            Assert.That(result.State, Is.EqualTo(RewardedAdState.Unavailable));
            Assert.That(result.Evidence, Is.Null);
            Assert.That(adapter.IsBusy, Is.False);
            Assert.That(eligibility.IsEligible, Is.False);
            Assert.That(eligibility.Reservation, Is.Null);
        }

        static PurchaseRequest PurchaseRequestOne()
        {
            return new PurchaseRequest(
                "request_1",
                MonetizationAllowlists.StarterOfferId,
                MonetizationAllowlists.StarterProductId
            );
        }

        static RewardedAdRequest AdRequestOne()
        {
            return new RewardedAdRequest(
                "ad_request_1",
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "opportunity_1"
            );
        }

        static VerifiedEntitlementGate DevelopmentGate(
            IPurchaseVerifier purchaseVerifier = null,
            IRewardedAdVerifier rewardedVerifier = null
        )
        {
            return new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                purchaseVerifier ?? new FixedPurchaseVerifier(
                    VerificationState.Verified
                ),
                rewardedVerifier ?? new FixedRewardedVerifier(
                    VerificationState.Verified
                )
            );
        }

        static PurchaseFlow PurchasedEvidence(string requestId, string transactionId)
        {
            var request = new PurchaseRequest(
                requestId,
                MonetizationAllowlists.StarterOfferId,
                MonetizationAllowlists.StarterProductId
            );
            var adapter = new ScriptedDevelopmentIapAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentPurchaseScript.Purchased(transactionId));
            return new PurchaseFlow
            {
                Request = request,
                Purchase = adapter.BeginPurchase(request),
            };
        }

        static PurchaseFlow VerifiedPurchaseFlow(string requestId, string transactionId)
        {
            return PurchasedEvidence(requestId, transactionId);
        }

        static RewardedFlow VerifiedRewardedFlow(string requestId, string rewardToken)
        {
            var request = new RewardedAdRequest(
                requestId,
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "opportunity_1"
            );
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentRewardedAdScript.Completed(rewardToken));
            RewardedAdResult result = adapter.Show(request);
            return new RewardedFlow
            {
                Request = request,
                Result = result,
            };
        }

        static void AssertPurchaseRejected(
            PurchaseFlow flow,
            IPurchaseVerifier verifier,
            string reason
        )
        {
            var gate = DevelopmentGate(purchaseVerifier: verifier);
            EntitlementEligibilityResult result = gate.TryReservePurchase(
                flow.Request,
                flow.Purchase
            );
            Assert.That(result.IsEligible, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(reason));
            Assert.That(result.Reservation, Is.Null);
            Assert.That(gate.ReservedCount, Is.Zero);
        }

        static void AssertRewardedRejected(
            RewardedFlow flow,
            IRewardedAdVerifier verifier,
            string reason
        )
        {
            var gate = DevelopmentGate(rewardedVerifier: verifier);
            EntitlementEligibilityResult result = gate.TryReserveRewardedAd(
                flow.Request,
                flow.Result
            );
            Assert.That(result.IsEligible, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(reason));
            Assert.That(result.Reservation, Is.Null);
            Assert.That(gate.ReservedCount, Is.Zero);
        }

        sealed class PurchaseFlow
        {
            public PurchaseRequest Request;
            public PurchaseResult Purchase;
        }

        sealed class RewardedFlow
        {
            public RewardedAdRequest Request;
            public RewardedAdResult Result;
        }

        sealed class FixedPurchaseVerifier : IPurchaseVerifier
        {
            readonly VerificationState state;

            public int Calls { get; private set; }

            public FixedPurchaseVerifier(VerificationState state)
            {
                this.state = state;
            }

            public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
            {
                Calls++;
                switch (state)
                {
                    case VerificationState.Verified:
                        return PurchaseVerificationResult.Verified(evidence);
                    case VerificationState.Retryable:
                        return PurchaseVerificationResult.Retryable(evidence);
                    case VerificationState.Unavailable:
                        return PurchaseVerificationResult.Unavailable(evidence);
                    default:
                        return PurchaseVerificationResult.Rejected(evidence);
                }
            }
        }

        sealed class AlteredPurchaseVerifier : IPurchaseVerifier
        {
            readonly MonetizationEnvironment? environment;
            readonly string providerId;
            readonly string productId;
            readonly string transactionId;

            public AlteredPurchaseVerifier(
                MonetizationEnvironment? environment = null,
                string providerId = null,
                string productId = null,
                string transactionId = null
            )
            {
                this.environment = environment;
                this.providerId = providerId;
                this.productId = productId;
                this.transactionId = transactionId;
            }

            public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
            {
                var altered = new PurchaseEvidence(
                    environment ?? evidence.Environment,
                    providerId ?? evidence.ProviderId,
                    productId ?? evidence.ProductId,
                    transactionId ?? evidence.TransactionId,
                    evidence.ReceiptPayload
                );
                return PurchaseVerificationResult.Verified(altered);
            }
        }

        sealed class ThrowingPurchaseVerifier : IPurchaseVerifier
        {
            public int Calls { get; private set; }

            public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
            {
                Calls++;
                throw new InvalidOperationException("test verifier failure");
            }
        }

        sealed class FixedRewardedVerifier : IRewardedAdVerifier
        {
            readonly VerificationState state;

            public int Calls { get; private set; }

            public FixedRewardedVerifier(VerificationState state)
            {
                this.state = state;
            }

            public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
            {
                Calls++;
                switch (state)
                {
                    case VerificationState.Verified:
                        return RewardedAdVerificationResult.Verified(evidence);
                    case VerificationState.Retryable:
                        return RewardedAdVerificationResult.Retryable(evidence);
                    case VerificationState.Unavailable:
                        return RewardedAdVerificationResult.Unavailable(evidence);
                    default:
                        return RewardedAdVerificationResult.Rejected(evidence);
                }
            }
        }

        sealed class AlteredRewardedVerifier : IRewardedAdVerifier
        {
            readonly MonetizationEnvironment? environment;
            readonly string providerId;
            readonly string placementId;
            readonly string opportunityId;
            readonly string rewardToken;

            public AlteredRewardedVerifier(
                MonetizationEnvironment? environment = null,
                string providerId = null,
                string placementId = null,
                string opportunityId = null,
                string rewardToken = null
            )
            {
                this.environment = environment;
                this.providerId = providerId;
                this.placementId = placementId;
                this.opportunityId = opportunityId;
                this.rewardToken = rewardToken;
            }

            public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
            {
                var altered = new RewardedAdEvidence(
                    environment ?? evidence.Environment,
                    providerId ?? evidence.ProviderId,
                    placementId ?? evidence.PlacementId,
                    opportunityId ?? evidence.OpportunityId,
                    rewardToken ?? evidence.RewardToken,
                    evidence.ProofPayload
                );
                return RewardedAdVerificationResult.Verified(altered);
            }
        }

        sealed class ThrowingRewardedVerifier : IRewardedAdVerifier
        {
            public int Calls { get; private set; }

            public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
            {
                Calls++;
                throw new InvalidOperationException("test verifier failure");
            }
        }
    }
}
