namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterPipelineBlockerCodes
    {
        public const string ManifestMissing = "manifest_missing";
        public const string ManifestSchemaInvalid = "manifest_schema_invalid";
        public const string ManifestIdMissing = "manifest_id_missing";
        public const string ChapterCountInvalid = "chapter_count_invalid";
        public const string ChapterEntryMissing = "chapter_entry_missing";
        public const string ChapterIdInvalid = "chapter_id_invalid";
        public const string ChapterIdDuplicate = "chapter_id_duplicate";
        public const string ChapterOrderInvalid = "chapter_order_invalid";
        public const string StableIdInvalid = "stable_id_invalid";
        public const string StableIdDuplicate = "stable_id_duplicate";
        public const string DisplayNameKeyMissing = "display_name_key_missing";
        public const string AddressMissing = "address_missing";
        public const string AddressPrefixInvalid = "address_prefix_invalid";
        public const string AddressDuplicate = "address_duplicate";
        public const string CanonicalFolderInvalid = "canonical_folder_invalid";
        public const string LegacySourceBoundaryInvalid = "legacy_source_boundary_invalid";
        public const string OwnershipIncomplete = "ownership_incomplete";
        public const string TemplateMissing = "template_missing";
        public const string TemplateSchemaInvalid = "template_schema_invalid";
        public const string TemplateIdentityInvalid = "template_identity_invalid";
        public const string TemplateLevelCountInvalid = "template_level_count_invalid";
        public const string TemplateFinaleInvalid = "template_finale_invalid";
        public const string TemplateMilestoneInvalid = "template_milestone_invalid";
        public const string EvidenceMissing = "transition_evidence_missing";
        public const string EvidenceNotReviewed = "transition_evidence_not_reviewed";
        public const string TransitionIdMissing = "transition_id_missing";
        public const string SourceHashInvalid = "source_hash_invalid";
        public const string LegacyValidationMissing = "legacy_validation_missing";
        public const string ManifestReviewMissing = "manifest_review_missing";
        public const string TemplateReviewMissing = "template_review_missing";
        public const string StableIdReviewMissing = "stable_id_review_missing";
        public const string AddressablesReviewMissing = "addressables_review_missing";
        public const string OwnershipReviewMissing = "ownership_review_missing";
        public const string LocalizationReviewMissing = "localization_review_missing";
        public const string QaReviewMissing = "qa_review_missing";
        public const string MigrationPlanMissing = "migration_plan_missing";
        public const string LockedArtBoundaryMissing = "locked_art_boundary_missing";
        public const string CriticalBlockersOpen = "critical_blockers_open";
        public const string ChapterMigrationNotAuthorized = "chapter_migration_not_authorized";
        public const string AddressablesMutationNotAuthorized = "addressables_mutation_not_authorized";
        public const string RuntimeCutoverNotAuthorized = "runtime_cutover_not_authorized";
        public const string AutomaticArtGenerationNotAuthorized = "automatic_art_generation_not_authorized";
        public const string LockedArtOverwriteNotAuthorized = "locked_art_overwrite_not_authorized";
    }
}
