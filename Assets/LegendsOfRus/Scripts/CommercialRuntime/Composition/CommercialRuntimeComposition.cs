using LegendsOfRus.CommercialCore.Analytics;
using LegendsOfRus.CommercialCore.Consent;
using LegendsOfRus.CommercialCore.Monetization;
using LegendsOfRus.CommercialCore.RemoteConfig;

namespace LegendsOfRus.CommercialRuntime
{
    public sealed class CommercialRuntimeCapabilities
    {
        public bool AnalyticsCaptureEnabled { get; private set; }
        public bool AnalyticsUploadEnabled { get; private set; }
        public bool IapEnabled { get; private set; }
        public bool RewardedAdsEnabled { get; private set; }
        public bool DurableFulfillmentEnabled { get; private set; }

        internal CommercialRuntimeCapabilities(
            bool analyticsCaptureEnabled,
            bool analyticsUploadEnabled,
            bool iapEnabled,
            bool rewardedAdsEnabled,
            bool durableFulfillmentEnabled
        )
        {
            AnalyticsCaptureEnabled = analyticsCaptureEnabled;
            AnalyticsUploadEnabled = analyticsUploadEnabled;
            IapEnabled = iapEnabled;
            RewardedAdsEnabled = rewardedAdsEnabled;
            DurableFulfillmentEnabled = durableFulfillmentEnabled;
        }

        internal static CommercialRuntimeCapabilities AllDisabled()
        {
            return new CommercialRuntimeCapabilities(
                false,
                false,
                false,
                false,
                false
            );
        }
    }

    public sealed class CommercialRuntimeProviderDiagnostics
    {
        public string ConsentProviderId { get; private set; }
        public string AnalyticsAdapterId { get; private set; }
        public string IapProviderId { get; private set; }
        public string RewardedAdProviderId { get; private set; }
        public bool ConsentRestricted { get; private set; }
        public bool AnalyticsAdapterDisabled { get; private set; }
        public bool IapAdapterDisabled { get; private set; }
        public bool PurchaseVerifierUnavailable { get; private set; }
        public bool RewardedAdAdapterDisabled { get; private set; }
        public bool RewardedAdVerifierUnavailable { get; private set; }

        internal CommercialRuntimeProviderDiagnostics(
            string consentProviderId,
            string analyticsAdapterId,
            string iapProviderId,
            string rewardedAdProviderId,
            bool consentRestricted,
            bool analyticsAdapterDisabled,
            bool iapAdapterDisabled,
            bool purchaseVerifierUnavailable,
            bool rewardedAdAdapterDisabled,
            bool rewardedAdVerifierUnavailable
        )
        {
            ConsentProviderId = consentProviderId ?? "disabled";
            AnalyticsAdapterId = analyticsAdapterId ?? "disabled";
            IapProviderId = iapProviderId ?? "disabled";
            RewardedAdProviderId = rewardedAdProviderId ?? "disabled";
            ConsentRestricted = consentRestricted;
            AnalyticsAdapterDisabled = analyticsAdapterDisabled;
            IapAdapterDisabled = iapAdapterDisabled;
            PurchaseVerifierUnavailable = purchaseVerifierUnavailable;
            RewardedAdAdapterDisabled = rewardedAdAdapterDisabled;
            RewardedAdVerifierUnavailable = rewardedAdVerifierUnavailable;
        }

        internal static CommercialRuntimeProviderDiagnostics AllDisabled()
        {
            return new CommercialRuntimeProviderDiagnostics(
                "disabled",
                "disabled",
                "disabled",
                "disabled",
                true,
                true,
                true,
                true,
                true,
                true
            );
        }
    }

    /// <summary>
    /// Immutable result of one trusted boot-time composition. It exposes only
    /// decisions and diagnostics, never raw providers, verifiers, receipts, or
    /// mutable configuration. The shipped build always resolves all external
    /// capabilities to false.
    /// </summary>
    public sealed class CommercialRuntimeComposition
    {
        public CommercialRuntimeActivationDecision Activation { get; private set; }
        public CommercialRuntimeConfigSnapshot LocalConfig { get; private set; }
        public RemoteConfigSnapshot RemoteConfig { get; private set; }
        public CommercialRuntimeCapabilities Capabilities { get; private set; }
        public CommercialRuntimeProviderDiagnostics Providers { get; private set; }
        public string LocalConfigReasonCode { get; private set; }
        public string RemoteConfigReasonCode { get; private set; }
        public bool HasRemoteConfigLastKnownGood { get; private set; }
        public bool PreservesOfflineCoreLoop { get { return true; } }
        public bool PerformsExternalCommunication { get { return false; } }
        public bool PerformsSaveWrites { get { return false; } }

        internal CommercialRuntimeComposition(
            CommercialRuntimeActivationDecision activation,
            CommercialRuntimeConfigSnapshot localConfig,
            RemoteConfigSnapshot remoteConfig,
            CommercialRuntimeCapabilities capabilities,
            CommercialRuntimeProviderDiagnostics providers,
            string localConfigReasonCode,
            string remoteConfigReasonCode,
            bool hasRemoteConfigLastKnownGood
        )
        {
            Activation = activation ?? CommercialRuntimeActivationDecision.Uninitialized;
            LocalConfig = localConfig ??
                CommercialRuntimeConfigSnapshot.ConservativeDisabled;
            RemoteConfig = remoteConfig ?? RemoteConfigSnapshot.ConservativeFallback;
            Capabilities = capabilities ?? CommercialRuntimeCapabilities.AllDisabled();
            Providers = providers ?? CommercialRuntimeProviderDiagnostics.AllDisabled();
            LocalConfigReasonCode = localConfigReasonCode ??
                CommercialRuntimeReasonCodes.ConfigSourceInvalid;
            RemoteConfigReasonCode = remoteConfigReasonCode ??
                CommercialRuntimeReasonCodes.RemoteConfigRejected;
            HasRemoteConfigLastKnownGood = hasRemoteConfigLastKnownGood;
        }

        public static CommercialRuntimeComposition SafeDisabled()
        {
            return new CommercialRuntimeComposition(
                CommercialRuntimeActivationDecision.Uninitialized,
                CommercialRuntimeConfigSnapshot.ConservativeDisabled,
                RemoteConfigSnapshot.ConservativeFallback,
                CommercialRuntimeCapabilities.AllDisabled(),
                CommercialRuntimeProviderDiagnostics.AllDisabled(),
                CommercialRuntimeReasonCodes.SafeDisabled,
                CommercialRuntimeReasonCodes.RemoteConfigFallback,
                false
            );
        }

        internal static CommercialRuntimeComposition BuildTrustedDisabled()
        {
            CommercialRuntimeConfigLoadResult localResult =
                CommercialRuntimeConfigLoader.Load();
            var remoteCache = new RuntimeRemoteConfigCache();
            RuntimeRemoteConfigUpdate remoteUpdate = remoteCache.LoadBundled();
            CommercialRuntimeActivationDecision activation =
                CommercialRuntimeActivationGate.Load(localResult, remoteUpdate);

            CommercialRuntimeConfigSnapshot local = localResult != null
                ? localResult.Snapshot
                : CommercialRuntimeConfigSnapshot.ConservativeDisabled;
            RemoteConfigSnapshot remote = remoteUpdate != null
                ? remoteUpdate.EffectiveSnapshot
                : RemoteConfigSnapshot.ConservativeFallback;

            IConsentProvider consentProvider = DisabledConsentProvider.Instance;
            IAnalyticsAdapter analyticsAdapter = DisabledAnalyticsAdapter.Instance;
            IIapAdapter iapAdapter = new DefaultUnavailableIapAdapter();
            IPurchaseVerifier purchaseVerifier =
                new DefaultUnavailablePurchaseVerifier();
            IRewardedAdAdapter rewardedAdAdapter =
                new DefaultUnavailableRewardedAdAdapter();
            IRewardedAdVerifier rewardedAdVerifier =
                new DefaultUnavailableRewardedAdVerifier();

            bool analyticsProposal = ReadFlag(
                remote,
                RemoteConfigKeys.AnalyticsCaptureEnabled
            );
            bool iapProposal = ReadFlag(remote, RemoteConfigKeys.IapEnabled);
            bool rewardedProposal = ReadFlag(
                remote,
                RemoteConfigKeys.RewardedAdsEnabled
            );

            bool activationReady = activation != null &&
                activation.IsRuntimeActivated;
            ConsentEligibilityResult analyticsConsent =
                ConsentEligibility.ForAnalytics(
                    consentProvider,
                    activationReady &&
                    local.AnalyticsCaptureAllowed &&
                    analyticsProposal
                );
            ConsentEligibilityResult rewardedConsent =
                ConsentEligibility.ForRewardedAdvertising(
                    consentProvider,
                    activationReady &&
                    local.RewardedAdsAllowed &&
                    rewardedProposal
                );

            // Remote config can only disable an independently approved local
            // capability. Disabled adapters/verifiers are an additional hard
            // stop. Upload and durable fulfillment are unavailable in Step 2.
            bool analyticsEnabled =
                activationReady &&
                activation.AnalyticsApproved &&
                local.AnalyticsCaptureAllowed &&
                analyticsProposal &&
                analyticsConsent.IsEligible &&
                analyticsAdapter.CaptureEnabled &&
                !analyticsAdapter.SupportsUpload;
            bool iapEnabled =
                activationReady &&
                activation.IapApproved &&
                local.IapAllowed &&
                iapProposal &&
                iapAdapter.Environment == MonetizationEnvironment.Production;
            bool rewardedEnabled =
                activationReady &&
                activation.RewardedAdsApproved &&
                local.RewardedAdsAllowed &&
                rewardedProposal &&
                rewardedConsent.IsEligible &&
                rewardedAdAdapter.Environment == MonetizationEnvironment.Production;

            var capabilities = new CommercialRuntimeCapabilities(
                analyticsEnabled,
                false,
                iapEnabled,
                rewardedEnabled,
                false
            );
            var providers = new CommercialRuntimeProviderDiagnostics(
                local.ConsentProviderId,
                analyticsAdapter.AdapterId,
                local.IapProviderId,
                local.RewardedAdProviderId,
                !analyticsConsent.IsEligible || !rewardedConsent.IsEligible,
                !analyticsAdapter.CaptureEnabled,
                iapAdapter.Environment == MonetizationEnvironment.Disabled,
                purchaseVerifier is DefaultUnavailablePurchaseVerifier,
                rewardedAdAdapter.Environment == MonetizationEnvironment.Disabled,
                rewardedAdVerifier is DefaultUnavailableRewardedAdVerifier
            );

            return new CommercialRuntimeComposition(
                activation,
                local,
                remote,
                capabilities,
                providers,
                localResult != null
                    ? localResult.ReasonCode
                    : CommercialRuntimeReasonCodes.ConfigSourceInvalid,
                remoteUpdate != null
                    ? remoteUpdate.ReasonCode
                    : CommercialRuntimeReasonCodes.RemoteConfigRejected,
                remoteCache.HasLastKnownGood
            );
        }

        static bool ReadFlag(RemoteConfigSnapshot snapshot, string key)
        {
            bool value;
            return
                snapshot != null &&
                snapshot.TryGetBoolean(key, out value) &&
                value;
        }
    }
}
