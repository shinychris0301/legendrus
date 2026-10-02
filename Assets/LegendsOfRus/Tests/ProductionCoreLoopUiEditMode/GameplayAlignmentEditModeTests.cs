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
                Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.Revision,Is.EqualTo(ProductionGameplayView.CurrentRevision));
                Assert.That(view.BoosterButtons,Is.EqualTo(controls));Assert.That(view.BoardMount,Is.SameAs(boardMount));
                Assert.That(instance.GetComponentsInChildren<Transform>(true),Is.EquivalentTo(transforms));
                Assert.That(view.Hud.Find("HudPanel").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Load("hud_header_r3")));
                Assert.That(view.Footer.Find("ScorePanel").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Load("score_panel_r3")));
                Assert.That(view.Status.HasRequiredBindings,Is.True);Assert.That(view.Status.IsVisible,Is.False);
                Assert.That(view.GetComponents<ProductionGameplayStatus>().Length,Is.EqualTo(1));
                Assert.That(view.Goals.All(g=>g.Symbol!=null),Is.True);
                Assert.That(view.BoardArea.GetChild(0).name,Is.EqualTo("BoardLacquer"));
                Assert.That(view.BoardArea.GetChild(view.BoardArea.childCount-1).name,Is.EqualTo("BoardFrame"));
                var frame=view.BoardArea.Find("BoardFrame");
                Assert.That(frame.GetComponent<ProductionGameplayCell>().enabled,Is.False,"Only the carved frame should render above the board.");
                var carvedFrame=frame.Find("CarvedFrame").GetComponent<Image>();
                Assert.That(carvedFrame.sprite,Is.SameAs(ProductionGameplaySourceControls.Load("board")));
                Assert.That(carvedFrame.raycastTarget,Is.False,"The decorative frame must not intercept edge-cell input.");
                Assert.That(view.Hud.Find("JoinedHeaderBacking").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplaySourceControls.Load("header")));
                Assert.That(view.Hud.Find("MissionParchment").GetComponent<Image>().enabled,Is.False,"No independently masked parchment box.");
                Assert.That(view.Hud.Find("TopScorePanel").GetComponent<Image>().enabled,Is.False,"No independently masked score box.");
                var inset=view.Footer.Find("ScoreInset").GetComponent<ProductionGameplayCell>();
                Assert.That(inset.BronzeSurface&&inset.InsetPanel,Is.True);Assert.That(inset.enabled,Is.False,"Dedicated footer contains the opaque score well.");Assert.That(inset.raycastTarget,Is.False);
                Assert.That(view.ScoreValue.transform.parent,Is.SameAs(view.Hud));
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
            var retired=new[]{view.GoalProgress,view.StarProgress,
                view.Hud.Find("GoalProgressFrame").GetComponent<Image>(),view.Hud.Find("StarProgressFrame").GetComponent<Image>()};
            foreach(var image in prefab.GetComponentsInChildren<Image>(true))
            {
                if(retired.Contains(image))
                { Assert.That(image.sprite,Is.Null,image.name);Assert.That(image.gameObject.activeSelf,Is.False,image.name); }
                else if(!(image is ProductionGameplayCell))Assert.That(image.sprite,Is.Not.Null,image.name);
            }
            var clip=prefab.transform.Find(ProductionGameplayRuntime.ContentName+"/GameplayHud/PortraitClip");Assert.That(clip.GetComponent<Mask>(),Is.Not.Null);
        }
        [Test] public void SourceStarsUseDistinctTexturesAndKeepOtherPagesNative()
        {
            var view=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionGameplayView>();
            foreach(var star in view.Stars)
            {
                Assert.That(star.StarEmptyArt,Is.Not.Null);Assert.That(star.StarFilledArt,Is.Not.Null);
                Assert.That(star.StarEmptyArt.texture,Is.Not.SameAs(star.StarFilledArt.texture));
            }
            foreach(string page in new[]{"Home","WorldMap","ChapterMap"})
            foreach(var graphic in Resources.Load<GameObject>("ProductionUI/Pages/"+page).GetComponentsInChildren<ProductionWorldMapGraphic>(true))
            {Assert.That(graphic.StarEmptyArt,Is.Null,page);Assert.That(graphic.StarFilledArt,Is.Null,page);}
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
            var home=Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>();Assert.That(home.Revision,Is.EqualTo(ProductionHomeView.CurrentRevision));
            Assert.That(Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>().Revision,Is.EqualTo(ProductionPreLevelView.CurrentRevision));
            var view=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionGameplayView>();Assert.That(view.Typography,Is.SameAs(home.Typography));
        }

        [Test] public void CarvedBoardFrameHasATransparentWindowInsteadOfAnOpaqueReferencePlate()
        {
            var sprite=ProductionGameplaySourceControls.Load("board");
            var path=AssetDatabase.GetAssetPath(sprite.texture);
            var pixels=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try
            {
                Assert.That(pixels.LoadImage(File.ReadAllBytes(path)),Is.True);
                var colors=pixels.GetPixels32();
                int left=pixels.width/5,right=pixels.width*4/5;
                int bottom=pixels.height/5,top=pixels.height*4/5;
                for(int y=bottom;y<top;y++)for(int x=left;x<right;x++)
                    if(colors[y*pixels.width+x].a>2)
                        Assert.Fail("The board frame contains opaque pixels over the playable window at "+x+","+y+".");
            }
            finally{Object.DestroyImmediate(pixels);}
        }
    }
}
#endif
