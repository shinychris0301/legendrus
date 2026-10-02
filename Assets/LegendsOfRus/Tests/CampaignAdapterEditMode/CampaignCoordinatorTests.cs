using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static LegendsOfRus.Tests.Campaign.CampaignSaveSessionTests;

namespace LegendsOfRus.Tests.Campaign
{
    public sealed class CampaignCoordinatorTests
    {
        const long Now = 638900000000000000;
        sealed class Host : ICampaignBoardHost
        {
            public int Starts, Stops; public bool Fail, Throw, Synchronous; public Action<CampaignBoardResult> Callback; public LevelData Level;
            public bool Start(LevelData level, int attempt, Action<CampaignBoardResult> done)
            { Starts++; Level = level; Callback = done; if (Throw) throw new Exception(); if (Synchronous) done(Result()); return !Fail; }
            public void Stop() { Stops++; }
            public CampaignBoardResult Result() => new CampaignBoardResult { Won = true, Score = 2000, Goals = Level.goals.Select(g => g.target).ToArray() };
        }
        sealed class Routes : ICampaignGameplayRoutes
        {
            public bool Fail, Throw; public int Maps, Results, Recoveries; public CampaignRunReceipt Receipt;
            bool Show() { if (Throw) throw new Exception(); return !Fail; }
            public bool ShowMap() { Maps++; return Show(); }
            public bool ShowResult(CampaignRunReceipt receipt) { Results++; Receipt = receipt; return Show(); }
            public bool ShowInterruptedRound(CampaignRunReceipt receipt) { Recoveries++; Receipt = receipt; return Show(); }
        }
        static MemoryStore Store(int energy = 5)
        {
            var p = JsonUtility.FromJson<PlayerSaveData>(Legacy()); p.levelProgress.Clear(); p.selectedLevel = 1; p.energy = energy; p.lastEnergyUtcTicks = Now;
            var store = new MemoryStore(); Assert.That(Session(store).ImportLegacyCopy(JsonUtility.ToJson(p)).Committed, Is.True); store.Crash(); return store;
        }
        static CampaignPlayerSave Load(MemoryStore store) { Assert.That(Session(store).TryLoad(out var p, out _), Is.True); return p; }
        [Test] public void BoardAndNavigationFollowCommittedTransactions()
        {
            var store = Store(); var host = new Host(); var routes = new Routes(); using(var c = new CampaignGameplayCoordinator(Session(store), host, routes)) {
                Assert.That(c.Recover(), Is.True); Assert.That(c.Start(0, Now), Is.True); Assert.That(Load(store).player.energy, Is.EqualTo(4));
                Assert.That(c.Start(0, Now), Is.True); Assert.That(host.Starts, Is.EqualTo(1));
                host.Callback(host.Result()); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Result));
                Assert.That(Load(store).player.levelProgress[0].completions, Is.EqualTo(1)); Assert.That(routes.Receipt.state, Is.EqualTo(CampaignRunState.Won));
                Assert.That(c.AcknowledgeAndReturn(1), Is.True); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Ready));
            }
        }
        [TestCase(false)] [TestCase(true)] public void BoardCreationFailureRequiresExplicitRecovery(bool throws)
        {
            var store = Store(); var host = new Host { Fail = true, Throw = throws }; var routes = new Routes(); using(var c = new CampaignGameplayCoordinator(Session(store), host, routes)) {
                c.Recover(); Assert.That(c.Start(0, Now), Is.False); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Recovery));
                c.Start(0, Now); Assert.That(host.Starts, Is.EqualTo(1)); Assert.That(Load(store).player.energy, Is.EqualTo(4));
                host.Fail = host.Throw = false; Assert.That(c.Start(1, Now, true), Is.True);
                Assert.That(host.Starts, Is.EqualTo(2)); Assert.That(Load(store).player.energy, Is.EqualTo(3));
                Assert.That(Load(store).player.levelProgress[0].failures, Is.EqualTo(1));
            }
        }
        [TestCase(false)] [TestCase(true)] public void NavigationFailureRetriesReceiptWithoutRegrant(bool throws)
        {
            var store = Store(); var host = new Host(); var routes = new Routes(); using(var c = new CampaignGameplayCoordinator(Session(store), host, routes)) {
                c.Recover(); c.Start(0, Now); routes.Fail = true; routes.Throw = throws; host.Callback(host.Result());
                Assert.That(c.State, Is.EqualTo(CampaignGameplayState.NavigationFailed)); int gold = Load(store).player.gold; int writes = store.Writes;
                routes.Fail = routes.Throw = false; Assert.That(c.Recover(), Is.True); Assert.That(store.Writes, Is.EqualTo(writes)); Assert.That(Load(store).player.gold, Is.EqualTo(gold));
                routes.Fail = true; Assert.That(c.AcknowledgeAndReturn(1), Is.False); writes = store.Writes;
                routes.Fail = false; Assert.That(c.Recover(), Is.True); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Ready)); Assert.That(store.Writes, Is.EqualTo(writes));
            }
        }
        [Test] public void ReopenedRunningSaveDoesNotRebuildBoardOrChargeAndCanBeAbandoned()
        {
            var store = Store(); var h = new Host(); var r = new Routes(); var first = new CampaignGameplayCoordinator(Session(store), h, r); first.Recover(); first.Start(0, Now); first.Dispose();
            store.Crash(); var host = new Host(); using(var c = new CampaignGameplayCoordinator(Session(store), host, new Routes())) {
                Assert.That(c.Recover(), Is.True); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Recovery)); Assert.That(host.Starts, Is.Zero);
                Assert.That(c.AbandonInterruptedRound(1), Is.True); Assert.That(Load(store).player.energy, Is.EqualTo(4)); Assert.That(Load(store).player.levelProgress[0].failures, Is.EqualTo(1));
                Assert.That(c.AbandonInterruptedRound(1), Is.False);
            }
        }
        [Test] public void ReopenedTerminalReceiptRoutesBeforeAcknowledgement()
        {
            var store = Store(); var host = new Host(); var c = new CampaignGameplayCoordinator(Session(store), host, new Routes()); c.Recover(); c.Start(0, Now); host.Callback(host.Result()); c.Dispose(); store.Crash();
            var routes = new Routes(); using(var next = new CampaignGameplayCoordinator(Session(store), new Host(), routes)) {
                Assert.That(next.Recover(), Is.True); Assert.That(routes.Results, Is.EqualTo(1)); Assert.That(next.State, Is.EqualTo(CampaignGameplayState.Result));
            }
        }
        [Test] public void DuplicateOldAndDisposedCallbacksCannotApplyAgain()
        {
            var store = Store(); var host = new Host(); var routes = new Routes(); var c = new CampaignGameplayCoordinator(Session(store), host, routes); c.Recover(); c.Start(0, Now);
            var old = host.Callback; var result = host.Result(); old(result); int writes = store.Writes; old(result); Assert.That(store.Writes, Is.EqualTo(writes));
            c.AcknowledgeAndReturn(1); c.Start(1, Now); writes = store.Writes; old(result); Assert.That(store.Writes, Is.EqualTo(writes));
            var current = host.Callback; c.Dispose(); current(result); Assert.That(store.Writes, Is.EqualTo(writes)); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Disposed));
        }
        [Test] public void InvalidResultDoesNotRouteVictory()
        {
            var store = Store(); var host = new Host(); var routes = new Routes(); using(var c = new CampaignGameplayCoordinator(Session(store), host, routes)) {
                c.Recover(); c.Start(0, Now); var result = host.Result(); result.Goals = new int[result.Goals.Length]; host.Callback(result);
                Assert.That(routes.Results, Is.Zero); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Recovery)); Assert.That(Load(store).run.state, Is.EqualTo(CampaignRunState.Running));
            }
        }
        [Test] public void SynchronousBoardCallbackIsDeferredUntilCreationCompletes()
        {
            var store = Store(); var host = new Host { Synchronous = true }; using(var c = new CampaignGameplayCoordinator(Session(store), host, new Routes())) {
                c.Recover(); Assert.That(c.Start(0, Now), Is.True); Assert.That(c.State, Is.EqualTo(CampaignGameplayState.Result)); Assert.That(Load(store).run.state, Is.EqualTo(CampaignRunState.Won));
            }
        }
        [Test] public void EmptyEnergyDoesNotConstructBoard()
        {
            var store = Store(0); var host = new Host(); using(var c = new CampaignGameplayCoordinator(Session(store), host, new Routes())) {
                c.Recover(); Assert.That(c.Start(0, Now), Is.False); Assert.That(host.Starts, Is.Zero); Assert.That(Load(store).runSequence, Is.Zero);
            }
        }
        [TestCase("start")] [TestCase("result")] public void StorageFaultCannotAdvanceBoardOrResultRoute(string phase)
        {
            for(int fault = 1; fault <= 6; fault++) {
                var store = Store(); var host = new Host(); var routes = new Routes(); using(var c = new CampaignGameplayCoordinator(Session(store), host, routes)) {
                    c.Recover(); if(phase == "result") c.Start(0, Now); store.Writes = 0; store.FailAt = fault;
                    if(phase == "result") host.Callback(host.Result()); else Assert.That(c.Start(0, Now), Is.False);
                    Assert.That(c.State, Is.EqualTo(CampaignGameplayState.StorageBlocked)); Assert.That(routes.Results, Is.Zero);
                    Assert.That(host.Starts, Is.EqualTo(phase == "result" ? 1 : 0));
                }
                store.Crash(); using(var next = new CampaignGameplayCoordinator(Session(store), new Host(), new Routes())) Assert.That(next.Recover(), Is.True);
            }
        }
    }
}
