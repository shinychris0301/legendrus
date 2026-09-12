namespace LegendsOfRus.LiveOpsReadiness
{
    public static class LiveOpsReadinessRequirements
    {
        public const int SchemaVersion = 1;
        public const int WeightBasisPointsTotal = 10000;
        public const int DefaultObservationCapacity = 30;
        public const int MaxObservationCapacity = 90;
        public const int MaxExperimentVariants = 4;
        public const string GuardrailProfilePrefix = "LOR-LO-GR-";
        public const string CadencePlanPrefix = "LOR-LO-CAD-";
        public const string EventPrefix = "LOR-LO-EVT-";
        public const string ContentHealthPrefix = "LOR-LO-HEALTH-";
        public const string ExperimentPrefix = "LOR-EXP-";
    }
}
