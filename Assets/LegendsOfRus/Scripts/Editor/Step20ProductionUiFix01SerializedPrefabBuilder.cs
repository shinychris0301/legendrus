#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        // Original Home reference is composition provenance only; no screenshot is a runtime asset.
        public const string HomeAlignmentReferenceSha256 = "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8";

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
                // Repair only a page that actually failed validation. A page alignment
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
            if (id == ScreenId.Gameplay)
            {
                var gameplayView=prefab.GetComponent<ProductionGameplayView>();
                var layout=prefab.GetComponent<ProductionGameplayLayout>();
                return gameplayView==null||!gameplayView.HasRequiredBindings||layout==null||layout.View!=gameplayView||prefab.transform.Find(ProductionGameplayRuntime.ContentName)==null;
            }
            if (id == ScreenId.PreLevel)
            {
                ProductionPreLevelView map = prefab.GetComponent<ProductionPreLevelView>();
                ProductionPreLevelLayout layout = prefab.GetComponent<ProductionPreLevelLayout>();
                return map == null || !map.HasRequiredBindings || layout == null || layout.View != map ||
                    prefab.transform.Find(ProductionPreLevelRuntime.ContentName) == null;
            }
            if (id == ScreenId.ChapterMap)
            {
                ProductionChapterMapView map = prefab.GetComponent<ProductionChapterMapView>();
                ProductionChapterMapLayout layout = prefab.GetComponent<ProductionChapterMapLayout>();
                return map == null || !map.HasRequiredBindings || layout == null || layout.View != map ||
                    prefab.transform.Find(ProductionChapterMapRuntime.ContentName) == null;
            }
            if (id == ScreenId.WorldMap)
            {
                ProductionWorldMapView map = prefab.GetComponent<ProductionWorldMapView>();
                ProductionWorldMapLayout layout = prefab.GetComponent<ProductionWorldMapLayout>();
                return map == null || !map.HasRequiredBindings || layout == null || layout.View != map ||
                    prefab.transform.Find(ProductionWorldMapRuntime.ContentName) == null;
            }
            if (id == ScreenId.Home)
            {
                ProductionHomeView home = prefab.GetComponent<ProductionHomeView>();
                ProductionHomeLayout layout = prefab.GetComponent<ProductionHomeLayout>();
                return home == null || !home.HasRequiredBindings ||
                    layout == null || !layout.HasRequiredBindings ||
                    prefab.transform.Find(ProductionHomeRuntime.ContentName) == null;
            }
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

        [MenuItem("Legends of Rus/Production UI/Home Alignment/Build or Repair Home Only")]
        public static void RebuildHomeAlignment()
        {
            RequireEditorIdle();
            EnsureFolder(PageRoot);
            BuildPage(ScreenId.Home);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot + "/Home.prefab", ImportAssetOptions.ForceSynchronousImport);
            if (PageNeedsBuild(ScreenId.Home))
                throw new InvalidOperationException("Saved Home alignment prefab failed the revision/binding read-back check.");
            Debug.Log("Home alignment serialized prefab saved and read back at revision " + ProductionHomeView.CurrentRevision + ".");
        }

        static void RequireEditorIdle()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play Mode before building production prefabs.");
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException("Wait for compilation, import and Player builds to finish before repairing production prefabs.");
        }

        [MenuItem("Legends of Rus/Production UI/World Map Alignment/Build or Repair World Map Only")]
        public static void RebuildWorldMapAlignment()
        {
            RequireEditorIdle();
            BuildPage(ScreenId.WorldMap);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot + "/WorldMap.prefab",ImportAssetOptions.ForceSynchronousImport);
            if(PageNeedsBuild(ScreenId.WorldMap)) throw new InvalidOperationException("World Map save/readback failed.");
            Debug.Log("World Map alignment serialized prefab saved and read back at revision " + ProductionWorldMapView.CurrentRevision + ".");
        }

        [MenuItem("Legends of Rus/Production UI/Rebuild Chapter Map Alignment")]
        public static void RebuildChapterMapAlignment()
        {
            RequireEditorIdle();
            BuildPage(ScreenId.ChapterMap);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot + "/ChapterMap.prefab",ImportAssetOptions.ForceSynchronousImport);
            if(PageNeedsBuild(ScreenId.ChapterMap)) throw new InvalidOperationException("Chapter Map save/readback failed.");
            Debug.Log("Chapter Map alignment serialized prefab saved and read back at revision " + ProductionChapterMapView.CurrentRevision + ".");
        }

        [MenuItem("Legends of Rus/Production UI/Rebuild Pre-Level Alignment")]
        public static void RebuildPreLevelAlignment()
        {
            RequireEditorIdle();
            BuildPage(ScreenId.PreLevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot + "/PreLevel.prefab",ImportAssetOptions.ForceSynchronousImport);
            if(PageNeedsBuild(ScreenId.PreLevel)) throw new InvalidOperationException("Pre-Level save/readback failed.");
            Debug.Log("Pre-Level alignment serialized prefab saved and read back at revision " + ProductionPreLevelView.CurrentRevision + ".");
        }

        [MenuItem("Legends of Rus/Production UI/Rebuild Gameplay Alignment")]
        public static void RebuildGameplayAlignment()
        {
            RequireEditorIdle();BuildPage(ScreenId.Gameplay);AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(PageRoot+"/Gameplay.prefab",ImportAssetOptions.ForceSynchronousImport);
            if(PageNeedsBuild(ScreenId.Gameplay))throw new InvalidOperationException("Gameplay save/readback failed.");
            Debug.Log("Gameplay alignment serialized prefab saved and read back at revision "+ProductionGameplayView.CurrentRevision+".");
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

                if (id == ScreenId.Gameplay)
                {
                    var view=prefab.GetComponent<ProductionGameplayView>();
                    if(view==null||!view.HasRequiredBindings)failures.Add(path+": Gameplay native bindings incomplete.");
                    continue;
                }
                if (id == ScreenId.PreLevel)
                {
                    ProductionPreLevelView view = prefab.GetComponent<ProductionPreLevelView>();
                    if(view == null || !view.HasRequiredBindings || prefab.transform.Find(ProductionPreLevelRuntime.ContentName) == null)
                        failures.Add(path + ": Pre-Level alignment revision/whole-sprite/live-control bindings incomplete.");
                }
                if (id == ScreenId.ChapterMap)
                {
                    ProductionChapterMapView view = prefab.GetComponent<ProductionChapterMapView>();
                    if(view == null || !view.HasRequiredBindings || prefab.transform.Find(ProductionChapterMapRuntime.ContentName) == null)
                        failures.Add(path + ": Chapter Map alignment revision/whole-sprite/live-control bindings incomplete.");
                }
                if (id == ScreenId.WorldMap)
                {
                    ProductionWorldMapView view = prefab.GetComponent<ProductionWorldMapView>();
                    if(view == null || !view.HasRequiredBindings || prefab.transform.Find(ProductionWorldMapRuntime.ContentName) == null)
                        failures.Add(path + ": World Map alignment revision/whole-sprite/live-control bindings incomplete.");
                }
                else if (id == ScreenId.Loading)
                {
                    ProductionLoadingView view = prefab.GetComponent<ProductionLoadingView>();
                    if (view == null || !view.HasRequiredBindings)
                        failures.Add(path + ": Loading alignment revision/standalone artwork/live control bindings incomplete.");
                    if (prefab.transform.Find(ProductionLoadingRuntime.ContentName) == null)
                        failures.Add(path + ": serialized aligned Loading children are missing.");
                }
                else if (id == ScreenId.Home)
                {
                    ProductionHomeView view = prefab.GetComponent<ProductionHomeView>();
                    if (view == null || !view.HasRequiredBindings)
                        failures.Add(path + ": Home alignment revision/standalone artwork/live control bindings incomplete.");
                    if (prefab.transform.Find(ProductionHomeRuntime.ContentName) == null)
                        failures.Add(path + ": serialized aligned Home children are missing.");
                    ProductionHomeLayout layout = prefab.GetComponent<ProductionHomeLayout>();
                    if (layout == null || !layout.HasRequiredBindings)
                        failures.Add(path + ": Home full-display/safe-area layout bindings incomplete.");
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
            foreach (string key in ProductionHomeRuntime.GetTextureResourceKeys())
                ValidateResource("Assets/LegendsOfRus/Resources/" + key + ".png", failures);
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
            if (id == ScreenId.Gameplay) { BuildGameplayPage(); return; }
            if (id == ScreenId.PreLevel) { BuildPreLevelPage(); return; }
            if (id == ScreenId.ChapterMap) { BuildChapterMapPage(); return; }
            if (id == ScreenId.WorldMap) { BuildWorldMapPage(); return; }
            if (id == ScreenId.Loading && File.Exists(PageRoot + "/Loading.prefab"))
            {
                string loadingPath = PageRoot + "/Loading.prefab";
                GameObject contents = PrefabUtility.LoadPrefabContents(loadingPath);
                try
                {
                    ProductionLoadingRuntime.UpgradeHomeStyle(contents.GetComponent<ProductionLoadingView>(),
                        key => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/LegendsOfRus/Resources/" + key + ".png"));
                    bool saved;
                    PrefabUtility.SaveAsPrefabAsset(contents, loadingPath, out saved);
                    if (!saved) throw new IOException("Could not save Loading Home-style alignment.");
                }
                finally { PrefabUtility.UnloadPrefabContents(contents); }
                return;
            }
            if (id == ScreenId.Home)
            {
                BuildHomePage();
                return;
            }
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

                if (id == ScreenId.WorldMap || id == ScreenId.ChapterMap)
                {
                    ProductionReferenceMatchPageMarker marker = root.AddComponent<ProductionReferenceMatchPageMarker>();
                    marker.Configure(id, ReferenceR2Sha(id), 4);
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

        static void BuildWorldMapPage()
        {
            foreach(string name in ProductionWorldMapRuntime.ArtNames)
            {
                string artPath="Assets/LegendsOfRus/Resources/"+ProductionWorldMapRuntime.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(artPath) as TextureImporter;
                if(importer==null) throw new InvalidOperationException("Missing World Map art: "+artPath);
                LockedDesignImporter.ConfigureWorldMapTexture(importer, name);
                importer.SaveAndReimport();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            string path=PageRoot+"/WorldMap.prefab";
            GameObject root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                root.layer=5;
                var rect=(RectTransform)root.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
                rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(941,1672);
                ProductionWorldMapRuntime.BuildInto(rect);
                root.GetComponent<ProductionReferenceMatchPageMarker>().Configure(ScreenId.WorldMap,ReferenceR2Sha(ScreenId.WorldMap),5);
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);
                if(!saved)throw new IOException("Could not save aligned World Map.");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        static void BuildChapterMapPage()
        {
            foreach(string name in ProductionChapterMapRuntime.ArtNames)
            {
                string artPath="Assets/LegendsOfRus/Resources/"+ProductionChapterMapRuntime.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(artPath) as TextureImporter;
                if(importer==null) throw new InvalidOperationException("Missing Chapter Map art: "+artPath);
                LockedDesignImporter.ConfigureWorldMapTexture(importer, name);
                importer.SaveAndReimport();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            string path=PageRoot+"/ChapterMap.prefab";
            GameObject root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                root.layer=5;
                var rect=(RectTransform)root.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
                rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(941,1672);
                ProductionChapterMapRuntime.BuildInto(rect);
                root.GetComponent<ProductionReferenceMatchPageMarker>().Configure(ScreenId.ChapterMap,ReferenceR2Sha(ScreenId.ChapterMap),5);
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);
                if(!saved)throw new IOException("Could not save aligned Chapter Map.");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        static void BuildGameplayPage()
        {
            foreach(string name in ProductionGameplayArt.Files)
            {
                string artPath="Assets/LegendsOfRus/Resources/"+ProductionGameplayArt.Root+name+".png";
                var importer=AssetImporter.GetAtPath(artPath) as TextureImporter;
                if(importer==null)throw new InvalidOperationException("Missing Gameplay art: "+artPath);
                GameplayAlignmentArtImporter.Configure(importer,name);importer.SaveAndReimport();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            string path=PageRoot+"/Gameplay.prefab";var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                root.layer=5;var rect=(RectTransform)root.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
                rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(941,1672);
                ProductionGameplayRuntime.BuildInto(rect);
                root.GetComponent<ProductionReferenceMatchR3PageMarker>().Configure(ScreenId.Gameplay,
                    "d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800",
                    "gameplay-frame-polish-r2-native-ornament-live-board-and-hud",7,true,false);
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);if(!saved)throw new IOException("Could not save Gameplay.");
            }
            finally{PrefabUtility.UnloadPrefabContents(root);}
        }

        static void BuildPreLevelPage()
        {
            foreach(string name in ProductionPreLevelRuntime.ArtNames)
            {
                string artPath="Assets/LegendsOfRus/Resources/"+ProductionPreLevelRuntime.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(artPath) as TextureImporter;
                if(importer==null) throw new InvalidOperationException("Missing Pre-Level art: "+artPath);
                LockedDesignImporter.ConfigureWorldMapTexture(importer, name);
                importer.SaveAndReimport();
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            string path=PageRoot+"/PreLevel.prefab";
            GameObject root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                root.layer=5;
                var rect=(RectTransform)root.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
                rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(941,1672);
                ProductionPreLevelRuntime.BuildInto(rect);
                root.GetComponent<ProductionReferenceMatchR3PageMarker>().Configure(ScreenId.PreLevel,
                    "32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195",
                    "pre-level-alignment-r1-whole-assets-live-data",8,false,false);
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);
                if(!saved)throw new IOException("Could not save aligned Pre-Level.");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        static void BuildHomePage()
        {
            ImportHomeAlignmentArt();
            string path = PageRoot + "/Home.prefab";
            bool loadedExisting = File.Exists(path);
            GameObject root = loadedExisting ? PrefabUtility.LoadPrefabContents(path) :
                new GameObject("Home", typeof(RectTransform));
            try
            {
                root.layer = 5;
                RectTransform rect = root.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(ProductionUiFix01Runtime.ReferenceWidth, ProductionUiFix01Runtime.ReferenceHeight);

                ProductionUiFix01PageBootstrap bootstrap = root.GetComponent<ProductionUiFix01PageBootstrap>();
                if (bootstrap == null) bootstrap = root.AddComponent<ProductionUiFix01PageBootstrap>();
                bootstrap.Configure(ScreenId.Home);
                ProductionCoreLoopPageMarker core = root.GetComponent<ProductionCoreLoopPageMarker>();
                if (core == null) core = root.AddComponent<ProductionCoreLoopPageMarker>();
                core.Configure(ScreenId.Home);
                ProductionReferenceMatchPageMarker marker = root.GetComponent<ProductionReferenceMatchPageMarker>();
                if (marker == null) marker = root.AddComponent<ProductionReferenceMatchPageMarker>();
                marker.Configure(ScreenId.Home, HomeAlignmentReferenceSha256, ProductionHomeRuntime.GetTextureResourceKeys().Count);

                // Upgrade foreground art and geometry without replacing existing object identities.
                ProductionHomeView view = root.GetComponent<ProductionHomeView>();
                ProductionHomeLayout layout = root.GetComponent<ProductionHomeLayout>();
                Transform content = root.transform.Find(ProductionHomeRuntime.ContentName);
                if (content != null && view != null && view.Revision >= 2 &&
                    view.Revision <= ProductionHomeView.CurrentRevision && layout != null && layout.HasBaseBindings)
                    ProductionHomeRuntime.UpgradeReferenceForeground(view, key => AssetDatabase.LoadAssetAtPath<Sprite>(
                        "Assets/LegendsOfRus/Resources/" + key + ".png"));
                if (content != null && (view == null || !view.HasRequiredBindings ||
                    layout == null || !layout.HasRequiredBindings))
                    UnityEngine.Object.DestroyImmediate(content.gameObject);
                Transform legacyContent = root.transform.Find("FIX01_CONTENT");
                if (legacyContent != null) UnityEngine.Object.DestroyImmediate(legacyContent.gameObject);
                ProductionHomeRuntime.BuildInto(rect, key => AssetDatabase.LoadAssetAtPath<Sprite>(
                    "Assets/LegendsOfRus/Resources/" + key + ".png"),
                    AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(
                        "Assets/LegendsOfRus/Resources/" + ProductionHomeRuntime.TypographyResource + ".asset"));
                view = root.GetComponent<ProductionHomeView>();
                if (view != null && view.PlayText != null)
                {
                    // Preserve the prior Shadow component identity while matching Splash's
                    // required order: optical glyph alignment must run before its emboss shadow.
                    SplashTitleTextGeometry geometry = view.PlayText.GetComponent<SplashTitleTextGeometry>();
                    Shadow shadow = view.PlayText.GetComponent<Shadow>();
                    if (geometry != null && shadow != null)
                    {
                        Component[] components = view.PlayText.GetComponents<Component>();
                        while (Array.IndexOf(components, geometry) > Array.IndexOf(components, shadow))
                        {
                            if (!UnityEditorInternal.ComponentUtility.MoveComponentUp(geometry))
                                throw new InvalidOperationException("Could not preserve Splash Start text effect order in Home.");
                            components = view.PlayText.GetComponents<Component>();
                        }
                    }
                }
                if (view == null || !view.HasRequiredBindings)
                    throw new InvalidOperationException("Home alignment builder produced incomplete bindings.");

                bool success;
                PrefabUtility.SaveAsPrefabAsset(root, path, out success);
                if (!success) throw new IOException("Could not save production prefab: " + path);
            }
            finally
            {
                if (loadedExisting) PrefabUtility.UnloadPrefabContents(root);
                else UnityEngine.Object.DestroyImmediate(root);
            }
        }

        static void ImportHomeAlignmentArt()
        {
            // Only Home's individual layers are owned here. Shared Splash art, typography
            // and legacy currency glyph importers remain untouched.
            foreach (string key in ProductionHomeRuntime.GetTextureResourceKeys())
            {
                if (!key.StartsWith(ProductionHomeRuntime.ArtRoot, StringComparison.Ordinal)) continue;
                string path = "Assets/LegendsOfRus/Resources/" + key + ".png";
                string name = Path.GetFileNameWithoutExtension(path);
                if (!File.Exists(path)) throw new FileNotFoundException("Missing reviewed Home layer.", path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) throw new InvalidOperationException("Home layer has no texture importer: " + path);
                LockedDesignImporter.ConfigureHomeAlignmentTexture(importer, name);
                importer.SaveAndReimport();
            }
        }

        static string ReferenceR2Sha(ScreenId id)
        {
            switch (id)
            {
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
