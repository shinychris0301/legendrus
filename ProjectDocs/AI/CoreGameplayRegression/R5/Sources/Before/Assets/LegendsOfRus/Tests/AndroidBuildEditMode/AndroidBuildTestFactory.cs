#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using LegendsOfRus.AndroidBuild;

namespace LegendsOfRus.Tests.AndroidBuild
{
    internal static class AndroidBuildTestFactory
    {
        internal static AndroidBuildFoundationPolicy Policy()
        {
            return new AndroidBuildFoundationPolicy
            {
                schemaVersion = 1,
                policyId = "v1.17-step01-android-build-signing-foundation",
                unityVersion = "6000.5.9f1",
                mode = "development-apk-only",
                requiredScene = "Assets/Scenes/Bootstrap.unity",
                developmentApplicationIdentifier =
                    "com.example.legendsofrus.v117debug",
                developmentBundleVersion = "0.0.0",
                developmentBundleVersionCode = 1,
                developmentOutputRelativePath =
                    "Builds/Verification/v1_17_step01/" +
                    "LegendsOfRus-v1.17-step01-development.apk",
                requiredTargetApiLevel = 36,
                minimumSupportedApiLevel = 26,
                maximumVersionCode = 2100000000,
                requireIl2Cpp = true,
                requireArm64 = true,
                requireGameCategory = true,
                requirePortrait = true,
                allowDevelopmentApk = true,
                allowReleaseApk = false,
                allowAppBundle = false,
                allowGradleExport = false,
                allowCustomSigning = false,
                productionApplicationIdentifierApproved = false,
                versioningApproved = false,
                sdkPolicyApproved = false,
                signingIdentityApproved = false,
                playAppSigningApproved = false,
                nativePluginInventoryApproved = false,
                sixteenKbPageCompatibilityVerified = false,
                dataSafetyApproved = false,
                productionBuildApproved = false
            };
        }

        internal static AndroidBuildFacts AllowedFacts()
        {
            return new AndroidBuildFacts
            {
                IsAndroidBuild = true,
                PolicyFilePresent = true,
                PolicyHashMatches = true,
                PolicyValid = true,
                SettingsReadSucceeded = true,
                UnityVersionMatches = true,
                ProjectSettingsPresent = true,
                PackageLockApproved = true,
                AndroidModuleAvailable = true,
                AndroidTargetActive = true,
                BuildProfileOverrideAbsent = true,
                BootstrapSceneOnly = true,
                DevelopmentIdentityMatches = true,
                DevelopmentVersionMatches = true,
                TargetApiMatches = true,
                MinimumApiSupported = true,
                Il2CppEnabled = true,
                Arm64Enabled = true,
                GameCategoryConfigured = true,
                PortraitConfigured = true,
                DevelopmentBuild = true,
                AppBundle = false,
                GradleExport = false,
                CustomSigning = false,
                NativeDebugSymbolsDisabled = true,
                ScriptingDefinesSafe = true,
                OutputPathApproved = true,
                CommercialBoundaryIntact = true,
                ProductionAndroidBuildCompiled = false
            };
        }
    }
}
#endif
