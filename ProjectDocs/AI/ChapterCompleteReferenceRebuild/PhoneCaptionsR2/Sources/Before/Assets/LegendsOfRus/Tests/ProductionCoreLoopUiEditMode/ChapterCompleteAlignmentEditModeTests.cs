#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class ChapterCompleteAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab";
        [Test] public void CanonicalChapterCompleteUsesIsolatedArtNativeControlsAndNoCapturedScreen()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);var view=root.GetComponent<ProductionChapterCompleteView>();
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.Revision,Is.EqualTo(8));
            Assert.That(view.Environment.sprite.name,Is.EqualTo("environment_source_r8"));
            Assert.That(view.Body.Find("NextHeadingOriginal").GetComponent<Image>().raycastTarget,Is.False);Assert.That(root.GetComponent<ProductionChapterCompleteLayout>().View,Is.SameAs(view));
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.ChapterComplete),Is.EqualTo("ProductionUI/Pages/ChapterComplete"));
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(2));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(root.transform.Find("FIX01_CONTENT"),Is.Null);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var n in ProductionChapterCompleteRuntime.ArtNames)
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionChapterCompleteRuntime.ArtRoot+n+".png");
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(importer.alphaIsTransparency,Is.True);
                Assert.That(importer.GetPlatformTextureSettings("Android").format,Is.EqualTo(TextureImporterFormat.RGBA32));
            }
        }
        [Test] public void ReusingChapterCompleteDoesNotDuplicateObjectsAndMissingRoundHasNoSampleGoals()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionChapterCompleteRuntime.BuildInto((RectTransform)root.transform);ProductionChapterCompleteRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                var v=root.GetComponent<ProductionChapterCompleteView>();foreach(var b in v.Actions)Assert.That(b.interactable,Is.False);

            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
