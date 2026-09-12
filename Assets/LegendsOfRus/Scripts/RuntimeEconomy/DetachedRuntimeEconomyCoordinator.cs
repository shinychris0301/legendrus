using System;
using System.Collections.Generic;
using System.Globalization;
using LegendsOfRus.EconomyCore;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Result of a detached Step 2 operation. Candidate is a deep copy and is
    /// never assigned to GameState or written by this assembly.
    /// </summary>
    public sealed class DetachedRuntimeEconomyResult
    {
        public bool Applied { get; }
        public bool Changed { get; }
        public string Code { get; }
        public string TransactionKey { get; }
        public PlayerSaveData Candidate { get; }
        public RuntimeEconomySnapshot Snapshot { get; }
        public int AttemptNumber { get; }
        public long MinutesUntilNextEnergy { get; }

        internal DetachedRuntimeEconomyResult(
            bool applied,
            bool changed,
            string code,
            string transactionKey,
            PlayerSaveData candidate,
            RuntimeEconomySnapshot snapshot,
            int attemptNumber,
            long minutesUntilNextEnergy
        )
        {
            Applied = applied;
            Changed = changed;
            Code = code ?? string.Empty;
            TransactionKey = transactionKey ?? string.Empty;
            Candidate = candidate;
            Snapshot = snapshot != null
                ? snapshot.DetachedCopy()
                : new RuntimeEconomySnapshot();
            AttemptNumber = attemptNumber;
            MinutesUntilNextEnergy = minutesUntilNextEnergy;
        }
    }

    /// <summary>
    /// Copy-on-write candidate for the approved runtime integration. It has no
    /// persistence, singleton, bootstrap, PlayerPrefs, analytics, store, ad, or
    /// GameState dependency. The shipped inactive gate causes every mutation
    /// method to fail closed. Tests may supply a detached all-approved result.
    /// </summary>
    public sealed class DetachedRuntimeEconomyCoordinator
    {
        const string ApprovalRequiredCode = "RUNTIME_ECON_APPROVAL_REQUIRED";
        const string SaveVersionCode = "RUNTIME_ECON_SAVE_VERSION_UNSUPPORTED";
        const string SaveInvalidCode = "RUNTIME_ECON_SAVE_INVALID";
        const string SaveRangeCode = "RUNTIME_ECON_SAVE_RANGE_UNSUPPORTED";
        const string LevelInvalidCode = "RUNTIME_ECON_LEVEL_INVALID";
        const string LevelLockedCode = "RUNTIME_ECON_LEVEL_LOCKED";
        const string RetryNotEligibleCode = "RUNTIME_ECON_RETRY_NOT_ELIGIBLE";
        const string LevelNotCompletedCode = "RUNTIME_ECON_LEVEL_NOT_COMPLETED";
        const string RewardPolicyMissingCode = "RUNTIME_ECON_REWARD_POLICY_MISSING";

        readonly RuntimeEconomyActivationResult activation;
        readonly RuntimeEconomyPolicy policy;
        readonly Func<long> utcNowTicks;
        readonly HashSet<string> stagedKeys =
            new HashSet<string>(StringComparer.Ordinal);
        bool hasActiveAttempt;
        int activeAttemptChapterId;
        int activeAttemptLevelId;
        int activeAttemptNumber;
        int activeAttemptCompletions;

        public bool IsReady
        {
            get
            {
                return
                    activation != null &&
                    activation.IsRuntimeActivated &&
                    policy != null &&
                    policy.IsCatalogConfigured;
            }
        }

        public RuntimeEconomyPolicy Policy
        {
            get { return policy; }
        }

        public int SuccessfulStageCount
        {
            get { return stagedKeys.Count; }
        }

        public DetachedRuntimeEconomyCoordinator(
            RuntimeEconomyActivationResult activation,
            Func<long> utcNowTicks = null
        )
        {
            this.activation = activation;
            policy = activation != null
                ? activation.Policy
                : RuntimeEconomyPolicy.LegacyFallback;
            this.utcNowTicks = utcNowTicks ?? (() => DateTime.UtcNow.Ticks);
        }

        public bool HasStaged(string transactionKey)
        {
            return
                !string.IsNullOrWhiteSpace(transactionKey) &&
                stagedKeys.Contains(transactionKey);
        }

        public DetachedRuntimeEconomyResult TryStage(
            PlayerSaveData source,
            RuntimeEconomyTransactionRequest request
        )
        {
            string key = request != null
                ? request.transactionKey ?? string.Empty
                : string.Empty;
            DetachedRuntimeEconomyResult rejected;
            PlayerSaveData candidate;
            if (!TryPrepare(source, key, out candidate, out rejected))
                return rejected;

            var engine = new RuntimeEconomyTransactionEngine(ToSnapshot(source));
            RuntimeEconomyTransactionResult transaction = engine.TryApply(request);
            if (!transaction.applied)
            {
                return Failure(
                    source,
                    transaction.code,
                    key,
                    transaction.snapshot
                );
            }

            if (!CanWriteToSave(transaction.snapshot))
                return Failure(source, SaveRangeCode, key, ToSnapshot(source));

            ApplySnapshot(candidate, transaction.snapshot);
            stagedKeys.Add(key);
            return Success(candidate, key, changed: !EconomyEquals(source, candidate));
        }

        public DetachedRuntimeEconomyResult TryAdvanceEnergy(
            PlayerSaveData source,
            long? nowTicks = null
        )
        {
            DetachedRuntimeEconomyResult rejected;
            PlayerSaveData candidate;
            if (!TryPrepare(source, string.Empty, out candidate, out rejected, false))
                return rejected;

            long now = nowTicks ?? utcNowTicks();
            EconomyEnergyResult energy = Advance(source, now);
            if (!energy.valid || energy.clockRollback)
            {
                return Failure(
                    source,
                    string.IsNullOrEmpty(energy.code)
                        ? EconomyViolationCodes.EnergyInvalid
                        : energy.code,
                    string.Empty,
                    ToSnapshot(source),
                    energy.minutesUntilNext
                );
            }

            if (energy.gained <= 0)
            {
                return new DetachedRuntimeEconomyResult(
                    true,
                    false,
                    string.Empty,
                    string.Empty,
                    candidate,
                    ToSnapshot(candidate),
                    0,
                    energy.minutesUntilNext
                );
            }

            if (energy.energy > int.MaxValue)
                return Failure(source, SaveRangeCode, string.Empty, ToSnapshot(source));

            candidate.energy = (int)energy.energy;
            candidate.lastEnergyUtcTicks = energy.lastRefillTick;
            return new DetachedRuntimeEconomyResult(
                true,
                true,
                string.Empty,
                string.Empty,
                candidate,
                ToSnapshot(candidate),
                0,
                energy.minutesUntilNext
            );
        }

        public DetachedRuntimeEconomyResult TryBeginLevelAttempt(
            PlayerSaveData source,
            LevelData requestedLevel,
            bool retry,
            string transactionKey,
            long? nowTicks = null
        )
        {
            DetachedRuntimeEconomyResult rejected;
            PlayerSaveData candidate;
            if (!TryPrepare(source, transactionKey, out candidate, out rejected))
                return rejected;

            LevelData level = ResolveCanonicalLevel(requestedLevel);
            if (level == null)
                return Failure(source, LevelInvalidCode, transactionKey, ToSnapshot(source));
            if (!IsLevelUnlocked(source, level))
                return Failure(source, LevelLockedCode, transactionKey, ToSnapshot(source));

            LevelProgressData existingProgress = GetLevelProgress(
                source,
                level.chapterId,
                level.levelId,
                false
            );
            if (
                retry &&
                (
                    !hasActiveAttempt ||
                    activeAttemptChapterId != level.chapterId ||
                    activeAttemptLevelId != level.levelId ||
                    existingProgress == null ||
                    existingProgress.attempts <= 0 ||
                    existingProgress.attempts != activeAttemptNumber ||
                    existingProgress.completions != activeAttemptCompletions
                )
            )
            {
                return Failure(
                    source,
                    RetryNotEligibleCode,
                    transactionKey,
                    ToSnapshot(source)
                );
            }

            long cost = retry ? policy.RetryCost : policy.LevelEntryCost;
            long now = nowTicks ?? utcNowTicks();
            if (retry && source.lastEnergyUtcTicks <= 0)
            {
                return Failure(
                    source,
                    EconomyViolationCodes.EnergyInvalid,
                    transactionKey,
                    ToSnapshot(source)
                );
            }
            if (retry && now < source.lastEnergyUtcTicks)
            {
                return Failure(
                    source,
                    EconomyViolationCodes.ClockRollback,
                    transactionKey,
                    ToSnapshot(source),
                    policy.RefillIntervalMinutes
                );
            }
            EconomyEnergyResult energy = retry
                ? new EconomyEnergyResult
                {
                    valid = true,
                    code = string.Empty,
                    energy = source.energy,
                    lastRefillTick = source.lastEnergyUtcTicks,
                    minutesUntilNext = 0
                }
                : Advance(source, now);
            if (!energy.valid || energy.clockRollback)
            {
                return Failure(
                    source,
                    string.IsNullOrEmpty(energy.code)
                        ? EconomyViolationCodes.EnergyInvalid
                        : energy.code,
                    transactionKey,
                    ToSnapshot(source),
                    energy.minutesUntilNext
                );
            }

            RuntimeEconomySnapshot advanced = ToSnapshot(source);
            advanced.energy = energy.energy;
            var engine = new RuntimeEconomyTransactionEngine(advanced);
            EconomyAmountData[] costs = cost == 0
                ? new EconomyAmountData[0]
                : new[]
                {
                    new EconomyAmountData(RuntimeEconomyResourceIds.Energy, cost)
                };
            RuntimeEconomyTransactionResult transaction = engine.TryApply(
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    costs,
                    new EconomyAmountData[0]
                )
            );
            if (!transaction.applied)
            {
                // Refill and spend are one logical candidate. If the spend is
                // denied, even a due refill remains uncommitted.
                return Failure(
                    source,
                    transaction.code,
                    transactionKey,
                    ToSnapshot(source),
                    energy.minutesUntilNext
                );
            }
            if (!CanWriteToSave(transaction.snapshot))
                return Failure(source, SaveRangeCode, transactionKey, ToSnapshot(source));

            LevelProgressData progress = GetLevelProgress(
                candidate,
                level.chapterId,
                level.levelId,
                true
            );
            if (progress.attempts == int.MaxValue)
                return Failure(source, EconomyViolationCodes.Overflow, transactionKey, ToSnapshot(source));

            ApplySnapshot(candidate, transaction.snapshot);
            candidate.lastEnergyUtcTicks = energy.lastRefillTick;
            candidate.selectedChapter = level.chapterId;
            candidate.selectedLevel = level.levelId;
            progress.attempts++;
            if (progress.attempts > 1)
            {
                if (progress.retries == int.MaxValue)
                    return Failure(source, EconomyViolationCodes.Overflow, transactionKey, ToSnapshot(source));
                progress.retries++;
            }

            stagedKeys.Add(transactionKey);
            hasActiveAttempt = true;
            activeAttemptChapterId = level.chapterId;
            activeAttemptLevelId = level.levelId;
            activeAttemptNumber = progress.attempts;
            activeAttemptCompletions = progress.completions;
            return new DetachedRuntimeEconomyResult(
                true,
                true,
                string.Empty,
                transactionKey,
                candidate,
                ToSnapshot(candidate),
                progress.attempts,
                energy.minutesUntilNext
            );
        }

        public DetachedRuntimeEconomyResult TryGrantFirstClear(
            PlayerSaveData source,
            LevelData requestedLevel,
            string transactionKey
        )
        {
            DetachedRuntimeEconomyResult rejected;
            PlayerSaveData ignoredCandidate;
            if (!TryPrepare(
                source,
                transactionKey,
                out ignoredCandidate,
                out rejected
            ))
            {
                return rejected;
            }

            LevelData level = ResolveCanonicalLevel(requestedLevel);
            if (level == null)
                return Failure(source, LevelInvalidCode, transactionKey, ToSnapshot(source));
            if (!IsLevelUnlocked(source, level))
                return Failure(source, LevelLockedCode, transactionKey, ToSnapshot(source));

            LevelProgressData existing = GetLevelProgress(
                source,
                level.chapterId,
                level.levelId,
                false
            );
            if (
                existing == null ||
                existing.attempts <= 0 ||
                existing.completions <= 0
            )
            {
                return Failure(
                    source,
                    LevelNotCompletedCode,
                    transactionKey,
                    ToSnapshot(source)
                );
            }
            if (existing != null && existing.rewardClaimed)
            {
                ClearActiveAttempt(level);
                return Failure(
                    source,
                    EconomyViolationCodes.DuplicateGrant,
                    transactionKey,
                    ToSnapshot(source)
                );
            }

            RuntimeEconomyFirstClearReward reward;
            if (!policy.TryGetFirstClearReward(
                level.chapterId,
                level.levelId,
                level.id,
                out reward
            ))
            {
                return Failure(
                    source,
                    RewardPolicyMissingCode,
                    transactionKey,
                    ToSnapshot(source)
                );
            }
            DetachedRuntimeEconomyResult staged = TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    new EconomyAmountData[0],
                    RewardGrants(reward)
                )
            );
            if (!staged.Applied)
                return staged;

            LevelProgressData progress = GetLevelProgress(
                staged.Candidate,
                level.chapterId,
                level.levelId,
                true
            );
            progress.rewardClaimed = true;
            ClearActiveAttempt(level);
            return Success(staged.Candidate, transactionKey, true);
        }

        public DetachedRuntimeEconomyResult TryClaimDaily(
            PlayerSaveData source,
            string transactionKey,
            long? nowTicks = null
        )
        {
            if (!IsReady)
                return Failure(source, ApprovalRequiredCode, transactionKey, ToSnapshot(source));
            if (!policy.DailyRewardEnabled)
                return Failure(source, EconomyViolationCodes.InvalidTransaction, transactionKey, ToSnapshot(source));

            DateTime today;
            try
            {
                today = new DateTime(nowTicks ?? utcNowTicks(), DateTimeKind.Utc).Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                return Failure(source, EconomyViolationCodes.InvalidTransaction, transactionKey, ToSnapshot(source));
            }

            if (!string.IsNullOrEmpty(source != null ? source.lastDailyRewardUtc : string.Empty))
            {
                DateTime last;
                if (!DateTime.TryParseExact(
                    source.lastDailyRewardUtc,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out last
                ))
                {
                    return Failure(source, EconomyViolationCodes.InvalidTransaction, transactionKey, ToSnapshot(source));
                }

                last = last.Date;
                if (today < last)
                    return Failure(source, EconomyViolationCodes.ClockRollback, transactionKey, ToSnapshot(source));
                if ((today - last).Days < policy.DailyCadenceUtcDays)
                {
                    return Failure(
                        source,
                        EconomyViolationCodes.DailyAlreadyClaimed,
                        transactionKey,
                        ToSnapshot(source)
                    );
                }
            }

            DetachedRuntimeEconomyResult staged = TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    new EconomyAmountData[0],
                    new[]
                    {
                        new EconomyAmountData(RuntimeEconomyResourceIds.Gold, policy.DailyGold),
                        new EconomyAmountData(RuntimeEconomyResourceIds.Gems, policy.DailyGems),
                        new EconomyAmountData(RuntimeEconomyResourceIds.Energy, policy.DailyEnergy)
                    }
                )
            );
            if (!staged.Applied)
                return staged;

            staged.Candidate.lastDailyRewardUtc = today.ToString(
                "yyyyMMdd",
                CultureInfo.InvariantCulture
            );
            return Success(staged.Candidate, transactionKey, true);
        }

        public DetachedRuntimeEconomyResult TryDoubleLastReward(
            PlayerSaveData source,
            string transactionKey
        )
        {
            if (!IsReady)
                return Failure(source, ApprovalRequiredCode, transactionKey, ToSnapshot(source));

            if (
                source == null ||
                !policy.RewardDoublingEnabled ||
                !source.lastRewardEligibleForDouble ||
                source.lastRewardDoubled ||
                source.lastRewardGold <= 0 ||
                !string.Equals(
                    policy.RewardDoublingCurrencyId,
                    RuntimeEconomyResourceIds.Gold,
                    StringComparison.Ordinal
                )
            )
            {
                return Failure(source, EconomyViolationCodes.InvalidTransaction, transactionKey, ToSnapshot(source));
            }

            long bonus;
            try
            {
                bonus = checked(
                    (long)source.lastRewardGold *
                    (policy.RewardDoublingMultiplier - 1L)
                );
            }
            catch (OverflowException)
            {
                return Failure(source, EconomyViolationCodes.Overflow, transactionKey, ToSnapshot(source));
            }

            DetachedRuntimeEconomyResult staged = TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    new EconomyAmountData[0],
                    new[]
                    {
                        new EconomyAmountData(RuntimeEconomyResourceIds.Gold, bonus)
                    }
                )
            );
            if (!staged.Applied)
                return staged;

            staged.Candidate.lastRewardDoubled = true;
            return Success(staged.Candidate, transactionKey, true);
        }

        public DetachedRuntimeEconomyResult TrySpendBooster(
            PlayerSaveData source,
            BoosterType booster,
            string transactionKey
        )
        {
            string resourceId;
            if (!TryGetBoosterResourceId(booster, out resourceId))
                return Failure(source, EconomyViolationCodes.UnknownBalance, transactionKey, ToSnapshot(source));

            return TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    new[] { new EconomyAmountData(resourceId, 1) },
                    new EconomyAmountData[0]
                )
            );
        }

        public DetachedRuntimeEconomyResult TryRefundBooster(
            PlayerSaveData source,
            BoosterType booster,
            string transactionKey
        )
        {
            string resourceId;
            if (!TryGetBoosterResourceId(booster, out resourceId))
                return Failure(source, EconomyViolationCodes.UnknownBalance, transactionKey, ToSnapshot(source));

            return TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    transactionKey,
                    new EconomyAmountData[0],
                    new[] { new EconomyAmountData(resourceId, 1) }
                )
            );
        }

        bool TryPrepare(
            PlayerSaveData source,
            string transactionKey,
            out PlayerSaveData candidate,
            out DetachedRuntimeEconomyResult rejected,
            bool requireTransactionKey = true
        )
        {
            candidate = Clone(source);
            rejected = null;
            if (!IsReady)
            {
                rejected = Failure(source, ApprovalRequiredCode, transactionKey, ToSnapshot(source));
                return false;
            }
            if (source == null || candidate == null)
            {
                rejected = Failure(source, SaveInvalidCode, transactionKey, ToSnapshot(source));
                return false;
            }
            if (source.saveVersion != 6)
            {
                rejected = Failure(source, SaveVersionCode, transactionKey, ToSnapshot(source));
                return false;
            }
            if (!IsValidSaveEconomy(source))
            {
                rejected = Failure(source, SaveInvalidCode, transactionKey, ToSnapshot(source));
                return false;
            }
            if (source.energyMax != policy.EnergyCapacity)
            {
                rejected = Failure(source, SaveRangeCode, transactionKey, ToSnapshot(source));
                return false;
            }
            if (requireTransactionKey)
            {
                if (
                    string.IsNullOrWhiteSpace(transactionKey) ||
                    !string.Equals(transactionKey, transactionKey.Trim(), StringComparison.Ordinal)
                )
                {
                    rejected = Failure(
                        source,
                        EconomyViolationCodes.InvalidTransaction,
                        transactionKey,
                        ToSnapshot(source)
                    );
                    return false;
                }
                if (stagedKeys.Contains(transactionKey))
                {
                    rejected = Failure(
                        source,
                        EconomyViolationCodes.DuplicateTransaction,
                        transactionKey,
                        ToSnapshot(source)
                    );
                    return false;
                }
            }
            return true;
        }

        EconomyEnergyResult Advance(PlayerSaveData source, long nowTicks)
        {
            if (source == null || source.lastEnergyUtcTicks <= 0)
            {
                return new EconomyEnergyResult
                {
                    valid = false,
                    code = EconomyViolationCodes.EnergyInvalid,
                    energy = source != null ? source.energy : 0,
                    lastRefillTick = source != null ? source.lastEnergyUtcTicks : 0
                };
            }

            return EconomyEnergyMath.Advance(
                new EconomyEnergyData
                {
                    capacity = source.energyMax,
                    startingAmount = source.energy,
                    levelEntryCost = policy.LevelEntryCost,
                    retryCost = policy.RetryCost,
                    refillIntervalMinutes = policy.RefillIntervalMinutes
                },
                source.energy,
                source.lastEnergyUtcTicks,
                nowTicks,
                TimeSpan.TicksPerMinute
            );
        }

        static PlayerSaveData Clone(PlayerSaveData source)
        {
            if (source == null)
                return null;
            try
            {
                return JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(source));
            }
            catch
            {
                return null;
            }
        }

        static RuntimeEconomySnapshot ToSnapshot(PlayerSaveData save)
        {
            return save == null
                ? new RuntimeEconomySnapshot()
                : new RuntimeEconomySnapshot(
                    save.gold,
                    save.gems,
                    save.energy,
                    save.energyMax,
                    save.boosterMace,
                    save.boosterFirebird,
                    save.boosterLivingLight,
                    save.boosterSeaPearl
                );
        }

        static bool IsValidSaveEconomy(PlayerSaveData save)
        {
            return
                save != null &&
                save.gold >= 0 &&
                save.gems >= 0 &&
                save.energyMax > 0 &&
                save.energy >= 0 &&
                save.energy <= save.energyMax &&
                save.boosterMace >= 0 &&
                save.boosterFirebird >= 0 &&
                save.boosterLivingLight >= 0 &&
                save.boosterSeaPearl >= 0 &&
                HasValidLevelProgressShape(save.levelProgress);
        }

        static bool HasValidLevelProgressShape(List<LevelProgressData> progressList)
        {
            if (progressList == null)
                return true;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < progressList.Count; i++)
            {
                LevelProgressData progress = progressList[i];
                if (
                    progress == null ||
                    progress.chapterId <= 0 ||
                    progress.levelId <= 0 ||
                    progress.bestStars < 0 || progress.bestStars > 3 ||
                    progress.bestScore < 0 ||
                    progress.attempts < 0 ||
                    progress.retries < 0 || progress.retries > progress.attempts ||
                    progress.failures < 0 || progress.failures > progress.attempts ||
                    progress.completions < 0 || progress.completions > progress.attempts ||
                    (long)progress.failures + progress.completions > progress.attempts ||
                    !seen.Add(progress.chapterId + ":" + progress.levelId)
                )
                {
                    return false;
                }
            }
            return true;
        }

        static bool CanWriteToSave(RuntimeEconomySnapshot snapshot)
        {
            return
                snapshot != null &&
                snapshot.gold >= 0 && snapshot.gold <= int.MaxValue &&
                snapshot.gems >= 0 && snapshot.gems <= int.MaxValue &&
                snapshot.energyCapacity > 0 && snapshot.energyCapacity <= int.MaxValue &&
                snapshot.energy >= 0 && snapshot.energy <= snapshot.energyCapacity &&
                snapshot.bogatyrMace >= 0 && snapshot.bogatyrMace <= int.MaxValue &&
                snapshot.firebird >= 0 && snapshot.firebird <= int.MaxValue &&
                snapshot.livingLight >= 0 && snapshot.livingLight <= int.MaxValue &&
                snapshot.seaPearl >= 0 && snapshot.seaPearl <= int.MaxValue;
        }

        static void ApplySnapshot(PlayerSaveData save, RuntimeEconomySnapshot snapshot)
        {
            save.gold = (int)snapshot.gold;
            save.gems = (int)snapshot.gems;
            save.energy = (int)snapshot.energy;
            save.energyMax = (int)snapshot.energyCapacity;
            save.boosterMace = (int)snapshot.bogatyrMace;
            save.boosterFirebird = (int)snapshot.firebird;
            save.boosterLivingLight = (int)snapshot.livingLight;
            save.boosterSeaPearl = (int)snapshot.seaPearl;
        }

        static bool EconomyEquals(PlayerSaveData left, PlayerSaveData right)
        {
            return
                left != null && right != null &&
                left.gold == right.gold &&
                left.gems == right.gems &&
                left.energy == right.energy &&
                left.energyMax == right.energyMax &&
                left.boosterMace == right.boosterMace &&
                left.boosterFirebird == right.boosterFirebird &&
                left.boosterLivingLight == right.boosterLivingLight &&
                left.boosterSeaPearl == right.boosterSeaPearl;
        }

        static LevelData ResolveCanonicalLevel(LevelData requested)
        {
            if (requested == null)
                return null;
            LevelData canonical = ChapterLevelCatalog.Find(
                requested.chapterId,
                requested.levelId
            );
            return canonical != null &&
                string.Equals(canonical.id, requested.id, StringComparison.Ordinal)
                ? canonical
                : null;
        }

        static bool IsLevelUnlocked(PlayerSaveData save, LevelData level)
        {
            return
                save != null &&
                level != null &&
                level.chapterId == save.selectedChapter &&
                level.levelId >= 1 &&
                level.levelId <= save.highestUnlockedLevel;
        }

        void ClearActiveAttempt(LevelData level)
        {
            if (
                level != null &&
                hasActiveAttempt &&
                activeAttemptChapterId == level.chapterId &&
                activeAttemptLevelId == level.levelId
            )
            {
                hasActiveAttempt = false;
                activeAttemptNumber = 0;
                activeAttemptCompletions = 0;
            }
        }

        static LevelProgressData GetLevelProgress(
            PlayerSaveData save,
            int chapterId,
            int levelId,
            bool create
        )
        {
            if (save == null)
                return null;
            if (save.levelProgress == null)
            {
                if (!create)
                    return null;
                save.levelProgress = new List<LevelProgressData>();
            }

            for (int i = 0; i < save.levelProgress.Count; i++)
            {
                LevelProgressData progress = save.levelProgress[i];
                if (
                    progress != null &&
                    progress.chapterId == chapterId &&
                    progress.levelId == levelId
                )
                {
                    return progress;
                }
            }

            if (!create)
                return null;
            var created = new LevelProgressData
            {
                chapterId = chapterId,
                levelId = levelId
            };
            save.levelProgress.Add(created);
            return created;
        }

        static EconomyAmountData[] RewardGrants(
            RuntimeEconomyFirstClearReward reward
        )
        {
            return new[]
            {
                new EconomyAmountData(RuntimeEconomyResourceIds.Gold, reward.Gold),
                new EconomyAmountData(RuntimeEconomyResourceIds.Gems, reward.Gems),
                new EconomyAmountData(RuntimeEconomyResourceIds.BogatyrMace, reward.BogatyrMace),
                new EconomyAmountData(RuntimeEconomyResourceIds.Firebird, reward.Firebird),
                new EconomyAmountData(RuntimeEconomyResourceIds.LivingLight, reward.LivingLight),
                new EconomyAmountData(RuntimeEconomyResourceIds.SeaPearl, reward.SeaPearl)
            };
        }

        static bool TryGetBoosterResourceId(BoosterType booster, out string resourceId)
        {
            switch (booster)
            {
                case BoosterType.BogatyrMace:
                    resourceId = RuntimeEconomyResourceIds.BogatyrMace;
                    return true;
                case BoosterType.Firebird:
                    resourceId = RuntimeEconomyResourceIds.Firebird;
                    return true;
                case BoosterType.LivingLight:
                    resourceId = RuntimeEconomyResourceIds.LivingLight;
                    return true;
                case BoosterType.SeaPearl:
                    resourceId = RuntimeEconomyResourceIds.SeaPearl;
                    return true;
                default:
                    resourceId = string.Empty;
                    return false;
            }
        }

        static DetachedRuntimeEconomyResult Success(
            PlayerSaveData candidate,
            string key,
            bool changed
        )
        {
            return new DetachedRuntimeEconomyResult(
                true,
                changed,
                string.Empty,
                key,
                candidate,
                ToSnapshot(candidate),
                0,
                0
            );
        }

        static DetachedRuntimeEconomyResult Failure(
            PlayerSaveData source,
            string code,
            string key,
            RuntimeEconomySnapshot snapshot,
            long minutesUntilNext = 0
        )
        {
            return new DetachedRuntimeEconomyResult(
                false,
                false,
                code,
                key,
                Clone(source),
                snapshot,
                0,
                minutesUntilNext
            );
        }
    }
}
