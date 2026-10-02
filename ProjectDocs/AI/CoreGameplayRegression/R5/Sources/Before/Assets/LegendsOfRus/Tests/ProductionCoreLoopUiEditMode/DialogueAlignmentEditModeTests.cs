#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class DialogueAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Dialogue.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionDialogueView>();Assert.That(v.HasRequiredBindings,Is.True);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(1));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 Assert.That(ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.Dialogue),Is.True);
 }
 [Test]public void BothLanguagesCoverEveryStorySceneWithoutAdvancingItsCursor(){
 foreach(var lang in new[]{"ru","en"})foreach(var scene in StoryCatalog.LoadChapter1().scenes){
 var localized=StoryCatalog.LoadDialogueLanguage(lang);var sequence=new StorySequenceRuntime(scene,StoryCatalog.LoadRussian());
 for(int i=0;i<scene.lines.Length;i++){var line=sequence.LocalizeCurrentLine(localized);Assert.That(line,Is.Not.Null);Assert.That(line.Text,Is.Not.Empty);Assert.That(line.Speaker,Is.Not.Empty);Assert.That(StoryCatalog.Localize(scene.lines[i].textKey,localized),Is.Not.Empty);Assert.That(sequence.LineIndex,Is.EqualTo(i));sequence.Advance();}
 }
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionDialogueRuntime.BuildInto((RectTransform)root.transform);ProductionDialogueRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
