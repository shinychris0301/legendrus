# First-clear reward recovery R1 — 2026-10-03

First-clear rewards and level progress now commit together through the existing save journal. Arithmetic rejection leaves the full save, reward eligibility, prior outcome/write result and journal unchanged. A blocked write publishes no completion or reward and preserves the existing persistence lock. There is no cap, partial payout, reward-value change or schema migration.

`TryCompleteLevel` returns false with an empty outcome on rejection; the existing `CompleteLevel` compatibility API rejects an uncommitted completion with `InvalidOperationException`. Argument, locked-level and continuation guards remain intact. A durable primary commit with incomplete cleanup follows the existing retained/reset convention: adopt the committed candidate, keep persistence locked and avoid granting it twice.

Gameplay now uses the boolean transaction. A rejected completed board stays in memory under the existing native error modal. Overflow permits retry after capacity is freed; a storage lock disables Retry. Visible Close and Android Back leave for Chapter Map without inventing a win or reward. Retry runs after modal dismissal and checks the exact board, its Completed phase and current Gameplay route. A premature or stale callback cannot commit. Ordinary Gameplay entry to the generic meta-page error dialog remains guarded. Win analytics/telemetry and Victory routing follow a committed completion.

The current APK remains **766,014,334 bytes**, SHA256 `9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654`, with its unchanged sidecar. It includes the prior daily-reward fix and **excludes both the doubled-gold and first-clear source changes**. [Existing packaging record](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md).

## Verification

The following evidence is reconciled by [final validation](QA/FinalValidation.json); only successful final runs count toward the final total.

| Run | Result | Evidence |
| --- | --- | --- |
| Before the production fix | Eight cases reproduced partial in-memory mutation: six reward resources, completion counter and blocked saving | [Reproduction](QA/Reproduce/summary.json) |
| Initial focused post-fix run | 20 service checks passed; three new asynchronous fixtures differed only in the full-energy clock timestamp | [Retained focus run](QA/Focus/summary.json) |
| Initial combined regression | 80 passed / 42 failed / one skipped; incorrect fixture grouping plus two fixture preconditions | [Retained combined run](QA/FinalPlay/summary.json) |
| Final core Play Mode | 56 distinct passed; zero failed/skipped | [Core](QA/Core/summary.json), [XML](QA/Core/results.xml) |
| Isolated retained-round and continuation Play Mode | 51 distinct passed; zero failed/skipped | [Retained](QA/Retained/summary.json) |
| Isolated native error-dialog Play Mode | Eight distinct passed; zero failed/skipped | [Error](QA/Error/summary.json) |
| Isolated native legal win/fail journeys | Two distinct passed; zero failed/skipped | [Journeys](QA/Journeys/summary.json) |
| Normal story recap Play Mode | Five distinct passed; zero failed/skipped | [Recap](QA/Recap/summary.json) |
| Explicit disposable-QA recap Play Mode | One distinct passed; zero failed/skipped | [Recap QA](QA/RecapQA/summary.json) |
| Save-journal write and Victory/DailyReward/ErrorDialog Edit contracts | 47 distinct passed; zero failed/skipped | [Edit](QA/Edit/summary.json) |

Final result: **123 distinct Play Mode + 47 Edit Mode = 170 distinct passes**, zero failures/skips across seven successful final launchers. These targeted checks are not a full-project sweep, Android execution, human difficulty acceptance or provider settlement test. Installed Unity: 6000.5.9f1; runtime Vulkan was selected for the process only. Every final launcher exited 0.

Boundary fixtures verify all six credits, an overflowing replay completion counter, unchanged complete save/journal key presence and values, no change event and no partial publication. The exact-limit fixture grants all six resources to `int.MaxValue`, reloads the journal and replays without another credit. Nonpositive rewards still normalize to zero; invalid arguments and locked levels retain their original behavior. The blocked-save fixture retains its future-version sentinel, empty outcome and write lock, then restores its synthetic journal for teardown.

Actual native gameplay fixtures execute legal swaps on round 1. In Russian and English, the win is rejected for one coin of insufficient capacity, repeated Retry preserves the same terminal board/hash/score/moves, existing `SpendGold(1)` frees that capacity, and modal Retry commits the full reward/progress once. Double taps publish one event; attempts stay at one, level 2 unlocks, and the journal matches the committed save. Separate tests cancel a pending retry by changing route, invoke the stale callback without reward, close an overflowing win without progress and use Android Back to leave a blocked-save win with the future journal untouched.

The initial asynchronous fixtures used a full energy bar; its existing clock updates the in-memory timestamp while playing. They now spend one energy through the ordinary service before the direct Gameplay fixture entry, matching an active round and stabilizing full-save comparisons. The exact-limit journal fixture uses the same precondition so load-time clock normalization cannot obscure its transaction assertions. The existing direct callback fixture now checks that an early callback is ignored, sets the board's existing Completed phase, then invokes its real router callback; it remains explicitly a synthetic callback exercise rather than a legal-swap witness.

The combined regression was incorrectly launched with continuation fixtures outside their required `/tmp/lor-continuation-r1/` namespace. Their failed setup/teardown restored a null fixture save, causing subsequent error-dialog and navigation cleanup failures. Those groups now run in separate fresh stores, with the required continuation namespace. The one explicit-QA recap check also runs separately with `LOR_GAMEPLAY_QA=fresh`. The unsuccessful runs and their diagnostics are retained rather than treated as product passes.

## Native evidence and preserved scope

[Russian recovered Victory portrait](QA/Core/Captures/LOR_VICTORY_CAPTURE_DIR/Victory_FirstClearOverflowRecovered_ru.png) · [English portrait](QA/Core/Captures/LOR_VICTORY_CAPTURE_DIR/Victory_FirstClearOverflowRecovered_en.png). The Russian portrait was inspected. These are actual Unity native-page captures after automated legal swaps and a synthetic balance boundary, not generated previews or human/device acceptance. No approved page was redesigned. The wallet header remains intentionally hidden on Victory; balance limits are verified by service assertions.

Only four task-start inputs changed: [GameState](../../../../Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs), [ScreenRouter](../../../../Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs), [level-system tests](../../../../Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs) and [Victory tests](../../../../Assets/LegendsOfRus/Tests/PlayMode/VictoryAlignmentPlayModeTests.cs). All 7,835 other inputs remain identical; none were added/deleted. [Scope](QA/Scope.json), [task-only patch](Sources/TaskChanges.patch). Earlier daily/double recovery edits remain intact. Four continuity documents plus this task's evidence/tools are updated. Temporary test scenes are removed; generated caches stay outside the patch.

All existing GUIDs/metas, actual prefabs and owning builders/revisions, art/fonts, approved Splash/Title, packages/settings/backend, save schema, player reward values and Android/provider guards are retained. Native error copy/sprites and controls are reused. No normal personal journal, credentials, phone enumeration/install/launch, hosted services, production build, store submission or Git commit/push was involved.

## Remaining work

The rejected completed board is retained only while its current Gameplay route is open. Explicit exit or process loss does not create a durable pending-win checkpoint; first-clear eligibility stays unclaimed for a later attempt. This task does not certify arbitrary disk/device fault recovery, cold-process Android behavior or real paid/video reward delivery. The durable-primary branch uses the existing journal convention; repository fault tests are not a new injected GameState filesystem-fault test.

`AddGold`, `AddGems` and `AddBooster` still need a separate overflow/transaction review. `stock-max` remains display/consumption only. Finish that independent credit work before a later guarded APK refresh carrying the source fixes. Genuine rounds 201–210 recordings, ratings and block observations remain pending; [recording intake](../../ChapterOneHumanAcceptance/R2RecordingReview/README.md) is ready. Review human results before tuning; phone/hosting remain deferred, all 18 physical rows pending, effects last.
