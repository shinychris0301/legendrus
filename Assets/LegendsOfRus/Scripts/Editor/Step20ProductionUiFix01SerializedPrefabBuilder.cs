#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 3.3R-FIX01 deterministic prefab delivery. The shipped project
    /// already contains the prefabs, and this builder is only a repair/regeneration
    /// path. It never reads DesignScreens or ReferenceOnly artwork.
    /// </summary>
    public static class Step20ProductionUiFix01SerializedPrefabBuilder
    {
        public const string Version = "v1.20-step03.3r-fix01";
        public const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        public const string LockPath = "Assets/LegendsOfRus/Resources/ProductionUI/FIX01_SERIALIZED_DELIVERY.lock";
        public const int RequiredPageCount = 10;

        static readonly ScreenId[] RequiredPages =
        {
            ScreenId.Loading,
            ScreenId.Home,
            ScreenId.WorldMap,
            ScreenId.ChapterMap,
            ScreenId.PreLevel,
            ScreenId.Gameplay,
            ScreenId.Pause,
            ScreenId.Victory,
            ScreenId.Fail,
            ScreenId.ChapterComplete
        };

        [InitializeOnLoadMethod]
        static void QueueAutomaticRepair()
        {
            EditorApplication.delayCall += AutomaticRepair;
        }

        static void AutomaticRepair()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
            {
                EditorApplication.delayCall += AutomaticRepair;
                return;
            }
            try
            {
                // Repair only a page that actually failed validation. Loading alignment
                // must never trigger regeneration of the other accepted/local pages.
                foreach (ScreenId id in RequiredPages)
                    if (PageNeedsBuild(id)) BuildPage(id);
            }
            catch (Exception e)
            {
                Debug.LogError("v1.20 Step 3.3R-FIX01 automatic serialized-prefab repair failed: " + e.Message);
            }
        }

        [MenuItem("Legends of Rus/Production UI/Build v1.20 Step 3.3R-FIX01 Serialized Prefabs")]
        public static void BuildMenu()
        {
            BuildAll(true);
        }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 3.3R-FIX01 Runtime Delivery")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R-FIX01 runtime delivery validation passed: 10/10 serialized prefabs.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "FIX01 serialized runtime delivery validates: 10/10.", "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R-FIX01 runtime delivery validation failed.\n" + report);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", report, "OK");
        }

        public static void BuildAll(bool showDialog)
        {
            RequireEditorIdle();
            EnsureFolder(PageRoot);
            foreach (ScreenId id in RequiredPages)
                BuildPage(id);

            File.WriteAllText(LockPath, Version + "\nserialized-prefabs=10\nscreenshot-crops=forbidden\n");
            AssetDatabase.ImportAsset(LockPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join("\n", failures.ToArray()));

            Debug.Log("v1.20 Step 3.3R-FIX01 built 10 serialized production page prefabs with direct runtime fallback.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Built and validated 10 FIX01 serialized production prefabs.", "OK");
        }

        public static bool IsInstalled()
        {
            if (!File.Exists(LockPath))
                return false;
            string text = File.ReadAllText(LockPath);
            return text.Contains(Version);
        }

        public static bool NeedsBuild()
        {
            foreach (ScreenId id in RequiredPages)
                if (PageNeedsBuild(id)) return true;
            return false;
        }

        static bool PageNeedsBuild(ScreenId id)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PageRoot + "/" + id + ".prefab");
            if (prefab == null) return true;
            ProductionUiFix01PageBootstrap bootstrap = prefab.GetComponent<ProductionUiFix01PageBootstrap>();
            if (bootstrap == null || bootstrap.ScreenId != id || bootstrap.StyleVersion != Version) return true;
            if (id != ScreenId.Loading) return false;
            ProductionLoadingView view = prefab.GetComponent<ProductionLoadingView>();
            return view == null || !view.HasRequiredBindings || prefab.transform.Find(ProductionLoadingRuntime.ContentName) == null;
        }

        [MenuItem("Legends of Rus/Production UI/Loading Alignment/Build or Repair Loading Only")]
        public static void BuildLoadingAligned()
        {
            RequireEditorIdle();
            EnsureFolder(PageRoot);
            BuildPage(ScreenId.Loading);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot + "/Loading.prefab", ImportAssetOptions.ForceSynchronousImport);
            if (PageNeedsBuild(ScreenId.Loading))
                throw new InvalidOperationException("Saved Loading alignment prefab failed the revision/binding read-back check.");
            Debug.Log("Loading alignment serialized prefab saved and read back at revision " + ProductionLoadingView.CurrentRevision + ".");
        }

        static void RequireEditorIdle()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play Mode before building production prefabs.");
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException("Wait for compilation, import and Player builds to finish before repairing production prefabs.");
        }

        public static List<string> ValidateAll()
        {
            var failures = new List<string>();
            if (!File.Exists(LockPath))
                failures.Add("Missing FIX01 delivery lock: " + LockPath);

            foreach (ScreenId id in RequiredPages)
            {
                string path = PageRoot + "/" + id + ".prefab";
                if (!File.Exists(path))
                {
                    failures.Add("Serialized prefab file is not physically present: " + path);
                    continue;
                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    failures.Add("Unity could not import serialized prefab: " + path);
                    continue;
                }

                ProductionUiFix01PageBootstrap bootstrap = prefab.GetComponent<ProductionUiFix01PageBootstrap>();
                if (bootstrap == null)
                    failures.Add(path + ": missing ProductionUiFix01PageBootstrap.");
                else if (bootstrap.ScreenId != id)
                    failures.Add(path + ": FIX01 bootstrap ScreenId mismatch.");

                if (id == ScreenId.Loading)
                {
                    ProductionLoadingView view = prefab.GetComponent<ProductionLoadingView>();
                    if (view == null || !view.HasRequiredBindings)
                        failures.Add(path + ": Loading alignment revision/standalone artwork/live control bindings incomplete.");
                    if (prefab.transform.Find(ProductionLoadingRuntime.ContentName) == null)
                        failures.Add(path + ": serialized aligned Loading children are missing.");
                }

                ProductionCoreLoopPageMarker core = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (core == null || core.ScreenId != id || !core.ScreenshotFree)
                    failures.Add(path + ": core screenshot-free marker missing or invalid.");

                string[] dependencies = AssetDatabase.GetDependencies(path, true);
                foreach (string dependency in dependencies)
                {
                    string normalized = dependency.Replace('\\', '/');
                    if (normalized.Contains("/Resources/DesignScreens/") || normalized.Contains("/Art/ReferenceOnly/"))
                        failures.Add(path + ": forbidden captured-screen dependency: " + dependency);
                }
            }

            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/bg_home_clean.png", failures);
            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/hero_ivan_firebird.png", failures);
            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/bg_worldmap_clean.png", failures);
            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/bg_chaptermap_clean.png", failures);
            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/bg_prelevel_clean.png", failures);
            ValidateResource("Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01/bg_gameplay_clean.png", failures);

            return failures;
        }

        static void ValidateResource(string path, List<string> failures)
        {
            if (!File.Exists(path))
                failures.Add("Missing FIX01 standalone runtime asset: " + path);
        }

        static void BuildPage(ScreenId id)
        {
            RequireEditorIdle();
            GameObject root = new GameObject(id.ToString(), typeof(RectTransform));
            if (id == ScreenId.Loading) root.layer = 5;
            try
            {
                RectTransform rect = root.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                ProductionUiFix01PageBootstrap bootstrap = root.AddComponent<ProductionUiFix01PageBootstrap>();
                bootstrap.Configure(id);
                ProductionCoreLoopPageMarker core = root.AddComponent<ProductionCoreLoopPageMarker>();
                core.Configure(id);

                if (id == ScreenId.Loading)
                {
                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
                    rect.sizeDelta = new Vector2(ProductionUiFix01Runtime.ReferenceWidth, ProductionUiFix01Runtime.ReferenceHeight);
                    ProductionLoadingRuntime.BuildInto(rect, key => AssetDatabase.LoadAssetAtPath<Sprite>(
                        "Assets/LegendsOfRus/Resources/" + key + ".png"),
                        AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(
                            "Assets/LegendsOfRus/Resources/" + ProductionLoadingRuntime.TypographyResource + ".asset"));
                    if (!root.GetComponent<ProductionLoadingView>().HasRequiredBindings)
                        throw new InvalidOperationException("Loading alignment builder produced incomplete bindings.");
                }

                if (id == ScreenId.Home || id == ScreenId.WorldMap || id == ScreenId.ChapterMap)
                {
                    ProductionReferenceMatchPageMarker marker = root.AddComponent<ProductionReferenceMatchPageMarker>();
                    marker.Configure(id, ReferenceR2Sha(id), id == ScreenId.Home ? 6 : 4);
                }
                else if (id == ScreenId.PreLevel || id == ScreenId.Gameplay || id == ScreenId.Pause)
                {
                    ProductionReferenceMatchR3PageMarker marker = root.AddComponent<ProductionReferenceMatchR3PageMarker>();
                    if (id == ScreenId.PreLevel)
                        marker.Configure(id, "32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195", "fix01-independent-assets-prelevel", 12, false, false);
                    else if (id == ScreenId.Gameplay)
                        marker.Configure(id, "d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800", "fix01-live-board-preserved", 5, true, false);
                    else
                        marker.Configure(id, "31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716", "fix01-live-gameplay-modal", 7, false, true);
                }

                string path = PageRoot + "/" + id + ".prefab";
                bool success;
                PrefabUtility.SaveAsPrefabAsset(root, path, out success);
                if (!success) throw new IOException("Could not save production prefab: " + path);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        static string ReferenceR2Sha(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.Home: return "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8";
                case ScreenId.WorldMap: return "b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19";
                case ScreenId.ChapterMap: return "e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf";
                default: return string.Empty;
            }
        }

        static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
