using System;
using System.Collections.Generic;

namespace LegendsOfRus.EconomyCore
{
    public sealed class EconomyLedger
    {
        readonly Dictionary<string, long> balances = new Dictionary<string, long>(StringComparer.Ordinal);
        readonly HashSet<string> appliedKeys = new HashSet<string>(StringComparer.Ordinal);

        public EconomyLedger(EconomyAmountData[] startingBalances)
        {
            if (startingBalances == null)
                throw new ArgumentNullException(nameof(startingBalances));

            for (int i = 0; i < startingBalances.Length; i++)
            {
                EconomyAmountData amount = startingBalances[i];
                if (amount == null || string.IsNullOrWhiteSpace(amount.id))
                    throw new ArgumentException("Every starting balance requires an id.", nameof(startingBalances));
                if (amount.amount < 0)
                    throw new ArgumentOutOfRangeException(nameof(startingBalances), "Starting balances cannot be negative.");
                if (balances.ContainsKey(amount.id))
                    throw new ArgumentException("Starting balance ids must be unique.", nameof(startingBalances));
                balances.Add(amount.id, amount.amount);
            }
        }

        public long GetBalance(string currencyId)
        {
            long balance;
            return !string.IsNullOrEmpty(currencyId) && balances.TryGetValue(currencyId, out balance)
                ? balance
                : 0;
        }

        public EconomyTransactionResult TryGrant(string grantKey, EconomyAmountData[] grants)
        {
            return Apply(grantKey, grants, false, true);
        }

        public EconomyTransactionResult TrySpend(string transactionKey, EconomyAmountData[] costs)
        {
            return Apply(transactionKey, costs, true, false);
        }

        public EconomyLedgerSnapshot Snapshot()
        {
            var amountList = new List<EconomyAmountData>(balances.Count);
            foreach (KeyValuePair<string, long> pair in balances)
                amountList.Add(new EconomyAmountData(pair.Key, pair.Value));
            EconomyAmountData[] amountArray = amountList.ToArray();
            Array.Sort(amountArray, CompareAmounts);

            string[] keys = new string[appliedKeys.Count];
            appliedKeys.CopyTo(keys);
            Array.Sort(keys, StringComparer.Ordinal);
            return new EconomyLedgerSnapshot { balances = amountArray, appliedKeys = keys };
        }

        EconomyTransactionResult Apply(
            string transactionKey,
            EconomyAmountData[] amounts,
            bool spend,
            bool duplicateIsGrant
        )
        {
            if (string.IsNullOrWhiteSpace(transactionKey) || amounts == null)
                return Result(false, EconomyViolationCodes.InvalidTransaction, transactionKey);
            if (appliedKeys.Contains(transactionKey))
            {
                return Result(
                    false,
                    duplicateIsGrant
                        ? EconomyViolationCodes.DuplicateGrant
                        : EconomyViolationCodes.DuplicateTransaction,
                    transactionKey
                );
            }

            var deltas = new Dictionary<string, long>(StringComparer.Ordinal);
            try
            {
                for (int i = 0; i < amounts.Length; i++)
                {
                    EconomyAmountData amount = amounts[i];
                    if (amount == null || string.IsNullOrWhiteSpace(amount.id) || amount.amount < 0)
                        return Result(false, EconomyViolationCodes.InvalidTransaction, transactionKey);
                    if (!balances.ContainsKey(amount.id))
                        return Result(false, EconomyViolationCodes.UnknownBalance, transactionKey);

                    long existing;
                    deltas.TryGetValue(amount.id, out existing);
                    deltas[amount.id] = checked(existing + amount.amount);
                }

                var replacements = new Dictionary<string, long>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, long> delta in deltas)
                {
                    long current = balances[delta.Key];
                    if (spend && current < delta.Value)
                        return Result(false, EconomyViolationCodes.InsufficientBalance, transactionKey);
                    replacements.Add(
                        delta.Key,
                        spend ? checked(current - delta.Value) : checked(current + delta.Value)
                    );
                }

                foreach (KeyValuePair<string, long> replacement in replacements)
                    balances[replacement.Key] = replacement.Value;
                appliedKeys.Add(transactionKey);
                return Result(true, string.Empty, transactionKey);
            }
            catch (OverflowException)
            {
                return Result(false, EconomyViolationCodes.Overflow, transactionKey);
            }
        }

        EconomyTransactionResult Result(bool applied, string code, string key)
        {
            return new EconomyTransactionResult
            {
                applied = applied,
                code = code ?? string.Empty,
                transactionKey = key ?? string.Empty,
                snapshot = Snapshot()
            };
        }

        static int CompareAmounts(EconomyAmountData left, EconomyAmountData right)
        {
            return string.Compare(left.id, right.id, StringComparison.Ordinal);
        }
    }
}
