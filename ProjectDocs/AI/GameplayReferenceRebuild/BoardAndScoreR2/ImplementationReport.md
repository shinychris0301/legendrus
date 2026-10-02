# Gameplay R2 — board, score and responsive scenery

Implemented the approved fidelity-review refinement in the canonical Gameplay prefab, revision13. [Standalone review](Gameplay_BoardAndScore_R2_Review.html) contains actual Unity captures at the reference aspect and simulated tall-phone safe areas, plus R1 and the original for comparison.

## Changes

- Replaced the duplicated lower skyline with one continuous riverbank, water and foreground. Scenario reconstructed the lower portion of the reference; the existing upper400rows remain unchanged, with a400–455px blend into the repair. The tall capture now exposes water beneath a single city instead of a second skyline. This reconstruction is inferred scenery, not exact hidden original pixels.
- Reduced the strong horizontal wood grain. Cell faces now have quieter, darker material; grid rails use subdued antique gold and intersections use lobed outlines around small beveled centers. The grid remains native8×7, with the original full-cell input areas.
- Extracted filled and empty stars directly from the supplied reference through Scenario. Restored original RGB under the alpha, removed disconnected rail fragments and retained the intentionally opaque dark center of the empty star. Replaced the thin native star treatment only on Gameplay. Earned-state changes refresh both the mesh and texture.
- Rounded the live progress meter's end caps and simplified the reconstructed score-frame bottom rail behind the stars.
- Added padding around the existing booster artwork and increased caption size from16to17reference pixels, with tighter line spacing to keep the approved long names inside their label wells.

The six regular gem PNGs are byte-for-byte unchanged. Existing character, booster artwork/names/functions, score accounting, staged missions, hidden nameplate, obstacles, reshuffle and effects remain intact. No changes to the saved progression or economy.

## Asset workflow

Three accepted Scenario jobs, **18recorded CU**: two star isolations at3CU each and one bounded scenery repair at12CU. No repeated submissions. Requests, outputs and billing are under Sources/Scenario; [AssetProvenance.json](Sources/AssetProvenance.json) records final hashes and repairs. New resources live in GameplayRefinedR2; R1resources remain untouched.

The star and score-panel alpha was inspected over black, white and magenta: [transparency sheet](QA/Alpha.jpg). The complete environment plate was inspected before import, then at941×1672and576×1280in actual Unity captures. Source enlargement does not add detail.

## Implementation and scope

ProductionGameplaySourceControls selects five new versioned sprites and owns the revised layout. Its editor builder imports the same assets and rebuilds the actual prefab at revision13. ProductionWorldMapGraphic has optional source-star slots: Gameplay assigns them; other pages retain native rendering with null slots. The existing Gameplay star polish effect skips source-backed stars. ScoreMeter retains the same live fraction and changes only its drawing geometry. ProductionGameplayCell retains its grid/input structure and changes its rail/joint rendering.

[Scope audit](QA/ScopeAndVerification.json):10existing Assets files modified,11new files,0deleted; all395prefab records retained. Only Gameplay.prefab changed. No existing PNG or.meta file changed, including all six gems: [hash check](QA/UnchangedGems.json).

## Verification

Unity6000.5.9f1 Build02 compiled, rebuilt and read back revision13. Final Verify02: **9EditMode +30PlayMode =39passed**,0failed/skipped. Coverage includes source-star bindings and native fallback on other pages, Gameplay/Pre-Level layout, RU/EN captions, safe areas, zero starting score, authored missions/obstacles, twelve resolved moves, earned stars, special boosters, reshuffle, input raycasts, inventory transactions and VFX cleanup.

Verify01 caught a Russian booster-caption glyph extending0.75px below its label bounds. Increasing the well height and reducing line spacing corrected it; the same glyph-bound checks pass in Verify02. Diagnostic logs/results are retained.

The review's10image choices load at390px browser width without script errors or horizontal overflow. Tests are targeted; no full-suite, Android build, physical-device or GPU-performance claims.

## Remaining fidelity limits

This is closer to the original, not pixel-exact. Live typography still approximates the original letterforms, weight and shading. Native grid-joint shapes approximate the small painted ornaments. The lower scenery and score-frame repair are reconstructions. Existing Ivan and booster artwork/names intentionally differ from the illustration. Long booster names still use two lines rather than the reference's shorter captions. Current mission counts, unearned/earned stars and score reflect actual game state.
