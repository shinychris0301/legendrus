# R29 — Orchard entrance rounds151–160 integrated

**250 Unity tests passed;0 failures;0 skipped.** Unity6000.5.9f1, Verify01. [Verification](QA/Verification.json) · [Content checks](QA/ContentChecks.json) · [Replay screening](QA/DifficultyScreening.json).

|Suite|Passed|Failed|
|---|---:|---:|
|Active campaign/resources, completion replays, migration and normal routes|175|0|
|Campaign board/UI integration|10|0|
|Level/rule/story/finale compatibility|47|0|
|Checkpoint persistence, acknowledgements, failed retries and boundaries|18|0|
|Total|250|0|

Normal gameplay now includes1–160 using Chapter01Sector04R29 through existing CampaignActive. Exact first150 levels and all earlier story/localization entries are preserved. New content matches the R28 briefs without budget, seed, layout or reward changes. No runtime/UI/art/settings changes.

## Board completion and difficulty screening

480/480 deterministic starts have legal moves and no opening matches.480/480 inventory-free completion replays passed across160 rounds, three attempt seeds each. All450 prior replays are identical to R27. New rounds complete in2–18 moves; these are legal solver solutions, **not human win rates or difficulty acceptance**.

|Round|Move limit|Completion moves, attempts1/2/3|
|---|---:|---|
|151|34|4 / 3 / 2|
|152|29|5 / 5 / 4|
|153|29|5 / 5 / 4|
|154|29|9 / 7 / 5|
|155|28|7 / 8 / 9|
|156|34|3 / 3 / 4|
|157|29|4 / 6 / 4|
|158|28|18 / 18 / 18|
|159|29|7 / 6 / 7|
|160|30|10 / 10 / 8|

No tuning was applied simply to shorten solver results. Survival158 correctly requires18 consumed moves, not elapsed time. Recovery151/156 and local peaks155/160 retain their drafted budgets.

## Integration and persistence

- Normal pooled map buttons→PreLevel→Gameplay→Victory→dialogue/map passed for150/151/155/158/160 with actual board tap/swipe APIs, existing rules, energy deduction and reward checks. This is automated native-control execution, not visual or physical-pointer acceptance.
- Thirteen earlier completed-save boundaries10/40/50/60/70/80/90/100/110/120/130/140/150 preserve currency, energy and progress during migration and unlock the next round. Earlier progress is an explicit fixture.
-160 replay/reload grants no duplicate gold, gems or any of the four boosters. First-clear151–160 totals1840gold,6gems,1SeaPearl; no separate story reward.
- Unread checkpoints50–160 survive interruption/reload and acknowledge once. All23 pending milestones remain ordered with no economic mutation. Failed checkpoint/retry fixtures50/100/150/160 retain failure data without rewards, unlocks or story/finale activation.
-161 and Chapter2 remain unavailable; Chapter1 finale remains planned500 and is not falsely completed.

[Story audit](QA/ContentAudit.json):24 referenced scenes,23 milestones,320 outcome reactions and358 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes. After160 orchard_entrance continues the150 transition toward paths between trees and fields. No harvest/helper/production mechanic is implied.

## Verification limits and scope

Tests ran in isolated save stores using the previously approved process-only Vulkan/descriptor4096 workaround. Project graphics settings and live player saves were untouched. Android build/device, full-project suite and new visual/pointer checks were not run. The default manual folder still contains5 valid recordings, all11–30; human31–160 difficulty is pending, and external folders were not searched. [Manual evidence](QA/ManualEvidence.json).

[Scope](QA/Scope.json):8 existing project files modified (four active JSONs/four tests),9 candidate/meta files added,0 deleted,7605 existing files unchanged. Task-start snapshots, reproducible scripts and test patch retained; generated test scene/linker cleanup complete. Scoped Python syntax/test-whitespace and exact draft parity checks pass. No commit.

Next: author bounded161–170 orchard-row paths with recovery161/166 and distinct route-choice layouts before installation; preserve1–160. No bulk fill or solver-only budget tightening. Human/device evidence, optional returning-player recap and older generic reactions remain open. Animation/effect work remains last.
