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
    public sealed class CampaignSectorCandidateTests
    {
        static string Read(string name)=>File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01",name+".json"));
        static CampaignDefinition Definition()=>JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(Path.Combine(Application.dataPath,"../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
        static ValidatedCampaignCatalog Catalog()
        {
            var c=new ValidatedCampaignCatalog(Definition());Assert.That(c.TryAdd(new CampaignPackageInput{ChapterId=1,ContentRevision=1,LevelsJson=Read("levels"),StoryJson=Read("story"),RussianJson=Read("story_ru"),EnglishJson=Read("story_en")},out var errors),Is.True,string.Join("\n",errors));return c;
        }
        static LevelData Level(int n)=>Catalog().LoadChapter(1).levels[n-1];
        static BoardModel Start(LevelData l,int attempt)=>LevelBoardFactory.Create(l,BoardSeedUtility.CreateRuntimeSeed(1,l.levelId,l.boardSeed+attempt),rules:Match3Board.CurrentGameplayRules);
        [Test] public void CandidatePreservesLegacyBoardsRewardsAndKeepsBothFinaleBoundaries()
        {
            var c=Catalog();var levels=c.LoadChapter(1);Assert.That(levels.levels.Length,Is.EqualTo(20));var legacy=JsonUtility.FromJson<ChapterLevelCatalogData>(Resources.Load<TextAsset>("Config/chapter01_levels").text);
            for(int i=0;i<10;i++){
                var a=JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(levels.levels[i]));var b=legacy.levels[i];
                a.storyTriggers=b.storyTriggers;a.displayName=b.displayName;a.difficulty=b.difficulty;
                Assert.That(JsonUtility.ToJson(a),Is.EqualTo(JsonUtility.ToJson(b)),"Historical board/reward "+(i+1));
            }
            var s=c.LoadStory(1);Assert.That(s.scenes.Any(x=>x.id=="chapter01.finale"),Is.False);Assert.That(s.completion==null||s.completion.finalLevelId==0,Is.True);
            Assert.That(s.milestones.Single(m=>m.afterLevelId==10).sceneId,Is.EqualTo("campaign.chapter01.hearth_secured"));
            Assert.That(s.milestones.Single(m=>m.afterLevelId==20).sceneId,Is.EqualTo("campaign.chapter01.well_cleared"));
            var legacyStory=JsonUtility.FromJson<ChapterStoryCatalogData>(Resources.Load<TextAsset>("Config/chapter01_story").text);
            Assert.That(legacyStory.completion.finalLevelId,Is.EqualTo(10));
            Assert.That(ActiveCampaignContent.Load().LoadStory(1).completion.finalLevelId,Is.EqualTo(500),"Historical candidate checks must not replace the installed finale.");
            var progress=c.Progression(Enumerable.Range(1,20).Select(n=>new CampaignProgressRecord{ChapterId=1,RoundId=n,Completions=1}),Array.Empty<int>());
            Assert.That(progress.Availability(1,21),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable));Assert.That(progress.Availability(2,1),Is.Not.EqualTo(CampaignRoundAvailability.Playable));
        }
        [Test] public void AllTwentyRoundsHaveDeterministicPlayableStartsAndTeachingOpportunities()
        {
            foreach(var l in Catalog().LoadChapter(1).levels)for(int attempt=1;attempt<=3;attempt++){
                var m=Start(l,attempt);Assert.That(m.HasAnyMatches(),Is.False,l.id);Assert.That(m.HasLegalMove(),Is.True,l.id);
                Assert.That(Start(l,attempt).ComputeStateHash(),Is.EqualTo(m.ComputeStateHash()));
                if(l.levelId==12||l.levelId==18){var p=new BoardResolutionPlanner(m,null,Match3Board.CurrentGameplayRules).PlanSwap(new BoardPosition(3,2),new BoardPosition(3,3));
                    Assert.That(p.IsResolved,Is.True);var kinds=p.Waves.SelectMany(w=>w.Creations).Select(x=>x.Kind).ToArray();
                    Assert.That(kinds.Any(k=>l.levelId==12?(k==SpecialGemKind.Row||k==SpecialGemKind.Column):k==SpecialGemKind.Burst),Is.True,"Authored teaching move "+l.id+" attempt "+attempt+": "+string.Join(",",kinds));}
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
        [TestCase(11)][TestCase(12)][TestCase(13)][TestCase(14)][TestCase(15)][TestCase(16)][TestCase(17)][TestCase(18)][TestCase(19)][TestCase(20)]
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
            var s=new CampaignSaveSession(new Store(),Definition(),Catalog());var p=new PlayerSaveData{energy=5,lastEnergyUtcTicks=DateTime.UtcNow.Ticks,selectedLevel=Math.Max(1,Math.Min(10,completed))};
            p.levelProgress=Enumerable.Range(1,Math.Min(10,completed)).Select(n=>new LevelProgressData{chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=1}).ToList();Assert.That(s.ImportLegacyCopy(JsonUtility.ToJson(p)).Committed,Is.True);
            // Only 1–10 are legacy data. Later completed rounds are a declared capture fixture,
            // produced through real campaign transactions, never invented legacy-format records.
            for(int n=11;n<=completed;n++){
                Assert.That(s.SelectRound(1,n).Committed,Is.True);
                Assert.That(s.StartRound(n-11,DateTime.UtcNow.AddHours(n).Ticks).Committed,Is.True);
                var l=s.ResolveSelectedRound();Assert.That(s.FinishRound(n-10,true,3000,l.goals.Select(g=>g.target).ToArray()).Committed,Is.True);
                Assert.That(s.AcknowledgeResult(n-10).Committed,Is.True);
            }
            return s;
        }
        static void Drain(CampaignSaveSession s){int guard=0;while(s.TryGetPendingStory(out var p,out _)&&p!=null){Assert.That(++guard,Is.LessThan(20));Assert.That(s.AcknowledgeStory(p.Key).Committed,Is.True);}}
        [Test] public void RoundTenHandoffDoesNotRegrantOrUnlockFinaleAndTwentyStopsAtContentBoundary()
        {
            var s=Fixture(9);Drain(s);Assert.That(s.SelectRound(1,10).Committed,Is.True);
            Assert.That(s.StartRound(0,DateTime.UtcNow.Ticks).Committed,Is.True);
            var ten=s.ResolveSelectedRound();Assert.That(s.FinishRound(1,true,3000,ten.goals.Select(g=>g.target).ToArray()).Committed,Is.True);
            Assert.That(s.TryGetPendingStory(out var pending,out _),Is.True);Assert.That(pending,Is.Null,"Unacknowledged result takes priority");
            s.TryLoad(out var before,out _);Assert.That(s.AcknowledgeResult(1,true).Committed,Is.True);
            Assert.That(s.TryGetPendingStory(out pending,out _),Is.True);Assert.That(pending.Key,Does.EndWith("hearth_secured"));
            Assert.That(s.AcknowledgeStory(pending.Key).Committed,Is.True);s.TryLoad(out var after,out _);
            Assert.That(after.player.selectedLevel,Is.EqualTo(11));Assert.That(after.player.gold,Is.EqualTo(before.player.gold));Assert.That(after.player.energy,Is.EqualTo(before.player.energy));Assert.That(after.player.acknowledgedChapterCompletions,Is.Empty);
            Assert.That(s.StartRound(1,DateTime.UtcNow.Ticks).Committed,Is.True);var l=s.ResolveSelectedRound();Assert.That(s.FinishRound(2,true,2000,l.goals.Select(g=>g.target).ToArray()).Committed,Is.True);s.TryLoad(out var won,out _);Assert.That(s.FinishRound(2,true,2000,l.goals.Select(g=>g.target).ToArray()).NoOp,Is.True);s.TryLoad(out var duplicate,out _);Assert.That(duplicate.player.gold,Is.EqualTo(won.player.gold));
            s=Fixture(20);Drain(s);Assert.That(s.SelectRound(1,21).Committed,Is.False);Assert.That(s.Availability(2,1),Is.Not.EqualTo(CampaignRoundAvailability.Playable));
        }
        static void Capture(GameObject root,string name)=>typeof(CampaignUIIntegrationTests).GetMethod("Capture",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{root,name,576,1280});
        [UnityTest,Timeout(300000)] public IEnumerator CandidateBoardsAndStoryRenderAndRoundElevenWins()
        {
            string language=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("en");
            try{
                foreach(int completed in new[]{10,20}){
                    var s=Fixture(completed);while(s.TryGetPendingStory(out var pending,out _)&&pending!=null&&!pending.Key.EndsWith(completed==10?"hearth_secured":"well_cleared"))s.AcknowledgeStory(pending.Key);
                    var root=new GameObject("SectorStory",typeof(RectTransform),typeof(Canvas));
                    try{var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>s,()=>{});yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Capture(root,"Sector_Story"+completed+"_EN");DemoFeatureState.SetLanguage("ru");ui.GetComponentInChildren<ProductionDialogueView>().Refresh();Capture(root,"Sector_Story"+completed+"_RU");DemoFeatureState.SetLanguage("en");}finally{UnityEngine.Object.Destroy(root);}yield return null;
                }
                foreach(int round in new[]{11,15,18,20}){
                    var s=Fixture(round-1);Drain(s);Assert.That(s.SelectRound(1,round).Committed,Is.True);var root=new GameObject("SectorBoard",typeof(RectTransform),typeof(Canvas));
                    try{var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>s,()=>{});yield return null;ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;Assert.That(ui.Board,Is.Not.Null);Capture(root,"Sector_Round"+round);
                        if(round==11){var w=Solve(Level(11),1);foreach(var a in w.commands){var board=ui.Board;Assert.That(board.BoardReplayToken,Is.EqualTo(a.BeforeToken));Assert.That(a.Tap?board.TapCell(a.X,a.Y):board.SwipeCell(a.X,a.Y,new Vector2Int(a.ToX-a.X,a.ToY-a.Y)),Is.True);float end=Time.realtimeSinceStartup+20;while(ui.Board&&ui.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(Time.realtimeSinceStartup,Is.LessThan(end));yield return null;}
                            Assert.That(ui.VisiblePage,Is.EqualTo("Victory"));Capture(root,"Sector_Round11_Victory");ui.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));s.TryLoad(out var saved,out _);Assert.That(saved.player.selectedLevel,Is.EqualTo(12));Assert.That(saved.player.energy,Is.EqualTo(4));}
                    }finally{UnityEngine.Object.Destroy(root);}yield return null;
                }
            }finally{DemoFeatureState.SetLanguage(language);}
        }
    }
}
