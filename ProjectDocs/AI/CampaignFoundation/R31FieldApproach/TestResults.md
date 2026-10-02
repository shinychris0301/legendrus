# R31 — Field-approach rounds171–180 integrated

**276 Unity tests passed;0 failures;0 skipped.** Unity6000.5.9f1, Verify01. [Verification](QA/Verification.json) · [Content checks](QA/ContentChecks.json) · [Replay screening](QA/DifficultyScreening.json).

|Suite|Passed|Failed|
|---|---:|---:|
|Active|197|0|
|Campaign|10|0|
|Compatibility|47|0|
|Checkpoint|22|0|

Normal gameplay now includes1–180 through the existing CampaignActive resources, matching Chapter01Sector04R31 exactly. First170 levels and all previous story/locales are preserved. No runtime/UI/art/settings changes.

## Board completion

540/540 deterministic starts have legal opening moves and no automatic matches.540/540 inventory-free completion replays pass, three attempt seeds perround. All510 prior replays are identical to R30. These legal solver solutions are **not human win rates or difficulty acceptance**. Move budgets were not tightened to match solver performance.

|Round|Move limit|Completion moves, attempts1/2/3|
|---|---:|---|
|171|34|3 / 3 / 3|
|172|29|4 / 5 / 5|
|173|29|5 / 4 / 5|
|174|29|5 / 5 / 6|
|175|28|11 / 8 / 12|
|176|34|3 / 4 / 4|
|177|29|5 / 5 / 4|
|178|28|18 / 18 / 18|
|179|29|7 / 4 / 4|
|180|30|7 / 10 / 8|

Recovery171/176, hard175/180 and eighteen-consumed-move survival178 retain the authored budgets. All ten installed layouts/seeds/goals/rewards match RoundBriefs.json.

## Progression and story

- Normal pooled map controls→PreLevel→Gameplay→Victory→dialogue/map pass for170/171/175/178/180 using actual board tap/swipe APIs. Energy deduction, rewards and story acknowledgement pass. This is automated control execution, not a physical-pointer or visual acceptance audit.
- Fifteen completed-save migration boundaries10/40/50/60/70/80/90/100/110/120/130/140/150/160/170 preserve balances, energy and progress and unlock the next round. Earlier progress is an explicit fixture.
- 180 replay/reload produces no duplicate gold, gems or any of the four boosters. First-clear171–180 totals1840gold/6gems/1Firebird, with no separate scene reward.
- Checkpoint50–180 interruption/reload/single acknowledgement and all25 pending milestones pass without economic mutation. Failed checkpoint/retry fixtures50/100/150/160/170/180 persist attempts/failures without rewards, unlocks, dialogue or false chapter completion.
- 181 and Chapter2 remain unavailable. Chapter1 finale remains planned500.

[Story audit](QA/ContentAudit.json):26 referenced scenes, 25 milestones, 360 outcome reactions and 400 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes. The field_approach scene links orchard-to-field access to the next paths toward homes. No harvest/helper/production state is claimed.

## Limits and scope

Tests ran with isolated saves and the prior approved process-only Vulkan/descriptor4096 workaround. Live player saves and project graphics settings were untouched. Android build/device, full-project suite and new visual/pointer checks were not run. Default manual exports still contain5 valid recordings covering11–30 only; human31–180 difficulty remains pending. External folders were not searched. [Manual evidence](QA/ManualEvidence.json).

[Scope](QA/Scope.json):8 modified projectfiles (four activeJSONs/fourtests),9 candidate/meta additions,0 deletions,7623 unchanged. Task-start snapshots, scripts and test patch retained. Temporary test scenes/linker cleanup done; source syntax/whitespace, brief parity and protected-file checks pass. No commit.

Next: author bounded181–190 field-to-home paths with recovery181/186 and mixed familiar tasks, preserving1–180. Validate the190 checkpoint and180-save transition before moving further. No bulkfill or solver-only budget tightening. Human/device evidence, optional returning-player recap and older generic reactions remain open. Animations/effects remain last.
