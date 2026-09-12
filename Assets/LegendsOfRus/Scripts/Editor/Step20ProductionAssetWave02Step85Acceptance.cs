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
    /// Post-cutover acceptance recorder for Step 8.5. Automated checks verify
    /// source/runtime boundaries; functional and physical Android passes require
    /// explicit interactive confirmation after the user actually runs those tests.
    /// </summary>
    public static class Step20ProductionAssetWave02Step85Acceptance
    {
        public const string FunctionalPassPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_POST_CUTOVER_FUNCTIONAL_REGRESSION_PASS.lock";
        public const string AndroidPassPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_PHYSICAL_ANDROID_REGRESSION_PASS.lock";
        public const string FinalAcceptancePath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_FINAL_SHOP_CUTOVER_ACCEPTANCE.json";
        const string PreDeviceReportPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_PRE_ANDROID_REGRESSION_GATE.json";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.5 Post-Cutover Pre-Android Regression Gate")]
        public static void RunPreAndroidGate()
        {
            List<string> failures = CollectAutomatedFailures();
            WritePreDeviceReport(failures);
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Debug.Log("v1.20 Step 8.5 pre-Android regression gate PASS. Shop is active through the isolated meta cutover registry; provider/economy authority remains unchanged.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.5 pre-Android automated gate PASS. Next run the complete EditMode/PlayMode + Step 5 functional flow regression, then record Functional Regression PASS.", "OK");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Record Post-Cutover Functional Regression PASS")]
        public static void RecordFunctionalRegressionPass()
        {
            RequireInteractiveAndActive();
            List<string> failures = CollectAutomatedFailures();
            if (failures.Count > 0)
                throw new InvalidOperationException("Automated preconditions are not passing:\n" + string.Join(Environment.NewLine, failures.ToArray()));

            bool confirmed = EditorUtility.DisplayDialog(
                "Step 8.5 Functional Regression",
                "Confirm ONLY after the ACTIVE Shop cutover passes: full EditMode tests, full PlayMode tests, Step 5 demo-flow QA, Home → Shop, visible Back and system Back → Home, Special Offer route, no new Console errors, and purchase/reward fail-closed behavior with no unverified grants.",
                "Confirm Functional PASS",
                "Cancel"
            );
            if (!confirmed) return;

            File.WriteAllText(FunctionalPassPath,
                "v1.20-step08.5-post-cutover-functional-regression-pass\n" +
                "confirmed-at-utc=" + DateTime.UtcNow.ToString("o") + "\n" +
                "checks=editmode,playmode,step5-demo-flow,home-shop,visible-back,system-back,special-offer-route,console-clean,provider-fail-closed,no-unverified-grants\n" +
                "confirmed-by=explicit-editor-user-action\n");
            AssetDatabase.ImportAsset(FunctionalPassPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("v1.20 Step 8.5 post-cutover functional regression PASS recorded.");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Record Physical Android Regression PASS")]
        public static void RecordAndroidPass()
        {
            RequireInteractiveAndActive();
            if (!File.Exists(FunctionalPassPath))
                throw new InvalidOperationException("Record the post-cutover Functional Regression PASS before physical Android acceptance.");

            List<string> failures = CollectAutomatedFailures();
            if (failures.Count > 0)
                throw new InvalidOperationException("Automated preconditions are not passing:\n" + string.Join(Environment.NewLine, failures.ToArray()));

            bool confirmed = EditorUtility.DisplayDialog(
                "Step 8.5 Physical Android Regression",
                "Confirm ONLY after testing the ACTIVE Shop cutover on a physical Android device: cold launch, Home → Shop, 9:16/tall-phone safe area, notch/cutout clearance, visible Back, Android system Back, all Shop touch hotspots, scroll/tap stability, live Cyrillic/price text, resume-from-background, Special Offer route, and purchase/ad/reward fail-closed behavior with no currency/item grant unless the authoritative provider verifies and durably commits it.",
                "Confirm Android PASS",
                "Cancel"
            );
            if (!confirmed) return;

            File.WriteAllText(AndroidPassPath,
                "v1.20-step08.5-physical-android-regression-pass\n" +
                "confirmed-at-utc=" + DateTime.UtcNow.ToString("o") + "\n" +
                "checks=cold-launch,home-shop,safe-area,cutout,visible-back,system-back,touch-hotspots,cyrillic,price-text,resume,special-offer-route,provider-fail-closed,no-unverified-grants\n" +
                "confirmed-by=explicit-editor-user-action\n");
            AssetDatabase.ImportAsset(AndroidPassPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("v1.20 Step 8.5 physical Android regression PASS recorded.");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Evaluate Final Shop Development Cutover Acceptance")]
        public static void EvaluateFinalAcceptance()
        {
            List<string> failures = CollectAutomatedFailures();
            if (!File.Exists(FunctionalPassPath)) failures.Add("Missing explicit post-cutover functional regression PASS.");
            if (!File.Exists(AndroidPassPath)) failures.Add("Missing explicit physical Android regression PASS.");

            bool accepted = failures.Count == 0;
            WriteFinalAcceptance(accepted, failures);
            if (accepted)
            {
                Debug.Log("v1.20 Step 8.5 ACCEPTED_SHOP_DEVELOPMENT_CUTOVER. Shop-only production visual cutover passed automated, functional, and physical Android acceptance. Full Wave 2 remains unauthorized (approved authority 1/9).");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 8.5 Shop DEVELOPMENT cutover accepted. Keep the immutable rollback snapshot. Full Wave 2 cutover is still blocked because the other eight pages do not have direct approved reference authority.", "OK");
            }
            else
            {
                Debug.LogWarning("v1.20 Step 8.5 final Shop acceptance remains PENDING/NO-GO.\n" + string.Join(Environment.NewLine, failures.ToArray()));
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 8.5 final acceptance is not complete. Review V1_20_STEP08_5_FINAL_SHOP_CUTOVER_ACCEPTANCE.json; use Roll Back if any regression exists.", "OK");
            }
        }

        public static List<string> CollectAutomatedFailures()
        {
            var failures = Step20ProductionAssetWave02Step85ShopCutover.ValidateActiveCutover();
            if (!File.Exists(Step20ProductionAssetWave02Step85ShopCutover.SnapshotPath))
                failures.Add("Immutable rollback snapshot is missing.");
            if (!Step20ProductionAssetWave02Step84ApprovedShopGate.HasPassingLock())
                failures.Add("Approved Shop Step 8.4 runtime PASS lock is missing.");
            return failures;
        }

        public static void ClearAcceptanceLocks()
        {
            DeleteAssetOrFile(FunctionalPassPath);
            DeleteAssetOrFile(AndroidPassPath);
            DeleteAssetOrFile(FinalAcceptancePath);
            DeleteAssetOrFile(PreDeviceReportPath);
        }

        static void RequireInteractiveAndActive()
        {
            if (Application.isBatchMode)
                throw new InvalidOperationException("Step 8.5 acceptance signoff requires an explicit interactive editor action.");
            if (!ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop) || !File.Exists(Step20ProductionAssetWave02Step85ShopCutover.AppliedPath))
                throw new InvalidOperationException("Apply the Step 8.5 Shop development cutover first.");
        }

        static void WritePreDeviceReport(List<string> failures)
        {
            File.WriteAllText(PreDeviceReportPath, BuildReport(
                "v1.20-step08.5-pre-android-regression-gate",
                failures.Count == 0 ? "PASS" : "FAIL",
                failures));
            AssetDatabase.ImportAsset(PreDeviceReportPath, ImportAssetOptions.ForceUpdate);
        }

        static void WriteFinalAcceptance(bool accepted, List<string> failures)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.5-final-shop-development-cutover-acceptance\",");
            sb.AppendLine("  \"decision\": \"" + (accepted ? "ACCEPTED_SHOP_DEVELOPMENT_CUTOVER" : "PENDING_OR_NO_GO_SHOP_DEVELOPMENT_CUTOVER") + "\",");
            sb.AppendLine("  \"shopOnly\": true,");
            sb.AppendLine("  \"fullWave02CutoverAuthorized\": false,");
            sb.AppendLine("  \"approvedReferenceAuthority\": \"1/9\",");
            sb.AppendLine("  \"postCutoverFunctionalRegressionPassed\": " + (File.Exists(FunctionalPassPath) ? "true" : "false") + ",");
            sb.AppendLine("  \"physicalAndroidRegressionPassed\": " + (File.Exists(AndroidPassPath) ? "true" : "false") + ",");
            sb.AppendLine("  \"immutableRollbackSnapshotPreserved\": " + (File.Exists(Step20ProductionAssetWave02Step85ShopCutover.SnapshotPath) ? "true" : "false") + ",");
            sb.AppendLine("  \"failures\": [");
            for (int i = 0; i < failures.Count; i++)
            {
                sb.Append("    \"").Append(Escape(failures[i])).Append("\"");
                if (i + 1 < failures.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(FinalAcceptancePath, sb.ToString());
            AssetDatabase.ImportAsset(FinalAcceptancePath, ImportAssetOptions.ForceUpdate);
        }

        static string BuildReport(string milestone, string result, List<string> failures)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"" + milestone + "\",");
            sb.AppendLine("  \"result\": \"" + result + "\",");
            sb.AppendLine("  \"shopCutoverActive\": " + (ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop) ? "true" : "false") + ",");
            sb.AppendLine("  \"coreRegistryStillTenRoutes\": " + (ProductionCutoverFreezeGuard.MatchesExpectedCount(10) ? "true" : "false") + ",");
            sb.AppendLine("  \"failures\": [");
            for (int i = 0; i < failures.Count; i++)
            {
                sb.Append("    \"").Append(Escape(failures[i])).Append("\"");
                if (i + 1 < failures.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            return sb.ToString();
        }

        static void DeleteAssetOrFile(string path)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
                AssetDatabase.DeleteAsset(path);
            else
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(path + ".meta")) File.Delete(path + ".meta");
            }
        }

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
