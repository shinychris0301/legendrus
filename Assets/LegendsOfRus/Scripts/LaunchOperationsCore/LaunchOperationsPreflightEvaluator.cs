using System.Collections.Generic;

namespace LegendsOfRus.LaunchOperations
{
    public static class LaunchOperationsPreflightEvaluator
    {
        public static LaunchOperationsPreflightResult Evaluate(LaunchOperationsPlan plan)
        {
            var review = new List<string>();
            if (plan == null)
            {
                review.Add(LaunchOperationsBlockerCodes.LaunchCandidateNotReady);
                return Result(review, review);
            }

            Require(LaunchOperationsValidation.IsCandidateId(plan.CandidateId), LaunchOperationsBlockerCodes.CandidateIdInvalid, review);
            Require(LaunchOperationsValidation.IsSha256(plan.ArtifactSha256), LaunchOperationsBlockerCodes.ArtifactHashInvalid, review);
            Require(plan.LaunchCandidateReadinessVerified, LaunchOperationsBlockerCodes.LaunchCandidateNotReady, review);
            Require(plan.ProductionReleaseAuthorized, LaunchOperationsBlockerCodes.ProductionReleaseNotAuthorized, review);
            Require(plan.StorePublishingAuthorized, LaunchOperationsBlockerCodes.StorePublishingNotAuthorized, review);
            Require(plan.MonitoringPlanApproved, LaunchOperationsBlockerCodes.MonitoringPlanNotApproved, review);
            Require(plan.IncidentResponsePlanApproved, LaunchOperationsBlockerCodes.IncidentResponsePlanNotApproved, review);
            Require(plan.RollbackPlanApproved, LaunchOperationsBlockerCodes.RollbackPlanNotApproved, review);
            Require(plan.SupportCoverageApproved, LaunchOperationsBlockerCodes.SupportCoverageNotApproved, review);
            Require(plan.ReleaseHistoryReviewed, LaunchOperationsBlockerCodes.ReleaseHistoryNotReviewed, review);
            Require(plan.OutstandingReleasesCleared, LaunchOperationsBlockerCodes.OutstandingReleaseExists, review);

            if (plan.ReleaseHistoryReviewed)
            {
                if (plan.ReleaseKind == ReleaseKind.FirstProductionRelease)
                    Require(plan.RequestedRolloutPercent == LaunchOperationsRequirements.FirstProductionReleaseRolloutPercent, LaunchOperationsBlockerCodes.FirstReleaseRequiresFullRollout, review);
                else
                    Require(plan.RequestedRolloutPercent >= LaunchOperationsRequirements.MinimumUpdateRolloutPercent && plan.RequestedRolloutPercent <= LaunchOperationsRequirements.MaximumRolloutPercent, LaunchOperationsBlockerCodes.UpdateRolloutInvalid, review);
            }

            var launch = new List<string>(review);
            Require(plan.ManualLaunchStartAuthorized, LaunchOperationsBlockerCodes.LaunchStartNotAuthorized, launch);
            return Result(review, launch);
        }

        static LaunchOperationsPreflightResult Result(List<string> review, List<string> launch)
        {
            return new LaunchOperationsPreflightResult
            {
                ReadyForHumanLaunchOperationsReview = review.Count == 0,
                MayStartManualProductionLaunch = launch.Count == 0,
                ReviewBlockers = review.ToArray(),
                LaunchBlockers = launch.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
