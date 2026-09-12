using System.Collections.Generic;

namespace LegendsOfRus.IncidentResponse
{
    public static class HotfixQualificationEvaluator
    {
        public static HotfixQualificationResult Evaluate(HotfixCandidateEvidence evidence, IncidentRegisterEvidence incidentRegister)
        {
            var review = new List<string>();
            if (evidence == null || !evidence.EvidenceRetained)
            {
                review.Add(IncidentResponseBlockerCodes.HotfixEvidenceMissing);
                return Result(review, review, false);
            }

            Require(incidentRegister != null && incidentRegister.EvidenceRetained, IncidentResponseBlockerCodes.RegisterMissing, review);
            Require(incidentRegister != null && incidentRegister.EvidenceReviewed, IncidentResponseBlockerCodes.RegisterNotReviewed, review);
            if (incidentRegister != null && incidentRegister.EvidenceRetained)
            {
                IncidentTriageResult triage = IncidentTriageEvaluator.Evaluate(incidentRegister);
                Require(triage.RegisterStructurallyValid, IncidentResponseBlockerCodes.LinkedIncidentInvalid, review);
            }

            Require(IncidentResponseValidation.IsHotfixId(evidence.HotfixId), IncidentResponseBlockerCodes.HotfixIdInvalid, review);
            Require(IncidentResponseValidation.IsBaseCandidateId(evidence.BaseCandidateId), IncidentResponseBlockerCodes.HotfixCandidateIdInvalid, review);
            Require(IncidentResponseValidation.IsSha256(evidence.BaseArtifactSha256), IncidentResponseBlockerCodes.BaseArtifactHashInvalid, review);
            Require(IncidentResponseValidation.IsHotfixCandidateId(evidence.HotfixCandidateId), IncidentResponseBlockerCodes.HotfixCandidateIdInvalid, review);
            Require(IncidentResponseValidation.IsSha256(evidence.HotfixArtifactSha256), IncidentResponseBlockerCodes.HotfixArtifactHashInvalid, review);
            if (IncidentResponseValidation.IsSha256(evidence.BaseArtifactSha256) && IncidentResponseValidation.IsSha256(evidence.HotfixArtifactSha256))
                Require(!string.Equals(evidence.BaseArtifactSha256, evidence.HotfixArtifactSha256, System.StringComparison.OrdinalIgnoreCase), IncidentResponseBlockerCodes.HotfixArtifactUnchanged, review);

            ValidateLinkedIncidents(evidence.LinkedIncidentIds, incidentRegister, review);
            Require(evidence.RootCauseReviewed, IncidentResponseBlockerCodes.RootCauseNotReviewed, review);
            Require(evidence.ScopeReviewed, IncidentResponseBlockerCodes.ScopeNotReviewed, review);
            Require(evidence.ChangedScopeUnits >= 1 && evidence.ChangedScopeUnits <= IncidentResponseRequirements.MaximumHotfixScopeUnits, IncidentResponseBlockerCodes.ScopeTooLarge, review);
            Require(evidence.SourceChangeReviewed, IncidentResponseBlockerCodes.SourceChangeNotReviewed, review);
            Require(evidence.UnityCompilePassed, IncidentResponseBlockerCodes.CompileNotVerified, review);
            Require(evidence.EditModePassed, IncidentResponseBlockerCodes.EditModeNotVerified, review);
            Require(evidence.PlayModePassed, IncidentResponseBlockerCodes.PlayModeNotVerified, review);
            Require(evidence.DeviceSmokePassed, IncidentResponseBlockerCodes.DeviceSmokeNotVerified, review);
            Require(evidence.SaveMigrationRiskReviewed, IncidentResponseBlockerCodes.SaveRiskNotReviewed, review);
            Require(evidence.EconomyRiskReviewed, IncidentResponseBlockerCodes.EconomyRiskNotReviewed, review);
            Require(evidence.PrivacyRiskReviewed, IncidentResponseBlockerCodes.PrivacyRiskNotReviewed, review);
            Require(evidence.StoreListingImpactReviewed, IncidentResponseBlockerCodes.StoreImpactNotReviewed, review);
            Require(evidence.VersionCodeReviewed, IncidentResponseBlockerCodes.VersionCodeNotReviewed, review);
            Require(evidence.AabEvidenceRetained, IncidentResponseBlockerCodes.AabEvidenceMissing, review);
            Require(evidence.SigningEvidenceReviewed, IncidentResponseBlockerCodes.SigningEvidenceNotReviewed, review);

            var release = new List<string>(review);
            Require(evidence.ManualHotfixReleaseAuthorized, IncidentResponseBlockerCodes.HotfixReleaseNotAuthorized, release);
            bool qualified = review.Count == 0;
            return Result(review, release, qualified);
        }

        static void ValidateLinkedIncidents(string[] linkedIds, IncidentRegisterEvidence register, List<string> blockers)
        {
            if (linkedIds == null || linkedIds.Length < 1 || linkedIds.Length > IncidentResponseRequirements.MaximumLinkedIncidents)
            {
                blockers.Add(IncidentResponseBlockerCodes.LinkedIncidentMissing);
                return;
            }
            var registerIds = new HashSet<string>();
            if (register != null && register.Incidents != null)
                for (int i = 0; i < register.Incidents.Length; i++)
                    if (register.Incidents[i] != null && IncidentResponseValidation.IsIncidentId(register.Incidents[i].IncidentId))
                        registerIds.Add(register.Incidents[i].IncidentId);

            var seen = new HashSet<string>();
            for (int i = 0; i < linkedIds.Length; i++)
            {
                string id = linkedIds[i];
                if (!IncidentResponseValidation.IsIncidentId(id) || !seen.Add(id) || !registerIds.Contains(id))
                    blockers.Add(IncidentResponseBlockerCodes.LinkedIncidentInvalid);
            }
        }

        static HotfixQualificationResult Result(List<string> review, List<string> release, bool qualified)
        {
            return new HotfixQualificationResult
            {
                ReadyForHumanHotfixReview = review.Count == 0,
                QualifiedForManualReleasePreparation = qualified,
                MayProceedWithAuthorizedManualHotfixRelease = release.Count == 0,
                ReviewBlockers = review.ToArray(),
                ReleaseBlockers = release.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
