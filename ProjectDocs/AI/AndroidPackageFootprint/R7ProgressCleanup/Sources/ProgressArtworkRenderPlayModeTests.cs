using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

namespace LegendsOfRus.Tests.PlayMode
{
    // Temporary measurement fixture; copied into the existing test assembly only for this audit.
    public sealed class ProgressArtworkRenderPlayModeTests
    {
        [Serializable] public sealed class Row { public string page,language;public int width,height,retiredImages,changedPixels; }
        [Serializable] public sealed class Report { public List<Row> rows=new List<Row>();public int measuredPages; }
        readonly string[] names = { "progress_fill", "progress_frame" };
        ScreenRouter router;

        [UnityTest]
        public IEnumerator RetiredSpritesDoNotAffectNativePagePixels()
        {
            string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config, Is.Not.Null.And.Not.Empty);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(config));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(config + Path.DirectorySeparatorChar), Is.True);
            string output = Environment.GetEnvironmentVariable("LOR_PROGRESS_CAPTURE_DIR");
            Assert.That(output, Is.Not.Null.And.Not.Empty);
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            GameState.Instance.ResetProgress(); StoryService.ResetRuntime();
            foreach (var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach (var step in level.tutorialSteps) GameState.Instance.CompleteTutorial(step.id);
            var assets = new Sprite[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                assets[i] = Resources.Load<Sprite>("ProductionUI/HomeAligned/" + names[i]) ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/LegendsOfRus/Art/ReferenceOnly/RetiredProgress/"+names[i]+".png");
                Assert.That(assets[i], Is.Not.Null);
            }
            var report = new Report();
            foreach (string language in new[] { "ru", "en" })
            {
                DemoFeatureState.SetLanguage(language);
                foreach (ScreenId page in new[] { ScreenId.Home, ScreenId.PreLevel, ScreenId.Gameplay,
                    ScreenId.Fail, ScreenId.ChapterMap, ScreenId.WorldMap })
                {
                    router.Show(page); Settle(); yield return null; yield return null; Settle();
                    Assert.That(router.CurrentScreen, Is.EqualTo(page));
                    foreach (var size in new[] { new Vector2Int(360,640), new Vector2Int(941,1672),
                        new Vector2Int(1080,2400) })
                    {
                        Rect safe = size.y == 2400 ? new Rect(45,100,990,2160) : new Rect(0,0,size.x,size.y);
                        Measure(page, language, size, safe, assets, report, output);
                    }
                    report.measuredPages++;
                }
            }
            Assert.That(report.rows.Count, Is.GreaterThan(0));
            File.WriteAllText(Path.Combine(output,"RuntimeMeasurements.json"), JsonUtility.ToJson(report, true));
            router.Show(ScreenId.SplashTitle); Settle();
        }

        void Settle()
        {
            for(int i=0;i<24;i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            Canvas.ForceUpdateCanvases();
        }
        static string Hierarchy(Transform t)
        {
            string name=t.name;while(t.parent!=null){t=t.parent;name=t.name+"/"+name;}return name;
        }
        static Rect Bounds(Vector3[] points, Transform transform, Camera camera)
        {
            Vector2 lo=new Vector2(float.MaxValue,float.MaxValue),hi=new Vector2(float.MinValue,float.MinValue);
            foreach(var p in points)
            {
                Vector2 v=camera.WorldToScreenPoint(transform==null?p:transform.TransformPoint(p));
                lo=Vector2.Min(lo,v);hi=Vector2.Max(hi,v);
            }
            return Rect.MinMaxRect(lo.x,lo.y,hi.x,hi.y);
        }
        void Measure(ScreenId page,string language,Vector2Int size,Rect safeRect,Sprite[] assets,Report report,string output)
        {
            var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();
            var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldDistance=canvas.planeDistance,oldScale=canvas.scaleFactor;
            bool oldScaler=scaler.enabled,oldAdaptive=adaptive.enabled,oldSafe=safe.enabled;
            var oldTarget=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);target.Create();
            var go=new GameObject("SharedTextureAuditCamera",typeof(Camera));var camera=go.GetComponent<Camera>();
            Texture2D capture=null;
            try
            {
                scaler.enabled=false;adaptive.enabled=false;safe.enabled=false;
                camera.enabled=false;camera.transform.position=new Vector3(0,0,-100);
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(4,13,24,255);
                camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.targetTexture=target;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;
                adaptive.Apply(safeRect,size);
                canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int((int)safeRect.width,(int)safeRect.height),941,1672);
                safe.Apply(safeRect,size);
                foreach(var text in router.Viewport.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();camera.Render();
                var retired=router.Viewport.GetComponentsInChildren<Image>(true).Where(i=>
                    i.name=="ProfileProgressFrame"||i.name=="QuestProgressFrame"||i.name=="MeasuredFill"||
                    i.name=="GoalProgress"||i.name=="StarProgress"||i.name=="GoalProgressFrame"||i.name=="StarProgressFrame").ToArray();
                foreach(var i in retired) { Assert.That(i.sprite,Is.Null);Assert.That(i.enabled&&i.gameObject.activeInHierarchy,Is.False); }
                RenderTexture.active=target;capture=new Texture2D(size.x,size.y,TextureFormat.RGB24,false);
                capture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);capture.Apply();var current=capture.GetPixels32();
                int changed=0;
                try
                {
                    foreach(var i in retired)i.sprite=assets[i.name.Contains("Frame")?1:0];
                    Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
                    capture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);capture.Apply();var original=capture.GetPixels32();
                    for(int i=0;i<current.Length;i++)if(!current[i].Equals(original[i]))changed++;
                }
                finally { foreach(var i in retired)i.sprite=null; }
                report.rows.Add(new Row{page=page.ToString(),language=language,width=size.x,height=size.y,retiredImages=retired.Length,changedPixels=changed});
                Assert.That(changed,Is.Zero,page+" "+language+" "+size);
                Canvas.ForceUpdateCanvases();camera.Render();
                if(size.x==941 && language=="ru")
                {
                    RenderTexture.active=target;
                    capture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);capture.Apply();
                    File.WriteAllBytes(Path.Combine(output,page+"_941x1672_RU_ACTUAL_UNITY.png"),capture.EncodeToPNG());
                }
            }
            finally
            {
                RenderTexture.active=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;
                canvas.planeDistance=oldDistance;canvas.scaleFactor=oldScale;
                scaler.enabled=oldScaler;adaptive.enabled=oldAdaptive;safe.enabled=oldSafe;
                adaptive.ApplyNow();safe.ApplyNow();
                if(capture!=null)UnityEngine.Object.Destroy(capture);
                UnityEngine.Object.Destroy(go);target.Release();UnityEngine.Object.Destroy(target);
            }
        }
    }
}
