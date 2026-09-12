#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 8.5 explicit Shop-only development cutover.
    ///
    /// Safety properties:
    /// - never runs automatically or in batch mode;
    /// - requires actual Step 8.4 Unity runtime capture + approved Shop pass + provider gate;
    /// - keeps the accepted ten-route core registry unchanged;
    /// - copies the refined Shop prefab before writing the activation state;
    /// - snapshots the pre-cutover Shop slot once and never overwrites that snapshot;
    /// - supports exact rollback to the pre-cutover Shop state.
    /// </summary>
    public static class Step20ProductionAssetWave02Step85ShopCutover
    {
        public const string ActiveShopPath = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/Shop.prefab";
        public const string CandidateShopPath = "Assets/LegendsOfRus/Resources/ProductionUI/Wave02Pages/Shop.prefab";
        public const string CutoverStatePath = "Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ShopCutoverState.json";
        public const string AppliedPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_SHOP_CUTOVER_APPLIED.json";
        public const string DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_SHOP_CUTOVER_DECISION.json";
        public const string RollbackAuditPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_5_SHOP_ROLLBACK_AUDIT.json";
        public const string Step84DecisionPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_CONDITIONAL_CUTOVER_READINESS.json";
        public const string RollbackRoot = "ProjectDocs/V1_20_STEP08_5/CutoverRollback";
        public const string SnapshotPath = RollbackRoot + "/IMMUTABLE_BASELINE_SNAPSHOT.json";
        const string BackupPrefabPath = RollbackRoot + "/original_Shop.prefab";
        const string BackupMetaPath = RollbackRoot + "/original_Shop.prefab.meta";

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Evaluate Rollback-Safe Shop Development Cutover")]
        public static void Evaluate()
        {
            List<string> blockers = CollectPreCutoverBlockers();
            WriteDecision(blockers);
            if (blockers.Count == 0)
            {
                Debug.Log("v1.20 Step 8.5 Shop cutover gate: ELIGIBLE. Explicit Apply is still required; no automatic cutover was performed.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 8.5 Shop cutover gate PASS. The approved Shop candidate is eligible for an explicit rollback-safe DEVELOPMENT cutover. Review the Step 8.4 runtime capture before applying.", "OK");
            }
            else
            {
                Debug.LogWarning("v1.20 Step 8.5 Shop cutover gate: NO-GO.\n" + string.Join(Environment.NewLine, blockers.ToArray()));
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 8.5 Shop cutover remains NO-GO. Review V1_20_STEP08_5_SHOP_CUTOVER_DECISION.json.", "OK");
            }
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Apply Rollback-Safe Shop Development Cutover")]
        public static void Apply()
        {
            if (Application.isBatchMode)
                throw new InvalidOperationException("Step 8.5 Shop development cutover requires an explicit interactive editor action.");
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before applying the Step 8.5 Shop cutover.");
            if (File.Exists(AppliedPath) || ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop))
                throw new InvalidOperationException("Step 8.5 Shop cutover is already active. Complete regression/Android QA or use Roll Back; do not re-apply over the immutable rollback snapshot.");

            List<string> blockers = CollectPreCutoverBlockers();
            if (blockers.Count > 0)
            {
                WriteDecision(blockers);
                throw new InvalidOperationException("Step 8.5 Shop cutover blocked:\n" + string.Join(Environment.NewLine, blockers.ToArray()));
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Apply Step 8.5 Shop Development Cutover",
                "This will activate ONLY the approved Shop reconstruction. The original ten-route core registry remains unchanged. A one-time immutable rollback snapshot is created before activation. Purchases/rewards remain owned by the existing ScreenRouter/MonetizationService authority. Continue?",
                "Apply Shop cutover",
                "Cancel"
            );
            if (!confirmed) return;

            EnsureImmutableSnapshot();

            try
            {
                // Transaction order matters: production prefab first; activation state last.
                if (AssetDatabase.LoadAssetAtPath<GameObject>(ActiveShopPath) != null)
                    AssetDatabase.DeleteAsset(ActiveShopPath);

                if (!AssetDatabase.CopyAsset(CandidateShopPath, ActiveShopPath))
                    throw new InvalidOperationException("Could not copy the refined Shop candidate into the active production path.");

                AssetDatabase.ImportAsset(ActiveShopPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                ValidateActiveShopPrefabOrThrow();

                Directory.CreateDirectory(Path.GetDirectoryName(CutoverStatePath));
                File.WriteAllText(CutoverStatePath, BuildStateJson());
                AssetDatabase.ImportAsset(CutoverStatePath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                if (!ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop))
                    throw new InvalidOperationException("Shop activation-state resource was written but the runtime meta registry did not resolve it as active.");

                File.WriteAllText(AppliedPath, BuildAppliedJson());
                AssetDatabase.ImportAsset(AppliedPath, ImportAssetOptions.ForceUpdate);
                WriteDecision(new List<string>());

                // Any previous acceptance becomes invalid on a new/re-applied cutover.
                Step20ProductionAssetWave02Step85Acceptance.ClearAcceptanceLocks();
                Debug.Log("v1.20 Step 8.5 Shop DEVELOPMENT cutover applied. Run post-cutover functional regression and physical Android QA next. Rollback remains available until final acceptance.");
            }
            catch
            {
                EmergencyRestoreFromSnapshot();
                throw;
            }
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 8.5/Roll Back Shop Development Cutover")]
        public static void Rollback()
        {
            if (Application.isBatchMode)
                throw new InvalidOperationException("Step 8.5 rollback requires an explicit interactive editor action.");
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before rolling back the Step 8.5 Shop cutover.");
            if (!File.Exists(SnapshotPath))
                throw new InvalidOperationException("No Step 8.5 immutable rollback snapshot exists.");

            bool confirmed = EditorUtility.DisplayDialog(
                "Roll Back Step 8.5 Shop",
                "Restore Shop to the exact pre-Step-8.5 state and clear Step 8.5 functional/Android acceptance locks? The immutable rollback snapshot and rollback audit will be preserved.",
                "Roll Back Shop",
                "Cancel"
            );
            if (!confirmed) return;

            EmergencyRestoreFromSnapshot();
            File.WriteAllText(RollbackAuditPath,
                "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step08.5-shop-rollback\",\n" +
                "  \"rolledBackAtUtc\": \"" + DateTime.UtcNow.ToString("o") + "\",\n" +
                "  \"shopCutoverActiveAfterRollback\": false,\n" +
                "  \"immutableSnapshotPreserved\": true,\n" +
                "  \"acceptanceLocksCleared\": true\n" +
                "}\n");
            AssetDatabase.ImportAsset(RollbackAuditPath, ImportAssetOptions.ForceUpdate);
            WriteDecision(CollectPreCutoverBlockers());
            Debug.Log("v1.20 Step 8.5 Shop rollback complete. The pre-cutover Shop state has been restored; the immutable rollback snapshot remains preserved.");
        }

        public static List<string> CollectPreCutoverBlockers()
        {
            var blockers = new List<string>();

            string captureRoot = Step20ProductionAssetWave02Step84RuntimeCapture.AbsoluteOutputRoot();
            string captureManifest = Path.Combine(captureRoot, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            bool hasRuntimeEvidence = File.Exists(captureManifest);

            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
            {
                if (hasRuntimeEvidence)
                {
                    blockers.Add("CANDIDATE-BUILD: Wave 2 candidate prefabs are stale/missing after Step 8.4 runtime evidence was generated. Rebuild the candidates, then rerun the Step 8.4 36-capture + Shop gate so evidence matches the candidate being cut over.");
                }
                else
                {
                    try { Step20ProductionAssetWave02PrefabBuilder.BuildAll(false); }
                    catch (Exception e) { blockers.Add("CANDIDATE-BUILD: " + e.Message); }
                }
            }
            if (!Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                blockers.AddRange(Step20ProductionAssetWave02PrefabBuilder.ValidateAll());

            if (!hasRuntimeEvidence)
                blockers.Add("STEP84-RUNTIME-CAPTURE: missing 36/36 actual Unity capture evidence.");
            else
            {
                string manifestText = File.ReadAllText(captureManifest);
                if (manifestText.IndexOf("\"captures\": 36", StringComparison.Ordinal) < 0 ||
                    manifestText.IndexOf("v1.20-step08.3-reference-accurate-wave02-refinement", StringComparison.Ordinal) < 0)
                    blockers.Add("STEP84-RUNTIME-CAPTURE: capture manifest does not prove 36 renders of the Step 8.3 refined Wave 2 candidate set.");
            }
            if (!Step20ProductionAssetWave02Step84ApprovedShopGate.HasPassingLock())
                blockers.Add("STEP84-SHOP-GATE: missing approved Shop runtime PASS lock.");

            List<string> providerFailures = Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false);
            for (int i = 0; i < providerFailures.Count; i++)
                blockers.Add("PROVIDER-BOUNDARY: " + providerFailures[i]);

            if (!File.Exists(Step84DecisionPath) || File.ReadAllText(Step84DecisionPath).IndexOf("ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS", StringComparison.Ordinal) < 0)
                blockers.Add("STEP84-READINESS: run the Step 8.4 Conditional Shop Cutover Readiness evaluator and obtain ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS.");

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                blockers.Add("CORE-REGISTRY: accepted ten-route core production registry has drifted.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop))
                blockers.Add("CORE-REGISTRY: Shop must not be inserted into ProductionCoreLoopPageRegistry.");
            if (ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop))
                blockers.Add("META-CUTOVER: Shop is already active.");
            if (File.Exists(AppliedPath))
                blockers.Add("CUTOVER-STATE: Step 8.5 applied marker already exists.");

            if (File.Exists(SnapshotPath))
                ValidateSnapshotBaselineState(blockers);

            ValidateCandidateShop(blockers);
            return Deduplicate(blockers);
        }

        public static List<string> ValidateActiveCutover()
        {
            var failures = new List<string>();
            if (!File.Exists(AppliedPath)) failures.Add("Missing Step 8.5 applied marker.");
            if (!ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop)) failures.Add("ProductionMetaPageCutoverRegistry does not report Shop active.");
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Accepted core production registry must remain exactly ten routes.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Shop)) failures.Add("Shop was incorrectly added to the core-loop registry.");

            GameObject active = AssetDatabase.LoadAssetAtPath<GameObject>(ActiveShopPath);
            if (active == null)
            {
                failures.Add("Missing active Shop production prefab.");
            }
            else
            {
                ProductionCoreLoopPageMarker marker = active.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null || marker.ScreenId != ScreenId.Shop || !marker.ScreenshotFree)
                    failures.Add("Active Shop screenshot-free marker missing/mismatched.");
                if (active.GetComponentsInChildren<Button>(true).Length != 0)
                    failures.Add("Active Shop contains Button components; ScreenRouter must remain the only interaction authority.");
                string[] deps = AssetDatabase.GetDependencies(ActiveShopPath, true);
                foreach (string dep in deps)
                {
                    string n = dep.Replace('\\', '/');
                    if (n.Contains("/Art/ReferenceOnly/") || n.Contains("/Resources/DesignScreens/") || n.Contains("/ProjectDocs/"))
                        failures.Add("Active Shop has forbidden reference/QA dependency: " + dep);
                }
            }

            List<string> provider = Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false);
            for (int i = 0; i < provider.Count; i++) failures.Add("Provider-boundary regression: " + provider[i]);
            return Deduplicate(failures);
        }


        static void ValidateSnapshotBaselineState(List<string> failures)
        {
            string text = File.Exists(SnapshotPath) ? File.ReadAllText(SnapshotPath) : string.Empty;
            bool expectedPrefab = text.IndexOf("\"originalActiveShopPrefabExisted\": true", StringComparison.Ordinal) >= 0;
            bool actualPrefab = File.Exists(ActiveShopPath);
            if (expectedPrefab != actualPrefab)
            {
                failures.Add("ROLLBACK-SNAPSHOT: current pre-cutover Shop prefab presence does not match the immutable baseline snapshot.");
                return;
            }

            if (expectedPrefab)
            {
                string expectedHash = ExtractJsonString(text, "originalActiveShopSha256");
                string actualHash = Sha256(ActiveShopPath);
                if (string.IsNullOrEmpty(expectedHash) || !string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase))
                    failures.Add("ROLLBACK-SNAPSHOT: current pre-cutover Shop prefab hash does not match the immutable baseline snapshot.");
            }
        }

        static string ExtractJsonString(string json, string key)
        {
            string token = "\"" + key + "\"";
            int k = json.IndexOf(token, StringComparison.Ordinal);
            if (k < 0) return string.Empty;
            int colon = json.IndexOf(':', k + token.Length);
            if (colon < 0) return string.Empty;
            int first = json.IndexOf('\"', colon + 1);
            if (first < 0) return string.Empty;
            int second = json.IndexOf('\"', first + 1);
            return second > first ? json.Substring(first + 1, second - first - 1) : string.Empty;
        }

        static void ValidateCandidateShop(List<string> failures)
        {
            GameObject shop = AssetDatabase.LoadAssetAtPath<GameObject>(CandidateShopPath);
            if (shop == null)
            {
                failures.Add("SHOP-CANDIDATE: refined candidate prefab is missing.");
                return;
            }
            ProductionCoreLoopPageMarker marker = shop.GetComponent<ProductionCoreLoopPageMarker>();
            if (marker == null || marker.ScreenId != ScreenId.Shop || !marker.ScreenshotFree)
                failures.Add("SHOP-CANDIDATE: screenshot-free marker missing/mismatched.");
            if (shop.GetComponentsInChildren<Button>(true).Length != 0)
                failures.Add("SHOP-CANDIDATE: presentation-only candidate contains Button components.");
        }

        static void ValidateActiveShopPrefabOrThrow()
        {
            GameObject active = AssetDatabase.LoadAssetAtPath<GameObject>(ActiveShopPath);
            if (active == null) throw new InvalidOperationException("Active Shop prefab failed to import.");
            ProductionCoreLoopPageMarker marker = active.GetComponent<ProductionCoreLoopPageMarker>();
            if (marker == null || marker.ScreenId != ScreenId.Shop || !marker.ScreenshotFree)
                throw new InvalidOperationException("Active Shop prefab marker mismatch.");
            if (active.GetComponentsInChildren<Button>(true).Length != 0)
                throw new InvalidOperationException("Active Shop must remain presentation-only; unexpected Button components were found.");
        }

        static void EnsureImmutableSnapshot()
        {
            Directory.CreateDirectory(RollbackRoot);
            if (File.Exists(SnapshotPath))
            {
                // The first snapshot is intentionally immutable. Never overwrite it.
                var snapshotFailures = new List<string>();
                ValidateSnapshotBaselineState(snapshotFailures);
                if (snapshotFailures.Count > 0)
                    throw new InvalidOperationException("Existing immutable rollback snapshot does not match the current pre-cutover Shop state:\n" + string.Join(Environment.NewLine, snapshotFailures.ToArray()));
                return;
            }

            bool originalPrefab = File.Exists(ActiveShopPath);
            bool originalMeta = File.Exists(ActiveShopPath + ".meta");
            if (originalPrefab) File.Copy(ActiveShopPath, BackupPrefabPath, false);
            if (originalMeta) File.Copy(ActiveShopPath + ".meta", BackupMetaPath, false);

            string candidateHash = File.Exists(CandidateShopPath) ? Sha256(CandidateShopPath) : string.Empty;
            string originalHash = originalPrefab ? Sha256(ActiveShopPath) : string.Empty;
            File.WriteAllText(SnapshotPath,
                "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step08.5-immutable-shop-rollback-snapshot\",\n" +
                "  \"createdAtUtc\": \"" + DateTime.UtcNow.ToString("o") + "\",\n" +
                "  \"originalActiveShopPrefabExisted\": " + (originalPrefab ? "true" : "false") + ",\n" +
                "  \"originalActiveShopMetaExisted\": " + (originalMeta ? "true" : "false") + ",\n" +
                "  \"originalActiveShopSha256\": \"" + originalHash + "\",\n" +
                "  \"candidateShopSha256AtFirstApply\": \"" + candidateHash + "\",\n" +
                "  \"snapshotOverwriteAllowed\": false\n" +
                "}\n");
        }

        static void EmergencyRestoreFromSnapshot()
        {
            // Disable the route first so an incomplete prefab can never remain active.
            if (AssetDatabase.LoadAssetAtPath<TextAsset>(CutoverStatePath) != null)
                AssetDatabase.DeleteAsset(CutoverStatePath);
            else
            {
                if (File.Exists(CutoverStatePath)) File.Delete(CutoverStatePath);
                if (File.Exists(CutoverStatePath + ".meta")) File.Delete(CutoverStatePath + ".meta");
            }

            bool originalExisted = File.Exists(BackupPrefabPath);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(ActiveShopPath) != null)
                AssetDatabase.DeleteAsset(ActiveShopPath);
            else
            {
                if (File.Exists(ActiveShopPath)) File.Delete(ActiveShopPath);
                if (File.Exists(ActiveShopPath + ".meta")) File.Delete(ActiveShopPath + ".meta");
            }

            if (originalExisted)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ActiveShopPath));
                File.Copy(BackupPrefabPath, ActiveShopPath, true);
                if (File.Exists(BackupMetaPath)) File.Copy(BackupMetaPath, ActiveShopPath + ".meta", true);
            }

            if (File.Exists(AppliedPath)) File.Delete(AppliedPath);
            if (File.Exists(AppliedPath + ".meta")) File.Delete(AppliedPath + ".meta");
            Step20ProductionAssetWave02Step85Acceptance.ClearAcceptanceLocks();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        static string BuildStateJson()
        {
            return "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"state\": \"" + ProductionMetaPageCutoverRegistry.ShopStateMagic + "\",\n" +
                "  \"enabled\": true,\n" +
                "  \"screen\": \"Shop\",\n" +
                "  \"resourceKey\": \"ProductionUI/Pages/Shop\",\n" +
                "  \"fullWave02Cutover\": false\n" +
                "}\n";
        }

        static string BuildAppliedJson()
        {
            return "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step08.5-rollback-safe-shop-development-cutover\",\n" +
                "  \"appliedAtUtc\": \"" + DateTime.UtcNow.ToString("o") + "\",\n" +
                "  \"shopOnly\": true,\n" +
                "  \"coreRegistryChanged\": false,\n" +
                "  \"activeMetaCutoverPages\": 1,\n" +
                "  \"rollbackSnapshot\": \"" + SnapshotPath + "\",\n" +
                "  \"postCutoverFunctionalRegressionRequired\": true,\n" +
                "  \"physicalAndroidAcceptanceRequired\": true,\n" +
                "  \"fullWave02CutoverAuthorized\": false\n" +
                "}\n";
        }

        static void WriteDecision(List<string> blockers)
        {
            bool active = ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Shop) && File.Exists(AppliedPath);
            bool functional = File.Exists(Step20ProductionAssetWave02Step85Acceptance.FunctionalPassPath);
            bool android = File.Exists(Step20ProductionAssetWave02Step85Acceptance.AndroidPassPath);
            string decision = blockers.Count > 0
                ? "NO_GO_SHOP_DEVELOPMENT_CUTOVER"
                : (!active ? "ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER"
                    : (!functional || !android ? "SHOP_CUTOVER_APPLIED_REGRESSION_ACCEPTANCE_PENDING"
                        : "ACCEPTED_SHOP_DEVELOPMENT_CUTOVER"));

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.5-shop-cutover-android-acceptance\",");
            sb.AppendLine("  \"decision\": \"" + decision + "\",");
            sb.AppendLine("  \"automaticCutover\": false,");
            sb.AppendLine("  \"shopCutoverActive\": " + (active ? "true" : "false") + ",");
            sb.AppendLine("  \"coreRegistryExpectedRoutes\": 10,");
            sb.AppendLine("  \"fullWave02CutoverAuthorized\": false,");
            sb.AppendLine("  \"postCutoverFunctionalRegressionPassed\": " + (functional ? "true" : "false") + ",");
            sb.AppendLine("  \"physicalAndroidRegressionPassed\": " + (android ? "true" : "false") + ",");
            sb.AppendLine("  \"rollbackAvailable\": " + (File.Exists(SnapshotPath) ? "true" : "false") + ",");
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

        static List<string> Deduplicate(List<string> values)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<string>();
            foreach (string value in values)
                if (!string.IsNullOrEmpty(value) && seen.Add(value)) result.Add(value);
            return result;
        }

        static string Sha256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
