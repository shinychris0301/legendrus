#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step20ProductionAssetStep112ARuntimeCloseout
    {
        public const string OutputRoot = "ProjectDocs/V1_20_STEP11_2A/RuntimeCloseout";
        public const string ManifestName = "PAUSE_CHAPTER_COMPLETE_RUNTIME_CAPTURE_EXECUTED.json";
        public const string FailureName = "PAUSE_CHAPTER_COMPLETE_RUNTIME_CAPTURE_FAILED.txt";
        const string RequestedKey = "LegendsOfRus.Step112A.PauseChapterComplete.RuntimeCaptureRequested";
        const string HarnessName = "__LegendsOfRus_Step112A_PauseChapterComplete_RuntimeCaptureHarness";

        static Step20ProductionAssetStep112ARuntimeCloseout()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update -= Poll;
            EditorApplication.update += Poll;
        }

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 11.2A Pause + Chapter Complete Focused Runtime Capture")]
        public static void StartCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before starting Step 11.2A focused runtime capture.");

            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);
            List<string> failures = Step20ProductionAssetWave01PrefabBuilder.ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            string pauseRef = Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(ScreenId.Pause));
            string completeRef = Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(ScreenId.ChapterComplete));
            if (!File.Exists(pauseRef) || !File.Exists(completeRef))
                throw new InvalidOperationException("Step 11.2A approved Pause and ChapterComplete references must exist before focused runtime capture.");

            string abs = AbsoluteOutputRoot();
            if (Directory.Exists(abs)) Directory.Delete(abs, true);
            Directory.CreateDirectory(abs);
            SessionState.SetBool(RequestedKey, true);
            Debug.Log("v1.20 Step 11.2A: entering Play Mode for 8 focused runtime captures (Pause + ChapterComplete × 4 aspects). Production routing remains unchanged.");
            EditorApplication.EnterPlaymode();
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(RequestedKey, false)) return;

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                GameObject existing = GameObject.Find(HarnessName);
                if (existing == null)
                {
                    GameObject go = new GameObject(HarnessName);
                    Step112APauseChapterCompleteRuntimeCaptureHarness harness = go.AddComponent<Step112APauseChapterCompleteRuntimeCaptureHarness>();
                    harness.Begin(AbsoluteOutputRoot());
                }
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                SessionState.SetBool(RequestedKey, false);
                string pass = Path.Combine(AbsoluteOutputRoot(), ManifestName);
                string fail = Path.Combine(AbsoluteOutputRoot(), FailureName);
                if (File.Exists(pass))
                {
                    Debug.Log("v1.20 Step 11.2A focused runtime capture evidence created: 8/8. Run the focused runtime fidelity analyzer next.");
                    if (!Application.isBatchMode)
                        EditorUtility.DisplayDialog("Legends of Rus", "Step 11.2A focused runtime capture completed: 8/8. Run the Pause + Chapter Complete Runtime Fidelity Analyzer next.", "OK");
                }
                else if (File.Exists(fail))
                {
                    Debug.LogError("v1.20 Step 11.2A focused runtime capture failed. See " + fail);
                }
            }
        }

        static void Poll()
        {
            if (!SessionState.GetBool(RequestedKey, false) || !EditorApplication.isPlaying) return;
            string abs = AbsoluteOutputRoot();
            if (File.Exists(Path.Combine(abs, ManifestName)) || File.Exists(Path.Combine(abs, FailureName)))
                EditorApplication.ExitPlaymode();
        }

        public static string AbsoluteOutputRoot()
        {
            return Step20ProductionAssetStep103Evidence.Absolute(OutputRoot);
        }

        public static bool HasFocusedEvidence(ScreenId id)
        {
            if (id != ScreenId.Pause && id != ScreenId.ChapterComplete) return false;
            string root = AbsoluteOutputRoot();
            string manifest = Path.Combine(root, ManifestName);
            if (!File.Exists(manifest)) return false;
            string text = File.ReadAllText(manifest);
            if (text.IndexOf("\"captures\": 8", StringComparison.Ordinal) < 0) return false;
            string[] suffixes = { "941x1672", "1080x2340", "1080x2400", "1600x2560" };
            for (int i = 0; i < suffixes.Length; i++)
                if (!File.Exists(Path.Combine(root, id + "_" + suffixes[i] + ".png"))) return false;
            return true;
        }
    }
}
#endif
