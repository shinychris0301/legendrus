#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step20ProductionAssetWave02Step84RuntimeCapture
    {
        public const string OutputRoot = "ProjectDocs/V1_20_STEP08_4/UnityRuntimeCaptures";
        const string RequestedKey = "LegendsOfRus.Step84.Wave02.RuntimeCaptureRequested";
        const string HarnessName = "__LegendsOfRus_Step84_Wave02_RuntimeCaptureHarness";

        static Step20ProductionAssetWave02Step84RuntimeCapture()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update -= Poll;
            EditorApplication.update += Poll;
        }

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.4 Wave 2 Unity Runtime Four-Aspect Capture")]
        public static void StartCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before starting Step 8.4 runtime capture.");

            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            string abs = AbsoluteOutputRoot();
            if (Directory.Exists(abs)) Directory.Delete(abs, true);
            Directory.CreateDirectory(abs);
            SessionState.SetBool(RequestedKey, true);
            Debug.Log("v1.20 Step 8.4: entering Play Mode for 36 refined Wave 2 candidate captures. No UI/provider/economy action will be invoked.");
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
                    Step84Wave02RuntimeCaptureHarness harness = go.AddComponent<Step84Wave02RuntimeCaptureHarness>();
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
                    Debug.Log("v1.20 Step 8.4 runtime capture evidence created. Run the Approved Shop Runtime Gate next.");
                    if (!Application.isBatchMode)
                        EditorUtility.DisplayDialog("Legends of Rus", "Step 8.4 Wave 2 runtime capture completed: 36/36. Run the Approved Shop Runtime Gate next.", "OK");
                }
                else if (File.Exists(fail))
                {
                    Debug.LogError("v1.20 Step 8.4 runtime capture failed. See " + fail);
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
