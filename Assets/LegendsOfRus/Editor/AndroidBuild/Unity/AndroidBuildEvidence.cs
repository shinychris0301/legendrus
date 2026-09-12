#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    [Serializable]
    internal sealed class AndroidBuildEvidence
    {
        public int schemaVersion;
        public string milestone;
        public string status;
        public string intent;
        public string reasonCode;
        public string expectedPolicySourceSha256;
        public bool policyValid;
        public bool exactUnityVersion;
        public bool projectSettingsPresent;
        public bool packageLockApproved;
        public bool androidModuleAvailable;
        public bool androidTargetActive;
        public bool buildProfileOverrideAbsent;
        public bool bootstrapSceneOnly;
        public bool exactDevelopmentIdentity;
        public bool exactDevelopmentVersion;
        public bool targetApi36;
        public bool minimumApiSupported;
        public bool il2Cpp;
        public bool arm64;
        public bool gameCategory;
        public bool portrait;
        public bool developmentBuild;
        public bool appBundle;
        public bool gradleExport;
        public bool splitApkPerCpuArchitecture;
        public bool splitApplicationBinary;
        public bool customSigning;
        public bool nativeDebugSymbolsDisabled;
        public bool scriptingDefinesSafe;
        public bool outputPathValidated;
        public bool outputPathValidationDeferred;
        public bool commercialBoundaryIntact;
        public bool productionAndroidBuildCompiled;
        public bool productionBuildAuthorized;

        internal static AndroidBuildEvidence From(
            AndroidBuildPreflightResult result)
        {
            AndroidBuildFacts facts = result != null ? result.Facts : null;
            AndroidBuildDecision decision = result != null
                ? result.Decision
                : null;
            return new AndroidBuildEvidence
            {
                schemaVersion = 1,
                milestone = "v1.17-step01",
                status = decision != null && decision.Allowed
                    ? "DEVELOPMENT_APK_PREFLIGHT_PASSED"
                    : "DEVELOPMENT_APK_PREFLIGHT_BLOCKED",
                intent = decision != null
                    ? decision.Intent.ToString()
                    : AndroidBuildIntent.DevelopmentApk.ToString(),
                reasonCode = decision != null
                    ? decision.ReasonCode
                    : AndroidBuildReasonCodes.UnexpectedFailure,
                expectedPolicySourceSha256 =
                    AndroidBuildPolicyLoader.ExpectedSourceSha256,
                policyValid = facts != null && facts.PolicyValid,
                exactUnityVersion =
                    facts != null && facts.UnityVersionMatches,
                projectSettingsPresent =
                    facts != null && facts.ProjectSettingsPresent,
                packageLockApproved =
                    facts != null && facts.PackageLockApproved,
                androidModuleAvailable =
                    facts != null && facts.AndroidModuleAvailable,
                androidTargetActive =
                    facts != null && facts.AndroidTargetActive,
                buildProfileOverrideAbsent =
                    facts != null && facts.BuildProfileOverrideAbsent,
                bootstrapSceneOnly =
                    facts != null && facts.BootstrapSceneOnly,
                exactDevelopmentIdentity =
                    facts != null && facts.DevelopmentIdentityMatches,
                exactDevelopmentVersion =
                    facts != null && facts.DevelopmentVersionMatches,
                targetApi36 = facts != null && facts.TargetApiMatches,
                minimumApiSupported =
                    facts != null && facts.MinimumApiSupported,
                il2Cpp = facts != null && facts.Il2CppEnabled,
                arm64 = facts != null && facts.Arm64Enabled,
                gameCategory =
                    facts != null && facts.GameCategoryConfigured,
                portrait = facts != null && facts.PortraitConfigured,
                developmentBuild =
                    facts != null && facts.DevelopmentBuild,
                appBundle = facts != null && facts.AppBundle,
                gradleExport = facts != null && facts.GradleExport,
                splitApkPerCpuArchitecture =
                    facts != null && facts.SplitApkPerCpuArchitecture,
                splitApplicationBinary =
                    facts != null && facts.SplitApplicationBinary,
                customSigning = facts != null && facts.CustomSigning,
                nativeDebugSymbolsDisabled =
                    facts != null && facts.NativeDebugSymbolsDisabled,
                scriptingDefinesSafe =
                    facts != null && facts.ScriptingDefinesSafe,
                outputPathValidated =
                    facts != null &&
                    facts.OutputPathApproved &&
                    !facts.OutputPathValidationDeferred,
                outputPathValidationDeferred =
                    facts != null && facts.OutputPathValidationDeferred,
                commercialBoundaryIntact =
                    facts != null && facts.CommercialBoundaryIntact,
                productionAndroidBuildCompiled =
                    facts != null && facts.ProductionAndroidBuildCompiled,
                productionBuildAuthorized = false
            };
        }

        internal string ToSanitizedJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
#endif
