using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsCreationFinishTests
 {
  GameObject root; MechanicsSequenceArtTests.BufferedRecorder recorder;
  [UnityTearDown]public IEnumerator Cleanup(){recorder?.Dispose();recorder=null;if(root)UnityEngine.Object.Destroy(root);yield return null;}
  MechanicsSandbox Open(){root=new GameObject("CreationFinishReview",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
  int captureFrame;float captureStart,captureNext;string captureKey,capturePath;
  void BeginCapture(string key){captureFrame=0;captureStart=captureNext=Time.unscaledTime;captureKey=key;string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");capturePath=string.IsNullOrEmpty(dir)?null:Path.Combine(dir,"CreationR21_"+key+"_Frames.tsv");if(capturePath!=null){Directory.CreateDirectory(Path.GetDirectoryName(capturePath));File.WriteAllText(capturePath,"");}Capture(key+"_Before");recorder=new MechanicsSequenceArtTests.BufferedRecorder(root,key,"CreationR21");}
  void Capture(string key){if(key.EndsWith("_After")){recorder?.Dispose();recorder=null;}MechanicsSandboxPlayModeTests.Capture(root,"CreationR21_"+key,576,1280);}
  void Sample(){if(Time.unscaledTime<captureNext||captureFrame>=100)return;float at=Time.unscaledTime-captureStart;recorder?.Sample();if(capturePath!=null)File.AppendAllText(capturePath,captureFrame+"\t"+at.ToString("F4",CultureInfo.InvariantCulture)+"\n");captureFrame++;captureNext=Time.unscaledTime+.025f;}
  [UnityTest]public IEnumerator TwoCreationsFinishTogetherAndReturnToExactIdleArt()
  {
   var s=Open();s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;
   var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));
   for(int x=0;x<3;x++){b.SetCell(new BoardPosition(x,2),new BoardCellData(5));b.SetCell(new BoardPosition(x,3),new BoardCellData(4));}
   b.SetCell(new BoardPosition(3,2),new BoardCellData(4));b.SetCell(new BoardPosition(3,3),new BoardCellData(5));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());
   var creations=new List<GameplayMotionEvent>();var at=new List<float>();
   s.Board.MotionEventPublished+=e=>{if(e.Kind==GameplayMotionKind.SpecialCreationStarted&&e.ComboIndex==1){creations.Add(e);at.Add(Time.unscaledTime);}};
   BeginCapture("Multiple");Assert.That(s.Board.SwipeCell(3,2,Vector2Int.up),Is.True);bool sawAssembledHold=false;float deadline=Time.realtimeSinceStartup+20;
   while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)
   {
    var views=s.Board.GetComponentsInChildren<SpecialCreationGraphic>();
    foreach(var view in views){Assert.That(view.raycastTarget,Is.False);Assert.That(view.GetComponentInParent<RectMask2D>(),Is.Not.Null);}
    var finished=views.Where(v=>v.FullyAssembled).ToArray();
    if(finished.Length>=2){sawAssembledHold=true;Canvas.ForceUpdateCanvases();foreach(var v in finished)Assert.That(v.canvasRenderer.GetMesh().vertexCount,Is.EqualTo(4),"settled art must be a seamless full sprite");}
    Sample();yield return null;
   }
   Capture("Multiple_After");
   Assert.That(creations.Count,Is.GreaterThanOrEqualTo(2));Assert.That(at.Max()-at.Min(),Is.LessThan(.02f),"same-wave creations must start together");Assert.That(sawAssembledHold,Is.True);
   Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.CanAcceptPointerInput,Is.True);Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));Assert.That(s.Board.Moves,Is.EqualTo(49));
   Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().All(v=>!v.Active),Is.True);
   foreach(var image in s.Board.GetComponentsInChildren<Image>().Where(i=>i.name=="Special"&&i.enabled)){Assert.That(image.color,Is.EqualTo(Color.white));Assert.That(image.transform.parent.localScale,Is.EqualTo(Vector3.one));}
  }
  [UnityTest]public IEnumerator ColorAndSparkCreationUseTheirExactIdleArtwork()
  {
   var s=Open();yield return null;
   foreach(bool spark in new[]{false,true})
   {
    s.Reset(spark?9:0,GameplayRuleVersion.PrototypeR4);yield return null;
    if(!spark){var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));for(int x=1;x<=5;x++)b.SetCell(new BoardPosition(x,3),new BoardCellData(5));b.SetCell(new BoardPosition(3,3),new BoardCellData(0));b.SetCell(new BoardPosition(3,4),new BoardCellData(5));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());}
    string key=spark?"Spark":"Color";BeginCapture(key);bool created=false;
    s.Board.MotionEventPublished+=e=>{if(e.Kind==GameplayMotionKind.SpecialCreationStarted&&e.SpecialKind==(spark?SpecialGemKind.Spark:SpecialGemKind.Color))created=true;};
    Assert.That(spark?s.Board.SwipeCell(4,3,Vector2Int.left):s.Board.SwipeCell(3,4,Vector2Int.down),Is.True);float deadline=Time.realtimeSinceStartup+20;
    while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<deadline){Sample();yield return null;}
    Assert.That(created,Is.True);Assert.That(s.Board.CanAcceptPointerInput,Is.True);Assert.That(s.Board.Moves,Is.EqualTo(49));Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().Any(v=>v.Artwork==PrototypeBoardArt.Load(spark?"spark":"color")),Is.True);Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().All(v=>!v.Active),Is.True);Capture(key+"_After");
   }
  }
  [UnityTest]public IEnumerator FormationAccentsAreBoundedClippedAndClearedOnReuseOrDisable()
  {
   root=new GameObject("CreationLayerLifecycle",typeof(RectTransform),typeof(Canvas),typeof(RectMask2D));
   var anchor=new GameObject("Anchor",typeof(RectTransform));anchor.transform.SetParent(root.transform,false);((RectTransform)anchor.transform).sizeDelta=new Vector2(80,80);
   var go=new GameObject("Body",typeof(RectTransform),typeof(SpecialCreationGraphic));go.transform.SetParent(root.transform,false);var body=go.GetComponent<SpecialCreationGraphic>();
   Assert.That(CreationAccentsGraphic.Atlas,Is.Not.Null);
   var kinds=new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst,SpecialGemKind.Color,SpecialGemKind.Spark};
   foreach(var kind in kinds)
   {
    string key=kind==SpecialGemKind.Burst?"bomb":kind==SpecialGemKind.Color?"color":kind==SpecialGemKind.Spark?"spark":"line";
    body.Begin(PrototypeBoardArt.Load(key),(RectTransform)anchor.transform,kind);
    yield return new WaitForSecondsRealtime(.12f);Canvas.ForceUpdateCanvases();
    var layers=root.GetComponentsInChildren<CreationAccentsGraphic>();Assert.That(layers.Length,Is.EqualTo(1),"reuse rather than accumulate effect objects");
    var layer=layers[0];Assert.That(layer.raycastTarget,Is.False);Assert.That(layer.GetComponentInParent<RectMask2D>(),Is.Not.Null);
    Assert.That(layer.canvasRenderer.GetMesh().vertexCount,Is.InRange(4,8));Assert.That(body.canvasRenderer.GetMesh().vertexCount,Is.InRange(4,256));
    body.Settle();yield return new WaitForSecondsRealtime(SpecialCreationGraphic.SettleSeconds);Canvas.ForceUpdateCanvases();
    Assert.That(body.FullyAssembled,Is.True);Assert.That(body.canvasRenderer.GetMesh().vertexCount,Is.EqualTo(4));
    body.Finish();Canvas.ForceUpdateCanvases();Assert.That(layer.canvasRenderer.GetMesh().vertexCount,Is.Zero);
   }
   body.Begin(PrototypeBoardArt.Load("spark"),(RectTransform)anchor.transform,SpecialGemKind.Spark);yield return null;body.enabled=false;Canvas.ForceUpdateCanvases();
   Assert.That(body.Active,Is.False);Assert.That(root.GetComponentInChildren<CreationAccentsGraphic>().canvasRenderer.GetMesh().vertexCount,Is.Zero);
  }
  [UnityTest]public IEnumerator CreationDoesNotEnqueueLegacyHaloAndAccentOpacityIsContinuous()
  {
   var s=Open();yield return null;var impact=s.Board.GetComponentInChildren<PrototypeImpactView>();
   foreach(var kind in new[]{GameplayMotionKind.SpecialGatherStarted,GameplayMotionKind.SpecialCreationStarted,GameplayMotionKind.SpecialConversionStarted})
    impact.SendMessage("Motion",new GameplayMotionEvent(1,kind,new BoardPosition(3,3),new BoardPosition(3,3),1,1,SpecialGemKind.Color,""));
   Assert.That(impact.ActiveCount,Is.Zero,"exact-art creation must not enqueue a second generic charge effect");
   var host=new GameObject("ContinuityClip",typeof(RectTransform),typeof(RectMask2D));host.transform.SetParent(root.transform,false);
   var go=new GameObject("CreationBody",typeof(RectTransform),typeof(SpecialCreationGraphic));go.transform.SetParent(host.transform,false);var body=go.GetComponent<SpecialCreationGraphic>();
   body.Begin(PrototypeBoardArt.Load("color"),(RectTransform)go.transform,SpecialGemKind.Color);yield return new WaitForSecondsRealtime(.10f);Canvas.ForceUpdateCanvases();
   var accents=host.GetComponentInChildren<CreationAccentsGraphic>();var before=accents.canvasRenderer.GetMesh().colors.Max(c=>c.a);
   body.Settle();accents.SetVerticesDirty();Canvas.ForceUpdateCanvases();var after=accents.canvasRenderer.GetMesh().colors.Max(c=>c.a);
   Assert.That(after,Is.EqualTo(before).Within(.01f),"settle must not double accent brightness");body.Finish();
  }
  [UnityTest]public IEnumerator R4DoesNotStackPooledHaloOrPulseOverDedicatedEffects()
  {
   var s=Open();yield return null;var presenter=s.Board.VfxPresenter;presenter.ResetEffects();
   foreach(var kind in new[]{GameplayMotionKind.SpecialCreationStarted,GameplayMotionKind.CascadeStarted,GameplayMotionKind.FallStarted,GameplayMotionKind.SpecialActivationStarted,GameplayMotionKind.BoosterActivated,GameplayMotionKind.BoosterCompleted})
   {
    presenter.Present(new GameplayMotionEvent(1,kind,new BoardPosition(3,3),new BoardPosition(4,3),1,1,SpecialGemKind.Row,"",boosterKind:GameplayBoosterKind.BogatyrMaceRow));
    Assert.That(presenter.LastCueEmissionCount,Is.Zero,kind+" must use dedicated R4 presentation");Assert.That(presenter.ActiveEffectCount,Is.Zero);
   }
   presenter.Present(new GameplayMotionEvent(2,GameplayMotionKind.SwapStarted,new BoardPosition(3,3),new BoardPosition(4,3),0,0,SpecialGemKind.None,""));
   Assert.That(presenter.LastCueEmissionCount,Is.GreaterThan(0),"non-circular swap feedback stays available");presenter.ResetEffects();
  }
  [UnityTest]public IEnumerator R4EnergyGeometryUsesMaskWithoutCollapsingAtBoardEdges()
  {
   var s=Open();yield return null;var impact=s.Board.GetComponentInChildren<PrototypeImpactView>();
   impact.SendMessage("Motion",new GameplayMotionEvent(1,GameplayMotionKind.SpecialActivationStarted,new BoardPosition(0,0),new BoardPosition(0,0),1,1,SpecialGemKind.Color,""));
   yield return new WaitForSecondsRealtime(.24f);Canvas.ForceUpdateCanvases();var mesh=impact.canvasRenderer.GetMesh();var rect=impact.rectTransform.rect;
   Assert.That(impact.GetComponentInParent<RectMask2D>(),Is.Not.Null);Assert.That(mesh.vertices.Any(p=>p.x<rect.xMin||p.y>rect.yMax),Is.True,"off-board geometry keeps its shape and is visually masked, not clamped into border streaks");
  }
  [UnityTest]public IEnumerator RecoveryRemovesAnInProgressAssembly()
  {
   var s=Open();yield return null;s.Board.SwipeCell(3,3,Vector2Int.down);float deadline=Time.realtimeSinceStartup+4;
   while(!s.Board.GetComponentsInChildren<SpecialCreationGraphic>().Any(v=>v.Active)&&Time.realtimeSinceStartup<deadline)yield return null;
   Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().Any(v=>v.Active),Is.True);
   s.Board.SendMessage("ResetVisualTransforms");Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().All(v=>!v.Active),Is.True);
   s.OpenRound(0);yield return null;Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().All(v=>!v.Active),Is.True);
  }
 }
}
