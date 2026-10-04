using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class RetainedRoundPlayModeTests
    {
        GameState State => GameState.Instance;
        ScreenRouter Router => ScreenRouter.Instance;
        const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        string previous;
        string[] values;
        readonly string[] keys = {"LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging"};
        [SetUp] public void SetUp()
        {
            string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(isolated,Does.StartWith("/tmp/"));
            Assert.That(Application.persistentDataPath,Does.StartWith(isolated+"/"));
            Assert.That(State,Is.Not.Null);
            previous=JsonUtility.ToJson(State.Save);
            values=keys.Select(k=>PlayerPrefs.HasKey(k)?PlayerPrefs.GetString(k):null).ToArray();
            Router.Show(ScreenId.SplashTitle);Settle();State.ResetProgress();
        }
        [TearDown] public void TearDown()
        {
            State.RetainedWriteOverride=null;State.ResetProgress();
            Router.Show(ScreenId.SplashTitle);Settle();
            State.Save=JsonUtility.FromJson<PlayerSaveData>(previous);
            for(int i=0;i<keys.Length;i++){PlayerPrefs.DeleteKey(keys[i]);if(values[i]!=null)PlayerPrefs.SetString(keys[i],values[i]);}
            PlayerPrefs.Save();
            foreach(var board in UnityEngine.Object.FindObjectsByType<Match3Board>(FindObjectsSortMode.None))
                if(board.name=="RetainedFixture")UnityEngine.Object.DestroyImmediate(board.gameObject);
        }
        void Settle(){for(int i=0;i<24;i++)Router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        RetainedRoundData Fixture(int number=31)
        {
            State.Save.highestUnlockedLevel=500;Assert.That(State.SelectLevel(1,number),Is.True);
            var level=ChapterLevelCatalog.Find(1,number);int attempt=State.RegisterLevelAttempt(level);
            int seed=BoardSeedUtility.CreateRuntimeSeed(1,number,level.boardSeed+attempt);
            var model=LevelBoardFactory.Create(level,seed,rules:Match3Board.CurrentGameplayRules);
            // Deliberate settled checkpoint fixture; not a human difficulty result.
            for(int x=0;x<5;x++) {
                var position=new BoardPosition(x,0);model.SetObstacle(position,new BoardObstacleData(BoardObstacleKind.None,0));
                model.SetTile(position,x%level.tileTypes);model.SetSpecial(position,(SpecialGemKind)(x+1));
            }
            model.SetObstacle(new BoardPosition(0,1),new BoardObstacleData(BoardObstacleKind.Crate,3));
            model.SetObstacle(new BoardPosition(1,1),new BoardObstacleData(BoardObstacleKind.Ice,2));
            model.SetObstacle(new BoardPosition(2,1),new BoardObstacleData(BoardObstacleKind.Chain,1));
            model.SetObstacle(new BoardPosition(3,1),new BoardObstacleData(BoardObstacleKind.RunicSeal,2));
            var feather=new BoardPosition(4,1);model.SetObstacle(feather,new BoardObstacleData(BoardObstacleKind.None,0));
            model.SetObjectiveItem(feather,BoardObjectiveItemKind.FirebirdFeather);
            var snapshot=model.CreateSnapshot();
            return new RetainedRoundData{attemptId=Guid.NewGuid().ToString("N"),chapter=1,level=number,attempt=attempt,
                levelFingerprint=RetainedRoundData.Fingerprint(level),rules=(int)Match3Board.CurrentGameplayRules,seed=seed,
                score=371,moves=0,stateHash=snapshot.StateHash,tiles=snapshot.Tiles,specials=snapshot.Specials.Select(x=>(int)x).ToArray(),
                obstacles=snapshot.Obstacles.Select(x=>(int)x).ToArray(),layers=snapshot.ObstacleLayers.Select(x=>(int)x).ToArray(),
                items=snapshot.ObjectiveItems.Select(x=>(int)x).ToArray(),goals=level.goals.Select(g=>Math.Min(1,g.target-1)).ToArray()};
        }
        static PlayerSaveWriteResult Failure(PlayerSaveData _) => new PlayerSaveWriteResult(PlayerSaveWriteStatus.StorageFailure,"test",0,"injected");
        [Test] public void JournalRoundTripPreservesAttemptBoardGoalsAndInventory()
        {
            var data=Fixture();int energy=State.Save.energy,gold=State.Save.gold,gems=State.Save.gems;
            Assert.That(State.TrySuspendRound(data),Is.True);State.Save=SaveService.Load();
            Assert.That(State.TryGetRetainedRound(out var restored),Is.True);
            Assert.That(JsonUtility.ToJson(restored),Is.EqualTo(JsonUtility.ToJson(data)));
            Assert.That(State.Save.energy,Is.EqualTo(energy));Assert.That(State.Save.gold,Is.EqualTo(gold));Assert.That(State.Save.gems,Is.EqualTo(gems));
            Assert.That(State.GetLevelProgress(1,31).failures,Is.Zero);
            Assert.That(State.RestoreRetainedFailurePresentation(),Is.True);Assert.That(State.LastFailureResult.Score,Is.EqualTo(371));
        }
        [Test] public void DuplicateAndForeignCallbacksCannotOverwriteAttempt()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);Assert.That(State.TrySuspendRound(data),Is.True);
            var changed=data.Copy();changed.score++;Assert.That(State.TrySuspendRound(changed),Is.False);
            changed=data.Copy();changed.attemptId=Guid.NewGuid().ToString("N");Assert.That(State.TrySuspendRound(changed),Is.False);
            Assert.That(State.SelectLevel(1,32),Is.False);
            Assert.Throws<InvalidOperationException>(()=>State.RegisterLevelAttempt(ChapterLevelCatalog.Find(1,31)));
            Assert.That(State.GetLevelProgress(1,31).attempts,Is.EqualTo(1));
        }
        [Test] public void DeclineIsDurableAndIdempotentWithoutRewardsOrEnergyCharge()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);int gold=State.Save.gold,energy=State.Save.energy;
            Assert.That(State.TryFinalizeRetainedFailure("wrong"),Is.False);
            Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.True);
            Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.False);
            State.Save=SaveService.Load();Assert.That(State.Save.retainedRound,Is.Null);
            var progress=State.GetLevelProgress(1,31);Assert.That(progress.failures,Is.EqualTo(1));Assert.That(progress.completions,Is.Zero);
            Assert.That(State.Save.gold,Is.EqualTo(gold));Assert.That(State.Save.energy,Is.EqualTo(energy));
        }
        [Test] public void FailedWritesKeepMemoryCopyAndDoNotFinalizeLoss()
        {
            var data=Fixture();State.RetainedWriteOverride=Failure;
            Assert.That(State.TrySuspendRound(data),Is.False);Assert.That(State.HasUnsavedRetainedRound,Is.True);
            Assert.That(State.Save.retainedRound,Is.Null);Assert.That(State.TryGetRetainedRound(out _),Is.True);
            Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.False);Assert.That(State.GetLevelProgress(1,31).failures,Is.Zero);
            State.RetainedWriteOverride=null;Assert.That(State.TrySuspendRound(data),Is.True);Assert.That(State.HasUnsavedRetainedRound,Is.False);
            State.RetainedWriteOverride=Failure;Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.False);
            Assert.That(SaveService.Load().retainedRound.attemptId,Is.EqualTo(data.attemptId));
            State.RetainedWriteOverride=null;Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.True);
        }
        [Test] public void ChangeListenersSeeCommittedStateAfterRecoveredWrite()
        {
            var data=Fixture();State.RetainedWriteOverride=Failure;Assert.That(State.TrySuspendRound(data),Is.False);
            State.RetainedWriteOverride=null;bool notified=false;
            Action changed=()=>{notified=true;Assert.That(State.HasUnsavedRetainedRound,Is.False);Assert.That(State.TryGetRetainedRound(out _),Is.False);};
            State.Changed+=changed;
            try{Assert.That(State.TryFinalizeRetainedFailure(data.attemptId),Is.True);Assert.That(notified,Is.True);}
            finally{State.Changed-=changed;}
        }
        [Test] public void ResetClearsUnsavedAttempt()
        {var data=Fixture();State.RetainedWriteOverride=Failure;State.TrySuspendRound(data);State.RetainedWriteOverride=null;State.ResetProgress();Assert.That(State.TryGetRetainedRound(out _),Is.False);Assert.That(State.HasUnsavedRetainedRound,Is.False);}
        [TestCase("schema")][TestCase("rules")][TestCase("fingerprint")][TestCase("hash")][TestCase("length")][TestCase("goal")][TestCase("enum")][TestCase("layers")][TestCase("identity")]
        public void IncompatibleOptionalCheckpointDoesNotResetProgress(string damage)
        {
            var data=Fixture();switch(damage){case "schema":data.schema++;break;case "rules":data.rules=0;break;case "fingerprint":data.levelFingerprint="old";break;case "hash":data.stateHash="bad";break;case "length":data.tiles=new int[1];break;case "goal":data.goals[0]=-1;break;case "enum":data.specials[0]=999;break;case "layers":data.layers[0]=999;break;case "identity":data.attemptId="bad";break;}
            State.Save.gold=812;State.Save.retainedRound=data;State.Persist();State.Save=SaveService.Load();
            Assert.That(State.TryGetRetainedRound(out _),Is.False);Assert.That(State.Save.gold,Is.EqualTo(812));
            Assert.That(State.RegisterLevelAttempt(ChapterLevelCatalog.Find(1,31)),Is.EqualTo(2));Assert.That(State.Save.retainedRound,Is.Null);
        }
        [UnityTest] public IEnumerator RestoredBoardIsLockedAndDoesNotSpendOrRegisterAgain()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);
            int energy=State.Save.energy;var go=new GameObject("RetainedFixture",typeof(RectTransform));
            var board=go.AddComponent<Match3Board>();board.InitializeRetained(data,_=>Assert.Fail("win"),_=>Assert.Fail("fail"));yield return null;
            Assert.That(board.BoardStateHash,Is.EqualTo(data.stateHash));Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Failed));
            Assert.That(board.Score,Is.EqualTo(data.score));Assert.That(board.Moves,Is.Zero);
            for(int i=0;i<data.goals.Length;i++)Assert.That(board.GetGoalSlot(i).Current,Is.EqualTo(data.goals[i]));
            Assert.That(State.GetLevelProgress(1,31).attempts,Is.EqualTo(data.attempt));Assert.That(State.Save.energy,Is.EqualTo(energy));
            Assert.That(JsonUtility.ToJson(board.CaptureFailedRound()),Is.EqualTo(JsonUtility.ToJson(data)));
        }
        [Test] public void FutureResolutionDependsOnSnapshotAndActionNotRandomCursor()
        {
            var data=Fixture();var level=ChapterLevelCatalog.Find(1,31);
            var a=LevelBoardFactory.Create(level,123,rules:Match3Board.CurrentGameplayRules);
            var b=LevelBoardFactory.Create(level,987,rules:Match3Board.CurrentGameplayRules);
            a.RestoreSnapshot(data.Snapshot(level));b.RestoreSnapshot(data.Snapshot(level));
            var pa=new BoardResolutionPlanner(a,null,Match3Board.CurrentGameplayRules);var pb=new BoardResolutionPlanner(b,null,Match3Board.CurrentGameplayRules);
            int legal=0;for(int y=0;y<level.rows;y++)for(int x=0;x<level.columns-1;x++){
                var from=new BoardPosition(x,y);var to=new BoardPosition(x+1,y);var p=pa.PlanSwap(from,to);var q=pb.PlanSwap(from,to);
                Assert.That(q.FinalSnapshot.StateHash,Is.EqualTo(p.FinalSnapshot.StateHash));Assert.That(q.PlanHash,Is.EqualTo(p.PlanHash));if(p.FinalSnapshot.StateHash!=data.stateHash)legal++;
            }Assert.That(legal,Is.GreaterThan(0));
        }
        [UnityTest] public IEnumerator FailedDeclineWriteKeepsFailureActionUsable()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);
            Router.Show(ScreenId.Home);Settle();yield return null;
            State.RetainedWriteOverride=Failure;
            Router.Viewport.GetComponentInChildren<ProductionFailView>().Exit.onClick.Invoke();Settle();yield return null;
            Assert.That(State.GetLevelProgress(1,31).failures,Is.Zero);
            var view=Router.Viewport.GetComponentInChildren<ProductionFailView>();Assert.That(view,Is.Not.Null);
            Assert.That(view.PaidOffer.interactable || view.AdOffer.interactable,Is.False);
            State.RetainedWriteOverride=null;view.Exit.onClick.Invoke();Settle();yield return null;
            Assert.That(State.GetLevelProgress(1,31).failures,Is.EqualTo(1));Assert.That(State.TryGetRetainedRound(out _),Is.False);
        }
        [Test] public void LegacySaveWithoutOptionalCheckpointStillLoads()
        {
            const string key="LegendsOfRus_Save_v1";
            foreach(var k in keys)PlayerPrefs.DeleteKey(k);
            PlayerPrefs.SetString(key,"{\"saveVersion\":5,\"gold\":812,\"gems\":4,\"energy\":3,\"chapter\":1,\"level\":1}");PlayerPrefs.Save();
            var result=SaveService.LoadWithResult();Assert.That(result.Succeeded,Is.True);
            Assert.That(result.Data.gold,Is.EqualTo(812));Assert.That(result.Data.retainedRound,Is.Null);
        }
        [UnityTest] public IEnumerator RealSwapOnLastMovePersistsSettledFailure()
        {
            State.Save.highestUnlockedLevel=31;State.SelectLevel(1,31);
            Router.Show(ScreenId.Gameplay);Settle();yield return null;
            var board=Router.Viewport.GetComponentInChildren<Match3Board>();
            var model=(BoardModel)typeof(Match3Board).GetField("model",Private).GetValue(board);
            var planner=new BoardResolutionPlanner(model,null,Match3Board.CurrentGameplayRules);
            BoardPosition from=default,to=default;bool found=false;
            for(int y=0;y<model.Height&&!found;y++)for(int x=0;x<model.Width-1&&!found;x++){
                from=new BoardPosition(x,y);to=new BoardPosition(x+1,y);found=planner.PlanSwap(from,to).IsResolved;
            }
            Assert.That(found,Is.True);typeof(Match3Board).GetField("moves",Private).SetValue(board,1);
            yield return (IEnumerator)typeof(Match3Board).GetMethod("TrySwap",Private).Invoke(board,new object[]{new Vector2Int(from.X,from.Y),new Vector2Int(to.X,to.Y)});
            Settle();yield return null;
            Assert.That(State.TryGetRetainedRound(out var saved),Is.True);
            Assert.That(saved.moves,Is.Zero);Assert.That(saved.score,Is.GreaterThan(0));Assert.That(State.GetLevelProgress(1,31).failures,Is.Zero);
            Assert.That(saved.IsCompatible(ChapterLevelCatalog.Find(1,31)),Is.True);
            Assert.That(SaveService.Load().retainedRound.stateHash,Is.EqualTo(saved.stateHash));
        }
        [UnityTest] public IEnumerator HomeRecoveryAndRetryUseSavedFailureThenOneNewAttempt()
        {
            var data=Fixture();Assert.That(State.TrySuspendRound(data),Is.True);State.Save=SaveService.Load();
            Router.Show(ScreenId.Home);Settle();yield return null;
            var view=Router.Viewport.GetComponentInChildren<ProductionFailView>();Assert.That(view,Is.Not.Null);
            Assert.That(State.GetLevelProgress(1,31).failures,Is.Zero);int energy=State.Save.energy;
            view.Retry.onClick.Invoke();view.Retry.onClick.Invoke();Settle();yield return null;
            Assert.That(Router.Viewport.GetComponentInChildren<Match3Board>(),Is.Not.Null);
            Assert.That(State.GetLevelProgress(1,31).failures,Is.EqualTo(1));Assert.That(State.GetLevelProgress(1,31).attempts,Is.EqualTo(2));
            Assert.That(State.Save.energy,Is.EqualTo(energy));Assert.That(State.Save.retainedRound,Is.Null);
        }
    }
}
