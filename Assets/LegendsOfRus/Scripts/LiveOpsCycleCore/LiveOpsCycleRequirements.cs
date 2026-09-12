namespace LegendsOfRus.LiveOpsCycle
{
    public static class LiveOpsCycleRequirements
    {
        public const int SchemaVersion = 1;
        public const int MinimumRetrospectiveParticipants = 100;
        public const int MaxActionItems = 20;
        public const int Sha256HexLength = 64;
        public const int MinimumNextCadenceLeadMinutes = 1440;
        public const int MinimumCadenceGapMinutes = 120;
        public const int MaxPlannedEvents = 12;
        public const int MaxPlannedExperiments = 4;
        public const int DefaultObservationCapacity = 30;
        public const int MaxObservationCapacity = 90;
        public const string RetrospectivePrefix = "LOR-RETRO-";
        public const string ArchivePrefix = "LOR-ARCHIVE-";
        public const string CadencePrefix = "LOR-CADENCE-";
    }
}
