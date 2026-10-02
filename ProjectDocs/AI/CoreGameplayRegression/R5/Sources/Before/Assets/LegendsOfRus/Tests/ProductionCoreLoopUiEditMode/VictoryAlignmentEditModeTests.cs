#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class VictoryAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab";
        [Test] public void CanonicalPageHasWholeSpritesLiveTextAndNativeControls()
        {
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Victory),Is.EqualTo("ProductionUI/Pages/Victory"));
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Path);var view=prefab.GetComponent<ProductionVictoryView>();
            Assert.That(view,Is.Not.Null);Assert.That(view.HasRequiredBindings,Is.True);
            Assert.That(view.Revision,Is.EqualTo(11));
            Assert.That(view.Hud.gameObject.activeSelf,Is.False);
            Assert.That(view.Title.GetComponent<ProductionVictoryTitleLettering>().ArcHeight,Is.Zero);
            Assert.That(view.ContinueButton.GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("continue_ru")));
            Assert.That(view.DoubleButton.GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("video")));
            Assert.That(view.FooterOrnament.sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("footer")));
            Assert.That(view.Reaction.transform.parent,Is.SameAs(view.Footer));
            Assert.That(view.Status.transform.parent,Is.SameAs(view.Footer));
            Assert.That(view.ResourceHeader.sprite,Is.SameAs(Resources.Load<Sprite>(ProductionVictoryRuntime.ArtRoot+"resource_header_r3")));
            Assert.That(view.Firebird.sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("firebird")));
            Assert.That(view.EnergyTimer.raycastTarget,Is.False);
            Assert.That(view.RewardsPanel.sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("panel_ru")));
            Assert.That(view.Body.Find("VictoryBanner").GetComponent<Image>().sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("title_ru")));
            Assert.That(view.Title.GetComponents<ProductionVictoryTitleLettering>().Length,Is.EqualTo(1));
            Assert.That(view.Title.GetComponent<ProductionVictoryTitleLettering>().enabled,Is.True);
            Assert.That(view.Title.GetComponent<ProductionHomeGoldText>().enabled,Is.False);
            Assert.That(view.Title.GetComponent<Shadow>().enabled,Is.False);
            Assert.That(view.ContinueText.GetComponent<ProductionHomeGoldText>().enabled,Is.True);
            Assert.That(view.ContinueText.GetComponent<ProductionHomeGoldText>().BeveledFace,Is.True);
            Assert.That(view.ContinueText.GetComponent<Shadow>().enabled,Is.True);
            foreach(var halo in view.StarGlows)
            {
                Assert.That(halo.GetComponents<ProductionVictoryStarHalo>().Length,Is.EqualTo(1));
                Assert.That(halo.GetComponent<ProductionVictoryStarHalo>().enabled,Is.True);
                Assert.That(halo.raycastTarget,Is.False);
            }
            Assert.That(view.RewardsPanel.GetComponent<ProductionVictoryPanelFit>(),Is.Not.Null);
            foreach(var card in view.RewardCards)
                Assert.That(card.GetComponent<ProductionVictoryPanelFit>().RewardCard,Is.True);
            foreach(var text in view.GetComponentsInChildren<Text>(true))
            {
                if(text==view.Title||text==view.ContinueText||text==view.Score)continue;
                Assert.That(text.GetComponent<SplashTitleTextGeometry>().SnapToScreenPixels,Is.True);
                var gold=text.GetComponent<ProductionHomeGoldText>();
                if(gold!=null)Assert.That(gold.enabled,Is.False,"Small lettering must not carry the bevel overlay.");
            }
            AssertHalosBehindEveryStar(view);
            Assert.That(prefab.GetComponent<ProductionVictoryLayout>().View,Is.SameAs(view));
            Assert.That(prefab.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(5));
            foreach(var image in prefab.GetComponentsInChildren<Image>(true))
            {if(image==view.MessageInset){Assert.That(image.sprite,Is.Null);Assert.That(image.raycastTarget,Is.False);continue;}Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(image.sprite.rect,Is.EqualTo(new Rect(0,0,image.sprite.texture.width,image.sprite.texture.height)),image.name);}
            foreach(var path in AssetDatabase.GetDependencies(Path,true))
            {Assert.That(path,Does.Not.Contain("ReferenceOnly"));Assert.That(path,Does.Not.Contain("DesignScreens"));}
            var source=Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>();
            Assert.That(view.Environment.sprite,Is.SameAs(ProductionVictoryReferenceStyle.Load("environment")));Assert.That(view.Environment.color,Is.EqualTo(Color.white));
            Assert.That(view.Typography,Is.SameAs(source.Typography));
        }
        [Test] public void NewSpritesUseUncompressedSingleImportWithTrueAlpha()
        {
            foreach(string name in ProductionVictoryReferenceStyle.ArtNames)
            {
                var importer=AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionVictoryReferenceStyle.Root+name+".png") as TextureImporter;
                Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single));Assert.That(importer.alphaIsTransparency,Is.True);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
                Assert.That(Resources.Load<Sprite>(ProductionVictoryReferenceStyle.Root+name),Is.Not.Null);
                if(true)
                {
                    importer.GetSourceTextureWidthAndHeight(out int width,out int height);
                    var texture=Resources.Load<Sprite>(ProductionVictoryReferenceStyle.Root+name).texture;
                    Assert.That(texture.width,Is.EqualTo(width),name+" must retain source width.");
                    Assert.That(texture.height,Is.EqualTo(height),name+" must retain source height.");
                    var android=importer.GetPlatformTextureSettings("Android");
                    Assert.That(android.overridden,Is.True);Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));
                    Assert.That(android.maxTextureSize,Is.GreaterThanOrEqualTo(Mathf.Max(width,height)));
                    Assert.That(importer.mipmapEnabled,Is.False);
                }
            }
        }
        [Test] public void ReusingSerializedContentDoesNotDuplicateTheNativePage()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                int effects=root.GetComponentsInChildren<BaseMeshEffect>(true).Length;
                ProductionVictoryRuntime.BuildInto(root.transform as RectTransform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                Assert.That(root.GetComponentsInChildren<BaseMeshEffect>(true).Length,Is.EqualTo(effects));
                Assert.That(root.GetComponentsInChildren<ProductionVictoryTitleLettering>(true).Length,Is.EqualTo(1));
                AssertHalosBehindEveryStar(root.GetComponent<ProductionVictoryView>());
            }
            finally{Object.DestroyImmediate(root);}
        }

        static void AssertHalosBehindEveryStar(ProductionVictoryView view)
        {
            Assert.That(view.Title.transform.parent,Is.SameAs(view.Body));
            foreach(var halo in view.StarGlows)
            {
                Assert.That(halo.transform.parent,Is.SameAs(view.Body));
                Assert.That(halo.transform.GetSiblingIndex(),Is.GreaterThan(view.Title.transform.GetSiblingIndex()));
                foreach(var star in view.Stars)
                {
                    Assert.That(star.transform.parent,Is.SameAs(view.Body));
                    Assert.That(halo.transform.GetSiblingIndex(),Is.LessThan(star.transform.GetSiblingIndex()),
                        halo.name+" must render behind "+star.name+" so a neighboring halo cannot wash over its finished face.");
                }
            }
        }
    }
}
#endif
