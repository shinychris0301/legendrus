using System.Collections.Generic;

namespace LegendsOfRus.IncidentResponse
{
    public static class RolloutControlReviewEvaluator
    {
        public static RolloutControlResult Evaluate(RolloutControlReview review)
        {
            var baseBlockers = new List<string>();
            if (review == null || !review.EvidenceRetained || !review.EvidenceReviewed)
            {
                baseBlockers.Add(IncidentResponseBlockerCodes.RolloutEvidenceNotReviewed);
                return Result(baseBlockers, baseBlockers);
            }

            Require(IncidentResponseValidation.IsRolloutPercent(review.CurrentRolloutPercent), IncidentResponseBlockerCodes.RolloutPercentInvalid, baseBlockers);
            if (review.FirstReleaseOnTrack && (review.CurrentState == RolloutTrackState.StagedActive || review.CurrentState == RolloutTrackState.StagedHalted))
                baseBlockers.Add(IncidentResponseBlockerCodes.FirstReleaseCannotBeStaged);
            if ((review.CurrentState == RolloutTrackState.FullyRolledOut || review.CurrentState == RolloutTrackState.FullReleaseHalted) && review.CurrentRolloutPercent != IncidentResponseRequirements.FullRolloutPercent)
                baseBlockers.Add(IncidentResponseBlockerCodes.RolloutPercentInvalid);
            if ((review.CurrentState == RolloutTrackState.StagedActive || review.CurrentState == RolloutTrackState.StagedHalted) && review.CurrentRolloutPercent >= IncidentResponseRequirements.FullRolloutPercent)
                baseBlockers.Add(IncidentResponseBlockerCodes.RolloutPercentInvalid);
            Require(review.IncidentTriageReviewed, IncidentResponseBlockerCodes.RolloutEvidenceNotReviewed, baseBlockers);
            Require(review.AndroidVitalsReviewed, IncidentResponseBlockerCodes.AndroidVitalsNotReviewed, baseBlockers);

            var action = new List<string>(baseBlockers);
            ValidateRequestedAction(review, action);
            var manual = new List<string>(action);
            if (review.RequestedAction != RolloutControlAction.None)
                Require(review.ManualRolloutActionAuthorized, IncidentResponseBlockerCodes.RolloutActionNotAuthorized, manual);

            return new RolloutControlResult
            {
                ReadyForHumanRolloutControlReview = baseBlockers.Count == 0,
                RequestedActionValid = action.Count == 0,
                MayApplyRequestedActionManually = manual.Count == 0 && review.RequestedAction != RolloutControlAction.None,
                ReviewBlockers = baseBlockers.ToArray(),
                ActionBlockers = manual.ToArray()
            };
        }

        static void ValidateRequestedAction(RolloutControlReview review, List<string> blockers)
        {
            switch (review.RequestedAction)
            {
                case RolloutControlAction.None:
                case RolloutControlAction.Hold:
                case RolloutControlAction.EscalateRollbackReview:
                    return;

                case RolloutControlAction.HaltCurrentRelease:
                    if (review.CurrentState == RolloutTrackState.StagedActive)
                    {
                        if (review.CurrentRolloutPercent >= IncidentResponseRequirements.FullRolloutPercent)
                            blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                        return;
                    }
                    if (review.CurrentState == RolloutTrackState.FullyRolledOut)
                    {
                        if (review.FirstReleaseOnTrack) blockers.Add(IncidentResponseBlockerCodes.FirstReleaseCannotHaltFull);
                        if (!review.PreviousEligibleReleaseAvailable) blockers.Add(IncidentResponseBlockerCodes.PreviousReleaseUnavailable);
                        return;
                    }
                    blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                    return;

                case RolloutControlAction.ResumeCurrentRelease:
                    if (review.CurrentState != RolloutTrackState.StagedHalted && review.CurrentState != RolloutTrackState.FullReleaseHalted)
                        blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                    if (!review.IncidentHealthClear || review.CurrentArtifactHasBlockingIssue)
                        blockers.Add(IncidentResponseBlockerCodes.BlockingIssueNotCleared);
                    if (!IncidentResponseValidation.IsRolloutPercent(review.RequestedRolloutPercent))
                        blockers.Add(IncidentResponseBlockerCodes.RequestedPercentInvalid);
                    return;

                case RolloutControlAction.IncreaseRollout:
                    if (review.CurrentState != RolloutTrackState.StagedActive)
                        blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                    if (!review.IncidentHealthClear || review.CurrentArtifactHasBlockingIssue)
                        blockers.Add(IncidentResponseBlockerCodes.BlockingIssueNotCleared);
                    if (!IncidentResponseValidation.IsRolloutPercent(review.RequestedRolloutPercent) || review.RequestedRolloutPercent <= review.CurrentRolloutPercent)
                        blockers.Add(IncidentResponseBlockerCodes.RequestedPercentInvalid);
                    return;

                case RolloutControlAction.ReplaceWithQualifiedHotfix:
                    if (!review.QualifiedHotfixAvailable)
                        blockers.Add(IncidentResponseBlockerCodes.HotfixNotQualified);
                    if (!review.CurrentArtifactHasBlockingIssue)
                        blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                    return;

                default:
                    blockers.Add(IncidentResponseBlockerCodes.RolloutActionInvalidForState);
                    return;
            }
        }

        static RolloutControlResult Result(List<string> review, List<string> action)
        {
            return new RolloutControlResult
            {
                ReadyForHumanRolloutControlReview = review.Count == 0,
                RequestedActionValid = action.Count == 0,
                MayApplyRequestedActionManually = false,
                ReviewBlockers = review.ToArray(),
                ActionBlockers = action.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
