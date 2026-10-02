using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus.Tests.Campaign
{
    public sealed class CampaignSaveSessionTests
    {
        internal static CampaignDefinition Definition() => CampaignAdapterTests.Definition();
        internal static ValidatedCampaignCatalog Catalog(bool complete = false)
        {
            var c = new ValidatedCampaignCatalog(Definition());
            Assert.That(c.TryAdd(complete ? CampaignAdapterTests.FullChapterOne() : CampaignAdapterTests.Legacy(), out var errors), Is.True, string.Join("\n",errors));
            Assert.That(c.TryAdd(CampaignAdapterTests.ChapterTwo(),out errors),Is.True,string.Join("\n",errors));
            return c;
        }
        internal static string Legacy()
        {
            var p=new PlayerSaveData {gold=5432,gems=123,energy=2,lastEnergyUtcTicks=123456,selectedLevel=10,highestUnlockedLevel=10,
                lastRewardGold=120,lastRewardEligibleForDouble=true,music=false,sfx=false,notifications=false,dailyRewardCycleDay=3,dailyRewardMonthClaims=4,
                lastDailyRewardUtc="2026-09-25",dailyRewardMonthUtc="2026-09",boosterMace=2,boosterFirebird=3,boosterLivingLight=4,boosterSeaPearl=5,
                completedTutorialSteps=new List<string>{"tutorial.match.start"},completedStoryScenes=new List<string>{"chapter01.finale"},acknowledgedChapterCompletions=new List<int>{1}};
            p.levelProgress=Enumerable.Range(1,10).Select(i=>new LevelProgressData{chapterId=1,levelId=i,bestStars=2,bestScore=1500,attempts=2,retries=1,failures=1,completions=1,rewardClaimed=true}).ToList();
            return JsonUtility.ToJson(p);
        }
        sealed class Inspector : ILegacySavePayloadInspector { public LegacyPayloadInspection Inspect(string payload) => LegacyPayloadInspection.NotRecognized(); }
        internal static SaveJournalRepository Repository(ISaveJournalStore store,int version=9) => new SaveJournalRepository(store,new SaveEnvelopeCodec(),new Inspector(),CampaignSaveSession.DevelopmentKeys(),version);
        internal static CampaignSaveSession Session(ISaveJournalStore store,ValidatedCampaignCatalog catalog=null) => new CampaignSaveSession(store,Definition(),catalog??Catalog());
        internal sealed class MemoryStore : ISaveJournalStore
        {
            public Dictionary<string,string> Current=new Dictionary<string,string>(),Durable=new Dictionary<string,string>();
            public int Writes, FailAt;
            void Mutate(){if(++Writes==FailAt)throw new IOException("Injected failure");}
            public bool TryRead(string key,out string value)=>Current.TryGetValue(key,out value);
            public void Write(string key,string value){Mutate();Current[key]=value;}
            public void Delete(string key){Mutate();Current.Remove(key);}
            public void Flush(){Mutate();Durable=new Dictionary<string,string>(Current);}
            public void Crash(){Current=new Dictionary<string,string>(Durable);FailAt=0;Writes=0;}
        }
        // Disk fixture only: proves a fresh store/session reopens the journal, not OS power-loss guarantees.
        sealed class DiskStore : ISaveJournalStore
        {
            readonly string directory;
            public DiskStore(string directory){this.directory=directory;Directory.CreateDirectory(directory);}
            string PathFor(string key)=>Path.Combine(directory,key);
            public bool TryRead(string key,out string value){var p=PathFor(key);value=File.Exists(p)?File.ReadAllText(p):null;return value!=null;}
            public void Write(string key,string value)=>File.WriteAllText(PathFor(key),value);
            public void Delete(string key)=>File.Delete(PathFor(key));
            public void Flush(){}
        }
        [Test] public void MigrationPreservesEconomyCountersAndArchivesLegacyFinale()
        {
            var source=Legacy();var original=JsonUtility.FromJson<PlayerSaveData>(source);var data=CampaignPlayerSaveCodec.FromLegacy(source,Definition());
            Assert.That(data.legacySourceJson,Is.EqualTo(source));Assert.That(data.player.gold,Is.EqualTo(original.gold));Assert.That(data.player.gems,Is.EqualTo(original.gems));
            Assert.That(data.player.energy,Is.EqualTo(original.energy));Assert.That(data.player.lastEnergyUtcTicks,Is.EqualTo(original.lastEnergyUtcTicks));
            Assert.That(data.player.boosterMace,Is.EqualTo(2));Assert.That(data.player.boosterSeaPearl,Is.EqualTo(5));
            Assert.That(data.player.levelProgress.Select(p=>JsonUtility.ToJson(p)),Is.EqualTo(original.levelProgress.Select(p=>JsonUtility.ToJson(p))));
            Assert.That(data.player.completedTutorialSteps,Is.EqualTo(original.completedTutorialSteps));Assert.That(data.player.lastDailyRewardUtc,Is.EqualTo(original.lastDailyRewardUtc));
            Assert.That(data.player.music,Is.False);Assert.That(data.player.notifications,Is.False);Assert.That(data.player.lastRewardEligibleForDouble,Is.True);
            Assert.That(data.player.completedStoryScenes,Is.Empty);Assert.That(data.player.acknowledgedChapterCompletions,Is.Empty);
            Assert.That(data.player.highestUnlockedLevel,Is.EqualTo(11));
        }
        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)]
        public void RealCampaignMigrationSurvivesAllJournalInterruptions(int fault)
        {
            var store=new MemoryStore();var source=Legacy();Assert.That(Repository(store).TrySave(source,6).Success,Is.True);store.Writes=0;store.FailAt=fault;
            var result=Session(store).MigrateLegacy();Assert.That(result.JournalWrite.Success,Is.False);
            Assert.That(result.Committed,Is.EqualTo(fault>=7),"Only successful primary flush is a commit.");
            store.Crash();var restarted=Session(store);Assert.That(restarted.MigrateLegacy().Committed,Is.True);
            Assert.That(restarted.TryLoad(out var data,out var error),Is.True,error);Assert.That(data.legacySourceJson,Is.EqualTo(source));
            Assert.That(data.player.gold,Is.EqualTo(5432));Assert.That(data.player.levelProgress.All(p=>p.rewardClaimed),Is.True);
            int writes=store.Writes;Assert.That(restarted.MigrateLegacy().NoOp,Is.True);Assert.That(store.Writes,Is.EqualTo(writes));
        }
        [Test] public void SelectionPersistsAfterFreshDiskStoreAndDoesNotChargeEnergy()
        {
            var directory=Path.Combine(Path.GetTempPath(),"lor-campaign-r3-"+Guid.NewGuid().ToString("N"));
            try{
                var first=Session(new DiskStore(directory));Assert.That(first.ImportLegacyCopy(Legacy()).Committed,Is.True);
                Assert.That(first.SelectRound(1,3).Committed,Is.True);
                var second=Session(new DiskStore(directory));Assert.That(second.ResolveSelectedRound().levelId,Is.EqualTo(3));
                Assert.That(second.TryLoad(out var data,out _),Is.True);Assert.That(data.player.energy,Is.EqualTo(2));
                Assert.That(second.SelectRound(1,11).Committed,Is.False);Assert.That(second.SelectRound(2,1).Committed,Is.False);
            }finally{Directory.Delete(directory,true);}
        }
        [Test] public void FullChapterProgressSelectsSecondChapterAndMissingPackagesDoNotEraseIt()
        {
            var data=CampaignPlayerSaveCodec.FromLegacy(Legacy(),Definition());
            data.player.levelProgress=Enumerable.Range(1,500).Select(i=>new LevelProgressData{chapterId=1,levelId=i,bestStars=2,bestScore=1500,attempts=2,retries=1,failures=1,completions=1,rewardClaimed=true}).ToList();
            data.player.acknowledgedChapterCompletions.Add(1);CampaignPlayerSaveCodec.RefreshDerivedProgress(data.player);
            var store=new MemoryStore();Assert.That(Repository(store).TrySave(CampaignPlayerSaveCodec.Encode(data,Definition()),9).Success,Is.True);
            var session=Session(store,Catalog(true));Assert.That(session.SelectRound(2,1).Committed,Is.True);Assert.That(session.ResolveSelectedRound().chapterId,Is.EqualTo(2));
            store.Crash();var missing=Session(store,new ValidatedCampaignCatalog(Definition()));int writes=store.Writes;
            Assert.That(missing.ResolveSelectedRound(),Is.Null);Assert.That(missing.TryLoad(out var restored,out _),Is.True);
            Assert.That(restored.player.selectedChapter,Is.EqualTo(2));Assert.That(restored.player.levelProgress.Count,Is.EqualTo(500));Assert.That(store.Writes,Is.EqualTo(writes));
        }
        [Test] public void OldApplicationCannotOverwriteCampaignSave()
        {
            var store=new MemoryStore();Assert.That(Session(store).ImportLegacyCopy(Legacy()).Committed,Is.True);var old=Repository(store,6);
            Assert.That(old.Load().Status,Is.EqualTo(SaveJournalLoadStatus.UnsupportedFutureVersion));int writes=store.Writes;
            Assert.That(old.TrySave(Legacy(),6).Success,Is.False);Assert.That(store.Writes,Is.EqualTo(writes));
        }
        [TestCase("unknown")] [TestCase("missing")] [TestCase("duplicate")] [TestCase("rehearsal")] [TestCase("hash")] [TestCase("future")]
        public void StrictCodecRejectsInvalidOrAmbiguousPayload(string defect)
        {
            string source=Legacy();string json=CampaignPlayerSaveCodec.Encode(CampaignPlayerSaveCodec.FromLegacy(source,Definition()),Definition());
            if(defect=="unknown")json=json.Insert(1,"\"unrecognized\":1,");
            if(defect=="missing")json=json.Replace("\"schemaVersion\":9,","");
            if(defect=="duplicate")json=json.Insert(1,"\"schemaVersion\":9,");
            if(defect=="rehearsal")json=CampaignMigrationRehearsal.Prepare(source);
            if(defect=="hash"){var d=JsonUtility.FromJson<CampaignPlayerSave>(json);d.legacySourceSha256="wrong";json=JsonUtility.ToJson(d);}
            if(defect=="future")json=json.Replace("\"schemaVersion\":9","\"schemaVersion\":10");
            Assert.That(CampaignPlayerSaveCodec.TryDecode(json,Definition(),out _,out _),Is.False);
        }
        [Test] public void DifferentLegacyCopyCannotReplaceExistingCampaignHistory()
        {
            var store=new MemoryStore();var session=Session(store);Assert.That(session.ImportLegacyCopy(Legacy()).Committed,Is.True);int writes=store.Writes;
            var different=JsonUtility.FromJson<PlayerSaveData>(Legacy());different.gold++;
            Assert.That(session.ImportLegacyCopy(JsonUtility.ToJson(different)).Committed,Is.False);Assert.That(store.Writes,Is.EqualTo(writes));
        }
        [Test] public void CorruptPrimaryRecoversBackupWithoutDuplicateMigrationRewards()
        {
            var store=new MemoryStore();var session=Session(store);Assert.That(session.ImportLegacyCopy(Legacy()).Committed,Is.True);
            Assert.That(session.SelectRound(1,2).Committed,Is.True);store.Write(CampaignSaveSession.DevelopmentKeys().PrimaryKey,"corrupt");store.Flush();store.Crash();
            var recovered=Session(store);Assert.That(recovered.TryLoad(out var data,out _),Is.True);Assert.That(data.player.gold,Is.EqualTo(5432));
            Assert.That(recovered.MigrateLegacy().NoOp,Is.True);
        }
        [Test] public void FutureJournalVersionAndSemanticCorruptionBlockAllWrites()
        {
            foreach(var version in new[]{9,10}){
                var store=new MemoryStore();string raw=version==9?"{}":"future";
                var envelope=new SaveEnvelopeCodec().TryEncode(raw,version,1);store.Write(CampaignSaveSession.DevelopmentKeys().PrimaryKey,envelope.Envelope);store.Flush();int writes=store.Writes;
                var session=Session(store);Assert.That(session.MigrateLegacy().Committed,Is.False);Assert.That(session.SelectRound(1,1).Committed,Is.False);Assert.That(store.Writes,Is.EqualTo(writes));
            }
        }
        [Test] public void CompleteTenThousandRoundLedgerFitsExistingEnvelopeAndRoundTrips()
        {
            var data=CampaignPlayerSaveCodec.FromLegacy(Legacy(),Definition());
            data.player.levelProgress=Enumerable.Range(1,20).SelectMany(c=>Enumerable.Range(1,500).Select(r=>new LevelProgressData{
                chapterId=c,levelId=r,bestStars=3,bestScore=int.MaxValue,attempts=int.MaxValue,retries=1000,failures=1000,completions=1000,rewardClaimed=true})).ToList();
            data.player.acknowledgedChapterCompletions=Enumerable.Range(1,20).ToList();CampaignPlayerSaveCodec.RefreshDerivedProgress(data.player);
            string json=CampaignPlayerSaveCodec.Encode(data,Definition());Assert.That(json.Length,Is.LessThanOrEqualTo(SaveEnvelopeCodec.MaximumPayloadCharacters));
            Assert.That(json,Does.Contain("compressedCampaign"));Assert.That(CampaignPlayerSaveCodec.TryDecode(json,Definition(),out var restored,out var error),Is.True,error);
            Assert.That(restored.player.levelProgress.Count,Is.EqualTo(10000));Assert.That(restored.player.stars,Is.EqualTo(30000));
            Assert.That(new SaveEnvelopeCodec().TryEncode(json,9,1).Success,Is.True);
        }
        [Test] public void LegacyRewardClaimsCannotBeRemovedFromMigratedPayload()
        {
            var data=CampaignPlayerSaveCodec.FromLegacy(Legacy(),Definition());data.player.levelProgress[0].rewardClaimed=false;
            Assert.Throws<ArgumentException>(()=>CampaignPlayerSaveCodec.Encode(data,Definition()));
        }
        [Test] public void CorruptCompressedDataIsRejected()
        {
            Assert.That(CampaignPlayerSaveCodec.TryDecode("{\"compressedCampaign\":\"not base64\"}",Definition(),out _,out _),Is.False);
        }
        [Test] public void ReturnedSaveMutationDoesNotWriteAndReimportIsNoOp()
        {
            var store=new MemoryStore();var s=Session(store);Assert.That(s.ImportLegacyCopy(Legacy()).Committed,Is.True);int writes=store.Writes;
            s.TryLoad(out var data,out _);data.player.gold=0;Assert.That(s.ImportLegacyCopy(Legacy()).NoOp,Is.True);s.TryLoad(out var restored,out _);
            Assert.That(restored.player.gold,Is.EqualTo(5432));Assert.That(store.Writes,Is.EqualTo(writes));
        }
    }
}
