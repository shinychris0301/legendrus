using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsSequenceArtTests
 {
  GameObject root;readonly List<GameplayMotionEvent> events=new List<GameplayMotionEvent>();
  [UnityTearDown]public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
  MechanicsSandbox Open(){root=new GameObject("SequenceArtReview",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
  static BoardModel Board(){var b=new BoardModel(8,7,6,new SeededBoardRandom(3));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));return b;}
  void Capture(string n)=>MechanicsSandboxPlayModeTests.Capture(root,"SequenceR19_"+n,576,1280);
  void Track(Match3Board b){events.Clear();b.MotionEventPublished+=events.Add;}
  // Test-only buffered recorder: reuse camera/targets, defer PNG encoding until playback ends.
  public sealed class BufferedRecorder : IDisposable
  {
   readonly Canvas canvas;readonly RenderMode mode;readonly Camera oldCamera;readonly float scale;
   readonly GameObject cameraObject;readonly Camera camera;readonly RenderTexture target;readonly Texture2D texture;
   readonly List<byte[]> frames=new List<byte[]>();readonly string directory,key,prefix;
   public BufferedRecorder(GameObject root,string key,string prefix="SequenceR19")
   {
    this.key=key;this.prefix=prefix;directory=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(string.IsNullOrEmpty(directory))return;
    canvas=root.GetComponent<Canvas>();mode=canvas.renderMode;oldCamera=canvas.worldCamera;scale=canvas.scaleFactor;
    cameraObject=new GameObject("BufferedArtCapture",typeof(Camera));camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.transform.position=new Vector3(0,0,-100);camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
    target=new RenderTexture(576,1280,24);target.Create();texture=new Texture2D(576,1280,TextureFormat.RGB24,false);camera.targetTexture=target;
    canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.scaleFactor=576f/941;
    foreach(var text in root.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
   }
   public void Sample()
   {
    if(!texture)return;var previous=RenderTexture.active;
    try{Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,576,1280),0,0);frames.Add(texture.GetRawTextureData<byte>().ToArray());}
    finally{RenderTexture.active=previous;}
   }
   public void Dispose()
   {
    if(!texture)return;
    try{for(int i=0;i<frames.Count;i++){texture.LoadRawTextureData(frames[i]);File.WriteAllBytes(Path.Combine(directory,"Mechanics_"+prefix+"_"+key+"_"+i.ToString("D2")+".png"),texture.EncodeToPNG());}}
    finally{canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.scaleFactor=scale;UnityEngine.Object.Destroy(cameraObject);UnityEngine.Object.Destroy(texture);target.Release();UnityEngine.Object.Destroy(target);frames.Clear();}
   }
  }
  IEnumerator Record(MechanicsSandbox s,string key,int expectedMoves=49)
  {
   string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR"),path=string.IsNullOrEmpty(dir)?null:Path.Combine(dir,"SequenceR19_"+key+"_Frames.tsv");if(path!=null)File.WriteAllText(path,string.Empty);
   float start=Time.unscaledTime,next=start,deadline=Time.realtimeSinceStartup+35;int frame=0,peak=0;bool bothLayers=false;
   using(var recorder=new BufferedRecorder(root,key))
   {
   while((s.Board.HasActiveCommand||Time.unscaledTime-start<1.0f)&&Time.realtimeSinceStartup<deadline)
   {
    var layers=s.Board.GetComponentsInChildren<SpecialSequenceView>();bothLayers|=layers.Length==2;
    foreach(var layer in layers){var mesh=layer.canvasRenderer.GetMesh();if(mesh!=null)peak=Math.Max(peak,mesh.vertexCount);}
    foreach(var layer in s.Board.GetComponentsInChildren<SpecialCreationGraphic>()){var mesh=layer.canvasRenderer.GetMesh();if(mesh!=null)peak=Math.Max(peak,mesh.vertexCount);}
    if(Time.unscaledTime>=next&&frame<100){recorder.Sample();if(path!=null)File.AppendAllText(path,frame+"\t"+(Time.unscaledTime-start).ToString("F4",CultureInfo.InvariantCulture)+"\n");frame++;next=Time.unscaledTime+.025f;}
    yield return null;
   }
   }
   Assert.That(bothLayers,Is.True);Assert.That(peak,Is.GreaterThan(0).And.LessThan(20000));Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.CanAcceptPointerInput,Is.True);
   Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.CommandRecovered),Is.False);Assert.That(s.Board.Moves,Is.EqualTo(expectedMoves));
   yield return new WaitForSecondsRealtime(.75f);foreach(var v in s.Board.GetComponentsInChildren<SpecialSequenceView>())Assert.That(v.ActiveCount,Is.Zero);Capture(key+"_After");
  }
  [UnityTest]public IEnumerator DirectionalFormationUsesArtworkAndKeepsResolution()
  {var s=Open();yield return null;Track(s.Board);Capture("LineFormation_Before");Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);yield return Record(s,"LineFormation",3);Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.SpecialCreationStarted&&(e.SpecialKind==SpecialGemKind.Row||e.SpecialKind==SpecialGemKind.Column)),Is.True);}
  [UnityTest]public IEnumerator BombFormationUsesMaterialLayerAndKeepsResolution()
  {
   var s=Open();yield return null;var b=Board();foreach(var p in new[]{new BoardPosition(2,3),new BoardPosition(4,3),new BoardPosition(3,2),new BoardPosition(3,1),new BoardPosition(3,4)})b.SetCell(p,new BoardCellData(5));b.SetCell(new BoardPosition(3,3),new BoardCellData(0));Assert.That(s.Board.LoadSnapshotForTesting(b.CreateSnapshot()),Is.True);Track(s.Board);Capture("BombFormation_Before");Assert.That(s.Board.SwipeCell(3,4,Vector2Int.down),Is.True);yield return Record(s,"BombFormation");Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.SpecialCreationStarted&&e.SpecialKind==SpecialGemKind.Burst),Is.True);Assert.That(s.Board.GetComponentsInChildren<SpecialCreationGraphic>().Any(v=>v.Artwork==PrototypeBoardArt.Load("bomb")),Is.True);
  }
  [UnityTest]public IEnumerator CombinedAttackAndChainUseRealContactsAndReleaseInput()
  {
   var s=Open();yield return null;var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,SpecialGemKind.Row));b.SetCell(new BoardPosition(4,3),new BoardCellData(5,SpecialGemKind.Burst));b.SetCell(new BoardPosition(7,3),new BoardCellData(5,SpecialGemKind.Burst));b.SetObstacle(new BoardPosition(7,4),new BoardObstacleData(BoardObstacleKind.Crate,2));s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Track(s.Board);Capture("CombinedChain_Before");Assert.That(s.Board.SwipeCell(3,3,Vector2Int.right),Is.True);yield return Record(s,"CombinedChain");Assert.That(events.Any(e=>e.CombinationKind==SpecialCombinationKind.RowWithBurst),Is.True);Assert.That(events.Count(e=>e.Kind==GameplayMotionKind.CellsImpacted),Is.GreaterThan(1));Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.SpecialActivationStarted&&e.SpecialKind==SpecialGemKind.Burst),Is.True);
  }
  [UnityTest]public IEnumerator IndependentLaunchesAndBombUseContactsAndKeepOutcomes()
  {
   var s=Open();yield return null;
   foreach(var kind in new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst})
   {
    s.Reset(0,GameplayRuleVersion.PrototypeR4);yield return null;var b=Board();
    b.SetCell(new BoardPosition(3,3),new BoardCellData(5,kind));b.SetObstacle(new BoardPosition(5,3),new BoardObstacleData(BoardObstacleKind.Crate,2));
    s.Board.LoadSnapshotForTesting(b.CreateSnapshot());Track(s.Board);string key=kind==SpecialGemKind.Row?"RowLaunch":kind==SpecialGemKind.Column?"ColumnLaunch":"BombBlast";
    Capture(key+"_Before");Assert.That(s.Board.TapCell(3,3),Is.True);yield return Record(s,key);
    Assert.That(events.Any(e=>e.Kind==GameplayMotionKind.CellsImpacted),Is.True);
    var layers=s.Board.GetComponentsInChildren<DirectionalLaunchView>();Assert.That(layers.Length,Is.EqualTo(2));
    foreach(var layer in layers){Assert.That(layer.ActiveCount,Is.Zero);Assert.That(layer.raycastTarget,Is.False);Assert.That(layer.GetComponentInParent<RectMask2D>(),Is.Not.Null);}
   }
  }
  [UnityTest]public IEnumerator DirectionalLayersTrackContactClockAndCleanUpUnderLoad()
  {
   var s=Open();yield return null;
   for(int d=0;d<=8;d++)Assert.That(DirectionalLaunchView.DistanceInCells(BoardPresentationTimeline.Charge+BoardPresentationTimeline.Sweep*d/8,8),Is.EqualTo(d).Within(.0001f));
   var cue=new GameplayMotionEvent(1,GameplayMotionKind.SpecialActivationStarted,new BoardPosition(0,0),new BoardPosition(0,0),1,1,SpecialGemKind.Column,"");
   var recover=new GameplayMotionEvent(2,GameplayMotionKind.CommandRecovered,new BoardPosition(0,0),new BoardPosition(0,0),0,0,SpecialGemKind.None,"");
   foreach(var layer in s.Board.GetComponentsInChildren<DirectionalLaunchView>())
   {
    for(int i=0;i<80;i++)layer.SendMessage("Motion",cue);Assert.That(layer.ActiveCount,Is.EqualTo(32));
    yield return new WaitForSecondsRealtime(.16f);Canvas.ForceUpdateCanvases();var mesh=layer.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0).And.LessThanOrEqualTo(2048));Assert.That(mesh.vertices.All(p=>!float.IsNaN(p.x)&&!float.IsInfinity(p.y)),Is.True);
    layer.SendMessage("Motion",recover);Assert.That(layer.ActiveCount,Is.Zero);layer.enabled=false;layer.SendMessage("Motion",cue);Assert.That(layer.ActiveCount,Is.Zero);layer.enabled=true;
   }
  }
  [UnityTest]public IEnumerator ArtLayersAreBoundedClippedAndClearOnRecoveryAndDisable()
  {
   var s=Open();yield return null;Assert.That(MechanicsSequenceArt.Available,Is.True);Assert.That(MechanicsSequenceArt.Effects.width,Is.EqualTo(1254));Assert.That(MechanicsSequenceArt.Materials.width,Is.EqualTo(1254));
   var cue=new GameplayMotionEvent(1,GameplayMotionKind.SpecialActivationStarted,new BoardPosition(3,3),new BoardPosition(3,3),1,1,SpecialGemKind.Burst,"");
   var recover=new GameplayMotionEvent(2,GameplayMotionKind.CommandRecovered,new BoardPosition(0,0),new BoardPosition(0,0),0,0,SpecialGemKind.None,"");
   foreach(var v in s.Board.GetComponentsInChildren<SpecialSequenceView>())
   {
    Assert.That(v.GetComponentInParent<RectMask2D>(),Is.Not.Null);Assert.That(v.raycastTarget,Is.False);
    for(int i=0;i<180;i++)v.SendMessage("Motion",cue);Assert.That(v.ActiveCount,Is.EqualTo(SpecialSequenceView.Capacity));Assert.That(v.DroppedCount,Is.EqualTo(52));
    yield return new WaitForSecondsRealtime(.16f);Canvas.ForceUpdateCanvases();var mesh=v.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0).And.LessThan(20000));Assert.That(mesh.vertices.All(p=>!float.IsNaN(p.x)&&!float.IsInfinity(p.y)),Is.True);
    v.SendMessage("Motion",recover);Assert.That(v.ActiveCount,Is.Zero);v.enabled=false;v.SendMessage("Motion",cue);Assert.That(v.ActiveCount,Is.Zero);v.enabled=true;
   }
   s.Reset(0,GameplayRuleVersion.PrototypeR3);yield return null;Assert.That(s.Board.GetComponentsInChildren<SpecialSequenceView>(),Is.Empty);foreach(var v in s.Board.GetComponentsInChildren<GemShatterView>())Assert.That(v.UsesArtwork,Is.False);
  }
 }
}
