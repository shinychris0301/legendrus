using System;

namespace LegendsOfRus.EconomyCore
{
    [Serializable]
    public sealed class EconomyCatalogData
    {
        // Required authoring fields deliberately default to invalid sentinels.
        // JsonUtility preserves field initializers for omitted JSON properties;
        // fail-closed defaults prevent a truncated proposal from validating.
        public int schemaVersion;
        public string catalogId = string.Empty;
        public bool proposalOnly;
        public bool authoritative = true;
        public EconomyCurrencyData[] currencies;
        public EconomyAmountData[] startingBalances;
        public EconomyEnergyData energy;
        public EconomyBoosterData[] boosters;
        public EconomyContinueData continueRule;
        public EconomyDailyRewardData dailyReward;
        public EconomyRewardDoublingData rewardDoubling;
        public EconomyProgressionData progression;
    }

    [Serializable]
    public sealed class EconomyCurrencyData
    {
        public string id = string.Empty;
    }

    [Serializable]
    public sealed class EconomyAmountData
    {
        public string id = string.Empty;
        public long amount;

        public EconomyAmountData() { }

        public EconomyAmountData(string id, long amount)
        {
            this.id = id ?? string.Empty;
            this.amount = amount;
        }
    }

    [Serializable]
    public sealed class EconomyEnergyData
    {
        public long capacity;
        public long startingAmount = -1;
        public long levelEntryCost = -1;
        public long retryCost = -1;
        public long refillIntervalMinutes;
    }

    [Serializable]
    public sealed class EconomyBoosterData
    {
        public string id = string.Empty;
        public long startingQuantity = -1;
        public bool purchaseEnabled;
        public bool priceDefined;
        public EconomyAmountData price;
    }

    [Serializable]
    public sealed class EconomyContinueData
    {
        public bool enabled;
        public bool priceDefined;
        public EconomyAmountData price;
        public int grantedMoves = -1;
        public int maxPerAttempt = -1;
        public bool freeExitAndRetry;
    }

    [Serializable]
    public sealed class EconomyDailyRewardData
    {
        public bool enabled;
        public int cadenceUtcDays;
        public EconomyAmountData[] grants;
        public long energyGrant = -1;
    }

    [Serializable]
    public sealed class EconomyRewardDoublingData
    {
        public bool enabled;
        public string eligibleCurrencyId = string.Empty;
        public int multiplier;
        public int maxClaimsPerReward = -1;
        public bool optional;
    }

    [Serializable]
    public sealed class EconomyBoosterAmountData
    {
        public string id = string.Empty;
        public long amount;

        public EconomyBoosterAmountData() { }

        public EconomyBoosterAmountData(string id, long amount)
        {
            this.id = id ?? string.Empty;
            this.amount = amount;
        }
    }

    [Serializable]
    public sealed class EconomyProgressionData
    {
        public int chapterId;
        public bool normalProgressRequiresPurchase;
        public EconomyProgressionLevelData[] levels;
    }

    [Serializable]
    public sealed class EconomyProgressionLevelData
    {
        public string id = string.Empty;
        public int levelNumber;
        public bool requiresStore;
        public bool requiresAd;
        public bool requiresPurchase;
        public string requiredBoosterId = string.Empty;
        public long requiredBoosterQuantity = -1;
        public EconomyAmountData[] firstClearGrants;
        public EconomyBoosterAmountData[] firstClearBoosterGrants;
    }
}
