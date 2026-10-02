# R17 well routes — test results

**122 distinct Unity tests passed; 0 failures; 0 skips.** Rounds51–60 are installed in normal gameplay. Unity6000.5.9f1; isolated test storage; all three suites passed on Verify01.

|Suite|Passed|
|---|---:|
|Active campaign,60-round replay screening, migration and normal routes|65|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **180/180 deterministic legal starts** passed:60 rounds ×3 attempt seeds, with no automatic opening matches.
- **180/180 inventory-free completion replays** passed within budget, including30/30 new-round solutions. New solutions use2–16moves. Round57 correctly requires16 consumed moves; it does not use a timer.
- Normal gameplay routes passed for50,51,55,57,60: chapter map → Pre-Level → legal taps/swaps → victory → applicable dialogue → map. Energy and first-clear rewards passed. Round50's beacon still works;60 acknowledges the new well scene.
- Completed10/40/50 save migration preserves balances, energy and progress, and unlocks11/41/51 respectively. Round60 save/reload and replay protection passed. No duplicate gold, gems, Mace, Sea Pearl or Firebird grants.
- New first-clear budget validated:1840gold,6gems,1Firebird. Story grants no bonus. Chapter1 is not marked complete;61 and Chapter2 stay unavailable.
- First50 rounds and all prior story/localization entries are preserved. Active/candidate equality, bilingual validation, goal counts, obstacle coordinates and delivery exits passed. Approved UI/art and project settings remain unchanged. Source whitespace checks passed.

|Round|Move budget|Three seed solution lengths|
|---|---:|---|
|51|34|4,4,3|
|52|29|3,3,3|
|53|29|3,2,5|
|54|28|5,8,4|
|55|27|9,5,9|
|56|34|3,3,3|
|57|27|16,16,16|
|58|29|6,4,4|
|59|29|3,2,4|
|60|28|6,6,4|

Solver results establish solvability, not human difficulty or win rates. **Not verified:** human difficulty acceptance for31–60, Android device/build, full-project test suite, new visual/pointer hit-area audit. Move budgets and difficulty labels remain provisional. Animation/effect refinement remains deferred to the final development phase.

[Verification](QA/Verification.json) · [Replay screening](QA/DifficultyScreening.json) · [Preservation checks](QA/ContentChecks.json) · [Scope](QA/Scope.json) · [Scenario/puzzle review](SectorPlan.md) · [Implementation](ImplementationNotes.md)
