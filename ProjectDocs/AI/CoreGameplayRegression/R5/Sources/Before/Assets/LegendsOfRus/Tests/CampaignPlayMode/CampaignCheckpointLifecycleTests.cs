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
                selectedChapter = 1, selectedLevel = round, highestUnlockedLevel = Math.Min(500, round + 1),
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

        [TestCase(50)][TestCase(60)][TestCase(70)][TestCase(80)][TestCase(90)][TestCase(100)][TestCase(110)][TestCase(120)][TestCase(130)][TestCase(140)][TestCase(150)][TestCase(160)][TestCase(170)][TestCase(180)][TestCase(190)][TestCase(200)][TestCase(250)][TestCase(300)][TestCase(350)][TestCase(400)][TestCase(450)][TestCase(490)]
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

        [TestCase(50)][TestCase(100)][TestCase(150)][TestCase(160)][TestCase(170)][TestCase(180)][TestCase(190)][TestCase(200)][TestCase(250)][TestCase(300)][TestCase(350)][TestCase(400)][TestCase(450)][TestCase(500)]
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

        [Test] public void ReturningPlayerReceivesAllTwentySevenMilestonesInOrderWithoutExtraRewards()
        {
            PrepareCompletedFixture(200, false);
            string before = NonStoryState();
            var milestones = StoryCatalog.LoadChapter1().milestones.Where(m=>m.afterLevelId<=200).ToArray();
            Assert.That(milestones.Length, Is.EqualTo(27));
            CollectionAssert.AreEqual(milestones.Select(m => m.afterLevelId).OrderBy(n => n), milestones.Select(m => m.afterLevelId));
            foreach (var milestone in milestones) {
                Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
                Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestone.sceneId));
                Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));
                Reload();
                Assert.That(NonStoryState(), Is.EqualTo(before));
            }
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(State.Save.completedStoryScenes.Count, Is.EqualTo(28));
            Assert.That(State.HasCompletedChapter(1), Is.False);
        }

        [Test] public void InterruptedMultiLineCatchUpRestartsUnreadSceneWithoutSkippingOrPayingAgain()
        {
            PrepareCompletedFixture(200, false);
            var milestones = StoryCatalog.LoadChapter1().milestones.Where(m=>m.afterLevelId<=200).ToArray();
            string before = NonStoryState();
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestones[0].sceneId));
            string firstLine = StoryService.CurrentLineForLanguage("ru").Text;
            Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            string secondLine = StoryService.CurrentLineForLanguage("ru").Text;
            Assert.That(secondLine, Is.Not.EqualTo(firstLine));
            // Repeated navigation requests must not advance or replace the active scene.
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.CurrentLineForLanguage("ru").Text, Is.EqualTo(secondLine));
            Assert.That(State.Save.completedStoryScenes.Count, Is.EqualTo(1));
            Reload();
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestones[0].sceneId));
            Assert.That(StoryService.CurrentLineForLanguage("ru").Text, Is.EqualTo(firstLine));
            Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));
            Reload();
            Assert.That(State.Save.completedStoryScenes.Count(id => id == milestones[0].sceneId), Is.EqualTo(1));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestones[1].sceneId));
            Assert.That(NonStoryState(), Is.EqualTo(before));
        }

        [Test] public void SparseCatchUpSkipsAcknowledgedScenesButRetainsUnreadOrderAcrossReload()
        {
            PrepareCompletedFixture(200, false);
            var milestones = StoryCatalog.LoadChapter1().milestones.Where(m=>m.afterLevelId<=200).ToArray();
            // Model a migrated save with non-contiguous acknowledgements, not a linear playthrough.
            var acknowledged = milestones.Where((m, index) => index % 2 == 0).Select(m => m.sceneId).ToArray();
            State.Save.completedStoryScenes.AddRange(acknowledged);
            State.Persist();
            Reload();
            string before = NonStoryState();
            foreach (var milestone in milestones.Where((m, index) => index % 2 != 0)) {
                Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
                Assert.That(StoryService.ActiveSceneId, Is.EqualTo(milestone.sceneId));
                Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterMap));
                Reload();
                Assert.That(NonStoryState(), Is.EqualTo(before));
            }
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            foreach (var milestone in milestones)
                Assert.That(State.Save.completedStoryScenes.Count(id => id == milestone.sceneId), Is.EqualTo(1));
            Assert.That(State.HasCompletedChapter(1), Is.False);
        }

        [Test] public void UnlockAndLastCompletedMarkersCannotSubstituteForMilestoneCompletion()
        {
            PrepareCompletedFixture(200, true);
            // Highest-unlocked/last-completed metadata alone must not unlock a story reward route.
            var progress = State.GetLevelProgress(1, 200);
            progress.completions = 0;
            progress.rewardClaimed = false;
            progress.bestStars = 0;
            progress.failures = 1;
            State.Persist();
            Reload();
            string before = JsonUtility.ToJson(State.Save);
            Assert.That(State.Save.highestUnlockedLevel, Is.EqualTo(201));
            Assert.That(State.Save.lastCompletedLevel, Is.EqualTo(200));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
        }

        [TestCase(51)][TestCase(55)][TestCase(60)]
        public void ExplicitOutcomeLanguagesPreserveLegacyDefaultAndPlayerState(int round)
        {
            PrepareCompletedFixture(round, true);
            string before = JsonUtility.ToJson(State.Save);
            var level = ChapterLevelCatalog.Find(1, round);
            foreach (string outcome in new[] { StoryReactionOutcomes.Victory, StoryReactionOutcomes.Failure })
            {
                var data = StoryCatalog.FindReaction(round, outcome);
                var ru = StoryService.GetLocalizedReaction(level, outcome, "ru");
                var en = StoryService.GetLocalizedReaction(level, outcome, "en");
                Assert.That(en.Text, Is.EqualTo(StoryCatalog.LocalizeReaction(data, StoryCatalog.LoadDialogueLanguage("en")).Text));
                Assert.That(en.Text, Is.Not.EqualTo(ru.Text));
                Assert.That(en.Speaker, Is.Not.EqualTo(ru.Speaker));
                Assert.That(StoryService.GetLocalizedReaction(level, outcome).Text, Is.EqualTo(ru.Text));
                Assert.That(StoryService.GetLocalizedReaction(level, outcome, "unknown").Text, Is.EqualTo(ru.Text));
                Assert.That(StoryService.GetLocalizedReaction(level, outcome, "ru").Text, Is.EqualTo(ru.Text));
            }
            Assert.That(StoryService.GetLocalizedReaction(null, "victory", "en"), Is.Null);
            Assert.That(StoryService.GetLocalizedReaction(level, "unsupported", "en"), Is.Null);
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
        }

        [Test] public void Round500FinaleSurvivesInterruptionAcknowledgesOnceAndNeverRepaysRewards()
        {
            PrepareCompletedFixture(499, true);
            var level = ChapterLevelCatalog.Find(1, 500);
            Assert.That(State.SelectLevel(1, 500), Is.True);
            Assert.That(State.GetChapterCompletionPhase(1), Is.EqualTo(ChapterCompletionPhase.InProgress));
            Assert.That(State.AcknowledgeChapterCompletion(1), Is.False);
            var result = State.CompleteLevel(level, 4000);
            Assert.That(result.FirstClearRewardGranted, Is.True);
            string economyBefore = NonStoryState();
            Assert.That(State.GetChapterCompletionPhase(1), Is.EqualTo(ChapterCompletionPhase.FinalePending));
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            string scene = StoryService.ActiveSceneId;
            Assert.That(scene, Is.EqualTo("campaign.chapter01.village_departure"));
            string firstLine = StoryService.CurrentLineForLanguage("ru").Text;
            Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            Reload();
            Assert.That(State.HasCompletedChapter(1), Is.False);
            Assert.That(State.AcknowledgeChapterCompletion(1), Is.False);
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.True);
            Assert.That(StoryService.CurrentLineForLanguage("ru").Text, Is.EqualTo(firstLine));
            Assert.That(FinishDialogue(), Is.EqualTo(StoryAdvanceResult.ChapterComplete));
            Assert.That(NonStoryState(), Is.EqualTo(economyBefore));
            Reload();
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.True);
            var summary = State.GetChapterCompletionSummary(1);
            Assert.That(summary.CompletedLevels, Is.EqualTo(500));
            Assert.That(summary.TotalLevels, Is.EqualTo(500));
            Assert.That(summary.MaximumStars, Is.EqualTo(1500));
            Assert.That(State.Save.completedStoryScenes.Count(s=>s==scene), Is.EqualTo(1));
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
            Assert.That(StoryService.AcknowledgeChapterComplete(), Is.False);
            Reload();
            Assert.That(State.HasAcknowledgedChapterCompletion(1), Is.True);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.False);
            var gold = State.Save.gold; var gems = State.Save.gems;
            var boosters = ProductionChapterCompleteView.ReceivedRewards(State);
            Assert.That(State.CompleteLevel(level, 5000).FirstClearRewardGranted, Is.False);
            Reload();
            Assert.That(State.Save.gold, Is.EqualTo(gold));
            Assert.That(State.Save.gems, Is.EqualTo(gems));
            CollectionAssert.AreEqual(boosters, ProductionChapterCompleteView.ReceivedRewards(State));
            Assert.That(StoryService.TryBeginPostVictoryDialogue(), Is.False);
            Assert.That(State.SelectLevel(1, 501), Is.False);
            Assert.That(State.SelectLevel(2, 1), Is.False);
        }

        [Test] public void FinaleRequiresFullCatalogAndExplicitReadiness()
        {
            var catalog = ActiveCampaignContent.Load(true);
            var levels = catalog.LoadChapter(1);
            var story = catalog.LoadStory(1);
            foreach (var locale in new[] {"ru", "en"})
            {
                var text = catalog.LoadLocalization(1, locale);
                Assert.That(StoryCatalog.ValidateCampaign(story, text, levels, true).Errors, Is.Empty);
                Assert.That(StoryCatalog.ValidateCampaign(story, text, levels, false).Errors, Is.Not.Empty);
                var truncated = JsonUtility.FromJson<ChapterLevelCatalogData>(JsonUtility.ToJson(levels));
                truncated.levels = truncated.levels.Take(499).ToArray();
                Assert.That(StoryCatalog.ValidateCampaign(story, text, truncated, true).Errors, Is.Not.Empty);
            }
        }

        [Test] public void UnavailableRoundAndChapterCannotChangeCompletedFiveHundredSave()
        {
            PrepareCompletedFixture(500, true);
            string before = JsonUtility.ToJson(State.Save);
            Assert.That(ChapterLevelCatalog.Find(1, 501), Is.Null);
            Assert.That(ChapterLevelCatalog.Next(ChapterLevelCatalog.Find(1, 500)), Is.Null);
            Assert.That(State.SelectLevel(1, 501), Is.False);
            Assert.That(State.SelectLevel(2, 1), Is.False);
            Assert.That(State.AcknowledgeChapterCompletion(1), Is.False);
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
            Reload();
            Assert.That(JsonUtility.ToJson(State.Save), Is.EqualTo(before));
            Assert.That(State.Save.highestUnlockedLevel, Is.EqualTo(500));
        }
    }
}
