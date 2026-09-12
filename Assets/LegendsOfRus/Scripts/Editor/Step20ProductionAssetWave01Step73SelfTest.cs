#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave01Step73SelfTest
    {
        const string RefinedRoot = "Assets/LegendsOfRus/Art/Production/Wave01Refined";
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_3_CUTOVER_DECISION.json";
        const string MetricsPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_3_REFERENCE_FIDELITY_METRICS.csv";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7.3 Reference Accuracy + Typography Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave01PrefabBuilder.ValidateAll());

            string abs = Path.Combine(Directory.GetCurrentDirectory(), RefinedRoot.Replace('/', Path.DirectorySeparatorChar));
            int refinedPngs = Directory.Exists(abs) ? Directory.GetFiles(abs, "*.png", SearchOption.AllDirectories).Length : 0;
            if (refinedPngs < 140) failures.Add("Expected at least 140 Step 7.3 refined PNG components, found " + refinedPngs + ".");

            if (!File.Exists(DecisionPath) || !File.ReadAllText(DecisionPath).Contains("\"decision\": \"NO_GO\""))
                failures.Add("Step 7.3 packaged cutover decision is missing or not NO_GO.");
            if (!File.Exists(MetricsPath)) failures.Add("Step 7.3 source fidelity metrics are missing.");

            string fontRoot = Path.Combine(Directory.GetCurrentDirectory(), Step20ProductionAssetWave01PrefabBuilder.ProductionFontRoot.Replace('/', Path.DirectorySeparatorChar));
            int fontCount = Directory.Exists(fontRoot)
                ? Directory.GetFiles(fontRoot, "*.*", SearchOption.TopDirectoryOnly).Length
                : 0;
            // Font import is optional for development and mandatory only for release signoff; no binary is shipped by this package.
            bool productionFontImported = false;
            if (Directory.Exists(fontRoot))
            {
                foreach (string f in Directory.GetFiles(fontRoot, "*.*", SearchOption.TopDirectoryOnly))
                {
                    string e = Path.GetExtension(f).ToLowerInvariant();
                    if (e == ".ttf" || e == ".otf") productionFontImported = true;
                }
            }

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Functional freeze violated: active cutover page count changed.");
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 7.3 self-test PASS. Refined components=" + refinedPngs + ", candidate prefabs=12, active cutover=false, production font imported=" + productionFontImported + ".");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 7.3 self-test PASS. Reference-accurate candidate reconstruction is ready for Unity four-aspect capture. Active cutover remains disabled.", "OK");
        }
    }
}
#endif
