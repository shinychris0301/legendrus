using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class ActiveCampaignIntegrationTests
    {
        static LevelData Level(int n)=>ChapterLevelCatalog.Find(1,n);
        static BoardModel Start(LevelData l,int attempt)=>LevelBoardFactory.Create(l,BoardSeedUtility.CreateRuntimeSeed(1,l.levelId,l.boardSeed+attempt),rules:Match3Board.CurrentGameplayRules);
        [Test] public void RuntimeResourcesMatchCandidateAndBothLanguagesValidate()
        {
            var c=ActiveCampaignContent.Load(true);var levels=ChapterLevelCatalog.LoadChapter1(true);
            Assert.That(levels.levels.Length,Is.EqualTo(500));
            foreach(var name in new[]{"levels","story","story_ru","story_en"})
                Assert.That(Resources.Load<TextAsset>(ActiveCampaignContent.ResourceRoot+name).text,Is.EqualTo(File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01CompleteR44",name+".json"))));
            var previous=JsonUtility.FromJson<ChapterLevelCatalogData>(File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01EditorialR42/levels.json")));
            for(int i=0;i<200;i++)Assert.That(JsonUtility.ToJson(levels.levels[i]),Is.EqualTo(JsonUtility.ToJson(previous.levels[i])));
            Assert.That(ChapterLevelCatalog.Next(Level(190)).levelId,Is.EqualTo(191));
            Assert.That(c.LoadStory(1).milestones.Single(m=>m.afterLevelId==200).sceneId,Is.EqualTo("campaign.chapter01.orchard_field_paths"));
            Assert.That(levels.levels.Skip(200).Sum(l=>l.rewards.gold),Is.EqualTo(55200));
            foreach(var language in new[]{"ru","en"})Assert.That(StoryCatalog.ValidateCampaign(c.LoadStory(1),c.LoadLocalization(1,language),levels,true).Errors,Is.Empty);
            Assert.That(StoryCatalog.LoadChapter1(true).completion?.finalLevelId??0,Is.EqualTo(500));
            Assert.That(ChapterLevelCatalog.Next(Level(10)).levelId,Is.EqualTo(11));
            Assert.That(ChapterLevelCatalog.Next(Level(500)),Is.Null);Assert.That(ChapterLevelCatalog.Find(2,1),Is.Null);
            foreach(var l in levels.levels)for(int a=1;a<=3;a++){var m=Start(l,a);Assert.That(m.HasAnyMatches(),Is.False,l.id);Assert.That(m.HasLegalMove(),Is.True,l.id);Assert.That(m.ComputeStateHash(),Is.EqualTo(Start(l,a).ComputeStateHash()));}
        }
        sealed class Node {public BoardSnapshot Board;public LevelGoalRuntimeSnapshot Goals;public int Score;public float Cost;public List<MechanicsReplayCommand> Path=new List<MechanicsReplayCommand>();}
        [Serializable] sealed class Witness {public int round,attempt,moveLimit,moves,expanded;public string initialHash,finalHash;public MechanicsReplayCommand[] commands;}
        static BoardModel Restore(BoardSnapshot s){var m=new BoardModel(s.Width,s.Height,s.TileTypeCount,new SeededBoardRandom(1)){SquareMatches=true,StandaloneSpecials=true};m.RestoreSnapshot(s);return m;}
        static IEnumerable<MechanicsReplayCommand> Actions(BoardSnapshot s)
        {
            var m=Restore(s);var token=s.ToReplayToken();
            for(int y=0;y<m.Height;y++)for(int x=0;x<m.Width;x++){
                var p=new BoardPosition(x,y);
                if(m.GetSpecial(p)!=SpecialGemKind.None)yield return new MechanicsReplayCommand{Rules=Match3Board.CurrentGameplayRules,BeforeToken=token,Tap=true,X=x,Y=y};
                foreach(var q in new[]{new BoardPosition(x+1,y),new BoardPosition(x,y+1)})
                    if(m.IsInside(q)&&m.CanSwap(p,q)&&(m.GetSpecial(p)!=SpecialGemKind.None||m.GetSpecial(q)!=SpecialGemKind.None||m.SwapCreatesMatch(p,q)))
                        yield return new MechanicsReplayCommand{Rules=Match3Board.CurrentGameplayRules,BeforeToken=token,X=x,Y=y,ToX=q.X,ToY=q.Y};
            }
        }
        static Witness Solve(LevelData l,int attempt)
        {
            var start=Start(l,attempt).CreateSnapshot();var nodes=new List<Node>{new Node{Board=start,Goals=new LevelGoalRuntime(l,0).CaptureSnapshot()}};int expanded=0;
            for(int depth=0;depth<l.moveLimit;depth++){
                var next=new List<Node>();var seen=new HashSet<string>();
                foreach(var n in nodes)foreach(var action in Actions(n.Board)){
                    Assert.That(++expanded,Is.LessThan(14000),"Search cap "+l.id);var p=action.Resolve();if(!p.IsResolved||!p.ConsumesMove)continue;
                    var g=new LevelGoalRuntime(l,0);g.RestoreSnapshot(n.Goals);int score=n.Score;foreach(var w in p.Waves){score+=w.ScoreDelta;g.ApplyWave(w,score);}g.RecordConsumedMove();
                    action.ExpectedPlanHash=p.PlanHash;var path=new List<MechanicsReplayCommand>(n.Path){action};
                    if(g.AllComplete)return new Witness{round=l.levelId,attempt=attempt,moveLimit=l.moveLimit,moves=path.Count,expanded=expanded,initialHash=start.StateHash,finalHash=p.FinalSnapshot.StateHash,commands=path.ToArray()};
                    var key=p.FinalSnapshot.StateHash+":"+string.Join(",",Enumerable.Range(0,g.SlotCount).Select(i=>g.RemainingAt(i)));if(!seen.Add(key))continue;
                    float cost=Enumerable.Range(0,g.SlotCount).Sum(i=>1000f*g.RemainingAt(i)/l.goals[i].target)+p.FinalSnapshot.ObstacleLayers.Sum(x=>(int)x)*5-p.FinalSnapshot.Specials.Count(x=>x!=SpecialGemKind.None)*5;
                    next.Add(new Node{Board=p.FinalSnapshot,Goals=g.CaptureSnapshot(),Score=score,Path=path,Cost=cost});
                }
                nodes=next.OrderBy(n=>n.Cost).ThenBy(n=>n.Board.StateHash,StringComparer.Ordinal).Take(8).ToList();
            }
            Assert.Fail("No inventory-free witness: "+l.id+" attempt "+attempt);return null;
        }
        static string Output {get{string p=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR")??Path.Combine(Application.temporaryCachePath,"CampaignSectorCandidate");Directory.CreateDirectory(p);return p;}}
        static IEnumerable<int> AuthoredRounds => Enumerable.Range(1,500);
        [TestCaseSource(nameof(AuthoredRounds))]
        public void ThreeAttemptSeedsHaveInventoryFreeReplayWitnesses(int round)
        {
            var l=Level(round);for(int attempt=1;attempt<=3;attempt++){
                var w=Solve(l,attempt);var m=Start(l,attempt);var goals=new LevelGoalRuntime(l,0);int score=0;
                foreach(var a in w.commands){Assert.That(a.BeforeToken,Is.EqualTo(m.CreateSnapshot().ToReplayToken()));var p=a.Resolve();Assert.That(p.IsResolved&&p.ConsumesMove,Is.True);foreach(var wave in p.Waves){score+=wave.ScoreDelta;goals.ApplyWave(wave,score);}goals.RecordConsumedMove();m.RestoreSnapshot(p.FinalSnapshot);}
                Assert.That(goals.AllComplete,Is.True);Assert.That(m.ComputeStateHash(),Is.EqualTo(w.finalHash));Assert.That(w.moves,Is.LessThanOrEqualTo(l.moveLimit));
                File.WriteAllText(Path.Combine(Output,$"Round{round}_Attempt{attempt}_Witness.json"),JsonUtility.ToJson(w,true));
            }
        }

        static void Isolation()
        {
            var root=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(root,Is.Not.Null.And.Not.Empty);
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(root)+Path.DirectorySeparatorChar,StringComparison.Ordinal),Is.True);
        }
        [UnityTest] public IEnumerator ExistingTenRoundSaveKeepsBalancesAndUnlocksEleven() { yield return SaveMigration(10); }
        [UnityTest] public IEnumerator ExistingFortyRoundSaveKeepsBalancesAndUnlocksFortyOne() { yield return SaveMigration(40); }
        [UnityTest] public IEnumerator ExistingFiftyRoundSaveKeepsBalancesAndUnlocksFiftyOne() { yield return SaveMigration(50); }
        [UnityTest] public IEnumerator ExistingSixtyRoundSaveKeepsBalancesAndUnlocksSixtyOne() { yield return SaveMigration(60); }
        [UnityTest] public IEnumerator ExistingSeventyRoundSaveKeepsBalancesAndUnlocksSeventyOne() { yield return SaveMigration(70); }
        [UnityTest] public IEnumerator ExistingEightyRoundSaveKeepsBalancesAndUnlocksEightyOne() { yield return SaveMigration(80); }
        [UnityTest] public IEnumerator ExistingNinetyRoundSaveKeepsBalancesAndUnlocksNinetyOne() { yield return SaveMigration(90); }
        [UnityTest] public IEnumerator ExistingHundredRoundSaveKeepsBalancesAndUnlocksHundredOne() { yield return SaveMigration(100); }
        [UnityTest] public IEnumerator ExistingHundredTenRoundSaveKeepsBalancesAndUnlocksHundredEleven() { yield return SaveMigration(110); }
        [UnityTest] public IEnumerator ExistingHundredTwentyRoundSaveKeepsBalancesAndUnlocksHundredTwentyOne() { yield return SaveMigration(120); }
        [UnityTest] public IEnumerator ExistingHundredThirtyRoundSaveKeepsBalancesAndUnlocksHundredThirtyOne() { yield return SaveMigration(130); }
        [UnityTest] public IEnumerator ExistingHundredFortyRoundSaveKeepsBalancesAndUnlocksHundredFortyOne() { yield return SaveMigration(140); }
        [UnityTest] public IEnumerator ExistingHundredFiftyRoundSaveKeepsBalancesAndUnlocksHundredFiftyOne() { yield return SaveMigration(150); }
        [UnityTest] public IEnumerator ExistingHundredSixtyRoundSaveKeepsBalancesAndUnlocksHundredSixtyOne() { yield return SaveMigration(160); }
        [UnityTest] public IEnumerator ExistingHundredSeventyRoundSaveKeepsBalancesAndUnlocksHundredSeventyOne() { yield return SaveMigration(170); }
        [UnityTest] public IEnumerator ExistingHundredEightyRoundSaveKeepsBalancesAndUnlocksHundredEightyOne() { yield return SaveMigration(180); }
        [UnityTest] public IEnumerator ExistingHundredNinetyRoundSaveKeepsBalancesAndUnlocksHundredNinetyOne() { yield return SaveMigration(190); }
        [UnityTest] public IEnumerator ExistingTwoHundredSaveUnlocks201WithoutReplayRewards() { yield return SaveMigration(200); }
        [UnityTest] public IEnumerator Existing490SaveReaches500WithoutEarlyFinale() { yield return SaveMigration(490); }
        IEnumerator SaveMigration(int completed)
        {
            Isolation();var obj=GameState.Instance==null?new GameObject("ActiveCampaignMigrationTest",typeof(GameState)):null;var state=GameState.Instance;var previous=JsonUtility.ToJson(state.Save);
            try {
                state.Save=new PlayerSaveData{gold=777,gems=19,energy=7,energyMax=30,highestUnlockedLevel=completed,selectedLevel=completed,lastEnergyUtcTicks=DateTime.UtcNow.Ticks};
                state.Save.levelProgress=Enumerable.Range(1,completed).Select(n=>new LevelProgressData{chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=2,attempts=1}).ToList();
                state.Persist();var saved=SaveService.LoadWithResult();Assert.That(saved.CanPersist,Is.True);state.Save=saved.Data;
                typeof(GameState).GetMethod("MigrateSave",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(state,null);
                Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(completed+1));Assert.That(state.Save.selectedLevel,Is.EqualTo(completed));
                Assert.That(state.Save.gold,Is.EqualTo(777));Assert.That(state.Save.gems,Is.EqualTo(19));Assert.That(state.Save.energy,Is.EqualTo(7));Assert.That(state.Save.levelProgress.Count,Is.EqualTo(completed));
                Assert.That(state.SelectLevel(1,completed+1),Is.True);Assert.That(state.SelectLevel(1,completed+2),Is.False);
                var replay=state.CompleteLevel(Level(completed),3000);Assert.That(state.Save.gold,Is.EqualTo(777));
                int gold=state.Save.gold,gems=state.Save.gems,mace=state.Save.boosterMace,pearl=state.Save.boosterSeaPearl;int firebird=state.Save.boosterFirebird,light=state.Save.boosterLivingLight;
                int end=Math.Min(500,completed+10);
                for(int n=completed+1;n<=end;n++){Assert.That(state.SelectLevel(1,n),Is.True);state.CompleteLevel(Level(n),3000);}
                Assert.That(state.Save.gold-gold,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.gold)));
                Assert.That(state.Save.gems-gems,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.gems)));
                Assert.That(state.Save.boosterMace-mace,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.boosterMace)));
                Assert.That(state.Save.boosterSeaPearl-pearl,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.boosterSeaPearl)));
                Assert.That(state.Save.boosterFirebird-firebird,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.boosterFirebird)));
                Assert.That(state.Save.boosterLivingLight-light,Is.EqualTo(Enumerable.Range(completed+1,end-completed).Sum(n=>Level(n).rewards.boosterLivingLight)));
                light=state.Save.boosterLivingLight;
                mace=state.Save.boosterMace;pearl=state.Save.boosterSeaPearl;firebird=state.Save.boosterFirebird;
                Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(Math.Min(500,end+1)));Assert.That(state.SelectLevel(1,end+1),Is.EqualTo(end<500));Assert.That(state.SelectLevel(2,1),Is.False);Assert.That(StoryService.ShouldShowChapterComplete(),Is.False);
                gold=state.Save.gold;gems=state.Save.gems;state.CompleteLevel(Level(end),4000);Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.gems,Is.EqualTo(gems));
                Assert.That(state.Save.boosterMace,Is.EqualTo(mace));Assert.That(state.Save.boosterSeaPearl,Is.EqualTo(pearl));Assert.That(state.Save.boosterFirebird,Is.EqualTo(firebird));Assert.That(state.Save.boosterLivingLight,Is.EqualTo(light));
                state.Persist();state.Save=SaveService.LoadWithResult().Data;Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(Math.Min(500,end+1)));Assert.That(state.GetLevelProgress(1,end).completions,Is.EqualTo(2));
            } finally { state.Save=JsonUtility.FromJson<PlayerSaveData>(previous);if(obj)UnityEngine.Object.Destroy(obj);StoryService.ResetRuntime(); }
            yield return null;
        }
        [UnityTest,Timeout(600000)] public IEnumerator NormalRouterPlaysNewSectorsAndFinaleWithNativeRewardsAndStory()
        {
            Isolation();var stateObject=GameState.Instance==null?new GameObject("ActiveCampaignRuntimeState",typeof(GameState)):null;var obj=ScreenRouter.Instance==null?new GameObject("ActiveCampaignRouterTest",typeof(ScreenRouter)):null;var router=obj?obj.GetComponent<ScreenRouter>():ScreenRouter.Instance;router.Initialize();yield return null;
            try {
                foreach(int n in new[]{200,201,245,298,350,400,450,495,500}){
                    var state=GameState.Instance;Assert.That(state,Is.Not.Null);
                    state.Save=new PlayerSaveData{highestUnlockedLevel=n,selectedLevel=n,energy=30,energyMax=30,lastEnergyUtcTicks=DateTime.UtcNow.Ticks,gold=0,gems=0};
                    state.Save.levelProgress=Enumerable.Range(1,n-1).Select(i=>new LevelProgressData{chapterId=1,levelId=i,completions=1,rewardClaimed=true}).ToList();
                    var catalog=StoryCatalog.LoadChapter1();
                    state.Save.completedStoryScenes=new[]{catalog.introSceneId}.Concat(catalog.milestones.Where(m=>m.afterLevelId<n).Select(m=>m.sceneId)).ToList();
                    StoryService.ResetRuntime();router.Show(ScreenId.ChapterMap);yield return Settle(router);
                    var map=router.GameCanvas.GetComponentInChildren<ProductionChapterMapView>();Assert.That(map.LevelAvailable(n),Is.True);Assert.That(map.LevelAvailable(501),Is.False);
                    if(n>=100){
                        map.Scroller.JumpToCurrent();yield return Settle(router);
                        var node=map.Nodes.Single(x=>x.LevelId==n);Assert.That(node.Root.gameObject.activeInHierarchy,Is.True);
                        Assert.That(node.Number.text,Is.EqualTo(n.ToString()));Assert.That(node.Button.IsInteractable(),Is.True);
                        node.Button.onClick.Invoke();
                    }else map.PlayButton.onClick.Invoke();
                    yield return Settle(router);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
                    router.GameCanvas.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return Settle(router);
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));var board=router.GameCanvas.GetComponentInChildren<ProductionGameplayView>().Board;
                    Assert.That(board.CurrentLevel.levelId,Is.EqualTo(n));Assert.That(board.Moves,Is.EqualTo(Level(n).moveLimit));Assert.That(board.RuleVersion,Is.EqualTo(GameplayRuleVersion.PrototypeR4));
                    var witness=Solve(Level(n),1);Assert.That(board.BoardStateHash,Is.EqualTo(witness.initialHash));
                    foreach(var command in witness.commands){
                        Assert.That(board.BoardReplayToken,Is.EqualTo(command.BeforeToken));
                        bool accepted=command.Tap?board.TapCell(command.X,command.Y):board.SwipeCell(command.X,command.Y,new Vector2Int(command.ToX-command.X,command.ToY-command.Y));Assert.That(accepted,Is.True);
                        float deadline=Time.realtimeSinceStartup+20;
                        while(router.CurrentScreen==ScreenId.Gameplay&&board&&(board.HasActiveCommand||!board.CanAcceptPointerInput)&&Time.realtimeSinceStartup<deadline)yield return null;
                        Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline));yield return Settle(router);
                    }
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Victory));Assert.That(state.Save.energy,Is.EqualTo(29));Assert.That(state.Save.gold,Is.EqualTo(Level(n).rewards.gold));Assert.That(state.GetLevelProgress(1,n).completions,Is.EqualTo(1));
                    router.GameCanvas.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return Settle(router);
                    int lines=0;while(router.CurrentScreen==ScreenId.Dialogue){Assert.That(++lines,Is.LessThan(10));router.GameCanvas.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();yield return Settle(router);}
                    if(n==500) {
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterComplete));
                        Assert.That(StoryService.ShouldShowChapterComplete(),Is.True);
                        var complete=router.GameCanvas.GetComponentInChildren<ProductionChapterCompleteView>();
                        Assert.That(complete.Summary.text,Does.Contain("500/500"));
                        var received=ProductionChapterCompleteView.ReceivedRewards(state);
                        Assert.That(received[0],Is.EqualTo(ChapterLevelCatalog.LoadChapter1().levels.Sum(l=>l.rewards.gold)));
                        string previousLanguage=DemoFeatureState.LanguageCode;
                        foreach(string language in new[]{"ru","en"}) {
                            DemoFeatureState.SetLanguage(language);complete.RefreshContent();yield return Settle(router);
                            CaptureFinale(router,complete,language,new Vector2Int(576,1280),new Rect(0,48,576,1172));
                            CaptureFinale(router,complete,language,new Vector2Int(360,640),new Rect(0,20,360,600));
                        }
                        DemoFeatureState.SetLanguage(previousLanguage);complete.RefreshContent();
                        complete.Actions[0].onClick.Invoke();yield return Settle(router);
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
                        Assert.That(state.HasAcknowledgedChapterCompletion(1),Is.True);
                        Assert.That(state.SelectLevel(2,1),Is.False);
                    } else {
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
                        var milestone=catalog.milestones.SingleOrDefault(m=>m.afterLevelId==n);
                        if(milestone!=null)Assert.That(state.HasCompletedStoryScene(milestone.sceneId),Is.True);
                        Assert.That(StoryService.ShouldShowChapterComplete(),Is.False);
                    }
                    Assert.That(state.Save.gold,Is.EqualTo(Level(n).rewards.gold),"Finale cannot grant a second reward");
                    router.Show(ScreenId.Home);yield return Settle(router);
                }
            } finally {router.Show(ScreenId.Home);if(obj)UnityEngine.Object.Destroy(obj);if(stateObject)UnityEngine.Object.Destroy(stateObject);StoryService.ResetRuntime();}
            yield return null;
        }
        // Render the actual native page with full-chapter totals and the production safe-area scaler.
        static void CaptureFinale(ScreenRouter router,ProductionChapterCompleteView view,string language,Vector2Int size,Rect area)
        {
            string directory=Environment.GetEnvironmentVariable("LOR_CHAPTER_COMPLETE_CAPTURE_DIR");
            Assert.That(directory,Is.Not.Null.And.Not.Empty);Directory.CreateDirectory(directory);
            var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;float distance=canvas.planeDistance,scale=canvas.scaleFactor;
            bool se=scaler.enabled,ae=adaptive.enabled,fe=safe.enabled;var active=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);var cameraObject=new GameObject("FullChapterCapture",typeof(Camera));var texture=new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try {
                target.Create();scaler.enabled=false;adaptive.enabled=false;safe.enabled=false;
                var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.transform.position=new Vector3(0,0,-100);camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.targetTexture=target;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;
                adaptive.Apply(area,size);canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int(Mathf.RoundToInt(area.width),Mathf.RoundToInt(area.height)),941,1672);safe.Apply(area,size);
                foreach(var text in view.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                foreach(var text in view.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                Assert.That(view.Summary.text,Does.Contain("500/500"));
                Assert.That(view.RewardCounts[0].text,Is.EqualTo("91 970"));
                foreach(var text in view.GetComponentsInChildren<Text>())if(text.enabled&&!string.IsNullOrEmpty(text.text)) {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.name+" width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.name+" height");
                }
                foreach(var button in view.Actions) {
                    var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);
                    Vector2 min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
                    Assert.That(area.Contains(min)&&area.Contains(max),Is.True,button.name+" safe area");
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=(min+max)*.5f},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                }
                camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);texture.Apply();
                File.WriteAllBytes(Path.Combine(directory,$"Chapter1_500_{language}_{size.x}.png"),texture.EncodeToPNG());
            } finally {
                RenderTexture.active=active;canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.planeDistance=distance;canvas.scaleFactor=scale;
                scaler.enabled=se;adaptive.enabled=ae;safe.enabled=fe;adaptive.ApplyNow();safe.ApplyNow();
                UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(cameraObject);target.Release();UnityEngine.Object.Destroy(target);
            }
        }
        static IEnumerator Settle(ScreenRouter r){for(int i=0;i<24;i++)r.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);yield return null;yield return null;}
    }
}
