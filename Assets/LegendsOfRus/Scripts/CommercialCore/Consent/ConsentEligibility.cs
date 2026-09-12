using System;

namespace LegendsOfRus.CommercialCore.Consent
{
    public sealed class ConsentEligibilityResult
    {
        public bool IsEligible { get; }
        public string ReasonCode { get; }

        internal ConsentEligibilityResult(bool isEligible, string reasonCode)
        {
            IsEligible = isEligible;
            ReasonCode = reasonCode ?? ConsentReasonCodes.ProviderFailed;
        }
    }

    /// <summary>
    /// Consent is necessary but never sufficient. Analytics or rewarded ads
    /// require both a valid affirmative consent decision and an independently
    /// reviewed external-services activation gate. The activation check occurs
    /// first so a disabled build never needs to query a provider.
    /// </summary>
    public static class ConsentEligibility
    {
        static readonly ConsentEligibilityResult eligible =
            new ConsentEligibilityResult(true, ConsentReasonCodes.Eligible);

        public static ConsentEligibilityResult ForAnalytics(
            IConsentProvider provider,
            bool externalServicesActivated
        )
        {
            return Evaluate(
                provider,
                externalServicesActivated,
                true
            );
        }

        public static ConsentEligibilityResult ForRewardedAdvertising(
            IConsentProvider provider,
            bool externalServicesActivated
        )
        {
            return Evaluate(
                provider,
                externalServicesActivated,
                false
            );
        }

        static ConsentEligibilityResult Evaluate(
            IConsentProvider provider,
            bool externalServicesActivated,
            bool analytics
        )
        {
            if (!externalServicesActivated)
                return Ineligible(ConsentReasonCodes.ExternalActivationDisabled);

            if (provider == null)
                return Ineligible(ConsentReasonCodes.ProviderMissing);

            ConsentProviderResult providerResult;
            try
            {
                providerResult = provider.GetConsent();
            }
            catch (Exception)
            {
                return Ineligible(ConsentReasonCodes.ProviderFailed);
            }

            if (providerResult == null)
                return Ineligible(ConsentReasonCodes.ProviderFailed);

            if (!providerResult.IsReady)
                return Ineligible(providerResult.ReasonCode);

            ConsentValidationResult validation =
                ConsentSnapshotValidator.Validate(providerResult.Snapshot);
            if (!validation.IsValid)
                return Ineligible(validation.ReasonCode);

            ConsentSnapshot snapshot = providerResult.Snapshot;
            if (snapshot.Audience != ConsentAudience.GeneralAudience)
                return Ineligible(ConsentReasonCodes.AudienceRestricted);

            ConsentDecision decision = analytics
                ? snapshot.Analytics
                : snapshot.RewardedAdvertising;
            if (decision == ConsentDecision.Unknown)
            {
                return Ineligible(
                    analytics
                        ? ConsentReasonCodes.AnalyticsDecisionRequired
                        : ConsentReasonCodes.RewardedAdvertisingDecisionRequired
                );
            }

            if (decision != ConsentDecision.Granted)
            {
                return Ineligible(
                    analytics
                        ? ConsentReasonCodes.AnalyticsDenied
                        : ConsentReasonCodes.RewardedAdvertisingDenied
                );
            }

            return eligible;
        }

        static ConsentEligibilityResult Ineligible(string reasonCode)
        {
            return new ConsentEligibilityResult(false, reasonCode);
        }
    }
}
