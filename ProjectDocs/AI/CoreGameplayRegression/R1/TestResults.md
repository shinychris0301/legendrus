# Core gameplay regression R1 — test results

2026-09-30. User approved the next concrete core-development task after postponing hosting and online services.

## Result

**62 Play Mode tests passed, 0 failed, 0 skipped in the final combined run.** The 23 historical board/input/motion failures were reproduced and resolved by correcting two test fixtures. Production gameplay code did not require a change for these failures.

| Unity execution | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| Current source before fixture correction | 12 | 23 | 0 | [BaselineLicensed](QA/BaselineLicensed/summary.json) |
| Corrected board/input/motion subset | 35 | 0 | 0 | [FixtureFix](QA/FixtureFix/summary.json) |
| Final combined subset, campaign integration and retained-round recovery | 62 | 0 | 0 | [Combined](QA/Combined/summary.json), [NUnit results](QA/Combined/results.xml) |

These runs overlap; they are not 109 distinct passing tests. The first fresh-config attempt exited 198 because Unity could not locate its license, before compilation/tests; it produced no test result and is not counted as a gameplay failure. The licensed baseline then reproduced all 23 target failures.

## Cause and correction

Both standalone fixtures initialized their board from the active 8×7 campaign and then loaded 8×9 QA presets/snapshots. The runtime correctly rejected incompatible dimensions. Each fixture now supplies an explicit disposable LevelData using BoardQAPresetFactory dimensions, with the normal production initializer and an assertion that the current gameplay rules are selected. No dimension check or runtime assertion was weakened.

The motion fixture's cascade search also used the planner's legacy default. Its board flags are now configured before generation, its planner uses Match3Board.CurrentGameplayRules, and a new final-state hash assertion verifies agreement between the fixture planner and actual runtime resolution.

Coverage includes directional gestures, pointer ownership/cancellation, invalid swaps, deterministic resolution, creation/cascade motion hooks, booster recovery, retained failed rounds and current campaign board/mechanics integration. Passing semantic motion tests does not establish visual animation quality.

## Scope and execution

Changed files:

- Assets/LegendsOfRus/Tests/PlayMode/Match3RuntimePlayModeTests.cs
- Assets/LegendsOfRus/Tests/PlayMode/CoreLoopMotionFeedbackPlayModeTests.cs

[Task-only patch](Sources/TaskChanges.patch) · [Scope evidence](QA/Scope.json). Across Assets, Packages, ProjectSettings and Backend: 2 existing files changed, 0 added/deleted, 7,783 unchanged. Existing user edits were preserved. No production code, prefab, campaign, art, backend, project settings, package, build guard or APK changed. Source whitespace checks passed.

Unity 6000.5.9f1 ran with previously approved process-only Vulkan. Each run used a separate disposable XDG configuration and loading-test root. Only the existing Unity licensing directory was linked for legitimate Editor validation; license contents were not inspected or copied. Product saves/preferences were isolated from the normal user configuration. [Runner](Sources/Verify.py) and run commands are retained. No Android/device or full-project suite was run for this test-only change.

## Remaining work

The [exact historical failure overlay](QA/RemainingHistoricalFailures.json) closes 23 of R43's 40 cases using the final combined run. **17 historical cases remain unrerun**, not 17 newly confirmed production defects: 5 legacy VFX cases, 11 campaign/UI/content expectations, and 1 Chapter Complete save/energy-clock comparison.

Next: reproduce and reconcile the non-VFX campaign/UI/save cases with current 500-round Chapter 1 and approved designs; retain meaningful state, reward and navigation assertions. Then run the affected groups together. Keep legacy VFX compatibility checks separate and do not restore rejected circles/digits/hotspots to satisfy stale expectations. Actual animation/effect redesign remains last.

Offline Android gameplay/lifecycle/performance, human difficulty and story pacing for unverified rounds 31–500, phone readability and optional returning-player recap remain outstanding. Hosting, accounts, commerce and social activation remain postponed.
