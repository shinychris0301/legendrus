using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// v1.20 Step 3.3R-FIX01 runtime-safe production UI composition.
    /// Uses only standalone runtime Resources assets and live UI objects; captured-screen
    /// and QA-reference art are outside this runtime class.
    /// </summary>
    public static class ProductionUiFix01Runtime
    {
        public const string StyleVersion = "v1.20-step03.3r-fix01";
        public const float ReferenceWidth = 941f;
        public const float ReferenceHeight = 1672f;
        const string Root = "ProductionUI/Assets/Fix01/";
        const string Kit = Root + "Kit/";
        const string Icons = Root + "Icons/";
        const string Gems = Root + "Gems/";

        static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        static Font cachedFont;

        public static bool TryBuild(ScreenId id, RectTransform parent)
        {
            if (!ProductionCoreLoopPageRegistry.IsCutover(id) || parent == null)
                return false;

            GameObject root = new GameObject("ProductionFix01_" + id, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            Stretch(root.GetComponent<RectTransform>());
            BuildInto(id, root.GetComponent<RectTransform>());
            return true;
        }

        public static void BuildInto(ScreenId id, RectTransform root)
        {
            if (root == null)
                return;
            if (id == ScreenId.Loading)
            {
                ProductionLoadingRuntime.BuildInto(root);
                return;
            }
            if (id == ScreenId.Home)
            {
                ProductionHomeRuntime.BuildInto(root);
                return;
            }
            if (id == ScreenId.Gameplay)
            {
                ProductionGameplayRuntime.BuildInto(root);
                return;
            }
            if (id == ScreenId.PreLevel)
            {
                ProductionPreLevelRuntime.BuildInto(root);
                return;
            }
            if (id == ScreenId.ChapterMap)
            {
                ProductionChapterMapRuntime.BuildInto(root);
                return;
            }
            if (id == ScreenId.WorldMap)
            {
                ProductionWorldMapRuntime.BuildInto(root);
                return;
            }
            if (root.Find("FIX01_CONTENT") != null)
                return;

            GameObject content = new GameObject("FIX01_CONTENT", typeof(RectTransform));
            content.transform.SetParent(root, false);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            Stretch(contentRect);

            switch (id)
            {
                case ScreenId.WorldMap: BuildWorldMap(contentRect); break;
                case ScreenId.ChapterMap: BuildChapterMap(contentRect); break;
                case ScreenId.PreLevel: BuildPreLevel(contentRect); break;
                case ScreenId.Gameplay: BuildGameplay(contentRect); break;
                case ScreenId.Pause: BuildPause(contentRect); break;
                case ScreenId.Victory: BuildVictory(contentRect); break;
                case ScreenId.Fail: BuildFail(contentRect); break;
                case ScreenId.ChapterComplete: BuildChapterComplete(contentRect); break;
            }
        }

        /// <summary>Exactly the distinct textures consumed by the current aligned Home composition.</summary>
        public static IReadOnlyList<string> GetHomeTextureResourceKeys()
        {
            return ProductionHomeRuntime.GetTextureResourceKeys();
        }

        static void BuildWorldMap(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_worldmap_clean", 0, 0, 941, 1672);
            BuildTopHud(root, true);
            AddTexture(root, "TitlePlaque", Root + "title_plaque_navy", 195, 145, 551, 145, true);
            AddTitle(root, "КАРТА МИРА", 235, 180, 471, 70, 43);

            Vector2[] nodes =
            {
                new Vector2(493,1238), new Vector2(606,1160), new Vector2(458,1132), new Vector2(290,1038),
                new Vector2(458,1019), new Vector2(634,957), new Vector2(482,929), new Vector2(300,873),
                new Vector2(482,801), new Vector2(660,744), new Vector2(444,686), new Vector2(333,620),
                new Vector2(625,566), new Vector2(466,518), new Vector2(525,426)
            };
            for (int i = 0; i < nodes.Length; i++)
                AddMapNode(root, i + 1, nodes[i].x, nodes[i].y, i < 4 ? 3 : (i < 8 ? 2 : 1), false);

            AddWorldNav(root, "MapNav", "icon_map", "КАРТА", 65, 1405);
            AddWorldNav(root, "TasksNav", "icon_tasks", "ЗАДАНИЯ", 343, 1405);
            AddWorldNav(root, "GuildNav", "icon_guild", "ГИЛЬДИЯ", 621, 1405);
            // Functionality-first demo cue. Final art/layout will be replaced
            // during the later asset-production pass.
            AddIcon(root, "Back", "icon_back", 8, 1515, 82, 82);
        }

        static void BuildChapterMap(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_chaptermap_clean", 0, 0, 941, 1672);
            BuildTopHud(root, true);
            AddTexture(root, "ChapterRibbon", Root + "ribbon_red", 90, 145, 761, 235, true);
            AddGoldText(root, "ГЛАВА 1", 260, 188, 421, 44, 27, TextAnchor.MiddleCenter);
            AddTitle(root, "РОДНАЯ ДЕРЕВНЯ", 145, 230, 651, 84, 45);

            Vector2[] nodes =
            {
                new Vector2(438,1368), new Vector2(318,1220), new Vector2(530,1090), new Vector2(314,970),
                new Vector2(546,907), new Vector2(416,805), new Vector2(485,635), new Vector2(360,560),
                new Vector2(560,490), new Vector2(440,420)
            };
            for (int i = 0; i < nodes.Length; i++)
                AddMapNode(root, i + 1, nodes[i].x, nodes[i].y, i < 3 ? 3 : 0, i >= 3);

            AddIcon(root, "Back", "icon_back", 24, 1450, 145, 145);
            AddKit(root, "Play", "ui_btn_primary", 205, 1435, 535, 165);
            AddTitle(root, "ИГРАТЬ", 285, 1470, 375, 75, 46);
        }

        static void BuildPreLevel(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_prelevel_clean", 0, 0, 941, 1672);
            BuildTopHud(root, true);
            AddTexture(root, "ChapterRibbon", Root + "ribbon_red", 90, 145, 761, 235, true);
            AddGoldText(root, "ГЛАВА 1", 260, 188, 421, 44, 27, TextAnchor.MiddleCenter);
            AddTitle(root, "РОДНАЯ ДЕРЕВНЯ", 145, 230, 651, 84, 45);

            AddKit(root, "GoalsPanel", "ui_panel_navy", 205, 405, 531, 310);
            AddTitle(root, "ЦЕЛИ", 330, 438, 281, 56, 32);
            AddGem(root, "GoalRuby", "Gem_Ruby", 300, 510, 135, 135);
            AddGem(root, "GoalSapphire", "Gem_Sapphire", 510, 510, 135, 135);
            AddGoldText(root, "30", 320, 635, 95, 50, 34, TextAnchor.MiddleCenter);
            AddGoldText(root, "30", 530, 635, 95, 50, 34, TextAnchor.MiddleCenter);

            AddKit(root, "Moves", "ui_jewel_round", 370, 700, 200, 200);
            AddGoldText(root, "ХОДЫ", 395, 724, 150, 36, 22, TextAnchor.MiddleCenter);
            AddTitle(root, "28", 400, 755, 140, 100, 58);

            AddKit(root, "BoostersPanel", "ui_panel_navy", 105, 855, 731, 285);
            AddTitle(root, "ВЫБЕРИТЕ БУСТЕРЫ", 225, 880, 491, 50, 31);
            AddBooster(root, "Hammer", "icon_hammer", "7", 145, 945);
            AddBooster(root, "Firebird", "icon_firebird", "5", 320, 945);
            AddBooster(root, "Shield", "icon_shield", "8", 495, 945);
            AddBooster(root, "Potion", "icon_potion", "10", 670, 945);

            AddKit(root, "RewardPanel", "ui_panel_navy", 105, 1160, 731, 250);
            AddTitle(root, "НАГРАДА ЗА УРОВЕНЬ", 190, 1185, 561, 50, 31);
            AddIcon(root, "RewardChest", "icon_chest", 230, 1245, 150, 150);
            AddIcon(root, "RewardCoin", "icon_coin", 485, 1255, 90, 90);
            AddGoldText(root, "500", 590, 1265, 120, 55, 34, TextAnchor.MiddleLeft);
            AddIcon(root, "RewardGem", "icon_gem", 485, 1330, 90, 90);
            AddGoldText(root, "10", 590, 1340, 120, 55, 34, TextAnchor.MiddleLeft);

            AddIcon(root, "Back", "icon_back", 25, 1440, 135, 135);
            AddKit(root, "Start", "ui_btn_primary", 205, 1435, 535, 165);
            AddTitle(root, "ИГРАТЬ", 285, 1470, 375, 75, 46);
        }

        static void BuildGameplay(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_gameplay_clean", 0, 0, 941, 1672);
            AddKit(root, "TopPanel", "ui_panel_navy", 0, 0, 941, 255);
            AddKit(root, "PortraitFrame", "ui_jewel_round", 15, 18, 175, 175);
            AddTexture(root, "Portrait", Root + "portrait_ivan", 35, 37, 135, 135, true);
            AddGoldText(root, "Иван", 205, 28, 175, 42, 27, TextAnchor.MiddleLeft);
            AddGoldText(root, "Уровень 28", 205, 72, 175, 34, 21, TextAnchor.MiddleLeft);
            AddKit(root, "Moves", "ui_jewel_round", 390, 66, 175, 175);
            AddGoldText(root, "ХОДЫ", 410, 92, 135, 36, 21, TextAnchor.MiddleCenter);
            AddTitle(root, "28", 415, 122, 125, 78, 50);
            AddTitle(root, "ЦЕЛИ", 660, 25, 200, 46, 30);
            AddGem(root, "GoalRuby", "Gem_Ruby", 650, 76, 95, 95);
            AddGem(root, "GoalSapphire", "Gem_Sapphire", 770, 76, 95, 95);

            AddKit(root, "BoardFrame", "ui_board_frame", 54, 325, 833, 995);

            AddBooster(root, "Hammer", "icon_hammer", "7", 45, 1300);
            AddBooster(root, "Firebird", "icon_firebird", "5", 245, 1300);
            AddBooster(root, "Shield", "icon_shield", "8", 445, 1300);
            AddBooster(root, "Potion", "icon_potion", "10", 645, 1300);
            AddIcon(root, "Pause", "icon_gear", 835, 1320, 95, 95);

            AddKit(root, "ScorePanel", "ui_panel_navy", 180, 1500, 581, 120);
            AddGoldText(root, "СЧЁТ", 350, 1510, 241, 34, 22, TextAnchor.MiddleCenter);
        }

        static void BuildPause(RectTransform root)
        {
            AddTint(root, "Dimmer", 0, 0, 941, 1672, new Color32(0, 0, 0, 172));
            AddKit(root, "Modal", "ui_modal_frame", 150, 295, 641, 1125);
            AddKit(root, "Header", "ui_header_frame", 235, 350, 471, 145);
            AddTitle(root, "ПАУЗА", 300, 392, 341, 64, 42);
            AddTitle(root, "ЦЕЛИ", 330, 520, 281, 46, 28);
            AddGem(root, "GoalRuby", "Gem_Ruby", 330, 575, 100, 100);
            AddGem(root, "GoalSapphire", "Gem_Sapphire", 515, 575, 100, 100);
            AddTitle(root, "БУСТЕРЫ", 330, 705, 281, 46, 28);
            AddIcon(root, "Hammer", "icon_hammer", 250, 760, 100, 100);
            AddIcon(root, "Firebird", "icon_firebird", 370, 760, 100, 100);
            AddIcon(root, "Shield", "icon_shield", 490, 760, 100, 100);
            AddIcon(root, "Potion", "icon_potion", 610, 760, 100, 100);
            AddKit(root, "Resume", "ui_btn_primary", 255, 900, 431, 100);
            AddGoldText(root, "ПРОДОЛЖИТЬ", 290, 920, 361, 56, 28, TextAnchor.MiddleCenter);
            AddKit(root, "Restart", "ui_btn_secondary", 255, 1015, 431, 100);
            AddGoldText(root, "ЗАНОВО", 290, 1035, 361, 56, 28, TextAnchor.MiddleCenter);
            AddKit(root, "Settings", "ui_btn_secondary", 255, 1130, 431, 100);
            AddGoldText(root, "НАСТРОЙКИ", 290, 1150, 361, 56, 26, TextAnchor.MiddleCenter);
            AddKit(root, "Exit", "ui_btn_danger", 255, 1245, 431, 100);
            AddGoldText(root, "ВЫЙТИ", 290, 1265, 361, 56, 28, TextAnchor.MiddleCenter);
        }

        static void BuildVictory(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_home_clean", 0, 0, 941, 1672);
            AddTint(root, "Veil", 0, 0, 941, 1672, new Color32(0, 30, 20, 90));
            AddKit(root, "Header", "ui_header_frame", 145, 160, 651, 190);
            AddTitle(root, "ПОБЕДА", 240, 205, 461, 80, 56);
            for (int i = 0; i < 3; i++) AddIcon(root, "Star" + i, "icon_star", 300 + i * 120, 540, 105, 105);
            AddKit(root, "Reward", "ui_panel_parchment", 150, 720, 641, 420);
            AddDarkText(root, "НАГРАДА", 300, 760, 341, 58, 34, TextAnchor.MiddleCenter);
            AddIcon(root, "Chest", "icon_chest", 265, 850, 170, 170);
            AddIcon(root, "Gem", "icon_gem", 505, 850, 170, 170);
            AddKit(root, "Continue", "ui_btn_primary", 185, 1330, 570, 149);
            AddTitle(root, "ПРОДОЛЖИТЬ", 250, 1368, 441, 70, 40);
            AddKit(root, "Double", "ui_btn_secondary", 180, 1480, 580, 120);
            AddGoldText(root, "УДВОИТЬ НАГРАДУ", 235, 1510, 471, 60, 28, TextAnchor.MiddleCenter);
        }

        static void BuildFail(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_home_clean", 0, 0, 941, 1672);
            AddTint(root, "Veil", 0, 0, 941, 1672, new Color32(40, 0, 0, 120));
            AddKit(root, "Header", "ui_header_frame", 145, 160, 651, 190);
            AddTitle(root, "ПОРАЖЕНИЕ", 205, 205, 531, 80, 52);
            AddKit(root, "Advice", "ui_panel_parchment", 145, 690, 651, 440);
            AddDarkText(root, "ПОПРОБУЙТЕ ЕЩЁ РАЗ", 215, 865, 511, 90, 34, TextAnchor.MiddleCenter);
            AddKit(root, "Retry", "ui_btn_primary", 185, 1260, 570, 140);
            AddTitle(root, "ПОВТОРИТЬ", 260, 1292, 421, 70, 40);
            AddKit(root, "Exit", "ui_btn_secondary", 190, 1450, 560, 120);
            AddGoldText(root, "К КАРТЕ", 290, 1480, 361, 60, 32, TextAnchor.MiddleCenter);
        }

        static void BuildChapterComplete(RectTransform root)
        {
            AddTexture(root, "Background", Root + "bg_chaptermap_clean", 0, 0, 941, 1672);
            AddTint(root, "Veil", 0, 0, 941, 1672, new Color32(0, 10, 25, 80));
            AddKit(root, "Header", "ui_header_frame", 115, 170, 711, 220);
            AddTitle(root, "ГЛАВА ЗАВЕРШЕНА", 150, 220, 641, 84, 45);
            AddIcon(root, "Chest", "icon_chest", 385, 480, 170, 170);
            for (int i = 0; i < 3; i++) AddIcon(root, "Star" + i, "icon_star", 300 + i * 120, 680, 105, 105);
            AddKit(root, "Summary", "ui_panel_navy", 110, 1010, 721, 260);
            AddGoldText(root, "Новая глава ждёт впереди", 195, 1090, 551, 70, 27, TextAnchor.MiddleCenter);
            AddKit(root, "Continue", "ui_btn_primary", 160, 1280, 320, 125);
            AddGoldText(root, "ДАЛЬШЕ", 200, 1310, 240, 60, 30, TextAnchor.MiddleCenter);
            AddKit(root, "Map", "ui_btn_secondary", 500, 1280, 300, 125);
            AddGoldText(root, "КАРТА", 545, 1310, 210, 60, 30, TextAnchor.MiddleCenter);
        }

        static void BuildTopHud(RectTransform root, bool portrait)
        {
            AddKit(root, "TopHud", "ui_top_bar", 0, 0, 941, 150);
            if (portrait)
            {
                AddKit(root, "PortraitFrame", "ui_jewel_round", 12, 12, 165, 165);
                AddTexture(root, "Portrait", Root + "portrait_ivan", 33, 32, 122, 122, true);
                AddGoldText(root, "Иван", 180, 30, 145, 36, 25, TextAnchor.MiddleLeft);
                AddGoldText(root, "Уровень 28", 180, 69, 145, 32, 19, TextAnchor.MiddleLeft);
                AddKit(root, "LevelTrack", "ui_progress_track", 180, 111, 155, 18);
            }
            AddKit(root, "GoldPill", "ui_currency_pill", 345, 24, 184, 78);
            AddIcon(root, "CoinIcon", "icon_coin", 348, 29, 64, 64);
            AddKit(root, "GemPill", "ui_currency_pill", 530, 24, 165, 78);
            AddIcon(root, "GemIcon", "icon_gem", 535, 29, 64, 64);
            AddKit(root, "EnergyPill", "ui_currency_pill", 700, 24, 155, 78);
            AddIcon(root, "EnergyIcon", "icon_energy", 705, 29, 64, 64);
            AddIcon(root, "SettingsIcon", "icon_gear", 850, 18, 84, 84);
        }

        static void AddWorldNav(RectTransform root, string name, string icon, string label, float x, float y)
        {
            AddKit(root, name + "Frame", "ui_nav_tile", x, y, 255, 205);
            AddIcon(root, name + "Icon", icon, x + 65, y + 18, 125, 125);
            AddGoldText(root, label, x + 20, y + 150, 215, 42, 22, TextAnchor.MiddleCenter);
        }

        static void AddMapNode(RectTransform root, int number, float cx, float cy, int stars, bool locked)
        {
            const float s = 90f;
            AddKit(root, "Node" + number, "ui_map_node", cx - s * 0.5f, cy - s * 0.5f, s, s);
            AddGoldText(root, number.ToString(), cx - 32, cy - 27, 64, 54, 28, TextAnchor.MiddleCenter);
            if (locked)
                AddIcon(root, "Lock" + number, "icon_lock", cx + 20, cy + 15, 40, 40);
            else
                for (int i = 0; i < stars; i++)
                    AddIcon(root, "NodeStar" + number + "_" + i, "icon_star", cx - 42 + i * 30, cy + 34, 30, 30);
        }

        static void AddBooster(RectTransform root, string name, string icon, string count, float x, float y)
        {
            AddKit(root, name + "Slot", "ui_booster_slot", x, y, 150, 150);
            AddIcon(root, name + "Icon", icon, x + 20, y + 17, 110, 110);
            AddKit(root, name + "CountFrame", "ui_jewel_round", x + 100, y + 100, 50, 50);
            AddGoldText(root, count, x + 107, y + 108, 36, 32, 18, TextAnchor.MiddleCenter);
        }

        static void AddGem(RectTransform parent, string name, string gem, float x, float y, float w, float h)
        {
            AddTexture(parent, name, Gems + gem, x, y, w, h, true);
        }

        static void AddIcon(RectTransform parent, string name, string icon, float x, float y, float w, float h)
        {
            AddTexture(parent, name, Icons + icon, x, y, w, h, true);
        }

        static void AddKit(RectTransform parent, string name, string asset, float x, float y, float w, float h)
        {
            AddTexture(parent, name, Kit + asset, x, y, w, h, false);
        }

        static void AddTexture(RectTransform parent, string name, string resourceKey, float x, float y, float w, float h, bool preserveAspect = false)
        {
            Sprite sprite = LoadSprite(resourceKey);
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            SetReferenceRect(rect, x, y, w, h);
            Image image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
        }

        static void AddTint(RectTransform parent, string name, float x, float y, float w, float h, Color32 color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            SetReferenceRect(rect, x, y, w, h);
            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
        }

        static void AddTitle(RectTransform parent, string text, float x, float y, float w, float h, int size)
        {
            AddText(parent, text, x, y, w, h, size, new Color32(248, 211, 121, 255), TextAnchor.MiddleCenter, true);
        }

        static void AddGoldText(RectTransform parent, string text, float x, float y, float w, float h, int size, TextAnchor anchor)
        {
            AddText(parent, text, x, y, w, h, size, new Color32(244, 214, 151, 255), anchor, false);
        }

        static void AddDarkText(RectTransform parent, string text, float x, float y, float w, float h, int size, TextAnchor anchor)
        {
            AddText(parent, text, x, y, w, h, size, new Color32(57, 35, 17, 255), anchor, true);
        }

        static void AddText(RectTransform parent, string value, float x, float y, float w, float h, int size, Color32 color, TextAnchor anchor, bool bold)
        {
            GameObject go = new GameObject("Text_" + value, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            SetReferenceRect(rect, x, y, w, h);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = GetFont();
            text.fontSize = size;
            text.alignment = anchor;
            text.color = color;
            text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = new Color32(20, 10, 4, 210);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            outline.useGraphicAlpha = true;
        }

        static Sprite LoadSprite(string key)
        {
            Sprite cached;
            if (SpriteCache.TryGetValue(key, out cached) && cached != null)
                return cached;

            Texture2D texture = HomeLoadingPreparation.LoadPreparedTexture(key) ?? Resources.Load<Texture2D>(key);
            if (texture == null)
            {
                Debug.LogError("FIX01 missing Texture2D resource: " + key);
                return null;
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            sprite.name = texture.name + "_RuntimeSprite";
            SpriteCache[key] = sprite;
            return sprite;
        }

        static Font GetFont()
        {
            if (cachedFont != null)
                return cachedFont;
            cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (cachedFont == null)
                cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedFont;
        }

        static void SetReferenceRect(RectTransform rect, float x, float y, float w, float h)
        {
            float left = x / ReferenceWidth;
            float right = (x + w) / ReferenceWidth;
            float top = 1f - y / ReferenceHeight;
            float bottom = 1f - (y + h) / ReferenceHeight;
            rect.anchorMin = new Vector2(left, bottom);
            rect.anchorMax = new Vector2(right, top);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }
}
