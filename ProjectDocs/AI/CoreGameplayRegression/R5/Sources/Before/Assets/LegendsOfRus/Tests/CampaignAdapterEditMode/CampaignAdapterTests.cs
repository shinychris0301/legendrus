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
    public sealed class CampaignAdapterTests
    {
        static string Resource(string path) => Resources.Load<TextAsset>(path).text;
        internal static CampaignDefinition Definition() => JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(Path.Combine(Application.dataPath,
            "../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
        internal static CampaignPackageInput Legacy() => new CampaignPackageInput { ChapterId=1,ContentRevision=1,LegacyChapterOne=true,
            LevelsJson=Resource("Config/chapter01_levels"),StoryJson=Resource("Config/chapter01_story"),RussianJson=Resource("Localization/story_ru"),EnglishJson=Resource("Localization/story_en") };
        // Synthetic Chapter 2 authoring fixture using real LevelData mechanics, not production content.
        internal static CampaignPackageInput ChapterTwo(int count = 2)
        {
            var source=JsonUtility.FromJson<ChapterLevelCatalogData>(Resource("Config/chapter01_levels"));
            var levels=Enumerable.Range(1,count).Select(i=>{
                var l=JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(source.levels[0]));l.chapterId=2;l.levelId=i;l.id="chapter02.level"+i.ToString("000");
                l.storyTriggers=i==1?new[]{"dialogue.fixture"}:Array.Empty<string>();return l;}).ToArray();
            var story=new ChapterStoryCatalogData{chapterId=2,introSceneId="fixture.intro",completion=null,
                scenes=new[]{new StorySceneData{id="fixture.intro",trigger="fixture.intro",returnRoute="PreLevel",lines=new[]{new StoryLineData{speakerKey="speaker",textKey="body"}}},
                    new StorySceneData{id="fixture.after",trigger="dialogue.fixture",lines=new[]{new StoryLineData{speakerKey="speaker",textKey="body"}}}},
                milestones=new[]{new StoryMilestoneData{id="fixture.milestone",afterLevelId=1,sceneId="fixture.after",major=true}},
                reactions=levels.SelectMany(l=>new[]{"victory","failure"}.Select(o=>new StoryReactionData{id=l.id+o,levelId=l.levelId,outcome=o,speakerKey="speaker",textKey="body"})).ToArray()};
            var ru=new StoryLocalizationData{locale="ru",entries=new[]{new StoryLocalizationEntry{key="speaker",value="Тест"},new StoryLocalizationEntry{key="body",value="Тестовая сцена"}}};
            var en=JsonUtility.FromJson<StoryLocalizationData>(JsonUtility.ToJson(ru));en.locale="en";en.entries[0].value="Test";en.entries[1].value="Fixture scene";
            return new CampaignPackageInput{ChapterId=2,ContentRevision=1,LevelsJson=JsonUtility.ToJson(new ChapterLevelCatalogData{chapterId=2,chapterName=Definition().Chapters[1].RussianName,levels=levels}),StoryJson=JsonUtility.ToJson(story),RussianJson=JsonUtility.ToJson(ru),EnglishJson=JsonUtility.ToJson(en)};
        }
        static ValidatedCampaignCatalog Catalog() => new ValidatedCampaignCatalog(Definition());
        [Test] public void RealLegacyPackageLoadsThroughExplicitExistingCatalogBoundary()
        {
            var catalog=Catalog();Assert.That(catalog.TryAdd(Legacy(),out var errors),Is.True,string.Join("\n",errors));
            var loaded=ChapterLevelCatalog.Find(catalog,1,8);Assert.That(loaded.boardSeed,Is.EqualTo(1108));Assert.That(loaded.goals.Any(g=>g.type=="bring_down"),Is.True);
            Assert.That(catalog.LoadStory(1).scenes.Length,Is.EqualTo(8));
            Assert.That(catalog.Progression(Array.Empty<CampaignProgressRecord>(),Array.Empty<int>()).Availability(1,11),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable));
        }
        [Test] public void SecondChapterResolvesItsOwnDataAndNeverFallsBackToChapterOne()
        {
            var catalog=Catalog();Assert.That(catalog.TryAdd(ChapterTwo(),out var errors),Is.True,string.Join("\n",errors));
            Assert.That(ChapterLevelCatalog.Find(catalog,2,2).chapterId,Is.EqualTo(2));
            Assert.That(ChapterLevelCatalog.Find(catalog,1,1),Is.Null);Assert.That(ChapterLevelCatalog.Find(catalog,2,3),Is.Null);
            Assert.That(catalog.Progression(Array.Empty<CampaignProgressRecord>(),Array.Empty<int>()).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Locked));
        }
        [Test] public void FiveHundredActualSchemaRoundsValidateWithoutRelaxingLegacyRules()
        {
            var input=ChapterTwo(500);var catalog=Catalog();Assert.That(catalog.TryAdd(input,out var errors),Is.True,string.Join("\n",errors));
            Assert.That(ChapterLevelCatalog.Find(catalog,2,500),Is.Not.Null);
            var levels=JsonUtility.FromJson<ChapterLevelCatalogData>(input.LevelsJson);levels.chapterId=1;foreach(var l in levels.levels)l.chapterId=1;
            Assert.That(LevelDataValidator.ValidateCatalog(levels).IsValid,Is.False);Assert.That(LevelDataValidator.ValidateCampaignCatalog(levels).IsValid,Is.True);
        }
        [Test] public void AcceptedAssetsAreImmutableSnapshots()
        {
            var input=Legacy();var catalog=Catalog();Assert.That(catalog.TryAdd(input,out _),Is.True);input.LevelsJson="{}";
            var first=catalog.LoadChapter(1);first.levels[0].moveLimit=99;Assert.That(catalog.LoadChapter(1).levels[0].moveLimit,Is.EqualTo(28));
        }
        [TestCase("json")] [TestCase("chapter")] [TestCase("moves")] [TestCase("revision")] [TestCase("locale")] [TestCase("reaction")] [TestCase("prematureFinale")]
        public void InvalidPackageIsNotInstalled(string defect)
        {
            var input=ChapterTwo();
            if(defect=="json")input.LevelsJson="broken";
            if(defect=="chapter")input.ChapterId=3;
            if(defect=="revision")input.ContentRevision=8;
            if(defect=="locale")input.EnglishJson=input.RussianJson;
            if(defect=="prematureFinale")input.FinaleReady=true;
            if(defect=="moves"){var l=JsonUtility.FromJson<ChapterLevelCatalogData>(input.LevelsJson);l.levels[0].moveLimit=0;input.LevelsJson=JsonUtility.ToJson(l);}
            if(defect=="reaction"){var s=JsonUtility.FromJson<ChapterStoryCatalogData>(input.StoryJson);s.reactions[0].textKey="missing";input.StoryJson=JsonUtility.ToJson(s);}
            var catalog=Catalog();Assert.That(catalog.TryAdd(input,out var errors),Is.False);Assert.That(errors,Is.Not.Empty);Assert.That(catalog.LoadChapter(input.ChapterId),Is.Null);
        }
        [Test] public void ExistingPackageCannotBeSilentlyReplaced()
        { var c=Catalog();Assert.That(c.TryAdd(Legacy(),out _),Is.True);Assert.That(c.TryAdd(Legacy(),out _),Is.False); }
        [Test] public void LegacyFinaleCannotBecomeCampaignFinale()
        { var p=Legacy();p.FinaleReady=true;Assert.That(Catalog().TryAdd(p,out _),Is.False);p.FinaleReady=false;p.LegacyChapterOne=false;Assert.That(Catalog().TryAdd(p,out _),Is.False); }

        [Test] public void LocaleLookupUsesRequestedChapterAndReturnsACopy()
        {
            var c=Catalog();Assert.That(c.TryAdd(ChapterTwo(),out _),Is.True);
            var en=c.LoadLocalization(2,"en");Assert.That(en.entries[1].value,Is.EqualTo("Fixture scene"));en.entries[1].value="changed";
            Assert.That(c.LoadLocalization(2,"en").entries[1].value,Is.EqualTo("Fixture scene"));
            Assert.That(c.LoadLocalization(1,"en"),Is.Null);Assert.That(c.LoadLocalization(2,"fr"),Is.Null);
        }
        internal static CampaignPackageInput FullChapterOne()
        {
            var p=ChapterTwo(500);p.ChapterId=1;p.FinaleReady=true;
            var levels=JsonUtility.FromJson<ChapterLevelCatalogData>(p.LevelsJson);levels.chapterId=1;levels.chapterName=Definition().Chapters[0].RussianName;
            foreach(var level in levels.levels){level.chapterId=1;level.id=level.id.Replace("chapter02","chapter01");}
            levels.levels[499].storyTriggers=new[]{"dialogue.finale","chapter.complete"};p.LevelsJson=JsonUtility.ToJson(levels);
            var story=JsonUtility.FromJson<ChapterStoryCatalogData>(p.StoryJson);story.chapterId=1;
            story.scenes=story.scenes.Concat(new[]{new StorySceneData{id="fixture.finale",trigger="dialogue.finale",returnRoute="ChapterComplete",
                lines=Enumerable.Range(1,4).Select(i=>new StoryLineData{speakerKey="speaker",textKey="body"}).ToArray()}}).ToArray();
            story.milestones=story.milestones.Concat(new[]{new StoryMilestoneData{id="fixture.end",afterLevelId=500,sceneId="fixture.finale",major=true}}).ToArray();
            story.completion=new StoryChapterCompletionData{finalLevelId=500,finaleSceneId="fixture.finale",completionTrigger="chapter.complete",summaryTitleKey="speaker",summaryFormatKey="summary",rewardNoteKey="body"};
            p.StoryJson=JsonUtility.ToJson(story);
            var ru=JsonUtility.FromJson<StoryLocalizationData>(p.RussianJson);ru.entries=ru.entries.Concat(new[]{new StoryLocalizationEntry{key="summary",value="{0} {1} {2} {3}"}}).ToArray();p.RussianJson=JsonUtility.ToJson(ru);
            var en=JsonUtility.FromJson<StoryLocalizationData>(p.EnglishJson);en.entries=en.entries.Concat(new[]{new StoryLocalizationEntry{key="summary",value="{0} {1} {2} {3}"}}).ToArray();p.EnglishJson=JsonUtility.ToJson(en);
            return p;
        }
        [Test] public void FullyValidatedFinaleAndProgressUnlockSecondChapter()
        {
            var p=FullChapterOne();
            var c=Catalog();Assert.That(c.TryAdd(p,out var errors),Is.True,string.Join("\n",errors));Assert.That(c.TryAdd(ChapterTwo(),out errors),Is.True,string.Join("\n",errors));
            var records=Enumerable.Range(1,500).Select(i=>new CampaignProgressRecord{ChapterId=1,RoundId=i,Completions=1}).ToArray();
            Assert.That(c.Progression(records,Array.Empty<int>()).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Locked));
            Assert.That(c.Progression(records,new[]{1}).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Playable));
            Assert.That(ChapterLevelCatalog.Find(c,2,1).chapterId,Is.EqualTo(2));
        }

        static string Save()
        {
            var save=new PlayerSaveData{gold=9876,gems=321,energy=2,lastEnergyUtcTicks=123456789,selectedLevel=10,
                completedStoryScenes=new List<string>{"chapter01.finale"},acknowledgedChapterCompletions=new List<int>{1},completedTutorialSteps=new List<string>{"tutorial.match.start"}};
            save.levelProgress=Enumerable.Range(1,10).Select(i=>new LevelProgressData{chapterId=1,levelId=i,bestStars=2,bestScore=1500,attempts=3,failures=2,retries=1,completions=1,rewardClaimed=true}).ToList();
            return JsonUtility.ToJson(save);
        }
        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)]
        public void JournalRehearsalRecoversAcrossEveryWriteBoundary(int mutation)
        {
            var source=Save();var result=CampaignMigrationRehearsal.Run(source,mutation);
            Assert.That(result.RecoveredPayloadValid,Is.True);Assert.That(result.RepeatedMigrationWasNoOp,Is.True);
            if(mutation>0)Assert.That(result.InterruptedWrite.Success,Is.False);
            var candidate=JsonUtility.FromJson<CampaignMigrationCandidate>(result.FinalCandidate);
            Assert.That(candidate.sourcePayload,Is.EqualTo(source));Assert.That(candidate.campaignFinales,Is.Empty);
            Assert.That(candidate.rewardClaims.All(x=>x),Is.True);Assert.That(candidate.roundIds.Length,Is.EqualTo(10));
        }
        [Test] public void CandidateTamperingAndUnknownVersionAreRejected()
        {
            var candidate=CampaignMigrationRehearsal.Prepare(Save());Assert.That(CampaignMigrationRehearsal.Validate(candidate),Is.True);
            Assert.That(CampaignMigrationRehearsal.Validate(candidate.Replace("chapter01.round001","chapter02.round001")),Is.False);
            Assert.Throws<ArgumentException>(()=>CampaignMigrationRehearsal.Prepare(Save().Replace("\"saveVersion\":6","\"saveVersion\":7")));
        }
        [Test] public void UnknownProgressIsRetainedAtSourceAndBlocksMigration()
        {
            var save=JsonUtility.FromJson<PlayerSaveData>(Save());save.levelProgress[0].chapterId=2;string source=JsonUtility.ToJson(save);
            Assert.Throws<ArgumentException>(()=>CampaignMigrationRehearsal.Prepare(source));Assert.That(save.levelProgress[0].chapterId,Is.EqualTo(2));
        }
    }
}
