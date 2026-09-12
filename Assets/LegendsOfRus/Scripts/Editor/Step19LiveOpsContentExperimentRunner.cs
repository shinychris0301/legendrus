#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.LiveOpsReadiness;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsContentExperimentRunner
    {
        const string ConfigPath="Assets/LegendsOfRus/Resources/Config/liveops_readiness_activation.json";
        const string EvidenceDir="ProjectDocs/LiveOpsReadiness";
        [Serializable] sealed class ActivationConfig
        {
            public bool productionAnalyticsProviderEnabled,productionRemoteConfigEnabled,experimentLaunchAuthorized,liveOpsPublishAuthorized,manualLiveOpsReviewAuthorized;
        }
        [Serializable] sealed class SanitizedSnapshot
        {
            public int schemaVersion=1;
            public bool profilePresent,cadencePresent,contentHealthPresent,experimentPresent;
            public bool readyForHumanReview,cadenceSafe,contentHealthy,experimentDesignReady,experimentLaunchReady,mayProceedWithManualLiveOpsPublishReview;
            public string recommendation=string.Empty;
            public int reviewBlockerCount,publishBlockerCount;
        }
        [MenuItem("Legends of Rus/QA/Print v1.19 Step 4 LiveOps-Content-Experiment Review")]
        public static void PrintReview()
        {
            string root=Directory.GetParent(Application.dataPath).FullName,dir=Path.Combine(root,EvidenceDir);
            string p=Path.Combine(dir,"guardrail_profile.json"),c=Path.Combine(dir,"cadence_plan.json"),h=Path.Combine(dir,"content_health.json"),e=Path.Combine(dir,"experiment_proposal.json");
            if(!File.Exists(p)||!File.Exists(c)||!File.Exists(h)||!File.Exists(e)){Debug.LogWarning("v1.19 Step 4 review is blocked: reviewed LiveOps/content/experiment evidence is not retained. Use ProjectDocs/LiveOpsReadiness templates.");return;}
            ActivationConfig cfg=LoadActivation(root);
            LiveOpsReadinessResult r=LiveOpsReadinessEvaluator.Evaluate(Read<ReviewedLiveOpsGuardrailProfile>(p),Read<LiveOpsCadencePlanEvidence>(c),Read<ContentHealthEvidence>(h),Read<ExperimentProposalEvidence>(e),cfg.productionAnalyticsProviderEnabled,cfg.productionRemoteConfigEnabled,cfg.experimentLaunchAuthorized,cfg.liveOpsPublishAuthorized,cfg.manualLiveOpsReviewAuthorized);
            Debug.Log("v1.19 Step 4 review: ready="+r.ReadyForHumanReview+", cadenceSafe="+r.CadenceSafe+", contentHealthy="+r.ContentHealthy+", experimentDesignReady="+r.ExperimentDesignReady+", experimentLaunchReady="+r.ExperimentLaunchReady+", recommendation="+r.Recommendation+", mayReviewPublish="+r.MayProceedWithManualLiveOpsPublishReview+", blockers="+string.Join(",",r.PublishBlockers));
        }
        [MenuItem("Legends of Rus/QA/Write v1.19 Step 4 Sanitized LiveOps Review Snapshot")]
        public static void WriteSnapshot()
        {
            string root=Directory.GetParent(Application.dataPath).FullName,dir=Path.Combine(root,EvidenceDir);Directory.CreateDirectory(dir);
            string p=Path.Combine(dir,"guardrail_profile.json"),c=Path.Combine(dir,"cadence_plan.json"),h=Path.Combine(dir,"content_health.json"),e=Path.Combine(dir,"experiment_proposal.json");
            var s=new SanitizedSnapshot{profilePresent=File.Exists(p),cadencePresent=File.Exists(c),contentHealthPresent=File.Exists(h),experimentPresent=File.Exists(e)};
            if(s.profilePresent&&s.cadencePresent&&s.contentHealthPresent&&s.experimentPresent){ActivationConfig cfg=LoadActivation(root);var r=LiveOpsReadinessEvaluator.Evaluate(Read<ReviewedLiveOpsGuardrailProfile>(p),Read<LiveOpsCadencePlanEvidence>(c),Read<ContentHealthEvidence>(h),Read<ExperimentProposalEvidence>(e),cfg.productionAnalyticsProviderEnabled,cfg.productionRemoteConfigEnabled,cfg.experimentLaunchAuthorized,cfg.liveOpsPublishAuthorized,cfg.manualLiveOpsReviewAuthorized);s.readyForHumanReview=r.ReadyForHumanReview;s.cadenceSafe=r.CadenceSafe;s.contentHealthy=r.ContentHealthy;s.experimentDesignReady=r.ExperimentDesignReady;s.experimentLaunchReady=r.ExperimentLaunchReady;s.mayProceedWithManualLiveOpsPublishReview=r.MayProceedWithManualLiveOpsPublishReview;s.recommendation=r.Recommendation.ToString();s.reviewBlockerCount=r.ReviewBlockers.Length;s.publishBlockerCount=r.PublishBlockers.Length;}
            string output=Path.Combine(dir,"STEP04_SANITIZED_LIVEOPS_REVIEW_SNAPSHOT.json");File.WriteAllText(output,JsonUtility.ToJson(s,true)+Environment.NewLine);Debug.Log("Wrote sanitized v1.19 Step 4 snapshot: "+output);
        }
        static ActivationConfig LoadActivation(string root){string p=Path.Combine(root,ConfigPath);if(!File.Exists(p))throw new FileNotFoundException("Missing v1.19 Step 4 activation config",p);var c=JsonUtility.FromJson<ActivationConfig>(File.ReadAllText(p));if(c==null)throw new InvalidOperationException("Invalid v1.19 Step 4 activation config");return c;}
        static T Read<T>(string p) where T:class{var v=JsonUtility.FromJson<T>(File.ReadAllText(p));if(v==null)throw new InvalidOperationException("Invalid evidence: "+p);return v;}
    }
}
#endif
