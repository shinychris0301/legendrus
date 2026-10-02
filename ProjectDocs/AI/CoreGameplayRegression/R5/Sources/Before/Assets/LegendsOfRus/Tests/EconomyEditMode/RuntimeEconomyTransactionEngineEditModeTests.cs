using LegendsOfRus.EconomyCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Economy
{
    public sealed class RuntimeEconomyTransactionEngineEditModeTests
    {
        [Test]
        public void TryApply_MixedMultiResourceRequest_CommitsEveryChangeAtomically()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            var request = Request(
                "level-entry:chapter-1:level-2:attempt-1",
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gold, 40),
                    Amount(RuntimeEconomyResourceIds.Energy, 1),
                    Amount(RuntimeEconomyResourceIds.Firebird, 2),
                },
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gems, 12),
                    Amount(RuntimeEconomyResourceIds.BogatyrMace, 3),
                    Amount(RuntimeEconomyResourceIds.LivingLight, 4),
                    Amount(RuntimeEconomyResourceIds.SeaPearl, 2),
                }
            );

            RuntimeEconomyTransactionResult result = engine.TryApply(request);

            Assert.That(result.applied, Is.True);
            Assert.That(result.code, Is.Empty);
            Assert.That(result.transactionKey, Is.EqualTo(request.transactionKey));
            AssertSnapshot(
                result.snapshot,
                gold: 60,
                gems: 62,
                energy: 2,
                energyCapacity: 5,
                bogatyrMace: 7,
                firebird: 1,
                livingLight: 6,
                seaPearl: 3
            );
            AssertSnapshotEqual(result.snapshot, engine.Snapshot());
            Assert.That(engine.SuccessfulTransactionCount, Is.EqualTo(1));
            Assert.That(engine.HasApplied(request.transactionKey), Is.True);
        }

        [Test]
        public void TryApply_ExactBalanceSpend_SucceedsAndReachesZero()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "spend:exact-balances",
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gold, 100),
                    Amount(RuntimeEconomyResourceIds.Energy, 3),
                    Amount(RuntimeEconomyResourceIds.SeaPearl, 1),
                },
                NoAmounts()
            ));

            Assert.That(result.applied, Is.True);
            Assert.That(result.snapshot.gold, Is.Zero);
            Assert.That(result.snapshot.energy, Is.Zero);
            Assert.That(result.snapshot.seaPearl, Is.Zero);
            Assert.That(engine.HasApplied("spend:exact-balances"), Is.True);
        }

        [Test]
        public void TryApply_EnergyGrantAboveHeadroom_CapsAtCapacity()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "energy:refill:test",
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Energy, long.MaxValue) }
            ));

            Assert.That(result.applied, Is.True);
            Assert.That(result.snapshot.energy, Is.EqualTo(5));
            Assert.That(result.snapshot.energyCapacity, Is.EqualTo(5));
            AssertSnapshotEqual(result.snapshot, engine.Snapshot());
        }

        [Test]
        public void TryApply_DuplicateSuccessfulKey_IsRejectedWithoutASecondMutation()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            const string key = "daily:2030-01-02";

            RuntimeEconomyTransactionResult first = engine.TryApply(Request(
                key,
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 10) }
            ));
            RuntimeEconomyTransactionResult duplicate = engine.TryApply(Request(
                key,
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 999) }
            ));

            Assert.That(first.applied, Is.True);
            Assert.That(duplicate.applied, Is.False);
            Assert.That(duplicate.code, Is.EqualTo(EconomyViolationCodes.DuplicateTransaction));
            Assert.That(duplicate.transactionKey, Is.EqualTo(key));
            Assert.That(duplicate.snapshot.gold, Is.EqualTo(110));
            Assert.That(engine.Snapshot().gold, Is.EqualTo(110));
            Assert.That(engine.SuccessfulTransactionCount, Is.EqualTo(1));
        }

        [Test]
        public void TryApply_FailedKey_RemainsRetryableUntilItSucceeds()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            const string key = "retryable:spend";

            RuntimeEconomyTransactionResult denied = engine.TryApply(Request(
                key,
                new[] { Amount(RuntimeEconomyResourceIds.Gems, 51) },
                NoAmounts()
            ));

            Assert.That(denied.applied, Is.False);
            Assert.That(denied.code, Is.EqualTo(EconomyViolationCodes.InsufficientBalance));
            Assert.That(engine.HasApplied(key), Is.False);
            Assert.That(engine.SuccessfulTransactionCount, Is.Zero);

            RuntimeEconomyTransactionResult retry = engine.TryApply(Request(
                key,
                new[] { Amount(RuntimeEconomyResourceIds.Gems, 50) },
                NoAmounts()
            ));

            Assert.That(retry.applied, Is.True);
            Assert.That(retry.snapshot.gems, Is.Zero);
            Assert.That(engine.HasApplied(key), Is.True);
            Assert.That(engine.SuccessfulTransactionCount, Is.EqualTo(1));
        }

        [Test]
        public void TryApply_InsufficientLaterCost_LeavesCostsAndGrantsUnchanged()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "atomic:insufficient-second-component",
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gold, 80),
                    Amount(RuntimeEconomyResourceIds.Gems, 51),
                },
                new[] { Amount(RuntimeEconomyResourceIds.Firebird, 20) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InsufficientBalance
            );
            Assert.That(engine.HasApplied(result.transactionKey), Is.False);
        }

        [Test]
        public void TryApply_OverflowAfterCost_LeavesEveryBalanceUnchanged()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            initial.gold = long.MaxValue;
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "atomic:overflow-after-cost",
                new[] { Amount(RuntimeEconomyResourceIds.Gems, 10) },
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.Overflow
            );
            Assert.That(engine.HasApplied(result.transactionKey), Is.False);
        }

        [Test]
        public void TryApply_NegativeAmountLine_IsRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid:negative-line",
                new[] { Amount(RuntimeEconomyResourceIds.Gold, -1) },
                NoAmounts()
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
        }

        [Test]
        public void TryApply_NullAmountLine_IsRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid:null-line",
                new EconomyAmountData[] { null },
                NoAmounts()
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
        }

        [Test]
        public void TryApply_UnknownResourceLine_IsRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid:unknown-resource",
                NoAmounts(),
                new[] { Amount("coins", 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.UnknownBalance
            );
        }

        [Test]
        public void TryApply_DuplicateLinesWithinOneVector_AreRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid:duplicate-costs",
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gold, 1),
                    Amount(RuntimeEconomyResourceIds.Gold, 2),
                },
                NoAmounts()
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.DuplicateTransaction
            );
        }

        [Test]
        public void TryApply_ResourceRepeatedAcrossCostAndGrant_IsRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid:cost-grant-overlap",
                new[] { Amount(RuntimeEconomyResourceIds.Gems, 1) },
                new[] { Amount(RuntimeEconomyResourceIds.Gems, 2) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.DuplicateTransaction
            );
        }

        [Test]
        public void TryApply_NullAmountVectors_AreRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult nullCosts = engine.TryApply(Request(
                "invalid:null-costs",
                null,
                NoAmounts()
            ));
            RuntimeEconomyTransactionResult nullGrants = engine.TryApply(Request(
                "invalid:null-grants",
                NoAmounts(),
                null
            ));

            AssertFailureDidNotMutate(
                engine,
                nullCosts,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
            AssertFailureDidNotMutate(
                engine,
                nullGrants,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(" padded")]
        [TestCase("padded ")]
        public void TryApply_BlankOrUntrimmedKey_IsRejectedWithoutMutation(string key)
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                key,
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
            Assert.That(engine.HasApplied(key), Is.False);
        }

        [Test]
        public void TryApply_NullRequest_IsRejectedWithoutMutation()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(null);

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InvalidTransaction
            );
            Assert.That(result.transactionKey, Is.Empty);
        }

        [Test]
        public void Constructor_NullInitialSnapshot_FailsClosed()
        {
            var engine = new RuntimeEconomyTransactionEngine(null);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid-initial:null",
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            Assert.That(result.applied, Is.False);
            Assert.That(result.code, Is.EqualTo(EconomyViolationCodes.InvalidTransaction));
            AssertSnapshot(
                engine.Snapshot(),
                gold: 0,
                gems: 0,
                energy: 0,
                energyCapacity: 0,
                bogatyrMace: 0,
                firebird: 0,
                livingLight: 0,
                seaPearl: 0
            );
            Assert.That(engine.SuccessfulTransactionCount, Is.Zero);
        }

        [Test]
        public void Constructor_NegativeInitialBalance_FailsClosedWithoutRepairingIt()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            initial.gems = -1;
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid-initial:negative",
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.InvalidValue
            );
        }

        [Test]
        public void Constructor_InvalidInitialEnergy_FailsClosedWithoutRepairingIt()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            initial.energy = 6;
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid-initial:energy",
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.EnergyInvalid
            );
        }

        [Test]
        public void Constructor_ZeroEnergyCapacity_FailsClosed()
        {
            RuntimeEconomySnapshot initial = CreateSnapshot();
            initial.energy = 0;
            initial.energyCapacity = 0;
            var engine = new RuntimeEconomyTransactionEngine(initial);

            RuntimeEconomyTransactionResult result = engine.TryApply(Request(
                "invalid-initial:zero-capacity",
                NoAmounts(),
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
            ));

            AssertFailureDidNotMutate(
                engine,
                result,
                initial,
                EconomyViolationCodes.EnergyInvalid
            );
        }

        [Test]
        public void ConstructorAndSnapshot_DetachMutableInputAndReturnedState()
        {
            RuntimeEconomySnapshot input = CreateSnapshot();
            var engine = new RuntimeEconomyTransactionEngine(input);

            input.gold = 0;
            input.energy = 0;
            RuntimeEconomySnapshot firstRead = engine.Snapshot();
            firstRead.gems = 0;
            firstRead.energyCapacity = 999;

            AssertSnapshot(
                engine.Snapshot(),
                gold: 100,
                gems: 50,
                energy: 3,
                energyCapacity: 5,
                bogatyrMace: 4,
                firebird: 3,
                livingLight: 2,
                seaPearl: 1
            );
        }

        [Test]
        public void TryApply_DetachesRequestAndResultFromEngineState()
        {
            var engine = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            EconomyAmountData grant = Amount(RuntimeEconomyResourceIds.Gold, 5);
            var grants = new[] { grant };
            var request = Request("detached:request-result", NoAmounts(), grants);

            RuntimeEconomyTransactionResult result = engine.TryApply(request);
            request.transactionKey = "changed";
            request.grants[0].id = RuntimeEconomyResourceIds.Gems;
            request.grants[0].amount = 999;
            request.grants = NoAmounts();
            result.transactionKey = "changed-again";
            result.snapshot.gold = 0;
            result.snapshot.gems = 0;

            RuntimeEconomySnapshot current = engine.Snapshot();
            Assert.That(current.gold, Is.EqualTo(105));
            Assert.That(current.gems, Is.EqualTo(50));
            Assert.That(engine.HasApplied("detached:request-result"), Is.True);
            Assert.That(engine.HasApplied("changed"), Is.False);
        }

        [Test]
        public void EquivalentEnginesAndRequestSequences_ProduceDeterministicResults()
        {
            var first = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            var second = new RuntimeEconomyTransactionEngine(CreateSnapshot());
            RuntimeEconomyTransactionRequest[] sequence =
            {
                Request(
                    "deterministic:1",
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 25) },
                    new[] { Amount(RuntimeEconomyResourceIds.Gems, 7) }
                ),
                Request(
                    "deterministic:2",
                    NoAmounts(),
                    new[] { Amount(RuntimeEconomyResourceIds.Energy, 100) }
                ),
                Request(
                    "deterministic:denied",
                    new[] { Amount(RuntimeEconomyResourceIds.SeaPearl, 2) },
                    NoAmounts()
                ),
                Request(
                    "deterministic:1",
                    NoAmounts(),
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 999) }
                ),
            };

            for (int i = 0; i < sequence.Length; i++)
            {
                RuntimeEconomyTransactionResult left = first.TryApply(sequence[i]);
                RuntimeEconomyTransactionResult right = second.TryApply(Clone(sequence[i]));

                Assert.That(right.applied, Is.EqualTo(left.applied), "applied at index " + i);
                Assert.That(right.code, Is.EqualTo(left.code), "code at index " + i);
                Assert.That(
                    right.transactionKey,
                    Is.EqualTo(left.transactionKey),
                    "key at index " + i
                );
                AssertSnapshotEqual(left.snapshot, right.snapshot);
            }

            AssertSnapshotEqual(first.Snapshot(), second.Snapshot());
            Assert.That(
                second.SuccessfulTransactionCount,
                Is.EqualTo(first.SuccessfulTransactionCount)
            );
        }

        static RuntimeEconomySnapshot CreateSnapshot()
        {
            return new RuntimeEconomySnapshot(
                gold: 100,
                gems: 50,
                energy: 3,
                energyCapacity: 5,
                bogatyrMace: 4,
                firebird: 3,
                livingLight: 2,
                seaPearl: 1
            );
        }

        static RuntimeEconomyTransactionRequest Request(
            string key,
            EconomyAmountData[] costs,
            EconomyAmountData[] grants
        )
        {
            return new RuntimeEconomyTransactionRequest(key, costs, grants);
        }

        static RuntimeEconomyTransactionRequest Clone(
            RuntimeEconomyTransactionRequest request
        )
        {
            return Request(
                request.transactionKey,
                Clone(request.costs),
                Clone(request.grants)
            );
        }

        static EconomyAmountData[] Clone(EconomyAmountData[] amounts)
        {
            var copy = new EconomyAmountData[amounts.Length];
            for (int i = 0; i < amounts.Length; i++)
                copy[i] = Amount(amounts[i].id, amounts[i].amount);
            return copy;
        }

        static EconomyAmountData Amount(string id, long amount)
        {
            return new EconomyAmountData(id, amount);
        }

        static EconomyAmountData[] NoAmounts()
        {
            return new EconomyAmountData[0];
        }

        static void AssertFailureDidNotMutate(
            RuntimeEconomyTransactionEngine engine,
            RuntimeEconomyTransactionResult result,
            RuntimeEconomySnapshot expected,
            string expectedCode
        )
        {
            Assert.That(result.applied, Is.False);
            Assert.That(result.code, Is.EqualTo(expectedCode));
            AssertSnapshotEqual(expected, result.snapshot);
            AssertSnapshotEqual(expected, engine.Snapshot());
            Assert.That(engine.SuccessfulTransactionCount, Is.Zero);
        }

        static void AssertSnapshotEqual(
            RuntimeEconomySnapshot expected,
            RuntimeEconomySnapshot actual
        )
        {
            AssertSnapshot(
                actual,
                expected.gold,
                expected.gems,
                expected.energy,
                expected.energyCapacity,
                expected.bogatyrMace,
                expected.firebird,
                expected.livingLight,
                expected.seaPearl
            );
        }

        static void AssertSnapshot(
            RuntimeEconomySnapshot actual,
            long gold,
            long gems,
            long energy,
            long energyCapacity,
            long bogatyrMace,
            long firebird,
            long livingLight,
            long seaPearl
        )
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.gold, Is.EqualTo(gold));
            Assert.That(actual.gems, Is.EqualTo(gems));
            Assert.That(actual.energy, Is.EqualTo(energy));
            Assert.That(actual.energyCapacity, Is.EqualTo(energyCapacity));
            Assert.That(actual.bogatyrMace, Is.EqualTo(bogatyrMace));
            Assert.That(actual.firebird, Is.EqualTo(firebird));
            Assert.That(actual.livingLight, Is.EqualTo(livingLight));
            Assert.That(actual.seaPearl, Is.EqualTo(seaPearl));
        }
    }
}
