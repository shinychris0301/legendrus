#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsOutcome;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsOutcomeExperimentDecisionCloseoutRunner
    {
        const string EvidenceRoot="ProjectDocs/LiveOpsOutcome";
        [MenuItem("Legends of Rus/QA/Print v1.19 Step 6 LiveOps Outcome Review")]
        public static void PrintReview()
        {
            var outcome=Load<LiveOpsOutcomeEvidence>("outcome_evidence.json","outcome_evidence.template.json");
            var decision=Load<ExperimentDecisionEvidence>("experiment_decision.json","experiment_decision.template.json");
            var closeout=Load<RotationCloseoutEvidence>("rotation_closeout.json","rotation_closeout.template.json");
            var result=LiveOpsOutcomeReviewEvaluator.Evaluate(outcome,decision,closeout,false,false,false,false,false,false);
            Debug.Log("v1.19 Step 6 LiveOps outcome review: recommendation="+result.Recommendation+", readyForHumanReview="+result.ReadyForHumanReview+", manualCloseout="+result.MayProceedWithManualCloseoutReview+", reviewBlockers="+string.Join(",",result.ReviewBlockers)+", actionBlockers="+string.Join(",",result.ActionBlockers));
        }
        [MenuItem("Legends of Rus/QA/Write v1.19 Step 6 Sanitized Closeout Snapshot")]
        public static void WriteSnapshot()
        {
            var outcome=Load<LiveOpsOutcomeEvidence>("outcome_evidence.json","outcome_evidence.template.json");
            var decision=Load<ExperimentDecisionEvidence>("experiment_decision.json","experiment_decision.template.json");
            var closeout=Load<RotationCloseoutEvidence>("rotation_closeout.json","rotation_closeout.template.json");
            var result=LiveOpsOutcomeReviewEvaluator.Evaluate(outcome,decision,closeout,false,false,false,false,false,false);
            string root=Directory.GetParent(Application.dataPath).FullName;
            string path=Path.Combine(root,EvidenceRoot,"last_closeout_review_snapshot.json");
            var snapshot=new SanitizedSnapshot{schemaVersion=1,recommendation=result.Recommendation.ToString(),readyForHumanReview=result.ReadyForHumanReview,manualCloseoutReady=result.MayProceedWithManualCloseoutReview,reviewBlockerCount=result.ReviewBlockers.Length,actionBlockerCount=result.ActionBlockers.Length};
            File.WriteAllText(path,JsonUtility.ToJson(snapshot,true));AssetDatabase.Refresh();Debug.Log("Wrote sanitized Step 6 snapshot: "+path);
        }
        static T Load<T>(string real,string template) where T:class
        {
            string root=Directory.GetParent(Application.dataPath).FullName;string p=Path.Combine(root,EvidenceRoot,real);if(!File.Exists(p))p=Path.Combine(root,EvidenceRoot,template);return JsonUtility.FromJson<T>(File.ReadAllText(p));
        }
        [Serializable] sealed class SanitizedSnapshot{public int schemaVersion;public string recommendation=string.Empty;public bool readyForHumanReview;public bool manualCloseoutReady;public int reviewBlockerCount;public int actionBlockerCount;}
    }
}
#endif
