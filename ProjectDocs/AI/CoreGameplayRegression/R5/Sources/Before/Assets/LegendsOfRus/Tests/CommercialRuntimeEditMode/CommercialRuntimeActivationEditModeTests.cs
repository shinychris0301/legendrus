using System;
using System.Reflection;
using LegendsOfRus.CommercialCore.RemoteConfig;
using LegendsOfRus.CommercialRuntime;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.CommercialRuntime
{
    public sealed class CommercialRuntimeActivationEditModeTests
    {
        [Test]
        public void ShippedConfig_IsPinnedAndEntirelyDisabled()
        {
            CommercialRuntimeConfigLoadResult result =
                CommercialRuntimeConfigLoader.Load();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.True, result.ReasonCode);
            Assert.That(result.ReasonCode, Is.EqualTo(CommercialRuntimeReasonCodes.None));
            Assert.That(result.Snapshot.IsFallback, Is.False);
            Assert.That(result.Snapshot.ConfigId,
                Is.EqualTo(CommercialRuntimeConfigLoader.RequiredConfigId));
            Assert.That(result.Snapshot.Environment, Is.EqualTo("disabled"));
            Assert.That(result.Snapshot.ConsentProviderId, Is.EqualTo("disabled"));
            Assert.That(result.Snapshot.AnalyticsProviderId, Is.EqualTo("disabled"));
            Assert.That(result.Snapshot.IapProviderId, Is.EqualTo("disabled"));
            Assert.That(result.Snapshot.RewardedAdProviderId, Is.EqualTo("disabled"));
            Assert.That(result.Snapshot.AnalyticsCaptureAllowed, Is.False);
            Assert.That(result.Snapshot.AnalyticsUploadAllowed, Is.False);
            Assert.That(result.Snapshot.IapAllowed, Is.False);
            Assert.That(result.Snapshot.RewardedAdsAllowed, Is.False);
            Assert.That(result.Snapshot.DevelopmentSandboxAllowed, Is.False);
            Assert.That(result.Snapshot.SourceSha256,
                Is.EqualTo(CommercialRuntimeConfigLoader.ExpectedSourceSha256));
        }

        [Test]
        public void ConfigMutation_IsRejectedBeforeItCanEnableAnything()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeConfigLoader.ResourcePath
            );
            Assert.That(asset, Is.Not.Null);

            string mutated = asset.text.Replace(
                "\"analyticsCaptureAllowed\": false",
                "\"analyticsCaptureAllowed\": true"
            );
            CommercialRuntimeConfigLoadResult result =
                CommercialRuntimeConfigLoader.Evaluate(mutated);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ReasonCode,
                Is.EqualTo(CommercialRuntimeReasonCodes.ConfigSourceHashMismatch));
            Assert.That(result.Snapshot.IsFallback, Is.True);
            Assert.That(result.Snapshot.AnalyticsCaptureAllowed, Is.False);
        }

        [Test]
        public void RemoteConfig_FirstFailureUsesConservativeFallback()
        {
            var cache = new RuntimeRemoteConfigCache();

            RuntimeRemoteConfigUpdate update = cache.TryRefresh(null);

            Assert.That(update.Accepted, Is.False);
            Assert.That(update.RetainedLastKnownGood, Is.False);
            Assert.That(update.UsesConservativeFallback, Is.True);
            Assert.That(cache.HasLastKnownGood, Is.False);
            Assert.That(cache.Current.IsFallback, Is.True);
        }

        [Test]
        public void RemoteConfig_BundledSnapshotBecomesProcessLocalLkg()
        {
            var cache = new RuntimeRemoteConfigCache();

            RuntimeRemoteConfigUpdate update = cache.LoadBundled();

            Assert.That(update.Accepted, Is.True, update.ReasonCode);
            Assert.That(update.RetainedLastKnownGood, Is.False);
            Assert.That(update.UsesConservativeFallback, Is.False);
            Assert.That(cache.HasLastKnownGood, Is.True);
            Assert.That(update.EffectiveSnapshot.ProposalOnly, Is.True);
            Assert.That(update.EffectiveSnapshot.IsAuthoritative, Is.False);
            Assert.That(update.EffectiveSnapshot.ContentSha256,
                Is.EqualTo(RuntimeRemoteConfigCache.ExpectedContentSha256));
        }

        [Test]
        public void RemoteConfig_ThrowAfterSuccessRetainsSameLkgAtomically()
        {
            var cache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate loaded = cache.LoadBundled();
            Assert.That(loaded.Accepted, Is.True, loaded.ReasonCode);
            RemoteConfigSnapshot before = cache.Current;

            RuntimeRemoteConfigUpdate failed = cache.TryRefresh(
                new ThrowingRemoteConfigProvider()
            );

            Assert.That(failed.Accepted, Is.False);
            Assert.That(failed.RetainedLastKnownGood, Is.True);
            Assert.That(failed.ReasonCode,
                Is.EqualTo(CommercialRuntimeReasonCodes.RemoteConfigProviderFailed));
            Assert.That(failed.EffectiveSnapshot, Is.SameAs(before));
            Assert.That(cache.Current, Is.SameAs(before));
        }

        [Test]
        public void ShippedManifest_ContainsNoApprovalOrActivationEvidence()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeActivationGate.ResourcePath
            );
            Assert.That(asset, Is.Not.Null);
            var data = JsonUtility.FromJson<CommercialRuntimeActivationManifestData>(
                asset.text
            );

            Assert.That(data, Is.Not.Null);
            Assert.That(data.environment, Is.EqualTo("disabled"));
            Assert.That(data.v115RuntimeEconomyApproved, Is.False);
            Assert.That(data.v116Step1CompilationVerified, Is.False);
            Assert.That(data.v116Step1ProjectValidatorPassed, Is.False);
            Assert.That(data.v116Step1EditorSelfTestsPassed, Is.False);
            Assert.That(data.v116Step1EditModeTestsPassed, Is.False);
            Assert.That(data.v116Step1PlayModeTestsPassed, Is.False);
            Assert.That(data.providerSelectionApproved, Is.False);
            Assert.That(data.legalPrivacyApproved, Is.False);
            Assert.That(data.securityReviewPassed, Is.False);
            Assert.That(data.saveMigrationApproved, Is.False);
            Assert.That(data.durableFulfillmentApproved, Is.False);
            Assert.That(data.analyticsActivationApproved, Is.False);
            Assert.That(data.iapActivationApproved, Is.False);
            Assert.That(data.rewardedAdsActivationApproved, Is.False);
            Assert.That(data.finalRuntimeActivationApproved, Is.False);
        }

        [Test]
        public void ShippedActivation_IsBlockedByInactiveV115Economy()
        {
            CommercialRuntimeConfigLoadResult config =
                CommercialRuntimeConfigLoader.Load();
            var cache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate remote = cache.LoadBundled();

            CommercialRuntimeActivationDecision decision =
                CommercialRuntimeActivationGate.Load(config, remote);

            Assert.That(decision.Phase,
                Is.EqualTo(CommercialRuntimePhase.VerificationRequired));
            Assert.That(decision.ReasonCode,
                Is.EqualTo(CommercialRuntimeReasonCodes.V115EconomyInactive));
            Assert.That(decision.IsRuntimeActivated, Is.False);
            Assert.That(decision.AnalyticsApproved, Is.False);
            Assert.That(decision.IapApproved, Is.False);
            Assert.That(decision.RewardedAdsApproved, Is.False);
            Assert.That(decision.ManifestSourceSha256,
                Is.EqualTo(CommercialRuntimeActivationGate.ExpectedManifestSourceSha256));
        }

        [Test]
        public void ActivationManifestMutation_FailsClosed()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeActivationGate.ResourcePath
            );
            Assert.That(asset, Is.Not.Null);
            string mutated = asset.text.Replace(
                "\"finalRuntimeActivationApproved\": false",
                "\"finalRuntimeActivationApproved\": true"
            );
            CommercialRuntimeActivationDecision decision =
                CommercialRuntimeActivationGate.Evaluate(
                    mutated,
                    CommercialRuntimeActivationGate.RequiredUnityVersion,
                    CommercialRuntimeConfigLoader.Load(),
                    new RuntimeRemoteConfigCache().LoadBundled(),
                    RuntimeEconomyActivationGate.Load(),
                    true
                );

            Assert.That(decision.IsRuntimeActivated, Is.False);
            Assert.That(decision.Phase, Is.EqualTo(CommercialRuntimePhase.Disabled));
            Assert.That(decision.ReasonCode,
                Is.EqualTo(CommercialRuntimeReasonCodes.ManifestSourceHashMismatch));
        }

        [Test]
        public void ExternalProviderActivationBuildSwitch_IsFalse()
        {
            FieldInfo field = typeof(CommercialRuntimeActivationGate).GetField(
                "ExternalProviderActivationCompiled",
                BindingFlags.Static | BindingFlags.NonPublic
            );

            Assert.That(field, Is.Not.Null);
            Assert.That(field.IsLiteral, Is.True);
            Assert.That(field.GetRawConstantValue(), Is.EqualTo(false));
        }

        [Test]
        public void SafeDisabledComposition_HasNoRuntimeCapability()
        {
            CommercialRuntimeComposition composition =
                CommercialRuntimeComposition.SafeDisabled();

            AssertAllDisabled(composition);
            Assert.That(composition.Activation.Phase,
                Is.EqualTo(CommercialRuntimePhase.Uninitialized));
            Assert.That(composition.HasRemoteConfigLastKnownGood, Is.False);
        }

        [Test]
        public void TrustedShippedComposition_UsesOnlyDisabledProviders()
        {
            CommercialRuntimeComposition composition =
                CommercialRuntimeComposition.BuildTrustedDisabled();

            AssertAllDisabled(composition);
            Assert.That(composition.Activation.Phase,
                Is.EqualTo(CommercialRuntimePhase.VerificationRequired));
            Assert.That(composition.HasRemoteConfigLastKnownGood, Is.True);
            Assert.That(composition.Providers.ConsentProviderId, Is.EqualTo("disabled"));
            Assert.That(composition.Providers.AnalyticsAdapterId, Is.EqualTo("disabled"));
            Assert.That(composition.Providers.IapProviderId, Is.EqualTo("disabled"));
            Assert.That(composition.Providers.RewardedAdProviderId, Is.EqualTo("disabled"));
            Assert.That(composition.Providers.ConsentRestricted, Is.True);
            Assert.That(composition.Providers.AnalyticsAdapterDisabled, Is.True);
            Assert.That(composition.Providers.IapAdapterDisabled, Is.True);
            Assert.That(composition.Providers.PurchaseVerifierUnavailable, Is.True);
            Assert.That(composition.Providers.RewardedAdAdapterDisabled, Is.True);
            Assert.That(composition.Providers.RewardedAdVerifierUnavailable, Is.True);
        }

        static void AssertAllDisabled(CommercialRuntimeComposition composition)
        {
            Assert.That(composition, Is.Not.Null);
            Assert.That(composition.Activation.IsRuntimeActivated, Is.False);
            Assert.That(composition.Capabilities.AnalyticsCaptureEnabled, Is.False);
            Assert.That(composition.Capabilities.AnalyticsUploadEnabled, Is.False);
            Assert.That(composition.Capabilities.IapEnabled, Is.False);
            Assert.That(composition.Capabilities.RewardedAdsEnabled, Is.False);
            Assert.That(composition.Capabilities.DurableFulfillmentEnabled, Is.False);
            Assert.That(composition.PreservesOfflineCoreLoop, Is.True);
            Assert.That(composition.PerformsExternalCommunication, Is.False);
            Assert.That(composition.PerformsSaveWrites, Is.False);
        }

        sealed class ThrowingRemoteConfigProvider : IRemoteConfigProvider
        {
            public RemoteConfigLoadResult LoadSnapshot()
            {
                throw new InvalidOperationException("test-only failure");
            }
        }
    }
}
