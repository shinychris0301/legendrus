# R19 courtyard block — test results

**144 distinct Unity tests passed; 0 failures; 0 skips.** Rounds71–80 are integrated into normal gameplay. Unity6000.5.9f1, isolated test storage; all suites passed on Verify01.

|Suite|Passed|
|---|---:|
|Active campaign,80-round replay screening, migration and normal journeys|87|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **240/240 deterministic legal starts** passed:80 rounds ×3 attempt seeds, without automatic opening matches.
- **240/240 inventory-free completion replays** passed within budget, including30/30 new-round solutions. All210 previous-round witness records exactly match R18.
- New solutions use2–13moves. Round75 uses7/7/13 of28moves;80 uses7/8/11 of28. No move-budget adjustment or rule workaround was needed.
- Normal map → Pre-Level → gameplay → victory → applicable dialogue → map journeys passed for70,71,73,75,78,80 using legal taps/swaps. Energy, first-clear rewards, far-street and courtyard scene acknowledgements passed.
- Completed10/40/50/60/70 saves retain balances, energy and progress, unlocking11/41/51/61/71. Round80 save/reload and replay protection passed for gold, gems and all four inventory booster types.
- New first-clear budget validated:1840gold,6gems,1Mace. No story/replay bonus. Chapter1 remains incomplete;81 and Chapter2 remain unavailable.
- First70 levels and all prior story/localization are unchanged. Active/candidate equality, RU/EN validation, goal counts, obstacle coordinates, delivery exits, protected artwork/settings and source whitespace passed.

|Round|Move budget|Three seed solution lengths|
|---|---:|---|
|71|34|3, 2, 4|
|72|29|4, 4, 4|
|73|29|3, 5, 5|
|74|28|6, 6, 6|
|75|28|7, 7, 13|
|76|34|3, 4, 4|
|77|29|4, 4, 4|
|78|29|5, 5, 3|
|79|29|3, 5, 7|
|80|28|7, 8, 11|

Automated solutions establish solvability, not human difficulty or win rates. **Not verified:** human difficulty acceptance for31–80, Android device/build, full-project test suite, new visual/pointer hit-area audit. Difficulty remains provisional. Animation/effect refinement stays deferred to the final development phase.

[Verification](QA/Verification.json) · [Replay results](QA/DifficultyScreening.json) · [Preservation](QA/ContentChecks.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
