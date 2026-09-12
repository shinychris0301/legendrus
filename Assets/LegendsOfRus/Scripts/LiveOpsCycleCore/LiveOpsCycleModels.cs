using System;

namespace LegendsOfRus.LiveOpsCycle
{
    public static class LiveOpsCycleBlockerCodes
    {
        public const string RetrospectiveMissing="LOR_RETRO_MISSING";
        public const string RetrospectiveNotReviewed="LOR_RETRO_NOT_REVIEWED";
        public const string RetrospectiveInvalid="LOR_RETRO_INVALID";
        public const string RetrospectiveSampleInsufficient="LOR_RETRO_SAMPLE_INSUFFICIENT";
        public const string RetrospectiveObjectivesMissing="LOR_RETRO_OBJECTIVES_MISSING";
        public const string RetrospectiveMetricsMissing="LOR_RETRO_METRICS_MISSING";
        public const string RetrospectiveIncidentLessonsMissing="LOR_RETRO_INCIDENT_LESSONS_MISSING";
        public const string RetrospectiveContentLessonsMissing="LOR_RETRO_CONTENT_LESSONS_MISSING";
        public const string RetrospectiveEconomyLessonsMissing="LOR_RETRO_ECONOMY_LESSONS_MISSING";
        public const string RetrospectiveSupportLessonsMissing="LOR_RETRO_SUPPORT_LESSONS_MISSING";
        public const string RetrospectivePrivacyLessonsMissing="LOR_RETRO_PRIVACY_LESSONS_MISSING";
        public const string RetrospectiveCriticalLessonsOpen="LOR_RETRO_CRITICAL_LESSONS_OPEN";
        public const string RetrospectiveActionItemsInvalid="LOR_RETRO_ACTION_ITEMS_INVALID";
        public const string RetrospectiveOwnersMissing="LOR_RETRO_OWNERS_MISSING";
        public const string RetrospectiveDueDatesMissing="LOR_RETRO_DUE_DATES_MISSING";
        public const string RetrospectiveArchiveInputsNotPinned="LOR_RETRO_ARCHIVE_INPUTS_NOT_PINNED";
        public const string ArchiveMissing="LOR_ARCHIVE_MISSING";
        public const string ArchiveNotReviewed="LOR_ARCHIVE_NOT_REVIEWED";
        public const string ArchiveInvalid="LOR_ARCHIVE_INVALID";
        public const string ArchiveHashesInvalid="LOR_ARCHIVE_HASHES_INVALID";
        public const string ArchiveImmutableStorageMissing="LOR_ARCHIVE_IMMUTABLE_STORAGE_MISSING";
        public const string ArchivePiiReviewFailed="LOR_ARCHIVE_PII_REVIEW_FAILED";
        public const string ArchiveSecretsReviewFailed="LOR_ARCHIVE_SECRETS_REVIEW_FAILED";
        public const string ArchiveManifestNotPinned="LOR_ARCHIVE_MANIFEST_NOT_PINNED";
        public const string CadenceMissing="LOR_CADENCE_MISSING";
        public const string CadenceNotReviewed="LOR_CADENCE_NOT_REVIEWED";
        public const string CadenceInvalid="LOR_CADENCE_INVALID";
        public const string CadenceLeadTimeInsufficient="LOR_CADENCE_LEAD_TIME_INSUFFICIENT";
        public const string CadenceTooManyEvents="LOR_CADENCE_TOO_MANY_EVENTS";
        public const string CadenceTooManyExperiments="LOR_CADENCE_TOO_MANY_EXPERIMENTS";
        public const string CadenceConcurrencyInvalid="LOR_CADENCE_CONCURRENCY_INVALID";
        public const string CadenceCooldownInsufficient="LOR_CADENCE_COOLDOWN_INSUFFICIENT";
        public const string CadenceCalendarConflict="LOR_CADENCE_CALENDAR_CONFLICT";
        public const string CadenceDependenciesMissing="LOR_CADENCE_DEPENDENCIES_MISSING";
        public const string CadenceCapacityMissing="LOR_CADENCE_CAPACITY_MISSING";
        public const string CadenceLocalizationMissing="LOR_CADENCE_LOCALIZATION_MISSING";
        public const string CadenceSupportCoverageMissing="LOR_CADENCE_SUPPORT_COVERAGE_MISSING";
        public const string CadenceContentReadinessMissing="LOR_CADENCE_CONTENT_READINESS_MISSING";
        public const string CadenceMeasurementPlanMissing="LOR_CADENCE_MEASUREMENT_PLAN_MISSING";
        public const string CadenceCarryoverBlockers="LOR_CADENCE_CARRYOVER_BLOCKERS";
        public const string ProductionAnalyticsDisabled="LOR_CYCLE_PROD_ANALYTICS_DISABLED";
        public const string ProductionRemoteConfigDisabled="LOR_CYCLE_PROD_REMOTE_CONFIG_DISABLED";
        public const string RetrospectiveReviewNotAuthorized="LOR_RETRO_REVIEW_NOT_AUTHORIZED";
        public const string ArchiveReviewNotAuthorized="LOR_ARCHIVE_REVIEW_NOT_AUTHORIZED";
        public const string NextCadenceReviewNotAuthorized="LOR_NEXT_CADENCE_REVIEW_NOT_AUTHORIZED";
        public const string ManualCycleClosureNotAuthorized="LOR_CYCLE_CLOSURE_NOT_AUTHORIZED";
        public const string LiveOpsPublishingNotAuthorized="LOR_CYCLE_LIVEOPS_PUBLISH_NOT_AUTHORIZED";
    }

    public enum LiveOpsCycleRecommendation
    {
        InsufficientEvidence=0,
        ResolveRetrospective=1,
        CompleteArchive=2,
        ReviseNextCadence=3,
        ReadyForManualCycleClosure=4
    }

    [Serializable]
    public sealed class CycleRetrospectiveEvidence
    {
        public int SchemaVersion=LiveOpsCycleRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string RetrospectiveId=string.Empty;
        public string CycleId=string.Empty;
        public string OutcomeId=string.Empty;
        public string ExperimentDecisionId=string.Empty;
        public string RotationCloseoutId=string.Empty;
        public int ParticipantsReviewed;
        public bool ObjectivesReviewed;
        public bool SuccessMetricsReviewed;
        public bool IncidentLessonsReviewed;
        public bool ContentLessonsReviewed;
        public bool EconomyLessonsReviewed;
        public bool SupportLessonsReviewed;
        public bool PrivacyLessonsReviewed;
        public int ActionItemCount;
        public bool ActionOwnersAssigned;
        public bool ActionDueDatesAssigned;
        public int UnresolvedCriticalLessons;
        public bool ArchiveInputsPinned;
    }

    [Serializable]
    public sealed class CycleArchiveEvidence
    {
        public int SchemaVersion=LiveOpsCycleRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string ArchiveId=string.Empty;
        public string CycleId=string.Empty;
        public string SourceTreeSha256=string.Empty;
        public string OutcomeSha256=string.Empty;
        public string ExperimentDecisionSha256=string.Empty;
        public string RotationCloseoutSha256=string.Empty;
        public string RetrospectiveSha256=string.Empty;
        public string ArchiveManifestSha256=string.Empty;
        public bool ImmutableStorageReviewed;
        public bool PiiReviewPassed;
        public bool SecretsReviewPassed;
        public bool ArchiveManifestPinned;
    }

    [Serializable]
    public sealed class NextCadencePlanEvidence
    {
        public int SchemaVersion=LiveOpsCycleRequirements.SchemaVersion;
        public bool EvidenceRetained;
        public bool EvidenceReviewed;
        public string PlanId=string.Empty;
        public string PriorCycleId=string.Empty;
        public int LeadTimeMinutes;
        public int PlannedEvents;
        public int PlannedExperiments;
        public int MaxConcurrentEvents;
        public int MinimumCooldownMinutes;
        public int CalendarConflictCount;
        public bool DependenciesReviewed;
        public bool CapacityReviewed;
        public bool LocalizationReviewed;
        public bool SupportCoverageReviewed;
        public bool ContentReady;
        public bool MeasurementPlanReviewed;
        public bool NoCarryoverBlockers;
    }

    [Serializable]
    public sealed class RetrospectiveReviewResult
    {
        public bool StructurallyValid;
        public bool ReadyForArchive;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class ArchiveReviewResult
    {
        public bool StructurallyValid;
        public bool ArchiveReady;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class NextCadenceReviewResult
    {
        public bool StructurallyValid;
        public bool CadenceReady;
        public string[] Blockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsCycleReviewResult
    {
        public bool ReadyForHumanReview;
        public bool MayCloseCycleManually;
        public bool MayPublishNextCadence;
        public LiveOpsCycleRecommendation Recommendation;
        public string[] ReviewBlockers=Array.Empty<string>();
        public string[] ActionBlockers=Array.Empty<string>();
    }

    [Serializable]
    public sealed class LiveOpsCycleAggregateObservation
    {
        public int WindowIndex;
        public int Participants;
        public int CompletionBasisPoints;
        public int OpenIncidentCount;
        public int RewardMismatchCount;
        public int RetrospectiveActionItems;
    }
}
