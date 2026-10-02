#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class HomeAlignmentDeliveryEditModeTests
    {
        const string ResourcesRoot = "Assets/LegendsOfRus/Resources/";
        const string PrefabPath = ResourcesRoot + "ProductionUI/Pages/Home.prefab";
        const string GeneratedSourceRoot = "ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/Sources/";

        [Test]
        public void ActiveHomePreservesIdentityAndHasCompleteSerializedViewAndLayout()
        {
            GameObject prefab = LoadHome();
            Assert.That(AssetDatabase.AssetPathToGUID(PrefabPath), Is.EqualTo("a3c6628ddb0a3bcc3d254d89a5b54043"));
            Assert.That(prefab.GetComponent<ProductionUiFix01PageBootstrap>().ScreenId, Is.EqualTo(ScreenId.Home));
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Home), Is.EqualTo("ProductionUI/Pages/Home"));
            ProductionHomeView view = prefab.GetComponent<ProductionHomeView>();
            Assert.That(view, Is.Not.Null);
            Assert.That(view.Revision, Is.EqualTo(ProductionHomeView.CurrentRevision));
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(prefab.GetComponent<ProductionHomeLayout>().HasRequiredBindings, Is.True);
            Assert.That(prefab.transform.Find(ProductionHomeRuntime.ContentName), Is.Not.Null);
            Assert.That(prefab.transform.Find("FIX01_CONTENT"), Is.Null);
            Assert.That(view.Typography, Is.SameAs(AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(
                ResourcesRoot + ProductionHomeRuntime.TypographyResource + ".asset")));
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true), Is.Empty);
            Button[] buttons = prefab.GetComponentsInChildren<Button>(true);
            Assert.That(buttons.Length, Is.EqualTo(12));
            Assert.That(buttons, Is.EquivalentTo(view.Buttons()));
            foreach (Button button in buttons)
            {
                Assert.That(button.targetGraphic, Is.Not.Null, button.name);
                Graphic hitSurface = button.GetComponent<Graphic>();
                Assert.That(hitSurface, Is.Not.Null, button.name);
                Assert.That(hitSurface.raycastTarget, Is.True, button.name);
                Assert.That(button.targetGraphic.transform == button.transform ||
                    button.targetGraphic.transform.IsChildOf(button.transform), Is.True, button.name);
                ProductionHomeButtonFeedback feedback = button.GetComponent<ProductionHomeButtonFeedback>();
                Assert.That(feedback, Is.Not.Null, button.name);
                Assert.That(feedback.Role, Is.EqualTo(button == view.PlayButton
                    ? MetaUiControlRole.Primary : MetaUiControlRole.Standard), button.name);
            }
            Type owner = Type.GetType("LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder, Assembly-CSharp-Editor", true);
            MethodInfo needsBuild = owner.GetMethod("PageNeedsBuild", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That((bool)needsBuild.Invoke(null, new object[] { ScreenId.Home }), Is.False);
            string referenceSha = (string)owner.GetField("HomeAlignmentReferenceSha256").GetRawConstantValue();
            Assert.That(prefab.GetComponent<ProductionReferenceMatchPageMarker>().ReferenceSha256, Is.EqualTo(referenceSha));
            Assert.That(referenceSha, Is.EqualTo(Sha256("Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png")));
        }

        [Test]
        public void LoadingPreparationListsExactlyTheTwentySixTexturesUsedBySerializedHome()
        {
            GameObject prefab = LoadHome();
            IReadOnlyList<string> keys = ProductionUiFix01Runtime.GetHomeTextureResourceKeys();
            Assert.That(keys.Count, Is.EqualTo(26));
            Assert.That(keys.Distinct().Count(), Is.EqualTo(keys.Count));
            var preparedPaths = new HashSet<string>(keys.Select(key => ResourcesRoot + key + ".png"));
            var renderedPaths = new HashSet<string>();
            foreach (Image image in prefab.GetComponentsInChildren<Image>(true))
                if (image.sprite != null) renderedPaths.Add(AssetDatabase.GetAssetPath(image.sprite));
            Assert.That(renderedPaths, Is.EquivalentTo(preparedPaths));
            using (var preparation = new HomeLoadingPreparation())
                Assert.That(preparation.TotalResourceCount, Is.EqualTo(27));
            foreach (string dependency in AssetDatabase.GetDependencies(PrefabPath, true))
            {
                Assert.That(dependency.Contains("/Resources/DesignScreens/") ||
                    dependency.Contains("/Art/ReferenceOnly/") || dependency.StartsWith("ProjectDocs/"), Is.False, dependency);
                Assert.That(dependency.Contains("/ReferenceMatch/") || dependency.Contains("/Wave01Refined/"), Is.False, dependency);
                Assert.That(dependency.EndsWith(".png") && !preparedPaths.Contains(dependency), Is.False,
                    "Home may use only its declared independently generated Home artwork: " + dependency);
            }
        }

        [TestCase("home_environment_generated")]
        [TestCase("nav_map_generated")]
        [TestCase("nav_shop_generated")]
        [TestCase("nav_events_generated")]
        [TestCase("nav_characters_generated")]
        [TestCase("mail_medallion_generated")]
        [TestCase("reward_medallion_generated")]
        [TestCase("hud_frame")]
        [TestCase("avatar_ring")]
        [TestCase("resource_capsule")]
        [TestCase("quest_panel")]
        [TestCase("coin_icon")]
        [TestCase("gems_icon")]
        [TestCase("energy_icon")]
        [TestCase("settings_medallion")]
        [TestCase("home_hero_firebird")]
        [TestCase("home_logo")]
        [TestCase("home_play_button")]
        [TestCase("avatar_portrait")]
        [TestCase("resource_plus")]
        [TestCase("progress_frame")]
        [TestCase("progress_fill")]
        [TestCase("quest_reward_frame")]
        [TestCase("quest_reward_gem")]
        [TestCase("footer_base_generated")]
        [TestCase("home_map_foliage")]
        public void IndividualLayerImportsWithoutPixelResizeOrColorSpaceAlteration(string name)
        {
            string path = ResourcesRoot + ProductionHomeRuntime.ArtRoot + name + ".png";
            bool foreground = name != "home_environment_generated";
            string sourceRoot = name == "home_map_foliage"
                ? "ProjectDocs/AI/HomeStyleAlignment/ToneAndFoliageR9/Sources/" : GeneratedSourceRoot;
            Assert.That(Sha256(path), Is.EqualTo(Sha256(sourceRoot + name + ".png")),
                "Runtime assets must retain exact selected independently generated bytes, including the new dusk background.");
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite));
            Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single));
            Assert.That(importer.npotScale, Is.EqualTo(TextureImporterNPOTScale.None));
            int maxSize = foreground ? 4096 : 2048;
            Assert.That(importer.maxTextureSize, Is.EqualTo(maxSize));
            Assert.That(importer.mipmapEnabled, Is.EqualTo(foreground));
            Assert.That(importer.mipmapFilter, Is.EqualTo(TextureImporterMipFilter.BoxFilter));
            Assert.That(importer.filterMode, Is.EqualTo(foreground ? FilterMode.Trilinear : FilterMode.Bilinear));
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.alphaIsTransparency, Is.EqualTo(foreground));
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            Assert.That(settings.spriteMeshType, Is.EqualTo(SpriteMeshType.FullRect));
            Assert.That(settings.spriteBorder, Is.EqualTo(Vector4.zero));
            TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
            Assert.That(android.overridden, Is.True);
            Assert.That(android.maxTextureSize, Is.EqualTo(maxSize));
            Assert.That(android.format, Is.EqualTo(foreground ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB24));
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            importer.GetSourceTextureWidthAndHeight(out int sourceWidth, out int sourceHeight);
            Assert.That(texture.width, Is.EqualTo(sourceWidth));
            Assert.That(texture.height, Is.EqualTo(sourceHeight));
            Assert.That(AssetDatabase.LoadAssetAtPath<Sprite>(path), Is.Not.Null);
        }

        [Test]
        public void BackgroundToneAndWholeMapFoliageRemainSeparateNoninteractiveLayers()
        {
            ProductionHomeView view = LoadHome().GetComponent<ProductionHomeView>();
            ProductionHomeLayout layout = view.GetComponent<ProductionHomeLayout>();
            ProductionHomeBackdropShade shade = view.BackgroundShade;
            Assert.That(shade, Is.Not.Null);
            Assert.That(view.GetComponentsInChildren<ProductionHomeBackdropShade>(true).Length, Is.EqualTo(1));
            Assert.That(shade.transform.parent, Is.SameAs(view.Environment.transform));
            Assert.That(view.Environment.transform.parent, Is.SameAs(view.Ivan.transform.parent));
            Assert.That(view.Environment.transform.GetSiblingIndex(), Is.LessThan(view.Ivan.transform.GetSiblingIndex()));
            Assert.That(shade.rectTransform.anchorMin, Is.EqualTo(Vector2.zero));
            Assert.That(shade.rectTransform.anchorMax, Is.EqualTo(Vector2.one));
            Assert.That(shade.rectTransform.offsetMin, Is.EqualTo(Vector2.zero));
            Assert.That(shade.rectTransform.offsetMax, Is.EqualTo(Vector2.zero));
            Assert.That(shade.enabled && shade.gameObject.activeSelf, Is.True);
            Assert.That(shade.raycastTarget, Is.False);
            Assert.That(shade.Strength, Is.GreaterThan(0).And.LessThanOrEqualTo(1));
            Assert.That(shade.GetComponentsInChildren<Selectable>(true), Is.Empty);
            Assert.That(view.LowerShade.color.a, Is.Zero, "The old wash above the hero must remain disabled by alpha.");

            Image foliage = view.MapFoliage;
            Assert.That(foliage, Is.Not.Null);
            Assert.That(foliage.sprite, Is.SameAs(AssetDatabase.LoadAssetAtPath<Sprite>(
                ResourcesRoot + ProductionHomeRuntime.ArtRoot + "home_map_foliage.png")));
            Assert.That(view.GetComponentsInChildren<Image>(true).Count(i => i.sprite == foliage.sprite), Is.EqualTo(1));
            Assert.That(foliage.enabled && foliage.gameObject.activeSelf, Is.True);
            Assert.That(foliage.transform.parent, Is.SameAs(layout.Navigation));
            Assert.That(layout.Navigation.parent, Is.SameAs(view.Ivan.transform.parent));
            Assert.That(layout.Navigation.GetSiblingIndex(), Is.GreaterThan(view.Ivan.transform.GetSiblingIndex()));
            Assert.That(foliage.transform.GetSiblingIndex(), Is.Zero);
            Assert.That(foliage.type, Is.EqualTo(Image.Type.Simple));
            Assert.That(foliage.useSpriteMesh, Is.False);
            Assert.That(foliage.preserveAspect, Is.True);
            Assert.That(foliage.color, Is.EqualTo(Color.white));
            Assert.That(foliage.raycastTarget, Is.False);
            Assert.That(foliage.sprite.rect.position, Is.EqualTo(Vector2.zero));
            Assert.That(foliage.sprite.rect.size, Is.EqualTo(new Vector2(foliage.sprite.texture.width, foliage.sprite.texture.height)));
            Assert.That(foliage.GetComponent<Mask>(), Is.Null);
            Assert.That(foliage.GetComponent<RectMask2D>(), Is.Null);
            Assert.That(foliage.GetComponentsInChildren<Selectable>(true), Is.Empty);
            foreach (Button card in new[] { view.WorldMapButton, view.ShopButton, view.EventsButton, view.CharactersButton })
            {
                Transform backing = layout.Navigation.Find(card.name + "OpaqueNavyBacking");
                Assert.That(backing, Is.Not.Null, card.name);
                Assert.That(backing.GetSiblingIndex(), Is.GreaterThan(foliage.transform.GetSiblingIndex()), card.name);
                Assert.That(card.transform.GetSiblingIndex(), Is.GreaterThan(backing.GetSiblingIndex()), card.name);
            }
            Assert.That(view.GetComponentsInChildren<Button>(true).Length, Is.EqualTo(12));
        }

        [Test]
        public void SelectiveLightingUsesOnlyTheTwoExistingWholeSceneSprites()
        {
            ProductionHomeView view = LoadHome().GetComponent<ProductionHomeView>();
            Assert.That(view.HasSceneLightingBindings, Is.True);
            Assert.That(view.GetComponentsInChildren<ProductionHomeSceneLighting>(true).Length, Is.EqualTo(2));
            foreach (Image image in new[] { view.Ivan, view.MapFoliage })
            {
                Assert.That(image.raycastTarget, Is.False);
                Assert.That(image.type, Is.EqualTo(Image.Type.Simple));
                Assert.That(image.useSpriteMesh, Is.False);
                Assert.That(image.sprite.rect.position, Is.EqualTo(Vector2.zero));
                Assert.That(image.sprite.rect.size, Is.EqualTo(new Vector2(image.sprite.texture.width, image.sprite.texture.height)));
                Assert.That(image.material.shader.name, Is.EqualTo(ProductionHomeRuntime.SceneLightingShader));
                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(image.sprite));
                Assert.That(importer.spritePackingTag, Is.Empty, "Lighting masks use the existing full, unpacked sprite UVs.");
            }
            Component[] effects = view.Ivan.GetComponents<Component>();
            Assert.That(Array.IndexOf(effects, view.Ivan.GetComponent<ProductionHomeSceneLighting>()),
                Is.GreaterThan(Array.IndexOf(effects, view.Ivan.GetComponent<SplashTitleCharacterFade>())),
                "Append lighting after the accepted lower-fade mesh has been produced.");
            string[] dependencies = AssetDatabase.GetDependencies(PrefabPath, true);
            Assert.That(dependencies, Does.Contain(ResourcesRoot + ProductionHomeRuntime.HeroLightingMaterialResource + ".mat"));
            Assert.That(dependencies, Does.Contain(ResourcesRoot + ProductionHomeRuntime.FoliageLightingMaterialResource + ".mat"));
        }

        [Test]
        public void HomeUsesDeclaredLayerGeometryWithSceneOnlyToneAndLiveLabels()
        {
            ProductionHomeView view = LoadHome().GetComponent<ProductionHomeView>();
            Assert.That(view.HasGeneratedControlBindings && view.HasReferenceForegroundBindings, Is.True);
            Assert.That(ProductionHomeRuntime.EnvironmentTone, Is.EqualTo((Color)new Color32(235, 235, 235, 255)));
            Assert.That(ProductionHomeRuntime.HeroTone, Is.EqualTo((Color)new Color32(242, 242, 242, 255)));
            foreach (Image image in view.GetComponentsInChildren<Image>(true))
                if (image.sprite != null)
                {
                    Color expected = image == view.Environment ? ProductionHomeRuntime.EnvironmentTone :
                        image == view.Ivan ? ProductionHomeRuntime.HeroTone : Color.white;
                    Assert.That(image.color, Is.EqualTo(expected), image.name);
                }
            ProductionHomeLayout layout = view.GetComponent<ProductionHomeLayout>();
            AssertRect(layout.Hud, Vector2.zero, new Vector2(941, 130));
            Assert.That(view.HudFrame.transform.parent, Is.SameAs(layout.Hud));
            Assert.That(layout.Hud.GetComponent<Image>().sprite, Is.Null);
            Assert.That(view.HudFrame.preserveAspect, Is.True);
            AssertRect(view.HudFrame.rectTransform, new Vector2(-22f * 941 / 2132, -(8 - 198f * 941 / 2132)), new Vector2(2176f * 941 / 2132, 723f * 941 / 2132));
            Assert.That(layout.HudBacking, Is.SameAs(view.transform.Find(
                ProductionHomeRuntime.ContentName + "/HudOpaqueNavyBacking")));
            AssertRect(layout.HudBacking, new Vector2(5, -5), new Vector2(931, 117));
            Assert.That(layout.HudBacking.parent, Is.SameAs(layout.Hud.parent));
            Assert.That(layout.HudBacking.GetSiblingIndex(), Is.LessThan(layout.Hud.GetSiblingIndex()));
            AssertRect(view.AvatarFrame.rectTransform, new Vector2(-20, 0), new Vector2(194, 194f * 1425 / 1104));
            AssertRect(view.Logo.rectTransform, new Vector2(205.5f, -110), new Vector2(530, 530f * 1041 / 1511));
            AssertRect(view.Ivan.rectTransform, new Vector2(-20, -474), new Vector2(967, 967));
            AssertRect(view.PlayButton.transform as RectTransform, new Vector2(185.5f, -(1230 - 570f * 803 / 1959)), new Vector2(570, 570f * 803 / 1959));
            foreach (Button medallion in new[] { view.MailButton, view.DailyRewardButton })
            {
                Mask mask = medallion.GetComponent<Mask>();
                Assert.That(mask == null || !mask.enabled, Is.True, "Whole ornamental silhouettes must not be circle-cropped.");
                ProductionHomePanel backing = medallion.transform.Find("OpaqueNavyBacking").GetComponent<ProductionHomePanel>();
                Assert.That(backing.Frame, Is.False);
                Assert.That(backing.raycastTarget, Is.False);
                Assert.That(backing.color.a * backing.LeftColor.a, Is.EqualTo(1));
                Assert.That(backing.transform.GetSiblingIndex(), Is.LessThan(medallion.targetGraphic.transform.GetSiblingIndex()));
            }
            Button[] navigation = { view.WorldMapButton, view.ShopButton, view.EventsButton, view.CharactersButton };
            Text[] labels = { view.WorldMapText, view.ShopText, view.EventsText, view.CharactersText };
            float cardArtY = 174 - 981f / 1254 * 255;
            float captionTop = 174 + 1.5f / 1254 * 255 - 16.5f;
            for (int i = 0; i < navigation.Length; i++)
            {
                AssertRect(navigation[i].transform as RectTransform, new Vector2(i * 212, 0), new Vector2(203, 222));
                Image hit = navigation[i].GetComponent<Image>();
                Assert.That(hit.sprite, Is.Null, "Retain the existing Image ID as the clear native hit surface.");
                Assert.That(hit.color, Is.EqualTo(Color.clear));
                Image art = navigation[i].targetGraphic as Image;
                Assert.That(art.transform.parent, Is.SameAs(navigation[i].transform));
                Assert.That(art.raycastTarget, Is.False);
                Assert.That(art.preserveAspect, Is.False);
                AssertRect(art.rectTransform, new Vector2(-10.5f, -cardArtY), new Vector2(224, 255));
                Assert.That(labels[i].transform.parent, Is.SameAs(navigation[i].transform));
                Assert.That(art.transform.GetSiblingIndex(), Is.LessThan(labels[i].transform.GetSiblingIndex()));
                float textRatio = i == 0 || i == 3 ? .94f : 1;
                AssertRect(labels[i].rectTransform, new Vector2(4, -captionTop), new Vector2(390 / textRatio, 66));
                Assert.That(labels[i].rectTransform.localScale, Is.EqualTo(new Vector3(.5f * textRatio, .5f, 1)));
                Assert.That(labels[i].fontSize, Is.EqualTo(48));
                Assert.That(labels[i].GetComponent<SplashTitleTextGeometry>(), Is.Not.Null);
                Assert.That(labels[i].raycastTarget, Is.False);
            }
            Assert.That(view.PlayButton.GetComponent<Image>().sprite, Is.Null);
            Image playArt = view.PlayButton.targetGraphic as Image;
            Assert.That(playArt.transform.parent, Is.SameAs(view.PlayButton.transform));
            Assert.That(playArt.raycastTarget, Is.False);
            Assert.That(playArt.preserveAspect, Is.False);
            AssertRect(playArt.rectTransform, new Vector2(0, (1230 - 570f * 803 / 1959) - 991), new Vector2(570, 264));
            AssertRect(view.GemsIcon.rectTransform, new Vector2(0, 5), new Vector2(53, 66));
            RectTransform rewardFrame = view.QuestRewardFrame.rectTransform;
            Vector2 rewardPosition = view.QuestFrame.rectTransform.anchoredPosition + rewardFrame.anchoredPosition;
            Assert.That(rewardPosition.x + 189f / 1095 * rewardFrame.rect.width, Is.EqualTo(612).Within(.01f));
            Assert.That(rewardPosition.y - 193f / 1437 * rewardFrame.rect.height, Is.EqualTo(-38).Within(.01f));
            Assert.That(712f / 1095 * rewardFrame.rect.width, Is.EqualTo(114).Within(.01f));
            Assert.That(1053f / 1437 * rewardFrame.rect.height, Is.EqualTo(132).Within(.01f),
                "Reward frame visible lower edge must end at referencey1620.");
            Assert.That(view.QuestProgressFill.type, Is.EqualTo(Image.Type.Filled));
            Assert.That(view.QuestProgressFill.fillAmount, Is.Zero);
            Assert.That(view.QuestProgressFill.gameObject.activeSelf, Is.False);
            Assert.That(view.HasQuestMeasurement, Is.False);
            Assert.That(view.ProfileProgressText.text, Is.EqualTo("—"));
            Assert.That(view.QuestProgressText.text, Is.EqualTo("— / —"));
            Assert.That(view.QuestRewardText.text, Is.EqualTo("—"));
        }

        [Test]
        public void ConnectedFooterIsWholeNoninteractiveArtBehindTheExistingControls()
        {
            ProductionHomeView view = LoadHome().GetComponent<ProductionHomeView>();
            ProductionHomeLayout layout = view.GetComponent<ProductionHomeLayout>();
            Assert.That(view.FooterBase, Is.Not.Null);
            Assert.That(view.FooterBase.sprite, Is.SameAs(AssetDatabase.LoadAssetAtPath<Sprite>(
                ResourcesRoot + ProductionHomeRuntime.ArtRoot + "footer_base_generated.png")));
            Assert.That(view.FooterBase.type, Is.EqualTo(Image.Type.Simple));
            Assert.That(view.FooterBase.useSpriteMesh, Is.False);
            Assert.That(view.FooterBase.raycastTarget, Is.False);
            Assert.That(view.FooterBase.GetComponent<Mask>(), Is.Null);
            Assert.That(view.FooterBase.GetComponent<RectMask2D>(), Is.Null);
            Assert.That(view.FooterBase.transform.parent, Is.SameAs(layout.Footer));
            Assert.That(layout.FooterBase, Is.SameAs(view.FooterBase.rectTransform));
            Assert.That(view.FooterBase.transform.GetSiblingIndex(), Is.EqualTo(1));
            Assert.That(view.FooterBase.preserveAspect, Is.True);
            AssertRect(view.FooterBase.rectTransform, new Vector2(-4f * 941 / 2136, -(222 - 708f * 941 / 2136)),
                new Vector2(2143f * 941 / 2136, 734f * 941 / 2136));
            Assert.That(layout.FooterBacking.GetSiblingIndex(), Is.Zero);
            AssertRect(layout.FooterBacking, new Vector2(0, -(222 - 362f * 941 / 2136)), new Vector2(941, 362f * 941 / 2136));
            Assert.That(layout.FooterBacking.GetComponent<ProductionHomePanel>().raycastTarget, Is.False);
            foreach (Transform control in new[] { view.MailButton.transform, view.QuestFrame.transform, view.DailyRewardButton.transform })
            {
                Assert.That(control.parent, Is.SameAs(layout.Footer));
                Assert.That(control.GetSiblingIndex(), Is.GreaterThan(view.FooterBase.transform.GetSiblingIndex()));
            }
            Assert.That(view.FooterBase.GetComponentsInChildren<Button>(true), Is.Empty);
        }

        [Test]
        public void ReferenceEnvironmentAndCombinedHeroReplaceHomeOnlyWhileLegacyIdsRemainAvailable()
        {
            ProductionHomeView view = LoadHome().GetComponent<ProductionHomeView>();
            Assert.That(view.Environment.sprite, Is.SameAs(AssetDatabase.LoadAssetAtPath<Sprite>(
                ResourcesRoot + ProductionHomeRuntime.ArtRoot + "home_environment_generated.png")));
            AssertRect(view.Environment.rectTransform, Vector2.zero, new Vector2(941, 1672));
            Assert.That(view.Firebird, Is.Not.Null, "Retain the prior native Image rather than deleting its prefab identity.");
            Assert.That(view.Firebird.gameObject.activeSelf, Is.False);
            Assert.That(view.Firebird.sprite, Is.Null, "The removed R5 foreground must not remain a prefab dependency.");
            Assert.That(view.Ivan.sprite, Is.SameAs(AssetDatabase.LoadAssetAtPath<Sprite>(
                ResourcesRoot + ProductionHomeRuntime.ArtRoot + "home_hero_firebird.png")));
            Assert.That(view.LowerShade.color.a, Is.Zero, "Do not darken the new reference-colored flame tail with the historical winter-page wash.");
            Transform portrait = view.ProfileButton.transform.Find("PortraitClip");
            Assert.That(portrait, Is.Not.Null);
            Assert.That(portrait.gameObject.activeSelf, Is.True);
            Assert.That(portrait.GetComponent<Mask>().enabled, Is.True);
            Assert.That(portrait.parent, Is.SameAs(view.AvatarFrame.transform.parent));
            Assert.That(portrait.GetSiblingIndex(), Is.LessThan(view.AvatarFrame.transform.GetSiblingIndex()),
                "The complete gold avatar ring must draw after the masked independent portrait.");
            Image portraitArt = portrait.Find("ApprovedIvanPortrait").GetComponent<Image>();
            Assert.That(portraitArt.sprite, Is.SameAs(AssetDatabase.LoadAssetAtPath<Sprite>(
                ResourcesRoot + ProductionHomeRuntime.ArtRoot + "avatar_portrait.png")));
            Assert.That(portraitArt.type, Is.EqualTo(Image.Type.Simple));
            Assert.That(portraitArt.useSpriteMesh, Is.False);
            ProductionHomeArtFade oldFade = view.Ivan.GetComponent<ProductionHomeArtFade>();
            Assert.That(oldFade == null || !oldFade.enabled, Is.True);
            SplashTitleCharacterFade fade = view.Ivan.GetComponent<SplashTitleCharacterFade>();
            Assert.That(fade, Is.Not.Null);
            Assert.That(fade.enabled, Is.True);
            Assert.That(fade.BottomFraction, Is.EqualTo(.08f), "Only the final lower8% blends away the generated source cutoff.");
        }

        [Test]
        public void ResourceCapsulesCompensateWholeSourceMarginsAndRetainIndependentLiveControls()
        {
            ProductionHomeView home = LoadHome().GetComponent<ProductionHomeView>();
            Image[] capsules = { home.GoldCapsule, home.GemsCapsule, home.EnergyCapsule };
            Text[] numbers = { home.GoldText, home.GemsText, home.EnergyText };
            Button[] pluses = { home.GoldPlusButton, home.GemsPlusButton, home.EnergyPlusButton };
            foreach (Image capsule in capsules)
            {
                RectTransform root = capsule.transform.parent as RectTransform;
                Assert.That(root.GetComponent<Image>().sprite, Is.Null);
                Assert.That(capsule.raycastTarget, Is.False);
                Assert.That(capsule.transform.GetSiblingIndex(), Is.Zero);
                Vector2 position = capsule.rectTransform.anchoredPosition, size = capsule.rectTransform.sizeDelta;
                Assert.That(position.x + 29f / 2172 * size.x, Is.EqualTo(0).Within(.01f));
                Assert.That(position.y - 101f / 724 * size.y, Is.EqualTo(0).Within(.01f));
                Assert.That(size.x * 2114 / 2172, Is.EqualTo(root.rect.width).Within(.01f));
                Assert.That(size.y * 517 / 724, Is.EqualTo(53).Within(.01f));
                int index = Array.IndexOf(capsules, capsule);
                Assert.That(numbers[index].transform.parent, Is.SameAs(root));
                Assert.That(pluses[index].transform.parent, Is.SameAs(root));
                Assert.That(root.Find("Icon").GetSiblingIndex(), Is.GreaterThan(capsule.transform.GetSiblingIndex()));
            }
        }

        [Test]
        public void HomeReferenceIdentityUsesIndependentWholeSpritesAndLivePlayText()
        {
            ProductionHomeView home = LoadHome().GetComponent<ProductionHomeView>();
            GameObject splashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ResourcesRoot + ProductionSplashTitleRuntime.PrefabResource + ".prefab");
            SplashTitleView splash = splashPrefab.GetComponent<SplashTitleView>();
            Image splashIvan = splashPrefab.transform.Find("Ivan").GetComponent<Image>();
            Image splashLogo = splashPrefab.transform.Find("Logo").GetComponent<Image>();
            Image splashStart = splash.StartButton.targetGraphic as Image;
            Assert.That(home.Ivan.sprite, Is.Not.SameAs(splashIvan.sprite));
            Assert.That(splashLogo.sprite.name, Is.EqualTo("source_logo"), "Splash uses its separately selected original; Home remains unchanged.");
            Assert.That(splashStart.sprite.name, Is.EqualTo("source_start_blank"));
            // Keep the earlier approved files/GUIDs available; only the active bindings change.
            const string oldSplash = ResourcesRoot + "ProductionUI/SplashTitleReconstructed/";
            Assert.That(AssetDatabase.AssetPathToGUID(oldSplash + "slavic_fantasy_prince_warrior_portrait.png"), Is.EqualTo("629fd7194cacf7bcdac70a030fb986c9"));
            Assert.That(AssetDatabase.AssetPathToGUID(oldSplash + "legends_of_rus_ornate_crest.png"), Is.EqualTo("7711f55e3129f27655998da59ef0850c"));
            Assert.That(AssetDatabase.AssetPathToGUID(oldSplash + "ornate_emerald_fantasy_ui_button.png"), Is.EqualTo("aeb48fef0c4c9e73d19949a9a131cf95"));
            foreach (Image art in new[] { home.Ivan, home.Logo, home.PlayButton.targetGraphic as Image })
            {
                Assert.That(AssetDatabase.GetAssetPath(art.sprite), Does.StartWith(ResourcesRoot + ProductionHomeRuntime.ArtRoot));
                Assert.That(art.color, Is.EqualTo(art == home.Ivan ? ProductionHomeRuntime.HeroTone : Color.white));
                Assert.That(art.type, Is.EqualTo(Image.Type.Simple));
                Assert.That(art.preserveAspect, Is.EqualTo(art != home.PlayButton.targetGraphic));
                Assert.That(art.useSpriteMesh, Is.False);
                Assert.That(art.sprite.rect.position, Is.EqualTo(Vector2.zero));
                Assert.That(art.sprite.rect.size, Is.EqualTo(new Vector2(art.sprite.texture.width, art.sprite.texture.height)));
            }
            Assert.That(home.PlayText.text, Is.EqualTo("ИГРАТЬ"));
            Assert.That(home.PlayText.raycastTarget, Is.False);
            Assert.That(home.PlayText.font == splash.StartText.font, Is.True, "Keep the project's existing font assignment without claiming an exact reference font.");
            Assert.That(home.PlayText.fontStyle, Is.EqualTo(FontStyle.Bold));
            Assert.That(home.PlayText.GetComponent<ProductionHomeGoldText>(), Is.Not.Null);
            Assert.That(home.PlayText.GetComponent<ProductionHomeGoldText>().enabled, Is.True);
            Assert.That(home.PlayText.GetComponent<ProductionHomeGoldText>().BeveledFace, Is.True);
            Material playGold = AssetDatabase.LoadAssetAtPath<Material>(ResourcesRoot + ProductionHomeRuntime.PlayGoldMaterialResource + ".mat");
            Assert.That(playGold, Is.Not.Null);
            Assert.That(home.PlayText.material, Is.SameAs(playGold));
            Assert.That(playGold.shader.name, Is.EqualTo("LegendsOfRus/UI/HomePlayGold"));
            Assert.That(ShaderUtil.ShaderHasError(playGold.shader), Is.False, "The serialized Play shader must compile.");
            Assert.That(home.GetComponentsInChildren<Text>(true).Count(t => t.material == playGold), Is.EqualTo(1),
                "Only Play may use the new material; accepted badges and other text stay unchanged.");
            Assert.That(AssetDatabase.GetDependencies(PrefabPath, true),
                Does.Contain(AssetDatabase.GetAssetPath(playGold.shader)), "Prefab loading must include its shader dependency.");
            Assert.That(home.QuestText.fontStyle, Is.EqualTo(FontStyle.Bold));
            SplashTitleTextGeometry geometry = home.PlayText.GetComponent<SplashTitleTextGeometry>();
            Assert.That(geometry, Is.Not.Null);
            Assert.That(geometry.enabled && geometry.CenterHorizontally && geometry.CenterVertically && geometry.SnapToScreenPixels, Is.True);
            Shadow shadow = home.PlayText.GetComponent<Shadow>();
            Assert.That(shadow, Is.Not.Null);
            Component[] effects = home.PlayText.GetComponents<Component>();
            Assert.That(Array.IndexOf(effects, geometry), Is.LessThan(Array.IndexOf(effects, shadow)),
                "Optical glyph alignment must precede the retained restrained emboss shadow.");
            Assert.That(home.Logo.transform.GetSiblingIndex(), Is.GreaterThan(home.GetComponent<ProductionHomeLayout>().Hud.GetSiblingIndex()),
                "The reference crest's upper ornament overlaps the lower HUD edge.");
        }

        [Test]
        public void ReferenceEventMarkerIsLiveNoninteractiveDecorationWithinTheExistingEventControl()
        {
            ProductionHomeView home = LoadHome().GetComponent<ProductionHomeView>();
            Assert.That(home.EventsBadge.transform.parent, Is.SameAs(home.EventsButton.transform));
            Assert.That(home.EventsBadge.raycastTarget, Is.False);
            Assert.That(home.EventsBadge.Frame, Is.True);
            Assert.That(home.EventsBadge.Radius, Is.EqualTo(23));
            Assert.That(home.EventsBadge.gameObject.activeSelf, Is.True);
            AssertRect(home.EventsBadge.rectTransform, new Vector2(146, 5), new Vector2(46, 46));
            Assert.That(home.EventsBadgeText.text, Is.EqualTo("!"));
            Assert.That(home.EventsBadgeText.raycastTarget, Is.False);
            Assert.That(home.EventsBadge.GetComponentsInChildren<Button>(true), Is.Empty);
            foreach (ProductionHomePanel badge in new[] { home.EventsBadge, home.DailyRewardBadge })
            {
                Assert.That(badge.RaisedBadge, Is.True);
                Assert.That(badge.raycastTarget, Is.False);
            }
            AssertRect(home.DailyRewardBadge.rectTransform, new Vector2(132, 0), new Vector2(44, 44));
            foreach (Text label in new[] { home.EventsBadgeText, home.DailyRewardBadgeText })
            {
                Assert.That(label.raycastTarget, Is.False);
                Assert.That(label.GetComponent<Shadow>(), Is.Not.Null);
                Assert.That(label.GetComponent<ProductionHomeGoldText>(), Is.Not.Null);
                Assert.That(label.GetComponent<ProductionHomeGoldText>().BeveledFace, Is.False);
                Component[] effects = label.GetComponents<Component>();
                Assert.That(Array.IndexOf(effects, label.GetComponent<SplashTitleTextGeometry>()),
                    Is.LessThan(Array.IndexOf(effects, label.GetComponent<Shadow>())));
            }
            Assert.That(home.GetComponentsInChildren<ProductionHomePanel>(true).Count(p => p.RaisedBadge), Is.EqualTo(2),
                "The shared panel renderer's new style must apply only to the Event and Gift badges.");
            Assert.That(home.Buttons().Length, Is.EqualTo(12));
        }

        static void AssertRect(RectTransform rect, Vector2 position, Vector2 size)
        {
            Assert.That(Vector2.Distance(rect.anchoredPosition, position), Is.LessThan(.01f), rect.name + " position");
            Assert.That(Vector2.Distance(rect.sizeDelta, size), Is.LessThan(.01f), rect.name + " size");
            Assert.That(rect.anchorMin, Is.EqualTo(new Vector2(0, 1)), rect.name);
            Assert.That(rect.anchorMax, Is.EqualTo(new Vector2(0, 1)), rect.name);
            Assert.That(rect.pivot, Is.EqualTo(new Vector2(0, 1)), rect.name);
        }
        static GameObject LoadHome()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, "Run the focused Home owner before delivery checks.");
            return prefab;
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
