using System;
using System.Collections.Generic;

namespace LegendsOfRus.EconomyCore
{
    public static class EconomyCatalogValidator
    {
        public const int CurrentSchemaVersion = 1;

        public static EconomyValidationReport Validate(EconomyCatalogData catalog)
        {
            var violations = new List<EconomyViolationData>();
            if (catalog == null)
            {
                AddError(violations, EconomyViolationCodes.NullCatalog, "$", "Catalog is null.");
                return Build(violations);
            }

            if (catalog.schemaVersion != CurrentSchemaVersion)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.UnsupportedSchema,
                    "schemaVersion",
                    "Only schema version 1 is supported."
                );
            }

            RequireId(violations, catalog.catalogId, "catalogId");
            if (!catalog.proposalOnly)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.ProposalOnlyRequired,
                    "proposalOnly",
                    "Step 1 catalogs must be explicitly proposal-only."
                );
            }
            if (catalog.authoritative)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.AuthoritativeForbidden,
                    "authoritative",
                    "Step 1 cannot make an economy catalog authoritative."
                );
            }

            var currencyIds = ValidateCurrencies(catalog.currencies, violations);
            ValidateAmounts(
                catalog.startingBalances,
                currencyIds,
                "startingBalances",
                true,
                violations
            );
            ValidateStartingBalanceCoverage(catalog.startingBalances, currencyIds, violations);
            ValidateEnergy(catalog.energy, violations);
            var boosterIds = ValidateBoosters(catalog.boosters, currencyIds, violations);
            ValidateContinue(catalog.continueRule, currencyIds, violations);
            ValidateDaily(catalog.dailyReward, currencyIds, violations);
            ValidateDoubling(catalog.rewardDoubling, currencyIds, violations);
            ValidateProgression(catalog.progression, currencyIds, boosterIds, violations);
            ValidateAggregateArithmetic(catalog, violations);

            return Build(violations);
        }

        static HashSet<string> ValidateCurrencies(
            EconomyCurrencyData[] currencies,
            List<EconomyViolationData> violations
        )
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (currencies == null || currencies.Length == 0)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.InvalidValue,
                    "currencies",
                    "At least one currency is required."
                );
                return ids;
            }

            for (int i = 0; i < currencies.Length; i++)
            {
                EconomyCurrencyData currency = currencies[i];
                string path = "currencies[" + i + "]";
                if (currency == null)
                {
                    AddError(violations, EconomyViolationCodes.InvalidValue, path, "Currency is null.");
                    continue;
                }

                if (!RequireId(violations, currency.id, path + ".id"))
                    continue;
                if (!ids.Add(currency.id))
                {
                    AddError(
                        violations,
                        EconomyViolationCodes.DuplicateId,
                        path + ".id",
                        "Currency id is duplicated."
                    );
                }
            }
            return ids;
        }

        static HashSet<string> ValidateBoosters(
            EconomyBoosterData[] boosters,
            HashSet<string> currencyIds,
            List<EconomyViolationData> violations
        )
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (boosters == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "boosters", "Boosters array is null.");
                return ids;
            }

            for (int i = 0; i < boosters.Length; i++)
            {
                EconomyBoosterData booster = boosters[i];
                string path = "boosters[" + i + "]";
                if (booster == null)
                {
                    AddError(violations, EconomyViolationCodes.InvalidValue, path, "Booster is null.");
                    continue;
                }
                if (RequireId(violations, booster.id, path + ".id") && !ids.Add(booster.id))
                {
                    AddError(violations, EconomyViolationCodes.DuplicateId, path + ".id", "Booster id is duplicated.");
                }
                RequireNonNegative(violations, booster.startingQuantity, path + ".startingQuantity");

                if (booster.purchaseEnabled && !booster.priceDefined)
                {
                    AddError(
                        violations,
                        EconomyViolationCodes.PriceUndefined,
                        path + ".priceDefined",
                        "An enabled booster purchase requires an explicit price."
                    );
                }
                if (booster.priceDefined)
                    ValidatePrice(booster.price, currencyIds, path + ".price", violations);
            }
            return ids;
        }

        static void ValidateContinue(
            EconomyContinueData rule,
            HashSet<string> currencyIds,
            List<EconomyViolationData> violations
        )
        {
            if (rule == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "continueRule", "Continue rule is null.");
                return;
            }
            if (rule.enabled && !rule.priceDefined)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.PriceUndefined,
                    "continueRule.priceDefined",
                    "An enabled paid continue requires an explicit price."
                );
            }
            if (rule.priceDefined)
                ValidatePrice(rule.price, currencyIds, "continueRule.price", violations);
            if (rule.grantedMoves < 0)
                AddError(violations, EconomyViolationCodes.NegativeValue, "continueRule.grantedMoves", "Granted moves cannot be negative.");
            if (rule.maxPerAttempt < 0)
                AddError(violations, EconomyViolationCodes.NegativeValue, "continueRule.maxPerAttempt", "Continue limit cannot be negative.");
            if (rule.enabled && rule.grantedMoves <= 0)
                AddError(violations, EconomyViolationCodes.InvalidValue, "continueRule.grantedMoves", "An enabled continue must grant at least one move.");
            if (rule.enabled && rule.maxPerAttempt <= 0)
                AddError(violations, EconomyViolationCodes.InvalidValue, "continueRule.maxPerAttempt", "An enabled continue must declare a positive per-attempt limit.");
            if (!rule.freeExitAndRetry)
            {
                AddError(
                    violations,
                    EconomyViolationCodes.RetryPathRequired,
                    "continueRule.freeExitAndRetry",
                    "Normal progression must retain a purchase-free exit and retry path."
                );
            }
        }

        static void ValidateDaily(
            EconomyDailyRewardData daily,
            HashSet<string> currencyIds,
            List<EconomyViolationData> violations
        )
        {
            if (daily == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "dailyReward", "Daily reward is null.");
                return;
            }
            if (daily.enabled && daily.cadenceUtcDays <= 0)
                AddError(violations, EconomyViolationCodes.InvalidValue, "dailyReward.cadenceUtcDays", "Enabled daily cadence must be positive.");
            RequireNonNegative(violations, daily.energyGrant, "dailyReward.energyGrant");
            ValidateAmounts(daily.grants, currencyIds, "dailyReward.grants", true, violations);
        }

        static void ValidateDoubling(
            EconomyRewardDoublingData doubling,
            HashSet<string> currencyIds,
            List<EconomyViolationData> violations
        )
        {
            if (doubling == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "rewardDoubling", "Reward doubling rule is null.");
                return;
            }
            if (!doubling.enabled) return;
            if (string.IsNullOrEmpty(doubling.eligibleCurrencyId) || !currencyIds.Contains(doubling.eligibleCurrencyId))
                AddError(violations, EconomyViolationCodes.MissingReference, "rewardDoubling.eligibleCurrencyId", "Reward doubling currency is unknown.");
            if (doubling.multiplier < 2)
                AddError(violations, EconomyViolationCodes.InvalidValue, "rewardDoubling.multiplier", "An enabled multiplier must be at least two.");
            if (doubling.maxClaimsPerReward < 1)
                AddError(violations, EconomyViolationCodes.InvalidValue, "rewardDoubling.maxClaimsPerReward", "An enabled reward must allow at least one claim.");
        }

        static void ValidateEnergy(EconomyEnergyData energy, List<EconomyViolationData> violations)
        {
            if (energy == null)
            {
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy", "Energy configuration is null.");
                return;
            }
            if (energy.capacity <= 0)
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy.capacity", "Capacity must be positive.");
            if (energy.startingAmount < 0 || energy.startingAmount > energy.capacity)
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy.startingAmount", "Starting energy must be within capacity.");
            if (energy.levelEntryCost < 0 || energy.levelEntryCost > energy.capacity)
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy.levelEntryCost", "Entry cost must be within capacity.");
            if (energy.retryCost < 0 || energy.retryCost > energy.capacity)
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy.retryCost", "Retry cost must be within capacity.");
            if (energy.refillIntervalMinutes <= 0)
                AddError(violations, EconomyViolationCodes.EnergyInvalid, "energy.refillIntervalMinutes", "Refill interval must be positive.");
        }

        static void ValidateProgression(
            EconomyProgressionData progression,
            HashSet<string> currencyIds,
            HashSet<string> boosterIds,
            List<EconomyViolationData> violations
        )
        {
            if (progression == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "progression", "Progression is null.");
                return;
            }
            if (progression.chapterId <= 0)
                AddError(violations, EconomyViolationCodes.InvalidValue, "progression.chapterId", "Chapter id must be positive.");
            if (progression.normalProgressRequiresPurchase)
                AddError(violations, EconomyViolationCodes.PurchaseRequired, "progression.normalProgressRequiresPurchase", "Normal progression cannot require a purchase.");
            if (progression.levels == null || progression.levels.Length == 0)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, "progression.levels", "At least one progression level is required.");
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < progression.levels.Length; i++)
            {
                EconomyProgressionLevelData level = progression.levels[i];
                string path = "progression.levels[" + i + "]";
                if (level == null)
                {
                    AddError(violations, EconomyViolationCodes.InvalidValue, path, "Progression level is null.");
                    continue;
                }
                if (RequireId(violations, level.id, path + ".id") && !ids.Add(level.id))
                    AddError(violations, EconomyViolationCodes.DuplicateId, path + ".id", "Progression level id is duplicated.");
                if (level.levelNumber != i + 1)
                    AddError(violations, EconomyViolationCodes.ProgressionSequence, path + ".levelNumber", "Progression levels must be contiguous and ordered from one.");
                if (level.requiresPurchase)
                    AddError(violations, EconomyViolationCodes.PurchaseRequired, path + ".requiresPurchase", "Normal progression cannot require a purchase.");
                if (level.requiresStore)
                    AddError(violations, EconomyViolationCodes.StoreRequired, path + ".requiresStore", "Normal progression cannot require a store.");
                if (level.requiresAd)
                    AddError(violations, EconomyViolationCodes.AdRequired, path + ".requiresAd", "Normal progression cannot require an ad.");
                RequireNonNegative(violations, level.requiredBoosterQuantity, path + ".requiredBoosterQuantity");
                if (level.requiredBoosterQuantity > 0 && (string.IsNullOrEmpty(level.requiredBoosterId) || !boosterIds.Contains(level.requiredBoosterId)))
                    AddError(violations, EconomyViolationCodes.MissingReference, path + ".requiredBoosterId", "Required booster is unknown.");
                ValidateAmounts(level.firstClearGrants, currencyIds, path + ".firstClearGrants", true, violations);
                ValidateBoosterAmounts(level.firstClearBoosterGrants, boosterIds, path + ".firstClearBoosterGrants", violations);
            }
        }

        static void ValidateAmounts(
            EconomyAmountData[] amounts,
            HashSet<string> knownIds,
            string path,
            bool requireUnique,
            List<EconomyViolationData> violations
        )
        {
            if (amounts == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, path, "Amount array is null.");
                return;
            }
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < amounts.Length; i++)
            {
                EconomyAmountData amount = amounts[i];
                string itemPath = path + "[" + i + "]";
                if (amount == null)
                {
                    AddError(violations, EconomyViolationCodes.InvalidValue, itemPath, "Amount is null.");
                    continue;
                }
                if (string.IsNullOrEmpty(amount.id) || !knownIds.Contains(amount.id))
                    AddError(violations, EconomyViolationCodes.MissingReference, itemPath + ".id", "Currency is unknown.");
                else if (requireUnique && !seen.Add(amount.id))
                    AddError(violations, EconomyViolationCodes.DuplicateId, itemPath + ".id", "Balance id is duplicated.");
                RequireNonNegative(violations, amount.amount, itemPath + ".amount");
            }
        }

        static void ValidateStartingBalanceCoverage(
            EconomyAmountData[] balances,
            HashSet<string> currencyIds,
            List<EconomyViolationData> violations
        )
        {
            var present = new HashSet<string>(StringComparer.Ordinal);
            if (balances != null)
            {
                for (int i = 0; i < balances.Length; i++)
                {
                    EconomyAmountData balance = balances[i];
                    if (balance != null && !string.IsNullOrEmpty(balance.id))
                        present.Add(balance.id);
                }
            }
            foreach (string currencyId in currencyIds)
            {
                if (!present.Contains(currencyId))
                {
                    AddError(
                        violations,
                        EconomyViolationCodes.MissingReference,
                        "startingBalances",
                        "Starting balance is missing for currency '" + currencyId + "'."
                    );
                }
            }
        }

        static void ValidateBoosterAmounts(
            EconomyBoosterAmountData[] amounts,
            HashSet<string> knownIds,
            string path,
            List<EconomyViolationData> violations
        )
        {
            if (amounts == null)
            {
                AddError(violations, EconomyViolationCodes.InvalidValue, path, "Booster amount array is null.");
                return;
            }
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < amounts.Length; i++)
            {
                EconomyBoosterAmountData amount = amounts[i];
                string itemPath = path + "[" + i + "]";
                if (amount == null)
                {
                    AddError(violations, EconomyViolationCodes.InvalidValue, itemPath, "Booster amount is null.");
                    continue;
                }
                if (string.IsNullOrEmpty(amount.id) || !knownIds.Contains(amount.id))
                    AddError(violations, EconomyViolationCodes.MissingReference, itemPath + ".id", "Booster is unknown.");
                else if (!seen.Add(amount.id))
                    AddError(violations, EconomyViolationCodes.DuplicateId, itemPath + ".id", "Booster grant id is duplicated.");
                RequireNonNegative(violations, amount.amount, itemPath + ".amount");
            }
        }

        static void ValidatePrice(
            EconomyAmountData price,
            HashSet<string> currencyIds,
            string path,
            List<EconomyViolationData> violations
        )
        {
            if (price == null)
            {
                AddError(violations, EconomyViolationCodes.PriceUndefined, path, "Price is null.");
                return;
            }
            if (string.IsNullOrEmpty(price.id) || !currencyIds.Contains(price.id))
                AddError(violations, EconomyViolationCodes.MissingReference, path + ".id", "Price currency is unknown.");
            if (price.amount <= 0)
                AddError(violations, EconomyViolationCodes.InvalidValue, path + ".amount", "Defined price must be positive.");
        }

        static void ValidateAggregateArithmetic(EconomyCatalogData catalog, List<EconomyViolationData> violations)
        {
            try
            {
                var sums = new Dictionary<string, long>(StringComparer.Ordinal);
                AddChecked(sums, catalog.startingBalances);
                if (catalog.dailyReward != null)
                    AddChecked(sums, catalog.dailyReward.grants);
                if (catalog.progression != null && catalog.progression.levels != null)
                {
                    for (int i = 0; i < catalog.progression.levels.Length; i++)
                    {
                        EconomyProgressionLevelData level = catalog.progression.levels[i];
                        if (level != null) AddChecked(sums, level.firstClearGrants);
                    }
                }
                if (
                    catalog.rewardDoubling != null &&
                    catalog.rewardDoubling.enabled &&
                    catalog.rewardDoubling.multiplier > 1 &&
                    catalog.rewardDoubling.maxClaimsPerReward > 0 &&
                    catalog.progression != null &&
                    catalog.progression.levels != null
                )
                {
                    long claimsFactor = checked(
                        (catalog.rewardDoubling.multiplier - 1L) *
                        catalog.rewardDoubling.maxClaimsPerReward
                    );
                    string eligibleId = catalog.rewardDoubling.eligibleCurrencyId ?? string.Empty;
                    for (int i = 0; i < catalog.progression.levels.Length; i++)
                    {
                        EconomyProgressionLevelData level = catalog.progression.levels[i];
                        if (level == null || level.firstClearGrants == null) continue;
                        for (int j = 0; j < level.firstClearGrants.Length; j++)
                        {
                            EconomyAmountData grant = level.firstClearGrants[j];
                            if (
                                grant == null ||
                                grant.amount < 0 ||
                                !string.Equals(grant.id, eligibleId, StringComparison.Ordinal)
                            )
                            {
                                continue;
                            }
                            long bonus = checked(grant.amount * claimsFactor);
                            long current;
                            sums.TryGetValue(eligibleId, out current);
                            sums[eligibleId] = checked(current + bonus);
                        }
                    }
                }

                var boosterSums = new Dictionary<string, long>(StringComparer.Ordinal);
                if (catalog.boosters != null)
                {
                    for (int i = 0; i < catalog.boosters.Length; i++)
                    {
                        EconomyBoosterData booster = catalog.boosters[i];
                        if (booster != null && !string.IsNullOrEmpty(booster.id) && booster.startingQuantity >= 0)
                            boosterSums[booster.id] = booster.startingQuantity;
                    }
                }
                if (catalog.progression != null && catalog.progression.levels != null)
                {
                    for (int i = 0; i < catalog.progression.levels.Length; i++)
                    {
                        EconomyProgressionLevelData level = catalog.progression.levels[i];
                        if (level == null || level.firstClearBoosterGrants == null) continue;
                        for (int j = 0; j < level.firstClearBoosterGrants.Length; j++)
                        {
                            EconomyBoosterAmountData grant = level.firstClearBoosterGrants[j];
                            if (grant == null || string.IsNullOrEmpty(grant.id) || grant.amount < 0) continue;
                            long current;
                            boosterSums.TryGetValue(grant.id, out current);
                            boosterSums[grant.id] = checked(current + grant.amount);
                        }
                    }
                }
            }
            catch (OverflowException)
            {
                AddError(violations, EconomyViolationCodes.Overflow, "$", "Configured guaranteed currency totals overflow Int64.");
            }
        }

        static void AddChecked(Dictionary<string, long> sums, EconomyAmountData[] amounts)
        {
            if (amounts == null) return;
            for (int i = 0; i < amounts.Length; i++)
            {
                EconomyAmountData amount = amounts[i];
                if (amount == null || string.IsNullOrEmpty(amount.id) || amount.amount < 0) continue;
                long value;
                sums.TryGetValue(amount.id, out value);
                sums[amount.id] = checked(value + amount.amount);
            }
        }

        static bool RequireId(List<EconomyViolationData> violations, string value, string path)
        {
            if (!string.IsNullOrWhiteSpace(value)) return true;
            AddError(violations, EconomyViolationCodes.MissingId, path, "A stable non-empty id is required.");
            return false;
        }

        static void RequireNonNegative(List<EconomyViolationData> violations, long value, string path)
        {
            if (value >= 0) return;
            AddError(violations, EconomyViolationCodes.NegativeValue, path, "Value cannot be negative.");
        }

        static void AddError(List<EconomyViolationData> violations, string code, string path, string message)
        {
            violations.Add(new EconomyViolationData(code, EconomyViolationSeverity.Error, path, message));
        }

        static EconomyValidationReport Build(List<EconomyViolationData> violations)
        {
            EconomyViolationData[] ordered = violations.ToArray();
            Array.Sort(ordered, CompareViolations);
            return new EconomyValidationReport
            {
                verdict = ordered.Length == 0 ? EconomyVerdict.Pass : EconomyVerdict.Fail,
                violations = ordered
            };
        }

        static int CompareViolations(EconomyViolationData left, EconomyViolationData right)
        {
            int path = string.Compare(left.path, right.path, StringComparison.Ordinal);
            if (path != 0) return path;
            int code = string.Compare(left.code, right.code, StringComparison.Ordinal);
            if (code != 0) return code;
            return string.Compare(left.message, right.message, StringComparison.Ordinal);
        }
    }
}
