using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsCausalAnimationTests
 {
  bool sawCarriedPayload;
  GameObject root; readonly List<GameplayMotionEvent> events=new List<GameplayMotionEvent>();readonly List<float> times=new List<float>();
  [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
  MechanicsSandbox Open(){root=new GameObject("CausalAnimation",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
  static BoardModel Board(){var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));return b;}
  void Track(Match3Board b){events.Clear();times.Clear();b.MotionEventPublished+=m=>{events.Add(m);times.Add(Time.unscaledTime);};}
  void Capture(string name)=>MechanicsSandboxPlayModeTests.Capture(root,"CausalR13_"+name,576,1280);
  IEnumerator Observe(Match3Board b,string label)
  {
   sawCarriedPayload=false;
   float start=Time.unscaledTime,next=start,deadline=Time.realtimeSinceStartup+30;int frame=0;
   string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");
   while(b.HasActiveCommand&&Time.realtimeSinceStartup<deadline)
   {
    sawCarriedPayload|=b.GetComponentInChildren<SparkFlightView>().ActivePayloadCount>0;
    if(Time.unscaledTime>=next&&frame<20)
    {
     float at=Time.unscaledTime-start;Capture(label+"_"+frame.ToString("D2"));
     if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,label+"_Frames.tsv"),frame+"\t"+at.ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+"\n");
     frame++;next=Time.unscaledTime+.06f;
    }
    yield return null;
   }
   Assert.That(b.HasActiveCommand,Is.False);Assert.That(b.CanAcceptPointerInput,Is.True);
   Assert.That(b.BoardStateHash,Is.EqualTo(b.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
   Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.CommandRecovered),Is.False);
   Capture(label+"_After");
   if(!string.IsNullOrEmpty(dir))File.WriteAllLines(Path.Combine(dir,label+"_Events.tsv"),events.Select((e,i)=>times[i].ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+"\t"+e));
  }
  [UnityTest] public IEnumerator LineContactStartsBombChainAndClearTravelsAcrossRow()
  {
   var s=Open();s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board();b.SetCell(new BoardPosition(1,3),new BoardCellData(0,SpecialGemKind.Row));b.SetCell(new BoardPosition(6,3),new BoardCellData(1,SpecialGemKind.Burst));Assert.That(s.Board.LoadSnapshotForTesting(b.CreateSnapshot()),Is.True);Track(s.Board);Capture("Chain_Before");Assert.That(s.Board.TapCell(1,3),Is.True);yield return Observe(s.Board,"Chain");
   int line=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialActivationStarted&&e.SpecialKind==SpecialGemKind.Row),bomb=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialActivationStarted&&e.SpecialKind==SpecialGemKind.Burst);Assert.That(line,Is.GreaterThanOrEqualTo(0));Assert.That(bomb,Is.GreaterThan(line));Assert.That(times[bomb]-times[line],Is.GreaterThanOrEqualTo(.27f));
   var impacts=events.Where(e=>e.Kind==GameplayMotionKind.CellsImpacted&&e.ComboIndex==1).ToArray();Assert.That(impacts.Length,Is.GreaterThan(1));Assert.That(s.Board.Moves,Is.EqualTo(49));
  }
  [UnityTest] public IEnumerator RainbowShowsConversionsBeforeDirectionalRelease()
  {
   var s=Open();s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,SpecialGemKind.Color));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,SpecialGemKind.Row));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Track(s.Board);Capture("Conversion_Before");Assert.That(s.Board.SwipeCell(3,3,Vector2Int.right),Is.True);yield return Observe(s.Board,"Conversion");
   int convert=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialConversionStarted),release=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialActivationStarted&&e.SpecialKind==SpecialGemKind.Row);Assert.That(convert,Is.GreaterThanOrEqualTo(0));Assert.That(release,Is.GreaterThan(convert));Assert.That(times[release]-times[convert],Is.GreaterThanOrEqualTo(.07f));
   var convertedCells=new HashSet<BoardPosition>(events.Where(e=>e.Kind==GameplayMotionKind.SpecialConversionStarted&&e.ComboIndex==1).Select(e=>e.Secondary));
   var visibleClears=events.Where(e=>e.Kind==GameplayMotionKind.CellsImpacted&&e.ComboIndex==1).SelectMany(e=>e.ClearedCells).Where(c=>convertedCells.Contains(c.Position)).ToArray();
   Assert.That(visibleClears.Length,Is.EqualTo(convertedCells.Count));Assert.That(visibleClears.All(c=>c.Cell.Special==SpecialGemKind.Row),Is.True,"converted line items must not emit hidden ordinary-gem fragments");
  }
  [UnityTest] public IEnumerator FirebirdCarriesEachPayloadAndImpactsOnlyAfterFlight()
  {
   var s=Open();yield return null;
   foreach(var kind in new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst})
   {
    s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,SpecialGemKind.Spark));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,kind));b.SetObstacle(new BoardPosition(7,0),new BoardObstacleData(BoardObstacleKind.RunicSeal,2));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Track(s.Board);
    bool payloadVisible=false;s.Board.MotionEventPublished+=m=>{if(m.Kind==GameplayMotionKind.CellsImpacted&&s.Board.GetComponentInChildren<SparkFlightView>().LastPayload==kind)payloadVisible=true;};
    Capture("Payload"+kind+"_Before");Assert.That(s.Board.SwipeCell(3,3,Vector2Int.right),Is.True);yield return Observe(s.Board,"Payload"+kind);
    int launch=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialActivationStarted&&e.SpecialKind==SpecialGemKind.Spark),hit=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialPayloadImpact&&e.SpecialKind==kind);Assert.That(launch,Is.GreaterThanOrEqualTo(0));Assert.That(hit,Is.GreaterThan(launch));Assert.That(times[hit]-times[launch],Is.GreaterThanOrEqualTo(.48f));Assert.That(payloadVisible,Is.True);Assert.That(sawCarriedPayload,Is.True,"carried artwork must be active during flight");Assert.That(events[launch].Payload,Is.EqualTo(kind));
   }
  }
  [UnityTest] public IEnumerator CreationGathersAndRetryClearsPresentation()
  {
   var s=Open();yield return null;Track(s.Board);Capture("Creation_Before");Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);yield return Observe(s.Board,"Creation");
   int gather=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialGatherStarted),created=events.FindIndex(e=>e.Kind==GameplayMotionKind.SpecialCreationStarted);Assert.That(gather,Is.GreaterThanOrEqualTo(0));Assert.That(created,Is.GreaterThan(gather));Assert.That(times[created]-times[gather],Is.GreaterThan(.24f));
   s.Reset(10,GameplayRuleVersion.PrototypeR4);yield return null;Assert.That(s.Board.TapCell(3,3),Is.True);yield return null;s.OpenRound(0);yield return null;Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.GetComponentInChildren<SparkFlightView>().ActiveCount,Is.Zero);Assert.That(s.Board.GetComponentInChildren<PrototypeImpactView>().ActiveCount,Is.Zero);Assert.That(s.Board.GetComponent<PrototypeBoardAudio>().PendingCount,Is.Zero);
  }
 }
}
