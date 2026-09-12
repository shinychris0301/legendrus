using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Bounded process-memory evidence buffer. It has no file, PlayerPrefs,
    /// logging, timer, background-worker, SDK, or network dependency.
    /// </summary>
    public sealed class BoundedErrorReportBuffer
    {
        readonly object syncRoot = new object();
        readonly List<ErrorReportRecord> records =
            new List<ErrorReportRecord>();
        readonly ErrorReportPolicy policy;
        long lastSequence;
        long lastCaptureUtcTicks;

        public ErrorReportPolicy Policy { get { return policy; } }

        public int Count
        {
            get
            {
                lock (syncRoot)
                    return records.Count;
            }
        }

        public BoundedErrorReportBuffer(ErrorReportPolicy policy = null)
        {
            this.policy = policy ?? ErrorReportPolicy.Default;
        }

        public ErrorReportOperationResult TryCapture(
            ErrorReportDraft draft,
            long utcTicks
        )
        {
            if (!policy.IsValid)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.PolicyInvalid
                );

            ErrorReportOperationResult validation =
                ErrorReportValidator.ValidateDraft(draft);
            if (!validation.Accepted)
                return validation;
            if (utcTicks < 1L)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.TimestampInvalid
                );

            string fingerprint;
            if (!ErrorReportDeterminism.TryComputeFingerprint(
                draft,
                out fingerprint
            ))
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.FingerprintFailure
                );
            }

            lock (syncRoot)
            {
                if (
                    lastCaptureUtcTicks > 0L &&
                    utcTicks < lastCaptureUtcTicks
                )
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.TimestampRollback
                    );
                }

                PruneExpiredUnsafe(utcTicks);
                for (int i = records.Count - 1; i >= 0; i--)
                {
                    ErrorReportRecord existing = records[i];
                    if (!string.Equals(
                        existing.Fingerprint,
                        fingerprint,
                        StringComparison.Ordinal
                    ))
                    {
                        continue;
                    }
                    if (utcTicks < existing.LastUtcTicks)
                    {
                        return ErrorReportOperationResult.Reject(
                            ErrorReportReasonCodes.TimestampRollback,
                            existing.Sequence,
                            existing.Fingerprint
                        );
                    }
                    if (
                        utcTicks - existing.LastUtcTicks >
                        policy.DeduplicationWindowTicks
                    )
                    {
                        break;
                    }

                    long occurrences = existing.OccurrenceCount == long.MaxValue
                        ? long.MaxValue
                        : existing.OccurrenceCount + 1L;
                    records[i] = Copy(
                        existing,
                        utcTicks,
                        occurrences,
                        existing.DeliveryState,
                        existing.DeliveryAttempts,
                        existing.NextAttemptUtcTicks
                    );
                    lastCaptureUtcTicks = utcTicks;
                    return ErrorReportOperationResult.Success(
                        ErrorReportReasonCodes.Deduplicated,
                        existing.Sequence,
                        existing.Fingerprint
                    );
                }

                if (records.Count >= policy.Capacity)
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.CapacityReached
                    );
                }
                if (lastSequence == long.MaxValue)
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.SequenceExhausted
                    );
                }

                long sequence = lastSequence + 1L;
                var record = new ErrorReportRecord(
                    ErrorReportSchema.CurrentSchemaVersion,
                    sequence,
                    fingerprint,
                    draft.Kind,
                    draft.Subsystem,
                    draft.ErrorCode,
                    draft.SiteCode,
                    utcTicks,
                    utcTicks,
                    1L,
                    ErrorReportDeliveryState.Pending,
                    0,
                    utcTicks
                );
                records.Add(record);
                lastSequence = sequence;
                lastCaptureUtcTicks = utcTicks;
                return ErrorReportOperationResult.Success(
                    ErrorReportReasonCodes.Captured,
                    sequence,
                    fingerprint
                );
            }
        }

        public int PruneExpired(long utcTicks)
        {
            if (!policy.IsValid || utcTicks < 1L)
                return 0;
            lock (syncRoot)
                return PruneExpiredUnsafe(utcTicks);
        }

        public IReadOnlyList<ErrorReportRecord> Snapshot()
        {
            lock (syncRoot)
            {
                var copy = new ErrorReportRecord[records.Count];
                for (int i = 0; i < records.Count; i++)
                    copy[i] = records[i].DetachedCopy();
                return new ReadOnlyCollection<ErrorReportRecord>(copy);
            }
        }

        public string CanonicalSnapshot()
        {
            return ErrorReportDeterminism.SerializeTrace(Snapshot());
        }

        internal bool TryGetNextDue(
            long utcTicks,
            out ErrorReportRecord record
        )
        {
            record = null;
            if (!policy.IsValid || utcTicks < 1L)
                return false;
            lock (syncRoot)
            {
                PruneExpiredUnsafe(utcTicks);
                for (int i = 0; i < records.Count; i++)
                {
                    ErrorReportRecord candidate = records[i];
                    if (
                        (candidate.DeliveryState == ErrorReportDeliveryState.Pending ||
                         candidate.DeliveryState ==
                            ErrorReportDeliveryState.RetryScheduled) &&
                        utcTicks >= candidate.NextAttemptUtcTicks
                    )
                    {
                        record = candidate.DetachedCopy();
                        return true;
                    }
                }
                return false;
            }
        }

        internal ErrorReportOperationResult MarkDelivered(long sequence)
        {
            lock (syncRoot)
            {
                int index = IndexOfSequence(sequence);
                if (index < 0)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.RecordInvalid
                    );
                ErrorReportRecord record = records[index];
                records.RemoveAt(index);
                return ErrorReportOperationResult.Success(
                    ErrorReportReasonCodes.Delivered,
                    record.Sequence,
                    record.Fingerprint
                );
            }
        }

        internal ErrorReportOperationResult MarkRetryableFailure(
            long sequence,
            long utcTicks
        )
        {
            lock (syncRoot)
            {
                int index = IndexOfSequence(sequence);
                if (index < 0)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.RecordInvalid
                    );
                ErrorReportRecord record = records[index];
                if (utcTicks < 1L || utcTicks < record.LastUtcTicks)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.TimestampRollback,
                        record.Sequence,
                        record.Fingerprint
                    );

                int attempts = record.DeliveryAttempts == int.MaxValue
                    ? int.MaxValue
                    : record.DeliveryAttempts + 1;
                if (attempts >= policy.MaximumDeliveryAttempts)
                {
                    records[index] = Copy(
                        record,
                        record.LastUtcTicks,
                        record.OccurrenceCount,
                        ErrorReportDeliveryState.RetryExhausted,
                        attempts,
                        long.MaxValue
                    );
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.RetryExhausted,
                        record.Sequence,
                        record.Fingerprint
                    );
                }

                long delay = policy.RetryDelayTicks(attempts);
                long next = ErrorReportPolicy.SaturatingAdd(utcTicks, delay);
                records[index] = Copy(
                    record,
                    record.LastUtcTicks,
                    record.OccurrenceCount,
                    ErrorReportDeliveryState.RetryScheduled,
                    attempts,
                    next
                );
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RetryScheduled,
                    record.Sequence,
                    record.Fingerprint
                );
            }
        }

        internal ErrorReportOperationResult MarkPermanentFailure(
            long sequence
        )
        {
            lock (syncRoot)
            {
                int index = IndexOfSequence(sequence);
                if (index < 0)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.RecordInvalid
                    );
                ErrorReportRecord record = records[index];
                int attempts = record.DeliveryAttempts == int.MaxValue
                    ? int.MaxValue
                    : record.DeliveryAttempts + 1;
                records[index] = Copy(
                    record,
                    record.LastUtcTicks,
                    record.OccurrenceCount,
                    ErrorReportDeliveryState.PermanentlyRejected,
                    attempts,
                    long.MaxValue
                );
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.PermanentlyRejected,
                    record.Sequence,
                    record.Fingerprint
                );
            }
        }

        int PruneExpiredUnsafe(long utcTicks)
        {
            int removed = 0;
            for (int i = records.Count - 1; i >= 0; i--)
            {
                ErrorReportRecord record = records[i];
                if (
                    utcTicks >= record.FirstUtcTicks &&
                    utcTicks - record.FirstUtcTicks >= policy.RetentionTicks
                )
                {
                    records.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

        int IndexOfSequence(long sequence)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].Sequence == sequence)
                    return i;
            }
            return -1;
        }

        static ErrorReportRecord Copy(
            ErrorReportRecord source,
            long lastUtcTicks,
            long occurrenceCount,
            ErrorReportDeliveryState deliveryState,
            int deliveryAttempts,
            long nextAttemptUtcTicks
        )
        {
            return new ErrorReportRecord(
                source.SchemaVersion,
                source.Sequence,
                source.Fingerprint,
                source.Kind,
                source.Subsystem,
                source.ErrorCode,
                source.SiteCode,
                source.FirstUtcTicks,
                lastUtcTicks,
                occurrenceCount,
                deliveryState,
                deliveryAttempts,
                nextAttemptUtcTicks
            );
        }
    }
}
