namespace LegendsOfRus.ClosedTestRollout
{
    public static class ClosedTestRolloutRequirements
    {
        public const int SchemaVersion = 1;
        public const int MinimumClosedTestObservationDays = 1;
        public const int MinimumClosedTestTesterCount = 1;
        public const int PersonalAccountProductionAccessTesterCount = 12;
        public const int PersonalAccountProductionAccessContinuousDays = 14;
        public const int FirstProductionReleasePercent = 100;
        public const int MinimumUpdateRolloutPercent = 1;
        public const int MaximumRolloutPercent = 100;
        public const string HashAlgorithm = "SHA-256";
        public const string CandidatePrefix = "v1.18-step05-rc";
    }
}
