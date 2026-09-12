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
    /// v1.20 Step 8.2 freeze guard for economy/provider authority. Wave 2 visual
    /// candidates are presentation-only and must not acquire their own purchase,
    /// ad, reward, friend-energy, or direct GameState mutation path.
    /// </summary>
    public static class Step20ProductionAssetWave02Step82ProviderBoundarySelfTest
    {
        const string ScreenRouterPath = "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs";
        const string MonetizationServicePath = "Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs";
        const string GameStatePath = "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs";
        const string VerifiedGatePath = "Assets/LegendsOfRus/Scripts/CommercialCore/Monetization/VerifiedEntitlementGate.cs";
        const string RegistryPath = "Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs";
        const string ReportPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP08_2_PROVIDER_BOUNDARY_RUNTIME_FREEZE_REPORT.json";

        static readonly string[] ExpectedHashes =
        {
            "6b66f9d6ad08c15b087489685cd2056b9b479e3389c19ce2773094b958861a02",
            "e2ef36f2bbab50ae45f1852e729b8692830539fc1345b835564e9e2f7b2d4e79",
            "d8853d33a49a6d21d414fab1830b957d032d047ed242b259af47d749414b6163",
            "acc60d5ef7f4c9b925f3282f3fe5a6d05619398410bb6e286c4736156cabeb29",
            "aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938"
        };

        static readonly string[] CriticalPaths =
        {
            ScreenRouterPath, MonetizationServicePath, GameStatePath, VerifiedGatePath, RegistryPath
        };

        static readonly ScreenId[] Pages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory,
            ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect,
            ScreenId.RewardClaim
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.2 Provider-Boundary Verification")]
        public static void Run()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);
            List<string> failures = Validate(true);
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            Debug.Log("v1.20 Step 8.2 provider-boundary verification PASS. Critical runtime authority files are frozen; 9/9 Wave 2 candidates remain presentation-only.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.2 provider-boundary PASS. Shop/Special Offer/Out-of-Energy cannot bypass the accepted monetization authority, and Reward Claim remains acknowledgement-only.", "OK");
        }

        public static List<string> Validate(bool writeReport)
        {
            var failures = new List<string>();

            for (int i = 0; i < CriticalPaths.Length; i++)
            {
                if (!File.Exists(CriticalPaths[i]))
                {
                    failures.Add("Missing critical authority source: " + CriticalPaths[i]);
                    continue;
                }
                string actual = Sha256(CriticalPaths[i]);
                if (!string.Equals(actual, ExpectedHashes[i], StringComparison.OrdinalIgnoreCase))
                    failures.Add("Authority freeze mismatch: " + CriticalPaths[i] + " expected=" + ExpectedHashes[i] + " actual=" + actual);
            }

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Active production registry count changed from the accepted 10-route baseline.");

            string router = File.Exists(ScreenRouterPath) ? File.ReadAllText(ScreenRouterPath) : string.Empty;
            RequireContains(router, "MonetizationService.Instance.TryPurchase(offerId)", "ScreenRouter purchase route no longer crosses MonetizationService.", failures);
            RequireContains(router, "!result.IsVerifiedAndDurablyCommitted", "ScreenRouter no longer fail-closes uncommitted monetization results.", failures);
            RequireContains(router, "MonetizationService.Instance.TryRewardedEnergy()", "Out-of-Energy rewarded-ad route no longer crosses MonetizationService.", failures);
            RequireContains(router, "Энергия не начисляется без серверного подтверждения", "Friend-energy demo acknowledgement no longer states the no-grant contract.", failures);
            RequireContains(router, "case ScreenId.RewardClaim:", "RewardClaim modal route is missing.", failures);
            RequireContains(router, "Hotspot(parent, \"Claim\", 260, 1260, 430, 140, CloseModal)", "RewardClaim candidate acknowledgement path changed from close-only behavior.", failures);

            string monetization = File.Exists(MonetizationServicePath) ? File.ReadAllText(MonetizationServicePath) : string.Empty;
            RequireContains(monetization, "DisabledRuntimeMonetizationPort", "Fail-closed disabled monetization port is missing.", failures);
            RequireContains(monetization, "RuntimeMonetizationActionState.VerifiedAndDurablyCommitted", "Verified-and-durably-committed state contract is missing.", failures);
            RequireContains(monetization, "MONETIZATION_AD_CONSENT_REQUIRED", "Rewarded-ad consent fail-closed guard is missing.", failures);

            foreach (ScreenId id in Pages)
            {
                string path = Step20ProductionAssetWave02PrefabBuilder.CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    failures.Add("Missing Wave 2 candidate prefab for provider-boundary audit: " + id);
                    continue;
                }

                Button[] buttons = prefab.GetComponentsInChildren<Button>(true);
                if (buttons.Length != 0)
                    failures.Add(id + ": Wave 2 candidate contains " + buttons.Length + " Button component(s); candidates must remain presentation-only until an explicitly reviewed interaction binder is introduced.");

                MonoBehaviour[] behaviours = prefab.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (MonoBehaviour behaviour in behaviours)
                {
                    if (behaviour == null || behaviour is ProductionCoreLoopPageMarker) continue;
                    Type type = behaviour.GetType();
                    if (string.Equals(type.Namespace, "UnityEngine.UI", StringComparison.Ordinal)) continue;
                    failures.Add(id + ": unexpected candidate MonoBehaviour dependency: " + type.FullName);
                }
            }

            if (writeReport) WriteReport(failures);
            return failures;
        }

        static void RequireContains(string source, string token, string message, List<string> failures)
        {
            if (source.IndexOf(token, StringComparison.Ordinal) < 0) failures.Add(message);
        }

        static string Sha256(string projectRelativePath)
        {
            using (FileStream stream = File.OpenRead(projectRelativePath))
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        static void WriteReport(List<string> failures)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step08.2-provider-boundary-verification\",");
            sb.AppendLine("  \"result\": \"" + (failures.Count == 0 ? "PASS" : "FAIL") + "\",");
            sb.AppendLine("  \"criticalRuntimeAuthorityFilesFrozen\": " + (failures.Count == 0 ? "true" : "false") + ",");
            sb.AppendLine("  \"wave02CandidateButtonsExpected\": 0,");
            sb.AppendLine("  \"activeRegistryExpectedRoutes\": 10,");
            sb.AppendLine("  \"directCandidateEconomyMutationAllowed\": false,");
            sb.AppendLine("  \"failures\": [");
            for (int i = 0; i < failures.Count; i++)
            {
                sb.Append("    \"").Append(Escape(failures[i])).Append("\"");
                if (i + 1 < failures.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            File.WriteAllText(ReportPath, sb.ToString());
            AssetDatabase.ImportAsset(ReportPath, ImportAssetOptions.ForceUpdate);
        }

        static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
