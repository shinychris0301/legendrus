#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>Non-mutating Step 7.2 Wave 1 cutover gate. This class never edits the active registry.</summary>
    public static class Step20ProductionAssetWave01CutoverDecision
    {
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_2_CUTOVER_DECISION.json";
        [MenuItem("Legends of Rus/Production UI/Evaluate v1.20 Step 7.2 Wave 1 Cutover Decision")]
        public static void Evaluate()
        {
            var blockers = new List<string>();
            var prefabFailures = Step20ProductionAssetWave01PrefabBuilder.ValidateAll();
            if (prefabFailures.Count > 0) blockers.Add("candidate-prefab-validation");
            if (!File.Exists(DecisionPath)) blockers.Add("missing-packaged-cutover-decision");
            else
            {
                string json = File.ReadAllText(DecisionPath);
                if (!json.Contains("\"decision\": \"NO_GO\"")) blockers.Add("packaged-decision-not-no-go-as-expected");
                if (!json.Contains("SplashTitle") || !json.Contains("Pause") || !json.Contains("ChapterComplete")) blockers.Add("missing-direct-reference-blockers-not-recorded");
            }
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) blockers.Add("active-registry-drift");
            if (blockers.Count > 0) throw new InvalidOperationException(string.Join("\n", blockers.ToArray()));
            Debug.Log("v1.20 Step 7.2 Wave 1 cutover decision: NO-GO. Serialized candidates are available for QA, but active runtime visuals remain frozen pending reference-accurate reconstruction, three missing direct references, licensed Cyrillic typography, Unity captures, and Android signoff.");
        }
    }
}
#endif
