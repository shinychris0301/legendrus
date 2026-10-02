# Gameplay R11 — original board rails and decorative joints

The user selected continued Gameplay board refinement. The native shared grid now uses horizontal and vertical metal rails and a rounded gold junction extracted from unobstructed regions of the original 077 reference. It replaces the previous thin geometric grid while retaining the 8×7 board, outer frame, source gems, gameplay rules/input, R10 header scenery and R8 booster panels.

## Asset provenance

`Sources/PrepareAssets.py` creates the 128×128 RGBA atlas at `Assets/LegendsOfRus/Resources/ProductionUI/GameplaySourceGridR11/grid.png`. Original RGB is unchanged. Narrow alpha masks isolate the rails and junction, retaining the intentional dark metal recess. Source boxes and exact RGB checks are recorded in `Sources/AssetProvenance.json` and `QA/SourceGridChecks.json`. All three pieces were visually inspected over black, white and saturated magenta. No Scenario generation or extraction request was needed for these clear source details: zero new calls or credits.

## Native integration

`ProductionGameplayCell` renders the atlas in one shared grid: 127 rail segments and 72 joints, with joints above rails. `Match3Board` supplies the sprite; the grid does not receive raycasts. `ProductionGameplaySourceControls` owns the resource path and import inclusion. The Gameplay view/builder and active serialized prefab agree on revision 22. Missing grid artwork retains the previous geometric fallback.

Seven existing Assets files changed and three were added, with zero deletions. All 405 prefab records remain. Exact paths and hashes are in `QA/ScopeAndVerification.json` and `Sources/BeforeAssetsHashes.json`; all previously existing artwork and metadata retained their hashes.

## Verification

- Unity 6000.5.9f1 Build01 compiled, imported, rebuilt and read back the actual Gameplay prefab at revision 22; exit 0.
- Verify01 passed 9 Edit Mode and 32 Play Mode checks, zero failures or skips. Coverage includes Gameplay, Pre-Level and existing booster/VFX regressions. The existing small-phone check verifies all internal junctions remain visible. The special fixture additionally verifies that the native grid binds the extracted atlas.
- Actual Unity captures inspected at 941×1672, 576×1280 with simulated safe area and 438×780 with controlled specials. Other RU/EN, objectives, obstacles, score and earned-star fixtures are available in the review.
- Source RGB checks passed for all three atlas regions. Comparing the default native R10 and R11 captures found 90,728 changed pixels, confined to bounds (58,474)–(886,1246), within the board area. Header, scenery outside the board and booster panels are pixel-identical in that comparison.
- Browser review at 390×844 loaded all 19 choices without JavaScript errors, missing images or horizontal overflow. Scoped `git diff --check` passed.

`Gameplay_SourceGrid_R11_Review.html` defaults to a standalone actual Unity portrait. Enlarged detail images are explicitly labeled comparison aids. Test captures use isolated saves, and special/large-value captures are fixtures.

## Remaining limits

This is closer to the original metal style, not a pixel-identical reconstruction of the whole screenshot: grid spacing, live gem arrangement, intentional gameplay values and repeated source rail texture differ. Source joints are small raster details and appear soft when enlarged. Existing desktop Liberation Serif development fallback remains; exact original typeface and Android font binding are unresolved. No Android build, physical-device run or full project regression suite was performed.

Next action: review R11 in the default native view and the board-detail choices. User visual acceptance remains pending.
