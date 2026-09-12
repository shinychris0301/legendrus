using System;
using LegendsOfRus.LaunchOperations;
using NUnit.Framework;

namespace LegendsOfRus.Tests.LaunchOperations
{
    public sealed class AndroidVitalsEvaluatorEditModeTests
    {
        [Test] public void HealthyEvidence_IsHealthy() => Assert.That(Eval(Healthy(), Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Healthy));
        [Test] public void HealthyEvidence_RequiresManualRolloutAuthorization() => Assert.That(Eval(Healthy(), Clean(), false).MayContinueOrIncreaseRollout, Is.False);
        [Test] public void HealthyEvidence_WithAuthorization_MayContinue() => Assert.That(Eval(Healthy(), Clean(), true).MayContinueOrIncreaseRollout, Is.True);
        [Test] public void CrashAtThreshold_IsCritical() { var v=Healthy(); v.UserPerceivedCrashRatePercent=1.09; Assert.That(Eval(v,Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Critical)); }
        [Test] public void AnrAtThreshold_IsCritical() { var v=Healthy(); v.UserPerceivedAnrRatePercent=0.47; Assert.That(Eval(v,Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Critical)); }
        [Test] public void DeviceCrashAtThreshold_IsCritical() { var v=Healthy(); v.MaxPhoneModelCrashRatePercent=8; Assert.That(Eval(v,Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Critical)); }
        [Test] public void DeviceAnrAtThreshold_IsCritical() { var v=Healthy(); v.MaxPhoneModelAnrRatePercent=8; Assert.That(Eval(v,Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Critical)); }
        [Test] public void WakeLockAtThreshold_IsUnhealthy() { var v=Healthy(); v.ExcessivePartialWakeLockRatePercent=5; Assert.That(Eval(v,Clean()).Classification, Is.EqualTo(LaunchHealthClassification.Unhealthy)); }
        [Test] public void CriticalIncident_IsCritical() { var i=Clean(); i.OpenCritical=1; Assert.That(Eval(Healthy(),i).Classification, Is.EqualTo(LaunchHealthClassification.Critical)); }
        [Test] public void StagedUpdateCritical_RecommendsHalt() { var v=Healthy(); v.UserPerceivedCrashRatePercent=2; Assert.That(Eval(v,Clean(),true,ReleaseKind.ProductionUpdate,25).Recommendation, Is.EqualTo(LaunchRolloutRecommendation.HaltStagedRollout)); }

        static LaunchHealthEvaluation Eval(AndroidVitalsSnapshot v, LaunchIncidentSummary i, bool auth=false, ReleaseKind kind=ReleaseKind.FirstProductionRelease, int percent=100) => AndroidVitalsEvaluator.Evaluate(v,i,kind,percent,auth);
        static AndroidVitalsSnapshot Healthy() => new AndroidVitalsSnapshot { CandidateId="v1.18-step05-rc01", ArtifactSha256=new string('a',64), EvidenceRetained=true, EvidenceReviewed=true, WindowDays=1, UserPerceivedCrashRatePercent=0.1, UserPerceivedAnrRatePercent=0.05, MaxPhoneModelCrashRatePercent=1, MaxPhoneModelAnrRatePercent=1, ExcessivePartialWakeLockRatePercent=0.2, ExcessiveBatteryUsageRatePercent=0.1 };
        static LaunchIncidentSummary Clean() => new LaunchIncidentSummary { RegisterRetained=true, RegisterReviewed=true };
    }
}
