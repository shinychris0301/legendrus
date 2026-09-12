#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    internal static class AndroidBuildMenus
    {
        const string Checklist =
            "This local Development APK contract requires the exact reviewed " +
            "manifest and resolved-package lock fingerprints.\n" +
            "Run Build Fix04 > Export Build Diagnostics (Read Only); continue " +
            "only when every readiness check passes. Missing, changed or unreadable " +
            "package inputs remain blocked.\n\n" +
            "Required configuration in Unity 6000.5.9f1:\n" +
            "• Android must be the active platform.\n" +
            "• Use the platform profile; no custom Build Profile may exist.\n" +
            "• Scene: Assets/Scenes/Bootstrap.unity only.\n" +
            "• Package: com.example.legendsofrus.v117debug.\n" +
            "• Version: 0.0.0, code 1.\n" +
            "• Target API 36; minimum API 26–36.\n" +
            "• IL2CPP with ARM64, category game; the obsolete Is Game " +
            "compatibility alias mirrors that category; Portrait.\n" +
            "• Development Build ON; normal Build may add Show Built Player.\n" +
            "• Default, LZ4 or LZ4HC compression is permitted; StrictMode is permitted.\n" +
            "• Build Fix04 provides a dedicated Development build-only command.\n" +
            "• Script Debugging, profiler/deep profiling, code coverage, " +
            "test assemblies, patch, and scripts-only options OFF.\n" +
            "• Use Build, not Build & Run; no AssetBundle manifest.\n" +
            "• App Bundle and Gradle export OFF.\n" +
            "• Split APK per CPU and Split Application Binary OFF.\n" +
            "• Native Debug Symbols: None (no symbols package).\n" +
            "• Custom Keystore OFF; Android scripting defines empty.\n" +
            "• Output: Builds/Verification/v1_17_step01/" +
            "LegendsOfRus-v1.17-step01-development.apk.\n\n" +
            "Release APK, AAB, custom signing, provider activation, and " +
            "production publishing remain blocked.";

        [MenuItem("Legends of Rus/Android/v1.17 Step 1/Validate Development APK Readiness")]
        internal static void ValidateDevelopmentApkReadiness()
        {
            AndroidBuildPreflightResult result =
                AndroidBuildGuard.EvaluateDevelopmentPreflight();
            AndroidBuildEvidence evidence = AndroidBuildEvidence.From(result);
            if (result != null && result.Decision != null &&
                result.Decision.Allowed)
            {
                Debug.Log(
                    "v1.17 Step 1 Android preflight passed.\n" +
                    evidence.ToSanitizedJson()
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Android preflight",
                        "Development APK readiness passed. Production and " +
                        "custom signing remain blocked.",
                        "OK"
                    );
                }
                return;
            }

            string reason = result != null && result.Decision != null
                ? result.Decision.ReasonCode
                : AndroidBuildReasonCodes.UnexpectedFailure;
            Debug.LogWarning(
                "v1.17 Step 1 Android preflight blocked: " + reason +
                ".\n" + evidence.ToSanitizedJson()
            );
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Android preflight blocked",
                    "Reason: " + reason + "\n\n" + Checklist,
                    "OK"
                );
            }
        }

        [MenuItem("Legends of Rus/Android/v1.17 Step 1/Show Manual Configuration Checklist")]
        internal static void ShowManualConfigurationChecklist()
        {
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Android configuration",
                    Checklist,
                    "OK"
                );
            }
            Debug.Log(
                "v1.17 Step 1 Android configuration checklist opened. " +
                "No setting was changed."
            );
        }

        [MenuItem("Legends of Rus/Android/v1.17 Step 1/Open Android Player Settings")]
        internal static void OpenAndroidPlayerSettings()
        {
            SettingsService.OpenProjectSettings("Project/Player");
        }

        [MenuItem("Legends of Rus/Android/v1.17 Step 1/Open Build Profiles")]
        internal static void OpenBuildProfiles()
        {
            if (!EditorApplication.ExecuteMenuItem("File/Build Profiles"))
            {
                Debug.LogWarning(
                    "Could not open Build Profiles automatically. Use " +
                    "File > Build Profiles. No setting was changed."
                );
            }
        }
    }
}
#endif
