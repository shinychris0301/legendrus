#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Source-stage decision for v1.20 Step 8.3. Shop may proceed to runtime
    /// fidelity validation, but full Wave 2 cutover cannot be authorized while
    /// only 1/9 pages has direct approved reference authority.
    /// </summary>
    public static class Step20ProductionAssetWave02Step83CutoverDecision
    {
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_3_CUTOVER_DECISION.json";

        [MenuItem("Legends of Rus/Production UI/Evaluate v1.20 Step 8.3 Wave 2 Refinement Decision")]
        public static void Evaluate()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            if (!File.Exists(DecisionPath))
                throw new InvalidOperationException("Missing Step 8.3 cutover decision document.");

            string decision = File.ReadAllText(DecisionPath);
            bool shopSourcePass = decision.IndexOf("SHOP_SOURCE_PREFLIGHT_PASS", StringComparison.Ordinal) >= 0;
            bool fullNoGo = decision.IndexOf("NO_GO_FULL_WAVE02_CUTOVER", StringComparison.Ordinal) >= 0;
            if (!shopSourcePass || !fullNoGo)
                throw new InvalidOperationException("Step 8.3 decision document does not preserve the expected source-pass/full-wave-NO-GO state.");

            Debug.LogWarning("v1.20 Step 8.3 decision: Shop source fidelity PASS and may proceed to Unity runtime validation; full Wave 2 active cutover remains NO-GO because direct approved reference coverage is 1/9.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.3: Shop source fidelity is ready for runtime validation. Full Wave 2 cutover remains NO-GO (direct approved reference coverage 1/9).", "OK");
        }
    }
}
#endif
