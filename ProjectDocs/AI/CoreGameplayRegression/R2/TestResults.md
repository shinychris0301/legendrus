# Core gameplay regression R2 — campaign, UI and save results

2026-09-30. User approved the next core-development task. Hosting and online activation remain postponed; animation/effect redesign remains last.

**Final combined Unity Play Mode run: 88 passed, 0 failed, 0 skipped.** All 12 targeted historical non-VFX cases pass. Ten failures reproduced in the baseline were caused by obsolete fixture/content expectations. Home story gating and Chapter Complete already passed in that baseline; the latter's clock-sensitive comparison was made robust while retaining every other saved-field assertion.

| Run | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| Current-source baseline, 12 historical cases | 2 | 10 | 0 | [Summary](QA/Baseline/summary.json) |
| First correction | 11 | 1 | 0 | [Summary](QA/CurrentContent/summary.json) |
| Final combined verification | 88 | 0 | 0 | [Summary](QA/Combined/summary.json), [NUnit XML](QA/Combined/results.xml) |

Runs overlap; counts must not be added as distinct tests. The intermediate failure was an incomplete fixture correction: the board presentation test still restricted selection to round 10. Final source unlocks the disposable fixture through round 500 and asserts the selected round before inspecting it. The presentation test allows ten minutes for the expanded 500-board scope; actual combined execution took about 195 seconds, 225 seconds including Editor startup/shutdown.

## Verified behavior

- All **500 installed rounds** open with their authored goal counts, level identity, zero starting score, obstacle stage sprites and collectibles. Removed obstacle numerals remain absent. All 500 PreLevel presentations retain authored moves/rewards and render without changing the saved state; first-clear versus replay reward and booster inventory assertions remain.
- **6,000 board starts** (12 seeds per round) preserve authored fixed cells, have no initial matches and offer a legal move under the current production rules. This is opening-state coverage, not completion, balance or human difficulty acceptance.
- All **57 Chapter 1 milestones** render their actual scene lines, correct supported speaker portraits, Russian/English text, and expected return routes. Chapter Complete follows the round-500 finale, not round 10. Scene line counts and completion recording are asserted.
- Native World Map cards dispatch navigation once. Native booster catalog/close buttons preserve modal blocking throughout open/close transitions. No invisible hotspots were restored. World Map reports the installed **1,500-star** chapter capacity.
- Current line-item artwork remains whole, oriented correctly and readable by the existing phone-size geometry checks. Intact and damaged obstacle stages use distinct approved sprites; no new assets were made.
- The historical ten-round finale and candidate 20/30/40-round boundaries remain explicitly tested using their own retained content. Candidate completion messages identify the final tested round, and unavailable next rounds remain rejected. Historical fixtures do not replace the installed 500-round finale.
- Chapter Complete's comparison permits only monotonic maintenance of the full-energy clock. Energy amount, rewards, progress, inventory and acknowledgement still must match the complete saved snapshot. Existing idempotency/finale tests passed together with these checks.
- The combined run also retains R1 board/input/motion coverage, campaign board/mechanics integration and failed-round recovery.

## Changes and execution

Ten test files changed; **no production runtime code, art, prefab, campaign data, backend, package, project setting, guard or APK changed**. [Exact file scope](QA/Scope.json): 10 changed, 0 added/deleted, 7,775 unchanged under Assets/Packages/ProjectSettings/Backend. [Task-only patch](Sources/TaskChanges.patch) and before/after snapshots preserve the pre-existing working tree. Source whitespace checks passed.

Unity 6000.5.9f1, previously approved process-only Vulkan, separate disposable XDG/loading-test roots for each run. Existing machine entitlement was validated through a link to only the Unity licensing directory; no license contents were inspected/copied and normal product save paths were not shared. [Runner](Sources/Verify.py), filters and full commands are retained. No full-project Play Mode sweep, Android build or device execution occurred.

## Capture inspection and limits

Inspected actual Unity captures: [round-500 dialogue at 576×1280](QA/CurrentContent/Captures/LOR_DIALOGUE_CAPTURE_DIR/Dialogue_Finale_576x1280_safe_EN.png) and [line-item orientation fixture at 438×780](QA/Combined/Captures/LOR_GAMEPLAY_CAPTURE_DIR/Gameplay_Specials_438x780_RU.png). The dialogue text/button fit their panel; line items are distinct from ordinary gems and preserve direction. The item screenshot uses an explicit snapshot fixture (50 moves and 999 goals), not a normal-round balance example. These inspections do not establish full visual, animation or physical-phone acceptance. Additional automated captures remain under each run's Captures directory.

## Remaining work

[Historical failure overlay](QA/RemainingHistoricalFailures.json): **35 of R43's 40 cases now pass in this combined run; five legacy VFX cases remain unrerun.** They are historical evidence, not five freshly confirmed current defects. Next reconcile their compatibility expectations with the current approved presentation without restoring rejected circles or redesigning effects, then run applicable combined coverage before the offline Android journey.

Actual Android gameplay/lifecycle/performance, human difficulty and story pacing for unverified rounds 31–500, broader runtime coverage, phone readability and optional returning-player recap remain open. Backend completion and hosted/live commercial/social acceptance remain deferred.
