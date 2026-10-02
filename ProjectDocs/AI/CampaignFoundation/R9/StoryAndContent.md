# R9 candidate: hearth to well

This is authored, playable development content. It is not activated in production, and its difficulty has not received human acceptance. All animation/effect refinement remains deferred to the final development step.

## Story mapping

| Beat | Candidate behavior | Existing legacy behavior |
|---|---|---|
| Intro / after 1, 3, 4, 6, 8, 9 | Preserve existing dialogue verbatim | Unchanged |
| After 2 | First blossom on the old apple tree; restores a missing scenario beat | No additional scene |
| After 10 | Firebird: one hearth burns again, but the other village fires still need protection | Original ten-round finale remains intact |
| After 20 | Elder: well water is clear; next task is the granary | No round20 in legacy catalog |
| After 500 | Future chapter finale / departure | Not present or executable in this partial candidate |

The candidate has no `chapter.complete` trigger, ChapterComplete return route or published finale. Reaching20 cannot unlock21 or Chapter2. The original finale source, levels1–10 gameplay data and first-clear rewards remain intact. The candidate changes round10's display name, difficulty label and story trigger; it does not change that board's moves, seed, obstacles, goals or rewards. Legacy and expanded scene acknowledgement keys remain separate. There is no production save migration in this task.

## Authored round designs

All boards are8×7. Draft move/goal/reward budgets from R7 are retained as provisional values. Seeds2111–2120 use the existing attempt-seed derivation and current PrototypeR4 rules. No inventory booster is required by the verified completion traces.

| Round | Puzzle | Moves / colors |
|---|---|---|
|11 Warm Ash|Open recovery board: red and blue collection|32 /5|
|12 Sparks over the Orchard|Guarded optional four-match setup; create a line item with (3,2)→(3,3)|30 /5|
|13 Rows of Light|Two separated three-cell ice clusters with match lanes around them|28 /5|
|14 Columns of Warmth|Two chain strips with an open center|29 /5|
|15 The Frozen Well|Ten two-layer ice cells form a central basin, with both sides open|27 /6|
|16 Water for the Hearth|Four single-layer corner ice cells; recovery space|32 /5|
|17 Free the Windlass|Eight chains in separated pockets|29 /5|
|18 Reflection in the Water|Six crates beside an optional T-match; (3,2)→(3,3) creates a Burst|30 /5|
|19 Buckets by the Well|Two crate islands with exposed adjacent matching lanes|29 /5|
|20 The First Clear Stream|Eight two-layer ice cells plus six edge chains; three goal slots|27 /6|

Coordinates are zero-based; y increases downwards. Guards around teaching setups prevent incidental random neighbors changing the intended special family. These are optional opportunities, not forced tutorials or new mechanics.

## Candidate localization and economy

RU/EN scene and reaction keys are complete for the20-round package and validated through the existing campaign catalog validator. New round names also have RU/EN keys; the current gameplay and Pre-Level screens display round numbers, not those name keys. Reactions are authored/localized data, not a newly connected result overlay.

Rounds11–20 award1,820 gold,6 gems and1 Mace in total, on first clear only. No scene/sector bonus or replay grant was added. The package's complete bilingual content is independent of the legacy localization files; production files are untouched.

## Playtest before expanding the batch

Start the project in Play Mode and reach Home. Use **Legends of Rus → Development → Campaign Sector Candidate - Start at 11 (Test Progress)**. This explicitly seeds disposable completion/claimed-reward records for1–10, acknowledges earlier scenes, and opens the new hearth scene before round11. To review the entire flow from the intro, use **Campaign Sector 01 Candidate (Rounds 1-20)** instead. Both use the existing campaign UI and an in-memory store; leaving Play Mode discards this test state.

Play without inventory boosters first. Record each round's attempt number, win/fail, moves remaining, retries and whether the intended line/Burst opportunity was understandable. Check the15→16 difficulty relief and20's mixed goals. Do not turn automated solution length into a player win rate: bounded search has full board and deterministic refill knowledge. The short solutions suggest generous budgets, which need human review before changing difficulty or adding21–30.

Production activation, existing-player recap/migration, rounds21–50, the500-round finale and Android acceptance remain later work. The first action after this delivery is hands-on acceptance of11–20, followed by the next authored batch and granary beat. Effects stay last.
