#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20CoreLoopReferenceMatchR3SelfTest
    {
        const string ArtRoot = Step20CoreLoopReferenceMatchR3Builder.ArtRoot;
        const string PageRoot = Step20CoreLoopReferenceMatchR3Builder.PageRoot;
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r3.json";
        const int ExpectedChecks = 137;

        struct AssetExpectation
        {
            public string File;
            public int Width;
            public int Height;
            public AssetExpectation(string file, int width, int height)
            {
                File = file;
                Width = width;
                Height = height;
            }
        }

        static readonly AssetExpectation[] Assets =
        {
            new AssetExpectation("prelevel_top_hud.png", 941, 145),
            new AssetExpectation("prelevel_chapter_band.png", 941, 260),
            new AssetExpectation("prelevel_goals_left_scene.png", 190, 445),
            new AssetExpectation("prelevel_goals_panel.png", 561, 445),
            new AssetExpectation("prelevel_goals_right_scene.png", 190, 445),
            new AssetExpectation("prelevel_boosters_left_scene.png", 105, 285),
            new AssetExpectation("prelevel_boosters_panel.png", 731, 285),
            new AssetExpectation("prelevel_boosters_right_scene.png", 105, 285),
            new AssetExpectation("prelevel_reward_left_scene.png", 95, 300),
            new AssetExpectation("prelevel_reward_panel.png", 751, 300),
            new AssetExpectation("prelevel_reward_right_scene.png", 95, 300),
            new AssetExpectation("prelevel_bottom_controls.png", 941, 237),
            new AssetExpectation("gameplay_top_shell.png", 941, 357),
            new AssetExpectation("gameplay_left_environment.png", 79, 942),
            new AssetExpectation("gameplay_right_environment.png", 76, 942),
            new AssetExpectation("gameplay_booster_shell.png", 941, 205),
            new AssetExpectation("gameplay_score_footer.png", 941, 168),
            new AssetExpectation("pause_header.png", 640, 150),
            new AssetExpectation("pause_left_border.png", 30, 925),
            new AssetExpectation("pause_goals_panel.png", 580, 270),
            new AssetExpectation("pause_boosters_panel.png", 580, 200),
            new AssetExpectation("pause_buttons_panel.png", 580, 455),
            new AssetExpectation("pause_right_border.png", 30, 925),
            new AssetExpectation("pause_footer.png", 640, 50)
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 3.3R.3 Self-Tests")]
        public static void Run()
        {
            Step20CoreLoopReferenceMatchR3Builder.BuildAll(false);

            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            List<string> builderFailures = Step20CoreLoopReferenceMatchR3Builder.ValidateAll();
            Check(ref total, ref passed, failures, builderFailures.Count == 0, "R3 builder validates 3/3 pages");

            foreach (AssetExpectation asset in Assets)
            {
                string path = ArtRoot + "/" + asset.File;
                Check(ref total, ref passed, failures, File.Exists(path), "asset exists: " + asset.File);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Check(ref total, ref passed, failures, texture != null, "asset imported: " + asset.File);
                bool sizeOk = texture != null && texture.width == asset.Width && texture.height == asset.Height;
                Check(ref total, ref passed, failures, sizeOk && !(asset.Width == 941 && asset.Height == 1672), "asset dimensions/non-full-screen: " + asset.File);
            }

            CheckPage(ref total, ref passed, failures, ScreenId.PreLevel, 12, "32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195", false, false);
            CheckPage(ref total, ref passed, failures, ScreenId.Gameplay, 5, "d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800", true, false);
            CheckPage(ref total, ref passed, failures, ScreenId.Pause, 7, "31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716", false, true);

            string activation = File.ReadAllText(Activation);
            CheckFlag(ref total, ref passed, failures, activation, "referenceMatchRebuildAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "referenceDerivedProductionComponentsAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "preLevelReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "gameplayShellReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "pauseModalReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "liveGameplayBoardPreservationRequired", true);
            CheckFlag(ref total, ref passed, failures, activation, "liveGameplayPauseBackdropRequired", true);
            CheckFlag(ref total, ref passed, failures, activation, "staticComponentPixelReconstructionVerified", true);
            CheckFlag(ref total, ref passed, failures, activation, "manualUnityRenderReviewRequired", true);
            CheckFlag(ref total, ref passed, failures, activation, "finalReferenceMatchSignoffGranted", false);
            CheckFlag(ref total, ref passed, failures, activation, "fullScreenScreenshotRuntimeUseAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "screenshotDeletionAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "lockedReferenceOverwriteAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "automaticArtGenerationAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "genericStyleSubstitutionAllowed", false);
            CheckFlag(ref total, ref passed, failures, activation, "gameplayBoardReplacementAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "gameplayRulesMutationAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "saveEconomyRewardMutationAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "productionContentPublishAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "playConsoleActionEnabled", false);

            string proof = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r3_pixel_proof.json");
            Check(ref total, ref passed, failures, proof.Contains("\"exactPixelReconstruction\": true"), "Pre-Level full-page exact proof");
            Check(ref total, ref passed, failures, proof.Contains("\"differentPixels\": 0"), "Pre-Level zero different pixels");
            Check(ref total, ref passed, failures, proof.Contains("\"maxChannelError\": 0"), "Pre-Level zero channel error");
            Check(ref total, ref passed, failures, proof.Contains("\"exactShellPixelReconstruction\": true"), "Gameplay shell exact proof");
            Check(ref total, ref passed, failures, proof.Contains("\"dynamicBoardPixelCount\""), "Gameplay proof excludes live board area");
            Check(ref total, ref passed, failures, Count(proof, "\"differentPixelsWithinScope\": 0") == 2, "Gameplay and Pause scoped difference = 0");
            Check(ref total, ref passed, failures, proof.Contains("\"exactModalPixelReconstruction\": true"), "Pause modal exact proof");
            Check(ref total, ref passed, failures, proof.Contains("\"provisional_captured_reference\""), "Pause reference remains provisional");
            Check(ref total, ref passed, failures, proof.Contains("\"live_gameplay_plus_runtime_dim_overlay\""), "Pause background remains live gameplay");

            string baseBuilder = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            Check(ref total, ref passed, failures, baseBuilder.Contains("Step20CoreLoopReferenceMatchR3Builder.BuildAll(false);"), "base Step 3.3 builder re-applies R3 pages");

            string router = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"Boosters\", 110, 850, 720, 285"), "Pre-Level booster hotspot corrected");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"StartLevel\", 205, 1435, 535, 190"), "Pre-Level start hotspot corrected");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"Resume\", 235, 910, 470, 105"), "Pause resume hotspot corrected");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"Restart\", 235, 1025, 470, 105"), "Pause restart hotspot corrected");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"Settings\", 235, 1140, 470, 105"), "Pause settings hotspot corrected");
            Check(ref total, ref passed, failures, router.Contains("Hotspot(parent, \"Exit\", 235, 1255, 470, 105"), "Pause exit hotspot corrected");

            string milestone = File.ReadAllText("MILESTONE_VERSION.txt").Trim();
            Check(ref total, ref passed, failures, milestone == "v1.20-step03.3R3-prelevel-gameplay-pause-visual-rebuild", "milestone string pinned");

            if (total != ExpectedChecks)
                failures.Add("Self-test definition mismatch: expected " + ExpectedChecks + " checks but executed " + total + ".");

            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R.3 self-tests passed: " + passed + "/" + total);
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "v1.20 Step 3.3R.3 self-tests passed: " + passed + "/" + total, "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R.3 self-tests failed: " + passed + "/" + total + "\n" + report);
            throw new InvalidOperationException("v1.20 Step 3.3R.3 self-tests failed.\n" + report);
        }

        static void CheckPage(ref int total, ref int passed, List<string> failures, ScreenId id, int componentCount, string referenceSha, bool preserveBoard, bool pauseOverGameplay)
        {
            string path = PageRoot + "/" + id + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Check(ref total, ref passed, failures, prefab != null, "prefab exists: " + id);
            ProductionReferenceMatchR3PageMarker marker = prefab != null ? prefab.GetComponent<ProductionReferenceMatchR3PageMarker>() : null;
            Check(ref total, ref passed, failures, marker != null, "R3 marker exists: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ReferenceMatchVersion == Step20CoreLoopReferenceMatchR3Builder.Version, "R3 version: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ProductionComponentCount == componentCount, "component count: " + id);
            Check(ref total, ref passed, failures, marker != null && !marker.FullScreenScreenshotDependency, "no full-screen dependency: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ReferenceSha256 == referenceSha, "reference SHA-256: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.PreservesLiveGameplayBoard == preserveBoard, "live-board boundary: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.OverlaysLiveGameplayForPause == pauseOverGameplay, "pause-live-gameplay boundary: " + id);

            bool dependenciesClean = prefab != null;
            if (prefab != null)
            {
                foreach (string raw in AssetDatabase.GetDependencies(path, true))
                {
                    string dep = raw.Replace('\\', '/');
                    if (dep.Contains("/Resources/DesignScreens/") || dep.Contains("/Art/ReferenceOnly/"))
                    {
                        dependenciesClean = false;
                        break;
                    }
                }
            }
            Check(ref total, ref passed, failures, dependenciesClean, "production-only dependencies: " + id);
        }

        static void CheckFlag(ref int total, ref int passed, List<string> failures, string json, string key, bool expected)
        {
            string needle = "\"" + key + "\": " + (expected ? "true" : "false");
            Check(ref total, ref passed, failures, json.Contains(needle), "activation flag " + key + " = " + expected);
        }

        static int Count(string text, string needle)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }
            return count;
        }

        static void Check(ref int total, ref int passed, List<string> failures, bool condition, string label)
        {
            total++;
            if (condition)
            {
                passed++;
                return;
            }
            failures.Add(label);
        }
    }
}
#endif
