#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.CandidateFreeze;
using LegendsOfRus.ClosedTestRollout;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ClosedTestRolloutReadinessRunner
    {
        const string EvidenceDir = "ProjectDocs/ClosedTestEvidence";
        const string FreezeManifestFile = EvidenceDir + "/CANDIDATE_FREEZE_MANIFEST.json";
        const string FreezeManifestHashFile = EvidenceDir + "/CANDIDATE_FREEZE_MANIFEST.sha256";
        const string UploadEvidenceFile = EvidenceDir + "/closed_test_upload_evidence.json";
        const string HealthEvidenceFile = EvidenceDir + "/closed_test_health_evidence.json";
        const string SnapshotFile = EvidenceDir + "/STEP06_ROLLOUT_READINESS_SNAPSHOT.json";
        const string ActivationFile = "Assets/LegendsOfRus/Resources/Config/closed_test_rollout_activation.json";

        [Serializable]
        sealed class Activation
        {
            public int schemaVersion;
            public string candidateId = string.Empty;
            public bool localClosedTestReadinessToolingApproved;
            public bool localGoNoGoToolingApproved;
            public bool localRolloutReadinessToolingApproved;
            public bool candidateFreezeManifestRetained;
            public bool candidateFreezeManifestVerified;
            public bool artifactHashMatchesFreeze;
            public bool playAppSetupComplete;
            public bool testingTrackReleasePermissionApproved;
            public bool closedTestTrackConfigured;
            public bool testerAccessConfigured;
            public bool countryAvailabilityReviewed;
            public bool releaseNotesApproved;
            public bool noOutstandingRelease;
            public bool closedTestUploadAuthorized;
            public bool closedTestPublishAuthorized;
            public bool closedTestUploadEvidenceRetained;
            public bool closedTestHealthEvidenceRetained;
            public bool closedTestGoNoGoAuthorized;
            public bool developerAccountTypeReviewed;
            public bool personalAccountCreatedAfter2023November13;
            public bool productionAccessGranted;
            public bool storeReleaseChecklistApproved;
            public bool privacyDataSafetyApproved;
            public bool contentRatingApproved;
            public bool targetAudienceApproved;
            public bool adsDeclarationApproved;
            public bool appAccessDeclarationApproved;
            public bool rollbackPlanReviewed;
            public bool monitoringPlanReviewed;
            public bool managedPublishingReviewed;
            public bool isFirstProductionRelease;
            public int requestedRolloutPercent;
            public bool productionRolloutAuthorized;
            public bool storePublishingAuthorized;
        }

        [Serializable]
        sealed class SanitizedSnapshot
        {
            public int schemaVersion = ClosedTestRolloutRequirements.SchemaVersion;
            public string candidateId = string.Empty;
            public bool freezeManifestVerified;
            public string freezeManifestSha256 = string.Empty;
            public string artifactSha256 = string.Empty;
            public bool mayPrepareClosedTestRelease;
            public bool mayRolloutClosedTestRelease;
            public bool uploadEvidenceValid;
            public int closedTestReleaseStatus;
            public bool healthEvidenceValid;
            public int observationDays;
            public int optedInTesterCount;
            public int continuousOptedInTesterCount;
            public int continuousTestDays;
            public bool mayApplyForProductionAccess;
            public bool mayRolloutProduction;
            public bool usesStagedRollout;
            public string[] closedTestBlockers = Array.Empty<string>();
            public string[] productionAccessBlockers = Array.Empty<string>();
            public string[] productionRolloutBlockers = Array.Empty<string>();
        }

        sealed class EvaluationContext
        {
            public Activation ActivationData;
            public CandidateFreezeManifest FreezeManifest;
            public string FreezeManifestSha256 = string.Empty;
            public bool FreezeManifestVerified;
            public ClosedTestUploadEvidence UploadEvidence;
            public ClosedTestEvidenceValidationResult UploadValidation;
            public ClosedTestHealthEvidence HealthEvidence;
            public ClosedTestEvidenceValidationResult HealthValidation;
            public ClosedTestUploadReadinessResult ClosedTestReadiness;
            public ProductionRolloutReadinessResult ProductionReadiness;
            public string[] IntakeIssues = Array.Empty<string>();
        }

        [MenuItem("Legends of Rus/QA/Print v1.18 Step 6 Closed Test Go-No-Go")]
        public static void PrintPreflight()
        {
            Debug.Log(Format(EvaluateCurrent()));
        }

        [MenuItem("Legends of Rus/QA/Write v1.18 Step 6 Sanitized Rollout Snapshot")]
        public static void WriteSanitizedSnapshot()
        {
            EvaluationContext c = EvaluateCurrent();
            Directory.CreateDirectory(Full(EvidenceDir));
            var snapshot = new SanitizedSnapshot
            {
                candidateId = c.ActivationData == null ? string.Empty : c.ActivationData.candidateId,
                freezeManifestVerified = c.FreezeManifestVerified,
                freezeManifestSha256 = c.FreezeManifestSha256,
                artifactSha256 = c.FreezeManifest == null ? string.Empty : c.FreezeManifest.ArtifactSha256,
                mayPrepareClosedTestRelease = c.ClosedTestReadiness != null && c.ClosedTestReadiness.MayPrepareClosedTestRelease,
                mayRolloutClosedTestRelease = c.ClosedTestReadiness != null && c.ClosedTestReadiness.MayRolloutClosedTestRelease,
                uploadEvidenceValid = c.UploadValidation != null && c.UploadValidation.Valid,
                closedTestReleaseStatus = c.UploadEvidence == null ? 0 : (int)c.UploadEvidence.ReleaseStatus,
                healthEvidenceValid = c.HealthValidation != null && c.HealthValidation.Valid,
                observationDays = c.HealthEvidence == null ? 0 : c.HealthEvidence.ObservationDays,
                optedInTesterCount = c.HealthEvidence == null ? 0 : c.HealthEvidence.OptedInTesterCount,
                continuousOptedInTesterCount = c.HealthEvidence == null ? 0 : c.HealthEvidence.ContinuousOptedInTesterCount,
                continuousTestDays = c.HealthEvidence == null ? 0 : c.HealthEvidence.ContinuousTestDays,
                mayApplyForProductionAccess = c.ProductionReadiness != null && c.ProductionReadiness.MayApplyForProductionAccess,
                mayRolloutProduction = c.ProductionReadiness != null && c.ProductionReadiness.MayRolloutProduction,
                usesStagedRollout = c.ProductionReadiness != null && c.ProductionReadiness.UsesStagedRollout,
                closedTestBlockers = c.ClosedTestReadiness == null ? Array.Empty<string>() : c.ClosedTestReadiness.RolloutBlockers,
                productionAccessBlockers = c.ProductionReadiness == null ? Array.Empty<string>() : c.ProductionReadiness.ProductionAccessBlockers,
                productionRolloutBlockers = c.ProductionReadiness == null ? Array.Empty<string>() : c.ProductionReadiness.ProductionRolloutBlockers
            };
            File.WriteAllText(Full(SnapshotFile), JsonUtility.ToJson(snapshot, true) + "\n", new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log("v1.18 Step 6 sanitized rollout-readiness snapshot written. It contains no tester identities, Play credentials, signing secrets, or device identifiers.");
        }

        static EvaluationContext EvaluateCurrent()
        {
            var c = new EvaluationContext();
            var issues = new List<string>();
            c.ActivationData = LoadActivation();
            if (c.ActivationData == null) issues.Add("Missing Step 6 activation config.");

            string freezePath = Full(FreezeManifestFile);
            string freezeHashPath = Full(FreezeManifestHashFile);
            if (File.Exists(freezePath))
            {
                c.FreezeManifestSha256 = Sha256File(freezePath);
                c.FreezeManifest = JsonUtility.FromJson<CandidateFreezeManifest>(File.ReadAllText(freezePath));
                string retainedHash = ReadFirstHash(freezeHashPath);
                c.FreezeManifestVerified = c.FreezeManifest != null && c.FreezeManifest.Frozen &&
                    CandidateFreezeEvaluator.IsSha256(c.FreezeManifest.ArtifactSha256) &&
                    string.Equals(retainedHash, c.FreezeManifestSha256, StringComparison.OrdinalIgnoreCase);
                if (!c.FreezeManifestVerified) issues.Add("Candidate freeze manifest is present but its retained SHA-256/frozen state is not valid.");
            }
            else
            {
                issues.Add("Missing " + FreezeManifestFile + ".");
            }

            string expectedArtifactSha = c.FreezeManifest == null ? string.Empty : c.FreezeManifest.ArtifactSha256;
            if (File.Exists(Full(UploadEvidenceFile)))
            {
                c.UploadEvidence = JsonUtility.FromJson<ClosedTestUploadEvidence>(File.ReadAllText(Full(UploadEvidenceFile)));
                c.UploadValidation = ClosedTestEvidenceValidator.ValidateUpload(c.UploadEvidence, expectedArtifactSha);
                if (!c.UploadValidation.Valid) issues.AddRange(c.UploadValidation.Issues);
            }
            else issues.Add("Missing " + UploadEvidenceFile + ".");

            if (File.Exists(Full(HealthEvidenceFile)))
            {
                c.HealthEvidence = JsonUtility.FromJson<ClosedTestHealthEvidence>(File.ReadAllText(Full(HealthEvidenceFile)));
                c.HealthValidation = ClosedTestEvidenceValidator.ValidateHealth(c.HealthEvidence, expectedArtifactSha);
                if (!c.HealthValidation.Valid) issues.AddRange(c.HealthValidation.Issues);
            }
            else issues.Add("Missing " + HealthEvidenceFile + ".");

            Activation a = c.ActivationData;
            bool actualFreezeVerified = c.FreezeManifestVerified && a != null && a.candidateFreezeManifestRetained && a.candidateFreezeManifestVerified;
            bool actualArtifactMatch = actualFreezeVerified && a != null && a.artifactHashMatchesFreeze;
            var uploadReadiness = new ClosedTestUploadReadinessEvidence
            {
                CandidateFreezeManifestVerified = actualFreezeVerified,
                ArtifactHashMatchesFreeze = actualArtifactMatch,
                PlayAppSetupComplete = a != null && a.playAppSetupComplete,
                TestingTrackReleasePermissionApproved = a != null && a.testingTrackReleasePermissionApproved,
                ClosedTestTrackConfigured = a != null && a.closedTestTrackConfigured,
                TesterAccessConfigured = a != null && a.testerAccessConfigured,
                CountryAvailabilityReviewed = a != null && a.countryAvailabilityReviewed,
                ReleaseNotesApproved = a != null && a.releaseNotesApproved,
                NoOutstandingRelease = a != null && a.noOutstandingRelease,
                ClosedTestUploadAuthorized = a != null && a.closedTestUploadAuthorized,
                ClosedTestPublishAuthorized = a != null && a.closedTestPublishAuthorized
            };
            c.ClosedTestReadiness = ClosedTestUploadReadinessEvaluator.Evaluate(uploadReadiness);

            bool uploadValid = c.UploadValidation != null && c.UploadValidation.Valid && a != null && a.closedTestUploadEvidenceRetained;
            bool healthValid = c.HealthValidation != null && c.HealthValidation.Valid && a != null && a.closedTestHealthEvidenceRetained;
            bool available = uploadValid && c.UploadEvidence != null && c.UploadEvidence.ReleaseStatus == ClosedTestReleaseStatus.AvailableToTesters;
            var production = new ProductionRolloutReadinessEvidence
            {
                ClosedTestUploadEvidenceValid = uploadValid,
                ClosedTestHealthEvidenceValid = healthValid,
                ClosedTestReleaseAvailableToTesters = available,
                ClosedTestGoNoGoAuthorized = a != null && a.closedTestGoNoGoAuthorized,
                DeveloperAccountTypeReviewed = a != null && a.developerAccountTypeReviewed,
                PersonalAccountCreatedAfter2023November13 = a != null && a.personalAccountCreatedAfter2023November13,
                ContinuousOptedInTesterCount = c.HealthEvidence == null ? 0 : c.HealthEvidence.ContinuousOptedInTesterCount,
                ContinuousTestDays = c.HealthEvidence == null ? 0 : c.HealthEvidence.ContinuousTestDays,
                ProductionAccessGranted = a != null && a.productionAccessGranted,
                StoreReleaseChecklistApproved = a != null && a.storeReleaseChecklistApproved,
                PrivacyDataSafetyApproved = a != null && a.privacyDataSafetyApproved,
                ContentRatingApproved = a != null && a.contentRatingApproved,
                TargetAudienceApproved = a != null && a.targetAudienceApproved,
                AdsDeclarationApproved = a != null && a.adsDeclarationApproved,
                AppAccessDeclarationApproved = a != null && a.appAccessDeclarationApproved,
                RollbackPlanReviewed = a != null && a.rollbackPlanReviewed,
                MonitoringPlanReviewed = a != null && a.monitoringPlanReviewed,
                ManagedPublishingReviewed = a != null && a.managedPublishingReviewed,
                NoOutstandingRelease = a != null && a.noOutstandingRelease,
                IsFirstProductionRelease = a == null || a.isFirstProductionRelease,
                RequestedRolloutPercent = a == null ? 0 : a.requestedRolloutPercent,
                ProductionRolloutAuthorized = a != null && a.productionRolloutAuthorized,
                StorePublishingAuthorized = a != null && a.storePublishingAuthorized
            };
            c.ProductionReadiness = ProductionRolloutReadinessEvaluator.Evaluate(production);
            c.IntakeIssues = issues.ToArray();
            return c;
        }

        static string Format(EvaluationContext c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("v1.18 Step 6 — Closed Test Upload, Go/No-Go, and Rollout Readiness");
            sb.AppendLine("Candidate: " + (c.ActivationData == null ? "<activation missing>" : c.ActivationData.candidateId));
            sb.AppendLine("Freeze manifest verified: " + c.FreezeManifestVerified);
            sb.AppendLine("May prepare closed-test release: " + (c.ClosedTestReadiness != null && c.ClosedTestReadiness.MayPrepareClosedTestRelease));
            sb.AppendLine("May rollout closed-test release: " + (c.ClosedTestReadiness != null && c.ClosedTestReadiness.MayRolloutClosedTestRelease));
            sb.AppendLine("Closed-test upload evidence valid: " + (c.UploadValidation != null && c.UploadValidation.Valid));
            sb.AppendLine("Closed-test health evidence valid: " + (c.HealthValidation != null && c.HealthValidation.Valid));
            if (c.HealthEvidence != null)
                sb.AppendLine("Tester continuity: " + c.HealthEvidence.ContinuousOptedInTesterCount + " testers / " + c.HealthEvidence.ContinuousTestDays + " days");
            sb.AppendLine("May apply for production access: " + (c.ProductionReadiness != null && c.ProductionReadiness.MayApplyForProductionAccess));
            sb.AppendLine("May rollout production: " + (c.ProductionReadiness != null && c.ProductionReadiness.MayRolloutProduction));
            if (c.IntakeIssues.Length > 0) sb.AppendLine("Evidence intake issues:\n- " + string.Join("\n- ", c.IntakeIssues));
            if (c.ClosedTestReadiness != null && c.ClosedTestReadiness.RolloutBlockers.Length > 0)
                sb.AppendLine("Closed-test blockers:\n- " + string.Join("\n- ", c.ClosedTestReadiness.RolloutBlockers));
            if (c.ProductionReadiness != null && c.ProductionReadiness.ProductionAccessBlockers.Length > 0)
                sb.AppendLine("Production-access blockers:\n- " + string.Join("\n- ", c.ProductionReadiness.ProductionAccessBlockers));
            if (c.ProductionReadiness != null && c.ProductionReadiness.ProductionRolloutBlockers.Length > 0)
                sb.AppendLine("Production-rollout blockers:\n- " + string.Join("\n- ", c.ProductionReadiness.ProductionRolloutBlockers));
            sb.AppendLine("This tool performs no Play Console upload, release creation, rollout, halt, resume, or credential access.");
            return sb.ToString();
        }

        static Activation LoadActivation()
        {
            string path = Full(ActivationFile);
            return File.Exists(path) ? JsonUtility.FromJson<Activation>(File.ReadAllText(path)) : null;
        }

        static string ReadFirstHash(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            string text = File.ReadAllText(path).Trim();
            if (string.IsNullOrEmpty(text)) return string.Empty;
            int space = text.IndexOfAny(new[] { ' ', '\t', '\r', '\n' });
            return space < 0 ? text : text.Substring(0, space);
        }

        static string Sha256File(string path)
        {
            using (var sha = SHA256.Create()) return ToHex(sha.ComputeHash(File.ReadAllBytes(path)));
        }

        static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++) sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }

        static string Full(string relative)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative ?? string.Empty));
        }
    }
}
#endif
