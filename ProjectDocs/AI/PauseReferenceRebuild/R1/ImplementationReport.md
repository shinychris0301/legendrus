# Pause reference rebuild R1

Native revision **2**, delivered for visual review on 2026-09-21. User approved continuing to the next screen after Failure. Visual Page 10, Pause, follows Failure in the established sequence.

[Standalone review](Pause_ReferenceRebuild_R1_Review.html) · [scope and verification](QA/ScopeAndVerification.json) · [asset provenance](Sources/AssetProvenance.json).

## Changes

The main modal now has continuous navy lacquer in place of the old repeated narrow texture. Its outer gold carving uses original pixels with a Scenario-derived silhouette. The attached title fragment is removed from the main-panel alpha; the existing separate original title renders once. Original Russian title and four action-button assets remain unchanged.

The goal frame now contains only the original thin gold border, so its interior shows the continuous main-panel surface. Russian goals/boosters headings, flourishes and instructions are extracted directly from the source, without a rectangular navy backing. English keeps live localizable captions with separate source flourishes. Missing-round instructions remain live; original instruction artwork is hidden in that state.

Booster silhouettes were extracted together through Scenario, then separated into the four original cards. Intentionally opaque navy faces are retained. Existing empty quantity faces are reused and actual inventory counts stay live. Goals still support two or three objectives with actual remaining amounts below icons.

Pause remains a native modal over the actual current Gameplay screen. It inherits the visible Gameplay header underneath the dimmer; it does not add another resource header or substitute the reference's old board. The four existing Resume, Restart, Settings and Exit actions and the board lifecycle are preserved.

## Asset workflow

Reference: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Pause.png`, SHA256 `31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716`.

Three accepted Scenario jobs:

- Main panel alpha: **3 CU**.
- Entire booster-row alpha: **3 CU**.
- Obscured panel-surface reconstruction: **13 CU**.

**19 CU total**, no paid resubmissions. The generated surface contributes only inside a recorded repair mask. The original frame, lettering, borders and booster artwork are retained outside their explicitly documented quantity/interior repairs. Preparation scripts, input crops, raw outputs, numeric billing and masks are archived in `Sources`.

Transparent sprites were inspected on black, white and saturated magenta. Russian and English native captures, including a tall three-goal layout, were inspected. Cleared material and fine alpha edges remain reconstructions; exact source-font identity is not claimed.

## Verification

Unity **6000.5.9f1** compiled and rebuilt only the active Pause prefab, with revision-2 save/readback. Final **Verify02: 12 Edit Mode + 18 Play Mode = 30 passed**, zero failures/skips, covering Pause plus the selected Gameplay and Failure fixtures.

Checks include native hit targets, safe-area bounds, text containment, clearance from goal/booster icons, RU/EN artwork switching, no duplicate objects, truthful missing-round state, actual stock/goals, input blocking, Resume preserving board/moves/score, Settings returning to the same paused board, Restart starting a fresh zero-score round, Exit routing to Chapter Map and duplicate-action prevention.

Baseline01 and Verify01 are retained. Verify01's automated checks passed but visual inspection found doubled English caption transforms. The scale was corrected and tests strengthened to check actual transformed bounds against the panel and live content; final Verify02 passed and its English captures were inspected.

The review's **8 choices** loaded in Chrome with zero JavaScript errors and no horizontal overflow. All native images use isolated test saves. No user save was used.

Not run: Android build, physical-device testing, full-project suite, extended manual play or performance profiling. Passing these checks is not visual identity or device acceptance.

## Changed files and preservation

Six existing Assets files changed:

- `Scripts/Runtime/UI/ProductionPauseRuntime.cs`
- `Scripts/Runtime/UI/ProductionPauseView.cs`
- `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Resources/ProductionUI/Pages/Pause.prefab`
- `Tests/ProductionCoreLoopUiEditMode/PauseAlignmentEditModeTests.cs`
- `Tests/PlayMode/PauseAlignmentPlayModeTests.cs`

Eleven versioned sprites and their metadata were added under `Resources/ProductionUI/PauseAligned`. Existing sprites and metadata retain their hashes. All **141 prior prefab records** remain, with **153 total** after adding the three decorative image layers. No Assets deleted. Gameplay, Failure, Victory, other page prefabs and shared-header implementation retain their before-task hashes.

## Remaining differences / next action

The actual paused Gameplay intentionally differs from the illustrative old board/header behind the reference. Live goals and inventory reflect gameplay rather than sample amounts. Counts remain below icons for two/three-goal clarity. Exact font identity and hidden material cannot be recovered from the flattened reference; existing English blank title/button faces still show some repeated repair texture. Review this Pause pass before further refinement or moving to Chapter Complete.
