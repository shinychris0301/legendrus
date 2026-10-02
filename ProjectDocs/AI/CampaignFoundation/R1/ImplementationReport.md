# Campaign Foundation R1

The approved 20-chapter × 500-round direction is now represented by a versioned campaign manifest and a compiled, tested progression/migration foundation. The live game deliberately continues using its accepted ten-level Chapter 1 until the content adapter and atomic save migration are ready.

## Delivered

- Canonical v2 manifest: 20 approved RU/EN chapter names, ordered prerequisites, 500 rounds/chapter, 50-round sectors, finale at round 500, stable IDs and content revision.
- Pure `CampaignContract` validator: rejects malformed scope, IDs, ordering, names, dependencies and versions.
- `CampaignProgression`: admits validated package metadata; handles partial availability, contiguous wins, replay access, chapter gates, published/acknowledged finales and campaign termination. It takes snapshots so caller mutations cannot change validated state.
- `CampaignSaveMigration.Plan`: non-writing projection of v6 progress, copied counters/claims, preserved legacy story state, distinct legacy finale acknowledgement, and blocking for unknown/duplicate records.
- Runtime-cutover design, first-sector authoring gate and save migration/rollback specification.

## Scope and limits

This is a **pre-activation foundation**, not 10,000 new levels or an installed Chapter 2. The tests use explicitly synthetic package/progress fixtures. The package adapter still needs to validate actual LevelData/StoryData/localization assets before supplying metadata to this core.

No GameState/SaveService/catalog/router/prefab/art changes, currency grants, migrations of user data, provider activation or Android release. The v1 100-level policy and exactly-ten legacy validators remain intact because their pinned verification contract is still in use. New campaign v2 supports 500; changing the active runtime requires its separate adapter and save-codec work.

## Next implementation

**Campaign Foundation R2 — validated local package adapter and migration rehearsal.** Load actual chapter data through the existing catalog boundary in a development-only fixture, separate the legacy ten-level teaching validator from generic campaign validation, and exercise Chapter 1/Chapter 2 selection and missing-content handling. Prepare a candidate save codec transaction and interrupted-write tests before any live cutover. First settle whether the accepted level-ten finale remains demo-only or maps to a revised campaign milestone; no accepted dialogue was rewritten here.

Then author and balance the first 50-round sector. No mass generation or art redesign is needed for the foundation work.

## Files

- `Assets/LegendsOfRus/Scripts/ContentPipelineCore/CampaignFoundation.cs` and new GUID/meta.
- `Assets/LegendsOfRus/Tests/ContentPipelineEditMode/CampaignFoundationTests.cs` and new GUID/meta.
- `ProjectDocs/ContentPipeline/CampaignV2/{campaign_definition.json,README.md,SaveMigration.md}`.
- This review's verification scripts, logs, XML, scope evidence and dashboard; CurrentTask and SessionLog.

## Verification

Results are recorded in QA/Verification.json. Unity 6000.5.9f1 content-pipeline EditMode run: 128 passed, none failed/skipped. Includes existing v1 pipeline/migration regressions and new v2 foundation cases. Initial PlayMode filter used the wrong namespace and discovered zero tests; it is retained as invalid evidence, not counted as a pass. Verify02 corrected the namespace but failed both tests because the required capture-directory environment variable was missing. Verify03 supplied that directory and a graphics context: both real win/fail native journeys passed (2/2), zero skips. Retained earlier attempts for transparency. 24 of the 128 EditMode cases are new campaign foundation cases.

No physical Android, backend, real save migration, all-level balance test or full-project test suite is claimed.

Scope hashing: 7225 pre-existing Assets/Packages/ProjectSettings files unchanged; exactly four new source/test/meta files, no changed or deleted pre-existing files. Unity removed temporary Addressables build-link files during initialization; both were restored to their exact pre-run SHA-256.
