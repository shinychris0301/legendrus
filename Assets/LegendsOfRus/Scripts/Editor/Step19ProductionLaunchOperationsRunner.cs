#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.LaunchOperations;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19ProductionLaunchOperationsRunner
    {
        const string ConfigPath = "Assets/LegendsOfRus/Resources/Config/launch_operations_activation.json";
        const string EvidenceDirectory = "ProjectDocs/LaunchOperations";

        [Serializable]
        sealed class ActivationConfig
        {
            public int schemaVersion;
            public string candidateId;
            public string artifactSha256;
            public bool launchCandidateReadinessVerified;
            public bool productionReleaseAuthorized;
            public bool storePublishingAuthorized;
            public bool monitoringPlanApproved;
            public bool incidentResponsePlanApproved;
            public bool rollbackPlanApproved;
            public bool supportCoverageApproved;
            public bool releaseHistoryReviewed;
            public bool outstandingReleasesCleared;
            public bool firstProductionRelease;
            public int requestedRolloutPercent;
            public bool manualLaunchStartAuthorized;
            public bool manualRolloutActionAuthorized;
        }

        [Serializable]
        sealed class SanitizedSnapshot
        {
            public int schemaVersion = 1;
            public string candidateId = string.Empty;
            public bool launchCandidateReadinessVerified;
            public bool productionReleaseAuthorized;
            public bool storePublishingAuthorized;
            public bool monitoringPlanApproved;
            public bool incidentResponsePlanApproved;
            public bool rollbackPlanApproved;
            public bool supportCoverageApproved;
            public bool androidVitalsEvidencePresent;
            public bool incidentRegisterPresent;
            public bool launchWindowReviewPresent;
            public double crashOverallBadBehaviorPercent = LaunchOperationsRequirements.UserPerceivedCrashOverallBadBehaviorPercent;
            public double anrOverallBadBehaviorPercent = LaunchOperationsRequirements.UserPerceivedAnrOverallBadBehaviorPercent;
            public double phoneModelBadBehaviorPercent = LaunchOperationsRequirements.UserPerceivedCrashPhoneModelBadBehaviorPercent;
        }

        [MenuItem("Legends of Rus/QA/Print v1.19 Step 1 Launch Operations Preflight")]
        public static void PrintPreflight()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg = LoadConfig(root);
            var plan = ToPlan(cfg);
            LaunchOperationsPreflightResult preflight = LaunchOperationsPreflightEvaluator.Evaluate(plan);
            Debug.Log("v1.19 Step 1 launch operations preflight: review=" + preflight.ReadyForHumanLaunchOperationsReview +
                      ", manualLaunch=" + preflight.MayStartManualProductionLaunch +
                      ", blockers=" + string.Join(",", preflight.LaunchBlockers));

            string vitalsPath = Path.Combine(root, EvidenceDirectory, "android_vitals_snapshot.json");
            string incidentsPath = Path.Combine(root, EvidenceDirectory, "launch_incident_summary.json");
            if (!File.Exists(vitalsPath) || !File.Exists(incidentsPath))
            {
                Debug.LogWarning("v1.19 Step 1 post-launch health evidence is not retained yet. Use the templates under ProjectDocs/LaunchOperations after a real release exists.");
                return;
            }

            var vitals = JsonUtility.FromJson<AndroidVitalsSnapshot>(File.ReadAllText(vitalsPath));
            var incidents = JsonUtility.FromJson<LaunchIncidentSummary>(File.ReadAllText(incidentsPath));
            var health = AndroidVitalsEvaluator.Evaluate(vitals, incidents, plan.ReleaseKind, plan.RequestedRolloutPercent, cfg.manualRolloutActionAuthorized);
            Debug.Log("v1.19 Step 1 health: class=" + health.Classification + ", recommendation=" + health.Recommendation +
                      ", mayContinue=" + health.MayContinueOrIncreaseRollout + ", blockers=" + string.Join(",", health.Blockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.19 Step 1 Sanitized Launch Operations Snapshot")]
        public static void WriteSanitizedSnapshot()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg = LoadConfig(root);
            string dir = Path.Combine(root, EvidenceDirectory);
            Directory.CreateDirectory(dir);
            var snapshot = new SanitizedSnapshot
            {
                candidateId = cfg.candidateId ?? string.Empty,
                launchCandidateReadinessVerified = cfg.launchCandidateReadinessVerified,
                productionReleaseAuthorized = cfg.productionReleaseAuthorized,
                storePublishingAuthorized = cfg.storePublishingAuthorized,
                monitoringPlanApproved = cfg.monitoringPlanApproved,
                incidentResponsePlanApproved = cfg.incidentResponsePlanApproved,
                rollbackPlanApproved = cfg.rollbackPlanApproved,
                supportCoverageApproved = cfg.supportCoverageApproved,
                androidVitalsEvidencePresent = File.Exists(Path.Combine(dir, "android_vitals_snapshot.json")),
                incidentRegisterPresent = File.Exists(Path.Combine(dir, "launch_incident_summary.json")),
                launchWindowReviewPresent = File.Exists(Path.Combine(dir, "launch_window_review.json"))
            };
            string path = Path.Combine(dir, "STEP01_SANITIZED_LAUNCH_OPERATIONS_SNAPSHOT.json");
            File.WriteAllText(path, JsonUtility.ToJson(snapshot, true) + Environment.NewLine);
            Debug.Log("Wrote sanitized v1.19 Step 1 snapshot: " + path);
        }

        static ActivationConfig LoadConfig(string root)
        {
            string path = Path.Combine(root, ConfigPath);
            if (!File.Exists(path)) throw new FileNotFoundException("Missing v1.19 Step 1 activation config", path);
            ActivationConfig cfg = JsonUtility.FromJson<ActivationConfig>(File.ReadAllText(path));
            if (cfg == null) throw new InvalidOperationException("Invalid v1.19 Step 1 activation config");
            return cfg;
        }

        static LaunchOperationsPlan ToPlan(ActivationConfig cfg)
        {
            return new LaunchOperationsPlan
            {
                CandidateId = cfg.candidateId ?? string.Empty,
                ArtifactSha256 = cfg.artifactSha256 ?? string.Empty,
                LaunchCandidateReadinessVerified = cfg.launchCandidateReadinessVerified,
                ProductionReleaseAuthorized = cfg.productionReleaseAuthorized,
                StorePublishingAuthorized = cfg.storePublishingAuthorized,
                MonitoringPlanApproved = cfg.monitoringPlanApproved,
                IncidentResponsePlanApproved = cfg.incidentResponsePlanApproved,
                RollbackPlanApproved = cfg.rollbackPlanApproved,
                SupportCoverageApproved = cfg.supportCoverageApproved,
                ReleaseHistoryReviewed = cfg.releaseHistoryReviewed,
                OutstandingReleasesCleared = cfg.outstandingReleasesCleared,
                ReleaseKind = cfg.firstProductionRelease ? ReleaseKind.FirstProductionRelease : ReleaseKind.ProductionUpdate,
                RequestedRolloutPercent = cfg.requestedRolloutPercent,
                ManualLaunchStartAuthorized = cfg.manualLaunchStartAuthorized
            };
        }
    }
}
#endif
