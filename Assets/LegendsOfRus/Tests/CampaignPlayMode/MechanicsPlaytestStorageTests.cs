using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class MechanicsPlaytestStorageTests
    {
        string folder;
        GameObject root;
        [SetUp] public void Setup(){folder=Path.Combine(Path.GetTempPath(),"lor-playtest-storage-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);}
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;Directory.Delete(folder,true);}
        static MechanicsPlaytestLog Log(bool automated=true){var log=new MechanicsPlaytestLog(automated,"test");log.Begin(1,"Test",4,4,"opening",new[]{2});return log;}
        static MechanicsPlaytestSession Read(string path)=>JsonUtility.FromJson<MechanicsPlaytestSession>(File.ReadAllText(path));
        [Test] public void CheckpointsReplaceAtomicallyKeepPreviousAndSkipUnchangedWrites()
        {
            var log=Log();var store=new MechanicsPlaytestStorage(log,folder);store.Save();string first=File.ReadAllText(store.CheckpointPath);
            log.Rate("difficulty","fair");store.Save();Assert.That(File.ReadAllText(store.CheckpointPath+".bak"),Is.EqualTo(first));
            string second=File.ReadAllText(store.CheckpointPath);log.Rate("goals","clear");store.Save();Assert.That(File.ReadAllText(store.CheckpointPath+".bak"),Is.EqualTo(second));
            var writeTime=File.GetLastWriteTimeUtc(store.CheckpointPath);store.Save();Assert.That(File.GetLastWriteTimeUtc(store.CheckpointPath),Is.EqualTo(writeTime));Assert.That(File.ReadAllText(store.CheckpointPath+".bak"),Is.EqualTo(second));
            Assert.That(Directory.GetFiles(Path.GetDirectoryName(store.CheckpointPath)).Length,Is.EqualTo(2));Assert.That(Read(store.CheckpointPath).activeAttempts.Single().goalClarity,Is.EqualTo("clear"));
        }
        [Test] public void BlockedReplacementRetainsLastGoodFileAndCanRetry()
        {
            var log=Log();var store=new MechanicsPlaytestStorage(log,folder);store.Save();string first=File.ReadAllText(store.CheckpointPath);
            // Force the commit step to fail after the temporary file has been written.
            Directory.CreateDirectory(store.CheckpointPath+".bak");log.Finish("lost","moves_exhausted");
            Assert.Throws<IOException>(()=>store.Save());Assert.That(File.ReadAllText(store.CheckpointPath),Is.EqualTo(first));Assert.That(File.Exists(store.CheckpointPath+".tmp"),Is.False);
            Directory.Delete(store.CheckpointPath+".bak");store.Save();Assert.That(Read(store.CheckpointPath).attempts.Single().outcome,Is.EqualTo("lost"));
        }
        [Test] public void NewSessionsNeverOverwritePriorManualOrAutomatedRecords()
        {
            var manual=Log(false);var automated=Log();var next=Log(false);
            var a=new MechanicsPlaytestStorage(manual,folder);var b=new MechanicsPlaytestStorage(automated,folder);var c=new MechanicsPlaytestStorage(next,folder);
            a.Save();b.Save();c.Save();Assert.That(new[]{a.CheckpointPath,b.CheckpointPath,c.CheckpointPath}.Distinct().Count(),Is.EqualTo(3));
            Assert.That(Path.GetDirectoryName(a.CheckpointPath),Does.EndWith("manual_sandbox"));Assert.That(Path.GetDirectoryName(b.CheckpointPath),Does.EndWith("automated_test"));
            // Simulated process restart: disk alone supplies the earlier ongoing snapshot.
            var restored=Read(a.CheckpointPath);Assert.That(restored.activeAttempts.Single().outcome,Is.EqualTo("ongoing"));Assert.That(restored.attempts,Is.Empty);Assert.That(restored.sessionId,Is.EqualTo(manual.SessionId));
            Assert.That(restored.activeAttempts.Single().difficulty,Is.EqualTo("unrated"));
        }
        MechanicsSandbox Open(bool automated=true){if(!root)root=new GameObject("StorageCapture",typeof(RectTransform),typeof(Canvas));return MechanicsSandbox.Open((RectTransform)root.transform,()=>{},automated,folder);}
        static IEnumerator Settle(Match3Board board){float end=Time.realtimeSinceStartup+25;while(board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(board.HasActiveCommand,Is.False);yield return null;}
        static string Evidence {get {string p=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR")??Path.Combine(Application.temporaryCachePath,"StorageEvidence");Directory.CreateDirectory(p);return p;}}
        [UnityTest] public IEnumerator WinAndFeedbackSurviveExitWithoutExplicitExport()
        {
            var s=Open();yield return null;string path=s.CheckpointPath;Assert.That(Read(path).activeAttempts.Single().movesRemaining,Is.EqualTo(4));
            Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);yield return Settle(s.Board);Assert.That(Read(path).activeAttempts.Single().movesRemaining,Is.EqualTo(3));
            Assert.That(s.Board.TapCell(3,2),Is.True);yield return Settle(s.Board);Assert.That(Read(path).attempts.Single().outcome,Is.EqualTo("won"));
            root.transform.Find("MechanicsSandbox/PlaytestFeedback/Difficulty").GetComponent<Button>().onClick.Invoke();Assert.That(Read(path).attempts.Single().difficulty,Is.EqualTo("easy"));
            Assert.That(s.LastExportPath,Is.Null);Assert.That(s.SaveStatus,Is.EqualTo("Saved locally"));
            // Capture the settled feedback UI after the existing impact animation expires.
            yield return new WaitForSecondsRealtime(.6f);MechanicsSandboxPlayModeTests.Capture(root,"Storage_Win",576,1280);
            root.GetComponentsInChildren<Text>().Single(t=>t.text=="Exit").GetComponent<Button>().onClick.Invoke();yield return null;
            var data=Read(path);Assert.That(data.attempts.Count,Is.EqualTo(1));Assert.That(data.attempts[0].outcome,Is.EqualTo("won"));Assert.That(data.activeAttempts,Is.Empty);
            File.Copy(path,Path.Combine(Evidence,"Storage_Win.json"),true);
            var next=Open();yield return null;Assert.That(next.CheckpointPath,Is.Not.EqualTo(path));Assert.That(Read(path).attempts.Single().difficulty,Is.EqualTo("easy"));Assert.That(Read(next.CheckpointPath).activeAttempts.Single().number,Is.EqualTo(1));
        }
        [UnityTest] public IEnumerator BackgroundAndShutdownPreserveOngoingAndInterruptedSeparately()
        {
            var s=Open();yield return null;string path=s.CheckpointPath;s.AttemptLog.Tick(3,true);s.SendMessage("OnApplicationFocus",false);
            var active=Read(path).activeAttempts.Single();Assert.That(active.activeSeconds,Is.GreaterThanOrEqualTo(3));Assert.That(active.outcome,Is.EqualTo("ongoing"));
            double seconds=active.activeSeconds;yield return new WaitForSeconds(.1f);s.SendMessage("OnApplicationPause",true);Assert.That(Read(path).activeAttempts.Single().activeSeconds,Is.EqualTo(seconds));
            Assert.That(s.Board.SwipeCell(3,3,Vector2Int.down),Is.True);s.gameObject.SetActive(false);yield return null;
            var ended=Read(path);Assert.That(ended.activeAttempts,Is.Empty);Assert.That(ended.attempts.Single().outcome,Is.EqualTo("interrupted"));Assert.That(ended.attempts[0].endReason,Is.EqualTo("disabled"));Assert.That(ended.attempts[0].snapshotSettled,Is.False);
            UnityEngine.Object.Destroy(s.gameObject);yield return null;Assert.That(Read(path).attempts.Count,Is.EqualTo(1));File.Copy(path,Path.Combine(Evidence,"Storage_Interrupted.json"),true);
            s=Open();yield return null;path=s.CheckpointPath;s.SendMessage("OnApplicationQuit");UnityEngine.Object.Destroy(s.gameObject);yield return null;Assert.That(Read(path).attempts.Single().endReason,Is.EqualTo("application_quit"));
        }
        [UnityTest] public IEnumerator SaveFailureIsVisibleRetryableAndBatchCannotLookManual()
        {
            var s=Open(false);yield return null;string path=s.CheckpointPath;
            if(Application.isBatchMode)Assert.That(Read(path).source,Is.EqualTo("automated_test"));
            string lastGood=File.ReadAllText(path);Directory.CreateDirectory(path+".tmp");s.RatePlaytest("effects","busy");
            Assert.That(s.SaveStatus,Does.Contain("failed"));Assert.That(File.ReadAllText(path),Is.EqualTo(lastGood));yield return null;
            Assert.That(root.transform.Find("MechanicsSandbox/PlaytestFeedback/AttemptSummary").GetComponent<Text>().text,Does.Contain("failed"));MechanicsSandboxPlayModeTests.Capture(root,"Storage_Error",576,1280);
            Directory.Delete(path+".tmp");root.transform.Find("MechanicsSandbox/PlaytestFeedback/Save").GetComponent<Button>().onClick.Invoke();Assert.That(s.SaveStatus,Is.EqualTo("Saved locally"));Assert.That(Read(path).activeAttempts.Single().effectClarity,Is.EqualTo("busy"));
            // The explicit export remains an immutable snapshot while checkpoints change.
            Assert.That(s.ExportPlaytest(folder),Is.True);string exported=File.ReadAllText(s.LastExportPath);s.OpenRound(1);yield return null;
            Assert.That(File.ReadAllText(s.LastExportPath),Is.EqualTo(exported));Assert.That(Read(path).attempts.Single().endReason,Is.EqualTo("next_round"));Assert.That(Read(path).activeAttempts.Single().round,Is.EqualTo(2));
        }
    }
}
