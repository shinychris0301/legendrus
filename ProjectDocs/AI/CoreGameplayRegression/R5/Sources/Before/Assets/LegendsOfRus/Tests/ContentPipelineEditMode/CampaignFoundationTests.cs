using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using LegendsOfRus.ContentPipeline;

namespace LegendsOfRus.Tests.ContentPipeline
{
    public sealed class CampaignFoundationTests
    {
        static CampaignDefinition Definition() => JsonUtility.FromJson<CampaignDefinition>(File.ReadAllText(
            Path.Combine(Application.dataPath, "../ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json")));
        static CampaignChapterPackage Package(int chapter, int count, bool finale = false) => new CampaignChapterPackage
        { ChapterId = chapter, ContentRevision = 1, ValidatedRoundIds = Enumerable.Range(1, count).ToArray(), FinaleReady = finale };
        static CampaignProgressRecord Win(int chapter, int round) => new CampaignProgressRecord
        { ChapterId = chapter, RoundId = round, BestStars = 3, BestScore = 1900, Attempts = 2, Retries = 1, Failures = 1, Completions = 1, RewardClaimed = true };
        static CampaignProgressRecord[] Cleared(int chapter, int count) => Enumerable.Range(1, count).Select(i => Win(chapter, i)).ToArray();
        static CampaignProgression State(CampaignChapterPackage[] packages, CampaignProgressRecord[] records = null, int[] finales = null) =>
            new CampaignProgression(Definition(), packages, records ?? Array.Empty<CampaignProgressRecord>(), finales ?? Array.Empty<int>());

        [Test] public void CanonicalManifestHasTwentyNamedChaptersAndTenThousandStableRoundIds()
        {
            var definition = Definition(); Assert.That(CampaignContract.Validate(definition), Is.Empty);
            var ids = definition.Chapters.SelectMany(c => Enumerable.Range(1, 500).Select(r => CampaignContract.RoundId(c.ChapterId, r))).ToArray();
            Assert.That(ids.Distinct().Count(), Is.EqualTo(10000));
            Assert.That(ids.Last(), Is.EqualTo("chapter20.round500"));
        }
        [TestCase(1,1)] [TestCase(50,1)] [TestCase(51,2)] [TestCase(500,10)]
        public void SectorBoundaries(int round, int expected) => Assert.That(CampaignContract.Sector(round), Is.EqualTo(expected));
        [TestCase(0)] [TestCase(501)] public void InvalidSectorRejected(int round) => Assert.Throws<ArgumentOutOfRangeException>(() => CampaignContract.Sector(round));
        [Test] public void OldAndFutureSchemasAreRejected()
        { foreach (int version in new[]{1,3}) { var d=Definition();d.SchemaVersion=version;Assert.That(CampaignContract.Validate(d), Is.Not.Empty); } }
        [Test] public void WrongSizeAndDuplicateOrCyclicChaptersRejected()
        {
            var d=Definition();d.RoundsPerChapter=100;Assert.That(CampaignContract.Validate(d),Is.Not.Empty);
            d=Definition();d.Chapters[2].ChapterId=2;Assert.That(CampaignContract.Validate(d),Is.Not.Empty);
            d=Definition();d.Chapters[1].PrerequisiteChapterId=2;Assert.That(CampaignContract.Validate(d),Is.Not.Empty);
        }
        [Test] public void ArtworkWithoutPackageNeverMakesRoundPlayable()
        { Assert.That(State(Array.Empty<CampaignChapterPackage>()).Availability(1,1),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable)); }
        [Test] public void PartialReleaseStopsAtInstalledRoundAndDoesNotCompleteCampaignChapter()
        {
            var state=State(new[]{Package(1,10)},Cleared(1,10),new[]{1});
            Assert.That(state.Availability(1,10),Is.EqualTo(CampaignRoundAvailability.Playable));
            Assert.That(state.Availability(1,11),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable));
            Assert.That(state.ChapterCompleted(1),Is.False);
        }
        [Test] public void GapInWinsDoesNotUnlockBeyondFirstUnfinishedRound()
        {
            var state=State(new[]{Package(1,50)},new[]{Win(1,1),Win(1,3)});
            Assert.That(state.Availability(1,2),Is.EqualTo(CampaignRoundAvailability.Playable));
            Assert.That(state.Availability(1,3),Is.EqualTo(CampaignRoundAvailability.Locked));
        }
        [Test] public void FailedAttemptDoesNotCountAsWin()
        {
            var record=new CampaignProgressRecord{ChapterId=1,RoundId=1,Attempts=1,Failures=1};
            Assert.That(State(new[]{Package(1,50)},new[]{record}).Availability(1,2),Is.EqualTo(CampaignRoundAvailability.Locked));
        }
        [Test] public void ChapterTwoNeedsFullFirstChapterAndAcknowledgedFinale()
        {
            var packages=new[]{Package(1,500,true),Package(2,50)};
            Assert.That(State(packages,Cleared(1,500)).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Locked));
            var state=State(packages,Cleared(1,500),new[]{1});
            Assert.That(state.Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Playable));
            Assert.That(state.Availability(2,2),Is.EqualTo(CampaignRoundAvailability.Locked));
            Assert.That(State(packages,Cleared(1,500).Concat(Cleared(2,1)).ToArray(),new[]{1}).Availability(2,2),Is.EqualTo(CampaignRoundAvailability.Playable));
        }
        [Test] public void UnavailableNextChapterStaysUnavailableAfterFinale()
        { Assert.That(State(new[]{Package(1,500,true)},Cleared(1,500),new[]{1}).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.ContentUnavailable)); }
        [Test] public void FinaleMustBePublishedAndEveryRoundCleared()
        {
            Assert.That(State(new[]{Package(1,500)},Cleared(1,500),new[]{1}).ChapterCompleted(1),Is.False);
            Assert.That(State(new[]{Package(1,500,true)},Cleared(1,499),new[]{1}).ChapterCompleted(1),Is.False);
        }
        [Test] public void FinalChapterHasNoPhantomNextChapter()
        {
            var packages=Enumerable.Range(1,20).Select(i=>Package(i,500,true)).ToArray();
            var progress=Enumerable.Range(1,20).SelectMany(i=>Cleared(i,500)).ToArray();
            var state=State(packages,progress,Enumerable.Range(1,20).ToArray());
            Assert.That(state.ChapterCompleted(20),Is.True);
            Assert.That(state.Availability(21,1),Is.EqualTo(CampaignRoundAvailability.Invalid));
        }
        [Test] public void CorruptPackagesAreRejected()
        {
            var p=Package(1,50);p.ValidatedRoundIds[1]=3;Assert.Throws<ArgumentException>(()=>State(new[]{p}));
            p=Package(1,50);p.ContentRevision=2;Assert.Throws<ArgumentException>(()=>State(new[]{p}));
            Assert.Throws<ArgumentException>(()=>State(new[]{Package(1,50,true)}));
            Assert.Throws<ArgumentException>(()=>State(new[]{Package(1,501)}));
            Assert.Throws<ArgumentException>(()=>State(new[]{Package(1,10),Package(1,10)}));
        }
        [Test] public void InvalidProgressIsRejectedRatherThanNormalized()
        {
            var r=Win(1,1);r.BestStars=4;Assert.Throws<ArgumentException>(()=>State(new[]{Package(1,10)},new[]{r}));
            Assert.Throws<ArgumentException>(()=>State(new[]{Package(1,10)},new[]{Win(1,1),Win(1,1)}));
        }
        [Test] public void CallerMutationDoesNotChangeValidatedSnapshot()
        {
            var package=Package(1,50);var win=Win(1,1);var state=State(new[]{package},new[]{win});
            package.ValidatedRoundIds[1]=500;win.RoundId=50;
            Assert.That(state.Availability(1,2),Is.EqualTo(CampaignRoundAvailability.Playable));
            Assert.That(state.Availability(1,50),Is.EqualTo(CampaignRoundAvailability.Locked));
        }
        [Test] public void LegacyMigrationPreservesRecordsAndKeepsOldFinaleSeparate()
        {
            var source=new LegacyCampaignProgress{SelectedRound=10,Records=Cleared(1,10),CompletedStoryScenes=new[]{"chapter01.finale"},AcknowledgedChapterCompletions=new[]{1}};
            var plan=CampaignSaveMigration.Plan(source);Assert.That(plan.CanReview,Is.True);
            Assert.That(plan.Records.Length,Is.EqualTo(10));Assert.That(plan.SelectedRound,Is.EqualTo(10));
            Assert.That(plan.LegacyChapterAcknowledgements,Is.EqualTo(new[]{1}));Assert.That(plan.CampaignFinales,Is.Empty);
            Assert.That(plan.LegacyStoryScenes,Is.EqualTo(source.CompletedStoryScenes));
            Assert.That(plan.Records[0].RewardClaimed,Is.True);Assert.That(plan.Records[0].BestScore,Is.EqualTo(1900));
            Assert.That(plan.Records[0].Retries,Is.EqualTo(1));Assert.That(plan.Records[0].Failures,Is.EqualTo(1));
            Assert.That(plan.Records[0].Attempts,Is.EqualTo(2));Assert.That(plan.Records[0].Completions,Is.EqualTo(1));
            plan.Records[0].BestStars=0;plan.LegacyStoryScenes[0]="changed";
            Assert.That(source.Records[0].BestStars,Is.EqualTo(3));Assert.That(source.CompletedStoryScenes[0],Is.EqualTo("chapter01.finale"));
        }
        [Test] public void MigrationIsRepeatableWithoutGrantingOrUnlockingAnything()
        {
            var source=new LegacyCampaignProgress{Records=Cleared(1,10)};
            var a=CampaignSaveMigration.Plan(source);var b=CampaignSaveMigration.Plan(source);
            Assert.That(a.Records.Select(r=>r.RewardClaimed),Is.EqualTo(b.Records.Select(r=>r.RewardClaimed)));
            Assert.That(State(new[]{Package(1,10),Package(2,50)},a.Records,a.CampaignFinales).Availability(2,1),Is.EqualTo(CampaignRoundAvailability.Locked));
        }
        [Test] public void UnknownVersionAndRecordsBlockMigrationWithoutPartialOutput()
        {
            Assert.That(CampaignSaveMigration.Plan(new LegacyCampaignProgress{SaveVersion=7}).CanReview,Is.False);
            foreach(var records in new[]{new[]{Win(2,1)},new[]{Win(1,11)},new[]{Win(1,1),Win(1,1)}})
            { var plan=CampaignSaveMigration.Plan(new LegacyCampaignProgress{Records=records});Assert.That(plan.CanReview,Is.False);Assert.That(plan.Records,Is.Empty); }
        }
        [Test] public void SourceAndLegacyPolicyStayUnchanged()
        {
            Assert.That(ChapterPipelinePolicy.MaximumLevelCount,Is.EqualTo(100));
            Assert.That(ChapterMigrationPolicy.ExpectedLevelCount,Is.EqualTo(10));
            var json=File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/Resources/Config/chapter01_levels.json"));
            Assert.That(json,Does.Contain("\"schemaVersion\": 3"));
            var activation=File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/Resources/Config/content_pipeline_activation.json"));
            Assert.That(activation,Does.Contain("\"runtimeContentCutoverAuthorized\": false"));
        }
    }
}
