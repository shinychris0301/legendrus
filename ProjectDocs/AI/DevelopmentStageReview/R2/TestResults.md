# R2 interim review — verification results

Date: 2026-09-29. Unity 6000.5.9f1. Existing tests; no product/test-source changes. Fresh isolated `/tmp/lor-stage-review-r2` saves, prior user-authorized **process-only Vulkan**, descriptor limit 4096. No graphics/project settings or activation manifests changed.

| Run | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| Commercial foundation + runtime Edit Mode | 124 | 0 | 0 | [XML](QA/CommercialEditMode/results.xml), [command/result](QA/CommercialEditMode/summary.json) |
| Commercial runtime wiring + functional navigation Play Mode | 20 | 0 | 2 | [XML](QA/LiveReadinessPlayMode/results.xml), [command/result](QA/LiveReadinessPlayMode/summary.json) |
| Separate-process persistence writer | 1 | 0 | 0 | [XML](QA/PersistenceWrite/results.xml), [command/result](QA/PersistenceWrite/summary.json) |
| Separate-process persistence reader | 1 | 0 | 0 | [XML](QA/PersistenceRead/results.xml), [command/result](QA/PersistenceRead/summary.json) |

**146 distinct tests passed across these runs.** The two skipped runtime cases were the writer/reader, intentionally requiring separate process phases; both then passed in those phases. No unresolved skip in this focused scope. This is not a new full-project combined run.

The fresh checks confirm current disabled-provider/activation behavior, candidate-only commercial validation, runtime wiring, native route journeys and separate-process local save restoration without duplicate first-clear reward. They do **not** test real account sign-in, live purchases/ad delivery, a server, multi-player social behavior, cloud merge or Android hardware.

Static inventory: **8 checks passed**, covering 36 native prefabs, 20×500 definition, installed contiguous 1–200, no premature finale, populated unique RU/EN story entries and explicit-key coverage, disabled commercial/social authorities and preservation of the known R43 failure count. [Checks](QA/InventoryChecks.json), [inventory](QA/CurrentInventory.json), [reusable script](Sources/AuditInventory.py). The script checks 442 explicit story/level key references; it is not a replacement for Unity's broader catalog validators or an all-page localization audit.

Fresh default manual-recording check: five parseable files, none covering 31–200. Historical development APK exists; presence and size only were checked. [Manual scope](QA/ManualEvidence.json), [APK scope](QA/BuildArtifact.json). No personal gameplay save was modified.

## Earlier evidence retained, not rerun in full

- R43 full Edit Mode: 1,917 passed, zero failed/skipped.
- R43 latest-case Play Mode aggregate: 857 passed / **40 failed**. This combines runs and is not one clean post-fix full suite. Current review does not clear those failures.
- 600 R43 legal completion witnesses for installed 200 rounds. Automated reachability, not human difficulty.
- Historical development APK and static Android policy/package checks; no current device acceptance.

## Scope and limitations

Source extraction, review documents, revised canonical schedule, inventory/check scripts and verification artifacts were added. Product Assets, Packages and ProjectSettings are protected by task-start hashes and final [Scope.json](QA/Scope.json); known generated test-scene/linker artifacts are cleaned using the retained baseline. Existing working-tree edits remain intact. No new service, SDK, provider, production activation, paid grant, art, level, rule, animation or effect was installed in this review. No commit or release.

Actual runtime fixture captures are under each run's `Captures` folder. They are not a new all-page visual acceptance or evidence of online functionality. Full remaining gaps and next work package are in [DevelopmentStageReview.md](DevelopmentStageReview.md) and [LiveServicesFirst.md](LiveServicesFirst.md).
