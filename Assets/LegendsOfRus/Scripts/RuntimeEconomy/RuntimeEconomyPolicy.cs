using System;
using System.Collections.Generic;
using System.Globalization;

namespace LegendsOfRus
{
    /// <summary>
    /// Immutable, catalog-bound first-clear reward. The runtime coordinator
    /// never reads reward amounts from mutable LevelData.
    /// </summary>
    public sealed class RuntimeEconomyFirstClearReward
    {
        public int ChapterId { get; }
        public int LevelNumber { get; }
        public string LevelId { get; }
        public long Gold { get; }
        public long Gems { get; }
        public long BogatyrMace { get; }
        public long Firebird { get; }
        public long LivingLight { get; }
        public long SeaPearl { get; }

        internal RuntimeEconomyFirstClearReward(
            int chapterId,
            int levelNumber,
            string levelId,
            long gold,
            long gems,
            long bogatyrMace,
            long firebird,
            long livingLight,
            long seaPearl
        )
        {
            ChapterId = chapterId;
            LevelNumber = levelNumber;
            LevelId = levelId ?? string.Empty;
            Gold = gold;
            Gems = gems;
            BogatyrMace = bogatyrMace;
            Firebird = firebird;
            LivingLight = livingLight;
            SeaPearl = seaPearl;
        }
    }

    /// <summary>
    /// Immutable subset of economy configuration that Step 2 may consume at
    /// runtime. Starting balances are intentionally absent: loading or changing
    /// a policy can never rebase an existing player save.
    /// </summary>
    public sealed class RuntimeEconomyPolicy
    {
        public const string LegacyPolicyId = "legacy-runtime-v6";

        static readonly RuntimeEconomyPolicy legacyFallback =
            new RuntimeEconomyPolicy(
                LegacyPolicyId,
                string.Empty,
                false,
                5L,
                1L,
                0L,
                30L,
                true,
                1,
                500L,
                20L,
                1L,
                true,
                "gold",
                2,
                1,
                true,
                new RuntimeEconomyFirstClearReward[0]
            );

        readonly Dictionary<string, RuntimeEconomyFirstClearReward>
            firstClearRewards;

        public static RuntimeEconomyPolicy LegacyFallback
        {
            get { return legacyFallback; }
        }

        public string PolicyId { get; }
        public string CatalogSourceSha256 { get; }
        public bool IsCatalogConfigured { get; }

        public long EnergyCapacity { get; }
        public long LevelEntryCost { get; }
        public long RetryCost { get; }
        public long RefillIntervalMinutes { get; }

        public bool DailyRewardEnabled { get; }
        public int DailyCadenceUtcDays { get; }
        public long DailyGold { get; }
        public long DailyGems { get; }
        public long DailyEnergy { get; }

        public bool RewardDoublingEnabled { get; }
        public string RewardDoublingCurrencyId { get; }
        public int RewardDoublingMultiplier { get; }
        public int RewardDoublingMaxClaims { get; }
        public bool RewardDoublingOptional { get; }
        public int FirstClearRewardCount { get; }

        /// <summary>
        /// Runtime policy activation is never permission to replace balances
        /// already present in a version-6 save.
        /// </summary>
        public bool CanRebaseStartingBalances
        {
            get { return false; }
        }

        internal RuntimeEconomyPolicy(
            string policyId,
            string catalogSourceSha256,
            bool isCatalogConfigured,
            long energyCapacity,
            long levelEntryCost,
            long retryCost,
            long refillIntervalMinutes,
            bool dailyRewardEnabled,
            int dailyCadenceUtcDays,
            long dailyGold,
            long dailyGems,
            long dailyEnergy,
            bool rewardDoublingEnabled,
            string rewardDoublingCurrencyId,
            int rewardDoublingMultiplier,
            int rewardDoublingMaxClaims,
            bool rewardDoublingOptional,
            RuntimeEconomyFirstClearReward[] firstClearRewards
        )
        {
            PolicyId = policyId ?? string.Empty;
            CatalogSourceSha256 = catalogSourceSha256 ?? string.Empty;
            IsCatalogConfigured = isCatalogConfigured;
            EnergyCapacity = energyCapacity;
            LevelEntryCost = levelEntryCost;
            RetryCost = retryCost;
            RefillIntervalMinutes = refillIntervalMinutes;
            DailyRewardEnabled = dailyRewardEnabled;
            DailyCadenceUtcDays = dailyCadenceUtcDays;
            DailyGold = dailyGold;
            DailyGems = dailyGems;
            DailyEnergy = dailyEnergy;
            RewardDoublingEnabled = rewardDoublingEnabled;
            RewardDoublingCurrencyId = rewardDoublingCurrencyId ?? string.Empty;
            RewardDoublingMultiplier = rewardDoublingMultiplier;
            RewardDoublingMaxClaims = rewardDoublingMaxClaims;
            RewardDoublingOptional = rewardDoublingOptional;

            this.firstClearRewards =
                new Dictionary<string, RuntimeEconomyFirstClearReward>(
                    StringComparer.Ordinal
                );
            RuntimeEconomyFirstClearReward[] rewards = firstClearRewards ??
                new RuntimeEconomyFirstClearReward[0];
            for (int i = 0; i < rewards.Length; i++)
            {
                RuntimeEconomyFirstClearReward reward = rewards[i];
                if (reward == null) continue;
                this.firstClearRewards.Add(
                    RewardKey(reward.ChapterId, reward.LevelNumber, reward.LevelId),
                    reward
                );
            }
            FirstClearRewardCount = this.firstClearRewards.Count;
        }

        public bool TryGetFirstClearReward(
            int chapterId,
            int levelNumber,
            string levelId,
            out RuntimeEconomyFirstClearReward reward
        )
        {
            return firstClearRewards.TryGetValue(
                RewardKey(chapterId, levelNumber, levelId),
                out reward
            );
        }

        static string RewardKey(int chapterId, int levelNumber, string levelId)
        {
            return
                chapterId.ToString(CultureInfo.InvariantCulture) + ":" +
                levelNumber.ToString(CultureInfo.InvariantCulture) + ":" +
                (levelId ?? string.Empty);
        }
    }
}
