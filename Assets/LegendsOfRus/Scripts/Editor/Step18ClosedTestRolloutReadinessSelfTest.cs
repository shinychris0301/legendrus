#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using LegendsOfRus.ClosedTestRollout;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ClosedTestRolloutReadinessSelfTest
    {
        static int passed;
        static int total;

        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 6 Self-Tests")]
        public static void Run()
        {
            passed = 0;
            total = 0;
            string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string core = Path.Combine(root, "Assets/LegendsOfRus/Scripts/ClosedTestRolloutCore");
            string cfg = Path.Combine(root, "Assets/LegendsOfRus/Resources/Config/closed_test_rollout_activation.json");
            string evid = Path.Combine(root, "ProjectDocs/ClosedTestEvidence");

            Check("schema version pinned", ClosedTestRolloutRequirements.SchemaVersion == 1);
            Check("personal account tester requirement pinned", ClosedTestRolloutRequirements.PersonalAccountProductionAccessTesterCount == 12);
            Check("personal account duration requirement pinned", ClosedTestRolloutRequirements.PersonalAccountProductionAccessContinuousDays == 14);
            Check("first production release is full rollout", ClosedTestRolloutRequirements.FirstProductionReleasePercent == 100);
            Check("hash algorithm pinned", ClosedTestRolloutRequirements.HashAlgorithm == "SHA-256");
            Check("candidate prefix pinned", ClosedTestRolloutRequirements.CandidatePrefix == "v1.18-step05-rc");
            Check("core asmdef exists", File.Exists(Path.Combine(core, "LegendsOfRus.ClosedTestRolloutCore.asmdef")));
            Check("core assembly is engine-free", File.ReadAllText(Path.Combine(core, "LegendsOfRus.ClosedTestRolloutCore.asmdef")).Contains("\"noEngineReferences\": true"));
            Check("activation config exists", File.Exists(cfg));
            string activation = File.Exists(cfg) ? File.ReadAllText(cfg) : string.Empty;
            Check("local closed-test readiness tooling approved", activation.Contains("\"localClosedTestReadinessToolingApproved\": true"));
            Check("local go-no-go tooling approved", activation.Contains("\"localGoNoGoToolingApproved\": true"));
            Check("local rollout readiness tooling approved", activation.Contains("\"localRolloutReadinessToolingApproved\": true"));
            Check("candidate freeze evidence remains unretained", activation.Contains("\"candidateFreezeManifestRetained\": false"));
            Check("closed-test upload remains unauthorized", activation.Contains("\"closedTestUploadAuthorized\": false"));
            Check("closed-test publishing remains unauthorized", activation.Contains("\"closedTestPublishAuthorized\": false"));
            Check("closed-test health evidence remains unretained", activation.Contains("\"closedTestHealthEvidenceRetained\": false"));
            Check("go-no-go remains unauthorized", activation.Contains("\"closedTestGoNoGoAuthorized\": false"));
            Check("developer account type remains unreviewed", activation.Contains("\"developerAccountTypeReviewed\": false"));
            Check("production access remains ungranted", activation.Contains("\"productionAccessGranted\": false"));
            Check("production rollout remains unauthorized", activation.Contains("\"productionRolloutAuthorized\": false"));
            Check("store publishing remains unauthorized", activation.Contains("\"storePublishingAuthorized\": false"));
            Check("upload evidence template exists", File.Exists(Path.Combine(evid, "closed_test_upload_evidence.template.json")));
            Check("health evidence template exists", File.Exists(Path.Combine(evid, "closed_test_health_evidence.template.json")));
            Check("production rollout template exists", File.Exists(Path.Combine(evid, "production_rollout_readiness.template.json")));
            Check("actual upload evidence absent", !File.Exists(Path.Combine(evid, "closed_test_upload_evidence.json")));
            Check("actual health evidence absent", !File.Exists(Path.Combine(evid, "closed_test_health_evidence.json")));
            Check("rollout snapshot absent in source", !File.Exists(Path.Combine(evid, "STEP06_ROLLOUT_READINESS_SNAPSHOT.json")));
            Check("signed AAB absent in source", !Directory.Exists(Path.Combine(evid, "Artifacts")) || Directory.GetFiles(Path.Combine(evid, "Artifacts"), "*.aab").Length == 0);
            Check("freeze manifest absent in source", !File.Exists(Path.Combine(evid, "CANDIDATE_FREEZE_MANIFEST.json")));

            var upload = ReadyUpload(); upload.CandidateFreezeManifestVerified = false;
            Check("closed test preparation blocks missing freeze", !ClosedTestUploadReadinessEvaluator.Evaluate(upload).MayPrepareClosedTestRelease);
            upload = ReadyUpload(); upload.ClosedTestUploadAuthorized = false; upload.ClosedTestPublishAuthorized = false;
            Check("closed test preparation can pass before authorization", ClosedTestUploadReadinessEvaluator.Evaluate(upload).MayPrepareClosedTestRelease);
            upload = ReadyUpload(); upload.ClosedTestPublishAuthorized = false;
            Check("closed test rollout requires separate publishing authorization", !ClosedTestUploadReadinessEvaluator.Evaluate(upload).MayRolloutClosedTestRelease);

            var uploadEvidence = ValidUploadEvidence();
            Check("valid closed-test upload evidence accepted", ClosedTestEvidenceValidator.ValidateUpload(uploadEvidence, HashA).Valid);
            uploadEvidence.ArtifactSha256 = HashB;
            Check("upload artifact hash mismatch rejected", !ClosedTestEvidenceValidator.ValidateUpload(uploadEvidence, HashA).Valid);
            var health = ValidHealth();
            Check("healthy closed-test evidence accepted", ClosedTestEvidenceValidator.ValidateHealth(health, HashA).Valid);
            health.BlockingCrashCount = 1;
            Check("blocking crash rejects health evidence", !ClosedTestEvidenceValidator.ValidateHealth(health, HashA).Valid);

            var production = ReadyProduction(); production.DeveloperAccountTypeReviewed = false;
            Check("production access requires account type review", !ProductionRolloutReadinessEvaluator.Evaluate(production).MayApplyForProductionAccess);
            production = ReadyProduction(); production.PersonalAccountCreatedAfter2023November13 = true; production.ContinuousOptedInTesterCount = 11;
            Check("new personal account requires twelve continuous testers", !ProductionRolloutReadinessEvaluator.Evaluate(production).MayApplyForProductionAccess);
            production = ReadyProduction(); production.PersonalAccountCreatedAfter2023November13 = true; production.ContinuousTestDays = 13;
            Check("new personal account requires fourteen continuous days", !ProductionRolloutReadinessEvaluator.Evaluate(production).MayApplyForProductionAccess);
            production = ReadyProduction(); production.IsFirstProductionRelease = true; production.RequestedRolloutPercent = 10;
            Check("first production release rejects staged rollout", !ProductionRolloutReadinessEvaluator.Evaluate(production).MayRolloutProduction);
            production = ReadyProduction(); production.IsFirstProductionRelease = false; production.RequestedRolloutPercent = 10;
            Check("update release can use staged rollout", ProductionRolloutReadinessEvaluator.Evaluate(production).UsesStagedRollout);

            string allSource = string.Join("\n", Directory.GetFiles(core, "*.cs").Select(File.ReadAllText)) + "\n" + File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Editor/Step18ClosedTestRolloutReadinessRunner.cs"));
            Check("Step 6 contains no build network credential or identifier API", !ContainsForbidden(allSource));

            if (passed != total) throw new Exception("v1.18 Step 6 self-tests failed: " + passed + "/" + total);
            Debug.Log("v1.18 Step 6 self-tests passed: " + passed + "/" + total);
        }

        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        static ClosedTestUploadReadinessEvidence ReadyUpload()
        {
            return new ClosedTestUploadReadinessEvidence
            {
                CandidateFreezeManifestVerified = true, ArtifactHashMatchesFreeze = true, PlayAppSetupComplete = true,
                TestingTrackReleasePermissionApproved = true, ClosedTestTrackConfigured = true, TesterAccessConfigured = true,
                CountryAvailabilityReviewed = true, ReleaseNotesApproved = true, NoOutstandingRelease = true,
                ClosedTestUploadAuthorized = true, ClosedTestPublishAuthorized = true
            };
        }

        static ClosedTestUploadEvidence ValidUploadEvidence()
        {
            return new ClosedTestUploadEvidence
            {
                CandidateId = "v1.18-step05-rc01", FreezeManifestSha256 = HashB, ArtifactSha256 = HashA,
                VersionName = "1.18.0", VersionCode = 11806, ReleaseStatus = ClosedTestReleaseStatus.AvailableToTesters,
                ArtifactAcceptedByPlay = true, ClosedTestTrackConfigured = true, TesterAccessConfigured = true,
                CountryAvailabilityReviewed = true, ReleaseNotesRetained = true
            };
        }

        static ClosedTestHealthEvidence ValidHealth()
        {
            return new ClosedTestHealthEvidence
            {
                CandidateId = "v1.18-step05-rc01", ArtifactSha256 = HashA, ObservationDays = 14, OptedInTesterCount = 12,
                ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14, BlockingCrashCount = 0, BlockingAnrCount = 0,
                OpenCriticalBugCount = 0, OpenBlockingFeedbackCount = 0, FeedbackReviewed = true, CoreLoopFeedbackReviewed = true
            };
        }

        static ProductionRolloutReadinessEvidence ReadyProduction()
        {
            return new ProductionRolloutReadinessEvidence
            {
                ClosedTestUploadEvidenceValid = true, ClosedTestHealthEvidenceValid = true, ClosedTestReleaseAvailableToTesters = true,
                ClosedTestGoNoGoAuthorized = true, DeveloperAccountTypeReviewed = true, PersonalAccountCreatedAfter2023November13 = false,
                ContinuousOptedInTesterCount = 12, ContinuousTestDays = 14, ProductionAccessGranted = true,
                StoreReleaseChecklistApproved = true, PrivacyDataSafetyApproved = true, ContentRatingApproved = true,
                TargetAudienceApproved = true, AdsDeclarationApproved = true, AppAccessDeclarationApproved = true,
                RollbackPlanReviewed = true, MonitoringPlanReviewed = true, ManagedPublishingReviewed = true, NoOutstandingRelease = true,
                IsFirstProductionRelease = true, RequestedRolloutPercent = 100, ProductionRolloutAuthorized = true, StorePublishingAuthorized = true
            };
        }

        static bool ContainsForbidden(string text)
        {
            string[] forbidden = { "BuildPipeline.BuildPlayer", "UnityWebRequest", "HttpClient", "keystorePass", "keyaliasPass", "deviceUniqueIdentifier", "advertisingIdentifier", "SystemInfo.deviceModel" };
            for (int i = 0; i < forbidden.Length; i++) if (text.Contains(forbidden[i])) return true;
            return false;
        }

        static void Check(string name, bool condition)
        {
            total++;
            if (!condition) throw new Exception("Step 6 self-test failed: " + name);
            passed++;
        }
    }
}
#endif
