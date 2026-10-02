using System;
using System.Linq;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests
{
 public class MechanicsTimelineTests
 {
  static BoardModel Board(){var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));return b;}
  static BoardResolutionPlanner Planner(BoardModel b)=>new BoardResolutionPlanner(b,BoardBalanceProfile.CommercialDefault,GameplayRuleVersion.PrototypeR4);
  [Test] public void ChainWaitsForIncomingSweepAndOrdinaryCellsClearInTravelOrder(){var b=Board();b.SetCell(new BoardPosition(1,3),new BoardCellData(0,SpecialGemKind.Row));b.SetCell(new BoardPosition(6,3),new BoardCellData(1,SpecialGemKind.Burst));var p=Planner(b).PlanTapSpecial(new BoardPosition(1,3));var w=p.Waves[0];var t=new BoardPresentationTimeline(w,p.BeforeSnapshot);int line=w.Activations.ToList().FindIndex(a=>a.Kind==SpecialGemKind.Row),bomb=w.Activations.ToList().FindIndex(a=>a.Kind==SpecialGemKind.Burst);Assert.That(t.ActivationTimes[line],Is.Zero);Assert.That(t.ActivationTimes[bomb],Is.EqualTo(.12f+.28f*5/8).Within(.001f));Assert.That(t.UnlinkedActivations,Is.Zero);int near=w.ClearedCells.ToList().FindIndex(c=>c.Position==new BoardPosition(2,3)),far=w.ClearedCells.ToList().FindIndex(c=>c.Position==new BoardPosition(4,3));Assert.That(t.ClearTimes[near],Is.LessThan(t.ClearTimes[far]));}
  [TestCase(SpecialGemKind.Row)][TestCase(SpecialGemKind.Column)][TestCase(SpecialGemKind.Burst)][TestCase(SpecialGemKind.Spark)]
  public void RainbowConversionsHaveAVisibleHoldBeforeActivation(SpecialGemKind partner){var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,SpecialGemKind.Color));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,partner));var p=Planner(b).PlanSwap(new BoardPosition(3,3),new BoardPosition(4,3));var t=new BoardPresentationTimeline(p.Waves[0],p.SnapshotAfterSwap);Assert.That(t.Converted.Count(v=>v),Is.GreaterThan(0));for(int i=0;i<t.Converted.Length;i++)if(t.Converted[i])Assert.That(t.ActivationTimes[i],Is.EqualTo(BoardPresentationTimeline.Conversion));}
  [TestCase(SpecialGemKind.Row)][TestCase(SpecialGemKind.Column)][TestCase(SpecialGemKind.Burst)]
  public void PayloadDoesNotHitRemoteCellsBeforeArrival(SpecialGemKind payload){var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,SpecialGemKind.Spark));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,payload));b.SetObstacle(new BoardPosition(7,0),new BoardObstacleData(BoardObstacleKind.RunicSeal,2));var p=Planner(b).PlanSwap(new BoardPosition(3,3),new BoardPosition(4,3));var w=p.Waves[0];var t=new BoardPresentationTimeline(w,p.SnapshotAfterSwap);var flight=w.Activations.First(a=>a.Kind==SpecialGemKind.Spark);Assert.That(flight.Payload,Is.EqualTo(payload));int hit=w.ObstacleInteractions.ToList().FindIndex(o=>o.Position==flight.Target.Value);Assert.That(hit,Is.GreaterThanOrEqualTo(0));Assert.That(t.ObstacleTimes[hit],Is.GreaterThanOrEqualTo(.52f));}
  [Test] public void AllSpecialPairTimelinesAreFiniteAndLinked()
  {
   var kinds=new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst,SpecialGemKind.Color,SpecialGemKind.Spark};
   for(int i=0;i<kinds.Length;i++)for(int j=i;j<kinds.Length;j++)
   {
    var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,kinds[i]));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,kinds[j]));
    var p=Planner(b).PlanSwap(new BoardPosition(3,3),new BoardPosition(4,3));var t=new BoardPresentationTimeline(p.Waves[0],p.SnapshotAfterSwap);
    Assert.That(t.UnlinkedActivations,Is.Zero,kinds[i]+"/"+kinds[j]);Assert.That(t.Duration,Is.LessThan(5));Assert.That(t.ClearTimes.All(v=>!float.IsInfinity(v)&&!float.IsNaN(v)&&v>=0),Is.True);
   }
  }
  [Test] public void TimelineLeavesPlanAndReplayStateUnchanged(){var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(0,SpecialGemKind.Burst));var p=Planner(b).PlanTapSpecial(new BoardPosition(3,3));string before=b.CreateSnapshot().ToReplayToken(),hash=p.PlanHash,after=p.FinalSnapshot.ToReplayToken();var a=new BoardPresentationTimeline(p.Waves[0],p.BeforeSnapshot);var c=new BoardPresentationTimeline(p.Waves[0],p.BeforeSnapshot);Assert.That(a.ActivationTimes,Is.EqualTo(c.ActivationTimes));Assert.That(a.ClearTimes,Is.EqualTo(c.ClearTimes));Assert.That(p.PlanHash,Is.EqualTo(hash));Assert.That(b.CreateSnapshot().ToReplayToken(),Is.EqualTo(before));Assert.That(p.FinalSnapshot.ToReplayToken(),Is.EqualTo(after));}
 }
}
