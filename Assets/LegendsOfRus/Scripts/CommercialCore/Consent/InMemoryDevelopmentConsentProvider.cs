namespace LegendsOfRus.CommercialCore.Consent
{
    /// <summary>
    /// Explicit test/development provider. State exists only in this instance,
    /// never persists, and invalid replacement fails closed to Restricted.
    /// Production composition must use DisabledConsentProvider until a reviewed
    /// platform consent flow and activation boundary are implemented.
    /// </summary>
    public sealed class InMemoryDevelopmentConsentProvider : IConsentProvider
    {
        ConsentProviderResult current;

        public InMemoryDevelopmentConsentProvider(ConsentSnapshot initialSnapshot)
        {
            current = BuildResult(
                initialSnapshot,
                ConsentReasonCodes.DevelopmentProviderReady
            );
        }

        public ConsentProviderResult GetConsent()
        {
            return current;
        }

        public ConsentProviderResult Replace(ConsentSnapshot snapshot)
        {
            current = BuildResult(
                snapshot,
                ConsentReasonCodes.DevelopmentProviderReady
            );
            return current;
        }

        public ConsentProviderResult ResetRestricted()
        {
            current = ConsentProviderResult.ReadyForDevelopment(
                ConsentSnapshot.Restricted,
                ConsentReasonCodes.DevelopmentProviderReset
            );
            return current;
        }

        static ConsentProviderResult BuildResult(
            ConsentSnapshot snapshot,
            string readyReasonCode
        )
        {
            ConsentValidationResult validation =
                ConsentSnapshotValidator.Validate(snapshot);
            return validation.IsValid
                ? ConsentProviderResult.ReadyForDevelopment(
                    snapshot,
                    readyReasonCode
                )
                : ConsentProviderResult.Invalid(validation.ReasonCode);
        }
    }
}
