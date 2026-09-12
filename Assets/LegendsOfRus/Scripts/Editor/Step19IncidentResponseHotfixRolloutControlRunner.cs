#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.IncidentResponse;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19IncidentResponseHotfixRolloutControlRunner
    {
        const string ConfigPath = "Assets/LegendsOfRus/Resources/Config/incident_response_activation.json";
        const string EvidenceDirectory = "ProjectDocs/IncidentResponse";

        [Serializable]
        sealed class ActivationConfig
        {
            public int schemaVersion;
            public bool localIncidentReviewToolingApproved;
            public bool localHotfixQualificationToolingApproved;
            public bool localRolloutControlReviewToolingApproved;
            public bool manualHotfixReleaseAuthorized;
            public bool manualRolloutActionAuthorized;
            public bool automaticPlayActionsEnabled;
            public bool automaticBuildEnabled;
            public bool playPublishingApiEnabled;
        }

        [Serializable]
        sealed class SanitizedSnapshot
        {
            public int schemaVersion = 1;
            public bool incidentRegisterPresent;
            public bool hotfixEvidencePresent;
            public bool rolloutReviewPresent;
            public bool incidentRegisterStructurallyValid;
            public bool productionHealthClear;
            public int openCritical;
            public int openHigh;
            public int pendingRetest;
            public bool hotfixQualified;
            public bool rolloutActionValid;
            public bool manualHotfixReleaseAuthorized;
            public bool manualRolloutActionAuthorized;
            public bool automaticPlayActionsEnabled;
        }

        [MenuItem("Legends of Rus/QA/Print v1.19 Step 2 Incident-Hotfix-Rollout Review")]
        public static void PrintReview()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg = LoadConfig(root);
            string dir = Path.Combine(root, EvidenceDirectory);
            string incidentPath = Path.Combine(dir, "incident_register.json");
            string hotfixPath = Path.Combine(dir, "hotfix_candidate_evidence.json");
            string rolloutPath = Path.Combine(dir, "rollout_control_review.json");

            IncidentRegisterEvidence register = ReadJsonOrNull<IncidentRegisterEvidence>(incidentPath);
            IncidentTriageResult triage = IncidentTriageEvaluator.Evaluate(register);
            Debug.Log("v1.19 Step 2 incident triage: structural=" + triage.RegisterStructurallyValid +
                      ", healthClear=" + triage.ProductionHealthClear +
                      ", recommendation=" + triage.Recommendation +
                      ", blockers=" + string.Join(",", triage.Blockers));

            HotfixCandidateEvidence hotfix = ReadJsonOrNull<HotfixCandidateEvidence>(hotfixPath);
            if (hotfix != null) hotfix.ManualHotfixReleaseAuthorized = cfg.manualHotfixReleaseAuthorized;
            HotfixQualificationResult hotfixResult = HotfixQualificationEvaluator.Evaluate(hotfix, register);
            Debug.Log("v1.19 Step 2 hotfix qualification: review=" + hotfixResult.ReadyForHumanHotfixReview +
                      ", qualified=" + hotfixResult.QualifiedForManualReleasePreparation +
                      ", manualRelease=" + hotfixResult.MayProceedWithAuthorizedManualHotfixRelease +
                      ", blockers=" + string.Join(",", hotfixResult.ReleaseBlockers));

            RolloutControlReview rollout = ReadJsonOrNull<RolloutControlReview>(rolloutPath);
            if (rollout != null)
            {
                rollout.ManualRolloutActionAuthorized = cfg.manualRolloutActionAuthorized;
                rollout.IncidentTriageReviewed = register != null && register.EvidenceReviewed;
                rollout.IncidentHealthClear = triage.ProductionHealthClear;
                rollout.QualifiedHotfixAvailable = hotfixResult.QualifiedForManualReleasePreparation;
            }
            RolloutControlResult rolloutResult = RolloutControlReviewEvaluator.Evaluate(rollout);
            Debug.Log("v1.19 Step 2 rollout control: review=" + rolloutResult.ReadyForHumanRolloutControlReview +
                      ", actionValid=" + rolloutResult.RequestedActionValid +
                      ", manualAction=" + rolloutResult.MayApplyRequestedActionManually +
                      ", blockers=" + string.Join(",", rolloutResult.ActionBlockers));

            if (!File.Exists(incidentPath) || !File.Exists(hotfixPath) || !File.Exists(rolloutPath))
                Debug.LogWarning("v1.19 Step 2 real evidence is incomplete. Copy the templates in ProjectDocs/IncidentResponse only after a real incident/hotfix/rollout review exists.");
        }

        [MenuItem("Legends of Rus/QA/Write v1.19 Step 2 Sanitized Incident-Hotfix-Rollout Snapshot")]
        public static void WriteSanitizedSnapshot()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            ActivationConfig cfg = LoadConfig(root);
            string dir = Path.Combine(root, EvidenceDirectory);
            Directory.CreateDirectory(dir);

            IncidentRegisterEvidence register = ReadJsonOrNull<IncidentRegisterEvidence>(Path.Combine(dir, "incident_register.json"));
            IncidentTriageResult triage = IncidentTriageEvaluator.Evaluate(register);
            HotfixCandidateEvidence hotfix = ReadJsonOrNull<HotfixCandidateEvidence>(Path.Combine(dir, "hotfix_candidate_evidence.json"));
            if (hotfix != null) hotfix.ManualHotfixReleaseAuthorized = cfg.manualHotfixReleaseAuthorized;
            HotfixQualificationResult hotfixResult = HotfixQualificationEvaluator.Evaluate(hotfix, register);
            RolloutControlReview rollout = ReadJsonOrNull<RolloutControlReview>(Path.Combine(dir, "rollout_control_review.json"));
            if (rollout != null)
            {
                rollout.ManualRolloutActionAuthorized = cfg.manualRolloutActionAuthorized;
                rollout.IncidentTriageReviewed = register != null && register.EvidenceReviewed;
                rollout.IncidentHealthClear = triage.ProductionHealthClear;
                rollout.QualifiedHotfixAvailable = hotfixResult.QualifiedForManualReleasePreparation;
            }
            RolloutControlResult rolloutResult = RolloutControlReviewEvaluator.Evaluate(rollout);

            var snapshot = new SanitizedSnapshot
            {
                incidentRegisterPresent = register != null,
                hotfixEvidencePresent = hotfix != null,
                rolloutReviewPresent = rollout != null,
                incidentRegisterStructurallyValid = triage.RegisterStructurallyValid,
                productionHealthClear = triage.ProductionHealthClear,
                openCritical = triage.OpenCritical,
                openHigh = triage.OpenHigh,
                pendingRetest = triage.PendingRetest,
                hotfixQualified = hotfixResult.QualifiedForManualReleasePreparation,
                rolloutActionValid = rolloutResult.RequestedActionValid,
                manualHotfixReleaseAuthorized = cfg.manualHotfixReleaseAuthorized,
                manualRolloutActionAuthorized = cfg.manualRolloutActionAuthorized,
                automaticPlayActionsEnabled = cfg.automaticPlayActionsEnabled
            };
            string path = Path.Combine(dir, "STEP02_SANITIZED_INCIDENT_HOTFIX_ROLLOUT_SNAPSHOT.json");
            File.WriteAllText(path, JsonUtility.ToJson(snapshot, true) + Environment.NewLine);
            Debug.Log("Wrote sanitized v1.19 Step 2 snapshot: " + path);
        }

        static ActivationConfig LoadConfig(string root)
        {
            string path = Path.Combine(root, ConfigPath);
            if (!File.Exists(path)) throw new FileNotFoundException("Missing v1.19 Step 2 activation config", path);
            ActivationConfig cfg = JsonUtility.FromJson<ActivationConfig>(File.ReadAllText(path));
            if (cfg == null) throw new InvalidOperationException("Invalid v1.19 Step 2 activation config");
            return cfg;
        }

        static T ReadJsonOrNull<T>(string path) where T : class
        {
            if (!File.Exists(path)) return null;
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
    }
}
#endif
