using System;
using System.Collections.Generic;

namespace LegendsOfRus.EconomyCore
{
    public static class EconomySimulator
    {
        public static EconomySimulationReport Run(EconomyCatalogData catalog)
        {
            return Run(catalog, new EconomySimulationRequest());
        }

        public static EconomySimulationReport Run(
            EconomyCatalogData catalog,
            EconomySimulationRequest request
        )
        {
            if (request == null) request = new EconomySimulationRequest();
            EconomyValidationReport validation = EconomyCatalogValidator.Validate(catalog);
            var report = new EconomySimulationReport
            {
                catalogId = catalog == null ? string.Empty : catalog.catalogId ?? string.Empty,
                scenarioId = request.scenarioId ?? string.Empty,
                validation = validation
            };
            var checks = new List<EconomyCheckData>();
            var metrics = new List<EconomyMetricData>();
            var trace = new List<string>();

            checks.Add(Check(
                "catalog-validation",
                validation.verdict,
                validation.verdict == EconomyVerdict.Pass ? string.Empty : EconomyViolationCodes.InvalidCatalog,
                validation.verdict == EconomyVerdict.Pass
                    ? "Proposal catalog is valid."
                    : "Invalid catalogs cannot produce a passing simulation."
            ));

            if (validation.verdict != EconomyVerdict.Pass)
            {
                report.verdict = EconomyVerdict.Fail;
                report.checks = SortChecks(checks);
                report.metrics = new EconomyMetricData[0];
                report.trace = new[] { "simulation:rejected-invalid-catalog" };
                return report;
            }
            if (request.ticksPerMinute <= 0 || request.optionalPaidContinuesPerLevel < 0)
            {
                checks.Add(Check(
                    "simulation-request",
                    EconomyVerdict.Fail,
                    EconomyViolationCodes.InvalidValue,
                    "Ticks per minute must be positive and optional continue count cannot be negative."
                ));
                report.verdict = EconomyVerdict.Fail;
                report.checks = SortChecks(checks);
                report.trace = new[] { "simulation:rejected-invalid-request" };
                return report;
            }

            var ledger = new EconomyLedger(catalog.startingBalances);
            var boosters = CreateBoosterBalances(catalog.boosters);
            long energy = catalog.energy.startingAmount;
            long nowTick = request.startTick;
            long lastRefillTick = request.startTick;
            long elapsedMinutes = 0;
            int completed = 0;
            int continuesApplied = 0;
            bool requestedContinueSpendFailed = false;
            string requestedContinueFailureCode = string.Empty;
            string progressionFailure = string.Empty;

            trace.Add("simulation:start:" + report.scenarioId);
            EconomyProgressionLevelData[] levels = catalog.progression.levels;
            try
            {
                for (int i = 0; i < levels.Length; i++)
                {
                    EconomyProgressionLevelData level = levels[i];
                    trace.Add("level:" + level.levelNumber + ":begin");
                    if (
                        (level.requiresStore && !request.allowStore) ||
                        (level.requiresAd && !request.allowAds) ||
                        (level.requiresPurchase && !request.allowPurchases)
                    )
                    {
                        progressionFailure = EconomyViolationCodes.UnreachableProgression;
                        trace.Add("level:" + level.levelNumber + ":blocked-external-dependency");
                        break;
                    }

                    long entryCost = catalog.energy.levelEntryCost;
                    if (energy < entryCost)
                    {
                        long missing = entryCost - energy;
                        long waitMinutes = checked(missing * catalog.energy.refillIntervalMinutes);
                        long waitTicks = checked(waitMinutes * request.ticksPerMinute);
                        nowTick = checked(nowTick + waitTicks);
                        elapsedMinutes = checked(elapsedMinutes + waitMinutes);
                        EconomyEnergyResult refill = EconomyEnergyMath.Advance(
                            catalog.energy,
                            energy,
                            lastRefillTick,
                            nowTick,
                            request.ticksPerMinute
                        );
                        if (!refill.valid || refill.energy < entryCost)
                        {
                            progressionFailure = refill.code;
                            trace.Add("level:" + level.levelNumber + ":energy-refill-failed:" + refill.code);
                            break;
                        }
                        energy = refill.energy;
                        lastRefillTick = refill.lastRefillTick;
                        trace.Add("level:" + level.levelNumber + ":wait-minutes:" + waitMinutes);
                    }
                    energy = checked(energy - entryCost);

                    if (request.consumeRequiredBoosters && level.requiredBoosterQuantity > 0)
                    {
                        long available;
                        if (
                            !boosters.TryGetValue(level.requiredBoosterId, out available) ||
                            available < level.requiredBoosterQuantity
                        )
                        {
                            progressionFailure = EconomyViolationCodes.UnreachableProgression;
                            trace.Add("level:" + level.levelNumber + ":required-booster-unaffordable");
                            break;
                        }
                        boosters[level.requiredBoosterId] = checked(available - level.requiredBoosterQuantity);
                    }

                    if (request.optionalPaidContinuesPerLevel > 0)
                    {
                        if (!catalog.continueRule.enabled || !catalog.continueRule.priceDefined)
                        {
                            trace.Add("level:" + level.levelNumber + ":optional-continue-inactive");
                        }
                        else
                        {
                            int allowed = request.optionalPaidContinuesPerLevel;
                            if (catalog.continueRule.maxPerAttempt > 0 && allowed > catalog.continueRule.maxPerAttempt)
                                allowed = catalog.continueRule.maxPerAttempt;
                            for (int continueIndex = 0; continueIndex < allowed; continueIndex++)
                            {
                                EconomyTransactionResult spend = ledger.TrySpend(
                                    "continue:" + level.levelNumber + ":" + continueIndex,
                                    new[] { catalog.continueRule.price }
                                );
                                if (!spend.applied)
                                {
                                    requestedContinueSpendFailed = true;
                                    requestedContinueFailureCode = spend.code;
                                    trace.Add("level:" + level.levelNumber + ":optional-continue-unaffordable:" + spend.code);
                                    break;
                                }
                                continuesApplied++;
                            }
                        }
                    }

                    EconomyTransactionResult reward = ledger.TryGrant(
                        "first-clear:" + level.id,
                        level.firstClearGrants ?? new EconomyAmountData[0]
                    );
                    if (!reward.applied)
                    {
                        progressionFailure = reward.code;
                        trace.Add("level:" + level.levelNumber + ":reward-failed:" + reward.code);
                        break;
                    }
                    if (!GrantBoosters(boosters, level.firstClearBoosterGrants))
                    {
                        progressionFailure = EconomyViolationCodes.Overflow;
                        trace.Add("level:" + level.levelNumber + ":booster-reward-overflow");
                        break;
                    }
                    completed++;
                    trace.Add("level:" + level.levelNumber + ":complete");
                }
            }
            catch (OverflowException)
            {
                progressionFailure = EconomyViolationCodes.Overflow;
                trace.Add("simulation:arithmetic-overflow");
            }

            EconomyVerdict progressionVerdict =
                string.IsNullOrEmpty(progressionFailure) && completed == levels.Length
                    ? EconomyVerdict.Pass
                    : EconomyVerdict.Fail;
            checks.Add(Check(
                "purchase-free-progression",
                progressionVerdict,
                progressionVerdict == EconomyVerdict.Pass ? string.Empty : progressionFailure,
                progressionVerdict == EconomyVerdict.Pass
                    ? "All configured levels are reachable with store, ads, and purchases disabled."
                    : "Chapter progression did not reach every configured level."
            ));
            checks.Add(EvaluateBoosterPriceAffordability(catalog.boosters, ledger));
            checks.Add(Check(
                "booster-inventory-affordability",
                progressionVerdict,
                progressionVerdict == EconomyVerdict.Pass ? string.Empty : progressionFailure,
                progressionVerdict == EconomyVerdict.Pass
                    ? "All mandatory booster quantities, if any, were funded by detached inventory."
                    : "A mandatory booster quantity was not reachable without an external dependency."
            ));
            checks.Add(EvaluateContinueAffordability(catalog.continueRule, ledger));
            if (request.optionalPaidContinuesPerLevel > 0 && catalog.continueRule.enabled)
            {
                checks.Add(Check(
                    "requested-continue-fulfillment",
                    requestedContinueSpendFailed ? EconomyVerdict.Fail : EconomyVerdict.Pass,
                    requestedContinueSpendFailed ? requestedContinueFailureCode : string.Empty,
                    requestedContinueSpendFailed
                        ? "At least one explicitly requested continue could not be funded atomically."
                        : "Every explicitly requested continue was funded and applied atomically."
                ));
            }
            checks.Add(EvaluateEnergyCadence(catalog.energy, request.ticksPerMinute));
            checks.Add(EvaluateRetryPath(catalog));
            checks.Add(EvaluateFirstClearIdempotency(levels, ledger, progressionVerdict, progressionFailure));
            checks.Add(EvaluateDailyCadence(catalog));
            checks.Add(EvaluateRewardDoublingCadence(catalog));

            metrics.Add(Metric("levels-completed", completed, "levels"));
            metrics.Add(Metric("levels-configured", levels.Length, "levels"));
            metrics.Add(Metric("elapsed-wait", elapsedMinutes, "minutes"));
            metrics.Add(Metric("ending-energy", energy, "energy"));
            metrics.Add(Metric("paid-continues-applied", continuesApplied, "continues"));
            metrics.Add(Metric("store-dependencies-used", 0, "dependencies"));
            metrics.Add(Metric("ad-dependencies-used", 0, "dependencies"));
            metrics.Add(Metric("purchases-used", 0, "purchases"));

            report.checks = SortChecks(checks);
            report.metrics = SortMetrics(metrics);
            report.endingWallet = ledger.Snapshot();
            report.endingBoosters = SnapshotBoosters(boosters);
            report.endingEnergy = energy;
            report.elapsedMinutes = elapsedMinutes;
            trace.Add("simulation:end:" + completed + "/" + levels.Length);
            report.trace = trace.ToArray();
            report.verdict = Aggregate(report.checks);
            return report;
        }

        static Dictionary<string, long> CreateBoosterBalances(EconomyBoosterData[] configured)
        {
            var result = new Dictionary<string, long>(StringComparer.Ordinal);
            for (int i = 0; i < configured.Length; i++)
                result.Add(configured[i].id, configured[i].startingQuantity);
            return result;
        }

        static bool GrantBoosters(
            Dictionary<string, long> balances,
            EconomyBoosterAmountData[] grants
        )
        {
            if (grants == null) return false;
            var replacements = new Dictionary<string, long>(StringComparer.Ordinal);
            try
            {
                for (int i = 0; i < grants.Length; i++)
                {
                    EconomyBoosterAmountData grant = grants[i];
                    long current = replacements.ContainsKey(grant.id)
                        ? replacements[grant.id]
                        : balances[grant.id];
                    replacements[grant.id] = checked(current + grant.amount);
                }
                foreach (KeyValuePair<string, long> replacement in replacements)
                    balances[replacement.Key] = replacement.Value;
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        static EconomyCheckData EvaluateBoosterPriceAffordability(
            EconomyBoosterData[] boosters,
            EconomyLedger ledger
        )
        {
            if (boosters.Length == 0)
                return Check("booster-price-affordability", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "No booster pricing is configured.");

            bool foundEnabledPrice = false;
            for (int i = 0; i < boosters.Length; i++)
            {
                EconomyBoosterData booster = boosters[i];
                if (!booster.purchaseEnabled || !booster.priceDefined)
                    continue;
                foundEnabledPrice = true;
                if (ledger.GetBalance(booster.price.id) < booster.price.amount)
                    return Check("booster-price-affordability", EconomyVerdict.Fail, EconomyViolationCodes.InsufficientBalance, "At least one enabled booster price is unaffordable in the ending wallet.");
            }
            return foundEnabledPrice
                ? Check("booster-price-affordability", EconomyVerdict.Pass, string.Empty, "Every enabled booster price is affordable at least once in the ending wallet.")
                : Check("booster-price-affordability", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "Booster purchasing is disabled or prices are undefined; affordability is intentionally not inferred.");
        }

        static EconomyCheckData EvaluateContinueAffordability(
            EconomyContinueData rule,
            EconomyLedger ledger
        )
        {
            if (!rule.enabled)
                return Check("continue-affordability", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "Paid continue behavior is inactive; purchase-free retry remains available.");
            if (!rule.priceDefined)
                return Check("continue-affordability", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "Continue price is undefined.");
            return ledger.GetBalance(rule.price.id) >= rule.price.amount
                ? Check("continue-affordability", EconomyVerdict.Pass, string.Empty, "One optional continue is affordable in the ending wallet.")
                : Check("continue-affordability", EconomyVerdict.Fail, EconomyViolationCodes.InsufficientBalance, "One optional continue is not affordable in the ending wallet.");
        }

        static EconomyCheckData EvaluateFirstClearIdempotency(
            EconomyProgressionLevelData[] levels,
            EconomyLedger ledger,
            EconomyVerdict progressionVerdict,
            string progressionFailure
        )
        {
            if (progressionVerdict != EconomyVerdict.Pass || levels.Length == 0)
                return Check("first-clear-reward-cadence", EconomyVerdict.Fail, progressionFailure, "Progression did not complete, so first-clear cadence could not be certified.");
            EconomyProgressionLevelData first = levels[0];
            EconomyTransactionResult duplicate = ledger.TryGrant(
                "first-clear:" + first.id,
                first.firstClearGrants ?? new EconomyAmountData[0]
            );
            return !duplicate.applied && duplicate.code == EconomyViolationCodes.DuplicateGrant
                ? Check("first-clear-reward-cadence", EconomyVerdict.Pass, string.Empty, "A repeated first-clear key was rejected without changing the wallet.")
                : Check("first-clear-reward-cadence", EconomyVerdict.Fail, duplicate.code, "A repeated first-clear key was not rejected deterministically.");
        }

        static EconomyCheckData EvaluateEnergyCadence(
            EconomyEnergyData config,
            long ticksPerMinute
        )
        {
            try
            {
                long intervalTicks = checked(config.refillIntervalMinutes * ticksPerMinute);
                EconomyEnergyResult before = EconomyEnergyMath.Advance(
                    config,
                    0,
                    0,
                    intervalTicks - 1,
                    ticksPerMinute
                );
                EconomyEnergyResult exact = EconomyEnergyMath.Advance(
                    config,
                    0,
                    0,
                    intervalTicks,
                    ticksPerMinute
                );
                bool passed =
                    before.valid &&
                    before.energy == 0 &&
                    before.gained == 0 &&
                    exact.valid &&
                    exact.energy == 1 &&
                    exact.gained == 1;
                return Check(
                    "energy-refill-cadence",
                    passed ? EconomyVerdict.Pass : EconomyVerdict.Fail,
                    passed ? string.Empty : EconomyViolationCodes.EnergyInvalid,
                    passed
                        ? "No energy is granted before the exact interval; one unit is granted at the boundary."
                        : "The explicit refill boundary did not produce the configured cadence."
                );
            }
            catch (OverflowException)
            {
                return Check(
                    "energy-refill-cadence",
                    EconomyVerdict.Fail,
                    EconomyViolationCodes.Overflow,
                    "The configured refill boundary overflows explicit tick arithmetic."
                );
            }
        }

        static EconomyCheckData EvaluateRetryPath(EconomyCatalogData catalog)
        {
            bool free = catalog.continueRule.freeExitAndRetry;
            if (!free)
                return Check("retry-energy-cost", EconomyVerdict.Fail, EconomyViolationCodes.RetryPathRequired, "A purchase-free exit and retry path is required.");
            return catalog.energy.retryCost == 0
                ? Check("retry-energy-cost", EconomyVerdict.Pass, string.Empty, "The detached retry path consumes zero additional energy, matching the current prototype.")
                : Check("retry-energy-cost", EconomyVerdict.Incomplete, EconomyViolationCodes.EnergyInvalid, "A non-zero retry cost is configured but Step 1 does not activate or exercise runtime retry spending.");
        }

        static EconomyCheckData EvaluateDailyCadence(EconomyCatalogData catalog)
        {
            if (!catalog.dailyReward.enabled)
                return Check("daily-reward-cadence", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "Daily rewards are disabled, so claim cadence is not evaluated.");
            var ledger = new EconomyLedger(catalog.startingBalances);
            EconomyDailyClaimResult first = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward,
                0,
                -1,
                catalog.energy.startingAmount,
                catalog.energy.capacity,
                ledger
            );
            EconomyDailyClaimResult duplicate = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward,
                0,
                first.claimDayIndex,
                first.energy,
                catalog.energy.capacity,
                ledger
            );
            return first.claimed && !duplicate.claimed && duplicate.code == EconomyViolationCodes.DailyAlreadyClaimed
                ? Check("daily-reward-cadence", EconomyVerdict.Pass, string.Empty, "The first UTC-day claim succeeds and a same-day repeat is rejected.")
                : Check("daily-reward-cadence", EconomyVerdict.Fail, duplicate.code, "Daily reward cadence did not reject a same-day repeat.");
        }

        static EconomyCheckData EvaluateRewardDoublingCadence(EconomyCatalogData catalog)
        {
            EconomyRewardDoublingData rule = catalog.rewardDoubling;
            if (!rule.enabled)
                return Check("reward-doubling-cadence", EconomyVerdict.Incomplete, EconomyViolationCodes.PriceUndefined, "Reward doubling is disabled, so its cadence is not evaluated.");

            EconomyProgressionLevelData source = null;
            EconomyAmountData eligible = null;
            for (int i = 0; i < catalog.progression.levels.Length && eligible == null; i++)
            {
                EconomyProgressionLevelData candidate = catalog.progression.levels[i];
                for (int j = 0; j < candidate.firstClearGrants.Length; j++)
                {
                    EconomyAmountData grant = candidate.firstClearGrants[j];
                    if (string.Equals(grant.id, rule.eligibleCurrencyId, StringComparison.Ordinal) && grant.amount > 0)
                    {
                        source = candidate;
                        eligible = grant;
                        break;
                    }
                }
            }
            if (eligible == null)
                return Check("reward-doubling-cadence", EconomyVerdict.Incomplete, EconomyViolationCodes.MissingReference, "No positive eligible first-clear grant exists to exercise reward doubling.");

            try
            {
                long bonusAmount = checked(eligible.amount * (rule.multiplier - 1L));
                var ledger = new EconomyLedger(catalog.startingBalances);
                string key = "reward-double:" + source.id + ":0";
                var bonus = new[] { new EconomyAmountData(rule.eligibleCurrencyId, bonusAmount) };
                EconomyTransactionResult first = ledger.TryGrant(key, bonus);
                EconomyTransactionResult duplicate = ledger.TryGrant(key, bonus);
                bool passed =
                    first.applied &&
                    !duplicate.applied &&
                    duplicate.code == EconomyViolationCodes.DuplicateGrant;
                return Check(
                    "reward-doubling-cadence",
                    passed ? EconomyVerdict.Pass : EconomyVerdict.Fail,
                    passed ? string.Empty : duplicate.code,
                    passed
                        ? "One deterministic reward-double key applies once and rejects a repeated claim."
                        : "Reward-doubling idempotency did not fail closed."
                );
            }
            catch (OverflowException)
            {
                return Check("reward-doubling-cadence", EconomyVerdict.Fail, EconomyViolationCodes.Overflow, "Reward-doubling arithmetic overflowed.");
            }
        }

        static EconomyBoosterAmountData[] SnapshotBoosters(Dictionary<string, long> balances)
        {
            var result = new EconomyBoosterAmountData[balances.Count];
            int index = 0;
            foreach (KeyValuePair<string, long> balance in balances)
                result[index++] = new EconomyBoosterAmountData(balance.Key, balance.Value);
            Array.Sort(result, CompareBoosterAmounts);
            return result;
        }

        static EconomyVerdict Aggregate(EconomyCheckData[] checks)
        {
            bool incomplete = false;
            for (int i = 0; i < checks.Length; i++)
            {
                if (checks[i].verdict == EconomyVerdict.Fail) return EconomyVerdict.Fail;
                if (checks[i].verdict == EconomyVerdict.Incomplete) incomplete = true;
            }
            return incomplete ? EconomyVerdict.Incomplete : EconomyVerdict.Pass;
        }

        static EconomyCheckData Check(string id, EconomyVerdict verdict, string code, string message)
        {
            return new EconomyCheckData { id = id, verdict = verdict, code = code ?? string.Empty, message = message ?? string.Empty };
        }

        static EconomyMetricData Metric(string id, long value, string unit)
        {
            return new EconomyMetricData { id = id, value = value, unit = unit };
        }

        static EconomyCheckData[] SortChecks(List<EconomyCheckData> checks)
        {
            EconomyCheckData[] result = checks.ToArray();
            Array.Sort(result, CompareChecks);
            return result;
        }

        static EconomyMetricData[] SortMetrics(List<EconomyMetricData> metrics)
        {
            EconomyMetricData[] result = metrics.ToArray();
            Array.Sort(result, CompareMetrics);
            return result;
        }

        static int CompareChecks(EconomyCheckData left, EconomyCheckData right)
        {
            return string.Compare(left.id, right.id, StringComparison.Ordinal);
        }

        static int CompareMetrics(EconomyMetricData left, EconomyMetricData right)
        {
            return string.Compare(left.id, right.id, StringComparison.Ordinal);
        }

        static int CompareBoosterAmounts(EconomyBoosterAmountData left, EconomyBoosterAmountData right)
        {
            return string.Compare(left.id, right.id, StringComparison.Ordinal);
        }
    }

    public static class EconomyComparisonReportBuilder
    {
        public static EconomyComparisonReport Build(
            string catalogId,
            EconomyComparisonEntryData[] entries
        )
        {
            EconomyComparisonEntryData[] ordered = entries == null
                ? new EconomyComparisonEntryData[0]
                : (EconomyComparisonEntryData[])entries.Clone();
            Array.Sort(ordered, CompareEntries);
            EconomyVerdict verdict = ordered.Length == 0
                ? EconomyVerdict.Incomplete
                : EconomyVerdict.Pass;
            for (int i = 0; i < ordered.Length; i++)
            {
                if (ordered[i] == null || ordered[i].status == EconomyComparisonStatus.Conflict)
                {
                    verdict = EconomyVerdict.Fail;
                    break;
                }
                if (
                    ordered[i].status == EconomyComparisonStatus.Missing ||
                    ordered[i].status == EconomyComparisonStatus.Dormant
                )
                {
                    verdict = EconomyVerdict.Incomplete;
                }
            }
            return new EconomyComparisonReport
            {
                catalogId = catalogId ?? string.Empty,
                verdict = verdict,
                entries = ordered
            };
        }

        static int CompareEntries(EconomyComparisonEntryData left, EconomyComparisonEntryData right)
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left == null) return -1;
            if (right == null) return 1;
            int path = string.Compare(left.path, right.path, StringComparison.Ordinal);
            if (path != 0) return path;
            return string.Compare(left.source, right.source, StringComparison.Ordinal);
        }
    }
}
