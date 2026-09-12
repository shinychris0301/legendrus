#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionUiFix01SelfTest
    {
        const int ExpectedChecks = 88;
        const string PageRoot = Step20ProductionUiFix01SerializedPrefabBuilder.PageRoot;
        const string AssetRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01";
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_fix01_v120_step03_3r.json";

        static readonly ScreenId[] Pages =
        {
            ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
            ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause, ScreenId.Victory,
            ScreenId.Fail, ScreenId.ChapterComplete
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 3.3R-FIX01 Self-Tests")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            bool built = true;
            try
            {
                Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false);
            }
            catch (Exception e)
            {
                built = false;
                failures.Add("FIX01 builder threw " + e.GetType().Name + ": " + e.Message);
            }
            Check(ref total, ref passed, failures, built, "serialized-prefab builder completes");
            List<string> validation = Step20ProductionUiFix01SerializedPrefabBuilder.ValidateAll();
            Check(ref total, ref passed, failures, validation.Count == 0, "serialized-prefab validation passes");
            if (validation.Count > 0) failures.AddRange(validation);
            Check(ref total, ref passed, failures, File.Exists(Step20ProductionUiFix01SerializedPrefabBuilder.LockPath), "FIX01 delivery lock exists");

            foreach (ScreenId id in Pages)
            {
                string path = PageRoot + "/" + id + ".prefab";
                Check(ref total, ref passed, failures, File.Exists(path), "serialized file exists: " + id);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Check(ref total, ref passed, failures, prefab != null, "AssetDatabase imports prefab: " + id);
                GameObject resource = Resources.Load<GameObject>("ProductionUI/Pages/" + id);
                Check(ref total, ref passed, failures, resource != null, "Resources loads prefab: " + id);
                ProductionUiFix01PageBootstrap bootstrap = prefab != null ? prefab.GetComponent<ProductionUiFix01PageBootstrap>() : null;
                Check(ref total, ref passed, failures, bootstrap != null && bootstrap.ScreenId == id, "FIX01 bootstrap: " + id);
                ProductionCoreLoopPageMarker core = prefab != null ? prefab.GetComponent<ProductionCoreLoopPageMarker>() : null;
                Check(ref total, ref passed, failures, core != null && core.ScreenId == id && core.ScreenshotFree, "screenshot-free core marker: " + id);
            }

            Check(ref total, ref passed, failures, Directory.GetFiles(AssetRoot + "/Icons", "*.png").Length == 20, "20 standalone icons");
            Check(ref total, ref passed, failures, Directory.GetFiles(AssetRoot + "/Kit", "*.png").Length == 24, "24 runtime-reachable kit primitives");
            Check(ref total, ref passed, failures, Directory.GetFiles(AssetRoot + "/Gems", "*.png").Length == 6, "6 runtime-reachable gem sprites");
            foreach (string file in new[] { "bg_home_clean.png", "bg_worldmap_clean.png", "bg_chaptermap_clean.png", "bg_prelevel_clean.png", "bg_gameplay_clean.png" })
                Check(ref total, ref passed, failures, File.Exists(AssetRoot + "/" + file), "environment plate: " + file);
            Check(ref total, ref passed, failures, File.Exists(AssetRoot + "/hero_ivan_firebird.png"), "standalone hero layer");
            Check(ref total, ref passed, failures, File.Exists(AssetRoot + "/portrait_ivan.png"), "standalone portrait layer");
            Check(ref total, ref passed, failures, File.Exists(AssetRoot + "/ribbon_red.png"), "standalone chapter ribbon");
            Check(ref total, ref passed, failures, File.Exists(AssetRoot + "/title_plaque_navy.png"), "standalone title plaque");

            string runtime = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs");
            Check(ref total, ref passed, failures, !runtime.Contains("DesignScreens"), "FIX01 runtime has no DesignScreens dependency");
            Check(ref total, ref passed, failures, !runtime.Contains("ReferenceOnly"), "FIX01 runtime has no ReferenceOnly dependency");
            Check(ref total, ref passed, failures, !runtime.Contains("ReferenceMatch/\"") && !runtime.Contains("ReferenceMatchR3/\""), "FIX01 runtime has no legacy screenshot-strip asset path");
            string loader = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs");
            Check(ref total, ref passed, failures, loader.Contains("ProductionUiFix01Runtime.TryBuild(id, parent)"), "direct asset-based runtime fallback wired");

            foreach (string file in new[]
            {
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs"
            })
            {
                string text = File.ReadAllText(file);
                Check(ref total, ref passed, failures, text.Contains("Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled()"), "legacy auto-build guard: " + Path.GetFileName(file));
                Check(ref total, ref passed, failures, text.Contains("Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false)"), "legacy manual rebuild restores FIX01: " + Path.GetFileName(file));
            }

            string activation = File.ReadAllText(Activation);
            CheckFlag(ref total, ref passed, failures, activation, "serializedPrefabDeliveryEnabled", true);
            CheckFlag(ref total, ref passed, failures, activation, "directRuntimeAssetFallbackEnabled", true);
            CheckFlag(ref total, ref passed, failures, activation, "independentRuntimeAssetCompositionEnabled", true);
            CheckFlag(ref total, ref passed, failures, activation, "designScreensRuntimeFallbackEnabled", false);
            CheckFlag(ref total, ref passed, failures, activation, "referenceOnlyRuntimeUseEnabled", false);
            CheckFlag(ref total, ref passed, failures, activation, "screenshotCropProductionUseEnabled", false);
            CheckFlag(ref total, ref passed, failures, activation, "productionContentPublishAuthorized", false);
            CheckFlag(ref total, ref passed, failures, activation, "finalVisualSignoffGranted", false);

            Check(ref total, ref passed, failures, File.ReadAllText("Packages/manifest.json").Contains("\"com.unity.addressables\": \"2.8.1\""), "Addressables 2.8.1 retained");
            Check(ref total, ref passed, failures, File.Exists("README_COMPILE_HOTFIX05.md"), "Compile Hotfix 05 retained");
            Check(ref total, ref passed, failures, File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs").Contains("SetPixelRect(rt, 79, 357, 786, 942)"), "live Gameplay board rectangle retained");
            Check(ref total, ref passed, failures, Directory.GetFiles("Assets/LegendsOfRus/Resources/DesignScreens", "*.png", SearchOption.TopDirectoryOnly).Length == 22, "22 captured references retained only for QA/later pages");

            string[] assetNames = Directory.GetFiles(AssetRoot, "*.png", SearchOption.AllDirectories).Select(p => Path.GetFileName(p)).ToArray();
            Check(ref total, ref passed, failures, !assetNames.Any(n => n.Contains("_component") || n.Contains("_scene") || n.Contains("_shell")), "FIX01 delivery contains no legacy screenshot-strip asset names");

            if (total != ExpectedChecks)
                failures.Add("Self-test definition mismatch: expected " + ExpectedChecks + " checks but executed " + total + ".");

            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R-FIX01 self-tests passed: " + passed + "/" + total);
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "v1.20 Step 3.3R-FIX01 self-tests passed: " + passed + "/" + total, "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R-FIX01 self-tests failed: " + passed + "/" + total + "\n" + report);
            throw new InvalidOperationException("v1.20 Step 3.3R-FIX01 self-tests failed.\n" + report);
        }

        static void CheckFlag(ref int total, ref int passed, List<string> failures, string json, string key, bool expected)
        {
            string needle = "\"" + key + "\": " + (expected ? "true" : "false");
            Check(ref total, ref passed, failures, json.Contains(needle), "activation flag " + key + " = " + expected);
        }

        static void Check(ref int total, ref int passed, List<string> failures, bool condition, string label)
        {
            total++;
            if (condition) passed++;
            else failures.Add("FAIL: " + label);
        }
    }
}
#endif
