using System;

namespace LegendsOfRus.AndroidBuild
{
    [Serializable]
    internal sealed class AndroidBuildFoundationPolicy
    {
        public int schemaVersion;
        public string policyId;
        public string unityVersion;
        public string mode;
        public string requiredScene;
        public string developmentApplicationIdentifier;
        public string developmentBundleVersion;
        public int developmentBundleVersionCode;
        public string developmentOutputRelativePath;
        public int requiredTargetApiLevel;
        public int minimumSupportedApiLevel;
        public int maximumVersionCode;
        public bool requireIl2Cpp;
        public bool requireArm64;
        public bool requireGameCategory;
        public bool requirePortrait;
        public bool allowDevelopmentApk;
        public bool allowReleaseApk;
        public bool allowAppBundle;
        public bool allowGradleExport;
        public bool allowCustomSigning;
        public bool productionApplicationIdentifierApproved;
        public bool versioningApproved;
        public bool sdkPolicyApproved;
        public bool signingIdentityApproved;
        public bool playAppSigningApproved;
        public bool nativePluginInventoryApproved;
        public bool sixteenKbPageCompatibilityVerified;
        public bool dataSafetyApproved;
        public bool productionBuildApproved;
    }

    internal enum AndroidBuildIntent
    {
        NonAndroid = 0,
        DevelopmentApk = 1,
        ReleaseApk = 2,
        AppBundle = 3,
        GradleExport = 4
    }

    internal sealed class AndroidBuildFacts
    {
        internal bool IsAndroidBuild { get; set; }
        internal bool PolicyFilePresent { get; set; }
        internal bool PolicyHashMatches { get; set; }
        internal bool PolicyValid { get; set; }
        internal bool SettingsReadSucceeded { get; set; }
        internal bool UnityVersionMatches { get; set; }
        internal bool ProjectSettingsPresent { get; set; }
        internal bool PackageLockApproved { get; set; }
        internal bool AndroidModuleAvailable { get; set; }
        internal bool AndroidTargetActive { get; set; }
        internal bool BuildProfileOverrideAbsent { get; set; }
        internal bool BootstrapSceneOnly { get; set; }
        internal bool DevelopmentIdentityMatches { get; set; }
        internal bool DevelopmentVersionMatches { get; set; }
        internal bool TargetApiMatches { get; set; }
        internal bool MinimumApiSupported { get; set; }
        internal bool Il2CppEnabled { get; set; }
        internal bool Arm64Enabled { get; set; }
        internal bool GameCategoryConfigured { get; set; }
        internal bool PortraitConfigured { get; set; }
        internal bool DevelopmentBuild { get; set; }
        internal bool AppBundle { get; set; }
        internal bool GradleExport { get; set; }
        internal bool SplitApkPerCpuArchitecture { get; set; }
        internal bool SplitApplicationBinary { get; set; }
        internal bool CustomSigning { get; set; }
        internal bool NativeDebugSymbolsDisabled { get; set; }
        internal bool ScriptingDefinesSafe { get; set; }
        internal bool OutputPathApproved { get; set; }
        internal bool OutputPathValidationDeferred { get; set; }
        internal bool CommercialBoundaryIntact { get; set; }
        internal bool ProductionAndroidBuildCompiled { get; set; }

    }

    internal sealed class AndroidBuildDecision
    {
        internal AndroidBuildDecision(
            bool allowed,
            AndroidBuildIntent intent,
            string reasonCode)
        {
            Allowed = allowed;
            Intent = intent;
            ReasonCode = reasonCode ?? AndroidBuildReasonCodes.UnexpectedFailure;
        }

        internal bool Allowed { get; private set; }
        internal AndroidBuildIntent Intent { get; private set; }
        internal string ReasonCode { get; private set; }
    }
}
