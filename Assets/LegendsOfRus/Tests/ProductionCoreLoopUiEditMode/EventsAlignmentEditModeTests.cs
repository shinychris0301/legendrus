#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class EventsAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab";
        [Test]public void ActiveEventsIsNativeAndHasNoReferenceScreenDependencies()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionEventsView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.Events),Is.EqualTo("ProductionUI/Pages/Events"));
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(13));
            var shop=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LegendsOfRus/Resources/ProductionUI/Pages/Shop.prefab").GetComponent<ProductionShopView>();
            Assert.That(v.Typography,Is.SameAs(shop.Typography));
            var home=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab").GetComponent<ProductionHomeView>();
            Assert.That(v.Environment.sprite,Is.SameAs(home.Environment.sprite),"Events must reuse Home's clean environment");
            Assert.That(v.Environment.color,Is.EqualTo(home.Environment.color));
            Assert.That(v.Body.Find("HeadingEnvironment").gameObject.activeSelf,Is.False,"No separate skyline over the shared Home background");
            Assert.That(v.Header,Is.Not.Null);Assert.That(v.Header.HasBindings,Is.True);
            Assert.That(v.Header.Backing.sprite,Is.SameAs(shop.Header.Backing.sprite));
            Assert.That(v.Header.Balances[2],Is.SameAs(v.EnergyText));
            Assert.That(v.Title.sprite.name,Is.EqualTo("title_original_r4"));
            Assert.That(v.BackButton.image.sprite,Is.SameAs(Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2")));
            Assert.That(root.GetComponent<ProductionEventsLayout>().View,Is.SameAs(v));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned"}))
            {
                var ti=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(ti.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(ti.mipmapEnabled,Is.False);
                Assert.That(ti.GetPlatformTextureSettings("Android").format,Is.EqualTo(TextureImporterFormat.RGBA32));
            }
        }
        [Test]public void RepeatedBuildPreservesHierarchyAndNeverEnablesUnscheduledEvents()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionEventsRuntime.BuildInto((RectTransform)root.transform);ProductionEventsRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));var v=root.GetComponent<ProductionEventsView>();
                for(int i=0;i<4;i++){Assert.That(v.CanParticipate(i),Is.False);Assert.That(v.DetailsButtons[i].interactable,Is.False);}
                Assert.That(v.Category,Is.EqualTo(EventsCategory.Upcoming));
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
