#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class ProfileAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionProfileView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.HasOnlineProfile,Is.False);Assert.That(v.Header,Is.Not.Null);Assert.That(root.GetComponentsInChildren<ProductionPlayerHeader>(true).Length,Is.EqualTo(1));
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.GreaterThanOrEqualTo(17));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(v.InfoPanel.activeSelf,Is.False);
 }
 [Test]public void SaveSnapshotSanitizesDuplicatesWithoutMutation(){
 var save=new PlayerSaveData{levelProgress=new System.Collections.Generic.List<LevelProgressData>{null,new LevelProgressData{chapterId=0,levelId=1},new LevelProgressData{chapterId=1,levelId=2,bestStars=99,attempts=-4,bestScore=-1},new LevelProgressData{chapterId=1,levelId=2,bestStars=1,attempts=9,bestScore=500},new LevelProgressData{chapterId=1,levelId=1,bestStars=-8}}};string before=JsonUtility.ToJson(save);var rows=ProductionProfileView.ReadRecords(save);Assert.That(rows.Count,Is.EqualTo(2));Assert.That(rows[0].levelId,Is.EqualTo(2));Assert.That(rows[0].bestStars,Is.EqualTo(3));Assert.That(rows[0].attempts,Is.EqualTo(9));Assert.That(rows[0].bestScore,Is.EqualTo(500));Assert.That(rows[1].bestStars,Is.Zero);rows[0].bestScore=0;Assert.That(JsonUtility.ToJson(save),Is.EqualTo(before));Assert.That(ProductionProfileView.ReadRecords(null),Is.Empty);
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionProfileRuntime.BuildInto((RectTransform)root.transform);ProductionProfileRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
