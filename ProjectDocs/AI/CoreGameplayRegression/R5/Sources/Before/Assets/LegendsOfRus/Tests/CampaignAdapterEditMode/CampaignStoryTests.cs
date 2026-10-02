using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using static LegendsOfRus.Tests.Campaign.CampaignSaveSessionTests;
namespace LegendsOfRus.Tests.Campaign
{
    public sealed class CampaignStoryTests
    {
        static MemoryStore Fresh()
        {
            var store=new MemoryStore();var p=new PlayerSaveData{lastEnergyUtcTicks=DateTime.UtcNow.Ticks};
            Assert.That(Session(store).ImportLegacyCopy(JsonUtility.ToJson(p)).Committed,Is.True);store.Crash();return store;
        }
        static CampaignStoryScene Pending(CampaignSaveSession session)
        { Assert.That(session.TryGetPendingStory(out var story,out var reason),Is.True,reason);return story; }
        static CampaignPlayerSave Read(CampaignSaveSession session)
        { Assert.That(session.TryLoad(out var save,out var reason),Is.True,reason);return save; }
        static void Win(CampaignSaveSession s)
        {
            Assert.That(s.StartRound(0,DateTime.UtcNow.Ticks).Committed,Is.True);
            var level=s.ResolveSelectedRound();Assert.That(s.FinishRound(1,true,2000,level.goals.Select(g=>g.target).ToArray()).Committed,Is.True);
        }
        [Test] public void IntroAcknowledgementIsDurableIdempotentAndDoesNotSpendOrGrant()
        {
            var store=Fresh();var s=Session(store);var before=Read(s);var intro=Pending(s);Assert.That(intro.Scene.id,Is.EqualTo("chapter01.intro"));
            Assert.That(s.AcknowledgeStory("unknown").Committed,Is.False);Assert.That(s.AcknowledgeStory(intro.Key).Committed,Is.True);
            int writes=store.Writes;Assert.That(s.AcknowledgeStory(intro.Key).NoOp,Is.True);Assert.That(store.Writes,Is.EqualTo(writes));store.Crash();s=Session(store);
            Assert.That(Pending(s),Is.Null);var after=Read(s);after.player.completedStoryScenes.Clear();Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(before)));
        }
        [Test] public void VictoryContinueAtomicallySelectsNextAndStoryNeverRegrantsRewards()
        {
            var store=Fresh();var s=Session(store);s.AcknowledgeStory(Pending(s).Key);Win(s);Assert.That(Pending(s),Is.Null,"result comes before story");
            var won=Read(s);Assert.That(s.AcknowledgeResult(1,true).Committed,Is.True);store.Crash();s=Session(store);
            var pending=Pending(s);Assert.That(pending.Scene.id,Is.EqualTo("chapter01.firebird_found"));Assert.That(Read(s).player.selectedLevel,Is.EqualTo(2));
            Assert.That(s.AcknowledgeResult(1,true).NoOp,Is.True);Assert.That(s.AcknowledgeStory(pending.Key).Committed,Is.True);Assert.That(Pending(s),Is.Null);
            var after=Read(s);Assert.That(after.player.gold,Is.EqualTo(won.player.gold));Assert.That(after.player.energy,Is.EqualTo(won.player.energy));Assert.That(after.player.levelProgress[0].completions,Is.EqualTo(1));
        }
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)][TestCase(6)][TestCase(7)]
        public void StoryWriteFailureReopensWithoutLosingOrDuplicatingAcknowledgement(int fault)
        {
            var store=Fresh();var s=Session(store);var key=Pending(s).Key;var before=Read(s);store.Writes=0;store.FailAt=fault;s.AcknowledgeStory(key);
            store.Crash();s=Session(store);var pending=Pending(s);if(pending!=null)Assert.That(s.AcknowledgeStory(pending.Key).Committed,Is.True);
            var after=Read(s);Assert.That(after.player.completedStoryScenes.Count(k=>k==key),Is.EqualTo(1));Assert.That(after.player.energy,Is.EqualTo(before.player.energy));Assert.That(after.player.gold,Is.EqualTo(before.player.gold));Assert.That(after.runSequence,Is.Zero);
        }
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)][TestCase(6)][TestCase(7)]
        public void ContinueWriteFailureCannotSplitAcknowledgementAndNextSelection(int fault)
        {
            var store=Fresh();var s=Session(store);s.AcknowledgeStory(Pending(s).Key);Win(s);var before=Read(s);store.Writes=0;store.FailAt=fault;s.AcknowledgeResult(1,true);
            store.Crash();s=Session(store);var recovered=Read(s);Assert.That(recovered.player.selectedLevel,Is.EqualTo(recovered.run.acknowledged?2:1));
            Assert.That(s.AcknowledgeResult(1,true).Committed,Is.True);Assert.That(Read(s).player.gold,Is.EqualTo(before.player.gold));Assert.That(Pending(s).Scene.id,Is.EqualTo("chapter01.firebird_found"));
        }
        [Test] public void BridgeFinaleAndAllDialogueLocalesAreScopedWithoutUnlockingChapterTwo()
        {
            var source=JsonUtility.FromJson<PlayerSaveData>(Legacy());source.completedStoryScenes.Clear();var store=new MemoryStore();var s=Session(store);s.ImportLegacyCopy(JsonUtility.ToJson(source));
            int count=0;CampaignStoryScene last=null;
            for(var pending=Pending(s);pending!=null;pending=Pending(s))
            {
                Assert.That(++count,Is.LessThan(10));last=pending;Assert.That(pending.Key,Does.StartWith("bridge-story:"));
                foreach(var line in pending.Scene.lines)foreach(var locale in new[]{pending.Russian,pending.English})
                {Assert.That(locale.entries.Any(e=>e.key==line.textKey&&!string.IsNullOrWhiteSpace(e.value)),Is.True);Assert.That(locale.entries.Any(e=>e.key==line.speakerKey&&!string.IsNullOrWhiteSpace(e.value)),Is.True);}
                Assert.That(s.AcknowledgeStory(pending.Key).Committed,Is.True);
            }
            Assert.That(count,Is.EqualTo(8));Assert.That(last.Scene.id,Is.EqualTo("chapter01.finale"));Assert.That(Read(s).player.acknowledgedChapterCompletions,Is.Empty);
            Assert.That(s.Availability(2,1),Is.Not.EqualTo(CampaignRoundAvailability.Playable));
        }
        [Test] public void ArchivedBridgeScenesDoNotSuppressExpandedCampaignStory()
        {
            var source=JsonUtility.FromJson<PlayerSaveData>(Legacy());source.completedStoryScenes.Add("chapter01.intro");var json=JsonUtility.ToJson(source);
            var store=new MemoryStore();Session(store).ImportLegacyCopy(json);
            Assert.That(Pending(Session(store)).Scene.id,Is.Not.EqualTo("chapter01.intro"));
            var input=CampaignAdapterTests.FullChapterOne();
            input.StoryJson=input.StoryJson.Replace("fixture.intro","chapter01.intro");
            var expanded=new ValidatedCampaignCatalog(Definition());Assert.That(expanded.TryAdd(input,out var errors),Is.True,string.Join("\n",errors));
            Assert.That(Pending(Session(store,expanded)).Scene.id,Is.EqualTo("chapter01.intro"));
        }
    }
}
