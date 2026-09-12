using System;
using System.Collections.Generic;

namespace LegendsOfRus.EconomyCore
{
    public static class RuntimeEconomyResourceIds
    {
        public const string Gold = "gold";
        public const string Gems = "gems";
        public const string Energy = "energy";
        public const string BogatyrMace = "bogatyr_mace";
        public const string Firebird = "firebird";
        public const string LivingLight = "living_light";
        public const string SeaPearl = "sea_pearl";

        public static bool IsKnown(string resourceId)
        {
            switch (resourceId)
            {
                case Gold:
                case Gems:
                case Energy:
                case BogatyrMace:
                case Firebird:
                case LivingLight:
                case SeaPearl:
                    return true;
                default:
                    return false;
            }
        }
    }

    [Serializable]
    public sealed class RuntimeEconomySnapshot
    {
        public long gold;
        public long gems;
        public long energy;
        public long energyCapacity;
        public long bogatyrMace;
        public long firebird;
        public long livingLight;
        public long seaPearl;

        public RuntimeEconomySnapshot() { }

        public RuntimeEconomySnapshot(
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
            this.gold = gold;
            this.gems = gems;
            this.energy = energy;
            this.energyCapacity = energyCapacity;
            this.bogatyrMace = bogatyrMace;
            this.firebird = firebird;
            this.livingLight = livingLight;
            this.seaPearl = seaPearl;
        }

        public RuntimeEconomySnapshot DetachedCopy()
        {
            return new RuntimeEconomySnapshot(
                gold,
                gems,
                energy,
                energyCapacity,
                bogatyrMace,
                firebird,
                livingLight,
                seaPearl
            );
        }
    }

    [Serializable]
    public sealed class RuntimeEconomyTransactionRequest
    {
        public string transactionKey = string.Empty;
        public EconomyAmountData[] costs = new EconomyAmountData[0];
        public EconomyAmountData[] grants = new EconomyAmountData[0];

        public RuntimeEconomyTransactionRequest() { }

        public RuntimeEconomyTransactionRequest(
            string transactionKey,
            EconomyAmountData[] costs,
            EconomyAmountData[] grants
        )
        {
            this.transactionKey = transactionKey ?? string.Empty;
            this.costs = costs;
            this.grants = grants;
        }
    }

    [Serializable]
    public sealed class RuntimeEconomyTransactionResult
    {
        public bool applied;
        public string code = string.Empty;
        public string transactionKey = string.Empty;
        public RuntimeEconomySnapshot snapshot = new RuntimeEconomySnapshot();
    }

    /// <summary>
    /// Applies a runtime economy request to a private detached snapshot. Successful
    /// keys are process-local by design; durable idempotency remains the caller's
    /// responsibility through the existing save-domain claim flags.
    /// </summary>
    public sealed class RuntimeEconomyTransactionEngine
    {
        readonly object syncRoot = new object();
        readonly HashSet<string> successfulKeys =
            new HashSet<string>(StringComparer.Ordinal);

        RuntimeEconomySnapshot current;
        readonly string initialSnapshotError;

        public RuntimeEconomyTransactionEngine(RuntimeEconomySnapshot initialSnapshot)
        {
            current = initialSnapshot != null
                ? initialSnapshot.DetachedCopy()
                : new RuntimeEconomySnapshot();
            initialSnapshotError = ValidateSnapshot(initialSnapshot);
        }

        public int SuccessfulTransactionCount
        {
            get
            {
                lock (syncRoot)
                    return successfulKeys.Count;
            }
        }

        public bool HasApplied(string transactionKey)
        {
            if (string.IsNullOrWhiteSpace(transactionKey))
                return false;

            lock (syncRoot)
                return successfulKeys.Contains(transactionKey);
        }

        public RuntimeEconomySnapshot Snapshot()
        {
            lock (syncRoot)
                return current.DetachedCopy();
        }

        public RuntimeEconomyTransactionResult TryApply(
            RuntimeEconomyTransactionRequest request
        )
        {
            lock (syncRoot)
            {
                string key = request != null
                    ? request.transactionKey ?? string.Empty
                    : string.Empty;

                if (!string.IsNullOrEmpty(initialSnapshotError))
                    return Result(false, initialSnapshotError, key, current);

                if (request == null || !IsStableKey(key))
                {
                    return Result(
                        false,
                        EconomyViolationCodes.InvalidTransaction,
                        key,
                        current
                    );
                }

                if (successfulKeys.Contains(key))
                {
                    return Result(
                        false,
                        EconomyViolationCodes.DuplicateTransaction,
                        key,
                        current
                    );
                }

                string requestError = ValidateRequest(request);
                if (!string.IsNullOrEmpty(requestError))
                    return Result(false, requestError, key, current);

                RuntimeEconomySnapshot candidate = current.DetachedCopy();
                try
                {
                    for (int i = 0; i < request.costs.Length; i++)
                    {
                        EconomyAmountData cost = request.costs[i];
                        long balance = Read(candidate, cost.id);
                        if (balance < cost.amount)
                        {
                            return Result(
                                false,
                                EconomyViolationCodes.InsufficientBalance,
                                key,
                                current
                            );
                        }

                        Write(candidate, cost.id, checked(balance - cost.amount));
                    }

                    for (int i = 0; i < request.grants.Length; i++)
                    {
                        EconomyAmountData grant = request.grants[i];
                        long balance = Read(candidate, grant.id);
                        if (string.Equals(
                            grant.id,
                            RuntimeEconomyResourceIds.Energy,
                            StringComparison.Ordinal
                        ))
                        {
                            long headroom = candidate.energyCapacity - balance;
                            Write(
                                candidate,
                                grant.id,
                                grant.amount >= headroom
                                    ? candidate.energyCapacity
                                    : checked(balance + grant.amount)
                            );
                        }
                        else
                        {
                            Write(candidate, grant.id, checked(balance + grant.amount));
                        }
                    }
                }
                catch (OverflowException)
                {
                    return Result(
                        false,
                        EconomyViolationCodes.Overflow,
                        key,
                        current
                    );
                }

                current = candidate;
                successfulKeys.Add(key);
                return Result(true, string.Empty, key, current);
            }
        }

        static bool IsStableKey(string transactionKey)
        {
            return !string.IsNullOrWhiteSpace(transactionKey) &&
                string.Equals(
                    transactionKey,
                    transactionKey.Trim(),
                    StringComparison.Ordinal
                );
        }

        static string ValidateSnapshot(RuntimeEconomySnapshot snapshot)
        {
            if (snapshot == null)
                return EconomyViolationCodes.InvalidTransaction;

            if (
                snapshot.gold < 0 ||
                snapshot.gems < 0 ||
                snapshot.bogatyrMace < 0 ||
                snapshot.firebird < 0 ||
                snapshot.livingLight < 0 ||
                snapshot.seaPearl < 0
            )
            {
                return EconomyViolationCodes.InvalidValue;
            }

            if (
                snapshot.energyCapacity <= 0 ||
                snapshot.energy < 0 ||
                snapshot.energy > snapshot.energyCapacity
            )
            {
                return EconomyViolationCodes.EnergyInvalid;
            }

            return string.Empty;
        }

        static string ValidateRequest(RuntimeEconomyTransactionRequest request)
        {
            if (request.costs == null || request.grants == null)
                return EconomyViolationCodes.InvalidTransaction;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            string error = ValidateAmounts(request.costs, seen);
            return !string.IsNullOrEmpty(error)
                ? error
                : ValidateAmounts(request.grants, seen);
        }

        static string ValidateAmounts(
            EconomyAmountData[] amounts,
            HashSet<string> seen
        )
        {
            for (int i = 0; i < amounts.Length; i++)
            {
                EconomyAmountData amount = amounts[i];
                if (
                    amount == null ||
                    string.IsNullOrWhiteSpace(amount.id) ||
                    amount.amount < 0
                )
                {
                    return EconomyViolationCodes.InvalidTransaction;
                }

                if (!RuntimeEconomyResourceIds.IsKnown(amount.id))
                    return EconomyViolationCodes.UnknownBalance;
                if (!seen.Add(amount.id))
                    return EconomyViolationCodes.DuplicateTransaction;
            }

            return string.Empty;
        }

        static long Read(RuntimeEconomySnapshot snapshot, string resourceId)
        {
            switch (resourceId)
            {
                case RuntimeEconomyResourceIds.Gold:
                    return snapshot.gold;
                case RuntimeEconomyResourceIds.Gems:
                    return snapshot.gems;
                case RuntimeEconomyResourceIds.Energy:
                    return snapshot.energy;
                case RuntimeEconomyResourceIds.BogatyrMace:
                    return snapshot.bogatyrMace;
                case RuntimeEconomyResourceIds.Firebird:
                    return snapshot.firebird;
                case RuntimeEconomyResourceIds.LivingLight:
                    return snapshot.livingLight;
                case RuntimeEconomyResourceIds.SeaPearl:
                    return snapshot.seaPearl;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(resourceId),
                        resourceId,
                        "Unknown runtime economy resource."
                    );
            }
        }

        static void Write(
            RuntimeEconomySnapshot snapshot,
            string resourceId,
            long value
        )
        {
            switch (resourceId)
            {
                case RuntimeEconomyResourceIds.Gold:
                    snapshot.gold = value;
                    return;
                case RuntimeEconomyResourceIds.Gems:
                    snapshot.gems = value;
                    return;
                case RuntimeEconomyResourceIds.Energy:
                    snapshot.energy = value;
                    return;
                case RuntimeEconomyResourceIds.BogatyrMace:
                    snapshot.bogatyrMace = value;
                    return;
                case RuntimeEconomyResourceIds.Firebird:
                    snapshot.firebird = value;
                    return;
                case RuntimeEconomyResourceIds.LivingLight:
                    snapshot.livingLight = value;
                    return;
                case RuntimeEconomyResourceIds.SeaPearl:
                    snapshot.seaPearl = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(resourceId),
                        resourceId,
                        "Unknown runtime economy resource."
                    );
            }
        }

        static RuntimeEconomyTransactionResult Result(
            bool applied,
            string code,
            string key,
            RuntimeEconomySnapshot snapshot
        )
        {
            return new RuntimeEconomyTransactionResult
            {
                applied = applied,
                code = code ?? string.Empty,
                transactionKey = key ?? string.Empty,
                snapshot = snapshot != null
                    ? snapshot.DetachedCopy()
                    : new RuntimeEconomySnapshot()
            };
        }
    }
}
