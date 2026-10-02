using System;
using LegendsOfRus.EconomyCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class DetachedRuntimeEconomyCoordinatorPlayModeTests
    {
        static readonly long FixedNowTicks =
            new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc).Ticks;

        [Test]
        public void ShippedInactiveGate_RejectsByteIdenticalInputAndStagesNoKey()
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(
                RuntimeEconomyActivationGate.Load(),
                () => FixedNowTicks
            );
            PlayerSaveData source = CreateValidSave();
            string sourceBefore = SaveJson(source);
            const string key = "test:inactive:mixed";

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    key,
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 25) },
                    new[] { Amount(RuntimeEconomyResourceIds.Gems, 5) }
                )
            );

            Assert.That(coordinator.IsReady, Is.False);
            Assert.That(result.Applied, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.Code, Is.EqualTo("RUNTIME_ECON_APPROVAL_REQUIRED"));
            Assert.That(result.TransactionKey, Is.EqualTo(key));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(result.Candidate, Is.Not.SameAs(source));
            Assert.That(coordinator.HasStaged(key), Is.False);
            Assert.That(coordinator.SuccessfulStageCount, Is.Zero);
        }

        [Test]
        public void GenericMixedTransaction_AppliesAtomicallyWithoutMutatingInputs()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.gems = 10;
            source.boosterMace = 2;
            source.boosterFirebird = 1;
            string sourceBefore = SaveJson(source);
            var request = new RuntimeEconomyTransactionRequest(
                "test:mixed:success",
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gold, 25),
                    Amount(RuntimeEconomyResourceIds.BogatyrMace, 1),
                },
                new[]
                {
                    Amount(RuntimeEconomyResourceIds.Gems, 5),
                    Amount(RuntimeEconomyResourceIds.Firebird, 2),
                }
            );
            string requestBefore = JsonUtility.ToJson(request);

            DetachedRuntimeEconomyResult result = coordinator.TryStage(source, request);

            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.Candidate.gold, Is.EqualTo(75));
            Assert.That(result.Candidate.gems, Is.EqualTo(15));
            Assert.That(result.Candidate.boosterMace, Is.EqualTo(1));
            Assert.That(result.Candidate.boosterFirebird, Is.EqualTo(3));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(JsonUtility.ToJson(request), Is.EqualTo(requestBefore));
            Assert.That(coordinator.HasStaged(request.transactionKey), Is.True);
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(1));
        }

        [Test]
        public void GenericMixedTransaction_InsufficientLaterCostRollsBackEveryChange()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.gems = 10;
            source.boosterMace = 2;
            string sourceBefore = SaveJson(source);
            const string key = "test:mixed:insufficient";

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    key,
                    new[]
                    {
                        Amount(RuntimeEconomyResourceIds.Gold, 25),
                        Amount(RuntimeEconomyResourceIds.Gems, 11),
                    },
                    new[] { Amount(RuntimeEconomyResourceIds.BogatyrMace, 5) }
                )
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo(EconomyViolationCodes.InsufficientBalance));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
            Assert.That(coordinator.SuccessfulStageCount, Is.Zero);
        }

        [Test]
        public void GenericTransaction_IntRangeOverflowFailsAtomically()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = int.MaxValue;
            string sourceBefore = SaveJson(source);
            const string key = "test:gold:int-overflow";

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                new RuntimeEconomyTransactionRequest(
                    key,
                    Array.Empty<EconomyAmountData>(),
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
                )
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo("RUNTIME_ECON_SAVE_RANGE_UNSUPPORTED"));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void GenericTransaction_NonVersionSixSaveFailsClosed()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.saveVersion = 5;
            string sourceBefore = SaveJson(source);
            const string key = "test:save:version";

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                Grant(key, RuntimeEconomyResourceIds.Gold, 1)
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo("RUNTIME_ECON_SAVE_VERSION_UNSUPPORTED"));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void GenericTransaction_InvalidVersionSixEconomyFailsClosed()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = -1;
            string sourceBefore = SaveJson(source);
            const string key = "test:save:invalid-v6";

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                Grant(key, RuntimeEconomyResourceIds.Gems, 1)
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo("RUNTIME_ECON_SAVE_INVALID"));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void EnergyAdvance_AtExactThirtyMinutesAddsOneAndAdvancesAnchorExactly()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 2;
            source.lastEnergyUtcTicks = FixedNowTicks - TimeSpan.TicksPerMinute * 30L;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult result = coordinator.TryAdvanceEnergy(
                source,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.Candidate.energy, Is.EqualTo(3));
            Assert.That(result.Candidate.lastEnergyUtcTicks, Is.EqualTo(FixedNowTicks));
            Assert.That(result.MinutesUntilNextEnergy, Is.EqualTo(30L));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.SuccessfulStageCount, Is.Zero);
        }

        [Test]
        public void EnergyAdvance_BeforeIntervalIsAnExactNoOp()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 2;
            source.lastEnergyUtcTicks = FixedNowTicks - TimeSpan.TicksPerMinute * 29L;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult result = coordinator.TryAdvanceEnergy(
                source,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.MinutesUntilNextEnergy, Is.EqualTo(1L));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        [TestCase(2)]
        [TestCase(5)]
        public void EnergyAdvance_ClockRollbackIsRejectedWithoutMovingAnchor(
            int startingEnergy
        )
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = startingEnergy;
            source.lastEnergyUtcTicks = FixedNowTicks + TimeSpan.TicksPerMinute;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult result = coordinator.TryAdvanceEnergy(
                source,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.Code, Is.EqualTo(EconomyViolationCodes.ClockRollback));
            Assert.That(result.MinutesUntilNextEnergy, Is.EqualTo(30L));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        [Test]
        public void LevelEntry_DueRefillCostAndAttemptAreOneSessionIdempotentCandidate()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 0;
            source.lastEnergyUtcTicks = FixedNowTicks - TimeSpan.TicksPerMinute * 30L;
            source.selectedChapter = 1;
            source.selectedLevel = 1;
            source.highestUnlockedLevel = 1;
            string sourceBefore = SaveJson(source);
            LevelData level = RequireLevel(1);
            const string key = "test:attempt:level01:session-a";

            DetachedRuntimeEconomyResult first = coordinator.TryBeginLevelAttempt(
                source,
                level,
                false,
                key,
                FixedNowTicks
            );

            Assert.That(first.Applied, Is.True, first.Code);
            Assert.That(first.Candidate.energy, Is.Zero);
            Assert.That(first.Candidate.lastEnergyUtcTicks, Is.EqualTo(FixedNowTicks));
            Assert.That(first.AttemptNumber, Is.EqualTo(1));
            Assert.That(first.MinutesUntilNextEnergy, Is.EqualTo(30L));
            LevelProgressData progress = RequireProgress(first.Candidate, 1, 1);
            Assert.That(progress.attempts, Is.EqualTo(1));
            Assert.That(progress.retries, Is.Zero);
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));

            string firstCandidateBefore = SaveJson(first.Candidate);
            DetachedRuntimeEconomyResult duplicate = coordinator.TryBeginLevelAttempt(
                first.Candidate,
                level,
                false,
                key,
                FixedNowTicks
            );

            Assert.That(duplicate.Applied, Is.False);
            Assert.That(duplicate.Code, Is.EqualTo(EconomyViolationCodes.DuplicateTransaction));
            Assert.That(SaveJson(duplicate.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(SaveJson(first.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(coordinator.HasStaged(key), Is.True);
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(1));
        }

        [Test]
        public void LevelEntry_WhenStillInsufficientLeavesEnergyTickAndProgressUnchanged()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 0;
            source.lastEnergyUtcTicks = FixedNowTicks - TimeSpan.TicksPerMinute * 29L;
            source.selectedChapter = 1;
            source.highestUnlockedLevel = 1;
            string sourceBefore = SaveJson(source);
            const string key = "test:attempt:insufficient";

            DetachedRuntimeEconomyResult result = coordinator.TryBeginLevelAttempt(
                source,
                RequireLevel(1),
                false,
                key,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo(EconomyViolationCodes.InsufficientBalance));
            Assert.That(result.MinutesUntilNextEnergy, Is.EqualTo(1L));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(result.Candidate.energy, Is.Zero);
            Assert.That(
                result.Candidate.lastEnergyUtcTicks,
                Is.EqualTo(source.lastEnergyUtcTicks)
            );
            Assert.That(result.Candidate.levelProgress, Is.Empty);
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void ZeroCostRetry_AtZeroEnergyKeepsEnergyClockExactAndIncrementsRetry()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 1;
            source.lastEnergyUtcTicks = FixedNowTicks;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult entry = coordinator.TryBeginLevelAttempt(
                source,
                RequireLevel(1),
                false,
                "test:retry:level01:entry",
                FixedNowTicks
            );
            Assert.That(entry.Applied, Is.True, entry.Code);
            Assert.That(entry.Candidate.energy, Is.Zero);

            PlayerSaveData invalidAnchor = JsonUtility.FromJson<PlayerSaveData>(
                SaveJson(entry.Candidate)
            );
            invalidAnchor.lastEnergyUtcTicks = 0L;
            DetachedRuntimeEconomyResult invalidClock =
                coordinator.TryBeginLevelAttempt(
                    invalidAnchor,
                    RequireLevel(1),
                    true,
                    "test:retry:level01:invalid-anchor",
                    FixedNowTicks
                );
            Assert.That(invalidClock.Applied, Is.False);
            Assert.That(invalidClock.Code, Is.EqualTo(EconomyViolationCodes.EnergyInvalid));
            Assert.That(
                coordinator.HasStaged("test:retry:level01:invalid-anchor"),
                Is.False
            );

            DetachedRuntimeEconomyResult rollback = coordinator.TryBeginLevelAttempt(
                entry.Candidate,
                RequireLevel(1),
                true,
                "test:retry:level01:rollback",
                FixedNowTicks - 1L
            );
            Assert.That(rollback.Applied, Is.False);
            Assert.That(rollback.Code, Is.EqualTo(EconomyViolationCodes.ClockRollback));
            Assert.That(
                SaveJson(rollback.Candidate),
                Is.EqualTo(SaveJson(entry.Candidate))
            );
            Assert.That(
                coordinator.HasStaged("test:retry:level01:rollback"),
                Is.False
            );

            DetachedRuntimeEconomyResult result = coordinator.TryBeginLevelAttempt(
                entry.Candidate,
                RequireLevel(1),
                true,
                "test:retry:level01:2",
                FixedNowTicks + TimeSpan.TicksPerHour * 6L
            );

            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Candidate.energy, Is.Zero);
            Assert.That(
                result.Candidate.lastEnergyUtcTicks,
                Is.EqualTo(entry.Candidate.lastEnergyUtcTicks)
            );
            Assert.That(result.AttemptNumber, Is.EqualTo(2));
            LevelProgressData progress = RequireProgress(result.Candidate, 1, 1);
            Assert.That(progress.attempts, Is.EqualTo(2));
            Assert.That(progress.retries, Is.EqualTo(1));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        [Test]
        public void ZeroCostRetry_WithoutMatchingCoordinatorCandidateIsRejected()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.energy = 0;
            source.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 1,
                    attempts = 1,
                    failures = 1,
                }
            );
            string sourceBefore = SaveJson(source);
            const string key = "test:retry:spoofed";

            DetachedRuntimeEconomyResult result = coordinator.TryBeginLevelAttempt(
                source,
                RequireLevel(1),
                true,
                key,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo("RUNTIME_ECON_RETRY_NOT_ELIGIBLE"));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);

            DetachedRuntimeEconomyCoordinator discardCoordinator =
                CreateApprovedCoordinator();
            PlayerSaveData historical = CreateValidSave();
            historical.energy = 1;
            historical.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 1,
                    attempts = 1,
                    failures = 1,
                }
            );
            string historicalBefore = SaveJson(historical);
            DetachedRuntimeEconomyResult paidEntry =
                discardCoordinator.TryBeginLevelAttempt(
                    historical,
                    RequireLevel(1),
                    false,
                    "test:retry:discarded-entry",
                    FixedNowTicks
                );
            Assert.That(paidEntry.Applied, Is.True, paidEntry.Code);
            Assert.That(paidEntry.Candidate.energy, Is.Zero);
            Assert.That(RequireProgress(paidEntry.Candidate, 1, 1).attempts, Is.EqualTo(2));

            DetachedRuntimeEconomyResult discardedCandidateRetry =
                discardCoordinator.TryBeginLevelAttempt(
                    historical,
                    RequireLevel(1),
                    true,
                    "test:retry:discarded-candidate",
                    FixedNowTicks
                );
            Assert.That(discardedCandidateRetry.Applied, Is.False);
            Assert.That(
                discardedCandidateRetry.Code,
                Is.EqualTo("RUNTIME_ECON_RETRY_NOT_ELIGIBLE")
            );
            Assert.That(SaveJson(historical), Is.EqualTo(historicalBefore));
            Assert.That(
                discardCoordinator.HasStaged("test:retry:discarded-candidate"),
                Is.False
            );
        }

        [Test]
        public void CanonicalLevelTwoFirstClear_Grants130GoldAndOneMaceAndMarksOnce()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.boosterMace = 2;
            source.highestUnlockedLevel = 2;
            source.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 2,
                    attempts = 1,
                    completions = 1,
                }
            );
            string sourceBefore = SaveJson(source);
            LevelData level = RequireLevel(2);

            DetachedRuntimeEconomyResult first = coordinator.TryGrantFirstClear(
                source,
                level,
                "test:first-clear:level02"
            );

            Assert.That(first.Applied, Is.True, first.Code);
            Assert.That(first.Candidate.gold, Is.EqualTo(230));
            Assert.That(first.Candidate.gems, Is.EqualTo(source.gems));
            Assert.That(first.Candidate.boosterMace, Is.EqualTo(3));
            Assert.That(RequireProgress(first.Candidate, 1, 2).rewardClaimed, Is.True);
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));

            string firstCandidateBefore = SaveJson(first.Candidate);
            DetachedRuntimeEconomyResult duplicate = coordinator.TryGrantFirstClear(
                first.Candidate,
                level,
                "test:first-clear:level02:second-key"
            );

            Assert.That(duplicate.Applied, Is.False);
            Assert.That(duplicate.Code, Is.EqualTo(EconomyViolationCodes.DuplicateGrant));
            Assert.That(SaveJson(duplicate.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(SaveJson(first.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(1));
        }

        [TestCase(1, 1, 0, "RUNTIME_ECON_LEVEL_NOT_COMPLETED")]
        [TestCase(2, 1, 1, "RUNTIME_ECON_LEVEL_LOCKED")]
        public void FirstClear_UncompletedOrLockedLevelIsRejected(
            int levelId,
            int highestUnlockedLevel,
            int completions,
            string expectedCode
        )
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.highestUnlockedLevel = highestUnlockedLevel;
            source.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = levelId,
                    attempts = 1,
                    completions = completions,
                }
            );
            string sourceBefore = SaveJson(source);
            const string key = "test:first-clear:ineligible";

            DetachedRuntimeEconomyResult result = coordinator.TryGrantFirstClear(
                source,
                RequireLevel(levelId),
                key
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo(expectedCode));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void FirstClear_UsesActivatedPolicyWhenCanonicalLevelRewardMutates()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.gems = 10;
            source.boosterMace = 2;
            source.highestUnlockedLevel = 2;
            source.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 2,
                    attempts = 1,
                    completions = 1,
                }
            );
            LevelData canonical = RequireLevel(2);
            var mutatedRequest = new LevelData
            {
                id = canonical.id,
                chapterId = canonical.chapterId,
                levelId = canonical.levelId,
                rewards = new LevelRewardData
                {
                    gold = 999999,
                    gems = 999999,
                    boosterMace = 999999,
                    boosterFirebird = 999999,
                    boosterLivingLight = 999999,
                    boosterSeaPearl = 999999,
                },
            };

            LevelRewardData originalCanonicalReward = canonical.rewards;
            DetachedRuntimeEconomyResult result;
            try
            {
                canonical.rewards = mutatedRequest.rewards;
                result = coordinator.TryGrantFirstClear(
                    source,
                    mutatedRequest,
                    "test:first-clear:canonicalized-level02"
                );
            }
            finally
            {
                canonical.rewards = originalCanonicalReward;
            }

            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Candidate.gold, Is.EqualTo(230));
            Assert.That(result.Candidate.gems, Is.EqualTo(10));
            Assert.That(result.Candidate.boosterMace, Is.EqualTo(3));
            Assert.That(result.Candidate.boosterFirebird, Is.EqualTo(source.boosterFirebird));
            Assert.That(result.Candidate.boosterLivingLight, Is.EqualTo(source.boosterLivingLight));
            Assert.That(result.Candidate.boosterSeaPearl, Is.EqualTo(source.boosterSeaPearl));
            Assert.That(mutatedRequest.rewards.gold, Is.EqualTo(999999));
        }

        [Test]
        public void DailyClaim_Grants500Gold20GemsOneEnergyAndMarksUtcDayOnce()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.gems = 10;
            source.energy = 4;
            source.lastDailyRewardUtc = string.Empty;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult first = coordinator.TryClaimDaily(
                source,
                "test:daily:20260901",
                FixedNowTicks
            );

            Assert.That(first.Applied, Is.True, first.Code);
            Assert.That(first.Candidate.gold, Is.EqualTo(600));
            Assert.That(first.Candidate.gems, Is.EqualTo(30));
            Assert.That(first.Candidate.energy, Is.EqualTo(5));
            Assert.That(first.Candidate.lastDailyRewardUtc, Is.EqualTo("20260901"));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));

            string firstCandidateBefore = SaveJson(first.Candidate);
            DetachedRuntimeEconomyResult duplicate = coordinator.TryClaimDaily(
                first.Candidate,
                "test:daily:20260901:second-key",
                FixedNowTicks
            );

            Assert.That(duplicate.Applied, Is.False);
            Assert.That(duplicate.Code, Is.EqualTo(EconomyViolationCodes.DailyAlreadyClaimed));
            Assert.That(SaveJson(duplicate.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(SaveJson(first.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(1));
        }

        [TestCase("not-a-utc-day", EconomyViolationCodes.InvalidTransaction)]
        [TestCase("20260902", EconomyViolationCodes.ClockRollback)]
        public void DailyClaim_MalformedOrFutureMarkerIsRejected(
            string marker,
            string expectedCode
        )
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.lastDailyRewardUtc = marker;
            string sourceBefore = SaveJson(source);
            const string key = "test:daily:invalid-marker";

            DetachedRuntimeEconomyResult result = coordinator.TryClaimDaily(
                source,
                key,
                FixedNowTicks
            );

            Assert.That(result.Applied, Is.False);
            Assert.That(result.Code, Is.EqualTo(expectedCode));
            Assert.That(SaveJson(result.Candidate), Is.EqualTo(sourceBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(coordinator.HasStaged(key), Is.False);
        }

        [Test]
        public void RewardDouble_GrantsExactlyOneBonusAndMarksOnce()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.gold = 100;
            source.lastRewardGold = 130;
            source.lastRewardEligibleForDouble = true;
            source.lastRewardDoubled = false;
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult first = coordinator.TryDoubleLastReward(
                source,
                "test:double:level02"
            );

            Assert.That(first.Applied, Is.True, first.Code);
            Assert.That(first.Candidate.gold, Is.EqualTo(230));
            Assert.That(first.Candidate.lastRewardDoubled, Is.True);
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));

            string firstCandidateBefore = SaveJson(first.Candidate);
            DetachedRuntimeEconomyResult duplicate = coordinator.TryDoubleLastReward(
                first.Candidate,
                "test:double:level02:second-key"
            );

            Assert.That(duplicate.Applied, Is.False);
            Assert.That(duplicate.Code, Is.EqualTo(EconomyViolationCodes.InvalidTransaction));
            Assert.That(SaveJson(duplicate.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(SaveJson(first.Candidate), Is.EqualTo(firstCandidateBefore));
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(1));
        }

        [Test]
        public void BoosterSpendAndRefund_RequireDistinctKeysAndRoundTripInventory()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.boosterMace = 2;
            string sourceBefore = SaveJson(source);
            const string spendKey = "test:booster:mace:spend";
            const string refundKey = "test:booster:mace:refund";

            DetachedRuntimeEconomyResult spend = coordinator.TrySpendBooster(
                source,
                BoosterType.BogatyrMace,
                spendKey
            );
            Assert.That(spend.Applied, Is.True, spend.Code);
            Assert.That(spend.Candidate.boosterMace, Is.EqualTo(1));

            DetachedRuntimeEconomyResult refund = coordinator.TryRefundBooster(
                spend.Candidate,
                BoosterType.BogatyrMace,
                refundKey
            );
            Assert.That(refund.Applied, Is.True, refund.Code);
            Assert.That(refund.Candidate.boosterMace, Is.EqualTo(2));
            Assert.That(coordinator.HasStaged(spendKey), Is.True);
            Assert.That(coordinator.HasStaged(refundKey), Is.True);
            Assert.That(coordinator.SuccessfulStageCount, Is.EqualTo(2));

            string refundCandidateBefore = SaveJson(refund.Candidate);
            DetachedRuntimeEconomyResult duplicateSpend = coordinator.TrySpendBooster(
                refund.Candidate,
                BoosterType.BogatyrMace,
                spendKey
            );
            Assert.That(duplicateSpend.Applied, Is.False);
            Assert.That(
                duplicateSpend.Code,
                Is.EqualTo(EconomyViolationCodes.DuplicateTransaction)
            );
            Assert.That(SaveJson(duplicateSpend.Candidate), Is.EqualTo(refundCandidateBefore));
            Assert.That(SaveJson(refund.Candidate), Is.EqualTo(refundCandidateBefore));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        [Test]
        public void PublicResultCandidateMutation_CannotMutateSourceSave()
        {
            DetachedRuntimeEconomyCoordinator coordinator = CreateApprovedCoordinator();
            PlayerSaveData source = CreateValidSave();
            source.levelProgress.Add(
                new LevelProgressData
                {
                    chapterId = 1,
                    levelId = 1,
                    attempts = 1,
                    completions = 1,
                    rewardClaimed = true,
                }
            );
            source.completedTutorialSteps.Add("tutorial.swap.start");
            string sourceBefore = SaveJson(source);

            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                source,
                Grant("test:detached-result", RuntimeEconomyResourceIds.Gems, 1)
            );
            Assert.That(result.Applied, Is.True, result.Code);
            Assert.That(result.Candidate, Is.Not.SameAs(source));
            Assert.That(result.Candidate.levelProgress, Is.Not.SameAs(source.levelProgress));

            result.Candidate.gold = -123;
            result.Candidate.levelProgress[0].attempts = 99;
            result.Candidate.completedTutorialSteps.Clear();
            result.Snapshot.gold = -456L;

            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(source.gold, Is.EqualTo(12540));
            Assert.That(source.levelProgress[0].attempts, Is.EqualTo(1));
            Assert.That(source.completedTutorialSteps, Is.EquivalentTo(new[] { "tutorial.swap.start" }));
        }

        static DetachedRuntimeEconomyCoordinator CreateApprovedCoordinator()
        {
            EconomyCatalogLoadResult proposal = EconomyCatalogLoader.LoadProposal();
            Assert.That(proposal.Succeeded, Is.True, proposal.Error);
            Assert.That(proposal.Catalog, Is.Not.Null);
            Assert.That(proposal.SourceJson, Is.Not.Empty);

            var manifest = new RuntimeEconomyActivationManifestData
            {
                schemaVersion = RuntimeEconomyActivationGate.ManifestSchemaVersion,
                manifestId = RuntimeEconomyActivationGate.ManifestId,
                catalogSchemaVersion = proposal.Catalog.schemaVersion,
                catalogId = RuntimeEconomyActivationGate.CatalogId,
                catalogSourceSha256 = RuntimeEconomyActivationGate.CatalogSourceSha256,
                unityVersion = RuntimeEconomyActivationGate.RequiredUnityVersion,
                productApproval = true,
                compilationVerified = true,
                editorSelfTestsPassed = true,
                editModeTestsPassed = true,
                playModeTestsPassed = true,
                runtimeActivationApproved = true,
            };
            RuntimeEconomyActivationResult activation = RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                manifest,
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );
            Assert.That(activation.IsRuntimeActivated, Is.True, activation.Reason);
            return new DetachedRuntimeEconomyCoordinator(
                activation,
                () => FixedNowTicks
            );
        }

        static PlayerSaveData CreateValidSave()
        {
            return new PlayerSaveData
            {
                saveVersion = 6,
                gold = 12540,
                gems = 350,
                energy = 5,
                energyMax = 5,
                lastEnergyUtcTicks = FixedNowTicks,
                chapter = 1,
                level = 1,
                selectedChapter = 1,
                selectedLevel = 1,
                highestUnlockedLevel = 1,
                boosterMace = 7,
                boosterFirebird = 5,
                boosterLivingLight = 8,
                boosterSeaPearl = 10,
            };
        }

        static RuntimeEconomyTransactionRequest Grant(
            string key,
            string resourceId,
            long amount
        )
        {
            return new RuntimeEconomyTransactionRequest(
                key,
                Array.Empty<EconomyAmountData>(),
                new[] { Amount(resourceId, amount) }
            );
        }

        static EconomyAmountData Amount(string resourceId, long amount)
        {
            return new EconomyAmountData(resourceId, amount);
        }

        static LevelData RequireLevel(int levelId)
        {
            LevelData level = ChapterLevelCatalog.Find(1, levelId);
            Assert.That(level, Is.Not.Null, "Missing canonical Chapter 1 level " + levelId + ".");
            return level;
        }

        static LevelProgressData RequireProgress(
            PlayerSaveData save,
            int chapterId,
            int levelId
        )
        {
            Assert.That(save, Is.Not.Null);
            Assert.That(save.levelProgress, Is.Not.Null);
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

            Assert.Fail("Missing progress for " + chapterId + ":" + levelId + ".");
            return null;
        }

        static string SaveJson(PlayerSaveData save)
        {
            return save == null ? "<null>" : JsonUtility.ToJson(save);
        }
    }
}
