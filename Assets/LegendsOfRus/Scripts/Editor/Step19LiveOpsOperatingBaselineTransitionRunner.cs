#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsBaseline;

namespace LegendsOfRus.EditorTools
{
    public static class Step19LiveOpsOperatingBaselineTransitionRunner
    {
        [MenuItem("Legends of Rus/QA/Print v1.19 Step 8 Operating Baseline & v1.20 Transition Review")]
        public static void PrintReview()
        {
            string root=ProjectRoot();
            var chain=Read<V119EvidenceChainEvidence>(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/v119_evidence_chain.template.json"));
            var baseline=Read<LiveOpsOperatingBaselineEvidence>(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/liveops_operating_baseline.template.json"));
            var transition=Read<V120TransitionPlanEvidence>(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/v120_transition_plan.template.json"));
            var r=LiveOpsBaselineReviewEvaluator.Evaluate(chain,baseline,transition,false,false,false,false,false,false,false,false);
            Debug.Log("v1.19 Step 8 review | recommendation="+r.Recommendation+" | ready="+r.ReadyForHumanReview+" | mayCloseV119="+r.MayCloseV119Manually+" | mayBeginV120="+r.MayBeginV120Preparation+" | reviewBlockers="+string.Join(",",r.ReviewBlockers)+" | actionBlockers="+string.Join(",",r.ActionBlockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.19 Step 8 Sanitized Transition Snapshot")]
        public static void WriteSnapshot()
        {
            string root=ProjectRoot();
            string dir=Path.Combine(root,"ProjectDocs/LiveOpsBaseline/Generated");
            Directory.CreateDirectory(dir);
            string path=Path.Combine(dir,"v1_19_step08_sanitized_transition_snapshot.json");
            string json="{\n  \"schemaVersion\": 1,\n  \"containsCredentials\": false,\n  \"containsDeviceIdentifiers\": false,\n  \"containsTesterIdentity\": false,\n  \"productionMutationAuthorized\": false,\n  \"v120PreparationAuthorized\": false\n}\n";
            File.WriteAllText(path,json);
            AssetDatabase.Refresh();
            Debug.Log("Wrote sanitized Step 8 snapshot: "+path);
        }

        static T Read<T>(string path) where T:class
        {
            if(!File.Exists(path))return null;
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
        static string ProjectRoot(){return Path.GetFullPath(Path.Combine(Application.dataPath,".."));}
    }
}
#endif
