using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    // Temporary measurement fixture; copied into the existing test assembly only for this audit.
    public sealed class SharedTextureAuditPlayModeTests
    {
        [Serializable] public sealed class Row
        {
            public string page, language, asset, hierarchy;
            public int screenWidth, screenHeight, textureWidth, textureHeight, mipCount;
            public bool enabled, active, culled, hasMesh;
            public float alpha;
            public Rect safeArea, rectPixels, meshPixels;
        }
        [Serializable] public sealed class Report { public List<Row> rows = new List<Row>(); public int measuredPages; }
        readonly string[] names = { "coin_icon", "progress_fill", "progress_frame" };
        ScreenRouter router;

        [UnityTest]
        public IEnumerator MeasureExistingPagesWithoutChangingImports()
        {
            string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config, Is.Not.Null.And.Not.Empty);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(config));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(config + Path.DirectorySeparatorChar), Is.True);
            string output = Environment.GetEnvironmentVariable("LOR_SHARED_TEXTURE_AUDIT_DIR");
            Assert.That(output, Is.Not.Null.And.Not.Empty);
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            GameState.Instance.ResetProgress(); StoryService.ResetRuntime();
            foreach (var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach (var step in level.tutorialSteps) GameState.Instance.CompleteTutorial(step.id);
            var assets = new Sprite[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                assets[i] = Resources.Load<Sprite>("ProductionUI/HomeAligned/" + names[i]);
                Assert.That(assets[i], Is.Not.Null);
            }
            var report = new Report();
            foreach (string language in new[] { "ru", "en" })
            {
                DemoFeatureState.SetLanguage(language);
                foreach (ScreenId page in new[] { ScreenId.Home, ScreenId.PreLevel, ScreenId.Gameplay,
                    ScreenId.Victory, ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.DailyReward,
                    ScreenId.Fail, ScreenId.ChapterMap, ScreenId.WorldMap })
                {
                    router.Show(page); Settle(); yield return null; yield return null; Settle();
                    Assert.That(router.CurrentScreen, Is.EqualTo(page));
                    foreach (var size in new[] { new Vector2Int(360,640), new Vector2Int(941,1672),
                        new Vector2Int(1080,2340), new Vector2Int(1080,2400), new Vector2Int(1440,3200) })
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
                foreach(var image in router.Viewport.GetComponentsInChildren<Image>(true))
                {
                    int index=Array.IndexOf(assets,image.sprite);if(index<0)continue;
                    Vector3[] corners=new Vector3[4];image.rectTransform.GetWorldCorners(corners);
                    var mesh=image.canvasRenderer.GetMesh();bool hasMesh=mesh!=null&&mesh.vertexCount>0;
                    report.rows.Add(new Row { page=page.ToString(),language=language,asset=names[index],hierarchy=Hierarchy(image.transform),
                        screenWidth=size.x,screenHeight=size.y,safeArea=safeRect,enabled=image.enabled,active=image.gameObject.activeInHierarchy,
                        culled=image.canvasRenderer.cull,hasMesh=hasMesh,alpha=image.canvasRenderer.GetInheritedAlpha(),
                        rectPixels=Bounds(corners,null,camera),meshPixels=hasMesh?Bounds(mesh.vertices,image.transform,camera):new Rect(),
                        textureWidth=image.sprite.texture.width,textureHeight=image.sprite.texture.height,mipCount=image.sprite.texture.mipmapCount });
                }
                if(size.x==941 && language=="ru")
                {
                    RenderTexture.active=target;capture=new Texture2D(size.x,size.y,TextureFormat.RGB24,false);
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
