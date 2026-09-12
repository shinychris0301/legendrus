using System;
using System.Collections.Generic;

namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Failure-isolating orchestration over the local buffer and a detached
    /// provider seam. The shipped source cannot invoke a network transport.
    /// </summary>
    public sealed class ErrorReportingPipeline
    {
        private const bool NetworkTransportCompiled = false;

        readonly object syncRoot = new object();
        readonly bool captureAllowed;
        readonly BoundedErrorReportBuffer buffer;
        readonly IErrorReportTransport transport;
        bool operationInProgress;

        public bool CaptureAllowed { get { return captureAllowed; } }
        public bool NetworkTransportAllowed { get { return false; } }
        public int BufferedCount { get { return buffer.Count; } }
        public ErrorReportPolicy Policy { get { return buffer.Policy; } }

        public ErrorReportingPipeline(
            bool captureAllowed,
            ErrorReportPolicy policy = null
        ) : this(
            captureAllowed,
            policy,
            DisabledErrorReportTransport.Instance
        ) { }

        internal ErrorReportingPipeline(
            bool captureAllowed,
            ErrorReportPolicy policy,
            IErrorReportTransport transport
        )
        {
            this.captureAllowed = captureAllowed;
            buffer = new BoundedErrorReportBuffer(policy);
            this.transport = transport ?? DisabledErrorReportTransport.Instance;
        }

        public ErrorReportOperationResult TryCapture(
            ErrorReportDraft draft,
            long utcTicks
        )
        {
            if (!captureAllowed)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.CaptureNotAllowed
                );
            if (!TryBeginOperation())
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.ReentrantOperationBlocked
                );

            try
            {
                ErrorReportOperationResult result =
                    buffer.TryCapture(draft, utcTicks);
                return result ?? ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.BufferFailure
                );
            }
            catch (Exception)
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.BufferFailure
                );
            }
            finally
            {
                EndOperation();
            }
        }

        public ErrorReportOperationResult TryFlushOne(
            long utcTicks,
            bool isOnline
        )
        {
            if (utcTicks < 1L)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.TimestampInvalid
                );
            if (!buffer.Policy.IsValid)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.PolicyInvalid
                );
            if (!TryBeginOperation())
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.ReentrantOperationBlocked
                );

            try
            {
                buffer.PruneExpired(utcTicks);
                if (!captureAllowed)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.CaptureNotAllowed
                    );
                if (!isOnline)
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.Offline
                    );

                bool enabled;
                bool supportsNetwork;
                try
                {
                    enabled = transport.DeliveryEnabled;
                    supportsNetwork = transport.SupportsNetwork;
                }
                catch (Exception)
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.TransportFailure
                    );
                }

                if (supportsNetwork && !NetworkTransportCompiled)
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.NetworkTransportBlocked
                    );
                }
                if (!enabled)
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.TransportDisabled
                    );
                }

                ErrorReportRecord record;
                if (!buffer.TryGetNextDue(utcTicks, out record))
                {
                    return ErrorReportOperationResult.Reject(
                        ErrorReportReasonCodes.NothingDue
                    );
                }

                ErrorReportTransportResult transportResult;
                try
                {
                    transportResult = transport.TrySend(record.DetachedCopy());
                }
                catch (Exception)
                {
                    ErrorReportOperationResult state =
                        buffer.MarkRetryableFailure(record.Sequence, utcTicks);
                    return FailureAfterRetryState(
                        state,
                        ErrorReportReasonCodes.TransportFailure,
                        record
                    );
                }

                if (transportResult == null)
                {
                    ErrorReportOperationResult state =
                        buffer.MarkRetryableFailure(record.Sequence, utcTicks);
                    return FailureAfterRetryState(
                        state,
                        ErrorReportReasonCodes.TransportFailure,
                        record
                    );
                }

                switch (transportResult.Disposition)
                {
                    case ErrorReportTransportDisposition.Delivered:
                        return buffer.MarkDelivered(record.Sequence);
                    case ErrorReportTransportDisposition.RetryableFailure:
                        return buffer.MarkRetryableFailure(
                            record.Sequence,
                            utcTicks
                        );
                    case ErrorReportTransportDisposition.PermanentFailure:
                        return buffer.MarkPermanentFailure(record.Sequence);
                    default:
                        ErrorReportOperationResult state =
                            buffer.MarkRetryableFailure(record.Sequence, utcTicks);
                        return FailureAfterRetryState(
                            state,
                            ErrorReportReasonCodes.TransportFailure,
                            record
                        );
                }
            }
            catch (Exception)
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.UnexpectedFailure
                );
            }
            finally
            {
                EndOperation();
            }
        }

        public IReadOnlyList<ErrorReportRecord> Snapshot()
        {
            try
            {
                return buffer.Snapshot();
            }
            catch (Exception)
            {
                return new ErrorReportRecord[0];
            }
        }

        public string CanonicalSnapshot()
        {
            try
            {
                return buffer.CanonicalSnapshot();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public int PruneExpired(long utcTicks)
        {
            if (!TryBeginOperation())
                return 0;
            try
            {
                return buffer.PruneExpired(utcTicks);
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                EndOperation();
            }
        }

        bool TryBeginOperation()
        {
            lock (syncRoot)
            {
                if (operationInProgress)
                    return false;
                operationInProgress = true;
                return true;
            }
        }

        void EndOperation()
        {
            lock (syncRoot)
                operationInProgress = false;
        }

        static ErrorReportOperationResult FailureAfterRetryState(
            ErrorReportOperationResult state,
            string fallbackReason,
            ErrorReportRecord record
        )
        {
            if (
                state != null &&
                state.ReasonCode == ErrorReportReasonCodes.RetryExhausted
            )
            {
                return state;
            }
            return ErrorReportOperationResult.Reject(
                fallbackReason,
                record.Sequence,
                record.Fingerprint
            );
        }
    }
}
