using NUnit.Framework;
using LegendsOfRus.LiveOpsExecution;

namespace LegendsOfRus.Tests.LiveOpsExecution
{
    public sealed class LiveOpsExecutionEvaluatorEditModeTests
    {
        static LiveOpsExecutionEvidence X(){return new LiveOpsExecutionEvidence{EvidenceRetained=true,EvidenceReviewed=true,ExecutionId="LOR-LO-EXEC-1",ApprovedReadinessId="LOR-READINESS-1",WindowStartMinute=0,WindowEndMinute=600,PreflightPassed=true,SupportCoverageConfirmed=true,RollbackPlanConfirmed=true};}
        static ContentRotationEvidence R(){return new ContentRotationEvidence{EvidenceRetained=true,EvidenceReviewed=true,RotationPlanId="LOR-LO-ROT-PLAN-1",Entries=new[]{new ContentRotationEntryEvidence{RotationId="LOR-LO-ROT-1",ContentId="A",ContentVersion="1",StartMinute=0,EndMinute=60,RequiredCooldownMinutes=30},new ContentRotationEntryEvidence{RotationId="LOR-LO-ROT-2",ContentId="B",ContentVersion="1",StartMinute=90,EndMinute=150}}};}
        static ExperimentObservationEvidence O(){return new ExperimentObservationEvidence{EvidenceRetained=true,EvidenceReviewed=true,ObservationId="LOR-EXP-OBS-1",ExperimentId="LOR-EXP-1",PrivacyReviewed=true,Windows=new[]{new ExperimentObservationWindow{WindowIndex=0,ControlSamples=500,TreatmentSamples=500,DataQualityPassed=true}}};}
        [Test] public void SourceReviewReady_WithEvidence(){Assert.IsTrue(LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),false,false,false,false,false,false,false).ReadyForHumanReview);}
        [Test] public void SourceActionBlocked(){Assert.IsFalse(LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),false,false,false,false,false,false,false).MayProceedWithManualExecutionReview);}
        [Test] public void AnalyticsBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.ProductionAnalyticsDisabled,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),false,true,true,true,true,true,true).ActionBlockers);}
        [Test] public void RemoteConfigBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.ProductionRemoteConfigDisabled,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,false,true,true,true,true,true).ActionBlockers);}
        [Test] public void ExecutionAuthBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.ExecutionNotAuthorized,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,false,true,true,true,true).ActionBlockers);}
        [Test] public void RotationAuthBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.RotationNotAuthorized,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,true,false,true,true,true).ActionBlockers);}
        [Test] public void ObservationAuthBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.ObservationNotAuthorized,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,true,true,false,true,true).ActionBlockers);}
        [Test] public void TrafficAuthBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.TrafficChangeNotAuthorized,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,true,true,true,false,true).ActionBlockers);}
        [Test] public void ManualReviewBlocked(){Assert.Contains(LiveOpsExecutionBlockerCodes.ManualExecutionReviewNotAuthorized,LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,true,true,true,true,false).ActionBlockers);}
        [Test] public void AllAuthorized_AllowsReview(){Assert.IsTrue(LiveOpsExecutionEvaluator.Evaluate(X(),R(),O(),true,true,true,true,true,true,true).MayProceedWithManualExecutionReview);}
        [Test] public void RotationUnsafe_Revise(){var r=R();r.Entries[1].StartMinute=10;Assert.AreEqual(LiveOpsExecutionRecommendation.ReviseRotation,LiveOpsExecutionEvaluator.Evaluate(X(),r,O(),true,true,true,true,true,true,true).Recommendation);}
        [Test] public void Guardrail_Stop(){var o=O();o.Windows[0].GuardrailBreaches=2;Assert.AreEqual(LiveOpsExecutionRecommendation.StopExperimentReview,LiveOpsExecutionEvaluator.Evaluate(X(),R(),o,true,true,true,true,true,true,true).Recommendation);}
        [Test] public void Ratio_Pause(){var o=O();o.Windows[0].ControlSamples=900;o.Windows[0].TreatmentSamples=100;Assert.AreEqual(LiveOpsExecutionRecommendation.PauseExperimentReview,LiveOpsExecutionEvaluator.Evaluate(X(),R(),o,true,true,true,true,true,true,true).Recommendation);}
        [Test] public void MissingExecution_Insufficient(){Assert.AreEqual(LiveOpsExecutionRecommendation.InsufficientEvidence,LiveOpsExecutionEvaluator.Evaluate(null,R(),O(),true,true,true,true,true,true,true).Recommendation);}
    }
}
