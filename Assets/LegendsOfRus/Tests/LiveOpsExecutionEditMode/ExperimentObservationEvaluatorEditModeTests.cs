using NUnit.Framework;
using LegendsOfRus.LiveOpsExecution;

namespace LegendsOfRus.Tests.LiveOpsExecution
{
    public sealed class ExperimentObservationEvaluatorEditModeTests
    {
        static ExperimentObservationEvidence Good(){return new ExperimentObservationEvidence{EvidenceRetained=true,EvidenceReviewed=true,ObservationId="LOR-EXP-OBS-1",ExperimentId="LOR-EXP-1",PrivacyReviewed=true,Windows=new[]{new ExperimentObservationWindow{WindowIndex=0,ControlSamples=500,TreatmentSamples=500,DataQualityPassed=true}}};}
        [Test] public void Good_Structural(){Assert.IsTrue(ExperimentObservationEvaluator.Evaluate(Good()).StructurallyValid);}
        [Test] public void Good_Sample(){Assert.IsTrue(ExperimentObservationEvaluator.Evaluate(Good()).SampleSufficient);}
        [Test] public void Good_Ratio(){Assert.IsTrue(ExperimentObservationEvaluator.Evaluate(Good()).SampleRatioHealthy);}
        [Test] public void Good_Data(){Assert.IsTrue(ExperimentObservationEvaluator.Evaluate(Good()).DataQualityHealthy);}
        [Test] public void Good_Guardrails(){Assert.IsTrue(ExperimentObservationEvaluator.Evaluate(Good()).GuardrailsHealthy);}
        [Test] public void Good_Continue(){Assert.AreEqual(ExperimentObservationRecommendation.ContinueObservation,ExperimentObservationEvaluator.Evaluate(Good()).Recommendation);}
        [Test] public void Complete_Conclude(){var e=Good();e.ExperimentWindowComplete=true;Assert.AreEqual(ExperimentObservationRecommendation.ConcludeForHumanReview,ExperimentObservationEvaluator.Evaluate(e).Recommendation);}
        [Test] public void LowSample_Blocks(){var e=Good();e.Windows[0].ControlSamples=10;Assert.Contains(LiveOpsExecutionBlockerCodes.ObservationSampleInsufficient,ExperimentObservationEvaluator.Evaluate(e).Blockers);}
        [Test] public void RatioMismatch_Pauses(){var e=Good();e.Windows[0].ControlSamples=900;e.Windows[0].TreatmentSamples=100;Assert.AreEqual(ExperimentObservationRecommendation.PauseForSampleRatioReview,ExperimentObservationEvaluator.Evaluate(e).Recommendation);}
        [Test] public void DataQuality_Pauses(){var e=Good();e.Windows[0].DataQualityPassed=false;Assert.AreEqual(ExperimentObservationRecommendation.PauseForDataQualityReview,ExperimentObservationEvaluator.Evaluate(e).Recommendation);}
        [Test] public void Guardrail_Stops(){var e=Good();e.Windows[0].GuardrailBreaches=1;Assert.AreEqual(ExperimentObservationRecommendation.StopForGuardrailReview,ExperimentObservationEvaluator.Evaluate(e).Recommendation);}
        [Test] public void StoppingRule_Stops(){var e=Good();e.StoppingRuleTriggered=true;Assert.AreEqual(ExperimentObservationRecommendation.StopForStoppingRuleReview,ExperimentObservationEvaluator.Evaluate(e).Recommendation);}
        [Test] public void PrivacyMissing_Blocks(){var e=Good();e.PrivacyReviewed=false;Assert.Contains(LiveOpsExecutionBlockerCodes.ObservationPrivacyReviewMissing,ExperimentObservationEvaluator.Evaluate(e).Blockers);}
        [Test] public void NegativeSample_Invalid(){var e=Good();e.Windows[0].ControlSamples=-1;Assert.Contains(LiveOpsExecutionBlockerCodes.ObservationInvalid,ExperimentObservationEvaluator.Evaluate(e).Blockers);}
        [Test] public void Totals_Aggregate(){var e=Good();e.Windows=new[]{new ExperimentObservationWindow{WindowIndex=0,ControlSamples=120,TreatmentSamples=130,DataQualityPassed=true},new ExperimentObservationWindow{WindowIndex=1,ControlSamples=80,TreatmentSamples=70,DataQualityPassed=true}};var r=ExperimentObservationEvaluator.Evaluate(e);Assert.AreEqual(200,r.TotalControlSamples);Assert.AreEqual(200,r.TotalTreatmentSamples);}
        [Test] public void RatioMismatch_ZeroBalanced(){Assert.AreEqual(0,ExperimentObservationEvaluator.Evaluate(Good()).SampleRatioMismatchBasisPoints);}
    }
}
