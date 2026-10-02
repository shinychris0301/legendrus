#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class SettingsAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Settings.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionSettingsView>();Assert.That(v.HasRequiredBindings,Is.True);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.GreaterThanOrEqualTo(17));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(v.InfoPanel.activeSelf,Is.False);
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionSettingsRuntime.BuildInto((RectTransform)root.transform);ProductionSettingsRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
