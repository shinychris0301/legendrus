using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class StoryRecapPlayModeTests
    {
        ScreenRouter router;
        [UnitySetUp] public IEnumerator SetUp()
        {
            string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(isolated,Is.Not.Null.And.Not.Empty);Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));
            yield return SceneManager.LoadSceneAsync("Bootstrap",LoadSceneMode.Single);
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);
            router=ScreenRouter.Instance;GameState.Instance.ResetProgress();StoryService.ResetRuntime();DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.Profile);Settle();yield return null;
        }
        [UnityTearDown] public IEnumerator TearDown(){router.Show(ScreenId.Home);Settle();StoryService.ResetRuntime();yield return null;}
        ProductionProfileView Profile()=>router.Viewport.GetComponentInChildren<ProductionProfileView>();
        ProductionStoryRecapView Reader()=>router.Viewport.GetComponentInChildren<ProductionStoryRecapView>();
        GameObject Root()=>Reader()!=null?Reader().gameObject:Profile().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        string Save=>JsonUtility.ToJson(GameState.Instance.Save);
        string Journal()=>string.Join("|",new[]{"LegendsOfRus_Save_v1","LegendsOfRus_Save_v1_Backup","LegendsOfRus_Save_v1_Staging",
            "LegendsOfRus_GameplayQA_v1_LegendsOfRus_Save_v1","LegendsOfRus_GameplayQA_v1_LegendsOfRus_Save_v1_Backup","LegendsOfRus_GameplayQA_v1_LegendsOfRus_Save_v1_Staging"}.Select(k=>PlayerPrefs.GetString(k,"absent")));
        void Seed(bool finale=false)
        {
            var save=GameState.Instance.Save;save.energy=4;save.lastEnergyUtcTicks=DateTime.UtcNow.Ticks;var catalog=StoryCatalog.LoadChapter1();save.completedStoryScenes.Add(catalog.introSceneId);
            foreach(var m in finale?catalog.milestones.Skip(catalog.milestones.Length-1):catalog.milestones.Take(2)){
                save.completedStoryScenes.Add(m.sceneId);save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=m.afterLevelId,completions=1});}
            for(int i=3;i<=10;i++)save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=i,attempts=2});
            GameState.Instance.Persist();Profile().RefreshState();Profile().SelectSection(ProfileSection.History);Profile().ChangeHistoryPage(1);
        }
        void Open(){Profile().DynamicRoot.Find("StoryRecap").GetComponent<Button>().onClick.Invoke();Settle();}
        [UnityTest] public IEnumerator EmptyHistoryIsTruthfulAndCannotOpenReader()
        {
            Profile().SelectSection(ProfileSection.History);string save=Save,journal=Journal();var button=Profile().DynamicRoot.Find("StoryRecap").GetComponent<Button>();
            Assert.That(button.interactable,Is.False);Assert.That(button.GetComponentInChildren<Text>().text,Does.Contain("НЕТ"));
            button.onClick.Invoke();Assert.That(router.ActiveModal,Is.Null);Assert.That(Save,Is.EqualTo(save));Assert.That(Journal(),Is.EqualTo(journal));
            Capture("Profile_Empty_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));yield return null;
        }
        [UnityTest] public IEnumerator NativeReaderBrowsesAndReturnsWithoutChangingStoryOrJournal()
        {
            Seed();Assert.That(StoryService.TryBeginChapterIntro(),Is.False);
            // A pending normal scene may already be active. Recap must leave its position alone.
            var pending=StoryCatalog.LoadChapter1().milestones[2];
            GameState.Instance.GetLevelProgress(1,pending.afterLevelId,true).completions=1;GameState.Instance.Persist();
            Assert.That(StoryService.TryBeginPostVictoryDialogue(),Is.True);string normal=StoryService.ActiveSceneId,line=StoryService.CurrentLine.Text;
            string save=Save,journal=Journal();var profile=Profile();int page=profile.HistoryPage;
            Capture("Profile_StoryEntry_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172),
                camera=>Click(profile.DynamicRoot.Find("StoryRecap").GetComponent<Button>(),camera));Settle();yield return null;
            var reader=Reader();Assert.That(reader.SceneIndex,Is.EqualTo(2));Assert.That(reader.Later.interactable,Is.False);
            Capture("Recap_Latest_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Recap_Earlier_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(reader.Earlier,camera));Settle();yield return null;
            Assert.That(reader.SceneIndex,Is.EqualTo(1));DemoFeatureState.SetLanguage("en");yield return null;
            Assert.That(reader.Dialogue.Description.text,Is.EqualTo(reader.Dialogue.LineProvider("en").Text));
            Capture("Recap_Earlier_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            profile.ChangeHistoryPage(1);Assert.That(profile.HistoryPage,Is.EqualTo(page),"Modal blocks the underlying controls.");
            router.NavigateBack();Settle();yield return null;Assert.That(router.ActiveModal,Is.Null);Assert.That(Profile(),Is.SameAs(profile));Assert.That(profile.HistoryPage,Is.EqualTo(page));
            Assert.That(StoryService.ActiveSceneId,Is.EqualTo(normal));Assert.That(StoryService.CurrentLine.Text,Is.EqualTo(line));Assert.That(Save,Is.EqualTo(save));Assert.That(Journal(),Is.EqualTo(journal));
        }
        [UnityTest] public IEnumerator IntroRepeatedNativeTapAdvancesOneLineAndBackCloses()
        {
            GameState.Instance.Save.completedStoryScenes.Add(StoryCatalog.LoadChapter1().introSceneId);GameState.Instance.Persist();Profile().SelectSection(ProfileSection.History);Open();yield return null;
            var reader=Reader();string save=Save,journal=Journal();
            Capture("Recap_Intro_RU",new Vector2Int(360,640),new Rect(0,20,360,600),camera=>{
                Click(reader.Dialogue.Continue,camera);reader.Dialogue.Continue.onClick.Invoke();});
            Assert.That(reader.LineIndex,Is.EqualTo(1));Settle();yield return null;
            DemoFeatureState.SetLanguage("en");yield return null;
            Capture("Recap_Back_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672),camera=>Click(reader.Back,camera));Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Profile));Assert.That(Save,Is.EqualTo(save));Assert.That(Journal(),Is.EqualTo(journal));
        }
        [UnityTest] public IEnumerator FinaleFinishAndRepeatedReentryNeverDispatchChapterComplete()
        {
            Seed(true);string save=Save,journal=Journal();var profile=Profile();int page=profile.HistoryPage;
            for(int visit=0;visit<2;visit++){
                Open();yield return null;var reader=Reader();Assert.That(reader.Dialogue.SceneIdProvider(),Is.EqualTo(StoryCatalog.LoadChapter1().completion.finaleSceneId));
                Capture("Recap_Finale_"+visit,new Vector2Int(576,1280),new Rect(0,48,576,1172));
                int lines=StoryCatalog.FindScene(StoryCatalog.LoadChapter1().completion.finaleSceneId).lines.Length;
                for(int i=0;i<lines;i++){reader.Dialogue.Continue.onClick.Invoke();reader.Dialogue.Continue.onClick.Invoke();Settle();yield return null;}
                Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Profile));Assert.That(profile.HistoryPage,Is.EqualTo(page));
                Assert.That(Save,Is.EqualTo(save));Assert.That(Journal(),Is.EqualTo(journal));
            }
            Assert.That(GameState.Instance.Save.acknowledgedChapterCompletions,Is.Empty);
        }
        [UnityTest] public IEnumerator DepartureRetiresReaderWithoutStaleCallbacks()
        {
            Seed();Open();yield return null;var old=Reader();string save=Save;
            router.Show(ScreenId.Settings);Settle();old.Back.onClick.Invoke();old.Dialogue.Continue.onClick.Invoke();yield return null;
            Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Settings));Assert.That(Save,Is.EqualTo(save));
            router.Show(ScreenId.Profile);Settle();Assert.That(Profile().Section,Is.EqualTo(ProfileSection.Profile));
        }
        [UnityTest] public IEnumerator DisposableRecapPreservesSyntheticNormalJournal()
        {
            Assert.That(Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA"),Is.EqualTo("fresh"));
            foreach(var key in new[]{"LegendsOfRus_Save_v1","LegendsOfRus_Save_v1_Backup","LegendsOfRus_Save_v1_Staging"})PlayerPrefs.SetString(key,"synthetic-normal-sentinel");PlayerPrefs.Save();
            Seed();string before=Journal();Open();yield return null;Reader().Earlier.onClick.Invoke();yield return null;router.NavigateBack();Settle();yield return null;
            Assert.That(Journal(),Is.EqualTo(before));Assert.That(PlayerPrefs.GetString("LegendsOfRus_Save_v1"),Is.EqualTo("synthetic-normal-sentinel"));
        }
        static void Click(Button button,Camera camera)
        {
            var corners=new Vector3[4];button.image.rectTransform.GetWorldCorners(corners);Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,(corners[0]+corners[2])*.5f);
            var pointer=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),
                button.name+" at "+point+" hits "+string.Join(",",hits.Select(h=>h.gameObject.name)));ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
        }
        void Capture(string name,Vector2Int size,Rect safeArea,Action<Camera> inspect=null)
        {
            string directory=Environment.GetEnvironmentVariable("LOR_RECAP_CAPTURE_DIR");Assert.That(directory,Is.Not.Empty);Directory.CreateDirectory(directory);
            var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldDistance=canvas.planeDistance,oldScale=canvas.scaleFactor;
            bool se=scaler.enabled,ae=adaptive.enabled,fe=safe.enabled;var oldActive=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);var cameraGo=new GameObject("RecapCaptureCamera",typeof(Camera));var texture=new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try{
                target.Create();scaler.enabled=adaptive.enabled=safe.enabled=false;var camera=cameraGo.GetComponent<Camera>();camera.enabled=false;
                camera.transform.position=new Vector3(0,0,-100);camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.targetTexture=target;
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(4,13,24,255);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;
                adaptive.Apply(safeArea,size);canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int((int)safeArea.width,(int)safeArea.height),941,1672);safe.Apply(safeArea,size);
                Canvas.ForceUpdateCanvases();foreach(var text in Root().GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                // Newly rebuilt Profile graphics need one render to establish raycast depths.
                camera.Render();
                foreach(var button in Root().GetComponentsInChildren<Button>())if(button.IsInteractable()){
                    var corners=new Vector3[4];button.image.rectTransform.GetWorldCorners(corners);var low=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);var high=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
                    Assert.That(low.x>=safeArea.xMin-1&&low.y>=safeArea.yMin-1&&high.x<=safeArea.xMax+1&&high.y<=safeArea.yMax+1,Is.True,button.name+" safe bounds");}
                inspect?.Invoke(camera);Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);texture.Apply();File.WriteAllBytes(Path.Combine(directory,name+".png"),texture.EncodeToPNG());
                File.WriteAllText(Path.Combine(directory,name+".json"),"{\"actualUnityCapture\":true,\"simulatedViewport\":true,\"width\":"+size.x+",\"height\":"+size.y+"}");
            }finally{RenderTexture.active=oldActive;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldDistance;canvas.scaleFactor=oldScale;scaler.enabled=se;adaptive.enabled=ae;safe.enabled=fe;UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(cameraGo);target.Release();UnityEngine.Object.DestroyImmediate(target);Canvas.ForceUpdateCanvases();}
        }
    }
}
