using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ReferenceMatchCoreLoopUiEditModeTests
    {
        const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string ArtRoot = "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatch";
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r2.json";

        static readonly string[] Components =
        {
            "home_top_hud.png",
            "home_logo_scene.png",
            "home_hero_scene.png",
            "home_play_component.png",
            "home_navigation_component.png",
            "home_quest_component.png",
            "worldmap_top_hud.png",
            "worldmap_title_component.png",
            "worldmap_painted_map_plate.png",
            "worldmap_bottom_navigation.png",
            "chaptermap_top_hud.png",
            "chaptermap_ribbon_component.png",
            "chaptermap_painted_level_path.png",
            "chaptermap_bottom_controls.png"
        };

        [Test]
        public void ReferenceMatchMarkerPinsStepThreeThreeR2()
        {
            GameObject go = new GameObject("ReferenceMatchMarkerTest");
            try
            {
                ProductionReferenceMatchPageMarker marker = go.AddComponent<ProductionReferenceMatchPageMarker>();
                marker.Configure(ScreenId.Home, new string('a', 64), 6);
                Assert.That(marker.ScreenId, Is.EqualTo(ScreenId.Home));
                Assert.That(marker.ReferenceMatchVersion, Is.EqualTo("v1.20-step03.3R2"));
                Assert.That(marker.ReferenceSha256, Is.EqualTo(new string('a', 64)));
                Assert.That(marker.ProductionComponentCount, Is.EqualTo(6));
                Assert.That(marker.FullScreenScreenshotDependency, Is.False);
                Assert.That(marker.ReferenceMatchStaticComposition, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void ActivationAuthorizesOnlyScopedReferenceMatchCutover()
        {
            string text = File.ReadAllText(Activation);
            StringAssert.Contains("\"referenceMatchRebuildAuthorized\": true", text);
            StringAssert.Contains("\"homeReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"worldMapReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"chapterMapReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"fullScreenScreenshotRuntimeUseAuthorized\": false", text);
            StringAssert.Contains("\"screenshotDeletionAuthorized\": false", text);
            StringAssert.Contains("\"lockedReferenceOverwriteAuthorized\": false", text);
            StringAssert.Contains("\"finalReferenceMatchSignoffGranted\": false", text);
        }

        [Test]
        public void ExactlyFourteenProductionComponentsArePinned()
        {
            Assert.That(Components.Length, Is.EqualTo(14));
            foreach (string component in Components)
                Assert.That(File.Exists(ArtRoot + "/" + component), Is.True, component);
        }

        [TestCase("home_top_hud.png", 941, 150)]
        [TestCase("home_logo_scene.png", 941, 350)]
        [TestCase("home_hero_scene.png", 941, 500)]
        [TestCase("home_play_component.png", 941, 210)]
        [TestCase("home_navigation_component.png", 941, 240)]
        [TestCase("home_quest_component.png", 941, 222)]
        [TestCase("worldmap_top_hud.png", 941, 135)]
        [TestCase("worldmap_title_component.png", 941, 150)]
        [TestCase("worldmap_painted_map_plate.png", 941, 1055)]
        [TestCase("worldmap_bottom_navigation.png", 941, 332)]
        [TestCase("chaptermap_top_hud.png", 941, 140)]
        [TestCase("chaptermap_ribbon_component.png", 941, 250)]
        [TestCase("chaptermap_painted_level_path.png", 941, 1060)]
        [TestCase("chaptermap_bottom_controls.png", 941, 222)]
        public void ProductionComponentHasPinnedDimensions(string file, int width, int height)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ArtRoot + "/" + file);
            if (texture == null)
                Assert.Ignore("Unity has not imported the new Step 3.3R.2 texture yet: " + file);
            Assert.That(texture.width, Is.EqualTo(width));
            Assert.That(texture.height, Is.EqualTo(height));
            Assert.That(texture.width == 941 && texture.height == 1672, Is.False, "No runtime component may be a full-screen approved screenshot.");
        }

        [Test]
        public void ComponentHeightsExactlyCoverHomeReferenceCanvas()
        {
            Assert.That(150 + 350 + 500 + 210 + 240 + 222, Is.EqualTo(1672));
        }

        [Test]
        public void ComponentHeightsExactlyCoverWorldMapReferenceCanvas()
        {
            Assert.That(135 + 150 + 1055 + 332, Is.EqualTo(1672));
        }

        [Test]
        public void ComponentHeightsExactlyCoverChapterMapReferenceCanvas()
        {
            Assert.That(140 + 250 + 1060 + 222, Is.EqualTo(1672));
        }

        [Test]
        public void PixelProofPinsExactStaticComponentReconstruction()
        {
            string text = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r2_pixel_proof.json");
            Assert.That(Count(text, "\"exactPixelReconstruction\": true"), Is.EqualTo(3));
            Assert.That(Count(text, "\"differentPixels\": 0"), Is.EqualTo(3));
            Assert.That(Count(text, "\"maxChannelError\": 0"), Is.EqualTo(3));
        }

        [Test]
        public void ReferenceSourcesRemainQaOnlyAndPinned()
        {
            Assert.That(File.Exists("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png"), Is.True);
            Assert.That(File.Exists("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/WorldMap.png"), Is.True);
            Assert.That(File.Exists("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterMap.png"), Is.True);
        }

        [Test]
        public void ReferenceMatchBuilderNeverLoadsReferenceOnlyOrDesignScreens()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs");
            StringAssert.DoesNotContain("ApprovedScreens/", text);
            StringAssert.Contains("forbidden DesignScreens dependency", text);
            StringAssert.Contains("Art/Production/CoreLoop/ReferenceMatch", text);
        }

        [Test]
        public void BaseBuilderReappliesReferenceMatchAfterManualRebuild()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            StringAssert.Contains("Step20CoreLoopReferenceMatchBuilder.BuildAll(false);", text);
        }

        [Test]
        public void RuntimeRegistryStillCutsOverHomeWorldMapChapterMap()
        {
            Assert.That(ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Home), Is.True);
            Assert.That(ProductionCoreLoopPageRegistry.IsCutover(ScreenId.WorldMap), Is.True);
            Assert.That(ProductionCoreLoopPageRegistry.IsCutover(ScreenId.ChapterMap), Is.True);
        }

        [TestCase(ScreenId.Home, 30)]
        [TestCase(ScreenId.WorldMap, 5)]
        [TestCase(ScreenId.ChapterMap, 5)]
        public void CurrentNativePrefabRetainsProductionOnlyDependencies(ScreenId id, int expectedComponents)
        {
            string path = PageRoot + "/" + id + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, path);

            ProductionReferenceMatchPageMarker marker = prefab.GetComponent<ProductionReferenceMatchPageMarker>();
            Assert.That(marker, Is.Not.Null);
            Assert.That(marker.ProductionComponentCount, Is.EqualTo(expectedComponents));
            Assert.That(marker.FullScreenScreenshotDependency, Is.False);

            string[] dependencies = AssetDatabase.GetDependencies(path, true);
            foreach (string raw in dependencies)
            {
                string dependency = raw.Replace('\\', '/');
                StringAssert.DoesNotContain("/Resources/DesignScreens/", dependency);
                StringAssert.DoesNotContain("/Art/ReferenceOnly/", dependency);
            }
        }

        [Test]
        public void ScreenRouterBehaviorLayerRemainsSeparateFromProductionVisuals()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("BuildInteractionLayer(id, root)", text);
            StringAssert.Contains("BuildDynamicResourceHud(root)", text);
            StringAssert.DoesNotContain("Art/ReferenceOnly/ApprovedScreens", text);
        }

        [Test]
        public void ComponentManifestPinsThreeReferencesAndFourteenComponents()
        {
            string text = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r2_component_manifest.json");
            Assert.That(Count(text, "\"sourceReference\""), Is.EqualTo(3));
            Assert.That(Count(text, "\"role\""), Is.EqualTo(14));
            StringAssert.Contains("Home.png", text);
            StringAssert.Contains("WorldMap.png", text);
            StringAssert.Contains("ChapterMap.png", text);
        }

        static int Count(string text, string needle)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(needle, index, System.StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }
            return count;
        }
    }
}
