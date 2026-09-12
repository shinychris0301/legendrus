using System;

namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Immutable local retention and retry limits. It owns no clock or timer.
    /// </summary>
    public sealed class ErrorReportPolicy
    {
        public const int DefaultCapacity = 64;
        public const int MaximumCapacity = 256;
        public const long DefaultDeduplicationWindowTicks =
            10L * TimeSpan.TicksPerMinute;
        public const long DefaultRetentionTicks =
            7L * TimeSpan.TicksPerDay;
        public const long MaximumRetentionTicks =
            30L * TimeSpan.TicksPerDay;
        public const int DefaultMaximumDeliveryAttempts = 5;
        public const int MaximumDeliveryAttemptsLimit = 10;
        public const long DefaultInitialRetryDelayTicks =
            30L * TimeSpan.TicksPerSecond;
        public const long DefaultMaximumRetryDelayTicks =
            TimeSpan.TicksPerHour;
        public const long MaximumRetryDelayTicksLimit = TimeSpan.TicksPerHour;

        static readonly ErrorReportPolicy defaultPolicy =
            new ErrorReportPolicy(
                DefaultCapacity,
                DefaultDeduplicationWindowTicks,
                DefaultRetentionTicks,
                DefaultMaximumDeliveryAttempts,
                DefaultInitialRetryDelayTicks,
                DefaultMaximumRetryDelayTicks
            );

        public static ErrorReportPolicy Default { get { return defaultPolicy; } }

        public int Capacity { get; private set; }
        public long DeduplicationWindowTicks { get; private set; }
        public long RetentionTicks { get; private set; }
        public int MaximumDeliveryAttempts { get; private set; }
        public long InitialRetryDelayTicks { get; private set; }
        public long MaximumRetryDelayTicks { get; private set; }
        public bool IsValid { get; private set; }

        public ErrorReportPolicy(
            int capacity,
            long deduplicationWindowTicks,
            long retentionTicks,
            int maximumDeliveryAttempts,
            long initialRetryDelayTicks,
            long maximumRetryDelayTicks
        )
        {
            Capacity = capacity;
            DeduplicationWindowTicks = deduplicationWindowTicks;
            RetentionTicks = retentionTicks;
            MaximumDeliveryAttempts = maximumDeliveryAttempts;
            InitialRetryDelayTicks = initialRetryDelayTicks;
            MaximumRetryDelayTicks = maximumRetryDelayTicks;
            IsValid =
                capacity >= 1 && capacity <= MaximumCapacity &&
                deduplicationWindowTicks >= 1L &&
                retentionTicks >= deduplicationWindowTicks &&
                retentionTicks <= MaximumRetentionTicks &&
                maximumDeliveryAttempts >= 1 &&
                maximumDeliveryAttempts <= MaximumDeliveryAttemptsLimit &&
                initialRetryDelayTicks >= 1L &&
                maximumRetryDelayTicks >= initialRetryDelayTicks &&
                maximumRetryDelayTicks <= MaximumRetryDelayTicksLimit &&
                maximumRetryDelayTicks <= retentionTicks;
        }

        public long RetryDelayTicks(int failedAttemptCount)
        {
            if (!IsValid || failedAttemptCount < 1)
                return 0L;

            long delay = InitialRetryDelayTicks;
            for (int attempt = 1; attempt < failedAttemptCount; attempt++)
            {
                if (delay >= MaximumRetryDelayTicks)
                    return MaximumRetryDelayTicks;
                if (delay > long.MaxValue / 2L)
                    return MaximumRetryDelayTicks;
                delay *= 2L;
            }
            return delay > MaximumRetryDelayTicks
                ? MaximumRetryDelayTicks
                : delay;
        }

        internal static long SaturatingAdd(long left, long right)
        {
            if (left < 0L || right < 0L || left > long.MaxValue - right)
                return long.MaxValue;
            return left + right;
        }
    }
}
