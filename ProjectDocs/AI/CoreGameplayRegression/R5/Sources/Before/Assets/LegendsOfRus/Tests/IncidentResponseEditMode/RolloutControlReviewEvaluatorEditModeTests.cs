using LegendsOfRus.IncidentResponse;
using NUnit.Framework;

namespace LegendsOfRus.Tests.IncidentResponse
{
    public sealed class RolloutControlReviewEvaluatorEditModeTests
    {
        [Test]
        public void UnreviewedEvidence_BlocksReview()
        {
            var r = RolloutControlReviewEvaluator.Evaluate(new RolloutControlReview());
            Assert.That(r.ReadyForHumanRolloutControlReview, Is.False);
        }

        [Test]
        public void StagedActive_CanHaltManually()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 20; e.RequestedAction = RolloutControlAction.HaltCurrentRelease;
            var r = RolloutControlReviewEvaluator.Evaluate(e);
            Assert.That(r.MayApplyRequestedActionManually, Is.True);
        }

        [Test]
        public void FirstFullRelease_CannotUseFullReleaseHalt()
        {
            var e = Ready(); e.FirstReleaseOnTrack = true; e.CurrentState = RolloutTrackState.FullyRolledOut; e.CurrentRolloutPercent = 100; e.RequestedAction = RolloutControlAction.HaltCurrentRelease; e.PreviousEligibleReleaseAvailable = false;
            var r = RolloutControlReviewEvaluator.Evaluate(e);
            CollectionAssert.Contains(r.ActionBlockers, IncidentResponseBlockerCodes.FirstReleaseCannotHaltFull);
        }

        [Test]
        public void FullUpdateHalt_RequiresPreviousEligibleRelease()
        {
            var e = Ready(); e.FirstReleaseOnTrack = false; e.CurrentState = RolloutTrackState.FullyRolledOut; e.CurrentRolloutPercent = 100; e.RequestedAction = RolloutControlAction.HaltCurrentRelease; e.PreviousEligibleReleaseAvailable = false;
            var r = RolloutControlReviewEvaluator.Evaluate(e);
            CollectionAssert.Contains(r.ActionBlockers, IncidentResponseBlockerCodes.PreviousReleaseUnavailable);
        }

        [Test]
        public void FullUpdateHalt_AllowsEligiblePreviousRelease()
        {
            var e = Ready(); e.FirstReleaseOnTrack = false; e.CurrentState = RolloutTrackState.FullyRolledOut; e.CurrentRolloutPercent = 100; e.RequestedAction = RolloutControlAction.HaltCurrentRelease; e.PreviousEligibleReleaseAvailable = true;
            Assert.That(RolloutControlReviewEvaluator.Evaluate(e).MayApplyRequestedActionManually, Is.True);
        }

        [Test]
        public void ResumeRequiresHaltedState()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.RequestedAction = RolloutControlAction.ResumeCurrentRelease; e.RequestedRolloutPercent = 25;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ActionBlockers, IncidentResponseBlockerCodes.RolloutActionInvalidForState);
        }

        [Test]
        public void ResumeRequiresBlockingIssueCleared()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedHalted; e.RequestedAction = RolloutControlAction.ResumeCurrentRelease; e.RequestedRolloutPercent = 25; e.CurrentArtifactHasBlockingIssue = true; e.IncidentHealthClear = false;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ActionBlockers, IncidentResponseBlockerCodes.BlockingIssueNotCleared);
        }

        [Test]
        public void ResumeHaltedHealthyRelease_IsValid()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedHalted; e.RequestedAction = RolloutControlAction.ResumeCurrentRelease; e.RequestedRolloutPercent = 25;
            Assert.That(RolloutControlReviewEvaluator.Evaluate(e).MayApplyRequestedActionManually, Is.True);
        }

        [Test]
        public void IncreaseRequiresLargerPercentage()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 25; e.RequestedAction = RolloutControlAction.IncreaseRollout; e.RequestedRolloutPercent = 25;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ActionBlockers, IncidentResponseBlockerCodes.RequestedPercentInvalid);
        }

        [Test]
        public void IncreaseHealthyStagedRollout_IsValid()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 25; e.RequestedAction = RolloutControlAction.IncreaseRollout; e.RequestedRolloutPercent = 50;
            Assert.That(RolloutControlReviewEvaluator.Evaluate(e).MayApplyRequestedActionManually, Is.True);
        }

        [Test]
        public void IncreaseBlocksWhenCurrentArtifactHasBlockingIssue()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 25; e.RequestedAction = RolloutControlAction.IncreaseRollout; e.RequestedRolloutPercent = 50; e.CurrentArtifactHasBlockingIssue = true;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ActionBlockers, IncidentResponseBlockerCodes.BlockingIssueNotCleared);
        }

        [Test]
        public void ReplaceRequiresQualifiedHotfix()
        {
            var e = Ready(); e.CurrentArtifactHasBlockingIssue = true; e.QualifiedHotfixAvailable = false; e.RequestedAction = RolloutControlAction.ReplaceWithQualifiedHotfix;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ActionBlockers, IncidentResponseBlockerCodes.HotfixNotQualified);
        }

        [Test]
        public void ReplaceWithQualifiedHotfix_IsValid()
        {
            var e = Ready(); e.CurrentArtifactHasBlockingIssue = true; e.QualifiedHotfixAvailable = true; e.RequestedAction = RolloutControlAction.ReplaceWithQualifiedHotfix;
            Assert.That(RolloutControlReviewEvaluator.Evaluate(e).MayApplyRequestedActionManually, Is.True);
        }

        [Test]
        public void ManualAuthorization_RemainsSeparate()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 20; e.RequestedAction = RolloutControlAction.HaltCurrentRelease; e.ManualRolloutActionAuthorized = false;
            var r = RolloutControlReviewEvaluator.Evaluate(e);
            Assert.That(r.RequestedActionValid, Is.True);
            Assert.That(r.MayApplyRequestedActionManually, Is.False);
            CollectionAssert.Contains(r.ActionBlockers, IncidentResponseBlockerCodes.RolloutActionNotAuthorized);
        }


        [Test]
        public void FirstRelease_CannotBeStaged()
        {
            var e = Ready(); e.FirstReleaseOnTrack = true; e.CurrentState = RolloutTrackState.StagedActive; e.CurrentRolloutPercent = 20;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ReviewBlockers, IncidentResponseBlockerCodes.FirstReleaseCannotBeStaged);
        }

        [Test]
        public void FullyRolledOutState_RequiresHundredPercent()
        {
            var e = Ready(); e.CurrentState = RolloutTrackState.FullyRolledOut; e.CurrentRolloutPercent = 75;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ReviewBlockers, IncidentResponseBlockerCodes.RolloutPercentInvalid);
        }

        [Test]
        public void AndroidVitalsReview_IsRequired()
        {
            var e = Ready(); e.AndroidVitalsReviewed = false;
            CollectionAssert.Contains(RolloutControlReviewEvaluator.Evaluate(e).ReviewBlockers, IncidentResponseBlockerCodes.AndroidVitalsNotReviewed);
        }

        static RolloutControlReview Ready() => new RolloutControlReview
        {
            EvidenceRetained = true, EvidenceReviewed = true, FirstReleaseOnTrack = false,
            CurrentState = RolloutTrackState.StagedActive, CurrentRolloutPercent = 20, PreviousEligibleReleaseAvailable = true,
            CurrentArtifactHasBlockingIssue = false, IncidentTriageReviewed = true, IncidentHealthClear = true,
            AndroidVitalsReviewed = true, QualifiedHotfixAvailable = true, RequestedAction = RolloutControlAction.Hold,
            RequestedRolloutPercent = 20, ManualRolloutActionAuthorized = true
        };
    }
}
