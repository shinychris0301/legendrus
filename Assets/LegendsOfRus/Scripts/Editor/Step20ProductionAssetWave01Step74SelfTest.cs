#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave01Step74SelfTest
    {
        const string DynamicManifest = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_DYNAMIC_MAP_DECOMPOSITION.json";
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_DECISION.json";
        const string AppliedPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_APPLIED.json";
        const string AndroidPass = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_ANDROID_QA_PASS.lock";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7.4 Source Readiness Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave01PrefabBuilder.ValidateAll());

            if (!File.Exists(DynamicManifest))
                failures.Add("Missing Step 7.4 dynamic-map decomposition manifest.");
            if (!File.Exists(DecisionPath))
                failures.Add("Missing packaged Step 7.4 cutover decision.");
            else if (!File.ReadAllText(DecisionPath).Contains("\"decision\": \"NO_GO\""))
                failures.Add("Packaged Step 7.4 cutover decision must remain NO_GO before Unity/Android evidence exists.");
            if (File.Exists(AppliedPath))
                failures.Add("Packaged Step 7.4 source must not contain a pre-applied runtime cutover.");
            if (File.Exists(AndroidPass))
                failures.Add("Packaged Step 7.4 source must not contain a pre-recorded physical Android PASS.");

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Functional freeze violated: active production registry count changed.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.SplashTitle) || ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Dialogue))
                failures.Add("SplashTitle/Dialogue must remain candidate-only for Step 7.4.");

            string world = Step20ProductionAssetWave01PrefabBuilder.DynamicMapRoot + "/WorldMap";
            string chapter = Step20ProductionAssetWave01PrefabBuilder.DynamicMapRoot + "/ChapterMap";
            string[] required = { "dynamic_node_frame.png", "star_filled.png", "star_empty.png", "selected_halo.png" };
            for (int i = 0; i < required.Length; i++)
            {
                if (!File.Exists(world + "/" + required[i])) failures.Add("Missing WorldMap dynamic asset: " + required[i]);
                if (!File.Exists(chapter + "/" + required[i])) failures.Add("Missing ChapterMap dynamic asset: " + required[i]);
            }

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            bool runtimeCaptureExecuted = File.Exists("ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            bool runtimeVisualPass = File.Exists("ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_VISUAL_QA_PASS.lock");
            bool mapStatePass = File.Exists(Step20ProductionAssetWave01Step74MapStateQa.PassLock);
            Debug.Log(
                "v1.20 Step 7.4 source readiness PASS. runtimeCapture=" + runtimeCaptureExecuted +
                ", runtimeVisualPass=" + runtimeVisualPass +
                ", mapStatePass=" + mapStatePass +
                ", activeCutover=false."
            );
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 source readiness PASS. Next run Runtime Four-Aspect Capture, Runtime Fidelity Analyzer, Dynamic Map-State QA, then physical Android QA before conditional cutover.", "OK");
        }
    }
}
#endif
