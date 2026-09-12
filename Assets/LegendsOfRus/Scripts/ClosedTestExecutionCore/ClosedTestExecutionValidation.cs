using System;
using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestExecution
{
    public static class ClosedTestExecutionValidation
    {
        public static ClosedTestExecutionValidationResult Validate(ClosedTestExecutionEvidence evidence, string expectedArtifactSha256)
        {
            var blockers = new List<string>();
            if (evidence == null)
                return new ClosedTestExecutionValidationResult { Valid = false, Blockers = new[] { ClosedTestExecutionBlockerCodes.ExecutionEvidenceMissing } };

            if (evidence.SchemaVersion != ClosedTestExecutionRequirements.SchemaVersion) blockers.Add("CTE_EXECUTION_SCHEMA_UNSUPPORTED");
            if (!IsCandidateId(evidence.CandidateId)) blockers.Add("CTE_CANDIDATE_ID_INVALID");
            if (!IsSha256(evidence.ArtifactSha256)) blockers.Add("CTE_ARTIFACT_SHA256_INVALID");
            if (IsSha256(expectedArtifactSha256) && !string.Equals(evidence.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase)) blockers.Add(ClosedTestExecutionBlockerCodes.ArtifactHashMismatch);
            if (!evidence.ClosedTestReleaseAvailableToTesters) blockers.Add(ClosedTestExecutionBlockerCodes.ReleaseNotAvailable);
            if (evidence.ObservationDays < ClosedTestExecutionRequirements.MinimumExecutionObservationDays) blockers.Add(ClosedTestExecutionBlockerCodes.ObservationTooShort);
            if (!evidence.HealthEvidenceValid) blockers.Add("CTE_HEALTH_EVIDENCE_INVALID");
            if (!evidence.DeviceEvidenceValid) blockers.Add("CTE_DEVICE_EVIDENCE_INVALID");
            if (!evidence.FeedbackEvidenceRetained) blockers.Add(ClosedTestExecutionBlockerCodes.FeedbackEvidenceMissing);
            if (!evidence.FeedbackTriageComplete) blockers.Add(ClosedTestExecutionBlockerCodes.FeedbackNotFullyTriaged);
            if (evidence.BlockingCrashOrAnrOpen) blockers.Add("CTE_BLOCKING_CRASH_OR_ANR_OPEN");
            if (evidence.BlockingVisualOrCoreLoopIssueOpen) blockers.Add("CTE_BLOCKING_VISUAL_OR_CORE_LOOP_ISSUE_OPEN");
            if (evidence.OptedInTesterCount < 0 || evidence.ContinuousOptedInTesterCount < 0 || evidence.ContinuousTestDays < 0) blockers.Add("CTE_TESTER_COUNTS_INVALID");

            return new ClosedTestExecutionValidationResult { Valid = blockers.Count == 0, Blockers = blockers.ToArray() };
        }

        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) return false;
            }
            return true;
        }

        public static bool IsCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(ClosedTestExecutionRequirements.CandidatePrefix, StringComparison.Ordinal)) return false;
            string suffix = value.Substring(ClosedTestExecutionRequirements.CandidatePrefix.Length);
            return suffix.Length == 2 && char.IsDigit(suffix[0]) && char.IsDigit(suffix[1]) && suffix != "00";
        }
    }
}
