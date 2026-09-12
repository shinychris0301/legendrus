#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20CoreLoopPageSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 3.3 Self-Tests")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            Check(ref total, ref passed, failures, ProductionCoreLoopPageRegistry.CutoverPageCount == 10, "10 scoped core-loop cutover routes");
            string[] configFlags =
            {
                "\"coreLoopScreenshotRuntimeCutoverAuthorized\": true",
                "\"screenshotDeletionAuthorized\": false",
                "\"missingProductionPrefabMayUseScreenshot\": false",
                "\"lockedArtOverwriteAuthorized\": false",
                "\"productionContentPublishAuthorized\": false"
            };
            string config = File.ReadAllText("Assets/LegendsOfRus/Resources/Config/production_ui_core_loop_v120_step03_3.json");
            foreach (string flag in configFlags)
                Check(ref total, ref passed, failures, config.Contains(flag), "activation flag: " + flag);

            ScreenId[] pages =
            {
                ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
                ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause, ScreenId.Victory,
                ScreenId.Fail, ScreenId.ChapterComplete
            };
            foreach (ScreenId id in pages)
            {
                Check(ref total, ref passed, failures, ProductionCoreLoopPageRegistry.IsCutover(id), "cutover route: " + id);
                Check(ref total, ref passed, failures, ProductionCoreLoopPageRegistry.ResourceKey(id) == "ProductionUI/Pages/" + id, "resource key: " + id);
            }

            Check(ref total, ref passed, failures, Directory.GetFiles("Assets/LegendsOfRus/Resources/DesignScreens", "*.png", SearchOption.TopDirectoryOnly).Length == 22, "22 reference/placeholders retained for QA and later waves");
            Check(ref total, ref passed, failures, Directory.GetFiles(Step20CanonicalUiKitBuilder.KitRoot, "*.png", SearchOption.TopDirectoryOnly).Length == 34, "34 canonical UI sprites retained");

            string runtime = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs");
            Check(ref total, ref passed, failures, !runtime.Contains("DesignScreenRegistry"), "production runtime has no legacy registry dependency");
            Check(ref total, ref passed, failures, !runtime.Contains("DesignScreens/"), "production runtime has no DesignScreens resource key");
            Check(ref total, ref passed, failures, runtime.Contains("BuildEmergencySurface"), "screenshot-free emergency fallback exists");

            string router = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            int production = router.IndexOf("ProductionCoreLoopPageRegistry.IsCutover(id)");
            int legacy = router.IndexOf("DesignScreenRegistry.ResourceKey(id)");
            Check(ref total, ref passed, failures, production >= 0 && legacy > production, "production registry precedes legacy registry");
            Check(ref total, ref passed, failures, router.Contains("case ScreenId.WorldMap:"), "WorldMap dynamic HUD path retained");

            try
            {
                Step20CoreLoopPageBuilder.BuildAll(false);
                List<string> validation = Step20CoreLoopPageBuilder.ValidateAll();
                Check(ref total, ref passed, failures, validation.Count == 0, "10 generated prefabs validate");
                if (validation.Count > 0) failures.AddRange(validation);
            }
            catch (Exception e)
            {
                Check(ref total, ref passed, failures, false, "core-loop page build threw " + e.GetType().Name + ": " + e.Message);
            }

            foreach (ScreenId id in pages)
            {
                string path = Step20CoreLoopPageBuilder.PageRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Check(ref total, ref passed, failures, prefab != null, "prefab exists: " + id);
                if (prefab == null) continue;
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                Check(ref total, ref passed, failures, marker != null && marker.ScreenshotFree, "screenshot-free marker: " + id);
                bool clean = true;
                foreach (string dep in AssetDatabase.GetDependencies(path, true))
                    if (dep.Replace('\\','/').Contains("/Resources/DesignScreens/")) clean = false;
                Check(ref total, ref passed, failures, clean, "no screenshot dependency: " + id);
            }

            Check(ref total, ref passed, failures, File.ReadAllText("Packages/manifest.json").Contains("com.unity.addressables"), "Addressables package retained");
            Check(ref total, ref passed, failures, File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs").Contains("AdaptiveCanvasScaler"), "adaptive canvas scaler retained");
            Check(ref total, ref passed, failures, File.Exists("README_COMPILE_HOTFIX05.md"), "Compile Hotfix 05 retained");

            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3 self-tests passed: " + passed + "/" + total);
                return;
            }
            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3 self-tests failed: " + passed + "/" + total + "\n" + report);
            throw new InvalidOperationException("v1.20 Step 3.3 self-tests failed.\n" + report);
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
