using System;
using System.Collections.Generic;

namespace LegendsOfRus.CommercialCore.RemoteConfig
{
    public enum RemoteConfigValueKind
    {
        Boolean,
        Int64,
        String
    }

    public enum RemoteConfigLoadStatus
    {
        LoadedDetachedLocal,
        SourceMissing,
        SourceTooLarge,
        SourceHashMismatch,
        JsonInvalid,
        ValidationFailed
    }

    public static class RemoteConfigFailureCodes
    {
        public const string None = "";
        public const string SourceMissing = "RC_SOURCE_MISSING";
        public const string SourceTooLarge = "RC_SOURCE_TOO_LARGE";
        public const string SourceHashMismatch = "RC_SOURCE_HASH_MISMATCH";
        public const string JsonInvalid = "RC_JSON_INVALID";
        public const string JsonUnknownField = "RC_JSON_UNKNOWN_FIELD";
        public const string JsonDuplicateField = "RC_JSON_DUPLICATE_FIELD";
        public const string JsonMissingField = "RC_JSON_MISSING_FIELD";
        public const string JsonTypeMismatch = "RC_JSON_TYPE_MISMATCH";
        public const string UnsupportedSchema = "RC_SCHEMA_UNSUPPORTED";
        public const string SnapshotIdentityInvalid = "RC_SNAPSHOT_ID_INVALID";
        public const string RevisionInvalid = "RC_REVISION_INVALID";
        public const string EnvironmentUnsupported = "RC_ENVIRONMENT_UNSUPPORTED";
        public const string ProposalOnlyRequired = "RC_PROPOSAL_ONLY_REQUIRED";
        public const string AuthorityForbidden = "RC_AUTHORITY_FORBIDDEN";
        public const string EconomyBindingMismatch = "RC_ECONOMY_BINDING_MISMATCH";
        public const string UnknownKey = "RC_UNKNOWN_KEY";
        public const string DuplicateKey = "RC_DUPLICATE_KEY";
        public const string MissingKey = "RC_MISSING_KEY";
        public const string TypeMismatch = "RC_TYPE_MISMATCH";
        public const string ValueInvalid = "RC_VALUE_INVALID";
        public const string UnsafeEnablement = "RC_UNSAFE_ENABLEMENT";
    }

    public static class RemoteConfigKeys
    {
        public const string AnalyticsCaptureEnabled = "analytics.capture.enabled";
        public const string AnalyticsUploadEnabled = "analytics.upload.enabled";
        public const string IapEnabled = "monetization.iap.enabled";
        public const string RewardedAdsEnabled = "monetization.rewarded_ads.enabled";
        public const string RewardedEnergyEnabled = "monetization.rewarded_energy.enabled";
        public const string RewardedContinueEnabled = "monetization.rewarded_continue.enabled";
        public const string RewardedDoubleEnabled = "monetization.rewarded_double.enabled";
        public const string StarterOfferEnabled = "monetization.starter_offer.enabled";
        public const string ChapterFinaleOfferEnabled = "monetization.chapter_finale_offer.enabled";
        public const string DevelopmentSandboxEnabled = "monetization.development_sandbox.enabled";
    }

    public sealed class RemoteConfigViolation
    {
        public string Code { get; }
        public string Path { get; }
        public string Message { get; }

        public RemoteConfigViolation(string code, string path, string message)
        {
            Code = code ?? RemoteConfigFailureCodes.JsonInvalid;
            Path = path ?? "$";
            Message = message ?? string.Empty;
        }
    }

    public sealed class RemoteConfigValidationReport
    {
        readonly RemoteConfigViolation[] violations;

        public bool IsValid { get { return violations.Length == 0; } }
        public int ViolationCount { get { return violations.Length; } }

        public RemoteConfigValidationReport(RemoteConfigViolation[] violations)
        {
            this.violations = violations == null
                ? new RemoteConfigViolation[0]
                : (RemoteConfigViolation[])violations.Clone();
        }

        public RemoteConfigViolation[] GetViolations()
        {
            return (RemoteConfigViolation[])violations.Clone();
        }
    }

    public interface IRemoteConfigProvider
    {
        RemoteConfigLoadResult LoadSnapshot();
    }

    public sealed class RemoteConfigLoadResult
    {
        public RemoteConfigLoadStatus Status { get; }
        public string Code { get; }
        public string Message { get; }
        public string SourceSha256 { get; }
        public string ContentSha256 { get; }
        public RemoteConfigSnapshot Snapshot { get; }
        public RemoteConfigValidationReport Validation { get; }

        public bool Succeeded
        {
            get { return Status == RemoteConfigLoadStatus.LoadedDetachedLocal; }
        }

        // A valid local snapshot is still only detached configuration evidence.
        // Runtime authority requires a separately reviewed integration boundary.
        public bool RuntimeApplied { get { return false; } }

        internal RemoteConfigLoadResult(
            RemoteConfigLoadStatus status,
            string code,
            string message,
            string sourceSha256,
            string contentSha256,
            RemoteConfigSnapshot snapshot,
            RemoteConfigValidationReport validation
        )
        {
            Status = status;
            Code = code ?? string.Empty;
            Message = message ?? string.Empty;
            SourceSha256 = sourceSha256 ?? string.Empty;
            ContentSha256 = contentSha256 ?? string.Empty;
            Snapshot = snapshot ?? RemoteConfigSnapshot.ConservativeFallback;
            Validation = validation ?? new RemoteConfigValidationReport(
                new RemoteConfigViolation[0]
            );
        }
    }

    public sealed class RemoteConfigSnapshot
    {
        readonly Dictionary<string, RemoteConfigValue> values;

        public static readonly RemoteConfigSnapshot ConservativeFallback =
            BuildConservativeFallback();

        public int SchemaVersion { get; }
        public string SnapshotId { get; }
        public int Revision { get; }
        public string Environment { get; }
        public bool ProposalOnly { get; }
        public bool IsAuthoritative { get; }
        public string EconomyCatalogId { get; }
        public string EconomyCatalogSourceSha256 { get; }
        public string ContentSha256 { get; }
        public bool IsFallback { get; }
        public int Count { get { return values.Count; } }

        internal RemoteConfigSnapshot(
            int schemaVersion,
            string snapshotId,
            int revision,
            string environment,
            bool proposalOnly,
            bool isAuthoritative,
            string economyCatalogId,
            string economyCatalogSourceSha256,
            IDictionary<string, RemoteConfigValue> values,
            bool isFallback
        )
        {
            SchemaVersion = schemaVersion;
            SnapshotId = snapshotId ?? string.Empty;
            Revision = revision;
            Environment = environment ?? string.Empty;
            ProposalOnly = proposalOnly;
            IsAuthoritative = isAuthoritative;
            EconomyCatalogId = economyCatalogId ?? string.Empty;
            EconomyCatalogSourceSha256 = economyCatalogSourceSha256 ?? string.Empty;
            IsFallback = isFallback;
            this.values = new Dictionary<string, RemoteConfigValue>(
                StringComparer.Ordinal
            );
            if (values != null)
            {
                foreach (KeyValuePair<string, RemoteConfigValue> pair in values)
                    this.values.Add(pair.Key, pair.Value);
            }
            ContentSha256 = RemoteConfigDeterministicJson.ComputeContentSha256(this);
        }

        public bool TryGetBoolean(string key, out bool value)
        {
            value = false;
            RemoteConfigValue stored;
            if (
                string.IsNullOrEmpty(key) ||
                !values.TryGetValue(key, out stored) ||
                stored.Kind != RemoteConfigValueKind.Boolean
            )
            {
                return false;
            }

            value = stored.BooleanValue;
            return true;
        }

        public bool TryGetInt64(string key, out long value)
        {
            value = 0L;
            RemoteConfigValue stored;
            if (
                string.IsNullOrEmpty(key) ||
                !values.TryGetValue(key, out stored) ||
                stored.Kind != RemoteConfigValueKind.Int64
            )
            {
                return false;
            }

            value = stored.Int64Value;
            return true;
        }

        public bool TryGetString(string key, out string value)
        {
            value = string.Empty;
            RemoteConfigValue stored;
            if (
                string.IsNullOrEmpty(key) ||
                !values.TryGetValue(key, out stored) ||
                stored.Kind != RemoteConfigValueKind.String
            )
            {
                return false;
            }

            value = stored.StringValue;
            return true;
        }

        public string[] GetKeys()
        {
            var result = new string[values.Count];
            values.Keys.CopyTo(result, 0);
            Array.Sort(result, StringComparer.Ordinal);
            return result;
        }

        internal IDictionary<string, RemoteConfigValue> DetachedValues()
        {
            return new Dictionary<string, RemoteConfigValue>(
                values,
                StringComparer.Ordinal
            );
        }

        static RemoteConfigSnapshot BuildConservativeFallback()
        {
            var fallbackValues = new Dictionary<string, RemoteConfigValue>(
                StringComparer.Ordinal
            );
            string[] keys = RemoteConfigAllowlist.GetRequiredKeys();
            for (int i = 0; i < keys.Length; i++)
                fallbackValues.Add(keys[i], RemoteConfigValue.FromBoolean(false));

            var snapshot = new RemoteConfigSnapshot(
                RemoteConfigValidator.CurrentSchemaVersion,
                "v1.16-safe-fallback",
                0,
                "fallback",
                true,
                false,
                string.Empty,
                string.Empty,
                fallbackValues,
                true
            );
            return snapshot;
        }
    }

    internal sealed class RemoteConfigValue
    {
        public RemoteConfigValueKind Kind { get; }
        public bool BooleanValue { get; }
        public long Int64Value { get; }
        public string StringValue { get; }

        RemoteConfigValue(
            RemoteConfigValueKind kind,
            bool booleanValue,
            long int64Value,
            string stringValue
        )
        {
            Kind = kind;
            BooleanValue = booleanValue;
            Int64Value = int64Value;
            StringValue = stringValue ?? string.Empty;
        }

        public static RemoteConfigValue FromBoolean(bool value)
        {
            return new RemoteConfigValue(
                RemoteConfigValueKind.Boolean,
                value,
                0L,
                string.Empty
            );
        }

        public static RemoteConfigValue FromInt64(long value)
        {
            return new RemoteConfigValue(
                RemoteConfigValueKind.Int64,
                false,
                value,
                string.Empty
            );
        }

        public static RemoteConfigValue FromString(string value)
        {
            return new RemoteConfigValue(
                RemoteConfigValueKind.String,
                false,
                0L,
                value
            );
        }
    }

    internal sealed class RemoteConfigDocumentData
    {
        public int SchemaVersion;
        public string SnapshotId = string.Empty;
        public int Revision;
        public string Environment = string.Empty;
        public bool ProposalOnly;
        public bool Authoritative = true;
        public string EconomyCatalogId = string.Empty;
        public string EconomyCatalogSourceSha256 = string.Empty;
        public RemoteConfigEntryData[] Entries = new RemoteConfigEntryData[0];
    }

    internal sealed class RemoteConfigEntryData
    {
        public string Key = string.Empty;
        public string Type = string.Empty;
        public string Value = string.Empty;
    }
}
