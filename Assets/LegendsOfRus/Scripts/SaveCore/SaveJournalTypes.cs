namespace LegendsOfRus.SaveCore
{
    public enum SaveJournalSlot
    {
        None = 0,
        Primary = 1,
        Backup = 2,
        Staging = 3
    }

    public enum SaveCandidateState
    {
        Missing = 0,
        Valid = 1,
        Invalid = 2,
        UnsupportedFuture = 3,
        StorageFailure = 4
    }

    public enum SavePayloadFormat
    {
        None = 0,
        EnvelopeV1 = 1,
        Legacy = 2
    }

    public enum SaveJournalLoadStatus
    {
        LoadedPrimary = 0,
        RecoveredBackup = 1,
        NoSave = 2,
        Unrecoverable = 3,
        UnsupportedFutureVersion = 4,
        StorageFailure = 5
    }

    public sealed class SaveJournalLoadResult
    {
        internal SaveJournalLoadResult(
            SaveJournalLoadStatus status,
            SaveJournalSlot source,
            SavePayloadFormat format,
            string payload,
            int payloadVersion,
            long revision,
            SaveCandidateState primaryState,
            SaveCandidateState backupState,
            SaveCandidateState stagingState,
            string diagnosticCode)
        {
            Status = status;
            Source = source;
            Format = format;
            Payload = payload;
            PayloadVersion = payloadVersion;
            Revision = revision;
            PrimaryState = primaryState;
            BackupState = backupState;
            StagingState = stagingState;
            DiagnosticCode = diagnosticCode ?? string.Empty;
        }

        public SaveJournalLoadStatus Status { get; private set; }
        public bool Success
        {
            get
            {
                return Status == SaveJournalLoadStatus.LoadedPrimary
                    || Status == SaveJournalLoadStatus.RecoveredBackup;
            }
        }
        public SaveJournalSlot Source { get; private set; }
        public SavePayloadFormat Format { get; private set; }
        public string Payload { get; private set; }
        public int PayloadVersion { get; private set; }
        public long Revision { get; private set; }
        public SaveCandidateState PrimaryState { get; private set; }
        public SaveCandidateState BackupState { get; private set; }
        public SaveCandidateState StagingState { get; private set; }
        public bool PrimaryWasMissing { get { return PrimaryState == SaveCandidateState.Missing; } }
        public string DiagnosticCode { get; private set; }

        public override string ToString()
        {
            return Status + ":" + DiagnosticCode;
        }
    }

    public enum SaveJournalWriteStatus
    {
        Success = 0,
        InvalidInput = 1,
        UnsupportedFutureVersion = 2,
        StorageFailure = 3,
        RevisionExhausted = 4,
        EncodingFailure = 5
    }

    public enum SaveJournalWritePhase
    {
        None = 0,
        Scan = 1,
        StagingWrite = 2,
        StagingFlush = 3,
        BackupWrite = 4,
        BackupFlush = 5,
        PrimaryWrite = 6,
        PrimaryFlush = 7,
        StagingDelete = 8,
        StagingDeleteFlush = 9
    }

    public enum SaveJournalWriteMode
    {
        Ordinary = 0,
        ReplaceHistory = 1
    }

    public sealed class SaveJournalWriteResult
    {
        internal SaveJournalWriteResult(
            SaveJournalWriteStatus status,
            SaveJournalWritePhase phase,
            SaveJournalWriteMode mode,
            long revision,
            string diagnosticCode,
            bool primaryDurablyCommitted = false)
        {
            Status = status;
            Phase = phase;
            Mode = mode;
            Revision = revision;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            PrimaryDurablyCommitted = primaryDurablyCommitted;
        }

        public SaveJournalWriteStatus Status { get; private set; }
        public bool Success { get { return Status == SaveJournalWriteStatus.Success; } }
        public SaveJournalWritePhase Phase { get; private set; }
        public SaveJournalWriteMode Mode { get; private set; }
        public long Revision { get; private set; }
        public string DiagnosticCode { get; private set; }
        // True only after the primary write's Flush returned successfully. A
        // later redundancy/cleanup failure can therefore coexist with Success=false.
        public bool PrimaryDurablyCommitted { get; private set; }

        public override string ToString()
        {
            return Status + ":" + Phase + ":" + DiagnosticCode;
        }
    }
}
