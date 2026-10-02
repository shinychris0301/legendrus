#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class TeamRankingAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamRanking.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionTeamRankingView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.HasLiveLeaderboard,Is.False);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(12));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(v.InfoPanel.activeSelf,Is.False);
 }
 [Test]public void SharedAssetsAndOriginalDetailsAreBound(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamRankingView>();
 Assert.That(v.Header.HasBindings,Is.True);Assert.That(v.Header.Balances[2],Is.SameAs(v.EnergyText));
 foreach(var podium in v.Podiums)Assert.That(podium.Find("RewardIcon_1").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Gem(1)));
 foreach(var row in v.Rows)Assert.That(row.Find("RewardIcon_1").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Gem(1)));
 Assert.That(v.DetailsButton.image.sprite.name,Is.EqualTo("details_ru_r7"));Assert.That(v.DetailsLabel.enabled,Is.False);
 Assert.That(v.Body.Find("CarvedFooter").GetSiblingIndex(),Is.LessThan(v.DetailsButton.transform.GetSiblingIndex()));
 }
 [Test]public void OriginalRewardEnclosuresReplaceOverlappingSlotFrames(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamRankingView>();
 foreach(var row in v.Rows){var e=row.Find("SourceRewardEnclosure");Assert.That(e.GetComponent<Image>().sprite.name,Is.EqualTo("reward_enclosure_r3"));Assert.That(e.GetSiblingIndex(),Is.LessThan(row.Find("RewardIcon_0").GetSiblingIndex()));for(int j=0;j<3;j++)Assert.That(row.Find("RewardSlot_"+j).GetComponent<Image>().enabled,Is.False);}
 foreach(var podium in v.Podiums){Assert.That(podium.Find("SourceFrameAndEmblem").GetComponent<Image>().sprite.name,Does.EndWith("_r5"));for(int j=0;j<3;j++)Assert.That(podium.Find("RewardSlot_"+j).GetComponent<Image>().enabled,Is.False);}
 }
 [Test]public void TableUsesContinuousSourceSurfaceAndThinDividers(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamRankingView>();
 Assert.That(v.Body.Find("TableFrame").GetComponent<Image>().sprite.name,Is.EqualTo("table_panel_r4"));
 for(int i=0;i<v.Rows.Length;i++){
 var frame=v.Rows[i].Find("Frame").GetComponent<Image>();Assert.That(frame.sprite.name,Is.EqualTo(i==3?"row_highlight_r4":"row_divider_r4"));
 if(i!=3)Assert.That(frame.rectTransform.rect.height,Is.LessThanOrEqualTo(3));
 Assert.That(v.Rows[i].Find("SourceRewardEnclosure").GetComponent<RectTransform>().sizeDelta,Is.EqualTo(new Vector2(220,61)));
 }
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionTeamRankingRuntime.BuildInto((RectTransform)root.transform);ProductionTeamRankingRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
