#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.EconomyCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step15EconomyConfigurationSimulationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.15 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            EconomyCatalogLoadResult load = EconomyCatalogLoader.LoadProposal();

            Check(
                "proposal resource loads and validates",
                () => load.Succeeded && load.Validation.verdict == EconomyVerdict.Pass,
                ref passed,
                ref total,
                failures
            );
            Check(
                "catalog remains explicitly proposal-only",
                () =>
                    load.Catalog.schemaVersion == EconomyCatalogValidator.CurrentSchemaVersion &&
                    load.Catalog.catalogId == "v1.15-step01-proposal" &&
                    load.Catalog.proposalOnly &&
                    !load.Catalog.authoritative,
                ref passed,
                ref total,
                failures
            );
            Check(
                "prototype starting wallet and energy are captured exactly",
                () =>
                    Balance(load.Catalog.startingBalances, "gold") == 12540 &&
                    Balance(load.Catalog.startingBalances, "gems") == 350 &&
                    load.Catalog.energy.startingAmount == 5 &&
                    load.Catalog.energy.capacity == 5 &&
                    load.Catalog.energy.levelEntryCost == 1 &&
                    load.Catalog.energy.refillIntervalMinutes == 30,
                ref passed,
                ref total,
                failures
            );
            Check(
                "prototype continue and daily values are explicit",
                () =>
                    !load.Catalog.continueRule.enabled &&
                    load.Catalog.continueRule.priceDefined &&
                    load.Catalog.continueRule.price.id == "gems" &&
                    load.Catalog.continueRule.price.amount == 20 &&
                    load.Catalog.continueRule.grantedMoves == 5 &&
                    load.Catalog.dailyReward.cadenceUtcDays == 1 &&
                    Balance(load.Catalog.dailyReward.grants, "gold") == 500 &&
                    Balance(load.Catalog.dailyReward.grants, "gems") == 20 &&
                    load.Catalog.dailyReward.energyGrant == 1,
                ref passed,
                ref total,
                failures
            );
            Check(
                "Chapter 1 first-clear totals remain 1850 gold and 40 gems",
                () =>
                    SumProgressionCurrency(load.Catalog, "gold") == 1850 &&
                    SumProgressionCurrency(load.Catalog, "gems") == 40 &&
                    load.Catalog.progression.levels.Length == 10,
                ref passed,
                ref total,
                failures
            );
            Check(
                "simulation report is byte deterministic",
                () =>
                {
                    string first = EconomyDeterministicJson.Serialize(
                        EconomySimulator.Run(load.Catalog)
                    );
                    string second = EconomyDeterministicJson.Serialize(
                        EconomySimulator.Run(load.Catalog)
                    );
                    return first == second;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "purchase-free Chapter 1 completes after 150 refill minutes",
                () =>
                {
                    EconomySimulationReport report = EconomySimulator.Run(load.Catalog);
                    return
                        report.verdict == EconomyVerdict.Incomplete &&
                        FindCheck(report, "purchase-free-progression").verdict == EconomyVerdict.Pass &&
                        FindCheck(report, "reward-doubling-cadence").verdict == EconomyVerdict.Pass &&
                        FindCheck(report, "retry-energy-cost").verdict == EconomyVerdict.Pass &&
                        FindMetric(report, "levels-completed") == 10 &&
                        FindMetric(report, "paid-continues-applied") == 0 &&
                        FindMetric(report, "store-dependencies-used") == 0 &&
                        FindMetric(report, "ad-dependencies-used") == 0 &&
                        FindMetric(report, "purchases-used") == 0 &&
                        report.elapsedMinutes == 150 &&
                        report.endingEnergy == 0 &&
                        Balance(report.endingWallet.balances, "gold") == 14390 &&
                        Balance(report.endingWallet.balances, "gems") == 390;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "energy uses the exact 30-minute boundary and rollback guard",
                () =>
                {
                    EconomyEnergyResult before = EconomyEnergyMath.Advance(
                        load.Catalog.energy, 0, 0, 1799, 60
                    );
                    EconomyEnergyResult exact = EconomyEnergyMath.Advance(
                        load.Catalog.energy, 0, 0, 1800, 60
                    );
                    EconomyEnergyResult rollback = EconomyEnergyMath.Advance(
                        load.Catalog.energy, 2, 1000, 999, 60
                    );
                    return
                        before.valid && before.gained == 0 &&
                        exact.valid && exact.gained == 1 && exact.energy == 1 &&
                        rollback.valid && rollback.clockRollback &&
                        rollback.code == EconomyViolationCodes.ClockRollback;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "duplicate grants are rejected without a second balance change",
                DuplicateGrantIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "insufficient multi-currency spends are atomic",
                InsufficientSpendIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "daily cadence rejects repeated and invalid day indices",
                () => DailyCadenceIsGuarded(load.Catalog),
                ref passed,
                ref total,
                failures
            );
            Check(
                "missing required catalog fields fail closed",
                () =>
                {
                    var truncated = new EconomyCatalogData();
                    EconomyValidationReport validation = EconomyCatalogValidator.Validate(truncated);
                    EconomySimulationReport simulation = EconomySimulator.Run(truncated);
                    return
                        validation.verdict == EconomyVerdict.Fail &&
                        ContainsCode(validation, EconomyViolationCodes.UnsupportedSchema) &&
                        ContainsCode(validation, EconomyViolationCodes.ProposalOnlyRequired) &&
                        ContainsCode(validation, EconomyViolationCodes.AuthoritativeForbidden) &&
                        simulation.verdict == EconomyVerdict.Fail;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "active commercial rules and duplicate grant vectors fail validation",
                () => InvalidActiveRulesFailClosed(load.SourceJson),
                ref passed,
                ref total,
                failures
            );
            Check(
                "partially funded requested continues fail explicitly",
                () => PartialContinueFundingFails(load.SourceJson),
                ref passed,
                ref total,
                failures
            );
            Check(
                "runtime comparison remains incomplete for dormant proposal values",
                ComparisonStaysIncomplete,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.15 Step 1 economy configuration and simulation " +
                    $"self-tests passed: {passed}/{total}."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.15 Step 1 self-tests passed: {passed}/{total}",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.15 Step 1 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
            if (Application.isBatchMode)
                throw new InvalidOperationException("v1.15 Step 1 self-tests failed.");
        }

        static bool DuplicateGrantIsAtomic()
        {
            var ledger = new EconomyLedger(new[] { Amount("gold", 10) });
            EconomyTransactionResult first = ledger.TryGrant(
                "mail:welcome",
                new[] { Amount("gold", 250) }
            );
            EconomyTransactionResult duplicate = ledger.TryGrant(
                "mail:welcome",
                new[] { Amount("gold", 250) }
            );
            return
                first.applied &&
                !duplicate.applied &&
                duplicate.code == EconomyViolationCodes.DuplicateGrant &&
                ledger.GetBalance("gold") == 260;
        }

        static bool InsufficientSpendIsAtomic()
        {
            var ledger = new EconomyLedger(new[]
            {
                Amount("gold", 10),
                Amount("gems", 5),
            });
            EconomyTransactionResult denied = ledger.TrySpend(
                "mixed-spend",
                new[] { Amount("gold", 7), Amount("gems", 6) }
            );
            return
                !denied.applied &&
                denied.code == EconomyViolationCodes.InsufficientBalance &&
                ledger.GetBalance("gold") == 10 &&
                ledger.GetBalance("gems") == 5 &&
                denied.snapshot.appliedKeys.Length == 0;
        }

        static bool DailyCadenceIsGuarded(EconomyCatalogData catalog)
        {
            var ledger = new EconomyLedger(new[] { Amount("gold", 0), Amount("gems", 0) });
            EconomyDailyClaimResult first = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 100, 99, 4, 5, ledger
            );
            EconomyDailyClaimResult duplicate = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 100, first.claimDayIndex, first.energy, 5, ledger
            );
            EconomyDailyClaimResult invalid = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, -1, -1, 4, 5, ledger
            );
            return
                first.claimed &&
                !duplicate.claimed &&
                duplicate.code == EconomyViolationCodes.DailyAlreadyClaimed &&
                !invalid.claimed &&
                invalid.code == EconomyViolationCodes.InvalidTransaction &&
                ledger.GetBalance("gold") == 500 &&
                ledger.GetBalance("gems") == 20;
        }

        static bool ComparisonStaysIncomplete()
        {
            EconomySimulationCoordinatorResult first =
                EconomySimulationCoordinator.LoadAndRunProposal();
            EconomySimulationCoordinatorResult second =
                EconomySimulationCoordinator.LoadAndRunProposal();
            return
                first.LoadSucceeded &&
                first.PrototypeComparison.verdict == EconomyVerdict.Incomplete &&
                EconomySimulationCoordinator.ToDeterministicJson(first) ==
                    EconomySimulationCoordinator.ToDeterministicJson(second);
        }

        static bool InvalidActiveRulesFailClosed(string sourceJson)
        {
            EconomyCatalogData catalog = JsonUtility.FromJson<EconomyCatalogData>(sourceJson);
            catalog.continueRule.enabled = true;
            catalog.continueRule.grantedMoves = 0;
            catalog.continueRule.maxPerAttempt = 0;
            catalog.rewardDoubling.multiplier = 1;
            catalog.rewardDoubling.maxClaimsPerReward = 0;
            catalog.dailyReward.grants = new[] { Amount("gold", 1), Amount("gold", 2) };
            EconomyValidationReport validation = EconomyCatalogValidator.Validate(catalog);
            return
                validation.verdict == EconomyVerdict.Fail &&
                ContainsCode(validation, EconomyViolationCodes.DuplicateId) &&
                ContainsCode(validation, EconomyViolationCodes.InvalidValue);
        }

        static bool PartialContinueFundingFails(string sourceJson)
        {
            EconomyCatalogData catalog = JsonUtility.FromJson<EconomyCatalogData>(sourceJson);
            catalog.continueRule.enabled = true;
            for (int i = 0; i < catalog.startingBalances.Length; i++)
                if (catalog.startingBalances[i].id == "gems")
                    catalog.startingBalances[i].amount = 30;
            EconomySimulationReport report = EconomySimulator.Run(
                catalog,
                new EconomySimulationRequest
                {
                    scenarioId = "partial-continue-funding",
                    ticksPerMinute = 1,
                    optionalPaidContinuesPerLevel = 1,
                }
            );
            return
                report.validation.verdict == EconomyVerdict.Pass &&
                report.verdict == EconomyVerdict.Fail &&
                FindCheck(report, "requested-continue-fulfillment").code ==
                    EconomyViolationCodes.InsufficientBalance &&
                Balance(report.endingWallet.balances, "gems") >= 0;
        }

        static long SumProgressionCurrency(EconomyCatalogData catalog, string id)
        {
            long total = 0;
            for (int i = 0; i < catalog.progression.levels.Length; i++)
            {
                EconomyAmountData[] grants = catalog.progression.levels[i].firstClearGrants;
                for (int j = 0; j < grants.Length; j++)
                    if (grants[j].id == id) total += grants[j].amount;
            }
            return total;
        }

        static long Balance(EconomyAmountData[] values, string id)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i].id == id) return values[i].amount;
            throw new InvalidOperationException("Missing economy amount: " + id);
        }

        static EconomyCheckData FindCheck(EconomySimulationReport report, string id)
        {
            for (int i = 0; i < report.checks.Length; i++)
                if (report.checks[i].id == id) return report.checks[i];
            throw new InvalidOperationException("Missing economy check: " + id);
        }

        static long FindMetric(EconomySimulationReport report, string id)
        {
            for (int i = 0; i < report.metrics.Length; i++)
                if (report.metrics[i].id == id) return report.metrics[i].value;
            throw new InvalidOperationException("Missing economy metric: " + id);
        }

        static bool ContainsCode(EconomyValidationReport report, string code)
        {
            for (int i = 0; i < report.violations.Length; i++)
                if (report.violations[i].code == code) return true;
            return false;
        }

        static EconomyAmountData Amount(string id, long amount)
        {
            return new EconomyAmountData(id, amount);
        }

        static void Check(
            string name,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test()) passed++;
                else failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
