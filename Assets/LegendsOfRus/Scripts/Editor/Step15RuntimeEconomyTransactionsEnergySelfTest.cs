#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.EconomyCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step15RuntimeEconomyTransactionsEnergySelfTest
    {
        static readonly long BaseTick =
            new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        [MenuItem("Legends of Rus/Tests/Run v1.15 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            EconomyCatalogLoadResult proposal = EconomyCatalogLoader.LoadProposal();
            RuntimeEconomyActivationResult approved = BuildApproved(proposal);

            Check(
                "shipped runtime activation remains blocked",
                () =>
                {
                    RuntimeEconomyActivationResult shipped =
                        RuntimeEconomyActivationGate.Load();
                    return
                        shipped.Status == RuntimeEconomyActivationStatus.VerificationIncomplete &&
                        !shipped.IsRuntimeActivated &&
                        shipped.UsesLegacyFallback;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "proposal identity and immutable source hash match",
                () =>
                    proposal.Succeeded &&
                    proposal.Catalog.catalogId == RuntimeEconomyActivationGate.CatalogId &&
                    proposal.Catalog.proposalOnly &&
                    !proposal.Catalog.authoritative &&
                    RuntimeEconomyActivationGate.ComputeNormalizedSourceSha256(
                        proposal.SourceJson
                    ) == RuntimeEconomyActivationGate.CatalogSourceSha256,
                ref passed,
                ref total,
                failures
            );
            Check(
                "detached all-approved evidence produces configured policy",
                () =>
                    approved.IsRuntimeActivated &&
                    approved.Policy.IsCatalogConfigured &&
                    approved.Policy.LevelEntryCost == 1 &&
                    approved.Policy.RetryCost == 0 &&
                    approved.Policy.RefillIntervalMinutes == 30 &&
                    approved.Policy.FirstClearRewardCount == 10 &&
                    !approved.Policy.CanRebaseStartingBalances,
                ref passed,
                ref total,
                failures
            );
            Check(
                "mixed multi-resource transaction is atomic",
                MixedTransactionIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "insufficient transaction leaves every resource unchanged",
                InsufficientTransactionIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "overflowing transaction fails without mutation",
                OverflowTransactionIsAtomic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "negative and unknown requests fail closed",
                InvalidRequestsFailClosed,
                ref passed,
                ref total,
                failures
            );
            Check(
                "successful transaction key cannot be replayed",
                DuplicateKeyFailsClosed,
                ref passed,
                ref total,
                failures
            );
            Check(
                "energy advances only at the exact configured boundary",
                () => ExactEnergyBoundary(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "energy clock rollback leaves the detached save unchanged",
                () => EnergyRollbackFailsClosed(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "level entry combines refill, cost, and attempt",
                () => LevelEntryIsOneCandidate(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "retry is free and preserves energy anchor",
                () => RetryIsFree(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "Level 2 first-clear reward grants gold and mace once",
                () => FirstClearIsIdempotent(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "daily reward grants wallet, capped energy, and marker once",
                () => DailyRewardIsIdempotent(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "last level reward doubles exactly once",
                () => DoubleRewardIsIdempotent(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "booster spend and compensation use distinct atomic keys",
                () => BoosterSpendAndRefund(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "detached coordination never mutates caller-owned save",
                () => SourceSaveRemainsDetached(approved),
                ref passed,
                ref total,
                failures
            );
            Check(
                "save version and PlayerPrefs key contract remain unchanged",
                SaveIdentityIsUnchanged,
                ref passed,
                ref total,
                failures
            );
            Check(
                "GameState has no live RuntimeEconomy registration",
                NoLiveGameStateRegistration,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.15 Step 2 runtime economy transaction and energy " +
                    $"self-tests passed: {passed}/{total}. Runtime activation remains blocked."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.15 Step 2 self-tests passed: {passed}/{total}\n" +
                        "Runtime activation remains blocked pending approval.",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.15 Step 2 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
            if (Application.isBatchMode)
                throw new InvalidOperationException("v1.15 Step 2 self-tests failed.");
        }

        static RuntimeEconomyActivationResult BuildApproved(
            EconomyCatalogLoadResult proposal
        )
        {
            if (proposal == null || !proposal.Succeeded)
                return null;
            return RuntimeEconomyActivationGate.Evaluate(
                proposal.Catalog,
                proposal.SourceJson,
                new RuntimeEconomyActivationManifestData
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
                    runtimeActivationApproved = true
                },
                RuntimeEconomyActivationGate.RequiredUnityVersion
            );
        }

        static bool MixedTransactionIsAtomic()
        {
            var engine = new RuntimeEconomyTransactionEngine(Snapshot());
            RuntimeEconomyTransactionResult result = engine.TryApply(
                Request(
                    "selftest:mixed",
                    new[]
                    {
                        Amount(RuntimeEconomyResourceIds.Gold, 40),
                        Amount(RuntimeEconomyResourceIds.Energy, 1)
                    },
                    new[]
                    {
                        Amount(RuntimeEconomyResourceIds.Gems, 20),
                        Amount(RuntimeEconomyResourceIds.BogatyrMace, 1)
                    }
                )
            );
            return
                result.applied &&
                result.snapshot.gold == 60 &&
                result.snapshot.gems == 70 &&
                result.snapshot.energy == 2 &&
                result.snapshot.bogatyrMace == 5;
        }

        static bool InsufficientTransactionIsAtomic()
        {
            RuntimeEconomySnapshot initial = Snapshot();
            var engine = new RuntimeEconomyTransactionEngine(initial);
            RuntimeEconomyTransactionResult result = engine.TryApply(
                Request(
                    "selftest:insufficient",
                    new[]
                    {
                        Amount(RuntimeEconomyResourceIds.Gold, 20),
                        Amount(RuntimeEconomyResourceIds.Gems, 51)
                    },
                    new[] { Amount(RuntimeEconomyResourceIds.Firebird, 3) }
                )
            );
            RuntimeEconomySnapshot after = engine.Snapshot();
            return
                !result.applied &&
                result.code == EconomyViolationCodes.InsufficientBalance &&
                after.gold == initial.gold &&
                after.gems == initial.gems &&
                after.firebird == initial.firebird;
        }

        static bool OverflowTransactionIsAtomic()
        {
            RuntimeEconomySnapshot initial = Snapshot();
            initial.gold = long.MaxValue;
            var engine = new RuntimeEconomyTransactionEngine(initial);
            RuntimeEconomyTransactionResult result = engine.TryApply(
                Request(
                    "selftest:overflow",
                    new EconomyAmountData[0],
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
                )
            );
            return
                !result.applied &&
                result.code == EconomyViolationCodes.Overflow &&
                engine.Snapshot().gold == long.MaxValue;
        }

        static bool InvalidRequestsFailClosed()
        {
            var negative = new RuntimeEconomyTransactionEngine(Snapshot());
            var unknown = new RuntimeEconomyTransactionEngine(Snapshot());
            RuntimeEconomyTransactionResult first = negative.TryApply(
                Request(
                    "selftest:negative",
                    new EconomyAmountData[0],
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, -1) }
                )
            );
            RuntimeEconomyTransactionResult second = unknown.TryApply(
                Request(
                    "selftest:unknown",
                    new EconomyAmountData[0],
                    new[] { Amount("coins", 1) }
                )
            );
            return
                !first.applied && first.code == EconomyViolationCodes.InvalidTransaction &&
                !second.applied && second.code == EconomyViolationCodes.UnknownBalance &&
                negative.Snapshot().gold == 100 &&
                unknown.Snapshot().gold == 100;
        }

        static bool DuplicateKeyFailsClosed()
        {
            var engine = new RuntimeEconomyTransactionEngine(Snapshot());
            RuntimeEconomyTransactionRequest request = Request(
                "selftest:duplicate",
                new EconomyAmountData[0],
                new[] { Amount(RuntimeEconomyResourceIds.Gold, 10) }
            );
            RuntimeEconomyTransactionResult first = engine.TryApply(request);
            RuntimeEconomyTransactionResult second = engine.TryApply(request);
            return
                first.applied &&
                !second.applied &&
                second.code == EconomyViolationCodes.DuplicateTransaction &&
                engine.Snapshot().gold == 110;
        }

        static bool ExactEnergyBoundary(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved, () => BaseTick);
            PlayerSaveData save = Save(energy: 1, lastTick: BaseTick);
            DetachedRuntimeEconomyResult before = coordinator.TryAdvanceEnergy(
                save,
                BaseTick + TimeSpan.FromMinutes(30).Ticks - 1
            );
            DetachedRuntimeEconomyResult exact = coordinator.TryAdvanceEnergy(
                save,
                BaseTick + TimeSpan.FromMinutes(30).Ticks
            );
            return
                before.Applied && !before.Changed && before.Candidate.energy == 1 &&
                exact.Applied && exact.Changed && exact.Candidate.energy == 2 &&
                exact.Candidate.lastEnergyUtcTicks ==
                    BaseTick + TimeSpan.FromMinutes(30).Ticks &&
                save.energy == 1 && save.lastEnergyUtcTicks == BaseTick;
        }

        static bool EnergyRollbackFailsClosed(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 5, lastTick: BaseTick);
            string before = JsonUtility.ToJson(save);
            DetachedRuntimeEconomyResult result = coordinator.TryAdvanceEnergy(
                save,
                BaseTick - 1
            );
            return
                !result.Applied &&
                result.Code == EconomyViolationCodes.ClockRollback &&
                JsonUtility.ToJson(save) == before &&
                JsonUtility.ToJson(result.Candidate) == before;
        }

        static bool LevelEntryIsOneCandidate(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 0, lastTick: BaseTick);
            LevelData level = ChapterLevelCatalog.Find(1, 1);
            DetachedRuntimeEconomyResult result = coordinator.TryBeginLevelAttempt(
                save,
                level,
                false,
                "selftest:entry:1",
                BaseTick + TimeSpan.FromMinutes(30).Ticks
            );
            LevelProgressData progress = FindProgress(result.Candidate, 1, 1);
            return
                result.Applied && result.Changed &&
                result.Candidate.energy == 0 &&
                result.Candidate.lastEnergyUtcTicks ==
                    BaseTick + TimeSpan.FromMinutes(30).Ticks &&
                result.AttemptNumber == 1 &&
                progress != null && progress.attempts == 1 &&
                save.energy == 0 && save.levelProgress.Count == 0;
        }

        static bool RetryIsFree(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 1, lastTick: BaseTick);
            LevelData level = ChapterLevelCatalog.Find(1, 1);
            DetachedRuntimeEconomyResult entry = coordinator.TryBeginLevelAttempt(
                save,
                level,
                false,
                "selftest:retry:entry",
                BaseTick
            );
            if (!entry.Applied || entry.Candidate.energy != 0) return false;
            DetachedRuntimeEconomyResult discardedCandidate =
                coordinator.TryBeginLevelAttempt(
                    save,
                    level,
                    true,
                    "selftest:retry:discarded-candidate",
                    BaseTick
                );
            PlayerSaveData invalidAnchor = JsonUtility.FromJson<PlayerSaveData>(
                JsonUtility.ToJson(entry.Candidate)
            );
            invalidAnchor.lastEnergyUtcTicks = 0L;
            DetachedRuntimeEconomyResult invalidClock =
                coordinator.TryBeginLevelAttempt(
                    invalidAnchor,
                    level,
                    true,
                    "selftest:retry:invalid-anchor",
                    BaseTick
                );
            DetachedRuntimeEconomyResult rollback = coordinator.TryBeginLevelAttempt(
                entry.Candidate,
                level,
                true,
                "selftest:retry:rollback",
                BaseTick - 1L
            );
            DetachedRuntimeEconomyResult result = coordinator.TryBeginLevelAttempt(
                entry.Candidate,
                level,
                true,
                "selftest:retry:2",
                BaseTick + TimeSpan.FromHours(3).Ticks
            );
            LevelProgressData progress = FindProgress(result.Candidate, 1, 1);
            return
                !discardedCandidate.Applied &&
                discardedCandidate.Code == "RUNTIME_ECON_RETRY_NOT_ELIGIBLE" &&
                !invalidClock.Applied &&
                invalidClock.Code == EconomyViolationCodes.EnergyInvalid &&
                !rollback.Applied &&
                rollback.Code == EconomyViolationCodes.ClockRollback &&
                result.Applied &&
                result.Candidate.energy == 0 &&
                result.Candidate.lastEnergyUtcTicks == BaseTick &&
                progress.attempts == 2 && progress.retries == 1;
        }

        static bool FirstClearIsIdempotent(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 3, lastTick: BaseTick);
            save.highestUnlockedLevel = 2;
            save.selectedLevel = 2;
            save.levelProgress.Add(new LevelProgressData
            {
                chapterId = 1,
                levelId = 2,
                attempts = 1,
                completions = 1
            });
            LevelData level = ChapterLevelCatalog.Find(1, 2);
            LevelRewardData originalReward = level.rewards;
            DetachedRuntimeEconomyResult first;
            try
            {
                level.rewards = new LevelRewardData
                {
                    gold = 999999,
                    gems = 999999,
                    boosterMace = 999999
                };
                first = coordinator.TryGrantFirstClear(
                    save,
                    level,
                    "selftest:first-clear:1:2"
                );
            }
            finally
            {
                level.rewards = originalReward;
            }
            var replayCoordinator = new DetachedRuntimeEconomyCoordinator(approved);
            DetachedRuntimeEconomyResult replay = replayCoordinator.TryGrantFirstClear(
                first.Candidate,
                level,
                "selftest:first-clear:1:2:relaunch"
            );
            LevelProgressData progress = FindProgress(first.Candidate, 1, 2);
            return
                first.Applied && first.Candidate.gold == save.gold + 130 &&
                first.Candidate.boosterMace == save.boosterMace + 1 &&
                progress != null && progress.rewardClaimed &&
                !replay.Applied && replay.Code == EconomyViolationCodes.DuplicateGrant;
        }

        static bool DailyRewardIsIdempotent(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 4, lastTick: BaseTick);
            DetachedRuntimeEconomyResult first = coordinator.TryClaimDaily(
                save,
                "selftest:daily:20260901",
                BaseTick
            );
            var replayCoordinator = new DetachedRuntimeEconomyCoordinator(approved);
            DetachedRuntimeEconomyResult replay = replayCoordinator.TryClaimDaily(
                first.Candidate,
                "selftest:daily:20260901:relaunch",
                BaseTick
            );
            return
                first.Applied &&
                first.Candidate.gold == save.gold + 500 &&
                first.Candidate.gems == save.gems + 20 &&
                first.Candidate.energy == 5 &&
                first.Candidate.lastDailyRewardUtc == "20260901" &&
                !replay.Applied &&
                replay.Code == EconomyViolationCodes.DailyAlreadyClaimed;
        }

        static bool DoubleRewardIsIdempotent(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 3, lastTick: BaseTick);
            save.lastRewardGold = 120;
            save.lastRewardEligibleForDouble = true;
            DetachedRuntimeEconomyResult first = coordinator.TryDoubleLastReward(
                save,
                "selftest:double:1:1"
            );
            var replayCoordinator = new DetachedRuntimeEconomyCoordinator(approved);
            DetachedRuntimeEconomyResult replay = replayCoordinator.TryDoubleLastReward(
                first.Candidate,
                "selftest:double:1:1:relaunch"
            );
            return
                first.Applied && first.Candidate.gold == save.gold + 120 &&
                first.Candidate.lastRewardDoubled &&
                !replay.Applied;
        }

        static bool BoosterSpendAndRefund(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 3, lastTick: BaseTick);
            DetachedRuntimeEconomyResult spend = coordinator.TrySpendBooster(
                save,
                BoosterType.Firebird,
                "selftest:booster:command:9"
            );
            DetachedRuntimeEconomyResult refund = coordinator.TryRefundBooster(
                spend.Candidate,
                BoosterType.Firebird,
                "selftest:booster:command:9:refund"
            );
            return
                spend.Applied && spend.Candidate.boosterFirebird == save.boosterFirebird - 1 &&
                refund.Applied && refund.Candidate.boosterFirebird == save.boosterFirebird;
        }

        static bool SourceSaveRemainsDetached(RuntimeEconomyActivationResult approved)
        {
            var coordinator = new DetachedRuntimeEconomyCoordinator(approved);
            PlayerSaveData save = Save(energy: 3, lastTick: BaseTick);
            string before = JsonUtility.ToJson(save);
            DetachedRuntimeEconomyResult result = coordinator.TryStage(
                save,
                Request(
                    "selftest:detached",
                    new EconomyAmountData[0],
                    new[] { Amount(RuntimeEconomyResourceIds.Gold, 1) }
                )
            );
            result.Candidate.gold = 0;
            return result.Applied && JsonUtility.ToJson(save) == before;
        }

        static bool SaveIdentityIsUnchanged()
        {
            var save = new PlayerSaveData();
            FieldInfo key = typeof(SaveService).GetField(
                "Key",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                save.saveVersion == 6 &&
                key != null &&
                (string)key.GetRawConstantValue() == "LegendsOfRus_Save_v1";
        }

        static bool NoLiveGameStateRegistration()
        {
            Type coordinatorType = typeof(DetachedRuntimeEconomyCoordinator);
            FieldInfo[] fields = typeof(GameState).GetFields(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );
            for (int i = 0; i < fields.Length; i++)
                if (fields[i].FieldType == coordinatorType) return false;

            PropertyInfo[] properties = typeof(GameState).GetProperties(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );
            for (int i = 0; i < properties.Length; i++)
                if (properties[i].PropertyType == coordinatorType) return false;
            return true;
        }

        static PlayerSaveData Save(int energy, long lastTick)
        {
            return new PlayerSaveData
            {
                saveVersion = 6,
                gold = 12540,
                gems = 350,
                energy = energy,
                energyMax = 5,
                lastEnergyUtcTicks = lastTick,
                selectedChapter = 1,
                selectedLevel = 1,
                highestUnlockedLevel = 1,
                levelProgress = new List<LevelProgressData>(),
                boosterMace = 7,
                boosterFirebird = 5,
                boosterLivingLight = 8,
                boosterSeaPearl = 10
            };
        }

        static RuntimeEconomySnapshot Snapshot()
        {
            return new RuntimeEconomySnapshot(100, 50, 3, 5, 4, 3, 2, 1);
        }

        static RuntimeEconomyTransactionRequest Request(
            string key,
            EconomyAmountData[] costs,
            EconomyAmountData[] grants
        )
        {
            return new RuntimeEconomyTransactionRequest(key, costs, grants);
        }

        static EconomyAmountData Amount(string id, long amount)
        {
            return new EconomyAmountData(id, amount);
        }

        static LevelProgressData FindProgress(
            PlayerSaveData save,
            int chapterId,
            int levelId
        )
        {
            if (save == null || save.levelProgress == null) return null;
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
            return null;
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
