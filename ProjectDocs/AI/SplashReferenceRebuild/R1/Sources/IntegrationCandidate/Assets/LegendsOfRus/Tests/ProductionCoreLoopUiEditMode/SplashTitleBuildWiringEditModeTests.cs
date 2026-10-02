#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class SplashTitleBuildWiringEditModeTests
    {
        const string PrefabPath = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab";
        GameObject fixtureRoot;

        [TearDown]
        public void RemoveIsolatedClone()
        {
            if (fixtureRoot != null) Object.DestroyImmediate(fixtureRoot);
        }

        [Test]
        public void SavedCanonicalPrefabPassesBuildWiringValidation()
        {
            GameObject prefab = LoadCanonicalPrefab();
            Assert.That(EditorUtility.IsPersistent(prefab), Is.True);
            Assert.That(Validate(Type.EmptyTypes, new object[0]), Is.Empty);
        }

        [Test]
        public void InactiveCloneRetainsFiveValidButtonArtworkTargets()
        {
            GameObject clone = CreateInactiveClone();
            Button[] buttons = clone.GetComponentsInChildren<Button>(true);
            Assert.That(buttons.Length, Is.EqualTo(5));
            foreach (Button button in buttons)
            {
                Assert.That(button.targetGraphic, Is.Not.Null);
                Assert.That(button.targetGraphic.transform.IsChildOf(button.transform), Is.True);
                Assert.That(button.targetGraphic.raycastTarget, Is.True);
                Assert.That(button.gameObject.activeInHierarchy, Is.False);
            }
            Assert.That(ValidateClone(clone), Is.Empty);
        }

        [Test]
        public void InactiveCloneStillRejectsRaycastDecorationOutsideButtons()
        {
            GameObject clone = CreateInactiveClone();
            Image source = clone.GetComponentInChildren<Image>(true);
            Assert.That(source, Is.Not.Null);
            Assert.That(source.sprite, Is.Not.Null);
            var decoration = new GameObject("StrayDecoration", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Image));
            decoration.transform.SetParent(clone.transform, false);
            Image image = decoration.GetComponent<Image>();
            image.sprite = source.sprite;
            image.preserveAspect = true;
            image.raycastTarget = true;

            Assert.That(ValidateClone(clone), Is.EquivalentTo(new[]
            {
                "Decorative graphic intercepts input: StrayDecoration"
            }));
        }

        [Test]
        public void SplashUsesSelectedOriginalWithLiveStartAndPreservesLoadingBranding()
        {
            const string resources="Assets/LegendsOfRus/Resources/";
            GameObject splashRoot=LoadCanonicalPrefab();var splash=splashRoot.GetComponent<SplashTitleView>();
            var home=AssetDatabase.LoadAssetAtPath<GameObject>(resources+"ProductionUI/Pages/Home.prefab").GetComponent<ProductionHomeView>();
            var loading=AssetDatabase.LoadAssetAtPath<GameObject>(resources+"ProductionUI/Pages/Loading.prefab").GetComponent<ProductionLoadingView>();
            Assert.That(splash.HasRequiredBindings&&loading.HasRequiredBindings,Is.True);
            Assert.That(splashRoot.transform.Find("Logo").GetComponent<Image>().sprite.name,Is.EqualTo("source_logo"));
            Assert.That(splashRoot.transform.Find("Ivan").GetComponent<Image>().sprite.name,Is.EqualTo("source_ivan_firebird"));
            Assert.That(splashRoot.transform.Find("SourceOuterFrame").GetComponent<Image>().raycastTarget,Is.False);
            Assert.That(splashRoot.transform.Find("Masha").gameObject.activeSelf,Is.False);
            Assert.That(splashRoot.transform.Find("Firebird").gameObject.activeSelf,Is.False);
            Assert.That(splash.StartButton.targetGraphic.GetComponent<Image>().sprite.name,Is.EqualTo("source_start_blank"));
            Assert.That(splash.StartText.GetComponent<ProductionHomeGoldText>().BeveledFace,Is.True);
            Assert.That(splash.StartText.text,Is.EqualTo("НАЖМИТЕ, ЧТОБЫ НАЧАТЬ"));
            Assert.That(splash.ReferenceTitleFont,Is.Not.Null);Assert.That(splash.ReferenceVersionFont,Is.Not.Null);
            foreach(var button in new[]{splash.LanguageButton,splash.SettingsButton,splash.AccountButton,splash.HelpButton})Assert.That(button.gameObject.activeSelf,Is.False,"Selected source exposes only Start; auxiliary routes remain elsewhere");
            Assert.That(loading.Logo.sprite,Is.SameAs(home.Logo.sprite));
            Assert.That(loading.Environment.sprite.name,Is.EqualTo("loading_environment_dusk"));
            Assert.That(loading.Ivan.sprite.name,Is.EqualTo("ivan_loading_dusk"));
        }

        [TestCase("masha_dusk", true)]
        [TestCase("ivan_splash_dusk", true)]
        [TestCase("ivan_loading_dusk", true)]
        [TestCase("loading_environment_dusk", false)]
        public void NewDuskArtUsesCompleteFinalSourcesWithVerifiedTransparency(string name, bool character)
        {
            string assetPath = "Assets/LegendsOfRus/Resources/ProductionUI/FrontEndHomeAligned/" + name + ".png";
            byte[] asset = File.ReadAllBytes(assetPath);
            CollectionAssert.AreEqual(File.ReadAllBytes("ProjectDocs/AI/FrontEndHomeAlignment/R1/Sources/Final/" + name + ".png"), asset);
            if (character) Assert.That(asset[25], Is.EqualTo(6), "Character PNG must carry actual RGBA, not a baked RGB checkerboard.");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            Assert.That(sprite, Is.Not.Null);
            Assert.That(sprite.rect.position, Is.EqualTo(Vector2.zero));
            Assert.That(sprite.rect.size, Is.EqualTo(new Vector2(sprite.texture.width, sprite.texture.height)));
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single));
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.alphaSource, Is.EqualTo(TextureImporterAlphaSource.FromInput));
            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            Assert.That(sprite.texture.width, Is.EqualTo(width));
            Assert.That(sprite.texture.height, Is.EqualTo(height));
        }

        GameObject CreateInactiveClone()
        {
            // Instantiate beneath an inactive parent so no runtime OnEnable callback runs.
            fixtureRoot = new GameObject("SplashWiringTestRoot");
            fixtureRoot.SetActive(false);
            GameObject clone = Object.Instantiate(LoadCanonicalPrefab(), fixtureRoot.transform, false);
            clone.SetActive(false);
            Assert.That(clone.activeInHierarchy, Is.False);
            return clone;
        }

        static GameObject LoadCanonicalPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, "The accepted canonical prefab must already exist.");
            return prefab;
        }

        static List<string> ValidateClone(GameObject clone)
        {
            return Validate(new[] { typeof(GameObject) }, new object[] { clone });
        }

        static List<string> Validate(Type[] argumentTypes, object[] arguments)
        {
            // An asmdef test assembly cannot directly reference the predefined Editor assembly.
            Type validator = Type.GetType(
                "LegendsOfRus.Editor.SplashTitleR3Validation, Assembly-CSharp-Editor", true);
            MethodInfo method = validator.GetMethod("ValidateWiring", BindingFlags.Public | BindingFlags.Static,
                null, argumentTypes, null);
            Assert.That(method, Is.Not.Null);
            return (List<string>)method.Invoke(null, arguments);
        }
    }
}
#endif
