using System;
using NUnit.Framework;
using UnityEngine;
using static LegendsOfRus.Tests.Campaign.CampaignSaveSessionTests;
namespace LegendsOfRus.Tests.Campaign
{
    public sealed class CampaignBoosterTests
    {
        const long Now=638900000000000000;
        static MemoryStore Store()
        {
            var p=JsonUtility.FromJson<PlayerSaveData>(Legacy());p.lastEnergyUtcTicks=Now;
            var store=new MemoryStore();Assert.That(Session(store).ImportLegacyCopy(JsonUtility.ToJson(p)).Committed,Is.True);Assert.That(Session(store).StartRound(0,Now).Committed,Is.True);store.Crash();return store;
        }
        [TestCase(BoosterType.BogatyrMace)] [TestCase(BoosterType.Firebird)] [TestCase(BoosterType.LivingLight)] [TestCase(BoosterType.SeaPearl)]
        public void ReserveApplyAndRefundAreExactlyOnce(BoosterType type)
        {
            var store=Store();var s=Session(store);s.TryLoad(out var before,out _);int count=CampaignBoosterTransactions.Count(before.player,type);
            Assert.That(s.ReserveBooster(1,0,type).Committed,Is.True);Assert.That(s.ReserveBooster(1,0,type).NoOp,Is.True);
            Assert.That(s.StartRound(1,Now,true).Committed,Is.False);Assert.That(s.FinishRound(1,false,0,new[]{0,0}).Committed,Is.False);
            Assert.That(s.SettleBooster(1,1,true).Committed,Is.True);Assert.That(s.SettleBooster(1,1,true).NoOp,Is.True);Assert.That(s.SettleBooster(1,1,false).Committed,Is.False);
            Assert.That(s.ReserveBooster(1,1,type).Committed,Is.True);Assert.That(s.SettleBooster(1,2,false).Committed,Is.True);Assert.That(s.SettleBooster(1,2,false).NoOp,Is.True);
            s.TryLoad(out var after,out _);Assert.That(CampaignBoosterTransactions.Count(after.player,type),Is.EqualTo(count-1));
            Assert.That(s.SettleBooster(1,1,true).Committed,Is.False);Assert.That(s.ReserveBooster(2,2,type).Committed,Is.False);
        }
        [TestCase("reserve")] [TestCase("apply")] [TestCase("refund")]
        public void EveryBoosterJournalBoundaryRecoversWithoutDuplicateDelta(string action)
        {
            for(int fault=1;fault<=8;fault++){
                var store=Store();var s=Session(store);if(action!="reserve")s.ReserveBooster(1,0,BoosterType.SeaPearl);
                Func<CampaignSaveSession,CampaignSaveOperation> command=x=>action=="reserve"?x.ReserveBooster(1,0,BoosterType.SeaPearl):x.SettleBooster(1,1,action=="apply");
                store.Writes=0;store.FailAt=fault;var result=command(s);Assert.That(result.Committed,Is.EqualTo(fault>=7));store.Crash();s=Session(store);
                Assert.That(command(s).Committed,Is.True);int writes=store.Writes;Assert.That(command(s).NoOp,Is.True);Assert.That(store.Writes,Is.EqualTo(writes));
                s.TryLoad(out var save,out _);Assert.That(save.player.boosterSeaPearl,Is.EqualTo(action=="refund"?5:4));
            }
        }
        [Test] public void LostBoardReservationRefundsOnceBeforeAbandonment()
        {
            var store=Store();var s=Session(store);s.ReserveBooster(1,0,BoosterType.SeaPearl);store.Crash();s=Session(store);
            Assert.That(s.RecoverReservedBooster().Committed,Is.True);Assert.That(s.RecoverReservedBooster().NoOp,Is.True);s.TryLoad(out var save,out _);
            Assert.That(save.player.boosterSeaPearl,Is.EqualTo(5));Assert.That(save.run.state,Is.EqualTo(CampaignRunState.Running));
        }
        [Test] public void InventoryBlocksAfterUncertainChargeAndDoesNotRequestEffect()
        {
            var store=Store();var s=Session(store);var inventory=new CampaignBoardInventory(s,1);store.Writes=0;store.FailAt=6;
            Assert.That(inventory.Consume(BoosterType.SeaPearl),Is.False);Assert.That(inventory.Blocked,Is.True);Assert.That(inventory.Complete(),Is.False);
            store.Crash();s=Session(store);Assert.That(s.RecoverReservedBooster().Committed,Is.True);s.TryLoad(out var save,out _);Assert.That(save.player.boosterSeaPearl,Is.EqualTo(5));
        }
        [Test] public void EmptyAndUnknownBoosterDoNotWrite()
        {
            var store=Store();var s=Session(store);int writes=store.Writes;Assert.That(s.ReserveBooster(1,0,(BoosterType)99).Committed,Is.False);Assert.That(store.Writes,Is.EqualTo(writes));
            for(int i=0;i<2;i++){Assert.That(s.ReserveBooster(1,i,BoosterType.BogatyrMace).Committed,Is.True);s.SettleBooster(1,i+1,true);}
            writes=store.Writes;Assert.That(s.ReserveBooster(1,2,BoosterType.BogatyrMace).Committed,Is.False);Assert.That(store.Writes,Is.EqualTo(writes));
        }
        [Test] public void V8UpgradePreservesUnfinishedRunAndPreventsDowngrade()
        {
            var store=Store();var s=Session(store);s.TryLoad(out var current,out _);
            var old=JsonUtility.FromJson<CampaignPlayerSaveV8>(JsonUtility.ToJson(current));old.schemaVersion=8;
            var oldStore=new MemoryStore();Repository(oldStore).TrySave(JsonUtility.ToJson(old),8);var upgraded=Session(oldStore);Assert.That(upgraded.MigrateLegacy().Committed,Is.True);
            upgraded.TryLoad(out var save,out _);Assert.That(save.schemaVersion,Is.EqualTo(9));Assert.That(JsonUtility.ToJson(save.run),Is.EqualTo(JsonUtility.ToJson(old.run)));
            Assert.That(save.booster.state,Is.EqualTo(CampaignBoosterState.None));Assert.That(Repository(oldStore,8).TrySave(JsonUtility.ToJson(old),8).Success,Is.False);
        }
    }
}
