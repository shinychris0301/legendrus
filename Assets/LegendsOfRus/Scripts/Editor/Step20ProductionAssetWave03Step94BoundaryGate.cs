#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step94BoundaryGate
    {
        const string RelativeRoot = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence";
        const string PassLock = RelativeRoot + "/SOCIAL_LIVEOPS_BOUNDARY_PASS.lock";
        const string ResultJson = RelativeRoot + "/SOCIAL_LIVEOPS_BOUNDARY_RESULT.json";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.4 Social-LiveOps Boundary Verification")]
        public static void RunMenu()
        {
            List<string> failures = Validate(true);
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
        }

        public static List<string> Validate(bool writeEvidence)
        {
            List<string> failures = Step20ProductionAssetWave03Step92BoundaryGate.Validate(false);
            if (writeEvidence) WriteEvidence(failures);
            if (writeEvidence && failures.Count == 0)
            {
                Debug.Log("v1.20 Step 9.4 Social/LiveOps boundary PASS: Step 9.2 frozen-authority checks still hold for the refined candidates.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 9.4 Social/LiveOps boundary PASS. Refined Wave 3 candidates remain passive and backend authority remains unchanged.", "OK");
            }
            return failures;
        }

        public static string AbsolutePassLock()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), PassLock.Replace('/', Path.DirectorySeparatorChar));
        }

        static void WriteEvidence(List<string> failures)
        {
            string resultAbs = Path.Combine(Directory.GetCurrentDirectory(), ResultJson.Replace('/', Path.DirectorySeparatorChar));
            string passAbs = AbsolutePassLock();
            Directory.CreateDirectory(Path.GetDirectoryName(resultAbs));
            if (File.Exists(passAbs)) File.Delete(passAbs);
            string json = "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step09.4-wave03-social-liveops-boundary\",\n" +
                "  \"passed\": " + (failures.Count == 0 ? "true" : "false") + ",\n" +
                "  \"inheritsStep92FrozenAuthority\": true,\n" +
                "  \"candidatePages\": 8,\n" +
                "  \"activeWave03CutoverAuthorized\": false,\n" +
                "  \"failureCount\": " + failures.Count + "\n" +
                "}\n";
            File.WriteAllText(resultAbs, json);
            if (failures.Count == 0)
                File.WriteAllText(passAbs, "v1.20-step09.4-social-liveops-boundary-pass\nactive-wave03-cutover-authorized=false\n");
            AssetDatabase.Refresh();
        }
    }
}
#endif
