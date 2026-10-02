# R18 far-street block — test results

**133 distinct Unity tests passed; 0 failures; 0 skips.** Rounds61–70 are integrated into normal gameplay. Unity6000.5.9f1, isolated test storage; all suites passed on Verify01.

|Suite|Passed|
|---|---:|
|Active campaign,70-round replay screening, saves and normal journeys|76|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **210/210 deterministic legal starts** passed:70 rounds ×3 attempt seeds, without automatic opening matches.
- **210/210 inventory-free completion replays** passed within budget, including30/30 new-round solutions. All180 prior-round witness records exactly match R17.
- New solutions use3–19moves. Round65 uses7/7/19 of28moves; round67 requires16 consumed moves in every tested variant. No budget adjustment or rules workaround was needed.
- Normal map → Pre-Level → gameplay → victory → applicable dialogue → map journeys passed for60,61,65,67,70 using legal taps/swaps. Energy and first-clear rewards passed. Both well and far-street scene acknowledgements passed.
- Completed10/40/50/60 save migration preserves balances, energy and progress, unlocking11/41/51/61. Round70 save/reload and replay protection passed for gold, gems and all four inventory booster types.
- New first-clear budget:1840gold,6gems,1Living Light. No story/replay bonus. Chapter1 stays incomplete;71 and Chapter2 stay unavailable.
- First60 levels and prior story/localization remain unchanged. Active/candidate equality, RU/EN validation, goal/obstacle counts, delivery exits, protected artwork/settings and source whitespace passed.

|Round|Move budget|Three seed solution lengths|
|---|---:|---|
|61|34|3, 3, 3|
|62|29|4, 5, 4|
|63|29|4, 3, 4|
|64|28|6, 8, 7|
|65|28|7, 7, 19|
|66|34|3, 3, 4|
|67|27|16, 16, 16|
|68|29|6, 5, 5|
|69|29|4, 6, 5|
|70|28|4, 10, 8|

Solver results establish solvability, not human difficulty or win rates. **Not verified:** human difficulty acceptance for31–70, Android device/build, full-project test suite, new visual/pointer hit-area audit. Difficulty labels and budgets remain provisional. Animations/effects remain deferred to the final phase.

[Verification](QA/Verification.json) · [Replay results](QA/DifficultyScreening.json) · [Preservation](QA/ContentChecks.json) · [Scope](QA/Scope.json) · [Scenario/puzzle review](DesignReview.md) · [Implementation](ImplementationNotes.md)
