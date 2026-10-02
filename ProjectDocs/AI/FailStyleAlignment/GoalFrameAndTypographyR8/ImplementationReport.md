# Failure R8 — goal frame and typography

Implemented the approved refinement in the canonical Failure prefab and owning runtime/builder at revision 8. [Open the actual Unity review](Fail_GoalFrameAndTypography_R8_Review.html).

## What changed

- Replaced the visually heavy double enclosure with a narrower, quieter outer frame. A separate 570×44 heading plaque crosses its top rail. The live heading fits inside the plaque, and the plaque clears the goal inset.
- Added an opt-in protected-detail scale to the existing Failure panel mesh fitter: outer goal frame 0.5, inner frame 0.78, heading plaque 0.65. All other panels retain the default 1. This renders the complete existing sprites with smaller border/corner details; it does not crop or edit their pixels.
- Set the existing title font to bold and reduced its bevel from 1.2 to 0.85 reference units, producing a stronger, cleaner gold face within the accepted title dimensions.
- Set the goal heading to bold 28px. Increased both availability labels to bold 19px (previously 17/16px), with separate 24px-high text regions.
- Gave booster names and the revised small labels a pale ivory-gold face and compact dark shadow. Booster names retain their 20px size, wording and line breaks; no font substitution or horizontal glyph stretching.

Preserved the original main panel, title-panel artwork and 691×240 dimensions, Ivan, background, header, retry art, R7 offer shapes/baselines, booster art/quantities, chapter-map placement and native actions. No new raster resources, generation or image edits. Paid/ad offers remain unavailable; their behavior is scheduled after UI completion in FAIL-CONT-01.

## Changed production files

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailPanelFit.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/FailAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/FailAlignmentEditModeTests.cs`

See `AppliedSourceChanges.diff` and `QA/SourceAudit.json`. No new production files. Of 1,827 baseline files, 1,820 retain their hashes. All 944 .meta files, 504 prior prefab object IDs and 93 R7 evidence hashes remain intact. The new plaque adds five serialized objects/components, giving 509 total. Earlier unrelated local changes were preserved.

## Checks performed

- Unity 6000.5.9f1 compile/import and Failure-only rebuild/save/readback succeeded.
- Initial EditMode run: 9 passed, 1 failed. The new repeated-refresh assertion caught the plaque moving above the live heading. Fixed sibling ordering; initial logs/XML are retained under `UnityVerification/InitialRun`. PlayMode did not run in that first attempt.
- Final run: **10/10 EditMode and 18/18 PlayMode passed**, zero failures/skips, with isolated test save/config directories. Filters cover Failure, Victory and Pre-Level. Added assertions verify plaque/heading containment, clearance from the goal inset and stable repeated-refresh draw order. Existing text-fit, safe-area, hit-target, route, goal-data, offer-alignment and prefab-reuse checks passed.
- Saved 9 Failure, 12 Victory and 20 Pre-Level PNG/JSON pairs. Visually inspected final 941RU, 576safeRU, three-goal576EN and no-resultEN captures. Other captures were generated but not individually visually reviewed.
- R7/R8 RU comparison: 186,808 changed pixels. Header and scene above y608 are identical. Five pixels outside the selected title/goal/small-label regions differ by at most one channel value. No exact outside-region byte-identity claim. See `QA/VisualScope.json`.
- Source whitespace, runner syntax, review image/link targets, and final production/evidence hashes checked.

## Not performed / remaining limits

No browser rendering/dropdown interaction, APK/AAB build, physical Android check, full project suite, manual gameplay/loss/animation or prolonged-play test, profiling, live ad/payment check, exact-font identification or calibrated color/blur measurement. The existing font family approximates the reference; bold styling is not proof of the original typeface. Long booster names remain small on narrow phone displays. Reference ornament shapes, offer faces and character/material details still differ.

Next: review R8. The next separate visual pass can refine offer-card green-face proportions and border details, followed by physical-device readability checks. Preserve the accepted main/title layout and keep continuation integration in its scheduled feature stage.
