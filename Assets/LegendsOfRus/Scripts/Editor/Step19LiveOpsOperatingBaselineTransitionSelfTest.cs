#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.LiveOpsBaseline;

namespace LegendsOfRus.EditorTools
{
    public static class Step19LiveOpsOperatingBaselineTransitionSelfTest
    {
        static int total,passed;
        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 8 Self-Tests")]
        public static void Run()
        {
            total=passed=0;
            var c=C(); var b=B(); var t=T();
            Check("chain healthy",V119EvidenceChainEvaluator.Evaluate(c).ChainReady);
            Check("chain missing",Has(LiveOpsBaselineValidation.ValidateChain(null),LiveOpsBaselineBlockerCodes.ChainMissing));
            var c1=C();c1.Step07Reviewed=false;Check("chain step missing",Has(LiveOpsBaselineValidation.ValidateChain(c1),LiveOpsBaselineBlockerCodes.ChainStepMissing));
            var c2=C();c2.SourceTreeSha256="bad";Check("chain source hash",Has(LiveOpsBaselineValidation.ValidateChain(c2),LiveOpsBaselineBlockerCodes.ChainSourceHashInvalid));
            var c3=C();c3.ArchiveSha256="bad";Check("chain archive hash",Has(LiveOpsBaselineValidation.ValidateChain(c3),LiveOpsBaselineBlockerCodes.ChainArchiveHashInvalid));
            var c4=C();c4.UnityCompileEvidenceReviewed=false;Check("chain unity evidence",Has(LiveOpsBaselineValidation.ValidateChain(c4),LiveOpsBaselineBlockerCodes.ChainUnityEvidenceMissing));
            var c5=C();c5.PrivacyBoundaryReviewed=false;Check("chain privacy",Has(LiveOpsBaselineValidation.ValidateChain(c5),LiveOpsBaselineBlockerCodes.ChainPrivacyBoundaryNotReviewed));
            var c6=C();c6.ProductionMutationBoundaryReviewed=false;Check("chain mutation",Has(LiveOpsBaselineValidation.ValidateChain(c6),LiveOpsBaselineBlockerCodes.ChainMutationBoundaryNotReviewed));
            var c7=C();c7.UnresolvedCriticalBlockers=1;Check("chain critical",Has(LiveOpsBaselineValidation.ValidateChain(c7),LiveOpsBaselineBlockerCodes.ChainCriticalBlockersOpen));
            Check("baseline healthy",OperatingBaselineEvaluator.Evaluate(b,c.ChainId).BaselineReady);
            var b1=B();b1.RunbookSectionCount=4;Check("baseline runbook count",Has(LiveOpsBaselineValidation.ValidateBaseline(b1,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineRunbooksIncomplete));
            var b2=B();b2.ObservabilityRunbookReviewed=false;Check("baseline observability",Has(LiveOpsBaselineValidation.ValidateBaseline(b2,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineObservabilityMissing));
            var b3=B();b3.IncidentRunbookReviewed=false;Check("baseline incident",Has(LiveOpsBaselineValidation.ValidateBaseline(b3,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineIncidentMissing));
            var b4=B();b4.HotfixRunbookReviewed=false;Check("baseline hotfix",Has(LiveOpsBaselineValidation.ValidateBaseline(b4,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineHotfixMissing));
            var b5=B();b5.LiveOpsRunbookReviewed=false;Check("baseline liveops",Has(LiveOpsBaselineValidation.ValidateBaseline(b5,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineLiveOpsMissing));
            var b6=B();b6.PrivacySupportRunbookReviewed=false;Check("baseline privacy support",Has(LiveOpsBaselineValidation.ValidateBaseline(b6,c.ChainId),LiveOpsBaselineBlockerCodes.BaselinePrivacySupportMissing));
            var b7=B();b7.RollbackBoundaryReviewed=false;Check("baseline rollback",Has(LiveOpsBaselineValidation.ValidateBaseline(b7,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineRollbackMissing));
            var b8=B();b8.ArchivePinned=false;Check("baseline archive",Has(LiveOpsBaselineValidation.ValidateBaseline(b8,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineArchiveNotPinned));
            var b9=B();b9.UnresolvedCriticalActions=1;Check("baseline critical",Has(LiveOpsBaselineValidation.ValidateBaseline(b9,c.ChainId),LiveOpsBaselineBlockerCodes.BaselineCriticalActionsOpen));
            Check("transition healthy",V120TransitionEvaluator.Evaluate(t,b.BaselineId).TransitionReady);
            var t1=T();t1.LeadTimeHours=23;Check("transition lead",Has(LiveOpsBaselineValidation.ValidateTransition(t1,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionLeadTimeInsufficient));
            var t2=T();t2.ChapterPipelineEntryReviewed=false;Check("transition chapter",Has(LiveOpsBaselineValidation.ValidateTransition(t2,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionChapterPipelineMissing));
            var t3=T();t3.AddressablesContentModelReviewed=false;Check("transition addressables",Has(LiveOpsBaselineValidation.ValidateTransition(t3,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionAddressablesReviewMissing));
            var t4=T();t4.LocalizationPipelineReviewed=false;Check("transition localization",Has(LiveOpsBaselineValidation.ValidateTransition(t4,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionLocalizationMissing));
            var t5=T();t5.QaStrategyReviewed=false;Check("transition qa",Has(LiveOpsBaselineValidation.ValidateTransition(t5,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionQaMissing));
            var t6=T();t6.AutomationStrategyReviewed=false;Check("transition automation",Has(LiveOpsBaselineValidation.ValidateTransition(t6,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionAutomationMissing));
            var t7=T();t7.TeamCapacityReviewed=false;Check("transition capacity",Has(LiveOpsBaselineValidation.ValidateTransition(t7,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionCapacityMissing));
            var t8=T();t8.CarryoverBlockerCount=1;Check("transition carryover",Has(LiveOpsBaselineValidation.ValidateTransition(t8,b.BaselineId),LiveOpsBaselineBlockerCodes.TransitionCarryoverBlockers));
            var gated=LiveOpsBaselineReviewEvaluator.Evaluate(c,b,t,false,false,false,false,false,false,false,false);
            Check("gated human review structurally ready",gated.ReadyForHumanReview);
            Check("gated cannot close",!gated.MayCloseV119Manually);
            Check("gated cannot begin v120",!gated.MayBeginV120Preparation);
            var allowed=LiveOpsBaselineReviewEvaluator.Evaluate(c,b,t,true,true,true,true,true,true,true,false);
            Check("allowed can close",allowed.MayCloseV119Manually);
            Check("allowed can begin v120",allowed.MayBeginV120Preparation);
            Check("publish remains separate",Has(allowed.ActionBlockers,LiveOpsBaselineBlockerCodes.LiveOpsPublishingNotAuthorized));
            Check("buffer bounded",BufferBounded());
            Check("buffer order",BufferOrder());
            Check("buffer clear",BufferClear());

            string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_baseline_activation.json"));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsBaselineCore"));
            string joined=core+File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOperatingBaselineTransitionRunner.cs"));
            Check("required core asmdef",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsBaselineCore/LegendsOfRus.LiveOpsBaselineCore.asmdef")));
            Check("required runner",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOperatingBaselineTransitionRunner.cs")));
            Check("required chain template",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/v119_evidence_chain.template.json")));
            Check("required baseline template",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/liveops_operating_baseline.template.json")));
            Check("required transition template",File.Exists(Path.Combine(root,"ProjectDocs/LiveOpsBaseline/v120_transition_plan.template.json")));
            Check("required readme",File.Exists(Path.Combine(root,"README_V1_19_STEP08.md")));
            Check("required verifier",File.Exists(Path.Combine(root,"Tools/verify_v1_19_step08.py")));
            Check("activation prod analytics false",activation.Contains("\"productionAnalyticsProviderEnabled\": false"));
            Check("activation remote config false",activation.Contains("\"productionRemoteConfigEnabled\": false"));
            Check("activation chain review false",activation.Contains("\"evidenceChainReviewAuthorized\": false"));
            Check("activation baseline review false",activation.Contains("\"operatingBaselineReviewAuthorized\": false"));
            Check("activation transition review false",activation.Contains("\"v120TransitionReviewAuthorized\": false"));
            Check("activation v119 closure false",activation.Contains("\"manualV119ClosureAuthorized\": false"));
            Check("activation v120 prep false",activation.Contains("\"v120PreparationAuthorized\": false"));
            Check("activation publish false",activation.Contains("\"liveOpsPublishingAuthorized\": false"));
            Check("activation automatic content false",activation.Contains("\"automaticContentMutationEnabled\": false"));
            Check("activation archive upload false",activation.Contains("\"automaticArchiveUploadEnabled\": false"));
            Check("activation experiment allocation false",activation.Contains("\"automaticExperimentAllocationEnabled\": false"));
            Check("activation reward mutation false",activation.Contains("\"automaticRewardMutationEnabled\": false"));
            Check("activation economy mutation false",activation.Contains("\"automaticEconomyMutationEnabled\": false"));
            Check("activation remote mutation false",activation.Contains("\"automaticRemoteConfigMutationEnabled\": false"));
            Check("activation play action false",activation.Contains("\"playConsoleActionEnabled\": false"));
            Check("local chain tooling true",activation.Contains("\"localEvidenceChainToolingApproved\": true"));
            Check("local baseline tooling true",activation.Contains("\"localOperatingBaselineToolingApproved\": true"));
            Check("local transition tooling true",activation.Contains("\"localV120TransitionPlanningToolingApproved\": true"));
            Check("banned BuildPipeline.BuildPlayer",!joined.Contains("BuildPipeline.BuildPlayer"));
            Check("banned UnityWebRequest",!joined.Contains("UnityWebRequest"));
            Check("banned HttpClient",!joined.Contains("HttpClient"));
            Check("banned Google.Apis",!joined.Contains("Google.Apis"));
            Check("banned keystorePass",!joined.Contains("keystorePass"));
            Check("banned keyaliasPass",!joined.Contains("keyaliasPass"));
            Check("banned serviceAccountKey",!joined.Contains("serviceAccountKey"));
            Check("banned deviceUniqueIdentifier",!joined.Contains("deviceUniqueIdentifier"));
            Check("banned advertisingIdentifier",!joined.Contains("advertisingIdentifier"));
            Check("banned PlayerPrefs.Set",!joined.Contains("PlayerPrefs.Set"));
            Check("banned RuntimeEconomyTransactionEngine",!joined.Contains("RuntimeEconomyTransactionEngine"));
            Check("banned GrantCurrency",!joined.Contains("GrantCurrency"));
            Check("banned GrantReward",!joined.Contains("GrantReward"));
            Check("engine free",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables 2.8.1",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("hotfix04",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("nunit hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.19-step08-liveops-operating-baseline-v1.20-transition-review");
            if(total!=82)throw new Exception("Step 19.8 self-test count mismatch: "+total+" expected 82");
            Debug.Log("v1.19 Step 8 self-tests passed: "+passed+"/"+total);
        }
        static V119EvidenceChainEvidence C(){return new V119EvidenceChainEvidence{EvidenceRetained=true,EvidenceReviewed=true,ChainId="LOR-V119-CHAIN-1",SourceTreeSha256=Sha(),ArchiveSha256=Sha(),Step01Reviewed=true,Step02Reviewed=true,Step03Reviewed=true,Step04Reviewed=true,Step05Reviewed=true,Step06Reviewed=true,Step07Reviewed=true,UnityCompileEvidenceReviewed=true,TestRunnerEvidenceReviewed=true,PrivacyBoundaryReviewed=true,ProductionMutationBoundaryReviewed=true};}
        static LiveOpsOperatingBaselineEvidence B(){return new LiveOpsOperatingBaselineEvidence{EvidenceRetained=true,EvidenceReviewed=true,BaselineId="LOR-OPS-BASELINE-1",ChainId="LOR-V119-CHAIN-1",BaselineSha256=Sha(),RunbookSectionCount=6,ObservabilityRunbookReviewed=true,IncidentRunbookReviewed=true,HotfixRunbookReviewed=true,LiveOpsRunbookReviewed=true,PrivacySupportRunbookReviewed=true,RollbackBoundaryReviewed=true,ArchivePinned=true};}
        static V120TransitionPlanEvidence T(){return new V120TransitionPlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,TransitionId="LOR-V120-TRANSITION-1",BaselineId="LOR-OPS-BASELINE-1",TargetMilestone="v1.20",LeadTimeHours=48,TransitionScopeReviewed=true,ChapterPipelineEntryReviewed=true,ContentOwnershipReviewed=true,AddressablesContentModelReviewed=true,LocalizationPipelineReviewed=true,QaStrategyReviewed=true,AutomationStrategyReviewed=true,TeamCapacityReviewed=true};}
        static string Sha(){return new string('a',64);}
        static bool BufferBounded(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=1});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=2});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=3});return x.Count==2;}
        static bool BufferOrder(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=1});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=2});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=3});var s=x.SnapshotOldestFirst();return s.Length==2&&s[0].WindowIndex==2&&s[1].WindowIndex==3;}
        static bool BufferClear(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation());x.Clear();return x.Count==0;}
        static bool Has(string[] a,string v){if(a==null)return false;for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string name,bool ok){total++;if(!ok)throw new Exception("Step 19.8 self-test failed: "+name);passed++;}
    }
}
#endif
