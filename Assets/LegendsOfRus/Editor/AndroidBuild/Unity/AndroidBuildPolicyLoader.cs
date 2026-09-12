#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    internal sealed class AndroidBuildPolicyLoadResult
    {
        internal AndroidBuildPolicyLoadResult(
            AndroidBuildFoundationPolicy policy,
            bool filePresent,
            bool hashMatches,
            bool isValid,
            string reasonCode)
        {
            Policy = policy;
            FilePresent = filePresent;
            HashMatches = hashMatches;
            IsValid = isValid;
            ReasonCode = reasonCode;
        }

        internal AndroidBuildFoundationPolicy Policy { get; private set; }
        internal bool FilePresent { get; private set; }
        internal bool HashMatches { get; private set; }
        internal bool IsValid { get; private set; }
        internal string ReasonCode { get; private set; }
    }

    internal static class AndroidBuildPolicyLoader
    {
        internal const string AssetPath =
            "Assets/LegendsOfRus/Editor/AndroidBuild/Config/" +
            "android_build_foundation_policy.json";
        internal const string ExpectedSourceSha256 =
            "95eec67ffb9ef8e45ad657524cbfcaa3c852258f4de2c7a54dc53ad18f96751c";

        static readonly string[] ExpectedKeys =
        {
            "schemaVersion",
            "policyId",
            "unityVersion",
            "mode",
            "requiredScene",
            "developmentApplicationIdentifier",
            "developmentBundleVersion",
            "developmentBundleVersionCode",
            "developmentOutputRelativePath",
            "requiredTargetApiLevel",
            "minimumSupportedApiLevel",
            "maximumVersionCode",
            "requireIl2Cpp",
            "requireArm64",
            "requireGameCategory",
            "requirePortrait",
            "allowDevelopmentApk",
            "allowReleaseApk",
            "allowAppBundle",
            "allowGradleExport",
            "allowCustomSigning",
            "productionApplicationIdentifierApproved",
            "versioningApproved",
            "sdkPolicyApproved",
            "signingIdentityApproved",
            "playAppSigningApproved",
            "nativePluginInventoryApproved",
            "sixteenKbPageCompatibilityVerified",
            "dataSafetyApproved",
            "productionBuildApproved"
        };

        internal static AndroidBuildPolicyLoadResult Load()
        {
            bool filePresent = false;
            try
            {
                string fullPath = FullPath(AssetPath);
                filePresent = File.Exists(fullPath);
                if (!filePresent)
                {
                    return Failed(
                        false,
                        false,
                        AndroidBuildReasonCodes.PolicyMissing
                    );
                }

                byte[] bytes = File.ReadAllBytes(fullPath);
                bool hashMatches = string.Equals(
                    Sha256(bytes),
                    ExpectedSourceSha256,
                    StringComparison.Ordinal
                );
                if (!hashMatches)
                {
                    return Failed(
                        true,
                        false,
                        AndroidBuildReasonCodes.PolicyHashMismatch
                    );
                }

                string source = new UTF8Encoding(false, true).GetString(bytes);
                if (!HasExactKeys(source))
                {
                    return Failed(
                        true,
                        true,
                        AndroidBuildReasonCodes.PolicyInvalid
                    );
                }

                AndroidBuildFoundationPolicy policy =
                    JsonUtility.FromJson<AndroidBuildFoundationPolicy>(source);
                if (!IsExactFoundationPolicy(policy))
                {
                    return Failed(
                        true,
                        true,
                        AndroidBuildReasonCodes.PolicyInvalid
                    );
                }

                return new AndroidBuildPolicyLoadResult(
                    policy,
                    true,
                    true,
                    true,
                    AndroidBuildReasonCodes.DevelopmentApkAllowed
                );
            }
            catch
            {
                return Failed(
                    filePresent,
                    false,
                    AndroidBuildReasonCodes.PolicyInvalid
                );
            }
        }

        internal static bool HasExactKeys(string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            MatchCollection matches = Regex.Matches(
                source,
                "\\\"(?<key>[A-Za-z][A-Za-z0-9]*)\\\"\\s*:"
            );
            foreach (Match match in matches)
            {
                string key = match.Groups["key"].Value;
                int count;
                counts.TryGetValue(key, out count);
                counts[key] = count + 1;
            }

            if (counts.Count != ExpectedKeys.Length)
                return false;
            foreach (string key in ExpectedKeys)
            {
                int count;
                if (!counts.TryGetValue(key, out count) || count != 1)
                    return false;
            }
            return true;
        }

        internal static bool IsExactFoundationPolicy(
            AndroidBuildFoundationPolicy value)
        {
            return
                value != null &&
                value.schemaVersion == 1 &&
                value.policyId ==
                    "v1.17-step01-android-build-signing-foundation" &&
                value.unityVersion == "6000.5.9f1" &&
                value.mode == "development-apk-only" &&
                value.requiredScene == "Assets/Scenes/Bootstrap.unity" &&
                value.developmentApplicationIdentifier ==
                    "com.example.legendsofrus.v117debug" &&
                value.developmentBundleVersion == "0.0.0" &&
                value.developmentBundleVersionCode == 1 &&
                value.developmentOutputRelativePath ==
                    "Builds/Verification/v1_17_step01/" +
                    "LegendsOfRus-v1.17-step01-development.apk" &&
                value.requiredTargetApiLevel == 36 &&
                value.minimumSupportedApiLevel == 26 &&
                value.maximumVersionCode == 2100000000 &&
                value.requireIl2Cpp &&
                value.requireArm64 &&
                value.requireGameCategory &&
                value.requirePortrait &&
                value.allowDevelopmentApk &&
                !value.allowReleaseApk &&
                !value.allowAppBundle &&
                !value.allowGradleExport &&
                !value.allowCustomSigning &&
                !value.productionApplicationIdentifierApproved &&
                !value.versioningApproved &&
                !value.sdkPolicyApproved &&
                !value.signingIdentityApproved &&
                !value.playAppSigningApproved &&
                !value.nativePluginInventoryApproved &&
                !value.sixteenKbPageCompatibilityVerified &&
                !value.dataSafetyApproved &&
                !value.productionBuildApproved;
        }

        static AndroidBuildPolicyLoadResult Failed(
            bool filePresent,
            bool hashMatches,
            string reasonCode)
        {
            return new AndroidBuildPolicyLoadResult(
                null,
                filePresent,
                hashMatches,
                false,
                reasonCode
            );
        }

        static string FullPath(string relativePath)
        {
            string root = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..")
            );
            return Path.GetFullPath(Path.Combine(root, relativePath));
        }

        static string Sha256(byte[] bytes)
        {
            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(bytes);
                return BitConverter.ToString(hash)
                    .Replace("-", string.Empty)
                    .ToLowerInvariant();
            }
        }
    }
}
#endif
