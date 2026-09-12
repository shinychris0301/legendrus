#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    internal static class AndroidBuildStateReader
    {
        const string PackageManifestSha256 =
            "03924bef7f475ba9f8ae358f7504960715b12362be2668361aa9411ea95e4dfc";
        const string ProjectVersionSha256 =
            "e06e05029943a1f6537d7cf3ca00ae2fb38dfe68d41075504802051e78c1d707";
        const string EditorBuildSettingsSha256 =
            "fd8634901fc667466dfc8f08d09c69a0237e50c1e8a4746980ff1a72e445101d";
        const string EconomyActivationSha256 =
            "4c4a3d062257addf9e3bd50008a5bde281aa3e227bffe097982b7890bd597248";
        const string CommercialActivationSha256 =
            "67f31982d6eafcc54f644f8ad1e81f672ddeefe3856da4eaae6b71af3bb753e9";
        const string CommercialConfigSha256 =
            "d5e7705c47f0a7368fa618db0740703e4e0628a2b99ab5318681933f00866c02";
        // Reviewed local development baseline. Approval requires these exact package bytes;
        // merely having a generated lock never grants build permission.
        internal const string ResolvedPackageLockSha256 =
            "86ca321b25b56367b6bfc109765ca1cc718931c98a81b9bb5bdf4d2a584bc622";

        static readonly HashSet<string> ForbiddenExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jks", ".keystore", ".p12", ".pfx", ".pem", ".key",
                ".aar", ".jar", ".so", ".apk", ".aab", ".apks", ".rsp",
                ".dll", ".unitypackage", ".xapk", ".java", ".kt"
            };

        static readonly HashSet<string> ForbiddenNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AndroidManifest.xml",
                "baseProjectTemplate.gradle",
                "gradle.properties",
                "gradleTemplate.properties",
                "launcherTemplate.gradle",
                "local.properties",
                "mainTemplate.gradle",
                "settingsTemplate.gradle",
                "google-services.json",
                "GoogleService-Info.plist"
            };

        internal static AndroidBuildFacts ReadForBuild(
            BuildReport report,
            AndroidBuildPolicyLoadResult policyLoad,
            bool productionAndroidBuildCompiled)
        {
            if (report == null)
                return FailedAndroidFacts(policyLoad);

            bool isAndroid = report.summary.platform == BuildTarget.Android;
            if (!isAndroid)
            {
                return new AndroidBuildFacts
                {
                    IsAndroidBuild = false,
                    SettingsReadSucceeded = true
                };
            }

            bool development =
                (report.summary.options & BuildOptions.Development) != 0;
            return Read(
                policyLoad,
                development,
                EditorUserBuildSettings.buildAppBundle,
                EditorUserBuildSettings.exportAsGoogleAndroidProject,
                report.summary.outputPath,
                false,
                productionAndroidBuildCompiled
            );
        }

        internal static AndroidBuildFacts ReadForScheduledBuild(
            BuildPlayerOptions options,
            AndroidBuildPolicyLoadResult policyLoad,
            bool productionAndroidBuildCompiled)
        {
            if (options.target != BuildTarget.Android)
                return new AndroidBuildFacts { IsAndroidBuild = false, SettingsReadSucceeded = true };
            return Read(policyLoad,
                (options.options & BuildOptions.Development) != 0,
                EditorUserBuildSettings.buildAppBundle,
                EditorUserBuildSettings.exportAsGoogleAndroidProject,
                options.locationPathName, false, productionAndroidBuildCompiled);
        }

        // Read-only fingerprints for diagnosing inherited policy pins. These do not approve them.
        internal static AndroidBuildFix03FileEvidence[] ReadProtectedFileEvidence()
        {
            string root = ProjectRoot();
            return new[]
            {
                AndroidBuildFix03Diagnostics.Fingerprint(root, "Packages/manifest.json", PackageManifestSha256),
                AndroidBuildFix03Diagnostics.Fingerprint(root, "ProjectSettings/ProjectVersion.txt", ProjectVersionSha256),
                AndroidBuildFix03Diagnostics.Fingerprint(root, "ProjectSettings/EditorBuildSettings.asset", EditorBuildSettingsSha256),
                AndroidBuildFix03Diagnostics.Fingerprint(root,
                    "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json", EconomyActivationSha256),
                AndroidBuildFix03Diagnostics.Fingerprint(root,
                    "Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json", CommercialActivationSha256),
                AndroidBuildFix03Diagnostics.Fingerprint(root,
                    "Assets/LegendsOfRus/Resources/Config/commercial_runtime_config.json", CommercialConfigSha256)
            };
        }

        internal static AndroidBuildFacts ReadForPreflight(
            AndroidBuildPolicyLoadResult policyLoad,
            bool productionAndroidBuildCompiled)
        {
            return Read(
                policyLoad,
                EditorUserBuildSettings.development,
                EditorUserBuildSettings.buildAppBundle,
                EditorUserBuildSettings.exportAsGoogleAndroidProject,
                null,
                true,
                productionAndroidBuildCompiled
            );
        }

        static AndroidBuildFacts Read(
            AndroidBuildPolicyLoadResult policyLoad,
            bool development,
            bool appBundle,
            bool gradleExport,
            string outputPath,
            bool preflight,
            bool productionAndroidBuildCompiled)
        {
            AndroidBuildFoundationPolicy policy = policyLoad != null
                ? policyLoad.Policy
                : null;
            AndroidBuildFacts facts = CreatePolicyFacts(
                policyLoad,
                productionAndroidBuildCompiled
            );
            facts.IsAndroidBuild = true;
            facts.DevelopmentBuild = development;
            facts.AppBundle = appBundle;
            facts.GradleExport = gradleExport;

            try
            {
                string root = ProjectRoot();
                facts.UnityVersionMatches =
                    policy != null &&
                    string.Equals(
                        Application.unityVersion,
                        policy.unityVersion,
                        StringComparison.Ordinal
                    );
                facts.ProjectSettingsPresent = File.Exists(
                    Path.Combine(root, "ProjectSettings/ProjectSettings.asset")
                );
                facts.PackageLockApproved = ResolvedPackageBaselineIsExact(root);
                facts.AndroidModuleAvailable =
                    BuildPipeline.IsBuildTargetSupported(
                        BuildTargetGroup.Android,
                        BuildTarget.Android
                    );
                facts.AndroidTargetActive =
                    EditorUserBuildSettings.activeBuildTarget ==
                    BuildTarget.Android;
                facts.BuildProfileOverrideAbsent = CustomBuildProfileAbsent();
                facts.BootstrapSceneOnly =
                    policy != null && HasExactEnabledScene(policy.requiredScene);
                facts.DevelopmentIdentityMatches =
                    policy != null &&
                    string.Equals(
                        PlayerSettings.GetApplicationIdentifier(
                            NamedBuildTarget.Android
                        ),
                        policy.developmentApplicationIdentifier,
                        StringComparison.Ordinal
                    );
                facts.DevelopmentVersionMatches =
                    policy != null &&
                    string.Equals(
                        PlayerSettings.bundleVersion,
                        policy.developmentBundleVersion,
                        StringComparison.Ordinal
                    ) &&
                    PlayerSettings.Android.bundleVersionCode ==
                        policy.developmentBundleVersionCode &&
                    PlayerSettings.Android.bundleVersionCode > 0 &&
                    PlayerSettings.Android.bundleVersionCode <=
                        policy.maximumVersionCode;
                int targetApi = (int)PlayerSettings.Android.targetSdkVersion;
                int minimumApi = (int)PlayerSettings.Android.minSdkVersion;
                facts.TargetApiMatches =
                    policy != null &&
                    targetApi == policy.requiredTargetApiLevel;
                facts.MinimumApiSupported =
                    policy != null &&
                    minimumApi >= policy.minimumSupportedApiLevel &&
                    minimumApi <= policy.requiredTargetApiLevel;
                facts.Il2CppEnabled =
                    PlayerSettings.GetScriptingBackend(
                        NamedBuildTarget.Android
                    ) == ScriptingImplementation.IL2CPP;
                facts.Arm64Enabled =
                    (PlayerSettings.Android.targetArchitectures &
                        AndroidArchitecture.ARM64) != 0;
                facts.GameCategoryConfigured = string.Equals(
                        PlayerSettings.Android.appCategory,
                        "game",
                        StringComparison.Ordinal
                    ) && LegacyAndroidGameCategoryEnabled();
                facts.PortraitConfigured =
                    PlayerSettings.defaultInterfaceOrientation ==
                    UIOrientation.Portrait;
                facts.CustomSigning =
                    PlayerSettings.Android.useCustomKeystore;
                facts.SplitApkPerCpuArchitecture =
                    PlayerSettings.Android.buildApkPerCpuArchitecture;
                facts.SplitApplicationBinary =
                    PlayerSettings.Android.splitApplicationBinary;
                facts.NativeDebugSymbolsDisabled =
                    NativeDebugSymbolsDisabled();
                facts.ScriptingDefinesSafe = string.IsNullOrEmpty(
                    PlayerSettings.GetScriptingDefineSymbols(
                        NamedBuildTarget.Android
                    )
                );
                facts.OutputPathValidationDeferred = preflight;
                facts.OutputPathApproved =
                    policy != null &&
                    !preflight &&
                    OutputIsExact(root, outputPath, policy);
                facts.CommercialBoundaryIntact =
                    ProtectedBoundaryIsExact(root) &&
                    NoForbiddenAndroidArtifacts(root);
                facts.SettingsReadSucceeded = true;
                return facts;
            }
            catch
            {
                facts.SettingsReadSucceeded = false;
                return facts;
            }
        }

        static AndroidBuildFacts CreatePolicyFacts(
            AndroidBuildPolicyLoadResult policyLoad,
            bool productionAndroidBuildCompiled)
        {
            return new AndroidBuildFacts
            {
                PolicyFilePresent =
                    policyLoad != null && policyLoad.FilePresent,
                PolicyHashMatches =
                    policyLoad != null && policyLoad.HashMatches,
                PolicyValid = policyLoad != null && policyLoad.IsValid,
                ProductionAndroidBuildCompiled =
                    productionAndroidBuildCompiled
            };
        }

        static AndroidBuildFacts FailedAndroidFacts(
            AndroidBuildPolicyLoadResult policyLoad)
        {
            AndroidBuildFacts facts = CreatePolicyFacts(policyLoad, false);
            facts.IsAndroidBuild = true;
            facts.SettingsReadSucceeded = false;
            return facts;
        }

        static bool HasExactEnabledScene(string requiredScene)
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene != null && scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            return
                scenes.Length == 1 &&
                string.Equals(
                    scenes[0],
                    requiredScene,
                    StringComparison.Ordinal
                );
        }

        internal static bool CustomBuildProfileAbsent()
        {
            if (BuildProfile.GetActiveBuildProfile() != null)
                return false;

            string[] profileGuids =
                AssetDatabase.FindAssets("t:BuildProfile");
            return profileGuids != null && profileGuids.Length == 0;
        }

        internal static bool NativeDebugSymbolsDisabled()
        {
            try
            {
                Type type = Type.GetType(
                    "UnityEditor.Android.UserBuildSettings+DebugSymbols, " +
                    "UnityEditor.Android.Extensions",
                    false
                );
                if (type == null)
                    return false;

                PropertyInfo levelProperty = type.GetProperty(
                    "level",
                    BindingFlags.Public | BindingFlags.Static
                );
                PropertyInfo formatProperty = type.GetProperty(
                    "format",
                    BindingFlags.Public | BindingFlags.Static
                );
                if (levelProperty == null || !levelProperty.CanRead ||
                    levelProperty.GetIndexParameters().Length != 0 ||
                    formatProperty == null || !formatProperty.CanRead ||
                    formatProperty.GetIndexParameters().Length != 0)
                    return false;

                Type levelType = levelProperty.PropertyType;
                Type formatType = formatProperty.PropertyType;
                if (!levelType.IsEnum ||
                    !string.Equals(
                        levelType.FullName,
                        "Unity.Android.Types.DebugSymbolLevel",
                        StringComparison.Ordinal
                    ) ||
                    !formatType.IsEnum ||
                    !string.Equals(
                        formatType.FullName,
                        "Unity.Android.Types.DebugSymbolFormat",
                        StringComparison.Ordinal
                    ) ||
                    !Enum.IsDefined(levelType, "None"))
                    return false;

                object expected = Enum.Parse(levelType, "None", false);
                object actualLevel = levelProperty.GetValue(null, null);
                object actualFormat = formatProperty.GetValue(null, null);
                if (actualLevel == null || actualFormat == null)
                    return false;

                ulong allowedFormatBits = 0;
                foreach (string name in new[]
                {
                    "Zip", "IncludeInBundle", "LegacyExtensions"
                })
                {
                    allowedFormatBits |= Convert.ToUInt64(
                        Enum.Parse(formatType, name, false)
                    );
                }
                ulong actualFormatBits = Convert.ToUInt64(actualFormat);
                return
                    (actualFormatBits & ~allowedFormatBits) == 0 &&
                    actualLevel.Equals(expected);
            }
            catch
            {
                return false;
            }
        }

        internal static bool LegacyAndroidGameCategoryEnabled()
        {
            try
            {
                Type androidType = typeof(PlayerSettings).GetNestedType(
                    "Android",
                    BindingFlags.Public | BindingFlags.NonPublic
                );
                PropertyInfo property = androidType != null
                    ? androidType.GetProperty(
                        "androidIsGame",
                        BindingFlags.Public | BindingFlags.Static
                    )
                    : null;
                if (property == null || !property.CanRead ||
                    property.GetIndexParameters().Length != 0 ||
                    property.PropertyType != typeof(bool))
                    return false;
                object value = property.GetValue(null, null);
                return value is bool && (bool)value;
            }
            catch
            {
                return false;
            }
        }

        internal static bool ScheduledOutputIsExact(
            string outputPath,
            AndroidBuildFoundationPolicy policy)
        {
            return OutputIsExact(ProjectRoot(), outputPath, policy);
        }

        static bool OutputIsExact(
            string root,
            string outputPath,
            AndroidBuildFoundationPolicy policy)
        {
            if (string.IsNullOrEmpty(outputPath) || policy == null)
                return false;

            string supplied = Path.IsPathRooted(outputPath)
                ? Path.GetFullPath(outputPath)
                : Path.GetFullPath(Path.Combine(root, outputPath));
            string expected = Path.GetFullPath(
                Path.Combine(root, policy.developmentOutputRelativePath)
            );
            return
                ExactOutputPathTextMatches(supplied, expected) &&
                string.Equals(
                    Path.GetExtension(supplied),
                    ".apk",
                    StringComparison.OrdinalIgnoreCase
                ) &&
                !HasExistingReparsePoint(root, supplied);
        }

        static bool HasExistingReparsePoint(string root, string filePath)
        {
            const StringComparison comparison = StringComparison.Ordinal;
            string normalizedRoot = Path.GetFullPath(root).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            );
            string normalizedFile = Path.GetFullPath(filePath);
            string rootPrefix = normalizedRoot + Path.DirectorySeparatorChar;
            if (!normalizedFile.StartsWith(rootPrefix, comparison))
                return true;

            try
            {
                if ((new DirectoryInfo(normalizedRoot).Attributes &
                    FileAttributes.ReparsePoint) != 0)
                    return true;
            }
            catch
            {
                return true;
            }

            string relative = normalizedFile.Substring(rootPrefix.Length);
            string[] parts = relative.Split(
                new[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                },
                StringSplitOptions.RemoveEmptyEntries
            );
            string current = normalizedRoot;
            for (int index = 0; index < parts.Length; index++)
            {
                if (!Directory.Exists(current))
                    return false;

                FileSystemInfo exactMatch = null;
                var foldedMatches = new List<FileSystemInfo>();
                try
                {
                    foreach (FileSystemInfo entry in
                        new DirectoryInfo(current).EnumerateFileSystemInfos())
                    {
                        if (string.Equals(
                            entry.Name,
                            parts[index],
                            StringComparison.Ordinal))
                            exactMatch = entry;
                        if (CaseFoldedFileSystemNameMatches(
                            entry.Name,
                            parts[index]))
                            foldedMatches.Add(entry);
                    }
                }
                catch
                {
                    return true;
                }

                if (foldedMatches.Count > 1)
                    return true;

                FileSystemInfo match = exactMatch;
                if (match == null && foldedMatches.Count == 1)
                    match = foldedMatches[0];

                if (match == null)
                    return false;

                FileAttributes attributes;
                try
                {
                    attributes = match.Attributes;
                }
                catch
                {
                    return true;
                }
                if ((attributes & FileAttributes.ReparsePoint) != 0)
                    return true;
                if (index < parts.Length - 1 &&
                    (attributes & FileAttributes.Directory) == 0)
                    return true;

                current = match.FullName;
            }
            return false;
        }

        internal static bool ExactOutputPathTextMatches(
            string supplied,
            string expected)
        {
            return string.Equals(
                supplied,
                expected,
                StringComparison.Ordinal
            );
        }

        internal static bool CaseFoldedFileSystemNameMatches(
            string left,
            string right)
        {
            return string.Equals(
                left,
                right,
                StringComparison.OrdinalIgnoreCase
            );
        }

        internal static bool ResolvedPackageBaselineIsExact(string root)
        {
            try
            {
                return HashMatches(root, "Packages/manifest.json", PackageManifestSha256) &&
                    HashMatches(root, "Packages/packages-lock.json", ResolvedPackageLockSha256);
            }
            catch
            {
                // Missing/unreadable/changed files and all path or I/O failures fail closed.
                return false;
            }
        }

        static bool ProtectedBoundaryIsExact(string root)
        {
            return
                HashMatches(root, "Packages/manifest.json", PackageManifestSha256) &&
                HashMatches(
                    root,
                    "ProjectSettings/ProjectVersion.txt",
                    ProjectVersionSha256
                ) &&
                HashMatches(
                    root,
                    "ProjectSettings/EditorBuildSettings.asset",
                    EditorBuildSettingsSha256
                ) &&
                HashMatches(
                    root,
                    "Assets/LegendsOfRus/Resources/Config/" +
                    "economy_runtime_activation.json",
                    EconomyActivationSha256
                ) &&
                HashMatches(
                    root,
                    "Assets/LegendsOfRus/Resources/Config/" +
                    "commercial_runtime_activation.json",
                    CommercialActivationSha256
                ) &&
                HashMatches(
                    root,
                    "Assets/LegendsOfRus/Resources/Config/" +
                    "commercial_runtime_config.json",
                    CommercialConfigSha256
                );
        }

        static bool NoForbiddenAndroidArtifacts(string root)
        {
            if (!NoEmbeddedPackages(root))
                return false;

            string assets = Path.Combine(root, "Assets");
            if (!Directory.Exists(assets))
                return false;
            string pluginsAndroid = Path.Combine(assets, "Plugins/Android");
            if (Directory.Exists(pluginsAndroid))
                return false;

            foreach (string directory in Directory.EnumerateDirectories(
                assets,
                "*",
                SearchOption.AllDirectories))
            {
                if (IsForbiddenAndroidLibraryDirectoryName(
                    Path.GetFileName(directory)))
                    return false;
            }

            foreach (string file in Directory.EnumerateFiles(
                assets,
                "*",
                SearchOption.AllDirectories))
            {
                if (IsForbiddenAndroidArtifactExtension(
                    Path.GetExtension(file)))
                    return false;
                if (IsForbiddenAndroidArtifactFileName(Path.GetFileName(file)))
                    return false;
            }
            return true;
        }

        internal static bool IsForbiddenAndroidLibraryDirectoryName(
            string directoryName)
        {
            return
                !string.IsNullOrEmpty(directoryName) &&
                directoryName.EndsWith(
                    ".androidlib",
                    StringComparison.OrdinalIgnoreCase
                );
        }

        internal static bool IsForbiddenAndroidArtifactFileName(
            string fileName)
        {
            return
                !string.IsNullOrEmpty(fileName) &&
                ForbiddenNames.Contains(fileName);
        }

        internal static bool IsForbiddenAndroidArtifactExtension(
            string extension)
        {
            return
                !string.IsNullOrEmpty(extension) &&
                ForbiddenExtensions.Contains(extension);
        }

        internal static bool NoEmbeddedPackages(string root)
        {
            try
            {
                string packages = Path.Combine(root, "Packages");
                return
                    Directory.Exists(packages) &&
                    !Directory.EnumerateDirectories(packages).Any();
            }
            catch
            {
                return false;
            }
        }

        static bool HashMatches(
            string root,
            string relativePath,
            string expectedSha256)
        {
            string path = Path.Combine(root, relativePath);
            if (!File.Exists(path))
                return false;
            using (SHA256 algorithm = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                string actual = BitConverter.ToString(
                    algorithm.ComputeHash(stream)
                ).Replace("-", string.Empty).ToLowerInvariant();
                return string.Equals(
                    actual,
                    expectedSha256,
                    StringComparison.Ordinal
                );
            }
        }

        static string ProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }
    }
}
#endif
