# Persistence and Lifecycle Audit R1

Completed 2026-09-26 using Unity 6000.5.9f1. Twenty-one PlayMode checks passed across three Editor processes. No production changes were required for the behavior tested.

[Portrait capture review](Persistence_Lifecycle_R1_Review.html) · [Verification and scope](QA/ScopeAndVerification.json) · [Separate-process result](QA/ColdProcessResult.json) · [Browser checks](QA/BrowserChecks.json)

## Separate-process persistence

The writer runs the real level-one victory journey, returns Home, changes all four settings through native UI clicks, persists English, consumes one mace and spends 17 gems through the existing GameState services, selects level two, and writes a test checkpoint. The reader starts in a different Unity process, sharing only the isolated PlayerPrefs directory and audit checkpoint. Process IDs are asserted different. It compares the complete journal payload and freshly initialized GameState to that checkpoint.

Restored values include 12,660 gold, 333 gems, four energy, three earned stars, level two unlocked/selected, the decremented booster stock, completed tutorial/story flags and all four settings disabled. Language restores as English from its separate preference key. The reader then calls the completion service for a replay and confirms the first-clear reward is not granted twice: gold, gems and mace count remain unchanged while completions increase to two. This replay assertion tests the completion service; it is not a second played round.

The original writer Victory is played with legal board commands, as in Core Journey R1. The checkpoint contains only synthetic test progress. No user save directory is read or changed. Reader startup uses the existing RuntimeBootstrap/GameState initialization; review navigation to Home, Settings and ChapterMap is explicitly requested by the test and does not demonstrate the full Splash/Loading startup interaction.

## Lifecycle and clock behavior

The lifecycle test enters the real gameplay page from Pre-Level, holds a cell touch, simulates Unity pause and focus messages, and checks that pointer ownership is released. A stale release causes no swap. The idle board hash, score, moves and energy remain unchanged. An existing Pause modal remains open and blocks board input after simulated resume. Resuming through its native control permits one legal swipe; pause/resume callbacks during its resolution do not duplicate the command or energy charge, and it returns to an interactive board with 27 moves.

Callbacks are explicitly dispatched to runtime MonoBehaviours. This is not OS suspension, rendering freeze or a physical touch test. The Editor continues running between those messages. Audio/haptic hardware output is not measured.

The clock test adjusts the isolated save's energy timestamp: 61 elapsed minutes restores exactly two energy once, repeated refresh grants nothing extra, a long interval caps at energyMax, and a future timestamp adds no energy. It calls the real clock-refresh method rather than actually suspending the app for an hour.

## Validation

| Process | Checks | Result |
| --- | ---: | --- |
| Writer | 1 | Passed |
| Reader | 1 | Passed |
| Regression | 19 | Passed |

All 21 passed with zero failures/skips. Regression includes eight navigation/lifecycle/clock tests and eleven existing save migration/recovery tests. Those cover legacy migration, corrupted-primary backup recovery, invalid/future-version write protection, reset-history isolation and missing-primary behavior. Results and launch records are under `QA/Verify01/{Write,Read,Regression}`.

There are 66 actual Unity captures at simulated 576×1280 phone dimensions. All 66 decoded in the browser viewer; its native-pixel toggle and page-overflow/JavaScript checks passed. Restored English Settings and post-resume Gameplay were visually inspected. This is a screenshot review, not a playable browser build or new reference-fidelity pass.

Only `Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs` changed under Assets; 7,195 other files were unchanged relative to the task-start snapshot, with none added/deleted. Added four tests plus checkpoint/message helpers. Existing GUIDs, art, layouts, prefabs/builders and revisions were preserved. No asset generation or Scenario calls were needed. Scoped whitespace checks and audit-script syntax checks passed.

## Remaining limits and next action

The save schema does not include an unfinished board, remaining moves or an active modal. A cold restart restores durable progress/preferences, not a round in progress. The reader confirms transient LastCompletionResult is absent, but this audit does not force-kill the app mid-round. No new resumable-round feature was added.

No Android build/device, actual process kill, hardware Back, notification interruption, backend, live ads/purchases, performance or whole-project test suite was run. Tutorial teaching overlays are precompleted for the core journey. No separate EditMode run or prefab rebuild was necessary for these test-only changes.

Next recommendation: physical Android lifecycle/navigation verification, covering background/foreground, screen lock, hardware Back and force-close/relaunch. Preserve the existing Android build approval guards and confirm device availability before a device run. Decide separately whether cold-restart recovery of an unfinished round is desired.
