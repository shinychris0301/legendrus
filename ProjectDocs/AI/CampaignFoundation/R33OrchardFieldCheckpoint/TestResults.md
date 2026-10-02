# R33 — Orchard/field checkpoint191–200 integrated

**302 Unity tests passed;0 failures;0 skipped.** Unity6000.5.9f1, Verify01. [Verification](QA/Verification.json) · [Content checks](QA/ContentChecks.json) · [Replay screening](QA/DifficultyScreening.json).

|Suite|Passed|Failed|
|---|---:|---:|
|Active|219|0|
|Campaign|10|0|
|Compatibility|47|0|
|Checkpoint|26|0|

Normal gameplay now includes1–200 through existing CampaignActive resources, matching Chapter01Sector04R33 exactly. First190 records and all previous story/locales are preserved. No runtime/UI/art/settings changes.

## Board completion

600/600 deterministic starts have legal opening moves and no automatic matches.600/600 inventory-free completion replays pass, three attempt seeds perround. All570 earlier replays match R32 exactly. These legal solver solutions are **not human win rates or difficulty acceptance**. Budgets, seeds and layouts were not adjusted to force test success.

|Round|Move limit|Completion moves, attempts1/2/3|
|---|---:|---|
|191|34|4 / 3 / 3|
|192|29|5 / 4 / 4|
|193|29|4 / 6 / 4|
|194|29|6 / 5 / 6|
|195|28|7 / 7 / 8|
|196|34|3 / 2 / 4|
|197|29|6 / 4 / 5|
|198|28|18 / 18 / 18|
|199|29|4 / 5 / 6|
|200|30|6 / 7 / 14|

Recovery191/196, hard195/200 and eighteen-consumed-move survival198 retain authored budgets. Installed geometry/goals/rewards match RoundBriefs.json.

## Progression and story

- Normal pooled map controls→PreLevel→Gameplay→Victory→dialogue/map pass for190/191/195/198/200 using actual board tap/swipe APIs. Energy, rewards and story acknowledgement pass. This is automated control execution, not visual or physical-pointer acceptance.
- Seventeen completed-save migration boundaries10/40/50/60/70/80/90/100/110/120/130/140/150/160/170/180/190 preserve balances, energy and progress and unlock the next round. Earlier progress is an explicit fixture.
- Round200 replay/reload produces no duplicate currency or any of the four boosters. First-clear191–200 totals1840gold/6gems/1SeaPearl, with no separate sector/story reward.
- Checkpoints50–200 interruption/reload/single acknowledgement and all27 pending milestones pass without economic mutation. Failed checkpoint/retry fixtures50/100/150/160/170/180/190/200 retain failure data without rewards, unlocks, dialogue or false chapter completion.
- Round201 and Chapter2 remain unavailable; Chapter1 finale remains planned500.

[Story audit](QA/ContentAudit.json):28 referenced scenes, 27 milestones, 400 outcome reactions and 442 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes. The orchard_field_paths scene concludes local access and points toward the river shadow and old village wards, following the earlier outline. It does not install a ward/harvest/helper/production system or reveal the raven's identity.

Sector151–200 first-clear total:9200gold/30gems/1Mace/1Firebird/1LivingLight/2SeaPearls. No additional checkpoint reward.

## Limits and scope

Tests used isolated saves and prior approved process-only Vulkan/descriptor4096. Live saves and project graphics settings were untouched. Android build/device, full-project suite and new visual/pointer checks were not run. Default manual exports contain5 valid recordings covering11–30 only; human31–200 difficulty remains pending. External folders were not searched. [Manual evidence](QA/ManualEvidence.json).

The historical R22 gaps remain open. Optional returning-player recap is unimplemented/user-unvalidated; the backlog can contain27 milestones. Earlier generic reactions still need editorial review. Persistence/structural passes do not establish pacing or literary quality. Animation/effect work remains last.

[Scope](QA/Scope.json):8 modified projectfiles (four activeJSONs/fourtests),9 candidate/meta additions,0 deletions,7641 unchanged. Task-start snapshots/scripts/test patch retained; temporary test scene/linker cleanup complete. Brief parity, Python syntax/test whitespace and protected-file checks pass. No commit.

Next: keep normal content at200 while reviewing the checkpoint, remaining human/device evidence, reaction repetition and returning-player backlog UX. The201–250 river/ward sector remains outline only; a distinct puzzle proposition and concrete201–210 briefs must precede implementation. Do not bulk-fill the next sector or tighten difficulty from solver results alone.
