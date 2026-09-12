using System;
using UnityEngine;

namespace LegendsOfRus.CommercialRuntime
{
    [Serializable]
    internal sealed class CommercialRuntimeConfigData
    {
        public int schemaVersion;
        public string configId = string.Empty;
        public string environment = string.Empty;
        public string consentProviderId = string.Empty;
        public string analyticsProviderId = string.Empty;
        public string iapProviderId = string.Empty;
        public string rewardedAdProviderId = string.Empty;
        public bool analyticsCaptureAllowed;
        public bool analyticsUploadAllowed;
        public bool iapAllowed;
        public bool rewardedAdsAllowed;
        public bool developmentSandboxAllowed;
    }

    public sealed class CommercialRuntimeConfigSnapshot
    {
        static readonly CommercialRuntimeConfigSnapshot conservative =
            new CommercialRuntimeConfigSnapshot(
                CommercialRuntimeConfigLoader.RequiredConfigId,
                CommercialRuntimeConfigLoader.RequiredEnvironment,
                CommercialRuntimeConfigLoader.DisabledProviderId,
                CommercialRuntimeConfigLoader.DisabledProviderId,
                CommercialRuntimeConfigLoader.DisabledProviderId,
                CommercialRuntimeConfigLoader.DisabledProviderId,
                false,
                false,
                false,
                false,
                false,
                true,
                string.Empty
            );

        public static CommercialRuntimeConfigSnapshot ConservativeDisabled
        {
            get { return conservative; }
        }

        public string ConfigId { get; private set; }
        public string Environment { get; private set; }
        public string ConsentProviderId { get; private set; }
        public string AnalyticsProviderId { get; private set; }
        public string IapProviderId { get; private set; }
        public string RewardedAdProviderId { get; private set; }
        public bool AnalyticsCaptureAllowed { get; private set; }
        public bool AnalyticsUploadAllowed { get; private set; }
        public bool IapAllowed { get; private set; }
        public bool RewardedAdsAllowed { get; private set; }
        public bool DevelopmentSandboxAllowed { get; private set; }
        public bool IsFallback { get; private set; }
        public string SourceSha256 { get; private set; }

        internal CommercialRuntimeConfigSnapshot(
            string configId,
            string environment,
            string consentProviderId,
            string analyticsProviderId,
            string iapProviderId,
            string rewardedAdProviderId,
            bool analyticsCaptureAllowed,
            bool analyticsUploadAllowed,
            bool iapAllowed,
            bool rewardedAdsAllowed,
            bool developmentSandboxAllowed,
            bool isFallback,
            string sourceSha256
        )
        {
            ConfigId = configId ?? string.Empty;
            Environment = environment ?? string.Empty;
            ConsentProviderId = consentProviderId ?? string.Empty;
            AnalyticsProviderId = analyticsProviderId ?? string.Empty;
            IapProviderId = iapProviderId ?? string.Empty;
            RewardedAdProviderId = rewardedAdProviderId ?? string.Empty;
            AnalyticsCaptureAllowed = analyticsCaptureAllowed;
            AnalyticsUploadAllowed = analyticsUploadAllowed;
            IapAllowed = iapAllowed;
            RewardedAdsAllowed = rewardedAdsAllowed;
            DevelopmentSandboxAllowed = developmentSandboxAllowed;
            IsFallback = isFallback;
            SourceSha256 = sourceSha256 ?? string.Empty;
        }
    }

    public sealed class CommercialRuntimeConfigLoadResult
    {
        public bool IsValid { get; private set; }
        public string ReasonCode { get; private set; }
        public CommercialRuntimeConfigSnapshot Snapshot { get; private set; }

        internal CommercialRuntimeConfigLoadResult(
            bool isValid,
            string reasonCode,
            CommercialRuntimeConfigSnapshot snapshot
        )
        {
            IsValid = isValid;
            ReasonCode = reasonCode ?? CommercialRuntimeReasonCodes.ConfigSourceInvalid;
            Snapshot = snapshot ?? CommercialRuntimeConfigSnapshot.ConservativeDisabled;
        }
    }

    public static class CommercialRuntimeConfigLoader
    {
        public const string ResourcePath = "Config/commercial_runtime_config";
        public const int CurrentSchemaVersion = 1;
        public const string RequiredConfigId = "v1.16-step02-disabled-composition";
        public const string RequiredEnvironment = "disabled";
        public const string DisabledProviderId = "disabled";
        public const string ExpectedSourceSha256 =
            "d5e7705c47f0a7368fa618db0740703e4e0628a2b99ab5318681933f00866c02";
        const int MaximumSourceCharacters = 16384;

        public static CommercialRuntimeConfigLoadResult Load()
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null)
                return Failed(CommercialRuntimeReasonCodes.ConfigResourceMissing);
            return Evaluate(asset.text);
        }

        internal static CommercialRuntimeConfigLoadResult Evaluate(string sourceJson)
        {
            if (
                string.IsNullOrEmpty(sourceJson) ||
                sourceJson.Length > MaximumSourceCharacters
            )
            {
                return Failed(CommercialRuntimeReasonCodes.ConfigSourceInvalid);
            }

            string sourceHash = CommercialRuntimeSourceHash.Compute(sourceJson);
            if (!string.Equals(
                sourceHash,
                ExpectedSourceSha256,
                StringComparison.Ordinal
            ))
            {
                return Failed(CommercialRuntimeReasonCodes.ConfigSourceHashMismatch);
            }

            CommercialRuntimeConfigData data;
            try
            {
                data = JsonUtility.FromJson<CommercialRuntimeConfigData>(sourceJson);
            }
            catch
            {
                return Failed(CommercialRuntimeReasonCodes.ConfigJsonInvalid);
            }

            if (data == null)
                return Failed(CommercialRuntimeReasonCodes.ConfigJsonInvalid);

            if (
                data.schemaVersion != CurrentSchemaVersion ||
                !string.Equals(data.configId, RequiredConfigId, StringComparison.Ordinal) ||
                !string.Equals(data.environment, RequiredEnvironment, StringComparison.Ordinal) ||
                !IsDisabledProvider(data.consentProviderId) ||
                !IsDisabledProvider(data.analyticsProviderId) ||
                !IsDisabledProvider(data.iapProviderId) ||
                !IsDisabledProvider(data.rewardedAdProviderId)
            )
            {
                return Failed(CommercialRuntimeReasonCodes.ConfigIdentityMismatch);
            }

            if (
                data.analyticsCaptureAllowed ||
                data.analyticsUploadAllowed ||
                data.iapAllowed ||
                data.rewardedAdsAllowed ||
                data.developmentSandboxAllowed
            )
            {
                return Failed(CommercialRuntimeReasonCodes.ConfigEnablementForbidden);
            }

            return new CommercialRuntimeConfigLoadResult(
                true,
                CommercialRuntimeReasonCodes.None,
                new CommercialRuntimeConfigSnapshot(
                    data.configId,
                    data.environment,
                    data.consentProviderId,
                    data.analyticsProviderId,
                    data.iapProviderId,
                    data.rewardedAdProviderId,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    sourceHash
                )
            );
        }

        static bool IsDisabledProvider(string providerId)
        {
            return string.Equals(
                providerId,
                DisabledProviderId,
                StringComparison.Ordinal
            );
        }

        static CommercialRuntimeConfigLoadResult Failed(string reasonCode)
        {
            return new CommercialRuntimeConfigLoadResult(
                false,
                reasonCode,
                CommercialRuntimeConfigSnapshot.ConservativeDisabled
            );
        }
    }
}
