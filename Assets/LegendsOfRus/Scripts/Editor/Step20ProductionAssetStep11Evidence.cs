#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Release-candidate evidence model for v1.20 Step 11. Development RC stability
    /// and store-release readiness are intentionally separate decisions.
    /// </summary>
    public static class Step20ProductionAssetStep11Evidence
    {
        public const string Root = "ProjectDocs/V1_20_STEP11/ReleaseCandidate";
        public const string StatusJson = Root + "/STEP11_RC_STABILIZATION_STATUS.json";
        public const string StatusMd = Root + "/STEP11_RC_STABILIZATION_STATUS.md";
        public const string ActiveMatrixCsv = Root + "/STEP11_ACTIVE_ROUTE_MATRIX.csv";
        public const string DevelopmentRcLock = Root + "/DEVELOPMENT_RC_FREEZE.lock";
        public const string DevelopmentRcManifest = Root + "/DEVELOPMENT_RC_MANIFEST.json";

        public static RcStatus EvaluateAndWrite()
        {
            List<string> globalFailures;
            List<Step20ProductionAssetStep103Evidence.PageState> states = Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            var active = new List<Step20ProductionAssetStep103Evidence.PageState>();
            int source=0,runtime=0,approved=0,fidelity=0,activeApproved=0,activeRuntime=0,activeFidelity=0;
            for(int i=0;i<states.Count;i++)
            {
                var s=states[i];
                if(s.sourceCandidate)source++;
                if(s.runtimeEvidence)runtime++;
                if(s.directApprovedReference)approved++;
                if(s.fidelityPass)fidelity++;
                if(!s.activeProductionRoute)continue;
                active.Add(s);
                if(s.directApprovedReference)activeApproved++;
                if(s.runtimeEvidence)activeRuntime++;
                if(s.directApprovedReference && s.runtimeEvidence && s.fidelityPass)activeFidelity++;
            }

            List<string> batchFailures=Step20ProductionAssetStep103RollbackCutover.ValidateActiveBatch();
            for(int i=0;i<batchFailures.Count;i++)globalFailures.Add("ACTIVE-CUTOVER: "+batchFailures[i]);

            string fingerprint=Step20ProductionAssetStep103AndroidRegression.CurrentIntegrationFingerprint();
            bool functionalPass=LockMatchesCurrentFingerprint(Step20ProductionAssetStep103AndroidRegression.FunctionalLock,fingerprint);
            bool androidPass=LockMatchesCurrentFingerprint(Step20ProductionAssetStep103AndroidRegression.AndroidLock,fingerprint);
            bool finalAndroidPass=FinalAndroidDecisionMatchesFingerprint(fingerprint);

            string developmentDecision;
            if(globalFailures.Count>0||source!=36) developmentDecision="NO_GO_DEVELOPMENT_RC_AUTHORITY_OR_SOURCE_FAILURE";
            else if(!functionalPass||!androidPass||!finalAndroidPass) developmentDecision="PENDING_FULL_FUNCTIONAL_AND_ANDROID_REGRESSION";
            else developmentDecision="READY_FOR_DEVELOPMENT_RC_FREEZE";

            string storeDecision;
            if(developmentDecision!="READY_FOR_DEVELOPMENT_RC_FREEZE") storeDecision="NO_GO_STORE_RC_REGRESSION_PENDING";
            else if(active.Count!=36) storeDecision="NO_GO_STORE_RC_PRODUCTION_ROUTE_COVERAGE_"+active.Count+"_OF_36";
            else if(activeApproved!=36) storeDecision="NO_GO_STORE_RC_APPROVED_REFERENCE_COVERAGE_"+activeApproved+"_OF_36";
            else if(activeRuntime!=36) storeDecision="NO_GO_STORE_RC_RUNTIME_EVIDENCE_"+activeRuntime+"_OF_36";
            else if(activeFidelity!=36) storeDecision="NO_GO_STORE_RC_RUNTIME_FIDELITY_"+activeFidelity+"_OF_36";
            else storeDecision="READY_FOR_STORE_RELEASE_CANDIDATE_FREEZE";

            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(Root));
            WriteActiveMatrix(active);
            string json="{\n"+
                "  \"schemaVersion\": 1,\n"+
                "  \"milestone\": \"v1.20-step11-release-candidate-stabilization\",\n"+
                "  \"integrationFingerprint\": \""+fingerprint+"\",\n"+
                "  \"developmentRcDecision\": \""+developmentDecision+"\",\n"+
                "  \"storeReleaseRcDecision\": \""+storeDecision+"\",\n"+
                "  \"functionalPages\": 36,\n"+
                "  \"sourceCandidatePages\": "+source+",\n"+
                "  \"runtimeEvidencePages\": "+runtime+",\n"+
                "  \"directApprovedReferencePages\": "+approved+",\n"+
                "  \"runtimeFidelityPassPages\": "+fidelity+",\n"+
                "  \"activeProductionRoutes\": "+active.Count+",\n"+
                "  \"activeRoutesWithApprovedReference\": "+activeApproved+",\n"+
                "  \"activeRoutesWithRuntimeEvidence\": "+activeRuntime+",\n"+
                "  \"activeRoutesWithApprovedRuntimeFidelityPass\": "+activeFidelity+",\n"+
                "  \"functionalRegressionPassForCurrentFingerprint\": "+Bool(functionalPass)+",\n"+
                "  \"physicalAndroidPassForCurrentFingerprint\": "+Bool(androidPass)+",\n"+
                "  \"finalAndroidAcceptanceForCurrentFingerprint\": "+Bool(finalAndroidPass)+",\n"+
                "  \"globalFailureCount\": "+globalFailures.Count+",\n"+
                "  \"automaticCutoverPerformed\": false,\n"+
                "  \"developmentRcFreezeCreated\": "+Bool(File.Exists(Step20ProductionAssetStep103Evidence.Absolute(DevelopmentRcLock)))+"\n"+
                "}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusJson),json);

            var md=new StringBuilder();
            md.AppendLine("# v1.20 Step 11 — Release Candidate Stabilization Status").AppendLine();
            md.AppendLine("Development RC decision: **"+developmentDecision+"**");
            md.AppendLine("Store-release RC decision: **"+storeDecision+"**").AppendLine();
            md.AppendLine("| Axis | Count / state |").AppendLine("|---|---:|")
              .AppendLine("| Source candidate pages | "+source+" / 36 |")
              .AppendLine("| Runtime-evidence pages | "+runtime+" / 36 |")
              .AppendLine("| Direct approved references | "+approved+" / 36 |")
              .AppendLine("| Active production routes | "+active.Count+" / 36 |")
              .AppendLine("| Active routes with direct approved reference | "+activeApproved+" / "+active.Count+" |")
              .AppendLine("| Active routes with runtime evidence | "+activeRuntime+" / "+active.Count+" |")
              .AppendLine("| Functional regression lock matches current integration | "+functionalPass+" |")
              .AppendLine("| Physical Android lock matches current integration | "+androidPass+" |")
              .AppendLine("| Final Android acceptance matches current integration | "+finalAndroidPass+" |").AppendLine();
            md.AppendLine("A development RC freeze stabilizes an already-regression-tested integration. Store-release RC readiness is stricter: every functional page must be a production route with direct approved visual authority and passing runtime fidelity.");
            if(globalFailures.Count>0){md.AppendLine().AppendLine("## Authority / integration failures");for(int i=0;i<globalFailures.Count;i++)md.AppendLine("- "+globalFailures[i]);}
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusMd),md.ToString());
            AssetDatabase.Refresh();
            return new RcStatus(developmentDecision,storeDecision,fingerprint,source,runtime,approved,active.Count,activeApproved,activeRuntime,activeFidelity,functionalPass,androidPass,finalAndroidPass,globalFailures.Count);
        }

        public static List<string> ValidateDevelopmentRcFreeze()
        {
            var failures=new List<string>();
            string lockPath=Step20ProductionAssetStep103Evidence.Absolute(DevelopmentRcLock);
            string manifestPath=Step20ProductionAssetStep103Evidence.Absolute(DevelopmentRcManifest);
            if(!File.Exists(lockPath))failures.Add("Development RC freeze lock is missing.");
            if(!File.Exists(manifestPath))failures.Add("Development RC manifest is missing.");
            if(failures.Count>0)return failures;
            string fingerprint=Step20ProductionAssetStep103AndroidRegression.CurrentIntegrationFingerprint();
            string lockText=File.ReadAllText(lockPath), manifestText=File.ReadAllText(manifestPath);
            if(lockText.IndexOf("integration-fingerprint="+fingerprint,StringComparison.Ordinal)<0)failures.Add("Development RC freeze lock belongs to a different integration fingerprint.");
            if(manifestText.IndexOf("\"integrationFingerprint\": \""+fingerprint+"\"",StringComparison.Ordinal)<0)failures.Add("Development RC manifest belongs to a different integration fingerprint.");
            string manifestHash=Sha256File(manifestPath);
            if(lockText.IndexOf("manifest-sha256="+manifestHash,StringComparison.Ordinal)<0)failures.Add("Development RC manifest hash does not match the freeze lock.");
            return failures;
        }

        public static bool LockMatchesCurrentFingerprint(string relative,string fingerprint)
        {
            string abs=Step20ProductionAssetStep103Evidence.Absolute(relative);
            if(!File.Exists(abs))return false;
            return File.ReadAllText(abs).IndexOf("integration-fingerprint="+fingerprint,StringComparison.Ordinal)>=0;
        }

        static bool FinalAndroidDecisionMatchesFingerprint(string fingerprint)
        {
            string abs=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103AndroidRegression.FinalDecision);
            if(!File.Exists(abs))return false;
            string text=File.ReadAllText(abs);
            return text.IndexOf("\"decision\": \"ACCEPTED_CURRENT_INTEGRATION_ANDROID_REGRESSION\"",StringComparison.Ordinal)>=0 &&
                   text.IndexOf("\"integrationFingerprint\": \""+fingerprint+"\"",StringComparison.Ordinal)>=0;
        }

        static void WriteActiveMatrix(List<Step20ProductionAssetStep103Evidence.PageState> active)
        {
            var sb=new StringBuilder();
            sb.AppendLine("screenId,wave,directApprovedReference,runtimeEvidence,fidelityPass,correlation,mae,coreRoute,metaRoute");
            for(int i=0;i<active.Count;i++)
            {
                var s=active[i];
                sb.Append(s.spec.id).Append(',').Append(s.spec.wave).Append(',').Append(s.directApprovedReference).Append(',').Append(s.runtimeEvidence).Append(',').Append(s.fidelityPass).Append(',')
                  .Append(Step20ProductionAssetStep103Evidence.FormatFloat(s.correlation)).Append(',').Append(Step20ProductionAssetStep103Evidence.FormatFloat(s.mae)).Append(',')
                  .Append(ProductionCoreLoopPageRegistry.IsCutover(s.spec.id)).Append(',').Append(ProductionMetaPageCutoverRegistry.IsCutover(s.spec.id)).AppendLine();
            }
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(ActiveMatrixCsv),sb.ToString());
        }

        public static string Sha256File(string path)
        {
            using(var sha=SHA256.Create())using(var stream=File.OpenRead(path)){return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-",string.Empty).ToLowerInvariant();}
        }
        static string Bool(bool value){return value?"true":"false";}

        public readonly struct RcStatus
        {
            public readonly string developmentDecision,storeDecision,fingerprint; public readonly int source,runtime,approved,active,activeApproved,activeRuntime,activeFidelity,globalFailures; public readonly bool functionalPass,androidPass,finalAndroidPass;
            public RcStatus(string dd,string sd,string fp,int s,int r,int a,int ac,int aa,int ar,int af,bool f,bool ap,bool finalp,int g){developmentDecision=dd;storeDecision=sd;fingerprint=fp;source=s;runtime=r;approved=a;active=ac;activeApproved=aa;activeRuntime=ar;activeFidelity=af;functionalPass=f;androidPass=ap;finalAndroidPass=finalp;globalFailures=g;}
        }
    }
}
#endif
