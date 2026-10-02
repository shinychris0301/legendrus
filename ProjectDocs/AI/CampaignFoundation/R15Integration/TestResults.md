# Campaign gameplay integration — test results

**211 distinct Unity tests passed; 0 failures; 0 skips.** Unity 6000.5.9f1, isolated test storage, process-only Vulkan with descriptor limit 4096.

| Final run | Passed |
|---|---:|
| Active runtime content, all-round solvability, save migration, normal gameplay | 43 |
| Campaign board/UI regressions | 10 |
| Native pointer win/fail navigation | 2 |
| Campaign Edit Mode contracts, transactions and persistence | 109 |
| Active level system plus explicitly isolated legacy story/finale fixtures | 47 |

- **40 active rounds:** runtime resources match R14 exactly; RU/EN story and content contracts validate.
- **120/120 deterministic starts:** legal opening actions and no automatic starting matches.
- **120/120 inventory-free completion replays:** all 40 rounds × three attempt seeds clear within their move budgets. Solutions used 2–16 moves. This is bounded-search solvability, not player win-rate evidence.
- **Normal game integration:** Chapter Map → Pre-Level → Gameplay → Victory → story/map passed for rounds 37 and 40 using legal taps/swipes. Energy charged once; first-clear rewards applied once. Native first-round win and loss/retry/exit routes also passed.
- **Persistence:** existing ten-round saves retain balances, energy and progress, unlock round 11, and retain round-40 progress after save/reload. Replay grants no duplicate first-clear rewards.
- **Boundaries:** round 10 leads to the hearth scene and round 11; round 40's riverbank story does not complete Chapter 1 or unlock round 41/Chapter 2.
- **Content preservation:** older resources, candidate data, project graphics settings and approved art unchanged. Authoring and project validation now address the active campaign package.
- **Source checks:** whitespace checks passed for all 14 new/modified C# files.

Earlier test-only failures were corrected: missing shared state, duplicate test router, historical finale expectation, fixture ordering and capture configuration. Final results above use the successful reruns without double-counting repeated tests. Historical ten-level finale tests now explicitly use preserved legacy fixtures; they do not imply the active campaign ends at ten.

**Not verified:** human difficulty acceptance for rounds 31–40 (no recording found), Android device performance/build, full project suite. Prior human rounds 11–30 evidence supports retaining the current budgets; no new difficulty tuning or effects were introduced.

[Machine-readable verification](QA/Verification.json) · [120 replay results](QA/DifficultyScreening.json) · [Scope](QA/Scope.json) · [Manual evidence search](QA/ManualEvidence.json)
