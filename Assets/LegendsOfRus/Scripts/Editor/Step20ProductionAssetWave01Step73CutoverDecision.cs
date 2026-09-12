#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>Step 7.3 non-mutating cutover gate. It never edits ProductionCoreLoopPageRegistry.</summary>
    public static class Step20ProductionAssetWave01Step73CutoverDecision
    {
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_3_CUTOVER_DECISION.json";

        [MenuItem("Legends of Rus/Production UI/Evaluate v1.20 Step 7.3 Wave 1 Cutover Decision")]
        public static void Evaluate()
        {
            var failures = new List<string>();
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);
            failures.AddRange(Step20ProductionAssetWave01PrefabBuilder.ValidateAll());
            if (!File.Exists(DecisionPath)) failures.Add("Missing Step 7.3 cutover decision document.");
            else
            {
                string json = File.ReadAllText(DecisionPath);
                if (!json.Contains("\"decision\": \"NO_GO\"")) failures.Add("Packaged Step 7.3 decision must remain NO_GO before Unity/Android signoff.");
                if (!json.Contains("SplashTitle") || !json.Contains("Pause") || !json.Contains("ChapterComplete")) failures.Add("Missing direct-reference blockers were not preserved.");
            }
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Active registry drifted from the accepted 10-page baseline.");
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            Debug.Log("v1.20 Step 7.3 cutover decision: NO-GO. Source fidelity is substantially improved; execute Unity four-aspect + Android + missing-reference signoff before activation.");
        }
    }
}
#endif
