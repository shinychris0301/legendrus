using System;
using System.Collections.Generic;
using System.Globalization;

namespace LegendsOfRus.CommercialCore.RemoteConfig
{
    internal sealed class RemoteConfigKeySpec
    {
        public string Key { get; }
        public RemoteConfigValueKind Kind { get; }
        public bool MustRemainFalseInSchemaOne { get; }

        public RemoteConfigKeySpec(
            string key,
            RemoteConfigValueKind kind,
            bool mustRemainFalseInSchemaOne
        )
        {
            Key = key;
            Kind = kind;
            MustRemainFalseInSchemaOne = mustRemainFalseInSchemaOne;
        }
    }

    public static class RemoteConfigAllowlist
    {
        static readonly RemoteConfigKeySpec[] specs =
        {
            Bool(RemoteConfigKeys.AnalyticsCaptureEnabled, false),
            Bool(RemoteConfigKeys.AnalyticsUploadEnabled, true),
            Bool(RemoteConfigKeys.IapEnabled, true),
            Bool(RemoteConfigKeys.RewardedAdsEnabled, true),
            Bool(RemoteConfigKeys.RewardedEnergyEnabled, true),
            Bool(RemoteConfigKeys.RewardedContinueEnabled, true),
            Bool(RemoteConfigKeys.RewardedDoubleEnabled, true),
            Bool(RemoteConfigKeys.StarterOfferEnabled, true),
            Bool(RemoteConfigKeys.ChapterFinaleOfferEnabled, true),
            Bool(RemoteConfigKeys.DevelopmentSandboxEnabled, false),
        };

        static readonly Dictionary<string, RemoteConfigKeySpec> byKey =
            BuildLookup();

        public static string[] GetRequiredKeys()
        {
            var keys = new string[specs.Length];
            for (int i = 0; i < specs.Length; i++)
                keys[i] = specs[i].Key;
            Array.Sort(keys, StringComparer.Ordinal);
            return keys;
        }

        public static bool IsAllowed(string key)
        {
            return key != null && byKey.ContainsKey(key);
        }

        internal static bool TryGet(string key, out RemoteConfigKeySpec spec)
        {
            if (key == null)
            {
                spec = null;
                return false;
            }
            return byKey.TryGetValue(key, out spec);
        }

        static RemoteConfigKeySpec Bool(string key, bool mustRemainFalse)
        {
            return new RemoteConfigKeySpec(
                key,
                RemoteConfigValueKind.Boolean,
                mustRemainFalse
            );
        }

        static Dictionary<string, RemoteConfigKeySpec> BuildLookup()
        {
            var result = new Dictionary<string, RemoteConfigKeySpec>(
                StringComparer.Ordinal
            );
            for (int i = 0; i < specs.Length; i++)
                result.Add(specs[i].Key, specs[i]);
            return result;
        }
    }

    public static class RemoteConfigValidator
    {
        public const int CurrentSchemaVersion = 1;
        public const string RequiredSnapshotId = "v1.16-step01-local-defaults";
        public const int RequiredRevision = 1;
        public const string RequiredEnvironment = "development";
        public const string RequiredEconomyCatalogId = "v1.15-step01-proposal";
        public const string RequiredEconomyCatalogSourceSha256 =
            "ba66d7104c3d0a33ea4e0575b9965325cf9f90a8177c4e089c638d202267c42c";

        internal static RemoteConfigValidationReport Validate(
            RemoteConfigDocumentData document
        )
        {
            var violations = new List<RemoteConfigViolation>();
            if (document == null)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.JsonInvalid,
                    "$",
                    "The remote-config document is null."
                );
                return Build(violations);
            }

            if (document.SchemaVersion != CurrentSchemaVersion)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.UnsupportedSchema,
                    "$.schemaVersion",
                    "Only remote-config schema version 1 is supported."
                );
            }
            if (!string.Equals(
                document.SnapshotId,
                RequiredSnapshotId,
                StringComparison.Ordinal
            ))
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.SnapshotIdentityInvalid,
                    "$.snapshotId",
                    "The local snapshot id is not the reviewed Step 1 identity."
                );
            }
            if (document.Revision != RequiredRevision)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.RevisionInvalid,
                    "$.revision",
                    "The local snapshot revision must be exactly one."
                );
            }
            if (!string.Equals(
                document.Environment,
                RequiredEnvironment,
                StringComparison.Ordinal
            ))
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.EnvironmentUnsupported,
                    "$.environment",
                    "Step 1 accepts only the detached development environment."
                );
            }
            if (!document.ProposalOnly)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.ProposalOnlyRequired,
                    "$.proposalOnly",
                    "The local snapshot must remain explicitly proposal-only."
                );
            }
            if (document.Authoritative)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.AuthorityForbidden,
                    "$.authoritative",
                    "A Step 1 local snapshot cannot claim runtime authority."
                );
            }
            if (!string.Equals(
                document.EconomyCatalogId,
                RequiredEconomyCatalogId,
                StringComparison.Ordinal
            ))
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.EconomyBindingMismatch,
                    "$.economyCatalogId",
                    "The snapshot catalog id does not match the reviewed economy proposal."
                );
            }
            if (!string.Equals(
                document.EconomyCatalogSourceSha256,
                RequiredEconomyCatalogSourceSha256,
                StringComparison.Ordinal
            ))
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.EconomyBindingMismatch,
                    "$.economyCatalogSourceSha256",
                    "The snapshot catalog hash does not match the reviewed economy proposal."
                );
            }
            if (!IsLowerHexSha256(document.EconomyCatalogSourceSha256))
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.ValueInvalid,
                    "$.economyCatalogSourceSha256",
                    "The economy source binding must be 64 lowercase hexadecimal characters."
                );
            }

            ValidateEntries(document, violations);
            return Build(violations);
        }

        internal static bool TryBuildSnapshot(
            RemoteConfigDocumentData document,
            RemoteConfigValidationReport validation,
            out RemoteConfigSnapshot snapshot
        )
        {
            snapshot = RemoteConfigSnapshot.ConservativeFallback;
            if (document == null || validation == null || !validation.IsValid)
                return false;

            var values = new Dictionary<string, RemoteConfigValue>(
                StringComparer.Ordinal
            );
            for (int i = 0; i < document.Entries.Length; i++)
            {
                RemoteConfigEntryData entry = document.Entries[i];
                RemoteConfigKeySpec spec;
                if (entry == null || !RemoteConfigAllowlist.TryGet(entry.Key, out spec))
                    return false;

                RemoteConfigValue value;
                if (!TryParseValue(entry, spec, out value) || values.ContainsKey(entry.Key))
                    return false;
                values.Add(entry.Key, value);
            }

            snapshot = new RemoteConfigSnapshot(
                document.SchemaVersion,
                document.SnapshotId,
                document.Revision,
                document.Environment,
                document.ProposalOnly,
                false,
                document.EconomyCatalogId,
                document.EconomyCatalogSourceSha256,
                values,
                false
            );
            return true;
        }

        static void ValidateEntries(
            RemoteConfigDocumentData document,
            List<RemoteConfigViolation> violations
        )
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            RemoteConfigEntryData[] entries = document.Entries;
            if (entries == null)
            {
                Add(
                    violations,
                    RemoteConfigFailureCodes.JsonMissingField,
                    "$.entries",
                    "The entries array is required."
                );
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                RemoteConfigEntryData entry = entries[i];
                string path = "$.entries[" + i.ToString(CultureInfo.InvariantCulture) + "]";
                if (entry == null)
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.JsonTypeMismatch,
                        path,
                        "Every config entry must be an object."
                    );
                    continue;
                }

                RemoteConfigKeySpec spec;
                if (!RemoteConfigAllowlist.TryGet(entry.Key, out spec))
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.UnknownKey,
                        path + ".key",
                        "The config key is not in the exact schema-one allowlist."
                    );
                    continue;
                }
                if (!seen.Add(entry.Key))
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.DuplicateKey,
                        path + ".key",
                        "Each allowlisted key must appear exactly once."
                    );
                    continue;
                }

                string expectedType = TypeName(spec.Kind);
                if (!string.Equals(entry.Type, expectedType, StringComparison.Ordinal))
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.TypeMismatch,
                        path + ".type",
                        "The entry type must be exactly '" + expectedType + "'."
                    );
                    continue;
                }

                RemoteConfigValue parsed;
                if (!TryParseValue(entry, spec, out parsed))
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.ValueInvalid,
                        path + ".value",
                        "The entry value is not canonical for its declared type."
                    );
                    continue;
                }
                if (
                    spec.MustRemainFalseInSchemaOne &&
                    parsed.Kind == RemoteConfigValueKind.Boolean &&
                    parsed.BooleanValue
                )
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.UnsafeEnablement,
                        path + ".value",
                        "This external or live commercial feature must remain disabled in Step 1."
                    );
                }
            }

            string[] requiredKeys = RemoteConfigAllowlist.GetRequiredKeys();
            for (int i = 0; i < requiredKeys.Length; i++)
            {
                if (!seen.Contains(requiredKeys[i]))
                {
                    Add(
                        violations,
                        RemoteConfigFailureCodes.MissingKey,
                        "$.entries",
                        "Required key '" + requiredKeys[i] + "' is missing."
                    );
                }
            }

            bool rewardedAds = ReadBoolean(entries, RemoteConfigKeys.RewardedAdsEnabled);
            if (!rewardedAds)
            {
                RequireFalse(
                    entries,
                    RemoteConfigKeys.RewardedEnergyEnabled,
                    violations
                );
                RequireFalse(
                    entries,
                    RemoteConfigKeys.RewardedContinueEnabled,
                    violations
                );
                RequireFalse(
                    entries,
                    RemoteConfigKeys.RewardedDoubleEnabled,
                    violations
                );
            }

            bool iap = ReadBoolean(entries, RemoteConfigKeys.IapEnabled);
            if (!iap)
            {
                RequireFalse(
                    entries,
                    RemoteConfigKeys.StarterOfferEnabled,
                    violations
                );
                RequireFalse(
                    entries,
                    RemoteConfigKeys.ChapterFinaleOfferEnabled,
                    violations
                );
            }
        }

        static void RequireFalse(
            RemoteConfigEntryData[] entries,
            string key,
            List<RemoteConfigViolation> violations
        )
        {
            if (!ReadBoolean(entries, key))
                return;
            Add(
                violations,
                RemoteConfigFailureCodes.UnsafeEnablement,
                "$.entries[" + key + "]",
                "A placement cannot be enabled while its adapter is disabled."
            );
        }

        static bool ReadBoolean(RemoteConfigEntryData[] entries, string key)
        {
            if (entries == null)
                return false;
            for (int i = 0; i < entries.Length; i++)
            {
                RemoteConfigEntryData entry = entries[i];
                if (
                    entry != null &&
                    string.Equals(entry.Key, key, StringComparison.Ordinal) &&
                    string.Equals(entry.Type, "bool", StringComparison.Ordinal)
                )
                {
                    return string.Equals(entry.Value, "true", StringComparison.Ordinal);
                }
            }
            return false;
        }

        static bool TryParseValue(
            RemoteConfigEntryData entry,
            RemoteConfigKeySpec spec,
            out RemoteConfigValue parsed
        )
        {
            parsed = null;
            if (entry == null || spec == null || entry.Value == null)
                return false;

            switch (spec.Kind)
            {
                case RemoteConfigValueKind.Boolean:
                    if (string.Equals(entry.Value, "false", StringComparison.Ordinal))
                    {
                        parsed = RemoteConfigValue.FromBoolean(false);
                        return true;
                    }
                    if (string.Equals(entry.Value, "true", StringComparison.Ordinal))
                    {
                        parsed = RemoteConfigValue.FromBoolean(true);
                        return true;
                    }
                    return false;

                case RemoteConfigValueKind.Int64:
                    long number;
                    if (
                        !long.TryParse(
                            entry.Value,
                            NumberStyles.AllowLeadingSign,
                            CultureInfo.InvariantCulture,
                            out number
                        ) ||
                        !string.Equals(
                            number.ToString(CultureInfo.InvariantCulture),
                            entry.Value,
                            StringComparison.Ordinal
                        )
                    )
                    {
                        return false;
                    }
                    parsed = RemoteConfigValue.FromInt64(number);
                    return true;

                case RemoteConfigValueKind.String:
                    if (!IsStableString(entry.Value))
                        return false;
                    parsed = RemoteConfigValue.FromString(entry.Value);
                    return true;

                default:
                    return false;
            }
        }

        static string TypeName(RemoteConfigValueKind kind)
        {
            switch (kind)
            {
                case RemoteConfigValueKind.Boolean:
                    return "bool";
                case RemoteConfigValueKind.Int64:
                    return "int64";
                case RemoteConfigValueKind.String:
                    return "string";
                default:
                    return string.Empty;
            }
        }

        static bool IsStableString(string value)
        {
            if (
                string.IsNullOrEmpty(value) ||
                !string.Equals(value, value.Trim(), StringComparison.Ordinal)
            )
            {
                return false;
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsControl(value[i]))
                    return false;
            }
            return true;
        }

        static bool IsLowerHexSha256(string value)
        {
            if (value == null || value.Length != 64)
                return false;
            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (
                    (current < '0' || current > '9') &&
                    (current < 'a' || current > 'f')
                )
                {
                    return false;
                }
            }
            return true;
        }

        static void Add(
            List<RemoteConfigViolation> violations,
            string code,
            string path,
            string message
        )
        {
            violations.Add(new RemoteConfigViolation(code, path, message));
        }

        static RemoteConfigValidationReport Build(
            List<RemoteConfigViolation> violations
        )
        {
            RemoteConfigViolation[] ordered = violations.ToArray();
            Array.Sort(ordered, CompareViolations);
            return new RemoteConfigValidationReport(ordered);
        }

        static int CompareViolations(
            RemoteConfigViolation left,
            RemoteConfigViolation right
        )
        {
            int path = string.Compare(left.Path, right.Path, StringComparison.Ordinal);
            if (path != 0)
                return path;
            int code = string.Compare(left.Code, right.Code, StringComparison.Ordinal);
            if (code != 0)
                return code;
            return string.Compare(left.Message, right.Message, StringComparison.Ordinal);
        }
    }
}
