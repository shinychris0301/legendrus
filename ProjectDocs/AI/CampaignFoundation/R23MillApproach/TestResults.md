# Campaign R23 — mill approach test results

**186 distinct Unity tests passed; 0 failed, 0 skipped.** Rounds101–110 are installed in normal gameplay. Unity6000.5.9f1, Verify01, isolated save stores; previously authorized process-only Vulkan/descriptor4096. Project graphics settings unchanged.

|Suite|Passed|Failed|Skipped|
|---|---:|---:|---:|
|Active campaign|120|0|0|
|Campaign board/UI|10|0|0|
|Level/story/finale compatibility|47|0|0|
|Checkpoint lifecycle|9|0|0|
|Total|186|0|0|

- **330/330** deterministic legal starts, without opening matches, and **330/330** replayed completion solutions without inventory boosters. All300 earlier witnesses exactly match R21.
- Eight completed-save boundaries10/40/50/60/70/80/90/100 pass migration;100 unlocks101 while preserving balances, energy and earlier progress. Completion/reload/replay through110 does not duplicate currency or any of the four booster rewards. Earlier completed levels are explicit fixtures, not human-play evidence.
- Actual Unity board-input journeys100/101/103/105/110 pass map node labels/buttons → PreLevel → Gameplay → Victory → appropriate dialogue → ChapterMap. Energy is consumed once and first-clear rewards apply once.
- Checkpoints50–110 survive runtime reset/save reload, RU/EN lookup and single acknowledgement without economic mutations. All18 pending milestones drain in order with reload after each. Completed110 cannot select111/Chapter2 or claim a false chapter finale.
- First100 levels and prior story/localization entries unchanged. Exact active/candidate equality, goal counts, obstacle bounds/layers, free delivery starts/exits, bilingual keys and reward budget pass.
- Story audit:19 referenced scenes,18 ordered milestone links,220 outcome reactions,253 required bilingual keys; no missing keys or orphan scenes. No installed Chapter1 finale; planned boundary remains500.
- First-clear101–110 totals1840gold,6gems,1Living Light. No scene/sector/replay bonus.

|Round|Move limit|Completion moves, attempts1 /2 /3|
|---|---:|---|
|101|34|4 / 3 / 3|
|102|29|4 / 4 / 4|
|103|29|5 / 4 / 3|
|104|29|5 / 3 / 4|
|105|28|6 / 8 / 9|
|106|34|3 / 4 / 3|
|107|29|7 / 6 / 5|
|108|29|4 / 4 / 7|
|109|29|5 / 6 / 8|
|110|30|6 / 7 / 6|

These are search-assisted legal solutions, not measured human win rates or difficulty acceptance. The R22 seeds, layouts, rewards and move limits were preserved; no budget/rule adjustment was needed to obtain passing results.

Source checks and task-start scope pass:8 existing files changed (four active JSONs/four test files),9 candidate files/metas added,0deleted,7560 pre-existing project files unchanged. Existing resource GUIDs, artwork, prefabs, gameplay rules and settings preserved. Test-generated scene/linker artifacts cleaned.

Not run: Android build/device, full-project suite, new visual/pointer audit or human31–110 difficulty acceptance. Default manual export folder still contains five valid recordings covering11–30 only. This does not rule out recordings elsewhere. Existing returning-player backlog has no optional recap; tests verify ordering, not UX acceptance. Animation/effect work remains deferred.

[Verification](QA/Verification.json) · [Replay evidence](QA/DifficultyScreening.json) · [Content checks](QA/ContentChecks.json) · [Story audit](QA/ContentAudit.json) · [Scope](QA/Scope.json) · [Implementation](ImplementationNotes.md)
