#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class WorldMapAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/WorldMap.prefab";
        [Test]
        public void CanonicalMapIsSerializedWithFifteenIndividualNodesAndWholeArt()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.WorldMap),Is.EqualTo("ProductionUI/Pages/WorldMap"));
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            var view=prefab.GetComponent<ProductionWorldMapView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);
            Assert.That(prefab.GetComponent<ProductionWorldMapLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<ProductionHomeView>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(view.Nodes.Select(x=>x.Button).Distinct().Count(),Is.EqualTo(15));
            Assert.That(view.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(25));
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
            var map=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionWorldMapView>();
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(home.Revision,Is.EqualTo(14));Assert.That(home.HasRequiredBindings,Is.True);
            Assert.That(map.Typography,Is.SameAs(home.Typography));
            Assert.That(map.Hud.Find("ReferenceArtwork").GetComponent<Image>().sprite,Is.SameAs(home.HudFrame.sprite));
            Assert.That(map.ProfileButton.targetGraphic.GetComponent<Image>().sprite,Is.SameAs(home.AvatarFrame.sprite));
            Assert.That(map.SettingsButton.GetComponent<Image>().sprite,Is.SameAs(home.SettingsButton.GetComponent<Image>().sprite));
            Assert.That(map.TitleText.material,Is.SameAs(home.PlayText.material));
        }

        [Test]
        public void WorldMapImportsUseWholeUnpackedSpritesAndStableAlpha()
        {
            foreach(string name in ProductionWorldMapRuntime.ArtNames)
            {
                string path="Assets/LegendsOfRus/Resources/"+ProductionWorldMapRuntime.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer,Is.Not.Null);Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single));
                Assert.That(importer.textureShape,Is.EqualTo(TextureImporterShape.Texture2D));
                Assert.That(Resources.Load<Sprite>(ProductionWorldMapRuntime.ArtRoot+name),Is.Not.Null);
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.EqualTo(name!="environment"));
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
    }
}
#endif
