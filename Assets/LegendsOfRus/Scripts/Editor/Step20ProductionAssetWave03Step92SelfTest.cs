#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step92SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.2 Wave 3 Runtime QA Readiness Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave03PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave03Step92BoundaryGate.Validate(false));

            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_2_RUNTIME_QA_PLAN.json"))
                failures.Add("Missing Step 9.2 runtime QA plan.");
            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_2_SOCIAL_LIVEOPS_BOUNDARY_POLICY.json"))
                failures.Add("Missing Step 9.2 social/LiveOps boundary policy.");
            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_2_VISUAL_ACCEPTANCE_POLICY.json"))
                failures.Add("Missing Step 9.2 visual acceptance policy.");

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 9.2 readiness self-test PASS: 8 candidates, frozen social/LiveOps authority, 32 runtime captures pending, cutover blocked by 0/8 approved references.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 9.2 readiness PASS. Next: run Social/LiveOps Boundary Verification, then the 32-capture Unity runtime QA.", "OK");
        }
    }
}
#endif
