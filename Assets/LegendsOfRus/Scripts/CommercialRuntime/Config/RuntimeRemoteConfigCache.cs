using System;
using LegendsOfRus.CommercialCore.RemoteConfig;
using UnityEngine;

namespace LegendsOfRus.CommercialRuntime
{
    public sealed class RuntimeRemoteConfigUpdate
    {
        public bool Accepted { get; private set; }
        public bool RetainedLastKnownGood { get; private set; }
        public string ReasonCode { get; private set; }
        public RemoteConfigSnapshot EffectiveSnapshot { get; private set; }
        public bool UsesConservativeFallback
        {
            get
            {
                return
                    EffectiveSnapshot == null ||
                    EffectiveSnapshot.IsFallback;
            }
        }

        internal RuntimeRemoteConfigUpdate(
            bool accepted,
            bool retainedLastKnownGood,
            string reasonCode,
            RemoteConfigSnapshot effectiveSnapshot
        )
        {
            Accepted = accepted;
            RetainedLastKnownGood = retainedLastKnownGood;
            ReasonCode = reasonCode ?? CommercialRuntimeReasonCodes.RemoteConfigRejected;
            EffectiveSnapshot = effectiveSnapshot ??
                RemoteConfigSnapshot.ConservativeFallback;
        }
    }

    /// <summary>
    /// Process-local only. A validated snapshot is retained across refresh
    /// failures in the current process, but never persisted or treated as
    /// activation authority. First-load failure uses the all-disabled fallback.
    /// </summary>
    public sealed class RuntimeRemoteConfigCache
    {
        public const string ExpectedContentSha256 =
            "3f4229158b3f5516a536ccf31954f417f8be45a64edd2e4cc33946c2d42e07eb";

        readonly object syncRoot = new object();
        RemoteConfigSnapshot current = RemoteConfigSnapshot.ConservativeFallback;
        bool hasLastKnownGood;

        public RemoteConfigSnapshot Current
        {
            get
            {
                lock (syncRoot)
                    return current;
            }
        }

        public bool HasLastKnownGood
        {
            get
            {
                lock (syncRoot)
                    return hasLastKnownGood;
            }
        }

        public RuntimeRemoteConfigUpdate LoadBundled()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                DeterministicLocalRemoteConfigProvider.ResourcePath
            );
            if (asset == null)
                return Retain(CommercialRuntimeReasonCodes.RemoteConfigProviderMissing);

            return TryRefresh(new DeterministicLocalRemoteConfigProvider(asset.text));
        }

        public RuntimeRemoteConfigUpdate TryRefresh(IRemoteConfigProvider provider)
        {
            if (provider == null)
                return Retain(CommercialRuntimeReasonCodes.RemoteConfigProviderMissing);

            RemoteConfigLoadResult result;
            try
            {
                result = provider.LoadSnapshot();
            }
            catch
            {
                return Retain(CommercialRuntimeReasonCodes.RemoteConfigProviderFailed);
            }

            if (result == null || !result.Succeeded || result.Snapshot == null)
                return Retain(CommercialRuntimeReasonCodes.RemoteConfigRejected);

            RemoteConfigSnapshot candidate = result.Snapshot;
            if (
                candidate.IsFallback ||
                !candidate.ProposalOnly ||
                candidate.IsAuthoritative ||
                result.Validation == null ||
                !result.Validation.IsValid ||
                !string.Equals(
                    result.SourceSha256,
                    DeterministicLocalRemoteConfigProvider.ExpectedBundledSourceSha256,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    result.ContentSha256,
                    ExpectedContentSha256,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    candidate.ContentSha256,
                    result.ContentSha256,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    candidate.SnapshotId,
                    RemoteConfigValidator.RequiredSnapshotId,
                    StringComparison.Ordinal
                ) ||
                candidate.Revision != RemoteConfigValidator.RequiredRevision ||
                !string.Equals(
                    candidate.EconomyCatalogId,
                    RemoteConfigValidator.RequiredEconomyCatalogId,
                    StringComparison.Ordinal
                ) ||
                !string.Equals(
                    candidate.EconomyCatalogSourceSha256,
                    RemoteConfigValidator.RequiredEconomyCatalogSourceSha256,
                    StringComparison.Ordinal
                )
            )
            {
                return Retain(CommercialRuntimeReasonCodes.RemoteConfigBindingMismatch);
            }

            lock (syncRoot)
            {
                current = candidate;
                hasLastKnownGood = true;
                return new RuntimeRemoteConfigUpdate(
                    true,
                    false,
                    CommercialRuntimeReasonCodes.RemoteConfigLoaded,
                    current
                );
            }
        }

        RuntimeRemoteConfigUpdate Retain(string reasonCode)
        {
            lock (syncRoot)
            {
                return new RuntimeRemoteConfigUpdate(
                    false,
                    hasLastKnownGood,
                    reasonCode,
                    current
                );
            }
        }
    }
}
