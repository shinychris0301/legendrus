#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep103CutoverReadiness
    {
        public const string Root = "ProjectDocs/V1_20_STEP10_3/FinalIntegration";
        public const string DecisionJson = Root + "/FINAL_36_PAGE_CUTOVER_READINESS.json";
        public const string MatrixCsv = Root + "/FINAL_36_PAGE_CUTOVER_READINESS_MATRIX.csv";
        public const string SummaryMd = Root + "/FINAL_36_PAGE_CUTOVER_READINESS.md";

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 10.3/Evaluate Final 36-Page Cutover Readiness")]
        public static void Run()
        {
            List<string> globalFailures;
            List<Step20ProductionAssetStep103Evidence.PageState> states = Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            ReadinessResult result = Write(states, globalFailures);
            string message = "Step 10.3 readiness: " + result.decision + ". Source=" + result.sourceReady + "/36, runtime=" + result.runtimeReady + "/36, approvedRefs=" + result.approvedRefs + "/36, fidelityPass=" + result.fidelityPass + ", active=" + result.active + ", eligibleNonCore=" + result.eligibleNonCore + ".";
            if (globalFailures.Count > 0) Debug.LogError(message + "\n" + string.Join(Environment.NewLine, globalFailures.ToArray())); else Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message + "\n\nNo page is activated by this evaluator. Use the rollback-controlled cutover command only after reviewing the matrix.", "OK");
        }

        public static ReadinessResult EvaluateAndWrite()
        {
            List<string> globalFailures;
            List<Step20ProductionAssetStep103Evidence.PageState> states = Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            return Write(states, globalFailures);
        }

        static ReadinessResult Write(List<Step20ProductionAssetStep103Evidence.PageState> states, List<string> globalFailures)
        {
            int source=0,runtime=0,approved=0,fidelity=0,active=0,eligible=0,runtimeAwaitingReference=0,blocked=0;
            var csv = new StringBuilder();
            csv.AppendLine("index,pageKey,screenId,wave,sourceCandidate,runtimeEvidence,directApprovedReference,fidelityEvaluated,fidelityPass,correlation,mae,authorityBoundaryPass,activeProductionRoute,developmentCutoverEligible,category");
            var eligiblePages = new List<string>();
            var blockedPages = new List<string>();
            for (int i=0;i<states.Count;i++)
            {
                var s=states[i]; if(s.sourceCandidate)source++; if(s.runtimeEvidence)runtime++; if(s.directApprovedReference)approved++; if(s.fidelityPass)fidelity++; if(s.activeProductionRoute)active++; if(s.developmentCutoverEligible){eligible++;eligiblePages.Add(s.spec.id.ToString());}
                if(s.category=="RUNTIME_VALID_AWAITING_APPROVED_REFERENCE")runtimeAwaitingReference++;
                if(s.category.StartsWith("BLOCKED",StringComparison.Ordinal)){blocked++;blockedPages.Add(s.spec.id+":"+s.category);}
                csv.Append(i).Append(',').Append(s.spec.pageKey).Append(',').Append(s.spec.id).Append(',').Append(s.spec.wave).Append(',')
                    .Append(s.sourceCandidate).Append(',').Append(s.runtimeEvidence).Append(',').Append(s.directApprovedReference).Append(',').Append(s.fidelityEvaluated).Append(',').Append(s.fidelityPass).Append(',')
                    .Append(Step20ProductionAssetStep103Evidence.FormatFloat(s.correlation)).Append(',').Append(Step20ProductionAssetStep103Evidence.FormatFloat(s.mae)).Append(',')
                    .Append(s.authorityBoundaryPass).Append(',').Append(s.activeProductionRoute).Append(',').Append(s.developmentCutoverEligible).Append(',').Append(s.category).AppendLine();
            }

            string decision;
            if(globalFailures.Count>0||source!=36) decision="NO_GO_GLOBAL_AUTHORITY_OR_SOURCE_FAILURE";
            else if(blocked>0) decision="NO_GO_PAGE_FIX_REQUIRED";
            else if(eligible>0) decision="ELIGIBLE_ROLLBACK_CONTROLLED_PARTIAL_DEVELOPMENT_CUTOVER";
            else if(runtime==36&&approved<36) decision="RUNTIME_COMPLETE_REFERENCE_AUTHORITY_INCOMPLETE";
            else decision="PENDING_RUNTIME_AND_REFERENCE_EVIDENCE";

            string absRoot=Step20ProductionAssetStep103Evidence.Absolute(Root); Directory.CreateDirectory(absRoot);
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(MatrixCsv),csv.ToString());
            string json="{\n"+
                "  \"schemaVersion\": 1,\n"+
                "  \"milestone\": \"v1.20-step10.3-final-runtime-integration-readiness\",\n"+
                "  \"decision\": \""+decision+"\",\n"+
                "  \"functionalPages\": 36,\n"+
                "  \"sourceCandidatePages\": "+source+",\n"+
                "  \"runtimeEvidencePages\": "+runtime+",\n"+
                "  \"directApprovedReferencePages\": "+approved+",\n"+
                "  \"approvedReferenceRuntimeFidelityPasses\": "+fidelity+",\n"+
                "  \"activeProductionRoutes\": "+active+",\n"+
                "  \"eligibleNonCoreDevelopmentCutovers\": "+eligible+",\n"+
                "  \"runtimeValidAwaitingApprovedReference\": "+runtimeAwaitingReference+",\n"+
                "  \"blockedFixRequired\": "+blocked+",\n"+
                "  \"globalAuthorityFailureCount\": "+globalFailures.Count+",\n"+
                "  \"automaticCutoverPerformed\": false,\n"+
                "  \"androidAcceptanceRequiredAfterAnyNewCutover\": true,\n"+
                "  \"eligiblePages\": ["+JsonArray(eligiblePages)+"],\n"+
                "  \"blockedPages\": ["+JsonArray(blockedPages)+"]\n"+
                "}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(DecisionJson),json);
            var md=new StringBuilder();
            md.AppendLine("# v1.20 Step 10.3 — Final 36-Page Cutover Readiness").AppendLine();
            md.AppendLine("Decision: **"+decision+"**").AppendLine();
            md.AppendLine("| Axis | Count |").AppendLine("|---|---:|")
              .AppendLine("| Source candidates | "+source+" / 36 |")
              .AppendLine("| Actual Unity runtime evidence | "+runtime+" / 36 |")
              .AppendLine("| Direct approved references | "+approved+" / 36 |")
              .AppendLine("| Approved-reference runtime fidelity passes | "+fidelity+" |")
              .AppendLine("| Active production routes | "+active+" |")
              .AppendLine("| Eligible non-core rollback-controlled cutovers | "+eligible+" |")
              .AppendLine("| Runtime-valid, awaiting approved reference | "+runtimeAwaitingReference+" |")
              .AppendLine("| Blocked/fix required | "+blocked+" |").AppendLine();
            md.AppendLine("This evaluator never activates a page. Source reconstruction, runtime verification, approved visual authority, cutover, and Android acceptance are separate gates.");
            if(eligiblePages.Count>0) md.AppendLine().AppendLine("Eligible non-core pages: "+string.Join(", ",eligiblePages.ToArray()));
            if(globalFailures.Count>0){md.AppendLine().AppendLine("## Global authority failures");for(int i=0;i<globalFailures.Count;i++)md.AppendLine("- "+globalFailures[i]);}
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(SummaryMd),md.ToString());
            AssetDatabase.Refresh();
            return new ReadinessResult(decision,source,runtime,approved,fidelity,active,eligible,runtimeAwaitingReference,blocked,globalFailures.Count);
        }

        static string JsonArray(List<string> items)
        {
            var sb=new StringBuilder(); for(int i=0;i<items.Count;i++){if(i>0)sb.Append(',');sb.Append('"').Append(Escape(items[i])).Append('"');} return sb.ToString();
        }
        static string Escape(string value){return(value??string.Empty).Replace("\\","\\\\").Replace("\"","\\\"");}

        public readonly struct ReadinessResult
        {
            public readonly string decision; public readonly int sourceReady,runtimeReady,approvedRefs,fidelityPass,active,eligibleNonCore,runtimeAwaitingReference,blocked,globalFailures;
            public ReadinessResult(string d,int s,int r,int a,int f,int active,int e,int rawr,int b,int g){decision=d;sourceReady=s;runtimeReady=r;approvedRefs=a;fidelityPass=f;this.active=active;eligibleNonCore=e;runtimeAwaitingReference=rawr;blocked=b;globalFailures=g;}
        }
    }
}
#endif
