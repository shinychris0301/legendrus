using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestRollout
{
    public static class ClosedTestUploadReadinessEvaluator
    {
        public static ClosedTestUploadReadinessResult Evaluate(ClosedTestUploadReadinessEvidence evidence)
        {
            var prepare = new List<string>();
            Require(evidence.CandidateFreezeManifestVerified, ClosedTestRolloutBlockerCodes.FreezeManifestMissing, prepare);
            Require(evidence.ArtifactHashMatchesFreeze, ClosedTestRolloutBlockerCodes.ArtifactHashMismatch, prepare);
            Require(evidence.PlayAppSetupComplete, ClosedTestRolloutBlockerCodes.AppSetupIncomplete, prepare);
            Require(evidence.TestingTrackReleasePermissionApproved, ClosedTestRolloutBlockerCodes.TestingPermissionMissing, prepare);
            Require(evidence.ClosedTestTrackConfigured, ClosedTestRolloutBlockerCodes.ClosedTrackMissing, prepare);
            Require(evidence.TesterAccessConfigured, ClosedTestRolloutBlockerCodes.TesterAccessMissing, prepare);
            Require(evidence.CountryAvailabilityReviewed, ClosedTestRolloutBlockerCodes.CountryReviewMissing, prepare);
            Require(evidence.ReleaseNotesApproved, ClosedTestRolloutBlockerCodes.ReleaseNotesMissing, prepare);
            Require(evidence.NoOutstandingRelease, ClosedTestRolloutBlockerCodes.OutstandingReleasePresent, prepare);

            var rollout = new List<string>(prepare);
            Require(evidence.ClosedTestUploadAuthorized, ClosedTestRolloutBlockerCodes.UploadNotAuthorized, rollout);
            Require(evidence.ClosedTestPublishAuthorized, ClosedTestRolloutBlockerCodes.ClosedTestPublishNotAuthorized, rollout);

            return new ClosedTestUploadReadinessResult
            {
                MayPrepareClosedTestRelease = prepare.Count == 0,
                MayRolloutClosedTestRelease = rollout.Count == 0,
                PrepareBlockers = prepare.ToArray(),
                RolloutBlockers = rollout.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
