using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsMaterialFeedbackTests
 {
  GameObject root;
  MechanicsSandbox Open(){root=new GameObject("MaterialReview",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
  [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
  static BoardModel Board(BoardObstacleKind kind,int layers)
  {
   var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%6));
   b.SetCell(new BoardPosition(1,3),new BoardCellData(0,SpecialGemKind.Row));b.SetObstacle(new BoardPosition(6,3),new BoardObstacleData(kind,layers));return b;
  }
  void Capture(string name)=>MechanicsSandboxPlayModeTests.Capture(root,"MaterialR14_"+name,576,1280);
  IEnumerator ShowMaterial(BoardObstacleKind kind)
  {
   var s=Open();yield return null;
   foreach(int layers in new[]{2,1})
   {
    s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board(kind,layers);Assert.That(s.Board.LoadSnapshotForTesting(b.CreateSnapshot()),Is.True);
    var view=s.Board.GetComponentInChildren<MaterialBreakView>();Assert.That(view,Is.Not.Null);Assert.That(view.raycastTarget,Is.False);
    bool actualHit=false,matchedLayers=false;float hitTime=-1;int maxVertices=0;
    s.Board.ObstacleInteracted+=o=>{if(o.Position==new BoardPosition(6,3)){actualHit=true;matchedLayers|=o.LayersBefore==layers&&o.LayersAfter==layers-1;if(hitTime<0)hitTime=Time.unscaledTime;}};
    string label=kind+(layers==2?"_First":"_Final");Capture(label+"_Before");Assert.That(s.Board.TapCell(1,3),Is.True);
    float start=Time.unscaledTime,next=start,deadline=Time.realtimeSinceStartup+25;int frame=0;string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");
    if(!string.IsNullOrEmpty(dir))File.WriteAllText(Path.Combine(dir,"MaterialR14_"+label+"_Frames.tsv"),string.Empty);
    while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)
    {
     if(Time.unscaledTime>=next&&frame<16)
     {
      float at=Time.unscaledTime-start;Capture(label+"_"+frame.ToString("D2"));
      var mesh=view.canvasRenderer.GetMesh();maxVertices=Math.Max(maxVertices,mesh==null?0:mesh.vertexCount);
      if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,"MaterialR14_"+label+"_Frames.tsv"),frame+"\t"+at.ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+"\n");
      frame++;next=Time.unscaledTime+.05f;
     }
     yield return null;
    }
    Assert.That(actualHit&&matchedLayers,Is.True,"actual planned layer transition");Assert.That(maxVertices,Is.GreaterThan(0).And.LessThan(60000));Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.CanAcceptPointerInput,Is.True);Assert.That(s.Board.Moves,Is.EqualTo(49));Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
    Assert.That(view.HitCount,Is.GreaterThan(0));if(layers==1)Assert.That(view.FinalBreakCount,Is.GreaterThan(0));Capture(label+"_After");
    if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,"Materials.txt"),$"{label}: actualHit={actualHit}, matchingLayers={matchedLayers}, meshPeak={maxVertices}, hitAt={hitTime-start}, plan={s.Board.LastMechanicsCommand.ExpectedPlanHash}\n");
   }
  }
  [UnityTest] public IEnumerator WoodSplintersDistinguishDamageAndBreak()=>ShowMaterial(BoardObstacleKind.Crate);
  [UnityTest] public IEnumerator IceFacetsDistinguishDamageAndBreak()=>ShowMaterial(BoardObstacleKind.Ice);
  [UnityTest] public IEnumerator MetalLinksDistinguishDamageAndBreak()=>ShowMaterial(BoardObstacleKind.Chain);
  [UnityTest] public IEnumerator SealFragmentsDistinguishDamageAndBreak()=>ShowMaterial(BoardObstacleKind.RunicSeal);

  [UnityTest] public IEnumerator MaterialPoolIsBoundedClippedAndRecoveryClearsIt()
  {
   var s=Open();yield return null;var view=s.Board.GetComponentInChildren<MaterialBreakView>();Assert.That(view.GetComponentInParent<RectMask2D>(),Is.Not.Null);
   for(int i=0;i<200;i++)view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(i%8,(i/8)%7),BoardObstacleKind.Chain,1,0,BoardObstacleHitCause.DirectClear));
   Assert.That(view.ActiveCount,Is.EqualTo(MaterialBreakView.Capacity));Assert.That(view.DroppedCount,Is.EqualTo(104));
   yield return null;Canvas.ForceUpdateCanvases();var mesh=view.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0).And.LessThan(60000));
   Assert.That(mesh.vertices.All(v=>!float.IsNaN(v.x)&&!float.IsNaN(v.y)),Is.True);
   view.SendMessage("OnMotion",new GameplayMotionEvent(1,GameplayMotionKind.CommandRecovered,new BoardPosition(0,0),new BoardPosition(0,0),0,0,SpecialGemKind.None,""));Assert.That(view.ActiveCount,Is.Zero);
   int count=view.HitCount;view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(2,2),BoardObstacleKind.Ice,2,2,BoardObstacleHitCause.DirectClear));Assert.That(view.HitCount,Is.EqualTo(count));Assert.That(view.ActiveCount,Is.Zero);
   view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(2,2),BoardObstacleKind.Ice,2,1,BoardObstacleHitCause.DirectClear));view.enabled=false;Assert.That(view.ActiveCount,Is.Zero);view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(2,2),BoardObstacleKind.Ice,2,1,BoardObstacleHitCause.DirectClear));Assert.That(view.ActiveCount,Is.Zero,"disabled view cannot accumulate stale impacts");view.enabled=true;
   view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(2,2),BoardObstacleKind.Ice,2,1,BoardObstacleHitCause.DirectClear));yield return new WaitForSecondsRealtime(.7f);Assert.That(view.ActiveCount,Is.Zero);
  }
  [UnityTest] public IEnumerator MaterialMeshesHaveDistinctSilhouettesAndSpecialsDoNotEmitGemGlints()
  {
   var s=Open();yield return null;var view=s.Board.GetComponentInChildren<MaterialBreakView>();var vertexCounts=new HashSet<int>();
   foreach(var kind in new[]{BoardObstacleKind.Crate,BoardObstacleKind.Ice,BoardObstacleKind.Chain,BoardObstacleKind.RunicSeal})
   {
    view.enabled=false;view.enabled=true;view.SendMessage("OnObstacle",new BoardObstacleInteractionInstruction(new BoardPosition(3,3),kind,1,0,BoardObstacleHitCause.DirectClear));yield return null;Canvas.ForceUpdateCanvases();vertexCounts.Add(view.canvasRenderer.GetMesh().vertexCount);
   }
   Assert.That(vertexCounts.Count,Is.EqualTo(4));view.enabled=false;view.enabled=true;
   var cells=new[]{new BoardClearedCellInstruction(new BoardPosition(2,2),new BoardCellData(0,SpecialGemKind.Burst))};
   view.SendMessage("OnMotion",new GameplayMotionEvent(1,GameplayMotionKind.CellsImpacted,new BoardPosition(2,2),new BoardPosition(2,2),1,1,SpecialGemKind.None,"",clearedCells:cells));Assert.That(view.ActiveCount,Is.Zero);
   var legacy=s.Board;s.Reset(0,GameplayRuleVersion.PrototypeR3);yield return null;Assert.That(s.Board.GetComponentInChildren<MaterialBreakView>(),Is.Null);
  }
  [UnityTest] public IEnumerator FineChipsDisperseInAllDirectionsAndExpireBeforeLargeShards()
  {
   var s=Open();yield return null;var view=s.Board.GetComponentsInChildren<GemShatterView>()[0];
   var cell=new BoardPosition(3,3);var cleared=new[]{new BoardClearedCellInstruction(cell,new BoardCellData(0))};
   var cue=new GameplayMotionEvent(1,GameplayMotionKind.CellsImpacted,cell,cell,1,1,SpecialGemKind.None,"",clearedCells:cleared);
   view.SendMessage("Motion",cue);Canvas.ForceUpdateCanvases();Assert.That(view.ActivePieces,Is.EqualTo(24));
   Assert.That(view.canvasRenderer.GetMesh().vertexCount,Is.EqualTo(88),"eight triangular shards and sixteen four-vertex chips");
   Assert.That(view.GetComponentInParent<RectMask2D>(),Is.Not.Null);
   var quadrants=new HashSet<int>();for(int i=0;i<16;i++){var p=GemShatterView.ChipOffset(cell,i,1,.15f);quadrants.Add((p.x>0?1:0)+(p.y>0?2:0));}
   Assert.That(quadrants.Count,Is.EqualTo(4),"scatter must not collapse into one fan");
   yield return new WaitForSecondsRealtime(.56f);Canvas.ForceUpdateCanvases();Assert.That(view.canvasRenderer.GetMesh().vertexCount,Is.EqualTo(24),"fine chips fade before the larger material pieces");
   yield return new WaitForSecondsRealtime(.14f);yield return null;Canvas.ForceUpdateCanvases();Assert.That(view.ActivePieces,Is.Zero);Assert.That(view.canvasRenderer.GetMesh().vertexCount,Is.Zero);
  }
  [UnityTest] public IEnumerator MatchedGemsUseApprovedTextureAndFacetedBreak()
  {
   var s=Open();s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board(BoardObstacleKind.None,0);b.SetCell(new BoardPosition(1,3),new BoardCellData(2));
   b.SetCell(new BoardPosition(2,3),new BoardCellData(1));b.SetCell(new BoardPosition(3,3),new BoardCellData(1));b.SetCell(new BoardPosition(4,3),new BoardCellData(2));b.SetCell(new BoardPosition(4,2),new BoardCellData(1));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Capture("Gems_Before");Assert.That(s.Board.SwipeCell(4,2,new Vector2Int(0,1)),Is.True);
   float deadline=Time.realtimeSinceStartup+20;int frame=0;float next=Time.unscaledTime,start=next;bool fragments=false;string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(!string.IsNullOrEmpty(dir))File.WriteAllText(Path.Combine(dir,"MaterialR14_Gems_Frames.tsv"),string.Empty);
   while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)
   {
    fragments|=s.Board.GetComponentsInChildren<GemShatterView>().Any(v=>v.ActivePieces>0);
    if(Time.unscaledTime>=next&&frame<16){float at=Time.unscaledTime-start;Capture("Gems_"+frame.ToString("D2"));if(!string.IsNullOrEmpty(dir))File.AppendAllText(Path.Combine(dir,"MaterialR14_Gems_Frames.tsv"),frame+"\t"+at.ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+"\n");frame++;next=Time.unscaledTime+.05f;}yield return null;
   }
   Assert.That(fragments,Is.True);Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));Capture("Gems_After");
   var views=s.Board.GetComponentsInChildren<GemShatterView>();for(int i=0;i<6;i++){Assert.That(views[i].UsesArtwork,Is.True);Assert.That(views[i].FragmentRegion,Is.EqualTo(MechanicsSequenceArt.GemRegion(i)));Assert.That(views[i].mainTexture,Is.SameAs(ProductionGameplayArt.Gem(i).texture));}
  }
 }
}
