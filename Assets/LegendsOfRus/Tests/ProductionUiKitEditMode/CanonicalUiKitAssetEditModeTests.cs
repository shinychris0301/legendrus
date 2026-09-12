using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionUiKit
{
    public sealed class CanonicalUiKitAssetEditModeTests
    {
        const string Root = "Assets/LegendsOfRus/Art/Production/UI/CanonicalKit";

        [Test]
        public void KitContainsExactlyThirtyFourPngSprites()
        {
            Assert.That(Directory.GetFiles(Root, "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(34));
        }

        [TestCase("ui_btn_primary.png", 1024, 256)]
        [TestCase("ui_btn_secondary.png", 1024, 256)]
        [TestCase("ui_btn_danger.png", 1024, 256)]
        [TestCase("ui_panel_navy.png", 768, 768)]
        [TestCase("ui_panel_parchment.png", 768, 768)]
        [TestCase("ui_modal_frame.png", 900, 1100)]
        [TestCase("ui_header_frame.png", 1024, 260)]
        [TestCase("ui_top_bar.png", 1024, 192)]
        [TestCase("ui_tab_idle.png", 512, 160)]
        [TestCase("ui_tab_selected.png", 512, 160)]
        [TestCase("ui_currency_pill.png", 512, 128)]
        [TestCase("ui_nav_tile.png", 320, 320)]
        [TestCase("ui_reward_tile.png", 320, 360)]
        [TestCase("ui_booster_slot.png", 256, 256)]
        [TestCase("ui_ranking_row.png", 1024, 180)]
        [TestCase("ui_system_state_panel.png", 900, 640)]
        [TestCase("ui_goal_panel.png", 640, 220)]
        [TestCase("ui_board_frame.png", 900, 900)]
        [TestCase("ui_progress_track.png", 640, 72)]
        [TestCase("ui_progress_fill.png", 640, 72)]
        [TestCase("ui_map_node.png", 220, 220)]
        public void KeySpriteSourceDimensionsArePinned(string file, int expectedWidth, int expectedHeight)
        {
            TextureImporter importer = AssetImporter.GetAtPath(Root + "/" + file) as TextureImporter;
            Assert.That(importer, Is.Not.Null, file);
            int width;
            int height;
            importer.GetSourceTextureWidthAndHeight(out width, out height);
            Assert.That(width, Is.EqualTo(expectedWidth), file + " width");
            Assert.That(height, Is.EqualTo(expectedHeight), file + " height");
        }

        [TestCase("ui_btn_primary.png")]
        [TestCase("ui_panel_navy.png")]
        [TestCase("ui_panel_parchment.png")]
        [TestCase("ui_top_bar.png")]
        [TestCase("ui_nav_tile.png")]
        [TestCase("ui_reward_tile.png")]
        [TestCase("ui_booster_slot.png")]
        [TestCase("ui_ranking_row.png")]
        [TestCase("ui_progress_track.png")]
        [TestCase("ui_progress_fill.png")]
        public void ProductionUiTexturesUseSafeMobileImportDefaults(string file)
        {
            TextureImporter importer = AssetImporter.GetAtPath(Root + "/" + file) as TextureImporter;
            Assert.That(importer, Is.Not.Null, file);
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite));
            Assert.That(importer.isReadable, Is.False);
            Assert.That(importer.mipmapEnabled, Is.False);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Clamp));
            TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
            Assert.That(android.overridden, Is.True);
            Assert.That(android.maxTextureSize, Is.LessThanOrEqualTo(2048));
            Assert.That(android.format, Is.EqualTo(TextureImporterFormat.ASTC_6x6));
        }

        [Test]
        public void KitManifestPinsLockedStyleAndNoBakedText()
        {
            string text = File.ReadAllText(Root + "/canonical_ui_kit_manifest.json");
            StringAssert.Contains("locked-russian-fairytale-production-ui", text);
            StringAssert.Contains("\"textBakedIntoSprites\": false", text);
            StringAssert.Contains("\"assetCount\": 34", text);
        }

        [Test]
        public void RuntimeCutoverRemainsDisabledDuringKitConstruction()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Resources/Config/production_ui_kit_v120_step03_2.json");
            StringAssert.Contains("\"screenshotRuntimeCutoverAuthorized\": false", text);
            StringAssert.Contains("\"screenshotDeletionAuthorized\": false", text);
        }

        [Test]
        public void ScreenshotPlaceholdersRemainIntactUntilPageReconstruction()
        {
            Assert.That(Directory.GetFiles("Assets/LegendsOfRus/Resources/DesignScreens", "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(22));
        }

        [Test]
        public void CanonicalMarkerPinsStepThreeTwoStyleVersion()
        {
            GameObject go = new GameObject("MarkerTest");
            try
            {
                CanonicalUiKitMarker marker = go.AddComponent<CanonicalUiKitMarker>();
                marker.Configure(CanonicalUiComponentRole.PrimaryButton);
                Assert.That(marker.StyleVersion, Is.EqualTo("v1.20-step03.2"));
                Assert.That(marker.Role, Is.EqualTo(CanonicalUiComponentRole.PrimaryButton));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
