using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.GameplayCore;
using LegendsOfRus.SaveCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignGranaryCandidateTests
    {
        static string Read(string name)=>File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R13",name+".json"));
        static CampaignDefinition Definition()=>JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(Path.Combine(Application.dataPath,"../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
        static ValidatedCampaignCatalog Catalog()
        {
            var c=new ValidatedCampaignCatalog(Definition());Assert.That(c.TryAdd(new CampaignPackageInput{ChapterId=1,ContentRevision=1,LevelsJson=Read("levels"),StoryJson=Read("story"),RussianJson=Read("story_ru"),EnglishJson=Read("story_en")},out var errors),Is.True,string.Join("\n",errors));return c;
        }
        static LevelData Level(int n)=>Catalog().LoadChapter(1).levels[n-1];
        static BoardModel Start(LevelData l,int attempt)=>LevelBoardFactory.Create(l,BoardSeedUtility.CreateRuntimeSeed(1,l.levelId,l.boardSeed+attempt),rules:Match3Board.CurrentGameplayRules);
        [Test] public void ThirtyRoundsPreserveR12AndGranaryIsNotChapterCompletion()
        {
            var c=Catalog();var levels=c.LoadChapter(1);Assert.That(levels.levels.Length,Is.EqualTo(30));
            var prior=JsonUtility.FromJson<ChapterLevelCatalogData>(File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R12/levels.json")));
            for(int n=0;n<20;n++)Assert.That(JsonUtility.ToJson(levels.levels[n]),Is.EqualTo(JsonUtility.ToJson(prior.levels[n])));
            Assert.That(c.LoadStory(1).milestones.Single(m=>m.afterLevelId==30).sceneId,Is.EqualTo("campaign.chapter01.granary_open"));
            Assert.That(c.LoadStory(1).completion==null||c.LoadStory(1).completion.finalLevelId==0,Is.True);
            var progress=c.Progression(Enumerable.Range(1,30).Select(n=>new CampaignProgressRecord{ChapterId=1,RoundId=n,Completions=1}),Array.Empty<int>());
            Assert.That(progress.Availability(1,31),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable));Assert.That(progress.Availability(2,1),Is.Not.EqualTo(CampaignRoundAvailability.Playable));
            var reward=levels.levels.Skip(20).Select(l=>l.rewards);Assert.That(reward.Sum(r=>r.gold),Is.EqualTo(1830));Assert.That(reward.Sum(r=>r.gems),Is.EqualTo(6));Assert.That(reward.Sum(r=>r.boosterFirebird),Is.EqualTo(1));
        }
        [Test] public void NinetyStartsArePlayableDeterministicAndColorOpportunityIsGuarded()
        {
            foreach(var l in Catalog().LoadChapter(1).levels)for(int attempt=1;attempt<=3;attempt++){
                var m=Start(l,attempt);Assert.That(m.HasAnyMatches(),Is.False,l.id);Assert.That(m.HasLegalMove(),Is.True,l.id);Assert.That(Start(l,attempt).ComputeStateHash(),Is.EqualTo(m.ComputeStateHash()));
                if(l.levelId==24){var plan=new BoardResolutionPlanner(m,null,Match3Board.CurrentGameplayRules).PlanSwap(new BoardPosition(3,2),new BoardPosition(3,3));Assert.That(plan.IsResolved,Is.True);Assert.That(plan.Waves.SelectMany(w=>w.Creations).Any(x=>x.Kind==SpecialGemKind.Color),Is.True,"Color lesson attempt "+attempt);}
            }
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
        [TestCase(21)][TestCase(22)][TestCase(23)][TestCase(24)][TestCase(25)][TestCase(26)][TestCase(27)][TestCase(28)][TestCase(29)][TestCase(30)]
        public void ThreeAttemptSeedsHaveInventoryFreeReplayWitnesses(int round)
        {
            var l=Level(round);for(int attempt=1;attempt<=3;attempt++){
                var w=Solve(l,attempt);var m=Start(l,attempt);var goals=new LevelGoalRuntime(l,0);int score=0;
                foreach(var a in w.commands){Assert.That(a.BeforeToken,Is.EqualTo(m.CreateSnapshot().ToReplayToken()));var p=a.Resolve();Assert.That(p.IsResolved&&p.ConsumesMove,Is.True);foreach(var wave in p.Waves){score+=wave.ScoreDelta;goals.ApplyWave(wave,score);}goals.RecordConsumedMove();m.RestoreSnapshot(p.FinalSnapshot);}
                Assert.That(goals.AllComplete,Is.True);Assert.That(m.ComputeStateHash(),Is.EqualTo(w.finalHash));Assert.That(w.moves,Is.LessThanOrEqualTo(l.moveLimit));
                File.WriteAllText(Path.Combine(Output,$"Round{round}_Attempt{attempt}_Witness.json"),JsonUtility.ToJson(w,true));
            }
        }
        sealed class Store:ISaveJournalStore {readonly Dictionary<string,string> v=new Dictionary<string,string>();public bool TryRead(string k,out string s)=>v.TryGetValue(k,out s);public void Write(string k,string s)=>v[k]=s;public void Delete(string k)=>v.Remove(k);public void Flush(){}}
        static CampaignSaveSession Fixture(int completed)
        {
            var s=new CampaignSaveSession(new Store(),Definition(),Catalog());var p=new PlayerSaveData{energy=30,energyMax=30,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0,lastEnergyUtcTicks=DateTime.UtcNow.Ticks,selectedLevel=Math.Max(1,Math.Min(10,completed))};
            p.levelProgress=Enumerable.Range(1,Math.Min(10,completed)).Select(n=>new LevelProgressData{chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=1}).ToList();Assert.That(s.ImportLegacyCopy(JsonUtility.ToJson(p)).Committed,Is.True);
            // Only 1–10 are legacy data. Later completed rounds are a declared capture fixture,
            // produced through real campaign transactions, never invented legacy-format records.
            for(int n=11;n<=completed;n++){
                Assert.That(s.SelectRound(1,n).Committed,Is.True);
                Assert.That(s.StartRound(n-11,DateTime.UtcNow.Ticks).Committed,Is.True);
                var l=s.ResolveSelectedRound();Assert.That(s.FinishRound(n-10,true,3000,l.goals.Select(g=>g.target).ToArray()).Committed,Is.True);
                Assert.That(s.AcknowledgeResult(n-10).Committed,Is.True);
            }
            return s;
        }
        static void Drain(CampaignSaveSession s){int guard=0;while(s.TryGetPendingStory(out var p,out _)&&p!=null){Assert.That(++guard,Is.LessThan(20));Assert.That(s.AcknowledgeStory(p.Key).Committed,Is.True);}}
        [Test] public void JumpFixtureRetainsOnlyEarnedInventoryAndTwentyEnergy()
        {
            var s=Fixture(20);s.TryLoad(out var save,out _);Assert.That(save.player.energy,Is.EqualTo(20));Assert.That(save.player.boosterMace,Is.EqualTo(1));
            Assert.That(save.player.boosterFirebird+save.player.boosterLivingLight+save.player.boosterSeaPearl,Is.Zero);Assert.That(save.runSequence,Is.EqualTo(10));
            Assert.That(s.SelectRound(1,21).Committed,Is.True);Assert.That(s.Availability(1,22),Is.Not.EqualTo(CampaignRoundAvailability.Playable));
        }
        static void DrainUI(CampaignDevelopmentUI ui){int count=0;while(ui.VisiblePage=="Dialogue"){Assert.That(++count,Is.LessThan(20));ui.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();}}
        static void Capture(GameObject root,string name)=>typeof(CampaignUIIntegrationTests).GetMethod("Capture",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{root,name,576,1280});
        [UnityTest,Timeout(300000)] public IEnumerator AnimatedTwentyOneAndThirtyCompleteWithStoryAndRecordingBoundary()
        {
            string language=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("en");
            string directory=Path.Combine(Path.GetTempPath(),"GranaryTest-"+Guid.NewGuid().ToString("N"));
            try{
                foreach(int round in new[]{21,30}){
                    var s=Fixture(round-1);
                    if(round==21){while(s.TryGetPendingStory(out var pending,out _)&&pending!=null&&!pending.Key.EndsWith("well_cleared"))s.AcknowledgeStory(pending.Key);}else Drain(s);
                    Assert.That(s.SelectRound(1,round).Committed,Is.True);var root=new GameObject("GranaryJourney",typeof(RectTransform),typeof(Canvas));
                    try{
                        var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>s,()=>{});
                        var recorder=CampaignPlaytestRecorder.Attach(ui,s,"CampaignSector01R13-"+CampaignPlaytestRecorder.Fingerprint(Read("levels")+"\n"+Read("story")+"\n"+Read("story_ru")+"\n"+Read("story_en")),directory,true,30,21);
                        yield return null;if(round==21){Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Capture(root,"WellBefore21_EN");DrainUI(ui);}
                        ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;Capture(root,"Granary_Round"+round);
                        var witness=Solve(Level(round),1);
                        foreach(var a in witness.commands){var board=ui.Board;Assert.That(board.BoardReplayToken,Is.EqualTo(a.BeforeToken));Assert.That(a.Tap?board.TapCell(a.X,a.Y):board.SwipeCell(a.X,a.Y,new Vector2Int(a.ToX-a.X,a.ToY-a.Y)),Is.True);
                            float end=Time.realtimeSinceStartup+20;while(ui.Board&&ui.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(Time.realtimeSinceStartup,Is.LessThan(end));yield return null;}
                        Assert.That(ui.VisiblePage,Is.EqualTo("Victory"));Assert.That(recorder.Log.Current.outcome,Is.EqualTo("won"));Capture(root,"Granary_Victory"+round);
                        ui.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return null;
                        if(round==21){Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));s.TryLoad(out var next,out _);Assert.That(next.player.selectedLevel,Is.EqualTo(22));}
                        else{
                            Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Capture(root,"GranaryStory_EN");DemoFeatureState.SetLanguage("ru");ui.GetComponentInChildren<ProductionDialogueView>().Refresh();Capture(root,"GranaryStory_RU");DrainUI(ui);yield return null;
                            Assert.That(ui.PlaytestComplete,Is.True);Capture(root,"GranaryComplete_RU");var dialog=ui.GetComponentInChildren<ProductionErrorDialogView>();Assert.That(dialog.Description.text,Does.Contain("21–30"));Assert.That(dialog.Description.text,Does.Contain("31"));
                            DemoFeatureState.SetLanguage("en");yield return new WaitForSecondsRealtime(.3f);Capture(root,"GranaryComplete_EN");
                            s.TryLoad(out var before,out _);Assert.That(ui.Coordinator.Recover(),Is.True);s.TryLoad(out var after,out _);Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(before)));Assert.That(after.player.acknowledgedChapterCompletions,Is.Empty);Assert.That(s.SelectRound(1,31).Committed,Is.False);
                            ui.ReplayCompletedPlaytest();Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));s.TryLoad(out var selected,out _);Assert.That(selected.player.energy,Is.EqualTo(before.player.energy));Assert.That(selected.player.gold,Is.EqualTo(before.player.gold));
                        }
                    }finally{UnityEngine.Object.Destroy(root);}yield return null;
                }
                foreach(int round in new[]{23,24,25,27,28}){
                    var s=Fixture(round-1);Drain(s);Assert.That(s.SelectRound(1,round).Committed,Is.True);var root=new GameObject("GranaryBoard",typeof(RectTransform),typeof(Canvas));
                    try{var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>s,()=>{});yield return null;ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;Capture(root,"Granary_Round"+round);}finally{UnityEngine.Object.Destroy(root);}yield return null;
                }
            }finally{DemoFeatureState.SetLanguage(language);if(Directory.Exists(directory))Directory.Delete(directory,true);}
        }
    }
}
