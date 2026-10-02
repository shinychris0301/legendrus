using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.Compliance;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CompliancePlayMode
{
    public sealed class PrivacySupportFoundationPlayModeTests
    {
        const string StoreKey = "LegendsOfRus_Compliance_v1";

        static readonly PropertyInfo Singleton = typeof(PrivacySupportService).GetProperty("Instance");
        readonly List<GameObject> ownedHosts = new List<GameObject>();
        PrivacySupportService previousService;
        ComplianceStateSnapshot previousState;
        bool captured, hadStore;
        string previousStore;

        [SetUp]
        public void SetUp()
        {
            string isolated = System.Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(isolated, Does.StartWith("/tmp/"));
            Assert.That(Application.persistentDataPath, Does.StartWith(isolated + "/"));
            previousService = PrivacySupportService.Instance;
            previousState = previousService != null ? previousService.State : null;
            hadStore = PlayerPrefs.HasKey(StoreKey);
            previousStore = PlayerPrefs.GetString(StoreKey);
            captured = true;
            // Temporarily bind the singleton to owned fixture services. Preserve the
            // original component and state on the shared runtime root exactly.
            Singleton.SetValue(null, null);
            PlayerPrefs.DeleteKey(StoreKey);
            PlayerPrefs.Save();
        }

        [TearDown]
        public void TearDown()
        {
            if (!captured) return;
            foreach (var host in ownedHosts)
                if (host != null) Object.DestroyImmediate(host);
            ownedHosts.Clear();
            Singleton.SetValue(null, previousService);
            if (hadStore) PlayerPrefs.SetString(StoreKey, previousStore);
            else PlayerPrefs.DeleteKey(StoreKey);
            PlayerPrefs.Save();
            captured = false;
            Assert.That(PrivacySupportService.Instance, Is.SameAs(previousService));
            if (previousService != null) Assert.That(previousService.State, Is.SameAs(previousState));
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

        PrivacySupportService CreateService()
        {
            GameObject host = new GameObject("PrivacySupportService_Test");
            ownedHosts.Add(host);
            return host.AddComponent<PrivacySupportService>();
        }
    }
}
