#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsCycle;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsCycleRetrospectiveArchiveNextCadenceRunner
    {
        [MenuItem("Legends of Rus/QA/Print v1.19 Step 7 LiveOps Cycle Review")]
        public static void Print()
        {
            string root=Directory.GetParent(Application.dataPath).FullName;
            var r=Load<CycleRetrospectiveEvidence>(root,"ProjectDocs/LiveOpsCycle/cycle_retrospective.json");
            var a=Load<CycleArchiveEvidence>(root,"ProjectDocs/LiveOpsCycle/cycle_archive.json");
            var c=Load<NextCadencePlanEvidence>(root,"ProjectDocs/LiveOpsCycle/next_cadence_plan.json");
            var result=LiveOpsCycleReviewEvaluator.Evaluate(r,a,c,false,false,false,false,false,false,false);
            Debug.Log("v1.19 Step 7 LiveOps Cycle Review: ready="+result.ReadyForHumanReview+", manualClose="+result.MayCloseCycleManually+", publishNext="+result.MayPublishNextCadence+", recommendation="+result.Recommendation+", reviewBlockers="+string.Join(",",result.ReviewBlockers)+", actionBlockers="+string.Join(",",result.ActionBlockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.19 Step 7 Sanitized Cycle Snapshot")]
        public static void WriteSnapshot()
        {
            string root=Directory.GetParent(Application.dataPath).FullName;
            string dir=Path.Combine(root,"ProjectDocs/LiveOpsCycle");Directory.CreateDirectory(dir);
            var snapshot=new SanitizedSnapshot{version="v1.19-step07",containsPersonalData=false,containsCredentials=false,containsDeviceIdentifiers=false,productionMutationEnabled=false,archiveUploadEnabled=false,publishingEnabled=false};
            File.WriteAllText(Path.Combine(dir,"sanitized_cycle_snapshot.json"),JsonUtility.ToJson(snapshot,true));AssetDatabase.Refresh();
            Debug.Log("Wrote sanitized Step 7 cycle snapshot.");
        }

        static T Load<T>(string root,string rel) where T:class
        {
            string p=Path.Combine(root,rel);if(!File.Exists(p))return null;try{return JsonUtility.FromJson<T>(File.ReadAllText(p));}catch{return null;}
        }
        [Serializable] sealed class SanitizedSnapshot{public string version="";public bool containsPersonalData;public bool containsCredentials;public bool containsDeviceIdentifiers;public bool productionMutationEnabled;public bool archiveUploadEnabled;public bool publishingEnabled;}
    }
}
#endif
