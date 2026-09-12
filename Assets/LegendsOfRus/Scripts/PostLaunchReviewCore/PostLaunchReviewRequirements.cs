namespace LegendsOfRus.PostLaunchReview
{
    public static class PostLaunchReviewRequirements
    {
        public const int SchemaVersion = 1;
        public const string GuardrailProfilePrefix = "LOR-GR-";
        public const string TrendWindowPrefix = "LOR-TW-";
        public const string RetentionCohortPrefix = "LOR-COHORT-";
        public const string EconomyFlowPrefix = "LOR-ECO-";
        public const int StableIdMaxLength = 64;
        public const int BasisPointsScale = 10000;
        public const int DefaultBufferCapacity = 30;
        public const int MaxBufferCapacity = 90;
        public const int MaxWindowDays = 90;
        public const int MaxAggregateCount = 1000000000;
        public const int MaxCurrencyAggregate = 2000000000;
        public const int MaxPerActiveUserGuardrail = 1000000;
    }
}
