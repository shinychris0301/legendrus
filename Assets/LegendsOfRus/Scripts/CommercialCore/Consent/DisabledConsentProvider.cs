namespace LegendsOfRus.CommercialCore.Consent
{
    /// <summary>
    /// Production-safe default. It cannot be configured into granting consent.
    /// </summary>
    public sealed class DisabledConsentProvider : IConsentProvider
    {
        static readonly DisabledConsentProvider instance =
            new DisabledConsentProvider();
        static readonly ConsentProviderResult disabled =
            ConsentProviderResult.Disabled();

        public static DisabledConsentProvider Instance => instance;

        DisabledConsentProvider()
        {
        }

        public ConsentProviderResult GetConsent()
        {
            return disabled;
        }
    }
}
