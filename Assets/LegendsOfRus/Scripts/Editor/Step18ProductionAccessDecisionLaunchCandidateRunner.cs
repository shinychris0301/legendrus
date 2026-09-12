#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.LaunchCandidate;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ProductionAccessDecisionLaunchCandidateRunner
    {
        const string EvidenceDir = "ProjectDocs/ClosedTestEvidence";
        const string ActivationFile = "Assets/LegendsOfRus/Resources/Config/launch_candidate_activation.json";
        const string DecisionFile = EvidenceDir + "/production_access_decision.json";
        const string LaunchFile = EvidenceDir + "/launch_candidate_readiness.json";
        const string SnapshotFile = EvidenceDir + "/STEP08_LAUNCH_CANDIDATE_READINESS_SNAPSHOT.json";

        [Serializable]
        sealed class Activation
        {
            public int schemaVersion;
            public string candidateId = string.Empty;
            public bool localProductionAccessDecisionToolingApproved;
            public bool localLaunchCandidateReadinessToolingApproved;
            public bool developerAccountRuleReviewed;
            public bool productionAccessRequirementResolved;
            public bool productionAccessDecisionRequired;
            public bool productionAccessDecisionEvidenceRetained;
            public bool productionAccessDecisionReviewed;
            public bool productionAccessDecisionSatisfied;
            public bool candidateFreezeManifestRetained;
            public bool candidateFreezeVerified;
            public bool artifactMatchesFreeze;
            public bool closedTestExecutionEvidenceVerified;
            public bool feedbackTriageReviewed;
            public bool storeReleaseChecklistApproved;
            public bool privacyAndDataSafetyApproved;
            public bool legalDocumentsApproved;
            public bool appContentDeclarationsApproved;
            public bool productionApplicationIdApproved;
            public bool versioningApproved;
            public bool playAppSigningConfigured;
            public bool uploadKeyWorkflowReviewed;
            public bool androidTechnicalRequirementsVerified;
            public bool releaseNotesApproved;
            public bool productionCountriesRegionsApproved;
            public bool supportReadinessApproved;
            public bool launchMonitoringApproved;
            public bool rollbackPlanApproved;
            public bool releaseHistoryReviewed;
            public bool outstandingReleasesCleared;
            public bool firstProductionRelease;
            public int requestedRolloutPercent;
            public bool productionReleaseAuthorized;
            public bool storePublishingAuthorized;
        }

        [Serializable]
        sealed class SanitizedSnapshot
        {
            public int schemaVersion = 1;
            public string candidateId = string.Empty;
            public bool productionAccessDecisionSatisfied;
            public bool readyForHumanLaunchReview;
            public bool mayCreateProductionReleaseDraft;
            public bool mayStartManualProductionRollout;
            public int requestedRolloutPercent;
            public string[] productionAccessBlockers = Array.Empty<string>();
            public string[] launchReviewBlockers = Array.Empty<string>();
            public string[] rolloutBlockers = Array.Empty<string>();
        }

        [MenuItem("Legends of Rus/QA/Print v1.18 Step 8 Production Access Decision & Launch Candidate Readiness")]
        public static void Print()
        {
            var state = Evaluate();
            Debug.Log("v1.18 Step 8 production-access satisfied=" + state.decision.Satisfied
                + ", human-launch-review=" + state.launch.ReadyForHumanLaunchReview
                + ", production-draft=" + state.launch.MayCreateProductionReleaseDraft
                + ", manual-rollout=" + state.launch.MayStartManualProductionRollout
                + "\nProduction-access blockers: " + Join(state.decision.Blockers)
                + "\nLaunch-review blockers: " + Join(state.launch.ReviewBlockers)
                + "\nRollout blockers: " + Join(state.launch.RolloutBlockers));
        }

        [MenuItem("Legends of Rus/QA/Write v1.18 Step 8 Sanitized Launch Readiness Snapshot")]
        public static void WriteSnapshot()
        {
            var state = Evaluate();
            Directory.CreateDirectory(EvidenceDir);
            var snapshot = new SanitizedSnapshot
            {
                candidateId = state.activation.candidateId,
                productionAccessDecisionSatisfied = state.decision.Satisfied,
                readyForHumanLaunchReview = state.launch.ReadyForHumanLaunchReview,
                mayCreateProductionReleaseDraft = state.launch.MayCreateProductionReleaseDraft,
                mayStartManualProductionRollout = state.launch.MayStartManualProductionRollout,
                requestedRolloutPercent = state.activation.requestedRolloutPercent,
                productionAccessBlockers = state.decision.Blockers,
                launchReviewBlockers = state.launch.ReviewBlockers,
                rolloutBlockers = state.launch.RolloutBlockers
            };
            File.WriteAllText(SnapshotFile, JsonUtility.ToJson(snapshot, true));
            AssetDatabase.Refresh();
            Debug.Log("Wrote privacy-minimal Step 8 snapshot: " + SnapshotFile);
        }

        static (Activation activation, ProductionAccessDecisionResult decision, LaunchCandidateReadinessResult launch) Evaluate()
        {
            var activation = ReadJson<Activation>(ActivationFile) ?? new Activation();
            var decisionEvidence = ReadJson<ProductionAccessDecisionEvidence>(DecisionFile) ?? new ProductionAccessDecisionEvidence
            {
                DeveloperAccountRuleReviewed = activation.developerAccountRuleReviewed,
                ProductionAccessRequirementResolved = activation.productionAccessRequirementResolved,
                ProductionAccessDecisionRequired = activation.productionAccessDecisionRequired,
                DecisionEvidenceRetained = activation.productionAccessDecisionEvidenceRetained,
                DecisionReviewed = activation.productionAccessDecisionReviewed,
                DecisionStatus = activation.productionAccessDecisionSatisfied
                    ? ProductionAccessDecisionStatus.Granted
                    : ProductionAccessDecisionStatus.Unknown
            };
            var decision = ProductionAccessDecisionEvaluator.Evaluate(decisionEvidence);

            var launchEvidence = ReadJson<LaunchCandidateEvidence>(LaunchFile) ?? new LaunchCandidateEvidence
            {
                CandidateId = activation.candidateId,
                CandidateFreezeManifestRetained = activation.candidateFreezeManifestRetained,
                CandidateFreezeVerified = activation.candidateFreezeVerified,
                ArtifactMatchesFreeze = activation.artifactMatchesFreeze,
                ClosedTestExecutionEvidenceValid = activation.closedTestExecutionEvidenceVerified,
                FeedbackTriageReady = activation.feedbackTriageReviewed,
                ProductionAccessDecisionSatisfied = decision.Satisfied,
                StoreReleaseChecklistComplete = activation.storeReleaseChecklistApproved,
                PrivacyAndDataSafetyApproved = activation.privacyAndDataSafetyApproved,
                LegalDocumentsApproved = activation.legalDocumentsApproved,
                AppContentDeclarationsApproved = activation.appContentDeclarationsApproved,
                ProductionApplicationIdApproved = activation.productionApplicationIdApproved,
                VersioningApproved = activation.versioningApproved,
                PlayAppSigningConfigured = activation.playAppSigningConfigured,
                UploadKeyWorkflowReviewed = activation.uploadKeyWorkflowReviewed,
                TargetApiLevel = activation.androidTechnicalRequirementsVerified ? LaunchCandidateRequirements.MinimumTargetApiLevel : 0,
                AppBundleVerified = activation.androidTechnicalRequirementsVerified,
                Arm64Verified = activation.androidTechnicalRequirementsVerified,
                PageSize16KbVerified = activation.androidTechnicalRequirementsVerified,
                ReleaseNotesApproved = activation.releaseNotesApproved,
                ProductionCountriesRegionsApproved = activation.productionCountriesRegionsApproved,
                SupportReadinessApproved = activation.supportReadinessApproved,
                LaunchMonitoringApproved = activation.launchMonitoringApproved,
                RollbackPlanApproved = activation.rollbackPlanApproved,
                ReleaseHistoryReviewed = activation.releaseHistoryReviewed,
                OutstandingReleasesCleared = activation.outstandingReleasesCleared,
                FirstProductionRelease = activation.firstProductionRelease,
                RequestedRolloutPercent = activation.requestedRolloutPercent,
                ProductionReleaseAuthorized = activation.productionReleaseAuthorized,
                StorePublishingAuthorized = activation.storePublishingAuthorized
            };
            var launch = LaunchCandidateReadinessEvaluator.Evaluate(launchEvidence);
            return (activation, decision, launch);
        }

        static T ReadJson<T>(string path) where T : class
        {
            if (!File.Exists(path)) return null;
            try { return JsonUtility.FromJson<T>(File.ReadAllText(path)); }
            catch (Exception e) { Debug.LogWarning("Step 8 evidence parse failed for " + path + ": " + e.Message); return null; }
        }

        static string Join(string[] values) => values == null || values.Length == 0 ? "none" : string.Join(", ", values);
    }
}
#endif
