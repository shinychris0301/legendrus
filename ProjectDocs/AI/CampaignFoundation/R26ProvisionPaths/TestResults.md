# Campaign R26 — provision-path test results

**222 distinct Unity tests passed; 0 failed, 0 skipped.** Rounds131–140 are installed in normal gameplay. Unity6000.5.9f1, Verify01, isolated save stores; previously authorized process-only Vulkan/descriptor4096. Project graphics settings unchanged.

|Suite|Passed|Failed|Skipped|
|---|---:|---:|---:|
|Active campaign|153|0|0|
|Campaign board/UI|10|0|0|
|Level/story/finale compatibility|47|0|0|
|Checkpoint lifecycle|12|0|0|
|Total|222|0|0|

- **420/420** deterministic legal starts without opening matches; **420/420** completion replays within move limits without inventory boosters. All390 earlier replay records exactly match R25.
- Eleven completed-save boundaries10/40/50/60/70/80/90/100/110/120/130 pass migration. Completed130 unlocks131 while preserving balances, energy and prior progress. Completion/reload/replay through140 does not duplicate currency or any of the four booster rewards. Earlier completion history is an explicit fixture, not human-play evidence.
- Actual Unity board-input journeys130/131/135/139/140 pass map node labels/buttons → PreLevel → Gameplay → Victory → appropriate dialogue → ChapterMap. Energy is consumed once and first-clear rewards apply once. Round139's survival target requires18 consumed moves in each of the three attempts.
- Checkpoints50–140 survive runtime reset/save reload, non-advancing RU/EN lookup and durable single acknowledgement without economic mutation. All21 pending milestones drain in order with reload after each. Completed140 cannot select141/Chapter2 or claim a false chapter finale.
- First130 levels and prior story/localizations unchanged. Active/candidate equality, goal quantities, obstacle coordinates, delivery starts/exits, bilingual keys, authored brief parity and reward budget pass.
- Story audit:22 referenced scenes,21 ordered milestone links,280 outcome reactions,316 required bilingual keys; no missing keys or orphan scenes. No installed Chapter1 finale; planned boundary remains500.
- First-clear131–140 totals1840gold,6gems,1Firebird booster. No dialogue/checkpoint/replay bonus.

|Round|Move limit|Completion moves, attempts1 /2 /3|
|---|---:|---|
|131|34|4 / 3 / 3|
|132|29|3 / 3 / 3|
|133|29|4 / 5 / 6|
|134|29|5 / 5 / 5|
|135|28|10 / 8 / 13|
|136|34|2 / 3 / 4|
|137|29|9 / 9 / 6|
|138|29|5 / 7 / 5|
|139|28|18 / 18 / 18|
|140|30|7 / 10 / 13|

These are search-assisted legal solutions, not measured human win rates or difficulty acceptance. New solutions use2–18moves. Authored seeds, layouts and budgets remained unchanged during verification; no rule or move-limit adjustment was needed for passing results.

Source checks and task-start scope pass:8 existing files changed (four active JSONs/four test files),9 candidate files/metas added,0deleted,7587 pre-existing project files unchanged. Resource GUIDs, runtime rules, artwork, prefabs and settings preserved. Test-owned scene/linker artifacts cleaned. No Git commit.

Not run: Android build/device, full-project suite, new visual/pointer audit or human31–140 difficulty acceptance. Default manual export folder contains five valid recordings covering11–30 only; this does not rule out external recordings. Optional recap for returning-player dialogue backlog and older repetitive reactions remain unresolved UX/editorial work. Animations/effects remain deferred.

[Verification](QA/Verification.json) · [Replay evidence](QA/DifficultyScreening.json) · [Content checks](QA/ContentChecks.json) · [Story audit](QA/ContentAudit.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
