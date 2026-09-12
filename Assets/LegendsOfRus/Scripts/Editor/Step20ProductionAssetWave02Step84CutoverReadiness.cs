#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Evaluates whether Shop is ready for a later explicit development cutover.
    /// This class never copies/replaces an active prefab and never edits the runtime
    /// registry. Full Wave 2 cutover remains blocked because approved authority is 1/9.
    /// </summary>
    public static class Step20ProductionAssetWave02Step84CutoverReadiness
    {
        const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_CONDITIONAL_CUTOVER_READINESS.json";
        const string ShopCandidatePath = "Assets/LegendsOfRus/Resources/ProductionUI/Wave02Pages/Shop.prefab";

        [MenuItem("Legends of Rus/Production UI/Evaluate v1.20 Step 8.4 Conditional Shop Cutover Readiness")]
        public static void Evaluate()
        {
            var blockers = new List<string>();
            blockers.AddRange(Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false));

            string captureRoot = Step20ProductionAssetWave02Step84RuntimeCapture.AbsoluteOutputRoot();
            if (!File.Exists(Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json")))
                blockers.Add("RUNTIME_CAPTURE: missing Step 8.4 36-capture execution evidence.");
            if (!Step20ProductionAssetWave02Step84ApprovedShopGate.HasPassingLock())
                blockers.Add("SHOP_APPROVED_GATE: missing passing approved-reference runtime lock.");

            GameObject shop = AssetDatabase.LoadAssetAtPath<GameObject>(ShopCandidatePath);
            if (shop == null)
            {
                blockers.Add("SHOP_CANDIDATE: missing refined Shop candidate prefab.");
            }
            else
            {
                ProductionCoreLoopPageMarker marker = shop.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null || marker.ScreenId != ScreenId.Shop || !marker.ScreenshotFree)
                    blockers.Add("SHOP_CANDIDATE: screenshot-free marker missing or mismatched.");
                if (shop.GetComponentsInChildren<Button>(true).Length != 0)
                    blockers.Add("SHOP_CANDIDATE: presentation-only candidate unexpectedly contains Button components.");
                string[] deps = AssetDatabase.GetDependencies(ShopCandidatePath, true);
                foreach (string dep in deps)
                {
                    string n = dep.Replace('\\', '/');
                    if (n.Contains("/Art/ReferenceOnly/") || n.Contains("/Resources/DesignScreens/") || n.Contains("/ProjectDocs/"))
                        blockers.Add("SHOP_CANDIDATE: forbidden reference/QA dependency " + dep);
                }
            }

            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop))
                blockers.Add("REGISTRY: Shop is already active; Step 8.4 readiness package must be evaluated before cutover.");
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                blockers.Add("FUNCTIONAL_FREEZE: active production registry no longer matches the accepted 10-route baseline.");

            bool eligible = blockers.Count == 0;
            WriteDecision(eligible, blockers);

            if (eligible)
            {
                Debug.LogWarning("v1.20 Step 8.4 readiness: ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS. No cutover was performed. Full Wave 2 remains NO-GO (approved authority 1/9). Proceed only through the next explicit rollback-safe cutover milestone.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Shop is eligible for the NEXT explicit development-cutover milestone. Step 8.4 did not replace any active prefab. Full Wave 2 remains NO-GO because approved reference coverage is 1/9.", "OK");
            }
            else
            {
                Debug.LogWarning("v1.20 Step 8.4 readiness: NO-GO. Blockers=" + blockers.Count + ". Review V1_20_STEP08_4_CONDITIONAL_CUTOVER_READINESS.json.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 8.4 conditional Shop cutover readiness is NO-GO. Review the decision JSON for blockers.", "OK");
            }
        }

        static void WriteDecision(bool eligible, List<string> blockers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.4-conditional-shop-cutover-readiness\",");
            sb.AppendLine("  \"decision\": \"" + (eligible ? "ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS" : "NO_GO_SHOP_CUTOVER_READINESS") + "\",");
            sb.AppendLine("  \"activeCutoverPerformed\": false,");
            sb.AppendLine("  \"shopOnlyEligibility\": true,");
            sb.AppendLine("  \"fullWave02CutoverAuthorized\": false,");
            sb.AppendLine("  \"directApprovedReferencePages\": 1,");
            sb.AppendLine("  \"otherWave02PagesEligibleForProductionCutover\": 0,");
            sb.AppendLine("  \"rollbackRequiredBeforeFutureApply\": true,");
            sb.AppendLine("  \"androidPhysicalAcceptanceRequiredAfterFutureApply\": true,");
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

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
