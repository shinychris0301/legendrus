# Campaign R27 — mill checkpoint test results

**234 distinct Unity tests passed; 0 failed, 0 skipped.** Rounds 141–150 are installed in normal gameplay. Unity 6000.5.9f1, Verify01, isolated save stores; previously authorized process-only Vulkan/descriptor4096. Project graphics settings unchanged.

|Suite|Passed|Failed|Skipped|
|---|---:|---:|---:|
|Active campaign|164|0|0|
|Campaign board/UI|10|0|0|
|Level/story/finale compatibility|47|0|0|
|Checkpoint lifecycle|13|0|0|
|Total|234|0|0|

- **450/450** deterministic legal starts without opening matches; **450/450** completion replays within move limits without inventory boosters. All 420 earlier replay records exactly match R26.
- Twelve completed-save boundaries (10,40,50,60,70,80,90,100,110,120,130,140) pass migration. Completed 140 unlocks 141 while preserving balances, energy and prior progress. Completion/reload/replay through 150 does not duplicate currency or any of the four booster rewards. Earlier completion history is an explicit fixture, not human-play evidence.
- Actual Unity board-input journeys 140/141/145/148/150 pass map node labels/buttons → PreLevel → Gameplay → Victory → appropriate dialogue → ChapterMap. Energy is consumed once and first-clear rewards apply once. Round 148 requires its intended 18 consumed moves in each attempt.
- Checkpoints 50–150 survive runtime reset/save reload, non-advancing RU/EN lookup and durable single acknowledgement without economic mutation. All 22 pending milestones drain in order with reload after each. Completed 150 cannot select 151/Chapter2 or claim a false chapter finale.
- First 140 levels and prior story/localizations unchanged. Active/candidate equality, goal quantities, obstacle coordinates, delivery starts/exits, bilingual keys, authored brief parity and reward budget pass.
- Story audit: 23 referenced scenes, 22 ordered milestone links, 300 outcome reactions, 337 required bilingual keys; no missing keys or orphan scenes. No installed Chapter1 finale; planned boundary remains 500.
- First-clear 141–150 totals 1840 gold, 6 gems and 1 Living Light. Full 101–150 sector totals 9200 gold, 30 gems, 1 Mace, 1 Firebird, 2 Living Light and 1 Sea Pearl. No dialogue/checkpoint/replay bonus.

|Round|Move limit|Completion moves, attempts 1 / 2 / 3|
|---|---:|---|
|141|34|4 / 4 / 3|
|142|29|5 / 4 / 3|
|143|29|4 / 7 / 5|
|144|29|8 / 6 / 5|
|145|28|10 / 6 / 9|
|146|34|3 / 3 / 2|
|147|29|5 / 3 / 4|
|148|28|18 / 18 / 18|
|149|29|8 / 6 / 5|
|150|30|11 / 9 / 9|

These are search-assisted legal solutions, not measured human win rates or difficulty acceptance. New solutions use 2–18 moves. Authored seeds, layouts and budgets remained unchanged during verification; no rule or move-limit adjustment was needed for passing results.

Source checks and task-start scope pass: 8 existing files changed (four active JSONs/four test files), 9 candidate files/metas added, 0 deleted, 7596 pre-existing project files unchanged. Resource GUIDs, runtime rules, artwork, prefabs and settings preserved. Test-owned scene/linker artifacts cleaned. No Git commit.

Not run: Android build/device, full-project suite, new visual/pointer audit or human 31–150 difficulty acceptance. Default manual export folder contains five valid recordings covering 11–30 only; this does not rule out external recordings. Optional recap for returning-player dialogue backlog and older repetitive reactions remain unresolved UX/editorial work. Animations/effects remain deferred.

[Verification](QA/Verification.json) · [Replay evidence](QA/DifficultyScreening.json) · [Content checks](QA/ContentChecks.json) · [Story audit](QA/ContentAudit.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
