using LegendsOfRus.Compliance;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Compliance
{
    public sealed class ComplianceFoundationEditModeTests
    {
        const string Privacy = "privacy-v1";
        const string Terms = "terms-v1";
        const string Reference = "0123456789abcdef0123456789abcdef";

        [Test]
        public void EmptyState_DoesNotAuthorizeOptionalDataUse()
        {
            ComplianceStateSnapshot state =
                ComplianceStateSnapshot.Empty(Reference);

            Assert.That(CompliancePolicy.IsValidState(state), Is.True);
            Assert.That(
                CompliancePolicy.MayUseAnalytics(state, Privacy, Terms),
                Is.False);
            Assert.That(
                CompliancePolicy.MayUseAds(state, Privacy, Terms),
                Is.False);
        }

        [Test]
        public void GrantedConsent_RequiresCurrentPrivacyAndTerms()
        {
            ComplianceStateSnapshot state = new ComplianceStateSnapshot(
                CompliancePolicy.CurrentSchemaVersion,
                Privacy,
                100L,
                Terms,
                101L,
                ConsentDecision.Granted,
                ConsentDecision.Granted,
                102L,
                Reference
            );

            Assert.That(CompliancePolicy.IsValidState(state), Is.True);
            Assert.That(
                CompliancePolicy.MayUseAnalytics(state, Privacy, Terms),
                Is.True);
            Assert.That(
                CompliancePolicy.MayUseAds(state, Privacy, Terms),
                Is.True);
            Assert.That(
                CompliancePolicy.MayUseAnalytics(
                    state,
                    "privacy-v2",
                    Terms),
                Is.False);
        }

        [Test]
        public void DeniedConsent_RemainsDeniedWithCurrentDocuments()
        {
            ComplianceStateSnapshot state = new ComplianceStateSnapshot(
                1, Privacy, 100L, Terms, 101L,
                ConsentDecision.Denied,
                ConsentDecision.Denied,
                102L,
                Reference
            );

            Assert.That(
                CompliancePolicy.MayUseAnalytics(state, Privacy, Terms),
                Is.False);
            Assert.That(
                CompliancePolicy.MayUseAds(state, Privacy, Terms),
                Is.False);
        }

        [TestCase("privacy-v1", true)]
        [TestCase("privacy.v1_2026", true)]
        [TestCase("", false)]
        [TestCase("privacy v1", false)]
        [TestCase("https://example.test", false)]
        public void VersionTokens_AreStrict(string value, bool expected)
        {
            Assert.That(
                CompliancePolicy.IsValidVersionToken(value),
                Is.EqualTo(expected));
        }

        [Test]
        public void SupportReference_IsLowerHexGuidNShape()
        {
            Assert.That(
                CompliancePolicy.IsValidSupportReference(Reference),
                Is.True);
            Assert.That(
                CompliancePolicy.IsValidSupportReference(
                    "0123456789ABCDEF0123456789ABCDEF"),
                Is.False);
        }

        [TestCase("ABCD-1234", true)]
        [TestCase("RECOVERY-CODE-2026", true)]
        [TestCase("short", false)]
        [TestCase("CODE WITH SPACE", false)]
        [TestCase("name@example.com", false)]
        public void RecoveryCandidateFormat_IsStrict(
            string value,
            bool expected)
        {
            Assert.That(
                AccountRecoveryPolicy.IsCandidateCodeValid(value),
                Is.EqualTo(expected));
        }

        [Test]
        public void RecoveryCandidatePersistence_IsHardBlocked()
        {
            Assert.That(
                AccountRecoveryPolicy.CandidatePersistenceAllowed,
                Is.False);
            Assert.That(
                AccountRecoveryPolicy.RecoveryBackendCompiled,
                Is.False);
            Assert.That(
                AccountRecoveryPolicy.AccountDeletionBackendCompiled,
                Is.False);
        }

        [Test]
        public void StateValidation_RejectsConsentTimestampMismatch()
        {
            ComplianceStateSnapshot invalid = new ComplianceStateSnapshot(
                1,
                string.Empty,
                0L,
                string.Empty,
                0L,
                ConsentDecision.Denied,
                ConsentDecision.Unknown,
                0L,
                Reference
            );

            Assert.That(CompliancePolicy.IsValidState(invalid), Is.False);
        }

        [Test]
        public void SupportText_RejectsControlCharacters()
        {
            Assert.That(SupportExportPolicy.IsSafeText("Pixel 8"), Is.True);
            Assert.That(SupportExportPolicy.IsSafeText("line1\nline2"), Is.False);
            Assert.That(
                SupportExportPolicy.SafeOrUnavailable("bad\rvalue"),
                Is.EqualTo("unavailable"));
        }
    }
}
