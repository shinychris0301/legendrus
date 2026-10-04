using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    public enum BoosterType
    {
        BogatyrMace,
        Firebird,
        LivingLight,
        SeaPearl
    }

    [Serializable]
    public sealed class BoosterCommandReceipt
    {
        public int schemaVersion = 1;
        public string commandId = "";
        public BoosterType type;
        public CampaignBoosterState state;

        internal bool IsValid => schemaVersion == 1 && CampaignBoosterTransactions.Known(type) &&
            (state == CampaignBoosterState.None ? string.IsNullOrEmpty(commandId) :
            state >= CampaignBoosterState.Reserved && state <= CampaignBoosterState.Refunded &&
            Guid.TryParseExact(commandId, "N", out _));
    }

    [Serializable]
    public sealed class LevelProgressData
    {
        public int chapterId = 1;
        public int levelId = 1;
        public int bestStars;
        public int bestScore;
        public int attempts;
        public int retries;
        public int failures;
        public int completions;
        public bool rewardClaimed;
    }

    public sealed class LevelCompletionResult
    {
        public int Score { get; }
        public LevelData Level { get; }
        public int Stars { get; }
        public int GoldReward { get; }
        public int GemReward { get; }
        public bool FirstClearRewardGranted { get; }

        public LevelCompletionResult(
            LevelData level,
            int stars,
            int goldReward,
            int gemReward,
            bool firstClearRewardGranted,
            int score = 0
        )
        {
            Level = level;
            Stars = stars;
            GoldReward = goldReward;
            GemReward = gemReward;
            FirstClearRewardGranted = firstClearRewardGranted;
            Score = Mathf.Max(0, score);
        }
    }

    public enum ChapterCompletionPhase
    {
        InProgress,
        FinalePending,
        PresentationPending,
        Acknowledged
    }

    public sealed class ChapterCompletionSummary
    {
        public int ChapterId { get; }
        public int CompletedLevels { get; }
        public int TotalLevels { get; }
        public int EarnedStars { get; }
        public int MaximumStars { get; }
        public ChapterCompletionPhase Phase { get; }

        public ChapterCompletionSummary(
            int chapterId,
            int completedLevels,
            int totalLevels,
            int earnedStars,
            int maximumStars,
            ChapterCompletionPhase phase
        )
        {
            ChapterId = chapterId;
            CompletedLevels = completedLevels;
            TotalLevels = totalLevels;
            EarnedStars = earnedStars;
            MaximumStars = maximumStars;
            Phase = phase;
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        public int saveVersion = 6;

        public int gold = 12540;
        public int gems = 350;

        public int energy = 5;
        public int energyMax = 5;
        public long lastEnergyUtcTicks;

        public int chapter = 1;
        public int level = 1;
        public int stars = 0;
        public int storyStage = 0;

        public int selectedChapter = 1;
        public int selectedLevel = 1;
        public int highestUnlockedLevel = 1;
        public int lastCompletedChapter;
        public int lastCompletedLevel;
        public int lastRewardGold;
        public int lastRewardGems;
        public bool lastRewardEligibleForDouble;
        public bool lastRewardDoubled;
        public List<LevelProgressData> levelProgress = new List<LevelProgressData>();
        public List<string> completedTutorialSteps = new List<string>();
        public List<string> completedStoryScenes = new List<string>();
        public List<int> acknowledgedChapterCompletions = new List<int>();

        public int boosterMace = 7;
        public int boosterFirebird = 5;
        public int boosterLivingLight = 8;
        public int boosterSeaPearl = 10;

        public string lastDailyRewardUtc = "";
        public int dailyRewardCycleDay;
        public string dailyRewardMonthUtc = "";
        public int dailyRewardMonthClaims;

        public bool music = true;
        public bool sfx = true;
        public bool vibration = true;
        public bool notifications = true;
        // Optional v6 extension with its own schema; legacy saves simply have no retained attempt.
        public RetainedRoundData retainedRound;
        public List<ContinuationTransactionData> continuationTransactions = new List<ContinuationTransactionData>();
        // Optional v6 extension. One durable receipt protects the ordinary board's in-flight booster.
        public BoosterCommandReceipt boosterCommand = new BoosterCommandReceipt();
    }

    public sealed partial class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        public const int EnergyRefillMinutes = 30;

        public PlayerSaveData Save = new PlayerSaveData();
        public event Action Changed;

        public PlayerSaveLoadResult LastLoadResult { get; private set; }
        // Current-session outcome, distinct from the saved personal best on replays.
        public LevelCompletionResult LastCompletionResult { get; private set; }
        public LevelFailureResult LastFailureResult { get; private set; }
        public PlayerSaveWriteResult LastWriteResult { get; private set; }
        public bool CanPersist
        {
            get { return canPersist; }
        }

        float nextEnergyRefresh;
        bool saveRewriteRequired;
        bool canPersist = true;

        void Awake()
        {
            Instance = this;
            LastLoadResult = SaveService.LoadWithResult();
            Save = LastLoadResult.Data ?? new PlayerSaveData();
            saveRewriteRequired = LastLoadResult.RequiresRewrite;
            canPersist = LastLoadResult.CanPersist;
            if (LastLoadResult.CanPersist)
            {
                MigrateSave();
                TryRecoverPendingBoosterCommand();
            }
            else
            {
                Debug.LogError(
                    "Player save persistence locked: " +
                    LastLoadResult.Status + ":" +
                    LastLoadResult.DiagnosticCode
                );
            }
            RefreshEnergyFromClock();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (Time.unscaledTime < nextEnergyRefresh) return;
            nextEnergyRefresh = Time.unscaledTime + 5f;
            RefreshEnergyFromClock();
        }

        void MigrateSave()
        {
            if (Save == null)
                Save = new PlayerSaveData();

            string sourceJson = JsonUtility.ToJson(Save);

            PlayerSaveMigrationResult migration =
                PlayerSaveMigrationRegistry.Migrate(
                    Save,
                    BuildLegacyProgress,
                    DateTime.UtcNow.Ticks
                );
            if (!migration.Succeeded || migration.Data == null)
            {
                Debug.LogError(
                    "Player save migration blocked: " +
                    (migration != null
                        ? migration.ReasonCode
                        : PlayerSaveMigrationRegistry.SourceInvalid)
                );
                return;
            }
            Save = migration.Data;
            // Migration clones through Unity JSON, which may materialize an empty optional object.
            if (Save.retainedRound != null && string.IsNullOrEmpty(Save.retainedRound.attemptId))
                Save.retainedRound = null;

            NormalizeLevelProgress();
            NormalizeTutorialProgress();
            NormalizeStoryProgress();
            NormalizeChapterCompletionProgress();

            string normalizedJson = JsonUtility.ToJson(Save);
            if (saveRewriteRequired
                || migration.Changed
                || !string.Equals(
                    sourceJson,
                    normalizedJson,
                    StringComparison.Ordinal))
            {
                Persist();
            }
            saveRewriteRequired = false;
        }

        List<LevelProgressData> BuildLegacyProgress(PlayerSaveData legacy)
        {
            var migrated = new List<LevelProgressData>();
            if (legacy == null)
                return migrated;

            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1();
            int levelCount = catalog.levels != null ? catalog.levels.Length : 1;
            int completed = Mathf.Clamp(legacy.level - 1, 0, levelCount);
            int starsLeft = Mathf.Max(0, legacy.stars);

            for (int levelId = 1; levelId <= completed; levelId++)
            {
                int bestStars = Mathf.Clamp(starsLeft, 1, 3);
                starsLeft = Mathf.Max(0, starsLeft - bestStars);
                migrated.Add(new LevelProgressData
                {
                    chapterId = catalog.chapterId,
                    levelId = levelId,
                    bestStars = bestStars,
                    attempts = 1,
                    completions = 1,
                    rewardClaimed = true
                });
            }
            return migrated;
        }

        void NormalizeLevelProgress()
        {
            if (Save.levelProgress == null)
                Save.levelProgress = new List<LevelProgressData>();

            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1();
            int maxLevel = 1;
            if (catalog.levels != null && catalog.levels.Length > 0)
                maxLevel = catalog.levels[catalog.levels.Length - 1].levelId;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = Save.levelProgress.Count - 1; i >= 0; i--)
            {
                LevelProgressData progress = Save.levelProgress[i];
                string key = progress != null
                    ? $"{progress.chapterId}:{progress.levelId}"
                    : string.Empty;
                if (
                    progress == null ||
                    ChapterLevelCatalog.Find(progress.chapterId, progress.levelId) == null ||
                    !seen.Add(key)
                )
                {
                    Save.levelProgress.RemoveAt(i);
                    continue;
                }

                progress.bestStars = Mathf.Clamp(progress.bestStars, 0, 3);
                progress.bestScore = Mathf.Max(0, progress.bestScore);
                progress.attempts = Mathf.Max(0, progress.attempts);
                progress.retries = Mathf.Clamp(progress.retries, 0, progress.attempts);
                progress.failures = Mathf.Clamp(progress.failures, 0, progress.attempts);
                progress.completions = Mathf.Max(0, progress.completions);
                if (progress.completions > 0)
                {
                    Save.highestUnlockedLevel = Mathf.Max(
                        Save.highestUnlockedLevel,
                        Mathf.Min(maxLevel, progress.levelId + 1)
                    );
                }
            }

            Save.selectedChapter = catalog.chapterId;
            Save.highestUnlockedLevel = Mathf.Clamp(
                Mathf.Max(1, Save.highestUnlockedLevel),
                1,
                maxLevel
            );
            Save.selectedLevel = Mathf.Clamp(
                Mathf.Max(1, Save.selectedLevel),
                1,
                Save.highestUnlockedLevel
            );

            Save.chapter = Save.selectedChapter;
            Save.level = Save.highestUnlockedLevel;
            Save.stars = CalculateTotalStars();
        }

        void NormalizeTutorialProgress()
        {
            if (Save.completedTutorialSteps == null)
                Save.completedTutorialSteps = new List<string>();

            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = Save.completedTutorialSteps.Count - 1; i >= 0; i--)
            {
                string tutorialId = Save.completedTutorialSteps[i];
                if (string.IsNullOrWhiteSpace(tutorialId) || !seen.Add(tutorialId))
                    Save.completedTutorialSteps.RemoveAt(i);
            }
        }

        void NormalizeStoryProgress()
        {
            if (Save.completedStoryScenes == null)
                Save.completedStoryScenes = new List<string>();
            StoryProgress.Normalize(Save.completedStoryScenes);
        }

        void NormalizeChapterCompletionProgress()
        {
            if (Save.acknowledgedChapterCompletions == null)
                Save.acknowledgedChapterCompletions = new List<int>();

            var seen = new HashSet<int>();
            for (int i = Save.acknowledgedChapterCompletions.Count - 1; i >= 0; i--)
            {
                int chapterId = Save.acknowledgedChapterCompletions[i];
                if (chapterId <= 0 || !seen.Add(chapterId))
                    Save.acknowledgedChapterCompletions.RemoveAt(i);
            }

            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (
                catalog != null &&
                Save.acknowledgedChapterCompletions.Contains(catalog.chapterId) &&
                !ChapterCompletionPrerequisitesMet(catalog)
            )
            {
                Save.acknowledgedChapterCompletions.Remove(catalog.chapterId);
            }

            Save.acknowledgedChapterCompletions.Sort();
        }

        public void RefreshEnergyFromClock()
        {
            if (Save.energy >= Save.energyMax)
            {
                Save.energy = Save.energyMax;
                Save.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
                return;
            }

            long now = DateTime.UtcNow.Ticks;
            long interval = TimeSpan.FromMinutes(EnergyRefillMinutes).Ticks;
            long elapsed = Math.Max(0, now - Save.lastEnergyUtcTicks);
            int gained = (int)(elapsed / interval);
            if (gained <= 0) return;

            Save.energy = Mathf.Min(Save.energyMax, Save.energy + gained);
            if (Save.energy >= Save.energyMax)
                Save.lastEnergyUtcTicks = now;
            else
                Save.lastEnergyUtcTicks += gained * interval;

            Persist();
        }

        public TimeSpan EnergyRemaining()
        {
            if (Save.energy >= Save.energyMax) return TimeSpan.Zero;

            long interval = TimeSpan.FromMinutes(EnergyRefillMinutes).Ticks;
            long now = DateTime.UtcNow.Ticks;
            long elapsed = Math.Max(0, now - Save.lastEnergyUtcTicks);
            long left = interval - (elapsed % interval);
            return TimeSpan.FromTicks(Math.Max(0, left));
        }

        public bool SpendEnergy(int amount = 1)
        {
            if (!canPersist || amount < 0) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            long now = DateTime.UtcNow.Ticks;
            if (candidate.energy >= candidate.energyMax)
            {
                candidate.energy = candidate.energyMax;
                candidate.lastEnergyUtcTicks = now;
            }
            else
            {
                long interval = TimeSpan.FromMinutes(EnergyRefillMinutes).Ticks;
                int gained = (int)(Math.Max(0, now - candidate.lastEnergyUtcTicks) / interval);
                if (gained > 0)
                {
                    candidate.energy = Mathf.Min(candidate.energyMax, candidate.energy + gained);
                    candidate.lastEnergyUtcTicks = candidate.energy >= candidate.energyMax
                        ? now : candidate.lastEnergyUtcTicks + gained * interval;
                }
            }
            if (candidate.energy < amount) return false;
            bool wasFull = candidate.energy >= candidate.energyMax;
            candidate.energy -= amount;
            if (wasFull)
                candidate.lastEnergyUtcTicks = now;
            return TryCommitBalance(candidate);
        }

        public void AddEnergy(int amount)
        {
            Save.energy = Mathf.Clamp(Save.energy + Mathf.Max(0, amount), 0, Save.energyMax);
            if (Save.energy >= Save.energyMax)
                Save.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
            Persist();
        }

        public void RefillEnergy()
        {
            Save.energy = Save.energyMax;
            Save.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
            Persist();
        }

        // Compatibility entry points retain their void contract. Callers that need
        // confirmation of a full, committed credit must use the TryAdd APIs.
        public void AddGold(int amount) => TryAddGold(amount);

        public bool TryAddGold(int amount) => TryCredit(amount,
            (candidate, credit) => candidate.gold = checked(candidate.gold + credit));

        bool TryCredit(int amount, Action<PlayerSaveData, int> apply)
        {
            if (!canPersist) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            try { apply(candidate, Mathf.Max(0, amount)); }
            catch (OverflowException) { return false; }

            return TryCommitBalance(candidate);
        }

        bool TryCommitBalance(PlayerSaveData candidate)
        {
            if (!canPersist) return false;
            bool written = SaveService.TrySave(candidate, out var result);
            LastWriteResult = result;
            if (!written && !result.PrimaryDurablyCommitted)
            {
                canPersist = false;
                return false;
            }

            // A committed primary is already a credit even if journal cleanup
            // failed. Publish it once and block later writes until recovery.
            Save = candidate;
            canPersist = written;
            Changed?.Invoke();
            return true;
        }

        public bool SpendGold(int amount)
        {
            if (!canPersist || amount < 0 || Save.gold < amount) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            candidate.gold -= amount;
            return TryCommitBalance(candidate);
        }

        public void AddGems(int amount) => TryAddGems(amount);

        public bool TryAddGems(int amount) => TryCredit(amount,
            (candidate, credit) => candidate.gems = checked(candidate.gems + credit));

        public bool SpendGems(int amount)
        {
            if (!canPersist || amount < 0 || Save.gems < amount) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            candidate.gems -= amount;
            return TryCommitBalance(candidate);
        }

        public bool IsLevelUnlocked(int chapterId, int levelId)
        {
            return
                chapterId == Save.selectedChapter &&
                levelId >= 1 &&
                levelId <= Save.highestUnlockedLevel;
        }

        public bool SelectLevel(int chapterId, int levelId)
        {
            if (ContinuationBlocksNewAttempt()) return false;
            if (TryGetRetainedRound(out var retained) && (retained.chapter != chapterId || retained.level != levelId)) return false;
            if (!IsLevelUnlocked(chapterId, levelId)) return false;
            if (ChapterLevelCatalog.Find(chapterId, levelId) == null) return false;

            Save.selectedChapter = chapterId;
            Save.selectedLevel = levelId;
            Persist();
            return true;
        }

        public void EnsureSelectedLevelIsUnlocked()
        {
            NormalizeLevelProgress();
            Persist();
        }

        public LevelProgressData GetLevelProgress(
            int chapterId,
            int levelId,
            bool create = false
        )
        {
            if (Save.levelProgress == null)
                Save.levelProgress = new List<LevelProgressData>();

            for (int i = 0; i < Save.levelProgress.Count; i++)
            {
                LevelProgressData progress = Save.levelProgress[i];
                if (
                    progress != null &&
                    progress.chapterId == chapterId &&
                    progress.levelId == levelId
                )
                {
                    return progress;
                }
            }

            if (!create) return null;

            var created = new LevelProgressData
            {
                chapterId = chapterId,
                levelId = levelId
            };
            Save.levelProgress.Add(created);
            return created;
        }

        public int RegisterLevelAttempt(LevelData levelData)
        {
            if (levelData == null) return 0;
            if (ContinuationBlocksNewAttempt()) throw new InvalidOperationException("Resolve or recover the continuation reservation before starting another round.");
            if (TryGetRetainedRound(out _)) throw new InvalidOperationException("Resolve the retained attempt before starting another round.");
            Save.retainedRound = null; // Reject stale/incompatible optional checkpoints without resetting progress.
            unsavedRetainedRound = null;
            if (!IsLevelUnlocked(levelData.chapterId, levelData.levelId))
                return 0;

            Save.selectedChapter = levelData.chapterId;
            Save.selectedLevel = levelData.levelId;
            LevelProgressData progress = GetLevelProgress(
                levelData.chapterId,
                levelData.levelId,
                create: true
            );
            progress.attempts++;
            if (progress.attempts > 1)
                progress.retries++;

            Persist();
            return progress.attempts;
        }

        public void RecordLevelFailure(LevelData levelData, int score, IReadOnlyList<int> goalProgress = null)
        {
            if (ContinuationBlocksNewAttempt()) throw new InvalidOperationException("Finalize the retained continuation transaction first.");
            if (levelData == null) return;
            if (!IsLevelUnlocked(levelData.chapterId, levelData.levelId))
                return;

            LevelProgressData progress = GetLevelProgress(
                levelData.chapterId,
                levelData.levelId,
                create: true
            );
            if (progress.attempts == 0)
                progress.attempts = 1;
            LastFailureResult = new LevelFailureResult(levelData, score, goalProgress);
            progress.failures++;
            progress.bestScore = Mathf.Max(progress.bestScore, score);
            Persist();
        }

        public LevelCompletionResult CompleteLevel(LevelData levelData, int score)
        {
            if (!TryCompleteLevel(levelData, score, out var completion))
                throw new InvalidOperationException("Level completion could not be committed.");
            return completion;
        }

        // Progress and the entire first-clear grant belong to the same journal commit.
        public bool TryCompleteLevel(LevelData levelData, int score, out LevelCompletionResult completion)
        {
            completion = null;
            if (ContinuationBlocksNewAttempt()) throw new InvalidOperationException("Continuation consumption is not integrated yet.");
            if (levelData == null)
                throw new ArgumentNullException(nameof(levelData));
            if (!IsLevelUnlocked(levelData.chapterId, levelData.levelId))
            {
                throw new InvalidOperationException(
                    $"Cannot complete locked level {levelData.chapterId}:{levelData.levelId}."
                );
            }

            if (!canPersist || HasPendingBoosterCommand) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (candidate.levelProgress == null) candidate.levelProgress = new List<LevelProgressData>();
            LevelProgressData progress = candidate.levelProgress.Find(p => p != null &&
                p.chapterId == levelData.chapterId && p.levelId == levelData.levelId);
            if (progress == null)
            {
                progress = new LevelProgressData { chapterId = levelData.chapterId, levelId = levelData.levelId };
                candidate.levelProgress.Add(progress);
            }
            if (progress.attempts == 0)
                progress.attempts = 1;

            int earnedStars = levelData.EvaluateStars(score);
            progress.bestScore = Mathf.Max(progress.bestScore, score);
            progress.bestStars = Mathf.Max(progress.bestStars, earnedStars);

            LevelRewardData rewards = levelData.rewards ?? new LevelRewardData();
            bool grantReward = !progress.rewardClaimed;
            int grantedGold = grantReward ? Mathf.Max(0, rewards.gold) : 0;
            int grantedGems = grantReward ? Mathf.Max(0, rewards.gems) : 0;

            try
            {
                progress.completions = checked(progress.completions + 1);
                if (grantReward)
                {
                    candidate.gold = checked(candidate.gold + grantedGold);
                    candidate.gems = checked(candidate.gems + grantedGems);
                    candidate.boosterMace = checked(candidate.boosterMace + Mathf.Max(0, rewards.boosterMace));
                    candidate.boosterFirebird = checked(candidate.boosterFirebird + Mathf.Max(0, rewards.boosterFirebird));
                    candidate.boosterLivingLight = checked(candidate.boosterLivingLight + Mathf.Max(0, rewards.boosterLivingLight));
                    candidate.boosterSeaPearl = checked(candidate.boosterSeaPearl + Mathf.Max(0, rewards.boosterSeaPearl));
                    progress.rewardClaimed = true;
                }
            }
            catch (OverflowException) { return false; }

            candidate.lastCompletedChapter = levelData.chapterId;
            candidate.lastCompletedLevel = levelData.levelId;
            candidate.lastRewardGold = grantedGold;
            candidate.lastRewardGems = grantedGems;
            candidate.lastRewardEligibleForDouble = grantReward && grantedGold > 0;
            candidate.lastRewardDoubled = false;
            candidate.storyStage = Mathf.Max(candidate.storyStage, levelData.levelId);

            LevelData next = ChapterLevelCatalog.Next(levelData);
            if (next != null)
            {
                candidate.highestUnlockedLevel = Mathf.Max(
                    candidate.highestUnlockedLevel,
                    next.levelId
                );
                candidate.selectedChapter = next.chapterId;
                candidate.selectedLevel = next.levelId;
            }
            else
            {
                candidate.highestUnlockedLevel = Mathf.Max(
                    candidate.highestUnlockedLevel,
                    levelData.levelId
                );
                candidate.selectedChapter = levelData.chapterId;
                candidate.selectedLevel = levelData.levelId;
            }

            candidate.chapter = candidate.selectedChapter;
            candidate.level = candidate.highestUnlockedLevel;
            candidate.stars = CalculateTotalStars(candidate);
            var committedCompletion = new LevelCompletionResult(
                levelData,
                earnedStars,
                grantedGold,
                grantedGems,
                grantReward,
                score
            );
            bool written = SaveService.TrySave(candidate, out var write);
            LastWriteResult = write;
            if (!written && !write.PrimaryDurablyCommitted) { canPersist = false; return false; }
            Save = candidate;
            canPersist = written;
            completion = LastCompletionResult = committedCompletion;
            Changed?.Invoke();
            return true;
        }

        public bool TryDoubleLastLevelGoldReward()
        {
            if (
                !canPersist ||
                !Save.lastRewardEligibleForDouble ||
                Save.lastRewardDoubled ||
                Save.lastRewardGold <= 0
            )
            {
                return false;
            }

            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            try
            {
                candidate.gold = checked(candidate.gold + candidate.lastRewardGold);
            }
            catch (OverflowException)
            {
                // Preserve the eligible reward until the full credit fits; never publish a partial grant.
                return false;
            }
            candidate.lastRewardDoubled = true;
            PlayerSaveWriteResult result;
            bool written = SaveService.TrySave(candidate, out result);
            LastWriteResult = result;
            if (!written) { canPersist = false; return false; }
            Save = candidate;
            Changed?.Invoke();
            return true;
        }

        public int CalculateTotalStars()
        {
            return CalculateTotalStars(Save);
        }

        static int CalculateTotalStars(PlayerSaveData save)
        {
            if (save.levelProgress == null) return 0;

            int total = 0;
            for (int i = 0; i < save.levelProgress.Count; i++)
            {
                LevelProgressData progress = save.levelProgress[i];
                if (progress != null)
                    total += Mathf.Clamp(progress.bestStars, 0, 3);
            }

            return total;
        }

        public void WinLevel(int starCount, int goldReward)
        {
            LevelData selected = ChapterLevelCatalog.ResolveSelected(Save);
            int index = Mathf.Clamp(starCount, 1, 3) - 1;
            int score = selected.starScoreThresholds != null && selected.starScoreThresholds.Length == 3
                ? selected.starScoreThresholds[index]
                : selected.startingScore + 1;
            CompleteLevel(selected, score);
        }

        public bool TryUseBooster(BoosterType type)
        {
            if (!canPersist || !CampaignBoosterTransactions.Known(type) || BoosterCount(type) < 1) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            CampaignBoosterTransactions.Add(candidate, type, -1);
            return TryCommitBalance(candidate);
        }

        public bool HasPendingBoosterCommand => Save.boosterCommand != null &&
            Save.boosterCommand.state == CampaignBoosterState.Reserved;

        public bool TryBeginBoosterCommand(BoosterType type, out string commandId)
        {
            commandId = null;
            if (!canPersist || HasPendingBoosterCommand || !CampaignBoosterTransactions.Known(type) || BoosterCount(type) < 1) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            CampaignBoosterTransactions.Add(candidate, type, -1);
            candidate.boosterCommand = new BoosterCommandReceipt
            { commandId = Guid.NewGuid().ToString("N"), type = type, state = CampaignBoosterState.Reserved };
            if (!TryCommitBalance(candidate)) return false;
            commandId = candidate.boosterCommand.commandId;
            return true;
        }

        public bool TrySettleBoosterCommand(string commandId, bool applied)
        {
            var receipt = Save.boosterCommand;
            if (receipt == null || !receipt.IsValid || string.IsNullOrEmpty(commandId) || receipt.commandId != commandId) return false;
            var target = applied ? CampaignBoosterState.Applied : CampaignBoosterState.Refunded;
            if (receipt.state == target) return true;
            if (!canPersist || receipt.state != CampaignBoosterState.Reserved) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            try { if (!applied) CampaignBoosterTransactions.Add(candidate, receipt.type, 1); }
            catch (OverflowException) { return false; }
            candidate.boosterCommand.state = target;
            return TryCommitBalance(candidate);
        }

        // Call after the owning board is lost. A reserved effect has no committed round result.
        public bool TryRecoverPendingBoosterCommand() => !HasPendingBoosterCommand ||
            TrySettleBoosterCommand(Save.boosterCommand.commandId, applied: false);

        public void AddBooster(BoosterType type, int amount = 1) => TryAddBooster(type, amount);

        public bool TryAddBooster(BoosterType type, int amount = 1)
        {
            Action<PlayerSaveData, int> apply;
            switch (type)
            {
                case BoosterType.BogatyrMace:
                    apply = (candidate, credit) => candidate.boosterMace = checked(candidate.boosterMace + credit);
                    break;
                case BoosterType.Firebird:
                    apply = (candidate, credit) => candidate.boosterFirebird = checked(candidate.boosterFirebird + credit);
                    break;
                case BoosterType.LivingLight:
                    apply = (candidate, credit) => candidate.boosterLivingLight = checked(candidate.boosterLivingLight + credit);
                    break;
                case BoosterType.SeaPearl:
                    apply = (candidate, credit) => candidate.boosterSeaPearl = checked(candidate.boosterSeaPearl + credit);
                    break;
                default:
                    return false;
            }
            if (!canPersist) return false;
            if (amount <= 0) return true;
            return TryCredit(amount, apply);
        }

        public int BoosterCount(BoosterType type)
        {
            switch (type)
            {
                case BoosterType.BogatyrMace: return Save.boosterMace;
                case BoosterType.Firebird: return Save.boosterFirebird;
                case BoosterType.LivingLight: return Save.boosterLivingLight;
                case BoosterType.SeaPearl: return Save.boosterSeaPearl;
                default: return 0;
            }
        }

        public bool ClaimDailyReward() => ClaimDailyReward(out _);

        // A receipt contains only committed balance deltas, including capped energy.
        // The confirmation UI never applies this receipt to the save a second time.
        public bool ClaimDailyReward(out DailyRewardGrant granted)
        {
            granted = default;
            DateTime now = DateTime.UtcNow;
            if (!canPersist || !DailyRewardSchedule.CanClaim(Save, now)) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            try
            {
                if (!DailyRewardSchedule.TryApply(candidate, now)) return false;
            }
            catch (OverflowException)
            {
                // Reject the candidate before writing or publishing a receipt; keep the day retryable.
                return false;
            }
            PlayerSaveWriteResult result;
            bool written = SaveService.TrySave(candidate, out result);
            LastWriteResult = result;
            if (!written) { canPersist = false; return false; }
            granted = new DailyRewardGrant(candidate.gold-Save.gold, candidate.gems-Save.gems,
                candidate.energy-Save.energy, candidate.boosterMace-Save.boosterMace,
                candidate.boosterFirebird-Save.boosterFirebird, candidate.boosterLivingLight-Save.boosterLivingLight,
                candidate.boosterSeaPearl-Save.boosterSeaPearl);
            Save = candidate;
            Changed?.Invoke();
            return true;
        }

        public bool HasCompletedTutorial(string tutorialId)
        {
            if (string.IsNullOrEmpty(tutorialId) || Save.completedTutorialSteps == null)
                return false;

            return Save.completedTutorialSteps.Contains(tutorialId);
        }

        public bool CompleteTutorial(string tutorialId)
        {
            if (string.IsNullOrWhiteSpace(tutorialId)) return false;
            if (Save.completedTutorialSteps == null)
                Save.completedTutorialSteps = new List<string>();
            if (Save.completedTutorialSteps.Contains(tutorialId)) return false;

            Save.completedTutorialSteps.Add(tutorialId);
            Persist();
            return true;
        }

        public bool HasCompletedStoryScene(string sceneId)
        {
            return StoryProgress.HasCompleted(Save.completedStoryScenes, sceneId);
        }

        public bool CompleteStoryScene(string sceneId)
        {
            if (Save.completedStoryScenes == null)
                Save.completedStoryScenes = new List<string>();
            if (!StoryProgress.Complete(Save.completedStoryScenes, sceneId))
                return false;

            Persist();
            return true;
        }

        public ChapterCompletionPhase GetChapterCompletionPhase(int chapterId)
        {
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (catalog == null || catalog.chapterId != chapterId)
                return ChapterCompletionPhase.InProgress;

            StoryChapterCompletionData completion = catalog.completion;
            LevelProgressData finalProgress = completion != null
                ? GetLevelProgress(chapterId, completion.finalLevelId)
                : null;
            if (finalProgress == null || finalProgress.completions <= 0)
                return ChapterCompletionPhase.InProgress;
            if (!HasCompletedStoryScene(completion.finaleSceneId))
                return ChapterCompletionPhase.FinalePending;
            if (HasAcknowledgedChapterCompletion(chapterId))
                return ChapterCompletionPhase.Acknowledged;

            return ChapterCompletionPhase.PresentationPending;
        }

        public bool HasCompletedChapter(int chapterId)
        {
            ChapterCompletionPhase phase = GetChapterCompletionPhase(chapterId);
            return
                phase == ChapterCompletionPhase.PresentationPending ||
                phase == ChapterCompletionPhase.Acknowledged;
        }

        public bool HasPendingChapterCompletion(int chapterId)
        {
            return GetChapterCompletionPhase(chapterId) == ChapterCompletionPhase.PresentationPending;
        }

        public bool HasAcknowledgedChapterCompletion(int chapterId)
        {
            return
                chapterId > 0 &&
                Save.acknowledgedChapterCompletions != null &&
                Save.acknowledgedChapterCompletions.Contains(chapterId);
        }

        public bool AcknowledgeChapterCompletion(int chapterId)
        {
            if (GetChapterCompletionPhase(chapterId) != ChapterCompletionPhase.PresentationPending)
                return false;

            if (Save.acknowledgedChapterCompletions == null)
                Save.acknowledgedChapterCompletions = new List<int>();
            if (Save.acknowledgedChapterCompletions.Contains(chapterId))
                return false;

            Save.acknowledgedChapterCompletions.Add(chapterId);
            Save.acknowledgedChapterCompletions.Sort();
            Persist();
            return true;
        }

        public ChapterCompletionSummary GetChapterCompletionSummary(int chapterId)
        {
            ChapterLevelCatalogData levels = ChapterLevelCatalog.LoadChapter1();
            if (levels?.levels == null || levels.chapterId != chapterId)
                return null;

            int completedLevels = 0;
            int earnedStars = 0;
            int totalLevels = 0;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                LevelData level = levels.levels[i];
                if (level == null) continue;

                totalLevels++;
                LevelProgressData progress = GetLevelProgress(chapterId, level.levelId);
                if (progress == null) continue;
                if (progress.completions > 0)
                    completedLevels++;
                earnedStars += Mathf.Clamp(progress.bestStars, 0, 3);
            }

            return new ChapterCompletionSummary(
                chapterId,
                completedLevels,
                totalLevels,
                earnedStars,
                totalLevels * 3,
                GetChapterCompletionPhase(chapterId)
            );
        }

        bool ChapterCompletionPrerequisitesMet(ChapterStoryCatalogData catalog)
        {
            if (catalog?.completion == null) return false;

            LevelProgressData finalProgress = GetLevelProgress(
                catalog.chapterId,
                catalog.completion.finalLevelId
            );
            return
                finalProgress != null &&
                finalProgress.completions > 0 &&
                HasCompletedStoryScene(catalog.completion.finaleSceneId);
        }

        public void ToggleMusic() { Save.music = !Save.music; Persist(); }
        public void ToggleSfx() { Save.sfx = !Save.sfx; Persist(); }
        public void ToggleVibration() { Save.vibration = !Save.vibration; Persist(); }
        public void ToggleNotifications() { Save.notifications = !Save.notifications; Persist(); }

        public void ResetProgress()
        {
            var reset = new PlayerSaveData
            {
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks
            };
            PlayerSaveWriteResult result;
            bool writeCompleted = SaveService.TryReplaceHistory(reset, out result);
            LastWriteResult = result;
            if (!writeCompleted && !result.PrimaryDurablyCommitted)
            {
                canPersist = false;
                Debug.LogError("Player save reset blocked: " + result);
                return;
            }

            StoryService.ResetRuntime();
            LastCompletionResult = null;
            LastFailureResult = null;
            unsavedRetainedRound = null;
            Save = reset;
            canPersist = writeCompleted;
            if (!writeCompleted)
            {
                Debug.LogWarning(
                    "Player save reset primary committed; journal cleanup incomplete: " +
                    result
                );
            }
            Changed?.Invoke();
        }

        public void Persist()
        {
            PlayerSaveWriteResult result;
            if (!SaveService.TrySave(Save, out result))
            {
                LastWriteResult = result;
                canPersist = false;
                Debug.LogError("Player save write blocked: " + result);
                return;
            }

            LastWriteResult = result;
            canPersist = true;
            Changed?.Invoke();
        }
    }
}
