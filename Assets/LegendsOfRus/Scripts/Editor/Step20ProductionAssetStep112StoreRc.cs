#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Final Step 11.2 store-release candidate readiness. This gate is intentionally
    /// stronger than the Step 11 Development RC freeze: every one of the 36 functional
    /// pages must be a production route with direct approved authority, runtime evidence,
    /// fidelity PASS, authority PASS, and regression/Android acceptance for the same
    /// integration fingerprint.
    /// </summary>
    public static class Step20ProductionAssetStep112StoreRc
    {
        public const string Root = "ProjectDocs/V1_20_STEP11_2/StoreRC";
        public const string StatusJson = Root + "/STORE_RC_READINESS.json";
        public const string StatusMd = Root + "/STORE_RC_READINESS.md";
        public const string MatrixCsv = Root + "/STORE_RC_36_PAGE_MATRIX.csv";
        public const string StoreRcLock = Root + "/STORE_RELEASE_RC_FREEZE.lock";
        public const string StoreRcManifest = Root + "/STORE_RELEASE_RC_MANIFEST.json";

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11.2/Evaluate Store-RC Readiness")]
        public static void EvaluateMenu()
        {
            StoreStatus s=EvaluateAndWrite();
            string message="Step 11.2 Store-RC: "+s.decision+". active="+s.active+"/36, approved="+s.approved+"/36, runtime="+s.runtime+"/36, fidelity="+s.fidelity+"/36.";
            Debug.Log(message);if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus",message,"OK");
        }

        public static StoreStatus EvaluateAndWrite()
        {
            List<string> globalFailures;List<Step20ProductionAssetStep103Evidence.PageState> states=Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            globalFailures.AddRange(Step20ProductionAssetStep103RollbackCutover.ValidateActiveBatch());
            globalFailures.AddRange(Step20ProductionAssetStep112RouteExpansion.ValidateActiveExpansions());
            int source=0,runtime=0,approved=0,fidelity=0,active=0,activeApproved=0,activeRuntime=0,activeFidelity=0;
            var csv=new StringBuilder();csv.AppendLine("index,pageKey,screenId,wave,sourceCandidate,runtimeEvidence,directApprovedReference,fidelityPass,activeProductionRoute,storeRcPageReady,category");
            for(int i=0;i<states.Count;i++)
            {
                var p=states[i];if(p.sourceCandidate)source++;if(p.runtimeEvidence)runtime++;if(p.directApprovedReference)approved++;if(p.fidelityPass)fidelity++;if(p.activeProductionRoute){active++;if(p.directApprovedReference)activeApproved++;if(p.runtimeEvidence)activeRuntime++;if(p.directApprovedReference&&p.runtimeEvidence&&p.fidelityPass)activeFidelity++;}
                bool ready=p.sourceCandidate&&p.runtimeEvidence&&p.directApprovedReference&&p.fidelityPass&&p.authorityBoundaryPass&&p.activeProductionRoute;
                csv.Append(i).Append(',').Append(p.spec.pageKey).Append(',').Append(p.spec.id).Append(',').Append(p.spec.wave).Append(',').Append(p.sourceCandidate).Append(',').Append(p.runtimeEvidence).Append(',').Append(p.directApprovedReference).Append(',').Append(p.fidelityPass).Append(',').Append(p.activeProductionRoute).Append(',').Append(ready).Append(',').Append(p.category).AppendLine();
            }
            List<string> freezeFailures=Step20ProductionAssetStep11Evidence.ValidateDevelopmentRcFreeze();bool devFreezeValid=freezeFailures.Count==0;
            string fingerprint=Step20ProductionAssetStep103AndroidRegression.CurrentIntegrationFingerprint();
            bool functional=Step20ProductionAssetStep11Evidence.LockMatchesCurrentFingerprint(Step20ProductionAssetStep103AndroidRegression.FunctionalLock,fingerprint);
            bool android=Step20ProductionAssetStep11Evidence.LockMatchesCurrentFingerprint(Step20ProductionAssetStep103AndroidRegression.AndroidLock,fingerprint);
            bool finalAndroid=FinalAndroidMatches(fingerprint);
            string decision;
            if(globalFailures.Count>0||source!=36)decision="NO_GO_STORE_RC_AUTHORITY_OR_SOURCE_FAILURE";
            else if(approved!=36)decision="NO_GO_STORE_RC_APPROVED_REFERENCE_COVERAGE_"+approved+"_OF_36";
            else if(runtime!=36)decision="NO_GO_STORE_RC_RUNTIME_EVIDENCE_"+runtime+"_OF_36";
            else if(fidelity!=36)decision="NO_GO_STORE_RC_RUNTIME_FIDELITY_"+fidelity+"_OF_36";
            else if(active!=36)decision="NO_GO_STORE_RC_PRODUCTION_ROUTE_COVERAGE_"+active+"_OF_36";
            else if(!functional||!android||!finalAndroid)decision="NO_GO_STORE_RC_REGRESSION_OR_ANDROID_PENDING";
            else if(!devFreezeValid)decision="NO_GO_STORE_RC_DEVELOPMENT_RC_FREEZE_INVALID";
            else decision="READY_FOR_STORE_RELEASE_RC_FREEZE";
            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(Root));File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(MatrixCsv),csv.ToString());
            string json="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step11.2-store-rc-readiness\",\n  \"decision\": \""+decision+"\",\n  \"integrationFingerprint\": \""+fingerprint+"\",\n  \"sourceCandidatePages\": "+source+",\n  \"directApprovedReferencePages\": "+approved+",\n  \"runtimeEvidencePages\": "+runtime+",\n  \"runtimeFidelityPassPages\": "+fidelity+",\n  \"activeProductionRoutes\": "+active+",\n  \"activeRoutesWithApprovedReference\": "+activeApproved+",\n  \"activeRoutesWithRuntimeEvidence\": "+activeRuntime+",\n  \"activeRoutesWithApprovedRuntimeFidelityPass\": "+activeFidelity+",\n  \"functionalRegressionPass\": "+Bool(functional)+",\n  \"physicalAndroidRegressionPass\": "+Bool(android)+",\n  \"finalAndroidAcceptancePass\": "+Bool(finalAndroid)+",\n  \"developmentRcFreezeValid\": "+Bool(devFreezeValid)+",\n  \"authorityFailureCount\": "+globalFailures.Count+",\n  \"automaticProductionCutoverPerformed\": false\n}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusJson),json);
            var md=new StringBuilder();md.AppendLine("# v1.20 Step 11.2 — Store-RC Readiness").AppendLine();md.AppendLine("Decision: **"+decision+"**").AppendLine();
            md.AppendLine("| Axis | State |").AppendLine("|---|---:|")
              .AppendLine("| Source candidates | "+source+" / 36 |")
              .AppendLine("| Direct approved references | "+approved+" / 36 |")
              .AppendLine("| Runtime evidence | "+runtime+" / 36 |")
              .AppendLine("| Runtime fidelity PASS | "+fidelity+" / 36 |")
              .AppendLine("| Active production routes | "+active+" / 36 |")
              .AppendLine("| Functional regression current fingerprint | "+functional+" |")
              .AppendLine("| Physical Android current fingerprint | "+android+" |")
              .AppendLine("| Final Android acceptance | "+finalAndroid+" |")
              .AppendLine("| Step 11 Development RC freeze valid | "+devFreezeValid+" |").AppendLine();
            md.AppendLine("Store-RC readiness requires all axes to pass for the same integration fingerprint. This evaluator never activates a page.");
            if(globalFailures.Count>0){md.AppendLine().AppendLine("## Authority / active-route failures");for(int i=0;i<globalFailures.Count;i++)md.AppendLine("- "+globalFailures[i]);}
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusMd),md.ToString());AssetDatabase.Refresh();
            return new StoreStatus(decision,fingerprint,source,approved,runtime,fidelity,active,functional,android,finalAndroid,devFreezeValid,globalFailures.Count);
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11.2/Create Store-Release RC Freeze")]
        public static void CreateStoreRcFreeze()
        {
            EnsureEditorSafe();StoreStatus s=EvaluateAndWrite();if(s.decision!="READY_FOR_STORE_RELEASE_RC_FREEZE")throw new InvalidOperationException("Store-release RC freeze is not eligible: "+s.decision);
            string lockAbs=Step20ProductionAssetStep103Evidence.Absolute(StoreRcLock),manifestAbs=Step20ProductionAssetStep103Evidence.Absolute(StoreRcManifest);if(File.Exists(lockAbs)||File.Exists(manifestAbs))throw new InvalidOperationException("A Step 11.2 Store-RC freeze already exists. Validate or clear it before creating another.");
            if(!EditorUtility.DisplayDialog("Legends of Rus","Freeze the CURRENT 36/36 production integration as the v1.20 Store-Release Candidate?\n\nAll pages must already have approved references, runtime fidelity, authority PASS, and Android regression for this exact fingerprint.","Freeze Store RC","Cancel"))return;
            Directory.CreateDirectory(Path.GetDirectoryName(manifestAbs));string manifest=BuildManifest(s);File.WriteAllText(manifestAbs,manifest);string mh=Sha256(manifestAbs);File.WriteAllText(lockAbs,"v1.20-step11.2-store-release-rc-freeze\nintegration-fingerprint="+s.fingerprint+"\nmanifest-sha256="+mh+"\nrecorded-at-utc="+DateTime.UtcNow.ToString("o")+"\n");AssetDatabase.Refresh();Debug.Log("Step 11.2 Store-Release RC freeze created for integration fingerprint "+s.fingerprint+".");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11.2/Validate Store-Release RC Freeze")]
        public static void ValidateStoreRcFreeze()
        {
            StoreStatus s=EvaluateAndWrite();var f=new List<string>();string lockAbs=Step20ProductionAssetStep103Evidence.Absolute(StoreRcLock),manifestAbs=Step20ProductionAssetStep103Evidence.Absolute(StoreRcManifest);if(!File.Exists(lockAbs))f.Add("Store-RC freeze lock missing.");if(!File.Exists(manifestAbs))f.Add("Store-RC manifest missing.");
            if(s.decision!="READY_FOR_STORE_RELEASE_RC_FREEZE")f.Add("Current integration no longer satisfies Store-RC readiness: "+s.decision+".");
            if(f.Count==0){string lockText=File.ReadAllText(lockAbs);if(lockText.IndexOf("integration-fingerprint="+s.fingerprint,StringComparison.Ordinal)<0)f.Add("Store-RC freeze belongs to a different integration fingerprint.");string mh=Sha256(manifestAbs);if(lockText.IndexOf("manifest-sha256="+mh,StringComparison.Ordinal)<0)f.Add("Store-RC manifest hash does not match freeze lock.");}
            if(f.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray()));Debug.Log("Step 11.2 Store-Release RC freeze VALID.");if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Store-Release RC freeze is valid for the current integration fingerprint.","OK");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11.2/Clear Store-Release RC Freeze")]
        public static void ClearStoreRcFreeze(){EnsureEditorSafe();if(!EditorUtility.DisplayDialog("Legends of Rus","Clear the Step 11.2 Store-RC freeze/manifest? This does not change production routes.","Clear","Cancel"))return;DeleteIfExists(StoreRcLock);DeleteIfExists(StoreRcManifest);AssetDatabase.Refresh();}

        static string BuildManifest(StoreStatus s)
        {
            var sb=new StringBuilder();sb.Append("{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step11.2-store-release-rc-freeze\",\n  \"integrationFingerprint\": \"").Append(s.fingerprint).Append("\",\n  \"activeProductionRoutes\": 36,\n  \"directApprovedReferences\": 36,\n  \"runtimeEvidencePages\": 36,\n  \"runtimeFidelityPassPages\": 36,\n  \"approvedReferenceSha256\": {\n");
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++){var p=Step20ProductionAssetStep103Evidence.Pages[i];string abs=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(p.id));sb.Append("    \"").Append(p.id).Append("\": \"").Append(File.Exists(abs)?Sha256(abs):string.Empty).Append("\"").Append(i==Step20ProductionAssetStep103Evidence.Pages.Length-1?"\n":",\n");}
            sb.Append("  },\n  \"frozenAtUtc\": \"").Append(DateTime.UtcNow.ToString("o")).Append("\"\n}\n");return sb.ToString();
        }
        static bool FinalAndroidMatches(string fp){string p=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103AndroidRegression.FinalDecision);if(!File.Exists(p))return false;string t=File.ReadAllText(p);return t.IndexOf("\"decision\": \"ACCEPTED_CURRENT_INTEGRATION_ANDROID_REGRESSION\"",StringComparison.Ordinal)>=0&&t.IndexOf("\"integrationFingerprint\": \""+fp+"\"",StringComparison.Ordinal)>=0;}
        static string Sha256(string path){using(var sha=SHA256.Create())using(var s=File.OpenRead(path)){return BitConverter.ToString(sha.ComputeHash(s)).Replace("-",string.Empty).ToLowerInvariant();}}
        static string Bool(bool v){return v?"true":"false";}
        static void DeleteIfExists(string rel){string p=Step20ProductionAssetStep103Evidence.Absolute(rel);if(File.Exists(p))File.Delete(p);}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Step 11.2 Store-RC actions cannot run in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Step 11.2 Store-RC actions.");}

        public readonly struct StoreStatus
        {
            public readonly string decision,fingerprint;public readonly int source,approved,runtime,fidelity,active,authorityFailures;public readonly bool functional,android,finalAndroid,devFreezeValid;
            public StoreStatus(string d,string fp,int s,int a,int r,int f,int ac,bool fun,bool an,bool fa,bool df,int af){decision=d;fingerprint=fp;source=s;approved=a;runtime=r;fidelity=f;active=ac;functional=fun;android=an;finalAndroid=fa;devFreezeValid=df;authorityFailures=af;}
        }
    }
}
#endif
