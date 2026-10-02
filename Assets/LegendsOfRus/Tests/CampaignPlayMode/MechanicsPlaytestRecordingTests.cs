using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class MechanicsPlaytestRecordingTests
    {
        GameObject root;
        string folder;
        [SetUp] public void Setup(){folder=Path.Combine(Path.GetTempPath(),"lor-recording-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);}
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;if(Directory.Exists(folder))Directory.Delete(folder,true);}
        static MechanicsPlaytestSession Data(MechanicsPlaytestLog log)=>JsonUtility.FromJson<MechanicsPlaytestSession>(log.ToJson());
        static void Begin(MechanicsPlaytestLog log)=>log.Begin(1,"Test",4,4,"opening",new[]{2});
        [Test] public void TerminalRecordsAreIdempotentAndRatingsRemainExplicit()
        {
            var log=new MechanicsPlaytestLog(true,"test");Begin(log);log.Tick(2,true);log.Tick(20,false);log.Tick(double.NaN,true);log.Observe(2,100,"Completed","final",new[]{0},true);log.Finish("won","goals_complete");log.Tick(20,true);log.Finish("interrupted","retry");
            Assert.That(Data(log).activeAttempts,Is.Empty);Assert.That(Data(log).attempts.Count,Is.EqualTo(1));Assert.That(log.Current.activeSeconds,Is.EqualTo(2));Assert.That(log.Current.outcome,Is.EqualTo("won"));Assert.That(log.Current.difficulty,Is.EqualTo("unrated"));log.Rate("difficulty","fair");Assert.That(Data(log).attempts[0].difficulty,Is.EqualTo("fair"));Assert.Throws<ArgumentException>(()=>log.Rate("goals","invented"));
            Begin(log);Assert.That(log.Current.difficulty,Is.EqualTo("unrated"));Assert.That(log.Current.number,Is.EqualTo(2));Assert.That(Data(log).activeAttempts.Single().number,Is.EqualTo(2));
        }
        [Test] public void ExportPreservesActiveAttemptAndCreatesDistinctValidFiles()
        {
            var log=new MechanicsPlaytestLog(true,"test");Begin(log);log.Observe(3,50,"Falling","partial",new[]{1},false);string first=log.Export(folder),second=log.Export(folder);Assert.That(first,Is.Not.EqualTo(second));var data=JsonUtility.FromJson<MechanicsPlaytestSession>(File.ReadAllText(first));Assert.That(data.attempts,Is.Empty);Assert.That(data.activeAttempts.Single().outcome,Is.EqualTo("ongoing"));Assert.That(data.activeAttempts.Single().snapshotSettled,Is.False);Assert.That(data.source,Is.EqualTo("automated_test"));Assert.That(Directory.GetFiles(folder,"*.tmp"),Is.Empty);Assert.That(log.Current.outcome,Is.EqualTo("ongoing"));
            string blocker=Path.Combine(folder,"blocked");File.WriteAllText(blocker,"keep");Assert.Throws<IOException>(()=>log.Export(blocker));Assert.That(File.ReadAllText(blocker),Is.EqualTo("keep"));Assert.That(log.Current.number,Is.EqualTo(1));
        }
        [Test] public void RetentionCapReportsDiscardedRecordsWithoutReusingNumbers()
        {
            var log=new MechanicsPlaytestLog(true,"test");for(int i=0;i<MechanicsPlaytestLog.Capacity+3;i++){Begin(log);log.Finish("interrupted","retry");}var data=Data(log);Assert.That(data.attempts.Count,Is.EqualTo(MechanicsPlaytestLog.Capacity));Assert.That(data.droppedAttempts,Is.EqualTo(3));Assert.That(data.attempts[0].number,Is.EqualTo(4));Assert.That(data.attempts.Last().number,Is.EqualTo(203));
        }
        static IEnumerator Settle(Match3Board b){float end=Time.realtimeSinceStartup+25;while(b.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(b.HasActiveCommand,Is.False);yield return null;}
        void Click(string name){root.transform.Find("MechanicsSandbox/PlaytestFeedback/"+name).GetComponent<Button>().onClick.Invoke();}
        static string Evidence {get {var p=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(string.IsNullOrEmpty(p))p=Path.Combine(Application.temporaryCachePath,"RecordingEvidence");Directory.CreateDirectory(p);return p;}}
        [UnityTest] public IEnumerator RuntimeWinFeedbackInterruptionsAndExportAreAccurate()
        {
            root=new GameObject("RecordingCapture",typeof(RectTransform),typeof(Canvas));var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);yield return null;var log=s.AttemptLog;
            Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);yield return Settle(s.Board);Assert.That(s.Board.TapCell(3,2),Is.True);yield return Settle(s.Board);
            Assert.That(log.Current.outcome,Is.EqualTo("won"));Assert.That(log.Current.movesRemaining,Is.EqualTo(2));Assert.That(log.Current.goalsRemaining,Is.All.EqualTo(0));Assert.That(log.Current.snapshotSettled,Is.True);
            Click("Difficulty");Click("GoalClarity");Click("GoalClarity");Click("EffectClarity");Click("EffectClarity");Assert.That(log.Current.difficulty,Is.EqualTo("easy"));Assert.That(log.Current.goalClarity,Is.EqualTo("unclear"));Assert.That(log.Current.effectClarity,Is.EqualTo("busy"));
            Assert.That(s.ExportPlaytest(folder),Is.True);Assert.That(File.Exists(s.LastExportPath),Is.True);yield return null;MechanicsSandboxPlayModeTests.Capture(root,"Recording_Win",576,1280);
            string blocker=Path.Combine(folder,"blocked");File.WriteAllText(blocker,"keep");Assert.That(s.ExportPlaytest(blocker),Is.False);Assert.That(s.ExportStatus,Does.Contain("failed"));Assert.That(log.CompletedCount,Is.EqualTo(1));
            s.OpenRound(0);yield return null;Assert.That(log.Current.number,Is.EqualTo(2));Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);Assert.That(s.Board.HasActiveCommand,Is.True);s.Reset(0,GameplayRuleVersion.Legacy);yield return null;
            var data=Data(log);Assert.That(data.attempts.Count,Is.EqualTo(2));Assert.That(data.attempts[1].outcome,Is.EqualTo("interrupted"));Assert.That(data.attempts[1].endReason,Is.EqualTo("fixtures"));Assert.That(data.attempts[1].snapshotSettled,Is.False);
            s.OpenRound(2);yield return null;s.OpenRound(3);yield return null;Assert.That(log.CompletedCount,Is.EqualTo(3));Assert.That(Data(log).attempts[2].endReason,Is.EqualTo("next_round"));Assert.That(s.ExportPlaytest(folder),Is.True);File.Copy(s.LastExportPath,Path.Combine(Evidence,"Recording_Active.json"),true);
            var exit=root.GetComponentsInChildren<Text>().Single(t=>t.text=="Exit").GetComponent<Button>();exit.onClick.Invoke();Assert.That(log.CompletedCount,Is.EqualTo(4));Assert.That(log.Current.endReason,Is.EqualTo("exit"));yield return null;Assert.That(log.CompletedCount,Is.EqualTo(4));File.WriteAllText(Path.Combine(Evidence,"Recording_Completed.json"),log.ToJson());
        }
        [Serializable] class Witness {public MechanicsReplayCommand[] commands;}
        [UnityTest] public IEnumerator RuntimeFailureRecordedOnceAndRetryStartsUnratedAttempt()
        {
            root=new GameObject("RecordingFailure",typeof(RectTransform),typeof(Canvas));var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);s.OpenRound(4,GameplayRuleVersion.PrototypeR3);yield return null;
            var path=Path.GetFullPath(Path.Combine(Application.dataPath,"../ProjectDocs/AI/GameplayMechanics/R4/QA/Witnesses/Round5_LosingWitness.json"));var witness=JsonUtility.FromJson<Witness>(File.ReadAllText(path));
            foreach(var a in witness.commands){Assert.That(a.Tap?s.Board.TapCell(a.X,a.Y):s.Board.SwipeCell(a.X,a.Y,new Vector2Int(a.ToX-a.X,a.ToY-a.Y)),Is.True);yield return Settle(s.Board);}
            var log=s.AttemptLog;Assert.That(log.Current.outcome,Is.EqualTo("lost"));Assert.That(log.Current.movesRemaining,Is.Zero);Assert.That(log.Current.goalsRemaining.Any(g=>g>0),Is.True);Assert.That(log.Current.difficulty,Is.EqualTo("unrated"));yield return null;MechanicsSandboxPlayModeTests.Capture(root,"Recording_Loss",576,1280);
            s.OpenRound(4,GameplayRuleVersion.PrototypeR3);yield return null;Assert.That(log.CompletedCount,Is.EqualTo(2));Assert.That(log.Current.number,Is.EqualTo(3));Assert.That(log.Current.outcome,Is.EqualTo("ongoing"));Assert.That(log.Current.movesRemaining,Is.EqualTo(10));Assert.That(log.Current.difficulty,Is.EqualTo("unrated"));Assert.That(s.ExportPlaytest(folder),Is.True);File.Copy(s.LastExportPath,Path.Combine(Evidence,"Recording_Loss.json"),true);
        }
    }
}
