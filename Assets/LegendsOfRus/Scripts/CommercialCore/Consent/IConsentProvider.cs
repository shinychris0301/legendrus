namespace LegendsOfRus.CommercialCore.Consent
{
    public enum ConsentProviderStatus
    {
        Ready = 0,
        Disabled = 1,
        InvalidSnapshot = 2,
        Failed = 3
    }

    /// <summary>
    /// A provider result always exposes a non-null, safe effective snapshot.
    /// Non-ready results expose ConsentSnapshot.Restricted.
    /// </summary>
    public sealed class ConsentProviderResult
    {
        public ConsentProviderStatus Status { get; }
        public ConsentSnapshot Snapshot { get; }
        public string ReasonCode { get; }
        public bool IsReady => Status == ConsentProviderStatus.Ready;

        ConsentProviderResult(
            ConsentProviderStatus status,
            ConsentSnapshot snapshot,
            string reasonCode
        )
        {
            Status = status;
            Snapshot = snapshot ?? ConsentSnapshot.Restricted;
            ReasonCode = reasonCode ?? ConsentReasonCodes.ProviderFailed;
        }

        public static ConsentProviderResult Ready(ConsentSnapshot snapshot)
        {
            return BuildReady(snapshot, ConsentReasonCodes.Valid);
        }

        internal static ConsentProviderResult ReadyForDevelopment(
            ConsentSnapshot snapshot,
            string reasonCode
        )
        {
            return BuildReady(snapshot, reasonCode);
        }

        static ConsentProviderResult BuildReady(
            ConsentSnapshot snapshot,
            string reasonCode
        )
        {
            ConsentValidationResult validation =
                ConsentSnapshotValidator.Validate(snapshot);
            return validation.IsValid
                ? new ConsentProviderResult(
                    ConsentProviderStatus.Ready,
                    snapshot,
                    reasonCode
                )
                : Invalid(validation.ReasonCode);
        }

        public static ConsentProviderResult Disabled()
        {
            return new ConsentProviderResult(
                ConsentProviderStatus.Disabled,
                ConsentSnapshot.Restricted,
                ConsentReasonCodes.ProviderDisabled
            );
        }

        internal static ConsentProviderResult Invalid(string reasonCode)
        {
            return new ConsentProviderResult(
                ConsentProviderStatus.InvalidSnapshot,
                ConsentSnapshot.Restricted,
                string.IsNullOrEmpty(reasonCode)
                    ? ConsentReasonCodes.ProviderSnapshotInvalid
                    : reasonCode
            );
        }

        public static ConsentProviderResult Failed()
        {
            return new ConsentProviderResult(
                ConsentProviderStatus.Failed,
                ConsentSnapshot.Restricted,
                ConsentReasonCodes.ProviderFailed
            );
        }
    }

    /// <summary>
    /// Supplies process-local consent state only. Persistence, platform prompts,
    /// user identity, and external communication are deliberately outside this
    /// Step 1 contract.
    /// </summary>
    public interface IConsentProvider
    {
        ConsentProviderResult GetConsent();
    }
}
