#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.LiveOpsExecution;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsExecutionExperimentObservationRunner
    {
        const string ConfigPath="Assets/LegendsOfRus/Resources/Config/liveops_execution_activation.json";
        const string EvidenceDir="ProjectDocs/LiveOpsExecution";
        [Serializable] sealed class ActivationConfig{public bool productionAnalyticsProviderEnabled,productionRemoteConfigEnabled,liveOpsExecutionAuthorized,contentRotationAuthorized,experimentObservationAuthorized,experimentTrafficChangeAuthorized,manualExecutionReviewAuthorized;}
        [Serializable] sealed class Snapshot{public int schemaVersion=1;public bool executionPresent,rotationPresent,observationPresent,readyForHumanReview,rotationSafe,experimentObservationHealthy,mayProceedWithManualExecutionReview;public string recommendation=string.Empty;public int reviewBlockerCount,actionBlockerCount;}
        [MenuItem("Legends of Rus/QA/Print v1.19 Step 5 LiveOps Execution Review")]
        public static void PrintReview()
        {
            string root=Directory.GetParent(Application.dataPath).FullName,dir=Path.Combine(root,EvidenceDir);string x=Path.Combine(dir,"execution_evidence.json"),r=Path.Combine(dir,"content_rotation.json"),o=Path.Combine(dir,"experiment_observation.json");
            if(!File.Exists(x)||!File.Exists(r)||!File.Exists(o)){Debug.LogWarning("v1.19 Step 5 review is blocked: reviewed execution/rotation/observation evidence is not retained. Use ProjectDocs/LiveOpsExecution templates.");return;}
            var cfg=LoadActivation(root);var result=LiveOpsExecutionEvaluator.Evaluate(Read<LiveOpsExecutionEvidence>(x),Read<ContentRotationEvidence>(r),Read<ExperimentObservationEvidence>(o),cfg.productionAnalyticsProviderEnabled,cfg.productionRemoteConfigEnabled,cfg.liveOpsExecutionAuthorized,cfg.contentRotationAuthorized,cfg.experimentObservationAuthorized,cfg.experimentTrafficChangeAuthorized,cfg.manualExecutionReviewAuthorized);
            Debug.Log("v1.19 Step 5 execution review: ready="+result.ReadyForHumanReview+", rotationSafe="+result.RotationSafe+", observationHealthy="+result.ExperimentObservationHealthy+", recommendation="+result.Recommendation+", mayReviewExecution="+result.MayProceedWithManualExecutionReview+", blockers="+string.Join(",",result.ActionBlockers));
        }
        [MenuItem("Legends of Rus/QA/Write v1.19 Step 5 Sanitized LiveOps Execution Snapshot")]
        public static void WriteSnapshot()
        {
            string root=Directory.GetParent(Application.dataPath).FullName,dir=Path.Combine(root,EvidenceDir);Directory.CreateDirectory(dir);string x=Path.Combine(dir,"execution_evidence.json"),r=Path.Combine(dir,"content_rotation.json"),o=Path.Combine(dir,"experiment_observation.json");
            var s=new Snapshot{executionPresent=File.Exists(x),rotationPresent=File.Exists(r),observationPresent=File.Exists(o)};
            if(s.executionPresent&&s.rotationPresent&&s.observationPresent){var cfg=LoadActivation(root);var v=LiveOpsExecutionEvaluator.Evaluate(Read<LiveOpsExecutionEvidence>(x),Read<ContentRotationEvidence>(r),Read<ExperimentObservationEvidence>(o),cfg.productionAnalyticsProviderEnabled,cfg.productionRemoteConfigEnabled,cfg.liveOpsExecutionAuthorized,cfg.contentRotationAuthorized,cfg.experimentObservationAuthorized,cfg.experimentTrafficChangeAuthorized,cfg.manualExecutionReviewAuthorized);s.readyForHumanReview=v.ReadyForHumanReview;s.rotationSafe=v.RotationSafe;s.experimentObservationHealthy=v.ExperimentObservationHealthy;s.mayProceedWithManualExecutionReview=v.MayProceedWithManualExecutionReview;s.recommendation=v.Recommendation.ToString();s.reviewBlockerCount=v.ReviewBlockers.Length;s.actionBlockerCount=v.ActionBlockers.Length;}
            string output=Path.Combine(dir,"STEP05_SANITIZED_LIVEOPS_EXECUTION_SNAPSHOT.json");File.WriteAllText(output,JsonUtility.ToJson(s,true)+Environment.NewLine);Debug.Log("Wrote sanitized v1.19 Step 5 snapshot: "+output);
        }
        static ActivationConfig LoadActivation(string root){string p=Path.Combine(root,ConfigPath);if(!File.Exists(p))throw new FileNotFoundException("Missing v1.19 Step 5 activation config",p);var c=JsonUtility.FromJson<ActivationConfig>(File.ReadAllText(p));if(c==null)throw new InvalidOperationException("Invalid v1.19 Step 5 activation config");return c;}
        static T Read<T>(string p) where T:class{var v=JsonUtility.FromJson<T>(File.ReadAllText(p));if(v==null)throw new InvalidOperationException("Invalid evidence: "+p);return v;}
    }
}
#endif
