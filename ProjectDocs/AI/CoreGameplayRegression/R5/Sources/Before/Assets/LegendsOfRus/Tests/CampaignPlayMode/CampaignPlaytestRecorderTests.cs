using System;
using System.IO;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignPlaytestRecorderTests
    {
        GameObject root;string directory;
        [SetUp] public void Prepare(){directory=Path.Combine(Path.GetTempPath(),"CampaignRecorder-"+Guid.NewGuid().ToString("N"));}
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;if(Directory.Exists(directory))Directory.Delete(directory,true);else if(File.Exists(directory))File.Delete(directory);}
        CampaignDevelopmentUI Open(out CampaignSaveSession s,out CampaignPlaytestRecorder r,string folder=null)
        {
            root=new GameObject("CampaignRecordingTest",typeof(RectTransform),typeof(Canvas));s=CampaignBoardIntegrationTests.Session();var save=s;
            var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,s,()=>save,()=>{});
            r=CampaignPlaytestRecorder.Attach(ui,s,"CampaignSector01R9-test-fingerprint",folder??directory,true);
            int steps=0;while(ui.VisiblePage=="Dialogue"){Assert.That(++steps,Is.LessThan(20));ui.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();}
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();Assert.That(ui.Board,Is.Not.Null);return ui;
        }
        static MechanicsPlaytestSession Read(string path)=>JsonUtility.FromJson<MechanicsPlaytestSession>(File.ReadAllText(path));
        [UnityTest] public IEnumerator VictoryRecordsBeforeBoardDestructionAndRatingsSurviveReopen()
        {
            var ui=Open(out var s,out var r);yield return null;string path=r.CheckpointPath;
            var initial=Read(path);Assert.That(initial.source,Is.EqualTo("automated_campaign"));Assert.That(initial.activeAttempts.Length,Is.EqualTo(1));Assert.That(initial.activeAttempts[0].round,Is.EqualTo(1));Assert.That(initial.activeAttempts[0].committedAttempt,Is.EqualTo(1));Assert.That(initial.activeAttempts[0].campaignRun,Is.EqualTo(1));Assert.That(initial.activeAttempts[0].levelFingerprint.Length,Is.EqualTo(64));
            int actions=0;
            while(ui.Board){var board=ui.Board;Assert.That(++actions,Is.LessThan(29));var p=CampaignBoardIntegrationTests.Choose(board,true);Assert.That(p,Is.Not.Null);
                Assert.That(board.SwipeCell(p.PrimaryPosition.X,p.PrimaryPosition.Y,new Vector2Int(p.SecondaryPosition.X-p.PrimaryPosition.X,p.SecondaryPosition.Y-p.PrimaryPosition.Y)),Is.True);
                float end=Time.realtimeSinceStartup+20;while(ui.Board&&ui.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;Assert.That(Time.realtimeSinceStartup,Is.LessThan(end));yield return null;
            }
            Assert.That(ui.VisiblePage,Is.EqualTo("Victory"));var completed=Read(path);Assert.That(completed.attempts.Count,Is.EqualTo(1));var a=completed.attempts[0];Assert.That(a.outcome,Is.EqualTo("won"));Assert.That(a.movesRemaining,Is.LessThan(a.moveBudget));Assert.That(a.goalsRemaining.All(x=>x==0),Is.True);Assert.That(a.inventoryBoostersUsed,Is.Zero);Assert.That(a.observedHash,Is.Not.EqualTo(a.openingHash));Assert.That(completed.activeAttempts,Is.Empty);
            r.Rate("difficulty","fair");r.Rate("specials","clear");Assert.That(Read(path).attempts[0].specialClarity,Is.EqualTo("clear"));
            string evidence=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(!string.IsNullOrEmpty(evidence)){Directory.CreateDirectory(evidence);File.Copy(path,Path.Combine(evidence,"AutomatedRecorderExample.json"),true);}
            s.TryLoad(out var before,out _);r.SaveNow();r.Interrupt("duplicate");s.TryLoad(out var after,out _);Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(before)));
            UnityEngine.Object.Destroy(root);yield return null;var reopened=Read(path);Assert.That(reopened.attempts.Count,Is.EqualTo(1));Assert.That(reopened.attempts[0].outcome,Is.EqualTo("won"));Assert.That(reopened.attempts[0].difficulty,Is.EqualTo("fair"));
        }
        [UnityTest] public IEnumerator RetryAndExitRemainSeparateInterruptedAttemptsAndPauseDoesNotCount()
        {
            var ui=Open(out var s,out var r);yield return null;Assert.That(ui.Board.TryPause(),Is.True);double active=r.Log.Current.activeSeconds;yield return new WaitForSecondsRealtime(.1f);Assert.That(r.Log.Current.activeSeconds,Is.EqualTo(active));ui.Board.TryResume();
            Assert.That(ui.Coordinator.Start(1,DateTime.UtcNow.Ticks,true),Is.True);yield return null;
            string path=r.CheckpointPath;var snapshot=Read(path);Assert.That(snapshot.attempts.Count,Is.EqualTo(1));Assert.That(snapshot.attempts[0].outcome,Is.EqualTo("interrupted"));Assert.That(snapshot.activeAttempts[0].committedAttempt,Is.EqualTo(2));Assert.That(snapshot.activeAttempts[0].campaignRun,Is.EqualTo(2));
            UnityEngine.Object.Destroy(root);yield return null;snapshot=Read(path);Assert.That(snapshot.attempts.Count,Is.EqualTo(2));Assert.That(snapshot.attempts.All(a=>a.outcome=="interrupted"),Is.True);Assert.That(snapshot.activeAttempts,Is.Empty);
        }
        [UnityTest] public IEnumerator RecordingWriteFailureDoesNotBlockGameplayAndSaveCanRetry()
        {
            File.WriteAllText(directory,"fixture blocks directory creation");var ui=Open(out var s,out var r);yield return null;Assert.That(r.SaveStatus,Does.Contain("failed"));Assert.That(ui.Board.CanAcceptPointerInput,Is.True);s.TryLoad(out var before,out _);
            File.Delete(directory);Assert.That(r.SaveNow(),Is.True);Assert.That(File.Exists(r.CheckpointPath),Is.True);Assert.That(Read(r.CheckpointPath).activeAttempts.Length,Is.EqualTo(1));
            s.TryLoad(out var after,out _);Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(before)));
        }
        [Test] public void DisposableThirtyEnergyPresetAndEmptyStockSurviveCampaignImport()
        {
            var s=CampaignBoardIntegrationTests.Session(initialize:false);
            Assert.That(s.ImportLegacyCopy(JsonUtility.ToJson(new PlayerSaveData{energy=30,energyMax=30,
                boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0,lastEnergyUtcTicks=DateTime.UtcNow.Ticks})).Committed,Is.True);
            Assert.That(s.StartRound(0,DateTime.UtcNow.Ticks).Committed,Is.True);s.TryLoad(out var data,out _);
            Assert.That(data.player.energyMax,Is.EqualTo(30));Assert.That(data.player.energy,Is.EqualTo(29));
            Assert.That(data.player.boosterMace+data.player.boosterFirebird+data.player.boosterLivingLight+data.player.boosterSeaPearl,Is.Zero);
        }
        [Test] public void ManualAndAutomatedSourcesStayDistinctAndSandboxContractStaysDefault()
        {
            Assert.That(new MechanicsPlaytestLog(false,"1","candidate-hash",true).Source,Is.EqualTo("manual_campaign"));
            Assert.That(new MechanicsPlaytestLog(true,"1","candidate-hash",true).Source,Is.EqualTo("automated_campaign"));
            Assert.That(new MechanicsPlaytestLog(false,"1").Source,Is.EqualTo("manual_sandbox"));
            Assert.That(new MechanicsPlaytestLog(true,"1").Source,Is.EqualTo("automated_test"));
        }
    }
}
