using LegendsOfRus.IncidentResponse;
using NUnit.Framework;

namespace LegendsOfRus.Tests.IncidentResponse
{
    public sealed class HotfixQualificationEvaluatorEditModeTests
    {
        const string A = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string B = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        [Test]
        public void CompleteEvidence_QualifiesForManualPreparation()
        {
            var r = HotfixQualificationEvaluator.Evaluate(ReadyEvidence(), Register());
            Assert.That(r.QualifiedForManualReleasePreparation, Is.True);
        }

        [Test]
        public void ManualAuthorization_IsSeparateFromQualification()
        {
            var e = ReadyEvidence(); e.ManualHotfixReleaseAuthorized = false;
            var r = HotfixQualificationEvaluator.Evaluate(e, Register());
            Assert.That(r.ReadyForHumanHotfixReview, Is.True);
            Assert.That(r.MayProceedWithAuthorizedManualHotfixRelease, Is.False);
        }


        [Test]
        public void IncidentRegisterMustBeReviewed()
        {
            var register = Register(); register.EvidenceReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(ReadyEvidence(), register).ReviewBlockers, IncidentResponseBlockerCodes.RegisterNotReviewed);
        }

        [Test]
        public void MissingEvidence_Fails()
        {
            var r = HotfixQualificationEvaluator.Evaluate(null, Register());
            CollectionAssert.Contains(r.ReviewBlockers, IncidentResponseBlockerCodes.HotfixEvidenceMissing);
        }

        [Test]
        public void SameArtifactHash_Fails()
        {
            var e = ReadyEvidence(); e.HotfixArtifactSha256 = A;
            var r = HotfixQualificationEvaluator.Evaluate(e, Register());
            CollectionAssert.Contains(r.ReviewBlockers, IncidentResponseBlockerCodes.HotfixArtifactUnchanged);
        }

        [Test]
        public void MissingLinkedIncident_Fails()
        {
            var e = ReadyEvidence(); e.LinkedIncidentIds = new string[0];
            var r = HotfixQualificationEvaluator.Evaluate(e, Register());
            CollectionAssert.Contains(r.ReviewBlockers, IncidentResponseBlockerCodes.LinkedIncidentMissing);
        }

        [Test]
        public void UnknownLinkedIncident_Fails()
        {
            var e = ReadyEvidence(); e.LinkedIncidentIds = new[] { "LOR-INC-9999" };
            var r = HotfixQualificationEvaluator.Evaluate(e, Register());
            CollectionAssert.Contains(r.ReviewBlockers, IncidentResponseBlockerCodes.LinkedIncidentInvalid);
        }

        [Test]
        public void MissingRootCauseReview_Fails()
        {
            var e = ReadyEvidence(); e.RootCauseReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.RootCauseNotReviewed);
        }

        [Test]
        public void ScopeOverMaximum_Fails()
        {
            var e = ReadyEvidence(); e.ChangedScopeUnits = IncidentResponseRequirements.MaximumHotfixScopeUnits + 1;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.ScopeTooLarge);
        }

        [Test]
        public void CompileMustPass()
        {
            var e = ReadyEvidence(); e.UnityCompilePassed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.CompileNotVerified);
        }

        [Test]
        public void EditModeMustPass()
        {
            var e = ReadyEvidence(); e.EditModePassed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.EditModeNotVerified);
        }

        [Test]
        public void PlayModeMustPass()
        {
            var e = ReadyEvidence(); e.PlayModePassed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.PlayModeNotVerified);
        }

        [Test]
        public void DeviceSmokeMustPass()
        {
            var e = ReadyEvidence(); e.DeviceSmokePassed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.DeviceSmokeNotVerified);
        }

        [Test]
        public void SaveRiskMustBeReviewed()
        {
            var e = ReadyEvidence(); e.SaveMigrationRiskReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.SaveRiskNotReviewed);
        }

        [Test]
        public void EconomyRiskMustBeReviewed()
        {
            var e = ReadyEvidence(); e.EconomyRiskReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.EconomyRiskNotReviewed);
        }

        [Test]
        public void PrivacyRiskMustBeReviewed()
        {
            var e = ReadyEvidence(); e.PrivacyRiskReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.PrivacyRiskNotReviewed);
        }

        [Test]
        public void AabEvidenceMustBeRetained()
        {
            var e = ReadyEvidence(); e.AabEvidenceRetained = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.AabEvidenceMissing);
        }

        [Test]
        public void SigningEvidenceMustBeReviewed()
        {
            var e = ReadyEvidence(); e.SigningEvidenceReviewed = false;
            CollectionAssert.Contains(HotfixQualificationEvaluator.Evaluate(e, Register()).ReviewBlockers, IncidentResponseBlockerCodes.SigningEvidenceNotReviewed);
        }

        static IncidentRegisterEvidence Register() => new IncidentRegisterEvidence
        {
            EvidenceRetained = true, EvidenceReviewed = true,
            Incidents = new[] { new IncidentRecord { IncidentId = "LOR-INC-0001", Severity = IncidentSeverity.High, State = IncidentLifecycleState.Investigating } }
        };

        static HotfixCandidateEvidence ReadyEvidence() => new HotfixCandidateEvidence
        {
            EvidenceRetained = true, HotfixId = "LOR-HF-0001", BaseCandidateId = "v1.18-step05-rc01", BaseArtifactSha256 = A,
            HotfixCandidateId = "v1.19-hotfix-rc01", HotfixArtifactSha256 = B, LinkedIncidentIds = new[] { "LOR-INC-0001" },
            RootCauseReviewed = true, ScopeReviewed = true, ChangedScopeUnits = 3, SourceChangeReviewed = true,
            UnityCompilePassed = true, EditModePassed = true, PlayModePassed = true, DeviceSmokePassed = true,
            SaveMigrationRiskReviewed = true, EconomyRiskReviewed = true, PrivacyRiskReviewed = true, StoreListingImpactReviewed = true,
            VersionCodeReviewed = true, AabEvidenceRetained = true, SigningEvidenceReviewed = true, ManualHotfixReleaseAuthorized = true
        };
    }
}
