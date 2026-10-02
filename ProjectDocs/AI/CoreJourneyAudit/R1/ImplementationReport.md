# Core Journey Audit R1

Completed 2026-09-26 in Unity 6000.5.9f1. The existing core navigation passed both tested journeys; no production change was required.

[Portrait review](CoreJourney_Audit_R1_Review.html) · [Scope and results](QA/ScopeAndVerification.json) · [Unity results](QA/Verify01/PlayMode/results.xml) · [Browser checks](QA/BrowserChecks.json)

## Change

Extended `Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs` with `NativeJourneyWinsAndReturnsHome` and `NativeJourneyFailsRetriesAndExitsHome`, plus isolated setup, move planning, native button raycasts and capture helpers. All other 7,195 Assets files remained unchanged relative to the task-start snapshot; no Assets files were added or deleted. Existing unrelated working-tree edits were preserved. No production prefabs, builders, revisions, art, fonts or metadata were changed.

## Verified behavior

Both journeys start at Home, tap World Map → Chapter 1 → Round 1, advance the intro, enter Pre-Level and start the actual authored round. Pause → Settings → Back → Resume preserves the same board instance, board hash and move count. Visible UI buttons are checked through EventSystem raycasts and pointer-click handlers at their centers within a simulated phone safe area.

| Journey | Actual outcome | Legal moves | Energy | Gold |
| --- | --- | ---: | --- | --- |
| Russian | Victory, post-victory story, maps, Home | 15 | 5 → 4 | 12,540 → 12,660 |
| English | Fail, Retry, Pause exit, maps, Home | 28 | 2 → 1 | 12,540 → 12,540 |

Victory records one first-clear completion and awards the authored 120 gold. Fail grants no gold. Retry creates a fresh board with 28 moves. The existing rule makes Retry free; entering from Pre-Level spends one energy. The English journey also tests zero-energy Play opening OutOfEnergy and closing it while preserving Pre-Level, then adds two energy in isolated test setup. Both journeys end at Home without an active modal or further energy deduction.

Move candidates are evaluated on planner copies, with a hash assertion that planning leaves the runtime board unchanged. The selected legal move is executed by the actual board through `SwipeCell`, including resolution and real outcome callbacks. Starting goals, moves and seed are not replaced, and no forced result or test snapshot is used.

## Evidence

`Sources/Verify.py Verify01` completed six PlayMode tests: six passed, zero failed or skipped. This includes the two new journeys and four existing navigation smoke tests. No separate EditMode suite or prefab rebuild was needed for this test-only change.

The run produced 36 actual Unity captures at 576×1280 with safe rectangle (0,48,576,1172). Captures show the state before each native button tap. The standalone HTML is a capture viewer, not a playable implementation. Browser verification decoded all 36 views, exercised the native-pixel toggle and found no horizontal overflow or JavaScript errors. Victory RU, Fail EN and Gameplay EN were visually inspected. Scoped C# whitespace checks passed.

## Limits and next step

These are two deterministic first-round journeys, not all chapters, seeds, languages or devices. Tutorial instruction overlays are marked complete in isolated setup; these journeys do not test first-time teaching interruptions. Navigation uses native UI raycasts, while board moves call the runtime swipe command rather than simulating physical finger gestures. User save storage is not used. The runtime save and language are restored after each journey.

No Android build/device, physical touch, hardware Back, lifecycle, process relaunch, backend, live ads/purchases, performance or full-project regression was run. No new assets or Scenario generation were necessary. This task establishes routing behavior and does not claim new pixel-for-pixel reference fidelity.

Next recommendation: audit save/relaunch persistence and app pause/resume, followed by physical Android validation.
