using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
 public class MechanicsEnergyTests
 {
  GameObject root;
  [UnityTearDown]public IEnumerator Cleanup(){if(root)Object.Destroy(root);yield return null;}
  [UnityTest]public IEnumerator EnergyShaderPreservesGoldChannelsAndRectClipping()
  {
   var shader=Resources.Load<Shader>("ProductionUI/MechanicsEffectsR16/Energy");Assert.That(shader,Is.Not.Null);Assert.That(shader.isSupported,Is.True);
   root=new GameObject("EnergyPixelCheck",typeof(RectTransform),typeof(Canvas));var canvas=root.GetComponent<Canvas>();
   var camGo=new GameObject("EnergyCamera",typeof(Camera));camGo.transform.SetParent(root.transform,false);var camera=camGo.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.transform.position=new Vector3(0,0,-100);camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
   var clip=new GameObject("Clip",typeof(RectTransform),typeof(RectMask2D));clip.transform.SetParent(root.transform,false);var clipRect=(RectTransform)clip.transform;clipRect.sizeDelta=new Vector2(32,32);
   var go=new GameObject("GoldLight",typeof(RectTransform),typeof(Image));go.transform.SetParent(clip.transform,false);((RectTransform)go.transform).sizeDelta=new Vector2(64,64);var image=go.GetComponent<Image>();var material=new Material(shader);image.material=material;image.color=new Color(1,.35f,.05f,1);image.raycastTarget=false;
   var target=new RenderTexture(64,64,24);var pixels=new Texture2D(64,64,TextureFormat.RGBA32,false);var old=RenderTexture.active;
   try
   {
    target.Create();camera.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.scaleFactor=1;
    yield return null;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,64,64),0,0);pixels.Apply();
    var center=pixels.GetPixel(32,32);Assert.That(center.r,Is.GreaterThan(.6f),"red channel must contribute to antique-gold energy");Assert.That(center.r,Is.GreaterThan(center.g));Assert.That(center.g,Is.GreaterThan(center.b));Assert.That(center.a,Is.GreaterThan(.95f),"additive layer preserves destination alpha");
    var outside=pixels.GetPixel(8,32);Assert.That(outside.maxColorComponent,Is.LessThan(.02f),"RectMask2D must clip the energy outside the board");
   }
   finally{RenderTexture.active=old;Object.Destroy(material);Object.Destroy(pixels);target.Release();Object.Destroy(target);}
  }
  [UnityTest]public IEnumerator R4OwnsEnergyMaterialAndRetryReleasesIt()
  {
   root=new GameObject("EnergyLifecycle",typeof(RectTransform),typeof(Canvas));var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);yield return null;
   var view=s.Board.GetComponentInChildren<PrototypeImpactView>();Assert.That(view.UsesEnergyMaterial,Is.True);var material=view.material;Assert.That(material.shader.name,Is.EqualTo("LegendsOfRus/UI/BoardEnergy"));
   s.Reset(0,GameplayRuleVersion.PrototypeR3);yield return null;Assert.That(material==null,Is.True,"owned runtime material is destroyed with previous board");Assert.That(s.Board.GetComponentInChildren<PrototypeImpactView>().UsesEnergyMaterial,Is.False);
  }
 }
}
