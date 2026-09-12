using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsCycle
{
    public static class LiveOpsCycleValidation
    {
        public static string[] ValidateRetrospective(CycleRetrospectiveEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsCycleBlockerCodes.RetrospectiveMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveNotReviewed);
            if(e.SchemaVersion!=LiveOpsCycleRequirements.SchemaVersion || !ValidId(e.RetrospectiveId,LiveOpsCycleRequirements.RetrospectivePrefix) || string.IsNullOrEmpty(e.CycleId) || string.IsNullOrEmpty(e.OutcomeId) || string.IsNullOrEmpty(e.ExperimentDecisionId) || string.IsNullOrEmpty(e.RotationCloseoutId) || e.ParticipantsReviewed<0 || e.ActionItemCount<0 || e.UnresolvedCriticalLessons<0)
                b.Add(LiveOpsCycleBlockerCodes.RetrospectiveInvalid);
            if(e.ParticipantsReviewed<LiveOpsCycleRequirements.MinimumRetrospectiveParticipants)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveSampleInsufficient);
            if(!e.ObjectivesReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveObjectivesMissing);
            if(!e.SuccessMetricsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveMetricsMissing);
            if(!e.IncidentLessonsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveIncidentLessonsMissing);
            if(!e.ContentLessonsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveContentLessonsMissing);
            if(!e.EconomyLessonsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveEconomyLessonsMissing);
            if(!e.SupportLessonsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveSupportLessonsMissing);
            if(!e.PrivacyLessonsReviewed)b.Add(LiveOpsCycleBlockerCodes.RetrospectivePrivacyLessonsMissing);
            if(e.UnresolvedCriticalLessons>0)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveCriticalLessonsOpen);
            if(e.ActionItemCount>LiveOpsCycleRequirements.MaxActionItems)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveActionItemsInvalid);
            if(e.ActionItemCount>0&&!e.ActionOwnersAssigned)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveOwnersMissing);
            if(e.ActionItemCount>0&&!e.ActionDueDatesAssigned)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveDueDatesMissing);
            if(!e.ArchiveInputsPinned)b.Add(LiveOpsCycleBlockerCodes.RetrospectiveArchiveInputsNotPinned);
            return Unique(b);
        }

        public static string[] ValidateArchive(CycleArchiveEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsCycleBlockerCodes.ArchiveMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsCycleBlockerCodes.ArchiveMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsCycleBlockerCodes.ArchiveNotReviewed);
            if(e.SchemaVersion!=LiveOpsCycleRequirements.SchemaVersion || !ValidId(e.ArchiveId,LiveOpsCycleRequirements.ArchivePrefix) || string.IsNullOrEmpty(e.CycleId))b.Add(LiveOpsCycleBlockerCodes.ArchiveInvalid);
            if(!ValidSha(e.SourceTreeSha256)||!ValidSha(e.OutcomeSha256)||!ValidSha(e.ExperimentDecisionSha256)||!ValidSha(e.RotationCloseoutSha256)||!ValidSha(e.RetrospectiveSha256)||!ValidSha(e.ArchiveManifestSha256))b.Add(LiveOpsCycleBlockerCodes.ArchiveHashesInvalid);
            if(!e.ImmutableStorageReviewed)b.Add(LiveOpsCycleBlockerCodes.ArchiveImmutableStorageMissing);
            if(!e.PiiReviewPassed)b.Add(LiveOpsCycleBlockerCodes.ArchivePiiReviewFailed);
            if(!e.SecretsReviewPassed)b.Add(LiveOpsCycleBlockerCodes.ArchiveSecretsReviewFailed);
            if(!e.ArchiveManifestPinned)b.Add(LiveOpsCycleBlockerCodes.ArchiveManifestNotPinned);
            return Unique(b);
        }

        public static string[] ValidateCadence(NextCadencePlanEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsCycleBlockerCodes.CadenceMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsCycleBlockerCodes.CadenceMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceNotReviewed);
            if(e.SchemaVersion!=LiveOpsCycleRequirements.SchemaVersion || !ValidId(e.PlanId,LiveOpsCycleRequirements.CadencePrefix) || string.IsNullOrEmpty(e.PriorCycleId) || e.LeadTimeMinutes<0 || e.PlannedEvents<0 || e.PlannedExperiments<0 || e.MaxConcurrentEvents<0 || e.MinimumCooldownMinutes<0 || e.CalendarConflictCount<0)b.Add(LiveOpsCycleBlockerCodes.CadenceInvalid);
            if(e.LeadTimeMinutes<LiveOpsCycleRequirements.MinimumNextCadenceLeadMinutes)b.Add(LiveOpsCycleBlockerCodes.CadenceLeadTimeInsufficient);
            if(e.PlannedEvents>LiveOpsCycleRequirements.MaxPlannedEvents)b.Add(LiveOpsCycleBlockerCodes.CadenceTooManyEvents);
            if(e.PlannedExperiments>LiveOpsCycleRequirements.MaxPlannedExperiments)b.Add(LiveOpsCycleBlockerCodes.CadenceTooManyExperiments);
            if(e.PlannedEvents>0&&(e.MaxConcurrentEvents<=0||e.MaxConcurrentEvents>e.PlannedEvents))b.Add(LiveOpsCycleBlockerCodes.CadenceConcurrencyInvalid);
            if(e.MinimumCooldownMinutes<LiveOpsCycleRequirements.MinimumCadenceGapMinutes)b.Add(LiveOpsCycleBlockerCodes.CadenceCooldownInsufficient);
            if(e.CalendarConflictCount>0)b.Add(LiveOpsCycleBlockerCodes.CadenceCalendarConflict);
            if(!e.DependenciesReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceDependenciesMissing);
            if(!e.CapacityReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceCapacityMissing);
            if(!e.LocalizationReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceLocalizationMissing);
            if(!e.SupportCoverageReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceSupportCoverageMissing);
            if(!e.ContentReady)b.Add(LiveOpsCycleBlockerCodes.CadenceContentReadinessMissing);
            if(!e.MeasurementPlanReviewed)b.Add(LiveOpsCycleBlockerCodes.CadenceMeasurementPlanMissing);
            if(!e.NoCarryoverBlockers)b.Add(LiveOpsCycleBlockerCodes.CadenceCarryoverBlockers);
            return Unique(b);
        }

        static bool ValidId(string value,string prefix){return !string.IsNullOrEmpty(value)&&value.StartsWith(prefix,StringComparison.Ordinal)&&value.Length>prefix.Length;}
        public static bool ValidSha(string value)
        {
            if(string.IsNullOrEmpty(value)||value.Length!=LiveOpsCycleRequirements.Sha256HexLength)return false;
            for(int i=0;i<value.Length;i++){char c=value[i];if(!((c>='0'&&c<='9')||(c>='a'&&c<='f')||(c>='A'&&c<='F')))return false;}return true;
        }
        static string[] Unique(List<string> values){var result=new List<string>();for(int i=0;i<values.Count;i++)if(!result.Contains(values[i]))result.Add(values[i]);return result.ToArray();}
    }
}
