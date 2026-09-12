using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20FunctionalQaSelfTest
    {
        const string Milestone = "v1.20-step05-functional-qa-full-demo-verification";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 5 Functional QA Self-Tests")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            Check(ref total, ref passed, failures, "milestone", Read("MILESTONE_VERSION.txt").Trim() == Milestone);
            Check(ref total, ref passed, failures, "Step 4 page/asset plan retained", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json")));
            Check(ref total, ref passed, failures, "Step 5 QA matrix", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP05_FUNCTIONAL_QA_MATRIX.json")));
            Check(ref total, ref passed, failures, "Step 5 full-flow checklist", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP05_FULL_DEMO_FLOW_CHECKLIST.md")));
            Check(ref total, ref passed, failures, "Step 5 route acceptance", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json")));
            Check(ref total, ref passed, failures, "Step 5 PlayMode navigation test", File.Exists(Path("Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs")));

            string plan = Read("Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json");
            Check(ref total, ref passed, failures, "36 page plan entries", Count(plan, "\"pageKey\"") == 36);
            Check(ref total, ref passed, failures, "world map asset plan includes back control", plan.Contains("\"back/home control\""));
            Check(ref total, ref passed, failures, "gameplay asset plan includes board/VFX", plan.Contains("\"board frame/cells\"") && plan.Contains("\"VFX\""));

            string routeAcceptance = Read("Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json");
            Check(ref total, ref passed, failures, "36-screen acceptance graph", routeAcceptance.Contains("\"screenCount\": 36") && routeAcceptance.Contains("\"reachableFromSplashCount\": 36"));
            Check(ref total, ref passed, failures, "individual ranking acceptance parent", routeAcceptance.Contains("\"IndividualRanking\": [") && routeAcceptance.Contains("\"Home\""));

            CheckBack(ref total, ref passed, failures, ScreenId.WorldMap, ScreenId.Home);
            CheckBack(ref total, ref passed, failures, ScreenId.ChapterMap, ScreenId.WorldMap);
            CheckBack(ref total, ref passed, failures, ScreenId.PreLevel, ScreenId.ChapterMap);
            CheckBack(ref total, ref passed, failures, ScreenId.EventDetail, ScreenId.Events);
            CheckBack(ref total, ref passed, failures, ScreenId.TeamRanking, ScreenId.TeamHub);
            CheckBack(ref total, ref passed, failures, ScreenId.TeamChat, ScreenId.TeamHub);
            CheckBack(ref total, ref passed, failures, ScreenId.FriendsList, ScreenId.TeamHub);
            CheckBack(ref total, ref passed, failures, ScreenId.SpecialOffer, ScreenId.Shop);
            CheckBack(ref total, ref passed, failures, ScreenId.LanguageSelect, ScreenId.Settings);
            CheckBack(ref total, ref passed, failures, ScreenId.Login, ScreenId.Profile);
            CheckBack(ref total, ref passed, failures, ScreenId.Inventory, ScreenId.Home);

            string router = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            Check(ref total, ref passed, failures, "source-bound map back dispatch", router.Contains("NavigateBackFrom(ScreenId expectedSource)"));
            Check(ref total, ref passed, failures, "world map back uses source-bound callback", router.Contains("() => NavigateBackFrom(ScreenId.WorldMap)"));
            Check(ref total, ref passed, failures, "chapter map back uses source-bound callback", router.Contains("() => NavigateBackFrom(ScreenId.ChapterMap)"));
            Check(ref total, ref passed, failures, "pre-level visible back uses source-bound callback", router.Contains("() => NavigateBackFrom(ScreenId.PreLevel)"));
            Check(ref total, ref passed, failures, "world map expanded touch target", router.Contains("1450,\n                        210,\n                        205,"));
            Check(ref total, ref passed, failures, "hardware back central policy retained", router.Contains("ScreenNavigationPolicy.TryGetBackDestination(current, out destination)"));

            string playMode = Read("Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs");
            Check(ref total, ref passed, failures, "actual map back hotspots exercised", playMode.Contains("MapBackHotspots_FollowChapterMapWorldMapHomeHierarchy"));
            Check(ref total, ref passed, failures, "pre-level visible back exercised", playMode.Contains("PreLevelVisibleBackHotspot_ReturnsToChapterMap"));
            Check(ref total, ref passed, failures, "extended demo routes exercised", playMode.Contains("ExtendedDemoRoutes_RenderWithAtLeastOneInteraction"));
            Check(ref total, ref passed, failures, "friends return exercised", playMode.Contains("NavigateBack_FriendsListReturnsToTeamHub"));

            string demoState = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs");
            Check(ref total, ref passed, failures, "demo has no gold grant", !demoState.Contains("AddGold("));
            Check(ref total, ref passed, failures, "demo has no gem grant", !demoState.Contains("AddGems("));
            Check(ref total, ref passed, failures, "demo has no energy grant", !demoState.Contains("AddEnergy("));
            Check(ref total, ref passed, failures, "demo has no PlayerPrefs persistence", !demoState.Contains("PlayerPrefs"));

            string activation = Read("Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json");
            Check(ref total, ref passed, failures, "functionality-first demo enabled", activation.Contains("\"functionalityFirstDemoEnabled\": true"));
            Check(ref total, ref passed, failures, "map back enabled", activation.Contains("\"mapBackNavigationEnabled\": true"));
            Check(ref total, ref passed, failures, "social backend disabled", activation.Contains("\"productionSocialBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "chat backend disabled", activation.Contains("\"productionChatBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "identity backend disabled", activation.Contains("\"productionIdentityBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "leaderboard backend disabled", activation.Contains("\"productionLeaderboardBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "liveops reward authority disabled", activation.Contains("\"productionLiveOpsRewardAuthorityEnabled\": false"));
            Check(ref total, ref passed, failures, "automatic demo economy grant disabled", activation.Contains("\"automaticEconomyGrantFromDemoEnabled\": false"));
            Check(ref total, ref passed, failures, "automatic publish disabled", activation.Contains("\"automaticProductionPublishEnabled\": false"));

            string gameState = Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs");
            Check(ref total, ref passed, failures, "save-v6 retained", gameState.Contains("public int saveVersion = 6;"));
            string packages = Read("Packages/manifest.json");
            Check(ref total, ref passed, failures, "Addressables 2.8.1 retained", packages.Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check(ref total, ref passed, failures, "no bundled AAB", Directory.GetFiles(Directory.GetCurrentDirectory(), "*.aab", SearchOption.AllDirectories).Length == 0);

            DemoFeatureState.ResetSession();
            Check(ref total, ref passed, failures, "event claim first succeeds", DemoFeatureState.TryClaimEventReward());
            Check(ref total, ref passed, failures, "event claim repeat blocked", !DemoFeatureState.TryClaimEventReward());
            Check(ref total, ref passed, failures, "friend request first succeeds", DemoFeatureState.TryRequestFriendEnergy());
            Check(ref total, ref passed, failures, "friend request repeat blocked", !DemoFeatureState.TryRequestFriendEnergy());
            DemoFeatureState.SetLanguage("en");
            Check(ref total, ref passed, failures, "language EN", DemoFeatureState.LanguageCode == "en");
            DemoFeatureState.SetLanguage("unexpected");
            Check(ref total, ref passed, failures, "language fallback RU", DemoFeatureState.LanguageCode == "ru");
            DemoFeatureState.SignInGuest();
            Check(ref total, ref passed, failures, "guest session", DemoFeatureState.GuestSignedIn);
            Check(ref total, ref passed, failures, "team message counter", DemoFeatureState.RecordTeamMessage() == 1);
            DemoFeatureState.ResetSession();

            if (failures.Count > 0)
            {
                string summary = string.Join(Environment.NewLine, failures.ToArray());
                Debug.LogError("v1.20 Step 5 self-tests failed: " + passed + "/" + total + " passed." + Environment.NewLine + summary);
                throw new InvalidOperationException("v1.20 Step 5 self-tests failed." + Environment.NewLine + summary);
            }

            Debug.Log("v1.20 Step 5 self-tests passed: " + passed + "/" + total + ". Run complete EditMode and PlayMode suites for executable Unity evidence.");
        }

        static void CheckBack(ref int total, ref int passed, List<string> failures, ScreenId source, ScreenId expected)
        {
            ScreenId actual;
            bool ok = ScreenNavigationPolicy.TryGetBackDestination(source, out actual) && actual == expected;
            Check(ref total, ref passed, failures, "back " + source + " -> " + expected, ok);
        }

        static int Count(string text, string token)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(token)) return 0;
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }

        static void Check(ref int total, ref int passed, List<string> failures, string label, bool condition)
        {
            total++;
            if (condition) passed++;
            else failures.Add("FAIL: " + label);
        }

        static string Path(string relative)
        {
            return System.IO.Path.Combine(Directory.GetCurrentDirectory(), relative.Replace('/', System.IO.Path.DirectorySeparatorChar));
        }

        static string Read(string relative)
        {
            string path = Path(relative);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }
    }
}
