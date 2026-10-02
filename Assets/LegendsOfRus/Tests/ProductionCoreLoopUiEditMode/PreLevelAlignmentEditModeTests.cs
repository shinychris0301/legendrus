#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class PreLevelAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab";
        [Test]
        public void CanonicalPageUsesIndividualControlsAndWholeArt()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.PreLevel),Is.EqualTo("ProductionUI/Pages/PreLevel"));
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            var view=prefab.GetComponent<ProductionPreLevelView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);
            Assert.That(prefab.GetComponent<ProductionPreLevelLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<ProductionHomeView>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(view.BoosterButtons.Distinct().Count(),Is.EqualTo(4));
            Assert.That(view.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(13));
            Assert.That(view.BoosterDetailsRoot.gameObject.activeSelf,Is.False);
            Assert.That(view.BoostersHeading.text,Is.EqualTo("БУСТЕРЫ В ИГРЕ"));
            foreach(var graphic in view.GetComponentsInChildren<ProductionWorldMapGraphic>(true))
                Assert.That(graphic.GetComponent<CanvasRenderer>(),Is.Not.Null,graphic.name);
            foreach(Image image in prefab.GetComponentsInChildren<Image>(true))
            {
                if(image.sprite==null)continue;
                Assert.That(image.sprite.rect.x,Is.Zero);Assert.That(image.sprite.rect.y,Is.Zero);
                Assert.That(image.sprite.rect.size,Is.EqualTo(new Vector2(image.sprite.texture.width,image.sprite.texture.height)),image.name);
            }
            foreach(string dependency in AssetDatabase.GetDependencies(Path,true))
            {
                Assert.That(dependency,Does.Not.Contain("ReferenceOnly"));Assert.That(dependency,Does.Not.Contain("DesignScreens"));
                Assert.That(dependency,Does.Not.Contain("/Assets/Fix01/"));
            }
        }

        [Test]
        public void HomeHudResourcesAndTypographyAreSharedWithoutChangingHome()
        {
            var map=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionPreLevelView>();
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(home.Revision,Is.EqualTo(ProductionHomeView.CurrentRevision));Assert.That(home.HasRequiredBindings,Is.True);
            Assert.That(map.Typography,Is.SameAs(home.Typography));
            Assert.That(map.Hud.Find("ReferenceArtwork").GetComponent<Image>().sprite,Is.SameAs(home.HudFrame.sprite));
            Assert.That(map.ProfileButton.targetGraphic.GetComponent<Image>().sprite,Is.SameAs(home.AvatarFrame.sprite));
            Assert.That((map.SettingsButton.targetGraphic as Image).sprite,Is.SameAs((home.SettingsButton.targetGraphic as Image).sprite));
            Assert.That(map.TitleText.material,Is.SameAs(Graphic.defaultGraphicMaterial));
            Assert.That(map.TitleText.GetComponent<ProductionHomeGoldText>().enabled,Is.False);
            var chapter=Resources.Load<GameObject>("ProductionUI/Pages/ChapterMap").GetComponent<ProductionChapterMapView>();
            Assert.That(map.SourcePlay.sprite,Is.SameAs(chapter.SourcePlay.sprite));
            Assert.That(map.PlayText.material,Is.SameAs(Graphic.defaultGraphicMaterial));
            Assert.That(map.Environment.sprite,Is.SameAs(Resources.Load<Sprite>(ProductionPreLevelSourceControls.ArtRoot+"environment")));
            Assert.That(map.Environment.color,Is.EqualTo(Color.white));
            Assert.That(map.TitlePlaque.sprite,Is.SameAs(chapter.TitlePlaque.sprite));
            Assert.That((map.BackButton.targetGraphic as Image).sprite,Is.SameAs(chapter.SourceBack.sprite));
            Assert.That(map.Header.HasBindings,Is.True);
            foreach(var button in map.GetComponentsInChildren<Button>(true))Assert.That(button.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null);

        }

        [Test]
        public void ChapterMapImportsUseWholeUnpackedSpritesAndStableAlpha()
        {
            foreach(string name in ProductionPreLevelSourceControls.ArtNames)
            {
                string path="Assets/LegendsOfRus/Resources/"+ProductionPreLevelSourceControls.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer,Is.Not.Null);Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single));
                Assert.That(importer.textureShape,Is.EqualTo(TextureImporterShape.Texture2D));
                Assert.That(Resources.Load<Sprite>(ProductionPreLevelSourceControls.ArtRoot+name),Is.Not.Null);
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
    }
}
#endif
