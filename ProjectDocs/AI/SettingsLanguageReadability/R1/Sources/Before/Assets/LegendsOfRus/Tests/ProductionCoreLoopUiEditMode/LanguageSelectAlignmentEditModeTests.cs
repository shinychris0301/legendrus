#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class LanguageSelectAlignmentEditModeTests
    {
        const string Path = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/LanguageSelect.prefab";
        [Test] public void NativePageHasCompleteBindingsAndSourceSprites()
        {
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            Assert.That(root, Is.Not.Null); var v = root.GetComponent<ProductionLanguageSelectView>();
            Assert.That(v.HasRequiredBindings, Is.True);
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenId, Is.EqualTo(ScreenId.LanguageSelect));
            Assert.That(ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.LanguageSelect), Is.True);
            Assert.That(v.Backdrop.interactable || v.Backdrop.blocksRaycasts, Is.False);
            Assert.That(v.Body.GetComponentsInChildren<Button>().Length, Is.EqualTo(3));
            Assert.That(root.GetComponentsInChildren<Canvas>(true), Is.Empty);
            foreach (var dep in AssetDatabase.GetDependencies(Path,true))
            { Assert.That(dep, Does.Not.Contain("ReferenceOnly")); Assert.That(dep, Does.Not.Contain("DesignScreens")); }
            Assert.That(v.Panel.sprite.name, Does.StartWith("language_panel"));
        }
        [Test] public void BuilderPreservesExistingHierarchy()
        {
            var root = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try { int n = root.GetComponentsInChildren<Transform>(true).Length;
                ProductionLanguageSelectRuntime.BuildInto((RectTransform)root.transform);
                ProductionLanguageSelectRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length, Is.EqualTo(n)); }
            finally { Object.DestroyImmediate(root); }
        }
    }
}
#endif
