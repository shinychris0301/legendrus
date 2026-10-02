using NUnit.Framework;
using LegendsOfRus.LiveOpsCycle;

namespace LegendsOfRus.Tests.LiveOpsCycle
{
    public sealed class LiveOpsCycleCoreTests
    {
        [Test]
        public void Retrospective_Healthy_Ready()
        {
            Assert.That(RetrospectiveEvaluator.Evaluate(R()).ReadyForArchive, Is.True);
        }

        [Test]
        public void Retrospective_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(null), LiveOpsCycleBlockerCodes.RetrospectiveMissing), Is.True);
        }

        [Test]
        public void Retrospective_NotRetained_Blocks()
        {
            var x=R(); x.EvidenceRetained=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveMissing), Is.True);
        }

        [Test]
        public void Retrospective_NotReviewed_Blocks()
        {
            var x=R(); x.EvidenceReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveNotReviewed), Is.True);
        }

        [Test]
        public void Retrospective_BadId_Blocks()
        {
            var x=R(); x.RetrospectiveId="bad";  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveInvalid), Is.True);
        }

        [Test]
        public void Retrospective_LowSample_Blocks()
        {
            var x=R(); x.ParticipantsReviewed=99;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveSampleInsufficient), Is.True);
        }

        [Test]
        public void Retrospective_Objectives_Blocks()
        {
            var x=R(); x.ObjectivesReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveObjectivesMissing), Is.True);
        }

        [Test]
        public void Retrospective_Metrics_Blocks()
        {
            var x=R(); x.SuccessMetricsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveMetricsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Incidents_Blocks()
        {
            var x=R(); x.IncidentLessonsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveIncidentLessonsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Content_Blocks()
        {
            var x=R(); x.ContentLessonsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveContentLessonsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Economy_Blocks()
        {
            var x=R(); x.EconomyLessonsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveEconomyLessonsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Support_Blocks()
        {
            var x=R(); x.SupportLessonsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveSupportLessonsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Privacy_Blocks()
        {
            var x=R(); x.PrivacyLessonsReviewed=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectivePrivacyLessonsMissing), Is.True);
        }

        [Test]
        public void Retrospective_Critical_Blocks()
        {
            var x=R(); x.UnresolvedCriticalLessons=1;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveCriticalLessonsOpen), Is.True);
        }

        [Test]
        public void Retrospective_TooManyActions_Blocks()
        {
            var x=R(); x.ActionItemCount=21;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveActionItemsInvalid), Is.True);
        }

        [Test]
        public void Retrospective_Owners_Blocks()
        {
            var x=R(); x.ActionOwnersAssigned=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveOwnersMissing), Is.True);
        }

        [Test]
        public void Retrospective_DueDates_Blocks()
        {
            var x=R(); x.ActionDueDatesAssigned=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveDueDatesMissing), Is.True);
        }

        [Test]
        public void Retrospective_ArchivePins_Blocks()
        {
            var x=R(); x.ArchiveInputsPinned=false;  Assert.That(Has(LiveOpsCycleValidation.ValidateRetrospective(x), LiveOpsCycleBlockerCodes.RetrospectiveArchiveInputsNotPinned), Is.True);
        }

        [Test]
        public void Retrospective_ZeroActions_DoesNotRequireOwners()
        {
            var x=R(); x.ActionItemCount=0; x.ActionOwnersAssigned=false; x.ActionDueDatesAssigned=false; Assert.That(RetrospectiveEvaluator.Evaluate(x).ReadyForArchive, Is.True);
        }

        [Test]
        public void Retrospective_SampleFloor_Passes()
        {
            var x=R(); x.ParticipantsReviewed=LiveOpsCycleRequirements.MinimumRetrospectiveParticipants; Assert.That(RetrospectiveEvaluator.Evaluate(x).ReadyForArchive, Is.True);
        }

        [Test]
        public void Archive_Healthy_Ready()
        {
            Assert.That(CycleArchiveEvaluator.Evaluate(A()).ArchiveReady, Is.True);
        }

        [Test]
        public void Archive_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(null), LiveOpsCycleBlockerCodes.ArchiveMissing), Is.True);
        }

        [Test]
        public void Archive_NotRetained_Blocks()
        {
            var x=A(); x.EvidenceRetained=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveMissing), Is.True);
        }

        [Test]
        public void Archive_NotReviewed_Blocks()
        {
            var x=A(); x.EvidenceReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveNotReviewed), Is.True);
        }

        [Test]
        public void Archive_BadId_Blocks()
        {
            var x=A(); x.ArchiveId="bad"; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveInvalid), Is.True);
        }

        [Test]
        public void Archive_BadHash_Blocks()
        {
            var x=A(); x.SourceTreeSha256="bad"; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveHashesInvalid), Is.True);
        }

        [Test]
        public void Archive_Immutable_Blocks()
        {
            var x=A(); x.ImmutableStorageReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveImmutableStorageMissing), Is.True);
        }

        [Test]
        public void Archive_Pii_Blocks()
        {
            var x=A(); x.PiiReviewPassed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchivePiiReviewFailed), Is.True);
        }

        [Test]
        public void Archive_Secrets_Blocks()
        {
            var x=A(); x.SecretsReviewPassed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveSecretsReviewFailed), Is.True);
        }

        [Test]
        public void Archive_Manifest_Blocks()
        {
            var x=A(); x.ArchiveManifestPinned=false; Assert.That(Has(LiveOpsCycleValidation.ValidateArchive(x), LiveOpsCycleBlockerCodes.ArchiveManifestNotPinned), Is.True);
        }

        [Test]
        public void Archive_UppercaseSha_Accepted()
        {
            var x=A(); x.SourceTreeSha256=Sha().ToUpperInvariant(); Assert.That(CycleArchiveEvaluator.Evaluate(x).ArchiveReady, Is.True);
        }

        [Test]
        public void Cadence_Healthy_Ready()
        {
            Assert.That(NextCadenceEvaluator.Evaluate(C()).CadenceReady, Is.True);
        }

        [Test]
        public void Cadence_Missing_Blocks()
        {
            Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(null), LiveOpsCycleBlockerCodes.CadenceMissing), Is.True);
        }

        [Test]
        public void Cadence_NotRetained_Blocks()
        {
            var x=C(); x.EvidenceRetained=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceMissing), Is.True);
        }

        [Test]
        public void Cadence_NotReviewed_Blocks()
        {
            var x=C(); x.EvidenceReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceNotReviewed), Is.True);
        }

        [Test]
        public void Cadence_BadId_Blocks()
        {
            var x=C(); x.PlanId="bad"; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceInvalid), Is.True);
        }

        [Test]
        public void Cadence_LeadTime_Blocks()
        {
            var x=C(); x.LeadTimeMinutes=100; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceLeadTimeInsufficient), Is.True);
        }

        [Test]
        public void Cadence_Events_Blocks()
        {
            var x=C(); x.PlannedEvents=13; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceTooManyEvents), Is.True);
        }

        [Test]
        public void Cadence_Experiments_Blocks()
        {
            var x=C(); x.PlannedExperiments=5; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceTooManyExperiments), Is.True);
        }

        [Test]
        public void Cadence_Concurrency_Blocks()
        {
            var x=C(); x.MaxConcurrentEvents=5; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceConcurrencyInvalid), Is.True);
        }

        [Test]
        public void Cadence_Cooldown_Blocks()
        {
            var x=C(); x.MinimumCooldownMinutes=60; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceCooldownInsufficient), Is.True);
        }

        [Test]
        public void Cadence_Calendar_Blocks()
        {
            var x=C(); x.CalendarConflictCount=1; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceCalendarConflict), Is.True);
        }

        [Test]
        public void Cadence_Dependencies_Blocks()
        {
            var x=C(); x.DependenciesReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceDependenciesMissing), Is.True);
        }

        [Test]
        public void Cadence_Capacity_Blocks()
        {
            var x=C(); x.CapacityReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceCapacityMissing), Is.True);
        }

        [Test]
        public void Cadence_Localization_Blocks()
        {
            var x=C(); x.LocalizationReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceLocalizationMissing), Is.True);
        }

        [Test]
        public void Cadence_Support_Blocks()
        {
            var x=C(); x.SupportCoverageReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceSupportCoverageMissing), Is.True);
        }

        [Test]
        public void Cadence_Content_Blocks()
        {
            var x=C(); x.ContentReady=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceContentReadinessMissing), Is.True);
        }

        [Test]
        public void Cadence_Measurement_Blocks()
        {
            var x=C(); x.MeasurementPlanReviewed=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceMeasurementPlanMissing), Is.True);
        }

        [Test]
        public void Cadence_Carryover_Blocks()
        {
            var x=C(); x.NoCarryoverBlockers=false; Assert.That(Has(LiveOpsCycleValidation.ValidateCadence(x), LiveOpsCycleBlockerCodes.CadenceCarryoverBlockers), Is.True);
        }

        [Test]
        public void Cadence_LeadFloor_Passes()
        {
            var x=C(); x.LeadTimeMinutes=LiveOpsCycleRequirements.MinimumNextCadenceLeadMinutes; Assert.That(NextCadenceEvaluator.Evaluate(x).CadenceReady, Is.True);
        }

        [Test]
        public void Cadence_NoEvents_AllowsZeroConcurrency()
        {
            var x=C(); x.PlannedEvents=0; x.MaxConcurrentEvents=0; Assert.That(NextCadenceEvaluator.Evaluate(x).CadenceReady, Is.True);
        }

        [Test]
        public void Overall_Healthy_ReadyForHumanReview()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),false,false,false,false,false,false,false).ReadyForHumanReview, Is.True);
        }

        [Test]
        public void Overall_AllGates_CloseCycle()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,true).MayCloseCycleManually, Is.True);
        }

        [Test]
        public void Overall_AllGates_PublishNextCadence()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,true).MayPublishNextCadence, Is.True);
        }

        [Test]
        public void Overall_RetroBlocker_Recommendation()
        {
            var x=R(); x.UnresolvedCriticalLessons=1; Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(x,A(),C(),true,true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsCycleRecommendation.ResolveRetrospective));
        }

        [Test]
        public void Overall_ArchiveBlocker_Recommendation()
        {
            var x=A(); x.PiiReviewPassed=false; Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),x,C(),true,true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsCycleRecommendation.CompleteArchive));
        }

        [Test]
        public void Overall_CadenceBlocker_Recommendation()
        {
            var x=C(); x.CalendarConflictCount=1; Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),x,true,true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsCycleRecommendation.ReviseNextCadence));
        }

        [Test]
        public void Overall_Healthy_Recommendation()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,true).Recommendation, Is.EqualTo(LiveOpsCycleRecommendation.ReadyForManualCycleClosure));
        }

        [Test]
        public void Overall_Analytics_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),false,true,true,true,true,true,true).ActionBlockers, LiveOpsCycleBlockerCodes.ProductionAnalyticsDisabled), Is.True);
        }

        [Test]
        public void Overall_RemoteConfig_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,false,true,true,true,true,true).ActionBlockers, LiveOpsCycleBlockerCodes.ProductionRemoteConfigDisabled), Is.True);
        }

        [Test]
        public void Overall_RetroAuth_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,false,true,true,true,true).ActionBlockers, LiveOpsCycleBlockerCodes.RetrospectiveReviewNotAuthorized), Is.True);
        }

        [Test]
        public void Overall_ArchiveAuth_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,false,true,true,true).ActionBlockers, LiveOpsCycleBlockerCodes.ArchiveReviewNotAuthorized), Is.True);
        }

        [Test]
        public void Overall_CadenceAuth_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,false,true,true).ActionBlockers, LiveOpsCycleBlockerCodes.NextCadenceReviewNotAuthorized), Is.True);
        }

        [Test]
        public void Overall_ClosureAuth_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,false,true).ActionBlockers, LiveOpsCycleBlockerCodes.ManualCycleClosureNotAuthorized), Is.True);
        }

        [Test]
        public void Overall_PublishAuth_GateBlocks()
        {
            Assert.That(Has(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,true,true,true,true,true,false).ActionBlockers, LiveOpsCycleBlockerCodes.LiveOpsPublishingNotAuthorized), Is.True);
        }

        [Test]
        public void Overall_PublishNeedsRemoteConfig()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,false,true,true,true,true,true).MayPublishNextCadence, Is.False);
        }

        [Test]
        public void Overall_CloseDoesNotRequireRemoteConfig()
        {
            Assert.That(LiveOpsCycleReviewEvaluator.Evaluate(R(),A(),C(),true,false,true,true,true,true,false).MayCloseCycleManually, Is.True);
        }

        [Test]
        public void Buffer_UsesRequestedCapacity()
        {
            Assert.That(new LiveOpsCycleObservationBuffer(3).Capacity, Is.EqualTo(3));
        }

        [Test]
        public void Buffer_ClampsLow()
        {
            Assert.That(new LiveOpsCycleObservationBuffer(0).Capacity, Is.EqualTo(1));
        }

        [Test]
        public void Buffer_ClampsHigh()
        {
            Assert.That(new LiveOpsCycleObservationBuffer(999).Capacity, Is.EqualTo(LiveOpsCycleRequirements.MaxObservationCapacity));
        }

        [Test]
        public void Buffer_IgnoresNull()
        {
            var b=new LiveOpsCycleObservationBuffer(2); b.Add(null); Assert.That(b.Count, Is.EqualTo(0));
        }

        [Test]
        public void Buffer_IsBounded()
        {
            var b=new LiveOpsCycleObservationBuffer(2); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=3}); Assert.That(b.Count, Is.EqualTo(2));
        }

        [Test]
        public void Buffer_OrderPreserved()
        {
            var b=new LiveOpsCycleObservationBuffer(2); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=3}); Assert.That(b.SnapshotOldestFirst()[0].WindowIndex, Is.EqualTo(2));
        }

        [Test]
        public void Buffer_NewestPreserved()
        {
            var b=new LiveOpsCycleObservationBuffer(2); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=1}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=2}); b.Add(new LiveOpsCycleAggregateObservation{WindowIndex=3}); Assert.That(b.SnapshotOldestFirst()[1].WindowIndex, Is.EqualTo(3));
        }

        [Test]
        public void Buffer_Clear()
        {
            var b=new LiveOpsCycleObservationBuffer(2); b.Add(new LiveOpsCycleAggregateObservation()); b.Clear(); Assert.That(b.Count, Is.EqualTo(0));
        }

        [Test]
        public void Sha_64Lower_Valid()
        {
            Assert.That(LiveOpsCycleValidation.ValidSha(Sha()), Is.True);
        }

        [Test]
        public void Sha_63_Invalid()
        {
            Assert.That(LiveOpsCycleValidation.ValidSha(Sha().Substring(0,63)), Is.False);
        }

        [Test]
        public void Sha_NonHex_Invalid()
        {
            Assert.That(LiveOpsCycleValidation.ValidSha(new string('z',64)), Is.False);
        }


        static CycleRetrospectiveEvidence R(){return new CycleRetrospectiveEvidence{EvidenceRetained=true,EvidenceReviewed=true,RetrospectiveId="LOR-RETRO-1",CycleId="LOR-CYCLE-1",OutcomeId="LOR-LO-OUT-1",ExperimentDecisionId="LOR-EXP-DEC-1",RotationCloseoutId="LOR-ROT-CLOSE-1",ParticipantsReviewed=500,ObjectivesReviewed=true,SuccessMetricsReviewed=true,IncidentLessonsReviewed=true,ContentLessonsReviewed=true,EconomyLessonsReviewed=true,SupportLessonsReviewed=true,PrivacyLessonsReviewed=true,ActionItemCount=3,ActionOwnersAssigned=true,ActionDueDatesAssigned=true,UnresolvedCriticalLessons=0,ArchiveInputsPinned=true};}
        static CycleArchiveEvidence A(){return new CycleArchiveEvidence{EvidenceRetained=true,EvidenceReviewed=true,ArchiveId="LOR-ARCHIVE-1",CycleId="LOR-CYCLE-1",SourceTreeSha256=Sha(),OutcomeSha256=Sha(),ExperimentDecisionSha256=Sha(),RotationCloseoutSha256=Sha(),RetrospectiveSha256=Sha(),ArchiveManifestSha256=Sha(),ImmutableStorageReviewed=true,PiiReviewPassed=true,SecretsReviewPassed=true,ArchiveManifestPinned=true};}
        static NextCadencePlanEvidence C(){return new NextCadencePlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,PlanId="LOR-CADENCE-1",PriorCycleId="LOR-CYCLE-1",LeadTimeMinutes=2880,PlannedEvents=4,PlannedExperiments=1,MaxConcurrentEvents=2,MinimumCooldownMinutes=240,CalendarConflictCount=0,DependenciesReviewed=true,CapacityReviewed=true,LocalizationReviewed=true,SupportCoverageReviewed=true,ContentReady=true,MeasurementPlanReviewed=true,NoCarryoverBlockers=true};}
        static string Sha(){return new string('a',64);}
        static bool Has(string[] values,string value){for(int i=0;i<values.Length;i++)if(values[i]==value)return true;return false;}
    }
}
