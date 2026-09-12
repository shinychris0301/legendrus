using System;

namespace LegendsOfRus.Compliance
{
    public enum ConsentDecision
    {
        Unknown = 0,
        Denied = 1,
        Granted = 2
    }

    public enum LegalDocumentKind
    {
        PrivacyPolicy = 1,
        TermsOfService = 2
    }

    public enum ComplianceNavigationTarget
    {
        Privacy = 1,
        Terms = 2,
        Support = 3,
        AccountRecovery = 4,
        DataDeletion = 5
    }

    public enum ComplianceActionStatus
    {
        Succeeded = 0,
        InvalidInput = 1,
        PersistenceUnavailable = 2,
        LegalDocumentsNotFinal = 3,
        UiNotApproved = 4,
        ConsentPrerequisitesMissing = 5,
        BackendUnavailable = 6,
        ExportUnavailable = 7
    }

    public sealed class ComplianceStateSnapshot
    {
        public int SchemaVersion { get; }
        public string PrivacyVersion { get; }
        public long PrivacyAcceptedUtcTicks { get; }
        public string TermsVersion { get; }
        public long TermsAcceptedUtcTicks { get; }
        public ConsentDecision AnalyticsConsent { get; }
        public ConsentDecision AdsConsent { get; }
        public long ConsentUpdatedUtcTicks { get; }
        public string SupportReference { get; }

        public ComplianceStateSnapshot(
            int schemaVersion,
            string privacyVersion,
            long privacyAcceptedUtcTicks,
            string termsVersion,
            long termsAcceptedUtcTicks,
            ConsentDecision analyticsConsent,
            ConsentDecision adsConsent,
            long consentUpdatedUtcTicks,
            string supportReference)
        {
            SchemaVersion = schemaVersion;
            PrivacyVersion = privacyVersion ?? string.Empty;
            PrivacyAcceptedUtcTicks = privacyAcceptedUtcTicks;
            TermsVersion = termsVersion ?? string.Empty;
            TermsAcceptedUtcTicks = termsAcceptedUtcTicks;
            AnalyticsConsent = analyticsConsent;
            AdsConsent = adsConsent;
            ConsentUpdatedUtcTicks = consentUpdatedUtcTicks;
            SupportReference = supportReference ?? string.Empty;
        }

        public static ComplianceStateSnapshot Empty(string supportReference)
        {
            return new ComplianceStateSnapshot(
                CompliancePolicy.CurrentSchemaVersion,
                string.Empty,
                0L,
                string.Empty,
                0L,
                ConsentDecision.Unknown,
                ConsentDecision.Unknown,
                0L,
                supportReference
            );
        }
    }

    public readonly struct ComplianceActionResult
    {
        public readonly ComplianceActionStatus Status;
        public readonly string ReasonCode;

        public ComplianceActionResult(
            ComplianceActionStatus status,
            string reasonCode)
        {
            Status = status;
            ReasonCode = reasonCode ?? string.Empty;
        }

        public bool Succeeded => Status == ComplianceActionStatus.Succeeded;
    }
}
