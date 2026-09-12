using System;

namespace LegendsOfRus.OptimizationCore
{
    /// <summary>
    /// Pinned v1.17 Step 4 evidence budgets. These are development guardrails,
    /// not a claim that any representative Android device has passed them.
    /// Production thresholds stay reviewable until retained device evidence
    /// exists.
    /// </summary>
    public static class OptimizationPolicy
    {
        public const int SchemaVersion = 1;
        public const long Mebibyte = 1024L * 1024L;

        public const long AllocatedWarningBytes = 384L * Mebibyte;
        public const long AllocatedCriticalBytes = 512L * Mebibyte;
        public const long ReservedWarningBytes = 640L * Mebibyte;
        public const long ReservedCriticalBytes = 768L * Mebibyte;
        public const long ManagedWarningBytes = 96L * Mebibyte;
        public const long ManagedCriticalBytes = 128L * Mebibyte;
        public const long MaximumSteadyGrowthBytes = 32L * Mebibyte;

        public const double ScreenRouteBudgetMilliseconds = 250.0;
        public const double ResourceCleanupBudgetMilliseconds = 750.0;
        public const double BootstrapBudgetMilliseconds = 5000.0;

        public const int DefaultEvidenceCapacity = 64;
        public const int MaximumEvidenceCapacity = 128;
        public const int RetiredScreensBeforeCleanup = 3;
        public const double MinimumCleanupIntervalSeconds = 10.0;
        public const double MemorySampleIntervalSeconds = 5.0;

        public const int RuntimeTextureMaximumDimension = 2048;

        public static bool IsValid
        {
            get
            {
                return SchemaVersion == 1 &&
                    AllocatedWarningBytes > 0 &&
                    AllocatedCriticalBytes > AllocatedWarningBytes &&
                    ReservedWarningBytes >= AllocatedCriticalBytes &&
                    ReservedCriticalBytes > ReservedWarningBytes &&
                    ManagedWarningBytes > 0 &&
                    ManagedCriticalBytes > ManagedWarningBytes &&
                    MaximumSteadyGrowthBytes > 0 &&
                    ScreenRouteBudgetMilliseconds > 0.0 &&
                    ResourceCleanupBudgetMilliseconds > 0.0 &&
                    BootstrapBudgetMilliseconds > 0.0 &&
                    DefaultEvidenceCapacity > 0 &&
                    DefaultEvidenceCapacity <= MaximumEvidenceCapacity &&
                    RetiredScreensBeforeCleanup >= 2 &&
                    MinimumCleanupIntervalSeconds >= 1.0 &&
                    MemorySampleIntervalSeconds >= 1.0 &&
                    RuntimeTextureMaximumDimension == 2048;
            }
        }
    }

    public enum MemoryBudgetBand
    {
        Unknown = 0,
        WithinBudget = 1,
        Warning = 2,
        Critical = 3
    }

    public enum OptimizationLoadKind
    {
        Bootstrap = 0,
        ScreenRoute = 1,
        ResourceCleanup = 2
    }

    public enum OptimizationLoadVerdict
    {
        Incomplete = 0,
        Pass = 1,
        Fail = 2
    }

    public static class MemoryBudgetEvaluator
    {
        public static MemoryBudgetBand Evaluate(
            long allocatedBytes,
            long reservedBytes,
            long managedBytes
        )
        {
            if (allocatedBytes < 0 || reservedBytes < 0 || managedBytes < 0)
                return MemoryBudgetBand.Unknown;

            if (allocatedBytes >= OptimizationPolicy.AllocatedCriticalBytes ||
                reservedBytes >= OptimizationPolicy.ReservedCriticalBytes ||
                managedBytes >= OptimizationPolicy.ManagedCriticalBytes)
                return MemoryBudgetBand.Critical;

            if (allocatedBytes >= OptimizationPolicy.AllocatedWarningBytes ||
                reservedBytes >= OptimizationPolicy.ReservedWarningBytes ||
                managedBytes >= OptimizationPolicy.ManagedWarningBytes)
                return MemoryBudgetBand.Warning;

            return MemoryBudgetBand.WithinBudget;
        }

        public static bool IsSteadyGrowthWithinBudget(long growthBytes)
        {
            if (growthBytes < 0)
                return true;
            return growthBytes <= OptimizationPolicy.MaximumSteadyGrowthBytes;
        }
    }

    public static class LoadingBudgetEvaluator
    {
        public static double BudgetMilliseconds(OptimizationLoadKind kind)
        {
            switch (kind)
            {
                case OptimizationLoadKind.Bootstrap:
                    return OptimizationPolicy.BootstrapBudgetMilliseconds;
                case OptimizationLoadKind.ScreenRoute:
                    return OptimizationPolicy.ScreenRouteBudgetMilliseconds;
                case OptimizationLoadKind.ResourceCleanup:
                    return OptimizationPolicy.ResourceCleanupBudgetMilliseconds;
                default:
                    return 0.0;
            }
        }

        public static OptimizationLoadVerdict Evaluate(
            OptimizationLoadKind kind,
            double durationMilliseconds
        )
        {
            double budget = BudgetMilliseconds(kind);
            if (budget <= 0.0 || durationMilliseconds < 0.0 ||
                double.IsNaN(durationMilliseconds) ||
                double.IsInfinity(durationMilliseconds))
                return OptimizationLoadVerdict.Incomplete;

            return durationMilliseconds <= budget
                ? OptimizationLoadVerdict.Pass
                : OptimizationLoadVerdict.Fail;
        }
    }
}
