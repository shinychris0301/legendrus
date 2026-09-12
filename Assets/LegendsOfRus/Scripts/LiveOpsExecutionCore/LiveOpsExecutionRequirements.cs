namespace LegendsOfRus.LiveOpsExecution
{
    public static class LiveOpsExecutionRequirements
    {
        public const int SchemaVersion = 1;
        public const int BasisPointsTotal = 10000;
        public const int DefaultObservationCapacity = 60;
        public const int MaxObservationCapacity = 180;
        public const int MaxRotationEntries = 32;
        public const int MaxObservationWindows = 60;
        public const int MinimumExperimentSamplesPerVariant = 100;
        public const int MaximumSampleRatioMismatchBasisPoints = 1000;
        public const string ExecutionPrefix = "LOR-LO-EXEC-";
        public const string RotationPrefix = "LOR-LO-ROT-";
        public const string ObservationPrefix = "LOR-EXP-OBS-";
    }
}
