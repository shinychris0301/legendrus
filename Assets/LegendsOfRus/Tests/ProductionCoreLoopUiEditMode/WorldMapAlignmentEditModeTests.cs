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
        public void CanonicalMapIsSerializedWithTwentyScrollableCardsAndWholeArt()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.WorldMap),Is.EqualTo("ProductionUI/Pages/WorldMap"));
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            var view=prefab.GetComponent<ProductionWorldMapView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);
            Assert.That(prefab.GetComponent<ProductionWorldMapLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<ProductionHomeView>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(view.Cards.Select(x=>x.Button).Distinct().Count(),Is.EqualTo(20));
            Assert.That(view.ChapterScroll.vertical,Is.True);Assert.That(view.ChapterScroll.horizontal,Is.False);
            Assert.That(view.NavButtons.Length,Is.EqualTo(5));
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
        public void HomeAndWorldMapUseOneHeaderComponentAndIdenticalPresentation()
        {
            var map=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionWorldMapView>();
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();
            Assert.That(home.Revision,Is.EqualTo(ProductionHomeView.CurrentRevision));Assert.That(home.HasRequiredBindings,Is.True);
            Assert.That(map.Typography,Is.SameAs(home.Typography));
            Assert.That(map.Header,Is.Not.Null);Assert.That(home.Header,Is.Not.Null);
            Assert.That(map.Header.Backing.sprite,Is.SameAs(home.Header.Backing.sprite));
            Assert.That(map.Header.GetType(),Is.EqualTo(home.Header.GetType()));
            Assert.That(map.Header.PlayerName.fontSize,Is.EqualTo(home.Header.PlayerName.fontSize));
            Assert.That(map.Header.Level.fontSize,Is.EqualTo(home.Header.Level.fontSize));
            var left=new[]{map.Header.PlayerName,map.Header.Level,map.Header.Experience}.Concat(map.Header.Balances).ToArray();
            var right=new[]{home.Header.PlayerName,home.Header.Level,home.Header.Experience}.Concat(home.Header.Balances).ToArray();
            for(int i=0;i<left.Length;i++)
            {
                Assert.That(left[i].font,Is.SameAs(right[i].font));Assert.That(left[i].color,Is.EqualTo(right[i].color));
                Assert.That(left[i].rectTransform.anchoredPosition,Is.EqualTo(right[i].rectTransform.anchoredPosition));
                Assert.That(left[i].rectTransform.sizeDelta,Is.EqualTo(right[i].rectTransform.sizeDelta));
            }
            Assert.That(map.ProfileButton.transform.Find("Avatar").GetComponent<Image>().sprite,Is.SameAs(home.AvatarFrame.sprite));
            Assert.That(map.SettingsButton.GetComponent<Image>().sprite,Is.SameAs(home.SettingsButton.targetGraphic.GetComponent<Image>().sprite));
            Assert.That(map.HeroName.font,Is.SameAs(home.ReferenceBodyFont));
        }

        [Test]
        public void WorldMapImportsUseWholeUnpackedSpritesAndStableAlpha()
        {
            foreach(string path in System.IO.Directory.GetFiles("Assets/LegendsOfRus/Resources/"+ProductionWorldMapSourceComposition.ArtRoot,"*.png"))
            {
                var importer=AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer,Is.Not.Null);Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single));
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
            var map=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionWorldMapView>();
            foreach(var card in map.Cards)
            {
                Assert.That(card.Surface.color,Is.EqualTo(Color.white));Assert.That(card.Frame.color,Is.EqualTo(Color.white));
                Assert.That(card.Button.transition,Is.EqualTo(Selectable.Transition.None),"Card selection must never tint all artwork.");
                Assert.That(card.BorderGlow,Is.TypeOf<WorldMapPanelBorderHighlight>(),"Use panel geometry, not extracted ornament alpha.");
                Assert.That(card.Root.Find("CurrentBorderGlow")?.gameObject.activeSelf ?? false,Is.False);
                Assert.That(card.BorderGlow.raycastTarget,Is.False);
                Assert.That(card.BorderGlow.GetComponent<CanvasRenderer>(),Is.Not.Null);
            }
            // Geometry must leave the entire inner rectangle empty, without ornament-shaped patches.
            var border=map.Cards[0].BorderGlow;
            using(var mesh=new VertexHelper())
            {
                typeof(WorldMapPanelBorderHighlight).GetMethod("OnPopulateMesh",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.DeclaredOnly).Invoke(border,new object[]{mesh});
                Assert.That(mesh.currentVertCount,Is.EqualTo(48));
                Rect bounds=border.rectTransform.rect;
                for(int i=0;i<mesh.currentVertCount;i++)
                {
                    UIVertex vertex=new UIVertex();mesh.PopulateUIVertex(ref vertex,i);
                    float edge=Mathf.Min(vertex.position.x-bounds.xMin,bounds.xMax-vertex.position.x,vertex.position.y-bounds.yMin,bounds.yMax-vertex.position.y);
                    Assert.That(edge,Is.InRange(1f,15f));
                }
            }
        }
    }
}
#endif
