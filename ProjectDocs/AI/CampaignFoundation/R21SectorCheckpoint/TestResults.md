# R21 sector checkpoint — test results

**166 distinct Unity tests passed; 0 failures; 0 skips.** Rounds91–100 are integrated into normal gameplay. Unity6000.5.9f1, isolated storage; all suites passed on Verify01.

|Suite|Passed|
|---|---:|
|Active campaign,100-round replay screening, migration and normal journeys|109|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **300/300 deterministic legal starts** passed:100 rounds ×3 attempt seeds, without automatic opening matches.
- **300/300 inventory-free completion replays** passed within budget, including30/30 new-round solutions. All270 prior witness records exactly match R20.
- New solutions use3–18moves.95 uses10/7/12 of28;98 requires18 consumed moves in all variants;100 uses12/10/9 of30. No budget adjustment or rule workaround was needed.
- Normal map → Pre-Level → gameplay → victory → applicable dialogue → map journeys passed for90,91,95,98,100 using legal taps/swaps. Energy, rewards, hearth and sector-checkpoint acknowledgements passed.
- **Map node100** was active, displayed “100,” and its bound button opened the correct Pre-Level/gameplay. This checks three-digit pooled-node navigation, not visual typography or physical pointer geometry.
- Completed10/40/50/60/70/80/90 saves retain balances, energy and progress, unlocking11/41/51/61/71/81/91. Round100 save/reload and replay protection passed for currency and all four inventory booster types.
- New first-clear budget:1840gold,6gems,1Firebird. No extra scene/sector/replay grant. The100 checkpoint does **not** complete Chapter1 or unlock101/Chapter2.
- First90 levels and all prior story/localization are unchanged. Active/candidate equality, RU/EN validation, goal counts, obstacle coordinates, delivery exits, protected artwork/settings and source whitespace passed.

|Round|Move budget|Three seed solution lengths|
|---|---:|---|
|91|34|4, 3, 3|
|92|29|3, 3, 3|
|93|29|4, 5, 4|
|94|29|7, 7, 5|
|95|28|10, 7, 12|
|96|34|3, 4, 4|
|97|29|3, 4, 4|
|98|28|18, 18, 18|
|99|29|4, 5, 5|
|100|30|12, 10, 9|

**Not verified:** human difficulty acceptance for31–100, Android device/build, full-project test suite, new visual/pointer hit-area audit. Automated solutions demonstrate solvability, not player win rates. Difficulty stays provisional; animation/effect refinement remains deferred.

[Verification](QA/Verification.json) · [Replay results](QA/DifficultyScreening.json) · [Preservation](QA/ContentChecks.json) · [Scope](QA/Scope.json) · [Design review](DesignReview.md) · [Implementation](ImplementationNotes.md)
