#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20CanonicalUiKitSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 3.2 Self-Tests")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            Check(ref total, ref passed, failures, Directory.Exists(Step20CanonicalUiKitBuilder.KitRoot), "production UI kit root exists");
            string[] pngs = Directory.GetFiles(Step20CanonicalUiKitBuilder.KitRoot, "*.png", SearchOption.TopDirectoryOnly);
            Check(ref total, ref passed, failures, pngs.Length == 34, "34 individual production UI PNGs exist");
            Check(ref total, ref passed, failures, File.Exists(Step20CanonicalUiKitBuilder.KitRoot + "/canonical_ui_kit_manifest.json"), "kit manifest exists");
            Check(ref total, ref passed, failures, File.Exists("Assets/LegendsOfRus/Resources/Config/production_ui_kit_v120_step03_2.json"), "runtime-neutral kit config exists");
            Check(ref total, ref passed, failures, Directory.GetFiles("Assets/LegendsOfRus/Resources/DesignScreens", "*.png").Length == 22, "22 screenshot placeholders retained until page cutover");

            string config = File.ReadAllText("Assets/LegendsOfRus/Resources/Config/production_ui_kit_v120_step03_2.json");
            Check(ref total, ref passed, failures, config.Contains("\"screenshotRuntimeCutoverAuthorized\": false"), "runtime screenshot cutover remains disabled");
            Check(ref total, ref passed, failures, config.Contains("\"screenshotDeletionAuthorized\": false"), "screenshot deletion remains disabled");
            Check(ref total, ref passed, failures, config.Contains("\"safeAreaRequired\": true"), "safe-area requirement pinned");
            Check(ref total, ref passed, failures, config.Contains("\"width\": 941") && config.Contains("\"height\": 1672"), "approved reference resolution pinned");

            string manifest = File.ReadAllText(Step20CanonicalUiKitBuilder.KitRoot + "/canonical_ui_kit_manifest.json");
            Check(ref total, ref passed, failures, manifest.Contains("\"assetCount\": 34"), "manifest pins 34 assets");
            Check(ref total, ref passed, failures, manifest.Contains("\"textBakedIntoSprites\": false"), "UI sprites contain no baked labels");
            Check(ref total, ref passed, failures, manifest.Contains("locked-russian-fairytale-production-ui"), "locked style identity pinned");

            string[] requiredSprites =
            {
                "ui_btn_primary.png", "ui_btn_secondary.png", "ui_btn_danger.png",
                "ui_panel_navy.png", "ui_panel_parchment.png", "ui_modal_frame.png",
                "ui_header_frame.png", "ui_top_bar.png", "ui_tab_idle.png",
                "ui_tab_selected.png", "ui_currency_pill.png", "ui_nav_tile.png",
                "ui_reward_tile.png", "ui_booster_slot.png", "ui_ranking_row.png",
                "ui_system_state_panel.png", "ui_goal_panel.png", "ui_board_frame.png",
                "ui_progress_track.png", "ui_progress_fill.png", "ui_modal_blocker.png",
                "ui_separator.png", "ui_jewel_sapphire.png", "ui_jewel_round.png", "ui_icon_button.png",
                "ui_pause_button.png", "ui_back_button.png", "ui_close_button.png",
                "ui_notification_badge.png", "ui_star.png", "ui_toggle_track.png",
                "ui_toggle_knob.png", "ui_map_node.png", "ui_corner_flourish.png"
            };
            foreach (string file in requiredSprites)
                Check(ref total, ref passed, failures, File.Exists(Step20CanonicalUiKitBuilder.KitRoot + "/" + file), "sprite exists: " + file);

            // Build/validate prefabs as part of the deterministic editor checkpoint.
            try
            {
                Step20CanonicalUiKitBuilder.BuildAll(false);
                List<string> prefabFailures = Step20CanonicalUiKitBuilder.ValidateAll();
                Check(ref total, ref passed, failures, prefabFailures.Count == 0, "21 required canonical UI prefabs validate after deterministic build");
                if (prefabFailures.Count > 0) failures.AddRange(prefabFailures);
            }
            catch (Exception e)
            {
                Check(ref total, ref passed, failures, false, "canonical UI prefab build threw " + e.GetType().Name + ": " + e.Message);
            }

            // Critical no-cutover boundary: Step 3.2 must not alter screenshot routing.
            string router = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            string registry = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/DesignScreenRegistry.cs");
            Check(ref total, ref passed, failures, router.Contains("BuildDesignLayer(id, root)"), "legacy screenshot design layer remains active for Step 3.2");
            Check(ref total, ref passed, failures, registry.Contains("DesignScreens/Home"), "DesignScreenRegistry remains active");

            if (failures.Count == 0)
            {
                Debug.Log("v1.20 Step 3.2 self-tests passed: " + passed + "/" + total);
                return;
            }
            string report = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.20 Step 3.2 self-tests failed: " + passed + "/" + total + "\n" + report);
            throw new InvalidOperationException("v1.20 Step 3.2 self-tests failed.\n" + report);
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
