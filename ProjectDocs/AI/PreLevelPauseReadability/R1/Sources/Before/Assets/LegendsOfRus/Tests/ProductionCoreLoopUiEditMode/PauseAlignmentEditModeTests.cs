#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class PauseAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Pause.prefab";
        [Test] public void CanonicalPauseUsesIsolatedArtNativeControlsAndNoCapturedScreen()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);var view=root.GetComponent<ProductionPauseView>();
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.Revision,Is.EqualTo(2));
            Assert.That(view.Body.Find("MainPanel").GetComponent<Image>().sprite.name,Is.EqualTo("panel_r2"));
            Assert.That(view.Body.Find("GoalsPanel").GetComponent<Image>().sprite.name,Is.EqualTo("goals_r2"));Assert.That(root.GetComponent<ProductionPauseLayout>().View,Is.SameAs(view));
            Assert.That(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Pause),Is.EqualTo("ProductionUI/Pages/Pause"));
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(4));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(root.transform.Find("FIX01_CONTENT"),Is.Null);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var n in ProductionPauseRuntime.ArtNames)
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/LegendsOfRus/Resources/"+ProductionPauseRuntime.ArtRoot+n+".png");
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(importer.alphaIsTransparency,Is.True);
                Assert.That(importer.GetPlatformTextureSettings("Android").format,Is.EqualTo(TextureImporterFormat.RGBA32));
            }
        }
        [Test] public void ReusingPauseDoesNotDuplicateObjectsAndMissingRoundHasNoSampleGoals()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionPauseRuntime.BuildInto((RectTransform)root.transform);ProductionPauseRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                var v=root.GetComponent<ProductionPauseView>();foreach(var b in v.Actions)Assert.That(b.interactable,Is.False);
                foreach(var goal in v.Goals)Assert.That(goal.Root.gameObject.activeSelf,Is.False);
                Assert.That(v.Instructions.enabled,Is.True);Assert.That(v.Body.Find("InstructionsArtwork").gameObject.activeSelf,Is.False,"No instruction artwork over the missing-round message.");
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
