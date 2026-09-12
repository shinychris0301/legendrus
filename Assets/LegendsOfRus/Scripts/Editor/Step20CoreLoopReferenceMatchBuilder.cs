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
    /// v1.20 Step 3.3R.2 reference-match correction for Home, World Map and
    /// Chapter Map.  The approved 941x1672 references are decomposed into
    /// page-specific production components; no prefab may depend on the
    /// ReferenceOnly or Resources/DesignScreens source images.
    /// </summary>
    public static class Step20CoreLoopReferenceMatchBuilder
    {
        public const string Version = "v1.20-step03.3R2";
        public const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        public const string ArtRoot = "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatch";
        public const int RequiredPageCount = 3;
        const float W = 941f;
        const float H = 1672f;

        const string HomeReferenceSha = "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8";
        const string WorldMapReferenceSha = "b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19";
        const string ChapterMapReferenceSha = "e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf";

        static readonly ScreenId[] RequiredPages =
        {
            ScreenId.Home,
            ScreenId.WorldMap,
            ScreenId.ChapterMap
        };

        [InitializeOnLoadMethod]
        static void QueueAutomaticBuild()
        {
            EditorApplication.delayCall += AutomaticBuild;
        }

        static void AutomaticBuild()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            if (Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled())
                return;
            try
            {
                if (NeedsBuild())
                    BuildAll(false);
            }
            catch (Exception e)
            {
                Debug.LogError("v1.20 Step 3.3R.2 automatic reference-match rebuild failed: " + e.Message);
            }
        }

        [MenuItem("Legends of Rus/Production UI/Build v1.20 Step 3.3R.2 Home-Map Reference Match")]
        public static void BuildMenu()
        {
            BuildAll(true);
        }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 3.3R.2 Home-Map Reference Match")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R.2 reference-match validation passed: 3/3 pages.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Reference-match pages validate: 3/3.", "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R.2 reference-match validation failed.\n" + report);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", report, "OK");
        }

        public static void BuildAll(bool showDialog)
        {
            EnsureFolder(PageRoot);
            ConfigureProductionSprites();
            BuildHome();
            BuildWorldMap();
            BuildChapterMap();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join("\n", failures.ToArray()));

            if (Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled())
                Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false);

            Debug.Log("v1.20 Step 3.3R.2 rebuilt Home, World Map and Chapter Map from 14 reference-match production components.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Built 3 reference-match production pages.", "OK");
        }

        public static List<string> ValidateAll()
        {
            var failures = new List<string>();
            foreach (ScreenId id in RequiredPages)
            {
                string path = PageRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    failures.Add("Missing reference-match prefab: " + path);
                    continue;
                }

                ProductionCoreLoopPageMarker core = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (core == null || core.ScreenId != id || !core.ScreenshotFree)
                    failures.Add(path + ": core screenshot-free marker is missing or invalid.");

                ProductionReferenceMatchPageMarker marker = prefab.GetComponent<ProductionReferenceMatchPageMarker>();
                if (marker == null)
                {
                    failures.Add(path + ": missing ProductionReferenceMatchPageMarker.");
                    continue;
                }

                if (marker.ScreenId != id)
                    failures.Add(path + ": reference-match ScreenId mismatch.");
                if (marker.ReferenceMatchVersion != Version)
                    failures.Add(path + ": stale reference-match version.");
                if (marker.FullScreenScreenshotDependency)
                    failures.Add(path + ": full-screen screenshot dependency flag must remain false.");
                if (!marker.ReferenceMatchStaticComposition)
                    failures.Add(path + ": static reference composition is not pinned.");
                if (marker.ProductionComponentCount != ExpectedComponentCount(id))
                    failures.Add(path + ": production component count mismatch.");
                if (marker.ReferenceSha256 != ReferenceSha(id))
                    failures.Add(path + ": approved reference hash mismatch.");

                string[] dependencies = AssetDatabase.GetDependencies(path, true);
                int referenceMatchDependencies = 0;
                foreach (string raw in dependencies)
                {
                    string dependency = raw.Replace('\\', '/');
                    if (dependency.Contains("/Resources/DesignScreens/"))
                        failures.Add(path + ": forbidden DesignScreens dependency: " + dependency);
                    if (dependency.Contains("/Art/ReferenceOnly/"))
                        failures.Add(path + ": forbidden ReferenceOnly dependency: " + dependency);
                    if (dependency.StartsWith(ArtRoot + "/", StringComparison.Ordinal))
                        referenceMatchDependencies++;
                }

                ProductionUiFix01PageBootstrap fix01 = prefab.GetComponent<ProductionUiFix01PageBootstrap>();
                if (fix01 == null && referenceMatchDependencies < ExpectedComponentCount(id))
                    failures.Add(path + ": missing reference-match production component dependencies.");
            }

            return failures;
        }

        public static bool IsReferenceMatchPage(ScreenId id)
        {
            return id == ScreenId.Home || id == ScreenId.WorldMap || id == ScreenId.ChapterMap;
        }

        static bool NeedsBuild()
        {
            foreach (ScreenId id in RequiredPages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PageRoot + "/" + id + ".prefab");
                if (prefab == null)
                    return true;
                ProductionReferenceMatchPageMarker marker = prefab.GetComponent<ProductionReferenceMatchPageMarker>();
                if (marker == null || marker.ReferenceMatchVersion != Version || marker.ReferenceSha256 != ReferenceSha(id))
                    return true;
            }
            return false;
        }

        static void BuildHome()
        {
            GameObject root = CreatePage(ScreenId.Home, HomeReferenceSha, 6);
            AddComponent(root.transform, "ReferenceTopHud", "home_top_hud.png", 0, 0, 941, 150);
            AddComponent(root.transform, "ReferenceLogoScene", "home_logo_scene.png", 0, 150, 941, 350);
            AddComponent(root.transform, "ReferenceHeroScene", "home_hero_scene.png", 0, 500, 941, 500);
            AddComponent(root.transform, "ReferencePlayComponent", "home_play_component.png", 0, 1000, 941, 210);
            AddComponent(root.transform, "ReferenceNavigationComponent", "home_navigation_component.png", 0, 1210, 941, 240);
            AddComponent(root.transform, "ReferenceQuestComponent", "home_quest_component.png", 0, 1450, 941, 222);
            Save(root, ScreenId.Home);
        }

        static void BuildWorldMap()
        {
            GameObject root = CreatePage(ScreenId.WorldMap, WorldMapReferenceSha, 4);
            AddComponent(root.transform, "ReferenceTopHud", "worldmap_top_hud.png", 0, 0, 941, 135);
            AddComponent(root.transform, "ReferenceTitle", "worldmap_title_component.png", 0, 135, 941, 150);
            AddComponent(root.transform, "ReferencePaintedMap", "worldmap_painted_map_plate.png", 0, 285, 941, 1055);
            AddComponent(root.transform, "ReferenceBottomNavigation", "worldmap_bottom_navigation.png", 0, 1340, 941, 332);
            Save(root, ScreenId.WorldMap);
        }

        static void BuildChapterMap()
        {
            GameObject root = CreatePage(ScreenId.ChapterMap, ChapterMapReferenceSha, 4);
            AddComponent(root.transform, "ReferenceTopHud", "chaptermap_top_hud.png", 0, 0, 941, 140);
            AddComponent(root.transform, "ReferenceChapterRibbon", "chaptermap_ribbon_component.png", 0, 140, 941, 250);
            AddComponent(root.transform, "ReferencePaintedLevelPath", "chaptermap_painted_level_path.png", 0, 390, 941, 1060);
            AddComponent(root.transform, "ReferenceBottomControls", "chaptermap_bottom_controls.png", 0, 1450, 941, 222);
            Save(root, ScreenId.ChapterMap);
        }

        static GameObject CreatePage(ScreenId id, string referenceSha, int componentCount)
        {
            GameObject root = new GameObject(
                id.ToString(),
                typeof(RectTransform),
                typeof(ProductionCoreLoopPageMarker),
                typeof(ProductionReferenceMatchPageMarker)
            );
            RectTransform rt = root.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(W, H);
            root.GetComponent<ProductionCoreLoopPageMarker>().Configure(id);
            root.GetComponent<ProductionReferenceMatchPageMarker>().Configure(id, referenceSha, componentCount);
            return root;
        }

        static void AddComponent(Transform parent, string name, string file, float x, float y, float width, float height)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            SetPixelRect(go.GetComponent<RectTransform>(), x, y, width, height);
            Image image = go.GetComponent<Image>();
            image.sprite = LoadProductionSprite(file);
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.color = Color.white;
        }

        static Sprite LoadProductionSprite(string file)
        {
            string path = ArtRoot + "/" + file;
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                throw new InvalidOperationException("Reference-match production sprite missing or not imported: " + path);
            return sprite;
        }

        static void ConfigureProductionSprites()
        {
            if (!Directory.Exists(ArtRoot))
                throw new DirectoryNotFoundException("Reference-match art folder missing: " + ArtRoot);

            foreach (string file in Directory.GetFiles(ArtRoot, "*.png", SearchOption.TopDirectoryOnly))
            {
                string path = file.Replace('\\', '/');
                int assetsIndex = path.IndexOf("Assets/", StringComparison.Ordinal);
                if (assetsIndex >= 0)
                    path = path.Substring(assetsIndex);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    continue;

                bool changed = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    changed = true;
                }
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                if (Math.Abs(importer.spritePixelsPerUnit - 100f) > 0.01f)
                {
                    importer.spritePixelsPerUnit = 100f;
                    changed = true;
                }
                if (importer.alphaIsTransparency == false)
                {
                    importer.alphaIsTransparency = true;
                    changed = true;
                }
                if (importer.textureCompression != TextureImporterCompression.CompressedHQ)
                {
                    importer.textureCompression = TextureImporterCompression.CompressedHQ;
                    changed = true;
                }

                if (changed)
                    importer.SaveAndReimport();
            }
        }

        static int ExpectedComponentCount(ScreenId id)
        {
            return id == ScreenId.Home ? 6 : 4;
        }

        static string ReferenceSha(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.Home: return HomeReferenceSha;
                case ScreenId.WorldMap: return WorldMapReferenceSha;
                case ScreenId.ChapterMap: return ChapterMapReferenceSha;
                default: return string.Empty;
            }
        }

        static void SetPixelRect(RectTransform rt, float x, float yTop, float width, float height)
        {
            rt.anchorMin = new Vector2(x / W, 1f - (yTop + height) / H);
            rt.anchorMax = new Vector2((x + width) / W, 1f - yTop / H);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string name = Path.GetFileName(path);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        static void Save(GameObject root, ScreenId id)
        {
            PrefabUtility.SaveAsPrefabAsset(root, PageRoot + "/" + id + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
#endif
