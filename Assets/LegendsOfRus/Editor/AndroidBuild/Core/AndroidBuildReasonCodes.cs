namespace LegendsOfRus.AndroidBuild
{
    internal static class AndroidBuildReasonCodes
    {
        internal const string NonAndroidBuildIgnored =
            "ANDROID_NON_ANDROID_BUILD_IGNORED";
        internal const string DevelopmentApkAllowed =
            "ANDROID_DEVELOPMENT_APK_ALLOWED";
        internal const string PolicyMissing = "ANDROID_POLICY_MISSING";
        internal const string PolicyInvalid = "ANDROID_POLICY_INVALID";
        internal const string PolicyHashMismatch =
            "ANDROID_POLICY_HASH_MISMATCH";
        internal const string UnityVersionMismatch =
            "ANDROID_UNITY_VERSION_MISMATCH";
        internal const string ProjectSettingsMissing =
            "ANDROID_PROJECT_SETTINGS_MISSING";
        internal const string PackageLockUnapproved =
            "ANDROID_PACKAGE_LOCK_UNAPPROVED";
        internal const string ModuleUnavailable =
            "ANDROID_MODULE_UNAVAILABLE";
        internal const string TargetNotActive =
            "ANDROID_TARGET_NOT_ACTIVE";
        internal const string BuildProfileOverrideBlocked =
            "ANDROID_BUILD_PROFILE_OVERRIDE_BLOCKED";
        internal const string BootstrapSceneMismatch =
            "ANDROID_BOOTSTRAP_SCENE_MISMATCH";
        internal const string DevelopmentIdentityMismatch =
            "ANDROID_DEVELOPMENT_IDENTITY_MISMATCH";
        internal const string VersioningInvalid =
            "ANDROID_VERSIONING_INVALID";
        internal const string TargetApiMismatch =
            "ANDROID_TARGET_API_MISMATCH";
        internal const string MinimumApiUnsupported =
            "ANDROID_MINIMUM_API_UNSUPPORTED";
        internal const string Il2CppRequired = "ANDROID_IL2CPP_REQUIRED";
        internal const string Arm64Required = "ANDROID_ARM64_REQUIRED";
        internal const string GameCategoryRequired =
            "ANDROID_GAME_CATEGORY_REQUIRED";
        internal const string PortraitRequired =
            "ANDROID_PORTRAIT_REQUIRED";
        internal const string CommercialBoundaryChanged =
            "ANDROID_COMMERCIAL_BOUNDARY_CHANGED";
        internal const string ScriptingDefinesUnsafe =
            "ANDROID_SCRIPTING_DEFINES_UNSAFE";
        internal const string ScheduledTargetMismatch =
            "ANDROID_SCHEDULED_TARGET_MISMATCH";
        internal const string EditorPlayModeActive = "ANDROID_EDITOR_PLAY_MODE_ACTIVE";
        internal const string DevelopmentBuildRequired =
            "ANDROID_DEVELOPMENT_BUILD_REQUIRED";
        internal const string BuildOptionsUnsafe =
            "ANDROID_BUILD_OPTIONS_UNSAFE";
        internal const string AssetBundleManifestBlocked =
            "ANDROID_ASSET_BUNDLE_MANIFEST_BLOCKED";
        internal const string CustomSigningBlocked =
            "ANDROID_CUSTOM_SIGNING_BLOCKED";
        internal const string GradleExportBlocked =
            "ANDROID_GRADLE_EXPORT_BLOCKED";
        internal const string AppBundleBlocked =
            "ANDROID_APP_BUNDLE_BLOCKED";
        internal const string SplitApkBlocked =
            "ANDROID_SPLIT_APK_BLOCKED";
        internal const string SplitApplicationBinaryBlocked =
            "ANDROID_SPLIT_APPLICATION_BINARY_BLOCKED";
        internal const string NativeDebugSymbolsBlocked =
            "ANDROID_NATIVE_DEBUG_SYMBOLS_BLOCKED";
        internal const string ReleaseBuildBlocked =
            "ANDROID_RELEASE_BUILD_BLOCKED";
        internal const string ProductionSwitchDisabled =
            "ANDROID_PRODUCTION_SWITCH_DISABLED";
        internal const string OutputPathInvalid =
            "ANDROID_OUTPUT_PATH_INVALID";
        internal const string SettingsReadFailed =
            "ANDROID_SETTINGS_READ_FAILED";
        internal const string UnexpectedFailure =
            "ANDROID_UNEXPECTED_FAILURE";
    }
}
