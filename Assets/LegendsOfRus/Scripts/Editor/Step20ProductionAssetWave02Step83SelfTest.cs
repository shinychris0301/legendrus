#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 8.3 source/refinement gate. This verifies that Wave 2 uses the
    /// refined sectional production layer, that Shop passes the strict source
    /// approved-reference proxy, and that the accepted provider/runtime authority
    /// remains frozen. It does not authorize active runtime cutover.
    /// </summary>
    public static class Step20ProductionAssetWave02Step83SelfTest
    {
        const string RefinedRoot = "Assets/LegendsOfRus/Art/Production/Wave02Refined";
        const string MetricsPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_3_REFERENCE_FIDELITY_METRICS.csv";
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_3_CUTOVER_DECISION.json";
        const string RefinedManifestPath = "Assets/LegendsOfRus/Art/Production/Wave02Refined/wave02_refined_manifest.json";
        const int ExpectedRefinedPngs = 40;

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.3 Reference-Accurate Wave 2 Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));

            string refinedAbs = Path.Combine(Directory.GetCurrentDirectory(), RefinedRoot.Replace('/', Path.DirectorySeparatorChar));
            int refinedPngs = Directory.Exists(refinedAbs) ? Directory.GetFiles(refinedAbs, "*.png", SearchOption.AllDirectories).Length : 0;
            if (refinedPngs != ExpectedRefinedPngs)
                failures.Add("Expected " + ExpectedRefinedPngs + " Step 8.3 refined PNGs; found " + refinedPngs + ".");

            if (!File.Exists(RefinedManifestPath)) failures.Add("Missing Step 8.3 refined asset manifest.");
            if (!File.Exists(MetricsPath)) failures.Add("Missing Step 8.3 fidelity metrics.");
            if (!File.Exists(DecisionPath)) failures.Add("Missing Step 8.3 cutover decision.");
            else
            {
                string decision = File.ReadAllText(DecisionPath);
                if (decision.IndexOf("NO_GO_FULL_WAVE02_CUTOVER", StringComparison.Ordinal) < 0)
                    failures.Add("Step 8.3 packaged full-Wave-2 decision must remain NO_GO.");
                if (decision.IndexOf("SHOP_SOURCE_PREFLIGHT_PASS", StringComparison.Ordinal) < 0)
                    failures.Add("Step 8.3 decision must record Shop source-preflight PASS.");
            }

            ValidateNoFullScreenReferenceSlice(failures);
            ValidateMetrics(failures);

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Functional freeze violated: accepted active production registry is no longer 10 routes.");

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.3 self-test PASS. Refined PNGs=" + refinedPngs + ", Shop source reference gate=PASS, provider authority=frozen, active Wave 2 cutover=false.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.3 self-test PASS. Shop source fidelity now passes the approved-reference proxy. Full Wave 2 cutover remains blocked by reference coverage and runtime/Android acceptance.", "OK");
        }

        static void ValidateNoFullScreenReferenceSlice(List<string> failures)
        {
            if (!AssetDatabase.IsValidFolder(RefinedRoot))
            {
                failures.Add("Refined Wave 2 folder is missing.");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { RefinedRoot });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null) continue;
                if (texture.width == 941 && texture.height == 1672)
                    failures.Add("Full-screen refined runtime image is forbidden: " + path);
            }
        }

        static void ValidateMetrics(List<string> failures)
        {
            if (!File.Exists(MetricsPath)) return;
            string[] lines = File.ReadAllLines(MetricsPath);
            foreach (string line in lines)
            {
                if (!line.StartsWith("Shop,", StringComparison.Ordinal)) continue;
                string[] parts = line.Split(',');
                if (parts.Length < 6)
                {
                    failures.Add("Malformed Shop fidelity row.");
                    return;
                }
                float correlation, mae;
                if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out correlation) ||
                    !float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out mae))
                {
                    failures.Add("Could not parse Shop fidelity metrics.");
                    return;
                }
                if (correlation < 0.80f || mae > 0.10f)
                    failures.Add("Shop source fidelity gate failed: correlation=" + correlation + ", mae=" + mae + ".");
                return;
            }
            failures.Add("Shop fidelity row is missing.");
        }
    }
}
#endif
