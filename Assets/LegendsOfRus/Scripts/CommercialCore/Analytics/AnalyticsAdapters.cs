using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LegendsOfRus.CommercialCore.Analytics
{
    /// <summary>
    /// Synchronous record boundary. The detached Step 1 pipeline rejects any
    /// adapter that declares upload support.
    /// </summary>
    public interface IAnalyticsAdapter
    {
        string AdapterId { get; }
        bool CaptureEnabled { get; }
        bool SupportsUpload { get; }
        AnalyticsRecordResult TryRecord(AnalyticsEventEnvelope envelope);
        AnalyticsRecordResult TryFlush();
    }

    public sealed class DisabledAnalyticsAdapter : IAnalyticsAdapter
    {
        public static DisabledAnalyticsAdapter Instance { get; } =
            new DisabledAnalyticsAdapter();

        public string AdapterId => "disabled";
        public bool CaptureEnabled => false;
        public bool SupportsUpload => false;

        DisabledAnalyticsAdapter() { }

        public AnalyticsRecordResult TryRecord(AnalyticsEventEnvelope envelope)
        {
            return AnalyticsRecordResult.Reject(AnalyticsReasonCodes.Disabled);
        }

        public AnalyticsRecordResult TryFlush()
        {
            return AnalyticsRecordResult.Reject(
                AnalyticsReasonCodes.UploadUnavailable
            );
        }
    }

    /// <summary>
    /// Bounded process-memory sandbox. It performs no logging, persistence,
    /// retry, background work, or upload. Capacity exhaustion rejects the new
    /// record and preserves every previously accepted envelope.
    /// </summary>
    public sealed class DeterministicOfflineAnalyticsAdapter : IAnalyticsAdapter
    {
        public const int DefaultCapacity = 256;
        public const int MaximumCapacity = 4096;

        readonly object syncRoot = new object();
        readonly List<AnalyticsEventEnvelope> records =
            new List<AnalyticsEventEnvelope>();
        readonly int capacity;
        readonly bool configurationValid;

        public string AdapterId => "offline_sandbox";
        public bool CaptureEnabled => configurationValid;
        public bool SupportsUpload => false;
        public int Capacity => capacity;

        public int Count
        {
            get
            {
                lock (syncRoot)
                    return records.Count;
            }
        }

        public DeterministicOfflineAnalyticsAdapter(
            int capacity = DefaultCapacity
        )
        {
            this.capacity = capacity;
            configurationValid = capacity >= 1 && capacity <= MaximumCapacity;
        }

        public AnalyticsRecordResult TryRecord(AnalyticsEventEnvelope envelope)
        {
            if (!configurationValid)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterConfigurationInvalid
                );
            }

            AnalyticsRecordResult validation =
                AnalyticsEventValidator.ValidateEnvelope(envelope);
            if (!validation.Accepted)
                return validation;

            lock (syncRoot)
            {
                if (records.Count >= capacity)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.CapacityReached
                    );
                }

                if (records.Count == 0)
                {
                    if (envelope.Sequence != 1L)
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.SequenceInvalid
                        );
                    }

                    if (!string.Equals(
                        envelope.EventId,
                        AnalyticsEventIds.SessionStart,
                        StringComparison.Ordinal
                    ))
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.SessionNotStarted
                        );
                    }
                }
                else
                {
                    AnalyticsEventEnvelope previous = records[records.Count - 1];
                    if (string.Equals(
                        envelope.EventId,
                        AnalyticsEventIds.SessionStart,
                        StringComparison.Ordinal
                    ))
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.SessionAlreadyStarted
                        );
                    }

                    if (!string.Equals(
                        previous.SessionId,
                        envelope.SessionId,
                        StringComparison.Ordinal
                    ))
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.SessionMismatch
                        );
                    }

                    if (envelope.Sequence != previous.Sequence + 1L)
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.SequenceInvalid
                        );
                    }

                    if (envelope.UtcTicks < previous.UtcTicks)
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.TimestampRollback
                        );
                    }
                }

                records.Add(envelope.DetachedCopy());
                return AnalyticsRecordResult.Success();
            }
        }

        public AnalyticsRecordResult TryFlush()
        {
            return AnalyticsRecordResult.Reject(
                AnalyticsReasonCodes.UploadUnavailable
            );
        }

        public IReadOnlyList<AnalyticsEventEnvelope> Snapshot()
        {
            lock (syncRoot)
            {
                var copy = new AnalyticsEventEnvelope[records.Count];
                for (int i = 0; i < records.Count; i++)
                    copy[i] = records[i].DetachedCopy();
                return new ReadOnlyCollection<AnalyticsEventEnvelope>(copy);
            }
        }

        public string CanonicalSnapshot()
        {
            return AnalyticsDeterministicJson.SerializeTrace(Snapshot());
        }
    }
}
