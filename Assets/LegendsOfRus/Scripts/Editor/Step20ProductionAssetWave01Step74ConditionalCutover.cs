#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Explicit, evidence-gated Step 7.4 development visual cutover. It never runs automatically.
    /// Only the eight directly referenced pages already present in the active registry are eligible.
    /// Pause/ChapterComplete remain on the accepted baseline until direct references exist;
    /// SplashTitle/Dialogue remain candidate-only until a later registry checkpoint.
    /// </summary>
    public static class Step20ProductionAssetWave01Step74ConditionalCutover
    {
        const string RuntimeCaptureManifest = "ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_CAPTURE_EXECUTED.json";
        const string RuntimeVisualPass = "ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_VISUAL_QA_PASS.lock";
        const string AndroidPass = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_ANDROID_QA_PASS.lock";
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_DECISION.json";
        const string AppliedPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_APPLIED.json";
        const string ActiveRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string BackupRoot = "ProjectDocs/V1_20_STEP07_4/CutoverBackup";

        static readonly ScreenId[] EligiblePages =
        {
            ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
            ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Victory, ScreenId.Fail
        };

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 7.4/Evaluate Conditional Wave 1 Cutover")]
        public static void Evaluate()
        {
            List<string> blockers = CollectBlockers();
            WriteDecision(blockers);
            if (blockers.Count == 0)
            {
                bool applied = File.Exists(AppliedPath);
                bool androidPass = File.Exists(AndroidPass);
                if (!applied)
                    Debug.Log("v1.20 Step 7.4 cutover gate: ELIGIBLE for the eight directly referenced active pages. Explicit Apply is still required.");
                else if (!androidPass)
                    Debug.Log("v1.20 Step 7.4 cutover is applied; post-cutover physical Android acceptance is still pending. Roll back if device QA fails.");
                else
                    Debug.Log("v1.20 Step 7.4 development cutover accepted: Unity runtime evidence + map QA + post-cutover Android QA are present.");

                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus",
                        !applied
                            ? "Step 7.4 gate PASS. Eight directly referenced active pages are eligible for DEVELOPMENT visual cutover. Use Apply only after reviewing the runtime captures."
                            : (!androidPass
                                ? "Step 7.4 cutover is active. Test it now on physical Android, then record PASS or use Roll Back if any regression appears."
                                : "Step 7.4 8-page development cutover is accepted. Full 12-page Wave 1 remains incomplete until deferred-reference pages are resolved."),
                        "OK");
            }
            else
            {
                Debug.LogWarning("v1.20 Step 7.4 cutover gate: NO-GO.\n" + string.Join(Environment.NewLine, blockers.ToArray()));
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 cutover remains NO-GO. See V1_20_STEP07_4_CUTOVER_DECISION.json for blockers.", "OK");
            }
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 7.4/Record Post-Cutover Physical Android QA PASS")]
        public static void RecordAndroidPass()
        {
            if (Application.isBatchMode)
                throw new InvalidOperationException("Physical Android signoff requires an explicit interactive editor action.");
            if (!File.Exists(AppliedPath))
                throw new InvalidOperationException("Apply the eligible Step 7.4 development cutover first, then test that active cutover on Android before recording PASS.");

            bool confirmed = EditorUtility.DisplayDialog(
                "Step 7.4 Android QA",
                "Confirm ONLY after testing the ACTIVE Step 7.4 cutover on a physical Android device: safe area, visible Back, system Back, touch targets, Cyrillic rendering, dynamic map state, and no gameplay/navigation regression. If anything fails, use Roll Back.",
                "Confirm Android QA PASS",
                "Cancel"
            );
            if (!confirmed) return;

            File.WriteAllText(AndroidPass,
                "v1.20-step07.4-android-qa-pass\n" +
                "confirmed-at-utc=" + DateTime.UtcNow.ToString("o") + "\n" +
                "checks=safe-area,touch,visible-back,system-back,cyrillic,map-state,functional-regression\n" +
                "confirmed-by=explicit-editor-user-action\n");
            AssetDatabase.ImportAsset(AndroidPass, ImportAssetOptions.ForceUpdate);
            Debug.Log("v1.20 Step 7.4 physical Android QA signoff recorded.");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 7.4/Apply Eligible 8-Page Development Cutover")]
        public static void ApplyEligibleCutover()
        {
            if (Application.isBatchMode)
                throw new InvalidOperationException("Step 7.4 development cutover requires an explicit interactive editor action.");
            if (File.Exists(AppliedPath))
                throw new InvalidOperationException("Step 7.4 development cutover is already applied. Run device QA or Roll Back; do not reapply over the rollback backup.");

            List<string> blockers = CollectBlockers();
            if (blockers.Count > 0)
            {
                WriteDecision(blockers);
                throw new InvalidOperationException("Step 7.4 cutover blocked:\n" + string.Join(Environment.NewLine, blockers.ToArray()));
            }

            if (!Application.isBatchMode)
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Apply Step 7.4 Development Cutover",
                    "This will replace the active visual prefabs for Loading, Home, WorldMap, ChapterMap, PreLevel, Gameplay, Victory and Fail. Function/navigation code is not changed. Baseline prefab YAML is backed up for rollback. Continue?",
                    "Apply 8-page cutover",
                    "Cancel"
                );
                if (!confirmed) return;
            }

            BackupActivePrefabs();
            foreach (ScreenId id in EligiblePages)
                ReplaceActivePrefab(id);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            File.WriteAllText(AppliedPath, BuildAppliedJson());
            AssetDatabase.ImportAsset(AppliedPath, ImportAssetOptions.ForceUpdate);
            WriteDecision(new List<string>());
            Debug.Log("v1.20 Step 7.4 conditional DEVELOPMENT cutover applied: 8 pages. Test the ACTIVE cutover on physical Android next; rollback remains available. Pause and ChapterComplete remain baseline; SplashTitle and Dialogue remain candidate-only.");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 7.4/Roll Back Eligible 8-Page Cutover")]
        public static void Rollback()
        {
            string absBackup = Absolute(BackupRoot);
            if (!Directory.Exists(absBackup))
                throw new InvalidOperationException("No Step 7.4 cutover backup exists.");

            foreach (ScreenId id in EligiblePages)
            {
                string backup = Path.Combine(absBackup, id + ".prefab");
                string active = Absolute(ActiveRoot + "/" + id + ".prefab");
                if (!File.Exists(backup))
                    throw new InvalidOperationException("Missing rollback prefab: " + backup);
                File.Copy(backup, active, true);
            }
            if (File.Exists(AppliedPath)) File.Delete(AppliedPath);
            if (File.Exists(AndroidPass)) File.Delete(AndroidPass);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            WriteDecision(CollectBlockers());
            Debug.Log("v1.20 Step 7.4 active visual prefab rollback complete for the eligible eight-page set. Android acceptance was cleared with the rollback.");
        }

        public static List<string> CollectBlockers()
        {
            var blockers = new List<string>();
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                blockers.Add("CANDIDATE-BUILD: Step 7.4 candidate prefabs have not been rebuilt/imported yet.");
            else
                blockers.AddRange(Step20ProductionAssetWave01PrefabBuilder.ValidateAll());

            if (!File.Exists(RuntimeCaptureManifest))
                blockers.Add("UNITY-RUNTIME-CAPTURE: 48 Unity Play-Mode captures have not been executed.");
            if (!File.Exists(RuntimeVisualPass))
                blockers.Add("UNITY-RUNTIME-FIDELITY: Runtime capture analyzer has not produced the 9/9 visual QA pass lock.");
            if (!File.Exists(Step20ProductionAssetWave01Step74MapStateQa.PassLock))
                blockers.Add("MAP-DYNAMIC-STATE: Dynamic World/Chapter map state QA has not passed.");
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                blockers.Add("FUNCTIONAL-FREEZE: active registry count drifted from the accepted 10-page baseline.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.SplashTitle) || ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Dialogue))
                blockers.Add("REGISTRY-SCOPE: SplashTitle/Dialogue must remain outside this conditional cutover checkpoint.");

            return blockers;
        }

        static void BackupActivePrefabs()
        {
            string abs = Absolute(BackupRoot);
            Directory.CreateDirectory(abs);
            foreach (ScreenId id in EligiblePages)
            {
                string source = Absolute(ActiveRoot + "/" + id + ".prefab");
                if (!File.Exists(source))
                    throw new InvalidOperationException("Missing active production prefab: " + source);
                File.Copy(source, Path.Combine(abs, id + ".prefab"), true);
            }
            File.WriteAllText(Path.Combine(abs, "README.txt"),
                "v1.20 Step 7.4 pre-cutover active prefab YAML backup.\n" +
                "Restore with the Unity menu Roll Back Eligible 8-Page Cutover.\n" +
                "The active .meta GUIDs are intentionally not replaced.\n");
        }

        static void ReplaceActivePrefab(ScreenId id)
        {
            string candidatePath = Step20ProductionAssetWave01PrefabBuilder.CandidateRoot + "/" + id + ".prefab";
            string activePath = ActiveRoot + "/" + id + ".prefab";
            GameObject candidate = AssetDatabase.LoadAssetAtPath<GameObject>(candidatePath);
            if (candidate == null)
                throw new InvalidOperationException("Missing cutover candidate: " + candidatePath);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(candidate);
            try
            {
                instance.name = id.ToString();
                PrefabUtility.SaveAsPrefabAsset(instance, activePath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        static void WriteDecision(List<string> blockers)
        {
            bool applied = File.Exists(AppliedPath);
            bool androidPass = File.Exists(AndroidPass);
            string decision;
            if (blockers.Count > 0)
                decision = "NO_GO";
            else if (!applied)
                decision = "ELIGIBLE_8_PAGE_DEVELOPMENT_CUTOVER";
            else if (!androidPass)
                decision = "CUTOVER_APPLIED_ANDROID_ACCEPTANCE_PENDING";
            else
                decision = "ACCEPTED_8_PAGE_DEVELOPMENT_CUTOVER";

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step07.4-runtime-capture-dynamic-map-conditional-cutover\",");
            sb.AppendLine("  \"decision\": \"" + decision + "\",");
            sb.AppendLine("  \"automaticCutover\": false,");
            sb.AppendLine("  \"developmentCutoverApplied\": " + (applied ? "true" : "false") + ",");
            sb.AppendLine("  \"postCutoverAndroidAccepted\": " + (androidPass ? "true" : "false") + ",");
            sb.AppendLine("  \"eligiblePages\": [\"Loading\", \"Home\", \"WorldMap\", \"ChapterMap\", \"PreLevel\", \"Gameplay\", \"Victory\", \"Fail\"],");
            sb.AppendLine("  \"deferredPages\": {\"missingDirectReference\": [\"SplashTitle\", \"Pause\", \"ChapterComplete\"], \"candidateOnlyRegistryScope\": [\"SplashTitle\", \"Dialogue\"]},");
            sb.AppendLine("  \"releaseFontFaceSignoffStillRequired\": true,");
            sb.AppendLine("  \"postCutoverAndroidAcceptanceRequired\": true,");
            sb.AppendLine("  \"blockers\": [");
            for (int i = 0; i < blockers.Count; i++)
            {
                sb.Append("    \"").Append(JsonEscape(blockers[i])).Append("\"");
                if (i + 1 < blockers.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(DecisionPath, sb.ToString());
            AssetDatabase.ImportAsset(DecisionPath, ImportAssetOptions.ForceUpdate);
        }

        static string BuildAppliedJson()
        {
            return "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step07.4-conditional-development-cutover\",\n" +
                "  \"appliedAtUtc\": \"" + DateTime.UtcNow.ToString("o") + "\",\n" +
                "  \"activeRegistryChanged\": false,\n" +
                "  \"visualPrefabsReplaced\": 8,\n" +
                "  \"pages\": [\"Loading\", \"Home\", \"WorldMap\", \"ChapterMap\", \"PreLevel\", \"Gameplay\", \"Victory\", \"Fail\"],\n" +
                "  \"deferred\": [\"SplashTitle\", \"Pause\", \"ChapterComplete\", \"Dialogue\"],\n" +
                "  \"rollbackAvailable\": true,\n" +
                "  \"releaseFontFaceSignoffStillRequired\": true\n" +
                "}\n";
        }

        static string Absolute(string projectRelative)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), projectRelative.Replace('/', Path.DirectorySeparatorChar));
        }

        static string JsonEscape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
