namespace LegendsOfRus.CommercialCore.Consent
{
    /// <summary>
    /// Coarse audience treatment selected by a future reviewed consent flow.
    /// It deliberately cannot represent a date of birth or a precise age.
    /// Unknown receives the same restricted treatment as Child.
    /// </summary>
    public enum ConsentAudience
    {
        Unknown = 0,
        Child = 1,
        GeneralAudience = 2
    }

    public enum ConsentDecision
    {
        Unknown = 0,
        Denied = 1,
        Granted = 2
    }

    /// <summary>
    /// Immutable, identifier-free consent state. This type contains no storage,
    /// clock, network, platform, or Unity dependencies.
    /// </summary>
    public sealed class ConsentSnapshot
    {
        public const int CurrentSchemaVersion = 1;
        public const int CurrentPolicyVersion = 1;

        static readonly ConsentSnapshot restricted = new ConsentSnapshot(
            CurrentSchemaVersion,
            CurrentPolicyVersion,
            ConsentAudience.Unknown,
            ConsentDecision.Unknown,
            ConsentDecision.Unknown
        );

        public int SchemaVersion { get; }
        public int PolicyVersion { get; }
        public ConsentAudience Audience { get; }
        public ConsentDecision Analytics { get; }
        public ConsentDecision RewardedAdvertising { get; }

        /// <summary>
        /// Conservative process-local default. It never authorizes analytics or
        /// advertising and is safe when loading or initialization has not occurred.
        /// </summary>
        public static ConsentSnapshot Restricted => restricted;

        public ConsentSnapshot(
            int schemaVersion,
            int policyVersion,
            ConsentAudience audience,
            ConsentDecision analytics,
            ConsentDecision rewardedAdvertising
        )
        {
            SchemaVersion = schemaVersion;
            PolicyVersion = policyVersion;
            Audience = audience;
            Analytics = analytics;
            RewardedAdvertising = rewardedAdvertising;
        }
    }
}
