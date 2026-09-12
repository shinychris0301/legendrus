using System;

namespace LegendsOfRus.CommercialCore.Analytics
{
    /// <summary>
    /// Consent-aware, detached analytics session. captureAllowed must be the
    /// result of external activation and consent composition. This class never
    /// reads consent, configuration, clocks, saves, or platform state itself.
    /// </summary>
    public sealed class AnalyticsPipeline
    {
        readonly object syncRoot = new object();
        readonly bool captureAllowed;
        readonly IAnalyticsAdapter adapter;

        bool sessionStarted;
        string sessionId = string.Empty;
        long lastSequence;
        long lastUtcTicks;

        public bool CaptureAllowed => captureAllowed;

        public bool HasStartedSession
        {
            get
            {
                lock (syncRoot)
                    return sessionStarted;
            }
        }

        public string SessionId
        {
            get
            {
                lock (syncRoot)
                    return sessionId;
            }
        }

        public long AcceptedCount
        {
            get
            {
                lock (syncRoot)
                    return lastSequence;
            }
        }

        public AnalyticsPipeline(
            bool captureAllowed,
            IAnalyticsAdapter adapter = null
        )
        {
            this.captureAllowed = captureAllowed;
            this.adapter = adapter ?? DisabledAnalyticsAdapter.Instance;
        }

        /// <summary>
        /// Begins exactly one process-local session. The accepted session_start
        /// envelope is always sequence 1 and therefore precedes all other events.
        /// </summary>
        public AnalyticsRecordResult TryBeginSession(
            string requestedSessionId,
            long utcTicks
        )
        {
            lock (syncRoot)
            {
                AnalyticsRecordResult readiness = CheckReadiness();
                if (!readiness.Accepted)
                    return readiness;

                if (sessionStarted)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.SessionAlreadyStarted
                    );
                }

                if (
                    !AnalyticsEventValidator.IsCanonicalIdentifier(
                        requestedSessionId,
                        AnalyticsSchemaRegistry.MaximumSessionIdLength
                    )
                )
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.SessionIdInvalid
                    );
                }

                if (utcTicks < 1L)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.TimestampInvalid
                    );
                }

                AnalyticsRecordResult schemaValidation =
                    AnalyticsEventValidator.ValidateRegistry();
                if (!schemaValidation.Accepted)
                    return schemaValidation;

                var envelope = new AnalyticsEventEnvelope(
                    AnalyticsSchemaRegistry.CurrentSchemaVersion,
                    AnalyticsEventIds.SessionStart,
                    requestedSessionId,
                    1L,
                    utcTicks,
                    new AnalyticsAttribute[0]
                );

                AnalyticsRecordResult result = TryAdapterRecord(envelope);
                if (!result.Accepted)
                    return result;

                sessionStarted = true;
                sessionId = requestedSessionId;
                lastSequence = 1L;
                lastUtcTicks = utcTicks;
                return AnalyticsRecordResult.Success();
            }
        }

        public AnalyticsRecordResult TryRecord(
            AnalyticsEventDraft draft,
            long utcTicks
        )
        {
            lock (syncRoot)
            {
                AnalyticsRecordResult readiness = CheckReadiness();
                if (!readiness.Accepted)
                    return readiness;

                if (!sessionStarted)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.SessionNotStarted
                    );
                }

                AnalyticsRecordResult validation =
                    AnalyticsEventValidator.ValidateDraft(draft);
                if (!validation.Accepted)
                    return validation;

                if (string.Equals(
                    draft.EventId,
                    AnalyticsEventIds.SessionStart,
                    StringComparison.Ordinal
                ))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.EventReserved
                    );
                }

                if (utcTicks < 1L)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.TimestampInvalid
                    );
                }

                if (utcTicks < lastUtcTicks)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.TimestampRollback
                    );
                }

                if (lastSequence == long.MaxValue)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.SequenceInvalid
                    );
                }

                AnalyticsAttribute[] attributes = draft.DetachedAttributes();
                Array.Sort(attributes, CompareAttributes);
                var envelope = new AnalyticsEventEnvelope(
                    AnalyticsSchemaRegistry.CurrentSchemaVersion,
                    draft.EventId,
                    sessionId,
                    lastSequence + 1L,
                    utcTicks,
                    attributes
                );

                AnalyticsRecordResult result = TryAdapterRecord(envelope);
                if (!result.Accepted)
                    return result;

                lastSequence = envelope.Sequence;
                lastUtcTicks = envelope.UtcTicks;
                return AnalyticsRecordResult.Success();
            }
        }

        /// <summary>
        /// Upload is intentionally absent from the detached foundation.
        /// </summary>
        public AnalyticsRecordResult TryFlush()
        {
            return AnalyticsRecordResult.Reject(
                AnalyticsReasonCodes.UploadUnavailable
            );
        }

        AnalyticsRecordResult CheckReadiness()
        {
            if (!captureAllowed)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.CaptureNotAllowed
                );
            }

            try
            {
                if (adapter.SupportsUpload)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.UploadUnavailable
                    );
                }

                if (!adapter.CaptureEnabled)
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.Disabled
                    );
                }
            }
            catch (Exception)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AdapterFailure
                );
            }

            return AnalyticsRecordResult.Success();
        }

        AnalyticsRecordResult TryAdapterRecord(AnalyticsEventEnvelope envelope)
        {
            try
            {
                AnalyticsRecordResult result = adapter.TryRecord(envelope);
                return result ?? AnalyticsRecordResult.Reject(
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

        static int CompareAttributes(
            AnalyticsAttribute left,
            AnalyticsAttribute right
        )
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left == null) return -1;
            if (right == null) return 1;
            return string.Compare(left.Key, right.Key, StringComparison.Ordinal);
        }
    }
}
