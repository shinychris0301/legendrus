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
    /// Deterministically reconstructs the first ten production pages from the
    /// canonical UI kit. The resulting prefabs are Resources assets and contain
    /// no dependency on Resources/DesignScreens.
    /// </summary>
    public static class Step20CoreLoopPageBuilder
    {
        public const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        public const string KitRoot = "Assets/LegendsOfRus/Art/Production/UI/CanonicalKit";
        public const string StyleVersion = "v1.20-step03.3";
        public const int RequiredPageCount = 10;
        const float W = 941f;
        const float H = 1672f;

        static readonly ScreenId[] RequiredPages =
        {
            ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
            ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause, ScreenId.Victory,
            ScreenId.Fail, ScreenId.ChapterComplete
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
                Debug.LogError("v1.20 Step 3.3 automatic page reconstruction failed: " + e.Message);
            }
        }

        [MenuItem("Legends of Rus/Production UI/Build v1.20 Step 3.3 Core Loop Pages")]
        public static void BuildMenu()
        {
            BuildAll(true);
        }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 3.3 Core Loop Pages")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.3 core-loop page validation passed: 10/10 screenshot-free production pages.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Core-loop production pages validate: 10/10.", "OK");
                return;
            }
            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.3 core-loop page validation failed.\n" + report);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", report, "OK");
        }

        public static void BuildAll(bool showDialog)
        {
            Step20CanonicalUiKitBuilder.BuildAll(false);
            EnsureFolder(PageRoot);
            BuildLoading();
            BuildHome();
            BuildWorldMap();
            BuildChapterMap();
            BuildPreLevel();
            BuildGameplay();
            BuildPause();
            BuildVictory();
            BuildFail();
            BuildChapterComplete();
            // Step 3.3R.2 owns the higher-fidelity Home / WorldMap / ChapterMap
            // variants. Re-apply them after any manual Step 3.3 base rebuild so
            // the older generic pages cannot accidentally overwrite the
            // reference-match correction.
            Step20CoreLoopReferenceMatchBuilder.BuildAll(false);
            // Step 3.3R.3 owns the higher-fidelity PreLevel / Gameplay / Pause
            // variants. Re-apply them after the R2 pages so the generic base
            // builder cannot regress this correction.
            Step20CoreLoopReferenceMatchR3Builder.BuildAll(false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join("\n", failures.ToArray()));
            if (Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled())
                Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false);

            Debug.Log("v1.20 Step 3.3 built 10 screenshot-free core-loop production page prefabs.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Built 10 core-loop production page prefabs.", "OK");
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
                    failures.Add("Missing production page prefab: " + path);
                    continue;
                }
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null)
                    failures.Add(path + ": missing ProductionCoreLoopPageMarker.");
                else
                {
                    if (marker.ScreenId != id) failures.Add(path + ": marker ScreenId mismatch.");
                    if (!marker.ScreenshotFree) failures.Add(path + ": screenshot-free flag is false.");
                    if (marker.StyleVersion != StyleVersion) failures.Add(path + ": stale style version.");
                    if (marker.ReferenceResolution != new Vector2(W, H)) failures.Add(path + ": reference resolution mismatch.");
                }
                string[] dependencies = AssetDatabase.GetDependencies(path, true);
                foreach (string dependency in dependencies)
                    if (dependency.Replace('\\','/').Contains("/Resources/DesignScreens/"))
                        failures.Add(path + ": forbidden screenshot dependency: " + dependency);
            }
            return failures;
        }

        static bool NeedsBuild()
        {
            foreach (ScreenId id in RequiredPages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PageRoot + "/" + id + ".prefab");
                if (prefab == null) return true;
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null || marker.StyleVersion != StyleVersion || !marker.ScreenshotFree || marker.ScreenId != id)
                    return true;
            }
            return false;
        }

        static GameObject CreatePage(ScreenId id, Color top, Color bottom)
        {
            GameObject root = new GameObject(id.ToString(), typeof(RectTransform), typeof(ProductionUiGradient), typeof(ProductionCoreLoopPageMarker));
            RectTransform rt = root.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(W, H);
            ProductionUiGradient gradient = root.GetComponent<ProductionUiGradient>();
            gradient.Configure(top, bottom);
            root.GetComponent<ProductionCoreLoopPageMarker>().Configure(id);
            AddCornerDecor(root.transform);
            return root;
        }

        static void BuildLoading()
        {
            GameObject root = CreatePage(ScreenId.Loading, C(10, 38, 67), C(2, 8, 15));
            AddSprite(root.transform, "TitleFrame", "ui_header_frame.png", 105, 430, 731, 260, true);
            AddText(root.transform, "Title", "ЛЕГЕНДЫ РУСИ", 150, 486, 641, 84, 61, TextAnchor.MiddleCenter, Gold());
            AddText(root.transform, "Subtitle", "ТРИ В РЯД", 235, 580, 470, 56, 34, TextAnchor.MiddleCenter, Pale());
            AddSprite(root.transform, "Jewel", "ui_jewel_sapphire.png", 407, 360, 126, 126, false);
            AddSprite(root.transform, "ProgressTrack", "ui_progress_track.png", 150, 1328, 641, 70, true);
            AddSprite(root.transform, "ProgressFill", "ui_progress_fill.png", 166, 1344, 420, 38, true);
            AddText(root.transform, "LoadingText", "ЗАГРУЗКА...", 250, 1424, 441, 54, 29, TextAnchor.MiddleCenter, Pale());
            Save(root, ScreenId.Loading);
        }

        static void BuildHome()
        {
            GameObject root = CreatePage(ScreenId.Home, C(11, 44, 77), C(17, 14, 12));
            AddTopBar(root.transform);
            AddSprite(root.transform, "TitleFrame", "ui_header_frame.png", 150, 180, 641, 220, true);
            AddText(root.transform, "Title", "ЛЕГЕНДЫ РУСИ", 205, 222, 531, 75, 52, TextAnchor.MiddleCenter, Gold());
            AddText(root.transform, "Subtitle", "ТРИ В РЯД", 295, 302, 351, 48, 28, TextAnchor.MiddleCenter, Pale());
            AddSprite(root.transform, "HeroPanel", "ui_panel_navy.png", 100, 435, 741, 465, true, 0.96f);
            AddSprite(root.transform, "HeroJewel", "ui_jewel_sapphire.png", 397, 505, 146, 146, false);
            AddText(root.transform, "HeroTitle", "ИВАН И ЖАР-ПТИЦА", 160, 675, 621, 72, 38, TextAnchor.MiddleCenter, Gold());
            AddText(root.transform, "HeroCopy", "ПУТЬ К ЛЕГЕНДАМ НАЧИНАЕТСЯ ЗДЕСЬ", 185, 755, 571, 56, 23, TextAnchor.MiddleCenter, Pale());
            AddButton(root.transform, "PlayVisual", "ui_btn_primary.png", "ИГРАТЬ", 218, 1014, 510, 190, 55);
            AddNavTile(root.transform, "WorldMapVisual", "КАРТА МИРА", 52, 1210, 205, 213);
            AddNavTile(root.transform, "ShopVisual", "МАГАЗИН", 266, 1210, 205, 213);
            AddNavTile(root.transform, "EventsVisual", "СОБЫТИЯ", 478, 1210, 205, 213);
            AddNavTile(root.transform, "CharactersVisual", "ПЕРСОНАЖИ", 688, 1210, 205, 213);
            AddSprite(root.transform, "QuestPanel", "ui_panel_navy.png", 180, 1450, 580, 150, true, 0.95f);
            AddText(root.transform, "Quest", "СОБЕРИТЕ СИНИЕ САМОЦВЕТЫ", 225, 1474, 490, 42, 24, TextAnchor.MiddleCenter, Pale());
            AddSprite(root.transform, "QuestTrack", "ui_progress_track.png", 255, 1530, 430, 44, true);
            AddSprite(root.transform, "QuestFill", "ui_progress_fill.png", 267, 1541, 245, 22, true);
            Save(root, ScreenId.Home);
        }

        static void BuildWorldMap()
        {
            GameObject root = CreatePage(ScreenId.WorldMap, C(33, 64, 80), C(6, 25, 34));
            AddTopBar(root.transform);
            AddSprite(root.transform, "TitleFrame", "ui_header_frame.png", 196, 145, 549, 170, true);
            AddText(root.transform, "Title", "КАРТА МИРА", 245, 187, 451, 74, 47, TextAnchor.MiddleCenter, Gold());
            AddSprite(root.transform, "MapField", "ui_panel_navy.png", 45, 330, 851, 1050, true, 0.72f);
            int[,] pts = { {470,1215},{580,1140},{395,1080},{315,995},{500,940},{655,880},{495,805},{295,745},{505,690},{665,625},{455,555},{305,490},{625,445},{455,390},{575,340} };
            for (int i=0;i<15;i++) AddMapNode(root.transform, i+1, pts[i,0], pts[i,1], i<3?3:(i<8?2:1));
            AddNavTile(root.transform, "MapNav", "КАРТА", 65, 1430, 255, 190);
            AddNavTile(root.transform, "TasksNav", "ЗАДАНИЯ", 343, 1430, 255, 190);
            AddNavTile(root.transform, "GuildNav", "ГИЛЬДИЯ", 621, 1430, 255, 190);
            Save(root, ScreenId.WorldMap);
        }

        static void BuildChapterMap()
        {
            GameObject root = CreatePage(ScreenId.ChapterMap, C(45, 64, 72), C(10, 28, 31));
            AddTopBar(root.transform);
            AddSprite(root.transform, "ChapterHeader", "ui_btn_danger.png", 100, 160, 741, 220, true);
            AddText(root.transform, "ChapterLabel", "ГЛАВА 1", 300, 192, 341, 45, 28, TextAnchor.MiddleCenter, Pale());
            AddText(root.transform, "ChapterName", "РОДНАЯ ДЕРЕВНЯ", 160, 237, 621, 85, 48, TextAnchor.MiddleCenter, Gold());
            AddSprite(root.transform, "PathPanel", "ui_panel_navy.png", 80, 405, 781, 970, true, 0.60f);
            int[,] pts = { {445,1220},{555,1130},{370,1050},{300,950},{480,870},{610,780},{470,690},{335,600},{535,510},{450,430} };
            for(int i=0;i<10;i++) AddMapNode(root.transform,i+1,pts[i,0],pts[i,1],i<3?3:0);
            AddButton(root.transform, "PlayVisual", "ui_btn_primary.png", "ИГРАТЬ", 270, 1435, 400, 130, 46);
            Save(root, ScreenId.ChapterMap);
        }

        static void BuildPreLevel()
        {
            GameObject root = CreatePage(ScreenId.PreLevel, C(15, 45, 72), C(5, 16, 27));
            AddTopBar(root.transform);
            AddSprite(root.transform, "Header", "ui_header_frame.png", 135, 185, 671, 170, true);
            AddText(root.transform, "Title", "ПОДГОТОВКА К УРОВНЮ", 180, 225, 581, 70, 38, TextAnchor.MiddleCenter, Gold());
            AddSprite(root.transform, "Goals", "ui_panel_parchment.png", 155, 430, 631, 360, true);
            AddText(root.transform, "GoalsTitle", "ЦЕЛИ", 320, 470, 301, 60, 32, TextAnchor.MiddleCenter, Ink());
            AddSprite(root.transform, "Goal1", "ui_goal_panel.png", 250, 560, 210, 120, true);
            AddSprite(root.transform, "Goal2", "ui_goal_panel.png", 480, 560, 210, 120, true);
            AddText(root.transform, "MovesLabel", "ХОДЫ", 350, 700, 241, 40, 24, TextAnchor.MiddleCenter, Ink());
            AddSprite(root.transform, "MovesFrame", "ui_jewel_round.png", 405, 745, 130, 130, false);
            AddSprite(root.transform, "Boosters", "ui_panel_navy.png", 190, 930, 560, 310, true);
            AddText(root.transform, "BoostersTitle", "УСИЛИТЕЛИ", 290, 950, 361, 50, 28, TextAnchor.MiddleCenter, Gold());
            for(int i=0;i<3;i++) AddSprite(root.transform, "Booster"+(i+1), "ui_booster_slot.png", 250+i*155, 1030, 130, 130, true);
            AddSprite(root.transform, "Reward", "ui_panel_parchment.png", 200, 1250, 541, 95, true);
            AddText(root.transform, "RewardLabel", "НАГРАДА", 250, 1265, 180, 50, 23, TextAnchor.MiddleLeft, Ink());
            AddButton(root.transform, "StartVisual", "ui_btn_primary.png", "НАЧАТЬ", 235, 1360, 480, 150, 48);
            Save(root, ScreenId.PreLevel);
        }

        static void BuildGameplay()
        {
            GameObject root = CreatePage(ScreenId.Gameplay, C(8, 36, 60), C(3, 12, 20));
            AddSprite(root.transform, "GoalFrame", "ui_goal_panel.png", 55, 60, 831, 245, true);
            AddText(root.transform, "MovesLabel", "ХОДЫ", 385, 78, 171, 42, 24, TextAnchor.MiddleCenter, Ink());
            AddSprite(root.transform, "MovesJewel", "ui_jewel_round.png", 415, 118, 111, 111, false);
            AddText(root.transform, "GoalsLabel", "ЦЕЛИ", 675, 78, 161, 42, 24, TextAnchor.MiddleCenter, Ink());
            AddSprite(root.transform, "BoardFrame", "ui_board_frame.png", 54, 325, 833, 995, true);
            for(int i=0;i<4;i++) AddSprite(root.transform, "BoosterSlot"+(i+1), "ui_booster_slot.png", 48+i*200, 1310, 185, 185, true);
            AddSprite(root.transform, "PauseVisual", "ui_pause_button.png", 830, 1330, 100, 100, false);
            AddSprite(root.transform, "ScoreFrame", "ui_panel_navy.png", 230, 1508, 481, 108, true);
            AddText(root.transform, "ScoreLabel", "СЧЁТ", 250, 1518, 441, 32, 20, TextAnchor.MiddleCenter, Gold());
            Save(root, ScreenId.Gameplay);
        }

        static void BuildPause()
        {
            GameObject root = CreatePage(ScreenId.Pause, C(3, 10, 18, 205), C(0, 0, 0, 225));
            AddSprite(root.transform, "Modal", "ui_modal_frame.png", 170, 410, 601, 930, true);
            AddSprite(root.transform, "Header", "ui_header_frame.png", 245, 500, 451, 150, true);
            AddText(root.transform, "Title", "ПАУЗА", 320, 545, 301, 60, 42, TextAnchor.MiddleCenter, Gold());
            AddButton(root.transform, "ResumeVisual", "ui_btn_primary.png", "ПРОДОЛЖИТЬ", 275, 790, 400, 105, 31);
            AddButton(root.transform, "RestartVisual", "ui_btn_secondary.png", "ЗАНОВО", 275, 915, 400, 105, 31);
            AddButton(root.transform, "SettingsVisual", "ui_btn_secondary.png", "НАСТРОЙКИ", 275, 1040, 400, 105, 29);
            AddButton(root.transform, "ExitVisual", "ui_btn_danger.png", "ВЫЙТИ", 275, 1170, 400, 105, 31);
            Save(root, ScreenId.Pause);
        }

        static void BuildVictory()
        {
            GameObject root = CreatePage(ScreenId.Victory, C(12, 74, 47), C(3, 16, 22));
            AddSprite(root.transform, "Header", "ui_header_frame.png", 145, 160, 651, 190, true);
            AddText(root.transform, "Title", "ПОБЕДА", 240, 207, 461, 80, 56, TextAnchor.MiddleCenter, Gold());
            AddText(root.transform, "Subtitle", "ЛЕГЕНДА СТАЛА БЛИЖЕ", 260, 300, 421, 45, 24, TextAnchor.MiddleCenter, Pale());
            for(int i=0;i<3;i++) AddSprite(root.transform, "Star"+(i+1), "ui_star.png", 300+i*120, 575, 105, 105, false);
            AddSprite(root.transform, "RewardPanel", "ui_panel_parchment.png", 145, 725, 651, 430, true);
            AddText(root.transform, "RewardTitle", "НАГРАДА", 300, 765, 341, 58, 34, TextAnchor.MiddleCenter, Ink());
            AddSprite(root.transform, "RewardTile1", "ui_reward_tile.png", 250, 860, 185, 205, true);
            AddSprite(root.transform, "RewardTile2", "ui_reward_tile.png", 505, 860, 185, 205, true);
            AddButton(root.transform, "ContinueVisual", "ui_btn_primary.png", "ПРОДОЛЖИТЬ", 185, 1330, 570, 149, 44);
            AddButton(root.transform, "DoubleVisual", "ui_btn_secondary.png", "УДВОИТЬ НАГРАДУ", 180, 1480, 580, 120, 31);
            Save(root, ScreenId.Victory);
        }

        static void BuildFail()
        {
            GameObject root = CreatePage(ScreenId.Fail, C(80, 25, 25), C(8, 10, 18));
            AddSprite(root.transform, "Header", "ui_header_frame.png", 145, 160, 651, 190, true);
            AddText(root.transform, "Title", "ПОРАЖЕНИЕ", 210, 207, 521, 80, 52, TextAnchor.MiddleCenter, Gold());
            AddText(root.transform, "Subtitle", "ПОПРОБУЙТЕ ЕЩЁ РАЗ", 255, 300, 431, 45, 24, TextAnchor.MiddleCenter, Pale());
            AddSprite(root.transform, "AdvicePanel", "ui_panel_parchment.png", 145, 700, 651, 430, true);
            AddSprite(root.transform, "AdviceJewel", "ui_jewel_sapphire.png", 405, 760, 130, 130, false);
            AddText(root.transform, "Advice", "СОБИРАЙТЕ ЦЕЛИ И БЕРЕГИТЕ ХОДЫ", 220, 930, 501, 105, 28, TextAnchor.MiddleCenter, Ink());
            AddButton(root.transform, "RetryVisual", "ui_btn_primary.png", "ПОВТОРИТЬ", 185, 1260, 570, 140, 43);
            AddButton(root.transform, "ExitVisual", "ui_btn_secondary.png", "К КАРТЕ", 190, 1450, 560, 120, 34);
            Save(root, ScreenId.Fail);
        }

        static void BuildChapterComplete()
        {
            GameObject root = CreatePage(ScreenId.ChapterComplete, C(16, 58, 84), C(5, 17, 28));
            AddSprite(root.transform, "Header", "ui_header_frame.png", 115, 170, 711, 220, true);
            AddText(root.transform, "Title", "ГЛАВА ЗАВЕРШЕНА", 155, 220, 631, 84, 47, TextAnchor.MiddleCenter, Gold());
            AddSprite(root.transform, "ChapterJewel", "ui_jewel_sapphire.png", 380, 455, 180, 180, false);
            for(int i=0;i<3;i++) AddSprite(root.transform, "Star"+(i+1), "ui_star.png", 300+i*120, 675, 105, 105, false);
            AddSprite(root.transform, "SummaryFrame", "ui_panel_navy.png", 110, 1010, 721, 260, true);
            AddButton(root.transform, "ContinueVisual", "ui_btn_primary.png", "ДАЛЬШЕ", 160, 1280, 320, 125, 34);
            AddButton(root.transform, "MapVisual", "ui_btn_secondary.png", "КАРТА", 500, 1280, 300, 125, 34);
            AddText(root.transform, "Footer", "НОВАЯ ГЛАВА ЖДЁТ ВПЕРЕДИ", 210, 1460, 521, 60, 25, TextAnchor.MiddleCenter, Pale());
            Save(root, ScreenId.ChapterComplete);
        }

        static void AddTopBar(Transform parent)
        {
            AddSprite(parent, "TopBar", "ui_top_bar.png", 0, 0, 941, 150, true);
            AddSprite(parent, "ProfileMedallion", "ui_jewel_round.png", 18, 20, 105, 105, false);
            AddText(parent, "PlayerName", "ИВАН", 130, 28, 205, 38, 27, TextAnchor.MiddleLeft, Gold());
            AddText(parent, "PlayerLevel", "УРОВЕНЬ 28", 130, 68, 205, 30, 20, TextAnchor.MiddleLeft, Pale());
            AddSprite(parent, "PlayerProgress", "ui_progress_track.png", 130, 108, 185, 18, true);
            AddSprite(parent, "GoldPill", "ui_currency_pill.png", 350, 25, 175, 72, true);
            AddSprite(parent, "GemPill", "ui_currency_pill.png", 535, 25, 155, 72, true);
            AddSprite(parent, "EnergyPill", "ui_currency_pill.png", 705, 25, 150, 72, true);
            AddSprite(parent, "Settings", "ui_icon_button.png", 850, 24, 82, 82, false);
        }

        static void AddNavTile(Transform parent, string name, string label, float x, float y, float w, float h)
        {
            AddSprite(parent, name, "ui_nav_tile.png", x, y, w, h, true);
            AddText(parent, name+"Label", label, x+10, y+h-56, w-20, 42, Math.Max(17, Math.Min(24,(int)(w/8))), TextAnchor.MiddleCenter, Gold());
        }

        static void AddMapNode(Transform parent, int number, int centerX, int centerY, int stars)
        {
            float size=100f;
            AddSprite(parent, "Node"+number, "ui_map_node.png", centerX-size/2, centerY-size/2, size, size, false);
            AddText(parent, "NodeLabel"+number, number.ToString(), centerX-35, centerY-30, 70, 60, 28, TextAnchor.MiddleCenter, Pale());
            for(int i=0;i<stars;i++) AddSprite(parent, "Node"+number+"Star"+i, "ui_star.png", centerX-36+i*26, centerY+48, 24, 24, false);
        }

        static void AddButton(Transform parent, string name, string sprite, string label, float x, float y, float w, float h, int fontSize)
        {
            AddSprite(parent, name, sprite, x, y, w, h, true);
            AddText(parent, name+"Label", label, x+40, y+20, w-80, h-40, fontSize, TextAnchor.MiddleCenter, Gold());
        }

        static void AddCornerDecor(Transform parent)
        {
            AddSprite(parent, "CornerTL", "ui_corner_flourish.png", 0, 0, 150, 150, false, 0.56f);
            GameObject tr = AddSprite(parent, "CornerTR", "ui_corner_flourish.png", 791, 0, 150, 150, false, 0.56f); tr.transform.localScale = new Vector3(-1f,1f,1f);
            GameObject bl = AddSprite(parent, "CornerBL", "ui_corner_flourish.png", 0, 1522, 150, 150, false, 0.56f); bl.transform.localScale = new Vector3(1f,-1f,1f);
            GameObject br = AddSprite(parent, "CornerBR", "ui_corner_flourish.png", 791, 1522, 150, 150, false, 0.56f); br.transform.localScale = new Vector3(-1f,-1f,1f);
        }

        static GameObject AddSprite(Transform parent, string name, string file, float x, float y, float w, float h, bool sliced, float alpha = 1f)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            SetPixelRect(go.GetComponent<RectTransform>(), x, y, w, h);
            Image image = go.GetComponent<Image>();
            image.sprite = LoadSprite(file);
            image.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            image.preserveAspect = !sliced;
            image.raycastTarget = false;
            image.color = new Color(1f,1f,1f,alpha);
            return go;
        }

        static Text AddText(Transform parent, string name, string value, float x, float y, float w, float h, int size, TextAnchor alignment, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            SetPixelRect(go.GetComponent<RectTransform>(), x, y, w, h);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null) text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Math.Max(12,size/2);
            text.resizeTextMaxSize = size;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.fontStyle = FontStyle.Bold;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        static Sprite LoadSprite(string file)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(KitRoot + "/" + file);
            if (sprite == null) throw new InvalidOperationException("Canonical UI sprite missing: " + file);
            return sprite;
        }

        static void SetPixelRect(RectTransform rt, float x, float yTop, float width, float height)
        {
            rt.anchorMin = new Vector2(x/W, 1f-(yTop+height)/H);
            rt.anchorMax = new Vector2((x+width)/W, 1f-yTop/H);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        static Color C(byte r, byte g, byte b, byte a = 255) { return new Color32(r,g,b,a); }
        static Color Gold() { return new Color32(250, 213, 127, 255); }
        static Color Pale() { return new Color32(235, 222, 190, 255); }
        static Color Ink() { return new Color32(49, 29, 13, 255); }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\','/');
            string name = Path.GetFileName(path);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent,name);
        }

        static void Save(GameObject root, ScreenId id)
        {
            PrefabUtility.SaveAsPrefabAsset(root, PageRoot + "/" + id + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
#endif
