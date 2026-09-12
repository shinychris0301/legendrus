namespace LegendsOfRus.SaveCore
{
    public enum SaveEnvelopeIntent
    {
        Ordinary = 0,
        ReplaceHistory = 1
    }

    public enum SaveEnvelopeDecodeStatus
    {
        Success = 0,
        MissingInput = 1,
        NotEnvelope = 2,
        Malformed = 3,
        InvalidEncoding = 4,
        IntegrityMismatch = 5,
        UnsupportedEnvelopeVersion = 6,
        UnsupportedPayloadVersion = 7
    }

    public enum SaveEnvelopeEncodeStatus
    {
        Success = 0,
        InvalidInput = 1,
        EncodingFailure = 2
    }

    public sealed class SaveEnvelopeDecodeResult
    {
        internal SaveEnvelopeDecodeResult(
            SaveEnvelopeDecodeStatus status,
            string payload,
            int payloadVersion,
            long revision,
            SaveEnvelopeIntent intent,
            string diagnosticCode)
        {
            Status = status;
            Payload = payload;
            PayloadVersion = payloadVersion;
            Revision = revision;
            Intent = intent;
            DiagnosticCode = diagnosticCode ?? string.Empty;
        }

        public SaveEnvelopeDecodeStatus Status { get; private set; }
        public bool Success { get { return Status == SaveEnvelopeDecodeStatus.Success; } }
        public string Payload { get; private set; }
        public int PayloadVersion { get; private set; }
        public long Revision { get; private set; }
        public SaveEnvelopeIntent Intent { get; private set; }
        public string DiagnosticCode { get; private set; }

        public override string ToString()
        {
            return Status + ":" + DiagnosticCode;
        }
    }

    public sealed class SaveEnvelopeEncodeResult
    {
        internal SaveEnvelopeEncodeResult(
            SaveEnvelopeEncodeStatus status,
            string envelope,
            string diagnosticCode)
        {
            Status = status;
            Envelope = envelope;
            DiagnosticCode = diagnosticCode ?? string.Empty;
        }

        public SaveEnvelopeEncodeStatus Status { get; private set; }
        public bool Success { get { return Status == SaveEnvelopeEncodeStatus.Success; } }
        public string Envelope { get; private set; }
        public string DiagnosticCode { get; private set; }

        public override string ToString()
        {
            return Status + ":" + DiagnosticCode;
        }
    }
}
