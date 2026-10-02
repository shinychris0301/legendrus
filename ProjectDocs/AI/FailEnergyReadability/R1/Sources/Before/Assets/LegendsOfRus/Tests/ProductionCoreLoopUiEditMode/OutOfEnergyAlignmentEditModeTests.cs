#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class OutOfEnergyAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/OutOfEnergy.prefab";
        [Test]public void NativeModalHasNoScreenshotDependencyAndStartsUnbound()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionOutOfEnergyView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.ActionsBound,Is.False);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.OutOfEnergy),Is.EqualTo("ProductionUI/Pages/OutOfEnergy"));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(4));Assert.That(v.Shade.raycastTarget,Is.True);
            foreach(string path in AssetDatabase.GetDependencies(Path,true)){Assert.That(path,Does.Not.Contain("DesignScreens"));Assert.That(path,Does.Not.Contain("ReferenceOnly"));}
            foreach(var t in root.GetComponentsInChildren<Text>(true))Assert.That(t.raycastTarget,Is.False);
            foreach(string name in new[]{"panel_r2","medallion_r2","badge_r2","title_ru_r2","energy_r2","ask_ru_r2","watch_ru_r2","buy_blank_r2","ask_blank_r2","watch_blank_r2","close","friends","ad","hourglass"})
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionOutOfEnergyRuntime.ArtRoot+name+".png");
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
        [TestCase(0, "00:00")][TestCase(59.1, "01:00")][TestCase(60, "01:00")][TestCase(1800, "30:00")]
        public void CountdownRoundsUpRealSeconds(double seconds,string expected)
        {Assert.That(ProductionOutOfEnergyView.FormatRemaining(System.TimeSpan.FromSeconds(seconds)),Is.EqualTo(expected));}
        [Test]public void RepeatedOwnerBuildKeepsButtonAndHierarchyIdentity()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                var v=root.GetComponent<ProductionOutOfEnergyView>();var button=v.Close;int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionOutOfEnergyRuntime.BuildInto((RectTransform)root.transform);ProductionOutOfEnergyRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(v.Close,Is.SameAs(button));Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));Assert.That(v.ActionsBound,Is.False);
                Assert.That(v.CountBacking.transform.GetSiblingIndex(),Is.LessThan(v.Count.transform.GetSiblingIndex()));
                Assert.That(v.Count.GetComponentInParent<ProductionPreLevelBadge>().enabled,Is.False);
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
