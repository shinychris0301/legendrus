#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegendsOfRus.CommercialCore.Analytics;
using LegendsOfRus.CommercialCore.Monetization;
using LegendsOfRus.CommercialCore.RemoteConfig;
using LegendsOfRus.CommercialRuntime;
using LegendsOfRus.CommercialRuntime.Fulfillment;
using LegendsOfRus.EconomyCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step16ProviderIntegrationRuntimeWiringSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.16 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("commercial config resource is valid and pinned",
                ConfigIsPinned, ref passed, ref total, failures);
            Check("commercial config allows no provider or capability",
                ConfigIsAllDisabled, ref passed, ref total, failures);
            Check("bundled remote config is accepted as proposal-only LKG",
                RemoteConfigLoadsProposalOnly, ref passed, ref total, failures);
            Check("remote config first failure uses conservative fallback",
                RemoteConfigFirstFailureFallsBack, ref passed, ref total, failures);
            Check("remote config refresh failure retains the same LKG",
                RemoteConfigFailureRetainsLkg, ref passed, ref total, failures);
            Check("commercial activation manifest retains no pass claims",
                ActivationManifestIsAllFalse, ref passed, ref total, failures);
            Check("shipped activation stops at inactive v1.15 economy",
                ShippedActivationIsBlocked, ref passed, ref total, failures);
            Check("external provider activation build switch is false",
                ExternalActivationSwitchIsFalse, ref passed, ref total, failures);
            Check("trusted composition preserves the offline core loop",
                CompositionPreservesOfflineCore, ref passed, ref total, failures);
            Check("trusted composition exposes no external communication or save writes",
                CompositionHasNoExternalEffects, ref passed, ref total, failures);
            Check("trusted composition capabilities are all false",
                CompositionCapabilitiesAreFalse, ref passed, ref total, failures);
            Check("trusted composition binds only disabled diagnostics",
                CompositionProvidersAreDisabled, ref passed, ref total, failures);
            Check("disabled analytics never reads its supplied clock",
                DisabledAnalyticsDoesNotReadClock, ref passed, ref total, failures);
            Check("typed analytics session and screen event are ordered",
                TypedAnalyticsIsOrdered, ref passed, ref total, failures);
            Check("rejected analytics begin leaves funnel state unchanged",
                AnalyticsBeginFailureIsAtomic, ref passed, ref total, failures);
            Check("analytics cancellation emits no terminal event",
                AnalyticsCancellationIsStateOnly, ref passed, ref total, failures);
            Check("analytics runtime bridge has no upload or flush API",
                AnalyticsHasNoTransportApi, ref passed, ref total, failures);
            Check("all four IAP identities have no fulfillment recipe",
                PurchasesHaveNoRecipe, ref passed, ref total, failures);
            Check("rewarded energy has no fulfillment recipe",
                RewardedEnergyHasNoRecipe, ref passed, ref total, failures);
            Check("only victory-double resolves to a catalog action",
                OnlyRewardDoubleMaps, ref passed, ref total, failures);
            Check("verified reward double prepares an exact detached candidate",
                RewardDoubleCandidateIsExact, ref passed, ref total, failures);
            Check("candidate preparation never persists or completes entitlement",
                CandidateNeverCommits, ref passed, ref total, failures);
            Check("candidate preparation releases its entitlement reservation",
                CandidateReleasesReservation, ref passed, ref total, failures);
            Check("atomic fulfillment boundary is internal and unavailable",
                AtomicBoundaryIsUnavailable, ref passed, ref total, failures);
            Check("runtime monetization binding and commit factory are non-public",
                RuntimeMonetizationTrustBoundaryIsInternal,
                ref passed, ref total, failures);
            Check("legacy monetization wrappers contain no direct grants",
                LegacyMonetizationGrantsAreAbsent, ref passed, ref total, failures);
            Check("commercial runtime has no GameState SaveService or PlayerPrefs access",
                CommercialRuntimeHasNoLiveStateAccess,
                ref passed, ref total, failures);
            Check("commercial runtime has no native SDK or network artifact",
                NoSdkOrNativePluginArtifacts, ref passed, ref total, failures);
            Check("save version and PlayerPrefs key remain v6-compatible",
                SaveIdentityIsUnchanged, ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.16 Step 2 provider integration and runtime wiring " +
                    "self-tests passed: " + passed + "/" + total +
                    ". Runtime activation remains disabled."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        "v1.16 Step 2 self-tests passed: " + passed + "/" + total +
                        ".\n\nThis is source verification only. Provider, legal, " +
                        "device, durable-fulfillment, and activation approvals " +
                        "remain outstanding.",
                        "OK"
                    );
                }
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.16 Step 2 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary
            );
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Test failures",
                    summary,
                    "OK"
                );
            }
            throw new InvalidOperationException(
                "v1.16 Step 2 self-tests failed."
            );
        }

        static bool ConfigIsPinned()
        {
            CommercialRuntimeConfigLoadResult result =
                CommercialRuntimeConfigLoader.Load();
            return
                result != null &&
                result.IsValid &&
                result.Snapshot != null &&
                !result.Snapshot.IsFallback &&
                result.Snapshot.ConfigId ==
                    CommercialRuntimeConfigLoader.RequiredConfigId &&
                result.Snapshot.SourceSha256 ==
                    CommercialRuntimeConfigLoader.ExpectedSourceSha256;
        }

        static bool ConfigIsAllDisabled()
        {
            CommercialRuntimeConfigSnapshot value =
                CommercialRuntimeConfigLoader.Load()?.Snapshot;
            return
                value != null &&
                value.Environment == "disabled" &&
                value.ConsentProviderId == "disabled" &&
                value.AnalyticsProviderId == "disabled" &&
                value.IapProviderId == "disabled" &&
                value.RewardedAdProviderId == "disabled" &&
                !value.AnalyticsCaptureAllowed &&
                !value.AnalyticsUploadAllowed &&
                !value.IapAllowed &&
                !value.RewardedAdsAllowed &&
                !value.DevelopmentSandboxAllowed;
        }

        static bool RemoteConfigLoadsProposalOnly()
        {
            var cache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate update = cache.LoadBundled();
            return
                update != null &&
                update.Accepted &&
                cache.HasLastKnownGood &&
                !update.UsesConservativeFallback &&
                update.EffectiveSnapshot.ProposalOnly &&
                !update.EffectiveSnapshot.IsAuthoritative &&
                update.EffectiveSnapshot.ContentSha256 ==
                    RuntimeRemoteConfigCache.ExpectedContentSha256;
        }

        static bool RemoteConfigFirstFailureFallsBack()
        {
            var cache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate update = cache.TryRefresh(null);
            return
                update != null &&
                !update.Accepted &&
                !update.RetainedLastKnownGood &&
                update.UsesConservativeFallback &&
                !cache.HasLastKnownGood;
        }

        static bool RemoteConfigFailureRetainsLkg()
        {
            var cache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate first = cache.LoadBundled();
            if (first == null || !first.Accepted)
                return false;
            RemoteConfigSnapshot before = cache.Current;
            RuntimeRemoteConfigUpdate failed = cache.TryRefresh(
                new ThrowingRemoteProvider()
            );
            return
                failed != null &&
                !failed.Accepted &&
                failed.RetainedLastKnownGood &&
                ReferenceEquals(before, failed.EffectiveSnapshot) &&
                ReferenceEquals(before, cache.Current);
        }

        static bool ActivationManifestIsAllFalse()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeActivationGate.ResourcePath
            );
            if (asset == null)
                return false;
            ActivationManifestMirror value =
                JsonUtility.FromJson<ActivationManifestMirror>(asset.text);
            return
                value != null &&
                value.environment == "disabled" &&
                !value.v115RuntimeEconomyApproved &&
                !value.v116Step1CompilationVerified &&
                !value.v116Step1ProjectValidatorPassed &&
                !value.v116Step1EditorSelfTestsPassed &&
                !value.v116Step1EditModeTestsPassed &&
                !value.v116Step1PlayModeTestsPassed &&
                !value.providerSelectionApproved &&
                !value.legalPrivacyApproved &&
                !value.consentFlowApproved &&
                !value.productIdentifiersApproved &&
                !value.rewardedPlacementsApproved &&
                !value.regionalBehaviorApproved &&
                !value.securityReviewPassed &&
                !value.representativeDeviceFailurePathsPassed &&
                !value.saveMigrationApproved &&
                !value.durableFulfillmentApproved &&
                !value.analyticsActivationApproved &&
                !value.iapActivationApproved &&
                !value.rewardedAdsActivationApproved &&
                !value.finalRuntimeActivationApproved;
        }

        static bool ShippedActivationIsBlocked()
        {
            CommercialRuntimeActivationDecision decision =
                CommercialRuntimeActivationGate.Load(
                    CommercialRuntimeConfigLoader.Load(),
                    new RuntimeRemoteConfigCache().LoadBundled()
                );
            return
                decision != null &&
                !decision.IsRuntimeActivated &&
                decision.Phase == CommercialRuntimePhase.VerificationRequired &&
                decision.ReasonCode ==
                    CommercialRuntimeReasonCodes.V115EconomyInactive &&
                !decision.AnalyticsApproved &&
                !decision.IapApproved &&
                !decision.RewardedAdsApproved;
        }

        static bool ExternalActivationSwitchIsFalse()
        {
            FieldInfo field = typeof(CommercialRuntimeActivationGate).GetField(
                "ExternalProviderActivationCompiled",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                field != null &&
                field.IsLiteral &&
                Equals(field.GetRawConstantValue(), false);
        }

        static CommercialRuntimeComposition TrustedComposition()
        {
            MethodInfo build = typeof(CommercialRuntimeComposition).GetMethod(
                "BuildTrustedDisabled",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return build != null
                ? build.Invoke(null, null) as CommercialRuntimeComposition
                : null;
        }

        static bool CompositionPreservesOfflineCore()
        {
            CommercialRuntimeComposition value = TrustedComposition();
            return value != null && value.PreservesOfflineCoreLoop;
        }

        static bool CompositionHasNoExternalEffects()
        {
            CommercialRuntimeComposition value = TrustedComposition();
            return
                value != null &&
                !value.PerformsExternalCommunication &&
                !value.PerformsSaveWrites;
        }

        static bool CompositionCapabilitiesAreFalse()
        {
            CommercialRuntimeCapabilities value = TrustedComposition()?.Capabilities;
            return
                value != null &&
                !value.AnalyticsCaptureEnabled &&
                !value.AnalyticsUploadEnabled &&
                !value.IapEnabled &&
                !value.RewardedAdsEnabled &&
                !value.DurableFulfillmentEnabled;
        }

        static bool CompositionProvidersAreDisabled()
        {
            CommercialRuntimeProviderDiagnostics value =
                TrustedComposition()?.Providers;
            return
                value != null &&
                value.ConsentProviderId == "disabled" &&
                value.AnalyticsAdapterId == "disabled" &&
                value.IapProviderId == "disabled" &&
                value.RewardedAdProviderId == "disabled" &&
                value.ConsentRestricted &&
                value.AnalyticsAdapterDisabled &&
                value.IapAdapterDisabled &&
                value.PurchaseVerifierUnavailable &&
                value.RewardedAdAdapterDisabled &&
                value.RewardedAdVerifierUnavailable;
        }

        static bool DisabledAnalyticsDoesNotReadClock()
        {
            int reads = 0;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                false,
                adapter,
                "selftest_disabled",
                () =>
                {
                    reads++;
                    throw new InvalidOperationException();
                }
            );
            AnalyticsRecordResult result = bridge.TryBeginSession();
            return
                result != null &&
                !result.Accepted &&
                reads == 0 &&
                adapter.Count == 0 &&
                bridge.AcceptedCount == 0;
        }

        static bool TypedAnalyticsIsOrdered()
        {
            long ticks = 100L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "selftest_session",
                () => ++ticks
            );
            if (!bridge.TryBeginSession().Accepted ||
                !bridge.TryScreenView("gameplay").Accepted)
                return false;
            var records = adapter.Snapshot();
            return
                records.Count == 2 &&
                records[0].EventId == AnalyticsEventIds.SessionStart &&
                records[0].Sequence == 1L &&
                records[1].EventId == AnalyticsEventIds.ScreenView &&
                records[1].Sequence == 2L &&
                records[0].UtcTicks < records[1].UtcTicks;
        }

        static bool AnalyticsBeginFailureIsAtomic()
        {
            long ticks = 200L;
            var adapter = new DeterministicOfflineAnalyticsAdapter(1);
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "selftest_atomic",
                () => ++ticks
            );
            if (!bridge.TryBeginSession().Accepted)
                return false;
            AnalyticsRecordResult failed =
                bridge.TryLevelStart("level_1", 1, false);
            return
                !failed.Accepted &&
                !bridge.HasActiveLevelAttempt &&
                bridge.TrackedLevelAttemptCount == 0 &&
                bridge.AcceptedLevelTerminalCount == 0 &&
                adapter.Count == 1;
        }

        static bool AnalyticsCancellationIsStateOnly()
        {
            long ticks = 300L;
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var bridge = new RuntimeAnalyticsBridge(
                true,
                adapter,
                "selftest_cancel",
                () => ++ticks
            );
            if (!bridge.TryBeginSession().Accepted ||
                !bridge.TryLevelStart("level_2", 1, false).Accepted)
                return false;
            int before = adapter.Count;
            AnalyticsRecordResult cancelled =
                bridge.TryCancelLevelAttempt("level_2", 1);
            return
                cancelled.Accepted &&
                adapter.Count == before &&
                !bridge.HasActiveLevelAttempt &&
                bridge.AcceptedLevelTerminalCount == 0;
        }

        static bool AnalyticsHasNoTransportApi()
        {
            string[] methods = typeof(RuntimeAnalyticsBridge)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(value => value.Name)
                .ToArray();
            return !methods.Any(value =>
                value.IndexOf("upload", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("flush", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        static bool PurchasesHaveNoRecipe()
        {
            var pairs = new[]
            {
                new[] { MonetizationAllowlists.StarterOfferId,
                    MonetizationAllowlists.StarterProductId },
                new[] { MonetizationAllowlists.HeroOfferId,
                    MonetizationAllowlists.HeroProductId },
                new[] { MonetizationAllowlists.FirebirdOfferId,
                    MonetizationAllowlists.FirebirdProductId },
                new[] { MonetizationAllowlists.EnergyOfferId,
                    MonetizationAllowlists.EnergyProductId }
            };
            CommercialFulfillmentCoordinator coordinator = ReadyCoordinator(
                new FixedRewardedVerifier()
            );
            for (int i = 0; i < pairs.Length; i++)
            {
                var request = new PurchaseRequest(
                    "selftest_purchase_" + i,
                    pairs[i][0],
                    pairs[i][1]
                );
                CommercialFulfillmentPreparationResult result =
                    coordinator.TryPreparePurchase(RewardSource(), request, null);
                if (result == null || result.IsPrepared ||
                    result.ReasonCode !=
                        CommercialFulfillmentReasonCodes.RecipeUnavailable)
                    return false;
            }
            return true;
        }

        static bool RewardedEnergyHasNoRecipe()
        {
            CommercialFulfillmentPreparationResult result = ReadyCoordinator(
                new FixedRewardedVerifier()
            ).TryPrepareRewardedAd(
                RewardSource(),
                new RewardedAdRequest(
                    "selftest_energy_request",
                    MonetizationAllowlists.RewardedEnergyPlacementId,
                    "selftest_energy_opportunity"
                ),
                null
            );
            return
                result != null &&
                !result.IsPrepared &&
                result.ReasonCode ==
                    CommercialFulfillmentReasonCodes.RecipeUnavailable;
        }

        static bool OnlyRewardDoubleMaps()
        {
            string action;
            string detail;
            bool doubled = CommercialEntitlementRecipeCatalog.TryResolveRewardedAction(
                new RewardedAdRequest(
                    "selftest_double_map",
                    MonetizationAllowlists.RewardedDoublePlacementId,
                    "selftest_double_opportunity"
                ),
                out action,
                out detail
            );
            bool energy = CommercialEntitlementRecipeCatalog.TryResolveRewardedAction(
                new RewardedAdRequest(
                    "selftest_energy_map",
                    MonetizationAllowlists.RewardedEnergyPlacementId,
                    "selftest_energy_opportunity"
                ),
                out _,
                out _
            );
            return
                doubled &&
                !energy &&
                action == CommercialEntitlementRecipeCatalog.RewardDoubleActionId &&
                detail == MonetizationReasonCodes.None;
        }

        static bool RewardDoubleCandidateIsExact()
        {
            VerifiedEntitlementGate gate;
            CommercialFulfillmentPreparationResult result = PrepareDouble(out gate);
            if (result == null || !result.IsPrepared || result.Candidate == null)
                return false;
            PlayerSaveData candidate = result.Candidate.CreateDetachedSaveCopy();
            return
                candidate != null &&
                candidate.gold == 230 &&
                candidate.lastRewardDoubled &&
                result.Candidate.ActionId ==
                    CommercialEntitlementRecipeCatalog.RewardDoubleActionId &&
                result.Candidate.DurableMarkerProposal.StartsWith("cf1-") &&
                result.Candidate.SourceFingerprint.Length == 64;
        }

        static bool CandidateNeverCommits()
        {
            VerifiedEntitlementGate gate;
            CommercialFulfillmentPreparationResult result = PrepareDouble(out gate);
            return
                result != null &&
                result.IsPrepared &&
                !result.Candidate.IsPersisted &&
                !result.Candidate.IsEntitlementCompleted &&
                gate.CompletedCount == 0;
        }

        static bool CandidateReleasesReservation()
        {
            VerifiedEntitlementGate gate;
            CommercialFulfillmentPreparationResult result = PrepareDouble(out gate);
            return
                result != null &&
                result.IsPrepared &&
                gate.ReservedCount == 0 &&
                gate.CompletedCount == 0;
        }

        static bool AtomicBoundaryIsUnavailable()
        {
            Assembly assembly = typeof(CommercialFulfillmentCoordinator).Assembly;
            Type store = assembly.GetType(
                "LegendsOfRus.CommercialRuntime.Fulfillment." +
                "UnavailableCommercialAtomicFulfillmentStore"
            );
            Type request = assembly.GetType(
                "LegendsOfRus.CommercialRuntime.Fulfillment." +
                "CommercialAtomicCommitRequest"
            );
            Type result = assembly.GetType(
                "LegendsOfRus.CommercialRuntime.Fulfillment." +
                "CommercialAtomicCommitResult"
            );
            return
                store != null && !store.IsPublic &&
                request != null && !request.IsPublic &&
                result != null && !result.IsPublic &&
                result.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .Length == 0;
        }

        static bool RuntimeMonetizationTrustBoundaryIsInternal()
        {
            return
                typeof(MonetizationService).GetMethod(
                    "TryBindRuntimePort",
                    BindingFlags.Public | BindingFlags.Instance
                ) == null &&
                typeof(RuntimeMonetizationActionResult).GetMethod(
                    "Committed",
                    BindingFlags.Public | BindingFlags.Static
                ) == null &&
                typeof(RuntimeMonetizationActionResult).GetConstructors(
                    BindingFlags.Public | BindingFlags.Instance
                ).Length == 0;
        }

        static bool LegacyMonetizationGrantsAreAbsent()
        {
            string source = ReadAssetText(
                "LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs"
            );
            return
                !string.IsNullOrEmpty(source) &&
                source.IndexOf("AddGold(", StringComparison.Ordinal) < 0 &&
                source.IndexOf("AddGems(", StringComparison.Ordinal) < 0 &&
                source.IndexOf("AddEnergy(", StringComparison.Ordinal) < 0 &&
                source.IndexOf("RefillEnergy(", StringComparison.Ordinal) < 0 &&
                source.IndexOf("TryDoubleLastReward(", StringComparison.Ordinal) < 0 &&
                source.IndexOf("+ 50", StringComparison.Ordinal) < 0;
        }

        static bool CommercialRuntimeHasNoLiveStateAccess()
        {
            string root = Path.Combine(
                Application.dataPath,
                "LegendsOfRus/Scripts/CommercialRuntime"
            );
            if (!Directory.Exists(root))
                return false;
            string source = string.Join("\n", Directory.GetFiles(
                root,
                "*.cs",
                SearchOption.AllDirectories
            ).Select(File.ReadAllText).ToArray());
            return
                source.IndexOf("GameState.", StringComparison.Ordinal) < 0 &&
                source.IndexOf("SaveService.", StringComparison.Ordinal) < 0 &&
                source.IndexOf("PlayerPrefs.", StringComparison.Ordinal) < 0 &&
                source.IndexOf(".Complete(", StringComparison.Ordinal) < 0;
        }

        static bool NoSdkOrNativePluginArtifacts()
        {
            string[] forbidden = { ".dll", ".aar", ".jar", ".so", ".dylib" };
            return !Directory.GetFiles(
                Application.dataPath,
                "*",
                SearchOption.AllDirectories
            ).Any(path => forbidden.Contains(
                Path.GetExtension(path).ToLowerInvariant()
            ));
        }

        static bool SaveIdentityIsUnchanged()
        {
            FieldInfo key = typeof(SaveService).GetField(
                "Key",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                new PlayerSaveData().saveVersion == 6 &&
                key != null &&
                key.IsLiteral &&
                Equals(key.GetRawConstantValue(), "LegendsOfRus_Save_v1");
        }

        static CommercialFulfillmentPreparationResult PrepareDouble(
            out VerifiedEntitlementGate gate
        )
        {
            var verifier = new FixedRewardedVerifier();
            gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new DefaultUnavailablePurchaseVerifier(),
                verifier
            );
            var request = new RewardedAdRequest(
                "selftest_reward_request",
                MonetizationAllowlists.RewardedDoublePlacementId,
                "selftest_reward_opportunity"
            );
            var evidence = new RewardedAdEvidence(
                MonetizationEnvironment.DevelopmentSandbox,
                DevelopmentMonetizationProofs.ProviderId,
                request.PlacementId,
                request.OpportunityId,
                "selftest_reward_token",
                DevelopmentMonetizationProofs.RewardedProof(
                    request.PlacementId,
                    request.OpportunityId,
                    "selftest_reward_token"
                )
            );
            var ad = new RewardedAdResult(
                request.RequestId,
                RewardedAdState.CompletedUnverified,
                MonetizationReasonCodes.None,
                evidence
            );
            return new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                ActiveEconomy(),
                gate
            ).TryPrepareRewardedAd(RewardSource(), request, ad);
        }

        static CommercialFulfillmentCoordinator ReadyCoordinator(
            IRewardedAdVerifier verifier
        )
        {
            return new CommercialFulfillmentCoordinator(
                ActiveCommercialDecision(),
                ActiveEconomy(),
                new VerifiedEntitlementGate(
                    MonetizationEnvironment.DevelopmentSandbox,
                    new DefaultUnavailablePurchaseVerifier(),
                    verifier
                )
            );
        }

        static CommercialRuntimeActivationDecision ActiveCommercialDecision()
        {
            ConstructorInfo constructor = typeof(CommercialRuntimeActivationDecision)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single();
            return constructor.Invoke(new object[]
            {
                CommercialRuntimePhase.Active,
                CommercialRuntimeReasonCodes.Activated,
                true,
                true,
                true,
                true,
                "development_sandbox",
                new string('c', 64),
                string.Empty
            }) as CommercialRuntimeActivationDecision;
        }

        static RuntimeEconomyActivationResult ActiveEconomy()
        {
            EconomyCatalogLoadResult proposal = EconomyCatalogLoader.LoadProposal();
            if (proposal == null || !proposal.Succeeded || proposal.Catalog == null)
                return null;
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
            return RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );
        }

        static PlayerSaveData RewardSource()
        {
            return new PlayerSaveData
            {
                saveVersion = 6,
                gold = 100,
                gems = 20,
                energy = 5,
                energyMax = 5,
                lastEnergyUtcTicks = 638900000000000000L,
                lastRewardGold = 130,
                lastRewardEligibleForDouble = true,
                lastRewardDoubled = false
            };
        }

        static string ReadAssetText(string relativePath)
        {
            string path = Path.Combine(
                Application.dataPath,
                relativePath.Replace('/', Path.DirectorySeparatorChar)
            );
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        static void Check(
            string name,
            Func<bool> predicate,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (predicate())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + name);
            }
            catch (Exception exception)
            {
                failures.Add(
                    "FAIL: " + name + " (" + exception.GetType().Name + ")"
                );
            }
        }

        sealed class ThrowingRemoteProvider : IRemoteConfigProvider
        {
            public RemoteConfigLoadResult LoadSnapshot()
            {
                throw new InvalidOperationException("self-test failure");
            }
        }

        sealed class FixedRewardedVerifier : IRewardedAdVerifier
        {
            public RewardedAdVerificationResult Verify(RewardedAdEvidence evidence)
            {
                return RewardedAdVerificationResult.Verified(evidence);
            }
        }

        [Serializable]
        sealed class ActivationManifestMirror
        {
            public string environment = string.Empty;
            public bool v115RuntimeEconomyApproved;
            public bool v116Step1CompilationVerified;
            public bool v116Step1ProjectValidatorPassed;
            public bool v116Step1EditorSelfTestsPassed;
            public bool v116Step1EditModeTestsPassed;
            public bool v116Step1PlayModeTestsPassed;
            public bool providerSelectionApproved;
            public bool legalPrivacyApproved;
            public bool consentFlowApproved;
            public bool productIdentifiersApproved;
            public bool rewardedPlacementsApproved;
            public bool regionalBehaviorApproved;
            public bool securityReviewPassed;
            public bool representativeDeviceFailurePathsPassed;
            public bool saveMigrationApproved;
            public bool durableFulfillmentApproved;
            public bool analyticsActivationApproved;
            public bool iapActivationApproved;
            public bool rewardedAdsActivationApproved;
            public bool finalRuntimeActivationApproved;
        }
    }
}
#endif
