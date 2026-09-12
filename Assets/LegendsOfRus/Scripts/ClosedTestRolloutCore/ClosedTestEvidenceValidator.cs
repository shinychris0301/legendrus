using System;
using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestRollout
{
    public static class ClosedTestEvidenceValidator
    {
        public static ClosedTestEvidenceValidationResult ValidateUpload(ClosedTestUploadEvidence evidence, string expectedArtifactSha256)
        {
            var issues = new List<string>();
            if (evidence == null) return Invalid(ClosedTestRolloutBlockerCodes.UploadEvidenceInvalid);
            if (evidence.SchemaVersion != ClosedTestRolloutRequirements.SchemaVersion) issues.Add("CTR_UPLOAD_SCHEMA_UNSUPPORTED");
            if (!IsCandidateId(evidence.CandidateId)) issues.Add("CTR_UPLOAD_CANDIDATE_ID_INVALID");
            if (!IsSha256(evidence.FreezeManifestSha256)) issues.Add("CTR_UPLOAD_FREEZE_MANIFEST_SHA256_INVALID");
            if (!IsSha256(evidence.ArtifactSha256)) issues.Add("CTR_UPLOAD_ARTIFACT_SHA256_INVALID");
            if (IsSha256(expectedArtifactSha256) && !string.Equals(evidence.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase)) issues.Add(ClosedTestRolloutBlockerCodes.ArtifactHashMismatch);
            if (string.IsNullOrWhiteSpace(evidence.VersionName)) issues.Add("CTR_UPLOAD_VERSION_NAME_MISSING");
            if (evidence.VersionCode <= 0) issues.Add("CTR_UPLOAD_VERSION_CODE_INVALID");
            if (!evidence.ArtifactAcceptedByPlay) issues.Add("CTR_UPLOAD_ARTIFACT_NOT_ACCEPTED_BY_PLAY");
            if (!evidence.ClosedTestTrackConfigured) issues.Add(ClosedTestRolloutBlockerCodes.ClosedTrackMissing);
            if (!evidence.TesterAccessConfigured) issues.Add(ClosedTestRolloutBlockerCodes.TesterAccessMissing);
            if (!evidence.CountryAvailabilityReviewed) issues.Add(ClosedTestRolloutBlockerCodes.CountryReviewMissing);
            if (!evidence.ReleaseNotesRetained) issues.Add(ClosedTestRolloutBlockerCodes.ReleaseNotesMissing);
            if (evidence.ReleaseStatus == ClosedTestReleaseStatus.None || evidence.ReleaseStatus == ClosedTestReleaseStatus.Rejected) issues.Add("CTR_UPLOAD_RELEASE_STATUS_INVALID");
            return new ClosedTestEvidenceValidationResult { Valid = issues.Count == 0, Issues = issues.ToArray() };
        }

        public static ClosedTestEvidenceValidationResult ValidateHealth(ClosedTestHealthEvidence evidence, string expectedArtifactSha256)
        {
            var issues = new List<string>();
            if (evidence == null) return Invalid(ClosedTestRolloutBlockerCodes.HealthEvidenceInvalid);
            if (evidence.SchemaVersion != ClosedTestRolloutRequirements.SchemaVersion) issues.Add("CTR_HEALTH_SCHEMA_UNSUPPORTED");
            if (!IsCandidateId(evidence.CandidateId)) issues.Add("CTR_HEALTH_CANDIDATE_ID_INVALID");
            if (!IsSha256(evidence.ArtifactSha256)) issues.Add("CTR_HEALTH_ARTIFACT_SHA256_INVALID");
            if (IsSha256(expectedArtifactSha256) && !string.Equals(evidence.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase)) issues.Add(ClosedTestRolloutBlockerCodes.ArtifactHashMismatch);
            if (evidence.ObservationDays < ClosedTestRolloutRequirements.MinimumClosedTestObservationDays) issues.Add("CTR_HEALTH_OBSERVATION_TOO_SHORT");
            if (evidence.OptedInTesterCount < ClosedTestRolloutRequirements.MinimumClosedTestTesterCount) issues.Add("CTR_HEALTH_NO_OPTED_IN_TESTERS");
            if (evidence.ContinuousOptedInTesterCount < 0 || evidence.ContinuousTestDays < 0) issues.Add("CTR_HEALTH_CONTINUITY_INVALID");
            if (evidence.BlockingCrashCount > 0) issues.Add(ClosedTestRolloutBlockerCodes.BlockingCrash);
            if (evidence.BlockingAnrCount > 0) issues.Add(ClosedTestRolloutBlockerCodes.BlockingAnr);
            if (evidence.OpenCriticalBugCount > 0) issues.Add(ClosedTestRolloutBlockerCodes.CriticalBug);
            if (evidence.OpenBlockingFeedbackCount > 0) issues.Add(ClosedTestRolloutBlockerCodes.BlockingFeedback);
            if (!evidence.FeedbackReviewed || !evidence.CoreLoopFeedbackReviewed) issues.Add(ClosedTestRolloutBlockerCodes.FeedbackNotReviewed);
            return new ClosedTestEvidenceValidationResult { Valid = issues.Count == 0, Issues = issues.ToArray() };
        }

        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool hex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }

        public static bool IsCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(ClosedTestRolloutRequirements.CandidatePrefix, StringComparison.Ordinal)) return false;
            string suffix = value.Substring(ClosedTestRolloutRequirements.CandidatePrefix.Length);
            return suffix.Length == 2 && char.IsDigit(suffix[0]) && char.IsDigit(suffix[1]) && suffix != "00";
        }

        static ClosedTestEvidenceValidationResult Invalid(string issue)
        {
            return new ClosedTestEvidenceValidationResult { Valid = false, Issues = new[] { issue } };
        }
    }
}
