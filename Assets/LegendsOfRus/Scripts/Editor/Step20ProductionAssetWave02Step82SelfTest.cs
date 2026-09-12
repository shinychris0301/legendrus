#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave02Step82SelfTest
    {
        const string PackagedDecision = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_2_VISUAL_ACCEPTANCE_DECISION.json";
        const string QaPlan = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_2_RUNTIME_QA_PROVIDER_VISUAL_PLAN.json";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.2 Wave 2 Runtime QA Readiness Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));

            if (!File.Exists(QaPlan)) failures.Add("Missing Step 8.2 runtime QA/provider/visual plan.");
            if (!File.Exists(PackagedDecision)) failures.Add("Missing packaged Step 8.2 visual acceptance decision.");
            else
            {
                string decision = File.ReadAllText(PackagedDecision);
                bool initialNoGo = decision.IndexOf("NO_GO_RUNTIME_EVIDENCE_PENDING", StringComparison.Ordinal) >= 0;
                bool analyzedNoGo = decision.IndexOf("NO_GO_FULL_WAVE02_CUTOVER", StringComparison.Ordinal) >= 0;
                if (!initialNoGo && !analyzedNoGo)
                    failures.Add("Step 8.2 decision must remain a NO-GO state; this milestone never authorizes active Wave 2 cutover.");
            }

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Functional freeze violated: active production registry count changed.");

            string captureRoot = Step20ProductionAssetWave02Step82RuntimeCapture.AbsoluteOutputRoot();
            bool captureExecuted = File.Exists(Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json"));
            bool shopStrictPass = File.Exists(Path.Combine(captureRoot, "SHOP_APPROVED_REFERENCE_RUNTIME_PASS.lock"));

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.2 readiness PASS. Wave02 candidates=9, providerBoundary=PASS, activeCutover=false, runtimeCapture=" + captureExecuted + ", shopStrictPass=" + shopStrictPass + ".");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.2 readiness PASS. Next run the 36-image Unity Runtime Four-Aspect Capture, then Runtime Visual Acceptance. Wave 2 cutover remains blocked.", "OK");
        }
    }
}
#endif
