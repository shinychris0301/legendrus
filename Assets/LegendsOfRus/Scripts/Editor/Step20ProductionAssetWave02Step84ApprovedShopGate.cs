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
    /// Strict runtime-render gate for the only Wave 2 page with direct approved
    /// visual authority: Shop. Other Wave 2 pages are reported only as guidance.
    /// Passing this gate does not edit the active production registry or prefab set.
    /// </summary>
    public static class Step20ProductionAssetWave02Step84ApprovedShopGate
    {
        const string ApprovedShopPath = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Shop.png";
        const string DemoRoot = "Assets/LegendsOfRus/Resources/DesignScreens";
        const string MetricsCsv = "ProjectDocs/V1_20_STEP08_4/UnityRuntimeCaptures/WAVE02_RUNTIME_VISUAL_METRICS.csv";
        const string ShopPassLock = "ProjectDocs/V1_20_STEP08_4/UnityRuntimeCaptures/SHOP_APPROVED_REFERENCE_RUNTIME_PASS.lock";
        const string ResultPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_APPROVED_SHOP_RUNTIME_GATE.json";
        const float CorrelationThreshold = 0.80f;
        const float MaeThreshold = 0.10f;

        static readonly ScreenId[] DemoReferencePages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile,
            ScreenId.OutOfEnergy, ScreenId.BoosterSelect, ScreenId.RewardClaim
        };

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 8.4 Approved Shop Runtime Gate")]
        public static void Analyze()
        {
            string captureRoot = Step20ProductionAssetWave02Step84RuntimeCapture.AbsoluteOutputRoot();
            string executed = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(executed))
                throw new InvalidOperationException("Run the Step 8.4 Wave 2 Unity Runtime Four-Aspect Capture first.");

            var csv = new StringBuilder();
            csv.AppendLine("pageKey,referenceClass,correlation,mae,gating,pass");

            Metrics shop = ComparePage(ScreenId.Shop, ApprovedShopPath, captureRoot);
            bool shopPass = shop.correlation >= CorrelationThreshold && shop.mae <= MaeThreshold;
            csv.Append("Shop,DIRECT_APPROVED,")
                .Append(shop.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                .Append(shop.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",true,")
                .Append(shopPass ? "True" : "False").AppendLine();

            foreach (ScreenId id in DemoReferencePages)
            {
                string demoPath = DemoRoot + "/" + id + ".png";
                if (!File.Exists(demoPath))
                {
                    csv.Append(id).Append(",DEMO_DESIGN_REFERENCE,NA,NA,false,MISSING_GUIDANCE").AppendLine();
                    continue;
                }
                Metrics metrics = ComparePage(id, demoPath, captureRoot);
                csv.Append(id).Append(",DEMO_DESIGN_REFERENCE,")
                    .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",false,INFORMATIONAL").AppendLine();
            }

            string metricsAbs = ProjectAbsolute(MetricsCsv);
            Directory.CreateDirectory(Path.GetDirectoryName(metricsAbs));
            File.WriteAllText(metricsAbs, csv.ToString());

            string lockAbs = ProjectAbsolute(ShopPassLock);
            if (File.Exists(lockAbs)) File.Delete(lockAbs);
            if (shopPass)
            {
                File.WriteAllText(lockAbs,
                    "v1.20-step08.4-shop-approved-reference-runtime-pass\n" +
                    "correlation=" + shop.correlation.ToString("F4", CultureInfo.InvariantCulture) + "\n" +
                    "mae=" + shop.mae.ToString("F4", CultureInfo.InvariantCulture) + "\n" +
                    "correlation-threshold=0.80\n" +
                    "mae-threshold=0.10\n" +
                    "active-cutover-performed=false\n" +
                    "full-wave02-cutover-authorized=false\n");
            }

            WriteResult(shop, shopPass);
            AssetDatabase.Refresh();

            string summary = "v1.20 Step 8.4 Approved Shop runtime gate=" + (shopPass ? "PASS" : "FAIL") +
                " (corr=" + shop.correlation.ToString("F4", CultureInfo.InvariantCulture) +
                ", mae=" + shop.mae.ToString("F4", CultureInfo.InvariantCulture) + ").";
            if (shopPass) Debug.Log(summary);
            else Debug.LogWarning(summary);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", summary + " Full Wave 2 cutover remains unauthorized because direct approved reference coverage is 1/9.", "OK");
        }

        public static bool HasPassingLock()
        {
            string lockAbs = ProjectAbsolute(ShopPassLock);
            return File.Exists(lockAbs) && File.ReadAllText(lockAbs).StartsWith("v1.20-step08.4-shop-approved-reference-runtime-pass", StringComparison.Ordinal);
        }

        static Metrics ComparePage(ScreenId id, string referenceProjectPath, string captureRoot)
        {
            string capturePath = Path.Combine(captureRoot, id + "_941x1672.png");
            string referencePath = ProjectAbsolute(referenceProjectPath);
            if (!File.Exists(capturePath)) throw new InvalidOperationException("Missing 9:16 runtime capture: " + capturePath);
            if (!File.Exists(referencePath)) throw new InvalidOperationException("Missing visual reference: " + referenceProjectPath);

            Texture2D capture = LoadPng(capturePath);
            Texture2D reference = LoadPng(referencePath);
            try { return Compare(capture, reference); }
            finally
            {
                UnityEngine.Object.DestroyImmediate(capture);
                UnityEngine.Object.DestroyImmediate(reference);
            }
        }

        static void WriteResult(Metrics shop, bool pass)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.4-approved-shop-runtime-gate\",");
            sb.AppendLine("  \"referenceAuthority\": \"DIRECT_APPROVED\",");
            sb.AppendLine("  \"shopRuntimeStrictPass\": " + (pass ? "true" : "false") + ",");
            sb.AppendLine("  \"shopCorrelation\": " + shop.correlation.ToString("F4", CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"shopMae\": " + shop.mae.ToString("F4", CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"thresholdCorrelation\": 0.80,");
            sb.AppendLine("  \"thresholdMae\": 0.10,");
            sb.AppendLine("  \"activeCutoverPerformed\": false,");
            sb.AppendLine("  \"fullWave02CutoverAuthorized\": false,");
            sb.AppendLine("  \"otherWave02PagesProductionApprovedByThisGate\": 0");
            sb.AppendLine("}");
            File.WriteAllText(ResultPath, sb.ToString());
            AssetDatabase.ImportAsset(ResultPath, ImportAssetOptions.ForceUpdate);
        }

        static Texture2D LoadPng(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes, false))
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

        static string ProjectAbsolute(string projectRelativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), projectRelativePath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
