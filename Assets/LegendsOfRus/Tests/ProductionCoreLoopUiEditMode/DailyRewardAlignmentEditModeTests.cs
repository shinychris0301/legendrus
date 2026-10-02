#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class DailyRewardAlignmentEditModeTests
    {
        [Test]public void NativePrefabHasSevenRewardsAndNoScreenshotDependencies()
        {
            const string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/DailyReward.prefab";
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionDailyRewardView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.DailyReward),Is.EqualTo("ProductionUI/Pages/DailyReward"));
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(8));Assert.That(v.ConfirmButton.GetComponent<Image>().sprite.name,Is.EqualTo("confirm_ru_r4"));Assert.That(v.ClaimButton.GetComponent<Image>().sprite.name,Is.EqualTo("claim_ru_r4"));Assert.That(v.ClaimText.enabled||v.ConfirmText.enabled,Is.False);Assert.That(v.Cards.Length,Is.EqualTo(7));Assert.That(v.PlayerHeader.HasBindings,Is.True);Assert.That(v.Header.sprite.name,Is.EqualTo("header_ru_r3"));Assert.That(v.Cards[6].sprite.name,Is.EqualTo("day7_surface_r5"));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
            foreach(var dep in AssetDatabase.GetDependencies(path,true)){Assert.That(dep,Does.Not.Contain("DesignScreens"));Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("Wave02"));}
            var instance=UnityEngine.Object.Instantiate(root);try{int count=instance.GetComponentsInChildren<Transform>(true).Length;ProductionDailyRewardRuntime.BuildInto((RectTransform)instance.transform);ProductionDailyRewardRuntime.BuildInto((RectTransform)instance.transform);Assert.That(instance.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));var rebuilt=instance.GetComponent<ProductionDailyRewardView>();Assert.That(rebuilt.MonthTitle.transform.parent.Find("CountBadge").GetSiblingIndex(),Is.LessThan(rebuilt.MonthCount.transform.GetSiblingIndex()));Assert.That(rebuilt.Cards[6].transform.Find("DaySevenPlaque").GetSiblingIndex(),Is.LessThan(rebuilt.DayLabels[6].transform.GetSiblingIndex()));}finally{UnityEngine.Object.DestroyImmediate(instance);}
        }
        [Test]public void SevenClaimsGrantExactRewardsAndWrapWithoutRepeatClaims()
        {
            var s=new PlayerSaveData{gold=0,gems=0,energy=0,energyMax=5,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0};var now=new DateTime(2026,9,1);
            for(int i=0;i<7;i++){Assert.That(DailyRewardSchedule.Day(s,now.AddDays(i)),Is.EqualTo(i+1));Assert.That(DailyRewardSchedule.TryApply(s,now.AddDays(i)),Is.True);string snapshot=JsonUtility.ToJson(s);Assert.That(DailyRewardSchedule.TryApply(s,now.AddDays(i).AddHours(23)),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(snapshot));s=JsonUtility.FromJson<PlayerSaveData>(snapshot);}
            Assert.That(s.gold,Is.EqualTo(1500));Assert.That(s.gems,Is.EqualTo(60));Assert.That(s.energy,Is.EqualTo(1));Assert.That(s.boosterMace,Is.EqualTo(2));Assert.That(s.boosterFirebird,Is.EqualTo(1));Assert.That(s.boosterLivingLight,Is.EqualTo(1));Assert.That(s.boosterSeaPearl,Is.EqualTo(1));Assert.That(s.dailyRewardMonthClaims,Is.EqualTo(7));Assert.That(DailyRewardSchedule.Day(s,now.AddDays(7)),Is.EqualTo(1));
        }
        [Test]public void MissedDayRestartsLegacySaveContinuesAndClockRollbackCannotClaim()
        {
            var now=new DateTime(2026,9,16);var s=new PlayerSaveData{lastDailyRewardUtc="20260914",dailyRewardCycleDay=5};Assert.That(DailyRewardSchedule.Day(s,now),Is.EqualTo(1));
            s=JsonUtility.FromJson<PlayerSaveData>("{\"lastDailyRewardUtc\":\"20260915\"}");Assert.That(DailyRewardSchedule.Day(s,now),Is.EqualTo(2));Assert.That(DailyRewardSchedule.TryApply(s,now),Is.True);
            Assert.That(DailyRewardSchedule.CanClaim(s,now.AddDays(-1)),Is.False);Assert.That(DailyRewardSchedule.TryApply(s,now.AddDays(-1)),Is.False);
        }
        [Test]public void MonthRolloverAndEnergyCapRemainConsistent()
        {
            var s=new PlayerSaveData{lastDailyRewardUtc="20260930",dailyRewardCycleDay=2,dailyRewardMonthUtc="202609",dailyRewardMonthClaims=20,energy=5,energyMax=5};var now=new DateTime(2026,10,1);
            Assert.That(DailyRewardSchedule.MonthClaims(s,now),Is.Zero);Assert.That(DailyRewardSchedule.TryApply(s,now),Is.True);Assert.That(s.dailyRewardCycleDay,Is.EqualTo(3));Assert.That(s.energy,Is.EqualTo(5));Assert.That(s.dailyRewardMonthClaims,Is.EqualTo(1));
        }
        [Test]public void OverflowDoesNotPartiallyGrantOrConsumeTheDay()
        {
            var s=new PlayerSaveData{gold=int.MaxValue};string before=JsonUtility.ToJson(s);Assert.Throws<OverflowException>(()=>DailyRewardSchedule.TryApply(s,new DateTime(2026,9,16)));Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));
        }
    }
}
#endif
