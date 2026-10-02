#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class LoginAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Login.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionLoginView>();Assert.That(v.HasRequiredBindings,Is.True);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.GreaterThanOrEqualTo(11));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 Assert.That(v.Revision,Is.EqualTo(ProductionLoginView.CurrentRevision));foreach(var b in root.GetComponentsInChildren<Button>(true)){Assert.That(b.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null);Assert.That(b.transition,Is.EqualTo(Selectable.Transition.None));}
 Assert.That(v.Identity.text,Is.Empty);Assert.That(v.Password.text,Is.Empty);Assert.That(v.Password.contentType,Is.EqualTo(InputField.ContentType.Password));Assert.That(v.OnlineAuthenticationAvailable,Is.False);Assert.That(v.InfoPanel.activeSelf,Is.False);
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionLoginRuntime.BuildInto((RectTransform)root.transform);ProductionLoginRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
