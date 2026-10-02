#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class TutorialAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/TutorialOverlay.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionTutorialView>();Assert.That(v.HasRequiredBindings,Is.True);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.GreaterThanOrEqualTo(18));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 Assert.That(v.Revision,Is.EqualTo(ProductionTutorialView.CurrentRevision));foreach(var b in root.GetComponentsInChildren<Button>(true)){Assert.That(b.GetComponent<ProductionMapButtonFeedback>(),Is.Not.Null);Assert.That(b.transition,Is.EqualTo(Selectable.Transition.None));}
 Assert.That(v.Section,Is.EqualTo(0));Assert.That(ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.TutorialOverlay),Is.True);
 }
 [Test] public void ExampleIsOneNeighborSwapWithExactlyOneRedMatch(){
 int[] before=ProductionTutorialRuntime.MatchExample(false),after=ProductionTutorialRuntime.MatchExample(true);
 int changed=0;for(int i=0;i<9;i++)if(before[i]!=after[i])changed++;Assert.That(changed,Is.EqualTo(2));
 Assert.That(after[3],Is.EqualTo(before[4]));Assert.That(after[4],Is.EqualTo(before[3]));
 int beforeLines=0,afterLines=0;
 for(int i=0;i<3;i++){
 if(before[i*3]==before[i*3+1]&&before[i*3]==before[i*3+2])beforeLines++;
 if(before[i]==before[i+3]&&before[i]==before[i+6])beforeLines++;
 if(after[i*3]==after[i*3+1]&&after[i*3]==after[i*3+2])afterLines++;
 if(after[i]==after[i+3]&&after[i]==after[i+6])afterLines++;
 }
 Assert.That(beforeLines,Is.Zero);Assert.That(afterLines,Is.EqualTo(1));Assert.That(after[0],Is.Zero);Assert.That(after[3],Is.Zero);Assert.That(after[6],Is.Zero);
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionTutorialRuntime.BuildInto((RectTransform)root.transform);ProductionTutorialRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
