using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.EconomyCore;
using UnityEngine;

namespace LegendsOfRus
{
    public enum RuntimeEconomyActivationStatus
    {
        Activated,
        ResourceMissing,
        JsonInvalid,
        CatalogInvalid,
        CatalogBindingMismatch,
        ManifestMismatch,
        UnityVersionMismatch,
        ProductApprovalMissing,
        VerificationIncomplete,
        RuntimeApprovalMissing,
        UnsupportedPolicy
    }

    /// <summary>
    /// Side-effect-free decision returned by the runtime economy activation gate.
    /// Policy is always non-null. A failed decision exposes the frozen legacy
    /// policy, never a partially approved catalog.
    /// </summary>
    public sealed class RuntimeEconomyActivationResult
    {
        public RuntimeEconomyActivationStatus Status { get; }
        public string ReasonCode { get; }
        public string Reason { get; }
        public RuntimeEconomyPolicy Policy { get; }
        public string ComputedSourceSha256 { get; }

        public bool IsRuntimeActivated
        {
            get { return Status == RuntimeEconomyActivationStatus.Activated; }
        }

        public bool UsesLegacyFallback
        {
            get { return !IsRuntimeActivated; }
        }

        internal RuntimeEconomyActivationResult(
            RuntimeEconomyActivationStatus status,
            string reasonCode,
            string reason,
            RuntimeEconomyPolicy policy,
            string computedSourceSha256
        )
        {
            Status = status;
            ReasonCode = reasonCode ?? string.Empty;
            Reason = reason ?? string.Empty;
            Policy = policy ?? RuntimeEconomyPolicy.LegacyFallback;
            ComputedSourceSha256 = computedSourceSha256 ?? string.Empty;
        }
    }

    /// <summary>
    /// Converts the immutable Step 1 proposal into a detached Step 2 runtime
    /// policy only after source identity, Unity version, verification, and both
    /// approval boundaries match. Evaluation performs no registration, logging,
    /// save access, PlayerPrefs access, or GameState mutation.
    /// </summary>
    public static class RuntimeEconomyActivationGate
    {
        public const string CatalogResourcePath = "Config/economy";
        public const string ManifestResourcePath = "Config/economy_runtime_activation";
        public const int ManifestSchemaVersion = 1;
        public const string ManifestId = "v1.15-step02-runtime-economy-activation";
        public const string CatalogId = "v1.15-step01-proposal";
        public const string RequiredUnityVersion = "6000.5.9f1";
        public const string CatalogSourceSha256 =
            "ba66d7104c3d0a33ea4e0575b9965325cf9f90a8177c4e089c638d202267c42c";

        public static RuntimeEconomyActivationResult Load()
        {
            TextAsset catalogAsset = Resources.Load<TextAsset>(CatalogResourcePath);
            if (catalogAsset == null)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.ResourceMissing,
                    "RUNTIME_ECON_CATALOG_RESOURCE_MISSING",
                    "Config/economy.json is missing.",
                    string.Empty
                );
            }

            TextAsset manifestAsset = Resources.Load<TextAsset>(ManifestResourcePath);
            if (manifestAsset == null)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.ResourceMissing,
                    "RUNTIME_ECON_MANIFEST_RESOURCE_MISSING",
                    "Config/economy_runtime_activation.json is missing.",
                    ComputeNormalizedSourceSha256(catalogAsset.text)
                );
            }

            EconomyCatalogData catalog;
            RuntimeEconomyActivationManifestData manifest;
            try
            {
                catalog = JsonUtility.FromJson<EconomyCatalogData>(catalogAsset.text);
            }
            catch (Exception exception)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.JsonInvalid,
                    "RUNTIME_ECON_CATALOG_JSON_INVALID",
                    exception.GetType().Name + ": " + exception.Message,
                    ComputeNormalizedSourceSha256(catalogAsset.text)
                );
            }

            try
            {
                manifest = JsonUtility.FromJson<RuntimeEconomyActivationManifestData>(
                    manifestAsset.text
                );
            }
            catch (Exception exception)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.JsonInvalid,
                    "RUNTIME_ECON_MANIFEST_JSON_INVALID",
                    exception.GetType().Name + ": " + exception.Message,
                    ComputeNormalizedSourceSha256(catalogAsset.text)
                );
            }

            return Evaluate(catalog, catalogAsset.text, manifest, Application.unityVersion);
        }

        public static RuntimeEconomyActivationResult Evaluate(
            EconomyCatalogData catalog,
            string sourceJson,
            RuntimeEconomyActivationManifestData manifest,
            string unityVersion
        )
        {
            string sourceHash = ComputeNormalizedSourceSha256(sourceJson);
            if (catalog == null)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogInvalid,
                    "RUNTIME_ECON_CATALOG_NULL",
                    "The detached economy catalog is null.",
                    sourceHash
                );
            }

            if (string.IsNullOrEmpty(sourceJson))
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                    "RUNTIME_ECON_SOURCE_EMPTY",
                    "The detached economy source is empty.",
                    sourceHash
                );
            }

            EconomyValidationReport validation = EconomyCatalogValidator.Validate(catalog);
            if (validation == null || validation.verdict != EconomyVerdict.Pass)
            {
                int violationCount = validation == null || validation.violations == null
                    ? 0
                    : validation.violations.Length;
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogInvalid,
                    "RUNTIME_ECON_CATALOG_INVALID",
                    "The detached economy proposal failed validation with " +
                        violationCount + " violation(s).",
                    sourceHash
                );
            }

            EconomyCatalogData sourceCatalog;
            try
            {
                sourceCatalog = JsonUtility.FromJson<EconomyCatalogData>(sourceJson);
            }
            catch (Exception exception)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.JsonInvalid,
                    "RUNTIME_ECON_SOURCE_JSON_INVALID",
                    exception.GetType().Name + ": " + exception.Message,
                    sourceHash
                );
            }

            EconomyValidationReport sourceValidation =
                EconomyCatalogValidator.Validate(sourceCatalog);
            if (sourceValidation.verdict != EconomyVerdict.Pass)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogInvalid,
                    "RUNTIME_ECON_SOURCE_CATALOG_INVALID",
                    "The source JSON does not contain a valid proposal catalog.",
                    sourceHash
                );
            }

            if (!string.Equals(
                EconomyDeterministicJson.Serialize(catalog),
                EconomyDeterministicJson.Serialize(sourceCatalog),
                StringComparison.Ordinal
            ))
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                    "RUNTIME_ECON_DETACHED_SOURCE_MISMATCH",
                    "The detached catalog does not match the supplied source JSON.",
                    sourceHash
                );
            }

            if (manifest == null)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.ManifestMismatch,
                    "RUNTIME_ECON_MANIFEST_NULL",
                    "The runtime activation manifest is null.",
                    sourceHash
                );
            }

            if (
                manifest.schemaVersion != ManifestSchemaVersion ||
                !string.Equals(manifest.manifestId, ManifestId, StringComparison.Ordinal)
            )
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.ManifestMismatch,
                    "RUNTIME_ECON_MANIFEST_IDENTITY_MISMATCH",
                    "The activation manifest schema or id is not supported.",
                    sourceHash
                );
            }

            if (
                catalog.schemaVersion != EconomyCatalogValidator.CurrentSchemaVersion ||
                manifest.catalogSchemaVersion != catalog.schemaVersion ||
                !string.Equals(catalog.catalogId, CatalogId, StringComparison.Ordinal) ||
                !string.Equals(manifest.catalogId, catalog.catalogId, StringComparison.Ordinal)
            )
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                    "RUNTIME_ECON_CATALOG_IDENTITY_MISMATCH",
                    "The activation manifest is not bound to the supported catalog schema and id.",
                    sourceHash
                );
            }

            if (
                !string.Equals(sourceHash, CatalogSourceSha256, StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.catalogSourceSha256,
                    sourceHash,
                    StringComparison.Ordinal
                )
            )
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.CatalogBindingMismatch,
                    "RUNTIME_ECON_SOURCE_HASH_MISMATCH",
                    "The normalized proposal source SHA-256 does not match its immutable binding.",
                    sourceHash
                );
            }

            if (
                !string.Equals(unityVersion, RequiredUnityVersion, StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.unityVersion,
                    RequiredUnityVersion,
                    StringComparison.Ordinal
                )
            )
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.UnityVersionMismatch,
                    "RUNTIME_ECON_UNITY_VERSION_MISMATCH",
                    "Runtime activation requires exactly Unity " + RequiredUnityVersion + ".",
                    sourceHash
                );
            }

            if (!manifest.productApproval)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.ProductApprovalMissing,
                    "RUNTIME_ECON_PRODUCT_APPROVAL_MISSING",
                    "Product approval for Step 2 has not been recorded.",
                    sourceHash
                );
            }

            if (!manifest.compilationVerified)
            {
                return VerificationFallback(
                    "RUNTIME_ECON_COMPILATION_NOT_VERIFIED",
                    "Unity compilation has not been verified.",
                    sourceHash
                );
            }

            if (!manifest.editorSelfTestsPassed)
            {
                return VerificationFallback(
                    "RUNTIME_ECON_EDITOR_SELF_TESTS_NOT_PASSED",
                    "Editor self-tests have not been recorded as passing.",
                    sourceHash
                );
            }

            if (!manifest.editModeTestsPassed)
            {
                return VerificationFallback(
                    "RUNTIME_ECON_EDIT_MODE_TESTS_NOT_PASSED",
                    "EditMode tests have not been recorded as passing.",
                    sourceHash
                );
            }

            if (!manifest.playModeTestsPassed)
            {
                return VerificationFallback(
                    "RUNTIME_ECON_PLAY_MODE_TESTS_NOT_PASSED",
                    "PlayMode tests have not been recorded as passing.",
                    sourceHash
                );
            }

            if (!manifest.runtimeActivationApproved)
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.RuntimeApprovalMissing,
                    "RUNTIME_ECON_RUNTIME_APPROVAL_MISSING",
                    "Runtime economy activation has not been explicitly approved.",
                    sourceHash
                );
            }

            RuntimeEconomyPolicy policy;
            string policyError;
            if (!TryBuildPolicy(catalog, sourceHash, out policy, out policyError))
            {
                return Fallback(
                    RuntimeEconomyActivationStatus.UnsupportedPolicy,
                    "RUNTIME_ECON_POLICY_UNSUPPORTED",
                    policyError,
                    sourceHash
                );
            }

            return new RuntimeEconomyActivationResult(
                RuntimeEconomyActivationStatus.Activated,
                "RUNTIME_ECON_ACTIVATED",
                "The immutable catalog passed every activation boundary.",
                policy,
                sourceHash
            );
        }

        /// <summary>
        /// Hashes the source after removing a leading Unicode BOM, converting
        /// CRLF/CR to LF, and encoding as UTF-8 without a byte-order mark.
        /// No whitespace is trimmed and no terminal newline is added.
        /// </summary>
        public static string ComputeNormalizedSourceSha256(string sourceJson)
        {
            string normalized = sourceJson ?? string.Empty;
            if (normalized.Length > 0 && normalized[0] == '\uFEFF')
                normalized = normalized.Substring(1);
            normalized = normalized.Replace("\r\n", "\n").Replace("\r", "\n");

            byte[] bytes = new UTF8Encoding(false).GetBytes(normalized);
            byte[] digest;
            using (SHA256 sha256 = SHA256.Create())
                digest = sha256.ComputeHash(bytes);

            var result = new StringBuilder(digest.Length * 2);
            for (int i = 0; i < digest.Length; i++)
                result.Append(digest[i].ToString("x2"));
            return result.ToString();
        }

        static bool TryBuildPolicy(
            EconomyCatalogData catalog,
            string sourceHash,
            out RuntimeEconomyPolicy policy,
            out string error
        )
        {
            policy = null;
            error = string.Empty;
            long dailyGold;
            long dailyGems;
            if (!TryGetAmount(catalog.dailyReward.grants, "gold", out dailyGold))
            {
                error = "The activated policy requires one daily gold grant.";
                return false;
            }
            if (!TryGetAmount(catalog.dailyReward.grants, "gems", out dailyGems))
            {
                error = "The activated policy requires one daily gems grant.";
                return false;
            }

            RuntimeEconomyFirstClearReward[] firstClearRewards;
            if (!TryBuildFirstClearRewards(
                catalog.progression,
                out firstClearRewards,
                out error
            ))
            {
                return false;
            }

            policy = new RuntimeEconomyPolicy(
                catalog.catalogId,
                sourceHash,
                true,
                catalog.energy.capacity,
                catalog.energy.levelEntryCost,
                catalog.energy.retryCost,
                catalog.energy.refillIntervalMinutes,
                catalog.dailyReward.enabled,
                catalog.dailyReward.cadenceUtcDays,
                dailyGold,
                dailyGems,
                catalog.dailyReward.energyGrant,
                catalog.rewardDoubling.enabled,
                catalog.rewardDoubling.eligibleCurrencyId,
                catalog.rewardDoubling.multiplier,
                catalog.rewardDoubling.maxClaimsPerReward,
                catalog.rewardDoubling.optional,
                firstClearRewards
            );
            return true;
        }

        static bool TryBuildFirstClearRewards(
            EconomyProgressionData progression,
            out RuntimeEconomyFirstClearReward[] rewards,
            out string error
        )
        {
            rewards = new RuntimeEconomyFirstClearReward[0];
            error = string.Empty;
            if (
                progression == null ||
                progression.chapterId <= 0 ||
                progression.levels == null ||
                progression.levels.Length == 0
            )
            {
                error = "The activated policy requires progression rewards.";
                return false;
            }

            var built = new RuntimeEconomyFirstClearReward[
                progression.levels.Length
            ];
            var levelKeys = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < progression.levels.Length; i++)
            {
                EconomyProgressionLevelData level = progression.levels[i];
                if (
                    level == null ||
                    level.levelNumber <= 0 ||
                    string.IsNullOrWhiteSpace(level.id)
                )
                {
                    error = "The activated policy contains an invalid progression level.";
                    return false;
                }

                string levelKey = progression.chapterId + ":" +
                    level.levelNumber + ":" + level.id;
                if (!levelKeys.Add(levelKey))
                {
                    error = "The activated policy contains a duplicate progression level.";
                    return false;
                }

                long gold = 0L;
                long gems = 0L;
                long mace = 0L;
                long firebird = 0L;
                long livingLight = 0L;
                long seaPearl = 0L;
                var resourceIds = new HashSet<string>(StringComparer.Ordinal);
                if (!TryReadCurrencyRewards(
                    level.firstClearGrants,
                    resourceIds,
                    ref gold,
                    ref gems,
                    out error
                ))
                {
                    return false;
                }
                if (!TryReadBoosterRewards(
                    level.firstClearBoosterGrants,
                    resourceIds,
                    ref mace,
                    ref firebird,
                    ref livingLight,
                    ref seaPearl,
                    out error
                ))
                {
                    return false;
                }

                built[i] = new RuntimeEconomyFirstClearReward(
                    progression.chapterId,
                    level.levelNumber,
                    level.id,
                    gold,
                    gems,
                    mace,
                    firebird,
                    livingLight,
                    seaPearl
                );
            }

            rewards = built;
            return true;
        }

        static bool TryReadCurrencyRewards(
            EconomyAmountData[] amounts,
            HashSet<string> resourceIds,
            ref long gold,
            ref long gems,
            out string error
        )
        {
            error = string.Empty;
            EconomyAmountData[] values = amounts ?? new EconomyAmountData[0];
            for (int i = 0; i < values.Length; i++)
            {
                EconomyAmountData amount = values[i];
                if (
                    amount == null ||
                    amount.amount < 0 ||
                    !resourceIds.Add(amount.id ?? string.Empty)
                )
                {
                    error = "The activated policy contains an invalid or duplicate first-clear grant.";
                    return false;
                }

                switch (amount.id)
                {
                    case RuntimeEconomyResourceIds.Gold:
                        gold = amount.amount;
                        break;
                    case RuntimeEconomyResourceIds.Gems:
                        gems = amount.amount;
                        break;
                    default:
                        error = "The activated policy contains an unsupported first-clear currency.";
                        return false;
                }
            }
            return true;
        }

        static bool TryReadBoosterRewards(
            EconomyBoosterAmountData[] amounts,
            HashSet<string> resourceIds,
            ref long mace,
            ref long firebird,
            ref long livingLight,
            ref long seaPearl,
            out string error
        )
        {
            error = string.Empty;
            EconomyBoosterAmountData[] values = amounts ??
                new EconomyBoosterAmountData[0];
            for (int i = 0; i < values.Length; i++)
            {
                EconomyBoosterAmountData amount = values[i];
                if (
                    amount == null ||
                    amount.amount < 0 ||
                    !resourceIds.Add(amount.id ?? string.Empty)
                )
                {
                    error = "The activated policy contains an invalid or duplicate first-clear booster grant.";
                    return false;
                }

                switch (amount.id)
                {
                    case RuntimeEconomyResourceIds.BogatyrMace:
                        mace = amount.amount;
                        break;
                    case RuntimeEconomyResourceIds.Firebird:
                        firebird = amount.amount;
                        break;
                    case RuntimeEconomyResourceIds.LivingLight:
                        livingLight = amount.amount;
                        break;
                    case RuntimeEconomyResourceIds.SeaPearl:
                        seaPearl = amount.amount;
                        break;
                    default:
                        error = "The activated policy contains an unsupported first-clear booster.";
                        return false;
                }
            }
            return true;
        }

        static bool TryGetAmount(
            EconomyAmountData[] amounts,
            string id,
            out long value
        )
        {
            value = 0L;
            if (amounts == null)
                return false;
            for (int i = 0; i < amounts.Length; i++)
            {
                EconomyAmountData amount = amounts[i];
                if (amount != null && string.Equals(amount.id, id, StringComparison.Ordinal))
                {
                    value = amount.amount;
                    return true;
                }
            }
            return false;
        }

        static RuntimeEconomyActivationResult VerificationFallback(
            string reasonCode,
            string reason,
            string sourceHash
        )
        {
            return Fallback(
                RuntimeEconomyActivationStatus.VerificationIncomplete,
                reasonCode,
                reason,
                sourceHash
            );
        }

        static RuntimeEconomyActivationResult Fallback(
            RuntimeEconomyActivationStatus status,
            string reasonCode,
            string reason,
            string sourceHash
        )
        {
            return new RuntimeEconomyActivationResult(
                status,
                reasonCode,
                reason,
                RuntimeEconomyPolicy.LegacyFallback,
                sourceHash
            );
        }
    }
}
