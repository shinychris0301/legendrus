#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step94SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.4 Refined Runtime + Authority Intake Readiness Self-Test")]
        public static void Run()
        {
            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);
            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave03PrefabBuilder.ValidateAll());
            failures.AddRange(Step20ProductionAssetWave03Step94BoundaryGate.Validate(false));

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Accepted core production registry changed; expected 10 routes.");
            string refined = Step20ProductionAssetWave03PrefabBuilder.RefinedRoot;
            if (!Directory.Exists(refined) || Directory.GetFiles(refined,"*.png",SearchOption.AllDirectories).Length != 36)
                failures.Add("Expected exactly 36 Step 9.3 refined Wave 3 PNG components.");

            string[] docs = {
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_4_REFINED_RUNTIME_CAPTURE_REFERENCE_AUTHORITY.md",
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_4_REFERENCE_AUTHORITY_INTAKE_POLICY.json",
                "Assets/LegendsOfRus/Documentation/V1_20_STEP09_4_CONDITIONAL_ACCEPTANCE_POLICY.json"
            };
            foreach(string d in docs) if(!File.Exists(d)) failures.Add("Missing Step 9.4 document: "+d);

            ScreenId[] pages={ScreenId.Events,ScreenId.EventDetail,ScreenId.IndividualRanking,ScreenId.TeamHub,ScreenId.TeamRanking,ScreenId.TeamChat,ScreenId.FriendsList,ScreenId.Mail};
            foreach(ScreenId id in pages)
            {
                string path=Step20ProductionAssetWave03PrefabBuilder.CandidateRoot+"/"+id+".prefab";GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if(prefab==null){failures.Add("Missing refined Wave 3 candidate: "+id);continue;}
                if(prefab.GetComponentsInChildren<Button>(true).Length!=0)failures.Add(id+": candidate contains Button authority.");
                if(prefab.GetComponentsInChildren<EventTrigger>(true).Length!=0)failures.Add(id+": candidate contains EventTrigger authority.");
                foreach(string dep in AssetDatabase.GetDependencies(path,true)){string n=dep.Replace('\\','/');if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/"))failures.Add(id+": forbidden reference/QA dependency "+dep);}
            }

            List<string> intakeFailures; Step20ProductionAssetWave03Step94ReferenceAuthorityIntake.CollectWithoutWriting(out intakeFailures); failures.AddRange(intakeFailures);
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 9.4 source readiness PASS: 8 refined candidates, 36 refined components, frozen social/LiveOps authority, 32 runtime captures pending, approved-reference intake is explicit and DesignScreens are non-authoritative.");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 9.4 source readiness PASS. Next: run 32 refined runtime captures, intake approved reference authority, analyze fidelity, then evaluate conditional acceptance readiness.","OK");
        }
    }
}
#endif
