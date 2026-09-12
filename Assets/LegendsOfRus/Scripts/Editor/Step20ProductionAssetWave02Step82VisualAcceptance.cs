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
    /// Runtime-render visual acceptance for Step 8.2. Shop is the only Wave 2 page
    /// with a direct approved reference and therefore the only strict fidelity gate.
    /// Six DesignScreens comparisons are informational; Inventory/SpecialOffer have
    /// no direct page reference and cannot be called visually approved here.
    /// </summary>
    public static class Step20ProductionAssetWave02Step82VisualAcceptance
    {
        const string ApprovedRoot = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens";
        const string DemoRoot = "Assets/LegendsOfRus/Resources/DesignScreens";
        const string MetricsCsv = "ProjectDocs/V1_20_STEP08_2/UnityRuntimeCaptures/WAVE02_RUNTIME_VISUAL_METRICS.csv";
        const string ShopPassLock = "ProjectDocs/V1_20_STEP08_2/UnityRuntimeCaptures/SHOP_APPROVED_REFERENCE_RUNTIME_PASS.lock";
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_2_VISUAL_ACCEPTANCE_DECISION.json";
        const float CorrelationThreshold = 0.80f;
        const float MaeThreshold = 0.10f;

        static readonly ScreenId[] DemoReferencePages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile,
            ScreenId.OutOfEnergy, ScreenId.BoosterSelect, ScreenId.RewardClaim
        };

        [MenuItem("Legends of Rus/Tests/Analyze v1.20 Step 8.2 Wave 2 Runtime Visual Acceptance")]
        public static void Analyze()
        {
            string captureRoot = Step20ProductionAssetWave02Step82RuntimeCapture.AbsoluteOutputRoot();
            string executed = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (!File.Exists(executed))
                throw new InvalidOperationException("Run the Step 8.2 Wave 2 Unity Runtime Four-Aspect Capture first.");

            var csv = new StringBuilder();
            csv.AppendLine("pageKey,referenceClass,correlation,mae,gating,pass");
            var blockers = new List<string>();

            Metrics shop = ComparePage(ScreenId.Shop, ApprovedRoot + "/Shop.png", captureRoot);
            bool shopPass = shop.correlation >= CorrelationThreshold && shop.mae <= MaeThreshold;
            csv.Append("Shop,DIRECT_APPROVED,")
                .Append(shop.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                .Append(shop.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",true,")
                .Append(shopPass ? "True" : "False").AppendLine();
            if (!shopPass)
                blockers.Add("SHOP-DIRECT-REFERENCE: runtime fidelity failed (corr=" + shop.correlation.ToString("F4", CultureInfo.InvariantCulture) + ", mae=" + shop.mae.ToString("F4", CultureInfo.InvariantCulture) + ").");

            foreach (ScreenId id in DemoReferencePages)
            {
                Metrics metrics = ComparePage(id, DemoRoot + "/" + id + ".png", captureRoot);
                csv.Append(id).Append(",DEMO_DESIGN_REFERENCE,")
                    .Append(metrics.correlation.ToString("F4", CultureInfo.InvariantCulture)).Append(',')
                    .Append(metrics.mae.ToString("F4", CultureInfo.InvariantCulture)).Append(",false,INFORMATIONAL").AppendLine();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), MetricsCsv.Replace('/', Path.DirectorySeparatorChar))));
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), MetricsCsv.Replace('/', Path.DirectorySeparatorChar)), csv.ToString());
            string shopLockAbs = Path.Combine(Directory.GetCurrentDirectory(), ShopPassLock.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(shopLockAbs)) File.Delete(shopLockAbs);
            if (shopPass)
            {
                File.WriteAllText(shopLockAbs,
                    "v1.20-step08.2-shop-approved-reference-runtime-pass\n" +
                    "correlation-threshold=0.80\n" +
                    "mae-threshold=0.10\n" +
                    "wave02-cutover-authorized=false\n");
            }

            blockers.Add("REFERENCE-COVERAGE: only Shop has a direct approved Wave 2 page reference; six pages are demo-reference only and Inventory/SpecialOffer are style-system derived.");
            blockers.Add("CUTOVER-SCOPE: Step 8.2 validates candidates only; no Wave 2 active cutover is authorized by this milestone.");
            WriteDecision(blockers, shop, shopPass);

            Debug.LogWarning("v1.20 Step 8.2 visual acceptance: NO-GO for full Wave 2 cutover. Shop strict runtime pass=" + shopPass + ". Review " + MetricsCsv + " and the decision JSON.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.2 runtime visual analysis completed. Full Wave 2 cutover remains NO-GO because direct approved reference coverage is incomplete; Shop strict pass=" + shopPass + ".", "OK");
        }

        static Metrics ComparePage(ScreenId id, string referenceProjectPath, string captureRoot)
        {
            string capturePath = Path.Combine(captureRoot, id + "_941x1672.png");
            string referencePath = Path.Combine(Directory.GetCurrentDirectory(), referenceProjectPath.Replace('/', Path.DirectorySeparatorChar));
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

        static void WriteDecision(List<string> blockers, Metrics shop, bool shopPass)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.2-wave02-runtime-visual-acceptance\",");
            sb.AppendLine("  \"decision\": \"NO_GO_FULL_WAVE02_CUTOVER\",");
            sb.AppendLine("  \"activeWave02CutoverPerformed\": false,");
            sb.AppendLine("  \"directApprovedReferencePages\": 1,");
            sb.AppendLine("  \"demoDesignReferencePages\": 6,");
            sb.AppendLine("  \"styleSystemDerivedPages\": 2,");
            sb.AppendLine("  \"shopRuntimeStrictPass\": " + (shopPass ? "true" : "false") + ",");
            sb.AppendLine("  \"shopCorrelation\": " + shop.correlation.ToString("F4", CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"shopMae\": " + shop.mae.ToString("F4", CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"blockers\": [");
            for (int i = 0; i < blockers.Count; i++)
            {
                sb.Append("    \"").Append(Escape(blockers[i])).Append("\"");
                if (i + 1 < blockers.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(DecisionPath, sb.ToString());
            AssetDatabase.ImportAsset(DecisionPath, ImportAssetOptions.ForceUpdate);
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

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
