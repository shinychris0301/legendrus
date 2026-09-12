#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Step 8.4 re-verification wrapper around the accepted Step 8.2 authority
    /// freeze. Produces milestone-local evidence without changing providers,
    /// economy, navigation, or candidate interaction behavior.
    /// </summary>
    public static class Step20ProductionAssetWave02Step84ProviderBoundaryGate
    {
        const string ReportPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_PROVIDER_BOUNDARY_GATE.json";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.4 Provider-Boundary Gate")]
        public static void Run()
        {
            List<string> failures = Validate(true);
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.4 provider-boundary gate PASS. Accepted purchase/ad/reward authority is unchanged and Wave 2 candidates remain presentation-only.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.4 provider-boundary gate PASS. No Wave 2 candidate can directly purchase, grant, reward, or mutate economy state.", "OK");
        }

        public static List<string> Validate(bool writeReport)
        {
            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));
            failures.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());

            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop))
                failures.Add("Shop is already in the active production registry; Step 8.4 package must remain pre-cutover.");
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Accepted active production registry no longer contains exactly 10 routes.");

            if (writeReport) WriteReport(failures);
            return failures;
        }

        static void WriteReport(List<string> failures)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.4-provider-boundary-gate\",");
            sb.AppendLine("  \"result\": \"" + (failures.Count == 0 ? "PASS" : "FAIL") + "\",");
            sb.AppendLine("  \"criticalRuntimeAuthorityFilesFrozen\": " + (failures.Count == 0 ? "true" : "false") + ",");
            sb.AppendLine("  \"shopActiveBeforeGate\": false,");
            sb.AppendLine("  \"candidateButtonsAllowed\": false,");
            sb.AppendLine("  \"directCandidateEconomyMutationAllowed\": false,");
            sb.AppendLine("  \"fullWave02CutoverAuthorized\": false,");
            sb.AppendLine("  \"failures\": [");
            for (int i = 0; i < failures.Count; i++)
            {
                sb.Append("    \"").Append(Escape(failures[i])).Append("\"");
                if (i + 1 < failures.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(ReportPath, sb.ToString());
            AssetDatabase.ImportAsset(ReportPath, ImportAssetOptions.ForceUpdate);
        }

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
