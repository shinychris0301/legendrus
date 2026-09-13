#if UNITY_EDITOR
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class GameplayAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab";
        [Test] public void FramePolishReapplicationPreservesControlsAndCreatesNoDuplicateLayers()
        {
            var instance=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                var view=instance.GetComponent<ProductionGameplayView>();
                var controls=view.BoosterButtons.ToArray();var boardMount=view.BoardMount;
                var transforms=instance.GetComponentsInChildren<Transform>(true);
                ProductionGameplayRuntime.BuildInto((RectTransform)instance.transform);
                ProductionGameplayRuntime.BuildInto((RectTransform)instance.transform);
                Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.Revision,Is.EqualTo(2));
                Assert.That(view.BoosterButtons,Is.EqualTo(controls));Assert.That(view.BoardMount,Is.SameAs(boardMount));
                Assert.That(instance.GetComponentsInChildren<Transform>(true),Is.EquivalentTo(transforms));
                Assert.That(view.BoardArea.GetChild(0).name,Is.EqualTo("BoardLacquer"));
                Assert.That(view.BoardArea.GetChild(view.BoardArea.childCount-1).name,Is.EqualTo("BoardFrame"));
                Assert.That(view.Hud.Find("MovesMedallion").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Load("moves_frame")));
                var inset=view.Footer.Find("ScoreInset").GetComponent<ProductionGameplayCell>();
                Assert.That(inset.BronzeSurface&&inset.InsetPanel,Is.True);Assert.That(inset.raycastTarget,Is.False);
                Assert.That(inset.transform.GetSiblingIndex(),Is.LessThan(view.ScoreValue.transform.GetSiblingIndex()));
                Assert.That(instance.GetComponentsInChildren<ProductionGameplayStarPolish>(true).Length,Is.EqualTo(3));
                foreach(var page in new[]{"Home","SplashTitle","Loading","WorldMap","ChapterMap","PreLevel"})
                    Assert.That(Resources.Load<GameObject>("ProductionUI/Pages/"+page).GetComponentsInChildren<ProductionGameplayStarPolish>(true),Is.Empty,page);
            }
            finally{Object.DestroyImmediate(instance);}
        }
        [Test] public void CanonicalGameplayHasFiveNativeControlsAndNoReferenceScreenDependencies()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Gameplay),Is.EqualTo("ProductionUI/Pages/Gameplay"));
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);var view=prefab.GetComponent<ProductionGameplayView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);Assert.That(prefab.GetComponent<ProductionGameplayLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(prefab.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(5));
            Assert.That(view.BoosterButtons.Distinct().Count(),Is.EqualTo(4));
            Assert.That(prefab.GetComponent<ProductionReferenceMatchR3PageMarker>().PreservesLiveGameplayBoard,Is.True);
            foreach(var path in AssetDatabase.GetDependencies(Path,true))
            {
                Assert.That(path,Does.Not.Contain("ReferenceOnly"));Assert.That(path,Does.Not.Contain("DesignScreens"));Assert.That(path,Does.Not.Contain("/Assets/Fix01/"));
            }
            foreach(var image in prefab.GetComponentsInChildren<Image>(true))
                if(!(image is ProductionGameplayCell))Assert.That(image.sprite,Is.Not.Null,image.name);
            var clip=prefab.transform.Find(ProductionGameplayRuntime.ContentName+"/GameplayHud/PortraitClip");Assert.That(clip.GetComponent<Mask>(),Is.Not.Null);
        }
        [Test] public void UserSheetsRetainOriginalBytesAndHaveExactDeclaredSpriteRegions()
        {
            foreach(string sheet in new[]{"gems","boosters","obstacles"})
            {
                string path="Assets/LegendsOfRus/Resources/"+ProductionGameplayArt.Root+sheet+".png";
                using(var hash=SHA256.Create())Assert.That(hash.ComputeHash(File.ReadAllBytes(path)),Is.EqualTo(hash.ComputeHash(File.ReadAllBytes("ProjectDocs/AI/GameplayStyleAlignment/R1/Sources/"+sheet+".png"))));
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Multiple));Assert.That(importer.isReadable,Is.False);Assert.That(importer.mipmapEnabled,Is.False);
            }
            foreach(var region in ProductionGameplayArt.Regions)
            {
                var sprite=ProductionGameplayArt.Load(region.Name);Assert.That(sprite,Is.Not.Null);
                if(ProductionGameplayArt.HasIsolatedOverride(region.Name))
                {
                    Assert.That(sprite.rect,Is.EqualTo(new Rect(0,0,region.TopLeftRect.width,region.TopLeftRect.height)));continue;
                }
                Rect r=region.TopLeftRect;Assert.That(sprite.rect,Is.EqualTo(new Rect(r.x,1086-r.y-r.height,r.width,r.height)),region.Name);
            }
        }
        [Test] public void IndependentArtImportsAndPriorPageRevisionsRemainStable()
        {
            foreach(string name in ProductionGameplayArt.Files)
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionGameplayArt.Root+name+".png");
                Assert.That(importer,Is.Not.Null);Assert.That(importer.textureShape,Is.EqualTo(TextureImporterShape.Texture2D));Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.sRGBTexture,Is.True);Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();Assert.That(home.Revision,Is.EqualTo(14));
            Assert.That(Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>().Revision,Is.EqualTo(1));
            var view=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionGameplayView>();Assert.That(view.Typography,Is.SameAs(home.Typography));
        }
    }
}
#endif
