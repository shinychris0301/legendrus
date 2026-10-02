# R30 — Orchard-row rounds161–170 integrated

**263 Unity tests passed;0 failures;0 skipped.** Unity6000.5.9f1, Verify01. [Verification](QA/Verification.json) · [Content checks](QA/ContentChecks.json) · [Replay screening](QA/DifficultyScreening.json).

|Suite|Passed|Failed|
|---|---:|---:|
|Active|186|0|
|Campaign|10|0|
|Compatibility|47|0|
|Checkpoint|20|0|

Normal gameplay now includes1–170 through the existing CampaignActive resources, matching Chapter01Sector04R30 exactly. First160 levels and all previous story/locales are preserved. No runtime/UI/art/settings changes.

## Board completion

510/510 deterministic starts have legal opening moves and no automatic matches.510/510 inventory-free completion replays pass, three attempt seeds perround. All480 prior replays are identical to R29. These legal solver solutions are **not human win rates or difficulty acceptance**. Move budgets were not tightened to match solver performance.

|Round|Move limit|Completion moves, attempts1/2/3|
|---|---:|---|
|161|34|3 / 4 / 3|
|162|29|5 / 3 / 3|
|163|29|4 / 4 / 4|
|164|29|3 / 5 / 5|
|165|28|9 / 9 / 12|
|166|34|4 / 2 / 3|
|167|29|7 / 5 / 8|
|168|29|5 / 3 / 7|
|169|28|18 / 18 / 18|
|170|30|10 / 7 / 9|

Recovery161/166, hard165/170 and eighteen-consumed-move survival169 retain the authored budgets. All ten installed layouts/seeds/goals/rewards match RoundBriefs.json.

## Progression and story

- Normal pooled map controls→PreLevel→Gameplay→Victory→dialogue/map pass for160/161/165/169/170 using actual board tap/swipe APIs. Energy deduction, rewards and story acknowledgement pass. This is automated control execution, not a physical-pointer or visual acceptance audit.
- Fourteen completed-save migration boundaries10/40/50/60/70/80/90/100/110/120/130/140/150/160 preserve balances, energy and progress and unlock the next round. Earlier progress is an explicit fixture.
-170replay/reload produces no duplicate gold, gems or any of the four boosters. First-clear161–170 totals1840gold/6gems/1Mace, with no separate scene reward.
- Checkpoint50–170 interruption/reload/single acknowledgement and all24 pending milestones pass without economic mutation. Failed checkpoint/retry fixtures50/100/150/160/170 persist attempts/failures without rewards, unlocks, dialogue or false chapter completion.
-171 and Chapter2 remain unavailable. Chapter1 finale remains planned500.

[Story audit](QA/ContentAudit.json):25 referenced scenes, 24 milestones, 340 outcome reactions and 379 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes. The orchard_paths scene links completed orchard access to the next field approach. No harvest/helper/production state is claimed.

## Limits and scope

Tests ran with isolated saves and the prior approved process-only Vulkan/descriptor4096 workaround. Live player saves and project graphics settings were untouched. Android build/device, full-project suite and new visual/pointer checks were not run. Default manual exports still contain5 valid recordings covering11–30 only; human31–170 difficulty remains pending. External folders were not searched. [Manual evidence](QA/ManualEvidence.json).

[Scope](QA/Scope.json):8 modified projectfiles (four activeJSONs/fourtests),9 candidate/meta additions,0 deletions,7614 unchanged. Task-start snapshots, scripts and test patch retained. Temporary test scenes/linker cleanup done; source syntax/whitespace, brief parity and protected-file checks pass. No commit.

Next: author bounded171–180 field-approach rounds with recovery171/176 and distinct open collection/separated layered gates, preserving1–170. Validate the180 checkpoint and170-save transition before moving further. No bulkfill or solver-only budget tightening. Human/device evidence, optional returning-player recap and older generic reactions remain open. Animations/effects remain last.
