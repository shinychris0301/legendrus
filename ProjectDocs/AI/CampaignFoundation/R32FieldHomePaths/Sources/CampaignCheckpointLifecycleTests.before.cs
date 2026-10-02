using System;
using System.Collections;
using System.Linq;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignCheckpointLifecycleTests
    {
        GameObject ownedState;
        string previousSave;
        GameState State => GameState.Instance;

        [UnitySetUp] public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(isolated, Is.Not.Null.And.Not.Empty);
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(
                Path.GetFullPath(isolated) + Path.DirectorySeparatorChar, StringComparison.Ordinal), Is.True);
            if (State == null) ownedState = new GameObject("CheckpointLifecycleState", typeof(GameState));
            previousSave = JsonUtility.ToJson(State.Save);
            StoryService.ResetRuntime();
            ChapterLevelCatalog.LoadChapter1(true);
            StoryCatalog.LoadChapter1(true);
            yield return null;
        }

        [UnityTearDown] public IEnumerator TearDown()
        {
            StoryService.ResetRuntime();
            if (State != null) { State.Save = JsonUtility.FromJson<PlayerSaveData>(previousSave); State.Persist(); }
            if (ownedState != null) UnityEngine.Object.Destroy(ownedState);
            yield return null;
        }

        void PrepareCompletedFixture(int round, bool acknowledgeEarlier)
        {
            var story = StoryCatalog.LoadChapter1();
            State.Save = new PlayerSaveData {
                gold = 1777, gems = 27, energy = 17, energyMax = 30,
                boosterMace = 2, boosterFirebird = 3, boosterLivingLight = 4, boosterSeaPearl = 5,
                selectedChapter = 1, selectedLevel = round, highestUnlockedLevel = Math.Min(180, round + 1),
                lastCompletedChapter = 1, lastCompletedLevel = round, lastEnergyUtcTicks = DateTime.UtcNow.Ticks
            };
            // Explicit earlier-progress fixture, never counted as human or board-play evidence.
            State.Save.levelProgress = Enumerable.Range(1, round).Select(n => new LevelProgressData {
                chapterId = 1, levelId = n, attempts = 1, completions = 1, rewardClaimed = true, bestStars = 2
            }).ToList();
            State.Save.completedStoryScenes.Add(story.introSceneId);
            if (acknowledgeEarlier) State.Save.completedStoryScenes.AddRange(
                story.milestones.Where(m => m.afterLevelId < round).Select(m => m.sceneId));
            State.Persist();
            Reload();
        }

        void Reload()
        {
            StoryService.ResetRuntime();
            var loaded = SaveService.LoadWithResult();
            Assert.That(loaded.CanPersist, Is.True);
            State.Save = loaded.Data;
        }

        string NonStoryState()
        {
            var clone = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(State.Save));
            clone.completedStoryScenes.Clear();
            return JsonUtility.ToJson(clone);
        }

        static StoryAdvanceResult FinishDialogue()
        {
            StoryAdvanceResult result; int count = 0;
            do { Assert.That(++count, Is.LessThan(20)); result = StoryService.AdvanceDialogue(); }
            while (result == StoryAdvanceResult.MoreLines);
            return result;
        }

        [TestCase(50)][TestCase(60)][TestCase(70)][TestCase(80)][TestCase(90)][TestCase(100)][TestCase(110)][TestCase(120)][TestCase(130)][TestCase(140)][TestCase(150)][TestCase(160)][TestCase(170)][TestCase(180)]
        public void UnreadCheckpointSurvivesReloadAndAcknowledgesWithoutEconomicMutation(int round)
        {
            PrepareCompletedFixture(round, true);
            string expectedScene = StoryCatalog.LoadChapter1().milestones.Single(m => m.afterLevelId == round).sceneId;
            string before = NonStoryState();
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(expectedScene));
            string russian = StoryService.CurrentLineForLanguage("ru").Text;
            string english = StoryService.CurrentLineForLanguage("en").Text;
            Assert.That(russian, Is.Not.Null.And.Not.Empty);
            Assert.That(english, Is.Not.Null.And.Not.Empty.And.Not.EqualTo(russian));
            Assert.That(StoryService.CurrentLineForLanguage("ru").Text, Is.EqualTo(russian));
            Assert.That(State.HasCompletedStoryScene(expectedScene), Is.False);
            Reload(); // Stop before Advance: no story acknowledgement has been written.
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(expectedScene));
            Assert.That(StoryService.CurrentLineForLanguage("ru").Text, Is.EqualTo(russian));
            Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));
            Reload();
            Assert.That(State.Save.completedStoryScenes.Count(id => id == expectedScene), Is.EqualTo(1));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(State.CompleteStoryScene(expectedScene), Is.False);
            Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.False);
            Assert.That(NonStoryState(), Is.EqualTo(before));
        }

        [TestCase(50)][TestCase(100)][TestCase(150)][TestCase(160)][TestCase(170)][TestCase(180)]
        public void FailedCheckpointAndRetryPersistWithoutRewardsUnlockOrStory(int round)
        {
            // Service-level loss fixture; this does not simulate board input or assess difficulty.
            PrepareCompletedFixture(round - 1, true);
            Assert.That(State.SelectLevel(1, round), Is.True);
            var level = ChapterLevelCatalog.Find(1, round);
            string expectedScene = StoryCatalog.LoadChapter1().milestones.Single(m => m.afterLevelId == round).sceneId;
            string before = StateWithoutLevelProgress();
            for (int attempt = 1; attempt <= 2; attempt++) {
                Assert.That(State.RegisterLevelAttempt(level), Is.EqualTo(attempt));
                State.RecordLevelFailure(level, 100 * attempt);
                Reload();
                var progress = State.GetLevelProgress(1, round);
                Assert.That(progress.attempts, Is.EqualTo(attempt));
                Assert.That(progress.retries, Is.EqualTo(attempt - 1));
                Assert.That(progress.failures, Is.EqualTo(attempt));
                Assert.That(progress.bestScore, Is.EqualTo(100 * attempt));
                Assert.That(progress.completions, Is.Zero);
                Assert.That(progress.bestStars, Is.Zero);
                Assert.That(progress.rewardClaimed, Is.False);
                Assert.That(State.Save.highestUnlockedLevel, Is.EqualTo(round));
                Assert.That(State.HasCompletedStoryScene(expectedScene), Is.False);
                Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
                Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
                Assert.That(StateWithoutLevelProgress(), Is.EqualTo(before));
            }
        }

        string StateWithoutLevelProgress()
        {
            var clone = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(State.Save));
            clone.levelProgress.Clear();
            return JsonUtility.ToJson(clone);
        }

        [Test] public void ReturningPlayerReceivesAllTwentyFiveMilestonesInOrderWithoutExtraRewards()
        {
            PrepareCompletedFixture(180, false);
            string before = NonStoryState();
            var milestones = StoryCatalog.LoadChapter1().milestones;
            Assert.That(milestones.Length, Is.EqualTo(25));
            CollectionAssert.AreEqual(milestones.Select(m => m.afterLevelId).OrderBy(n => n), milestones.Select(m => m.afterLevelId));
            foreach (var milestone in milestones) {
                Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
                Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestone.sceneId));
                Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));
                Reload();
                Assert.That(NonStoryState(), Is.EqualTo(before));
            }
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(State.Save.completedStoryScenes.Count, Is.EqualTo(26));
            Assert.That(State.HasCompletedChapter(1), Is.False);
        }

        [Test] public void UnavailableRoundAndChapterCannotChangeCompletedHundredEightySave()
        {
            PrepareCompletedFixture(180, true);
            string before = JsonUtility.ToJson(State.Save);
            Assert.That(ChapterLevelCatalog.Find(1, 181), Is.Null);
            Assert.That(ChapterLevelCatalog.Next(ChapterLevelCatalog.Find(1, 180)), Is.Null);
            Assert.That(State.SelectLevel(1, 181), Is.False);
            Assert.That(State.SelectLevel(2, 1), Is.False);
            Assert.That(State.AcknowledgeChapterCompletion(1), Is.False);
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
            Reload();
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
            Assert.That(State.Save.highestUnlockedLevel, Is.EqualTo(180));
        }
    }
}
