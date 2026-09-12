namespace LegendsOfRus.CandidateFreeze
{
    public static class CandidateFreezeRequirements
    {
        public const int SchemaVersion = 1;
        public const int RequiredPhysicalProfileCount = 5;
        public const int MinimumTestDurationMinutes = 5;
        public const string HashAlgorithm = "SHA-256";
        public const string CandidatePrefix = "v1.18-step05-rc";

        public static readonly string[] RequiredProfileIds =
        {
            "android_low_16_9_720p",
            "android_mid_18_9_1080p",
            "android_mid_19_5_9_1080p",
            "android_high_20_9_1080p",
            "android_high_20_9_1440p"
        };
    }
}
