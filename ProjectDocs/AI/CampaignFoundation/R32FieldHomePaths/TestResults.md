# R32 — Field-to-home rounds181–190 integrated

**289 Unity tests passed;0 failures;0 skipped.** Unity6000.5.9f1, Verify01. [Verification](QA/Verification.json) · [Content checks](QA/ContentChecks.json) · [Replay screening](QA/DifficultyScreening.json).

|Suite|Passed|Failed|
|---|---:|---:|
|Active|208|0|
|Campaign|10|0|
|Compatibility|47|0|
|Checkpoint|24|0|

Normal gameplay now includes1–190 through the existing CampaignActive resources, matching Chapter01Sector04R32 exactly. First180 records and all previous story/locales are preserved. No runtime/UI/art/settings changes.

## Board completion

570/570 deterministic starts have legal opening moves and no automatic matches.570/570 inventory-free completion replays pass, three attempt seeds perround. All540 prior replays match R31 exactly. These legal solver solutions are **not human win rates or difficulty acceptance**. Budgets, seeds and layouts were not adjusted to force successful tests.

|Round|Move limit|Completion moves, attempts1/2/3|
|---|---:|---|
|181|34|3 / 3 / 3|
|182|29|7 / 6 / 5|
|183|29|5 / 4 / 4|
|184|29|6 / 5 / 5|
|185|28|18 / 7 / 10|
|186|34|4 / 4 / 3|
|187|29|5 / 5 / 4|
|188|28|18 / 18 / 18|
|189|29|4 / 7 / 5|
|190|30|12 / 12 / 11|

Recovery181/186, hard185/190 and eighteen-consumed-move survival188 retain their authored budgets. Installed geometry/goals/rewards match RoundBriefs.json.

## Progression and story

- Normal pooled map controls→PreLevel→Gameplay→Victory→dialogue/map pass for180/181/185/188/190 using actual board tap/swipe APIs. Energy deduction, rewards and story acknowledgement pass. This is automated control execution, not visual or physical-pointer acceptance.
- Sixteen completed-save migration boundaries10/40/50/60/70/80/90/100/110/120/130/140/150/160/170/180 preserve balances, energy and progress and unlock the next round. Earlier progress is an explicit fixture.
- Round190 replay/reload produces no duplicate gold, gems or any of the four boosters. First-clear181–190 totals1840gold/6gems/1LivingLight, with no separate story reward.
- Checkpoints50–190 interruption/reload/single acknowledgement and all26 pending milestones pass without economic mutation. Failed checkpoint/retry fixtures50/100/150/160/170/180/190 persist attempts/failures without rewards, unlocks, dialogue or false chapter completion.
- Round191 and Chapter2 remain unavailable; Chapter1 finale remains planned500.

[Story audit](QA/ContentAudit.json):27 referenced scenes, 26 milestones, 380 outcome reactions and 421 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes. The field_home_paths scene links the new field-to-home access to remaining bypass checks. No harvest/helper/production state is claimed.

## Limits and scope

Tests ran with isolated saves and the prior approved process-only Vulkan/descriptor4096 workaround. Live player saves and project graphics settings were untouched. Android build/device, full-project suite and new visual/pointer checks were not run. Default manual exports contain5 valid recordings covering11–30 only; human31–190 difficulty remains pending. External folders were not searched. [Manual evidence](QA/ManualEvidence.json).

The historical R22 gaps remain open: optional returning-player recap is not implemented/user-validated, the current backlog can contain26 scenes, and older generic reaction wording still needs editorial review. Passing persistence and structural checks does not establish pacing or literary quality. Animation/effect work remains last.

[Scope](QA/Scope.json):8 modified projectfiles (four activeJSONs/fourtests),9 candidate/meta additions,0 deletions,7632 unchanged. Task-start snapshots/scripts/test patch retained; temporary test scene/linker cleanup complete. Brief parity, source syntax/whitespace and protected-file checks pass. No commit.

Next: author bounded191–200 local orchard/field consolidation with recovery191/196 and varied familiar tasks, preserving1–190. Validate190 migration and200 reward/story/reload bounds. Review the200-round checkpoint and outstanding evidence before expanding further. No bulkfill or solver-only budget tightening.
