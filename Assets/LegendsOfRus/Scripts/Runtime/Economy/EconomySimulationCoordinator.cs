using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LegendsOfRus.EconomyCore;

namespace LegendsOfRus
{
    /// <summary>
    /// Read-only v1.15 Step 1 report seam. It compares detached proposal data
    /// with explicitly recorded prototype values; it is not runtime authority.
    /// </summary>
    public static class EconomySimulationCoordinator
    {
        public const string ReportId = "v1.15-step01-economy-simulation";
        public const string Authority = "proposal-comparison-only";

        public static EconomySimulationCoordinatorResult LoadAndRunProposal()
        {
            EconomyCatalogLoadResult load = EconomyCatalogLoader.LoadProposal();
            return RunLoaded(load);
        }

        public static EconomySimulationCoordinatorResult RunProposal(
            EconomyCatalogData catalog
        )
        {
            EconomyValidationReport validation = EconomyCatalogValidator.Validate(catalog);
            var load = new EconomyCatalogLoadResult(
                catalog,
                validation,
                string.Empty,
                validation.verdict == EconomyVerdict.Pass
                    ? string.Empty
                    : EconomyViolationCodes.InvalidCatalog,
                validation.verdict == EconomyVerdict.Pass
                    ? string.Empty
                    : "The supplied proposal catalog failed validation."
            );
            return RunLoaded(load);
        }

        public static string ToDeterministicJson(
            EconomySimulationCoordinatorResult result
        )
        {
            var json = new StringBuilder(8192);
            json.Append('{');
            AppendLong(json, "schemaVersion", 1); json.Append(',');
            AppendString(json, "reportId", ReportId); json.Append(',');
            AppendBoolean(json, "nonAuthoritative", true); json.Append(',');
            AppendString(json, "authority", Authority); json.Append(',');
            json.Append("\"fixedInputs\":{");
            AppendLong(json, "startTick", 0); json.Append(',');
            AppendLong(json, "ticksPerMinute", 1); json.Append(',');
            AppendBoolean(json, "allowStore", false); json.Append(',');
            AppendBoolean(json, "allowAds", false); json.Append(',');
            AppendBoolean(json, "allowPurchases", false); json.Append(',');
            AppendLong(json, "optionalPaidContinuesPerLevel", 0);
            json.Append("},");
            json.Append("\"load\":{");
            AppendBoolean(json, "succeeded", result != null && result.LoadSucceeded); json.Append(',');
            AppendString(json, "errorCode", result == null ? "ECON_COORDINATOR_RESULT_NULL" : result.ErrorCode); json.Append(',');
            AppendString(json, "error", result == null ? "Coordinator result is null." : result.Error);
            json.Append("},");
            json.Append("\"simulation\":");
            json.Append(result == null || result.Simulation == null
                ? "null"
                : EconomyDeterministicJson.Serialize(result.Simulation));
            json.Append(',');
            json.Append("\"prototypeComparison\":");
            json.Append(result == null || result.PrototypeComparison == null
                ? "null"
                : EconomyDeterministicJson.Serialize(result.PrototypeComparison));
            json.Append('}');
            return json.ToString();
        }

        static EconomySimulationCoordinatorResult RunLoaded(
            EconomyCatalogLoadResult load
        )
        {
            EconomyCatalogData catalog = load == null ? null : load.Catalog;
            var request = new EconomySimulationRequest
            {
                scenarioId = "chapter01-purchase-free-baseline",
                startTick = 0,
                ticksPerMinute = 1,
                allowStore = false,
                allowAds = false,
                allowPurchases = false,
                consumeRequiredBoosters = true,
                optionalPaidContinuesPerLevel = 0
            };
            EconomySimulationReport simulation = EconomySimulator.Run(catalog, request);
            EconomyComparisonReport comparison = BuildPrototypeComparison(catalog);
            return new EconomySimulationCoordinatorResult(
                load != null && load.Succeeded,
                load == null ? "ECON_LOAD_RESULT_NULL" : load.ErrorCode,
                load == null ? "Proposal loader returned no result." : load.Error,
                simulation,
                comparison
            );
        }

        static EconomyComparisonReport BuildPrototypeComparison(
            EconomyCatalogData catalog
        )
        {
            var entries = new List<EconomyComparisonEntryData>();
            bool proposalAuthorityValid =
                catalog != null && catalog.proposalOnly && !catalog.authoritative;
            Add(entries, "catalog.authority",
                proposalAuthorityValid
                    ? EconomyComparisonStatus.Match
                    : EconomyComparisonStatus.Conflict,
                "prototype literals remain authoritative",
                proposalAuthorityValid
                    ? "proposal-only; non-authoritative"
                    : "invalid authority flags",
                "EconomyCatalogData authority flags");

            if (catalog == null)
            {
                Add(entries, "catalog", EconomyComparisonStatus.Missing,
                    "prototype values available", "proposal unavailable",
                    "EconomyCatalogLoader");
                return EconomyComparisonReportBuilder.Build(string.Empty, entries.ToArray());
            }

            AddLong(entries, "wallet.gold.starting", 12540,
                FindAmount(catalog.startingBalances, "gold"), "PlayerSaveData.gold");
            AddLong(entries, "wallet.gems.starting", 350,
                FindAmount(catalog.startingBalances, "gems"), "PlayerSaveData.gems");
            AddLong(entries, "energy.capacity", 5,
                catalog.energy == null ? long.MinValue : catalog.energy.capacity,
                "PlayerSaveData.energyMax");
            AddLong(entries, "energy.startingAmount", 5,
                catalog.energy == null ? long.MinValue : catalog.energy.startingAmount,
                "PlayerSaveData.energy");
            AddLong(entries, "energy.levelEntryCost", 1,
                catalog.energy == null ? long.MinValue : catalog.energy.levelEntryCost,
                "ScreenRouter.StartLevel/SpendEnergy");
            AddLong(entries, "energy.retryCost", 0,
                catalog.energy == null ? long.MinValue : catalog.energy.retryCost,
                "ScreenRouter.RestartCurrentLevel");
            AddLong(entries, "energy.refillIntervalMinutes", 30,
                catalog.energy == null ? long.MinValue : catalog.energy.refillIntervalMinutes,
                "GameState.EnergyRefillMinutes");

            AddLong(entries, "boosters.bogatyr_mace.startingQuantity", 7,
                FindBooster(catalog.boosters, "bogatyr_mace"), "PlayerSaveData.boosterMace");
            AddLong(entries, "boosters.firebird.startingQuantity", 5,
                FindBooster(catalog.boosters, "firebird"), "PlayerSaveData.boosterFirebird");
            AddLong(entries, "boosters.living_light.startingQuantity", 8,
                FindBooster(catalog.boosters, "living_light"), "PlayerSaveData.boosterLivingLight");
            AddLong(entries, "boosters.sea_pearl.startingQuantity", 10,
                FindBooster(catalog.boosters, "sea_pearl"), "PlayerSaveData.boosterSeaPearl");
            Add(entries, "boosters.purchasePrices", EconomyComparisonStatus.Missing,
                "no runtime booster prices", "disabled and undefined",
                "GameState.TryUseBooster");

            bool continueDisabled = catalog.continueRule != null && !catalog.continueRule.enabled;
            Add(entries, "continue.enabled",
                continueDisabled ? EconomyComparisonStatus.Match : EconomyComparisonStatus.Conflict,
                "false", continueDisabled ? "false" : "true",
                "ScreenRouter.Fail");
            Add(entries, "continue.price", EconomyComparisonStatus.Dormant,
                "not consumed by runtime", ContinuePrice(catalog.continueRule),
                "legacy economy value retained for comparison");
            Add(entries, "continue.grantedMoves", EconomyComparisonStatus.Dormant,
                "not granted by runtime",
                catalog.continueRule == null
                    ? "missing"
                    : catalog.continueRule.grantedMoves.ToString(CultureInfo.InvariantCulture),
                "MonetizationService.MockRewardedContinue");

            AddLong(entries, "dailyReward.gold", 500,
                FindAmount(catalog.dailyReward == null ? null : catalog.dailyReward.grants, "gold"),
                "GameState.ClaimDailyReward");
            AddLong(entries, "dailyReward.gems", 20,
                FindAmount(catalog.dailyReward == null ? null : catalog.dailyReward.grants, "gems"),
                "GameState.ClaimDailyReward");
            AddLong(entries, "dailyReward.energy", 1,
                catalog.dailyReward == null ? long.MinValue : catalog.dailyReward.energyGrant,
                "GameState.ClaimDailyReward");
            AddLong(entries, "rewardDoubling.multiplier", 2,
                catalog.rewardDoubling == null ? long.MinValue : catalog.rewardDoubling.multiplier,
                "GameState.TryDoubleLastLevelGoldReward");

            AddLong(entries, "progression.chapter01.firstClearGold", 1850,
                SumCurrency(catalog.progression, "gold"),
                "chapter01_levels.json");
            AddLong(entries, "progression.chapter01.firstClearGems", 40,
                SumCurrency(catalog.progression, "gems"),
                "chapter01_levels.json");
            AddLong(entries, "progression.chapter01.levelCount", 10,
                catalog.progression == null || catalog.progression.levels == null
                    ? long.MinValue
                    : catalog.progression.levels.Length,
                "chapter01_levels.json");
            Add(entries, "prototype.mockOffers", EconomyComparisonStatus.Missing,
                "development-only grants", "not modeled by Step 1 proposal",
                "MonetizationService.MockPurchase");
            Add(entries, "prototype.mailClaim", EconomyComparisonStatus.Dormant,
                "repeatable +250 gold prototype hotspot", "excluded from guaranteed income",
                "ScreenRouter.Mail");

            return EconomyComparisonReportBuilder.Build(catalog.catalogId, entries.ToArray());
        }

        static long FindAmount(EconomyAmountData[] values, string id)
        {
            if (values == null) return long.MinValue;
            for (int i = 0; i < values.Length; i++)
                if (values[i] != null && string.Equals(values[i].id, id, StringComparison.Ordinal))
                    return values[i].amount;
            return long.MinValue;
        }

        static long FindBooster(EconomyBoosterData[] values, string id)
        {
            if (values == null) return long.MinValue;
            for (int i = 0; i < values.Length; i++)
                if (values[i] != null && string.Equals(values[i].id, id, StringComparison.Ordinal))
                    return values[i].startingQuantity;
            return long.MinValue;
        }

        static long SumCurrency(EconomyProgressionData progression, string id)
        {
            if (progression == null || progression.levels == null) return long.MinValue;
            try
            {
                long total = 0;
                for (int i = 0; i < progression.levels.Length; i++)
                {
                    EconomyProgressionLevelData level = progression.levels[i];
                    if (level == null) return long.MinValue;
                    long value = FindAmount(level.firstClearGrants, id);
                    if (value != long.MinValue) total = checked(total + value);
                }
                return total;
            }
            catch (OverflowException)
            {
                return long.MinValue;
            }
        }

        static string ContinuePrice(EconomyContinueData rule)
        {
            if (rule == null || !rule.priceDefined || rule.price == null)
                return "undefined";
            return rule.price.amount.ToString(CultureInfo.InvariantCulture) + " " +
                (rule.price.id ?? string.Empty);
        }

        static void AddLong(
            List<EconomyComparisonEntryData> entries,
            string path,
            long runtime,
            long proposal,
            string source
        )
        {
            bool missing = proposal == long.MinValue;
            Add(
                entries,
                path,
                missing
                    ? EconomyComparisonStatus.Missing
                    : runtime == proposal
                        ? EconomyComparisonStatus.Match
                        : EconomyComparisonStatus.Conflict,
                runtime.ToString(CultureInfo.InvariantCulture),
                missing ? "missing" : proposal.ToString(CultureInfo.InvariantCulture),
                source
            );
        }

        static void Add(
            List<EconomyComparisonEntryData> entries,
            string path,
            EconomyComparisonStatus status,
            string runtime,
            string proposal,
            string source
        )
        {
            entries.Add(new EconomyComparisonEntryData
            {
                path = path,
                status = status,
                runtimeValue = runtime,
                proposalValue = proposal,
                source = source
            });
        }

        static void AppendString(StringBuilder json, string name, string value)
        {
            AppendEscaped(json, name); json.Append(':'); AppendEscaped(json, value ?? string.Empty);
        }

        static void AppendLong(StringBuilder json, string name, long value)
        {
            AppendEscaped(json, name); json.Append(':').Append(value.ToString(CultureInfo.InvariantCulture));
        }

        static void AppendBoolean(StringBuilder json, string name, bool value)
        {
            AppendEscaped(json, name); json.Append(':').Append(value ? "true" : "false");
        }

        static void AppendEscaped(StringBuilder json, string value)
        {
            json.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '"': json.Append("\\\""); break;
                    case '\\': json.Append("\\\\"); break;
                    case '\b': json.Append("\\b"); break;
                    case '\f': json.Append("\\f"); break;
                    case '\n': json.Append("\\n"); break;
                    case '\r': json.Append("\\r"); break;
                    case '\t': json.Append("\\t"); break;
                    default:
                        if (c < 32) json.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else json.Append(c);
                        break;
                }
            }
            json.Append('"');
        }
    }

    public sealed class EconomySimulationCoordinatorResult
    {
        public bool LoadSucceeded { get; }
        public string ErrorCode { get; }
        public string Error { get; }
        public EconomySimulationReport Simulation { get; }
        public EconomyComparisonReport PrototypeComparison { get; }

        public EconomySimulationCoordinatorResult(
            bool loadSucceeded,
            string errorCode,
            string error,
            EconomySimulationReport simulation,
            EconomyComparisonReport prototypeComparison
        )
        {
            LoadSucceeded = loadSucceeded;
            ErrorCode = errorCode ?? string.Empty;
            Error = error ?? string.Empty;
            Simulation = simulation;
            PrototypeComparison = prototypeComparison;
        }
    }
}
