#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class RewardReceivedAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/RewardClaim.prefab";
        [Test]public void NativeModalHasNoScreenshotDependencyAndStartsWithoutAReceipt()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionRewardReceivedView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.HasReceipt,Is.False);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.RewardClaim),Is.EqualTo("ProductionUI/Pages/RewardClaim"));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(1));Assert.That(v.Shade.raycastTarget,Is.True);
            foreach(string path in AssetDatabase.GetDependencies(Path,true)){Assert.That(path,Does.Not.Contain("DesignScreens"));Assert.That(path,Does.Not.Contain("ReferenceOnly"));}
            foreach(var t in root.GetComponentsInChildren<Text>(true))Assert.That(t.raycastTarget,Is.False);
            foreach(string name in new[]{"panel_r2","chest_r2","chest_glow_r2","title_ru_r2","button_ru_r2","button_blank_r2","card_r2"})
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionRewardReceivedRuntime.ArtRoot+name+".png");
                Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.mipmapEnabled,Is.False);Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
        [Test]public void RepeatedOwnerBuildKeepsButtonAndHierarchyIdentity()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                var v=root.GetComponent<ProductionRewardReceivedView>();var button=v.Confirm;int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionRewardReceivedRuntime.BuildInto((RectTransform)root.transform);ProductionRewardReceivedRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(v.Confirm,Is.SameAs(button));Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));Assert.That(v.HasReceipt,Is.False);Assert.That(v.ChestGlow.transform.GetSiblingIndex(),Is.LessThan(v.Chest.transform.GetSiblingIndex()));
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
