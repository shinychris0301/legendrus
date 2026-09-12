using System;
using LegendsOfRus.Compliance;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class SupportExportDto
    {
        public int schemaVersion;
        public string supportReference;
        public string applicationVersion;
        public string unityVersion;
        public string platform;
        public string deviceModel;
        public string operatingSystem;
        public int systemMemoryMb;
        public int graphicsMemoryMb;
        public string saveLoadStatus;
        public bool contentCatalogReady;
        public bool localAddressablesActive;
    }

    public sealed class PrivacySupportService : MonoBehaviour
    {
        readonly PlayerPrefsComplianceStateStore store =
            new PlayerPrefsComplianceStateStore();

        PrivacySupportActivationSnapshot activation;
        ComplianceStateSnapshot state;

        public static PrivacySupportService Instance { get; private set; }
        public PrivacySupportActivationSnapshot Activation => activation;
        public ComplianceStateSnapshot State => state;

        public bool MayUseAnalytics =>
            activation.FinalLegalDocumentsApproved &&
            CompliancePolicy.MayUseAnalytics(
                state,
                activation.PrivacyDocumentVersion,
                activation.TermsDocumentVersion);

        public bool MayUseAds =>
            activation.FinalLegalDocumentsApproved &&
            CompliancePolicy.MayUseAds(
                state,
                activation.PrivacyDocumentVersion,
                activation.TermsDocumentVersion);

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            activation = PrivacySupportActivation.Load();
            LoadOrCreateState();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public ComplianceActionResult TryRecordLegalAcceptance(
            LegalDocumentKind kind)
        {
            if (!activation.FinalLegalDocumentsApproved)
                return Result(
                    ComplianceActionStatus.LegalDocumentsNotFinal,
                    "LEGAL_DOCUMENTS_NOT_FINAL");
            if (!activation.LegalAcceptanceUiApproved)
                return Result(
                    ComplianceActionStatus.UiNotApproved,
                    "LEGAL_ACCEPTANCE_UI_NOT_APPROVED");
            if (!activation.MayPersistLocalState || state == null)
                return Result(
                    ComplianceActionStatus.PersistenceUnavailable,
                    "COMPLIANCE_PERSISTENCE_UNAVAILABLE");

            string privacy = state.PrivacyVersion;
            long privacyTicks = state.PrivacyAcceptedUtcTicks;
            string terms = state.TermsVersion;
            long termsTicks = state.TermsAcceptedUtcTicks;
            long now = DateTime.UtcNow.Ticks;

            switch (kind)
            {
                case LegalDocumentKind.PrivacyPolicy:
                    privacy = activation.PrivacyDocumentVersion;
                    privacyTicks = now;
                    break;
                case LegalDocumentKind.TermsOfService:
                    terms = activation.TermsDocumentVersion;
                    termsTicks = now;
                    break;
                default:
                    return Result(
                        ComplianceActionStatus.InvalidInput,
                        "LEGAL_DOCUMENT_KIND_INVALID");
            }

            ComplianceStateSnapshot candidate = new ComplianceStateSnapshot(
                CompliancePolicy.CurrentSchemaVersion,
                privacy,
                privacyTicks,
                terms,
                termsTicks,
                state.AnalyticsConsent,
                state.AdsConsent,
                state.ConsentUpdatedUtcTicks,
                state.SupportReference
            );
            return Commit(candidate, "LEGAL_ACCEPTANCE_RECORDED");
        }

        public ComplianceActionResult TrySetAnalyticsConsent(
            ConsentDecision decision)
        {
            return TrySetConsent(decision, true);
        }

        public ComplianceActionResult TrySetAdsConsent(
            ConsentDecision decision)
        {
            return TrySetConsent(decision, false);
        }

        public ComplianceActionResult TryNavigate(
            ComplianceNavigationTarget target)
        {
            switch (target)
            {
                case ComplianceNavigationTarget.Privacy:
                case ComplianceNavigationTarget.Terms:
                    return activation.LegalAcceptanceUiApproved
                        ? Result(
                            ComplianceActionStatus.Succeeded,
                            "LEGAL_UI_BOUNDARY_READY")
                        : Result(
                            ComplianceActionStatus.UiNotApproved,
                            "LEGAL_UI_NOT_APPROVED");
                case ComplianceNavigationTarget.Support:
                    return activation.SupportUiApproved
                        ? Result(
                            ComplianceActionStatus.Succeeded,
                            "SUPPORT_UI_BOUNDARY_READY")
                        : Result(
                            ComplianceActionStatus.UiNotApproved,
                            "SUPPORT_UI_NOT_APPROVED");
                case ComplianceNavigationTarget.AccountRecovery:
                    return activation.AccountRecoveryUiApproved
                        ? Result(
                            ComplianceActionStatus.Succeeded,
                            "RECOVERY_UI_BOUNDARY_READY")
                        : Result(
                            ComplianceActionStatus.UiNotApproved,
                            "RECOVERY_UI_NOT_APPROVED");
                case ComplianceNavigationTarget.DataDeletion:
                    return activation.DataDeletionBackendApproved
                        ? Result(
                            ComplianceActionStatus.Succeeded,
                            "DELETION_BOUNDARY_READY")
                        : Result(
                            ComplianceActionStatus.BackendUnavailable,
                            "DELETION_BACKEND_UNAVAILABLE");
                default:
                    return Result(
                        ComplianceActionStatus.InvalidInput,
                        "COMPLIANCE_NAVIGATION_TARGET_INVALID");
            }
        }

        public ComplianceActionResult TryBeginAccountRecovery(
            string candidateCode)
        {
            if (!AccountRecoveryPolicy.IsCandidateCodeValid(candidateCode))
                return Result(
                    ComplianceActionStatus.InvalidInput,
                    "RECOVERY_CODE_FORMAT_INVALID");

            // Do not log, persist, copy, hash, or include candidateCode in a
            // diagnostic. The production backend is intentionally absent.
            if (!activation.AccountRecoveryBackendApproved ||
                !activation.ProductionAccountServicesApproved ||
                !PrivacySupportActivation.RecoveryBackendCompiled ||
                !PrivacySupportActivation.ProductionAccountTransportCompiled)
                return Result(
                    ComplianceActionStatus.BackendUnavailable,
                    "RECOVERY_BACKEND_UNAVAILABLE");

            return Result(
                ComplianceActionStatus.BackendUnavailable,
                "RECOVERY_BACKEND_NOT_IMPLEMENTED");
        }

        public ComplianceActionResult TryRequestAccountDeletion()
        {
            if (!activation.DataDeletionBackendApproved ||
                !activation.ProductionAccountServicesApproved ||
                !PrivacySupportActivation.DataDeletionBackendCompiled ||
                !PrivacySupportActivation.ProductionAccountTransportCompiled)
                return Result(
                    ComplianceActionStatus.BackendUnavailable,
                    "DELETION_BACKEND_UNAVAILABLE");

            return Result(
                ComplianceActionStatus.BackendUnavailable,
                "DELETION_BACKEND_NOT_IMPLEMENTED");
        }

        public ComplianceActionResult TryBuildSupportExport(out string json)
        {
            json = string.Empty;
            if (!activation.MayExportLocalSupportSnapshot || state == null)
                return Result(
                    ComplianceActionStatus.ExportUnavailable,
                    "SUPPORT_EXPORT_UNAVAILABLE");

            SupportExportDto dto = new SupportExportDto
            {
                schemaVersion = SupportExportPolicy.CurrentSchemaVersion,
                supportReference = state.SupportReference,
                applicationVersion = Safe(Application.version),
                unityVersion = Safe(Application.unityVersion),
                platform = Safe(Application.platform.ToString()),
                deviceModel = Safe(SystemInfo.deviceModel),
                operatingSystem = Safe(SystemInfo.operatingSystem),
                systemMemoryMb = Math.Max(0, SystemInfo.systemMemorySize),
                graphicsMemoryMb = Math.Max(0, SystemInfo.graphicsMemorySize),
                saveLoadStatus = Safe(
                    GameState.Instance != null &&
                    GameState.Instance.LastLoadResult != null
                        ? GameState.Instance.LastLoadResult.Status.ToString()
                        : "unavailable"),
                contentCatalogReady = ContentDeliveryService.Instance != null &&
                    ContentDeliveryService.Instance.CatalogReady,
                localAddressablesActive = ContentDeliveryService.Instance != null &&
                    ContentDeliveryService.Instance.LocalAddressablesActive
            };

            try
            {
                json = JsonUtility.ToJson(dto, true);
                return Result(
                    ComplianceActionStatus.Succeeded,
                    "SUPPORT_EXPORT_BUILT_LOCALLY");
            }
            catch
            {
                json = string.Empty;
                return Result(
                    ComplianceActionStatus.ExportUnavailable,
                    "SUPPORT_EXPORT_SERIALIZATION_FAILED");
            }
        }

        public ComplianceActionResult ClearLocalComplianceState()
        {
            if (!activation.MayPersistLocalState)
                return Result(
                    ComplianceActionStatus.PersistenceUnavailable,
                    "COMPLIANCE_PERSISTENCE_UNAVAILABLE");

            store.Clear();
            state = ComplianceStateSnapshot.Empty(NewSupportReference());
            return store.TrySave(state)
                ? Result(
                    ComplianceActionStatus.Succeeded,
                    "LOCAL_COMPLIANCE_STATE_RESET")
                : Result(
                    ComplianceActionStatus.PersistenceUnavailable,
                    "COMPLIANCE_PERSISTENCE_FAILED");
        }

        ComplianceActionResult TrySetConsent(
            ConsentDecision decision,
            bool analytics)
        {
            if (decision != ConsentDecision.Denied &&
                decision != ConsentDecision.Granted)
                return Result(
                    ComplianceActionStatus.InvalidInput,
                    "CONSENT_DECISION_INVALID");
            if (!activation.MayPersistLocalState || state == null)
                return Result(
                    ComplianceActionStatus.PersistenceUnavailable,
                    "COMPLIANCE_PERSISTENCE_UNAVAILABLE");

            if (decision == ConsentDecision.Granted &&
                (!activation.FinalLegalDocumentsApproved ||
                 !CompliancePolicy.HasCurrentLegalAcceptances(
                    state,
                    activation.PrivacyDocumentVersion,
                    activation.TermsDocumentVersion)))
                return Result(
                    ComplianceActionStatus.ConsentPrerequisitesMissing,
                    "CONSENT_LEGAL_PREREQUISITES_MISSING");

            ComplianceStateSnapshot candidate = new ComplianceStateSnapshot(
                CompliancePolicy.CurrentSchemaVersion,
                state.PrivacyVersion,
                state.PrivacyAcceptedUtcTicks,
                state.TermsVersion,
                state.TermsAcceptedUtcTicks,
                analytics ? decision : state.AnalyticsConsent,
                analytics ? state.AdsConsent : decision,
                DateTime.UtcNow.Ticks,
                state.SupportReference
            );
            return Commit(candidate, "CONSENT_UPDATED");
        }

        ComplianceActionResult Commit(
            ComplianceStateSnapshot candidate,
            string successCode)
        {
            if (!CompliancePolicy.IsValidState(candidate))
                return Result(
                    ComplianceActionStatus.InvalidInput,
                    "COMPLIANCE_STATE_INVALID");
            if (!store.TrySave(candidate))
                return Result(
                    ComplianceActionStatus.PersistenceUnavailable,
                    "COMPLIANCE_PERSISTENCE_FAILED");
            state = candidate;
            return Result(ComplianceActionStatus.Succeeded, successCode);
        }

        void LoadOrCreateState()
        {
            if (!activation.MayPersistLocalState)
            {
                state = ComplianceStateSnapshot.Empty(NewSupportReference());
                return;
            }

            if (store.TryLoad(out ComplianceStateSnapshot loaded))
            {
                state = loaded;
                return;
            }

            state = ComplianceStateSnapshot.Empty(NewSupportReference());
            store.TrySave(state);
        }

        static string NewSupportReference()
        {
            return Guid.NewGuid().ToString("N");
        }

        static string Safe(string value)
        {
            return SupportExportPolicy.SafeOrUnavailable(value ?? string.Empty);
        }

        static ComplianceActionResult Result(
            ComplianceActionStatus status,
            string reasonCode)
        {
            return new ComplianceActionResult(status, reasonCode);
        }
    }
}
