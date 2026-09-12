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
    /// v1.20 Step 8.3 reference-accurate refinement builder for Wave 2 meta/progression/economy pages.
    /// Output is parallel and inactive. It never edits ProductionUI/Pages or the runtime cutover registry.
    /// DesignScreens and ReferenceOnly are measurement/reference sources only and are forbidden prefab dependencies.
    /// </summary>
    public static class Step20ProductionAssetWave02PrefabBuilder
    {
        public const string Version = "v1.20-step08.3-reference-accurate-wave02-refinement";
        public const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave02Pages";
        public const string AssetRoot = "Assets/LegendsOfRus/Art/Production/Wave02";
        public const string RefinedRoot = "Assets/LegendsOfRus/Art/Production/Wave02Refined";
        public const string Wave0Root = "Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation";
        public const string ProductionFontRoot = "Assets/LegendsOfRus/Art/Fonts/Production";
        const float W = 941f;
        const float H = 1672f;
        static Font cachedFont;

        static readonly ScreenId[] Pages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory,
            ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect,
            ScreenId.RewardClaim
        };

        [InitializeOnLoadMethod]
        static void QueueBuild()
        {
            EditorApplication.delayCall += delegate
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                try { if (NeedsBuild()) BuildAll(false); }
                catch (Exception e) { Debug.LogError("v1.20 Step 8 Wave 2 candidate build failed: " + e.Message); }
            };
        }

        [MenuItem("Legends of Rus/Production UI/Build/Repair v1.20 Step 8.3 Refined Wave 2 Candidates")]
        public static void BuildMenu() { BuildAll(true); }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 8.3 Refined Wave 2 Candidates")]
        public static void ValidateMenu()
        {
            List<string> failures = ValidateAll();
            if (failures.Count == 0) Debug.Log("v1.20 Step 8.3 Wave 2 candidates validate: 9/9. Active runtime cutover unchanged.");
            else throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
        }

        public static bool NeedsBuild()
        {
            string lockPath = CandidateRoot + "/WAVE02_BUILD.lock";
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
            ConfigureAllPngs(AssetRoot);
            ConfigureAllPngs(RefinedRoot);
            cachedFont = null;

            foreach (ScreenId id in Pages) BuildPage(id);
            File.WriteAllText(CandidateRoot + "/WAVE02_BUILD.lock",
                Version + "\nprefabs=9\nactive-runtime-cutover=false\nreferenceonly-dependencies=forbidden\ndesignscreens-dependencies=forbidden\nsectional-reference-reconstruction=true\nlive-localized-text=true\nprovider-grants=forbidden\nfont-binaries-bundled=false\n");
            AssetDatabase.ImportAsset(CandidateRoot + "/WAVE02_BUILD.lock", ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            Debug.Log("v1.20 Step 8.3 rebuilt 9 refined screenshot-free Wave 2 candidates. Shop uses direct-approved sectional reconstruction; no active runtime cutover was performed.");
            if (showDialog && !Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.3 Wave 2 candidates rebuilt: 9/9. Shop is direct-approved sectional reconstruction; six pages remain demo-guided and two style-system derived. Runtime cutover is unchanged.", "OK");
        }

        public static List<string> ValidateAll()
        {
            var failures = new List<string>();
            if (Pages.Length != 9) failures.Add("Wave 2 page list must contain exactly 9 entries.");
            foreach (ScreenId id in Pages)
            {
                string path = CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) { failures.Add("Missing candidate prefab: " + path); continue; }
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                if (marker == null || marker.ScreenId != id || !marker.ScreenshotFree)
                    failures.Add(path + ": screenshot-free page marker missing/mismatch.");
                string[] deps = AssetDatabase.GetDependencies(path, true);
                foreach (string dep in deps)
                {
                    string n = dep.Replace('\\', '/');
                    if (n.Contains("/Art/ReferenceOnly/") || n.Contains("/Resources/DesignScreens/") || n.Contains("/ProjectDocs/"))
                        failures.Add(path + ": forbidden reference/QA dependency " + dep);
                }
                Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                if (texts.Length == 0) failures.Add(path + ": no live Text components found.");
                foreach (Text t in texts)
                    if (t.raycastTarget) failures.Add(path + ": text must not intercept input: " + t.name);
            }
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))
                failures.Add("Accepted active production registry changed; expected 10 routes.");
            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP08_WAVE02_ASSET_MANIFEST.json"))
                failures.Add("Missing Step 8 Wave 2 asset manifest.");
            if (!File.Exists(RefinedRoot + "/wave02_refined_manifest.json"))
                failures.Add("Missing Step 8.3 refined Wave 2 manifest.");
            return failures;
        }

        static void BuildPage(ScreenId id)
        {
            GameObject root = new GameObject("Wave02_" + id, typeof(RectTransform)); root.layer = 5;
            RectTransform rt = root.GetComponent<RectTransform>(); Stretch(rt);
            ProductionCoreLoopPageMarker marker = root.AddComponent<ProductionCoreLoopPageMarker>(); marker.Configure(id);
            switch (id)
            {
                case ScreenId.Characters: BuildCharacters(rt); break;
                case ScreenId.DailyReward: BuildDailyReward(rt); break;
                case ScreenId.Profile: BuildProfile(rt); break;
                case ScreenId.Inventory: BuildInventory(rt); break;
                case ScreenId.Shop: BuildShop(rt); break;
                case ScreenId.SpecialOffer: BuildSpecialOffer(rt); break;
                case ScreenId.OutOfEnergy: BuildOutOfEnergy(rt); break;
                case ScreenId.BoosterSelect: BuildBoosterSelect(rt); break;
                case ScreenId.RewardClaim: BuildRewardClaim(rt); break;
            }
            PrefabUtility.SaveAsPrefabAsset(root, CandidateRoot + "/" + id + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
        }

        static void Base(RectTransform r, string folder, string title)
        {
            Page(r, folder, "background_plate.png", 0, 0, 941, 1672);
            Shared(r, "Panels/top_bar.png", 0, 0, 941, 150);
            Shared(r, "Icons/icon_back.png", 24, 45, 55, 55);
            Shared(r, "Icons/icon_coin.png", 385, 45, 50, 50);
            Shared(r, "Icons/icon_gem.png", 610, 45, 50, 50);
            Shared(r, "Icons/icon_energy.png", 800, 45, 50, 50);
            HudT(r, "12 540", 430, 45, 120, 50, 18);
            HudT(r, "350", 660, 45, 90, 50, 18);
            HudT(r, "25/30", 850, 45, 80, 50, 16);
            Shared(r, "Panels/header_frame.png", 115, 155, 711, 155);
            DisplayT(r, title, 170, 185, 601, 90, 32);
        }

        static void BuildCharacters(RectTransform r)
        {
            Page(r, "CharacterCollection", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "CharacterCollection", "top_hud_shell.png", 0, 0, 941, 145);
            Refined(r, "CharacterCollection", "header_tabs_shell.png", 0, 145, 941, 190);
            Refined(r, "CharacterCollection", "hero_detail_shell.png", 0, 335, 941, 665);
            Refined(r, "CharacterCollection", "gallery_shell.png", 0, 1000, 941, 490);
            Refined(r, "CharacterCollection", "bottom_navigation_shell.png", 0, 1490, 941, 182);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "ПЕРСОНАЖИ", 220, 170, 501, 70, 31);
            HeadingT(r, "ИВАН", 555, 390, 280, 50, 24); HeadingT(r, "ЛЕГЕНДАРНЫЙ", 555, 438, 280, 36, 14);
            DisplayT(r, "ВЫБРАТЬ", 555, 872, 280, 62, 18);
        }

        static void BuildDailyReward(RectTransform r)
        {
            Page(r, "DailyReward", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "DailyReward", "top_hud_shell.png", 0, 0, 941, 145);
            Refined(r, "DailyReward", "header_intro_shell.png", 0, 145, 941, 205);
            Refined(r, "DailyReward", "daily_grid_shell.png", 0, 350, 941, 620);
            Refined(r, "DailyReward", "monthly_progress_shell.png", 0, 970, 941, 360);
            Refined(r, "DailyReward", "claim_footer_shell.png", 0, 1330, 941, 342);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "ЕЖЕДНЕВНАЯ НАГРАДА", 145, 170, 651, 70, 28);
            BodyLightT(r, "Заходите каждый день и получайте ценные награды!", 155, 295, 631, 42, 16);
            DisplayT(r, "ПОЛУЧИТЬ", 245, 1368, 451, 70, 22);
            HudT(r, "Следующая награда через 18:42:16", 180, 1515, 581, 42, 16);
        }

        static void BuildProfile(RectTransform r)
        {
            Page(r, "Profile", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "Profile", "top_hud_shell.png", 0, 0, 941, 145);
            Refined(r, "Profile", "profile_header_shell.png", 0, 145, 941, 185);
            Refined(r, "Profile", "stat_panels_shell.png", 0, 330, 941, 680);
            Refined(r, "Profile", "progress_chart_shell.png", 0, 1010, 941, 480);
            Refined(r, "Profile", "bottom_navigation_shell.png", 0, 1490, 941, 182);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "ЛИЧНАЯ СТАТИСТИКА", 170, 170, 601, 70, 29);
            DisplayT(r, "ТРОФЕИ\n1 248", 345, 380, 230, 120, 24); HeadingT(r, "УРОВЕНЬ 28", 560, 390, 260, 55, 18);
            HeadingT(r, "ВАШ ПРОГРЕСС", 270, 1030, 401, 45, 20);
            DisplayT(r, "ИЗМЕНИТЬ ИМЯ", 260, 1500, 421, 65, 18);
        }

        static void BuildInventory(RectTransform r)
        {
            Base(r, "Inventory", "ИНВЕНТАРЬ");
            Tab(r, "БУСТЕРЫ", 110, 330, true); Tab(r, "ПРЕДМЕТЫ", 360, 330, false); Tab(r, "РЕДКОСТИ", 610, 330, false);
            Shared(r, "Panels/panel_navy.png", 70, 435, 801, 880);
            Refined(r, "Inventory", "inventory_items_grid.png", 95, 470, 751, 750);
            Shared(r, "Panels/panel_parchment.png", 170, 1340, 601, 150);
            BodyDarkT(r, "Выберите предмет, чтобы увидеть описание.", 200, 1365, 541, 90, 18);
        }

        static void BuildShop(RectTransform r)
        {
            Page(r, "Shop", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "Shop", "top_hud_shell.png", 0, 0, 941, 145);
            Refined(r, "Shop", "shop_title_shell.png", 0, 145, 941, 150);
            Refined(r, "Shop", "shop_tabs_shell.png", 0, 295, 941, 95);
            Refined(r, "Shop", "product_coin_shell.png", 0, 390, 471, 475);
            Refined(r, "Shop", "product_chest_shell.png", 471, 390, 470, 475);
            Refined(r, "Shop", "product_cart_shell.png", 0, 865, 471, 500);
            Refined(r, "Shop", "product_magic_shell.png", 471, 865, 470, 500);
            Refined(r, "Shop", "support_strip.png", 0, 1365, 941, 75);
            Refined(r, "Shop", "bottom_navigation_shell.png", 0, 1440, 941, 232);

            HudT(r, "Иван", 165, 27, 110, 30, 18); HudT(r, "Уровень 28", 165, 60, 120, 28, 14);
            HudT(r, "12 540", 395, 36, 100, 40, 19); HudT(r, "350", 585, 36, 75, 40, 19); HudT(r, "25/30", 742, 36, 105, 40, 18);
            DisplayT(r, "МАГАЗИН", 250, 180, 441, 75, 40);
            HeadingT(r, "МОНЕТЫ", 35, 315, 190, 45, 18); HeadingT(r, "САМОЦВЕТЫ", 255, 315, 220, 45, 17);
            HeadingT(r, "НАБОРЫ", 500, 315, 195, 45, 18); HeadingT(r, "БУСТЕРЫ", 720, 315, 190, 45, 18);

            ShopProductText(r, 0, "Мешок монет", "500", "99 ₽");
            ShopProductText(r, 1, "Сундук богатыря", "2 500", "399 ₽");
            ShopProductText(r, 2, "Золотая телега", "10 000", "999 ₽");
            ShopProductText(r, 3, "Набор мага", "500   50   5", "599 ₽");

            BodyLightT(r, "Спасибо, что поддерживаете Легенды Руси!", 170, 1380, 601, 40, 16);
            HeadingT(r, "ПРЕДЛОЖЕНИЯ", 10, 1570, 170, 42, 13); HeadingT(r, "САМОЦВЕТЫ", 195, 1570, 170, 42, 13);
            HeadingT(r, "НАБОРЫ", 385, 1570, 170, 42, 13); HeadingT(r, "БУСТЕРЫ", 570, 1570, 170, 42, 13); HeadingT(r, "VIP", 760, 1570, 165, 42, 13);
        }

        static void ShopProductText(RectTransform r, int index, string title, string quantity, string price)
        {
            bool right = (index % 2) == 1; bool bottom = index >= 2;
            float x = right ? 500 : 65; float y = bottom ? 892 : 405;
            HeadingT(r, title, x, y, 376, 46, 18);
            HudT(r, quantity, x + 55, y + 310, 266, 46, 20);
            DisplayT(r, price, x + 45, y + 390, 286, 55, 28);
        }

        static void BuildSpecialOffer(RectTransform r)
        {
            Base(r, "SpecialOffer", "СПЕЦПРЕДЛОЖЕНИЕ");
            Shared(r, "Panels/modal_frame.png", 100, 350, 741, 1000);
            Refined(r, "SpecialOffer", "offer_hero_bundle.png", 140, 430, 661, 470);
            Refined(r, "SpecialOffer", "discount_badge.png", 610, 390, 180, 90); DisplayT(r, "-60%", 620, 405, 160, 55, 24);
            DisplayT(r, "НАБОР БОГАТЫРЯ", 210, 920, 521, 65, 28);
            Shared(r, "Icons/icon_coin.png", 250, 1010, 80, 80); Shared(r, "Icons/icon_gem.png", 430, 1010, 80, 80); Shared(r, "Icons/icon_chest.png", 610, 1010, 80, 80);
            HudT(r, "5 000", 225, 1090, 130, 40, 18); HudT(r, "250", 405, 1090, 130, 40, 18); HudT(r, "×3", 585, 1090, 130, 40, 18);
            HeadingT(r, "599 ₽   249 ₽", 270, 1170, 401, 60, 24); Button(r, "КУПИТЬ", 220, 1250, 501, 90, true);
            HudT(r, "02:48:16", 335, 1380, 271, 55, 20);
        }

        static void BuildOutOfEnergy(RectTransform r)
        {
            Page(r, "OutOfEnergy", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "OutOfEnergy", "context_top_shell.png", 0, 0, 941, 350);
            Refined(r, "OutOfEnergy", "modal_upper_shell.png", 0, 350, 941, 520);
            Refined(r, "OutOfEnergy", "modal_actions_shell.png", 0, 870, 941, 470);
            Refined(r, "OutOfEnergy", "context_bottom_shell.png", 0, 1340, 941, 332);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "НЕТ ЭНЕРГИИ", 250, 405, 441, 65, 28);
            BodyLightT(r, "Ваша энергия на исходе. Подождите или восстановите её.", 230, 790, 481, 75, 16);
            DisplayT(r, "05:37", 350, 855, 241, 55, 25);
            DisplayT(r, "КУПИТЬ ЭНЕРГИЮ", 175, 958, 591, 55, 17); HeadingT(r, "99 ₽", 650, 958, 100, 55, 18);
            HeadingT(r, "ПОПРОСИТЬ У ДРУЗЕЙ", 175, 1063, 591, 55, 16);
            HeadingT(r, "СМОТРЕТЬ РЕКЛАМУ", 175, 1168, 591, 55, 16);
        }

        static void BuildBoosterSelect(RectTransform r)
        {
            Page(r, "BoosterSelect", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "BoosterSelect", "context_top_shell.png", 0, 0, 941, 290);
            Refined(r, "BoosterSelect", "modal_header_shell.png", 0, 290, 941, 260);
            Refined(r, "BoosterSelect", "booster_grid_shell.png", 0, 550, 941, 640);
            Refined(r, "BoosterSelect", "modal_footer_shell.png", 0, 1190, 941, 482);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "ВЫБОР БУСТЕРОВ", 220, 320, 501, 70, 28);
            BodyLightT(r, "Выберите бустеры для этого уровня", 190, 405, 561, 48, 16);
            DisplayT(r, "ИГРАТЬ", 220, 1270, 501, 80, 28);
        }

        static void BuildRewardClaim(RectTransform r)
        {
            Page(r, "RewardClaim", "background_plate.png", 0, 0, 941, 1672);
            Refined(r, "RewardClaim", "context_top_shell.png", 0, 0, 941, 340);
            Refined(r, "RewardClaim", "reward_modal_upper_shell.png", 0, 340, 941, 680);
            Refined(r, "RewardClaim", "reward_items_shell.png", 0, 1020, 941, 345);
            Refined(r, "RewardClaim", "context_bottom_shell.png", 0, 1365, 941, 307);
            HudT(r, "Иван", 160, 24, 155, 34, 18); HudT(r, "Уровень 28", 160, 57, 165, 30, 14);
            DisplayT(r, "ПОЛУЧЕНА НАГРАДА", 215, 420, 511, 70, 28);
            DisplayT(r, "ОТЛИЧНАЯ РАБОТА!", 250, 885, 441, 55, 20);
            DisplayT(r, "ЗАБРАТЬ", 220, 1240, 501, 80, 27);
        }

        static void Tab(RectTransform r, string label, float x, float y, bool selected)
        {
            Shared(r, selected ? "Controls/tab_selected.png" : "Controls/tab_idle.png", x, y, 220, 70);
            HeadingT(r, label, x + 10, y + 10, 200, 50, 16);
        }

        static void Button(RectTransform r, string label, float x, float y, float w, float h, bool primary)
        {
            Shared(r, "Buttons/" + (primary ? "btn_primary_idle.png" : "btn_secondary_idle.png"), x, y, w, h);
            DisplayT(r, label, x + 20, y + 10, w - 40, h - 20, w > 500 ? 22 : 20);
        }

        static void Page(RectTransform r, string folder, string file, float x, float y, float w, float h)
        { Img(r, AssetRoot + "/" + folder + "/" + file, x, y, w, h); }
        static void Refined(RectTransform r, string folder, string file, float x, float y, float w, float h)
        { Img(r, RefinedRoot + "/" + folder + "/" + file, x, y, w, h); }
        static void Shared(RectTransform r, string rel, float x, float y, float w, float h)
        { Img(r, Wave0Root + "/" + rel, x, y, w, h); }

        static void Img(RectTransform r, string path, float x, float y, float w, float h)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) throw new InvalidOperationException("Missing Sprite: " + path);
            GameObject go = new GameObject(Path.GetFileNameWithoutExtension(path), typeof(RectTransform), typeof(Image));
            go.layer = 5; go.transform.SetParent(r, false); Set(go.GetComponent<RectTransform>(), x, y, w, h);
            Image image = go.GetComponent<Image>(); image.sprite = sprite; image.type = Image.Type.Simple; image.preserveAspect = false; image.raycastTarget = false;
        }

        static void DisplayT(RectTransform r, string v, float x, float y, float w, float h, int s)
        { TextImpl(r, v, x, y, w, h, s, new Color32(244, 205, 119, 255), new Color32(64, 28, 9, 245), true); }
        static void HeadingT(RectTransform r, string v, float x, float y, float w, float h, int s)
        { TextImpl(r, v, x, y, w, h, s, new Color32(237, 203, 132, 255), new Color32(50, 25, 10, 235), true); }
        static void HudT(RectTransform r, string v, float x, float y, float w, float h, int s)
        { TextImpl(r, v, x, y, w, h, s, new Color32(244, 224, 180, 255), new Color32(15, 18, 23, 230), false); }
        static void BodyLightT(RectTransform r, string v, float x, float y, float w, float h, int s)
        { TextImpl(r, v, x, y, w, h, s, new Color32(239, 218, 176, 255), new Color32(20, 20, 20, 210), false); }
        static void BodyDarkT(RectTransform r, string v, float x, float y, float w, float h, int s)
        { TextImpl(r, v, x, y, w, h, s, new Color32(73, 45, 27, 255), new Color32(225, 194, 133, 0), false); }

        static void TextImpl(RectTransform r, string value, float x, float y, float w, float h, int size, Color color, Color outlineColor, bool strong)
        {
            GameObject go = new GameObject("LiveText", typeof(RectTransform), typeof(Text)); go.layer = 5; go.transform.SetParent(r, false); Set(go.GetComponent<RectTransform>(), x, y, w, h);
            Text t = go.GetComponent<Text>(); t.text = value; t.font = ResolveFont(); t.fontSize = size; t.fontStyle = strong ? FontStyle.Bold : FontStyle.Normal; t.alignment = TextAnchor.MiddleCenter; t.color = color; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow; t.raycastTarget = false;
            if (outlineColor.a > 0.01f)
            {
                Outline o = go.AddComponent<Outline>(); o.effectColor = outlineColor; o.effectDistance = strong ? new Vector2(2f, -2f) : new Vector2(1f, -1f); o.useGraphicAlpha = true;
                Shadow sh = go.AddComponent<Shadow>(); sh.effectColor = new Color(0f, 0f, 0f, strong ? 0.55f : 0.35f); sh.effectDistance = new Vector2(1f, -2f); sh.useGraphicAlpha = true;
            }
        }

        static Font ResolveFont()
        {
            if (cachedFont != null) return cachedFont;
            if (AssetDatabase.IsValidFolder(ProductionFontRoot))
            {
                string[] guids = AssetDatabase.FindAssets("t:Font", new[] { ProductionFontRoot });
                if (guids != null && guids.Length > 0)
                {
                    Array.Sort(guids, StringComparer.Ordinal);
                    cachedFont = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
            }
            if (cachedFont == null) cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (cachedFont == null) cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedFont;
        }

        static void Set(RectTransform rt, float x, float y, float w, float h)
        { rt.anchorMin = new Vector2(x / W, 1f - (y + h) / H); rt.anchorMax = new Vector2((x + w) / W, 1f - y / H); rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }
        static void Stretch(RectTransform rt)
        { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }

        static void ConfigureAllPngs(string root)
        {
            string abs = Path.Combine(Directory.GetCurrentDirectory(), root.Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(abs)) return;
            foreach (string f in Directory.GetFiles(abs, "*.png", SearchOption.AllDirectories))
            {
                string asset = f.Replace('\\', '/'); string prefix = Directory.GetCurrentDirectory().Replace('\\', '/').TrimEnd('/') + "/";
                if (asset.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) asset = asset.Substring(prefix.Length);
                TextureImporter ti = AssetImporter.GetAtPath(asset) as TextureImporter; if (ti == null) continue; bool dirty = false;
                if (ti.textureType != TextureImporterType.Sprite) { ti.textureType = TextureImporterType.Sprite; dirty = true; }
                if (ti.spriteImportMode != SpriteImportMode.Single) { ti.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (!ti.alphaIsTransparency) { ti.alphaIsTransparency = true; dirty = true; }
                if (ti.isReadable) { ti.isReadable = false; dirty = true; }
                if (ti.mipmapEnabled) { ti.mipmapEnabled = false; dirty = true; }
                if (ti.wrapMode != TextureWrapMode.Clamp) { ti.wrapMode = TextureWrapMode.Clamp; dirty = true; }
                if (dirty) ti.SaveAndReimport();
            }
        }

        static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/'); string cur = parts[0];
            for (int i = 1; i < parts.Length; i++) { string next = cur + "/" + parts[i]; if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]); cur = next; }
        }
    }
}
#endif
