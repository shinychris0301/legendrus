using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20FunctionalGameFlowSelfTest
    {
        const string Milestone = "v1.20-step04-functional-game-flow-demo-logic";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 4 Functional Flow Self-Tests")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            Check(ref total, ref passed, failures, "milestone", Read("MILESTONE_VERSION.txt").Trim() == Milestone);
            Check(ref total, ref passed, failures, "screen navigation policy", File.Exists(Path("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs")));
            Check(ref total, ref passed, failures, "demo page runtime", File.Exists(Path("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFunctionalPageRuntime.cs")));
            Check(ref total, ref passed, failures, "demo feature state", File.Exists(Path("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs")));
            Check(ref total, ref passed, failures, "function asset plan json", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json")));
            Check(ref total, ref passed, failures, "function asset plan csv", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.csv")));
            Check(ref total, ref passed, failures, "route graph", File.Exists(Path("Assets/LegendsOfRus/Documentation/V1_20_STEP04_ROUTE_GRAPH.json")));
            Check(ref total, ref passed, failures, "activation", File.Exists(Path("Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json")));

            CheckBack(ref total, ref passed, failures, ScreenId.WorldMap, ScreenId.Home);
            CheckBack(ref total, ref passed, failures, ScreenId.ChapterMap, ScreenId.WorldMap);
            CheckBack(ref total, ref passed, failures, ScreenId.PreLevel, ScreenId.ChapterMap);
            CheckBack(ref total, ref passed, failures, ScreenId.EventDetail, ScreenId.Events);
            CheckBack(ref total, ref passed, failures, ScreenId.TeamRanking, ScreenId.TeamHub);
            CheckBack(ref total, ref passed, failures, ScreenId.TeamChat, ScreenId.TeamHub);
            CheckBack(ref total, ref passed, failures, ScreenId.SpecialOffer, ScreenId.Shop);
            CheckBack(ref total, ref passed, failures, ScreenId.LanguageSelect, ScreenId.Settings);
            CheckBack(ref total, ref passed, failures, ScreenId.Login, ScreenId.Profile);
            CheckBack(ref total, ref passed, failures, ScreenId.Inventory, ScreenId.Home);

            Check(ref total, ref passed, failures, "legacy enum loading", (int)ScreenId.Loading == 0);
            Check(ref total, ref passed, failures, "legacy enum home", (int)ScreenId.Home == 1);
            Check(ref total, ref passed, failures, "legacy enum world map", (int)ScreenId.WorldMap == 2);
            Check(ref total, ref passed, failures, "legacy enum chapter map", (int)ScreenId.ChapterMap == 3);
            Check(ref total, ref passed, failures, "legacy enum chapter complete", (int)ScreenId.ChapterComplete == 21);
            Check(ref total, ref passed, failures, "new enum starts append-only", (int)ScreenId.SplashTitle == 22);
            Check(ref total, ref passed, failures, "new enum ends expected", (int)ScreenId.EmptyErrorState == 36);

            ScreenId[] demoPages =
            {
                ScreenId.SplashTitle, ScreenId.EventDetail, ScreenId.IndividualRanking,
                ScreenId.TeamHub, ScreenId.TeamRanking, ScreenId.TeamChat,
                ScreenId.FriendsList, ScreenId.Inventory, ScreenId.SpecialOffer,
                ScreenId.LanguageSelect, ScreenId.Login, ScreenId.TutorialOverlay,
                ScreenId.OfflineState, ScreenId.MaintenanceState, ScreenId.EmptyErrorState
            };
            for (int i = 0; i < demoPages.Length; i++)
                Check(ref total, ref passed, failures, "demo route " + demoPages[i], DemoFunctionalPageRuntime.IsDemoRoute(demoPages[i]));

            string router = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            Check(ref total, ref passed, failures, "world map back hotspot", router.Contains("case ScreenId.WorldMap:") && router.Contains("Hotspot(parent, \"Back\", 0, 1480, 125, 150, NavigateBack)"));
            Check(ref total, ref passed, failures, "chapter map back hotspot", router.Contains("Hotspot(parent, \"Back\", 0, 1400, 205, 272, NavigateBack)"));
            Check(ref total, ref passed, failures, "hardware back central policy", router.Contains("ScreenNavigationPolicy.TryGetBackDestination"));
            Check(ref total, ref passed, failures, "event detail route", router.Contains("Show(ScreenId.EventDetail)"));
            Check(ref total, ref passed, failures, "team hub route", router.Contains("Show(ScreenId.TeamHub)"));
            Check(ref total, ref passed, failures, "inventory route", router.Contains("Show(ScreenId.Inventory)"));
            Check(ref total, ref passed, failures, "language route", router.Contains("Show(ScreenId.LanguageSelect)"));
            Check(ref total, ref passed, failures, "login route", router.Contains("Show(ScreenId.Login)"));
            Check(ref total, ref passed, failures, "offline route", router.Contains("Show(ScreenId.OfflineState)"));
            Check(ref total, ref passed, failures, "maintenance route", router.Contains("Show(ScreenId.MaintenanceState)"));
            Check(ref total, ref passed, failures, "error route", router.Contains("Show(ScreenId.EmptyErrorState)"));
            Check(ref total, ref passed, failures, "tutorial route", router.Contains("Show(ScreenId.TutorialOverlay)"));

            string bootstrap = Read("Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs");
            Check(ref total, ref passed, failures, "splash boot entry", bootstrap.Contains("router.Show(ScreenId.SplashTitle)"));

            string demoState = Read("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs");
            Check(ref total, ref passed, failures, "demo has no gold grant", !demoState.Contains("AddGold("));
            Check(ref total, ref passed, failures, "demo has no gem grant", !demoState.Contains("AddGems("));
            Check(ref total, ref passed, failures, "demo has no energy grant", !demoState.Contains("AddEnergy("));
            Check(ref total, ref passed, failures, "demo has no playerprefs persistence", !demoState.Contains("PlayerPrefs"));

            string activation = Read("Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json");
            Check(ref total, ref passed, failures, "functionality first enabled", activation.Contains("\"functionalityFirstDemoEnabled\": true"));
            Check(ref total, ref passed, failures, "map back enabled", activation.Contains("\"mapBackNavigationEnabled\": true"));
            Check(ref total, ref passed, failures, "social backend disabled", activation.Contains("\"productionSocialBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "identity backend disabled", activation.Contains("\"productionIdentityBackendEnabled\": false"));
            Check(ref total, ref passed, failures, "demo economy grant disabled", activation.Contains("\"automaticEconomyGrantFromDemoEnabled\": false"));
            Check(ref total, ref passed, failures, "production publish disabled", activation.Contains("\"automaticProductionPublishEnabled\": false"));

            DemoFeatureState.ResetSession();
            Check(ref total, ref passed, failures, "event demo first claim", DemoFeatureState.TryClaimEventReward());
            Check(ref total, ref passed, failures, "event demo repeat blocked", !DemoFeatureState.TryClaimEventReward());
            Check(ref total, ref passed, failures, "friend request first", DemoFeatureState.TryRequestFriendEnergy());
            Check(ref total, ref passed, failures, "friend request repeat blocked", !DemoFeatureState.TryRequestFriendEnergy());
            DemoFeatureState.SetLanguage("en");
            Check(ref total, ref passed, failures, "language en", DemoFeatureState.LanguageCode == "en");
            DemoFeatureState.SetLanguage("bad");
            Check(ref total, ref passed, failures, "language fallback ru", DemoFeatureState.LanguageCode == "ru");
            DemoFeatureState.SignInGuest();
            Check(ref total, ref passed, failures, "guest state", DemoFeatureState.GuestSignedIn);
            Check(ref total, ref passed, failures, "team message counter", DemoFeatureState.RecordTeamMessage() == 1);
            DemoFeatureState.ResetSession();

            if (failures.Count > 0)
            {
                string summary = string.Join(Environment.NewLine, failures.ToArray());
                Debug.LogError("v1.20 Step 4 self-tests failed: " + passed + "/" + total + " passed." + Environment.NewLine + summary);
                throw new InvalidOperationException("v1.20 Step 4 self-tests failed." + Environment.NewLine + summary);
            }

            Debug.Log("v1.20 Step 4 self-tests passed: " + passed + "/" + total);
        }

        static void CheckBack(ref int total, ref int passed, List<string> failures, ScreenId source, ScreenId expected)
        {
            ScreenId actual;
            bool ok = ScreenNavigationPolicy.TryGetBackDestination(source, out actual) && actual == expected;
            Check(ref total, ref passed, failures, "back " + source + " -> " + expected, ok);
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
