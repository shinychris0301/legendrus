#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step93SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.3 Reference-Accurate Wave 3 Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave03PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave03Step92BoundaryGate.Validate(false));

            string refined = Step20ProductionAssetWave03PrefabBuilder.RefinedRoot;
            if (!Directory.Exists(refined)) failures.Add("Missing Wave03Refined asset root.");
            else if (Directory.GetFiles(refined, "*.png", SearchOption.AllDirectories).Length != 36)
                failures.Add("Expected exactly 36 Step 9.3 refined PNG components.");

            string[] docs = {
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_REFERENCE_ACCURATE_WAVE03.md",
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_REFINEMENT_MANIFEST.json",
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_GUIDANCE_FIDELITY_METRICS.csv",
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_CUTOVER_DECISION.json"
            };
            foreach (string d in docs) if (!File.Exists(d)) failures.Add("Missing Step 9.3 document: " + d);

            ScreenId[] pages = { ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub, ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail };
            foreach (ScreenId id in pages)
            {
                string path = Step20ProductionAssetWave03PrefabBuilder.CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) { failures.Add("Missing refined candidate: " + id); continue; }
                if (prefab.GetComponentsInChildren<Button>(true).Length != 0) failures.Add(id + ": visual candidate contains Button authority.");
                if (prefab.GetComponentsInChildren<EventTrigger>(true).Length != 0) failures.Add(id + ": visual candidate contains EventTrigger authority.");
                foreach (string dep in AssetDatabase.GetDependencies(path, true))
                {
                    string n = dep.Replace('\\','/');
                    if (n.Contains("/Art/ReferenceOnly/") || n.Contains("/Resources/DesignScreens/") || n.Contains("/ProjectDocs/"))
                        failures.Add(id + ": forbidden reference/QA dependency " + dep);
                }
            }

            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            Debug.Log("v1.20 Step 9.3 self-test PASS: 8 refined Wave 3 candidates, 36 refined components, frozen social/LiveOps authority, 0/8 approved-reference coverage, no active cutover.");
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", "Step 9.3 reference-accurate Wave 3 self-test PASS. Runtime cutover remains blocked by 0/8 direct approved references.", "OK");
        }
    }
}
#endif
