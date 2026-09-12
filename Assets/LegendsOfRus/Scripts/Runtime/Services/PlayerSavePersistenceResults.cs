namespace LegendsOfRus
{
    public enum PlayerSaveLoadStatus
    {
        LoadedPrimary = 0,
        RecoveredBackup = 1,
        FirstRun = 2,
        InvalidPayload = 3,
        UnsupportedFutureVersion = 4,
        StorageFailure = 5
    }

    public sealed class PlayerSaveLoadResult
    {
        internal PlayerSaveLoadResult(
            PlayerSaveLoadStatus status,
            PlayerSaveData data,
            int payloadVersion,
            long revision,
            bool requiresRewrite,
            string diagnosticCode)
        {
            Status = status;
            Data = data;
            PayloadVersion = payloadVersion;
            Revision = revision;
            RequiresRewrite = requiresRewrite;
            DiagnosticCode = diagnosticCode ?? string.Empty;
        }

        public PlayerSaveLoadStatus Status { get; private set; }
        public PlayerSaveData Data { get; private set; }
        public int PayloadVersion { get; private set; }
        public long Revision { get; private set; }
        public bool RequiresRewrite { get; private set; }
        public string DiagnosticCode { get; private set; }

        public bool Succeeded
        {
            get
            {
                return Status == PlayerSaveLoadStatus.LoadedPrimary
                    || Status == PlayerSaveLoadStatus.RecoveredBackup
                    || Status == PlayerSaveLoadStatus.FirstRun;
            }
        }

        public bool Recovered
        {
            get { return Status == PlayerSaveLoadStatus.RecoveredBackup; }
        }

        public bool CanPersist { get { return Succeeded; } }
    }

    public enum PlayerSaveWriteStatus
    {
        Success = 0,
        InvalidInput = 1,
        WriteBlocked = 2,
        StorageFailure = 3,
        RevisionExhausted = 4,
        EncodingFailure = 5
    }

    public sealed class PlayerSaveWriteResult
    {
        internal PlayerSaveWriteResult(
            PlayerSaveWriteStatus status,
            string phase,
            long revision,
            string diagnosticCode,
            bool primaryDurablyCommitted = false)
        {
            Status = status;
            Phase = phase ?? string.Empty;
            Revision = revision;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            PrimaryDurablyCommitted = primaryDurablyCommitted;
        }

        public PlayerSaveWriteStatus Status { get; private set; }
        public string Phase { get; private set; }
        public long Revision { get; private set; }
        public string DiagnosticCode { get; private set; }
        public bool Success { get { return Status == PlayerSaveWriteStatus.Success; } }
        // Allows callers replacing history to adopt a committed value even if
        // backup or staging cleanup subsequently reported a storage failure.
        public bool PrimaryDurablyCommitted { get; private set; }

        public override string ToString()
        {
            return Status + ":" + Phase + ":" + DiagnosticCode;
        }
    }
}
