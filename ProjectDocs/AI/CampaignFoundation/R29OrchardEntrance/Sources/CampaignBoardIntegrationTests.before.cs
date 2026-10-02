using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.GameplayCore;
using LegendsOfRus.SaveCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignBoardIntegrationTests
    {
        sealed class Store : ISaveJournalStore
        {
            readonly Dictionary<string,string> values = new Dictionary<string,string>();
            public bool TryRead(string key, out string value) => values.TryGetValue(key, out value);
            public void Write(string key, string value) => values[key] = value;
            public void Delete(string key) => values.Remove(key);
            public void Flush() { }
        }
        sealed class Routes : ICampaignGameplayRoutes
        {
            public CampaignRunReceipt Result;
            public bool ShowMap() => true;
            public bool ShowResult(CampaignRunReceipt receipt) { Result = receipt; return true; }
            public bool ShowInterruptedRound(CampaignRunReceipt receipt) => true;
        }
        static string Resource(string path) => Resources.Load<TextAsset>(path).text;
        internal static CampaignSaveSession Session(ISaveJournalStore store=null,bool initialize=true)
        {
            var definition = JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(Path.Combine(Application.dataPath,"../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
            var catalog = new ValidatedCampaignCatalog(definition);
            Assert.That(catalog.TryAdd(new CampaignPackageInput { ChapterId = 1, ContentRevision = 1, LegacyChapterOne = true,
                LevelsJson = Resource("Config/chapter01_levels"), StoryJson = Resource("Config/chapter01_story"),
                RussianJson = Resource("Localization/story_ru"), EnglishJson = Resource("Localization/story_en") }, out var errors), Is.True, string.Join(";",errors));
            var session = new CampaignSaveSession(store??new Store(), definition, catalog);
            if(initialize)Assert.That(session.ImportLegacyCopy(JsonUtility.ToJson(new PlayerSaveData { lastEnergyUtcTicks = DateTime.UtcNow.Ticks })).Committed, Is.True);
            return session;
        }
        [Test] public void AdoptedRulesGeneratePlayableBoardsForAllAuthoredRounds()
        {
            var levels=ChapterLevelCatalog.LoadChapter1().levels;Assert.That(levels.Length,Is.EqualTo(150));
            Assert.That(Match3Board.CurrentGameplayRules,Is.EqualTo(GameplayRuleVersion.PrototypeR4));
            foreach(var level in levels)for(int attempt=1;attempt<=3;attempt++)
            {
                int seed=BoardSeedUtility.CreateRuntimeSeed(level.chapterId,level.levelId,level.boardSeed+attempt);
                var model=LevelBoardFactory.Create(level,seed,rules:Match3Board.CurrentGameplayRules);
                Assert.That(model.SquareMatches&&model.StandaloneSpecials,Is.True,level.id);
                Assert.That(model.HasAnyMatches(),Is.False,level.id+" initial automatic match");
                Assert.That(model.HasLegalMove(),Is.True,level.id+" no legal opening action");
                var repeat=LevelBoardFactory.Create(level,seed,rules:Match3Board.CurrentGameplayRules);
                Assert.That(repeat.ComputeStateHash(),Is.EqualTo(model.ComputeStateHash()),level.id+" deterministic generation");
            }
        }
        [UnityTest, Timeout(300000)] public IEnumerator LegalBoardVictoryCommitsBeforeResultRoute() { yield return Journey(true); }
        [UnityTest, Timeout(300000)] public IEnumerator LegalBoardFailureAndRetryUseCampaignAttempts() { yield return Journey(false); }
        IEnumerator Journey(bool win)
        {
            // Test-owned host in an isolated runner configuration, not a production startup scene.
            var go = new GameObject("CampaignIntegrationCanvas", typeof(RectTransform), typeof(Canvas));
            var root = (RectTransform)go.transform; root.sizeDelta = new Vector2(800,700);
            GameObject stateObject = null; var live = GameState.Instance;
            if (!live) { stateObject = new GameObject("CampaignLegacyIsolationProbe"); live = stateObject.AddComponent<GameState>(); }
            bool wasEnabled = live.enabled; live.enabled = false; string before = JsonUtility.ToJson(live.Save);
            var session = Session(); var host = new CampaignMatch3BoardHost(root); var routes = new Routes(); var coordinator = new CampaignGameplayCoordinator(session, host, routes);
            try
            {
                Assert.That(coordinator.Recover(), Is.True); Assert.That(coordinator.Start(0, DateTime.UtcNow.Ticks), Is.True);
                var board = host.Current; Assert.That(board, Is.Not.Null); string hash = board.BoardStateHash;
                Assert.That(coordinator.Start(0, DateTime.UtcNow.Ticks), Is.True); Assert.That(host.Current, Is.SameAs(board)); Assert.That(board.BoardStateHash, Is.EqualTo(hash));
                foreach (BoosterType booster in Enum.GetValues(typeof(BoosterType))) board.UseBooster(booster);
                Assert.That(board.IsBoosterArmed, Is.False); Assert.That(JsonUtility.ToJson(live.Save), Is.EqualTo(before), "Campaign board must not write live attempts/inventory.");
                Assert.That(board.TryPause(), Is.True); Assert.That(board.TryResume(), Is.True);
                int moves = 0;
                while (coordinator.State == CampaignGameplayState.Playing)
                {
                    Assert.That(++moves, Is.LessThanOrEqualTo(28)); var plan = Choose(board, win); Assert.That(plan, Is.Not.Null);
                    Assert.That(board.SwipeCell(plan.PrimaryPosition.X, plan.PrimaryPosition.Y,
                        new Vector2Int(plan.SecondaryPosition.X-plan.PrimaryPosition.X,plan.SecondaryPosition.Y-plan.PrimaryPosition.Y)), Is.True);
                    float end = Time.realtimeSinceStartup + 15;
                    while (coordinator.State == CampaignGameplayState.Playing && board && (board.HasActiveCommand || !board.CanAcceptPointerInput) && Time.realtimeSinceStartup < end) yield return null;
                    Assert.That(Time.realtimeSinceStartup, Is.LessThan(end)); yield return null;
                }
                Assert.That(coordinator.State, Is.EqualTo(CampaignGameplayState.Result), coordinator.Diagnostic);
                Assert.That(routes.Result.state, Is.EqualTo(win ? CampaignRunState.Won : CampaignRunState.Failed));
                Assert.That(session.TryLoad(out var save, out _), Is.True); Assert.That(save.player.energy, Is.EqualTo(4));
                Assert.That(save.player.levelProgress.Single().attempts, Is.EqualTo(1)); Assert.That(save.player.levelProgress.Single().completions, Is.EqualTo(win ? 1 : 0));
                Assert.That(host.Current, Is.Null); Assert.That(JsonUtility.ToJson(live.Save), Is.EqualTo(before));
                if (!win)
                {
                    Assert.That(coordinator.Start(1, DateTime.UtcNow.Ticks, true), Is.True); Assert.That(host.Current, Is.Not.Null);
                    Assert.That(session.TryLoad(out save, out _), Is.True); Assert.That(save.player.energy, Is.EqualTo(3));
                    Assert.That(save.player.levelProgress.Single().attempts, Is.EqualTo(2)); Assert.That(save.player.levelProgress.Single().failures, Is.EqualTo(1));
                }
                else Assert.That(coordinator.AcknowledgeAndReturn(1), Is.True);
                Assert.That(JsonUtility.ToJson(live.Save), Is.EqualTo(before));
                var dir = Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");
                if (!string.IsNullOrEmpty(dir)) { Directory.CreateDirectory(dir); File.WriteAllText(Path.Combine(dir,win?"CampaignWin.json":"CampaignFailRetry.json"),JsonUtility.ToJson(save,true)); }
            }
            finally
            {
                coordinator.Dispose(); UnityEngine.Object.Destroy(go); live.enabled = wasEnabled;
                if (stateObject) UnityEngine.Object.Destroy(stateObject);
            }
            yield return null;
        }
        internal static BoardResolutionPlan Choose(Match3Board board, bool win)
        {
            // Read-only planning; execute the chosen swipe on the actual board and wait for its callback.
            var model = (BoardModel)typeof(Match3Board).GetField("model",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(board);
            var planner = new BoardResolutionPlanner(model,null,board.RuleVersion); BoardResolutionPlan best = null; float bestValue = float.NegativeInfinity;
            for(int y=0;y<model.Height;y++) for(int x=0;x<model.Width;x++) foreach(var d in new[]{new BoardPosition(1,0),new BoardPosition(0,1)})
            {
                var a = new BoardPosition(x,y); var b = new BoardPosition(x+d.X,y+d.Y); if(!model.IsInside(b))continue;
                var p = planner.PlanSwap(a,b); if(!p.IsResolved || !p.ConsumesMove)continue;
                int red=0,blue=0; foreach(var wave in p.Waves){red+=wave.ClearedByTileType[0];blue+=wave.ClearedByTileType[1];}
                float value = win ? 1000*(Math.Min(red,board.RedRemaining)+Math.Min(blue,board.BlueRemaining))+p.TotalScoreDelta : -10000*blue-p.TotalScoreDelta;
                if(value>bestValue){bestValue=value;best=p;}
            }
            return best;
        }
    }
}
