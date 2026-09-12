#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    public static class Step20CanonicalUiKitBuilder
    {
        public const string KitRoot = "Assets/LegendsOfRus/Art/Production/UI/CanonicalKit";
        public const string PrefabRoot = "Assets/LegendsOfRus/Prefabs/UI/Kit";
        public const string StyleVersion = "v1.20-step03.2";

        sealed class SpriteSpec
        {
            public readonly string File;
            public readonly Vector4 Border;
            public readonly bool Sliced;

            public SpriteSpec(string file, Vector4 border, bool sliced)
            {
                File = file;
                Border = border;
                Sliced = sliced;
            }
        }

        static readonly SpriteSpec[] SpriteSpecs =
        {
            S("ui_btn_primary.png", 72, 58, 72, 58),
            S("ui_btn_secondary.png", 72, 58, 72, 58),
            S("ui_btn_danger.png", 72, 58, 72, 58),
            S("ui_panel_navy.png", 86, 86, 86, 86),
            S("ui_panel_parchment.png", 86, 86, 86, 86),
            S("ui_modal_frame.png", 96, 96, 96, 96),
            S("ui_header_frame.png", 90, 64, 90, 64),
            S("ui_top_bar.png", 52, 42, 52, 42),
            S("ui_tab_idle.png", 54, 42, 54, 42),
            S("ui_tab_selected.png", 54, 42, 54, 42),
            S("ui_currency_pill.png", 54, 36, 54, 36),
            S("ui_nav_tile.png", 60, 60, 60, 60),
            S("ui_reward_tile.png", 58, 58, 58, 58),
            S("ui_booster_slot.png", 52, 52, 52, 52),
            S("ui_ranking_row.png", 48, 38, 48, 38),
            S("ui_system_state_panel.png", 88, 88, 88, 88),
            S("ui_goal_panel.png", 58, 52, 58, 52),
            S("ui_board_frame.png", 72, 72, 72, 72),
            S("ui_progress_track.png", 38, 28, 38, 28),
            S("ui_progress_fill.png", 38, 28, 38, 28),
            S("ui_toggle_track.png", 52, 28, 52, 28),
            U("ui_modal_blocker.png"), U("ui_separator.png"),
            U("ui_jewel_sapphire.png"), U("ui_jewel_round.png"),
            U("ui_icon_button.png"), U("ui_pause_button.png"),
            U("ui_back_button.png"), U("ui_close_button.png"),
            U("ui_notification_badge.png"), U("ui_star.png"),
            U("ui_toggle_knob.png"), U("ui_map_node.png"),
            U("ui_corner_flourish.png")
        };

        static SpriteSpec S(string file, float left, float bottom, float right, float top)
        {
            return new SpriteSpec(file, new Vector4(left, bottom, right, top), true);
        }

        static SpriteSpec U(string file)
        {
            return new SpriteSpec(file, Vector4.zero, false);
        }

        [MenuItem("Legends of Rus/Production UI/Build v1.20 Step 3.2 Canonical UI Kit")]
        public static void BuildMenu()
        {
            BuildAll(true);
        }

        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 3.2 Canonical UI Kit")]
        public static void ValidateMenu()
        {
            var failures = ValidateAll();
            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.2 canonical UI kit validation passed.");
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus", "Canonical UI kit validation passed.", "OK");
                return;
            }

            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.2 canonical UI kit validation failed:\n" + report);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", report, "OK");
        }

        public static void BuildAll(bool logResult)
        {
            EnsureFolder("Assets/LegendsOfRus/Prefabs");
            EnsureFolder("Assets/LegendsOfRus/Prefabs/UI");
            EnsureFolder(PrefabRoot);

            ConfigureSpriteImports();

            BuildButton("PrimaryButton", "ui_btn_primary.png", CanonicalUiComponentRole.PrimaryButton);
            BuildButton("SecondaryButton", "ui_btn_secondary.png", CanonicalUiComponentRole.SecondaryButton);
            BuildButton("DangerButton", "ui_btn_danger.png", CanonicalUiComponentRole.DangerButton);
            BuildImagePrefab("NavyPanel", "ui_panel_navy.png", CanonicalUiComponentRole.NavyPanel, true, new Vector2(720, 720));
            BuildImagePrefab("ParchmentPanel", "ui_panel_parchment.png", CanonicalUiComponentRole.ParchmentPanel, true, new Vector2(720, 720));
            BuildImagePrefab("ModalFrame", "ui_modal_frame.png", CanonicalUiComponentRole.ModalFrame, true, new Vector2(760, 980));
            BuildImagePrefab("HeaderFrame", "ui_header_frame.png", CanonicalUiComponentRole.HeaderFrame, true, new Vector2(860, 218));
            BuildTopBar();
            BuildTabBar();
            BuildCurrencyBar();
            BuildBottomNavigation();
            BuildProgressBar();
            BuildImagePrefab("RewardTile", "ui_reward_tile.png", CanonicalUiComponentRole.RewardTile, true, new Vector2(250, 285));
            BuildImagePrefab("BoosterSlot", "ui_booster_slot.png", CanonicalUiComponentRole.BoosterSlot, true, new Vector2(180, 180));
            BuildRankingRow();
            BuildIconButton();
            BuildNotificationBadge();
            BuildToggle();
            BuildImagePrefab("GoalPanel", "ui_goal_panel.png", CanonicalUiComponentRole.GoalPanel, true, new Vector2(560, 190));
            BuildImagePrefab("BoardFrame", "ui_board_frame.png", CanonicalUiComponentRole.BoardFrame, true, new Vector2(820, 820));
            BuildImagePrefab("MapNode", "ui_map_node.png", CanonicalUiComponentRole.MapNode, false, new Vector2(150, 150));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            List<string> failures = ValidateAll();
            if (failures.Count > 0)
                throw new InvalidOperationException("Canonical UI kit build produced invalid output:\n" + string.Join("\n", failures.ToArray()));

            if (logResult)
                Debug.Log("v1.20 Step 3.2 canonical UI kit built: 34 sprites, 21 reusable prefabs. Screenshot cutover remains disabled.");
        }

        public static List<string> ValidateAll()
        {
            var failures = new List<string>();
            foreach (SpriteSpec spec in SpriteSpecs)
            {
                string path = KitRoot + "/" + spec.File;
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null)
                {
                    failures.Add("Missing production UI sprite: " + path);
                    continue;
                }
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    failures.Add("Missing TextureImporter: " + path);
                    continue;
                }
                if (importer.textureType != TextureImporterType.Sprite)
                    failures.Add(path + ": textureType must be Sprite.");
                if (importer.isReadable)
                    failures.Add(path + ": Read/Write must remain disabled.");
                if (importer.mipmapEnabled)
                    failures.Add(path + ": mipmaps must remain disabled.");
                if (spec.Sliced && importer.spriteBorder != spec.Border)
                    failures.Add(path + ": 9-slice border mismatch.");
            }

            string[] required =
            {
                "PrimaryButton", "SecondaryButton", "DangerButton", "NavyPanel",
                "ParchmentPanel", "ModalFrame", "HeaderFrame", "TopBar", "TabBar",
                "CurrencyBar", "BottomNavigation", "ProgressBar", "RewardTile",
                "BoosterSlot", "RankingRow", "IconButton", "NotificationBadge",
                "Toggle", "GoalPanel", "BoardFrame", "MapNode"
            };

            foreach (string name in required)
            {
                string path = PrefabRoot + "/" + name + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    failures.Add("Missing canonical UI prefab: " + path);
                    continue;
                }
                CanonicalUiKitMarker marker = prefab.GetComponent<CanonicalUiKitMarker>();
                if (marker == null || marker.StyleVersion != StyleVersion)
                    failures.Add(path + ": missing or stale CanonicalUiKitMarker.");
            }
            return failures;
        }

        static void ConfigureSpriteImports()
        {
            foreach (SpriteSpec spec in SpriteSpecs)
            {
                string path = KitRoot + "/" + spec.File;
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    throw new InvalidOperationException("Texture importer missing: " + path);

                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (importer.alphaIsTransparency != true) { importer.alphaIsTransparency = true; dirty = true; }
                if (importer.isReadable) { importer.isReadable = false; dirty = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; dirty = true; }
                if (importer.wrapMode != TextureWrapMode.Clamp) { importer.wrapMode = TextureWrapMode.Clamp; dirty = true; }
                if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; dirty = true; }
                if (importer.spritePixelsPerUnit != 100f) { importer.spritePixelsPerUnit = 100f; dirty = true; }
                if (importer.spriteBorder != spec.Border) { importer.spriteBorder = spec.Border; dirty = true; }

                TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
                if (!android.overridden || android.maxTextureSize != 2048 || android.format != TextureImporterFormat.ASTC_6x6)
                {
                    android.overridden = true;
                    android.maxTextureSize = 2048;
                    android.format = TextureImporterFormat.ASTC_6x6;
                    android.compressionQuality = 70;
                    importer.SetPlatformTextureSettings(android);
                    dirty = true;
                }
                if (dirty) importer.SaveAndReimport();
            }
        }

        static void BuildButton(string name, string spriteFile, CanonicalUiComponentRole role)
        {
            GameObject root = CreateRoot(name, new Vector2(520, 132));
            Image image = root.AddComponent<Image>();
            image.sprite = LoadSprite(spriteFile);
            image.type = Image.Type.Sliced;
            image.raycastTarget = true;
            Button button = root.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.75f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            AddMarker(root, role);
            Text label = AddText(root.transform, "Label", "КНОПКА", 44, TextAnchor.MiddleCenter);
            Stretch(label.rectTransform, 56, 26, 56, 24);
            label.color = new Color32(255, 222, 146, 255);
            Save(root, name);
        }

        static void BuildImagePrefab(string name, string spriteFile, CanonicalUiComponentRole role, bool sliced, Vector2 size)
        {
            GameObject root = CreateRoot(name, size);
            Image image = root.AddComponent<Image>();
            image.sprite = LoadSprite(spriteFile);
            image.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            image.raycastTarget = false;
            AddMarker(root, role);
            Save(root, name);
        }

        static void BuildTopBar()
        {
            GameObject root = CreateRoot("TopBar", new Vector2(941, 150));
            Image image = root.AddComponent<Image>();
            image.sprite = LoadSprite("ui_top_bar.png");
            image.type = Image.Type.Sliced;
            image.raycastTarget = false;
            AddMarker(root, CanonicalUiComponentRole.TopBar);

            GameObject profile = ChildRect(root.transform, "ProfileAnchor", new Vector2(105, 105), new Vector2(18, -20), new Vector2(0, 1));
            Image profileImage = profile.AddComponent<Image>();
            profileImage.sprite = LoadSprite("ui_jewel_round.png");
            profileImage.raycastTarget = false;

            float x = 360f;
            for (int i = 0; i < 3; i++)
            {
                GameObject pill = ChildRect(root.transform, "Currency_" + (i + 1), new Vector2(165, 74), new Vector2(x + i * 182f, -38), new Vector2(0, 1));
                Image pimg = pill.AddComponent<Image>();
                pimg.sprite = LoadSprite("ui_currency_pill.png");
                pimg.type = Image.Type.Sliced;
                pimg.raycastTarget = false;
                Text value = AddText(pill.transform, "Value", "0", 28, TextAnchor.MiddleCenter);
                Stretch(value.rectTransform, 42, 8, 20, 8);
                value.color = new Color32(248, 224, 169, 255);
            }
            Save(root, "TopBar");
        }

        static void BuildTabBar()
        {
            GameObject root = CreateRoot("TabBar", new Vector2(760, 108));
            AddMarker(root, CanonicalUiComponentRole.TabBar);
            HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = true;
            for (int i = 0; i < 3; i++)
            {
                GameObject tab = new GameObject("Tab_" + (i + 1), typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
                tab.transform.SetParent(root.transform, false);
                Image img = tab.GetComponent<Image>();
                img.sprite = LoadSprite(i == 0 ? "ui_tab_selected.png" : "ui_tab_idle.png");
                img.type = Image.Type.Sliced;
                Text text = AddText(tab.transform, "Label", "ВКЛАДКА", 24, TextAnchor.MiddleCenter);
                Stretch(text.rectTransform, 18, 8, 18, 8);
                text.color = new Color32(248, 224, 169, 255);
            }
            Save(root, "TabBar");
        }

        static void BuildCurrencyBar()
        {
            GameObject root = CreateRoot("CurrencyBar", new Vector2(220, 82));
            Image img = root.AddComponent<Image>();
            img.sprite = LoadSprite("ui_currency_pill.png");
            img.type = Image.Type.Sliced;
            AddMarker(root, CanonicalUiComponentRole.CurrencyBar);
            GameObject icon = ChildRect(root.transform, "Icon", new Vector2(54, 54), new Vector2(10, -14), new Vector2(0, 1));
            Image ii = icon.AddComponent<Image>(); ii.sprite = LoadSprite("ui_jewel_round.png"); ii.raycastTarget = false;
            Text value = AddText(root.transform, "Value", "0", 30, TextAnchor.MiddleCenter);
            Stretch(value.rectTransform, 70, 8, 24, 8); value.color = new Color32(248, 224, 169, 255);
            Save(root, "CurrencyBar");
        }

        static void BuildBottomNavigation()
        {
            GameObject root = CreateRoot("BottomNavigation", new Vector2(900, 220));
            AddMarker(root, CanonicalUiComponentRole.BottomNavigation);
            HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12; layout.padding = new RectOffset(8, 8, 8, 8); layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true; layout.childForceExpandWidth = true; layout.childControlHeight = true; layout.childForceExpandHeight = true;
            for (int i = 0; i < 4; i++)
            {
                GameObject tile = new GameObject("Nav_" + (i + 1), typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
                tile.transform.SetParent(root.transform, false);
                Image img = tile.GetComponent<Image>(); img.sprite = LoadSprite("ui_nav_tile.png"); img.type = Image.Type.Sliced;
                Text label = AddText(tile.transform, "Label", "РАЗДЕЛ", 20, TextAnchor.LowerCenter);
                Stretch(label.rectTransform, 16, 16, 16, 14); label.color = new Color32(248, 224, 169, 255);
            }
            Save(root, "BottomNavigation");
        }

        static void BuildProgressBar()
        {
            GameObject root = CreateRoot("ProgressBar", new Vector2(560, 64));
            Image track = root.AddComponent<Image>(); track.sprite = LoadSprite("ui_progress_track.png"); track.type = Image.Type.Sliced; track.raycastTarget = false;
            AddMarker(root, CanonicalUiComponentRole.ProgressBar);
            GameObject fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image)); fillGo.transform.SetParent(root.transform, false);
            Stretch(fillGo.GetComponent<RectTransform>(), 8, 8, 8, 8);
            Image fill = fillGo.GetComponent<Image>(); fill.sprite = LoadSprite("ui_progress_fill.png"); fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillAmount = 0.66f; fill.raycastTarget = false;
            Save(root, "ProgressBar");
        }

        static void BuildRankingRow()
        {
            GameObject root = CreateRoot("RankingRow", new Vector2(850, 150));
            Image img = root.AddComponent<Image>(); img.sprite = LoadSprite("ui_ranking_row.png"); img.type = Image.Type.Sliced;
            AddMarker(root, CanonicalUiComponentRole.RankingRow);
            Text rank = AddText(root.transform, "Rank", "1", 30, TextAnchor.MiddleCenter); SetAnchored(rank.rectTransform, new Vector2(90, 110), new Vector2(18, -20), new Vector2(0,1)); rank.color = new Color32(255,220,128,255);
            Text name = AddText(root.transform, "Name", "ИГРОК", 28, TextAnchor.MiddleLeft); SetAnchored(name.rectTransform, new Vector2(430, 110), new Vector2(120, -20), new Vector2(0,1)); name.color = new Color32(248,224,169,255);
            Text score = AddText(root.transform, "Score", "0", 30, TextAnchor.MiddleRight); SetAnchored(score.rectTransform, new Vector2(180, 110), new Vector2(640, -20), new Vector2(0,1)); score.color = new Color32(80,190,255,255);
            Save(root, "RankingRow");
        }

        static void BuildIconButton()
        {
            GameObject root = CreateRoot("IconButton", new Vector2(112, 112));
            Image img = root.AddComponent<Image>(); img.sprite = LoadSprite("ui_icon_button.png"); img.type = Image.Type.Simple;
            root.AddComponent<Button>(); AddMarker(root, CanonicalUiComponentRole.IconButton); Save(root, "IconButton");
        }

        static void BuildNotificationBadge()
        {
            GameObject root = CreateRoot("NotificationBadge", new Vector2(64, 64));
            Image img = root.AddComponent<Image>(); img.sprite = LoadSprite("ui_notification_badge.png"); img.raycastTarget = false;
            AddMarker(root, CanonicalUiComponentRole.NotificationBadge);
            Text value = AddText(root.transform, "Value", "1", 26, TextAnchor.MiddleCenter); Stretch(value.rectTransform, 8, 8, 8, 8); value.color = Color.white;
            Save(root, "NotificationBadge");
        }

        static void BuildToggle()
        {
            GameObject root = CreateRoot("Toggle", new Vector2(170, 64));
            Image track = root.AddComponent<Image>(); track.sprite = LoadSprite("ui_toggle_track.png"); track.type = Image.Type.Sliced;
            Toggle toggle = root.AddComponent<Toggle>(); AddMarker(root, CanonicalUiComponentRole.Toggle);
            GameObject knob = ChildRect(root.transform, "Knob", new Vector2(56,56), new Vector2(106,-4), new Vector2(0,1));
            Image k = knob.AddComponent<Image>(); k.sprite = LoadSprite("ui_toggle_knob.png"); k.raycastTarget = false;
            toggle.graphic = k; toggle.targetGraphic = track; toggle.isOn = true;
            Save(root, "Toggle");
        }

        static Sprite LoadSprite(string file)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(KitRoot + "/" + file);
            if (sprite == null) throw new InvalidOperationException("Sprite missing: " + file);
            return sprite;
        }

        static GameObject CreateRoot(string name, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.GetComponent<RectTransform>().sizeDelta = size;
            return go;
        }

        static void AddMarker(GameObject go, CanonicalUiComponentRole role)
        {
            CanonicalUiKitMarker marker = go.AddComponent<CanonicalUiKitMarker>();
            marker.Configure(role);
        }

        static Text AddText(Transform parent, string name, string value, int size, TextAnchor alignment)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null) text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Math.Max(12, size / 2);
            text.resizeTextMaxSize = size;
            return text;
        }

        static GameObject ChildRect(Transform parent, string name, Vector2 size, Vector2 pos, Vector2 anchor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            SetAnchored(go.GetComponent<RectTransform>(), size, pos, anchor);
            return go;
        }

        static void SetAnchored(RectTransform rt, Vector2 size, Vector2 pos, Vector2 anchor)
        {
            rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = new Vector2(0,1); rt.sizeDelta = size; rt.anchoredPosition = pos;
        }

        static void Stretch(RectTransform rt, float left, float bottom, float right, float top)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom); rt.offsetMax = new Vector2(-right, -top);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\','/');
            string name = Path.GetFileName(path);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        static void Save(GameObject root, string name)
        {
            string path = PrefabRoot + "/" + name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
#endif
