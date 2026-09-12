using System;

namespace LegendsOfRus.CommercialCore.Consent
{
    public sealed class ConsentValidationResult
    {
        public bool IsValid { get; }
        public string ReasonCode { get; }

        internal ConsentValidationResult(bool isValid, string reasonCode)
        {
            IsValid = isValid;
            ReasonCode = reasonCode ?? ConsentReasonCodes.ProviderFailed;
        }
    }

    /// <summary>
    /// Strict validation for the one supported schema and policy. Omitted,
    /// future, corrupt, or internally contradictory state is rejected rather
    /// than normalized into permission.
    /// </summary>
    public static class ConsentSnapshotValidator
    {
        static readonly ConsentValidationResult valid =
            new ConsentValidationResult(true, ConsentReasonCodes.Valid);

        public static ConsentValidationResult Validate(ConsentSnapshot snapshot)
        {
            if (snapshot == null)
                return Invalid(ConsentReasonCodes.SnapshotMissing);

            if (snapshot.SchemaVersion != ConsentSnapshot.CurrentSchemaVersion)
                return Invalid(ConsentReasonCodes.SchemaUnsupported);

            if (snapshot.PolicyVersion != ConsentSnapshot.CurrentPolicyVersion)
                return Invalid(ConsentReasonCodes.PolicyUnsupported);

            if (!Enum.IsDefined(typeof(ConsentAudience), snapshot.Audience))
                return Invalid(ConsentReasonCodes.AudienceInvalid);

            if (!Enum.IsDefined(typeof(ConsentDecision), snapshot.Analytics))
                return Invalid(ConsentReasonCodes.AnalyticsDecisionInvalid);

            if (!Enum.IsDefined(
                typeof(ConsentDecision),
                snapshot.RewardedAdvertising
            ))
            {
                return Invalid(
                    ConsentReasonCodes.RewardedAdvertisingDecisionInvalid
                );
            }

            if (
                snapshot.Audience != ConsentAudience.GeneralAudience &&
                (
                    snapshot.Analytics == ConsentDecision.Granted ||
                    snapshot.RewardedAdvertising == ConsentDecision.Granted
                )
            )
            {
                return Invalid(ConsentReasonCodes.RestrictedAudienceGrant);
            }

            return valid;
        }

        static ConsentValidationResult Invalid(string reasonCode)
        {
            return new ConsentValidationResult(false, reasonCode);
        }
    }
}
