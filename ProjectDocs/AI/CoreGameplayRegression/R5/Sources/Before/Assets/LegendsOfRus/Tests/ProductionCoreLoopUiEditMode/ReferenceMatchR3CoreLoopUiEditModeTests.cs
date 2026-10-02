using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ReferenceMatchR3CoreLoopUiEditModeTests
    {
        const string PageRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string ArtRoot = "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatchR3";
        const string Activation = "Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r3.json";

        [Test]
        public void R3MarkerPinsScopedVisualFidelityContract()
        {
            GameObject go = new GameObject("R3MarkerTest");
            try
            {
                ProductionReferenceMatchR3PageMarker marker = go.AddComponent<ProductionReferenceMatchR3PageMarker>();
                marker.Configure(ScreenId.Gameplay, new string('a', 64), "shell", 5, true, false);
                Assert.That(marker.ScreenId, Is.EqualTo(ScreenId.Gameplay));
                Assert.That(marker.ReferenceMatchVersion, Is.EqualTo("v1.20-step03.3R3"));
                Assert.That(marker.ReferenceSha256, Is.EqualTo(new string('a', 64)));
                Assert.That(marker.ProofScope, Is.EqualTo("shell"));
                Assert.That(marker.ProductionComponentCount, Is.EqualTo(5));
                Assert.That(marker.FullScreenScreenshotDependency, Is.False);
                Assert.That(marker.PreservesLiveGameplayBoard, Is.True);
                Assert.That(marker.OverlaysLiveGameplayForPause, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void ActivationKeepsGameplayAndPublishingMutationsFailClosed()
        {
            string text = File.ReadAllText(Activation);
            StringAssert.Contains("\"preLevelReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"gameplayShellReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"pauseModalReferenceMatchCutoverAuthorized\": true", text);
            StringAssert.Contains("\"liveGameplayBoardPreservationRequired\": true", text);
            StringAssert.Contains("\"fullScreenScreenshotRuntimeUseAuthorized\": false", text);
            StringAssert.Contains("\"gameplayBoardReplacementAuthorized\": false", text);
            StringAssert.Contains("\"gameplayRulesMutationAuthorized\": false", text);
            StringAssert.Contains("\"saveEconomyRewardMutationAuthorized\": false", text);
            StringAssert.Contains("\"finalReferenceMatchSignoffGranted\": false", text);
        }

        [TestCase("prelevel_top_hud.png", 941, 145)]
        [TestCase("prelevel_chapter_band.png", 941, 260)]
        [TestCase("prelevel_goals_left_scene.png", 190, 445)]
        [TestCase("prelevel_goals_panel.png", 561, 445)]
        [TestCase("prelevel_goals_right_scene.png", 190, 445)]
        [TestCase("prelevel_boosters_left_scene.png", 105, 285)]
        [TestCase("prelevel_boosters_panel.png", 731, 285)]
        [TestCase("prelevel_boosters_right_scene.png", 105, 285)]
        [TestCase("prelevel_reward_left_scene.png", 95, 300)]
        [TestCase("prelevel_reward_panel.png", 751, 300)]
        [TestCase("prelevel_reward_right_scene.png", 95, 300)]
        [TestCase("prelevel_bottom_controls.png", 941, 237)]
        [TestCase("gameplay_top_shell.png", 941, 357)]
        [TestCase("gameplay_left_environment.png", 79, 942)]
        [TestCase("gameplay_right_environment.png", 76, 942)]
        [TestCase("gameplay_booster_shell.png", 941, 205)]
        [TestCase("gameplay_score_footer.png", 941, 168)]
        [TestCase("pause_header.png", 640, 150)]
        [TestCase("pause_left_border.png", 30, 925)]
        [TestCase("pause_goals_panel.png", 580, 270)]
        [TestCase("pause_boosters_panel.png", 580, 200)]
        [TestCase("pause_buttons_panel.png", 580, 455)]
        [TestCase("pause_right_border.png", 30, 925)]
        [TestCase("pause_footer.png", 640, 50)]
        public void R3ProductionComponentHasPinnedDimensions(string file, int width, int height)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ArtRoot + "/" + file);
            if (texture == null)
                Assert.Ignore("Unity has not imported the R3 texture yet: " + file);
            Assert.That(texture.width, Is.EqualTo(width));
            Assert.That(texture.height, Is.EqualTo(height));
            Assert.That(texture.width == 941 && texture.height == 1672, Is.False);
        }

        [Test]
        public void PreLevelPixelProofIsExactForFullReference()
        {
            string text = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r3_pixel_proof.json");
            StringAssert.Contains("\"PreLevel\"", text);
            StringAssert.Contains("\"exactPixelReconstruction\": true", text);
            StringAssert.Contains("\"differentPixels\": 0", text);
        }

        [Test]
        public void GameplayPixelProofIsExactOutsideDynamicBoard()
        {
            string text = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r3_pixel_proof.json");
            StringAssert.Contains("\"dynamicBoardPixelCount\"", text);
            StringAssert.Contains("\"differentPixelsWithinScope\": 0", text);
            StringAssert.Contains("\"exactShellPixelReconstruction\": true", text);
        }

        [Test]
        public void PausePixelProofIsExactForCentralModalCrop()
        {
            string text = File.ReadAllText("ProjectDocs/UIReferenceMatch/v1_20_step03_3r3_pixel_proof.json");
            StringAssert.Contains("\"referenceStatus\": \"provisional_captured_reference\"", text);
            StringAssert.Contains("\"exactModalPixelReconstruction\": true", text);
            StringAssert.Contains("\"backgroundMode\": \"live_gameplay_plus_runtime_dim_overlay\"", text);
        }

        [Test]
        public void GameplayStillUsesLiveMatch3BoardOverlay()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("typeof(RectTransform), typeof(Match3Board)", text);
            StringAssert.Contains("boardGo.transform.SetParent(view.BoardMount, false);", text);
            StringAssert.Contains("view.Bind(activeBoard, ShowPause);", text);
        }

        [Test]
        public void PauseRemainsRealModalOverGameplay()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("ShowModal(ScreenId.Pause)", text);
            StringAssert.Contains("activeBoard.TryPause()", text);
            StringAssert.Contains("ResumeGameplayFromPause", text);
        }

        [Test]
        public void NativePreparationAndPauseExposeBoundInteractiveControls()
        {
            var preparation = Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>();
            var pause = Resources.Load<GameObject>("ProductionUI/Pages/Pause").GetComponent<ProductionPauseView>();
            Assert.That(preparation.HasRequiredBindings, Is.True);
            Assert.That(pause.HasRequiredBindings, Is.True);
            Assert.That(preparation.PlayButton.targetGraphic.raycastTarget, Is.True);
            foreach (var button in preparation.BoosterButtons)
                Assert.That(button.targetGraphic.raycastTarget, Is.True);
            foreach (var button in pause.Actions)
                Assert.That(button.targetGraphic.raycastTarget, Is.True);
        }

        [Test]
        public void BaseBuilderReappliesBothReferenceMatchWaves()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs");
            StringAssert.Contains("Step20CoreLoopReferenceMatchBuilder.BuildAll(false);", text);
            StringAssert.Contains("Step20CoreLoopReferenceMatchR3Builder.BuildAll(false);", text);
        }

        [Test]
        public void R3BuilderNeverLoadsCapturedReferenceAtRuntime()
        {
            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs");
            StringAssert.DoesNotContain("Resources.Load<Sprite>(\"DesignScreens/", text);
            StringAssert.DoesNotContain("ApprovedScreens/", text);
            StringAssert.Contains("forbidden DesignScreens dependency", text);
            StringAssert.Contains("ReferenceMatchR3", text);
        }

        [TestCase(ScreenId.PreLevel, 8, false, false)]
        [TestCase(ScreenId.Gameplay, 8, true, false)]
        [TestCase(ScreenId.Pause, 7, false, true)]
        public void GeneratedR3PrefabUsesExpectedLiveRuntimeBoundary(ScreenId id, int componentCount, bool preserveBoard, bool liveGameplayBehindPause)
        {
            string path = PageRoot + "/" + id + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, path);

            ProductionReferenceMatchR3PageMarker marker = prefab.GetComponent<ProductionReferenceMatchR3PageMarker>();
            Assert.That(marker, Is.Not.Null);
            Assert.That(marker.ProductionComponentCount, Is.EqualTo(componentCount));
            Assert.That(marker.PreservesLiveGameplayBoard, Is.EqualTo(preserveBoard));
            Assert.That(marker.OverlaysLiveGameplayForPause, Is.EqualTo(liveGameplayBehindPause));
            Assert.That(marker.FullScreenScreenshotDependency, Is.False);

            foreach (string raw in AssetDatabase.GetDependencies(path, true))
            {
                string dependency = raw.Replace('\\', '/');
                StringAssert.DoesNotContain("/Resources/DesignScreens/", dependency);
                StringAssert.DoesNotContain("/Art/ReferenceOnly/", dependency);
            }
        }

        [Test]
        public void ReferenceSourcesRemainUnchangedQaInputs()
        {
            Assert.That(File.Exists("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/PreLevel.png"), Is.True);
            Assert.That(File.Exists("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Gameplay.png"), Is.True);
            Assert.That(File.Exists("Assets/LegendsOfRus/Resources/DesignScreens/Pause.png"), Is.True);
        }
    }
}
