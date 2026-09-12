#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.CommercialCore.Analytics;
using LegendsOfRus.CommercialCore.Consent;
using LegendsOfRus.CommercialCore.Monetization;
using LegendsOfRus.CommercialCore.RemoteConfig;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step16CommercialAdaptersSandboxSelfTest
    {
        const long FixedTicks = 1000L;

        [MenuItem("Legends of Rus/Tests/Run v1.16 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check(
                "analytics schema one contains nineteen strictly valid events",
                () =>
                    AnalyticsSchemaRegistry.All.Count == 19 &&
                    AnalyticsEventValidator.ValidateRegistry().Accepted,
                ref passed,
                ref total,
                failures
            );
            Check(
                "unknown and free-form analytics fail closed",
                InvalidAnalyticsFailsClosed,
                ref passed,
                ref total,
                failures
            );
            Check(
                "capture-denied analytics never starts a session",
                () =>
                {
                    var adapter = new DeterministicOfflineAnalyticsAdapter();
                    var pipeline = new AnalyticsPipeline(false, adapter);
                    AnalyticsRecordResult result = pipeline.TryBeginSession(
                        "session_1",
                        FixedTicks
                    );
                    return
                        !result.Accepted &&
                        result.ReasonCode == AnalyticsReasonCodes.CaptureNotAllowed &&
                        adapter.Count == 0 &&
                        !pipeline.HasStartedSession;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "offline analytics orders attributes and sequences deterministically",
                OfflineAnalyticsIsDeterministic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "analytics upload remains unavailable",
                () =>
                {
                    var adapter = new DeterministicOfflineAnalyticsAdapter();
                    var pipeline = new AnalyticsPipeline(true, adapter);
                    return
                        !pipeline.TryFlush().Accepted &&
                        pipeline.TryFlush().ReasonCode ==
                            AnalyticsReasonCodes.UploadUnavailable &&
                        !adapter.SupportsUpload;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "level funnel accepts one terminal outcome per attempt",
                FunnelAcceptsOneTerminal,
                ref passed,
                ref total,
                failures
            );
            Check(
                "invalid funnel transitions preserve active attempt state",
                FunnelFailureIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "bundled remote config matches reviewed source and content hashes",
                BundledRemoteConfigMatches,
                ref passed,
                ref total,
                failures
            );
            Check(
                "bundled remote config is detached and non-authoritative",
                () =>
                {
                    RemoteConfigLoadResult result = LoadBundledRemoteConfig();
                    return
                        result != null &&
                        result.Succeeded &&
                        !result.RuntimeApplied &&
                        !result.Snapshot.IsAuthoritative &&
                        result.Snapshot.ProposalOnly &&
                        !result.Snapshot.IsFallback;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "all ten remote config flags remain disabled",
                () => EveryRemoteConfigFlagIsFalse(
                    LoadBundledRemoteConfig()?.Snapshot
                ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "unsafe remote enablement falls back atomically",
                UnsafeRemoteConfigFallsBack,
                ref passed,
                ref total,
                failures
            );
            Check(
                "restricted consent default is valid and non-granting",
                () =>
                {
                    ConsentValidationResult validation =
                        ConsentSnapshotValidator.Validate(ConsentSnapshot.Restricted);
                    return
                        validation.IsValid &&
                        ConsentSnapshot.Restricted.Audience == ConsentAudience.Unknown &&
                        ConsentSnapshot.Restricted.Analytics == ConsentDecision.Unknown &&
                        ConsentSnapshot.Restricted.RewardedAdvertising ==
                            ConsentDecision.Unknown;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "disabled consent provider always returns restricted",
                () =>
                {
                    ConsentProviderResult result =
                        DisabledConsentProvider.Instance.GetConsent();
                    return
                        result != null &&
                        !result.IsReady &&
                        result.Status == ConsentProviderStatus.Disabled &&
                        ReferenceEquals(result.Snapshot, ConsentSnapshot.Restricted);
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "external activation short-circuits consent provider access",
                ExternalActivationShortCircuits,
                ref passed,
                ref total,
                failures
            );
            Check(
                "general-audience grants remain purpose-specific",
                ConsentPurposesAreIndependent,
                ref passed,
                ref total,
                failures
            );
            Check(
                "child and unknown audiences remain restricted",
                RestrictedAudiencesStayRestricted,
                ref passed,
                ref total,
                failures
            );
            Check(
                "default purchase and ad adapters are unavailable",
                DefaultAdaptersAreUnavailable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "development adapters require explicit availability and scripts",
                DevelopmentAdaptersRequireExplicitSetup,
                ref passed,
                ref total,
                failures
            );
            Check(
                "unverified purchase never reserves entitlement",
                UnverifiedPurchaseCannotReserve,
                ref passed,
                ref total,
                failures
            );
            Check(
                "verified purchase reserves and completes exactly once",
                VerifiedPurchaseCompletesOnce,
                ref passed,
                ref total,
                failures
            );
            Check(
                "unavailable rewarded ad terminates without entitlement",
                UnavailableAdCannotReserve,
                ref passed,
                ref total,
                failures
            );
            Check(
                "verified rewarded completion reserves exactly once",
                VerifiedRewardedAdCompletesOnce,
                ref passed,
                ref total,
                failures
            );
            Check(
                "entitlement reservations contain no reward amount or action",
                ReservationHasNoFulfillmentRecipe,
                ref passed,
                ref total,
                failures
            );
            Check(
                "save version and PlayerPrefs key remain unchanged",
                SaveIdentityIsUnchanged,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.16 Step 1 commercial adapter sandbox self-tests passed: " +
                    passed + "/" + total +
                    ". Commercial systems remain detached and inactive."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        "v1.16 Step 1 self-tests passed: " + passed + "/" + total +
                        "\nCommercial systems remain detached and inactive.",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                "v1.16 Step 1 self-tests failed: " + passed + "/" + total +
                "\n" + string.Join("\n", failures)
            );
            if (Application.isBatchMode)
                throw new InvalidOperationException("v1.16 Step 1 self-tests failed.");
        }

        static bool InvalidAnalyticsFailsClosed()
        {
            AnalyticsRecordResult unknown = AnalyticsEventValidator.ValidateDraft(
                new AnalyticsEventDraft("unknown_event")
            );
            AnalyticsRecordResult freeForm = AnalyticsEventValidator.ValidateDraft(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.ScreenView,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.ScreenId,
                        "email@example.com"
                    )
                )
            );
            return
                !unknown.Accepted &&
                unknown.ReasonCode == AnalyticsReasonCodes.EventUnknown &&
                !freeForm.Accepted &&
                freeForm.ReasonCode == AnalyticsReasonCodes.StringValueInvalid;
        }

        static bool OfflineAnalyticsIsDeterministic()
        {
            var adapter = new DeterministicOfflineAnalyticsAdapter();
            var pipeline = new AnalyticsPipeline(true, adapter);
            AnalyticsRecordResult started = pipeline.TryBeginSession(
                "session_1",
                FixedTicks
            );
            AnalyticsRecordResult recorded = pipeline.TryRecord(
                new AnalyticsEventDraft(
                    AnalyticsEventIds.LevelStart,
                    AnalyticsAttribute.String(
                        AnalyticsAttributeKeys.LevelId,
                        "chapter01.level01"
                    ),
                    AnalyticsAttribute.Boolean(AnalyticsAttributeKeys.IsRetry, false),
                    AnalyticsAttribute.Integer(
                        AnalyticsAttributeKeys.AttemptNumber,
                        1L
                    )
                ),
                FixedTicks + 1L
            );
            IReadOnlyList<AnalyticsEventEnvelope> records = adapter.Snapshot();
            string firstTrace = adapter.CanonicalSnapshot();
            string secondTrace = adapter.CanonicalSnapshot();
            return
                started.Accepted &&
                recorded.Accepted &&
                records.Count == 2 &&
                records[1].Sequence == 2L &&
                records[1].Attributes[0].Key ==
                    AnalyticsAttributeKeys.AttemptNumber &&
                firstTrace == secondTrace &&
                firstTrace.Contains("\"sequence\":2") &&
                firstTrace.Contains("\"eventId\":\"level_start\"");
        }

        static bool FunnelAcceptsOneTerminal()
        {
            if (!AnalyticsAttemptKey.TryCreate(
                "chapter01.level01",
                1L,
                out string key
            ))
            {
                return false;
            }
            var tracker = new AnalyticsFunnelTracker();
            AnalyticsRecordResult begin = tracker.TryBeginLevelAttempt(key);
            AnalyticsRecordResult complete = tracker.TryCompleteLevelAttempt(key);
            AnalyticsRecordResult duplicate = tracker.TryFailLevelAttempt(key);
            return
                begin.Accepted &&
                complete.Accepted &&
                !duplicate.Accepted &&
                duplicate.ReasonCode == AnalyticsReasonCodes.FunnelTerminalDuplicate &&
                tracker.AcceptedTerminalCount == 1 &&
                tracker.TerminalOutcome == AnalyticsLevelTerminalOutcome.Complete;
        }

        static bool FunnelFailureIsAtomic()
        {
            AnalyticsAttemptKey.TryCreate("chapter01.level01", 1L, out string first);
            AnalyticsAttemptKey.TryCreate("chapter01.level02", 1L, out string second);
            var tracker = new AnalyticsFunnelTracker();
            tracker.TryBeginLevelAttempt(first);
            AnalyticsRecordResult mismatch = tracker.TryCompleteLevelAttempt(second);
            return
                !mismatch.Accepted &&
                mismatch.ReasonCode == AnalyticsReasonCodes.FunnelAttemptMismatch &&
                tracker.ActiveAttemptKey == first &&
                !tracker.HasTerminalOutcome &&
                tracker.AcceptedTerminalCount == 0;
        }

        static RemoteConfigLoadResult LoadBundledRemoteConfig()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                DeterministicLocalRemoteConfigProvider.ResourcePath
            );
            return asset == null
                ? null
                : new DeterministicLocalRemoteConfigProvider(asset.text).LoadSnapshot();
        }

        static bool BundledRemoteConfigMatches()
        {
            RemoteConfigLoadResult result = LoadBundledRemoteConfig();
            return
                result != null &&
                result.Succeeded &&
                result.SourceSha256 ==
                    DeterministicLocalRemoteConfigProvider.ExpectedBundledSourceSha256 &&
                result.ContentSha256 ==
                    "3f4229158b3f5516a536ccf31954f417f8be45a64edd2e4cc33946c2d42e07eb" &&
                result.Snapshot.EconomyCatalogId ==
                    RemoteConfigValidator.RequiredEconomyCatalogId &&
                result.Snapshot.EconomyCatalogSourceSha256 ==
                    RemoteConfigValidator.RequiredEconomyCatalogSourceSha256;
        }

        static bool EveryRemoteConfigFlagIsFalse(RemoteConfigSnapshot snapshot)
        {
            if (snapshot == null)
                return false;
            string[] keys = RemoteConfigAllowlist.GetRequiredKeys();
            if (keys.Length != 10 || snapshot.Count != 10)
                return false;
            for (int i = 0; i < keys.Length; i++)
            {
                if (!snapshot.TryGetBoolean(keys[i], out bool enabled) || enabled)
                    return false;
            }
            return true;
        }

        static bool UnsafeRemoteConfigFallsBack()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                DeterministicLocalRemoteConfigProvider.ResourcePath
            );
            if (asset == null)
                return false;
            string mutated = asset.text.Replace(
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"false\" }",
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"true\" }"
            );
            var provider = new DeterministicLocalRemoteConfigProvider(
                mutated,
                RemoteConfigSourceHash.ComputeNormalizedSha256(mutated)
            );
            RemoteConfigLoadResult result = provider.LoadSnapshot();
            return
                !result.Succeeded &&
                !result.RuntimeApplied &&
                ReferenceEquals(
                    result.Snapshot,
                    RemoteConfigSnapshot.ConservativeFallback
                ) &&
                EveryRemoteConfigFlagIsFalse(result.Snapshot);
        }

        static bool ExternalActivationShortCircuits()
        {
            var provider = new CountingConsentProvider(GeneralConsent(
                ConsentDecision.Granted,
                ConsentDecision.Granted
            ));
            ConsentEligibilityResult analytics = ConsentEligibility.ForAnalytics(
                provider,
                false
            );
            ConsentEligibilityResult ads = ConsentEligibility.ForRewardedAdvertising(
                provider,
                false
            );
            return
                !analytics.IsEligible &&
                !ads.IsEligible &&
                analytics.ReasonCode == ConsentReasonCodes.ExternalActivationDisabled &&
                ads.ReasonCode == ConsentReasonCodes.ExternalActivationDisabled &&
                provider.Calls == 0;
        }

        static bool ConsentPurposesAreIndependent()
        {
            var analyticsOnly = new InMemoryDevelopmentConsentProvider(
                GeneralConsent(ConsentDecision.Granted, ConsentDecision.Denied)
            );
            var adsOnly = new InMemoryDevelopmentConsentProvider(
                GeneralConsent(ConsentDecision.Denied, ConsentDecision.Granted)
            );
            return
                ConsentEligibility.ForAnalytics(analyticsOnly, true).IsEligible &&
                !ConsentEligibility.ForRewardedAdvertising(
                    analyticsOnly,
                    true
                ).IsEligible &&
                !ConsentEligibility.ForAnalytics(adsOnly, true).IsEligible &&
                ConsentEligibility.ForRewardedAdvertising(adsOnly, true).IsEligible;
        }

        static bool RestrictedAudiencesStayRestricted()
        {
            var child = new InMemoryDevelopmentConsentProvider(
                new ConsentSnapshot(
                    1,
                    1,
                    ConsentAudience.Child,
                    ConsentDecision.Denied,
                    ConsentDecision.Denied
                )
            );
            var unknown = new InMemoryDevelopmentConsentProvider(
                ConsentSnapshot.Restricted
            );
            return
                !ConsentEligibility.ForAnalytics(child, true).IsEligible &&
                !ConsentEligibility.ForRewardedAdvertising(child, true).IsEligible &&
                !ConsentEligibility.ForAnalytics(unknown, true).IsEligible &&
                !ConsentEligibility.ForRewardedAdvertising(unknown, true).IsEligible;
        }

        static ConsentSnapshot GeneralConsent(
            ConsentDecision analytics,
            ConsentDecision advertising
        )
        {
            return new ConsentSnapshot(
                ConsentSnapshot.CurrentSchemaVersion,
                ConsentSnapshot.CurrentPolicyVersion,
                ConsentAudience.GeneralAudience,
                analytics,
                advertising
            );
        }

        static bool DefaultAdaptersAreUnavailable()
        {
            var iap = new DefaultUnavailableIapAdapter();
            var ad = new DefaultUnavailableRewardedAdAdapter();
            PurchaseResult purchase = iap.BeginPurchase(PurchaseRequest());
            RewardedAdResult rewarded = ad.Show(RewardedRequest());
            return
                purchase.State == PurchaseState.Unavailable &&
                purchase.Evidence == null &&
                rewarded.State == RewardedAdState.Unavailable &&
                rewarded.Evidence == null;
        }

        static bool DevelopmentAdaptersRequireExplicitSetup()
        {
            var iap = new ScriptedDevelopmentIapAdapter();
            var ad = new ScriptedDevelopmentRewardedAdAdapter();
            PurchaseResult purchase = iap.BeginPurchase(PurchaseRequest());
            RewardedAdResult rewarded = ad.Show(RewardedRequest());
            return
                purchase.State == PurchaseState.Unavailable &&
                purchase.ReasonCode == MonetizationReasonCodes.AdapterUnavailable &&
                rewarded.State == RewardedAdState.Unavailable &&
                rewarded.ReasonCode == MonetizationReasonCodes.AdapterUnavailable;
        }

        static bool UnverifiedPurchaseCannotReserve()
        {
            PurchaseFixture fixture = BuildPurchaseFixture(false);
            var gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                fixture.Verifier,
                new FixedRewardedVerifier()
            );
            EntitlementEligibilityResult result = gate.TryReservePurchase(
                fixture.Request,
                fixture.Result
            );
            var unbound = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                null,
                new FixedRewardedVerifier()
            );
            EntitlementEligibilityResult unboundResult =
                unbound.TryReservePurchase(
                    fixture.Request,
                    fixture.Result
                );
            return
                !result.IsEligible &&
                result.Reservation == null &&
                gate.ReservedCount == 0 &&
                gate.CompletedCount == 0 &&
                !unbound.IsOperational &&
                !unboundResult.IsEligible &&
                unboundResult.ReasonCode ==
                    MonetizationReasonCodes.VerifierUnavailable &&
                unbound.ReservedCount == 0;
        }

        static bool VerifiedPurchaseCompletesOnce()
        {
            PurchaseFixture fixture = BuildPurchaseFixture(true);
            var gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                fixture.Verifier,
                new FixedRewardedVerifier()
            );
            EntitlementEligibilityResult first = gate.TryReservePurchase(
                fixture.Request,
                fixture.Result
            );
            if (!first.IsEligible || first.Reservation == null)
                return false;
            bool completed = gate.Complete(first.Reservation);
            EntitlementEligibilityResult duplicate = gate.TryReservePurchase(
                fixture.Request,
                fixture.Result
            );
            return
                completed &&
                !duplicate.IsEligible &&
                duplicate.ReasonCode == MonetizationReasonCodes.DuplicateEntitlement &&
                gate.CompletedCount == 1;
        }

        static bool UnavailableAdCannotReserve()
        {
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentRewardedAdScript.Unavailable());
            RewardedAdRequest request = RewardedRequest();
            RewardedAdResult result = adapter.Show(request);
            var gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new FixedPurchaseVerifier(true),
                new FixedRewardedVerifier()
            );
            EntitlementEligibilityResult reservation = gate.TryReserveRewardedAd(
                request,
                result
            );
            return
                result.State == RewardedAdState.Unavailable &&
                result.Evidence == null &&
                !adapter.IsBusy &&
                !reservation.IsEligible &&
                gate.ReservedCount == 0;
        }

        static bool VerifiedRewardedAdCompletesOnce()
        {
            RewardedFixture fixture = BuildRewardedFixture();
            var gate = new VerifiedEntitlementGate(
                MonetizationEnvironment.DevelopmentSandbox,
                new FixedPurchaseVerifier(true),
                fixture.Verifier
            );
            EntitlementEligibilityResult first = gate.TryReserveRewardedAd(
                fixture.Request,
                fixture.Result
            );
            if (!first.IsEligible || first.Reservation == null)
                return false;
            bool completed = gate.Complete(first.Reservation);
            EntitlementEligibilityResult duplicate = gate.TryReserveRewardedAd(
                fixture.Request,
                fixture.Result
            );
            return
                completed &&
                !duplicate.IsEligible &&
                duplicate.ReasonCode == MonetizationReasonCodes.DuplicateEntitlement &&
                gate.CompletedCount == 1;
        }

        static bool ReservationHasNoFulfillmentRecipe()
        {
            string[] forbidden =
            {
                "amount", "quantity", "currency", "gold", "gems", "energy",
                "grant", "action"
            };
            PropertyInfo[] properties = typeof(EntitlementReservation).GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            );
            for (int i = 0; i < properties.Length; i++)
            {
                string name = properties[i].Name.ToLowerInvariant();
                for (int j = 0; j < forbidden.Length; j++)
                {
                    if (name.Contains(forbidden[j]))
                        return false;
                }
            }
            return true;
        }

        static bool SaveIdentityIsUnchanged()
        {
            FieldInfo keyField = typeof(SaveService).GetField(
                "Key",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                new PlayerSaveData().saveVersion == 6 &&
                keyField != null &&
                (string)keyField.GetRawConstantValue() == "LegendsOfRus_Save_v1";
        }

        static PurchaseRequest PurchaseRequest()
        {
            return new PurchaseRequest(
                "selftest_purchase_1",
                MonetizationAllowlists.StarterOfferId,
                MonetizationAllowlists.StarterProductId
            );
        }

        static RewardedAdRequest RewardedRequest()
        {
            return new RewardedAdRequest(
                "selftest_ad_1",
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "selftest_opportunity_1"
            );
        }

        static PurchaseFixture BuildPurchaseFixture(bool verified)
        {
            PurchaseRequest request = PurchaseRequest();
            var adapter = new ScriptedDevelopmentIapAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(DevelopmentPurchaseScript.Purchased("selftest_transaction_1"));
            PurchaseResult result = adapter.BeginPurchase(request);
            return new PurchaseFixture
            {
                Request = request,
                Result = result,
                Verifier = new FixedPurchaseVerifier(verified),
            };
        }

        static RewardedFixture BuildRewardedFixture()
        {
            RewardedAdRequest request = RewardedRequest();
            var adapter = new ScriptedDevelopmentRewardedAdAdapter();
            adapter.SetAvailable(true);
            adapter.Enqueue(
                DevelopmentRewardedAdScript.Completed("selftest_reward_token_1")
            );
            RewardedAdResult result = adapter.Show(request);
            return new RewardedFixture
            {
                Request = request,
                Result = result,
                Verifier = new FixedRewardedVerifier(),
            };
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

        sealed class CountingConsentProvider : IConsentProvider
        {
            readonly ConsentProviderResult result;
            public int Calls;

            public CountingConsentProvider(ConsentSnapshot snapshot)
            {
                result = ConsentProviderResult.Ready(snapshot);
            }

            public ConsentProviderResult GetConsent()
            {
                Calls++;
                return result;
            }
        }

        sealed class PurchaseFixture
        {
            public PurchaseRequest Request;
            public PurchaseResult Result;
            public IPurchaseVerifier Verifier;
        }

        sealed class RewardedFixture
        {
            public RewardedAdRequest Request;
            public RewardedAdResult Result;
            public IRewardedAdVerifier Verifier;
        }

        sealed class FixedPurchaseVerifier : IPurchaseVerifier
        {
            readonly bool verified;

            public FixedPurchaseVerifier(bool verified)
            {
                this.verified = verified;
            }

            public PurchaseVerificationResult Verify(PurchaseEvidence evidence)
            {
                return verified
                    ? PurchaseVerificationResult.Verified(evidence)
                    : PurchaseVerificationResult.Rejected(evidence);
            }
        }

        sealed class FixedRewardedVerifier : IRewardedAdVerifier
        {
            public RewardedAdVerificationResult Verify(
                RewardedAdEvidence evidence
            )
            {
                return RewardedAdVerificationResult.Verified(evidence);
            }
        }
    }
}
#endif
