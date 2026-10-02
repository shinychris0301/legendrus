#if UNITY_EDITOR || (UNITY_ANDROID && DEVELOPMENT_BUILD)
using System;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    // Fixed synthetic v6 profiles only. No preferences, file imports, GameState
    // instance, progression service or configurable quantity is accessed here.
    internal static class DevelopmentGameplayFixtures
    {
        internal static bool IsPreset(string command)
        {
            switch (command)
            {
                case "stock-999": case "stock-1000": case "stock-9999": case "stock-max":
                case "story-early": case "story-finale": return true;
                default: return false;
            }
        }

        internal static PlayerSaveData Create(string command, long utcTicks)
        {
            if (!IsPreset(command)) throw new ArgumentException("Unknown gameplay QA preset", nameof(command));
            // Direct validated active content avoids the normal level fallback.
            var catalog = ActiveCampaignContent.Load();
            return Create(command, utcTicks, catalog.LoadChapter(1), catalog.LoadStory(1));
        }

        internal static PlayerSaveData Create(string command, long utcTicks,
            ChapterLevelCatalogData levels, ChapterStoryCatalogData story)
        {
            if (!IsPreset(command)) throw new ArgumentException("Unknown gameplay QA preset", nameof(command));
            if (utcTicks <= 0 || utcTicks > DateTime.MaxValue.Ticks) throw new ArgumentOutOfRangeException(nameof(utcTicks));
            if (levels?.levels == null || levels.chapterId != 1 || levels.levels.Length != 500 ||
                levels.levels.Where((level, index) => level == null || level.chapterId != 1 || level.levelId != index + 1).Any() ||
                story?.scenes == null || story.chapterId != 1 || story.scenes.Length != 58 ||
                story.milestones == null || story.milestones.Length != 57 || story.milestones.Any(m => m == null) || story.completion == null ||
                story.completion.finalLevelId != 500 || story.milestones.Last().afterLevelId != 500 ||
                story.milestones.Last().sceneId != story.completion.finaleSceneId)
                throw new InvalidOperationException("Gameplay QA presets require the installed complete Chapter 1");
            var ids = story.scenes.Select(scene => scene?.id).ToArray();
            if (ids.Any(string.IsNullOrWhiteSpace) || ids.Distinct(StringComparer.Ordinal).Count() != 58 ||
                !ids.Contains(story.introSceneId) || story.scenes.Any(scene => scene.lines == null || scene.lines.Length == 0))
                throw new InvalidOperationException("Gameplay QA story catalogue is incomplete");
            int previous = 0;
            foreach (var milestone in story.milestones)
            {
                if (milestone == null || milestone.afterLevelId <= previous || milestone.afterLevelId > 500 ||
                    !ids.Contains(milestone.sceneId)) throw new InvalidOperationException("Gameplay QA milestones are invalid");
                var scene = story.scenes.Single(entry => entry.id == milestone.sceneId);
                if (!levels.levels[milestone.afterLevelId - 1].HasStoryTrigger(scene.trigger))
                    throw new InvalidOperationException("Gameplay QA story and levels do not agree");
                previous = milestone.afterLevelId;
            }

            var player = new PlayerSaveData { energy = 4, lastEnergyUtcTicks = utcTicks };
            int quantity;
            switch (command)
            {
                case "stock-999": quantity = 999; break;
                case "stock-1000": quantity = 1000; break;
                case "stock-9999": quantity = 9999; break;
                case "stock-max": quantity = int.MaxValue; break;
                default: quantity = -1; break;
            }
            if (quantity >= 0)
            {
                // stock-max is a display/consumption fixture, not a reward-cap override.
                player.boosterMace = player.boosterFirebird = player.boosterLivingLight = player.boosterSeaPearl = quantity;
                return player;
            }

            bool finale = command == "story-finale";
            int completed = finale ? 500 : 8;
            if (!finale && story.milestones[1].afterLevelId > completed)
                throw new InvalidOperationException("Gameplay QA early scenes require completed rounds");
            checked
            {
                foreach (var level in levels.levels.Take(completed))
                {
                    int stars = level.EvaluateStars(int.MaxValue);
                    if (stars != 3) throw new InvalidOperationException("Gameplay QA star thresholds are invalid");
                    player.levelProgress.Add(new LevelProgressData { chapterId = 1, levelId = level.levelId,
                        bestScore = int.MaxValue, bestStars = stars, attempts = 1, completions = 1, rewardClaimed = true });
                    var reward = level.rewards ?? new LevelRewardData();
                    player.gold += Math.Max(0, reward.gold); player.gems += Math.Max(0, reward.gems);
                    player.boosterMace += Math.Max(0, reward.boosterMace); player.boosterFirebird += Math.Max(0, reward.boosterFirebird);
                    player.boosterLivingLight += Math.Max(0, reward.boosterLivingLight); player.boosterSeaPearl += Math.Max(0, reward.boosterSeaPearl);
                    player.stars += stars;
                    player.lastCompletedChapter = 1; player.lastCompletedLevel = level.levelId;
                    player.lastRewardGold = Math.Max(0, reward.gold); player.lastRewardGems = Math.Max(0, reward.gems);
                    player.lastRewardEligibleForDouble = player.lastRewardGold > 0;
                }
            }
            player.storyStage = completed;
            player.selectedLevel = player.highestUnlockedLevel = player.level = Math.Min(500, completed + 1);
            player.completedStoryScenes.Add(story.introSceneId);
            foreach (var milestone in story.milestones.Take(finale ? 57 : 2)) player.completedStoryScenes.Add(milestone.sceneId);
            if (finale) player.acknowledgedChapterCompletions.Add(1);
            return player;
        }
    }
}
#endif
