using LegendsOfRus.LaunchOperations;
using NUnit.Framework;

namespace LegendsOfRus.Tests.LaunchOperations
{
    public sealed class LaunchOperationsPreflightEditModeTests
    {
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        [Test] public void ReadyPlan_IsReadyForHumanReview() => Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(Ready()).ReadyForHumanLaunchOperationsReview, Is.True);
        [Test] public void ReadyPlan_WithManualAuthorization_MayLaunch() => Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(Ready()).MayStartManualProductionLaunch, Is.True);
        [Test] public void MissingLaunchCandidate_BlocksReview() { var p=Ready(); p.LaunchCandidateReadinessVerified=false; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void MissingMonitoringPlan_BlocksReview() { var p=Ready(); p.MonitoringPlanApproved=false; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void MissingIncidentPlan_BlocksReview() { var p=Ready(); p.IncidentResponsePlanApproved=false; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void MissingRollbackPlan_BlocksReview() { var p=Ready(); p.RollbackPlanApproved=false; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void FirstRelease_RequiresHundredPercent() { var p=Ready(); p.RequestedRolloutPercent=20; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void Update_AllowsStagedRollout() { var p=Ready(); p.ReleaseKind=ReleaseKind.ProductionUpdate; p.RequestedRolloutPercent=10; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.True); }
        [Test] public void OutstandingRelease_BlocksReview() { var p=Ready(); p.OutstandingReleasesCleared=false; Assert.That(LaunchOperationsPreflightEvaluator.Evaluate(p).ReadyForHumanLaunchOperationsReview, Is.False); }
        [Test] public void ManualAuthorization_IsSeparateGate() { var p=Ready(); p.ManualLaunchStartAuthorized=false; var r=LaunchOperationsPreflightEvaluator.Evaluate(p); Assert.That(r.ReadyForHumanLaunchOperationsReview && !r.MayStartManualProductionLaunch, Is.True); }

        static LaunchOperationsPlan Ready() => new LaunchOperationsPlan
        {
            CandidateId="v1.18-step05-rc01", ArtifactSha256=HashA, LaunchCandidateReadinessVerified=true,
            ProductionReleaseAuthorized=true, StorePublishingAuthorized=true, MonitoringPlanApproved=true,
            IncidentResponsePlanApproved=true, RollbackPlanApproved=true, SupportCoverageApproved=true,
            ReleaseHistoryReviewed=true, OutstandingReleasesCleared=true, ReleaseKind=ReleaseKind.FirstProductionRelease,
            RequestedRolloutPercent=100, ManualLaunchStartAuthorized=true
        };
    }
}
