# Campaign R24 — mill-yard test results

**198 distinct Unity tests passed; 0 failed, 0 skipped.** Rounds111–120 are installed in normal gameplay. Verification used Unity6000.5.9f1, isolated save stores and the previously authorized process-only Vulkan/descriptor4096 configuration. Project graphics settings are unchanged.

|Suite|Passed|Failed|Skipped|
|---|---:|---:|---:|
|Active campaign|131|0|0|
|Campaign board/UI|10|0|0|
|Level/story/finale compatibility|47|0|0|
|Checkpoint lifecycle|10|0|0|
|Total|198|0|0|

- **360/360** deterministic legal starts without opening matches; **360/360** completion replays within move limits, without inventory boosters. All330 replay records for rounds1–110 exactly match R23.
- Nine completed-save boundaries10/40/50/60/70/80/90/100/110 pass migration. Completed110 unlocks111 while preserving balances, energy and earlier progress. Completion/reload/replay through120 does not duplicate currency or any of the four booster rewards. Earlier completion history is an explicit fixture, not human-play evidence.
- Actual Unity board-input journeys110/111/115/118/120 pass map node labels/buttons → PreLevel → Gameplay → Victory → appropriate dialogue → ChapterMap. Energy is consumed once and first-clear rewards apply once. Round118's consumed-move survival completes after18 moves in all three attempts.
- Checkpoints50–120 survive runtime reset/save reload, non-advancing RU/EN lookup and durable single acknowledgement without economic mutation. All19 pending milestones drain in order with reload after each. Completed120 cannot select121/Chapter2 or claim a false chapter finale.
- First110 levels and prior story/localizations unchanged. Exact active/candidate equality, goal quantities, obstacle coordinates, free delivery starts/exits, bilingual keys, authored brief parity and reward budget pass.
- Story audit:20 referenced scenes,19 ordered milestone links,240 outcome reactions,274 required bilingual keys; no missing keys or orphan scenes. No installed Chapter1 finale; planned boundary remains500.
- First-clear111–120 totals1840gold,6gems,1SeaPearl. No checkpoint/dialogue/replay bonus.

|Round|Move limit|Completion moves, attempts1 /2 /3|
|---|---:|---|
|111|34|3 / 5 / 3|
|112|29|6 / 4 / 3|
|113|29|5 / 5 / 5|
|114|29|3 / 4 / 5|
|115|28|9 / 7 / 7|
|116|34|3 / 3 / 4|
|117|29|8 / 7 / 7|
|118|28|18 / 18 / 18|
|119|29|5 / 6 / 5|
|120|30|4 / 10 / 10|

These are search-assisted legal solutions, not measured human win rates or difficulty acceptance. New solutions use3–18moves. Authored seeds/layouts/budgets remained unchanged during verification; no rule or move-limit adjustment was needed for passing results.

Source checks and task-start scope pass:8 existing files changed (four active JSONs/four test files),9 candidate files/metas added,0deleted,7569 pre-existing project files unchanged. Resource GUIDs, runtime rules, artwork, prefabs and settings preserved. Test-owned scene/linker artifacts cleaned. No Git commit.

Not run: Android build/device, full-project suite, new visual/pointer audit or human31–120 difficulty acceptance. Default manual export folder contains five valid recordings covering11–30 only; this does not rule out external recordings. Optional recap for long returning-player dialogue backlog and older repetitive reactions remain unresolved UX/editorial work. Animations/effects remain deferred.

[Test verification](QA/Verification.json) · [Replay evidence](QA/DifficultyScreening.json) · [Content checks](QA/ContentChecks.json) · [Story audit](QA/ContentAudit.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
