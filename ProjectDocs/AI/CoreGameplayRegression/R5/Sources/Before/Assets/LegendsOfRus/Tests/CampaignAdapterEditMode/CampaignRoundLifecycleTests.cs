using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static LegendsOfRus.Tests.Campaign.CampaignSaveSessionTests;

namespace LegendsOfRus.Tests.Campaign
{
    public sealed class CampaignRoundLifecycleTests
    {
        const long Now = 638900000000000000;
        static MemoryStore NewStore(int energy = 5, int gold = 100)
        {
            var source = JsonUtility.FromJson<PlayerSaveData>(Legacy());
            source.levelProgress.Clear(); source.completedStoryScenes.Clear(); source.acknowledgedChapterCompletions.Clear();
            source.selectedChapter = source.chapter = 1; source.selectedLevel = source.level = source.highestUnlockedLevel = 1;
            source.energy = energy; source.gold = gold; source.lastEnergyUtcTicks = Now;
            var store = new MemoryStore(); Assert.That(Session(store).ImportLegacyCopy(JsonUtility.ToJson(source)).Committed, Is.True);
            store.Crash(); return store;
        }
        static CampaignPlayerSave Load(CampaignSaveSession session)
        { Assert.That(session.TryLoad(out var save, out var error), Is.True, error); return save; }
        static int[] Goals(CampaignSaveSession session) => JsonUtility.FromJson<LevelData>(Load(session).run.levelJson).goals.Select(g => g.target).ToArray();
        static CampaignSaveOperation Win(CampaignSaveSession session, long sequence = 1) => session.FinishRound(sequence, true, 2000, Goals(session));

        [Test] public void WinPersistsRewardsOnceAndReplayOnlyImprovesRecord()
        {
            var store = NewStore(); var s = Session(store); Assert.That(s.StartRound(0, Now).Committed, Is.True);
            Assert.That(s.StartRound(0, Now).NoOp, Is.True); var initial = Load(s); var reward = JsonUtility.FromJson<LevelData>(initial.run.levelJson).rewards;
            Assert.That(initial.player.energy, Is.EqualTo(4)); Assert.That(Win(s).Committed, Is.True);
            var won = Load(s); Assert.That(won.player.gold, Is.EqualTo(100 + reward.gold)); Assert.That(won.player.levelProgress[0].completions, Is.EqualTo(1));
            Assert.That(won.player.highestUnlockedLevel, Is.EqualTo(2)); Assert.That(Win(s).NoOp, Is.True);
            Assert.That(s.StartRound(1, Now).Committed, Is.False); Assert.That(s.AcknowledgeResult(1).Committed, Is.True);
            Assert.That(s.AcknowledgeResult(1).NoOp, Is.True); Assert.That(s.StartRound(1, Now).Committed, Is.True);
            Assert.That(Win(s, 2).Committed, Is.True); var replay = Load(s);
            Assert.That(replay.player.gold, Is.EqualTo(won.player.gold)); Assert.That(replay.player.energy, Is.EqualTo(3));
            Assert.That(replay.player.levelProgress[0].attempts, Is.EqualTo(2)); Assert.That(replay.player.levelProgress[0].retries, Is.EqualTo(1));
            Assert.That(replay.run.granted.gold, Is.Zero);
            store.Crash(); Assert.That(Load(Session(store)).run.state, Is.EqualTo(CampaignRunState.Won));
        }
        [Test] public void FailureRetryAndPauseRestartChargeOneAttemptEach()
        {
            var store = NewStore(); var s = Session(store); s.StartRound(0, Now);
            Assert.That(s.FinishRound(1, false, 100, new int[Goals(s).Length]).Committed, Is.True);
            Assert.That(s.StartRound(1, Now, true).Committed, Is.True); Assert.That(s.StartRound(1, Now, true).NoOp, Is.True);
            Assert.That(s.StartRound(2, Now, true).Committed, Is.True);
            var save = Load(s); Assert.That(save.player.energy, Is.EqualTo(2));
            var p = save.player.levelProgress[0]; Assert.That(p.attempts, Is.EqualTo(3)); Assert.That(p.failures, Is.EqualTo(2)); Assert.That(p.retries, Is.EqualTo(2));
            Assert.That(save.player.gold, Is.EqualTo(100));
        }
        [Test] public void InvalidAndStaleCommandsDoNotWrite()
        {
            var store = NewStore(); var s = Session(store); int writes = store.Writes;
            Assert.That(s.StartRound(-1, Now).Committed, Is.False); Assert.That(s.StartRound(0, -1).Committed, Is.False);
            Assert.That(s.StartRound(0, Now, true).Committed, Is.False); Assert.That(store.Writes, Is.EqualTo(writes));
            s.StartRound(0, Now); writes = store.Writes;
            Assert.That(s.SelectRound(1, 1).Committed, Is.False);
            Assert.That(s.FinishRound(0, true, 2000, Goals(s)).Committed, Is.False);
            Assert.That(s.FinishRound(1, true, 2000, new int[Goals(s).Length]).Committed, Is.False);
            Assert.That(s.FinishRound(1, true, -1, Goals(s)).Committed, Is.False); Assert.That(s.AcknowledgeResult(1).Committed, Is.False);
            Assert.That(store.Writes, Is.EqualTo(writes)); Win(s); writes = store.Writes;
            Assert.That(s.FinishRound(1, false, 2000, Goals(s)).Committed, Is.False); Assert.That(s.StartRound(1, Now, true).Committed, Is.False);
            Assert.That(store.Writes, Is.EqualTo(writes));
        }
        [Test] public void EnergyRefillUsesExistingIntervalAndRejectsClockRollbackGain()
        {
            var store = NewStore(0); var s = Session(store); int writes = store.Writes;
            Assert.That(s.StartRound(0, Now - TimeSpan.FromDays(1).Ticks).Committed, Is.False); Assert.That(store.Writes, Is.EqualTo(writes));
            Assert.That(s.StartRound(0, Now + TimeSpan.FromMinutes(30).Ticks).Committed, Is.True);
            var save = Load(s); Assert.That(save.player.energy, Is.Zero); Assert.That(save.player.lastEnergyUtcTicks, Is.EqualTo(Now + TimeSpan.FromMinutes(30).Ticks));
        }
        [Test] public void InsufficientRetryEnergyKeepsPriorRunAndReceipt()
        {
            var store = NewStore(1); var s = Session(store); s.StartRound(0, Now); int writes = store.Writes;
            Assert.That(s.StartRound(1, Now, true).Committed, Is.False); Assert.That(store.Writes, Is.EqualTo(writes));
            Assert.That(Load(s).runSequence, Is.EqualTo(1)); Assert.That(Load(s).run.state, Is.EqualTo(CampaignRunState.Running));
        }
        [Test] public void OverflowCannotPartiallyGrantRewardOrCompleteRound()
        {
            var store = NewStore(5, int.MaxValue); var s = Session(store); s.StartRound(0, Now); int writes = store.Writes;
            Assert.That(Win(s).Committed, Is.False); Assert.That(store.Writes, Is.EqualTo(writes));
            var save = Load(s); Assert.That(save.run.state, Is.EqualTo(CampaignRunState.Running)); Assert.That(save.player.levelProgress[0].rewardClaimed, Is.False);
        }
        [Test] public void RunningResultUsesPinnedRewardsWhenCatalogDisappears()
        {
            var store = NewStore(); var s = Session(store); s.StartRound(0, Now);
            var missing = Session(store, new ValidatedCampaignCatalog(Definition())); Assert.That(missing.ResolveSelectedRound(), Is.Null);
            Assert.That(Win(missing).Committed, Is.True); Assert.That(Load(missing).player.levelProgress[0].rewardClaimed, Is.True);
        }
        [Test] public void PreviousRunCannotReapplyResultAfterNextRunStarts()
        {
            var store = NewStore(); var s = Session(store); s.StartRound(0, Now); var goals = Goals(s); Win(s); s.AcknowledgeResult(1); s.StartRound(1, Now);
            int writes = store.Writes; Assert.That(s.FinishRound(1, true, 2000, goals).Committed, Is.False); Assert.That(store.Writes, Is.EqualTo(writes));
        }
        [TestCase("start")] [TestCase("win")] [TestCase("fail")] [TestCase("retry")] [TestCase("ack")]
        public void EveryLifecycleWriteBoundaryRecoversExactlyOnce(string step)
        {
            for (int fault = 1; fault <= 8; fault++)
            {
                var store = NewStore(); var s = Session(store);
                if (step != "start") s.StartRound(0, Now);
                if (step == "retry") s.FinishRound(1, false, 0, new int[Goals(s).Length]);
                if (step == "ack") Win(s);
                Func<CampaignSaveSession, CampaignSaveOperation> operation = session => step == "start" ? session.StartRound(0, Now) :
                    step == "win" ? Win(session) : step == "fail" ? session.FinishRound(1, false, 0, new int[Goals(session).Length]) :
                    step == "retry" ? session.StartRound(1, Now, true) : session.AcknowledgeResult(1);
                store.Writes = 0; store.FailAt = fault; var result = operation(s);
                Assert.That(result.Committed, Is.EqualTo(fault >= 7), step + ":" + fault);
                if (!result.Committed) Assert.That(s.TryLoad(out _, out _), Is.False, "Uncertain session must not accept volatile state.");
                store.Crash(); s = Session(store); Assert.That(operation(s).Committed, Is.True, step + ":" + fault);
                int writes = store.Writes; Assert.That(operation(s).NoOp, Is.True); Assert.That(store.Writes, Is.EqualTo(writes));
                var save = Load(s); Assert.That(save.player.energy, Is.EqualTo(step == "retry" ? 3 : 4));
                Assert.That(save.player.levelProgress[0].attempts, Is.EqualTo(step == "retry" ? 2 : 1));
                Assert.That(save.player.levelProgress[0].completions, Is.EqualTo(step == "win" || step == "ack" ? 1 : 0));
            }
        }
        [Test] public void V7UpgradePreservesProgressAndRejectsDowngrade()
        {
            var current = CampaignPlayerSaveCodec.FromLegacy(Legacy(), Definition());
            var prior = JsonUtility.FromJson<CampaignPlayerSaveV7>(JsonUtility.ToJson(current)); prior.schemaVersion = 7;
            var store = new MemoryStore(); Assert.That(Repository(store).TrySave(JsonUtility.ToJson(prior), 7).Success, Is.True);
            var s = Session(store); Assert.That(s.TryLoad(out _, out _), Is.False); Assert.That(s.MigrateLegacy().Committed, Is.True);
            var save = Load(s); Assert.That(save.schemaVersion, Is.EqualTo(9)); Assert.That(save.runSequence, Is.Zero);
            Assert.That(JsonUtility.ToJson(save.player), Is.EqualTo(JsonUtility.ToJson(prior.player)));
            Assert.That(Repository(store, 7).TrySave(JsonUtility.ToJson(prior), 7).Success, Is.False);
            Assert.That(s.MigrateLegacy().NoOp, Is.True);
        }
        [Test] public void V7UpgradeRecoversAtEveryWriteBoundary()
        {
            for (int fault = 1; fault <= 8; fault++)
            {
                var prior = JsonUtility.FromJson<CampaignPlayerSaveV7>(JsonUtility.ToJson(CampaignPlayerSaveCodec.FromLegacy(Legacy(), Definition()))); prior.schemaVersion = 7;
                var store = new MemoryStore(); Repository(store).TrySave(JsonUtility.ToJson(prior), 7); store.Writes = 0; store.FailAt = fault;
                Session(store).MigrateLegacy(); store.Crash(); var s = Session(store);
                Assert.That(s.MigrateLegacy().Committed, Is.True); Assert.That(Load(s).legacySourceJson, Is.EqualTo(prior.legacySourceJson));
            }
        }
        [Test] public void MalformedRunReceiptIsRejected()
        {
            var store = NewStore(); var s = Session(store); s.StartRound(0, Now); var save = Load(s);
            save.run.sequence++; Assert.Throws<ArgumentException>(() => CampaignPlayerSaveCodec.Encode(save, Definition()));
        }
    }
}
