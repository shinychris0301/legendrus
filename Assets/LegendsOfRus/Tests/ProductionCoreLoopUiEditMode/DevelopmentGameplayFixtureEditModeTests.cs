using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class DevelopmentGameplayFixtureEditModeTests
    {
        static readonly Type Factory = typeof(GameState).Assembly.GetType("LegendsOfRus.DevelopmentGameplayFixtures", true);
        static readonly MethodInfo Build = Factory.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic,
            null, new[] { typeof(string), typeof(long), typeof(ChapterLevelCatalogData), typeof(ChapterStoryCatalogData) }, null);
        static PlayerSaveData Create(string name, long ticks, ChapterLevelCatalogData levels, ChapterStoryCatalogData story)
        {
            try { return (PlayerSaveData)Build.Invoke(null, new object[] { name, ticks, levels, story }); }
            catch (TargetInvocationException error) when (error.InnerException != null) { throw error.InnerException; }
        }
        static T Clone<T>(T source) => JsonUtility.FromJson<T>(JsonUtility.ToJson(source));

        [TestCase("stock-999")][TestCase("stock-1000")][TestCase("stock-9999")][TestCase("stock-max")]
        [TestCase("story-early")][TestCase("story-finale")]
        public void PresetsMatchIndependentlyRehearsedServiceOutcomes(string name)
        {
            var levels = ChapterLevelCatalog.LoadChapter1(); var story = StoryCatalog.LoadChapter1();
            string beforeLevels = JsonUtility.ToJson(levels), beforeStory = JsonUtility.ToJson(story);
            string path = Path.Combine(Application.dataPath, "../ProjectDocs/AI/AndroidGameplayProfile/R3FixturePreparation/QA/Fixtures", name + ".player.json");
            var expected = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(path));
            expected.lastEnergyUtcTicks = 123456789;
            var actual = Create(name, expected.lastEnergyUtcTicks, levels, story);
            Assert.That(JsonUtility.ToJson(actual), Is.EqualTo(JsonUtility.ToJson(expected)));
            Assert.That(JsonUtility.ToJson(levels), Is.EqualTo(beforeLevels));
            Assert.That(JsonUtility.ToJson(story), Is.EqualTo(beforeStory));
            actual.levelProgress.Clear(); actual.completedStoryScenes.Clear();
            Assert.That(JsonUtility.ToJson(Create(name, expected.lastEnergyUtcTicks, levels, story)), Is.EqualTo(JsonUtility.ToJson(expected)), "Candidates must not share mutable lists.");
        }

        [TestCase(null)][TestCase("")][TestCase("stock-1001")][TestCase("Stock-1000")][TestCase("story-finale ")][TestCase("fresh")]
        public void FactoryRejectsUnboundedOrExistingNonPresetCommands(string command)
        {
            Assert.Throws<ArgumentException>(() => Create(command, 1, ChapterLevelCatalog.LoadChapter1(), StoryCatalog.LoadChapter1()));
        }

        [TestCase(0L)][TestCase(-1L)][TestCase(long.MaxValue)]
        public void FactoryRejectsInvalidClockValues(long ticks)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Create("story-finale", ticks, ChapterLevelCatalog.LoadChapter1(), StoryCatalog.LoadChapter1()));
        }

        [TestCase("fallback")][TestCase("null-level")][TestCase("wrong-chapter")][TestCase("missing-scene")]
        [TestCase("null-milestone")][TestCase("wrong-finale")][TestCase("trigger")][TestCase("overflow")]
        public void InvalidOrIncompatibleInstalledContentCannotProduceCandidate(string defect)
        {
            var levels = Clone(ChapterLevelCatalog.LoadChapter1()); var story = Clone(StoryCatalog.LoadChapter1());
            if (defect == "fallback") levels.levels = levels.levels.Take(1).ToArray();
            if (defect == "null-level") levels.levels[0] = null;
            if (defect == "wrong-chapter") levels.chapterId = 2;
            if (defect == "missing-scene") story.scenes[0].id = "missing-intro";
            if (defect == "null-milestone") story.milestones[0] = null;
            if (defect == "wrong-finale") story.completion.finaleSceneId = "unknown";
            if (defect == "trigger") story.scenes.First(s => s.id == story.milestones[0].sceneId).trigger = "missing-trigger";
            if (defect == "overflow") levels.levels[0].rewards.gold = int.MaxValue;
            if (defect == "overflow") Assert.Throws<OverflowException>(() => Create("story-finale", 1, levels, story));
            else Assert.Throws<InvalidOperationException>(() => Create("story-finale", 1, levels, story));
        }
    }
}
