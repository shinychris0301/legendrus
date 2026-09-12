namespace LegendsOfRus.IncidentResponse
{
    public static class IncidentResponseRequirements
    {
        public const int SchemaVersion = 1;
        public const string HashAlgorithm = "SHA-256";
        public const int Sha256HexLength = 64;
        public const string IncidentIdPrefix = "LOR-INC-";
        public const string HotfixIdPrefix = "LOR-HF-";
        public const string HotfixCandidatePrefix = "v1.19-hotfix-rc";
        public const int NumericIdDigits = 4;
        public const int HotfixCandidateDigits = 2;
        public const int MaximumIncidentRecords = 256;
        public const int MaximumLinkedIncidents = 64;
        public const int MinimumRolloutPercent = 1;
        public const int MaximumRolloutPercent = 100;
        public const int FullRolloutPercent = 100;
        public const int MaximumHotfixScopeUnits = 32;

        // Policy notes rechecked against Google Play Console Help on 2026-09-09.
        // Staged rollouts may be halted/resumed for app updates; a first release cannot be staged.
        // Fully rolled-out releases may be halted only when an eligible previous release exists;
        // a first release on a track cannot be halted because no previous release can take its place.
        public const bool AutomaticPlayActionsAllowed = false;
    }
}
