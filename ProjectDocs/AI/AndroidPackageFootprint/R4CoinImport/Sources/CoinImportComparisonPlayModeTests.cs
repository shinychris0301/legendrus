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
    public sealed class CoinImportComparisonPlayModeTests
    {
        [Serializable] public sealed class Row
        {
            public string page, language, asset, hierarchy;
            public int screenWidth, screenHeight, textureWidth, textureHeight, mipCount;
            public bool enabled, active, culled, hasMesh;
            public float alpha;
            public Rect safeArea, rectPixels, meshPixels;
        }
        [Serializable] public sealed class Difference
        {
            public string capture; public int pixels, maximumChannelDifference, p99ChannelDifference;
            public float meanAbsoluteRgbDifference; public bool screeningPassed; public Rect region;
        }
        [Serializable] public sealed class Report
        {
            public List<Row> rows = new List<Row>(); public List<Difference> comparisons = new List<Difference>();
            public int measuredPages;
        }
        readonly string[] names = { "coin_icon" };
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
                Assert.That(assets[i].texture.width,Is.EqualTo(int.Parse(Environment.GetEnvironmentVariable("LOR_COIN_EXPECTED_SIZE"))));
            }
            var report = new Report();
            foreach (string language in new[] { "ru", "en" })
            {
                DemoFeatureState.SetLanguage(language);
                foreach (ScreenId page in new[] { ScreenId.PreLevel, ScreenId.SpecialOffer, ScreenId.DailyReward })
                {
                    GameState.Instance.ResetProgress(); StoryService.ResetRuntime();
                    router.Show(page); Settle(); yield return null; yield return null; Settle();
                    Assert.That(router.CurrentScreen, Is.EqualTo(page));
                    for(int state=0;state<(page==ScreenId.DailyReward?2:1);state++)
                    {
                    if(state==1)
                    {
                        Assert.That(GameState.Instance.ClaimDailyReward(),Is.True);
                        router.Viewport.GetComponentInChildren<ProductionDailyRewardView>().RefreshContent();
                        Settle();yield return null;Settle();
                    }
                    foreach (var size in new[] { new Vector2Int(360,640), new Vector2Int(941,1672),
                        new Vector2Int(1080,2340), new Vector2Int(1080,2400), new Vector2Int(1440,3200) })
                    {
                        Rect safe = size.y == 2400 ? new Rect(45,100,990,2160) : new Rect(0,0,size.x,size.y);
                        Measure(page, language+"_"+state, size, safe, assets, report, output);
                    }
                    report.measuredPages++;
                    }
                }
            }
            AlphaDiagnostics(assets[0],report,output);
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
                Rect coinBounds=new Rect();int visibleCoins=0;
                foreach(var image in router.Viewport.GetComponentsInChildren<Image>(true))
                {
                    int index=Array.IndexOf(assets,image.sprite);if(index<0)continue;
                    Vector3[] corners=new Vector3[4];image.rectTransform.GetWorldCorners(corners);
                    var mesh=image.canvasRenderer.GetMesh();bool hasMesh=mesh!=null&&mesh.vertexCount>0;
                    if(image.enabled&&image.gameObject.activeInHierarchy&&!image.canvasRenderer.cull&&hasMesh)
                    {coinBounds=Bounds(mesh.vertices,image.transform,camera);visibleCoins++;}
                    report.rows.Add(new Row { page=page.ToString(),language=language,asset=names[index],hierarchy=Hierarchy(image.transform),
                        screenWidth=size.x,screenHeight=size.y,safeArea=safeRect,enabled=image.enabled,active=image.gameObject.activeInHierarchy,
                        culled=image.canvasRenderer.cull,hasMesh=hasMesh,alpha=image.canvasRenderer.GetInheritedAlpha(),
                        rectPixels=Bounds(corners,null,camera),meshPixels=hasMesh?Bounds(mesh.vertices,image.transform,camera):new Rect(),
                        textureWidth=image.sprite.texture.width,textureHeight=image.sprite.texture.height,mipCount=image.sprite.texture.mipmapCount });
                }
                Assert.That(visibleCoins,Is.EqualTo(1));
                    RenderTexture.active=target;capture=new Texture2D(size.x,size.y,TextureFormat.RGB24,false);
                    capture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);capture.Apply();
                    string filename=page+"_"+size.x+"x"+size.y+"_"+language+"_ACTUAL_UNITY.png";
                    SaveAndCompare(capture,filename,coinBounds,report,output);
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

        void SaveAndCompare(Texture2D capture,string name,Rect bounds,Report report,string output)
        {
            File.WriteAllBytes(Path.Combine(output,name),capture.EncodeToPNG());
            string baseline=Environment.GetEnvironmentVariable("LOR_COIN_BASELINE_DIR");
            if(string.IsNullOrEmpty(baseline))return;
            var original=new Texture2D(2,2,TextureFormat.RGB24,false);
            try
            {
                Assert.That(original.LoadImage(File.ReadAllBytes(Path.Combine(baseline,name))),Is.True);
                Assert.That(original.width,Is.EqualTo(capture.width));Assert.That(original.height,Is.EqualTo(capture.height));
                var a=original.GetPixels32();var b=capture.GetPixels32();int[] histogram=new int[256];
                int x0=Mathf.Max(0,Mathf.FloorToInt(bounds.xMin)-2),y0=Mathf.Max(0,Mathf.FloorToInt(bounds.yMin)-2);
                int x1=Mathf.Min(capture.width,Mathf.CeilToInt(bounds.xMax)+2),y1=Mathf.Min(capture.height,Mathf.CeilToInt(bounds.yMax)+2);
                long sum=0;int worst=0,count=0;
                for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++)
                {
                    int i=y*capture.width+x,r=Math.Abs(a[i].r-b[i].r),g=Math.Abs(a[i].g-b[i].g),blue=Math.Abs(a[i].b-b[i].b);
                    int m=Math.Max(r,Math.Max(g,blue));worst=Math.Max(worst,m);sum+=r+g+blue;histogram[m]++;count++;
                }
                int threshold=(int)Math.Ceiling(count*.99),total=0,p99=0;
                for(int i=0;i<256;i++){total+=histogram[i];if(total>=threshold){p99=i;break;}}
                float mean=(float)sum/(count*3);
                report.comparisons.Add(new Difference{capture=name,pixels=count,region=new Rect(x0,y0,x1-x0,y1-y0),
                    meanAbsoluteRgbDifference=mean,maximumChannelDifference=worst,p99ChannelDifference=p99,
                    screeningPassed=mean<=3f&&p99<=24});
            }
            finally{UnityEngine.Object.Destroy(original);}
        }

        void AlphaDiagnostics(Sprite sprite,Report report,string output)
        {
            var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();
            var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            bool oldScaler=scaler.enabled,oldAdaptive=adaptive.enabled,oldSafe=safe.enabled,oldViewport=router.Viewport.gameObject.activeSelf;
            var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldScale=canvas.scaleFactor,oldDistance=canvas.planeDistance;
            var oldTarget=RenderTexture.active;var target=new RenderTexture(512,512,24,RenderTextureFormat.ARGB32);target.Create();
            var go=new GameObject("CoinAlphaDiagnosticCamera",typeof(Camera));var camera=go.GetComponent<Camera>();
            // The production Canvas has an opaque backdrop outside Viewport. Cover it
            // explicitly so the diagnostic really samples the requested background.
            var backgroundGo=new GameObject("CoinAlphaBackground",typeof(RectTransform),typeof(Image));backgroundGo.transform.SetParent(canvas.transform,false);
            var background=backgroundGo.GetComponent<Image>();background.raycastTarget=false;
            background.rectTransform.anchorMin=Vector2.zero;background.rectTransform.anchorMax=Vector2.one;
            background.rectTransform.offsetMin=background.rectTransform.offsetMax=Vector2.zero;
            var imageGo=new GameObject("CoinAlphaDiagnostic",typeof(RectTransform),typeof(Image));imageGo.transform.SetParent(canvas.transform,false);
            var image=imageGo.GetComponent<Image>();image.sprite=sprite;image.preserveAspect=true;
            var rect=image.rectTransform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
            try
            {
                router.Viewport.gameObject.SetActive(false);scaler.enabled=false;adaptive.enabled=false;safe.enabled=false;
                camera.enabled=false;camera.transform.position=new Vector3(0,0,-100);camera.clearFlags=CameraClearFlags.SolidColor;
                camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.targetTexture=target;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.scaleFactor=1;
                Color[] colors={Color.black,Color.white,Color.magenta};string[] labels={"black","white","magenta"};
                foreach(int diameter in new[]{27,57,150,230})for(int c=0;c<3;c++)
                {
                    rect.sizeDelta=new Vector2(diameter,diameter);camera.backgroundColor=colors[c];background.color=colors[c];
                    Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
                    var capture=new Texture2D(512,512,TextureFormat.RGB24,false);
                    try{capture.ReadPixels(new Rect(0,0,512,512),0,0);capture.Apply();
                        Color sampled=capture.GetPixel(10,10);
                        Assert.That(sampled.r,Is.EqualTo(colors[c].r).Within(.01));
                        Assert.That(sampled.g,Is.EqualTo(colors[c].g).Within(.01));
                        Assert.That(sampled.b,Is.EqualTo(colors[c].b).Within(.01));
                        SaveAndCompare(capture,"CoinAlpha_"+diameter+"_"+labels[c]+"_ACTUAL_UNITY.png",
                            new Rect(256-diameter*.5f,256-diameter*.5f,diameter,diameter),report,output);}
                    finally{UnityEngine.Object.Destroy(capture);}
                }
            }
            finally
            {
                imageGo.SetActive(false);backgroundGo.SetActive(false);RenderTexture.active=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;
                canvas.planeDistance=oldDistance;canvas.scaleFactor=oldScale;router.Viewport.gameObject.SetActive(oldViewport);
                scaler.enabled=oldScaler;adaptive.enabled=oldAdaptive;safe.enabled=oldSafe;adaptive.ApplyNow();safe.ApplyNow();
                UnityEngine.Object.Destroy(backgroundGo);UnityEngine.Object.Destroy(imageGo);UnityEngine.Object.Destroy(go);target.Release();UnityEngine.Object.Destroy(target);
            }
        }
    }
}
