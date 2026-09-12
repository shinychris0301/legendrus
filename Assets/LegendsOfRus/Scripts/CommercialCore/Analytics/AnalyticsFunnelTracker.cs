using System;
using System.Collections.Generic;
using System.Globalization;

namespace LegendsOfRus.CommercialCore.Analytics
{
    public enum AnalyticsLevelTerminalOutcome
    {
        None = 0,
        Complete,
        Fail
    }

    /// <summary>
    /// Canonical attempt-key helper. It carries no player identifier and uses
    /// only a stable level id plus a positive, caller-owned attempt number.
    /// </summary>
    public static class AnalyticsAttemptKey
    {
        public static bool TryCreate(
            string levelId,
            long attemptNumber,
            out string attemptKey
        )
        {
            attemptKey = string.Empty;
            if (
                !AnalyticsEventValidator.IsCanonicalIdentifier(levelId, 64) ||
                attemptNumber < 1L
            )
            {
                return false;
            }

            string candidate =
                levelId + ".attempt." +
                attemptNumber.ToString(CultureInfo.InvariantCulture);
            if (
                !AnalyticsEventValidator.IsCanonicalIdentifier(
                    candidate,
                    AnalyticsFunnelTracker.MaximumAttemptKeyLength
                )
            )
            {
                return false;
            }

            attemptKey = candidate;
            return true;
        }
    }

    /// <summary>
    /// Process-local funnel state guard. It emits no analytics events; callers
    /// may emit a typed level event only after this tracker accepts the matching
    /// transition. Every rejected transition leaves all tracker state unchanged.
    /// </summary>
    public sealed class AnalyticsFunnelTracker
    {
        public const int DefaultAttemptCapacity = 1024;
        public const int MaximumAttemptCapacity = 4096;
        public const int MaximumAttemptKeyLength = 96;

        readonly object syncRoot = new object();
        readonly HashSet<string> begunAttemptKeys =
            new HashSet<string>(StringComparer.Ordinal);
        readonly int attemptCapacity;
        readonly bool configurationValid;

        bool hasActiveAttempt;
        string activeAttemptKey = string.Empty;
        bool terminalAccepted;
        AnalyticsLevelTerminalOutcome terminalOutcome =
            AnalyticsLevelTerminalOutcome.None;
        int acceptedTerminalCount;

        public AnalyticsFunnelTracker(
            int attemptCapacity = DefaultAttemptCapacity
        )
        {
            this.attemptCapacity = attemptCapacity;
            configurationValid =
                attemptCapacity >= 1 &&
                attemptCapacity <= MaximumAttemptCapacity;
        }

        public int AttemptCapacity => attemptCapacity;

        public int TrackedAttemptCount
        {
            get
            {
                lock (syncRoot)
                    return begunAttemptKeys.Count;
            }
        }

        public int AcceptedTerminalCount
        {
            get
            {
                lock (syncRoot)
                    return acceptedTerminalCount;
            }
        }

        public bool HasActiveAttempt
        {
            get
            {
                lock (syncRoot)
                    return hasActiveAttempt && !terminalAccepted;
            }
        }

        public string ActiveAttemptKey
        {
            get
            {
                lock (syncRoot)
                    return activeAttemptKey;
            }
        }

        public bool HasTerminalOutcome
        {
            get
            {
                lock (syncRoot)
                    return terminalAccepted;
            }
        }

        public AnalyticsLevelTerminalOutcome TerminalOutcome
        {
            get
            {
                lock (syncRoot)
                    return terminalOutcome;
            }
        }

        public AnalyticsRecordResult TryBeginLevelAttempt(string attemptKey)
        {
            lock (syncRoot)
            {
                if (!configurationValid)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelConfigurationInvalid
                    );
                }

                if (!IsCanonicalAttemptKey(attemptKey))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                if (begunAttemptKeys.Contains(attemptKey))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptDuplicate
                    );
                }

                if (hasActiveAttempt && !terminalAccepted)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptAlreadyActive
                    );
                }

                if (begunAttemptKeys.Count >= attemptCapacity)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelCapacityReached
                    );
                }

                begunAttemptKeys.Add(attemptKey);
                hasActiveAttempt = true;
                activeAttemptKey = attemptKey;
                terminalAccepted = false;
                terminalOutcome = AnalyticsLevelTerminalOutcome.None;
                return AnalyticsRecordResult.Success();
            }
        }

        /// <summary>
        /// Atomically accepts a begin transition only after the supplied record
        /// operation succeeds. A rejected, null, or throwing operation leaves
        /// every funnel field and replay key unchanged.
        /// </summary>
        public AnalyticsRecordResult TryBeginLevelAttempt(
            string attemptKey,
            Func<AnalyticsRecordResult> tryRecord
        )
        {
            lock (syncRoot)
            {
                AnalyticsRecordResult validation = ValidateBegin(attemptKey);
                if (!validation.Accepted)
                    return validation;

                AnalyticsRecordResult recorded = InvokeRecord(tryRecord);
                if (!recorded.Accepted)
                    return recorded;

                begunAttemptKeys.Add(attemptKey);
                hasActiveAttempt = true;
                activeAttemptKey = attemptKey;
                terminalAccepted = false;
                terminalOutcome = AnalyticsLevelTerminalOutcome.None;
                return recorded;
            }
        }

        public AnalyticsRecordResult TryCompleteLevelAttempt(string attemptKey)
        {
            return TryAcceptTerminal(
                attemptKey,
                AnalyticsLevelTerminalOutcome.Complete
            );
        }

        public AnalyticsRecordResult TryFailLevelAttempt(string attemptKey)
        {
            return TryAcceptTerminal(
                attemptKey,
                AnalyticsLevelTerminalOutcome.Fail
            );
        }

        public AnalyticsRecordResult TryAcceptTerminal(
            string attemptKey,
            AnalyticsLevelTerminalOutcome outcome
        )
        {
            lock (syncRoot)
            {
                if (!configurationValid)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelConfigurationInvalid
                    );
                }

                if (!IsCanonicalAttemptKey(attemptKey))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                if (
                    outcome != AnalyticsLevelTerminalOutcome.Complete &&
                    outcome != AnalyticsLevelTerminalOutcome.Fail
                )
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelOutcomeInvalid
                    );
                }

                if (!hasActiveAttempt)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelTerminalOutOfOrder
                    );
                }

                if (!string.Equals(
                    activeAttemptKey,
                    attemptKey,
                    StringComparison.Ordinal
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptMismatch
                    );
                }

                if (terminalAccepted)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelTerminalDuplicate
                    );
                }

                terminalAccepted = true;
                terminalOutcome = outcome;
                acceptedTerminalCount++;
                return AnalyticsRecordResult.Success();
            }
        }

        /// <summary>
        /// Atomically accepts a terminal transition only after the supplied
        /// record operation succeeds. Rejection cannot consume the terminal.
        /// </summary>
        public AnalyticsRecordResult TryAcceptTerminal(
            string attemptKey,
            AnalyticsLevelTerminalOutcome outcome,
            Func<AnalyticsRecordResult> tryRecord
        )
        {
            lock (syncRoot)
            {
                AnalyticsRecordResult validation = ValidateTerminal(
                    attemptKey,
                    outcome
                );
                if (!validation.Accepted)
                    return validation;

                AnalyticsRecordResult recorded = InvokeRecord(tryRecord);
                if (!recorded.Accepted)
                    return recorded;

                terminalAccepted = true;
                terminalOutcome = outcome;
                acceptedTerminalCount++;
                return recorded;
            }
        }

        /// <summary>
        /// Cancels only the currently active process-local attempt. The begun
        /// key remains retained, so cancellation cannot be used to replay the
        /// same attempt identity. No analytics event is emitted by this state
        /// transition; a typed abandon event requires a separately reviewed
        /// schema revision.
        /// </summary>
        public AnalyticsRecordResult TryCancelActiveLevelAttempt(
            string attemptKey
        )
        {
            lock (syncRoot)
            {
                if (!configurationValid)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelConfigurationInvalid
                    );
                }

                if (!IsCanonicalAttemptKey(attemptKey))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                    );
                }

                if (!hasActiveAttempt)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelTerminalOutOfOrder
                    );
                }

                if (!string.Equals(
                    activeAttemptKey,
                    attemptKey,
                    StringComparison.Ordinal
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelAttemptMismatch
                    );
                }

                if (terminalAccepted)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.FunnelTerminalDuplicate
                    );
                }

                hasActiveAttempt = false;
                activeAttemptKey = string.Empty;
                terminalAccepted = false;
                terminalOutcome = AnalyticsLevelTerminalOutcome.None;
                return AnalyticsRecordResult.Success();
            }
        }

        /// <summary>
        /// Clears only process-local funnel memory. There is no persistence or
        /// external state to reset.
        /// </summary>
        public void Reset()
        {
            lock (syncRoot)
            {
                begunAttemptKeys.Clear();
                hasActiveAttempt = false;
                activeAttemptKey = string.Empty;
                terminalAccepted = false;
                terminalOutcome = AnalyticsLevelTerminalOutcome.None;
                acceptedTerminalCount = 0;
            }
        }

        AnalyticsRecordResult ValidateBegin(string attemptKey)
        {
            if (!configurationValid)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelConfigurationInvalid
                );
            }

            if (!IsCanonicalAttemptKey(attemptKey))
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                );
            }

            if (begunAttemptKeys.Contains(attemptKey))
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelAttemptDuplicate
                );
            }

            if (hasActiveAttempt && !terminalAccepted)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelAttemptAlreadyActive
                );
            }

            if (begunAttemptKeys.Count >= attemptCapacity)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelCapacityReached
                );
            }

            return AnalyticsRecordResult.Success();
        }

        AnalyticsRecordResult ValidateTerminal(
            string attemptKey,
            AnalyticsLevelTerminalOutcome outcome
        )
        {
            if (!configurationValid)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelConfigurationInvalid
                );
            }

            if (!IsCanonicalAttemptKey(attemptKey))
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelAttemptKeyInvalid
                );
            }

            if (
                outcome != AnalyticsLevelTerminalOutcome.Complete &&
                outcome != AnalyticsLevelTerminalOutcome.Fail
            )
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelOutcomeInvalid
                );
            }

            if (!hasActiveAttempt)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelTerminalOutOfOrder
                );
            }

            if (!string.Equals(
                activeAttemptKey,
                attemptKey,
                StringComparison.Ordinal
            ))
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelAttemptMismatch
                );
            }

            if (terminalAccepted)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.FunnelTerminalDuplicate
                );
            }

            return AnalyticsRecordResult.Success();
        }

        static AnalyticsRecordResult InvokeRecord(
            Func<AnalyticsRecordResult> tryRecord
        )
        {
            if (tryRecord == null)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterFailure
                );
            }

            try
            {
                return tryRecord() ?? AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterFailure
                );
            }
            catch (Exception)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterFailure
                );
            }
        }

        static bool IsCanonicalAttemptKey(string attemptKey)
        {
            const string separator = ".attempt.";
            if (
                !AnalyticsEventValidator.IsCanonicalIdentifier(
                    attemptKey,
                    MaximumAttemptKeyLength
                )
            )
            {
                return false;
            }

            int separatorIndex = attemptKey.LastIndexOf(
                separator,
                StringComparison.Ordinal
            );
            if (
                separatorIndex < 1 ||
                separatorIndex + separator.Length >= attemptKey.Length
            )
            {
                return false;
            }

            string levelId = attemptKey.Substring(0, separatorIndex);
            string attemptText = attemptKey.Substring(
                separatorIndex + separator.Length
            );
            long attemptNumber;
            if (
                !AnalyticsEventValidator.IsCanonicalIdentifier(levelId, 64) ||
                !long.TryParse(
                    attemptText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out attemptNumber
                ) ||
                attemptNumber < 1L
            )
            {
                return false;
            }

            return string.Equals(
                attemptText,
                attemptNumber.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal
            );
        }
    }
}
