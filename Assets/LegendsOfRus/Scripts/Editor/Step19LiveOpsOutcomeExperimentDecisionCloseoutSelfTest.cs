#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsOutcome;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsOutcomeExperimentDecisionCloseoutSelfTest
    {
        static int total,passed;
        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 6 Self-Tests")]
        public static void Run()
        {
            total=passed=0;string root=Directory.GetParent(Application.dataPath).FullName;
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_outcome_activation.json"));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore"));
            string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutRunner.cs"));
            string joined=core+runner;
            Check("schema",LiveOpsOutcomeRequirements.SchemaVersion==1);
            Check("basis",LiveOpsOutcomeRequirements.BasisPointsTotal==10000);
            Check("participants floor",LiveOpsOutcomeRequirements.MinimumOutcomeParticipants==100);
            Check("sample floor",LiveOpsOutcomeRequirements.MinimumExperimentSamplesTotal==200);
            Check("buffer default",LiveOpsOutcomeRequirements.DefaultObservationCapacity==30);
            Check("buffer max",LiveOpsOutcomeRequirements.MaxObservationCapacity==90);
            Check("outcome valid",LiveOpsOutcomeValidation.ValidateOutcome(O()).Length==0);
            Check("decision valid",LiveOpsOutcomeValidation.ValidateDecision(D()).Length==0);
            Check("closeout valid",LiveOpsOutcomeValidation.ValidateCloseout(C()).Length==0);
            Check("rate",LiveOpsOutcomeValidation.RateBasisPoints(4,5)==8000);
            Check("outcome healthy",LiveOpsOutcomeEvaluator.Evaluate(O()).OutcomeHealthy);
            Check("outcome sample",LiveOpsOutcomeEvaluator.Evaluate(O()).SampleSufficient);
            Check("outcome completion",LiveOpsOutcomeEvaluator.Evaluate(O()).CompletionRateBasisPoints==8000);
            Check("decision final",ExperimentDecisionEvaluator.Evaluate(D()).DecisionFinalized);
            Check("decision archive",ExperimentDecisionEvaluator.Evaluate(D()).ReadyForArchive);
            Check("decision recommendation",ExperimentDecisionEvaluator.Evaluate(D()).Recommendation==ExperimentDecisionRecommendation.ArchiveTreatment);
            Check("closeout complete",RotationCloseoutEvaluator.Evaluate(C()).CloseoutComplete);
            Check("overall ready",LiveOpsOutcomeReviewEvaluator.Evaluate(O(),D(),C(),false,false,false,false,false,false).ReadyForHumanReview);
            Check("overall gated",!LiveOpsOutcomeReviewEvaluator.Evaluate(O(),D(),C(),false,false,false,false,false,false).MayProceedWithManualCloseoutReview);
            Check("overall authorized",LiveOpsOutcomeReviewEvaluator.Evaluate(O(),D(),C(),true,true,true,true,true,true).MayProceedWithManualCloseoutReview);
            Check("overall recommendation",LiveOpsOutcomeReviewEvaluator.Evaluate(O(),D(),C(),true,true,true,true,true,true).Recommendation==LiveOpsOutcomeRecommendation.ReadyForManualCloseoutReview);
            Check("outcome crash blocker",Has(LiveOpsOutcomeEvaluator.Evaluate(WithCrash()).Blockers,LiveOpsOutcomeBlockerCodes.OutcomeCrashObserved));
            Check("outcome incident blocker",Has(LiveOpsOutcomeEvaluator.Evaluate(WithIncident()).Blockers,LiveOpsOutcomeBlockerCodes.OutcomeBlockingIncidentOpen));
            Check("outcome reward blocker",Has(LiveOpsOutcomeEvaluator.Evaluate(WithRewardMismatch()).Blockers,LiveOpsOutcomeBlockerCodes.OutcomeRewardReconciliationMismatch));
            Check("decision dq revisit",ExperimentDecisionEvaluator.Evaluate(WithBadData()).Recommendation==ExperimentDecisionRecommendation.RevisitDecision);
            Check("decision window continue",ExperimentDecisionEvaluator.Evaluate(WithOpenWindow()).Recommendation==ExperimentDecisionRecommendation.ContinueObservation);
            Check("closeout reward blocker",Has(RotationCloseoutEvaluator.Evaluate(WithPendingRewards()).Blockers,LiveOpsOutcomeBlockerCodes.CloseoutRewardClaimsPending));
            Check("closeout active blocker",Has(RotationCloseoutEvaluator.Evaluate(WithActiveRotation()).Blockers,LiveOpsOutcomeBlockerCodes.CloseoutEntriesRemaining));
            Check("analytics blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.ProductionAnalyticsDisabled));
            Check("remote blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.ProductionRemoteConfigDisabled));
            Check("outcome auth blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.OutcomeReviewNotAuthorized));
            Check("decision auth blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.ExperimentDecisionNotAuthorized));
            Check("closeout auth blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.RotationCloseoutNotAuthorized));
            Check("manual auth blocker",Has(Gated().ActionBlockers,LiveOpsOutcomeBlockerCodes.ManualCloseoutReviewNotAuthorized));
            Check("buffer bounded",BufferBounded());
            Check("buffer order",BufferOrder());
            Check("buffer clear",BufferClear());
            Check("required 0",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_outcome_activation.json")));
            Check("required 1",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/LegendsOfRus.LiveOpsOutcomeCore.asmdef")));
            Check("required 2",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/LiveOpsOutcomeReviewEvaluator.cs")));
            Check("required 3",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutRunner.cs")));
            Check("required 4",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsOutcome/outcome_evidence.template.json")));
            Check("required 5",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsOutcome/experiment_decision.template.json")));
            Check("required 6",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsOutcome/rotation_closeout.template.json")));
            Check("required 7",File.Exists(Path.Combine(root,"README_V1_19_STEP06.md")));
            Check("required 8",File.Exists(Path.Combine(root,"Tools/verify_v1_19_step06.py")));
            Check("activation false productionAnalyticsProviderEnabled",activation.Contains("\"productionAnalyticsProviderEnabled\": false"));
            Check("activation false productionRemoteConfigEnabled",activation.Contains("\"productionRemoteConfigEnabled\": false"));
            Check("activation false outcomeReviewAuthorized",activation.Contains("\"outcomeReviewAuthorized\": false"));
            Check("activation false experimentDecisionAuthorized",activation.Contains("\"experimentDecisionAuthorized\": false"));
            Check("activation false rotationCloseoutAuthorized",activation.Contains("\"rotationCloseoutAuthorized\": false"));
            Check("activation false manualCloseoutReviewAuthorized",activation.Contains("\"manualCloseoutReviewAuthorized\": false"));
            Check("activation false automaticContentMutationEnabled",activation.Contains("\"automaticContentMutationEnabled\": false"));
            Check("activation false automaticExperimentDecisionApplyEnabled",activation.Contains("\"automaticExperimentDecisionApplyEnabled\": false"));
            Check("activation false automaticExperimentArchiveEnabled",activation.Contains("\"automaticExperimentArchiveEnabled\": false"));
            Check("activation false automaticRewardMutationEnabled",activation.Contains("\"automaticRewardMutationEnabled\": false"));
            Check("activation false automaticEconomyMutationEnabled",activation.Contains("\"automaticEconomyMutationEnabled\": false"));
            Check("activation false automaticRemoteConfigMutationEnabled",activation.Contains("\"automaticRemoteConfigMutationEnabled\": false"));
            Check("activation false playConsoleActionEnabled",activation.Contains("\"playConsoleActionEnabled\": false"));
            Check("activation true localOutcomeReviewToolingApproved",activation.Contains("\"localOutcomeReviewToolingApproved\": true"));
            Check("activation true localExperimentDecisionToolingApproved",activation.Contains("\"localExperimentDecisionToolingApproved\": true"));
            Check("activation true localRotationCloseoutToolingApproved",activation.Contains("\"localRotationCloseoutToolingApproved\": true"));
            Check("activation true localSanitizedSnapshotToolingApproved",activation.Contains("\"localSanitizedSnapshotToolingApproved\": true"));
            Check("banned BuildPipeline.BuildPlayer",!joined.Contains("BuildPipeline.BuildPlayer"));
            Check("banned UnityWebRequest",!joined.Contains("UnityWebRequest"));
            Check("banned HttpClient",!joined.Contains("HttpClient"));
            Check("banned Google.Apis",!joined.Contains("Google.Apis"));
            Check("banned keystorePass",!joined.Contains("keystorePass"));
            Check("banned keyaliasPass",!joined.Contains("keyaliasPass"));
            Check("banned serviceAccountKey",!joined.Contains("serviceAccountKey"));
            Check("banned deviceUniqueIdentifier",!joined.Contains("deviceUniqueIdentifier"));
            Check("banned advertisingIdentifier",!joined.Contains("advertisingIdentifier"));
            Check("banned SystemInfo.deviceModel",!joined.Contains("SystemInfo.deviceModel"));
            Check("banned PlayerPrefs.Set",!joined.Contains("PlayerPrefs.Set"));
            Check("banned RemoteConfigService.Fetch",!joined.Contains("RemoteConfigService.Fetch"));
            Check("banned AnalyticsService.CustomData",!joined.Contains("AnalyticsService.CustomData"));
            Check("banned RuntimeEconomyTransactionEngine",!joined.Contains("RuntimeEconomyTransactionEngine"));
            Check("banned EconomyLedger",!joined.Contains("EconomyLedger"));
            Check("banned GrantCurrency",!joined.Contains("GrantCurrency"));
            Check("banned GrantReward",!joined.Contains("GrantReward"));
            Check("banned SetBalance",!joined.Contains("SetBalance"));
            Check("banned PlayerSaveData",!joined.Contains("PlayerSaveData"));
            Check("engine free",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("hotfix04",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("nunit hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.19-step06-liveops-outcome-experiment-decision-rotation-closeout");
            if(total!=89)throw new Exception("Step 19.6 self-test count mismatch: "+total+" expected 89");
            Debug.Log("v1.19 Step 6 self-tests passed: "+passed+"/"+total);
        }
        static LiveOpsOutcomeEvidence O(){return new LiveOpsOutcomeEvidence{EvidenceRetained=true,EvidenceReviewed=true,OutcomeId="LOR-LO-OUT-1",ExecutionId="LOR-LO-EXEC-1",WindowStartMinute=0,WindowEndMinute=600,ActiveParticipants=500,ContentEntries=1000,ContentCompletions=800,RewardClaimsExpected=700,RewardClaimsGranted=700,ContentHealthReviewed=true,EconomyImpactReviewed=true,PrivacyImpactReviewed=true};}
        static ExperimentDecisionEvidence D(){return new ExperimentDecisionEvidence{EvidenceRetained=true,EvidenceReviewed=true,DecisionId="LOR-EXP-DEC-1",ObservationId="LOR-EXP-OBS-1",ExperimentId="LOR-EXP-1",Decision=ExperimentDecisionKind.AdoptTreatment,TotalSamples=1000,PrimaryMetricDeltaBasisPoints=500,DataQualityPassed=true,SampleRatioHealthy=true,ExperimentWindowComplete=true,StoppingRuleReviewed=true,DecisionBasisReviewed=true};}
        static RotationCloseoutEvidence C(){return new RotationCloseoutEvidence{EvidenceRetained=true,EvidenceReviewed=true,CloseoutId="LOR-ROT-CLOSE-1",RotationPlanId="LOR-LO-ROT-PLAN-1",PlannedEntries=2,ClosedEntries=2,CooldownsSatisfied=true,ContentVersionsArchived=true,SupportHandoffComplete=true,CloseoutWindowComplete=true};}
        static LiveOpsOutcomeEvidence WithCrash(){var x=O();x.ContentCrashes=1;return x;}static LiveOpsOutcomeEvidence WithIncident(){var x=O();x.OpenBlockingIncidents=1;return x;}static LiveOpsOutcomeEvidence WithRewardMismatch(){var x=O();x.RewardClaimsGranted--;return x;}
        static ExperimentDecisionEvidence WithBadData(){var x=D();x.DataQualityPassed=false;return x;}static ExperimentDecisionEvidence WithOpenWindow(){var x=D();x.ExperimentWindowComplete=false;return x;}
        static RotationCloseoutEvidence WithPendingRewards(){var x=C();x.PendingRewardClaims=1;return x;}static RotationCloseoutEvidence WithActiveRotation(){var x=C();x.ActiveEntriesRemaining=1;return x;}
        static LiveOpsOutcomeReviewResult Gated(){return LiveOpsOutcomeReviewEvaluator.Evaluate(O(),D(),C(),false,false,false,false,false,false);}
        static bool BufferBounded(){var b=new LiveOpsOutcomeObservationBuffer(2);b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=1});b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=2});b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=3});return b.Count==2;}
        static bool BufferOrder(){var b=new LiveOpsOutcomeObservationBuffer(2);b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=1});b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=2});b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=3});return b.SnapshotOldestFirst()[0].WindowIndex==2&&b.SnapshotOldestFirst()[1].WindowIndex==3;}
        static bool BufferClear(){var b=new LiveOpsOutcomeObservationBuffer(2);b.Add(new LiveOpsOutcomeAggregateObservation());b.Clear();return b.Count==0;}
        static bool Has(string[]a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string n,bool c){total++;if(!c)throw new Exception("Step 19.6 self-test failed: "+n);passed++;}
    }
}
#endif
