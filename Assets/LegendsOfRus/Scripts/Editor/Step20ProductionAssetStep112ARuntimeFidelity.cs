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
    public static class Step20ProductionAssetStep112ARuntimeFidelity
    {
        public const string MetricsCsv = "ProjectDocs/V1_20_STEP11_2A/RuntimeCloseout/PAUSE_CHAPTER_COMPLETE_RUNTIME_FIDELITY.csv";
        public const string MetricsJson = "ProjectDocs/V1_20_STEP11_2A/RuntimeCloseout/PAUSE_CHAPTER_COMPLETE_RUNTIME_FIDELITY.json";
        public const string PassLock = "ProjectDocs/V1_20_STEP11_2A/RuntimeCloseout/PAUSE_CHAPTER_COMPLETE_RUNTIME_FIDELITY_PASS.lock";
        public const string NoGoReport = "ProjectDocs/V1_20_STEP11_2A/RuntimeCloseout/PAUSE_CHAPTER_COMPLETE_RUNTIME_FIDELITY_NO_GO.md";
        const float MinCorrelation = 0.80f;
        const float MaxMae = 0.10f;

        static readonly ScreenId[] Pages = { ScreenId.Pause, ScreenId.ChapterComplete };

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 11.2A Pause + Chapter Complete Runtime Fidelity")]
        public static void AnalyzeMenu()
        {
            AnalysisResult result = AnalyzeAndWrite();
            string message = "Step 11.2A Pause + ChapterComplete runtime fidelity: " + result.decision + ". PASS=" + result.passCount + "/2.";
            if (result.passCount == 2) Debug.Log(message); else Debug.LogWarning(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message + "\n\nSee PAUSE_CHAPTER_COMPLETE_RUNTIME_FIDELITY.csv for actual runtime metrics.", "OK");
        }

        public static AnalysisResult AnalyzeAndWrite()
        {
            string root = Step20ProductionAssetStep112ARuntimeCloseout.AbsoluteOutputRoot();
            string manifest = Path.Combine(root, Step20ProductionAssetStep112ARuntimeCloseout.ManifestName);
            if (!File.Exists(manifest))
                throw new InvalidOperationException("Run the Step 11.2A focused runtime capture first.");

            var csv = new StringBuilder();
            csv.AppendLine("page,correlation,mae,pass");
            var rows = new List<Row>();
            int passCount = 0;

            foreach (ScreenId id in Pages)
            {
                string capturePath = Path.Combine(root, id + "_941x1672.png");
                string referencePath = Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id));
                if (!File.Exists(capturePath) || !File.Exists(referencePath))
                    throw new InvalidOperationException(id + ": focused runtime capture or approved reference missing.");

                Texture2D capture = LoadPng(capturePath);
                Texture2D reference = LoadPng(referencePath);
                Step20ProductionAssetStep103Evidence.Metrics metrics;
                try { metrics = Compare(capture, reference); }
                finally { UnityEngine.Object.DestroyImmediate(capture); UnityEngine.Object.DestroyImmediate(reference); }

                bool pass = metrics.correlation >= MinCorrelation && metrics.mae <= MaxMae;
                if (pass) passCount++;
                rows.Add(new Row(id, metrics.correlation, metrics.mae, pass));
                csv.Append(id).Append(',').Append(F(metrics.correlation)).Append(',').Append(F(metrics.mae)).Append(',').Append(pass ? "True" : "False").AppendLine();
            }

            string decision = passCount == 2 ? "PASS_ACTIVE_WAVE1_REFERENCE_RUNTIME_CLOSEOUT" : "NO_GO_RUNTIME_FIDELITY_REFINEMENT_REQUIRED";
            Directory.CreateDirectory(root);
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(MetricsCsv), csv.ToString());

            var json = new StringBuilder();
            json.AppendLine("{").AppendLine("  \"schemaVersion\": 1,")
                .AppendLine("  \"milestone\": \"v1.20-step11.2a-pause-chapter-complete-runtime-fidelity\",")
                .AppendLine("  \"decision\": \"" + decision + "\",")
                .AppendLine("  \"correlationThreshold\": 0.80,")
                .AppendLine("  \"maeThreshold\": 0.10,")
                .AppendLine("  \"passCount\": " + passCount + ",")
                .AppendLine("  \"pages\": [");
            for (int i = 0; i < rows.Count; i++)
            {
                Row r = rows[i];
                json.Append("    { \"page\": \"").Append(r.id).Append("\", \"correlation\": ").Append(F(r.correlation)).Append(", \"mae\": ").Append(F(r.mae)).Append(", \"pass\": ").Append(r.pass ? "true" : "false").Append(" }");
                json.AppendLine(i + 1 < rows.Count ? "," : string.Empty);
            }
            json.AppendLine("  ],").AppendLine("  \"productionRoutingModified\": false").AppendLine("}");
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(MetricsJson), json.ToString());

            string lockAbs = Step20ProductionAssetStep103Evidence.Absolute(PassLock);
            string noGoAbs = Step20ProductionAssetStep103Evidence.Absolute(NoGoReport);
            if (File.Exists(lockAbs)) File.Delete(lockAbs);
            if (File.Exists(noGoAbs)) File.Delete(noGoAbs);

            if (passCount == 2)
            {
                File.WriteAllText(lockAbs,
                    "v1.20-step11.2a-pause-chapter-complete-runtime-fidelity-pass\n" +
                    "pages=Pause,ChapterComplete\n" +
                    "correlation-threshold=0.80\n" +
                    "mae-threshold=0.10\n" +
                    "production-routing-modified=false\n");
            }
            else
            {
                var md = new StringBuilder();
                md.AppendLine("# Step 11.2A Runtime Closeout — NO-GO").AppendLine();
                md.AppendLine("One or both active Wave 1 pages failed runtime fidelity against the newly approved direct references.").AppendLine();
                md.AppendLine("| Page | Correlation | MAE | Pass |").AppendLine("|---|---:|---:|---|");
                for (int i = 0; i < rows.Count; i++)
                    md.AppendLine("| " + rows[i].id + " | " + F(rows[i].correlation) + " | " + F(rows[i].mae) + " | " + (rows[i].pass ? "PASS" : "FAIL") + " |");
                md.AppendLine().AppendLine("Do not alter the approved references to make this test pass. Refine the screenshot-free runtime candidate prefab/assets, then rerun the focused 8-capture closeout.");
                File.WriteAllText(noGoAbs, md.ToString());
            }

            Step20ProductionAssetStep112RouteExpansion.EvaluateAndWrite();
            AssetDatabase.Refresh();
            return new AnalysisResult(decision, passCount);
        }

        static Step20ProductionAssetStep103Evidence.Metrics Compare(Texture2D a, Texture2D b)
        {
            if (a.width != b.width || a.height != b.height)
                throw new InvalidOperationException("Runtime/reference size mismatch: " + a.width + "x" + a.height + " vs " + b.width + "x" + b.height + ".");
            Color32[] pa = a.GetPixels32(), pb = b.GetPixels32();
            int width = a.width, height = a.height; const int stride = 4;
            double sumA=0d,sumB=0d,sumAA=0d,sumBB=0d,sumAB=0d,mae=0d; long samples=0;
            for (int topY=0; topY<height; topY+=stride)
            {
                int py=height-1-topY,row=py*width;
                for (int x=0;x<width;x+=stride)
                {
                    Color32 ca=pa[row+x], cb=pb[row+x];
                    double la=(0.2126*ca.r+0.7152*ca.g+0.0722*ca.b)/255d, lb=(0.2126*cb.r+0.7152*cb.g+0.0722*cb.b)/255d;
                    sumA+=la;sumB+=lb;sumAA+=la*la;sumBB+=lb*lb;sumAB+=la*lb;
                    mae+=(Math.Abs(ca.r-cb.r)+Math.Abs(ca.g-cb.g)+Math.Abs(ca.b-cb.b))/(3d*255d);samples++;
                }
            }
            double n=samples,cov=sumAB-(sumA*sumB/n),va=sumAA-(sumA*sumA/n),vb=sumBB-(sumB*sumB/n),den=Math.Sqrt(Math.Max(0d,va)*Math.Max(0d,vb));
            return new Step20ProductionAssetStep103Evidence.Metrics { correlation=(float)(den>1e-12?cov/den:0d), mae=(float)(mae/n) };
        }

        static Texture2D LoadPng(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path), false)) throw new InvalidOperationException("Could not decode PNG: " + path);
            return texture;
        }
        static string F(float value) { return value.ToString("F4", CultureInfo.InvariantCulture); }

        readonly struct Row
        {
            public readonly ScreenId id; public readonly float correlation, mae; public readonly bool pass;
            public Row(ScreenId id,float correlation,float mae,bool pass){this.id=id;this.correlation=correlation;this.mae=mae;this.pass=pass;}
        }
        public readonly struct AnalysisResult
        {
            public readonly string decision; public readonly int passCount;
            public AnalysisResult(string decision,int passCount){this.decision=decision;this.passCount=passCount;}
        }
    }
}
#endif
