#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 9.2 authority-boundary gate. Wave 3 visuals must remain
    /// presentation-only while the inherited demo/fail-closed social/LiveOps logic stays frozen.
    /// </summary>
    public static class Step20ProductionAssetWave03Step92BoundaryGate
    {
        const string OutputRoot = "ProjectDocs/V1_20_STEP09_2/UnityRuntimeCaptures";
        const string PassLock = OutputRoot + "/SOCIAL_LIVEOPS_BOUNDARY_PASS.lock";
        const string ResultJson = OutputRoot + "/SOCIAL_LIVEOPS_BOUNDARY_RESULT.json";

        static readonly ScreenId[] Pages =
        {
            ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub,
            ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail
        };

        static readonly FrozenFile[] Frozen =
        {
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", "6b66f9d6ad08c15b087489685cd2056b9b479e3389c19ce2773094b958861a02"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs", "655879000c576ec5bf317a85ce8743b1b06afa0aeba374c8c278fc5fa5bcf4e3"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", "d8853d33a49a6d21d414fab1830b957d032d047ed242b259af47d749414b6163"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs", "aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMetaPageCutoverRegistry.cs", "fc1bdb088b71d5d0998fcafbcecc4a49ba38088ff0bce48245e9f42cfb2ee4b3")
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.2 Social-LiveOps Boundary Verification")]
        public static void RunMenu()
        {
            List<string> failures = Validate(true);
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
        }

        public static List<string> Validate(bool writeEvidence)
        {
            var failures = new List<string>();

            foreach (FrozenFile frozen in Frozen)
            {
                if (!File.Exists(frozen.path))
                {
                    failures.Add("Missing frozen authority file: " + frozen.path);
                    continue;
                }
                string actual = Sha256(frozen.path);
                if (!string.Equals(actual, frozen.sha256, StringComparison.OrdinalIgnoreCase))
                    failures.Add("Authority drift: " + frozen.path + " expected " + frozen.sha256 + " but found " + actual + ".");
            }

            string demoSource = File.Exists("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs")
                ? File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs")
                : string.Empty;
            if (demoSource.IndexOf("GameState", StringComparison.Ordinal) >= 0)
                failures.Add("DemoFeatureState must not reference GameState/economy authority.");
            if (demoSource.IndexOf("MonetizationService", StringComparison.Ordinal) >= 0 ||
                demoSource.IndexOf("VerifiedEntitlementGate", StringComparison.Ordinal) >= 0)
                failures.Add("DemoFeatureState must not reference commercial fulfillment authority.");

            string routerSource = File.Exists("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs")
                ? File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs")
                : string.Empty;
            RequireContains(routerSource, "DemoFeatureState.TryClaimEventReward()", "Event demo claim must remain session-only acknowledgement.", failures);
            RequireContains(routerSource, "Валюта не начисляется без авторитетного LiveOps-рецепта", "Event claim must explicitly remain no-currency demo behavior.", failures);
            RequireContains(routerSource, "DemoFeatureState.RecordTeamMessage()", "Team chat must remain local-session demo behavior.", failures);
            RequireContains(routerSource, "DemoFeatureState.TryRequestFriendEnergy()", "Friend-energy request must remain local-session acknowledgement.", failures);
            RequireContains(routerSource, "Почтовые награды пока недоступны", "Mail rewards must remain unavailable without backend.", failures);

            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Accepted core-loop production registry changed; expected 10 routes.");

            foreach (ScreenId id in Pages)
            {
                string path = Step20ProductionAssetWave03PrefabBuilder.CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    failures.Add("Missing Wave 3 candidate prefab: " + path);
                    continue;
                }
                if (prefab.GetComponentsInChildren<Button>(true).Length != 0)
                    failures.Add(path + ": candidate contains Button behavior; social/LiveOps authority must remain in ScreenRouter/backend boundaries.");
                if (prefab.GetComponentsInChildren<EventTrigger>(true).Length != 0)
                    failures.Add(path + ": candidate contains EventTrigger behavior; presentation candidates must be passive.");
                foreach (string dep in AssetDatabase.GetDependencies(path, true))
                {
                    string normalized = dep.Replace('\\', '/');
                    if (normalized.Contains("/Art/ReferenceOnly/") || normalized.Contains("/Resources/DesignScreens/") || normalized.Contains("/ProjectDocs/"))
                        failures.Add(path + ": forbidden reference/QA dependency " + dep);
                }
            }

            if (writeEvidence)
                WriteEvidence(failures);

            if (writeEvidence && failures.Count == 0)
            {
                Debug.Log("v1.20 Step 9.2 Social/LiveOps boundary verification PASS: frozen authority + 8/8 passive candidates.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Step 9.2 Social/LiveOps boundary verification PASS. Wave 3 candidates are presentation-only; inherited demo/fail-closed authority remains frozen.", "OK");
            }
            return failures;
        }

        static void WriteEvidence(List<string> failures)
        {
            string absRoot = Path.Combine(Directory.GetCurrentDirectory(), OutputRoot.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(absRoot);
            string passAbs = Path.Combine(Directory.GetCurrentDirectory(), PassLock.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(passAbs)) File.Delete(passAbs);

            string json = "{\n" +
                "  \"schemaVersion\": 1,\n" +
                "  \"milestone\": \"v1.20-step09.2-wave03-social-liveops-boundary\",\n" +
                "  \"passed\": " + (failures.Count == 0 ? "true" : "false") + ",\n" +
                "  \"frozenAuthorityFiles\": 5,\n" +
                "  \"candidatePages\": 8,\n" +
                "  \"candidateButtonsExpected\": 0,\n" +
                "  \"candidateEventTriggersExpected\": 0,\n" +
                "  \"eventClaimEconomyGrantAuthorized\": false,\n" +
                "  \"leaderboardBackendActivated\": false,\n" +
                "  \"teamBackendActivated\": false,\n" +
                "  \"chatBackendActivated\": false,\n" +
                "  \"friendEnergyGrantAuthorized\": false,\n" +
                "  \"mailRewardGrantAuthorized\": false,\n" +
                "  \"failureCount\": " + failures.Count + "\n" +
                "}\n";
            string resultAbs = Path.Combine(Directory.GetCurrentDirectory(), ResultJson.Replace('/', Path.DirectorySeparatorChar));
            File.WriteAllText(resultAbs, json);
            if (failures.Count == 0)
            {
                File.WriteAllText(passAbs,
                    "v1.20-step09.2-social-liveops-boundary-pass\n" +
                    "wave03-cutover-authorized=false\n" +
                    "production-social-backends-activated=false\n");
            }
            AssetDatabase.Refresh();
        }

        static void RequireContains(string source, string needle, string message, List<string> failures)
        {
            if (source.IndexOf(needle, StringComparison.Ordinal) < 0)
                failures.Add(message);
        }

        static string Sha256(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        readonly struct FrozenFile
        {
            public readonly string path;
            public readonly string sha256;
            public FrozenFile(string path, string sha256) { this.path = path; this.sha256 = sha256; }
        }
    }
}
#endif
