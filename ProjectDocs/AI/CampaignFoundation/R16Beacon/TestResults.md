# R16 Beacon — test results

**111 distinct Unity tests passed; 0 failures; 0 skips.** Unity 6000.5.9f1, isolated test storage.

|Final Verify02 suite|Passed|
|---|---:|
|Active content, 50-round solvability, migration and normal gameplay|54|
|Campaign board/UI regressions|10|
|Active level system and explicit legacy story/finale fixtures|47|

- **150/150 deterministic legal starts** passed: 50 rounds × three attempt seeds, with no automatic opening matches.
- **150/150 inventory-free completion replays** passed within the move budgets, including **30/30 for rounds 41–50**. New-block solutions use 2–18 moves. Survival rounds 44 and 49 correctly require at least 16 and 18 consumed moves, respectively.
- Normal gameplay journeys passed for rounds **37, 40, 44 and 50**: map, Pre-Level, legal taps/swipes, victory, story and return to map.
- Existing completed-ten-round and completed-forty-round saves retained balances/energy/progress and unlocked **11 and 41**, respectively. Round-50 save/reload and replay checks passed.
- First-clear gold/gems and Mace/Sea Pearl grants passed; replay did not grant duplicate rewards. The new block totals **1840 gold, 6 gems, one Mace and one Sea Pearl**.
- Round 50 opens the beacon scene, without marking Chapter 1 complete or unlocking round 51/Chapter 2.
- First 40 levels, earlier story/localization, approved UI/art and project settings remain unchanged. Active Resources match the new R16 package; RU/EN validation passed. Source whitespace checks passed.

Verify01 initially passed 53/54 active checks. The round-44 fixture left a prior story unread, and the router correctly showed that dialogue before Pre-Level. Correcting the fixture produced the passing Verify02 results; no gameplay or difficulty workaround was required.

Automated solutions establish solvability, not human win rates. **Not verified:** human difficulty acceptance for rounds 31–50, Android device/build, full-project test suite. Existing provisional budgets were retained. Animation/effect refinement remains deferred.

[Verification](QA/Verification.json) · [Difficulty/replay results](QA/DifficultyScreening.json) · [Preservation checks](QA/ContentChecks.json) · [Changed files](QA/Scope.json)
