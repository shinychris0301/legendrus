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
    public sealed class CampaignTuningPlaytestTests
    {
        GameObject root; string directory,language;
        static string Folder=>Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates");
        static string Read(string name)=>File.ReadAllText(Path.Combine(Folder,"Chapter01Sector01",name+".json"));
        static CampaignDefinition Definition()=>JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(Path.Combine(Application.dataPath,"../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
        static CampaignPackageInput Input()=>new CampaignPackageInput{ChapterId=1,ContentRevision=1,
            LevelsJson=File.ReadAllText(Path.Combine(Folder,"Chapter01Sector01R12/levels.json")),StoryJson=Read("story"),RussianJson=Read("story_ru"),EnglishJson=Read("story_en")};
        static string Fingerprint(){var i=Input();return "CampaignSector01R12-"+CampaignPlaytestRecorder.Fingerprint(i.LevelsJson+"\n"+i.StoryJson+"\n"+i.RussianJson+"\n"+i.EnglishJson);}
        static ValidatedCampaignCatalog Catalog(){var c=new ValidatedCampaignCatalog(Definition());Assert.That(c.TryAdd(Input(),out var errors),Is.True,string.Join(";",errors));return c;}
        sealed class Store:ISaveJournalStore{readonly Dictionary<string,string> data=new Dictionary<string,string>();public bool TryRead(string k,out string v)=>data.TryGetValue(k,out v);public void Write(string k,string v)=>data[k]=v;public void Delete(string k)=>data.Remove(k);public void Flush(){}}
        static CampaignSaveSession Fixture()
        {
            var s=new CampaignSaveSession(new Store(),Definition(),Catalog());
            var p=new PlayerSaveData{energy=30,energyMax=30,lastEnergyUtcTicks=DateTime.UtcNow.Ticks,selectedLevel=10,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0,
                levelProgress=Enumerable.Range(1,10).Select(n=>new LevelProgressData{chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=1}).ToList()};
            Assert.That(s.ImportLegacyCopy(JsonUtility.ToJson(p)).Committed,Is.True);
            // Declared test progress through 19 via campaign transactions; not human evidence.
            for(int n=11;n<=19;n++){Assert.That(s.SelectRound(1,n).Committed,Is.True);Assert.That(s.StartRound(n-11,DateTime.UtcNow.Ticks).Committed,Is.True);
                var l=s.ResolveSelectedRound();Assert.That(s.FinishRound(n-10,true,3000,l.goals.Select(g=>g.target).ToArray()).Committed,Is.True);Assert.That(s.AcknowledgeResult(n-10).Committed,Is.True);}
            while(s.TryGetPendingStory(out var story,out _)&&story!=null)Assert.That(s.AcknowledgeStory(story.Key).Committed,Is.True);
            Assert.That(s.SelectRound(1,20).Committed,Is.True);return s;
        }
        [SetUp] public void Prepare(){directory=Path.Combine(Path.GetTempPath(),"CampaignTuning-"+Guid.NewGuid().ToString("N"));language=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("en");}
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);DemoFeatureState.SetLanguage(language);yield return null;if(Directory.Exists(directory))Directory.Delete(directory,true);else if(File.Exists(directory))File.Delete(directory);}
        CampaignDevelopmentUI Open(CampaignSaveSession s,out CampaignPlaytestRecorder recorder,int endRound=20)
        {
            root=new GameObject("TuningCompletion",typeof(RectTransform),typeof(Canvas));
            var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>s,()=>{});
            recorder=CampaignPlaytestRecorder.Attach(ui,s,Fingerprint(),directory,true,endRound);return ui;
        }
        static void Drain(CampaignDevelopmentUI ui){int count=0;while(ui.VisiblePage=="Dialogue"){Assert.That(++count,Is.LessThan(20));ui.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();}}
        [Serializable] sealed class Witness{public MechanicsReplayCommand[] commands;}
        static Witness WitnessFor(int n,int attempt)=>JsonUtility.FromJson<Witness>(File.ReadAllText(Path.Combine(Application.dataPath,$"../ProjectDocs/AI/CampaignFoundation/R9/QA/Verify05/Candidate/Journey/Round{n}_Attempt{attempt}_Witness.json")));
        static BoardModel Start(LevelData l,int attempt)=>LevelBoardFactory.Create(l,BoardSeedUtility.CreateRuntimeSeed(1,l.levelId,l.boardSeed+attempt),rules:Match3Board.CurrentGameplayRules);
        [Test] public void OnlyFourBudgetsChangeAndAllThirtyExistingWitnessesStillComplete()
        {
            var tuned=Catalog().LoadChapter(1);var original=JsonUtility.FromJson<ChapterLevelCatalogData>(Read("levels"));
            var budgets=new Dictionary<int,int>{{13,25},{14,26},{17,26},{19,25}};
            for(int index=0;index<20;index++){
                var l=tuned.levels[index];var old=original.levels[index];Assert.That(l.moveLimit,Is.EqualTo(budgets.TryGetValue(index+1,out var limit)?limit:old.moveLimit));
                var clone=JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(l));clone.moveLimit=old.moveLimit;Assert.That(JsonUtility.ToJson(clone),Is.EqualTo(JsonUtility.ToJson(old)));
                if(index<10)continue;
                for(int attempt=1;attempt<=3;attempt++){
                    var m=Start(l,attempt);Assert.That(m.HasAnyMatches(),Is.False);Assert.That(m.HasLegalMove(),Is.True);
                    var goals=new LevelGoalRuntime(l,0);int score=0;var witness=WitnessFor(l.levelId,attempt);
                    Assert.That(witness.commands.Length,Is.LessThanOrEqualTo(l.moveLimit));
                    foreach(var a in witness.commands){Assert.That(a.BeforeToken,Is.EqualTo(m.CreateSnapshot().ToReplayToken()));var plan=a.Resolve();Assert.That(plan.IsResolved&&plan.ConsumesMove,Is.True);
                        foreach(var wave in plan.Waves){score+=wave.ScoreDelta;goals.ApplyWave(wave,score);}goals.RecordConsumedMove();m.RestoreSnapshot(plan.FinalSnapshot);}
                    Assert.That(goals.AllComplete,Is.True);
                }
            }
            Assert.That(Catalog().Progression(Enumerable.Range(1,20).Select(n=>new CampaignProgressRecord{ChapterId=1,RoundId=n,Completions=1}),Array.Empty<int>()).Availability(1,21),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable));
        }
        [UnityTest,Timeout(180000)] public IEnumerator ActualRoundTwentyWinThenStoryThenCompletionAndExplicitReplay()
        {
            var s=Fixture();var ui=Open(s,out var recorder);yield return null;
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;
            foreach(var a in WitnessFor(20,1).commands){var b=ui.Board;Assert.That(b,Is.Not.Null);Assert.That(b.BoardReplayToken,Is.EqualTo(a.BeforeToken));
                Assert.That(a.Tap?b.TapCell(a.X,a.Y):b.SwipeCell(a.X,a.Y,new Vector2Int(a.ToX-a.X,a.ToY-a.Y)),Is.True);
                float until=Time.realtimeSinceStartup+20;while(ui.Board&&ui.Board.HasActiveCommand&&Time.realtimeSinceStartup<until)yield return null;Assert.That(Time.realtimeSinceStartup,Is.LessThan(until));yield return null;}
            Assert.That(ui.VisiblePage,Is.EqualTo("Victory"));Assert.That(recorder.Log.Current.outcome,Is.EqualTo("won"));
            ui.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return null;
            Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Assert.That(ui.PlaytestComplete,Is.False);Drain(ui);yield return null;
            Assert.That(ui.PlaytestComplete,Is.True);Assert.That(ui.Board,Is.Null);Assert.That(ui.Coordinator.State,Is.EqualTo(CampaignGameplayState.Ready));
            s.TryLoad(out var completed,out _);string before=JsonUtility.ToJson(completed);
            var view=ui.GetComponentInChildren<ProductionErrorDialogView>();Assert.That(view.Support.IsInteractable(),Is.True);view.Support.onClick.Invoke();Assert.That(File.Exists(recorder.CheckpointPath),Is.True);
            Capture("PlaytestComplete_EN",941,1672);DemoFeatureState.SetLanguage("ru");yield return new WaitForSecondsRealtime(.3f);Assert.That(view.Heading.text,Is.EqualTo("ТЕСТ ЗАВЕРШЁН"));Capture("PlaytestComplete_RU",576,1280);
            Assert.That(ui.Coordinator.Recover(),Is.True);Assert.That(ui.PlaytestComplete,Is.True);s.TryLoad(out var repeated,out _);Assert.That(JsonUtility.ToJson(repeated),Is.EqualTo(before));
            Assert.That(s.SelectRound(1,21).Committed,Is.False);Assert.That(completed.player.acknowledgedChapterCompletions,Is.Empty);
            ui.ReplayCompletedPlaytest();Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));s.TryLoad(out var selected,out _);Assert.That(JsonUtility.ToJson(selected),Is.EqualTo(before),"Replay selection does not spend energy or grant anything");
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;
            Assert.That(recorder.Log.Current.committedAttempt,Is.EqualTo(2));s.TryLoad(out var replay,out _);Assert.That(replay.player.energy,Is.EqualTo(completed.player.energy-1));
            Assert.That(replay.player.gold,Is.EqualTo(completed.player.gold));Assert.That(recorder.Log.CompletedCount,Is.EqualTo(1));
        }
        void Capture(string name,int w,int h)=>typeof(CampaignUIIntegrationTests).GetMethod("Capture",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{root,name,w,h});
        [UnityTest] public IEnumerator FailedTerminalRoundAndNonRecordedPreviewDoNotShowCompletion()
        {
            var s=Fixture();Assert.That(s.StartRound(9,DateTime.UtcNow.Ticks).Committed,Is.True);Assert.That(s.FinishRound(10,false,0,new int[3]).Committed,Is.True);
            var ui=Open(s,out _);yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Fail"));Assert.That(ui.Coordinator.AcknowledgeAndReturn(10),Is.True);Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));
            UnityEngine.Object.Destroy(root);yield return null;
            s=Fixture();Assert.That(s.StartRound(9,DateTime.UtcNow.Ticks).Committed,Is.True);Assert.That(s.FinishRound(10,true,5000,s.ResolveSelectedRound().goals.Select(g=>g.target).ToArray()).Committed,Is.True);
            ui=Open(s,out _,0);Assert.That(ui.Coordinator.AcknowledgeAndReturn(10),Is.True);Drain(ui);Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));
        }
        [UnityTest] public IEnumerator CompletionExitPreservesSavedRecordingAndFailedSaveCanRetry()
        {
            var s=Fixture();Assert.That(s.StartRound(9,DateTime.UtcNow.Ticks).Committed,Is.True);Assert.That(s.FinishRound(10,true,5000,s.ResolveSelectedRound().goals.Select(g=>g.target).ToArray()).Committed,Is.True);
            var ui=Open(s,out var recorder);Assert.That(ui.Coordinator.AcknowledgeAndReturn(10),Is.True);Drain(ui);yield return null;Assert.That(ui.PlaytestComplete,Is.True);
            string path=recorder.CheckpointPath;s.TryLoad(out var before,out _);
            Directory.Delete(directory,true);File.WriteAllText(directory,"fixture blocks save");ui.ExitCompletedPlaytest();Assert.That(ui.gameObject.activeSelf,Is.True);Assert.That(recorder.SaveStatus,Does.Contain("failed"));
            File.Delete(directory);ui.GetComponentInChildren<ProductionErrorDialogView>().Support.onClick.Invoke();Assert.That(File.Exists(path),Is.True);
            ui.ExitCompletedPlaytest();yield return null;Assert.That(ui==null,Is.True);Assert.That(File.Exists(path),Is.True);s.TryLoad(out var after,out _);Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(before)));
        }
    }
}
