namespace LegendsOfRus.ContentPipeline
{
    public static class ChapterMigrationBlockerCodes
    {
        public const string EvidenceMissing = "dry_run_evidence_missing";
        public const string SchemaInvalid = "dry_run_schema_invalid";
        public const string DryRunIdInvalid = "dry_run_id_invalid";
        public const string PackageMissing = "canonical_package_missing";
        public const string PackageSchemaInvalid = "canonical_package_schema_invalid";
        public const string PackageIdentityInvalid = "canonical_package_identity_invalid";
        public const string PackageManifestHashInvalid = "package_manifest_hash_invalid";
        public const string DefinitionHashInvalid = "definition_hash_invalid";
        public const string PayloadCountInvalid = "copied_payload_count_invalid";
        public const string PayloadMissing = "copied_payload_missing";
        public const string PayloadPathInvalid = "copied_payload_path_invalid";
        public const string PayloadHashInvalid = "copied_payload_hash_invalid";
        public const string PayloadHashMismatch = "copied_payload_hash_mismatch";
        public const string PayloadByteCountInvalid = "copied_payload_byte_count_invalid";
        public const string LevelCountInvalid = "chapter01_level_count_invalid";
        public const string StoryMilestoneCountInvalid = "chapter01_story_milestone_count_invalid";
        public const string LocalizationEntryCountInvalid = "chapter01_localization_entry_count_invalid";
        public const string LegacyAuthorityLost = "legacy_runtime_authority_lost";
        public const string RuntimeCutoverOccurred = "runtime_cutover_occurred";
        public const string AddressablesMutated = "addressables_mutated";
        public const string LockedArtTouched = "locked_art_touched";
        public const string AutomaticArtGenerationUsed = "automatic_art_generation_used";
        public const string LocalizationRewritten = "localization_rewritten";
        public const string LevelBalanceChanged = "level_balance_changed";
        public const string RewardsChanged = "rewards_changed";
        public const string ProgressionChanged = "progression_changed";
        public const string CriticalBlockersOpen = "dry_run_critical_blockers_open";
    }
}
