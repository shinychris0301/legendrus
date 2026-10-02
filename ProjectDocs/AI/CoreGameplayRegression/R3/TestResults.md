# Core gameplay regression R3 — VFX compatibility and recovery

2026-09-30. User requested the next step. Animation/effect redesign remains deferred; this task reconciles the remaining historical test contracts with the approved current presentation.

**Final combined Unity Play Mode: 105 passed, 0 failed, 0 skipped. All 40 cases from R43's historical failure inventory pass under the corrected contracts.** This is a focused combined run, not a new full-project suite or visual/device acceptance claim.

| Run | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| Existing VFX fixture baseline | 11 | 5 | 0 | [Summary](QA/Baseline/summary.json) |
| Corrected compatibility/current fixture | 17 | 0 | 0 | [Summary](QA/CompatibilityAndCurrent/summary.json) |
| Combined with R2's 88 cases | 105 | 0 | 0 | [Summary](QA/Combined/summary.json), [NUnit XML](QA/Combined/results.xml) |

Runs overlap; counts are not additive. The fixture has one additional test guarding suppression of duplicate legacy effects under current rules. Final test execution took about 203 seconds, 230 seconds including Editor startup/shutdown.

## Cause and correction

The five failures expected generic pooled rings/bursts during normal current-rule gameplay. CoreLoopVfxPresenter deliberately suppresses these cues because dedicated current presenters already handle creation, impact and boosters. Reinstating those effects would undo the approved removal of duplicate rings.

Three retained geometry/pool tests now explicitly use the existing boardless presenter initialization seam, under the `LegacyVfxCompatibility` category. They still check visible mesh geometry, exact bounded-pool saturation/drop counts, release and reuse without allocations. They do not change production rules or re-enable legacy visuals on normal boards. Any optional captures from those tests are explicitly labelled compatibility captures; none were requested in these runs.

Current-board tests now use the retained swap trail for reset/disable/preset cleanup, clipping and pointer-raycast checks. They require an active effect first, preventing an empty layer from passing the raycast/save-isolation checks vacuously. A new current-rule test asserts that non-swap motion events retain diagnostics without spawning duplicate pooled effects, while the swap trail renders and returns to the pool.

All five real inventory booster variants now verify that the dedicated PrototypeBoosterView starts exactly one presentation and the retired generic booster pool remains inactive. Existing command ordering, completion and inventory-consumption assertions remain. Forced Firebird timeout verifies immediate clearing of the dedicated presentation and an active pooled trail, restored board hash, refunded inventory/save, suppressed completion and released pointer ownership. No recovery behavior was weakened.

## Verification scope

The final combined run includes R1/R2 board/input/motion, campaign board/mechanics, retained-round recovery, all 500 board/PreLevel presentations, 6,000 playable starts, all 57 milestones, native navigation/modal blocking, save comparisons and explicit historical finale compatibility. The [historical failure closure](QA/HistoricalFailureClosure.json) maps all 40 R43 failed case names to passing results in this run. Historical source reports/XML remain untouched.

Only `Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs` changed. [Scope](QA/Scope.json): 1 existing file changed, 0 added/deleted, 7,784 unchanged across Assets/Packages/ProjectSettings/Backend. [Task-only patch](Sources/TaskChanges.patch), before/after snapshots and initial working-tree inventory retained. Existing user edits preserved; source whitespace check passed. No production runtime, art, prefab, campaign, package/settings, build guard, backend or APK changes.

Unity 6000.5.9f1 with previously approved process-only Vulkan. Each run used a separate disposable XDG/loading-test configuration; only the Unity licensing directory was linked for legitimate existing entitlement validation. No license contents were read/copied and normal product saves were not shared. [Runner](Sources/Verify.py) and exact run commands are retained.

## Limits and next task

Passing lifecycle/geometry tests does not establish impressive animation, frame-rate/memory performance or phone visual quality. No full-project suite, Android build, install or device run was performed. No new effects were designed.

Next: perform the **offline Android player journey**, using disposable QA state while preserving existing player data: Home → map → PreLevel → gameplay → victory/rewards/story/next round; failure/free retry/quit; Pause/Settings/return; background, interrupted input and process restart. Confirm first-clear versus replay rewards, energy/booster consumption and the actual retained-failed-round contract. Do not imply arbitrary mid-round resume or paid/ad continuation is complete. Earlier Android storage/title evidence is not gameplay acceptance.

Human difficulty/story pacing for unverified rounds 31–500, phone readability/performance, broader runtime coverage and optional returning-player recap remain open. Hosting/live backend/commercial/social activation remain postponed. Actual animation/effect work stays last.
