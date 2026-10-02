#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ProductionUiFix01DeliveryEditModeTests
    {
        const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string AssetRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01";
        const string LockPath = "Assets/LegendsOfRus/Resources/ProductionUI/FIX01_SERIALIZED_DELIVERY.lock";
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_fix01_v120_step03_3r.json";

        [Test]
        public void DeliveryLockExistsAndPinsFix01Version()
        {
            Assert.That(File.Exists(LockPath), Is.True);
            StringAssert.Contains(ProductionUiFix01Runtime.StyleVersion, File.ReadAllText(LockPath));
        }

        [Test]
        public void ActivationConfigExists()
        {
            Assert.That(File.Exists(Activation), Is.True);
        }

        [Test]
        public void ActivationEnablesSerializedPrefabDelivery()
        {
            StringAssert.Contains("\"serializedPrefabDeliveryEnabled\": true", File.ReadAllText(Activation));
        }

        [Test]
        public void ActivationKeepsScreenshotRuntimeUseDisabled()
        {
            string json = File.ReadAllText(Activation);
            StringAssert.Contains("\"designScreensRuntimeFallbackEnabled\": false", json);
            StringAssert.Contains("\"referenceOnlyRuntimeUseEnabled\": false", json);
            StringAssert.Contains("\"screenshotCropProductionUseEnabled\": false", json);
        }

        [Test]
        public void ExactlyTenSerializedPageFilesAreShipped()
        {
            Assert.That(Directory.GetFiles(PageRoot, "*.prefab", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(10));
        }

        [TestCase("Loading", ScreenId.Loading)]
        [TestCase("Home", ScreenId.Home)]
        [TestCase("WorldMap", ScreenId.WorldMap)]
        [TestCase("ChapterMap", ScreenId.ChapterMap)]
        [TestCase("PreLevel", ScreenId.PreLevel)]
        [TestCase("Gameplay", ScreenId.Gameplay)]
        [TestCase("Pause", ScreenId.Pause)]
        [TestCase("Victory", ScreenId.Victory)]
        [TestCase("Fail", ScreenId.Fail)]
        [TestCase("ChapterComplete", ScreenId.ChapterComplete)]
        public void SerializedPrefabImportsWithFix01Bootstrap(string file, ScreenId expected)
        {
            string path = PageRoot + "/" + file + ".prefab";
            Assert.That(File.Exists(path), Is.True, path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, path);
            ProductionUiFix01PageBootstrap bootstrap = prefab.GetComponent<ProductionUiFix01PageBootstrap>();
            Assert.That(bootstrap, Is.Not.Null, path);
            Assert.That(bootstrap.ScreenId, Is.EqualTo(expected));
            Assert.That(bootstrap.StyleVersion, Is.EqualTo(ProductionUiFix01Runtime.StyleVersion));
        }

        [TestCase("Loading")]
        [TestCase("Home")]
        [TestCase("WorldMap")]
        [TestCase("ChapterMap")]
        [TestCase("PreLevel")]
        [TestCase("Gameplay")]
        [TestCase("Pause")]
        [TestCase("Victory")]
        [TestCase("Fail")]
        [TestCase("ChapterComplete")]
        public void SerializedPrefabTextHasNoCapturedScreenPath(string file)
        {
            string text = File.ReadAllText(PageRoot + "/" + file + ".prefab");
            Assert.That(text.Contains("DesignScreens"), Is.False);
            Assert.That(text.Contains("ReferenceOnly"), Is.False);
        }

        [Test]
        public void RuntimeBuilderContainsNoCapturedScreenDependency()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs");
            Assert.That(text.Contains("DesignScreens"), Is.False);
            Assert.That(text.Contains("ReferenceOnly"), Is.False);
            Assert.That(text.Contains("ReferenceMatch/"), Is.False);
            Assert.That(text.Contains("ReferenceMatchR3/"), Is.False);
        }

        [Test]
        public void CoreLoopRuntimeHasDirectFix01Fallback()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs");
            StringAssert.Contains("ProductionUiFix01Runtime.TryBuild(id, parent)", text);
        }

        [Test]
        public void OldBuildersAreAutoBuildGuardedByFix01()
        {
            string[] files =
            {
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs"
            };
            foreach (string file in files)
                StringAssert.Contains("Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled()", File.ReadAllText(file), file);
        }

        [Test]
        public void OldBuildersReapplyFix01AfterManualBuild()
        {
            string[] files =
            {
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs"
            };
            foreach (string file in files)
                StringAssert.Contains("Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false)", File.ReadAllText(file), file);
        }

        [Test]
        public void Fix01ShipsFiveCleanEnvironmentPlates()
        {
            string[] files = { "bg_home_clean.png", "bg_worldmap_clean.png", "bg_chaptermap_clean.png", "bg_prelevel_clean.png", "bg_gameplay_clean.png" };
            foreach (string file in files)
                Assert.That(File.Exists(AssetRoot + "/" + file), Is.True, file);
        }

        [TestCase("bg_home_clean.png", "Home.png")]
        [TestCase("bg_worldmap_clean.png", "WorldMap.png")]
        [TestCase("bg_chaptermap_clean.png", "ChapterMap.png")]
        [TestCase("bg_prelevel_clean.png", "PreLevel.png")]
        [TestCase("bg_gameplay_clean.png", "Gameplay.png")]
        public void EnvironmentPlateIsNotByteIdenticalToApprovedFullScreen(string plate, string reference)
        {
            string a = Sha256(AssetRoot + "/" + plate);
            string b = Sha256("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/" + reference);
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void HeroLayerAndPortraitAreStandaloneResources()
        {
            Assert.That(File.Exists(AssetRoot + "/hero_ivan_firebird.png"), Is.True);
            Assert.That(File.Exists(AssetRoot + "/portrait_ivan.png"), Is.True);
        }

        [Test]
        public void TwentyStandaloneIconsAreShipped()
        {
            Assert.That(Directory.GetFiles(AssetRoot + "/Icons", "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(20));
        }

        [Test]
        public void TwentyFourKitPrimitivesAreRuntimeReachable()
        {
            Assert.That(Directory.GetFiles(AssetRoot + "/Kit", "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(24));
        }

        [Test]
        public void SixGemSpritesAreRuntimeReachable()
        {
            Assert.That(Directory.GetFiles(AssetRoot + "/Gems", "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(6));
        }

        [Test]
        public void Fix01AssetsDoNotContainOldReferenceMatchStripNames()
        {
            string[] names = Directory.GetFiles(AssetRoot, "*.png", SearchOption.AllDirectories).Select(p => Path.GetFileName(p)).ToArray();
            Assert.That(names.Any(n => n.Contains("_component") || n.Contains("_scene") || n.Contains("_shell")), Is.False);
        }

        [Test]
        public void GameplayLiveBoardBoundaryIsStillPresent()
        {
            string router = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("SetPixelRect(rt, 79, 357, 786, 942)", router);
        }

        [Test]
        public void ScreenRouterStillPrefersProductionRegistry()
        {
            string router = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            int production = router.IndexOf("ProductionCoreLoopPageRegistry.IsCutover(id)", StringComparison.Ordinal);
            int legacy = router.IndexOf("DesignScreenRegistry.ResourceKey(id)", StringComparison.Ordinal);
            Assert.That(production, Is.GreaterThanOrEqualTo(0));
            Assert.That(legacy, Is.GreaterThan(production));
        }

        [Test]
        public void Addressables281RemainsPinned()
        {
            string manifest = File.ReadAllText("Packages/manifest.json");
            StringAssert.Contains("\"com.unity.addressables\": \"2.8.1\"", manifest);
        }

        [Test]
        public void CompileHotfix05RemainsDocumented()
        {
            Assert.That(File.Exists("README_COMPILE_HOTFIX05.md"), Is.True);
        }

        [Test]
        public void Fix01ProductionPublishingRemainsDisabled()
        {
            string json = File.ReadAllText(Activation);
            StringAssert.Contains("\"productionContentPublishAuthorized\": false", json);
            StringAssert.Contains("\"playConsoleActionEnabled\": false", json);
            StringAssert.Contains("\"finalVisualSignoffGranted\": false", json);
        }

        static string Sha256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
#endif
