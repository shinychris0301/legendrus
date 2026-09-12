namespace LegendsOfRus.LaunchOperations
{
    public static class LaunchOperationsRequirements
    {
        public const int SchemaVersion = 1;
        public const string HashAlgorithm = "SHA-256";
        public const string CandidatePrefix = "v1.18-step05-rc";
        public const int Sha256HexLength = 64;
        public const int FirstProductionReleaseRolloutPercent = 100;
        public const int MinimumUpdateRolloutPercent = 1;
        public const int MaximumRolloutPercent = 100;
        public const int MaximumVitalsWindowDays = 28;
        public const int ObservationBufferCapacity = 64;

        // Google Play Android vitals bad-behavior thresholds, rechecked 2026-09-09.
        public const double UserPerceivedCrashOverallBadBehaviorPercent = 1.09;
        public const double UserPerceivedAnrOverallBadBehaviorPercent = 0.47;
        public const double UserPerceivedCrashPhoneModelBadBehaviorPercent = 8.0;
        public const double UserPerceivedAnrPhoneModelBadBehaviorPercent = 8.0;
        public const double ExcessivePartialWakeLockOverallBadBehaviorPercent = 5.0;
        public const double ExcessiveBatteryUsageOverallBadBehaviorPercent = 1.0;

        // Local review band only; this is not a Google Play threshold.
        public const double WatchBandRatio = 0.75;
    }
}
