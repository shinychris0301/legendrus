#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class TeamHubAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamHub.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionTeamHubView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.HasLiveTeam,Is.False);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(14));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(v.InfoPanel.activeSelf,Is.False);
 Assert.That(v.Header.HasBindings,Is.True);Assert.That(v.Header.Balances[2],Is.SameAs(v.EnergyText));
 Assert.That(v.Header.Backing.sprite,Is.SameAs(Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponentInChildren<ProductionPlayerHeader>(true).Backing.sprite));
 for(int i=0;i<4;i++){Assert.That(v.NavButtons[i].image.sprite.name,Is.EqualTo("nav_"+i+"_ru"));Assert.That(v.NavLabels[i].enabled,Is.False);}
 for(int i=0;i<3;i++)Assert.That(v.TaskLabels[i].transform.parent.Find("RewardGem").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Gem(1)));
 Assert.That(v.ProgressFill.transform.parent.GetComponent<ProductionHomePanel>().enabled,Is.False,"No rectangular backing outside arrow-shaped rail");
 Assert.That(v.ProgressFill.transform.parent.Find("EmptyChannel"),Is.Not.Null);
 Assert.That(v.TaskLabels[2].transform.parent.Find("Icon").GetComponent<Image>().sprite,Is.SameAs(ProductionBoosterArt.Icon(3)));
 }
 [Test]public void SummaryAndTasksHaveIndependentOrderedOrnaments(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamHubView>();
 Assert.That(v.Body.Find("MemberGroupIcon").GetComponent<Image>().sprite.name,Is.EqualTo("members_icon_r2"));
 Assert.That(v.Body.Find("GuildChestEnclosure").GetSiblingIndex(),Is.LessThan(v.ChestButton.transform.GetSiblingIndex()));
 Assert.That(v.ChestButton.image.sprite,Is.SameAs(Resources.Load<Sprite>("ProductionUI/TeamHubAligned/chest")));
 Assert.That(v.Body.Find("TasksEnclosure").GetSiblingIndex(),Is.LessThan(v.TasksHeading.transform.GetSiblingIndex()));
 Assert.That(v.ChestLabel.enabled,Is.False);Assert.That(v.TasksHeading.enabled,Is.False);
 foreach(var label in v.TaskCounts){var card=label.transform.parent;Assert.That(card.Find("CountPlaque").GetSiblingIndex(),Is.LessThan(label.transform.GetSiblingIndex()));
 var reward=card.Find("RewardPlaque").GetComponent<Image>();Assert.That(reward.rectTransform.anchoredPosition.y,Is.EqualTo(-111));Assert.That(reward.sprite.name,Is.EqualTo("task_reward_plaque_r2"));}
 }
 [Test]public void RosterUsesSourceGuildBadgesAndPlainLowerRanks(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamHubView>();
 for(int i=0;i<5;i++){
 var row=v.MemberRows[i];var badge=row.Find("RankIcon").GetComponent<Image>();
 Assert.That(badge.gameObject.activeSelf,Is.EqualTo(i<3));Assert.That(v.MemberRanks[i].enabled,Is.EqualTo(i>=3));
 if(i<3)Assert.That(badge.sprite.name,Is.EqualTo("guild_medal_"+i+"_r3"));
 Assert.That(row.Find("Frame").GetComponent<Image>().sprite.name,Is.EqualTo(i==0?"roster_leader_r3":"roster_member_r3"));
 Assert.That(row.Find("PowerIcon").GetComponent<Image>().sprite,Is.SameAs(Resources.Load<Sprite>("ProductionUI/TeamHubAligned/power")));
 Assert.That(v.Portraits[i].sprite.name,Does.StartWith("portrait_"));
 Assert.That(v.MemberNames[i].gameObject.activeSelf,Is.True);
 }
 }
 [Test]public void FooterHasSourceCrestAndLocalizedMottoWithNativeInfo(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamHubView>();
 Assert.That(v.Body.Find("FooterCrest").GetComponent<Image>().sprite.name,Is.EqualTo("footer_crest_r4"));
 Assert.That(v.Body.Find("MottoFrame").GetComponent<Image>().sprite.name,Is.EqualTo("motto_ru_r4"));Assert.That(v.Motto.enabled,Is.False);
 Assert.That(v.GuildInfoButton.image.sprite.name,Is.EqualTo("guild_info_r4"));Assert.That(v.GuildInfoButton.image.raycastTarget,Is.True);
 Assert.That(v.Body.Find("MottoFrame").GetSiblingIndex(),Is.LessThan(v.Body.Find("FooterCrest").GetSiblingIndex()));
 Assert.That(v.Body.Find("FooterCrest").GetSiblingIndex(),Is.LessThan(v.InfoPanel.transform.GetSiblingIndex()));
 }
 [Test]public void SeasonFillUsesApprovedSapphireAndRemainsBehindLiveCount(){
 var v=AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionTeamHubView>();
 Assert.That(v.SeasonFill.sprite,Is.SameAs(Resources.Load<Sprite>("ProductionUI/LoadingReferenceAligned/source_progress_fill")));
 Assert.That(v.ProgressFill.enabled,Is.False);Assert.That(v.SeasonFill.type,Is.EqualTo(Image.Type.Filled));
 Assert.That(v.SeasonFill.fillMethod,Is.EqualTo(Image.FillMethod.Horizontal));Assert.That(v.SeasonFill.fillOrigin,Is.Zero);
 Assert.That(v.SeasonFill.fillAmount,Is.EqualTo(.756f).Within(.0001f));Assert.That(v.Progress.text,Is.EqualTo("7 560 / 10 000"));
 Assert.That(v.SeasonFill.transform.GetSiblingIndex(),Is.LessThan(v.Progress.transform.GetSiblingIndex()));
 Assert.That(v.Power.alignment,Is.EqualTo(TextAnchor.MiddleLeft));
 Assert.That(v.BodyFont,Is.SameAs(Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames")));
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionTeamHubRuntime.BuildInto((RectTransform)root.transform);ProductionTeamHubRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));var v=root.GetComponent<ProductionTeamHubView>();
 Assert.That(v.Body.Find("GuildChestEnclosure").GetSiblingIndex(),Is.LessThan(v.ChestButton.transform.GetSiblingIndex()));
 Assert.That(v.Body.Find("TasksEnclosure").GetSiblingIndex(),Is.LessThan(v.TasksHeading.transform.GetSiblingIndex()));
 foreach(var label in v.TaskCounts)Assert.That(label.transform.parent.Find("CountPlaque").GetSiblingIndex(),Is.LessThan(label.transform.GetSiblingIndex()));
 Assert.That(v.Body.Find("CarvedFooter").GetSiblingIndex(),Is.LessThan(v.BackButton.transform.GetSiblingIndex()));
 Assert.That(v.Body.Find("GuildSummaryBacking").GetSiblingIndex(),Is.LessThan(v.GuildName.transform.GetSiblingIndex()));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
