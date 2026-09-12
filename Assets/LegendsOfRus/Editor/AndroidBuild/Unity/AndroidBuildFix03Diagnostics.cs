#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    [Serializable]
    internal sealed class AndroidBuildFix03FileEvidence
    {
        public string relativePath;
        public bool present;
        public long byteLength;
        public string sha256;
        public string expectedSha256;
        public bool matchesExpectedPin;
        public string status;
    }

    [Serializable]
    internal sealed class AndroidBuildFix03PackageEvidence
    {
        public string name;
        public string version;
        public string source;
    }

    [Serializable]
    internal sealed class AndroidBuildFix03Report
    {
        public int schemaVersion = 2;
        public string revision = "R3-BuildOptionsFix04";
        public string unityVersion;
        public string activeTarget;
        public string evidenceScope = "Read-only local preflight; NOT an APK or device test";
        public bool editorPlaying;
        public bool editorPlayingOrEntering;
        public bool editorCompiling;
        public bool editorUpdating;
        public bool uiDevelopment;
        public bool uiScriptDebugging;
        public bool uiAutoconnectProfiler;
        public bool uiScriptsOnly;
        public AndroidBuildEvidence policyEvidence;
        public AndroidBuildFix03FileEvidence resolvedPackageLock;
        public AndroidBuildFix03FileEvidence[] protectedFiles;
        public AndroidBuildFix03PackageEvidence[] registeredUnityPackages;
        public string[] observations;
        // Enum definitions are read from this Editor, not guessed from a formatted error string.
        public bool lastScheduledOptionsPresent;
        public string lastScheduledOptions;
        public AndroidBuildOptionEnumEvidence[] installedBuildOptions;
        public string permittedDevelopmentOptions;
    }

    /// <summary>Read-only diagnostics; never approves a lock or mutates Unity settings.</summary>
    internal static class AndroidBuildFix03Diagnostics
    {
        internal const string ReportPath =
            "ProjectDocs/AndroidBuildFix03/AndroidBuildDiagnostics.json";
        const string MenuRoot = "Legends of Rus/Android/Build Fix03/";
        const long MaximumFingerprintBytes = 4 * 1024 * 1024;

        internal static string ExplainReason(string reason)
        {
            switch (reason)
            {
                case AndroidBuildReasonCodes.EditorPlayModeActive:
                    return "Stop Play Mode (Pause is not Stop), wait for Edit Mode, then retry. " +
                        "No content build has been requested by this guard.";
                case AndroidBuildReasonCodes.DevelopmentBuildRequired:
                    return "Development=False in the scheduled request. Enable Development Build, " +
                        "or use Android > Build Fix04 > Build Development APK (No Run). " +
                        "This guard does not silently convert a release request.";
                case AndroidBuildReasonCodes.BuildOptionsUnsafe:
                    return "Development is required. ShowBuiltPlayer, LZ4/LZ4HC compression and " +
                        "StrictMode are permitted. AutoRunPlayer and unreviewed additional flags " +
                        "remain outside this build-only workflow. Use Build, not Build And Run.";
                case AndroidBuildReasonCodes.PackageLockUnapproved:
                    return "The inherited v1.17 resolved-package approval is still closed. " +
                        "A generated packages-lock.json alone does not open it. Run Android > " +
                        "Build Fix03 > Export Build Diagnostics; review the resolved packages and stale pins.";
                case AndroidBuildReasonCodes.CommercialBoundaryChanged:
                    return "One or more protected-file fingerprints or artifact checks differ from " +
                        "the inherited policy. Export Build Diagnostics; do not update hashes blindly.";
                case AndroidBuildReasonCodes.OutputPathInvalid:
                    return "The existing contract requires the exact development output named in " +
                        "android_build_foundation_policy.json. The destination has not been changed.";
                case AndroidBuildReasonCodes.ProjectSettingsMissing:
                    return "ProjectSettings/ProjectSettings.asset is unavailable to the policy reader.";
                case AndroidBuildReasonCodes.BuildProfileOverrideBlocked:
                    return "The inherited policy disallows custom Build Profile assets; inspect the " +
                        "existing platform-profile requirement before changing the build workflow.";
                default:
                    return "Run Legends of Rus > Android > Build Fix03 > Export Build Diagnostics " +
                        "to inspect the current policy checks.";
            }
        }

        internal static string DescribeOptions(BuildOptions options)
        {
            return AndroidDevelopmentBuildOptions.Describe(options);
        }

        [MenuItem(MenuRoot + "Export Build Diagnostics")]
        internal static void Export()
        {
            if (!CanInspect())
            {
                Debug.LogWarning("Build Fix03: stop Play Mode and wait for compilation/import/build activity " +
                    "to finish before exporting diagnostics. No setting was changed.");
                return;
            }
            try
            {
                string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                AndroidBuildPreflightResult preflight = AndroidBuildGuard.EvaluateDevelopmentPreflight();
                AndroidBuildEvidence policy = AndroidBuildEvidence.From(preflight);
                var observations = new List<string>();
                if (!policy.packageLockApproved)
                    observations.Add(ExplainReason(AndroidBuildReasonCodes.PackageLockUnapproved));
                if (!policy.commercialBoundaryIntact)
                    observations.Add(ExplainReason(AndroidBuildReasonCodes.CommercialBoundaryChanged));
                if (!policy.projectSettingsPresent)
                    observations.Add(ExplainReason(AndroidBuildReasonCodes.ProjectSettingsMissing));
                observations.Add("Build And Run is outside the existing allowed option set. " +
                    "This export does not reconstruct the previous failed build's flags.");
                observations.Add("No Addressables content build, Player build, font approval, " +
                    "package approval, signing operation, or settings change was performed.");

                var report = new AndroidBuildFix03Report
                {
                    unityVersion = Application.unityVersion,
                    activeTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
                    editorPlaying = EditorApplication.isPlaying,
                    editorPlayingOrEntering = EditorApplication.isPlayingOrWillChangePlaymode,
                    editorCompiling = EditorApplication.isCompiling,
                    editorUpdating = EditorApplication.isUpdating,
                    uiDevelopment = EditorUserBuildSettings.development,
                    uiScriptDebugging = EditorUserBuildSettings.allowDebugging,
                    uiAutoconnectProfiler = EditorUserBuildSettings.connectProfiler,
                    uiScriptsOnly = EditorUserBuildSettings.buildScriptsOnly,
                    policyEvidence = policy,
                    resolvedPackageLock = Fingerprint(root, "Packages/packages-lock.json", string.Empty),
                    protectedFiles = AndroidBuildStateReader.ReadProtectedFileEvidence(),
                    registeredUnityPackages = ReadPackages(),
                    lastScheduledOptionsPresent = AndroidDevelopmentBuildOptions.HasScheduledSnapshot,
                    lastScheduledOptions = AndroidDevelopmentBuildOptions.ScheduledSnapshot(),
                    installedBuildOptions = AndroidDevelopmentBuildOptions.ReadEnumEvidence(),
                    permittedDevelopmentOptions = AndroidDevelopmentBuildOptions.FormatActive(
                        AndroidDevelopmentBuildOptions.Allowed),
                    observations = observations.ToArray()
                };
                string destination = Path.Combine(root, ReportPath);
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.WriteAllText(destination, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
                Debug.Log("Build Fix03 diagnostics exported: " + ReportPath +
                    "\nCurrent preflight: " + policy.status + " / " + policy.reasonCode +
                    "\nNo credentials or local account paths are included; no settings were changed.");
            }
            catch (Exception exception)
            {
                // Exception messages may include local paths. Record only the exception type.
                Debug.LogError("Build Fix03 diagnostic export failed: " + exception.GetType().Name +
                    ". No build or configuration changes were attempted.");
            }
        }

        [MenuItem(MenuRoot + "Export Build Diagnostics", true)]
        static bool ValidateExport() { return CanInspect(); }

        static bool CanInspect()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode &&
                !EditorApplication.isCompiling && !EditorApplication.isUpdating && !BuildPipeline.isBuildingPlayer;
        }

        static AndroidBuildFix03PackageEvidence[] ReadPackages()
        {
            var result = new List<AndroidBuildFix03PackageEvidence>();
            // Fully qualified to avoid UnityEditor.PackageInfo ambiguity.
            var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            if (packages != null)
                foreach (var package in packages)
                    if (package != null && package.name != null &&
                        package.name.StartsWith("com.unity.", StringComparison.Ordinal))
                        result.Add(new AndroidBuildFix03PackageEvidence
                        {
                            name = package.name,
                            version = SafeVersion(package.version),
                            source = package.source.ToString()
                        });
            result.Sort((left, right) => string.CompareOrdinal(left.name, right.name));
            return result.ToArray();
        }

        static string SafeVersion(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 100)
                return "unavailable";
            foreach (char c in value)
                if (!(char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '+'))
                    return "non-semver-value-omitted";
            return value;
        }

        internal static AndroidBuildFix03FileEvidence Fingerprint(
            string root, string relativePath, string expectedHash)
        {
            var evidence = new AndroidBuildFix03FileEvidence
            {
                relativePath = relativePath, expectedSha256 = expectedHash,
                sha256 = string.Empty, status = "unavailable"
            };
            try
            {
                string path = Path.Combine(root, relativePath);
                evidence.present = File.Exists(path);
                if (!evidence.present) { evidence.status = "missing"; return evidence; }
                evidence.byteLength = new FileInfo(path).Length;
                if (evidence.byteLength > MaximumFingerprintBytes)
                { evidence.status = "too-large-for-bounded-diagnostic"; return evidence; }
                using (var stream = File.OpenRead(path))
                using (SHA256 algorithm = SHA256.Create())
                    evidence.sha256 = BitConverter.ToString(algorithm.ComputeHash(stream))
                        .Replace("-", string.Empty).ToLowerInvariant();
                evidence.matchesExpectedPin = !string.IsNullOrEmpty(expectedHash) &&
                    string.Equals(evidence.sha256, expectedHash, StringComparison.Ordinal);
                evidence.status = string.IsNullOrEmpty(expectedHash) ? "observed-not-approved" :
                    (evidence.matchesExpectedPin ? "pin-matches" : "pin-mismatch");
            }
            catch { evidence.status = "read-failed"; }
            return evidence;
        }
    }
}
#endif
