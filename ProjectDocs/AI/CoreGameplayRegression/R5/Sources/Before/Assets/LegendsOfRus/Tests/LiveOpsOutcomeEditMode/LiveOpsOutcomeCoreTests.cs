using NUnit.Framework;
using LegendsOfRus.LiveOpsOutcome;

namespace LegendsOfRus.Tests.LiveOpsOutcome
{
    public sealed class LiveOpsOutcomeCoreTests
    {
        [Test]
        public void Requirements_SchemaVersion_IsOne()
        {
            Assert.That(LiveOpsOutcomeRequirements.SchemaVersion, Is.EqualTo(1));
        }
        [Test]
        public void Requirements_BasisPointsTotal_Is10000()
        {
            Assert.That(LiveOpsOutcomeRequirements.BasisPointsTotal, Is.EqualTo(10000));
        }
        [Test]
        public void Requirements_MinimumParticipants_Is100()
        {
            Assert.That(LiveOpsOutcomeRequirements.MinimumOutcomeParticipants, Is.EqualTo(100));
        }
        [Test]
        public void Requirements_MinimumExperimentSamples_Is200()
        {
            Assert.That(LiveOpsOutcomeRequirements.MinimumExperimentSamplesTotal, Is.EqualTo(200));
        }
        [Test]
        public void Requirements_DefaultBuffer_Is30()
        {
            Assert.That(LiveOpsOutcomeRequirements.DefaultObservationCapacity, Is.EqualTo(30));
        }
        [Test]
        public void Requirements_MaxBuffer_Is90()
        {
            Assert.That(LiveOpsOutcomeRequirements.MaxObservationCapacity, Is.EqualTo(90));
        }
        [Test]
        public void Validation_Prefix_AcceptsExpected()
        {
            Assert.That(LiveOpsOutcomeValidation.HasPrefix("LOR-LO-OUT-1", LiveOpsOutcomeRequirements.OutcomePrefix), Is.True);
        }
        [Test]
        public void Validation_Prefix_RejectsEmpty()
        {
            Assert.That(LiveOpsOutcomeValidation.HasPrefix(string.Empty, LiveOpsOutcomeRequirements.OutcomePrefix), Is.False);
        }
        [Test]
        public void Validation_Rate_Quarter_Is2500()
        {
            Assert.That(LiveOpsOutcomeValidation.RateBasisPoints(1,4), Is.EqualTo(2500));
        }
        [Test]
        public void Validation_Rate_ZeroTotal_IsZero()
        {
            Assert.That(LiveOpsOutcomeValidation.RateBasisPoints(5,0), Is.EqualTo(0));
        }
        [Test]
        public void Outcome_HealthyFixture_IsValid()
        {
            Assert.That(LiveOpsOutcomeValidation.ValidateOutcome(HealthyOutcome()).Length, Is.EqualTo(0));
        }
        [Test]
        public void Outcome_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(null), LiveOpsOutcomeBlockerCodes.OutcomeMissing), Is.True);
        }
        [Test]
        public void Outcome_Unreviewed_Blocks()
        {
            var x=HealthyOutcome(); x.EvidenceReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomeNotReviewed), Is.True);
        }
        [Test]
        public void Outcome_BadId_Blocks()
        {
            var x=HealthyOutcome(); x.OutcomeId="bad"; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomeInvalid), Is.True);
        }
        [Test]
        public void Outcome_ContentHealthReview_Required()
        {
            var x=HealthyOutcome(); x.ContentHealthReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomeContentHealthReviewMissing), Is.True);
        }
        [Test]
        public void Outcome_EconomyReview_Required()
        {
            var x=HealthyOutcome(); x.EconomyImpactReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomeEconomyImpactReviewMissing), Is.True);
        }
        [Test]
        public void Outcome_PrivacyReview_Required()
        {
            var x=HealthyOutcome(); x.PrivacyImpactReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomePrivacyImpactReviewMissing), Is.True);
        }
        [Test]
        public void Outcome_CompletionRate_Computed()
        {
            Assert.That(LiveOpsOutcomeEvaluator.Evaluate(HealthyOutcome()).CompletionRateBasisPoints, Is.EqualTo(8000));
        }
        [Test]
        public void Outcome_HealthyFixture_IsHealthy()
        {
            Assert.That(LiveOpsOutcomeEvaluator.Evaluate(HealthyOutcome()).OutcomeHealthy, Is.True);
        }
        [Test]
        public void Outcome_SampleFloor_Blocks()
        {
            var x=HealthyOutcome(); x.ActiveParticipants=99; Assert.That(Has(LiveOpsOutcomeEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.OutcomeSampleInsufficient), Is.True);
        }
        [Test]
        public void Outcome_Crash_Blocks()
        {
            var x=HealthyOutcome(); x.ContentCrashes=1; Assert.That(Has(LiveOpsOutcomeEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.OutcomeCrashObserved), Is.True);
        }
        [Test]
        public void Outcome_BlockingIncident_Blocks()
        {
            var x=HealthyOutcome(); x.OpenBlockingIncidents=1; Assert.That(Has(LiveOpsOutcomeEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.OutcomeBlockingIncidentOpen), Is.True);
        }
        [Test]
        public void Outcome_RewardMismatch_Blocks()
        {
            var x=HealthyOutcome(); x.RewardClaimsGranted--; Assert.That(Has(LiveOpsOutcomeEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.OutcomeRewardReconciliationMismatch), Is.True);
        }
        [Test]
        public void Outcome_SupportEscalation_Blocks()
        {
            var x=HealthyOutcome(); x.OpenSupportEscalations=1; Assert.That(Has(LiveOpsOutcomeEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.OutcomeSupportEscalationOpen), Is.True);
        }
        [Test]
        public void Decision_HealthyFixture_IsValid()
        {
            Assert.That(LiveOpsOutcomeValidation.ValidateDecision(HealthyDecision()).Length, Is.EqualTo(0));
        }
        [Test]
        public void Decision_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(null), LiveOpsOutcomeBlockerCodes.DecisionMissing), Is.True);
        }
        [Test]
        public void Decision_Unreviewed_Blocks()
        {
            var x=HealthyDecision(); x.EvidenceReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(x), LiveOpsOutcomeBlockerCodes.DecisionNotReviewed), Is.True);
        }
        [Test]
        public void Decision_None_Blocks()
        {
            var x=HealthyDecision(); x.Decision=ExperimentDecisionKind.None; Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(x), LiveOpsOutcomeBlockerCodes.DecisionInvalid), Is.True);
        }
        [Test]
        public void Decision_StoppingRuleReview_Required()
        {
            var x=HealthyDecision(); x.StoppingRuleReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(x), LiveOpsOutcomeBlockerCodes.DecisionStoppingRuleReviewMissing), Is.True);
        }
        [Test]
        public void Decision_BasisReview_Required()
        {
            var x=HealthyDecision(); x.DecisionBasisReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(x), LiveOpsOutcomeBlockerCodes.DecisionBasisReviewMissing), Is.True);
        }
        [Test]
        public void Decision_HealthyFixture_ArchivesTreatment()
        {
            Assert.That(ExperimentDecisionEvaluator.Evaluate(HealthyDecision()).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.ArchiveTreatment));
        }
        [Test]
        public void Decision_HealthyFixture_IsFinal()
        {
            Assert.That(ExperimentDecisionEvaluator.Evaluate(HealthyDecision()).DecisionFinalized, Is.True);
        }
        [Test]
        public void Decision_SampleFloor_Blocks()
        {
            var x=HealthyDecision(); x.TotalSamples=199; Assert.That(Has(ExperimentDecisionEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.DecisionSampleInsufficient), Is.True);
        }
        [Test]
        public void Decision_DataQuality_Blocks()
        {
            var x=HealthyDecision(); x.DataQualityPassed=false; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.RevisitDecision));
        }
        [Test]
        public void Decision_SampleRatio_Blocks()
        {
            var x=HealthyDecision(); x.SampleRatioHealthy=false; Assert.That(Has(ExperimentDecisionEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.DecisionSampleRatioUnhealthy), Is.True);
        }
        [Test]
        public void Decision_IncompleteWindow_Continues()
        {
            var x=HealthyDecision(); x.ExperimentWindowComplete=false; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.ContinueObservation));
        }
        [Test]
        public void Decision_GuardrailConflict_BlocksAdopt()
        {
            var x=HealthyDecision(); x.GuardrailBreaches=1; Assert.That(Has(ExperimentDecisionEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict), Is.True);
        }
        [Test]
        public void Decision_GuardrailStop_WithBreach_Archives()
        {
            var x=HealthyDecision(); x.Decision=ExperimentDecisionKind.StopForGuardrail; x.GuardrailBreaches=2; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.ArchiveStoppedForGuardrail));
        }
        [Test]
        public void Decision_GuardrailStop_WithoutBreach_Blocks()
        {
            var x=HealthyDecision(); x.Decision=ExperimentDecisionKind.StopForGuardrail; Assert.That(Has(ExperimentDecisionEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.DecisionGuardrailConflict), Is.True);
        }
        [Test]
        public void Decision_KeepControl_ArchivesControl()
        {
            var x=HealthyDecision(); x.Decision=ExperimentDecisionKind.KeepControl; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.ArchiveControl));
        }
        [Test]
        public void Decision_Inconclusive_ArchivesInconclusive()
        {
            var x=HealthyDecision(); x.Decision=ExperimentDecisionKind.Inconclusive; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).Recommendation, Is.EqualTo(ExperimentDecisionRecommendation.ArchiveInconclusive));
        }
        [Test]
        public void Closeout_HealthyFixture_IsValid()
        {
            Assert.That(LiveOpsOutcomeValidation.ValidateCloseout(HealthyCloseout()).Length, Is.EqualTo(0));
        }
        [Test]
        public void Closeout_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeValidation.ValidateCloseout(null), LiveOpsOutcomeBlockerCodes.CloseoutMissing), Is.True);
        }
        [Test]
        public void Closeout_Unreviewed_Blocks()
        {
            var x=HealthyCloseout(); x.EvidenceReviewed=false; Assert.That(Has(LiveOpsOutcomeValidation.ValidateCloseout(x), LiveOpsOutcomeBlockerCodes.CloseoutNotReviewed), Is.True);
        }
        [Test]
        public void Closeout_BadCounts_Blocks()
        {
            var x=HealthyCloseout(); x.ClosedEntries=4; Assert.That(Has(LiveOpsOutcomeValidation.ValidateCloseout(x), LiveOpsOutcomeBlockerCodes.CloseoutInvalid), Is.True);
        }
        [Test]
        public void Closeout_HealthyFixture_Completes()
        {
            Assert.That(RotationCloseoutEvaluator.Evaluate(HealthyCloseout()).CloseoutComplete, Is.True);
        }
        [Test]
        public void Closeout_ActiveEntry_Blocks()
        {
            var x=HealthyCloseout(); x.ActiveEntriesRemaining=1; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutEntriesRemaining), Is.True);
        }
        [Test]
        public void Closeout_ClosedCountMismatch_Blocks()
        {
            var x=HealthyCloseout(); x.ClosedEntries=1; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutEntriesRemaining), Is.True);
        }
        [Test]
        public void Closeout_PendingRewards_Block()
        {
            var x=HealthyCloseout(); x.PendingRewardClaims=1; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutRewardClaimsPending), Is.True);
        }
        [Test]
        public void Closeout_OrphanedContent_Blocks()
        {
            var x=HealthyCloseout(); x.OrphanedContentReferences=1; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutOrphanedContent), Is.True);
        }
        [Test]
        public void Closeout_Cooldown_Blocks()
        {
            var x=HealthyCloseout(); x.CooldownsSatisfied=false; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutCooldownIncomplete), Is.True);
        }
        [Test]
        public void Closeout_Archive_Blocks()
        {
            var x=HealthyCloseout(); x.ContentVersionsArchived=false; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutVersionsNotArchived), Is.True);
        }
        [Test]
        public void Closeout_SupportHandoff_Blocks()
        {
            var x=HealthyCloseout(); x.SupportHandoffComplete=false; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutSupportHandoffMissing), Is.True);
        }
        [Test]
        public void Closeout_Window_Blocks()
        {
            var x=HealthyCloseout(); x.CloseoutWindowComplete=false; Assert.That(Has(RotationCloseoutEvaluator.Evaluate(x).Blockers, LiveOpsOutcomeBlockerCodes.CloseoutWindowIncomplete), Is.True);
        }
        [Test]
        public void Overall_Healthy_IsReadyForHumanReview()
        {
            Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),false,false,false,false,false,false).ReadyForHumanReview, Is.True);
        }
        [Test]
        public void Overall_DisabledGates_BlockManualCloseout()
        {
            Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),false,false,false,false,false,false).MayProceedWithManualCloseoutReview, Is.False);
        }
        [Test]
        public void Overall_AllGates_AllowManualReview()
        {
            Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,true,true,true,true).MayProceedWithManualCloseoutReview, Is.True);
        }
        [Test]
        public void Overall_AllGates_RecommendReady()
        {
            Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsOutcomeRecommendation.ReadyForManualCloseoutReview));
        }
        [Test]
        public void Overall_OutcomeBlocker_Recommended()
        {
            var x=HealthyOutcome(); x.ContentCrashes=1; Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(x,HealthyDecision(),HealthyCloseout(),true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsOutcomeRecommendation.ResolveOutcomeBlockers));
        }
        [Test]
        public void Overall_DecisionBlocker_Recommended()
        {
            var x=HealthyDecision(); x.DataQualityPassed=false; Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),x,HealthyCloseout(),true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsOutcomeRecommendation.RevisitExperimentDecision));
        }
        [Test]
        public void Overall_CloseoutBlocker_Recommended()
        {
            var x=HealthyCloseout(); x.PendingRewardClaims=1; Assert.That(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),x,true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsOutcomeRecommendation.CompleteRotationCloseout));
        }
        [Test]
        public void Overall_AnalyticsGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),false,true,true,true,true,true).ActionBlockers, LiveOpsOutcomeBlockerCodes.ProductionAnalyticsDisabled), Is.True);
        }
        [Test]
        public void Overall_RemoteConfigGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,false,true,true,true,true).ActionBlockers, LiveOpsOutcomeBlockerCodes.ProductionRemoteConfigDisabled), Is.True);
        }
        [Test]
        public void Overall_OutcomeAuthGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,false,true,true,true).ActionBlockers, LiveOpsOutcomeBlockerCodes.OutcomeReviewNotAuthorized), Is.True);
        }
        [Test]
        public void Overall_DecisionAuthGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,true,false,true,true).ActionBlockers, LiveOpsOutcomeBlockerCodes.ExperimentDecisionNotAuthorized), Is.True);
        }
        [Test]
        public void Overall_CloseoutAuthGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,true,true,false,true).ActionBlockers, LiveOpsOutcomeBlockerCodes.RotationCloseoutNotAuthorized), Is.True);
        }
        [Test]
        public void Overall_ManualAuthGate_Blocks()
        {
            Assert.That(Has(LiveOpsOutcomeReviewEvaluator.Evaluate(HealthyOutcome(),HealthyDecision(),HealthyCloseout(),true,true,true,true,true,false).ActionBlockers, LiveOpsOutcomeBlockerCodes.ManualCloseoutReviewNotAuthorized), Is.True);
        }
        [Test]
        public void Buffer_UsesRequestedCapacity()
        {
            Assert.That(new LiveOpsOutcomeObservationBuffer(3).Capacity, Is.EqualTo(3));
        }
        [Test]
        public void Buffer_ClampsLowCapacity()
        {
            Assert.That(new LiveOpsOutcomeObservationBuffer(0).Capacity, Is.EqualTo(1));
        }
        [Test]
        public void Buffer_ClampsHighCapacity()
        {
            Assert.That(new LiveOpsOutcomeObservationBuffer(1000).Capacity, Is.EqualTo(LiveOpsOutcomeRequirements.MaxObservationCapacity));
        }
        [Test]
        public void Buffer_IgnoresNull()
        {
            var b=new LiveOpsOutcomeObservationBuffer(2); b.Add(null); Assert.That(b.Count, Is.EqualTo(0));
        }
        [Test]
        public void Buffer_IsBounded()
        {
            var b=new LiveOpsOutcomeObservationBuffer(2); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=3}); Assert.That(b.Count, Is.EqualTo(2));
        }
        [Test]
        public void Buffer_PreservesOldestFirstAfterWrap()
        {
            var b=new LiveOpsOutcomeObservationBuffer(2); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=3}); Assert.That(b.SnapshotOldestFirst()[0].WindowIndex, Is.EqualTo(2));
        }
        [Test]
        public void Buffer_PreservesNewestAfterWrap()
        {
            var b=new LiveOpsOutcomeObservationBuffer(2); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsOutcomeAggregateObservation{WindowIndex=3}); Assert.That(b.SnapshotOldestFirst()[1].WindowIndex, Is.EqualTo(3));
        }
        [Test]
        public void Buffer_Clear_ResetsCount()
        {
            var b=new LiveOpsOutcomeObservationBuffer(2); b.Add(new LiveOpsOutcomeAggregateObservation()); b.Clear(); Assert.That(b.Count, Is.EqualTo(0));
        }
        [Test]
        public void Outcome_InvalidCompletionCount_Blocks()
        {
            var x=HealthyOutcome(); x.ContentCompletions=x.ContentEntries+1; Assert.That(Has(LiveOpsOutcomeValidation.ValidateOutcome(x), LiveOpsOutcomeBlockerCodes.OutcomeInvalid), Is.True);
        }
        [Test]
        public void Decision_BadId_Blocks()
        {
            var x=HealthyDecision(); x.DecisionId="bad"; Assert.That(Has(LiveOpsOutcomeValidation.ValidateDecision(x), LiveOpsOutcomeBlockerCodes.DecisionInvalid), Is.True);
        }
        [Test]
        public void Closeout_BadId_Blocks()
        {
            var x=HealthyCloseout(); x.CloseoutId="bad"; Assert.That(Has(LiveOpsOutcomeValidation.ValidateCloseout(x), LiveOpsOutcomeBlockerCodes.CloseoutInvalid), Is.True);
        }
        [Test]
        public void Outcome_SampleExactlyFloor_Passes()
        {
            var x=HealthyOutcome(); x.ActiveParticipants=LiveOpsOutcomeRequirements.MinimumOutcomeParticipants; Assert.That(LiveOpsOutcomeEvaluator.Evaluate(x).SampleSufficient, Is.True);
        }
        [Test]
        public void Decision_SampleExactlyFloor_Passes()
        {
            var x=HealthyDecision(); x.TotalSamples=LiveOpsOutcomeRequirements.MinimumExperimentSamplesTotal; Assert.That(ExperimentDecisionEvaluator.Evaluate(x).DecisionFinalized, Is.True);
        }

        static LiveOpsOutcomeEvidence HealthyOutcome(){return new LiveOpsOutcomeEvidence{EvidenceRetained=true,EvidenceReviewed=true,OutcomeId="LOR-LO-OUT-1",ExecutionId="LOR-LO-EXEC-1",WindowStartMinute=0,WindowEndMinute=600,ActiveParticipants=500,ContentEntries=1000,ContentCompletions=800,ContentCrashes=0,OpenBlockingIncidents=0,RewardClaimsExpected=700,RewardClaimsGranted=700,OpenSupportEscalations=0,ContentHealthReviewed=true,EconomyImpactReviewed=true,PrivacyImpactReviewed=true};}
        static ExperimentDecisionEvidence HealthyDecision(){return new ExperimentDecisionEvidence{EvidenceRetained=true,EvidenceReviewed=true,DecisionId="LOR-EXP-DEC-1",ObservationId="LOR-EXP-OBS-1",ExperimentId="LOR-EXP-1",Decision=ExperimentDecisionKind.AdoptTreatment,TotalSamples=1000,PrimaryMetricDeltaBasisPoints=500,GuardrailBreaches=0,DataQualityPassed=true,SampleRatioHealthy=true,ExperimentWindowComplete=true,StoppingRuleReviewed=true,DecisionBasisReviewed=true};}
        static RotationCloseoutEvidence HealthyCloseout(){return new RotationCloseoutEvidence{EvidenceRetained=true,EvidenceReviewed=true,CloseoutId="LOR-ROT-CLOSE-1",RotationPlanId="LOR-LO-ROT-PLAN-1",PlannedEntries=2,ClosedEntries=2,ActiveEntriesRemaining=0,PendingRewardClaims=0,OrphanedContentReferences=0,CooldownsSatisfied=true,ContentVersionsArchived=true,SupportHandoffComplete=true,CloseoutWindowComplete=true};}
        static bool Has(string[] values,string value){for(int i=0;i<values.Length;i++)if(values[i]==value)return true;return false;}
    }
}
