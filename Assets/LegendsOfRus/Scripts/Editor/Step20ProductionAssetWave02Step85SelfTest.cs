#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>Source/readiness self-test for v1.20 Step 8.5.</summary>
    public static class Step20ProductionAssetWave02Step85SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.5 Source Readiness Self-Test")]
        public static void Run()
        {
            var failures = new List<string>();
            string captureRoot = Step20ProductionAssetWave02Step84RuntimeCapture.AbsoluteOutputRoot();
            bool hasStep84RuntimeEvidence = File.Exists(Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json"));
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
            {
                if (hasStep84RuntimeEvidence)
                    failures.Add("Wave 2 candidate prefabs are stale/missing while Step 8.4 runtime evidence exists. Rebuild, then rerun Step 8.4 capture/Shop gate before cutover.");
                else
                    Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);
            }
            if (!Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Accepted ProductionCoreLoopPageRegistry must remain exactly ten routes.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop))
                failures.Add("Shop must not be inserted into the core-loop production registry.");

            bool active = ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop);
            if (active && !File.Exists(Step20ProductionAssetWave02Step85ShopCutover.AppliedPath))
                failures.Add("Shop meta cutover is active without the Step 8.5 applied audit marker.");
            if (!active && File.Exists(Step20ProductionAssetWave02Step85ShopCutover.AppliedPath))
                failures.Add("Step 8.5 applied audit marker exists while Shop meta cutover is inactive.");

            string designRegistry = "Assets/LegendsOfRus/Scripts/Runtime/UI/DesignScreenRegistry.cs";
            string demoRuntime = "Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFunctionalPageRuntime.cs";
            if (!File.Exists(designRegistry) || File.ReadAllText(designRegistry).IndexOf("ProductionMetaPageCutoverRegistry.IsCutover", StringComparison.Ordinal) < 0)
                failures.Add("DesignScreenRegistry is missing the opt-in meta cutover dispatch guard.");
            if (!File.Exists(demoRuntime) || File.ReadAllText(demoRuntime).IndexOf("ProductionMetaPageRuntime.TryBuild", StringComparison.Ordinal) < 0)
                failures.Add("DemoFunctionalPageRuntime is missing the production meta-page delegation.");

            List<string> provider = Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false);
            for (int i = 0; i < provider.Count; i++) failures.Add("Provider-boundary readiness: " + provider[i]);

            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.5 source readiness PASS. Shop cutover infrastructure is isolated from the accepted ten-route core registry and authoritative commercial/economy paths remain frozen.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", active
                    ? "Step 8.5 source readiness PASS with Shop cutover active. Continue post-cutover regression/Android acceptance."
                    : "Step 8.5 source readiness PASS. Run Step 8.4 runtime evidence/readiness first if not already done, then Evaluate/Apply the rollback-safe Shop cutover.", "OK");
        }
    }
}
#endif
