# R20 household routes — test results

**155 distinct Unity tests passed; 0 failures; 0 skips.** Rounds81–90 are integrated into normal gameplay. Unity6000.5.9f1, isolated test storage; all suites passed on Verify01.

|Suite|Passed|
|---|---:|
|Active campaign,90-round replay screening, migration and normal journeys|98|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **270/270 deterministic legal starts** passed:90 rounds ×3 attempt seeds, without automatic opening matches.
- **270/270 inventory-free completion replays** passed within budget, including30/30 new-round solutions. All240 prior-round witness records exactly match R19.
- New solutions use2–7moves. No move-budget adjustment or gameplay-rule workaround was needed. Short solver solutions do not establish human difficulty.
- Normal map → Pre-Level → gameplay → victory → applicable dialogue → map journeys passed for80,81,83,85,90 using legal taps/swaps. Energy, first-clear rewards, courtyard and hearth-route scene acknowledgements passed.
- Completed10/40/50/60/70/80 saves retain balances, energy and progress, unlocking11/41/51/61/71/81. Round90 save/reload and replay protection passed for gold, gems and all four inventory booster types.
- New first-clear budget validated:1840gold,6gems,1Sea Pearl. No story/replay bonus. Chapter1 remains incomplete;91 and Chapter2 remain unavailable.
- First80 levels and prior story/localization remain unchanged. Active/candidate equality, RU/EN validation, goal counts, obstacle coordinates, delivery exits, protected artwork/settings and source whitespace passed.

|Round|Move budget|Three seed solution lengths|
|---|---:|---|
|81|34|4, 3, 3|
|82|29|4, 4, 4|
|83|29|3, 4, 4|
|84|28|6, 6, 7|
|85|28|6, 7, 6|
|86|34|2, 2, 3|
|87|29|4, 5, 4|
|88|29|5, 4, 6|
|89|29|4, 4, 3|
|90|28|7, 5, 6|

**Not verified:** human difficulty acceptance for31–90, Android device/build, full-project test suite, new visual/pointer hit-area audit. Automated solutions demonstrate solvability, not player win rates. Difficulty remains provisional; animations/effects remain deferred to the final phase.

[Verification](QA/Verification.json) · [Replay results](QA/DifficultyScreening.json) · [Preservation](QA/ContentChecks.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
