#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step20ProductionAssetWave03Step94RuntimeCapture
    {
        public const string OutputRoot = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence";
        const string RequestedKey = "LegendsOfRus.Step94.Wave03.RuntimeCaptureRequested";
        const string HarnessName = "__LegendsOfRus_Step94_Wave03_RuntimeCaptureHarness";

        static Step20ProductionAssetWave03Step94RuntimeCapture()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update -= Poll;
            EditorApplication.update += Poll;
        }

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.4 Wave 3 Refined Unity Runtime Four-Aspect Capture")]
        public static void StartCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before starting Step 9.4 runtime capture.");

            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);

            List<string> failures = Step20ProductionAssetWave03PrefabBuilder.ValidateAll();
            failures.AddRange(Step20ProductionAssetWave03Step94BoundaryGate.Validate(false));
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            string abs = AbsoluteOutputRoot();
            PreserveReadmeThenClearEvidence(abs);
            Directory.CreateDirectory(abs);
            SessionState.SetBool(RequestedKey, true);
            Debug.Log("v1.20 Step 9.4: entering Play Mode for 32 refined Wave 3 runtime captures. No LiveOps, leaderboard, social, mail, reward, provider, or economy action will be invoked.");
            EditorApplication.EnterPlaymode();
        }

        static void PreserveReadmeThenClearEvidence(string abs)
        {
            if (!Directory.Exists(abs)) return;
            foreach (string file in Directory.GetFiles(abs))
            {
                if (string.Equals(Path.GetFileName(file), "README.md", StringComparison.OrdinalIgnoreCase)) continue;
                File.Delete(file);
            }
            foreach (string dir in Directory.GetDirectories(abs))
                Directory.Delete(dir, true);
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(RequestedKey, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (GameObject.Find(HarnessName) == null)
                {
                    GameObject go = new GameObject(HarnessName);
                    Step94Wave03RuntimeCaptureHarness harness = go.AddComponent<Step94Wave03RuntimeCaptureHarness>();
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
                    Debug.Log("v1.20 Step 9.4 runtime capture evidence created: 32/32. Next: intake approved reference authority, analyze runtime fidelity, then evaluate conditional acceptance readiness.");
                    if (!Application.isBatchMode)
                        EditorUtility.DisplayDialog("Legends of Rus", "Step 9.4 refined Wave 3 runtime capture completed: 32/32. Next run Reference-Authority Intake and Runtime Visual Acceptance.", "OK");
                }
                else if (File.Exists(fail))
                    Debug.LogError("v1.20 Step 9.4 runtime capture failed. See " + fail);
            }
        }

        static void Poll()
        {
            if (!SessionState.GetBool(RequestedKey, false) || !EditorApplication.isPlaying) return;
            string abs = AbsoluteOutputRoot();
            if (File.Exists(Path.Combine(abs, "UNITY_RUNTIME_CAPTURE_EXECUTED.json")) ||
                File.Exists(Path.Combine(abs, "UNITY_RUNTIME_CAPTURE_FAILED.txt")))
                EditorApplication.ExitPlaymode();
        }

        public static string AbsoluteOutputRoot()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), OutputRoot.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
