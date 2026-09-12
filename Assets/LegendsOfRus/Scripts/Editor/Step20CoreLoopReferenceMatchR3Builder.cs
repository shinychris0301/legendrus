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
    /// v1.20 Step 3.3R.3 visual-fidelity correction for Pre-Level, Gameplay,
    /// and Pause. Pre-Level is decomposed across its complete approved
    /// reference. Gameplay reconstructs only the fixed shell outside the live
    /// Match-3 board rectangle. Pause reconstructs only the central modal and
    /// uses a runtime dim layer over the real gameplay page.
    /// </summary>
    public static class Step20CoreLoopReferenceMatchR3Builder
    {
        public const string Version = "v1.20-step03.3R3";
        public const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        public const string ArtRoot = "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatchR3";
        public const int RequiredPageCount = 3;
        const float W = 941f;
        const float H = 1672f;

        const string PreLevelReferenceSha = "32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195";
        const string GameplayReferenceSha = "d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800";
        const string PauseReferenceSha = "31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716";

        static readonly ScreenId[] RequiredPages =
        {
            ScreenId.PreLevel,
            ScreenId.Gameplay,
            ScreenId.Pause
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
                Debug.LogError("v1.20 Step 3.3R.3 automatic reference-match rebuild failed: " + e.Message);
            }
        }

        [MenuItem("Legends of Rus/Production UI/Build v1.20 Step 3.3R.3 Pre-Level Gameplay Pause Reference Match")]
        public static void BuildMenu()
        {
            BuildAll(true);
        }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 3.3R.3 Pre-Level Gameplay Pause Reference Match")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3R.3 reference-match validation passed: 3/3 pages.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "R3 reference-match pages validate: 3/3.", "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3R.3 reference-match validation failed.\n" + report);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", report, "OK");
        }

        public static void BuildAll(bool showDialog)
        {
            EnsureFolder(PageRoot);
            ConfigureProductionSprites();
            BuildPreLevel();
            BuildGameplay();
            BuildPause();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join("\n", failures.ToArray()));

            if (Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled())
                Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false);

            Debug.Log("v1.20 Step 3.3R.3 rebuilt Pre-Level, Gameplay shell, and Pause modal from 24 production components while retaining live gameplay.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Built 3 R3 reference-match production pages.", "OK");
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
                    failures.Add("Missing R3 prefab: " + path);
                    continue;
                }

                ProductionCoreLoopPageMarker core = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (core == null || core.ScreenId != id || !core.ScreenshotFree)
                    failures.Add(path + ": core screenshot-free marker is missing or invalid.");

                ProductionReferenceMatchR3PageMarker marker = prefab.GetComponent<ProductionReferenceMatchR3PageMarker>();
                if (marker == null)
                {
                    failures.Add(path + ": missing ProductionReferenceMatchR3PageMarker.");
                    continue;
                }

                if (marker.ScreenId != id)
                    failures.Add(path + ": ScreenId mismatch.");
                if (marker.ReferenceMatchVersion != Version)
                    failures.Add(path + ": stale R3 version.");
                if (marker.FullScreenScreenshotDependency)
                    failures.Add(path + ": full-screen screenshot dependency flag must remain false.");
                if (marker.ReferenceSha256 != ReferenceSha(id))
                    failures.Add(path + ": reference hash mismatch.");
                if (marker.ProductionComponentCount != ExpectedComponentCount(id))
                    failures.Add(path + ": component count mismatch.");
                if (id == ScreenId.Gameplay && !marker.PreservesLiveGameplayBoard)
                    failures.Add(path + ": Gameplay must preserve the live Match-3 board.");
                if (id != ScreenId.Gameplay && marker.PreservesLiveGameplayBoard)
                    failures.Add(path + ": unexpected live-board marker.");
                if (id == ScreenId.Pause && !marker.OverlaysLiveGameplayForPause)
                    failures.Add(path + ": Pause must overlay the live Gameplay page.");
                if (id != ScreenId.Pause && marker.OverlaysLiveGameplayForPause)
                    failures.Add(path + ": unexpected live-gameplay modal marker.");

                int productionDependencies = 0;
                foreach (string raw in AssetDatabase.GetDependencies(path, true))
                {
                    string dependency = raw.Replace('\\', '/');
                    if (dependency.Contains("/Resources/DesignScreens/"))
                        failures.Add(path + ": forbidden DesignScreens dependency: " + dependency);
                    if (dependency.Contains("/Art/ReferenceOnly/"))
                        failures.Add(path + ": forbidden ReferenceOnly dependency: " + dependency);
                    if (dependency.StartsWith(ArtRoot + "/", StringComparison.Ordinal))
                        productionDependencies++;
                }
                ProductionUiFix01PageBootstrap fix01 = prefab.GetComponent<ProductionUiFix01PageBootstrap>();
                if (fix01 == null && productionDependencies < ExpectedComponentCount(id))
                    failures.Add(path + ": missing R3 production component dependencies.");
            }
            return failures;
        }

        public static bool IsR3Page(ScreenId id)
        {
            return id == ScreenId.PreLevel || id == ScreenId.Gameplay || id == ScreenId.Pause;
        }

        static bool NeedsBuild()
        {
            foreach (ScreenId id in RequiredPages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PageRoot + "/" + id + ".prefab");
                if (prefab == null)
                    return true;
                ProductionReferenceMatchR3PageMarker marker = prefab.GetComponent<ProductionReferenceMatchR3PageMarker>();
                if (marker == null || marker.ReferenceMatchVersion != Version || marker.ReferenceSha256 != ReferenceSha(id))
                    return true;
            }
            return false;
        }

        static void BuildPreLevel()
        {
            GameObject root = CreatePage(ScreenId.PreLevel, PreLevelReferenceSha, "full-reference-component-reconstruction", 12, false, false);
            AddComponent(root.transform, "ReferenceTopHud", "prelevel_top_hud.png", 0, 0, 941, 145);
            AddComponent(root.transform, "ReferenceChapterBand", "prelevel_chapter_band.png", 0, 145, 941, 260);
            AddComponent(root.transform, "GoalsLeftScene", "prelevel_goals_left_scene.png", 0, 405, 190, 445);
            AddComponent(root.transform, "GoalsPanel", "prelevel_goals_panel.png", 190, 405, 561, 445);
            AddComponent(root.transform, "GoalsRightScene", "prelevel_goals_right_scene.png", 751, 405, 190, 445);
            AddComponent(root.transform, "BoostersLeftScene", "prelevel_boosters_left_scene.png", 0, 850, 105, 285);
            AddComponent(root.transform, "BoostersPanel", "prelevel_boosters_panel.png", 105, 850, 731, 285);
            AddComponent(root.transform, "BoostersRightScene", "prelevel_boosters_right_scene.png", 836, 850, 105, 285);
            AddComponent(root.transform, "RewardLeftScene", "prelevel_reward_left_scene.png", 0, 1135, 95, 300);
            AddComponent(root.transform, "RewardPanel", "prelevel_reward_panel.png", 95, 1135, 751, 300);
            AddComponent(root.transform, "RewardRightScene", "prelevel_reward_right_scene.png", 846, 1135, 95, 300);
            AddComponent(root.transform, "BottomControls", "prelevel_bottom_controls.png", 0, 1435, 941, 237);
            Save(root, ScreenId.PreLevel);
        }

        static void BuildGameplay()
        {
            GameObject root = CreatePage(ScreenId.Gameplay, GameplayReferenceSha, "fixed-shell-excluding-live-board-x79-y357-w786-h942", 5, true, false);
            AddComponent(root.transform, "ReferenceTopShell", "gameplay_top_shell.png", 0, 0, 941, 357);
            AddComponent(root.transform, "LeftEnvironment", "gameplay_left_environment.png", 0, 357, 79, 942);
            AddComponent(root.transform, "RightEnvironment", "gameplay_right_environment.png", 865, 357, 76, 942);
            AddComponent(root.transform, "BoosterShell", "gameplay_booster_shell.png", 0, 1299, 941, 205);
            AddComponent(root.transform, "ScoreFooter", "gameplay_score_footer.png", 0, 1504, 941, 168);
            Save(root, ScreenId.Gameplay);
        }

        static void BuildPause()
        {
            GameObject root = CreatePage(ScreenId.Pause, PauseReferenceSha, "central-modal-over-live-gameplay", 7, false, true);

            GameObject dim = new GameObject("LiveGameplayDim", typeof(RectTransform), typeof(Image));
            dim.transform.SetParent(root.transform, false);
            Stretch(dim.GetComponent<RectTransform>());
            Image dimImage = dim.GetComponent<Image>();
            dimImage.color = new Color(0f, 0f, 0f, 0.58f);
            dimImage.raycastTarget = false;

            AddComponent(root.transform, "PauseHeader", "pause_header.png", 150, 295, 640, 150);
            AddComponent(root.transform, "PauseLeftBorder", "pause_left_border.png", 150, 445, 30, 925);
            AddComponent(root.transform, "PauseGoalsPanel", "pause_goals_panel.png", 180, 445, 580, 270);
            AddComponent(root.transform, "PauseBoostersPanel", "pause_boosters_panel.png", 180, 715, 580, 200);
            AddComponent(root.transform, "PauseButtonsPanel", "pause_buttons_panel.png", 180, 915, 580, 455);
            AddComponent(root.transform, "PauseRightBorder", "pause_right_border.png", 760, 445, 30, 925);
            AddComponent(root.transform, "PauseFooter", "pause_footer.png", 150, 1370, 640, 50);
            Save(root, ScreenId.Pause);
        }

        static GameObject CreatePage(ScreenId id, string referenceSha, string scope, int componentCount, bool preserveLiveBoard, bool liveGameplayBehindPause)
        {
            GameObject root = new GameObject(
                id.ToString(),
                typeof(RectTransform),
                typeof(ProductionCoreLoopPageMarker),
                typeof(ProductionReferenceMatchR3PageMarker)
            );
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            root.GetComponent<ProductionCoreLoopPageMarker>().Configure(id);
            root.GetComponent<ProductionReferenceMatchR3PageMarker>().Configure(
                id, referenceSha, scope, componentCount, preserveLiveBoard, liveGameplayBehindPause);
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
                throw new InvalidOperationException("R3 production sprite missing or not imported: " + path);
            return sprite;
        }

        static void ConfigureProductionSprites()
        {
            if (!Directory.Exists(ArtRoot))
                throw new DirectoryNotFoundException("R3 art folder missing: " + ArtRoot);

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
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; changed = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; changed = true; }
                if (Math.Abs(importer.spritePixelsPerUnit - 100f) > 0.01f) { importer.spritePixelsPerUnit = 100f; changed = true; }
                if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; changed = true; }
                if (importer.textureCompression != TextureImporterCompression.CompressedHQ) { importer.textureCompression = TextureImporterCompression.CompressedHQ; changed = true; }
                if (changed)
                    importer.SaveAndReimport();
            }
        }

        static int ExpectedComponentCount(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.PreLevel: return 12;
                case ScreenId.Gameplay: return 5;
                case ScreenId.Pause: return 7;
                default: return 0;
            }
        }

        static string ReferenceSha(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.PreLevel: return PreLevelReferenceSha;
                case ScreenId.Gameplay: return GameplayReferenceSha;
                case ScreenId.Pause: return PauseReferenceSha;
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

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
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
