namespace LegendsOfRus.LiveOpsOutcome
{
    public static class LiveOpsOutcomeRequirements
    {
        public const int SchemaVersion = 1;
        public const int BasisPointsTotal = 10000;
        public const int MinimumOutcomeParticipants = 100;
        public const int MinimumExperimentSamplesTotal = 200;
        public const int DefaultObservationCapacity = 30;
        public const int MaxObservationCapacity = 90;
        public const int MaxOutcomeWindows = 60;
        public const string OutcomePrefix = "LOR-LO-OUT-";
        public const string DecisionPrefix = "LOR-EXP-DEC-";
        public const string CloseoutPrefix = "LOR-ROT-CLOSE-";
    }
}
