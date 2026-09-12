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
    /// <summary>
    /// Step 9.2 runtime visual acceptance. Wave 3 has 0 direct approved references,
    /// therefore Events/Mail comparisons are informational only and cutover remains NO-GO.
    /// </summary>
    public static class Step20ProductionAssetWave03Step92VisualAcceptance
    {
        const string DemoRoot = "Assets/LegendsOfRus/Resources/DesignScreens";
        const string MetricsCsv = "ProjectDocs/V1_20_STEP09_2/UnityRuntimeCaptures/WAVE03_RUNTIME_VISUAL_METRICS.csv";
        const string DecisionPath = "ProjectDocs/V1_20_STEP09_2/UnityRuntimeCaptures/WAVE03_VISUAL_ACCEPTANCE_DECISION.json";

        static readonly DemoReference[] DemoReferences =
        {
            new DemoReference(ScreenId.Events, "Events.png"),
            new DemoReference(ScreenId.Mail, "Mail.png")
        };

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 9.2 Wave 3 Runtime Visual Acceptance")]
        public static void Analyze()
        {
            string captureRoot = Step20ProductionAssetWave03Step92RuntimeCapture.AbsoluteOutputRoot();
            string executed = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(executed))
                throw new InvalidOperationException("Run the Step 9.2 Wave 3 Unity Runtime Four-Aspect Capture first.");

            var csv = new StringBuilder();
            csv.AppendLine("pageKey,referenceClass,correlation,mae,gating,pass");
            foreach (DemoReference demo in DemoReferences)
            {
                Metrics metrics = ComparePage(demo.id, DemoRoot + "/" + demo.fileName, captureRoot);
                csv.Append(demo.id).Append(",DEMO_DESIGN_GUIDANCE,")
                    .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",false,INFORMATIONAL").AppendLine();
            }

            string metricsAbs = Path.Combine(Directory.GetCurrentDirectory(), MetricsCsv.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(metricsAbs));
            File.WriteAllText(metricsAbs, csv.ToString());

            var blockers = new List<string>
            {
                "REFERENCE-COVERAGE: 0/8 Wave 3 pages have direct approved production references.",
                "EVENTS-MAIL: existing DesignScreens are demo/composition guidance only and cannot authorize production cutover.",
                "STYLE-DERIVED: EventDetail, IndividualRanking, TeamHub, TeamRanking, TeamChat, and FriendsList have no direct page reference.",
                "BACKENDS: LiveOps, leaderboard, team/chat/friends, and mail production authorities remain intentionally inactive.",
                "CUTOVER-SCOPE: Step 9.2 is runtime QA/acceptance evidence only; no Wave 3 active cutover is authorized."
            };
            WriteDecision(blockers);

            Debug.LogWarning("v1.20 Step 9.2 visual acceptance: NO-GO for Wave 3 cutover because direct approved reference coverage is 0/8. Events/Mail metrics are informational only.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 9.2 runtime visual analysis completed. Wave 3 cutover remains NO-GO: 0/8 pages have direct approved references. Events/Mail comparisons are informational only.", "OK");
        }

        static Metrics ComparePage(ScreenId id, string referenceProjectPath, string captureRoot)
        {
            string capturePath = Path.Combine(captureRoot, id + "_941x1672.png");
            string referencePath = Path.Combine(Directory.GetCurrentDirectory(), referenceProjectPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(capturePath)) throw new InvalidOperationException("Missing 9:16 runtime capture: " + capturePath);
            if (!File.Exists(referencePath)) throw new InvalidOperationException("Missing demo design guidance: " + referenceProjectPath);

            Texture2D capture = LoadPng(capturePath);
            Texture2D reference = LoadPng(referencePath);
            try { return Compare(capture, reference); }
            finally
            {
                UnityEngine.Object.DestroyImmediate(capture);
                UnityEngine.Object.DestroyImmediate(reference);
            }
        }

        static void WriteDecision(List<string> blockers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step09.2-wave03-runtime-visual-acceptance\",");
            sb.AppendLine("  \"decision\": \"NO_GO_WAVE03_CUTOVER_REFERENCE_AUTHORITY_MISSING\",");
            sb.AppendLine("  \"activeWave03CutoverPerformed\": false,");
            sb.AppendLine("  \"directApprovedReferencePages\": 0,");
            sb.AppendLine("  \"demoDesignGuidancePages\": 2,");
            sb.AppendLine("  \"styleSystemDerivedPages\": 6,");
            sb.AppendLine("  \"runtimeCaptureRequired\": 32,");
            sb.AppendLine("  \"demoMetricsAreGating\": false,");
            sb.AppendLine("  \"blockers\": [");
            for (int i = 0; i < blockers.Count; i++)
            {
                sb.Append("    \"").Append(Escape(blockers[i])).Append("\"");
                if (i + 1 < blockers.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            string abs = Path.Combine(Directory.GetCurrentDirectory(), DecisionPath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(abs));
            File.WriteAllText(abs, sb.ToString());
            AssetDatabase.Refresh();
        }

        static Texture2D LoadPng(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path), false))
                throw new InvalidOperationException("Could not decode PNG: " + path);
            return texture;
        }

        struct Metrics { public float correlation; public float mae; }

        static Metrics Compare(Texture2D a, Texture2D b)
        {
            if (a.width != b.width || a.height != b.height)
                throw new InvalidOperationException("Runtime/reference size mismatch: " + a.width + "x" + a.height + " vs " + b.width + "x" + b.height + ".");

            Color32[] pa = a.GetPixels32();
            Color32[] pb = b.GetPixels32();
            int width = a.width;
            int height = a.height;
            const int stride = 4;
            double sumA = 0d, sumB = 0d, sumAA = 0d, sumBB = 0d, sumAB = 0d, mae = 0d;
            long samples = 0;

            for (int topY = 0; topY < height; topY += stride)
            {
                int pixelY = height - 1 - topY;
                int row = pixelY * width;
                for (int x = 0; x < width; x += stride)
                {
                    Color32 ca = pa[row + x];
                    Color32 cb = pb[row + x];
                    double la = (0.2126 * ca.r + 0.7152 * ca.g + 0.0722 * ca.b) / 255d;
                    double lb = (0.2126 * cb.r + 0.7152 * cb.g + 0.0722 * cb.b) / 255d;
                    sumA += la; sumB += lb; sumAA += la * la; sumBB += lb * lb; sumAB += la * lb;
                    mae += (Math.Abs(ca.r - cb.r) + Math.Abs(ca.g - cb.g) + Math.Abs(ca.b - cb.b)) / (3d * 255d);
                    samples++;
                }
            }

            if (samples <= 1) return new Metrics { correlation = 0f, mae = 1f };
            double n = samples;
            double cov = sumAB - (sumA * sumB / n);
            double va = sumAA - (sumA * sumA / n);
            double vb = sumBB - (sumB * sumB / n);
            double denom = Math.Sqrt(Math.Max(0d, va) * Math.Max(0d, vb));
            return new Metrics
            {
                correlation = (float)(denom > 1e-12 ? cov / denom : 0d),
                mae = (float)(mae / n)
            };
        }

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        readonly struct DemoReference
        {
            public readonly ScreenId id;
            public readonly string fileName;
            public DemoReference(ScreenId id, string fileName) { this.id = id; this.fileName = fileName; }
        }
    }
}
#endif
