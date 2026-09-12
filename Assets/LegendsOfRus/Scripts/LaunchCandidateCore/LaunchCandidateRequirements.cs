namespace LegendsOfRus.LaunchCandidate
{
    public static class LaunchCandidateRequirements
    {
        public const int SchemaVersion = 1;
        public const string HashAlgorithm = "SHA-256";
        public const string CandidatePrefix = "v1.18-step05-rc";
        public const int MinimumTargetApiLevel = 36;
        public const int FirstProductionReleaseRolloutPercent = 100;
        public const int MinimumUpdateRolloutPercent = 1;
        public const int MaximumRolloutPercent = 100;
        public const int Sha256HexLength = 64;
    }
}
