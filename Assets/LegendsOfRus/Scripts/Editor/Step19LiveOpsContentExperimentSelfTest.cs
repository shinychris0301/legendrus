#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.LiveOpsReadiness;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19LiveOpsContentExperimentSelfTest
    {
        static int total,passed;
        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 4 Self-Tests")]
        public static void Run()
        {
            total=passed=0;string root=Directory.GetParent(Application.dataPath).FullName;
            var p=P();var c=C();var h=H();var e=E();
            Check("schema",LiveOpsReadinessRequirements.SchemaVersion==1);
            Check("weights",LiveOpsReadinessRequirements.WeightBasisPointsTotal==10000);
            Check("default buffer",LiveOpsReadinessRequirements.DefaultObservationCapacity==30);
            Check("max buffer",LiveOpsReadinessRequirements.MaxObservationCapacity==90);
            Check("max variants",LiveOpsReadinessRequirements.MaxExperimentVariants==4);
            Check("profile valid",LiveOpsReadinessValidation.ValidateProfile(p).Length==0);
            Check("cadence valid",LiveOpsReadinessValidation.ValidateCadence(c).Length==0);
            Check("health valid",LiveOpsReadinessValidation.ValidateContentHealth(h).Length==0);
            Check("experiment valid",LiveOpsReadinessValidation.ValidateExperiment(e).Length==0);
            Check("rate",LiveOpsReadinessValidation.RateBasisPoints(1,4)==2500);
            var cr=LiveOpsCadenceEvaluator.Evaluate(p,c);Check("cadence structural",cr.StructurallyValid);Check("cadence safe",cr.CadenceSafe);Check("cadence concurrency",cr.MaximumObservedConcurrency==1);
            var hr=ContentHealthEvaluator.Evaluate(p,h);Check("health structural",hr.StructurallyValid);Check("health sample",hr.SampleSufficient);Check("health healthy",hr.ContentHealthy);Check("health completion",hr.CompletionRateBasisPoints==6000);
            var er=ExperimentReadinessEvaluator.Evaluate(p,e,false,false,false);Check("experiment structural",er.StructurallyValid);Check("experiment design",er.DesignReady);Check("measurement off",!er.MeasurementReady);Check("launch off",!er.LaunchReady);Check("analytics blocker",Contains(er.LaunchBlockers,LiveOpsReadinessBlockerCodes.ProductionAnalyticsDisabled));Check("remote config blocker",Contains(er.LaunchBlockers,LiveOpsReadinessBlockerCodes.ProductionRemoteConfigDisabled));Check("authorization blocker",Contains(er.LaunchBlockers,LiveOpsReadinessBlockerCodes.ExperimentLaunchNotAuthorized));
            var all=LiveOpsReadinessEvaluator.Evaluate(p,c,h,e,false,false,false,false,false);Check("overall review ready",all.ReadyForHumanReview);Check("overall cadence",all.CadenceSafe);Check("overall health",all.ContentHealthy);Check("overall design",all.ExperimentDesignReady);Check("overall launch blocked",!all.ExperimentLaunchReady);Check("overall publish blocked",!all.MayProceedWithManualLiveOpsPublishReview);Check("manual review blocker",Contains(all.PublishBlockers,LiveOpsReadinessBlockerCodes.ManualLiveOpsReviewNotAuthorized));Check("default recommendation",all.Recommendation==LiveOpsReadinessRecommendation.HoldExperimentLaunch);
            var buf=new LiveOpsObservationBuffer(3);buf.Add(new LiveOpsAggregateObservation{WindowIndex=1});buf.Add(new LiveOpsAggregateObservation{WindowIndex=2});buf.Add(new LiveOpsAggregateObservation{WindowIndex=3});buf.Add(new LiveOpsAggregateObservation{WindowIndex=4});Check("buffer bounded",buf.Count==3);Check("buffer oldest",buf.SnapshotOldestFirst()[0].WindowIndex==2);Check("buffer newest",buf.SnapshotOldestFirst()[2].WindowIndex==4);buf.Clear();Check("buffer clear",buf.Count==0);
            string[] required={"Assets/LegendsOfRus/Resources/Config/liveops_readiness_activation.json","Assets/LegendsOfRus/Scripts/LiveOpsReadinessCore/LegendsOfRus.LiveOpsReadinessCore.asmdef","ProjectDocs/LiveOpsReadiness/guardrail_profile.template.json","ProjectDocs/LiveOpsReadiness/cadence_plan.template.json","ProjectDocs/LiveOpsReadiness/content_health.template.json","ProjectDocs/LiveOpsReadiness/experiment_proposal.template.json","README_V1_19_STEP04.md","Tools/verify_v1_19_step04.py"};
            for(int i=0;i<required.Length;i++)Check("required "+i,File.Exists(Path.Combine(root,required[i])));
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/liveops_readiness_activation.json"));
            string[] falseTokens={"\"productionAnalyticsProviderEnabled\": false","\"productionRemoteConfigEnabled\": false","\"experimentLaunchAuthorized\": false","\"liveOpsPublishAuthorized\": false","\"manualLiveOpsReviewAuthorized\": false","\"automaticLiveOpsMutationEnabled\": false","\"automaticEconomyMutationEnabled\": false","\"automaticRewardMutationEnabled\": false","\"automaticLevelBalanceMutationEnabled\": false","\"automaticExperimentAllocationEnabled\": false","\"playConsoleActionEnabled\": false"};
            for(int i=0;i<falseTokens.Length;i++)Check("activation false "+i,activation.Contains(falseTokens[i]));
            string[] trueTokens={"\"localCadenceReviewToolingApproved\": true","\"localContentHealthReviewToolingApproved\": true","\"localExperimentDesignReviewToolingApproved\": true","\"localSanitizedSnapshotToolingApproved\": true"};
            for(int i=0;i<trueTokens.Length;i++)Check("activation true "+i,activation.Contains(trueTokens[i]));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/LiveOpsReadinessCore"));string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsContentExperimentRunner.cs"));string joined=core+runner;
            string[] banned={"BuildPipeline.BuildPlayer","UnityWebRequest","HttpClient","Google.Apis","keystorePass","keyaliasPass","serviceAccountKey","deviceUniqueIdentifier","advertisingIdentifier","SystemInfo.deviceModel","PlayerSaveData","EconomyLedger","RuntimeEconomyTransactionEngine","PlayerPrefs.Set","RemoteConfigService.Fetch","AnalyticsService.CustomData"};
            for(int i=0;i<banned.Length;i++)Check("banned absent "+i,!joined.Contains(banned[i]));
            Check("core engine free",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables retained",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6 retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("hotfix04 retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("nunit hotfix retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef hotfix retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.19-step04-liveops-cadence-content-health-experiment-readiness");
            if(total!=82)throw new Exception("Step 19.4 self-test count mismatch: "+total+" expected 82");
            Debug.Log("v1.19 Step 4 self-tests passed: "+passed+"/"+total);
        }
        static ReviewedLiveOpsGuardrailProfile P(){return new ReviewedLiveOpsGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-LO-GR-0001",MinimumLeadMinutes=60,MaximumConcurrentEvents=2,MinimumGapMinutes=30,MaximumEventDurationMinutes=1440,MinimumContentParticipants=100,MinimumContentCompletionRateBasisPoints=2000,MaximumContentEntryFailureRateBasisPoints=500,MaximumContentCrashRateBasisPoints=200,MinimumRewardClaimRateBasisPoints=1000,MaximumRewardClaimRateBasisPoints=9900,MinimumExperimentSamplePerVariant=100,MinimumExperimentDurationHours=24,MaximumExperimentDurationHours=336};}
        static LiveOpsCadencePlanEvidence C(){return new LiveOpsCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-LO-CAD-0001",MinutesUntilWindowStart=120,Events=new[]{new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0001",StartMinute=0,EndMinute=120,ContentVersion="1"},new LiveOpsEventSlotEvidence{EventId="LOR-LO-EVT-0002",StartMinute=180,EndMinute=300,ContentVersion="1"}}};}
        static ContentHealthEvidence H(){return new ContentHealthEvidence{EvidenceRetained=true,EvidenceReviewed=true,HealthId="LOR-LO-HEALTH-0001",ContentVersion="1",Participants=1000,Entries=1000,Completions=600,EntryFailures=10,ContentCrashes=1,RewardClaims=500};}
        static ExperimentProposalEvidence E(){return new ExperimentProposalEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExperimentId="LOR-EXP-0001",HypothesisReviewed=true,PrimaryMetricReviewed=true,GuardrailMetricCount=2,ExpectedEligibleUsers=1000,DurationHours=72,Variants=new[]{new ExperimentVariantEvidence{VariantId="CONTROL",WeightBasisPoints=5000},new ExperimentVariantEvidence{VariantId="TREATMENT",WeightBasisPoints=5000}},MutuallyExclusiveExposure=true,StoppingRuleReviewed=true,PrivacyReviewed=true};}
        static bool Contains(string[]a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string n,bool c){total++;if(!c)throw new Exception("Step 19.4 self-test failed: "+n);passed++;}
    }
}
#endif
