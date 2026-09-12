#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step94ConditionalAcceptance
    {
        const string DecisionJson = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence/WAVE03_CONDITIONAL_ACCEPTANCE_READINESS.json";

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 9.4/Evaluate Conditional Wave 3 Acceptance Readiness")]
        public static void Evaluate()
        {
            var blockers = new List<string>();
            string capture = Path.Combine(Step20ProductionAssetWave03Step94RuntimeCapture.AbsoluteOutputRoot(), "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(capture) || File.ReadAllText(capture).IndexOf("\"captures\": 32", StringComparison.Ordinal) < 0)
                blockers.Add("RUNTIME-CAPTURE: 32/32 Step 9.4 refined Unity captures are required.");

            List<string> boundaryFailures = Step20ProductionAssetWave03Step94BoundaryGate.Validate(true);
            if (boundaryFailures.Count > 0 || !File.Exists(Step20ProductionAssetWave03Step94BoundaryGate.AbsolutePassLock()))
                blockers.Add("AUTHORITY: Step 9.4 Social/LiveOps boundary gate must pass.");

            Step20ProductionAssetWave03Step94VisualAcceptance.VisualGateResult visual = default(Step20ProductionAssetWave03Step94VisualAcceptance.VisualGateResult);
            bool visualExecuted = false;
            if (blockers.Count == 0)
            {
                visual = Step20ProductionAssetWave03Step94VisualAcceptance.AnalyzeInternal(false);
                visualExecuted = true;
                if (visual.approvedReferenceCount == 0) blockers.Add("REFERENCE-AUTHORITY: no direct approved Wave 3 references have been intaken.");
                if (visual.approvedReferenceFailures > 0) blockers.Add("VISUAL-FIDELITY: one or more approved-reference runtime gates failed.");
            }

            string decision;
            if (blockers.Count > 0) decision = "NO_GO_WAVE03_CONDITIONAL_ACCEPTANCE_READINESS";
            else if (visual.approvedReferenceCount < 8) decision = "ELIGIBLE_PARTIAL_WAVE03_ACCEPTANCE_READINESS";
            else decision = "ELIGIBLE_FULL_WAVE03_ACCEPTANCE_READINESS";

            WriteDecision(decision, visualExecuted ? visual.approvedReferenceCount : 0, visualExecuted ? visual.approvedReferencePasses : 0, blockers);
            string message = "v1.20 Step 9.4 conditional acceptance readiness: " + decision + ". This milestone never performs active Wave 3 cutover.";
            if (blockers.Count > 0) Debug.LogWarning(message); else Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message, "OK");
        }

        static void WriteDecision(string decision, int approvedCount, int approvedPasses, List<string> blockers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step09.4-wave03-conditional-acceptance-readiness\",");
            sb.AppendLine("  \"decision\": \"" + decision + "\",");
            sb.AppendLine("  \"directApprovedReferenceCount\": " + approvedCount + ",");
            sb.AppendLine("  \"approvedRuntimePasses\": " + approvedPasses + ",");
            sb.AppendLine("  \"fullWaveAuthorityTarget\": 8,");
            sb.AppendLine("  \"activeWave03CutoverPerformed\": false,");
            sb.AppendLine("  \"productionSocialBackendsActivated\": false,");
            sb.AppendLine("  \"blockers\": [");
            for(int i=0;i<blockers.Count;i++){sb.Append("    \"").Append(Escape(blockers[i])).Append("\"");if(i+1<blockers.Count)sb.Append(',');sb.AppendLine();}
            sb.AppendLine("  ]"); sb.AppendLine("}");
            string abs=Path.Combine(Directory.GetCurrentDirectory(),DecisionJson.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(Path.GetDirectoryName(abs));File.WriteAllText(abs,sb.ToString());AssetDatabase.Refresh();
        }
        static string Escape(string value){return (value??string.Empty).Replace("\\","\\\\").Replace("\"","\\\"");}
    }
}
#endif
