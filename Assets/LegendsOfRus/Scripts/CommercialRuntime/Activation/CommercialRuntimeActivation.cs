using System;
using LegendsOfRus.CommercialCore.RemoteConfig;
using UnityEngine;

namespace LegendsOfRus.CommercialRuntime
{
    public enum CommercialRuntimePhase
    {
        Uninitialized = 0,
        Disabled = 1,
        VerificationRequired = 2,
        ApprovalRequired = 3,
        ReadyForProviderBinding = 4,
        Active = 5
    }

    [Serializable]
    internal sealed class CommercialRuntimeActivationManifestData
    {
        public int schemaVersion;
        public string manifestId = string.Empty;
        public string unityVersion = string.Empty;
        public string environment = string.Empty;
        public string runtimeConfigId = string.Empty;
        public string runtimeConfigSourceSha256 = string.Empty;
        public string economyCatalogId = string.Empty;
        public string economyCatalogSourceSha256 = string.Empty;
        public string remoteConfigSnapshotId = string.Empty;
        public int remoteConfigRevision;
        public string remoteConfigSourceSha256 = string.Empty;
        public string remoteConfigContentSha256 = string.Empty;

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

    public sealed class CommercialRuntimeActivationDecision
    {
        static readonly CommercialRuntimeActivationDecision uninitialized =
            new CommercialRuntimeActivationDecision(
                CommercialRuntimePhase.Uninitialized,
                CommercialRuntimeReasonCodes.NotInitialized,
                false,
                false,
                false,
                false,
                "disabled",
                string.Empty,
                string.Empty
            );

        public static CommercialRuntimeActivationDecision Uninitialized
        {
            get { return uninitialized; }
        }

        public CommercialRuntimePhase Phase { get; private set; }
        public string ReasonCode { get; private set; }
        public bool IsRuntimeActivated { get; private set; }
        public bool AnalyticsApproved { get; private set; }
        public bool IapApproved { get; private set; }
        public bool RewardedAdsApproved { get; private set; }
        public string Environment { get; private set; }
        public string ManifestSourceSha256 { get; private set; }
        public string EconomyReasonCode { get; private set; }

        internal CommercialRuntimeActivationDecision(
            CommercialRuntimePhase phase,
            string reasonCode,
            bool isRuntimeActivated,
            bool analyticsApproved,
            bool iapApproved,
            bool rewardedAdsApproved,
            string environment,
            string manifestSourceSha256,
            string economyReasonCode
        )
        {
            Phase = phase;
            ReasonCode = reasonCode ?? CommercialRuntimeReasonCodes.UnexpectedFailure;
            IsRuntimeActivated = isRuntimeActivated;
            AnalyticsApproved = isRuntimeActivated && analyticsApproved;
            IapApproved = isRuntimeActivated && iapApproved;
            RewardedAdsApproved = isRuntimeActivated && rewardedAdsApproved;
            Environment = environment ?? "disabled";
            ManifestSourceSha256 = manifestSourceSha256 ?? string.Empty;
            EconomyReasonCode = economyReasonCode ?? string.Empty;
        }
    }

    public static class CommercialRuntimeActivationGate
    {
        public const string ResourcePath = "Config/commercial_runtime_activation";
        public const int CurrentSchemaVersion = 1;
        public const string RequiredManifestId =
            "v1.16-step02-commercial-runtime-preactivation";
        public const string RequiredUnityVersion = "6000.5.9f1";
        public const string RequiredEnvironment = "disabled";
        public const string ExpectedManifestSourceSha256 =
            "67f31982d6eafcc54f644f8ad1e81f672ddeefe3856da4eaae6b71af3bb753e9";

        // This pre-activation build cannot activate external providers even if
        // a local JSON file is edited. A later reviewed source change must flip
        // this boundary together with pinned evidence and provider bindings.
        private const bool ExternalProviderActivationCompiled = false;
        const int MaximumSourceCharacters = 32768;

        public static CommercialRuntimeActivationDecision Load(
            CommercialRuntimeConfigLoadResult runtimeConfig,
            RuntimeRemoteConfigUpdate remoteConfig
        )
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            RuntimeEconomyActivationResult economy =
                RuntimeEconomyActivationGate.Load();
            if (asset == null)
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestResourceMissing,
                    string.Empty,
                    economy
                );
            }

            return Evaluate(
                asset.text,
                Application.unityVersion,
                runtimeConfig,
                remoteConfig,
                economy,
                false
            );
        }

        internal static CommercialRuntimeActivationDecision Evaluate(
            string manifestSource,
            string unityVersion,
            CommercialRuntimeConfigLoadResult runtimeConfig,
            RuntimeRemoteConfigUpdate remoteConfig,
            RuntimeEconomyActivationResult economy,
            bool trustedProductionProvidersBound
        )
        {
            if (
                string.IsNullOrEmpty(manifestSource) ||
                manifestSource.Length > MaximumSourceCharacters
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestSourceInvalid,
                    string.Empty,
                    economy
                );
            }

            string manifestHash = CommercialRuntimeSourceHash.Compute(manifestSource);
            if (!string.Equals(
                manifestHash,
                ExpectedManifestSourceSha256,
                StringComparison.Ordinal
            ))
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestSourceHashMismatch,
                    manifestHash,
                    economy
                );
            }

            CommercialRuntimeActivationManifestData manifest;
            try
            {
                manifest = JsonUtility.FromJson<
                    CommercialRuntimeActivationManifestData
                >(manifestSource);
            }
            catch
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestJsonInvalid,
                    manifestHash,
                    economy
                );
            }

            if (manifest == null)
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestJsonInvalid,
                    manifestHash,
                    economy
                );
            }

            if (
                manifest.schemaVersion != CurrentSchemaVersion ||
                !string.Equals(
                    manifest.manifestId,
                    RequiredManifestId,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    manifest.environment,
                    RequiredEnvironment,
                    StringComparison.Ordinal
                )
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.ManifestIdentityMismatch,
                    manifestHash,
                    economy
                );
            }

            if (
                !string.Equals(
                    unityVersion,
                    RequiredUnityVersion,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    manifest.unityVersion,
                    RequiredUnityVersion,
                    StringComparison.Ordinal
                )
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.UnityVersionMismatch,
                    manifestHash,
                    economy
                );
            }

            if (
                runtimeConfig == null ||
                !runtimeConfig.IsValid ||
                runtimeConfig.Snapshot == null ||
                runtimeConfig.Snapshot.IsFallback ||
                !string.Equals(
                    manifest.runtimeConfigId,
                    runtimeConfig.Snapshot.ConfigId,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    manifest.runtimeConfigSourceSha256,
                    runtimeConfig.Snapshot.SourceSha256,
                    StringComparison.Ordinal
                )
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.RuntimeConfigBindingMismatch,
                    manifestHash,
                    economy
                );
            }

            RemoteConfigSnapshot remoteSnapshot = remoteConfig != null
                ? remoteConfig.EffectiveSnapshot
                : null;
            if (
                remoteConfig == null ||
                (!remoteConfig.Accepted && !remoteConfig.RetainedLastKnownGood) ||
                remoteSnapshot == null ||
                remoteSnapshot.IsFallback ||
                remoteSnapshot.IsAuthoritative ||
                !remoteSnapshot.ProposalOnly ||
                !string.Equals(
                    manifest.remoteConfigSnapshotId,
                    remoteSnapshot.SnapshotId,
                    StringComparison.Ordinal
                ) ||
                manifest.remoteConfigRevision != remoteSnapshot.Revision ||
                !string.Equals(
                    manifest.remoteConfigSourceSha256,
                    DeterministicLocalRemoteConfigProvider.ExpectedBundledSourceSha256,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    manifest.remoteConfigContentSha256,
                    remoteSnapshot.ContentSha256,
                    StringComparison.Ordinal
                )
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.RemoteConfigBindingMismatch,
                    manifestHash,
                    economy
                );
            }

            if (
                !string.Equals(
                    manifest.economyCatalogId,
                    RuntimeEconomyActivationGate.CatalogId,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    manifest.economyCatalogSourceSha256,
                    RuntimeEconomyActivationGate.CatalogSourceSha256,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    remoteSnapshot.EconomyCatalogId,
                    manifest.economyCatalogId,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    remoteSnapshot.EconomyCatalogSourceSha256,
                    manifest.economyCatalogSourceSha256,
                    StringComparison.Ordinal
                )
            )
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.EconomyBindingMismatch,
                    manifestHash,
                    economy
                );
            }

            if (
                !manifest.v115RuntimeEconomyApproved ||
                economy == null ||
                !economy.IsRuntimeActivated
            )
            {
                return Rejected(
                    CommercialRuntimePhase.VerificationRequired,
                    CommercialRuntimeReasonCodes.V115EconomyInactive,
                    manifestHash,
                    economy
                );
            }

            if (
                !manifest.v116Step1CompilationVerified ||
                !manifest.v116Step1ProjectValidatorPassed ||
                !manifest.v116Step1EditorSelfTestsPassed ||
                !manifest.v116Step1EditModeTestsPassed ||
                !manifest.v116Step1PlayModeTestsPassed
            )
            {
                return Rejected(
                    CommercialRuntimePhase.VerificationRequired,
                    CommercialRuntimeReasonCodes.StepOneVerificationIncomplete,
                    manifestHash,
                    economy
                );
            }

            if (
                !manifest.providerSelectionApproved ||
                !manifest.legalPrivacyApproved ||
                !manifest.consentFlowApproved ||
                !manifest.productIdentifiersApproved ||
                !manifest.rewardedPlacementsApproved ||
                !manifest.regionalBehaviorApproved ||
                !manifest.securityReviewPassed ||
                !manifest.representativeDeviceFailurePathsPassed
            )
            {
                return Rejected(
                    CommercialRuntimePhase.ApprovalRequired,
                    CommercialRuntimeReasonCodes.ReviewApprovalsIncomplete,
                    manifestHash,
                    economy
                );
            }

            if (
                !manifest.saveMigrationApproved ||
                !manifest.durableFulfillmentApproved
            )
            {
                return Rejected(
                    CommercialRuntimePhase.ApprovalRequired,
                    CommercialRuntimeReasonCodes.DurableFulfillmentUnavailable,
                    manifestHash,
                    economy
                );
            }

            if (
                !manifest.analyticsActivationApproved &&
                !manifest.iapActivationApproved &&
                !manifest.rewardedAdsActivationApproved
            )
            {
                return Rejected(
                    CommercialRuntimePhase.ApprovalRequired,
                    CommercialRuntimeReasonCodes.CapabilityApprovalMissing,
                    manifestHash,
                    economy
                );
            }

            if (!manifest.finalRuntimeActivationApproved)
            {
                return Rejected(
                    CommercialRuntimePhase.ApprovalRequired,
                    CommercialRuntimeReasonCodes.FinalApprovalMissing,
                    manifestHash,
                    economy
                );
            }

            if (!IsExternalProviderActivationCompiled())
            {
                return Rejected(
                    CommercialRuntimePhase.Disabled,
                    CommercialRuntimeReasonCodes.BuildActivationDisabled,
                    manifestHash,
                    economy
                );
            }

            if (!trustedProductionProvidersBound)
            {
                return Rejected(
                    CommercialRuntimePhase.ReadyForProviderBinding,
                    CommercialRuntimeReasonCodes.ProviderBindingUnavailable,
                    manifestHash,
                    economy
                );
            }

            return new CommercialRuntimeActivationDecision(
                CommercialRuntimePhase.Active,
                CommercialRuntimeReasonCodes.Activated,
                true,
                manifest.analyticsActivationApproved,
                manifest.iapActivationApproved,
                manifest.rewardedAdsActivationApproved,
                manifest.environment,
                manifestHash,
                economy.ReasonCode
            );
        }

        static CommercialRuntimeActivationDecision Rejected(
            CommercialRuntimePhase phase,
            string reasonCode,
            string manifestHash,
            RuntimeEconomyActivationResult economy
        )
        {
            return new CommercialRuntimeActivationDecision(
                phase,
                reasonCode,
                false,
                false,
                false,
                false,
                RequiredEnvironment,
                manifestHash,
                economy != null ? economy.ReasonCode : string.Empty
            );
        }

        static bool IsExternalProviderActivationCompiled()
        {
            return ExternalProviderActivationCompiled;
        }
    }
}
