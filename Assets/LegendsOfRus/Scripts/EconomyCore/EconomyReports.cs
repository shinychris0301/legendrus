using System;

namespace LegendsOfRus.EconomyCore
{
    public enum EconomyVerdict
    {
        Pass,
        Fail,
        Incomplete
    }

    public enum EconomyViolationSeverity
    {
        Error,
        Warning,
        Information
    }

    public static class EconomyViolationCodes
    {
        public const string NullCatalog = "ECON_CATALOG_NULL";
        public const string UnsupportedSchema = "ECON_SCHEMA_UNSUPPORTED";
        public const string MissingId = "ECON_ID_MISSING";
        public const string DuplicateId = "ECON_ID_DUPLICATE";
        public const string MissingReference = "ECON_REFERENCE_MISSING";
        public const string ProposalOnlyRequired = "ECON_PROPOSAL_ONLY_REQUIRED";
        public const string AuthoritativeForbidden = "ECON_AUTHORITATIVE_FORBIDDEN";
        public const string NegativeValue = "ECON_VALUE_NEGATIVE";
        public const string InvalidValue = "ECON_VALUE_INVALID";
        public const string Overflow = "ECON_ARITHMETIC_OVERFLOW";
        public const string EnergyInvalid = "ECON_ENERGY_INVALID";
        public const string DuplicateGrant = "ECON_GRANT_DUPLICATE";
        public const string DuplicateTransaction = "ECON_TRANSACTION_DUPLICATE";
        public const string InvalidTransaction = "ECON_TRANSACTION_INVALID";
        public const string InsufficientBalance = "ECON_BALANCE_INSUFFICIENT";
        public const string UnknownBalance = "ECON_BALANCE_UNKNOWN";
        public const string ProgressionSequence = "ECON_PROGRESSION_SEQUENCE";
        public const string PurchaseRequired = "ECON_PROGRESSION_PURCHASE_REQUIRED";
        public const string StoreRequired = "ECON_PROGRESSION_STORE_REQUIRED";
        public const string AdRequired = "ECON_PROGRESSION_AD_REQUIRED";
        public const string RetryPathRequired = "ECON_FREE_RETRY_PATH_REQUIRED";
        public const string PriceUndefined = "ECON_PRICE_UNDEFINED";
        public const string ClockRollback = "ECON_CLOCK_ROLLBACK";
        public const string DailyAlreadyClaimed = "ECON_DAILY_ALREADY_CLAIMED";
        public const string InvalidCatalog = "ECON_SIMULATION_INVALID_CATALOG";
        public const string UnreachableProgression = "ECON_PROGRESSION_UNREACHABLE";
    }

    [Serializable]
    public sealed class EconomyViolationData
    {
        public string code = string.Empty;
        public EconomyViolationSeverity severity;
        public string path = string.Empty;
        public string message = string.Empty;

        public EconomyViolationData() { }

        public EconomyViolationData(
            string code,
            EconomyViolationSeverity severity,
            string path,
            string message
        )
        {
            this.code = code ?? string.Empty;
            this.severity = severity;
            this.path = path ?? string.Empty;
            this.message = message ?? string.Empty;
        }
    }

    [Serializable]
    public sealed class EconomyValidationReport
    {
        public EconomyVerdict verdict = EconomyVerdict.Fail;
        public EconomyViolationData[] violations = new EconomyViolationData[0];
    }

    [Serializable]
    public sealed class EconomyCheckData
    {
        public string id = string.Empty;
        public EconomyVerdict verdict = EconomyVerdict.Incomplete;
        public string code = string.Empty;
        public string message = string.Empty;
    }

    [Serializable]
    public sealed class EconomyMetricData
    {
        public string id = string.Empty;
        public long value;
        public string unit = string.Empty;
    }

    [Serializable]
    public sealed class EconomyLedgerSnapshot
    {
        public EconomyAmountData[] balances = new EconomyAmountData[0];
        public string[] appliedKeys = new string[0];
    }

    [Serializable]
    public sealed class EconomyTransactionResult
    {
        public bool applied;
        public string code = string.Empty;
        public string transactionKey = string.Empty;
        public EconomyLedgerSnapshot snapshot = new EconomyLedgerSnapshot();
    }

    [Serializable]
    public sealed class EconomyEnergyResult
    {
        public bool valid;
        public string code = string.Empty;
        public long energy;
        public long lastRefillTick;
        public long gained;
        public long minutesUntilNext;
        public bool clockRollback;
    }

    [Serializable]
    public sealed class EconomyDailyClaimResult
    {
        public bool claimed;
        public string code = string.Empty;
        public long claimDayIndex;
        public long energy;
        public EconomyLedgerSnapshot wallet = new EconomyLedgerSnapshot();
    }

    [Serializable]
    public sealed class EconomySimulationRequest
    {
        public string scenarioId = "purchase-free-chapter01";
        public long startTick;
        public long ticksPerMinute = 1;
        public bool allowStore;
        public bool allowAds;
        public bool allowPurchases;
        public bool consumeRequiredBoosters = true;
        public int optionalPaidContinuesPerLevel;
    }

    [Serializable]
    public sealed class EconomySimulationReport
    {
        public string catalogId = string.Empty;
        public string scenarioId = string.Empty;
        public EconomyVerdict verdict = EconomyVerdict.Fail;
        public EconomyValidationReport validation = new EconomyValidationReport();
        public EconomyCheckData[] checks = new EconomyCheckData[0];
        public EconomyMetricData[] metrics = new EconomyMetricData[0];
        public EconomyLedgerSnapshot endingWallet = new EconomyLedgerSnapshot();
        public EconomyBoosterAmountData[] endingBoosters = new EconomyBoosterAmountData[0];
        public long endingEnergy;
        public long elapsedMinutes;
        public string[] trace = new string[0];
    }

    public enum EconomyComparisonStatus
    {
        Match,
        Missing,
        Dormant,
        Conflict
    }

    [Serializable]
    public sealed class EconomyComparisonEntryData
    {
        public string path = string.Empty;
        public EconomyComparisonStatus status;
        public string runtimeValue = string.Empty;
        public string proposalValue = string.Empty;
        public string source = string.Empty;
    }

    [Serializable]
    public sealed class EconomyComparisonReport
    {
        public string catalogId = string.Empty;
        public EconomyVerdict verdict = EconomyVerdict.Incomplete;
        public EconomyComparisonEntryData[] entries = new EconomyComparisonEntryData[0];
    }
}
