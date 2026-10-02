#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ShopAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Shop.prefab";
        [Test]public void ActiveShopIsNativeAndHasNoReferenceScreenDependencies()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionShopView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(v.Header,Is.Not.Null);Assert.That(v.Header.HasBindings,Is.True);
            Assert.That(v.Header.Backing.sprite.name,Is.EqualTo("source_header"));
            Assert.That(v.Hud.GetSiblingIndex(),Is.LessThan(v.Title.transform.GetSiblingIndex()),"Source title crest must render above the header backing.");
            Assert.That(v.Cards[0].GetComponent<Image>().sprite.name,Is.EqualTo("card_r2"));
            Assert.That(root.GetComponentsInChildren<ProductionPlayerHeader>(true).Length,Is.EqualTo(1));
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.Shop),Is.EqualTo("ProductionUI/Pages/Shop"));
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);
            Assert.That(root.GetComponent<ProductionShopLayout>().View,Is.SameAs(v));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/LegendsOfRus/Resources/ProductionUI/ShopAligned"}))
            {
                var ti=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(ti.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(ti.mipmapEnabled,Is.False);
                Assert.That(ti.GetPlatformTextureSettings("Android").format,Is.EqualTo(TextureImporterFormat.RGBA32));
            }
        }
        [Test]public void RepeatedBuildPreservesHierarchyAndNeverEnablesUnverifiedPurchases()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionShopRuntime.BuildInto((RectTransform)root.transform);
                var first=root.GetComponent<ProductionShopView>();
                Assert.That(first.Hud.GetSiblingIndex(),Is.LessThan(first.Title.transform.GetSiblingIndex()),"One rebuild must preserve crest layer order.");
                ProductionShopRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));var v=root.GetComponent<ProductionShopView>();
                for(int i=0;i<4;i++){Assert.That(v.CanPurchase(i),Is.False);Assert.That(v.PurchaseButtons[i].interactable,Is.False);}
                CollectionAssert.AreEqual(new[]{"starter","hero","firebird",null},ProductionShopView.OfferIds);
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
