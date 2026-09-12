#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step20ProductionAssetWave03Step92RuntimeCapture
    {
        public const string OutputRoot = "ProjectDocs/V1_20_STEP09_2/UnityRuntimeCaptures";
        const string RequestedKey = "LegendsOfRus.Step92.Wave03.RuntimeCaptureRequested";
        const string HarnessName = "__LegendsOfRus_Step92_Wave03_RuntimeCaptureHarness";

        static Step20ProductionAssetWave03Step92RuntimeCapture()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update -= Poll;
            EditorApplication.update += Poll;
        }

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.2 Wave 3 Unity Runtime Four-Aspect Capture")]
        public static void StartCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before starting Step 9.2 runtime capture.");

            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);

            List<string> failures = Step20ProductionAssetWave03PrefabBuilder.ValidateAll();
            failures.AddRange(Step20ProductionAssetWave03Step92BoundaryGate.Validate(false));
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            string abs = AbsoluteOutputRoot();
            if (Directory.Exists(abs)) Directory.Delete(abs, true);
            Directory.CreateDirectory(abs);
            SessionState.SetBool(RequestedKey, true);
            Debug.Log("v1.20 Step 9.2: entering Play Mode for 32 Wave 3 candidate captures. No UI, LiveOps, leaderboard, social, mail, reward, or economy action will be invoked.");
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
                    Step92Wave03RuntimeCaptureHarness harness = go.AddComponent<Step92Wave03RuntimeCaptureHarness>();
                    harness.Begin(AbsoluteOutputRoot());
                }
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                string pass = Path.Combine(AbsoluteOutputRoot(), "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
                string fail = Path.Combine(AbsoluteOutputRoot(), "UNITY_RUNTIME_CAPTURE_FAILED.txt");
                SessionState.SetBool(RequestedKey, false);
                if (File.Exists(pass))
                {
                    Debug.Log("v1.20 Step 9.2 runtime capture evidence created. Run the Wave 3 Visual Acceptance Analyzer next.");
                    if (!Application.isBatchMode)
                        EditorUtility.DisplayDialog("Legends of Rus", "Step 9.2 Wave 3 runtime capture completed: 32/32. Run the Visual Acceptance Analyzer next.", "OK");
                }
                else if (File.Exists(fail))
                {
                    Debug.LogError("v1.20 Step 9.2 runtime capture failed. See " + fail);
                }
            }
        }

        static void Poll()
        {
            if (!SessionState.GetBool(RequestedKey, false) || !EditorApplication.isPlaying)
                return;

            string abs = AbsoluteOutputRoot();
            if (File.Exists(Path.Combine(abs, "UNITY_RUNTIME_CAPTURE_EXECUTED.json")) ||
                File.Exists(Path.Combine(abs, "UNITY_RUNTIME_CAPTURE_FAILED.txt")))
            {
                EditorApplication.ExitPlaymode();
            }
        }

        public static string AbsoluteOutputRoot()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), OutputRoot.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
