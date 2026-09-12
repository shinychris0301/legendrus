using System.Collections.Generic;

namespace LegendsOfRus.LaunchCandidate
{
    public static class LaunchCandidateReadinessEvaluator
    {
        public static LaunchCandidateReadinessResult Evaluate(LaunchCandidateEvidence evidence)
        {
            var review = new List<string>();
            if (evidence == null)
            {
                review.Add(LaunchCandidateBlockerCodes.CandidateFreezeMissing);
                return Result(review, review, review);
            }

            Require(LaunchCandidateValidation.IsCandidateId(evidence.CandidateId), LaunchCandidateBlockerCodes.CandidateIdInvalid, review);
            Require(LaunchCandidateValidation.IsSha256(evidence.ArtifactSha256), LaunchCandidateBlockerCodes.ArtifactHashInvalid, review);
            Require(LaunchCandidateValidation.IsSha256(evidence.CandidateFreezeManifestSha256), LaunchCandidateBlockerCodes.FreezeHashInvalid, review);
            Require(evidence.CandidateFreezeManifestRetained, LaunchCandidateBlockerCodes.CandidateFreezeMissing, review);
            Require(evidence.CandidateFreezeVerified, LaunchCandidateBlockerCodes.CandidateFreezeInvalid, review);
            Require(evidence.ArtifactMatchesFreeze, LaunchCandidateBlockerCodes.ArtifactFreezeMismatch, review);
            Require(evidence.ClosedTestExecutionEvidenceValid, LaunchCandidateBlockerCodes.ClosedTestEvidenceInvalid, review);
            Require(evidence.FeedbackTriageReady, LaunchCandidateBlockerCodes.FeedbackTriageNotReady, review);
            Require(evidence.ProductionAccessDecisionSatisfied, LaunchCandidateBlockerCodes.DecisionNotGranted, review);
            Require(evidence.StoreReleaseChecklistComplete, LaunchCandidateBlockerCodes.StoreChecklistIncomplete, review);
            Require(evidence.PrivacyAndDataSafetyApproved, LaunchCandidateBlockerCodes.PrivacyDataSafetyIncomplete, review);
            Require(evidence.LegalDocumentsApproved, LaunchCandidateBlockerCodes.LegalDocumentsIncomplete, review);
            Require(evidence.AppContentDeclarationsApproved, LaunchCandidateBlockerCodes.AppContentDeclarationsIncomplete, review);
            Require(evidence.ProductionApplicationIdApproved, LaunchCandidateBlockerCodes.ApplicationIdUnapproved, review);
            Require(evidence.VersioningApproved, LaunchCandidateBlockerCodes.VersioningUnapproved, review);
            Require(evidence.PlayAppSigningConfigured, LaunchCandidateBlockerCodes.PlayAppSigningIncomplete, review);
            Require(evidence.UploadKeyWorkflowReviewed, LaunchCandidateBlockerCodes.UploadKeyWorkflowIncomplete, review);
            Require(evidence.TargetApiLevel >= LaunchCandidateRequirements.MinimumTargetApiLevel, LaunchCandidateBlockerCodes.TargetApiInsufficient, review);
            Require(evidence.AppBundleVerified, LaunchCandidateBlockerCodes.AppBundleUnverified, review);
            Require(evidence.Arm64Verified, LaunchCandidateBlockerCodes.Arm64Unverified, review);
            Require(evidence.PageSize16KbVerified, LaunchCandidateBlockerCodes.PageSize16KbUnverified, review);
            Require(evidence.ReleaseNotesApproved, LaunchCandidateBlockerCodes.ReleaseNotesUnapproved, review);
            Require(evidence.ProductionCountriesRegionsApproved, LaunchCandidateBlockerCodes.CountriesRegionsUnapproved, review);
            Require(evidence.SupportReadinessApproved, LaunchCandidateBlockerCodes.SupportNotReady, review);
            Require(evidence.LaunchMonitoringApproved, LaunchCandidateBlockerCodes.MonitoringNotReady, review);
            Require(evidence.RollbackPlanApproved, LaunchCandidateBlockerCodes.RollbackNotReady, review);
            Require(evidence.ReleaseHistoryReviewed, LaunchCandidateBlockerCodes.ReleaseHistoryNotReviewed, review);
            Require(evidence.OutstandingReleasesCleared, LaunchCandidateBlockerCodes.OutstandingReleaseExists, review);

            if (evidence.ReleaseHistoryReviewed)
            {
                if (evidence.FirstProductionRelease)
                {
                    Require(evidence.RequestedRolloutPercent == LaunchCandidateRequirements.FirstProductionReleaseRolloutPercent, LaunchCandidateBlockerCodes.FirstReleaseRequiresFullRollout, review);
                }
                else
                {
                    bool validPercent = evidence.RequestedRolloutPercent >= LaunchCandidateRequirements.MinimumUpdateRolloutPercent && evidence.RequestedRolloutPercent <= LaunchCandidateRequirements.MaximumRolloutPercent;
                    Require(validPercent, LaunchCandidateBlockerCodes.UpdateRolloutInvalid, review);
                }
            }

            var draft = new List<string>(review);
            var rollout = new List<string>(draft);
            Require(evidence.ProductionReleaseAuthorized, LaunchCandidateBlockerCodes.ProductionReleaseNotAuthorized, rollout);
            Require(evidence.StorePublishingAuthorized, LaunchCandidateBlockerCodes.StorePublishingNotAuthorized, rollout);
            return Result(review, draft, rollout);
        }

        static LaunchCandidateReadinessResult Result(List<string> review, List<string> draft, List<string> rollout)
        {
            return new LaunchCandidateReadinessResult
            {
                ReadyForHumanLaunchReview = review.Count == 0,
                MayCreateProductionReleaseDraft = draft.Count == 0,
                MayStartManualProductionRollout = rollout.Count == 0,
                ReviewBlockers = review.ToArray(),
                DraftBlockers = draft.ToArray(),
                RolloutBlockers = rollout.ToArray()
            };
        }

        static void Require(bool condition, string blocker, List<string> blockers)
        {
            if (!condition) blockers.Add(blocker);
        }
    }
}
