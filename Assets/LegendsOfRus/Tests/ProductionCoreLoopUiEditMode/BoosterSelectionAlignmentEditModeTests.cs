#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class BoosterSelectionAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/BoosterSelect.prefab";
        [Test]public void NativeModalHasNoScreenshotDependencyAndStartsUnbound()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionBoosterSelectionView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.ActionsBound,Is.False);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.BoosterSelect),Is.EqualTo("ProductionUI/Pages/BoosterSelect"));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(6));Assert.That(v.Shade.raycastTarget,Is.True);
            foreach(string path in AssetDatabase.GetDependencies(Path,true)){Assert.That(path,Does.Not.Contain("DesignScreens"));Assert.That(path,Does.Not.Contain("ReferenceOnly"));}
            foreach(var t in root.GetComponentsInChildren<Text>(true))Assert.That(t.raycastTarget,Is.False);
            foreach(string name in new[]{"panel_r2","play_ru_r2","play_blank_r2","title_ru_r2","preview_0_r2","preview_1_r2","preview_2_r2","preview_3_r2","close"})
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionBoosterSelectionRuntime.ArtRoot+name+".png");
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
        [Test]public void RepeatedOwnerBuildKeepsButtonAndHierarchyIdentity()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                var v=root.GetComponent<ProductionBoosterSelectionView>();var button=v.Close;int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionBoosterSelectionRuntime.BuildInto((RectTransform)root.transform);ProductionBoosterSelectionRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(v.Close,Is.SameAs(button));Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));Assert.That(v.ActionsBound,Is.False);
                Assert.That(v.HeadingLettering.transform.GetSiblingIndex(),Is.GreaterThan(v.Panel.transform.GetSiblingIndex()));
                foreach(var icon in v.PreviewIcons)
                {Assert.That(icon.sprite,Is.Not.Null);Assert.That(icon.raycastTarget,Is.False);Assert.That(icon.GetComponent<Button>(),Is.Null);}
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
