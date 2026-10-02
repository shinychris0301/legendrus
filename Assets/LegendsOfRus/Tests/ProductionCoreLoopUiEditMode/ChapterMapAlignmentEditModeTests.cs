#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ChapterMapAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab";
        [Test]
        public void CanonicalMapIsSerializedWithRecycledNodesAndWholeArt()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.ChapterMap),Is.EqualTo("ProductionUI/Pages/ChapterMap"));
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            var view=prefab.GetComponent<ProductionChapterMapView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);
            Assert.That(prefab.GetComponent<ProductionChapterMapLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<ProductionHomeView>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(view.Nodes.Select(x=>x.Button).Distinct().Count(),Is.EqualTo(14));
            Assert.That(view.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(22));
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
            var map=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionChapterMapView>();
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(home.Revision,Is.EqualTo(ProductionHomeView.CurrentRevision));Assert.That(home.HasRequiredBindings,Is.True);
            Assert.That(map.Typography,Is.SameAs(home.Typography));
            Assert.That(map.Hud.Find("ReferenceArtwork").GetComponent<Image>().sprite,Is.SameAs(home.HudFrame.sprite));
            Assert.That(map.ProfileButton.targetGraphic.GetComponent<Image>().sprite,Is.SameAs(home.AvatarFrame.sprite));
            Assert.That((map.SettingsButton.targetGraphic as Image).sprite,Is.SameAs((home.SettingsButton.targetGraphic as Image).sprite));
            Assert.That(map.Header.HasBindings,Is.True);Assert.That(map.SourceControls,Is.True);
            Assert.That(map.Environment.sprite,Is.SameAs(Resources.Load<Sprite>(ProductionChapterMapSourceControls.ArtRoot+"environment_worldmap01_landmarks")));
            Assert.That(map.Environment.color,Is.EqualTo(Color.white));
            Assert.That(map.HeroName.font,Is.SameAs(home.HeroName.font));
            Assert.That(map.SourcePlay.sprite.name,Is.EqualTo("play"));
            Assert.That(map.SourceBack.sprite.name,Is.EqualTo("back"));
            foreach(var node in map.Nodes){Assert.That(node.SourceStars.Length,Is.EqualTo(3));Assert.That(node.Number.font,Is.SameAs(Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers")));}
            Assert.That(map.PlayText.enabled,Is.False);
            Assert.That(ProductionChapterMapScroller.PlannedLevels,Is.EqualTo(500));
            Assert.That(map.Scroller.Scenery.Length,Is.EqualTo(3));
            Assert.That(map.Scroller.GetComponent<RectMask2D>(),Is.Not.Null);
        }

        [Test]
        public void ChapterMapImportsUseWholeUnpackedSpritesAndStableAlpha()
        {
            foreach(string name in ProductionChapterMapSourceControls.ArtNames)
            {
                string path="Assets/LegendsOfRus/Resources/"+ProductionChapterMapSourceControls.ArtRoot+name+".png";
                var importer=AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer,Is.Not.Null);Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single));
                Assert.That(importer.textureShape,Is.EqualTo(TextureImporterShape.Texture2D));
                Assert.That(Resources.Load<Sprite>(ProductionChapterMapSourceControls.ArtRoot+name),Is.Not.Null);
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
    }
}
#endif
