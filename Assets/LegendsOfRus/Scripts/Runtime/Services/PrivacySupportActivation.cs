using System;
using LegendsOfRus.Compliance;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class PrivacySupportActivationDto
    {
        public int schemaVersion;
        public bool localCompliancePersistenceApproved;
        public bool localSupportExportApproved;
        public bool finalLegalDocumentsApproved;
        public bool legalAcceptanceUiApproved;
        public bool supportUiApproved;
        public bool accountRecoveryUiApproved;
        public bool accountRecoveryBackendApproved;
        public bool dataDeletionBackendApproved;
        public bool productionAccountServicesApproved;
        public string privacyDocumentVersion;
        public string termsDocumentVersion;
    }

    public readonly struct PrivacySupportActivationSnapshot
    {
        public readonly bool LocalPersistenceApproved;
        public readonly bool LocalSupportExportApproved;
        public readonly bool FinalLegalDocumentsApproved;
        public readonly bool LegalAcceptanceUiApproved;
        public readonly bool SupportUiApproved;
        public readonly bool AccountRecoveryUiApproved;
        public readonly bool AccountRecoveryBackendApproved;
        public readonly bool DataDeletionBackendApproved;
        public readonly bool ProductionAccountServicesApproved;
        public readonly string PrivacyDocumentVersion;
        public readonly string TermsDocumentVersion;

        public PrivacySupportActivationSnapshot(
            bool localPersistenceApproved,
            bool localSupportExportApproved,
            bool finalLegalDocumentsApproved,
            bool legalAcceptanceUiApproved,
            bool supportUiApproved,
            bool accountRecoveryUiApproved,
            bool accountRecoveryBackendApproved,
            bool dataDeletionBackendApproved,
            bool productionAccountServicesApproved,
            string privacyDocumentVersion,
            string termsDocumentVersion)
        {
            LocalPersistenceApproved = localPersistenceApproved;
            LocalSupportExportApproved = localSupportExportApproved;
            FinalLegalDocumentsApproved = finalLegalDocumentsApproved;
            LegalAcceptanceUiApproved = legalAcceptanceUiApproved;
            SupportUiApproved = supportUiApproved;
            AccountRecoveryUiApproved = accountRecoveryUiApproved;
            AccountRecoveryBackendApproved = accountRecoveryBackendApproved;
            DataDeletionBackendApproved = dataDeletionBackendApproved;
            ProductionAccountServicesApproved = productionAccountServicesApproved;
            PrivacyDocumentVersion = privacyDocumentVersion ?? string.Empty;
            TermsDocumentVersion = termsDocumentVersion ?? string.Empty;
        }

        public bool MayPersistLocalState => LocalPersistenceApproved;
        public bool MayAcceptLegal => FinalLegalDocumentsApproved &&
            LegalAcceptanceUiApproved;
        public bool MayExportLocalSupportSnapshot => LocalSupportExportApproved;
        public bool AnyProductionAccountFeatureApproved =>
            AccountRecoveryBackendApproved ||
            DataDeletionBackendApproved ||
            ProductionAccountServicesApproved;
    }

    public static class PrivacySupportActivation
    {
        public const bool ProductionAccountTransportCompiled = false;
        public const bool RecoveryBackendCompiled = false;
        public const bool DataDeletionBackendCompiled = false;

        public static PrivacySupportActivationSnapshot Load()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                "Config/privacy_support_activation");
            if (asset == null)
                return Disabled();

            try
            {
                PrivacySupportActivationDto dto =
                    JsonUtility.FromJson<PrivacySupportActivationDto>(asset.text);
                if (dto == null || dto.schemaVersion != 1 ||
                    !CompliancePolicy.IsValidVersionToken(
                        dto.privacyDocumentVersion) ||
                    !CompliancePolicy.IsValidVersionToken(
                        dto.termsDocumentVersion))
                    return Disabled();

                if ((dto.accountRecoveryBackendApproved ||
                     dto.dataDeletionBackendApproved ||
                     dto.productionAccountServicesApproved) &&
                    (!ProductionAccountTransportCompiled ||
                     !RecoveryBackendCompiled ||
                     !DataDeletionBackendCompiled))
                    return Disabled();

                return new PrivacySupportActivationSnapshot(
                    dto.localCompliancePersistenceApproved,
                    dto.localSupportExportApproved,
                    dto.finalLegalDocumentsApproved,
                    dto.legalAcceptanceUiApproved,
                    dto.supportUiApproved,
                    dto.accountRecoveryUiApproved,
                    dto.accountRecoveryBackendApproved,
                    dto.dataDeletionBackendApproved,
                    dto.productionAccountServicesApproved,
                    dto.privacyDocumentVersion,
                    dto.termsDocumentVersion
                );
            }
            catch
            {
                return Disabled();
            }
        }

        static PrivacySupportActivationSnapshot Disabled()
        {
            return new PrivacySupportActivationSnapshot(
                false, false, false, false, false, false,
                false, false, false, string.Empty, string.Empty
            );
        }
    }
}
