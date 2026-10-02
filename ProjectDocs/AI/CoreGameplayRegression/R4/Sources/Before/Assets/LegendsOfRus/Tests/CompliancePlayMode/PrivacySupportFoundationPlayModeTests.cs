using System.Collections;
using LegendsOfRus.Compliance;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CompliancePlayMode
{
    public sealed class PrivacySupportFoundationPlayModeTests
    {
        const string StoreKey = "LegendsOfRus_Compliance_v1";

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(StoreKey);
            PlayerPrefs.Save();
            DestroyExisting();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyExisting();
            PlayerPrefs.DeleteKey(StoreKey);
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator Activation_FailsClosedForLegalAndAccountBackends()
        {
            PrivacySupportService service = CreateService();
            yield return null;

            Assert.That(service.Activation.MayPersistLocalState, Is.True);
            Assert.That(
                service.Activation.MayExportLocalSupportSnapshot,
                Is.True);
            Assert.That(service.Activation.FinalLegalDocumentsApproved, Is.False);
            Assert.That(service.Activation.LegalAcceptanceUiApproved, Is.False);
            Assert.That(service.Activation.AnyProductionAccountFeatureApproved, Is.False);
            Assert.That(PrivacySupportActivation.RecoveryBackendCompiled, Is.False);
            Assert.That(
                PrivacySupportActivation.DataDeletionBackendCompiled,
                Is.False);
            Assert.That(service.MayUseAnalytics, Is.False);
            Assert.That(service.MayUseAds, Is.False);
        }

        [UnityTest]
        public IEnumerator DeniedConsent_PersistsAcrossServiceRestart()
        {
            PrivacySupportService first = CreateService();
            yield return null;

            ComplianceActionResult result =
                first.TrySetAnalyticsConsent(ConsentDecision.Denied);
            Assert.That(result.Succeeded, Is.True);
            string reference = first.State.SupportReference;

            Object.Destroy(first.gameObject);
            yield return null;

            PrivacySupportService second = CreateService();
            yield return null;

            Assert.That(
                second.State.AnalyticsConsent,
                Is.EqualTo(ConsentDecision.Denied));
            Assert.That(second.State.SupportReference, Is.EqualTo(reference));
            Assert.That(second.MayUseAnalytics, Is.False);
        }

        [UnityTest]
        public IEnumerator GrantedConsent_IsBlockedWithoutFinalLegalDocuments()
        {
            PrivacySupportService service = CreateService();
            yield return null;

            ComplianceActionResult result =
                service.TrySetAnalyticsConsent(ConsentDecision.Granted);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(
                result.Status,
                Is.EqualTo(
                    ComplianceActionStatus.ConsentPrerequisitesMissing));
            Assert.That(
                service.State.AnalyticsConsent,
                Is.EqualTo(ConsentDecision.Unknown));
        }

        [UnityTest]
        public IEnumerator SupportExport_IsAllowlistedAndLocalOnly()
        {
            PrivacySupportService service = CreateService();
            yield return null;

            ComplianceActionResult result =
                service.TryBuildSupportExport(out string json);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(json, Does.Contain("supportReference"));
            Assert.That(json, Does.Contain("applicationVersion"));
            Assert.That(json, Does.Contain("deviceModel"));
            Assert.That(json, Does.Not.Contain("deviceUniqueIdentifier"));
            Assert.That(json, Does.Not.Contain("password"));
            Assert.That(json, Does.Not.Contain("email"));
            Assert.That(json, Does.Not.Contain("recoveryCode"));
            Assert.That(json, Does.Not.Contain("token"));
        }

        [UnityTest]
        public IEnumerator RecoveryCandidate_IsNotPersistedWhenBackendUnavailable()
        {
            PrivacySupportService service = CreateService();
            yield return null;

            const string candidate = "RECOVERY-CODE-2026";
            ComplianceActionResult result =
                service.TryBeginAccountRecovery(candidate);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(
                result.Status,
                Is.EqualTo(ComplianceActionStatus.BackendUnavailable));
            Assert.That(
                PlayerPrefs.GetString(StoreKey, string.Empty),
                Does.Not.Contain(candidate));
        }

        [UnityTest]
        public IEnumerator LegalNavigation_FailsClosedWithoutApprovedScreens()
        {
            PrivacySupportService service = CreateService();
            yield return null;

            Assert.That(
                service.TryNavigate(ComplianceNavigationTarget.Privacy).Status,
                Is.EqualTo(ComplianceActionStatus.UiNotApproved));
            Assert.That(
                service.TryNavigate(ComplianceNavigationTarget.Terms).Status,
                Is.EqualTo(ComplianceActionStatus.UiNotApproved));
            Assert.That(
                service.TryNavigate(ComplianceNavigationTarget.Support).Status,
                Is.EqualTo(ComplianceActionStatus.UiNotApproved));
            Assert.That(
                service.TryNavigate(
                    ComplianceNavigationTarget.AccountRecovery).Status,
                Is.EqualTo(ComplianceActionStatus.UiNotApproved));
        }

        static PrivacySupportService CreateService()
        {
            GameObject host = new GameObject("PrivacySupportService_Test");
            return host.AddComponent<PrivacySupportService>();
        }

        static void DestroyExisting()
        {
            // Never destroy the shared persistent runtime root just because the
            // service is attached to it; other PlayMode fixtures may share that
            // root in the same Test Runner session.
            for (int i = 0; i < 8; i++)
            {
                PrivacySupportService existing =
                    Object.FindAnyObjectByType<PrivacySupportService>();
                if (existing == null)
                    return;

                if (existing.gameObject.name == "PrivacySupportService_Test")
                    Object.DestroyImmediate(existing.gameObject);
                else
                    Object.DestroyImmediate(existing);
            }
        }
    }
}
