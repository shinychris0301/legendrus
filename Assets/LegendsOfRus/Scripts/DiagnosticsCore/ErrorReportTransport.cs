namespace LegendsOfRus.DiagnosticsCore
{
    internal enum ErrorReportTransportDisposition
    {
        Delivered = 0,
        RetryableFailure = 1,
        PermanentFailure = 2
    }

    internal sealed class ErrorReportTransportResult
    {
        public ErrorReportTransportDisposition Disposition { get; private set; }

        ErrorReportTransportResult(ErrorReportTransportDisposition disposition)
        {
            Disposition = disposition;
        }

        public static ErrorReportTransportResult Delivered()
        {
            return new ErrorReportTransportResult(
                ErrorReportTransportDisposition.Delivered
            );
        }

        public static ErrorReportTransportResult RetryableFailure()
        {
            return new ErrorReportTransportResult(
                ErrorReportTransportDisposition.RetryableFailure
            );
        }

        public static ErrorReportTransportResult PermanentFailure()
        {
            return new ErrorReportTransportResult(
                ErrorReportTransportDisposition.PermanentFailure
            );
        }
    }

    /// <summary>
    /// Provider-neutral synchronous seam. Step 3 ships no network-capable
    /// implementation and the pipeline contains an independent compiled gate.
    /// </summary>
    internal interface IErrorReportTransport
    {
        string TransportId { get; }
        bool DeliveryEnabled { get; }
        bool SupportsNetwork { get; }
        ErrorReportTransportResult TrySend(ErrorReportRecord record);
    }

    internal sealed class DisabledErrorReportTransport : IErrorReportTransport
    {
        public static DisabledErrorReportTransport Instance { get; private set; } =
            new DisabledErrorReportTransport();

        public string TransportId { get { return "disabled"; } }
        public bool DeliveryEnabled { get { return false; } }
        public bool SupportsNetwork { get { return false; } }

        DisabledErrorReportTransport() { }

        public ErrorReportTransportResult TrySend(ErrorReportRecord record)
        {
            return ErrorReportTransportResult.PermanentFailure();
        }
    }
}
