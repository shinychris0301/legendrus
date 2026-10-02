using System;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public enum CampaignRunState { None, Running, Won, Failed }

    [Serializable] public sealed class CampaignRunReceipt
    {
        public long sequence;
        public int chapter, round;
        public CampaignRunState state;
        public bool retry, acknowledged;
        public string levelJson = "";
        public int score, stars;
        public int[] goalProgress = Array.Empty<int>();
        public LevelRewardData granted = new LevelRewardData { gold = 0 };
    }

    // Local transaction boundary; result facts must come from the trusted board controller.
    // Does not instantiate a board, route a page, or validate a remote player's gameplay.
    public static class CampaignRoundLifecycle
    {
        internal static bool ValidRewards(LevelRewardData r) => r != null && r.gold >= 0 && r.gems >= 0 &&
            r.boosterMace >= 0 && r.boosterFirebird >= 0 && r.boosterLivingLight >= 0 && r.boosterSeaPearl >= 0;
        internal static bool ZeroReward(LevelRewardData r) => ValidRewards(r) && r.gold == 0 && r.gems == 0 &&
            r.boosterMace == 0 && r.boosterFirebird == 0 && r.boosterLivingLight == 0 && r.boosterSeaPearl == 0;
        internal static string Validate(CampaignPlayerSave save)
        {
            var r = save.run;
            if (r == null || save.runSequence < 0 || r.sequence != save.runSequence || !ValidRewards(r.granted) || r.goalProgress == null)
                return "campaign_run_invalid";
            if (r.state == CampaignRunState.None)
                return r.sequence == 0 && r.chapter == 0 && r.round == 0 && r.levelJson == "" && r.score == 0 && r.stars == 0 &&
                    !r.retry && !r.acknowledged && r.goalProgress.Length == 0 && ZeroReward(r.granted) ? null : "campaign_empty_run_invalid";
            if (r.sequence <= 0 || r.state < CampaignRunState.Running || r.state > CampaignRunState.Failed || r.score < 0)
                return "campaign_run_invalid";
            var level = JsonUtility.FromJson<LevelData>(r.levelJson);
            if (level == null || JsonUtility.ToJson(level) != r.levelJson || level.chapterId != r.chapter || level.levelId != r.round ||
                !ContentPipeline.CampaignContract.IsRound(r.chapter, r.round) || level.goals == null || level.goals.Length == 0 ||
                level.goals.Any(g => g == null || g.target <= 0) || !ValidRewards(level.rewards) ||
                level.starScoreThresholds == null || level.starScoreThresholds.Length != 3)
                return "campaign_run_level_invalid";
            var p = save.player.levelProgress.FirstOrDefault(x => x.chapterId == r.chapter && x.levelId == r.round);
            if (p == null || p.attempts < 1) return "campaign_run_attempt_missing";
            if (r.state == CampaignRunState.Running)
                return !r.acknowledged && r.score == 0 && r.stars == 0 && r.goalProgress.Length == 0 && ZeroReward(r.granted) &&
                    save.player.selectedChapter == r.chapter && save.player.selectedLevel == r.round ? null : "campaign_running_state_invalid";
            if (r.goalProgress.Length != level.goals.Length || r.goalProgress.Any(x => x < 0) || p.bestScore < r.score)
                return "campaign_result_invalid";
            if (r.state == CampaignRunState.Failed)
                return r.stars == 0 && p.failures > 0 && ZeroReward(r.granted) ? null : "campaign_failure_invalid";
            if (r.stars != level.EvaluateStars(r.score) || p.completions < 1 || p.bestStars < r.stars || !p.rewardClaimed ||
                r.goalProgress.Where((g, i) => g < level.goals[i].target).Any()) return "campaign_victory_invalid";
            return ZeroReward(r.granted) || JsonUtility.ToJson(r.granted) == JsonUtility.ToJson(level.rewards)
                ? null : "campaign_reward_invalid";
        }
    }

    public sealed partial class CampaignSaveSession
    {
        static CampaignSaveOperation NoChange(string reason) => new CampaignSaveOperation { Committed = true, NoOp = true, Diagnostic = reason };

        // expectedSequence is retained by the caller for retries after uncertain delivery.
        // Reusing it can never charge twice, even after the corresponding run has finished.
        public CampaignSaveOperation StartRound(long expectedSequence, long utcTicks, bool retry = false)
        {
            if (!TryLoad(out var save, out var reason)) return Blocked(reason);
            if (expectedSequence < 0 || expectedSequence == long.MaxValue || utcTicks <= 0 || utcTicks > DateTime.MaxValue.Ticks)
                return Blocked("campaign_start_invalid");
            if (save.booster.state == CampaignBoosterState.Reserved) return Blocked("campaign_booster_pending");
            var prior = save.run; var player = save.player;
            if (save.runSequence == expectedSequence + 1)
                return prior.retry == retry && prior.chapter == player.selectedChapter && prior.round == player.selectedLevel
                    ? NoChange("campaign_start_already_committed") : Blocked("campaign_start_conflict");
            if (save.runSequence != expectedSequence) return Blocked("campaign_stale_run");
            if (retry)
            {
                if (prior.state == CampaignRunState.None || prior.state == CampaignRunState.Won ||
                    prior.chapter != player.selectedChapter || prior.round != player.selectedLevel) return Blocked("campaign_retry_invalid");
            }
            else if (prior.state != CampaignRunState.None && !prior.acknowledged) return Blocked("campaign_result_pending");
            if (catalog.Progression(CampaignPlayerSaveCodec.Records(player), player.acknowledgedChapterCompletions)
                .Availability(player.selectedChapter, player.selectedLevel) != ContentPipeline.CampaignRoundAvailability.Playable)
                return Blocked("campaign_round_not_playable");
            var level = ChapterLevelCatalog.Find(catalog, player.selectedChapter, player.selectedLevel);
            if (level == null) return Blocked("campaign_content_missing");
            try
            {
                // Same 30-minute refill rule, computed once inside the start transaction.
                long now = Math.Max(utcTicks, player.lastEnergyUtcTicks);
                long interval = TimeSpan.FromMinutes(GameState.EnergyRefillMinutes).Ticks;
                long gained = (now - player.lastEnergyUtcTicks) / interval;
                if (gained > 0 && player.energy < player.energyMax)
                {
                    int add = (int)Math.Min(gained, player.energyMax - player.energy);
                    player.energy += add;
                    player.lastEnergyUtcTicks = player.energy == player.energyMax ? now : checked(player.lastEnergyUtcTicks + add * interval);
                }
                if (player.energy < 1) return Blocked("campaign_energy_required");
                if (player.energy == player.energyMax) player.lastEnergyUtcTicks = now;
                player.energy--;
                var progress = player.levelProgress.FirstOrDefault(x => x.chapterId == level.chapterId && x.levelId == level.levelId);
                if (progress == null) { progress = new LevelProgressData { chapterId = level.chapterId, levelId = level.levelId }; player.levelProgress.Add(progress); }
                if (retry && prior.state == CampaignRunState.Running) progress.failures = checked(progress.failures + 1);
                progress.attempts = checked(progress.attempts + 1);
                if (progress.attempts > 1) progress.retries = checked(progress.retries + 1);
                save.runSequence++;
                save.run = new CampaignRunReceipt { sequence = save.runSequence, chapter = level.chapterId, round = level.levelId,
                    state = CampaignRunState.Running, retry = retry, levelJson = JsonUtility.ToJson(level) };
                CampaignPlayerSaveCodec.RefreshDerivedProgress(player);
                return Commit(save);
            }
            catch (OverflowException) { return Blocked("campaign_counter_overflow"); }
        }

        public CampaignSaveOperation FinishRound(long sequence, bool won, int score, int[] goalProgress)
        {
            if (!TryLoad(out var save, out var reason)) return Blocked(reason);
            if (save.booster.state == CampaignBoosterState.Reserved) return Blocked("campaign_booster_pending");
            var r = save.run;
            if (sequence <= 0 || sequence != save.runSequence || r.state == CampaignRunState.None) return Blocked("campaign_stale_run");
            var level = JsonUtility.FromJson<LevelData>(r.levelJson);
            if (score < 0 || goalProgress == null || goalProgress.Length != level.goals.Length || goalProgress.Any(g => g < 0) ||
                (won && goalProgress.Where((g, i) => g < level.goals[i].target).Any())) return Blocked("campaign_result_invalid");
            var state = won ? CampaignRunState.Won : CampaignRunState.Failed;
            if (r.state != CampaignRunState.Running)
                return r.state == state && r.score == score && r.goalProgress.SequenceEqual(goalProgress)
                    ? NoChange("campaign_result_already_committed") : Blocked("campaign_result_conflict");
            var p = save.player; var progress = p.levelProgress.First(x => x.chapterId == r.chapter && x.levelId == r.round);
            try
            {
                r.state = state; r.score = score; r.goalProgress = (int[])goalProgress.Clone();
                progress.bestScore = Math.Max(progress.bestScore, score);
                if (won)
                {
                    progress.completions = checked(progress.completions + 1);
                    r.stars = level.EvaluateStars(score); progress.bestStars = Math.Max(progress.bestStars, r.stars);
                    if (!progress.rewardClaimed)
                    {
                        var reward = level.rewards;
                        p.gold = checked(p.gold + reward.gold); p.gems = checked(p.gems + reward.gems);
                        p.boosterMace = checked(p.boosterMace + reward.boosterMace); p.boosterFirebird = checked(p.boosterFirebird + reward.boosterFirebird);
                        p.boosterLivingLight = checked(p.boosterLivingLight + reward.boosterLivingLight); p.boosterSeaPearl = checked(p.boosterSeaPearl + reward.boosterSeaPearl);
                        progress.rewardClaimed = true;
                        r.granted = JsonUtility.FromJson<LevelRewardData>(JsonUtility.ToJson(reward));
                    }
                    p.lastCompletedChapter = r.chapter; p.lastCompletedLevel = r.round;
                    p.lastRewardGold = r.granted.gold; p.lastRewardGems = r.granted.gems;
                    // No ad/double-reward provider is part of this development transaction.
                    p.lastRewardEligibleForDouble = false; p.lastRewardDoubled = false;
                }
                else progress.failures = checked(progress.failures + 1);
                CampaignPlayerSaveCodec.RefreshDerivedProgress(p);
                return Commit(save);
            }
            catch (OverflowException) { return Blocked("campaign_counter_overflow"); }
        }

        public CampaignSaveOperation AcknowledgeResult(long sequence, bool advanceToNext = false)
        {
            if (!TryLoad(out var save, out var reason)) return Blocked(reason);
            if (sequence <= 0 || sequence != save.runSequence || save.run.state == CampaignRunState.Running || save.run.state == CampaignRunState.None)
                return Blocked("campaign_result_not_ready");
            if (save.run.acknowledged) return NoChange("campaign_result_already_acknowledged");
            // Presentation acknowledgement only: rewards committed with victory, never here.
            save.run.acknowledged = true;
            // Commit acknowledgement and selection together; a crash cannot strand an acknowledged
            // victory on the previous round. Repeated acknowledgement never moves selection again.
            if (advanceToNext && save.run.state == CampaignRunState.Won &&
                catalog.Progression(CampaignPlayerSaveCodec.Records(save.player), save.player.acknowledgedChapterCompletions)
                    .Availability(save.run.chapter, save.run.round + 1) == ContentPipeline.CampaignRoundAvailability.Playable)
            {
                save.player.selectedChapter = save.run.chapter; save.player.selectedLevel = save.run.round + 1;
                CampaignPlayerSaveCodec.RefreshDerivedProgress(save.player);
            }
            return Commit(save);
        }
    }
}
