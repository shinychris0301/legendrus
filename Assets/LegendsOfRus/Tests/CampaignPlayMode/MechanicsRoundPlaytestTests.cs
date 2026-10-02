using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.TestTools;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class MechanicsRoundPlaytestTests
    {
        GameObject root;
        class Node {public BoardSnapshot Board;public LevelGoalRuntimeSnapshot Goals;public int Score;public List<MechanicsReplayCommand> Path=new List<MechanicsReplayCommand>();public float Cost;}
        [Serializable] class Witness {public string title;public int moveLimit;public int solutionMoves;public MechanicsReplayCommand[] commands;}
        static BoardModel Model(BoardSnapshot s){var b=new BoardModel(s.Width,s.Height,s.TileTypeCount,new SeededBoardRandom(1)){SquareMatches=true,StandaloneSpecials=true};b.RestoreSnapshot(s);return b;}
        static IEnumerable<MechanicsReplayCommand> Actions(BoardSnapshot snapshot)
        {
            var b=Model(snapshot);string token=snapshot.ToReplayToken();
            for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++)
            {
                var p=new BoardPosition(x,y);if(b.GetSpecial(p)!=SpecialGemKind.None)yield return new MechanicsReplayCommand{Rules=GameplayRuleVersion.PrototypeR4,BeforeToken=token,Tap=true,X=x,Y=y};
                foreach(var q in new[]{new BoardPosition(x+1,y),new BoardPosition(x,y+1)})
                    if(b.IsInside(q)&&b.CanSwap(p,q)&&(b.GetSpecial(p)!=SpecialGemKind.None||b.GetSpecial(q)!=SpecialGemKind.None||b.SwapCreatesMatch(p,q)))
                        yield return new MechanicsReplayCommand{Rules=GameplayRuleVersion.PrototypeR4,BeforeToken=token,X=x,Y=y,ToX=q.X,ToY=q.Y};
            }
        }
        // Bounded deterministic witness search; does not claim optimality or a win rate.
        static List<MechanicsReplayCommand> Solve(MechanicsPlaytestRound round,bool lose=false)
        {
            var goals=new LevelGoalRuntime(round.Level,0);var nodes=new List<Node>{new Node{Board=round.Snapshot,Goals=goals.CaptureSnapshot()}};int expanded=0;
            for(int depth=0;depth<round.Level.moveLimit;depth++)
            {
                var next=new List<Node>();var seen=new HashSet<string>();
                foreach(var n in nodes)foreach(var action in depth==0&&round.Level.levelId==1 ? new[]{new MechanicsReplayCommand{Rules=GameplayRuleVersion.PrototypeR4,BeforeToken=n.Board.ToReplayToken(),X=3,Y=3,ToX=3,ToY=2}} : depth==0&&round.Level.levelId==3 ? new[]{new MechanicsReplayCommand{Rules=GameplayRuleVersion.PrototypeR4,BeforeToken=n.Board.ToReplayToken(),X=4,Y=3,ToX=3,ToY=3}} : Actions(n.Board))
                {
                    if(++expanded>6000)Assert.Fail("Witness search budget exceeded: "+round.Title);
                    var plan=action.Resolve();if(!plan.IsResolved||!plan.ConsumesMove)continue;
                    var g=new LevelGoalRuntime(round.Level,0);g.RestoreSnapshot(n.Goals);int score=n.Score;
                    foreach(var wave in plan.Waves){score+=wave.ScoreDelta;g.ApplyWave(wave,score);}g.RecordConsumedMove();
                    action.ExpectedPlanHash=plan.PlanHash;var path=new List<MechanicsReplayCommand>(n.Path){action};
                    if(g.AllComplete){if(!lose)return path;continue;}
                    if(lose&&depth==round.Level.moveLimit-1)return path;
                    string key=plan.FinalSnapshot.StateHash+":"+string.Join(",",Enumerable.Range(0,g.SlotCount).Select(i=>g.RemainingAt(i)));
                    if(!seen.Add(key))continue;
                    float cost=Enumerable.Range(0,g.SlotCount).Sum(i=>g.RemainingAt(i))*100+plan.FinalSnapshot.ObstacleLayers.Sum(x=>(int)x)*6-plan.FinalSnapshot.Specials.Count(x=>x!=SpecialGemKind.None)*2;
                    if(lose)cost=-cost;
                    next.Add(new Node{Board=plan.FinalSnapshot,Goals=g.CaptureSnapshot(),Score=score,Path=path,Cost=cost});
                }
                nodes=next.OrderBy(n=>n.Cost).ThenBy(n=>n.Board.StateHash,StringComparer.Ordinal).Take(10).ToList();
            }
            Assert.Fail("No inventory-free witness within proposed budget: "+round.Title);return null;
        }
        static string Output {get {var p=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(string.IsNullOrEmpty(p))p=Path.Combine(Application.temporaryCachePath,"MechanicsRoundPlaytest");Directory.CreateDirectory(p);return p;}}
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
        [UnityTest] public IEnumerator AllFiveRoundsWinWithoutInventoryAndRetryRestoresExactStart()
        {
            root=new GameObject("RoundCapture",typeof(RectTransform),typeof(Canvas));var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);yield return null;
            for(int i=0;i<MechanicsPlaytestRounds.Count;i++)
            {
                var round=MechanicsPlaytestRounds.Create(i);var path=Solve(round);File.WriteAllText(Path.Combine(Output,"Round"+(i+1)+"_Witness.json"),JsonUtility.ToJson(new Witness{title=round.Title,moveLimit=round.Level.moveLimit,solutionMoves=path.Count,commands=path.ToArray()},true));
                s.OpenRound(i);yield return null;Assert.That(s.Board.Moves,Is.EqualTo(round.Level.moveLimit));Assert.That(s.Board.BoardStateHash,Is.EqualTo(round.Snapshot.StateHash));
                s.Board.UseBooster(BoosterType.BogatyrMace);s.Board.TapCell(0,0);yield return null;s.Board.CancelArmedBooster(false);Assert.That(s.Board.BoardStateHash,Is.EqualTo(round.Snapshot.StateHash));Assert.That(s.Board.Moves,Is.EqualTo(round.Level.moveLimit));Assert.That(s.Board.LastRejectedCommandReason,Does.Contain("empty"));
                if(i==0||i==2)Assert.That(path[0].Resolve().Waves.SelectMany(w=>w.Creations).Any(c=>c.Kind==(i==0?SpecialGemKind.Column:SpecialGemKind.Spark)),Is.True,"Teaching witness creates its intended special");
                MechanicsSandboxPlayModeTests.Capture(root,"Round_"+(i+1)+"_Start",576,1280);
                foreach(var action in path)
                {
                    Assert.That(s.Board.BoardReplayToken,Is.EqualTo(action.BeforeToken));bool accepted=action.Tap?s.Board.TapCell(action.X,action.Y):s.Board.SwipeCell(action.X,action.Y,new Vector2Int(action.ToX-action.X,action.ToY-action.Y));Assert.That(accepted,Is.True);
                    float end=Time.realtimeSinceStartup+25;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.BoardStateHash,Is.EqualTo(action.Resolve().FinalSnapshot.StateHash));
                }
                Assert.That(s.Board.Phase,Is.EqualTo(GameplayPhase.Completed));Assert.That(s.Board.Moves,Is.EqualTo(round.Level.moveLimit-path.Count));for(int g=0;g<s.Board.GoalCount;g++)Assert.That(s.Board.GetGoalSlot(g).IsComplete,Is.True);
                yield return null;MechanicsSandboxPlayModeTests.Capture(root,"Round_"+(i+1)+"_Win",576,1280);
                s.OpenRound(i);yield return null;Assert.That(s.Board.BoardStateHash,Is.EqualTo(round.Snapshot.StateHash));Assert.That(s.Board.Moves,Is.EqualTo(round.Level.moveLimit));Assert.That(s.Board.GoalSlot1Remaining,Is.EqualTo(round.Level.goals[0].target));
            }
            s.Reset(0,GameplayRuleVersion.Legacy);yield return null;Assert.That(s.IsRoundPlaytest,Is.False);Assert.That(s.Board.Moves,Is.EqualTo(50));
        }
        [UnityTest] public IEnumerator ExhaustionStopsInputAndRetryRestoresRound()
        {
            root=new GameObject("RoundFailure",typeof(RectTransform),typeof(Canvas));var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);s.OpenRound(4);yield return null;
            int budget=s.Board.Moves;var losingPath=Solve(MechanicsPlaytestRounds.Create(4),true);Assert.That(losingPath.Count,Is.EqualTo(budget));
            File.WriteAllText(Path.Combine(Output,"Round5_LosingWitness.json"),JsonUtility.ToJson(new Witness{title="Move exhaustion",moveLimit=budget,solutionMoves=losingPath.Count,commands=losingPath.ToArray()},true));
            foreach(var action in losingPath)
            {
                Assert.That(action.Tap?s.Board.TapCell(action.X,action.Y):s.Board.SwipeCell(action.X,action.Y,new Vector2Int(action.ToX-action.X,action.ToY-action.Y)),Is.True);
                float end=Time.realtimeSinceStartup+25;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(s.Board.HasActiveCommand,Is.False);
            }
            Assert.That(s.Board.Moves,Is.Zero);Assert.That(s.Board.Phase,Is.EqualTo(GameplayPhase.Failed));Assert.That(s.Board.CanAcceptPointerInput,Is.False);yield return null;MechanicsSandboxPlayModeTests.Capture(root,"Round_5_Fail",576,1280);
            s.OpenRound(4);yield return null;Assert.That(s.Board.Moves,Is.EqualTo(budget));Assert.That(s.Board.CanAcceptPointerInput,Is.True);Assert.That(s.Board.GoalSlot1Remaining,Is.EqualTo(MechanicsPlaytestRounds.Create(4).Level.goals[0].target));
        }
    }
}
