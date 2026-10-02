using System;
using System.Linq;
using System.Reflection;
using LegendsOfRus.CommercialCore.Monetization;
using LegendsOfRus.CommercialRuntime;
using LegendsOfRus.CommercialRuntime.Fulfillment;
using LegendsOfRus.EconomyCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.CommercialRuntime
{
    public sealed class CommercialFulfillmentEditModeTests
    {
        [Test]
        public void ShippedInactiveGates_RejectAndPreserveSourceBytes()
        {
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var coordinator = new CommercialFulfillmentCoordinator(
                CommercialRuntimeActivationDecision.Uninitialized,
                RuntimeEconomyActivationGate.Load(),
                DevelopmentGate(new ScriptedDevelopmentRewardedAdVerifier())
            );

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, null, null);

            Assert.That(coordinator.IsReady, Is.False);
            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.RuntimeInactive));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void InactiveEconomyRejectsBeforeAnyEntitlementReservation()
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            VerifiedEntitlementGate gate = DevelopmentGate(verifier);
            var coordinator = new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                RuntimeEconomyActivationGate.Load(),
                gate
            );
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, null, null);

            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.EconomyInactive));
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(gate.CompletedCount, Is.Zero);
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [TestCase(MonetizationAllowlists.StarterOfferId,
            MonetizationAllowlists.StarterProductId)]
        [TestCase(MonetizationAllowlists.HeroOfferId,
            MonetizationAllowlists.HeroProductId)]
        [TestCase(MonetizationAllowlists.FirebirdOfferId,
            MonetizationAllowlists.FirebirdProductId)]
        [TestCase(MonetizationAllowlists.EnergyOfferId,
            MonetizationAllowlists.EnergyProductId)]
        public void EveryKnownPurchaseIdentity_HasNoApprovedEconomyRecipe(
            string offerId,
            string productId
        )
        {
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(
                new ScriptedDevelopmentRewardedAdVerifier()
            );
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var request = new PurchaseRequest("purchase_request_1", offerId, productId);

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPreparePurchase(source, request, null);

            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.RecipeUnavailable));
            Assert.That(result.Candidate, Is.Null);
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void UnknownPurchaseNeverFallsBackToLegacyGemGrant()
        {
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(
                new ScriptedDevelopmentRewardedAdVerifier()
            );
            PlayerSaveData source = ValidRewardSource();
            source.gems = 17;
            string before = SaveJson(source);
            var request = new PurchaseRequest(
                "purchase_request_unknown",
                "unknown_pack",
                "unknown_product"
            );

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPreparePurchase(source, request, null);

            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.UnknownIdentity));
            Assert.That(source.gems, Is.EqualTo(17));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void RewardedEnergyIdentity_HasNoApprovedEconomyRecipe()
        {
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(
                new ScriptedDevelopmentRewardedAdVerifier()
            );
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var request = new RewardedAdRequest(
                "reward_request_energy",
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "opportunity_energy_1"
            );

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, null);

            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.RecipeUnavailable));
            Assert.That(source.energy, Is.EqualTo(5));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void UnknownRewardedPlacement_IsRejectedWithoutMutation()
        {
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(
                new ScriptedDevelopmentRewardedAdVerifier()
            );
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var request = new RewardedAdRequest(
                "reward_request_continue",
                "rewarded_continue",
                "opportunity_continue_1"
            );

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, null);

            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.UnknownIdentity));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void VerifiedRewardDouble_PreparesExactDetachedCandidateOnly()
        {
            const string token = "reward_token_1";
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(verifier);
            PlayerSaveData source = ValidRewardSource();
            source.gold = 100;
            source.lastRewardGold = 130;
            string before = SaveJson(source);
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(token, out request);
            verifier.Enqueue(DevelopmentVerificationScript.Verified());

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, adResult);

            Assert.That(result.IsPrepared, Is.True,
                result.ReasonCode + ":" + result.DetailCode);
            CommercialFulfillmentCandidate candidate = result.Candidate;
            Assert.That(candidate.ActionId,
                Is.EqualTo(CommercialEntitlementRecipeCatalog.RewardDoubleActionId));
            Assert.That(candidate.Changed, Is.True);
            Assert.That(candidate.IsPersisted, Is.False);
            Assert.That(candidate.IsEntitlementCompleted, Is.False);
            Assert.That(candidate.DurableMarkerProposal,
                Does.Match("^cf1-[0-9a-f]{64}$"));
            Assert.That(candidate.TransactionKey,
                Is.EqualTo("commercial.fulfillment." + candidate.DurableMarkerProposal));
            Assert.That(candidate.SourceFingerprint,
                Does.Match("^[0-9a-f]{64}$"));
            PlayerSaveData detached = candidate.CreateDetachedSaveCopy();
            Assert.That(detached.gold, Is.EqualTo(230));
            Assert.That(detached.lastRewardDoubled, Is.True);
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void CandidateCopiesAndSnapshots_DoNotAliasEachOtherOrSource()
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(verifier);
            PlayerSaveData source = ValidRewardSource();
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(
                "reward_token_copy",
                out request
            );
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            CommercialFulfillmentCandidate candidate = coordinator
                .TryPrepareRewardedAd(source, request, adResult).Candidate;
            Assert.That(candidate, Is.Not.Null);

            PlayerSaveData first = candidate.CreateDetachedSaveCopy();
            PlayerSaveData second = candidate.CreateDetachedSaveCopy();
            first.gold = -999;
            first.completedStoryScenes.Add("mutated");
            var snapshot = candidate.Snapshot;
            snapshot.gold = -123;

            Assert.That(second.gold, Is.EqualTo(source.gold + source.lastRewardGold));
            Assert.That(second.completedStoryScenes, Is.Empty);
            Assert.That(candidate.Snapshot.gold,
                Is.EqualTo(source.gold + source.lastRewardGold));
            Assert.That(source.gold, Is.EqualTo(100));
        }

        [Test]
        public void CandidatePreparation_ReleasesReservationAndNeverCompletesGate()
        {
            const string token = "reward_token_retry";
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            VerifiedEntitlementGate gate = DevelopmentGate(verifier);
            var coordinator = new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                ActiveEconomy(),
                gate
            );
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(token, out request);
            verifier.Enqueue(DevelopmentVerificationScript.Verified());

            CommercialFulfillmentPreparationResult first =
                coordinator.TryPrepareRewardedAd(
                    ValidRewardSource(),
                    request,
                    adResult
                );
            verifier.Enqueue(DevelopmentVerificationScript.Verified());
            CommercialFulfillmentPreparationResult retry =
                coordinator.TryPrepareRewardedAd(
                    ValidRewardSource(),
                    request,
                    adResult
                );

            Assert.That(first.IsPrepared, Is.True, first.ReasonCode);
            Assert.That(retry.IsPrepared, Is.True, retry.ReasonCode);
            Assert.That(retry.Candidate.DurableMarkerProposal,
                Is.EqualTo(first.Candidate.DurableMarkerProposal));
            Assert.That(gate.ReservedCount, Is.Zero);
            Assert.That(gate.CompletedCount, Is.Zero);
        }

        [Test]
        public void DurableReplaySeed_BlocksCandidateBeforeAnyGrant()
        {
            const string token = "reward_token_seeded";
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            var gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new DefaultUnavailablePurchaseVerifier(),
                verifier,
                new[] { "ad|development_sandbox|" + token }
            );
            var coordinator = new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                ActiveEconomy(),
                gate
            );
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(token, out request);
            verifier.Enqueue(DevelopmentVerificationScript.Verified());

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, adResult);

            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.CandidateRejected));
            Assert.That(result.DetailCode,
                Is.EqualTo(MonetizationReasonCodes.DuplicateEntitlement));
            Assert.That(gate.CompletedCount, Is.EqualTo(1));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void VerificationMismatch_RejectsWithoutCandidateOrMutation()
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(verifier);
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var request = new RewardedAdRequest(
                "reward_request_mismatch",
                MonetizationAllowlists.RewardedDoublePlacementId,
                "opportunity_expected"
            );
            var evidence = new RewardedAdEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                MonetizationAllowlists.RewardedDoublePlacementId,
                "opportunity_other",
                "reward_token_mismatch",
                DevelopmentMonetizationProofs.RewardedProof(
                    MonetizationAllowlists.RewardedDoublePlacementId,
                    "opportunity_other",
                    "reward_token_mismatch"
                )
            );
            var adResult = new RewardedAdResult(
                request.RequestId,
                RewardedAdState.CompletedUnverified,
                MonetizationReasonCodes.None,
                evidence
            );

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, adResult);

            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.DetailCode,
                Is.EqualTo(MonetizationReasonCodes.OpportunityMismatch));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void InvalidOrAlreadyDoubledReward_IsRejected(
            bool eligible,
            bool doubled
        )
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(verifier);
            PlayerSaveData source = ValidRewardSource();
            source.lastRewardEligibleForDouble = eligible;
            source.lastRewardDoubled = doubled;
            string before = SaveJson(source);
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(
                eligible
                    ? "reward_token_invalid_eligible"
                    : "reward_token_invalid_ineligible",
                out request
            );
            verifier.Enqueue(DevelopmentVerificationScript.Verified());

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, adResult);

            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.CandidateRejected));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void OverflowRewardDouble_RollsBackDetachedCandidate()
        {
            var verifier = new ScriptedDevelopmentRewardedAdVerifier();
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(verifier);
            PlayerSaveData source = ValidRewardSource();
            source.gold = int.MaxValue;
            source.lastRewardGold = 1;
            string before = SaveJson(source);
            RewardedAdRequest request;
            RewardedAdResult adResult = CompletedReward(
                "reward_token_overflow",
                out request
            );
            verifier.Enqueue(DevelopmentVerificationScript.Verified());

            CommercialFulfillmentPreparationResult result =
                coordinator.TryPrepareRewardedAd(source, request, adResult);

            Assert.That(result.IsPrepared, Is.False);
            Assert.That(result.Candidate, Is.Null);
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        [Test]
        public void CandidatePublicSurface_ContainsNoReceiptProofOrPersistenceApi()
        {
            string[] members = typeof(CommercialFulfillmentCandidate)
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Select(member => member.Name)
                .ToArray();

            Assert.That(members.Any(name => ContainsSensitive(name)), Is.False);
            Assert.That(members.Any(name =>
                string.Equals(name, "Commit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "TryCommit", StringComparison.OrdinalIgnoreCase)),
                Is.False);
            Assert.That(members.Any(name =>
                string.Equals(name, "Complete", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "TryComplete", StringComparison.OrdinalIgnoreCase)),
                Is.False);
        }

        [Test]
        public void AtomicStoreBoundary_IsInternalAndAlwaysUnavailable()
        {
            Assert.That(typeof(ICommercialAtomicFulfillmentStore).IsPublic, Is.False);
            Assert.That(typeof(CommercialAtomicCommitRequest).IsPublic, Is.False);
            Assert.That(typeof(CommercialAtomicCommitResult).IsPublic, Is.False);
            var store = new UnavailableCommercialAtomicFulfillmentStore();
            PlayerSaveData source = ValidRewardSource();
            string before = SaveJson(source);
            var request = new CommercialAtomicCommitRequest(
                "cf1-" + new string('a', 64),
                new string('b', 64),
                source
            );

            CommercialAtomicCommitResult result = store.TryCommit(request);

            Assert.That(result.Committed, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialFulfillmentReasonCodes.AtomicStoreUnavailable));
            Assert.That(SaveJson(source), Is.EqualTo(before));
        }

        static bool ContainsSensitive(string value)
        {
            return
                value.IndexOf("Receipt", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("Proof", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("Token", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static CommercialFulfillmentCoordinator ReadyCoordinator(
            ScriptedDevelopmentRewardedAdVerifier verifier
        )
        {
            return new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                ActiveEconomy(),
                DevelopmentGate(verifier)
            );
        }

        static VerifiedEntitlementGate DevelopmentGate(
            ScriptedDevelopmentRewardedAdVerifier verifier
        )
        {
            return new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new DefaultUnavailablePurchaseVerifier(),
                verifier
            );
        }

        static CommercialRuntimeActivationDecision ActiveCommercialDecision()
        {
            return new CommercialRuntimeActivationDecision(
                CommercialRuntimePhase.Active,
                CommercialRuntimeReasonCodes.Activated,
                true,
                true,
                true,
                true,
                "development_sandbox",
                new string('c', 64),
                string.Empty
            );
        }

        static RuntimeEconomyActivationResult ActiveEconomy()
        {
            EconomyCatalogLoadResult proposal = EconomyCatalogLoader.LoadProposal();
            Assert.That(proposal.Succeeded, Is.True, proposal.Error);
            var manifest = new RuntimeEconomyActivationManifestData
            {
                schemaVersion = RuntimeEconomyActivationGate.ManifestSchemaVersion,
                manifestId = RuntimeEconomyActivationGate.ManifestId,
                catalogSchemaVersion = proposal.Catalog.schemaVersion,
                catalogId = RuntimeEconomyActivationGate.CatalogId,
                catalogSourceSha256 = RuntimeEconomyActivationGate.CatalogSourceSha256,
                unityVersion = RuntimeEconomyActivationGate.RequiredUnityVersion,
                productApproval = true,
                compilationVerified = true,
                editorSelfTestsPassed = true,
                editModeTestsPassed = true,
                playModeTestsPassed = true,
                runtimeActivationApproved = true
            };
            RuntimeEconomyActivationResult result = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );
            Assert.That(result.IsRuntimeActivated, Is.True, result.Reason);
            return result;
        }

        static RewardedAdResult CompletedReward(
            string token,
            out RewardedAdRequest request
        )
        {
            request = new RewardedAdRequest(
                "reward_request_" + token,
                MonetizationAllowlists.RewardedDoublePlacementId,
                "opportunity_" + token
            );
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentRewardedAdScript.Completed(token));
            RewardedAdResult result = adapter.Show(request);
            Assert.That(result.State, Is.EqualTo(RewardedAdState.CompletedUnverified));
            return result;
        }

        static PlayerSaveData ValidRewardSource()
        {
            return new PlayerSaveData
            {
                saveVersion = 6,
                gold = 100,
                gems = 17,
                energy = 5,
                energyMax = 5,
                lastEnergyUtcTicks = 638900000000000000L,
                lastRewardGold = 25,
                lastRewardEligibleForDouble = true,
                lastRewardDoubled = false
            };
        }

        static string SaveJson(PlayerSaveData save)
        {
            return JsonUtility.ToJson(save);
        }
    }
}
