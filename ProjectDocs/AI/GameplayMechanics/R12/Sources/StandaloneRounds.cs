using System;using System.Collections.Generic;using System.Linq;using NUnit.Framework;using LegendsOfRus;using LegendsOfRus.GameplayCore;class StandaloneRounds {
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

static void Main(){for(int i=0;i<5;i++){var r=MechanicsPlaytestRounds.Create(i);var p=Solve(r);Console.WriteLine("Round "+(i+1)+" budget="+r.Level.moveLimit+" solved="+p.Count);foreach(var a in p)Console.WriteLine("Witness\t"+(i+1)+"\t"+a.Tap+"\t"+a.X+"\t"+a.Y+"\t"+a.ToX+"\t"+a.ToY+"\t"+a.BeforeToken+"\t"+a.ExpectedPlanHash);}}}