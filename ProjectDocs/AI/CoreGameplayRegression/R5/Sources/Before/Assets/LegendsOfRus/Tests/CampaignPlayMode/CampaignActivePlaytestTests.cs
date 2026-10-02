#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignActivePlaytestTests
    {
        GameObject root, ownedState;
        string folder, liveBefore;
        [SetUp] public void SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(isolated, Is.Not.Null.And.Not.Empty);
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar), Is.True);
            if (!GameState.Instance) ownedState = new GameObject("CollectionIsolationState", typeof(GameState));
            liveBefore = PlayerStateWithoutClockTick();
            folder = Path.Combine(Path.GetTempPath(), "ActiveCollection-"+Guid.NewGuid().ToString("N"));
        }
        [UnityTearDown] public IEnumerator TearDown()
        {
            if (root) UnityEngine.Object.Destroy(root);
            yield return null;
            string liveAfter = PlayerStateWithoutClockTick();
            if (ownedState) UnityEngine.Object.Destroy(ownedState);
            yield return null;
            if (Directory.Exists(folder)) Directory.Delete(folder,true);
            Assert.That(liveAfter, Is.EqualTo(liveBefore), "Preview must not change normal progress, balances or preferences");
        }
        static string PlayerStateWithoutClockTick()
        {
            var copy = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            // GameState's normal five-second full-energy clock update continues independently.
            // Exclude only its timestamp; energy, currencies, inventory, preferences and progress stay checked.
            copy.lastEnergyUtcTicks = 0;
            return JsonUtility.ToJson(copy);
        }
        static string Read(string name) => Resources.Load<TextAsset>("CampaignActive/"+name).text;
        static CampaignActivePlaytestSetup FromTexts(int first, string story = null) => new CampaignActivePlaytestSetup(first,
            Read("definition"),Read("levels"),story??Read("story"),Read("story_ru"),Read("story_en"));
        static MechanicsPlaytestSession Export(string path) => JsonUtility.FromJson<MechanicsPlaytestSession>(File.ReadAllText(path));

        static System.Collections.Generic.IEnumerable<int> Blocks => Enumerable.Range(0,50).Select(n=>1+10*n);
        [TestCaseSource(nameof(Blocks))]
        public void EachInstalledBlockHasIsolatedExactContentFixtureAndNoMeasuredRuns(int first)
        {
            var fixture = CampaignActivePlaytestSetup.Create(first);
            Assert.That(CampaignActivePlaytestSetup.AvailableBlocks(), Does.Contain(first));
            Assert.That(fixture.EndRound, Is.EqualTo(first+9));
            var s=fixture.Session;
            Assert.That(s.TryLoad(out var save,out _),Is.True);
            Assert.That(save.player.energy,Is.EqualTo(30));
            Assert.That(save.player.boosterMace+save.player.boosterFirebird+save.player.boosterLivingLight+save.player.boosterSeaPearl,Is.Zero);
            Assert.That(save.runSequence,Is.Zero);
            Assert.That(save.player.levelProgress.Count,Is.EqualTo(first-1));
            Assert.That(save.player.selectedLevel,Is.EqualTo(first));
            Assert.That(s.ResolveSelectedRound().levelId,Is.EqualTo(first));
            Assert.That(JsonUtility.ToJson(s.ResolveSelectedRound()),Is.EqualTo(JsonUtility.ToJson(ChapterLevelCatalog.Find(ActiveCampaignContent.Load(),1,first))));
            Assert.That(s.TryGetPendingStory(out var story,out _),Is.True);
            if(first==1) Assert.That(story.Scene.id,Is.EqualTo("chapter01.intro")); else Assert.That(story,Is.Null);
            Assert.That(fixture.Reopen().TryLoad(out var reloaded,out _),Is.True);
            Assert.That(JsonUtility.ToJson(reloaded),Is.EqualTo(JsonUtility.ToJson(save)));
        }
        [TestCase(0)][TestCase(2)][TestCase(501)] public void UnavailableOrMisalignedBlockRejected(int first) =>
            Assert.Throws<ArgumentOutOfRangeException>(()=>CampaignActivePlaytestSetup.Create(first));
        [Test] public void MalformedContentRejectedWithoutFallback() => Assert.Throws<ArgumentException>(()=>FromTexts(31,"{}"));
        [Test] public void FingerprintIncludesDefinitionAndAllFourInstalledFiles()
        {
            var a=FromTexts(31);
            Assert.That(a.Fingerprint,Is.EqualTo("CampaignActive-"+CampaignPlaytestRecorder.Fingerprint(string.Join("\n",new[]{Read("definition"),Read("levels"),Read("story"),Read("story_ru"),Read("story_en")}))));
            var b=new CampaignActivePlaytestSetup(31,Read("definition")+"\n",Read("levels"),Read("story"),Read("story_ru"),Read("story_en"));
            Assert.That(b.Fingerprint,Is.Not.EqualTo(a.Fingerprint));
        }
        CampaignDevelopmentUI Open(CampaignActivePlaytestSetup setup)
        {
            root=new GameObject("ActiveCollection",typeof(RectTransform),typeof(Canvas));
            return setup.Open((RectTransform)root.transform,()=>{},folder,true);
        }
        [UnityTest] public IEnumerator RecordedBlockUsesCurrentRulesArtAndPersistsInterruptedAttemptOnClose()
        {
            var f=CampaignActivePlaytestSetup.Create(31);var ui=Open(f);var r=ui.PlaytestRecorder;
            string path=r.CheckpointPath;var empty=Export(path);
            Assert.That(empty.attempts,Is.Empty);Assert.That(empty.activeAttempts,Is.Empty);
            Assert.That(empty.campaignStartRound,Is.EqualTo(31));Assert.That(empty.campaignEndRound,Is.EqualTo(40));
            Assert.That(empty.fixtureCompletedThrough,Is.EqualTo(30));Assert.That(empty.collectionContext,Does.Contain("disposable"));
            Assert.That(empty.contentVersion,Is.EqualTo(f.Fingerprint));
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;
            Assert.That(ui.Board.RuleVersion,Is.EqualTo(Match3Board.CurrentGameplayRules));Assert.That(ui.Board.UsesCurrentBoardArt,Is.True);
            Assert.That(r.Log.Current.committedAttempt,Is.EqualTo(1));Assert.That(r.Log.Current.campaignRun,Is.EqualTo(1));
            f.Session.TryLoad(out var before,out _);Assert.That(before.player.energy,Is.EqualTo(29));
            r.Rate("goals","clear");UnityEngine.Object.Destroy(root);yield return null;
            var saved=Export(path);Assert.That(saved.source,Is.EqualTo("automated_campaign"));Assert.That(saved.activeAttempts,Is.Empty);
            Assert.That(saved.attempts.Count,Is.EqualTo(1));Assert.That(saved.attempts[0].outcome,Is.EqualTo("interrupted"));
            Assert.That(saved.attempts[0].round,Is.EqualTo(31));Assert.That(saved.attempts[0].goalClarity,Is.EqualTo("clear"));
            Assert.That(f.Reopen().TryLoad(out var after,out _),Is.True);
            Assert.That(after.player.energy,Is.EqualTo(before.player.energy));Assert.That(after.player.gold,Is.EqualTo(before.player.gold));
        }
        // Service-level progress fixture before the final round; not player outcomes or recording evidence.
        static void AdvanceFixture(CampaignActivePlaytestSetup fixture)
        {
            var s=fixture.Session;
            for(int n=fixture.StartRound;n<fixture.EndRound;n++)
            {
                Assert.That(s.SelectRound(1,n).Committed,Is.True);s.TryLoad(out var save,out _);
                Assert.That(s.StartRound(save.runSequence,DateTime.UtcNow.Ticks).Committed,Is.True);
                var l=s.ResolveSelectedRound();s.TryLoad(out save,out _);
                Assert.That(s.FinishRound(save.runSequence,true,4000,l.goals.Select(g=>g.target).ToArray()).Committed,Is.True);
                Assert.That(s.AcknowledgeResult(save.runSequence,true).Committed,Is.True);
                while(s.TryGetPendingStory(out var story,out _)&&story!=null)Assert.That(s.AcknowledgeStory(story.Key).Committed,Is.True);
            }
        }
        [Serializable] sealed class Witness {public MechanicsReplayCommand[] commands;}
        [UnityTest,Timeout(180000)] public IEnumerator Round40StopsBefore41AndReplayStays40() {yield return Terminal(31);}
        [UnityTest,Timeout(180000)] public IEnumerator Round200StopsWithoutChapterFinaleAndReplayStays200() {yield return Terminal(191);}
        [UnityTest,Timeout(180000)] public IEnumerator Round500EndsRecordedBlockAndReplayStays500() {yield return Terminal(491);}
        IEnumerator Terminal(int first)
        {
            var f=CampaignActivePlaytestSetup.Create(first);AdvanceFixture(f);var ui=Open(f);var r=ui.PlaytestRecorder;
            Assert.That(r.Log.CompletedCount,Is.Zero);
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;
            string evidence=f.EndRound>200?"R44ChapterOneCompletion/QA/Witnesses":"R33OrchardFieldCheckpoint/QA/Verify01/Active/Journey";
            var witness=JsonUtility.FromJson<Witness>(File.ReadAllText(Path.Combine(Application.dataPath,$"../ProjectDocs/AI/CampaignFoundation/{evidence}/Round{f.EndRound}_Attempt1_Witness.json")));
            foreach(var a in witness.commands)
            {
                var board=ui.Board;Assert.That(board,Is.Not.Null);Assert.That(board.BoardReplayToken,Is.EqualTo(a.BeforeToken));
                Assert.That(a.Tap?board.TapCell(a.X,a.Y):board.SwipeCell(a.X,a.Y,new Vector2Int(a.ToX-a.X,a.ToY-a.Y)),Is.True);
                float until=Time.realtimeSinceStartup+20;while(ui.Board&&ui.Board.HasActiveCommand&&Time.realtimeSinceStartup<until)yield return null;
                Assert.That(Time.realtimeSinceStartup,Is.LessThan(until));yield return null;
            }
            Assert.That(ui.VisiblePage,Is.EqualTo("Victory"));r.Rate("difficulty","fair");
            ui.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return null;
            Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));
            int guard=0;while(ui.VisiblePage=="Dialogue"){Assert.That(++guard,Is.LessThan(8));ui.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();}
            Assert.That(ui.PlaytestComplete,Is.True);Assert.That(ui.Board,Is.Null);
            f.Session.TryLoad(out var completed,out _);Assert.That(completed.player.acknowledgedChapterCompletions,Is.Empty);
            Assert.That(Export(r.CheckpointPath).attempts.Count,Is.EqualTo(1));
            ui.ReplayCompletedPlaytest();Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));
            f.Session.TryLoad(out var replaySelected,out _);Assert.That(replaySelected.player.selectedLevel,Is.EqualTo(f.EndRound));
            Assert.That(replaySelected.player.energy,Is.EqualTo(completed.player.energy));Assert.That(replaySelected.player.gold,Is.EqualTo(completed.player.gold));
            ui.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return null;
            Assert.That(r.Log.Current.round,Is.EqualTo(f.EndRound));Assert.That(r.Log.Current.committedAttempt,Is.EqualTo(2));
            f.Session.TryLoad(out var replay,out _);Assert.That(replay.player.energy,Is.EqualTo(completed.player.energy-1));
            string path=r.CheckpointPath;UnityEngine.Object.Destroy(root);yield return null;
            var export=Export(path);Assert.That(export.attempts.Count,Is.EqualTo(2));Assert.That(export.attempts[0].outcome,Is.EqualTo("won"));
            Assert.That(export.attempts[0].difficulty,Is.EqualTo("fair"));Assert.That(export.attempts[1].outcome,Is.EqualTo("interrupted"));
            Assert.That(export.attempts.All(a=>a.round==f.EndRound),Is.True);
            string capture=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");
            if(!string.IsNullOrEmpty(capture)){Directory.CreateDirectory(capture);File.Copy(path,Path.Combine(capture,$"AutomatedTerminal{f.EndRound}.json"),true);}
        }
    }
}
#endif
