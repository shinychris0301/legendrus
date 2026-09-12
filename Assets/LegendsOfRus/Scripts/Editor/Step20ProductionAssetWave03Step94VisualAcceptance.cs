#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step94VisualAcceptance
    {
        const float MinCorrelation = 0.80f;
        const float MaxMae = 0.10f;
        const string DemoRoot = "Assets/LegendsOfRus/Resources/DesignScreens";
        const string MetricsCsv = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence/WAVE03_RUNTIME_VISUAL_METRICS.csv";
        const string DecisionJson = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence/WAVE03_RUNTIME_VISUAL_ACCEPTANCE.json";
        const string PassLockRoot = "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence/ApprovedReferencePassLocks";

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 9.4 Wave 3 Refined Runtime Visual Acceptance")]
        public static void AnalyzeMenu()
        {
            VisualGateResult result = AnalyzeInternal(true);
            if (result.approvedReferenceFailures > 0)
                throw new InvalidOperationException("One or more direct approved Wave 3 references failed the runtime fidelity gate. See Step 9.4 visual acceptance evidence.");
        }

        public static VisualGateResult AnalyzeInternal(bool showDialog)
        {
            string captureRoot = Step20ProductionAssetWave03Step94RuntimeCapture.AbsoluteOutputRoot();
            string executed = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(executed)) throw new InvalidOperationException("Run the Step 9.4 refined Wave 3 runtime capture first.");

            List<string> intakeFailures;
            List<Step20ProductionAssetWave03Step94ReferenceAuthorityIntake.IntakeEntry> approved =
                Step20ProductionAssetWave03Step94ReferenceAuthorityIntake.WriteIntake(out intakeFailures);
            if (intakeFailures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, intakeFailures.ToArray()));

            string locksAbs = Absolute(PassLockRoot);
            if (Directory.Exists(locksAbs)) Directory.Delete(locksAbs, true);
            Directory.CreateDirectory(locksAbs);

            int approvedPasses = 0;
            int approvedFailures = 0;
            var csv = new StringBuilder();
            csv.AppendLine("pageKey,referenceClass,correlation,mae,minCorrelation,maxMae,gating,result");
            foreach (var entry in approved)
            {
                Metrics metrics = ComparePage(entry.id, entry.projectPath, captureRoot);
                bool pass = metrics.correlation >= MinCorrelation && metrics.mae <= MaxMae;
                if (pass)
                {
                    approvedPasses++;
                    File.WriteAllText(Path.Combine(locksAbs, entry.pageKey + ".lock"),
                        "v1.20-step09.4-approved-reference-runtime-pass\npage=" + entry.pageKey + "\nreference-sha256=" + entry.sha256 + "\n");
                }
                else approvedFailures++;
                csv.Append(entry.pageKey).Append(",DIRECT_APPROVED,")
                    .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(MinCorrelation.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                    .Append(MaxMae.ToString("F2", CultureInfo.InvariantCulture)).Append(",true,")
                    .Append(pass ? "PASS" : "FAIL").AppendLine();
            }

            AppendDemoMetric(csv, ScreenId.Events, "EventsHub", "Events.png", captureRoot, approved);
            AppendDemoMetric(csv, ScreenId.Mail, "Mail", "Mail.png", captureRoot, approved);
            File.WriteAllText(Absolute(MetricsCsv), csv.ToString());

            string decision;
            if (approved.Count == 0) decision = "NO_GO_WAVE03_ACCEPTANCE_REFERENCE_AUTHORITY_MISSING";
            else if (approvedFailures > 0) decision = "NO_GO_WAVE03_APPROVED_REFERENCE_RUNTIME_FIDELITY";
            else if (approved.Count < 8) decision = "ELIGIBLE_PARTIAL_WAVE03_ACCEPTANCE_READINESS";
            else decision = "ELIGIBLE_FULL_WAVE03_ACCEPTANCE_READINESS";

            var result = new VisualGateResult(approved.Count, approvedPasses, approvedFailures, decision);
            WriteDecision(result);
            AssetDatabase.Refresh();
            string message = "Step 9.4 runtime visual gate: " + decision + ". Approved authority=" + approved.Count + "/8, approved runtime passes=" + approvedPasses + "/" + approved.Count + ". Demo guidance remains non-gating.";
            if (approvedFailures > 0) Debug.LogError(message); else Debug.Log(message);
            if (showDialog && !Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message, "OK");
            return result;
        }

        static void AppendDemoMetric(StringBuilder csv, ScreenId id, string pageKey, string fileName, string captureRoot,
            List<Step20ProductionAssetWave03Step94ReferenceAuthorityIntake.IntakeEntry> approved)
        {
            bool hasApproved = false;
            foreach (var e in approved) if (e.id == id) { hasApproved = true; break; }
            string projectPath = DemoRoot + "/" + fileName;
            if (!File.Exists(Absolute(projectPath))) return;
            Metrics metrics = ComparePage(id, projectPath, captureRoot);
            csv.Append(pageKey).Append(",DEMO_DESIGN_GUIDANCE,")
                .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",,,false,")
                .Append(hasApproved ? "INFORMATIONAL_SUPERSEDED_BY_APPROVED" : "INFORMATIONAL").AppendLine();
        }

        static Metrics ComparePage(ScreenId id, string referenceProjectPath, string captureRoot)
        {
            string capturePath = Path.Combine(captureRoot, id + "_941x1672.png");
            string referencePath = Absolute(referenceProjectPath);
            if (!File.Exists(capturePath)) throw new InvalidOperationException("Missing 9:16 runtime capture: " + capturePath);
            if (!File.Exists(referencePath)) throw new InvalidOperationException("Missing reference: " + referenceProjectPath);
            Texture2D capture = LoadPng(capturePath); Texture2D reference = LoadPng(referencePath);
            try { return Compare(capture, reference); }
            finally { UnityEngine.Object.DestroyImmediate(capture); UnityEngine.Object.DestroyImmediate(reference); }
        }

        static void WriteDecision(VisualGateResult result)
        {
            string json = "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step09.4-wave03-refined-runtime-visual-acceptance\",\n" +
                "  \"decision\": \"" + result.decision + "\",\n" +
                "  \"directApprovedReferencePages\": " + result.approvedReferenceCount + ",\n" +
                "  \"approvedReferenceRuntimePasses\": " + result.approvedReferencePasses + ",\n" +
                "  \"approvedReferenceRuntimeFailures\": " + result.approvedReferenceFailures + ",\n" +
                "  \"demoDesignGuidancePages\": 2,\n" +
                "  \"demoMetricsAreGating\": false,\n" +
                "  \"fullWaveApprovalRequiresApprovedReferences\": 8,\n" +
                "  \"activeWave03CutoverPerformed\": false\n" +
                "}\n";
            File.WriteAllText(Absolute(DecisionJson), json);
        }

        static Texture2D LoadPng(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path), false)) throw new InvalidOperationException("Could not decode PNG: " + path);
            return texture;
        }
        struct Metrics { public float correlation; public float mae; }
        static Metrics Compare(Texture2D a, Texture2D b)
        {
            if (a.width != b.width || a.height != b.height)
                throw new InvalidOperationException("Runtime/reference size mismatch: " + a.width + "x" + a.height + " vs " + b.width + "x" + b.height + ".");
            Color32[] pa = a.GetPixels32(); Color32[] pb = b.GetPixels32(); int width=a.width, height=a.height; const int stride=4;
            double sumA=0d,sumB=0d,sumAA=0d,sumBB=0d,sumAB=0d,mae=0d; long samples=0;
            for(int topY=0;topY<height;topY+=stride){int pixelY=height-1-topY;int row=pixelY*width;for(int x=0;x<width;x+=stride){Color32 ca=pa[row+x],cb=pb[row+x];double la=(0.2126*ca.r+0.7152*ca.g+0.0722*ca.b)/255d;double lb=(0.2126*cb.r+0.7152*cb.g+0.0722*cb.b)/255d;sumA+=la;sumB+=lb;sumAA+=la*la;sumBB+=lb*lb;sumAB+=la*lb;mae+=(Math.Abs(ca.r-cb.r)+Math.Abs(ca.g-cb.g)+Math.Abs(ca.b-cb.b))/(3d*255d);samples++;}}
            if(samples<=1)return new Metrics{correlation=0f,mae=1f};double n=samples,cov=sumAB-(sumA*sumB/n),va=sumAA-(sumA*sumA/n),vb=sumBB-(sumB*sumB/n),denom=Math.Sqrt(Math.Max(0d,va)*Math.Max(0d,vb));
            return new Metrics{correlation=(float)(denom>1e-12?cov/denom:0d),mae=(float)(mae/n)};
        }
        static string Absolute(string path){return Path.Combine(Directory.GetCurrentDirectory(),path.Replace('/',Path.DirectorySeparatorChar));}

        public readonly struct VisualGateResult
        {
            public readonly int approvedReferenceCount, approvedReferencePasses, approvedReferenceFailures; public readonly string decision;
            public VisualGateResult(int count,int passes,int failures,string decision){approvedReferenceCount=count;approvedReferencePasses=passes;approvedReferenceFailures=failures;this.decision=decision;}
        }
    }
}
#endif
