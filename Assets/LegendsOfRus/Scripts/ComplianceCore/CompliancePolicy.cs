using System;

namespace LegendsOfRus.Compliance
{
    public static class CompliancePolicy
    {
        public const int CurrentSchemaVersion = 1;
        public const int MaximumVersionTokenLength = 64;
        public const int SupportReferenceLength = 32;

        public static bool IsValidVersionToken(string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length > MaximumVersionTokenLength)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool allowed =
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    c == '.' || c == '_' || c == '-';
                if (!allowed)
                    return false;
            }
            return true;
        }

        public static bool IsValidSupportReference(string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length != SupportReferenceLength)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool lowerHex =
                    (c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f');
                if (!lowerHex)
                    return false;
            }
            return true;
        }

        public static bool IsValidConsent(ConsentDecision value)
        {
            return value == ConsentDecision.Unknown ||
                value == ConsentDecision.Denied ||
                value == ConsentDecision.Granted;
        }

        public static bool IsValidState(ComplianceStateSnapshot value)
        {
            if (value == null ||
                value.SchemaVersion != CurrentSchemaVersion ||
                !IsValidSupportReference(value.SupportReference) ||
                !IsValidConsent(value.AnalyticsConsent) ||
                !IsValidConsent(value.AdsConsent))
                return false;

            if (!LegalPairIsValid(
                    value.PrivacyVersion,
                    value.PrivacyAcceptedUtcTicks) ||
                !LegalPairIsValid(
                    value.TermsVersion,
                    value.TermsAcceptedUtcTicks))
                return false;

            bool hasConsentDecision =
                value.AnalyticsConsent != ConsentDecision.Unknown ||
                value.AdsConsent != ConsentDecision.Unknown;
            if (hasConsentDecision && value.ConsentUpdatedUtcTicks <= 0L)
                return false;
            if (!hasConsentDecision && value.ConsentUpdatedUtcTicks != 0L)
                return false;

            return true;
        }

        public static bool IsDocumentCurrent(
            ComplianceStateSnapshot state,
            LegalDocumentKind kind,
            string currentVersion)
        {
            if (state == null || !IsValidVersionToken(currentVersion))
                return false;

            switch (kind)
            {
                case LegalDocumentKind.PrivacyPolicy:
                    return state.PrivacyAcceptedUtcTicks > 0L &&
                        string.Equals(
                            state.PrivacyVersion,
                            currentVersion,
                            StringComparison.Ordinal);
                case LegalDocumentKind.TermsOfService:
                    return state.TermsAcceptedUtcTicks > 0L &&
                        string.Equals(
                            state.TermsVersion,
                            currentVersion,
                            StringComparison.Ordinal);
                default:
                    return false;
            }
        }

        public static bool HasCurrentLegalAcceptances(
            ComplianceStateSnapshot state,
            string privacyVersion,
            string termsVersion)
        {
            return IsDocumentCurrent(
                    state,
                    LegalDocumentKind.PrivacyPolicy,
                    privacyVersion) &&
                IsDocumentCurrent(
                    state,
                    LegalDocumentKind.TermsOfService,
                    termsVersion);
        }

        public static bool MayUseAnalytics(
            ComplianceStateSnapshot state,
            string privacyVersion,
            string termsVersion)
        {
            return state != null &&
                state.AnalyticsConsent == ConsentDecision.Granted &&
                HasCurrentLegalAcceptances(
                    state,
                    privacyVersion,
                    termsVersion);
        }

        public static bool MayUseAds(
            ComplianceStateSnapshot state,
            string privacyVersion,
            string termsVersion)
        {
            return state != null &&
                state.AdsConsent == ConsentDecision.Granted &&
                HasCurrentLegalAcceptances(
                    state,
                    privacyVersion,
                    termsVersion);
        }

        static bool LegalPairIsValid(string version, long utcTicks)
        {
            bool hasVersion = !string.IsNullOrEmpty(version);
            if (!hasVersion)
                return utcTicks == 0L;
            return IsValidVersionToken(version) && utcTicks > 0L;
        }
    }
}
