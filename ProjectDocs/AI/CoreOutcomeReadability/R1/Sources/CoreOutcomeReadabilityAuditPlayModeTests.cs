using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class CoreOutcomeReadabilityAuditPlayModeTests
    {
        object owner;
        readonly List<Observation> measurements=new List<Observation>();
        [Serializable] public sealed class TextRow {public string path,text;public float fontPixels,preferredWidth,preferredHeight,width,height;public int visibleCharacters,fontSize;public bool widthOverflow,heightOverflow;public Rect glyphBounds,labelBounds;}
        [Serializable] public sealed class ButtonRow {public string path;public Rect bounds,hitBounds;public bool enabled,interactable,centerReachable;}
        [Serializable] public sealed class Observation {public string page,state,language;public int width,height;public Rect safe;public TextRow[] texts;public ButtonRow[] buttons;}
        [Serializable] public sealed class Record {public Observation[] observations;}
        static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
        IEnumerator Begin(object fixture){measurements.Clear();owner=fixture;yield return (IEnumerator)owner.GetType().GetMethod("SetUp").Invoke(owner,null);}
        [UnityTearDown] public IEnumerator End()
        {
            if(owner!=null){yield return (IEnumerator)owner.GetType().GetMethod("TearDown").Invoke(owner,null);if(owner is ChapterCompleteAlignmentPlayModeTests chapter)chapter.RestoreActiveContent();owner=null;}
            var dir=Environment.GetEnvironmentVariable("LOR_CORE_OUTCOME_READABILITY_DIR");if(!string.IsNullOrEmpty(dir))File.WriteAllText(Path.Combine(dir,GetType().Name+"_"+Guid.NewGuid().ToString("N")+".json"),JsonUtility.ToJson(new Record{observations=measurements.ToArray()},true));
        }
        [UnityTest] public IEnumerator GameplayNativeValuesAndTargets()
        {
            yield return Begin(new GameplayAlignmentPlayModeTests());var router=ScreenRouter.Instance;GameState.Instance.Save.highestUnlockedLevel=500;GameState.Instance.SelectLevel(1,8);router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
            var v=router.Viewport.GetComponentInChildren<ProductionGameplayView>();Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);yield return null;yield return null;string save=StableSave();string board=v.Board.BoardStateHash;
                CaptureAll(v.gameObject,"Gameplay","Normal");
                v.MovesValue.text="999";v.ScoreValue.text="2 147 483 647";v.LevelText.text=lang=="ru"?"УРОВЕНЬ 500":"LEVEL 500";foreach(var g in v.Goals)g.Count.text="12 345";foreach(var count in v.BoosterCounts)count.text="9 999";
                CaptureAll(v.gameObject,"Gameplay","LargeValues");v.RefreshContent();v.BoosterButtons[2].onClick.Invoke();yield return null;CaptureAll(v.gameObject,"Gameplay","Targeting");v.Board.CancelArmedBooster(showToast:false);
                Assert.That(StableSave(),Is.EqualTo(save));Assert.That(v.Board.BoardStateHash,Is.EqualTo(board));
            }
        }
        [UnityTest] public IEnumerator VictoryCurrentFirstClearReplayAndUnavailableStates()
        {
            yield return Begin(new VictoryAlignmentPlayModeTests());var router=ScreenRouter.Instance;var state=GameState.Instance;state.Save.highestUnlockedLevel=500;
            var level=ChapterLevelCatalog.Find(1,500);state.CompleteLevel(level,int.MaxValue);router.Show(ScreenId.Victory);Settle();yield return new WaitForSecondsRealtime(1);
            var v=router.Viewport.GetComponentInChildren<ProductionVictoryView>();Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var lang in new[]{"ru","en"}){DemoFeatureState.SetLanguage(lang);v.RefreshContent();yield return null;yield return null;string save=StableSave();CaptureAll(v.gameObject,"Victory","FinaleFirstClear");Assert.That(StableSave(),Is.EqualTo(save));}
            state.CompleteLevel(level,3500);router.Show(ScreenId.Victory);Settle();yield return new WaitForSecondsRealtime(1);v=router.Viewport.GetComponentInChildren<ProductionVictoryView>();
            foreach(var lang in new[]{"ru","en"}){DemoFeatureState.SetLanguage(lang);v.RefreshContent();yield return null;yield return null;string save=StableSave();CaptureAll(v.gameObject,"Victory","FinaleReplay");v.SetStatus(lang=="ru"?"Видео с наградой недоступно":"Rewarded video is unavailable");CaptureAll(v.gameObject,"Victory","Unavailable");Assert.That(StableSave(),Is.EqualTo(save));}
        }
        [UnityTest] public IEnumerator ChapterCompleteInstalledFiveHundredRoundSummary()
        {
            var fixture=new ChapterCompleteAlignmentPlayModeTests();yield return Begin(fixture);
            var field=fixture.GetType().GetField("legacyFixture",Flags);((IDisposable)field.GetValue(fixture)).Dispose();field.SetValue(fixture,null);
            var state=GameState.Instance;state.Save.highestUnlockedLevel=500;state.Save.levelProgress.Clear();foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)state.Save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=level.levelId,completions=1,bestStars=3,rewardClaimed=true});var story=StoryCatalog.LoadChapter1();state.CompleteStoryScene(story.introSceneId);foreach(var m in story.milestones)state.CompleteStoryScene(m.sceneId);StoryService.ResetRuntime();ScreenRouter.Instance.Show(ScreenId.ChapterComplete);Settle();yield return null;
            var v=ScreenRouter.Instance.Viewport.GetComponentInChildren<ProductionChapterCompleteView>();Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var lang in new[]{"ru","en"}){DemoFeatureState.SetLanguage(lang);v.RefreshContent();yield return null;yield return null;string save=StableSave();CaptureAll(v.gameObject,"ChapterComplete","Finale");Assert.That(StableSave(),Is.EqualTo(save));}
        }
        static string StableSave(){var save=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));save.lastEnergyUtcTicks=0;return JsonUtility.ToJson(save);}
        static void Settle(){for(int i=0;i<24;i++)ScreenRouter.Instance.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        void CaptureAll(GameObject root,string page,string state)
        {
            foreach(var pair in new[]{(new Vector2Int(360,640),new Rect(0,20,360,600)),(new Vector2Int(576,1280),new Rect(0,48,576,1172)),(new Vector2Int(941,1672),new Rect(0,0,941,1672)),(new Vector2Int(1080,2400),new Rect(45,100,990,2160))})Capture(root,page,state,pair.Item1,pair.Item2);
        }
        static Rect Bounds(RectTransform r,Camera c){var corners=new Vector3[4];r.GetWorldCorners(corners);var a=RectTransformUtility.WorldToScreenPoint(c,corners[0]);var b=RectTransformUtility.WorldToScreenPoint(c,corners[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
        static string PathOf(Transform t,Transform root)=>t==root?t.name:PathOf(t.parent,root)+"/"+t.name;
        void Capture(GameObject root,string page,string state,Vector2Int size,Rect safeArea)
        {
            var router=ScreenRouter.Instance;var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;float distance=canvas.planeDistance,scale=canvas.scaleFactor;bool se=scaler.enabled,ae=adaptive.enabled,fe=safe.enabled;var active=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);var cameraGo=new GameObject("CoreOutcomeAuditCamera",typeof(Camera));var texture=new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try
            {
                target.Create();scaler.enabled=adaptive.enabled=safe.enabled=false;var camera=cameraGo.GetComponent<Camera>();camera.enabled=false;camera.transform.position=new Vector3(0,0,-100);camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(4,13,24,255);camera.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;adaptive.Apply(safeArea,size);canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int((int)safeArea.width,(int)safeArea.height),941,1672);safe.Apply(safeArea,size);Canvas.ForceUpdateCanvases();foreach(var text in root.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                var texts=new List<TextRow>();foreach(var t in root.GetComponentsInChildren<Text>())
                {
                    if(!t.enabled||string.IsNullOrEmpty(t.text))continue;Rect label=Bounds(t.rectTransform,camera);int font=t.resizeTextForBestFit?t.cachedTextGenerator.fontSizeUsedForBestFit:t.fontSize;var glyph=label;var mesh=t.canvasRenderer.GetMesh();if(mesh!=null&&mesh.vertexCount>0){var points=mesh.vertices.Select(x=>RectTransformUtility.WorldToScreenPoint(camera,t.rectTransform.TransformPoint(x))).ToArray();glyph=Rect.MinMaxRect(points.Min(x=>x.x),points.Min(x=>x.y),points.Max(x=>x.x),points.Max(x=>x.y));}
                    texts.Add(new TextRow{path=PathOf(t.transform,root.transform),text=t.text,fontSize=font,fontPixels=(t.resizeTextForBestFit?font/t.pixelsPerUnit:font)*label.height/t.rectTransform.rect.height,preferredWidth=t.preferredWidth,preferredHeight=t.preferredHeight,width=t.rectTransform.rect.width,height=t.rectTransform.rect.height,visibleCharacters=t.cachedTextGenerator.characterCountVisible,widthOverflow=!t.resizeTextForBestFit&&t.horizontalOverflow!=HorizontalWrapMode.Wrap&&t.preferredWidth>t.rectTransform.rect.width+1,heightOverflow=!t.resizeTextForBestFit&&t.preferredHeight>t.rectTransform.rect.height+1,glyphBounds=glyph,labelBounds=label});
                }
                var buttons=new List<ButtonRow>();foreach(var button in root.GetComponentsInChildren<Button>()){Rect bounds=Bounds(button.transform as RectTransform,camera);var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=bounds.center},hits);buttons.Add(new ButtonRow{path=PathOf(button.transform,root.transform),bounds=bounds,hitBounds=button.transform.Find("TouchTarget")!=null?Bounds((RectTransform)button.transform.Find("TouchTarget"),camera):bounds,enabled=button.enabled,interactable=button.IsInteractable(),centerReachable=hits.Count>0&&hits[0].gameObject.GetComponentInParent<Button>()==button});}
                measurements.Add(new Observation{page=page,state=state,language=DemoFeatureState.LanguageCode,width=size.x,height=size.y,safe=safeArea,texts=texts.ToArray(),buttons=buttons.ToArray()});
                camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);texture.Apply();var dir=Environment.GetEnvironmentVariable("LOR_CORE_OUTCOME_READABILITY_DIR");var output=System.IO.Path.Combine(dir,page+"_"+state+"_"+DemoFeatureState.LanguageCode+"_"+size.x+"x"+size.y+".png");Assert.That(File.Exists(output),Is.False);File.WriteAllBytes(output,texture.EncodeToPNG());
            }
            finally{RenderTexture.active=active;canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.planeDistance=distance;canvas.scaleFactor=scale;scaler.enabled=se;adaptive.enabled=ae;safe.enabled=fe;adaptive.ApplyNow();safe.ApplyNow();UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(cameraGo);target.Release();UnityEngine.Object.Destroy(target);}
        }
    }
}
