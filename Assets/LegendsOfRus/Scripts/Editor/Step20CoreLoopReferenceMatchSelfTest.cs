#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20CoreLoopReferenceMatchSelfTest
    {
        const string ArtRoot = Step20CoreLoopReferenceMatchBuilder.ArtRoot;
        const string PageRoot = Step20CoreLoopReferenceMatchBuilder.PageRoot;
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r2.json";
        const int ExpectedChecks = 85;

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
            new AssetExpectation("home_top_hud.png", 941, 150),
            new AssetExpectation("home_logo_scene.png", 941, 350),
            new AssetExpectation("home_hero_scene.png", 941, 500),
            new AssetExpectation("home_play_component.png", 941, 210),
            new AssetExpectation("home_navigation_component.png", 941, 240),
            new AssetExpectation("home_quest_component.png", 941, 222),
            new AssetExpectation("worldmap_top_hud.png", 941, 135),
            new AssetExpectation("worldmap_title_component.png", 941, 150),
            new AssetExpectation("worldmap_painted_map_plate.png", 941, 1055),
            new AssetExpectation("worldmap_bottom_navigation.png", 941, 332),
            new AssetExpectation("chaptermap_top_hud.png", 941, 140),
            new AssetExpectation("chaptermap_ribbon_component.png", 941, 250),
            new AssetExpectation("chaptermap_painted_level_path.png", 941, 1060),
            new AssetExpectation("chaptermap_bottom_controls.png", 941, 222)
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 3.3R.2 Self-Tests")]
        public static void Run()
        {
            Step20CoreLoopReferenceMatchBuilder.BuildAll(false);

            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            List<string> builderFailures = Step20CoreLoopReferenceMatchBuilder.ValidateAll();
            Check(ref total, ref passed, failures, builderFailures.Count == 0, "reference-match builder validates 3/3 pages");

            foreach (AssetExpectation asset in Assets)
            {
                string path = ArtRoot + "/" + asset.File;
                Check(ref total, ref passed, failures, File.Exists(path), "asset exists: " + asset.File);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Check(ref total, ref passed, failures, texture != null, "asset imported: " + asset.File);
                bool sizeOk = texture != null && texture.width == asset.Width && texture.height == asset.Height;
                Check(ref total, ref passed, failures, sizeOk && !(asset.Width == 941 && asset.Height == 1672), "asset dimensions/non-full-screen: " + asset.File);
            }

            CheckPage(ref total, ref passed, failures, ScreenId.Home, 6, "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8");
            CheckPage(ref total, ref passed, failures, ScreenId.WorldMap, 4, "b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19");
            CheckPage(ref total, ref passed, failures, ScreenId.ChapterMap, 4, "e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf");

            string activation = File.ReadAllText(Activation);
            CheckFlag(ref total, ref passed, failures, activation, "referenceMatchRebuildAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "referenceDerivedProductionComponentsAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "homeReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "worldMapReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "chapterMapReferenceMatchCutoverAuthorized", true);
            CheckFlag(ref total, ref passed, failures, activation, "fullScreenScreenshotRuntimeUseAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "screenshotDeletionAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "lockedReferenceOverwriteAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "genericStyleSubstitutionAllowed", false);
            CheckFlag(ref total, ref passed, failures, activation, "finalReferenceMatchSignoffGranted", false);

            string proof = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r2_pixel_proof.json");
            Check(ref total, ref passed, failures, Count(proof, "\"exactPixelReconstruction\": true") == 3, "pixel proof exact reconstruction count = 3");
            Check(ref total, ref passed, failures, Count(proof, "\"differentPixels\": 0") == 3, "pixel proof different pixel count = 0 for three pages");
            Check(ref total, ref passed, failures, Count(proof, "\"maxChannelError\": 0") == 3, "pixel proof max channel error = 0 for three pages");
            Check(ref total, ref passed, failures, proof.Contains("\"Home\""), "pixel proof contains Home");
            Check(ref total, ref passed, failures, proof.Contains("\"WorldMap\""), "pixel proof contains WorldMap");
            Check(ref total, ref passed, failures, proof.Contains("\"ChapterMap\""), "pixel proof contains ChapterMap");

            string baseBuilder = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            Check(ref total, ref passed, failures, baseBuilder.Contains("Step20CoreLoopReferenceMatchBuilder.BuildAll(false);"), "base Step 3.3 builder re-applies R2 pages");

            string milestone = File.ReadAllText("MILESTONE_VERSION.txt").Trim();
            Check(ref total, ref passed, failures, milestone == "v1.20-step03.3R2-home-world-map-chapter-map-visual-rebuild", "milestone string pinned");

            if (total != ExpectedChecks)
                failures.Add("Self-test definition mismatch: expected " + ExpectedChecks + " checks but executed " + total + ".");

            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R.2 self-tests passed: " + passed + "/" + total);
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "v1.20 Step 3.3R.2 self-tests passed: " + passed + "/" + total, "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R.2 self-tests failed: " + passed + "/" + total + "\n" + report);
            throw new InvalidOperationException("v1.20 Step 3.3R.2 self-tests failed.\n" + report);
        }

        static void CheckPage(ref int total, ref int passed, List<string> failures, ScreenId id, int componentCount, string referenceSha)
        {
            string path = PageRoot + "/" + id + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Check(ref total, ref passed, failures, prefab != null, "prefab exists: " + id);
            ProductionReferenceMatchPageMarker marker = prefab != null ? prefab.GetComponent<ProductionReferenceMatchPageMarker>() : null;
            Check(ref total, ref passed, failures, marker != null, "reference marker exists: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ReferenceMatchVersion == Step20CoreLoopReferenceMatchBuilder.Version, "reference version: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ProductionComponentCount == componentCount, "component count: " + id);
            Check(ref total, ref passed, failures, marker != null && !marker.FullScreenScreenshotDependency, "no full-screen dependency flag: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ReferenceMatchStaticComposition, "static composition reference-match flag: " + id);
            Check(ref total, ref passed, failures, marker != null && marker.ReferenceSha256 == referenceSha, "reference SHA-256: " + id);

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
