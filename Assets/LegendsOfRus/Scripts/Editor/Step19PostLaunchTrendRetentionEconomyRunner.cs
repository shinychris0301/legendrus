#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.PostLaunchReview;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19PostLaunchTrendRetentionEconomyRunner
    {
        const string ConfigPath = "Assets/LegendsOfRus/Resources/Config/post_launch_review_activation.json";
        const string EvidenceDir = "ProjectDocs/PostLaunchReview";

        [Serializable] sealed class ActivationConfig
        {
            public int schemaVersion;
            public bool guardrailProfileRetained, guardrailProfileReviewed, trendWindowEvidenceRetained, trendWindowEvidenceReviewed;
            public bool retentionCohortEvidenceRetained, retentionCohortEvidenceReviewed, economyFlowEvidenceRetained, economyFlowEvidenceReviewed;
            public bool manualEconomyChangeReviewAuthorized;
        }
        [Serializable] sealed class SanitizedSnapshot
        {
            public int schemaVersion=1;
            public bool guardrailProfilePresent, trendWindowPresent, retentionCohortPresent, economyFlowPresent;
            public bool readyForHumanReview, trendRetentionHealthy, economyHealthy, mayProceedWithManualEconomyChangeReview;
            public string recommendation=string.Empty;
            public int reviewBlockerCount, economyChangeBlockerCount;
        }

        [MenuItem("Legends of Rus/QA/Print v1.19 Step 3 Post-Launch Trend-Retention-Economy Review")]
        public static void PrintReview()
        {
            string root=Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg=LoadActivation(root);
            string profilePath=Path.Combine(root,EvidenceDir,"guardrail_profile.json");
            string trendPath=Path.Combine(root,EvidenceDir,"trend_window.json");
            string retentionPath=Path.Combine(root,EvidenceDir,"retention_cohort.json");
            string economyPath=Path.Combine(root,EvidenceDir,"economy_flow.json");
            if (!File.Exists(profilePath)||!File.Exists(trendPath)||!File.Exists(retentionPath)||!File.Exists(economyPath))
            {
                Debug.LogWarning("v1.19 Step 3 review is blocked: real reviewed evidence files are not retained yet. Use the templates under ProjectDocs/PostLaunchReview.");
                return;
            }
            var result=PostLaunchReviewEvaluator.Evaluate(Read<ReviewedGuardrailProfile>(profilePath),Read<TrendWindowEvidence>(trendPath),Read<RetentionCohortEvidence>(retentionPath),Read<EconomyFlowEvidence>(economyPath),cfg.manualEconomyChangeReviewAuthorized);
            Debug.Log("v1.19 Step 3 review: ready="+result.ReadyForHumanReview+", trendRetentionHealthy="+result.TrendRetentionHealthy+", economyHealthy="+result.EconomyHealthy+", recommendation="+result.Recommendation+", mayReviewEconomyChange="+result.MayProceedWithManualEconomyChangeReview+", blockers="+string.Join(",",result.ReviewBlockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.19 Step 3 Sanitized Post-Launch Review Snapshot")]
        public static void WriteSanitizedSnapshot()
        {
            string root=Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg=LoadActivation(root);
            string dir=Path.Combine(root,EvidenceDir); Directory.CreateDirectory(dir);
            string profilePath=Path.Combine(dir,"guardrail_profile.json"), trendPath=Path.Combine(dir,"trend_window.json"), retentionPath=Path.Combine(dir,"retention_cohort.json"), economyPath=Path.Combine(dir,"economy_flow.json");
            var snap=new SanitizedSnapshot { guardrailProfilePresent=File.Exists(profilePath), trendWindowPresent=File.Exists(trendPath), retentionCohortPresent=File.Exists(retentionPath), economyFlowPresent=File.Exists(economyPath) };
            if (snap.guardrailProfilePresent&&snap.trendWindowPresent&&snap.retentionCohortPresent&&snap.economyFlowPresent)
            {
                var r=PostLaunchReviewEvaluator.Evaluate(Read<ReviewedGuardrailProfile>(profilePath),Read<TrendWindowEvidence>(trendPath),Read<RetentionCohortEvidence>(retentionPath),Read<EconomyFlowEvidence>(economyPath),cfg.manualEconomyChangeReviewAuthorized);
                snap.readyForHumanReview=r.ReadyForHumanReview; snap.trendRetentionHealthy=r.TrendRetentionHealthy; snap.economyHealthy=r.EconomyHealthy; snap.mayProceedWithManualEconomyChangeReview=r.MayProceedWithManualEconomyChangeReview; snap.recommendation=r.Recommendation.ToString(); snap.reviewBlockerCount=r.ReviewBlockers.Length; snap.economyChangeBlockerCount=r.EconomyChangeBlockers.Length;
            }
            string outPath=Path.Combine(dir,"STEP03_SANITIZED_POST_LAUNCH_REVIEW_SNAPSHOT.json");
            File.WriteAllText(outPath,JsonUtility.ToJson(snap,true)+Environment.NewLine);
            Debug.Log("Wrote sanitized v1.19 Step 3 snapshot: "+outPath);
        }

        static ActivationConfig LoadActivation(string root)
        {
            string path=Path.Combine(root,ConfigPath); if(!File.Exists(path)) throw new FileNotFoundException("Missing v1.19 Step 3 activation config",path);
            var cfg=JsonUtility.FromJson<ActivationConfig>(File.ReadAllText(path)); if(cfg==null) throw new InvalidOperationException("Invalid v1.19 Step 3 activation config"); return cfg;
        }
        static T Read<T>(string path) where T:class { var v=JsonUtility.FromJson<T>(File.ReadAllText(path)); if(v==null) throw new InvalidOperationException("Invalid evidence: "+path); return v; }
    }
}
#endif
