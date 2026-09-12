namespace LegendsOfRus.LiveOpsBaseline
{
    public static class LiveOpsBaselineRequirements
    {
        public const int SchemaVersion = 1;
        public const int RequiredV119StepCount = 7;
        public const int Sha256HexLength = 64;
        public const int MinimumRunbookSections = 5;
        public const int MinimumTransitionLeadHours = 24;
        public const int MaxCarryoverBlockers = 0;
        public const int DefaultObservationCapacity = 30;
        public const int MaxObservationCapacity = 90;
        public const string ChainPrefix = "LOR-V119-CHAIN-";
        public const string BaselinePrefix = "LOR-OPS-BASELINE-";
        public const string TransitionPrefix = "LOR-V120-TRANSITION-";
        public const string V120Milestone = "v1.20";
    }
}
