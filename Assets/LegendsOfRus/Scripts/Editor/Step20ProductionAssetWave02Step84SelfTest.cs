#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Source/readiness self-test for Step 8.4. Runtime captures remain external
    /// evidence and are not required for this source package to compile/import.
    /// </summary>
    public static class Step20ProductionAssetWave02Step84SelfTest
    {
        const string Step83Decision = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_3_CUTOVER_DECISION.json";
        const string Step84PackagedDecision = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_PACKAGED_READINESS_STATE.json";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.4 Source Readiness Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false));

            if (!File.Exists(Step83Decision))
                failures.Add("Missing Step 8.3 cutover decision prerequisite.");
            else
            {
                string d = File.ReadAllText(Step83Decision);
                if (d.IndexOf("SHOP_SOURCE_PREFLIGHT_PASS", StringComparison.Ordinal) < 0)
                    failures.Add("Step 8.3 Shop source-preflight PASS prerequisite is missing.");
                if (d.IndexOf("NO_GO_FULL_WAVE02_CUTOVER", StringComparison.Ordinal) < 0)
                    failures.Add("Step 8.3 full-Wave-2 NO-GO prerequisite is missing.");
            }

            if (!File.Exists(Step84PackagedDecision))
                failures.Add("Missing Step 8.4 packaged readiness-state document.");
            else
            {
                string d = File.ReadAllText(Step84PackagedDecision);
                if (d.IndexOf("PENDING_UNITY_RUNTIME_CAPTURE", StringComparison.Ordinal) < 0)
                    failures.Add("Packaged Step 8.4 state must remain pending actual Unity runtime capture.");
                if (d.IndexOf("activeCutoverPerformed\": false", StringComparison.Ordinal) < 0)
                    failures.Add("Packaged Step 8.4 state must record no active cutover.");
            }

            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop))
                failures.Add("Shop must not already be active in the distributed Step 8.4 package.");
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Accepted active production registry must remain exactly 10 routes.");

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.4 source readiness self-test PASS. Runtime capture/gating remains pending user Unity execution; active Wave 2 cutover=false.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.4 source readiness PASS. Next: run 36 Unity runtime captures, Approved Shop Runtime Gate, Provider-Boundary Gate, then Conditional Shop Cutover Readiness.", "OK");
        }
    }
}
#endif
