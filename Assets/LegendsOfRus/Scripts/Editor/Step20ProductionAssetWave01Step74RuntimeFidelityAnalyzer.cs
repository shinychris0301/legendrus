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
    public static class Step20ProductionAssetWave01Step74RuntimeFidelityAnalyzer
    {
        const string RefRoot = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens";
        const string MetricsCsv = "ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_FIDELITY_METRICS.csv";
        const string PassLock = "ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_VISUAL_QA_PASS.lock";
        const float CorrelationThreshold = 0.80f;
        const float MaeThreshold = 0.10f;

        static readonly ScreenId[] DirectReferencePages =
        {
            ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
            ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Victory, ScreenId.Fail, ScreenId.Dialogue
        };

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 7.4 Unity Runtime Capture Fidelity")]
        public static void Analyze()
        {
            string captureRoot = Step20ProductionAssetWave01Step74RuntimeCapture.AbsoluteOutputRoot();
            string executed = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(executed))
                throw new InvalidOperationException("Run the Step 7.4 Unity Runtime Four-Aspect Capture first.");

            var csv = new StringBuilder();
            csv.AppendLine("pageKey,correlation,mae,comparisonRegion,pass");
            var failures = new List<string>();

            foreach (ScreenId id in DirectReferencePages)
            {
                string capturePath = Path.Combine(captureRoot, id + "_941x1672.png");
                string referencePath = Path.Combine(Directory.GetCurrentDirectory(), RefRoot.Replace('/', Path.DirectorySeparatorChar), id + ".png");
                if (!File.Exists(capturePath) || !File.Exists(referencePath))
                {
                    failures.Add(id + ": missing 9:16 runtime capture or approved reference.");
                    continue;
                }

                Texture2D capture = LoadPng(capturePath);
                Texture2D reference = LoadPng(referencePath);
                bool shellOnly = id == ScreenId.Gameplay;
                Metrics metrics = Compare(capture, reference, shellOnly);
                UnityEngine.Object.DestroyImmediate(capture);
                UnityEngine.Object.DestroyImmediate(reference);

                bool pass = metrics.correlation >= CorrelationThreshold && metrics.mae <= MaeThreshold;
                if (!pass)
                    failures.Add(id + ": runtime fidelity proxy failed (corr=" + metrics.correlation.ToString("F4", CultureInfo.InvariantCulture) + ", mae=" + metrics.mae.ToString("F4", CultureInfo.InvariantCulture) + ").");

                csv.Append(id).Append(',')
                    .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(shellOnly ? "non-board-shell" : "full-page").Append(',')
                    .Append(pass ? "True" : "False").AppendLine();
            }

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), MetricsCsv.Replace('/', Path.DirectorySeparatorChar)), csv.ToString());
            string lockAbs = Path.Combine(Directory.GetCurrentDirectory(), PassLock.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(lockAbs)) File.Delete(lockAbs);

            if (failures.Count > 0)
            {
                Debug.LogWarning("v1.20 Step 7.4 runtime visual fidelity gate: NO-GO.\n" + string.Join(Environment.NewLine, failures.ToArray()));
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 runtime fidelity is NO-GO. Review UNITY_RUNTIME_FIDELITY_METRICS.csv; active cutover remains blocked.", "OK");
                return;
            }

            File.WriteAllText(lockAbs,
                "v1.20-step07.4-unity-runtime-visual-qa\n" +
                "direct-reference-pages=9/9\n" +
                "correlation-threshold=0.80\n" +
                "mae-threshold=0.10\n" +
                "gameplay-comparison=non-board-shell\n" +
                "active-runtime-cutover-modified=false\n");
            Debug.Log("v1.20 Step 7.4 runtime visual fidelity gate PASS: 9/9 direct-reference pages.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 runtime visual fidelity PASS: 9/9 direct-reference pages. Continue with map-state QA and Android signoff before cutover.", "OK");
        }

        static Texture2D LoadPng(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes, false))
                throw new InvalidOperationException("Could not decode PNG: " + path);
            return texture;
        }

        struct Metrics
        {
            public float correlation;
            public float mae;
        }

        static Metrics Compare(Texture2D a, Texture2D b, bool gameplayShellOnly)
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
                if (gameplayShellOnly && topY >= 357 && topY < 1299)
                    continue;

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

            if (samples <= 1)
                return new Metrics { correlation = 0f, mae = 1f };

            double n = samples;
            double cov = sumAB - (sumA * sumB / n);
            double va = sumAA - (sumA * sumA / n);
            double vb = sumBB - (sumB * sumB / n);
            double denom = Math.Sqrt(Math.Max(0d, va) * Math.Max(0d, vb));
            double corr = denom > 1e-12 ? cov / denom : 0d;
            return new Metrics
            {
                correlation = (float)corr,
                mae = (float)(mae / n)
            };
        }
    }
}
#endif
