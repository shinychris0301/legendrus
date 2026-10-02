using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class MechanicsSandboxPlayModeTests
    {
        GameObject root;
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
        MechanicsSandbox Open(){root=new GameObject("MechanicsCapture",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);}
        [UnityTest] public IEnumerator PrototypeTapRecordsReplayAndCapturesImpact()
        {
            var s=Open();yield return null;s.Reset(1,GameplayRuleVersion.Legacy);yield return null;
            int moves=s.Board.Moves;Assert.That(s.Board.TapCell(3,3),Is.True);yield return null;Assert.That(s.Board.Moves,Is.EqualTo(moves));Capture(root,"Legacy",576,1280);
            s.Reset(1,GameplayRuleVersion.PrototypeR1);yield return null;Capture(root,"Before",576,1280);Assert.That(s.Board.TapCell(3,3),Is.True);
            bool effects=false;for(int i=0;i<24;i++){yield return new WaitForSecondsRealtime(.04f);effects|=s.Board.GetComponentInChildren<PrototypeImpactView>().ActiveCount>0;Capture(root,"Bomb_"+i.ToString("D2"),576,1280);}
            float end=Time.realtimeSinceStartup+15;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;
            Assert.That(s.Board.HasActiveCommand,Is.False);Assert.That(s.Board.Moves,Is.EqualTo(49));Assert.That(effects,Is.True);Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
            Capture(root,"After",576,1280);
        }
        [UnityTest] public IEnumerator FixturesAndAllInventoryBoostersRemainPlayable()
        {
            var s=Open();yield return null;
            for(int i=0;i<9;i++){
                s.Reset(i,GameplayRuleVersion.PrototypeR1);yield return null;
                bool accepted=i==3||i==4||i==5?s.Board.SwipeCell(3,3,Vector2Int.right):i==8?s.Board.SwipeCell(3,3,Vector2Int.down):s.Board.TapCell(3,3);
                Assert.That(accepted,Is.True,"fixture "+i);float end=Time.realtimeSinceStartup+15;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;
                Assert.That(s.Board.Moves,Is.EqualTo(49),"fixture "+i);Assert.That(s.Board.CanAcceptPointerInput,Is.True);Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
            }
            foreach(BoosterType t in Enum.GetValues(typeof(BoosterType))){s.Reset(0,GameplayRuleVersion.PrototypeR1);yield return null;s.Board.UseBooster(t);if(t!=BoosterType.SeaPearl)Assert.That(s.Board.TapCell(3,3),Is.True);float end=Time.realtimeSinceStartup+15;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(s.Board.Moves,Is.EqualTo(t==BoosterType.SeaPearl?55:50));}
        }
        public static void Capture(GameObject root,string name,int width,int height)
        {
            string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(string.IsNullOrEmpty(dir))return;Directory.CreateDirectory(dir);
            var canvas=root.GetComponent<Canvas>();var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldScale=canvas.scaleFactor;
            var camGo=new GameObject("CampaignCaptureCamera",typeof(Camera));var cam=camGo.GetComponent<Camera>();cam.enabled=false;cam.orthographic=true;cam.transform.position=new Vector3(0,0,-100);cam.nearClipPlane=.1f;cam.farClipPlane=1000;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.black;
            var target=new RenderTexture(width,height,24);var texture=new Texture2D(width,height,TextureFormat.RGBA32,false);var active=RenderTexture.active;
            try{target.Create();cam.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=100;canvas.scaleFactor=width/941f;
                foreach(var t in root.GetComponentsInChildren<Text>(true))t.SetVerticesDirty();Canvas.ForceUpdateCanvases();foreach(var t in root.GetComponentsInChildren<Text>(true))t.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                var impact=root.GetComponentInChildren<PrototypeImpactView>();
                if(impact){Assert.That(impact.GetComponent<CanvasRenderer>(),Is.Not.Null);var mesh=impact.canvasRenderer.GetMesh();if(impact.ActiveCount>0)Assert.That(mesh!=null&&mesh.vertexCount>0,Is.True,"Active impacts must produce a rendered mesh");File.AppendAllText(Path.Combine(dir,"ImpactDiagnostics.txt"),$"{name}: active={impact.ActiveCount}, vertices={mesh?.vertexCount}, cull={impact.canvasRenderer.cull}, alpha={impact.canvasRenderer.GetInheritedAlpha()}, rect={impact.rectTransform.rect}, sibling={impact.transform.GetSiblingIndex()}/{impact.transform.parent.childCount}, materials={impact.canvasRenderer.materialCount}\n");}
                cam.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Path.Combine(dir,"Mechanics_"+name+".png"),texture.EncodeToPNG());
            }finally{RenderTexture.active=active;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.scaleFactor=oldScale;UnityEngine.Object.Destroy(camGo);UnityEngine.Object.Destroy(texture);target.Release();UnityEngine.Object.Destroy(target);}
        }
    }
}
