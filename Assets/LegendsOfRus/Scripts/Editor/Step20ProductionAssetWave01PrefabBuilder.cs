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
    /// v1.20 Step 7.4 Wave 1 reference-locked candidate builder with dynamic map-state decomposition.
    /// Candidate output remains parallel/inactive and never edits ProductionUI/Pages or the runtime cutover registry.
    /// ApprovedScreens and DesignScreens are measurement/reference sources only and are forbidden prefab dependencies.
    /// </summary>
    public static class Step20ProductionAssetWave01PrefabBuilder
    {
        public const string Version = "v1.20-step07.4-runtime-capture-dynamic-map-conditional-cutover";
        public const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages";
        public const string Wave0Root = "Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation";
        public const string PageRoot = "Assets/LegendsOfRus/Art/Production/Pages";
        public const string RefinedRoot = "Assets/LegendsOfRus/Art/Production/Wave01Refined";
        public const string ProductionFontRoot = "Assets/LegendsOfRus/Art/Fonts/Production";
        public const string DynamicMapRoot = "Assets/LegendsOfRus/Art/Production/Wave01DynamicMaps";
        const float W = 941f;
        const float H = 1672f;
        static Font cachedFont;

        static readonly ScreenId[] Pages =
        {
            ScreenId.SplashTitle, ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap,
            ScreenId.ChapterMap, ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause,
            ScreenId.Victory, ScreenId.Fail, ScreenId.ChapterComplete, ScreenId.Dialogue
        };

        [InitializeOnLoadMethod]
        static void QueueCandidateBuild()
        {
            EditorApplication.delayCall += delegate
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                try { if (NeedsBuild()) BuildAll(false); }
                catch (Exception e) { Debug.LogError("v1.20 Step 7.4 candidate prefab repair failed: " + e.Message); }
            };
        }

        [MenuItem("Legends of Rus/Production UI/Build/Repair v1.20 Step 7.4 Dynamic-State Wave 1 Candidates")]
        public static void BuildMenu() { BuildAll(true); }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 7.4 Dynamic-State Wave 1 Candidates")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0) Debug.Log("v1.20 Step 7.4 Wave 1 candidates validate: 12/12. Active runtime cutover remains conditional.");
            else throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
        }

        public static bool NeedsBuild()
        {
            string lockPath = CandidateRoot + "/WAVE01_BUILD.lock";
            if (!File.Exists(lockPath) || !File.ReadAllText(lockPath).StartsWith(Version, StringComparison.Ordinal)) return true;
            foreach (ScreenId id in Pages)
                if (AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot + "/" + id + ".prefab") == null) return true;
            return false;
        }

        public static void BuildAll(bool showDialog)
        {
            EnsureFolder(CandidateRoot);
            EnsureFolder(ProductionFontRoot);
            ConfigureAllPngs(Wave0Root);
            ConfigureAllPngs(PageRoot);
            ConfigureAllPngs(RefinedRoot);
            ConfigureAllPngs(DynamicMapRoot);
            ConfigureAllPngs("Assets/LegendsOfRus/Resources/Gameplay/Approved");
            cachedFont = null;

            foreach (ScreenId id in Pages) BuildPage(id);
            File.WriteAllText(CandidateRoot + "/WAVE01_BUILD.lock",
                Version + "\nprefabs=12\nactive-runtime-cutover=conditional-only\nreferenceonly-dependencies=forbidden\ndesignscreens-dependencies=forbidden\nlive-localized-text=true\ndynamic-map-state=true\nfont-binaries-bundled=false\n");
            AssetDatabase.ImportAsset(CandidateRoot + "/WAVE01_BUILD.lock", ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            Debug.Log("v1.20 Step 7.4 rebuilt 12 reference-locked Wave 1 candidates with live map state. No automatic runtime cutover was performed.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 candidates rebuilt: 12/12. World/Chapter map labels, stars, locks and selection are live state. Cutover remains conditional on runtime capture QA.", "OK");
        }

        public static List<string> ValidateAll()
        {
            var failures = new List<string>();
            foreach (ScreenId id in Pages)
            {
                string path = CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) { failures.Add("Missing candidate prefab: " + path); continue; }
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null || marker.ScreenId != id || !marker.ScreenshotFree) failures.Add(path + ": screenshot-free page marker missing/mismatch.");
                string[] deps = AssetDatabase.GetDependencies(path, true);
                foreach (string dep in deps)
                {
                    string n = dep.Replace('\\','/');
                    if (n.Contains("/Art/ReferenceOnly/") || n.Contains("/Resources/DesignScreens/") || n.Contains("/ProjectDocs/"))
                        failures.Add(path + ": forbidden reference/QA dependency " + dep);
                }
                Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                if (texts.Length == 0) failures.Add(path + ": no live Text components found.");
                foreach (Text t in texts)
                {
                    if (t.raycastTarget) failures.Add(path + ": decorative/live text must not intercept input: " + t.name);
                }
            }
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Active production registry count changed; expected 10.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.SplashTitle)) failures.Add("SplashTitle must remain candidate-only.");
            if (ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Dialogue)) failures.Add("Dialogue must remain candidate-only.");
            if (!File.Exists(RefinedRoot + "/wave01_refined_manifest.json")) failures.Add("Missing refined reconstruction manifest.");
            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_DYNAMIC_MAP_DECOMPOSITION.json"))
                failures.Add("Missing Step 7.4 dynamic-map decomposition manifest.");

            GameObject worldMap = AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot + "/WorldMap.prefab");
            GameObject chapterMap = AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot + "/ChapterMap.prefab");
            ProductionMapVisualStateController worldController = worldMap != null ? worldMap.GetComponent<ProductionMapVisualStateController>() : null;
            ProductionMapVisualStateController chapterController = chapterMap != null ? chapterMap.GetComponent<ProductionMapVisualStateController>() : null;
            if (worldController == null || worldController.Mode != ProductionMapVisualMode.WorldChapters || worldController.VisibleSlotCount != 15 || worldController.LogicalCapacity != 20)
                failures.Add("WorldMap dynamic visual-state controller must expose 15 visible slots over 20 logical chapters.");
            if (chapterController == null || chapterController.Mode != ProductionMapVisualMode.ChapterLevels || chapterController.VisibleSlotCount != 7 || chapterController.LogicalCapacity != 10)
                failures.Add("ChapterMap dynamic visual-state controller must expose 7 visible slots over 10 authored Chapter 1 levels.");
            return failures;
        }

        static void BuildPage(ScreenId id)
        {
            GameObject root = new GameObject("Wave01_" + id, typeof(RectTransform)); root.layer = 5;
            RectTransform rt = root.GetComponent<RectTransform>(); Stretch(rt);
            ProductionCoreLoopPageMarker marker = root.AddComponent<ProductionCoreLoopPageMarker>(); marker.Configure(id);
            switch (id)
            {
                case ScreenId.SplashTitle: BuildSplash(rt); break;
                case ScreenId.Loading: BuildLoading(rt); break;
                case ScreenId.Home: BuildHome(rt); break;
                case ScreenId.WorldMap: BuildWorldMap(rt); break;
                case ScreenId.ChapterMap: BuildChapterMap(rt); break;
                case ScreenId.PreLevel: BuildPreLevel(rt); break;
                case ScreenId.Gameplay: BuildGameplay(rt); break;
                case ScreenId.Pause: BuildPause(rt); break;
                case ScreenId.Victory: BuildVictory(rt); break;
                case ScreenId.Fail: BuildFail(rt); break;
                case ScreenId.ChapterComplete: BuildChapterComplete(rt); break;
                case ScreenId.Dialogue: BuildDialogue(rt); break;
            }
            PrefabUtility.SaveAsPrefabAsset(root, CandidateRoot + "/" + id + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
        }

        // Three style-derived pages remain explicit candidates until direct approved references exist.
        static void BuildSplash(RectTransform r)
        {
            Refined(r,"SplashTitle","environment_title.png",0,0,941,1672);
            Refined(r,"SplashTitle","hero_firebird_title.png",0,250,941,930);
            Refined(r,"SplashTitle","ambient_sparks.png",0,170,941,941);
            Refined(r,"SplashTitle","logo_plaque.png",155,205,631,154);
            DisplayT(r,"ЛЕГЕНДЫ РУСИ",165,225,611,65,46); HeadingT(r,"ТРИ В РЯД",265,292,411,48,27);
            Button(r,"НАЧАТЬ",240,1325,461,128,true); HudT(r,"v1.20",360,1520,221,40,18);
        }

        static void BuildLoading(RectTransform r)
        {
            // Exact approved composition is reconstructed from independent sectional components, never ApprovedScreens/Loading.png.
            Refined(r,"Loading","hero_logo_scene.png",0,0,941,1040);
            Refined(r,"Loading","tip_panel.png",135,1040,671,345);
            Refined(r,"Loading","loading_footer.png",0,1385,941,287);
            Patch(r,"Loading","tip_title",280,1080,381,54); BodyDarkT(r,"СОВЕТ",280,1080,381,54,26);
            Patch(r,"Loading","tip_body",185,1160,571,120); BodyDarkT(r,"Совет: объединяйте особые самоцветы, чтобы очищать поле быстрее.",185,1160,571,120,18);
            Patch(r,"Loading","loading",300,1400,341,55); DisplayT(r,"ЗАГРУЗКА",300,1400,341,55,28);
            Patch(r,"Loading","percent",405,1510,131,48); HudT(r,"75%",405,1510,131,48,21);
        }

        static void BuildHome(RectTransform r)
        {
            Refined(r,"Home","home_top_hud.png",0,0,941,150);
            Refined(r,"Home","home_logo_scene.png",0,150,941,350);
            Refined(r,"Home","home_hero_scene.png",0,500,941,500);
            Refined(r,"Home","home_play_component.png",0,1000,941,210);
            Refined(r,"Home","home_navigation_component.png",0,1210,941,240);
            Refined(r,"Home","home_quest_component.png",0,1450,941,222);
            HomeHudText(r);
            Patch(r,"Home","play",315,1022,311,90); DisplayT(r,"ИГРАТЬ",315,1022,311,90,34);
            string[] n={"КАРТА МИРА","МАГАЗИН","СОБЫТИЯ","ПЕРСОНАЖИ"}; float[] x={10,245,475,705}; float[] w={220,220,220,226}; string[] k={"nav_map","nav_shop","nav_events","nav_char"};
            for(int i=0;i<4;i++){ Patch(r,"Home",k[i],x[i],1362,w[i],52); HeadingT(r,n[i],x[i],1362,w[i],52,15); }
            Patch(r,"Home","quest",255,1475,431,46); BodyLightT(r,"Соберите синие самоцветы",255,1475,431,46,16);
        }

        static void BuildWorldMap(RectTransform r)
        {
            Refined(r,"WorldMap","worldmap_top_hud.png",0,0,941,135);
            Refined(r,"WorldMap","worldmap_title_component.png",0,135,941,150);
            // The approved painted route remains decorative. Every authored node/star is fully
            // covered by the live dynamic-state layer at its measured anchor.
            Refined(r,"WorldMap","worldmap_painted_map_plate.png",0,285,941,1055);
            BuildDynamicWorldMapState(r);
            Refined(r,"WorldMap","worldmap_bottom_navigation.png",0,1340,941,332);
            HomeHudText(r);
            Patch(r,"WorldMap","title",240,150,461,95); DisplayT(r,"КАРТА МИРА",240,150,461,95,34);
            string[] n={"КАРТА","ЗАДАНИЯ","ГИЛЬДИЯ"}; float[] x={75,365,655}; string[] k={"nav_map","nav_tasks","nav_guild"};
            for(int i=0;i<3;i++){ Patch(r,"WorldMap",k[i],x[i],1500,210,58); HeadingT(r,n[i],x[i],1500,210,58,16); }
        }

        static void BuildChapterMap(RectTransform r)
        {
            Refined(r,"ChapterMap","chaptermap_top_hud.png",0,0,941,140);
            Refined(r,"ChapterMap","chaptermap_ribbon_component.png",0,140,941,250);
            // Seven visible reference-measured slots are windowed over the authoritative
            // ten Chapter 1 levels. Static source labels/stars are only decorative underlay.
            Refined(r,"ChapterMap","chaptermap_painted_level_path.png",0,390,941,1060);
            BuildDynamicChapterMapState(r);
            Refined(r,"ChapterMap","chaptermap_bottom_controls.png",0,1450,941,222);
            HomeHudText(r);
            Patch(r,"ChapterMap","chapter",335,165,271,45); HeadingT(r,"ГЛАВА 1",335,165,271,45,20);
            Patch(r,"ChapterMap","title",170,210,601,90); DisplayT(r,"РОДНАЯ ДЕРЕВНЯ",170,210,601,90,31);
            Patch(r,"ChapterMap","play",315,1510,311,85); DisplayT(r,"ИГРАТЬ",315,1510,311,85,31);
        }

        static void BuildDynamicWorldMapState(RectTransform r)
        {
            // Bindings are ordered from route start (bottom) to route end (top).
            Vector2[] measured =
            {
                new Vector2(493,1246), new Vector2(607,1185), new Vector2(457,1134),
                new Vector2(289,1037), new Vector2(456,1029), new Vector2(638,989),
                new Vector2(482,925), new Vector2(301,869), new Vector2(485,802),
                new Vector2(660,746), new Vector2(445,692), new Vector2(336,620),
                new Vector2(628,568), new Vector2(462,514), new Vector2(524,417)
            };
            BuildDynamicMapBindings(r, ProductionMapVisualMode.WorldChapters, 20, "WorldMap", measured);
        }

        static void BuildDynamicChapterMapState(RectTransform r)
        {
            // Reference viewport exposes seven route slots. Runtime windows those slots across
            // levels 1-10; reference QA may temporarily label them 10-16 without touching save data.
            Vector2[] measured =
            {
                new Vector2(433,1379), new Vector2(317,1217), new Vector2(521,1089),
                new Vector2(325,972), new Vector2(416,809), new Vector2(552,906),
                new Vector2(486,639)
            };
            BuildDynamicMapBindings(r, ProductionMapVisualMode.ChapterLevels, 10, "ChapterMap", measured);
        }

        static void BuildDynamicMapBindings(
            RectTransform r,
            ProductionMapVisualMode mode,
            int logicalCapacity,
            string assetPage,
            Vector2[] measuredCenters
        )
        {
            var bindings = new ProductionMapVisualNodeBinding[measuredCenters.Length];
            Sprite filled = AssetDatabase.LoadAssetAtPath<Sprite>(DynamicMapRoot + "/" + assetPage + "/star_filled.png");
            Sprite empty = AssetDatabase.LoadAssetAtPath<Sprite>(DynamicMapRoot + "/" + assetPage + "/star_empty.png");
            if (filled == null || empty == null)
                throw new InvalidOperationException("Step 7.4 dynamic map star sprites are missing for " + assetPage + ".");

            for (int i = 0; i < measuredCenters.Length; i++)
            {
                Vector2 c = measuredCenters[i];
                GameObject slot = new GameObject("DynamicStateSlot_" + (i + 1).ToString("00"), typeof(RectTransform));
                slot.layer = 5;
                slot.transform.SetParent(r, false);
                Stretch(slot.GetComponent<RectTransform>());

                Image halo = DynImg(slot.GetComponent<RectTransform>(), assetPage, "selected_halo.png", c.x - 50, c.y - 50, 100, 100, "SelectedHalo");
                Image frame = DynImg(slot.GetComponent<RectTransform>(), assetPage, "dynamic_node_frame.png", c.x - 43, c.y - 43, 86, 86, "NodeFrame");
                Image lockIcon = SharedImg(slot.GetComponent<RectTransform>(), "Icons/icon_lock.png", c.x - 17, c.y - 17, 34, 34, "Lock");
                Text label = DynamicMapLabel(slot.GetComponent<RectTransform>(), "1", c.x - 32, c.y - 29, 64, 58, 27);

                var starImages = new Image[3];
                for (int s = 0; s < 3; s++)
                {
                    float sx = c.x - 43 + s * 29;
                    starImages[s] = DynImg(slot.GetComponent<RectTransform>(), assetPage, "star_empty.png", sx, c.y + 35, 29, 29, "Star" + (s + 1));
                }

                bindings[i] = new ProductionMapVisualNodeBinding
                {
                    slotIndex = i,
                    root = slot.GetComponent<RectTransform>(),
                    frame = frame,
                    selectedHalo = halo,
                    lockIcon = lockIcon,
                    label = label,
                    stars = starImages
                };
            }

            ProductionMapVisualStateController controller = r.gameObject.AddComponent<ProductionMapVisualStateController>();
            controller.Configure(mode, logicalCapacity, filled, empty, bindings);
            controller.ApplyQaDefault();
        }

        static Image DynImg(RectTransform r, string page, string file, float x, float y, float w, float h, string name)
        {
            return ImgR(r, DynamicMapRoot + "/" + page + "/" + file, x, y, w, h, name);
        }

        static Image SharedImg(RectTransform r, string rel, float x, float y, float w, float h, string name)
        {
            return ImgR(r, Wave0Root + "/" + rel, x, y, w, h, name);
        }

        static Text DynamicMapLabel(RectTransform r, string value, float x, float y, float w, float h, int size)
        {
            GameObject go = new GameObject("NodeLabel", typeof(RectTransform), typeof(Text));
            go.layer = 5;
            go.transform.SetParent(r, false);
            Set(go.GetComponent<RectTransform>(), x, y, w, h);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = ResolveFont();
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color32(247, 215, 142, 255);
            text.raycastTarget = false;
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color32(41, 20, 6, 235);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            return text;
        }

        static void BuildPreLevel(RectTransform r)
        {
            Refined(r,"PreLevel","environment_clean.png",0,0,941,1672);
            Refined(r,"PreLevel","prelevel_top_hud.png",0,0,941,145);
            Refined(r,"PreLevel","prelevel_chapter_band.png",0,145,941,260);
            Refined(r,"PreLevel","prelevel_goals_left_scene.png",0,405,190,445);
            Refined(r,"PreLevel","prelevel_goals_panel.png",190,405,561,445);
            Refined(r,"PreLevel","prelevel_goals_right_scene.png",751,405,190,445);
            Refined(r,"PreLevel","prelevel_boosters_left_scene.png",0,850,105,285);
            Refined(r,"PreLevel","prelevel_boosters_panel.png",105,850,731,285);
            Refined(r,"PreLevel","prelevel_boosters_right_scene.png",836,850,105,285);
            Refined(r,"PreLevel","prelevel_reward_left_scene.png",0,1135,95,300);
            Refined(r,"PreLevel","prelevel_reward_panel.png",95,1135,751,300);
            Refined(r,"PreLevel","prelevel_reward_right_scene.png",846,1135,95,300);
            Refined(r,"PreLevel","prelevel_bottom_controls.png",0,1435,941,237);
            HomeHudText(r);
            Patch(r,"PreLevel","chapter",335,165,271,45); HeadingT(r,"ГЛАВА 1",335,165,271,45,20);
            Patch(r,"PreLevel","title",170,210,601,90); DisplayT(r,"РОДНАЯ ДЕРЕВНЯ",170,210,601,90,31);
            Patch(r,"PreLevel","goals",370,430,201,45); HeadingT(r,"ЦЕЛИ",370,430,201,45,22);
            Patch(r,"PreLevel","moves",405,715,131,80); DisplayT(r,"28",405,715,131,80,35);
            Patch(r,"PreLevel","boosters",290,865,361,45); HeadingT(r,"ВЫБЕРИТЕ БУСТЕРЫ",290,865,361,45,18);
            Patch(r,"PreLevel","reward",265,1160,411,45); HeadingT(r,"НАГРАДА ЗА УРОВЕНЬ",265,1160,411,45,18);
            Patch(r,"PreLevel","play",315,1510,311,85); DisplayT(r,"ИГРАТЬ",315,1510,311,85,31);
        }

        static void BuildGameplay(RectTransform r)
        {
            Refined(r,"Gameplay","environment_clean.png",0,0,941,1672);
            Refined(r,"Gameplay","gameplay_top_shell.png",0,0,941,357);
            Refined(r,"Gameplay","gameplay_left_environment.png",0,357,79,942);
            Refined(r,"Gameplay","gameplay_right_environment.png",865,357,76,942);
            BuildReferenceMeasuredBoard(r);
            Refined(r,"Gameplay","gameplay_booster_shell.png",0,1299,941,205);
            Refined(r,"Gameplay","gameplay_score_footer.png",0,1504,941,168);
            Patch(r,"Gameplay","player",135,25,150,42); HudT(r,"Иван",135,25,150,42,18);
            Patch(r,"Gameplay","level",135,62,160,35); HudT(r,"Уровень 28",135,62,160,35,14);
            Patch(r,"Gameplay","moves_label",385,78,171,38); HeadingT(r,"ХОДЫ",385,78,171,38,16);
            Patch(r,"Gameplay","moves",415,110,111,65); DisplayT(r,"28",415,110,111,65,29);
            Patch(r,"Gameplay","goals",690,35,161,45); HeadingT(r,"ЦЕЛИ",690,35,161,45,18);
            Patch(r,"Gameplay","score_label",390,1530,161,35); HeadingT(r,"СЧЕТ",390,1530,161,35,15);
            Patch(r,"Gameplay","score",350,1570,241,55); HudT(r,"12 540",350,1570,241,55,22);
        }

        static void BuildReferenceMeasuredBoard(RectTransform r)
        {
            // Reference-measured 8x8 sample arrangement. Runtime gameplay remains authoritative and replaces the sample visuals during active gameplay.
            string R="Gem_Ruby.png", B="Gem_Sapphire.png", G="Gem_Emerald.png", A="Gem_Amber.png", P="Gem_Amethyst.png", S="Gem_Shield.png";
            string[,] m={
                {R,B,S,G,A,R,B,S},{P,S,R,B,R,S,S,A},{R,S,P,A,G,R,R,S},{S,R,R,A,B,G,P,G},
                {S,A,R,R,P,G,S,A},{R,A,S,G,A,R,G,B},{R,P,R,A,G,R,P,R},{G,S,B,A,P,R,B,A}
            };
            float x0=79f,y0=357f,cw=98.25f,ch=117.75f;
            for(int y=0;y<8;y++) for(int x=0;x<8;x++)
            {
                float xx=x0+x*cw, yy=y0+y*ch;
                GameplaySprite(r,"Cell_Approved.png",xx,yy,cw,ch);
                GameplaySprite(r,m[y,x],xx+8,yy+10,cw-16,ch-20);
            }
            GameplaySprite(r,"Special_Burst.png",374,785,193,193);
        }

        static void BuildPause(RectTransform r)
        {
            Refined(r,"Gameplay","environment_clean.png",0,0,941,1672);
            Refined(r,"Pause","pause_backdrop_vignette.png",0,0,941,1672);
            Refined(r,"Pause","pause_header.png",150,295,640,150);
            Refined(r,"Pause","pause_left_border.png",150,445,30,925);
            Refined(r,"Pause","pause_right_border.png",760,445,30,925);
            Refined(r,"Pause","pause_goals_panel.png",180,445,580,270);
            Refined(r,"Pause","pause_boosters_panel.png",180,715,580,200);
            Refined(r,"Pause","pause_buttons_panel.png",180,915,580,455);
            Refined(r,"Pause","pause_footer.png",150,1370,640,50);
            Patch(r,"Pause","title",255,320,431,75); DisplayT(r,"ПАУЗА",255,320,431,75,40);
            Patch(r,"Pause","goals",260,475,421,58); HeadingT(r,"ЦЕЛИ УРОВНЯ",260,475,421,58,20);
            Patch(r,"Pause","boosters",300,735,341,55); HeadingT(r,"БУСТЕРЫ",300,735,341,55,20);
            Patch(r,"Pause","continue",250,940,441,66); DisplayT(r,"ПРОДОЛЖИТЬ",250,940,441,66,24);
            Patch(r,"Pause","restart",250,1050,441,66); HeadingT(r,"ПЕРЕЗАПУСК",250,1050,441,66,22);
            Patch(r,"Pause","settings",250,1160,441,66); HeadingT(r,"НАСТРОЙКИ",250,1160,441,66,22);
            Patch(r,"Pause","exit",250,1270,441,66); HeadingT(r,"ВЫЙТИ",250,1270,441,66,22);
        }

        static void BuildVictory(RectTransform r)
        {
            Refined(r,"Victory","victory_scene.png",0,0,941,640);
            Refined(r,"Victory","victory_stars.png",0,640,941,290);
            Refined(r,"Victory","reward_panel.png",45,930,851,355);
            Refined(r,"Victory","victory_controls.png",0,1285,941,387);
            Patch(r,"Victory","title",180,610,581,110); DisplayT(r,"ПОБЕДА",180,610,581,110,44);
            Patch(r,"Victory","reward_title",310,920,321,48); HeadingT(r,"ВАШИ НАГРАДЫ",310,920,321,48,20);
            Patch(r,"Victory","coins",145,1090,180,55); HudT(r,"5 000",145,1090,180,55,22);
            Patch(r,"Victory","gems",380,1090,180,55); HudT(r,"20",380,1090,180,55,22);
            Patch(r,"Victory","continue",250,1290,441,85); DisplayT(r,"ПРОДОЛЖИТЬ",250,1290,441,85,26);
            Patch(r,"Victory","double",210,1460,521,70); HeadingT(r,"ПОЛУЧИТЬ ×2 НАГРАДЫ",210,1460,521,70,18);
        }

        static void BuildFail(RectTransform r)
        {
            Refined(r,"Fail","fail_scene.png",0,0,941,570);
            Refined(r,"Fail","fail_result.png",125,570,691,335);
            Refined(r,"Fail","fail_goals_actions.png",80,905,781,405);
            Refined(r,"Fail","fail_boosters.png",0,1310,941,362);
            Patch(r,"Fail","title",245,610,451,85); DisplayT(r,"ПОРАЖЕНИЕ",245,610,451,85,34);
            Patch(r,"Fail","subtitle",260,695,421,48); HeadingT(r,"ХОДЫ ЗАКОНЧИЛИСЬ",260,695,421,48,18);
            Patch(r,"Fail","goals",245,805,451,48); HeadingT(r,"НЕ ВЫПОЛНЕНЫ ЦЕЛИ",245,805,451,48,17);
            Patch(r,"Fail","retry",245,1045,451,75); DisplayT(r,"ПОПРОБОВАТЬ СНОВА",245,1045,451,75,23);
            Patch(r,"Fail","exit",350,1220,241,55); HeadingT(r,"ВЫЙТИ",350,1220,241,55,19);
        }

        static void BuildChapterComplete(RectTransform r)
        {
            Refined(r,"ChapterComplete","environment_chapter_complete.png",0,0,941,1672);
            Refined(r,"ChapterComplete","hero_firebird_complete.png",0,170,941,690);
            Refined(r,"ChapterComplete","complete_ribbon.png",85,440,771,210);
            DisplayT(r,"ГЛАВА ЗАВЕРШЕНА",175,485,591,75,36);
            for(int i=0;i<3;i++) Shared(r,"Ornaments/star.png",315+i*105,670,95,95);
            Shared(r,"Panels/panel_navy.png",175,810,591,230); HeadingT(r,"НАГРАДА ЗА ГЛАВУ",245,835,451,45,21);
            Shared(r,"Icons/icon_chest.png",280,900,100,100); Shared(r,"Icons/icon_gem.png",555,915,72,72); HudT(r,"50",630,925,80,45,24);
            Shared(r,"Panels/panel_parchment.png",185,1080,571,180); BodyDarkT(r,"СЛЕДУЮЩАЯ ГЛАВА",260,1100,421,45,20); BodyDarkT(r,"МОЛОЧНЫЕ БЕРЕГА",250,1160,441,45,22);
            Button(r,"ПРОДОЛЖИТЬ",210,1320,521,125,true);
        }

        static void BuildDialogue(RectTransform r)
        {
            Refined(r,"Dialogue","story_scene.png",0,0,941,1085);
            Refined(r,"Dialogue","dialogue_panel.png",55,1085,831,587);
            Patch(r,"Dialogue","speaker",320,1115,301,55); DisplayT(r,"ДОБРЫНЯ",320,1115,301,55,24);
            Patch(r,"Dialogue","body",120,1210,701,210); BodyDarkT(r,"Темная сила пробудилась в дальних землях. Только ты можешь остановить её, Иван. Собери древние артефакты и верни мир на Русь!",120,1210,701,210,20);
            Patch(r,"Dialogue","continue",270,1480,401,75); DisplayT(r,"ПРОДОЛЖИТЬ",270,1480,401,75,24);
        }

        static void HomeHudText(RectTransform r)
        {
            string[] keys={"player","level","coin","gems","energy"};
            string[] values={"Иван","Уровень 28","12 540","350","25/30"};
            float[,] rect={{135,25,150,42},{135,62,160,35},{405,35,120,45},{615,35,90,45},{805,35,100,45}};
            int[] sizes={19,15,19,19,18};
            string page=CurrentPageName(r);
            for(int i=0;i<5;i++){ Patch(r,page,keys[i],rect[i,0],rect[i,1],rect[i,2],rect[i,3]); HudT(r,values[i],rect[i,0],rect[i,1],rect[i,2],rect[i,3],sizes[i]); }
        }

        static string CurrentPageName(RectTransform r)
        {
            string n=r.gameObject.name;
            return n.StartsWith("Wave01_",StringComparison.Ordinal)?n.Substring(7):n;
        }

        static void Button(RectTransform r,string label,float x,float y,float w,float h,bool primary)
        {
            Shared(r,"Buttons/"+(primary?"btn_primary_idle.png":"btn_secondary_idle.png"),x,y,w,h);
            DisplayT(r,label,x+20,y+12,w-40,h-24,w>500?28:24);
        }

        static void GameplaySprite(RectTransform r,string file,float x,float y,float w,float h)
        {
            Img(r,"Assets/LegendsOfRus/Resources/Gameplay/Approved/"+file,x,y,w,h);
        }
        static void Shared(RectTransform r,string rel,float x,float y,float w,float h){ Img(r,Wave0Root+"/"+rel,x,y,w,h); }
        static void Refined(RectTransform r,string page,string file,float x,float y,float w,float h){ Img(r,RefinedRoot+"/"+page+"/"+file,x,y,w,h); }
        static void Patch(RectTransform r,string page,string key,float x,float y,float w,float h)
        {
            string path=RefinedRoot+"/"+page+"/text_patch_"+key+".png";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(path)!=null) Img(r,path,x,y,w,h);
        }
        static Image ImgR(RectTransform r,string path,float x,float y,float w,float h,string name)
        {
            Sprite sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path); if(sprite==null) throw new InvalidOperationException("Missing Sprite: "+path);
            GameObject go=new GameObject(string.IsNullOrEmpty(name)?Path.GetFileNameWithoutExtension(path):name,typeof(RectTransform),typeof(Image)); go.layer=5; go.transform.SetParent(r,false); Set(go.GetComponent<RectTransform>(),x,y,w,h);
            Image image=go.GetComponent<Image>(); image.sprite=sprite; image.type=Image.Type.Simple; image.preserveAspect=false; image.raycastTarget=false; return image;
        }

        static void Img(RectTransform r,string path,float x,float y,float w,float h)
        {
            ImgR(r,path,x,y,w,h,Path.GetFileNameWithoutExtension(path));
        }

        static void DisplayT(RectTransform r,string v,float x,float y,float w,float h,int s){ TextImpl(r,v,x,y,w,h,s,new Color32(244,205,119,255),new Color32(64,28,9,245),true); }
        static void HeadingT(RectTransform r,string v,float x,float y,float w,float h,int s){ TextImpl(r,v,x,y,w,h,s,new Color32(237,203,132,255),new Color32(50,25,10,235),true); }
        static void HudT(RectTransform r,string v,float x,float y,float w,float h,int s){ TextImpl(r,v,x,y,w,h,s,new Color32(244,224,180,255),new Color32(15,18,23,230),false); }
        static void BodyDarkT(RectTransform r,string v,float x,float y,float w,float h,int s){ TextImpl(r,v,x,y,w,h,s,new Color32(73,45,27,255),new Color32(225,194,133,0),false); }
        static void BodyLightT(RectTransform r,string v,float x,float y,float w,float h,int s){ TextImpl(r,v,x,y,w,h,s,new Color32(239,218,176,255),new Color32(20,20,20,210),false); }

        static void TextImpl(RectTransform r,string value,float x,float y,float w,float h,int size,Color color,Color outlineColor,bool strong)
        {
            GameObject go=new GameObject("LiveText",typeof(RectTransform),typeof(Text)); go.layer=5; go.transform.SetParent(r,false); Set(go.GetComponent<RectTransform>(),x,y,w,h);
            Text t=go.GetComponent<Text>(); t.text=value; t.font=ResolveFont(); t.fontSize=size; t.fontStyle=strong?FontStyle.Bold:FontStyle.Normal; t.alignment=TextAnchor.MiddleCenter; t.color=color; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Overflow; t.raycastTarget=false;
            if (outlineColor.a>0.01f)
            {
                Outline o=go.AddComponent<Outline>(); o.effectColor=outlineColor; o.effectDistance=strong?new Vector2(2f,-2f):new Vector2(1f,-1f); o.useGraphicAlpha=true;
                Shadow sh=go.AddComponent<Shadow>(); sh.effectColor=new Color(0f,0f,0f,strong?0.55f:0.35f); sh.effectDistance=new Vector2(1f,-2f); sh.useGraphicAlpha=true;
            }
        }

        static Font ResolveFont()
        {
            if(cachedFont!=null) return cachedFont;
            if(AssetDatabase.IsValidFolder(ProductionFontRoot))
            {
                string[] guids=AssetDatabase.FindAssets("t:Font",new[]{ProductionFontRoot});
                if(guids!=null && guids.Length>0)
                {
                    Array.Sort(guids,StringComparer.Ordinal);
                    cachedFont=AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
            }
            if(cachedFont==null) cachedFont=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if(cachedFont==null) cachedFont=Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedFont;
        }

        static void Set(RectTransform rt,float x,float y,float w,float h){ rt.anchorMin=new Vector2(x/W,1f-(y+h)/H); rt.anchorMax=new Vector2((x+w)/W,1f-y/H); rt.offsetMin=Vector2.zero; rt.offsetMax=Vector2.zero; }
        static void Stretch(RectTransform rt){ rt.anchorMin=Vector2.zero; rt.anchorMax=Vector2.one; rt.offsetMin=Vector2.zero; rt.offsetMax=Vector2.zero; }
        static void ConfigureAllPngs(string root)
        {
            string abs=Path.Combine(Directory.GetCurrentDirectory(),root.Replace('/',Path.DirectorySeparatorChar)); if(!Directory.Exists(abs))return;
            foreach(string f in Directory.GetFiles(abs,"*.png",SearchOption.AllDirectories))
            {
                string asset=f.Replace('\\','/'); string prefix=Directory.GetCurrentDirectory().Replace('\\','/').TrimEnd('/')+"/"; if(asset.StartsWith(prefix,StringComparison.OrdinalIgnoreCase))asset=asset.Substring(prefix.Length);
                TextureImporter ti=AssetImporter.GetAtPath(asset) as TextureImporter; if(ti==null)continue; bool dirty=false;
                if(ti.textureType!=TextureImporterType.Sprite){ti.textureType=TextureImporterType.Sprite;dirty=true;} if(ti.spriteImportMode!=SpriteImportMode.Single){ti.spriteImportMode=SpriteImportMode.Single;dirty=true;} if(ti.alphaIsTransparency!=true){ti.alphaIsTransparency=true;dirty=true;} if(ti.isReadable){ti.isReadable=false;dirty=true;} if(ti.mipmapEnabled){ti.mipmapEnabled=false;dirty=true;} if(ti.wrapMode!=TextureWrapMode.Clamp){ti.wrapMode=TextureWrapMode.Clamp;dirty=true;} if(dirty)ti.SaveAndReimport();
            }
        }
        static void EnsureFolder(string path){ string[] parts=path.Split('/'); string cur=parts[0]; for(int i=1;i<parts.Length;i++){ string next=cur+"/"+parts[i]; if(!AssetDatabase.IsValidFolder(next))AssetDatabase.CreateFolder(cur,parts[i]); cur=next; } }
    }
}
#endif
