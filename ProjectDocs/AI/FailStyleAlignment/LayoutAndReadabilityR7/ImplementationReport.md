# Failure R7 — layout and readability

Implemented the four approved R6 review recommendations in the canonical Failure prefab and its owning builder/runtime (revision 7). [Open actual Unity portraits](Fail_LayoutAndReadability_R7_Review.html).

## Changes

- Moved the existing chapter-map action into the upper-right area of the booster section. The lower center sapphire and surrounding scrollwork are visible again. The original action binding and wording remain.
- Aligned the whole red/blue offer sprites at native aspect ratio, accounting for the blue card's taller crest. Estimated upper painted faces differ by0.60px and green-face centers by0.10px at941px width. These are measurements from manually identified source landmarks, not a pixel-exactness claim. Ad title, unavailable status and play symbol occupy separate vertical spaces.
- Added an outer goal frame enclosing the heading and reduced the independent inner frame from643×185 to603×179. Actual two/three-goal icons and counts remain centered and data-driven.
- Raised/rebalanced the booster section; enlarged medallions166→176px and illustration bounds134→140px. Nameplates are194×56, with live20px labels in188×48 bounds (previous19px in176×43). Current names, artwork, badge counts and functions remain. Selecting a booster uses the heading area for its explanation, without covering the return action or adding a second crowded text row.

Preserved the original main-panel art and891×1005 geometry, title art and691×240 geometry, scene, Ivan, header, retry artwork, fonts, colors and existing routes. No raster generation, cropping or raster alteration. Paid/ad controls remain disabled with unavailable status and no invented price; behavior is still scheduled after UI completion in [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md).

## Exact production changes

1. `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab`
2. `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
3. `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailRuntime.cs`
4. `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailView.cs`
5. `Assets/LegendsOfRus/Tests/PlayMode/FailAlignmentPlayModeTests.cs`
6. `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/FailAlignmentEditModeTests.cs`

The existing-file diff is in `AppliedSourceChanges.diff`. No new production files. The added nested panel creates five serialized objects/components in the existing prefab; all 499 previous object IDs remain. All 944 baseline .meta files and all 98 R6 evidence files retain their hashes. Of 1,827 baseline production files,1,821 are unchanged. This excludes unrelated pre-existing working-tree changes from the R7 patch.

## Verification performed

- Unity 6000.5.9f1 compile/import and Failure-only builder save/reimport/readback: exit0.
- 10/10 EditMode and18/18 PlayMode tests passed, zero failures/skips, using the existing Failure/Victory/Pre-Level test filters and isolated temporary save/config directories.
- Extended existing layout checks cover footer-carving clearance, chapter-map separation from heading/booster controls, approximate painted offer landmarks, native offer aspect, ad title/status separation and goal inset containment. Existing checks cover safe-area bounds, button-center raycasts, text fit, actual goal data, native navigation and prefab reuse.
- Retained 9 Failure,12 Victory and20 Pre-Level PNG/JSON pairs plus test XML, commands and logs. Visually inspected four final Failure captures:941RU,576safeRU, three-goal576EN and booster-guideRU, against original reference and R6. Other captures were generated/tested but not individually visually reviewed.
- R6/R7 RU pixel comparison found 467,513 changed pixels. All substantive changes fall within the goal and lower-control regions. Eleven isolated title/retry pixels outside those regions differ by at most one channel value; therefore exact outside-region byte identity is **not** claimed. The first strict scope assertion failed because its footer mask excluded the final changed row and it demanded zero outside differences; the corrected diagnostic retains these exceptions. See `QA/VisualScopeAndOfferGeometry.json`.
- Checked source whitespace, runner syntax, review image/link targets and production/evidence hashes. No source font replacement or exact-font claim.

## Not performed / limitations

No browser rendering/dropdown interaction, APK/AAB build, physical Android run, full project test suite, manual gameplay/loss/animation/prolonged-play test, performance profiling, live ad/payment verification or calibrated color/font comparison. The simulated576px portraits still have small availability and long booster labels; physical-device readability needs review. Main/character/material/font details remain different from the original reference. This is a focused layout correction, not certification of a pixel-identical page.

Next: review the R7 portraits. If further visual refinement is needed, target panel ornament/typography details separately; preserve the title/main-panel decisions. Continuation integration remains a later feature stage.
