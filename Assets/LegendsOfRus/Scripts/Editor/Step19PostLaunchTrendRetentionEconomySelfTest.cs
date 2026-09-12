#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.PostLaunchReview;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step19PostLaunchTrendRetentionEconomySelfTest
    {
        static int total, passed;
        [MenuItem("Legends of Rus/Tests/Run v1.19 Step 3 Self-Tests")]
        public static void Run()
        {
            total=passed=0;
            string root=Directory.GetParent(Application.dataPath).FullName;
            var p=Profile(); var t=Trend(); var r=Retention(); var e=Economy();
            Check("schema",PostLaunchReviewRequirements.SchemaVersion==1);
            Check("default buffer",PostLaunchReviewRequirements.DefaultBufferCapacity==30);
            Check("max buffer",PostLaunchReviewRequirements.MaxBufferCapacity==90);
            Check("profile id",PostLaunchReviewValidation.IsStableId(p.ProfileId,PostLaunchReviewRequirements.GuardrailProfilePrefix));
            Check("trend id",PostLaunchReviewValidation.IsStableId(t.WindowId,PostLaunchReviewRequirements.TrendWindowPrefix));
            Check("retention id",PostLaunchReviewValidation.IsStableId(r.CohortId,PostLaunchReviewRequirements.RetentionCohortPrefix));
            Check("economy id",PostLaunchReviewValidation.IsStableId(e.WindowId,PostLaunchReviewRequirements.EconomyFlowPrefix));
            Check("profile valid",PostLaunchReviewValidation.ValidateProfile(p).Length==0);
            Check("trend valid",PostLaunchReviewValidation.ValidateTrend(t).Length==0);
            Check("retention valid",PostLaunchReviewValidation.ValidateRetention(r).Length==0);
            Check("economy valid",PostLaunchReviewValidation.ValidateEconomy(e).Length==0);
            Check("rate",PostLaunchReviewValidation.RateBasisPoints(50,100)==5000);
            Check("decline",PostLaunchReviewValidation.PositiveRelativeDeclineBasisPoints(100,80)==2000);
            Check("drop",PostLaunchReviewValidation.PositiveDrop(5000,4500)==500);
            var tr=TrendRetentionReviewEvaluator.Evaluate(p,t,r);
            Check("trend structural",tr.StructurallyValid); Check("trend sample",tr.SampleSufficient); Check("trend healthy",tr.TrendHealthy); Check("retention healthy",tr.RetentionHealthy);
            var eg=EconomyGuardrailEvaluator.Evaluate(p,e);
            Check("economy structural",eg.StructurallyValid); Check("economy sample",eg.SampleSufficient); Check("economy healthy",eg.EconomyHealthy); Check("soft ratio",eg.SoftCurrencySinkSourceRatioBasisPoints==9000);
            var all=PostLaunchReviewEvaluator.Evaluate(p,t,r,e,false);
            Check("combined ready",all.ReadyForHumanReview); Check("combined healthy trend",all.TrendRetentionHealthy); Check("combined healthy economy",all.EconomyHealthy); Check("manual boundary",!all.MayProceedWithManualEconomyChangeReview); Check("manual blocker",Contains(all.EconomyChangeBlockers,PostLaunchReviewBlockerCodes.ManualEconomyReviewNotAuthorized));
            var buffer=new PostLaunchAggregateBuffer(3); buffer.Add(new AggregateObservation{DayIndex=1}); buffer.Add(new AggregateObservation{DayIndex=2}); buffer.Add(new AggregateObservation{DayIndex=3}); buffer.Add(new AggregateObservation{DayIndex=4});
            Check("buffer bounded",buffer.Count==3); Check("buffer order first",buffer.SnapshotOldestFirst()[0].DayIndex==2); Check("buffer order last",buffer.SnapshotOldestFirst()[2].DayIndex==4); buffer.Clear(); Check("buffer clear",buffer.Count==0);

            string[] required={
                "Assets/LegendsOfRus/Resources/Config/post_launch_review_activation.json",
                "Assets/LegendsOfRus/Scripts/PostLaunchReviewCore/LegendsOfRus.PostLaunchReviewCore.asmdef",
                "ProjectDocs/PostLaunchReview/guardrail_profile.template.json",
                "ProjectDocs/PostLaunchReview/trend_window.template.json",
                "ProjectDocs/PostLaunchReview/retention_cohort.template.json",
                "ProjectDocs/PostLaunchReview/economy_flow.template.json",
                "README_V1_19_STEP03.md",
                "Tools/verify_v1_19_step03.py"
            };
            for(int i=0;i<required.Length;i++) Check("required "+i,File.Exists(Path.Combine(root,required[i])));
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/post_launch_review_activation.json"));
            string[] falseTokens={"\"guardrailProfileRetained\": false","\"manualEconomyChangeReviewAuthorized\": false","\"automaticEconomyMutationEnabled\": false","\"automaticRewardMutationEnabled\": false","\"automaticLevelBalanceMutationEnabled\": false","\"productionTelemetryUploadEnabled\": false","\"productionAnalyticsProviderEnabled\": false","\"playConsoleActionEnabled\": false"};
            for(int i=0;i<falseTokens.Length;i++) Check("activation false "+i,activation.Contains(falseTokens[i]));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/PostLaunchReviewCore"));
            string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step19PostLaunchTrendRetentionEconomyRunner.cs"));
            string joined=core+runner;
            string[] banned={"BuildPipeline.BuildPlayer","UnityWebRequest","HttpClient","Google.Apis","keystorePass","keyaliasPass","serviceAccountKey","deviceUniqueIdentifier","advertisingIdentifier","SystemInfo.deviceModel","PlayerSaveData","EconomyLedger","RuntimeEconomyTransactionEngine"};
            for(int i=0;i<banned.Length;i++) Check("banned absent "+i,!joined.Contains(banned[i]));
            Check("core engine free",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables retained",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6 retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("hotfix04 retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("nunit type hotfix retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content delivery asmdef fix retained",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.19-step03-post-launch-trend-retention-economy-guardrail-review");
            if(total!=67) throw new Exception("Step 19.3 self-test count mismatch: "+total+" expected 67");
            Debug.Log("v1.19 Step 3 self-tests passed: "+passed+"/"+total);
        }

        static ReviewedGuardrailProfile Profile(){return new ReviewedGuardrailProfile{EvidenceRetained=true,EvidenceReviewed=true,ProfileId="LOR-GR-REVIEWED-0001",MinimumActiveUsers=100,MinimumCohortSize=100};}
        static TrendWindowEvidence Trend(){return new TrendWindowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-TW-WEEK-0001",WindowDays=7,BaselineActiveUsers=1000,CurrentActiveUsers=950,BaselineSessions=2000,CurrentSessions=1950,BaselineLevelStarts=5000,BaselineLevelCompletes=4000,CurrentLevelStarts=4800,CurrentLevelCompletes=3800};}
        static RetentionCohortEvidence Retention(){return new RetentionCohortEvidence{EvidenceRetained=true,EvidenceReviewed=true,CohortId="LOR-COHORT-0001",CohortSize=1000,D1Returned=400,D7Returned=200,D30Returned=100,BaselineD1BasisPoints=4200,BaselineD7BasisPoints=2100,BaselineD30BasisPoints=1050};}
        static EconomyFlowEvidence Economy(){return new EconomyFlowEvidence{EvidenceRetained=true,EvidenceReviewed=true,WindowId="LOR-ECO-WEEK-0001",ActiveUsers=1000,SoftCurrencyEarned=100000,SoftCurrencySpent=90000,HardCurrencyGranted=10000,HardCurrencySpent=9000,BoostersGranted=3000,BoostersConsumed=2500,BaselineRewardUnitsGranted=100000,CurrentRewardUnitsGranted=105000};}
        static bool Contains(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string path){var s="";foreach(var f in Directory.GetFiles(path,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string name,bool condition){total++;if(!condition)throw new Exception("Step 19.3 self-test failed: "+name);passed++;}
    }
}
#endif
