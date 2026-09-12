#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsCycle;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsCycleRetrospectiveArchiveNextCadenceSelfTest
    {
        static int total,passed;
        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 7 Self-Tests")]
        public static void Run()
        {
            total=passed=0;string root=Directory.GetParent(Application.dataPath).FullName;
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_cycle_activation.json"));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsCycleCore"));
            string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceRunner.cs"));string joined=core+runner;
            Check("healthy retrospective",RetrospectiveEvaluator.Evaluate(R()).ReadyForArchive);
            Check("healthy archive",CycleArchiveEvaluator.Evaluate(A()).ArchiveReady);
            Check("healthy cadence",NextCadenceEvaluator.Evaluate(C()).CadenceReady);
            Check("healthy human review",LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),false,false,false,false,false,false,false).ReadyForHumanReview);
            Check("all gates close cycle",LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,true).MayCloseCycleManually);
            Check("all gates publish next",LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,true).MayPublishNextCadence);
            Check("retro critical blocker",Has(LiveOpsCycleValidation.ValidateRetrospective(RCritical()),LiveOpsCycleBlockerCodes.RetrospectiveCriticalLessonsOpen));
            Check("archive hash blocker",Has(LiveOpsCycleValidation.ValidateArchive(ABadHash()),LiveOpsCycleBlockerCodes.ArchiveHashesInvalid));
            Check("cadence conflict blocker",Has(LiveOpsCycleValidation.ValidateCadence(CConflict()),LiveOpsCycleBlockerCodes.CadenceCalendarConflict));
            Check("cadence carryover blocker",Has(LiveOpsCycleValidation.ValidateCadence(CCarryover()),LiveOpsCycleBlockerCodes.CadenceCarryoverBlockers));
            Check("analytics blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.ProductionAnalyticsDisabled));
            Check("remote blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.ProductionRemoteConfigDisabled));
            Check("retro auth blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.RetrospectiveReviewNotAuthorized));
            Check("archive auth blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.ArchiveReviewNotAuthorized));
            Check("cadence auth blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.NextCadenceReviewNotAuthorized));
            Check("closure auth blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.ManualCycleClosureNotAuthorized));
            Check("publish auth blocker",Has(Gated().ActionBlockers,LiveOpsCycleBlockerCodes.LiveOpsPublishingNotAuthorized));
            Check("buffer bounded",BufferBounded());
            Check("buffer order",BufferOrder());
            Check("buffer clear",BufferClear());
            Check("required 0",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_cycle_activation.json")));
            Check("required 1",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsCycleCore/LegendsOfRus.LiveOpsCycleCore.asmdef")));
            Check("required 2",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsCycleCore/LiveOpsCycleEvaluators.cs")));
            Check("required 3",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceRunner.cs")));
            Check("required 4",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsCycle/cycle_retrospective.template.json")));
            Check("required 5",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsCycle/cycle_archive.template.json")));
            Check("required 6",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsCycle/next_cadence_plan.template.json")));
            Check("required 7",File.Exists(Path.Combine(root,"README_V1_19_STEP07.md")));
            Check("required 8",File.Exists(Path.Combine(root,"Tools/verify_v1_19_step07.py")));
            Check("activation false productionAnalyticsProviderEnabled",activation.Contains("\"productionAnalyticsProviderEnabled\": false"));
            Check("activation false productionRemoteConfigEnabled",activation.Contains("\"productionRemoteConfigEnabled\": false"));
            Check("activation false retrospectiveReviewAuthorized",activation.Contains("\"retrospectiveReviewAuthorized\": false"));
            Check("activation false archiveReviewAuthorized",activation.Contains("\"archiveReviewAuthorized\": false"));
            Check("activation false nextCadenceReviewAuthorized",activation.Contains("\"nextCadenceReviewAuthorized\": false"));
            Check("activation false manualCycleClosureAuthorized",activation.Contains("\"manualCycleClosureAuthorized\": false"));
            Check("activation false liveOpsPublishingAuthorized",activation.Contains("\"liveOpsPublishingAuthorized\": false"));
            Check("activation false automaticContentMutationEnabled",activation.Contains("\"automaticContentMutationEnabled\": false"));
            Check("activation false automaticArchiveUploadEnabled",activation.Contains("\"automaticArchiveUploadEnabled\": false"));
            Check("activation false automaticExperimentAllocationEnabled",activation.Contains("\"automaticExperimentAllocationEnabled\": false"));
            Check("activation false automaticRewardMutationEnabled",activation.Contains("\"automaticRewardMutationEnabled\": false"));
            Check("activation false automaticEconomyMutationEnabled",activation.Contains("\"automaticEconomyMutationEnabled\": false"));
            Check("activation false automaticRemoteConfigMutationEnabled",activation.Contains("\"automaticRemoteConfigMutationEnabled\": false"));
            Check("activation false playConsoleActionEnabled",activation.Contains("\"playConsoleActionEnabled\": false"));
            Check("activation true localRetrospectiveToolingApproved",activation.Contains("\"localRetrospectiveToolingApproved\": true"));
            Check("activation true localArchiveValidationToolingApproved",activation.Contains("\"localArchiveValidationToolingApproved\": true"));
            Check("activation true localNextCadencePlanningToolingApproved",activation.Contains("\"localNextCadencePlanningToolingApproved\": true"));
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
            Check("banned Addressables.LoadAssetAsync",!joined.Contains("Addressables.LoadAssetAsync"));
            Check("banned Addressables.Release",!joined.Contains("Addressables.Release"));
            Check("engine free",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("hotfix04",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("nunit hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.19-step07-liveops-cycle-retrospective-archive-next-cadence");
            if(total!=75)throw new Exception("Step 19.7 self-test count mismatch: "+total+" expected 75");
            Debug.Log("v1.19 Step 7 self-tests passed: "+passed+"/"+total);
        }
        static CycleRetrospectiveEvidence R(){return new CycleRetrospectiveEvidence{EvidenceRetained=true,EvidenceReviewed=true,RetrospectiveId="LOR-RETRO-1",CycleId="LOR-CYCLE-1",OutcomeId="LOR-LO-OUT-1",ExperimentDecisionId="LOR-EXP-DEC-1",RotationCloseoutId="LOR-ROT-CLOSE-1",ParticipantsReviewed=500,ObjectivesReviewed=true,SuccessMetricsReviewed=true,IncidentLessonsReviewed=true,ContentLessonsReviewed=true,EconomyLessonsReviewed=true,SupportLessonsReviewed=true,PrivacyLessonsReviewed=true,ActionItemCount=3,ActionOwnersAssigned=true,ActionDueDatesAssigned=true,ArchiveInputsPinned=true};}
        static CycleArchiveEvidence A(){return new CycleArchiveEvidence{EvidenceRetained=true,EvidenceReviewed=true,ArchiveId="LOR-ARCHIVE-1",CycleId="LOR-CYCLE-1",SourceTreeSha256=Sha(),OutcomeSha256=Sha(),ExperimentDecisionSha256=Sha(),RotationCloseoutSha256=Sha(),RetrospectiveSha256=Sha(),ArchiveManifestSha256=Sha(),ImmutableStorageReviewed=true,PiiReviewPassed=true,SecretsReviewPassed=true,ArchiveManifestPinned=true};}
        static NextCadencePlanEvidence C(){return new NextCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-CADENCE-1",PriorCycleId="LOR-CYCLE-1",LeadTimeMinutes=2880,PlannedEvents=4,PlannedExperiments=1,MaxConcurrentEvents=2,MinimumCooldownMinutes=240,DependenciesReviewed=true,CapacityReviewed=true,LocalizationReviewed=true,SupportCoverageReviewed=true,ContentReady=true,MeasurementPlanReviewed=true,NoCarryoverBlockers=true};}
        static string Sha(){return new string('a',64);}static CycleRetrospectiveEvidence RCritical(){var x=R();x.UnresolvedCriticalLessons=1;return x;}static CycleArchiveEvidence ABadHash(){var x=A();x.SourceTreeSha256="bad";return x;}static NextCadencePlanEvidence CConflict(){var x=C();x.CalendarConflictCount=1;return x;}static NextCadencePlanEvidence CCarryover(){var x=C();x.NoCarryoverBlockers=false;return x;}
        static LiveOpsCycleReviewResult Gated(){return LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),false,false,false,false,false,false,false);}
        static bool BufferBounded(){var b=new LiveOpsCycleObservationBuffer(2);b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=1});b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=2});b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=3});return b.Count==2;}
        static bool BufferOrder(){var b=new LiveOpsCycleObservationBuffer(2);b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=1});b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=2});b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=3});return b.SnapshotOldestFirst()[0].WindowIndex==2&&b.SnapshotOldestFirst()[1].WindowIndex==3;}
        static bool BufferClear(){var b=new LiveOpsCycleObservationBuffer(2);b.Add(new LiveOpsCycleAggregateObservation());b.Clear();return b.Count==0;}
        static bool Has(string[]a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string n,bool c){total++;if(!c)throw new Exception("Step 19.7 self-test failed: "+n);passed++;}
    }
}
#endif
