using System;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>
    /// Time-source-independent command watchdog. Runtime passes unscaled time;
    /// EditMode tests pass deterministic numeric values.
    /// </summary>
    public sealed class BoardCommandWatchdog
    {
        long nextToken;
        long activeToken;
        double deadline;

        public bool IsActive { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public double StartedAt { get; private set; }
        public double Deadline => deadline;
        public long ActiveToken => activeToken;

        public long Begin(
            string label,
            double now,
            double timeoutSeconds
        )
        {
            if (IsActive)
                throw new InvalidOperationException("A board command is already active.");
            if (timeoutSeconds <= 0d)
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));

            activeToken = ++nextToken;
            Label = label ?? string.Empty;
            StartedAt = now;
            deadline = now + timeoutSeconds;
            IsActive = true;
            return activeToken;
        }

        public bool IsTimedOut(double now)
        {
            return IsActive && now >= deadline;
        }

        public bool Complete(long token)
        {
            if (!IsActive || token != activeToken)
                return false;

            Reset();
            return true;
        }

        public void Cancel()
        {
            Reset();
        }

        public void ForceTimeout(double now)
        {
            if (!IsActive) return;
            deadline = now - 0.001d;
        }

        void Reset()
        {
            IsActive = false;
            activeToken = 0L;
            deadline = 0d;
            StartedAt = 0d;
            Label = string.Empty;
        }
    }
}
