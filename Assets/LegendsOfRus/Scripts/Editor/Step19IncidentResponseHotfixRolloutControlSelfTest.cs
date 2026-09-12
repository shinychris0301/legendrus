#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using LegendsOfRus.IncidentResponse;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19IncidentResponseHotfixRolloutControlSelfTest
    {
        static int total;
        static int passed;
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string HashB = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 2 Self-Tests")]
        public static void Run()
        {
            total = 0; passed = 0;
            string root = Directory.GetParent(Application.dataPath).FullName;
            string core = Path.Combine(root, "Assets/LegendsOfRus/Scripts/IncidentResponseCore");
            string evidence = Path.Combine(root, "ProjectDocs/IncidentResponse");
            string activationPath = Path.Combine(root, "Assets/LegendsOfRus/Resources/Config/incident_response_activation.json");
            string activation = File.ReadAllText(activationPath);

            Check("IncidentResponse core asmdef exists", File.Exists(Path.Combine(core, "LegendsOfRus.IncidentResponseCore.asmdef")));
            Check("IncidentResponse requirements exist", File.Exists(Path.Combine(core, "IncidentResponseRequirements.cs")));
            Check("IncidentResponse models exist", File.Exists(Path.Combine(core, "IncidentResponseModels.cs")));
            Check("IncidentResponse validation exists", File.Exists(Path.Combine(core, "IncidentResponseValidation.cs")));
            Check("Incident triage evaluator exists", File.Exists(Path.Combine(core, "IncidentTriageEvaluator.cs")));
            Check("Hotfix qualification evaluator exists", File.Exists(Path.Combine(core, "HotfixQualificationEvaluator.cs")));
            Check("Rollout control evaluator exists", File.Exists(Path.Combine(core, "RolloutControlReviewEvaluator.cs")));
            Check("incident register template exists", File.Exists(Path.Combine(evidence, "incident_register.template.json")));
            Check("hotfix template exists", File.Exists(Path.Combine(evidence, "hotfix_candidate_evidence.template.json")));
            Check("rollout template exists", File.Exists(Path.Combine(evidence, "rollout_control_review.template.json")));
            Check("evidence readme exists", File.Exists(Path.Combine(evidence, "README.md")));
            Check("activation config exists", File.Exists(activationPath));
            Check("local incident tooling approved", activation.Contains("\"localIncidentReviewToolingApproved\": true"));
            Check("local hotfix tooling approved", activation.Contains("\"localHotfixQualificationToolingApproved\": true"));
            Check("local rollout tooling approved", activation.Contains("\"localRolloutControlReviewToolingApproved\": true"));
            Check("manual hotfix release remains false", activation.Contains("\"manualHotfixReleaseAuthorized\": false"));
            Check("manual rollout action remains false", activation.Contains("\"manualRolloutActionAuthorized\": false"));
            Check("automatic Play actions remain false", activation.Contains("\"automaticPlayActionsEnabled\": false"));
            Check("automatic build remains false", activation.Contains("\"automaticBuildEnabled\": false"));
            Check("automatic signing remains false", activation.Contains("\"automaticSigningEnabled\": false"));
            Check("Play publishing API remains false", activation.Contains("\"playPublishingApiEnabled\": false"));
            Check("Play credential access remains false", activation.Contains("\"playConsoleCredentialAccessEnabled\": false"));

            Check("SHA256 valid lower", IncidentResponseValidation.IsSha256(HashA));
            Check("SHA256 invalid short", !IncidentResponseValidation.IsSha256("abc"));
            Check("incident id canonical valid", IncidentResponseValidation.IsIncidentId("LOR-INC-0001"));
            Check("incident id zero invalid", !IncidentResponseValidation.IsIncidentId("LOR-INC-0000"));
            Check("hotfix id canonical valid", IncidentResponseValidation.IsHotfixId("LOR-HF-0001"));
            Check("hotfix candidate canonical valid", IncidentResponseValidation.IsHotfixCandidateId("v1.19-hotfix-rc01"));
            Check("hotfix candidate zero invalid", !IncidentResponseValidation.IsHotfixCandidateId("v1.19-hotfix-rc00"));
            Check("base candidate canonical valid", IncidentResponseValidation.IsBaseCandidateId("v1.18-step05-rc01"));
            Check("base candidate rejects spaces", !IncidentResponseValidation.IsBaseCandidateId("bad candidate"));
            Check("rollout one valid", IncidentResponseValidation.IsRolloutPercent(1));
            Check("rollout hundred valid", IncidentResponseValidation.IsRolloutPercent(100));
            Check("rollout zero invalid", !IncidentResponseValidation.IsRolloutPercent(0));
            Check("rollout 101 invalid", !IncidentResponseValidation.IsRolloutPercent(101));

            Check("missing register blocks health", !IncidentTriageEvaluator.Evaluate(null).ProductionHealthClear);
            Check("clean register health clear", IncidentTriageEvaluator.Evaluate(CleanRegister()).ProductionHealthClear);
            var critical = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Critical)));
            Check("critical counted", critical.OpenCritical == 1);
            Check("critical escalates rollout review", critical.Recommendation == IncidentResponseRecommendation.EscalateRolloutControlReview);
            var high = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.High)));
            Check("high counted", high.OpenHigh == 1);
            Check("high requests hotfix review", high.Recommendation == IncidentResponseRecommendation.HotfixQualificationReview);
            var medium = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Medium)));
            Check("medium requests hold review", medium.Recommendation == IncidentResponseRecommendation.HoldRolloutReview);
            var low = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Low)));
            Check("low does not block health", low.ProductionHealthClear);
            var dataLossIncident = Incident("LOR-INC-0001", IncidentSeverity.Low); dataLossIncident.UserDataLossRisk = true;
            Check("data loss risk blocks", !IncidentTriageEvaluator.Evaluate(CleanRegister(dataLossIncident)).ProductionHealthClear);
            var purchaseIncident = Incident("LOR-INC-0001", IncidentSeverity.Low); purchaseIncident.PurchaseIntegrityRisk = true;
            Check("purchase integrity risk blocks", !IncidentTriageEvaluator.Evaluate(CleanRegister(purchaseIncident)).ProductionHealthClear);
            var pending = Incident("LOR-INC-0001", IncidentSeverity.Medium); pending.State = IncidentLifecycleState.FixedPendingRetest; pending.RetestRequired = true;
            Check("pending retest counted", IncidentTriageEvaluator.Evaluate(CleanRegister(pending)).PendingRetest == 1);
            pending.RetestPassed = true;
            Check("passed retest clears pending", IncidentTriageEvaluator.Evaluate(CleanRegister(pending)).PendingRetest == 0);
            var dup = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Low), Incident("LOR-INC-0001", IncidentSeverity.Low)));
            Check("duplicate id structurally invalid", !dup.RegisterStructurallyValid);

            var register = CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.High));
            var hotfix = ReadyHotfix();
            var hotfixResult = HotfixQualificationEvaluator.Evaluate(hotfix, register);
            Check("complete hotfix evidence qualifies", hotfixResult.QualifiedForManualReleasePreparation);
            hotfix = ReadyHotfix(); hotfix.ManualHotfixReleaseAuthorized = false;
            hotfixResult = HotfixQualificationEvaluator.Evaluate(hotfix, register);
            Check("hotfix review can pass without release auth", hotfixResult.ReadyForHumanHotfixReview);
            Check("hotfix release auth remains separate", !hotfixResult.MayProceedWithAuthorizedManualHotfixRelease);
            hotfix = ReadyHotfix(); hotfix.HotfixArtifactSha256 = HashA;
            Check("unchanged hotfix artifact rejected", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.HotfixArtifactUnchanged));
            hotfix = ReadyHotfix(); hotfix.LinkedIncidentIds = new string[0];
            Check("hotfix needs linked incident", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.LinkedIncidentMissing));
            hotfix = ReadyHotfix(); hotfix.RootCauseReviewed = false;
            Check("hotfix needs root cause review", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.RootCauseNotReviewed));
            hotfix = ReadyHotfix(); hotfix.ChangedScopeUnits = IncidentResponseRequirements.MaximumHotfixScopeUnits + 1;
            Check("oversized hotfix scope rejected", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.ScopeTooLarge));
            hotfix = ReadyHotfix(); hotfix.UnityCompilePassed = false;
            Check("hotfix requires Unity compile", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.CompileNotVerified));
            hotfix = ReadyHotfix(); hotfix.EditModePassed = false;
            Check("hotfix requires EditMode", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.EditModeNotVerified));
            hotfix = ReadyHotfix(); hotfix.PlayModePassed = false;
            Check("hotfix requires PlayMode", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.PlayModeNotVerified));
            hotfix = ReadyHotfix(); hotfix.DeviceSmokePassed = false;
            Check("hotfix requires device smoke", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.DeviceSmokeNotVerified));
            hotfix = ReadyHotfix(); hotfix.SaveMigrationRiskReviewed = false;
            Check("hotfix requires save risk review", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.SaveRiskNotReviewed));
            hotfix = ReadyHotfix(); hotfix.EconomyRiskReviewed = false;
            Check("hotfix requires economy risk review", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.EconomyRiskNotReviewed));
            hotfix = ReadyHotfix(); hotfix.PrivacyRiskReviewed = false;
            Check("hotfix requires privacy risk review", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.PrivacyRiskNotReviewed));
            hotfix = ReadyHotfix(); hotfix.AabEvidenceRetained = false;
            Check("hotfix requires AAB evidence", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.AabEvidenceMissing));
            hotfix = ReadyHotfix(); hotfix.SigningEvidenceReviewed = false;
            Check("hotfix requires signing review", HotfixQualificationEvaluator.Evaluate(hotfix, register).ReviewBlockers.Contains(IncidentResponseBlockerCodes.SigningEvidenceNotReviewed));

            var rollout = ReadyRollout(); rollout.CurrentState = RolloutTrackState.StagedActive; rollout.CurrentRolloutPercent = 20; rollout.RequestedAction = RolloutControlAction.HaltCurrentRelease;
            Check("staged rollout halt valid", RolloutControlReviewEvaluator.Evaluate(rollout).MayApplyRequestedActionManually);
            rollout = ReadyRollout(); rollout.FirstReleaseOnTrack = true; rollout.CurrentState = RolloutTrackState.FullyRolledOut; rollout.CurrentRolloutPercent = 100; rollout.RequestedAction = RolloutControlAction.HaltCurrentRelease; rollout.PreviousEligibleReleaseAvailable = false;
            Check("first full release halt blocked", RolloutControlReviewEvaluator.Evaluate(rollout).ActionBlockers.Contains(IncidentResponseBlockerCodes.FirstReleaseCannotHaltFull));
            rollout = ReadyRollout(); rollout.FirstReleaseOnTrack = false; rollout.CurrentState = RolloutTrackState.FullyRolledOut; rollout.CurrentRolloutPercent = 100; rollout.RequestedAction = RolloutControlAction.HaltCurrentRelease; rollout.PreviousEligibleReleaseAvailable = false;
            Check("full update halt needs previous release", RolloutControlReviewEvaluator.Evaluate(rollout).ActionBlockers.Contains(IncidentResponseBlockerCodes.PreviousReleaseUnavailable));
            rollout.PreviousEligibleReleaseAvailable = true;
            Check("full update halt allows eligible previous", RolloutControlReviewEvaluator.Evaluate(rollout).MayApplyRequestedActionManually);
            rollout = ReadyRollout(); rollout.CurrentState = RolloutTrackState.StagedHalted; rollout.RequestedAction = RolloutControlAction.ResumeCurrentRelease; rollout.RequestedRolloutPercent = 25;
            Check("healthy halted rollout can resume", RolloutControlReviewEvaluator.Evaluate(rollout).MayApplyRequestedActionManually);
            rollout.CurrentArtifactHasBlockingIssue = true; rollout.IncidentHealthClear = false;
            Check("resume blocks on unresolved issue", RolloutControlReviewEvaluator.Evaluate(rollout).ActionBlockers.Contains(IncidentResponseBlockerCodes.BlockingIssueNotCleared));
            rollout = ReadyRollout(); rollout.CurrentState = RolloutTrackState.StagedActive; rollout.CurrentRolloutPercent = 25; rollout.RequestedAction = RolloutControlAction.IncreaseRollout; rollout.RequestedRolloutPercent = 50;
            Check("healthy staged increase valid", RolloutControlReviewEvaluator.Evaluate(rollout).MayApplyRequestedActionManually);
            rollout.RequestedRolloutPercent = 25;
            Check("rollout increase must increase", RolloutControlReviewEvaluator.Evaluate(rollout).ActionBlockers.Contains(IncidentResponseBlockerCodes.RequestedPercentInvalid));
            rollout = ReadyRollout(); rollout.CurrentArtifactHasBlockingIssue = true; rollout.QualifiedHotfixAvailable = true; rollout.RequestedAction = RolloutControlAction.ReplaceWithQualifiedHotfix;
            Check("qualified hotfix replacement valid", RolloutControlReviewEvaluator.Evaluate(rollout).MayApplyRequestedActionManually);
            rollout.QualifiedHotfixAvailable = false;
            Check("unqualified hotfix replacement blocked", RolloutControlReviewEvaluator.Evaluate(rollout).ActionBlockers.Contains(IncidentResponseBlockerCodes.HotfixNotQualified));
            rollout = ReadyRollout(); rollout.CurrentState = RolloutTrackState.StagedActive; rollout.CurrentRolloutPercent = 20; rollout.RequestedAction = RolloutControlAction.HaltCurrentRelease; rollout.ManualRolloutActionAuthorized = false;
            var rolloutResult = RolloutControlReviewEvaluator.Evaluate(rollout);
            Check("rollout action validity separate from authorization", rolloutResult.RequestedActionValid);
            Check("rollout manual authorization required", !rolloutResult.MayApplyRequestedActionManually);
            rollout = ReadyRollout(); rollout.AndroidVitalsReviewed = false;
            Check("rollout needs Android vitals review", RolloutControlReviewEvaluator.Evaluate(rollout).ReviewBlockers.Contains(IncidentResponseBlockerCodes.AndroidVitalsNotReviewed));

            string source = string.Join("\n", Directory.GetFiles(core, "*.cs").Select(File.ReadAllText)) + "\n" +
                            File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Editor/Step19IncidentResponseHotfixRolloutControlRunner.cs"));
            Check("Step2 contains no automatic build", !source.Contains("BuildPipeline.BuildPlayer"));
            Check("Step2 contains no UnityWebRequest", !source.Contains("UnityWebRequest"));
            Check("Step2 contains no HttpClient", !source.Contains("HttpClient"));
            Check("Step2 contains no Google APIs", !source.Contains("Google.Apis"));
            Check("Step2 contains no Play reporting SDK", !source.Contains("PlayDeveloperReporting"));
            Check("Step2 contains no signing credential fields", !ContainsAny(source, new[] { "keystorePass", "keyaliasPass", "keyStorePass", "keyAliasPass" }));
            Check("Step2 contains no Play credential fields", !ContainsAny(source, new[] { "serviceAccountKey", "playCredential", "oauthToken", "accessToken" }));
            Check("Step2 contains no device identifiers", !ContainsAny(source, new[] { "deviceUniqueIdentifier", "advertisingIdentifier", "SystemInfo.deviceModel", "AndroidId", "IMEI" }));
            Check("Step2 contains no tester identity fields", !ContainsAny(source, new[] { "TesterEmail", "testerEmail", "TesterName", "testerName", "GoogleGroup", "googleGroup" }));
            Check("automatic Play actions constant false", !IncidentResponseRequirements.AutomaticPlayActionsAllowed);

            string manifest = File.ReadAllText(Path.Combine(root, "Packages/manifest.json"));
            Check("Addressables 2.8.1 retained", manifest.Contains("\"com.unity.addressables\": \"2.8.1\""));
            string balanceAsm = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef"));
            Check("Hotfix04 TestAssemblies retained", balanceAsm.Contains("TestAssemblies"));
            string diagnosticsTest = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs"));
            Check("NUnit type containment hotfix retained", diagnosticsTest.Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            string runtimeTests = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef"));
            Check("ContentDeliveryCore asmdef hotfix retained", runtimeTests.Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("approved PNG count remains 65", Directory.GetFiles(Path.Combine(root, "Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65);
            string gameState = File.ReadAllText(Path.Combine(root, "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs"));
            Check("save v6 boundary retained", gameState.Contains("public int saveVersion = 6;"));
            Check("Step1 launch operations core retained", Directory.Exists(Path.Combine(root, "Assets/LegendsOfRus/Scripts/LaunchOperationsCore")));
            Check("Step1 Android vitals evaluator retained", File.Exists(Path.Combine(root, "Assets/LegendsOfRus/Scripts/LaunchOperationsCore/AndroidVitalsEvaluator.cs")));

            if (passed != total) throw new Exception("v1.19 Step 2 self-tests failed: " + passed + "/" + total);
            Debug.Log("v1.19 Step 2 self-tests passed: " + passed + "/" + total);
        }

        static IncidentRecord Incident(string id, IncidentSeverity severity) => new IncidentRecord { IncidentId = id, Severity = severity, State = IncidentLifecycleState.Investigating };
        static IncidentRegisterEvidence CleanRegister(params IncidentRecord[] incidents) => new IncidentRegisterEvidence { EvidenceRetained = true, EvidenceReviewed = true, Incidents = incidents };

        static HotfixCandidateEvidence ReadyHotfix() => new HotfixCandidateEvidence
        {
            EvidenceRetained = true, HotfixId = "LOR-HF-0001", BaseCandidateId = "v1.18-step05-rc01", BaseArtifactSha256 = HashA,
            HotfixCandidateId = "v1.19-hotfix-rc01", HotfixArtifactSha256 = HashB, LinkedIncidentIds = new[] { "LOR-INC-0001" },
            RootCauseReviewed = true, ScopeReviewed = true, ChangedScopeUnits = 3, SourceChangeReviewed = true,
            UnityCompilePassed = true, EditModePassed = true, PlayModePassed = true, DeviceSmokePassed = true,
            SaveMigrationRiskReviewed = true, EconomyRiskReviewed = true, PrivacyRiskReviewed = true, StoreListingImpactReviewed = true,
            VersionCodeReviewed = true, AabEvidenceRetained = true, SigningEvidenceReviewed = true, ManualHotfixReleaseAuthorized = true
        };

        static RolloutControlReview ReadyRollout() => new RolloutControlReview
        {
            EvidenceRetained = true, EvidenceReviewed = true, FirstReleaseOnTrack = false,
            CurrentState = RolloutTrackState.StagedActive, CurrentRolloutPercent = 20, PreviousEligibleReleaseAvailable = true,
            CurrentArtifactHasBlockingIssue = false, IncidentTriageReviewed = true, IncidentHealthClear = true,
            AndroidVitalsReviewed = true, QualifiedHotfixAvailable = true, RequestedAction = RolloutControlAction.Hold,
            RequestedRolloutPercent = 20, ManualRolloutActionAuthorized = true
        };

        static bool ContainsAny(string text, string[] values)
        {
            for (int i = 0; i < values.Length; i++) if (text.Contains(values[i])) return true;
            return false;
        }

        static void Check(string name, bool condition)
        {
            total++;
            if (!condition) throw new Exception("Step 19.2 self-test failed: " + name);
            passed++;
        }
    }
}
#endif
