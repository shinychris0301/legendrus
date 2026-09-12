#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    /// <summary>
    /// Explicit development-only build entry. Never auto-runs, edits signing/package approvals,
    /// switches platforms, installs a menu handler, or queues a build after a domain reload.
    /// </summary>
    public static class AndroidDevelopmentBuildCommand
    {
        private const string MenuRoot = "Legends of Rus/Android/Build Fix04/";
        private static bool running;

        [MenuItem(MenuRoot + "Prepare Development Options and Export Diagnostics")]
        public static void PrepareDevelopmentOptions()
        {
            if (!CanStart())
            {
                Debug.LogWarning("Build Fix04: stop Play Mode and wait for compilation/import/build work to finish.");
                return;
            }
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogWarning("Build Fix04: activate the Android platform first. No setting was changed.");
                return;
            }
            if (!AndroidBuildStateReader.CustomBuildProfileAbsent())
            {
                Debug.LogWarning(AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.BuildProfileOverrideBlocked).Message);
                AndroidBuildFix03Diagnostics.Export();
                return;
            }

            // These are the only persistent setting changes made by this explicit preparation command.
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildScriptsOnly = false;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
            Debug.Log("Build Fix04: Development Build ON; script debugging, autoconnect profiler, " +
                "scripts-only and deep profiling OFF. Player identity, signing, API levels, " +
                "package approvals, scenes and compression settings are unchanged. No build was started.");
            AndroidBuildFix03Diagnostics.Export();
        }

        [MenuItem(MenuRoot + "Export Build Diagnostics (Read Only)")]
        public static void ExportDiagnostics() { AndroidBuildFix03Diagnostics.Export(); }

        [MenuItem(MenuRoot + "Build Development APK (No Run)")]
        public static void BuildDevelopmentApk()
        {
            if (!CanStart())
                throw AndroidBuildGuard.Denied(EditorApplication.isPlaying ||
                    EditorApplication.isPlayingOrWillChangePlaymode
                    ? AndroidBuildReasonCodes.EditorPlayModeActive
                    : AndroidBuildReasonCodes.SettingsReadFailed);

            running = true;
            try
            {
                AndroidBuildPolicyLoadResult load = AndroidBuildPolicyLoader.Load();
                if (load == null || !load.FilePresent)
                    throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.PolicyMissing);
                if (!load.HashMatches)
                    throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.PolicyHashMismatch);
                if (!load.IsValid || load.Policy == null)
                    throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.PolicyInvalid);

                BuildPlayerOptions options = CreateRequest(load.Policy);
                // Exact intent is used even when the separate Build Profiles UI still shows Release.
                CheckRequest(options, load.Policy);
                RequireReadiness(AndroidBuildGuard.EvaluateScheduledPreflight(options));

                if (!Application.isBatchMode)
                {
                    if (!EditorUtility.DisplayDialog("Legends of Rus - Development APK",
                        "Build the existing Bootstrap scene as a local Development APK?\n\n" +
                        "This uses LZ4 and StrictMode. It does not install or launch an app, " +
                        "approve package locks, or change signing. The existing policy's output " +
                        "file will be replaced if a previous build exists.", "Build APK", "Cancel"))
                        return;
                    if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                }

                // Saving can run editor callbacks; re-check before creating directories or starting a build.
                AndroidBuildEarlyPreflight.ValidateEditorState(EditorApplication.isPlaying,
                    EditorApplication.isPlayingOrWillChangePlaymode);
                if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
                    throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.SettingsReadFailed);
                load = AndroidBuildPolicyLoader.Load();
                if (load == null || !load.HashMatches || !load.IsValid || load.Policy == null)
                    throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.PolicyInvalid);
                CheckRequest(options, load.Policy);
                RequireReadiness(AndroidBuildGuard.EvaluateScheduledPreflight(options));

                string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string output = Path.GetFullPath(Path.Combine(root, options.locationPathName));
                Directory.CreateDirectory(Path.GetDirectoryName(output));

                // Align UI-facing callbacks only for this build; restore the user's prior toggles afterward.
                bool oldDevelopment = EditorUserBuildSettings.development;
                bool oldDebugging = EditorUserBuildSettings.allowDebugging;
                bool oldProfiler = EditorUserBuildSettings.connectProfiler;
                bool oldScriptsOnly = EditorUserBuildSettings.buildScriptsOnly;
                bool oldDeep = EditorUserBuildSettings.buildWithDeepProfilingSupport;
                BuildReport report;
                try
                {
                    EditorUserBuildSettings.development = true;
                    EditorUserBuildSettings.allowDebugging = false;
                    EditorUserBuildSettings.connectProfiler = false;
                    EditorUserBuildSettings.buildScriptsOnly = false;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                    Debug.Log("Build Fix04 starting Development APK build. " +
                        AndroidDevelopmentBuildOptions.Describe(options.options));
                    // Regular pipeline: Addressables and every existing Player build guard still execute.
                    report = BuildPipeline.BuildPlayer(options);
                }
                finally
                {
                    EditorUserBuildSettings.development = oldDevelopment;
                    EditorUserBuildSettings.allowDebugging = oldDebugging;
                    EditorUserBuildSettings.connectProfiler = oldProfiler;
                    EditorUserBuildSettings.buildScriptsOnly = oldScriptsOnly;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = oldDeep;
                }

                if (report == null || report.summary.result != BuildResult.Succeeded)
                    throw new BuildFailedException("Build Fix04: Unity did not report a successful APK build. " +
                        "Read the first build error in the Console; no app was installed or launched by this command.");
                if (!File.Exists(output))
                    throw new BuildFailedException("Build Fix04: expected APK is absent despite a success report. " +
                        "No app was installed or launched by this command.");
                Debug.Log("Build Fix04 APK created at the existing policy output: " +
                    load.Policy.developmentOutputRelativePath +
                    ". Not installed, not device-tested, not production-approved.");
            }
            catch (BuildFailedException)
            {
                // Includes all policy booleans and pins, not just the first rejected option.
                AndroidBuildFix03Diagnostics.Export();
                throw;
            }
            catch (Exception exception)
            {
                AndroidBuildFix03Diagnostics.Export();
                throw new BuildFailedException("Build Fix04 failed (" + exception.GetType().Name +
                    "). Consult Unity's build errors. No signing credentials were read by this command.");
            }
            finally { running = false; }
        }

        internal static BuildPlayerOptions CreateRequest(AndroidBuildFoundationPolicy policy)
        {
            if (policy == null) throw new ArgumentNullException("policy");
            return new BuildPlayerOptions
            {
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                subtarget = 0,
                scenes = new[] { policy.requiredScene },
                locationPathName = policy.developmentOutputRelativePath,
                options = AndroidDevelopmentBuildOptions.BuildOnlyDefaults,
                extraScriptingDefines = new string[0],
                assetBundleManifestPath = string.Empty
            };
        }

        private static void CheckRequest(BuildPlayerOptions options, AndroidBuildFoundationPolicy policy)
        {
            AndroidBuildEarlyPreflight.ValidateEditorState(EditorApplication.isPlaying,
                EditorApplication.isPlayingOrWillChangePlaymode);
            AndroidBuildPlayerOptionsGuard.ValidateOptions(options, policy);
            if (!AndroidBuildStateReader.CustomBuildProfileAbsent())
                throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.BuildProfileOverrideBlocked);
        }

        private static void RequireReadiness(AndroidBuildPreflightResult result)
        {
            if (result == null || result.Decision == null || !result.Decision.Allowed)
                throw AndroidBuildGuard.Denied(result != null && result.Decision != null
                    ? result.Decision.ReasonCode : AndroidBuildReasonCodes.UnexpectedFailure);
        }

        [MenuItem(MenuRoot + "Prepare Development Options and Export Diagnostics", true)]
        private static bool ValidatePrepareMenu() { return CanStart(); }

        [MenuItem(MenuRoot + "Build Development APK (No Run)", true)]
        private static bool ValidateBuildMenu() { return CanStart(); }

        private static bool CanStart()
        {
            return !running && !EditorApplication.isPlaying &&
                !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                !EditorApplication.isUpdating && !BuildPipeline.isBuildingPlayer;
        }
    }
}
#endif
