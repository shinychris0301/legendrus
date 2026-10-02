# Gameplay Mechanics R4 — representative round playtest

Five disposable rounds now open in the existing Gameplay Mechanics Sandbox. They reuse the approved Gameplay prefab, gem sprites, obstacle art and R3 mechanics. Each has live objectives, a finite move allowance, zero inventory stock, terminal win/fail states, Retry and Next controls. Fixtures switches to the earlier laboratory; Rounds returns to this set. Rule switching is disabled in round mode because the boards require R3.

The slice is not registered in the campaign catalog. No progress, rewards or inventory are persisted. No production rule migration, new startup path, prefab rebuild or bitmap generation was introduced.

## Round set

| Round | Purpose | Goals | Moves | Verified winning path |
|---|---|---|---:|---:|
| 1 — Ледяная тропа | Create and use a directional special | 2 single-layer ice cells | 4 | 2 |
| 2 — Под ледяной коркой | Repeat bomb hits against layered obstacles | 2 double-layer ice cells, 1 crate | 5 | 2 |
| 3 — Полет искры | Create a square Spark and reach a distant target | 1 crate | 4 | 2 |
| 4 — Рунные врата | Hit a seal twice | 1 double-layer seal | 5 | 2 |
| 5 — Испытание мастера | Mixed goals spread across the board | 2 double-layer ice cells, 2 crates, 2 double-layer seals | 10 | 3 |

These are short teaching/playtest setups, not campaign difficulty ratings. Rounds2,4,5 intentionally provide board specials. Round1 and3 replay witnesses start with the intended four-in-line or square creation, and assertions verify that creation. Other solutions are allowed. The search found one winning route; it does not establish the minimum moves, player win rate or adequate teaching clarity. The mixed round retains a generous allowance pending human attempts.

The first pass had budgets6/7/6/7/10 and two-move witnesses for all five. Based on that evidence, the introductory budgets were reduced and three obstacles were added outside the central combination footprint in Round5. The final mixed witness takes three moves. A separate ten-move losing replay deliberately avoids finishing goals and verifies move exhaustion, blocked input and Retry.

## Implementation

- New `MechanicsPlaytestRounds.cs` creates fresh deterministic board/level instances. Three teaching layouts are authored cell-by-cell; the bomb and mixed layouts use fixed generated seeds42/45 plus authored specials and obstacles. Both stability and a legal first action are checked. Seeds and opening snapshots are pinned in witnesses.
- `MechanicsSandbox.cs` adds round/fixture modes, instructions, terminal status, zero-stock rounds and finite goals. It retains all20 earlier R3 fixtures and prior rule profiles. This developer harness uses the existing menu/safe area, Canvas and router exit callback.
- `Match3Board.LoadSnapshotForTesting` now preserves the level's move limit when `preserveGoals` is true. Default diagnostic snapshots still start at50; existing R3 fixtures themselves specify50. Production startup does not call this loading helper.
- New `MechanicsRoundPlaytestTests.cs` searches bounded deterministic paths in test code only. Each command records its opening snapshot, action, rule version and expected plan hash. The runtime replays winning paths through actual taps/swipes and compares final board hashes after every command. Retry checks exact opening state, goals and moves. A direct empty-inventory attempt is rejected without board or move mutation.

## Evidence and limits

[Review with actual Unity captures](Gameplay_Mechanics_R4_Review.html) · [Test results](QA/Verification.json) · [Scope](QA/Scope.json) · [Witnesses](QA/Witnesses/Round1_Witness.json).

**9/9 selected PlayMode tests passed**, zero failed/skipped: final round suite2, seal2, earlier mechanics2, Spark1 and campaign board2. Browser review passed22 capture/viewport checks at430/1440, including selectors, solution steps and local links, with no JavaScript errors. Unity compilation succeeded in these runs.

Scope hashes: two existing source files modified, two new C# files plus metadata, zero deletions;7,295 existing Assets/Packages/ProjectSettings files unchanged. Exact pre-run Addressables link.xml/meta restored after the final Unity run. Approved art, prefabs, settings, packages and earlier local changes preserved.

Eleven576×1280 Unity captures cover five starts, five wins and mixed-round failure. The HTML displays one portrait and optional solution steps. It is a capture viewer, not a browser implementation of the game. No new animation timings or effect intensity were tuned in this pass; effects can now be assessed in objective-driven rounds.

Earlier test failures are retained. Verify01 assumed empty stock would prevent arming; the existing command flow checks stock on target submission, so the test was corrected to check that rejection and no mutation. The initial losing-path heuristic ran out of non-seal-hit choices; Verify02 confirmed all five winning paths but not exhaustion. A bounded losing-path search replaced it; Verify03 passed the initial slice. Verify04 passes the revised final slice.

No physical Android, IL2CPP build, complete project test sweep or human difficulty acceptance is claimed. Prior R3 core results are historical evidence and are not counted as a new R4 run. Campaign rules and the held first50-round authoring plan remain unchanged.

## Next step

Human playtest these five rounds without viewing solutions, recording attempts, remaining moves, unclear targets and effects that obscure the board. The next bounded implementation should add a small attempt/feedback record to this sandbox, then tune difficulty and effect readability from those observations before expanding to10 representative rounds or campaign authoring.
