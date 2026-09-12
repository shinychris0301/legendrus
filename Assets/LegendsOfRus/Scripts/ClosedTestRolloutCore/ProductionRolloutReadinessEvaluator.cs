using System.Collections.Generic;

namespace LegendsOfRus.ClosedTestRollout
{
    public static class ProductionRolloutReadinessEvaluator
    {
        public static ProductionRolloutReadinessResult Evaluate(ProductionRolloutReadinessEvidence evidence)
        {
            var access = new List<string>();
            Require(evidence.ClosedTestUploadEvidenceValid, ClosedTestRolloutBlockerCodes.UploadEvidenceInvalid, access);
            Require(evidence.ClosedTestHealthEvidenceValid, ClosedTestRolloutBlockerCodes.HealthEvidenceInvalid, access);
            Require(evidence.ClosedTestReleaseAvailableToTesters, ClosedTestRolloutBlockerCodes.ReleaseNotAvailableToTesters, access);
            Require(evidence.ClosedTestGoNoGoAuthorized, ClosedTestRolloutBlockerCodes.GoNoGoNotAuthorized, access);
            Require(evidence.DeveloperAccountTypeReviewed, ClosedTestRolloutBlockerCodes.AccountTypeNotReviewed, access);

            if (evidence.DeveloperAccountTypeReviewed && evidence.PersonalAccountCreatedAfter2023November13)
            {
                Require(evidence.ContinuousOptedInTesterCount >= ClosedTestRolloutRequirements.PersonalAccountProductionAccessTesterCount,
                    ClosedTestRolloutBlockerCodes.PersonalTesterCountInsufficient, access);
                Require(evidence.ContinuousTestDays >= ClosedTestRolloutRequirements.PersonalAccountProductionAccessContinuousDays,
                    ClosedTestRolloutBlockerCodes.PersonalTestDurationInsufficient, access);
            }

            var rollout = new List<string>(access);
            Require(evidence.ProductionAccessGranted, ClosedTestRolloutBlockerCodes.ProductionAccessMissing, rollout);
            Require(evidence.StoreReleaseChecklistApproved, ClosedTestRolloutBlockerCodes.StoreChecklistIncomplete, rollout);
            Require(evidence.PrivacyDataSafetyApproved, ClosedTestRolloutBlockerCodes.PrivacyDataSafetyIncomplete, rollout);
            bool declarations = evidence.ContentRatingApproved && evidence.TargetAudienceApproved && evidence.AdsDeclarationApproved && evidence.AppAccessDeclarationApproved;
            Require(declarations, ClosedTestRolloutBlockerCodes.PlayDeclarationsIncomplete, rollout);
            Require(evidence.RollbackPlanReviewed, ClosedTestRolloutBlockerCodes.RollbackPlanMissing, rollout);
            Require(evidence.MonitoringPlanReviewed, ClosedTestRolloutBlockerCodes.MonitoringPlanMissing, rollout);
            Require(evidence.ManagedPublishingReviewed, ClosedTestRolloutBlockerCodes.ManagedPublishingNotReviewed, rollout);
            Require(evidence.NoOutstandingRelease, ClosedTestRolloutBlockerCodes.OutstandingReleasePresent, rollout);

            if (evidence.IsFirstProductionRelease)
            {
                Require(evidence.RequestedRolloutPercent == ClosedTestRolloutRequirements.FirstProductionReleasePercent,
                    ClosedTestRolloutBlockerCodes.FirstReleaseStagedRolloutUnsupported, rollout);
            }
            else
            {
                Require(evidence.RequestedRolloutPercent >= ClosedTestRolloutRequirements.MinimumUpdateRolloutPercent &&
                        evidence.RequestedRolloutPercent <= ClosedTestRolloutRequirements.MaximumRolloutPercent,
                    ClosedTestRolloutBlockerCodes.RolloutPercentInvalid, rollout);
            }

            Require(evidence.ProductionRolloutAuthorized, ClosedTestRolloutBlockerCodes.ProductionRolloutNotAuthorized, rollout);
            Require(evidence.StorePublishingAuthorized, ClosedTestRolloutBlockerCodes.StorePublishingNotAuthorized, rollout);

            return new ProductionRolloutReadinessResult
            {
                MayApplyForProductionAccess = access.Count == 0,
                MayRolloutProduction = rollout.Count == 0,
                UsesStagedRollout = rollout.Count == 0 && !evidence.IsFirstProductionRelease && evidence.RequestedRolloutPercent < 100,
                ProductionAccessBlockers = access.ToArray(),
                ProductionRolloutBlockers = rollout.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
