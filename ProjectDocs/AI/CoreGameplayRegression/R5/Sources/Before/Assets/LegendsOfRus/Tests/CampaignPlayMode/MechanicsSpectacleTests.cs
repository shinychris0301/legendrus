using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsSpectacleTests
 {
  GameObject root;
  [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
  MechanicsSandbox Open(){root=new GameObject("SpectacleReview",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
  static BoardModel Fixture(SpecialGemKind kind)
  {
   var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%6));
   b.SetCell(new BoardPosition(3,3),new BoardCellData(0,kind));return b;
  }
  void Capture(string name)=>MechanicsSandboxPlayModeTests.Capture(root,"SpectacleR15_"+name,576,1280);
  IEnumerator Record(MechanicsSandbox s,string key)
  {
   float start=Time.unscaledTime,next=start,deadline=Time.realtimeSinceStartup+25;int frame=0,peak=0;bool fragments=false;
   string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");var impact=s.Board.GetComponentInChildren<PrototypeImpactView>();
   if(!string.IsNullOrEmpty(dir))File.WriteAllText(Path.Combine(dir,"SpectacleR15_"+key+"_Frames.tsv"),string.Empty);
   while((s.Board.HasActiveCommand||Time.unscaledTime-start<.85f)&&Time.realtimeSinceStartup<deadline)
   {
    fragments|=s.Board.GetComponentsInChildren<GemShatterView>().Any(v=>v.ActivePieces>0);
    if(frame<24&&Time.unscaledTime>=next&&Time.unscaledTime-start<1.5f)
    {
     float at=Time.unscaledTime-start;Capture(key+"_"+frame.ToString("D2"));
     var mesh=impact.canvasRenderer.GetMesh();peak=Math.Max(peak,mesh==null?0:mesh.vertexCount);
     if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,"SpectacleR15_"+key+"_Frames.tsv"),frame+"\t"+at.ToString("F4",CultureInfo.InvariantCulture)+"\n");
     frame++;next=Time.unscaledTime+.035f;
    }
    yield return null;
   }
   Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.CanAcceptPointerInput,Is.True);
   Assert.That(peak,Is.GreaterThan(0).And.LessThan(65535));
   if(key!="SeaPearl")Assert.That(fragments,Is.True,"actual clears must shatter approved gems");
   if(key=="Gems"||key=="Row"||key=="Column"||key=="Burst")Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
   else if(key!="SeaPearl")Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastPlanFinalStateHash));
   yield return new WaitForSecondsRealtime(.85f);Assert.That(impact.ActiveCount,Is.Zero);Assert.That(s.Board.GetComponentInChildren<PrototypeBoosterView>().IsPresenting,Is.False);Capture(key+"_After");
   if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,"Spectacle.txt"),$"{key}: peakVertices={peak}, fragments={fragments}, moves={s.Board.Moves}, finalHash={s.Board.BoardStateHash}\n");
  }
  [UnityTest] public IEnumerator DirectionalAndBombCommandsKeepHashesAndReleaseInput()
  {
   var s=Open();yield return null;
   foreach(var kind in new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst})
   {
    s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;s.Board.LoadSnapshotForTesting(Fixture(kind).CreateSnapshot());Capture(kind+"_Before");Assert.That(s.Board.TapCell(3,3),Is.True);yield return Record(s,kind.ToString());Assert.That(s.Board.Moves,Is.EqualTo(49));
   }
  }
  [UnityTest] public IEnumerator EveryInventoryBoosterHasAVisibleReleaseAndKeepsMoveCost()
  {
   var s=Open();yield return null;
   foreach(BoosterType type in Enum.GetValues(typeof(BoosterType)))
   {
    s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;s.Board.LoadSnapshotForTesting(Fixture(SpecialGemKind.None).CreateSnapshot());Capture(type+"_Before");
    s.Board.UseBooster(type);if(type!=BoosterType.SeaPearl)Assert.That(s.Board.TapCell(3,3),Is.True);
    Assert.That(s.Board.GetComponentInChildren<PrototypeBoosterView>().PresentationCount,Is.EqualTo(1));
    yield return Record(s,type.ToString());Assert.That(s.Board.Moves,Is.EqualTo(type==BoosterType.SeaPearl?55:50));
   }
  }
  [UnityTest] public IEnumerator OrdinaryMatchShowsTextureFragmentsAndColoredChips()
  {
   var s=Open();s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Fixture(SpecialGemKind.None);b.SetCell(new BoardPosition(1,3),new BoardCellData(2));
   b.SetCell(new BoardPosition(2,3),new BoardCellData(1));b.SetCell(new BoardPosition(3,3),new BoardCellData(1));b.SetCell(new BoardPosition(4,3),new BoardCellData(2));b.SetCell(new BoardPosition(4,2),new BoardCellData(1));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Capture("Gems_Before");Assert.That(s.Board.SwipeCell(4,2,new Vector2Int(0,1)),Is.True);yield return Record(s,"Gems");Assert.That(s.Board.Moves,Is.EqualTo(49));
  }
  [UnityTest] public IEnumerator RichEffectsAreBoundedRecoverableAndIgnoreDisabledEvents()
  {
   var s=Open();yield return null;var v=s.Board.GetComponentInChildren<PrototypeImpactView>();
   var cue=new GameplayMotionEvent(1,GameplayMotionKind.SpecialActivationStarted,new BoardPosition(3,3),new BoardPosition(3,3),1,1,SpecialGemKind.Burst,"");
   for(int i=0;i<240;i++)v.SendMessage("Motion",cue);yield return new WaitForSecondsRealtime(.15f);Canvas.ForceUpdateCanvases();
   Assert.That(v.GetComponentInParent<RectMask2D>(),Is.Not.Null);Assert.That(v.ActiveCount,Is.LessThanOrEqualTo(192));var mesh=v.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0).And.LessThan(65535));Assert.That(mesh.vertices.All(p=>!float.IsNaN(p.x)&&!float.IsInfinity(p.y)),Is.True);
   v.SendMessage("Motion",new GameplayMotionEvent(2,GameplayMotionKind.CommandRecovered,new BoardPosition(0,0),new BoardPosition(0,0),0,0,SpecialGemKind.None,""));Assert.That(v.ActiveCount,Is.Zero);
   v.enabled=false;v.SendMessage("Motion",cue);Assert.That(v.ActiveCount,Is.Zero);v.enabled=true;v.SendMessage("Motion",cue);yield return new WaitForSecondsRealtime(.8f);Assert.That(v.ActiveCount,Is.Zero);
   var art=s.Board.GetComponentInChildren<PrototypeBoosterView>();art.enabled=false;art.SendMessage("Motion",new GameplayMotionEvent(3,GameplayMotionKind.BoosterActivated,new BoardPosition(2,2),new BoardPosition(2,2),0,1,SpecialGemKind.None,"",boosterKind:GameplayBoosterKind.Firebird));Assert.That(art.IsPresenting,Is.False);
  }
 }
}
