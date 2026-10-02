using System;
using System.Linq;
using LegendsOfRus.EconomyCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Economy
{
    public sealed class EconomyConfigurationSimulationEditModeTests
    {
        [Test]
        public void AuthoredContract_IsVersionedAndProposalOnly()
        {
            EconomyCatalogData catalog = CreateCatalog();

            Assert.That(catalog.schemaVersion, Is.EqualTo(EconomyCatalogValidator.CurrentSchemaVersion));
            Assert.That(catalog.catalogId, Is.EqualTo("v1.15-step01-proposal"));
            Assert.That(catalog.proposalOnly, Is.True);
            Assert.That(catalog.authoritative, Is.False);
            Assert.That(catalog.currencies.Select(item => item.id), Is.EquivalentTo(new[] { "gold", "gems" }));
            Assert.That(catalog.boosters.Select(item => item.id), Is.EquivalentTo(
                new[] { "bogatyr_mace", "firebird", "living_light", "sea_pearl" }
            ));
            Assert.That(catalog.progression.levels, Has.Length.EqualTo(10));
        }

        [Test]
        public void PrototypeBaseline_CapturesCurrentValuesWithoutDrift()
        {
            EconomyCatalogData catalog = CreateCatalog();

            Assert.That(Balance(catalog.startingBalances, "gold"), Is.EqualTo(12540));
            Assert.That(Balance(catalog.startingBalances, "gems"), Is.EqualTo(350));
            Assert.That(catalog.energy.startingAmount, Is.EqualTo(5));
            Assert.That(catalog.energy.capacity, Is.EqualTo(5));
            Assert.That(catalog.energy.levelEntryCost, Is.EqualTo(1));
            Assert.That(catalog.energy.refillIntervalMinutes, Is.EqualTo(30));
            Assert.That(Booster(catalog.boosters, "bogatyr_mace").startingQuantity, Is.EqualTo(7));
            Assert.That(Booster(catalog.boosters, "firebird").startingQuantity, Is.EqualTo(5));
            Assert.That(Booster(catalog.boosters, "living_light").startingQuantity, Is.EqualTo(8));
            Assert.That(Booster(catalog.boosters, "sea_pearl").startingQuantity, Is.EqualTo(10));
            Assert.That(catalog.continueRule.enabled, Is.False);
            Assert.That(catalog.continueRule.priceDefined, Is.True);
            Assert.That(catalog.continueRule.price.id, Is.EqualTo("gems"));
            Assert.That(catalog.continueRule.price.amount, Is.EqualTo(20));
            Assert.That(catalog.continueRule.grantedMoves, Is.EqualTo(5));
            Assert.That(catalog.dailyReward.cadenceUtcDays, Is.EqualTo(1));
            Assert.That(Balance(catalog.dailyReward.grants, "gold"), Is.EqualTo(500));
            Assert.That(Balance(catalog.dailyReward.grants, "gems"), Is.EqualTo(20));
            Assert.That(catalog.dailyReward.energyGrant, Is.EqualTo(1));

            Assert.That(SumProgressionCurrency(catalog, "gold"), Is.EqualTo(1850));
            Assert.That(SumProgressionCurrency(catalog, "gems"), Is.EqualTo(40));
            Assert.That(SumProgressionBooster(catalog, "bogatyr_mace"), Is.EqualTo(2));
            Assert.That(SumProgressionBooster(catalog, "firebird"), Is.EqualTo(2));
            Assert.That(SumProgressionBooster(catalog, "living_light"), Is.EqualTo(2));
            Assert.That(SumProgressionBooster(catalog, "sea_pearl"), Is.EqualTo(2));
        }

        [Test]
        public void Validator_AcceptsCompleteProposalCatalog()
        {
            EconomyValidationReport report = EconomyCatalogValidator.Validate(CreateCatalog());

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(report.violations, Is.Empty);
        }

        [Test]
        public void Validator_MissingRequiredFieldsFailClosed()
        {
            var truncated = new EconomyCatalogData();

            EconomyValidationReport validation = EconomyCatalogValidator.Validate(truncated);
            EconomySimulationReport simulation = EconomySimulator.Run(truncated);

            Assert.That(validation.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(validation), Does.Contain(EconomyViolationCodes.UnsupportedSchema));
            Assert.That(Codes(validation), Does.Contain(EconomyViolationCodes.MissingId));
            Assert.That(Codes(validation), Does.Contain(EconomyViolationCodes.ProposalOnlyRequired));
            Assert.That(Codes(validation), Does.Contain(EconomyViolationCodes.AuthoritativeForbidden));
            Assert.That(simulation.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(simulation.trace, Is.EqualTo(new[] { "simulation:rejected-invalid-catalog" }));
        }

        [Test]
        public void Validator_RejectsDuplicateIdsAndMissingReferencesWithStableOrdering()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.currencies = new[]
            {
                new EconomyCurrencyData { id = "gold" },
                new EconomyCurrencyData { id = "gold" },
            };
            catalog.startingBalances[1].id = "unknown";
            catalog.continueRule.price.id = "missing";

            EconomyValidationReport first = EconomyCatalogValidator.Validate(catalog);
            EconomyValidationReport second = EconomyCatalogValidator.Validate(catalog);

            Assert.That(first.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(first), Does.Contain(EconomyViolationCodes.DuplicateId));
            Assert.That(Codes(first), Does.Contain(EconomyViolationCodes.MissingReference));
            Assert.That(EconomyDeterministicJson.Serialize(first), Is.EqualTo(
                EconomyDeterministicJson.Serialize(second)
            ));
            AssertViolationsAreOrdinalSorted(first.violations);
        }

        [Test]
        public void Validator_RejectsNegativeAndInvalidAmounts()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.startingBalances[0].amount = -1;
            catalog.energy.startingAmount = -1;
            catalog.continueRule.price.amount = 0;
            catalog.dailyReward.grants[0].amount = -5;

            EconomyValidationReport report = EconomyCatalogValidator.Validate(catalog);

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(report), Does.Contain(EconomyViolationCodes.NegativeValue));
            Assert.That(Codes(report), Does.Contain(EconomyViolationCodes.EnergyInvalid));
            Assert.That(Codes(report), Does.Contain(EconomyViolationCodes.InvalidValue));
        }

        [Test]
        public void Validator_RejectsInvalidActiveRulesAndDuplicateGrantVectors()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.continueRule.enabled = true;
            catalog.continueRule.grantedMoves = 0;
            catalog.continueRule.maxPerAttempt = 0;
            catalog.rewardDoubling.multiplier = 1;
            catalog.rewardDoubling.maxClaimsPerReward = 0;
            catalog.dailyReward.grants = new[] { Amount("gold", 10), Amount("gold", 20) };
            catalog.progression.levels[0].firstClearGrants = new[]
            {
                Amount("gems", 1),
                Amount("gems", 2),
            };

            EconomyValidationReport report = EconomyCatalogValidator.Validate(catalog);

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(report).Count(code => code == EconomyViolationCodes.DuplicateId), Is.EqualTo(2));
            Assert.That(Codes(report).Count(code => code == EconomyViolationCodes.InvalidValue), Is.GreaterThanOrEqualTo(4));
        }

        [Test]
        public void Validator_RejectsAggregateArithmeticOverflow()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.startingBalances[0].amount = long.MaxValue;
            catalog.dailyReward.grants[0].amount = 1;

            EconomyValidationReport report = EconomyCatalogValidator.Validate(catalog);

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(report), Does.Contain(EconomyViolationCodes.Overflow));
        }

        [Test]
        public void DeterministicSerialization_IsStableAcrossEquivalentOrdering()
        {
            EconomyCatalogData first = CreateCatalog();
            EconomyCatalogData second = CreateCatalog();
            Array.Reverse(second.currencies);
            Array.Reverse(second.startingBalances);
            Array.Reverse(second.boosters);

            Assert.That(EconomyDeterministicJson.Serialize(first), Is.EqualTo(
                EconomyDeterministicJson.Serialize(second)
            ));

            EconomySimulationReport firstReport = EconomySimulator.Run(first);
            EconomySimulationReport secondReport = EconomySimulator.Run(second);
            Assert.That(EconomyDeterministicJson.Serialize(firstReport), Is.EqualTo(
                EconomyDeterministicJson.Serialize(secondReport)
            ));
        }

        [Test]
        public void EnergyRefill_UsesExactBoundaryAndPreservesPartialInterval()
        {
            EconomyEnergyData energy = CreateCatalog().energy;

            EconomyEnergyResult before = EconomyEnergyMath.Advance(energy, 0, 0, 1799, 60);
            EconomyEnergyResult exact = EconomyEnergyMath.Advance(energy, 0, 0, 1800, 60);
            EconomyEnergyResult partial = EconomyEnergyMath.Advance(energy, 0, 0, 2700, 60);

            Assert.That(before.valid, Is.True);
            Assert.That(before.gained, Is.Zero);
            Assert.That(before.energy, Is.Zero);
            Assert.That(before.lastRefillTick, Is.Zero);
            Assert.That(before.minutesUntilNext, Is.EqualTo(1));

            Assert.That(exact.valid, Is.True);
            Assert.That(exact.gained, Is.EqualTo(1));
            Assert.That(exact.energy, Is.EqualTo(1));
            Assert.That(exact.lastRefillTick, Is.EqualTo(1800));
            Assert.That(exact.minutesUntilNext, Is.EqualTo(30));

            Assert.That(partial.gained, Is.EqualTo(1));
            Assert.That(partial.lastRefillTick, Is.EqualTo(1800));
            Assert.That(partial.minutesUntilNext, Is.EqualTo(15));
        }

        [Test]
        public void EnergyRefill_CapsAtCapacityAndRejectsClockRollback()
        {
            EconomyEnergyData energy = CreateCatalog().energy;

            EconomyEnergyResult capped = EconomyEnergyMath.Advance(energy, 1, 100, 90100, 60);
            EconomyEnergyResult rollback = EconomyEnergyMath.Advance(energy, 2, 1000, 999, 60);

            Assert.That(capped.valid, Is.True);
            Assert.That(capped.energy, Is.EqualTo(5));
            Assert.That(capped.gained, Is.EqualTo(4));
            Assert.That(capped.lastRefillTick, Is.EqualTo(90100));
            Assert.That(capped.minutesUntilNext, Is.Zero);

            Assert.That(rollback.valid, Is.True);
            Assert.That(rollback.clockRollback, Is.True);
            Assert.That(rollback.code, Is.EqualTo(EconomyViolationCodes.ClockRollback));
            Assert.That(rollback.energy, Is.EqualTo(2));
            Assert.That(rollback.lastRefillTick, Is.EqualTo(1000));
        }

        [Test]
        public void Ledger_DuplicateGrantIsIdempotentAndOrdinalSnapshotIsStable()
        {
            var ledger = new EconomyLedger(new[]
            {
                Amount("gold", 10),
                Amount("gems", 5),
            });

            EconomyTransactionResult first = ledger.TryGrant("mail:welcome", new[] { Amount("gold", 250) });
            EconomyTransactionResult duplicate = ledger.TryGrant("mail:welcome", new[] { Amount("gold", 250) });

            Assert.That(first.applied, Is.True);
            Assert.That(duplicate.applied, Is.False);
            Assert.That(duplicate.code, Is.EqualTo(EconomyViolationCodes.DuplicateGrant));
            Assert.That(ledger.GetBalance("gold"), Is.EqualTo(260));
            Assert.That(duplicate.snapshot.appliedKeys, Is.EqualTo(new[] { "mail:welcome" }));
            Assert.That(duplicate.snapshot.balances.Select(value => value.id), Is.EqualTo(new[] { "gems", "gold" }));
        }

        [Test]
        public void Ledger_InsufficientMultiCurrencySpendIsAtomicAndCanRetrySameKey()
        {
            var ledger = new EconomyLedger(new[]
            {
                Amount("gold", 10),
                Amount("gems", 5),
            });

            EconomyTransactionResult denied = ledger.TrySpend(
                "continue:1",
                new[] { Amount("gold", 7), Amount("gems", 6) }
            );

            Assert.That(denied.applied, Is.False);
            Assert.That(denied.code, Is.EqualTo(EconomyViolationCodes.InsufficientBalance));
            Assert.That(ledger.GetBalance("gold"), Is.EqualTo(10));
            Assert.That(ledger.GetBalance("gems"), Is.EqualTo(5));
            Assert.That(denied.snapshot.appliedKeys, Is.Empty);

            EconomyTransactionResult accepted = ledger.TrySpend(
                "continue:1",
                new[] { Amount("gold", 7), Amount("gems", 5) }
            );
            Assert.That(accepted.applied, Is.True);
            Assert.That(ledger.GetBalance("gold"), Is.EqualTo(3));
            Assert.That(ledger.GetBalance("gems"), Is.Zero);
        }

        [Test]
        public void Ledger_OverflowFailsClosedWithoutRecordingGrant()
        {
            var ledger = new EconomyLedger(new[] { Amount("gold", long.MaxValue) });

            EconomyTransactionResult result = ledger.TryGrant("overflow", new[] { Amount("gold", 1) });

            Assert.That(result.applied, Is.False);
            Assert.That(result.code, Is.EqualTo(EconomyViolationCodes.Overflow));
            Assert.That(ledger.GetBalance("gold"), Is.EqualTo(long.MaxValue));
            Assert.That(result.snapshot.appliedKeys, Is.Empty);
        }

        [Test]
        public void DailyCadence_GrantsOncePerEligibleUtcDayAndCapsEnergy()
        {
            EconomyCatalogData catalog = CreateCatalog();
            var ledger = new EconomyLedger(new[] { Amount("gold", 0), Amount("gems", 0) });

            EconomyDailyClaimResult first = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 100, 99, 4, 5, ledger
            );
            EconomyDailyClaimResult duplicate = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 100, first.claimDayIndex, first.energy, 5, ledger
            );
            EconomyDailyClaimResult nextDay = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 101, first.claimDayIndex, first.energy, 5, ledger
            );

            Assert.That(first.claimed, Is.True);
            Assert.That(first.energy, Is.EqualTo(5));
            Assert.That(duplicate.claimed, Is.False);
            Assert.That(duplicate.code, Is.EqualTo(EconomyViolationCodes.DailyAlreadyClaimed));
            Assert.That(nextDay.claimed, Is.True);
            Assert.That(ledger.GetBalance("gold"), Is.EqualTo(1000));
            Assert.That(ledger.GetBalance("gems"), Is.EqualTo(40));
            Assert.That(nextDay.energy, Is.EqualTo(5));
        }

        [Test]
        public void DailyCadence_RejectsInvalidDaySentinelsWithoutMutation()
        {
            EconomyCatalogData catalog = CreateCatalog();
            var ledger = new EconomyLedger(new[] { Amount("gold", 10), Amount("gems", 5) });
            string before = EconomyDeterministicJson.Serialize(ledger.Snapshot());

            EconomyDailyClaimResult negativeCurrent = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, -1, -1, 2, 5, ledger
            );
            EconomyDailyClaimResult invalidLast = EconomyRewardCadence.TryClaimDaily(
                catalog.dailyReward, 0, -2, 2, 5, ledger
            );

            Assert.That(negativeCurrent.claimed, Is.False);
            Assert.That(negativeCurrent.code, Is.EqualTo(EconomyViolationCodes.InvalidTransaction));
            Assert.That(invalidLast.claimed, Is.False);
            Assert.That(invalidLast.code, Is.EqualTo(EconomyViolationCodes.InvalidTransaction));
            Assert.That(EconomyDeterministicJson.Serialize(ledger.Snapshot()), Is.EqualTo(before));
        }

        [Test]
        public void PurchaseFreeChapter1_CompletesInOneHundredFiftyMinutesWithoutExternalDependency()
        {
            EconomySimulationRequest request = PurchaseFreeRequest();

            EconomySimulationReport report = EconomySimulator.Run(CreateCatalog(), request);

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
            Assert.That(Check(report, "purchase-free-progression").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "energy-refill-cadence").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "first-clear-reward-cadence").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "daily-reward-cadence").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "reward-doubling-cadence").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "retry-energy-cost").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Check(report, "booster-inventory-affordability").verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(Metric(report, "levels-completed"), Is.EqualTo(10));
            Assert.That(Metric(report, "paid-continues-applied"), Is.Zero);
            Assert.That(Metric(report, "store-dependencies-used"), Is.Zero);
            Assert.That(Metric(report, "ad-dependencies-used"), Is.Zero);
            Assert.That(Metric(report, "purchases-used"), Is.Zero);
            Assert.That(report.elapsedMinutes, Is.EqualTo(150));
            Assert.That(report.endingEnergy, Is.Zero);
            Assert.That(Balance(report.endingWallet.balances, "gold"), Is.EqualTo(14390));
            Assert.That(Balance(report.endingWallet.balances, "gems"), Is.EqualTo(390));
            Assert.That(Booster(report.endingBoosters, "bogatyr_mace"), Is.EqualTo(9));
            Assert.That(Booster(report.endingBoosters, "firebird"), Is.EqualTo(7));
            Assert.That(Booster(report.endingBoosters, "living_light"), Is.EqualTo(10));
            Assert.That(Booster(report.endingBoosters, "sea_pearl"), Is.EqualTo(12));
            Assert.That(request.allowStore, Is.False);
            Assert.That(request.allowAds, Is.False);
            Assert.That(request.allowPurchases, Is.False);
        }

        [Test]
        public void RequestedPaidContinue_PartialFundingFailsExplicitlyWithoutNegativeBalance()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.continueRule.enabled = true;
            catalog.startingBalances.Single(item => item.id == "gems").amount = 30;
            EconomySimulationRequest request = PurchaseFreeRequest();
            request.optionalPaidContinuesPerLevel = 1;

            EconomySimulationReport report = EconomySimulator.Run(catalog, request);

            Assert.That(report.validation.verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Check(report, "requested-continue-fulfillment").verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Check(report, "requested-continue-fulfillment").code, Is.EqualTo(
                EconomyViolationCodes.InsufficientBalance
            ));
            Assert.That(Metric(report, "paid-continues-applied"), Is.GreaterThan(0));
            Assert.That(Metric(report, "paid-continues-applied"), Is.LessThan(10));
            Assert.That(Balance(report.endingWallet.balances, "gems"), Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void MandatoryExternalDependency_InvalidatesCatalogAndSimulation()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.progression.levels[3].requiresPurchase = true;

            EconomyValidationReport validation = EconomyCatalogValidator.Validate(catalog);
            EconomySimulationReport report = EconomySimulator.Run(catalog, PurchaseFreeRequest());

            Assert.That(validation.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(validation), Does.Contain(EconomyViolationCodes.PurchaseRequired));
            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Check(report, "catalog-validation").code, Is.EqualTo(EconomyViolationCodes.InvalidCatalog));
            Assert.That(report.trace, Is.EqualTo(new[] { "simulation:rejected-invalid-catalog" }));
        }

        [Test]
        public void UnaffordableMandatoryBooster_FailsPurchaseFreeReachability()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.progression.levels[0].requiredBoosterId = "firebird";
            catalog.progression.levels[0].requiredBoosterQuantity = 6;

            EconomySimulationReport report = EconomySimulator.Run(catalog, PurchaseFreeRequest());

            Assert.That(report.validation.verdict, Is.EqualTo(EconomyVerdict.Pass));
            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Check(report, "purchase-free-progression").code, Is.EqualTo(
                EconomyViolationCodes.UnreachableProgression
            ));
            Assert.That(Metric(report, "levels-completed"), Is.Zero);
            Assert.That(report.endingEnergy, Is.EqualTo(4));
        }

        [Test]
        public void InvalidCatalog_CannotProducePassingSimulation()
        {
            EconomyCatalogData catalog = CreateCatalog();
            catalog.authoritative = true;

            EconomySimulationReport report = EconomySimulator.Run(catalog);

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Fail));
            Assert.That(Codes(report.validation), Does.Contain(EconomyViolationCodes.AuthoritativeForbidden));
            Assert.That(Check(report, "catalog-validation").verdict, Is.EqualTo(EconomyVerdict.Fail));
        }

        [Test]
        public void ProposalRuntimeComparison_RemainsIncompleteForDormantAndMissingValues()
        {
            EconomyComparisonEntryData[] entries =
            {
                Comparison("energy.refillIntervalMinutes", EconomyComparisonStatus.Match, "30", "30"),
                Comparison("continueRule.enabled", EconomyComparisonStatus.Dormant, "mock-only", "false"),
                Comparison("boosters.firebird.price", EconomyComparisonStatus.Missing, "undefined", "undefined"),
            };

            EconomyComparisonReport report = EconomyComparisonReportBuilder.Build(
                "v1.15-step01-proposal",
                entries
            );

            Assert.That(report.verdict, Is.EqualTo(EconomyVerdict.Incomplete));
            Assert.That(report.entries.Select(item => item.path), Is.EqualTo(new[]
            {
                "boosters.firebird.price",
                "continueRule.enabled",
                "energy.refillIntervalMinutes",
            }));
            Assert.That(EconomyDeterministicJson.Serialize(report), Does.Contain("\"Incomplete\""));
        }

        static EconomyCatalogData CreateCatalog()
        {
            return new EconomyCatalogData
            {
                schemaVersion = 1,
                catalogId = "v1.15-step01-proposal",
                proposalOnly = true,
                authoritative = false,
                currencies = new[]
                {
                    new EconomyCurrencyData { id = "gold" },
                    new EconomyCurrencyData { id = "gems" },
                },
                startingBalances = new[] { Amount("gold", 12540), Amount("gems", 350) },
                energy = new EconomyEnergyData
                {
                    capacity = 5,
                    startingAmount = 5,
                    levelEntryCost = 1,
                    retryCost = 0,
                    refillIntervalMinutes = 30,
                },
                boosters = new[]
                {
                    Booster("bogatyr_mace", 7),
                    Booster("firebird", 5),
                    Booster("living_light", 8),
                    Booster("sea_pearl", 10),
                },
                continueRule = new EconomyContinueData
                {
                    enabled = false,
                    priceDefined = true,
                    price = Amount("gems", 20),
                    grantedMoves = 5,
                    maxPerAttempt = 1,
                    freeExitAndRetry = true,
                },
                dailyReward = new EconomyDailyRewardData
                {
                    enabled = true,
                    cadenceUtcDays = 1,
                    grants = new[] { Amount("gold", 500), Amount("gems", 20) },
                    energyGrant = 1,
                },
                rewardDoubling = new EconomyRewardDoublingData
                {
                    enabled = true,
                    eligibleCurrencyId = "gold",
                    multiplier = 2,
                    maxClaimsPerReward = 1,
                    optional = true,
                },
                progression = new EconomyProgressionData
                {
                    chapterId = 1,
                    normalProgressRequiresPurchase = false,
                    levels = new[]
                    {
                        Level(1, 120, 0),
                        Level(2, 130, 0, BoosterAmount("bogatyr_mace", 1)),
                        Level(3, 140, 5),
                        Level(4, 150, 0, BoosterAmount("firebird", 1)),
                        Level(5, 160, 0, BoosterAmount("living_light", 1)),
                        Level(6, 190, 5),
                        Level(7, 170, 0, BoosterAmount("sea_pearl", 1)),
                        Level(8, 210, 5),
                        Level(9, 230, 5, BoosterAmount("bogatyr_mace", 1)),
                        Level(
                            10,
                            350,
                            20,
                            BoosterAmount("firebird", 1),
                            BoosterAmount("living_light", 1),
                            BoosterAmount("sea_pearl", 1)
                        ),
                    },
                },
            };
        }

        static EconomyProgressionLevelData Level(
            int number,
            long gold,
            long gems,
            params EconomyBoosterAmountData[] boosters
        )
        {
            return new EconomyProgressionLevelData
            {
                id = "chapter01.level" + number.ToString("00"),
                levelNumber = number,
                requiredBoosterId = string.Empty,
                requiredBoosterQuantity = 0,
                firstClearGrants = new[] { Amount("gold", gold), Amount("gems", gems) },
                firstClearBoosterGrants = boosters ?? new EconomyBoosterAmountData[0],
            };
        }

        static EconomyBoosterData Booster(string id, long quantity)
        {
            return new EconomyBoosterData
            {
                id = id,
                startingQuantity = quantity,
                purchaseEnabled = false,
                priceDefined = false,
                price = Amount(string.Empty, 0),
            };
        }

        static EconomyBoosterAmountData BoosterAmount(string id, long amount)
        {
            return new EconomyBoosterAmountData(id, amount);
        }

        static EconomyAmountData Amount(string id, long amount)
        {
            return new EconomyAmountData(id, amount);
        }

        static EconomySimulationRequest PurchaseFreeRequest()
        {
            return new EconomySimulationRequest
            {
                scenarioId = "purchase-free-chapter01",
                startTick = 0,
                ticksPerMinute = 1,
                allowStore = false,
                allowAds = false,
                allowPurchases = false,
                consumeRequiredBoosters = true,
                optionalPaidContinuesPerLevel = 0,
            };
        }

        static EconomyComparisonEntryData Comparison(
            string path,
            EconomyComparisonStatus status,
            string runtime,
            string proposal
        )
        {
            return new EconomyComparisonEntryData
            {
                path = path,
                status = status,
                runtimeValue = runtime,
                proposalValue = proposal,
                source = "test",
            };
        }

        static string[] Codes(EconomyValidationReport report)
        {
            return report.violations.Select(item => item.code).ToArray();
        }

        static long Balance(EconomyAmountData[] values, string id)
        {
            EconomyAmountData match = values.Single(item => item.id == id);
            return match.amount;
        }

        static EconomyBoosterData Booster(EconomyBoosterData[] values, string id)
        {
            return values.Single(item => item.id == id);
        }

        static long Booster(EconomyBoosterAmountData[] values, string id)
        {
            return values.Single(item => item.id == id).amount;
        }

        static long SumProgressionCurrency(EconomyCatalogData catalog, string id)
        {
            return catalog.progression.levels.Sum(level =>
                level.firstClearGrants.Where(item => item.id == id).Sum(item => item.amount)
            );
        }

        static long SumProgressionBooster(EconomyCatalogData catalog, string id)
        {
            return catalog.progression.levels.Sum(level =>
                level.firstClearBoosterGrants.Where(item => item.id == id).Sum(item => item.amount)
            );
        }

        static EconomyCheckData Check(EconomySimulationReport report, string id)
        {
            return report.checks.Single(item => item.id == id);
        }

        static long Metric(EconomySimulationReport report, string id)
        {
            return report.metrics.Single(item => item.id == id).value;
        }

        static void AssertViolationsAreOrdinalSorted(EconomyViolationData[] violations)
        {
            for (int i = 1; i < violations.Length; i++)
            {
                EconomyViolationData previous = violations[i - 1];
                EconomyViolationData current = violations[i];
                int path = string.Compare(previous.path, current.path, StringComparison.Ordinal);
                Assert.That(
                    path < 0 || (path == 0 && string.Compare(previous.code, current.code, StringComparison.Ordinal) <= 0),
                    Is.True,
                    "Violations must be ordered by path and then code."
                );
            }
        }
    }
}
