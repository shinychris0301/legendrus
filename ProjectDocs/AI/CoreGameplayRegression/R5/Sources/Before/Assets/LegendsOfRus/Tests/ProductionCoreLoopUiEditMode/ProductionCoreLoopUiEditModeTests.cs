using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ProductionCoreLoopUiEditModeTests
    {
        const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        static readonly ScreenId[] Cutover =
        {
            ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap,
            ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause, ScreenId.Victory,
            ScreenId.Fail, ScreenId.ChapterComplete
        };

        [Test]
        public void RegistryPinsTenCutoverPages()
        {
            Assert.That(ProductionCoreLoopPageRegistry.CutoverPageCount, Is.EqualTo(10));
        }

        [TestCase(ScreenId.Loading)] [TestCase(ScreenId.Home)] [TestCase(ScreenId.WorldMap)]
        [TestCase(ScreenId.ChapterMap)] [TestCase(ScreenId.PreLevel)] [TestCase(ScreenId.Gameplay)]
        [TestCase(ScreenId.Pause)] [TestCase(ScreenId.Victory)] [TestCase(ScreenId.Fail)]
        [TestCase(ScreenId.ChapterComplete)]
        public void CoreLoopRouteIsCutover(ScreenId id)
        {
            Assert.That(ProductionCoreLoopPageRegistry.IsCutover(id), Is.True);
            StringAssert.StartsWith("ProductionUI/Pages/", ProductionCoreLoopPageRegistry.ResourceKey(id));
        }

        [TestCase(ScreenId.Dialogue)] [TestCase(ScreenId.Shop)] [TestCase(ScreenId.DailyReward)]
        [TestCase(ScreenId.RewardClaim)] [TestCase(ScreenId.BoosterSelect)] [TestCase(ScreenId.Events)]
        [TestCase(ScreenId.Profile)] [TestCase(ScreenId.Settings)] [TestCase(ScreenId.Characters)]
        [TestCase(ScreenId.Mail)] [TestCase(ScreenId.OutOfEnergy)] [TestCase(ScreenId.Chapter15)]
        public void NonCoreRouteIsNotPrematurelyCutover(ScreenId id)
        {
            Assert.That(ProductionCoreLoopPageRegistry.IsCutover(id), Is.False);
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(id), Is.Empty);
        }

        [Test]
        public void ScreenRouterUsesProductionRegistryBeforeLegacyRegistry()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            int production = text.IndexOf("ProductionCoreLoopPageRegistry.IsCutover(id)");
            int legacy = text.IndexOf("DesignScreenRegistry.ResourceKey(id)");
            Assert.That(production, Is.GreaterThanOrEqualTo(0));
            Assert.That(legacy, Is.GreaterThan(production));
        }

        [Test]
        public void MissingProductionPrefabCannotFallBackToDesignScreens()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs");
            StringAssert.DoesNotContain("DesignScreenRegistry", text);
            StringAssert.DoesNotContain("DesignScreens/", text);
            StringAssert.Contains("BuildEmergencySurface", text);
        }

        [Test]
        public void ActivationPinsScopedCutoverAndBlocksDeletion()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Resources/Config/production_ui_core_loop_v120_step03_3.json");
            StringAssert.Contains("\"cutoverPageCount\": 10", text);
            StringAssert.Contains("\"coreLoopScreenshotRuntimeCutoverAuthorized\": true", text);
            StringAssert.Contains("\"screenshotDeletionAuthorized\": false", text);
            StringAssert.Contains("\"missingProductionPrefabMayUseScreenshot\": false", text);
        }

        [Test]
        public void ScreenshotReferencesRemainForQaAndLaterWaves()
        {
            Assert.That(Directory.GetFiles("Assets/LegendsOfRus/Resources/DesignScreens", "*.png", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(22));
        }

        [Test]
        public void CoreLoopBuilderDeclaresExactlyTenRequiredPages()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            StringAssert.Contains("RequiredPageCount = 10", text);
            StringAssert.Contains("BuildLoading();", text);
            StringAssert.Contains("BuildChapterComplete();", text);
        }

        [Test]
        public void BuilderForbidsDesignScreenDependencies()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            StringAssert.Contains("/Resources/DesignScreens/", text);
            StringAssert.Contains("forbidden screenshot dependency", text);
        }

        [Test]
        public void WorldMapNowReceivesDynamicTopHudValues()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("case ScreenId.WorldMap:", text);
            StringAssert.Contains("BuildDynamicResourceHud(root)", text);
        }

        [Test]
        public void MarkerDefaultsToScreenshotFreeStepThreeThree()
        {
            GameObject go = new GameObject("MarkerTest");
            try
            {
                ProductionCoreLoopPageMarker marker = go.AddComponent<ProductionCoreLoopPageMarker>();
                marker.Configure(ScreenId.Home);
                Assert.That(marker.ScreenId, Is.EqualTo(ScreenId.Home));
                Assert.That(marker.ScreenshotFree, Is.True);
                Assert.That(marker.StyleVersion, Is.EqualTo("v1.20-step03.3"));
                Assert.That(marker.ReferenceResolution, Is.EqualTo(new Vector2(941f, 1672f)));
            }
            finally { Object.DestroyImmediate(go); }
        }

        [TestCase("Loading")] [TestCase("Home")] [TestCase("WorldMap")] [TestCase("ChapterMap")]
        [TestCase("PreLevel")] [TestCase("Gameplay")] [TestCase("Pause")] [TestCase("Victory")]
        [TestCase("Fail")] [TestCase("ChapterComplete")]
        public void GeneratedPrefabIsScreenshotFreeWhenEditorBuildHasRun(string page)
        {
            string path = PageRoot + "/" + page + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                Assert.Ignore("Deterministic page builder has not run yet in this editor session.");
            string[] deps = AssetDatabase.GetDependencies(path, true);
            foreach (string dep in deps)
                StringAssert.DoesNotContain("/Resources/DesignScreens/", dep.Replace('\\','/'));
            ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
            Assert.That(marker, Is.Not.Null);
            Assert.That(marker.ScreenshotFree, Is.True);
        }
    }
}
